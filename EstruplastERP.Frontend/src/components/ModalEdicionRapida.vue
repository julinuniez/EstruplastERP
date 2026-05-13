<script setup lang="ts">
import { ref, watch } from 'vue'
import api from '@/services/axiosInstance'
import type { ProduccionItem } from './ListaProduccion.vue'

const props = defineProps<{
    visible: boolean;
    ordenEditando: ProduccionItem | null;
}>();

const emit = defineEmits(['close', 'guardado']);

const guardandoEdicion = ref(false);

const formEdicion = ref({
    largo: 0, ancho: 0, espesor: 0, cantidad: 0, kilosTotales: 0, desperdicio: 8,
    conBrillo: false, llevaFilm: false, tipoCorona: 'Ninguno', color: '', ignorarStock: true, recetaNueva: [] as any[]
});

watch(() => props.visible, (newVal) => {
    if (newVal && props.ordenEditando) {
        const o = props.ordenEditando;
        formEdicion.value = {
            largo: o.largo || 0, ancho: o.ancho || 0, espesor: o.espesor || 0,
            cantidad: o.cantidad || 0, kilosTotales: o.kilos || 0, desperdicio: o.desperdicio || 8,
            conBrillo: o.conBrillo || false, llevaFilm: o.llevaFilm || false, tipoCorona: o.tipoCorona || 'Ninguno',
            color: o.color || '', ignorarStock: true, recetaNueva: JSON.parse(JSON.stringify(o.consumos || []))
        };
    }
});

const recalcularModal = () => {
    if (!props.ordenEditando) return;
    const o = props.ordenEditando;
    
    const oldVol = (o.largo || 1) * (o.ancho || 1) * (o.espesor || 1) * (o.cantidad || 1);
    const newVol = (formEdicion.value.largo || 1) * (formEdicion.value.ancho || 1) * (formEdicion.value.espesor || 1) * (formEdicion.value.cantidad || 1);
    
    let ratio = 1;
    if (oldVol > 0) ratio = newVol / oldVol;

    formEdicion.value.kilosTotales = Number((o.kilos * ratio).toFixed(2));
    formEdicion.value.recetaNueva = (o.consumos || []).map((c: any) => ({
        materiaPrimaId: c.materiaPrimaId,
        cantidadKilos: Number((c.cantidadKilos * ratio).toFixed(2))
    }));
};

async function guardarEdicionRapida() {
    if (!props.ordenEditando) return;
    guardandoEdicion.value = true;
    try {
        await api.put(`/Ordenes/modificar/${props.ordenEditando.id}`, formEdicion.value);
        emit('guardado');
    } catch (e: any) {
        alert(e.response?.data?.mensaje || e.response?.data || "Error al modificar la orden.");
    } finally {
        guardandoEdicion.value = false;
    }
}
</script>

<template>
    <div v-if="visible" class="modal-overlay" @click.self="emit('close')">
        <div class="modal-content">
            <div class="modal-header">
                <h3>✏️ Edición Rápida | Orden #{{ ordenEditando?.id }}</h3>
                <button class="btn-close" @click="emit('close')">✕</button>
            </div>
            
            <p class="warning-text">⚠️ Al modificar, la orden perderá su estado de "Impresa" y se recalculará el stock.</p>

            <div class="grid-medidas">
                <div class="campo-rapido">
                    <label>Largo (mm)</label>
                    <input type="number" v-model="formEdicion.largo" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Ancho (mm)</label>
                    <input type="number" v-model="formEdicion.ancho" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Espesor</label>
                    <input type="number" v-model="formEdicion.espesor" step="0.01" @input="recalcularModal">
                </div>
                <div class="campo-rapido">
                    <label>Cantidad</label>
                    <input type="number" v-model="formEdicion.cantidad" min="1" @input="recalcularModal">
                </div>
            </div>

            <div class="kilos-recalculados">
                Nuevo Peso Estimado: <strong>{{ formEdicion.kilosTotales }} kg</strong>
            </div>

            <h4 class="titulo-seccion">Aditivos & Opciones</h4>
            <div class="grid-aditivos">
                <label class="switch-label"><input type="checkbox" v-model="formEdicion.conBrillo" @change="recalcularModal"> ✨ Brillo</label>
                <label class="switch-label"><input type="checkbox" v-model="formEdicion.llevaFilm" @change="recalcularModal"> 🛡️ Lleva Film</label>
            </div>
            
            <div class="grid-aditivos" style="margin-top: 10px;">
                <label class="switch-label" style="color: #b45309; font-weight: bold;">
                    <input type="checkbox" v-model="formEdicion.ignorarStock"> ⚠️ Forzar guardado sin stock
                </label>
            </div>

            <div class="grid-medidas" style="margin-top: 15px;">
                <div class="campo-rapido">
                    <label>⚡ Tratamiento Corona:</label>
                    <select v-model="formEdicion.tipoCorona" @change="recalcularModal">
                        <option value="Ninguno">Ninguno</option>
                        <option value="Simple">Simple</option>
                        <option value="Doble">Doble</option>
                    </select>
                </div>
                <div class="campo-rapido">
                    <label>🎨 Color:</label>
                    <input type="text" v-model="formEdicion.color" placeholder="Ej: BLANCO" @input="recalcularModal">
                </div>
            </div>

            <div class="modal-footer">
                <button class="btn-cancelar" @click="emit('close')">Cancelar</button>
                <button class="btn-guardar" @click="guardarEdicionRapida" :disabled="guardandoEdicion">
                    {{ guardandoEdicion ? 'Guardando...' : '💾 Confirmar Cambios' }}
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; backdrop-filter: blur(2px); }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 450px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #f1f5f9; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #1e293b; font-size: 1.2rem; }
.btn-close { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #94a3b8; }
.btn-close:hover { color: #ef4444; }

.warning-text { font-size: 0.85rem; color: #b45309; background: #fef3c7; padding: 10px; border-radius: 6px; border-left: 4px solid #f59e0b; margin-bottom: 20px; }

.grid-medidas { display: grid; grid-template-columns: 1fr 1fr; gap: 15px; margin-bottom: 15px; }
.campo-rapido label { display: block; font-size: 0.85rem; font-weight: 600; color: #475569; margin-bottom: 5px; }
.campo-rapido input, .campo-rapido select { width: 100%; padding: 8px 10px; border: 1px solid #cbd5e1; border-radius: 6px; box-sizing: border-box; }
.campo-rapido input:focus, .campo-rapido select:focus { outline: none; border-color: #3b82f6; }

input[type=number]::-webkit-inner-spin-button,
input[type=number]::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
}
input[type=number] {
  -moz-appearance: textfield;
  appearance: textfield;
}

.kilos-recalculados { text-align: center; background: #eff6ff; color: #1d4ed8; padding: 10px; border-radius: 6px; font-size: 1.1rem; margin-bottom: 20px; border: 1px dashed #93c5fd; }

.titulo-seccion { font-size: 0.95rem; color: #334155; border-bottom: 1px solid #e2e8f0; padding-bottom: 5px; margin-bottom: 10px; }
.grid-aditivos { display: grid; grid-template-columns: 1fr 1fr; gap: 10px; }
.switch-label { display: flex; align-items: center; gap: 8px; font-size: 0.9rem; color: #475569; cursor: pointer; }

.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; border-top: 1px solid #f1f5f9; padding-top: 15px; }
.btn-cancelar { background: white; border: 1px solid #cbd5e1; color: #475569; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-cancelar:hover { background: #f8fafc; }
.btn-guardar { background: #3b82f6; color: white; border: none; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-guardar:hover:not(:disabled) { background: #2563eb; }
.btn-guardar:disabled { background: #94a3b8; cursor: not-allowed; }
</style>