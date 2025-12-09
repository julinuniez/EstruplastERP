<script setup lang="ts">
import { ref, onMounted } from 'vue'

const producciones = ref([])
const cargando = ref(false)

// Fechas por defecto: Desde el día 1 del mes hasta Hoy
const fechaHoy = new Date()
const primerDia = new Date(fechaHoy.getFullYear(), fechaHoy.getMonth(), 1)

// Formato YYYY-MM-DD para los inputs HTML
const filtros = ref({
    desde: primerDia.toISOString().split('T')[0],
    hasta: fechaHoy.toISOString().split('T')[0]
})

const apiUrl = 'https://localhost:7244/api' 

// Función flexible: Carga por rango
async function cargarHistorial() {
  cargando.value = true
  try {
    // Construimos la URL con los parámetros
    const url = `${apiUrl}/Produccion/rango?desde=${filtros.value.desde}&hasta=${filtros.value.hasta}`
    
    const respuesta = await fetch(url)
    if (respuesta.ok) {
        producciones.value = await respuesta.json()
    }
  } catch (e) {
    console.error("Error conectando al servidor")
  } finally {
    cargando.value = false
  }
}

// Exponemos la función para que el Padre (App.vue) pueda recargar la tabla al guardar
defineExpose({ cargarHistorial })

// Carga inicial
onMounted(() => {
    cargarHistorial()
})
</script>

<template>
  <div class="historial-container">
    <div class="cabecera-historial">
        <h3>📋 Historial de Producción</h3>
        
        <div class="filtros">
            <label>Desde:</label>
            <input type="date" v-model="filtros.desde">
            
            <label>Hasta:</label>
            <input type="date" v-model="filtros.hasta">
            
            <button @click="cargarHistorial" class="btn-buscar">🔍 Buscar</button>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla-produccion">
          <thead>
            <tr>
              <th>Fecha/Hora</th>
              <th>Turno</th>
              <th>Operario</th>
              <th>Producto</th>
              <th>Lote</th>
              <th>Cant.</th>
              <th>Kilos</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in producciones" :key="p.id">
              <td>{{ new Date(p.fecha).toLocaleDateString() }} {{ new Date(p.fecha).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</td>
              <td><span class="badge-turno">{{ p.turno }}</span></td>
              <td>{{ p.operario }}</td>
              <td class="texto-prod">{{ p.producto }}</td>
              <td class="mono">{{ p.lote }}</td>
              <td class="numero">{{ p.cantidad }}</td>
              <td class="numero">{{ p.kilos }} kg</td>
            </tr>
            <tr v-if="producciones.length === 0 && !cargando">
                <td colspan="7" class="vacio">No hay registros en estas fechas.</td>
            </tr>
          </tbody>
        </table>
    </div>
    
    <div class="totales-pie" v-if="producciones.length > 0">
        <strong>Total Periodo:</strong> 
        {{ producciones.reduce((a, b) => a + b.cantidad, 0) }} Unid. / 
        {{ producciones.reduce((a, b) => a + b.kilos, 0).toFixed(2) }} Kg
    </div>
  </div>
</template>

<style scoped>
.historial-container { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }

.cabecera-historial { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; margin-bottom: 15px; }
.cabecera-historial h3 { margin: 0; color: #2c3e50; border-left: 5px solid #e67e22; padding-left: 10px; }

.filtros { display: flex; gap: 10px; align-items: center; background: #f8f9fa; padding: 8px; border-radius: 6px; border: 1px solid #ddd; }
.filtros input { padding: 5px; border: 1px solid #ccc; border-radius: 4px; }
.btn-buscar { background: #3498db; color: white; border: none; padding: 6px 12px; border-radius: 4px; cursor: pointer; }
.btn-buscar:hover { background: #2980b9; }

.tabla-scroll { overflow-x: auto; }
.tabla-produccion { width: 100%; border-collapse: collapse; min-width: 600px; }
.tabla-produccion th { background-color: #2c3e50; color: white; padding: 10px; text-align: left; font-size: 0.9em; }
.tabla-produccion td { padding: 10px; border-bottom: 1px solid #eee; color: #555; }
.tabla-produccion tr:hover { background-color: #f1f1f1; }

.badge-turno { background: #eee; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; color: #555; }
.texto-prod { font-weight: bold; color: #2c3e50; }
.mono { font-family: monospace; color: #666; }
.numero { font-weight: bold; text-align: right; }
.vacio { text-align: center; padding: 20px; color: #999; font-style: italic; }

.totales-pie { margin-top: 15px; text-align: right; font-size: 1.1em; border-top: 2px solid #2c3e50; padding-top: 10px; color: #2c3e50; }
</style>