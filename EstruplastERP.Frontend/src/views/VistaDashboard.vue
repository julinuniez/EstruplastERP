<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
// Importamos el gráfico desde la carpeta de componentes
import GraficoBarras from '../components/GraficoBarras.vue'

// --- ESTADO ---
const resumenProduccion = ref<any[]>([]);
const topProductos = ref<any[]>([]);
const kpis = ref({ produccionMes: 0, pendientes: 0 });
const cargando = ref(true);
const error = ref('');

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api'; 
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

// --- CARGA DE DATOS ---
onMounted(async () => {
    try {
        cargando.value = true;
        // Hacemos las 3 peticiones en paralelo para optimizar la carga
        const [resResumen, resTop, resKpis] = await Promise.all([
            axios.get(`${apiUrl}/Estadisticas/resumen-mensual`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/top-productos`, getAuthConfig()),
            axios.get(`${apiUrl}/Estadisticas/resumen-kpis`, getAuthConfig())
        ]);
        
        resumenProduccion.value = resResumen.data;
        topProductos.value = resTop.data;
        kpis.value = resKpis.data;

    } catch (e: any) { 
        console.error(e);
        error.value = "No se pudieron cargar los datos del tablero.";
    } finally {
        cargando.value = false;
    }
});
</script>

<template>
    <div class="dashboard-container">
        <div class="header-dashboard">
            <h2>📊 Tablero de Control (BI)</h2>
            <span class="fecha-actual">{{ new Date().toLocaleDateString() }}</span>
        </div>

        <div v-if="cargando" class="loading">⏳ Cargando métricas...</div>
        <div v-else-if="error" class="error-msg">{{ error }}</div>

        <div v-else class="contenido-dashboard">
            
            <div class="fila-kpis">
                <div class="card-kpi azul">
                    <div class="icono">⚖️</div>
                    <div class="info">
                        <span class="titulo">Producción del Mes</span>
                        <strong class="valor">{{ kpis.produccionMes.toLocaleString() }} kg</strong>
                    </div>
                </div>

                <div class="card-kpi naranja">
                    <div class="icono">📋</div>
                    <div class="info">
                        <span class="titulo">Órdenes Pendientes</span>
                        <strong class="valor">{{ kpis.pendientes }}</strong>
                    </div>
                </div>
                
                <div class="card-kpi verde">
                    <div class="icono">📈</div>
                    <div class="info">
                        <span class="titulo">Eficiencia (Est.)</span>
                        <strong class="valor">98.5%</strong>
                    </div>
                </div>
            </div>

            <div class="grid-principal">
                <div class="card grafico-card">
                    <h3>🏆 Top 5 Productos Fabricados</h3>
                    <div class="area-grafico">
                        <GraficoBarras 
                            v-if="topProductos.length > 0"
                            titulo="Kilos Totales Históricos"
                            :etiquetas="topProductos.map(p => p.producto)"
                            :datos="topProductos.map(p => p.totalKilos)"
                            color="#3498db"
                        />
                        <p v-else class="sin-datos">No hay datos suficientes para graficar.</p>
                    </div>
                </div>

                <div class="card tabla-card">
                    <h3>📅 Evolución Mensual</h3>
                    <div class="tabla-responsive">
                        <table class="tabla-stats">
                            <thead>
                                <tr>
                                    <th>Periodo</th>
                                    <th style="text-align:center">Órdenes</th>
                                    <th style="text-align:right">Kilos Prod.</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="m in resumenProduccion" :key="m.periodo">
                                    <td>{{ m.periodo }}</td>
                                    <td style="text-align:center">
                                        <span class="badge">{{ m.cantidadOrdenes }}</span>
                                    </td>
                                    <td style="text-align:right; font-weight:bold; color: #27ae60;">
                                        {{ m.kilos.toFixed(2) }} kg
                                    </td>
                                </tr>
                                <tr v-if="resumenProduccion.length === 0">
                                    <td colspan="3" class="sin-datos">Sin historial reciente.</td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

        </div>
    </div>
</template>

<style scoped>
.dashboard-container { padding: 20px; font-family: 'Segoe UI', sans-serif; background-color: #f4f6f9; min-height: 100vh; }

.header-dashboard { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; }
.header-dashboard h2 { margin: 0; color: #2c3e50; border-bottom: 3px solid #3498db; padding-bottom: 5px; }
.fecha-actual { font-weight: bold; color: #7f8c8d; }

/* KPIs */
.fila-kpis { display: flex; gap: 20px; margin-bottom: 30px; flex-wrap: wrap; }
.card-kpi { flex: 1; min-width: 200px; background: white; padding: 20px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); display: flex; align-items: center; transition: transform 0.2s; }
.card-kpi:hover { transform: translateY(-5px); }
.card-kpi .icono { font-size: 2.5rem; margin-right: 20px; }
.card-kpi .info { display: flex; flex-direction: column; }
.card-kpi .titulo { font-size: 0.9rem; color: #7f8c8d; text-transform: uppercase; letter-spacing: 1px; font-weight: bold; }
.card-kpi .valor { font-size: 1.8rem; font-weight: 800; color: #2c3e50; }

.azul { border-left: 5px solid #3498db; }
.naranja { border-left: 5px solid #e67e22; }
.verde { border-left: 5px solid #2ecc71; }

/* Grid Principal */
.grid-principal { display: grid; grid-template-columns: repeat(auto-fit, minmax(450px, 1fr)); gap: 25px; }

.card { background: white; padding: 25px; border-radius: 12px; box-shadow: 0 4px 15px rgba(0,0,0,0.05); border: 1px solid #edf2f7; }
.card h3 { margin-top: 0; color: #2c3e50; font-size: 1.2rem; border-bottom: 1px solid #eee; padding-bottom: 15px; margin-bottom: 20px; }

.area-grafico { min-height: 300px; }

/* Tabla */
.tabla-responsive { overflow-x: auto; }
.tabla-stats { width: 100%; border-collapse: collapse; }
.tabla-stats th { text-align: left; padding: 12px; background-color: #f8f9fa; color: #666; font-size: 0.9rem; border-bottom: 2px solid #eee; }
.tabla-stats td { padding: 12px; border-bottom: 1px solid #eee; color: #333; }
.badge { background-color: #fff3e0; color: #d35400; padding: 4px 10px; border-radius: 20px; font-weight: bold; font-size: 0.85rem; }

.sin-datos { text-align: center; color: #aaa; font-style: italic; padding: 20px; }
.loading { text-align: center; font-size: 1.2rem; color: #3498db; margin-top: 50px; }
.error-msg { text-align: center; color: #e74c3c; background: #fee; padding: 20px; border-radius: 8px; margin-top: 20px; }

@media (max-width: 768px) {
    .grid-principal { grid-template-columns: 1fr; }
    .fila-kpis { flex-direction: column; }
}
</style>