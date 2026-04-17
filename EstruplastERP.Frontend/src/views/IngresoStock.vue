<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue'
import api from '@/services/axiosInstance'; 

// --- 1. INTERFACES ---
interface Insumo {
    id: number;
    nombre: string;
    stockActual: number;
    esScrap?: boolean;
    esMateriaPrima?: boolean;
}

interface Proveedor {
    id: number;
    razonSocial: string;
}

interface Movimiento {
    id: number;
    fecha: string;
    producto: string;
    cantidad: number;
    proveedor: string; 
    remito: string;
    tipoMovimiento: string;
}

// --- ESTADO ---
const listaInsumos = ref<Insumo[]>([])
const listaProveedores = ref<Proveedor[]>([]) 
const listaMovimientosBruta = ref<Movimiento[]>([]) 
const cargando = ref(false);

const filtroMes = ref('') // Formato: 'YYYY-MM'
const filtroBusqueda = ref('')

// Variables para Paginación
const paginaActual = ref(1);
const registrosPorPagina = 30;

const form = ref({
  productoId: '' as number | '', 
  proveedorId: '' as number | '', 
  cantidad: 0,
  numeroRemito: ''
})

const mensaje = ref('')
const error = ref('')

const formatearFecha = (fechaOriginal: string | null | undefined) => {
    if (!fechaOriginal) return '-';
    try {
        const d = new Date(fechaOriginal);
        if (isNaN(d.getTime())) return 'Fecha Inválida'; 
        return d.toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
    } catch {
        return '-';
    }
};

// Reiniciar a la página 1 cuando se cambian los filtros
watch([filtroBusqueda, filtroMes], () => {
    paginaActual.value = 1;
});

const movimientosFiltrados = computed(() => {
    let lista = listaMovimientosBruta.value || [];

    // 1. Filtrar solo Entradas/Compras REALES (Excluyendo Ajustes Manuales)
    lista = lista.filter(m => {
        const tipo = (m.tipoMovimiento || '').toUpperCase();
        
        // Verifica si es un movimiento de entrada
        const esIngreso = tipo.includes('COMPRA') || 
                          tipo.includes('ENTRADA') || 
                          tipo.includes('INGRESO') || 
                          tipo.includes('RECEPCIÓN');
                          
        // Verifica si es un ajuste manual
        const esAjuste = tipo.includes('AJUSTE');

        // Solo lo dejamos pasar si es ingreso Y NO es un ajuste
        return esIngreso && !esAjuste;
    });

    // 2.  TRABA: Ocultar todo lo que sea MOLIDO
    lista = lista.filter(m => {
        const nombreProducto = (m.producto || '').toUpperCase();
        return !nombreProducto.includes('MOLIDO'); 
    });

    // 3. Filtro de Búsqueda de texto
    if (filtroBusqueda.value) {
        const busq = filtroBusqueda.value.toUpperCase();
        lista = lista.filter(m => 
            m.producto.toUpperCase().includes(busq) || 
            (m.proveedor && m.proveedor.toUpperCase().includes(busq)) ||
            (m.remito && m.remito.toUpperCase().includes(busq))
        );
    }

    // 4. Filtro Mensual ('YYYY-MM')
    if (filtroMes.value) {
        lista = lista.filter(m => {
            if (!m.fecha) return false;
            return m.fecha.startsWith(filtroMes.value); 
        });
    }

    return lista.sort((a, b) => {
        const timeA = a.fecha ? new Date(a.fecha).getTime() : 0;
        const timeB = b.fecha ? new Date(b.fecha).getTime() : 0;
        return timeB - timeA;
    });
});

// Lógica de Paginación
const totalPaginas = computed(() => Math.ceil(movimientosFiltrados.value.length / registrosPorPagina) || 1);

const movimientosPaginados = computed(() => {
    const inicio = (paginaActual.value - 1) * registrosPorPagina;
    const fin = inicio + registrosPorPagina;
    return movimientosFiltrados.value.slice(inicio, fin);
});

const irAPagina = (pag: number) => {
    if (pag >= 1 && pag <= totalPaginas.value) {
        paginaActual.value = pag;
    }
};

const totalKilosPeriodo = computed(() => {
    return movimientosFiltrados.value.reduce((acc, m) => acc + m.cantidad, 0);
});

onMounted(async () => {
    // Seteamos el filtro por defecto al mes actual
    const hoy = new Date();
    filtroMes.value = `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}`;
    
    await cargarTodo();
})

async function cargarTodo() {
    cargando.value = true;
    try {
        await Promise.all([cargarInsumos(), cargarProveedores(), cargarHistorialCompras()]);
    } finally {
        cargando.value = false;
    }
}

async function cargarInsumos() {
    try {
        const res = await api.get('/Productos');
        
        listaInsumos.value = res.data.filter((p: any) => {
            const nombre = p.nombre ? p.nombre.toUpperCase() : '';
            const esProductoGenerico = p.esGenerico === true || p.EsGenerico === true || p.esGenerico === 1 || p.EsGenerico === 1;
            return (
                p.esMateriaPrima === true && 
                p.esScrap !== true && 
                !p.esGenerico &&
                !nombre.includes("[SCRAP]") && 
                !nombre.includes("RECUPERADO") &&
                !nombre.includes("BASE") &&
                !nombre.includes("MOLIDO")   
            );
        }).sort((a: any, b: any) => {
            const nombreA = a.nombre || '';
            const nombreB = b.nombre || '';
            return nombreA.localeCompare(nombreB);
        });

    } catch (e) { 
        console.error("Error al cargar insumos:", e);
    }
}

async function cargarProveedores() {
    try {
        const res = await api.get('/Proveedores');
        listaProveedores.value = res.data.sort((a: any, b: any) => (a.razonSocial || '').localeCompare(b.razonSocial || ''));
    } catch (e) { console.error(e) }
}

async function cargarHistorialCompras() {
    try {
        const res = await api.get('/Movimientos');
        listaMovimientosBruta.value = res.data.map((m: any) => {
            let remitoLimpio = m.numeroRemito || '';
            if (!remitoLimpio && m.observacion && m.observacion.includes('Remito:')) {
                remitoLimpio = m.observacion.split('Remito:')[1]?.trim();
            }
            return {
                id: m.id,
                fecha: m.fecha || m.fechaMovimiento || new Date().toISOString(), 
                producto: m.producto || m.productoNombre || 'Material Desconocido',
                cantidad: m.cantidad || 0,
                proveedor: m.proveedor || m.proveedorNombre || null,
                remito: remitoLimpio || '-',
                tipoMovimiento: (m.tipoMovimiento || m.tipo || '').toUpperCase()
            };
        });
    } catch (e) { console.error(e) }
}

async function registrarCompra() {
    mensaje.value = ''; error.value = '';
    if (!form.value.proveedorId || !form.value.productoId || form.value.cantidad <= 0) {
        error.value = "Complete los campos obligatorios."; return;
    }
    try {
        await api.post('/Compras', {
            productoId: Number(form.value.productoId),
            proveedorId: Number(form.value.proveedorId),
            cantidad: form.value.cantidad,
            numeroRemito: form.value.numeroRemito,
            observacion: `Ingreso Compra MP - Remito: ${form.value.numeroRemito || 'S/N'}`
        });
        mensaje.value = `✅ Ingreso de ${form.value.cantidad} kg exitoso.`;
        form.value.cantidad = 0;
        form.value.numeroRemito = '';
        await cargarTodo();
    } catch (e: any) {
        error.value = "❌ Error: " + (e.response?.data?.mensaje || "Error al conectar");
    }
}
</script>

<template>
  <div class="contenedor-ingresos">
      
      <div class="header-dashboard">
          <div>
            <h2>Recepción de Materia Prima</h2>
          </div>
          <div class="stats-card">
              <span class="stats-label">Total en Pantalla</span>
              <span class="stats-value">{{ totalKilosPeriodo.toLocaleString() }} <small>kg</small></span>
          </div>
      </div>

      <div class="hoja-stock">
        <div class="fila-doble">
            <div class="campo mitad">
                <label>Proveedor:</label>
                <select v-model="form.proveedorId">
                    <option value="" disabled>Seleccionar</option>
                    <option v-for="prov in listaProveedores" :key="prov.id" :value="prov.id">{{ prov.razonSocial }}</option>
                </select>
            </div>
            <div class="campo mitad">
                <label>N° Remito:</label>
                <input type="text" v-model="form.numeroRemito" placeholder="Ej: R-0001-XXXX" />
            </div>
        </div>

        <div class="fila-doble">
            <div class="campo mitad">
                <label>Material:</label>
                <select v-model="form.productoId">
                    <option value="" disabled>Seleccione</option>
                    <option v-for="p in listaInsumos" :key="p.id" :value="p.id">{{ p.nombre }}</option>
                </select>
            </div>

            <div class="campo mitad">
                <label>Cantidad:</label>
                <div class="input-con-unidad">
                    <input type="number" v-model="form.cantidad" placeholder="0" min="0" step="0.1" />
                    <span class="unidad">kg</span>
                </div>
            </div>
        </div>

        <button class="btn-ingreso" @click="registrarCompra" :disabled="form.cantidad <= 0 || !form.productoId || !form.proveedorId || cargando">
            <span v-if="cargando">PROCESANDO...</span>
            <span v-else>📥 CONFIRMAR INGRESO A STOCK</span>
        </button>

        <p v-if="mensaje" class="exito">{{ mensaje }}</p>
        <p v-if="error" class="error">{{ error }}</p>
      </div>

      <div class="historial-rapido">
          <div class="header-historial" style="display: flex; gap: 15px; align-items: center; flex-wrap: wrap;">
              <input type="text" v-model="filtroBusqueda" placeholder="🔍 Buscar material, proveedor o remito..." class="input-search" style="flex: 1; min-width: 250px;">
              
              <div class="filtros-fecha" style="display: flex; align-items: center; gap: 10px; background: #f1f5f9; padding: 5px 15px; border-radius: 6px;">
                  <label style="font-weight: bold; color: #475569; font-size: 0.9rem;">Mes:</label>
                  <input type="month" v-model="filtroMes" style="border: 1px solid #cbd5e1; padding: 6px 10px; border-radius: 4px; font-weight: bold; color: #1e293b;">
                  <button v-if="filtroMes" @click="filtroMes = ''" title="Limpiar filtro" style="background: none; border: none; color: #e74c3c; cursor: pointer; font-size: 1.1rem;">✖</button>
              </div>
          </div>

          <div class="table-container">
              <table class="tabla-mini">
                  <thead>
                      <tr>
                          <th>Fecha</th>
                          <th>Proveedor</th>
                          <th>Material</th>
                          <th>Remito</th>
                          <th style="text-align:right">Cantidad</th>
                      </tr>
                  </thead>
                  <tbody>
                      <tr v-for="mov in movimientosPaginados" :key="mov.id">
                          <td>{{ formatearFecha(mov.fecha) }}</td> 
                          <td class="txt-prov">{{ mov.proveedor || '-' }}</td>
                          <td><strong>{{ mov.producto }}</strong></td>
                          <td><span class="txt-remito-solo">{{ mov.remito }}</span></td>
                          <td style="text-align:right; font-weight:900; color:#10b981;">+{{ mov.cantidad.toLocaleString() }} kg</td>
                      </tr>
                      <tr v-if="movimientosPaginados.length === 0">
                          <td colspan="5" class="vacio-msg">Sin registros en este periodo.</td>
                      </tr>
                  </tbody>
              </table>
          </div>

          <div class="paginacion" v-if="totalPaginas > 1">
              <button @click="irAPagina(paginaActual - 1)" :disabled="paginaActual === 1">Anterior</button>
              <span>Página {{ paginaActual }} de {{ totalPaginas }}</span>
              <button @click="irAPagina(paginaActual + 1)" :disabled="paginaActual === totalPaginas">Siguiente</button>
          </div>

      </div>
  </div>
</template>

<style scoped>
.contenedor-ingresos { max-width: 1000px; margin: 0 auto; font-family: 'Segoe UI', sans-serif; padding: 20px; background-color: #f9fafb; min-height: 100vh; }

.header-dashboard { display: flex; justify-content: space-between; align-items: center; margin-bottom: 25px; }
.header-dashboard h2 { margin: 0; color: #111827; font-size: 1.5rem; }
.subtitle { margin: 2px 0 0; color: #6b7280; font-size: 0.9rem; }

.stats-card { background: white; padding: 10px 15px; border-radius: 10px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); border-left: 4px solid #10b981; text-align: right; }
.stats-label { display: block; font-size: 0.65rem; text-transform: uppercase; font-weight: 700; color: #6b7280; }
.stats-value { font-size: 1.2rem; font-weight: 800; color: #111827; }

.hoja-stock { background: #ffffff; padding: 25px; border: 1px solid #e5e7eb; border-radius: 12px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05); margin-bottom: 30px; }
.campo label { display: block; font-weight: 600; margin-bottom: 6px; color: #374151; font-size: 0.85rem; }
.campo select, .campo input { width: 100%; padding: 10px; border: 1px solid #d1d5db; border-radius: 8px; font-size: 0.9rem; background-color: #f9fafb; }
.fila-doble { display: flex; gap: 15px; margin-bottom: 15px; }
.mitad { flex: 1; }

.input-con-unidad { position: relative; display: flex; align-items: center; }
.input-con-unidad input { padding-right: 40px; font-weight: bold; }
.unidad { position: absolute; right: 12px; color: #9ca3af; font-weight: 600; font-size: 0.8rem; }

.btn-ingreso { background: #111827; color: white; padding: 14px; border: none; border-radius: 8px; cursor: pointer; font-size: 0.95rem; font-weight: 700; width: 100%; transition: 0.2s; margin-top: 5px; }
.btn-ingreso:hover:not(:disabled) { background: #1f2937; }
.btn-ingreso:disabled { background: #9ca3af; cursor: not-allowed; }

.exito { background: #dcfce7; color: #166534; padding: 10px; border-radius: 8px; margin-top: 15px; text-align: center; font-size: 0.9rem; }
.error { background: #fef2f2; color: #991b1b; padding: 10px; border-radius: 8px; margin-top: 15px; text-align: center; font-size: 0.9rem; }

.historial-rapido { background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 1px 3px rgba(0,0,0,0.1); border: 1px solid #e5e7eb; }
.header-historial { background: #f9fafb; padding: 15px; border-bottom: 1px solid #e5e7eb; display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; gap: 10px; }
.input-search { padding: 8px 12px; border-radius: 8px; border: 1px solid #d1d5db; min-width: 250px; font-size: 0.85rem; }
.filtros-fecha { display: flex; gap: 8px; align-items: center; }
.filtros-fecha input { border: 1px solid #d1d5db; border-radius: 6px; padding: 4px 8px; font-size: 0.8rem; }

.table-container { width: 100%; overflow-x: auto; }
.tabla-mini { width: 100%; border-collapse: collapse; font-size: 0.85rem; }
.tabla-mini th { background: #f9fafb; text-align: left; padding: 12px; font-weight: 700; color: #4b5563; border-bottom: 2px solid #f3f4f6; }
.tabla-mini td { padding: 12px; border-bottom: 1px solid #f3f4f6; color: #1f2937; }

.txt-remito-solo { font-family: monospace; font-weight: 600; color: #475569; background: #f1f5f9; padding: 2px 6px; border-radius: 4px; }
.btn-undo { background: transparent; border: none; color: #ef4444; cursor: pointer; font-size: 1rem; }
.vacio-msg { text-align: center; padding: 30px !important; color: #9ca3af; }

.paginacion {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 15px;
    background: #f8fafc;
    border-top: 1px solid #e2e8f0;
    border-radius: 0 0 8px 8px;
}
.paginacion button {
    background-color: #3498db;
    color: white;
    border: none;
    padding: 8px 16px;
    border-radius: 4px;
    font-weight: bold;
    cursor: pointer;
    transition: background 0.2s;
}
.paginacion button:disabled {
    background-color: #cbd5e1;
    cursor: not-allowed;
}
.paginacion button:hover:not(:disabled) {
    background-color: #2980b9;
}
.paginacion span {
    font-weight: bold;
    color: #475569;
}
@media (max-width: 768px) {
    .fila-doble { flex-direction: column; gap: 0; }
    .header-dashboard { flex-direction: column; align-items: flex-start; gap: 10px; }
}
</style>