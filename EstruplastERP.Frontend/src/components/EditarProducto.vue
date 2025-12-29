<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';

const route = useRoute();
const router = useRouter();
const loading = ref(true);
const guardando = ref(false);

// Listas de datos
const listaMateriasPrimas = ref([]); 

// Objeto del producto principal
const producto = ref({
    id: 0,
    nombre: '',
    codigoSku: '',
    pesoEspecifico: 1.0,
    stockMinimo: 0,
    esMateriaPrima: false,
    esProductoTerminado: false,
    esFazon: false,
    receta: [] 
});

// Variables para agregar ingrediente
const ingredienteSeleccionado = ref('');
const cantidadIngrediente = ref('');

const getConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- COMPUTED: CALCULAR TOTAL PORCENTAJE ---
const totalPorcentaje = computed(() => {
    if (!producto.value.receta) return 0;
    // Sumamos y redondeamos a 2 decimales para evitar problemas de coma flotante (99.99999)
    const suma = producto.value.receta.reduce((acc, item) => acc + Number(item.cantidad), 0);
    return Math.round(suma * 100) / 100;
});

// Validamos si está listo para guardar (Solo si es Prod Terminado requerimos 100%)
const puedeGuardar = computed(() => {
    if (guardando.value) return false;
    if (producto.value.esProductoTerminado) {
        return totalPorcentaje.value === 100;
    }
    return true; // Si es materia prima, no valida receta
});

// 1. Cargar datos al entrar
onMounted(async () => {
    const id = route.params.id;
    if (!id) return router.push('/stock'); 

    try {
        const resProd = await axios.get(`https://localhost:7244/api/Productos/${id}`, getConfig());
        producto.value = resProd.data;
        if (!producto.value.receta) producto.value.receta = [];

        const resMP = await axios.get('https://localhost:7244/api/Productos/materias-primas', getConfig());
        listaMateriasPrimas.value = resMP.data.filter(mp => mp.id !== Number(id));

    } catch (e) {
        alert("Error al cargar datos: " + e.message);
        router.push('/stock');
    } finally {
        loading.value = false;
    }
});

// --- LÓGICA DE RECETA (PORCENTAJES) ---

const agregarIngrediente = () => {
    if (!ingredienteSeleccionado.value) return alert("Seleccione una materia prima.");
    
    const cantidad = Number(cantidadIngrediente.value);
    if (!cantidad || cantidad <= 0) return alert("Ingrese un porcentaje válido.");

    // Validar que no se pase del 100%
    if (totalPorcentaje.value + cantidad > 100) {
        return alert(`⚠️ No puedes agregar ${cantidad}%. El total superaría el 100% (Actual: ${totalPorcentaje.value}%).`);
    }

    const mpInfo = listaMateriasPrimas.value.find(m => m.id === ingredienteSeleccionado.value);

    const existe = producto.value.receta.find(r => r.materiaPrimaId === ingredienteSeleccionado.value);
    if (existe) {
        alert("Esta materia prima ya está en la receta.");
        return;
    }

    producto.value.receta.push({
        materiaPrimaId: ingredienteSeleccionado.value,
        nombreInsumo: mpInfo.nombre,
        cantidad: cantidad
    });

    ingredienteSeleccionado.value = '';
    cantidadIngrediente.value = '';
};

const quitarIngrediente = (index) => {
    producto.value.receta.splice(index, 1);
};

// --- GUARDADO ---

const guardarConfiguracion = async () => {
    // Validación final de seguridad
    if (producto.value.esProductoTerminado && totalPorcentaje.value !== 100) {
        return alert(`⚠️ La receta debe sumar exactamente 100%. Actual: ${totalPorcentaje.value}%`);
    }

    guardando.value = true;
    try {
        const payload = {
            stockMinimo: Number(producto.value.stockMinimo),
            pesoEspecifico: Number(producto.value.pesoEspecifico),
            // Enviamos los valores actuales (ya no se editan en pantalla, pero el backend los necesita)
            esMateriaPrima: producto.value.esMateriaPrima,
            esProductoTerminado: producto.value.esProductoTerminado,
            esFazon: producto.value.esFazon,
            
            receta: producto.value.receta.map(item => ({
                materiaPrimaId: item.materiaPrimaId,
                cantidad: Number(item.cantidad)
            }))
        };

        await axios.put(`https://localhost:7244/api/Productos/configurar/${producto.value.id}`, payload, getConfig());
        
        alert("✅ Configuración guardada correctamente.");
        router.back();
        
    } catch (e) {
        alert("Error al guardar: " + (e.response?.data || e.message));
    } finally {
        guardando.value = false;
    }
};

const volver = () => {
    if (window.history.length > 1) {
        router.back(); 
    } else {
        router.push('/'); 
    }
};
</script>

<template>
    <div class="container-edit">
        <div v-if="loading" class="loading">Cargando datos...</div>
        
        <div v-else class="card-edit">
            <div class="header">
                <h2>⚙️ Configuración de Producto</h2>
                <p class="subtitle">{{ producto.nombre }}</p>
                <div class="badges-header">
                    <span v-if="producto.esProductoTerminado" class="badge-tipo pt">📦 Producto Terminado</span>
                    <span v-else-if="producto.esMateriaPrima" class="badge-tipo mp">🏭 Materia Prima</span>
                    <span v-if="producto.esFazon" class="badge-tipo faz">🤝 Fazon</span>
                </div>
            </div>

            <div class="seccion-box">
                <h4>📊 Parámetros Técnicos</h4>
                <div class="grid-2">
                    <div class="campo">
                        <label>SKU (Flexxus)</label>
                        <input type="text" v-model="producto.codigoSku" disabled class="input-readonly">
                    </div>
                    <div class="campo">
                        <label>Stock Mínimo (Alerta)</label>
                        <input type="number" v-model.number="producto.stockMinimo" placeholder="Ej: 100">
                    </div>
                </div>
                <div class="campo mt-2">
                    <label>🧪 Peso Específico (g/cm³)</label>
                    <div class="input-group">
                        <input type="number" v-model.number="producto.pesoEspecifico" step="0.0001">
                        <span class="unit">g/cm³</span>
                    </div>
                    <small>Usado para calcular rendimiento.</small>
                </div>
            </div>

            <div v-if="producto.esProductoTerminado" class="seccion-box">
                <div class="header-receta">
                    <h4>📝 Fórmula (Porcentajes)</h4>
                    <div class="total-badge" :class="totalPorcentaje === 100 ? 'ok' : 'error'">
                        Total: {{ totalPorcentaje }}%
                    </div>
                </div>

                <div class="buscador-receta">
                    <select v-model="ingredienteSeleccionado" class="select-mp">
                        <option value="" disabled selected>🔍 Seleccionar Materia Prima...</option>
                        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">
                            {{ mp.codigoSku }} - {{ mp.nombre }}
                        </option>
                    </select>
                    <input type="number" v-model="cantidadIngrediente" placeholder="%" class="input-cant" min="0" max="100">
                    <button @click="agregarIngrediente" class="btn-add">➕</button>
                </div>

                <div class="tabla-receta-wrapper">
                    <table class="tabla-receta">
                        <thead>
                            <tr>
                                <th>Insumo</th>
                                <th width="100" class="text-center">Porcentaje</th>
                                <th width="40"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in producto.receta" :key="index">
                                <td>{{ item.nombreInsumo }}</td>
                                <td class="text-center font-bold">{{ item.cantidad }} %</td>
                                <td>
                                    <button @click="quitarIngrediente(index)" class="btn-x">×</button>
                                </td>
                            </tr>
                            <tr v-if="producto.receta.length === 0">
                                <td colspan="3" class="text-center text-muted">Aún no hay ingredientes. Agregue materias primas para completar el 100%.</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="footer-actions">
                <button @click="volver" class="btn-cancelar" :disabled="guardando">Cancelar</button>
                
                <button @click="guardarConfiguracion" class="btn-guardar" :disabled="!puedeGuardar">
                    <span v-if="guardando">Guardando...</span>
                    <span v-else-if="!puedeGuardar">⚠️ Complete el 100%</span>
                    <span v-else>💾 Guardar Todo</span>
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.container-edit { display: flex; justify-content: center; padding: 20px; background: #f4f6f8; min-height: 100vh; font-family: 'Segoe UI', sans-serif; }
.card-edit { 
    background: white; 
    width: 900px; /* Ancho corregido */
    max-width: 95vw;
    padding: 30px; 
    border-radius: 8px; 
    box-shadow: 0 4px 15px rgba(0,0,0,0.1); 
}

.header { text-align: center; margin-bottom: 20px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
.header h2 { margin: 0; color: #2c3e50; }
.subtitle { color: #7f8c8d; margin-top: 5px; font-weight: bold; font-size: 1.1em; }
.badges-header { margin-top: 10px; display: flex; justify-content: center; gap: 10px; }
.badge-tipo { padding: 4px 10px; border-radius: 12px; font-size: 0.85em; font-weight: bold; text-transform: uppercase; }
.badge-tipo.pt { background: #e8f5e9; color: #2e7d32; border: 1px solid #c8e6c9; }
.badge-tipo.mp { background: #fff3e0; color: #ef6c00; border: 1px solid #ffe0b2; }
.badge-tipo.faz { background: #f3e5f5; color: #7b1fa2; border: 1px solid #e1bee7; }

.seccion-box { background: #f8f9fa; border: 1px solid #e9ecef; border-radius: 6px; padding: 20px; margin-bottom: 20px; }
.seccion-box h4 { margin-top: 0; color: #3498db; margin-bottom: 15px; border-bottom: 1px solid #eee; padding-bottom: 5px; }

.grid-2 { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
.campo { display: flex; flex-direction: column; }
.campo label { font-weight: bold; margin-bottom: 5px; color: #555; font-size: 0.9em; }
.campo input { padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; }
.input-readonly { background: #e9ecef; color: #666; cursor: not-allowed; }
.mt-2 { margin-top: 15px; }

.input-group { position: relative; }
.input-group input { width: 100%; padding-right: 50px; box-sizing: border-box; }
.unit { position: absolute; right: 10px; top: 50%; transform: translateY(-50%); color: #999; }

/* ESTILOS RECETA */
.header-receta { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; }
.total-badge { font-weight: bold; padding: 5px 15px; border-radius: 20px; font-size: 0.9em; }
.total-badge.ok { background: #27ae60; color: white; }
.total-badge.error { background: #c0392b; color: white; animation: pulse 2s infinite; }

.buscador-receta { display: flex; gap: 10px; margin-bottom: 15px; background: white; padding: 10px; border: 1px solid #eee; border-radius: 6px; }
.select-mp { flex-grow: 1; padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; }
.input-cant { width: 90px; padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; text-align: center; }
.btn-add { background: #27ae60; color: white; border: none; border-radius: 4px; width: 50px; cursor: pointer; font-size: 1.4em; transition: background 0.2s; }
.btn-add:hover { background: #219150; }

.tabla-receta-wrapper { border: 1px solid #dee2e6; border-radius: 4px; overflow: hidden; background: white; }
.tabla-receta { width: 100%; border-collapse: collapse; }
.tabla-receta th { background: #34495e; color: white; padding: 12px; text-align: left; font-size: 0.9em; }
.tabla-receta td { padding: 10px 12px; border-bottom: 1px solid #f1f1f1; }
.btn-x { background: none; border: none; color: #c0392b; font-weight: bold; cursor: pointer; font-size: 1.4em; }
.btn-x:hover { color: #e74c3c; transform: scale(1.1); }

.footer-actions { display: flex; justify-content: flex-end; gap: 15px; border-top: 1px solid #eee; padding-top: 20px; }
.btn-cancelar { padding: 12px 25px; border: 1px solid #ccc; background: white; border-radius: 4px; cursor: pointer; font-weight: bold; color: #666; }
.btn-guardar { padding: 12px 30px; border: none; background: #3498db; color: white; border-radius: 4px; cursor: pointer; font-weight: bold; font-size: 1em; transition: background 0.3s; }
.btn-guardar:disabled { background: #bdc3c7; cursor: not-allowed; }
.btn-guardar:hover:not(:disabled) { background: #2980b9; }

.text-center { text-align: center; }
.text-muted { color: #999; font-style: italic; padding: 20px; }
.font-bold { font-weight: bold; color: #2c3e50; }

@keyframes pulse { 0% { opacity: 1; } 50% { opacity: 0.8; } 100% { opacity: 1; } }
</style>