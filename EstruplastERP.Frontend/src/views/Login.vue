<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { useSesionStore } from '../stores/Sesion'

const router = useRouter()
const sesion = useSesionStore()

const form = ref({ usuario: '', password: '' })
const error = ref('')

let urlBase = import.meta.env.VITE_API_URL || 'https://localhost:5122/api';

if (urlBase === '/api' && window.location.port === '5173') {
    console.warn("⚠️ Detectado entorno DEV con ruta relativa. Forzando https://localhost:5122/api");
    urlBase = 'https://localhost:5122/api';
}

const apiUrl = urlBase;

async function ingresar() {
    error.value = '' 
    
    try {
<<<<<<< HEAD
        const res = await axios.post(`${apiUrl}/Auth/login`, {
            nombreUsuario: form.value.usuario,
=======
        // CAMBIO: Usamos Axios en lugar de fetch para mejor manejo de errores
        // Asegúrate que el puerto coincida con tu backend .NET (ej: 7244 o 5123)
        const res = await axios.post('/api/Auth/login', {
            nombreUsuario: form.value.usuario, // Mapeamos usuario -> nombreUsuario
>>>>>>> master
            password: form.value.password
        })

        // El backend devuelve: { token: "...", usuario: "admin", rol: "Admin" }
        const datosUsuario = res.data

        // 3. Llamamos a la acción del Store nuevo
        sesion.iniciar(datosUsuario) 
<<<<<<< HEAD
        router.push({ name: 'produccion' }) 
=======
        
        // 4. Redirigimos
        router.push({ name: 'produccion' })
>>>>>>> master
        
    } catch (e: any) {
        console.error(e)
        if (e.response && e.response.status === 401) {
            error.value = "❌ Usuario o contraseña incorrectos"
        } else if (e.code === "ERR_NETWORK") {
            error.value = "❌ No se pudo conectar con el servidor (¿Está prendido el Backend?)"
        } else if (e.response && e.response.status === 404) {
            error.value = `❌ Error 404: No encuentro la ruta ${apiUrl}/Auth/login`
        } else {
            error.value = "❌ Error al iniciar sesión: " + (e.message || "")
        }
    }
}
</script>

<template>
    <div class="login-container">
        <div class="card-login">
            <h2>🔐 Estruplast ERP</h2>
            <input v-model.trim="form.usuario" type="text" placeholder="Usuario" @keyup.enter="ingresar">
            <input v-model.trim="form.password" type="password" placeholder="Contraseña" @keyup.enter="ingresar">
            
            <button @click="ingresar">ENTRAR</button>
            
            <p v-if="error" class="error">{{ error }}</p>
        </div>
    </div>
</template>

<style scoped>
.login-container { height: 100vh; display: flex; justify-content: center; align-items: center; background: #2c3e50; }
.card-login { background: white; padding: 40px; border-radius: 10px; text-align: center; width: 300px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
input { display: block; width: 100%; margin: 10px 0; padding: 10px; border: 1px solid #ccc; border-radius: 5px; }
button { width: 100%; padding: 10px; background: #e67e22; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; margin-top: 10px;}
button:hover { background: #d35400; } /* Un pequeño hover visual */
.error { color: red; margin-top: 10px; font-weight: bold; font-size: 0.9em; }
</style>