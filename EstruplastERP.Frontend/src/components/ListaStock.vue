<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

const inventario = ref([])
const cargando = ref(true)
const busqueda = ref('')

// Variables para el MODAL de Ajuste
const mostrandoModal = ref(false)
const itemAjustar = ref({ id: 0, nombre: '', stockActual: 0, stockReal: 0, motivo: '' })

// KPIs (Calculados)
const totalItems = computed(() => inventario.value.length)
const itemsCriticos = computed(() => inventario.value.filter(i => i.estado === 'CRITICO').length)
const valorTotalInventario = computed(() => inventario.value.reduce((acc, item) => acc + item.valorTotal, 0))

// Filtro
const listaFiltrada = computed(() => {
  if (!busqueda.value) return inventario.value
  return inventario.value.filter(item => 
    item.nombre.toLowerCase().includes(busqueda.value.toLowerCase()) ||
    item.codigoSku.toLowerCase().includes(busqueda.value.toLowerCase())
  )
})

const apiUrl = 'https://localhost:7244/api' 

// Carga Inicial
async function cargarInventario() {
  cargando.value = true
  try {
    const res = await fetch(`${apiUrl}/Stock/inventario`)
    if (res.ok) inventario.value = await res.json()
  } catch (e) { console.error(e) } 
  finally { cargando.value = false }
}

// Abrir Modal
function abrirAjuste(item) {
    itemAjustar.value = { 
        id: item.id, 
        nombre: item.nombre, 
        stockActual: item.stockActual, 
        stockReal: item.stockActual, // Por defecto sugerimos el mismo
        motivo: ''
    }
    mostrandoModal.value = true
}

// Guardar Ajuste (Backend)
async function guardarAjuste() {
    if (!itemAjustar.value.motivo) return alert("Debes escribir un motivo (ej: Recuento, Rotura)")

    try {
        const res = await fetch(`${apiUrl}/Stock/ajuste`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                productoId: itemAjustar.value.id,
                cantidadReal: itemAjustar.value.stockReal,
                motivo: itemAjustar.value.motivo
            })
        })

        if (res.ok) {
            alert("✅ Stock corregido.")
            mostrandoModal.value = false
            cargarInventario() // Recargamos tabla
        } else {
            alert("Error al guardar ajuste")
        }
    } catch (e) { console.error(e) }
}

onMounted(() => cargarInventario())
</script>

<template>
  <div class="panel-stock">
    <h2>📦 Inventario General</h2>

    <div class="kpi-container">
        <div class="card-kpi"><span>Items</span><strong>{{ totalItems }}</strong></div>
        <div class="card-kpi danger"><span>⚠️ Críticos</span><strong>{{ itemsCriticos }}</strong></div>
        <div class="card-kpi money"><span>💰 Valorizado</span><strong>{{ valorTotalInventario.toLocaleString('es-AR', { style: 'currency', currency: 'ARS' }) }}</strong></div>
    </div>

    <div class="buscador">
        <input v-model="busqueda" type="text" placeholder="🔍 Buscar..." />
        <button @click="cargarInventario">🔄</button>
    </div>

    <table class="tabla-stock">
      <thead>
        <tr>
          <th>SKU</th>
          <th>Nombre</th>
          <th>Stock</th>
          <th>Costo Unit.</th>
          <th>Estado</th>
          <th>Acción</th> </tr>
      </thead>
      <tbody>
        <tr v-for="item in listaFiltrada" :key="item.id" :class="{ 'fila-critica': item.estado === 'CRITICO' }">
          <td>{{ item.codigoSku }}</td>
          <td>{{ item.nombre }}</td>
          <td class="numero">{{ item.stockActual }}</td>
          <td>${{ item.precioCosto }}</td> <td>
              <span v-if="item.estado === 'CRITICO'" class="alerta">⚠️ BAJO</span>
              <span v-else class="ok">✅ OK</span>
          </td>
          <td>
              <button class="btn-editar" @click="abrirAjuste(item)">✏️ Ajustar</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div v-if="mostrandoModal" class="modal-overlay">
        <div class="modal-content">
            <h3>Corrección de Stock</h3>
            <p>Producto: <strong>{{ itemAjustar.nombre }}</strong></p>
            
            <div class="campo">
                <label>Stock en Sistema:</label>
                <input type="text" :value="itemAjustar.stockActual" disabled style="background: #eee;">
            </div>

            <div class="campo">
                <label>Stock Real (Conteo Físico):</label>
                <input type="number" v-model.number="itemAjustar.stockReal" class="input-grande">
            </div>

            <div class="campo">
                <label>Motivo del cambio:</label>
                <input type="text" v-model="itemAjustar.motivo" placeholder="Ej: Rotura, Error de carga, Recuento semestral">
            </div>

            <div class="botones-modal">
                <button @click="mostrandoModal = false" class="btn-cancelar">Cancelar</button>
                <button @click="guardarAjuste" class="btn-guardar">💾 Confirmar Cambio</button>
            </div>
        </div>
    </div>
  </div>
</template>

<style scoped>
/* Tus estilos anteriores + Estilos del Modal */
.panel-stock { max-width: 1000px; margin: 0 auto; }
.kpi-container { display: flex; gap: 20px; margin-bottom: 20px; }
.card-kpi { flex: 1; background: white; padding: 15px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); text-align: center; }
.card-kpi strong { display: block; font-size: 1.8em; color: #2c3e50; margin-top: 5px; }
.card-kpi.danger strong { color: #e74c3c; }
.card-kpi.money strong { color: #27ae60; }
.buscador { display: flex; gap: 10px; margin-bottom: 15px; }
.buscador input { flex: 1; padding: 10px; border: 1px solid #ccc; border-radius: 5px; }
.tabla-stock { width: 100%; border-collapse: collapse; background: white; }
.tabla-stock th, .tabla-stock td { padding: 12px; text-align: left; border-bottom: 1px solid #eee; }
.numero { font-weight: bold; }
.fila-critica { background-color: #fff5f5; }
.fila-critica td { color: #c0392b; }
.btn-editar { background: #f39c12; color: white; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; }

/* MODAL */
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 25px; border-radius: 10px; width: 400px; box-shadow: 0 5px 15px rgba(0,0,0,0.3); }
.modal-content h3 { margin-top: 0; color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }
.modal-content .campo { margin-bottom: 15px; }
.modal-content label { display: block; margin-bottom: 5px; font-weight: bold; }
.modal-content input { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; } /* box-sizing clave para que no se salga */
.input-grande { font-size: 1.2em; font-weight: bold; color: #2980b9; }
.botones-modal { display: flex; justify-content: flex-end; gap: 10px; margin-top: 20px; }
.btn-cancelar { background: #95a5a6; color: white; border: none; padding: 10px 15px; border-radius: 5px; cursor: pointer; }
.btn-guardar { background: #27ae60; color: white; border: none; padding: 10px 15px; border-radius: 5px; cursor: pointer; }
</style>