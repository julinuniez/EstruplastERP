<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
// @ts-ignore
import html2pdf from 'html2pdf.js'

// --- ESTADO ---
const productos = ref([])
const empleados = ref([])
const clientes = ref([]) 
const recetaActual = ref([])
const emit = defineEmits(['guardado'])

const form = ref({
  productoTerminadoId: '',
  clienteId: '', 
  cantidad: 1,
  empleadoId: '',
  observacion: '',
  turno: 'Mañana',
  conBrillo: false,
  conCorona: false,
  kilosTotales: 0
})

const mensaje = ref('')
const error = ref('')
const estadoStock = ref({ posible: false, mensaje: '' })
const idProduccionGenerada = ref(false)

const apiUrl = 'https://localhost:7244/api' // ⚠️ REVISA QUE COINCIDA CON TU PUERTO SWAGGER

// --- COMPUTADOS (Buscadores) ---
const productoSeleccionado = computed(() => {
  const p: any = productos.value.find((x: any) => x.id === form.value.productoTerminadoId)
  return p || null
})

const empleadoSeleccionado = computed(() => {
    return empleados.value.find((x: any) => x.id === form.value.empleadoId)
})

const clienteSeleccionado = computed(() => {
    return clientes.value.find((x: any) => x.id === form.value.clienteId)
})

// --- CARGA INICIAL (API) ---
onMounted(async () => {
  try {
    // 1. Productos
    const resProd = await fetch(`${apiUrl}/Productos`)
    const dataProd = await resProd.json()
    productos.value = dataProd.filter((p: any) => p.esProductoTerminado)

    // 2. Empleados
    const resEmp = await fetch(`${apiUrl}/Empleados`)
    if (resEmp.ok) empleados.value = await resEmp.json()

    // 3. Clientes
    const resCli = await fetch(`${apiUrl}/Clientes`)
    if (resCli.ok) clientes.value = await resCli.json()

  } catch (e) {
    error.value = 'Error conectando con el servidor. Revise que el Backend esté corriendo.'
  }
})

// --- CARGAR RECETA (Para mostrar en PDF) ---
async function cargarReceta(id: any) {
    try {
        const res = await fetch(`${apiUrl}/Formulas/${id}`)
        if(res.ok) recetaActual.value = await res.json()
        else recetaActual.value = []
    } catch { recetaActual.value = [] }
}

// --- EL GRAN OBSERVADOR (Calculadora + Semáforo) ---
watch(() => [form.value.productoTerminadoId, form.value.cantidad], ([newId, newCant]) => {
  
  if (newId) cargarReceta(newId)
  
  // 1. Semáforo
  verificarDisponibilidad()

  // 2. Calculadora de Ingeniería (Peso Teórico)
  if (productoSeleccionado.value) {
    const p = productoSeleccionado.value
    
    // Si tenemos todas las medidas cargadas en la BD
    if (p.largo > 0 && p.ancho > 0 && p.espesor > 0) {
      // Usamos el Peso Especifico real del producto (o 1.05 PAI por defecto si es 0)
      const densidad = p.pesoEspecifico > 0 ? p.pesoEspecifico : 1.05 
      
      // Fórmula: (Largo * Ancho * Espesor * Densidad) / 1.000.000 = Kg por unidad
      const pesoUnitario = (p.largo * p.ancho * p.espesor * densidad) / 1000000
      
      // Total
      form.value.kilosTotales = parseFloat((pesoUnitario * form.value.cantidad).toFixed(2))
    } 
    else if (form.value.kilosTotales === 0) {
        // Si no hay medidas, ponemos 0
        form.value.kilosTotales = 0
    }
  }
})

// --- VERIFICAR STOCK ---
async function verificarDisponibilidad() {
  if (!form.value.productoTerminadoId || form.value.cantidad <= 0) return
  try {
    const respuesta = await fetch(`${apiUrl}/Produccion/verificar`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
          productoTerminadoId: form.value.productoTerminadoId,
          cantidad: form.value.cantidad,
          empleadoId: 0
      })
    })
    estadoStock.value = await respuesta.json()
  } catch (e) { console.error(e) }
}

// --- GUARDAR PRODUCCIÓN ---
async function registrarProduccion() {
  mensaje.value = ''
  error.value = ''
  
  // Concatenamos datos técnicos
  const detalles = ` | Cli: ${clienteSeleccionado.value?.razonSocial || '-'} | Brillo: ${form.value.conBrillo ? 'SI' : 'NO'} | Corona: ${form.value.conCorona ? 'SI' : 'NO'}`
  const obsFinal = (form.value.observacion || '') + detalles

  const payload = {
    productoTerminadoId: form.value.productoTerminadoId,
    clienteId: form.value.clienteId || null,
    cantidad: form.value.cantidad,
    empleadoId: form.value.empleadoId,
    turno: form.value.turno,
    observacion: obsFinal,
    
    // --- CAMBIO IMPORTANTE 1: Enviamos los kilos calculados ---
    // (Asegurate que tu backend espere la propiedad "kilos" o "Kilos")
    kilos: form.value.kilosTotales 
  }
  
  try {
    const respuesta = await fetch(`${apiUrl}/Produccion/registrar`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(payload)
    })

    if (!respuesta.ok) {
        // Intentamos leer el error como JSON
        const dataError = await respuesta.json().catch(() => null)
        
        // Si viene mensaje, lo usamos. Si no, probamos leer errores de validación
        let mensajeError = dataError?.mensaje || 'Error desconocido'
        
        // Truco: Si es error de validación de .NET (el problema de los signos ?), viene en 'errors'
        if (dataError?.errors) {
             // Agarramos el primer error que encontremos
             mensajeError = Object.values(dataError.errors).flat()[0]
        }
        
        throw new Error(mensajeError)
    }
    
    const datos = await respuesta.json()
    mensaje.value = `✅ Producción Guardada. Stock descontado.`
    idProduccionGenerada.value = true 

    console.log("1. Formulario: Enviando aviso 'guardado'...")
    // --- CAMBIO IMPORTANTE 2: Avisamos al padre (App.vue) ---
    emit('guardado') 

  } catch (e: any) {
    error.value = '❌ ' + e.message
  }
}

// --- GENERAR PDF ---
function generarOrdenProduccionPDF() {
  const elemento = document.getElementById('hoja-de-impresion')
  const opciones = {
    margin: 0, 
    filename: `Orden_${form.value.observacion || 'Estruplast'}.pdf`,
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { scale: 2, scrollY: 0 },
    jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
  }
  html2pdf().set(opciones).from(elemento).save()
}
</script>

<template>
  <div class="contenedor-doble">
    
    <div id="hoja-de-impresion" class="hoja-papel">
      
      <div class="cabecera">
        <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAlYAAAA4CAYAAAA/xLYcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkY4MzhCNEFBOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkY4MzhCNEFCOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjgzOEI0QTg4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RjgzOEI0QTk4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6QwnXzAAATQUlEQVR42uxd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" alt="LOGO ESTRUPLAST" class="logo-img"> 
       
        <div class="datos-orden">
            <h3>ORDEN DE PRODUCCIÓN</h3>
            <p>FECHA: <strong>{{ new Date().toLocaleDateString() }}</strong></p>
            <p>TURNO: <strong>{{ form.turno.toUpperCase() }}</strong></p>
        </div>
      </div>
      
      <div class="fila-pdf">
        <div style="display: flex; justify-content: space-between;">
            <div>
                <strong>RESPONSABLE:</strong>
                <span class="dato-relleno">{{ empleadoSeleccionado?.nombreCompleto || '________________' }}</span>
            </div>
            <div>
                <strong>CLIENTE:</strong>
                <span class="dato-relleno">{{ clienteSeleccionado?.razonSocial || '________________' }}</span>
            </div>
        </div>
      </div>

      <div class="caja-producto">
        <div class="titulo-seccion">MATERIAL / PRODUCTO A FABRICAR</div>
        <div class="producto-nombre">
            {{ productoSeleccionado?.nombre || '(SELECCIONE PRODUCTO)' }}
        </div>
        <div class="producto-sku">CÓDIGO INTERNO: {{ productoSeleccionado?.codigoSku }}</div>
      </div>

      <div class="ficha-tecnica">
        <div class="dato-box"><span class="label-tech">COLOR</span><span class="valor-tech">{{ productoSeleccionado?.color || '-' }}</span></div>
        <div class="dato-box"><span class="label-tech">LARGO</span><span class="valor-tech">{{ productoSeleccionado?.largo || 0 }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ANCHO</span><span class="valor-tech">{{ productoSeleccionado?.ancho || 0 }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ESPESOR</span><span class="valor-tech">{{ productoSeleccionado?.espesor || 0 }} mm</span></div>
      </div>

      <div class="ficha-tecnica">
        <div class="dato-box">
            <span class="label-tech">LLEVA BRILLO</span>
            <span class="valor-tech" :class="{'texto-resaltado': form.conBrillo}">{{ form.conBrillo ? 'SÍ' : 'NO' }}</span>
        </div>
        <div class="dato-box">
            <span class="label-tech">TRAT. CORONA</span>
            <span class="valor-tech" :class="{'texto-resaltado': form.conCorona}">{{ form.conCorona ? 'SÍ' : 'NO' }}</span>
        </div>
        <div class="dato-box doble-ancho">
            <span class="label-tech">TOTAL KILOS (Estimado)</span>
            <span class="valor-tech" style="font-size: 18px;">{{ form.kilosTotales }} kg</span>
        </div>
      </div>

      <div v-if="recetaActual.length > 0" class="seccion-receta">
          <div class="titulo-seccion-receta">FÓRMULA DE PRODUCCIÓN</div>
          <table class="tabla-receta">
              <thead><tr><th>INSUMO</th><th>% / CANT</th><th>TOTAL REQUERIDO</th></tr></thead>
              <tbody>
                  <tr v-for="(r, i) in recetaActual" :key="i">
                      <td>{{ r.ingrediente }}</td>
                      <td>{{ r.cantidad }}</td>
                      <td><strong>{{ (r.cantidad * form.cantidad).toFixed(2) }} kg</strong></td>
                  </tr>
              </tbody>
          </table>
      </div>

      <div class="fila-lotes">
        <div class="mitad">
            <strong>CANTIDAD (UNIDADES):</strong>
            <div class="recuadro-gigante">{{ form.cantidad }}</div>
        </div>
        <div class="mitad">
            <strong>N. LOTE / OBS:</strong>
            <div class="recuadro-gigante texto-lote">{{ form.observacion }}</div>
        </div>
      </div>

      <div class="pie-firma">
        <div class="linea-firma">Firma Responsable</div>
        <div class="linea-firma">Firma Calidad / Supervisor</div>
      </div>
    </div>

    <div class="panel-control">
        <h3>⚙️ Configurar Orden</h3>
        
        <label>Turno:</label>
        <select v-model="form.turno"><option>Mañana</option><option>Noche</option></select>

        <label>Responsable:</label>
        <select v-model="form.empleadoId">
            <option v-for="e in empleados" :value="e.id">{{e.nombreCompleto}}</option>
        </select>

        <label>Cliente:</label>
        <select v-model="form.clienteId">
            <option disabled value="">Seleccionar Cliente...</option>
            <option v-for="c in clientes" :value="c.id">{{c.razonSocial}}</option>
        </select>

        <label>Producto:</label>
        <select v-model="form.productoTerminadoId">
            <option v-for="p in productos" :value="p.id">{{p.nombre}}</option>
        </select>

        <div class="fila-checkbox">
            <label class="check-container"><input type="checkbox" v-model="form.conBrillo"> ✨ Brillo</label>
            <label class="check-container"><input type="checkbox" v-model="form.conCorona"> ⚡ Corona</label>
        </div>

        <div class="fila-input">
            <div><label>Cant:</label><input type="number" v-model="form.cantidad" min="1"></div>
            <div><label>Total Kgs:</label><input type="number" v-model="form.kilosTotales" step="0.1"></div>
        </div>
        
        <div class="fila-input">
             <div style="width: 100%"><label>Lote / Obs:</label><input type="text" v-model="form.observacion" placeholder="Ej: 3521" style="width:100%"></div>
        </div>

        <div v-if="form.productoTerminadoId" class="alerta-stock" :class="{ 'ok': estadoStock.posible, 'no': !estadoStock.posible }">
            {{ estadoStock.mensaje }}
        </div>

        <button class="btn-guardar" @click="registrarProduccion" :disabled="!estadoStock.posible || !form.empleadoId">
            💾 GUARDAR ORDEN
        </button>

        <button v-if="idProduccionGenerada" class="btn-imprimir" @click="generarOrdenProduccionPDF">
            🖨️ DESCARGAR PDF
        </button>
        
        <p class="success">{{ mensaje }}</p>
        <p class="error">{{ error }}</p>
    </div>

  </div>
</template>

<style scoped>
/* Layout */
.contenedor-doble { display: flex; flex-direction: row; gap: 30px; justify-content: center; align-items: flex-start; padding: 20px; background-color: #f4f4f4; min-height: 100vh; }

/* Hoja A4 */
.hoja-papel { 
    background: white; 
    width: 210mm; 
    height: 210mm;
    min-height: auto; /* Ahora la altura es automática */
    padding: 10mm;    /* Reducimos un poco el relleno */
    border: 1px solid #ccc; 
    box-shadow: 5px 5px 15px rgba(0,0,0,0.2); 
    color: black; 
    font-family: Arial, sans-serif; 
    box-sizing: border-box; 
    margin: 0 auto; 
}
/* Elementos PDF */
.cabecera { border-bottom: 2px solid black; padding-bottom: 15px; display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.logo-img { max-height: 70px; max-width: 200px; object-fit: contain; } /* Controla tamaño logo */
.datos-orden { text-align: right; }
.datos-orden h3 { margin: 0; text-decoration: underline; font-size: 20px; font-weight: 900; }
.datos-orden p { margin: 5px 0 0 0; font-size: 13px; }

.fila-pdf { margin-bottom: 20px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; }

.caja-producto { border: 2px solid black; padding: 10px; margin-bottom: 15px; text-align: center; background: #f9f9f9; }
.titulo-seccion { font-size: 11px; font-weight: bold; margin-bottom: 5px; letter-spacing: 1px; }
.producto-nombre { font-size: 20px; font-weight: 900; }
.producto-sku { font-size: 14px; margin-top: 5px; }

.ficha-tecnica { display: flex; border: 2px solid black; margin-bottom: 10px; }
.dato-box { flex: 1; border-right: 1px solid black; text-align: center; padding: 5px; }
.dato-box:last-child { border-right: none; }
.dato-box.doble-ancho { flex: 2; background: #e8e8e8; }

.label-tech { display: block; font-size: 10px; font-weight: bold; color: #333; }
.valor-tech { font-size: 15px; font-weight: bold; margin-top: 2px; display: block; }
.texto-resaltado { color: black; font-weight: 900; font-size: 16px; text-decoration: underline; }

/* Receta en PDF */
.seccion-receta { margin-top: 20px; margin-bottom: 20px; border: 1px solid black; font-size: 12px; }
.titulo-seccion-receta { background: #eee; padding: 5px; font-weight: bold; text-align: center; border-bottom: 1px solid black; }
.tabla-receta { width: 100%; border-collapse: collapse; }
.tabla-receta th { border-right: 1px solid black; border-bottom: 1px solid black; text-align: left; padding: 4px; }
.tabla-receta td { border-right: 1px solid black; padding: 4px; }

.fila-lotes { display: flex; gap: 20px; margin-top: 30px; }
.mitad { flex: 1; }
.recuadro-gigante { border: 3px solid black; height: 50px; font-size: 28px; display: flex; align-items: center; justify-content: center; margin-top: 5px; font-weight: 900; }
.texto-lote { font-size: 20px; }

.pie-firma { margin-top: 80px; display: flex; justify-content: space-around; }
.linea-firma { border-top: 2px solid black; width: 40%; text-align: center; font-size: 12px; padding-top: 5px; font-weight: bold; }

/* Panel Control */
.panel-control { width: 300px; min-width: 300px; background: #2c3e50; color: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 10px rgba(0,0,0,0.3); position: sticky; top: 20px; height: fit-content; }
.panel-control h3 { margin-top: 0; border-bottom: 1px solid #555; padding-bottom: 10px; }
.panel-control label { display: block; margin-top: 10px; font-size: 12px; color: #ccc; }
.panel-control select, .panel-control input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 14px; box-sizing: border-box; }
.fila-input { display: flex; gap: 10px; }
.fila-checkbox { display: flex; gap: 15px; margin-top: 15px; background: #34495e; padding: 8px; border-radius: 5px; }
.check-container { display: flex; align-items: center; cursor: pointer; color: white; font-weight: bold; font-size: 13px; margin-top: 0 !important; }
.check-container input { width: auto; margin-right: 5px; }

.btn-guardar {
    /* Usamos el color de marca para la acción principal */
    background: #E67E22; 
    color: white;
    margin-top: 20px;
    border: none;
    padding: 15px 30px;
    border-radius: 8px;
    cursor: pointer;
    font-size: 1.1em;
    font-weight: bold;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    transition: background 0.3s;
}

.btn-guardar:hover {
    /* Un tono más oscuro al pasar el mouse */
    background: #d46914; 
}
.btn-guardar:disabled { background: #555; cursor: not-allowed; }
.btn-imprimir { width: 100%; padding: 12px; background: #e67e22; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; margin-top: 10px; font-size: 14px; transition: 0.3s; }
.btn-imprimir:hover { background: #d35400; }

.alerta-stock { padding: 10px; margin-top: 15px; border-radius: 5px; text-align: center; font-weight: bold; font-size: 12px; color: black; }
.alerta-stock.ok { background: #a5d6a7; color: #1b5e20; }
.alerta-stock.no { background: #ef9a9a; color: #b71c1c; }
.success { color: #a5d6a7; text-align: center; font-weight: bold; margin-top: 10px; }
.error { color: #ef9a9a; text-align: center; font-weight: bold; margin-top: 10px; }

@media (max-width: 1000px) { .contenedor-doble { flex-direction: column; align-items: center; } .panel-control { width: 100%; position: static; } }
</style>