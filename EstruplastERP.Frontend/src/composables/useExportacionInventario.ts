import ExcelJS from 'exceljs';
import { saveAs } from 'file-saver';

export async function exportarInventarioExcel(inventarioOriginal: any[], clientes: any[] = []) {
    const exclusiones = ['PRODUCTO TERMINADO', 'FAMILIA BASE', 'SERVICIO FAZON', 'SERVICIO A FAZON'];
    
    // 1. Filtrado de seguridad
    let inventario = inventarioOriginal.filter(p => {
        const rubro = (p.rubro || '').toUpperCase().trim();
        const nombre = (p.nombre || '').toUpperCase().trim();
        if (exclusiones.includes(rubro) || exclusiones.some(ex => nombre.includes(ex))) return false;
        return true;
    });

    const workbook = new ExcelJS.Workbook();
    const sheet = workbook.addWorksheet('Auditoria Inventario');

    sheet.columns = [
        { header: 'NOMBRE DEL MATERIAL', key: 'nombre', width: 36 },
        { header: 'STOCK ACTUAL', key: 'fisico', width: 15 },
        { header: 'STOCK DISPONIBLE', key: 'disponible', width: 16},
        { header: 'CONTEO REAL ', key: 'conteo', width: 18 }
    ];

    // Estilo del encabezado
    const headerRow = sheet.getRow(1);
    headerRow.font = { bold: true, color: { argb: 'FFFFFFFF' } };
    headerRow.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FF2C3E50' } };
    headerRow.alignment = { horizontal: 'center' };

    const categorias = [...new Set(inventario.map(item => item.rubro || 'SIN CLASIFICAR'))];

    categorias.sort().forEach(cat => {
        const separador = sheet.addRow({ nombre: `▬▬ ${cat.toUpperCase()} ▬▬` });
        separador.font = { bold: true, color: { argb: 'FFD35400' } };
        separador.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'FFFDF2E9' } };

        const productos = inventario.filter(p => (p.rubro || 'SIN CLASIFICAR') === cat);
        
        productos.sort((a, b) => (a.nombre || '').localeCompare(b.nombre || '')).forEach(p => {
            let nombreFinal = p.nombre || '';
            const clientId = Number(p.clienteId || p.ClienteId || 0);
            
            // Limpieza de texto [MOLIDO] y asignación de Dueño
            if (nombreFinal.toUpperCase().includes('MOLIDO')) {
                nombreFinal = nombreFinal.replace(/\[?MOLIDO\]?/gi, '').trim()
                                         .replace(/\s+/g, ' ').replace(/^-\s*/, '').trim();

                if (clientId > 0 && clientes.length > 0) {
                    const clienteData = clientes.find(c => Number(c.id) === clientId);
                    if (clienteData) nombreFinal = `${nombreFinal} - DE: ${clienteData.razonSocial.toUpperCase()}`;
                }
            }

            // 🚀 MAPEANDO EL STOCK DEL DTO
            // Verificamos 'stockActual' (minúscula) que es el estándar de JSON
            // Copiamos la misma lógica infalible que usa tu tabla HTML
const stockFisico = Number(p.stockFisico ?? p.stockActual ?? p.StockActual ?? 0);
const stockReservado = Number(p.stockReservado ?? p.StockReservado ?? 0);
const stockDisponible = Number(p.stockDisponible ?? (stockFisico - stockReservado));

            const fila = sheet.addRow({
                nombre: nombreFinal,
                fisico: Number(stockFisico).toFixed(2), 
                disponible: Number(stockDisponible).toFixed(2),
                conteo: '' 
            });

            fila.getCell('fisico').alignment = { horizontal: 'right' };
            fila.getCell('disponible').alignment = { horizontal: 'right' };
            fila.getCell('conteo').border = {
                top: { style: 'medium' }, left: { style: 'medium' },
                bottom: { style: 'medium' }, right: { style: 'medium' }
            };
        });
    });

    const buffer = await workbook.xlsx.writeBuffer();
    saveAs(new Blob([buffer]), `Auditoria_Inventario_${new Date().toLocaleDateString('es-AR').replace(/\//g, '-')}.xlsx`);
}