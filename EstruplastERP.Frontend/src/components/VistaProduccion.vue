<script setup lang="ts">
import { ref } from 'vue';
import FormularioProduccion from './FormularioProduccion.vue';
import ListaProduccion from './ListaProduccion.vue';

const tablaHistorial = ref();

function recargarTabla() {
    if (tablaHistorial.value) {
        tablaHistorial.value.cargarHistorial();
    }
}
</script>

<template>
  <div class="vista-produccion">
     <FormularioProduccion @guardado="recargarTabla" />
    <hr class="separador">
    <ListaProduccion ref="tablaHistorial" />
  </div>
</template>

<style scoped>
.vista-produccion { display: flex; flex-direction: column; gap: 30px; }
.separador { border: 0; border-top: 2px dashed #ccc; width: 100%; margin: 20px 0; }
</style>