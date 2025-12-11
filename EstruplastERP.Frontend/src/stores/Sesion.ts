import { defineStore } from 'pinia';
import { ref, computed } from 'vue';

// 1. DEFINIMOS LA INTERFAZ (El molde del usuario)
// Esto le dice a TypeScript que tu usuario es un OBJETO con propiedad nombre
export interface Usuario {
    nombre: string;
    email?: string;
    // Puedes agregar más datos aquí si tu backend los manda
}

export const useSesionStore = defineStore('sesion', () => {
    
    // 2. ESTADO (State)
    // Inicializamos intentando leer el objeto JSON del localStorage
    const usuario = ref<Usuario | null>(null);
    const token = ref<string | null>(localStorage.getItem('token'));
    const rol = ref<string | null>(localStorage.getItem('rol'));

    // Lógica para recuperar el usuario guardado (Manejo de errores de JSON)
    const usuarioGuardado = localStorage.getItem('usuario');
    if (usuarioGuardado) {
        try {
            // Intentamos convertir el texto guardado en objeto
            usuario.value = JSON.parse(usuarioGuardado);
        } catch (e) {
            // Si falla (o era un string viejo), lo adaptamos
            usuario.value = { nombre: usuarioGuardado };
        }
    }

    // 3. COMPUTADAS (Getters)
    const estaLogueado = computed(() => !!token.value);

    // 4. ACCIONES (Actions)
    
    // Modificamos para recibir un objeto 'user' completo o al menos el nombre
    function iniciar(datos: { usuario: any; token: string; rol: string }) {
        
        // A. Si el backend manda solo un string, lo convertimos a objeto
        if (typeof datos.usuario === 'string') {
            usuario.value = { nombre: datos.usuario };
        } else {
            // Si ya es objeto (lo ideal), lo usamos directo
            usuario.value = datos.usuario;
        }

        token.value = datos.token;
        rol.value = datos.rol;

        // B. Persistimos en el navegador
        // IMPORTANTE: Guardamos el usuario como JSON string
        localStorage.setItem('usuario', JSON.stringify(usuario.value));
        localStorage.setItem('token', datos.token);
        localStorage.setItem('rol', datos.rol);
    }

    function cerrar() {
        usuario.value = null;
        token.value = null;
        rol.value = null;
        localStorage.clear();
    }

    return { usuario, token, rol, estaLogueado, iniciar, cerrar };
});