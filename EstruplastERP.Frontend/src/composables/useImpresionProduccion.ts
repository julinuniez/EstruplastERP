import { nextTick } from 'vue';
import type { Ref } from 'vue';
// @ts-ignore
import html2pdf from 'html2pdf.js';
import { ProduccionAPI } from '@/services/produccionService';
import Swal from 'sweetalert2'; 

export function useImpresionProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    ocultarFormula: Ref<boolean>,
    imprimiendoHistorial: Ref<boolean>,
    cantidadPalletsUsuario: Ref<number>,
    mensaje: Ref<string>,
    error: Ref<string>,
    loading: Ref<boolean>,
    listaProduccionRef: Ref<any>,
    balancearBase: () => void,
    limpiarFormulario: () => void,
    inventarioCompleto: Ref<any[]>
) {

    const determinarDuenioMaterial = (idInsumo: number, insumoHistorial: any) => {
        const mpMaestra = inventarioCompleto.value?.find(m => Number(m.id) === idInsumo);
        if (mpMaestra) {
            const esFazon = mpMaestra.esFazon === true || mpMaestra.EsFazon === true;
            return esFazon ? Number(mpMaestra.clienteId || mpMaestra.ClienteId || 0) : 0;
        } else {
            return Number(insumoHistorial.clienteId || insumoHistorial.ClienteId || 0);
        }
    };

    function calcularEtiquetasPallets(kilosTotales: number, cantidadTotal: number, cantidadPalletsElegida: number) {
        if (cantidadPalletsElegida <= 1) return [{ palletNumero: 1, palletTotal: 1, kilos: kilosTotales, laminas: cantidadTotal }];
        let pallets = [];
        let laminasRestantes = cantidadTotal;
        let kilosRestantes = kilosTotales;

        for (let i = 1; i <= cantidadPalletsElegida; i++) {
            let esUltimoPallet = (i === cantidadPalletsElegida);
            let laminasPallet = esUltimoPallet ? laminasRestantes : Math.round(cantidadTotal / cantidadPalletsElegida);
            let kilosPallet = esUltimoPallet ? kilosRestantes : Math.round((kilosTotales / cantidadTotal) * laminasPallet);
            pallets.push({ palletNumero: i, palletTotal: cantidadPalletsElegida, kilos: kilosPallet, laminas: laminasPallet });
            laminasRestantes -= laminasPallet;
            kilosRestantes -= kilosPallet;
        }
        return pallets;
    }

    async function generarPDF(tipo: 'orden' | 'carga' | 'carga-consolidada', limiteKilosCliente: number = 1000) {
        const tipoLimpio = String(tipo).trim().toLowerCase();
        ocultarFormula.value = (tipoLimpio === 'orden');
        
        const bloqueoOriginal = imprimiendoHistorial.value;
        imprimiendoHistorial.value = true;

        const elementoTarget = document.getElementById('impresion-fantasma') || document.getElementById('hoja-de-impresion');
        if (!elementoTarget) {
            console.error("CRÍTICO: No se encontró el componente de impresión en el HTML.");
            return;
        }

        const opcionesPDF: any = {
            margin: 0,
            image: { type: 'jpeg', quality: 0.85 },
            html2canvas: { scale: 2, useCORS: true },
            jsPDF: { unit: 'mm', format: 'a4' }
        };

        if (tipoLimpio === 'orden' && form.value.kilosTotales > limiteKilosCliente && cantidadPalletsUsuario.value > 1) {
            const tickets = calcularEtiquetasPallets(form.value.kilosTotales, form.value.cantidad, cantidadPalletsUsuario.value);
            const originalKilos = form.value.kilosTotales;
            const originalCantidad = form.value.cantidad;
            const originalObs = form.value.observacion;

            const contenedorLote = document.createElement('div');

            for (const ticket of tickets) {
                form.value.kilosTotales = ticket.kilos;
                form.value.cantidad = ticket.laminas;
                form.value.observacion = originalObs 
                    ? `${originalObs} | [PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]` 
                    : `[PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]`;

                await nextTick();
                await new Promise(r => setTimeout(r, 60));

                const clon = elementoTarget.cloneNode(true) as HTMLElement;
                clon.style.display = 'block';
                
                const inputsOriginales = elementoTarget.querySelectorAll('input, textarea');
                const inputsClonados = clon.querySelectorAll('input, textarea');
                inputsClonados.forEach((input: any, idx) => {
                    const span = document.createElement('span');
                    span.innerText = (inputsOriginales[idx] as HTMLInputElement).value;
                    span.style.fontWeight = 'bold';
                    input.parentNode?.replaceChild(span, input);
                });

                const wrap = document.createElement('div');
                if (ticket.palletNumero < ticket.palletTotal) wrap.style.pageBreakAfter = 'always';
                wrap.appendChild(clon);
                contenedorLote.appendChild(wrap);
            }

            opcionesPDF.filename = `Orden_${form.value.notaPedido}_Pallets_${Date.now()}.pdf`;
            await html2pdf().set(opcionesPDF).from(contenedorLote).save();

            form.value.kilosTotales = originalKilos;
            form.value.cantidad = originalCantidad;
            form.value.observacion = originalObs;
        } else {
            await nextTick();
            await new Promise(r => setTimeout(r, 150));
            opcionesPDF.filename = `Doc_${Date.now()}.pdf`;
            await html2pdf().set(opcionesPDF).from(elementoTarget).save();
        }

        ocultarFormula.value = false;
        imprimiendoHistorial.value = bloqueoOriginal;
    }

    const imprimirDesdeHistorial = async (payload: { orden: any, tipo: string, imprimirEnPaquetes?: boolean }) => {
        const { orden, tipo, imprimirEnPaquetes } = payload;
        
        let obsCruda = String(orden?.observacion || '');
        let forzarModoCarga = false;
        
        if (obsCruda.includes('[FORZAR_CARGA]')) {
            forzarModoCarga = true;
            obsCruda = obsCruda.replace('[FORZAR_CARGA]', '').trim();
            if (orden) orden.observacion = obsCruda; 
        }

        let tipoLimpio = String(tipo).trim().toLowerCase();
        const limiteKilos = Number(orden?.limiteKilosPallet || orden?.LimiteKilosPallet || 1000);

        try {
            loading.value = true;
            imprimiendoHistorial.value = true;
            
            form.value.esConsolidado = orden?.esConsolidado || tipoLimpio.includes('consolidada');
            form.value.productoTerminadoId = orden.productoId;
            form.value.clienteId = orden.clienteId;
            form.value.notaPedido = String(orden.notaPedido || orden.id);
            form.value.productoNombre = orden.producto;
            form.value.numeroPedidoCliente = orden.numeroPedidoCliente;
            form.value.clienteNombre = orden.clienteNombre;
            form.value.largo = orden.largo;
            form.value.ancho = orden.ancho;
            form.value.espesor = orden.espesor;
            form.value.esBobina = !!orden.esBobina;
            form.value.cantidad = orden.cantidad;
            form.value.observacion = obsCruda;
            form.value.conBrillo = orden.conBrillo || false;
            form.value.llevaFilm = orden.llevaFilm || false;
            form.value.tipoCorona = orden.tipoCorona || 'Ninguno';
            form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;
            form.value.aditivoUV = orden.aditivoUV || orden.AditivoUV || false;
            form.value.color = orden.color || orden.colorTexto || '';
            form.value.colorTexto = orden.colorTexto || orden.color || '';
            form.value.Color = orden.color || '';
            
            const desp = Number(orden.desperdicio || 0);
            form.value.merma = desp;
            form.value.kilosTotales = orden.kilos;
            form.value.imprimirEnPaquetes = imprimirEnPaquetes || false;

            const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

            const esHojaCargaOMezlca = 
                tipoLimpio.includes('carga') || 
                tipoLimpio.includes('consolidada') || 
                forzarModoCarga ||
                form.value.esConsolidado ||
                String(orden?.producto || '').toUpperCase().includes('CONSOLIDADA') ||
                String(orden?.producto || '').toUpperCase().includes('MEZCLA');

            if (esHojaCargaOMezlca) {
                tipoLimpio = (form.value.esConsolidado || String(orden?.producto || '').toUpperCase().includes('CONSOLIDADA')) ? 'carga-consolidada' : 'carga';
                ocultarFormula.value = false; 
            }

            const esKilosFijos = orden.esFinalizada || String(orden.estado).toUpperCase() === 'FINALIZADA' || orden.kilosYaCalculados;

            // 🚀 DEDUCIMOS LOS PORCENTAJES REALES DE LAS TOLVAS DESDE LA BASE DE DATOS
            let pesoA = 0; let pesoB = 0; let pesoC = 0; let pesoTotalPuros = 0;

            orden.consumos.forEach((c: any) => {
                const destino = String(c.extrusoraDestino || c.ExtrusoraDestino || 'UNICA').toUpperCase();
                const valorDB = Number(c.cantidadKilos || c.CantidadKilos || c.cantidad || 0);
                
                // 🚀 MATEMÁTICA CORRECTA: Si es pendiente (aún no se consumió en stock físico), le agregamos la merma al peso neto.
                let kilos = esKilosFijos ? valorDB : (valorDB * (1 + (desp / 100)));
                const n = String(c.nombreMateriaPrima || c.nombreInsumo || '').toUpperCase();
                const esAditivo = n.includes('ESTEARATO') || n.includes('BRILLO') || n.includes('UV') || n.includes('CAUCHO');
                
                if (!esAditivo) {
                    if (destino === 'A') pesoA += kilos;
                    else if (destino === 'B') pesoB += kilos;
                    else if (destino === 'C') pesoC += kilos;
                    pesoTotalPuros += kilos;
                }
            });

            if (pesoTotalPuros > 0) {
                form.value.porcentajeTolvaA = Math.round((pesoA / pesoTotalPuros) * 100);
                form.value.porcentajeTolvaB = Math.round((pesoB / pesoTotalPuros) * 100);
                form.value.porcentajeTolvaC = Math.round((pesoC / pesoTotalPuros) * 100);
            } else {
                form.value.porcentajeTolvaA = 100; form.value.porcentajeTolvaB = 0; form.value.porcentajeTolvaC = 0;
            }

            recetaDinamica.value = orden.consumos.map((c: any) => {
                const idBuscado = Number(c.materiaPrimaId || c.id);
                const destino = String(c.extrusoraDestino || c.ExtrusoraDestino || 'UNICA').toUpperCase();
                const valorDB = Number(c.cantidadKilos || c.CantidadKilos || c.cantidad || 0);
                
                // 🚀 MATEMÁTICA CORRECTA
                const kilosFisicosReales = esKilosFijos ? valorDB : (valorDB * (1 + (desp / 100)));

                let pesoDeEstaTolva = pesoTotalPuros;
                if (destino === 'A') pesoDeEstaTolva = pesoA;
                if (destino === 'B') pesoDeEstaTolva = pesoB;
                if (destino === 'C') pesoDeEstaTolva = pesoC;

                const n = String(c.nombreMateriaPrima || c.nombreInsumo || '').toUpperCase();
                const esAditivo = n.includes('ESTEARATO') || n.includes('BRILLO') || n.includes('UV') || n.includes('CAUCHO');

                let porcentajeLocal = 0;
                if (esAditivo || esHojaCargaOMezlca) {
                    porcentajeLocal = pesoBrutoTotal > 0 ? (kilosFisicosReales / pesoBrutoTotal) * 100 : 0;
                } else {
                    porcentajeLocal = pesoDeEstaTolva > 0 ? (kilosFisicosReales / pesoDeEstaTolva) * 100 : 0;
                }

                return {
                    id: Math.random(),
                    materiaPrimaId: idBuscado,
                    nombreInsumo: c.nombreMateriaPrima || c.nombreInsumo,
                    cantidad: porcentajeLocal.toFixed(2), 
                    kilosFijos: kilosFisicosReales.toFixed(2), 
                    clienteId: determinarDuenioMaterial(idBuscado, c),
                    extrusoraDestino: destino
                };
            });

            if (!form.value.esConsolidado && typeof balancearBase === 'function') balancearBase();

            const debaPreguntarPallets = !esHojaCargaOMezlca && tipoLimpio === 'orden' && Number(orden?.kilos || 0) > limiteKilos;

            if (debaPreguntarPallets) {
                const palletsSugeridos = Math.ceil(Number(orden.kilos) / limiteKilos);
                
                const result = await Swal.fire({
                    title: 'Dividir Impresión',
                    text: `⚠️ Pedido de ${orden.kilos} kg.\nEl límite para este cliente es de ${limiteKilos} kg.\n¿En cuántos pallets querés dividir la impresión de las OP?`,
                    input: 'number',
                    inputValue: palletsSugeridos,
                    showCancelButton: true,
                    confirmButtonText: 'Imprimir',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: '#3498db',
                    inputValidator: (value) => {
                        if (!value || parseInt(value) <= 0) {
                            return 'Debe ser mayor a 0';
                        }
                    }
                });

                if (!result.isConfirmed) {
                    loading.value = false;
                    imprimiendoHistorial.value = false;
                    return;
                }

                cantidadPalletsUsuario.value = parseInt(result.value);
            } else {
                cantidadPalletsUsuario.value = 1;
            }

            await generarPDF(tipoLimpio as any, limiteKilos);

            if (tipoLimpio === 'orden') {
                await ProduccionAPI.marcarOrdenImpresa(orden.id);
                if (listaProduccionRef.value) await listaProduccionRef.value.cargarHistorial();
            }
        } catch (e) {
            error.value = "Error crítico en impresión.";
            console.error(e);
        } finally { 
            imprimiendoHistorial.value = false;
            loading.value = false; 
            setTimeout(() => { if (typeof limpiarFormulario === 'function') limpiarFormulario(); }, 200);
        }
    };

    const imprimirLoteOPsDesdeHistorial = async (ordenesArray: any[]) => {
        try {
            mensaje.value = `⏳ Construyendo páginas a máxima velocidad...`;
            ocultarFormula.value = true; 
            imprimiendoHistorial.value = true; 

            const elementoTarget = document.getElementById('impresion-fantasma') || document.getElementById('hoja-de-impresion');
            if (!elementoTarget) throw new Error("No se encontró contenedor para imprimir");

            const contenedorLote = document.createElement('div');

            for (const orden of ordenesArray) {
                form.value.esConsolidado = false;
                form.value.productoTerminadoId = orden.productoId;
                form.value.clienteId = orden.clienteId;
                form.value.notaPedido = String(orden.notaPedido || orden.id);
                form.value.numeroPedidoCliente = orden.numeroPedidoCliente || '-';
                form.value.largo = orden.largo || 0;
                form.value.ancho = orden.ancho || 0;
                form.value.espesor = orden.espesor || 0;
                form.value.cantidad = orden.cantidad;
                form.value.esBobina = !!orden.esBobina;
                form.value.observacion = orden.observacion || '';
                form.value.kilosTotales = orden.kilos; 
                
                form.value.conBrillo = orden.conBrillo || orden.ConBrillo || false;
                form.value.llevaFilm = orden.llevaFilm || orden.LlevaFilm || false;
                form.value.tipoCorona = orden.tipoCorona || orden.TipoCorona || 'Ninguno';
                form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;
                form.value.aditivoUV = orden.aditivoUV || orden.AditivoUV || false;
                
                form.value.color = orden.color || orden.colorTexto || '';
                form.value.colorTexto = orden.colorTexto || orden.color || '';
                form.value.Color = orden.color || '';

                form.value.imprimirEnPaquetes = false;
                
                const desp = Number(orden.desperdicio || 0);
                const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));
                const esKilosFijos = orden.esFinalizada || String(orden.estado).toUpperCase() === 'FINALIZADA' || orden.kilosYaCalculados;

                if (orden.consumos) {
                    recetaDinamica.value = orden.consumos.map((c: any) => {
                        const idBuscado = Number(c.materiaPrimaId || c.id);
                        const valorDB = Number(c.cantidadKilos || c.CantidadKilos || c.cantidad || 0);
                        
                        // 🚀 MATEMÁTICA CORRECTA PARA LOS LOTES
                        const kilosFisicosReales = esKilosFijos ? valorDB : (valorDB * (1 + (desp / 100)));
                        const porcentajeVisible = pesoBrutoTotal > 0 ? (kilosFisicosReales / pesoBrutoTotal) * 100 : 0;
                        
                        return {
                            id: Math.random(),
                            materiaPrimaId: idBuscado,
                            nombreInsumo: c.nombreMateriaPrima || c.nombreInsumo,
                            cantidad: porcentajeVisible.toFixed(2),
                            kilosFijos: kilosFisicosReales.toFixed(2), 
                            clienteId: determinarDuenioMaterial(idBuscado, c),
                            extrusoraDestino: c.extrusoraDestino || c.ExtrusoraDestino || 'UNICA'
                        };
                    });
                }
                
                if (typeof balancearBase === 'function') balancearBase();
                
                await nextTick();
                await new Promise(r => setTimeout(r, 60));

                const clon = elementoTarget.cloneNode(true) as HTMLElement;
                clon.style.display = 'block';
                
                const inputsOriginales = elementoTarget.querySelectorAll('input, textarea');
                const inputsClonados = clon.querySelectorAll('input, textarea');
                inputsClonados.forEach((input: any, idx) => {
                    const span = document.createElement('span');
                    span.innerText = (inputsOriginales[idx] as HTMLInputElement).value;
                    span.style.fontWeight = 'bold';
                    input.parentNode?.replaceChild(span, input);
                });

                const wrap = document.createElement('div');
                wrap.style.pageBreakAfter = 'always';
                wrap.appendChild(clon);
                contenedorLote.appendChild(wrap);
            }

            await html2pdf().set({
                margin: 0,
                filename: `Lote_OP_${Date.now()}.pdf`,
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'a4' }
            }).from(contenedorLote).save();

            for (const orden of ordenesArray) await ProduccionAPI.marcarOrdenImpresa(orden.id);
            if (listaProduccionRef.value) await listaProduccionRef.value.cargarHistorial();

            mensaje.value = "✅ Lote generado con éxito";
        } catch (e) {
            error.value = "Error al generar lote.";
            console.error(e);
        } finally {
            ocultarFormula.value = false;
            imprimiendoHistorial.value = false;
            setTimeout(() => { if (typeof limpiarFormulario === 'function') limpiarFormulario(); }, 200);
        }
    };

    return { imprimirDesdeHistorial, imprimirLoteOPsDesdeHistorial };
}