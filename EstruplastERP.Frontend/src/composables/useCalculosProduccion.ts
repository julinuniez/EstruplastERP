import { computed } from 'vue';
import type { Ref } from 'vue'; 

const DENSIDAD_DEFAULT = 1.1;

export function useCalculosProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    productoSeleccionado: Ref<any>
) {
    // 1. Suma de porcentajes de la receta
    const totalPorcentajeReceta = computed(() => {
        return parseFloat(recetaDinamica.value.reduce((acc, item) => 
            acc + (parseFloat(item.cantidad.toString()) || 0), 0
        ).toFixed(2));
    });

    // 2. Cálculo de la densidad de la mezcla
    const densidadMezcla = computed(() => {
        if (recetaDinamica.value.length === 0) return productoSeleccionado.value?.pesoEspecifico || DENSIDAD_DEFAULT;
        let suma = 0, porc = 0;
        recetaDinamica.value.forEach(item => {
            const p = parseFloat(item.cantidad.toString()) || 0;
            const d = parseFloat(item.densidad?.toString()) || DENSIDAD_DEFAULT;
            suma += (p * d); 
            porc += p;
        });
        return porc === 0 ? DENSIDAD_DEFAULT : (suma / porc);
    });

    // 3. Cálculo de Kilos (Fórmula geométrica o por bobina)
    const kilosCalculados = computed(() => {
        if (form.value.esConsolidado) return Number(form.value.kilosTotales);
        if (!productoSeleccionado.value) return 0;
        
        const Cant = Number(form.value.cantidad) || 1;

        if (form.value.esBobina) {
            return parseFloat(((Number(form.value.kilosPorBobina) || 0) * Cant).toFixed(4));
        }
        
        const L = (Number(form.value.largo) || 0) / 1000; 
        const A = (Number(form.value.ancho) || 0) / 1000; 
        const E = Number(form.value.espesor) || 0;        
        const Dens = Number(densidadMezcla.value);
        
        return parseFloat((L * A * E * Dens * Cant).toFixed(4));
    });

    // 4. Factor de merma (Ej: 8% = 1.08)
    const factorMerma = computed(() => 1 + (Number(form.value.merma || 0) / 100));

    // Devolvemos lo que el componente principal necesita usar
    return {
        totalPorcentajeReceta,
        densidadMezcla,
        kilosCalculados,
        factorMerma
    };
}