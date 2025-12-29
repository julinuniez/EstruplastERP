import { createRouter, createWebHashHistory, type RouteRecordRaw } from 'vue-router';

// --- IMPORTACIÓN DE COMPONENTES ---
import Login from '../components/Login.vue';
import VistaProduccion from '../components/VistaProduccion.vue';
import ListaStock from '../components/ListaStock.vue';
import IngresoStock from '../components/IngresoStock.vue';
import VistaRemitos from '../components/VistaRemitos.vue';
import DespachoRemitos from '../components/DespachoRemitos.vue';
import Administracion from '../components/Administracion.vue';
import EditarProducto from '../components/EditarProducto.vue';

const routes: Array<RouteRecordRaw> = [
    // --- LOGIN ---
    { 
        path: '/login', 
        name: 'login', 
        component: Login,
        meta: { requiresAuth: false } 
    },

    // --- HOME (Redirección) ---
    { 
        path: '/', 
        redirect: { name: 'produccion' } 
    }, 

    // --- PRODUCCIÓN ---
    { 
        path: '/produccion', 
        name: 'produccion', 
        component: VistaProduccion,
        meta: { requiresAuth: true } 
    },

    // --- INVENTARIO ---
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

    // --- REMITOS ---
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

    {
        path: '/editar-producto/:id', // :id es el parámetro dinámico
        name: 'editar-producto',
        component: EditarProducto
    },
];

const router = createRouter({
    history: createWebHashHistory(),
    routes: routes // <--- AQUÍ ESTABA EL ERROR: Ahora pasamos la constante completa
});

// --- GUARDIA DE NAVEGACIÓN (Auth) ---
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