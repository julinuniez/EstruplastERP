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
const fileInputExcelCliente = ref<HTMLInputElement | null>(null);

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

                <button v-if="tabActual === 'CLI'" class="btn-header btn-importar-cliente" @click="clickImportarClienteExcel" :disabled="cargando || !clienteFiltro" :title="!clienteFiltro ? 'Seleccione un cliente primero' : 'Importar Excel para este cliente'">
                    📥 Importar Excel Cliente
                </button>

                <input type="file" ref="fileInputFlexxus" style="display: none" accept=".csv, .xlsx, .xls" @change="procesarArchivoFlexxus"/>
                <input type="file" ref="fileInputExcelCliente" style="display: none" accept=".xlsx, .xls, .csv, application/vnd.ms-excel, application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" @change="procesarExcelCliente"/>

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
                        <th style="width: 45px;"></th>
                        <th>Material / Color Agrupado</th>
                        <th style="text-align:right; width: 140px;">Físico Total (Kg)</th>
                        <th style="text-align:center; width: 140px;">Reservado (Kg)</th>
                        <th style="text-align:right; width: 140px;">Disponible (Kg)</th>
                    </tr>
                </thead>
                <tbody v-for="grupo in molidosAgrupados" :key="grupo.idGrupo">
                    <tr class="fila-grupo" @click="toggleGrupo(grupo.idGrupo)">
                        <td style="text-align:center; color:#64748b; font-size:0.85rem;">
                            {{ gruposExpandidos.includes(grupo.idGrupo) ? '▼' : '▶' }}
                        </td>
                        <td>
                            <span class="badge-material" :class="grupo.material.toLowerCase().replace(' ', '-')">{{ grupo.material }}</span>
                            <strong style="margin-left: 12px; color:#1e293b; font-size: 0.95rem;">{{ grupo.color }}</strong>
                            <small style="color:#94a3b8; margin-left: 6px; font-weight: 500;">({{ grupo.items.length }} lotes)</small>
                        </td>
                        <td style="text-align:right; font-weight:700; color: #334155;">{{ grupo.fisico.toFixed(2) }}</td>
                        <td style="text-align:center; font-weight:700; color:#ea580c;">{{ grupo.reservado.toFixed(2) }}</td>
                        <td style="text-align:right; font-weight:700;" :class="{'text-danger': grupo.disponible <= 0, 'text-success': grupo.disponible > 0}">
                            {{ grupo.disponible.toFixed(2) }}
                        </td>
                    </tr>
                    
                    <template v-if="gruposExpandidos.includes(grupo.idGrupo)">
                        <tr v-for="p in grupo.items" :key="p.id" class="fila-detalle-grupo">
                            <td></td>
                            <td style="padding-left: 25px;">
                                <span class="sku-cell">{{ p.codigoSku }}</span>
                                <span style="margin-left:12px; color:#475569; font-weight: 500;">{{ p.nombre }}</span>
                            </td>
                            <td style="text-align:right; color:#64748b;">{{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}</td>
                            <td style="text-align:center; color:#64748b;">
                                <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-buchon">
                                    🔒 {{ p.stockReservado }}
                                </button>
                                <span v-else>-</span>
                            </td>
                            <td style="text-align:right; color:#64748b; font-weight: 600;">{{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}</td>
                        </tr>
                    </template>
                </tbody>
            </table>

            <table v-else-if="(tabActual === 'CLI' && vistaFazonFiltrada.length > 0) || (tabActual !== 'CLI' && productosFiltrados.length > 0)">
                <thead>
                    <tr>
                        <th style="width: 140px;">SKU</th>
                        <th>Descripción</th>
                        <th v-if="tabActual === 'CLI'" style="text-align: center; width: 120px;">Material</th>
                        <th v-if="tabActual === 'PT' || tabActual === 'CLI'" style="width: 180px;">Dueño / Cliente</th>
                        <th v-if="tabActual === 'MP'" style="width: 180px;">Proveedor</th> 
                        <th style="text-align:center; width: 120px;">Físico<br><small>(Kg)</small></th>
                        <th style="text-align:center; width: 120px;">Reservado</th>
                        <th style="text-align:center; width: 120px;">Disponible</th>
                        <th style="text-align:center; width: 150px;">Acción</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="p in (tabActual === 'CLI' ? vistaFazonFiltrada : productosFiltrados)" :key="p.id" :class="{'bajo-stock': ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0)) <= (p.stockMinimo || 0)}">
                        <td class="sku-cell"><span>{{ p.codigoSku }}</span></td>
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

                        <td v-if="tabActual === 'PT' || tabActual === 'CLI'">
                            <span v-if="getClienteId(p) > 0" class="badge-cliente-purple">
                                {{ listaClientes.find(c => Number(c.id) === Number(getClienteId(p)))?.razonSocial || 'Cliente' }}
                            </span>
                            <span v-else class="badge-propio">Estruplast</span>
                        </td>

                        <td v-if="tabActual === 'MP'">
                            <span v-if="p.proveedorNombre" class="badge-proveedor">{{ p.proveedorNombre }}</span>
                            <span v-else class="text-null">-</span>
                        </td>

                        <td class="text-center font-numero">{{ (p.stockFisico ?? p.stockActual ?? 0).toFixed(2) }}</td>
                        
                        <td class="text-center">
                            <button v-if="(p.stockReservado || 0) > 0" @click="verDetalleReserva(p)" class="btn-reservado" title="Ver Órdenes">
                                🔒 {{ p.stockReservado.toFixed(2) }}
                            </button>
                            <span v-else class="text-null">-</span>
                        </td>
                        
                        <td class="text-center font-numero font-bold" :class="{'text-danger': (p.stockFisico - p.stockReservado) <= 0, 'text-success': (p.stockFisico - p.stockReservado) > 0}">
                            {{ (p.stockDisponible ?? ((p.stockFisico ?? p.stockActual ?? 0) - (p.stockReservado ?? 0))).toFixed(2) }}
                        </td>
                        
                        <td class="text-center">
                            <div class="celda-acciones">
                                <button @click="irAEditar(p.id)" class="btn-accion-tabla btn-edit" title="Editar Insumo">✏️</button>
                                <button @click="abrirModalAjuste(p)" class="btn-accion-tabla btn-ajuste" title="Ajustar Stock Físico">⚖️</button>
                                <button @click="abrirKardex(p)" class="btn-accion-tabla btn-kardex" title="Ver Historial">🕒</button>
                            </div>
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
                            <td colspan="3" style="text-align: center; color: #94a3b8; padding: 25px;">No hay reservas activas.</td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal-footer-reserva">
                <button class="btn-cerrar-reserva" @click="mostrarModalReservas = false">Cerrar Ventana</button>
            </div>
        </div>
    </div>

    <div v-if="mostrarResumen" class="modal-overlay">
        <div class="modal-content modal-resumen-box">
            <h3>📊 Resumen de Importación</h3>
            <div class="modal-stats">
                <div class="stat-item creado">
                    <span class="emoji">✨</span>
                    <span class="num">{{ resumenData.creados }}</span>
                    <span class="label">Nuevos Dados de Alta</span>
                </div>
                <div class="stat-item actualizado">
                    <span class="emoji">🔄</span>
                    <span class="num">{{ resumenData.actualizados }}</span>
                    <span class="label">Inventario Actualizado</span>
                </div>
            </div>
            <p v-if="resumenData.errores > 0" class="text-error">
                ⚠️ Se ignoraron {{ resumenData.errores }} filas por inconsistencias de formato.
            </p>
            <button class="btn-cerrar" @click="cerrarModal">Aceptar e Internalizar</button>
        </div>
    </div>

    <div v-if="mostrarModalGlobal" class="modal-overlay" @click.self="mostrarModalGlobal = false">
        <div class="modal-content modal-lg">
            <div class="modal-header">
                <h3 style="margin-top:0; border-bottom:none; color: #1e293b;">🌍 Balance Integrado de Material: "TUTI"</h3>
                <button class="btn-cancelar" @click="mostrarModalGlobal = false" style="padding: 2px 8px; border:none; background:none; cursor:pointer; font-size:1.3rem; color: #64748b;">✕</button>
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
/* GENERAL ESTRUCTURA MODERNIZADA */
.contenedor-stock { max-width: 1340px; margin: 0 auto; background: #ffffff; padding: 30px; border-radius: 12px; font-family: 'Segoe UI', system-ui, sans-serif; box-shadow: 0 4px 20px -2px rgba(50, 65, 78, 0.08); }
.header-stock { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; flex-wrap: wrap; gap: 20px; }
.titulos-stock h2 { margin: 0; color: #1e293b; font-size: 1.75rem; font-weight: 800; letter-spacing: -0.5px; }

.acciones-header { display: flex; gap: 12px; align-items: center; flex-wrap: wrap; }
.btn-header { padding: 10px 18px; border: 1px solid transparent; border-radius: 8px; font-weight: 700; font-size: 0.88rem; cursor: pointer; transition: all 0.2s ease; display: flex; align-items: center; gap: 6px; box-shadow: 0 2px 4px rgba(0,0,0,0.04); }

.btn-excel { background-color: #10b981; color: white; }
.btn-excel:hover { background-color: #059669; transform: translateY(-1px); }
.btn-nueva-mp { background: #3b82f6; color: white; }
.btn-nueva-mp:hover { background: #2563eb; transform: translateY(-1px); }
.btn-masterbatch { background: #6d28d9; color: white; }
.btn-masterbatch:hover { background: #5b21b6; transform: translateY(-1px); }
.btn-flexxus { background-color: #475569; color: white; }
.btn-flexxus:hover { background-color: #334155; transform: translateY(-1px); }

.btn-importar-cliente { background-color: #f97316; color: white; }
.btn-importar-cliente:hover:not(:disabled) { background-color: #ea580c; transform: translateY(-1px); }
.btn-importar-cliente:disabled { background-color: #e2e8f0; color: #94a3b8; cursor: not-allowed; box-shadow: none; border-color: #cbd5e1; }

.buscador input { padding: 10px 16px; width: 280px; border: 1px solid #cbd5e1; border-radius: 8px; font-size: 0.9rem; outline: none; background-color: #f8fafc; transition: all 0.2s ease; color: #334155; }
.buscador input:focus { border-color: #3b82f6; background-color: #ffffff; box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.15); }

/* TABS PREMIUM STYLE */
.tabs-container { display: flex; gap: 4px; margin-bottom: 25px; border-bottom: 2px solid #f1f5f9; padding-bottom: 0; }
.tab-btn { background: none; border: none; padding: 14px 22px; font-size: 0.95rem; color: #64748b; cursor: pointer; font-weight: 700; display: flex; align-items: center; gap: 8px; border-bottom: 3px solid transparent; transition: all 0.2s ease; }
.tab-btn:hover { color: #1e293b; background-color: #f8fafc; border-radius: 8px 8px 0 0; }
.tab-btn.active { color: #2563eb; border-bottom-color: #2563eb; }

.counter { background: #f1f5f9; padding: 2px 10px; border-radius: 20px; font-size: 0.78rem; color: #475569; font-weight: 800; }
.tab-btn.active .counter { background: #dbeafe; color: #2563eb; }

/* TOOLBARS DESIGN */
.toolbar-clientes { background-color: #f8fafc; padding: 10px 14px; border-radius: 10px; margin-bottom: 25px; border: 1px solid #e2e8f0; display: inline-flex; }
.sub-tabs { display: flex; gap: 4px; }
.sub-tabs button { border: none; background: transparent; padding: 8px 16px; font-size: 0.88rem; color: #64748b; cursor: pointer; border-radius: 6px; font-weight: 700; transition: all 0.15s ease; }
.sub-tabs button:hover { color: #1e293b; }
.sub-tabs button.sub-active { background: #ffffff; color: #15803d; box-shadow: 0 1px 3px rgba(0,0,0,0.06); border: 1px solid #e2e8f0; }

.toolbar-fazon { background-color: #ffffff; padding: 20px; border-radius: 12px; margin-bottom: 25px; border: 1px solid #e2e8f0; display: flex; justify-content: space-between; align-items: flex-end; flex-wrap: wrap; gap: 20px; box-shadow: 0 2px 8px rgba(0,0,0,0.02); }
.filtros-izquierda { display: flex; gap: 20px; flex-wrap: wrap; }
.filtro-columna { display: flex; flex-direction: column; gap: 6px; }
.filtro-columna label { font-size: 0.82rem; color: #475569; font-weight: 700; text-transform: uppercase; letter-spacing: 0.3px; }
.select-chico { padding: 9px 14px; border: 1px solid #cbd5e1; border-radius: 8px; font-size: 0.92rem; width: 220px; color: #1e293b; background: #f8fafc; outline: none; font-weight: 600; transition: all 0.2s ease; }
.select-chico:focus { border-color: #3b82f6; background: white; }

.acciones-derecha-fazon { display: flex; align-items: center; gap: 15px; }
.btn-exportar-fazon { background-color: #2563eb; color: white; border: none; padding: 0 18px; border-radius: 8px; font-weight: 700; cursor: pointer; transition: all 0.2s ease; height: 38px; display: flex; align-items: center; box-shadow: 0 2px 4px rgba(37,99,235,0.2); }
.btn-exportar-fazon:hover { background-color: #1d4ed8; }
.toggle-agrupado { display: flex; align-items: center; background: #fff7ed; padding: 0 16px; border-radius: 8px; border: 1px solid #fed7aa; color: #c2410c; font-weight: 700; cursor: pointer; height: 38px; font-size: 0.88rem; transition: all 0.2s ease; }
.toggle-agrupado:hover { background: #ffedd5; }
.toggle-agrupado input { margin-right: 8px; transform: scale(1.1); cursor: pointer; }

/* TABLES MINIMALISTIC INDUSTRIAL LOOK */
.tabla-wrapper { overflow-x: auto; border: 1px solid #e2e8f0; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.01); background: white; }
table { width: 100%; border-collapse: collapse; font-size: 0.92rem; text-align: left; }
th { background: #f8fafc; padding: 14px 16px; font-weight: 700; color: #475569; border-bottom: 2px solid #e2e8f0; font-size: 0.82rem; text-transform: uppercase; letter-spacing: 0.5px; }
td { padding: 14px 16px; border-bottom: 1px solid #f1f5f9; vertical-align: middle; color: #334155; transition: background 0.15s ease; }
tr:hover td { background-color: #f8fafc; }

.sku-cell span { font-family: 'SFMono-Regular', Consolas, monospace; font-weight: 700; color: #475569; background: #f1f5f9; padding: 3px 8px; border-radius: 6px; font-size: 0.82rem; border: 1px solid #e2e8f0; }
.nombre-prod { font-weight: 700; color: #0f172a; font-size: 0.92rem; }

/* BADGES DEFINITIONS */
.badge-proveedor { background-color: #faf5ff; color: #6b21a8; padding: 4px 10px; border-radius: 6px; font-size: 0.78rem; font-weight: 700; border: 1px solid #f3e8ff; text-transform: uppercase; }
.btn-global { background: #7c3aed; color: white; border: none; padding: 3px 12px; border-radius: 20px; font-size: 0.72rem; font-weight: 700; cursor: pointer; margin-top: 6px; display: inline-flex; box-shadow: 0 2px 4px rgba(124,58,237,0.2); }
.btn-global:hover { background: #6d28d9; }

.text-null { color: #94a3b8; font-style: italic; font-size: 0.85rem; }

/* BAJO STOCK CARD STATE */
tr.bajo-stock td { background-color: #fef2f2; }
tr.bajo-stock:hover td { background-color: #fee2e2; }
tr.bajo-stock .sku-cell span { color: #991b1b; border-color: #fca5a5; background: #fee2e2; }
tr.bajo-stock .nombre-prod { color: #991b1b; }

/* RESERVADO BUTTON */
.btn-reservado { background: #fffbeb; border: 1px solid #fde68a; color: #b45309; padding: 6px 14px; border-radius: 20px; font-size: 0.82rem; font-weight: 700; cursor: pointer; transition: all 0.2s ease; box-shadow: 0 1px 2px rgba(180,83,9,0.05); }
.btn-reservado:hover { background: #fef3c7; border-color: #fcd34d; }

.btn-buchon { background: #fff7ed; border: 1px solid #ffedd5; color: #ea580c; padding: 2px 8px; border-radius: 6px; font-weight: 700; cursor: pointer; font-size: 0.8rem; }

/* ACTIONS ROW STYLING */
.celda-acciones { display: flex; gap: 6px; }
.btn-accion-tabla { border: 1px solid #e2e8f0; width: 32px; height: 32px; border-radius: 8px; cursor: pointer; display: flex; align-items: center; justify-content: center; font-size: 0.95rem; background: #ffffff; transition: all 0.2s ease; box-shadow: 0 1px 2px rgba(0,0,0,0.02); }
.btn-accion-tabla:hover { transform: translateY(-1px); box-shadow: 0 3px 6px rgba(0,0,0,0.05); }
.btn-edit:hover { background: #ecfdf5; border-color: #a7f3d0; color: #059669; }
.btn-ajuste:hover { background: #fffbeb; border-color: #fde68a; color: #d97706; }
.btn-kardex:hover { background: #f0f9ff; border-color: #bae6fd; color: #0284c7; }

/* ROW TAGS */
.tags-fila { display: flex; gap: 6px; align-items: center; margin-top: 6px; }
.tag-fazon { background: #6b21a8; color: white; padding: 2px 6px; border-radius: 4px; font-size: 0.68rem; font-weight: 800; text-transform: uppercase; letter-spacing: 0.3px; }
.tag-molido { color: #16a34a; font-weight: 800; font-size: 0.75rem; text-transform: uppercase; }
.tag-scrap { color: #ea580c; font-weight: 800; font-size: 0.75rem; text-transform: uppercase; }

.badge-cliente-purple { background-color: #7c3aed; color: white; padding: 3px 8px; border-radius: 6px; font-size: 0.75rem; font-weight: 700; box-shadow: 0 1px 2px rgba(124,58,237,0.15); }
.badge-propio { background: #eff6ff; color: #1e40af; padding: 3px 8px; border-radius: 6px; font-size: 0.75rem; font-weight: 700; border: 1px solid #bfdbfe; }

.badge-material { background: #475569; color: white; padding: 4px 12px; border-radius: 20px; font-size: 0.78rem; font-weight: 800; letter-spacing: 0.3px; box-shadow: inset 0 -1px 0 rgba(0,0,0,0.1); }
.badge-material.pai { background: #f97316; }
.badge-material.pead { background: #10b981; }
.badge-material.pp { background: #8b5cf6; }
.badge-material.abs { background: #ef4444; }

/* GROUP VISTA (MOLIDO) */
.fila-grupo { background-color: #f8fafc; cursor: pointer; font-weight: bold; border-top: 1px solid #e2e8f0; }
.fila-grupo:hover td { background-color: #f1f5f9 !important; }
.fila-detalle-grupo td { background-color: #ffffff; font-size: 0.88rem; border-bottom: 1px dashed #e2e8f0; padding-top: 10px; padding-bottom: 10px; }

/* PREMIUM MODALS */
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(15, 23, 42, 0.4); backdrop-filter: blur(4px); display: flex; align-items: center; justify-content: center; z-index: 2000; transition: all 0.3s; }
.modal-reserva { background: #ffffff; border-radius: 12px; width: 580px; max-height: 85vh; display: flex; flex-direction: column; padding: 30px; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1), 0 10px 10px -5px rgba(0,0,0,0.04); }
.modal-header-reserva h3 { margin: 0 0 10px 0; color: #1e293b; font-size: 1.3rem; font-weight: 800; border-bottom: 1px solid #f1f5f9; padding-bottom: 12px; }
.nombre-prod-modal { color: #475569; font-weight: 700; font-size: 1rem; margin-bottom: 20px; background: #f8fafc; padding: 10px 14px; border-radius: 8px; border-left: 4px solid #3b82f6; }

.modal-body-reserva { overflow-y: auto; flex-grow: 1; padding-right: 4px; }
.modal-body-reserva::-webkit-scrollbar { width: 6px; }
.modal-body-reserva::-webkit-scrollbar-thumb { background: #cbd5e1; border-radius: 10px; }

.tabla-reservas-detalle th { background: #f8fafc; border-bottom: 2px solid #e2e8f0; padding: 10px; font-weight: 700; font-size: 0.8rem; }
.reserva-nota { color: #2563eb; font-weight: 700; font-family: monospace; }
.reserva-kilos { color: #c2410c; font-weight: 700; text-align: right; }

.modal-footer-reserva { text-align: right; margin-top: 20px; }
.btn-cerrar-reserva { background: #ef4444; color: white; border: none; padding: 10px 22px; border-radius: 8px; font-weight: 700; cursor: pointer; transition: background 0.2s; }
.btn-cerrar-reserva:hover { background: #dc2626; }

.modal-content { background: white; padding: 30px; border-radius: 12px; width: 440px; text-align: center; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); }
.modal-stats { display: flex; justify-content: space-around; margin: 25px 0; gap: 15px; }
.stat-item { display: flex; flex-direction: column; align-items: center; background: #f8fafc; padding: 15px; border-radius: 10px; flex: 1; border: 1px solid #e2e8f0; }
.stat-item .num { font-size: 1.9rem; font-weight: 800; color: #1e293b; margin-top: 5px; }
.stat-item.creado .num { color: #10b981; }
.stat-item.actualizado .num { color: #3b82f6; }
.stat-item .label { font-size: 0.78rem; color: #64748b; font-weight: 600; margin-top: 2px; }

.btn-cerrar { background: #1e293b; color: white; border: none; padding: 11px 30px; border-radius: 8px; cursor: pointer; font-weight: 700; font-size: 0.92rem; width: 100%; margin-top: 10px; transition: background 0.2s; }
.btn-cerrar:hover { background: #0f172a; }

.vacio { text-align: center; padding: 50px; color: #94a3b8; font-style: italic; font-weight: 500; font-size: 1rem; }
.loading { text-align: center; padding: 50px; color: #2563eb; font-weight: 700; font-size: 1rem; }

/* CARDS TOTALES REPORTE GLOBAL */
.modal-lg { max-width: 820px !important; width: 820px; text-align: left; }
.tarjetas-resumen { display: flex; gap: 16px; margin: 20px 0; flex-wrap: wrap; }
.tarjeta-stock { flex: 1; min-width: 200px; padding: 20px; border-radius: 10px; text-align: center; color: white; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.tarjeta-stock span { font-size: 0.72rem; font-weight: 700; opacity: 0.9; letter-spacing: 0.5px; text-transform: uppercase; }
.tarjeta-stock h2 { margin: 8px 0 0 0; font-size: 1.75rem; font-weight: 800; }
.fisico { background: linear-gradient(135deg, #475569, #334155); }
.reservado { background: linear-gradient(135deg, #f97316, #ea580c); }
.libre { background: linear-gradient(135deg, #10b981, #059669); }

.text-error { background: #fef2f2; border: 1px solid #fca5a5; color: #991b1b; padding: 10px; border-radius: 6px; font-size: 0.85rem; font-weight: 600; margin-bottom: 15px; }

@media (max-width: 768px) {
    .header-stock { flex-direction: column; align-items: stretch; }
    .acciones-header { flex-direction: column; align-items: stretch; }
    .buscador input { width: 100%; }
}
</style>