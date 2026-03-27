import { computed } from 'vue';

const TIPOS_MATERIALES = [
    'PAI', 'PEAD', 'PP', 'ABS', 'RESISTENTE FREON', 'POLIETILENO'
];

// Funciones puras de ayuda (no necesitan ser exportadas)
const getSku = (p: any) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p: any) => (p.nombre || p.Nombre || '').toUpperCase();
const getRubro = (p: any) => (p.rubro || p.Rubro || '').toUpperCase();
const getClienteId = (p: any) => p.clienteId || p.ClienteId || 0;
const checkEsPT = (p: any) => !!(p.esProductoTerminado || p.EsProductoTerminado);
const checkEsMP = (p: any) => !!(p.esMateriaPrima || p.EsMateriaPrima);
const checkEsFazon = (p: any) => !!(p.esFazon || p.EsFazon);
const checkEsScrap = (p: any) => !!(p.esScrap || p.EsScrap);
const checkGenerico = (p: any) => !!(p.esGenerico || p.EsGenerico);

const checkEsMolido = (p: any) => getRubro(p).includes('MOLIDO') || getRubro(p).includes('SCRAP');
const esMpCliente = (p: any) => getRubro(p).includes('CLIENTE') || (checkEsMP(p) && getClienteId(p) > 0);
const checkEsMasterbatch = (p: any) => getRubro(p).includes('MASTER') || getRubro(p).includes('MASTERBATCH') || getNombre(p).includes('PIGMENTO');
const checkEsAditivo = (p: any) => getRubro(p).includes('ADITIVO');

export const detectarTipo = (p: any) => {
    if (p.tipoMaterial) {
        const t = p.tipoMaterial.toUpperCase().trim();
        if (TIPOS_MATERIALES.includes(t)) return t;
    }
    const n = getNombre(p);
    if (n.includes('FREON') || n.includes('RESISTENTE')) return 'RESISTENTE FREON';
    if (n.includes('ABS')) return 'ABS';
    if (n.includes('PEAD') || n.includes('ALTA') || n.includes('HDPE')) return 'PEAD';
    if (n.includes('PP') || n.includes('POLIPROPILENO')) return 'PP';
    if (n.includes('POLIETILENO') || n.includes('PEBD') || n.includes('BAJA') || n.includes('LDPE')) return 'POLIETILENO';
    if (n.includes('PAI') || n.includes('TUTI') || n.includes('IMPACTO') || n.includes('A.I.')) return 'PAI';
    return 'OTROS';
};

export function useFiltrosInventario(
    listaProductos: any, 
    listaClientes: any, 
    tabActual: any, 
    subTabMP: any, 
    subTabCliente: any, 
    clienteFiltro: any, 
    materialFiltro: any, 
    busqueda: any
) {

    const clientesFazon = computed(() => listaClientes.value.filter((c: any) => c.esFazon === true));

    const productosFiltrados = computed(() => {
        let lista = listaProductos.value;
        const tab = tabActual.value;
        
        if (tab === 'MP') {
            lista = lista.filter((p: any) => checkEsMP(p) && !esMpCliente(p) && !checkEsScrap(p) && !checkEsMolido(p) && p.id !== 90);

            if (subTabMP.value === 'MASTERBATCH') {
                lista = lista.filter(checkEsMasterbatch);
            } else if (subTabMP.value === 'ADITIVOS') {
                lista = lista.filter((p: any) => checkEsAditivo(p) && !checkEsMasterbatch(p));
            } else if (subTabMP.value === 'VIRGEN') {
                lista = lista.filter((p: any) => {
                    const n = getNombre(p);
                    if (colorProd.includes('GENERICO') || colorProd.includes('GENÉRICO')) return false;
                    if (n.includes('FAZON') || n.includes('FAZÓN') || n.includes('BASE')) return false;
                    return !checkEsMasterbatch(p) && !checkEsAditivo(p);
                });
            }
        } else if (tab === 'PT') {
            lista = lista.filter(checkEsPT);
        } else if (tab === 'CLI') {
            if (!clienteFiltro.value) return [];
            const idFiltro = Number(clienteFiltro.value);
            lista = lista.filter((p: any) => getClienteId(p) === idFiltro);
            
            if (subTabCliente.value === 'MP_CLI') {
                lista = lista.filter((p: any) => checkEsMP(p) && !checkEsMolido(p) && !checkEsPT(p));
            } else if (subTabCliente.value === 'MOLIDO_CLI') {
                lista = lista.filter((p: any) => checkEsMolido(p) || checkEsScrap(p));
            } else {
                lista = lista.filter((p: any) => checkEsFazon(p) || checkEsPT(p));
            }
            
            if (materialFiltro.value) {
                lista = lista.filter((p: any) => detectarTipo(p) === materialFiltro.value);
            }
        }

        if (busqueda.value) {
            const texto = busqueda.value.toUpperCase();
            lista = lista.filter((p: any) => getNombre(p).includes(texto) || getSku(p).includes(texto));
        }
        return lista;
    });

    const baseMP = computed(() => listaProductos.value.filter((p: any) => 
        checkEsMP(p) && !esMpCliente(p) && !checkEsMolido(p) && !checkEsScrap(p) && !checkGenerico(p) && 
        !getNombre(p).includes('GENERICO') && !getNombre(p).includes('BASE') && p.id !== 90
    ));

    const countMP = computed(() => baseMP.value.length);
    const countPT = computed(() => listaProductos.value.filter(checkEsPT).length);
    const countCLI = computed(() => listaProductos.value.filter((p: any) => getClienteId(p) > 0).length);

    return {
        TIPOS_MATERIALES,
        clientesFazon,
        productosFiltrados,
        countMP,
        countPT,
        countCLI,
        getClienteId,
        checkEsFazon,
        checkEsMolido,
        checkEsScrap
    };
}