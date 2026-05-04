<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import JsBarcode from 'jsbarcode';

const logoImg = new URL('../assets/estruplast-logo.png', import.meta.url).href;

const props = defineProps<{
    form: any;
    producto: any;
    cliente: any; 
    receta: any[];
    colorFinal: string;
    densidad: number;
    totalPorcentaje: number;
    materiasPrimas: any[]; 
    ocultarFormula: boolean; 
    tipoSalidaVisual?: string;
}>();

const emit = defineEmits(['add-insumo', 'remove-insumo', 'update-receta']);

const insumoBusquedaTexto = ref(''); 
const insumoExtraPorc = ref<number | ''>('');
const mostrarLista = ref(false); 

const esConsolidadoReal = computed(() => {
    return props.form?.observacion?.includes('MEZCLA CONSOLIDADA') || props.form?.esConsolidado;
});

const codigoLoteVisible = computed(() => {
    if (!props.form?.observacion) return props.form?.id;
    const match = props.form.observacion.match(/\[LOTE: (HC-[^\]]+)\]/);
    return match ? match[1] : props.form?.id;
});

const valorCodigoBarra = computed(() => {
    if (esConsolidadoReal.value) {
        if (!codigoLoteVisible.value) return '';
        return `LOTE-${codigoLoteVisible.value}`;
    }
    if (!props.form?.id) return ''; 
    return `OP-${props.form?.id}`;
});

const generarCodigoDirecto = (texto: string) => {
    if (!texto || texto.includes('undefined')) return '';
    try {
        const canvas = document.createElement("canvas");
        JsBarcode(canvas, texto, {
            format: "CODE128",
            displayValue: true, 
            fontSize: 14,
            height: 40, 
            width: 1.5, 
            margin: 0
        });
        return canvas.toDataURL("image/png");
    } catch (error) {
        console.error("Fallo JsBarcode:", error);
        return '';
    }
};

const cantidadCopias = computed(() => props.ocultarFormula ? 2 : 1);

const kilosNetosExactos = computed(() => {
    return Number(props.form?.kilosTotales) || Number(props.form?.kilosEstimados) || Number(props.form?.kilos) || 0;
});

const pesoBrutoExacto = computed(() => {
    const porcentajeDesperdicio = Number(props.form?.merma) || Number(props.form?.desperdicio) || 0; 
    const resultado = kilosNetosExactos.value * (1 + (porcentajeDesperdicio / 100));
    return isNaN(resultado) ? 0 : resultado;
});

const pesoVisualRedondeado = computed(() => Math.ceil(pesoBrutoExacto.value));

const kilosCabeceraRedondeado = computed(() => {
    if (esConsolidadoReal.value) {
        return Math.round(kilosNetosExactos.value);
    }
    return props.ocultarFormula ? Math.ceil(kilosNetosExactos.value) : Math.ceil(pesoBrutoExacto.value);
});

const ceilKilos = (valor: number, decimales = 3) => {
    const num = Number(valor) || 0;
    const factor = Math.pow(10, decimales);
    return Math.ceil(num * factor) / factor;
};

const densidadReal = computed(() => {
    if (esConsolidadoReal.value) return 0;
    if (props.producto?.pesoEspecifico > 0) return Number(props.producto.pesoEspecifico);
    if (props.form?.producto?.pesoEspecifico > 0) return Number(props.form.producto.pesoEspecifico);
    return Number(props.densidad) || 0;
});

const recetaVisual = computed(() => {
    let lista = JSON.parse(JSON.stringify(props.receta || []));

    if (props.tipoSalidaVisual === 'NATURAL') {
        let porcentajeRemovido = 0;
        let kilosRemovidos = 0;
        const listaLimpia: any[] = [];

        lista.forEach((item: any) => {
            const n = (item.nombreInsumo || item.nombreMateriaPrima || '').toUpperCase();
            const esColor = item.esColor || n.includes('MB') || n.includes('MASTER') || n.includes('COLOR');
            
            if (esColor) {
                porcentajeRemovido += parseFloat(item.cantidad || 0);
                kilosRemovidos += parseFloat(item.kilosFijos || item.cantidadKilos || 0);
            } else {
                listaLimpia.push(item);
            }
        });

        if (listaLimpia.length > 0 && porcentajeRemovido > 0) {
            listaLimpia.sort((a: any, b: any) => (parseFloat(b.cantidad) || 0) - (parseFloat(a.cantidad) || 0));
            const materialPrincipal = listaLimpia.find((i: any) => i.esBase) || listaLimpia[0];

            if (materialPrincipal) {
                materialPrincipal.cantidad = (parseFloat(materialPrincipal.cantidad || 0) + porcentajeRemovido).toFixed(2);
                
                if (materialPrincipal.kilosFijos) {
                    materialPrincipal.kilosFijos = (parseFloat(materialPrincipal.kilosFijos) + kilosRemovidos).toFixed(2);
                } else if (materialPrincipal.cantidadKilos) {
                    materialPrincipal.cantidadKilos = (parseFloat(materialPrincipal.cantidadKilos) + kilosRemovidos).toFixed(2);
                }
            }
        }
        lista = listaLimpia;
    }

    return lista.sort((a: any, b: any) => (parseFloat(b.cantidadKilos || b.cantidad) || 0) - (parseFloat(a.cantidadKilos || a.cantidad) || 0));
});

const obtenerTipoMaterial = (item: any) => {
    if (!item) return '';
    if (item.tipoMaterial && item.tipoMaterial !== 'OTROS') return item.tipoMaterial.toUpperCase();
    const n = (item.nombre || '').toUpperCase();
    if (n.includes('PAI') || n.includes('IMPACTO') || n.includes('A.I.') || n.includes('AI ')) return 'PAI';
    if (n.includes('PEAD') || n.includes('ALTA') || n.includes('HDPE')) return 'PEAD';
    if (n.includes('PEBD') || n.includes('BAJA') || n.includes('LDPE') || n.includes('POLIETILENO')) return 'POLIETILENO';
    if (n.includes('PP') || n.includes('POLIPROPILENO')) return 'PP';
    if (n.includes('ABS')) return 'ABS';
    if (n.includes('FREON') || n.includes('RESISTENTE')) return 'RESISTENTE FREON';
    if (n.includes('BIO')) return 'BIO';
    return '';
};

const sugerenciasFiltradas = computed(() => {
    const texto = insumoBusquedaTexto.value.trim().toUpperCase();
    let lista = props.materiasPrimas || [];
    
    const idClienteActual = Number(props.cliente?.id || props.form?.clienteId || 0);

    lista = lista.filter(mp => {
        const idDuenio = Number(mp.clienteId || mp.ClienteId || 0);
        if (idDuenio !== 0 && idDuenio !== idClienteActual) return false;

        const nombreLimpio = (mp.nombre || '').toUpperCase().trim();

        if (nombreLimpio.includes('BASE')) return false;

        const excluidosExactos = ['ABS','PAI', 'PEAD', 'POLIPROPILENO','POLIETILENO','RESISTENTE AL FREON'];
        if (excluidosExactos.includes(nombreLimpio)) return false;

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

const seleccionarInsumo = (mp: any) => { insumoBusquedaTexto.value = mp.nombre; mostrarLista.value = false; };
const cerrarListaConDelay = () => { setTimeout(() => { mostrarLista.value = false; }, 200); };

const solicitarAgregar = () => {
    if (!insumoBusquedaTexto.value || !insumoExtraPorc.value) return;
    const mpEncontrada = sugerenciasFiltradas.value.find(m => m.nombre === insumoBusquedaTexto.value);
    if (mpEncontrada) {
        emit('add-insumo', { id: mpEncontrada.id, porcentaje: Number(insumoExtraPorc.value) });
        insumoBusquedaTexto.value = ''; insumoExtraPorc.value = ''; mostrarLista.value = false;
    } else {
        alert("⚠️ Seleccione un insumo válido de la lista.");
    }
};

const solicitarQuitar = (item: any) => { 
    const indexReal = props.receta.indexOf(item);
    if (indexReal !== -1) emit('remove-insumo', indexReal); 
};

const obtenerEtiquetaOrigen = (itemReceta: any) => {
    let idDuenioMaterial = 0;
    const idMpBuscado = itemReceta.materiaPrimaId || itemReceta.id;

    if (props.materiasPrimas && props.materiasPrimas.length > 0) {
        const mpReal = props.materiasPrimas.find((m: any) => m.id === idMpBuscado);
        if (mpReal) {
            idDuenioMaterial = Number(mpReal.clienteId || mpReal.ClienteId || 0);
        } else {
            idDuenioMaterial = Number(itemReceta.clienteId || itemReceta.ClienteId || 0);
        }
    } else {
        idDuenioMaterial = Number(itemReceta.clienteId || itemReceta.ClienteId || 0);
    }
    if (idDuenioMaterial === 0) {
        return ''; 
    }
    
    if (props.cliente && idDuenioMaterial === Number(props.cliente.id)) {
        return `(DE ${props.cliente.razonSocial.toUpperCase()})`;
    }
    
    return '(MATERIAL PRESTADO/TERCERO)';
};

const getOrigenMaterial = (r: any) => obtenerEtiquetaOrigen(r);

const fechaHoy = new Date().toLocaleDateString('es-AR', { day: '2-digit', month: '2-digit', year: 'numeric' });

const tituloLimpioParaPDF = computed(() => {
    if (esConsolidadoReal.value) {
        return "MEZCLA MULTIPLE";
    }

    let crudo = props.form?.productoNombre || props.producto?.nombre || '';
    crudo = crudo.trim();
    const upper = crudo.toUpperCase();
    
    const prefijos = ['LAMINADO A FAZON -', 'LAMINADO A FAZON-', 'FAZON -', 'FAZON-', 'FAZON '];
    
    for (const pref of prefijos) {
        if (upper.startsWith(pref)) {
            return crudo.substring(pref.length).trim();
        }
    }
    return crudo;
});

const observacionLimpia = computed(() => {
    if (!props.form?.observacion) return '-';
    let obs = props.form.observacion.replace(/\[LOTE: HC-[^\]]+\]/g, '').trim();
    return obs;
});

const esVerdadero = (valor: any) => {
    if (valor === true || valor === 1) return true;
    if (typeof valor === 'string') {
        const vLimpio = valor.trim().toLowerCase();
        return vLimpio === 'true' || vLimpio === '1' || vLimpio === 'sí' || vLimpio === 'si';
    }
    return false;
};

const verificarCaracteristica = (propMinuscula: string, propMayuscula: string) => {
    if (esVerdadero(props.form?.[propMinuscula]) || esVerdadero(props.form?.[propMayuscula])) return true;
    if (esVerdadero(props.producto?.[propMinuscula]) || esVerdadero(props.producto?.[propMayuscula])) return true;
    
    const subOrdenes = props.form?.ordenes || props.form?.pedidos || props.form?.detalles || props.form?.items || [];
    return subOrdenes.some((o: any) => esVerdadero(o[propMinuscula]) || esVerdadero(o[propMayuscula]));
};

const tieneBrillo = computed(() => verificarCaracteristica('conBrillo', 'ConBrillo'));
const llevaFilm = computed(() => verificarCaracteristica('llevaFilm', 'LlevaFilm'));
const esGofrado = computed(() => verificarCaracteristica('esGofrado', 'EsGofrado'));
const tieneUV = computed(() => verificarCaracteristica('aditivoUV', 'AditivoUV'));

const tipoCorona = computed(() => {
    let val = props.form?.tipoCorona || props.form?.TipoCorona || props.producto?.tipoCorona || props.producto?.TipoCorona;
    
    const validarCorona = (v: any) => {
        if (!v) return false;
        const texto = String(v).trim().toUpperCase();
        return texto !== 'NINGUNO' && texto !== 'FALSE' && texto !== '0' && texto !== 'NULL' && texto !== '';
    };

    if (!validarCorona(val)) {
        const subOrdenes = props.form?.ordenes || props.form?.pedidos || props.form?.detalles || props.form?.items || [];
        const ordenConCorona = subOrdenes.find((o: any) => validarCorona(o.tipoCorona) || validarCorona(o.TipoCorona));
        if (ordenConCorona) val = ordenConCorona.tipoCorona || ordenConCorona.TipoCorona;
    }
    
    return validarCorona(val) ? String(val).toUpperCase() : null;
});

</script>

<template>
  <div id="hoja-de-impresion" class="contenedor-principal-pdf">

    <div v-for="n in cantidadCopias" :key="n" class="pagina-copia" :class="{ 'modo-mitad': cantidadCopias === 2 }">
        <div v-if="cantidadCopias === 2" class="marca-agua">{{ n === 1 ? 'ORIGINAL' : 'DUPLICADO' }}</div>

        <div class="header-pdf">
            <div class="logo-area"><img :src="logoImg" class="logo-central" /></div>
            
            <div class="datos-orden">
                <h3>{{ esConsolidadoReal ? 'HOJA DE CARGA MÚLTIPLE' : (ocultarFormula ? 'ORDEN DE PRODUCCIÓN' : 'HOJA DE CARGA') }}</h3>
                
                <div v-if="esConsolidadoReal" class="lote-mezcla-resaltado">
                    LOTE N°: {{ codigoLoteVisible }} 
                </div>

                <p>FECHA: <strong>{{ fechaHoy }}</strong></p>
                <p>NOTA PEDIDO: <strong>{{ form?.notaPedido || '-' }}</strong></p>
                <p v-if="!esConsolidadoReal">OC CLIENTE: <strong>{{ form?.numeroPedidoCliente || '-' }}</strong></p>
            </div>
        </div>
        
        <div class="fila-pdf" v-if="!esConsolidadoReal">
            <div><strong>CLIENTE:</strong> <span class="dato-relleno">{{ cliente?.razonSocial || form.clienteNombre || 'STOCK / INTERNO' }}</span></div>
        </div>

        <div class="caja-producto-pdf">
            <div class="titulo-seccion-pdf">PRODUCTO A FABRICAR</div>
            <div class="producto-nombre-pdf">{{ tituloLimpioParaPDF }}</div>
            <div v-if="!ocultarFormula && !esConsolidadoReal" class="producto-sku-pdf">CÓDIGO: {{ producto?.codigoSku }}</div>
        </div>

        <div class="ficha-tecnica-pdf">
            <div class="dato-box-pdf" v-if="!esConsolidadoReal"><span class="label-tech-pdf">COLOR</span><span class="valor-tech-pdf">{{ colorFinal || form?.color || form?.Color || '-' }}</span></div>
            <div class="dato-box-pdf" v-if="!esConsolidadoReal">
                <span class="label-tech-pdf">{{ form.esBobina ? 'FORMATO' : 'LARGO' }}</span>
                <span class="valor-tech-pdf">{{ form.esBobina ? 'BOBINA (' + (form.kilosPorBobina || 0) + ' Kg)' : (form.largo || 0) + ' mm' }}</span>
            </div>
            <div class="dato-box-pdf" v-if="!esConsolidadoReal"><span class="label-tech-pdf">ANCHO</span><span class="valor-tech-pdf">{{ form.ancho }} mm</span></div>
            <div class="dato-box-pdf" v-if="!esConsolidadoReal"><span class="label-tech-pdf">ESPESOR</span><span class="valor-tech-pdf">{{ form.espesor }} mm</span></div>
            <div class="dato-box-pdf">
                <span class="label-tech-pdf">{{ ocultarFormula ? 'TOTAL KILOS (NETO)' : 'TOTAL MEZCLA (BRUTO)' }}</span>
                <span class="valor-tech-pdf">{{ kilosCabeceraRedondeado }} kg</span>
            </div>
        </div>

        <!-- 🚀 ESTE BLOQUE AHORA APARECE SIEMPRE QUE HAYA UN ADITIVO -->
        <div class="ficha-tecnica-pdf" style="margin-top: -4px;" v-if="tieneBrillo || llevaFilm || tipoCorona || esGofrado || tieneUV">
            <div class="dato-box-pdf" v-if="tieneBrillo">
                <span class="label-tech-pdf">BRILLO</span>
                <span class="valor-tech-pdf">SÍ</span>
            </div>
            <div class="dato-box-pdf" v-if="llevaFilm">
                <span class="label-tech-pdf">FILM</span>
                <span class="valor-tech-pdf">SÍ</span>
            </div>
            <div class="dato-box-pdf" v-if="tipoCorona">
                <span class="label-tech-pdf">CORONA</span>
                <span class="valor-tech-pdf">{{ tipoCorona }}</span>
            </div>
            <div class="dato-box-pdf" v-if="esGofrado">
                <span class="label-tech-pdf">ACABADO</span>
                <span class="valor-tech-pdf">GOFRADO</span>
            </div>
            <div class="dato-box-pdf" v-if="tieneUV">
                <span class="label-tech-pdf">UV</span>
                <span class="valor-tech-pdf">SÍ</span>
            </div>
        </div>

        <div v-show="!ocultarFormula" class="seccion-receta-pdf">
            <div class="titulo-receta-pdf">
                {{ esConsolidadoReal ? 'RESUMEN DE MEZCLA CONSOLIDADA' : (densidadReal > 0 ? `FÓRMULA DE MEZCLA (Densidad: ${parseFloat(densidadReal.toFixed(3))})` : 'FÓRMULA DE MEZCLA') }}

                <span style="float:right; font-size: 0.8em; color: #333" v-if="!esConsolidadoReal">Total: {{ Number(totalPorcentaje).toFixed(2) }}%</span>
            </div>
            <table class="tabla-receta-pdf">
                <thead>
                    <tr>
                        <th>INSUMO / MATERIA PRIMA</th>
                        <th style="width:100px" v-if="!esConsolidadoReal">% MEZCLA</th>
                        <th style="width:120px; text-align:right;">PESO A CARGAR</th>
                        <th data-html2canvas-ignore="true" style="width:40px" v-if="!esConsolidadoReal"></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-if="recetaVisual.length === 0">
                        <td colspan="4" style="text-align: center; color: #7f8c8d; padding: 15px; font-style: italic;">
                            Aún no hay materiales. Agregue insumos desde el buscador 👇
                        </td>
                    </tr>
                    <template v-for="(r, i) in recetaVisual" :key="i">
                        <tr>
                            <td style="font-weight: 600;">
                                {{ r.nombreInsumo || r.nombreMateriaPrima }}
                                <span v-if="!esConsolidadoReal && obtenerEtiquetaOrigen(r)" style="font-size: 0.85em; font-style: italic; color: #555; margin-left: 5px;">
                                    {{ obtenerEtiquetaOrigen(r) }}
                                </span>
                            </td>
                            <td style="text-align:center; vertical-align: middle;" v-if="!esConsolidadoReal">
                                <div class="porcentaje-celda" v-if="r.esEstearato || (r.nombreInsumo || r.nombreMateriaPrima || '').toUpperCase().includes('ESTEARATO')" style="font-weight: bold; color: #2980b9;">
                                    FIJO
                                </div>
                                <div class="porcentaje-celda" v-else>
                                    {{ Number(r.cantidad).toFixed(2) }} %
                                </div>
                            </td>
                            <td style="text-align:right; font-size: 1.1em;">
                                <strong v-if="r.esEstearato || (r.nombreInsumo || r.nombreMateriaPrima || '').toUpperCase().includes('ESTEARATO')" style="color: #2980b9;">
                                    {{ esConsolidadoReal 
                                        ? parseFloat(r.cantidadKilos || r.CantidadKilos || r.cantidad || 0).toFixed(2) 
                                        : parseFloat(r.kilosFijos || r.cantidad || 0).toFixed(2) 
                                    }} kg
                                </strong>
                                <strong v-else>
                                    {{ esConsolidadoReal 
                                        ? parseFloat(r.cantidadKilos || r.CantidadKilos || r.cantidad || 0).toFixed(2) 
                                        : ceilKilos((pesoBrutoExacto * (parseFloat(r.cantidad?.toString()) || 0)) / 100).toFixed(2) 
                                    }} kg
                                </strong>
                            </td>
                            <td data-html2canvas-ignore="true" v-if="!esConsolidadoReal" style="text-align:center;">
                                <button v-if="!r.esEstearato" @click="solicitarQuitar(r)" class="btn-borrar-insumo" title="Quitar insumo">❌</button>
                            </td>
                        </tr>
                    </template>
                </tbody>
            </table>

            <div class="agregar-fila-pdf" data-html2canvas-ignore="true" v-if="!esConsolidadoReal">
                <div class="buscador-wrapper">
                    <input type="text" v-model="insumoBusquedaTexto" @focus="mostrarLista = true" @blur="cerrarListaConDelay" class="input-buscador" placeholder="Buscar materia prima..." />
                    <div class="lista-resultados" v-if="mostrarLista && sugerenciasFiltradas.length > 0">
                        <div v-for="mp in sugerenciasFiltradas" :key="mp.id" class="item-resultado" @click="seleccionarInsumo(mp)">
                            <span class="nombre-insumo-lista">{{ mp.nombre }}</span>
                            
                            <span v-if="(mp.clienteId || mp.ClienteId) > 0" class="badge-mini-cliente">
                                👤 {{ cliente?.razonSocial || 'CLIENTE' }}
                            </span>
                            <span v-else class="badge-mini-propio">🏢 PROPIO</span>
                        </div>
                    </div>
                </div>
                <input type="number" v-model="insumoExtraPorc" placeholder="%" style="width: 60px; padding: 6px; border: 1px solid #ccc; border-radius: 4px; margin-left: 5px;" />
                <button class="btn-add-insumo" @click="solicitarAgregar" style="margin-left: 5px;">AGREGAR</button>
            </div>
        </div>

        <div class="fila-lotes-pdf">
            <div class="mitad-pdf" v-if="!esConsolidadoReal">
                <strong>CANTIDAD (UNIDADES):</strong>
                <div class="recuadro-gigante-pdf">{{ form.cantidad }}</div>
            </div>
            <div class="mitad-pdf" :style="esConsolidadoReal ? 'width: 100%;' : ''">
                <strong>OBSERVACIONES / DETALLES DE LOTE:</strong>
                <div class="recuadro-gigante-pdf texto-lote-pdf">{{ observacionLimpia }}</div>
            </div>
        </div>

        <div v-if="!ocultarFormula" class="seccion-totales-manuales">
            <div class="titulo-totales-manuales">REGISTRO DE CARGA REAL (KG)</div>
            <div class="contenedor-columnas-totales">
                <div class="columna-total">
                    <span class="etiqueta-manual">TOTAL MOLIDO:</span>
                    <div class="linea-llenado"></div>
                </div>
                <div class="columna-total">
                    <span class="etiqueta-manual">TOTAL MASTER:</span>
                    <div class="linea-llenado"></div>
                </div>
                <div class="columna-total">
                    <span class="etiqueta-manual">TOTAL VIRGEN:</span>
                    <div class="linea-llenado"></div>
                </div>
            </div>
        </div>
        
        <div class="pie-firma-pdf">
            <div class="caja-firmas-operarios">
                <div class="opcion-firma"><div class="box-firma"></div> Acuña/Rodriguez</div>
                <div class="opcion-firma"><div class="box-firma"></div> Saavedra/Ayala</div>
                <div class="opcion-firma" v-if="!ocultarFormula"><div class="box-firma"></div> Marcori</div>
            </div>
            
            <div class="caja-firma-responsable">
                <div class="linea-firma-pdf">Firma Responsable Calidad</div>
            </div>

            <div class="barcode-impresion">
                <img 
                    v-if="ocultarFormula && valorCodigoBarra && !valorCodigoBarra.includes('undefined')" 
                    :src="generarCodigoDirecto(valorCodigoBarra)" 
                    alt="Código de Barras OP" 
                />
            </div>
        </div>
        <div v-if="cantidadCopias === 2 && n === 1" class="linea-corte-pdf"><span>✂️ CORTAR AQUÍ</span></div>
    </div>
  </div>
</template>

<style>
.contenedor-principal-pdf { background: white; width: 209mm; min-height: 290mm; padding: 0; box-sizing: border-box; color: black; font-family: Arial, sans-serif; position: relative; }
.pagina-copia { padding: 15mm; box-sizing: border-box; width: 100%; height: 290mm; display: flex; flex-direction: column; position: relative; }
.pagina-copia.modo-mitad { height: 145mm; padding: 5mm 15mm; border-bottom: 1px dashed #999; display: block; }
.header-pdf { display: flex; justify-content: space-between; align-items: center; border-bottom: 2px solid black; padding-bottom: 10px; margin-bottom: 10px; }
.logo-central { max-height: 50px; max-width: 180px; }
.datos-orden { text-align: right; }
.datos-orden h3 { margin: 0; text-decoration: underline; font-size: 18px; font-weight: 900; }
.datos-orden p { margin: 2px 0; font-size: 12px; }
.lote-mezcla-resaltado { font-size: 16px; font-weight: 900; border: 2px solid black; padding: 3px 6px; margin: 4px 0; display: inline-block; background-color: #f0f0f0; }
.fila-pdf { margin-bottom: 10px; font-size: 14px; border-bottom: 1px solid #eee; padding-bottom: 5px; }
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; }
.caja-producto-pdf { border: 2px solid black; padding: 8px; margin-bottom: 8px; text-align: center; background: #f9f9f9; }
.titulo-seccion-pdf { font-size: 10px; font-weight: bold; margin-bottom: 2px; letter-spacing: 1px; }
.producto-nombre-pdf { font-size: 18px; font-weight: 900; }
.producto-sku-pdf { font-size: 12px; margin-top: 2px; }
.ficha-tecnica-pdf { display: flex; border: 2px solid black; margin-bottom: 8px; }
.dato-box-pdf { flex: 1; border-right: 1px solid black; text-align: center; padding: 4px; }
.dato-box-pdf:last-child { border-right: none; }
.label-tech-pdf { display: block; font-size: 9px; font-weight: bold; color: #333; }
.valor-tech-pdf { font-size: 14px; font-weight: bold; margin-top: 2px; display: block; }
.seccion-receta-pdf { margin-top: 10px; border: 2px solid black; font-size: 14px; }
.titulo-receta-pdf { background: #e0e0e0; padding: 5px; font-weight: 900; text-align: center; border-bottom: 2px solid black; font-size: 14px; }
.tabla-receta-pdf { width: 100%; border-collapse: collapse; }
.tabla-receta-pdf th { border-right: 1px solid black; border-bottom: 2px solid black; padding: 5px; background: #f4f4f4; font-size: 11px; }
.tabla-receta-pdf td { border-right: 1px solid black; padding: 5px; font-size: 12px; border-bottom: 1px solid #ccc; }
.fila-lotes-pdf { display: flex; gap: 15px; margin-top: 5px; margin-bottom: 10px; }
.mitad-pdf { flex: 1; }
.recuadro-gigante-pdf { border: 2px solid black; height: 35px; font-size: 20px; display: flex; align-items: center; justify-content: center; margin-top: 2px; font-weight: 900; overflow: hidden; white-space: nowrap; }
.texto-lote-pdf { font-size: 14px; }

.pie-firma-pdf { margin-top: auto; padding-top: 15px; display: flex; justify-content: space-between; align-items: flex-end; }
.caja-firmas-operarios { width: 33%; display: flex; flex-direction: column; gap: 8px; }
.opcion-firma { display: flex; align-items: center; font-size: 12px; font-weight: bold; }
.box-firma { width: 16px; height: 16px; border: 2px solid black; margin-right: 8px; display: inline-block; background-color: white; }

.caja-firma-responsable { width: 33%; display: flex; justify-content: center; padding-bottom: 5px; }
.linea-firma-pdf { border-top: 2px solid black; width: 100%; text-align: center; font-size: 11px; padding-top: 2px; font-weight: bold; }

.barcode-impresion { width: 33%; display: flex; justify-content: flex-end; align-items: flex-end; }
.barcode-impresion img { max-height: 55px; max-width: 100%; object-fit: contain; }

.seccion-totales-manuales { margin-top: 10px; border: 2px solid black; background-color: #fff; }
.titulo-totales-manuales { background-color: #e0e0e0; font-size: 9px; font-weight: 900; text-align: center; border-bottom: 2px solid black; padding: 2px; }
.contenedor-columnas-totales { display: flex; justify-content: space-around; padding: 10px 5px; }
.columna-total { flex: 1; display: flex; flex-direction: column; align-items: center; gap: 5px; }
.etiqueta-manual { font-size: 11px; font-weight: 900; }
.linea-llenado { width: 80%; border-bottom: 2px solid black; height: 20px; }

.marca-agua { position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%) rotate(-30deg); font-size: 50px; color: rgba(0,0,0,0.03); font-weight: 900; border: 5px solid rgba(0,0,0,0.03); padding: 10px 40px; border-radius: 20px; z-index: 0; pointer-events: none; }
.linea-corte-pdf { position: absolute; bottom: -12px; left: 0; width: 100%; text-align: center; font-size: 10px; color: #999; z-index: 10; }
.linea-corte-pdf span { background: white; padding: 0 10px; }
.agregar-fila-pdf { padding: 5px; border-top: 1px solid #ccc; display: flex; gap: 5px; align-items: center; justify-content: flex-end; background: #f9f9f9; }
.btn-add-insumo { background:#2ecc71; color:white; border:none; padding:5px 10px; cursor:pointer; font-weight: bold; border-radius: 4px; }
.btn-borrar-insumo { background: none; border: none; cursor: pointer; font-size: 14px; padding: 2px; }
.buscador-wrapper { position: relative; width: 400px; }
.input-buscador { width: 100%; padding: 6px; border: 1px solid #ccc; border-radius: 4px; }
.lista-resultados { position: absolute; bottom: 100%; left: 0; right: 0; background: white; border: 1px solid #ccc; max-height: 150px; overflow-y: auto; z-index: 999; box-shadow: 0 -4px 6px rgba(0,0,0,0.1); margin-bottom: 2px; border-radius: 4px; }

.item-resultado { padding: 8px; border-bottom: 1px solid #eee; cursor: pointer; font-size: 12px; font-weight: 600; display: flex; justify-content: space-between; align-items: center; }
.item-resultado:hover { background-color: #f1f2f6; }
.nombre-insumo-lista { white-space: nowrap; overflow: hidden; text-overflow: ellipsis; max-width: 70%; text-align: left; }
.badge-mini-cliente { background: #f39c12; color: white; padding: 2px 6px; border-radius: 4px; font-size: 9px; letter-spacing: 0.5px; }
.badge-mini-propio { background: #3498db; color: white; padding: 2px 6px; border-radius: 4px; font-size: 9px; letter-spacing: 0.5px; }
</style>