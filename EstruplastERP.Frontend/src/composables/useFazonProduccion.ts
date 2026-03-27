import type { Ref } from 'vue';

export function useFazonProduccion(
    recetaDinamica: Ref<any[]>,
    listaInventarioCompleto: Ref<any[]>,
    listaLotesCliente: Ref<any[]>,
    loteFazonSeleccionadoId: Ref<string | number>,
    stockFazonDetectado: Ref<number | null>,
    clienteTieneFazonActivo: Ref<boolean>,
    balancearBase: () => void
) {

    const detectarMaterial = (item: any) => {
        if (!item) return '';
        if (item.tipoMaterial && item.tipoMaterial !== 'OTROS') return item.tipoMaterial.toUpperCase();
        
        const n = (item.nombre || item).toString().toUpperCase();
        if (n.includes('PAI') || n.includes('IMPACTO') || n.includes('A.I.')) return 'PAI';
        if (n.includes('PP') || n.includes('POLIPROPILENO')) return 'PP';
        if (n.includes('PEAD') || n.includes('ALTA') || n.includes('HDPE')) return 'PEAD';
        if (n.includes('PEBD') || n.includes('BAJA') || n.includes('LDPE') || n.includes('POLIETILENO')) return 'POLIETILENO';
        if (n.includes('ABS')) return 'ABS';
        if (n.includes('FREON') || n.includes('RESISTENTE')) return 'RESISTENTE FREON';
        if (n.includes('BIO')) return 'BIO';
        return '';
    };

    function aplicarLoteFazonAReceta(lote: any) {
        let itemFazon = recetaDinamica.value.find(r => r.esFazonInput || r.esBase);

        if (itemFazon && lote) {
            itemFazon.materiaPrimaId = lote.id;
            itemFazon.nombreInsumo = `MP: ${lote.nombre}`; 
            itemFazon.densidad = lote.pesoEspecifico || 1;
        } else if (!itemFazon && lote) {
            recetaDinamica.value.push({
                id: Date.now(),
                materiaPrimaId: lote.id,
                nombreInsumo: `MP: ${lote.nombre}`,
                cantidad: 100,
                densidad: lote.pesoEspecifico || 1,
                esBase: true,
                esFazonInput: true
            });
        }

        stockFazonDetectado.value = lote?.stockActual || null;
        balancearBase(); 
    }

    async function actualizarRecetaFazonConCliente(clienteId: string | number, producto: any) {
        listaLotesCliente.value = [];
        loteFazonSeleccionadoId.value = '';

        if (!clienteId || !producto) return;

        const esFazon = producto.esFazon || producto.nombre.toUpperCase().includes('FAZON') || producto.nombre.toUpperCase().includes('SERVICIO');
        if (!esFazon || !clienteTieneFazonActivo.value) return;

        const materialPT = detectarMaterial(producto);

        const todoElStockCliente = listaInventarioCompleto.value.filter((p: any) => {
            const esDelCliente = Number(p.clienteId) === Number(clienteId);
            const tieneStock = p.stockActual > 0;
            const rubro = (p.rubro || '').toUpperCase();
            
            const esMolido = p.esScrap === true || rubro.includes('MOLIDO');

            if (!esDelCliente || !tieneStock || !esMolido) return false;

            if (materialPT) {
                const materialLote = detectarMaterial(p);
                if (materialLote && materialLote !== materialPT) {
                    return false;
                }
            }
            return true;
        });

        listaLotesCliente.value = todoElStockCliente.sort((a, b) => b.stockActual - a.stockActual);

        if (listaLotesCliente.value.length > 0) {
            const mejorOpcion = listaLotesCliente.value[0];
            loteFazonSeleccionadoId.value = mejorOpcion.id;
            aplicarLoteFazonAReceta(mejorOpcion);
        } else {
            const itemFazon = recetaDinamica.value.find(r => r.esFazonInput || r.esBase);
            if (itemFazon) {
                itemFazon.nombreInsumo = "⚠️ CLIENTE SIN MATERIAL RECUPERADO/MOLIDO";
                itemFazon.materiaPrimaId = 0; 
            }
        }
    }

    function alCambiarLoteFazon() {
        const lote = listaLotesCliente.value.find(l => l.id === loteFazonSeleccionadoId.value);
        if (lote) aplicarLoteFazonAReceta(lote);
    }

    return {
        detectarMaterial,
        actualizarRecetaFazonConCliente,
        alCambiarLoteFazon,
        aplicarLoteFazonAReceta
    };
}