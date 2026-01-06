<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router' // Importamos herramientas de ruta
import axios from 'axios'

// Recibimos el ID como prop (gracias al props: true del router)
const props = defineProps<{
  id?: string | number // Es opcional, porque si creamos uno nuevo no viene
}>()

const route = useRoute()
const router = useRouter()
const esModoEdicion = ref(false)

// Tu objeto form (asegúrate que coincida con tu backend)
const form = ref({
  nombre: '',
  codigoSku: '',
  stock: 0,
  // ... resto de campos
})

onMounted(async () => {
  // 1. Obtenemos el valor crudo de las props o la ruta
  const rawId = props.id || route.params.id;

  const idValue = Array.isArray(rawId) ? rawId[0] : rawId;
  const idProducto = Number(idValue);
  if (idProducto && !isNaN(idProducto) && idProducto > 0) {
    esModoEdicion.value = true;
    await cargarDatosProducto(idProducto); 
  }
})

async function cargarDatosProducto(id: string | number) {
  try {
    // Ajusta la URL a tu API real
    const { data } = await axios.get(`https://localhost:7244/api/Productos/${id}`)
    
    // Rellenamos el formulario con los datos que vinieron
    form.value = { ...data } 
    
  } catch (error) {
    console.error("Error al cargar producto:", error)   
    alert("No se pudo cargar el producto.")
  }
}

// Función para guardar (Decide si es POST o PUT)
async function guardar() {
  try {
    if (esModoEdicion.value) {
        // EDITAR (PUT)
        await axios.put(`https://localhost:7244/api/Productos/${props.id}`, form.value)
    } else {
        // CREAR (POST)
        await axios.post(`https://localhost:7244/api/Productos`, form.value)
    }
    router.push('/productos') // Volver a la lista
  } catch (e) {
    console.error(e)
  }
}
</script>