<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'
import axios from 'axios'
// @ts-ignore
import html2pdf from 'html2pdf.js'

// --- INTERFACES ---
interface Cliente {
  id: number;
  razonSocial: string;
  cuit?: string;
  direccion?: string;
}

interface RemitoItem {
    id: number;
    productoNombre: string;
    cantidad: number;
    detalle?: string;
}

interface Remito {
  id: number;
  fecha: string;
  clienteId: number;
  cliente?: Cliente;
  // Propiedad de respaldo por si el cliente es null (borrado)
  clienteNombreBackup?: string; 
  observacion?: string;
  items?: RemitoItem[]; 
}

// --- ESTADO ---
const remitos = ref<Remito[]>([])
const cargando = ref(false)
const error = ref('')
const remitoParaImprimir = ref<Remito | null>(null)

const apiUrl = 'https://localhost:7244/api'; 

const getAuthConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// --- CARGAR DATOS ---
async function cargarHistorial() {
    cargando.value = true;
    try {
        const res = await axios.get(`${apiUrl}/Remitos`, getAuthConfig());
        remitos.value = res.data.map((r: any) => ({
            ...r,
            items: r.remitoDetalles || r.items 
        }));
    } catch (e: any) {
        console.error(e);
        error.value = 'Error al cargar el historial.';
    } finally {
        cargando.value = false;
    }
}

// --- GENERAR PDF ---
async function descargarPDF(remito: Remito) {
    remitoParaImprimir.value = remito;
    await nextTick();

    const element = document.getElementById('remito-imprimible');

    const opt = {
        // CAMBIO CLAVE: Margen en 0. Controlamos el espacio con CSS (padding).
        margin:       0, 
        filename:     `Remito-${remito.id}-${remito.cliente?.razonSocial || 'Cliente'}.pdf`,
        image:        { type: 'jpeg', quality: 0.98 },
        html2canvas:  { 
            scale: 2, 
            useCORS: true, 
            scrollY: 0 
        },
        jsPDF:        { unit: 'mm', format: 'a4', orientation: 'portrait' }
    };

    html2pdf().set(opt).from(element).save();
}

onMounted(() => {
    cargarHistorial();
});
</script>

<template>
  <div class="contenedor-historial">
    
    <div class="header-seccion">
        <h2>📄 Historial de Remitos</h2>
        <div class="botones">
            <button class="btn-recargar" @click="cargarHistorial">🔄 Recargar</button>
            <router-link :to="{ name: 'DespachoRemitos' }" class="btn-nuevo">
                ➕ Nuevo Remito
            </router-link>
        </div>
    </div>

    <div v-if="cargando" class="loading">Cargando datos...</div>
    <div v-if="error" class="error-msg">{{ error }}</div>

    <div class="tabla-container" v-if="!cargando && remitos.length > 0">
        <table>
            <thead>
                <tr>
                    <th>Nro #</th>
                    <th>Fecha</th> 
                    <th>Cliente</th>
                    <th>Productos / Detalle</th> 
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="r in remitos" :key="r.id">
                    <td><strong>{{ r.id.toString().padStart(4, '0') }}</strong></td>
                    <td>{{ new Date(r.fecha).toLocaleDateString() }}</td>
                    
                    <td class="cliente-cell">
                        {{ r.cliente?.razonSocial || r.clienteNombreBackup || 'Desconocido' }}
                    </td>
                    
                    <td class="celda-productos">
                        <div v-for="item in r.items" :key="item.id" class="item-fila">
                            • {{ item.productoNombre }} 
                            <span v-if="item.detalle" class="tag-detalle">({{ item.detalle }})</span>
                            <strong>x {{ item.cantidad }}kg</strong>
                        </div>
                    </td>
                    <td>
                        <button class="btn-ver" @click="descargarPDF(r)">⬇️ PDF</button>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="contenedor-impresion">
        <div id="remito-imprimible" v-if="remitoParaImprimir">
            
            <div class="pdf-header">
                <div class="empresa-info">
                    <h1>ESTRUPLAST</h1>
                    <p>Fábrica de Plásticos</p>
                    <p>Dirección de la fábrica, Buenos Aires</p>
                    <p>Tel: (011) 1234-5678</p>
                </div>
                <div class="remito-info">
                    <h2>REMITO</h2>
                    <div class="box-numero">N° {{ remitoParaImprimir.id.toString().padStart(8, '0') }}</div>
                    <p><strong>Fecha:</strong> {{ new Date(remitoParaImprimir.fecha).toLocaleDateString() }}</p>
                </div>
            </div>

            <hr class="pdf-divider">

            <div class="pdf-cliente">
                <p><strong>Señor(es):</strong> {{ remitoParaImprimir.cliente?.razonSocial || remitoParaImprimir.clienteNombreBackup }}</p>
                <p><strong>Dirección:</strong> {{ remitoParaImprimir.cliente?.direccion || '-' }}</p>
                <p><strong>CUIT:</strong> {{ remitoParaImprimir.cliente?.cuit || '-' }}</p>
            </div>

            <table class="pdf-tabla">
                <thead>
                    <tr>
                        <th style="width: 15%">CANT (Kg)</th>
                        <th style="width: 65%">DESCRIPCIÓN</th>
                        <th style="width: 20%">OBS</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="item in remitoParaImprimir.items" :key="item.id">
                        <td class="center">{{ item.cantidad }}</td>
                        <td>
                            {{ item.productoNombre }}
                            <span v-if="item.detalle"> - {{ item.detalle }}</span>
                        </td>
                        <td>-</td>
                    </tr>
                    <tr v-for="n in (10 - (remitoParaImprimir.items?.length || 0))" :key="n" class="fila-vacia">
                        <td>&nbsp;</td><td></td><td></td>
                    </tr>
                </tbody>
            </table>

            <div class="pdf-footer">
                <div class="observaciones">
                    <strong>Observaciones:</strong> {{ remitoParaImprimir.observacion || '-' }}
                </div>
                
                <div class="firmas">
                    <div class="firma-box">
                        <div class="linea"></div>
                        <p>Recibí Conforme</p>
                    </div>
                    <div class="firma-box">
                        <div class="linea"></div>
                        <p>Firma y Aclaración</p>
                    </div>
                </div>
                
                <p class="nota-legal">Documento no válido como factura.</p>
            </div>
        </div>
    </div>

  </div>
</template>

<style scoped>
/* =================================================================
   ESTILOS DE PANTALLA
   ================================================================= */
.contenedor-historial { padding: 25px; background: #f8f9fa; border-radius: 8px; min-height: 500px; font-family: 'Segoe UI', sans-serif; }
.header-seccion { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; border-bottom: 2px solid #e9ecef; padding-bottom: 15px; }
h2 { margin: 0; color: #2c3e50; font-size: 1.5rem; }
.botones { display: flex; gap: 12px; }

/* Botones */
.btn-recargar { background: #95a5a6; color: white; border: none; padding: 10px 18px; border-radius: 6px; cursor: pointer; font-weight: 600; }
.btn-nuevo { background: #27ae60; color: white; text-decoration: none; padding: 10px 18px; border-radius: 6px; font-weight: 600; font-size: 14px; display: inline-block; border: none; cursor: pointer; }
.btn-nuevo:hover { background: #219150; }

/* Tabla */
.tabla-container { overflow-x: auto; background: white; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.05); border: 1px solid #e1e1e1; }
table { width: 100%; border-collapse: collapse; font-size: 0.95rem; }
th { background: #34495e; color: white; padding: 15px; text-align: left; text-transform: uppercase; font-size: 0.85rem; }
td { padding: 15px; border-bottom: 1px solid #f1f1f1; color: #333; vertical-align: top; }
tr:hover { background-color: #f8f9fa; }

.cliente-cell { font-weight: 700; color: #2980b9; text-transform: uppercase; }
.celda-productos { font-size: 0.9em; line-height: 1.6; }
.tag-detalle { background-color: #eef2f3; color: #555; padding: 2px 6px; border-radius: 4px; font-style: italic; font-size: 0.85em; margin: 0 5px; }
.btn-ver { background: #e74c3c; color: white; border: none; padding: 8px 14px; border-radius: 4px; cursor: pointer; font-weight: 600; font-size: 0.85rem; }

.loading { text-align: center; padding: 40px; color: #7f8c8d; font-size: 1.1rem; }
.error-msg { color: #721c24; background: #f8d7da; padding: 15px; border-radius: 6px; margin-bottom: 20px; }


/* =================================================================
   ESTILOS DEL PDF (A4 OCULTO) - SOLUCIÓN ANCHO Y DOBLE HOJA
   ================================================================= */
.contenedor-impresion {
    position: fixed;
    left: -9999px;
    top: 0;
    z-index: -1;
}

#remito-imprimible {
    width: 209mm; 
    min-height: 290mm;
    background: white;
    padding: 15mm; 
    
    font-family: 'Helvetica', 'Arial', sans-serif;
    color: #000;
    
    /* 4. CLAVE: Box-sizing para que el padding no sume al width */
    box-sizing: border-box; 
    
    border: none;
    position: relative;
    overflow: hidden;
}

.pdf-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; }
.empresa-info h1 { margin: 0 0 5px 0; font-size: 26px; font-weight: 900; letter-spacing: 1px; text-transform: uppercase; }
.empresa-info p { margin: 2px 0; font-size: 11px; color: #333; }
.remito-info { text-align: right; min-width: 150px; }
.remito-info h2 { margin: 0 0 10px 0; font-size: 22px; background: #f1f1f1; padding: 6px 15px; display: inline-block; border-radius: 4px; border: 1px solid #ddd; }
.box-numero { font-size: 16px; font-weight: bold; margin-bottom: 5px; }

.pdf-divider { border: 0; border-top: 2px solid #000; margin: 10px 0 20px 0; }

.pdf-cliente { border: 1px solid #000; padding: 12px; border-radius: 4px; margin-bottom: 25px; background: #fff; }
.pdf-cliente p { margin: 6px 0; font-size: 13px; line-height: 1.4; }

.pdf-tabla { width: 100%; border-collapse: collapse; margin-bottom: 40px; }
.pdf-tabla th { background: #000; color: white; padding: 8px; font-size: 11px; text-transform: uppercase; border: 1px solid #000; text-align: center; }
.pdf-tabla td { border: 1px solid #000; padding: 10px 8px; font-size: 12px; vertical-align: middle; }
.pdf-tabla .center { text-align: center; }
.fila-vacia td { height: 25px; border-bottom: 1px solid #000; }

.pdf-footer { 
    position: absolute;
    bottom: 15mm; /* Mismo valor que tu padding general */
    left: 15mm;
    right: 15mm;
    margin-top: 0; 
}
.observaciones { border: 1px solid #000; padding: 10px; min-height: 50px; margin-bottom: 60px; font-size: 11px; }
.firmas { display: flex; justify-content: space-between; margin-top: 50px; padding: 0 20px; }
.firma-box { text-align: center; width: 40%; }
.linea { border-top: 1px solid #000; margin-bottom: 8px; }
.firma-box p { font-size: 11px; color: #000; margin: 0; font-weight: bold; }
.nota-legal { text-align: center; font-size: 9px; color: #555; margin-top: 40px; border-top: 1px solid #ccc; padding-top: 5px; }
</style>