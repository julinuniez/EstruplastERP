<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'

// 👇 IMPORTAMOS EL NUEVO COMPONENTE
import HojaImpresion from './HojaImpresion.vue'

// --- 1. CONSTANTES ---
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 
const PESO_LATA_KG = 0.35; 
const KILOS_BASE_LATA = 500;

// --- 2. INTERFACES ---
interface Producto {
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean; esGenerico: boolean; esFazon?: boolean;
    largo: number; ancho: number; espesor: number; pesoEspecifico: number; color?: string;
}
interface Empleado { id: number; nombreCompleto: string; }
interface Cliente { id: number; razonSocial: string; }
interface ItemReceta { 
    id: number | string; cantidad: number; nombreInsumo: string; densidad: number; materiaPrimaId: number; 
    esColor?: boolean; esCarga?: boolean; esBase?: boolean; esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean;
}

// --- 3. ESTADO ---
const productos = ref<Producto[]>([])
const listaMasterbatches = ref<any[]>([]) 
const listaTodasMateriasPrimas = ref<any[]>([]) 
const empleados = ref<Empleado[]>([])
const clientes = ref<Cliente[]>([])
const recetaDinamica = ref<ItemReceta[]>([]) 

const emit = defineEmits(['guardado'])

const form = ref({
  productoTerminadoId: '' as string | number,
  clienteId: '' as string | number, 
  cantidad: 1, 
  empleadoId: '' as string | number,
  observacion: '',
  turno: 'Mañana',
  largo: 0, ancho: 0, espesor: 0,
  conBrillo: false, porcBrillo: 2.00, 
  llevaFilm: false, tipoCorona: 'Ninguno', 
  conEstearato: false, 
  esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '', 
  aditivoUV: false, porcentajeUv: 1.00, 
  aditivoCaucho: false, porcentajeCaucho: 1.00,
  aditivoCarga: 0, kilosTotales: 0 
})

const mensaje = ref('')
const error = ref('')
const idProduccionGenerada = ref(false)
const ocultarFormula = ref(false) // Control para el PDF

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- 4. COMPUTADOS ---
const productoSeleccionado = computed(() => productos.value.find(p => p.id === form.value.productoTerminadoId) || null);
const empleadoSeleccionado = computed(() => empleados.value.find(e => e.id === form.value.empleadoId) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === form.value.clienteId) || null);

const medidasBloqueadas = computed(() => {
    if (!productoSeleccionado.value) return true; 
    return !productoSeleccionado.value.esGenerico;
});

const totalPorcentajeReceta = computed(() => {
    const suma = recetaDinamica.value.reduce((acc, item) => acc + (parseFloat(item.cantidad.toString()) || 0), 0);
    return parseFloat(suma.toFixed(2));
});

const colorFinalParaPDF = computed(() => {
    if (!form.value.esProductoColor) return form.value.colorTexto || '-';
    if (!form.value.masterbatchId) return 'A DEFINIR';
    const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
    if (mb) {
        const palabras = mb.nombre.split(' ');
        return palabras.length > 1 ? palabras.slice(1).join(' ') : mb.nombre;
    }
    return '-';
});

const densidadMezcla = computed(() => {
    if (recetaDinamica.value.length === 0) return productoSeleccionado.value?.pesoEspecifico || 0.92;
    let dTotal = 0, pTotal = 0;
    recetaDinamica.value.forEach(item => {
        const porc = parseFloat(item.cantidad.toString()) || 0;
        const dens = item.densidad || 0.92; 
        dTotal += (porc * dens);
        pTotal += porc;
    });
    return pTotal === 0 ? 0.92 : parseFloat((dTotal / pTotal).toFixed(4));
});

const kilosCalculados = computed(() => {
    if (!productoSeleccionado.value) return 0;
    const L_m = (parseFloat(form.value.largo.toString()) || 0) / 1000;
    const A_m = (parseFloat(form.value.ancho.toString()) || 0) / 1000;
    const E_mm = parseFloat(form.value.espesor.toString()) || 0;
    const Cant = parseFloat(form.value.cantidad.toString()) || 1;
    const D = densidadMezcla.value; 
    const peso = L_m * A_m * E_mm * D;
    return parseFloat((peso * Cant).toFixed(2));
});

const insumosSinStock = computed(() => {
    if (form.value.kilosTotales <= 0) return [];
    const faltantes: any[] = [];
    recetaDinamica.value.forEach(item => {
        const porcentaje = parseFloat(item.cantidad.toString()) || 0;
        const consumoNecesario = (form.value.kilosTotales * porcentaje) / 100;
        const mpEnStock = listaTodasMateriasPrimas.value.find(m => m.id === item.materiaPrimaId);
        if (mpEnStock) {
            const stockDisponible = mpEnStock.stockActual || 0;
            if (stockDisponible < consumoNecesario) {
                faltantes.push({ nombre: item.nombreInsumo, necesario: consumoNecesario, disponible: stockDisponible, diferencia: consumoNecesario - stockDisponible });
            }
        }
    });
    return faltantes;
});

const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);

const materiasPrimasParaManual = computed(() => {
    return listaTodasMateriasPrimas.value.filter(mp => {
        const nombre = mp.nombre.toUpperCase();
        return !(nombre.includes('BRILLO') || nombre.includes('ESTEARATO') || nombre.includes('CAUCHO') || nombre.includes('CARGA MINERAL') || nombre.includes('UV'));
    });
});

// --- 5. FUNCIONES ---

// Evento recibido desde el Hijo para agregar insumo manual
const agregarInsumoManual = (datos: { id: number, porcentaje: number }) => {
    const mat = listaTodasMateriasPrimas.value.find(m => m.id === datos.id);
    if (!mat) return;
    recetaDinamica.value.push({
        id: Date.now(),
        materiaPrimaId: mat.id,
        nombreInsumo: mat.nombre,
        cantidad: datos.porcentaje,
        densidad: mat.pesoEspecifico || 0.92,
        esBase: false 
    });
    recalcularFormulaAutomatica();
};

const quitarInsumoManual = (index: number) => {
    recetaDinamica.value.splice(index, 1);
    recalcularFormulaAutomatica();
};

async function CargarProductosFiltrados(clienteId: number | string = '') {
    try {
        let url = `${apiUrl}/Productos`;
        if (clienteId) url += `?clienteId=${clienteId}`;
        const res = await axios.get(url, getAuthConfig());
        productos.value = res.data.filter((p: any) => p.esProductoTerminado);
        
        if (form.value.productoTerminadoId) {
             const aunExiste = productos.value.find(p => p.id === form.value.productoTerminadoId);
             if (!aunExiste) { form.value.productoTerminadoId = ''; recetaDinamica.value = []; }
        }
    } catch (e) { console.error(e); }
}

async function CargarDatosProductos(id: number) {
    if (!id) return;
    try {
        const res = await axios.get(`${apiUrl}/Productos/${id}`, getAuthConfig());
        const productoDto = res.data;

        if (!productoDto.esGenerico) {
            form.value.largo = productoDto.largo; form.value.ancho = productoDto.ancho; form.value.espesor = productoDto.espesor;
            if(!form.value.observacion) form.value.observacion = "Producción Estándar de Stock";
        } else {
            form.value.largo = productoDto.largo || 0; form.value.ancho = productoDto.ancho || 0; form.value.espesor = productoDto.espesor || 0;
        }

        form.value.esProductoColor = productoDto.nombre.toUpperCase().includes('COLOR') || productoDto.nombre.toUpperCase().includes('VARIOS');
        form.value.colorTexto = productoDto.color || '';
        form.value.masterbatchId = ''; form.value.aditivoCarga = 0; form.value.aditivoUV = false; form.value.aditivoCaucho = false;

        if (productoDto.receta && productoDto.receta.length > 0) {
            recetaDinamica.value = productoDto.receta.map((r: any) => ({
                id: Date.now() + Math.random(),
                materiaPrimaId: r.materiaPrimaId,
                nombreInsumo: r.nombreInsumo || 'Insumo',
                cantidad: r.cantidad,
                densidad: 0.92, 
                esColor: false, esCarga: false, esBrillo: false, esEstearato: false,
                esBase: r.cantidad > 50 
            }));
            recalcularFormulaAutomatica();
        } else { recetaDinamica.value = []; }
    } catch (e) { console.error(e); recetaDinamica.value = []; }
}

function recalcularFormulaAutomatica() {
    let nuevaReceta = [...recetaDinamica.value];
    if(nuevaReceta.length === 0) return;

    nuevaReceta = nuevaReceta.filter(r => !r.esColor && !r.esCarga && !r.esBrillo && !r.esEstearato && !r.esUv && !r.esCaucho);
    
    if (form.value.conBrillo && form.value.porcBrillo > 0) {
        const mat = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('BRILLO'));
        if (mat) nuevaReceta.push({ id: 'brillo', cantidad: form.value.porcBrillo, nombreInsumo: mat.nombre, densidad: mat.pesoEspecifico || 0.92, materiaPrimaId: mat.id, esBrillo: true });
    }
    if (form.value.conEstearato) { 
        const mat = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('ESTEARATO'));
        if (mat) nuevaReceta.push({ id: 'estearato', cantidad: parseFloat(((PESO_LATA_KG / KILOS_BASE_LATA) * 100).toFixed(4)), nombreInsumo: mat.nombre, densidad: mat.pesoEspecifico || 0.92, materiaPrimaId: mat.id, esEstearato: true });
    }
    if (form.value.aditivoUV && form.value.porcentajeUv > 0) {
        const mat = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('UV'));
        if (mat) nuevaReceta.push({ id: 'uv', cantidad: form.value.porcentajeUv, nombreInsumo: mat.nombre, densidad: mat.pesoEspecifico || 0.92, materiaPrimaId: mat.id, esUv: true });
    }
    if (form.value.aditivoCaucho && form.value.porcentajeCaucho > 0) {
        const mat = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('CAUCHO'));
        if (mat) nuevaReceta.push({ id: 'caucho', cantidad: form.value.porcentajeCaucho, nombreInsumo: mat.nombre, densidad: mat.pesoEspecifico || 0.92, materiaPrimaId: mat.id, esCaucho: true });
    }
    if (form.value.esProductoColor && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        if (mb) nuevaReceta.push({ id: 'color', cantidad: 2, nombreInsumo: mb.nombre, densidad: mb.pesoEspecifico, esColor: true, materiaPrimaId: mb.id });
    }
    if (form.value.aditivoCarga > 0) {
        const mat = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('CARGA') || m.nombre.toUpperCase().includes('CARBONATO'));
        nuevaReceta.push({ id: 'carga', cantidad: form.value.aditivoCarga, nombreInsumo: mat ? mat.nombre : 'CARGA MINERAL', densidad: 1.8, materiaPrimaId: mat ? mat.id : 0, esCarga: true });
    }

    let itemBase = nuevaReceta.find(r => r.esBase);
    if (!itemBase && nuevaReceta.length > 0) {
        const candidatos = nuevaReceta.filter(r => !r.esColor && !r.esCarga && !r.esBrillo && !r.esEstearato && !r.esUv && !r.esCaucho);
        if (candidatos.length > 0) { itemBase = candidatos.reduce((prev, current) => (prev.cantidad > current.cantidad) ? prev : current); itemBase.esBase = true; }
    }
    if (itemBase) {
        const sumaOtros = nuevaReceta.reduce((sum, item) => item.esBase ? sum : sum + parseFloat(item.cantidad.toString()), 0);
        itemBase.cantidad = parseFloat((100 - sumaOtros).toFixed(2));
        if (itemBase.cantidad < 0) itemBase.cantidad = 0;
    }
    recetaDinamica.value = nuevaReceta;
}

async function registrarProduccion() {
  mensaje.value = ''; error.value = '';
  if (!form.value.empleadoId || form.value.kilosTotales <= 0) { error.value = "Faltan datos obligatorios."; return; }
  if (!medidasBloqueadas.value && (form.value.largo <= 0 || form.value.ancho <= 0 || form.value.espesor <= 0)) { error.value = "Datos de medidas incompletos"; return; }
  if (Math.abs(totalPorcentajeReceta.value - 100) > 0.5) { error.value = `Error en receta: suma ${totalPorcentajeReceta.value}%`; return; }

  let techDetails = ` | ${form.value.largo}x${form.value.ancho}x${form.value.espesor}mm | Color: ${colorFinalParaPDF.value} | Corona: ${form.value.tipoCorona} | Brillo: ${form.value.conBrillo ? 'SI' : 'NO'}`;
  if(form.value.aditivoUV) techDetails += ` | UV: ${form.value.porcentajeUv}%`;
  if(form.value.aditivoCaucho) techDetails += ` | Caucho: ${form.value.porcentajeCaucho}%`;
  if(form.value.aditivoCarga > 0) techDetails += ` | Carga: ${form.value.aditivoCarga}%`;

  const consumos = recetaDinamica.value.map(item => ({ materiaPrimaId: item.materiaPrimaId, cantidadKilos: Number(((form.value.kilosTotales * parseFloat(item.cantidad.toString())) / 100).toFixed(3)) }));

  const payload = {
    productoTerminadoId: form.value.productoTerminadoId,
    clienteId: form.value.clienteId || null,
    cantidad: form.value.cantidad,
    empleadoId: form.value.empleadoId,
    turno: form.value.turno,
    observacion: (form.value.observacion || '') + techDetails,
    kilos: form.value.kilosTotales,
    consumos: consumos 
  }
  
  try {
    await axios.post(`${apiUrl}/Ordenes`, payload, getAuthConfig())
    mensaje.value = `✅ Orden Generada Correctamente.`
    idProduccionGenerada.value = true 
    emit('guardado') 
  } catch (e: any) { error.value = '❌ ' + (e.response?.data?.mensaje || e.message); }
}

// --- FUNCIÓN DE IMPRESIÓN MEJORADA ---
async function generarPDF(tipo: 'orden' | 'carga') {
  // 1. Controlamos qué se ve
  ocultarFormula.value = (tipo === 'orden');

  // 2. Esperamos que Vue actualice el DOM en el Hijo
  await nextTick();

  // 3. Buscamos el elemento (vive dentro del hijo, pero el ID es global en el DOM)
  const elemento = document.getElementById('hoja-de-impresion');
  const nombreArchivo = tipo === 'orden' ? `Orden_${Date.now()}.pdf` : `HojaCarga_${Date.now()}.pdf`;

  const opt = { margin: 0, filename: nombreArchivo, image: { type: 'jpeg', quality: 0.98 }, html2canvas: { scale: 2 }, jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' } };
  
  await html2pdf().set(opt).from(elemento).save();

  // 4. Restauramos
  ocultarFormula.value = false; 
}

// --- 6. WATCHERS ---
watch(() => form.value.clienteId, (v) => CargarProductosFiltrados(v));
watch(() => form.value.productoTerminadoId, (v) => v ? CargarDatosProductos(Number(v)) : (recetaDinamica.value = []));
watch(() => [form.value.masterbatchId, form.value.aditivoCarga, form.value.porcBrillo, form.value.conEstearato, form.value.aditivoUV, form.value.porcentajeUv, form.value.aditivoCaucho, form.value.porcentajeCaucho], recalcularFormulaAutomatica);
watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { if (!v) form.value.llevaFilm = false; recalcularFormulaAutomatica(); });
watch(kilosCalculados, (v) => form.value.kilosTotales = v);

onMounted(async () => {
    try {
        const config = getAuthConfig();
        const [resEmp, resCli, resMat] = await Promise.all([ axios.get(`${apiUrl}/Empleados`, config), axios.get(`${apiUrl}/Clientes`, config), axios.get(`${apiUrl}/Productos/materias-primas`, config) ]);
        await CargarProductosFiltrados(); 
        listaTodasMateriasPrimas.value = resMat.data;
        listaMasterbatches.value = resMat.data.filter((m: any) => m.nombre && (m.nombre.toUpperCase().includes('MASTERBATCH') || m.nombre.toUpperCase().includes('PIGMENTO')));
        empleados.value = resEmp.data;
        clientes.value = resCli.data;
    } catch (e) { error.value = 'Error de conexión.'; }
});
</script>

<template>
  <div class="contenedor-doble">
    
    <HojaImpresion 
        :form="form"
        :producto="productoSeleccionado"
        :cliente="clienteSeleccionado"
        :empleado="empleadoSeleccionado"
        :receta="recetaDinamica"
        :colorFinal="colorFinalParaPDF"
        :densidad="densidadMezcla"
        :totalPorcentaje="totalPorcentajeReceta"
        :materiasPrimas="materiasPrimasParaManual"
        :ocultarFormula="ocultarFormula"
        @add-insumo="agregarInsumoManual"
        @remove-insumo="quitarInsumoManual"
    />

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
            <option disabled value="">Seleccionar Producto...</option>
            <option v-for="p in productos" :key="p.id" :value="p.id">
                {{ p.esFazon ? '★ ' : '' }}{{ p.nombre }} 
                {{ p.esGenerico ? '(A Medida)' : (p.esFazon ? '(Fazon)' : '(Estándar)') }}
            </option>
        </select>

        <div v-if="form.productoTerminadoId" class="seccion-medidas-editables">
            <div v-if="form.esProductoColor" style="margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px;">
                <label style="color: #f39c12; font-weight: bold;">🎨 Seleccione Color:</label>
                <select v-model="form.masterbatchId">
                    <option disabled value="">-- Elegir Masterbatch --</option>
                    <option v-for="mb in listaMasterbatches" :key="mb.id" :value="mb.id">{{ mb.nombre }}</option>
                </select>
            </div>

            <label class="lbl-sep">
                Medidas (mm): <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span><span v-else style="color:#2ecc71">(EDITABLES)</span>
            </label>
            <div class="fila-input">
                <div><label>Largo</label><input type="number" v-model="form.largo" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}"></div>
                <div><label>Ancho</label><input type="number" v-model="form.ancho" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}"></div>
            </div>
            <div class="fila-input">
                <div><label>Esp (mm)</label><input type="number" v-model="form.espesor" step="0.1" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}"></div>
                <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
            </div>
            
            <label class="lbl-sep">Aditivos:</label>
            
            <div class="fila-control-aditivo">
                <label class="check-container" :class="{ 'disabled': form.espesor < 1 }"><input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> ✨ Brillo</label>
                <div v-if="form.conBrillo" class="bloque-derecha">
                    <div class="input-porcentaje"><input type="number" v-model="form.porcBrillo" step="0.01" min="0"> %</div>
                </div>
            </div>
            
            <label class="check-container" :class="{ 'disabled': !form.conBrillo }"><input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 🛡️ Con Film</label>
            
            <div class="fila-control-aditivo">
                <label class="check-container"><input type="checkbox" v-model="form.conEstearato"> 🧪 Estearato</label>
            </div>

            <div class="fila-control-aditivo">
                <label class="check-container"><input type="checkbox" v-model="form.aditivoUV"> ☀️ UV</label>
                <div v-if="form.aditivoUV" class="bloque-derecha">
                    <div class="input-porcentaje"><input type="number" v-model="form.porcentajeUv" step="0.01" min="0"> %</div>
                </div>
            </div>

            <div class="fila-control-aditivo">
                <label class="check-container"><input type="checkbox" v-model="form.aditivoCaucho"> 🚜 Caucho</label>
                <div v-if="form.aditivoCaucho" class="bloque-derecha">
                    <div class="input-porcentaje"><input type="number" v-model="form.porcentajeCaucho" step="0.01" min="0"> %</div>
                </div>
            </div>

            <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
            <select v-model="form.tipoCorona"><option value="Ninguno">Sin Tratamiento</option><option value="Simple">Simple</option><option value="Doble">Doble</option></select>

            <label class="lbl-sep">Cargas:</label>
            <div class="fila-input"><div style="flex:1"><label>Carga Mineral (%)</label><input type="number" v-model="form.aditivoCarga"></div></div>

            <div class="resumen-peso">Peso Calc: {{ form.kilosTotales }} Kg</div>
        </div>
        
        <div class="fila-input" style="margin-top:10px"><div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div></div>

        <div v-if="Math.abs(totalPorcentajeReceta - 100) > 0.5" class="alerta-error">⚠️ Receta suma {{ totalPorcentajeReceta }}%.</div>

        <div v-if="hayBloqueoDeStock" class="alerta-stock">
            <h4>🚫 Stock Insuficiente</h4>
            <ul><li v-for="(falla, i) in insumosSinStock" :key="i"><strong>{{ falla.nombre }}</strong>: Falta {{ falla.diferencia.toFixed(2) }} kg</li></ul>
        </div>

        <button class="btn-guardar" @click="registrarProduccion" :disabled="!form.empleadoId || form.kilosTotales <= 0 || hayBloqueoDeStock">
            <span v-if="hayBloqueoDeStock">⚠️ REVISAR STOCK</span>
            <span v-else>💾 GUARDAR</span>
        </button>

        <div v-if="idProduccionGenerada" class="grupo-botones-pdf">
            <button class="btn-imprimir btn-orden" @click="generarPDF('orden')">📄 Orden Prod.</button>
            <button class="btn-imprimir btn-carga" @click="generarPDF('carga')">🧪 Hoja Carga</button>
        </div>
        
        <p class="success">{{ mensaje }}</p>
        <p class="error">{{ error }}</p>
    </div>
  </div>
</template>

<style scoped>
/* ESTILOS DEL PANEL DE CONTROL (El resto se fue al hijo) */
.contenedor-doble { display: flex; flex-direction: row; gap: 20px; justify-content: center; align-items: flex-start; padding: 20px; background-color: #eef2f5; min-height: 100vh; }
.panel-control { width: 320px; min-width: 320px; background: #2c3e50; color: white; padding: 20px; border-radius: 8px; position: sticky; top: 10px; height: fit-content; box-shadow: 0 4px 10px rgba(0,0,0,0.2); }
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
.alerta-error { background: #e74c3c; color: white; padding: 10px; border-radius: 5px; margin-top: 15px; font-weight: bold; text-align: center; }
.btn-guardar { background: #27ae60; color: white; margin-top: 20px; border: none; padding: 12px; border-radius: 6px; cursor: pointer; font-size: 1.1em; font-weight: bold; width: 100%; transition: background 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #7f8c8d; cursor: not-allowed; }
.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; }
.fila-control-aditivo { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; }
.input-porcentaje { display: flex; align-items: center; background: #ecf0f1; border-radius: 4px; padding-right: 5px; color: #333; }
.input-porcentaje input { width: 50px !important; margin: 0 !important; text-align: right; background: transparent !important; color: #333 !important; font-weight: bold; }
.alerta-stock { background-color: #ffebee; border: 1px solid #ef5350; color: #c62828; padding: 10px; border-radius: 6px; margin-top: 15px; font-size: 13px; text-align: left; }
.alerta-stock h4 { margin: 0 0 5px 0; font-size: 14px; font-weight: bold; display: flex; align-items: center; gap: 5px; }
.alerta-stock ul { margin: 0; padding-left: 20px; }
.alerta-stock li { margin-bottom: 3px; }
.bloque-derecha { display: flex; flex-direction: column; align-items: flex-end; }
.input-lock { background-color: #4a5d6e !important; color: #95a5a6 !important; cursor: not-allowed; border: 1px solid #3e4f5e !important; }

/* Grupo de botones PDF */
.grupo-botones-pdf { display: flex; gap: 10px; margin-top: 10px; }
.btn-imprimir { flex: 1; padding: 10px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; font-size: 13px; color: white; }
.btn-orden { background: #34495e; } .btn-orden:hover { background: #2c3e50; }
.btn-carga { background: #8e44ad; } .btn-carga:hover { background: #9b59b6; }

@media (max-width: 1000px) { .contenedor-doble { flex-direction: column; align-items: center; } .panel-control { width: 100%; position: static; } }
</style>