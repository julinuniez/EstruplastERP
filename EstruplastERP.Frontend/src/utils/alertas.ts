// src/utils/alertas.ts
import Swal from 'sweetalert2';

export const Alertas = {
    exito: (mensaje: string) => {
        return Swal.fire({
            icon: 'success',
            title: '¡Éxito!',
            text: mensaje,
            timer: 2000,
            showConfirmButton: false
        });
    },

    error: (mensaje: string) => {
        return Swal.fire({
            icon: 'error',
            title: 'Operación Rechazada',
            html: mensaje.replace(/\n/g, '<br>'), // Convierte saltos de línea para que se vean bien
            confirmButtonColor: '#e74c3c'
        });
    },

    advertencia: (mensaje: string) => {
        return Swal.fire({
            icon: 'warning',
            title: 'Atención',
            text: mensaje,
            confirmButtonColor: '#f39c12'
        });
    },

    // ESTE ES EL MÁS IMPORTANTE (Es asíncrono)
    confirmar: async (titulo: string, mensaje: string) => {
        const result = await Swal.fire({
            title: titulo,
            text: mensaje,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#27ae60',
            cancelButtonColor: '#95a5a6',
            confirmButtonText: 'Sí, confirmar',
            cancelButtonText: 'Cancelar'
        });
        return result.isConfirmed; // Devuelve true o false
    }
};