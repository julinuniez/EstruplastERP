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
    limpiarFormulario: () => void,
    inventarioCompleto: Ref<any[]>
) {

    // --- LÓGICA DE NEGOCIO BLINDADA ---
    const determinarDuenioMaterial = (idInsumo: number, insumoHistorial: any) => {
        const mpMaestra = inventarioCompleto.value?.find(m => Number(m.id) === idInsumo);
        
        if (mpMaestra) {
            // 🛡️ REGLA DICTATORIAL: Si existe en la base, pero NO tiene el tilde de Fazon, es 100% tuyo (0)
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

    async function generarPDF(tipo: 'orden' | 'carga' | 'carga-consolidada') {
        ocultarFormula.value = (tipo === 'orden');
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

        if (tipo === 'orden' && form.value.kilosTotales > 1000 && cantidadPalletsUsuario.value > 1) {
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
                await new Promise(r => setTimeout(r, 60)); // Renderizado ultra-rápido

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

    const imprimirDesdeHistorial = async (payload: { orden: any, tipo: string }) => {
        const { orden, tipo } = payload;
        const isConsolidado = tipo === 'carga-consolidada';
        
        try {
            loading.value = true;
            imprimiendoHistorial.value = true;
            
            form.value.esConsolidado = isConsolidado;
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
            form.value.observacion = orden.observacion || '';
            form.value.conBrillo = orden.conBrillo || false;
            form.value.llevaFilm = orden.llevaFilm || false;
            form.value.tipoCorona = orden.tipoCorona || 'Ninguno';
            form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;
            form.value.color = orden.color || orden.colorTexto || '';
            form.value.colorTexto = orden.colorTexto || orden.color || '';
            form.value.Color = orden.color || '';
            
            const desp = Number(orden.desperdicio || 0);
            form.value.merma = desp;
            form.value.kilosTotales = orden.kilos;

            const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

            // ARMADO DE RECETA CON INVENTARIO COMPLETO
            recetaDinamica.value = orden.consumos.map((c: any) => {
                const idBuscado = Number(c.materiaPrimaId || c.id);
                return {
                    id: Math.random(),
                    materiaPrimaId: idBuscado,
                    nombreInsumo: c.nombreMateriaPrima || c.nombreInsumo,
                    cantidad: isConsolidado ? c.cantidadKilos : Number(((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2)),
                    clienteId: determinarDuenioMaterial(idBuscado, c) // 👈 ACÁ MUERE FULANITO
                };
            });

            if (!form.value.esConsolidado && typeof balancearBase === 'function') balancearBase();

            if (tipo === 'orden' && form.value.kilosTotales > 1000) {
                const palletsSugeridos = Math.ceil(form.value.kilosTotales / 1000);
                const respuesta = prompt(`⚠️ Pedido grande (${form.value.kilosTotales} kg).\n¿En cuántos pallets querés dividir la impresión?`, String(palletsSugeridos));
                cantidadPalletsUsuario.value = respuesta ? parseInt(respuesta) : 1;
            } else {
                cantidadPalletsUsuario.value = 1;
            }

            await generarPDF(tipo as any);

            if (tipo === 'orden') {
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
                
                // 🚀 ESTA ES LA CORRECCIÓN EXACTA
                form.value.conBrillo = orden.conBrillo || orden.ConBrillo || false;
                form.value.llevaFilm = orden.llevaFilm || orden.LlevaFilm || false;
                form.value.tipoCorona = orden.tipoCorona || orden.TipoCorona || 'Ninguno';
                form.value.esGofrado = orden.esGofrado || orden.EsGofrado || false;

                form.value.color = orden.color || orden.colorTexto || '';
                form.value.colorTexto = orden.colorTexto || orden.color || '';
                form.value.Color = orden.color || '';
                
                const desp = Number(orden.desperdicio || 0);
                const pesoBrutoTotal = orden.kilos * (1 + (desp / 100));

                if (orden.consumos) {
                    recetaDinamica.value = orden.consumos.map((c: any) => {
                        const idBuscado = Number(c.materiaPrimaId || c.id);
                        return {
                            id: Math.random(),
                            materiaPrimaId: idBuscado,
                            nombreInsumo: c.nombreMateriaPrima || c.nombreInsumo,
                            cantidad: ((c.cantidadKilos / pesoBrutoTotal) * 100).toFixed(2),
                            clienteId: determinarDuenioMaterial(idBuscado, c)
                        };
                    });
                }
                
                if (typeof balancearBase === 'function') balancearBase();
                
                await nextTick();
                await new Promise(r => setTimeout(r, 60)); // Renderizado ultra-rápido

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