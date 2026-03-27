import type { Ref } from 'vue';

// Nos traemos las constantes para acá y las borramos del componente principal
const DENSIDAD_DEFAULT = 1.1;
const ID_BRILLO_777 = 1073; 
const ID_ESTEARATO = 1074; 
const ID_UV = 1075; 
const ID_CAUCHO = 1076; 
const ID_CARGA = 1077; 
const PORC_ESTEARATO = 0.08; 

export function useRecetaProduccion(
    form: Ref<any>,
    recetaDinamica: Ref<any[]>,
    listaTodasMateriasPrimas: Ref<any[]>,
    listaInventarioCompleto: Ref<any[]>,
    listaMasterbatches: Ref<any[]>,
    idCristal555: Ref<number>,
    mostrarCajaColor: Ref<boolean>
) {

    function balancearBase() {
        if (form.value.esConsolidado) return; 
        if (recetaDinamica.value.length === 0) return;

        recetaDinamica.value.forEach(r => r.esBase = false);

        const sorted = [...recetaDinamica.value].sort((a, b) => Number(b.cantidad) - Number(a.cantidad));
        const nuevaBase = sorted[0];

        if (nuevaBase) {
            nuevaBase.esBase = true; 
            const sumaOtros = recetaDinamica.value.reduce((acc, item) => {
                if (item === nuevaBase) return acc;
                return acc + (parseFloat(item.cantidad.toString()) || 0);
            }, 0);

            const nuevoPorcentajeBase = 100 - sumaOtros;
            nuevaBase.cantidad = parseFloat(Math.max(0, nuevoPorcentajeBase).toFixed(2));
        }
    }

    function recalcularFormulaAutomatica() {
        let porcentajeColor = 2.00;
        
        const colorExistente = recetaDinamica.value.find(r => r.esColor || Number(r.materiaPrimaId) === 22);
        if (colorExistente) porcentajeColor = Number(colorExistente.cantidad);

        const borrar = ['esCarga', 'esBrillo', 'esEstearato', 'esUv', 'esCaucho'];
        if (mostrarCajaColor.value && form.value.masterbatchId) borrar.push('esColor');

        let nueva = recetaDinamica.value.filter(r => {
            for (const flag of borrar) if (r[flag as keyof any]) return false;
            if (mostrarCajaColor.value && form.value.masterbatchId && Number(r.materiaPrimaId) === 22) return false;
            return true;
        });

        const add = (nom: string, cant: number, tipo: string, mpId: number = 0, dens: number = DENSIDAD_DEFAULT) => {
            let m = null;
            if (mpId === 0) m = listaTodasMateriasPrimas.value.find(x => x.nombre.toUpperCase().includes(nom));
            else m = listaTodasMateriasPrimas.value.find(x => x.id === mpId);

            nueva.push({
                id: tipo,
                cantidad: cant,
                nombreInsumo: m ? m.nombre : (nom === 'COLOR' ? 'MASTERBATCH' : nom),
                densidad: m ? (m.pesoEspecifico || dens) : dens,
                materiaPrimaId: m ? m.id : mpId,
                [tipo]: true,
                esColor: tipo === 'esColor',
                esEstearato: tipo === 'esEstearato' 
            });
        };

        if (form.value.conBrillo) {
            const idBrillo = form.value.tipoBrillo === '777' ? ID_BRILLO_777 : idCristal555.value;
            const nombreBrillo = form.value.tipoBrillo === '777' ? 'BRILLO 777' : 'CRISTAL 555';
            add(nombreBrillo, form.value.porcBrillo, 'esBrillo', idBrillo);
        }
        
        if (form.value.conEstearato) add('ESTEARATO', PORC_ESTEARATO, 'esEstearato', ID_ESTEARATO);
        if (form.value.aditivoUV) add('UV', form.value.porcentajeUv, 'esUv', ID_UV);
        if (form.value.aditivoCaucho) add('CAUCHO', form.value.porcentajeCaucho, 'esCaucho', ID_CAUCHO);
        if (mostrarCajaColor.value && form.value.masterbatchId) {
            const mb = listaMasterbatches.value.find(m => m.id === form.value.masterbatchId);
            if (mb) add('COLOR', porcentajeColor, 'esColor', mb.id, mb.pesoEspecifico);
        }
        if (form.value.aditivoCarga > 0) add('CARGA', form.value.aditivoCarga, 'esCarga', ID_CARGA);

        recetaDinamica.value = nueva;
        balancearBase();
    }

    function quitarInsumoManual(index: number) {
        if (index >= 0 && index < recetaDinamica.value.length) {
            recetaDinamica.value.splice(index, 1);
            balancearBase();
        }
    }

    function agregarInsumoDesdeHijo(item: { id: number, porcentaje: number }) {
        const mp = listaTodasMateriasPrimas.value.find(m => m.id === item.id) || 
                   listaInventarioCompleto.value.find(m => m.id === item.id);
        
        if (mp) {
            recetaDinamica.value.push({
                id: 'manual_' + Date.now(),
                materiaPrimaId: mp.id,
                nombreInsumo: mp.nombre,
                cantidad: item.porcentaje,
                densidad: mp.pesoEspecifico || 1.1,
                esBase: false
            });
            balancearBase();
        } else {
            alert("⚠️ Ocurrió un error: No pudimos encontrar los datos técnicos de este material.");
        }
    }

    return {
        balancearBase,
        recalcularFormulaAutomatica,
        quitarInsumoManual,
        agregarInsumoDesdeHijo
    };
}