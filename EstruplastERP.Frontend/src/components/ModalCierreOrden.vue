<script setup lang="ts">
import { ref, watch } from 'vue'
import axios from 'axios'

const props = defineProps<{
  orden: any,
  materiasPrimas: any[],
  visible: boolean
}>()

const emit = defineEmits(['close', 'confirmar'])

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';

// --- ESTADOS ---
const kilosReales = ref<number>(0)
const consumosBase = ref<{ materiaPrimaId: number, nombre: string, teorico: number, real: number, stockActual: number }[]>([])

const formCierre = ref({
  adiciones: [] as { materiaPrimaId: number, nombre: string, cantidad: number, motivo: string }[],
  kilosDesperdicio: 0,
  observacionCierre: ''
})

const adicionSeleccionada = ref<number | ''>('')
const cantidadAdicion = ref<number | ''>('')
const motivoAdicion = ref('')

// --- INICIALIZAR DATOS CUANDO SE ABRE EL MODAL ---
watch(() => props.visible, (isOpen) => {
    if (isOpen && props.orden) {
        // Precargar lo que debía pesar la bobina
        kilosReales.value = props.orden.kilos || props.orden.kilosEstimados || 0;
        
        // Limpiar el form extra
        formCierre.value.kilosDesperdicio = props.orden.desperdicio || 0;
        formCierre.value.observacionCierre = '';
        formCierre.value.adiciones = [];

        // Precargar la receta teórica en inputs editables
        if (props.orden.consumos) {
            consumosBase.value = props.orden.consumos.map((c: any) => {
                // Buscamos el stock físico actual para poder mostrar advertencias
                const mp = props.materiasPrimas.find(m => m.id === c.materiaPrimaId);
                return {
                    materiaPrimaId: c.materiaPrimaId,
                    nombre: c.nombreMateriaPrima,
                    teorico: c.cantidadKilos,
                    real: c.cantidadKilos, // Inicia igual al teórico
                    stockActual: mp ? mp.stockActual : 0
                };
            });
        } else {
            consumosBase.value = [];
        }
    }
});

// --- AGREGAR MATERIAL EXTRA ---
const agregarAdicion = () => {
    if (!adicionSeleccionada.value || !cantidadAdicion.value || Number(cantidadAdicion.value) <= 0) {
        alert("Ingrese datos válidos.");
        return;
    }

    const mp = props.materiasPrimas.find(m => m.id === adicionSeleccionada.value);
    
    if (mp) {
        // Advertencia si queda en negativo (no bloquea)
        if (mp.stockActual < Number(cantidadAdicion.value)) {
            const confirmarNegativo = confirm(`⚠️ CUIDADO: El stock de ${mp.nombre} quedará en negativo.\nStock: ${mp.stockActual} Kg\nAgregas: ${cantidadAdicion.value} Kg\n¿Deseas continuar de todas formas?`);
            if (!confirmarNegativo) return;
        }

        // Lo guardamos en el array de adiciones extra
        formCierre.value.adiciones.push({
            materiaPrimaId: mp.id,
            nombre: mp.nombre,
            cantidad: Number(cantidadAdicion.value),
            motivo: motivoAdicion.value || 'Ajuste' // La observación individual de este material
        });
        
        // Limpiamos los inputs
        adicionSeleccionada.value = '';
        cantidadAdicion.value = '';
        motivoAdicion.value = '';
    }
}

const quitarAdicion = (index: number) => {
    formCierre.value.adiciones.splice(index, 1);
}

const confirmarCierre = async () => {
    // 1. Unimos la Receta Base con las Adiciones Extra
    const todosLosConsumos = [
        ...consumosBase.value.map(c => ({ 
            materiaPrimaId: c.materiaPrimaId, 
            cantidadKilosReales: Number(c.real) 
        })),
        ...formCierre.value.adiciones.map(a => ({ 
            materiaPrimaId: a.materiaPrimaId, 
            cantidadKilosReales: Number(a.cantidad) 
        }))
    ];

    // 🚨 NUEVA VALIDACIÓN FRONTEND: Agrupar y chequear negativos
    const calculoStock = new Map();
    let hayNegativos = false;
    let mensajeError = "⛔ No podés cerrar la orden. Falta cargar stock de:\n";

    for (const item of todosLosConsumos) {
        const id = item.materiaPrimaId;
        calculoStock.set(id, (calculoStock.get(id) || 0) + item.cantidadKilosReales);
    }

    for (const [id, totalKilos] of calculoStock.entries()) {
        const mp = props.materiasPrimas.find(m => m.id === id);
        if (mp && mp.stockActual < totalKilos) {
            hayNegativos = true;
            mensajeError += `- ${mp.nombre} (Faltan ${(totalKilos - mp.stockActual).toFixed(2)} Kg)\n`;
        }
    }

    if (hayNegativos) {
        alert(mensajeError);
        return; // Frenamos la ejecución acá, no mandamos nada al backend
    }
    // 🚨 FIN VALIDACIÓN FRONTEND

    const mensaje = `¿Estás seguro de cerrar la orden con estos consumos reales?`;
    if (!confirm(mensaje)) return;

    try {
        // 2. Armamos el Payload
        const payload = {
            kilosProducidosReales: kilosReales.value,
            desperdicioReal: formCierre.value.kilosDesperdicio,
            observacion: formCierre.value.observacionCierre, 
            consumosReales: todosLosConsumos
        };

        // 3. Enviamos al Backend
        await axios.post(`${apiUrl}/Ordenes/confirmar/${props.orden.id}`, payload, {
             headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } 
        });

        emit('confirmar');
        emit('close');

    } catch (e: any) {
        alert("Error al cerrar: " + (e.response?.data?.mensaje || e.message));
    }
}
</script>

<template>
  <div v-if="visible" class="modal-overlay">
    <div class="modal-content">
      
      <div class="modal-header">
        <h3>🔒 Cerrar Orden #{{ orden.id }}</h3>
        <button class="btn-close" @click="$emit('close')">×</button>
      </div>

      <div class="modal-body">
        
        <div class="info-bar">
            <span>📦 <strong>Producto:</strong> {{ orden.nombreProducto || orden.producto }}</span>
            <span>👤 <strong>Cliente:</strong> {{ orden.clienteNombre }}</span>
            <span>📅 <strong>Fecha:</strong> {{ orden.fecha ? new Date(orden.fecha).toLocaleDateString() : orden.fechaCreacion }}</span>
        </div>

        <div class="seccion box-resultado">
            <h4>✅ Producción Final (Bobina)</h4>
            <div class="input-group">
                <label>Total Kilos Fabricados:</label>
                <input type="number" v-model="kilosReales" class="input-success" min="0" step="0.1">
            </div>
        </div>

        <div class="seccion" v-if="consumosBase.length > 0">
            <h4>📋 Materiales de la Receta</h4>
            <p class="help-text">Corrige los kilos si el consumo real fue diferente a la teoría.</p>
            
            <div class="tabla-container">
                <table>
                    <thead><tr><th>Insumo</th><th class="text-center">Teórico (Kg)</th><th>Consumo Real (Kg)</th><th></th></tr></thead>
                    <tbody>
                        <tr v-for="(item, idx) in consumosBase" :key="idx">
                            <td>{{ item.nombre }}</td>
                            <td class="text-center" style="color: #7f8c8d;">{{ item.teorico.toFixed(2) }}</td>
                            <td>
                                <input type="number" v-model="item.real" style="width: 100px; padding: 5px;" step="0.1">
                            </td>
                            <td>
                                <span v-if="item.real > (item.stockActual + item.teorico)" class="badge-alerta" title="Quedará en negativo">
                                    ⚠️ Stock
                                </span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <hr class="divider">

        <div class="seccion">
            <h4>➕ Adiciones Imprevistas (Material Extra)</h4>
            <p class="help-text">Si el operario le agregó algo extra (ej: purga, otro color), anótalo aquí.</p>
            
            <div class="form-row-add">
                <div class="col-grow">
                    <select v-model="adicionSeleccionada">
                        <option value="">-- Seleccionar Insumo --</option>
                        <option v-for="mp in materiasPrimas" :key="mp.id" :value="mp.id">
                            {{ mp.nombre }} (Stock: {{ mp.stockActual }} Kg)
                        </option>
                    </select>
                </div>
                
                <div class="col-fixed-80">
                    <input type="number" v-model="cantidadAdicion" placeholder="Kg" min="0" step="0.1">
                </div>
                
                <div class="col-grow">
                    <input type="text" v-model="motivoAdicion" placeholder="Motivo (ej: Purga)">
                </div>
                
                <div class="col-btn">
                    <button @click="agregarAdicion" class="btn-plus" title="Agregar">+</button>
                </div>
            </div>

            <div class="tabla-container" v-if="formCierre.adiciones.length > 0">
                <table>
                    <thead><tr><th>Insumo Extra</th><th class="text-right">Cant (Kg)</th><th>Motivo</th><th></th></tr></thead>
                    <tbody>
                        <tr v-for="(item, idx) in formCierre.adiciones" :key="idx">
                            <td><span class="badge-extra">Extra</span> {{ item.nombre }}</td>
                            <td class="text-right"><strong>{{ item.cantidad }}</strong></td>
                            <td>{{ item.motivo }}</td>
                            <td class="text-right"><button @click="quitarAdicion(idx)" class="btn-trash">🗑️</button></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

        <hr class="divider">

        <div class="seccion-flex">
            <div class="box-desperdicio">
                <h4>⚠️ Desperdicio / Scrap</h4>
                <div class="input-group">
                    <label>Total Kilos:</label>
                    <input type="number" v-model="formCierre.kilosDesperdicio" class="input-warning" min="0" step="0.1">
                </div>
            </div>

            <div class="box-obs">
                <h4>📝 Observaciones</h4>
                <textarea v-model="formCierre.observacionCierre" rows="2" placeholder="Comentarios del cierre..."></textarea>
            </div>
        </div>

      </div>

      <div class="modal-footer">
        <button class="btn-cancelar" @click="$emit('close')">Cancelar</button>
        <button class="btn-confirmar" @click="confirmarCierre">✅ Confirmar Cierre</button>
      </div>

    </div>
  </div>
</template>

<style scoped>
/* Conserve el 100% de tu CSS original y solo agregué dos clasecitas para los detalles nuevos */
.modal-overlay { 
    position: fixed; top: 0; left: 0; width: 100%; height: 100%; 
    background: rgba(0,0,0,0.6); backdrop-filter: blur(2px);
    display: flex; justify-content: center; align-items: center; 
    z-index: 2000; 
}
.modal-content { 
    background: white; width: 800px; max-width: 95vw; max-height: 90vh; 
    border-radius: 8px; box-shadow: 0 10px 30px rgba(0,0,0,0.4); 
    display: flex; flex-direction: column; font-family: 'Segoe UI', sans-serif;
}
.modal-header { 
    background: #2c3e50; color: white; padding: 15px 20px; 
    display: flex; justify-content: space-between; align-items: center; border-radius: 8px 8px 0 0;
}
.modal-header h3 { margin: 0; font-size: 1.2rem; }
.btn-close { background: none; border: none; color: white; font-size: 24px; cursor: pointer; }
.modal-body { padding: 20px; overflow-y: auto; }
.info-bar { 
    background: #f1f2f6; padding: 10px 15px; border-radius: 6px; 
    display: flex; flex-wrap: wrap; gap: 20px; font-size: 0.9rem; color: #333; margin-bottom: 20px;
    border-left: 4px solid #3498db;
}
.seccion h4 { margin: 0 0 5px 0; color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.help-text { margin: 0 0 15px 0; font-size: 0.85rem; color: #7f8c8d; font-style: italic; }
.form-row-add { display: flex; gap: 10px; align-items: center; background: #eef2f5; padding: 12px; border-radius: 6px; }
.col-grow { flex: 1; } 
.col-fixed-80 { width: 80px; } 
.col-btn { width: 40px; }
select, input, textarea { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }
.btn-plus { background: #27ae60; color: white; border: none; width: 100%; height: 38px; border-radius: 4px; cursor: pointer; font-size: 1.2rem; display: flex; align-items: center; justify-content: center; }
.btn-plus:hover { background: #2ecc71; }
.tabla-container { margin-top: 15px; border: 1px solid #eee; border-radius: 4px; overflow: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
th { background: #dfe6e9; padding: 8px; text-align: left; color: #2d3436; }
td { padding: 8px; border-bottom: 1px solid #f1f1f1; }
.text-right { text-align: right; }
.text-center { text-align: center; }
.btn-trash { background: none; border: none; cursor: pointer; font-size: 1.1rem; }
.divider { border: 0; border-top: 1px dashed #ccc; margin: 25px 0; }
.seccion-flex { display: flex; gap: 20px; }
.box-desperdicio { flex: 1; background: #fff8e1; padding: 15px; border-radius: 6px; border: 1px solid #ffe0b2; }
.box-obs { flex: 2; }
.input-warning { border: 1px solid #f57c00; font-weight: bold; color: #e65100; }
.input-group { display: flex; gap: 10px; align-items: center; margin-top: 10px; }
.box-desperdicio h4 { color: #e65100; border-bottom-color: #ffe0b2; }

/* NUEVAS CLASES AGREGADAS */
.box-resultado { background: #e8f5e9; padding: 15px; border-radius: 6px; border: 1px solid #c8e6c9; margin-bottom: 20px; }
.box-resultado h4 { color: #2e7d32; border-bottom-color: #c8e6c9; }
.input-success { border: 1px solid #4caf50; font-weight: bold; color: #2e7d32; font-size: 1.1em; }
.badge-alerta { background: #e74c3c; color: white; padding: 3px 6px; border-radius: 4px; font-size: 0.8em; }
.badge-extra { background: #3498db; color: white; padding: 2px 5px; border-radius: 4px; font-size: 0.75em; margin-right: 5px; }

.modal-footer { padding: 15px 20px; background: #f8f9fa; border-top: 1px solid #eee; display: flex; justify-content: flex-end; gap: 10px; border-radius: 0 0 8px 8px; }
.btn-cancelar { padding: 10px 20px; border: 1px solid #ccc; background: white; border-radius: 4px; cursor: pointer; }
.btn-confirmar { padding: 10px 25px; border: none; background: #2c3e50; color: white; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-confirmar:hover { background: #34495e; }

@media (max-width: 850px) {
    .modal-content { width: 98%; }
    .form-row-add { flex-direction: column; align-items: stretch; }
    .col-fixed-80, .col-btn { width: 100%; }
    .seccion-flex { flex-direction: column; }
}
</style>