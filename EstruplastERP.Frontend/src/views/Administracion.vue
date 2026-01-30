<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'

interface Entidad {
  id: number;
  nombreCompleto?: string;
  dni?: string;
  puesto?: string;
  razonSocial?: string;
  cuit?: string;
  contactoNombre?: string; 
  email?: string;
  telefono?: string;
  direccion?: string;
  esFazon?: boolean; 
  activo: boolean;
}

const pestana = ref<'empleados' | 'clientes' | 'proveedores'>('empleados')
const lista = ref<Entidad[]>([]) 
const cargando = ref(false)
const cargandoFazon = ref<number | null>(null)

const listaPuestos = ['Operario General', 'Logística', 'Extrusor', 'Impresor', 'Supervisor', 'Mantenimiento', 'Administración', 'Gerencia', 'Chofer']

const itemForm = ref({
  id: 0,
  nombre: '', 
  identificacion: '', 
  puesto: 'Operario General',
  contacto: '',
  email: '',
  telefono: '',
  direccion: '',
  esFazon: false,
  activo: true
})

const modoEdicion = ref(false)
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api'; 

const getAuthConfig = () => {
  const token = localStorage.getItem('token');
  return { headers: { Authorization: `Bearer ${token}` } };
};

async function cargarDatos() {
  cargando.value = true;
  lista.value = [];
  
  const endpoints = {
    empleados: 'Empleados',
    clientes: 'Clientes',
    proveedores: 'Proveedores'
  };
  
  try {
    const res = await axios.get(`${apiUrl}/${endpoints[pestana.value]}`, getAuthConfig());
    lista.value = res.data;
  } catch (error) {
    console.error(error);
  } finally {
    cargando.value = false;
  }
}

async function guardar() {
  if (!itemForm.value.nombre) {
    alert("El Nombre / Razón Social es obligatorio.");
    return;
  }

  const endpoints = {
    empleados: 'Empleados',
    clientes: 'Clientes',
    proveedores: 'Proveedores'
  };
  const endpoint = endpoints[pestana.value];

  let payload: any = {
    id: itemForm.value.id,
    activo: itemForm.value.activo
  };

  if (pestana.value === 'empleados') {
    payload.nombreCompleto = itemForm.value.nombre;
    payload.dni = itemForm.value.identificacion;
    payload.puesto = itemForm.value.puesto;
  } else {
    payload.razonSocial = itemForm.value.nombre;
    payload.cuit = itemForm.value.identificacion;

    if (pestana.value === 'clientes') {
      payload.esFazon = itemForm.value.esFazon;
    }

    if (pestana.value === 'proveedores') {
      payload.contactoNombre = itemForm.value.contacto;
      payload.email = itemForm.value.email;
      payload.telefono = itemForm.value.telefono;
      payload.direccion = itemForm.value.direccion;
    }
  }

  try {
    if (modoEdicion.value) {
      await axios.put(`${apiUrl}/${endpoint}/${itemForm.value.id}`, payload, getAuthConfig());
    } else {
      payload.id = 0; 
      await axios.post(`${apiUrl}/${endpoint}`, payload, getAuthConfig());
    }
    limpiarForm();
    cargarDatos();
  } catch (error: any) {
    console.error(error);
    alert("Error: " + (error.response?.data || error.message));
  }
}

function editar(item: Entidad) {
  modoEdicion.value = true;
  
  itemForm.value = {
    id: item.id,
    nombre: item.razonSocial || item.nombreCompleto || '',
    identificacion: item.cuit || item.dni || '',
    puesto: item.puesto || 'Operario General',
    contacto: item.contactoNombre || '',
    email: item.email || '',
    telefono: item.telefono || '',
    direccion: item.direccion || '',
    esFazon: item.esFazon || false,
    activo: item.activo
  };
}

async function eliminar(id: number) {
  if (!confirm("¿Estás seguro de eliminar/desactivar este registro?")) return;
  const endpoints = { empleados: 'Empleados', clientes: 'Clientes', proveedores: 'Proveedores' };
  
  try {
    await axios.delete(`${apiUrl}/${endpoints[pestana.value]}/${id}`, getAuthConfig());
    cargarDatos();
  } catch (error: any) {
    alert("Error: " + (error.response?.data?.mensaje || error.message));
  }
}

function limpiarForm() {
  modoEdicion.value = false;
  itemForm.value = {
    id: 0, nombre: '', identificacion: '', puesto: 'Operario General',
    contacto: '', email: '', telefono: '', direccion: '', esFazon: false, activo: true
  };
}

const toggleFazon = async (cliente: Entidad) => {
    if (cliente.esFazon) {
        alert("Este cliente ya está habilitado para servicios.");
        return;
    }

    if (!confirm(`¿Habilitar a ${cliente.razonSocial} para operar con Fazón?`)) return;

    cargandoFazon.value = cliente.id;
    try {
        const res = await axios.post(`${apiUrl}/Clientes/habilitar-fazon/${cliente.id}`, {}, getAuthConfig());
        cliente.esFazon = true;
        alert(res.data.mensaje);
    } catch (e: any) {
        alert("Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        cargandoFazon.value = null;
    }
};

onMounted(() => cargarDatos())
</script>

<template>
  <div class="panel-admin">
    <h2>⚙️ Administración General</h2>
    
    <div class="tabs">
        <button :class="{ active: pestana==='empleados' }" @click="pestana='empleados'; limpiarForm(); cargarDatos()">👷 Empleados</button>
        <button :class="{ active: pestana==='clientes' }" @click="pestana='clientes'; limpiarForm(); cargarDatos()">🏢 Clientes</button>
        <button :class="{ active: pestana==='proveedores' }" @click="pestana='proveedores'; limpiarForm(); cargarDatos()">🚚 Proveedores</button>
    </div>

    <div class="contenido-abm">
        <div class="card-form">
            <h3>{{ modoEdicion ? 'Editar' : 'Nuevo' }} {{ pestana.toUpperCase() }}</h3>
            
            <label>Nombre / Razón Social:</label>
            <input type="text" v-model="itemForm.nombre" placeholder="Nombre completo o Empresa S.A.">

            <div v-if="pestana==='empleados'">
                <label>DNI / Legajo:</label>
                <input type="text" v-model="itemForm.identificacion">
                <label>Puesto:</label>
                <select v-model="itemForm.puesto">
                    <option v-for="p in listaPuestos" :key="p" :value="p">{{ p }}</option>
                </select>
            </div>
            
            <div v-if="pestana!=='empleados'">
                <label>CUIT:</label>
                <input type="text" v-model="itemForm.identificacion" placeholder="XX-XXXXXXXX-X">
            </div>

            <div v-if="pestana==='clientes'" class="check-box fazon-check">
                <input type="checkbox" v-model="itemForm.esFazon" id="chkFazon">
                <label for="chkFazon">🛠️ Habilitar Servicio Fazón</label>
            </div>

            <div v-if="pestana==='proveedores'" class="campos-extra">
                <label>Contacto (Vendedor):</label>
                <input type="text" v-model="itemForm.contacto">
                <div class="fila-doble">
                    <div><label>Teléfono:</label><input type="text" v-model="itemForm.telefono"></div>
                    <div><label>Email:</label><input type="text" v-model="itemForm.email"></div>
                </div>
                <label>Dirección / Depósito:</label>
                <input type="text" v-model="itemForm.direccion">
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
                        <th v-if="pestana!=='empleados'">CUIT</th>
                        <th v-if="pestana==='proveedores'">Contacto</th> 
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in lista" :key="item.id">
                        <td>
                            {{ item.nombreCompleto || item.razonSocial }}
                            <div v-if="pestana==='proveedores' && item.email" style="font-size:0.8em; color:#666">📧 {{ item.email }}</div>
                        </td>
                        
                        <td v-if="pestana==='empleados'"><span class="badge-puesto">{{ item.puesto || '-' }}</span></td>
                        <td v-if="pestana!=='empleados'">{{ item.cuit || '-' }}</td>
                        <td v-if="pestana==='proveedores'">{{ item.contactoNombre || '-' }}</td>

                        <td>
                            <span :class="item.activo ? 'badge-ok' : 'badge-no'">{{ item.activo ? 'Activo' : 'Inactivo' }}</span>
                        </td>
                        <td>
                            <button @click="editar(item)" class="btn-small" title="Editar">✏️</button>
                            
                            <button 
                                v-if="pestana==='clientes'" 
                                @click="toggleFazon(item)" 
                                class="btn-small"
                                :class="item.esFazon ? 'btn-fazon-activo' : 'btn-fazon-inactivo'"
                                :title="item.esFazon ? 'Servicio Habilitado' : 'Habilitar Fazón'"
                                :disabled="cargandoFazon === item.id">
                                <span v-if="cargandoFazon === item.id">⏳</span>
                                <span v-else>{{ item.esFazon ? '✅' : '🏭' }}</span>
                            </button>

                            <button @click="eliminar(item.id)" class="btn-small btn-del" title="Eliminar">🗑️</button>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
  </div>
</template>

<style scoped>
.panel-admin { max-width: 1200px; margin: 0 auto; font-family: 'Segoe UI', sans-serif; padding: 20px; }
h2 { color: #2c3e50; border-bottom: 2px solid #eee; padding-bottom: 10px; margin-bottom: 20px; }
h3 { margin-top: 0; color: #34495e; font-size: 1.2rem; margin-bottom: 15px; }

.tabs { display: flex; gap: 15px; margin-bottom: 25px; border-bottom: 1px solid #ddd; }
.tabs button { background: none; border: none; font-size: 1rem; cursor: pointer; padding: 10px 15px; color: #7f8c8d; transition: all 0.3s; border-bottom: 3px solid transparent; }
.tabs button:hover { color: #3498db; background-color: #f8f9fa; }
.tabs button.active { color: #2980b9; font-weight: bold; border-bottom: 3px solid #3498db; }

.contenido-abm { display: grid; grid-template-columns: 350px 1fr; gap: 25px; align-items: start; }
@media (max-width: 900px) { .contenido-abm { grid-template-columns: 1fr; } }

.card-form { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e1e1e1; position: sticky; top: 20px; }
.card-form label { display: block; margin-bottom: 5px; font-weight: 600; font-size: 0.9rem; color: #555; }
.card-form input[type="text"], .card-form select { width: 100%; padding: 10px; margin-bottom: 15px; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; font-size: 0.95rem; }
.card-form input:focus, .card-form select:focus { outline: none; border-color: #3498db; box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2); }

.check-box { margin: 10px 0 20px 0; display: flex; gap: 8px; align-items: center; cursor: pointer; background: #f8f9fa; padding: 10px; border-radius: 5px; }
.fazon-check { background: #f3e5f5; border: 1px solid #e1bee7; color: #8e44ad; }
.check-box input { width: 18px; height: 18px; cursor: pointer; }

.fila-doble { display: flex; gap: 10px; }
.fila-doble > div { flex: 1; }
.campos-extra { background: #f0f7ff; padding: 12px; border-radius: 6px; margin-bottom: 15px; border: 1px solid #d6e9ff; }
.campos-extra label { color: #1c5b99; }

.btn-group { display: flex; gap: 10px; margin-top: 10px; }
.btn-save { flex: 1; background: #27ae60; color: white; border: none; padding: 12px; border-radius: 5px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-save:hover { background: #219150; }
.btn-cancel { background: #95a5a6; color: white; border: none; padding: 12px 15px; border-radius: 5px; cursor: pointer; transition: background 0.2s; }
.btn-cancel:hover { background: #7f8c8d; }

.tabla-container { background: white; border-radius: 8px; overflow-x: auto; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e1e1e1; }
table { width: 100%; border-collapse: collapse; min-width: 600px; }
thead { background-color: #f8f9fa; border-bottom: 2px solid #e9ecef; }
th { padding: 15px; text-align: left; font-weight: 600; color: #495057; font-size: 0.9rem; text-transform: uppercase; }
td { padding: 12px 15px; border-bottom: 1px solid #f1f1f1; color: #2c3e50; font-size: 0.95rem; vertical-align: middle; }
tr:hover { background-color: #fcfcfc; }

.badge-ok { background: #d4edda; color: #155724; padding: 4px 8px; border-radius: 12px; font-size: 0.75em; font-weight: bold; border: 1px solid #c3e6cb; }
.badge-no { background: #f8d7da; color: #721c24; padding: 4px 8px; border-radius: 12px; font-size: 0.75em; font-weight: bold; border: 1px solid #f5c6cb; }
.badge-puesto { background: #e3f2fd; color: #0d47a1; padding: 4px 8px; border-radius: 4px; font-size: 0.8em; font-weight: 600; display: inline-block; }

.btn-small { border: none; background: #ecf0f1; width: 32px; height: 32px; border-radius: 4px; cursor: pointer; margin-right: 5px; transition: all 0.2s; display: inline-flex; align-items: center; justify-content: center; font-size: 1rem; }
.btn-small:hover { background: #bdc3c7; transform: translateY(-2px); }
.btn-del { background: #ffecec; color: #e74c3c; }
.btn-del:hover { background: #fadbd8; color: #c0392b; }

.btn-fazon-activo { background-color: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
.btn-fazon-inactivo { background-color: #f3e5f5; color: #8e44ad; border: 1px solid #e1bee7; }
.btn-fazon-inactivo:hover { background-color: #e1bee7; color: #6c3483; }
</style>