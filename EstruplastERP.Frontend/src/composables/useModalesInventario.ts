import { ref } from 'vue';
import api from '@/services/axiosInstance';
import { Alertas } from '@/utils/alertas';

export function useModalesInventario(cargarDatos: (forzar?: boolean) => Promise<void>) {
    
    // --- LÓGICA: MODAL DE ALTA DE MATERIA PRIMA MANUAL ---
    const mostrarModalNuevaMP = ref(false);
    // 🚀 AGREGADO: proveedorId inicializado en null
    const nuevaMP = ref({ nombre: '', codigoSku: '', proveedorId: null as number | null });
    const guardandoMP = ref(false);

    const guardarNuevaMateriaPrima = async () => {
        if (!nuevaMP.value.nombre || !nuevaMP.value.codigoSku) {
            return Alertas.advertencia("⚠️ El Nombre y el SKU son obligatorios.");
        }
        guardandoMP.value = true;
        try {
            // 🚀 APUNTAMOS AL ENDPOINT NUEVO
            await api.post('/Productos/crear-materia-prima', {
                nombre: nuevaMP.value.nombre,
                codigoSku: nuevaMP.value.codigoSku,
                proveedorId: nuevaMP.value.proveedorId
            });
            
            Alertas.exito("✅ Insumo creado correctamente.");
            nuevaMP.value = { nombre: '', codigoSku: '', proveedorId: null };
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