<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/axiosInstance' 

const emit = defineEmits(['imprimir-historial']);

interface ProduccionItem {
  id: number;
  fecha: string;
  producto: string;
  cantidad: number;
  kilos: number;
  estado: string;
  esFinalizada: boolean;
  notaPedido?: string;
  numeroPedidoCliente?: string;
  clienteNombre?: string; 
  cliente?: string;
  largo?: number;
  ancho?: number;
  espesor?: number;
  consumos?: any[];
  productoId?: number;
  clienteId?: number;
  observacion?: string;
}

const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)
const error = ref('')

const filtroEstado = ref('EnCola'); // <--- AHORA EL DEFAULT ES EN COLA
const filtroFecha = ref(''); 

const produccionesFiltradas = computed(() => {
    return producciones.value.filter(item => {
        let pasaEstado = true;
        if (filtroEstado.value === 'EnCola') pasaEstado = item.estado === 'EnCola';
        else if (filtroEstado.value === 'Pendientes') pasaEstado = item.estado === 'Pendiente' || item.estado === 'EnProceso';
        else if (filtroEstado.value === 'Finalizadas') pasaEstado = item.estado === 'Finalizada';
        else if (filtroEstado.value === 'Canceladas') pasaEstado = item.estado === 'Cancelada';
        else if (filtroEstado.value === 'Todos') pasaEstado = true;

        let pasaFecha = true;
        if (filtroFecha.value) {
            const [year, month, day] = filtroFecha.value.split('-');
            const fechaBuscada = `${day}/${month}`; 
            pasaFecha = item.fecha.startsWith(fechaBuscada);
        }

        return pasaEstado && pasaFecha;
    });
});

async function cargarHistorial() {
  cargando.value = true
  error.value = ''
  try {
    const res = await api.get('/Ordenes/recientes')
    if (Array.isArray(res.data)) {
        producciones.value = res.data.sort((a: any, b: any) => b.id - a.id);
    } else {
        error.value = "Error de conexión con el servidor.";
    }
  } catch (e: any) {
    error.value = "No se pudieron cargar las órdenes."
  } finally {
    cargando.value = false
  }
}

async function activarOrden(item: ProduccionItem) {
    if(!confirm(`¿Mandar la orden #${item.id} a las máquinas?\n\nAsegúrese de que el material ya ingresó a la planta.`)) return;
    try {
        await api.post(`/Ordenes/activar/${item.id}`)
        item.estado = "Pendiente";
        alert("🚀 Orden enviada a Producción.");
    } catch (e: any) {
        const msj = e.response?.data?.mensaje || e.response?.data || "Error de conexión con el servidor";
        alert(msj);
    }
}

async function confirmarOrdenRapida(item: ProduccionItem) {
    if(!confirm(`¿Confirmar orden #${item.id} como TERMINADA?\n\n⚠️ ESTO DESCONTARÁ LA MATERIA PRIMA DEL INVENTARIO.`)) return;
    try {
        await api.post(`/Ordenes/confirmar/${item.id}`)
        item.esFinalizada = true;
        item.estado = "Finalizada";
        alert("✅ Stock Descontado y PT Sumado.");
    } catch (e: any) {
        alert("❌ Error: " + (e.response?.data?.mensaje || "Error de conexión"));
    }
}

async function cancelarOrden(item: ProduccionItem) {
    if (!confirm(`⚠️ ¿Estás seguro de CANCELAR la Orden #${item.id}?`)) return;
    try {
        await api.post(`/Ordenes/cancelar/${item.id}`);
        item.estado = "Cancelada";
        await cargarHistorial();
    } catch (e: any) {
        alert("❌ " + (e.response?.data || "Error al cancelar"));
    }
}

const solicitarImpresion = (orden: ProduccionItem, tipo: 'orden' | 'carga') => {
    emit('imprimir-historial', { orden, tipo });
};

onMounted(() => {
    cargarHistorial();
})

defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3 style="color: #2c3e50;">
            {{ filtroEstado === 'EnCola' ? '🕒 Backlog (En Espera)' : (filtroEstado === 'Pendientes' ? '🔥 En Máquina' : '🗄️ Histórico') }}
        </h3>
        <div class="filtros-container">
            <select v-model="filtroEstado" class="input-filtro">
                <option value="EnCola">🕒 En Cola (Falta Mat.)</option>
                <option value="Pendientes">🔥 En Producción</option>
                <option value="Finalizadas">✅ Finalizadas</option>
                <option value="Canceladas">❌ Canceladas</option>
                <option value="Todos">📁 Todas</option>
            </select>
            <input type="date" v-model="filtroFecha" class="input-filtro">
            <button @click="cargarHistorial" class="btn-refresh" :disabled="cargando">
                {{ cargando ? '⏳' : '🔄' }}
            </button>
        </div>
    </div>

    <div v-if="error" class="error-msg">{{ error }}</div>

    <div class="tabla-scroll">
        <table class="tabla-custom">
            <thead>
                <tr>
                    <th>Fecha</th>
                    <th>Nota Pedido</th>
                    <th>Producto</th>
                    <th>Cant.</th>
                    <th>Kilos</th>
                    <th>Estado</th>
                    <th>Acción</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in produccionesFiltradas" :key="p.id" :class="{'fila-ok': p.estado === 'Finalizada', 'fila-cancel': p.estado === 'Cancelada'}">
                    <td>{{ p.fecha }}</td>
                    <td>
                        <div style="font-weight: bold; color: #2c3e50;">
                            {{ p.notaPedido || '-' }}
                        </div>
                        <small v-if="p.numeroPedidoCliente" style="color: #7f8c8d; display: block; font-size: 0.7rem; margin-top: 2px;">
                            OC: {{ p.numeroPedidoCliente }}
                        </small>
                    </td>
                    <td class="td-prod">{{ p.producto }}</td>
                    <td style="text-align: center;">{{ p.cantidad }}</td>
                    <td style="text-align: right; font-weight: bold;">{{ p.kilos }}</td>
                    <td>
                        <span :class="{
                            'badge-cola': p.estado === 'EnCola',
                            'badge-pend': p.estado === 'Pendiente' || p.estado === 'EnProceso',
                            'badge-ok': p.estado === 'Finalizada',
                            'badge-cancel': p.estado === 'Cancelada'
                        }">
                            {{ p.estado === 'EnCola' ? 'EN COLA' : (p.estado === 'Cancelada' ? 'CANCELADA' : (p.estado === 'Finalizada' ? 'FINALIZADA' : 'EN MÁQUINA')) }}
                        </span>
                    </td>
                    <td class="td-acciones">
                        <template v-if="p.estado === 'EnCola'">
                            <button @click="activarOrden(p)" class="btn-action btn-check" title="Enviar a Máquina (Llegó Material)">🚀</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action">📄</button>
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel">❌</button>
                        </template>

                        <template v-else-if="p.estado === 'Pendiente' || p.estado === 'EnProceso'">
                            <button @click="confirmarOrdenRapida(p)" class="btn-action btn-check" title="Confirmar Fin y Descontar Stock">✅</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action">📄</button>
                            <button @click="solicitarImpresion(p, 'carga')" class="btn-action">🧪</button>
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel">❌</button>
                        </template>
                        
                        <template v-else-if="p.estado === 'Finalizada'">
                             <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Reimprimir Orden">📄</button>
                        </template>
                    </td>
                </tr>
                <tr v-if="produccionesFiltradas.length === 0 && !cargando">
                    <td colspan="7" class="vacio">No hay órdenes en esta bandeja.</td>
                </tr>
            </tbody>
        </table>
        <div v-if="cargando" class="loading-overlay">Cargando datos...</div>
    </div>
  </div>
</template>

<style scoped>
.historial-wrapper { background: white; padding: 15px; border-radius: 8px; border: 1px solid #e0e0e0; height: 100%; display: flex; flex-direction: column; position: relative; }
.header-tabla { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; border-bottom: 2px solid #f1c40f; padding-bottom: 5px; flex-wrap: wrap; gap: 10px; }
.header-tabla h3 { margin: 0; font-size: 1.1rem; }
.filtros-container { display: flex; gap: 8px; align-items: center; }
.input-filtro { padding: 4px 8px; border: 1px solid #ccc; border-radius: 4px; font-size: 0.9rem; background: #fff; cursor: pointer; }
.btn-refresh { background: none; border: 1px solid #ccc; border-radius: 4px; cursor: pointer; padding: 4px 10px; font-size: 1rem; }
.tabla-scroll { overflow-y: auto; flex: 1; }
.tabla-custom { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.tabla-custom th { background: #2c3e50; color: white; padding: 8px; text-align: left; position: sticky; top: 0; z-index: 5; }
.tabla-custom td { padding: 8px; border-bottom: 1px solid #eee; color: #333; vertical-align: middle; }
.td-prod { font-weight: 600; color: #2c3e50; }
.fila-ok { background-color: #f8fff9; color: #888; }
.fila-ok .td-prod { text-decoration: line-through; } 
.fila-cancel { background-color: #fff5f5; color: #999; }
.fila-cancel .td-prod { text-decoration: line-through; color: #c0392b; }

/* Semáforo de estados */
.badge-cola { background: #e0f7fa; color: #2e7d32; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #b2dfdb; }
.badge-pend { background: #fff3cd; color: #d35400; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #ffeeba; }
.badge-ok { background: #e8eaed; color: #7f8c8d; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #cfd8dc; }
.badge-cancel { background: #f8d7da; color: #721c24; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #f5c6cb; }

.td-acciones { display: flex; gap: 4px; justify-content: flex-start; }
.btn-action { border: 1px solid #ddd; background: white; border-radius: 4px; cursor: pointer; padding: 5px 8px; font-size: 1.1rem; display: flex; align-items: center; justify-content: center;}
.btn-action:hover { transform: scale(1.1); background: #f0f8ff; }
.btn-cancel { color: red; border-color: #ffcccc; }
.btn-cancel:hover { background: #ffebee; }

.vacio { text-align: center; padding: 20px; color: #aaa; font-style: italic; }
.loading-overlay { position: absolute; top: 50px; left: 0; width: 100%; text-align: center; background: rgba(255,255,255,0.8); padding: 20px; color: #3498db; font-weight: bold; }
.error-msg { text-align: center; padding: 10px; color: #e74c3c; background: #fadbd8; margin-bottom: 10px; border-radius: 4px; }
</style>