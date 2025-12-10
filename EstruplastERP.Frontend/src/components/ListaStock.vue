<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router'; 

// --- Estado Reactivo (Datos) ---
const listaInventario = ref([]); 
const busqueda = ref(''); 
const mostrandoModal = ref(false); 
const itemAjustar = ref({}); 
const router = useRouter(); 

// --- Lógica (Carga desde la API real) ---

const cargarInventario = async () => {
    console.log('🔄 Cargando inventario real...');
    const API_URL = 'https://localhost:7244/api/Productos/inventario-completo'; 
    
    try {
        const res = await fetch(API_URL);

        if (!res.ok) {
            throw new Error(`Fallo la carga de inventario: ${res.statusText}`);
        }

        const datosReales = await res.json();
        
        listaInventario.value = datosReales.map(item => ({
            ...item,
            stockMinimo: item.stockMinimo || 0, 
            estado: item.stockActual < item.stockMinimo ? 'CRITICO' : 'OK'
        }));

    } catch (e) {
        console.error("❌ Error al cargar el inventario desde la API:", e);
        listaInventario.value = []; 
    }
};

onMounted(() => {
    cargarInventario();
});


// --- Propiedades Computadas (KPIs y Filtrado) ---
const listaFiltrada = computed(() => {
    const busquedaTexto = busqueda.value.toLowerCase().trim();
    if (!busquedaTexto) {
        return listaInventario.value;
    }
    return listaInventario.value.filter(item =>
        item.nombre.toLowerCase().includes(busquedaTexto) ||
        item.codigoSku.toLowerCase().includes(busquedaTexto)
    );
});
const totalItems = computed(() => listaInventario.value.length);
const itemsCriticos = computed(() => listaInventario.value.filter(item => item.estado === 'CRITICO').length);

// ❌ VALORIZACIÓN: Mantenemos la computed property pero no se usará en el template.
const valorTotalInventario = computed(() => {
    return listaInventario.value.reduce((total, item) => {
        const stock = Number(item.stockActual) || 0;
        const costo = Number(item.precioCosto) || 0;
        return total + (stock * costo);
    }, 0);
});


// --- Métodos (Acciones del Usuario) ---

// 1. Abre el modal de ajuste de stock
const abrirAjuste = (item) => {
    itemAjustar.value = { 
        ...item, 
        stockRealNuevo: item.stockActual,
        motivo: ''
    }; 
    mostrandoModal.value = true;
};

const guardarAjuste = async () => {
    // A. Validaciones básicas en el frontend
    if (itemAjustar.value.stockRealNuevo < 0 || itemAjustar.value.stockRealNuevo === '') {
        alert('❌ El stock real no puede ser negativo ni estar vacío.');
        return;
    }
    if (!itemAjustar.value.motivo) {
        alert('⚠️ Por favor, indica un motivo para el ajuste (Ej: Rotura, Conteo).');
        return;
    }

    try {
        // B. Preparamos los datos para el DTO "AjusteDto" de C#
        const payload = {
            productoId: itemAjustar.value.id,
            cantidadReal: Number(itemAjustar.value.stockRealNuevo), // Aseguramos que sea número
            motivo: itemAjustar.value.motivo
        };

        // C. Enviamos al endpoint correcto (StockController)
        const res = await fetch('https://localhost:7244/api/Stock/ajuste', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
        });

        // D. Manejo de respuesta
        if (!res.ok) {
            const data = await res.json().catch(() => ({ mensaje: 'Error del servidor' }));
            throw new Error(data.mensaje || 'No se pudo guardar el ajuste.');
        }

        const respuestaExito = await res.json();
        alert(`✅ ${respuestaExito.mensaje}`);

        // E. Cerramos modal y recargamos la tabla para ver el cambio
        mostrandoModal.value = false;
        await cargarInventario(); 

    } catch (e) {
        console.error(e);
        alert("❌ Error al ajustar stock: " + e.message);
    }
};
// 3. 🚨 IMPLEMENTACIÓN REAL DE LA ELIMINACIÓN
const eliminarProducto = async (id, nombre) => {
    if (confirm(`¿Estás seguro de que quieres ELIMINAR DEFINITIVAMENTE el producto ${nombre} (ID: ${id})? Esta acción no se puede deshacer.`)) {
        
        try {
            const API_DELETE_URL = `https://localhost:7244/api/Productos/eliminar/${id}`;
            
            const res = await fetch(API_DELETE_URL, {
                method: 'DELETE'
            });

            if (!res.ok) {
                const errorData = await res.json().catch(() => ({ mensaje: 'Error desconocido del servidor.' }));
                alert(`❌ No se pudo eliminar el producto: ${errorData.mensaje}`);
                return;
            }

            // Éxito: Eliminar de la lista local
            listaInventario.value = listaInventario.value.filter(item => item.id !== id);
            alert(`✅ Producto ${nombre} eliminado con éxito.`);
            mostrandoModal.value = false;

        } catch (error) {
            alert("🚨 Error de red o conexión al intentar eliminar el producto.");
            console.error("Error de eliminación:", error);
        }
    }
};

// 4. Implementación del botón Nuevo Producto (Creación)
const abrirCreacion = () => {
    console.log("🟢 CLICK DETECTADO: Intentando navegar a crear-producto"); // <--- AGREGA ESTO
    console.log("Router instance:", router); // <--- Y ESTO
    router.push({ name: 'crear-producto' })
        .then(() => console.log("Navegación exitosa"))
        .catch((err) => console.error("Error de navegación:", err));
};

// 5. 🚨 IMPLEMENTACIÓN DEL BOTÓN EDITAR DATOS (Precio, Nombre, etc.)
const editarDatos = (id) => {
    // Navega a la ruta de edición, pasando el ID como parámetro
    router.push({ name: 'editar-producto', params: { id: id } }); 
};
</script>

<template>
  <div class="panel-stock">
    <h2>📦 Inventario General</h2>

    <div class="kpi-container">
      <div class="card-kpi"><span>Items</span><strong>{{ totalItems }}</strong></div>
      <div class="card-kpi danger"><span>⚠️ Críticos</span><strong>{{ itemsCriticos }}</strong></div>
    </div>

    <div class="buscador">
      <input v-model="busqueda" type="text" placeholder="🔍 Buscar..." />
      <button @click="cargarInventario">🔄</button>
      <button class="btn-nuevo" @click="abrirCreacion">➕ Nuevo Producto</button>
    </div>

    <table class="tabla-stock">
      <thead>
        <tr>
          <th>SKU</th>
          <th>Nombre</th>
          <th>Stock</th>
          <th>Estado</th>
          <th>Acción</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="item in listaFiltrada" :key="item.id" :class="{ 'fila-critica': item.estado === 'CRITICO' }">
          <td>{{ item.codigoSku }}</td>
          <td>{{ item.nombre }}</td>
          <td class="numero">{{ item.stockActual }}</td>
          <td>
            <span v-if="item.estado === 'CRITICO'" class="alerta">⚠️ BAJO</span><span v-else class="ok">✅ OK</span>
          </td>
          <td>
            <button class="btn-editar" @click="abrirAjuste(item)">✏️ Ajustar</button>
            <button class="btn-editar-datos" @click="editarDatos(item.id)">⚙️ Editar</button>
          </td>
        </tr>
        
        <tr v-if="listaInventario.length === 0">
            <td colspan="5" style="text-align: center;">Cargando inventario...</td>
        </tr>
        <tr v-else-if="listaFiltrada.length === 0">
            <td colspan="5" style="text-align: center;">No se encontraron productos.</td>
        </tr>
      </tbody>
    </table>

    <div v-if="mostrandoModal" class="modal-overlay">
      <div class="modal-content">
        <h3>Corrección de Stock</h3>
        <p>Producto: <strong>{{ itemAjustar.nombre }}</strong></p>

        <div class="campo">
          <label>Stock en Sistema:</label>
          <input type="text" :value="itemAjustar.stockActual" disabled style="background: #eee;">
        </div>

        <div class="campo">
          <label>Stock Real (Conteo Físico):</label>
          <input type="number" v-model.number="itemAjustar.stockRealNuevo" class="input-grande">
        </div>

        <div class="campo">
          <label>Motivo del cambio:</label>
          <input type="text" v-model="itemAjustar.motivo" placeholder="Ej: Rotura, Error de carga">
        </div>

        <div class="botones-modal">
          <button @click="mostrandoModal = false" class="btn-cancelar">Cancelar</button>
          <button @click="guardarAjuste" class="btn-guardar" :disabled="!itemAjustar.motivo">💾 Confirmar</button>
          <button class="btn-del" @click="eliminarProducto(itemAjustar.id, itemAjustar.nombre)">🗑️</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
/* Contenedor principal */
.panel-stock {
  padding: 20px;
  background-color: #f4f7f9;
  border-radius: 8px;
  font-family: Arial, sans-serif;
}

.btn-editar-datos {
    background-color: #3498db; /* Azul para edición de datos */
    color: white;
    border: none;
    padding: 6px 12px;
    border-radius: 4px;
    cursor: pointer;
    font-size: 14px;
    margin-left: 5px;
}

h2 {
  color: #333;
  border-bottom: 2px solid #ddd;
  padding-bottom: 10px;
  margin-bottom: 20px;
}

/* --- KPIs --- */
.kpi-container {
  display: flex;
  gap: 20px;
  margin-bottom: 25px;
}

.card-kpi {
  flex-grow: 1;
  padding: 15px;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  border-left: 5px solid #007bff;
}

.card-kpi span {
  display: block;
  font-size: 14px;
  color: #666;
  margin-bottom: 5px;
}

.card-kpi strong {
  font-size: 24px;
  color: #333;
}

.card-kpi.danger {
  border-left-color: #dc3545;
}

.card-kpi.danger strong {
  color: #dc3545;
}

.card-kpi.money {
  border-left-color: #28a745;
}

/* --- Buscador --- */
.buscador {
  display: flex;
  gap: 10px;
  margin-bottom: 20px;
}

.buscador input[type="text"] {
  flex-grow: 1;
  padding: 10px 15px;
  border: 1px solid #ccc;
  border-radius: 6px;
  font-size: 16px;
}

.buscador button {
  padding: 10px 15px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  background-color: #007bff;
  color: white;
  font-size: 16px;
}

.buscador button.btn-nuevo {
    background-color: #28a745;
}

/* --- Tabla --- */
.tabla-stock {
  width: 100%;
  border-collapse: collapse;
  background: #fff;
  border-radius: 8px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.tabla-stock th, .tabla-stock td {
  padding: 12px 15px;
  text-align: left;
  border-bottom: 1px solid #eee;
}

.tabla-stock thead {
  background-color: #e9ecef;
}

.tabla-stock th {
  font-weight: bold;
  color: #495057;
}

.tabla-stock tbody tr:hover {
  background-color: #f8f9fa;
}

.fila-critica {
  background-color: #fff3f3; /* Fondo ligeramente rojo para críticas */
}

.fila-critica td {
  color: #a00;
}

.alerta {
  color: #dc3545;
  font-weight: bold;
  padding: 4px 8px;
  border-radius: 4px;
  background: #f8d7da;
}

.ok {
  color: #28a745;
  font-weight: bold;
  padding: 4px 8px;
  border-radius: 4px;
  background: #d4edda;
}

.numero {
  text-align: right; /* Alineación para números */
  font-weight: bold;
}

/* Botones de acción */
.btn-editar {
  background-color: #ffc107;
  color: #333;
  border: none;
  padding: 6px 12px;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
}

/* --- Modal --- */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 8px;
  width: 450px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.modal-content h3 {
  border-bottom: 2px solid #ddd;
  padding-bottom: 10px;
  margin-bottom: 20px;
  color: #007bff;
}

.campo {
  margin-bottom: 15px;
}

.campo label {
  display: block;
  font-weight: bold;
  margin-bottom: 5px;
  color: #555;
}

.campo input[type="text"], .campo input[type="number"] {
  width: 90%;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px;
}

.campo input.input-grande {
    font-size: 18px;
    font-weight: bold;
    color: #007bff;
    text-align: center;
}

.botones-modal {
  display: flex;
  justify-content: space-between;
  margin-top: 30px;
}

.btn-cancelar, .btn-guardar, .btn-del {
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: bold;
}

.btn-cancelar {
  background-color: #6c757d;
  color: white;
}

.btn-guardar {
  background-color: #007bff;
  color: white;
  flex-grow: 1;
  margin: 0 10px;
}

.btn-guardar:disabled {
    background-color: #a0c9f1;
    cursor: not-allowed;
}

.btn-del {
  background-color: #dc3545;
  color: white;
  padding: 10px;
}
</style>