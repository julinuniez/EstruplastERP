<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useRoute } from 'vue-router'; 

const route = useRoute(); 

// --- Variables ---
const esModoEdicion = computed(() => !!route.params.id);
const productoId = computed(() => Number(route.params.id) || 0);

// Estado del formulario
const form = ref({
    nombre: '',
    codigoSku: '',
    largo: 0,
    ancho: 0,
    espesor: 0,
    stockMinimo: 1000,
    pesoEspecifico: 0.92,
    precioCosto: 0,
    receta: [] as { materiaPrimaId: number, nombreMP: string, cantidad: number }[]
})

const listaMateriasPrimas = ref<any[]>([]);
const listaProductosBase = ref<any[]>([]); 
const productoBaseId = ref(''); 
const modoCreacion = ref('PT'); // PT = Producto Terminado
const ingredienteTemp = ref({ id: '', cantidad: 0 });
const mensaje = ref('');
const error = ref('');

// VARIABLE PARA DEBUG
const debugDatosRecibidos = ref<any>(null);

// ===============================================
// 1. FUNCIÓN DE COPIADO
// ===============================================
async function copiarDatosDeProducto() {
    if (!productoBaseId.value) return;

    try {
        error.value = '';
        const res = await fetch(`https://localhost:7244/api/Productos/${productoBaseId.value}`);
        if (!res.ok) throw new Error("Error al conectar con el servidor.");
        
        const data = await res.json();
        
        // DEBUG
        debugDatosRecibidos.value = data; 
        console.log("📦 JSON RECIBIDO:", data);

        // --- MAPEO DE DATOS ---
        form.value.nombre = `${data.nombre || data.Nombre} (Copia)`;
        form.value.codigoSku = ""; 
        
        form.value.largo = data.largo ?? data.Largo ?? 0;
        form.value.ancho = data.ancho ?? data.Ancho ?? 0;
        form.value.espesor = data.espesor ?? data.Espesor ?? 0;
        form.value.pesoEspecifico = data.pesoEspecifico ?? data.PesoEspecifico ?? 0.92;
        form.value.stockMinimo = data.stockMinimo ?? data.StockMinimo ?? 1000;

        const esPT = data.esProductoTerminado ?? data.EsProductoTerminado ?? true;
        modoCreacion.value = esPT ? 'PT' : 'MP';

        // --- RECETA ---
        const formulasBackend = data.formulas || data.Formulas || data.receta || [];

        if (formulasBackend && formulasBackend.length > 0) {
            form.value.receta = formulasBackend.map((f: any) => {
                const idMP = f.materiaPrimaId || f.MateriaPrimaId || f.MateriaPrimaID;
                const cant = f.cantidad || f.Cantidad;
                const insumo = listaMateriasPrimas.value.find(m => m.id == idMP);
                
                return {
                    materiaPrimaId: Number(idMP),
                    nombreMP: insumo ? insumo.nombre : `(ID: ${idMP} - No encontrado)`,
                    cantidad: Number(cant)
                };
            });
        } else {
            form.value.receta = [];
        }

    } catch (e: any) {
        console.error("Error fatal:", e);
        error.value = "Error al copiar: " + e.message;
    }
}

// ===============================================
// 2. CARGA INICIAL
// ===============================================
onMounted(async () => {
    try {
        const [resMP, resProd] = await Promise.all([
            fetch('https://localhost:7244/api/Stock/materias-primas'),
            fetch('https://localhost:7244/api/Productos')
        ]);
        listaMateriasPrimas.value = await resMP.json();
        listaProductosBase.value = await resProd.json();
        
        if (esModoEdicion.value && productoId.value) {
            await cargarDatosEdicion(productoId.value);
        }
    } catch (e) { console.error("Error inicial", e); }
});

async function cargarDatosEdicion(id: number) {
    const res = await fetch(`https://localhost:7244/api/Productos/${id}`);
    const data = await res.json();
    form.value.nombre = data.nombre || data.Nombre;
    form.value.codigoSku = data.codigoSku || data.CodigoSku;
    form.value.largo = data.largo || data.Largo;
    form.value.ancho = data.ancho || data.Ancho;
    form.value.espesor = data.espesor || data.Espesor;
    form.value.pesoEspecifico = data.pesoEspecifico || data.PesoEspecifico;
    form.value.stockMinimo = data.stockMinimo || data.StockMinimo;
    
    const raw = data.formulas || data.Formulas || [];
    form.value.receta = raw.map((f: any) => {
        const idMP = f.materiaPrimaId || f.MateriaPrimaId;
        const insumo = listaMateriasPrimas.value.find(m => m.id == idMP);
        return { materiaPrimaId: idMP, nombreMP: insumo?.nombre || '?', cantidad: f.cantidad || f.Cantidad };
    });
}

// ===============================================
// 3. UI HELPER METHODS
// ===============================================
function agregarIngrediente() {
    if (!ingredienteTemp.value.id || ingredienteTemp.value.cantidad <= 0) return;
    const mp = listaMateriasPrimas.value.find(m => m.id == ingredienteTemp.value.id)
    form.value.receta.push({
        materiaPrimaId: Number(ingredienteTemp.value.id),
        nombreMP: mp ? mp.nombre : '?',
        cantidad: ingredienteTemp.value.cantidad
    })
    ingredienteTemp.value.id = ''; ingredienteTemp.value.cantidad = 0;
}
function quitarIngrediente(index: number) { form.value.receta.splice(index, 1) }

async function guardarProducto() {
    mensaje.value = ''; error.value = '';
    if (!form.value.nombre || !form.value.codigoSku) { error.value = "Faltan datos obligatorios"; return; }

    const url = esModoEdicion.value 
        ? `https://localhost:7244/api/Productos/actualizar/${productoId.value}`
        : 'https://localhost:7244/api/Productos/crear';
    const method = esModoEdicion.value ? 'PUT' : 'POST';

    try {
        const res = await fetch(url, {
            method: method,
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(form.value)
        })
        if(res.ok) {
            mensaje.value = "✅ Guardado correctamente";
            if(!esModoEdicion.value) {
                form.value.nombre = ''; form.value.codigoSku = ''; form.value.receta = [];
                productoBaseId.value = ''; debugDatosRecibidos.value = null;
            }
        } else {
            const d = await res.json(); error.value = "❌ " + (d.mensaje || "Error servidor");
        }
    } catch(e: any) { error.value = "❌ Red: " + e.message; }
}
</script>

<template>
    <div class="contenedor-principal">
        
        <div class="header-action">
            <h2>{{ esModoEdicion ? '✏️ Editar Producto' : '📦 Nuevo Producto' }}</h2>
            
            <div v-if="!esModoEdicion" class="selector-copia">
                <label>Copiar desde existente:</label>
                <select v-model="productoBaseId" @change="copiarDatosDeProducto">
                    <option value="">-- Seleccionar --</option>
                    <option v-for="p in listaProductosBase" :key="p.id" :value="p.id">
                        {{ p.nombre }}
                    </option>
                </select>
            </div>
        </div>

        <div v-if="debugDatosRecibidos" class="debug-box">
            <h4>🔍 Diagnóstico de Carga</h4>
            <p><strong>Fórmulas recibidas (Raw):</strong> {{ debugDatosRecibidos.formulas || debugDatosRecibidos.Formulas || 'Ninguna' }}</p>
            <details>
                <summary>Ver JSON Completo</summary>
                <pre>{{ debugDatosRecibidos }}</pre>
            </details>
        </div>

        <div class="tabs">
            <div class="tab" :class="{ active: modoCreacion === 'PT' }" @click="modoCreacion = 'PT'">
                🏭 Producto Terminado
            </div>
            <div class="tab" :class="{ active: modoCreacion === 'MP' }" @click="modoCreacion = 'MP'">
                🛢️ Materia Prima
            </div>
        </div>

        <div class="form-grid">
            <div class="card">
                <h3>Datos Generales</h3>
                <div class="campo">
                    <label>Nombre del Producto</label>
                    <input v-model="form.nombre" type="text" placeholder="Ej: Lámina PAI...">
                </div>
                <div class="campo-row">
                    <div class="campo">
                        <label>SKU (Código)</label>
                        <input v-model="form.codigoSku" type="text" placeholder="PAI-001">
                    </div>
                    <div class="campo">
                        <label>Stock Mínimo</label>
                        <input v-model.number="form.stockMinimo" type="number">
                    </div>
                </div>

                <div v-if="modoCreacion === 'PT'" class="seccion-pt">
                    <h4 class="subtitulo">Dimensiones y Material</h4>
                    
                    <div class="campo-row tres">
                        <div class="campo-chico">
                            <label>Largo (mm)</label>
                            <input v-model.number="form.largo" type="number">
                        </div>
                        <div class="campo-chico">
                            <label>Ancho (mm)</label>
                            <input v-model.number="form.ancho" type="number">
                        </div>
                        <div class="campo-chico">
                            <label>Espesor (mic)</label>
                            <input v-model.number="form.espesor" type="number">
                        </div>
                    </div>

                    <div class="campo">
                        <label>Peso Específico (Densidad)</label>
                        <div class="input-con-unidad">
                            <input v-model.number="form.pesoEspecifico" type="number" step="0.01">
                            <span>g/cm³</span>
                        </div>
                    </div>
                </div>
            </div>

            <div class="card" v-if="modoCreacion === 'PT'">
                <div class="card-header">
                    <h3>
                        <span v-if="esModoEdicion">🔒 Fórmula (Bloqueada)</span>
                        <span v-else>Fórmula / Receta (en Kilos)</span>
                    </h3>
                    <span class="badge">{{ form.receta.length }} items</span>
                </div>
                
                <div class="add-bar" v-if="!esModoEdicion">
                    <select v-model="ingredienteTemp.id">
                        <option value="">-- Seleccionar Insumo --</option>
                        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">
                            {{ mp.nombre }}
                        </option>
                    </select>
                    <input 
                        v-model.number="ingredienteTemp.cantidad" 
                        type="number" 
                        step="0.001" 
                        placeholder="Kg"
                        class="input-cant"
                    >
                    <button @click="agregarIngrediente" class="btn-icon">➕</button>
                </div>

                <div v-else class="alerta-bloqueo">
    <div style="font-size: 1.5rem; margin-bottom: 5px;">🧪</div>
    <strong>Fórmula Gestionada en otro Módulo</strong>
    <p style="margin: 5px 0 0 0; font-size: 0.9em;">
        Para modificar ingredientes o cantidades, por favor dirígete a la sección 
        <strong>"Fórmulas"</strong> en el menú principal.
    </p>
    </div>

                <div class="lista-items">
                    <div v-for="(item, index) in form.receta" :key="index" class="item-receta">
                        <div>
                            <strong>{{ item.nombreMP }}</strong>
                            <div style="font-size:0.85em; color:#666">
                                Cantidad: <strong>{{ item.cantidad }} kg</strong>
                            </div>
                        </div>
                        <button v-if="!esModoEdicion" @click="quitarIngrediente(index)" class="btn-trash">🗑️</button>
                    </div>
                    
                    <div v-if="form.receta.length === 0" class="vacio">
                        <span v-if="esModoEdicion">Receta oculta o vacía.</span>
                        <span v-else>Sin ingredientes cargados.</span>
                    </div>
                </div>
            </div>

            <div class="card info-box" v-else>
                <h3>ℹ️ Modo Materia Prima</h3>
                <p>Las materias primas no llevan fórmula de composición.</p>
            </div>
        </div>

        <div class="footer-actions">
            <span v-if="mensaje" class="msg success">{{ mensaje }}</span>
            <span v-if="error" class="msg error">{{ error }}</span>
            <button @click="guardarProducto" class="btn-save">
                {{ esModoEdicion ? 'Guardar Cambios' : 'Crear Producto' }}
            </button>
        </div>
    </div>
</template>

<style scoped>
.contenedor-principal { max-width: 1000px; margin: 0 auto; color: #333; font-family: sans-serif; }
.header-action { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }

/* Selector */
.selector-copia { background: #f0f4f8; padding: 8px 15px; border-radius: 6px; border: 1px solid #dceefb; display: flex; align-items: center; gap: 10px; }
.selector-copia select { border: 1px solid #bdc3c7; background: white; padding: 5px; border-radius: 4px; min-width: 250px; }

/* Debug Box */
.debug-box { background: #333; color: #0f0; padding: 15px; margin-bottom: 20px; border-radius: 5px; font-family: monospace; font-size: 0.8rem; overflow-x: auto;}
.debug-box pre { background: #111; padding: 10px; border: 1px solid #444; }

/* Tabs */
.tabs { display: flex; gap: 5px; margin-bottom: 15px; border-bottom: 2px solid #eee; }
.tab { padding: 10px 20px; cursor: pointer; border-radius: 5px 5px 0 0; background: #f9f9f9; color: #777; }
.tab.active { background: #fff; color: #2c3e50; font-weight: bold; border: 1px solid #eee; border-bottom: 2px solid white; margin-bottom: -2px; }

/* Grid */
.form-grid { display: grid; grid-template-columns: 1.1fr 0.9fr; gap: 20px; }
.card { background: white; padding: 20px; border-radius: 8px; border: 1px solid #e0e0e0; box-shadow: 0 2px 5px rgba(0,0,0,0.05); }
.card h3 { margin-top: 0; border-bottom: 1px solid #eee; padding-bottom: 10px; margin-bottom: 15px; color: #2c3e50; }

/* Inputs */
.campo { margin-bottom: 15px; }
label { display: block; font-weight: 600; font-size: 0.85rem; margin-bottom: 5px; color: #555; }
input, select { width: 100%; padding: 8px 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }
input:focus { border-color: #3498db; outline: none; }
.campo-row { display: flex; gap: 15px; }
.campo-chico { flex: 1; } 
.subtitulo { margin: 15px 0 10px 0; font-size: 0.95rem; color: #3498db; text-transform: uppercase; font-weight: bold; letter-spacing: 0.5px; border-bottom: 1px dashed #eee; padding-bottom: 5px; }
.input-con-unidad { display: flex; align-items: center; gap: 5px; }
.input-con-unidad span { color: #666; font-size: 0.9rem; white-space: nowrap; }

/* --- ESTILOS DE BARRA DE AGREGAR AJUSTADOS --- */
.add-bar { 
    display: flex; 
    gap: 10px; 
    margin-bottom: 10px; 
}
/* El select crece para ocupar espacio */
.add-bar select { 
    flex-grow: 1; 
}
/* El input de cantidad se queda fijo pequeño */
.input-cant {
    width: 90px !important;
}

.btn-icon { background: #27ae60; color: white; border: none; width: 45px; cursor: pointer; border-radius: 4px; font-weight: bold; font-size: 1.2rem; }
/* --------------------------------------------- */

/* Alerta de bloqueo */
.alerta-bloqueo {
    background-color: #e3f2fd; /* Azul suave */
    color: #0d47a1;            /* Azul oscuro texto */
    padding: 15px;
    border-radius: 8px;
    border: 1px solid #bbdefb;
    font-size: 0.95rem;
    margin-bottom: 15px;
    text-align: center;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.lista-items { border: 1px solid #eee; border-radius: 4px; max-height: 250px; overflow-y: auto; background: #fafafa; }
.item-receta { display: flex; justify-content: space-between; align-items: center; padding: 8px 12px; border-bottom: 1px solid #eee; background: white; }
.btn-trash { background: none; border: none; cursor: pointer; font-size: 1.2rem; }

/* Footer */
.footer-actions { margin-top: 20px; display: flex; justify-content: flex-end; align-items: center; gap: 15px; }
.btn-save { background: #2c3e50; color: white; padding: 12px 25px; border: none; border-radius: 6px; font-weight: bold; cursor: pointer; }
.msg.success { color: green; font-weight: bold; }
.msg.error { color: red; font-weight: bold; }
</style>