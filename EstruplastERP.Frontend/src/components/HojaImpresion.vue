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

// --- LÓGICA DE PESOS (TU CÓDIGO INTACTO) ---
const pesoBrutoExacto = computed(() => {
    const kilosNetos = Number(props.form?.kilosTotales) || 0;
    const porcentajeDesperdicio = Number(props.form?.merma) || 0; 
    const resultado = kilosNetos * (1 + (porcentajeDesperdicio / 100));
    return isNaN(resultado) ? 0 : resultado;
});

const pesoVisualRedondeado = computed(() => Math.ceil(pesoBrutoExacto.value));

// --- FILTRO DE INSUMOS (TU CÓDIGO INTACTO) ---
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
                <img src="." alt="Logo" class="logo-img-pdf">
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
                            <td style="text-align:center">
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
/* Modo Mitad (cuando hay 2 copias en una hoja) */
.pagina-copia.modo-mitad { 
    height: 145mm; padding: 5mm 15mm; border-bottom: 1px dashed #999; 
    display: block; /* Importante para que no estire */
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

/* Ficha Técnica (Grid Simulado con Flex) */
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

/* Inputs y Botones (Para que se vean bien en pantalla e impreso) */
.input-print { border: none; background: transparent; font-weight: bold; color: inherit; width: 50px; text-align: center; font-size: 12px; }
.input-print:focus { border-bottom: 1px solid black; outline: none; }
.btn-borrar-insumo { background:none; border:none; color:red; cursor:pointer; font-weight:bold; }

/* Buscador (Ignorado en PDF pero estilizado para pantalla) */
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

/* AQUÍ EL CAMBIO PARA MÁS ESPACIO DE FIRMA */
.pie-firma-pdf { margin-top: auto; padding-top: 60px; display: flex; justify-content: space-around; }

.linea-firma-pdf { border-top: 2px solid black; width: 40%; text-align: center; font-size: 11px; padding-top: 2px; font-weight: bold; }

/* Marca de Agua y Corte */
.marca-agua { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%) rotate(-30deg); font-size: 50px; color: rgba(0,0,0,0.03); font-weight: 900; border: 5px solid rgba(0,0,0,0.03); padding: 10px 40px; border-radius: 20px; z-index: 0; pointer-events: none; }
.linea-corte-pdf { position: absolute; bottom: -12px; left: 0; width: 100%; text-align: center; font-size: 10px; color: #999; z-index: 10; }
.linea-corte-pdf span { background: white; padding: 0 10px; }

</style>