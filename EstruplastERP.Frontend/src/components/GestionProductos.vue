<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'

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

// Listas para los desplegables
const listaMateriasPrimas = ref([])
const modoCreacion = ref('PT')

// Variables temporales para agregar ingredientes
const ingredienteTemp = ref({
  id: '',
  cantidad: 0
})

const mensaje = ref('')
const error = ref('')

// Cargar Materias Primas al iniciar
onMounted(async () => {
    try {
        // Reutilizamos el endpoint que hicimos para Ingreso Stock
        const res = await fetch('https://localhost:7244/api/Stock/materias-primas')
        listaMateriasPrimas.value = await res.json()
    } catch (e) {
        console.error("Error cargando materias primas")
    }
})

// Función para agregar ingrediente a la lista temporal (Visual)
function agregarIngrediente() {
    if (!ingredienteTemp.value.id || ingredienteTemp.value.cantidad <= 0) {
        alert("Selecciona un insumo y una cantidad válida")
        return
    }

    // Buscamos el nombre para mostrarlo lindo en la tabla
    const mp = listaMateriasPrimas.value.find(m => m.id == ingredienteTemp.value.id)

    // Agregamos a la lista de la receta
    form.value.receta.push({
        materiaPrimaId: Number(ingredienteTemp.value.id),
        nombreMP: mp ? mp.nombre : 'Desc.',
        cantidad: ingredienteTemp.value.cantidad
    })

    // Limpiamos los inputs pequeños
    ingredienteTemp.value.id = ''
    ingredienteTemp.value.cantidad = 0
}

function quitarIngrediente(index: number) {
    form.value.receta.splice(index, 1)
}

async function guardarProducto() {
    mensaje.value = ''
    error.value = ''
    
    // 1. Limpieza de campos según el modo (Se mantiene igual)
    if (modoCreacion.value === 'MP') {
        form.value.receta = []
        form.value.largo = 0
        form.value.ancho = 0
        form.value.espesor = 0
        form.value.pesoEspecifico = 0
    }

    // 2. VALIDACIÓN CORREGIDA (Se mantiene igual)
    if (modoCreacion.value === 'PT' && form.value.receta.length === 0) {
        error.value = "⚠️ La receta es obligatoria para Productos Terminados. Agrega al menos un insumo."
        return
    }

    // 3. Verificación de campos obligatorios (Se mantiene igual)
    if (!form.value.nombre || !form.value.codigoSku) {
        error.value = "⚠️ El Nombre y el Código SKU son obligatorios."
        return
    }
    
    // Si llegaste aquí, o es MP (sin receta) o es PT (con receta). Procedemos.

    try {
        const res = await fetch('https://localhost:7244/api/Productos/crear', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(form.value)
        })

        if (!res.ok) {
            // 🚨 PUNTO DE MODIFICACIÓN CLAVE 🚨
            let errorMensaje = "Error desconocido al guardar el ítem.";
            
            try {
                // Intenta leer la respuesta como JSON
                const data = await res.json();
                
                // Si la data.mensaje existe, la usamos. Si no, usamos el estado.
                errorMensaje = data.mensaje || `El servidor devolvió un error: ${res.status}`;
                
            } catch (jsonError) {
                // ❌ CAPTURA EL ERROR "Unexpected token 'E', 'Error al g'..."
                errorMensaje = "❌ No se pudo crear el producto. El servidor devolvió una respuesta no válida o inesperada. Revisa los logs del backend.";
                console.error("Error al parsear JSON del servidor:", jsonError);
                // Si quieres leer el texto plano que causó el error:
                // console.log("Respuesta de texto crudo:", await res.text()); 
            }
            
            error.value = "❌ " + errorMensaje;
            return; // Detiene la ejecución en caso de error
        }

        // Si la respuesta fue OK (res.ok es true)
        // Opcional: leer el JSON de éxito si lo hay, aunque no es estrictamente necesario aquí.
        // const resultado = await res.json(); 
        
        mensaje.value = "✅ Ítem creado correctamente! Puedes registrar compras ahora."
        
        // Limpiar formulario (Se mantiene igual)
        form.value = {
            nombre: '', codigoSku: '', largo: 0, ancho: 0, espesor: 0, 
            stockMinimo: 1000, pesoEspecifico: 0.92, precioCosto: 0, receta: []
        }

    } catch (e: any) {
        // Esto captura errores de red (servidor completamente inaccesible o caído)
        error.value = "❌ Error de conexión con el servidor: " + e.message;
        console.error("Error de red:", e);
    }
}
</script>

<template>
    <div class="panel-creacion">
        <h2>📦 Nuevo Ítem</h2>

        <div class="selector-tipo">
            <button 
                @click="modoCreacion = 'PT'" 
                :class="{ 'activo': modoCreacion === 'PT' }"
            >
                Lámina / Producto Terminado
            </button>
            <button 
                @click="modoCreacion = 'MP'" 
                :class="{ 'activo': modoCreacion === 'MP' }"
            >
                Materia Prima / Insumo
            </button>
        </div>
        
        <div class="grid-form">
            <div class="columna">
                <h3>1. Datos Básicos</h3>
                <label>Nombre:</label>
                <input v-model="form.nombre" type="text" placeholder="Ej: Polietileno de Baja Densidad" />

                <label>Código SKU:</label>
                <input v-model="form.codigoSku" type="text" placeholder="Ej: PEBD-CRISTAL" />

                <div v-if="modoCreacion === 'PT'">
                    <div class="fila-tres">
                        <div><label>Largo (mm)</label><input v-model.number="form.largo" type="number"></div>
                        <div><label>Ancho (mm)</label><input v-model.number="form.ancho" type="number"></div>
                        <div><label>Espesor (mic)</label><input v-model.number="form.espesor" type="number"></div>
                    </div>

                    <label>Densidad / Peso Esp.:</label>
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <input v-model.number="form.pesoEspecifico" type="number" step="0.01" placeholder="0.92" />
                        <small style="color: #666; white-space: nowrap;">(0.92 Baja / 1.05 Alta)</small>
                    </div>
                </div>
                <label>Stock Mínimo Alerta:</label>
                <input v-model.number="form.stockMinimo" type="number" />
            </div>

            <div class="columna receta-box" v-if="modoCreacion === 'PT'">
                <h3>2. Fórmula / Receta 🧪</h3>
                <p class="hint">Agrega los insumos necesarios para 1 unidad o %.</p>
                
                <div class="agregar-box">
                    <select v-model="ingredienteTemp.id">
                        <option value="">Seleccionar Insumo...</option>
                        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">
                            {{ mp.nombre }}
                        </option>
                    </select>
                    <input v-model.number="ingredienteTemp.cantidad" type="number" placeholder="Cant." style="width: 80px;" />
                    <button @click="agregarIngrediente" class="btn-add">+</button>
                </div>
                <ul class="lista-ingredientes">
                    <li v-for="(item, index) in form.receta" :key="index">
                        <span>{{ item.nombreMP }} ({{ item.cantidad }})</span>
                        <button @click="quitarIngrediente(index)" class="btn-del">🗑️</button>
                    </li>
                    <li v-if="form.receta.length === 0" class="vacio">Sin ingredientes aún...</li>
                </ul>
                </div>
            
            <div class="columna info-mp" v-else>
                <h3>2. Tipo de Ítem</h3>
                <p>Este ítem se registrará como **Materia Prima/Insumo**. No requiere dimensiones ni fórmula.</p>
            </div>
        </div>
        
        <div class="acciones">
            <button @click="guardarProducto" class="btn-guardar">💾 GUARDAR ÍTEM</button>
        </div>

        <p v-if="mensaje" class="msg-exito">{{ mensaje }}</p>
        <p v-if="error" class="msg-error">{{ error }}</p>
    </div>
</template>

<style scoped>
.panel-creacion { background: white; padding: 20px; border-radius: 10px; max-width: 900px; margin: 0 auto; box-shadow: 0 4px 10px rgba(0,0,0,0.1); }
.grid-form { display: grid; grid-template-columns: 1fr 1fr; gap: 30px; margin-bottom: 20px; }
.columna h3 { border-bottom: 2px solid #eee; padding-bottom: 10px; margin-bottom: 15px; color: #2c3e50; }
label { display: block; margin-top: 10px; font-weight: bold; font-size: 0.9em; }
input, select { width: 100%; padding: 8px; margin-top: 5px; border: 1px solid #ccc; border-radius: 4px; }
.fila-tres { display: flex; gap: 10px; }

/* Estilos Receta */
.receta-box { background: #f9f9f9; padding: 15px; border-radius: 8px; border: 1px dashed #ccc; }
.hint { font-size: 0.8em; color: #666; margin-bottom: 10px; }
.agregar-box { display: flex; gap: 5px; margin-bottom: 15px; }
.btn-add { background: #3498db; color: white; border: none; width: 40px; font-weight: bold; cursor: pointer; border-radius: 4px; }
.lista-ingredientes { list-style: none; padding: 0; }
.lista-ingredientes li { display: flex; justify-content: space-between; padding: 8px; border-bottom: 1px solid #eee; background: white; margin-bottom: 5px; }
.btn-del { background: transparent; border: none; cursor: pointer; }
.vacio { color: #999; font-style: italic; text-align: center; padding: 10px; }

/* Botón final */
.btn-guardar { background: #2c3e50; color: white; width: 100%; padding: 15px; font-size: 1.1em; border: none; border-radius: 6px; cursor: pointer; transition: background 0.3s; }
.btn-guardar:hover { background: #1a252f; }

.msg-exito { color: green; text-align: center; margin-top: 15px; font-weight: bold; }
.msg-error { color: red; text-align: center; margin-top: 15px; font-weight: bold; }

.selector-tipo {
    display: flex;
    gap: 15px;
    margin-bottom: 25px;
    padding-bottom: 10px;
    border-bottom: 2px solid #eee;
}

.selector-tipo button {
    padding: 10px 15px;
    border: 1px solid #ccc;
    background: #f9f9f9;
    cursor: pointer;
    border-radius: 6px;
    transition: all 0.2s;
}

.selector-tipo button.activo {
    background: #3498db; /* Azul para resaltar la elección */
    color: white;
    font-weight: bold;
    border-color: #3498db;
}

.info-mp {
    background-color: #e8f5e9; /* Verde muy claro */
    padding: 20px;
    border-radius: 8px;
    height: fit-content;
    border: 1px dashed #4caf50;
}
</style>