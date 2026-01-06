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
const subTabCliente = ref('MP_CLI'); // 'MP_CLI' | 'PT_CLI'
const clienteFiltro = ref<number | string>(''); 

const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

// --- 🛡️ FUNCIONES BLINDADAS (Leen Mayúsculas y Minúsculas) ---

const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();

// Obtener ID (Soporta clienteId, ClienteId o ID null)
const getClienteId = (p: any) => p.clienteId || p.ClienteId || 0;

// Detectar Tipo por SKU (Más seguro que los flags booleanos)
const esMpCliente = (p: any) => getSku(p).startsWith('MP-CLI');
const esServicioFazon = (p: any) => getSku(p).startsWith('FAZ-');

// --- FILTRADO PRINCIPAL ---
const productosFiltrados = computed(() => {
    let lista = listaProductos.value;
    const tab = tabActual.value;

    if (tab === 'MP') {
        // Muestra Materias Primas de la Fábrica (Excluye las que empiezan con MP-CLI)
        lista = lista.filter(p => 
            (p.esMateriaPrima || p.EsMateriaPrima) && 
            !esMpCliente(p)
        );
    } 
    else if (tab === 'PT') {
        // Muestra PT Fábrica (Excluye servicios FAZ)
        lista = lista.filter(p => 
            (p.esProductoTerminado || p.EsProductoTerminado) && 
            !esServicioFazon(p)
        );
    } 
    else if (tab === 'CLI') {
        // --- PESTAÑA CLIENTES ---

        // 1. Filtrar por Sub-Tab
        if (subTabCliente.value === 'MP_CLI') {
            // MODO: Materias Primas del Cliente
            // Filtro: Todo lo que tenga SKU "MP-CLI..."
            // Esto ignorará si EsFazon viene true/false/null, confía en el código.
            lista = lista.filter(p => esMpCliente(p));

            // Filtro Dropdown (Solo aplica a MP porque tienen dueño específico)
            if (clienteFiltro.value) {
                const idFiltro = Number(clienteFiltro.value);
                lista = lista.filter(p => getClienteId(p) === idFiltro);
            }

        } else {
            // MODO: Servicios / Prod Terminados
            // Filtro: Todo lo que tenga SKU "FAZ-..."
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
const countMP = computed(() => listaProductos.value.filter(p => (p.esMateriaPrima || p.EsMateriaPrima) && !esMpCliente(p)).length);
const countPT = computed(() => listaProductos.value.filter(p => (p.esProductoTerminado || p.EsProductoTerminado) && !esServicioFazon(p)).length);
const countCLI = computed(() => listaProductos.value.filter(p => esMpCliente(p) || esServicioFazon(p)).length);

const irAEditar = (id: number) => {
    router.push({ name: 'editar-producto', params: { id } });
};

// --- CARGA DE DATOS ---
onMounted(async () => {
    try {
        cargando.value = true;
        const [resProd, resCli] = await Promise.all([
            axios.get(`${apiUrl}/Productos`, getAuthConfig()),
            axios.get(`${apiUrl}/Clientes`, getAuthConfig())
        ]);
        
        listaProductos.value = resProd.data.sort((a: any, b: any) => getNombre(a).localeCompare(getNombre(b)));
        listaClientes.value = resCli.data;

        // DEBUG: Mira la consola para ver qué está llegando
        console.log("Productos Cargados:", listaProductos.value.length);
        console.log("Ejemplo de MP Cliente:", listaProductos.value.find(p => getSku(p).startsWith('MP-CLI')));

    } catch (e: any) {
        error.value = "Error al cargar datos.";
        console.error(e);
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
                    📥 Sus Materias Primas
                </button>
                <button 
                    :class="{ 'sub-active': subTabCliente === 'PT_CLI' }" 
                    @click="subTabCliente = 'PT_CLI'">
                    📤 Sus Productos Terminados
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