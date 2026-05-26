<script setup lang="ts">
import { ref, watch } from 'vue'
import api from '@/services/axiosInstance'
import type { ProduccionItem } from './ListaProduccion.vue'

const ID_BRILLO_777 = 1073;
const ID_BRILLO_555 = 2290;

const props = defineProps<{
    visible: boolean;
    ordenEditando: ProduccionItem | null;
}>();

const emit = defineEmits(['close', 'guardado']);

const guardandoEdicion = ref(false);

const recetaBaseOriginal = ref<any[]>([]);

const formEdicion = ref({
    notaPedido: '', numeroPedidoCliente: '',
    largo: 0, ancho: 0, espesor: 0, cantidad: 0, 
    kilosTotales: 0, desperdicio: 8,
    conBrillo: false, tipoBrillo: '777', porcBrillo: 2.00,
    llevaFilm: false, tipoCorona: 'Ninguno', color: '', 
    ignorarStock: true, recetaNueva: [] as any[]
});

watch(() => props.visible, (newVal) => {
    if (newVal && props.ordenEditando) {
        const o = props.ordenEditando;
        
        recetaBaseOriginal.value = (o.consumos || [])
            .filter((c: any) => {
                const id = Number(c.materiaPrimaId ?? c.MateriaPrimaId ?? 0);
                return id !== ID_BRILLO_777 && id !== ID_BRILLO_555;
            })
            .map((c: any) => ({
                materiaPrimaId: Number(c.materiaPrimaId ?? c.MateriaPrimaId ?? 0),
                nombre: c.nombreMateriaPrima || c.NombreMateriaPrima || c.nombre || 'Insumo',
                cantidadKilos: Number(c.cantidadKilos ?? c.CantidadKilos ?? 0)
            }));

        const itemBrillo = (o.consumos || []).find((c: any) => {
            const id = Number(c.materiaPrimaId ?? c.MateriaPrimaId ?? 0);
            return id === ID_BRILLO_777 || id === ID_BRILLO_555;
        });
        
        const tieneBrilloFisico = !!itemBrillo;
        const idBrilloReal = itemBrillo ? Number(itemBrillo.materiaPrimaId ?? itemBrillo.MateriaPrimaId) : ID_BRILLO_777;
        
        let tipoDetectado = idBrilloReal === ID_BRILLO_555 ? '555' : '777';
        let porcDetectado = 2.00;
        
        if (itemBrillo) {
            const kilosBrillo = Number(itemBrillo.cantidadKilos ?? itemBrillo.CantidadKilos ?? 0);
            const kilosOrden = Number(o.kilos ?? 0);
            const desperdicio = Number(o.desperdicio ?? 8);
            const factorMerma = 1 + (desperdicio / 100);
            
            if (kilosOrden > 0 && kilosBrillo > 0) {
                porcDetectado = Number(((kilosBrillo / (kilosOrden * factorMerma)) * 100).toFixed(2));
            }
        }

        formEdicion.value = {
            notaPedido: o.notaPedido || '',
            numeroPedidoCliente: o.numeroPedidoCliente || '',
            largo: o.largo || 0, 
            ancho: o.ancho || 0, 
            espesor: o.espesor || 0,
            cantidad: o.cantidad || 0, 
            kilosTotales: Number(o.kilos ?? 0), 
            desperdicio: Number(o.desperdicio ?? 8),
            conBrillo: o.conBrillo || tieneBrilloFisico, 
            tipoBrillo: tipoDetectado,
            porcBrillo: porcDetectado,
            llevaFilm: o.llevaFilm || false, 
            tipoCorona: o.tipoCorona || 'Ninguno',
            color: o.color || '', 
            ignorarStock: true,
            recetaNueva: [] 
        };
        
        recalcularModal();
    }
});

// 🚀 REGLA DE DEPENDENCIA: Si quita el brillo, se quita el film
watch(() => formEdicion.value.conBrillo, (nuevoValor) => {
    if (!nuevoValor) {
        formEdicion.value.llevaFilm = false;
    }
});

const recalcularModal = () => {
    if (!props.ordenEditando) return;
    const o = props.ordenEditando;
    
    const oldVol = (o.largo || 1) * (o.ancho || 1) * (o.espesor || 1) * (o.cantidad || 1);
    const newVol = (formEdicion.value.largo || 1) * (formEdicion.value.ancho || 1) * (formEdicion.value.espesor || 1) * (formEdicion.value.cantidad || 1);
    
    let ratio = 1;
    if (oldVol > 0) ratio = newVol / oldVol;

    const nuevosKilosTotales = Number((Number(o.kilos ?? 0) * ratio).toFixed(2));
    formEdicion.value.kilosTotales = nuevosKilosTotales;
    
    const factorMerma = 1 + (Number(formEdicion.value.desperdicio ?? 8) / 100);

    const nuevaLista = recetaBaseOriginal.value.map((c: any) => {
        return {
            materiaPrimaId: c.materiaPrimaId,
            nombre: c.nombre,
            cantidadKilos: Number((c.cantidadKilos * ratio).toFixed(2))
        };
    });
    
    if (formEdicion.value.conBrillo && Number(formEdicion.value.porcBrillo) > 0) {
        const idBrilloDestino = formEdicion.value.tipoBrillo === '555' ? ID_BRILLO_555 : ID_BRILLO_777;
        const nombreBrilloDestino = formEdicion.value.tipoBrillo === '555' ? 'CRISTAL 555 (FINO)' : 'CRISTAL 777';
        const kilosCalculadosBrillo = ((nuevosKilosTotales * Number(formEdicion.value.porcBrillo)) / 100) * factorMerma;
        
        nuevaLista.push({
            materiaPrimaId: idBrilloDestino,
            nombre: nombreBrilloDestino,
            cantidadKilos: Number(kilosCalculadosBrillo.toFixed(2))
        });
    }
    
    formEdicion.value.recetaNueva = nuevaLista;
};

async function guardarEdicionRapida() {
    if (!props.ordenEditando) return;
    guardandoEdicion.value = true;
    try {
        await api.put(`/Ordenes/modificar/${props.ordenEditando.id}`, formEdicion.value);
        emit('guardado');
        emit('close');
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
                <h3>✏️ Editar Orden #{{ ordenEditando?.id }}</h3>
                <button class="btn-close" @click="emit('close')">✕</button>
            </div>

            <div class="modal-split-body">
                
                <div class="columna-formulario">
                    
                    <div class="grid-inputs-doble">
                        <div class="campo-compacto">
                            <label style="color:#1abc9c">Nota Pedido</label>
                            <input type="text" v-model="formEdicion.notaPedido">
                        </div>
                        <div class="campo-compacto">
                            <label style="color:#f39c12">OC Cliente</label>
                            <input type="text" v-model="formEdicion.numeroPedidoCliente">
                        </div>
                    </div>

                    <div class="grid-inputs-doble" style="margin-top: 8px;">
                        <div class="campo-compacto">
                            <label>Largo (mm)</label>
                            <input type="number" v-model="formEdicion.largo" @input="recalcularModal">
                        </div>
                        <div class="campo-compacto">
                            <label>Ancho (mm)</label>
                            <input type="number" v-model="formEdicion.ancho" @input="recalcularModal">
                        </div>
                    </div>

                    <div class="grid-inputs-doble" style="margin-top: 8px;">
                        <div class="campo-compacto">
                            <label>Espesor</label>
                            <input type="number" v-model="formEdicion.espesor" step="0.01" @input="recalcularModal">
                        </div>
                        <div class="campo-compacto">
                            <label>Cantidad</label>
                            <input type="number" v-model="formEdicion.cantidad" min="1" @input="recalcularModal">
                        </div>
                    </div>

                    <div class="bloque-brillo-complejo" :class="{ 'brillo-activo': formEdicion.conBrillo }">
                        <div class="fila-brillo-check">
                            <label class="check-fast">
                                <input type="checkbox" v-model="formEdicion.conBrillo" @change="recalcularModal"> ✨ Lleva Aditivo de Brillo
                            </label>
                            <label class="check-fast" :class="{ 'disabled-check': !formEdicion.conBrillo }">
                                <input type="checkbox" v-model="formEdicion.llevaFilm" @change="recalcularModal" :disabled="!formEdicion.conBrillo"> 🛡️ Film
                            </label>
                        </div>
                        
                        <div v-if="formEdicion.conBrillo" class="desplegable-opciones-brillo">
                            <div class="campo-compacto" style="flex: 1.2;">
                                <label>Tipo de Brillo</label>
                                <select v-model="formEdicion.tipoBrillo" @change="recalcularModal">
                                    <option value="777">Cristal 777</option>
                                    <option value="555">Cristal 555 (Fino)</option>
                                </select>
                            </div>
                            <div class="campo-compacto" style="flex: 0.8;">
                                <label>Porcentaje (%)</label>
                                <input type="number" v-model="formEdicion.porcBrillo" step="0.1" min="0" @input="recalcularModal" style="font-weight: bold; color: #2563eb;">
                            </div>
                        </div>
                    </div>

                    <div class="grid-inputs-doble" style="margin-top: 8px;">
                        <div class="campo-compacto">
                            <label>Tratamiento Corona</label>
                            <select v-model="formEdicion.tipoCorona" @change="recalcularModal">
                                <option value="Ninguno">Ninguno</option>
                                <option value="Simple">Simple</option>
                                <option value="Doble">Doble</option>
                            </select>
                        </div>
                        <div class="campo-compacto">
                            <label>Color (Texto)</label>
                            <input type="text" v-model="formEdicion.color" placeholder="Ej: AZUL" @input="recalcularModal">
                        </div>
                    </div>
                </div>

                <div class="columna-hoja-carga">
                    <div class="header-hoja">
                        <span>📋 Hoja de Carga Estimada</span>
                        <strong>Total: {{ formEdicion.kilosTotales }} kg</strong>
                    </div>
                    
                    <div class="lista-consumos-mini">
                        <div v-for="(item, idx) in formEdicion.recetaNueva" :key="idx" class="item-consumo-mini">
                            <span class="nombre-mp">{{ item.nombre }}</span>
                            <span class="kilos-mp">{{ item.cantidadKilos }} kg</span>
                        </div>
                        <div v-if="formEdicion.recetaNueva.length === 0" class="hoja-vacia">
                            No hay consumos asociados a esta orden.
                        </div>
                    </div>
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
.modal-overlay { position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(0,0,0,0.4); display: flex; align-items: center; justify-content: center; z-index: 9999; backdrop-filter: blur(1px); }
.modal-content { background: white; padding: 12px 20px; border-radius: 8px; width: 800px; max-width: 95vw; box-shadow: 0 8px 20px rgba(0,0,0,0.2); display: flex; flex-direction: column; }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 1px solid #e2e8f0; padding-bottom: 6px; margin-bottom: 8px; }
.modal-header h3 { margin: 0; color: #1e293b; font-size: 1rem; font-weight: bold; }
.btn-close { background: none; border: none; font-size: 1.1rem; cursor: pointer; color: #94a3b8; }

.modal-split-body { display: flex; gap: 15px; align-items: stretch; }
.columna-formulario { flex: 1.1; display: flex; flex-direction: column; gap: 4px; }
.columna-hoja-carga { flex: 0.9; background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 6px; padding: 8px; display: flex; flex-direction: column; min-height: 230px; max-height: 250px; }

.grid-inputs-doble { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; }
.campo-compacto label { display: block; font-size: 0.75rem; font-weight: bold; color: #475569; margin-bottom: 1px; }
.campo-compacto input, .campo-compacto select { width: 100%; padding: 4px 8px; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box; font-size: 0.82rem; height: 28px; }
.campo-compacto input:focus, .campo-compacto select:focus { outline: none; border-color: #3b82f6; }

.bloque-brillo-complejo { background: #f1f5f9; padding: 6px; border-radius: 4px; margin-top: 2px; border: 1px solid transparent; }
.bloque-brillo-complejo.brillo-activo { background: #e0f2fe; border-color: #bae6fd; }
.fila-brillo-check { display: flex; justify-content: space-between; padding: 0 4px; }
.desplegable-opciones-brillo { display: flex; gap: 8px; margin-top: 6px; border-top: 1px dashed #cbd5e1; padding-top: 6px; }

.check-fast { display: flex; align-items: center; gap: 5px; font-size: 0.8rem; color: #334155; cursor: pointer; font-weight: bold; transition: opacity 0.2s; }
.disabled-check { opacity: 0.5; cursor: not-allowed; } /* 🚀 Estilo para el film deshabilitado */

.header-hoja { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #cbd5e1; padding-bottom: 4px; margin-bottom: 6px; font-size: 0.8rem; color: #475569; }
.header-hoja strong { color: #1d4ed8; font-size: 0.85rem; }
.lista-consumos-mini { flex: 1; overflow-y: auto; display: flex; flex-direction: column; gap: 3px; }
.item-consumo-mini { display: flex; justify-content: space-between; background: white; padding: 4px 8px; border-radius: 4px; border: 1px solid #e2e8f0; font-size: 0.78rem; font-weight: bold; }
.nombre-mp { color: #334155; }
.kilos-mp { color: #2563eb; }
.hoja-vacia { text-align: center; color: #94a3b8; font-style: italic; font-size: 0.78rem; margin-top: 15px; }

input[type=number]::-webkit-inner-spin-button, input[type=number]::-webkit-outer-spin-button { -webkit-appearance: none; margin: 0; }
input[type=number] { -moz-appearance: textfield; appearance: textfield; }

.modal-footer { display: flex; justify-content: flex-end; gap: 6px; margin-top: 8px; border-top: 1px solid #f1f5f9; padding-top: 6px; }
.btn-cancelar { background: white; border: 1px solid #cbd5e1; color: #475569; padding: 4px 10px; border-radius: 4px; cursor: pointer; font-size: 0.82rem; font-weight: 600; }
.btn-guardar { background: #3b82f6; color: white; border: none; padding: 4px 10px; border-radius: 4px; cursor: pointer; font-size: 0.82rem; font-weight: 600; }
.btn-guardar:hover:not(:disabled) { background: #2563eb; }
.btn-guardar:disabled { background: #94a3b8; cursor: not-allowed; }
</style>