<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'

// --- INTERFAZ ---
interface Entidad {
    id: number;
    nombreCompleto?: string;
    dni?: string;
    puesto?: string;
    razonSocial?: string;
    cuit?: string;
    activo: boolean;
}

// --- ESTADO ---
const pestana = ref<'empleados' | 'clientes'>('empleados')
const lista = ref<Entidad[]>([]) 
const cargando = ref(false)

const listaPuestos = [
    'Operario General', 'Logística', 'Extrusor', 'Impresor', 
    'Supervisor', 'Mantenimiento', 'Administración', 'Gerencia', 'Chofer'
]

// Tipamos el formulario explícitamente para evitar conflictos
const itemForm = ref<{
    id: number;
    nombre: string;
    dni: string;
    cuit: string;
    puesto: string;
    activo: boolean;
}>({ 
    id: 0, 
    nombre: '', 
    dni: '', 
    cuit: '', 
    puesto: 'Operario General', 
    activo: true 
})

const modoEdicion = ref(false)
const apiUrl = 'https://localhost:7244/api' 

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- CRUD ---

async function cargarDatos() {
  cargando.value = true
  const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
  try {
    const res = await axios.get(`${apiUrl}/${endpoint}`, getAuthConfig())
    lista.value = res.data
  } catch (e: any) { 
    console.error(e)
    if (e.response?.status === 401) alert("Sesión expirada.")
  } finally { 
    cargando.value = false 
  }
}

async function guardar() {
  const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
  
  const payload = pestana.value === 'empleados' 
    ? { 
        id: itemForm.value.id,
        nombreCompleto: itemForm.value.nombre, 
        dni: itemForm.value.dni, 
        activo: itemForm.value.activo,
        puesto: itemForm.value.puesto 
      }
    : { 
        id: itemForm.value.id,
        razonSocial: itemForm.value.nombre, 
        cuit: itemForm.value.cuit, 
        activo: itemForm.value.activo 
      }

  const url = modoEdicion.value ? `${apiUrl}/${endpoint}/${itemForm.value.id}` : `${apiUrl}/${endpoint}`

  try {
      if (modoEdicion.value) {
          await axios.put(url, payload, getAuthConfig())
      } else {
          await axios.post(url, payload, getAuthConfig())
      }
      alert("✅ Guardado correctamente")
      limpiarForm()
      cargarDatos()
  } catch(e: any) { 
      const msg = e.response?.data?.mensaje || e.message
      alert("❌ Error al guardar: " + msg) 
  }
}

function editar(item: Entidad) {
    modoEdicion.value = true
    
    // CORRECCIÓN AQUÍ 👇
    // Agregamos " || '' " al final para asegurar que NUNCA sea undefined
    itemForm.value = {
        id: item.id,
        nombre: item.nombreCompleto || item.razonSocial || '',
        dni: item.dni || '',
        cuit: item.cuit || '',
        // Si item.puesto es null Y listaPuestos[0] falla, usamos un string vacío
        puesto: item.puesto || listaPuestos[0] || '', 
        activo: item.activo
    }
}

async function eliminar(id: number) {
    if(!confirm("¿Desea eliminar/desactivar este registro?")) return
    const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
    try {
        await axios.delete(`${apiUrl}/${endpoint}/${id}`, getAuthConfig())
        cargarDatos()
    } catch (e: any) {
        alert("Error al eliminar: " + e.message)
    }
}

function limpiarForm() {
    modoEdicion.value = false
    itemForm.value = { 
        id: 0, 
        nombre: '', 
        dni: '', 
        cuit: '', 
        // CORRECCIÓN AQUÍ TAMBIÉN 👇
        puesto: listaPuestos[0] || '', 
        activo: true 
    }
}

onMounted(() => cargarDatos())
</script>

<template>
  <div class="panel-admin">
    <h2>⚙️ Configuración General</h2>
    
    <div class="tabs">
        <button :class="{ active: pestana==='empleados' }" @click="pestana='empleados'; cargarDatos()">👷 Empleados</button>
        <button :class="{ active: pestana==='clientes' }" @click="pestana='clientes'; cargarDatos()">🏢 Clientes</button>
    </div>

    <div class="contenido-abm">
        <div class="card-form">
            <h3>{{ modoEdicion ? 'Editar' : 'Nuevo' }} {{ pestana === 'empleados' ? 'Empleado' : 'Cliente' }}</h3>
            
            <label>Nombre / Razón Social:</label>
            <input type="text" v-model="itemForm.nombre" placeholder="Nombre completo">

            <div v-if="pestana==='empleados'">
                <label>DNI / Legajo:</label>
                <input type="text" v-model="itemForm.dni">

                <label>Puesto:</label>
                <select v-model="itemForm.puesto">
                    <option v-for="p in listaPuestos" :key="p" :value="p">{{ p }}</option>
                </select>
            </div>
            
            <div v-if="pestana==='clientes'">
                <label>CUIT:</label>
                <input type="text" v-model="itemForm.cuit">
            </div>

            <div class="check-box">
                <input type="checkbox" v-model="itemForm.activo" id="act">
                <label for="act">Activo</label>
            </div>

            <div class="btn-group">
                <button v-if="modoEdicion" @click="limpiarForm" class="btn-cancel">Cancelar</button>
                <button @click="guardar" class="btn-save">💾 Guardar</button>
            </div>
        </div>

        <div class="tabla-container">
            <table>
                <thead>
                    <tr>
                        <th>Nombre</th>
                        <th v-if="pestana==='empleados'">Puesto</th> 
                        <th>Identificación</th>
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in lista" :key="item.id">
                        <td>{{ item.nombreCompleto || item.razonSocial }}</td>
                        
                        <td v-if="pestana==='empleados'">
                            <span class="badge-puesto">{{ item.puesto || '-' }}</span>
                        </td>
                        
                        <td>{{ item.dni || item.cuit }}</td>
                        <td>
                            <span :class="item.activo ? 'badge-ok' : 'badge-no'">
                                {{ item.activo ? 'Activo' : 'Inactivo' }}
                            </span>
                        </td>
                        <td>
                            <button @click="editar(item)" class="btn-small">✏️</button>
                            <button @click="eliminar(item.id)" class="btn-small btn-del">🗑️</button>
                        </td>
                    </tr>
                    <tr v-if="lista.length === 0">
                        <td colspan="5" style="text-align: center; color: #888;">No hay datos cargados.</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
  </div>
</template>

<style scoped>
/* (ESTILOS ORIGINALES CON PEQUEÑAS MEJORAS) */
.panel-admin { max-width: 1000px; margin: 0 auto; }
.tabs { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #ddd; padding-bottom: 10px; }
.tabs button { background: none; border: none; font-size: 1.1em; cursor: pointer; padding: 10px; color: #777; }
.tabs button.active { color: #2c3e50; font-weight: bold; border-bottom: 3px solid #3498db; }

.contenido-abm { display: grid; grid-template-columns: 1fr 2fr; gap: 20px; }
.card-form { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); height: fit-content; }

.card-form input[type="text"], 
.card-form select { width: 100%; padding: 8px; margin-bottom: 10px; border: 1px solid #ccc; border-radius: 4px; }

.check-box { margin: 10px 0; display: flex; gap: 5px; align-items: center; cursor: pointer; }

.btn-group { display: flex; gap: 5px; margin-top: 10px; }
.btn-save { flex: 1; background: #27ae60; color: white; border: none; padding: 10px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-save:hover { background: #219150; }
.btn-cancel { background: #95a5a6; color: white; border: none; padding: 10px; border-radius: 4px; cursor: pointer; }

.tabla-container { background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
table { width: 100%; border-collapse: collapse; }
th, td { padding: 10px; text-align: left; border-bottom: 1px solid #eee; }

.badge-ok { background: #d4edda; color: #155724; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; }
.badge-no { background: #f8d7da; color: #721c24; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; }
.badge-puesto { background: #e3f2fd; color: #0d47a1; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; }

.btn-small { border: none; background: #eee; padding: 5px 8px; border-radius: 4px; cursor: pointer; margin-right: 5px; }
.btn-del { background: #ffcccc; color: red; }
</style>