<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRouter } from 'vue-router';
// 👇 IMPORTANTE: Importamos tu instancia configurada
import api from '@/services/axiosInstance'; 
// Si tu archivo se llama axios.js y está en src, usa: import api from '@/axios';

const router = useRouter();

// 🗑️ BORRAMOS ESTO: const apiUrl = ... (Ya no lo necesitamos)

const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]); 
const busqueda = ref('');
const cargando = ref(true);
const importando = ref(false);
const error = ref('');
const fileInput = ref<HTMLInputElement | null>(null);

const tabActual = ref('MP'); 
const subTabCliente = ref('MP_CLI'); 
const clienteFiltro = ref<number | string>(''); 
const materialFiltro = ref<string>(''); 

const TIPOS_MATERIALES = [
    'PAI', 'PEAD', 'PP', 'BIO', 'ABS', 'RESISTENTE FREON', 'POLIETILENO'
];

// 🗑️ BORRAMOS ESTO: const getAuthConfig = ... (El interceptor ya lo hace)

const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
const getClienteId = (p: any) => p.clienteId || p.ClienteId || 0;

const checkEsPT = (p: any) => !!(p.esProductoTerminado || p.EsProductoTerminado);
const checkEsMP = (p: any) => !!(p.esMateriaPrima || p.EsMateriaPrima);
const checkEsFazon = (p: any) => !!(p.esFazon || p.EsFazon);
const checkEsScrap = (p: any) => !!(p.esScrap || p.EsScrap) || getSku(p).startsWith('SCRAP');
const checkGenerico = (p: any) => !!(p.esGenerico || p.EsGenerico);

const detectarTipo = (p: any) => {
    if (p.tipoMaterial) {
        const t = p.tipoMaterial.toUpperCase().trim();
        if (TIPOS_MATERIALES.includes(t)) return t;
    }
    const n = getNombre(p);
    if (n.includes('FREON') || n.includes('RESISTENTE')) return 'RESISTENTE FREON';
    if (n.includes('BIO') || n.includes('DEGRADABLE')) return 'BIO';
    if (n.includes('ABS')) return 'ABS';
    if (n.includes('PEAD') || n.includes('ALTA') || n.includes('HDPE')) return 'PEAD';
    if (n.includes('PP') || n.includes('POLIPROPILENO')) return 'PP';
    if (n.includes('POLIETILENO') || n.includes('PEBD') || n.includes('BAJA') || n.includes('LDPE')) return 'POLIETILENO';
    if (n.includes('PAI') || n.includes('TUTI') || n.includes('IMPACTO') || n.includes('A.I.')) return 'PAI';
    return 'OTROS';
};

watch(clienteFiltro, () => { materialFiltro.value = ''; });

const esMpCliente = (p: any) => getSku(p).startsWith('MP-CLI') || (checkEsMP(p) && getClienteId(p) > 0);

const clientesFazon = computed(() => {
    return listaClientes.value.filter(c => c.esFazon === true);
});

const productosFiltrados = computed(() => {
    let lista = listaProductos.value;
    const tab = tabActual.value;

    if (tab === 'MP') {
        lista = lista.filter(p => 
            checkEsMP(p) && 
            !esMpCliente(p) && !checkEsScrap(p) && !checkGenerico(p) && 
            !getNombre(p).includes('GENERICO') && !getNombre(p).includes('BASE') && p.id !== 90 
        );
    } 
    else if (tab === 'PT') {
        lista = lista.filter(p => checkEsPT(p));
    } 
    else if (tab === 'CLI') {
        if (!clienteFiltro.value) return [];

        const idFiltro = Number(clienteFiltro.value);
        lista = lista.filter(p => getClienteId(p) === idFiltro);

        if (subTabCliente.value === 'MP_CLI') {
            lista = lista.filter(p => checkEsMP(p) && !checkEsScrap(p));
        } else if (subTabCliente.value === 'SCRAP_CLI') {
            lista = lista.filter(p => checkEsScrap(p));
        } else {
            lista = lista.filter(p => checkEsFazon(p) || checkEsPT(p));
        }

        if (materialFiltro.value) {
            lista = lista.filter(p => detectingTipo(p) === materialFiltro.value);
        }
    }

    if (busqueda.value) {
        const texto = busqueda.value.toUpperCase();
        lista = lista.filter(p => getNombre(p).includes(texto) || getSku(p).includes(texto));
    }
    return lista;
});

const detectingTipo = (p: any) => detectarTipo(p);

const countMP = computed(() => listaProductos.value.filter(p => checkEsMP(p) && !esMpCliente(p) && !checkEsScrap(p)).length);
const countPT = computed(() => listaProductos.value.filter(p => checkEsPT(p)).length);
const countCLI = computed(() => listaProductos.value.filter(p => getClienteId(p) > 0).length);

const irAEditar = (id: number) => {
    router.push({ name: 'editar-producto', params: { id } });
};

const clickImportar = () => fileInput.value?.click();

const subirArchivoFlexxus = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (!target.files || target.files.length === 0) return;

    const archivo = target.files[0];
    if (!archivo) return;

    const esExcel = archivo.name.toLowerCase().endsWith('.xlsx');
    const esCsv = archivo.name.toLowerCase().endsWith('.csv');

    if (esCsv && tabActual.value === 'CLI' && !clienteFiltro.value) {
        alert("⚠️ ATENCIÓN:\nPara importar un CSV de stock específico, por favor seleccione primero el CLIENTE en el filtro.");
        target.value = ''; 
        return;
    }

    const formData = new FormData();
    formData.append('archivo', archivo);
    
    if (esCsv && clienteFiltro.value) {
        formData.append('clienteId', clienteFiltro.value.toString());
    }

    try {
        importando.value = true;
        let urlEndpoint = '';

        if (esExcel) {
            urlEndpoint = `/Integration/importar-excel-multicliente`;
        } else {
            urlEndpoint = `/Integration/importar-maestro`;
        }

        const res = await api.post(urlEndpoint, formData, {
            headers: { 
                'Content-Type': 'multipart/form-data' 
            }
        });

        alert(`✅ ÉXITO:\n${res.data.mensaje}`);
        
        if (res.data.logs && res.data.logs.length > 0) {
            console.warn("Reporte de Importación:", res.data.logs);
            alert("⚠️ Atención: Algunas hojas se saltaron. Revisa la consola (F12) para ver detalles.");
        }

        await cargarDatos();

    } catch (e: any) {
        console.error(e);
        const msg = e.response?.data || "Error al procesar el archivo.";
        alert(`❌ ERROR: ${msg}`);
    } finally {
        importando.value = false;
        if (fileInput.value) fileInput.value.value = '';
    }
};

// 👇 AQUÍ ESTABA EL PROBLEMA PRINCIPAL:
const cargarDatos = async () => {
    try {
        cargando.value = true;
        // 👇 Usamos 'api' y la ruta relativa (sin http://localhost...)
        const [resProd, resCli] = await Promise.all([
            api.get('/Productos'),
            api.get('/Clientes')
        ]);

        if (Array.isArray(resProd.data)) {
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
        // Mejor manejo de error para saber qué pasa
        console.error("Error cargando datos:", e);
        error.value = "Error conectando al servidor. Revisa la consola.";
        
        if (e.response && e.response.status === 401) {
             // El interceptor redirige, pero por si acaso:
             error.value = "Sesión caducada.";
        }
    } finally {
        cargando.value = false;
    }
};

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
                <input type="file" ref="fileInput" class="hidden-input" accept=".csv, .xlsx" @change="subirArchivoFlexxus" />

                <button class="btn-importar" @click="clickImportar" :disabled="importando || cargando">
                    <span v-if="importando">⏳ Procesando...</span>
                    <span v-else-if="tabActual === 'CLI' && clienteFiltro">
                        📥 Importar Stock {{ listaClientes.find(c => c.id === clienteFiltro)?.razonSocial }}
                    </span>
                    <span v-else>📥 Importar CSV Flexxus / Excel</span>
                </button>

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
                <button :class="{ 'sub-active': subTabCliente === 'SCRAP_CLI' }" @click="subTabCliente = 'SCRAP_CLI'">♻️ Scrap / MP</button>
                <button :class="{ 'sub-active': subTabCliente === 'PT_CLI' }" @click="subTabCliente = 'PT_CLI'">📤 Prod. Terminados</button>
                <button :class="{ 'sub-active': subTabCliente === 'MP_CLI' }" @click="subTabCliente = 'MP_CLI'">📥 Otros</button>
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
                        <th>Color / Var.</th>
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
                            <small v-if="checkEsFazon(p)" class="tag-fazon">FAZON</small>
                            <small v-if="checkEsScrap(p)" style="color: #d35400; font-weight:bold; margin-left:5px;">(SCRAP)</small>
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

                        <td>{{ p.color || '-' }}</td>
                        <td style="text-align:right; font-weight: bold;" :class="{'text-danger': p.stockActual <= 0}">{{ p.stockActual }}</td>
                        <td style="text-align:right;">{{ p.precioCosto ? `$${p.precioCosto}` : '-' }}</td>
                        <td style="text-align:center;">
                            <button @click="irAEditar(p.id)" class="btn-editar" title="Editar">✏️</button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div v-if="productosFiltrados.length === 0" class="vacio">
                <div v-if="tabActual === 'CLI' && !clienteFiltro" class="mensaje-guia">
                    <h3>👈 Selecciona un Cliente</h3>
                    <p>Elige un cliente para ver su stock de Scrap, MP o Productos.</p>
                </div>
                <div v-else>
                    No hay productos que coincidan con los filtros.
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.contenedor-stock { max-width: 1200px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }

.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; gap: 15px; }
.header-stock h2 { margin: 0; color: #2c3e50; }

.acciones-header { display: flex; gap: 10px; align-items: center; }
.hidden-input { display: none; } 

.btn-importar {
    background-color: #27ae60; color: white; border: none; padding: 10px 15px;
    border-radius: 6px; font-weight: bold; cursor: pointer; transition: background 0.2s;
    display: flex; align-items: center; gap: 5px;
}
.btn-importar:hover:not(:disabled) { background-color: #2ecc71; }
.btn-importar:disabled { background-color: #95a5a6; cursor: not-allowed; }

.buscador input { padding: 10px; width: 250px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }

.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; overflow-x: auto; }
.tab-btn {
    background: none; border: none; padding: 10px 20px; font-size: 1rem;
    color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s;
    font-weight: 600; display: flex; align-items: center; gap: 8px; white-space: nowrap;
}
.tab-btn:hover { background-color: #f7f9fa; color: #34495e; }
.tab-btn.active { background-color: #e3f2fd; color: #1976d2; }
.counter { background: #eee; padding: 2px 6px; border-radius: 10px; font-size: 0.8em; color: #555; }
.tab-btn.active .counter { background: #bbdefb; color: #0d47a1; }

.toolbar-clientes {
    background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px;
    border: 1px solid #e0e0e0; display: flex; flex-direction: column; gap: 15px;
}
.fila-filtros { display: flex; gap: 20px; flex-wrap: wrap; align-items: flex-end; }
.filtro-item { display: flex; flex-direction: column; gap: 5px; }
.filtro-item label { font-weight: bold; color: #555; font-size: 0.9rem; }
.filtro-item select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 220px; font-weight: 500; }

.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; width: fit-content; }
.sub-tabs button {
    border: none; background: transparent; padding: 6px 12px;
    font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500;
}
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }

.tabla-wrapper { overflow-x: auto; min-height: 300px; }
table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
th { background: #f8f9fa; text-align: left; padding: 12px; font-weight: 600; color: #555; border-bottom: 2px solid #eee; }
td { padding: 12px; border-bottom: 1px solid #f1f1f1; vertical-align: middle; color: #333; }
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

.bajo-stock td { background-color: #fff5f5; }
.text-danger { color: #e74c3c; }
.btn-editar { border: none; background: #e3f2fd; color: #1976d2; width: 35px; height: 35px; border-radius: 50%; cursor: pointer; transition: background 0.2s; font-size: 1.1rem; }
.btn-editar:hover { background: #bbdefb; }

.loading { text-align: center; padding: 40px; color: #3498db; font-size: 1.2rem; }
.vacio { text-align: center; padding: 30px; color: #999; font-style: italic; }
.error { color: red; text-align: center; }

.mensaje-guia { background: #e8f6f3; padding: 20px; border-radius: 8px; border: 2px dashed #1abc9c; color: #16a085; text-align: center; }
.mensaje-guia h3 { margin: 0 0 10px 0; }

@media (max-width: 768px) {
    .header-stock, .fila-filtros { flex-direction: column; align-items: flex-start; }
    .acciones-header, .filtro-item select, .btn-importar { width: 100%; }
}
</style>