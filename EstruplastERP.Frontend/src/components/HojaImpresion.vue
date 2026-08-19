<script setup lang="ts">
import { ref, computed } from 'vue'
// @ts-ignore
import JsBarcode from 'jsbarcode/dist/JsBarcode.all.min.js';

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
const insumoExtraExtrusora = ref('UNICA'); // 🚀 NUEVO: Destino para agregados manuales
const mostrarLista = ref(false); 

const esConsolidadoReal = computed(() => {
    const obs = String(props.form?.observacion || '').toUpperCase();
    return props.form?.esConsolidado === true || 
           props.form?.esConsolidado === 'true' || 
           obs.includes('MEZCLA CONSOLIDADA') || 
           obs.includes('MEZCLA MÚLTIPLE') ||
           props.form?.productoNombre === 'MEZCLA CONSOLIDADA';
});

const esCargaSimple = computed(() => {
    return (props.form?.observacion || '').includes('[Grupo: HC-S');
});

const modoCargaLimpia = computed(() => esConsolidadoReal.value || esCargaSimple.value);

const codigoLoteVisible = computed(() => {
    const obs = props.form?.observacion || '';
    const match = obs.match(/\[Grupo: (HC-[^\]]+)\]/);
    if (match) return match[1];

    const idLote = props.form?.id || props.form?.Id;
    if (idLote && !String(idLote).toLowerCase().includes('undefined')) {
        return idLote;
    }
    
    return 'MÚLTIPLE';
});

const valorCodigoBarra = computed(() => {
    if (modoCargaLimpia.value) {
        if (!codigoLoteVisible.value || codigoLoteVisible.value === 'MÚLTIPLE') return '';
        return `LOTE-${codigoLoteVisible.value}`;
    }
    if (!props.form?.id) return ''; 
    return `OP-${props.form?.id}`;
});

const generarCodigoDirecto = (texto: string) => {
    if (!texto || texto.includes('undefined')) return '';
    try {
        const canvas = document.createElement("canvas");
        (window as any).JsBarcode(canvas, texto, {
            format: "CODE128", displayValue: true, fontSize: 14, height: 40, width: 1.5, margin: 0
        });
        return canvas.toDataURL("image/png");
    } catch (error) {
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

const kilosCabeceraRedondeado = computed(() => {
    if (props.ocultarFormula) {
        return Math.ceil(kilosNetosExactos.value);
    }
    let sumaFisica = 0;
    recetaVisual.value.forEach((r: any) => {
        if (esConsolidadoReal.value) {
            sumaFisica += Number(r.cantidadKilos || r.CantidadKilos || r.real || r.kilos || 0);
        } else {
            sumaFisica += r.kilosFijos 
                ? Number(r.kilosFijos) 
                : ceilKilos((pesoBrutoExacto.value * (Number(r.cantidad) || 0)) / 100);
        }
    });
    return Math.round(sumaFisica); 
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

const obtenerEtiquetaOrigen = (itemReceta: any) => {
    const idDuenio = Number(itemReceta.clienteId || itemReceta.ClienteId || 0);
    const nombreDuenio = itemReceta.clienteNombre || itemReceta.ClienteNombre || '';
    if (idDuenio <= 1) return '';
    if (nombreDuenio && nombreDuenio.trim() !== '') return `(DE ${nombreDuenio.toUpperCase()})`;
    if (props.cliente && Number(props.cliente.id) === idDuenio) return `(DE ${String(props.cliente.razonSocial || '').toUpperCase()})`;
    return '';
};

const esInsumoFijo = (r: any) => {
    if (!r) return false;
    if (r.esEstearato) return true;
    if (r.kilosFijos !== undefined && r.kilosFijos !== null) return true;
    
    const n = String(r.nombreInsumo || r.nombreMateriaPrima || '').toUpperCase();
    if (n.includes('ESTEARATO') || n.includes('BRILLO') || n.includes('CRISTAL') || 
        n.includes('777') || n.includes('555') || n.includes('UV') || n.includes('CAUCHO')) return true;
    return false;
};

const notasPedidoVisibles = computed(() => {
    try {
        let notas: string[] = [];
        const nP = props.form?.notaPedido || props.form?.NotaPedido;
        if (nP && !String(nP).toLowerCase().includes('undefined')) notas.push(String(nP));

        const subOrdenes = props.form?.ordenes || props.form?.pedidos || props.form?.detalles || props.form?.items || [];
        if (subOrdenes && subOrdenes.length > 0) {
            subOrdenes.forEach((o: any) => {
                const n = o.notaPedido || o.NotaPedido;
                if (n && !String(n).toLowerCase().includes('undefined') && !String(n).toLowerCase().includes('null')) {
                    notas.push(String(n));
                } else if (o.id && !String(o.id).toLowerCase().includes('undefined')) {
                    notas.push(String(o.id)); 
                }
            });
        }

        if (notas.length === 0) {
            const idPrincipal = props.form?.id || props.form?.Id;
            if (idPrincipal && !String(idPrincipal).toLowerCase().includes('undefined')) return String(idPrincipal);
            return '-';
        }

        const crudo = notas.join(' | ');
        const arrayLimpio = crudo.replace(/undefined/gi, '').replace(/null/gi, '').split(/[|,]/).map(s => s.trim()).filter(s => s !== '');
        const finales = [...new Set(arrayLimpio)];
        return finales.length > 0 ? finales.join(' | ') : '-';
    } catch (e) {
        return '-';
    }
});

const insumosParaImprimir = computed(() => {
    if (props.receta && props.receta.length > 0) return props.receta;
    if (props.form?.consumos && props.form.consumos.length > 0) return props.form.consumos;
    if (props.form?.receta && props.form.receta.length > 0) return props.form.receta;
    return [];
});

const recetaVisual = computed(() => {
    let lista = JSON.parse(JSON.stringify(insumosParaImprimir.value));

    if (esConsolidadoReal.value) {
        const map = new Map<string, any>();

        lista.forEach((item: any) => {
            const idMp = Number(item.materiaPrimaId || item.MateriaPrimaId || item.id);
            if (!idMp) return; 

            const nombre = item.nombreInsumo || item.nombreMateriaPrima || item.NombreMateriaPrima || item.nombre || 'Insumo';
            const kilos = Number(item.real !== undefined ? item.real : (item.cantidadKilos || item.CantidadKilos || item.kilos || item.cantidad || 0));
            const fijos = Number(item.kilosFijos || 0);
            
            // 🚀 RESCATAMOS EL DESTINO (Por defecto UNICA)
            const destino = item.extrusoraDestino || item.ExtrusoraDestino || 'UNICA';
            
            // Agrupamos por ID de material y por Destino
            const key = `${idMp}-${destino}`; 

            if (!map.has(key)) {
                map.set(key, {
                    ...item, 
                    materiaPrimaId: idMp,
                    nombreInsumo: nombre,
                    cantidadKilos: kilos, 
                    kilosFijos: fijos,
                    extrusoraDestino: destino,
                    esEstearato: item.esEstearato || nombre.toUpperCase().includes('ESTEARATO'),
                    esColor: item.esColor || nombre.toUpperCase().includes('MB') || nombre.toUpperCase().includes('MASTER') || nombre.toUpperCase().includes('COLOR'),
                    esBase: item.esBase || false
                });
            } else {
                const agrupado = map.get(key);
                agrupado.cantidadKilos += kilos;
                agrupado.kilosFijos += fijos;
            }
        });

        lista = Array.from(map.values());
    } else {
        // Aseguramos que los no consolidados tengan la variable seteada
        lista.forEach((item: any) => {
            item.extrusoraDestino = item.extrusoraDestino || item.ExtrusoraDestino || 'UNICA';
        });
    }

    if (props.tipoSalidaVisual === 'NATURAL') {
        let porcentajeRemovido = 0;
        let kilosRemovidos = 0;
        const listaLimpia: any[] = [];

        lista.forEach((item: any) => {
            const n = (item.nombreInsumo || item.nombreMateriaPrima || item.NombreMateriaPrima || '').toUpperCase();
            const esColor = item.esColor || n.includes('MB') || n.includes('MASTER') || n.includes('COLOR');
            
            if (esColor) {
                porcentajeRemovido += Number(item.cantidad || 0);
                kilosRemovidos += Number(item.kilosFijos || item.cantidadKilos || item.CantidadKilos || item.kilos || item.real || item.cantidad || 0);
            } else {
                listaLimpia.push(item);
            }
        });

        if (listaLimpia.length > 0 && kilosRemovidos > 0) {
            listaLimpia.sort((a: any, b: any) => Number(b.cantidadKilos || b.CantidadKilos || b.real || 0) - Number(a.cantidadKilos || a.CantidadKilos || a.real || 0));
            const materialPrincipal = listaLimpia.find((i: any) => i.esBase) || listaLimpia[0];

            if (materialPrincipal) {
                materialPrincipal.cantidad = (Number(materialPrincipal.cantidad || 0) + porcentajeRemovido).toFixed(2);
                
                if (Number(materialPrincipal.kilosFijos) > 0) {
                    materialPrincipal.kilosFijos = Number(materialPrincipal.kilosFijos) + kilosRemovidos;
                } else {
                    materialPrincipal.cantidadKilos = Number(materialPrincipal.cantidadKilos || materialPrincipal.CantidadKilos || materialPrincipal.real || materialPrincipal.cantidad || 0) + kilosRemovidos;
                }
            }
        }
        lista = listaLimpia;
    }

    return lista.sort((a: any, b: any) => {
        const aEsFijo = esInsumoFijo(a) ? 1 : 0;
        const bEsFijo = esInsumoFijo(b) ? 1 : 0;
        if (aEsFijo !== bEsFijo) return aEsFijo - bEsFijo;

        const cantidadA = Number(a.cantidadKilos || a.CantidadKilos || a.real || a.cantidad || 0);
        const cantidadB = Number(b.cantidadKilos || b.CantidadKilos || b.real || b.cantidad || 0);
        return cantidadB - cantidadA;
    });
});

// 🚀 CEREBRO DE AGRUPACIÓN POR TOLVAS PARA EL PDF
const gruposReceta = computed(() => {
    const grupos = {
        A: { titulo: 'EXTRUSORA A (Capa)', items: [] as any[] },
        B: { titulo: 'EXTRUSORA B (Masa)', items: [] as any[] },
        UNICA: { titulo: 'MEZCLA GENERAL', items: [] as any[] }
    };

    recetaVisual.value.forEach((r: any) => {
        const dest = String(r.extrusoraDestino || 'UNICA').toUpperCase();
        if (dest === 'A') grupos.A.items.push(r);
        else if (dest === 'B') grupos.B.items.push(r);
        else grupos.UNICA.items.push(r);
    });

    const tieneSeparacion = grupos.A.items.length > 0 || grupos.B.items.length > 0;
    
    // Si no hay separación A/B, metemos todo a ÚNICA pero sin título para que se vea normal
    if (!tieneSeparacion) {
        grupos.UNICA.titulo = '';
    }

    return { tieneSeparacion, A: grupos.A, B: grupos.B, UNICA: grupos.UNICA };
});

const sugerenciasFiltradas = computed(() => {
    const texto = insumoBusquedaTexto.value.trim().toUpperCase();
    let lista = props.materiasPrimas || [];
    const idClienteActual = Number(props.cliente?.id || props.form?.clienteId || 0);

    lista = lista.filter(mp => {
        const idDuenio = Number(mp.clienteId || mp.ClienteId || 0);
        if (idDuenio > 1 && idDuenio !== idClienteActual) return false;
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
        // 🚀 AHORA ENVÍA TAMBIÉN A QUÉ TOLVA VA EL INSUMO
        emit('add-insumo', { 
            id: mpEncontrada.id, 
            porcentaje: Number(insumoExtraPorc.value),
            extrusoraDestino: insumoExtraExtrusora.value
        });
        insumoBusquedaTexto.value = ''; 
        insumoExtraPorc.value = ''; 
        insumoExtraExtrusora.value = 'UNICA';
        mostrarLista.value = false;
    }
};

const solicitarQuitar = (item: any) => { 
    const indexReal = props.receta.findIndex((r: any) => r.materiaPrimaId === item.materiaPrimaId || r.id === item.id);
    if (indexReal !== -1) {
        emit('remove-insumo', indexReal); 
    }
};

const solicitarModificarPorcentaje = (item: any, event: Event) => {
    const target = event.target as HTMLInputElement | null;
    if (target) {
        const val = Number(target.value);
        if (!isNaN(val) && val >= 0) {
            emit('add-insumo', { 
                id: item.materiaPrimaId || item.id, 
                porcentaje: val,
                extrusoraDestino: item.extrusoraDestino // Respeta la tolva original
            });
        }
    }
};

const fechaHoy = new Date().toLocaleString('es-AR', { 
    day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit'
});

const tituloLimpioParaPDF = computed(() => {
    const nombreForm = props.form?.productoNombre;
    const nombreProducto = props.producto?.nombre;
    const nombreAdentroDeForm = props.form?.producto?.nombre;
    const candidatos = [nombreProducto, nombreAdentroDeForm, nombreForm].filter(Boolean);

    let tituloFinal = "MEZCLA MÚLTIPLE"; 
    for (let candidato of candidatos) {
        let texto = String(candidato).trim();
        if (texto && texto !== "MEZCLA CONSOLIDADA" && texto !== "MEZCLA MÚLTIPLE" && !texto.includes('[object')) {
            tituloFinal = texto;
            break;
        }
    }

    const upper = tituloFinal.toUpperCase();
    const prefijos = ['LAMINADO A FAZON -', 'LAMINADO A FAZON-', 'FAZON -', 'FAZON-', 'FAZON '];
    for (const pref of prefijos) {
        if (upper.startsWith(pref)) return tituloFinal.substring(pref.length).trim();
    }
    return tituloFinal;
});

const observacionLimpia = computed(() => {
    if (!props.form?.observacion) return '-';
    let limpia = props.form.observacion
        .replace(/\[Grupo: HC-[^\]]+\]/g, '')
        .replace(/\[LOTE: HC-[^\]]+\]/g, '')
        .replace(/\[FORZAR_CARGA\]/g, '')
        .trim();

    if (limpia === '.' || limpia === '-' || limpia === '.-') return '-';
    limpia = limpia.replace(/^[.\s-]+\n+/g, '').trim();
    return limpia || '-';
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
                <h3>{{ modoCargaLimpia ? (esConsolidadoReal ? 'HOJA DE CARGA MÚLTIPLE' : 'HOJA DE CARGA INDIVIDUAL') : (ocultarFormula ? 'ORDEN DE PRODUCCIÓN' : 'HOJA DE CARGA') }}</h3>
                
                <div v-if="modoCargaLimpia" class="lote-mezcla-resaltado">
                    LOTE N°: {{ codigoLoteVisible }} 
                </div>

                <p>FECHA: <strong>{{ fechaHoy }}</strong></p>
                <p>NOTA PEDIDO: <strong>{{ notasPedidoVisibles }}</strong></p>
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

        <div class="ficha-tecnica-pdf" style="margin-top: -4px;" v-if="tieneBrillo || llevaFilm || tipoCorona || esGofrado || tieneUV || form?.esImpresion">
            <div class="dato-box-pdf" v-if="form?.esImpresion || form?.cargaImpresion > 0">
                <span class="label-tech-pdf">CARGA (IMPRESIÓN)</span>
                <span class="valor-tech-pdf">{{ form?.cargaImpresion || form?.aditivoCarga || 0 }} Kg/Porc</span>
            </div>
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
                <span class="label-tech-pdf">TRAT. UV</span>
                <span class="valor-tech-pdf">SÍ</span>
            </div>
        </div>

        <div v-show="!ocultarFormula" class="seccion-receta-pdf">
            <div class="titulo-receta-pdf">
                {{ modoCargaLimpia ? (esConsolidadoReal ? 'RESUMEN DE MEZCLA CONSOLIDADA' : 'RECETA DE CARGA A BATEA') : (densidadReal > 0 ? `FÓRMULA DE MEZCLA (Densidad: ${parseFloat(densidadReal.toFixed(3))})` : 'FÓRMULA DE MEZCLA') }}
                <span style="float:right; font-size: 0.8em; color: #333" v-if="!modoCargaLimpia" class="ocultar-en-impresion">Total: {{ Number(totalPorcentaje).toFixed(2) }}%</span>
            </div>

            <!-- 🚀 RECORREMOS LOS GRUPOS DE EXTRUSORAS -->
            <div class="contenedor-tolvas">
                <template v-for="grupoKey in ['A', 'B', 'UNICA']" :key="grupoKey">
                    <div v-if="gruposReceta[grupoKey].items.length > 0" class="grupo-tolva">
                        <div v-if="gruposReceta[grupoKey].titulo" :class="'titulo-tolva tolva-' + grupoKey">
                            {{ gruposReceta[grupoKey].titulo }}
                        </div>
                        
                        <table class="tabla-receta-pdf">
                            <thead>
                                <tr v-if="!gruposReceta.tieneSeparacion">
                                    <th>INSUMO / MATERIA PRIMA</th>
                                    <th style="width:100px; text-align:center;" v-if="!modoCargaLimpia" class="ocultar-en-impresion">% MEZCLA</th>
                                    <th style="width:120px; text-align:right;">PESO A CARGAR</th>
                                    <th data-html2canvas-ignore="true" style="width:40px" v-if="!modoCargaLimpia" class="ocultar-en-impresion"></th>
                                </tr>
                            </thead>
                            <tbody>
                                <template v-for="(r, i) in gruposReceta[grupoKey].items" :key="i">
                                    <tr>
                                        <td style="font-weight: 600;">
                                            {{ r.nombreInsumo || r.nombreMateriaPrima }}
                                            <span v-if="obtenerEtiquetaOrigen(r)" style="font-size: 0.85em; font-style: italic; color: #555; margin-left: 5px;">
                                                {{ obtenerEtiquetaOrigen(r) }}
                                            </span>
                                        </td>
                                        
                                        <td style="text-align:center; vertical-align: middle; width:100px;" v-if="!modoCargaLimpia" class="ocultar-en-impresion">
                                            <div v-if="esInsumoFijo(r)" style="font-weight: bold; color: #2980b9; font-size: 11px; letter-spacing: 1px;">
                                                (EXTRA)
                                            </div>
                                            <div v-else>
                                                <div style="display:flex; justify-content:center; align-items:center;">
                                                    <input type="number" step="0.01" min="0" :value="Number(r.cantidad).toFixed(2)" @change="solicitarModificarPorcentaje(r, $event)" class="input-porc-edit"/> %
                                                </div>
                                            </div>
                                        </td>
                                        
                                        <td style="text-align:right; font-size: 1.1em; width:120px;">
                                            <strong v-if="esInsumoFijo(r)" style="color: #2980b9;">
                                                {{ esConsolidadoReal 
                                                    ? parseFloat(r.cantidadKilos || r.CantidadKilos || r.real || r.kilos || 0).toFixed(2) 
                                                    : (r.kilosFijos 
                                                        ? parseFloat(r.kilosFijos).toFixed(2) 
                                                        : ceilKilos((pesoBrutoExacto * (parseFloat(r.cantidad?.toString()) || 0)) / 100).toFixed(2)) 
                                                }} kg
                                            </strong>
                                            <strong v-else>
                                                {{ esConsolidadoReal 
                                                    ? parseFloat(r.cantidadKilos || r.CantidadKilos || r.real || r.kilos || 0).toFixed(2) 
                                                    : ceilKilos((pesoBrutoExacto * (parseFloat(r.cantidad?.toString()) || 0)) / 100).toFixed(2) 
                                                }} kg
                                            </strong>
                                        </td>
                                        
                                        <td data-html2canvas-ignore="true" v-if="!modoCargaLimpia" style="text-align:center; width:40px;" class="ocultar-en-impresion">
                                            <button v-if="!esInsumoFijo(r)" @click="solicitarQuitar(r)" class="btn-borrar-insumo" title="Quitar insumo">❌</button>
                                        </td>
                                    </tr>
                                </template>
                            </tbody>
                        </table>
                    </div>
                </template>

                <div v-if="recetaVisual.length === 0" style="text-align: center; color: #7f8c8d; padding: 15px; font-style: italic;">
                    Aún no hay materiales. Agregue insumos desde el buscador 👇
                </div>
            </div>

            <div class="agregar-fila-pdf ocultar-en-impresion" data-html2canvas-ignore="true" v-if="!modoCargaLimpia">
                <div class="buscador-wrapper">
                    <input type="text" v-model="insumoBusquedaTexto" @focus="mostrarLista = true" @blur="cerrarListaConDelay" class="input-buscador" placeholder="Buscar materia prima..." />
                    <div class="lista-resultados" v-if="mostrarLista && sugerenciasFiltradas.length > 0">
                        <div v-for="mp in sugerenciasFiltradas" :key="mp.id" class="item-resultado" @click="seleccionarInsumo(mp)">
                            <span class="nombre-insumo-lista">{{ mp.nombre }}</span>
                            <span v-if="(mp.clienteId || mp.ClienteId) > 1" class="badge-mini-cliente">
                                👤 {{ cliente && (mp.clienteId || mp.ClienteId) === cliente.id ? cliente.razonSocial : 'TERCERO' }}
                            </span>
                            <span v-else class="badge-mini-propio">🏢 PROPIO</span>
                        </div>
                    </div>
                </div>
                <input type="number" v-model="insumoExtraPorc" placeholder="%" style="width: 60px; padding: 6px; border: 1px solid #ccc; border-radius: 4px; margin-left: 5px;" />
                
                <!-- 🚀 SELECTOR PARA ELEGIR TOLVA AL AGREGAR MANUALMENTE -->
                <select v-model="insumoExtraExtrusora" style="margin-left: 5px; padding: 6px; border: 1px solid #ccc; border-radius: 4px;">
                    <option value="UNICA">Única</option>
                    <option value="A">Tolva A</option>
                    <option value="B">Tolva B</option>
                </select>

                <button class="btn-add-insumo" @click="solicitarAgregar" style="margin-left: 5px;">AGREGAR</button>
            </div>
        </div>

        <div class="fila-lotes-pdf">
            <div class="mitad-pdf" v-if="!esConsolidadoReal">
                <strong>CANTIDAD (UNIDADES):</strong>
                
                <div class="recuadro-gigante-pdf" style="font-size: 16px;">
                    {{ form.cantidad }}
                    <span v-if="form.cantidad >= 10 && !form.esBobina" style="font-size: 13px; color: #333; margin-left: 6px; font-weight: bold;">
                        | {{ Math.floor(form.cantidad / (form.cantidad >= 200 ? 20 : 10)) }} paq. de {{ form.cantidad >= 200 ? 20 : 10 }}<span v-if="form.cantidad % (form.cantidad >= 200 ? 20 : 10) > 0"> y 1 de {{ form.cantidad % (form.cantidad >= 200 ? 20 : 10) }}</span>
                    </span>
                </div>
            </div>
            <div class="mitad-pdf" :style="esConsolidadoReal ? 'width: 100%;' : ''">
                <strong>OBSERVACIONES / DETALLES DE LOTE:</strong>
                <div class="recuadro-gigante-pdf observacion-wrap-pdf">{{ observacionLimpia }}</div>
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
.dato-relleno { font-family: 'Courier New', monospace; font-size: 16px; font-weight: bold; margin-left: 10px; text-transform: uppercase; white-space: normal; word-break: break-word;}
.caja-producto-pdf { border: 2px solid black; padding: 8px; margin-bottom: 8px; text-align: center; background: #f9f9f9; }
.titulo-seccion-pdf { font-size: 10px; font-weight: bold; margin-bottom: 2px; letter-spacing: 1px; }
.producto-nombre-pdf { font-size: 18px; font-weight: 900; white-space: normal; word-break: break-word;}
.producto-sku-pdf { font-size: 12px; margin-top: 2px; }
.ficha-tecnica-pdf { display: flex; border: 2px solid black; margin-bottom: 8px; }
.dato-box-pdf { flex: 1; border-right: 1px solid black; text-align: center; padding: 4px; }
.dato-box-pdf:last-child { border-right: none; }
.label-tech-pdf { display: block; font-size: 9px; font-weight: bold; color: #333; }
.valor-tech-pdf { font-size: 14px; font-weight: bold; margin-top: 2px; display: block; }
.seccion-receta-pdf { margin-top: 10px; border: 2px solid black; font-size: 14px; }
.titulo-receta-pdf { background: #e0e0e0; padding: 5px; font-weight: 900; text-align: center; border-bottom: 2px solid black; font-size: 14px; }

/* 🚀 ESTILOS PARA LAS TOLVAS MÚLTIPLES */
.contenedor-tolvas { display: flex; flex-direction: column; gap: 0px; }
.titulo-tolva { font-size: 11px; font-weight: 900; padding: 4px 8px; border-bottom: 2px solid black; border-top: 2px solid black; text-align: center; }
.tolva-A { background-color: #e0f2fe; color: #0369a1; }
.tolva-B { background-color: #dcfce7; color: #15803d; }
.tolva-UNICA { background-color: #f1f5f9; color: #475569; }

.tabla-receta-pdf { width: 100%; border-collapse: collapse; }
.tabla-receta-pdf th { border-right: 1px solid black; border-bottom: 2px solid black; padding: 5px; background: #f4f4f4; font-size: 11px; text-align: left; }
.tabla-receta-pdf th:last-child { border-right: none; }
.tabla-receta-pdf td { border-right: 1px solid black; padding: 5px; font-size: 12px; border-bottom: 1px solid #ccc; }
.tabla-receta-pdf td:last-child { border-right: none; }

.fila-lotes-pdf { display: flex; gap: 15px; margin-top: 5px; margin-bottom: 10px; }
.mitad-pdf { flex: 1; }
.recuadro-gigante-pdf { border: 2px solid black; height: 35px; font-size: 20px; display: flex; align-items: center; justify-content: center; margin-top: 2px; font-weight: 900; overflow: hidden; white-space: nowrap; }

.observacion-wrap-pdf {
    white-space: pre-wrap !important; 
    height: auto !important;
    min-height: 35px;
    padding: 6px 12px;
    font-size: 12px !important;
    line-height: 1.3;
    justify-content: flex-start !important;
    align-items: flex-start !important;
    text-align: left !important;
    word-break: break-word !important;
    overflow-wrap: break-word !important;
}

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

.input-porc-edit {
    width: 60px; text-align: right; border: 1px solid #bdc3c7; border-radius: 4px; padding: 4px; font-size: 12px; font-weight: bold; color: #2c3e50; background: #fff; margin-right: 4px; transition: all 0.2s;
}
.input-porc-edit:focus { border-color: #3498db; outline: none; box-shadow: 0 0 3px rgba(52, 152, 219, 0.5); }

@media print {
    .ocultar-en-impresion { display: none !important; }
}

input[type=number]::-webkit-inner-spin-button,
input[type=number]::-webkit-outer-spin-button { -webkit-appearance: none; margin: 0; }
input[type=number] { -moz-appearance: textfield; appearance: textfield; }
</style>