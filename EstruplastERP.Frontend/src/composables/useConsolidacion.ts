import { ref } from 'vue'
import api from '@/services/axiosInstance'

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

export function useConsolidacion() {
    const procesandoCarga = ref(false);

    const procesarConsolidacion = async (ordenesAImprimir: any[]) => {
        if (!ordenesAImprimir || ordenesAImprimir.length < 2) return null;

        procesandoCarga.value = true;
        try {
            const familiaBase = normalizarNombreFamilia(ordenesAImprimir[0]?.producto || '');
            
            let codigoHojaCarga = "MIX";
            try {
                const ids = ordenesAImprimir.map(o => o.id);
                const res = await api.post('/Ordenes/registrar-hoja-carga', ids);
                codigoHojaCarga = res.data.codigo; 
            } catch (e) {
                console.error("No se pudo registrar la hoja de carga en la API", e);
            }

            let totalKilosMezcla = 0;
            const recetaConsolidadaMap: Record<string, any> = {};
            const notasSet = new Set<string>();

            ordenesAImprimir.forEach(orden => {
                const refPedido = orden.notaPedido ? String(orden.notaPedido) : String(orden.id);
                notasSet.add(refPedido);

                if (orden.consumos && Array.isArray(orden.consumos)) {
                    orden.consumos.forEach((consumo: any) => {
                        const mpId = consumo.materiaPrimaId;
                        const mapKey = `${mpId}`;

                        if (!recetaConsolidadaMap[mapKey]) {
                            recetaConsolidadaMap[mapKey] = { 
                                id: mpId, 
                                nombre: consumo.nombreMateriaPrima || 'Insumo', 
                                kilos: 0,
                                clienteId: Number(consumo.clienteId || consumo.ClienteId || 0),
                                clienteNombre: consumo.clienteNombre || consumo.ClienteNombre || '' 
                            };
                        } else {
                            // 🚀 LA MAGIA: Si el material ya estaba en 0, pero esta nueva orden SÍ tiene el cliente, lo rescatamos!
                            const idActual = Number(consumo.clienteId || consumo.ClienteId || 0);
                            if (idActual > 1 && Number(recetaConsolidadaMap[mapKey].clienteId) <= 1) {
                                recetaConsolidadaMap[mapKey].clienteId = idActual;
                                recetaConsolidadaMap[mapKey].clienteNombre = consumo.clienteNombre || consumo.ClienteNombre || '';
                            }
                        }
                        
                        const kilosItem = Number(consumo.cantidadKilos || consumo.CantidadKilos || 0);
                        recetaConsolidadaMap[mapKey].kilos += kilosItem;
                        totalKilosMezcla += kilosItem;
                    });
                }
            });

            const consumosArray = Object.values(recetaConsolidadaMap).sort((a, b) => b.kilos - a.kilos);
            const notasUnicas = Array.from(notasSet);

            // 🚀 Ahora el mapeo se transfiere intacto al PDF
            const consumosMapeados = consumosArray.map(c => ({
                materiaPrimaId: c.id,
                nombreMateriaPrima: c.nombre, 
                nombreInsumo: c.nombre,
                cantidadKilos: c.kilos,
                cantidad: c.kilos,
                clienteId: c.clienteId,
                clienteNombre: c.clienteNombre
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