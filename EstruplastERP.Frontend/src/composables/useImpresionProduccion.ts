import { nextTick } from 'vue';
import type { Ref } from 'vue';
// @ts-ignore
import html2pdf from 'html2pdf.js';
import { ProduccionAPI } from '@/services/produccionService';

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
    limpiarFormulario: () => void
) {

    // 1. Calculador de Pallets (Interno)
    function calcularEtiquetasPallets(kilosTotales: number, cantidadTotal: number, cantidadPalletsElegida: number) {
        if (cantidadPalletsElegida <= 1) {
            return [{ palletNumero: 1, palletTotal: 1, kilos: kilosTotales, laminas: cantidadTotal }];
        }
        let pallets = [];
        let laminasRestantes = cantidadTotal;
        let kilosRestantes = kilosTotales;

        for (let i = 1; i <= cantidadPalletsElegida; i++) {
            let esUltimoPallet = (i === cantidadPalletsElegida);
            let laminasPallet = esUltimoPallet ? laminasRestantes : Math.round(cantidadTotal / cantidadPalletsElegida);
            let kilosPallet = esUltimoPallet ? kilosRestantes : Math.round((kilosTotales / cantidadTotal) * laminasPallet);

            pallets.push({
                palletNumero: i,
                palletTotal: cantidadPalletsElegida,
                kilos: kilosPallet,
                laminas: laminasPallet
            });
            laminasRestantes -= laminasPallet;
            kilosRestantes -= kilosPallet;
        }
        return pallets;
    }

    // 2. Generador del PDF (Interno)
    async function generarPDF(tipo: 'orden' | 'carga' | 'carga-consolidada') {
        ocultarFormula.value = (tipo === 'orden');
        const bloqueoOriginal = imprimiendoHistorial.value;
        imprimiendoHistorial.value = true;

        if (tipo === 'orden' && form.value.kilosTotales > 1000 && cantidadPalletsUsuario.value > 1) {
            const tickets = calcularEtiquetasPallets(form.value.kilosTotales, form.value.cantidad, cantidadPalletsUsuario.value);
            const originalKilos = form.value.kilosTotales;
            const originalCantidad = form.value.cantidad;
            const originalObs = form.value.observacion;

            const elementoOriginal = document.getElementById('hoja-de-impresion');
            const contenedorTemporal = document.createElement('div');
            contenedorTemporal.style.width = '210mm';

            for (const ticket of tickets) {
                form.value.kilosTotales = ticket.kilos;
                form.value.cantidad = ticket.laminas;
                form.value.observacion = originalObs 
                    ? `${originalObs} | [PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]` 
                    : `[PALLET ${ticket.palletNumero} DE ${ticket.palletTotal}]`;

                await nextTick();
                await new Promise(r => setTimeout(r, 800));

                if (elementoOriginal) {
                    const clon = elementoOriginal.cloneNode(true) as HTMLElement;
                    clon.style.display = 'block';
                    
                    const inputsOriginales = elementoOriginal.querySelectorAll('input, textarea');
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
                    contenedorTemporal.appendChild(wrap);
                }
            }

            await html2pdf().set({
                margin: 0,
                filename: `Orden_${form.value.notaPedido}_Pallets_${Date.now()}.pdf`,
                image: { type: 'jpeg', quality: 0.75 },
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'a4' }
            }).from(contenedorTemporal).save();

            contenedorTemporal.remove();
            form.value.kilosTotales = originalKilos;
            form.value.cantidad = originalCantidad;
            form.value.observacion = originalObs;

        } else {
            await nextTick();
            await new Promise(r => setTimeout(r, 600));
            
            const elemento = document.getElementById('hoja-de-impresion');
            if (elemento) {
                await html2pdf().set({
                    margin: 0,
                    filename: `Doc_${Date.now()}.pdf`,
                    image: { type: 'jpeg', quality: 0.70 },
                    html2canvas: { scale: 2 },
                    jsPDF: { unit: 'mm', format: 'a4' }
                }).from(elemento).save();
            }
        }

        ocultarFormula.value = false;
        imprimiendoHistorial.value = bloqueoOriginal;
    }

    // 3. Imprimir UNA orden desde el historial (Exportado)
    const imprimirDesdeHistorial = async (payload: { orden: any, tipo: string }) => {
        const { orden, tipo } = payload;
        const isConsolidado = tipo === 'carga-consolidada';
        try {
            loading.value = true;
            imprimiendoHistorial.value = true;
            
            recetaDinamica.value = []; 
            form.value.observacion = '';

            form.value.esConsolidado = isConsolidado;
            form.value.productoTerminadoId = orden.productoId;
            form.value.clienteId = orden.clienteId;
            
            await nextTick();
            
            form.value.notaPedido = String(orden.notaPedido || orden.id);
            form.value.productoNombre = orden.producto;
            form.value.numeroPedidoCliente = orden.numeroPedidoCliente;
            form.value.clienteNombre = orden.clienteNombre;
            form.value.largo = orden.largo;
            form.value.ancho = orden.ancho;
            form.value.espesor = orden.espesor;
            form.value.esBobina = !!orden.esBobina;
            form.value.cantidad = orden.cantidad;
            form.value.observacion = orden.observacion || '';
            
            form.value.conBrillo = orden.conBrillo || false;
            form.value.llevaFilm = orden.llevaFilm || false;
            form.value.tipoCorona = orden.tipoCorona || 'Ninguno';
            form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;
            form.value.color = orden.color || '';
            form.value.colorTexto = orden.color || ''; 
            
            const desp = Number(orden.desperdicio || 0);
            form.value.merma = desp;
            form.value.kilosTotales = orden.kilos;

            const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

            recetaDinamica.value = orden.consumos.map((c: any) => ({
                id: Math.random(),
                materiaPrimaId: c.materiaPrimaId,
                nombreInsumo: c.nombreMateriaPrima,
                cantidad: isConsolidado ? c.cantidadKilos : Number(((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2))
            }));

            if (!form.value.esConsolidado) {
                balancearBase();
            }
            await new Promise(r => setTimeout(r, 1000)); 

            if (tipo === 'orden' && form.value.kilosTotales > 1000) {
                const palletsSugeridos = Math.ceil(form.value.kilosTotales / 1000);
                const respuesta = prompt(`⚠️ Pedido grande (${form.value.kilosTotales} kg).\n\n¿En cuántos pallets querés dividir la impresión?`, String(palletsSugeridos));
                
                cantidadPalletsUsuario.value = respuesta ? parseInt(respuesta) : 1;
                if (isNaN(cantidadPalletsUsuario.value) || cantidadPalletsUsuario.value < 1) {
                    cantidadPalletsUsuario.value = 1;
                }
            } else {
                cantidadPalletsUsuario.value = 1;
            }

            await generarPDF(tipo as any);

            if (tipo === 'orden') {
                await ProduccionAPI.marcarOrdenImpresa(orden.id);
                if (listaProduccionRef.value) {
                    await listaProduccionRef.value.cargarHistorial();
                }
            }

        } catch (e) {
            console.error("Error en reimpresión:", e);
        } finally { 
            imprimiendoHistorial.value = false;
            loading.value = false; 
            
            setTimeout(() => {
                if (typeof limpiarFormulario === 'function') limpiarFormulario();
            }, 1000);
        }
    };

    // 4. Imprimir MÚLTIPLES órdenes en lote (Exportado)
    const imprimirLoteOPsDesdeHistorial = async (ordenesArray: any[]) => {
        try {
            mensaje.value = `⏳ Construyendo páginas...`;
            
            ocultarFormula.value = true; 
            imprimiendoHistorial.value = true; 

            const elementoOriginal = document.getElementById('hoja-de-impresion');
            if (!elementoOriginal) throw new Error("No se encontró el elemento hoja-de-impresion");

            const contenedorTemporal = document.createElement('div');
            contenedorTemporal.style.width = '210mm';

            for (const orden of ordenesArray) {
                recetaDinamica.value = [];
                form.value.esConsolidado = false;
                form.value.productoTerminadoId = orden.productoId;
                form.value.clienteId = orden.clienteId;
                
                await nextTick();
                form.value.notaPedido = String(orden.notaPedido || orden.id);
                form.value.numeroPedidoCliente = orden.numeroPedidoCliente || '-';
                form.value.largo = orden.largo || 0;
                form.value.ancho = orden.ancho || 0;
                form.value.espesor = orden.espesor || 0;
                form.value.colorTexto = orden.color || '';
                form.value.cantidad = orden.cantidad;
                form.value.esBobina = !!orden.esBobina;

                form.value.observacion = orden.observacion || '';
                form.value.conBrillo = !!orden.conBrillo;
                form.value.llevaFilm = !!orden.llevaFilm;
                form.value.tipoCorona = orden.tipoCorona || 'Ninguno';
                form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;
                
                const desp = Number(orden.desperdicio || 0);
                form.value.merma = desp;
                form.value.kilosTotales = orden.kilos; 

                const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

                if (orden.consumos) {
                    recetaDinamica.value = orden.consumos.map((c: any) => ({
                        id: Math.random(),
                        materiaPrimaId: c.materiaPrimaId,
                        nombreInsumo: c.nombreMateriaPrima,
                        cantidad: ((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2)
                    }));
                }
                if (!form.value.esConsolidado) {
                    balancearBase();
                }
                
                await new Promise(r => setTimeout(r, 800));

                const clon = elementoOriginal.cloneNode(true) as HTMLElement;
                clon.style.display = 'block';
                
                const inputsOriginales = elementoOriginal.querySelectorAll('input, textarea');
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
                contenedorTemporal.appendChild(wrap);
            }

            await html2pdf().set({
                margin: 0,
                filename: `Lote_OP_${Date.now()}.pdf`,
                html2canvas: { scale: 2 },
                jsPDF: { unit: 'mm', format: 'a4' }
            }).from(contenedorTemporal).save();

            for (const orden of ordenesArray) {
                await ProduccionAPI.marcarOrdenImpresa(orden.id);
            }
            if (listaProduccionRef.value) {
                await listaProduccionRef.value.cargarHistorial();
            }

            contenedorTemporal.remove();
            mensaje.value = "✅ Lote generado con éxito";
        } catch (e) {
            console.error(e);
            error.value = "Error al generar lote.";
        } finally {
            ocultarFormula.value = false;
            imprimiendoHistorial.value = false;
            
            setTimeout(() => {
                if (typeof limpiarFormulario === 'function') limpiarFormulario();
            }, 1000);
        }
    };

    return {
        imprimirDesdeHistorial,
        imprimirLoteOPsDesdeHistorial
    };
}