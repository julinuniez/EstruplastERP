<script setup lang="ts">
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale
} from 'chart.js'
import { Bar } from 'vue-chartjs'
import { computed } from 'vue'

// Registramos los módulos necesarios de Chart.js para que funcionen las barras
ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend)

const props = defineProps<{
  datos: number[],       // Ej: [100, 250, 50]
  etiquetas: string[],   // Ej: ['Enero', 'Febrero', 'Marzo']
  titulo: string,        // Ej: 'Producción Mensual'
  color?: string         // Opcional: Color de las barras
}>()

// Configuración reactiva de los datos
const chartData = computed(() => ({
  labels: props.etiquetas,
  datasets: [
    {
      label: props.titulo,
      backgroundColor: props.color || '#3498db', // Azul por defecto
      data: props.datos,
      borderRadius: 4 // Bordes redondeados modernos
    }
  ]
}))

// Opciones visuales (Responsive, sin grid feo, etc.)
const chartOptions = {
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: false }, // Ocultamos leyenda si es un solo dato
    title: {
      display: true,
      text: props.titulo,
      font: { size: 16 }
    }
  },
  scales: {
    y: {
      beginAtZero: true,
      grid: { color: '#f3f3f3' }
    },
    x: {
      grid: { display: false }
    }
  }
}
</script>

<template>
  <div class="contenedor-grafico">
    <Bar :data="chartData" :options="chartOptions" />
  </div>
</template>

<style scoped>
.contenedor-grafico {
  position: relative;
  height: 300px; /* Altura fija para que no se deforme */
  width: 100%;
}
</style>