import api from '@/services/axiosInstance';

// Definimos la estructura básica para el payload de la orden
export interface PayloadOrdenNueva {
    productoTerminadoId: number;
    clienteId: number;
    numeroPedidoCliente: string;
    notaPedido: string;
    cantidad: number;
    observacion: string;
    kilos: number;
    desperdicio: number;
    esBobina: boolean;
    largo: number;
    ancho: number;
    espesor: number;
    color: string;
    consumos: any[];
    conBrillo: boolean;
    llevaFilm: boolean;
    esGofrado: boolean;
    aditivoUV: boolean;
    tipoCorona: string;
    porcBrillo?: number;
    porcentajeUv?: number;
    aditivoCaucho?: boolean;
    porcentajeCaucho?: number;
}

export const ProduccionAPI = {
    // 1. Cargas Iniciales (Combos)
    async obtenerClientes() {
        const { data } = await api.get('/Clientes');
        return data;
    },
    
    async obtenerInventarioCompleto() {
        const { data } = await api.get('/Productos/inventario-completo');
        return data;
    },

    // 2. Productos
    async obtenerProductos(clienteId: string | number = '') {
        const query = clienteId ? `?clienteId=${clienteId}` : '';
        const { data } = await api.get(`/Productos${query}`);
        return data;
    },

    async obtenerProductoPorId(id: number) {
        const { data } = await api.get(`/Productos/${id}`);
        return data;
    },

    // 3. Órdenes
    async obtenerOrdenesRecientes() {
        const { data } = await api.get('/Ordenes/recientes');
        return data;
    },

    async registrarNuevaOrden(payload: PayloadOrdenNueva) {
        const { data } = await api.post('/Ordenes', payload);
        return data;
    },

    async marcarOrdenImpresa(id: number) {
        const { data } = await api.post(`/Ordenes/marcar-impresa/${id}`);
        return data;
    }
};