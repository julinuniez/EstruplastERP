import { ref } from 'vue'
import api from '@/services/axiosInstance'

// Helpers privados (no se exportan, solo los usa el composable)
function normalizarNombreFamilia(nombre: any) {
    if (!nombre || typeof nombre !== 'string') return '';
    let n = nombre.toUpperCase().trim();
    const prefijos = ['FAZON -', 'FAZON-', 'FAZON', 'SERVICIO DE FAZON -', 'SERVICIO DE FAZON'];
    for (const pref of prefijos) {
        if (n.startsWith(pref)) {
            n = n.substring(pref.length).trim();
            break;
        }
    }
    n = n.replace(/FAZON/g, '').replace(/SERVICIO/g, '').replace(/LAMINADO/g, '').replace(/-/g, '').trim();
    return n.replace(/\s+/g, ' '); 
}

function obtenerNombreClienteReal(orden: any) {
    if (typeof orden.cliente === 'string' && orden.cliente.trim() !== '') return orden.cliente;
    if (orden.clienteNombre && typeof orden.clienteNombre === 'string' && orden.clienteNombre.trim() !== '') return orden.clienteNombre;
    if (orden.cliente && typeof orden.cliente === 'object' && orden.cliente.razonSocial) return orden.cliente.razonSocial;
    return 'Desconocido';
}

// Exportamos la función principal
export function useConsolidacion() {
    const procesandoCarga = ref(false);

    const procesarConsolidacion = async (ordenesAImprimir: any[]) => {
        if (!ordenesAImprimir || ordenesAImprimir.length < 2) return null;

        procesandoCarga.value = true;
        try {
            const familiaBase = normalizarNombreFamilia(ordenesAImprimir[0]?.producto || '');
            
            // 1. Llamada a la API para registrar el lote
            let codigoHojaCarga = "MIX";
            try {
                const ids = ordenesAImprimir.map(o => o.id);
                const res = await api.post('/Ordenes/registrar-hoja-carga', ids);
                codigoHojaCarga = res.data.codigo; 
            } catch (e) {
                console.error("No se pudo registrar la hoja de carga en la API", e);
            }

            // 2. Lógica matemática de agrupación
            let totalKilosMezcla = 0;
            const recetaConsolidadaMap: Record<string, any> = {};
            const notasSet = new Set<string>();

            ordenesAImprimir.forEach(orden => {
                const refPedido = orden.notaPedido ? String(orden.notaPedido) : String(orden.id);
                const nombreCliente = obtenerNombreClienteReal(orden);
                
                notasSet.add(refPedido);

                if (orden.consumos && Array.isArray(orden.consumos)) {
                    orden.consumos.forEach((consumo: any) => {
                        const mpId = consumo.materiaPrimaId;
                        const nombreMP = (consumo.nombreMateriaPrima || 'Insumo').toUpperCase();
                        const esFazon = nombreMP.includes('MOLIDO') || nombreMP.includes('RECUPERADO') || nombreMP.includes('FAZON');
                        const mapKey = esFazon ? `${mpId}-${refPedido}` : `${mpId}`;

                        if (!recetaConsolidadaMap[mapKey]) {
                            let tituloInsumo = consumo.nombreMateriaPrima || 'Insumo';
                            
                            if (esFazon) {
                                const clId = orden.clienteId || 0;
                                if (clId > 0 && nombreCliente && !nombreCliente.toUpperCase().includes('STOCK')) {
                                    tituloInsumo = `${tituloInsumo} (De ${nombreCliente})`;
                                } else {
                                    tituloInsumo = `${tituloInsumo} (Stock Estruplast)`;
                                }
                            }

                            recetaConsolidadaMap[mapKey] = { 
                                id: mpId, 
                                nombre: tituloInsumo, 
                                kilos: 0,
                                clienteId: esFazon ? (orden.clienteId || 0) : 0,
                                clienteNombreFazon: esFazon ? nombreCliente : null
                            };
                        }
                        
                        const kilosItem = (consumo.cantidadKilos || 0);
                        recetaConsolidadaMap[mapKey].kilos += kilosItem;
                        totalKilosMezcla += kilosItem;
                    });
                }
            });

            // 3. Mapeo final ("El Caballo de Troya")
            const consumosArray = Object.values(recetaConsolidadaMap).sort((a, b) => b.kilos - a.kilos);
            const notasUnicas = Array.from(notasSet);

            const consumosMapeados = consumosArray.map(c => ({
                materiaPrimaId: c.id,
                nombreMateriaPrima: c.nombre, 
                nombreInsumo: c.nombre,
                cantidadKilos: c.kilos,
                cantidad: c.kilos,
                clienteId: c.clienteId || 0,                 
                clienteNombreFazon: c.clienteNombreFazon     
            }));

            const ordenConsolidadaFalsa = {
                id: 999999, 
                notaPedido: notasUnicas.join(' / '),
                numeroPedidoCliente: familiaBase, 
                producto: familiaBase,
                kilosTotales: totalKilosMezcla,
                kilosEstimados: totalKilosMezcla,
                kilos: totalKilosMezcla, 
                cantidad: 0, largo: 0, ancho: 0, espesor: 0, desperdicio: 0,
                observacion: `[LOTE: ${codigoHojaCarga}] MEZCLA CONSOLIDADA: Pedidos #${notasUnicas.join(', #')}`,
                consumos: consumosMapeados
            };

            // Retornamos el paquete listo para emitirse
            return { 
                orden: ordenConsolidadaFalsa, 
                receta: consumosMapeados, 
                tipo: 'carga-consolidada' 
            };

        } finally {
            procesandoCarga.value = false;
        }
    };

    return {
        procesandoCarga,
        procesarConsolidacion
    };
}