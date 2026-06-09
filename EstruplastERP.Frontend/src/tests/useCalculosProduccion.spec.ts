import { describe, it, expect } from 'vitest';
import { ref } from 'vue';
import { useCalculosProduccion } from '@/composables/useCalculosProduccion';

describe('Calculadora de Producción', () => {
    
    it('debería calcular el factor de merma correctamente (ej: 8% = 1.08)', () => {
        // 1. Preparamos los datos falsos (Mock)
        const formMock = ref({ merma: 8 });
        const recetaMock = ref([]);
        const productoMock = ref(null);

        // 2. Ejecutamos tu función
        const { factorMerma } = useCalculosProduccion(formMock, recetaMock, productoMock);

        // 3. Afirmamos el resultado esperado
        expect(factorMerma.value).toBe(1.08);
    });

    it('debería calcular los kilos geométricos correctamente', () => {
        const formMock = ref({ 
            esBobina: false, 
            cantidad: 2, 
            largo: 1000,
            ancho: 1000, 
            espesor: 1,   
            merma: 8
        });
        const recetaMock = ref([]); // Sin receta, usa densidad por defecto (1.1)
        const productoMock = ref({ pesoEspecifico: 1.1, esGenerico: true });

        const { kilosCalculados } = useCalculosProduccion(formMock, recetaMock, productoMock);

        console.log("LOS KILOS CALCULADOS FUERON: ", kilosCalculados.value);
        // Fórmula: 1m * 1m * 1mm * 1.1 (densidad) * 2 (cantidad) = 2.2 kilos
        expect(kilosCalculados.value).toBe(2.2);
    });
});