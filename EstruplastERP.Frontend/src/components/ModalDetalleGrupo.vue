<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue'
// 🚀 ACÁ ESTABA EL ERROR: Ahora usamos TU instancia de API, no el axios crudo
import api from '@/services/axiosInstance' 
import { Alertas } from '@/utils/alertas';

const props = defineProps<{
    visible: boolean,
    codigo: string,
    ordenes: any[]
}>()

const emit = defineEmits(['close', 'actualizar-lista', 'imprimir-carga'])

const procesando = ref(false);
const consumosMezcla = ref<{ materiaPrimaId: number, nombre: string, teorico: number, real: number }[]>([])

const todasMateriasPrimas = ref<any[]>([]);
const insumoExtraSeleccionado = ref<number | ''>('');

const yaEstaDeclarado = computed(() => {
    if (props.ordenes.length === 0) return false;
    return props.ordenes.some(o => o.estado === 'MaterialPreparado' || o.estado === 'Finalizada');
});

const puedeRevertir = computed(() => {
    if (!yaEstaDeclarado.value) return false;
    return !props.ordenes.some(o => o.estado === 'Finalizada');
});

const esCargaSimple = computed(() => {
    return props.codigo && props.codigo.includes('HC-S');
});

const hojaCargaId = computed(() => {
    const ordenConId = props.ordenes.find(o => o.hojaCargaId);
    if (ordenConId) return ordenConId.hojaCargaId;
    
    if (props.codigo) {
        const match = props.codigo.match(/HC-(\d+)/i);
        if (match && match[1]) {
            return parseInt(match[1], 10);
        }
    }
    return null;
});

const idClienteUnico = computed(() => {
    if (props.ordenes.length === 0) return null;
    const primerId = props.ordenes[0].clienteId || props.ordenes[0].ClienteId;
    
    if (!primerId || primerId <= 1) return null;

    const todosIguales = props.ordenes.every(o => (o.clienteId || o.ClienteId) === primerId);
    return todosIguales ? primerId : null;
});

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
                if (!grupos.cliente.label) {
                    const orden = props.ordenes.find(o => (o.clienteId || o.ClienteId) === p.clienteId);
                    let nombreCliente = orden?.clienteNombre || orden?.ClienteNombre || p.cliente?.razonSocial || p.clienteNombre || `CLIENTE #${p.clienteId}`;
                    grupos.cliente.label = `👤 PROPIEDAD DE: ${nombreCliente.toUpperCase()}`;
                }
                grupos.cliente.items.push(p);
            }
        }
    });

    grupos.estruplast.items.sort((a, b) => a.nombre.localeCompare(b.nombre));
    grupos.cliente.items.sort((a, b) => a.nombre.localeCompare(b.nombre));

    const resultado = [];
    if (grupos.estruplast.items.length > 0) resultado.push(grupos.estruplast);
    if (grupos.cliente.items.length > 0) resultado.push(grupos.cliente);

    return resultado;
});

const cargarCatálogoMateriales = async () => {
    try {
        const { data } = await api.get(`/Productos`);
        todasMateriasPrimas.value = data; 
    } catch (error) {
        console.error("Error cargando insumos", error);
    }
};

onMounted(() => {
    cargarCatálogoMateriales();
});

watch(() => props.visible, async (isOpen) => {
    if (isOpen && props.ordenes.length > 0) {
        consumosMezcla.value = [];
        procesando.value = true;
        
        try {
            const map = new Map<number, any>();

            // 1. Pedimos el detalle completo de CADA orden a la API configurada
            const peticiones = props.ordenes.map(async (o) => {
                let insumosOrden: any[] = [];
                try {
                    const resFull = await api.get(`/Ordenes/${o.id}`);
                    const ordenFull = resFull.data;
                    
                    if (ordenFull && ordenFull.consumos && ordenFull.consumos.length > 0) {
                        insumosOrden = ordenFull.consumos;
                    } 
                    else if (ordenFull && (ordenFull.receta || ordenFull.recetaDinamica)) {
                        insumosOrden = ordenFull.receta || ordenFull.recetaDinamica;
                    }
                    
                    if (yaEstaDeclarado.value && insumosOrden.length === 0) {
                        try {
                            const resCons = await api.get(`/Ordenes/${o.id}/consumos`);
                            if (resCons.data && resCons.data.length > 0) {
                                insumosOrden = resCons.data;
                            }
                        } catch (err) {}
                    }
                } catch (e) {
                    console.warn(`No se pudo traer el detalle histórico de la OP ${o.id}`);
                }
                return insumosOrden;
            });

            const resultadosInsumos = await Promise.all(peticiones);

            // 2. Sumamos todo en nuestra tablita local
            resultadosInsumos.forEach(arrayInsumos => {
                arrayInsumos.forEach((c: any) => {
                    const mId = c.materiaPrimaId || c.MateriaPrimaId || c.insumoId || c.id;
                    if (!mId) return;

                    const nombre = c.nombreMateriaPrima || c.nombreInsumo || c.nombre || 'Insumo';
                    const kilosReales = Number(c.real !== undefined ? c.real : (c.cantidadKilos || c.CantidadKilos || c.kilos || c.cantidad || 0));
                    const kilosTeoricos = Number(c.teorico !== undefined ? c.teorico : kilosReales);

                    if (!map.has(mId)) {
                        map.set(mId, { materiaPrimaId: mId, nombre, teorico: 0, real: 0 });
                    }

                    if (yaEstaDeclarado.value) {
                        map.get(mId).real += kilosReales;
                        map.get(mId).teorico += kilosTeoricos;
                    } else {
                        map.get(mId).teorico += kilosReales;
                    }
                });
            });

            // 3. Convertimos a la lista final
            const consumosList = Array.from(map.values()).map(c => {
                if (c.real === 0 && !yaEstaDeclarado.value) {
                    c.real = Number(c.teorico.toFixed(2));
                }
                return c;
            });

            consumosMezcla.value = consumosList;
            
        } catch (e) {
            console.error("Error armando los consumos históricos", e);
        } finally {
            procesando.value = false;
            insumoExtraSeleccionado.value = '';
        }
    }
});

const agregarFilaExtra = () => {
    if (!insumoExtraSeleccionado.value) return;

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
        Alertas.error("Error crítico: No se pudo determinar el ID de la Hoja de Carga.");
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

        await api.post(`/HojasCarga/${hojaCargaId.value}/declarar-consumos`, payload);
        
        Alertas.exito("✅ Mezcla declarada correctamente.");
        emit('actualizar-lista'); 
        emit('close');
    } catch (e: any) {
        Alertas.error("❌ Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        procesando.value = false;
    }
};

const revertirDeclaracion = async () => {
    const confirmado = await Alertas.confirmar(
        "⏪ Revertir Declaración",
        "Se devolverán los insumos de esta mezcla al stock y las órdenes regresarán a estado 'En Producción'.\n\n¿Estás seguro?"
    );

    if (!confirmado) return;
    
    if (!hojaCargaId.value) {
        Alertas.error("❌ No se pudo determinar el ID de la Hoja de Carga para revertir.");
        return;
    }

    procesando.value = true;

    try {
        await api.post(`/HojasCarga/${hojaCargaId.value}/revertir`);
        
        Alertas.exito("✅ Declaración revertida y stock devuelto correctamente.");
        emit('actualizar-lista');
        emit('close');
    } catch (e: any) {
        Alertas.error("❌ Error al revertir: " + (e.response?.data?.mensaje || e.message));
    } finally {
        procesando.value = false;
    }
};
</script>

<template>
    <div v-if="visible" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>📦 Hoja de Carga: {{ codigo }}</h3>
                <button class="btn-close" @click="$emit('close')">×</button>
            </div>
            
            <div class="modal-body">
                <div v-if="esCargaSimple && !yaEstaDeclarado" class="alerta-info" style="border-left-color: #94a3b8; color: #64748b; background: #f8fafc;">
                    👁️ <strong>Modo Lectura Activo</strong><br>
                    Esta es una Hoja de Carga Individual. El consumo final se declarará cuando cierres la OP.
                </div>

                <div v-else-if="yaEstaDeclarado" class="alerta-ok">
                    ✅ <strong>El material de {{ esCargaSimple ? 'esta orden' : 'este grupo' }} ya fue descontado del inventario.</strong><br>
                    Abajo podés ver el detalle exacto de lo que se consumió.
                </div>
                
                <div v-else class="alerta-info">
                    ℹ️ <strong>Declaración de Pastón / Mezcla (Fase 1)</strong><br>
                    Cargue los kilos exactos del papel del maquinista. Si usaron algo que no estaba previsto, agréguelo abajo.
                </div>

                <div v-if="procesando && consumosMezcla.length === 0" style="text-align: center; padding: 30px; font-weight: bold; color: #3498db;">
                    <div class="spinner-mini"></div>
                    ⏳ Recuperando datos históricos desde el servidor...
                </div>

                <div v-else-if="consumosMezcla.length > 0" class="seccion">
                    <h4>{{ yaEstaDeclarado ? '✅ Materiales Descontados del Stock' : '⚖️ Receta Base (Solo lectura)' }}</h4>
                    <div class="tabla-container">
                        <table>
                            <thead>
                                <tr>
                                    <th>Insumo</th>
                                    <th class="text-center" v-if="!yaEstaDeclarado">Suma Teórica (Kg)</th>
                                    <th :class="{'text-center': yaEstaDeclarado}">{{ yaEstaDeclarado ? 'Cantidad Descontada (Kg)' : 'Consumo Real (Kg)' }}</th>
                                    <th style="width: 40px; text-align: center;" v-if="!yaEstaDeclarado"></th> 
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, idx) in consumosMezcla" :key="idx">
                                    <td>{{ item.nombre }}</td>
                                    <td class="text-center" style="color: #7f8c8d;" v-if="!yaEstaDeclarado">
                                        {{ item.teorico > 0 ? item.teorico.toFixed(2) : '---' }}
                                    </td>
                                    <td :class="{'text-center': yaEstaDeclarado}">
                                        <input 
                                            v-if="!yaEstaDeclarado" 
                                            type="number" 
                                            v-model="item.real" 
                                            style="width: 100px; padding: 5px; font-weight: bold;" 
                                            step="0.1" 
                                            min="0"
                                            :disabled="esCargaSimple"
                                        >
                                        <span v-else style="font-weight: 800; color: #27ae60;">{{ item.real.toFixed(2) }} kg</span>
                                    </td>
                                    <td style="text-align: center;" v-if="!yaEstaDeclarado">
                                        <button v-if="!esCargaSimple" class="btn-quitar" @click="quitarInsumo(idx)" title="Quitar insumo">❌</button>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                        
                        <div v-if="!yaEstaDeclarado && !esCargaSimple" class="barra-agregar-extra" style="margin-top: 15px; padding-top: 15px; border-top: 1px dashed #cbd5e1; display: flex; flex-wrap: wrap; gap: 10px; align-items: center;">
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
                    <h4>📄 Órdenes incluidas en {{ esCargaSimple ? 'la hoja' : 'este grupo' }} ({{ ordenes.length }})</h4>
                    <ul class="lista-ordenes">
                        <li v-for="o in ordenes" :key="o.id">
                            <strong>Nota Pedido #{{ o.notaPedido || o.id }}</strong> - {{ o.producto }} 
                            <span style="color:#7f8c8d; font-weight:bold; margin-left:5px;">({{ o.cantidad }} Unid. | {{ o.kilos }} Kg)</span>
                            <span :class="['badge', 'badge-' + o.estado.toLowerCase()]">{{ o.estado === 'MaterialPreparado' ? 'MATERIAL LISTO' : (o.estado === 'Pendiente' ? 'EN MÁQUINA' : o.estado.toUpperCase()) }}</span>
                        </li>
                    </ul>
                    <div style="background: #e2e8f0; padding: 10px 15px; border-radius: 6px; margin-top: 5px; text-align: right; font-weight: 900; color: #1e293b;">
                        TOTAL GRUPO: 
                        <span style="color: #3b82f6; margin-left: 10px;">{{ ordenes.reduce((sum, o) => sum + (Number(o.cantidad) || 0), 0) }} Unidades</span>
                        <span style="color: #10b981; margin-left: 10px;">{{ ordenes.reduce((sum, o) => sum + (Number(o.kilos) || 0), 0).toFixed(2) }} Kg</span>
                    </div>
                </div>
            </div>
            
            <div class="modal-footer">
                <span v-if="esCargaSimple" style="color: #64748b; font-size: 0.85rem; font-style: italic; margin-right: auto; align-self: center;">
                    👁️ Visor bloqueado. (Consumo individual vía ✅)
                </span>
                
                <button class="btn-imprimir" @click="$emit('imprimir-carga', codigo, ordenes, consumosMezcla)" :disabled="procesando" title="Ver PDF de Carga">
                    🖨️ Ver / Imprimir Hoja
                </button>
                
                <button class="btn-cancelar" @click="$emit('close')">Cerrar</button>
                
                <button v-if="puedeRevertir && !esCargaSimple" class="btn-revertir" @click="revertirDeclaracion" :disabled="procesando">
                    {{ procesando ? '⏳ Procesando...' : '⏪ Revertir Declaración' }}
                </button>
                <button v-if="!yaEstaDeclarado && !esCargaSimple" class="btn-confirmar" @click="declararConsumos" :disabled="procesando">
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

.btn-imprimir { background: #3b82f6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s;}
.btn-imprimir:hover:not(:disabled) { background: #2563eb; }
.btn-imprimir:disabled { background: #93c5fd; cursor: not-allowed; }

.btn-cancelar { background: #95a5a6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s;}
.btn-cancelar:hover { background: #7f8c8d; }

.btn-confirmar { background: #27ae60; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s;}
.btn-confirmar:hover:not(:disabled) { background: #2ecc71; }
.btn-confirmar:disabled { background: #bdc3c7; cursor: not-allowed; }

.btn-revertir { background: #e67e22; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s;}
.btn-revertir:hover:not(:disabled) { background: #d35400; }
.btn-revertir:disabled { background: #f39c12; cursor: not-allowed; opacity: 0.7;}

.badge { padding: 3px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: bold; }
.badge-pendiente, .badge-enproceso { background: #fff7ed; color: #d97706; border: 1px solid #fcd34d; }
.badge-materialpreparado { background: #eff6ff; color: #3b82f6; border: 1px solid #93c5fd; }
.badge-finalizada { background: #ecfdf5; color: #10b981; border: 1px solid #a7f3d0; }

.btn-orden { background: #34495e; border: 1px solid #7f8c8d; color: white; cursor: pointer; border-radius: 4px; font-weight: bold; } 
.btn-orden:hover:not(:disabled) { background: #2980b9; }
.btn-orden:disabled { opacity: 0.5; cursor: not-allowed; }

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

.spinner-mini {
    border: 3px solid #f3f3f3;
    border-top: 3px solid #3498db;
    border-radius: 50%;
    width: 20px;
    height: 20px;
    animation: spin 1s linear infinite;
    display: inline-block;
    vertical-align: middle;
    margin-right: 8px;
}

@keyframes spin {
    0% { transform: rotate(0deg); }
    100% { transform: rotate(360deg); }
}
</style>