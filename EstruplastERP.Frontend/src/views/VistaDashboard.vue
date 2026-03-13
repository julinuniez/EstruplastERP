<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import axios from 'axios';
import {
  Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, 
  BarElement, ArcElement, Title, Tooltip, Legend, Filler
} from 'chart.js';
import { Bar, Doughnut, Line } from 'vue-chartjs';
import packageInfo from '../../package.json';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, BarElement, ArcElement, Title, Tooltip, Legend, Filler);

const version = packageInfo.version || '1.0.0';
const cargando = ref(true);
const error = ref('');

// --- SELECTOR DE MES ESTILO MERCADO PAGO ---
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
    cargarDatos(); // Recarga el dashboard al cambiar
}
// --------------------------------------------

const kpis = ref({ produccionMes: 0, variacionMes: 0, esPositivo: true });
const resumenMensual = ref<any[]>([]);
const produccionSemanal = ref<any[]>([]);
const topProductos = ref<any[]>([]);
const topMateriales = ref<any[]>([]);
const topClientes = ref<any[]>([]);

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

async function cargarDatos() {
    cargando.value = true;
    error.value = '';
    
    const mes = fechaFiltro.value.getMonth() + 1;
    const anio = fechaFiltro.value.getFullYear();
    const query = `?mes=${mes}&anio=${anio}`;

    try {
        const [resKpis, resMes, resSemana, resProd, resMat, resClientes] = await Promise.all([
            axios.get(`${apiUrl}/Estadisticas/resumen-kpis${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/resumen-mensual`, getAuthConfig()), // Históricos globales
            axios.get(`${apiUrl}/Estadisticas/produccion-semanal`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-productos${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-materiales${query}`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-clientes${query}`, getAuthConfig())
        ]);

        resumenMensual.value = Array.isArray(resMes.data) ? resMes.data : [];
        produccionSemanal.value = Array.isArray(resSemana.data) ? resSemana.data : [];
        topProductos.value = Array.isArray(resProd.data) ? resProd.data : [];
        topMateriales.value = Array.isArray(resMat.data) ? resMat.data : [];
        topClientes.value = Array.isArray(resClientes.data) ? resClientes.data : [];

        // Cálculo de variación desde el backend
        const prodActual = resKpis.data?.produccionMes || 0;
        const prodAnterior = resKpis.data?.produccionMesAnterior || 0;
        
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
            esPositivo: positivo
        };

    } catch (e) {
        error.value = "Error al conectar con el servidor de métricas.";
    } finally {
        cargando.value = false;
    }
}

const chartDataMensual = computed(() => ({
    labels: resumenMensual.value.map(m => m?.periodo || ''),
    datasets: [{
        label: 'Kilos (Últimos 12 meses)',
        borderColor: '#27ae60',
        backgroundColor: 'rgba(39, 174, 96, 0.2)',
        data: resumenMensual.value.map(m => m?.kilos || 0),
        fill: true, tension: 0.3
    }]
}));

const chartDataSemanal = computed(() => ({
    labels: produccionSemanal.value.map(s => s?.periodo || ''),
    datasets: [{
        label: 'Kilos (Últimas 8 semanas)',
        backgroundColor: '#3498db',
        borderRadius: 4,
        data: produccionSemanal.value.map(s => s?.kilos || 0)
    }]
}));

const chartDataMateriales = computed(() => ({
    labels: topMateriales.value.map(m => m?.material || 'Desconocido'),
    datasets: [{
        label: 'Kilos Consumidos',
        backgroundColor: '#9b59b6',
        borderRadius: 4,
        data: topMateriales.value.map(m => m?.totalKilos || 0)
    }]
}));

const chartDataProductos = computed(() => ({
    labels: topProductos.value.map(p => p?.producto || 'Desconocido'),
    datasets: [{
        backgroundColor: ['#3498db', '#e74c3c', '#f1c40f', '#2ecc71', '#34495e'],
        data: topProductos.value.map(p => p?.totalKilos || 0)
    }]
}));

const chartDataClientes = computed(() => ({
    labels: topClientes.value.map(c => c?.cliente || 'Sin Cliente'),
    datasets: [{
        label: 'Kilos Comprados',
        backgroundColor: '#e67e22',
        borderRadius: 4,
        data: topClientes.value.map(c => c?.totalKilos || 0)
    }]
}));

onMounted(() => cargarDatos());
</script>

<template>
    <div class="dashboard-container">
        <div class="header-dashboard">
            <h2>📊 Tablero de Control <small class="version">v{{ version }}</small></h2>
            
            <div class="selector-mes-app">
                <button @click="cambiarMes(-1)" class="btn-flecha">❮</button>
                <span class="etiqueta-mes">{{ nombreMesSeleccionado }}</span>
                <button @click="cambiarMes(1)" class="btn-flecha" :disabled="esMesActual" :class="{ 'disabled': esMesActual }">❯</button>
            </div>
            
            <button @click="cargarDatos" class="btn-refresh">🔄</button>
        </div>

        <div v-if="cargando" class="loading">⏳ Cargando información del periodo...</div>
        <div v-else-if="error" class="error-msg">{{ error }}</div>

        <div v-else class="contenido-dashboard">
            <div class="fila-kpis">
                <div class="card-kpi azul">
                    <div class="icono">⚖️</div>
                    <div class="info">
                        <span class="titulo">Producción en {{ nombreMesSeleccionado.split(' ')[0] }}</span>
                        <strong class="valor">{{ kpis.produccionMes.toLocaleString('es-AR', { maximumFractionDigits: 2 }) }} kg</strong>
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
                    <h3>🛢️ Insumos Consumidos ({{ nombreMesSeleccionado }})</h3>
                    <div class="area-grafico" v-if="topMateriales.length > 0">
                        <Bar :data="chartDataMateriales" :options="{ indexAxis: 'y', responsive: true, maintainAspectRatio: false, plugins: { legend: { display: false } } }" />
                    </div>
                    <p class="sin-datos" v-else>Sin consumos en este mes.</p>
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

/* ESTILO MERCADO PAGO */
.selector-mes-app { display: flex; align-items: center; background: white; border-radius: 25px; padding: 5px 15px; box-shadow: 0 2px 8px rgba(0,0,0,0.1); border: 1px solid #eee; }
.btn-flecha { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #3498db; font-weight: bold; padding: 0 15px; transition: transform 0.2s; }
.btn-flecha:hover:not(.disabled) { transform: scale(1.2); }
.btn-flecha.disabled { color: #ccc; cursor: not-allowed; }
.etiqueta-mes { font-size: 1.1rem; font-weight: bold; color: #2c3e50; min-width: 140px; text-align: center; }

.btn-refresh { background: white; border: 1px solid #ccc; padding: 8px 12px; border-radius: 50%; cursor: pointer; color: #34495e; transition: background 0.2s;}
.btn-refresh:hover { background: #eef2f5; }

.fila-kpis { display: flex; gap: 20px; margin-bottom: 30px; flex-wrap: wrap; }
.card-kpi { flex: 1; min-width: 280px; background: white; padding: 25px; border-radius: 15px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); display: flex; align-items: center; border-left: 6px solid #ccc; }
.card-kpi .icono { font-size: 3rem; margin-right: 20px; }
.card-kpi .info { display: flex; flex-direction: column; }
.card-kpi .titulo { font-size: 0.9rem; color: #7f8c8d; text-transform: uppercase; font-weight: bold; margin-bottom: 5px; }
.card-kpi .valor { font-size: 2rem; font-weight: 900; color: #2c3e50; letter-spacing: -0.5px; }

.azul { border-left-color: #3498db; }
.verde { border-left-color: #2ecc71; }
.rojo { border-left-color: #e74c3c; }

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
}
</style>