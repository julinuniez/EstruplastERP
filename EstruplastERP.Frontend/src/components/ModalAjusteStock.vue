<script setup lang="ts">
import { ref, reactive, watch } from 'vue';
import api from '@/services/axiosInstance';

const props = defineProps<{
    visible: boolean;
    producto: any;
}>();

const emit = defineEmits(['close', 'confirmado']);

const form = reactive({
    tipo: 'EGRESO',
    cantidad: 0,
    motivo: 'Material Malo / Descarte',
    notas: ''
});

const guardando = ref(false);

// Reiniciar formulario al abrir el modal
watch(() => props.visible, (newVal) => {
    if (newVal) {
        form.tipo = 'EGRESO';
        form.cantidad = 0;
        form.motivo = 'Material Malo / Descarte';
        form.notas = '';
    }
});

const ejecutarAjuste = async () => {
    if (!form.cantidad || form.cantidad <= 0) {
        return alert("⚠️ Por favor ingresa una cantidad mayor a cero.");
    }
    
    // Validar stock negativo si es un egreso
    if (form.tipo === 'EGRESO' && form.cantidad > (props.producto.stockFisico ?? props.producto.stockActual)) {
        if(!confirm(`⚠️ La cantidad a descontar (${form.cantidad}kg) es mayor al stock físico actual. ¿Estás seguro de continuar y dejar stock negativo?`)) return;
    }

    guardando.value = true;
    try {
        // Apuntamos a la ruta exacta de tu controlador
        await api.post('/Movimientos/ajuste', {
            productoId: props.producto.id,
            cantidad: Number(form.cantidad),
            tipoMovimiento: form.tipo,
            observacion: `${form.motivo} - ${form.notas}`
        });
        
        emit('confirmado');
        emit('close');
    } catch (e: any) {
        alert("❌ Error al ajustar el stock: " + (e.response?.data || e.message));
    } finally {
        guardando.value = false;
    }
};
</script>

<template>
    <div v-if="visible" class="modal-overlay" @click.self="emit('close')">
        <div class="modal-content">
            <div class="modal-header">
                <h3>⚖️ Ajustar Stock Manual</h3>
                <button class="btn-close" @click="emit('close')">✕</button>
            </div>
            
            <div class="info-producto">
                <span class="badge-sku">{{ producto?.codigoSku }}</span>
                <p class="nombre-producto">{{ producto?.nombre }}</p>
                <p class="stock-actual">Stock Físico Actual: <strong>{{ (producto?.stockFisico ?? producto?.stockActual ?? 0).toFixed(2) }} kg</strong></p>
            </div>

            <div class="form-container">
                <div class="form-group">
                    <label>Tipo de Operación:</label>
                    <div class="selector-tipo">
                        <button :class="{ active: form.tipo === 'INGRESO' }" @click="form.tipo = 'INGRESO'" class="btn-ingreso">➕ Ingresar (+)</button>
                        <button :class="{ active: form.tipo === 'EGRESO' }" @click="form.tipo = 'EGRESO'" class="btn-egreso">➖ Descontar (-)</button>
                    </div>
                </div>

                <div class="form-group row-flex">
                    <div style="flex: 1;">
                        <label>Motivo del Ajuste:</label>
                        <select v-model="form.motivo" class="input-modern">
                            <option value="Material Malo / Descarte">Material Malo / Descarte</option>
                            <option value="Scrap / Devolución a Molienda">Scrap de Producción</option>
                            <option value="Diferencia de Inventario">Diferencia de Inventario (Conteo)</option>
                            <option value="Ingreso Manual Extra">Ingreso Manual Extra</option>
                            <option value="Otro">Otro Motivo</option>
                        </select>
                    </div>
                    <div style="width: 120px;">
                        <label>Cantidad (Kg):</label>
                        <input type="number" v-model="form.cantidad" class="input-modern cantidad-input" min="0" step="0.01">
                    </div>
                </div>

                <div class="form-group">
                    <label>Nota / Referencia (Opcional):</label>
                    <textarea v-model="form.notas" class="input-modern" rows="2" placeholder="Ej: Lote dañado, bolsas rotas, conteo de fin de mes..."></textarea>
                </div>
            </div>

            <div class="modal-acciones">
                <button class="btn-cancelar" @click="emit('close')">Cancelar</button>
                <button class="btn-guardar" @click="ejecutarAjuste" :disabled="guardando || form.cantidad <= 0">
                    <span v-if="guardando">⏳ Procesando...</span>
                    <span v-else>💾 Guardar Ajuste</span>
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 10000; }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 500px; max-width: 95vw; box-shadow: 0 10px 30px rgba(0,0,0,0.3); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #eee; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #2c3e50; font-size: 1.3rem; }
.btn-close { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #95a5a6; }
.btn-close:hover { color: #e74c3c; }
.info-producto { background: #f8f9fa; padding: 15px; border-radius: 8px; margin-bottom: 20px; border-left: 4px solid #3498db; }
.badge-sku { background: #e0e0e0; padding: 2px 6px; border-radius: 4px; font-size: 0.75rem; font-weight: bold; font-family: monospace; color: #555; }
.nombre-producto { font-weight: bold; color: #2c3e50; margin: 5px 0; font-size: 1.1rem; }
.stock-actual { margin: 0; font-size: 0.9rem; color: #7f8c8d; }
.form-group { margin-bottom: 15px; }
.form-group label { display: block; font-weight: bold; color: #555; margin-bottom: 5px; font-size: 0.85rem; }
.row-flex { display: flex; gap: 15px; }
.selector-tipo { display: flex; gap: 10px; }
.selector-tipo button { flex: 1; padding: 10px; border: 2px solid #eee; border-radius: 6px; font-weight: bold; cursor: pointer; background: white; color: #7f8c8d; transition: all 0.2s; }
.selector-tipo button.btn-ingreso.active { border-color: #27ae60; background: #e9f7ef; color: #27ae60; }
.selector-tipo button.btn-egreso.active { border-color: #e74c3c; background: #fdedec; color: #e74c3c; }
.input-modern { width: 100%; padding: 10px; border: 1px solid #ccc; border-radius: 6px; font-family: inherit; font-size: 0.95rem; box-sizing: border-box; }
.input-modern:focus { border-color: #3498db; outline: none; box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2); }
.cantidad-input { font-weight: bold; color: #2c3e50; font-size: 1.1rem; text-align: center; }
.modal-acciones { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; border-top: 1px solid #eee; padding-top: 15px; }
.btn-cancelar { background: #f1f2f6; color: #7f8c8d; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-cancelar:hover { background: #e2e6ea; }
.btn-guardar { background: #2c3e50; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-guardar:hover:not(:disabled) { background: #1a252f; }
.btn-guardar:disabled { background: #95a5a6; cursor: not-allowed; }
</style>