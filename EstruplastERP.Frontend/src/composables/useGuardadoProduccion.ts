import { watch } from 'vue';
import type { Ref } from 'vue';
import { ProduccionAPI } from '@/services/produccionService';

const STORAGE_NOTA_PEDIDO_NEXT = 'produccion_notaPedido_siguiente';

export function useGuardadoProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    notaPedidoSugerida: Ref<string>,
    mensaje: Ref<string>,
    error: Ref<string>,
    loading: Ref<boolean>,
    idProduccionGenerada: Ref<boolean>,
    totalPorcentajeReceta: Ref<number>,
    espesorValido: Ref<boolean>,
    limiteMinimo: Ref<number>,
    limiteMaximo: Ref<number>,
    kilosCalculados: Ref<number>,
    colorFinalParaPDF: Ref<string>,
    listaProduccionRef: Ref<any>,
    limpiarBorrador: () => void,
    emit: (evento: 'guardado') => void
) {

    function limpiarFormulario() {
        form.value = {
            productoTerminadoId: '',
            clienteId: '',
            numeroPedidoCliente: '',
            notaPedido: notaPedidoSugerida.value || '',
            cantidad: 1,
            observacion: '',
            largo: 0, ancho: 0, espesor: 0, color: '',
            conBrillo: false, tipoBrillo: '777', porcBrillo: 2.00,
            llevaFilm: false, tipoCorona: 'Ninguno',
            conEstearato: false, esProductoColor: false, masterbatchId: '', colorTexto: '',
            aditivoUV: false, porcentajeUv: 1.00, aditivoCaucho: false, porcentajeCaucho: 1.00,
            aditivoCarga: 0, merma: 8, kilosTotales: 0,
            esConsolidado: false, esBobina: false, kilosPorBobina: 0,
            productoNombre: '',
            clienteNombre: ''
        };
        recetaDinamica.value = [];
        idProduccionGenerada.value = false;
        limpiarBorrador();
    }

    async function registrarProduccion() {
        mensaje.value = '';
        error.value = '';

        if (!form.value.esConsolidado && Math.abs(totalPorcentajeReceta.value - 100) > 0.1) {
            error.value = `⛔ ERROR DE FÓRMULA: La receta suma ${totalPorcentajeReceta.value}%. Debe ajustarla para que dé exactamente 100% antes de guardar.`;
            return;
        }
        if (!espesorValido.value) {
            error.value = `⛔ ERROR DE CALIDAD: El espesor debe estar entre ${limiteMinimo.value} y ${limiteMaximo.value} mm.`;
            return;
        }

        const pesoNetoGeometrico = Number(kilosCalculados.value);
        if (pesoNetoGeometrico <= 0) {
            error.value = "El peso calculado es 0. Revise las medidas.";
            return;
        }
        
        if (!form.value.clienteId) {
            error.value = "⛔ ERROR: Debe seleccionar un Cliente obligatoriamente.";
            return;
        }
        
        const tieneProhibido = recetaDinamica.value.some(r => r.materiaPrimaId === 22);
        if (tieneProhibido) {
            error.value = "⛔ ERROR: Reemplaza el 'Masterbatch Varios' por un color real.";
            return;
        }

        const tieneCero = recetaDinamica.value.some(r => Number(r.materiaPrimaId) === 0);
        if (tieneCero) {
            error.value = "⛔ ERROR: Hay un material en la fórmula sin asignar.";
            return;
        }

        const porcentajeDesperdicio = Number(form.value.merma || 0);
        const factorMultiplicador = 1 + (porcentajeDesperdicio / 100);

        const consumosRealesBrutos = recetaDinamica.value.map(i => {
            const porcentajeEnReceta = parseFloat(i.cantidad.toString()) || 0;
            const kilosInsumoBruto = ((pesoNetoGeometrico * porcentajeEnReceta) / 100) * factorMultiplicador;
            return {
                materiaPrimaId: Number(i.materiaPrimaId),
                cantidadKilos: Number(kilosInsumoBruto.toFixed(3))
            };
        });

        try {
            loading.value = true;
            await ProduccionAPI.registrarNuevaOrden({
                productoTerminadoId: Number(form.value.productoTerminadoId),
                clienteId: Number(form.value.clienteId),
                numeroPedidoCliente: form.value.numeroPedidoCliente || '', 
                notaPedido: form.value.notaPedido || '',
                cantidad: Number(form.value.cantidad),
                observacion: (form.value.observacion || ''),
                kilos: Number(pesoNetoGeometrico.toFixed(3)), 
                desperdicio: porcentajeDesperdicio, 
                esBobina: form.value.esBobina,
                largo: Number(form.value.largo),
                ancho: Number(form.value.ancho),
                espesor: Number(form.value.espesor),
                color: colorFinalParaPDF.value,
                consumos: consumosRealesBrutos,
                conBrillo: form.value.conBrillo,
                llevaFilm: form.value.llevaFilm,
                tipoCorona: form.value.tipoCorona
            });

            mensaje.value = `✅ Orden Generada (Neto: ${pesoNetoGeometrico.toFixed(2)}kg). Insumos con ${porcentajeDesperdicio}% de desperdicio.`;
            idProduccionGenerada.value = true;
            limpiarBorrador(); 
            if (listaProduccionRef.value) listaProduccionRef.value.cargarHistorial();
            emit('guardado');
            limpiarFormulario(); 
            setTimeout(() => { mensaje.value = ''; }, 5000);
        } catch (e: any) {
            error.value = '❌ ' + (e.response?.data?.mensaje || e.message);
        } finally {
            loading.value = false;
        }
    }

    async function cargarNotaPedidoSugerida() {
        try {
            const data = await ProduccionAPI.obtenerOrdenesRecientes();
            let maxNota = 0;

            if (Array.isArray(data) && data.length > 0) {
                const candidatos = data
                    .map((o: any) => o?.notaPedido ?? o?.numeroPedidoCliente ?? o?.id)
                    .map((v: any) => Number(v))
                    .filter((n: number) => !isNaN(n) && n > 0);

                if (candidatos.length > 0) {
                    maxNota = Math.max(...candidatos);
                }
            }

            notaPedidoSugerida.value = maxNota > 0 ? String(maxNota) : '';
            const correlativo = maxNota > 0 ? maxNota + 1 : 1;
            localStorage.setItem(STORAGE_NOTA_PEDIDO_NEXT, String(correlativo));

            if (!form.value.notaPedido || String(form.value.notaPedido).trim() === '') {
                form.value.notaPedido = String(correlativo);
            }
        } catch (e) {
            const nextGuardadoRaw = localStorage.getItem(STORAGE_NOTA_PEDIDO_NEXT);
            const nextGuardado = nextGuardadoRaw ? Number(nextGuardadoRaw) : NaN;
            if (!isNaN(nextGuardado) && nextGuardado > 0) {
                notaPedidoSugerida.value = String(Math.trunc(nextGuardado) - 1); 
                if (!form.value.notaPedido || String(form.value.notaPedido).trim() === '') {
                    form.value.notaPedido = String(Math.trunc(nextGuardado));
                }
            } else {
                notaPedidoSugerida.value = '';
            }
        }
    }

    function aplicarNotaPedidoSugerida() {
        if (notaPedidoSugerida.value) form.value.notaPedido = notaPedidoSugerida.value;
    }

    watch(
        () => form.value.notaPedido,
        (v) => {
            const num = Number(v);
            if (!isNaN(num) && num > 0) {
                const anterior = Math.trunc(num) - 1;
                if (anterior > 0) {
                    notaPedidoSugerida.value = String(anterior);
                }
                localStorage.setItem(STORAGE_NOTA_PEDIDO_NEXT, String(num));
            }
        }
    );
    
    return {
        limpiarFormulario,
        registrarProduccion,
        cargarNotaPedidoSugerida,
        aplicarNotaPedidoSugerida
    };
}