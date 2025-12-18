<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'

// --- 1. INTERFACES ---
interface Insumo {
    id: number;
    nombre: string;
    stockActual: number;
}

interface Proveedor {
    id: number;
    razonSocial: string;
}

interface Movimiento {
    id: number;
    fecha: string;
    producto: string;
    cantidad: number;
    proveedor: string; 
    precioUnitario: number;
    loteProveedor: string;
    tipoMovimiento: string;
}

// --- ESTADO ---
const listaInsumos = ref<Insumo[]>([])
const listaProveedores = ref<Proveedor[]>([]) 
const listaUltimosMovimientos = ref<Movimiento[]>([]) 

const form = ref({
  productoId: '' as number | '', 
  proveedorId: '' as number | '', 
  cantidad: 0,
  precioUnitario: 0, // <--- CAMBIO: Ahora el usuario ingresa esto
  numeroRemito: '',
  lote: '' 
})

const mensaje = ref('')
const error = ref('')

const apiUrl = 'https://localhost:7244/api' 

// --- HELPER TOKEN ---
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// CAMBIO: Calculamos el TOTAL DE FACTURA (Solo visual)
const totalEstimadoFactura = computed(() => {
    if (form.value.cantidad > 0 && form.value.precioUnitario > 0) {
        return (form.value.cantidad * form.value.precioUnitario).toFixed(2)
    }
    return "0.00"
})

onMounted(async () => {
  await cargarInsumos();
  await cargarProveedores();
  await cargarHistorialReciente(); 
})

// --- CARGA DE DATOS ---
async function cargarInsumos() {
    try {
        const res = await axios.get(`${apiUrl}/Productos/materias-primas`, getAuthConfig())
        listaInsumos.value = res.data
    } catch (e: any) { 
        console.error("Error cargando insumos", e)
    }
}

async function cargarProveedores() {
    try {
        const res = await axios.get(`${apiUrl}/Proveedores`, getAuthConfig())
        listaProveedores.value = res.data
    } catch (e: any) {
        console.error("Error cargando proveedores", e)
        error.value = "No se pudieron cargar los proveedores."
    }
}

async function cargarHistorialReciente() {
    try {
        const res = await axios.get(`${apiUrl}/Movimientos`, getAuthConfig())
        listaUltimosMovimientos.value = res.data
            .filter((m: Movimiento) => m.tipoMovimiento === 'COMPRA' || m.tipoMovimiento.includes('ENTRADA'))
            .slice(0, 5) 
            
    } catch (e) { console.error("Error cargando historial", e) }
}

// --- REGISTRAR INGRESO (COMPRA) ---
async function registrarCompra() {
  mensaje.value = ''
  error.value = ''

  // Validaciones
  if (!form.value.productoId) { error.value = "Seleccione un producto."; return; }
  if (!form.value.proveedorId) { error.value = "Seleccione un proveedor."; return; }
  if (form.value.cantidad <= 0) { error.value = "La cantidad debe ser mayor a 0."; return; }
  // Validación extra opcional:
  if (form.value.precioUnitario < 0) { error.value = "El precio no puede ser negativo."; return; }

  // Payload directo (Ya no calculamos nada aquí)
  const payload = {
      productoId: Number(form.value.productoId),
      proveedorId: Number(form.value.proveedorId),
      cantidad: form.value.cantidad,
      precioUnitario: form.value.precioUnitario, // Enviamos directo lo que escribió el usuario
      numeroRemito: form.value.numeroRemito,
      lote: form.value.lote,
      observacion: `Ingreso Web - ${new Date().toLocaleDateString()}`
  }

  try {
    await axios.post(`${apiUrl}/Compras`, payload, getAuthConfig())

    mensaje.value = `✅ Compra registrada. Stock y costos actualizados.`
    
    // Limpieza
    form.value.cantidad = 0
    form.value.precioUnitario = 0 // Reseteamos precio
    form.value.lote = ''
    
    await cargarInsumos()
    await cargarHistorialReciente()

  } catch (e: any) {
    console.error(e)
    const msg = e.response?.data?.mensaje || e.message
    error.value = "❌ Error: " + msg
  }
}

// --- ELIMINAR ---
async function eliminarMovimiento(id: number) {
    if(!confirm("⚠️ ¿Eliminar este registro? Se revertirá el stock.")) return;

    try {
        await axios.delete(`${apiUrl}/Movimientos/eliminar/${id}`, getAuthConfig());
        alert("✅ Eliminado correctamente.");
        await cargarHistorialReciente();
        await cargarInsumos();
    } catch (e: any) { 
        alert("Error: " + (e.response?.data?.mensaje || e.message)); 
    }
}
</script>

<template>
  <div class="contenedor-ingresos">
      
      <div class="hoja-stock">
        <h3>🏭 Registro de Compras (Ingreso)</h3>
        
        <div class="campo">
          <label>Proveedor:</label>
          <select v-model="form.proveedorId" :class="{'input-vacio': !form.proveedorId}">
            <option value="" disabled>-- Seleccione Proveedor --</option>
            <option v-for="prov in listaProveedores" :key="prov.id" :value="prov.id">
                {{ prov.razonSocial }}
            </option>
          </select>
        </div>

        <div class="fila-doble">
             <div class="campo mitad">
                <label>N° Remito / Factura:</label>
                <input type="text" v-model="form.numeroRemito" placeholder="Ej: 0001-000452" />
            </div>
             <div class="campo mitad">
                <label>Lote Proveedor:</label>
                <input type="text" v-model="form.lote" placeholder="Ej: L-2024-X" />
            </div>
        </div>

        <hr class="separador">

        <div class="campo">
          <label>Materia Prima:</label>
          <select v-model="form.productoId">
            <option value="" disabled>-- Seleccione Material --</option>
            <option v-for="p in listaInsumos" :key="p.id" :value="p.id">
                {{ p.nombre }} (Stock: {{ p.stockActual }})
            </option>
          </select>
        </div>

        <div class="fila-doble">
            <div class="campo mitad">
                <label>Cantidad (Kg):</label>
                <input type="number" v-model="form.cantidad" placeholder="0" min="0" step="0.1" />
            </div>

            <div class="campo mitad">
                <label>Precio Unitario ($ / Kg):</label>
                <input type="number" v-model="form.precioUnitario" placeholder="$0.00" min="0" step="0.01" />
            </div>
        </div>
          
        <div class="info-costo" v-if="form.cantidad > 0 && form.precioUnitario > 0">
              💰 Total Estimado Factura: <strong>${{ totalEstimadoFactura }}</strong>
        </div>

        <button class="btn-ingreso" @click="registrarCompra">📥 REGISTRAR COMPRA</button>

        <p v-if="mensaje" class="exito">{{ mensaje }}</p>
        <p v-if="error" class="error">{{ error }}</p>
      </div>

      <div class="historial-rapido">
          <h4>📋 Últimas Compras</h4>
          <table class="tabla-mini">
              <thead>
                  <tr>
                      <th>Fecha</th>
                      <th>Proveedor</th>
                      <th>Producto</th>
                      <th>Lote</th>
                      <th>Cant.</th>
                      <th>$ Unit.</th>
                      <th></th>
                  </tr>
              </thead>
              <tbody>
                  <tr v-for="mov in listaUltimosMovimientos" :key="mov.id">
                      <td>{{ mov.fecha?.split(' ')[0] }}</td> 
                      <td>{{ mov.proveedor || '-' }}</td> 
                      <td>{{ mov.producto }}</td>
                      <td style="font-size: 0.8em; color:#666;">{{ mov.loteProveedor || '-' }}</td>
                      <td style="font-weight:bold; color:green;">{{ mov.cantidad }}</td>
                      <td>${{ mov.precioUnitario }}</td>
                      <td>
                          <button @click="eliminarMovimiento(mov.id)" class="btn-undo">✖</button>
                      </td>
                  </tr>
                  <tr v-if="listaUltimosMovimientos.length === 0">
                      <td colspan="7" style="text-align:center; padding: 20px;">Sin movimientos recientes.</td>
                  </tr>
              </tbody>
          </table>
      </div>

  </div>
</template>

<style scoped>
/* Tus estilos se mantienen exactamente igual */
.contenedor-ingresos { max-width: 700px; margin: 0 auto; font-family: 'Segoe UI', sans-serif; }

.hoja-stock { 
    background: #ffffff; 
    padding: 25px; 
    border: 1px solid #d1d5db; 
    border-radius: 12px; 
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1);
    margin-bottom: 25px;
}

h3 { color: #1f2937; border-bottom: 2px solid #16a34a; padding-bottom: 10px; margin-top: 0;}

.campo { margin-bottom: 15px; text-align: left; }
.campo label { display: block; font-weight: 600; margin-bottom: 5px; color: #374151; font-size: 0.9rem;}
.campo select, .campo input { 
    width: 100%; padding: 10px; 
    border: 1px solid #d1d5db; border-radius: 6px; 
    box-sizing: border-box; font-size: 1rem;
    transition: border-color 0.2s;
}
.campo select:focus, .campo input:focus { outline: none; border-color: #16a34a; ring: 2px solid #16a34a;}

.fila-doble { display: flex; gap: 15px; }
.mitad { flex: 1; }

.separador { border: 0; border-top: 1px dashed #e5e7eb; margin: 20px 0; }

.info-costo {
    background-color: #ecfdf5; color: #065f46;
    padding: 10px; border-radius: 6px; margin-bottom: 15px;
    font-size: 0.9rem; text-align: center; border: 1px solid #a7f3d0;
}

.btn-ingreso { 
    background: #16a34a; color: white; 
    padding: 12px; border: none; border-radius: 6px; 
    cursor: pointer; font-size: 16px; font-weight: bold; width: 100%; 
    transition: background 0.2s;
}
.btn-ingreso:hover { background: #15803d; }

.exito { background: #dcfce7; color: #166534; padding: 10px; border-radius: 5px; margin-top: 10px; text-align: center;}
.error { background: #fee2e2; color: #991b1b; padding: 10px; border-radius: 5px; margin-top: 10px; text-align: center;}

.historial-rapido { background: white; padding: 0; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.05); border: 1px solid #eee; }
.historial-rapido h4 { background: #f9fafb; padding: 15px; margin: 0; border-bottom: 1px solid #eee; color: #374151; }
.tabla-mini { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.tabla-mini th { background: #f3f4f6; text-align: left; padding: 10px; font-weight: 600; color: #4b5563; }
.tabla-mini td { border-bottom: 1px solid #f3f4f6; padding: 10px; color: #1f2937; }
.btn-undo { background: transparent; border: none; color: #ef4444; cursor: pointer; font-size: 1.1rem; }
.btn-undo:hover { color: #b91c1c; transform: scale(1.1); }
</style>