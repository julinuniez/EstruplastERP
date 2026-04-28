import { ref } from 'vue';
import api from '@/services/axiosInstance';

export function useModalesInventario(cargarDatos: (forzar?: boolean) => Promise<void>) {
    
    // --- LÓGICA: MODAL DE ALTA DE MATERIA PRIMA MANUAL ---
    const mostrarModalNuevaMP = ref(false);
    // 🚀 AGREGADO: proveedorId inicializado en null
    const nuevaMP = ref({ nombre: '', codigoSku: '', proveedorId: null as number | null });
    const guardandoMP = ref(false);

    const guardarNuevaMateriaPrima = async () => {
        if (!nuevaMP.value.nombre || !nuevaMP.value.codigoSku) {
            return alert("⚠️ El Nombre y el SKU son obligatorios.");
        }
        guardandoMP.value = true;
        try {
            await api.post('/Productos/crear', {
                nombre: nuevaMP.value.nombre.toUpperCase(),
                codigoSku: nuevaMP.value.codigoSku.toUpperCase(),
                proveedorId: nuevaMP.value.proveedorId, // 🚀 AGREGADO: Viaja al backend
                precioCosto: 0,
                stockMinimo: 0,
                receta: []
            });
            alert("✅ Insumo creado correctamente.");
            // 🚀 AGREGADO: Limpiamos la variable completa
            nuevaMP.value = { nombre: '', codigoSku: '', proveedorId: null };
            mostrarModalNuevaMP.value = false;
            
            await cargarDatos(true);
            
        } catch (e: any) {
            const msg = e.response?.data?.mensaje || e.response?.data || "Error de conexión";
            alert("❌ Error al crear: " + msg);
        } finally {
            guardandoMP.value = false;
        }
    };


    // --- LÓGICA: MODAL DE DETALLE DE RESERVAS ---
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
            alert("Error al cargar el detalle de reservas.");
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