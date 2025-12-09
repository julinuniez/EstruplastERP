<script setup lang="ts">
import { ref, onMounted } from 'vue'

const producciones = ref([])
const cargando = ref(false)

// Asegúrate que este puerto sea el mismo que en tu Formulario
const apiUrl = 'https://localhost:7244/api' 

async function cargarHistorial() {
  try {
    cargando.value = true
    const res = await fetch(`${apiUrl}/Produccion/hoy`)
    
    if (res.ok) {
      producciones.value = await res.json()
    } else {
      console.error("Error al cargar historial")
    }
  } catch (e) {
    console.error("Error de conexión", e)
  } finally {
    cargando.value = false
  }
}

// Cargar al iniciar el componente
onMounted(() => {
  cargarHistorial()
})

// Exponemos la función para que el padre pueda recargar la tabla al guardar
defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-container">
    <h3>📋 Historial de Hoy</h3>
    <table class="tabla">
      <thead>
        <tr>
          <th>Hora</th>
          <th>Producto</th>
          <th>Cant</th>
          <th>Kilos</th>
          <th>Operario</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in producciones" :key="p.id">
          <td>{{ new Date(p.fecha).toLocaleTimeString() }}</td>
          <td>{{ p.producto }}</td>
          <td>{{ p.cantidad }}</td>
          <td>{{ p.kilos }} kg</td>
          <td>{{ p.operario }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.historial-container { margin-top: 30px; border-top: 2px solid #eee; padding-top: 20px; }
.tabla { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla th { background: #eee; text-align: left; padding: 8px; }
.tabla td { border-bottom: 1px solid #ddd; padding: 8px; }
</style>