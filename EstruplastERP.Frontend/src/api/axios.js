import axios from 'axios';

// Crea una instancia base (opcional, pero recomendado)
const api = axios.create({
  baseURL: 'http://localhost:5123/api' // <--- TU URL BASE AQUÍ
});

// INTERCEPTOR DE PETICIONES (REQUEST)
// Esto se ejecuta ANTES de que salga cada petición
api.interceptors.request.use((config) => {
  
  // 1. Buscamos si hay un token guardado
  const token = localStorage.getItem('token');

  // 2. Si hay token, lo pegamos en el encabezado (Header)
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
}, (error) => {
  return Promise.reject(error);
});

// INTERCEPTOR DE RESPUESTAS (RESPONSE) - Opcional
// Si el token vence (Error 401), sacamos al usuario
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // Token vencido o inválido: Borramos todo y mandamos al login
      localStorage.removeItem('token');
      localStorage.removeItem('usuario');
      window.location.href = '/login'; 
    }
    return Promise.reject(error);
  }
);

export default api;