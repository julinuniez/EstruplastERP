import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

export const useSesionStore = defineStore('sesion', () => {
    
    // 1. ESTADO (State)
    // Inicializamos leyendo de localStorage por si el usuario recargó la página
    const usuario = ref<string | null>(localStorage.getItem('usuario'));
    const token = ref<string | null>(localStorage.getItem('token'));
    const rol = ref<string | null>(localStorage.getItem('rol'));

    // 2. COMPUTADAS (Getters)
    // Propiedad útil para saber rápidamente si hay sesión activa en cualquier parte de la app
    const estaLogueado = computed(() => !!token.value);

    // 3. ACCIONES (Actions)
    
    // Esta función se llama desde el Login.vue cuando el backend responde OK
    function iniciar(datos: { usuario: string; token: string; rol: string }) {
        // A. Actualizamos el estado reactivo de Pinia
        usuario.value = datos.usuario;
        token.value = datos.token;
        rol.value = datos.rol;

        // B. Persistimos en el navegador (para que no se borre al F5)
        localStorage.setItem('usuario', datos.usuario);
        localStorage.setItem('token', datos.token);
        localStorage.setItem('rol', datos.rol);
    }

    function cerrar() {
        // A. Limpiamos estado
        usuario.value = null;
        token.value = null;
        rol.value = null;

        // B. Limpiamos navegador
        localStorage.clear();
    }

    return { usuario, token, rol, estaLogueado, iniciar, cerrar };
});