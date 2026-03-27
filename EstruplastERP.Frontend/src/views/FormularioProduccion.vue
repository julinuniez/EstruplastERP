<script setup lang="ts">
import { ref, onMounted, computed, watch, nextTick } from 'vue'
import HojaImpresion from '../components/HojaImpresion.vue'
import ListaProduccion from '../components/ListaProduccion.vue'
import { ProduccionAPI } from '@/services/produccionService'
import { useCalculosProduccion } from '@/composables/useCalculosProduccion';
import { useBorradorProduccion } from '@/composables/useBorradorProduccion';
import { useImpresionProduccion } from '@/composables/useImpresionProduccion';
import { useRecetaProduccion } from '@/composables/useRecetaProduccion';
import { useFiltrosProduccion } from '@/composables/useFiltrosProduccion';
import { useFazonProduccion } from '@/composables/useFazonProduccion';
import { useGuardadoProduccion } from '@/composables/useGuardadoProduccion';

interface Producto {
    id: number; nombre: string; codigoSku: string; esProductoTerminado: boolean;
    esGenerico: boolean; esFazon?: boolean; esMateriaPrima?: boolean; esScrap?: boolean; rubro?: string;
    largo: number; ancho: number; espesor: number; color?: string; pesoEspecifico: number;
    receta?: any[]; espesorMinimo?: number; espesorMaximo?: number; clienteId?: number;
    tipoMaterial?: string;
}
interface Cliente { id: number; razonSocial: string; esFazon?: boolean; }
interface ItemReceta {
    id: number | string; cantidad: number; nombreInsumo: string; densidad: number;
    materiaPrimaId: number; esColor?: boolean; esCarga?: boolean; esBase?: boolean;
    esBrillo?: boolean; esEstearato?: boolean; esUv?: boolean; esCaucho?: boolean;
    esFazonInput?: boolean; materialBase?: string;
}

const loading = ref(false);
const productos = ref<Producto[]>([])
const listaInventarioCompleto = ref<any[]>([])
const listaTodasMateriasPrimas = ref<any[]>([])
const clientes = ref<Cliente[]>([])
const recetaDinamica = ref<ItemReceta[]>([])
const stockFazonDetectado = ref<number | null>(null);
const listaLotesCliente = ref<any[]>([]); 
const loteFazonSeleccionadoId = ref<string | number>('');
const imprimiendoHistorial = ref(false);
const listaProduccionRef = ref<any>(null);
const limiteMinimo = ref(0);
const limiteMaximo = ref(0);
const mensaje = ref('');
const error = ref('');
const idProduccionGenerada = ref(false);
const ocultarFormula = ref(false);
const cantidadPalletsUsuario = ref(1);
const notaPedidoSugerida = ref<string>('');

const emit = defineEmits(['guardado'])

const form = ref({
    productoTerminadoId: '' as string | number,
    clienteId: '' as string | number,
    numeroPedidoCliente: '',
    notaPedido: '',
    cantidad: 1,
    observacion: '',
    largo: 0, ancho: 0, espesor: 0, color: '' as string,
    conBrillo: false, 
    tipoBrillo: '777',
    porcBrillo: 2.00, 
    llevaFilm: false, tipoCorona: 'Ninguno',
    conEstearato: false, esProductoColor: false, masterbatchId: '' as string | number, colorTexto: '',
    aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00,
    aditivoCarga: 0,
    merma: 8, kilosTotales: 0,
    esConsolidado: false,
    esBobina: false,
    kilosPorBobina: 0,
    productoNombre: '',
    clienteNombre: ''
})

const productoSeleccionado = computed(() => productos.value.find(p => p.id === Number(form.value.productoTerminadoId)) || null);
const clienteSeleccionado = computed(() => clientes.value.find(c => c.id === Number(form.value.clienteId)) || null);

const { limpiarBorrador, revisarYRecuperarBorrador } = useBorradorProduccion(form, recetaDinamica, mensaje);

const { 
    totalPorcentajeReceta, 
    densidadMezcla, 
    kilosCalculados, 
    factorMerma 
} = useCalculosProduccion(form, recetaDinamica, productoSeleccionado);

const { 
    listaMasterbatches,
    idCristal555,
    mostrarCajaColor,
    colorFinalParaPDF,
    clienteTieneFazonActivo,
    clientesHabilitados,
    medidasBloqueadas,
    espesorValido,
    listaProductosDisponibles,
    materiasPrimasLimpias,
    insumosSinStock,
    hayBloqueoDeStock
} = useFiltrosProduccion(
    form, recetaDinamica, productos, clientes, listaTodasMateriasPrimas, 
    listaInventarioCompleto, productoSeleccionado, clienteSeleccionado, 
    kilosCalculados, factorMerma, limiteMinimo, limiteMaximo
);

const { 
    balancearBase, 
    recalcularFormulaAutomatica, 
    quitarInsumoManual, 
    agregarInsumoDesdeHijo 
} = useRecetaProduccion(
    form, 
    recetaDinamica, 
    listaTodasMateriasPrimas, 
    listaInventarioCompleto, 
    listaMasterbatches, 
    idCristal555, 
    mostrarCajaColor
);

const { 
    detectarMaterial, 
    actualizarRecetaFazonConCliente, 
    alCambiarLoteFazon, 
    aplicarLoteFazonAReceta 
} = useFazonProduccion(
    recetaDinamica, 
    listaInventarioCompleto, 
    listaLotesCliente, 
    loteFazonSeleccionadoId, 
    stockFazonDetectado, 
    clienteTieneFazonActivo, 
    balancearBase
);

const { 
    limpiarFormulario, 
    registrarProduccion, 
    cargarNotaPedidoSugerida, 
    aplicarNotaPedidoSugerida 
} = useGuardadoProduccion(
    form, recetaDinamica, notaPedidoSugerida, mensaje, error, loading, 
    idProduccionGenerada, totalPorcentajeReceta, espesorValido, limiteMinimo, 
    limiteMaximo, kilosCalculados, colorFinalParaPDF, listaProduccionRef, 
    limpiarBorrador, emit
);

const { imprimirDesdeHistorial, imprimirLoteOPsDesdeHistorial } = useImpresionProduccion(
    form, recetaDinamica, ocultarFormula, imprimiendoHistorial, cantidadPalletsUsuario, mensaje, error, loading, 
    listaProduccionRef, balancearBase, limpiarFormulario
);


async function CargarProductosFiltrados(clienteId: number | string = '') {
    try {
        const cid = clienteId ? clienteId : '';
        productos.value = await ProduccionAPI.obtenerProductos(cid);
        if (form.value.productoTerminadoId) {
            const estaEnLista = listaProductosDisponibles.value.some(p => p.id === Number(form.value.productoTerminadoId));
            if (!estaEnLista) {
                form.value.productoTerminadoId = '';
                recetaDinamica.value = [];
            }
        }
    } catch (e) { console.error(e); }
}

async function CargarDatosProductos(id: number) {
    if (!id || imprimiendoHistorial.value) return; 
    try {
        const prod = await ProduccionAPI.obtenerProductoPorId(id);

        if (prod.receta && Array.isArray(prod.receta) && prod.receta.length > 0) {
            recetaDinamica.value = prod.receta.map((r: any) => ({
                id: r.id || Math.random(),
                materiaPrimaId: r.materiaPrimaId || r.id,
                nombreInsumo: r.nombreInsumo || r.nombreMateriaPrima || r.nombre,
                cantidad: r.cantidad || r.porcentaje || 0,
                densidad: r.densidad || r.pesoEspecifico || 1.1,
                esBase: r.esBase || false
            }));
            
            if (typeof balancearBase === 'function') balancearBase();
        }

        if (!form.value.largo || form.value.largo === 0) {
            form.value.esBobina = (prod.nombre || '').toUpperCase().includes('BOBINA');
            form.value.largo = form.value.esBobina ? 0 : (prod.largo || 0);
        }
        
        if (!form.value.ancho || form.value.ancho === 0) {
            form.value.ancho = prod.ancho || 0;
        }
        
        if (!form.value.espesor || form.value.espesor === 0) {
            form.value.espesor = prod.espesor || 0;
        }

        if (!form.value.colorTexto || form.value.colorTexto === '') {
            form.value.colorTexto = prod.color || '';
        }

    } catch (e) { 
        console.error("Error cargando datos maestros:", e); 
    }
}

watch(mostrarCajaColor, (v) => {
    if (!v) form.value.masterbatchId = '';
});

watch(() => form.value.clienteId, async (nuevoCli) => {
    if (nuevoCli && !form.value.productoTerminadoId) {
        await CargarProductosFiltrados(nuevoCli);
    }
    if (nuevoCli && form.value.productoTerminadoId) {
        const prod = productos.value.find(p => p.id === Number(form.value.productoTerminadoId));
        if (prod) {
            setTimeout(async () => { await actualizarRecetaFazonConCliente(nuevoCli, prod); }, 200);
        }
    }
});

watch(() => form.value.productoTerminadoId, (v) => {
    if (form.value.esConsolidado) return;
    if (v && !imprimiendoHistorial.value) {
        CargarDatosProductos(Number(v));
    } else if (!v) {
        recetaDinamica.value = [];
    }
});

watch(
    [
        () => form.value.masterbatchId, () => form.value.aditivoCarga, 
        () => form.value.porcBrillo, () => form.value.conEstearato, 
        () => form.value.aditivoUV, () => form.value.porcentajeUv, 
        () => form.value.aditivoCaucho, () => form.value.porcentajeCaucho,
        () => form.value.conBrillo, () => form.value.tipoBrillo
    ],
    recalcularFormulaAutomatica
);

watch(() => form.value.espesor, (v) => { if (v < 1) form.value.conBrillo = false; });
watch(() => form.value.conBrillo, (v) => { if (!v) form.value.llevaFilm = false; });

watch(kilosCalculados, (v) => {
    if (!form.value.esConsolidado && !imprimiendoHistorial.value) {
        form.value.kilosTotales = v;
    }
}, { immediate: true });

watch(() => form.value.kilosTotales, (v) => {
    if (v > 1000) {
        cantidadPalletsUsuario.value = Math.ceil(v / 1000);
    } else {
        cantidadPalletsUsuario.value = 1;
    }
});

watch(imprimiendoHistorial, (estaImprimiendo) => {
    if (!estaImprimiendo && recetaDinamica.value.length > 0 && !form.value.esConsolidado) {
        balancearBase();
    }
});

onMounted(async () => {
    try {
        loading.value = true;
        
        const [resProd, resCli, resInv] = await Promise.all([
            ProduccionAPI.obtenerProductos(),
            ProduccionAPI.obtenerClientes(),
            ProduccionAPI.obtenerInventarioCompleto()
        ]);
        
        if (Array.isArray(resProd)) {
            productos.value = resProd;
            listaTodasMateriasPrimas.value = productos.value.filter(p => p.esMateriaPrima);
        }
        if (Array.isArray(resCli)) clientes.value = resCli;
        if (Array.isArray(resInv)) listaInventarioCompleto.value = resInv;

        revisarYRecuperarBorrador();
    } catch (e) {
        console.error("Error inicializando producción:", e);
    } finally {
        loading.value = false;
    }

    await cargarNotaPedidoSugerida();
});
defineExpose({ form, error, mensaje, registrarProduccion });
</script>

<template>
  <div class="contenedor-principal-produccion">
    
    <div class="bloque-superior">
        <div class="panel-izquierdo">
            <div class="hoja-contenedor">
                <HojaImpresion 
                    id="hoja-de-impresion"
                    :form="form" 
                    :producto="productoSeleccionado" 
                    :cliente="clienteSeleccionado" 
                    :receta="recetaDinamica" 
                    :colorFinal="colorFinalParaPDF" 
                    :densidad="densidadMezcla" 
                    :totalPorcentaje="totalPorcentajeReceta" 
                    :materiasPrimas="materiasPrimasLimpias" 
                    :ocultarFormula="ocultarFormula" 
                    @add-insumo="agregarInsumoDesdeHijo" 
                    @remove-insumo="quitarInsumoManual" 
                    @update-receta="balancearBase"  
                />
            </div>
        </div>

        <div class="panel-derecho">
            <div class="header-control"><h3>⚙️ Configuración</h3></div>
            
            <label>Cliente / Producto:</label>
            <select v-model="form.clienteId" style="margin-bottom:5px">
                <option disabled value="">Cliente...</option>
                <option v-for="c in clientes" :key="c.id" :value="c.id">
                    {{c.razonSocial}} {{ c.esFazon ? '' : '(Venta)' }}
                </option>
            </select>

            <label style="color:#f39c12;">📂 N° Pedido Cliente (OC):</label>
            <input type="text" v-model="form.numeroPedidoCliente" placeholder="Ej: OC-4455" style="font-weight:bold; border: 1px solid #f39c12; margin-bottom: 5px;">

            <label style="color:#1abc9c;">🧾 Nota de Pedido (Flexxus):</label>
            <div class="fila-input" style="margin-bottom: 5px;">
                <input
                    type="text"
                    v-model="form.notaPedido"
                    placeholder="Ej: 12345"
                    style="font-weight:bold; border: 1px solid #1abc9c;"
                >
                <button
                    type="button"
                    class="btn-sugerido"
                    :disabled="!notaPedidoSugerida"
                    @click="aplicarNotaPedidoSugerida"
                    :title="notaPedidoSugerida ? `Copiar número anterior: ${notaPedidoSugerida}` : 'Sin sugerencia'"
                >
                    Usar: {{ notaPedidoSugerida || '-' }}
                </button>
            </div>
            
            <select v-model="form.productoTerminadoId">
                <option disabled value="">Seleccionar Producto...</option>
                <option v-for="p in listaProductosDisponibles" :key="p.id" :value="p.id">
                    {{ p.esFazon ? '★ ' : '' }}{{ p.nombre }} {{ p.esGenerico ? '(A Medida)' : (p.esFazon ? '(Fazon)' : '(Estándar)') }}
                </option>
            </select>

            <div v-if="form.productoTerminadoId" class="caja-detalles-producto">
                
                <div v-if="listaLotesCliente.length > 0" class="box-fazon-selector">
                    <label style="color: #2ecc71;">♻️ Lote Recuperado (Fazón):</label>
                    <select v-model="loteFazonSeleccionadoId" @change="alCambiarLoteFazon" class="select-fazon">
                        <option disabled value="">-- Seleccionar Lote --</option>
                        <option v-for="lote in listaLotesCliente" :key="lote.id" :value="lote.id">
                            {{ lote.nombre }} (Stock: {{ lote.stockActual }} kg)
                        </option>
                    </select>
                </div>

                <div v-if="mostrarCajaColor" class="box-color">
                    <label style="color: #f39c12;">🎨 Seleccione Color:</label>
                    <select v-model="form.masterbatchId">
                        <option disabled value="">-- Elegir Masterbatch --</option>
                        <option v-for="mb in listaMasterbatches" :key="mb.id" :value="mb.id">{{ mb.nombre }}</option>
                    </select>
                </div>

                <div class="fila-input" style="margin-top: 5px;">
                    <div style="flex:1">
                        <label style="color:#3498db;">✏️ Texto Color (Opcional):</label>
                        <input 
                            type="text" 
                            v-model="form.colorTexto" 
                            placeholder="Ej: AZUL PANTONE..."
                            style="font-weight:bold; color:#2980b9;"
                        >
                    </div>
                </div>

                <div style="display: flex; justify-content: space-between; align-items: flex-end; margin-top: 15px; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-bottom: 5px;">
                    <label class="lbl-sep" style="border: none; margin: 0 !important; padding: 0;">
                        Medidas: <span v-if="medidasBloqueadas" style="color:#e74c3c">(FIJAS)</span><span v-else style="color:#2ecc71">(EDITABLES)</span>
                    </label>
                    <label class="check-container" style="margin: 0 !important; color: #3498db;">
                        <input type="checkbox" v-model="form.esBobina"> 🗞️ Formato Bobina
                    </label>
                </div>
                
                <div style="font-size:11px; color:#bbb; margin-top:-5px; margin-bottom:5px;">
                    <span v-if="limiteMaximo > 0">Rango: {{ limiteMinimo }} - {{ limiteMaximo }} mm</span>
                    <span v-else-if="limiteMinimo > 0">Mínimo: {{ limiteMinimo }} mm (Sin tope)</span>
                </div>

                <div class="fila-input">
                    <div v-if="!form.esBobina">
                        <label>Largo (mm)</label>
                        <input type="number" v-model="form.largo" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}">
                    </div>
                    
                    <div v-else>
                        <label style="color:#f39c12">Kilos x Bobina</label>
                        <input type="number" v-model="form.kilosPorBobina" step="0.1" style="border: 2px solid #f39c12; font-weight: bold; background: #fff3e0; color: #d35400;">
                    </div>
                    
                    <div>
                        <label>Ancho (mm)</label>
                        <input type="number" v-model="form.ancho" :disabled="medidasBloqueadas" :class="{'input-lock': medidasBloqueadas}">
                    </div>
                </div>
                
                <div class="fila-input">
                    <div>
                        <label>Espesor</label>
                        <input type="number" v-model="form.espesor" step="0.01" 
                            :disabled="medidasBloqueadas" 
                            :class="{'input-lock': medidasBloqueadas, 'input-error': !espesorValido}" 
                            style="font-weight:bold;">
                    </div>
                    <div><label>Cant.</label><input type="number" v-model="form.cantidad" min="1"></div>
                </div>
                
                <div class="fila-input" style="margin-top:10px; border-top:1px dashed #7f8c8d; padding-top:10px;">
                    <div style="flex:1">
                        <label style="color:#e67e22;">🔥 Desperdicio (%)</label>
                        <input type="number" v-model="form.merma" min="0" max="50" style="color:#e67e22; font-weight:bold;">
                    </div>
                </div>

                <div class="resumen-peso">Peso Final PT: {{ form.kilosTotales }} Kg <small style="color:#bbb; display:block;">(Consumo Real MP +{{ form.merma }}%)</small></div>
                
                <label class="lbl-sep">Aditivos:</label>
                
                <div class="fila-control-aditivo" style="align-items: flex-start;">
                    <label class="check-container" :class="{ 'disabled': form.espesor < 1 }" style="margin-top: 5px !important;">
                        <input type="checkbox" v-model="form.conBrillo" :disabled="form.espesor < 1"> ✨ Brillo
                    </label>
                    <div v-if="form.conBrillo" class="bloque-derecha-brillo">
                        <select v-model="form.tipoBrillo" class="select-brillo">
                            <option value="777">Brillo 777</option>
                            <option value="555">Brillo 555 (Cristal)</option>
                        </select>
                        <div class="input-porcentaje">
                            <input type="number" v-model="form.porcBrillo" step="0.01" min="0"> %
                        </div>
                    </div>
                </div>

                <div class="fila-control-aditivo"><label class="check-container" :class="{ 'disabled': !form.conBrillo }"><input type="checkbox" v-model="form.llevaFilm" :disabled="!form.conBrillo"> 🛡️ Con Film</label></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.conEstearato"> 🧪 Estearato</label></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.aditivoUV"> ☀️ UV</label><div v-if="form.aditivoUV" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeUv" step="0.01" min="0"> %</div></div></div>
                <div class="fila-control-aditivo"><label class="check-container"><input type="checkbox" v-model="form.aditivoCaucho"> 🚜 Caucho</label><div v-if="form.aditivoCaucho" class="bloque-derecha"><div class="input-porcentaje"><input type="number" v-model="form.porcentajeCaucho" step="0.01" min="0"> %</div></div></div>

                <label style="margin-top:10px; font-size:13px; color:#bdc3c7">⚡ Tratamiento Corona:</label>
                <select v-model="form.tipoCorona"><option value="Ninguno">Sin Tratamiento</option><option value="Simple">Simple</option><option value="Doble">Doble</option></select>
                
                <label class="lbl-sep">Cargas:</label>
                <div class="fila-input"><div style="flex:1"><label>Carga Mineral (%)</label><input type="number" v-model="form.aditivoCarga"></div></div>
                
            </div> <div class="fila-input" style="margin-top:10px"><div style="width: 100%"><label>Obs:</label><input type="text" v-model="form.observacion" style="width:100%"></div></div>
            
            <div v-if="Math.abs(totalPorcentajeReceta - 100) > 0.5" class="alerta-error">⚠️ Receta suma {{ totalPorcentajeReceta }}%.</div>
            
            <div v-if="hayBloqueoDeStock" class="alerta-stock-warning">
                <h4>⚠️ Material Insuficiente (Irá a la Cola)</h4>
                <p style="margin: 0 0 5px 0; font-size: 11px;">La orden se guardará en el Backlog hasta que ingrese este stock:</p>
                <ul>
                    <li v-for="(falla, i) in insumosSinStock" :key="i">
                        <strong>{{ falla.nombre }}</strong>: Falta {{ falla.diferencia.toFixed(2) }} kg (Disp: {{ falla.disponible }})
                    </li>
                </ul>
            </div>

            <button 
                class="btn-guardar" 
                @click="registrarProduccion" 
                :disabled="loading || !form.clienteId || !form.productoTerminadoId" 
                :class="{ 'btn-warning': hayBloqueoDeStock && form.clienteId && form.productoTerminadoId }"
            >
                <span v-if="loading">⏳ PROCESANDO...</span>
                <span v-else-if="!form.clienteId || !form.productoTerminadoId">🚫 SELECCIONE CLIENTE Y PRODUCTO</span>
                <span v-else-if="hayBloqueoDeStock">📥 GUARDAR EN COLA</span>
                <span v-else>💾 GUARDAR ORDEN LISTA</span>
            </button>
            
            <p class="success">{{ mensaje }}</p>
            <p class="error">{{ error }}</p>
        </div>
    </div>

    <div class="bloque-inferior">
        <ListaProduccion 
            ref="listaProduccionRef" 
            @imprimir-historial="imprimirDesdeHistorial" 
            @imprimir-carga-consolidada="imprimirDesdeHistorial" 
            @imprimir-lote-op="imprimirLoteOPsDesdeHistorial"
        />
    </div>

  </div>
</template>

<style scoped>
.contenedor-principal-produccion {
    display: flex; flex-direction: column; width: 100%; min-height: 100vh;
    font-family: 'Segoe UI', sans-serif; background-color: #ecf0f1;
}
.bloque-superior { display: flex; width: 100%; flex-wrap: wrap; }
.panel-izquierdo { 
    flex: 1; background-color: #e0e6ed; display: flex; justify-content: center; 
    align-items: flex-start; padding: 20px; border-right: 1px solid #bdc3c7; 
    overflow: hidden; min-width: 400px; 
}
.hoja-contenedor { 
    background: white; width: 210mm; min-height: 297mm; 
    box-shadow: 0 10px 25px rgba(0,0,0,0.1); transform: scale(0.80); 
    transform-origin: top center; margin-bottom: -350px; 
}
.panel-derecho { 
    width: 350px; min-width: 350px; background-color: #2c3e50; color: white; 
    display: flex; flex-direction: column; padding: 20px; 
    box-shadow: -5px 0 15px rgba(0,0,0,0.1); z-index: 10; border-left: 1px solid #34495e; 
}
.bloque-inferior { width: 100%; padding: 20px; background-color: #f8f9fa; border-top: 3px solid #bdc3c7; }

.header-control h3 { margin-top: 0; border-bottom: 2px solid #3498db; padding-bottom: 10px; color: #ecf0f1; font-size: 1.1rem; }
label { display: block; margin-top: 8px; font-size: 13px; color: #bdc3c7; font-weight: 600; }
select, input { width: 100%; padding: 8px; margin-top: 2px; border-radius: 4px; border: none; font-size: 13px; box-sizing: border-box; background: #ecf0f1; color: #2c3e50; }
.fila-input { display: flex; gap: 8px; margin-bottom: 5px; }
.btn-sugerido {
    width: 130px;
    margin-top: 2px;
    border-radius: 4px;
    border: 1px solid #1abc9c;
    background: transparent;
    color: #1abc9c;
    font-weight: bold;
    cursor: pointer;
    font-size: 12px;
    padding: 8px;
}
.btn-sugerido:disabled {
    opacity: 0.5;
    cursor: not-allowed;
}
.seccion-medidas-editables { background: #34495e; padding: 12px; border-radius: 6px; margin-top: 15px; border: 1px solid #4e6475; }
.box-color { margin-bottom: 15px; border: 1px dashed #f39c12; padding: 5px; border-radius: 4px; }
.lbl-sep { color: #f1c40f !important; font-weight: bold; border-bottom: 1px dashed #7f8c8d; padding-bottom: 3px; margin-top: 15px !important; margin-bottom: 5px; }
.resumen-peso { font-weight: bold; color: #2ecc71; text-align: right; margin-top: 10px; font-size: 14px; border-top: 1px solid #7f8c8d; padding-top: 5px; }
.check-container { display: flex; align-items: center; cursor: pointer; color: #ecf0f1; font-weight: bold; font-size: 13px; margin-top: 8px !important; }
.check-container input { width: auto; margin-right: 8px; }
.check-container.disabled { opacity: 0.5; cursor: not-allowed; }
.alerta-error { background: #c0392b; color: white; padding: 10px; border-radius: 5px; margin-top: 15px; font-weight: bold; text-align: center; font-size: 12px; }

.alerta-stock-warning { 
    background-color: #fff9e6; 
    border: 1px solid #f1c40f; 
    color: #d35400; 
    padding: 10px; 
    border-radius: 6px; 
    margin-top: 15px; 
    font-size: 12px; 
    text-align: left; 
}
.alerta-stock-warning h4 { margin: 0 0 5px 0; color: #e67e22; font-size: 13px; }
.alerta-stock-warning ul { margin: 0; padding-left: 20px; }

.btn-guardar { background: #27ae60; color: white; margin-top: 20px; border: none; padding: 12px; border-radius: 6px; cursor: pointer; font-size: 1em; font-weight: bold; width: 100%; transition: background 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #7f8c8d; cursor: not-allowed; opacity: 0.7; }
.btn-warning { background: #f39c12 !important; color: white !important; }
.btn-warning:hover { background: #e67e22 !important; }

.success { color: #2ecc71; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.error { color: #e74c3c; text-align: center; font-weight: bold; margin-top: 10px; font-size: 13px; }
.fila-control-aditivo { display: flex; justify-content: space-between; align-items: center; margin-top: 8px; }
.input-porcentaje { display: flex; align-items: center; background: #ecf0f1; border-radius: 4px; padding-right: 5px; color: #333; }
.input-porcentaje input { width: 45px !important; margin: 0 !important; text-align: right; background: transparent; color: #333; }

.bloque-derecha-brillo { display: flex; flex-direction: column; align-items: flex-end; gap: 4px; }
.select-brillo { width: 130px; padding: 4px; font-size: 11px; margin: 0; background: #ecf0f1; border-radius: 4px; border: none; color: #2c3e50; font-weight: bold; }

.bloque-derecha { display: flex; flex-direction: column; align-items: flex-end; }
.input-lock { background-color: #4a5d6e !important; color: #bdc3c7 !important; cursor: not-allowed; border: 1px solid #3e4f5e !important; }
.input-error { border: 2px solid #e74c3c !important; background-color: #fab1a0 !important; color: #c0392b !important; }

.grupo-botones-pdf { display: flex; gap: 5px; margin-top: 10px; }
.btn-imprimir { flex: 1; padding: 8px; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; font-size: 12px; color: white; }
.btn-orden { background: #34495e; border: 1px solid #7f8c8d; } .btn-orden:hover { background: #2980b9; }
.btn-carga { background: #8e44ad; border: 1px solid #9b59b6; } .btn-carga:hover { background: #9b59b6; }

.box-fazon-selector {
    background-color: #27ae60;
    padding: 10px;
    border-radius: 6px;
    margin-top: 10px;
    border: 1px solid #2ecc71;
}
.box-fazon-selector label {
    color: white !important;
}
.select-fazon {
    background-color: white;
    font-weight: bold;
    color: #2c3e50;
    border: 2px solid #2ecc71;
}

.caja-detalles-producto {
    background-color: #34495e; /* Un tono más claro que el azul oscuro de atrás */
    padding: 15px;
    border-radius: 8px;
    margin-top: 15px;
    border: 1px solid #4a6278;
    box-shadow: inset 0 2px 4px rgba(0,0,0,0.1);
}

.alerta-pallets {
    background-color: #fff3cd;
    padding: 15px;
    border-radius: 8px;
    margin-top: 15px;
    color: #856404;
    border: 1px solid #ffeeba;
}

@media (max-width: 1000px) { 
    .bloque-superior { flex-direction: column; } 
    .panel-izquierdo { width: 100%; border-right: none; border-bottom: 1px solid #bdc3c7; } 
    .panel-derecho { width: 100%; min-width: auto; } 
    .hoja-contenedor { transform: scale(0.55); margin-bottom: -400px; } 
}
</style>