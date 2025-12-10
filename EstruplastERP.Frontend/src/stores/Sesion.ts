// src/stores/sesion.ts
import { defineStore } from 'pinia';
import { ref } from 'vue';

export const useSesionStore = defineStore('sesion', () => {
    // Estado
    const usuario = ref<{ nombre: string } | null>({ nombre: 'Desarrollador' }); // Simulado por ahora

    // Acciones
    function recuperar() {
        console.log("Sesión recuperada");
        // Aquí iría la lógica real de leer localStorage
    }

    function cerrar() {
        usuario.value = null;
        console.log("Sesión cerrada");
    }

    return { usuario, recuperar, cerrar };
});