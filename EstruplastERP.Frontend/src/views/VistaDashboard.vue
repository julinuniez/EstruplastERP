<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import {
    Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, 
    BarElement, ArcElement, Title, Tooltip, Legend, Filler
} from 'chart.js';
import { Bar, Doughnut, Line } from 'vue-chartjs';
import packageInfo from '../../package.json';
import { Alertas } from '@/utils/alertas';
// 🚀 Importamos ExcelJS y FileSaver para generar el reporte premium con filtros dinámicos
import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, ArcElement, Title, Tooltip, Legend, Filler);

const version = packageInfo.version || '1.0.0';
const cargando = ref(true);
const descargandoExcel = ref(false);
const error = ref('');

const cotizacionDolar = ref({ compra: 0, venta: 0, fecha: '' });
const alertasCriticas = ref<any[]>([]);

const fechaFiltro = ref(new Date()); 

const nombreMesSeleccionado = computed(() => {
    const nombre = fechaFiltro.value.toLocaleDateString('es-AR', { month: 'long', year: 'numeric' });
    return nombre.charAt(0).toUpperCase() + nombre.slice(1); 
});

const esMesActual = computed(() => {
    const hoy = new Date();
    return fechaFiltro.value.getMonth() === hoy.getMonth() && fechaFiltro.value.getFullYear() === hoy.getFullYear();
});

function cambiarMes(delta: number) {
    const nuevaFecha = new Date(fechaFiltro.value);
    nuevaFecha.setMonth(nuevaFecha.getMonth() + delta);
    fechaFiltro.value = nuevaFecha;
    cargarDatos();
}

const kpis = ref({ produccionMes: 0, variacionMes: 0, esPositivo: true, kilosPendientes: 0 });
const resumenMensual = ref<any[]>([]);
const produccionSemanal = ref<any[]>([]);
const topProductos = ref<any[]>([]);
const topMateriales = ref<any[]>([]);
const topClientes = ref<any[]>([]);
const stockMateriales = ref<any[]>([]); 

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

async function cargarDolarBNA() {
    try {
        const res = await axios.get('https://dolarapi.com/v1/dolares/oficial');
        cotizacionDolar.value = {
            compra: res.data.compra,
            venta: res.data.venta,
            fecha: new Date(res.data.fechaActualizacion).toLocaleTimeString('es-AR', { hour: '2-digit', minute: '2-digit' })
        };
    } catch (e) {
        console.error("No se pudo obtener la cotización del dólar");
    }
}

async function cargarDatos() {
    cargando.value = true;
    error.value = '';
    
    const mes = fechaFiltro.value.getMonth() + 1;
    const anio = fechaFiltro.value.getFullYear();
    const query = `?mes=${mes}&anio=${anio}`;

    try {
        const [resKpis, resMes, resSemana, resProd, resMat, resClientes, resInventario] = await Promise.all([
            axios.get(`${apiUrl}/Estadisticas/resumen-kpis${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/resumen-mensual`, getAuthConfig()), 
            axios.get(`${apiUrl}/Estadisticas/produccion-semanal`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-productos${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-materiales${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-clientes${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Productos`, getAuthConfig()) 
        ]);

        resumenMensual.value = Array.isArray(resMes.data) ? resMes.data : [];
        produccionSemanal.value = Array.isArray(resSemana.data) ? resSemana.data : [];
        topProductos.value = Array.isArray(resProd.data) ? resProd.data : [];
        topMateriales.value = Array.isArray(resMat.data) ? resMat.data : [];
        topClientes.value = Array.isArray(resClientes.data) ? resClientes.data : [];

        const todosLosProductos = Array.isArray(resInventario.data) ? resInventario.data : [];
        
        alertasCriticas.value = todosLosProductos.filter(p => {
            const stockActual = (p.stockDisponible ?? p.stockFisico ?? p.stockActual) || 0;
            const stockMin = p.stockMinimo || 0;
            return p.esCritico === true && stockActual <= stockMin; 
        });

        stockMateriales.value = todosLosProductos
            .filter(p => {
                const nombre = (p.nombre || '').toUpperCase();
                const rubro = (p.rubro || '').toUpperCase();
                
                return p.esMateriaPrima && 
                       !p.esScrap && 
                       !rubro.includes('MOLIDO') && 
                       !rubro.includes('CLIENTE') &&
                       (p.clienteId === null || p.clienteId === 0) &&
                       !nombre.includes('BASE') &&
                       !nombre.includes('GENERICO') &&
                       !nombre.includes('GENÉRICO') &&
                       p.id !== 90;
            })
            .sort((a, b) => (b.stockDisponible || 0) - (a.stockDisponible || 0)) 
            .slice(0, 7); 

        const prodActual = Math.round(resKpis.data?.produccionMes || 0);
        const prodAnterior = Math.round(resKpis.data?.produccionMesAnterior || 0);
        const pendientes = Math.round(resKpis.data?.kilosPendientes || 0);
        
        let variacion = 0;
        let positivo = true;

        if (prodAnterior > 0) {
            variacion = ((prodActual - prodAnterior) / prodAnterior) * 100;
            positivo = variacion >= 0;
        } else if (prodActual > 0) {
            variacion = 100;
        }

        kpis.value = {
            produccionMes: prodActual,
            variacionMes: Math.abs(variacion),
            esPositivo: positivo,
            kilosPendientes: pendientes
        };

    } catch (e) {
        error.value = "Error al conectar con el servidor de métricas.";
    } finally {
        cargando.value = false;
    }
}

async function exportarProduccionAExcel() {
    if (descargandoExcel.value) return;
    descargandoExcel.value = true;
    
    const mes = fechaFiltro.value.getMonth() + 1;
    const anio = fechaFiltro.value.getFullYear();

    try {
        // 🚀 Ahora leemos SOLO del Kardex (Movimientos), que tiene la verdad absoluta y filtrada
        const resMovimientos = await axios.get(`${apiUrl}/Movimientos/auditoria?mes=${mes}&anio=${anio}`, getAuthConfig());
        const datosMovimientos = Array.isArray(resMovimientos.data) ? resMovimientos.data : [];

        if (datosMovimientos.length === 0) {
            Alertas.error(`No se detectaron consumos reales en el período seleccionado.`);
            descargandoExcel.value = false;
            return;
        }

        const workbook = new ExcelJS.Workbook();
        
        // --- HOJA 1: TABLA MAESTRA INTERACTIVA ---
        const wsMaestra = workbook.addWorksheet('Auditoría de Consumos');
        wsMaestra.views = [{ showGridLines: true }];

        wsMaestra.mergeCells('A1:F1');
        const cellTitulo = wsMaestra.getCell('A1');
        cellTitulo.value = `REPORTE UNIFICADO DE CONSUMOS REALES - ${nombreMesSeleccionado.value.toUpperCase()}`;
        cellTitulo.font = { name: 'Segoe UI', size: 14, bold: true, color: { argb: 'FFFFFFFF' } };
        cellTitulo.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF2C3E50' } };
        cellTitulo.alignment = { horizontal: 'center', vertical: 'middle' };
        wsMaestra.getRow(1).height = 35;

        wsMaestra.getCell('A2').value = `Filtre las columnas para obtener totales dinámicos por Cliente o Insumo.`;
        wsMaestra.getCell('A2').font = { name: 'Segoe UI', size: 10, italic: true, color: { argb: 'FF7F8C8D' } };

        const columnasMaestra = [
            { header: 'Fecha Mov.', key: 'fecha', width: 15 },
            { header: 'Origen / Módulo', key: 'origen', width: 22 },
            { header: 'Dueño / Cliente', key: 'cliente', width: 35 },
            { header: 'Material / Insumo', key: 'insumo', width: 45 },
            { header: 'Tipo Movimiento', key: 'tipo', width: 20 },
            { header: 'Consumo Real (Kg)', key: 'kilos', width: 22 }
        ];
        wsMaestra.columns = columnasMaestra;

        wsMaestra.getRow(4).height = 25;
        wsMaestra.getRow(4).eachCell((cell) => {
            cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF34495E' } };
            cell.font = { name: 'Segoe UI', color: { argb: 'FFFFFFFF' }, bold: true, size: 10 };
            cell.alignment = { horizontal: 'center', vertical: 'middle' };
            cell.border = { bottom: { style: 'medium', color: { argb: 'FF2C3E50' } } };
        });

        let filaActual = 5;

        // Inyectamos la data sin duplicaciones
        datosMovimientos.forEach(m => {
            const row = wsMaestra.addRow({
                fecha: m.fecha ? new Date(m.fecha).toLocaleDateString('es-AR') : '-',
                origen: m.tipoMovimiento === 'CONSUMO_MEZCLA' ? 'Hoja de Carga (Mezcla)' : 'Cierre de OP',
                cliente: m.clienteNombre || 'Stock Estruplast (Propio)',
                insumo: m.productoNombre || 'Materia Prima',
                tipo: m.tipoMovimiento || 'CONSUMO',
                kilos: Math.abs(Number(m.cantidad || 0))
            });
            row.getCell('kilos').numFmt = '#,##0.00 "Kg"';
            filaActual++;
        });

        wsMaestra.autoFilter = `A4:F${filaActual - 1}`;

        const filaTotales = filaActual + 1;
        wsMaestra.mergeCells(`A${filaTotales}:E${filaTotales}`);
        wsMaestra.getCell(`A${filaTotales}`).value = 'TOTAL CONSUMIDO SELECCIONADO:';
        wsMaestra.getCell(`A${filaTotales}`).font = { name: 'Segoe UI', bold: true, size: 11, color: { argb: 'FF27AE60' } };
        wsMaestra.getCell(`A${filaTotales}`).alignment = { horizontal: 'right', vertical: 'middle' };

        const cellTotalKilos = wsMaestra.getCell(`F${filaTotales}`);
        cellTotalKilos.value = { formula: `=SUBTOTAL(109,F5:F${filaTotales - 2})`, date1904: false };
        cellTotalKilos.font = { name: 'Segoe UI', bold: true, size: 12, color: { argb: 'FF27AE60' } };
        cellTotalKilos.numFmt = '#,##0.00 "Kg"';
        cellTotalKilos.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FFE8F8F5' } };
        
        wsMaestra.getRow(filaTotales).height = 24;
        wsMaestra.getRow(filaTotales).border = {
            top: { style: 'thin', color: { argb: 'FFBDC3C7' } },
            bottom: { style: 'double', color: { argb: 'FF27AE60' } }
        };

        // --- HOJA 2: RESUMEN DE KILOS ACUMULADOS POR CLIENTE ---
        const wsClientes = workbook.addWorksheet('Resumen por Cliente');
        wsClientes.views = [{ showGridLines: true }];
        
        wsClientes.addRow(['CLIENTE / DUEÑO', 'KILOS CONSUMIDOS TOTALES (MES)']);
        wsClientes.getRow(1).font = { name: 'Segoe UI', bold: true, color: { argb: 'FFFFFFFF' } };
        wsClientes.getRow(1).eachCell(c => c.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FFE67E22' } });
        wsClientes.columns = [{ width: 45 }, { width: 30 }];

        const mapaClientes: Record<string, number> = {};
        datosMovimientos.forEach(m => { 
            const cl = m.clienteNombre || 'Stock Estruplast (Propio)';
            mapaClientes[cl] = (mapaClientes[cl] || 0) + Math.abs(Number(m.cantidad || 0)); 
        });

        Object.entries(mapaClientes).forEach(([cli, kg]) => {
            const r = wsClientes.addRow([cli, kg]);
            r.getCell(2).numFmt = '#,##0.00 "Kg"';
        });

        const buffer = await workbook.xlsx.writeBuffer();
        saveAs(new Blob([buffer]), `Auditoria_Consumos_Estruplast_${mes}_${anio}.xlsx`);
        
        Alertas.exito("¡Reporte generado correctamente!");

    } catch (e) {
        console.error("Error en exportación:", e);
        Alertas.error("Ocurrió un error al generar el archivo.");
    } finally {
        descargandoExcel.value = false;
    }
}

const obtenerRangoFechasPorSemana = (periodoStr: string) => {
    if (!periodoStr) return '';
    const match = periodoStr.match(/\d+/);
    if (!match) return periodoStr;

    const numeroSemana = parseInt(match[0], 10);
    const anio = new Date().getFullYear(); 

    const fechaBase = new Date(anio, 0, 4);
    const diaDeLaSemana = fechaBase.getDay() || 7; 
    
    fechaBase.setDate(fechaBase.getDate() - diaDeLaSemana + 1);
    
    const lunes = new Date(fechaBase);
    lunes.setDate(lunes.getDate() + (numeroSemana - 1) * 7);
    
    const domingo = new Date(lunes);
    domingo.setDate(domingo.getDate() + 6);
    
    const opciones: Intl.DateTimeFormatOptions = { day: '2-digit', month: 'short' };
    const strInicio = lunes.toLocaleDateString('es-AR', opciones).replace('.', '');
    const strFin = domingo.toLocaleDateString('es-AR', opciones).replace('.', '');
    
    return `${strInicio} al ${strFin}`;
};

const chartDataMensual = computed(() => ({
    labels: resumenMensual.value.map(m => m?.periodo || ''),
    datasets: [{
        label: 'Kilos (Últimos 12 meses)',
        borderColor: '#27ae60',
        backgroundColor: 'rgba(39, 174, 96, 0.2)',
        data: resumenMensual.value.map(m => Math.round(m?.kilos || 0)),
        fill: true, tension: 0.3
    }]
}));

const chartDataSemanal = computed(() => ({
    labels: produccionSemanal.value.map(s => obtenerRangoFechasPorSemana(s?.periodo || '')),
    datasets: [{
        label: 'Kilos (Últimas 8 semanas)',
        backgroundColor: '#3498db',
        borderRadius: 4,
        data: produccionSemanal.value.map(s => Math.round(s?.kilos || 0))
    }]
}));

const chartDataMateriales = computed(() => ({
    labels: topMateriales.value.map(m => m?.material || 'Desconocido'),
    datasets: [{
        label: 'Kilos Consumidos',
        backgroundColor: '#9b59b6',
        borderRadius: 4,
        data: topMateriales.value.map(m => Math.round(m?.totalKilos || 0))
    }]
}));

const chartDataStock = computed(() => ({
    labels: stockMateriales.value.map(m => m?.nombre || 'Desconocido'),
    datasets: [{
        label: 'Stock Disponible (Kg)',
        backgroundColor: '#1abc9c', 
        borderRadius: 4,
        data: stockMateriales.value.map(m => Math.round(m?.stockDisponible || 0))
    }]
}));

const chartDataProductos = computed(() => {
    const etiquetas = topProductos.value.map(p => p?.producto || 'Desconocido');
    const datosKilos = topProductos.value.map(p => Math.round(p?.totalKilos || 0));

    const coloresProductos = ['#3498db', '#2ecc71', '#e67e22', '#e74c3c', '#9b59b6'];
    const colorGrisOtros = '#95a5a6'; 

    const listaColoresFinal = etiquetas.map((label, index) => {
        if (label === 'OTROS PRODUCTOS') {
            return colorGrisOtros;
        }
        return coloresProductos[index % coloresProductos.length];
    });

    return {
        labels: etiquetas,
        datasets: [{
            backgroundColor: listaColoresFinal, 
            borderWidth: 2, 
            borderColor: '#ffffff',
            data: datosKilos
        }]
    };
});

const chartDataClientes = computed(() => ({
    labels: topClientes.value.map(c => c?.cliente || 'Sin Cliente'),
    datasets: [{
        label: 'Kilos Comprados',
        backgroundColor: '#e67e22',
        borderRadius: 4,
        data: topClientes.value.map(c => Math.round(c?.totalKilos || 0))
    }]
}));

onMounted(() => {
    cargarDolarBNA();
    cargarDatos();
});
</script>

<template>
    <div class="dashboard-container">
        <div class="header-dashboard">
            <h2>📊 Tablero de Control <small class="version">v{{ version }}</small></h2>
            
            <div class="widget-dolar" v-if="cotizacionDolar.venta > 0" title="Cotización Oficial DolarAPI">
                <span class="dolar-icono">💵 BNA</span>
                <div class="dolar-valores">
                    <span>C: ${{ cotizacionDolar.compra }}</span>
                    <strong>V: ${{ cotizacionDolar.venta }}</strong>
                </div>
            </div>
            
            <div class="selector-mes-app">
                <button @click="cambiarMes(-1)" class="btn-flecha">❮</button>
                <span class="etiqueta-mes">{{ nombreMesSeleccionado }}</span>
                <button @click="cambiarMes(1)" class="btn-flecha" :disabled="esMesActual" :class="{ 'disabled': esMesActual }">❯</button>
            </div>
            
            <div class="acciones-header">
                <button @click="exportarProduccionAExcel" class="btn-excel" :disabled="descargandoExcel || cargando">
                    <span v-if="descargandoExcel">⏳ Generando...</span>
                    <span v-else>📥 Exportar a Excel</span>
                </button>
                
                <button @click="cargarDatos" class="btn-refresh" title="Actualizar datos">🔄</button>
            </div>
        </div>

        <div v-if="alertasCriticas.length > 0" class="alerta-urgente-criticos">
            <div class="alerta-header">
                <strong>🚨 ¡ATENCIÓN! QUIEBRE DE STOCK EN MATERIALES CRÍTICOS 🚨</strong>
            </div>
            <ul class="lista-criticos">
                <li v-for="item in alertasCriticas" :key="item.id">
                    El insumo <strong>{{ item.nombre }}</strong> tiene <strong>{{ item.stockActual || 0 }} kg</strong> 
                    (Mínimo requerido: {{ item.stockMinimo }} kg).
                </li>
            </ul>
        </div>

        <div v-if="cargando" class="loading">⏳ Cargando información del periodo...</div>
        <div v-else-if="error" class="error-msg">{{ error }}</div>

        <div v-else class="contenido-dashboard">
            <div class="fila-kpis">
                <div class="card-kpi azul">
                    <div class="icono">⚖️</div>
                    <div class="info">
                        <span class="titulo">Producción en {{ nombreMesSeleccionado.split(' ')[0] }}</span>
                        <strong class="valor">{{ kpis.produccionMes.toLocaleString('es-AR') }} kg</strong>
                    </div>
                </div>
                
                <div class="card-kpi naranja">
                    <div class="icono">🔥</div>
                    <div class="info">
                        <span class="titulo">Kilos Pendientes en Planta</span>
                        <strong class="valor">{{ kpis.kilosPendientes.toLocaleString('es-AR') }} kg</strong>
                    </div>
                </div>

                <div class="card-kpi" :class="kpis.esPositivo ? 'verde' : 'rojo'">
                    <div class="icono">{{ kpis.esPositivo ? '📈' : '📉' }}</div>
                    <div class="info">
                        <span class="titulo">Comparativa vs Mes Anterior</span>
                        <strong class="valor">
                            {{ kpis.esPositivo ? '+' : '-' }}{{ kpis.variacionMes.toFixed(1) }}%
                        </strong>
                    </div>
                </div>
            </div>

            <div class="grid-principal">
                
                <div class="card">
                    <h3>📦 Stock Disponible (MP Virgen - Top 7)</h3>
                    <div class="area-grafico" v-if="stockMateriales.length > 0">
                        <Bar :data="chartDataStock" :options="{ indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }" />
                    </div>
                    <p class="sin-datos" v-else>No hay stock registrado.</p>
                </div>

                <div class="card">
                    <h3>🛢️ Insumos Consumidos ({{ nombreMesSeleccionado }})</h3>
                    <div class="area-grafico" v-if="topMateriales.length > 0">
                        <Bar :data="chartDataMateriales" :options="{ indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }" />
                    </div>
                    <p class="sin-datos" v-else>Sin consumos en este mes.</p>
                </div>

                <div class="card">
                    <h3>📅 Evolución Mensual (Largo Plazo)</h3>
                    <div class="area-grafico">
                        <Line :data="chartDataMensual" :options="{ responsive: true, maintainAspectRatio: false }" />
                    </div>
                </div>

                <div class="card">
                    <h3>📆 Producción Semanal (Últimos 2 Meses)</h3>
                    <div class="area-grafico">
                        <Bar :data="chartDataSemanal" :options="{ responsive: true, maintainAspectRatio: false }" />
                    </div>
                </div>

                <div class="card">
                    <h3>🤝 Top 7 Clientes ({{ nombreMesSeleccionado }})</h3>
                    <div class="area-grafico" v-if="topClientes.length > 0">
                        <Bar :data="chartDataClientes" :options="{ responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }" />
                    </div>
                    <p class="sin-datos" v-else>No hay ventas registradas en este mes.</p>
                </div>

                <div class="card">
                    <h3>🏆 Top Productos ({{ nombreMesSeleccionado }})</h3>
                    <div class="area-grafico" v-if="topProductos.length > 0">
                        <Doughnut :data="chartDataProductos" :options="{ responsive: true, maintainAspectRatio: false }" />
                    </div>
                    <p class="sin-datos" v-else>Sin producción este mes.</p>
                </div>

            </div>
        </div>
    </div>
</template>

<style scoped>
.dashboard-container { padding: 20px; font-family: 'Segoe UI', sans-serif; background-color: #f4f6f9; min-height: 100vh; }

.header-dashboard { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; border-bottom: 2px solid #ddd; padding-bottom: 15px; flex-wrap: wrap; gap: 15px; }
.header-dashboard h2 { margin: 0; color: #2c3e50; display: flex; align-items: center; gap: 10px; font-size: 1.5rem; }
.version { font-size: 0.8rem; background: #34495e; color: white; padding: 3px 8px; border-radius: 12px; font-weight: normal;}

.selector-mes-app { display: flex; align-items: center; background: white; border-radius: 25px; padding: 5px 15px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); border: 1px solid #eee; }
.btn-flecha { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #3498db; font-weight: bold; padding: 0 15px; transition: transform 0.2s; }
.btn-flecha:hover:not(.disabled) { transform: scale(1.2); }
.btn-flecha.disabled { color: #ccc; cursor: not-allowed; }
.etiqueta-mes { font-size: 1.1rem; font-weight: bold; color: #2c3e50; min-width: 140px; text-align: center; }

.acciones-header { display: flex; gap: 10px; align-items: center; }

.btn-excel {
    background-color: #27ae60;
    color: white;
    border: none;
    padding: 10px 15px;
    border-radius: 6px;
    font-weight: bold;
    cursor: pointer;
    font-size: 0.95rem;
    display: flex;
    align-items: center;
    gap: 5px;
    transition: background-color 0.2s, transform 0.1s;
    box-shadow: 0 2px 5px rgba(39, 174, 96, 0.3);
}
.btn-excel:hover:not(:disabled) {
    background-color: #219150;
    transform: translateY(-1px);
}
.btn-excel:disabled {
    background-color: #95a5a6;
    cursor: not-allowed;
    box-shadow: none;
}

.btn-refresh { background: white; border: 1px solid #ccc; padding: 8px 12px; border-radius: 50%; cursor: pointer; color: #34495e; transition: background 0.2s;}
.btn-refresh:hover { background: #eef2f5; }

.widget-dolar { display: flex; align-items: center; background: #2c3e50; color: white; padding: 5px 15px; border-radius: 8px; border: 1px solid #34495e; }
.dolar-icono { font-weight: 900; color: #2ecc71; margin-right: 10px; font-size: 1.1rem; }
.dolar-valores { display: flex; flex-direction: column; font-size: 0.8rem; }
.dolar-valores strong { font-size: 0.95rem; color: #f1c40f; }

.alerta-urgente-criticos { background-color: #c0392b; color: white; padding: 15px; border-radius: 8px; margin-bottom: 20px; box-shadow: 0 4px 15px rgba(192, 57, 43, 0.4); border-left: 8px solid #922b21; animation: latido 2s infinite; }
.alerta-urgente-criticos .alerta-header { font-size: 1.1rem; border-bottom: 1px solid rgba(255,255,255,0.3); padding-bottom: 8px; margin-bottom: 8px; }
.lista-criticos { margin: 0; padding-left: 20px; font-size: 0.95rem; }
@keyframes latido { 0% { box-shadow: 0 0 0 0 rgba(192, 57, 43, 0.7); } 70% { box-shadow: 0 0 0 10px rgba(192, 57, 43, 0); } 100% { box-shadow: 0 0 0 0 rgba(192, 57, 43, 0); } }

.fila-kpis { display: flex; gap: 20px; margin-bottom: 30px; flex-wrap: wrap; }
.card-kpi { flex: 1; min-width: 250px; background: white; padding: 25px; border-radius: 15px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); display: flex; align-items: center; border-left: 6px solid #ccc; }
.card-kpi .icono { font-size: 3rem; margin-right: 20px; }
.card-kpi .info { display: flex; flex-direction: column; }
.card-kpi .titulo { font-size: 0.9rem; color: #7f8c8d; text-transform: uppercase; font-weight: bold; margin-bottom: 5px; }
.card-kpi .valor { font-size: 2rem; font-weight: 900; color: #2c3e50; letter-spacing: -0.5px; }

.azul { border-left-color: #3498db; }
.verde { border-left-color: #2ecc71; }
.rojo { border-left-color: #e74c3c; }
.naranja { border-left-color: #e67e22; }

.grid-principal { display: grid; grid-template-columns: repeat(auto-fit, minmax(400px, 1fr)); gap: 25px; }

.card { background: white; padding: 25px; border-radius: 15px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); border: 1px solid #edf2f7; }
.card h3 { margin-top: 0; color: #34495e; font-size: 1.1rem; border-bottom: 1px dashed #eee; padding-bottom: 10px; margin-bottom: 20px; }

.area-grafico { height: 280px; position: relative; }
.sin-datos { text-align: center; color: #aaa; font-style: italic; margin-top: 50px; }

.loading { text-align: center; font-size: 1.2rem; color: #3498db; margin-top: 50px; font-weight: bold;}
.error-msg { text-align: center; color: #e74c3c; background: #fee; padding: 15px; border-radius: 8px; margin-top: 20px; font-weight: bold;}

@media (max-width: 768px) {
    .grid-principal { grid-template-columns: 1fr; }
    .header-dashboard { flex-direction: column; align-items: stretch; }
    .selector-mes-app { justify-content: space-between; }
    .acciones-header { justify-content: space-between; margin-top: 10px; }
    .widget-dolar { justify-content: center; }
}
</style>