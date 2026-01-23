<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';

// --- 1. INTERFACES (Tipado Estricto) ---
interface Producto {
    id: number;
    nombre: string;
    codigoSku: string;
    stockActual: number;
    esProductoTerminado: boolean;
}

interface Cliente {
    id: number;
    razonSocial: string;
}

interface ItemCarrito {
    productoId: number;
    nombre: string;
    sku: string;
    cantidad: number;
    detalle: string; // ✅ Campo clave para el color/medida específica
}

// --- ESTADO ---
const listaProductosTerminados = ref<Producto[]>([]);
const listaClientes = ref<Cliente[]>([]); 
const carrito = ref<ItemCarrito[]>([]); 
const cargando = ref(false);

const datosRemito = ref({
    clienteId: '' as number | '', 
    numero: '',
    fecha: new Date().toISOString().split('T')[0] // Por defecto Hoy
});

// Estado temporal para la línea que se está agregando
const itemTemporal = ref({
    productoId: null as number | null,
    cantidad: '' as number | '',
    detalle: '' 
});

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

// --- HELPER TOKEN ---
const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- CARGA INICIAL ---
onMounted(async () => {
    await cargarProductos();
    await cargarClientes();
});

async function cargarProductos() {
    try {
        const res = await axios.get(`${apiUrl}/Productos`, getAuthConfig());
        const todos: Producto[] = res.data;
        listaProductosTerminados.value = todos.filter(p => p.esProductoTerminado);
    } catch (e: any) { 
        console.error("Error productos:", e);
        if (e.response?.status === 401) alert("Sesión expirada");
    }
}

async function cargarClientes() {
    try {
        const res = await axios.get(`${apiUrl}/Clientes`, getAuthConfig());
        listaClientes.value = res.data;
    } catch (e) { console.error("Error clientes", e); }
}

// --- CREAR CLIENTE RÁPIDO ---
async function crearClienteRapido() {
    const razonSocial = prompt("Ingrese la Razón Social del Nuevo Cliente:");
    if (!razonSocial) return;

    try {
        const payload = { 
            razonSocial: razonSocial, 
            activo: true
        };

        const res = await axios.post(`${apiUrl}/Clientes`, payload, getAuthConfig());
        
        await cargarClientes(); 
        if(res.data && res.data.id) {
            datosRemito.value.clienteId = res.data.id;
        }
        alert("✅ Cliente agregado.");

    } catch (e: any) { 
        alert("Error al crear cliente: " + (e.response?.data?.mensaje || e.message)); 
    }
}

// --- 🔥 NUEVO: AUTO-FORMATEO DE REMITO (0000-00000000) ---
const formatearRemito = (evento: Event) => {
    const input = evento.target as HTMLInputElement;
    let valor = input.value.replace(/\D/g, ''); // Borrar todo lo que no sea número

    // Limitar a 12 dígitos (4 + 8)
    if (valor.length > 12) valor = valor.slice(0, 12);

    // Aplicar máscara 0000-00000000
    if (valor.length > 4) {
        valor = valor.slice(0, 4) + '-' + valor.slice(4);
    }

    datosRemito.value.numero = valor; // Actualizar modelo
};

// --- LÓGICA CARRITO ---
const agregarItem = () => {
    const pid = itemTemporal.value.productoId;
    const cant = Number(itemTemporal.value.cantidad);
    const detalleTexto = itemTemporal.value.detalle.trim();
    
    if (!pid || cant <= 0) {
        alert("Selecciona producto y cantidad válida.");
        return;
    }

    const prod = listaProductosTerminados.value.find(p => p.id === pid);
    if (!prod) return;
    
    carrito.value.push({
        productoId: pid,
        nombre: prod.nombre,
        sku: prod.codigoSku,
        cantidad: cant,
        detalle: detalleTexto 
    });
    
    itemTemporal.value = { productoId: null, cantidad: '', detalle: '' };
};

const quitarDelCarrito = (index: number) => {
    carrito.value.splice(index, 1);
};

const procesarRemito = async () => {
    if (carrito.value.length === 0) return alert("El remito está vacío.");
    if (!datosRemito.value.clienteId) return alert("Seleccione un Cliente.");

    // Validación de formato estricta antes de enviar
    const regex = /^\d{4}-\d{8}$/;
    if (!datosRemito.value.numero || !regex.test(datosRemito.value.numero)) {
        return alert("⚠️ Error: El N° de Remito debe tener el formato 0000-00000000 (4 dígitos, guion, 8 dígitos).");
    }

    cargando.value = true;
    try {
        const payload = {
            clienteId: datosRemito.value.clienteId, 
            numeroRemito: datosRemito.value.numero, 
            fecha: datosRemito.value.fecha,
            observacion: `Generado desde Despacho.`,
            items: carrito.value.map(i => ({
                productoId: i.productoId,
                cantidad: i.cantidad,
                detalle: i.detalle 
            }))
        };

        await axios.post(`${apiUrl}/Remitos`, payload, getAuthConfig());

        alert("🚚 Despacho Exitoso! Stock descontado.");
        
        carrito.value = [];
        datosRemito.value.numero = '';
        datosRemito.value.clienteId = ''; 
        
        await cargarProductos(); 

    } catch (e: any) {
        console.error(e);
        const mensaje = e.response?.data || e.message || "Error desconocido";
        alert("❌ Error al procesar: " + mensaje);
    } finally {
        cargando.value = false;
    }
};
</script>

<template>
  <div class="panel-remitos">
    <div class="header-remito">
        <h2>🚚 Remito / Salida de Stock</h2>
        <p>Seleccione producto genérico y aclare color/medida en "Detalle".</p>
    </div>

    <div class="card-datos">
        <div class="fila">
            <div class="col-cliente">
                <label>Cliente / Destino:</label>
                <div class="input-group">
                    <select v-model="datosRemito.clienteId">
                        <option value="">Seleccionar Cliente...</option>
                        <option v-for="c in listaClientes" :key="c.id" :value="c.id">
                            {{ c.razonSocial }}
                        </option>
                    </select>
                    <button @click="crearClienteRapido" class="btn-small" title="Nuevo Cliente">➕</button>
                </div>
            </div>
            
            <div>
                <label>N° Remito <span style="color:red">*</span>:</label>
                <input 
                    type="text" 
                    :value="datosRemito.numero"
                    @input="formatearRemito"
                    placeholder="0001-00001234" 
                    maxlength="13"
                    class="input-remito"
                />
                <small v-if="datosRemito.numero && !/^\d{4}-\d{8}$/.test(datosRemito.numero)" style="color: #e74c3c; font-size: 0.8em; display: block; margin-top: 2px;">
                    Formato requerido: 0000-00000000
                </small>
            </div>

            <div>
                <label>Fecha:</label>
                <input v-model="datosRemito.fecha" type="date">
            </div>
        </div>
    </div>

    <div class="card-items">
        <h3>📦 Agregar Productos</h3>
        <div class="fila-agregar">
            <select v-model="itemTemporal.productoId" class="select-prod">
                <option :value="null">Seleccionar Producto...</option>
                <option v-for="p in listaProductosTerminados" :key="p.id" :value="p.id">
                    {{ p.nombre }} (Stock: {{ p.stockActual }})
                </option>
            </select>

            <input type="number" 
                   v-model="itemTemporal.cantidad" 
                   placeholder="Kg" 
                   class="input-cant">

            <input type="text" 
                   v-model="itemTemporal.detalle" 
                   placeholder="Detalle (Ej: Rojo, 40 micrones)" 
                   class="input-detalle">

            <button @click="agregarItem" class="btn-add">➕</button>
        </div>
    </div>

    <div class="tabla-container">
        <table class="tabla-remito">
            <thead>
                <tr>
                    <th>SKU</th>
                    <th>Producto</th>
                    <th>Detalle / Color</th> <th>Cantidad</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in carrito" :key="index">
                    <td>{{ item.sku }}</td>
                    <td>{{ item.nombre }}</td>
                    
                    <td style="font-style: italic; color: #555;">
                        {{ item.detalle || '-' }}
                    </td>
                    
                    <td class="numero"><strong>{{ item.cantidad }} kg</strong></td>
                    <td style="text-align: center;">
                        <button @click="quitarDelCarrito(index)" class="btn-del">❌</button>
                    </td>
                </tr>
                <tr v-if="carrito.length === 0">
                    <td colspan="5" style="text-align: center; color: #888; padding: 20px;">
                        El remito está vacío. Agregue productos arriba.
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="acciones-finales">
        <div class="resumen">Items: {{ carrito.length }}</div>
        <button @click="procesarRemito" :disabled="cargando" class="btn-confirmar">
            {{ cargando ? 'Guardando...' : '✅ CONFIRMAR SALIDA' }}
        </button>
    </div>

  </div>
</template>

<style scoped>
.panel-remitos { max-width: 900px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.header-remito { border-bottom: 2px solid #eee; margin-bottom: 20px; }
.card-datos, .card-items { background: #f8f9fa; padding: 15px; border-radius: 6px; border: 1px solid #ddd; margin-bottom: 20px; }
.fila { display: flex; gap: 20px; }
.fila div { flex: 1; }
.col-cliente { flex: 2 !important; } 
.input-group { display: flex; gap: 5px; }
.btn-small { background: #27ae60; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; width: 35px; }
label { display: block; font-weight: bold; font-size: 0.9em; margin-bottom: 5px; }
input, select { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; box-sizing: border-box; }

/* Estilo especial para el input de remito */
.input-remito {
    font-family: 'Courier New', monospace;
    font-weight: bold;
    letter-spacing: 1px;
    font-size: 1.1rem;
    text-align: center;
    border: 2px solid #3498db; /* Borde azul para destacar */
}

.fila-agregar { display: flex; gap: 10px; align-items: center; }
.select-prod { flex-grow: 2; }
.input-cant { width: 100px; }
.input-detalle { flex-grow: 2; } 
.btn-add { background: #3498db; color: white; border: none; padding: 10px 20px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-add:hover { background: #2980b9; }

.tabla-remito { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla-remito th { background: #2c3e50; color: white; padding: 10px; text-align: left; }
.tabla-remito td { padding: 10px; border-bottom: 1px solid #eee; }
.acciones-finales { display: flex; justify-content: space-between; align-items: center; margin-top: 20px; padding-top: 10px; border-top: 2px solid #eee; }
.btn-confirmar { background: #e67e22; color: white; border: none; padding: 12px 30px; font-size: 1.1em; font-weight: bold; border-radius: 6px; cursor: pointer; transition: background 0.2s; }
.btn-confirmar:hover { background: #d35400; }
.btn-confirmar:disabled { background: #ccc; cursor: not-allowed; }
.btn-del { background: transparent; border: none; cursor: pointer; font-size: 1.2em; }
.resumen { font-weight: bold; color: #555; }

@media (max-width: 700px) {
    .fila { flex-direction: column; gap: 10px; }
    .fila-agregar { flex-direction: column; }
    .input-cant, .input-detalle, .select-prod { width: 100%; }
}
</style>