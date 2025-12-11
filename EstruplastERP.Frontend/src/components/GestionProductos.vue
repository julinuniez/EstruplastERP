<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router'; 
import axios from 'axios'; 

const route = useRoute(); 

// --- Variables ---
const esModoEdicion = computed(() => !!route.params.id);
const productoId = computed(() => Number(route.params.id) || 0);

// Estado del formulario (SOLO PRODUCTOS TERMINADOS / MATERIALES DE VENTA)
const form = ref({
    nombre: '',
    codigoSku: '',
    color: '',
    stockMinimo: 1000,
    precioCosto: 0,
    
    // Estos valores se envían en 0 o null al backend
    largo: 0,
    ancho: 0,
    espesor: 0,
    pesoEspecifico: 0, 
    
    // Receta (%)
    receta: [] as { materiaPrimaId: number, nombreMP: string, cantidad: number }[]
})

const listaMateriasPrimas = ref<any[]>([]);
const listaProductosBase = ref<any[]>([]); 
const productoBaseId = ref(''); 
const ingredienteTemp = ref({ id: '', cantidad: '' as string | number }); 
const mensaje = ref('');
const error = ref('');

const apiUrl = import.meta.env.VITE_API_URL;

// --- COMPUTADOS ---

// Calcula el total del porcentaje acumulado
const totalPorcentaje = computed(() => {
    if (form.value.receta.length === 0) return 0;
    const total = form.value.receta.reduce((acc, item) => acc + item.cantidad, 0);
    return parseFloat(total.toFixed(2));
});

// Validación: Solo permite guardar si suma 100% (o si es PET Reventa que no lleva receta)
const esRecetaValida = computed(() => {
    // Si es un producto de reventa (PET), permitimos guardar sin receta
    const esReventa = form.value.nombre.toUpperCase().includes('PET') || form.value.nombre.toUpperCase().includes('REVENTA');
    if (esReventa) return true;

    return totalPorcentaje.value === 100;
});

// --- HELPER TOKEN ---
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// ===============================================
// 1. FUNCIÓN DE COPIADO 
// ===============================================
async function copiarDatosDeProducto() {
    if (!productoBaseId.value) return;
    try {
        error.value = '';
        const res = await axios.get(`${apiUrl}/Productos/${productoBaseId.value}`, getAuthConfig());
        const data = res.data; 
        
        form.value.nombre = `${data.nombre || data.Nombre} (Copia)`;
        form.value.codigoSku = ""; // Limpiamos SKU
        form.value.color = data.color || '';
        form.value.stockMinimo = data.stockMinimo ?? 1000;

        // Copiamos Receta
        const formulasBackend = data.formulas || data.Formulas || data.receta || [];
        if (formulasBackend.length > 0) {
            form.value.receta = formulasBackend.map((f: any) => {
                const idMP = f.materiaPrimaId || f.MateriaPrimaId;
                const cant = f.cantidad || f.Cantidad;
                const insumo = listaMateriasPrimas.value.find(m => m.id == idMP);
                return {
                    materiaPrimaId: Number(idMP),
                    nombreMP: insumo?.nombre || `(ID: ${idMP}?)`,
                    cantidad: Number(cant)
                };
            });
        } else {
            form.value.receta = [];
        }
    } catch (e: any) { error.value = "Error al copiar producto."; }
}

// ===============================================
// 2. CARGA INICIAL
// ===============================================
onMounted(async () => {
    try {
        const config = getAuthConfig();
        
        // Carga paralela de Insumos y Productos Existentes
        const [resMP, resProd] = await Promise.all([
            // Insumos para el dropdown de receta
            axios.get(`${apiUrl}/Productos/materias-primas`, config),
            // Productos Terminados para la lista
            axios.get(`${apiUrl}/Productos`, config)
        ]);
        
        listaMateriasPrimas.value = resMP.data;
        // Filtramos solo los terminados (Materiales de Venta)
        listaProductosBase.value = resProd.data.filter((p:any) => p.esProductoTerminado);
        
        // Si venimos a editar
        if (esModoEdicion.value && productoId.value) {
            await cargarDatosEdicion(productoId.value, config);
        }
    } catch (e: any) { 
        if(e.response && e.response.status === 401) error.value = "Sesión expirada.";
        else error.value = "Error de conexión o datos.";
    }
});

async function cargarDatosEdicion(id: number, config: any) {
    try {
        const res = await axios.get(`${apiUrl}/Productos/${id}`, config);
        const data = res.data;
        form.value.nombre = data.nombre;
        form.value.codigoSku = data.codigoSku;
        form.value.color = data.color;
        form.value.stockMinimo = data.stockMinimo;
        
        const raw = data.formulas || data.Formulas || [];
        form.value.receta = raw.map((f: any) => {
            const idMP = f.materiaPrimaId || f.MateriaPrimaId;
            const insumo = listaMateriasPrimas.value.find(m => m.id == idMP);
            return { materiaPrimaId: idMP, nombreMP: insumo?.nombre || '?', cantidad: f.cantidad };
        });
    } catch (e) { error.value = "Error cargando edición."; }
}

// ===============================================
// 3. RECETA (Agregar / Quitar)
// ===============================================
function agregarIngrediente() {
    const id = ingredienteTemp.value.id;
    const porc = parseFloat(ingredienteTemp.value.cantidad.toString());

    if (!id || porc <= 0) return;

    // Validación de porcentaje
    if (totalPorcentaje.value + porc > 100) {
        alert(`❌ Excede el 100%. (Disponible: ${(100 - totalPorcentaje.value).toFixed(2)}%)`);
        return;
    }

    const mp = listaMateriasPrimas.value.find(m => m.id == id)
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

function quitarIngrediente(index: number) { form.value.receta.splice(index, 1) }

// ===============================================
// 4. GUARDAR
// ===============================================
async function guardarProducto() {
    mensaje.value = ''; error.value = '';
    
    if (!form.value.nombre || !form.value.codigoSku) { 
        error.value = "Faltan datos obligatorios"; return; 
    }

    // Excepción PET: No valida receta
    const esReventa = form.value.nombre.toUpperCase().includes('PET') || form.value.nombre.toUpperCase().includes('REVENTA');
    
    if (!esReventa) {
        if (form.value.receta.length === 0) {
            error.value = "⛔ Este material requiere una fórmula de producción."; return;
        }
        if (totalPorcentaje.value !== 100) {
            error.value = `⛔ Error: La fórmula suma ${totalPorcentaje.value}%. Debe sumar 100%.`; return;
        }
    }

    const url = esModoEdicion.value 
        ? `${apiUrl}/Productos/actualizar/${productoId.value}`
        : `${apiUrl}/Productos/crear`;

    const payload = {
        ...form.value,
        esProductoTerminado: true, // Siempre es PT
        esMateriaPrima: false,
        largo: 0, ancho: 0, espesor: 0, pesoEspecifico: 0 // Se definen en producción
    };

    try {
        if (esModoEdicion.value) await axios.put(url, payload, getAuthConfig());
        else await axios.post(url, payload, getAuthConfig());

        mensaje.value = "✅ Guardado correctamente";
        
        if(!esModoEdicion.value) {
            // Limpiar form
            form.value.nombre = ''; form.value.codigoSku = ''; form.value.receta = []; form.value.color = '';
            productoBaseId.value = ''; 
            // Recargar lista
            const res = await axios.get(`${apiUrl}/Productos`, getAuthConfig());
            listaProductosBase.value = res.data.filter((p:any) => p.esProductoTerminado);
        }
    } catch(e: any) { 
        error.value = "❌ " + (e.response?.data?.mensaje || e.message);
    }
}
</script>

<template>
    <div class="contenedor-principal">
        
        <div class="header-action">
            <h2>{{ esModoEdicion ? '✏️ Editar Material' : '📦 Nuevo Material de Venta' }}</h2>
            
            <div v-if="!esModoEdicion" class="selector-copia">
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
                    <label>Nombre Comercial (Ej: PAI Blanco, ABS Negro)</label>
                    <input v-model="form.nombre" type="text" placeholder="Ej: Material PAI Blanco Estándar">
                </div>
                <div class="campo-row">
                    <div class="campo">
                        <label>SKU / Código</label>
                        <input v-model="form.codigoSku" type="text" placeholder="MAT-001">
                    </div>
                    <div class="campo">
                        <label>Color</label>
                        <input v-model="form.color" type="text" placeholder="Ej: Blanco">
                    </div>
                </div>
                <div class="campo">
                    <label>Stock Mínimo (Alerta Kg)</label>
                    <input v-model.number="form.stockMinimo" type="number">
                </div>
                <div class="nota-info">
                    <p>ℹ️ Las medidas (ancho, largo, espesor) se definen al momento de crear la <strong>Orden de Producción</strong>.</p>
                </div>
            </div>

            <div class="card">
                <div class="card-header">
                    <h3>🧪 Fórmula de Producción (%)</h3>
                    <div class="indicador-total" :class="{'ok': totalPorcentaje===100, 'error': totalPorcentaje!==100}">
                        Total: {{ totalPorcentaje }}%
                    </div>
                </div>
                
                <div class="add-bar"> 
    <select v-model="ingredienteTemp.id">
        <option value="">-- Insumo --</option>
        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">
            {{ mp.nombre }}
        </option>
    </select>
    
    <input 
        v-model.number="ingredienteTemp.cantidad" 
        type="number" step="0.1" max="100"
        placeholder="%"
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
                            <span class="badge-porc">{{ item.cantidad }}%</span>
                            <button @click="quitarIngrediente(index)" class="btn-trash">🗑️</button>
                        </div>
                    </div>
                    <div v-if="form.receta.length === 0" class="vacio">
                        <span>Sin ingredientes. Defina la composición.</span>
                    </div>
                </div>
                
                <div class="barra-fondo">
                    <div class="barra-progreso" :style="{width: totalPorcentaje + '%'}" :class="{'completo': totalPorcentaje===100}"></div>
                </div>
            </div>
        </div>

        <div class="footer-actions">
            <span v-if="mensaje" class="msg success">{{ mensaje }}</span>
            <span v-if="error" class="msg error">{{ error }}</span>
            
            <button @click="guardarProducto" class="btn-save" :disabled="!esRecetaValida && !form.nombre.toUpperCase().includes('PET')">
                {{ esModoEdicion ? 'Guardar Cambios' : 'Crear Material' }}
            </button>
        </div>

        <div class="lista-card">
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
                                <button class="btn-mini" @click="$router.push(`/crear-producto/${p.id}`)">✏️</button> 
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
</template>

<style scoped>
/* Estilos Limpios */
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
.nota-info { font-size: 0.85em; color: #555; background: #fff3cd; padding: 10px; border-radius: 4px; border: 1px solid #ffeeba; margin-top: 15px; }

/* Receta */
.card-header { display: flex; justify-content: space-between; align-items: center; }
.indicador-total { font-weight: bold; font-size: 0.9em; }
.indicador-total.ok { color: green; }
.indicador-total.error { color: #e74c3c; }
.add-bar { display: flex; gap: 5px; margin-bottom: 10px; }
.add-bar select { flex-grow: 1; }
.input-cant { width: 70px !important; text-align: center; }
.btn-icon { background: #27ae60; color: white; border: none; width: 40px; cursor: pointer; border-radius: 4px; font-weight: bold; font-size: 1.2rem; }
.btn-icon:disabled { background: #ccc; cursor: not-allowed; }
.lista-items { border: 1px solid #eee; border-radius: 4px; height: 150px; overflow-y: auto; background: #fafafa; margin-bottom: 10px; }
.item-receta { display: flex; justify-content: space-between; align-items: center; padding: 8px 12px; border-bottom: 1px solid #eee; background: white; }
.acciones-item { display: flex; align-items: center; gap: 10px; }
.badge-porc { background: #e3f2fd; color: #1565c0; padding: 2px 6px; border-radius: 4px; font-weight: bold; font-size: 0.9em; }
.btn-trash { background: none; border: none; cursor: pointer; }
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
.btn-mini { padding: 4px 8px; margin-right: 5px; border: none; background: #ddd; border-radius: 4px; cursor: pointer; }

@media (max-width: 800px) { .form-grid { grid-template-columns: 1fr; } }
</style>