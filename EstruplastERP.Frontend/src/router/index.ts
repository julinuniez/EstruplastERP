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
import VistaRemitos from '@/components/VistaRemitos.vue';

const routes: Array<RouteRecordRaw> = [
    // RUTA PÚBLICA: Login
    { 
        path: '/login', 
        name: 'login', 
        component: Login,
        meta: { requiresAuth: false } // No requiere autenticación
    },

    // RUTAS PRIVADAS (Todas tienen meta: requiresAuth: true)
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

    { 
        path: '/productos/nuevo', 
        name: 'crear-producto', 
        component: GestionProductos,
        meta: { requiresAuth: true }
    },
    
    { 
        path: '/productos/editar/:id', 
        name: 'editar-producto', 
        component: GestionProductos,
        meta: { requiresAuth: true }
    },

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

    { 
        path: '/formulas', 
        name: 'formulas', 
        component: EditorRecetas,
        meta: { requiresAuth: true }
    },

    { 
        path: '/configuracion', 
        name: 'configuracion', 
        component: Administracion,
        meta: { requiresAuth: true }
    },
    
    { 
        path: '/remitos', 
        name: 'remitos', 
        component: DespachoRemitos,
        meta: { requiresAuth: true }
    },
    {
      path: '/remitos',
      name: 'remitos',
      component: VistaRemitos,
      meta: { requiresAuth: true } // Si usas protección de rutas
    },
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

// ========================================================
// 🛡️ GUARDIA DE NAVEGACIÓN (El Portero)
// ========================================================
router.beforeEach((to, from, next) => {
    // 1. Buscamos el token en localStorage
    const token = localStorage.getItem('token');
    
    // 2. Verificamos si la ruta a la que va requiere autenticación
    // (Usamos to.matched.some por si usas rutas hijas anidadas)
    const requiereAuth = to.matched.some(record => record.meta.requiresAuth);

    if (requiereAuth && !token) {
        // CASO A: Quiere entrar a zona VIP sin token -> Lo mandamos al Login
        next({ name: 'login' });
    } 
    else if (to.name === 'login' && token) {
        // CASO B: Ya tiene token y quiere ir al Login -> Lo mandamos adentro
        next({ name: 'produccion' });
    } 
    else {
        // CASO C: Todo correcto, pase usted
        next();
    }
});

export default router;