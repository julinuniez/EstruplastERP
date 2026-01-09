<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import axios from 'axios'

const pestanaActiva = ref('ingreso'); // 'ingreso' | 'proceso'
const clientes = ref<any[]>([]);
const mensaje = ref('');
const error = ref('');
const apiUrl = import.meta.env.VITE_API_URL || '/api'; 
const getAuthConfig = () => ({ headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } });

// Formularios
const formIngreso = ref({ clienteId: '', kilos: 0, remito: '' });
const formProceso = ref({ clienteId: '', kilosEntrada: 0, kilosSalida: 0 });

// Merma en vivo para la pestaña de proceso
const mermaCalculada = computed(() => {
    const ent = formProceso.value.kilosEntrada;
    const sal = formProceso.value.kilosSalida;
    if (ent > 0 && sal > 0) {
        const dif = ent - sal;
        const porc = (dif / ent) * 100;
        return { kilos: dif.toFixed(2), porc: porc.toFixed(1) };
    }
    return { kilos: '0', porc: '0' };
});

onMounted(async () => {
    try {
        const res = await axios.get(`${apiUrl}/Clientes`, getAuthConfig());
        clientes.value = res.data;
    } catch (e) { console.error(e); }
});

async function guardarIngreso() {
    if (!formIngreso.value.clienteId || formIngreso.value.kilos <= 0) return alert("Faltan datos");
    try {
        await axios.post(`${apiUrl}/Recuperacion/ingreso-scrap`, {
            clienteId: formIngreso.value.clienteId,
            kilos: Number(formIngreso.value.kilos),
            remito: formIngreso.value.remito
        }, getAuthConfig());
        mensaje.value = "✅ Ingreso de Scrap registrado correctamente.";
        formIngreso.value.kilos = 0; formIngreso.value.remito = '';
        setTimeout(() => mensaje.value = '', 3000);
    } catch (e: any) { alert(e.response?.data || "Error"); }
}

async function guardarProceso() {
    if (formProceso.value.kilosSalida > formProceso.value.kilosEntrada) return alert("Error: Salida mayor a entrada");
    try {
        await axios.post(`${apiUrl}/Recuperacion/procesar-scrap`, {
            clienteId: formProceso.value.clienteId,
            kilosEntrada: Number(formProceso.value.kilosEntrada),
            kilosSalida: Number(formProceso.value.kilosSalida)
        }, getAuthConfig());
        mensaje.value = "♻️ Peletizado registrado. Stock limpio disponible.";
        formProceso.value.kilosEntrada = 0; formProceso.value.kilosSalida = 0;
        setTimeout(() => mensaje.value = '', 3000);
    } catch (e: any) { alert(e.response?.data || "Error"); }
}
</script>

<template>
    <div class="scrap-container">
        <h2>♻️ Gestión de Recuperado (Scrap)</h2>

        <div class="tabs">
            <button :class="{ active: pestanaActiva === 'ingreso' }" @click="pestanaActiva = 'ingreso'">📥 1. Recepción (Balanza)</button>
            <button :class="{ active: pestanaActiva === 'proceso' }" @click="pestanaActiva = 'proceso'">⚙️ 2. Peletizado (Máquina)</button>
        </div>

        <div class="panel-contenido">
            <div v-if="pestanaActiva === 'ingreso'" class="form-seccion">
                <h3>Llegada de Material Sucio</h3>
                <label>Cliente Dueño:</label>
                <select v-model="formIngreso.clienteId">
                    <option disabled value="">Seleccionar...</option>
                    <option v-for="c in clientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                </select>

                <div class="fila">
                    <div><label>Kilos (Balanza):</label><input type="number" v-model="formIngreso.kilos"></div>
                    <div><label>N° Remito Cliente:</label><input type="text" v-model="formIngreso.remito"></div>
                </div>

                <button class="btn-accion verde" @click="guardarIngreso">📥 INGRESAR A DEPÓSITO SUCIO</button>
            </div>

            <div v-if="pestanaActiva === 'proceso'" class="form-seccion">
                <h3>Transformación (Sucio -> Limpio)</h3>
                <label>Cliente:</label>
                <select v-model="formProceso.clienteId">
                    <option disabled value="">Seleccionar...</option>
                    <option v-for="c in clientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                </select>

                <div class="maquina-visual">
                    <div class="caja in">
                        <span>ENTRADA (Sucio)</span>
                        <input type="number" v-model="formProceso.kilosEntrada" placeholder="kg">
                    </div>
                    <div class="flecha">➡️ ⚙️ ➡️</div>
                    <div class="caja out">
                        <span>SALIDA (Limpio)</span>
                        <input type="number" v-model="formProceso.kilosSalida" placeholder="kg">
                    </div>
                </div>

                <div class="resumen-merma">
                    🔥 Desperdicio del proceso: <strong>{{ mermaCalculada.kilos }} kg</strong> ({{ mermaCalculada.porc }}%)
                </div>

                <button class="btn-accion azul" @click="guardarProceso">♻️ REGISTRAR PRODUCCIÓN RECUPERADO</button>
            </div>
        </div>

        <div v-if="mensaje" class="toast">{{ mensaje }}</div>
    </div>
</template>

<style scoped>
.scrap-container { max-width: 600px; margin: 0 auto; font-family: 'Segoe UI', sans-serif; }
.tabs { display: flex; margin-bottom: 20px; }
.tabs button { flex: 1; padding: 15px; border: none; background: #eee; cursor: pointer; font-weight: bold; font-size: 1rem; }
.tabs button.active { background: #34495e; color: white; border-bottom: 4px solid #2ecc71; }

.panel-contenido { background: white; padding: 25px; border: 1px solid #ddd; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.form-seccion h3 { margin-top: 0; color: #555; border-bottom: 1px solid #eee; padding-bottom: 10px; }
label { display: block; margin-top: 10px; font-weight: 600; color: #666; }
select, input { width: 100%; padding: 10px; margin-top: 5px; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; }
.fila { display: flex; gap: 15px; } .fila div { flex: 1; }

.maquina-visual { display: flex; align-items: center; justify-content: space-between; margin: 20px 0; background: #f8f9fa; padding: 20px; border-radius: 10px; }
.caja { display: flex; flex-direction: column; width: 40%; }
.caja input { font-size: 1.2rem; text-align: center; font-weight: bold; }
.caja.in input { border: 2px solid #e74c3c; color: #e74c3c; }
.caja.out input { border: 2px solid #2ecc71; color: #2ecc71; }
.flecha { font-size: 1.5rem; color: #555; }

.resumen-merma { text-align: center; margin-bottom: 20px; color: #e67e22; background: #fff3e0; padding: 10px; border-radius: 5px; }

.btn-accion { width: 100%; padding: 15px; border: none; border-radius: 5px; color: white; font-weight: bold; font-size: 1rem; cursor: pointer; margin-top: 15px; }
.verde { background: #27ae60; } .verde:hover { background: #219150; }
.azul { background: #2980b9; } .azul:hover { background: #21618c; }

.toast { position: fixed; bottom: 20px; right: 20px; background: #2ecc71; color: white; padding: 15px 25px; border-radius: 5px; box-shadow: 0 4px 10px rgba(0,0,0,0.2); animation: fadeIn 0.5s; }
@keyframes fadeIn { from { opacity: 0; transform: translateY(20px); } to { opacity: 1; transform: translateY(0); } }
</style>