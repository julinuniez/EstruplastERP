<script setup lang="ts">
import { ref, computed, watch } from 'vue';

const props = defineProps<{
    visible: boolean;
    orden: any;
}>();

const emit = defineEmits(['close', 'guardar']);

const kilosObjetivoPallet = ref(1100);
const guardando = ref(false); // Bloqueo anti-doble clic

interface Pallet {
    id: number;
    numero: number;
    kilos: number;
    estado: string;
}

const pallets = ref<Pallet[]>([]);

const kilosTotalesOP = computed(() => {
    return props.orden ? Math.round(Number(props.orden.kilos)) : 0;
});

// 🚀 LÓGICA DE DIVISIÓN EQUITATIVA CON NÚMEROS REDONDOS
const generarDesgloseTeorico = () => {
    if (kilosTotalesOP.value <= 0 || kilosObjetivoPallet.value <= 0) return;
    
    // Calculamos cuántos pallets se necesitan (siempre redondeando hacia arriba)
    const cantidadPallets = Math.ceil(kilosTotalesOP.value / kilosObjetivoPallet.value);
    
    // Dividimos y redondeamos al entero más cercano para evitar decimales
    const kilosEquitativos = Math.round(kilosTotalesOP.value / cantidadPallets);
    
    const nuevaLista: Pallet[] = [];
    let acumulado = 0;

    for (let i = 1; i <= cantidadPallets; i++) {
        // Al último pallet le tiramos la diferencia exacta para que la suma dé perfecta
        let pesoFinal = kilosEquitativos;
        if (i === cantidadPallets) {
            pesoFinal = kilosTotalesOP.value - acumulado;
        }

        nuevaLista.push({ 
            id: Date.now() + i, 
            numero: i, 
            kilos: pesoFinal, 
            estado: 'Pendiente' 
        });
        acumulado += pesoFinal;
    }
    
    pallets.value = nuevaLista;
};

watch(() => props.visible, (esVisible) => {
    if (esVisible && props.orden) {
        generarDesgloseTeorico();
        guardando.value = false; // Resetear al abrir el modal
    } else {
        pallets.value = [];
    }
});

const totalAsignado = computed(() => {
    return pallets.value.reduce((acc, p) => acc + (Number(p.kilos) || 0), 0);
});

const diferencia = computed(() => {
    return kilosTotalesOP.value - totalAsignado.value;
});

const agregarPalletExtra = () => {
    pallets.value.push({
        id: Date.now(),
        numero: pallets.value.length + 1,
        kilos: diferencia.value > 0 ? diferencia.value : 0,
        estado: 'Pendiente'
    });
};

const quitarPallet = (index: number) => {
    pallets.value.splice(index, 1);
    pallets.value.forEach((p, i) => p.numero = i + 1);
};

const cerrar = () => {
    if (guardando.value) return;
    emit('close');
};

const confirmar = async () => {
    if (guardando.value) return; 
    guardando.value = true;
    emit('guardar', pallets.value);
};
</script>

<template>
  <div v-if="visible" class="modal-fondo">
    <div class="modal-caja">
      <h3>📦 Desglose Equitativo de Pallets</h3>
      <p style="margin-bottom: 5px; color: #64748b;">
        OP #{{ orden?.id }} - <strong>{{ orden?.clienteNombre || 'INTERNO' }}</strong>
      </p>
      
      <div class="controles-superiores">
        <div class="grupo-control">
          <label>Total OP (Kg):</label>
          <input type="number" :value="kilosTotalesOP" class="input-cabecera" disabled />
        </div>
        <div class="grupo-control">
          <label>Peso promedio pallet:</label>
          <div style="display:flex; gap: 5px;">
            <input type="number" v-model="kilosObjetivoPallet" class="input-cabecera" />
            <button @click="generarDesgloseTeorico" class="btn-secundario">🔄</button>
          </div>
        </div>
      </div>

      <div class="tabla-contenedor">
        <table class="tabla-subordenes">
          <thead>
            <tr>
              <th style="width: 80px;">N°</th>
              <th>Kilos Reales</th>
              <th style="width: 50px;"></th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(pallet, index) in pallets" :key="pallet.id">
              <td><strong>#{{ pallet.numero }}</strong></td>
              <td>
                <div class="input-kg-wrapper">
                    <input type="number" v-model="pallet.kilos" class="input-editable-kg" />
                    <span class="unidad">kg</span>
                </div>
              </td>
              <td>
                <button @click="quitarPallet(index)" class="btn-borrar">❌</button>
              </td>
            </tr>
          </tbody>
        </table>
        <button @click="agregarPalletExtra" class="btn-agregar-fila">➕ Agregar pallet manual</button>
      </div>

      <div class="resumen-footer">
        <div class="totales">
            <div :class="{'dif-ok': diferencia === 0, 'dif-error': diferencia !== 0}">
                {{ diferencia === 0 ? '✅ Total exacto' : '⚠️ Dif: ' + diferencia + ' kg' }}
            </div>
        </div>
        
        <div style="display: flex; gap: 10px;">
            <button @click="cerrar" class="btn-cancelar" :disabled="guardando">Cancelar</button>
            <button @click="confirmar" class="btn-guardar" :disabled="guardando || diferencia !== 0">
                {{ guardando ? '⏳ Guardando...' : '💾 Confirmar' }}
            </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Los mismos estilos que ya tenías, solo agregué .dif-error */
.modal-fondo { position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(15, 23, 42, 0.7); display: flex; justify-content: center; align-items: center; z-index: 1000; }
.modal-caja { background: white; padding: 15px 20px; border-radius: 12px; width: 450px; max-height: 90vh; overflow-y: auto; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1); font-family: 'Segoe UI', Arial, sans-serif; }
h3 { margin-top: 0; color: #1e293b; border-bottom: 2px solid #3b82f6; padding-bottom: 8px; font-size: 1.1rem; }
.controles-superiores { display: flex; gap: 15px; background: #f8fafc; padding: 10px; border-radius: 8px; margin-bottom: 10px; border: 1px solid #e2e8f0; }
.grupo-control { display: flex; flex-direction: column; gap: 4px; flex: 1; }
.grupo-control label { font-size: 0.75rem; font-weight: bold; color: #475569; }
.input-cabecera { padding: 6px; border: 1px solid #cbd5e1; border-radius: 6px; font-weight: bold; font-size: 0.9rem; width: 100%; }
.tabla-contenedor { border: 1px solid #e2e8f0; border-radius: 8px; overflow: hidden; max-height: 250px; overflow-y: auto; margin-bottom: 10px;}
.tabla-subordenes { width: 100%; border-collapse: collapse; font-size: 13px; }
.tabla-subordenes th { background: #f1f5f9; padding: 8px; text-align: left; }
.tabla-subordenes td { padding: 6px 10px; border-bottom: 1px solid #f1f5f9; }
.input-editable-kg { width: 80px; padding: 4px; border: 2px solid #cbd5e1; border-radius: 4px; font-weight: 800; text-align: right; }
.btn-guardar { background: #10b981; color: white; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; font-weight: bold; }
.btn-guardar:disabled { background: #94a3b8; cursor: not-allowed; }
.dif-ok { color: #10b981; font-weight: bold; }
.dif-error { color: #ef4444; font-weight: bold; }
.btn-cancelar { background: #e2e8f0; color: #475569; border: none; padding: 10px 20px; border-radius: 8px; cursor: pointer; }
</style>