<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router'; 
import axios from 'axios'; 

const listaInventario = ref([]); 
const busqueda = ref(''); 
const mostrandoModal = ref(false); 
const itemAjustar = ref({}); 
const router = useRouter(); 
const tabActiva = ref('mp'); 
const clienteFiltroFazon = ref('');

const getConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

const cargarInventario = async () => {
    const API_URL = 'https://localhost:7244/api/Productos/inventario-completo'; 
    try {
        const res = await axios.get(API_URL, getConfig());
        const datosReales = res.data;
        listaInventario.value = datosReales.map(item => ({
            ...item,
            stockMinimo: item.stockMinimo || 0, 
            estado: (item.stockActual < item.stockMinimo && !item.codigoSku.startsWith('MP-CLI-')) ? 'CRITICO' : 'OK'
        }));
    } catch (e) {
        if (e.response && e.response.status === 401) alert("Sesión expirada.");
        listaInventario.value = []; 
    }
};

onMounted(() => cargarInventario());

const listaClientesFazon = computed(() => {
    const items = listaInventario.value.filter(p => p.codigoSku.startsWith('MP-CLI-'));
    const names = new Set();
    items.forEach(i => {
        if(i.nombre.includes('PROPIEDAD DE ')) {
            names.add(i.nombre.split('PROPIEDAD DE ')[1].trim());
        }
    });
    return Array.from(names).sort();
});

const listaFiltrada = computed(() => {
    let items = [];

    if (tabActiva.value === 'mp') {
        items = listaInventario.value.filter(p => p.esMateriaPrima && !p.codigoSku.startsWith('MP-CLI-'));
    } 
    else if (tabActiva.value === 'pt') {
        items = listaInventario.value.filter(p => p.esProductoTerminado && !p.esFazon);
    } 
    else if (tabActiva.value === 'fazon') {
        items = listaInventario.value.filter(p => p.codigoSku.startsWith('MP-CLI-'));
        if (clienteFiltroFazon.value) {
            items = items.filter(p => p.nombre.includes(clienteFiltroFazon.value));
        }
    }

    const texto = busqueda.value.toLowerCase().trim();
    if (!texto) return items;

    return items.filter(item =>
        item.nombre.toLowerCase().includes(texto) ||
        item.codigoSku.toLowerCase().includes(texto)
    );
});

const totalItems = computed(() => listaFiltrada.value.length);
const itemsCriticos = computed(() => listaFiltrada.value.filter(item => item.estado === 'CRITICO').length);

const abrirAjuste = (item) => {
    itemAjustar.value = { ...item, stockRealNuevo: item.stockActual, motivo: '' }; 
    mostrandoModal.value = true;
};

const guardarAjuste = async () => {
    if (itemAjustar.value.stockRealNuevo === '') return alert('Ingrese un valor.');
    if (!itemAjustar.value.motivo) return alert('Ingrese un motivo.');

    try {
        const payload = {
            productoId: itemAjustar.value.id,
            cantidadReal: Number(itemAjustar.value.stockRealNuevo),
            motivo: itemAjustar.value.motivo
        };
        await axios.post('https://localhost:7244/api/Stock/ajuste', payload, getConfig());
        alert('Ajuste realizado correctamente');
        mostrandoModal.value = false;
        await cargarInventario(); 
    } catch (e) {
        alert("Error: " + (e.response?.data?.mensaje || e.message));
    }
};

const eliminarProducto = async (id, nombre) => {
    if (confirm(`¿Eliminar ${nombre}?`)) {
        try {
            await axios.delete(`https://localhost:7244/api/Productos/eliminar/${id}`, getConfig());
            listaInventario.value = listaInventario.value.filter(item => item.id !== id);
            mostrandoModal.value = false;
        } catch (error) {
            alert(`Error: ${error.response?.data?.mensaje || error.message}`);
        }
    }
};

const abrirCreacion = () => router.push({ name: 'crear-producto' });
const editarDatos = (id) => router.push({ name: 'editar-producto', params: { id: id } }); 
</script>

<template>
  <div class="panel-stock">
    
    <div class="header-top">
        <h2>📦 Control de Stock</h2>
        <div class="kpi-group">
            <span class="badge-kpi">Total: <strong>{{ totalItems }}</strong></span>
            <span v-if="itemsCriticos > 0 && tabActiva !== 'fazon'" class="badge-kpi danger">⚠️ Críticos: <strong>{{ itemsCriticos }}</strong></span>
        </div>
    </div>

    <div class="tabs-container">
        <button :class="{ active: tabActiva === 'mp' }" @click="tabActiva = 'mp'">🏭 Materia Prima</button>
        <button :class="{ active: tabActiva === 'pt' }" @click="tabActiva = 'pt'">📦 Prod. Terminado</button>
        <button :class="{ active: tabActiva === 'fazon' }" @click="tabActiva = 'fazon'" class="tab-fazon">🤝 Stock Clientes</button>
    </div>

    <div class="main-content" :class="{ 'layout-fazon': tabActiva === 'fazon' }">
        
        <div v-if="tabActiva === 'fazon'" class="sidebar-clientes">
            <h4>📁 Clientes</h4>
            <button :class="{ active: clienteFiltroFazon === '' }" @click="clienteFiltroFazon = ''">Ver Todos</button>
            <button v-for="cli in listaClientesFazon" :key="cli" :class="{ active: clienteFiltroFazon === cli }" @click="clienteFiltroFazon = cli">
                👤 {{ cli }}
            </button>
            <div v-if="listaClientesFazon.length === 0" class="empty-cli">Sin stock de terceros.</div>
        </div>

        <div class="tabla-container">
            <div class="buscador">
                <input v-model="busqueda" type="text" placeholder="🔍 Buscar..." />
                <button @click="cargarInventario">🔄</button>
                <button class="btn-nuevo" @click="abrirCreacion">➕ Nuevo</button>
            </div>

            <table class="tabla-stock">
                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Descripción</th>
                        <th class="text-right">Stock Actual</th>
                        <th v-if="tabActiva !== 'fazon'">Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in listaFiltrada" :key="item.id" :class="{ 'fila-critica': item.estado === 'CRITICO' }">
                        <td class="mono">{{ item.codigoSku }}</td>
                        <td>
                            <strong v-if="tabActiva === 'fazon'" style="color: #8e44ad;">
                                {{ item.nombre.replace(/PROPIEDAD DE .*/, '') }}
                                <span style="display:block; font-size:0.8em; color:#666; font-weight:normal;">
                                    DE {{ item.nombre.split('PROPIEDAD DE ')[1] }}
                                </span>
                            </strong>
                            <span v-else>{{ item.nombre }}</span>
                        </td>
                        <td class="numero">
                            <span v-if="tabActiva === 'fazon'" :class="item.stockActual < 0 ? 'text-red' : 'text-blue'">
                                {{ item.stockActual.toFixed(2) }} Kg
                            </span>
                            <span v-else>{{ item.stockActual.toFixed(2) }}</span>
                        </td>
                        <td v-if="tabActiva !== 'fazon'">
                            <span v-if="item.estado === 'CRITICO'" class="alerta">⚠️ BAJO</span>
                            <span v-else class="ok">OK</span>
                        </td>
                        <td class="acciones">
                            <button class="btn-icon" @click="abrirAjuste(item)">📝</button>
                            <button class="btn-icon blue" @click="editarDatos(item.id)">⚙️</button>
                        </td>
                    </tr>
                    <tr v-if="listaFiltrada.length === 0">
                        <td colspan="5" style="text-align: center; padding: 20px; color: #999;">Sin resultados.</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>

    <div v-if="mostrandoModal" class="modal-overlay">
      <div class="modal-content">
        <h3>⚖️ Ajuste de Inventario</h3>
        <p style="font-weight:bold; margin-bottom:15px; color:#555">{{ itemAjustar.nombre }}</p>
        <div class="grid-modal">
            <div class="campo">
                <label>Sistema:</label>
                <input type="text" :value="itemAjustar.stockActual" disabled style="background:#eee; text-align:center;">
            </div>
            <div class="campo">
                <label>Conteo Real:</label>
                <input type="number" v-model.number="itemAjustar.stockRealNuevo" style="font-size:1.2em; font-weight:bold; text-align:center; color:#27ae60;">
            </div>
        </div>
        <div class="campo">
          <label>Motivo:</label>
          <input type="text" v-model="itemAjustar.motivo" placeholder="Ej: Recuento, Rotura...">
        </div>
        <div class="botones-modal">
          <button @click="eliminarProducto(itemAjustar.id, itemAjustar.nombre)" class="btn-del">🗑️ Borrar</button>
          <div style="display:flex; gap:10px;">
              <button @click="mostrandoModal = false" class="btn-cancelar">Cancelar</button>
              <button @click="guardarAjuste" class="btn-guardar" :disabled="!itemAjustar.motivo">Confirmar</button>
          </div>
        </div>
      </div>
    </div>

  </div>
</template>

<style scoped>
.panel-stock { padding: 20px; background-color: #f4f6f8; min-height: 100vh; font-family: 'Segoe UI', sans-serif; }
.header-top { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; border-bottom: 2px solid #e0e0e0; padding-bottom: 10px; }
h2 { color: #2c3e50; margin: 0; }

.tabs-container { display: flex; gap: 5px; margin-bottom: 15px; }
.tabs-container button { padding: 10px 20px; border: none; background: #e0e0e0; color: #555; cursor: pointer; font-weight: bold; border-radius: 6px 6px 0 0; transition: 0.2s; }
.tabs-container button:hover { background: #d0d0d0; }
.tabs-container button.active { background: white; color: #3498db; border-top: 3px solid #3498db; box-shadow: 0 -2px 5px rgba(0,0,0,0.05); }
.tabs-container button.tab-fazon.active { border-top-color: #8e44ad; color: #8e44ad; }

.main-content { width: 100%; }
.layout-fazon { display: flex; gap: 20px; }
.tabla-container { flex-grow: 1; }

.sidebar-clientes { width: 220px; min-width: 220px; background: white; border-radius: 8px; padding: 15px; box-shadow: 0 2px 5px rgba(0,0,0,0.05); height: fit-content; }
.sidebar-clientes h4 { margin-top: 0; color: #8e44ad; border-bottom: 1px solid #eee; padding-bottom: 8px; }
.sidebar-clientes button { display: block; width: 100%; text-align: left; padding: 8px 10px; border: none; background: none; cursor: pointer; border-radius: 4px; margin-bottom: 2px; font-size: 0.9em; color: #555; }
.sidebar-clientes button:hover { background: #f3e5f5; color: #8e44ad; }
.sidebar-clientes button.active { background: #8e44ad; color: white; font-weight: bold; }
.empty-cli { font-size: 0.8em; color: #999; font-style: italic; margin-top: 10px; }

.buscador { display: flex; gap: 10px; margin-bottom: 15px; background: white; padding: 10px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.05); }
.buscador input { flex-grow: 1; padding: 8px 12px; border: 1px solid #ccc; border-radius: 4px; }
.buscador button { padding: 8px 15px; border: none; border-radius: 4px; cursor: pointer; background: #3498db; color: white; font-weight: bold; }
.buscador button.btn-nuevo { background: #27ae60; }

.tabla-stock { width: 100%; border-collapse: collapse; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }
.tabla-stock th { background: #34495e; color: white; padding: 12px; text-align: left; font-size: 0.9em; }
.tabla-stock td { padding: 10px 12px; border-bottom: 1px solid #eee; color: #333; }
.mono { font-family: monospace; color: #666; font-size: 1.1em; }
.numero { text-align: right; font-weight: bold; }
.text-right { text-align: right; }
.text-red { color: #c0392b; }
.text-blue { color: #2980b9; }

.alerta { background: #ffebee; color: #c62828; padding: 4px 8px; border-radius: 12px; font-size: 0.8em; font-weight: bold; border: 1px solid #ef9a9a; }
.ok { background: #e8f5e9; color: #2e7d32; padding: 4px 8px; border-radius: 12px; font-size: 0.8em; font-weight: bold; border: 1px solid #a5d6a7; }
.fila-critica { background-color: #fff8f8; }

.badge-kpi { background: #e3f2fd; padding: 5px 10px; border-radius: 4px; margin-left: 10px; font-size: 0.9em; }
.badge-kpi.danger { background: #ffebee; color: #c62828; }

.btn-icon { background: none; border: 1px solid #ccc; padding: 5px 8px; border-radius: 4px; cursor: pointer; transition: 0.2s; margin-right: 5px; }
.btn-icon:hover { background: #f0f0f0; transform: scale(1.1); }
.btn-icon.blue:hover { border-color: #3498db; background: #eef7fc; }

.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 2000; }
.modal-content { background: white; padding: 25px; border-radius: 8px; width: 400px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-content h3 { margin-top: 0; color: #3498db; border-bottom: 1px solid #eee; padding-bottom: 10px; }
.grid-modal { display: flex; gap: 15px; margin-bottom: 15px; }
.campo label { display: block; font-size: 0.85em; color: #7f8c8d; margin-bottom: 5px; font-weight: bold; }
.campo input { width: 100%; padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; box-sizing: border-box; }
.botones-modal { display: flex; justify-content: space-between; margin-top: 25px; border-top: 1px solid #eee; padding-top: 15px; }
.btn-cancelar { background: white; border: 1px solid #ccc; padding: 8px 15px; border-radius: 4px; cursor: pointer; }
.btn-guardar { background: #27ae60; color: white; border: none; padding: 8px 20px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-del { background: none; border: none; color: #c0392b; cursor: pointer; font-size: 0.9em; text-decoration: underline; }
</style>