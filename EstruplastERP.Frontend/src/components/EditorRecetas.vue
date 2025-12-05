<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'

const terminados = ref([]) // Lista de productos terminados
const insumos = ref([])    // Lista de materias primas
const recetaActual = ref([]) // La lista de ingredientes del producto seleccionado

const prodSeleccionado = ref('')
const formIngrediente = ref({
  materiaPrimaId: '',
  cantidad: 0
})

const apiUrl = 'https://localhost:7244/api' // ⚠️ REVISA TU PUERTO

onMounted(async () => {
  const res = await fetch(`${apiUrl}/Productos`)
  const data = await res.json()
  
  terminados.value = data.filter((p: any) => p.esProductoTerminado)
  insumos.value = data.filter((p: any) => p.esMateriaPrima)
})

// Cuando el usuario elige un producto, cargamos su receta automáticamente
watch(prodSeleccionado, async (newId) => {
  if(!newId) return
  cargarReceta(newId)
})

async function cargarReceta(id: any) {
  const res = await fetch(`${apiUrl}/Formulas/${id}`)
  if(res.ok) {
    recetaActual.value = await res.json()
  } else {
    recetaActual.value = [] // Si da 404 es que no tiene receta aún
  }
}

async function agregarIngrediente() {
  const payload = {
    productoTerminadoId: prodSeleccionado.value,
    materiaPrimaId: formIngrediente.value.materiaPrimaId,
    cantidad: formIngrediente.value.cantidad
  }

  const res = await fetch(`${apiUrl}/Formulas`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload)
  })

  if(res.ok) {
    cargarReceta(prodSeleccionado.value) // Recargar lista
    formIngrediente.value.cantidad = 0
  } else {
    alert("Error al agregar ingrediente")
  }
}

async function borrarIngrediente(id: number) {
  if(!confirm("¿Borrar este ingrediente?")) return
  await fetch(`${apiUrl}/Formulas/${id}`, { method: 'DELETE' })
  cargarReceta(prodSeleccionado.value)
}
</script>

<template>
  <div class="panel-receta">
    <h3>🧪 Editor de Fórmulas</h3>
    
    <div class="seccion-header">
      <label>1. Selecciona el Producto a Fabricar:</label>
      <select v-model="prodSeleccionado" class="select-big">
        <option disabled value="">Elegir Producto...</option>
        <option v-for="p in terminados" :key="p.id" :value="p.id">{{ p.nombre }}</option>
      </select>
    </div>

    <div v-if="prodSeleccionado" class="zona-carga">
      <h4>Agregar Ingrediente:</h4>
      <div class="form-inline">
        <select v-model="formIngrediente.materiaPrimaId">
          <option disabled value="">Qué insumo lleva...</option>
          <option v-for="i in insumos" :key="i.id" :value="i.id">{{ i.nombre }}</option>
        </select>
        <input type="number" v-model="formIngrediente.cantidad" placeholder="Cant." step="0.01">
        <button @click="agregarIngrediente">➕ Agregar</button>
      </div>

      <table class="tabla-receta">
        <thead><tr><th>Ingrediente</th><th>Cantidad</th><th>Acción</th></tr></thead>
        <tbody>
          <tr v-for="r in recetaActual" :key="r.id">
            <td>{{ r.ingrediente }}</td>
            <td>{{ r.cantidad }}</td>
            <td><button class="btn-rojo" @click="borrarIngrediente(r.id)">🗑️</button></td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<style scoped>
.panel-receta { background: #e3f2fd; padding: 20px; border: 1px solid #2196f3; border-radius: 8px; max-width: 600px; margin: 0 auto; }
.select-big { width: 100%; padding: 10px; font-size: 16px; margin-bottom: 20px; }
.form-inline { display: flex; gap: 10px; margin-bottom: 20px; }
.tabla-receta { width: 100%; background: white; border-collapse: collapse; }
.tabla-receta th, .tabla-receta td { border: 1px solid #ddd; padding: 8px; text-align: left; }
.btn-rojo { background: #ff5252; color: white; border: none; cursor: pointer; }
</style>