import { ref } from 'vue'
import axios from 'axios'
import Swal from 'sweetalert2' // 🚀 Importado para los carteles

// Obtenemos la URL de la API tal como lo haces en tus otros componentes
const apiUrl = import.meta.env.VITE_API_URL || 'http://127.0.0.1:5122/api';

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
                // 🚀 Cambiado de api.post a axios.post con la apiUrl
                const res = await axios.post(`${apiUrl}/Ordenes/registrar-hoja-carga`, ids);
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

            let cantidadPallets = 1;
            if (totalKilosMezcla >= 1100) {
                const result = await Swal.fire({
                    title: 'Dividir en Pallets',
                    text: `La Hoja de Carga (Mezcla) suma ${totalKilosMezcla} kg.\n¿En cuántos pallets deseas dividirla para imprimir las etiquetas de ingreso a stock?`,
                    input: 'number',
                    inputValue: Math.ceil(totalKilosMezcla / 1000), 
                    showCancelButton: true,
                    confirmButtonText: 'Confirmar',
                    cancelButtonText: 'Cancelar',
                    confirmButtonColor: '#3498db',
                    inputValidator: (value) => {
                        if (!value || parseInt(value) <= 0) {
                            return 'Debes ingresar un número válido mayor a 0';
                        }
                    }
                });

                if (!result.isConfirmed) {
                    procesandoCarga.value = false;
                    return null; 
                }

                cantidadPallets = parseInt(result.value);
            }

            const consumosArray = Object.values(recetaConsolidadaMap).sort((a, b) => b.kilos - a.kilos);
            const notasUnicas = Array.from(notasSet);

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
                tipo: 'carga-consolidada',
                sugerenciaPallets: cantidadPallets 
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