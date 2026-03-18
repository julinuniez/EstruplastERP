<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/services/axiosInstance' 

const emit = defineEmits(['imprimir-historial', 'imprimir-carga-consolidada', 'imprimir-lote-op']);

interface ProduccionItem {
    id: number;
    fecha: string;
    producto: string;
    cantidad: number;
    kilos: number;
    desperdicio?: number; // 🚨 Propiedad vital para los cálculos
    estado: string;
    esFinalizada: boolean;
    notaPedido?: string;
    numeroPedidoCliente?: string;
    clienteNombre?: string; 
    cliente?: any;
    largo?: number;
    ancho?: number;
    espesor?: number;
    color?: string;
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

const produccionesFiltradas = computed(() => {
    return producciones.value.filter(item => {
        let pasaEstado = true;
        if (filtroEstado.value === 'EnCola') pasaEstado = item.estado === 'EnCola';
        else if (filtroEstado.value === 'Pendientes') pasaEstado = item.estado === 'Pendiente' || item.estado === 'EnProceso';
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

async function activarOrden(item: ProduccionItem) {
    try {
        await api.post(`/Ordenes/activar/${item.id}`)
        item.estado = "Pendiente"; 
    } catch (e: any) {
        const msj = e.response?.data?.mensaje || e.response?.data || "Error de conexión";
        alert("❌ " + msj); 
    }
}

async function confirmarOrdenRapida(item: ProduccionItem) {
    try {
        await api.post(`/Ordenes/confirmar/${item.id}`)
        item.esFinalizada = true;
        item.estado = "Finalizada"; 
        ordenesSeleccionadas.value = ordenesSeleccionadas.value.filter(id => id !== item.id);
    } catch (e: any) {
        alert("❌ Error: " + (e.response?.data?.mensaje || "Error de conexión"));
    }
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
    emit('imprimir-historial', { orden, tipo });
};

function normalizarNombreFamilia(nombre: string) {
    if (!nombre) return '';
    let n = nombre.toUpperCase().trim();
    const prefijos = ['LAMINADO A FAZON -', 'LAMINADO A FAZON-', 'LAMINADO A FAZON', 'SERVICIO DE FAZON -', 'SERVICIO DE FAZON'];
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

// 🚨 MAGIA 1: Calculamos el NETO para la tabla de visualización (Le sacamos la basura)
function calcularKilosNetos(kilosBrutos: number, desperdicio: number | undefined) {
    if (!kilosBrutos) return 0;
    const porcentaje = desperdicio || 0;
    // Ejemplo: Si son 110kg Brutos con 10% desperdicio -> 110 / 1.10 = 100kg Netos
    return Math.round(kilosBrutos / (1 + (porcentaje / 100)));
}

function imprimirLoteOP() {
    if (ordenesSeleccionadas.value.length === 0) return;
    const ordenesAImprimir = producciones.value.filter(p => ordenesSeleccionadas.value.includes(p.id));
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
                
                // 🚨 CAMBIO VITAL: Ya NO multiplicamos por desperdicio aquí.
                // Como ahora guardamos los consumos BRUTOS en la BD, simplemente los sumamos.
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
        productoId: 0,
        clienteId: 0,
        cantidad: 0,
        largo: 0,
        ancho: 0,
        espesor: 0,
        
        kilos: totalKilosMezcla, // Este ya es el Bruto total
        desperdicio: 0,          // Se envía 0 para que el PDF no vuelva a calcular nada
        
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

onMounted(() => {
    cargarHistorial();
})

defineExpose({ cargarHistorial })
</script>

<template>
  <div class="historial-wrapper">
    <div class="header-tabla">
        <h3 class="titulo-bandeja">
            {{ filtroEstado === 'EnCola' ? '🕒 Bandeja de Espera' : (filtroEstado === 'Pendientes' ? '🔥 Bandeja de Producción' : '🗄️ Histórico de Órdenes') }}
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
                <option value="EnCola">🕒 En Cola (Falta Mat.)</option>
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
                <tr v-for="p in produccionesFiltradas" :key="p.id" :class="{'fila-ok': p.estado === 'Finalizada', 'fila-cancel': p.estado === 'Cancelada', 'fila-seleccionada': ordenesSeleccionadas.includes(p.id)}">
                    
                    <td style="text-align: center; vertical-align: middle;">
                        <input 
                            type="checkbox" 
                            :checked="ordenesSeleccionadas.includes(p.id)"
                            @change="toggleSeleccionMultiple(p.id)"
                            v-if="p.estado === 'Pendientes' || p.estado === 'Pendiente' || p.estado === 'EnProceso' || p.estado === 'EnCola'"
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
                            'badge-cola': p.estado === 'EnCola',
                            'badge-pend': p.estado === 'Pendiente' || p.estado === 'EnProceso',
                            'badge-ok': p.estado === 'Finalizada',
                            'badge-cancel': p.estado === 'Cancelada'
                        }">
                            {{ p.estado === 'EnCola' ? 'EN COLA' : (p.estado === 'Cancelada' ? 'CANCELADA' : (p.estado === 'Finalizada' ? 'FINALIZADA' : 'EN MÁQUINA')) }}
                        </span>
                    </td>
                    
                    <td class="td-acciones">
                        <template v-if="p.estado === 'EnCola'">
                            <button @click="activarOrden(p)" class="btn-action btn-check" title="Enviar a Máquina">🚀</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Imprimir OP">📄</button>
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel" title="Cancelar Orden">❌</button>
                        </template>

                        <template v-else-if="p.estado === 'Pendiente' || p.estado === 'EnProceso'">
                            <button @click="confirmarOrdenRapida(p)" class="btn-action btn-check" title="Confirmar Producción Finalizada">✅</button>
                            <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Imprimir OP">📄</button>
                            <button @click="solicitarImpresion(p, 'carga')" class="btn-action btn-ciencia" title="Imprimir Hoja de Carga">🧪</button>
                            <button @click="cancelarOrden(p)" class="btn-action btn-cancel" title="Cancelar y Devolver Material">❌</button>
                        </template>
                        
                        <template v-else-if="p.estado === 'Finalizada'">
                             <button @click="solicitarImpresion(p, 'orden')" class="btn-action" title="Reimprimir Orden">📄</button>
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
.tabla-scroll { overflow-y: auto; flex: 1; margin-bottom: 55px; border-radius: 6px; border: 1px solid #e2e8f0; }
.tabla-custom { width: 100%; border-collapse: separate; border-spacing: 0; font-size: 0.85rem; }
.tabla-custom th { background: #f8fafc; color: #475569; padding: 12px 10px; text-align: left; position: sticky; top: 0; z-index: 5; font-weight: 700; border-bottom: 2px solid #e2e8f0; text-transform: uppercase; font-size: 0.75rem; letter-spacing: 0.5px; }
.tabla-custom td { padding: 10px; border-bottom: 1px solid #f1f5f9; color: #334155; vertical-align: middle; transition: background-color 0.2s; }
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
.badge-cola { background: #f1f5f9; color: #475569; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #cbd5e1; }
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
</style>