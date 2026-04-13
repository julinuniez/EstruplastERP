<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import api from '@/services/axiosInstance'

const props = defineProps<{
    visible: boolean;
    productoId: number | null;
    productoNombre: string;
}>();

const emit = defineEmits(['close']);

interface Movimiento {
    id: number;
    fecha: string;
    tipoMovimiento: string;
    cantidad: number;
    observacion: string;
    fechaFormateada?: string;
}

const cargando = ref(false);
const movimientos = ref<Movimiento[]>([]);

const fechaActual = new Date();
const mesSeleccionado = ref(fechaActual.getMonth() + 1);
const anioSeleccionado = ref(fechaActual.getFullYear());

const listaMeses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
];

const listaAnios = computed(() => {
    const anios = [];
    for (let i = 2025; i <= fechaActual.getFullYear() + 1; i++) {
        anios.push(i);
    }
    return anios;
});

const saldoCalculado = computed(() => {
    return movimientos.value.reduce((acc, mov) => acc + mov.cantidad, 0);
});

async function cargarMovimientos() {
    if (!props.productoId) return;
    
    cargando.value = true;
    try {
        const res = await api.get(`/Productos/movimientos/${props.productoId}?mes=${mesSeleccionado.value}&anio=${anioSeleccionado.value}`);
        
        // 🕵️‍♂️ ESPÍA: Vamos a imprimir en la consola qué carajo está devolviendo C#
        console.log("Respuesta cruda de C#:", res.data);

        // 🛡️ BLINDAJE: Buscamos dónde está realmente el Array
        let arrayDeDatos = [];
        
        if (Array.isArray(res.data)) {
            arrayDeDatos = res.data; // Todo normal, es un array directo
        } else if (res.data && Array.isArray(res.data.data)) {
            arrayDeDatos = res.data.data; // Vino envuelto como { data: [...] }
        } else if (res.data && Array.isArray(res.data.movimientos)) {
            arrayDeDatos = res.data.movimientos; // Vino envuelto como { movimientos: [...] }
        } else {
            console.warn("⚠️ C# no devolvió una lista reconocible. Revisa el endpoint.");
            movimientos.value = [];
            return; // Cortamos acá para que no explote
        }

        // Ahora sí, hacemos el map() seguros de que es una lista
        movimientos.value = arrayDeDatos.map((m: any) => ({
            ...m,
            fechaFormateada: new Date(m.fecha).toLocaleDateString('es-AR', { 
                day: '2-digit', month: '2-digit', hour: '2-digit', minute: '2-digit' 
            })
        })).sort((a: any, b: any) => new Date(a.fecha).getTime() - new Date(b.fecha).getTime());

    } catch (e) {
        console.error("Error al cargar historial", e);
        movimientos.value = []; 
    } finally {
        cargando.value = false;
    }
}

// Escuchar cambios: Si se abre el modal o cambian los filtros de fecha, recargar
watch([() => props.visible, mesSeleccionado, anioSeleccionado], ([visibleNuevo]) => {
    if (visibleNuevo && props.productoId) {
        cargarMovimientos();
    } else if (!visibleNuevo) {
        movimientos.value = []; // Limpiamos al cerrar
    }
});

const formatearTipo = (tipo: string) => {
    if (tipo === 'CONSUMO_PRODUCCION') return '🔴 Salida a OP';
    if (tipo === 'PRODUCCION_TERMINADA') return '🟢 Ingreso Terminados';
    if (tipo === 'INGRESO_MANUAL') return '🔵 Ingreso Manual';
    if (tipo === 'AJUSTE') return '🟡 Ajuste Inventario';
    return `⚪ ${tipo}`;
};

</script>

<template>
    <div v-if="visible" class="modal-overlay" @click.self="emit('close')">
        <div class="modal-content modal-lg">
            <div class="modal-header">
                <h3>📋 Kardex: {{ productoNombre }}</h3>
                <button class="btn-close" @click="emit('close')">✕</button>
            </div>
            
            <div class="filtros-kardex">
                <div class="grupo-filtro-tiempo">
                    <label>📅 Período:</label>
                    <select v-model="mesSeleccionado" class="select-mes">
                        <option v-for="m in listaMeses" :key="m.id" :value="m.id">{{ m.nombre }}</option>
                    </select>
                    <select v-model="anioSeleccionado" class="select-anio">
                        <option v-for="a in listaAnios" :key="a" :value="a">{{ a }}</option>
                    </select>
                </div>
                
                <div class="saldo-mes" :class="{'saldo-positivo': saldoCalculado >= 0, 'saldo-negativo': saldoCalculado < 0}">
                    Balance del Mes: <strong>{{ saldoCalculado > 0 ? '+' : '' }}{{ saldoCalculado.toFixed(2) }} kg</strong>
                </div>
            </div>
            
            <div class="tabla-scroll">
                <table class="tabla-custom">
                    <thead>
                        <tr>
                            <th style="width: 140px;">Fecha y Hora</th>
                            <th style="width: 180px;">Tipo de Movimiento</th>
                            <th>Observación / Ref.</th>
                            <th style="text-align: right; width: 100px;">Kilos</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-if="cargando">
                            <td colspan="4" style="text-align: center; padding: 20px;">Cargando historial... ⏳</td>
                        </tr>
                        <tr v-else-if="movimientos.length === 0">
                            <td colspan="4" style="text-align: center; padding: 20px; color: #94a3b8; font-style: italic;">
                                No se registraron movimientos en este período.
                            </td>
                        </tr>
                        <tr v-else v-for="m in movimientos" :key="m.id">
                            <td style="color: #64748b; font-size: 0.85rem;">{{ m.fechaFormateada }}</td>
                            <td style="font-weight: 600;">{{ formatearTipo(m.tipoMovimiento) }}</td>
                            <td>{{ m.observacion || '-' }}</td>
                            <td style="text-align: right; font-weight: bold;" :class="{'texto-rojo': m.cantidad < 0, 'texto-verde': m.cantidad > 0}">
                                {{ m.cantidad > 0 ? '+' : '' }}{{ m.cantidad.toFixed(2) }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal-footer">
                <button class="btn-cancelar" @click="emit('close')">Cerrar</button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; backdrop-filter: blur(2px); }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 450px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); display: flex; flex-direction: column; max-height: 90vh; }
.modal-lg { width: 800px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #f1f5f9; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #1e293b; font-size: 1.2rem; }
.btn-close { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #94a3b8; }
.btn-close:hover { color: #ef4444; }

.filtros-kardex { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; background: #f8fafc; padding: 10px; border-radius: 8px; border: 1px solid #e2e8f0;}
.grupo-filtro-tiempo { display: flex; align-items: center; gap: 8px; }
.grupo-filtro-tiempo label { font-weight: bold; color: #475569; font-size: 0.9rem; margin: 0; }
.select-mes, .select-anio { padding: 4px; border: 1px solid #cbd5e1; border-radius: 4px; font-weight: 500; color: #1e293b; background: white; cursor: pointer; }

.saldo-mes { padding: 6px 12px; border-radius: 6px; font-size: 0.95rem; font-weight: 500; }
.saldo-positivo { background: #ecfdf5; color: #065f46; border: 1px solid #a7f3d0; }
.saldo-negativo { background: #fef2f2; color: #991b1b; border: 1px solid #fecaca; }

.tabla-scroll { overflow-y: auto; border-radius: 6px; border: 1px solid #e2e8f0; flex-grow: 1; min-height: 200px; }
.tabla-custom { width: 100%; border-collapse: separate; border-spacing: 0; font-size: 0.85rem; }
.tabla-custom th { background: #f1f5f9; color: #475569; padding: 12px 10px; text-align: left; position: sticky; top: 0; font-weight: 700; border-bottom: 2px solid #cbd5e1; text-transform: uppercase; font-size: 0.75rem; }
.tabla-custom td { padding: 10px; border-bottom: 1px solid #f1f5f9; color: #334155; vertical-align: middle; }
.tabla-custom tbody tr:hover td { background-color: #f8fafc; }

.texto-rojo { color: #dc2626; }
.texto-verde { color: #16a34a; }

.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; border-top: 1px solid #f1f5f9; padding-top: 15px; }
.btn-cancelar { background: white; border: 1px solid #cbd5e1; color: #475569; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-cancelar:hover { background: #f8fafc; }
</style>