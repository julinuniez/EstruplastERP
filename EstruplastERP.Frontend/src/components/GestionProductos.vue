<script setup lang="ts">
// ==========================================
// 1. IMPORTS
// ==========================================
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';
// @ts-ignore
import html2pdf from 'html2pdf.js'

// ==========================================
// 2. CONFIGURACIÓN
// ==========================================
const route = useRoute();
const router = useRouter();
const apiUrl = import.meta.env.VITE_API_URL;

// ==========================================
// 3. ESTADO (VARIABLES REACTIVAS)
// ==========================================
const form = ref({
    nombre: '',
    codigoSku: '',
    color: '',
    stockMinimo: 1000,
    precioCosto: 0,
    largo: 0, ancho: 0, espesor: 0, pesoEspecifico: 0,
    // El front espera: materiaPrimaId, nombreMP, cantidad
    receta: [] as { materiaPrimaId: number, nombreMP: string, cantidad: number }[]
});

const listaMateriasPrimas = ref<any[]>([]);
const listaProductosBase = ref<any[]>([]);
const productoBaseId = ref(''); 
const ingredienteTemp = ref({ id: '', cantidad: '' as string | number });
const mensaje = ref('');
const error = ref('');

// ==========================================
// 4. COMPUTED
// ==========================================
const esModoEdicion = computed(() => !!route.params.id);
const productoId = computed(() => Number(route.params.id) || 0);

const totalPorcentaje = computed(() => {
    if (form.value.receta.length === 0) return 0;
    const total = form.value.receta.reduce((acc, item) => acc + item.cantidad, 0);
    return parseFloat(total.toFixed(2));
});

const esRecetaValida = computed(() => {
    // Si estamos editando, NO validamos la receta (siempre es true)
    if (esModoEdicion.value) return true;

    // Si es producto nuevo, validamos REVENTA o 100%
    const esReventa = form.value.nombre.toUpperCase().includes('PET') || form.value.nombre.toUpperCase().includes('REVENTA');
    if (esReventa) return true;
    
    return totalPorcentaje.value === 100;
});

// ==========================================
// 5. FUNCIONES AUXILIARES
// ==========================================
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

const editarProducto = (id: number) => {
    router.push({ name: 'EditarProducto', params: { id: id } })
}

function buscarNombreMateriaPrima(id: number) {
    const mp = listaMateriasPrimas.value.find(m => m.id == id);
    return mp ? mp.nombre : '(MP Desconocida)';
}

function limpiarFormulario() {
    form.value = {
        nombre: '',
        codigoSku: '',
        color: '',
        stockMinimo: 1000,
        precioCosto: 0,
        largo: 0, ancho: 0, espesor: 0, pesoEspecifico: 0,
        receta: []
    };
    productoBaseId.value = '';
    mensaje.value = '';
    error.value = '';
}

// ==========================================
// 6. LÓGICA DE CARGA (AQUÍ ESTÁ LA SOLUCIÓN) 🛠️
// ==========================================

async function cargarDatosEdicion(id: number, config: any) {
    try {
        error.value = ''; 
        console.log(`📡 Solicitando datos al Backend para ID: ${id}`);

        const res = await axios.get(`${apiUrl}/Productos/${id}`, config);
        const data = res.data; // Aquí llega tu ProductoDetalleDto

        console.log("📦 JSON Recibido del Backend:", data);

        // 1. Mapeo de datos básicos
        form.value.nombre = data.nombre || data.Nombre || '';
        form.value.codigoSku = data.codigoSku || data.CodigoSku || '';
        form.value.color = data.color || data.Color || '';
        form.value.stockMinimo = data.stockMinimo ?? data.StockMinimo ?? 1000;
        form.value.precioCosto = data.precioCosto || data.PrecioCosto || 0;
        
        // Dimensiones (si las usas)
        form.value.largo = data.largo || 0;
        form.value.ancho = data.ancho || 0;
        form.value.espesor = data.espesor || 0;
        form.value.pesoEspecifico = data.pesoEspecifico || 0;

        // 🛠️ 2. MAPEO DE LA RECETA (LA PARTE CRÍTICA)
        // Buscamos 'receta' (minúscula) porque es lo estándar en JSON, o 'Receta' por si acaso.
        const listaIngredientes = data.receta || data.Receta || [];

        console.log(`🧪 Procesando ${listaIngredientes.length} ingredientes de la BD...`);

        form.value.receta = listaIngredientes.map((item: any) => {
            // BACKEND (DTO) vs FRONTEND (Form)
            // item.materiaPrimaId  -> materiaPrimaId
            // item.nombreInsumo    -> nombreMP  <--- AQUÍ OCURRE LA MAGIA
            // item.cantidad        -> cantidad

            const mpId = item.materiaPrimaId || item.MateriaPrimaId;
            
            // Prioridad: 1. Nombre que viene de la BD (Join). 2. Buscar en lista local.
            let nombreFinal = item.nombreInsumo || item.NombreInsumo;
            
            if (!nombreFinal) {
                nombreFinal = buscarNombreMateriaPrima(mpId);
            }

            return {
                materiaPrimaId: Number(mpId),
                nombreMP: nombreFinal,
                cantidad: Number(item.cantidad || item.Cantidad)
            };
        });

    } catch (e: any) {
        console.error("❌ Error cargando edición:", e);
        error.value = "Error al cargar los datos. Revisa la consola.";
    }
}

// ==========================================
// 7. OTRAS FUNCIONES (COPIAR, GUARDAR, AGREGAR)
// ==========================================
async function copiarDatosDeProducto() {
    if (!productoBaseId.value) return;
    try {
        error.value = '';
        const res = await axios.get(`${apiUrl}/Productos/${productoBaseId.value}`, getAuthConfig());
        const data = res.data;
        
        form.value.nombre = `${data.nombre || data.Nombre} (Copia)`;
        form.value.codigoSku = "";
        form.value.color = data.color || '';
        form.value.stockMinimo = data.stockMinimo ?? 1000;

        // Misma lógica de mapeo para la copia
        const formulasBackend = data.receta || data.Receta || data.formulas || [];
        
        form.value.receta = formulasBackend.map((f: any) => {
            const idMP = f.materiaPrimaId || f.MateriaPrimaId;
            // En copia quizás no venga nombreInsumo, buscamos local
            const nombre = f.nombreInsumo || buscarNombreMateriaPrima(idMP); 
            
            return {
                materiaPrimaId: Number(idMP),
                nombreMP: nombre,
                cantidad: Number(f.cantidad || f.Cantidad)
            };
        });
    } catch (e: any) { error.value = "Error al copiar producto."; }
}

function agregarIngrediente() {
    const id = ingredienteTemp.value.id;
    const porc = parseFloat(ingredienteTemp.value.cantidad.toString());
    
    if (!id || porc <= 0) return;
    if (totalPorcentaje.value + porc > 100) {
        alert(`❌ Excede el 100%. (Disponible: ${(100 - totalPorcentaje.value).toFixed(2)}%)`);
        return;
    }

    const mp = listaMateriasPrimas.value.find(m => m.id == id);
    const existe = form.value.receta.find(r => r.materiaPrimaId === Number(id));

    if (existe) {
        existe.cantidad += porc;
    } else {
        form.value.receta.push({
            materiaPrimaId: Number(id),
            nombreMP: mp ? mp.nombre : '?',
            cantidad: porc
        })
    }
    ingredienteTemp.value.id = ''; ingredienteTemp.value.cantidad = '';
}

function quitarIngrediente(index: number) { 
    form.value.receta.splice(index, 1);
}

async function guardarProducto() {
    mensaje.value = ''; error.value = '';
    
    if (!form.value.nombre || !form.value.codigoSku) {
        error.value = "Faltan datos obligatorios"; return;
    }

    const esReventa = form.value.nombre.toUpperCase().includes('PET') || form.value.nombre.toUpperCase().includes('REVENTA');

    // --- CAMBIO AQUÍ: Solo validamos receta si es CREACIÓN (No Edición) ---
    if (!esModoEdicion.value && !esReventa) {
        if (form.value.receta.length === 0) {
            error.value = "⛔ Este material requiere una fórmula."; return;
        }
        if (totalPorcentaje.value !== 100) {
            error.value = `⛔ Error: La fórmula suma ${totalPorcentaje.value}%. Debe sumar 100%.`; return;
        }
    }

    const url = esModoEdicion.value
        ? `${apiUrl}/Productos/actualizar/${productoId.value}`
        : `${apiUrl}/Productos/crear`;
        
    const payload: any = {
        ...form.value,
        esProductoTerminado: true,
        esMateriaPrima: false,
        largo: 0, ancho: 0, espesor: 0, pesoEspecifico: 0
    };
    if(esModoEdicion.value) {
        delete payload.receta;
    }

    try {
        if (esModoEdicion.value) await axios.put(url, payload, getAuthConfig());
        else await axios.post(url, payload, getAuthConfig());
        
        mensaje.value = "✅ Guardado correctamente";
        
        if (!esModoEdicion.value) {
            limpiarFormulario();
            const res = await axios.get(`${apiUrl}/Productos`, getAuthConfig());
            listaProductosBase.value = res.data.filter((p: any) => p.esProductoTerminado);
        }
    } catch (e: any) {
        error.value = "❌ " + (e.response?.data?.mensaje || e.message);
    }
}

// ==========================================
// 8. WATCHERS & LIFECYCLE 🛠️
// ==========================================

// Este Watch arregla el problema de "tengo que recargar la página"
watch(
    () => route.params.id,
    async (newId) => {
        if (newId) {
            await cargarDatosEdicion(Number(newId), getAuthConfig());
        } else {
            limpiarFormulario();
        }
    }
);

onMounted(async () => {
    try {
        const token = localStorage.getItem('token');
        if (!token) {
            error.value = "No hay sesión activa.";
            return;
        }
        
        const config = { headers: { Authorization: `Bearer ${token}` } };
        
        // 1. Cargar Maestros
        const [resMP, resProd] = await Promise.all([
            axios.get(`${apiUrl}/Productos/materias-primas`, config),
            axios.get(`${apiUrl}/Productos`, config)
        ]);
        
        listaMateriasPrimas.value = resMP.data;
        listaProductosBase.value = resProd.data.filter((p: any) => p.esProductoTerminado);

        // 2. Si entraste directo a editar (por URL), carga el producto
        if (esModoEdicion.value && productoId.value) {
            await cargarDatosEdicion(productoId.value, config);
        }
    } catch (e: any) {
        if (e.response && e.response.status === 401) {
            error.value = "Sesión expirada.";
        } else {
            error.value = "Error de conexión o datos.";
        }
    }
});
</script>

<template>
    <div class="contenedor-principal" id="hoja-gestion">
        <div class="header-action">
            <h2>{{ esModoEdicion ? '✏️ Editar Material' : '📦 Nuevo Material de Venta' }}</h2>
            <div v-if="!esModoEdicion" class="selector-copia" data-html2canvas-ignore="true">
                <label>Usar Plantilla:</label>
                <select v-model="productoBaseId" @change="copiarDatosDeProducto">
                    <option value="">-- Seleccionar --</option>
                    <option v-for="p in listaProductosBase" :key="p.id" :value="p.id">
                        {{ p.nombre }}
                    </option>
                </select>
            </div>
        </div>

        <div class="form-grid">
            <div class="card">
                <h3>Datos del Material / Mezcla</h3>
                <div class="campo">
                    <label>Nombre Comercial</label>
                    <input v-model="form.nombre" type="text" class="input-sin-borde-form" placeholder="Ej: Material PAI Blanco">
                </div>
                <div class="campo-row">
                    <div class="campo">
                        <label>SKU / Código</label>
                        <input v-model="form.codigoSku" type="text" class="input-sin-borde-form" placeholder="MAT-001">
                    </div>
                    <div class="campo">
                        <label>Color</label>
                        <input v-model="form.color" type="text" class="input-sin-borde-form" placeholder="Ej: Blanco">
                    </div>
                </div>
                <div class="campo">
                    <label>Stock Mínimo (Kg)</label>
                    <input v-model.number="form.stockMinimo" type="number" class="input-sin-borde-form">
                </div>
            </div>

<div class="card" :class="{ 'bloqueado': esModoEdicion }">
                <div class="card-header">
                    <h3>🧪 Fórmula de Producción (%)</h3>
                    <div class="indicador-total" :class="{'ok': totalPorcentaje===100, 'error': totalPorcentaje!==100}">
                        Total: {{ totalPorcentaje }}%
                    </div>
                </div>

                <div v-if="esModoEdicion" class="aviso-bloqueo">
                    <span>🔒 <strong>Fórmula Bloqueada:</strong> No se puede modificar la estructura en modo edición.</span>
                </div>

                <div v-else class="add-bar" data-html2canvas-ignore="true">
                    <select v-model="ingredienteTemp.id">
                        <option value="">-- Insumo --</option>
                        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">{{ mp.nombre }}</option>
                    </select>
                    
                    <input 
                        v-model.number="ingredienteTemp.cantidad" 
                        type="number" step="0.1" max="100" placeholder="%" 
                        class="input-cant" 
                        @keyup.enter="agregarIngrediente"
                    >
                    <button 
                        type="button" 
                        @click="agregarIngrediente" 
                        class="btn-icon" 
                        :disabled="totalPorcentaje >= 100"
                    >➕</button>
                </div>

                <div class="lista-items">
                    <div v-for="(item, index) in form.receta" :key="index" class="item-receta">
                        <div><strong>{{ item.nombreMP }}</strong></div>
                        <div class="acciones-item">
                            <input type="number" :value="item.cantidad" class="input-sin-borde" readonly style="width: 50px; text-align: right;"> %
                            
                            <button v-if="!esModoEdicion" @click="quitarIngrediente(index)" class="btn-trash" data-html2canvas-ignore="true">🗑️</button>
                        </div>
                    </div>
                    
                    <div v-if="form.receta.length === 0" class="vacio">
                        <span v-if="esModoEdicion">No hay fórmula visible o registrada.</span>
                        <span v-else>Sin ingredientes. Defina la composición.</span>
                    </div>
                </div>

                <div class="barra-fondo">
                    <div class="barra-progreso" :style="{width: totalPorcentaje + '%'}" :class="{'completo': totalPorcentaje===100}"></div>
                </div>
            </div>
        </div>

        <div class="footer-actions" data-html2canvas-ignore="true">
            <span v-if="mensaje" class="msg success">{{ mensaje }}</span>
            <span v-if="error" class="msg error">{{ error }}</span>
            <button @click="guardarProducto" class="btn-save" :disabled="!esRecetaValida && !form.nombre.toUpperCase().includes('PET')">
                {{ esModoEdicion ? 'Guardar Cambios' : 'Crear Material' }}
            </button>
        </div>

        <div class="lista-card" data-html2canvas-ignore="true">
            <h3>Materiales Activos (Venta por Kilo)</h3>
            <div class="scroll-tabla">
                <table>
                    <thead><tr><th>SKU</th><th>Material / Mezcla</th><th>Color</th><th>Acción</th></tr></thead>
                    <tbody>
                        <tr v-for="p in listaProductosBase" :key="p.id">
                            <td>{{ p.codigoSku }}</td>
                            <td>{{ p.nombre }}</td>
                            <td>{{ p.color || '-' }}</td>
                            <td>
                                <button @click="editarProducto(p.id)" class="btn-editar">✏️</button>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Estilos Base */
.contenedor-principal { max-width: 900px; margin: 0 auto; color: #333; font-family: sans-serif; padding-bottom: 50px; }
.header-action { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
.selector-copia { background: #f0f4f8; padding: 8px 15px; border-radius: 6px; display: flex; align-items: center; gap: 10px; }
.form-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-bottom: 20px; }
.card { background: white; padding: 20px; border-radius: 8px; border: 1px solid #e0e0e0; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
.card h3 { margin-top: 0; margin-bottom: 15px; color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 10px; }
.campo { margin-bottom: 15px; }
label { display: block; font-weight: 600; font-size: 0.85rem; margin-bottom: 5px; color: #555; }
input, select { width: 100%; padding: 8px 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }
.campo-row { display: flex; gap: 10px; }

/* Inputs sin borde para aspecto "limpio" */
.input-sin-borde { border: none; background: transparent; font-weight: bold; color: #333; font-size: 1em; padding: 0; outline: none; }
.input-sin-borde-form { border: 1px solid #ddd; border-radius: 4px; padding: 8px; width: 100%; }

/* Receta */
.card-header { display: flex; justify-content: space-between; align-items: center; }
.indicador-total { font-weight: bold; font-size: 0.9em; }
.indicador-total.ok { color: green; }
.indicador-total.error { color: #e74c3c; }
.add-bar { display: flex; gap: 5px; margin-bottom: 10px; }
.add-bar select { flex-grow: 1; }
.input-cant { width: 70px !important; text-align: center; }
.btn-icon { background: #27ae60; color: white; border: none; width: 40px; cursor: pointer; border-radius: 4px; font-weight: bold; font-size: 1.2rem; }
.lista-items { border: 1px solid #eee; border-radius: 4px; min-height: 100px; background: #fafafa; margin-bottom: 10px; }
.item-receta { display: flex; justify-content: space-between; align-items: center; padding: 8px 12px; border-bottom: 1px solid #eee; background: white; }
.acciones-item { display: flex; align-items: center; gap: 5px; }
.btn-trash { background: none; border: none; cursor: pointer; font-size: 1.1em; }
.vacio { padding: 20px; text-align: center; color: #999; font-style: italic; }
.barra-fondo { width: 100%; height: 8px; background: #eee; border-radius: 4px; overflow: hidden; }
.barra-progreso { height: 100%; background: #e74c3c; transition: width 0.3s ease; }
.barra-progreso.completo { background: #2ecc71; }

.footer-actions { margin-top: 20px; display: flex; justify-content: flex-end; align-items: center; gap: 15px; margin-bottom: 30px; }
.btn-save { background: #2c3e50; color: white; padding: 12px 30px; border: none; border-radius: 6px; font-weight: bold; cursor: pointer; font-size: 1.1em; }
.btn-save:disabled { background: #95a5a6; cursor: not-allowed; }
.msg { font-weight: bold; }
.msg.success { color: green; }
.msg.error { color: #c0392b; }

.lista-card { margin-top: 20px; background: white; padding: 20px; border-radius: 8px; border: 1px solid #e0e0e0; }
.scroll-tabla { max-height: 300px; overflow-y: auto; }
table { width: 100%; border-collapse: collapse; font-size: 0.9em; }
th { text-align: left; background: #f4f4f4; padding: 8px; position: sticky; top: 0; }
td { border-bottom: 1px solid #eee; padding: 8px; }
.btn-editar { border: none; background: none; font-size: 1.2em; cursor: pointer; }
.card.bloqueado {
    background-color: #f9f9f9; /* Un gris muy claro */
    border: 1px dashed #ccc;
}
.aviso-bloqueo {
    background-color: #e9ecef;
    color: #495057;
    padding: 10px;
    border-radius: 4px;
    margin-bottom: 10px;
    text-align: center;
    border: 1px solid #ced4da;
    font-size: 0.9em;
}
.card.bloqueado .item-receta {
    background-color: #f0f0f0;
    color: #555;
}

@media (max-width: 800px) { .form-grid { grid-template-columns: 1fr; } }
</style>