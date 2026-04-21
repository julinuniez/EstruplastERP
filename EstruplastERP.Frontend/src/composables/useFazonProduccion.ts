import { watch } from 'vue';
import type { Ref } from 'vue';

export function useFazonProduccion(
    recetaDinamica: Ref<any[]>,
    listaInventarioCompleto: Ref<any[]>,
    listaTodasMateriasPrimas: Ref<any[]>,
    listaLotesCliente: Ref<any[]>,
    loteFazonSeleccionadoId: Ref<string | number>,
    stockFazonDetectado: Ref<number | null>,
    clienteTieneFazonActivo: Ref<boolean>,
    balancearBase: () => void
) {
    watch(loteFazonSeleccionadoId, (newId) => {
        if (!newId) return;
        const loteIdStr = String(newId);
        const lote = listaLotesCliente.value.find(l => String(l.id) === loteIdStr);
        if (lote) aplicarLoteFazonAReceta(lote);
    });

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
        let itemFazon = recetaDinamica.value.find(r => r.esFazonInput || String(r.nombreInsumo).includes('CAJA VERDE') || String(r.nombreInsumo).includes('ELIJA'));
        
        const cId = Number(lote.clienteId || lote.ClienteId) || 0;

        if (itemFazon && lote) {
            itemFazon.materiaPrimaId = lote.id;
            itemFazon.nombreInsumo = `MP: ${lote.nombre}`; 
            itemFazon.densidad = lote.pesoEspecifico || 1;
            itemFazon.clienteId = cId;
            itemFazon.esFazonInput = true;
        } else if (!itemFazon && lote) {
            let itemBaseOriginal = recetaDinamica.value.find(r => r.esBase);

            if (itemBaseOriginal) {
                itemBaseOriginal.materiaPrimaId = lote.id;
                itemBaseOriginal.nombreInsumo = `MP: ${lote.nombre}`;
                itemBaseOriginal.densidad = lote.pesoEspecifico || 1;
                itemBaseOriginal.clienteId = cId;
                itemBaseOriginal.esFazonInput = true; 
            } else {
                recetaDinamica.value.push({
                    id: 'fazon_' + Date.now(),
                    materiaPrimaId: lote.id,
                    nombreInsumo: `MP: ${lote.nombre}`,
                    cantidad: 50,
                    densidad: lote.pesoEspecifico || 1,
                    esBase: false, 
                    esFazonInput: true,
                    clienteId: cId
                });
            }
        }
        stockFazonDetectado.value = lote?.stockActual || null;
        balancearBase(); 
    }

    async function actualizarRecetaFazonConCliente(clienteId: string | number, producto: any) {
        if (!clienteId || !producto) return;

        // 🚀 Limpiamos el caché atascado
        listaLotesCliente.value = [];

        const esFazon = producto.esFazon || String(producto.nombre).toUpperCase().includes('FAZON') || String(producto.nombre).toUpperCase().includes('SERVICIO');
        if (!esFazon || !clienteTieneFazonActivo.value) return;

        const materialPT = detectarMaterial(producto);

        const todoElStockCliente = listaInventarioCompleto.value.filter((p: any) => {
            const cId = Number(p.clienteId || p.ClienteId) || 0;
            const esDelCliente = cId === Number(clienteId);
            const rubro = (p.rubro || '').toUpperCase();
            const esMolido = p.esScrap === true || rubro.includes('MOLIDO');

            if (!esDelCliente || !esMolido) return false;

            if (materialPT) {
                const materialLote = detectarMaterial(p);
                if (materialLote && materialLote !== materialPT) {
                    return false;
                }
            }
            return true;
        });

        listaLotesCliente.value = todoElStockCliente.sort((a, b) => b.stockActual - a.stockActual);

        const cIdNum = Number(clienteId);
        const materialYaCargado = recetaDinamica.value.find(r => {
            const tieneIdValido = r.materiaPrimaId > 0;
            const noEsAlerta = !String(r.nombreInsumo).includes('CAJA VERDE') && !String(r.nombreInsumo).includes('ELIJA');
            
            const mpMaestro = listaInventarioCompleto.value.find((m: any) => m.id === r.materiaPrimaId) || 
                              listaTodasMateriasPrimas.value.find((m: any) => m.id === r.materiaPrimaId);
            
            const duenioReal = mpMaestro ? Number(mpMaestro.clienteId || mpMaestro.ClienteId || 0) : Number(r.clienteId || 0);

            const esDelCliente = (duenioReal === cIdNum) || String(r.nombreInsumo).toUpperCase().includes('DE ');

            return tieneIdValido && noEsAlerta && esDelCliente;
        });

        if (materialYaCargado) {
            loteFazonSeleccionadoId.value = materialYaCargado.materiaPrimaId;
            materialYaCargado.esFazonInput = true; 
            
            const existeEnCombo = listaLotesCliente.value.find((l: any) => l.id === materialYaCargado.materiaPrimaId);
            if (!existeEnCombo) {
                const mpPerdida = listaInventarioCompleto.value.find((m: any) => m.id === materialYaCargado.materiaPrimaId) || 
                                  listaTodasMateriasPrimas.value.find((m: any) => m.id === materialYaCargado.materiaPrimaId);
                if (mpPerdida) listaLotesCliente.value.push(mpPerdida);
            }
            return; 
        }

        loteFazonSeleccionadoId.value = '';

        if (listaLotesCliente.value.length === 1) {
            const unicaOpcion = listaLotesCliente.value[0];
            loteFazonSeleccionadoId.value = unicaOpcion.id;
            aplicarLoteFazonAReceta(unicaOpcion);
        } else if (listaLotesCliente.value.length > 1) {
            let itemFazon = recetaDinamica.value.find(r => r.esFazonInput || String(r.nombreInsumo).includes('CAJA VERDE') || String(r.nombreInsumo).includes('ELIJA'));
            
            if (itemFazon) {
                itemFazon.nombreInsumo = "⚠️ ELIJA UN LOTE EN LA CAJA VERDE";
                itemFazon.materiaPrimaId = 0; 
                itemFazon.clienteId = Number(clienteId);
                itemFazon.esFazonInput = true;
            } else {
                let itemBaseOriginal = recetaDinamica.value.find(r => r.esBase);
                if (itemBaseOriginal) {
                    itemBaseOriginal.nombreInsumo = "⚠️ ELIJA UN LOTE EN LA CAJA VERDE";
                    itemBaseOriginal.materiaPrimaId = 0;
                    itemBaseOriginal.clienteId = Number(clienteId);
                    itemBaseOriginal.esFazonInput = true;
                } else {
                    recetaDinamica.value.push({
                        id: 'fazon_vacio_' + Date.now(),
                        materiaPrimaId: 0,
                        nombreInsumo: "⚠️ ELIJA UN LOTE EN LA CAJA VERDE",
                        cantidad: 50,
                        densidad: 1,
                        esBase: false, 
                        esFazonInput: true,
                        clienteId: Number(clienteId)
                    });
                }
            }
            balancearBase();
        } else {
            let itemFazon = recetaDinamica.value.find(r => r.esFazonInput || String(r.nombreInsumo).includes('CAJA VERDE') || String(r.nombreInsumo).includes('ELIJA'));
            if (!itemFazon) itemFazon = recetaDinamica.value.find(r => r.esBase);
            if (itemFazon) {
                itemFazon.nombreInsumo = "⚠️ CLIENTE SIN MATERIAL RECUPERADO/MOLIDO";
                itemFazon.materiaPrimaId = 0; 
                itemFazon.esFazonInput = true;
                itemFazon.clienteId = Number(clienteId);
            }
            balancearBase();
        }
    }

    function alCambiarLoteFazon() {
        const loteIdStr = String(loteFazonSeleccionadoId.value);
        const lote = listaLotesCliente.value.find((l: any) => String(l.id) === loteIdStr);
        if (lote) aplicarLoteFazonAReceta(lote);
    }

    return {
        detectarMaterial,
        actualizarRecetaFazonConCliente,
        alCambiarLoteFazon,
        aplicarLoteFazonAReceta
    };
}