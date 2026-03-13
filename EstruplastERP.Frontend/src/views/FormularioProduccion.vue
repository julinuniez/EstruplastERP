<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
// @ts-ignore
import html2pdf from 'html2pdf.js'
import HojaImpresion from '../components/HojaImpresion.vue'
import ListaProduccion from '../components/ListaProduccion.vue'
import api from '@/services/axiosInstance' 

const DENSIDAD_DEFAULT = 1.1;

const ID_BRILLO_777 = 1073; 
const ID_ESTEARATO = 1074; 
const ID_UV = 1075; 
const ID_CAUCHO = 1076; 
const ID_CARGA = 1077; 
const ID_MASTERBATCH_GENERICO = 90;
const PORC_ESTEARATO = 0.08; 

interface Producto {
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean;
    esGenerico: boolean; esFazon?: boolean; esMateriaPrima?: boolean; esScrap?: boolean; rubro?: string;
    largo: number; ancho: number; espesor: number; color?: string; pesoEspecifico: number;
    receta?: any[]; espesorMinimo?: number; espesorMaximo?: number; clienteId?: number;
    tipoMaterial?: string;
}
interface Cliente { id: number; razonSocial: string; esFazon?: boolean; }
interface ItemReceta {
    id: number | string; cantidad: number; nombreInsumo: string; densidad: number;
    materiaPrimaId: number; esColor?: boolean; esCarga?: boolean; esBase?: boolean;
    esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean;
    esFazonInput?: boolean; materialBase?: string;
}

const loading = ref(false);
const productos = ref<Producto[]>([])
const listaInventarioCompleto = ref<any[]>([])
const listaMasterbatches = ref<any[]>([])
const listaTodasMateriasPrimas = ref<any[]>([])

const idCristal555 = computed(() => {
    const material = listaTodasMateriasPrimas.value.find(m => 
        m.codigoSku === 'MP-CRI-555' || m.nombre === 'CRISTAL 555'
    );
    return material ? material.id : 0;
});

const clientes = ref<Cliente[]>([])
const recetaDinamica = ref<ItemReceta[]>([])
const stockFazonDetectado = ref<number | null>(null);

const listaLotesCliente = ref<any[]>([]); 
const loteFazonSeleccionadoId = ref<string | number>('');

const listaProduccionRef = ref<any>(null);

const limiteMinimo = ref(0);
const limiteMaximo = ref(0);
const mensaje = ref('');
const error = ref('');
const idProduccionGenerada = ref(false);
const ocultarFormula = ref(false);
const cantidadPalletsUsuario = ref(1);

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
    porcBrillo: 2.00, 
    llevaFilm: false, tipoCorona: 'Ninguno',
    conEstearato: false, esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '',
    aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00,
    aditivoCarga: 0,
    merma: 8, kilosTotales: 0,
    esConsolidado: false 
})

const STORAGE_KEY = 'produccion_borrador';
const STORAGE_NOTA_PEDIDO_NEXT = 'produccion_notaPedido_siguiente';

const notaPedidoSugerida = ref<string>('');

const detectarMaterial = (item: any) => {
    if (!item) return '';
    if (item.tipoMaterial && item.tipoMaterial !== 'OTROS') return item.tipoMaterial.toUpperCase();
    
    const n = (item.nombre || item).toString().toUpperCase();
    if (n.includes('PAI') || n.includes('IMPACTO') || n.includes('A.I.')) return 'PAI';
    if (n.includes('PP') || n.includes('POLIPROPILENO')) return 'PP';
    if (n.includes('PEAD') || n.includes('ALTA') || n.includes('HDPE')) return 'PEAD';
    if (n.includes('PEBD') || n.includes('BAJA') || n.includes('LDPE') || n.includes('POLIETILENO')) return 'POLIETILENO';
    if (n.includes('ABS')) return 'ABS';
    if (n.includes('FREON') || n.includes('RESISTENTE')) return 'RESISTENTE FREON';
    if (n.includes('BIO')) return 'BIO';
    return '';
};

async function cargarNotaPedidoSugerida() {
    try {
        const res = await api.get('/Ordenes/recientes');
        let maxNota = 0;

        if (Array.isArray(res.data) && res.data.length > 0) {
            const candidatos = res.data
                .map((o: any) => o?.notaPedido ?? o?.numeroPedidoCliente ?? o?.id)
                .map((v: any) => Number(v))
                .filter((n: number) => !isNaN(n) && n > 0);

            if (candidatos.length > 0) {
                maxNota = Math.max(...candidatos);
            }
        }

        notaPedidoSugerida.value = maxNota > 0 ? String(maxNota) : '';
        const correlativo = maxNota > 0 ? maxNota + 1 : 1;
        localStorage.setItem(STORAGE_NOTA_PEDIDO_NEXT, String(correlativo));

        if (!form.value.notaPedido || String(form.value.notaPedido).trim() === '') {
            form.value.notaPedido = String(correlativo);
        }
    } catch (e) {
        const nextGuardadoRaw = localStorage.getItem(STORAGE_NOTA_PEDIDO_NEXT);
        const nextGuardado = nextGuardadoRaw ? Number(nextGuardadoRaw) : NaN;
        if (!isNaN(nextGuardado) && nextGuardado > 0) {
            notaPedidoSugerida.value = String(Math.trunc(nextGuardado) - 1); 
            if (!form.value.notaPedido || String(form.value.notaPedido).trim() === '') {
                form.value.notaPedido = String(Math.trunc(nextGuardado));
            }
        } else {
            notaPedidoSugerida.value = '';
        }
    }
}

function aplicarNotaPedidoSugerida() {
    if (notaPedidoSugerida.value) form.value.notaPedido = notaPedidoSugerida.value;
}

watch(
    () => form.value.notaPedido,
    (v) => {
        const num = Number(v);
        if (!isNaN(num) && num > 0) {
            const anterior = Math.trunc(num) - 1;
            if (anterior > 0) {
                notaPedidoSugerida.value = String(anterior);
            }
            localStorage.setItem(STORAGE_NOTA_PEDIDO_NEXT, String(num));
        }
    }
);

watch(
    [form, recetaDinamica], 
    ([nuevoForm, nuevaReceta]) => {
        const borrador = {
            form: nuevoForm,
            receta: nuevaReceta,
            timestamp: Date.now()
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(borrador));
    },
    { deep: true }
);

const limpiarBorrador = () => {
    localStorage.removeItem(STORAGE_KEY);
};

const productoSeleccionado = computed(() => productos.value.find(p => p.id === Number(form.value.productoTerminadoId)) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === Number(form.value.clienteId)) || null);

const clienteTieneFazonActivo = computed(() => {
    if (!clienteSeleccionado.value) return false;
    return clienteSeleccionado.value.esFazon === true;
});

const clientesHabilitados = computed(() => {
    return clientes.value.filter(c => c.esFazon === true);
});

const medidasBloqueadas = computed(() => !productoSeleccionado.value || !productoSeleccionado.value.esGenerico);

const espesorValido = computed(() => {
    const e = Number(form.value.espesor);
    if (e <= 0) return true;
    if (limiteMinimo.value > 0 && e < limiteMinimo.value) return false;
    if (limiteMaximo.value > 0 && e > limiteMaximo.value) return false;
    return true;
});

const listaProductosDisponibles = computed(() => {
    if (!productos.value || productos.value.length === 0) return [];
    
    const idClienteSeleccionado = form.value.clienteId ? Number(form.value.clienteId) : null;

    return productos.value.filter(p => {
        const nombre = (p.nombre || '').toUpperCase();
        const rubro = (p.rubro || '').toUpperCase();
        
        if (p.esMateriaPrima || p.esScrap || rubro.includes('MOLIDO')) return false;
        if (rubro.includes('MATERIA') || rubro.includes('INSUMO') || rubro.includes('MASTERBATCH')) return false;
        if (nombre.includes('BASE') && !nombre.includes('ALTA')) return false;
        if (nombre.includes('(BASE)') || nombre.includes('(VARIOS)')) return false;
        if (nombre.includes('GENERICO') || nombre.includes('GENÉRICO')) return false;
        if (nombre.includes('MASTERBATCH') || nombre.includes('PIGMENTO') || nombre.includes('SCRAP')) return false;
        if (p.id >= 990 && p.id <= 999) return false; 

        const esProductoFazon = p.esFazon || nombre.includes('FAZON') || nombre.includes('SERVICIO');

        if (esProductoFazon) {
            if (idClienteSeleccionado && !clienteTieneFazonActivo.value) return false;
            if (!idClienteSeleccionado) return false;
            
            const esPropioDelCliente = p.clienteId && p.clienteId == idClienteSeleccionado;
            const esServicioGenerico = !p.clienteId || p.clienteId === 0;

            if ((esPropioDelCliente || esServicioGenerico) && clienteTieneFazonActivo.value) {
                return true; 
            } else {
                return false; 
            }
        }
        return true; 
    });
});

const materiasPrimasLimpias = computed(() => {
    const esFazon = productoSeleccionado.value?.esFazon || 
                    (productoSeleccionado.value?.nombre || '').toUpperCase().includes('FAZON');

    const materialesBaseAbstractos = [
        "POLIPROPILENO", "PEAD", "PEBD", "PAI", "POLIETILENO", 
        "ABS", "RESISTENTE AL FREON", "ALTO IMPACTO"
    ];

    return listaTodasMateriasPrimas.value.filter(mp => {
        const nombre = (mp.nombre || '').toUpperCase().trim();
        const rubro = (mp.rubro || '').toUpperCase();

        if (materialesBaseAbstractos.includes(nombre)) return false;
        if (mp.esGenerico) return false;

        const esMolido = mp.esScrap || rubro.includes('MOLIDO') || nombre.includes('SCRAP') || nombre.includes('RECUPERADO');

        if (!clienteTieneFazonActivo.value || !esFazon) {
            if (esMolido) return false;
            if (nombre.includes('FAZON') || mp.esFazon) return false;
        }

        return true; 
    });
});

const totalPorcentajeReceta = computed(() => parseFloat(recetaDinamica.value.reduce((acc, item) => acc + (parseFloat(item.cantidad.toString()) || 0), 0).toFixed(2)));

const densidadMezcla = computed(() => {
    if (recetaDinamica.value.length === 0) return productoSeleccionado.value?.pesoEspecifico || DENSIDAD_DEFAULT;
    let suma = 0, porc = 0;
    recetaDinamica.value.forEach(item => {
        const p = parseFloat(item.cantidad.toString()) || 0;
        const d = parseFloat(item.densidad?.toString()) || DENSIDAD_DEFAULT;
        suma += (p * d); porc += p;
    });
    return porc === 0 ? DENSIDAD_DEFAULT : (suma / porc);
});

const kilosCalculados = computed(() => {
    if (form.value.esConsolidado) return Number(form.value.kilosTotales);
    if (!productoSeleccionado.value) return 0;
    const L = (Number(form.value.largo) || 0) / 1000; 
    const A = (Number(form.value.ancho) || 0) / 1000; 
    const E = Number(form.value.espesor) || 0;         
    const Cant = Number(form.value.cantidad) || 1;
    const Dens = Number(densidadMezcla.value);
    return parseFloat((L * A * E * Dens * Cant).toFixed(4));
});

const factorMerma = computed(() => 1 + (Number(form.value.merma || 0) / 100));

const insumosSinStock = computed(() => {
    const kilosNetos = Number(kilosCalculados.value);
    if (kilosNetos <= 0) return [];
    const faltantes: any[] = [];
    const factor = factorMerma.value;

    recetaDinamica.value.forEach(item => {
        const porcentajeInsumo = parseFloat(item.cantidad.toString()) || 0;
        const pesoNetoInsumo = (kilosNetos * porcentajeInsumo) / 100;
        const consumoReal = Number((pesoNetoInsumo * factor).toFixed(3));
        const idMaterial = Number(item.materiaPrimaId);

        let stockDisponible = 0;
        let nombreMaterial = item.nombreInsumo;

        if (idMaterial >= 990 && idMaterial <= 999) {
            stockDisponible = 0;
        } else {
            const mp = listaInventarioCompleto.value.find(m => m.id === idMaterial) || listaTodasMateriasPrimas.value.find(m => m.id === idMaterial);
            if (mp) {
                stockDisponible = Number(mp.stockActual || 0);
                nombreMaterial = mp.nombre;
            }
        }

        if (stockDisponible < (consumoReal - 0.001)) {
            faltantes.push({
                nombre: nombreMaterial,
                necesario: consumoReal,
                disponible: stockDisponible,
                diferencia: Number((consumoReal - stockDisponible).toFixed(2))
            });
        }
    });
    return faltantes;
});

const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);

const colorFinalParaPDF = computed(() => {
    if (form.value.colorTexto && form.value.colorTexto.trim() !== '') {
        return form.value.colorTexto.toUpperCase();
    }
    if (form.value.esProductoColor && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        return mb ? (mb.nombre.split(' ').length > 1 ? mb.nombre.split(' ').slice(1).join(' ') : mb.nombre) : 'A DEFINIR';
    }
    return '-';
});

function balancearBase() {
    if (recetaDinamica.value.length === 0) return;

    recetaDinamica.value.forEach(r => r.esBase = false);

    const sorted = [...recetaDinamica.value].sort((a, b) => Number(b.cantidad) - Number(a.cantidad));
    const nuevaBase = sorted[0];

    if (nuevaBase) {
        nuevaBase.esBase = true; 

        const sumaOtros = recetaDinamica.value.reduce((acc, item) => {
            if (item === nuevaBase) return acc;
            return acc + (parseFloat(item.cantidad.toString()) || 0);
        }, 0);

        const nuevoPorcentajeBase = 100 - sumaOtros;
        
        nuevaBase.cantidad = parseFloat(Math.max(0, nuevoPorcentajeBase).toFixed(2));
    }
}

function recalcularFormulaAutomatica() {
    let porcentajeColor = 2.00;
    const colorExistente = recetaDinamica.value.find(r => r.esColor);
    if (colorExistente) porcentajeColor = Number(colorExistente.cantidad);

    const borrar = ['esCarga', 'esBrillo', 'esEstearato', 'esUv', 'esCaucho'];
    if (form.value.esProductoColor && form.value.masterbatchId) borrar.push('esColor');

    let nueva = recetaDinamica.value.filter(r => {
        for (const flag of borrar) if (r[flag as keyof ItemReceta]) return false;
        return true;
    });

    const add = (nom: string, cant: number, tipo: string, mpId: number = 0, dens: number = DENSIDAD_DEFAULT) => {
        let m = null;
        if (mpId === 0) m = listaTodasMateriasPrimas.value.find(x => x.nombre.toUpperCase().includes(nom));
        else m = listaTodasMateriasPrimas.value.find(x => x.id === mpId);

        nueva.push({
            id: tipo,
            cantidad: cant,
            nombreInsumo: m ? m.nombre : (nom === 'COLOR' ? 'MASTERBATCH' : nom),
            densidad: m ? (m.pesoEspecifico || dens) : dens,
            materiaPrimaId: m ? m.id : mpId,
            [tipo]: true,
            esColor: tipo === 'esColor',
            esEstearato: tipo === 'esEstearato' 
        });
    };

    if (form.value.conBrillo) {
        const idBrillo = form.value.tipoBrillo === '777' ? ID_BRILLO_777 : idCristal555.value;
        const nombreBrillo = form.value.tipoBrillo === '777' ? 'BRILLO 777' : 'CRISTAL 555';
        add(nombreBrillo, form.value.porcBrillo, 'esBrillo', idBrillo);
    }
    
    if (form.value.conEstearato) add('ESTEARATO', PORC_ESTEARATO, 'esEstearato', ID_ESTEARATO);
    if (form.value.aditivoUV) add('UV', form.value.porcentajeUv, 'esUv', ID_UV);
    if (form.value.aditivoCaucho) add('CAUCHO', form.value.porcentajeCaucho, 'esCaucho', ID_CAUCHO);
    if (form.value.esProductoColor && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        if (mb) add('COLOR', porcentajeColor, 'esColor', mb.id, mb.pesoEspecifico);
    }
    if (form.value.aditivoCarga > 0) add('CARGA', form.value.aditivoCarga, 'esCarga', ID_CARGA);

    recetaDinamica.value = nueva;
    
    balancearBase();
}

async function actualizarRecetaFazonConCliente(clienteId: string | number, producto: Producto) {
    listaLotesCliente.value = [];
    loteFazonSeleccionadoId.value = '';

    if (!clienteId || !producto) return;

    const esFazon = producto.esFazon || producto.nombre.toUpperCase().includes('FAZON') || producto.nombre.toUpperCase().includes('SERVICIO');
    if (!esFazon || !clienteTieneFazonActivo.value) return;

    const materialPT = detectarMaterial(producto);

    const todoElStockCliente = listaInventarioCompleto.value.filter((p: any) => {
        const esDelCliente = Number(p.clienteId) === Number(clienteId);
        const tieneStock = p.stockActual > 0;
        const rubro = (p.rubro || '').toUpperCase();
        
        const esMolido = p.esScrap === true || rubro.includes('MOLIDO');

        if (!esDelCliente || !tieneStock || !esMolido) return false;

        if (materialPT) {
            const materialLote = detectarMaterial(p);
            if (materialLote && materialLote !== materialPT) {
                return false;
            }
        }
        return true;
    });

    listaLotesCliente.value = todoElStockCliente.sort((a, b) => b.stockActual - a.stockActual);

    if (listaLotesCliente.value.length > 0) {
        const mejorOpcion = listaLotesCliente.value[0];
        loteFazonSeleccionadoId.value = mejorOpcion.id;
        aplicarLoteFazonAReceta(mejorOpcion);
    } else {
        const itemFazon = recetaDinamica.value.find(r => r.esFazonInput || r.esBase);
        if (itemFazon) {
            itemFazon.nombreInsumo = "⚠️ CLIENTE SIN MATERIAL RECUPERADO/MOLIDO";
            itemFazon.materiaPrimaId = 0; 
        }
    }
}

function alCambiarLoteFazon() {
    const lote = listaLotesCliente.value.find(l => l.id === loteFazonSeleccionadoId.value);
    if (lote) aplicarLoteFazonAReceta(lote);
}

function aplicarLoteFazonAReceta(lote: any) {
    let itemFazon = recetaDinamica.value.find(r => r.esFazonInput || r.esBase);

    if (itemFazon && lote) {
        itemFazon.materiaPrimaId = lote.id;
        itemFazon.nombreInsumo = `MP: ${lote.nombre}`; 
        itemFazon.densidad = lote.pesoEspecifico || 1;
    } else if (!itemFazon && lote) {
        recetaDinamica.value.push({
            id: Date.now(),
            materiaPrimaId: lote.id,
            nombreInsumo: `MP: ${lote.nombre}`,
            cantidad: 100,
            densidad: lote.pesoEspecifico || 1,
            esBase: true,
            esFazonInput: true
        });
    }

    stockFazonDetectado.value = lote?.stockActual || null;
    balancearBase(); 
}

function quitarInsumoManual(index: number) {
    if (index >= 0 && index < recetaDinamica.value.length) {
        recetaDinamica.value.splice(index, 1);
        balancearBase();
    }
}

function agregarInsumoDesdeHijo(item: { id: number, porcentaje: number }) {
    const mp = listaTodasMateriasPrimas.value.find(m => m.id === item.id);
    if (mp) {
        recetaDinamica.value.push({
            id: Date.now(),
            materiaPrimaId: mp.id,
            nombreInsumo: mp.nombre,
            cantidad: item.porcentaje,
            densidad: mp.pesoEspecifico || 1,
            esBase: false
        });
        balancearBase();
    }
}

async function CargarProductosFiltrados(clienteId: number | string = '') {
    try {
        const cid = clienteId ? clienteId : '';
        const res = await api.get(`/Productos?clienteId=${cid}`);
        productos.value = res.data;
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
    if (!id) return;
    try {
        const res = await api.get(`/Productos/${id}`);
        const prod = res.data;

        if (!prod.esGenerico) {
            form.value.largo = prod.largo;
            form.value.ancho = prod.ancho;
            form.value.espesor = prod.espesor;
            if (!form.value.observacion) form.value.observacion = "Producción Stock";
        } else {
            form.value.largo = prod.largo || 0;
            form.value.ancho = prod.ancho || 0;
            form.value.espesor = prod.espesor || 0;
        }

        limiteMinimo.value = prod.espesorMinimo ?? 0;
        limiteMaximo.value = prod.espesorMaximo ?? 0;
        
        if (limiteMinimo.value === 0 && limiteMaximo.value === 0) {
            const nombre = (prod.nombre || '').toUpperCase();
            if (nombre.includes("FINO")) { 
                limiteMinimo.value = 0.40; 
                limiteMaximo.value = 0.90; 
            }
            else if (nombre.includes("GRUESO")) { 
                limiteMinimo.value = 0.90; 
                if (nombre.includes("ABS")) limiteMinimo.value = 1.00; 
            }
        }

        form.value.esProductoColor = prod.nombre.toUpperCase().includes('COLOR');
        form.value.colorTexto = prod.color || '';
        form.value.masterbatchId = '';
        form.value.aditivoCarga = 0;
        form.value.aditivoUV = false;
        form.value.aditivoCaucho = false;

        if (prod.receta?.length > 0) {
            recetaDinamica.value = prod.receta.map((r: any) => {
                const nombre = (r.nombreInsumo || '').toUpperCase();
                const idMP = Number(r.materiaPrimaId);
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
            
            if (form.value.clienteId) {
                nextTick(async () => { await actualizarRecetaFazonConCliente(form.value.clienteId, prod); });
            }
            
            recalcularFormulaAutomatica();
            stockFazonDetectado.value = null;
        } else {
            recetaDinamica.value = [];
            stockFazonDetectado.value = null;
        }
    } catch (e) {
        console.error(e);
        recetaDinamica.value = [];
    }
}

async function registrarProduccion() {
    mensaje.value = '';
    error.value = '';

    if (!espesorValido.value) return error.value = `⛔ ERROR DE CALIDAD: El espesor debe estar entre ${limiteMinimo.value} y ${limiteMaximo.value} mm.`;

    const pesoNetoGeometrico = kilosCalculados.value;
    if (pesoNetoGeometrico <= 0) return error.value = "El peso calculado es 0. Revise las medidas.";
    if (productoSeleccionado.value?.esFazon) {
        if (!form.value.clienteId) return error.value = "Seleccione Cliente.";
        if (!clienteTieneFazonActivo.value) return error.value = "⛔ ERROR: El cliente seleccionado NO tiene servicio de Fazón habilitado.";
    }
    if (!form.value.clienteId) return error.value = "⛔ ERROR: Debe seleccionar un Cliente obligatoriamente.";

    if (!espesorValido.value) return error.value = `⛔ ERROR DE CALIDAD: El espesor debe estar entre ${limiteMinimo.value} y ${limiteMaximo.value} mm.`;
    
    const tieneProhibido = recetaDinamica.value.some(r => r.materiaPrimaId === ID_MASTERBATCH_GENERICO);
    if (tieneProhibido) return error.value = "⛔ ERROR: Reemplaza el 'Masterbatch Varios' por un color real.";

    const tieneCero = recetaDinamica.value.some(r => Number(r.materiaPrimaId) === 0);
    if (tieneCero) return error.value = "⛔ ERROR: Hay un material en la fórmula sin asignar. Verifique que el cliente tenga stock válido o agregue la MP manualmente.";

    const porcentajeMerma = Number(form.value.merma || 0);
    const factorMermaCalc = 1 + (porcentajeMerma / 100);
    const pesoBrutoExacto = pesoNetoGeometrico * factorMermaCalc;

    const consumosReales = recetaDinamica.value.map(i => {
        const porcentajeEnReceta = parseFloat(i.cantidad.toString()) || 0;
        const kilosInsumo = (pesoBrutoExacto * porcentajeEnReceta) / 100;
        return {
            materiaPrimaId: Number(i.materiaPrimaId),
            cantidadKilos: Number(kilosInsumo.toFixed(3))
        };
    });

    try {
        loading.value = true;
        await api.post('/Ordenes', {
            productoTerminadoId: Number(form.value.productoTerminadoId),
            clienteId: form.value.clienteId ? Number(form.value.clienteId) : null,
            numeroPedidoCliente: form.value.numeroPedidoCliente || '', 
            notaPedido: form.value.notaPedido || '',
            cantidad: Number(form.value.cantidad),
            observacion: (form.value.observacion || ''),
            kilos: Number(pesoBrutoExacto.toFixed(3)),
            largo: Number(form.value.largo),
            ancho: Number(form.value.ancho),
            espesor: Number(form.value.espesor),
            color: colorFinalParaPDF.value,
            consumos: consumosReales
        });

        mensaje.value = `✅ Orden Generada. Peso Total: ${pesoBrutoExacto.toFixed(2)}kg`;
        idProduccionGenerada.value = true;
        
        limpiarBorrador(); 

        if (listaProduccionRef.value) listaProduccionRef.value.cargarHistorial();
        emit('guardado');
    } catch (e: any) {
        const msg = e.response?.data?.mensaje || e.response?.data || e.message;
        error.value = '❌ ' + msg;
    } finally {
        loading.value = false;
    }
}

function calcularEtiquetasPallets(kilosTotales: number, cantidadTotal: number, cantidadPalletsElegida: number) {
    if (cantidadPalletsElegida <= 1) {
        return [{ palletNumero: 1, palletTotal: 1, kilos: kilosTotales, laminas: cantidadTotal }];
    }

    let pallets = [];
    let laminasRestantes = cantidadTotal;
    let kilosRestantes = kilosTotales;

    for (let i = 1; i <= cantidadPalletsElegida; i++) {
        let esUltimoPallet = (i === cantidadPalletsElegida);

        let laminasPallet = esUltimoPallet 
            ? laminasRestantes 
            : Math.round(cantidadTotal / cantidadPalletsElegida);

        let kilosPallet = esUltimoPallet 
            ? kilosRestantes 
            : Math.round((kilosTotales / cantidadTotal) * laminasPallet);

        pallets.push({
            palletNumero: i,
            palletTotal: cantidadPalletsElegida,
            kilos: kilosPallet,
            laminas: laminasPallet
        });

        laminasRestantes -= laminasPallet;
        kilosRestantes -= kilosPallet;
    }

    return pallets;
}

const imprimirDesdeHistorial = async (payload: { orden: any, tipo: string }) => {
    const { orden, tipo } = payload;
    const isConsolidado = tipo === 'carga-consolidada';
    const tituloAlerta = isConsolidado ? 'LOTE MÚLTIPLE' : tipo.toUpperCase();
    
    if (!confirm(`¿Reimprimir ${tituloAlerta} de la Orden #${orden.id}?`)) return;

    try {
        loading.value = true;
        
        form.value.esConsolidado = isConsolidado;
        form.value.observacion = orden.observacion || '';
        form.value.cantidad = orden.cantidad;
        
        const mermaOrden = Number(orden?.merma ?? form.value.merma ?? 0) || 0;
        form.value.merma = isConsolidado ? 0 : mermaOrden;
        
        const kilosBrutos = Number(orden?.kilos) || 0;
        const factor = 1 + (mermaOrden / 100);
        
        form.value.kilosTotales = isConsolidado ? kilosBrutos : (factor > 0 ? Number((kilosBrutos / factor).toFixed(4)) : 0);

        form.value.notaPedido = String(orden?.notaPedido ?? orden?.id ?? '');
        form.value.numeroPedidoCliente = orden?.numeroPedidoCliente || '';
        form.value.clienteId = orden.clienteId || '';

        // 1. CARGAMOS EL PRODUCTO PARA TRAER EL COLOR ORIGINAL
        if (!isConsolidado && orden.productoId) {
            await CargarDatosProductos(orden.productoId);
        }

        form.value.productoTerminadoId = orden.productoId || ''; 

        if (orden.consumos && Array.isArray(orden.consumos)) {
            recetaDinamica.value = orden.consumos.map((c: any) => {
                const mpOriginal = listaTodasMateriasPrimas.value.find((m: any) => m.id === c.materiaPrimaId);
                const esEstearato = c.materiaPrimaId === ID_ESTEARATO;
                const esColor = c.materiaPrimaId === ID_MASTERBATCH_GENERICO || 
                                (c.nombreMateriaPrima && c.nombreMateriaPrima.toUpperCase().includes('MASTER'));
                
                const valorCantidad = isConsolidado 
                    ? c.cantidadKilos 
                    : (c.cantidadKilos && kilosBrutos > 0 ? ((c.cantidadKilos / kilosBrutos) * 100).toFixed(2) : 0);

                return {
                    id: Date.now() + Math.random(),
                    materiaPrimaId: c.materiaPrimaId,
                    nombreInsumo: c.nombreMateriaPrima || mpOriginal?.nombre || 'Insumo Histórico',
                    cantidad: valorCantidad, 
                    densidad: mpOriginal ? mpOriginal.pesoEspecifico : 1,
                    esEstearato: esEstearato,
                    esColor: esColor,
                    esBase: false 
                };
            });
        }

        // 2. MAGIA ANTIBUGS: Esperamos 1.5 segundos para asegurarnos de que la recarga 
        // automática no nos pise las variables, y recién ahí inyectamos las medidas exactas.
        setTimeout(async () => {
            if (orden.largo) form.value.largo = orden.largo;
            if (orden.ancho) form.value.ancho = orden.ancho;
            if (orden.espesor) form.value.espesor = orden.espesor;
            form.value.colorTexto = orden.color || '';

            await nextTick();
            await generarPDF(tipo as any);
            
            mensaje.value = `✅ Reimpresión ${tituloAlerta} generada.`;
            loading.value = false;
            
            setTimeout(() => { form.value.esConsolidado = false; }, 1000);
        }, 1500);

    } catch (e) {
        console.error(e);
        error.value = "Error recuperando historial.";
        loading.value = false;
    }
};

async function generarPDF(tipo: 'orden' | 'carga' | 'carga-consolidada') {
    ocultarFormula.value = (tipo === 'orden');

    if (tipo === 'orden' && form.value.kilosTotales > 1000 && cantidadPalletsUsuario.value > 1) {
        const tickets = calcularEtiquetasPallets(form.value.kilosTotales, form.value.cantidad, cantidadPalletsUsuario.value);
        const originalKilos = form.value.kilosTotales;
        const originalCantidad = form.value.cantidad;
        const originalObs = form.value.observacion;

        for (const ticket of tickets) {
            form.value.kilosTotales = ticket.kilos;
            form.value.cantidad = ticket.laminas;
            form.value.observacion = originalObs 
                ? `${originalObs} | [PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]` 
                : `[PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]`;

            await nextTick();
            await new Promise(r => setTimeout(r, 400));

            const elemento = document.getElementById('hoja-de-impresion');
            if (elemento) {
                await html2pdf().set({
                    margin: 0,
                    filename: `Orden_${form.value.notaPedido}_P${ticket.palletNumero}_${Date.now()}.pdf`,
                    image: { type: 'jpeg', quality: 0.75 },
                    html2canvas: { scale: 2 },
                    jsPDF: { unit: 'mm', format: 'a4' }
                }).from(elemento).save();
            }
        }

        form.value.kilosTotales = originalKilos;
        form.value.cantidad = originalCantidad;
        form.value.observacion = originalObs;

    } else {
        await nextTick();
        await new Promise(r => setTimeout(r, 200));
        
        const elemento = document.getElementById('hoja-de-impresion');
        if (elemento) {
            await html2pdf().set({
                margin: 0,
                filename: `Doc_${Date.now()}.pdf`,
                image: { type: 'jpeg', quality: 0.70 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'a4' }
            }).from(elemento).save();
        }
    }

    ocultarFormula.value = false;
}

watch(() => form.value.clienteId, async (nuevoCli) => {
    if (nuevoCli && !form.value.productoTerminadoId) {
        await CargarProductosFiltrados(nuevoCli);
    }
    if (nuevoCli && form.value.productoTerminadoId) {
        const prod = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
        if (prod) {
            setTimeout(async () => { await actualizarRecetaFazonConCliente(nuevoCli, prod); }, 200);
        }
    }
});

watch(() => form.value.productoTerminadoId, (v) => {
    if (form.value.esConsolidado) return;
    if (v) CargarDatosProductos(Number(v));
    else recetaDinamica.value = [];
});

watch(
    [
        () => form.value.masterbatchId, () => form.value.aditivoCarga, 
        () => form.value.porcBrillo, () => form.value.conEstearato, 
        () => form.value.aditivoUV, () => form.value.porcentajeUv, 
        () => form.value.aditivoCaucho, () => form.value.porcentajeCaucho,
        () => form.value.conBrillo, () => form.value.tipoBrillo
    ],
    recalcularFormulaAutomatica
);

watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { if (!v) form.value.llevaFilm = false; });

watch(kilosCalculados, (v) => {
    if (!form.value.esConsolidado) {
        form.value.kilosTotales = v;
    }
}, { immediate: true });

watch(() => form.value.kilosTotales, (v) => {
    if (v > 1000) {
        cantidadPalletsUsuario.value = Math.ceil(v / 1000);
    } else {
        cantidadPalletsUsuario.value = 1;
    }
});
const imprimirLoteOPsDesdeHistorial = async (ordenesArray: any[]) => {
    if (!confirm(`¿Generar un PDF unificado con las ${ordenesArray.length} Órdenes de Producción?\n\n(No toques nada en la pantalla mientras procesa)`)) return;

    try {
        mensaje.value = `⏳ Construyendo ${ordenesArray.length} páginas. Por favor, espere...`;
        ocultarFormula.value = true; 
        window.scrollTo(0, 0);

        // 1. Buscamos la hoja original y a su "padre" en el HTML
        const elementoOriginal = document.getElementById('hoja-de-impresion');
        if (!elementoOriginal || !elementoOriginal.parentNode) throw new Error("No se encontró la hoja base");

        // 2. Contenedor temporal (SIN posiciones raras, pegado directo al DOM normal)
        const contenedorTemporal = document.createElement('div');
        contenedorTemporal.id = 'temp-pdf-wrapper';
        contenedorTemporal.style.width = '210mm'; 
        contenedorTemporal.style.backgroundColor = '#ffffff';
        
        // Lo insertamos justo después de tu hoja original
        elementoOriginal.parentNode.insertBefore(contenedorTemporal, elementoOriginal.nextSibling);

        for (let i = 0; i < ordenesArray.length; i++) {
            const orden = ordenesArray[i];

            // Cargar datos en el formulario Vue
            form.value.esConsolidado = false;
            form.value.observacion = orden.observacion || '';
            form.value.cantidad = orden.cantidad;
            const mermaOrden = Number(orden?.merma ?? form.value.merma ?? 0) || 0;
            form.value.merma = mermaOrden;
            
            const kilosBrutos = Number(orden?.kilos) || 0;
            const factor = 1 + (mermaOrden / 100);
            form.value.kilosTotales = factor > 0 ? Number((kilosBrutos / factor).toFixed(4)) : 0;
            form.value.notaPedido = String(orden?.notaPedido ?? orden?.id ?? '');
            form.value.numeroPedidoCliente = orden?.numeroPedidoCliente || '';
            form.value.clienteId = orden.clienteId || '';

            if (orden.productoId) await CargarDatosProductos(orden.productoId);
            form.value.productoTerminadoId = orden.productoId || ''; 

            if (orden.consumos && Array.isArray(orden.consumos)) {
                recetaDinamica.value = orden.consumos.map((c: any) => {
                    const mpOriginal = listaTodasMateriasPrimas.value.find((m: any) => m.id === c.materiaPrimaId);
                    
                    // 🚨 RECUPERAMOS LA DETECCIÓN DE COLOR Y ESTEARATO
                    const nombreMayus = (c.nombreMateriaPrima || mpOriginal?.nombre || '').toUpperCase();
                    // Si el nombre dice "MASTER" o "COLOR", le avisamos al sistema que es el color
                    const esColor = nombreMayus.includes('MASTER') || nombreMayus.includes('COLOR');
                    const esEstearato = nombreMayus.includes('ESTEARATO');

                    return {
                        id: Date.now() + Math.random(),
                        materiaPrimaId: c.materiaPrimaId,
                        nombreInsumo: c.nombreMateriaPrima || mpOriginal?.nombre || 'Insumo',
                        cantidad: (c.cantidadKilos && kilosBrutos > 0 ? ((c.cantidadKilos / kilosBrutos) * 100).toFixed(2) : 0), 
                        densidad: mpOriginal ? mpOriginal.pesoEspecifico : 1,
                        esEstearato: esEstearato, // <-- Y ESTO TAMBIÉN
                        esBase: false 
                    };
                });
            }

            // Dejar que Vue pinte los datos reales
            await new Promise(r => setTimeout(r, 1000)); 
            form.value.largo = orden.largo;
            form.value.ancho = orden.ancho;
            form.value.espesor = orden.espesor;
            form.value.color = orden.color || '';
            await nextTick();
            await new Promise(r => setTimeout(r, 400)); 

            // CLONAR LA HOJA
            const clon = elementoOriginal.cloneNode(true) as HTMLElement;
            clon.removeAttribute('id'); 
            clon.style.display = 'block';

            // 🚨 TRUCO MAESTRO: Transformamos los inputs en texto puro
            // para que html2canvas no colapse tratando de renderizar etiquetas <input>
            const clonedInputs = clon.querySelectorAll('input, textarea');
            const originalInputs = elementoOriginal.querySelectorAll('input, textarea');
            
            clonedInputs.forEach((clonedInput: any, index) => {
                const valorReal = (originalInputs[index] as HTMLInputElement).value;
                const spanEstatico = document.createElement('span');
                spanEstatico.innerText = valorReal;
                spanEstatico.style.fontWeight = 'bold';
                spanEstatico.style.fontSize = '12px';
                spanEstatico.style.display = 'inline-block';
                // Reemplazamos el <input> por el <span>
                if (clonedInput.parentNode) {
                    clonedInput.parentNode.replaceChild(spanEstatico, clonedInput);
                }
            });

            // Armar la página BLINDADA a tamaño A4 estricto
            const hojaWrapper = document.createElement('div');
            hojaWrapper.style.width = '210mm';
            hojaWrapper.style.height = '296mm'; // 🚨 CLAVE: 1mm menos que el A4 real para que no rebalse
            hojaWrapper.style.overflow = 'hidden'; // 🚨 CLAVE: Si sobra 1 pixel invisible, lo corta y lo oculta
            hojaWrapper.style.boxSizing = 'border-box';
            hojaWrapper.style.position = 'relative';
            hojaWrapper.appendChild(clon);

            // Inyectar el salto oficial si no es la última hoja
            if (i < ordenesArray.length - 1) {
                const salto = document.createElement('div');
                salto.className = 'html2pdf__page-break';
                hojaWrapper.appendChild(salto);
            }

            contenedorTemporal.appendChild(hojaWrapper);

            // Inyectar el salto oficial si no es la última hoja
            if (i < ordenesArray.length - 1) {
                const salto = document.createElement('div');
                salto.className = 'html2pdf__page-break';
                hojaWrapper.appendChild(salto);
            }

            contenedorTemporal.appendChild(hojaWrapper);
        }

        mensaje.value = "⏳ Guardando el archivo PDF...";
        await new Promise(r => setTimeout(r, 500)); 

        // 3. MOMENTO CLAVE: Ocultamos tu hoja real para que no estorbe en la "foto"
        const displayOriginal = elementoOriginal.style.display;
        elementoOriginal.style.display = 'none';

        // 4. Configuración del PDF
        const opt: any = {
            margin: 0,
            filename: `Lote_OP_x${ordenesArray.length}_${Date.now()}.pdf`,
            image: { type: 'jpeg', quality: 0.70 }, 
            html2canvas: { scale: 2, useCORS: true, scrollY: 0 }, 
            pagebreak: { mode: 'css' },
            jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
        };

        await html2pdf().set(opt).from(contenedorTemporal).save();

        // 5. RESTAURAMOS TODO a la normalidad
        elementoOriginal.style.display = displayOriginal;
        contenedorTemporal.remove();

        mensaje.value = `✅ Archivo unificado de ${ordenesArray.length} OPs generado con éxito.`;

    } catch (e) {
        console.error("Error crítico en impresión múltiple:", e);
        error.value = "Error al generar el lote de impresión.";
    } finally {
        ocultarFormula.value = false;
    }
};
onMounted(async () => {
    try {
        loading.value = true;
        const [resProd, resCli, resInv] = await Promise.all([
            api.get('/Productos'),
            api.get('/Clientes'),
            api.get('/Productos/inventario-completo')
        ]);
        if (Array.isArray(resProd.data)) {
            productos.value = resProd.data;
            listaTodasMateriasPrimas.value = productos.value.filter(p => p.esMateriaPrima);
            listaMasterbatches.value = productos.value.filter(p => p.rubro?.includes('MASTERBATCH') || p.nombre.includes('MASTER'));
        }
        if (Array.isArray(resCli.data)) clientes.value = resCli.data;
        if (Array.isArray(resInv.data)) listaInventarioCompleto.value = resInv.data;

        const borradorGuardado = localStorage.getItem(STORAGE_KEY);
        if (borradorGuardado) {
            try {
                const datos = JSON.parse(borradorGuardado);
                const unDia = 24 * 60 * 60 * 1000;
                if (Date.now() - datos.timestamp < unDia) {
                    if (confirm("📝 Encontré un trabajo sin terminar. ¿Quieres recuperarlo?")) {
                        
                        Object.assign(form.value, datos.form);
                        recetaDinamica.value = datos.receta;
                        
                        setTimeout(() => {
                            form.value.largo = datos.form.largo;
                            form.value.ancho = datos.form.ancho;
                            form.value.espesor = datos.form.espesor;
                            form.value.cantidad = datos.form.cantidad;
                            form.value.observacion = datos.form.observacion;
                            form.value.kilosTotales = datos.form.kilosTotales;
                            form.value.colorTexto = datos.form.colorTexto || '';
                                
                            mensaje.value = "📝 Datos y medidas recuperados con éxito.";
                        }, 1500);

                    } else {
                        limpiarBorrador();
                    }
                }
            } catch (e) {
                console.error("Error leyendo borrador", e);
                limpiarBorrador();
            }
        }

    } catch (e) {
        console.error("Error inicializando producción:", e);
    } finally {
        loading.value = false;
    }

    await cargarNotaPedidoSugerida();
});
</script>

<template>
  <div class="contenedor-principal-produccion">
    
    <div class="bloque-superior">
        <div class="panel-izquierdo">
            <div class="hoja-contenedor">
                <HojaImpresion 
                    id="hoja-de-impresion"
                    :form="form" 
                    :producto="productoSeleccionado" 
                    :cliente="clienteSeleccionado" 
                    :receta="recetaDinamica" 
                    :colorFinal="colorFinalParaPDF" 
                    :densidad="densidadMezcla" 
                    :totalPorcentaje="totalPorcentajeReceta" 
                    :materiasPrimas="materiasPrimasLimpias" 
                    :ocultarFormula="ocultarFormula" 
                    @add-insumo="agregarInsumoDesdeHijo" 
                    @remove-insumo="quitarInsumoManual" 
                    @update-receta="balancearBase"  
                />
            </div>
        </div>

        <div class="panel-derecho">
            <div class="header-control"><h3>⚙️ Configuración</h3></div>
            
            <label>Cliente / Producto:</label>
            <select v-model="form.clienteId" style="margin-bottom:5px">
                <option disabled value="">Cliente...</option>
                <option v-for="c in clientes" :key="c.id" :value="c.id">
                    {{c.razonSocial}} {{ c.esFazon ? '' : '(Venta)' }}
                </option>
            </select>

            <label style="color:#f39c12;">📂 N° Pedido Cliente (OC):</label>
            <input type="text" v-model="form.numeroPedidoCliente" placeholder="Ej: OC-4455" style="font-weight:bold; border: 1px solid #f39c12; margin-bottom: 5px;">

            <label style="color:#1abc9c;">🧾 Nota de Pedido (Flexxus):</label>
            <div class="fila-input" style="margin-bottom: 5px;">
                <input
                    type="text"
                    v-model="form.notaPedido"
                    placeholder="Ej: 12345"
                    style="font-weight:bold; border: 1px solid #1abc9c;"
                >
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
                <option v-for="p in listaProductosDisponibles" :key="p.id" :value="p.id">
                    {{ p.esFazon ? '★ ' : '' }}{{ p.nombre }} {{ p.esGenerico ? '(A Medida)' : (p.esFazon ? '(Fazon)' : '(Estándar)') }}
                </option>
            </select>

            <div v-if="listaLotesCliente.length > 0" class="box-fazon-selector">
                <label style="color: #2ecc71;">♻️ Lote Recuperado (Fazón):</label>
                <select v-model="loteFazonSeleccionadoId" @change="alCambiarLoteFazon" class="select-fazon">
                    <option disabled value="">-- Seleccionar Lote --</option>
                    <option v-for="lote in listaLotesCliente" :key="lote.id" :value="lote.id">
                        {{ lote.nombre }} (Stock: {{ lote.stockActual }} kg)
                    </option>
                </select>
            </div>

            <div v-if="form.productoTerminadoId" class="seccion-medidas-editables">
                <div v-if="form.esProductoColor" class="box-color">
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
                        >
                    </div>
                </div>

                <label class="lbl-sep">
                    Medidas: <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span><span v-else style="color:#2ecc71">(EDITABLES)</span>
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
                        <input type="number" v-model="form.espesor" step="0.01" 
                               :disabled="medidasBloqueadas" 
                               :class="{'input-lock': medidasBloqueadas, 'input-error': !espesorValido}" 
                               style="font-weight:bold;">
                    </div>
                    <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
                </div>
                
                <div class="fila-input" style="margin-top:10px; border-top:1px dashed #7f8c8d; padding-top:10px;">
                    <div style="flex:1">
                        <label style="color:#e67e22;">🔥 Desperdicio (%)</label>
                        <input type="number" v-model="form.merma" min="0" max="50" style="color:#e67e22; font-weight:bold;">
                    </div>
                </div>

                <div class="resumen-peso">Peso Final PT: {{ form.kilosTotales }} Kg <small style="color:#bbb; display:block;">(Consumo Real MP +{{ form.merma }}%)</small></div>
                
                <label class="lbl-sep">Aditivos:</label>
                
                <div class="fila-control-aditivo" style="align-items: flex-start;">
                    <label class="check-container" :class="{ 'disabled': form.espesor < 1 }" style="margin-top: 5px !important;">
                        <input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> ✨ Brillo
                    </label>
                    <div v-if="form.conBrillo" class="bloque-derecha-brillo">
                        <select v-model="form.tipoBrillo" class="select-brillo">
                            <option value="777">Brillo 777</option>
                            <option value="555">Brillo 555 (Cristal)</option>
                        </select>
                        <div class="input-porcentaje">
                            <input type="number" v-model="form.porcBrillo" step="0.01" min="0"> %
                        </div>
                    </div>
                </div>

                <div class="fila-control-aditivo"><label class="check-container" :class="{ 'disabled': !form.conBrillo }"><input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 🛡️ Con Film</label></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.conEstearato"> 🧪 Estearato</label></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.aditivoUV"> ☀️ UV</label><div v-if="form.aditivoUV" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeUv" step="0.01" min="0"> %</div></div></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.aditivoCaucho"> 🚜 Caucho</label><div v-if="form.aditivoCaucho" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeCaucho" step="0.01" min="0"> %</div></div></div>

                <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
                <select v-model="form.tipoCorona"><option value="Ninguno">Sin Tratamiento</option><option value="Simple">Simple</option><option value="Doble">Doble</option></select>
                
                <label class="lbl-sep">Cargas:</label>
                <div class="fila-input"><div style="flex:1"><label>Carga Mineral (%)</label><input type="number" v-model="form.aditivoCarga"></div></div>
            </div>
            
            <div class="fila-input" style="margin-top:10px"><div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div></div>
            
            <div v-if="Math.abs(totalPorcentajeReceta - 100) > 0.5" class="alerta-error">⚠️ Receta suma {{ totalPorcentajeReceta }}%.</div>
            
            <div v-if="hayBloqueoDeStock" class="alerta-stock-warning">
                <h4>⚠️ Material Insuficiente (Irá a la Cola)</h4>
                <p style="margin: 0 0 5px 0; font-size: 11px;">La orden se guardará en el Backlog hasta que ingrese este stock:</p>
                <ul>
                    <li v-for="(falla, i) in insumosSinStock" :key="i">
                        <strong>{{ falla.nombre }}</strong>: Falta {{ falla.diferencia.toFixed(2) }} kg (Disp: {{ falla.disponible }})
                    </li>
                </ul>
            </div>

            <button 
                class="btn-guardar" 
                @click="registrarProduccion" 
                :disabled="loading || !form.clienteId || !form.productoTerminadoId" 
                :class="{ 'btn-warning': hayBloqueoDeStock && form.clienteId && form.productoTerminadoId }"
            >
                <span v-if="loading">⏳ PROCESANDO...</span>
                <span v-else-if="!form.clienteId || !form.productoTerminadoId">🚫 SELECCIONE CLIENTE Y PRODUCTO</span>
                <span v-else-if="hayBloqueoDeStock">📥 GUARDAR EN COLA</span>
                <span v-else>💾 GUARDAR ORDEN LISTA</span>
            </button>

            <div v-if="idProduccionGenerada && form.kilosTotales > 1000" class="alerta-pallets">
                <p style="margin: 0 0 10px 0; font-weight: bold; font-size: 13px;">⚠️ Pedido de {{ form.kilosTotales }} kg.</p>
                <label style="color: #856404; display: inline-block; margin: 0;">¿Dividir en cuántos pallets?</label>
                <input type="number" v-model.number="cantidadPalletsUsuario" min="1" style="width: 60px; display: inline-block; margin-left: 10px; border: 1px solid #ffeeba; background: white; color: black; padding: 4px;">
            </div>

            <div v-if="idProduccionGenerada" class="grupo-botones-pdf">
                <button class="btn-imprimir btn-orden" @click="generarPDF('orden')">📄 Orden</button>
                <button class="btn-imprimir btn-carga" @click="generarPDF('carga')">🧪 Carga</button>
            </div>
            
            <p class="success">{{ mensaje }}</p>
            <p class="error">{{ error }}</p>
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
</template>

<style scoped>
.contenedor-principal-produccion {
    display: flex; flex-direction: column; width: 100%; min-height: 100vh;
    font-family: 'Segoe UI', sans-serif; background-color: #ecf0f1;
}
.bloque-superior { display: flex; width: 100%; flex-wrap: wrap; }
.panel-izquierdo { 
    flex: 1; background-color: #e0e6ed; display: flex; justify-content: center; 
    align-items: flex-start; padding: 20px; border-right: 1px solid #bdc3c7; 
    overflow: hidden; min-width: 400px; 
}
.hoja-contenedor { 
    background: white; width: 210mm; min-height: 297mm; 
    box-shadow: 0 10px 25px rgba(0,0,0,0.1); transform: scale(0.80); 
    transform-origin: top center; margin-bottom: -350px; 
}
.panel-derecho { 
    width: 350px; min-width: 350px; background-color: #2c3e50; color: white; 
    display: flex; flex-direction: column; padding: 20px; 
    box-shadow: -5px 0 15px rgba(0,0,0,0.1); z-index: 10; border-left: 1px solid #34495e; 
}
.bloque-inferior { width: 100%; padding: 20px; background-color: #f8f9fa; border-top: 3px solid #bdc3c7; }

.header-control h3 { margin-top: 0; border-bottom: 2px solid #3498db; padding-bottom: 10px; color: #ecf0f1; font-size: 1.1rem; }
label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; font-weight: 600; }
select, input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 13px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
.fila-input { display: flex; gap: 8px; margin-bottom: 5px; }
.btn-sugerido {
    width: 130px;
    margin-top: 2px;
    border-radius: 4px;
    border: 1px solid #1abc9c;
    background: transparent;
    color: #1abc9c;
    font-weight: bold;
    cursor: pointer;
    font-size: 12px;
    padding: 8px;
}
.btn-sugerido:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}
.seccion-medidas-editables { background: #34495e; padding: 12px; border-radius: 6px; margin-top: 15px; border: 1px solid #4e6475; }
.box-color { margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px; }
.lbl-sep { color: #f1c40f !important; font-weight: bold; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-top: 15px !important; margin-bottom: 5px; }
.resumen-peso { font-weight: bold; color: #2ecc71; text-align: right; margin-top: 10px; font-size: 14px; border-top: 1px solid #7f8c8d; padding-top: 5px; }
.check-container { display: flex; align-items: center; cursor: pointer; color: #ecf0f1; font-weight: bold; font-size: 13px; margin-top: 8px !important; }
.check-container input { width: auto; margin-right: 8px; }
.check-container.disabled { opacity: 0.5; cursor: not-allowed; }
.alerta-error { background: #c0392b; color: white; padding: 10px; border-radius: 5px; margin-top: 15px; font-weight: bold; text-align: center; font-size: 12px; }

.alerta-stock-warning { 
    background-color: #fff9e6; 
    border: 1px solid #f1c40f; 
    color: #d35400; 
    padding: 10px; 
    border-radius: 6px; 
    margin-top: 15px; 
    font-size: 12px; 
    text-align: left; 
}
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

.box-fazon-selector {
    background-color: #27ae60;
    padding: 10px;
    border-radius: 6px;
    margin-top: 10px;
    border: 1px solid #2ecc71;
}
.box-fazon-selector label {
    color: white !important;
}
.select-fazon {
    background-color: white;
    font-weight: bold;
    color: #2c3e50;
    border: 2px solid #2ecc71;
}

.alerta-pallets {
    background-color: #fff3cd;
    padding: 15px;
    border-radius: 8px;
    margin-top: 15px;
    color: #856404;
    border: 1px solid #ffeeba;
}

@media (max-width: 1000px) { 
    .bloque-superior { flex-direction: column; } 
    .panel-izquierdo { width: 100%; border-right: none; border-bottom: 1px solid #bdc3c7; } 
    .panel-derecho { width: 100%; min-width: auto; } 
    .hoja-contenedor { transform: scale(0.55); margin-bottom: -400px; } 
}
</style>