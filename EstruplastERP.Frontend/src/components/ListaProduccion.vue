<script setup lang="ts">
import { ref, onMounted} from 'vue'

// 1. Definimos la "forma" de tus datos para que TypeScript no se queje
interface ProduccionItem {
  id: number;
  fecha: string;
  turno: string;
  operario: string; // Tu API devuelve 'operario'
  producto: string; // Tu API devuelve 'producto'
  lote: string;
  cantidad: number;
  kilos: number;
}

// 2. Aplicamos el tipo a la referencia
const producciones = ref<ProduccionItem[]>([])
const cargando = ref(false)

// Fechas por defecto
const fechaHoy = new Date()
const primerDia = new Date(fechaHoy.getFullYear(), fechaHoy.getMonth(), 1)

const filtros = ref({
    desde: primerDia.toISOString().split('T')[0],
    hasta: fechaHoy.toISOString().split('T')[0]
})

const apiUrl = 'https://localhost:7244/api' 

// Carga por rango
async function cargarHistorial() {
  cargando.value = true
  try {
    const url = `${apiUrl}/Produccion/rango?desde=${filtros.value.desde}&hasta=${filtros.value.hasta}`
    const respuesta = await fetch(url)
    
    if (respuesta.ok) {
        producciones.value = await respuesta.json()
    }
  } catch (e) {
    console.error("Error conectando al servidor")
  } finally {
    cargando.value = false
  }
}

const imprimirEtiqueta = (item: ProduccionItem) => {
    const ventana = window.open('', 'PRINT', 'height=600,width=800'); // Un poco más grande

    if (ventana) {
        ventana.document.write(`
            <html>
            <head>
                <title>Lote ${item.lote}</title>
                <style>
                    body { font-family: 'Arial', sans-serif; text-align: center; border: 4px solid #000; padding: 15px; margin: 10px; max-width: 600px; }
                    .header { border-bottom: 2px solid #000; padding-bottom: 5px; margin-bottom: 10px; }
                    h1 { margin: 0; font-size: 24px; text-transform: uppercase; }
                    .main-data { display: flex; justify-content: space-between; border: 2px solid #000; padding: 10px; margin: 15px 0; background: #f0f0f0; }
                    .big-qty { font-size: 40px; font-weight: 900; display: block;}
                    .label-text { font-size: 14px; text-transform: uppercase; color: #555; }
                    
                    /* 🚨 ESTO ES LO NUEVO: LA TABLA DE MOVIMIENTOS */
                    .movimientos { width: 100%; border-collapse: collapse; margin-top: 20px; font-size: 12px; }
                    .movimientos th { background: #333; color: white; padding: 5px; text-align: left; }
                    .movimientos td { border: 1px solid #999; height: 25px; } /* Altura para escribir a mano */
                </style>
            </head>
            <body>
                <div class="header">
                    <h1>ESTRUPLAST S.A.</h1>
                    <small>Etiqueta de Control de Lote</small>
                </div>

                <div style="text-align: left;">
                    <span class="label-text">Producto:</span><br>
                    <strong style="font-size: 22px;">${item.producto}</strong>
                </div>

                <div class="main-data">
                    <div>
                        <span class="label-text">Cantidad Inicial</span>
                        <span class="big-qty">${item.cantidad}</span>
                    </div>
                    <div>
                        <span class="label-text">Lote ID</span>
                        <span class="big-qty">${item.lote}</span>
                    </div>
                </div>

                <div style="text-align: left; margin-bottom: 15px;">
                    <strong>Fecha:</strong> ${new Date(item.fecha).toLocaleDateString()} &nbsp;|&nbsp; 
                    <strong>Operario:</strong> ${item.operario}
                </div>

                <div style="text-align: left; border-top: 2px dashed #000; padding-top: 10px;">
                    <strong>📋 CONTROL DE SALIDAS PARCIALES (Escribir al retirar)</strong>
                    <table class="movimientos">
                        <thead>
                            <tr>
                                <th width="30%">Fecha</th>
                                <th width="20%">Retira</th>
                                <th width="20%">Quedan</th>
                                <th width="30%">Firma</th>
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
            </body>
            </html>
        `);
        
        ventana.document.close();
        ventana.focus();
        setTimeout(() => { ventana.print(); ventana.close(); }, 500);
    }
};

defineExpose({ cargarHistorial })

onMounted(() => {
    cargarHistorial()
})
</script>

<template>
  <div class="historial-container">
    <div class="cabecera-historial">
        <h3>📋 Historial de Producción</h3>
        
        <div class="filtros">
            <label>Desde:</label>
            <input type="date" v-model="filtros.desde">
            
            <label>Hasta:</label>
            <input type="date" v-model="filtros.hasta">
            
            <button @click="cargarHistorial" class="btn-buscar">🔍 Buscar</button>
        </div>
    </div>

    <div class="tabla-scroll">
        <table class="tabla-produccion">
          <thead>
            <tr>
              <th>Fecha/Hora</th>
              <th>Turno</th>
              <th>Operario</th>
              <th>Producto</th>
              <th>Lote</th>
              <th>Cant.</th>
              <th>Kilos</th>
              <th>Acción</th> </tr>
          </thead>
          <tbody>
            <tr v-for="p in producciones" :key="p.id">
              <td>{{ new Date(p.fecha).toLocaleDateString() }} <small>{{ new Date(p.fecha).toLocaleTimeString([], {hour: '2-digit', minute:'2-digit'}) }}</small></td>
              <td><span class="badge-turno">{{ p.turno }}</span></td>
              <td>{{ p.operario }}</td>
              <td class="texto-prod">{{ p.producto }}</td>
              <td class="mono">{{ p.lote }}</td>
              <td class="numero">{{ p.cantidad }}</td>
              <td class="numero">{{ p.kilos }} kg</td>
              <td style="text-align: center;">
                 <button @click="imprimirEtiqueta(p)" class="btn-print" title="Imprimir Etiqueta">🖨️</button>
              </td>
            </tr>
            <tr v-if="producciones.length === 0 && !cargando">
                <td colspan="8" class="vacio">No hay registros en estas fechas.</td>
            </tr>
          </tbody>
        </table>
    </div>
    
    <div class="totales-pie" v-if="producciones.length > 0">
        <strong>Total Periodo:</strong> 
        {{ producciones.reduce((a, b) => a + b.cantidad, 0) }} Unid. / 
        {{ producciones.reduce((a, b) => a + b.kilos, 0).toFixed(2) }} Kg
    </div>
  </div>
</template>

<style scoped>
/* Tus estilos originales se mantienen igual, agrego solo el botón de print */
.historial-container { background: white; padding: 20px; border-radius: 10px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); }

.cabecera-historial { display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; margin-bottom: 15px; }
.cabecera-historial h3 { margin: 0; color: #2c3e50; border-left: 5px solid #e67e22; padding-left: 10px; }

.filtros { display: flex; gap: 10px; align-items: center; background: #f8f9fa; padding: 8px; border-radius: 6px; border: 1px solid #ddd; }
.filtros input { padding: 5px; border: 1px solid #ccc; border-radius: 4px; }
.btn-buscar { background: #3498db; color: white; border: none; padding: 6px 12px; border-radius: 4px; cursor: pointer; }
.btn-buscar:hover { background: #2980b9; }

.tabla-scroll { overflow-x: auto; }
.tabla-produccion { width: 100%; border-collapse: collapse; min-width: 600px; }
.tabla-produccion th { background-color: #2c3e50; color: white; padding: 10px; text-align: left; font-size: 0.9em; }
.tabla-produccion td { padding: 10px; border-bottom: 1px solid #eee; color: #555; }
.tabla-produccion tr:hover { background-color: #f1f1f1; }

.badge-turno { background: #eee; padding: 2px 6px; border-radius: 4px; font-size: 0.8em; font-weight: bold; color: #555; }
.texto-prod { font-weight: bold; color: #2c3e50; }
.mono { font-family: monospace; color: #666; }
.numero { font-weight: bold; text-align: right; }
.vacio { text-align: center; padding: 20px; color: #999; font-style: italic; }

.totales-pie { margin-top: 15px; text-align: right; font-size: 1.1em; border-top: 2px solid #2c3e50; padding-top: 10px; color: #2c3e50; }

/* Estilo botón imprimir */
.btn-print {
    background: white;
    border: 1px solid #ccc;
    cursor: pointer;
    font-size: 1.2em;
    padding: 2px 6px;
    border-radius: 4px;
    transition: transform 0.2s;
}
.btn-print:hover { transform: scale(1.1); background: #f0f8ff; border-color: #3498db; }
</style>