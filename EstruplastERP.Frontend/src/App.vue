<script setup lang="ts">
import { ref, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import { useSesionStore } from './stores/Sesion';
import logoImg from '@/assets/estruplast-logo.png';

const route = useRoute();
const router = useRouter();
const sesion = useSesionStore();
const version = __APP_VERSION__;
const menuReducido = ref(false); 

const mostrarMenu = computed(() => route.name !== 'login');

function cerrarSesion() {
  sesion.cerrar();
  router.push({ name: 'login' });
}

function toggleMenu() {
  menuReducido.value = !menuReducido.value;
}
</script>

<template>
  <div class="app-layout">
    
    <nav v-if="mostrarMenu" class="sidebar" :class="{ 'reducido': menuReducido }">
      <div class="sidebar-header">
        <button @click="toggleMenu" class="btn-toggle" title="Contraer/Expandir">
          <span class="icono-hamburguesa">☰</span>
        </button>
      </div>

      <div class="menu-items">
        <router-link :to="{ name: 'dashboard' }" class="nav-btn" active-class="activo" title="Tablero">
          <span class="icon">📊</span>
          <span class="text" v-show="!menuReducido">Tablero</span>
        </router-link>

        <router-link :to="{ name: 'produccion' }" class="nav-btn" active-class="activo" title="Órdenes de Producción">
          <span class="icon">🏭</span>
          <span class="text" v-show="!menuReducido">Producción</span>
        </router-link>

        <router-link to="/tablero-pedidos" class="nav-btn" active-class="activo" title="Gestión de Pedidos">
          <span class="icon">📋</span>
          <span class="text" v-show="!menuReducido">Pedidos</span>
        </router-link>
        
        <router-link :to="{ name: 'inventario' }" class="nav-btn" active-class="activo" title="Control de Stock">
          <span class="icon">📦</span>
          <span class="text" v-show="!menuReducido">Inventario</span>
        </router-link>

        <router-link :to="{ name: 'ingreso-stock' }" class="nav-btn" active-class="activo" title="Recepción de Materiales">
          <span class="icon">📥</span>
          <span class="text" v-show="!menuReducido">Ingreso Material</span>
        </router-link>

        <router-link :to="{ name: 'Scrap' }" class="nav-btn" active-class="activo" title="Planta de Recuperado">
          <span class="icon">♻️</span>
          <span class="text" v-show="!menuReducido">Molienda</span>
        </router-link>

        <router-link :to="{ name: 'remitos' }" class="nav-btn" active-class="activo" title="Despacho y Logística">
          <span class="icon">🚚</span>
          <span class="text" v-show="!menuReducido">Despacho</span>
        </router-link>

        <router-link :to="{ name: 'configuracion' }" class="nav-btn" active-class="activo" title="Configuración del Sistema">
          <span class="icon">🔧</span>
          <span class="text" v-show="!menuReducido">Configuración</span>
        </router-link>
      </div>
      <div style="margin-top: auto; padding: 10px; font-size: 0.8rem; text-align: center; color: #666; border-top: 1px solid #444;">
        v{{ version }}
      </div>
    </nav>

    <div class="main-wrapper" :class="{ 'margen-reducido': menuReducido, 'margen-normal': !menuReducido, 'full-screen': !mostrarMenu }">
      
      <header v-if="mostrarMenu" class="top-bar">
        
        <div class="left-spacer"></div>

        <div class="center-brand">
          <img :src="logoImg" alt="Logo Empresa" class="logo-central" />
        </div>

        <div class="user-area">
            <span class="user-name">Hola, <strong>{{ sesion.usuario?.nombre || 'Admin' }}</strong></span>
            <button @click="cerrarSesion" class="btn-salir-top" title="Cerrar Sesión">
               Salir
            </button>
        </div>
      </header>

      <main class="page-content">
        <router-view></router-view>
      </main>

    </div>
  </div>
</template>

<style>
/* RESET BÁSICO */
body { margin: 0; padding: 0; font-family: 'Segoe UI', sans-serif; background-color: #f4f6f9; overflow-x: hidden; }
* { box-sizing: border-box; }

.app-layout { display: flex; min-height: 100vh; width: 100%; position: relative; }

/* --- SIDEBAR (FIJO A LA IZQUIERDA) --- */
.sidebar {
  position: fixed;
  top: 0;
  left: 0;
  height: 100vh;
  background-color: #2c3e50;
  color: white;
  display: flex;
  flex-direction: column;
  z-index: 1000;
  transition: width 0.3s ease;
  width: 200px; /* Ya estaba en 200px */
  box-shadow: 2px 0 5px rgba(0,0,0,0.1);
}

.sidebar.reducido {
  width: 60px;
}

.sidebar-header { height: 60px; display: flex; align-items: center; justify-content: center; background: #243444; border-bottom: 1px solid #3e5871; }
.btn-toggle { background: none; border: none; color: white; font-size: 1.5rem; cursor: pointer; }
.menu-items { flex: 1; overflow-y: auto; padding: 10px 0; overflow-x: hidden; }
.nav-btn { display: flex; align-items: center; padding: 12px 15px; color: #bdc3c7; text-decoration: none; white-space: nowrap; transition: background 0.2s; height: 45px; }
.nav-btn:hover { background-color: #34495e; color: white; }
.nav-btn.activo { background-color: #3498db; color: white; border-left: 4px solid #fff; }
.icon { font-size: 1.2rem; min-width: 30px; text-align: center; display: inline-block; }
.text { margin-left: 10px; font-weight: 500; white-space: nowrap; opacity: 1; transition: opacity 0.2s; }
.sidebar.reducido .text { display: none; }

/* --- WRAPPER DERECHO (CONTENIDO) --- */
.main-wrapper {
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  transition: margin-left 0.3s ease, width 0.3s ease;
  background-color: #f4f6f9;
}

/* 🚀 AJUSTE: Cambiamos 240px por 200px para que coincida con el sidebar */
.main-wrapper.margen-normal { margin-left: 200px; width: calc(100% - 200px); }
.main-wrapper.margen-reducido { margin-left: 60px; width: calc(100% - 60px); }
.main-wrapper.full-screen { margin-left: 0; width: 100%; }

/* --- TOP BAR --- */
.top-bar {
  height: 60px;
  background-color: white;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.05);
  position: sticky;
  top: 0;
  z-index: 900;
}

/* Elemento central absoluto */
.center-brand {
  position: absolute;
  left: 50%;
  top: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  justify-content: center;
  width: 100%;
  pointer-events: none;
}

.logo-central {
  max-height: 40px;
  width: auto;
  max-width: 400px;
  object-fit: contain;
  pointer-events: auto;
}

.user-area { display: flex; align-items: center; gap: 15px; }
.user-name { color: #555; font-size: 0.95rem; }
.avatar-circle { width: 35px; height: 35px; background-color: #3498db; color: white; border-radius: 50%; display: flex; align-items: center; justify-content: center; font-weight: bold; }
.btn-salir-top { padding: 5px 10px; background: #ffebee; color: #c62828; border: 1px solid #ef9a9a; border-radius: 4px; cursor: pointer; font-weight: bold; font-size: 0.8rem; transition: background 0.2s; }
.btn-salir-top:hover { background: #ffcdd2; }

/* CONTENIDO DE PÁGINA */
.page-content { padding: 20px; flex: 1; overflow-x: auto; }

/* RESPONSIVE */
@media (max-width: 768px) {
  /* 🚀 AJUSTE RESPONSIVE: Mantenemos los 200px */
  .sidebar { transform: translateX(-100%); width: 200px; }
  .sidebar.reducido { transform: translateX(0); width: 60px; }
  
  .main-wrapper.margen-normal, .main-wrapper.margen-reducido { margin-left: 0; width: 100%; }
  .sidebar.reducido + .main-wrapper { margin-left: 60px; width: calc(100% - 60px); }
  
  .user-name { display: none; }
  .logo-central { max-height: 30px; }
}
</style>