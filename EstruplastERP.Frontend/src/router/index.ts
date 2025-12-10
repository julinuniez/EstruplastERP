import { createRouter, createWebHistory, type RouteRecordRaw } from 'vue-router';

// 1. Importamos TODOS tus componentes
import VistaProduccion from '../components/VistaProduccion.vue'; // La vista combinada que acabamos de crear
import GestionProductos from '../components/GestionProductos.vue';
import ListaStock from '../components/ListaStock.vue';
import IngresoStock from '../components/IngresoStock.vue';     // 🚚 Recuperado
import EditorRecetas from '../components/EditorRecetas.vue';   // 🧪 Recuperado
import Administracion from '../components/Administracion.vue'; // ⚙️ Recuperado (Clientes/Empleados)
import Login from '../components/Login.vue';
import DespachoRemitos from '../components/DespachoRemitos.vue';

const routes: Array<RouteRecordRaw> = [
    { path: '/login', name: 'login', component: Login },
    { path: '/', redirect: { name: 'produccion' } }, // Redirigir a producción al entrar

    // ⚙️ Producción (Ahora usa la vista combinada)
    { path: '/produccion', name: 'produccion', component: VistaProduccion },

    // 📦 Productos (Crear y Editar)
    { path: '/productos/nuevo', name: 'crear-producto', component: GestionProductos },
    { path: '/productos/editar/:id', name: 'editar-producto', component: GestionProductos },

    // 📊 Inventario
    { path: '/inventario', name: 'inventario', component: ListaStock },

    // 🚚 Ingreso Stock (RECUPERADO)
    { path: '/ingreso-stock', name: 'ingreso-stock', component: IngresoStock },

    // 🧪 Fórmulas (RECUPERADO)
    { path: '/formulas', name: 'formulas', component: EditorRecetas },

    // ⚙️ Configuración / Admin (RECUPERADO)
    { path: '/configuracion', name: 'configuracion', component: Administracion },
    // 🚛 Despacho y Remitos (RECUPERADO
    { 
    path: '/remitos', 
    name: 'remitos', 
    component: DespachoRemitos 
}];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;