<script setup lang="ts">
import { ref, computed } from 'vue'

const props = defineProps<{
  form: any;
  producto: any;
  cliente: any;
  empleado: any;
  receta: any[];
  colorFinal: string;
  densidad: number;
  totalPorcentaje: number;
  materiasPrimas: any[]; 
  ocultarFormula: boolean; 
}>();

const emit = defineEmits(['add-insumo', 'remove-insumo', 'update-receta']);

// Variables para el buscador manual
const insumoBusquedaTexto = ref(''); 
const insumoExtraPorc = ref<number | ''>('');
const mostrarLista = ref(false); 

const cantidadCopias = computed(() => props.ocultarFormula ? 2 : 1);

// --- LÓGICA DE PESOS ---
const pesoBrutoExacto = computed(() => {
    const kilosNetos = Number(props.form?.kilosTotales) || 0;
    const porcentajeDesperdicio = Number(props.form?.merma) || 0; 
    const resultado = kilosNetos * (1 + (porcentajeDesperdicio / 100));
    return isNaN(resultado) ? 0 : resultado;
});

const pesoVisualRedondeado = computed(() => Math.ceil(pesoBrutoExacto.value));

// --- FILTRO DE INSUMOS ---
const sugerenciasFiltradas = computed(() => {
    const texto = insumoBusquedaTexto.value.trim().toUpperCase();
    let lista = props.materiasPrimas || [];

    lista = lista.filter(mp => {
        const nombre = (mp.nombre || '').toUpperCase();
        if (nombre.includes('BASE') && !nombre.includes('ALTA')) return false; 
        if (nombre.includes('GENERICO') || nombre.includes('GENÉRICO')) return false;
        if (nombre.includes('MATERIAL DE CLIENTE')) return false;
        if (mp.id >= 990 && mp.id <= 999) return false;

        const esPrivado = mp.clienteId || nombre.includes('PROPIEDAD DE') || nombre.includes('(FAZÓN)') || nombre.includes('(FAZON)') || nombre.startsWith('MP:');

        if (esPrivado) {
            if (!props.cliente || !props.cliente.id) return false; 
            if (mp.clienteId && mp.clienteId != props.cliente.id) return false;
            if (nombre.includes('PROPIEDAD DE')) {
                 const nombreClienteActual = (props.cliente.razonSocial || '').toUpperCase();
                 if (!nombre.includes(nombreClienteActual)) return false;
            }
        }
        return true;
    });

    if (texto) {
        lista = lista.filter(mp => {
            const nombre = (mp.nombre || '').toUpperCase();
            const rubro = (mp.rubro || '').toUpperCase();
            return nombre.includes(texto) || rubro.includes(texto);
        });
    }
    return [...lista].sort((a, b) => a.nombre.localeCompare(b.nombre));
});

const seleccionarInsumo = (mp: any) => {
    insumoBusquedaTexto.value = mp.nombre;
    mostrarLista.value = false;
};

const cerrarListaConDelay = () => {
    setTimeout(() => { mostrarLista.value = false; }, 200);
};

const solicitarAgregar = () => {
    if (!insumoBusquedaTexto.value || !insumoExtraPorc.value) return;
    const mpEncontrada = sugerenciasFiltradas.value.find(m => m.nombre === insumoBusquedaTexto.value);

    if (mpEncontrada) {
        emit('add-insumo', { id: mpEncontrada.id, porcentaje: Number(insumoExtraPorc.value) });
        insumoBusquedaTexto.value = '';
        insumoExtraPorc.value = '';
        mostrarLista.value = false;
    } else {
        alert("⚠️ Seleccione un insumo válido de la lista.");
    }
};

const solicitarQuitar = (index: number) => { emit('remove-insumo', index); };

// Helper para fecha
const fechaHoy = new Date().toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
</script>

<template>
  <div id="hoja-de-impresion" class="contenedor-principal-pdf">
    
    <div v-for="n in cantidadCopias" :key="n" class="pagina-copia" :class="{ 'modo-mitad': cantidadCopias === 2 }">
        
        <div v-if="cantidadCopias === 2" class="marca-agua">{{ n === 1 ? 'ORIGINAL' : 'DUPLICADO' }}</div>

        <div class="header-pdf">
            <div class="logo-area">
                <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAlYAAAA4CAYAAAA/xLYcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkY4MzhCNEFBOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkY4MzhCNEFCOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjgzOEI0QTg4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RjgzOEI0QTk4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6QwnXzAAATQUlEQVR42uxd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" alt="Logo" class="logo-img-pdf">
            </div>
            <div class="datos-orden">
                <h3>{{ ocultarFormula ? 'ORDEN DE PRODUCCIÓN' : 'HOJA DE CARGA' }}</h3>
                <p>FECHA: <strong>{{ fechaHoy }}</strong></p>
                <p>TURNO: <strong>{{ form.turno ? form.turno.toUpperCase() : '-' }}</strong></p>
            </div>
        </div>
        
        <div class="fila-pdf">
            <div style="display: flex; justify-content: space-between;">
                <div><strong>RESPONSABLE:</strong> <span class="dato-relleno">{{ empleado?.nombreCompleto }}</span></div>
                <div><strong>CLIENTE:</strong> <span class="dato-relleno">{{ cliente?.razonSocial }}</span></div>
            </div>
        </div>

        <div class="caja-producto-pdf">
            <div class="titulo-seccion-pdf">MATERIAL / PRODUCTO A FABRICAR</div>
            <div class="producto-nombre-pdf">{{ producto?.nombre || '...' }}</div>
            <div class="producto-sku-pdf">CÓDIGO: {{ producto?.codigoSku }}</div>
            <div v-if="producto?.esGenerico" style="font-size:10px; font-style:italic; margin-top:2px">(MEDIDAS ESPECIALES)</div>
        </div>

        <div class="ficha-tecnica-pdf">
            <div class="dato-box-pdf"><span class="label-tech-pdf">COLOR</span><span class="valor-tech-pdf">{{ colorFinal }}</span></div>
            <div class="dato-box-pdf"><span class="label-tech-pdf">LARGO</span><span class="valor-tech-pdf">{{ form.largo }} mm</span></div>
            <div class="dato-box-pdf"><span class="label-tech-pdf">ANCHO</span><span class="valor-tech-pdf">{{ form.ancho }} mm</span></div>
            <div class="dato-box-pdf"><span class="label-tech-pdf">ESPESOR</span><span class="valor-tech-pdf">{{ form.espesor }} mm</span></div>
        </div>

        <div class="ficha-tecnica-pdf">
            <div class="dato-box-pdf">
                <span class="label-tech-pdf">BRILLO</span>
                <span class="valor-tech-pdf">{{ form.conBrillo ? 'SÍ' : 'NO' }}</span>
            </div>
            <div class="dato-box-pdf">
                <span class="label-tech-pdf">UV</span>
                <span class="valor-tech-pdf">{{ form.aditivoUV ? 'SÍ' : 'NO' }}</span>
            </div>
            <div class="dato-box-pdf">
                <span class="label-tech-pdf">CAUCHO</span>
                <span class="valor-tech-pdf">{{ form.aditivoCaucho ? 'SÍ' : 'NO' }}</span>
            </div>
            <div class="dato-box-pdf">
                <span class="label-tech-pdf">TRAT. CORONA</span>
                <span class="valor-tech-pdf">{{ form.tipoCorona && form.tipoCorona !== 'Ninguno' ? form.tipoCorona.toUpperCase() : 'NO' }}</span>
            </div>
            
            <div v-if="!ocultarFormula" class="dato-box-pdf doble-ancho-pdf">
                <span class="label-tech-pdf">TOTAL CARGA</span>
                <span class="valor-tech-pdf" style="font-size: 18px;">{{ pesoVisualRedondeado }} kg</span>
            </div>
        </div>

        <div class="ficha-tecnica-pdf" v-if="form.aditivoCarga > 0 || (form.conBrillo && form.llevaFilm)">
            <div class="dato-box-pdf" v-if="form.conBrillo && form.llevaFilm"><span class="label-tech-pdf">FILM PROTECTOR</span><span class="valor-tech-pdf">SÍ</span></div>
            <div class="dato-box-pdf" v-if="form.aditivoCarga > 0"><span class="label-tech-pdf">CARGA MINERAL</span><span class="valor-tech-pdf">{{ form.aditivoCarga }} %</span></div>
            <div class="dato-box-pdf" style="background: #f4f4f4; flex-grow: 1; border:none;"></div>
        </div>

        <div v-if="receta.length > 0" v-show="!ocultarFormula" class="seccion-receta-pdf">
            <div class="titulo-receta-pdf">
                FÓRMULA (Densidad: {{ densidad }})
                <span style="float:right; font-size: 0.8em; color: #333">Total: {{ totalPorcentaje }}%</span>
            </div>
            <table class="tabla-receta-pdf">
                <thead>
                    <tr>
                        <th>INSUMO</th>
                        <th style="width:100px">% MEZCLA</th>
                        <th style="width:100px">PESO REAL</th>
                        <th data-html2canvas-ignore="true" style="width:40px"></th>
                    </tr>
                </thead>
                <tbody>
                    <template v-for="(r, i) in receta" :key="i">
                        <tr v-if="!r.esEstearato">
                            <td>{{ r.nombreInsumo }}</td>
                            <td style="text-align:center; vertical-align: middle;">
                                <div>
                                    <input 
                                        type="number" 
                                        v-model.number="r.cantidad" 
                                        class="input-print" 
                                        step="0.01" 
                                        @input="$emit('update-receta')"
                                        @change="$emit('update-receta')" 
                                    > %
                                </div>
                            </td>
                            <td style="text-align:right"><strong>{{ ((pesoBrutoExacto * (parseFloat(r.cantidad.toString()) || 0)) / 100).toFixed(3) }} kg</strong></td>
                            
                            <td data-html2canvas-ignore="true">
                                <button @click="solicitarQuitar(i)" class="btn-borrar-insumo">X</button>
                            </td>
                        </tr>
                    </template>
                </tbody>
            </table>
            
            <div class="agregar-fila-pdf" data-html2canvas-ignore="true">
                <div class="buscador-wrapper">
                    <input 
                        type="text" 
                        v-model="insumoBusquedaTexto" 
                        placeholder="🔍 Buscar Insumo..." 
                        class="input-buscador"
                        @focus="mostrarLista = true"
                        @click="mostrarLista = true"
                        @input="mostrarLista = true"
                        @blur="cerrarListaConDelay"
                    >
                    <div v-if="mostrarLista && sugerenciasFiltradas.length > 0" class="lista-resultados">
                        <div 
                            v-for="mp in sugerenciasFiltradas" 
                            :key="mp.id" 
                            class="item-resultado"
                            @click="seleccionarInsumo(mp)"
                        >
                            <div style="font-weight:bold;">{{ mp.nombre }}</div>
                            <div style="font-size:10px; color:#666;">{{ mp.rubro }}</div>
                        </div>
                    </div>
                </div>

                <input type="number" v-model="insumoExtraPorc" placeholder="%" style="width: 60px;">
                <button @click="solicitarAgregar" type="button" class="btn-add-insumo">Añadir</button>
            </div>
        </div>

        <div class="fila-lotes-pdf">
            <div class="mitad-pdf"><strong>CANTIDAD (UNIDADES):</strong><div class="recuadro-gigante-pdf">{{ form.cantidad }}</div></div>
            <div class="mitad-pdf"><strong>OBSERVACIONES:</strong><div class="recuadro-gigante-pdf texto-lote-pdf">{{ form.observacion }}</div></div>
        </div>
        
        <div class="pie-firma-pdf">
            <div class="linea-firma-pdf">Firma Responsable</div>
            <div class="linea-firma-pdf">Firma Calidad</div>
        </div>

        <div v-if="cantidadCopias === 2 && n === 1" class="linea-corte-pdf">
            <span>✂️ CORTAR AQUÍ</span>
        </div>
    </div>
  </div>
</template>

<style>
/* Estructura A4 */
.contenedor-principal-pdf { 
    background: white; width: 209mm; min-height: 290mm; padding: 0; 
    box-sizing: border-box; color: black; font-family: Arial, sans-serif; 
    position: relative; 
}
.pagina-copia { 
    padding: 15mm; box-sizing: border-box; width: 100%; height: 290mm; 
    display: flex; flex-direction: column; position: relative; 
}
.pagina-copia.modo-mitad { 
    height: 145mm; padding: 5mm 15mm; border-bottom: 1px dashed #999; 
    display: block; 
}
.modo-mitad .header-pdf { margin-bottom: 5px; padding-bottom: 5px; }
.modo-mitad .producto-nombre-pdf { font-size: 16px; }
.modo-mitad .recuadro-gigante-pdf { height: 25px; font-size: 16px; }

/* Header */
.header-pdf { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid black; padding-bottom: 10px; margin-bottom: 10px; }
.logo-img-pdf { max-height: 60px; max-width: 200px; object-fit: contain; } 
.datos-orden { text-align: right; }
.datos-orden h3 { margin: 0; text-decoration: underline; font-size: 18px; font-weight: 900; }
.datos-orden p { margin: 2px 0; font-size: 12px; }

/* Filas y Datos */
.fila-pdf { margin-bottom: 10px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; }

/* Caja Producto */
.caja-producto-pdf { border: 2px solid black; padding: 8px; margin-bottom: 8px; text-align: center; background: #f9f9f9; }
.titulo-seccion-pdf { font-size: 10px; font-weight: bold; margin-bottom: 2px; letter-spacing: 1px; }
.producto-nombre-pdf { font-size: 18px; font-weight: 900; }
.producto-sku-pdf { font-size: 12px; margin-top: 2px; }

/* Ficha Técnica */
.ficha-tecnica-pdf { display: flex; border: 2px solid black; margin-bottom: 8px; }
.dato-box-pdf { flex: 1; border-right: 1px solid black; text-align: center; padding: 4px; }
.dato-box-pdf:last-child { border-right: none; }
.dato-box-pdf.doble-ancho-pdf { flex: 2; background: #e8e8e8; }
.label-tech-pdf { display: block; font-size: 9px; font-weight: bold; color: #333; }
.valor-tech-pdf { font-size: 14px; font-weight: bold; margin-top: 2px; display: block; }

/* Receta */
.seccion-receta-pdf { margin-top: 10px; border: 2px solid black; font-size: 14px; }
.titulo-receta-pdf { background: #e0e0e0; padding: 5px; font-weight: 900; text-align: center; border-bottom: 2px solid black; font-size: 14px; }
.tabla-receta-pdf { width: 100%; border-collapse: collapse; }
.tabla-receta-pdf th { border-right: 1px solid black; border-bottom: 2px solid black; padding: 5px; background: #f4f4f4; font-size: 11px; }
.tabla-receta-pdf td { border-right: 1px solid black; padding: 5px; font-size: 12px; border-bottom: 1px solid #ccc; }

/* ✅ CSS ARREGLADO: INPUTS QUE NO SE CORTAN */
.input-print { 
    border: none; 
    background: transparent; 
    font-weight: bold; 
    color: inherit; 
    width: 60px; /* Ancho original mantenido */
    text-align: center; 
    font-size: 12px; 
    padding: 0;
    margin: 0;
    
    /* 🔥 Altura y centrado forzado para evitar recortes */
    height: 20px; 
    line-height: 20px;
    vertical-align: middle;
    display: inline-block;

    /* 🔥 Eliminar flechitas nativas que rompen el layout */
    -moz-appearance: textfield; 
    appearance: none;
}
.input-print::-webkit-outer-spin-button,
.input-print::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
}
.input-print:focus { border-bottom: 1px solid black; outline: none; }

.btn-borrar-insumo { background:none; border:none; color:red; cursor:pointer; font-weight:bold; }

/* Buscador */
.agregar-fila-pdf { padding: 5px; border-top: 1px solid #ccc; display: flex; gap: 5px; align-items: center; justify-content: flex-end; background: #f9f9f9; }
.btn-add-insumo { background:#2ecc71; color:white; border:none; padding:5px 10px; cursor:pointer; }
.buscador-wrapper { position: relative; width: 250px; }
.input-buscador { width: 100%; padding: 6px; border: 1px solid #ccc; border-radius: 4px; }
.lista-resultados { position: absolute; top: 100%; left: 0; right: 0; background: white; border: 1px solid #ccc; max-height: 150px; overflow-y: auto; z-index: 999; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.item-resultado { padding: 5px; border-bottom: 1px solid #eee; cursor: pointer; text-align: left; font-size: 12px; }
.item-resultado:hover { background-color: #f0f0f0; }

/* Pie y Firma */
.fila-lotes-pdf { display: flex; gap: 15px; margin-top: 5px; margin-bottom: 10px; }
.mitad-pdf { flex: 1; }
.recuadro-gigante-pdf { border: 2px solid black; height: 35px; font-size: 20px; display: flex; align-items: center; justify-content: center; margin-top: 2px; font-weight: 900; overflow: hidden; white-space: nowrap; }
.texto-lote-pdf { font-size: 14px; }
.pie-firma-pdf { margin-top: auto; padding-top: 60px; display: flex; justify-content: space-around; }
.linea-firma-pdf { border-top: 2px solid black; width: 40%; text-align: center; font-size: 11px; padding-top: 2px; font-weight: bold; }

/* Marca de Agua y Corte */
.marca-agua { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%) rotate(-30deg); font-size: 50px; color: rgba(0,0,0,0.03); font-weight: 900; border: 5px solid rgba(0,0,0,0.03); padding: 10px 40px; border-radius: 20px; z-index: 0; pointer-events: none; }
.linea-corte-pdf { position: absolute; bottom: -12px; left: 0; width: 100%; text-align: center; font-size: 10px; color: #999; z-index: 10; }
.linea-corte-pdf span { background: white; padding: 0 10px; }

</style>