<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import axios from 'axios'
import { Alertas } from '@/utils/alertas';

const props = defineProps<{
    visible: boolean
}>()

const emit = defineEmits(['close', 'creado'])

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';

const form = ref({
    nombreColor: '',
    codigoPersonalizado: '',
    stockInicial: 0,
    proveedorId: '' as number | ''
})

const proveedores = ref<any[]>([])
const procesando = ref(false)

const cargarProveedores = async () => {
    try {
        const res = await axios.get(`${apiUrl}/Proveedores`, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        proveedores.value = res.data;
    } catch (error) {
        console.error("No se pudieron cargar los proveedores.", error);
    }
}

watch(() => props.visible, (isOpen) => {
    if (isOpen) {
        form.value = { nombreColor: '', codigoPersonalizado: '', stockInicial: 0, proveedorId: '' }
    }
})

onMounted(() => {
    cargarProveedores();
})

const guardarColor = async () => {
    if (!form.value.nombreColor.trim()) {
        Alertas.advertencia("El nombre del color es obligatorio.");
        return;
    }

    procesando.value = true;
    try {
        const payload = {
            ...form.value,
            proveedorId: form.value.proveedorId ? Number(form.value.proveedorId) : null
        };

        await axios.post(`${apiUrl}/Productos/crear-masterbatch`, payload, {
            headers: { Authorization: `Bearer ${localStorage.getItem('token')}` }
        });
        
        Alertas.exito("🎨 Color registrado en el inventario.");
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
                <h3>🎨 Nuevo Color Masterbatch</h3>
                <button class="btn-close" @click="$emit('close')">×</button>
            </div>
            
            <div class="modal-body">
                <div class="input-group">
                    <label>Nombre del Color (Ej: Rojo Ferrari)</label>
                    <input type="text" v-model="form.nombreColor" placeholder="Describa el color..." autofocus>
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
                        <input type="text" v-model="form.codigoPersonalizado" placeholder="Auto-generar">
                    </div>
                    <div class="input-group flex-1">
                        <label>Stock Inicial (Kg)</label>
                        <input type="number" v-model="form.stockInicial" min="0" step="0.1">
                    </div>
                </div>
            </div>
            
            <div class="modal-footer">
                <button class="btn-cancelar" @click="$emit('close')" :disabled="procesando">Cancelar</button>
                <button class="btn-confirmar" @click="guardarColor" :disabled="procesando">
                    {{ procesando ? '⏳ Guardando...' : '✅ Crear Masterbatch' }}
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 500px; max-width: 95vw; box-shadow: 0 10px 25px rgba(0,0,0,0.2); box-sizing: border-box; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #8b5cf6; padding-bottom: 10px; margin-bottom: 20px; }
.modal-header h3 { margin: 0; color: #2c3e50; }
.btn-close { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #7f8c8d; }
.input-group { margin-bottom: 15px; display: flex; flex-direction: column; gap: 5px; }
.input-group label { font-weight: 600; color: #475569; font-size: 0.9rem; }

/* 🚀 ACÁ ESTÁ LA MAGIA DEL BOX-SIZING */
.input-group input, .input-group select { 
    width: 100%; 
    box-sizing: border-box; 
    padding: 10px; 
    border: 1px solid #cbd5e1; 
    border-radius: 6px; 
    font-size: 1rem; 
}

.input-group input:focus, .input-group select:focus { border-color: #8b5cf6; outline: none; box-shadow: 0 0 0 2px rgba(139, 92, 246, 0.2); }
.form-row { display: flex; gap: 15px; }
.flex-1 { flex: 1; }
.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 10px; padding-top: 15px; border-top: 1px solid #e2e8f0; }
.btn-cancelar { background: #f1f5f9; color: #475569; border: 1px solid #cbd5e1; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-confirmar { background: #8b5cf6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-confirmar:hover:not(:disabled) { background: #7c3aed; }
.btn-confirmar:disabled { opacity: 0.7; cursor: not-allowed; }
</style>