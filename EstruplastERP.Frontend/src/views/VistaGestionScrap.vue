<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import axios from 'axios';
import { useRouter } from 'vue-router';

const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

const clientes = ref<any[]>([]);
const listaScrap = ref<any[]>([]);
const procesando = ref(false);
const mensaje = ref('');
const error = ref('');
const errorConexion = ref('');

const form = ref({
    clienteId: '',
    scrapId: '',
    kilosEntrada: 0, 
    kilosSalida: 0   
});

const scrapSeleccionado = computed(() => {
    return listaScrap.value.find(p => p.id === form.value.scrapId);
});

const mermaCalculada = computed(() => {
    if (form.value.kilosEntrada <= 0) return 0;
    const desperdicio = form.value.kilosEntrada - form.value.kilosSalida;
    return desperdicio > 0 ? Number(desperdicio.toFixed(2)) : 0;
});

const porcentajeMerma = computed(() => {
    if (form.value.kilosEntrada <= 0) return 0;
    return ((mermaCalculada.value / form.value.kilosEntrada) * 100).toFixed(1);
});

onMounted(async () => {
    await cargarClientes();
});

const cargarClientes = async () => {
    try {
        const res = await axios.get(`${apiUrl}/Clientes`, {
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });
        if (Array.isArray(res.data)) {
            clientes.value = res.data;
            errorConexion.value = ''; 
        }
    } catch (e: any) {
        if (e.code === "ERR_NETWORK" || e.response?.status >= 500) {
            errorConexion.value = "⛔ ERROR CRÍTICO: No se puede conectar con la Base de Datos.";
        } else {
            error.value = "Error al cargar datos iniciales.";
        }
    }
};

watch(() => form.value.clienteId, async (nuevoId) => {
    form.value.scrapId = ''; 
    form.value.kilosEntrada = 0;
    form.value.kilosSalida = 0;
    mensaje.value = '';
    error.value = '';

    if (!nuevoId) {
        listaScrap.value = [];
        return;
    }

    try {
        const res = await axios.get(`${apiUrl}/Productos`, {
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });
        
        if (Array.isArray(res.data)) {
            listaScrap.value = res.data.filter((p: any) => 
                p.clienteId === Number(nuevoId) && 
                p.esScrap === true &&
                p.stockActual > 0
            );
        }
    } catch (e) {
        console.error(e);
        error.value = "Error al cargar el stock de scrap.";
    }
});

const confirmarTransformacion = async () => {
    if (!form.value.clienteId || !form.value.scrapId) {
        alert("Selecciona Cliente y Material.");
        return;
    }
    if (form.value.kilosEntrada <= 0 || form.value.kilosSalida <= 0) {
        alert("Ingresa los kilos de Entrada y Salida.");
        return;
    }
    if (form.value.kilosSalida > form.value.kilosEntrada) {
        alert("❌ Error físico: No puede salir más material del que entra.");
        return;
    }
    if (scrapSeleccionado.value && form.value.kilosEntrada > scrapSeleccionado.value.stockActual) {
        alert(`❌ Error: Solo tienes ${scrapSeleccionado.value.stockActual} kg disponibles.`);
        return;
    }

    try {
        procesando.value = true;
        error.value = '';
        mensaje.value = '';

        const payload = {
            ClienteId: Number(form.value.clienteId),
            ProductoScrapId: Number(form.value.scrapId),
            KilosEntrada: Number(form.value.kilosEntrada),
            KilosObtenidos: Number(form.value.kilosSalida)
        };

        await axios.post(`${apiUrl}/Produccion/transformar-scrap`, payload, {
            headers: { 'Authorization': `Bearer ${localStorage.getItem('token')}` }
        });
        
        mensaje.value = `✅ Transformación Exitosa. Desperdicio: ${mermaCalculada.value} kg.`;
        
        form.value.kilosEntrada = 0;
        form.value.kilosSalida = 0;
        
        // Refrescar lista de scrap simulando cambio
        const idActual = form.value.clienteId;
        form.value.clienteId = ''; 
        setTimeout(() => form.value.clienteId = idActual, 10);

    } catch (e: any) {
        const errorMsg = e.response?.data?.mensaje || e.response?.data || "Ocurrió un error en la transformación.";
        error.value = typeof errorMsg === 'object' ? JSON.stringify(errorMsg) : errorMsg;
    } finally {
        procesando.value = false;
    }
};
</script>

<template>
    <div class="container-scrap">
        
        <div v-if="errorConexion" class="pantalla-error">
            <div class="box-error-grave">
                <h3>⚠️ SIN CONEXIÓN</h3>
                <p>{{ errorConexion }}</p>
                <button @click="cargarClientes" class="btn-retry">Reintentar Conexión</button>
            </div>
        </div>

        <div v-else class="contenido-real">
            <div class="card-proceso">
                <div class="header-card">
                    <h2>♻️ Gestión de Recuperado</h2>
                    <p>Transformación de Scrap (Sucio) a Materia Prima (Limpia)</p>
                </div>

                <div class="form-group">
                    <label>1️⃣ Cliente Dueño del Material</label>
                    <select v-model="form.clienteId">
                        <option value="">-- Seleccionar Cliente --</option>
                        <option v-for="c in clientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
                    </select>
                </div>

                <div class="form-group" v-if="form.clienteId">
                    <label>2️⃣ Lote de Scrap a Procesar</label>
                    <select v-model="form.scrapId" :disabled="listaScrap.length === 0">
                        <option value="">
                            {{ listaScrap.length > 0 ? '-- Seleccionar Scrap --' : '(Este cliente no tiene scrap disponible)' }}
                        </option>
                        <option v-for="s in listaScrap" :key="s.id" :value="s.id">
                            {{ s.nombre }} — (Disponible: {{ s.stockActual }} kg)
                        </option>
                    </select>
                </div>

                <div class="stock-info" v-if="scrapSeleccionado">
                    <div class="dato"><span>Material:</span><strong>{{ scrapSeleccionado.tipoMaterial }}</strong></div>
                    <div class="dato"><span>Color:</span><strong>{{ scrapSeleccionado.color }}</strong></div>
                    <div class="dato stock"><span>Stock Actual:</span><strong>{{ scrapSeleccionado.stockActual }} kg</strong></div>
                </div>

                <div class="row-kilos" v-if="form.scrapId">
                    <div class="col-input">
                        <label>📥 Entrada (Sucio)</label>
                        <input type="number" v-model="form.kilosEntrada" placeholder="kg" class="input-big">
                    </div>
                    <div class="flecha">➡️</div>
                    <div class="col-input">
                        <label>📤 Salida (Limpio)</label>
                        <input type="number" v-model="form.kilosSalida" placeholder="kg" class="input-big success-border">
                    </div>
                </div>

                <div class="info-merma" v-if="form.kilosEntrada > 0">
                    🗑️ Desperdicio: <strong>{{ mermaCalculada }} kg</strong> ({{ porcentajeMerma }}%)
                </div>

                <div class="actions" v-if="form.kilosSalida > 0">
                    <button class="btn-transformar" @click="confirmarTransformacion" :disabled="procesando">
                        <span v-if="procesando">⚙️ Procesando...</span>
                        <span v-else>🔄 Confirmar Transformación</span>
                    </button>
                </div>

                <div v-if="mensaje" class="msg success">{{ mensaje }}</div>
                <div v-if="error" class="msg error">{{ error }}</div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.container-scrap { 
    display: flex; 
    flex-direction: column;
    align-items: center;
    padding: 40px 20px; 
    background-color: #f4f6f8; 
    min-height: 80vh;
    width: 100%;
}

.contenido-real {
    width: 100%;
    display: flex;
    justify-content: center;
}

/* PANTALLA ERROR */
.pantalla-error {
    position: fixed;
    top: 0; left: 0; right: 0; bottom: 0;
    background: rgba(255,255,255,0.9);
    display: flex; justify-content: center; align-items: center;
    z-index: 9999;
}
.box-error-grave {
    background: white; border: 3px solid #c0392b;
    padding: 40px; text-align: center; border-radius: 10px;
    box-shadow: 0 10px 30px rgba(192, 57, 43, 0.2);
}
.box-error-grave h3 { color: #c0392b; font-size: 24px; margin-top: 0; }
.btn-retry { background: #c0392b; color: white; border: none; padding: 10px 20px; border-radius: 5px; cursor: pointer; margin-top: 15px; font-weight: bold; }

.card-proceso { 
    background: white; 
    padding: 30px; 
    border-radius: 12px; 
    box-shadow: 0 4px 20px rgba(0,0,0,0.08); 
    width: 100%; 
    max-width: 600px; 
}

.header-card { text-align: center; margin-bottom: 30px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
.header-card h2 { color: #27ae60; margin: 0 0 5px 0; }
.header-card p { color: #7f8c8d; margin: 0; font-size: 0.9rem; }

.form-group { margin-bottom: 20px; }
.form-group label { display: block; font-weight: bold; margin-bottom: 8px; color: #2c3e50; }

select, input { 
    width: 100%; 
    padding: 12px; 
    border: 1px solid #dcdcdc; 
    border-radius: 8px; 
    font-size: 1rem; 
    background-color: #fff;
    transition: border 0.3s;
}
select:focus, input:focus { border-color: #27ae60; outline: none; }

.stock-info { 
    background-color: #e8f8f5; 
    padding: 15px; 
    border-radius: 8px; 
    border-left: 5px solid #2ecc71; 
    margin-bottom: 20px; 
    display: flex; 
    justify-content: space-between;
}
.dato { display: flex; flex-direction: column; font-size: 0.9rem; }
.dato span { color: #7f8c8d; margin-bottom: 2px; }
.dato strong { color: #2c3e50; }
.dato.stock strong { color: #27ae60; font-size: 1.1rem; }

.row-kilos { display: flex; align-items: center; gap: 15px; margin: 25px 0; }
.col-input { flex: 1; }
.col-input label { display: block; font-weight: bold; font-size: 0.85rem; color: #555; margin-bottom: 5px; }
.input-big { text-align: center; font-size: 1.3rem; font-weight: bold; }
.success-border { border-color: #27ae60; background-color: #f0fff4; }
.flecha { font-size: 2rem; color: #bdc3c7; }
.info-merma { text-align: center; background: #fff3cd; color: #856404; padding: 10px; border-radius: 6px; margin-bottom: 20px; font-weight: bold; font-size: 0.95rem; border: 1px solid #ffeeba; }

.btn-transformar { 
    width: 100%; 
    padding: 15px; 
    background-color: #2c3e50; 
    color: white; 
    font-size: 1.1rem; 
    font-weight: bold; 
    border: none; 
    border-radius: 8px; 
    cursor: pointer; 
    transition: background 0.3s;
}
.btn-transformar:hover { background-color: #34495e; }
.btn-transformar:disabled { background-color: #95a5a6; cursor: not-allowed; }

.msg { margin-top: 20px; padding: 15px; border-radius: 8px; text-align: center; font-weight: 500; }
.msg.success { background-color: #d4edda; color: #155724; border: 1px solid #c3e6cb; }
.msg.error { background-color: #f8d7da; color: #721c24; border: 1px solid #f5c6cb; }
</style>