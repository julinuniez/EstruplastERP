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
  operario: string;
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
  empleadoId?: number;
  turno?: string;
  observacion?: string;
}

const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)
const error = ref('')

const filtroEstado = ref('Pendientes');
const filtroFecha = ref(''); 

const produccionesFiltradas = computed(() => {
    return producciones.value.filter(item => {
        let pasaEstado = true;
        if (filtroEstado.value === 'Pendientes') pasaEstado = !item.esFinalizada && item.estado !== 'Cancelada';
        else if (filtroEstado.value === 'Finalizadas') pasaEstado = item.esFinalizada;
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

async function confirmarOrdenRapida(item: ProduccionItem) {
    if(!confirm(`¿Confirmar orden #${item.id}? Se sumará al stock.`)) return;
    try {
        await api.post(`/Ordenes/confirmar/${item.id}`)
        item.esFinalizada = true;
        item.estado = "Finalizada";
        alert("✅ Confirmado.");
    } catch (e: any) {
        const msg = e.response?.data?.mensaje || "Error de conexión";
        alert("❌ Error: " + msg);
    }
}

async function cancelarOrden(item: ProduccionItem) {
    if (!confirm(`⚠️ ¿Estás seguro de CANCELAR la Orden #${item.id}?\n\nSe devolverán los materiales al stock.`)) return;
    try {
        await api.post(`/Ordenes/cancelar/${item.id}`);
        item.estado = "Cancelada";
        await cargarHistorial();
        alert("✅ Orden cancelada correctamente.");
    } catch (e: any) {
        const msg = e.response?.data || "Error al cancelar";
        alert("❌ " + msg);
    }
}

const solicitarImpresion = (orden: ProduccionItem, tipo: 'orden' | 'carga') => {
    emit('imprimir-historial', { orden, tipo });
};

const imprimirEtiqueta = (item: any) => {
    const ventana = window.open('', 'PRINT', 'height=600,width=800');
    if (ventana) {
        ventana.document.write(`
            <html>
            <head>
                <title>Lote #${item.id}</title>
                <style>
                    body { font-family: 'Arial', sans-serif; padding: 20px; text-align: center; border: 4px solid black; margin: 10px; }
                    h1 { font-size: 32px; margin-bottom: 5px; text-transform: uppercase; }
                    .meta { font-size: 16px; color: #555; margin-bottom: 20px; }
                    .kilos { font-size: 70px; font-weight: 900; margin: 20px 0; }
                    .footer { font-size: 12px; margin-top: 40px; border-top: 1px dashed black; padding-top: 10px; }
                </style>
            </head>
            <body>
                <h1>${item.producto}</h1>
                <div class="meta">LOTE: #${item.id} | OP: ${item.operario}</div>
                <div class="kilos">${item.kilos} Kg</div>
                <div class="footer">ESTRUPLAST S.A. - CONTROL DE PRODUCCIÓN</div>
            </body>
            </html>
        `);
        ventana.document.close();
        ventana.focus();
        setTimeout(() => { 
            ventana.print(); 
            ventana.close(); 
        }, 500);
    }
};

onMounted(() => {
    cargarHistorial();
})

defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3 :style="{ color: filtroEstado === 'Pendientes' ? '#2c3e50' : '#7f8c8d' }">
            {{ filtroEstado === 'Pendientes' ? '🔥 Órdenes en Curso' : '🗄️ Archivo Histórico (' + filtroEstado + ')' }}
        </h3>
        <div class="filtros-container">
            <select v-model="filtroEstado" class="input-filtro">
                <option value="Pendientes">Pendientes</option>
                <option value="Finalizadas">Finalizadas</option>
                <option value="Canceladas">Canceladas</option>
                <option value="Todos">Todos</option>
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
                    <th>Operario</th>
                    <th>Estado</th>
                    <th>Acción</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in produccionesFiltradas" :key="p.id" :class="{'fila-ok': p.esFinalizada, 'fila-cancel': p.estado === 'Cancelada'}">
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
                    <td>{{ p.operario }}</td>
                    <td>
                        <span :class="{
                            'badge-ok': p.esFinalizada, 
                            'badge-pend': !p.esFinalizada && p.estado !== 'Cancelada',
                            'badge-cancel': p.estado === 'Cancelada'
                        }">
                            {{ p.estado === 'Cancelada' ? 'CANCELADA' : (p.esFinalizada ? 'FINALIZADA' : 'PENDIENTE') }}
                        </span>
                    </td>
                    <td class="td-acciones">
                        <template v-if="!p.esFinalizada && p.estado !== 'Cancelada'">
                            <button @click="confirmarOrdenRapida(p)" class="btn-action btn-check">✅</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action btn-orden">📄</button>
                            <button @click="solicitarImpresion(p, 'carga')" class="btn-action btn-carga">🧪</button>
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel" style="color:red; border-color: #ffcccc;">❌</button>
                        </template>
                        <button @click="imprimirEtiqueta(p)" class="btn-action btn-print">🖨️</button>
                    </td>
                </tr>
                <tr v-if="produccionesFiltradas.length === 0 && !cargando">
                    <td colspan="8" class="vacio">No hay órdenes en esta bandeja.</td>
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
.tabla-custom td { padding: 8px; border-bottom: 1px solid #eee; color: #333; }
.td-prod { font-weight: 600; color: #2c3e50; }
.fila-ok { background-color: #f8fff9; color: #888; }
.fila-ok .td-prod { text-decoration: line-through; } 
.fila-cancel { background-color: #fff5f5; color: #999; }
.fila-cancel .td-prod { text-decoration: line-through; color: #c0392b; }
.badge-ok { background: #d4edda; color: #155724; padding: 3px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #c3e6cb; }
.badge-pend { background: #fff3cd; color: #856404; padding: 3px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #ffeeba; }
.badge-cancel { background: #f8d7da; color: #721c24; padding: 3px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #f5c6cb; }
.td-acciones { display: flex; gap: 4px; justify-content: center; }
.btn-action { border: 1px solid #ddd; background: white; border-radius: 4px; cursor: pointer; padding: 4px 6px; font-size: 1.1rem; }
.btn-action:hover { transform: scale(1.1); background: #f0f8ff; }
.vacio { text-align: center; padding: 20px; color: #aaa; font-style: italic; }
.loading-overlay { position: absolute; top: 50px; left: 0; width: 100%; text-align: center; background: rgba(255,255,255,0.8); padding: 20px; color: #3498db; font-weight: bold; }
.error-msg { text-align: center; padding: 10px; color: #e74c3c; background: #fadbd8; margin-bottom: 10px; border-radius: 4px; }
</style>