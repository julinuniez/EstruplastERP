import { computed } from 'vue';
import type { Ref } from 'vue';

export function useFiltrosProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    productos: Ref<any[]>,
    clientes: Ref<any[]>,
    listaTodasMateriasPrimas: Ref<any[]>,
    listaInventarioCompleto: Ref<any[]>,
    productoSeleccionado: Ref<any>,
    clienteSeleccionado: Ref<any>,
    kilosCalculados: Ref<number>,
    factorMerma: Ref<number>,
    limiteMinimo: Ref<number>,
    limiteMaximo: Ref<number>
) {

    const listaMasterbatches = computed(() => {
        const origenDatos = listaTodasMateriasPrimas.value.length > 0 
            ? listaTodasMateriasPrimas.value 
            : listaInventarioCompleto.value;

        return origenDatos.filter(mp => {
            const nombre = (mp.nombre || '').toUpperCase();
            const rubro = (mp.rubro || '').toUpperCase();
            return rubro.includes('MASTERBATCH') || nombre.includes('MASTERBATCH') || nombre.includes('PIGMENTO');
        }).sort((a, b) => a.nombre.localeCompare(b.nombre));
    });

    const idCristal555 = computed(() => {
        const material = listaTodasMateriasPrimas.value.find(m => m.codigoSku === 'MP-CRI-555' || m.nombre === 'CRISTAL 555');
        return material ? material.id : 0;
    });

    const mostrarCajaColor = computed(() => {
        if (productoSeleccionado.value) {
            if ((productoSeleccionado.value.nombre || '').toUpperCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "").includes('COLOR')) return true;
        }
        if (recetaDinamica.value && recetaDinamica.value.length > 0) {
            return recetaDinamica.value.some(r => Number(r.materiaPrimaId) === 22 || r.esColor === true);
        }
        return false;
    });

    const colorFinalParaPDF = computed(() => {
        if (form.value.colorTexto && form.value.colorTexto.trim() !== '') {
            return form.value.colorTexto.toUpperCase();
        }
        if (mostrarCajaColor.value && form.value.masterbatchId) {
            const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
            return mb ? (mb.nombre.split(' ').length > 1 ? mb.nombre.split(' ').slice(1).join(' ') : mb.nombre) : 'A DEFINIR';
        }
        return '-';
    });

    const clienteTieneFazonActivo = computed(() => {
        if (!clienteSeleccionado.value) return false;
        return clienteSeleccionado.value.esFazon === true;
    });

    const clientesHabilitados = computed(() => {
        return clientes.value.filter(c => c.esFazon === true);
    });

    const medidasBloqueadas = computed(() => !productoSeleccionado.value || !productoSeleccionado.value.esGenerico);

    const espesorValido = computed(() => {
        const e = Number(form.value.espesor);
        if (e <= 0) return true;
        if (limiteMinimo.value > 0 && e < limiteMinimo.value) return false;
        if (limiteMaximo.value > 0 && e > limiteMaximo.value) return false;
        return true;
    });

    const listaProductosDisponibles = computed(() => {
        if (!productos.value || productos.value.length === 0) return [];
        
        const idClienteSeleccionado = form.value.clienteId ? Number(form.value.clienteId) : null;

        return productos.value.filter(p => {
            const nombre = (p.nombre || '').toUpperCase();
            const rubro = (p.rubro || '').toUpperCase();
            
            if (p.esMateriaPrima || p.esScrap || rubro.includes('MOLIDO')) return false;
            if (rubro.includes('MATERIA') || rubro.includes('INSUMO') || rubro.includes('MASTERBATCH')) return false;
            if (nombre.includes('BASE') && !nombre.includes('ALTA')) return false;
            if (nombre.includes('(BASE)') || nombre.includes('(VARIOS)')) return false;
            if (nombre.includes('GENERICO') || nombre.includes('GENÉRICO')) return false;
            if (nombre.includes('MASTERBATCH') || nombre.includes('PIGMENTO') || nombre.includes('SCRAP')) return false;
            if (p.id >= 990 && p.id <= 999) return false; 

            const esProductoFazon = p.esFazon || nombre.includes('FAZON') || nombre.includes('SERVICIO');

            if (esProductoFazon) {
                if (idClienteSeleccionado && !clienteTieneFazonActivo.value) return false;
                if (!idClienteSeleccionado) return false;
                
                const esPropioDelCliente = p.clienteId && p.clienteId == idClienteSeleccionado;
                const esServicioGenerico = !p.clienteId || p.clienteId === 0;

                if ((esPropioDelCliente || esServicioGenerico) && clienteTieneFazonActivo.value) {
                    return true; 
                } else {
                    return false; 
                }
            }
            return true; 
        });
    });

    const materiasPrimasLimpias = computed(() => {
        const esFazonOp = productoSeleccionado.value?.esFazon || 
                          (productoSeleccionado.value?.nombre || '').toUpperCase().includes('FAZON');

        const materialesBaseAbstractos = [
            "POLIPROPILENO", "PEAD", "PEBD", "PAI", "POLIETILENO", 
            "ABS", "RESISTENTE AL FREON", "ALTO IMPACTO"
        ];

        return listaTodasMateriasPrimas.value.filter(mp => {
            const nombre = (mp.nombre || '').toUpperCase().trim();
            const rubro = (mp.rubro || '').toUpperCase().trim();
            const clienteIdMp = Number(mp.clienteId) || 0;

            // 1. Ocultamos las familias genéricas (los nombres puros)
            if (materialesBaseAbstractos.includes(nombre)) return false;

            // 2. ♻️ DETECCIÓN INFALIBLE POR NOMBRE: Busca la etiqueta "[MOLIDO]"
            if (nombre.includes('[MOLIDO]') || rubro.includes('MOLIDO')) {
                // Si NO tiene cliente (es Estruplast), pasa directo a la lista
                if (clienteIdMp === 0) return true;
                
                // Si TIENE cliente (Fazón), lo ocultamos de acá
                return false; 
            }

            // 3. 🚫 BLOQUEO DE SCRAP: Si llegó acá y es Scrap, es el sucio sin procesar. Afuera.
            if (mp.esScrap || nombre.includes('SCRAP') || rubro.includes('SCRAP')) return false;

            // 4. MATERIALES DE FAZON (Cajas, tubos del cliente): Solo para OPs de Fazón
            if (mp.esFazon || nombre.includes('FAZON')) return esFazonOp;

            // El resto (Virgen, Aditivos, Masterbatches) pasa siempre
            return true; 
        });
    });

    const insumosSinStock = computed(() => {
        const kilosNetos = Number(kilosCalculados.value);
        if (kilosNetos <= 0) return [];
        const faltantes: any[] = [];
        const factor = factorMerma.value;

        recetaDinamica.value.forEach(item => {
            const porcentajeInsumo = parseFloat(item.cantidad.toString()) || 0;
            const pesoNetoInsumo = (kilosNetos * porcentajeInsumo) / 100;
            const consumoReal = Number((pesoNetoInsumo * factor).toFixed(3));
            const idMaterial = Number(item.materiaPrimaId);

            let stockDisponible = 0;
            let nombreMaterial = item.nombreInsumo;

            if (idMaterial >= 990 && idMaterial <= 999) {
                stockDisponible = 0;
            } else {
                const mp = listaInventarioCompleto.value.find(m => m.id === idMaterial) || listaTodasMateriasPrimas.value.find(m => m.id === idMaterial);
                if (mp) {
                    stockDisponible = Number(mp.stockActual || 0);
                    nombreMaterial = mp.nombre;
                }
            }

            if (stockDisponible < (consumoReal - 0.001)) {
                faltantes.push({
                    nombre: nombreMaterial,
                    necesorio: consumoReal,
                    disponible: stockDisponible,
                    diferencia: Number((consumoReal - stockDisponible).toFixed(2))
                });
            }
        });
        return faltantes;
    });

    const hayBloqueoDeStock = computed(() => insumosSinStock.value.length > 0);

    return {
        listaMasterbatches,
        idCristal555,
        mostrarCajaColor,
        colorFinalParaPDF,
        clienteTieneFazonActivo,
        clientesHabilitados,
        medidasBloqueadas,
        espesorValido,
        listaProductosDisponibles,
        materiasPrimasLimpias,
        insumosSinStock,
        hayBloqueoDeStock
    };
}