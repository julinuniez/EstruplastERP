import { ref } from 'vue';
import api from '@/services/axiosInstance';
import { Alertas } from '@/utils/alertas';

export function useModalesInventario(cargarDatos: (forzar?: boolean) => Promise<void>) {
    
    // --- LÓGICA: MODAL DE ALTA DE MATERIA PRIMA MANUAL ---
    const mostrarModalNuevaMP = ref(false);
    
    // 🚀 CORRECCIÓN: Definimos la estructura completa aquí. 
    // TypeScript ahora sabe que existen estas propiedades.
    const nuevaMP = ref({ 
        nombre: '', 
        codigoSku: '', 
        proveedorId: null as number | null,
        tipoMaterial: '',
        stockActual: 0 
    });
    
    const guardandoMP = ref(false);

    const guardarNuevaMateriaPrima = async () => {
        if (!nuevaMP.value.nombre || !nuevaMP.value.codigoSku) {
            return Alertas.advertencia("⚠️ El Nombre y el SKU son obligatorios.");
        }
        guardandoMP.value = true;
        try {
            // 🚀 ACTUALIZACIÓN: Enviamos también los campos nuevos a la API
            await api.post('/Productos/crear-materia-prima', {
                nombre: nuevaMP.value.nombre,
                codigoSku: nuevaMP.value.codigoSku,
                proveedorId: nuevaMP.value.proveedorId,
                tipoMaterial: nuevaMP.value.tipoMaterial,
                stockActual: nuevaMP.value.stockActual
            });
            
            Alertas.exito("✅ Insumo creado correctamente.");
            
            // Reset del objeto con los nuevos campos
            nuevaMP.value = { nombre: '', codigoSku: '', proveedorId: null, tipoMaterial: '', stockActual: 0 };
            mostrarModalNuevaMP.value = false;
            
            await cargarDatos(true);
            
        } catch (e: any) {
            const msg = e.response?.data?.mensaje || e.response?.data || "Error de conexión";
            Alertas.error("❌ Error al crear: " + msg);
        } finally {
            guardandoMP.value = false;
        }
    };

    const mostrarModalReservas = ref(false);
    const productoSeleccionado = ref<any>(null);
    const ordenesReserva = ref<any[]>([]);
    const cargandoReservas = ref(false);

    const verDetalleReserva = async (producto: any) => {
        productoSeleccionado.value = producto;
        mostrarModalReservas.value = true;
        cargandoReservas.value = true;
        ordenesReserva.value = [];
        
        try {
            const res = await api.get(`/Productos/${producto.id}/reservas`);
            ordenesReserva.value = res.data;
            cargandoReservas.value = false;
        } catch (e) {
            console.error(e);
            cargandoReservas.value = false;
            Alertas.error("Error al cargar el detalle de reservas.");
        }
    };

    return {
        mostrarModalNuevaMP,
        nuevaMP,
        guardandoMP,
        guardarNuevaMateriaPrima,

        mostrarModalReservas,
        productoSeleccionado,
        ordenesReserva,
        cargandoReservas,
        verDetalleReserva
    };
}