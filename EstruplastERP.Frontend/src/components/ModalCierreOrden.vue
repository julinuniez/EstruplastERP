<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import axios from 'axios'

const props = defineProps<{
  orden: any,
  materiasPrimas: any[],
  visible: boolean
}>()

const emit = defineEmits(['close', 'confirmar'])

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';

const kilosReales = ref<number>(0)
const fechaCierreManual = ref<string>(new Date().toISOString().slice(0, 10))

const consumosBase = ref<{ materiaPrimaId: number, nombre: string, teorico: number, real: number, stockActual: number, clienteId?: number }[]>([])

const formCierre = ref({
  adiciones: [] as { materiaPrimaId: number, nombre: string, cantidad: number, motivo: string, clienteId?: number }[],
  kilosDesperdicio: 0,
  observacionCierre: ''
})

const adicionSeleccionada = ref<number | ''>('')
const cantidadAdicion = ref<number | ''>('')
const motivoAdicion = ref('')

const materialYaDescontado = computed(() => {
  return props.orden?.estado === 'MaterialPreparado';
});

const insumosPermitidos = computed(() => {
  if (!props.materiasPrimas) return [];
  const idClienteOrden = props.orden?.clienteId || 0;
  return props.materiasPrimas.filter(mp => {
    const idDueñoInsumo = mp.clienteId || 0;
    const nombreInsumo = (mp.nombre || '').toUpperCase();
    const skuInsumo = (mp.codigoSku || '').toUpperCase();
    if (nombreInsumo.includes('BASE') || skuInsumo.includes('BASE')) return false;
    return idDueñoInsumo === 0 || idDueñoInsumo === idClienteOrden;
  });
});

watch(() => props.visible, (isOpen) => {
  if (isOpen && props.orden) {
    kilosReales.value = props.orden.kilos || props.orden.kilosEstimados || 0;
    formCierre.value.kilosDesperdicio = props.orden.desperdicio || 0;
    formCierre.value.observacionCierre = '';
    formCierre.value.adiciones = [];
    fechaCierreManual.value = new Date().toISOString().slice(0, 10);

    if (props.orden.consumos) {
      consumosBase.value = props.orden.consumos.map((c: any) => {
        const mp = props.materiasPrimas.find(m => m.id === c.materiaPrimaId);
        return {
          materiaPrimaId: c.materiaPrimaId,
          nombre: c.nombreMateriaPrima,
          teorico: c.cantidadKilos,
          real: c.cantidadKilos,
          stockActual: mp ? mp.stockActual : 0,
          clienteId: mp ? mp.clienteId : 0
        };
      });
    } else {
      consumosBase.value = [];
    }
  }
});

const agregarAdicion = () => {
  if (!adicionSeleccionada.value || !cantidadAdicion.value || Number(cantidadAdicion.value) <= 0) {
    alert("Ingrese datos válidos.");
    return;
  }
  const mp = props.materiasPrimas.find(m => m.id === adicionSeleccionada.value);
  if (mp) {
    if (mp.stockActual < Number(cantidadAdicion.value)) {
      const confirmarNegativo = confirm(`⚠️ CUIDADO: El stock de ${mp.nombre} quedará en negativo.\nStock: ${mp.stockActual} Kg\nAgregas: ${cantidadAdicion.value} Kg\n¿Deseas continuar?`);
      if (!confirmarNegativo) return;
    }
    formCierre.value.adiciones.push({
      materiaPrimaId: mp.id,
      nombre: mp.nombre,
      cantidad: Number(cantidadAdicion.value),
      motivo: motivoAdicion.value || 'Ajuste extra de máquina',
      clienteId: mp.clienteId || 0
    });
    adicionSeleccionada.value = '';
    cantidadAdicion.value = '';
    motivoAdicion.value = '';
  }
}

const quitarAdicion = (index: number) => {
  formCierre.value.adiciones.splice(index, 1);
}

const confirmarCierre = async () => {
  let todosLosConsumos: any[] = [];

  // 🚀 SI LA BASE YA FUE DESCONTADA, enviamos SOLO las adiciones extra
  if (materialYaDescontado.value) {
    todosLosConsumos = formCierre.value.adiciones.map(a => ({ 
      materiaPrimaId: a.materiaPrimaId, 
      cantidadKilosReales: Number(a.cantidad) 
    }));
  } else {
    // Si es una OP normal, mandamos Base + Extras
    todosLosConsumos = [
      ...consumosBase.value.map(c => ({ 
        materiaPrimaId: c.materiaPrimaId, 
        cantidadKilosReales: Number(c.real) 
      })),
      ...formCierre.value.adiciones.map(a => ({ 
        materiaPrimaId: a.materiaPrimaId, 
        cantidadKilosReales: Number(a.cantidad) 
      }))
    ];
  }

  // Validación local del stock sumado
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
    return;
  }

  const msjConfirmacion = materialYaDescontado.value 
    ? `¿Cerrar orden con ${kilosReales.value}kg finales? \n\nMaterial Base: YA DESCONTADO.\nExtras a descontar ahora: ${formCierre.value.adiciones.length > 0 ? formCierre.value.adiciones.length + ' insumo(s)' : 'Ninguno'}.`
    : `¿Estás seguro de cerrar la orden con estos consumos reales?`;

  if (!confirm(msjConfirmacion)) return;

  try {
    const payload = {
      kilosProducidosReales: kilosReales.value,
      desperdicioReal: formCierre.value.kilosDesperdicio,
      observacion: formCierre.value.observacionCierre, 
      consumosReales: todosLosConsumos,
      fechaCierre: fechaCierreManual.value
    };

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
        
        <div v-if="materialYaDescontado" class="alerta-preparado">
          ✅ <strong>La receta base de esta orden ya fue descontada de stock.</strong><br>
          Indique la producción final. Solo utilice la tabla de materiales si agregó material EXTRA directamente en la máquina.
        </div>

        <div class="info-bar">
          <span>📦 <strong>Producto:</strong> {{ orden.nombreProducto || orden.producto }}</span>
          <span>👤 <strong>Cliente:</strong> {{ orden.clienteNombre }}</span>
          <span>📅 <strong>Creada:</strong> {{ orden.fecha ? new Date(orden.fecha).toLocaleDateString() : orden.fechaCreacion }}</span>
        </div>
        
        <div class="seccion box-resultado box-flex-header">
          <div>
            <h4>✅ Producción Final (Bobina)</h4>
            <div class="input-group">
              <label>Total Kilos Fabricados:</label>
              <input type="number" v-model="kilosReales" class="input-success" min="0" step="0.1">
            </div>
          </div>
          <div class="box-fecha-cierre">
            <h4>⏱️ Fecha Real de Producción</h4>
            <div class="input-group">
              <label>Finalizó el:</label>
              <input type="date" v-model="fechaCierreManual" class="input-date">
            </div>
          </div>
        </div>

        <template v-if="!materialYaDescontado">
          <div class="seccion" v-if="consumosBase.length > 0">
            <h4>📋 Materiales de la Receta</h4>
            <div class="tabla-container">
              <table>
                <thead><tr><th>Insumo</th><th class="text-center">Teórico (Kg)</th><th>Consumo Real (Kg)</th><th></th></tr></thead>
                <tbody>
                  <tr v-for="(item, idx) in consumosBase" :key="idx">
                    <td>
                      <span v-if="item.clienteId && item.clienteId > 0" class="badge-cliente">{{ orden.clienteNombre }}</span>
                      {{ item.nombre }}
                    </td>
                    <td class="text-center" style="color: #7f8c8d;">{{ item.teorico.toFixed(2) }}</td>
                    <td><input type="number" v-model="item.real" style="width: 100px; padding: 5px;" step="0.1"></td>
                    <td>
                      <span v-if="item.real > (item.stockActual + item.teorico)" class="badge-alerta" title="Quedará en negativo">⚠️ Stock</span>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </template>
        
        <hr class="divider">
          
        <div class="seccion">
          <h4>➕ Adiciones Extras en Máquina (Opcional)</h4>
          <div class="form-row-add">
            <div class="col-grow">
              <select v-model="adicionSeleccionada">
                <option value="">-- Seleccionar Insumo Extra --</option>
                <option v-for="mp in insumosPermitidos" :key="mp.id" :value="mp.id">
                  {{ (mp.clienteId === 0 || !mp.clienteId) ? '🏢 ESTRUPLAST' : '👤 ' + (orden.clienteNombre || 'CLIENTE') }} - {{ mp.nombre }} (Stock: {{ mp.stockActual }} Kg)
                </option>
              </select>
            </div>
            <div class="col-fixed-80">
              <input type="number" v-model="cantidadAdicion" placeholder="Kg" min="0" step="0.1">
            </div>
            <div class="col-grow">
              <input type="text" v-model="motivoAdicion" placeholder="Motivo (ej: Color extra, purga...)">
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
                  <td>
                    <span v-if="item.clienteId && item.clienteId > 0" class="badge-cliente">{{ orden.clienteNombre }}</span>
                    {{ item.nombre }}
                  </td>
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
.alerta-preparado { background-color: #eff6ff; border-left: 4px solid #3b82f6; padding: 12px; margin-bottom: 15px; border-radius: 4px; color: #1e3a8a; font-size: 0.95rem; }
.modal-overlay { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(0, 0, 0, 0.5); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-content { background: white; padding: 25px; border-radius: 12px; width: 750px; max-width: 95%; max-height: 90vh; overflow-y: auto; box-shadow: 0 10px 25px rgba(0, 0, 0, 0.2); }
.modal-header { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid #3498db; padding-bottom: 15px; margin-bottom: 20px; }
.modal-header h3 { margin: 0; color: #2c3e50; font-size: 1.4rem; }
.btn-close { background: none; border: none; font-size: 1.8rem; cursor: pointer; color: #7f8c8d; line-height: 1; }
.btn-close:hover { color: #e74c3c; }
.info-bar { display: flex; justify-content: space-between; background: #f8fafc; padding: 12px 15px; border-radius: 8px; margin-bottom: 20px; font-size: 0.9rem; color: #334155; border: 1px solid #e2e8f0; }
.seccion { margin-bottom: 20px; }
.seccion h4 { margin-bottom: 12px; color: #34495e; font-size: 1.1rem; display: flex; align-items: center; gap: 8px; }
.tabla-container { border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden; }
table { width: 100%; border-collapse: collapse; font-size: 0.9rem; }
th, td { padding: 10px 12px; text-align: left; border-bottom: 1px solid #e2e8f0; }
th { background-color: #f8fafc; font-weight: 600; color: #475569; }
.text-center { text-align: center; }
.text-right { text-align: right; }
.badge-cliente { background-color: #e0f2fe; color: #0369a1; padding: 2px 6px; border-radius: 4px; font-size: 0.7rem; font-weight: bold; margin-right: 8px; }
.badge-alerta { background-color: #fef2f2; color: #ef4444; padding: 2px 6px; border-radius: 4px; font-size: 0.7rem; font-weight: bold; border: 1px solid #fca5a5; }
.divider { border: 0; height: 1px; background: #e2e8f0; margin: 25px 0; }
.form-row-add { display: flex; gap: 10px; align-items: center; margin-bottom: 15px; background: #f8fafc; padding: 12px; border-radius: 8px; border: 1px dashed #cbd5e1; }
.col-grow { flex: 1; }
.col-fixed-80 { width: 90px; }
.col-btn { width: 40px; }
select, input[type="number"], input[type="text"], input[type="date"], textarea { width: 100%; padding: 8px 12px; border: 1px solid #cbd5e1; border-radius: 6px; font-size: 0.95rem; outline: none; }
select:focus, input:focus, textarea:focus { border-color: #3498db; box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2); }
.btn-plus { background: #3498db; color: white; border: none; width: 36px; height: 36px; border-radius: 6px; font-size: 1.2rem; cursor: pointer; font-weight: bold; display: flex; align-items: center; justify-content: center; }
.btn-plus:hover { background: #2980b9; }
.btn-trash { background: #fee2e2; color: #ef4444; border: 1px solid #fca5a5; padding: 6px 10px; border-radius: 6px; cursor: pointer; transition: 0.2s; }
.btn-trash:hover { background: #fecaca; }
.seccion-flex { display: flex; gap: 20px; }
.box-desperdicio { flex: 1; background: #fffbeb; padding: 15px; border-radius: 8px; border: 1px solid #fde68a; }
.box-obs { flex: 2; }
.input-group { display: flex; flex-direction: column; gap: 8px; }
.input-group label { font-weight: 600; color: #475569; font-size: 0.9rem; }
.input-success { border-color: #2ecc71 !important; background: #f8fff9; font-weight: bold; font-size: 1.1rem !important; color: #27ae60; }
.input-warning { border-color: #f59e0b !important; font-weight: bold; }
.modal-footer { display: flex; justify-content: flex-end; gap: 12px; margin-top: 25px; padding-top: 15px; border-top: 1px solid #e2e8f0; }
.btn-cancelar { background: #f1f5f9; color: #475569; border: 1px solid #cbd5e1; padding: 10px 20px; border-radius: 6px; font-weight: 600; cursor: pointer; font-size: 1rem; }
.btn-cancelar:hover { background: #e2e8f0; }
.btn-confirmar { background: #27ae60; color: white; border: none; padding: 10px 25px; border-radius: 6px; font-weight: bold; cursor: pointer; font-size: 1rem; box-shadow: 0 4px 6px rgba(39, 174, 96, 0.2); }
.btn-confirmar:hover { background: #219653; transform: translateY(-1px); }
.box-resultado { background: #f0fdf4; border: 1px solid #bbf7d0; padding: 15px; border-radius: 8px; margin-bottom: 20px; }
.box-flex-header { display: flex; justify-content: space-between; align-items: flex-end; gap: 20px; }
.box-flex-header > div { flex: 1; }
.input-date { border-color: #94a3b8; background-color: #ffffff; color: #1e293b; font-weight: 600; }
</style>