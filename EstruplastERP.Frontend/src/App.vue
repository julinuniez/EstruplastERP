<script setup lang="ts">
import { useRouter } from 'vue-router';
import { useSesionStore } from './stores/sesion';


// 1. Inicializamos el store y el router
const sesion = useSesionStore();
const router = useRouter();

// 2. Función para cerrar sesión y redirigir
function cerrarSesion() {
  sesion.cerrar(); // Limpia variables y localStorage
  router.push({ name: 'login' }); // Manda al usuario al login
}
</script>

<template>
  <div class="contenedor-principal">
    
    <header>
      <div class="usuario-info">
          Hola, <strong>{{ sesion.usuario?.nombre || 'Invitado' }}</strong>
          
          <button v-if="sesion.estaLogueado" @click="cerrarSesion" class="btn-salir">
            Cerrar Sesión
          </button>
      </div>

      <div class="logo-container">
         <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAlYAAAA4CAYAAAA/xLYcAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyZpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuNi1jMDY3IDc5LjE1Nzc0NywgMjAxNS8wMy8zMC0yMzo0MDo0MiAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENDIDIwMTUgKFdpbmRvd3MpIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOkY4MzhCNEFBOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOkY4MzhCNEFCOEU0NzExRUFCRDkyOUIyODQ1RTEzREQ5Ij4gPHhtcE1NOkRlcml2ZWRGcm9tIHN0UmVmOmluc3RhbmNlSUQ9InhtcC5paWQ6RjgzOEI0QTg4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiIHN0UmVmOmRvY3VtZW50SUQ9InhtcC5kaWQ6RjgzOEI0QTk4RTQ3MTFFQUJEOTI5QjI4NDVFMTNERDkiLz4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6QwnXzAAATQUlEQVR42uxd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" alt="ESTRUPLAST ERP" class="logo-img" />
      </div>

      <nav class="menu">
        
        <router-link :to="{ name: 'produccion' }" class="nav-btn" active-class="activo">
            ⚙️ Producción
        </router-link>

        <router-link :to="{ name: 'ingreso-stock' }" class="nav-btn" active-class="activo">
            🚚 Ingreso Stock
        </router-link>

        <router-link :to="{ name: 'remitos' }" class="nav-btn" active-class="activo">
            🚚 Remitos / Despacho
        </router-link>

        <router-link :to="{ name: 'inventario' }" class="nav-btn" active-class="activo">
            📊 Inventario
        </router-link>

        <router-link :to="{ name: 'configuracion' }" class="nav-btn" active-class="activo">
            ⚙️ Configuración
        </router-link>
      
      </nav>
    </header>

    <main>
      <router-view></router-view>
    </main>

  </div>
</template>

<style scoped>
/* Contenedor central (Layout) */
.contenedor-principal {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  max-width: 1200px;
  margin: 0 auto;
  padding: 20px;
  background-color: #fafafa;
  min-height: 100vh;
}

/* Cabecera */
header {
  margin-bottom: 30px;
  text-align: center;
  position: relative;
}

/* Info de Usuario */
.usuario-info {
    position: absolute;
    top: 0;
    right: 0;
    font-size: 0.9em;
    color: #555;
    background: white;
    padding: 8px 15px;
    border-radius: 20px;
    box-shadow: 0 2px 5px rgba(0,0,0,0.1);
    display: flex;
    align-items: center;
    gap: 10px;
}

.btn-salir {
    background: #c0392b;
    color: white;
    border: none;
    padding: 5px 10px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 0.8em;
    transition: background 0.3s;
}
.btn-salir:hover { background: #a93226; }

/* Menú de Navegación */
.menu {
  display: flex;
  justify-content: center;
  gap: 10px;
  flex-wrap: wrap; 
  margin-top: 20px;
}

/* Estilos para los enlaces del Router */
.nav-btn {
  padding: 10px 20px;
  border: none;
  background: #e0e0e0;
  cursor: pointer;
  font-size: 15px;
  border-radius: 6px;
  transition: all 0.2s;
  color: #555;
  text-decoration: none;
  display: inline-block;
}

.nav-btn:hover { background: #d5d5d5; }

/* Vue Router aplica esta clase automáticamente cuando estás en esa ruta */
.nav-btn.activo {
  background: #E67E22; 
  color: white;
  font-weight: bold;
  box-shadow: 0 2px 5px rgba(0,0,0,0.2);
}
</style>