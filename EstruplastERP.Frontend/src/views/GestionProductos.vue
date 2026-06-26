<script setup lang="ts">
import { ref, onMounted, watch, computed } from 'vue';
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';
import { useRouter } from 'vue-router';
import { useMaestrosStore } from '@/stores/useMaestrosStore';
import { storeToRefs } from 'pinia';
import axios from 'axios';
import api from '@/services/axiosInstance';
import { useFiltrosInventario, detectarTipo } from '@/composables/useFiltrosInventario';
import { useImportacionInventario } from '@/composables/useImportacionInventario';
import { useModalesInventario } from '@/composables/useModalesInventario';
import ModalHistorialStock from '@/components/ModalHistorialStock.vue';
import ModalAjusteStock from '@/components/ModalAjusteStock.vue'; 
import ModalNuevoMasterbatch from '@/components/ModalNuevoMasterbatch.vue'; 
import { exportarInventarioExcel } from '@/composables/useExportacionInventario';
import { Alertas } from '@/utils/alertas';

const router = useRouter();
const maestrosStore = useMaestrosStore();

// Listas locales
const listaProductos = ref<any[]>([]);
const listaClientes = ref<any[]>([]);
const cargando = ref(false);

const apiUrl = import.meta.env.VITE_API_URL || '/api';
const busqueda = ref('');
const error = ref('');
const fileInputFlexxus = ref<HTMLInputElement | null>(null);
const fileInputExcelCliente = ref<HTMLInputElement | null>(null); // 🚀 Ref para Excel Cliente

// Pestañas
const tabActual = ref('MP');
const subTabMP = ref('VIRGEN'); 
const subTabCliente = ref('MP_CLI');
const clienteFiltro = ref<number | string>('');
const materialFiltro = ref<string>('');
const importClienteFiltro = ref<number | string>('');

// Modales y Estados Globales
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

const mostrarResumen = ref(false);
const resumenData = ref({ creados: 0, actualizados: 0, errores: 0 });

const listaProveedores = ref<any[]>([]);
const descargandoExcelFazon = ref(false);
const verMolidoAgrupado = ref(false);
const gruposExpandidos = ref<string[]>([]);

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

const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const esMpCliente = (p: any) => getSku(p).startsWith('MP-CLI');
const esServicioFazon = (p: any) => getSku(p).startsWith('FAZ-');
const esScrap = (p: any) => !!(p.esScrap || p.EsScrap) || getSku(p).startsWith('SCRAP-CLI');

const vistaFazonFiltrada = computed(() => {
    let lista = listaProductos.value.filter(p => 
        getClienteId(p) > 0 || esMpCliente(p) || esServicioFazon(p) || esScrap(p)
    );
    
    if (clienteFiltro.value) {
        lista = lista.filter(p => getClienteId(p) === Number(clienteFiltro.value));
    }
    
    if (materialFiltro.value && materialFiltro.value !== 'Todos los Materiales') {
        lista = lista.filter(p => {
            const tipo = detectarTipo(p) || '';
            return tipo.toUpperCase() === materialFiltro.value.toUpperCase();
        });
    }
    
    if (busqueda.value) {
        const b = busqueda.value.toUpperCase();
        lista = lista.filter(p => getNombre(p).includes(b) || getSku(p).includes(b));
    }
    
    return lista;
});

const { 
    mostrarModalNuevaMP, nuevaMP, guardandoMP, 
    mostrarModalReservas, productoSeleccionado, ordenesReserva, cargandoReservas,
    verDetalleReserva, guardarNuevaMateriaPrima
} = useModalesInventario(cargarDatos);

const {
    fileInput, importando, clickImportar, subirArchivoFlexxus
} = useImportacionInventario(tabActual, clienteFiltro, importClienteFiltro, cargarDatos);

const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

const clickImportarFlexxus = () => { fileInputFlexxus.value?.click(); };

// 🚀 DISPARADOR DE LA IMPORTACIÓN DE CLIENTES EN EXCEL
const clickImportarClienteExcel = () => {
    if (!clienteFiltro.value) {
        Alertas.advertencia("Por favor seleccione un cliente en el filtro antes de importar su inventario.");
        return;
    }
    fileInputExcelCliente.value?.click();
};

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
        cargando.value = true;
        error.value = '';
        const [resProd, resCli] = await Promise.all([
            axios.get(`${apiUrl}/Productos`, getAuthConfig()),
            axios.get(`${apiUrl}/Clientes`, getAuthConfig())
        ]);
        listaProductos.value = resProd.data.sort((a: any, b: any) => getNombre(a).localeCompare(getNombre(b)));
        listaClientes.value = resCli.data;
    } catch (e: any) {
        error.value = "Error conectando al servidor. Revisa la consola.";
    } finally {
        cargando.value = false;
    }
}

const recargarDatos = async () => { await cargarDatos(true); };

const procesarArchivoFlexxus = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (!target.files || target.files.length === 0) return;
    const file = target.files[0];
    if (!file) return; 

    if (!file.name.endsWith('.csv') && !file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
        Alertas.advertencia('Por favor selecciona un archivo CSV o Excel válido.');
        return;
    }

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

        resumenData.value = {
            creados: response.data.creados || 0,
            actualizados: response.data.actualizados || 0,
            errores: response.data.errores || 0
        };
        mostrarResumen.value = true;
        await recargarDatos(); 
    } catch (e: any) {
        console.error(e);
        Alertas.error(`Error al importar: ${e.response?.data?.message || 'Revisa el archivo.'}`);
    } finally {
        cargando.value = false;
        target.value = ''; 
    }
};

// 🚀 PROCESADOR EXCEL CLIENTE MEDIANTE TU ENDPOINT MULTICLIENTE EXISTENTE
const procesarExcelCliente = async (event: Event) => {
    const target = event.target as HTMLInputElement;
    if (!target.files || target.files.length === 0) return;
    const file = target.files[0];
    if (!file) return;

    if (!file.name.endsWith('.xlsx') && !file.name.endsWith('.xls')) {
        Alertas.advertencia('Por favor selecciona una planilla Excel (.xlsx o .xls) válida.');
        target.value = '';
        return;
    }

    try {
        cargando.value = true;
        const formData = new FormData();
        formData.append('archivo', file);
        if (clienteFiltro.value) {
            formData.append('clienteIdFiltro', String(clienteFiltro.value));
        }

        const response = await axios.post(`${apiUrl}/Flexxus/importar-excel-multicliente`, formData, {
            headers: { ...getAuthConfig().headers, 'Content-Type': 'multipart/form-data' }
        });

        Alertas.exito(response.data?.mensaje || 'Importación completada correctamente.');
        await recargarDatos();
    } catch (e: any) {
        Alertas.error(`Error al importar inventario: ${e.response?.data || e.message || 'Estructura incorrecta.'}`);
    } finally {
        cargando.value = false;
        target.value = '';
    }
};

const cerrarModal = () => { mostrarResumen.value = false; };

const abrirPantallazoGlobal = (producto: any) => {
    const familiaOriginal = detectarTipo(producto).toUpperCase();
    detalleGlobal.value = listaProductos.value.filter(item => {
        const textoCompleto = ((item.nombre || '') + ' ' + (item.codigoSku || '')).toUpperCase();
        return textoCompleto.includes('TUTI') && detectarTipo(item).toUpperCase() === familiaOriginal;
    });
    resumenGlobal.value.fisico = detalleGlobal.value.reduce((acc, item) => acc + (item.stockFisico ?? item.stockActual ?? 0), 0);
    resumenGlobal.value.reservado = detalleGlobal.value.reduce((acc, item) => acc + (item.stockReservado ?? 0), 0);
    resumenGlobal.value.libre = resumenGlobal.value.fisico - resumenGlobal.value.reservado;
    mostrarModalGlobal.value = true;
};

const descargarAuditoria = async () => {
    if (!listaProductos.value || listaProductos.value.length === 0) {
        Alertas.advertencia("No hay datos de inventario para exportar.");
        return;
    }
    try {
        exportando.value = true;
        await exportarInventarioExcel(listaProductos.value, listaClientes.value);
    } catch (e) {
        Alertas.error("Hubo un problema al crear el archivo Excel.");
    } finally {
        exportando.value = false;
    }
};

const toggleGrupo = (id: string) => {
    const index = gruposExpandidos.value.indexOf(id);
    if (index === -1) gruposExpandidos.value.push(id);
    else gruposExpandidos.value.splice(index, 1);
};

const molidosAgrupados = computed(() => {
    if (tabActual.value !== 'CLI') return [];
    const grupos: Record<string, any> = {};
    const baseList = vistaFazonFiltrada.value.filter(p => (p.codigoSku || '').toUpperCase().includes('MOL') || checkEsMolido(p));

    baseList.forEach(p => {
        const sku = (p.codigoSku || '').toUpperCase();
        const nombre = (p.nombre || '').toUpperCase();
        const partesSku = sku.split('-');
        const materialBruto = partesSku.length > 1 ? partesSku[1].trim() : 'VARIOS';
        const material = materialBruto.replace(/[0-9]/g, '').trim() || 'VARIOS';

        let colorRaw = nombre.replace(/MOLIDO/i, '').replace(new RegExp(materialBruto, 'i'), '').replace(new RegExp(material, 'i'), '');
        const color = colorRaw.replace(/\(.*?\)/g, '').replace(/[^a-zA-Z0-9ñÑáéíóúÁÉÍÓÚ ]/g, '').trim().toUpperCase() || 'SIN COLOR';

        const key = `${material}_${color}`;
        if (!grupos[key]) {
            grupos[key] = { idGrupo: key, material, color, fisico: 0, reservado: 0, disponible: 0, items: [] };
        }

        const fisico = p.stockFisico ?? p.stockActual ?? 0;
        const reservado = p.stockReservado ?? 0;
        grupos[key].fisico += fisico;
        grupos[key].reservado += reservado;
        grupos[key].disponible += (p.stockDisponible ?? (fisico - reservado));
        grupos[key].items.push(p);
    });

    return Object.values(grupos).sort((a: any, b: any) => {
        if (a.material !== b.material) return a.material.localeCompare(b.material);
        return a.color.localeCompare(b.color);
    });
});

const cargarProveedores = async () => {
    try {
        const response = await api.get('/Proveedores'); 
        listaProveedores.value = response.data;
    } catch (e) {
        console.error("Error al cargar proveedores", e);
    }
};

const generarExcelFazon = async (clienteId: number) => {
    if (descargandoExcelFazon.value) return;
    descargandoExcelFazon.value = true;
    try {
        const clienteSeleccionado = listaClientes.value.find(c => Number(c.id) === clienteId);
        const nombreClienteReal = clienteSeleccionado ? clienteSeleccionado.razonSocial : 'Cliente_Desconocido';
        const fechaHoy = new Date().toISOString().split('T')[0];
        
        const inventarioCliente = listaProductos.value.filter(p => Number(getClienteId(p)) === Number(clienteId));

        const workbook = new ExcelJS.Workbook();
        const wsInv = workbook.addWorksheet('Inventario Actual');
        wsInv.mergeCells('A1:C1');
        wsInv.getCell('A1').value = `REPORTE DE INVENTARIO - ${nombreClienteReal.toUpperCase()}`;
        wsInv.getCell('A1').font = { size: 14, bold: true, color: { argb: 'FF2980B9' } };
        
        const columnsInv = [
            { header: 'SKU', key: 'sku', width: 20 },
            { header: 'Nombre del Producto', key: 'nombre', width: 45 },
            { header: 'Stock (kg)', key: 'stock', width: 25 }
        ];
        wsInv.columns = columnsInv;
        wsInv.getRow(2).eachCell(cell => { cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF2980B9' } }; cell.font = { color: { argb: 'FFFFFFFF' }, bold: true }; });

        inventarioCliente.forEach((p) => {
            const disponible = p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0));
            wsInv.addRow({ sku: p.codigoSku || '-', nombre: p.nombre, stock: disponible }).getCell('stock').numFmt = '#,##0.00';
        });

        const buffer = await workbook.xlsx.writeBuffer();
        saveAs(new Blob([buffer]), `Reporte_Inventario_${nombreClienteReal}_${fechaHoy}.xlsx`);
    } catch (e) {
        Alertas.error("Error al generar el Excel.");
    } finally {
        descargandoExcelFazon.value = false;
    }
};

const irAEditar = (id: number) => { router.push({ name: 'editar-producto', params: { id } }); };

onMounted(() => {
    cargarDatos(true);
    cargarProveedores();
});
</script>

<template>
    <div class="contenedor-stock">
        <div class="header-stock">
            <div class="titulos-stock">
                <h2>📦 Gestión de Stock</h2>
            </div>
            
            <div class="acciones-header">
                <button @click="descargarAuditoria" :disabled="exportando || cargando" class="btn-header btn-excel">
                    <span v-if="exportando">⏳ Generando...</span>
                    <span v-else>📊 Planilla Conteo</span>
                </button>

                <button class="btn-header btn-nueva-mp" @click="mostrarModalNuevaMP = true">➕ Crear Insumo</button>
                <button class="btn-header btn-masterbatch" @click="mostrarModalMasterbatch = true">🎨 Alta Color</button>
                <button class="btn-header btn-flexxus" @click="clickImportarFlexxus" :disabled="cargando">📄 Importar CSV</button>

                <button v-if="tabActual === 'CLI'" class="btn-header btn-importar-cliente" @click="clickImportarClienteExcel" :disabled="cargando || !clienteFiltro">
                    📥 Importar Excel Cliente
                </button>

                <input type="file" ref="fileInputFlexxus" style="display: none" accept=".csv, .xlsx, .xls" @change="procesarArchivoFlexxus"/>
                <input type="file" ref="fileInputExcelCliente" style="display: none" accept=".xlsx, .xls" @change="procesarExcelCliente"/>

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

        <div v-if="tabActual === 'MP'" class="toolbar-clientes">
            <div class="sub-tabs">
                <button :class="{ 'sub-active': subTabMP === 'VIRGEN' }" @click="subTabMP = 'VIRGEN'">🧪 Material Virgen</button>
                <button :class="{ 'sub-active': subTabMP === 'ADITIVOS' }" @click="subTabMP = 'ADITIVOS'">⚙️ Aditivos</button>
                <button :class="{ 'sub-active': subTabMP === 'MASTERBATCH' }" @click="subTabMP = 'MASTERBATCH'">🎨 Masterbatch / Colores</button>
                <button :class="{ 'sub-active': subTabMP === 'MOLIDO_PROPIO' }" @click="subTabMP = 'MOLIDO_PROPIO'">♻️ Molienda Propia</button>
            </div>
        </div>

        <div v-if="tabActual === 'CLI'" class="toolbar-fazon">
            <div class="filtros-izquierda">
                <div class="filtro-columna">
                    <label>🏢 Cliente:</label>
                    <select v-model="clienteFiltro" class="select-chico">
                        <option value="">Todos los Clientes</option>
                        <option v-for="c in listaClientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                    </select>
                </div>

                <div class="filtro-columna">
                    <label>🧱 Material:</label>
                    <select v-model="materialFiltro" class="select-chico">
                        <option value="">Todos los Materiales</option>
                        <option v-for="mat in TIPOS_MATERIALES" :key="mat" :value="mat">{{ mat }}</option>
                    </select>
                </div>
            </div>

            <div class="acciones-derecha-fazon">
                <button v-if="clienteFiltro" @click="generarExcelFazon(Number(clienteFiltro))" :disabled="descargandoExcelFazon" class="btn-exportar-fazon">
                    <span v-if="descargandoExcelFazon">⏳ Generando...</span>
                    <span v-else>📊 Exportar Reporte</span>
                </button>

                <label class="toggle-agrupado">
                    <input type="checkbox" v-model="verMolidoAgrupado"> 📂 Agrupar por Color
                </label>
            </div>
        </div>

        <div v-if="cargando" class="loading">⏳ Cargando Inventario, por favor espere...</div>
        <div v-else-if="error" class="error">❌ {{ error }}</div>

        <div v-else class="tabla-wrapper">
            
            <table v-if="tabActual === 'CLI' && verMolidoAgrupado">
                <thead>
                    <tr>
                        <th style="width: 40px;"></th>
                        <th>Material / Color Agrupado</th>
                        <th style="text-align:right; width: 140px;">Físico Total (Kg)</th>
                        <th style="text-align:center; width: 140px;">Reservado (Kg)</th>
                        <th style="text-align:right; width: 140px;">Disponible (Kg)</th>
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
                            <small style="color:#95a5a6; margin-left: 5px;">({{ grupo.items.length }} lotes)</small>
                        </td>
                        <td style="text-align:right; font-weight:bold;">{{ grupo.fisico.toFixed(2) }}</td>
                        <td style="text-align:center; font-weight:bold; color:#e67e22;">{{ grupo.reservado.toFixed(2) }}</td>
                        <td style="text-align:right; font-weight:bold;" :class="{'text-danger': grupo.disponible <= 0, 'text-success': grupo.disponible > 0}">
                            {{ grupo.disponible.toFixed(2) }}
                        </td>
                    </tr>
                    
                    <template v-if="gruposExpandidos.includes(grupo.idGrupo)">
                        <tr v-for="p in grupo.items" :key="p.id" class="fila-detalle-grupo">
                            <td></td>
                            <td style="padding-left: 20px;">
                                <span class="sku-cell">{{ p.codigoSku }}</span>
                                <span style="margin-left:8px; color:#555;">{{ p.nombre }}</span>
                            </td>
                            <td style="text-align:right; color:#7f8c8d;">{{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}</td>
                            <td style="text-align:center; color:#7f8c8d;">
                                <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-buchon">
                                    🔒 {{ p.stockReservado }}
                                </button>
                                <span v-else>-</span>
                            </td>
                            <td style="text-align:right; color:#7f8c8d;">{{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}</td>
                        </tr>
                    </template>
                </tbody>
            </table>

            <table v-else-if="(tabActual === 'CLI' && vistaFazonFiltrada.length > 0) || (tabActual !== 'CLI' && productosFiltrados.length > 0)">
                <thead>
                    <tr>
                        <th style="width: 130px;">SKU</th>
                        <th>Descripción</th>
                        <th v-if="tabActual === 'CLI'" style="text-align: center; width: 120px;">Material</th>
                        <th v-if="tabActual === 'PT' || tabActual === 'CLI'" style="width: 160px;">Dueño / Cliente</th>
                        <th v-if="tabActual === 'MP'" style="width: 160px;">Proveedor</th> 
                        <th style="text-align:center; width: 110px;">Físico<br><small>(Kg)</small></th>
                        <th style="text-align:center; width: 110px;">Reservado</th>
                        <th style="text-align:center; width: 110px;">Disponible</th>
                        <th style="text-align:center; width: 140px;">Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="p in (tabActual === 'CLI' ? vistaFazonFiltrada : productosFiltrados)" :key="p.id" :class="{'bajo-stock': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= (p.stockMinimo || 0)}">
                        <td class="sku-cell">{{ p.codigoSku }}</td>
                        <td>
                            <div class="nombre-prod">
                                {{ p.nombre }}
                            </div>
                            <div class="tags-fila">
                                <small v-if="checkEsFazon(p) && tabActual !== 'CLI'" class="tag-fazon">FAZON</small>
                                <small v-if="checkEsMolido(p)" class="tag-molido">(MOLIDO)</small>
                                <small v-else-if="checkEsScrap(p)" class="tag-scrap">(SCRAP)</small>
                                <button v-if="p.nombre.toUpperCase().includes('TUTI') && (!getClienteId(p) || getClienteId(p) === 0)" @click="abrirPantallazoGlobal(p)" class="btn-global">
                                    🌍 Ver Total Global
                                </button>
                            </div>
                        </td>
                        
                        <td v-if="tabActual === 'CLI'" style="text-align: center;">
                            <span class="badge-material" :class="detectarTipo(p).toLowerCase().replace(' ', '-')">
                                {{ detectarTipo(p) }}
                            </span>
                        </td>

                        <td v-if="tabActual === 'PT' || tabActual === 'CLI'" class="text-muted">
                            <span v-if="getClienteId(p) > 0" class="badge-cliente-purple">
                                {{ listaClientes.find(c => Number(c.id) === Number(getClienteId(p)))?.razonSocial || 'Cliente' }}
                            </span>
                            <span v-else class="badge-propio">Estruplast</span>
                        </td>

                        <td v-if="tabActual === 'MP'" class="text-muted">
                            <span v-if="p.proveedorNombre" class="badge-proveedor">{{ p.proveedorNombre }}</span>
                            <span v-else>-</span>
                        </td>

                        <td class="text-center font-numero">{{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}</td>
                        
                        <td class="text-center">
                            <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-reservado" title="Ver Órdenes">
                                <span style="font-size: 0.8rem;">🔒</span><br>
                                <span>{{ p.stockReservado.toFixed(3) }}</span>
                            </button>
                            <span v-else class="text-muted">-</span>
                        </td>
                        
                        <td class="text-center font-numero font-bold" :class="{'text-danger': (p.stockFisico - p.stockReservado) <= 0}">
                            {{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}
                        </td>
                        
                        <td class="text-center celda-acciones">
                            <button @click="irAEditar(p.id)" class="btn-accion-tabla btn-edit" title="Editar Insumo">✏️</button>
                            <button @click="abrirModalAjuste(p)" class="btn-accion-tabla btn-ajuste" title="Ajustar Stock Físico">⚖️</button>
                            <button @click="abrirKardex(p)" class="btn-accion-tabla btn-kardex" title="Ver Historial">🕒</button>
                        </td>
                    </tr>
                </tbody>
            </table>

            <div v-else class="vacio">
                ⚠️ No hay ningún material o lote registrado que coincida con los filtros aplicados.
            </div>
        </div>
    </div>

    <div v-if="mostrarModalReservas" class="modal-overlay" @click.self="mostrarModalReservas = false">
        <div class="modal-content modal-reserva">
            <div class="modal-header-reserva">
                <h3>🔒 Detalle de Reserva</h3>
            </div>
            
            <div class="modal-body-reserva">
                <p class="nombre-prod-modal">{{ productoSeleccionado?.nombre }}</p>
                
                <table class="tabla-reservas-detalle">
                    <thead>
                        <tr>
                            <th>N° Pedido<br><small>(Flexxus)</small></th>
                            <th>Cliente / Destino</th>
                            <th style="text-align: right;">Retenido</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="res in ordenesReserva" :key="res.id">
                            <td class="reserva-nota">{{ res.notaPedido || res.numeroPedidoCliente || 'S/N' }}</td>
                            <td class="reserva-cliente">{{ res.cliente }}</td>
                            <td class="reserva-kilos">{{ res.amount || res.cantidad }} kg</td>
                        </tr>
                        <tr v-if="!ordenesReserva || ordenesReserva.length === 0">
                            <td colspan="3" style="text-align: center; color: #7f8c8d; padding: 20px;">No hay reservas activas.</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal-footer-reserva">
                <button class="btn-cerrar-reserva" @click="mostrarModalReservas = false">Cerrar</button>
            </div>
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
                ⚠️ Se ignoraron {{ resumenData.errores }} filas por inconsistencias.
            </p>
            <button class="btn-cerrar" @click="cerrarModal">Aceptar</button>
        </div>
    </div>

    <div v-if="mostrarModalGlobal" class="modal-overlay" @click.self="mostrarModalGlobal = false">
        <div class="modal-content modal-lg">
            <div class="modal-header">
                <h3 style="margin-top:0; border-bottom:none;">🌍 Pantallazo Global de Material: "TUTI"</h3>
                <button class="btn-cancelar" @click="mostrarModalGlobal = false" style="padding: 2px 8px; border:none; background:none; cursor:pointer; font-size:1.2rem;">✕</button>
            </div>
            
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
        </div>
    </div>

    <ModalHistorialStock :visible="mostrarModalHistorial" :productoId="productoIdSeleccionado" :productoNombre="productoNombreSeleccionado" @close="mostrarModalHistorial = false" />
    <ModalAjusteStock :visible="mostrarModalAjuste" :producto="productoParaAjustar" @close="mostrarModalAjuste = false" @confirmado="onAjusteConfirmado" />
    <ModalNuevoMasterbatch :visible="mostrarModalMasterbatch" @close="mostrarModalMasterbatch = false" @creado="cargarDatos(true)" />
</template>

<style scoped>
/* ESTILOS GLOBALES ESTRUCTURA */
.contenedor-stock { max-width: 1280px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; font-family: 'Segoe UI', system-ui, sans-serif; }
.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; flex-wrap: wrap; gap: 15px; }
.titulos-stock h2 { margin: 0; color: #2c3e50; font-size: 1.6rem; font-weight: bold;}

.acciones-header { display: flex; gap: 10px; align-items: center; flex-wrap: wrap; }
.btn-header { padding: 8px 14px; border: none; border-radius: 4px; font-weight: bold; font-size: 0.85rem; cursor: pointer; transition: background 0.2s; }
.btn-excel { background-color: #27ae60; color: white; }
.btn-nueva-mp { background: #3498db; color: white; }
.btn-masterbatch { background: #8b5cf6; color: white; }
.btn-flexxus { background-color: #34495e; color: white; }

/* 🚀 ESTILO DEL NUEVO BOTÓN NARANJA EXCEL CLIENTE */
.btn-importar-cliente { background-color: #e67e22; color: white; }
.btn-importar-cliente:hover:not(:disabled) { background-color: #d35400; }
.btn-importar-cliente:disabled { background-color: #cbd5e1; color: #94a3b8; cursor: not-allowed; }

.buscador input { padding: 8px 12px; width: 250px; border: 1px solid #ddd; border-radius: 4px; font-size: 0.9rem; outline: none; }

/* TABS NAVEGACIÓN */
.tabs-container { display: flex; gap: 8px; margin-bottom: 20px; border-bottom: 2px solid #eaeaea; padding-bottom: 0; }
.tab-btn { background: none; border: none; padding: 12px 20px; font-size: 0.95rem; color: #7f8c8d; cursor: pointer; font-weight: bold; display: flex; align-items: center; gap: 8px; border-bottom: 3px solid transparent; }
.tab-btn:hover { color: #34495e; background-color: #f8f9fa; border-radius: 6px 6px 0 0; }
.tab-btn.active { color: #2980b9; border-bottom-color: #2980b9; }
.counter { background: #e0e0e0; padding: 2px 8px; border-radius: 12px; font-size: 0.75rem; color: #555; }
.tab-btn.active .counter { background: #d6eaf8; color: #2980b9; }

.toolbar-clientes { background-color: #f4f6f7; padding: 12px 15px; border-radius: 6px; margin-bottom: 20px; border: 1px solid #e5e8e8; }
.sub-tabs { display: flex; gap: 4px; background: transparent; }
.sub-tabs button { border: none; background: transparent; padding: 6px 14px; font-size: 0.85rem; color: #7f8c8d; cursor: pointer; border-radius: 4px; font-weight: bold; }
.sub-tabs button.sub-active { background: white; color: #2c3e50; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }

.toolbar-fazon { background-color: #ffffff; padding: 15px 20px; border-radius: 8px; margin-bottom: 20px; border: 1px solid #eaeaea; display: flex; justify-content: space-between; align-items: flex-end; flex-wrap: wrap; gap: 15px; }
.filtros-izquierda { display: flex; gap: 15px; flex-wrap: wrap; }
.filtro-columna { display: flex; flex-direction: column; gap: 6px; }
.filtro-columna label { font-size: 0.85rem; color: #555; font-weight: bold; }
.select-chico { padding: 6px 10px; border: 1px solid #ccc; border-radius: 4px; font-size: 0.9rem; width: 200px; color: #333; background: #fff; outline: none; }

.acciones-derecha-fazon { display: flex; align-items: center; gap: 15px; }
.btn-exportar-fazon { background-color: #2980b9; color: white; border: none; padding: 0 15px; border-radius: 4px; font-weight: bold; cursor: pointer; transition: background 0.2s; height: 34px; display: flex; align-items: center; }
.toggle-agrupado { display: flex; align-items: center; background: #fff3e0; padding: 0 15px; border-radius: 6px; border: 1px solid #f39c12; color: #d35400; font-weight: bold; cursor: pointer; height: 34px; }
.toggle-agrupado input { margin-right: 8px; }

/* TABLA PRINCIPAL */
.tabla-wrapper { overflow-x: auto; border-top: 1px solid #eee; }
table { width: 100%; border-collapse: collapse; font-size: 0.9rem; text-align: left; }
th { background: #ffffff; padding: 12px 10px; font-weight: bold; color: #555; border-bottom: 2px solid #eaeaea; font-size: 0.85rem; }
td { padding: 12px 10px; border-bottom: 1px solid #f9f9f9; vertical-align: middle; color: #444; }
.sku-cell { font-family: 'Courier New', monospace; font-weight: bold; color: #666; font-size: 0.85rem; }
.nombre-prod { font-weight: 700; color: #1e293b; font-size: 0.9rem; }

.badge-proveedor { background-color: #f5eef8; color: #8e44ad; padding: 3px 8px; border-radius: 4px; font-size: 0.75rem; font-weight: bold; border: 1px solid #ebdef0; }
.btn-global { background: #8e44ad; color: white; border: none; padding: 2px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: bold; cursor: pointer; margin-top: 4px; }

tr.bajo-stock td { background-color: #fdedec !important; }
.btn-reservado { background: #fef5e7; border: 1px solid #f8c471; color: #d35400; padding: 4px 12px; border-radius: 12px; font-size: 0.8rem; font-weight: bold; cursor: pointer; line-height: 1.2; }

.celda-acciones { display: flex; gap: 8px; justify-content: center; }
.btn-accion-tabla { border: none; width: 28px; height: 28px; border-radius: 50%; cursor: pointer; display: flex; align-items: center; justify-content: center; }
.btn-edit { background: #ebf5fb; color: #2980b9; }
.btn-ajuste { background: #fef5e7; color: #e67e22; }
.btn-kardex { background: #f4f6f7; color: #7f8c8d; }

.tags-fila { display: flex; gap: 5px; align-items: center; margin-top: 5px; }
.tag-fazon { background: #7b1fa2; color: white; padding: 1px 5px; border-radius: 4px; font-size: 0.7rem; font-weight: bold; }
.tag-molido { color: #27ae60; font-weight: bold; font-size: 0.75rem; }
.tag-scrap { color: #d35400; font-weight: bold; font-size: 0.75rem; }

.badge-cliente-purple { background-color: #8e44ad; color: white; padding: 2px 6px; border-radius: 4px; font-size: 0.75rem; font-weight: bold; text-transform: uppercase; }
.badge-propio { background: #e3f2fd; color: #0d47a1; padding: 4px 10px; border-radius: 4px; font-size: 0.8rem; font-weight: bold; border: 1px solid #bbdefb; }
.badge-material { background: #64748b; color: white; padding: 4px 10px; border-radius: 12px; font-size: 0.8rem; font-weight: bold; text-transform: uppercase; }

.fila-grupo { background-color: #f8fafc; cursor: pointer; font-weight: bold; }
.fila-detalle-grupo td { background-color: #ffffff; font-size: 0.85rem; border-bottom: 1px dashed #e2e8f0; }

.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.5); display: flex; align-items: center; justify-content: center; z-index: 2000; }
.modal-reserva { background: white; border-radius: 8px; width: 550px; max-height: 90vh; display: flex; flex-direction: column; padding: 25px 35px; }
.modal-body-reserva { overflow-y: auto; flex-grow: 1; }
.tabla-reservas-detalle { width: 100%; border-collapse: collapse; }
.tabla-reservas-detalle th { background: #f9f9f9; padding: 10px; font-size: 0.8rem; border-bottom: 2px solid #eaeaea; }
.reserva-nota { color: #2980b9; font-weight: bold; }
.reserva-kilos { color: #d35400; font-weight: bold; text-align: right; }

.modal-content { background: white; padding: 25px; border-radius: 8px; width: 400px; text-align: center; }
.modal-stats { display: flex; justify-content: space-around; margin: 25px 0; }
.stat-item { display: flex; flex-direction: column; align-items: center; }
.stat-item .num { font-size: 1.8rem; font-weight: bold; }
.stat-item.creado .num { color: #27ae60; }
.btn-cerrar { background: #34495e; color: white; border: none; padding: 10px 25px; border-radius: 25px; cursor: pointer; }
.vacio, .loading { text-align: center; padding: 40px; color: #7f8c8d; font-weight: bold; }

.modal-lg { max-width: 800px !important; width: 800px; text-align: left; }
.tarjetas-resumen { display: flex; gap: 15px; margin: 15px 0; }
.tarjeta-stock { flex: 1; padding: 16px; border-radius: 8px; text-align: center; color: white; }
.fisico { background: linear-gradient(135deg, #334155, #1e293b); }
.reservado { background: linear-gradient(135deg, #ea580c, #c2410c); }
.libre { background: linear-gradient(135deg, #16a34a, #15803d); }
</style>