<script setup lang="ts">
import { ref, onMounted } from 'vue'

const productos = ref([])
const form = ref({
  productoId: '',
  cantidad: 0,
  observacion: ''
})
const mensaje = ref('')

const apiUrl = 'https://localhost:7244/api' // ⚠️ REVISA TU PUERTO

onMounted(async () => {
  // Traemos solo Materias Primas
  const res = await fetch(`${apiUrl}/Productos`)
  const data = await res.json()
  productos.value = data.filter((p: any) => p.esMateriaPrima)
})

async function guardarIngreso() {
  if(form.value.cantidad <= 0) return alert("La cantidad debe ser mayor a 0")

  const res = await fetch(`${apiUrl}/Movimientos/ajuste`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(form.value)
  })

  if (res.ok) {
    mensaje.value = "✅ Stock ingresado correctamente!"
    form.value.cantidad = 0
    form.value.observacion = ''
  } else {
    alert("Error al guardar")
  }
}
</script>

<template>
  <div class="panel">
    <h3>🚚 Ingreso de Mercadería (Compras)</h3>
    
    <div class="campo">
      <label>Materia Prima:</label>
      <select v-model="form.productoId">
        <option disabled value="">Seleccionar...</option>
        <option v-for="p in productos" :key="p.id" :value="p.id">
          {{ p.nombre }} (Stock: {{ p.stockActual }})
        </option>
      </select>
    </div>

    <div class="campo">
      <label>Cantidad a Ingresar:</label>
      <input type="number" v-model="form.cantidad" placeholder="0.00" />
    </div>

    <div class="campo">
      <label>Remito / Proveedor:</label>
      <input type="text" v-model="form.observacion" placeholder="Ej: Dow Chemical - Remito 999" />
    </div>

    <button @click="guardarIngreso" class="btn-verde">📥 INGRESAR AL DEPÓSITO</button>
    <p class="success">{{ mensaje }}</p>
  </div>
</template>

<style scoped>
.panel { background: #e8f5e9; padding: 20px; border: 1px solid #4caf50; border-radius: 8px; max-width: 500px; margin: 0 auto; }
.campo { margin-bottom: 10px; display: flex; flex-direction: column; }
.btn-verde { background: #4caf50; color: white; padding: 10px; border: none; cursor: pointer; font-weight: bold; }
.success { color: green; text-align: center; font-weight: bold; }
input, select { padding: 8px; }
</style>