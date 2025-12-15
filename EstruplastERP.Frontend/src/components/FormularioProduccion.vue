<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'

// --- 1. CONSTANTES (Ajustables) ---
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 
// Estearato: 1 Lata (aprox 350g) cada 500kg de mezcla
const PESO_LATA_KG = 0.35; 
const KILOS_BASE_LATA = 500;

// --- 2. INTERFACES ---
interface Producto {
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean;
    largo: number; ancho: number; espesor: number; pesoEspecifico: number; color?: string;
}
interface Empleado { id: number; nombreCompleto: string; }
interface Cliente { id: number; razonSocial: string; }
interface ItemReceta { 
    id: number | string; 
    cantidad: number; 
    nombreInsumo: string; 
    densidad: number; 
    materiaPrimaId: number; // Quitamos el ? para evitar errores de tipo
    esColor?: boolean; esCarga?: boolean; esBase?: boolean;
    esBrillo?: boolean;
    esEstearato?: boolean;
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
  
  // -- Terminación y Aditivos --
  conBrillo: false, 
  porcBrillo: 2.00, // Por defecto 2%
  llevaFilm: false, 
  tipoCorona: 'Ninguno', 
  
  conEstearato: false, 
  
  esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '', 
  aditivoUV: false, aditivoCaucho: false, aditivoCarga: 0, 
  kilosTotales: 0 
})

const insumoExtraId = ref('')
const insumoExtraPorc = ref<number | ''>('')
const mensaje = ref('')
const error = ref('')
const idProduccionGenerada = ref(false)

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- 4. COMPUTADOS ---
const productoSeleccionado = computed(() => productos.value.find(p => p.id === form.value.productoTerminadoId) || null);
const empleadoSeleccionado = computed(() => empleados.value.find(e => e.id === form.value.empleadoId) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === form.value.clienteId) || null);

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

// --- STOCK EN TIEMPO REAL ---
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
                faltantes.push({
                    nombre: item.nombreInsumo,
                    necesario: consumoNecesario,
                    disponible: stockDisponible,
                    diferencia: consumoNecesario - stockDisponible
                });
            }
        }
    });
    return faltantes;
});

const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);

const materiasPrimasParaManual = computed(() => {
    return listaTodasMateriasPrimas.value.filter(mp => {
        const nombre = mp.nombre.toUpperCase();
        // Excluir palabras clave de aditivos controlados por lógica
        const esAditivoControlado = 
            nombre.includes('BRILLO') || 
            nombre.includes('ESTEARATO') || 
            nombre.includes('CAUCHO') ||
            nombre.includes('CARGA MINERAL') || // O como se llame tu carga
            nombre.includes('UV'); // Si el UV también es fijo

        return !esAditivoControlado;
    });
});

// --- 5. FUNCIONES ---

async function CargarDatosProductos(id: number) {
    if (!id) return;
    const prod = productos.value.find(p => p.id === id);
    if (prod) {
        if (prod.largo > 0) form.value.largo = prod.largo;
        if (prod.ancho > 0) form.value.ancho = prod.ancho;
        if (prod.espesor > 0) form.value.espesor = prod.espesor;

        form.value.esProductoColor = prod.nombre.toUpperCase().includes('COLOR') || prod.nombre.toUpperCase().includes('VARIOS');
        form.value.colorTexto = prod.color || '';
        form.value.masterbatchId = ''; 
        form.value.aditivoCarga = 0; 
        form.value.aditivoUV = false;
        form.value.aditivoCaucho = false;
    }

    try {
        const res = await axios.get(`${apiUrl}/Formulas/${id}`, getAuthConfig());
        const recetaBase = res.data.map((r: any) => ({
            id: r.id, 
            materiaPrimaId: r.materiaPrimaId,
            nombreInsumo: r.ingrediente || r.Ingrediente,
            cantidad: r.cantidad,
            densidad: r.Densidad || 0.92,
            esColor: false, esCarga: false, esBrillo: false, esEstearato: false,
            esBase: r.cantidad > 50 
        }));
        recetaDinamica.value = recetaBase;
        recalcularFormulaAutomatica();
    } catch (e) {
        console.error("Error cargando fórmula", e);
        recetaDinamica.value = [];
    }
}

function agregarInsumoExtra() {
    if (!insumoExtraId.value || !insumoExtraPorc.value) return;
    const mat = listaTodasMateriasPrimas.value.find(m => m.id === Number(insumoExtraId.value));
    if (!mat) return;

    recetaDinamica.value.push({
        id: Date.now(),
        materiaPrimaId: mat.id,
        nombreInsumo: mat.nombre,
        cantidad: Number(insumoExtraPorc.value),
        densidad: mat.pesoEspecifico || 0.92,
        esBase: false 
    });
    
    recalcularFormulaAutomatica();
    insumoExtraId.value = '';
    insumoExtraPorc.value = '';
}

function quitarInsumo(index: number) {
    recetaDinamica.value.splice(index, 1);
    recalcularFormulaAutomatica();
}

function recalcularFormulaAutomatica() {
    let nuevaReceta = [...recetaDinamica.value];
    if(nuevaReceta.length === 0) return;

    // A. LIMPIEZA
    nuevaReceta = nuevaReceta.filter(r => !r.esColor && !r.esCarga && !r.esBrillo && !r.esEstearato);
    
    // --- B. BRILLO ---
    if (form.value.conBrillo && form.value.porcBrillo > 0) {
        const matBrillo = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('BRILLO'));
        if (matBrillo) {
            nuevaReceta.push({
                id: 'brillo_auto',
                cantidad: parseFloat(parseFloat(form.value.porcBrillo.toString()).toFixed(2)),
                nombreInsumo: matBrillo.nombre,
                densidad: matBrillo.pesoEspecifico || 0.92,
                materiaPrimaId: matBrillo.id,
                esBrillo: true, esBase: false
            });
        }
    }

    // --- C. ESTEARATO (Lógica Lata Durazno) ---
    if (form.value.conEstearato) { 
        const matEstearato = listaTodasMateriasPrimas.value.find(m => m.nombre.toUpperCase().includes('ESTEARATO'));
        if (matEstearato) {
            // Regla: 1 Lata (0.35kg) cada 500kg
            // Porcentaje = (0.35 / 500) * 100
            const porcentajeAutomatico = (PESO_LATA_KG / KILOS_BASE_LATA) * 100;

            nuevaReceta.push({
                id: 'estearato_auto',
                cantidad: parseFloat(porcentajeAutomatico.toFixed(4)), // Mas precisión interna
                nombreInsumo: matEstearato.nombre,
                densidad: matEstearato.pesoEspecifico || 0.92,
                materiaPrimaId: matEstearato.id,
                esEstearato: true, esBase: false
            });
        }
    }

    // --- D. COLOR ---
    if (form.value.esProductoColor && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        if (mb) {
            nuevaReceta.push({
                id: 'color_auto', cantidad: 2, 
                nombreInsumo: mb.nombre, densidad: mb.pesoEspecifico, 
                esColor: true, materiaPrimaId: mb.id, esBase: false
            });
        }
    }

    // --- E. CARGA ---
    if (form.value.aditivoCarga > 0) {
        // 1. Intentamos buscar el producto real en tu lista de stock
        const matCarga = listaTodasMateriasPrimas.value.find(m => 
            m.nombre.toUpperCase().includes('CARGA') || 
            m.nombre.toUpperCase().includes('CARBONATO') || 
            m.nombre.toUpperCase().includes('TALCO') || 
            m.nombre.toUpperCase().includes('CALCIO') ||
            m.nombre.toUpperCase().includes('MINERAL') // Agregué este por si acaso
        );

        if (matCarga) {
            // CASO IDEAL: Encontramos el producto, usamos su ID real
            nuevaReceta.push({ 
                id: 'carga_auto', 
                cantidad: form.value.aditivoCarga, 
                nombreInsumo: matCarga.nombre, 
                densidad: matCarga.pesoEspecifico || 1.8, 
                materiaPrimaId: matCarga.id, 
                esCarga: true, esBase: false
            });
        } else {
            // CASO FALLBACK: No encontramos el nombre, pero lo agregamos visualmente
            console.warn("⚠️ No se encontró 'Carga' en stock. Se agrega visualmente.");
            
            // Imprimimos la lista en la consola (F12) para que veas qué nombres tienes
            console.log("Nombres disponibles:", listaTodasMateriasPrimas.value.map(m => m.nombre));

            nuevaReceta.push({ 
                id: 'carga_auto', 
                cantidad: form.value.aditivoCarga, 
                nombreInsumo: 'CARGA MINERAL (Manual)', // Nombre genérico
                densidad: 1.8, 
                materiaPrimaId: 0, // ID 0 indica que no se descontará stock automático
                esCarga: true, esBase: false
            });
        }
    }

    // --- F. AJUSTAR BASE ---
    let itemBase = nuevaReceta.find(r => r.esBase);
    if (!itemBase && nuevaReceta.length > 0) {
        const candidatosBase = nuevaReceta.filter(r => !r.esColor && !r.esCarga && !r.esBrillo && !r.esEstearato);
        if (candidatosBase.length > 0) {
             itemBase = candidatosBase.reduce((prev, current) => (prev.cantidad > current.cantidad) ? prev : current);
             itemBase.esBase = true;
        }
    }

    if (itemBase) {
        const sumaOtros = nuevaReceta.reduce((sum, item) => {
            return item.esBase ? sum : sum + parseFloat(item.cantidad.toString());
        }, 0);
        let nuevoPorcentajeBase = 100 - sumaOtros;
        if (nuevoPorcentajeBase < 0) nuevoPorcentajeBase = 0;
        itemBase.cantidad = parseFloat(nuevoPorcentajeBase.toFixed(2));
    }
    
    recetaDinamica.value = nuevaReceta;
}

// --- REGISTRAR PRODUCCIÓN (Envío de Kilos) ---
async function registrarProduccion() {
  mensaje.value = ''; error.value = '';
  
  if (!form.value.empleadoId || form.value.kilosTotales <= 0) {
      error.value = "Faltan datos obligatorios."; return;
  }
  
  if (Math.abs(totalPorcentajeReceta.value - 100) > 0.5) {
      error.value = `⛔ La receta suma ${totalPorcentajeReceta.value}%. Debe ser 100%.`; return;
  }

  const colorFinal = colorFinalParaPDF.value;
  let techDetails = ` | ${form.value.largo}x${form.value.ancho}x${form.value.espesor}mm`;
  techDetails += ` | Color: ${colorFinal}`;
  techDetails += ` | Corona: ${form.value.tipoCorona}`;
  techDetails += ` | Brillo: ${form.value.conBrillo ? 'SI' : 'NO'}`;
  if(form.value.conBrillo) techDetails += ` | Film: ${form.value.llevaFilm ? 'SI' : 'NO'}`;
  if(form.value.aditivoUV) techDetails += ` | UV: SÍ`;
  if(form.value.aditivoCaucho) techDetails += ` | Caucho: SÍ`;
  if(form.value.aditivoCarga > 0) techDetails += ` | Carga: ${form.value.aditivoCarga}%`;

  // Calcular Kilos Reales para el Backend
  const listaConsumos = recetaDinamica.value.map(item => {
      const kilosCalc = (form.value.kilosTotales * parseFloat(item.cantidad.toString())) / 100;
      return {
          materiaPrimaId: item.materiaPrimaId,
          cantidadKilos: Number(kilosCalc.toFixed(3)) 
      };
  });

  const payload = {
    productoTerminadoId: form.value.productoTerminadoId,
    clienteId: form.value.clienteId || null,
    cantidad: form.value.cantidad,
    empleadoId: form.value.empleadoId,
    turno: form.value.turno,
    observacion: (form.value.observacion || '') + techDetails,
    kilos: form.value.kilosTotales,
    consumos: listaConsumos 
  }
  
  try {
    await axios.post(`${apiUrl}/Produccion/registrar`, payload, getAuthConfig())
    mensaje.value = `✅ Orden Generada Correctamente.`
    idProduccionGenerada.value = true 
    emit('guardado') 
  } catch (e: any) { 
    console.error(e);
    error.value = '❌ ' + (e.response?.data?.mensaje || e.message); 
  }
}

function generarOrdenProduccionPDF() {
  const elemento = document.getElementById('hoja-de-impresion')
  const opt = { margin: 0, filename: `Orden_${Date.now()}.pdf`, image: { type: 'jpeg', quality: 0.98 }, html2canvas: { scale: 2 }, jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' } };
  // @ts-ignore
  html2pdf().set(opt).from(elemento).save()
}

// --- 6. WATCHERS ---
watch(() => form.value.productoTerminadoId, (newId) => {
    if (newId) CargarDatosProductos(Number(newId));
    else { recetaDinamica.value = []; form.value.largo=0; form.value.ancho=0; form.value.espesor=0; }
});
watch(() => form.value.masterbatchId, recalcularFormulaAutomatica);
watch(() => form.value.aditivoCarga, recalcularFormulaAutomatica);
watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { 
    if (!v) form.value.llevaFilm = false; 
    recalcularFormulaAutomatica();
});
watch(() => form.value.porcBrillo, recalcularFormulaAutomatica);
watch(() => form.value.conEstearato, recalcularFormulaAutomatica);
watch(kilosCalculados, (v) => form.value.kilosTotales = v);

onMounted(async () => {
    try {
        const config = getAuthConfig();
        const [resProd, resEmp, resCli, resMat] = await Promise.all([
            axios.get(`${apiUrl}/Productos`, config),
            axios.get(`${apiUrl}/Empleados`, config),
            axios.get(`${apiUrl}/Clientes`, config),
            axios.get(`${apiUrl}/Productos/materias-primas`, config) 
        ]);
        
        productos.value = resProd.data.filter((p: any) => p.esProductoTerminado);
        listaTodasMateriasPrimas.value = resMat.data;
        listaMasterbatches.value = resMat.data.filter((m: any) => 
            m.nombre && (m.nombre.toUpperCase().includes('MASTERBATCH') || m.nombre.toUpperCase().includes('PIGMENTO'))
        );
        empleados.value = resEmp.data;
        clientes.value = resCli.data;
    } catch (e) { console.error(e); error.value = 'Error de conexión.'; }
});
</script>

<template>
  <div class="contenedor-doble">
    
    <div id="hoja-de-impresion" class="hoja-papel">
      <div class="cabecera">
        <img src="." alt="LOGO" class="logo-img"> 
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
        <div class="dato-box"><span class="label-tech">COLOR</span><span class="valor-tech">{{ colorFinalParaPDF }}</span></div>
        <div class="dato-box"><span class="label-tech">LARGO</span><span class="valor-tech">{{ form.largo }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ANCHO</span><span class="valor-tech">{{ form.ancho }} mm</span></div>
        <div class="dato-box"><span class="label-tech">ESPESOR</span><span class="valor-tech">{{ form.espesor }} mm</span></div>
      </div>

      <div class="ficha-tecnica">
        <div class="dato-box">
            <span class="label-tech">BRILLO</span>
            
            <div v-if="form.conBrillo">
                <span class="valor-tech">SÍ ({{ form.porcBrillo }}%)</span>
                <span style="font-size: 10px; display:block;">
                    {{ ((form.kilosTotales * form.porcBrillo) / 100).toFixed(2) }} kg
                </span>
            </div>
            <span v-else class="valor-tech">NO</span>
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

      <div class="ficha-tecnica" v-if="form.aditivoUV || form.aditivoCaucho || form.aditivoCarga > 0 || form.conEstearato">
          
          <div class="dato-box" v-if="form.aditivoUV">
              <span class="label-tech">ADITIVO UV</span>
              <span class="valor-tech">SÍ</span>
          </div>
          <div class="dato-box" v-if="form.aditivoCaucho">
              <span class="label-tech">ADITIVO CAUCHO</span>
              <span class="valor-tech">SÍ</span>
          </div>
          <div class="dato-box" v-if="form.aditivoCarga > 0">
              <span class="label-tech">CARGA MINERAL</span>
              <span class="valor-tech">{{ form.aditivoCarga }} %</span>
          </div>

          <div class="dato-box" v-if="form.conEstearato">
              <span class="label-tech">ESTEARATO</span>
              <span class="valor-tech" style="font-size: 12px;">
                  {{ (form.kilosTotales / 500).toFixed(1) }} Latas
              </span>
              <span style="font-size: 10px; display:block; font-weight:bold">
                  Total: {{ ((form.kilosTotales * (PESO_LATA_KG / KILOS_BASE_LATA) * 100) / 100).toFixed(3) }} kg
              </span>
          </div>
      </div>

      <div v-if="recetaDinamica.length > 0" class="seccion-receta">
          <div class="titulo-seccion-receta">
              FÓRMULA (Densidad: {{ densidadMezcla }})
              <span style="float:right; font-size: 0.8em; color: #333">Total: {{ totalPorcentajeReceta }}%</span>
          </div>
          <table class="tabla-receta">
              <thead><tr><th>INSUMO</th><th style="width:100px">% MEZCLA</th><th style="width:100px">PESO REAL</th><th data-html2canvas-ignore="true" style="width:40px"></th></tr></thead>
              <tbody>
                  <tr v-for="(r, i) in recetaDinamica" :key="i">
                    <td>{{ r.nombreInsumo }}</td>
                    
                    <td style="text-align:center">
                        <div v-if="r.esEstearato">
                             <span style="font-weight:bold; font-size: 14px;">{{ r.cantidad }} %</span>
                             <div style="font-size: 9px; color: #555;">(1 Lata x 500kg)</div>
                        </div>
                        <div v-else>
                            <input type="number" v-model="r.cantidad" class="input-sin-borde" step="0.01"> %
                        </div>
                    </td>

                    <td style="text-align:right">
                        <strong>{{ ((form.kilosTotales * (parseFloat(r.cantidad.toString()) || 0)) / 100).toFixed(3) }} kg</strong>
                         
                         <div v-if="r.esEstearato" style="font-size: 10px; color: #7f8c8d;">
                             aprox. {{ (form.kilosTotales / 500).toFixed(1) }} Latas
                         </div>
                    </td>

                    <td data-html2canvas-ignore="true">
                        <button @click="quitarInsumo(i)" style="background:none; border:none; color:red; cursor:pointer; font-weight:bold;">X</button>
                    </td>
                  </tr>
              </tbody>
          </table>
          
          <div class="agregar-fila" data-html2canvas-ignore="true">
    <select v-model="insumoExtraId" style="width: 200px;">
        <option value="">+ Agregar Insumo...</option>
        <option v-for="mp in materiasPrimasParaManual" :key="mp.id" :value="mp.id">
            {{ mp.nombre }}
        </option>
    </select>
    
    <input type="number" v-model="insumoExtraPorc" placeholder="%" style="width: 60px;">
    <button @click="agregarInsumoExtra" type="button" style="background:#2ecc71; color:white; border:none; padding:5px 10px; cursor:pointer;">Añadir</button>
</div>
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
            <div v-if="form.esProductoColor" style="margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px;">
                <label style="color: #f39c12; font-weight: bold;">🎨 Seleccione Color:</label>
                <select v-model="form.masterbatchId">
                    <option disabled value="">-- Elegir Masterbatch --</option>
                    <option v-for="mb in listaMasterbatches" :key="mb.id" :value="mb.id">{{ mb.nombre }}</option>
                </select>
            </div>

            <label class="lbl-sep">Medidas (mm):</label>
            <div class="fila-input">
                <div><label>Largo</label><input type="number" v-model="form.largo"></div>
                <div><label>Ancho</label><input type="number" v-model="form.ancho"></div>
            </div>
            <div class="fila-input">
                <div><label>Esp (mm)</label><input type="number" v-model="form.espesor" step="0.1"></div>
                <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
            </div>
            
            <label class="lbl-sep">Aditivos y Terminación:</label>

            <div class="fila-control-aditivo">
                <label class="check-container" :class="{ 'disabled': form.espesor < 1 }">
                    <input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> 
                    ✨ Brillo
                </label>
                
                <div v-if="form.conBrillo" class="bloque-derecha">
                    <div class="input-porcentaje">
                        <input type="number" v-model="form.porcBrillo" step="0.01" min="0"> %
                    </div>
                    <div v-if="form.kilosTotales > 0" class="texto-kilos-extra">
                        = {{ ((form.kilosTotales * form.porcBrillo) / 100).toFixed(2) }} kg
                    </div>
                </div>
            </div>

            <label class="check-container" :class="{ 'disabled': !form.conBrillo }">
                <input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 
                🛡️ Con Film Protector
            </label>

            <div class="fila-control-aditivo">
                <label class="check-container">
                    <input type="checkbox" v-model="form.conEstearato"> 
                    🧪 Estearato
                </label>
                
                <div v-if="form.conEstearato" class="info-estearato">
                    <span class="regla-tapita">1 Lata / 500kg</span>
                    
                    <div v-if="form.kilosTotales > 0">
                        <span>{{ (form.kilosTotales / 500).toFixed(1) }} Latas</span>
                        <div class="texto-kilos-extra">
                            = {{ ((form.kilosTotales * (PESO_LATA_KG / KILOS_BASE_LATA) * 100) / 100).toFixed(3) }} kg
                        </div>
                    </div>
                </div>
            </div>

            <div class="fila-checkbox" style="margin-top: 10px;">
                <label class="check-container"><input type="checkbox" v-model="form.aditivoUV"> ☀️ UV</label>
                <label class="check-container"><input type="checkbox" v-model="form.aditivoCaucho"> 🚜 Caucho</label>
            </div>

            <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
            <select v-model="form.tipoCorona">
                <option value="Ninguno">Sin Tratamiento</option>
                <option value="Simple">Simple</option>
                <option value="Doble">Doble</option>
            </select>

            <label class="lbl-sep">Cargas:</label>
            <div class="fila-input">
                 <div style="flex:1"><label>Carga Mineral (%)</label><input type="number" v-model="form.aditivoCarga"></div>
            </div>

            <div class="resumen-peso">Peso Calc: {{ form.kilosTotales }} Kg</div>
        </div>
        
        <div class="fila-input" style="margin-top:10px">
             <div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div>
        </div>

        <div v-if="Math.abs(totalPorcentajeReceta - 100) > 0.5" class="alerta-error">
            ⚠️ La receta suma {{ totalPorcentajeReceta }}%. Debe ajustar al 100%.
        </div>

        <div v-if="hayBloqueoDeStock" class="alerta-stock">
            <h4>🚫 Stock Insuficiente</h4>
            <ul>
                <li v-for="(falla, i) in insumosSinStock" :key="i">
                    <strong>{{ falla.nombre }}</strong>: 
                    Necesitas {{ falla.necesario.toFixed(2) }} kg 
                    (Tienes {{ falla.disponible.toFixed(2) }} kg)
                </li>
            </ul>
            <small>No se puede guardar hasta reponer stock.</small>
        </div>

        <button class="btn-guardar" 
                @click="registrarProduccion" 
                :disabled="!form.empleadoId || form.kilosTotales <= 0 || hayBloqueoDeStock">
            <span v-if="hayBloqueoDeStock">⚠️ REVISAR STOCK</span>
            <span v-else>💾 GUARDAR</span>
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
.agregar-fila { padding: 10px; border-top: 1px solid #ccc; display: flex; gap: 5px; align-items: center; justify-content: flex-end; background: #f9f9f9; }
.fila-lotes { display: flex; gap: 20px; margin-top: 20px; }
.mitad { flex: 1; }
.recuadro-gigante { border: 3px solid black; height: 40px; font-size: 24px; display: flex; align-items: center; justify-content: center; margin-top: 5px; font-weight: 900; }
.texto-lote { font-size: 16px; }
.pie-firma { margin-top: 60px; display: flex; justify-content: space-around; }
.linea-firma { border-top: 2px solid black; width: 40%; text-align: center; font-size: 11px; padding-top: 5px; font-weight: bold; }
.panel-control { width: 320px; min-width: 320px; background: #2c3e50; color: white; padding: 20px; border-radius: 8px; position: sticky; top: 10px; height: fit-content; box-shadow: 0 4px 10px rgba(0,0,0,0.2); }
.panel-control h3 { margin-top: 0; border-bottom: 1px solid #4e6475; padding-bottom: 10px; color: #ecf0f1; }
.panel-control label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; }
.panel-control select, .panel-control input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 14px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
.fila-input { display: flex; gap: 10px; margin-bottom: 5px; }
.fila-checkbox { display: flex; gap: 15px; margin-bottom: 5px; }
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
.btn-imprimir { width: 100%; padding: 12px; background: #2980b9; color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; margin-top: 10px; font-size: 14px; }
.btn-imprimir:hover { background: #3498db; }
.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; }
.input-sin-borde { border: none; background: transparent; font-weight: bold; color: inherit; width: 60px; text-align: center; }
.input-sin-borde:focus { border-bottom: 1px solid #000; outline: none; }

.fila-control-aditivo {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-top: 8px;
}

.input-porcentaje {
    display: flex;
    align-items: center;
    background: #ecf0f1;
    border-radius: 4px;
    padding-right: 5px;
    color: #333;
}

.input-porcentaje input {
    width: 50px !important;
    margin: 0 !important;
    text-align: right;
    background: transparent !important;
    color: #333 !important;
    font-weight: bold;
}

.alerta-stock {
    background-color: #ffebee;
    border: 1px solid #ef5350;
    color: #c62828;
    padding: 10px;
    border-radius: 6px;
    margin-top: 15px;
    font-size: 13px;
    text-align: left;
}

.alerta-stock h4 {
    margin: 0 0 5px 0;
    font-size: 14px;
    font-weight: bold;
    display: flex;
    align-items: center;
    gap: 5px;
}

.alerta-stock ul {
    margin: 0;
    padding-left: 20px;
}

.alerta-stock li {
    margin-bottom: 3px;
}

/* Ajuste visual para cuando el botón está deshabilitado por stock */
.btn-guardar:disabled {
    background-color: #95a5a6;
    cursor: not-allowed;
    opacity: 0.8;
}

/* Estilos nuevos para los cálculos */
.bloque-derecha {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
}

.texto-kilos-extra {
    color: #2ecc71; /* Verde brillante */
    font-size: 11px;
    font-weight: bold;
    margin-top: 2px;
}

.info-estearato {
    font-size: 11px; 
    color: #f39c12; 
    text-align: right;
    display: flex;
    flex-direction: column;
}

.regla-tapita {
    font-weight: bold; 
    margin-bottom: 2px;
}
@media (max-width: 1000px) { .contenedor-doble { flex-direction: column; align-items: center; } .panel-control { width: 100%; position: static; } }
</style>