<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

const listaInsumos = ref([])
const form = ref({
  productoId: '',
  cantidad: 0,
  proveedor: '',
  costoTotalFactura: 0 // <--- CAMBIO: Ahora pedimos el total
})
const mensaje = ref('')
const error = ref('')

const apiUrl = 'https://localhost:7244/api' 

// Calculamos el unitario automáticamente para mostrarlo en pantalla
// Esto es solo visual para que el usuario verifique
const precioUnitarioCalculado = computed(() => {
    if (form.value.cantidad > 0 && form.value.costoTotalFactura > 0) {
        return (form.value.costoTotalFactura / form.value.cantidad).toFixed(2)
    }
    return "0.00"
})

onMounted(async () => {
  try {
    const res = await fetch(`${apiUrl}/Stock/materias-primas`)
    if (res.ok) listaInsumos.value = await res.json()
  } catch (e) { console.error("Error cargando insumos") }
})

async function registrarIngreso() {
  mensaje.value = ''
  error.value = ''

  if (!form.value.productoId || form.value.cantidad <= 0) {
    error.value = "Seleccione un producto y una cantidad válida."
    return
  }

  // LÓGICA DE NEGOCIO:
  // Si puso un total, calculamos el unitario. Si no, mandamos 0 (mantiene precio viejo)
  let precioUnitarioParaEnviar = 0
  if (form.value.costoTotalFactura > 0) {
      precioUnitarioParaEnviar = form.value.costoTotalFactura / form.value.cantidad
  }

  try {
    const res = await fetch(`${apiUrl}/Stock/ingresar`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            productoId: form.value.productoId,
            cantidad: form.value.cantidad,
            proveedor: form.value.proveedor,
            
            // 👇 AQUÍ ESTÁ LA MAGIA: Enviamos el resultado de la división
            nuevoPrecio: precioUnitarioParaEnviar 
        })
    })

    if (!res.ok) throw new Error('Error al guardar ingreso')

    mensaje.value = `✅ Ingreso guardado. Costo unitario actualizado a $${precioUnitarioCalculado.value}`
    
    // Limpiamos
    form.value.cantidad = 0
    form.value.costoTotalFactura = 0 // Reseteamos el total
    form.value.proveedor = ''
    
    const resLista = await fetch(`${apiUrl}/Stock/materias-primas`)
    listaInsumos.value = await resLista.json()

  } catch (e) {
    error.value = "❌ Error al conectar con el servidor"
  }
}
</script>

<template>
  <div class="hoja-stock">
    <h3>🚚 Ingreso de Mercadería (Compras)</h3>
    
    <div class="campo">
      <label>Materia Prima:</label>
      <select v-model="form.productoId">
        <option value="" disabled>Seleccione Insumo...</option>
        <option v-for="p in listaInsumos" :key="p.id" :value="p.id">
            {{ p.nombre }} (Stock: {{ p.stockActual }})
        </option>
      </select>
    </div>

    <div class="campo">
      <label>Cantidad (Kg/Uni):</label>
      <input type="number" v-model="form.cantidad" placeholder="0" />
    </div>

    <div class="campo">
      <label>Total de la Factura ($):</label>
      <div style="display: flex; gap: 5px; align-items: center;">
          <input type="number" v-model="form.costoTotalFactura" placeholder="Monto total pagado" />
      </div>
      
      <small v-if="form.costoTotalFactura > 0" style="color: #27ae60; font-weight: bold;">
          ℹ️ Costo Unitario resultante: ${{ precioUnitarioCalculado }} / kg
      </small>
      <small v-else style="color: #666;">
          Dejar en 0 para mantener precio anterior.
      </small>
    </div>

    <div class="campo">
      <label>Remito / Proveedor:</label>
      <input type="text" v-model="form.proveedor" placeholder="Ej: Dow Chemical - Remito 999" />
    </div>

    <button class="btn-ingreso" @click="registrarIngreso">📥 INGRESAR AL DEPÓSITO</button>

    <p v-if="mensaje" class="exito">{{ mensaje }}</p>
    <p v-if="error" class="error">{{ error }}</p>
  </div>
</template>

<style scoped>
.hoja-stock { 
    background: #f0fdf4; /* Verde muy clarito */
    padding: 20px; 
    border: 1px solid #4ade80; 
    border-radius: 8px; 
    max-width: 600px; 
    margin: 0 auto;
}
.campo { margin-bottom: 15px; text-align: left; }
.campo label { display: block; font-weight: bold; margin-bottom: 5px; }
.campo select, .campo input { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 5px; }
.btn-ingreso { background: #16a34a; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; width: 100%; }
.btn-ingreso:hover { background: #15803d; }
.exito { color: green; font-weight: bold; margin-top: 10px; }
.error { color: red; font-weight: bold; margin-top: 10px; }
</style>