<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'
import HojaImpresion from './HojaImpresion.vue'

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 
const PESO_LATA_KG = 0.35; 
const KILOS_BASE_LATA = 500;
const ID_MASTERBATCH_GENERICO = 22;
const DENSIDAD_DEFAULT = 1.1;

interface Producto { 
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean; 
    esGenerico: boolean; esFazon?: boolean; esMateriaPrima?: boolean; rubro?: string;           
    largo: number; ancho: number; espesor: number; pesoEspecifico: number; color?: string; receta?: any[];
    espesorMinimo?: number; espesorMaximo?: number;
}
interface Empleado { id: number; nombreCompleto: string; }
interface Cliente { id: number; razonSocial: string; }
interface ItemReceta { id: number | string; cantidad: number; nombreInsumo: string; densidad: number; materiaPrimaId: number; esColor?: boolean; esCarga?: boolean; esBase?: boolean; esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean; esFazonInput?: boolean; materialBase?: string; }

const productos = ref<Producto[]>([])
const listaInventarioCompleto = ref<any[]>([]) 
const listaMasterbatches = ref<any[]>([]) 
const listaTodasMateriasPrimas = ref<any[]>([]) 
const empleados = ref<Empleado[]>([])
const clientes = ref<Cliente[]>([])
const recetaDinamica = ref<ItemReceta[]>([]) 
const stockFazonDetectado = ref<number | null>(null);

const limiteMinimo = ref(0);
const limiteMaximo = ref(0);
const emit = defineEmits(['guardado'])

const form = ref({ 
    productoTerminadoId: '' as string | number, clienteId: '' as string | number, 
    cantidad: 1, empleadoId: '' as string | number, observacion: '', turno: 'Mañana', 
    largo: 0, ancho: 0, espesor: 0, 
    conBrillo: false, porcBrillo: 2.00, llevaFilm: false, tipoCorona: 'Ninguno', 
    conEstearato: false, esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '', 
    aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00, 
    aditivoCarga: 0, merma: 8, kilosTotales: 0 
})

const mensaje = ref(''); const error = ref(''); const idProduccionGenerada = ref(false); const ocultarFormula = ref(false)
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

// Computadas
const productoSeleccionado = computed(() => productos.value.find(p => p.id === form.value.productoTerminadoId) || null);
const empleadoSeleccionado = computed(() => empleados.value.find(e => e.id === form.value.empleadoId) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === form.value.clienteId) || null);
const medidasBloqueadas = computed(() => !productoSeleccionado.value || !productoSeleccionado.value.esGenerico);
const totalPorcentajeReceta = computed(() => parseFloat(recetaDinamica.value.reduce((acc, item) => acc + (parseFloat(item.cantidad.toString()) || 0), 0).toFixed(2)));

const clienteTieneFazonHabilitado = computed(() => {
    if (!form.value.clienteId) return false;
    const prefijo = `MP-CLI-${form.value.clienteId}`;
    return listaInventarioCompleto.value.some(p => p.codigoSku && p.codigoSku.startsWith(prefijo));
});

const listaProductosDisponibles = computed(() => {
    return productos.value.filter(p => {
        const nombreUpper = (p.nombre || '').toUpperCase();
        const rubroUpper = (p.rubro || '').toUpperCase();

        if (p.esMateriaPrima) return false; 
        if (rubroUpper.includes('MASTERBATCH') || rubroUpper.includes('MATERIA PRIMA') || rubroUpper.includes('INSUMO')) return false;
        if (nombreUpper.includes('MASTERBATCH') || nombreUpper.includes('PIGMENTO') || nombreUpper.includes('SCRAP')) return false;

        const esTipoFazon = p.esFazon || nombreUpper.includes('FAZON') || nombreUpper.includes('SERVICIO');
        if (esTipoFazon) return clienteTieneFazonHabilitado.value;

        return true;
    });
});

const colorFinalParaPDF = computed(() => {
  if (!form.value.esProductoColor) return form.value.colorTexto || '-';
  if (!form.value.masterbatchId) return 'A DEFINIR';
  const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
  return mb ? (mb.nombre.split(' ').length > 1 ? mb.nombre.split(' ').slice(1).join(' ') : mb.nombre) : '-';
});

const densidadMezcla = computed(() => {
  if (recetaDinamica.value.length === 0) return productoSeleccionado.value?.pesoEspecifico || DENSIDAD_DEFAULT;
  let masaTotal = 0; let porcentajeTotal = 0;
  recetaDinamica.value.forEach(item => { 
      const porc = parseFloat(item.cantidad.toString()) || 0;
      const dens = parseFloat(item.densidad?.toString()) || DENSIDAD_DEFAULT; 
      masaTotal += (porc * dens); porcentajeTotal += porc; 
  });
  return porcentajeTotal === 0 ? DENSIDAD_DEFAULT : parseFloat((masaTotal / porcentajeTotal).toFixed(4));
});

const kilosCalculados = computed(() => {
  if (!productoSeleccionado.value) return 0;
  const L = (parseFloat(form.value.largo.toString()) || 0) / 1000, A = (parseFloat(form.value.ancho.toString()) || 0) / 1000, E = parseFloat(form.value.espesor.toString()) || 0, C = parseFloat(form.value.cantidad.toString()) || 1;
  return parseFloat((L * A * E * densidadMezcla.value * C).toFixed(2));
});

const factorMerma = computed(() => 1 + (form.value.merma / 100));

const insumosSinStock = computed(() => {
  if (form.value.kilosTotales <= 0) return [];
  const faltantes: any[] = [];
  const kilosBase = form.value.kilosTotales * factorMerma.value;

  recetaDinamica.value.forEach(item => {
    const consumo = (kilosBase * parseFloat(item.cantidad.toString()) || 0) / 100;
    if (item.materiaPrimaId === 999 || item.esFazonInput) {
        if (stockFazonDetectado.value !== null && stockFazonDetectado.value < consumo) {
            faltantes.push({ nombre: item.nombreInsumo, necesario: consumo, disponible: stockFazonDetectado.value, diferencia: consumo - (stockFazonDetectado.value || 0) });
        }
    } else {
        const mp = listaTodasMateriasPrimas.value.find(m => m.id === item.materiaPrimaId) || listaInventarioCompleto.value.find(m => m.id === item.materiaPrimaId);
        if (mp && mp.stockActual < consumo) {
            faltantes.push({ nombre: item.nombreInsumo, necesario: consumo, disponible: mp.stockActual, diferencia: consumo - mp.stockActual });
        }
    }
  });
  return faltantes;
});
const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);

// --- FUNCIONES ---

// Se usa para obtener el nombre "base" inicial
function getMaterialNameFromId(id: number): string {
    const prod = productos.value.find(p => p.id === id);
    if(!prod) return "MATERIAL";
    const nombre = prod.nombre.toUpperCase();
    if(nombre.includes("FINO")) return "A.I. FINO";
    if(nombre.includes("GRUESO")) return "A.I. GRUESO";
    if(nombre.includes("TUTTI")) return "A.I. TUTTI";
    if(nombre.includes("ABS")) return "ABS";
    return "MATERIAL";
}

function quitarInsumoManual(index: number) {
    if (index >= 0 && index < recetaDinamica.value.length) {
        recetaDinamica.value.splice(index, 1);
        balancearBase(); 
    }
}

function agregarInsumoDesdeHijo(item: { id: number, porcentaje: number }) {
    const mp = listaTodasMateriasPrimas.value.find(m => m.id === item.id);
    if(mp) {
        recetaDinamica.value.push({
            id: Date.now(), materiaPrimaId: mp.id, nombreInsumo: mp.nombre,
            cantidad: item.porcentaje, densidad: mp.pesoEspecifico || 1, esBase: false
        });
        balancearBase();
    }
}

function balancearBase() {
    if (recetaDinamica.value.length === 0) return;
    let base = recetaDinamica.value.find(r => r.esBase);
    if (!base && recetaDinamica.value.length > 0) {
        base = recetaDinamica.value.reduce((prev, current) => (parseFloat(prev.cantidad.toString()) > parseFloat(current.cantidad.toString())) ? prev : current);
        base.esBase = true; 
    }
    if (base) {
        const sumaOtros = recetaDinamica.value.reduce((acc, item) => {
            if (item === base) return acc;
            return acc + (parseFloat(item.cantidad.toString()) || 0);
        }, 0);
        base.cantidad = parseFloat((Math.max(0, 100 - sumaOtros)).toFixed(2));
    }
}

function esProductoFazon(p: any): boolean {
    if (!p) return false;
    // 1. Si viene la bandera explícita
    if (p.esFazon === true) return true;
    
    // 2. Si el nombre contiene la palabra clave
    const nombre = (p.nombre || '').toUpperCase();
    if (nombre.includes('FAZON') || nombre.includes('SERVICIO')) return true;

    // 3. Si el SKU empieza con FAZ
    const sku = (p.codigoSku || '').toUpperCase();
    if (sku.startsWith('FAZ')) return true;

    return false;
}

async function CargarProductosFiltrados(clienteId: number | string = '') {
  try {
    const cid = clienteId ? clienteId : ''; 
    const res = await axios.get(`${apiUrl}/Productos?clienteId=${cid}`, getAuthConfig());
    productos.value = res.data;
    if (form.value.productoTerminadoId) {
       const prodActual = productos.value.find(p => p.id === form.value.productoTerminadoId);
       if (!prodActual || (prodActual.esFazon && !clienteTieneFazonHabilitado.value)) {
           form.value.productoTerminadoId = ''; 
           recetaDinamica.value = [];
       }
    }
  } catch (e) { console.error(e); }
}

async function CargarDatosProductos(id: number) {
  if (!id) return;
  try {
    const res = await axios.get(`${apiUrl}/Productos/${id}`, getAuthConfig());
    const prod = res.data;
    
    // Configuración de medidas
    if (!prod.esGenerico) { form.value.largo = prod.largo; form.value.ancho = prod.ancho; form.value.espesor = prod.espesor; if(!form.value.observacion) form.value.observacion = "Producción Stock"; } 
    else { form.value.largo = prod.largo || 0; form.value.ancho = prod.ancho || 0; form.value.espesor = prod.espesor || 0; }
    
    limiteMinimo.value = prod.espesorMinimo ?? 0;
    limiteMaximo.value = prod.espesorMaximo ?? 0;
    
    if (limiteMinimo.value === 0 && limiteMaximo.value === 0) {
        const nombre = (prod.nombre || '').toUpperCase();
        if (nombre.includes("FINO")) { limiteMinimo.value = 0.40; limiteMaximo.value = 0.90; } 
        else if (nombre.includes("GRUESO")) { limiteMinimo.value = 0.90; if (nombre.includes("ABS")) limiteMinimo.value = 1.00; }
    }

    form.value.esProductoColor = prod.nombre.toUpperCase().includes('COLOR'); form.value.colorTexto = prod.color || '';
    form.value.masterbatchId = ''; form.value.aditivoCarga = 0; form.value.aditivoUV = false; form.value.aditivoCaucho = false;

    if (prod.receta?.length > 0) {
        // 1. Mapeo de la receta (Esto estaba bien, lo dejamos igual)
        recetaDinamica.value = prod.receta.map((r: any) => {
            const nombre = (r.nombreInsumo || '').toUpperCase();
            const idMP = Number(r.materiaPrimaId);
            
            // Detección
            const esBaseFazon = (idMP >= 990 && idMP <= 999) || (prod.esFazon && r.cantidad > 80);
            const esMb = idMP === ID_MASTERBATCH_GENERICO || nombre.includes('MASTER') || nombre.includes('COLOR');
            const mpReal = listaTodasMateriasPrimas.value.find(m => m.id === r.materiaPrimaId) || listaInventarioCompleto.value.find(m => m.id === r.materiaPrimaId);
            const densidadReal = mpReal ? mpReal.pesoEspecifico : DENSIDAD_DEFAULT;
            
            return { 
                id: Date.now() + Math.random(), 
                materiaPrimaId: r.materiaPrimaId, 
                nombreInsumo: r.nombreInsumo || 'Insumo', 
                cantidad: r.cantidad, 
                densidad: densidadReal, 
                esBase: r.cantidad > 50, 
                esColor: esMb,
                esFazonInput: esBaseFazon 
            };
        });

        // 2. Bloque INDEPENDIENTE para intentar reemplazar Fazón
        if (form.value.clienteId && esProductoFazon(prod)) {
        nextTick(async () => {
            await actualizarRecetaFazonConCliente(form.value.clienteId, prod);
        });
    }

        // 3. Esto se ejecuta SIEMPRE que hay receta (sea fazón o no)
        recalcularFormulaAutomatica();
        stockFazonDetectado.value = null;

    } else { 
        // 4. Bloque ELSE: Se ejecuta si prod.receta NO tiene largo > 0
        recetaDinamica.value = []; 
        stockFazonDetectado.value = null;
    } // <--- ESTA LLAVE CIERRA EL IF PRINCIPAL (prod.receta)

  } catch (e) { 
      console.error(e); 
      recetaDinamica.value = []; 
  }
}

// Reemplaza tu función actual por esta nueva versión
async function actualizarRecetaFazonConCliente(clienteId: string | number, producto: Producto) {
    if (!clienteId || !producto) return;

    // 1. Identificar la fila en el frontend que espera ser reemplazada (el Genérico)
    const itemFazon = recetaDinamica.value.find(r => r.esFazonInput);
    
    if (!itemFazon) return;

    try {
        // 2. LLAMADA AL BACKEND: Pedimos la receta "traducida"
        // Pasamos 100kg para que nos devuelva porcentajes relativos o cantidades base
        const res = await axios.get(`${apiUrl}/Produccion/receta-proyectada`, {
            ...getAuthConfig(),
            params: {
                productoId: producto.id,
                clienteId: clienteId,
                kilos: 100 
            }
        });

        const recetaTraducida = res.data; // Viene el array de ItemFormulaVisualDto

        // 3. BUSCAR LA SUSTITUCIÓN
        // Buscamos en la respuesta del servidor cuál es el ítem que fue marcado como sustitución
        // O buscamos aquel cuyo ID sea diferente al que tenemos actualmente (si era 990 y viene 5000)
        const itemSustituto = recetaTraducida.find((r: any) => r.esSustitucion === true);

        if (itemSustituto) {
            // 4. BUSCAR DATOS COMPLETOS DEL MATERIAL (Densidad, Stock)
            // El backend nos dio ID y Nombre, pero necesitamos la densidad y el stock para el front
            const materialReal = listaInventarioCompleto.value.find(m => m.id === itemSustituto.materiaPrimaId) 
                              || listaTodasMateriasPrimas.value.find(m => m.id === itemSustituto.materiaPrimaId);

            // 5. ACTUALIZAR LA RECETA VISUAL
            itemFazon.materiaPrimaId = itemSustituto.materiaPrimaId;
            itemFazon.nombreInsumo = itemSustituto.nombre; // Viene con "(Nombre Cliente)" si tu backend lo formatea, o úsalo del materialReal
            
            if (materialReal) {
                itemFazon.densidad = materialReal.pesoEspecifico;
                stockFazonDetectado.value = materialReal.stockActual;
                
                // Formateo visual bonito
                const clienteObj = clientes.value.find(c => c.id == clienteId);
                const nombreCli = clienteObj ? clienteObj.razonSocial : 'Cliente';
                itemFazon.nombreInsumo = `MP: ${materialReal.nombre} (${nombreCli})`; // Feedback visual claro
            } else {
                // Caso raro: El backend dice que usemos ID 5000 pero el front no lo tiene cargado en memoria
                itemFazon.densidad = DENSIDAD_DEFAULT;
                stockFazonDetectado.value = 0;
            }

            console.log("✅ Receta actualizada desde Backend con:", itemFazon.nombreInsumo);
        } else {
            console.log("ℹ️ El backend indica que no hay sustitución configurada para este cliente.");
            // Opcional: Revertir al genérico si hiciera falta, o dejarlo como está
        }

    } catch (e) {
        console.error("Error obteniendo receta proyectada:", e);
        error.value = "Error al consultar materiales del cliente.";
    }
}

function recalcularFormulaAutomatica() {
  let porcentajeColor = 2.00;
  const colorExistente = recetaDinamica.value.find(r => r.esColor);
  if (colorExistente) porcentajeColor = Number(colorExistente.cantidad);

  const borrar = ['esCarga','esBrillo','esEstearato','esUv','esCaucho'];
  if (form.value.esProductoColor && form.value.masterbatchId) borrar.push('esColor'); 

  let nueva = recetaDinamica.value.filter(r => {
      for (const flag of borrar) if (r[flag as keyof ItemReceta]) return false;
      return true;
  });

  const add = (nom:string, cant:number, tipo:string, mpId:number=0, dens:number= DENSIDAD_DEFAULT) => {
      let m = null;
      if (mpId === 0) m = listaTodasMateriasPrimas.value.find(x => x.nombre.toUpperCase().includes(nom));
      else m = listaTodasMateriasPrimas.value.find(x => x.id === mpId); 

      nueva.push({ 
          id: tipo, cantidad: cant, nombreInsumo: m ? m.nombre : (nom==='COLOR'?'MASTERBATCH':nom), 
          densidad: m ? (m.pesoEspecifico||dens) : dens, materiaPrimaId: m ? m.id : mpId, [tipo]: true, esColor: tipo === 'esColor' 
      });
  };

  if(form.value.conBrillo) add('BRILLO', form.value.porcBrillo, 'esBrillo');
  if(form.value.conEstearato) add('ESTEARATO', parseFloat(((PESO_LATA_KG/KILOS_BASE_LATA)*100).toFixed(4)), 'esEstearato');
  if(form.value.aditivoUV) add('UV', form.value.porcentajeUv, 'esUv');
  if(form.value.aditivoCaucho) add('CAUCHO', form.value.porcentajeCaucho, 'esCaucho');
  if(form.value.esProductoColor && form.value.masterbatchId) {
      const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
      if(mb) add('COLOR', porcentajeColor, 'esColor', mb.id, mb.pesoEspecifico);
  }
  if(form.value.aditivoCarga > 0) add('CARGA', form.value.aditivoCarga, 'esCarga');

  recetaDinamica.value = nueva;
  balancearBase(); 
}

function validarMasterbatchProhibido(): boolean {
    const tieneProhibido = recetaDinamica.value.some(r => r.materiaPrimaId === ID_MASTERBATCH_GENERICO);
    if (tieneProhibido) {
        error.value = "⛔ ERROR: Debes reemplazar el 'Masterbatch Varios' por un color específico.";
        return false;
    }
    return true;
}

async function registrarProduccion() {
  mensaje.value = ''; error.value = '';
  if (!form.value.empleadoId || form.value.kilosTotales <= 0) return error.value = "Faltan datos."; 
  if (productoSeleccionado.value?.esFazon && !form.value.clienteId) return error.value = "Seleccione Cliente.";
  if (hayBloqueoDeStock.value) return error.value = "STOCK INSUFICIENTE (Considere el desperdicio).";
  if (!validarMasterbatchProhibido()) return; 

  const min = limiteMinimo.value;
  const max = limiteMaximo.value;
  const val = Number(form.value.espesor);

  if (min > 0 && val < min) return error.value = `⚠️ Espesor inválido. > ${min.toFixed(2)} mm.`;
  if (max > 0 && val > max) return error.value = `⚠️ Espesor inválido. < ${max.toFixed(2)} mm.`;

  const consumos = recetaDinamica.value.map(i => ({ 
      materiaPrimaId: i.materiaPrimaId, 
      cantidadKilos: Number(((form.value.kilosTotales * parseFloat(i.cantidad.toString()) / 100) * factorMerma.value).toFixed(3)) 
  }));
  
  try {
    await axios.post(`${apiUrl}/Ordenes`, {
        productoTerminadoId: form.value.productoTerminadoId, clienteId: form.value.clienteId || null, 
        cantidad: form.value.cantidad, empleadoId: form.value.empleadoId, turno: form.value.turno,
        observacion: (form.value.observacion||'') + ` | ${form.value.largo}x${form.value.ancho}x${form.value.espesor}`,
        kilos: form.value.kilosTotales, consumos: consumos 
    }, getAuthConfig());
    mensaje.value = `✅ Orden Generada.`; idProduccionGenerada.value = true; emit('guardado');
  } catch (e: any) { error.value = '❌ ' + (e.response?.data?.mensaje || e.message); }
}

async function generarPDF(tipo: 'orden'|'carga') {
  ocultarFormula.value = (tipo === 'orden'); 
  await nextTick(); await new Promise(r => setTimeout(r, 100)); 
  await html2pdf().set({ margin: 0, filename: `Doc_${Date.now()}.pdf`, image: { type: 'jpeg', quality: 0.98 }, html2canvas: { scale: 2 }, jsPDF: { unit: 'mm', format: 'a4' } }).from(document.getElementById('hoja-de-impresion')).save();
  ocultarFormula.value = false;
}

watch([() => form.value.clienteId, () => form.value.productoTerminadoId], async ([nuevoCli, nuevoProd]) => {
    // 1. Cargar productos filtrados... (igual que antes)
    if (nuevoCli && !form.value.productoTerminadoId) { 
        await CargarProductosFiltrados(nuevoCli);
    }

    // 2. Si tenemos ambos datos...
    if (nuevoCli && form.value.productoTerminadoId) {
        const prod = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
        
        // CORRECCIÓN AQUÍ: Usamos la función robusta
        if (esProductoFazon(prod)) {
            // Esperamos a que Vue procese la receta base
            setTimeout(async () => {
                await actualizarRecetaFazonConCliente(nuevoCli, prod);
            }, 200); // Dale 200ms de margen por seguridad
        }
    }
});

watch(() => form.value.productoTerminadoId, (v) => v ? CargarDatosProductos(Number(v)) : (recetaDinamica.value=[]));
watch(() => [form.value.masterbatchId, form.value.aditivoCarga, form.value.porcBrillo, form.value.conEstearato, form.value.aditivoUV, form.value.porcentajeUv, form.value.aditivoCaucho, form.value.porcentajeCaucho], recalcularFormulaAutomatica);
watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { if (!v) form.value.llevaFilm = false; recalcularFormulaAutomatica(); });
watch(kilosCalculados, (v) => form.value.kilosTotales = v);

onMounted(async () => {
    try {
        const cfg = getAuthConfig();
        const [re, rc, rm, ri] = await Promise.all([
            axios.get(`${apiUrl}/Empleados`, cfg), axios.get(`${apiUrl}/Clientes`, cfg), 
            axios.get(`${apiUrl}/Productos/materias-primas`, cfg), axios.get(`${apiUrl}/Productos/inventario-completo`, cfg)
        ]);
        empleados.value = re.data; clientes.value = rc.data; listaTodasMateriasPrimas.value = rm.data;
        listaInventarioCompleto.value = ri.data;
        listaMasterbatches.value = rm.data.filter((m:any) => m.nombre && (m.nombre.toUpperCase().includes('MASTER') || m.nombre.toUpperCase().includes('PIGMENTO')));
        await CargarProductosFiltrados();
    } catch (e) { error.value = 'Error conexión'; }
});
</script>

<template>
  <div class="layout-global">
    <div class="panel-izquierdo">
        <div class="hoja-contenedor">
            <HojaImpresion 
                :form="form" :producto="productoSeleccionado" :cliente="clienteSeleccionado" 
                :empleado="empleadoSeleccionado" :receta="recetaDinamica" :colorFinal="colorFinalParaPDF" 
                :densidad="densidadMezcla" :totalPorcentaje="totalPorcentajeReceta" :materiasPrimas="listaTodasMateriasPrimas" 
                :ocultarFormula="ocultarFormula" @add-insumo="agregarInsumoDesdeHijo" 
                @remove-insumo="quitarInsumoManual" @update-receta="balancearBase"  
            />
        </div>
    </div>

    <div class="panel-derecho">
        <div class="header-control"><h3>⚙️ Configuración</h3></div>
        
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
            <option v-for="p in listaProductosDisponibles" :key="p.id" :value="p.id">
                {{ p.esFazon ? '★ ' : '' }}{{ p.nombre }} {{ p.esGenerico ? '(A Medida)' : (p.esFazon ? '(Fazon)' : '(Estándar)') }}
            </option>
        </select>

        <div v-if="form.productoTerminadoId" class="seccion-medidas-editables">
            <div v-if="form.esProductoColor" class="box-color">
                <label style="color: #f39c12;">🎨 Seleccione Color:</label>
                <select v-model="form.masterbatchId">
                    <option disabled value="">-- Elegir Masterbatch --</option>
                    <option v-for="mb in listaMasterbatches" :key="mb.id" :value="mb.id">{{ mb.nombre }}</option>
                </select>
            </div>

            <label class="lbl-sep">
                Medidas: 
                <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span>
                <span v-else style="color:#2ecc71">(EDITABLES)</span>
            </label>
            
            <div style="font-size:11px; color:#bbb; margin-top:-5px; margin-bottom:5px;">
                <span v-if="limiteMaximo > 0">Rango: {{ limiteMinimo }} - {{ limiteMaximo }} mm</span>
                <span v-else-if="limiteMinimo > 0">Mínimo: {{ limiteMinimo }} mm (Sin tope)</span>
            </div>

            <div class="fila-input">
                <div><label>Largo</label><input type="number" v-model="form.largo" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}"></div>
                <div><label>Ancho</label><input type="number" v-model="form.ancho" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}"></div>
            </div>
            <div class="fila-input">
                <div>
                    <label>Espesor</label>
                    <input type="number" v-model="form.espesor" step="0.01" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}">
                </div>
                <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
            </div>
            
            <div class="fila-input" style="margin-top:10px; border-top:1px dashed #7f8c8d; padding-top:10px;">
                <div style="flex:1">
                    <label style="color:#e67e22;">🔥 Desperdicio (%)</label>
                    <input type="number" v-model="form.merma" min="0" max="50" style="color:#e67e22; font-weight:bold;">
                </div>
            </div>

            <div class="resumen-peso">
                Peso Final PT: {{ form.kilosTotales }} Kg
                <small style="color:#bbb; display:block;">(Consumo Real MP +{{ form.merma }}%)</small>
            </div>
            
            <label class="lbl-sep">Aditivos:</label>
            <div class="fila-control-aditivo">
                <label class="check-container" :class="{ 'disabled': form.espesor < 1 }"><input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> ✨ Brillo</label>
                <div v-if="form.conBrillo" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcBrillo" step="0.01" min="0"> %</div></div>
            </div>
            <div class="fila-control-aditivo"><label class="check-container" :class="{ 'disabled': !form.conBrillo }"><input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 🛡️ Con Film</label></div>
            <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.conEstearato"> 🧪 Estearato</label></div>
            <div class="fila-control-aditivo">
                <label class="check-container"><input type="checkbox" v-model="form.aditivoUV"> ☀️ UV</label>
                <div v-if="form.aditivoUV" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeUv" step="0.01" min="0"> %</div></div>
            </div>
            <div class="fila-control-aditivo">
                <label class="check-container"><input type="checkbox" v-model="form.aditivoCaucho"> 🚜 Caucho</label>
                <div v-if="form.aditivoCaucho" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeCaucho" step="0.01" min="0"> %</div></div>
            </div>

            <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
            <select v-model="form.tipoCorona"><option value="Ninguno">Sin Tratamiento</option><option value="Simple">Simple</option><option value="Doble">Doble</option></select>
            
            <label class="lbl-sep">Cargas:</label>
            <div class="fila-input"><div style="flex:1"><label>Carga Mineral (%)</label><input type="number" v-model="form.aditivoCarga"></div></div>
        </div>
        
        <div class="fila-input" style="margin-top:10px"><div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div></div>
        
        <div v-if="Math.abs(totalPorcentajeReceta - 100) > 0.5" class="alerta-error">⚠️ Receta suma {{ totalPorcentajeReceta }}%.</div>
        <div v-if="hayBloqueoDeStock" class="alerta-stock">
            <h4>🚫 Stock Insuficiente</h4>
            <ul><li v-for="(falla, i) in insumosSinStock" :key="i"><strong>{{ falla.nombre }}</strong>: Falta {{ falla.diferencia.toFixed(2) }} kg (Disp: {{ falla.disponible }})</li></ul>
        </div>

        <button class="btn-guardar" @click="registrarProduccion" :disabled="!form.empleadoId || form.kilosTotales <= 0 || hayBloqueoDeStock">
            <span v-if="hayBloqueoDeStock">⚠️ REVISAR STOCK</span><span v-else>💾 GUARDAR ORDEN</span>
        </button>

        <div v-if="idProduccionGenerada" class="grupo-botones-pdf">
            <button class="btn-imprimir btn-orden" @click="generarPDF('orden')">📄 Orden</button>
            <button class="btn-imprimir btn-carga" @click="generarPDF('carga')">🧪 Carga</button>
        </div>
        
        <p class="success">{{ mensaje }}</p>
        <p class="error">{{ error }}</p>
    </div>
  </div>
</template>

<style scoped>
/* Los estilos se mantienen igual */
.layout-global { display: flex; width: 100%; min-height: 100vh; font-family: 'Segoe UI', sans-serif; background-color: #ecf0f1; }
.panel-izquierdo { flex: 1; background-color: #ecf0f1; display: flex; justify-content: center; padding: 30px; border-right: 1px solid #bdc3c7; }
.hoja-contenedor { background: white; box-shadow: 0 4px 15px rgba(0,0,0,0.15); min-height: 1000px; width: 100%; max-width: 800px; margin-bottom: 50px; }
.panel-derecho { width: 320px; min-width: 320px; background-color: #2c3e50; color: white; display: flex; flex-direction: column; padding: 20px; box-shadow: -5px 0 15px rgba(0,0,0,0.2); z-index: 10; border-left: 1px solid #34495e; min-height: 100vh; }
.header-control h3 { margin-top: 0; border-bottom: 2px solid #3498db; padding-bottom: 10px; color: #ecf0f1; font-size: 1.1rem; }
label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; font-weight: 600; }
select, input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 13px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
.fila-input { display: flex; gap: 8px; margin-bottom: 5px; }
.seccion-medidas-editables { background: #34495e; padding: 12px; border-radius: 6px; margin-top: 15px; border: 1px solid #4e6475; }
.box-color { margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px; }
.lbl-sep { color: #f1c40f !important; font-weight: bold; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-top: 15px !important; margin-bottom: 5px; }
.resumen-peso { font-weight: bold; color: #2ecc71; text-align: right; margin-top: 10px; font-size: 14px; border-top: 1px solid #7f8c8d; padding-top: 5px; }
.check-container { display: flex; align-items: center; cursor: pointer; color: #ecf0f1; font-weight: bold; font-size: 13px; margin-top: 8px !important; }
.check-container input { width: auto; margin-right: 8px; }
.check-container.disabled { opacity: 0.5; cursor: not-allowed; }
.alerta-error { background: #c0392b; color: white; padding: 10px; border-radius: 5px; margin-top: 15px; font-weight: bold; text-align: center; font-size: 12px; }
.btn-guardar { background: #27ae60; color: white; margin-top: 20px; border: none; padding: 12px; border-radius: 6px; cursor: pointer; font-size: 1em; font-weight: bold; width: 100%; transition: background 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #7f8c8d; cursor: not-allowed; opacity: 0.7; }
.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.fila-control-aditivo { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; }
.input-porcentaje { display: flex; align-items: center; background: #ecf0f1; border-radius: 4px; padding-right: 5px; color: #333; }
.input-porcentaje input { width: 45px !important; margin: 0 !important; text-align: right; background: transparent; color: #333; }
.alerta-stock { background-color: #ffebee; border: 1px solid #ef5350; color: #c62828; padding: 10px; border-radius: 6px; margin-top: 15px; font-size: 12px; text-align: left; }
.bloque-derecha { display: flex; flex-direction: column; align-items: flex-end; }
.input-lock { background-color: #4a5d6e !important; color: #bdc3c7 !important; cursor: not-allowed; border: 1px solid #3e4f5e !important; }
.grupo-botones-pdf { display: flex; gap: 5px; margin-top: 10px; }
.btn-imprimir { flex: 1; padding: 8px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; font-size: 12px; color: white; }
.btn-orden { background: #34495e; border: 1px solid #7f8c8d; } .btn-orden:hover { background: #2980b9; }
.btn-carga { background: #8e44ad; border: 1px solid #9b59b6; } .btn-carga:hover { background: #9b59b6; }
@media (max-width: 1000px) { .layout-global { flex-direction: column; height: auto; } .panel-izquierdo { width: 100%; border-right: none; border-bottom: 1px solid #bdc3c7; } .panel-derecho { width: 100%; min-width: auto; min-height: auto; } .hoja-contenedor { transform: scale(0.95); margin-bottom: 20px; transform-origin: top center; } }
</style>