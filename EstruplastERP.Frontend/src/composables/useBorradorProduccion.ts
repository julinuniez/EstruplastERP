import { ref, watch } from 'vue';

export function useBorradorProduccion(form: any, recetaDinamica: any, mensaje: any) {
    const borradorDisponible = ref(false);

    const revisarBorrador = () => {
        const guardado = localStorage.getItem('produccion_borrador');
        if (guardado) {
            borradorDisponible.value = true;
        }
    };

    const recuperarBorrador = () => {
        try {
            const guardado = localStorage.getItem('produccion_borrador');
            if (guardado) {
                const data = JSON.parse(guardado);
                Object.assign(form.value, data.form);
                recetaDinamica.value = data.receta;
                mensaje.value = '✅ Borrador recuperado con éxito.';
            }
        } catch (e) {
            console.error(e);
        } finally {
            borradorDisponible.value = false;
        }
    };

    const limpiarBorrador = () => {
        localStorage.removeItem('produccion_borrador');
        borradorDisponible.value = false;
    };

    const descartarBorrador = () => {
        limpiarBorrador();
    };

    watch([form, recetaDinamica], () => {
        if (form.value.clienteId) {
            const dataToSave = { form: form.value, receta: recetaDinamica.value };
            localStorage.setItem('produccion_borrador', JSON.stringify(dataToSave));
        }
    }, { deep: true });

    return {
        borradorDisponible,
        revisarBorrador,
        recuperarBorrador,
        descartarBorrador,
        limpiarBorrador
    };
}