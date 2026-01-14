<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';

const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

// --- ESTADO ---
const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]); 
const busqueda = ref('');
const cargando = ref(true);
const error = ref('');

// Pestañas
const tabActual = ref('MP'); // 'MP' | 'PT' | 'CLI'
const subTabCliente = ref('MP_CLI'); // 'MP_CLI' | 'SCRAP_CLI' | 'PT_CLI'
const clienteFiltro = ref<number | string>(''); 

const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

// --- 🛡️ FUNCIONES BLINDADAS (Leen Mayúsculas y Minúsculas) ---

const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();

// Obtener ID (Soporta clienteId, ClienteId o ID null)
const getClienteId = (p: any) => p.clienteId || p.ClienteId || 0;

// --- DETECTORES DE TIPO (Basados en SKU para máxima seguridad) ---
const esMpCliente = (p: any) => getSku(p).startsWith('MP-CLI');
const esServicioFazon = (p: any) => getSku(p).startsWith('FAZ-');
// 🔥 NUEVO: Detector de Scrap (Por Flag o por SKU)
const esScrap = (p: any) => !!(p.esScrap || p.EsScrap) || getSku(p).startsWith('SCRAP-CLI');

// --- FILTRADO PRINCIPAL ---
const productosFiltrados = computed(() => {
    let lista = listaProductos.value;
    const tab = tabActual.value;

    // Helper interno para leer propiedad sin importar mayúsculas/minúsculas
    const checkGenerico = (p: any) => !!(p.esGenerico || p.EsGenerico);
    
    // Helper para leer propiedad EsScrap (blindado)
    const checkEsScrap = (p: any) => !!(p.esScrap || p.EsScrap) || getSku(p).startsWith('SCRAP');

    if (tab === 'MP') {
        // Pestaña: MATERIAS PRIMAS (Vírgenes de Fábrica)
        lista = lista.filter(p => 
            (p.esMateriaPrima || p.EsMateriaPrima) && 
            !esMpCliente(p) &&     // No mostrar MP de Clientes
            !checkEsScrap(p) &&    // ⛔ NO MOSTRAR SCRAP AQUÍ (Ni propio ni ajeno)
            
            // Filtros Lógicos (Ocultar bases de sistema)
            !checkGenerico(p) && 
            !getNombre(p).includes('GENERICO') &&
            !getNombre(p).includes('BASE') && 
            p.id !== 90 
        );
    } 
    else if (tab === 'PT') {
        // Pestaña: PRODUCTOS TERMINADOS (Fábrica)
        // Aquí mostramos tus productos de venta (aunque sean genéricos)
        lista = lista.filter(p => 
            (p.esProductoTerminado || p.EsProductoTerminado) && 
            !esServicioFazon(p) // Excluir servicios de inyección puro
        );
    } 
    else if (tab === 'CLI') {
        // --- PESTAÑA CLIENTES ---

        if (subTabCliente.value === 'MP_CLI') {
            // MODO: Materias Primas Vírgenes del Cliente
            lista = lista.filter(p => esMpCliente(p) && !checkEsScrap(p));

            if (clienteFiltro.value) {
                const idFiltro = Number(clienteFiltro.value);
                lista = lista.filter(p => getClienteId(p) === idFiltro);
            }
        } 
        else if (subTabCliente.value === 'SCRAP_CLI') {
            // MODO: Scrap / Molido / Recuperado
            // Aquí SI mostramos todo lo que sea Scrap
            lista = lista.filter(p => checkEsScrap(p));

            if (clienteFiltro.value) {
                const idFiltro = Number(clienteFiltro.value);
                lista = lista.filter(p => getClienteId(p) === idFiltro);
            }
        } 
        else {
            // MODO: Servicios Fazon
            lista = lista.filter(p => esServicioFazon(p));
        }
    }

    // --- BUSCADOR GLOBAL ---
    if (busqueda.value) {
        const texto = busqueda.value.toUpperCase();
        lista = lista.filter(p => 
            getNombre(p).includes(texto) || 
            getSku(p).includes(texto)
        );
    }

    return lista;
});

// --- CONTEOS ---
const countMP = computed(() => listaProductos.value.filter(p => (p.esMateriaPrima || p.EsMateriaPrima) && !esMpCliente(p) && !esScrap(p)).length);
const countPT = computed(() => listaProductos.value.filter(p => (p.esProductoTerminado || p.EsProductoTerminado) && !esServicioFazon(p)).length);
// El contador de clientes suma todo: MP + SCRAP + SERVICIOS
const countCLI = computed(() => listaProductos.value.filter(p => esMpCliente(p) || esServicioFazon(p) || esScrap(p)).length);

const irAEditar = (id: number) => {
    router.push({ name: 'editar-producto', params: { id } });
};

// --- CARGA DE DATOS BLINDADA ---
onMounted(async () => {
    try {
        cargando.value = true;
        
        // Hacemos las peticiones
        const [resProd, resCli] = await Promise.all([
            axios.get(`${apiUrl}/Productos`, getAuthConfig()),
            axios.get(`${apiUrl}/Clientes`, getAuthConfig())
        ]);

        // 🛡️ VALIDACIÓN DE SEGURIDAD PARA PRODUCTOS
        if (Array.isArray(resProd.data)) {
            // Si es un array real, ordenamos y asignamos
            listaProductos.value = resProd.data.sort((a: any, b: any) => 
                getNombre(a).localeCompare(getNombre(b))
            );
        } else {
            // Si NO es un array (es HTML de error), avisamos y evitamos el crash
            console.error("⚠️ ALERTA API PRODUCTOS: Se esperaba una lista [] pero llegó:", resProd.data);
            listaProductos.value = []; // Dejamos la lista vacía para que no explote la tabla
        }

        // 🛡️ VALIDACIÓN DE SEGURIDAD PARA CLIENTES
        if (Array.isArray(resCli.data)) {
            listaClientes.value = resCli.data;
        } else {
            console.error("⚠️ ALERTA API CLIENTES: Se esperaba una lista [] pero llegó:", resCli.data);
            listaClientes.value = [];
        }

    } catch (e: any) {
        error.value = "Error de conexión con el servidor.";
        console.error("❌ Error Crítico:", e);
        
        // Si el error es 401 (No autorizado), mandamos al login
        if (e.response && e.response.status === 401) {
            alert("Tu sesión ha expirado. Por favor ingresa nuevamente.");
            // router.push('/login'); // Descomenta si tienes ruta de login
        }
    } finally {
        cargando.value = false;
    }
});
</script>

<template>
    <div class="contenedor-stock">
        <div class="header-stock">
            <h2>📦 Gestión de Stock</h2>
            <div class="buscador">
                <input type="text" v-model="busqueda" placeholder="🔍 Buscar SKU o Nombre...">
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
                <button 
                    :class="{ 'sub-active': subTabCliente === 'MP_CLI' }" 
                    @click="subTabCliente = 'MP_CLI'">
                    📥 Fazon
                </button>
                
                <button 
                    :class="{ 'sub-active': subTabCliente === 'SCRAP_CLI' }" 
                    @click="subTabCliente = 'SCRAP_CLI'">
                    ♻️ Scrap / Molido
                </button>

                <button 
                    :class="{ 'sub-active': subTabCliente === 'PT_CLI' }" 
                    @click="subTabCliente = 'PT_CLI'">
                    📤 Prod. Terminados
                </button>
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
                        
                        <th v-if="tabActual === 'CLI'">Cliente</th>
                        
                        <th>Color / Variedad</th>
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

                        <td v-if="tabActual === 'CLI'">
                            {{ listaClientes.find(c => c.id === p.clienteId)?.razonSocial || 'N/A' }}
                        </td>

                        <td>{{ p.color || '-' }}</td>
                        
                        <td style="text-align:right; font-weight: bold;" :class="{'text-danger': p.stockActual <= 0}">
                            {{ p.stockActual }}
                        </td>
                        
                        <td style="text-align:right;">
                            {{ p.precioCosto ? `$${p.precioCosto}` : '-' }}
                        </td>

                        <td style="text-align:center;">
                            <button @click="irAEditar(p.id)" class="btn-editar" title="Editar">✏️</button>
                        </td>
                    </tr>
                </tbody>
            </table>
            
            <div v-if="productosFiltrados.length === 0" class="vacio">
                No hay productos que coincidan con los filtros.
            </div>
        </div>
    </div>
</template>

<style scoped>
.contenedor-stock { max-width: 1200px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }

.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.header-stock h2 { margin: 0; color: #2c3e50; }
.buscador input { padding: 10px; width: 300px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }

/* PESTAÑAS PRINCIPALES */
.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; }
.tab-btn {
    background: none; border: none; padding: 10px 20px; font-size: 1rem;
    color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s;
    font-weight: 600; display: flex; align-items: center; gap: 8px;
}
.tab-btn:hover { background-color: #f7f9fa; color: #34495e; }
.tab-btn.active { background-color: #e3f2fd; color: #1976d2; }
.counter { background: #eee; padding: 2px 6px; border-radius: 10px; font-size: 0.8em; color: #555; }
.tab-btn.active .counter { background: #bbdefb; color: #0d47a1; }

/* BARRA DE CLIENTES */
.toolbar-clientes {
    background-color: #f8f9fa;
    padding: 15px;
    border-radius: 8px;
    margin-bottom: 20px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    border: 1px solid #e0e0e0;
}
.filtro-cliente { display: flex; align-items: center; gap: 10px; font-weight: bold; color: #555; }
.filtro-cliente select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 200px; }

.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; }
.sub-tabs button {
    border: none; background: transparent; padding: 6px 12px;
    font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500;
}
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; shadow: 0 2px 4px rgba(0,0,0,0.1); }

/* TABLA */
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

@media (max-width: 768px) {
    .header-stock, .toolbar-clientes { flex-direction: column; gap: 10px; align-items: flex-start; }
    .buscador input, .filtro-cliente select { width: 100%; }
    .tabs-container { overflow-x: auto; padding-bottom: 5px; }
    .tab-btn { white-space: nowrap; }
}
</style>