<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
// @ts-ignore
import html2pdf from 'html2pdf.js'
import HojaImpresion from '../components/HojaImpresion.vue'
import ListaProduccion from '../components/ListaProduccion.vue'
import { ProduccionAPI } from '@/services/produccionService'

const DENSIDAD_DEFAULT = 1.1;

const ID_BRILLO_777 = 1073; 
const ID_ESTEARATO = 1074; 
const ID_UV = 1075; 
const ID_CAUCHO = 1076; 
const ID_CARGA = 1077; 
const ID_MASTERBATCH_GENERICO = 22;
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
const listaTodasMateriasPrimas = ref<any[]>([])

const listaMasterbatches = computed(() => {
    const origenDatos = listaTodasMateriasPrimas.value.length > 0 
        ? listaTodasMateriasPrimas.value 
        : listaInventarioCompleto.value;

    return origenDatos.filter(mp => {
        const nombre = (mp.nombre || '').toUpperCase();
        const rubro = (mp.rubro || '').toUpperCase();
        
        return rubro.includes('MASTERBATCH') || 
               nombre.includes('MASTERBATCH') || 
               nombre.includes('PIGMENTO');
    });
});

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
const imprimiendoHistorial = ref(false);
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
    esConsolidado: false,
    esBobina: false,
    kilosPorBobina: 0,
    productoNombre: '',
    clienteNombre: ''
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

        const esScrapPuro = mp.esScrap || nombre.includes('SCRAP') || rubro.includes('SCRAP');
        if (esScrapPuro) return false;

        const esMolido = rubro.includes('MOLIDO') || nombre.includes('RECUPERADO');

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
    
    const Cant = Number(form.value.cantidad) || 1;

    if (form.value.esBobina) {
        return parseFloat(((Number(form.value.kilosPorBobina) || 0) * Cant).toFixed(4));
    }
    
    const L = (Number(form.value.largo) || 0) / 1000; 
    const A = (Number(form.value.ancho) || 0) / 1000; 
    const E = Number(form.value.espesor) || 0;        
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

const mostrarCajaColor = computed(() => {
    if (form.value.productoTerminadoId) {
        const prod = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
        if (prod && (prod.nombre || '').toUpperCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").includes('COLOR')) return true;
    }
    
    if (recetaDinamica.value && recetaDinamica.value.length > 0) {
        return recetaDinamica.value.some(r => Number(r.materiaPrimaId) === 22 || r.esColor === true);
    }
    
    return false;
});

watch(mostrarCajaColor, (v) => {
    if (!v) form.value.masterbatchId = '';
});

const colorFinalParaPDF = computed(() => {
    if (form.value.colorTexto && form.value.colorTexto.trim() !== '') {
        return form.value.colorTexto.toUpperCase();
    }
    if (mostrarCajaColor.value && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        return mb ? (mb.nombre.split(' ').length > 1 ? mb.nombre.split(' ').slice(1).join(' ') : mb.nombre) : 'A DEFINIR';
    }
    return '-';
});

function balancearBase() {
    if (form.value.esConsolidado) return; 

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
    
    // 🚨 ACÁ ESTÁ LA MAGIA: Buscamos si ya hay un color O si está el comodín (ID 22) para copiarle el %
    const colorExistente = recetaDinamica.value.find(r => r.esColor || Number(r.materiaPrimaId) === 22);
    if (colorExistente) porcentajeColor = Number(colorExistente.cantidad);

    const borrar = ['esCarga', 'esBrillo', 'esEstearato', 'esUv', 'esCaucho'];
    if (mostrarCajaColor.value && form.value.masterbatchId) borrar.push('esColor');

    let nueva = recetaDinamica.value.filter(r => {
        for (const flag of borrar) if (r[flag as keyof ItemReceta]) return false;
        // Quitamos el ID 22 para poder meter el color real en su lugar
        if (mostrarCajaColor.value && form.value.masterbatchId && Number(r.materiaPrimaId) === 22) return false;
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
    if (mostrarCajaColor.value && form.value.masterbatchId) {
        const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
        // 🚨 Acá inyecta el color pero usando el porcentaje exacto que le robamos al comodín arriba
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
    const mp = listaTodasMateriasPrimas.value.find(m => m.id === item.id) || 
               listaInventarioCompleto.value.find(m => m.id === item.id);
    
    if (mp) {
        recetaDinamica.value.push({
            id: 'manual_' + Date.now(),
            materiaPrimaId: mp.id,
            nombreInsumo: mp.nombre,
            cantidad: item.porcentaje,
            densidad: mp.pesoEspecifico || 1.1,
            esBase: false
        });
        
        balancearBase();
    } else {
        alert("⚠️ Ocurrió un error: No pudimos encontrar los datos técnicos de este material en el sistema.");
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
    if (!id || imprimiendoHistorial.value) return; 
    try {
        const res = await api.get(`/Productos/${id}`);
        const prod = res.data;

        if (prod.receta && Array.isArray(prod.receta) && prod.receta.length > 0) {
            recetaDinamica.value = prod.receta.map((r: any) => ({
                id: r.id || Math.random(),
                materiaPrimaId: r.materiaPrimaId || r.id,
                nombreInsumo: r.nombreInsumo || r.nombreMateriaPrima || r.nombre,
                cantidad: r.cantidad || r.porcentaje || 0,
                densidad: r.densidad || r.pesoEspecifico || 1.1,
                esBase: r.esBase || false
            }));
            
            if (typeof balancearBase === 'function') balancearBase();
        }

        if (!form.value.largo || form.value.largo === 0) {
            form.value.esBobina = (prod.nombre || '').toUpperCase().includes('BOBINA');
            form.value.largo = form.value.esBobina ? 0 : (prod.largo || 0);
        }
        
        if (!form.value.ancho || form.value.ancho === 0) {
            form.value.ancho = prod.ancho || 0;
        }
        
        if (!form.value.espesor || form.value.espesor === 0) {
            form.value.espesor = prod.espesor || 0;
        }

        if (!form.value.colorTexto || form.value.colorTexto === '') {
            form.value.colorTexto = prod.color || '';
        }

    } catch (e) { 
        console.error("Error cargando datos maestros:", e); 
    }
}

const imprimirDesdeHistorial = async (payload: { orden: any, tipo: string }) => {
    const { orden, tipo } = payload;
    const isConsolidado = tipo === 'carga-consolidada';
    try {
        loading.value = true;
        imprimiendoHistorial.value = true;
        
        recetaDinamica.value = []; 
        form.value.observacion = '';

        form.value.esConsolidado = isConsolidado;
        form.value.productoTerminadoId = orden.productoId;
        form.value.clienteId = orden.clienteId;
        
        await nextTick();
        
        form.value.notaPedido = String(orden.notaPedido || orden.id);
        form.value.productoNombre = orden.producto;
        form.value.numeroPedidoCliente = orden.numeroPedidoCliente;
        form.value.clienteNombre = orden.clienteNombre;
        form.value.largo = orden.largo;
        form.value.ancho = orden.ancho;
        form.value.espesor = orden.espesor;
        form.value.esBobina = !!orden.esBobina;
        form.value.cantidad = orden.cantidad;
        form.value.observacion = orden.observacion || '';
        
        // 🚨 ACÁ ESTÁ LA CORRECCIÓN CLAVE: Leemos los aditivos directo de la DB
        form.value.conBrillo = orden.conBrillo || false;
        form.value.llevaFilm = orden.llevaFilm || false;
        form.value.tipoCorona = orden.tipoCorona || 'Ninguno';
        form.value.color = orden.color || '';
        form.value.colorTexto = orden.color || ''; 
        
        const desp = Number(orden.desperdicio || 0);
        form.value.merma = desp;
        
        form.value.kilosTotales = isConsolidado ? orden.kilos : orden.kilos;

        const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

        recetaDinamica.value = orden.consumos.map((c: any) => ({
            id: Math.random(),
            materiaPrimaId: c.materiaPrimaId,
            nombreInsumo: c.nombreMateriaPrima,
            cantidad: isConsolidado ? c.cantidadKilos : Number(((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2))
        }));

        if (!form.value.esConsolidado) {
            balancearBase();
        }
        await new Promise(r => setTimeout(r, 1000)); 

        if (tipo === 'orden' && form.value.kilosTotales > 1000) {
            const palletsSugeridos = Math.ceil(form.value.kilosTotales / 1000);
            const respuesta = prompt(`⚠️ Pedido grande (${form.value.kilosTotales} kg).\n\n¿En cuántos pallets querés dividir la impresión?`, String(palletsSugeridos));
            
            cantidadPalletsUsuario.value = respuesta ? parseInt(respuesta) : 1;
            if (isNaN(cantidadPalletsUsuario.value) || cantidadPalletsUsuario.value < 1) {
                cantidadPalletsUsuario.value = 1;
            }
        } else {
            cantidadPalletsUsuario.value = 1;
        }

        await generarPDF(tipo as any);

        if (tipo === 'orden') {
            await api.post(`/Ordenes/marcar-impresa/${orden.id}`);
            if (listaProduccionRef.value) {
                await listaProduccionRef.value.cargarHistorial();
            }
        }

    } catch (e) {
        console.error("Error en reimpresión:", e);
    } finally { 
        imprimiendoHistorial.value = false;
        loading.value = false; 
        
        setTimeout(() => {
            if (typeof limpiarFormulario === 'function') limpiarFormulario();
        }, 1000);
    }
};

function limpiarFormulario() {
    form.value = {
        productoTerminadoId: '',
        clienteId: '',
        numeroPedidoCliente: '',
        notaPedido: notaPedidoSugerida.value || '',
        cantidad: 1,
        observacion: '',
        largo: 0, ancho: 0, espesor: 0, color: '',
        conBrillo: false, tipoBrillo: '777', porcBrillo: 2.00,
        llevaFilm: false, tipoCorona: 'Ninguno',
        conEstearato: false, esProductoColor: false, masterbatchId: '', colorTexto: '',
        aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00,
        aditivoCarga: 0, merma: 8, kilosTotales: 0,
        esConsolidado: false, esBobina: false, kilosPorBobina: 0,
        productoNombre: '',
        clienteNombre: ''
    };
    recetaDinamica.value = [];
    idProduccionGenerada.value = false;
    limpiarBorrador();
}

async function registrarProduccion() {
    mensaje.value = '';
    error.value = '';

    if (!form.value.esConsolidado && Math.abs(totalPorcentajeReceta.value - 100) > 0.1) {
        return error.value = `⛔ ERROR DE FÓRMULA: La receta suma ${totalPorcentajeReceta.value}%. Debe ajustarla para que dé exactamente 100% antes de guardar.`;
    }
    if (!espesorValido.value) return error.value = `⛔ ERROR DE CALIDAD: El espesor debe estar entre ${limiteMinimo.value} y ${limiteMaximo.value} mm.`;

    const pesoNetoGeometrico = Number(kilosCalculados.value);
    if (pesoNetoGeometrico <= 0) return error.value = "El peso calculado es 0. Revise las medidas.";
    
    if (!form.value.clienteId) return error.value = "⛔ ERROR: Debe seleccionar un Cliente obligatoriamente.";
    
    const tieneProhibido = recetaDinamica.value.some(r => r.materiaPrimaId === ID_MASTERBATCH_GENERICO);
    if (tieneProhibido) return error.value = "⛔ ERROR: Reemplaza el 'Masterbatch Varios' por un color real.";

    const tieneCero = recetaDinamica.value.some(r => Number(r.materiaPrimaId) === 0);
    if (tieneCero) return error.value = "⛔ ERROR: Hay un material en la fórmula sin asignar.";

    const porcentajeDesperdicio = Number(form.value.merma || 0);
    const factorMultiplicador = 1 + (porcentajeDesperdicio / 100);

    const consumosRealesBrutos = recetaDinamica.value.map(i => {
        const porcentajeEnReceta = parseFloat(i.cantidad.toString()) || 0;
        const kilosInsumoBruto = ((pesoNetoGeometrico * porcentajeEnReceta) / 100) * factorMultiplicador;
        return {
            materiaPrimaId: Number(i.materiaPrimaId),
            cantidadKilos: Number(kilosInsumoBruto.toFixed(3))
        };
    });

    try {
        loading.value = true;
        await api.post('/Ordenes', {
            productoTerminadoId: Number(form.value.productoTerminadoId),
            clienteId: Number(form.value.clienteId),
            numeroPedidoCliente: form.value.numeroPedidoCliente || '', 
            notaPedido: form.value.notaPedido || '',
            cantidad: Number(form.value.cantidad),
            observacion: (form.value.observacion || ''),
            
            kilos: Number(pesoNetoGeometrico.toFixed(3)), 
            
            desperdicio: porcentajeDesperdicio, 
            esBobina: form.value.esBobina,
            largo: Number(form.value.largo),
            ancho: Number(form.value.ancho),
            espesor: Number(form.value.espesor),
            color: colorFinalParaPDF.value,
            consumos: consumosRealesBrutos,
            
            conBrillo: form.value.conBrillo,
            llevaFilm: form.value.llevaFilm,
            tipoCorona: form.value.tipoCorona
        });

        mensaje.value = `✅ Orden Generada (Neto: ${pesoNetoGeometrico.toFixed(2)}kg). Insumos con ${porcentajeDesperdicio}% de desperdicio.`;
        idProduccionGenerada.value = true;
        limpiarBorrador(); 
        if (listaProduccionRef.value) listaProduccionRef.value.cargarHistorial();
        emit('guardado');
        limpiarFormulario(); 
        setTimeout(() => { mensaje.value = ''; }, 5000);
    } catch (e: any) {
        error.value = '❌ ' + (e.response?.data?.mensaje || e.message);
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

async function generarPDF(tipo: 'orden' | 'carga' | 'carga-consolidada') {
    ocultarFormula.value = (tipo === 'orden');
    const bloqueoOriginal = imprimiendoHistorial.value;
    imprimiendoHistorial.value = true;

    if (tipo === 'orden' && form.value.kilosTotales > 1000 && cantidadPalletsUsuario.value > 1) {
        const tickets = calcularEtiquetasPallets(form.value.kilosTotales, form.value.cantidad, cantidadPalletsUsuario.value);
        const originalKilos = form.value.kilosTotales;
        const originalCantidad = form.value.cantidad;
        const originalObs = form.value.observacion;

        const elementoOriginal = document.getElementById('hoja-de-impresion');
        const contenedorTemporal = document.createElement('div');
        contenedorTemporal.style.width = '210mm';

        for (const ticket of tickets) {
            form.value.kilosTotales = ticket.kilos;
            form.value.cantidad = ticket.laminas;
            form.value.observacion = originalObs 
                ? `${originalObs} | [PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]` 
                : `[PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]`;

            await nextTick();
            await new Promise(r => setTimeout(r, 800));

            if (elementoOriginal) {
                const clon = elementoOriginal.cloneNode(true) as HTMLElement;
                clon.style.display = 'block';
                
                const inputsOriginales = elementoOriginal.querySelectorAll('input, textarea');
                const inputsClonados = clon.querySelectorAll('input, textarea');
                inputsClonados.forEach((input: any, idx) => {
                    const span = document.createElement('span');
                    span.innerText = (inputsOriginales[idx] as HTMLInputElement).value;
                    span.style.fontWeight = 'bold';
                    input.parentNode?.replaceChild(span, input);
                });

                const wrap = document.createElement('div');
                if (ticket.palletNumero < ticket.palletTotal) {
                    wrap.style.pageBreakAfter = 'always';
                }
                wrap.appendChild(clon);
                contenedorTemporal.appendChild(wrap);
            }
        }

        await html2pdf().set({
            margin: 0,
            filename: `Orden_${form.value.notaPedido}_Pallets_${Date.now()}.pdf`,
            image: { type: 'jpeg', quality: 0.75 },
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'mm', format: 'a4' }
        }).from(contenedorTemporal).save();

        contenedorTemporal.remove();
        form.value.kilosTotales = originalKilos;
        form.value.cantidad = originalCantidad;
        form.value.observacion = originalObs;

    } else {
        await nextTick();
        await new Promise(r => setTimeout(r, 600));
        
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
    imprimiendoHistorial.value = bloqueoOriginal;
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
    if (v && !imprimiendoHistorial.value) {
        CargarDatosProductos(Number(v));
    } else if (!v) {
        recetaDinamica.value = [];
    }
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
    if (!form.value.esConsolidado && !imprimiendoHistorial.value) {
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
watch(imprimiendoHistorial, (estaImprimiendo) => {
    if (!estaImprimiendo && recetaDinamica.value.length > 0 && !form.value.esConsolidado) {
        balancearBase();
    }
});

const imprimirLoteOPsDesdeHistorial = async (ordenesArray: any[]) => {
    try {
        mensaje.value = `⏳ Construyendo páginas...`;
        
        ocultarFormula.value = true; 
        imprimiendoHistorial.value = true; 

        const elementoOriginal = document.getElementById('hoja-de-impresion');
        if (!elementoOriginal) throw new Error("No se encontró el elemento hoja-de-impresion");

        const contenedorTemporal = document.createElement('div');
        contenedorTemporal.style.width = '210mm';

        for (const orden of ordenesArray) {
            recetaDinamica.value = [];
            form.value.esConsolidado = false;
            form.value.productoTerminadoId = orden.productoId;
            form.value.clienteId = orden.clienteId;
            
            await nextTick();
            form.value.notaPedido = String(orden.notaPedido || orden.id);
            form.value.numeroPedidoCliente = orden.numeroPedidoCliente || '-';
            form.value.largo = orden.largo || 0;
            form.value.ancho = orden.ancho || 0;
            form.value.espesor = orden.espesor || 0;
            form.value.colorTexto = orden.color || '';
            form.value.cantidad = orden.cantidad;
            form.value.esBobina = !!orden.esBobina;

            form.value.observacion = orden.observacion || '';
            form.value.conBrillo = !!orden.conBrillo;
            form.value.llevaFilm = !!orden.llevaFilm;
            form.value.tipoCorona = orden.tipoCorona || 'Ninguno';

            const desp = Number(orden.desperdicio || 0);
            form.value.merma = desp;
            form.value.kilosTotales = orden.kilos; 

            const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

            if (orden.consumos) {
                recetaDinamica.value = orden.consumos.map((c: any) => ({
                    id: Math.random(),
                    materiaPrimaId: c.materiaPrimaId,
                    nombreInsumo: c.nombreMateriaPrima,
                    cantidad: ((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2)
                }));
            }
            if (!form.value.esConsolidado) {
                balancearBase();
            }
            
            await new Promise(r => setTimeout(r, 800));

            const clon = elementoOriginal.cloneNode(true) as HTMLElement;
            clon.style.display = 'block';
            
            const inputsOriginales = elementoOriginal.querySelectorAll('input, textarea');
            const inputsClonados = clon.querySelectorAll('input, textarea');
            inputsClonados.forEach((input: any, idx) => {
                const span = document.createElement('span');
                span.innerText = (inputsOriginales[idx] as HTMLInputElement).value;
                span.style.fontWeight = 'bold';
                input.parentNode?.replaceChild(span, input);
            });

            const wrap = document.createElement('div');
            wrap.style.pageBreakAfter = 'always';
            wrap.appendChild(clon);
            contenedorTemporal.appendChild(wrap);
        }

        await html2pdf().set({
            margin: 0,
            filename: `Lote_OP_${Date.now()}.pdf`,
            html2canvas: { scale: 2 },
            jsPDF: { unit: 'mm', format: 'a4' }
        }).from(contenedorTemporal).save();

        for (const orden of ordenesArray) {
            await api.post(`/Ordenes/marcar-impresa/${orden.id}`);
        }
        if (listaProduccionRef.value) {
            await listaProduccionRef.value.cargarHistorial();
        }

        contenedorTemporal.remove();
        mensaje.value = "✅ Lote generado con éxito";
    } catch (e) {
        console.error(e);
        error.value = "Error al generar lote.";
    } finally {
        ocultarFormula.value = false;
        imprimiendoHistorial.value = false;
        
        setTimeout(() => {
            if (typeof limpiarFormulario === 'function') limpiarFormulario();
        }, 1000);
    }
};

onMounted(async () => {
    try {
        loading.value = true;
        
        // Magia: Todo encapsulado y limpio
        const [resProd, resCli, resInv] = await Promise.all([
            ProduccionAPI.obtenerProductos(),
            ProduccionAPI.obtenerClientes(),
            ProduccionAPI.obtenerInventarioCompleto()
        ]);
        
        if (Array.isArray(resProd)) {
            productos.value = resProd;
            listaTodasMateriasPrimas.value = productos.value.filter(p => p.esMateriaPrima);
        }
        if (Array.isArray(resCli)) clientes.value = resCli;
        if (Array.isArray(resInv)) listaInventarioCompleto.value = resInv;
        
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

            <div v-if="form.productoTerminadoId" class="caja-detalles-producto">
                
                <div v-if="listaLotesCliente.length > 0" class="box-fazon-selector">
                    <label style="color: #2ecc71;">♻️ Lote Recuperado (Fazón):</label>
                    <select v-model="loteFazonSeleccionadoId" @change="alCambiarLoteFazon" class="select-fazon">
                        <option disabled value="">-- Seleccionar Lote --</option>
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
                        >
                    </div>
                </div>

                <div style="display: flex; justify-content: space-between; align-items: flex-end; margin-top: 15px; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-bottom: 5px;">
                    <label class="lbl-sep" style="border: none; margin: 0 !important; padding: 0;">
                        Medidas: <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span><span v-else style="color:#2ecc71">(EDITABLES)</span>
                    </label>
                    <label class="check-container" style="margin: 0 !important; color: #3498db;">
                        <input type="checkbox" v-model="form.esBobina"> 🗞️ Formato Bobina
                    </label>
                </div>
                
                <div style="font-size:11px; color:#bbb; margin-top:-5px; margin-bottom:5px;">
                    <span v-if="limiteMaximo > 0">Rango: {{ limiteMinimo }} - {{ limiteMaximo }} mm</span>
                    <span v-else-if="limiteMinimo > 0">Mínimo: {{ limiteMinimo }} mm (Sin tope)</span>
                </div>

                <div class="fila-input">
                    <div v-if="!form.esBobina">
                        <label>Largo (mm)</label>
                        <input type="number" v-model="form.largo" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}">
                    </div>
                    
                    <div v-else>
                        <label style="color:#f39c12">Kilos x Bobina</label>
                        <input type="number" v-model="form.kilosPorBobina" step="0.1" style="border: 2px solid #f39c12; font-weight: bold; background: #fff3e0; color: #d35400;">
                    </div>
                    
                    <div>
                        <label>Ancho (mm)</label>
                        <input type="number" v-model="form.ancho" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}">
                    </div>
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
                
            </div> <div class="fila-input" style="margin-top:10px"><div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div></div>
            
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

.caja-detalles-producto {
    background-color: #34495e; /* Un tono más claro que el azul oscuro de atrás */
    padding: 15px;
    border-radius: 8px;
    margin-top: 15px;
    border: 1px solid #4a6278;
    box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);
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