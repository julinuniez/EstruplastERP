<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { useSesionStore } from '../stores/sesion' // 1. Importamos la definición del store

const router = useRouter()
const sesion = useSesionStore() // 2. ¡IMPORTANTE! Instanciamos el store aquí

const form = ref({ usuario: '', password: '' })
const error = ref('')

async function ingresar() {
    error.value = '' 
    
    try {
        // CAMBIO: Usamos Axios en lugar de fetch para mejor manejo de errores
        // Asegúrate que el puerto coincida con tu backend .NET (ej: 7244 o 5123)
        const res = await axios.post('https://localhost:7244/api/Auth/login', {
            nombreUsuario: form.value.usuario, // Mapeamos usuario -> nombreUsuario
            password: form.value.password
        })

        // El backend devuelve: { token: "...", usuario: "admin", rol: "Admin" }
        const datosUsuario = res.data

        // 3. Llamamos a la acción del Store nuevo
        sesion.iniciar(datosUsuario) 
        
        // 4. Redirigimos
        router.push('/dashboard') 
        
    } catch (e: any) {
        console.error(e)
        if (e.response && e.response.status === 401) {
            error.value = "❌ Usuario o contraseña incorrectos"
        } else if (e.code === "ERR_NETWORK") {
            error.value = "❌ No se pudo conectar con el servidor (¿Está prendido?)"
        } else {
            error.value = "❌ Error al iniciar sesión"
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
/* Tus estilos estaban perfectos, los mantengo igual */
.login-container { height: 100vh; display: flex; justify-content: center; align-items: center; background: #2c3e50; }
.card-login { background: white; padding: 40px; border-radius: 10px; text-align: center; width: 300px; box-shadow: 0 10px 25px rgba(0,0,0,0.2); }
input { display: block; width: 100%; margin: 10px 0; padding: 10px; border: 1px solid #ccc; border-radius: 5px; }
button { width: 100%; padding: 10px; background: #e67e22; color: white; border: none; border-radius: 5px; cursor: pointer; font-weight: bold; margin-top: 10px;}
.error { color: red; margin-top: 10px; font-weight: bold; }
</style>