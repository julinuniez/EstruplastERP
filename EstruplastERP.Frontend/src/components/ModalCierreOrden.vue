<script setup lang="ts">
import { ref } from 'vue'
import axios from 'axios'

const props = defineProps<{
  orden: any,
  materiasPrimas: any[],
  visible: boolean
}>()

const emit = defineEmits(['close', 'confirmar'])

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

const formCierre = ref({
  adiciones: [] as { materiaPrimaId: number, nombre: string, cantidad: number, motivo: string }[],
  kilosDesperdicio: 0,
  observacionCierre: ''
})

const adicionSeleccionada = ref<number | ''>('')
const cantidadAdicion = ref<number | ''>('')
const motivoAdicion = ref('')

const agregarAdicion = () => {
    if (!adicionSeleccionada.value || !cantidadAdicion.value || Number(cantidadAdicion.value) <= 0) {
        alert("Ingrese datos válidos.");
        return;
    }

    const mp = props.materiasPrimas.find(m => m.id === adicionSeleccionada.value);
    
    if (mp) {
        if (mp.stockActual < Number(cantidadAdicion.value)) {
            alert(`⚠️ CUIDADO: No hay suficiente stock de ${mp.nombre}.\nStock Actual: ${mp.stockActual} Kg\nIntentas restar: ${cantidadAdicion.value} Kg`);
            return; 
        }

        formCierre.value.adiciones.push({
            materiaPrimaId: mp.id,
            nombre: mp.nombre,
            cantidad: Number(cantidadAdicion.value),
            motivo: motivoAdicion.value || 'Ajuste'
        });
        
        // Limpiar inputs
        adicionSeleccionada.value = '';
        cantidadAdicion.value = '';
        motivoAdicion.value = '';
    }
}

const quitarAdicion = (index: number) => {
    formCierre.value.adiciones.splice(index, 1);
}

const confirmarCierre = async () => {
    const totalAdiciones = formCierre.value.adiciones.reduce((sum, item) => sum + item.cantidad, 0);
    const mensaje = `¿Cerrar orden?\n\nSe descontarán ${totalAdiciones.toFixed(2)} Kg extra de Stock.`;

    if (!confirm(mensaje)) return;

    try {
        const payload = {
            ordenId: props.orden.id,
            adiciones: formCierre.value.adiciones,
            desperdicio: formCierre.value.kilosDesperdicio,
            observacion: formCierre.value.observacionCierre
        };

        // Ajusta la URL según tu backend real
        await axios.post(`${apiUrl}/Ordenes/finalizar/${props.orden.id}`, payload, {
             headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } 
        });

        emit('confirmar');
        emit('close');

    } catch (e: any) {
        alert("Error: " + (e.response?.data?.mensaje || e.message));
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
            <span>📦 <strong>Producto:</strong> {{ orden.nombreProducto }}</span>
            <span>👤 <strong>Operario:</strong> {{ orden.nombreEmpleado }}</span>
            <span>📅 <strong>Fecha:</strong> {{ new Date(orden.fecha).toLocaleDateString() }}</span>
        </div>

        <div class="seccion">
            <h4>➕ Adiciones (Material Extra)</h4>
            <p class="help-text">Si el operario anotó agregados manuales en la hoja, cárgalos aquí.</p>
            
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
                    <thead><tr><th>Insumo</th><th class="text-right">Cant (Kg)</th><th>Motivo</th><th></th></tr></thead>
                    <tbody>
                        <tr v-for="(item, idx) in formCierre.adiciones" :key="idx">
                            <td>{{ item.nombre }}</td>
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
                    <input type="number" v-model="formCierre.kilosDesperdicio" class="input-warning" min="0">
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
/* FONDO OSCURO DETRÁS */
.modal-overlay { 
    position: fixed; top: 0; left: 0; width: 100%; height: 100%; 
    background: rgba(0,0,0,0.6); backdrop-filter: blur(2px);
    display: flex; justify-content: center; align-items: center; /* Centrado perfecto */
    z-index: 2000; 
}

/* LA VENTANA DEL MODAL */
.modal-content { 
    background: white; 
    width: 800px; /* ANCHO FIJO GENEROSO */
    max-width: 95vw; /* Si la pantalla es chica, usa el 95% */
    max-height: 90vh; /* No más alto que el 90% de la pantalla */
    border-radius: 8px; 
    box-shadow: 0 10px 30px rgba(0,0,0,0.4); 
    display: flex; flex-direction: column; /* Para que el header y footer queden fijos */
    font-family: 'Segoe UI', sans-serif;
}

/* HEADER */
.modal-header { 
    background: #2c3e50; color: white; padding: 15px 20px; 
    display: flex; justify-content: space-between; align-items: center; 
    border-radius: 8px 8px 0 0;
}
.modal-header h3 { margin: 0; font-size: 1.2rem; }
.btn-close { background: none; border: none; color: white; font-size: 24px; cursor: pointer; }

/* BODY CON SCROLL INTERNO */
.modal-body { 
    padding: 20px; 
    overflow-y: auto; /* Si es muy alto, scrollea solo el contenido */
}

/* BARRA INFO */
.info-bar { 
    background: #f1f2f6; padding: 10px 15px; border-radius: 6px; 
    display: flex; flex-wrap: wrap; gap: 20px; font-size: 0.9rem; color: #333; margin-bottom: 20px;
    border-left: 4px solid #3498db;
}

/* FORMULARIOS */
.seccion h4 { margin: 0 0 5px 0; color: #2c3e50; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.help-text { margin: 0 0 15px 0; font-size: 0.85rem; color: #7f8c8d; font-style: italic; }

/* LA FILA DE AGREGAR (FLEXBOX PARA QUE ENTRE TODO) */
.form-row-add { 
    display: flex; gap: 10px; align-items: center; background: #eef2f5; padding: 12px; border-radius: 6px; 
}
.col-grow { flex: 1; } /* Ocupa todo el espacio disponible */
.col-fixed-80 { width: 80px; } /* Ancho fijo para cantidad */
.col-btn { width: 40px; }

/* INPUTS */
select, input, textarea { 
    width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; 
}
.btn-plus { 
    background: #27ae60; color: white; border: none; width: 100%; height: 38px; 
    border-radius: 4px; cursor: pointer; font-size: 1.2rem; display: flex; align-items: center; justify-content: center;
}
.btn-plus:hover { background: #2ecc71; }

/* TABLA */
.tabla-container { margin-top: 15px; border: 1px solid #eee; border-radius: 4px; overflow: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
th { background: #dfe6e9; padding: 8px; text-align: left; color: #2d3436; }
td { padding: 8px; border-bottom: 1px solid #f1f1f1; }
.text-right { text-align: right; }
.btn-trash { background: none; border: none; cursor: pointer; font-size: 1.1rem; }

.divider { border: 0; border-top: 1px dashed #ccc; margin: 25px 0; }

/* SECCIÓN ABAJO (Dos columnas: Desperdicio y Obs) */
.seccion-flex { display: flex; gap: 20px; }
.box-desperdicio { flex: 1; background: #fff8e1; padding: 15px; border-radius: 6px; border: 1px solid #ffe0b2; }
.box-obs { flex: 2; }

.input-warning { border: 1px solid #f57c00; font-weight: bold; color: #e65100; }
.input-group { display: flex; gap: 10px; align-items: center; margin-top: 10px; }
.box-desperdicio h4 { color: #e65100; border-bottom-color: #ffe0b2; }

/* FOOTER */
.modal-footer { 
    padding: 15px 20px; background: #f8f9fa; border-top: 1px solid #eee; 
    display: flex; justify-content: flex-end; gap: 10px; 
    border-radius: 0 0 8px 8px;
}
.btn-cancelar { padding: 10px 20px; border: 1px solid #ccc; background: white; border-radius: 4px; cursor: pointer; }
.btn-confirmar { padding: 10px 25px; border: none; background: #2c3e50; color: white; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-confirmar:hover { background: #34495e; }

/* RESPONSIVE (Si la pantalla es chica, apila todo) */
@media (max-width: 850px) {
    .modal-content { width: 98%; }
    .form-row-add { flex-direction: column; align-items: stretch; }
    .col-fixed-80, .col-btn { width: 100%; }
    .seccion-flex { flex-direction: column; }
}
</style>