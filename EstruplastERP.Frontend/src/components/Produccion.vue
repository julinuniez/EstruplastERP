<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'

// 1. IMPORTAMOS TU COMPONENTE GIGANTE
// Asegúrate de que la ruta sea correcta según dónde lo guardaste
import FormularioProduccion from './FormularioProduccion.vue' 

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- ESTADO ---
const listaOrdenes = ref<any[]>([]);
const cargando = ref(false);
const mostrarFormulario = ref(false); // 👁️ Controla si vemos la Tabla o el Formulario

// --- FUNCIONES ---

async function cargarDatos() {
    cargando.value = true;
    try {
        // Asegúrate que este endpoint coincida con tu backend (OrdenesController)
        const res = await axios.get(`${apiUrl}/Ordenes`, getAuthConfig());
        listaOrdenes.value = res.data;
    } catch (error) {
        console.error("Error cargando historial:", error);
    } finally {
        cargando.value = false;
    }
}

// Esta función se ejecuta cuando tu hijo emite "guardado"
function ordenGuardadaExitosamente() {
    alert("¡Orden registrada en el sistema!");
    mostrarFormulario.value = false; // Ocultamos el formulario
    cargarDatos(); // Recargamos la tabla para ver la nueva orden
}

async function finalizarOrden(id: number) {
    if(!confirm("¿Confirmar finalización y entrada de stock?")) return;
    try {
        await axios.post(`${apiUrl}/Ordenes/finalizar/${id}`, {}, getAuthConfig());
        cargarDatos();
    } catch (e: any) { alert(e.response?.data || "Error"); }
}

async function cancelarOrden(id: number) {
    if(!confirm("¿Cancelar orden?")) return;
    try {
        await axios.post(`${apiUrl}/Ordenes/cancelar/${id}`, {}, getAuthConfig());
        cargarDatos();
    } catch (e) { console.error(e); }
}

// Utilidades visuales
function getEstadoLabel(estado: number) {
    switch (estado) {
        case 0: return 'Pendiente ⏳';
        case 1: return 'En Proceso ⚙️';
        case 2: return 'Finalizada ✅';
        case -1: return 'Cancelada ❌';
        default: return 'Desconocido';
    }
}
function getClaseEstado(estado: number) {
    switch (estado) {
        case 0: return 'badge-pendiente';
        case 2: return 'badge-finalizada';
        case -1: return 'badge-cancelada';
        default: return '';
    }
}

onMounted(() => cargarDatos());
</script>

<template>
  <div class="panel-produccion">
    
    <div class="header-produccion">
        <h2>🏭 Gestión de Producción</h2>
        
        <button 
            @click="mostrarFormulario = !mostrarFormulario" 
            :class="mostrarFormulario ? 'btn-volver' : 'btn-nueva'"
        >
            {{ mostrarFormulario ? '⬅ Volver al Listado' : '➕ Nueva Orden' }}
        </button>
    </div>

    <div v-if="mostrarFormulario">
        <FormularioProduccion @guardado="ordenGuardadaExitosamente" />
    </div>

    <div v-else class="tabla-container">
        <p v-if="cargando">Cargando datos...</p>
        
        <table v-else>
            <thead>
                <tr>
                    <th>ID</th>
                    <th>Fecha</th>
                    <th>Producto</th>
                    <th>Cant.</th>
                    <th>Estado</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="orden in listaOrdenes" :key="orden.id" :class="{'row-dimmed': orden.estado === -1}">
                    <td>#{{ orden.id }}</td>
                    <td>{{ new Date(orden.fechaCreacion).toLocaleDateString() }}</td>
                    <td>{{ orden.producto?.nombre || '...' }}</td> 
                    <td>{{ orden.cantidad }}</td>
                    <td>
                        <span :class="['badge', getClaseEstado(orden.estado)]">
                            {{ getEstadoLabel(orden.estado) }}
                        </span>
                    </td>
                    <td>
                        <button v-if="orden.estado === 0" @click="finalizarOrden(orden.id)" class="btn-action btn-ok" title="Finalizar">✅</button>
                        <button v-if="orden.estado === 0" @click="cancelarOrden(orden.id)" class="btn-action btn-cancel" title="Cancelar">🚫</button>
                    </td>
                </tr>
            </tbody>
        </table>
        <p v-if="listaOrdenes.length === 0" class="text-center">No hay órdenes recientes.</p>
    </div>

  </div>
</template>

<style scoped>
.panel-produccion { max-width: 1200px; margin: 0 auto; padding: 20px; }
.header-produccion { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }

/* Botones Superiores */
.btn-nueva { background-color: #3498db; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; font-weight: bold; }
.btn-nueva:hover { background-color: #2980b9; }

.btn-volver { background-color: #95a5a6; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; font-weight: bold; }
.btn-volver:hover { background-color: #7f8c8d; }

/* Tabla */
.tabla-container { background: white; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); overflow: hidden; }
table { width: 100%; border-collapse: collapse; }
th, td { padding: 12px; text-align: left; border-bottom: 1px solid #eee; }
th { background-color: #ecf0f1; font-weight: bold; color: #2c3e50; }

/* Badges */
.badge { padding: 4px 8px; border-radius: 12px; font-size: 0.85em; font-weight: bold; }
.badge-pendiente { background: #fff3cd; color: #856404; }
.badge-finalizada { background: #d4edda; color: #155724; }
.badge-cancelada { background: #f8d7da; color: #721c24; }

.btn-action { border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; margin-right: 5px; font-size: 1.2em; }
.btn-ok { background: #e8f5e9; color: green; }
.btn-cancel { background: #ffebee; color: red; }
.row-dimmed { opacity: 0.5; background: #f9f9f9; }
.text-center { text-align: center; padding: 20px; color: #777; }
</style>