<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/axiosInstance'; // Usamos la instancia segura

// --- 1. INTERFACES ---
interface Insumo {
    id: number;
    nombre: string;
    stockActual: number;
    // Propiedades para filtrar
    esScrap?: boolean;
    esMateriaPrima?: boolean;
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
const listaMovimientosBruta = ref<Movimiento[]>([]) 

// Filtros de Fecha
const filtroFechaDesde = ref('')
const filtroFechaHasta = ref('')

const form = ref({
  productoId: '' as number | '', 
  proveedorId: '' as number | '', 
  cantidad: 0,
  precioUnitario: 0, 
  numeroRemito: '',
  lote: '' 
})

const mensaje = ref('')
const error = ref('')

// Computed: Total Factura
const totalEstimadoFactura = computed(() => {
    if (form.value.cantidad > 0 && form.value.precioUnitario > 0) {
        return (form.value.cantidad * form.value.precioUnitario).toFixed(2)
    }
    return "0.00"
})

// --- 🔥 COMPUTED: FILTRO SOLO COMPRAS REALES ---
const movimientosFiltrados = computed(() => {
    let lista = listaMovimientosBruta.value;

    // 1. FILTRAR SOLO COMPRAS (Excluir Producción y Scrap Interno)
    lista = lista.filter(m => 
        m.tipoMovimiento === 'COMPRA' || 
        m.tipoMovimiento === 'ENTRADA'
    );

    // 2. Filtro por Fecha DESDE
    if (filtroFechaDesde.value) {
        const desde = new Date(filtroFechaDesde.value);
        desde.setHours(0,0,0,0); 
        lista = lista.filter(m => new Date(m.fecha) >= desde);
    }

    // 3. Filtro por Fecha HASTA
    if (filtroFechaHasta.value) {
        const hasta = new Date(filtroFechaHasta.value);
        hasta.setHours(23,59,59,999); 
        lista = lista.filter(m => new Date(m.fecha) <= hasta);
    }

    // Ordenar descendente (más nuevo primero)
    return lista.sort((a, b) => new Date(b.fecha).getTime() - new Date(a.fecha).getTime());
});

onMounted(async () => {
  await cargarInsumos();
  await cargarProveedores();
  await cargarHistorialCompras(); 
})

// 🔥 FILTRO BLINDADO: SOLO MATERIA PRIMA VIRGEN
async function cargarInsumos() {
    try {
        const res = await api.get('/Productos');
        
        // Filtramos para que NO aparezca Scrap, ni Recuperado, ni nada sucio
        listaInsumos.value = res.data.filter((p: any) => {
            const nombre = p.nombre ? p.nombre.toUpperCase() : '';
            return (
                p.esMateriaPrima === true &&       // Es MP
                p.esScrap !== true &&              // NO es Scrap
                !nombre.includes("[SCRAP]") &&     // NO dice Scrap
                !nombre.includes("RECUPERADO") &&  // NO es Recuperado
                !nombre.includes("[REC]")          // NO es Recuperado (alias)
            );
        });
    } catch (e) { console.error(e) }
}

async function cargarProveedores() {
    try {
        const res = await api.get('/Proveedores');
        listaProveedores.value = res.data;
    } catch (e) { console.error(e) }
}

async function cargarHistorialCompras() {
    try {
        const res = await api.get('/Movimientos');
        listaMovimientosBruta.value = res.data;
    } catch (e) { console.error(e) }
}

async function registrarCompra() {
  mensaje.value = ''
  error.value = ''

  if (!form.value.productoId) { error.value = "Seleccione un producto."; return; }
  if (!form.value.proveedorId) { error.value = "Seleccione un proveedor."; return; }
  if (form.value.cantidad <= 0) { error.value = "La cantidad debe ser mayor a 0."; return; }

  const payload = {
      productoId: Number(form.value.productoId),
      proveedorId: Number(form.value.proveedorId),
      cantidad: form.value.cantidad,
      precioUnitario: form.value.precioUnitario,
      numeroRemito: form.value.numeroRemito,
      lote: form.value.lote,
      observacion: `Ingreso Web - ${new Date().toLocaleDateString()}`
  }

  try {
    await api.post('/Compras', payload);
    
    mensaje.value = `✅ Compra registrada correctamente. Stock Actualizado.`
    
    // Resetear formulario
    form.value.cantidad = 0
    form.value.precioUnitario = 0
    form.value.lote = ''
    form.value.numeroRemito = ''
    
    await cargarInsumos()
    await cargarHistorialCompras()

  } catch (e: any) {
    const msg = e.response?.data?.mensaje || e.message || "Error desconocido";
    error.value = "❌ Error: " + msg;
  }
}

async function eliminarMovimiento(id: number) {
    if(!confirm("⚠️ ¿Eliminar este registro de compra? Se descontará el stock.")) return;
    try {
        await api.delete(`/Movimientos/eliminar/${id}`);
        alert("✅ Eliminado correctamente.");
        await cargarHistorialCompras();
        await cargarInsumos();
    } catch (e: any) { 
        alert("Error: " + (e.response?.data?.mensaje || e.message)); 
    }
}
</script>

<template>
  <div class="contenedor-ingresos">
      
      <div class="hoja-stock">
        <h3>🏭 Registro de Compras (Materia Prima)</h3>
        
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
          <label>Materia Prima (Insumos):</label>
          <select v-model="form.productoId">
            <option value="" disabled>-- Seleccione Material --</option>
            <option v-for="p in listaInsumos" :key="p.id" :value="p.id">
                {{ p.nombre }} (Stock: {{ p.stockActual }})
            </option>
          </select>
          <small v-if="listaInsumos.length === 0" style="color:red">
              No hay materias primas registradas (El scrap está oculto).
          </small>
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
          <div class="header-historial">
              <h4>📋 Historial de Compras</h4>
              
              <div class="filtros-fecha">
                  <div class="filtro-item">
                      <small>Desde:</small>
                      <input type="date" v-model="filtroFechaDesde">
                  </div>
                  <div class="filtro-item">
                      <small>Hasta:</small>
                      <input type="date" v-model="filtroFechaHasta">
                  </div>
              </div>
          </div>

          <table class="tabla-mini">
              <thead>
                  <tr>
                      <th>Fecha</th>
                      <th>Proveedor</th>
                      <th>Producto</th>
                      <th>Lote / Remito</th>
                      <th>Cant.</th>
                      <th>$ Unit.</th>
                      <th></th>
                  </tr>
              </thead>
              <tbody>
                  <tr v-for="mov in movimientosFiltrados" :key="mov.id">
                      <td>{{ new Date(mov.fecha).toLocaleDateString() }}</td> 
                      
                      <td>{{ mov.proveedor || '-' }}</td>
                      
                      <td>{{ mov.producto }}</td>
                      
                      <td style="font-size: 0.8em; color:#666;">
                          {{ mov.loteProveedor || '-' }}
                      </td>
                      
                      <td style="font-weight:bold; color:green;">+{{ mov.cantidad }}</td>
                      
                      <td>
                          <span v-if="mov.precioUnitario > 0">${{ mov.precioUnitario }}</span>
                          <span v-else style="color:#aaa">-</span>
                      </td>
                      
                      <td>
                          <button @click="eliminarMovimiento(mov.id)" class="btn-undo" title="Eliminar Compra">✖</button>
                      </td>
                  </tr>
                  <tr v-if="movimientosFiltrados.length === 0">
                      <td colspan="7" style="text-align:center; padding: 20px; color: #666;">
                          No hay compras registradas en este período.
                      </td>
                  </tr>
              </tbody>
          </table>
      </div>
  </div>
</template>

<style scoped>
.contenedor-ingresos { max-width: 850px; margin: 0 auto; font-family: 'Segoe UI', sans-serif; }

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

/* Historial */
.historial-rapido { background: white; padding: 0; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.05); border: 1px solid #eee; }
.header-historial { background: #f9fafb; padding: 15px; border-bottom: 1px solid #eee; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 10px; }
.header-historial h4 { margin: 0; color: #374151; }
.filtros-fecha { display: flex; gap: 10px; align-items: center; }
.filtro-item { display: flex; align-items: center; gap: 5px; }
.tabla-mini { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.tabla-mini th { background: #f3f4f6; text-align: left; padding: 10px; font-weight: 600; color: #4b5563; }
.tabla-mini td { border-bottom: 1px solid #f3f4f6; padding: 10px; color: #1f2937; }
.btn-undo { background: transparent; border: none; color: #ef4444; cursor: pointer; font-size: 1.1rem; }
.btn-undo:hover { color: #b91c1c; transform: scale(1.1); }
</style>