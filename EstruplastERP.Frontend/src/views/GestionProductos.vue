<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

// ... (Resto de tus imports y variables de estado igual que antes) ...
const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL || '/api';
const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]); 
const busqueda = ref('');
const cargando = ref(true);
const error = ref('');
const fileInputFlexxus = ref<HTMLInputElement | null>(null);

// 🆕 ESTADO PARA EL MODAL DE RESUMEN
const mostrarResumen = ref(false);
const resumenData = ref({ creados: 0, actualizados: 0, errores: 0 });

// ... (Funciones de filtrado, getSku, tabs, etc. MANTENER IGUAL) ...
// Pestañas
const tabActual = ref('MP'); 
const subTabCliente = ref('MP_CLI');
const clienteFiltro = ref<number | string>(''); 
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });
const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
const getClienteId = (p: any) => p.clienteId || p.ClienteId || 0;
const esMpCliente = (p: any) => getSku(p).startsWith('MP-CLI');
const esServicioFazon = (p: any) => getSku(p).startsWith('FAZ-');
const esScrap = (p: any) => !!(p.esScrap || p.EsScrap) || getSku(p).startsWith('SCRAP-CLI');
// ... (Computed properties filtrados y counts MANTENER IGUAL) ...
const productosFiltrados = computed(() => {
    let lista = listaProductos.value;
    const tab = tabActual.value;
    if (tab === 'MP') {
        lista = lista.filter(p => (p.esMateriaPrima || p.EsMateriaPrima) && !esMpCliente(p) && !esScrap(p));
    } else if (tab === 'PT') {
        lista = lista.filter(p => (p.esProductoTerminado || p.EsProductoTerminado) && !esServicioFazon(p));
    } else if (tab === 'CLI') {
        if (subTabCliente.value === 'MP_CLI') {
            lista = lista.filter(p => esMpCliente(p) && !esScrap(p));
            if (clienteFiltro.value) lista = lista.filter(p => getClienteId(p) === Number(clienteFiltro.value));
        } else if (subTabCliente.value === 'SCRAP_CLI') {
            lista = lista.filter(p => esScrap(p));
            if (clienteFiltro.value) lista = lista.filter(p => getClienteId(p) === Number(clienteFiltro.value));
        } else {
            lista = lista.filter(p => esServicioFazon(p));
        }
    }
    if (busqueda.value) {
        const texto = busqueda.value.toUpperCase();
        lista = lista.filter(p => getNombre(p).includes(texto) || getSku(p).includes(texto));
    }
    return lista;
});
const countMP = computed(() => listaProductos.value.filter(p => (p.esMateriaPrima || p.EsMateriaPrima) && !esMpCliente(p) && !esScrap(p)).length);
const countPT = computed(() => listaProductos.value.filter(p => (p.esProductoTerminado || p.EsProductoTerminado) && !esServicioFazon(p)).length);
const countCLI = computed(() => listaProductos.value.filter(p => esMpCliente(p) || esServicioFazon(p) || esScrap(p)).length);
const irAEditar = (id: number) => { router.push({ name: 'editar-producto', params: { id } }); };


// --- IMPORTACIÓN FLEXXUS MODIFICADA 🆕 ---
const clickImportarFlexxus = () => {
    fileInputFlexxus.value?.click();
};

const procesarArchivoFlexxus = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (!target.files || target.files.length === 0) return;

    const file = target.files[0];
    
    // Validación más estricta para CSV si quieres
    if (!confirm(`¿Procesar archivo "${file.name}"?`)) {
        target.value = '';
        return;
    }

    try {
        cargando.value = true;
        const formData = new FormData();
        formData.append('archivo', file);

        const response = await axios.post(`${apiUrl}/Flexxus/importar-mp`, formData, {
            headers: { ...getAuthConfig().headers, 'Content-Type': 'multipart/form-data' }
        });

        // 🆕 Guardamos los datos y mostramos el modal
        resumenData.value = {
            creados: response.data.creados,
            actualizados: response.data.actualizados,
            errores: response.data.errores || 0
        };
        mostrarResumen.value = true;

        await recargarDatos(); 

    } catch (e: any) {
        console.error(e);
        const msg = e.response?.data?.message || e.message || 'Error al importar.';
        alert(`❌ Error: ${msg}`);
    } finally {
        cargando.value = false;
        target.value = '';
    }
};

const cerrarModal = () => {
    mostrarResumen.value = false;
};

// ... (RecargarDatos y onMounted MANTENER IGUAL) ...
const recargarDatos = async () => {
    try {
        cargando.value = true;
        const [resProd, resCli] = await Promise.all([
            axios.get(`${apiUrl}/Productos`, getAuthConfig()),
            axios.get(`${apiUrl}/Clientes`, getAuthConfig())
        ]);
        listaProductos.value = resProd.data.sort((a: any, b: any) => getNombre(a).localeCompare(getNombre(b)));
        listaClientes.value = resCli.data;
    } catch (e: any) {
        error.value = "Error al cargar datos.";
        console.error(e);
    } finally {
        cargando.value = false;
    }
};
onMounted(() => { recargarDatos(); });
</script>

<template>
    <div class="contenedor-stock">
        <div class="header-stock">
            <div class="titulos">
                <h2>📦 Gestión de Stock</h2>
            </div>
            
            <div class="acciones-header">
                <button class="btn-flexxus" @click="clickImportarFlexxus" :disabled="cargando">
                    📄 Importar CSV Flexxus
                </button>
                <span class="info-csv">(Formato .csv o .xlsx)</span>

                <input 
                    type="file" 
                    ref="fileInputFlexxus" 
                    style="display: none" 
                    accept=".csv, .xlsx, .xls"
                    @change="procesarArchivoFlexxus"
                />

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

        <div v-if="tabActual === 'CLI'" class="toolbar-clientes">
            <div class="filtro-cliente">
                <label>Filtrar por Cliente:</label>
                <select v-model="clienteFiltro">
                    <option value="">-- Todos los Clientes --</option>
                    <option v-for="c in listaClientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                </select>
            </div>
            <div class="sub-tabs">
                <button :class="{ 'sub-active': subTabCliente === 'MP_CLI' }" @click="subTabCliente = 'MP_CLI'">📥 Fazon</button>
                <button :class="{ 'sub-active': subTabCliente === 'SCRAP_CLI' }" @click="subTabCliente = 'SCRAP_CLI'">♻️ Scrap / Molido</button>
                <button :class="{ 'sub-active': subTabCliente === 'PT_CLI' }" @click="subTabCliente = 'PT_CLI'">📤 Prod. Terminados</button>
            </div>
        </div>

        <div v-if="cargando" class="loading">⏳ Procesando...</div>
        <div v-else-if="error" class="error">{{ error }}</div>

        <div v-else class="tabla-wrapper">
            <table>
                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Descripción</th>
                        <th v-if="tabActual === 'CLI'">Cliente</th>
                        <th>Color</th>
                        <th style="text-align:right">Stock (Kg)</th>
                        <th style="text-align:right">Costo ($)</th>
                        <th style="text-align:center">Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="p in productosFiltrados" :key="p.id" :class="{'bajo-stock': p.stockActual <= p.stockMinimo}">
                        <td class="sku">{{ p.codigoSku }}</td>
                        <td>
                            <div class="nombre-prod">{{ p.nombre }}</div>
                            <small v-if="p.esFazon && tabActual !== 'CLI'" class="tag-fazon">FAZON</small>
                            <small v-if="esScrap(p)" style="color: #d35400; font-weight:bold; margin-left:5px;">(RECUPERADO)</small>
                        </td>
                        <td v-if="tabActual === 'CLI'">{{ listaClientes.find(c => c.id === p.clienteId)?.razonSocial || 'N/A' }}</td>
                        <td>{{ p.color || '-' }}</td>
                        <td style="text-align:right; font-weight: bold;" :class="{'text-danger': p.stockActual <= 0}">{{ p.stockActual }}</td>
                        <td style="text-align:right;">{{ p.precioCosto ? `$${p.precioCosto}` : '-' }}</td>
                        <td style="text-align:center;">
                            <button @click="irAEditar(p.id)" class="btn-editar" title="Editar">✏️</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div v-if="mostrarResumen" class="modal-overlay">
        <div class="modal-content">
            <h3>📊 Resumen de Importación</h3>
            <div class="modal-stats">
                <div class="stat-item creado">
                    <span class="emoji">✨</span>
                    <span class="num">{{ resumenData.creados }}</span>
                    <span class="label">Nuevos Creados</span>
                </div>
                <div class="stat-item actualizado">
                    <span class="emoji">🔄</span>
                    <span class="num">{{ resumenData.actualizados }}</span>
                    <span class="label">Actualizados</span>
                </div>
            </div>
            
            <p v-if="resumenData.errores > 0" class="text-error">
                ⚠️ Se ignoraron {{ resumenData.errores }} filas por errores.
            </p>

            <button class="btn-cerrar" @click="cerrarModal">Aceptar</button>
        </div>
    </div>
</template>

<style scoped>
/* ... MANTENER ESTILOS VIEJOS ... */
.contenedor-stock { max-width: 1200px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; gap: 15px;}
.header-stock h2 { margin: 0; color: #2c3e50; }
.acciones-header { display: flex; gap: 10px; align-items: center; }
.btn-flexxus { background-color: #27ae60; color: white; border: none; padding: 10px 15px; border-radius: 5px; cursor: pointer; font-weight: bold; display: flex; align-items: center; gap: 8px; transition: background 0.3s; }
.btn-flexxus:hover { background-color: #219150; }
.info-csv { font-size: 0.8rem; color: #7f8c8d; font-style: italic; }
.buscador input { padding: 10px; width: 300px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }
.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; }
.tab-btn { background: none; border: none; padding: 10px 20px; font-size: 1rem; color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s; font-weight: 600; display: flex; align-items: center; gap: 8px; }
.tab-btn:hover { background-color: #f7f9fa; color: #34495e; }
.tab-btn.active { background-color: #e3f2fd; color: #1976d2; }
.counter { background: #eee; padding: 2px 6px; border-radius: 10px; font-size: 0.8em; color: #555; }
.tab-btn.active .counter { background: #bbdefb; color: #0d47a1; }
.toolbar-clientes { background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; display: flex; align-items: center; justify-content: space-between; border: 1px solid #e0e0e0; }
.filtro-cliente { display: flex; align-items: center; gap: 10px; font-weight: bold; color: #555; }
.filtro-cliente select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 200px; }
.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; }
.sub-tabs button { border: none; background: transparent; padding: 6px 12px; font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500; }
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; shadow: 0 2px 4px rgba(0,0,0,0.1); }
.tabla-wrapper { overflow-x: auto; min-height: 300px; }
table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
th { background: #f8f9fa; text-align: left; padding: 12px; font-weight: 600; color: #555; border-bottom: 2px solid #eee; }
td { padding: 12px; border-bottom: 1px solid #f1f1f1; vertical-align: middle; color: #333; }
tr:hover { background-color: #f9f9f9; }
.sku { font-family: 'Courier New', monospace; font-weight: bold; color: #666; font-size: 0.9em; }
.nombre-prod { font-weight: 600; color: #2c3e50; }
.tag-fazon { background: purple; color: white; padding: 2px 4px; border-radius: 4px; font-size: 0.7em; margin-left: 5px; }
.bajo-stock td { background-color: #fff5f5; }
.text-danger { color: #e74c3c; }
.btn-editar { border: none; background: #e3f2fd; color: #1976d2; width: 35px; height: 35px; border-radius: 50%; cursor: pointer; transition: background 0.2s; font-size: 1.1rem; }
.btn-editar:hover { background: #bbdefb; }
.loading { text-align: center; padding: 40px; color: #3498db; font-size: 1.2rem; }
.vacio { text-align: center; padding: 30px; color: #999; font-style: italic; }
.error { color: red; text-align: center; }

/* 🆕 ESTILOS DEL MODAL (NUEVO) */
.modal-overlay {
    position: fixed; top: 0; left: 0; width: 100%; height: 100%;
    background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000;
}
.modal-content {
    background: white; padding: 30px; border-radius: 12px;
    box-shadow: 0 10px 25px rgba(0,0,0,0.2); width: 90%; max-width: 400px;
    text-align: center;
}
.modal-content h3 { color: #2c3e50; margin-top: 0; }
.modal-stats { display: flex; justify-content: space-around; margin: 25px 0; }
.stat-item { display: flex; flex-direction: column; align-items: center; }
.stat-item .emoji { font-size: 2rem; margin-bottom: 5px; }
.stat-item .num { font-size: 1.8rem; font-weight: bold; color: #333; }
.stat-item .label { font-size: 0.8rem; text-transform: uppercase; color: #777; font-weight: bold; }
.stat-item.creado .num { color: #27ae60; }
.stat-item.actualizado .num { color: #2980b9; }

.btn-cerrar {
    background: #34495e; color: white; border: none; padding: 10px 25px;
    border-radius: 25px; cursor: pointer; font-size: 1rem; transition: background 0.2s;
}
.btn-cerrar:hover { background: #2c3e50; }
</style>