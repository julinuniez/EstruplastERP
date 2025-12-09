<script setup lang="ts">
import { ref } from 'vue'
import { sesion } from '../stores/sesion' // Importamos el Singleton

const form = ref({ usuario: '', password: '' })
const error = ref('')

async function ingresar() {
    try {
        const res = await fetch('https://localhost:7244/api/Usuarios/login', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify(form.value)
        })

        if (!res.ok) throw new Error("Datos incorrectos")

        const datosUsuario = await res.json()
        
        // ¡AQUÍ USAMOS EL SINGLETON!
        sesion.iniciar(datosUsuario) 
        
    } catch (e) {
        error.value = "❌ Acceso denegado"
    }
}
</script>

<template>
    <div class="login-container">
        <div class="card-login">
            <h2>🔐 Estruplast ERP</h2>
            <input v-model="form.usuario" type="text" placeholder="Usuario" @keyup.enter="ingresar">
            <input v-model="form.password" type="password" placeholder="Contraseña" @keyup.enter="ingresar">
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
.error { color: red; margin-top: 10px; }
</style>