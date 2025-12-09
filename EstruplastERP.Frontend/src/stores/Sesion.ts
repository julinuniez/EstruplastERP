import { reactive } from 'vue'

// Este objeto es el SINGLETON. Existe una sola vez y se comparte.
export const sesion = reactive({
    usuario: null as any, // Aquí guardaremos los datos del usuario logueado
    estaLogueado: false,

    // Método para iniciar sesión
    iniciar(datosUsuario: any) {
        this.usuario = datosUsuario
        this.estaLogueado = true
        localStorage.setItem('usuario_erp', JSON.stringify(datosUsuario)) // Persistencia básica
    },

    // Método para cerrar sesión
    cerrar() {
        this.usuario = null
        this.estaLogueado = false
        localStorage.removeItem('usuario_erp')
    },

    // Verificar si ya había sesión guardada al recargar la página
    recuperar() {
        const guardado = localStorage.getItem('usuario_erp')
        if (guardado) {
            this.usuario = JSON.parse(guardado)
            this.estaLogueado = true
        }
    }
})