import { ref } from 'vue';
import api from '@/services/axiosInstance';
import { Alertas } from '@/utils/alertas';

export function useImportacionInventario(
    tabActual: any,
    clienteFiltro: any,
    importClienteFiltro: any,
    cargarDatos: (forzar?: boolean) => Promise<void> 
) {
    const fileInput = ref<HTMLInputElement | null>(null);
    const importando = ref(false);

    const clickImportar = () => fileInput.value?.click();

    const subirArchivoFlexxus = async (event: Event) => {
        const target = event.target as HTMLInputElement;
        if (!target.files || target.files.length === 0) return;
        const archivo = target.files[0];
        if (!archivo) return;

        const esExcel = archivo.name.toLowerCase().endsWith('.xlsx');
        const esCsv = archivo.name.toLowerCase().endsWith('.csv');

        if (esCsv && tabActual.value === 'CLI' && !clienteFiltro.value) {
            Alertas.advertencia("⚠️ ATENCIÓN:\nPara importar un CSV de stock específico, por favor seleccione primero el CLIENTE en el filtro.");
            target.value = '';
            return;
        }

        const formData = new FormData();
        formData.append('archivo', archivo);

        if (esCsv && clienteFiltro.value) {
            formData.append('clienteId', clienteFiltro.value.toString());
        }

        if (esExcel && importClienteFiltro.value) {
            formData.append('clienteIdFiltro', importClienteFiltro.value.toString());
        }

        try {
            importando.value = true;
            let urlEndpoint = esExcel ? `/Integration/importar-excel-multicliente` : `/Integration/importar-maestro`;

            const res = await api.post(urlEndpoint, formData, {
                headers: { 'Content-Type': 'multipart/form-data' }
            });

            Alertas.exito(`✅ ÉXITO:\n${res.data.mensaje}`);

            if (res.data.logs && res.data.logs.length > 0) {
                console.warn("Reporte de Importación (Hojas omitidas):", res.data.logs);
                if (!importClienteFiltro.value) {
                    Alertas.advertencia("⚠️ Atención: Algunas hojas fueron omitidas por no coincidir con ningún cliente registrado. Revisa la consola (F12) para más detalles.");
                }
            }
            await cargarDatos(true);

        } catch (e: any) {
            console.error(e);
            const msg = e.response?.data || "Error al procesar el archivo.";
            Alertas.error(`❌ ERROR: ${msg}`);
        } finally {
            importando.value = false;
            if (fileInput.value) fileInput.value.value = '';
        }
    };

    return {
        fileInput,
        importando,
        clickImportar,
        subirArchivoFlexxus
    };
}