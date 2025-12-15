import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';

// 1. Importamos tus componentes
import VistaProduccion from '../components/VistaProduccion.vue';
import GestionProductos from '../components/GestionProductos.vue';
import ListaStock from '../components/ListaStock.vue';
import IngresoStock from '../components/IngresoStock.vue';
import EditorRecetas from '../components/EditorRecetas.vue';
import Administracion from '../components/Administracion.vue';
import Login from '../components/Login.vue';
import DespachoRemitos from '../components/DespachoRemitos.vue';
// import VistaRemitos from '@/components/VistaRemitos.vue'; // ⚠️ Comentado por duplicado
// import FormularioProducto from '@/components/FormularioProducto.vue'; // ⚠️ No lo necesitas si usas GestionProductos para todo

const routes: Array<RouteRecordRaw> = [
    // RUTA PÚBLICA: Login
    { 
        path: '/login', 
        name: 'login', 
        component: Login,
        meta: { requiresAuth: false } 
    },

    // RUTAS PRIVADAS
    { 
        path: '/', 
        redirect: { name: 'produccion' } 
    }, 

    { 
        path: '/produccion', 
        name: 'produccion', 
        component: VistaProduccion,
        meta: { requiresAuth: true } 
    },

    // --- GESTIÓN DE PRODUCTOS (Lista y Edición en el mismo componente) ---
    { 
        path: '/productos', 
        name: 'GestionProductos', // Ruta principal de la lista
        component: GestionProductos,
        meta: { requiresAuth: true }
    },
    { 
        path: '/productos/nuevo', 
        name: 'crear-producto', 
        component: GestionProductos, // Reusamos el mismo componente
        meta: { requiresAuth: true }
    },
    { 
        path: '/productos/editar/:id', 
        name: 'EditarProducto', // 🔥 Este nombre debe coincidir con el router.push
        component: GestionProductos, // Reusamos el mismo componente
        meta: { requiresAuth: true },
        props: true 
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

    // --- FÓRMULAS ---
    { 
        path: '/formulas', 
        name: 'formulas', 
        component: EditorRecetas,
        meta: { requiresAuth: true }
    },

    // --- CONFIGURACIÓN ---
    { 
        path: '/configuracion', 
        name: 'configuracion', 
        component: Administracion,
        meta: { requiresAuth: true }
    },
    
    // --- REMITOS ---
    { 
        path: '/remitos', 
        name: 'remitos', 
        component: DespachoRemitos, // Elige uno de los dos componentes
        meta: { requiresAuth: true }
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

// ========================================================
// 🛡️ GUARDIA DE NAVEGACIÓN
// ========================================================
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