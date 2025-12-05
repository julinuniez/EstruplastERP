<script setup lang="ts">
import { ref, onMounted } from 'vue'

const productos = ref([])
const form = ref({
  nombre: '',
  codigoSku: '',
  stockMinimo: 100,
  esMateriaPrima: false,
  esProductoTerminado: true,
  // Datos Técnicos
  color: '',
  largo: 0,
  ancho: 0,
  espesor: 0,
  pesoEspecifico: 1.05 // Valor por defecto (PAI)
})
const mensaje = ref('')

const apiUrl = 'https://localhost:7244/api' // ⚠️ REVISA TU PUERTO

onMounted(async () => {
  cargarProductos()
})

async function cargarProductos() {
  const res = await fetch(`${apiUrl}/Productos`)
  productos.value = await res.json()
}

async function guardarProducto() {
  try {
    const res = await fetch(`${apiUrl}/Productos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    })
    
    if (res.ok) {
        mensaje.value = "✅ Producto creado correctamente"
        cargarProductos() // Recargar lista
        // Limpiar form básico
        form.value.nombre = ''
        form.value.codigoSku = ''
    } else {
        mensaje.value = "❌ Error al crear"
    }
  } catch (e) { console.error(e) }
}
</script>

<template>
  <div class="panel-gestion">
    <h3>📦 Gestión de Productos</h3>
    
    <div class="formulario-creacion">
        <h4>Nuevo Producto</h4>
        
        <div class="fila">
            <input v-model="form.nombre" placeholder="Nombre (Ej: Lámina PAI)" class="full">
            <input v-model="form.codigoSku" placeholder="Código SKU" class="corto">
        </div>

        <div class="fila-tipos">
            <label><input type="checkbox" v-model="form.esMateriaPrima"> Es Insumo</label>
            <label><input type="checkbox" v-model="form.esProductoTerminado"> Es Venta</label>
        </div>

        <div class="caja-tecnica" v-if="form.esProductoTerminado">
            <h5>📏 Datos Técnicos (Para Cálculos)</h5>
            <div class="fila">
                <label>Color: <input v-model="form.color"></label>
                <label>Largo (mm): <input type="number" v-model="form.largo"></label>
            </div>
            <div class="fila">
                <label>Ancho (mm): <input type="number" v-model="form.ancho"></label>
                <label>Espesor (mm): <input type="number" v-model="form.espesor" step="0.01"></label>
            </div>
            <div class="fila">
                <label title="Densidad (g/cm3). PAI=1.05, PEAD=0.95">
                    ⚖️ Peso Específico: <input type="number" v-model="form.pesoEspecifico" step="0.0001">
                </label>
            </div>
        </div>

        <button @click="guardarProducto" class="btn-guardar">💾 CREAR PRODUCTO</button>
        <p>{{ mensaje }}</p>
    </div>

    <hr>

    <div class="listado">
        <h4>Inventario Actual</h4>
        <ul>
            <li v-for="p in productos" :key="p.id">
                <strong>{{ p.nombre }}</strong> (Stock: {{ p.stockActual }})
                <span v-if="p.esProductoTerminado" class="tag">Venta</span>
            </li>
        </ul>
    </div>
  </div>
</template>

<style scoped>
.panel-gestion { max-width: 600px; margin: 0 auto; padding: 20px; font-family: sans-serif; }
.formulario-creacion { background: #f4f4f4; padding: 20px; border-radius: 8px; }
.fila { display: flex; gap: 10px; margin-bottom: 10px; }
input { padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
.full { flex: 1; }
.corto { width: 100px; }
.caja-tecnica { background: #e3f2fd; padding: 10px; border-radius: 5px; margin-bottom: 15px; border: 1px solid #90caf9; }
.caja-tecnica h5 { margin-top: 0; color: #1565c0; }
.caja-tecnica label { display: flex; flex-direction: column; font-size: 12px; font-weight: bold; }
.btn-guardar { width: 100%; background: #2c3e50; color: white; padding: 10px; border: none; cursor: pointer; font-weight: bold; }
.listado ul { list-style: none; padding: 0; }
.listado li { border-bottom: 1px solid #eee; padding: 5px 0; display: flex; justify-content: space-between; }
.tag { background: #4caf50; color: white; padding: 2px 6px; font-size: 10px; border-radius: 4px; }
</style>