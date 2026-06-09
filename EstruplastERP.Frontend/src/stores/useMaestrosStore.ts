import { defineStore } from 'pinia';
import { ref } from 'vue';
import api from '@/services/axiosInstance';

export const useMaestrosStore = defineStore('maestros', () => {
    // 1. ESTADO GLOBAL (Las variables que viven en memoria)
    const productos = ref<any[]>([]);
    const clientes = ref<any[]>([]);
    const cargando = ref(false);
    
    // Esta bandera es la magia del caché: evita volver a consultar a la BD
    const yaCargado = ref(false); 

    // 2. ACCIONES (Las funciones que modifican el estado)
    const cargarDatosMaestros = async (forzarRecarga = false) => {
        // Si ya fuimos a buscar los datos a C# y no nos piden forzar, no hacemos nada.
        if (yaCargado.value && !forzarRecarga) return; 

        cargando.value = true;
        try {
            const [resProd, resCli] = await Promise.all([
                api.get('/Productos'),
                api.get('/Clientes')
            ]);
            
            // Reutilizamos tu lógica de ordenamiento alfabético
            const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
            
            productos.value = Array.isArray(resProd.data) 
                ? resProd.data.sort((a: any, b: any) => getNombre(a).localeCompare(getNombre(b)))
                : [];
                
            clientes.value = Array.isArray(resCli.data) ? resCli.data : [];
            
            yaCargado.value = true; // Marcamos como éxito
        } catch (error) {
            console.error("Error cargando maestros:", error);
        } finally {
            cargando.value = false;
        }
    };

    // Función útil por si alguien crea un producto nuevo y necesitamos refrescar
    const limpiarCache = () => {
        yaCargado.value = false;
        productos.value = [];
        clientes.value = [];
    };

    return { 
        productos, 
        clientes, 
        cargando, 
        yaCargado,
        cargarDatosMaestros,
        limpiarCache
    };
});