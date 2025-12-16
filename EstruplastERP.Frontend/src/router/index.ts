import { createRouter, createWebHashHistory, type RouteRecordRaw } from 'vue-router';

// 1. Componentes Esenciales
import VistaProduccion from '../components/VistaProduccion.vue'; // La pantalla principal
import ListaStock from '../components/ListaStock.vue';           // Para ver stock
import IngresoStock from '../components/IngresoStock.vue';       // Para reponer stock
import Administracion from '../components/Administracion.vue';   // Configuración (Usuarios/Turnos)
import Login from '../components/Login.vue';

// --- REMITOS ---
import DespachoRemitos from '../components/DespachoRemitos.vue';
import VistaRemitos from '../components/VistaRemitos.vue';

// ⚠️ NOTA: Ya no importamos 'GestionProductos' ni 'EditorRecetas' 
// porque eso ahora es estático desde el código.

const routes: Array<RouteRecordRaw> = [
    // --- LOGIN ---
    { 
        path: '/login', 
        name: 'login', 
        component: Login,
        meta: { requiresAuth: false } 
    },

    // --- HOME (Redirige a producción) ---
    { 
        path: '/', 
        redirect: { name: 'produccion' } 
    }, 

    // --- MÓDULO PRINCIPAL: PRODUCCIÓN ---
    { 
        path: '/produccion', 
        name: 'produccion', 
        component: VistaProduccion,
        meta: { requiresAuth: true } 
    },

    // --- MÓDULO: INVENTARIO (Solo ver cantidades y reponer) ---
    { 
        path: '/inventario', 
        name: 'inventario', 
        component: ListaStock,
        meta: { requiresAuth: true }
    },
    { 
        path: '/ingreso-stock', 
        name: 'ingreso-stock', 
        component: IngresoStock,
        meta: { requiresAuth: true }
    },

    // --- MÓDULO: REMITOS ---
    { 
        path: '/remitos', 
        name: 'remitos', 
        component: VistaRemitos, // Historial
        meta: { requiresAuth: true }
    },
    { 
        path: '/remitos/nuevo', 
        name: 'DespachoRemitos', 
        component: DespachoRemitos, // Crear Nuevo
        meta: { requiresAuth: true }
    },

    // --- CONFIGURACIÓN ---
    { 
        path: '/configuracion', 
        name: 'configuracion', 
        component: Administracion,
        meta: { requiresAuth: true }
    },
];

const router = createRouter({
    history: createWebHashHistory(), // Usamos Hash para evitar problemas de recarga
    routes,
});

// Guardia de navegación (Auth)
router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    const requiereAuth = to.matched.some(record => record.meta.requiresAuth);

    if (requiereAuth && !token) {
        next({ name: 'login' });
    } 
    else if (to.name === 'login' && token) {
        next({ name: 'produccion' });
    } 
    else {
        next();
    }
});

export default router;