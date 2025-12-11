<script setup lang="ts">
import { ref, onMounted } from 'vue';
import axios from 'axios';
// @ts-ignore
import html2pdf from 'html2pdf.js';

// --- INTERFACES ---
interface Producto { id: number; nombre: string; codigoSku: string; stockActual: number; esProductoTerminado: boolean; }
interface Cliente { id: number; razonSocial: string; }
interface ItemCarrito { productoId: number; nombre: string; sku: string; cantidad: number; }

// Interface para el historial
interface RemitoHistorial {
    id: number;
    numeroRemito: string;
    fecha: string;
    clienteNombre: string;
    totalItems: number;
    items: { producto: string; sku: string; cantidad: number }[];
}

// --- ESTADO ---
const pestana = ref<'nuevo' | 'historial'>('nuevo');
const listaProductosTerminados = ref<Producto[]>([]);
const listaClientes = ref<Cliente[]>([]);
const listaHistorial = ref<RemitoHistorial[]>([]); // <--- LISTA HISTORIAL
const carrito = ref<ItemCarrito[]>([]);
const cargando = ref(false);

// Datos para el PDF (remito seleccionado para imprimir)
const remitoParaImprimir = ref<RemitoHistorial | null>(null);

const datosRemito = ref({
    clienteNombre: '',
    numero: '',
    fecha: new Date().toISOString().split('T')[0]
});

const lineaTemp = ref<{ productoId: number | '', cantidad: number }>({ productoId: '', cantidad: 1 });
const apiUrl = 'https://localhost:7244/api';

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- CARGA INICIAL ---
onMounted(async () => {
    await cargarProductos();
    await cargarClientes();
    await cargarHistorial(); // <--- Cargar historial al inicio
});

// --- FUNCIONES DE CARGA ---
async function cargarProductos() {
    try {
        const res = await axios.get(`${apiUrl}/Productos`, getAuthConfig());
        listaProductosTerminados.value = res.data.filter((p: any) => p.esProductoTerminado);
    } catch (e) { console.error(e); }
}

async function cargarClientes() {
    try {
        const res = await axios.get(`${apiUrl}/Clientes`, getAuthConfig());
        listaClientes.value = res.data;
    } catch (e) { console.error(e); }
}

async function cargarHistorial() {
    try {
        const res = await axios.get(`${apiUrl}/Remitos`, getAuthConfig());
        listaHistorial.value = res.data;
    } catch (e) { console.error("Error historial", e); }
}

// ... (Las funciones crearClienteRapido, agregarAlCarrito y quitarDelCarrito SIGUEN IGUAL que antes) ...
// Para ahorrar espacio no las repito aquí, pero NO las borres.
// Solo copiaremos la parte nueva del historial y PDF.

// --- LÓGICA CARRITO (Resumida para contexto) ---
const agregarAlCarrito = () => {
    const pid = Number(lineaTemp.value.productoId);
    const cant = Number(lineaTemp.value.cantidad);
    if (!pid || cant <= 0) return alert("Datos inválidos");
    const prod = listaProductosTerminados.value.find(p => p.id === pid);
    if (!prod) return;
    
    const existe = carrito.value.find(i => i.productoId === pid);
    if (existe) existe.cantidad += cant;
    else carrito.value.push({ productoId: pid, nombre: prod.nombre, sku: prod.codigoSku, cantidad: cant });
    
    lineaTemp.value.productoId = ''; lineaTemp.value.cantidad = 1;
};
const quitarDelCarrito = (i: number) => carrito.value.splice(i, 1);
async function crearClienteRapido() { /* Tu lógica anterior va aquí */ }


// --- PROCESAR REMITO (Guardar) ---
const procesarRemito = async () => {
    if (carrito.value.length === 0 || !datosRemito.value.clienteNombre || !datosRemito.value.numero) {
        return alert("Faltan datos.");
    }

    cargando.value = true;
    try {
        const payload = {
            cliente: datosRemito.value.clienteNombre,
            numeroRemito: datosRemito.value.numero,
            items: carrito.value.map(i => ({ productoId: i.productoId, cantidad: i.cantidad }))
        };

        await axios.post(`${apiUrl}/Stock/registrar-remito`, payload, getAuthConfig());

        alert("🚚 Despacho Exitoso!");
        carrito.value = [];
        datosRemito.value.numero = '';
        datosRemito.value.clienteNombre = '';
        
        await cargarProductos();
        await cargarHistorial(); // Actualizamos la lista del historial
        pestana.value = 'historial'; // Vamos a la pestaña historial

    } catch (e: any) {
        alert("❌ Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        cargando.value = false;
    }
};

// --- IMPRIMIR PDF ---
const prepararImpresion = (remito: RemitoHistorial) => {
    // 1. Cargamos los datos para que el v-if muestre el HTML
    remitoParaImprimir.value = remito;

    // 2. Esperamos un poco a que Vue dibuje el HTML oculto
    setTimeout(() => {
        const elemento = document.getElementById('remito-pdf');

        // VALIDACIÓN DE SEGURIDAD:
        // Si por alguna razón el elemento no apareció, evitamos el error
        if (!elemento) {
            alert("Error: No se pudo generar la vista previa del PDF.");
            return;
        }

        const opt = {
            margin: 10, // Un poco de margen ayuda
            filename: `Remito_${remito.numeroRemito}.pdf`,
            image: { type: 'jpeg', quality: 0.98 },
            html2canvas: { scale: 2 }, // Mejor calidad
            jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' }
        };

        // 3. Ignoramos el error de tipo de TypeScript y generamos
        // @ts-ignore
        html2pdf().set(opt).from(elemento).save();

    }, 300); // Aumentamos un poco el tiempo (300ms) para asegurar que se renderice
};
</script>

<template>
  <div class="panel-remitos">
    
    <div class="tabs">
        <button :class="{ active: pestana === 'nuevo' }" @click="pestana = 'nuevo'">➕ Nuevo Despacho</button>
        <button :class="{ active: pestana === 'historial' }" @click="pestana = 'historial'">📂 Historial</button>
    </div>

    <div v-if="pestana === 'nuevo'">
        <div class="header-remito">
            <h2>🚚 Nuevo Remito</h2>
            <p>Salida de stock</p>
        </div>

        <div class="card-datos">
            <div class="fila">
                <div class="col-cliente">
                    <label>Cliente:</label>
                    <div class="input-group">
                        <select v-model="datosRemito.clienteNombre">
                            <option value="">Seleccionar...</option>
                            <option v-for="c in listaClientes" :key="c.id" :value="c.razonSocial">{{ c.razonSocial }}</option>
                        </select>
                        <button @click="crearClienteRapido" class="btn-small">➕</button>
                    </div>
                </div>
                <div><label>N° Remito:</label><input v-model="datosRemito.numero" type="text" placeholder="0001-0000..."></div>
                <div><label>Fecha:</label><input v-model="datosRemito.fecha" type="date"></div>
            </div>
        </div>

        <div class="card-items">
            <div class="fila-agregar">
                <select v-model="lineaTemp.productoId" class="select-prod">
                    <option value="">Producto...</option>
                    <option v-for="p in listaProductosTerminados" :key="p.id" :value="p.id">
                        {{ p.codigoSku }} - {{ p.nombre }} (Stock: {{ p.stockActual }})
                    </option>
                </select>
                <input v-model.number="lineaTemp.cantidad" type="number" class="input-cant" placeholder="Cant.">
                <button @click="agregarAlCarrito" class="btn-add">⬇️</button>
            </div>
        </div>

        <div class="tabla-container">
            <table class="tabla-remito">
                <thead><tr><th>SKU</th><th>Producto</th><th>Cant</th><th></th></tr></thead>
                <tbody>
                    <tr v-for="(item, index) in carrito" :key="index">
                        <td>{{ item.sku }}</td><td>{{ item.nombre }}</td><td class="numero">{{ item.cantidad }}</td>
                        <td><button @click="quitarDelCarrito(index)" class="btn-del">❌</button></td>
                    </tr>
                </tbody>
            </table>
        </div>

        <div class="acciones-finales">
            <button @click="procesarRemito" :disabled="cargando" class="btn-confirmar">✅ CONFIRMAR SALIDA</button>
        </div>
    </div>

    <div v-else class="vista-historial">
        <h2>📂 Historial de Remitos</h2>
        <table class="tabla-remito">
            <thead>
                <tr>
                    <th>Fecha</th>
                    <th>N° Remito</th>
                    <th>Cliente</th>
                    <th>Items</th>
                    <th>Acción</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="r in listaHistorial" :key="r.id">
                    <td>{{ r.fecha }}</td>
                    <td style="font-weight: bold;">{{ r.numeroRemito }}</td>
                    <td>{{ r.clienteNombre }}</td>
                    <td>{{ r.totalItems }} un.</td>
                    <td>
                        <button @click="prepararImpresion(r)" class="btn-print">🖨️ Imprimir</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="pdf-container">
        <div id="remito-pdf" class="hoja-a4" v-if="remitoParaImprimir">
            
            <div class="header-pdf">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAA..." alt="LOGO" class="logo-pdf">
                <div class="info-empresa">
                    <h1>ESTRUPLAST S.A.</h1>
                    <p>REMITO "R"</p>
                    <p>DOCUMENTO NO VÁLIDO COMO FACTURA</p>
                </div>
                <div class="info-remito">
                    <p><strong>N°:</strong> {{ remitoParaImprimir.numeroRemito }}</p>
                    <p><strong>Fecha:</strong> {{ remitoParaImprimir.fecha }}</p>
                </div>
            </div>

            <div class="datos-cliente-pdf">
                <p><strong>Señor(es):</strong> {{ remitoParaImprimir.clienteNombre }}</p>
            </div>

            <table class="tabla-pdf">
                <thead><tr><th>CÓDIGO</th><th>DESCRIPCIÓN</th><th>CANTIDAD</th></tr></thead>
                <tbody>
                    <tr v-for="(item, i) in remitoParaImprimir.items" :key="i">
                        <td>{{ item.sku }}</td>
                        <td>{{ item.producto }}</td>
                        <td style="text-align: right;">{{ item.cantidad }}</td>
                    </tr>
                </tbody>
            </table>

            <div class="pie-pdf">
                <div class="firma">Recibí Conforme (Firma y Aclaración)</div>
            </div>
        </div>
    </div>

  </div>
</template>

<style scoped>
/* Estilos generales */
.panel-remitos { max-width: 900px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; box-shadow: 0 4px 10px rgba(0,0,0,0.05); }

/* Tabs */
.tabs { display: flex; gap: 10px; margin-bottom: 20px; border-bottom: 2px solid #eee; }
.tabs button { background: none; border: none; padding: 10px 20px; font-size: 16px; cursor: pointer; color: #777; }
.tabs button.active { color: #e67e22; border-bottom: 3px solid #e67e22; font-weight: bold; }

/* Tablas */
.tabla-remito { width: 100%; border-collapse: collapse; margin-top: 10px; }
.tabla-remito th { background: #2c3e50; color: white; padding: 10px; text-align: left; }
.tabla-remito td { padding: 10px; border-bottom: 1px solid #eee; }

/* Botones */
.btn-print { background: #3498db; color: white; border: none; padding: 5px 10px; border-radius: 4px; cursor: pointer; }
.btn-confirmar { background: #e67e22; color: white; border: none; padding: 10px 30px; font-size: 1.1em; font-weight: bold; border-radius: 6px; cursor: pointer; width: 100%; margin-top: 20px; }

/* PDF Oculto (Fuera de pantalla) */
.pdf-container { position: absolute; left: -9999px; top: 0; }
.hoja-a4 { width: 210mm; min-height: 297mm; background: white; padding: 20mm; font-family: Arial, sans-serif; color: black; border: 1px solid #000; }
.header-pdf { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid black; padding-bottom: 10px; margin-bottom: 20px; }
.logo-pdf { max-height: 50px; }
.info-empresa h1 { margin: 0; font-size: 24px; }
.info-remito { border: 1px solid black; padding: 10px; }
.tabla-pdf { width: 100%; border-collapse: collapse; margin-top: 20px; }
.tabla-pdf th, .tabla-pdf td { border: 1px solid black; padding: 8px; }
.pie-pdf { margin-top: 100px; display: flex; justify-content: flex-end; }
.firma { border-top: 1px solid black; width: 200px; text-align: center; padding-top: 5px; }

/* Reutiliza los estilos de inputs del formulario anterior... */
.card-datos, .card-items { background: #f8f9fa; padding: 15px; border-radius: 6px; margin-bottom: 20px; }
.fila { display: flex; gap: 20px; }
.col-cliente { flex: 2; }
.input-group { display: flex; gap: 5px; }
input, select { width: 100%; padding: 8px; border: 1px solid #ccc; border-radius: 4px; }
.fila-agregar { display: flex; gap: 10px; }
.select-prod { flex-grow: 1; }
.input-cant { width: 100px; }
.btn-add { background: #27ae60; color: white; border: none; padding: 0 15px; border-radius: 4px; cursor: pointer; font-weight: bold; }
.btn-del { background: transparent; border: none; cursor: pointer; }
</style>