<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'

// --- 1. INTERFACES (Tipado Estricto) ---
interface Insumo {
    id: number;
    nombre: string;
    stockActual: number;
    // Agrega otras propiedades si las necesitas
}

interface Movimiento {
    id: number;
    fecha: string;
    producto: string; // Nombre del producto
    cantidad: number;
    tipoMovimiento: string;
}

// --- ESTADO ---
const listaInsumos = ref<Insumo[]>([])
const listaUltimosMovimientos = ref<Movimiento[]>([]) 

const form = ref({
  productoId: '' as number | '', // Puede estar vacío al inicio
  cantidad: 0,
  proveedor: '',
  costoTotalFactura: 0
})

const mensaje = ref('')
const error = ref('')

const apiUrl = 'https://localhost:7244/api' 

// --- HELPER TOKEN ---
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// Calculamos el unitario automáticamente
const precioUnitarioCalculado = computed(() => {
    if (form.value.cantidad > 0 && form.value.costoTotalFactura > 0) {
        return (form.value.costoTotalFactura / form.value.cantidad).toFixed(2)
    }
    return "0.00"
})

onMounted(async () => {
  await cargarInsumos();
  await cargarHistorialReciente(); 
})

// --- CARGA DE DATOS ---
async function cargarInsumos() {
    try {
        const res = await axios.get(`${apiUrl}/Stock/materias-primas`, getAuthConfig())
        listaInsumos.value = res.data
    } catch (e: any) { 
        console.error("Error cargando insumos:", e)
        if (e.response?.status === 401) error.value = "Sesión expirada."
    }
}

async function cargarHistorialReciente() {
    try {
        const res = await axios.get(`${apiUrl}/Movimientos`, getAuthConfig())
        const todos: Movimiento[] = res.data
        
        // Filtramos solo ENTRADAS y mostramos las ultimas 5
        // TypeScript ahora sabe que 'm' es un Movimiento
        listaUltimosMovimientos.value = todos
            .filter(m => m.tipoMovimiento.toUpperCase().includes("ENTRADA") || m.tipoMovimiento.toUpperCase().includes("INGRESO"))
            .slice(0, 5) 
            
    } catch (e) { console.error("Error cargando historial", e) }
}

// --- REGISTRAR INGRESO ---
async function registrarIngreso() {
  mensaje.value = ''
  error.value = ''

  if (!form.value.productoId || form.value.cantidad <= 0) {
    error.value = "Seleccione un producto y una cantidad válida."
    return
  }

  let precioUnitarioParaEnviar = 0
  if (form.value.costoTotalFactura > 0) {
      precioUnitarioParaEnviar = form.value.costoTotalFactura / form.value.cantidad
  }

  const payload = {
      productoId: Number(form.value.productoId), // Aseguramos que sea número
      cantidad: form.value.cantidad,
      proveedor: form.value.proveedor,
      nuevoPrecio: precioUnitarioParaEnviar 
  }

  try {
    await axios.post(`${apiUrl}/Stock/ingresar`, payload, getAuthConfig())

    mensaje.value = `✅ Ingreso guardado. Stock actualizado.`
    
    // Limpiamos form
    form.value.cantidad = 0
    form.value.costoTotalFactura = 0
    form.value.proveedor = ''
    // Nota: No limpiamos el productoId por si quiere cargar otro lote del mismo
    
    // Recargamos datos para ver cambios al instante
    await cargarInsumos()
    await cargarHistorialReciente()

  } catch (e: any) {
    console.error(e)
    const msg = e.response?.data?.mensaje || e.message
    error.value = "❌ Error: " + msg
  }
}

// --- ELIMINAR (DESHACER) ---
async function eliminarMovimiento(id: number) {
    if(!confirm("⚠️ ¿Te equivocaste? \nAl eliminar este ingreso, se descontará el stock automáticamente.")) return;

    try {
        await axios.delete(`${apiUrl}/Movimientos/eliminar/${id}`, getAuthConfig());
        
        alert("✅ Ingreso eliminado correctamente.");
        await cargarHistorialReciente(); // Refrescar lista
        await cargarInsumos(); // Refrescar stock en el select

    } catch (e: any) { 
        alert("Error al eliminar: " + (e.response?.data?.mensaje || e.message)); 
    }
}
</script>

<template>
  <div class="contenedor-ingresos">
      
      <div class="hoja-stock">
        <h3>🚚 Ingreso de Mercadería (Compras)</h3>
        
        <div class="campo">
          <label>Materia Prima:</label>
          <select v-model="form.productoId">
            <option value="" disabled>Seleccione Insumo...</option>
            <option v-for="p in listaInsumos" :key="p.id" :value="p.id">
                {{ p.nombre }} (Stock actual: {{ p.stockActual }})
            </option>
          </select>
        </div>

        <div class="campo">
          <label>Cantidad (Kg/Uni):</label>
          <input type="number" v-model="form.cantidad" placeholder="0" min="0" step="0.1" />
        </div>

        <div class="campo">
          <label>Total de la Factura ($):</label>
          <div style="display: flex; gap: 5px; align-items: center;">
              <input type="number" v-model="form.costoTotalFactura" placeholder="Monto total pagado" min="0" step="0.01" />
          </div>
          
          <small v-if="form.costoTotalFactura > 0" style="color: #27ae60; font-weight: bold;">
              ℹ️ Costo Unitario resultante: ${{ precioUnitarioCalculado }} / kg
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

      <div class="historial-rapido">
          <h4>🕒 Últimos Ingresos Cargados</h4>
          <table class="tabla-mini">
              <thead>
                  <tr>
                      <th>Fecha</th>
                      <th>Producto</th>
                      <th>Cant.</th>
                      <th>Acción</th>
                  </tr>
              </thead>
              <tbody>
                  <tr v-for="mov in listaUltimosMovimientos" :key="mov.id">
                      <td>{{ mov.fecha?.split(' ')[0] || '-' }}</td> 
                      <td>{{ mov.producto }}</td>
                      <td style="font-weight:bold; color:green;">+{{ mov.cantidad }}</td>
                      <td>
                          <button @click="eliminarMovimiento(mov.id)" class="btn-undo" title="Deshacer ingreso">
                              ↩️ Deshacer
                          </button>
                      </td>
                  </tr>
                  <tr v-if="listaUltimosMovimientos.length === 0">
                      <td colspan="4" style="text-align:center; color:#888;">No hay cargas recientes.</td>
                  </tr>
              </tbody>
          </table>
      </div>

  </div>
</template>

<style scoped>
.contenedor-ingresos {
    max-width: 600px;
    margin: 0 auto;
}

.hoja-stock { 
    background: #f0fdf4; /* Verde muy clarito */
    padding: 20px; 
    border: 1px solid #4ade80; 
    border-radius: 8px; 
    margin-bottom: 20px;
}

.campo { margin-bottom: 15px; text-align: left; }
.campo label { display: block; font-weight: bold; margin-bottom: 5px; }
.campo select, .campo input { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; }
.btn-ingreso { background: #16a34a; color: white; padding: 10px 20px; border: none; border-radius: 5px; cursor: pointer; font-size: 16px; width: 100%; }
.btn-ingreso:hover { background: #15803d; }
.exito { color: green; font-weight: bold; margin-top: 10px; }
.error { color: red; font-weight: bold; margin-top: 10px; }

/* Estilos Historial Rápido */
.historial-rapido {
    background: white;
    padding: 15px;
    border-radius: 8px;
    border: 1px solid #eee;
    box-shadow: 0 2px 5px rgba(0,0,0,0.05);
}
.historial-rapido h4 { margin-top: 0; color: #555; border-bottom: 1px solid #eee; padding-bottom: 5px;}

.tabla-mini { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
.tabla-mini th { text-align: left; color: #666; border-bottom: 1px solid #ddd; padding: 5px; }
.tabla-mini td { border-bottom: 1px solid #eee; padding: 8px 5px; }

.btn-undo {
    background: white;
    border: 1px solid #e74c3c;
    color: #e74c3c;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.8rem;
    padding: 2px 6px;
}
.btn-undo:hover { background: #e74c3c; color: white; }
</style>