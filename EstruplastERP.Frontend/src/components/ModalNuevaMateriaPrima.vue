<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import api from '@/services/axiosInstance'
import { Alertas } from '@/utils/alertas';

const props = defineProps<{
    visible: boolean
}>()

const emit = defineEmits(['close', 'creado'])

const form = ref({
    nombre: '',
    codigoSku: '',
    categoriaInsumoId: '' as number | '',
    stockInicial: 0,
    proveedorId: '' as number | ''
})

const proveedores = ref<any[]>([])
const procesando = ref(false)

const cargarProveedores = async () => {
    try {
        const res = await api.get('/Proveedores');
        proveedores.value = res.data;
    } catch (error) {
        console.error("No se pudieron cargar los proveedores.", error);
    }
}

watch(() => props.visible, (isOpen) => {
    if (isOpen) {
        form.value = { nombre: '', codigoSku: '', categoriaInsumoId: '', stockInicial: 0, proveedorId: '' }
    }
})

onMounted(() => {
    cargarProveedores();
})

const guardarMateriaPrima = async () => {
    if (!form.value.nombre.trim()) {
        Alertas.advertencia("El nombre del insumo es obligatorio.");
        return;
    }
    if (!form.value.categoriaInsumoId) {
        Alertas.advertencia("Debe seleccionar el rol lógico del insumo en la receta.");
        return;
    }

    procesando.value = true;
    try {
        const payload = {
            ...form.value,
            proveedorId: form.value.proveedorId ? Number(form.value.proveedorId) : null,
            categoriaInsumoId: Number(form.value.categoriaInsumoId)
        };

        // 🚨 Ajustá esta ruta según el endpoint que uses en tu backend para Materias Primas
        await api.post('/Productos/crear-insumo', payload);
        
        Alertas.exito("🧪 Insumo registrado en el inventario.");
        emit('creado');
        emit('close');
    } catch (e: any) {
        Alertas.error("Error al crear: " + (e.response?.data?.mensaje || e.message));
    } finally {
        procesando.value = false;
    }
}
</script>

<template>
    <div v-if="visible" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>➕ Nuevo Insumo / Materia Prima</h3>
                <button class="btn-close" @click="$emit('close')">×</button>
            </div>
            
            <div class="modal-body">
                <div class="input-group">
                    <label>Nombre del Material <span style="color:red">*</span></label>
                    <input type="text" v-model="form.nombre" placeholder="Ej: PEAD INYECCIÓN" autofocus>
                </div>

                <div class="input-group">
                    <label>Rol Lógico del Insumo <span style="color:red">*</span></label>
                    <select v-model="form.categoriaInsumoId">
                        <option value="" disabled>-- Seleccionar Rol --</option>
                        <option :value="1">Materia Prima Virgen (BASE)</option>
                        <option :value="3">Aditivo UV/Deslizante (DEPENDIENTE)</option>
                        <option :value="5">Otros / Insumos Generales</option>
                    </select>
                </div>

                <div class="input-group">
                    <label>Proveedor (Opcional)</label>
                    <select v-model="form.proveedorId">
                        <option value="">Seleccionar Proveedor</option>
                        <option v-for="prov in proveedores" :key="prov.id" :value="prov.id">
                            {{ prov.razonSocial }}
                        </option>
                    </select>
                </div>
                
                <div class="form-row">
                    <div class="input-group flex-1">
                        <label>Código SKU (Opcional)</label>
                        <input type="text" v-model="form.codigoSku" placeholder="Auto-generar">
                    </div>
                    <div class="input-group flex-1">
                        <label>Stock Físico Inicial (Kg)</label>
                        <input type="number" v-model="form.stockInicial" min="0" step="0.1">
                    </div>
                </div>
            </div>
            
            <div class="modal-footer">
                <button class="btn-cancelar" @click="$emit('close')" :disabled="procesando">Cancelar</button>
                <button class="btn-confirmar-mp" @click="guardarMateriaPrima" :disabled="procesando">
                    {{ procesando ? '⏳ Guardando...' : '💾 Guardar Insumo' }}
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 500px; max-width: 95vw; box-shadow: 0 10px 25px rgba(0,0,0,0.2); box-sizing: border-box; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #3b82f6; padding-bottom: 10px; margin-bottom: 20px; }
.modal-header h3 { margin: 0; color: #1e293b; }
.btn-close { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #94a3b8; }
.input-group { margin-bottom: 15px; display: flex; flex-direction: column; gap: 5px; }
.input-group label { font-weight: 600; color: #475569; font-size: 0.9rem; }
.input-group input, .input-group select { 
    width: 100%; 
    box-sizing: border-box; 
    padding: 10px; 
    border: 1px solid #cbd5e1; 
    border-radius: 6px; 
    font-size: 1rem; 
}
.input-group input:focus, .input-group select:focus { border-color: #3b82f6; outline: none; box-shadow: 0 0 0 2px rgba(59, 130, 246, 0.2); }
.form-row { display: flex; gap: 15px; }
.flex-1 { flex: 1; }
.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 10px; padding-top: 15px; border-top: 1px solid #e2e8f0; }
.btn-cancelar { background: #f1f5f9; color: #475569; border: 1px solid #cbd5e1; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-confirmar-mp { background: #3b82f6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-confirmar-mp:hover:not(:disabled) { background: #2563eb; }
.btn-confirmar-mp:disabled { opacity: 0.7; cursor: not-allowed; }
</style>