<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import api from '@/services/axiosInstance';

interface OrdenResumen {
    id: number;
    producto: string;
    medidas: string;
    estado: string;
    cantidad: number;
}

interface PedidoAgrupado {
    notaPedido: string;      
    pedido: string;          
    cliente: string;
    avance: number;          
    ordenes: OrdenResumen[];
}

const listaPedidos = ref<PedidoAgrupado[]>([]);
const cargando = ref(false);
const clienteFiltro = ref('');
const mostrarSoloPendientes = ref(false);

const cargarTablero = async () => {
    cargando.value = true;
    try {
        const res = await api.get('/Produccion/tablero-pedidos');
        listaPedidos.value = res.data;
    } catch (e) {
        console.error("Error cargando tablero:", e);
    } finally {
        cargando.value = false;
    }
};

const pedidosFiltrados = computed(() => {
    return listaPedidos.value.filter(p => {
        const busqueda = clienteFiltro.value.toLowerCase();
        
        const matchCliente = p.cliente.toLowerCase().includes(busqueda) || 
                             (p.notaPedido && p.notaPedido.toLowerCase().includes(busqueda)) ||
                             (p.pedido && p.pedido.toLowerCase().includes(busqueda));
        
        const matchPendiente = mostrarSoloPendientes.value ? p.avance < 100 : true;

        return matchCliente && matchPendiente;
    });
});

const claseEstado = (estado: string) => {
    if (estado === 'Pendiente') return 'badge-gris';
    if (estado === 'EnProceso') return 'badge-azul';
    if (estado === 'Finalizada') return 'badge-verde';
    return 'badge-gris';
};

onMounted(() => {
    cargarTablero();
});
</script>

<template>
    <div class="tablero-container">
        <div class="header-tablero">
            <h2>📋 Tablero de Pedidos</h2>
            <div class="filtros">
                <input v-model="clienteFiltro" placeholder="🔍 Buscar Cliente o N° Nota..." class="input-busqueda">
                <label class="check-pendientes">
                    <input type="checkbox" v-model="mostrarSoloPendientes"> Ocultar Terminados
                </label>
                <button @click="cargarTablero" class="btn-refresh">🔄</button>
            </div>
        </div>

        <div v-if="cargando" class="loading">Cargando Tablero...</div>

        <div v-else class="grid-pedidos">
            <div v-for="(p, index) in pedidosFiltrados" :key="index" class="card-pedido">
                <div class="card-header">
                    <div class="titulo-pedido">
                        <span class="lbl-nota">Nota: {{ p.notaPedido || 'Sin Nota' }}</span>
                        <span v-if="p.pedido" class="lbl-oc">OC: {{ p.pedido }}</span>
                        <span class="lbl-cli">{{ p.cliente }}</span>
                    </div>
                    <div class="badge-avance" :class="{'full': p.avance === 100}">
                        {{ p.avance }}%
                    </div>
                </div>

                <div class="progress-bar-bg">
                    <div class="progress-bar-fill" :style="{ width: p.avance + '%' }"></div>
                </div>

                <div class="lista-ordenes">
                    <div v-for="orden in p.ordenes" :key="orden.id" class="item-orden">
                        <div class="orden-info">
                            <strong>OP #{{ orden.id }}</strong>
                            <span class="prod-nombre">{{ orden.producto }}</span>
                            <small>{{ orden.medidas }}</small>
                        </div>
                        <div class="orden-estado">
                            <span :class="['badge', claseEstado(orden.estado)]">{{ orden.estado }}</span>
                            <span class="cant">{{ orden.cantidad }} un.</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div v-if="pedidosFiltrados.length === 0 && !cargando" class="vacio">
            No se encontraron pedidos con ese criterio.
        </div>
    </div>
</template>

<style scoped>
.tablero-container { padding: 20px; background-color: #ecf0f1; min-height: 100vh; font-family: 'Segoe UI', sans-serif; }
.header-tablero { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; gap: 10px; }
.header-tablero h2 { margin: 0; color: #2c3e50; }

.filtros { display: flex; gap: 10px; align-items: center; }
.input-busqueda { padding: 8px 12px; border-radius: 20px; border: 1px solid #bdc3c7; width: 250px; outline: none; }
.check-pendientes { font-size: 14px; color: #34495e; cursor: pointer; user-select: none; }
.btn-refresh { background: #3498db; color: white; border: none; padding: 8px 12px; border-radius: 6px; cursor: pointer; }

.grid-pedidos { display: grid; grid-template-columns: repeat(auto-fill, minmax(350px, 1fr)); gap: 20px; }

.card-pedido { background: white; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.05); overflow: hidden; border: 1px solid #e0e0e0; transition: transform 0.2s; }
.card-pedido:hover { transform: translateY(-2px); box-shadow: 0 5px 15px rgba(0,0,0,0.1); }

.card-header { padding: 15px; display: flex; justify-content: space-between; align-items: flex-start; background: #fff; }
.titulo-pedido { display: flex; flex-direction: column; }
.lbl-nota { font-size: 1.15rem; font-weight: 900; color: #2980b9; }
.lbl-oc { font-size: 0.85rem; color: #f39c12; font-weight: 600; margin-top: 2px; }
.lbl-cli { font-size: 0.95rem; color: #34495e; margin-top: 4px; }

.badge-avance { background: #ecf0f1; color: #7f8c8d; padding: 5px 10px; border-radius: 12px; font-weight: bold; font-size: 0.9rem; }
.badge-avance.full { background: #2ecc71; color: white; }

.progress-bar-bg { height: 6px; background: #ecf0f1; width: 100%; }
.progress-bar-fill { height: 100%; background: #3498db; transition: width 0.5s ease; }

.lista-ordenes { padding: 10px; background: #f9f9f9; border-top: 1px solid #eee; }
.item-orden { display: flex; justify-content: space-between; align-items: center; padding: 8px 0; border-bottom: 1px solid #eee; }
.item-orden:last-child { border-bottom: none; }

.orden-info { display: flex; flex-direction: column; font-size: 0.85rem; }
.prod-nombre { color: #34495e; font-weight: 500; }
.orden-info small { color: #95a5a6; }

.orden-estado { display: flex; flex-direction: column; align-items: flex-end; gap: 2px; }
.badge { font-size: 0.75rem; padding: 2px 6px; border-radius: 4px; color: white; font-weight: bold; }
.badge-gris { background: #95a5a6; }
.badge-azul { background: #3498db; }
.badge-verde { background: #27ae60; }
.cant { font-size: 0.8rem; font-weight: bold; color: #2c3e50; }

.loading, .vacio { text-align: center; padding: 40px; color: #7f8c8d; font-size: 1.1rem; }
</style>