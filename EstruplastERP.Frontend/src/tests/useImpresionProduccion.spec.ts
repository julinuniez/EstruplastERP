import { describe, it, expect, vi, beforeEach } from 'vitest';
import { ref } from 'vue';
import { useImpresionProduccion } from '@/composables/useImpresionProduccion';
import { ProduccionAPI } from '@/services/produccionService';

// 1. MOCK DE HTML2PDF: Evitamos que la consola intente renderizar un PDF real
vi.mock('html2pdf.js', () => {
    return {
        default: () => ({
            set: vi.fn().mockReturnThis(),
            from: vi.fn().mockReturnThis(),
            save: vi.fn().mockResolvedValue(true)
        })
    };
});

// 2. MOCK DE LA API: Evitamos que marque órdenes como impresas en tu base de datos real
vi.mock('@/services/produccionService', () => ({
    ProduccionAPI: {
        marcarOrdenImpresa: vi.fn(() => Promise.resolve())
    }
}));

describe('Pruebas de Impresión y Cálculos de Hoja de Carga', () => {
    
    // Preparamos las herramientas (Refs) vacías antes de cada test
    let form: any, recetaDinamica: any, ocultarFormula: any, imprimiendoHistorial: any;
    let cantidadPalletsUsuario: any, mensaje: any, error: any, loading: any, listaProduccionRef: any;
    let balancearBaseMock: any, limpiarFormularioMock: any;

    beforeEach(() => {
        vi.clearAllMocks();
        form = ref({ kilosTotales: 0, cantidad: 1, observacion: '' });
        recetaDinamica = ref([]);
        ocultarFormula = ref(false);
        imprimiendoHistorial = ref(false);
        cantidadPalletsUsuario = ref(1);
        mensaje = ref('');
        error = ref('');
        loading = ref(false);
        listaProduccionRef = ref({ cargarHistorial: vi.fn() });
        balancearBaseMock = vi.fn();
        limpiarFormularioMock = vi.fn();

        // Simulamos el elemento HTML que necesita html2pdf para no dar error
        document.body.innerHTML = '<div id="hoja-de-impresion"><input value="test" /></div>';
    });

    it('Impresión Simple: Debería cargar medidas, notas y color correctamente en el formulario', async () => {
        const { imprimirDesdeHistorial } = useImpresionProduccion(
            form, recetaDinamica, ocultarFormula, imprimiendoHistorial, cantidadPalletsUsuario,
            mensaje, error, loading, listaProduccionRef, balancearBaseMock, limpiarFormularioMock
        );

        // 1. Armamos el MOCK usando exactamente las propiedades que lee tu función
        const ordenFalsaCompleta = {
            id: 888,
            productoId: 10,
            clienteId: 5,
            kilos: 100, 
            desperdicio: 10, // Tu código busca "desperdicio" y lo guarda en "merma"
            largo: 1200,
            ancho: 1000,
            espesor: 2.5,
            notaPedido: 'NP-2026-X',
            numeroPedidoCliente: 'OC-999888',
            color: 'Azul Francia',
            conBrillo: true,
            consumos: [
                { materiaPrimaId: 1, nombreMateriaPrima: 'PEAD BASE', cantidadKilos: 55 }
            ]
        };

        // 2. LA SOLUCIÓN: Le pasamos el objeto con la estructura { orden, tipo }
        await imprimirDesdeHistorial({ 
            orden: ordenFalsaCompleta, 
            tipo: 'orden' 
        }); 

        // 3. Verificamos que tu código haya llenado el "form"
        expect(form.value.notaPedido).toBe('NP-2026-X');
        expect(form.value.numeroPedidoCliente).toBe('OC-999888');
        expect(form.value.largo).toBe(1200);
        expect(form.value.ancho).toBe(1000);
        expect(form.value.espesor).toBe(2.5);
        expect(form.value.color).toBe('Azul Francia');
        expect(form.value.conBrillo).toBe(true);
        expect(form.value.merma).toBe(10); // Entró 10 en desperdicio, debe estar en merma
        
        // Verificamos que cargó bien la receta dinámica para la máquina
        expect(recetaDinamica.value.length).toBeGreaterThan(0);
        expect(recetaDinamica.value[0].nombreInsumo).toBe('PEAD BASE');
    });

    it('Impresión Lote Múltiple: Debería procesar varias órdenes sin mezclar los kilos', async () => {
        const { imprimirLoteOPsDesdeHistorial } = useImpresionProduccion(
            form, recetaDinamica, ocultarFormula, imprimiendoHistorial, cantidadPalletsUsuario,
            mensaje, error, loading, listaProduccionRef, balancearBaseMock, limpiarFormularioMock
        );

        const loteOrdenesFalsas = [
            { id: 101, kilos: 50, desperdicio: 5, consumos: [] },
            { id: 102, kilos: 200, desperdicio: 8, consumos: [] }
        ];

        await imprimirLoteOPsDesdeHistorial(loteOrdenesFalsas);

        // Verificamos que se haya llamado a la API para marcar AMBAS como impresas
        expect(ProduccionAPI.marcarOrdenImpresa).toHaveBeenCalledTimes(2);
        expect(ProduccionAPI.marcarOrdenImpresa).toHaveBeenCalledWith(101);
        expect(ProduccionAPI.marcarOrdenImpresa).toHaveBeenCalledWith(102);
        
        // Verificamos que el mensaje de éxito final se haya asignado
        expect(mensaje.value).toBe("✅ Lote generado con éxito");
    });
});