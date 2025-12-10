<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

const listaInsumos = ref([])
const listaUltimosMovimientos = ref([]) // <--- NUEVO: Para mostrar historial reciente

const form = ref({
  productoId: '',
  cantidad: 0,
  proveedor: '',
  costoTotalFactura: 0
})

const mensaje = ref('')
const error = ref('')

const apiUrl = 'https://localhost:7244/api' 

// Calculamos el unitario automáticamente
const precioUnitarioCalculado = computed(() => {
    if (form.value.cantidad > 0 && form.value.costoTotalFactura > 0) {
        return (form.value.costoTotalFactura / form.value.cantidad).toFixed(2)
    }
    return "0.00"
})

onMounted(async () => {
  await cargarInsumos();
  await cargarHistorialReciente(); // <--- Cargamos historial al entrar
})

async function cargarInsumos() {
    try {
        const res = await fetch(`${apiUrl}/Stock/materias-primas`)
        if (res.ok) listaInsumos.value = await res.json()
    } catch (e) { console.error("Error cargando insumos") }
}

// --- NUEVA FUNCIÓN: CARGAR HISTORIAL RECIENTE ---
async function cargarHistorialReciente() {
    try {
        const res = await fetch(`${apiUrl}/Movimientos`)
        if (res.ok) {
            const todos = await res.json()
            // Filtramos solo ENTRADAS y mostramos las ultimas 5
            listaUltimosMovimientos.value = todos
                .filter((m: any) => m.tipoMovimiento.includes("ENTRADA") || m.tipoMovimiento.includes("Ingreso"))
                .slice(0, 5) 
        }
    } catch (e) { console.error("Error cargando historial") }
}

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

  try {
    const res = await fetch(`${apiUrl}/Stock/ingresar`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            productoId: form.value.productoId,
            cantidad: form.value.cantidad,
            proveedor: form.value.proveedor,
            nuevoPrecio: precioUnitarioParaEnviar 
        })
    })

    if (!res.ok) throw new Error('Error al guardar ingreso')

    mensaje.value = `✅ Ingreso guardado. Costo unitario actualizado a $${precioUnitarioCalculado.value}`
    
    // Limpiamos form
    form.value.cantidad = 0
    form.value.costoTotalFactura = 0
    form.value.proveedor = ''
    
    // Recargamos datos para ver cambios al instante
    await cargarInsumos()
    await cargarHistorialReciente()

  } catch (e) {
    error.value = "❌ Error al conectar con el servidor"
  }
}

// --- NUEVA FUNCIÓN: ELIMINAR (DESHACER) ---
async function eliminarMovimiento(id: number) {
    if(!confirm("⚠️ ¿Te equivocaste? \nAl eliminar este ingreso, se descontará el stock automáticamente.")) return;

    try {
        const res = await fetch(`${apiUrl}/Movimientos/eliminar/${id}`, { method: 'DELETE' });
        if (res.ok) {
            alert("✅ Ingreso eliminado correctamente.");
            await cargarHistorialReciente(); // Refrescar lista
            await cargarInsumos(); // Refrescar stock en el select
        } else {
            alert("❌ No se pudo eliminar.");
        }
    } catch (e) { alert("Error de conexión"); }
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
                      <td>{{ mov.fecha.split(' ')[0] }}</td> <td>{{ mov.producto }}</td>
                      <td style="font-weight:bold; color:green;">+{{ mov.cantidad }}</td>
                      <td>
                          <button @click="eliminarMovimiento(mov.id)" class="btn-undo" title="Deshacer ingreso">
                              ↩️ Deshacer
                          </button>
                      </td>
                  </tr>
                  <tr v-if="listaUltimosMovimientos.length === 0">
                      <td colspan="4" style="text-align:center; color:#888;">No hay cargas recientes hoy.</td>
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