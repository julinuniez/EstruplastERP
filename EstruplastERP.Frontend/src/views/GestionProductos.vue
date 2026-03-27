<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useRouter } from 'vue-router';
import api from '@/services/axiosInstance';
import { useFiltrosInventario, detectarTipo } from '@/composables/useFiltrosInventario';
import { useImportacionInventario } from '@/composables/useImportacionInventario';
import { useModalesInventario } from '@/composables/useModalesInventario';

const router = useRouter();

const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]);
const busqueda = ref('');
const cargando = ref(true);
const error = ref('');

const tabActual = ref('MP');
const subTabMP = ref('VIRGEN'); 
const subTabCliente = ref('MOLIDO_CLI');
const clienteFiltro = ref<number | string>('');
const materialFiltro = ref<string>('');
const importClienteFiltro = ref<number | string>('');

watch(clienteFiltro, () => { materialFiltro.value = ''; });
watch(tabActual, () => { subTabMP.value = 'VIRGEN'; });

const { 
    TIPOS_MATERIALES, clientesFazon, productosFiltrados, 
    countMP, countPT, countCLI, getClienteId, checkEsFazon, 
    checkEsMolido, checkEsScrap 
} = useFiltrosInventario(
    listaProductos, listaClientes, tabActual, subTabMP, 
    subTabCliente, clienteFiltro, materialFiltro, busqueda
);

const { 
    mostrarModalNuevaMP, nuevaMP, guardandoMP, 
    mostrarModalReservas, productoSeleccionado, ordenesReserva, cargandoReservas,
    verDetalleReserva, guardarNuevaMateriaPrima
} = useModalesInventario(cargarDatos);

const {
    fileInput, importando, clickImportar, subirArchivoFlexxus
} = useImportacionInventario(tabActual, clienteFiltro, importClienteFiltro, cargarDatos);

const irAEditar = (id: number) => {
    router.push({ name: 'editar-producto', params: { id } });
};

async function cargarDatos() {
    try {
        cargando.value = true;
        const [resProd, resCli] = await Promise.all([
            api.get('/Productos'),
            api.get('/Clientes')
        ]);
        
        const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
        
        if (Array.isArray(resProd.data)) {
            const productosActivos = resProd.data.filter(p => p.activo !== false && p.estado !== 0);
            listaProductos.value = resProd.data.sort((a: any, b: any) => getNombre(a).localeCompare(getNombre(b)));
        } else {
            listaProductos.value = [];
        }
        
        if (Array.isArray(resCli.data)) {
            listaClientes.value = resCli.data;
        } else {
            listaClientes.value = [];
        }
    } catch (e: any) {
        console.error("Error cargando datos:", e);
        error.value = "Error conectando al servidor. Revisa la consola.";
        if (e.response && e.response.status === 401) {
            error.value = "Sesión caducada.";
        }
    } finally {
        cargando.value = false;
    }
}

onMounted(() => {
    cargarDatos();
});
</script>

<template>
    <div class="contenedor-stock">
        <div class="header-stock">
            <div style="display: flex; flex-direction: column;">
                <h2>📦 Gestión de Stock</h2>
                <small style="color: #7f8c8d;">Administración de inventario</small>
            </div>
            
            <div class="acciones-header">
                <button class="btn-nueva-mp" @click="mostrarModalNuevaMP = true">
                    ➕ Crear Insumo
                </button>

                <input type="file" ref="fileInput" class="hidden-input" accept=".csv, .xlsx" @change="subirArchivoFlexxus" />

                <div class="import-group">
                    <select v-model="importClienteFiltro" class="select-import" :disabled="importando || cargando">
                        <option value="">🏢 Importación Completa (Todos los clientes)</option>
                        <option v-for="c in listaClientes" :key="c.id" :value="c.id">
                            📄 {{ c.razonSocial }}
                        </option>
                    </select>
                    <button class="btn-importar" @click="clickImportar" :disabled="importando || cargando">
                        <span v-if="importando">⏳ Procesando...</span>
                        <span v-else>📥 Importar</span>
                    </button>
                </div>

                <div class="buscador">
                    <input type="text" v-model="busqueda" placeholder="🔍 Buscar SKU o Nombre...">
                </div>
            </div>
        </div>

        <div class="tabs-container">
            <button class="tab-btn" :class="{ active: tabActual === 'MP' }" @click="tabActual = 'MP'">
                🧪 Materias Primas <span class="counter">{{ countMP }}</span>
            </button>
            <button class="tab-btn" :class="{ active: tabActual === 'PT' }" @click="tabActual = 'PT'">
                🏭 Productos Terminados <span class="counter">{{ countPT }}</span>
            </button>
            <button class="tab-btn" :class="{ active: tabActual === 'CLI' }" @click="tabActual = 'CLI'">
                🤝 Clientes (Fazon) <span class="counter">{{ countCLI }}</span>
            </button>
        </div>

        <div v-if="tabActual === 'MP'" class="toolbar-clientes" style="margin-top: -10px;">
            <div class="sub-tabs">
                <button :class="{ 'sub-active': subTabMP === 'VIRGEN' }" @click="subTabMP = 'VIRGEN'">🧪 Material Virgen</button>
                <button :class="{ 'sub-active': subTabMP === 'ADITIVOS' }" @click="subTabMP = 'ADITIVOS'">⚙️ Aditivos</button>
                <button :class="{ 'sub-active': subTabMP === 'MASTERBATCH' }" @click="subTabMP = 'MASTERBATCH'">🎨 Masterbatch / Colores</button>
            </div>
        </div>

        <div v-if="tabActual === 'CLI'" class="toolbar-clientes">
            <div class="fila-filtros">
                <div class="filtro-item">
                    <label>🏢 Cliente:</label>
                    <select v-model="clienteFiltro">
                        <option value="">-- Seleccionar Cliente --</option>
                        <option v-for="c in clientesFazon" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                    </select>
                </div>

                <div class="filtro-item" v-if="clienteFiltro">
                    <label>🧱 Material:</label>
                    <select v-model="materialFiltro">
                        <option value="">-- Todos los Materiales --</option>
                        <option v-for="mat in TIPOS_MATERIALES" :key="mat" :value="mat">{{ mat }}</option>
                    </select>
                </div>
            </div>

            <div class="sub-tabs" v-if="clienteFiltro">
                <button :class="{ 'sub-active': subTabCliente === 'MOLIDO_CLI' }" @click="subTabCliente = 'MOLIDO_CLI'">♻️ Molido / Recuperado</button>
                <button :class="{ 'sub-active': subTabCliente === 'PT_CLI' }" @click="subTabCliente = 'PT_CLI'">📤 Prod. Terminados</button>
                <button :class="{ 'sub-active': subTabCliente === 'MP_CLI' }" @click="subTabCliente = 'MP_CLI'">📥 MP Virgen / Otros</button>
            </div>
        </div>

        <div v-if="cargando" class="loading">⏳ Cargando inventario...</div>
        <div v-else-if="error" class="error">{{ error }}</div>

        <div v-else class="tabla-wrapper">
            <table>
                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Descripción</th>
                        <th v-if="tabActual === 'PT'">Dueño</th>
                        <th v-if="tabActual === 'CLI'">Material</th> 
                        <th style="text-align:right; width: 90px;" title="Stock Real en Galpón">Físico (Kg)</th>
                        <th style="text-align:center; width: 90px;" title="Retenido en Órdenes de Producción">Reservado</th>
                        <th style="text-align:right; width: 90px;" title="Stock Libre para usar">Disponible</th>
                        <th style="text-align:center">Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="p in productosFiltrados" :key="p.id" 
                        :class="{'bajo-stock': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= p.stockMinimo}">
                        
                        <td class="sku">{{ p.codigoSku }}</td>
                        <td>
                            <div class="nombre-prod">{{ p.nombre }}</div>
                            <small v-if="checkEsFazon(p)" class="tag-fazon">FAZON</small>
                            <small v-if="checkEsMolido(p)" style="color: #27ae60; font-weight:bold; margin-left:5px;">(MOLIDO)</small>
                            <small v-else-if="checkEsScrap(p)" style="color: #d35400; font-weight:bold; margin-left:5px;">(SCRAP)</small>
                        </td>
                        
                        <td v-if="tabActual === 'PT'">
                            <span v-if="getClienteId(p) > 0" class="badge-cliente">
                                {{ listaClientes.find(c => c.id === getClienteId(p))?.razonSocial || 'Cliente #' + getClienteId(p) }}
                            </span>
                            <span v-else class="badge-propio">Propio</span>
                        </td>

                        <td v-if="tabActual === 'CLI'">
                            <span class="badge-material" v-if="detectingTipo(p) !== 'OTROS'" :class="detectingTipo(p).toLowerCase().replace(' ', '-')">
                                {{ detectingTipo(p) }}
                            </span>
                        </td>

                        <td style="text-align:right; font-weight: 500;">
                            {{ p.stockFisico ?? p.stockActual ?? 0 }}
                        </td>

                        <td style="text-align:center;">
                            <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-buchon" title="Ver dónde está reservado">
                                🔒 {{ p.stockReservado }}
                            </button>
                            <span v-else style="color: #bdc3c7;">-</span>
                        </td>

                        <td style="text-align:right; font-weight: bold; font-size: 1.05rem;" 
                            :class="{'text-danger': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= 0}">
                            {{ p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) }}
                        </td>

                        <td style="text-align:center;">
                            <button @click="irAEditar(p.id)" class="btn-editar" title="Editar">✏️</button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div v-if="productosFiltrados.length === 0" class="vacio">
                <div v-if="tabActual === 'CLI' && !clienteFiltro" class="mensaje-guia">
                    <h3>👈 Selecciona un Cliente</h3>
                    <p>Elige un cliente para ver su stock de Molido, MP o Productos.</p>
                </div>
                <div v-else>
                    No hay productos que coincidan con los filtros.
                </div>
            </div>
        </div>

        <div v-if="mostrarModalReservas" class="modal-overlay" @click.self="mostrarModalReservas = false">
            <div class="modal-content" style="width: 450px;">
                <h3>🔒 Detalle de Reserva</h3>
                <p style="color: #34495e; font-weight: bold; margin-top: -5px; margin-bottom: 20px;">
                    {{ productoSeleccionado?.nombre }}
                </p>

                <div v-if="cargandoReservas" style="text-align: center; padding: 20px;">
                    ⏳ Buscando órdenes de producción...
                </div>
                
                <div v-else class="caja-pedidos-reservados">
                    <table class="tabla-reservas">
                        <thead>
                            <tr>
                                <th>N° Pedido (Flexxus)</th> 
                                <th>Cliente / Destino</th>
                                <th style="text-align: right;">Retenido</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="res in ordenesReserva" :key="res.id">
                                <td style="font-weight: bold; color: #2980b9;">{{ res.notaPedido }}</td>
                                <td>{{ res.cliente }}</td>
                                <td style="text-align: right; font-weight: bold; color: #e67e22;">{{ res.cantidad }} kg</td>
                            </tr>
                            <tr v-if="ordenesReserva.length === 0">
                                <td colspan="3" style="text-align: center; color: #7f8c8d; padding: 15px;">
                                    No se encontraron órdenes activas.
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                <div style="text-align: right; margin-top: 20px;">
                    <button class="btn-cancelar" @click="mostrarModalReservas = false">Cerrar</button>
                </div>
            </div>
        </div>

        <div v-if="mostrarModalNuevaMP" class="modal-overlay">
            <div class="modal-content">
                <h3>📦 Alta Manual de Insumo</h3>
                <p style="font-size: 0.8rem; color: #666; margin-bottom: 15px;">
                    Usá esto solo si el material no bajó de Flexxus o necesitás crearlo urgente para una Orden de Producción.
                </p>

                <label style="display:block; margin-bottom:5px; font-weight:bold; font-size: 0.9rem;">Nombre del Material:</label>
                <input type="text" v-model="nuevaMP.nombre" placeholder="Ej: PEAD VIRGEN DOW..." class="input-modal">

                <label style="display:block; margin-bottom:5px; font-weight:bold; font-size: 0.9rem;">Código SKU (Inventalo si no lo sabés):</label>
                <input type="text" v-model="nuevaMP.codigoSku" placeholder="Ej: MP-PEAD-001" class="input-modal">

                <div class="modal-acciones">
                    <button class="btn-cancelar" @click="mostrarModalNuevaMP = false">Cancelar</button>
                    <button class="btn-guardar-mp" @click="guardarNuevaMateriaPrima" :disabled="guardandoMP">
                        {{ guardandoMP ? 'Guardando...' : '💾 Guardar Insumo' }}
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.contenedor-stock { max-width: 1200px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }

.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; gap: 15px; }
.header-stock h2 { margin: 0; color: #2c3e50; }

.acciones-header { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
.hidden-input { display: none; } 

.import-group { display: flex; align-items: center; gap: 8px; background: #f8f9fa; padding: 5px 10px; border-radius: 6px; border: 1px solid #e0e0e0; }
.select-import { padding: 8px; border: 1px solid #ccc; border-radius: 4px; font-weight: 500; min-width: 200px; }

.btn-importar { background-color: #27ae60; color: white; border: none; padding: 10px 15px; border-radius: 6px; font-weight: bold; cursor: pointer; transition: background 0.2s; display: flex; align-items: center; gap: 5px; }
.btn-importar:hover:not(:disabled) { background-color: #2ecc71; }
.btn-importar:disabled { background-color: #95a5a6; cursor: not-allowed; }

.btn-nueva-mp { background: #3498db; color: white; border: none; padding: 10px 15px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; display: flex; align-items: center; gap: 5px; }
.btn-nueva-mp:hover { background: #2980b9; }

.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; }
.modal-content { background: white; padding: 25px; border-radius: 8px; width: 380px; box-shadow: 0 4px 15px rgba(0,0,0,0.2); }
.modal-content h3 { margin-top: 0; color: #2c3e50; border-bottom: 2px solid #eee; padding-bottom: 10px; }

.input-modal { width: 100%; padding: 10px; margin-bottom: 15px; border: 1px solid #ccc; border-radius: 4px; font-weight: bold; text-transform: uppercase; box-sizing: border-box; }

.modal-acciones { display: flex; justify-content: flex-end; gap: 10px; margin-top: 10px; }
.btn-cancelar { background: #e74c3c; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-guardar-mp { background: #27ae60; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-guardar-mp:disabled { background: #95a5a6; cursor: not-allowed; }

.buscador input { padding: 10px; width: 250px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }

.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; overflow-x: auto; }
.tab-btn { background: none; border: none; padding: 10px 20px; font-size: 1rem; color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s; font-weight: 600; display: flex; align-items: center; gap: 8px; white-space: nowrap; }
.tab-btn:hover { background-color: #f7f9fa; color: #34495e; }
.tab-btn.active { background-color: #e3f2fd; color: #1976d2; }
.counter { background: #eee; padding: 2px 6px; border-radius: 10px; font-size: 0.8em; color: #555; }
.tab-btn.active .counter { background: #bbdefb; color: #0d47a1; }

.toolbar-clientes { background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #e0e0e0; display: flex; flex-direction: column; gap: 15px; }
.fila-filtros { display: flex; gap: 20px; flex-wrap: wrap; align-items: flex-end; }
.filtro-item { display: flex; flex-direction: column; gap: 5px; }
.filtro-item label { font-weight: bold; color: #555; font-size: 0.9rem; }
.filtro-item select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 220px; font-weight: 500; }

.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; width: fit-content; }
.sub-tabs button { border: none; background: transparent; padding: 6px 12px; font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500; transition: all 0.2s; }
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }

.tabla-wrapper { overflow-x: auto; min-height: 300px; }
table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
th { background: #f8f9fa; text-align: left; padding: 12px; font-weight: 600; color: #555; border-bottom: 2px solid #eee; }
td { padding: 12px; border-bottom: 1px solid #f1f1f1; vertical-align: middle; color: #333; transition: background-color 0.2s; }
tr:hover { background-color: #f9f9f9; }

.sku { font-family: 'Courier New', monospace; font-weight: bold; color: #666; font-size: 0.9em; }
.nombre-prod { font-weight: 600; color: #2c3e50; }
.tag-fazon { background: purple; color: white; padding: 2px 4px; border-radius: 4px; font-size: 0.7em; margin-left: 5px; }

.badge-cliente { background: #8e44ad; color: white; padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; }
.badge-propio { background: #2980b9; color: white; padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; opacity: 0.8; }

.badge-material { background: #95a5a6; color: white; padding: 3px 10px; border-radius: 12px; font-size: 0.75rem; font-weight: bold; letter-spacing: 0.5px; }
.badge-material.pai { background: #e67e22; }
.badge-material.pead { background: #27ae60; }
.badge-material.pp { background: #8e44ad; }
.badge-material.bio { background: #2ecc71; }
.badge-material.abs { background: #c0392b; }
.badge-material.resistente-freon { background: #2980b9; }
.badge-material.polietileno { background: #16a085; }

tr.bajo-stock td { background-color: #fdeaea !important; }
tr.bajo-stock:hover td { background-color: #fadada !important; }
tr.bajo-stock .sku { color: #c0392b; }
tr.bajo-stock .nombre-prod { color: #c0392b; font-weight: 700; }
.text-danger { color: #e74c3c; }

.btn-buchon { background: #fff3e0; border: 1px solid #ffb74d; color: #e67e22; padding: 4px 10px; border-radius: 15px; font-size: 0.85rem; font-weight: bold; cursor: pointer; transition: all 0.2s; }
.btn-buchon:hover { background: #ffe0b2; transform: scale(1.05); }

/* ESTILOS TABLA MODAL RESERVAS CON SCROLL */
.caja-pedidos-reservados {
    max-height: 60vh;
    overflow-y: auto;
    padding-right: 10px;
}

.caja-pedidos-reservados::-webkit-scrollbar {
    width: 6px;
}
.caja-pedidos-reservados::-webkit-scrollbar-track {
    background: #f1f5f9; 
    border-radius: 4px;
}
.caja-pedidos-reservados::-webkit-scrollbar-thumb {
    background: #cbd5e1; 
    border-radius: 4px;
}
.caja-pedidos-reservados::-webkit-scrollbar-thumb:hover {
    background: #94a3b8; 
}

.tabla-reservas { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla-reservas th { background: #ecf0f1; padding: 8px; font-size: 0.85rem; }
.tabla-reservas td { padding: 10px 8px; font-size: 0.9rem; border-bottom: 1px solid #ecf0f1; }

.btn-editar { border: none; background: #e3f2fd; color: #1976d2; width: 35px; height: 35px; border-radius: 50%; cursor: pointer; transition: background 0.2s; font-size: 1.1rem; }
.btn-editar:hover { background: #bbdefb; }

.loading { text-align: center; padding: 40px; color: #3498db; font-size: 1.2rem; }
.vacio { text-align: center; padding: 30px; color: #999; font-style: italic; }
.error { color: red; text-align: center; }

.mensaje-guia { background: #e8f6f3; padding: 20px; border-radius: 8px; border: 2px dashed #1abc9c; color: #16a085; text-align: center; }
.mensaje-guia h3 { margin: 0 0 10px 0; }

@media (max-width: 768px) {
    .header-stock, .fila-filtros { flex-direction: column; align-items: flex-start; }
    .acciones-header, .filtro-item select, .import-group, .btn-importar, .btn-nueva-mp { width: 100%; justify-content: center; }
    .select-import { width: 100%; }
}
</style>