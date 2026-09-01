<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import HojaImpresion from '../components/HojaImpresion.vue'
import ListaProduccion from '../components/ListaProduccion.vue'
import { ProduccionAPI } from '@/services/produccionService'
import { useCalculosProduccion } from '@/composables/useCalculosProduccion';
import { useBorradorProduccion } from '@/composables/useBorradorProduccion';
import { useImpresionProduccion } from '@/composables/useImpresionProduccion';
import { useRecetaProduccion } from '@/composables/useRecetaProduccion';
import { useFiltrosProduccion } from '@/composables/useFiltrosProduccion';
import { useFazonProduccion } from '@/composables/useFazonProduccion';
import { useGuardadoProduccion } from '@/composables/useGuardadoProduccion';

interface Producto {
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean;
    esGenerico: boolean; esFazon?: boolean; esMateriaPrima?: boolean; esScrap?: boolean; rubro?: string;
    largo: number; ancho: number; espesor: number; pesoEspecifico: number; color?: string;
    receta?: any[]; espesorMinimo?: number; espesorMaximo?: number; clienteId?: number;
    tipoMaterial?: string;
}

interface Cliente { 
    id: number; 
    razonSocial: string; 
    esFazon?: boolean; 
    limiteKilosPallet?: number; 
    LimiteKilosPallet?: number; 
}

interface ItemReceta {
    id: number | string; cantidad: number | string; nombreInsumo: string; densidad: number;
    materiaPrimaId: number; esColor?: boolean; esCarga?: boolean; esBase?: boolean;
    esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean;
    esFazonInput?: boolean; materialBase?: string;
    kilosFijos?: number | string;
    extrusoraDestino?: string;
}

const apiUrl = import.meta.env.VITE_API_URL || '/api'; 
const DENSIDAD_DEFAULT = 1.1;
const ID_MASTERBATCH_GENERICO = 90; 
const PESO_LATA_KG = 0.150;        
const KILOS_BASE_LATA = 25;

const loading = ref(false);
const guardando = ref(false); 
const mutandoParaGuardar = ref(false); 

const productos = ref<Producto[]>([])
const listaInventarioCompleto = ref<any[]>([])
const listaTodasMateriasPrimas = ref<any[]>([])
const clientes = ref<Cliente[]>([])
const recetaDinamica = ref<ItemReceta[]>([])
const stockFazonDetectado = ref<number | null>(null);
const listaLotesCliente = ref<any[]>([]); 
const loteFazonSeleccionadoId = ref<string | number>('');
const imprimiendoHistorial = ref(false);
const listaProduccionRef = ref<any>(null);
const limiteMinimo = ref(0);
const limiteMaximo = ref(0);
const mensaje = ref('');
const error = ref('');
const uiLlevaImpresion = ref(false);
const idProduccionGenerada = ref(false);
const ocultarFormula = ref(false);
const cantidadPalletsUsuario = ref(1);
const notaPedidoSugerida = ref<string>('');

const tipoSalidaVisual = ref<'NORMAL' | 'NATURAL'>('NORMAL');

const emit = defineEmits(['guardado'])

const form = ref({
    productoTerminadoId: '' as string | number,
    clienteId: '' as string | number,
    numeroPedidoCliente: '',
    notaPedido: '',
    cantidad: 1,
    observacion: '',
    largo: 0, ancho: 0, espesor: 0, color: '' as string,
    conBrillo: false, 
    tipoBrillo: '777',
    porcBrillo: 10.00, 
    llevaFilm: false, tipoCorona: 'Ninguno',
    esGofrado: false,
    conEstearato: false, 
    esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '',
    aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00,
    aditivoCarga: 0,
    merma: 8, 
    kilosTotales: 0,
    esConsolidado: false,
    esBobina: false,
    kilosPorBobina: 0,
    productoNombre: '',
    clienteNombre: '',
    porcentajeTolvaA: 0,
    porcentajeTolvaB: 0,
    porcentajeTolvaC: 0
})

const ultimoPedidoGuardado = ref({
    clienteId: '' as string | number,
    numeroPedidoCliente: '',
    notaPedido: ''
});
const mostrarOpcionMismoPedido = ref(false);

const usaCoextrusionCalculado = computed(() => recetaDinamica.value.some(r => r.extrusoraDestino === 'A' || r.extrusoraDestino === 'B' || r.extrusoraDestino === 'C'));

const espesorValido = computed(() => {
    if (limiteMinimo.value === 0 && limiteMaximo.value === 0) return true;
    const esp = Number(form.value.espesor) || 0;
    if (esp === 0) return false;
    if (limiteMaximo.value === 0) return esp >= limiteMinimo.value;
    return esp >= limiteMinimo.value && esp <= limiteMaximo.value;
});

const productoSeleccionado = computed(() => productos.value.find(p => p.id === Number(form.value.productoTerminadoId)) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === Number(form.value.clienteId)) || null);

const densidadPT = computed(() => productoSeleccionado.value?.pesoEspecifico || 1.1);

const porcentajesPorTolva = computed(() => {
    let sumas = { UNICA: 0, A: 0, B: 0, C: 0 };
    let tieneCoextrusion = false;

    recetaDinamica.value.forEach((r: any) => {
        const mpId = r.materiaPrimaId || r.id;
        const mpInfo = listaTodasMateriasPrimas.value.find(m => m.id === mpId) || listaInventarioCompleto.value.find(m => m.id === mpId);
        
        const rubro = mpInfo ? String(mpInfo.rubro || mpInfo.Rubro || '').toUpperCase() : '';
        const nombreMaterial = mpInfo ? String(mpInfo.nombre || mpInfo.Nombre || '').toUpperCase() : '';
        const esCargaFisica = r.esCarga || nombreMaterial.includes('CARGA') || nombreMaterial.includes('CARBONATO') || nombreMaterial.includes('TIZA');

        const destino = r.extrusoraDestino || r.ExtrusoraDestino || 'UNICA';
        if (destino === 'A' || destino === 'B' || destino === 'C') tieneCoextrusion = true;

        if ((rubro !== 'ADITIVO' && rubro !== 'OTROS') || esCargaFisica) {
            if (destino === 'A') sumas.A += Number(r.cantidad || 0);
            else if (destino === 'B') sumas.B += Number(r.cantidad || 0);
            else if (destino === 'C') sumas.C += Number(r.cantidad || 0);
            else sumas.UNICA += Number(r.cantidad || 0);
        }
    });

    return {
        A: Math.round(sumas.A * 100) / 100,
        B: Math.round(sumas.B * 100) / 100,
        C: Math.round(sumas.C * 100) / 100,
        UNICA: Math.round(sumas.UNICA * 100) / 100,
        usaCoextrusion: tieneCoextrusion
    };
});

const errorBasePorcentaje = computed(() => {
    if (mutandoParaGuardar.value) return null;

    const p = porcentajesPorTolva.value;
    if (p.usaCoextrusion) {
        const errA = p.A > 0 && Math.abs(p.A - 100) > 0.5;
        const errB = p.B > 0 && Math.abs(p.B - 100) > 0.5;
        const errC = p.C > 0 && Math.abs(p.C - 100) > 0.5;
        if (errA || errB || errC) {
            let msj = [];
            if (errA) msj.push(`Tolva A (${p.A}%)`);
            if (errB) msj.push(`Tolva B (${p.B}%)`);
            if (errC) msj.push(`Tolva C (${p.C}%)`);
            return `Mezcla incompleta: ${msj.join(' / ')}. Cada tolva debe sumar 100%.`;
        }
        return null;
    } else {
        if (Math.abs(p.UNICA - 100) > 0.5 && recetaDinamica.value.length > 0) {
            return `La mezcla única suma ${p.UNICA}%. Debe sumar exactamente 100%.`;
        }
        return null;
    }
});

const porcentajeSoloBase = computed(() => {
    return errorBasePorcentaje.value ? 0 : 100;
});

const { factorMerma } = useCalculosProduccion(form, recetaDinamica, productoSeleccionado);

const productosDropdownOrdenados = computed(() => {
    if (!listaProductosDisponibles.value) return [];
    return [...listaProductosDisponibles.value].sort((a, b) => {
        const aFazon = a.esFazon ? 1 : 0;
        const bFazon = b.esFazon ? 1 : 0;
        if (aFazon !== bFazon) return aFazon - bFazon; 
        return (a.nombre || '').localeCompare(b.nombre || '');
    });
});

const kilosCalculados = computed(() => form.value.kilosTotales);

const kilosEstearato = computed(() => {
    let kilosBase = Number(form.value.kilosTotales);
    if (isNaN(kilosBase) || kilosBase <= 0) {
        kilosBase = Number(kilosCalculados.value);
    }
    return kilosBase * 0.0008;
});

const recetaConExtrasParaVista = computed(() => {
    if (imprimiendoHistorial.value) {
        return recetaDinamica.value.map(r => {
            return {
                ...r,
                cantidad: Number(r.cantidad || 0).toFixed(2),
                kilosFijos: Number(r.kilosFijos || 0).toFixed(2)
            };
        });
    }

    // 🚀 CLONACIÓN VITAL PARA EVITAR BUCLE INFINITO AL IMPRIMIR
    const recetaLimpia = recetaDinamica.value
        .filter(r => {
            const n = (r.nombreInsumo || '').toUpperCase();
            return !n.includes('ESTEARATO') && !n.includes('BRILLO') && !n.includes('UV') && !n.includes('CAUCHO');
        })
        .map(r => ({ ...r })); 

    const kilosBase = form.value.kilosTotales > 0 ? form.value.kilosTotales : 1;
    const usaCoextrusion = usaCoextrusionCalculado.value;
    
    const porcA = usaCoextrusion ? Number(form.value.porcentajeTolvaA) : 100;
    const porcB = usaCoextrusion ? Number(form.value.porcentajeTolvaB) : 100;
    const porcC = usaCoextrusion ? Number(form.value.porcentajeTolvaC) : 100;

    const kilosCapaA = kilosBase * (porcA / 100);
    const kilosMasaB = kilosBase * (porcB / 100);
    const kilosCapaC = kilosBase * (porcC / 100);

    recetaLimpia.forEach(r => {
        const destino = r.extrusoraDestino || 'UNICA';
        let kilosDeLaTolva = kilosBase;
        
        if (destino === 'A') kilosDeLaTolva = kilosCapaA;
        if (destino === 'B') kilosDeLaTolva = kilosMasaB;
        if (destino === 'C') kilosDeLaTolva = kilosCapaC;
        
        r.kilosFijos = ((kilosDeLaTolva * Number(r.cantidad)) / 100).toFixed(2);
    });

    const tolvaCapa = usaCoextrusion ? 'A' : 'UNICA';
    const tolvaMasa = usaCoextrusion ? 'B' : 'UNICA';

    const est = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('ESTEARATO'));
    if (est && kilosEstearato.value > 0) {
        const valorKilos = kilosEstearato.value.toFixed(2);
        recetaLimpia.push({
            id: 'estearato-fijo', materiaPrimaId: est.id, nombreInsumo: `🧪 ${est.nombre}`,
            densidad: est.pesoEspecifico || 1, esEstearato: true, 
            cantidad: 0.08, 
            kilosFijos: valorKilos,
            extrusoraDestino: tolvaMasa
        });
    }
    
    if (form.value.conBrillo && form.value.porcBrillo > 0) {
        const keywordBrillo = form.value.tipoBrillo === '555' ? '555' : '777';
        let mpBrillo = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes(`BRILLO ${keywordBrillo}`)) || listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('BRILLO'));
        if (mpBrillo) {
            const baseParaBrillo = tolvaCapa === 'A' ? kilosCapaA : kilosBase;
            const kilosAditivo = ((baseParaBrillo * form.value.porcBrillo) / 100).toFixed(2);
            recetaLimpia.push({
                id: 'brillo-fijo', materiaPrimaId: mpBrillo.id, nombreInsumo: `✨ ${mpBrillo.nombre}`,
                densidad: mpBrillo.pesoEspecifico || 1, 
                cantidad: form.value.porcBrillo,
                kilosFijos: kilosAditivo,
                extrusoraDestino: tolvaCapa
            });
        }
    }

    if (form.value.aditivoUV && form.value.porcentajeUv > 0) {
        const mpUV = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('UV'));
        if (mpUV) {
            const baseParaUv = tolvaCapa === 'A' ? kilosCapaA : kilosBase;
            const kilosAditivo = ((baseParaUv * form.value.porcentajeUv) / 100).toFixed(2);
            recetaLimpia.push({
                id: 'uv-fijo', materiaPrimaId: mpUV.id, nombreInsumo: `☀️ ${mpUV.nombre}`,
                densidad: mpUV.pesoEspecifico || 1, 
                cantidad: form.value.porcentajeUv,
                kilosFijos: kilosAditivo,
                extrusoraDestino: tolvaCapa
            });
        }
    }
    
    if (form.value.aditivoCaucho && form.value.porcentajeCaucho > 0) {
        const mpCaucho = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('CAUCHO'));
        if (mpCaucho) {
            const baseParaCaucho = tolvaMasa === 'B' ? kilosMasaB : kilosBase;
            const kilosAditivo = ((baseParaCaucho * form.value.porcentajeCaucho) / 100).toFixed(2);
            recetaLimpia.push({
                id: 'caucho-fijo', materiaPrimaId: mpCaucho.id, nombreInsumo: `🚜 ${mpCaucho.nombre}`,
                densidad: mpCaucho.pesoEspecifico || 1, 
                cantidad: form.value.porcentajeCaucho,
                kilosFijos: kilosAditivo,
                extrusoraDestino: tolvaMasa
            });
        }
    }

    return recetaLimpia.map(r => {
        let c = Number(r.cantidad || 0).toFixed(2);
        let k = Number(r.kilosFijos || 0).toFixed(2);
        return { ...r, cantidad: c, kilosFijos: k };
    });
});

const { 
    borradorDisponible, revisarBorrador, recuperarBorrador, descartarBorrador, limpiarBorrador
} = useBorradorProduccion(form, recetaDinamica, mensaje);

const { 
    listaMasterbatches, idCristal555, mostrarCajaColor, colorFinalParaPDF,
    clienteTieneFazonActivo, clientesHabilitados, medidasBloqueadas,
    listaProductosDisponibles, materiasPrimasLimpias, insumosSinStock, hayBloqueoDeStock
} = useFiltrosProduccion(
    form, recetaDinamica, productos, clientes, listaTodasMateriasPrimas, 
    listaInventarioCompleto, productoSeleccionado, clienteSeleccionado, 
    kilosCalculados, factorMerma, limiteMinimo, limiteMaximo
);

const { 
    balancearBase, recalcularFormulaAutomatica, quitarInsumoManual, agregarInsumoDesdeHijo 
} = useRecetaProduccion(
    form, recetaDinamica, listaTodasMateriasPrimas, listaInventarioCompleto, 
    listaMasterbatches, idCristal555, mostrarCajaColor
);

const { 
    detectarMaterial, actualizarRecetaFazonConCliente, alCambiarLoteFazon, aplicarLoteFazonAReceta 
} = useFazonProduccion(
    recetaDinamica, listaInventarioCompleto, listaTodasMateriasPrimas,
    listaLotesCliente, loteFazonSeleccionadoId, stockFazonDetectado, 
    clienteTieneFazonActivo, balancearBase
);

const { 
    limpiarFormulario, registrarProduccion, cargarNotaPedidoSugerida, aplicarNotaPedidoSugerida 
} = useGuardadoProduccion(
    form, recetaDinamica, notaPedidoSugerida, mensaje, error, guardando, 
    idProduccionGenerada, porcentajeSoloBase, espesorValido, limiteMinimo, 
    limiteMaximo, kilosCalculados, colorFinalParaPDF, listaProduccionRef, 
    limpiarBorrador, emit
);

const { imprimirDesdeHistorial, imprimirLoteOPsDesdeHistorial } = useImpresionProduccion(
    form, 
    recetaDinamica, 
    ocultarFormula, 
    imprimiendoHistorial, 
    cantidadPalletsUsuario,
    mensaje, 
    error, 
    loading, 
    listaProduccionRef, 
    balancearBase, 
    limpiarFormulario,
    listaInventarioCompleto 
);

async function CargarProductosFiltrados(clienteId: number | string = '') {
    try {
        const cid = clienteId ? clienteId : '';
        productos.value = await ProduccionAPI.obtenerProductos(cid);
        if (form.value.productoTerminadoId) {
            const estaEnLista = listaProductosDisponibles.value.some(p => p.id === Number(form.value.productoTerminadoId));
            if (!estaEnLista) {
                form.value.productoTerminadoId = '';
                recetaDinamica.value = [];
            }
        }
    } catch (e) { console.error(e); }
}

async function CargarDatosProductos(id: number) {
    if (!id || imprimiendoHistorial.value) return; 
    try {
        const prod = await ProduccionAPI.obtenerProductoPorId(id);

        if (prod.receta && Array.isArray(prod.receta) && prod.receta.length > 0) {
            
            const tieneA = prod.receta.some((r: any) => (r.extrusoraDestino || 'UNICA') === 'A');
            const tieneB = prod.receta.some((r: any) => (r.extrusoraDestino || 'UNICA') === 'B');
            const tieneC = prod.receta.some((r: any) => (r.extrusoraDestino || 'UNICA') === 'C');

            if (tieneA && tieneB && !tieneC) {
                form.value.porcentajeTolvaA = 20;
                form.value.porcentajeTolvaB = 80;
                form.value.porcentajeTolvaC = 0;
            } else if (tieneA && tieneB && tieneC) {
                form.value.porcentajeTolvaA = 10;
                form.value.porcentajeTolvaB = 80;
                form.value.porcentajeTolvaC = 10;
            } else {
                form.value.porcentajeTolvaA = 100;
                form.value.porcentajeTolvaB = 0;
                form.value.porcentajeTolvaC = 0;
            }

            recetaDinamica.value = prod.receta.map((r: any) => {
                const mpId = r.materiaPrimaId || r.id;
                const mp = listaTodasMateriasPrimas.value.find(m => m.id === mpId) || 
                           listaInventarioCompleto.value.find(m => m.id === mpId);
                
                const idDuenioReal = mp ? Number(mp.clienteId || mp.ClienteId || 0) : Number(r.clienteId || 0);
                const destino = r.extrusoraDestino || r.ExtrusoraDestino || 'UNICA';

                return {
                    id: r.id || Math.random(),
                    materiaPrimaId: mpId,
                    nombreInsumo: r.nombreInsumo || r.nombreMateriaPrima || mp?.nombre,
                    cantidad: Number(r.cantidad || r.porcentaje || 0).toFixed(2),
                    densidad: r.densidad || r.pesoEspecifico || 1.1,
                    esBase: r.esBase || false,
                    clienteId: idDuenioReal,
                    extrusoraDestino: destino
                };
            });
            
            if (typeof balancearBase === 'function') balancearBase();
        }

        if (!form.value.largo || form.value.largo === 0) {
            form.value.esBobina = (prod.nombre || '').toUpperCase().includes('BOBINA');
            form.value.largo = form.value.esBobina ? 0 : Number(prod.largo || prod.Largo || 0);
        }
        
        if (!form.value.ancho || form.value.ancho === 0) {
            form.value.ancho = Number(prod.ancho || prod.Ancho || 0);
        }
        
        if (!form.value.espesor || form.value.espesor === 0) {
            form.value.espesor = Number(prod.espesor || prod.Espesor || 0);
        }

        limiteMinimo.value = Number(prod.espesorMinimo || prod.EspesorMinimo || 0);
        limiteMaximo.value = Number(prod.espesorMaximo || prod.EspesorMaximo || 0);

    } catch (e) { 
        console.error("Error cargando datos maestros:", e); 
    }
}

watch(mostrarCajaColor, (v) => {
    if (!v) form.value.masterbatchId = '';
});

watch(() => form.value.porcentajeTolvaA, (newVal) => {
    if (!usaCoextrusionCalculado.value || imprimiendoHistorial.value) return;
    const a = Number(newVal) || 0;
    const c = Number(form.value.porcentajeTolvaC) || 0;
    form.value.porcentajeTolvaB = Math.max(0, 100 - a - c);
});

watch(() => form.value.porcentajeTolvaC, (newVal) => {
    if (!usaCoextrusionCalculado.value || imprimiendoHistorial.value) return;
    const c = Number(newVal) || 0;
    const a = Number(form.value.porcentajeTolvaA) || 0;
    form.value.porcentajeTolvaB = Math.max(0, 100 - a - c);
});

const cargarLotesFazonSeguro = async () => {
    if (!form.value.clienteId || !form.value.productoTerminadoId) {
        listaLotesCliente.value = [];
        loteFazonSeleccionadoId.value = '';
        return;
    }
    const prodFinal = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
    if (prodFinal) {
        await actualizarRecetaFazonConCliente(Number(form.value.clienteId), prodFinal);
    }
};

watch(() => form.value.clienteId, async (nuevoCli) => {
    if (nuevoCli) {
        await CargarProductosFiltrados(nuevoCli);
        await cargarLotesFazonSeguro();
    } else {
        listaLotesCliente.value = [];
        loteFazonSeleccionadoId.value = '';
    }
});

watch(() => form.value.productoTerminadoId, async (nuevoProdId) => {
    if (form.value.esConsolidado) return;
    
    form.value.merma = 8;
    tipoSalidaVisual.value = 'NORMAL';
    
    if (nuevoProdId && !imprimiendoHistorial.value) {
        await CargarDatosProductos(Number(nuevoProdId)); 
        await nextTick();
        if (listaInventarioCompleto.value && listaInventarioCompleto.value.length > 0) {
            await cargarLotesFazonSeguro();
        }
    } else if (!nuevoProdId) {
        recetaDinamica.value = [];
        listaLotesCliente.value = [];
        loteFazonSeleccionadoId.value = '';
    }
});

watch(() => listaInventarioCompleto.value?.length, (nuevoLargo) => {
    if (nuevoLargo && nuevoLargo > 0 && form.value.productoTerminadoId && form.value.clienteId) {
        cargarLotesFazonSeguro();
    }
});

// 🚀 ESCUDO CONTRA EL BUCLE INFINITO
watch(
    [
        () => form.value.masterbatchId, () => form.value.aditivoCarga, 
        () => form.value.porcBrillo, 
        () => form.value.aditivoUV, () => form.value.porcentajeUv, 
        () => form.value.aditivoCaucho, () => form.value.porcentajeCaucho,
        () => form.value.conBrillo, () => form.value.tipoBrillo
    ],
    () => {
        if (imprimiendoHistorial.value || mutandoParaGuardar.value) return;
        recalcularFormulaAutomatica();
    }
);

watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { if (!v) form.value.llevaFilm = false; });

watch(
    [
        () => form.value.ancho, 
        () => form.value.espesor, 
        () => form.value.kilosPorBobina,
        () => densidadPT.value,
        () => form.value.esBobina
    ], 
    () => {
        if (form.value.esBobina && form.value.ancho > 0 && form.value.espesor > 0 && densidadPT.value > 0 && form.value.kilosPorBobina > 0) {
            const largoDespejado = (form.value.kilosPorBobina * 1000000) / (form.value.ancho * form.value.espesor * densidadPT.value);
            form.value.largo = Math.round(largoDespejado);
        }
    }, 
    { immediate: true }
);

watch(
    [
        () => form.value.largo, 
        () => form.value.ancho, 
        () => form.value.espesor, 
        () => form.value.cantidad, 
        () => form.value.esBobina, 
        () => form.value.kilosPorBobina
    ], 
    () => {
        if (!form.value.esConsolidado && !imprimiendoHistorial.value) {
            if (form.value.esBobina) {
                form.value.kilosTotales = Number((form.value.kilosPorBobina * form.value.cantidad).toFixed(2));
            } else {
                if (form.value.largo > 0 && form.value.ancho > 0 && form.value.espesor > 0) {
                    const mm3 = form.value.largo * form.value.ancho * form.value.espesor;
                    const pesoUnaPiezaKg = (mm3 * densidadPT.value) / 1000000;
                    form.value.kilosTotales = Number((pesoUnaPiezaKg * form.value.cantidad).toFixed(2));
                } else {
                    form.value.kilosTotales = 0;
                }
            }
        }
    }, 
    { immediate: true }
);

watch(
    [() => form.value.kilosTotales, () => form.value.clienteId], 
    ([kilos]) => {
        const cliente = clienteSeleccionado.value;
        const limiteVal = cliente?.limiteKilosPallet || cliente?.LimiteKilosPallet;
        const limite = (limiteVal && Number(limiteVal) > 0) ? Number(limiteVal) : 1000;
                        
        if (Number(kilos) > limite) {
            cantidadPalletsUsuario.value = Math.ceil(Number(kilos) / limite);
        } else {
            cantidadPalletsUsuario.value = 1;
        }
    },
    { immediate: true }
);

watch(imprimiendoHistorial, (estaImprimiendo) => {
    if (!estaImprimiendo && recetaDinamica.value.length > 0 && !form.value.esConsolidado) {
        balancearBase();
    }
});

onMounted(async () => {
    try {
        loading.value = true;
        
        const [resProd, resCli, resInv] = await Promise.all([
            ProduccionAPI.obtenerProductos(),
            ProduccionAPI.obtenerClientes(),
            ProduccionAPI.obtenerInventarioCompleto()
        ]);
        
        if (Array.isArray(resProd)) {
            productos.value = resProd.sort((a, b) => (a.nombre || '').localeCompare(b.nombre || ''));
            listaTodasMateriasPrimas.value = productos.value.filter(p => p.esMateriaPrima);
        }
        if (Array.isArray(resCli)) {
            clientes.value = resCli.sort((a, b) => (a.razonSocial || '').localeCompare(b.razonSocial || ''));
        }
        if (Array.isArray(resInv)) {
            listaInventarioCompleto.value = resInv;
        }

        revisarBorrador();
    } catch (e) {
        console.error("Error inicializando producción:", e);
    } finally {
        loading.value = false;
    }

    await cargarNotaPedidoSugerida();
});

const procesarGuardado = async () => {
    if (guardando.value) return; 
    error.value = ''; 
    mostrarOpcionMismoPedido.value = false; 
    
    mutandoParaGuardar.value = true;

    const copiaProfundaOriginal = JSON.parse(JSON.stringify(recetaDinamica.value));
    
    const usaCoextrusion = usaCoextrusionCalculado.value;
    const tolvaCapa = usaCoextrusion ? 'A' : 'UNICA';
    const tolvaMasa = usaCoextrusion ? 'B' : 'UNICA';

    if (tipoSalidaVisual.value === 'NATURAL') {
        let porcentajeRemovido = 0;
        const listaLimpia: any[] = [];

        recetaDinamica.value.forEach((item: any) => {
            const n = (item.nombreInsumo || item.nombreMateriaPrima || '').toUpperCase();
            const esColor = item.esColor || n.includes('MB') || n.includes('MASTER') || n.includes('COLOR');

            if (esColor) {
                porcentajeRemovido += parseFloat(item.cantidad || 0);
            } else {
                listaLimpia.push(item);
            }
        });

        if (listaLimpia.length > 0 && porcentajeRemovido > 0) {
            listaLimpia.sort((a: any, b: any) => (parseFloat(b.cantidad) || 0) - (parseFloat(a.cantidad) || 0));
            const materialPrincipal = listaLimpia.find((i: any) => i.esBase) || listaLimpia[0];

            if (materialPrincipal) {
                materialPrincipal.cantidad = (parseFloat(materialPrincipal.cantidad || 0) + porcentajeRemovido).toFixed(2);
            }
        }
        recetaDinamica.value = listaLimpia;
        form.value.masterbatchId = '';
        form.value.colorTexto = '';
    }

    if (usaCoextrusion) {
        recetaDinamica.value.forEach(item => {
            const destino = item.extrusoraDestino || 'UNICA';
            let factor = 1;
            if (destino === 'A') factor = form.value.porcentajeTolvaA / 100;
            if (destino === 'B') factor = form.value.porcentajeTolvaB / 100;
            if (destino === 'C') factor = form.value.porcentajeTolvaC / 100;
            
            if (item.cantidad !== undefined) {
                item.cantidad = (Number(item.cantidad) * factor).toFixed(2);
            }
        });
    }

    const est = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('ESTEARATO'));
    if (est && !recetaDinamica.value.some(r => r.materiaPrimaId === est.id)) {
        recetaDinamica.value.push({
            id: 0,
            materiaPrimaId: est.id,
            nombreInsumo: est.nombre,
            cantidad: Number((kilosEstearato.value / (form.value.kilosTotales > 0 ? form.value.kilosTotales : 1) * 100).toFixed(2)), 
            densidad: est.pesoEspecifico || 1,
            esEstearato: true,
            extrusoraDestino: tolvaMasa
        });
    }

    if (form.value.conBrillo && form.value.porcBrillo > 0) {
        const keywordBrillo = form.value.tipoBrillo === '555' ? '555' : '777';
        let mpBrillo = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes(`BRILLO ${keywordBrillo}`)) 
                    || listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('BRILLO'));
        
        if (mpBrillo) {
            const factor = tolvaCapa === 'A' ? (form.value.porcentajeTolvaA / 100) : 1;
            const cantidadGlobal = (Number(form.value.porcBrillo) * factor).toFixed(2);
            recetaDinamica.value.push({
                id: 0,
                materiaPrimaId: mpBrillo.id,
                nombreInsumo: mpBrillo.nombre,
                cantidad: cantidadGlobal,
                densidad: mpBrillo.pesoEspecifico || 1,
                extrusoraDestino: tolvaCapa
            });
        }
    }

    if (form.value.aditivoUV && form.value.porcentajeUv > 0) {
        const mpUV = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('UV'));
        if (mpUV) {
            const factor = tolvaCapa === 'A' ? (form.value.porcentajeTolvaA / 100) : 1;
            const cantidadGlobal = (Number(form.value.porcentajeUv) * factor).toFixed(2);
            recetaDinamica.value.push({
                id: 0, 
                materiaPrimaId: mpUV.id, 
                nombreInsumo: mpUV.nombre,
                cantidad: cantidadGlobal,
                densidad: mpUV.pesoEspecifico || 1,
                extrusoraDestino: tolvaCapa
            });
        }
    }

    if (form.value.aditivoCaucho && form.value.porcentajeCaucho > 0) {
        const mpCaucho = listaTodasMateriasPrimas.value.find(mp => (mp.nombre || '').toUpperCase().includes('CAUCHO'));
        if (mpCaucho) {
            const factor = tolvaMasa === 'B' ? (form.value.porcentajeTolvaB / 100) : 1;
            const cantidadGlobal = (Number(form.value.porcentajeCaucho) * factor).toFixed(2);
            recetaDinamica.value.push({
                id: 0, 
                materiaPrimaId: mpCaucho.id, 
                nombreInsumo: mpCaucho.nombre,
                cantidad: cantidadGlobal,
                densidad: mpCaucho.pesoEspecifico || 1,
                extrusoraDestino: tolvaMasa
            });
        }
    }

    ultimoPedidoGuardado.value = {
        clienteId: form.value.clienteId,
        numeroPedidoCliente: form.value.numeroPedidoCliente,
        notaPedido: form.value.notaPedido
    };

    await registrarProduccion();
    
    recetaDinamica.value = copiaProfundaOriginal;
    mutandoParaGuardar.value = false;

    if (!error.value) {
        mostrarOpcionMismoPedido.value = true;
    }
};

const continuarMismoPedido = () => {
    form.value.clienteId = ultimoPedidoGuardado.value.clienteId;
    form.value.numeroPedidoCliente = ultimoPedidoGuardado.value.numeroPedidoCliente;
    form.value.notaPedido = ultimoPedidoGuardado.value.notaPedido;
    
    mostrarOpcionMismoPedido.value = false;
    mensaje.value = "Datos del pedido recuperados. Seleccione el siguiente producto.";
};

defineExpose({ form, error, mensaje, registrarProduccion, recetaDinamica });
</script>

<template>
  <div class="contenedor-principal-produccion">
    
    <div class="bloque-superior">
        <div class="panel-izquierdo">
            <div class="hoja-contenedor" :style="{ opacity: imprimiendoHistorial ? '0.01' : '1', pointerEvents: imprimiendoHistorial ? 'none' : 'auto', transition: 'opacity 0.2s' }">
                <HojaImpresion 
                    id="hoja-de-impresion"
                    :form="form" 
                    :producto="productoSeleccionado" 
                    :cliente="clienteSeleccionado" 
                    :receta="recetaConExtrasParaVista" 
                    :colorFinal="colorFinalParaPDF" 
                    :densidad="densidadPT" 
                    :totalPorcentaje="100" 
                    :materiasPrimas="listaTodasMateriasPrimas" 
                    :ocultarFormula="ocultarFormula" 
                    :tipoSalidaVisual="tipoSalidaVisual"
                    @add-insumo="agregarInsumoDesdeHijo" 
                    @remove-insumo="quitarInsumoManual" 
                    @update-receta="balancearBase"  
                />
            </div>
        </div>

        <div class="panel-derecho">
            <div class="header-control">
                <h3>⚙️ Configuración</h3>
            </div>
            
            <div v-if="borradorDisponible" class="banner-borrador">
                <div class="banner-texto">
                    <span>📝 <strong>Tenés una orden sin terminar.</strong></span>
                    <small>Se guardó automáticamente la última vez.</small>
                </div>
                <div class="banner-acciones">
                    <button @click="recuperarBorrador" class="btn-borrador-ok">Recuperar</button>
                    <button @click="descartarBorrador" class="btn-borrador-no">Descartar</button>
                </div>
            </div>
            
            <label>Cliente</label>
            <select v-model="form.clienteId" style="margin-bottom:5px">
                <option disabled value="">Seleccione un cliente...</option>
                <option :value="1">ESTRUPLAST</option>
                <option v-for="c in clientes" :key="c.id" :value="c.id">
                    {{c.razonSocial}} {{ c.esFazon ? '' : '(Venta)' }}
                </option>
            </select>

            <label style="color:#f39c12;">📂 N° Pedido Cliente (OC):</label>
            <input type="text" v-model="form.numeroPedidoCliente" placeholder="Ej: OC-4455" style="font-weight:bold; border: 1px solid #f39c12; margin-bottom: 5px;" />

            <label style="color:#1abc9c;">🧾 Nota de Pedido (Flexxus):</label>
            <div class="fila-input" style="margin-bottom: 5px;">
                <input
                    type="text"
                    v-model="form.notaPedido"
                    placeholder="Ej: 12345"
                    style="font-weight:bold; border: 1px solid #1abc9c;"
                />
                <button
                    type="button"
                    class="btn-sugerido"
                    :disabled="!notaPedidoSugerida"
                    @click="aplicarNotaPedidoSugerida"
                    :title="notaPedidoSugerida ? `Copiar número anterior: ${notaPedidoSugerida}` : 'Sin sugerencia'"
                >
                    Usar: {{ notaPedidoSugerida || '-' }}
                </button>
            </div>
            
            <select v-model="form.productoTerminadoId">
                <option disabled value="">Seleccionar Producto...</option>
                <option v-for="p in productosDropdownOrdenados" :key="p.id" :value="p.id">
                    {{ p.esFazon ? '★ ' : '' }}{{ p.nombre }} {{ p.esGenerico ? '(A Medida)' : (p.esFazon ? '(Fazón)' : '(Estándar)') }}
                </option>
            </select>

            <!-- 🚀 REPARTICIÓN AUTOMÁTICA DE PORCENTAJES A, B y C -->
            <div v-if="usaCoextrusionCalculado" class="fila-input" style="margin-top: 15px; border-top: 1px dashed #7f8c8d; padding-top: 10px; background: #e8f4f8; padding: 10px; border-radius: 6px; border: 1px solid #bde0fe; flex-wrap: wrap;">
                <div style="flex:1; min-width: 80px;" v-if="porcentajesPorTolva.A > 0 || form.porcentajeTolvaA > 0">
                    <label style="color:#2980b9; font-weight: bold; margin-top: 0;">🟦 Capa A (%)</label>
                    <input type="number" v-model="form.porcentajeTolvaA" min="1" max="99" @wheel.prevent style="font-weight:bold; color:#2980b9; border: 1px solid #3498db;" />
                </div>
                <div style="flex:1; min-width: 80px;" v-if="porcentajesPorTolva.B > 0 || form.porcentajeTolvaB > 0">
                    <label style="color:#27ae60; font-weight: bold; margin-top: 0;">🟩 Masa B (%)</label>
                    <input type="number" v-model="form.porcentajeTolvaB" disabled style="font-weight:bold; background:#eafaf1; color:#27ae60; border: 1px solid #2ecc71;" />
                </div>
                <div style="flex:1; min-width: 80px;" v-if="porcentajesPorTolva.C > 0 || form.porcentajeTolvaC > 0">
                    <label style="color:#d97706; font-weight: bold; margin-top: 0;">🟨 Capa C (%)</label>
                    <input type="number" v-model="form.porcentajeTolvaC" min="1" max="99" @wheel.prevent style="font-weight:bold; color:#d97706; border: 1px solid #f59e0b;" />
                </div>
                
                <div v-if="usaCoextrusionCalculado && (form.porcentajeTolvaA + form.porcentajeTolvaB + form.porcentajeTolvaC) !== 100" class="text-error" style="width: 100%; font-size: 11px; margin-top: 4px;">
                    ⚠️ Las tolvas suman {{ form.porcentajeTolvaA + form.porcentajeTolvaB + form.porcentajeTolvaC }}%. Deben sumar 100%.
                </div>
            </div>

            <div v-if="form.productoTerminadoId" class="caja-detalles-producto">
                
                <div v-if="productoSeleccionado && !productoSeleccionado.esFazon && !(productoSeleccionado.nombre || '').toUpperCase().includes('COLOR')" class="fila-input" style="margin-bottom: 15px; display: flex; gap: 10px;">
                    <button 
                        @click="tipoSalidaVisual = 'NORMAL'" 
                        type="button"
                        style="flex: 1; padding: 10px; border-radius: 6px; font-weight: 900; font-size: 14px; cursor: pointer; border: 2px solid #3498db; transition: all 0.2s;"
                        :style="tipoSalidaVisual === 'NORMAL' ? 'background: #3498db; color: white; box-shadow: 0 4px 10px rgba(52, 152, 219, 0.3);' : 'background: transparent; color: #3498db;'"
                    >
                        ESTÁNDAR
                    </button>
                    <button 
                        @click="tipoSalidaVisual = 'NATURAL'" 
                        type="button"
                        style="flex: 1; padding: 10px; border-radius: 6px; font-weight: 900; font-size: 14px; cursor: pointer; border: 2px solid #2ecc71; transition: all 0.2s;"
                        :style="tipoSalidaVisual === 'NATURAL' ? 'background: #2ecc71; color: white; box-shadow: 0 4px 10px rgba(46, 204, 113, 0.3);' : 'background: transparent; color: #2ecc71;'"
                    >NATURAL
                    </button>
                </div>

                <div v-if="listaLotesCliente.length > 0" class="box-fazon-selector">
                    <label style="color: #2ecc71;">♻️ Lote Recuperado (Fazón):</label>
                    <select v-model="loteFazonSeleccionadoId" @change="alCambiarLoteFazon" class="select-fazon">
                        <option disabled value="">Seleccionar Lote</option>
                        <option v-for="lote in listaLotesCliente" :key="lote.id" :value="lote.id">
                            {{ lote.nombre }} (Stock: {{ lote.stockActual }} kg)
                        </option>
                    </select>
                </div>

                <div v-if="mostrarCajaColor" class="box-color">
                    <label style="color: #f39c12;">🎨 Seleccione Color:</label>
                    <select v-model="form.masterbatchId">
                        <option disabled value="">-- Elegir Masterbatch --</option>
                        <option v-for="mb in listaMasterbatches" :key="mb.id" :value="mb.id">{{ mb.nombre }}</option>
                    </select>
                </div>

                <div class="fila-input" style="margin-top: 5px;">
                    <div style="flex:1">
                        <label style="color:#3498db;">✏️ Texto Color (Opcional):</label>
                        <input 
                            type="text" 
                            v-model="form.colorTexto" 
                            placeholder="Ej: AZUL PANTONE..."
                            style="font-weight:bold; color:#2980b9;"
                        />
                    </div>
                </div>

                <div style="display: flex; justify-content: space-between; align-items: flex-end; margin-top: 15px; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-bottom: 5px;">
                    <label class="lbl-sep" style="border: none; margin: 0 !important; padding: 0;">
                        Medidas: <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span><span v-else style="color:#2ecc71">(EDITABLES)</span>
                    </label>
                    <label class="check-container" style="margin: 0 !important; color: #3498db;">
                        <input type="checkbox" v-model="form.esBobina" /> 🗞️ Formato Bobina
                    </label>
                </div>
                
                <div style="font-size:11px; color:#bbb; margin-top:-5px; margin-bottom:5px;">
                    <span v-if="limiteMaximo > 0">Rango: {{ limiteMinimo }} - {{ limiteMaximo }} mm</span>
                    <span v-else-if="limiteMinimo > 0">Mínimo: {{ limiteMinimo }} mm (Sin tope)</span>
                </div>

                <div class="fila-input">
                    <div v-if="!form.esBobina">
                        <label>Largo (mm)</label>
                        <input type="number" v-model="form.largo" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}" @wheel.prevent />
                    </div>
                    
                    <div v-else>
                        <label style="color:#f39c12; margin-bottom: 2px;">Kilos x Bobina</label>
                        <input type="number" v-model="form.kilosPorBobina" step="0.1" style="border: 2px solid #f39c12; font-weight: bold; background: #fff3e0; color: #d35400; margin-bottom: 2px;" @wheel.prevent />
                        <small style="color: #7f8c8d; font-size: 10px; font-weight: bold; display: block;">Largo calculado: {{ form.largo }} mm</small>
                    </div>
                    
                    <div>
                        <label>Ancho (mm)</label>
                        <input type="number" v-model="form.ancho" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}" @wheel.prevent />
                    </div>
                </div>
                
                <div class="fila-input">
                    <div :class="{ 'error-espesor': !espesorValido }">
                        <label>Espesor (mm)</label>
                        <input type="number" v-model="form.espesor" step="0.01" 
                            :disabled="medidasBloqueadas" 
                            :class="{'input-lock': medidasBloqueadas, 'input-error': !espesorValido}" 
                            style="font-weight:bold;" @wheel.prevent />
                        
                        <span v-if="!espesorValido" style="color: #e74c3c; font-size: 11px; font-weight: bold; display: block; margin-top: 4px;">
                            <span v-if="limiteMaximo > 0">⚠️ Rango permitido: {{ limiteMinimo }} a {{ limiteMaximo }} mm</span>
                            <span v-else>⚠️ Espesor mínimo permitido: {{ limiteMinimo }} mm</span>
                        </span>
                    </div>
                    <div>
                        <label>Cantidad</label>
                        <input type="number" v-model="form.cantidad" min="1" @wheel.prevent />
                    </div>
                </div>
                
                <div class="fila-input" style="margin-top:10px; border-top:1px dashed #7f8c8d; padding-top:10px; display: flex; gap: 10px;">
                    <div style="flex:1">
                        <label style="color:#e67e22;">🔥 Desperdicio Fijo</label>
                        <div style="padding: 8px; background: #fdf2e9; border: 1px solid #e67e22; border-radius: 4px; color:#e67e22; font-weight:bold; text-align: center;">
                            8 %
                        </div>
                    </div>
                    <div style="flex:1">
                        <label style="color:#2980b9;">Estearato</label>
                        <div style="padding: 8px; background: #ebf5fb; border: 1px solid #3498db; border-radius: 4px; color:#2980b9; font-weight:bold; text-align: center;">
                            {{ Number(kilosEstearato).toFixed(3) }} Kg
                        </div>
                    </div>
                </div>

                <div class="resumen-peso">
                    Peso Final: {{ form.kilosTotales }} Kg <small style="color:#bbb; display:block;">(Consumo Real MP +8%)</small>
                    
                    <div v-if="cantidadPalletsUsuario > 1" style="color:#3498db; margin-top: 8px; font-weight: bold; background: #ebf5fb; padding: 5px; border-radius: 4px; font-size: 12px; border-left: 3px solid #3498db; text-align: left;">
                        📦 Se dividirá en {{ cantidadPalletsUsuario }} pallets internos <br>
                        <span style="color:#7f8c8d; font-size: 10px;">(Límite de este cliente: {{ clienteSeleccionado?.limiteKilosPallet || clienteSeleccionado?.LimiteKilosPallet || 1000 }} kg)</span>
                    </div>
                </div>
                
                <label class="lbl-sep">Aditivos:</label>
                
                <div class="fila-control-aditivo" style="align-items: flex-start;">
                    <label class="check-container" :class="{ 'disabled': form.espesor < 1 }" style="margin-top: 5px !important;">
                        <input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1" /> ✨ Brillo
                    </label>
                    <div v-if="form.conBrillo" class="bloque-derecha-brillo">
                        <select v-model="form.tipoBrillo" class="select-brillo">
                            <option value="777">Brillo 777</option>
                            <option value="555">Brillo 555 (Cristal)</option>
                        </select>
                        <div class="input-porcentaje">
                            <input type="number" v-model="form.porcBrillo" step="0.01" min="0" @wheel.prevent /> %
                        </div>
                    </div>
                </div>

                <div class="fila-control-aditivo">
                    <label class="check-container" :class="{ 'disabled': !form.conBrillo }">
                        <input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo" /> 🛡️ Con Film
                    </label>
                </div>
                
                <div class="fila-control-aditivo">
                    <label class="check-container">
                        <input type="checkbox" v-model="form.esGofrado" /> 🧇 Gofrado
                    </label>
                </div>

                <div class="fila-control-aditivo">
                    <label class="check-container">
                        <input type="checkbox" v-model="form.aditivoUV" /> ☀️ UV
                    </label>
                    <div v-if="form.aditivoUV" class="bloque-derecha">
                        <div class="input-porcentaje">
                            <input type="number" v-model="form.porcentajeUv" step="0.01" min="0" @wheel.prevent /> %
                        </div>
                    </div>
                </div>
                
                <div class="fila-control-aditivo">
                    <label class="check-container">
                        <input type="checkbox" v-model="form.aditivoCaucho" /> 🚜 Caucho
                    </label>
                    <div v-if="form.aditivoCaucho" class="bloque-derecha">
                        <div class="input-porcentaje">
                            <input type="number" v-model="form.porcentajeCaucho" step="0.01" min="0" @wheel.prevent /> %
                        </div>
                    </div>
                </div>

                <div style="margin-top: 5px; margin-bottom: 15px;">
                    <div class="fila-control-aditivo">
                        <label class="check-container">
                            <input type="checkbox" v-model="uiLlevaImpresion" /> 🖨️ Impresión
                        </label>
                    </div>
                    
                    <div v-if="uiLlevaImpresion" style="margin-top: 4px; margin-left: 28px; padding: 10px 15px; background: #eff6ff; border-radius: 8px; border: 1px solid #bfdbfe; border-left: 4px solid #3b82f6; display: flex; justify-content: space-between; align-items: center; box-shadow: 0 2px 4px rgba(59, 130, 246, 0.05);">
                        <div style="display: flex; flex-direction: column; gap: 2px;">
                            <span style="color: #1e40af; font-weight: 800; font-size: 0.85rem;">🪨 Carga Mineral</span>
                            <span style="color: #60a5fa; font-size: 0.75rem; font-weight: 600;">Proporción para impresión</span>
                        </div>
                        
                        <div style="display: flex; align-items: center; background: white; border: 1px solid #93c5fd; border-radius: 6px; padding: 2px 10px;">
                            <input 
                                type="number" 
                                v-model="form.aditivoCarga" 
                                @wheel.prevent 
                                placeholder="0"
                                style="width: 45px; border: none; outline: none; font-weight: 900; color: #1e293b; text-align: right; background: transparent; padding: 4px; font-size: 1rem;"
                            />
                            <span style="color: #3b82f6; font-weight: 900; margin-left: 5px; font-size: 0.9rem;">%</span>
                        </div>
                    </div>
                </div>

                <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
                <select v-model="form.tipoCorona">
                    <option value="Ninguno">Sin Tratamiento</option>
                    <option value="Simple">Simple</option>
                    <option value="Doble">Doble</option>
                </select>
                
                <template v-if="!uiLlevaImpresion">
                    <label class="lbl-sep">Cargas:</label>
                    <div class="fila-input">
                        <div style="flex:1">
                            <label>Carga Mineral (%)</label>
                            <input type="number" v-model="form.aditivoCarga" @wheel.prevent />
                        </div>
                    </div>
                </template>

            </div>
            
            <div class="fila-input" style="margin-top:10px">
                <div style="width: 100%">
                    <label>Obs:</label>
                    <input type="text" v-model="form.observacion" style="width:100%" />
                </div>
            </div>
            
            <div v-if="errorBasePorcentaje && recetaDinamica.length > 0" class="alerta-error">
                ⚠️ {{ errorBasePorcentaje }}
            </div>
            
            <div v-if="hayBloqueoDeStock" class="alerta-stock-warning">
                <h4>⚠️ Material Insuficiente (Stock Libre Negativo)</h4>
                <p style="margin: 0 0 5px 0; font-size: 11px;">La orden nacerá como <strong>Pendiente</strong>, pero requerirá compras para producirse:</p>
                <ul>
                    <li v-for="(falla, i) in insumosSinStock" :key="i">
                        <strong>{{ falla.nombre }}</strong>: Faltan {{ falla.diferencia.toFixed(2) }} kg (Disp: {{ falla.disponible }})
                    </li>
                </ul>
            </div>

            <button 
                class="btn-guardar" 
                @click="procesarGuardado" 
                :disabled="guardando || loading || form.clienteId === '' || !form.productoTerminadoId || !espesorValido || !!errorBasePorcentaje || (usaCoextrusionCalculado && (form.porcentajeTolvaA + form.porcentajeTolvaB + form.porcentajeTolvaC) !== 100)" 
                :class="{ 'btn-warning': hayBloqueoDeStock && form.clienteId !== '' && form.productoTerminadoId && espesorValido }"
            >
                <span v-if="guardando">⏳ GUARDANDO ORDEN...</span>
                <span v-else-if="loading">⏳ PROCESANDO...</span>
                <span v-else-if="form.clienteId === '' || !form.productoTerminadoId">🚫 SELECCIONE CLIENTE Y PRODUCTO</span>
                <span v-else-if="!espesorValido">🚫 ERROR: ESPESOR FUERA DE RANGO</span>
                <span v-else-if="!!errorBasePorcentaje">🚫 ERROR EN PORCENTAJES DE MEZCLA</span>
                <span v-else-if="usaCoextrusionCalculado && (form.porcentajeTolvaA + form.porcentajeTolvaB + form.porcentajeTolvaC) !== 100">🚫 LAS TOLVAS DEBEN SUMAR 100%</span>
                <span v-else-if="hayBloqueoDeStock">💾 GUARDAR PENDIENTE (FALTA STOCK)</span>
                <span v-else>💾 GUARDAR ORDEN LISTA</span>
            </button>
            
            <div v-if="mostrarOpcionMismoPedido" class="banner-borrador" style="border-left-color: #2ecc71; margin-top: 15px;">
                <div class="banner-texto">
                    <span style="color: #2ecc71;">✅ <strong>Orden Guardada</strong></span>
                    <small style="color: #ecf0f1;">¿Deseas seguir cargando ítems en este pedido?</small>
                </div>
                <div class="banner-acciones" style="margin-top: 8px;">
                    <button @click="continuarMismoPedido" class="btn-borrador-ok" style="background-color: #3498db; width: 100%;">Sí, mantener cliente y OC</button>
                    <button @click="mostrarOpcionMismoPedido = false" class="btn-borrador-no" style="color: #ecf0f1; width: 100%;">Terminar</button>
                </div>
            </div>

            <p v-else-if="mensaje" class="success">{{ mensaje }}</p>
            <p v-if="error" class="error">{{ error }}</p>
        </div>
    </div>

    <div class="bloque-inferior">
        <ListaProduccion 
            ref="listaProduccionRef" 
            @imprimir-historial="imprimirDesdeHistorial" 
            @imprimir-carga-consolidada="imprimirDesdeHistorial" 
            @imprimir-lote-op="imprimirLoteOPsDesdeHistorial"
        />
    </div>

  </div>
  <div style="position: absolute; left: -9999px; top: -9999px; opacity: 0; pointer-events: none;">
    <div id="impresion-fantasma">
        <HojaImpresion 
            :form="form" 
            :producto="productoSeleccionado" 
            :cliente="clienteSeleccionado" 
            :receta="recetaConExtrasParaVista" 
            :colorFinal="colorFinalParaPDF" 
            :densidad="densidadPT" 
            :totalPorcentaje="100" 
            :materiasPrimas="listaTodasMateriasPrimas" 
            :ocultarFormula="ocultarFormula" 
            :tipoSalidaVisual="tipoSalidaVisual"
        />
    </div>
  </div>
</template>

<style scoped>
/* CSS Idéntico - No se toca */
.contenedor-principal-produccion { display: flex; flex-direction: column; width: 100%; min-height: 100vh; font-family: 'Segoe UI', sans-serif; background-color: #ecf0f1; }
.bloque-superior { display: flex; width: 100%; flex-wrap: wrap; }
.panel-izquierdo { flex: 1; background-color: #e0e6ed; display: flex; justify-content: center; align-items: flex-start; padding: 20px; border-right: 1px solid #bdc3c7; overflow: hidden; min-width: 400px; }
.hoja-contenedor { background: white; width: 210mm; min-height: 297mm; box-shadow: 0 10px 25px rgba(0,0,0,0.1); transform: scale(0.80); transform-origin: top center; margin-bottom: -350px; }
.panel-derecho { width: 350px; min-width: 350px; background-color: #2c3e50; color: white; display: flex; flex-direction: column; padding: 20px; box-shadow: -5px 0 15px rgba(0,0,0,0.1); z-index: 10; border-left: 1px solid #34495e; }
.bloque-inferior { width: 100%; padding: 7px; background-color: #f8f9fa; border-top: 3px solid #bdc3c7; }
.header-control h3 { margin-top: 0; border-bottom: 2px solid #3498db; padding-bottom: 10px; color: #ecf0f1; font-size: 1.1rem; }
label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; font-weight: 600; }
select, input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 13px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
input[type=number]::-webkit-inner-spin-button, input[type=number]::-webkit-outer-spin-button { -webkit-appearance: none; margin: 0; }
input[type=number] { -moz-appearance: textfield; appearance: textfield; }
.fila-input { display: flex; gap: 8px; margin-bottom: 5px; }
.btn-sugerido { width: 130px; margin-top: 2px; border-radius: 4px; border: 1px solid #1abc9c; background: transparent; color: #1abc9c; font-weight: bold; cursor: pointer; font-size: 12px; padding: 8px; }
.btn-sugerido:disabled { opacity: 0.5; cursor: not-allowed; }
.seccion-medidas-editables { background: #34495e; padding: 12px; border-radius: 6px; margin-top: 15px; border: 1px solid #4e6475; }
.box-color { margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px; }
.lbl-sep { color: #f1c40f !important; font-weight: bold; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-top: 15px !important; margin-bottom: 5px; }
.resumen-peso { font-weight: bold; color: #2ecc71; text-align: right; margin-top: 10px; font-size: 14px; border-top: 1px solid #7f8c8d; padding-top: 5px; }
.check-container { display: flex; align-items: center; cursor: pointer; color: #ecf0f1; font-weight: bold; font-size: 13px; margin-top: 8px !important; }
.check-container input { width: auto; margin-right: 8px; }
.check-container.disabled { opacity: 0.5; cursor: not-allowed; }
.alerta-error { background: #c0392b; color: white; padding: 10px; border-radius: 5px; margin-top: 15px; font-weight: bold; text-align: center; font-size: 12px; }
.alerta-stock-warning { background-color: #fff9e6; border: 1px solid #f1c40f; color: #d35400; padding: 10px; border-radius: 6px; margin-top: 15px; font-size: 12px; text-align: left; }
.alerta-stock-warning h4 { margin: 0 0 5px 0; color: #e67e22; font-size: 13px; }
.alerta-stock-warning ul { margin: 0; padding-left: 20px; }
.btn-guardar { background: #27ae60; color: white; margin-top: 20px; border: none; padding: 12px; border-radius: 6px; cursor: pointer; font-size: 1em; font-weight: bold; width: 100%; transition: background 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #7f8c8d; cursor: not-allowed; opacity: 0.7; }
.btn-warning { background: #f39c12 !important; color: white !important; }
.btn-warning:hover { background: #e67e22 !important; }
.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.fila-control-aditivo { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; }
.input-porcentaje { display: flex; align-items: center; background: #ecf0f1; border-radius: 4px; padding-right: 5px; color: #333; }
.input-porcentaje input { width: 45px !important; margin: 0 !important; text-align: right; background: transparent; color: #333; }
.bloque-derecha-brillo { display: flex; flex-direction: column; align-items: flex-end; gap: 4px; }
.select-brillo { width: 130px; padding: 4px; font-size: 11px; margin: 0; background: #ecf0f1; border-radius: 4px; border: none; color: #2c3e50; font-weight: bold; }
.bloque-derecha { display: flex; flex-direction: column; align-items: flex-end; }
.input-lock { background-color: #4a5d6e !important; color: #bdc3c7 !important; cursor: not-allowed; border: 1px solid #3e4f5e !important; }
.input-error { border: 2px solid #e74c3c !important; background-color: #fab1a0 !important; color: #c0392b !important; }
.grupo-botones-pdf { display: flex; gap: 5px; margin-top: 10px; }
.btn-imprimir { flex: 1; padding: 8px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; font-size: 12px; color: white; }
.btn-orden { background: #34495e; border: 1px solid #7f8c8d; } .btn-orden:hover { background: #2980b9; }
.btn-carga { background: #8e44ad; border: 1px solid #9b59b6; } .btn-carga:hover { background: #9b59b6; }
.box-fazon-selector { background-color: #27ae60; padding: 10px; border-radius: 6px; margin-top: 10px; border: 1px solid #2ecc71; }
.box-fazon-selector label { color: white !important; }
.select-fazon { background-color: white; font-weight: bold; color: #2c3e50; border: 2px solid #2ecc71; }
.caja-detalles-producto { background-color: #34495e; padding: 15px; border-radius: 8px; margin-top: 15px; border: 1px solid #4a6278; box-shadow: inset 0 2px 4px rgba(0,0,0,0.1); }
.alerta-pallets { background-color: #fff3cd; padding: 15px; border-radius: 8px; margin-top: 15px; color: #856404; border: 1px solid #ffeeba; }
.banner-borrador { background-color: #34495e; border-left: 4px solid #f1c40f; padding: 12px; border-radius: 6px; margin-bottom: 15px; display: flex; flex-direction: column; gap: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.banner-texto span { color: #f1c40f; font-size: 13px; display: block; }
.banner-texto small { color: #bdc3c7; font-size: 11px; }
.banner-acciones { display: flex; gap: 8px; }
.btn-borrador-ok { flex: 1; background: #27ae60; color: white; border: none; padding: 6px; border-radius: 4px; font-weight: bold; cursor: pointer; font-size: 11px; }
.btn-borrador-ok:hover { background: #2ecc71; }
.btn-borrador-no { flex: 1; background: transparent; color: #bdc3c7; border: 1px solid #7f8c8d; padding: 6px; border-radius: 4px; font-weight: bold; cursor: pointer; font-size: 11px; }
.btn-borrador-no:hover { background: #95a5a6; color: white; }
@media (max-width: 1000px) { 
    .bloque-superior { flex-direction: column; } 
    .panel-izquierdo { width: 100%; border-right: none; border-bottom: 1px solid #bdc3c7; } 
    .panel-derecho { width: 100%; min-width: auto; } 
    .hoja-contenedor { transform: scale(0.55); margin-bottom: -400px; } 
}
</style>