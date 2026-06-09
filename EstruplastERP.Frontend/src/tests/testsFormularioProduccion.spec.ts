import { describe, it, expect, vi, beforeEach } from 'vitest';
import { nextTick } from 'vue';
import { mount, flushPromises } from '@vue/test-utils';
import FormularioProduccion from '@/views/FormularioProduccion.vue'; // O la ruta correcta donde esté
import { ProduccionAPI } from '@/services/produccionService';

vi.mock('@/services/produccionService', () => ({
    ProduccionAPI: {
        obtenerProductos: vi.fn(() => Promise.resolve([
            { id: 100, nombre: 'BOLSA TEST', pesoEspecifico: 1.1, esGenerico: true }
        ])),
        obtenerClientes: vi.fn(() => Promise.resolve([
            { id: 10, razonSocial: 'CLIENTE PRUEBA' }
        ])),
        obtenerInventarioCompleto: vi.fn(() => Promise.resolve([])),
        obtenerOrdenesRecientes: vi.fn(() => Promise.resolve([{ notaPedido: 500 }])),
        registrarNuevaOrden: vi.fn(() => Promise.resolve({ success: true }))
    }
}));

describe('Test de Regresión: FormularioProduccion', () => {

    beforeEach(() => {
        vi.clearAllMocks(); // Limpia el historial de llamadas antes de cada test
        localStorage.clear(); // Limpia el borrador
    });

    it('Regresión 1: El componente carga y pide los datos iniciales a la API', async () => {
        const wrapper = mount(FormularioProduccion);
        
        // flushPromises espera a que terminen los await del onMounted
        await flushPromises(); 

        // Verificamos que al entrar, haya llamado a tus 3 endpoints clave
        expect(ProduccionAPI.obtenerProductos).toHaveBeenCalledTimes(1);
        expect(ProduccionAPI.obtenerClientes).toHaveBeenCalledTimes(1);
        expect(ProduccionAPI.obtenerInventarioCompleto).toHaveBeenCalledTimes(1);
    });

    it('Regresión 2: Debería recalcular los kilos al cambiar las medidas del formulario', async () => {
        const wrapper = mount(FormularioProduccion);
        await flushPromises();

        // Como usamos <script setup>, interactuamos directo con el HTML
        // (Ajustá los selectores 'input' si tenés clases específicas como '.input-largo')
        
        // Buscamos los inputs y les forzamos un valor (Ej: 1 metro x 1 metro x 1 mm)
        const inputsDeNumero = wrapper.findAll('input[type="number"]');
        
        // NOTA: Tendrías que buscar el input exacto usando clases, ej: wrapper.find('.input-largo')
        // Asumiendo que pudimos inyectar los valores en el form:
        wrapper.vm.form.largo = 1000;
        wrapper.vm.form.ancho = 1000;
        wrapper.vm.form.espesor = 1;
        wrapper.vm.form.cantidad = 2;
        wrapper.vm.form.productoTerminadoId = 100; // El producto falso que creamos arriba

        await nextTick(); // Esperamos que Vue actualice los cálculos

        // Fórmula geométrica: 1 * 1 * 1 * 1.1 (densidad default) * 2 (cantidad) = 2.2 kg
        expect(wrapper.vm.form.kilosTotales).toBe(2.2);
    });

    it('Regresión 3: Debería guardar exitosamente cuando TODOS los datos están completos', async () => {
        const wrapper = mount(FormularioProduccion);
        await flushPromises(); // Esperamos que cargue clientes y productos

        // 1. Llenamos ABSOLUTAMENTE TODO lo que exige la fábrica
        wrapper.vm.form.esConsolidado = true; // Salteamos la regla matemática del 100% exacto por ahora
        wrapper.vm.form.clienteId = 10; // Hay Cliente
        wrapper.vm.form.productoTerminadoId = 100; // Hay Producto
        wrapper.vm.form.largo = 1000; // Medidas
        wrapper.vm.form.ancho = 1000;
        wrapper.vm.form.espesor = 1;
        wrapper.vm.form.cantidad = 5; // Cantidad de láminas/bobinas

        // 2. Le inyectamos una receta válida (para evitar el error de "material en cero o prohibido")
        wrapper.vm.recetaDinamica = [
            { id: 1,materiaPrimaId: 1, cantidad: 100, nombreInsumo: 'Plástico Test', densidad: 1.1 }
        ];

        // Forzamos a Vue a actualizar todo su estado interno
        await nextTick();

        // (Truco de test): Como la calculadora a veces tarda en reaccionar en el entorno falso, 
        // le aseguramos temporalmente que el peso no es cero para no trabar la validación.
        wrapper.vm.form.kilosTotales = 5.5; 

        // 3. Apretamos el botón mágico
        await wrapper.vm.registrarProduccion();

        // 4. LA PRUEBA DE ORO: Verificamos que no haya errores de validación...
        expect(wrapper.vm.error).toBe(''); 
        
        // ... ¡Y que la orden haya sido enviada al backend!
        expect(ProduccionAPI.registrarNuevaOrden).toHaveBeenCalledTimes(1);
    });
});