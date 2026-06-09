import axios from 'axios';

// 1. Creamos la instancia usando la variable de entorno
const api = axios.create({
    baseURL: import.meta.env.VITE_API_URL, // Vite lee esto del archivo .env
    headers: {
        'Content-Type': 'application/json'
    }
});

// 2. Interceptor: Inyecta el Token automáticamente en cada petición
api.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token');
        if (token) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// 3. Interceptor: Maneja errores globales (como el 401)
api.interceptors.response.use(
    (response) => response,
    (error) => {
        // Si el token venció o es inválido
        if (error.response && error.response.status === 401) {
            console.warn("Sesión expirada. Redirigiendo al login...");
            localStorage.removeItem('token');
            window.location.href = '/login'; // O tu ruta de login
        }
        return Promise.reject(error);
    }
);

export default api;