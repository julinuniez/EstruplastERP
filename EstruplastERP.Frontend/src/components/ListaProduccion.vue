<script setup lang="ts">
import { ref, onMounted } from 'vue'
// Asegúrate de que este archivo exista en la misma carpeta o ajusta la ruta
import ModalCierreOrden from './ModalCierreOrden.vue' 

// INTERFACES (Adaptadas a lo que devuelve el endpoint /recientes)
interface ProduccionItem {
  id: number;
  fecha: string;
  producto: string;
  cantidad: number;
  kilos: number;
  operario: string;
  estado: string; // El backend devuelve "Pendiente" o "Finalizada" como string
  esFinalizada: boolean; // El backend devuelve true/false
  
  // Campos opcionales para lógica interna
  lote?: string;
  turno?: string;
}

// ESTADO
const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)
const error = ref('')

// MODAL
const mostrarModalCierre = ref(false)
const ordenSeleccionada = ref<ProduccionItem | null>(null)
const listaMateriasPrimas = ref<any[]>([]) 

const apiUrl = import.meta.env.VITE_API_URL || '/api';

// --- CARGAR DATOS (USANDO /RECIENTES) ---
async function cargarHistorial() {
  cargando.value = true
  error.value = ''
  try {
    // 🔥 CAMBIO CLAVE: Usamos el endpoint simple que no falla por fechas
    const res = await fetch(`${apiUrl}/Ordenes/recientes`, {
        headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
    })
    
    if (!res.ok) throw new Error("Error al obtener datos")
    
    producciones.value = await res.json()
  } catch (e: any) {
    console.error("Error cargando historial:", e)
    error.value = "No se pudieron cargar las órdenes."
  } finally {
    cargando.value = false
  }
}

// --- LOGICA DE FINALIZAR (CONFIRMACIÓN SIMPLE) ---
async function confirmarOrdenRapida(item: ProduccionItem) {
    if(!confirm(`¿Confirmar orden del producto ${item.producto}? Se sumarán ${item.kilos}kg al stock.`)) return;

    try {
        const res = await fetch(`${apiUrl}/Ordenes/confirmar/${item.id}`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });

        if(res.ok) {
            // Actualizar localmente para feedback instantáneo
            item.esFinalizada = true;
            item.estado = "Finalizada";
        } else {
            const data = await res.json();
            alert("Error: " + (data.mensaje || "No se pudo confirmar"));
        }
    } catch (e) {
        alert("Error de conexión");
    }
}

// IMPRIMIR ETIQUETA
const imprimirEtiqueta = (item: ProduccionItem) => {
    const ventana = window.open('', 'PRINT', 'height=600,width=800');
    if (ventana) {
        ventana.document.write(`
            <html>
            <head>
                <title>Etiqueta ${item.id}</title>
                <style>
                    body { font-family: sans-serif; padding: 20px; text-align: center; border: 2px solid black; }
                    h1 { font-size: 40px; margin-bottom: 10px; }
                    .dato { font-size: 20px; margin: 10px 0; }
                    .grande { font-size: 60px; font-weight: bold; margin: 20px 0; }
                </style>
            </head>
            <body>
                <h1>${item.producto}</h1>
                <div class="dato">Fecha: ${item.fecha}</div>
                <div class="dato">Operario: ${item.operario}</div>
                <hr>
                <div class="grande">${item.kilos} Kg</div>
                <div class="dato">Lote ID: ${item.id}</div>
            </body>
            </html>
        `);
        ventana.document.close();
        ventana.focus();
        setTimeout(() => { ventana.print(); ventana.close(); }, 500);
    }
};

onMounted(() => {
    cargarHistorial();
})

// Exponemos la función para que el Padre (Formulario) pueda recargar la tabla
defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3>📋 Últimos Movimientos de Producción</h3>
        <button @click="cargarHistorial" class="btn-refresh" title="Actualizar">🔄 Actualizar</button>
    </div>

    <div v-if="cargando" class="loading">Cargando...</div>
    <div v-else-if="error" class="error-msg">{{ error }}</div>

    <div v-else class="tabla-scroll">
        <table class="tabla-custom">
            <thead>
                <tr>
                    <th>Fecha</th>
                    <th>Producto</th>
                    <th>Cant.</th>
                    <th>Kilos</th>
                    <th>Operario</th>
                    <th>Estado</th>
                    <th>Acción</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in producciones" :key="p.id" :class="{'fila-ok': p.esFinalizada}">
                    <td>{{ p.fecha }}</td>
                    <td class="td-prod">{{ p.producto }}</td>
                    <td style="text-align: center;">{{ p.cantidad }}</td>
                    <td style="text-align: right; font-weight: bold;">{{ p.kilos }}</td>
                    <td>{{ p.operario }}</td>
                    <td>
                        <span :class="p.esFinalizada ? 'badge-ok' : 'badge-pend'">
                            {{ p.esFinalizada ? 'FINALIZADA' : 'PENDIENTE' }}
                        </span>
                    </td>
                    <td class="td-acciones">
                        <button 
                            v-if="!p.esFinalizada" 
                            @click="confirmarOrdenRapida(p)" 
                            class="btn-action btn-check" 
                            title="Confirmar Producción y Stock">
                            ✅
                        </button>
                        
                        <button @click="imprimirEtiqueta(p)" class="btn-action btn-print" title="Imprimir Etiqueta">
                            🖨️
                        </button>
                    </td>
                </tr>
                <tr v-if="producciones.length === 0">
                    <td colspan="7" class="vacio">No hay órdenes recientes.</td>
                </tr>
            </tbody>
        </table>
    </div>
  </div>
</template>

<style scoped>
.historial-wrapper { background: white; padding: 15px; border-radius: 8px; border: 1px solid #e0e0e0; height: 100%; display: flex; flex-direction: column; }

.header-tabla { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; border-bottom: 2px solid #f1c40f; padding-bottom: 5px; }
.header-tabla h3 { margin: 0; color: #2c3e50; font-size: 1.1rem; }

.btn-refresh { background: none; border: 1px solid #ccc; border-radius: 4px; cursor: pointer; padding: 4px 8px; font-size: 0.9rem; transition: all 0.2s; }
.btn-refresh:hover { background: #f9f9f9; color: #3498db; border-color: #3498db; }

.tabla-scroll { overflow-y: auto; flex: 1; }
.tabla-custom { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.tabla-custom th { background: #2c3e50; color: white; padding: 8px; text-align: left; position: sticky; top: 0; z-index: 5; }
.tabla-custom td { padding: 8px; border-bottom: 1px solid #eee; color: #333; }

.td-prod { font-weight: 600; color: #2c3e50; }
.fila-ok { background-color: #f8fff9; color: #888; }
.fila-ok .td-prod { color: #888; }

.badge-ok { background: #d4edda; color: #155724; padding: 3px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #c3e6cb; }
.badge-pend { background: #fff3cd; color: #856404; padding: 3px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; border: 1px solid #ffeeba; }

.td-acciones { display: flex; gap: 5px; justify-content: center; }
.btn-action { border: 1px solid #ddd; background: white; border-radius: 4px; cursor: pointer; padding: 4px 8px; font-size: 1.1rem; transition: transform 0.1s; }
.btn-action:hover { transform: scale(1.1); background: #f0f8ff; }
.btn-check { color: green; border-color: #c3e6cb; }
.btn-check:hover { background: #d4edda; }

.vacio { text-align: center; padding: 20px; color: #aaa; font-style: italic; }
.loading { text-align: center; padding: 20px; color: #3498db; }
.error-msg { text-align: center; padding: 20px; color: #e74c3c; font-weight: bold; }
</style>