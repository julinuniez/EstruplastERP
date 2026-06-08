<script setup lang="ts">
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { Alertas } from '@/utils/alertas';

interface Entidad {
  id: number;
  razonSocial?: string;
  cuit?: string;
  contactoNombre?: string; 
  email?: string;
  telefono?: string;
  direccion?: string;
  esFazon?: boolean; 
  activo: boolean;
}

// Ahora la pestaña por defecto es 'clientes'
const pestana = ref<'clientes' | 'proveedores'>('clientes')
const lista = ref<Entidad[]>([]) 
const cargando = ref(false)
const cargandoFazon = ref<number | null>(null)

const itemForm = ref({
  id: 0,
  nombre: '', 
  identificacion: '', 
  contacto: '',
  email: '',
  telefono: '',
  direccion: '',
  esFazon: false,
  activo: true
})

const modoEdicion = ref(false)
<<<<<<< HEAD
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:5122/api'; 
=======
const apiUrl = '/api';  
>>>>>>> master

const getAuthConfig = () => {
  const token = localStorage.getItem('token');
  return { headers: { Authorization: `Bearer ${token}` } };
};

async function cargarDatos() {
  cargando.value = true;
  lista.value = [];
  
  const endpoints = {
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
    Alertas.advertencia("La Razón Social es obligatoria.");
    return;
  }

  const endpoints = {
    clientes: 'Clientes',
    proveedores: 'Proveedores'
  };
  const endpoint = endpoints[pestana.value];

  let payload: any = {
    id: itemForm.value.id,
    activo: itemForm.value.activo,
    razonSocial: itemForm.value.nombre,
    cuit: itemForm.value.identificacion
  };

  if (pestana.value === 'clientes') {
    payload.esFazon = itemForm.value.esFazon;
  }

  if (pestana.value === 'proveedores') {
    payload.contactoNombre = itemForm.value.contacto;
    payload.email = itemForm.value.email;
    payload.telefono = itemForm.value.telefono;
    payload.direccion = itemForm.value.direccion;
  }

  try {
    if (modoEdicion.value) {
      await axios.put(`${apiUrl}/${endpoint}/${itemForm.value.id}`, payload, getAuthConfig());
      Alertas.exito("Registro actualizado correctamente.");
    } else {
      payload.id = 0; 
      await axios.post(`${apiUrl}/${endpoint}`, payload, getAuthConfig());
      Alertas.exito("Registro creado correctamente.");
    }
    limpiarForm();
    cargarDatos();
  } catch (error: any) {
    console.error(error);
    Alertas.error("Error: " + (error.response?.data || error.message));
  }
}

function editar(item: Entidad) {
  modoEdicion.value = true;
  
  itemForm.value = {
    id: item.id,
    nombre: item.razonSocial || '',
    identificacion: item.cuit || '',
    contacto: item.contactoNombre || '',
    email: item.email || '',
    telefono: item.telefono || '',
    direccion: item.direccion || '',
    esFazon: item.esFazon || false,
    activo: item.activo
  };
}

async function eliminar(id: number) {
  if (!await Alertas.confirmar("¿Eliminar registro?", "¿Estás seguro de eliminar/desactivar este registro?")) return;
  const endpoints = { clientes: 'Clientes', proveedores: 'Proveedores' };
  
  try {
    await axios.delete(`${apiUrl}/${endpoints[pestana.value]}/${id}`, getAuthConfig());
    Alertas.exito("Registro eliminado/desactivado correctamente.");
    cargarDatos();
  } catch (error: any) {
    Alertas.error("Error: " + (error.response?.data?.mensaje || error.message));
  }
}

function limpiarForm() {
  modoEdicion.value = false;
  itemForm.value = {
    id: 0, nombre: '', identificacion: '',
    contacto: '', email: '', telefono: '', direccion: '', esFazon: false, activo: true
  };
}

const toggleFazon = async (cliente: Entidad) => {
    if (cliente.esFazon) {
        Alertas.advertencia("Este cliente ya está habilitado para servicios.");
        return;
    }

    if (!await Alertas.confirmar("Habilitar Fazón", `¿Habilitar a ${cliente.razonSocial} para operar con Fazón?`)) return;

    cargandoFazon.value = cliente.id;
    try {
        const res = await axios.post(`${apiUrl}/Clientes/habilitar-fazon/${cliente.id}`, {}, getAuthConfig());
        cliente.esFazon = true;
        Alertas.exito(res.data.mensaje);
    } catch (e: any) {
        Alertas.error("Error: " + (e.response?.data?.mensaje || e.message));
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
        <button :class="{ active: pestana==='clientes' }" @click="pestana='clientes'; limpiarForm(); cargarDatos()">🏢 Clientes</button>
        <button :class="{ active: pestana==='proveedores' }" @click="pestana='proveedores'; limpiarForm(); cargarDatos()">🚚 Proveedores</button>
    </div>

    <div class="contenido-abm">
        <div class="card-form">
            <h3>{{ modoEdicion ? 'Editar' : 'Nuevo' }} {{ pestana.toUpperCase() }}</h3>
            
            <label>Razón Social / Nombre:</label>
            <input type="text" v-model="itemForm.nombre" placeholder="Empresa S.A. o Nombre">

            <label>CUIT:</label>
            <input type="text" v-model="itemForm.identificacion" placeholder="XX-XXXXXXXX-X">

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
                        <th>Razón Social</th>
                        <th>CUIT</th>
                        <th v-if="pestana==='proveedores'">Contacto</th> 
                        <th>Estado</th>
                        <th>Acciones</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in lista" :key="item.id">
                        <td>
                            <strong>{{ item.razonSocial }}</strong>
                            <div v-if="pestana==='proveedores' && item.email" style="font-size:0.8em; color:#666; margin-top: 4px;">
                                📧 {{ item.email }}
                            </div>
                        </td>
                        
                        <td>{{ item.cuit || '-' }}</td>
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
.tabs button { background: none; border: none; font-size: 1rem; cursor: pointer; padding: 10px 15px; color: #7f8c8d; transition: all 0.3s; border-bottom: 3px solid transparent; font-weight: 500; }
.tabs button:hover { color: #3498db; background-color: #f8f9fa; }
.tabs button.active { color: #2980b9; font-weight: bold; border-bottom: 3px solid #3498db; }

.contenido-abm { display: grid; grid-template-columns: 350px 1fr; gap: 25px; align-items: start; }
@media (max-width: 900px) { .contenido-abm { grid-template-columns: 1fr; } }

.card-form { background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e1e1e1; position: sticky; top: 20px; }
.card-form label { display: block; margin-bottom: 5px; font-weight: 600; font-size: 0.9rem; color: #555; }
.card-form input[type="text"], .card-form select { width: 100%; padding: 10px; margin-bottom: 15px; border: 1px solid #ccc; border-radius: 5px; box-sizing: border-box; font-size: 0.95rem; }
.card-form input:focus, .card-form select:focus { outline: none; border-color: #3498db; box-shadow: 0 0 0 2px rgba(52, 152, 219, 0.2); }

.check-box { margin: 10px 0 20px 0; display: flex; gap: 8px; align-items: center; cursor: pointer; background: #f8f9fa; padding: 10px; border-radius: 5px; }
.fazon-check { background: #f3e5f5; border: 1px solid #e1bee7; color: #8e44ad; font-weight: bold; }
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

.btn-small { border: none; background: #ecf0f1; width: 32px; height: 32px; border-radius: 4px; cursor: pointer; margin-right: 5px; transition: all 0.2s; display: inline-flex; align-items: center; justify-content: center; font-size: 1rem; }
.btn-small:hover { background: #bdc3c7; transform: translateY(-2px); }
.btn-del { background: #ffecec; color: #e74c3c; }
.btn-del:hover { background: #fadbd8; color: #c0392b; }

.btn-fazon-activo { background-color: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
.btn-fazon-inactivo { background-color: #f3e5f5; color: #8e44ad; border: 1px solid #e1bee7; }
.btn-fazon-inactivo:hover { background-color: #e1bee7; color: #6c3483; }
</style>