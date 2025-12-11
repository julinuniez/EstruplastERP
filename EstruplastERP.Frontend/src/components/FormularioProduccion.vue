<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'

// --- INTERFACES ---
interface Producto {
    id: number;
    nombre: string;
    codigoSku: string;
    esProductoTerminado: boolean;
    largo: number;
    ancho: number;
    espesor: number;
    pesoEspecifico: number; 
    color?: string;
}

interface Empleado { id: number; nombreCompleto: string; }
interface Cliente { id: number; razonSocial: string; }
interface ItemReceta { id: number; cantidad: number; materiaPrima?: { nombre: string }; nombreInsumo?: string; densidad?: number; }

// --- ESTADO ---
const productos = ref<Producto[]>([])
const empleados = ref<Empleado[]>([])
const clientes = ref<Cliente[]>([])
const recetaActual = ref<ItemReceta[]>([])

const emit = defineEmits(['guardado'])

const form = ref({
  productoTerminadoId: '' as string | number,
  clienteId: '' as string | number, 
  cantidad: 1, 
  empleadoId: '' as string | number,
  observacion: '',
  turno: 'Mañana',
  
  // Medidas
  largo: 0, ancho: 0, espesor: 0,
  
  // Características
  conBrillo: false,
  llevaFilm: false, 
  tipoCorona: 'Ninguno', 
  
  // Aditivos (%)
  aditivoUV: 0, aditivoCaucho: 0, aditivoCarga: 0,

  kilosTotales: 0 
})

const mensaje = ref('')
const error = ref('')
const estadoStock = ref({ posible: false, mensaje: '' })
const idProduccionGenerada = ref(false)

const apiUrl = 'https://localhost:7244/api' // Cambiar a .env en prod

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- COMPUTADOS ---
const productoSeleccionado = computed(() => productos.value.find(p => p.id === form.value.productoTerminadoId) || null);
const empleadoSeleccionado = computed(() => empleados.value.find(e => e.id === form.value.empleadoId) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === form.value.clienteId) || null);

// DENSIDAD MEZCLA
const densidadMezcla = computed(() => {
    if (!recetaActual.value || recetaActual.value.length === 0) {
        return productoSeleccionado.value?.pesoEspecifico || 0.92;
    }
    let densidadTotal = 0, porcentajeTotal = 0;
    recetaActual.value.forEach(item => {
        const porc = item.cantidad || 0;
        const dens = item.densidad || 0.92; 
        densidadTotal += (porc * dens);
        porcentajeTotal += porc;
    });
    if (porcentajeTotal === 0) return 0.92;
    return parseFloat((densidadTotal / porcentajeTotal).toFixed(4));
});

// CALCULADORA KILOS
const kilosCalculados = computed(() => {
    if (!productoSeleccionado.value) return 0;
    const L_m = (parseFloat(form.value.largo.toString()) || 0) / 1000;
    const A_m = (parseFloat(form.value.ancho.toString()) || 0) / 1000;
    const E_mm = parseFloat(form.value.espesor.toString()) || 0;
    const Cant = parseFloat(form.value.cantidad.toString()) || 1;
    const D = densidadMezcla.value;

    const pesoUnitario = L_m * A_m * E_mm * D;
    return parseFloat((pesoUnitario * Cant).toFixed(2));
});

// --- WATCHERS (REGLAS DE NEGOCIO) ---

// 1. Copiar datos al elegir producto
watch(() => form.value.productoTerminadoId, (newId) => {
    const prod = productos.value.find(p => p.id === newId);
    if (prod) {
        form.value.largo = prod.largo;
        form.value.ancho = prod.ancho;
        form.value.espesor = prod.espesor; 
        cargarReceta(newId);
    }
});

// 2. REGLA: Brillo depende de Espesor >= 1
watch(() => form.value.espesor, (nuevoEspesor) => {
    if (nuevoEspesor < 1) {
        form.value.conBrillo = false;
    }
});

// 3. REGLA: Film depende de Brillo
// Si se desactiva el brillo (manual o por espesor), se desactiva el film
watch(() => form.value.conBrillo, (tieneBrillo) => {
    if (!tieneBrillo) {
        form.value.llevaFilm = false;
    }
});

// 4. Kilos
watch(kilosCalculados, (nuevoPeso) => form.value.kilosTotales = nuevoPeso);


// --- CARGA INICIAL ---
onMounted(async () => {
    try {
        const config = getAuthConfig();
        if (!config.headers.Authorization.includes('Bearer null')) {
            const [resProd, resEmp, resCli] = await Promise.all([
                axios.get(`${apiUrl}/Productos`, config),
                axios.get(`${apiUrl}/Empleados`, config),
                axios.get(`${apiUrl}/Clientes`, config)
            ]);
            productos.value = resProd.data.filter((p: any) => p.esProductoTerminado);
            empleados.value = resEmp.data;
            clientes.value = resCli.data;
        }
    } catch (e) { error.value = 'Error de conexión.'; }
});

async function cargarReceta(id: any) {
    try {
        const res = await axios.get(`${apiUrl}/Formulas/${id}`, getAuthConfig()) 
        recetaActual.value = res.data.map((r: any) => ({
            ...r,
            nombreInsumo: r.ingrediente || r.Ingrediente || 'Sin Nombre',
            densidad: r.Densidad || r.densidad || 0.92
        }))
    } catch { recetaActual.value = [] }
}

// --- GUARDAR ---
async function registrarProduccion() {
  mensaje.value = ''; error.value = '';
  
  const cliNombre = clienteSeleccionado.value?.razonSocial || '-';
  const medidasTxt = `${form.value.largo}x${form.value.ancho}x${form.value.espesor}mm`;
  
  let techDetails = ` | Medidas: ${medidasTxt}`;
  techDetails += ` | Corona: ${form.value.tipoCorona}`;
  techDetails += ` | Brillo: ${form.value.conBrillo ? 'SI' : 'NO'}`;
  // Solo mostramos Film si tiene Brillo (lógica visual)
  if(form.value.conBrillo) techDetails += ` | Film: ${form.value.llevaFilm ? 'SI' : 'NO'}`;
  
  if(form.value.aditivoUV > 0) techDetails += ` | UV: ${form.value.aditivoUV}%`;
  if(form.value.aditivoCaucho > 0) techDetails += ` | Caucho: ${form.value.aditivoCaucho}%`;
  if(form.value.aditivoCarga > 0) techDetails += ` | Carga: ${form.value.aditivoCarga}%`;

  const obsFinal = (form.value.observacion || '') + techDetails;

  const payload = {
    productoTerminadoId: form.value.productoTerminadoId,
    clienteId: form.value.clienteId || null,
    cantidad: form.value.cantidad,
    empleadoId: form.value.empleadoId,
    turno: form.value.turno,
    observacion: obsFinal,
    kilos: form.value.kilosTotales 
  }
  
  try {
    await axios.post(`${apiUrl}/Produccion/registrar`, payload, getAuthConfig())
    mensaje.value = `✅ Producción Guardada.`
    idProduccionGenerada.value = true 
    emit('guardado') 
  } catch (e: any) {
    error.value = '❌ ' + (e.response?.data?.mensaje || e.message);
  }
}

// --- PDF ---
function generarOrdenProduccionPDF() {
  const elemento = document.getElementById('hoja-de-impresion')
  const opciones = {
    margin: 0, 
    filename: `Orden_${Date.now()}.pdf`,
    image: { type: 'jpeg', quality: 0.98 },
    html2canvas: { scale: 2 },
    jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
  }
  // @ts-ignore
  html2pdf().set(opciones).from(elemento).save()
}
</script>

<template>
  <div class="contenedor-doble">
    
    <div id="hoja-de-impresion" class="hoja-papel">
      <div class="cabecera">
         <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAlYAAAA4CAYAAAA/xLYcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkY4MzhCNEFBOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkY4MzhCNEFCOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjgzOEI0QTg4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RjgzOEI0QTk4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6QwnXzAAATQUlEQVR42uxd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" alt="ESTRUPLAST ERP" class="logo-img" />
        <div class="datos-orden">
            <h3>ORDEN DE PRODUCCIÓN</h3>
            <p>FECHA: <strong>{{ new Date().toLocaleDateString() }}</strong></p>
            <p>TURNO: <strong>{{ form.turno.toUpperCase() }}</strong></p>
        </div>
      </div>
      
      <div class="fila-pdf">
        <div style="display: flex; justify-content: space-between;">
            <div><strong>RESPONSABLE:</strong> <span class="dato-relleno">{{ empleadoSeleccionado?.nombreCompleto }}</span></div>
            <div><strong>CLIENTE:</strong> <span class="dato-relleno">{{ clienteSeleccionado?.razonSocial }}</span></div>
        </div>
      </div>

      <div class="caja-producto">
        <div class="titulo-seccion">MATERIAL / PRODUCTO A FABRICAR</div>
        <div class="producto-nombre">{{ productoSeleccionado?.nombre || '...' }}</div>
        <div class="producto-sku">CÓDIGO: {{ productoSeleccionado?.codigoSku }}</div>
      </div>

      <div class="ficha-tecnica">
        <div class="dato-box"><span class="label-tech">COLOR</span><span class="valor-tech">{{ productoSeleccionado?.color || '-' }}</span></div>
        <div class="dato-box"><span class="label-tech">LARGO</span><span class="valor-tech">{{ form.largo }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ANCHO</span><span class="valor-tech">{{ form.ancho }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ESPESOR</span><span class="valor-tech">{{ form.espesor }} mm</span></div>
      </div>

      <div class="ficha-tecnica">
        <div class="dato-box">
            <span class="label-tech">BRILLO</span>
            <span class="valor-tech">{{ form.conBrillo ? 'SÍ' : 'NO' }}</span>
        </div>
        
        <div class="dato-box">
            <span class="label-tech">FILM PROTECTOR</span>
            <span class="valor-tech">{{ form.conBrillo && form.llevaFilm ? 'SÍ' : '-' }}</span>
        </div>
        
        <div class="dato-box">
            <span class="label-tech">TRAT. CORONA</span>
            <span class="valor-tech" style="text-transform: uppercase;">{{ form.tipoCorona }}</span>
        </div>
        <div class="dato-box doble-ancho">
            <span class="label-tech">TOTAL KILOS</span>
            <span class="valor-tech" style="font-size: 18px;">{{ form.kilosTotales }} kg</span>
        </div>
      </div>

      <div class="ficha-tecnica" v-if="form.aditivoUV > 0 || form.aditivoCaucho > 0 || form.aditivoCarga > 0">
          <div class="dato-box" v-if="form.aditivoUV > 0">
              <span class="label-tech">ADITIVO UV</span>
              <span class="valor-tech">{{ form.aditivoUV }} %</span>
          </div>
          <div class="dato-box" v-if="form.aditivoCaucho > 0">
              <span class="label-tech">ADITIVO CAUCHO</span>
              <span class="valor-tech">{{ form.aditivoCaucho }} %</span>
          </div>
          <div class="dato-box" v-if="form.aditivoCarga > 0">
              <span class="label-tech">CARGA</span>
              <span class="valor-tech">{{ form.aditivoCarga }} %</span>
          </div>
      </div>

      <div v-if="recetaActual.length > 0" class="seccion-receta">
          <div class="titulo-seccion-receta">FÓRMULA BASE (Densidad Mezcla: {{ densidadMezcla }})</div>
          <table class="tabla-receta">
              <thead><tr><th>INSUMO</th><th>% MEZCLA</th><th>PESO REAL</th></tr></thead>
              <tbody>
                  <tr v-for="(r, i) in recetaActual" :key="i">
                      <td>{{ r.nombreInsumo }}</td>
                      <td style="text-align:center">{{ r.cantidad }} %</td>
                      <td style="text-align:right"><strong>{{ ((form.kilosTotales * r.cantidad) / 100).toFixed(2) }} kg</strong></td>
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
            <strong>OBSERVACIONES:</strong>
            <div class="recuadro-gigante texto-lote">{{ form.observacion }}</div>
        </div>
      </div>
      
      <div class="pie-firma">
        <div class="linea-firma">Firma Responsable</div>
        <div class="linea-firma">Firma Calidad</div>
      </div>
    </div>

    <div class="panel-control">
        <h3>⚙️ Configuración</h3>
        
        <label>Turno / Operario:</label>
        <div class="fila-input">
            <select v-model="form.turno" style="flex:1"><option>Mañana</option><option>Noche</option></select>
            <select v-model="form.empleadoId" style="flex:2">
                <option disabled value="">Seleccionar...</option>
                <option v-for="e in empleados" :key="e.id" :value="e.id">{{e.nombreCompleto}}</option>
            </select>
        </div>

        <label>Cliente / Producto:</label>
        <select v-model="form.clienteId" style="margin-bottom:5px">
            <option disabled value="">Cliente...</option>
            <option v-for="c in clientes" :key="c.id" :value="c.id">{{c.razonSocial}}</option>
        </select>
        <select v-model="form.productoTerminadoId">
            <option disabled value="">Producto...</option>
            <option v-for="p in productos" :key="p.id" :value="p.id">{{p.nombre}}</option>
        </select>

        <div v-if="form.productoTerminadoId" class="seccion-medidas-editables">
            <label class="lbl-sep">Medidas (mm):</label>
            <div class="fila-input">
                <div><label>Largo</label><input type="number" v-model="form.largo"></div>
                <div><label>Ancho</label><input type="number" v-model="form.ancho"></div>
            </div>
            <div class="fila-input">
                <div><label>Espesor</label><input type="number" v-model="form.espesor" step="0.1"></div>
                <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
            </div>
            
            <label class="lbl-sep">Terminación:</label>
            
            <label class="check-container" :class="{ 'disabled': form.espesor < 1 }">
                <input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> 
                ✨ Brillo 
            </label>

            <label class="check-container" :class="{ 'disabled': !form.conBrillo }">
                <input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 
                🛡️ Con Film Protector
            </label>

            <label style="margin-top:5px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
            <select v-model="form.tipoCorona">
                <option value="Ninguno">Sin Tratamiento</option>
                <option value="Simple">Simple</option>
                <option value="Doble">Doble</option>
            </select>

            <label class="lbl-sep">Aditivos (%):</label>
            <div class="fila-input">
                <div><label>UV</label><input type="number" v-model="form.aditivoUV"></div>
                <div><label>Caucho</label><input type="number" v-model="form.aditivoCaucho"></div>
                <div><label>Carga</label><input type="number" v-model="form.aditivoCarga"></div>
            </div>

            <div class="resumen-peso">Peso Calc: {{ form.kilosTotales }} Kg</div>
        </div>
        
        <div class="fila-input" style="margin-top:10px">
             <div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div>
        </div>

        <button class="btn-guardar" @click="registrarProduccion" :disabled="!form.empleadoId || form.kilosTotales <= 0">
            💾 GUARDAR
        </button>

        <button v-if="idProduccionGenerada" class="btn-imprimir" @click="generarOrdenProduccionPDF">
            🖨️ PDF
        </button>
        
        <p class="success">{{ mensaje }}</p>
        <p class="error">{{ error }}</p>
    </div>

  </div>
</template>

<style scoped>
/* Estilos Base */
.contenedor-doble { display: flex; flex-direction: row; gap: 20px; justify-content: center; align-items: flex-start; padding: 20px; background-color: #eef2f5; min-height: 100vh; }
.hoja-papel { background: white; width: 210mm; min-height: 297mm; padding: 10mm; border: 1px solid #ccc; box-shadow: 0 4px 10px rgba(0,0,0,0.1); color: black; font-family: Arial, sans-serif; box-sizing: border-box; }

/* Panel Control */
.panel-control { 
    width: 320px; min-width: 320px; 
    background: #2c3e50; color: white; 
    padding: 20px; border-radius: 8px; 
    position: sticky; top: 10px; height: fit-content; 
    box-shadow: 0 4px 10px rgba(0,0,0,0.2);
}
.panel-control h3 { margin-top: 0; border-bottom: 1px solid #4e6475; padding-bottom: 10px; color: #ecf0f1; }
.panel-control label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; }
.panel-control select, .panel-control input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 14px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
.fila-input { display: flex; gap: 10px; margin-bottom: 5px; }

.seccion-medidas-editables { background: #34495e; padding: 10px; border-radius: 5px; margin-top: 15px; border: 1px solid #4e6475; }
.lbl-sep { color: #f1c40f !important; font-weight: bold; border-bottom: 1px dashed #555; padding-bottom: 3px; margin-top: 15px !important; margin-bottom: 5px; }
.resumen-peso { font-weight: bold; color: #2ecc71; text-align: right; margin-top: 10px; font-size: 16px; border-top: 1px solid #555; padding-top: 5px; }

.check-container { display: flex; align-items: center; cursor: pointer; color: white; font-weight: bold; font-size: 13px; margin-top: 8px !important; }
.check-container input { width: auto; margin-right: 8px; }
.check-container.disabled { opacity: 0.5; cursor: not-allowed; }

.btn-guardar { background: #27ae60; color: white; margin-top: 20px; border: none; padding: 12px; border-radius: 6px; cursor: pointer; font-size: 1.1em; font-weight: bold; width: 100%; transition: background 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #7f8c8d; cursor: not-allowed; }

.btn-imprimir { width: 100%; padding: 12px; background: #2980b9; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; margin-top: 10px; font-size: 14px; }
.btn-imprimir:hover { background: #3498db; }

.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; }

/* Estilos PDF */
.cabecera { border-bottom: 2px solid black; padding-bottom: 15px; display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.logo-img { max-height: 80px; max-width: 250px; object-fit: contain; }
.datos-orden { text-align: right; }
.datos-orden h3 { margin: 0; text-decoration: underline; font-size: 18px; font-weight: 900; }
.datos-orden p { margin: 2px 0; font-size: 12px; }
.fila-pdf { margin-bottom: 15px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; }
.caja-producto { border: 2px solid black; padding: 10px; margin-bottom: 10px; text-align: center; background: #f9f9f9; }
.titulo-seccion { font-size: 10px; font-weight: bold; margin-bottom: 5px; letter-spacing: 1px; }
.producto-nombre { font-size: 18px; font-weight: 900; }
.producto-sku { font-size: 12px; margin-top: 5px; }
.ficha-tecnica { display: flex; border: 2px solid black; margin-bottom: 10px; }
.dato-box { flex: 1; border-right: 1px solid black; text-align: center; padding: 5px; }
.dato-box:last-child { border-right: none; }
.dato-box.doble-ancho { flex: 2; background: #e8e8e8; }
.label-tech { display: block; font-size: 9px; font-weight: bold; color: #333; }
.valor-tech { font-size: 14px; font-weight: bold; margin-top: 2px; display: block; }
.seccion-receta { margin-top: 15px; border: 2px solid black; font-size: 14px; }
.titulo-seccion-receta { background: #e0e0e0; padding: 8px; font-weight: 900; text-align: center; border-bottom: 2px solid black; font-size: 15px; }
.tabla-receta { width: 100%; border-collapse: collapse; }
.tabla-receta th { border-right: 1px solid black; border-bottom: 2px solid black; padding: 8px; background: #f4f4f4; font-size: 12px; }
.tabla-receta td { border-right: 1px solid black; padding: 8px; font-size: 13px; }
.fila-lotes { display: flex; gap: 20px; margin-top: 20px; }
.mitad { flex: 1; }
.recuadro-gigante { border: 3px solid black; height: 40px; font-size: 24px; display: flex; align-items: center; justify-content: center; margin-top: 5px; font-weight: 900; }
.texto-lote { font-size: 16px; }
.pie-firma { margin-top: 60px; display: flex; justify-content: space-around; }
.linea-firma { border-top: 2px solid black; width: 40%; text-align: center; font-size: 11px; padding-top: 5px; font-weight: bold; }

@media (max-width: 1000px) { .contenedor-doble { flex-direction: column; align-items: center; } .panel-control { width: 100%; position: static; } }
</style>