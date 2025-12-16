<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'

// --- INTERFACES ---
interface Cliente {
  id: number;
  razonSocial: string;
}

interface RemitoItem {
    id: number;
    productoNombre: string;
    cantidad: number;
    detalle?: string; // ✅ Aquí viene el "Rojo" o "40 micrones"
}

interface Remito {
  id: number;
  fecha: string;
  clienteId: number;
  cliente?: Cliente;
  observacion?: string;
  items?: RemitoItem[]; // ✅ Lista de items dentro del remito
}

// --- ESTADO ---
const remitos = ref<Remito[]>([])
const cargando = ref(false)
const error = ref('')
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- FUNCIONES ---
async function cargarHistorial() {
    cargando.value = true;
    error.value = '';
    try {
        const res = await axios.get(`${apiUrl}/Remitos`, getAuthConfig());
        remitos.value = res.data;
    } catch (e: any) {
        console.error(e);
        error.value = 'Error al cargar el historial.';
    } finally {
        cargando.value = false;
    }
}

function verDetalle(id: number) {
    alert(`Aquí se generaría el PDF del Remito #${id}`);
    // Aquí iría la lógica de html2pdf
}

onMounted(() => {
    cargarHistorial();
});
</script>

<template>
  <div class="contenedor-historial">
    <div class="header-seccion">
        <h2>📄 Historial de Remitos</h2>
        
        <div class="botones">
            <button class="btn-recargar" @click="cargarHistorial">🔄 Recargar</button>
            <router-link :to="{ name: 'DespachoRemitos' }" class="btn-nuevo">
                ➕ Nuevo Remito
            </router-link>
        </div>
    </div>

    <div v-if="cargando" class="loading">Cargando datos...</div>
    <div v-if="error" class="error-msg">{{ error }}</div>

    <div class="tabla-container" v-if="!cargando && remitos.length > 0">
        <table>
            <thead>
                <tr>
                    <th>Nro #</th>
                    <th>Fecha</th> <th>Cliente</th>
                    <th>Productos / Detalle</th> <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="r in remitos" :key="r.id">
                    <td><strong>{{ r.id.toString().padStart(4, '0') }}</strong></td>
                    
                    <td>{{ new Date(r.fecha).toLocaleDateString() }}</td>
                    
                    <td class="cliente-cell">{{ r.cliente?.razonSocial || 'Desconocido' }}</td>
                    
                    <td class="celda-productos">
                        <div v-for="item in r.items" :key="item.id" class="item-fila">
                            • {{ item.productoNombre }} 
                            <span v-if="item.detalle" class="tag-detalle">({{ item.detalle }})</span>
                            <strong>x {{ item.cantidad }}kg</strong>
                        </div>
                        <small v-if="!r.items || r.items.length === 0" style="color:#999">Sin detalles</small>
                    </td>

                    <td>
                        <button class="btn-ver" @click="verDetalle(r.id)">📄 PDF</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div v-else-if="!cargando && remitos.length === 0" class="vacio">
        <p>📭 No se han generado remitos aún.</p>
    </div>
  </div>
</template>

<style scoped>
.contenedor-historial { padding: 20px; background: #f8f9fa; border-radius: 8px; min-height: 400px; }
.header-seccion { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; border-bottom: 2px solid #ddd; padding-bottom: 10px; }
h2 { margin: 0; color: #2c3e50; }
.botones { display: flex; gap: 10px; }

.tabla-container { overflow-x: auto; background: white; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
table { width: 100%; border-collapse: collapse; font-family: Arial, sans-serif; font-size: 14px; }
th { background: #2c3e50; color: white; padding: 12px; text-align: left; }
td { padding: 12px; border-bottom: 1px solid #eee; color: #333; vertical-align: top; }
tr:hover { background-color: #f1f1f1; }

.cliente-cell { font-weight: bold; color: #2980b9; text-transform: uppercase; }
.celda-productos { font-size: 0.95em; }
.item-fila { margin-bottom: 4px; }
.tag-detalle { background-color: #eef2f3; color: #555; padding: 1px 5px; border-radius: 4px; font-style: italic; font-size: 0.9em; }

.btn-recargar { background: #95a5a6; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-nuevo { background: #27ae60; color: white; text-decoration: none; padding: 8px 15px; border-radius: 4px; font-weight: bold; font-size: 14px; display: inline-block; }
.btn-nuevo:hover { background: #2ecc71; }

.btn-ver { background: #34495e; color: white; border: none; padding: 6px 12px; border-radius: 4px; cursor: pointer; font-size: 12px; }
.btn-ver:hover { background: #2c3e50; }

.loading { text-align: center; padding: 20px; color: #7f8c8d; }
.error-msg { color: #c0392b; background: #fadbd8; padding: 10px; border-radius: 4px; margin-bottom: 10px; }
.vacio { text-align: center; color: #bdc3c7; padding: 40px; font-size: 18px; }
</style>