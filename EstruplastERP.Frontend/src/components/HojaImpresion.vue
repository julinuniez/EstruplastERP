<script setup lang="ts">
import { ref, computed } from 'vue'
import logoImg from '@/assets/estruplast-logo.png';

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

const insumoBusquedaTexto = ref(''); 
const insumoExtraPorc = ref<number | ''>('');
const mostrarLista = ref(false); 

const cantidadCopias = computed(() => props.ocultarFormula ? 2 : 1);

const pesoBrutoExacto = computed(() => {
    const kilosNetos = Number(props.form?.kilosTotales) || 0;
    const porcentajeDesperdicio = Number(props.form?.merma) || 0; 
    const resultado = kilosNetos * (1 + (porcentajeDesperdicio / 100));
    return isNaN(resultado) ? 0 : resultado;
});

const pesoVisualRedondeado = computed(() => Math.ceil(pesoBrutoExacto.value));

const recetaVisual = computed(() => {
    let lista = [...props.receta];
    lista = lista.filter(item => {
        const cant = parseFloat(item.cantidad);
        return cant > 0;
    });
    lista.sort((a, b) => {
        const cantA = parseFloat(a.cantidad) || 0;
        const cantB = parseFloat(b.cantidad) || 0;
        return cantB - cantA; 
    });
    return lista;
});

const sugerenciasFiltradas = computed(() => {
    const texto = insumoBusquedaTexto.value.trim().toUpperCase();
    let lista = props.materiasPrimas || [];

    const idClienteOrden = props.cliente ? props.cliente.id : 0;
    const esClienteFazon = props.cliente && props.cliente.esFazon === true;

    let materialRequerido = '';
    const nombreProd = (props.producto?.nombre || '').toUpperCase();
    
    if (nombreProd.includes('PAI') || nombreProd.includes('IMPACTO') || nombreProd.includes('A.I.') || nombreProd.includes('AI ')) materialRequerido = 'PAI';
    else if (nombreProd.includes('PP') || nombreProd.includes('POLIPROPILENO')) materialRequerido = 'PP';
    else if (nombreProd.includes('PEAD') || nombreProd.includes('ALTA')) materialRequerido = 'PEAD';
    else if (nombreProd.includes('PEBD') || nombreProd.includes('BAJA')) materialRequerido = 'PEBD';

    lista = lista.filter(mp => {
        const nombreMP = (mp.nombre || '').toUpperCase();
        const rubroMP = (mp.rubro || '').toUpperCase();
        const idDuenioMaterial = mp.clienteId || 0;
        
        // --- 1. FILTRO DE CALIDAD ---
        if (nombreMP.includes('SCRAP') || nombreMP.includes('SUCIO')) return false;
        if (mp.esScrap) {
            const esProcesado = nombreMP.includes('MOLIDO') || nombreMP.includes('RECUPERADO');
            if (!esProcesado) return false;
        }

        // --- 2. LÓGICA DE CLIENTE ---
        const esDelClienteActual = (idClienteOrden > 0 && idDuenioMaterial === idClienteOrden);

        if (esDelClienteActual) {
            if (materialRequerido) {
                if (materialRequerido === 'PP' && (nombreMP.includes('PAI') || nombreMP.includes('IMPACTO'))) return false;
                if (materialRequerido === 'PAI' && (nombreMP.includes('PP') || nombreMP.includes('POLIPROPILENO'))) return false;
            }
            return true;
        }

        // --- 3. FILTROS GENERALES ---
        if (idDuenioMaterial > 0 && idDuenioMaterial !== idClienteOrden) return false;
        if (nombreMP.includes('BASE') && !nombreMP.includes('ALTA')) return false; 
        if (nombreMP.includes('GENERICO') || nombreMP.includes('GENÉRICO')) return false;
        if (mp.id >= 990 && mp.id <= 999) return false;

        if (materialRequerido) {
            const esAditivo = rubroMP.includes('ADITIVO') || rubroMP.includes('MASTER') || nombreMP.includes('MASTER') || nombreMP.includes('COLOR') || nombreMP.includes('CARGA');
            if (!esAditivo) {
                if (materialRequerido === 'PAI' && (nombreMP.includes('PP') || nombreMP.includes('PEAD') || nombreMP.includes('POLIETILENO'))) return false;
                if (materialRequerido === 'PP' && (nombreMP.includes('PAI') || nombreMP.includes('IMPACTO') || nombreMP.includes('PEAD'))) return false;
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
    
    return [...lista].sort((a, b) => {
        const aEsCliente = a.clienteId === idClienteOrden ? 1 : 0;
        const bEsCliente = b.clienteId === idClienteOrden ? 1 : 0;
        if (aEsCliente !== bEsCliente) return bEsCliente - aEsCliente; 
        return a.nombre.localeCompare(b.nombre);
    });
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

const solicitarQuitar = (item: any) => { 
    const indexReal = props.receta.indexOf(item);
    if (indexReal !== -1) {
        emit('remove-insumo', indexReal); 
    }
};

const fechaHoy = new Date().toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });
</script>

<template>
  <div id="hoja-de-impresion" class="contenedor-principal-pdf">
    
    <div v-for="n in cantidadCopias" :key="n" class="pagina-copia" :class="{ 'modo-mitad': cantidadCopias === 2 }">
        
        <div v-if="cantidadCopias === 2" class="marca-agua">{{ n === 1 ? 'ORIGINAL' : 'DUPLICADO' }}</div>

        <div class="header-pdf">
            <div class="logo-area">
                <img :src="logoImg" alt="Logo Empresa" class="logo-central" />
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

        <div v-if="recetaVisual.length > 0" v-show="!ocultarFormula" class="seccion-receta-pdf">
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
                    <template v-for="(r, i) in recetaVisual" :key="i">
                        <tr>
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
                                <button @click="solicitarQuitar(r)" class="btn-borrar-insumo">X</button>
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
                            <div style="font-size:10px; color:#666;">
                                <span v-if="mp.clienteId" style="color:#8e44ad; font-weight:bold;">★ PROPIO (CLIENTE)</span>
                                <span v-else>{{ mp.rubro }}</span>
                            </div>
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
/* ... MISMOS ESTILOS DE SIEMPRE ... */
.contenedor-principal-pdf { background: white; width: 209mm; min-height: 290mm; padding: 0; box-sizing: border-box; color: black; font-family: Arial, sans-serif; position: relative; }
.pagina-copia { padding: 15mm; box-sizing: border-box; width: 100%; height: 290mm; display: flex; flex-direction: column; position: relative; }
.pagina-copia.modo-mitad { height: 145mm; padding: 5mm 15mm; border-bottom: 1px dashed #999; display: block; }
.modo-mitad .header-pdf { margin-bottom: 5px; padding-bottom: 5px; }
.modo-mitad .producto-nombre-pdf { font-size: 16px; }
.modo-mitad .recuadro-gigante-pdf { height: 25px; font-size: 16px; }

.header-pdf { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid black; padding-bottom: 10px; margin-bottom: 10px; }
.logo-central { max-height: 50px; max-width: 180px; }
.datos-orden { text-align: right; }
.datos-orden h3 { margin: 0; text-decoration: underline; font-size: 18px; font-weight: 900; }
.datos-orden p { margin: 2px 0; font-size: 12px; }

.fila-pdf { margin-bottom: 10px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; }

.caja-producto-pdf { border: 2px solid black; padding: 8px; margin-bottom: 8px; text-align: center; background: #f9f9f9; }
.titulo-seccion-pdf { font-size: 10px; font-weight: bold; margin-bottom: 2px; letter-spacing: 1px; }
.producto-nombre-pdf { font-size: 18px; font-weight: 900; }
.producto-sku-pdf { font-size: 12px; margin-top: 2px; }

.ficha-tecnica-pdf { display: flex; border: 2px solid black; margin-bottom: 8px; }
.dato-box-pdf { flex: 1; border-right: 1px solid black; text-align: center; padding: 4px; }
.dato-box-pdf:last-child { border-right: none; }
.dato-box-pdf.doble-ancho-pdf { flex: 2; background: #e8e8e8; }
.label-tech-pdf { display: block; font-size: 9px; font-weight: bold; color: #333; }
.valor-tech-pdf { font-size: 14px; font-weight: bold; margin-top: 2px; display: block; }

.seccion-receta-pdf { margin-top: 10px; border: 2px solid black; font-size: 14px; }
.titulo-receta-pdf { background: #e0e0e0; padding: 5px; font-weight: 900; text-align: center; border-bottom: 2px solid black; font-size: 14px; }
.tabla-receta-pdf { width: 100%; border-collapse: collapse; }
.tabla-receta-pdf th { border-right: 1px solid black; border-bottom: 2px solid black; padding: 5px; background: #f4f4f4; font-size: 11px; }
.tabla-receta-pdf td { border-right: 1px solid black; padding: 5px; font-size: 12px; border-bottom: 1px solid #ccc; }

.input-print { border: none; background: transparent; font-weight: bold; color: inherit; width: 60px; text-align: center; font-size: 12px; padding: 0; margin: 0; height: 20px; line-height: 20px; vertical-align: middle; display: inline-block; -moz-appearance: textfield; appearance: none;}
.input-print::-webkit-outer-spin-button, .input-print::-webkit-inner-spin-button { -webkit-appearance: none; margin: 0; }
.input-print:focus { border-bottom: 1px solid black; outline: none; }

.btn-borrar-insumo { background:none; border:none; color:red; cursor:pointer; font-weight:bold; }

.agregar-fila-pdf { padding: 5px; border-top: 1px solid #ccc; display: flex; gap: 5px; align-items: center; justify-content: flex-end; background: #f9f9f9; }
.btn-add-insumo { background:#2ecc71; color:white; border:none; padding:5px 10px; cursor:pointer; }
.buscador-wrapper { position: relative; width: 250px; }
.input-buscador { width: 100%; padding: 6px; border: 1px solid #ccc; border-radius: 4px; }
.lista-resultados { position: absolute; top: 100%; left: 0; right: 0; background: white; border: 1px solid #ccc; max-height: 150px; overflow-y: auto; z-index: 999; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }
.item-resultado { padding: 5px; border-bottom: 1px solid #eee; cursor: pointer; text-align: left; font-size: 12px; }
.item-resultado:hover { background-color: #f0f0f0; }

.fila-lotes-pdf { display: flex; gap: 15px; margin-top: 5px; margin-bottom: 10px; }
.mitad-pdf { flex: 1; }
.recuadro-gigante-pdf { border: 2px solid black; height: 35px; font-size: 20px; display: flex; align-items: center; justify-content: center; margin-top: 2px; font-weight: 900; overflow: hidden; white-space: nowrap; }
.texto-lote-pdf { font-size: 14px; }
.pie-firma-pdf { margin-top: auto; padding-top: 60px; display: flex; justify-content: space-around; }
.linea-firma-pdf { border-top: 2px solid black; width: 40%; text-align: center; font-size: 11px; padding-top: 2px; font-weight: bold; }

.marca-agua { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%) rotate(-30deg); font-size: 50px; color: rgba(0,0,0,0.03); font-weight: 900; border: 5px solid rgba(0,0,0,0.03); padding: 10px 40px; border-radius: 20px; z-index: 0; pointer-events: none; }
.linea-corte-pdf { position: absolute; bottom: -12px; left: 0; width: 100%; text-align: center; font-size: 10px; color: #999; z-index: 10; }
.linea-corte-pdf span { background: white; padding: 0 10px; }
</style>