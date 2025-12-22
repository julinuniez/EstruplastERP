<script setup lang="ts">
import { ref, onMounted } from 'vue'
// 👇 1. IMPORTAMOS EL MODAL (Asegúrate de tener el archivo ModalCierreOrden.vue creado)
import ModalCierreOrden from './ModalCierreOrden.vue'

// 1. Interfaces
interface ProduccionItem {
  id: number;
  fecha: string;
  turno: string;
  operario: string;
  producto: string;
  lote: string;
  cantidad: number;
  estado?: number; 
  kilos: number;
  // Agregamos esto para pasar datos extra al modal si hace falta
  nombreProducto?: string; 
  nombreEmpleado?: string;
}

// 2. Variables reactivas
const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)

// 👇 VARIABLES PARA EL MODAL DE CIERRE
const mostrarModalCierre = ref(false)
const ordenSeleccionada = ref<ProduccionItem | null>(null)
const listaMateriasPrimas = ref<any[]>([]) // Necesario para el desplegable del modal

const fechaHoy = new Date()
const primerDia = new Date(fechaHoy.getFullYear(), fechaHoy.getMonth(), 1)

const filtros = ref({
    desde: primerDia.toISOString().split('T')[0],
    hasta: fechaHoy.toISOString().split('T')[0]
})

const apiUrl = import.meta.env.VITE_API_URL || 'https://localhost:7244/api';

const obtenerTextoEstado = (estado: number | undefined) => estado === 2 ? 'FINALIZADO' : 'PENDIENTE';
const obtenerClaseEstado = (estado: number | undefined) => estado === 2 ? 'badge-finalizado' : 'badge-pendiente';

// --- CARGA DE DATOS ---
async function cargarHistorial() {
  cargando.value = true
  try {
    const url = `${apiUrl}/Ordenes/rango?desde=${filtros.value.desde}&hasta=${filtros.value.hasta}`
    const respuesta = await fetch(url)
    if (respuesta.ok) producciones.value = await respuesta.json()
  } catch (e) {
    console.error("Error conectando al servidor")
  } finally {
    cargando.value = false
  }
}

// 👇 CARGAR MATERIAS PRIMAS (Se ejecuta una vez al inicio)
async function cargarMateriasPrimas() {
    try {
        const res = await fetch(`${apiUrl}/Productos/materias-primas`);
        if (res.ok) listaMateriasPrimas.value = await res.json();
    } catch (e) { console.error("Error cargando MPs", e); }
}

// --- NUEVA LÓGICA DE FINALIZAR ---

// Paso 1: Al hacer clic en el Check ✅
const abrirConfirmacion = (item: ProduccionItem) => {
    // Preparamos los datos para el modal
    ordenSeleccionada.value = {
        ...item,
        // Aseguramos que los nombres coincidan con lo que espera el Modal
        nombreProducto: item.producto,
        nombreEmpleado: item.operario
    };
    mostrarModalCierre.value = true;
}

// Paso 2: Cuando el Modal nos avisa que guardó exitosamente
const onOrdenFinalizadaExito = () => {
    // ⚡ TRUCO VISUAL: Actualizamos la lista localmente
    if (ordenSeleccionada.value) {
        const item = producciones.value.find(p => p.id === ordenSeleccionada.value?.id);
        if (item) {
            item.estado = 2; // Cambia a verde al instante
        }
    }
    mostrarModalCierre.value = false;
    ordenSeleccionada.value = null;
}

const imprimirEtiqueta = (item: ProduccionItem) => {
    // Abrimos una ventana nueva en blanco
    const ventana = window.open('', 'PRINT', 'height=800,width=600');

    if (ventana) {
        // Escribimos el HTML completo de la etiqueta
        ventana.document.write(`
            <html>
            <head>
                <title>Etiqueta Lote ${item.lote}</title>
                <style>
                    body { 
                        font-family: 'Arial', sans-serif; 
                        padding: 20px; 
                        max-width: 800px; 
                        margin: 0 auto; 
                        border: 2px solid #000; /* Borde exterior para recortar */
                    }
                    .header { 
                        text-align: center; 
                        border-bottom: 3px solid #000; 
                        padding-bottom: 10px; 
                        margin-bottom: 20px; 
                    }
                    h1 { margin: 0; font-size: 28px; text-transform: uppercase; }
                    .sub-header { font-size: 14px; color: #555; }
                    
                    /* DATOS PRINCIPALES (Producto y Lote) */
                    .main-info { 
                        display: flex; 
                        justify-content: space-between; 
                        margin-bottom: 20px; 
                        border: 1px solid #000;
                        padding: 10px;
                        background-color: #f9f9f9;
                    }
                    .info-box { flex: 1; text-align: center; }
                    .label { font-size: 12px; text-transform: uppercase; color: #666; display: block; }
                    .value { font-size: 24px; font-weight: bold; display: block; margin-top: 5px;}
                    .producto-titulo { font-size: 32px; font-weight: 900; margin: 15px 0; text-align: center; text-decoration: underline;}

                    /* TABLA DE HISTORIAL MANUAL */
                    .historial-section { margin-top: 30px; }
                    .titulo-tabla { font-weight: bold; background: #000; color: white; padding: 5px 10px; display: inline-block; margin-bottom: 5px; }
                    
                    table { width: 100%; border-collapse: collapse; margin-top: 5px; }
                    th { border: 1px solid #000; background: #ddd; padding: 8px; font-size: 12px; text-align: center; }
                    td { border: 1px solid #000; height: 35px; } /* Altura para escribir cómodo */
                    
                    .footer { margin-top: 20px; font-size: 10px; text-align: right; border-top: 1px dashed #999; padding-top: 5px; }

                    /* Ocultar elementos de navegador al imprimir */
                    @media print {
                        @page { margin: 0; }
                        body { border: none; }
                    }
                </style>
            </head>
            <body>
                
                <div class="header">
                    <h1>ESTRUPLAST S.A.</h1>
                    <div class="sub-header">CONTROL DE STOCK Y TRAZABILIDAD</div>
                </div>

                <div class="producto-titulo">${item.producto}</div>

                <div class="main-info">
                    <div class="info-box" style="border-right: 1px solid #ccc;">
                        <span class="label">Nro de Lote</span>
                        <span class="value">${item.lote}</span>
                    </div>
                    <div class="info-box" style="border-right: 1px solid #ccc;">
                        <span class="label">Cantidad Inicial</span>
                        <span class="value">${item.cantidad} Unid. / ${item.kilos} Kg</span>
                    </div>
                    <div class="info-box">
                        <span class="label">Fecha Prod.</span>
                        <span class="value">${new Date(item.fecha).toLocaleDateString()}</span>
                    </div>
                </div>

                <div style="margin-bottom: 20px;">
                    <strong>Operario Responsable:</strong> ${item.operario} | <strong>Turno:</strong> ${item.turno}
                </div>

                <div class="historial-section">
                    <div class="titulo-tabla">📝 REGISTRO DE MOVIMIENTOS / SALIDAS PARCIALES</div>
                    <p style="font-size: 12px; margin: 0 0 5px 0;">* Anotar cada vez que se retire material de este lote/pallet.</p>
                    
                    <table>
                        <thead>
                            <tr>
                                <th style="width: 20%">FECHA</th>
                                <th style="width: 20%">RETIRA (Cant)</th>
                                <th style="width: 20%">QUEDAN (Stock)</th>
                                <th style="width: 40%">FIRMA / RESPONSABLE</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                            <tr><td></td><td></td><td></td><td></td></tr>
                        </tbody>
                    </table>
                </div>

                <div class="footer">
                    Documento generado el ${new Date().toLocaleString()} - Sistema Estruplast
                </div>

            </body>
            </html>
        `);
        
        ventana.document.close(); // Necesario para terminar la carga
        ventana.focus(); // Enfocar la ventana
        
        // Esperamos un poquito para que carguen los estilos y lanzamos imprimir
        setTimeout(() => {
            ventana.print();
            ventana.close();
        }, 500);
    }
};

onMounted(() => {
    cargarHistorial();
    cargarMateriasPrimas(); // 👇 Cargamos esto al iniciar
})

defineExpose({
  cargarHistorial
})
</script>

<template>
  <div class="historial-container">
    <div class="cabecera-historial">
        <h3>📋 Historial de Producción</h3>
        <div class="filtros">
            <label>Desde:</label><input type="date" v-model="filtros.desde">
            <label>Hasta:</label><input type="date" v-model="filtros.hasta">
            <button @click="cargarHistorial" class="btn-buscar">🔍 Buscar</button>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla-produccion">
          <thead>
            <tr>
              <th>Fecha/Hora</th>
              <th>Estado</th> <th>Turno</th>
              <th>Operario</th>
              <th>Producto</th>
              <th>Lote</th>
              <th>Cant.</th>
              <th>Kilos</th>
              <th>Acción</th> 
            </tr>
          </thead>
          <tbody>
            <tr v-for="p in producciones" :key="p.id">
              <td>
                  {{ new Date(p.fecha).toLocaleDateString() }} 
                  <small>{{ new Date(p.fecha).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</small>
              </td>
              <td><span :class="obtenerClaseEstado(p.estado)">{{ obtenerTextoEstado(p.estado) }}</span></td>
              <td><span class="badge-turno">{{ p.turno }}</span></td>
              <td>{{ p.operario }}</td>
              <td class="texto-prod">{{ p.producto }}</td>
              <td class="mono">{{ p.lote }}</td>
              <td class="numero">{{ p.cantidad }}</td>
              <td class="numero">{{ p.kilos }} kg</td>
              
              <td class="celda-acciones">
                <button 
                    @click="p.estado !== 2 ? abrirConfirmacion(p) : null" 
                    class="btn-check"
                    :class="{ 'invisible': p.estado === 2 }" 
                    title="Finalizar Producción">
                    ✅
                </button>

                <button @click="imprimirEtiqueta(p)" class="btn-print" title="Imprimir Etiqueta">🖨️</button>
              </td>
            </tr>
            <tr v-if="producciones.length === 0 && !cargando">
                <td colspan="9" class="vacio">No hay registros en estas fechas.</td>
            </tr>
          </tbody>
        </table>
    </div>
    
    <ModalCierreOrden 
        v-if="mostrarModalCierre && ordenSeleccionada"
        :visible="mostrarModalCierre"
        :orden="ordenSeleccionada"
        :materiasPrimas="listaMateriasPrimas"
        @close="mostrarModalCierre = false"
        @confirmar="onOrdenFinalizadaExito"
    />

  </div>
</template>

<style scoped>
/* ... (TUS MISMOS ESTILOS QUE YA TIENES) ... */
.historial-container { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }
.cabecera-historial { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; margin-bottom: 15px; }
.cabecera-historial h3 { margin: 0; color: #2c3e50; border-left: 5px solid #e67e22; padding-left: 10px; }
.filtros { display: flex; gap: 10px; align-items: center; background: #f8f9fa; padding: 8px; border-radius: 6px; border: 1px solid #ddd; }
.filtros input { padding: 5px; border: 1px solid #ccc; border-radius: 4px; }
.btn-buscar { background: #3498db; color: white; border: none; padding: 6px 12px; border-radius: 4px; cursor: pointer; }
.btn-buscar:hover { background: #2980b9; }
.tabla-scroll { overflow-x: auto; }
.tabla-produccion { width: 100%; border-collapse: collapse; min-width: 800px; } 
.tabla-produccion th { background-color: #2c3e50; color: white; padding: 10px; text-align: left; font-size: 0.9em; }
.tabla-produccion td { padding: 10px; border-bottom: 1px solid #eee; color: #555; }
.tabla-produccion tr:hover { background-color: #f1f1f1; }
.badge-turno { background: #eee; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; color: #555; }
.texto-prod { font-weight: bold; color: #2c3e50; }
.mono { font-family: monospace; color: #666; }
.numero { font-weight: bold; text-align: right; }
.vacio { text-align: center; padding: 20px; color: #999; font-style: italic; }
.totales-pie { margin-top: 15px; text-align: right; font-size: 1.1em; border-top: 2px solid #2c3e50; padding-top: 10px; color: #2c3e50; }
.btn-print { background: white; border: 1px solid #ccc; cursor: pointer; font-size: 1.2em; padding: 2px 6px; border-radius: 4px; transition: transform 0.2s; }
.btn-print:hover { transform: scale(1.1); background: #f0f8ff; border-color: #3498db; }
.btn-check { background: white; border: 1px solid #ccc; border-radius: 4px; cursor: pointer; font-size: 1.2em; padding: 2px 6px; margin-right: 5px; color: green; transition: transform 0.2s; }
.btn-check:hover { transform: scale(1.1); background: #e8f5e9; border-color: green; }
.badge-pendiente { background-color: #fff3cd; color: #856404; padding: 4px 8px; border-radius: 20px; font-size: 0.8em; font-weight: bold; border: 1px solid #ffeeba; }
.badge-finalizado { background-color: #d4edda; color: #155724; padding: 4px 8px; border-radius: 20px; font-size: 0.8em; font-weight: bold; border: 1px solid #c3e6cb; }
.celda-acciones { text-align: center; white-space: nowrap; }
.btn-check { margin-right: 10px; }
.invisible { visibility: hidden; pointer-events: none; cursor: default; }
</style>