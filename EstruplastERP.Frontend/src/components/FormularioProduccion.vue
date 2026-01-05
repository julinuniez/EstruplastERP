<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'
import HojaImpresion from './HojaImpresion.vue'

// CONFIGURACIÓN
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 
const PESO_LATA_KG = 0.35; 
const KILOS_BASE_LATA = 500;
const ID_MASTERBATCH_GENERICO = 22;
const DENSIDAD_DEFAULT = 1.1;

// INTERFACES
interface Producto { 
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean; 
    esGenerico: boolean; esFazon?: boolean; esMateriaPrima?: boolean; rubro?: string;           
    largo: number; ancho: number; espesor: number; pesoEspecifico: number; color?: string; receta?: any[];
    espesorMinimo?: number; espesorMaximo?: number;
    clienteId?: number; 
}
interface Empleado { id: number; nombreCompleto: string; }
interface Cliente { id: number; razonSocial: string; }
interface ItemReceta { id: number | string; cantidad: number; nombreInsumo: string; densidad: number; materiaPrimaId: number; esColor?: boolean; esCarga?: boolean; esBase?: boolean; esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean; esFazonInput?: boolean; materialBase?: string; }

// ESTADO REACTIVO
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

// --- COMPUTED PROPERTIES ---

const productoSeleccionado = computed(() => productos.value.find(p => p.id === Number(form.value.productoTerminadoId)) || null);
const empleadoSeleccionado = computed(() => empleados.value.find(e => e.id === Number(form.value.empleadoId)) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === Number(form.value.clienteId)) || null);
const medidasBloqueadas = computed(() => !productoSeleccionado.value || !productoSeleccionado.value.esGenerico);
const totalPorcentajeReceta = computed(() => parseFloat(recetaDinamica.value.reduce((acc, item) => acc + (parseFloat(item.cantidad.toString()) || 0), 0).toFixed(2)));

const clienteTieneFazonHabilitado = computed(() => {
    if (!form.value.clienteId) return false;
    const prefijo = `MP-CLI-${form.value.clienteId}`;
    return listaInventarioCompleto.value.some(p => p.codigoSku && p.codigoSku.startsWith(prefijo));
});

const listaProductosDisponibles = computed(() => {
    // Si no hay datos cargados, lista vacía
    if (!productos.value || productos.value.length === 0) return [];

    return productos.value.filter(p => {
        const nombre = (p.nombre || '').toUpperCase();
        const rubro = (p.rubro || '').toUpperCase();
        if (p.esMateriaPrima) return false;
        
        // Bloquear Rubros de insumos
        if (rubro.includes('MATERIA') || rubro.includes('INSUMO') || rubro.includes('MASTERBATCH')) return false;
        
        // Bloquear por Nombre (Bases, Genéricos, Scrap, etc.)
        if (nombre.includes('BASE') && !nombre.includes('ALTA')) return false; // Ojo: "Polietileno Alta" puede pasar, "Base" sola no.
        if (nombre.includes('(BASE)')) return false;
        if (nombre.includes('(VARIOS)')) return false;
        if (nombre.includes('GENERICO') || nombre.includes('GENÉRICO')) return false;
        if (nombre.includes('MASTERBATCH') || nombre.includes('PIGMENTO') || nombre.includes('SCRAP')) return false;
        
        // Bloquear por ID reservados
        if (p.id >= 990 && p.id <= 999) return false;
        const esFazon = p.esFazon || nombre.includes('FAZON') || nombre.includes('SERVICIO');

        if (esFazon) {
            // A. SI NO HAY CLIENTE SELECCIONADO:
            if (!form.value.clienteId) return false;

            // B. SI HAY CLIENTE SELECCIONADO:
            if (p.clienteId && p.clienteId != form.value.clienteId) {
                return false; 
            }
        }

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
  
  let sumaProducto = 0;
  let sumaPorcentajes = 0;

  recetaDinamica.value.forEach(item => { 
      const porc = parseFloat(item.cantidad.toString()) || 0;
      const dens = parseFloat(item.densidad?.toString()) || DENSIDAD_DEFAULT; 
      
      // Ponderación: (Densidad * Porcentaje)
      sumaProducto += (porc * dens); 
      sumaPorcentajes += porc; 
  });

  // Si la receta no suma 100% (ej: 98%), normalizamos el promedio
  return sumaPorcentajes === 0 ? DENSIDAD_DEFAULT : (sumaProducto / sumaPorcentajes);
});

const kilosCalculados = computed(() => {
  if (!productoSeleccionado.value) return 0;

  const L_metros = (parseFloat(form.value.largo.toString()) || 0) / 1000;
  const A_metros = (parseFloat(form.value.ancho.toString()) || 0) / 1000;
  const E_milimetros = parseFloat(form.value.espesor.toString()) || 0;
  const Cantidad = parseFloat(form.value.cantidad.toString()) || 1;
  const Densidad = Number(densidadMezcla.value);

  // Física: Volumen * Densidad
  const pesoNeto = (L_metros * A_metros * E_milimetros * Densidad * Cantidad);

  // 🔥 SIN REDONDEO FUERTE: Mantenemos precisión decimal
  return parseFloat(pesoNeto.toFixed(4));
});

// 🔥 2. CÁLCULO DE STOCK CON MERMA Y NUMBER()
const factorMerma = computed(() => 1 + (Number(form.value.merma || 0) / 100));

const insumosSinStock = computed(() => {
  const kilosNetos = Number(form.value.kilosTotales);
  if (kilosNetos <= 0) return [];
  
  const faltantes: any[] = [];
  
  // Factor de multiplicación (Ej: Si merma es 8, factor es 1.08)
  const factorMerma = 1 + (Number(form.value.merma || 0) / 100);

  recetaDinamica.value.forEach(item => {
    // 1. Cuánto pesa ESTE insumo dentro de la mezcla pura (sin desperdicio)
    const porcentajeInsumo = parseFloat(item.cantidad.toString()) || 0;
    const pesoNetoInsumo = (kilosNetos * porcentajeInsumo) / 100;

    // 2. Le agregamos el desperdicio proporcional
    const consumoReal = Number((pesoNetoInsumo * factorMerma).toFixed(3));

    // 3. Verificamos Stock
    const idMaterial = Number(item.materiaPrimaId);
    let stockDisponible = 0;
    
    // Si es Base (990-999), stock es 0
    if (idMaterial >= 990 && idMaterial <= 999) {
        stockDisponible = 0;
    } else {
        const mp = listaInventarioCompleto.value.find(m => m.id === idMaterial) || 
                   listaTodasMateriasPrimas.value.find(m => m.id === idMaterial);
        stockDisponible = mp ? Number(mp.stockActual || 0) : 0;
    }

    // Margen de tolerancia 0.001 para decimales
    if (stockDisponible < (consumoReal - 0.001)) {
        faltantes.push({ 
            nombre: item.nombreInsumo, 
            necesario: consumoReal, 
            disponible: stockDisponible, 
            diferencia: Number((consumoReal - stockDisponible).toFixed(2)) 
        });
    }
  });
  
  return faltantes;
});

const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);


// --- FUNCIONES ---

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

async function CargarProductosFiltrados(clienteId: number | string = '') {
  try {
    const cid = clienteId ? clienteId : ''; 
    const res = await axios.get(`${apiUrl}/Productos?clienteId=${cid}`, getAuthConfig());
    productos.value = res.data;
    
    if (form.value.productoTerminadoId) {
       // Verificar si el producto actual sigue siendo válido con el nuevo filtro
       const prodActual = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
       const estaEnLista = listaProductosDisponibles.value.some(p => p.id === Number(form.value.productoTerminadoId));
       
       if (!prodActual || !estaEnLista) {
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
    
    // Config Medidas
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

    // Cargar Receta
    if (prod.receta?.length > 0) {
        recetaDinamica.value = prod.receta.map((r: any) => {
            const nombre = (r.nombreInsumo || '').toUpperCase();
            const idMP = Number(r.materiaPrimaId);
            
            // Detección de Fazón
            const esBaseFazon = (idMP >= 990 && idMP <= 999) || (prod.esFazon && r.cantidad > 80);
            
            const esMb = idMP === ID_MASTERBATCH_GENERICO || nombre.includes('MASTER') || nombre.includes('COLOR');
            const mpReal = listaTodasMateriasPrimas.value.find(m => m.id === r.materiaPrimaId) || listaInventarioCompleto.value.find(m => m.id === r.materiaPrimaId);
            const densidadReal = mpReal ? mpReal.pesoEspecifico : DENSIDAD_DEFAULT;
            
            return { 
                id: Date.now()+Math.random(), 
                materiaPrimaId: r.materiaPrimaId, 
                nombreInsumo: r.nombreInsumo || 'Insumo', 
                cantidad: r.cantidad, 
                densidad: densidadReal, 
                esBase: r.cantidad > 50, 
                esColor: esMb,
                esFazonInput: esBaseFazon 
            };
        });

        // Intentar reemplazo inmediato si corresponde
        if (form.value.clienteId && prod.esFazon) {
            nextTick(async () => {
                await actualizarRecetaFazonConCliente(form.value.clienteId, prod);
            });
        }
        recalcularFormulaAutomatica();
        stockFazonDetectado.value = null;
    } else { 
        recetaDinamica.value = []; stockFazonDetectado.value = null;
    }
  } catch (e) { console.error(e); recetaDinamica.value = []; }
}

async function actualizarRecetaFazonConCliente(clienteId: string | number, producto: Producto) {
    if (!clienteId || !producto) return;
    
    const itemFazon = recetaDinamica.value.find(r => r.esFazonInput);
    if (!itemFazon) return;

    // Buscar materiales del cliente
    const materialesCliente = listaInventarioCompleto.value.filter((p: any) => 
        p.esMateriaPrima && p.clienteId == clienteId
    );

    if (materialesCliente.length === 0) {
        itemFazon.nombreInsumo = "⚠️ CLIENTE SIN STOCK DE MP";
        stockFazonDetectado.value = 0;
        return;
    }

    // Algoritmo de Coincidencia
    let materialElegido = null;
    const nombreProd = producto.nombre.toUpperCase();

    const reglas = [
        { key: "TUTTI", match: ["TUTTI", "MEZCLA", "RECUPERADO"] },
        { key: "GRUESO", match: ["GRUESO", "GRU", "GSO", "AI"] }, 
        { key: "FINO", match: ["FINO", "FIN", "LAM", "AI"] },
        { key: "ABS", match: ["ABS"] },
        { key: "PEAD", match: ["PEAD", "AD", "ALTA"] },
        { key: "PEBD", match: ["PEBD", "BAJA"] },
        { key: "PP", match: ["PP", "POLIPROPILENO"] }
    ];

    const regla = reglas.find(r => nombreProd.includes(r.key));
    if (regla) {
        materialElegido = materialesCliente.find(m => {
            const txt = (m.nombre + (m.codigoSku||'')).toUpperCase();
            return regla.match.some(palabra => txt.includes(palabra));
        });
    }

    if (!materialElegido && (nombreProd.includes("A.I.") || nombreProd.includes("IMPACTO"))) {
        materialElegido = materialesCliente.find(m => m.nombre.toUpperCase().includes("AI") || m.nombre.toUpperCase().includes("IMPACTO"));
    }

    if (!materialElegido && materialesCliente.length > 0) {
        materialElegido = materialesCliente[0];
    }

    // Aplicar cambio
    if (materialElegido) {
        const clienteObj = clientes.value.find(c => c.id == clienteId);
        const nombreCli = clienteObj ? clienteObj.razonSocial : 'CLIENTE';

        itemFazon.materiaPrimaId = materialElegido.id;
        itemFazon.nombreInsumo = `MP: ${materialElegido.nombre} (${nombreCli})`;
        itemFazon.densidad = materialElegido.pesoEspecifico;
        stockFazonDetectado.value = materialElegido.stockActual;
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

  if (!form.value.empleadoId) return error.value = "Faltan datos."; 
  
  const pesoNetoGeometrico = kilosCalculados.value;
  if (pesoNetoGeometrico <= 0) return error.value = "Peso calculado es 0.";

  // APLICAR DESPERDICIO SOBRE EL VOLUMEN
  const porcentajeMerma = Number(form.value.merma || 0);
  const factorMerma = 1 + (porcentajeMerma / 100);

  // Este es el peso REAL FÍSICO que se va a fabricar.
  // Ej: 100kg neto * 1.08 = 108.000 kg exactos.
  const pesoBrutoExacto = computed(() => {
    const kilosNetos = Number(props.form.kilosTotales) || 0; // Este viene del cálculo geométrico del padre
    const porcentajeDesperdicio = Number(props.form.merma) || 0; 
    
    // Fórmula física exacta
    return kilosNetos * (1 + (porcentajeDesperdicio / 100));
});

const pesoVisualRedondeado = computed(() => {
    return Math.ceil(pesoBrutoExacto.value);
});
  // REPARTIR ESE PESO EXACTO EN LA RECETA
  const consumosReales = recetaDinamica.value.map(i => {
      const porcentajeEnReceta = parseFloat(i.cantidad.toString()) || 0;
      
      // Regla de tres sobre el peso BRUTO EXACTO
      const kilosInsumo = (pesoBrutoExacto * porcentajeEnReceta) / 100;

      return { 
          materiaPrimaId: Number(i.materiaPrimaId), 
          // Guardamos con 3 decimales para el stock (Gramos)
          cantidadKilos: Number(kilosInsumo.toFixed(3)) 
      };
  });
  
  try {
    await axios.post(`${apiUrl}/Ordenes`, {
        // ... (resto de campos) ...
        productoTerminadoId: Number(form.value.productoTerminadoId), 
        clienteId: form.value.clienteId ? Number(form.value.clienteId) : null, 
        cantidad: Number(form.value.cantidad), 
        empleadoId: Number(form.value.empleadoId), 
        turno: form.value.turno,
        observacion: (form.value.observacion||'') + ` | Geo: ${pesoNetoGeometrico}kg | Merma: ${porcentajeMerma}%`,
        
        // Enviamos el peso EXACTO, no el redondeado
        kilos: Number(pesoBrutoExacto.toFixed(3)), 
        consumos: consumosReales 
    }, getAuthConfig());

    mensaje.value = `✅ Orden Generada. Peso Real: ${pesoBrutoExacto.toFixed(2)}kg`; 
    idProduccionGenerada.value = true; 
    emit('guardado');
  } catch (e: any) { 
      // ... error ...
  }
}

async function generarPDF(tipo: 'orden'|'carga') {
  ocultarFormula.value = (tipo === 'orden'); 
  await nextTick(); await new Promise(r => setTimeout(r, 100)); 
  await html2pdf().set({ margin: 0, filename: `Doc_${Date.now()}.pdf`, image: { type: 'jpeg', quality: 0.98 }, html2canvas: { scale: 2 }, jsPDF: { unit: 'mm', format: 'a4' } }).from(document.getElementById('hoja-de-impresion')).save();
  ocultarFormula.value = false;
}

watch([() => form.value.clienteId, () => form.value.productoTerminadoId], async ([nuevoCli, nuevoProd]) => {
    if (nuevoCli && !form.value.productoTerminadoId) { 
        await CargarProductosFiltrados(nuevoCli);
    }
    if (nuevoCli && form.value.productoTerminadoId) {
        const prod = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
        if (prod && (prod.esFazon || prod.nombre.toUpperCase().includes('FAZON'))) {
            setTimeout(async () => {
                await actualizarRecetaFazonConCliente(nuevoCli, prod);
            }, 200); 
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