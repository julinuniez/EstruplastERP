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

// ✅ NUEVO: Variable para mostrar spinner en el botón específico que se está clickeando
const cargandoFazon = ref<number | null>(null)

const listaPuestos = [
    'Operario General', 'Logística', 'Extrusor', 'Impresor', 
    'Supervisor', 'Mantenimiento', 'Administración', 'Gerencia', 'Chofer'
]

// ... (Resto de tus variables itemForm, modoEdicion, apiUrl, getAuthConfig igual que antes) ...
// ... (Tus funciones cargarDatos, guardar, editar, eliminar, limpiarForm igual que antes) ...

const itemForm = ref<{
    id: number;
    nombre: string;
    dni: string;
    cuit: string;
    puesto: string;
    activo: boolean;
}>({ 
    id: 0, nombre: '', dni: '', cuit: '', puesto: 'Operario General', activo: true 
})

const modoEdicion = ref(false)
const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';  

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// ... (Tus funciones existentes CRUD van aquí) ...
// --- FUNCIONES CRUD IMPLEMENTADAS ---

// 1. Cargar Datos (GET)
async function cargarDatos() {
    cargando.value = true;
    lista.value = [];
    
    // Determinamos el endpoint según la pestaña activa
    const endpoint = pestana.value === 'clientes' ? 'Clientes' : 'Empleados';

    try {
        const res = await axios.get(`${apiUrl}/${endpoint}`, getAuthConfig());
        lista.value = res.data;
    } catch (error) {
        console.error("Error cargando datos:", error);
    } finally {
        cargando.value = false;
    }
}

// 2. Guardar (POST / PUT) - CON MAPEO DE DATOS
async function guardar() {
    // Validación básica frontend
    if (!itemForm.value.nombre) {
        alert("El Nombre / Razón Social es obligatorio.");
        return;
    }

    const endpoint = pestana.value === 'clientes' ? 'Clientes' : 'Empleados';
    
    // PREPARAR EL OBJETO PARA EL BACKEND (Mapeo)
    // C# espera propiedades exactas (RazonSocial, NombreCompleto, etc.)
    let payload: any = {
        id: itemForm.value.id,
        activo: itemForm.value.activo
    };

    if (pestana.value === 'clientes') {
        // Mapeo para Cliente
        payload.razonSocial = itemForm.value.nombre; 
        payload.cuit = itemForm.value.cuit;
    } else {
        // Mapeo para Empleado
        payload.nombreCompleto = itemForm.value.nombre;
        payload.dni = itemForm.value.dni;
        payload.puesto = itemForm.value.puesto;
    }

    try {
        if (modoEdicion.value) {
            // EDITAR (PUT)
            await axios.put(`${apiUrl}/${endpoint}/${itemForm.value.id}`, payload, getAuthConfig());
            alert("Editado correctamente");
        } else {
            // CREAR (POST)
            // Aseguramos que el ID no vaya o sea 0 para que la DB cree uno nuevo
            payload.id = 0; 
            await axios.post(`${apiUrl}/${endpoint}`, payload, getAuthConfig());
            alert("Creado correctamente");
        }
        
        limpiarForm();
        cargarDatos(); // Recargar la tabla

    } catch (error: any) {
        console.error("Error al guardar:", error);
        // Intentar mostrar el mensaje exacto que manda el backend
        const mensajeBackend = error.response?.data || error.message;
        alert("Error al guardar: " + mensajeBackend);
    }
}

// 3. Editar (Cargar formulario con datos existentes)
function editar(item: Entidad) {
    modoEdicion.value = true;

    // Mapeo INVERSO (Backend -> Formulario)
    // Si es cliente usa razonSocial, si es empleado usa nombreCompleto
    const nombre = item.razonSocial || item.nombreCompleto || '';
    const identificacion = item.cuit || item.dni || '';

    itemForm.value = {
        id: item.id,
        nombre: nombre,
        dni: pestana.value === 'empleados' ? identificacion : '',
        cuit: pestana.value === 'clientes' ? identificacion : '',
        puesto: item.puesto || 'Operario General',
        activo: item.activo
    };
}

// 4. Eliminar (DELETE)
async function eliminar(id: number) {
    if (!confirm("¿Estás seguro de eliminar/desactivar este registro?")) return;

    const endpoint = pestana.value === 'clientes' ? 'Clientes' : 'Empleados';

    try {
        await axios.delete(`${apiUrl}/${endpoint}/${id}`, getAuthConfig());
        alert("Eliminado correctamente");
        cargarDatos();
    } catch (error: any) {
        console.error("Error al eliminar:", error);
        alert("No se pudo eliminar: " + (error.response?.data || error.message));
    }
}

// 5. Limpiar Formulario
function limpiarForm() {
    modoEdicion.value = false;
    itemForm.value = {
        id: 0,
        nombre: '',
        dni: '',
        cuit: '',
        puesto: 'Operario General',
        activo: true
    };
}


// ✅ NUEVA FUNCIÓN: Habilitar Fazon
async function habilitarFazon(cliente: Entidad) {
    if (!cliente.razonSocial) return;

    if (!confirm(`¿Desea crear la cuenta de stock de material para ${cliente.razonSocial}?`)) return;

    cargandoFazon.value = cliente.id; // Activamos spinner solo para este ID

    try {
        // Llamada al endpoint mágico que creamos en el Backend
        const res = await axios.post(`${apiUrl}/Productos/habilitar-fazon/${cliente.id}`, {}, getAuthConfig());
        
        alert(res.data.mensaje); // "✅ Fazon habilitado..."
        
    } catch (e: any) {
        console.error(e);
        // Si ya existe o hay error, mostramos el mensaje del backend
        const msg = e.response?.data || "Error al habilitar Fazon.";
        alert(msg);
    } finally {
        cargandoFazon.value = null; // Liberamos el botón
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
                            <button @click="editar(item)" class="btn-small" title="Editar">✏️</button>
                            <button @click="eliminar(item.id)" class="btn-small btn-del" title="Eliminar/Desactivar">🗑️</button>
                            
                            <button 
                                v-if="pestana === 'clientes' && item.activo" 
                                @click="habilitarFazon(item)" 
                                class="btn-small btn-fazon"
                                :disabled="cargandoFazon === item.id"
                                title="Habilitar servicio de Fazon (Crear Stock)"
                            >
                                <span v-if="cargandoFazon === item.id">⏳</span>
                                <span v-else>🏭</span>
                            </button>
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
/* ... tus estilos anteriores ... */

.btn-small { border: none; background: #eee; padding: 5px 8px; border-radius: 4px; cursor: pointer; margin-right: 5px; }
.btn-del { background: #ffcccc; color: red; }

/* ✅ NUEVO ESTILO */
.btn-fazon {
    background-color: #8e44ad; /* Violeta */
    color: white;
    transition: background 0.3s;
}
.btn-fazon:hover {
    background-color: #9b59b6; /* Violeta claro */
}
.btn-fazon:disabled {
    background-color: #ccc;
    cursor: not-allowed;
}
</style>