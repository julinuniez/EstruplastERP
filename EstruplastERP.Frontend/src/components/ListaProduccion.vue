<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import api from '@/services/axiosInstance' 
import ModalCierreOrden from './ModalCierreOrden.vue' 
import ModalEdicionRapida from './ModalEdicionRapida.vue'
import ModalDetalleGrupo from './ModalDetalleGrupo.vue'
import ModalDesglosePallets from './ModalDesglosePallets.vue'
import { useConsolidacion } from '@/composables/useConsolidacion'
import { Alertas } from '@/utils/alertas'
import Swal from 'sweetalert2'

const emit = defineEmits(['imprimir-historial', 'imprimir-carga-consolidada', 'imprimir-lote-op'])

export interface ProduccionItem {
    id: number;
    fecha: string;
    producto: string;
    cantidad: number;
    kilos: number;
    desperdicio?: number;
    estado: string;
    esFinalizada: boolean;
    esImpreso?: boolean;
    notaPedido?: string;
    NotaPedido?: string;
    numeroPedidoCliente?: string;
    clienteNombre?: string; 
    cliente?: any;
    largo?: number;
    ancho?: number;
    espesor?: number;
    color?: string;
    conBrillo?: boolean;
    llevaFilm?: boolean;
    tipoCorona?: string;
    aditivoUV?: boolean;
    consumos?: any[];
    productoId?: number;
    clienteId?: number;
    observacion?: string;
    hojaCargaId?: number | null;
    pallets?: any[]; 
    imprimirEnPaquetes?: boolean;
}

const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)
const error = ref('')

const filtroEstado = ref('Pendientes')
const filtroLibre = ref('')
const filtroFecha = ref('')

const categoriaActiva = ref('TODOS')
const categoriasFiltro = [
  { id: 'TODOS', label: 'Todas las Órdenes' },
  { id: 'PAI', label: 'PAI' },
  { id: 'A.I. BICAPA', label: 'A.I. Bicapa' },
  { id: 'TRICAPA', label: 'Tricapa' },
  { id: 'FREON', label: 'Resist. Freón' },
  { id: 'ABS', label: 'ABS' },
  { id: 'PEAD', label: 'PEAD' },
  { id: 'PP', label: 'PP' }
]

const mostrarModalCierre = ref(false)
const ordenParaCerrar = ref<ProduccionItem | null>(null)
const materiasPrimas = ref<any[]>([]) 

const fechaActual = new Date()
const mesSeleccionado = ref(fechaActual.getMonth() + 1)
const anioSeleccionado = ref(fechaActual.getFullYear())

const listaMeses = [
    { id: 1, nombre: 'Enero' }, { id: 2, nombre: 'Febrero' }, { id: 3, nombre: 'Marzo' },
    { id: 4, nombre: 'Abril' }, { id: 5, nombre: 'Mayo' }, { id: 6, nombre: 'Junio' },
    { id: 7, nombre: 'Julio' }, { id: 8, nombre: 'Agosto' }, { id: 9, nombre: 'Septiembre' },
    { id: 10, nombre: 'Octubre' }, { id: 11, nombre: 'Noviembre' }, { id: 12, nombre: 'Diciembre' }
]

const nombreMesActual = computed(() => {
    return listaMeses.find(m => m.id === mesSeleccionado.value)?.nombre || ''
})

const listaAnios = computed(() => {
    const anios = []
    const anioBase = 2025 
    const anioTope = fechaActual.getFullYear() + 2
    for (let i = anioBase; i <= anioTope; i++) {
        anios.push(i)
    }
    return anios
})

const ordenesSeleccionadas = ref<number[]>([])

const mostrarModalEdicion = ref(false)
const ordenEditando = ref<ProduccionItem | null>(null)

const mostrarModalGrupo = ref(false)
const codigoGrupoSeleccionado = ref('')

const mostrarModalDesglose = ref(false)
const ordenParaDesglose = ref<ProduccionItem | null>(null)
const cantidadPalletsSugerida = ref<number>(1) 

const filasExpandidas = ref<number[]>([])

const { procesarConsolidacion } = useConsolidacion()

const toggleExpandir = (id: number) => {
    const index = filasExpandidas.value.indexOf(id)
    if (index === -1) filasExpandidas.value.push(id)
    else filasExpandidas.value.splice(index, 1)
}

function extraerCodigoHC(obs: string | undefined) {
    if (!obs) return null
    const match = obs.match(/\[Grupo: (HC-[^\]]+)\]/)
    return match ? match[1] : null
}

const getCodigoCarga = (p: ProduccionItem) => {
    const consolidado = extraerCodigoHC(p.observacion);
    if (consolidado) return consolidado;
    return `HC-S${p.id}`; 
}

const ordenesDelGrupo = computed(() => {
    if (!codigoGrupoSeleccionado.value) return []
    
    if (codigoGrupoSeleccionado.value.startsWith('HC-S')) {
        const idBuscado = parseInt(codigoGrupoSeleccionado.value.replace('HC-S', ''), 10);
        return producciones.value.filter(p => p.id === idBuscado);
    }
    
    return producciones.value.filter(p => (p.observacion || '').includes(codigoGrupoSeleccionado.value))
})

const abrirModalGrupo = (codigo: string | null | undefined) => {
    if (!codigo) return
    codigoGrupoSeleccionado.value = codigo
    mostrarModalGrupo.value = true
}

const getObservacionLimpia = (obs: string | undefined) => {
    if (!obs) return ''
    return obs.replace(/\[Grupo: HC-[^\]]+\]/g, '').replace(/\[LOTE: HC-[^\]]+\]/g, '').trim()
}

const produccionesFiltradas = computed(() => {
    return producciones.value.filter(item => {
        let pasaEstado = true
        if (filtroEstado.value === 'Pendientes') pasaEstado = item.estado === 'Pendiente' || item.estado === 'EnProceso' || item.estado === 'MaterialPreparado'
        else if (filtroEstado.value === 'Finalizadas') pasaEstado = item.estado === 'Finalizada'
        else if (filtroEstado.value === 'Canceladas') pasaEstado = item.estado === 'Cancelada'
        else if (filtroEstado.value === 'Todos' || filtroEstado.value === 'todos') pasaEstado = true

        if (filtroFecha.value && !item.fecha.startsWith(filtroFecha.value)) {
            return false
        }

        let pasaFiltroLibre = true
        if (filtroLibre.value.trim() !== '') {
            const busqueda = filtroLibre.value.toLowerCase().trim()
            const nomCliente = (item.clienteNombre || 'interno stock').toLowerCase()
            const notaPed = (item.notaPedido || '').toLowerCase()
            const ocCli = (item.numeroPedidoCliente || '').toLowerCase()
            const color = (item.color || '').toLowerCase()
            const prod = (item.producto || '').toLowerCase()
            const obs = (item.observacion || '').toLowerCase()
            
            pasaFiltroLibre = nomCliente.includes(busqueda) || notaPed.includes(busqueda) || 
                              ocCli.includes(busqueda) || color.includes(busqueda) || 
                              prod.includes(busqueda) || obs.includes(busqueda)
        }

        let pasaCategoria = true
        if (categoriaActiva.value !== 'TODOS') {
            const nombreProd = (item.producto || '').toUpperCase()
            switch (categoriaActiva.value) {
                case 'PAI': pasaCategoria = nombreProd.includes('A.I.') && (nombreProd.includes('FINO') || nombreProd.includes('GRUESO')); break;
                case 'A.I. BICAPA': pasaCategoria = nombreProd.includes('A.I.') && nombreProd.includes('BICAPA'); break;
                case 'TRICAPA': pasaCategoria = nombreProd.includes('TRICAPA'); break;
                case 'FREON': pasaCategoria = nombreProd.includes('FREON') || nombreProd.includes('FREÓN'); break;
                case 'ABS': pasaCategoria = nombreProd.includes('ABS'); break;
                case 'PEAD': pasaCategoria = nombreProd.includes('PEAD'); break;
                case 'PP': pasaCategoria = nombreProd.includes('PP') || nombreProd.includes('POLIPROPILENO'); break;
            }
        }

        return pasaEstado && pasaFiltroLibre && pasaCategoria
    })
})

async function cargarHistorial() {
    cargando.value = true
    error.value = ''
    ordenesSeleccionadas.value = []
    try {
        const res = await api.get(`/Ordenes/recientes?mes=${mesSeleccionado.value}&anio=${anioSeleccionado.value}`)
        if (Array.isArray(res.data)) {
            producciones.value = res.data.sort((a: any, b: any) => b.id - a.id)
        } else {
            error.value = "Error de conexión con el servidor."
        }
    } catch (e: any) {
        error.value = "No se pudieron cargar las órdenes."
    } finally {
        cargando.value = false
    }
}

async function cargarMateriasPrimas() {
    try {
        const res = await api.get('/Productos/materias-primas')
        materiasPrimas.value = res.data
    } catch (e) {
        console.error("Error al cargar materias primas", e)
    }
}

async function revertirOrden(item: ProduccionItem) {
    const mensaje = `Esto devolverá los materiales al stock y restará el producto terminado del inventario.`
    const confirmado = await Alertas.confirmar(`⚠️ ¿Revertir orden #${item.id}?`, mensaje)
    
    if (!confirmado) return

    try {
        await api.post(`/Ordenes/revertir/${item.id}`)
        await cargarHistorial()
    } catch (e: any) {
        Alertas.error("Error al revertir: " + (e.response?.data?.mensaje || e.message))
    }
}

const abrirModalDesglose = async (orden: ProduccionItem) => {
    let sugerencia = 1

    if (orden.kilos >= 1100) {
        const result = await Swal.fire({
            title: 'Dividir en Pallets',
            text: `Esta orden tiene ${orden.kilos} kg. ¿En cuántos pallets deseas dividirla?`,
            input: 'number',
            inputValue: Math.ceil(orden.kilos / 1000),
            showCancelButton: true,
            confirmButtonText: 'Continuar',
            cancelButtonText: 'Cancelar',
            inputValidator: (value) => {
                if (!value || parseInt(value) <= 0) {
                    return 'Debes ingresar un número válido mayor a 0'
                }
            }
        })

        if (!result.isConfirmed) return
        sugerencia = parseInt(result.value)
    }

    cantidadPalletsSugerida.value = sugerencia
    ordenParaDesglose.value = orden
    mostrarModalDesglose.value = true
}

const cerrarModalDesglose = () => {
    mostrarModalDesglose.value = false
    ordenParaDesglose.value = null
    cantidadPalletsSugerida.value = 1
}

const onDesgloseConfirmado = async (palletsCalculados: any[]) => {
    if (!ordenParaDesglose.value) return
    
    try {
        const idOrden = ordenParaDesglose.value.id
        const payload = palletsCalculados.map(p => ({
            numero: p.numero,
            kilos: p.kilos
        }))

        await api.post(`/Ordenes/${idOrden}/desglose`, payload)
        Alertas.exito("¡Desglose guardado con éxito!")
        cerrarModalDesglose()
        await cargarHistorial() 
        
    } catch (e: any) {
        console.error("Error en desglose:", e)
        Alertas.error("Error al guardar el desglose: " + (e.response?.data?.mensaje || e.message))
    }
}

const finalizarPalletAcordeon = async (palletId: number, numero: number) => {
    const confirmado = await Alertas.confirmar(
        "Confirmar Ingreso", 
        `¿Confirmás el ingreso a stock del Pallet N° ${numero}?\nSe descontará la materia prima proporcional y se sumará el producto terminado.`
    )
    
    if (!confirmado) return
    
    try {
        await api.post(`/Ordenes/finalizar-pallet/${palletId}`)
        await cargarHistorial() 
    } catch (e: any) {
        Alertas.error("Error al confirmar pallet: " + (e.response?.data?.mensaje || e.message))
    }
}

const abrirModalCierre = (orden: ProduccionItem) => {
    const ordenCorregida = { ...orden }
    if (ordenCorregida.fecha && ordenCorregida.fecha.length <= 12) {
        const partes = ordenCorregida.fecha.split(' ') 
        const diaMes = partes[0] || ''
        const hora = partes[1] || '00:00'
        
        if (diaMes && diaMes.includes('/')) {
            const [dia, mes] = diaMes.split('/')
            const anioFijo = anioSeleccionado.value 
            ordenCorregida.fecha = `${anioFijo}-${mes}-${dia}T${hora}`
        }
    }
    ordenParaCerrar.value = ordenCorregida
    mostrarModalCierre.value = true
}

const cerrarModalCierre = () => {
    mostrarModalCierre.value = false
    ordenParaCerrar.value = null
}

const onCierreConfirmado = () => {
    cargarHistorial()
}

async function cancelarOrden(item: ProduccionItem) {
    let titulo = `¿Cancelar Orden #${item.id}?`
    let mensaje = `Esto devolverá el material al inventario.`
    
    const tienePalletsFinalizados = item.pallets && item.pallets.some(p => p.estado === 'Finalizada')
    if (tienePalletsFinalizados) {
        titulo = "🚨 ATENCIÓN: Orden Parcialmente Ingresada"
        mensaje = `Esta orden tiene pallets ya ingresados a stock.\nAl cancelar, se RESTARÁ el producto terminado y se DEVOLVERÁ la materia prima al inventario.\n¿Confirmas la cancelación total?`
    }

    const confirmado = await Alertas.confirmar(titulo, mensaje)
    if (!confirmado) return

    try {
        await api.post(`/Ordenes/cancelar/${item.id}`)
        Alertas.exito("Orden cancelada y stock restaurado correctamente.")
        await cargarHistorial()
    } catch (e: any) {
        Alertas.error("Error: " + (e.response?.data?.mensaje || "No se pudo cancelar"))
    }
}

// 🚀 REIMPRESIÓN DESDE EL MODAL (CORREGIDA)
const manejarImpresionDesdeModal = async (codigo: string, ordenesGrupo: any[], consumosMezcla: any[]) => {
    if (!ordenesGrupo || ordenesGrupo.length === 0) return;

    const recetaInyectada = consumosMezcla && consumosMezcla.length > 0 ? consumosMezcla.map(c => {
        const kilosAImprimir = Number(c.real) > 0 ? Number(c.real) : Number(c.teorico);
        return {
            id: c.materiaPrimaId,
            materiaPrimaId: c.materiaPrimaId,
            MateriaPrimaId: c.materiaPrimaId,
            nombreMateriaPrima: c.nombre,
            nombreInsumo: c.nombre,
            cantidadKilos: kilosAImprimir, 
            CantidadKilos: kilosAImprimir, 
            kilos: kilosAImprimir,
            cantidad: kilosAImprimir,
            kilosFijos: kilosAImprimir 
        };
    }) : [];

    if (codigo.startsWith('HC-S')) {
        const ordenCopia = JSON.parse(JSON.stringify(ordenesGrupo[0]));
        ordenCopia.observacion = (ordenCopia.observacion ? ordenCopia.observacion + ' ' : '') + `[Grupo: HC-S${ordenCopia.id}] [FORZAR_CARGA]`;
        
        ordenCopia.consumos = recetaInyectada;
        ordenCopia.receta = recetaInyectada;
        
        emit('imprimir-historial', { 
            orden: ordenCopia, 
            tipo: 'carga', 
            receta: recetaInyectada, 
            materiasPrimasBase: materiasPrimas.value,
            imprimirEnPaquetes: false
        });
    } else {
        const nombresUnicos = [...new Set(ordenesGrupo.map(o => o.producto))];
        
        const notasArray = ordenesGrupo.map(o => {
            const n = o.notaPedido || o.NotaPedido;
            if (n && String(n).toLowerCase() !== 'undefined' && String(n).toLowerCase() !== 'null') {
                return String(n).trim();
            }
            return String(o.id);
        });
        const notasAgrupadas = [...new Set(notasArray)].join(' | ');
        
        const totalKilos = ordenesGrupo.reduce((acc, curr) => acc + (Number(curr.kilos) || 0), 0);
        const totalUnidades = ordenesGrupo.reduce((acc, curr) => acc + (Number(curr.cantidad) || 0), 0);

        const formObj = JSON.parse(JSON.stringify(ordenesGrupo[0]));
        
        formObj.productoNombre = nombresUnicos.length === 1 ? nombresUnicos[0] : "MEZCLA CONSOLIDADA";
        formObj.esConsolidado = true;
        formObj.cantidad = totalUnidades;
        formObj.kilosTotales = totalKilos;
        formObj.kilosEstimados = totalKilos;
        formObj.kilos = totalKilos;
        
        formObj.notaPedido = notasAgrupadas;
        formObj.NotaPedido = notasAgrupadas;
        
        formObj.observacion = `[Grupo: ${codigo}] [MEZCLA CONSOLIDADA] [FORZAR_CARGA]`;
        formObj.producto = ordenesGrupo[0].producto || { id: ordenesGrupo[0].productoId || 9999, nombre: "MEZCLA", pesoEspecifico: 0, codigoSku: 'MEZCLA' };
        formObj.cliente = ordenesGrupo[0].cliente || { id: ordenesGrupo[0].clienteId || 1, razonSocial: 'MÚLTIPLE' };
        
        formObj.consumos = recetaInyectada;
        formObj.receta = recetaInyectada;

        const payloadImpresion = {
            form: formObj,
            orden: formObj,
            receta: recetaInyectada,
            tipo: 'carga',
            materiasPrimasBase: materiasPrimas.value,
            imprimirEnPaquetes: false
        };

        emit('imprimir-carga-consolidada', payloadImpresion);
    }
    mostrarModalGrupo.value = false;
}

// 🚀 FUNCIÓN AYUDANTE: Limpia puntos huérfanos e inyecta alertas sin duplicar
function inyectarAlertasFabrica(orden: any) {
    // 1. Limpiamos la observación de puntos huérfanos, guiones o espacios vacíos
    let obsText = (orden.observacion || '').trim();
    if (obsText === '.' || obsText === '-' || obsText === '.-' || obsText === 'undefined' || obsText === 'null') {
        obsText = '';
    }

    // 2. ALERTA NYLON (10 a 199 -> 10 | >= 200 -> 20)
    if (!orden.esBobina && orden.cantidad >= 10) {
        const divisor = orden.cantidad >= 200 ? 20 : 10;
        // Texto limpio sin emojis para evitar símbolos raros en html2pdf
        const alertaNylon = `SEPARAR CADA ${divisor} LÁMINAS CON UN NYLON.`;
        
        if (!String(orden.observacion || '').includes("SEPARAR CADA")) {
            if (obsText && !obsText.endsWith('.')) obsText += '.';
            obsText = obsText ? `${obsText}\n\n${alertaNylon}` : alertaNylon;
        }
    }

    // 3. ALERTA CORONA
    if (orden.tipoCorona && orden.tipoCorona !== 'Ninguno') {
        const alertaPrueba = "COLOCAR LÁMINA DE PRUEBA ARRIBA.";
        if (!String(orden.observacion || '').includes("LÁMINA DE PRUEBA")) {
            if (obsText && !obsText.endsWith('.')) obsText += '.';
            obsText = obsText ? `${obsText}\n\n${alertaPrueba}` : alertaPrueba;
        }
    }

    orden.observacion = obsText;
    return orden;
}

const solicitarImpresion = async (orden: ProduccionItem, tipo: 'orden' | 'carga') => {
    const ordenCopia = { ...orden };

    if (tipo === 'carga') {
        ordenCopia.observacion = (ordenCopia.observacion ? ordenCopia.observacion + ' ' : '') + `[Grupo: HC-S${orden.id}] [FORZAR_CARGA]`;
        
        emit('imprimir-historial', { 
            orden: ordenCopia, 
            tipo: 'carga', 
            materiasPrimasBase: materiasPrimas.value,
            imprimirEnPaquetes: false
        });
        return; 
    }

    // 🚀 INYECTAR ALERTAS SIN DUPLICADOS
    inyectarAlertasFabrica(ordenCopia);

    if (ordenCopia.esImpreso) {
        const confirmado = await Alertas.confirmar(
            "Orden ya impresa", 
            `La orden #${ordenCopia.id} ya fue impresa. ¿Seguro quieres reimprimirla?`
        )
        if (!confirmado) return
    }

    let enPaquetes = false;
    if (ordenCopia.cantidad >= 10) {
        const divisorMsg = ordenCopia.cantidad >= 200 ? 20 : 10;
        const resp = await Swal.fire({
            title: '¿Imprimir en Paquetes?',
            text: `¿Deseas imprimir las etiquetas de OP divididas en paquetes de ${divisorMsg}?`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: '📦 Sí, en paquetes',
            cancelButtonText: '📄 No, impresión normal',
            confirmButtonColor: '#3498db',
            cancelButtonColor: '#7f8c8d'
        });
        enPaquetes = resp.isConfirmed;
    }

    ordenCopia.imprimirEnPaquetes = enPaquetes;

    emit('imprimir-historial', { 
        orden: ordenCopia, 
        tipo: 'orden', 
        materiasPrimasBase: materiasPrimas.value,
        imprimirEnPaquetes: enPaquetes 
    });
}

function toggleSeleccionMultiple(id: number) {
    const index = ordenesSeleccionadas.value.indexOf(id)
    if (index === -1) {
        ordenesSeleccionadas.value.push(id)
    } else {
        ordenesSeleccionadas.value.splice(index, 1)
    }
}

async function imprimirLoteOP() {
    if (ordenesSeleccionadas.value.length === 0) return
    
    const ordenesBase = producciones.value.filter(p => ordenesSeleccionadas.value.includes(p.id))
    
    const yaImpresas = ordenesBase.filter(o => o.esImpreso).length
    if (yaImpresas > 0) {
        const msj = yaImpresas === 1 
            ? "Hay 1 orden seleccionada que ya fue impresa. ¿Seguro quieres reimprimirla?" 
            : `Hay ${yaImpresas} órdenes seleccionadas que ya fueron impresas. ¿Seguro quieres reimprimirlas?`
        
        const confirmado = await Alertas.confirmar("Órdenes ya impresas", msj)
        if (!confirmado) return
    }

    const ordenesAImprimir = ordenesBase.map(orden => {
        const copia = { ...orden };
        // 🚀 INYECTAR ALERTAS SIN DUPLICADOS
        inyectarAlertasFabrica(copia);
        copia.imprimirEnPaquetes = false; 
        return copia;
    });

    emit('imprimir-lote-op', ordenesAImprimir)
    ordenesSeleccionadas.value = []
}

// 🚀 CREAR HOJA NUEVA DESDE LA BANDEJA (CORREGIDA)
async function ejecutarCargaConsolidada() {
    if (ordenesSeleccionadas.value.length < 2) return
    const ordenesAImprimir = producciones.value.filter(p => ordenesSeleccionadas.value.includes(p.id))
    
    const payload = await procesarConsolidacion(ordenesAImprimir)
    
    if (payload) {
        const nombresUnicos = [...new Set(ordenesAImprimir.map(o => o.producto))]
        
        const payloadCrudo = (payload as any).form ? (payload as any).form : payload;
        const formObj = JSON.parse(JSON.stringify(payloadCrudo));
        
        const recetaCalculada = (payload as any).receta || []

        const notasArray = ordenesAImprimir.map(o => {
            const n = o.notaPedido || o.NotaPedido;
            if (n && String(n).toLowerCase() !== 'undefined' && String(n).toLowerCase() !== 'null') {
                return String(n).trim();
            }
            return String(o.id);
        });
        const notasAgrupadas = [...new Set(notasArray)].join(' | ');

        if (nombresUnicos.length === 1) {
            formObj.productoNombre = nombresUnicos[0]
        } else {
            formObj.productoNombre = "MEZCLA CONSOLIDADA"
        }

        formObj.esConsolidado = true
        formObj.observacion = '[MEZCLA CONSOLIDADA] ' + (formObj.observacion || '') + ' [FORZAR_CARGA]'
        
        formObj.cantidad = ordenesAImprimir.reduce((acc, curr) => acc + (Number(curr.cantidad) || 0), 0);
        formObj.kilosTotales = ordenesAImprimir.reduce((acc, curr) => acc + (Number(curr.kilos) || 0), 0);
        formObj.kilosEstimados = formObj.kilosTotales;
        formObj.kilos = formObj.kilosTotales;

        formObj.notaPedido = notasAgrupadas;
        formObj.NotaPedido = notasAgrupadas;

        formObj.consumos = recetaCalculada;
        formObj.receta = recetaCalculada;

        const payloadImpresion = {
            form: formObj, 
            orden: formObj,
            receta: recetaCalculada,
            tipo: 'carga',
            materiasPrimasBase: materiasPrimas.value,
            imprimirEnPaquetes: false
        };

        emit('imprimir-carga-consolidada', payloadImpresion)
        
        ordenesSeleccionadas.value = []
        await cargarHistorial()
    }
}

const abrirModalEdicion = (orden: ProduccionItem) => {
    ordenEditando.value = orden
    mostrarModalEdicion.value = true
}

const onEdicionGuardada = () => {
    mostrarModalEdicion.value = false
    cargarHistorial()
}

watch([mesSeleccionado, anioSeleccionado], () => { cargarHistorial() })

onMounted(() => {
    cargarHistorial()
    cargarMateriasPrimas() 
})

defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3 class="titulo-bandeja">
            {{ filtroEstado === 'Pendientes' ? '🔥 Bandeja de Producción Activa' : `🗄️ Histórico de ${nombreMesActual} ${anioSeleccionado}` }}
        </h3>
        <div class="filtros-container">
            <input 
                type="text" 
                v-model="filtroLibre" 
                class="input-filtro input-buscador" 
                placeholder="🔍 Buscar Cliente, Nota, Producto..."
            >
            <input type="date" v-model="filtroFecha" class="input-filtro">
            <select v-model="filtroEstado" class="input-filtro">
                <option value="Pendientes">🔥 En Producción</option>
                <option value="Finalizadas">✅ Finalizadas</option>
                <option value="Canceladas">❌ Canceladas</option>
                <option value="Todos">📁 Todas</option>
            </select>
            
            <div class="grupo-filtro-tiempo" v-show="filtroEstado !== 'Pendientes'">
                <label>📅</label>
                <select v-model="mesSeleccionado" class="select-mes">
                    <option v-for="m in listaMeses" :key="m.id" :value="m.id">{{ m.nombre }}</option>
                </select>
                <select v-model="anioSeleccionado" class="select-anio">
                    <option v-for="a in listaAnios" :key="a" :value="a">{{ a }}</option>
                </select>
            </div>
            
            <button @click="cargarHistorial" class="btn-refresh" :disabled="cargando" title="Actualizar datos">
                {{ cargando ? '⏳' : '🔄' }}
            </button>
        </div>
    </div>

    <div class="filtros-produccion">
        <button 
            v-for="cat in categoriasFiltro" 
            :key="cat.id"
            @click="categoriaActiva = cat.id"
            :class="['btn-filtro', { 'activo': categoriaActiva === cat.id }]"
        >
            {{ cat.label }}
        </button>
    </div>

    <div v-if="cargando" class="loading">Cargando...</div>
    <div v-else-if="error" class="error-msg">{{ error }}</div>

    <div class="tabla-scroll">
        <table class="tabla-custom">
            <thead>
                <tr>
                    <th style="width: 30px; text-align: center;">✓</th>
                    <th style="width: 25px;">Fecha</th>
                    <th style="width: 95px;">Cliente</th>
                    <th style="width: 50px;">N° Pedido</th>
                    <th>Producto</th>
                    <th style="width: 50px; text-align: center;">Cant.</th>
                    <th style="width: 80px; text-align: right;">Kilos</th>
                    <th style="width: 125px; text-align: center;">Estado</th>
                    <th style="width: 230px; text-align: center;">Acciones</th>
                </tr>
            </thead>
            <tbody>
                <template v-for="p in produccionesFiltradas" :key="p.id">
                    <tr :class="{'fila-impresa': p.esImpreso && p.estado !== 'Finalizada' && p.estado !== 'Cancelada', 'fila-no-impresa': !p.esImpreso && p.estado !== 'Finalizada' && p.estado !== 'Cancelada', 'fila-ok': p.estado === 'Finalizada', 'fila-cancel': p.estado === 'Cancelada', 'fila-seleccionada': ordenesSeleccionadas.includes(p.id)}">
                        
                        <td style="text-align: center; vertical-align: middle;">
                            <input type="checkbox" :checked="ordenesSeleccionadas.includes(p.id)" @change="toggleSeleccionMultiple(p.id)" v-if="p.estado === 'Pendiente' || p.estado === 'EnProceso' || p.estado === 'MaterialPreparado'" class="check-orden">
                        </td>

                        <td class="td-fecha">{{ p.fecha }}</td>
                        
                        <td>
                            <span class="badge-cliente">{{ p.clienteNombre && p.clienteNombre !== 'Desconocido' ? p.clienteNombre : 'Interno / Stock' }}</span>
                        </td>
                        
                        <td>
                            <div class="texto-nota">{{ p.notaPedido || '-' }}</div>
                            <small v-if="p.numeroPedidoCliente" class="texto-oc">OC: {{ p.numeroPedidoCliente }}</small>
                            <div class="tag-hc clickeable" title="Ver detalle de hoja de carga" @click.stop="abrirModalGrupo(getCodigoCarga(p))">
                                📦 {{ getCodigoCarga(p) }}
                            </div>
                        </td>
                        
                        <td class="td-prod">
                            <span class="prod-nombre">{{ p.producto }}</span>
                            <div v-if="p.color || p.conBrillo || p.llevaFilm || p.aditivoUV || (p.tipoCorona && p.tipoCorona !== 'Ninguno')" class="tags-produccion">
                                <span v-if="p.color" class="tag-color">🎨 {{ p.color.toUpperCase() }}</span>
                                <span v-if="p.conBrillo" class="tag-extra">✨ Brillo</span>
                                <span v-if="p.llevaFilm" class="tag-extra">🛡️ Film</span>
                                <span v-if="p.aditivoUV" class="tag-extra" style="background:#fef3c7; color:#d97706; border-color:#fde68a;">☀️ UV</span>
                                <span v-if="p.tipoCorona && p.tipoCorona !== 'Ninguno'" class="tag-extra">⚡ Corona {{ p.tipoCorona }}</span>
                            </div>
                            <div v-if="getObservacionLimpia(p.observacion)" class="nota-operario" title="Nota de Producción">
                                💬 <i>"{{ getObservacionLimpia(p.observacion) }}"</i>
                            </div>

                            <div v-if="p.pallets && p.pallets.length > 0" style="margin-top: 8px;">
                                <button @click="toggleExpandir(p.id)" class="btn-desplegar-pallets">
                                    {{ filasExpandidas.includes(p.id) ? '🔽 Ocultar Pallets' : '▶️ Mostrar Pallets' }} 
                                    <span class="badge-mini-pallets">{{ p.pallets.filter(x => x.estado === 'Finalizada').length }} / {{ p.pallets.length }} listos</span>
                                </button>
                            </div>
                        </td>

                        <td style="text-align: center; font-weight: 500;">{{ p.cantidad }}</td>
                        
                        <td style="text-align: right; font-weight: bold; color: #2c3e50;">{{ Math.round(p.kilos) }}</td>
                        
                        <td style="text-align: center;">
                            <span :class="{'badge-pend': p.estado === 'Pendiente' || p.estado === 'EnProceso', 'badge-prep': p.estado === 'MaterialPreparado', 'badge-ok': p.estado === 'Finalizada', 'badge-cancel': p.estado === 'Cancelada'}">
                                {{ p.estado === 'Cancelada' ? 'CANCELADA' : (p.estado === 'Finalizada' ? 'FINALIZADA' : (p.estado === 'MaterialPreparado' ? 'MATERIAL LISTO' : 'EN MÁQUINA')) }}
                            </span>
                        </td>
                        
                        <td class="td-acciones">
                            <div class="acciones-wrapper">
                                <template v-if="p.estado === 'Pendiente' || p.estado === 'EnProceso' || p.estado === 'MaterialPreparado'">
                                    <button @click="abrirModalEdicion(p)" class="btn-action" title="Modificar Orden">✏️</button>
                                    
                                    <button v-if="(!p.pallets || p.pallets.length === 0) && p.kilos >= 1100" @click="abrirModalDesglose(p)" class="btn-action btn-desglose" title="Desglosar en Pallets">📦</button>
                                    
                                    <button v-if="!p.pallets || p.pallets.length === 0" @click="abrirModalCierre(p)" class="btn-action btn-check" title="Declarar Consumos y Cerrar OP">✅</button>
                                    
                                    <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Imprimir OP">📄</button>
                                    <button @click="solicitarImpresion(p, 'carga')" class="btn-action btn-ciencia" title="Imprimir Hoja de Carga Individual">🧪</button>
                                    <button @click="cancelarOrden(p)" class="btn-action btn-cancel" title="Cancelar y Devolver Material">❌</button>
                                </template>
                                <template v-else-if="p.estado === 'Finalizada'">
                                    <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Reimprimir OP Finalizada">📄</button>
                                    <button @click="revertirOrden(p)" class="btn-action" style="color: #e67e22; border-color: #e67e22;" title="Revertir Cierre de Producción">⏪</button>
                                </template>
                            </div>
                        </td>
                    </tr>

                    <tr v-if="filasExpandidas.includes(p.id) && p.pallets && p.pallets.length > 0" class="fila-acordeon">
                        <td colspan="9" style="padding: 0; background: transparent;">
                            <div class="acordeon-caja">
                                <table class="tabla-pallets-interna">
                                    <thead>
                                        <tr>
                                            <th style="width: 100px;">N° Pallet</th>
                                            <th style="width: 120px;">Kilos Físicos</th>
                                            <th style="width: 120px;">Estado</th>
                                            <th>Acción de Fábrica</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr v-for="pallet in p.pallets" :key="pallet.id" :class="{'pallet-ok': pallet.estado === 'Finalizada'}">
                                            <td><strong>Pallet {{ pallet.numeroPallet }}</strong></td>
                                            <td>{{ pallet.kilos }} kg</td>
                                            <td>
                                                <span v-if="pallet.estado === 'Finalizada'" style="color: #10b981; font-weight: bold;">✔️ Descontado</span>
                                                <span v-else style="color: #d97706; font-weight: bold;">⏳ Pendiente</span>
                                            </td>
                                            <td>
                                                <button v-if="pallet.estado !== 'Finalizada'" @click="finalizarPalletAcordeon(pallet.id, pallet.numeroPallet)" class="btn-baja-pallet">
                                                    ✅ Ingresar y dar de baja
                                                </button>
                                                <span v-else class="texto-pallet-cerrado">Stock actualizado automáticamente</span>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </template>
                
                <tr v-if="produccionesFiltradas.length === 0">
                    <td colspan="9" class="vacio">No hay órdenes que coincidan con los filtros seleccionados.</td>
                </tr>
            </tbody>
        </table>
        <div v-if="cargando" class="loading-overlay"><div class="spinner"></div> Cargando datos...</div>
    </div>

    <div v-if="ordenesSeleccionadas.length > 0" class="barra-flotante-consolidada">
        <div class="resumen-seleccion">
            <span class="badge-count">{{ ordenesSeleccionadas.length }}</span> órdenes seleccionadas
        </div>
        <div class="botones-flotantes">
            <button class="btn-consolidado btn-op" @click="imprimirLoteOP">
                📄 Imprimir OP x {{ ordenesSeleccionadas.length }}
            </button>
            <button class="btn-consolidado" @click="ejecutarCargaConsolidada" v-if="ordenesSeleccionadas.length > 1">
                🧪 Imprimir Hoja de Carga (Mezcla)
            </button>
        </div>
    </div>

    <ModalDesglosePallets 
        :visible="mostrarModalDesglose"
        :orden="ordenParaDesglose"
        :sugerenciaPallets="cantidadPalletsSugerida" 
        @close="cerrarModalDesglose"
        @guardar="onDesgloseConfirmado"
    />

    <ModalDetalleGrupo
        :visible="mostrarModalGrupo"
        :codigo="codigoGrupoSeleccionado"
        :ordenes="ordenesDelGrupo"
        @close="mostrarModalGrupo = false"
        @actualizar-lista="cargarHistorial"
        @imprimir-carga="manejarImpresionDesdeModal"
    />

    <ModalEdicionRapida
        :visible="mostrarModalEdicion"
        :ordenEditando="ordenEditando"
        @close="mostrarModalEdicion = false"
        @guardado="onEdicionGuardada"
    />

    <ModalCierreOrden 
        :visible="mostrarModalCierre"
        :orden="ordenParaCerrar"
        :materiasPrimas="materiasPrimas"
        @close="cerrarModalCierre"
        @confirmar="onCierreConfirmado"
    />
  </div>
</template>

<style scoped>
.historial-wrapper { background: #ffffff; padding: 20px; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); height: 100%; display: flex; flex-direction: column; position: relative; overflow: hidden; }
.header-tabla { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; border-bottom: 2px solid #3498db; padding-bottom: 10px; flex-wrap: wrap; gap: 10px; }
.titulo-bandeja { margin: 0; font-size: 1.2rem; color: #2c3e50; font-weight: 700; }
.filtros-container { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
.input-filtro { padding: 6px 12px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.9rem; background: #f8fafc; cursor: pointer; color: #334155; font-weight: 500; transition: border-color 0.2s; }
.input-filtro:focus { border-color: #3498db; outline: none; }
.input-buscador { width: 220px; cursor: text; } 
.btn-refresh { background: white; border: 1px solid #cbd5e1; border-radius: 6px; cursor: pointer; padding: 6px 12px; font-size: 1rem; transition: all 0.2s; }
.btn-refresh:hover { background: #f1f5f9; border-color: #94a3b8; }

.grupo-filtro-tiempo { display: flex; align-items: center; gap: 8px; background: #f8fafc; padding: 4px 12px; border-radius: 6px; border: 1px solid #cbd5e1; }
.grupo-filtro-tiempo label { font-weight: bold; color: #475569; font-size: 0.9rem; margin: 0; }
.select-mes, .select-anio { padding: 4px; border: 1px solid #cbd5e1; border-radius: 4px; font-weight: 500; color: #1e293b; outline: none; background: white; cursor: pointer; }
.select-mes { min-width: 110px; }

.filtros-produccion { display: flex; gap: 10px; margin-bottom: 15px; flex-wrap: wrap; }
.btn-filtro { background-color: #f1f5f9; color: #64748b; border: 1px solid #cbd5e1; padding: 8px 16px; border-radius: 20px; font-weight: 600; font-size: 0.9rem; cursor: pointer; transition: all 0.2s ease; }
.btn-filtro:hover { background-color: #e2e8f0; color: #334155; }
.btn-filtro.activo { background-color: #3b82f6; color: white; border-color: #3b82f6; box-shadow: 0 2px 4px rgba(59, 130, 246, 0.3); }

.tabla-scroll { overflow-y: auto; flex: 1; margin-bottom: 55px; border-radius: 6px; border: 1px solid #e2e8f0; }
.tabla-custom { width: 100%; border-collapse: separate; border-spacing: 0; font-size: 0.85rem; }
.tabla-custom th { background: #f8fafc; color: #475569; padding: 12px 10px; text-align: left; position: sticky; top: 0; z-index: 5; font-weight: 700; border-bottom: 2px solid #e2e8f0; text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.5px; }
.tabla-custom td { padding: 10px; border-bottom: 1px solid #f1f5f9; color: #334155; vertical-align: middle; transition: background-color 0.2s; }

tr.fila-impresa td { background-color: #d4edda; }
tr.fila-no-impresa td { background-color: #f8d7da; }
tr.fila-impresa:hover td { background-color: #c3e6cb; }
tr.fila-no-impresa:hover td { background-color: #f5c6cb; }

.tabla-custom tbody tr:hover td { background-color: #f8fafc; }
.check-orden { transform: scale(1.3); cursor: pointer; accent-color: #3498db; }
.td-fecha { color: #64748b; font-size: 0.8rem; }

.td-prod { font-weight: 700; color: #1e293b; line-height: 1.1; vertical-align: middle; }
.prod-nombre { display: block; font-size: 0.85rem; margin-bottom: 4px; }
.tags-produccion { display: flex; gap: 4px; flex-wrap: wrap; margin-top: 4px; }
.tag-color { background: #f1f5f9; border: 1px solid #cbd5e1; color: #334155; font-size: 0.6rem; padding: 1px 4px; border-radius: 3px; font-weight: 800; letter-spacing: 0.5px; }
.tag-extra { background: #fffbeb; border: 1px solid #fde68a; color: #b45309; font-size: 0.6rem; padding: 1px 4px; border-radius: 3px; font-weight: 700; }

.badge-cliente { background-color: #e0f2fe; color: #0369a1; padding: 4px 8px; border-radius: 4px; font-weight: 600; font-size: 0.75rem; display: inline-block; max-width: 140px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.texto-nota { font-weight: 700; color: #334155; font-size: 0.9rem; }
.texto-oc { color: #94a3b8; display: block; font-size: 0.7rem; margin-top: 2px; }
.fila-ok td { background-color: #fbfdfc; color: #94a3b8; }
.fila-ok .td-prod { text-decoration: line-through; color: #94a3b8; } 
.fila-cancel td { background-color: #fffbfa; color: #94a3b8; }
.fila-cancel .td-prod { text-decoration: line-through; color: #ef4444; }
.fila-seleccionada td { background-color: #fffbeb !important; } 
.badge-pend { background: #fff7ed; color: #d97706; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fcd34d; box-shadow: 0 0 5px rgba(217, 119, 6, 0.2); white-space: nowrap;}
.badge-ok { background: #ecfdf5; color: #10b981; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #a7f3d0; white-space: nowrap;}
.badge-cancel { background: #fef2f2; color: #ef4444; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fecaca; white-space: nowrap;}
.badge-prep { background: #eff6ff; color: #3b82f6; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #93c5fd; white-space: nowrap;}

.td-acciones { vertical-align: middle; padding: 6px 10px !important; }
.acciones-wrapper { display: flex; gap: 6px; justify-content: center; align-items: center; width: 100%; max-width: 230px; margin: 0 auto; }
.btn-action { flex: 1; max-width: 38px; min-width: 30px; height: 32px; border: 1px solid #cbd5e1; background: white; border-radius: 6px; cursor: pointer; font-size: 1.05rem; display: flex; align-items: center; justify-content: center; transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); padding: 0; }
.btn-action:hover { transform: translateY(-2px); box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); background: #f8fafc; }
.btn-check:hover { background: #f0fdf4; border-color: #6ee7b7; }
.btn-ciencia:hover { background: #eff6ff; border-color: #93c5fd; }
.btn-desglose:hover { background: #f8fafc; border-color: #94a3b8; }
.btn-cancel { color: #ef4444; }
.btn-cancel:hover { background: #fef2f2; border-color: #fca5a5; }

.vacio { text-align: center; padding: 40px; color: #94a3b8; font-style: italic; font-size: 0.9rem; }
.loading-overlay { position: absolute; top: 0; left: 0; width: 100%; height: 100%; display: flex; align-items: center; justify-content: center; flex-direction: column; background: rgba(255,255,255,0.9); z-index: 10; color: #3498db; font-weight: bold; font-size: 1.2rem; }
.barra-flotante-consolidada { position: absolute; bottom: 0; left: 0; width: 100%; background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%); padding: 12px 25px; display: flex; justify-content: space-between; align-items: center; box-shadow: 0 -4px 15px rgba(0,0,0,0.2); z-index: 20; }
.resumen-seleccion { color: white; font-size: 0.9rem; display: flex; align-items: center; gap: 10px; }
.badge-count { background-color: #f1c40f; color: #1e293b; padding: 4px 12px; border-radius: 20px; font-weight: 900; font-size: 1rem; box-shadow: 0 2px 4px rgba(0,0,0,0.2); }
.botones-flotantes { display: flex; gap: 12px; }
.btn-consolidado { background-color: #8b5cf6; color: white; border: none; padding: 10px 20px; border-radius: 8px; font-weight: 700; font-size: 1rem; cursor: pointer; transition: all 0.2s; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.btn-consolidado:hover { background-color: #7c3aed; transform: translateY(-2px); box-shadow: 0 6px 12px rgba(0,0,0,0.2); }
.btn-op { background-color: #3498db; }
.btn-op:hover { background-color: #2980b9; }
.spinner { border: 4px solid #f3f3f3; border-top: 4px solid #3498db; border-radius: 50%; width: 30px; height: 30px; animation: spin 1s linear infinite; margin-bottom: 10px; }
@keyframes spin { 0% { transform: rotate(0deg); } 100% { transform: rotate(360deg); } }

.tag-hc { background: #8b5cf6; color: white; display: inline-block; font-size: 0.65rem; padding: 2px 6px; border-radius: 4px; font-weight: bold; margin-top: 4px; white-space: nowrap; }
.tag-hc.clickeable { cursor: pointer; transition: transform 0.2s; }
.tag-hc.clickeable:hover { transform: scale(1.05); background: #7c3aed; }
.nota-operario { 
    font-size: 0.75rem; 
    color: #92400e; 
    background: #fef3c7; 
    padding: 4px 8px; 
    border-radius: 4px; 
    margin-top: 6px; 
    border-left: 3px solid #f59e0b; 
    display: inline-block;
    max-width: 100%;
    word-wrap: break-word;
}

.btn-desplegar-pallets { background: #e0f2fe; color: #0284c7; border: 1px solid #bae6fd; padding: 4px 8px; border-radius: 6px; font-size: 0.75rem; font-weight: bold; cursor: pointer; transition: background 0.2s; display: inline-flex; align-items: center; gap: 6px; }
.btn-desplegar-pallets:hover { background: #bae6fd; }
.badge-mini-pallets { background: #0284c7; color: white; padding: 1px 5px; border-radius: 4px; font-size: 0.65rem; }

.fila-acordeon { background: #f8fafc !important; }
.acordeon-caja { padding: 10px 20px 20px 40px; border-left: 4px solid #3b82f6; background: #f1f5f9; box-shadow: inset 0 2px 4px rgba(0,0,0,0.02); }
.tabla-pallets-interna { width: 100%; border-collapse: collapse; font-size: 0.8rem; background: white; border-radius: 6px; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
.tabla-pallets-interna th { background: #e2e8f0; padding: 8px 12px; text-align: left; color: #475569; border-bottom: 1px solid #cbd5e1; }
.tabla-pallets-interna td { padding: 8px 12px; border-bottom: 1px solid #f1f5f9; }
.pallet-ok td { background: #f0fdf4; color: #64748b; }
.btn-baja-pallet { background: #10b981; color: white; border: none; padding: 4px 10px; border-radius: 4px; font-weight: bold; cursor: pointer; transition: background 0.2s; }
.btn-baja-pallet:hover { background: #059669; }
.texto-pallet-cerrado { color: #94a3b8; font-style: italic; font-size: 0.75rem; }
</style>