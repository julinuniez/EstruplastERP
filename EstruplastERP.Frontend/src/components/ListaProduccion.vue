<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/axiosInstance' 
// 👇 1. IMPORTAMOS EL NUEVO MODAL
import ModalCierreOrden from './ModalCierreOrden.vue' 

const emit = defineEmits(['imprimir-historial', 'imprimir-carga-consolidada', 'imprimir-lote-op']);

interface ProduccionItem {
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
    consumos?: any[];
    productoId?: number;
    clienteId?: number;
    observacion?: string;
}

const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)
const error = ref('')

const filtroEstado = ref('Pendientes'); 
const filtroFecha = ref(''); 
const filtroLibre = ref(''); 

const ordenesSeleccionadas = ref<number[]>([]);

// Estados para Edición
const mostrarModalEdicion = ref(false);
const guardandoEdicion = ref(false);
const ordenEditando = ref<ProduccionItem | null>(null);

// 👇 2. ESTADOS PARA EL NUEVO MODAL DE CIERRE
const mostrarModalCierre = ref(false);
const ordenParaCerrar = ref<ProduccionItem | null>(null);
const materiasPrimas = ref<any[]>([]); // Inventario para adiciones extra

const formEdicion = ref({
    largo: 0,
    ancho: 0,
    espesor: 0,
    cantidad: 0,
    kilosTotales: 0,
    desperdicio: 8,
    conBrillo: false,
    llevaFilm: false,
    tipoCorona: 'Ninguno',
    color: '',
    recetaNueva: [] as any[]
});

const produccionesFiltradas = computed(() => {
    return producciones.value.filter(item => {
        let pasaEstado = true;
        if (filtroEstado.value === 'Pendientes') pasaEstado = item.estado === 'Pendiente' || item.estado === 'EnProceso';
        else if (filtroEstado.value === 'Finalizadas') pasaEstado = item.estado === 'Finalizada';
        else if (filtroEstado.value === 'Canceladas') pasaEstado = item.estado === 'Cancelada';
        else if (filtroEstado.value === 'Todos') pasaEstado = true;

        let pasaFecha = true;
        if (filtroFecha.value) {
            const [year, month, day] = filtroFecha.value.split('-');
            const fechaBuscada = `${day}/${month}`; 
            pasaFecha = item.fecha.startsWith(fechaBuscada);
        }

        let pasaFiltroLibre = true;
        if (filtroLibre.value.trim() !== '') {
            const busqueda = filtroLibre.value.toLowerCase().trim();
            const nomCliente = (item.clienteNombre || 'interno stock').toLowerCase();
            const notaPed = (item.notaPedido || '').toLowerCase();
            const ocCli = (item.numeroPedidoCliente || '').toLowerCase();
            
            pasaFiltroLibre = nomCliente.includes(busqueda) || 
                              notaPed.includes(busqueda) || 
                              ocCli.includes(busqueda);
        }

        return pasaEstado && pasaFecha && pasaFiltroLibre;
    });
});

async function cargarHistorial() {
    cargando.value = true
    error.value = ''
    ordenesSeleccionadas.value = [];
    try {
        const res = await api.get('/Ordenes/recientes')
        if (Array.isArray(res.data)) {
            producciones.value = res.data.sort((a: any, b: any) => b.id - a.id);
        } else {
            error.value = "Error de conexión con el servidor.";
        }
    } catch (e: any) {
        error.value = "No se pudieron cargar las órdenes."
    } finally {
        cargando.value = false
    }
}

async function cargarMateriasPrimas() {
    try {
        const res = await api.get('/Productos/materiasprimas');
        materiasPrimas.value = res.data;
    } catch (e) {
        console.error("Error al cargar materias primas", e);
    }
}

async function activarOrden(item: ProduccionItem) {
    try {
        await api.post(`/Ordenes/activar/${item.id}`)
        item.estado = "Pendiente"; 
    } catch (e: any) {
        const msj = e.response?.data?.mensaje || e.response?.data || "Error de conexión";
        alert("❌ " + msj); 
    }
}

// 👇 FUNCIÓN DE REVERSIÓN
async function revertirOrden(item: ProduccionItem) {
    const mensaje = item.estado === 'Finalizada' 
        ? `⚠️ PELIGRO: Vas a revertir la orden #${item.id} a Pendiente.\n\nEsto devolverá los materiales al stock y restará el producto terminado del inventario.\n¿Estás completamente seguro?`
        : `¿Quitar la marca de "Impresa" de la orden #${item.id}?`;

    if (!confirm(mensaje)) return;

    try {
        await api.post(`/Ordenes/revertir/${item.id}`);
        await cargarHistorial();
    } catch (e: any) {
        alert("❌ Error al revertir: " + (e.response?.data?.mensaje || e.message));
    }
}

const abrirModalCierre = (orden: ProduccionItem) => {
    ordenParaCerrar.value = orden;
    mostrarModalCierre.value = true;
}

const cerrarModalCierre = () => {
    mostrarModalCierre.value = false;
    ordenParaCerrar.value = null;
}

const onCierreConfirmado = () => {
    cargarHistorial();
}

async function cancelarOrden(item: ProduccionItem) {
    if (!confirm(`⚠️ ¿Cancelar la Orden #${item.id}?`)) return;
    try {
        await api.post(`/Ordenes/cancelar/${item.id}`);
        item.estado = "Cancelada";
        ordenesSeleccionadas.value = ordenesSeleccionadas.value.filter(id => id !== item.id);
    } catch (e: any) {
        alert("❌ " + (e.response?.data?.mensaje || "Error al cancelar"));
    }
}

const solicitarImpresion = (orden: ProduccionItem, tipo: 'orden' | 'carga') => {
    if (tipo === 'orden' && orden.esImpreso) {
        if (!confirm(`La orden #${orden.id} ya fue impresa. ¿Seguro quieres reimprimirla?`)) return;
    }
    emit('imprimir-historial', { orden, tipo });
};

function normalizarNombreFamilia(nombre: any) {
    if (!nombre || typeof nombre !== 'string') return '';
    let n = nombre.toUpperCase().trim();
    const prefijos = ['FAZON -', 'FAZON-', 'FAZON', 'SERVICIO DE FAZON -', 'SERVICIO DE FAZON'];
    for (const pref of prefijos) {
        if (n.startsWith(pref)) {
            n = n.substring(pref.length).trim();
            break;
        }
    }
    n = n.replace(/FAZON/g, '').replace(/SERVICIO/g, '').replace(/LAMINADO/g, '').replace(/-/g, '').trim();
    return n.replace(/\s+/g, ' '); 
}

function toggleSeleccionMultiple(id: number) {
    const index = ordenesSeleccionadas.value.indexOf(id);
    if (index === -1) {
        ordenesSeleccionadas.value.push(id);
    } else {
        ordenesSeleccionadas.value.splice(index, 1);
    }
}

function obtenerNombreClienteReal(orden: any) {
    if (typeof orden.cliente === 'string' && orden.cliente.trim() !== '') return orden.cliente;
    if (orden.clienteNombre && typeof orden.clienteNombre === 'string' && orden.clienteNombre.trim() !== '') return orden.clienteNombre;
    if (orden.cliente && typeof orden.cliente === 'object' && orden.cliente.razonSocial) return orden.cliente.razonSocial;
    return 'Desconocido';
}

function calcularKilosNetos(kilosBrutos: number, desperdicio: number | undefined) {
    if (!kilosBrutos) return 0;
    const porcentaje = desperdicio || 0;
    return Math.round(kilosBrutos / (1 + (porcentaje / 100)));
}

function imprimirLoteOP() {
    if (ordenesSeleccionadas.value.length === 0) return;
    const ordenesAImprimir = producciones.value.filter(p => ordenesSeleccionadas.value.includes(p.id));
    
    const yaImpresas = ordenesAImprimir.filter(o => o.esImpreso).length;
    if (yaImpresas > 0) {
        const msj = yaImpresas === 1 
            ? "Hay 1 orden seleccionada que ya fue impresa. ¿Seguro quieres reimprimirla?" 
            : `Hay ${yaImpresas} órdenes seleccionadas que ya fueron impresas. ¿Seguro quieres reimprimirlas?`;
        if (!confirm(msj)) return;
    }

    emit('imprimir-lote-op', ordenesAImprimir);
    ordenesSeleccionadas.value = [];
}

function imprimirCargaConsolidada() {
    if (ordenesSeleccionadas.value.length < 2) return;

    const ordenesAImprimir = producciones.value.filter(p => ordenesSeleccionadas.value.includes(p.id));
    
    const familiaBase = normalizarNombreFamilia(ordenesAImprimir[0]?.producto || '');
    const sonDiferentes = ordenesAImprimir.some(o => normalizarNombreFamilia(o.producto || '') !== familiaBase);
    
    if (sonDiferentes) {
        alert(`⛔ ERROR DE COMPATIBILIDAD\n\nPara la Hoja de Mezcla Consolidada, TODAS las órdenes deben ser del mismo material base.\nMaterial detectado: ${familiaBase}`);
        return;
    }

    let totalKilosMezcla = 0;
    const recetaConsolidadaMap: Record<string, { id: number, nombre: string, kilos: number }> = {};
    const notasSet = new Set<string>();

    ordenesAImprimir.forEach(orden => {
        const refPedido = orden.notaPedido ? String(orden.notaPedido) : String(orden.id);
        const nombreCliente = obtenerNombreClienteReal(orden);
        
        notasSet.add(refPedido);

        if (orden.consumos && Array.isArray(orden.consumos)) {
            orden.consumos.forEach(consumo => {
                const mpId = consumo.materiaPrimaId;
                const nombreMP = (consumo.nombreMateriaPrima || 'Insumo').toUpperCase();
                const esMaterialSeparado = nombreMP.includes('MOLIDO') || nombreMP.includes('RECUPERADO') || nombreMP.includes('FAZON') || nombreMP.includes('SCRAP');
                const mapKey = esMaterialSeparado ? `${mpId}-${refPedido}` : `${mpId}`;
                
                let nombreAVisualizar = consumo.nombreMateriaPrima || 'Insumo';
                if (esMaterialSeparado) {
                    nombreAVisualizar = `${nombreAVisualizar} (${nombreCliente})`;
                }

                if (!recetaConsolidadaMap[mapKey]) {
                    recetaConsolidadaMap[mapKey] = { id: mpId, nombre: nombreAVisualizar, kilos: 0 };
                }
                
                const kilosYaBrutos = (consumo.cantidadKilos || 0);

                recetaConsolidadaMap[mapKey].kilos += kilosYaBrutos;
                totalKilosMezcla += kilosYaBrutos;
            });
        }
    });

    const consumosConsolidadosArray = Object.values(recetaConsolidadaMap).sort((a, b) => b.kilos - a.kilos);
    const notasUnicas = Array.from(notasSet);
    const nombreProductoBase = normalizarNombreFamilia(ordenesAImprimir[0]?.producto || '');

    const ordenConsolidadaFalsa = {
        id: "MIX",
        notaPedido: notasUnicas.join(' / '),
        numeroPedidoCliente: nombreProductoBase, 
        producto: familiaBase,
        productoId: 0,
        clienteId: 0,
        cantidad: 0,
        largo: 0,
        ancho: 0,
        espesor: 0,
        
        kilos: totalKilosMezcla,
        desperdicio: 0,
        
        observacion: `LOTE CONSOLIDADO: Pedidos #${notasUnicas.join(', #')}`,
        consumos: consumosConsolidadosArray.map(c => ({
            materiaPrimaId: c.id,
            nombreMateriaPrima: c.nombre,
            cantidadKilos: Math.round(c.kilos * 100) / 100 
        }))
    };

    emit('imprimir-carga-consolidada', { orden: ordenConsolidadaFalsa, tipo: 'carga-consolidada' });
    ordenesSeleccionadas.value = [];
}

const abrirModalEdicion = (orden: ProduccionItem) => {
    ordenEditando.value = orden;
    formEdicion.value = {
        largo: orden.largo || 0,
        ancho: orden.ancho || 0,
        espesor: orden.espesor || 0,
        cantidad: orden.cantidad || 0,
        kilosTotales: orden.kilos || 0,
        desperdicio: orden.desperdicio || 8,
        conBrillo: orden.conBrillo || false,
        llevaFilm: orden.llevaFilm || false,
        tipoCorona: orden.tipoCorona || 'Ninguno',
        color: orden.color || '',
        recetaNueva: JSON.parse(JSON.stringify(orden.consumos || []))
    };
    mostrarModalEdicion.value = true;
};

const cerrarModalEdicion = () => {
    mostrarModalEdicion.value = false;
    ordenEditando.value = null;
};

const recalcularModal = () => {
    if (!ordenEditando.value) return;
    
    const o = ordenEditando.value;
    const oldVol = (o.largo || 1) * (o.ancho || 1) * (o.espesor || 1) * (o.cantidad || 1);
    const newVol = (formEdicion.value.largo || 1) * (formEdicion.value.ancho || 1) * (formEdicion.value.espesor || 1) * (formEdicion.value.cantidad || 1);
    
    let ratio = 1;
    if (oldVol > 0) ratio = newVol / oldVol;

    formEdicion.value.kilosTotales = Number((o.kilos * ratio).toFixed(2));

    formEdicion.value.recetaNueva = (o.consumos || []).map((c: any) => ({
        materiaPrimaId: c.materiaPrimaId,
        cantidadKilos: Number((c.cantidadKilos * ratio).toFixed(2))
    }));
};

async function guardarEdicionRapida() {
    if (!ordenEditando.value) return;
    guardandoEdicion.value = true;
    try {
        await api.put(`/Ordenes/modificar/${ordenEditando.value.id}`, {
            largo: formEdicion.value.largo,
            ancho: formEdicion.value.ancho,
            espesor: formEdicion.value.espesor,
            cantidad: formEdicion.value.cantidad,
            kilosTotales: formEdicion.value.kilosTotales,
            desperdicio: formEdicion.value.desperdicio,
            conBrillo: formEdicion.value.conBrillo,
            llevaFilm: formEdicion.value.llevaFilm,
            tipoCorona: formEdicion.value.tipoCorona,
            color: formEdicion.value.color,
            recetaNueva: formEdicion.value.recetaNueva
        });
        cerrarModalEdicion();
        await cargarHistorial();
    } catch (e: any) {
        alert(e.response?.data?.mensaje || e.response?.data || "Error al modificar la orden.");
    } finally {
        guardandoEdicion.value = false;
    }
}

onMounted(() => {
    cargarHistorial();
    cargarMateriasPrimas(); 
})

defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3 class="titulo-bandeja">
            {{ filtroEstado === 'Pendientes' ? '🔥 Bandeja de Producción' : '🗄️ Histórico de Órdenes' }}
        </h3>
        <div class="filtros-container">
            <input 
                type="text" 
                v-model="filtroLibre" 
                class="input-filtro input-buscador" 
                placeholder="🔍 Buscar Cliente, Nota o OC..."
            >
            
            <select v-model="filtroEstado" class="input-filtro">
                <option value="Pendientes">🔥 En Producción</option>
                <option value="Finalizadas">✅ Finalizadas</option>
                <option value="Canceladas">❌ Canceladas</option>
                <option value="Todos">📁 Todas</option>
            </select>
            <input type="date" v-model="filtroFecha" class="input-filtro">
            <button @click="cargarHistorial" class="btn-refresh" :disabled="cargando" title="Actualizar datos">
                {{ cargando ? '⏳' : '🔄' }}
            </button>
        </div>
    </div>

    <div v-if="error" class="error-msg">{{ error }}</div>

    <div class="tabla-scroll">
        <table class="tabla-custom">
            <thead>
                <tr>
                    <th style="width: 40px; text-align: center;">✓</th>
                    <th style="width: 80px;">Fecha</th>
                    <th style="width: 150px;">Cliente</th>
                    <th style="width: 100px;">N° Pedido</th>
                    <th>Producto</th>
                    <th style="width: 70px; text-align: center;">Cant.</th>
                    <th style="width: 90px; text-align: right;">Kilos (Neto)</th>
                    <th style="width: 120px; text-align: center;">Estado</th>
                    <th style="width: 150px; text-align: center;">Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="p in produccionesFiltradas" :key="p.id" :class="{'fila-impresa': p.esImpreso && p.estado !== 'Finalizada' && p.estado !== 'Cancelada', 'fila-no-impresa': !p.esImpreso && p.estado !== 'Finalizada' && p.estado !== 'Cancelada', 'fila-ok': p.estado === 'Finalizada', 'fila-cancel': p.estado === 'Cancelada', 'fila-seleccionada': ordenesSeleccionadas.includes(p.id)}">
                    
                    <td style="text-align: center; vertical-align: middle;">
                        <input 
                            type="checkbox" 
                            :checked="ordenesSeleccionadas.includes(p.id)"
                            @change="toggleSeleccionMultiple(p.id)"
                            v-if="p.estado === 'Pendiente' || p.estado === 'EnProceso'"
                            class="check-orden"
                        >
                    </td>

                    <td class="td-fecha">{{ p.fecha }}</td>
                    
                    <td>
                        <span class="badge-cliente">
                            {{ p.clienteNombre && p.clienteNombre !== 'Desconocido' ? p.clienteNombre : 'Interno / Stock' }}
                        </span>
                    </td>
                    
                    <td>
                        <div class="texto-nota">
                            {{ p.notaPedido || '-' }}
                        </div>
                        <small v-if="p.numeroPedidoCliente" class="texto-oc">
                            OC: {{ p.numeroPedidoCliente }}
                        </small>
                    </td>
                    
                    <td class="td-prod">{{ p.producto }}</td>
                    <td style="text-align: center; font-weight: 500;">{{ p.cantidad }}</td>
                    
                    <td style="text-align: right; font-weight: bold; color: #2c3e50;">
                        {{ Math.round(p.kilos) }}
                    </td>
                    
                    <td style="text-align: center;">
                        <span :class="{
                            'badge-pend': p.estado === 'Pendiente' || p.estado === 'EnProceso',
                            'badge-ok': p.estado === 'Finalizada',
                            'badge-cancel': p.estado === 'Cancelada'
                        }">
                            {{ p.estado === 'Cancelada' ? 'CANCELADA' : (p.estado === 'Finalizada' ? 'FINALIZADA' : 'EN MÁQUINA') }}
                        </span>
                    </td>
                    
                    <td class="td-acciones">
                        
                        <template v-if="p.estado === 'Pendiente' || p.estado === 'EnProceso'">
                            <button @click="abrirModalEdicion(p)" class="btn-action" title="Modificar Orden">✏️</button>
                            <button @click="abrirModalCierre(p)" class="btn-action btn-check" title="Declarar Consumos y Cerrar OP">✅</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Imprimir OP">📄</button>
                            <button @click="solicitarImpresion(p, 'carga')" class="btn-action btn-ciencia" title="Imprimir Hoja de Carga">🧪</button>
                            
                            <button v-if="p.esImpreso" @click="revertirOrden(p)" class="btn-action" style="color: #f39c12; border-color: #f39c12;" title="Deshacer Impresión">↩️</button>
                            
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel" title="Cancelar y Devolver Material">❌</button>
                        </template>
                        
                        <template v-else-if="p.estado === 'Finalizada'">
                             <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Reimprimir Orden">📄</button>
                             
                             <button @click="revertirOrden(p)" class="btn-action" style="color: #e67e22; border-color: #e67e22;" title="Revertir Cierre de Producción">⏪</button>
                        </template>
                    </td>
                </tr>
                <tr v-if="produccionesFiltradas.length === 0 && !cargando">
                    <td colspan="9" class="vacio">No hay órdenes en esta bandeja para los filtros seleccionados.</td>
                </tr>
            </tbody>
        </table>
        <div v-if="cargando" class="loading-overlay">
            <div class="spinner"></div> Cargando datos...
        </div>
    </div>

    <div v-if="ordenesSeleccionadas.length > 0" class="barra-flotante-consolidada">
        <div class="resumen-seleccion">
            <span class="badge-count">{{ ordenesSeleccionadas.length }}</span> órdenes seleccionadas
        </div>
        <div class="botones-flotantes">
            <button class="btn-consolidado btn-op" @click="imprimirLoteOP">
                📄 Imprimir OP x {{ ordenesSeleccionadas.length }}
            </button>
            <button class="btn-consolidado" @click="imprimirCargaConsolidada" v-if="ordenesSeleccionadas.length > 1">
                🧪 Imprimir Hoja de Carga (Mezcla)
            </button>
        </div>
    </div>

    <div v-if="mostrarModalEdicion" class="modal-overlay" @click.self="cerrarModalEdicion">
        <div class="modal-content">
            <div class="modal-header">
                <h3>✏️ Edición Rápida | Orden #{{ ordenEditando?.id }}</h3>
                <button class="btn-close" @click="cerrarModalEdicion">✕</button>
            </div>
            
            <p class="warning-text">⚠️ Al modificar, la orden perderá su estado de "Impresa" y se recalculará el stock.</p>

            <div class="grid-medidas">
                <div class="campo-rapido">
                    <label>Largo (mm)</label>
                    <input type="number" v-model="formEdicion.largo" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Ancho (mm)</label>
                    <input type="number" v-model="formEdicion.ancho" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Espesor</label>
                    <input type="number" v-model="formEdicion.espesor" step="0.01" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Cantidad</label>
                    <input type="number" v-model="formEdicion.cantidad" min="1" @input="recalcularModal">
                </div>
            </div>

            <div class="kilos-recalculados">
                Nuevo Peso Estimado: <strong>{{ formEdicion.kilosTotales }} kg</strong>
            </div>

            <h4 class="titulo-seccion">Aditivos & Opciones</h4>
            <div class="grid-aditivos">
                <label class="switch-label"><input type="checkbox" v-model="formEdicion.conBrillo" @change="recalcularModal"> ✨ Brillo</label>
                <label class="switch-label"><input type="checkbox" v-model="formEdicion.llevaFilm" @change="recalcularModal"> 🛡️ Lleva Film</label>
            </div>

            <div class="grid-medidas" style="margin-top: 15px;">
                <div class="campo-rapido">
                    <label>⚡ Tratamiento Corona:</label>
                    <select v-model="formEdicion.tipoCorona" @change="recalcularModal">
                        <option value="Ninguno">Ninguno</option>
                        <option value="Simple">Simple</option>
                        <option value="Doble">Doble</option>
                    </select>
                </div>
                <div class="campo-rapido">
                    <label>🎨 Color:</label>
                    <input type="text" v-model="formEdicion.color" placeholder="Ej: BLANCO" @input="recalcularModal">
                </div>
            </div>

            <div class="modal-footer">
                <button class="btn-cancelar" @click="cerrarModalEdicion">Cancelar</button>
                <button class="btn-guardar" @click="guardarEdicionRapida" :disabled="guardandoEdicion">
                    {{ guardandoEdicion ? 'Validando Stock...' : '💾 Confirmar Cambios' }}
                </button>
            </div>
        </div>
    </div>

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
/* Tus estilos exactos sin tocar nada */
.historial-wrapper { background: #ffffff; padding: 20px; border-radius: 12px; border: 1px solid #e2e8f0; box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.05); height: 100%; display: flex; flex-direction: column; position: relative; overflow: hidden; }
.header-tabla { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; border-bottom: 2px solid #3498db; padding-bottom: 10px; flex-wrap: wrap; gap: 10px; }
.titulo-bandeja { margin: 0; font-size: 1.2rem; color: #2c3e50; font-weight: 700; }
.filtros-container { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
.input-filtro { padding: 6px 12px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.9rem; background: #f8fafc; cursor: pointer; color: #334155; font-weight: 500; transition: border-color 0.2s; }
.input-filtro:focus { border-color: #3498db; outline: none; }
.input-buscador { width: 220px; cursor: text; } 
.btn-refresh { background: white; border: 1px solid #cbd5e1; border-radius: 6px; cursor: pointer; padding: 6px 12px; font-size: 1rem; transition: all 0.2s; }
.btn-refresh:hover { background: #f1f5f9; border-color: #94a3b8; }
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
.td-prod { font-weight: 700; color: #1e293b; }
.badge-cliente { background-color: #e0f2fe; color: #0369a1; padding: 4px 8px; border-radius: 4px; font-weight: 600; font-size: 0.75rem; display: inline-block; max-width: 140px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.texto-nota { font-weight: 700; color: #334155; font-size: 0.9rem; }
.texto-oc { color: #94a3b8; display: block; font-size: 0.7rem; margin-top: 2px; }
.fila-ok td { background-color: #fbfdfc; color: #94a3b8; }
.fila-ok .td-prod { text-decoration: line-through; color: #94a3b8; } 
.fila-cancel td { background-color: #fffbfa; color: #94a3b8; }
.fila-cancel .td-prod { text-decoration: line-through; color: #ef4444; }
.fila-seleccionada td { background-color: #fffbeb !important; } 
.badge-pend { background: #fff7ed; color: #d97706; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fcd34d; box-shadow: 0 0 5px rgba(217, 119, 6, 0.2); }
.badge-ok { background: #ecfdf5; color: #10b981; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #a7f3d0; }
.badge-cancel { background: #fef2f2; color: #ef4444; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fecaca; }
.td-acciones { display: flex; gap: 6px; justify-content: center; align-items: center; }
.btn-action { border: 1px solid #e2e8f0; background: white; border-radius: 6px; cursor: pointer; padding: 6px; font-size: 1.1rem; display: flex; align-items: center; justify-content: center; transition: all 0.2s cubic-bezier(0.4, 0, 0.2, 1); width: 32px; height: 32px; }
.btn-action:hover { transform: translateY(-2px); box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1); }
.btn-check:hover { background: #f0fdf4; border-color: #6ee7b7; }
.btn-ciencia:hover { background: #eff6ff; border-color: #93c5fd; }
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

.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; backdrop-filter: blur(2px); }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 450px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #f1f5f9; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #1e293b; font-size: 1.2rem; }
.btn-close { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #94a3b8; }
.btn-close:hover { color: #ef4444; }

.warning-text { font-size: 0.85rem; color: #b45309; background: #fef3c7; padding: 10px; border-radius: 6px; border-left: 4px solid #f59e0b; margin-bottom: 20px; }

.grid-medidas { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px; }
.campo-rapido label { display: block; font-size: 0.85rem; font-weight: 600; color: #475569; margin-bottom: 5px; }
.campo-rapido input, .campo-rapido select { width: 100%; padding: 8px 10px; border: 1px solid #cbd5e1; border-radius: 6px; box-sizing: border-box; }
.campo-rapido input:focus, .campo-rapido select:focus { outline: none; border-color: #3b82f6; }

.kilos-recalculados { text-align: center; background: #eff6ff; color: #1d4ed8; padding: 10px; border-radius: 6px; font-size: 1.1rem; margin-bottom: 20px; border: 1px dashed #93c5fd; }

.titulo-seccion { font-size: 0.95rem; color: #334155; border-bottom: 1px solid #e2e8f0; padding-bottom: 5px; margin-bottom: 10px; }
.grid-aditivos { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.switch-label { display: flex; align-items: center; gap: 8px; font-size: 0.9rem; color: #475569; cursor: pointer; }

.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; border-top: 1px solid #f1f5f9; padding-top: 15px; }
.btn-cancelar { background: white; border: 1px solid #cbd5e1; color: #475569; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-cancelar:hover { background: #f8fafc; }
.btn-guardar { background: #3b82f6; color: white; border: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-guardar:hover:not(:disabled) { background: #2563eb; }
.btn-guardar:disabled { background: #94a3b8; cursor: not-allowed; }
</style>