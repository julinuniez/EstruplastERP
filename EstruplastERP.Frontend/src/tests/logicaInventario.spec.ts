import { describe, it, expect } from 'vitest';

// Simulamos la función que usás en tu código para calcular el stock
// (Ajustá los nombres según cómo lo tengas en tu composable de inventario)
function calcularStock(stockFisico: number, stockReservado: number) {
    return stockFisico - stockReservado;
}

function simularCrearOrden(kilosPedidos: number, estadoActual: any) {
    return {
        stockFisico: estadoActual.stockFisico, // ¡NO SE TOCA!
        stockReservado: estadoActual.stockReservado + kilosPedidos
    };
}

function simularConsumoEnMaquina(kilosConsumidos: number, estadoActual: any) {
    return {
        stockFisico: estadoActual.stockFisico - kilosConsumidos,
        stockReservado: estadoActual.stockReservado - kilosConsumidos
    };
}

describe('Lógica Crítica de Inventario y Reservas', () => {

    it('Cálculo base: Disponible = Físico - Reservado', () => {
        const disponible = calcularStock(1000, 200);
        expect(disponible).toBe(800);
    });

    it('Al CREAR una orden: Sube el reservado, el físico queda intacto y baja el disponible', () => {
        const inventarioInicial = { stockFisico: 1000, stockReservado: 200 };
        const kilosParaNuevaOrden = 100;

        const nuevoInventario = simularCrearOrden(kilosParaNuevaOrden, inventarioInicial);
        const nuevoDisponible = calcularStock(nuevoInventario.stockFisico, nuevoInventario.stockReservado);

        // Verificamos que no haya "doble resta"
        expect(nuevoInventario.stockFisico).toBe(1000); // Sigue en el galpón
        expect(nuevoInventario.stockReservado).toBe(300); // 200 + 100
        expect(nuevoDisponible).toBe(700); // 1000 - 300
    });

    it('Al CONSUMIR una orden (Máquina): Baja el físico y baja el reservado, el disponible se mantiene', () => {
        // Imaginate que ya teníamos los 100kg reservados de la prueba anterior
        const inventarioInicial = { stockFisico: 1000, stockReservado: 300 }; 
        const kilosConsumidos = 100;

        const nuevoInventario = simularConsumoEnMaquina(kilosConsumidos, inventarioInicial);
        const nuevoDisponible = calcularStock(nuevoInventario.stockFisico, nuevoInventario.stockReservado);

        expect(nuevoInventario.stockFisico).toBe(900); // Ahora sí se usó el plástico
        expect(nuevoInventario.stockReservado).toBe(200); // Ya no está reservado, se usó
        
        // El disponible sigue siendo 700, porque esa resta ya se había hecho al crear la orden
        expect(nuevoDisponible).toBe(700); 
    });

});