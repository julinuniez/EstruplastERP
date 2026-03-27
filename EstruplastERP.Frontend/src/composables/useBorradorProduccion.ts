import { watch } from 'vue';
import type { Ref } from 'vue';

const STORAGE_KEY = 'produccion_borrador';

export function useBorradorProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    mensaje: Ref<string>
) {
    // 1. Guardado automático (Escucha cambios en el form y la receta)
    watch(
        [form, recetaDinamica], 
        ([nuevoForm, nuevaReceta]) => {
            const borrador = {
                form: nuevoForm,
                receta: nuevaReceta,
                timestamp: Date.now()
            };
            localStorage.setItem(STORAGE_KEY, JSON.stringify(borrador));
        },
        { deep: true }
    );

    // 2. Limpiar el caché
    const limpiarBorrador = () => {
        localStorage.removeItem(STORAGE_KEY);
    };

    // 3. Revisar si hay un trabajo pendiente al entrar
    const revisarYRecuperarBorrador = () => {
        const borradorGuardado = localStorage.getItem(STORAGE_KEY);
        if (borradorGuardado) {
            try {
                const datos = JSON.parse(borradorGuardado);
                const unDia = 24 * 60 * 60 * 1000; // 24 horas
                
                if (Date.now() - datos.timestamp < unDia) {
                    if (confirm("📝 Encontré un trabajo sin terminar. ¿Quieres recuperarlo?")) {
                        Object.assign(form.value, datos.form);
                        recetaDinamica.value = datos.receta;
                        
                        setTimeout(() => {
                            form.value.largo = datos.form.largo;
                            form.value.ancho = datos.form.ancho;
                            form.value.espesor = datos.form.espesor;
                            form.value.cantidad = datos.form.cantidad;
                            form.value.observacion = datos.form.observacion;
                            form.value.kilosTotales = datos.form.kilosTotales;
                            form.value.colorTexto = datos.form.colorTexto || '';
                                
                            mensaje.value = "📝 Datos y medidas recuperados con éxito.";
                        }, 1500);
                    } else {
                        limpiarBorrador();
                    }
                } else {
                    // Si es más viejo que 24hs, lo descarta solo
                    limpiarBorrador();
                }
            } catch (e) {
                console.error("Error leyendo borrador", e);
                limpiarBorrador();
            }
        }
    };

    return {
        limpiarBorrador,
        revisarYRecuperarBorrador
    };
}