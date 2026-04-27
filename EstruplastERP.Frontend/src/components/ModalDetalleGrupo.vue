<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import axios from 'axios'

const props = defineProps<{
    visible: boolean,
    codigo: string,
    ordenes: any[]
}>()

const emit = defineEmits(['close', 'actualizar-lista'])

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';

const procesando = ref(false);
const consumosMezcla = ref<{ materiaPrimaId: number, nombre: string, teorico: number, real: number }[]>([])

// Verificamos si este grupo ya fue procesado (Si las órdenes ya no son "Pendiente")
const yaEstaDeclarado = computed(() => {
    if (props.ordenes.length === 0) return false;
    return props.ordenes.some(o => o.estado === 'MaterialPreparado' || o.estado === 'Finalizada');
});

// Obtenemos el ID real de la base de datos de este grupo
const hojaCargaId = computed(() => {
    if (props.ordenes.length === 0) return null;
    return props.ordenes[0].hojaCargaId;
});

watch(() => props.visible, (isOpen) => {
    if (isOpen && props.ordenes.length > 0) {
        // Agrupamos la suma total de todas las materias primas de las 5 órdenes
        const map = new Map<number, any>();
        
        props.ordenes.forEach(o => {
            if (o.consumos) {
                o.consumos.forEach((c: any) => {
                    if (!map.has(c.materiaPrimaId)) {
                        map.set(c.materiaPrimaId, { 
                            materiaPrimaId: c.materiaPrimaId, 
                            nombre: c.nombreMateriaPrima, 
                            teorico: 0, 
                            real: 0 
                        });
                    }
                    map.get(c.materiaPrimaId).teorico += Number(c.cantidadKilos);
                });
            }
        });
        
        consumosMezcla.value = Array.from(map.values()).map(c => {
            c.real = Number(c.teorico.toFixed(2)); // Sugerimos el teórico por defecto
            return c;
        });
    }
});

const declararConsumos = async () => {
    if (!hojaCargaId.value) {
        alert("Error crítico: La orden no tiene un HojaCargaId válido en la base de datos.");
        return;
    }

    if (!confirm("⚠️ ¿Descontar estos materiales del stock?\n\nLas órdenes de este grupo pasarán a 'Material Preparado' y ya no descontarán stock al ser finalizadas.")) return;

    procesando.value = true;
    try {
        const payload = consumosMezcla.value.map(c => ({
            materiaPrimaId: c.materiaPrimaId,
            cantidadRealKg: Number(c.real)
        }));

        await axios.post(`${apiUrl}/HojasCarga/${hojaCargaId.value}/declarar-consumos`, payload);
        
        alert("✅ Mezcla declarada correctamente.");
        emit('actualizar-lista'); // Le decimos a la tabla que se recargue
        emit('close');
    } catch (e: any) {
        alert("❌ Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        procesando.value = false;
    }
}
</script>

<template>
    <div v-if="visible" class="modal-overlay">
        <div class="modal-content">
            <div class="modal-header">
                <h3>📦 Hoja de Carga: {{ codigo }}</h3>
                <button class="btn-close" @click="$emit('close')">×</button>
            </div>
            
            <div class="modal-body">
                <div v-if="yaEstaDeclarado" class="alerta-ok">
                    ✅ <strong>El material de este grupo ya fue descontado del inventario.</strong><br>
                    Las órdenes están listas para cerrarse a medida que salgan de la máquina.
                </div>
                <div v-else class="alerta-info">
                    ℹ️ <strong>Declaración de Pastón / Mezcla (Fase 1)</strong><br>
                    Ingrese los kilos reales que se usaron para armar este grupo. Esto descontará el stock de planta.
                </div>

                <div class="seccion" v-if="!yaEstaDeclarado && consumosMezcla.length > 0">
                    <h4>⚖️ Consumos del Grupo Completo</h4>
                    <div class="tabla-container">
                        <table>
                            <thead>
                                <tr>
                                    <th>Insumo</th>
                                    <th class="text-center">Suma Teórica (Kg)</th>
                                    <th>Consumo Real (Kg)</th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr v-for="(item, idx) in consumosMezcla" :key="idx">
                                    <td>{{ item.nombre }}</td>
                                    <td class="text-center" style="color: #7f8c8d;">{{ item.teorico.toFixed(2) }}</td>
                                    <td>
                                        <input type="number" v-model="item.real" style="width: 120px; padding: 5px; font-weight: bold;" step="0.1">
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>

                <div class="seccion">
                    <h4>📄 Órdenes incluidas en este grupo ({{ ordenes.length }})</h4>
                    <ul class="lista-ordenes">
                        <li v-for="o in ordenes" :key="o.id">
                            <strong>OP #{{ o.id }}</strong> - {{ o.producto }} ({{ o.kilos }} Kg) 
                            <span :class="['badge', 'badge-' + o.estado.toLowerCase()]">{{ o.estado }}</span>
                        </li>
                    </ul>
                </div>
            </div>
            
            <div class="modal-footer">
                <button class="btn-cancelar" @click="$emit('close')">Cerrar</button>
                <button v-if="!yaEstaDeclarado" class="btn-confirmar" @click="declararConsumos" :disabled="procesando">
                    {{ procesando ? '⏳ Procesando...' : '✅ Declarar Consumos de Mezcla' }}
                </button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0,0,0,0.6); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 20px; border-radius: 12px; width: 600px; max-width: 95vw; max-height: 90vh; overflow-y: auto; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #3498db; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #2c3e50; }
.btn-close { background: none; border: none; font-size: 1.5rem; cursor: pointer; color: #7f8c8d; }
.alerta-info { background: #ebf5fb; border-left: 4px solid #3498db; padding: 12px; margin-bottom: 15px; border-radius: 4px; color: #2980b9; font-size: 0.9rem; }
.alerta-ok { background: #eafaf1; border-left: 4px solid #2ecc71; padding: 12px; margin-bottom: 15px; border-radius: 4px; color: #27ae60; font-size: 0.9rem; }
.seccion { margin-bottom: 20px; }
.seccion h4 { color: #34495e; border-bottom: 1px dashed #bdc3c7; padding-bottom: 5px; margin-bottom: 10px; }
.tabla-container { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 10px; }
table { width: 100%; border-collapse: collapse; }
th { text-align: left; padding: 8px; border-bottom: 2px solid #cbd5e1; color: #475569; font-size: 0.85rem; }
td { padding: 8px; border-bottom: 1px solid #e2e8f0; vertical-align: middle; }
.text-center { text-align: center; }
.lista-ordenes { list-style: none; padding: 0; margin: 0; }
.lista-ordenes li { background: #f8fafc; padding: 8px 12px; margin-bottom: 5px; border-radius: 6px; border: 1px solid #e2e8f0; font-size: 0.9rem; display: flex; justify-content: space-between; align-items: center;}
.modal-footer { display: flex; justify-content: flex-end; gap: 10px; border-top: 1px solid #ecf0f1; padding-top: 15px; }
.btn-cancelar { background: #95a5a6; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; }
.btn-confirmar { background: #27ae60; color: white; border: none; padding: 10px 20px; border-radius: 6px; cursor: pointer; font-weight: bold; }
.btn-confirmar:disabled { background: #bdc3c7; cursor: not-allowed; }
.badge { padding: 3px 8px; border-radius: 12px; font-size: 0.75rem; font-weight: bold; }
.badge-pendiente { background: #fff7ed; color: #d97706; border: 1px solid #fcd34d; }
.badge-materialpreparado { background: #eff6ff; color: #3b82f6; border: 1px solid #93c5fd; }
.badge-finalizada { background: #ecfdf5; color: #10b981; border: 1px solid #a7f3d0; }
</style>