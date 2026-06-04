<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'
import axios from 'axios'
import { Alertas } from '@/utils/alertas';

const props = defineProps<{
    visible: boolean,
    codigo: string,
    ordenes: any[]
}>()

const emit = defineEmits(['close', 'actualizar-lista'])

const apiUrl = import.meta.env.VITE_API_URL || 'http://127.0.0.1:5122/api';

const procesando = ref(false);
const consumosMezcla = ref<{ materiaPrimaId: number, nombre: string, teorico: number, real: number }[]>([])

// Variables para cargar insumos extras
const todasMateriasPrimas = ref<any[]>([]);
const insumoExtraSeleccionado = ref<number | ''>('');

const yaEstaDeclarado = computed(() => {
    if (props.ordenes.length === 0) return false;
    return props.ordenes.some(o => o.estado === 'MaterialPreparado' || o.estado === 'Finalizada');
});

const hojaCargaId = computed(() => {
    if (props.ordenes.length === 0) return null;
    return props.ordenes[0].hojaCargaId;
});

const idClienteUnico = computed(() => {
    if (props.ordenes.length === 0) return null;
    const primerId = props.ordenes[0].clienteId || props.ordenes[0].ClienteId;
    
    if (!primerId || primerId <= 1) return null;

    const todosIguales = props.ordenes.every(o => (o.clienteId || o.ClienteId) === primerId);
    return todosIguales ? primerId : null;
});

// 🚀 AGRUPACIÓN Y ORDENAMIENTO INTELIGENTE
const materiasPrimasAgrupadas = computed(() => {
    const idClienteHabilitado = idClienteUnico.value;
    
    const grupos = {
        estruplast: { label: '🏢 ESTRUPLAST (MATERIAL PROPIO)', items: [] as any[] },
        cliente: { label: '', items: [] as any[] }
    };

    todasMateriasPrimas.value.forEach((p: any) => {
        const esPropio = !p.clienteId || p.clienteId <= 1;
        const esDelCliente = idClienteHabilitado !== null && p.clienteId === idClienteHabilitado;

        const materialValido = p.esMateriaPrima && (esPropio || esDelCliente);
        const estaActivo = p.activo !== false && p.estado !== 'Inactivo'; 
        const noEsGenerico = !p.esGenerico; 
        const nombreLimpio = (p.nombre || '').toUpperCase().trim();
        const noEsBase = !nombreLimpio.includes('BASE');

        if (materialValido && estaActivo && noEsGenerico && noEsBase) {
            if (esPropio) {
                grupos.estruplast.items.push(p);
            } else if (esDelCliente) {
                // Capturamos la razón social real desde la orden o el producto
                if (!grupos.cliente.label) {
                    const orden = props.ordenes.find(o => (o.clienteId || o.ClienteId) === p.clienteId);
                    let nombreCliente = orden?.clienteNombre || orden?.ClienteNombre || p.cliente?.razonSocial || p.clienteNombre || `CLIENTE #${p.clienteId}`;
                    grupos.cliente.label = `👤 PROPIEDAD DE: ${nombreCliente.toUpperCase()}`;
                }
                grupos.cliente.items.push(p);
            }
        }
    });

    // Ordenamos alfabéticamente dentro de cada grupo
    grupos.estruplast.items.sort((a, b) => a.nombre.localeCompare(b.nombre));
    grupos.cliente.items.sort((a, b) => a.nombre.localeCompare(b.nombre));

    const resultado = [];
    if (grupos.estruplast.items.length > 0) resultado.push(grupos.estruplast);
    if (grupos.cliente.items.length > 0) resultado.push(grupos.cliente);

    return resultado;
});

const cargarCatálogoMateriales = async () => {
    try {
        const { data } = await axios.get(`${apiUrl}/Productos`);
        todasMateriasPrimas.value = data; 
    } catch (error) {
        console.error("Error cargando insumos", error);
    }
};

onMounted(() => {
    cargarCatálogoMateriales();
});

watch(() => props.visible, (isOpen) => {
    if (isOpen && props.ordenes.length > 0) {
        const map = new Map<number, any>();
        
        props.ordenes.forEach(o => {
            if (o.consumos) {
                o.consumos.forEach((c: any) => {
                    if (!map.has(c.materiaPrimaId)) {
                        map.set(c.materiaPrimaId, { 
                            materiaPrimaId: c.materiaPrimaId, 
                            nombre: c.nombreMateriaPrima, 
                            teorico: 0, 
                            real: 0 
                        });
                    }
                    map.get(c.materiaPrimaId).teorico += Number(c.cantidadKilos);
                });
            }
        });
        
        consumosMezcla.value = Array.from(map.values()).map(c => {
            c.real = Number(c.teorico.toFixed(2));
            return c;
        });

        insumoExtraSeleccionado.value = ''; 
    }
});

const agregarFilaExtra = () => {
    if (!insumoExtraSeleccionado.value) return;

    // Buscamos en la lista completa
    const mp = todasMateriasPrimas.value.find(m => m.id === insumoExtraSeleccionado.value);
    if (!mp) return;

    const yaExiste = consumosMezcla.value.find(c => c.materiaPrimaId === mp.id);
    if (yaExiste) {
        Alertas.advertencia(`El insumo "${mp.nombre}" ya está en la lista. Modificá los kilos en su renglón correspondiente.`);
        insumoExtraSeleccionado.value = '';
        return;
    }

    consumosMezcla.value.push({
        materiaPrimaId: mp.id,
        nombre: mp.nombre + " (Extra)",
        teorico: 0,
        real: 0
    });

    insumoExtraSeleccionado.value = ''; 
};

const quitarInsumo = (index: number) => {
    consumosMezcla.value.splice(index, 1);
};

const declararConsumos = async () => {
    if (!hojaCargaId.value) {
        Alertas.error("Error crítico: La orden no tiene un HojaCargaId válido.");
        return;
    }

   const confirmado = await Alertas.confirmar(
    "Confirmar Descuento",
    "⚠️ ¿Descontar estos materiales del stock?\n\nLas órdenes de este grupo pasarán a 'Material Preparado' y ya no descontarán material base al cerrarse."
    );

    if (!confirmado) return;
    procesando.value = true;
    try {
        const payload = consumosMezcla.value
            .filter(c => Number(c.real) > 0) 
            .map(c => ({
                materiaPrimaId: c.materiaPrimaId,
                cantidadRealKg: Number(c.real)
            }));

        await axios.post(`${apiUrl}/HojasCarga/${hojaCargaId.value}/declarar-consumos`, payload);
        
        Alertas.exito("✅ Mezcla declarada correctamente.");
        emit('actualizar-lista'); 
        emit('close');
    } catch (e: any) {
        Alertas.error("❌ Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        procesando.value = false;
    }
}
</script>

<template>
    <div v-if="visible" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>📦 Hoja de Carga: {{ codigo }}</h3>
                <button class="btn-close" @click="$emit('close')">×</button>
            </div>
            
            <div class="modal-body">
                <div v-if="yaEstaDeclarado" class="alerta-ok">
                    ✅ <strong>El material de este grupo ya fue descontado del inventario.</strong><br>
                    Las órdenes están listas para cerrarse a medida que salgan de la máquina.
                </div>
                
                <div v-else class="alerta-info">
                    ℹ️ <strong>Declaración de Pastón / Mezcla (Fase 1)</strong><br>
                    Cargue los kilos exactos del papel del maquinista. Si usaron algo que no estaba previsto, agréguelo abajo.
                </div>

                <div class="seccion" v-if="!yaEstaDeclarado">
                    <h4>⚖️ Consumos del Grupo Completo</h4>
                    <div class="tabla-container">
                        <table>
                            <thead>
                                <tr>
                                    <th>Insumo</th>
                                    <th class="text-center">Suma Teórica (Kg)</th>
                                    <th>Consumo Real (Kg)</th>
                                    <th style="width: 40px; text-align: center;"></th> </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, idx) in consumosMezcla" :key="idx">
                                    <td>{{ item.nombre }}</td>
                                    <td class="text-center" style="color: #7f8c8d;">
                                        {{ item.teorico > 0 ? item.teorico.toFixed(2) : '---' }}
                                    </td>
                                    <td>
                                        <input type="number" v-model="item.real" style="width: 100px; padding: 5px; font-weight: bold;" step="0.1" min="0">
                                    </td>
                                    <td style="text-align: center;">
                                        <button class="btn-quitar" @click="quitarInsumo(idx)" title="Quitar insumo">❌</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        
                        <div class="barra-agregar-extra" style="margin-top: 15px; padding-top: 15px; border-top: 1px dashed #cbd5e1; display: flex; flex-wrap: wrap; gap: 10px; align-items: center;">
                            <label style="font-weight: bold; color: #34495e;">➕ Agregar Extra:</label>
                            
                            <select v-model="insumoExtraSeleccionado" class="select-lindo-agrupado">
                                <option value="">Seleccione un insumo no planificado...</option>
                                <optgroup v-for="grupo in materiasPrimasAgrupadas" :key="grupo.label" :label="grupo.label">
                                    <option v-for="mp in grupo.items" :key="mp.id" :value="mp.id">
                                        {{ mp.nombre }}
                                    </option>
                                </optgroup>
                            </select>
                            
                            <button class="btn-orden" style="padding: 6px 15px; white-space: nowrap;" @click="agregarFilaExtra" :disabled="!insumoExtraSeleccionado">
                                Añadir
                            </button>
                        </div>
                    </div>
                </div>

                <div class="seccion">
                    <h4>📄 Órdenes incluidas en este grupo ({{ ordenes.length }})</h4>
                    <ul class="lista-ordenes">
                        <li v-for="o in ordenes" :key="o.id">
                            <strong>OP #{{ o.id }}</strong> - {{ o.producto }} ({{ o.kilos }} Kg) 
                            <span :class="['badge', 'badge-' + o.estado.toLowerCase()]">{{ o.estado }}</span>
                        </li>
                    </ul>
                </div>
            </div>
            
            <div class="modal-footer">
                <button class="btn-cancelar" @click="$emit('close')">Cerrar</button>
                <button v-if="!yaEstaDeclarado" class="btn-confirmar" @click="declararConsumos" :disabled="procesando">
                    {{ procesando ? '⏳ Procesando...' : '✅ Declarar Consumos de Mezcla' }}
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 20px; border-radius: 12px; width: 650px; max-width: 95vw; max-height: 90vh; overflow-y: auto; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #3498db; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #2c3e50; }
.btn-close { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #7f8c8d; }
.alerta-info { background: #ebf5fb; border-left: 4px solid #3498db; padding: 12px; margin-bottom: 15px; border-radius: 4px; color: #2980b9; font-size: 0.9rem; }
.alerta-ok { background: #eafaf1; border-left: 4px solid #2ecc71; padding: 12px; margin-bottom: 15px; border-radius: 4px; color: #27ae60; font-size: 0.9rem; }
.seccion { margin-bottom: 20px; }
.seccion h4 { color: #34495e; border-bottom: 1px dashed #bdc3c7; padding-bottom: 5px; margin-bottom: 10px; }
.tabla-container { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 15px; }
table { width: 100%; border-collapse: collapse; }
th { text-align: left; padding: 8px; border-bottom: 2px solid #cbd5e1; color: #475569; font-size: 0.85rem; }
td { padding: 8px; border-bottom: 1px solid #e2e8f0; vertical-align: middle; }
.text-center { text-align: center; }

.btn-quitar { background: transparent; border: none; cursor: pointer; font-size: 1rem; transition: transform 0.2s; padding: 0; }
.btn-quitar:hover { transform: scale(1.2); }

.lista-ordenes { list-style: none; padding: 0; margin: 0; }
.lista-ordenes li { background: #f8fafc; padding: 8px 12px; margin-bottom: 5px; border-radius: 6px; border: 1px solid #e2e8f0; font-size: 0.9rem; display: flex; justify-content: space-between; align-items: center;}
.modal-footer { display: flex; justify-content: flex-end; gap: 10px; border-top: 1px solid #ecf0f1; padding-top: 15px; }
.btn-cancelar { background: #95a5a6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; }
.btn-confirmar { background: #27ae60; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; }
.btn-confirmar:disabled { background: #bdc3c7; cursor: not-allowed; }
.badge { padding: 3px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: bold; }
.badge-pendiente { background: #fff7ed; color: #d97706; border: 1px solid #fcd34d; }
.badge-materialpreparado { background: #eff6ff; color: #3b82f6; border: 1px solid #93c5fd; }
.badge-finalizada { background: #ecfdf5; color: #10b981; border: 1px solid #a7f3d0; }
.btn-orden { background: #34495e; border: 1px solid #7f8c8d; color: white; cursor: pointer; border-radius: 4px; font-weight: bold; } 
.btn-orden:hover:not(:disabled) { background: #2980b9; }
.btn-orden:disabled { opacity: 0.5; cursor: not-allowed; }

/* 🚀 Estilos del nuevo select con marco lindo */
.select-lindo-agrupado {
    flex: 1 1 200px;
    min-width: 150px;
    padding: 8px 12px;
    border-radius: 6px;
    border: 2px solid #3498db;
    background-color: #f0f8ff;
    font-weight: bold;
    color: #2c3e50;
    outline: none;
    box-shadow: 0 2px 4px rgba(52, 152, 219, 0.15);
    transition: all 0.3s ease;
    cursor: pointer;
}
.select-lindo-agrupado:focus {
    border-color: #2980b9;
    box-shadow: 0 2px 8px rgba(41, 128, 185, 0.4);
    background-color: #ffffff;
}
.select-lindo-agrupado optgroup {
    font-weight: 900;
    color: #d35400;
    background-color: #ffffff;
    font-style: normal;
    padding: 5px;
}
.select-lindo-agrupado option {
    font-weight: 600;
    color: #34495e;
    padding: 6px;
    background-color: #ffffff;
}
</style>