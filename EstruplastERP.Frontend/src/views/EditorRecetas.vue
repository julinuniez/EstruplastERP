<script setup lang="ts">
import { ref, onMounted, watch } from 'vue'
import axios from 'axios'

// 1. DEFINIMOS LAS "PLANTILLAS" DE LOS DATOS (INTERFACES)
interface Producto {
    id: number;
    nombre: string;
    codigoSku: string;
    esProductoTerminado: boolean;
}

interface ItemReceta {
    id: number;
    cantidad: number;
    nombreMostrar: string; // La propiedad que creamos manualmente
}

// 2. USAMOS ESAS PLANTILLAS EN LOS REFS
const terminados = ref<Producto[]>([]) // "Esta lista guardará Productos"
const insumos = ref<Producto[]>([])    // "Esta también"
const recetaActual = ref<ItemReceta[]>([]) 

const prodSeleccionado = ref('') // ID del producto (puede ser string o number)
const formIngrediente = ref({
  materiaPrimaId: '',
  cantidad: 0
})

const error = ref('')
const cargando = ref(false)

const apiUrl = 'https://localhost:7244/api' 

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

onMounted(async () => {
  try {
    const res = await axios.get(`${apiUrl}/Productos`, getAuthConfig())
    const data = res.data
    
    // Ahora TypeScript sabe que 'p' es un Producto
    terminados.value = data.filter((p: Producto) => p.esProductoTerminado)
    insumos.value = data.filter((p: Producto) => !p.esProductoTerminado)

  } catch (e: any) {
    console.error("Error al cargar productos:", e)
    if (e.response?.status === 401) error.value = "Sesión expirada."
  }
})

watch(prodSeleccionado, async (newId) => {
  if(!newId) return
  cargarReceta(newId)
})

async function cargarReceta(id: any) {
  cargando.value = true
  try {
    const res = await axios.get(`${apiUrl}/Formulas/${id}`, getAuthConfig())
    
    recetaActual.value = res.data.map((r: any) => ({
        ...r,
        nombreMostrar: r.ingrediente || r.Ingrediente || 'Insumo'
    }))
    
  } catch (e) {
    recetaActual.value = [] 
  } finally {
    cargando.value = false
  }
}

async function agregarIngrediente() {
  if (!formIngrediente.value.materiaPrimaId || formIngrediente.value.cantidad <= 0) {
      alert("Datos inválidos.")
      return
  }

  const payload = {
    productoTerminadoId: prodSeleccionado.value,
    materiaPrimaId: formIngrediente.value.materiaPrimaId,
    cantidad: formIngrediente.value.cantidad
  }

  try {
    await axios.post(`${apiUrl}/Formulas`, payload, getAuthConfig())
    await cargarReceta(prodSeleccionado.value) 
    formIngrediente.value.cantidad = 0
  } catch (e: any) {
    alert("Error: " + (e.response?.data || e.message))
  }
}

async function borrarIngrediente(id: number) {
  if(!confirm("¿Borrar?")) return
  try {
      await axios.delete(`${apiUrl}/Formulas/${id}`, getAuthConfig())
      cargarReceta(prodSeleccionado.value)
  } catch (e: any) { alert(e.message) }
}
</script>

<template>
  <div class="panel-receta">
    <h3>🧪 Editor de Fórmulas</h3>
    
    <div v-if="error" class="error-box">{{ error }}</div>

    <div class="seccion-header">
      <label>1. Selecciona el Producto a Fabricar:</label>
      <select v-model="prodSeleccionado" class="select-big">
        <option disabled value="">-- Elegir Producto Terminado --</option>
        <option v-for="p in terminados" :key="p.id" :value="p.id">
            {{ p.nombre }} ({{ p.codigoSku }})
        </option>
      </select>
    </div>

    <div v-if="prodSeleccionado" class="zona-carga">
      <h4>
          Fórmula para: 
          <span style="color: #2196f3">
            {{ terminados.find(p => p.id == Number(prodSeleccionado))?.nombre || '...' }}
          </span>
      </h4>

      <div class="form-inline">
        <select v-model="formIngrediente.materiaPrimaId">
          <option disabled value="">-- Seleccionar Insumo --</option>
          <option v-for="i in insumos" :key="i.id" :value="i.id">
            {{ i.nombre }}
          </option>
        </select>
        
        <input type="number" v-model="formIngrediente.cantidad" placeholder="Kg" step="0.001">
        <button @click="agregarIngrediente" class="btn-add">➕ Agregar</button>
      </div>

      <div v-if="cargando" style="text-align:center; color:#666">Cargando...</div>

      <table v-else class="tabla-receta">
        <thead><tr><th>Ingrediente</th><th>Kg</th><th>Acción</th></tr></thead>
        <tbody>
          <tr v-for="r in recetaActual" :key="r.id">
            <td>{{ r.nombreMostrar }}</td> 
            <td>{{ r.cantidad }}</td>
            <td><button class="btn-rojo" @click="borrarIngrediente(r.id)">🗑️</button></td>
          </tr>
          <tr v-if="recetaActual.length === 0">
              <td colspan="3" style="text-align:center">Sin ingredientes.</td>
          </tr>
        </tbody>
      </table>
    </div>
    
    <div v-else class="placeholder-text">👆 Selecciona un producto arriba.</div>
  </div>
</template>

<style scoped>
/* (Pega aquí los mismos estilos CSS que tenías antes, esos estaban bien) */
.panel-receta { background: #fff; padding: 25px; border: 1px solid #ddd; border-radius: 8px; max-width: 800px; margin: 0 auto; box-shadow: 0 2px 10px rgba(0,0,0,0.05); }
.error-box { background: #ffebee; color: #c62828; padding: 10px; border-radius: 4px; margin-bottom: 15px; border: 1px solid #ef9a9a; }
h3 { margin-top: 0; color: #333; border-bottom: 2px solid #2196f3; padding-bottom: 10px; margin-bottom: 20px; }
h4 { margin: 20px 0 10px 0; color: #555; }
.select-big { width: 100%; padding: 12px; font-size: 16px; margin-bottom: 20px; border: 1px solid #ccc; border-radius: 4px; background-color: #f9f9f9; }
.form-inline { display: flex; gap: 10px; margin-bottom: 20px; background: #f1f8e9; padding: 15px; border-radius: 6px; border: 1px solid #c5e1a5; align-items: center; }
.form-inline select { flex-grow: 1; padding: 8px; border-radius: 4px; border: 1px solid #aaa; }
.form-inline input { width: 100px; padding: 8px; border-radius: 4px; border: 1px solid #aaa; }
.btn-add { background: #4caf50; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-add:hover { background: #43a047; }
.tabla-receta { width: 100%; background: white; border-collapse: collapse; margin-top: 10px; }
.tabla-receta th { background: #eceff1; color: #333; font-weight: bold; }
.tabla-receta th, .tabla-receta td { border: 1px solid #ddd; padding: 10px; text-align: left; }
.btn-rojo { background: #ff5252; color: white; border: none; cursor: pointer; padding: 5px 10px; border-radius: 4px; }
.btn-rojo:hover { background: #d32f2f; }
.placeholder-text { text-align: center; color: #999; margin-top: 30px; font-style: italic; }
</style>