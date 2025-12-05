<script setup lang="ts">
import { ref, onMounted } from 'vue'

// Aquí guardaremos la lista de productos que vengan del servidor
const productos = ref([])
const error = ref('')

// Cambia esto por TU puerto real de Swagger
const apiUrl = 'https://localhost:7244/api/Productos' 

// Esta función se ejecuta apenas carga la página
onMounted(async () => {
  try {
    const respuesta = await fetch(apiUrl)
    if (!respuesta.ok) throw new Error('Error al conectar con API')
    
    // Convertimos los datos a JSON y los guardamos
    productos.value = await respuesta.json()
  } catch (e) {
    error.value = 'No se pudo conectar con el Backend: ' + e
    console.error(e)
  }
})
</script>

<template>
  <div class="contenedor">
    <h1>🏭 Estruplast ERP</h1>
    <h2>Inventario en Tiempo Real</h2>

    <div v-if="error" class="error">{{ error }}</div>

    <table v-else border="1">
      <thead>
        <tr>
          <th>ID</th>
          <th>Nombre</th>
          <th>Stock</th>
          <th>Tipo</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="p in productos" :key="p.id">
          <td>{{ p.id }}</td>
          <td>{{ p.nombre }}</td>
          <td>{{ p.stockActual }}</td>
          <td>
            <span v-if="p.esMateriaPrima">📦 Insumo</span>
            <span v-if="p.esProductoTerminado">🛍️ Venta</span>
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<style scoped>
.contenedor { font-family: sans-serif; padding: 20px; }
table { width: 100%; border-collapse: collapse; margin-top: 20px; }
th, td { padding: 10px; text-align: left; }
.error { color: red; font-weight: bold; }
</style>
