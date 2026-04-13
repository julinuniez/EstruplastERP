<script setup lang="ts">
import type { ProduccionItem } from './ListaProduccion.vue'

defineProps<{
    visible: boolean;
    codigo: string;
    ordenes: ProduccionItem[];
}>();

const emit = defineEmits(['close']);
</script>

<template>
    <div v-if="visible" class="modal-overlay" @click.self="emit('close')">
        <div class="modal-content modal-lg">
            <div class="modal-header">
                <h3>📦 Detalle de Hoja de Carga: {{ codigo }}</h3>
                <button class="btn-close" @click="emit('close')">✕</button>
            </div>
            
            <p style="color: #64748b; font-size: 0.9rem; margin-top:-10px;">Estas son las órdenes que fueron agrupadas en esta hoja de mezcla.</p>
            
            <div class="tabla-scroll" style="max-height: 400px; margin-bottom: 0;">
                <table class="tabla-custom">
                    <thead>
                        <tr>
                            <th style="width: 80px;">OP #</th>
                            <th style="width: 100px;">Nota</th>
                            <th>Cliente</th>
                            <th>Producto / Color</th>
                            <th style="text-align: right; width: 80px;">Kilos</th>
                            <th style="text-align: center; width: 100px;">Estado</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="o in ordenes" :key="o.id">
                            <td style="font-weight: bold; color: #3498db;">{{ o.id }}</td>
                            <td>{{ o.notaPedido || '-' }}</td>
                            <td><span class="badge-cliente">{{ o.clienteNombre || 'Stock' }}</span></td>
                            <td>
                                <span class="prod-nombre">{{ o.producto }}</span>
                                <span v-if="o.color" class="tag-color" style="margin-top:2px; display:inline-block;">🎨 {{ o.color.toUpperCase() }}</span>
                            </td>
                            <td style="text-align: right; font-weight: bold;">{{ Math.round(o.kilos) }}</td>
                            <td style="text-align: center;">
                                <span :class="{'badge-pend': o.estado !== 'Finalizada' && o.estado !== 'Cancelada', 'badge-ok': o.estado === 'Finalizada', 'badge-cancel': o.estado === 'Cancelada'}">
                                    {{ o.estado === 'Cancelada' ? 'CANCELADA' : (o.estado === 'Finalizada' ? 'FINALIZADA' : 'MÁQUINA') }}
                                </span>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>

            <div class="modal-footer">
                <button class="btn-cancelar" @click="emit('close')">Cerrar Detalle</button>
            </div>
        </div>
    </div>
</template>

<style scoped>
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.6); display: flex; align-items: center; justify-content: center; z-index: 9999; backdrop-filter: blur(2px); }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 450px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
.modal-lg { width: 700px; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #f1f5f9; padding-bottom: 10px; margin-bottom: 15px; }
.modal-header h3 { margin: 0; color: #1e293b; font-size: 1.2rem; }
.btn-close { background: none; border: none; font-size: 1.2rem; cursor: pointer; color: #94a3b8; }
.btn-close:hover { color: #ef4444; }

.tabla-scroll { overflow-y: auto; border-radius: 6px; border: 1px solid #e2e8f0; }
.tabla-custom { width: 100%; border-collapse: separate; border-spacing: 0; font-size: 0.85rem; }
.tabla-custom th { background: #f8fafc; color: #475569; padding: 12px 10px; text-align: left; position: sticky; top: 0; font-weight: 700; border-bottom: 2px solid #e2e8f0; text-transform: uppercase; font-size: 0.75rem; }
.tabla-custom td { padding: 10px; border-bottom: 1px solid #f1f5f9; color: #334155; vertical-align: middle; }
.prod-nombre { display: block; font-weight: bold; }
.tag-color { background: #f1f5f9; border: 1px solid #cbd5e1; color: #334155; font-size: 0.6rem; padding: 1px 4px; border-radius: 3px; font-weight: 800; }
.badge-cliente { background-color: #e0f2fe; color: #0369a1; padding: 4px 8px; border-radius: 4px; font-weight: 600; font-size: 0.75rem; }
.badge-pend { background: #fff7ed; color: #d97706; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fcd34d; }
.badge-ok { background: #ecfdf5; color: #10b981; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #a7f3d0; }
.badge-cancel { background: #fef2f2; color: #ef4444; padding: 4px 10px; border-radius: 12px; font-size: 0.7rem; font-weight: 700; border: 1px solid #fecaca; }

.modal-footer { display: flex; justify-content: flex-end; gap: 10px; margin-top: 25px; border-top: 1px solid #f1f5f9; padding-top: 15px; }
.btn-cancelar { background: white; border: 1px solid #cbd5e1; color: #475569; padding: 8px 15px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-cancelar:hover { background: #f8fafc; }
</style>