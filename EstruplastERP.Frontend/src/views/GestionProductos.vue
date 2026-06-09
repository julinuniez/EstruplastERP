<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import { useRouter } from 'vue-router';
import { useMaestrosStore } from '@/stores/useMaestrosStore';
import { storeToRefs } from 'pinia';
import api from '@/services/axiosInstance';
import { useFiltrosInventario, detectarTipo } from '@/composables/useFiltrosInventario';
import { useImportacionInventario } from '@/composables/useImportacionInventario';
import { useModalesInventario } from '@/composables/useModalesInventario';
import ModalHistorialStock from '@/components/ModalHistorialStock.vue';
import ModalAjusteStock from '@/components/ModalAjusteStock.vue'; 
import ModalNuevoMasterbatch from '@/components/ModalNuevoMasterbatch.vue'; 
import { exportarInventarioExcel } from '@/composables/useExportacionInventario';
import { Alertas } from '@/utils/alertas';

// ... (Resto de tus imports y variables de estado igual que antes) ...
const router = useRouter();
<<<<<<< HEAD
const maestrosStore = useMaestrosStore();
const { productos: listaProductos, clientes: listaClientes, cargando } = storeToRefs(maestrosStore);
=======
const apiUrl = import.meta.env.VITE_API_URL || '/api';
const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]); 
>>>>>>> master
const busqueda = ref('');
const error = ref('');
const fileInputFlexxus = ref<HTMLInputElement | null>(null);

<<<<<<< HEAD
const tabActual = ref('MP');
const subTabMP = ref('VIRGEN'); 
const subTabCliente = ref('MOLIDO_CLI');
const clienteFiltro = ref<number | string>('');
const materialFiltro = ref<string>('');
const importClienteFiltro = ref<number | string>('');

const mostrarModalGlobal = ref(false);
const detalleGlobal = ref<any[]>([]);
const resumenGlobal = ref({ fisico: 0, reservado: 0, libre: 0 });

const mostrarModalHistorial = ref(false);
const productoIdSeleccionado = ref<number | null>(null);
const productoNombreSeleccionado = ref('');

const mostrarModalAjuste = ref(false);
const productoParaAjustar = ref<any>(null);

const mostrarModalMasterbatch = ref(false);

const exportando = ref(false);

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
=======
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
>>>>>>> master


// --- IMPORTACIÓN FLEXXUS MODIFICADA 🆕 ---
const clickImportarFlexxus = () => {
    fileInputFlexxus.value?.click();
};

<<<<<<< HEAD
const abrirKardex = (producto: any) => {
    productoIdSeleccionado.value = producto.id;
    productoNombreSeleccionado.value = producto.nombre;
    mostrarModalHistorial.value = true;
};

const abrirModalAjuste = (p: any) => {
    productoParaAjustar.value = p;
    mostrarModalAjuste.value = true;
};

const onAjusteConfirmado = () => {
    mostrarModalAjuste.value = false;
    cargarDatos(true); 
};

async function cargarDatos(forzar = false) {
    try {
        error.value = '';
        await maestrosStore.cargarDatosMaestros(forzar);
=======
const procesarArchivoFlexxus = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    
    // 1. Validamos la lista de archivos
    if (!target.files || target.files.length === 0) return;

    const file = target.files[0];
    
    // 🛑 FIX TYPESCRIPT: Agrega esta línea
    // Esto le asegura al compilador que 'file' no es undefined
    if (!file) return; 

    // Validar extensión simple
    if (!file.name.endsWith('.csv') && !file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
        alert('Por favor selecciona un archivo CSV o Excel válido.');
        return;
    }

    // Ahora TS ya no se quejará aquí de que file.name puede ser undefined
    if (!confirm(`¿Procesar archivo "${file.name}"?`)) {
        target.value = ''; 
        return;
    }

    try {
        cargando.value = true;
        
        const formData = new FormData();
        // Ni tampoco se quejará aquí
        formData.append('archivo', file); 

        const response = await axios.post(`${apiUrl}/Flexxus/importar-mp`, formData, {
            headers: {
                ...getAuthConfig().headers,
                'Content-Type': 'multipart/form-data'
            }
        });

        // Actualizamos datos del modal
        resumenData.value = {
            creados: response.data.creados,
            actualizados: response.data.actualizados,
            errores: response.data.errores || 0
        };
        mostrarResumen.value = true;

        await recargarDatos(); 

    } catch (e: any) {
        console.error(e);
        const msg = e.response?.data?.message || 'Error al importar archivo.';
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
>>>>>>> master
    } catch (e: any) {
        error.value = "Error conectando al servidor. Revisa la consola.";
    }
<<<<<<< HEAD
}

const abrirPantallazoGlobal = (producto: any) => {
    const familiaOriginal = detectarTipo(producto).toUpperCase();

    detalleGlobal.value = listaProductos.value.filter(item => {
        const n = (item.nombre || '').toUpperCase();
        const sku = (item.codigoSku || '').toUpperCase();
        const textoCompleto = n + ' ' + sku;
        
        const esTuti = textoCompleto.includes('TUTI');
        
        const esMismaFamilia = detectarTipo(item).toUpperCase() === familiaOriginal;

        return esTuti && esMismaFamilia;
    });

    resumenGlobal.value.fisico = detalleGlobal.value.reduce((acc, item) => acc + (item.stockFisico ?? item.stockActual ?? 0), 0);
    resumenGlobal.value.reservado = detalleGlobal.value.reduce((acc, item) => acc + (item.stockReservado ?? 0), 0);
    resumenGlobal.value.libre = resumenGlobal.value.fisico - resumenGlobal.value.reservado;

    mostrarModalGlobal.value = true;
};

const descargarAuditoria = async () => {
    if (!listaProductos.value || listaProductos.value.length === 0) {
        // 🚀 Reemplazo de alert por Alertas.advertencia
        Alertas.advertencia("No hay datos de inventario para exportar.");
        return;
    }
    
    try {
        exportando.value = true;
        await exportarInventarioExcel(listaProductos.value, listaClientes.value);
    } catch (e) {
        console.error("Error al generar Excel:", e);
        // 🚀 Reemplazo de alert por Alertas.error
        Alertas.error("Hubo un problema al crear el archivo Excel.");
    } finally {
        exportando.value = false;
    }
};

const verMolidoAgrupado = ref(true);
const gruposExpandidos = ref<string[]>([]);

const toggleGrupo = (id: string) => {
    const index = gruposExpandidos.value.indexOf(id);
    if (index === -1) gruposExpandidos.value.push(id);
    else gruposExpandidos.value.splice(index, 1);
};

const molidosAgrupados = computed(() => {
    if (tabActual.value !== 'CLI' || subTabCliente.value !== 'MOLIDO_CLI') return [];

    const grupos: Record<string, any> = {};

    const baseList = listaProductos.value.filter(p => (p.codigoSku || '').toUpperCase().includes('MOL'));

    baseList.forEach(p => {
        if (clienteFiltro.value && getClienteId(p) !== Number(clienteFiltro.value)) return;

        const sku = (p.codigoSku || '').toUpperCase();
        const nombre = (p.nombre || '').toUpperCase();
        
        const partesSku = sku.split('-');
        
        const materialBruto = partesSku.length > 1 ? partesSku[1].trim() : 'VARIOS';
        
        const material = materialBruto.replace(/[0-9]/g, '').trim() || 'VARIOS';

        let colorRaw = nombre.replace(/MOLIDO/i, '')
                             .replace(new RegExp(materialBruto, 'i'), '')
                             .replace(new RegExp(material, 'i'), '');
                             
        colorRaw = colorRaw.replace(/\(.*?\)/g, ''); 
        const color = colorRaw.replace(/[^a-zA-Z0-9ñÑáéíóúÁÉÍÓÚ ]/g, '').trim().toUpperCase() || 'SIN COLOR';

        const key = `${material}_${color}`;
        
        if (!grupos[key]) {
            grupos[key] = {
                idGrupo: key,
                material: material, 
                color: color,
                fisico: 0,
                reservado: 0,
                disponible: 0,
                items: []
            };
        }

        const fisico = p.stockFisico ?? p.stockActual ?? 0;
        const reservado = p.stockReservado ?? 0;
        const disponible = p.stockDisponible ?? (fisico - reservado);

        grupos[key].fisico += fisico;
        grupos[key].reservado += reservado;
        grupos[key].disponible += disponible;
        grupos[key].items.push(p);
    });

    return Object.values(grupos).sort((a: any, b: any) => {
        if (a.material !== b.material) return a.material.localeCompare(b.material);
        return a.color.localeCompare(b.color);
    });
});

const listaProveedores = ref<any[]>([]);

const cargarProveedores = async () => {
    try {
        const response = await api.get('/Proveedores'); 
        listaProveedores.value = response.data;
    } catch (e) {
        console.error("Error al cargar proveedores", e);
    }
};

const descargandoExcelFazon = ref(false);

const generarExcelFazon = async (clienteId: number) => {
    if (descargandoExcelFazon.value) return;
    descargandoExcelFazon.value = true;

    try {
        const clienteSeleccionado = listaClientes.value.find(c => Number(c.id) === clienteId);
        const nombreClienteReal = clienteSeleccionado ? clienteSeleccionado.razonSocial : 'Cliente_Desconocido';
        const fechaHoy = new Date().toISOString().split('T')[0];
        
        const inventarioCliente = listaProductos.value
            .filter(p => Number(getClienteId(p)) === Number(clienteId))
            .sort((a, b) => {
                const comparacionSku = (a.codigoSku || '').localeCompare(b.codigoSku || '');
                if (comparacionSku !== 0) return comparacionSku;
                return (a.nombre || '').localeCompare(b.nombre || '');
            });

        const workbook = new ExcelJS.Workbook();
        
        const wsInv = workbook.addWorksheet('Inventario Actual');
        
        wsInv.mergeCells('A1:C1');
        wsInv.getCell('A1').value = `REPORTE DE INVENTARIO - ${nombreClienteReal.toUpperCase()}`;
        wsInv.getCell('A1').font = { size: 14, bold: true, color: { argb: 'FF2980B9' } };
        wsInv.getCell('A2').value = `Fecha de emisión: ${new Date().toLocaleDateString('es-AR')}`;

        const columnsInv = [
            { header: 'SKU', key: 'sku', width: 20 },
            { header: 'Nombre del Producto / Material', key: 'nombre', width: 45 },
            { header: 'Stock Disponible (kg)', key: 'stock', width: 25 }
        ];
        wsInv.columns = columnsInv;

        wsInv.getRow(4).eachCell((cell) => {
            cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF2980B9' } };
            cell.font = { color: { argb: 'FFFFFFFF' }, bold: true };
            cell.alignment = { horizontal: 'center' };
        });

        if (inventarioCliente.length > 0) {
            inventarioCliente.forEach((p) => {
                const fisico = p.stockFisico ?? p.stockActual ?? 0;
                const reservado = p.stockReservado ?? 0;
                const disponible = p.stockDisponible ?? (fisico - reservado);

                const nombreLimpio = (p.nombre || 'Sin Nombre').replace(/\[MOLIDO\]/gi, '').trim();

                const row = wsInv.addRow({ 
                    sku: p.codigoSku || '-', 
                    nombre: nombreLimpio, 
                    stock: disponible 
                });
                row.getCell('stock').numFmt = '#,##0.00';
            });
        } else {
            wsInv.addRow({ sku: '', nombre: 'No hay productos en stock para este cliente', stock: 0 });
        }

        const nombreArchivoLimpio = nombreClienteReal.replace(/[^a-zA-Z0-9]/g, '_'); 
        const buffer = await workbook.xlsx.writeBuffer();
        saveAs(new Blob([buffer]), `Reporte_Inventario_${nombreArchivoLimpio}_${fechaHoy}.xlsx`);

    } catch (e) {
        console.error("Error excel fazon:", e);
        // 🚀 Reemplazo de alert por Alertas.error
        Alertas.error("Error al generar el Excel. Revisa la consola.");
    } finally {
        descargandoExcelFazon.value = false;
    }
};

onMounted(() => {
    cargarDatos(true);
    cargarProveedores();
});
=======
};
onMounted(() => { recargarDatos(); });
>>>>>>> master
</script>

<template>
    <div class="contenedor-stock">
        <div class="header-stock">
<<<<<<< HEAD
            <div style="display: flex; flex-direction: column;">
                <h2>📦 Gestión de Stock</h2>
                <small style="color: #7f8c8d;">Administración de inventario</small>
            </div>
            
            <div class="acciones-header">
                <button 
                    @click="descargarAuditoria" 
                    :disabled="exportando || cargando"
                    style="background-color: #27ae60; color: white; border: none; padding: 8px 12px; border-radius: 4px; font-weight: bold; cursor: pointer; margin-right: 10px;"
                >
                    <span v-if="exportando">⏳ Generando...</span>
                    <span v-else>📊 Planilla Conteo</span>
                </button>

                <button class="btn-nueva-mp" @click="mostrarModalNuevaMP = true">➕ Crear Insumo</button>
                
                <button class="btn-masterbatch" @click="mostrarModalMasterbatch = true">🎨 Alta Color</button>

                <input type="file" ref="fileInput" class="hidden-input" accept=".csv, .xlsx" @change="subirArchivoFlexxus" />
                <div class="import-group">
                    <select v-model="importClienteFiltro" class="select-import" :disabled="importando || cargando">
                        <option value="">🏢 Importación Completa (Todos los clientes)</option>
                        <option v-for="c in listaClientes" :key="c.id" :value="c.id">📄 {{ c.razonSocial }}</option>
                    </select>
                    <button class="btn-importar" @click="clickImportar" :disabled="importando || cargando">
                        <span v-if="importando">⏳ Procesando...</span>
                        <span v-else>📥 Importar</span>
                    </button>
                </div>
=======
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

>>>>>>> master
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

<<<<<<< HEAD
        <div v-if="tabActual === 'MP'" class="toolbar-clientes" style="margin-top: -10px;">
            <div class="sub-tabs">
                <button :class="{ 'sub-active': subTabMP === 'VIRGEN' }" @click="subTabMP = 'VIRGEN'">🧪 Material Virgen</button>
                <button :class="{ 'sub-active': subTabMP === 'ADITIVOS' }" @click="subTabMP = 'ADITIVOS'">⚙️ Aditivos</button>
                <button :class="{ 'sub-active': subTabMP === 'MASTERBATCH' }" @click="subTabMP = 'MASTERBATCH'">🎨 Masterbatch / Colores</button>
                <button :class="{ 'sub-active': subTabMP === 'MOLIDO_PROPIO' }" @click="subTabMP = 'MOLIDO_PROPIO'">♻️ Molienda Propia</button>
            </div>
        </div>

        <div v-if="tabActual === 'CLI'" class="toolbar-clientes">
            <div class="fila-filtros">
                
                <div class="filtro-item">
                    <label>🏢 Cliente:</label>
                    <select v-model="clienteFiltro">
                        <option value="">Todos los Clientes (Global)</option>
                        <option v-for="c in clientesFazon" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                    </select>
                </div>

                <div class="filtro-item" v-if="clienteFiltro">
                    <label>🧱 Material:</label>
                    <select v-model="materialFiltro">
                        <option value="">Todos los Materiales</option>
                        <option v-for="mat in TIPOS_MATERIALES" :key="mat" :value="mat">{{ mat }}</option>
                    </select>
                </div>

                <div class="filtro-item" v-if="clienteFiltro" style="align-self: flex-end; margin-bottom: 2px;">
                    <button 
                        @click="generarExcelFazon(Number(clienteFiltro))" 
                        :disabled="descargandoExcelFazon"
                        style="background-color: #2980b9; color: white; border: none; padding: 0 15px; border-radius: 4px; font-weight: bold; cursor: pointer; display: flex; align-items: center; gap: 5px; height: 35px;"
                    >
                        <span v-if="descargandoExcelFazon">⏳ Generando...</span>
                        <span v-else>📊 Exportar Reporte Fazón</span>
                    </button>
                </div>

                <div class="filtro-item" style="margin-left: auto; align-self: flex-end; margin-bottom: 2px;">
                    <label class="toggle-agrupado" style="height: 35px; display: flex; align-items: center; margin: 0; padding: 0 15px; box-sizing: border-box; cursor: pointer;">
                        <input type="checkbox" v-model="verMolidoAgrupado" style="margin-right: 8px; cursor: pointer;"> 📂 Agrupar por Color
                    </label>
                </div>

=======
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
>>>>>>> master
            </div>
        </div>

        <div v-if="cargando" class="loading">⏳ Procesando...</div>
        <div v-else-if="error" class="error">{{ error }}</div>

        <div v-else class="tabla-wrapper">
            
            <table v-if="tabActual === 'CLI' && subTabCliente === 'MOLIDO_CLI' && verMolidoAgrupado">
                <thead>
                    <tr>
                        <th style="width: 30px;"></th>
                        <th>Material / Color Agrupado</th>
                        <th style="text-align:right; width: 120px;">Físico Total</th>
                        <th style="text-align:center; width: 120px;">Reservado</th>
                        <th style="text-align:right; width: 120px;">Disponible</th>
                    </tr>
                </thead>
                <tbody v-for="grupo in molidosAgrupados" :key="grupo.idGrupo">
                    <tr class="fila-grupo" @click="toggleGrupo(grupo.idGrupo)">
                        <td style="text-align:center; color:#7f8c8d; font-size:0.8rem;">
                            {{ gruposExpandidos.includes(grupo.idGrupo) ? '▼' : '▶' }}
                        </td>
                        <td>
                            <span class="badge-material" :class="grupo.material.toLowerCase().replace(' ', '-')">{{ grupo.material }}</span>
                            <strong style="margin-left: 10px; color:#2c3e50;">{{ grupo.color }}</strong>
                            <small style="color:#95a5a6; margin-left: 5px;">({{ grupo.items.length }} bolsas/lotes)</small>
                        </td>
                        <td style="text-align:right; font-weight:bold; font-size:1.1rem;">{{ grupo.fisico.toFixed(2) }}</td>
                        <td style="text-align:center; font-weight:bold; color:#e67e22;">{{ grupo.reservado.toFixed(2) }}</td>
                        <td style="text-align:right; font-weight:bold; font-size:1.1rem;" :class="{'text-danger': grupo.disponible <= 0, 'text-success': grupo.disponible > 0}">
                            {{ grupo.disponible.toFixed(2) }}
                        </td>
                    </tr>
                    
                    <template v-if="gruposExpandidos.includes(grupo.idGrupo)">
                        <tr v-for="p in grupo.items" :key="p.id" class="fila-detalle-grupo">
                            <td></td>
                            <td style="padding-left: 10px;">
                                <span class="sku">{{ p.codigoSku }}</span>
                                <span style="margin-left:8px; color:#555;">{{ p.nombre }}</span>
                                <span v-if="getClienteId(p) > 0" class="badge-fazon" style="margin-left:8px;">
                                    {{ listaClientes.find(c => Number(c.id) === Number(getClienteId(p)))?.razonSocial || 'Cliente' }}
                                </span>
                                <span v-else class="badge-propio" style="margin-left:8px;">Propio</span>
                                
                                <button @click="irAEditar(p.id)" class="btn-editar btn-mini" title="Editar Lote" style="margin-left: 10px;">✏️</button>
                                <button @click="abrirModalAjuste(p)" class="btn-editar btn-mini" style="margin-left: 5px; background: #fdf2e9; color: #e67e22;" title="Ajustar Stock Manual">⚖️</button>
                                <button @click="abrirKardex(p)" class="btn-editar btn-mini" title="Ver Historial de Movimientos" style="margin-left: 5px;">🕒</button>
                            </td>
                            <td style="text-align:right; color:#7f8c8d;">{{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}</td>
                            <td style="text-align:center; color:#7f8c8d;">
                                <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-buchon" style="padding: 2px 6px; font-size:0.7rem;">
                                    🔒 {{ p.stockReservado }}
                                </button>
                                <span v-else>-</span>
                            </td>
                            <td style="text-align:right; color:#7f8c8d;">{{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}</td>
                        </tr>
                    </template>
                </tbody>
            </table>

            <table v-else-if="productosFiltrados.length > 0 || (tabActual === 'CLI' && subTabCliente === 'MOLIDO_CLI' && !verMolidoAgrupado && molidosAgrupados.length > 0)">
                <thead>
                    <tr>
                        <th>SKU</th>
                        <th>Descripción</th>
<<<<<<< HEAD
                        <th v-if="tabActual === 'PT' || tabActual === 'CLI'">Dueño</th>
                        
                        <th v-if="tabActual === 'MP'">Proveedor</th> 

                        <th v-if="tabActual === 'CLI'">Material</th> 
                        <th style="text-align:right; width: 90px;" title="Stock Real en Galpón">Físico (Kg)</th>
                        <th style="text-align:center; width: 90px;" title="Retenido en Órdenes de Producción">Reservado</th>
                        <th style="text-align:right; width: 90px;" title="Stock Libre para usar">Disponible</th>
=======
                        <th v-if="tabActual === 'CLI'">Cliente</th>
                        <th>Color</th>
                        <th style="text-align:right">Stock (Kg)</th>
                        <th style="text-align:right">Costo ($)</th>
>>>>>>> master
                        <th style="text-align:center">Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="p in (tabActual === 'CLI' && subTabCliente === 'MOLIDO_CLI' && !clienteFiltro ? listaProductos.filter(x => (x.codigoSku || '').toUpperCase().includes('MOL')) : productosFiltrados)" :key="p.id" 
                        :class="{'bajo-stock': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= p.stockMinimo}">
                        
                        <td class="sku">{{ p.codigoSku }}</td>
                        <td>
                            <div class="nombre-prod">{{ p.nombre }}</div>
                            <small v-if="checkEsFazon(p)" class="tag-fazon">FAZON</small>
                            <small v-if="checkEsMolido(p)" style="color: #27ae60; font-weight:bold; margin-left:5px;">(MOLIDO)</small>
                            <small v-else-if="checkEsScrap(p)" style="color: #d35400; font-weight:bold; margin-left:5px;">(SCRAP)</small>
                            
                            <button 
    v-if="p.nombre.toUpperCase().includes('TUTI') && (!getClienteId(p) || getClienteId(p) === 0)" 
    @click="abrirPantallazoGlobal(p)" 
    class="btn-global"
    title="Ver Stock Global de todos los clientes"
>
    🌍 Ver Total Global
</button>
                        </td>
                        
                        <td v-if="tabActual === 'PT' || tabActual === 'CLI'">
                            <span v-if="getClienteId(p) > 0" class="badge-cliente">
                                {{ listaClientes.find(c => Number(c.id) === Number(getClienteId(p)))?.razonSocial || 'Cliente #' + getClienteId(p) }}
                            </span>
                            <span v-else class="badge-propio">Propio</span>
                        </td>

                        <td v-if="tabActual === 'MP'">
                            <span v-if="p.proveedorNombre" class="badge-proveedor">
                                {{ p.proveedorNombre }}
                            </span>
                            <span v-else style="color: #bdc3c7; font-size: 0.8rem;">-</span>
                        </td>
<<<<<<< HEAD

                        <td v-if="tabActual === 'CLI'">
                            <span class="badge-material" v-if="detectarTipo(p) !== 'OTROS'" :class="detectarTipo(p).toLowerCase().replace(' ', '-')">
                                {{ detectarTipo(p) }}
                            </span>
                        </td>

                        <td style="text-align:right; font-weight: 500;">
                            {{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}
                        </td>

=======
                        <td v-if="tabActual === 'CLI'">{{ listaClientes.find(c => c.id === p.clienteId)?.razonSocial || 'N/A' }}</td>
                        <td>{{ p.color || '-' }}</td>
                        <td style="text-align:right; font-weight: bold;" :class="{'text-danger': p.stockActual <= 0}">{{ p.stockActual }}</td>
                        <td style="text-align:right;">{{ p.precioCosto ? `$${p.precioCosto}` : '-' }}</td>
>>>>>>> master
                        <td style="text-align:center;">
                            <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-buchon" title="Ver dónde está reservado">
                                🔒 {{ p.stockReservado }}
                            </button>
                            <span v-else style="color: #bdc3c7;">-</span>
                        </td>

                        <td style="text-align:right; font-weight: bold; font-size: 1.05rem;" 
                            :class="{'text-danger': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= 0}">
                            {{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}
                        </td>

                        <td style="text-align:center; display: flex; justify-content: center; gap: 5px;">
                            <button @click="irAEditar(p.id)" class="btn-editar" title="Editar">✏️</button>
                            <button @click="abrirModalAjuste(p)" class="btn-editar" style="background: #fdf2e9; color: #e67e22;" title="Ajustar Stock Manual">⚖️</button>
                            <button @click="abrirKardex(p)" class="btn-editar" title="Ver Historial de Movimientos">🕒</button>
                        </td>
                    </tr>
                </tbody>
            </table>
<<<<<<< HEAD

            <div v-else class="vacio">
                <div v-if="tabActual === 'CLI' && subTabCliente !== 'MOLIDO_CLI' && !clienteFiltro" class="mensaje-guia">
                    <h3>👈 Selecciona un Cliente</h3>
                    <p>Para ver Productos Terminados o Materia Prima de Fazon, primero debes seleccionar a quién pertenecen.</p>
                </div>
                <div v-else>
                    No hay productos que coincidan con los filtros actuales.
=======
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
>>>>>>> master
                </div>
            </div>
            
            <p v-if="resumenData.errores > 0" class="text-error">
                ⚠️ Se ignoraron {{ resumenData.errores }} filas por errores.
            </p>

            <button class="btn-cerrar" @click="cerrarModal">Aceptar</button>
        </div>

        <div v-if="mostrarModalGlobal" class="modal-overlay" @click.self="mostrarModalGlobal = false">
            <div class="modal-content modal-lg">
                <div class="modal-header">
                    <h3 style="margin-top:0; border-bottom:none;">🌍 Pantallazo Global de Material: "TUTI"</h3>
                    <button class="btn-cancelar" @click="mostrarModalGlobal = false" style="padding: 2px 8px;">✕</button>
                </div>
                
                <p style="font-size:0.85rem; color:#7f8c8d;">Suma del material propio de Estruplast + el material retenido de todos los clientes.</p>

                <div class="tarjetas-resumen">
                    <div class="tarjeta-stock fisico">
                        <span>FÍSICO TOTAL EN PLANTA</span>
                        <h2>{{ resumenGlobal.fisico.toFixed(2) }} Kg</h2>
                    </div>
                    <div class="tarjeta-stock reservado">
                        <span>TOTAL COMPROMETIDO</span>
                        <h2>{{ resumenGlobal.reservado.toFixed(2) }} Kg</h2>
                    </div>
                    <div class="tarjeta-stock libre">
                        <span>LIBRE REAL TOTAL</span>
                        <h2>{{ resumenGlobal.libre.toFixed(2) }} Kg</h2>
                    </div>
                </div>

                <h4 style="margin-top: 20px; color:#2c3e50; border-bottom: 1px solid #eee; padding-bottom:5px;">📋 Desglose por Dueño / Cliente</h4>
                <div class="caja-pedidos-reservados">
                    <table class="tabla-reservas">
                        <thead>
                            <tr>
                                <th>Material</th>
                                <th>Dueño / Cliente</th>
                                <th style="text-align: right;">Físico</th>
                                <th style="text-align: right;">Reservado</th>
                                <th style="text-align: right;">Libre</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="t in detalleGlobal" :key="t.id">
                                <td><strong>{{ t.nombre }}</strong></td>
                                <td>
                                    <span v-if="getClienteId(t) > 0" class="badge-fazon">
                                        {{ listaClientes.find(c => Number(c.id) === Number(getClienteId(t)))?.razonSocial || t.clienteNombre || 'Cliente #' + getClienteId(t) }}
                                    </span>
                                    <span v-else class="badge-propio">Propio (Estruplast)</span>
                                </td>
                                <td style="text-align: right;">{{ (t.stockFisico ?? t.stockActual ?? 0).toFixed(2) }}</td>
                                <td style="text-align: right; color: #e67e22;">{{ (t.stockReservado || 0).toFixed(2) }}</td>
                                <td style="text-align: right;" :class="{'text-danger': ((t.stockFisico ?? t.stockActual ?? 0) - (t.stockReservado ?? 0)) < 0}">
                                    <strong>{{ ((t.stockFisico ?? t.stockActual ?? 0) - (t.stockReservado ?? 0)).toFixed(2) }}</strong>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div style="text-align: right; margin-top: 15px;">
                    <button class="btn-cancelar" @click="mostrarModalGlobal = false">Cerrar</button>
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

                <label style="display:block; margin-bottom:5px; font-weight:bold; font-size: 0.9rem; color: #8e44ad;">Proveedor (Opcional):</label>
                <select v-model="nuevaMP.proveedorId" class="input-modal" style="background-color: #f5eef8; border-color: #d2b4de;">
                    <option :value="null">Sin Proveedor</option>
                    
                    <option v-for="prov in listaProveedores" :key="prov.id" :value="prov.id">
                        {{ prov.razonSocial }}
                    </option>
                </select>

                <div class="modal-acciones">
                    <button class="btn-cancelar" @click="mostrarModalNuevaMP = false">Cancelar</button>
                    <button class="btn-guardar-mp" @click="guardarNuevaMateriaPrima" :disabled="guardandoMP">
                        {{ guardandoMP ? 'Guardando...' : '💾 Guardar Insumo' }}
                    </button>
                </div>
            </div>
        </div>

        <ModalHistorialStock 
            :visible="mostrarModalHistorial"
            :productoId="productoIdSeleccionado"
            :productoNombre="productoNombreSeleccionado"
            @close="mostrarModalHistorial = false"
        />

        <ModalAjusteStock 
            :visible="mostrarModalAjuste"
            :producto="productoParaAjustar"
            @close="mostrarModalAjuste = false"
            @confirmado="onAjusteConfirmado"
        />

        <ModalNuevoMasterbatch 
            :visible="mostrarModalMasterbatch"
            @close="mostrarModalMasterbatch = false"
            @creado="cargarDatos(true)"
        />

    </div>
</template>

<style scoped>
/* ... MANTENER ESTILOS VIEJOS ... */
.contenedor-stock { max-width: 1200px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
<<<<<<< HEAD

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

.btn-masterbatch { background: #8b5cf6; color: white; border: none; padding: 10px 15px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; display: flex; align-items: center; gap: 5px; }
.btn-masterbatch:hover { background: #7c3aed; }

.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; }
.modal-content { 
    background: white; padding: 25px; border-radius: 8px; width: 400px; 
    max-width: 95vw; max-height: 90vh; display: flex; flex-direction: column; 
    box-shadow: 0 10px 30px rgba(0,0,0,0.3); 
}
.modal-content h3 { margin-top: 0; color: #2c3e50; border-bottom: 2px solid #eee; padding-bottom: 10px; }

.input-modal { width: 100%; padding: 10px; margin-bottom: 15px; border: 1px solid #ccc; border-radius: 4px; font-weight: bold; text-transform: uppercase; box-sizing: border-box; }

.modal-acciones { display: flex; justify-content: flex-end; gap: 10px; margin-top: 10px; }
.btn-cancelar { background: #e74c3c; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-guardar-mp { background: #27ae60; color: white; border: none; padding: 8px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-guardar-mp:disabled { background: #95a5a6; cursor: not-allowed; }

.buscador input { padding: 10px; width: 250px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }

.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; overflow-x: auto; }
.tab-btn { background: none; border: none; padding: 10px 20px; font-size: 1rem; color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s; font-weight: 600; display: flex; align-items: center; gap: 8px; white-space: nowrap; }
=======
.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; flex-wrap: wrap; gap: 15px;}
.header-stock h2 { margin: 0; color: #2c3e50; }
.acciones-header { display: flex; gap: 10px; align-items: center; }
.btn-flexxus { background-color: #27ae60; color: white; border: none; padding: 10px 15px; border-radius: 5px; cursor: pointer; font-weight: bold; display: flex; align-items: center; gap: 8px; transition: background 0.3s; }
.btn-flexxus:hover { background-color: #219150; }
.info-csv { font-size: 0.8rem; color: #7f8c8d; font-style: italic; }
.buscador input { padding: 10px; width: 300px; border: 1px solid #ccc; border-radius: 5px; font-size: 1rem; }
.tabs-container { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; padding-bottom: 10px; }
.tab-btn { background: none; border: none; padding: 10px 20px; font-size: 1rem; color: #7f8c8d; cursor: pointer; border-radius: 6px; transition: all 0.3s; font-weight: 600; display: flex; align-items: center; gap: 8px; }
>>>>>>> master
.tab-btn:hover { background-color: #f7f9fa; color: #34495e; }
.tab-btn.active { background-color: #e3f2fd; color: #1976d2; }
.counter { background: #eee; padding: 2px 6px; border-radius: 10px; font-size: 0.8em; color: #555; }
.tab-btn.active .counter { background: #bbdefb; color: #0d47a1; }
<<<<<<< HEAD

.toolbar-clientes { background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #e0e0e0; display: flex; flex-direction: column; gap: 15px; }
.fila-filtros { display: flex; gap: 20px; flex-wrap: wrap; align-items: flex-end; }
.filtro-item { display: flex; flex-direction: column; gap: 5px; }
.filtro-item label { font-weight: bold; color: #555; font-size: 0.9rem; }
.filtro-item select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 220px; font-weight: 500; }

.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; width: fit-content; }
.sub-tabs button { border: none; background: transparent; padding: 6px 12px; font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500; transition: all 0.2s; }
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }

/* ESTILOS VISTA AGRUPADA */
.toggle-agrupado { margin-left: auto; display: flex; align-items: center; background: #fff3e0; padding: 4px 10px; border-radius: 6px; border: 1px solid #f39c12; color: #d35400; font-weight: bold; cursor: pointer; font-size: 0.85rem; }
.toggle-agrupado input { margin-right: 5px; cursor: pointer; }
.fila-grupo { background-color: #f8fafc; cursor: pointer; transition: background 0.2s; border-top: 2px solid #e2e8f0; }
.fila-grupo:hover { background-color: #e2e8f0; }
.fila-detalle-grupo { background-color: #ffffff; font-size: 0.9rem; border-bottom: 1px dashed #f1f5f9; }
.text-success { color: #27ae60; }
.btn-mini { width: 24px; height: 24px; font-size: 0.8rem; margin-left: 10px; }

=======
.toolbar-clientes { background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; display: flex; align-items: center; justify-content: space-between; border: 1px solid #e0e0e0; }
.filtro-cliente { display: flex; align-items: center; gap: 10px; font-weight: bold; color: #555; }
.filtro-cliente select { padding: 8px; border: 1px solid #ccc; border-radius: 4px; min-width: 200px; }
.sub-tabs { display: flex; gap: 5px; background: #e0e0e0; padding: 4px; border-radius: 6px; }
.sub-tabs button { border: none; background: transparent; padding: 6px 12px; font-size: 0.9rem; color: #666; cursor: pointer; border-radius: 4px; font-weight: 500; }
.sub-tabs button.sub-active { background: white; color: #2c3e50; font-weight: bold; shadow: 0 2px 4px rgba(0,0,0,0.1); }
>>>>>>> master
.tabla-wrapper { overflow-x: auto; min-height: 300px; }
table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
th { background: #f8f9fa; text-align: left; padding: 12px; font-weight: 600; color: #555; border-bottom: 2px solid #eee; }
td { padding: 12px; border-bottom: 1px solid #f1f1f1; vertical-align: middle; color: #333; transition: background-color 0.2s; }
tr:hover { background-color: #f9f9f9; }
.sku { font-family: 'Courier New', monospace; font-weight: bold; color: #666; font-size: 0.9em; }
.nombre-prod { font-weight: 600; color: #2c3e50; }
.tag-fazon { background: purple; color: white; padding: 2px 4px; border-radius: 4px; font-size: 0.7em; margin-left: 5px; }
<<<<<<< HEAD

.badge-cliente { background: #8e44ad; color: white; padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; }
.badge-propio { background: #2980b9; color: white; padding: 2px 8px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; opacity: 0.8; }

/* 🚀 NUEVO ESTILO PROVEEDOR */
.badge-proveedor { background-color: #f3e5f5; color: #7b1fa2; padding: 3px 8px; border-radius: 4px; font-size: 0.75rem; font-weight: bold; border: 1px solid #e1bee7; }

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

.caja-pedidos-reservados { flex: 1; min-height: 0; overflow-y: auto; padding-right: 10px; margin-bottom: 10px; }
.caja-pedidos-reservados::-webkit-scrollbar { width: 6px; }
.caja-pedidos-reservados::-webkit-scrollbar-track { background: #f1f5f9; border-radius: 4px; }
.caja-pedidos-reservados::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 4px; }
.caja-pedidos-reservados::-webkit-scrollbar-thumb:hover { background: #94a3b8; }

.tabla-reservas { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla-reservas th { background: #ecf0f1; padding: 8px; font-size: 0.85rem; position: sticky; top: 0; z-index: 1;}
.tabla-reservas td { padding: 10px 8px; font-size: 0.9rem; border-bottom: 1px solid #ecf0f1; }

.btn-editar { border: none; background: #e3f2fd; color: #1976d2; width: 35px; height: 35px; border-radius: 50%; cursor: pointer; transition: background 0.2s; font-size: 1.1rem; display: inline-flex; align-items: center; justify-content: center;}
=======
.bajo-stock td { background-color: #fff5f5; }
.text-danger { color: #e74c3c; }
.btn-editar { border: none; background: #e3f2fd; color: #1976d2; width: 35px; height: 35px; border-radius: 50%; cursor: pointer; transition: background 0.2s; font-size: 1.1rem; }
>>>>>>> master
.btn-editar:hover { background: #bbdefb; }
.loading { text-align: center; padding: 40px; color: #3498db; font-size: 1.2rem; }
.vacio { text-align: center; padding: 30px; color: #999; font-style: italic; }
.error { color: red; text-align: center; }

<<<<<<< HEAD
.mensaje-guia { background: #e8f6f3; padding: 20px; border-radius: 8px; border: 2px dashed #1abc9c; color: #16a085; text-align: center; }
.mensaje-guia h3 { margin: 0 0 10px 0; }

.btn-global { background: #8e44ad; color: white; border: none; padding: 2px 8px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; cursor: pointer; margin-top: 5px; display: inline-block; box-shadow: 0 2px 4px rgba(0,0,0,0.1); transition: transform 0.2s; }
.btn-global:hover { background: #9b59b6; transform: scale(1.05); }

.tarjetas-resumen { display: flex; gap: 15px; margin-top: 15px; flex-shrink: 0; }
.tarjeta-stock { flex: 1; padding: 15px; border-radius: 8px; text-align: center; color: white; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.tarjeta-stock span { font-size: 0.75rem; font-weight: bold; opacity: 0.9; text-transform: uppercase; letter-spacing: 0.5px; }
.tarjeta-stock h2 { margin: 10px 0 0 0; font-size: 1.6rem; }

.fisico { background: linear-gradient(135deg, #2c3e50, #34495e); }
.reservado { background: linear-gradient(135deg, #d35400, #e67e22); }
.libre { background: linear-gradient(135deg, #27ae60, #2ecc71); }

.badge-fazon { background: #f39c12; color: white; padding: 3px 8px; border-radius: 4px; font-size: 0.75rem; }
.modal-lg { width: 850px !important; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #eee; padding-bottom: 10px; flex-shrink: 0; }

@media (max-width: 768px) {
    .header-stock, .fila-filtros { flex-direction: column; align-items: flex-start; }
    .acciones-header, .filtro-item select, .import-group, .btn-importar, .btn-nueva-mp, .btn-masterbatch { width: 100%; justify-content: center; }
    .select-import { width: 100%; }
    .tarjetas-resumen { flex-direction: column; }
    .sub-tabs { flex-direction: column; }
    .toggle-agrupado { margin-left: 0; margin-top: 10px; }
=======
/* 🆕 ESTILOS DEL MODAL (NUEVO) */
.modal-overlay {
    position: fixed; top: 0; left: 0; width: 100%; height: 100%;
    background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000;
>>>>>>> master
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