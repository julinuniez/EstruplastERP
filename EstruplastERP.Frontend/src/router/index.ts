import { createRouter, createWebHashHistory, type RouteRecordRaw } from 'vue-router';

// Asegúrate de que los archivos estén físicamente en la carpeta 'views'
import Login from '../views/Login.vue';
import VistaDashboard from '../views/VistaDashboard.vue';        
import FormularioProduccion from '../views/FormularioProduccion.vue'; 
import GestionProductos from '../views/GestionProductos.vue';     
import IngresoStock from '../views/IngresoStock.vue';
import VistaGestionScrap from '../views/VistaGestionScrap.vue';   
import VistaRemitos from '../views/VistaRemitos.vue';
import DespachoRemitos from '../views/DespachoRemitos.vue';
import Administracion from '../views/Administracion.vue';
import EditarProducto from '../views/EditarProducto.vue';

const routes: Array<RouteRecordRaw> = [
    // --- LOGIN ---
    { 
        path: '/login', 
        name: 'login', 
        component: Login,
        meta: { requiresAuth: false } 
    },

    // --- HOME (Redirección a Producción) ---
    { 
        path: '/', 
        redirect: { name: 'produccion' } 
    }, 

    // --- PRODUCCIÓN ---
    { 
        path: '/produccion', 
        name: 'produccion', 
        component: FormularioProduccion,
        meta: { requiresAuth: true } 
    },

    // --- DASHBOARD (BI) ---
    { 
        path: '/dashboard', 
        name: 'dashboard', 
        component: VistaDashboard,
        meta: { requiresAuth: true } 
    },

    // --- RECUPERADO / SCRAP ---
    { 
        path: '/scrap', 
        name: 'scrap', 
        component: VistaGestionScrap,
        meta: { requiresAuth: true } 
    },

    // --- GESTIÓN PRODUCTOS (INVENTARIO) ---
    { 
        path: '/productos', 
        name: 'inventario', // Mantenemos 'inventario' como pediste
        component: GestionProductos, 
        meta: { requiresAuth: true }
    },
    {
  path: '/ingreso-scrap',
  name: 'ingreso-scrap',
  component: () => import('../views/IngresoScrap.vue')
},
    
    {
        path: '/tablero-pedidos',
        name: 'TableroPedidos',
        component: () => import('../views/TableroPedidos.vue')
    },

    {
        path: '/editar-producto/:id', 
        name: 'editar-producto',
        component: EditarProducto,
        props: true, 
        meta: { requiresAuth: true }
    },

    // --- COMPRAS / INGRESO STOCK ---
    { 
        path: '/ingreso-stock', 
        name: 'ingreso-stock', 
        component: IngresoStock,
        meta: { requiresAuth: true }
    },

    // --- LOGÍSTICA / REMITOS ---
    { 
        path: '/remitos', 
        name: 'remitos', 
        component: VistaRemitos,
        meta: { requiresAuth: true } 
    },
    { 
        path: '/remitos/nuevo', 
        name: 'DespachoRemitos', 
        component: DespachoRemitos,
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
    history: createWebHashHistory(),
    routes: routes 
});

// --- GUARDIA DE NAVEGACIÓN ---
router.beforeEach((to, from, next) => {
    const token = localStorage.getItem('token');
    const requiereAuth = to.matched.some(record => record.meta.requiresAuth);

    if (requiereAuth && !token) {
        next({ name: 'login' });
    } 
    else if (to.name === 'login' && token) {
        // Si ya está logueado, redirigir a Producción
        next({ name: 'produccion' }); 
    } 
    else {
        next();
    }
});

export default router;