<script setup lang="ts">
import { ref, onMounted } from 'vue';

// --- Estado ---
const listaProductosTerminados = ref([]);
const listaClientes = ref([]); 
const carrito = ref([]); 
const cargando = ref(false);

const datosRemito = ref({
    clienteNombre: '', // Guardamos la Razón Social aquí
    numero: '',
    fecha: new Date().toISOString().split('T')[0]
});

// Variables temporales
const lineaTemp = ref({ productoId: '', cantidad: 1 });

// --- Carga Inicial ---
onMounted(async () => {
    await cargarProductos();
    await cargarClientes();
});

async function cargarProductos() {
    try {
        // Asegúrate que esta URL sea la correcta según tu StockController
        const res = await fetch('https://localhost:7244/api/Stock/inventario'); 
        if (res.ok) {
            const todos = await res.json();
            // Filtramos solo productos terminados
            listaProductosTerminados.value = todos.filter((p: any) => p.esProductoTerminado);
        }
    } catch (e) { console.error(e); }
}

async function cargarClientes() {
    try {
        const res = await fetch('https://localhost:7244/api/Clientes');
        if (res.ok) {
            listaClientes.value = await res.json();
        }
    } catch (e) { console.error("Error cargando clientes", e); }
}

// 🚨 CREAR CLIENTE RÁPIDO ADAPTADO A TU CLASE
async function crearClienteRapido() {
    const razonSocial = prompt("Ingrese la Razón Social del Nuevo Cliente:");
    if (!razonSocial) return;

    try {
        const res = await fetch('https://localhost:7244/api/Clientes', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            // Enviamos el objeto tal cual lo espera tu clase C#
            body: JSON.stringify({ 
                razonSocial: razonSocial, 
                activo: true,
                direccion: '', // Campos opcionales vacíos para evitar error
                telefono: '',
                email: ''
            })
        });

        if (res.ok) {
            await cargarClientes(); 
            datosRemito.value.clienteNombre = razonSocial; 
            alert("✅ Cliente agregado.");
        } else {
            alert("Error al guardar cliente. Revisa la consola.");
        }
    } catch (e) { alert("Error de conexión"); }
}

const agregarAlCarrito = () => {
    const pid = Number(lineaTemp.value.productoId);
    const cant = Number(lineaTemp.value.cantidad);
    
    if (!pid || cant <= 0) return alert("Selecciona producto y cantidad.");

    const prod: any = listaProductosTerminados.value.find((p: any) => p.id === pid);
    
    // Buscar si ya existe en el carrito para sumar cantidad
    const existe: any = carrito.value.find((item: any) => item.productoId === pid);
    
    if (existe) {
        existe.cantidad += cant;
    } else {
        carrito.value.push({
            productoId: pid,
            nombre: prod.nombre,
            sku: prod.codigoSku,
            cantidad: cant
        });
    }
    // Resetear input
    lineaTemp.value.productoId = '';
    lineaTemp.value.cantidad = 1;
};

const quitarDelCarrito = (index: number) => {
    carrito.value.splice(index, 1);
};

const procesarRemito = async () => {
    if (carrito.value.length === 0) return alert("El remito está vacío.");
    if (!datosRemito.value.clienteNombre || !datosRemito.value.numero) return alert("Faltan datos.");

    cargando.value = true;
    try {
        const payload = {
            cliente: datosRemito.value.clienteNombre, 
            numeroRemito: datosRemito.value.numero,
            items: carrito.value.map((i: any) => ({
                productoId: i.productoId,
                cantidad: i.cantidad
            }))
        };

        const res = await fetch('https://localhost:7244/api/Stock/registrar-remito', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        const data = await res.json();
        if (!res.ok) throw new Error(data.mensaje);

        alert("🚚 Despacho Exitoso!");
        carrito.value = [];
        datosRemito.value.numero = '';
        datosRemito.value.clienteNombre = '';
        
        await cargarProductos(); // Actualizar stock visual

    } catch (e: any) {
        alert("❌ Error: " + e.message);
    } finally {
        cargando.value = false;
    }
};
</script>

<template>
  <div class="panel-remitos">
    <div class="header-remito">
        <h2>🚚 Remito / Salida de Stock</h2>
        <p>Solo productos terminados.</p>
    </div>

    <div class="card-datos">
        <div class="fila">
            <div class="col-cliente">
                <label>Cliente / Destino:</label>
                <div class="input-group">
                    <select v-model="datosRemito.clienteNombre">
                        <option value="">Seleccionar Cliente...</option>
                        <option v-for="c in listaClientes" :key="c.id" :value="c.razonSocial">
                            {{ c.razonSocial }}
                        </option>
                    </select>
                    <button @click="crearClienteRapido" class="btn-small" title="Nuevo Cliente">➕</button>
                </div>
            </div>
            <div>
                <label>N° Remito:</label>
                <input v-model="datosRemito.numero" type="text" placeholder="0001-00001234">
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
            <select v-model="lineaTemp.productoId" class="select-prod">
                <option value="">Seleccionar Producto Terminado...</option>
                <option v-for="p in listaProductosTerminados" :key="p.id" :value="p.id">
                    {{ p.codigoSku }} - {{ p.nombre }} (Stock: {{ p.stockActual }})
                </option>
            </select>
            <input v-model.number="lineaTemp.cantidad" type="number" placeholder="Cant." class="input-cant">
            <button @click="agregarAlCarrito" class="btn-add">⬇️ Agregar</button>
        </div>
    </div>

    <div class="tabla-container">
        <table class="tabla-remito">
            <thead>
                <tr>
                    <th>SKU</th>
                    <th>Producto</th>
                    <th>Cantidad</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in carrito" :key="index">
                    <td>{{ item.sku }}</td>
                    <td>{{ item.nombre }}</td>
                    <td class="numero">{{ item.cantidad }}</td>
                    <td style="text-align: center;">
                        <button @click="quitarDelCarrito(index)" class="btn-del">❌</button>
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
/* Estilos igual que antes */
.panel-remitos { max-width: 900px; margin: 0 auto; background: white; padding: 25px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }
.header-remito { border-bottom: 2px solid #eee; margin-bottom: 20px; }
.card-datos, .card-items { background: #f8f9fa; padding: 15px; border-radius: 6px; border: 1px solid #ddd; margin-bottom: 20px; }
.fila { display: flex; gap: 20px; }
.fila div { flex: 1; }
.col-cliente { flex: 2 !important; } 
.input-group { display: flex; gap: 5px; }
.btn-small { background: #27ae60; color: white; border: none; border-radius: 4px; cursor: pointer; font-weight: bold; width: 35px; }
label { display: block; font-weight: bold; font-size: 0.9em; margin-bottom: 5px; }
input, select { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
.fila-agregar { display: flex; gap: 10px; }
.select-prod { flex-grow: 1; }
.input-cant { width: 100px; }
.btn-add { background: #3498db; color: white; border: none; padding: 0 20px; border-radius: 4px; cursor: pointer; }
.tabla-remito { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla-remito th { background: #2c3e50; color: white; padding: 10px; text-align: left; }
.tabla-remito td { padding: 10px; border-bottom: 1px solid #eee; }
.acciones-finales { display: flex; justify-content: space-between; margin-top: 20px; padding-top: 10px; border-top: 2px solid #eee; }
.btn-confirmar { background: #e67e22; color: white; border: none; padding: 10px 30px; font-size: 1.1em; font-weight: bold; border-radius: 6px; cursor: pointer; }
.btn-del { background: transparent; border: none; cursor: pointer; }
</style>