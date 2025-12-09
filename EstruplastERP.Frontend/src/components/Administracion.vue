<script setup lang="ts">
import { ref, onMounted } from 'vue'

const pestana = ref('empleados')
const lista = ref([])
const cargando = ref(false)

// 👇 1. TU LISTA HARDCODEADA DE PUESTOS (Edítala aquí si necesitas)
const listaPuestos = [
    'Operario General',
    'Logística',
    'Extrusor',
    'Impresor',
    'Supervisor',
    'Mantenimiento',
    'Administración',
    'Gerencia',
    'Chofer'
]

const itemForm = ref({ 
    id: 0, 
    nombre: '', 
    dni: '', 
    cuit: '', 
    puesto: 'Operario General', // Valor por defecto
    activo: true 
})

const modoEdicion = ref(false)
const apiUrl = 'https://localhost:7244/api' // Ajusta puerto si hace falta

async function cargarDatos() {
  cargando.value = true
  const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
  try {
    const res = await fetch(`${apiUrl}/${endpoint}`)
    if (res.ok) lista.value = await res.json()
  } catch (e) { console.error(e) }
  finally { cargando.value = false }
}

async function guardar() {
  const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
  
  // Armamos el objeto según sea Empleado o Cliente
  const payload = pestana.value === 'empleados' 
    ? { 
        id: itemForm.value.id,
        nombreCompleto: itemForm.value.nombre, 
        dni: itemForm.value.dni, 
        activo: itemForm.value.activo,
        puesto: itemForm.value.puesto // 👇 Enviamos el puesto seleccionado
      }
    : { 
        id: itemForm.value.id,
        razonSocial: itemForm.value.nombre, 
        cuit: itemForm.value.cuit, 
        activo: itemForm.value.activo 
      }

  const metodo = modoEdicion.value ? 'PUT' : 'POST'
  const url = modoEdicion.value ? `${apiUrl}/${endpoint}/${itemForm.value.id}` : `${apiUrl}/${endpoint}`

  try {
      const res = await fetch(url, {
          method: metodo,
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(payload)
      })
      
      if(!res.ok) throw new Error("Error en la petición")

      alert("✅ Guardado correctamente")
      limpiarForm()
      cargarDatos()
  } catch(e) { alert("Error al guardar: Verifique los datos") }
}

function editar(item) {
    modoEdicion.value = true
    itemForm.value = {
        id: item.id,
        nombre: item.nombreCompleto || item.razonSocial,
        dni: item.dni || '',
        cuit: item.cuit || '',
        // Si el empleado ya tiene puesto, lo ponemos. Si no, el primero de la lista.
        puesto: item.puesto || listaPuestos[0], 
        activo: item.activo
    }
}

function eliminar(id) {
    if(!confirm("¿Desea eliminar/desactivar este registro?")) return
    const endpoint = pestana.value === 'empleados' ? 'Empleados' : 'Clientes'
    fetch(`${apiUrl}/${endpoint}/${id}`, { method: 'DELETE' }).then(() => cargarDatos())
}

function limpiarForm() {
    modoEdicion.value = false
    itemForm.value = { 
        id: 0, 
        nombre: '', 
        dni: '', 
        cuit: '', 
        puesto: listaPuestos[0], 
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
                        <th v-if="pestana==='empleados'">Puesto</th> <th>Identificación</th>
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
                </tbody>
            </table>
        </div>
    </div>
  </div>
</template>

<style scoped>
/* (Mismos estilos de antes + estilo nuevo para el select) */
.panel-admin { max-width: 1000px; margin: 0 auto; }
.tabs { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #ddd; padding-bottom: 10px; }
.tabs button { background: none; border: none; font-size: 1.1em; cursor: pointer; padding: 10px; color: #777; }
.tabs button.active { color: #2c3e50; font-weight: bold; border-bottom: 3px solid #3498db; }

.contenido-abm { display: grid; grid-template-columns: 1fr 2fr; gap: 20px; }
.card-form { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 2px 5px rgba(0,0,0,0.1); height: fit-content; }

/* Estilos Inputs y Selects */
.card-form input[type="text"], 
.card-form select { 
    width: 100%; padding: 8px; margin-bottom: 10px; border: 1px solid #ccc; border-radius: 4px; 
}

.check-box { margin: 10px 0; display: flex; gap: 5px; align-items: center; }

.btn-group { display: flex; gap: 5px; margin-top: 10px; }
.btn-save { flex: 1; background: #27ae60; color: white; border: none; padding: 10px; border-radius: 4px; cursor: pointer; }
.btn-cancel { background: #95a5a6; color: white; border: none; padding: 10px; border-radius: 4px; cursor: pointer; }

.tabla-container { background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 5px rgba(0,0,0,0.1); }
table { width: 100%; border-collapse: collapse; }
th, td { padding: 10px; text-align: left; border-bottom: 1px solid #eee; }

.badge-ok { background: #d4edda; color: #155724; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; }
.badge-no { background: #f8d7da; color: #721c24; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; }
.badge-puesto { background: #e3f2fd; color: #0d47a1; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; }

.btn-small { border: none; background: #eee; padding: 5px 8px; border-radius: 4px; cursor: pointer; margin-right: 5px; }
.btn-del { background: #ffcccc; color: red; }
</style>