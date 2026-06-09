<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import api from '@/services/axiosInstance';
import { Alertas } from '@/utils/alertas';

const route = useRoute();
const router = useRouter();
const loading = ref(true);
const guardando = ref(false);

const listaMateriasPrimas = ref([]); 

const producto = ref({
    id: 0,
    nombre: '',
    codigoSku: '',
<<<<<<< HEAD
    pesoEspecifico: 1.1,
    stockMinimo: 0,
    precioCosto: 0,
    rubro: '',
=======
    stockActual: 0,
    stockMinimo: 0,
    pesoEspecifico: 1.0,
>>>>>>> master
    esMateriaPrima: false,
    esProductoTerminado: false,
    esFazon: false,
    receta: [] 
});

const ingredienteSeleccionado = ref('');
const cantidadIngrediente = ref('');

<<<<<<< HEAD
const mostrarCalculadora = ref(false);
const calcPorcentajeCapa = ref(20);
const calcPorcentajeInterno = ref(99.92);

=======
// Configuración de Headers (Token)
const getConfig = () => {
    const token = localStorage.getItem('token');
    return { headers: { Authorization: `Bearer ${token}` } };
};

// URL Base (Asegúrate de que coincida con tu variable de entorno o hardcode)
// Si usas proxy en vite, puedes dejar solo '/api'
const apiBase = '/api'; 

// --- COMPUTED ---
>>>>>>> master
const totalPorcentaje = computed(() => {
    if (!producto.value.receta) return 0;
    const suma = producto.value.receta.reduce((acc, item) => acc + Number(item.cantidad), 0);
    return Math.round(suma * 10000) / 10000;
});

<<<<<<< HEAD
const porcentajeProyectado = computed(() => {
    const pCapa = Number(calcPorcentajeCapa.value) || 0;
    const pInterno = Number(calcPorcentajeInterno.value) || 0;
    return (pCapa * pInterno) / 100;
});

const faltaPrecioCosto = computed(() => producto.value.esMateriaPrima && (!producto.value.precioCosto || producto.value.precioCosto <= 0));
const faltaRubro = computed(() => producto.value.esMateriaPrima && !producto.value.rubro);

=======
>>>>>>> master
const puedeGuardar = computed(() => {
    if (guardando.value) return false;
    if (producto.value.esMateriaPrima) {
        if (faltaPrecioCosto.value || faltaRubro.value) return false;
    }
    if (producto.value.esProductoTerminado) {
        return totalPorcentaje.value === 100;
    }
    return true; 
});

<<<<<<< HEAD
const setTipoProducto = (tipo) => {
    producto.value.esProductoTerminado = tipo === 'PT';
    producto.value.esMateriaPrima = tipo === 'MP';
    if (tipo === 'MP') producto.value.receta = [];
};

const getSku = (p) => (p.codigoSku || p.CodigoSku || '').toUpperCase();
const getNombre = (p) => (p.nombre || p.Nombre || '').toUpperCase();

const isRecuperado = (p) => {
    const sku = getSku(p);
    const nom = getNombre(p);
    return !!(p.esScrap || p.EsScrap) || 
           sku.includes('SCRAP') || sku.includes('MOLIDO') || sku.includes('PELET') ||
           nom.includes('SCRAP') || nom.includes('MOLIDO') || nom.includes('PELET');
};

=======
// --- CARGA DE DATOS ---
>>>>>>> master
onMounted(async () => {
    const id = route.params.id;
    if (!id) return router.push('/stock'); 

    try {
<<<<<<< HEAD
        const resProd = await api.get(`/Productos/${id}`);
=======
        const resProd = await axios.get(`${apiBase}/Productos/${id}`, getConfig());
>>>>>>> master
        producto.value = resProd.data;
        if (!producto.value.receta) producto.value.receta = [];
        
        producto.value.rubro = resProd.data.rubro || resProd.data.Rubro || '';
        producto.value.precioCosto = resProd.data.precioCosto || resProd.data.PrecioCosto || 0;

<<<<<<< HEAD
        const resTodos = await api.get('/Productos');
        
        listaMateriasPrimas.value = resTodos.data.filter(p => 
            p.id !== Number(id) && 
            (p.esMateriaPrima || p.EsMateriaPrima) &&
            !isRecuperado(p) 
        ).sort((a, b) => getNombre(a).localeCompare(getNombre(b)));
=======
        const resMP = await axios.get(`${apiBase}/Productos/materias-primas`, getConfig());
        listaMateriasPrimas.value = resMP.data.filter(mp => mp.id !== Number(id));
>>>>>>> master

    } catch (e) {
        console.error(e);
        Alertas.error("Error al cargar datos.");
        router.push('/stock');
    } finally {
        loading.value = false;
    }
});

<<<<<<< HEAD
const procesarIngresoIngrediente = (idInsumo, cantidadAingresar) => {
    if (!idInsumo) return Alertas.advertencia("Seleccione un insumo.");
=======
// --- LÓGICA DE RECETA ---
const agregarIngrediente = () => {
    if (!ingredienteSeleccionado.value) return alert("Seleccione una materia prima.");
>>>>>>> master
    
    const cantidad = Number(cantidadAingresar);
    if (!cantidad || cantidad <= 0) return Alertas.advertencia("Ingrese un porcentaje válido.");

    if (totalPorcentaje.value + cantidad > 100) {
        return Alertas.advertencia(`⚠️ No puedes agregar ${cantidad}%. El total superaría el 100% (Actual: ${totalPorcentaje.value}%).`);
    }

<<<<<<< HEAD
    const mpInfo = listaMateriasPrimas.value.find(m => m.id === idInsumo);
    const existe = producto.value.receta.find(r => r.materiaPrimaId === idInsumo);
    
    if (existe) {
        existe.cantidad = Math.round((Number(existe.cantidad) + cantidad) * 10000) / 10000;
    } else {
        producto.value.receta.push({
            materiaPrimaId: idInsumo,
            nombreInsumo: mpInfo.nombre || mpInfo.Nombre,
            cantidad: Math.round(cantidad * 10000) / 10000
        });
    }
};
=======
    const mpInfo = listaMateriasPrimas.value.find(m => m.id === ingredienteSeleccionado.value);
    const existe = producto.value.receta.find(r => r.materiaPrimaId === ingredienteSeleccionado.value);
    
    if (existe) return alert("Esta materia prima ya está en la receta.");
>>>>>>> master

const agregarIngredienteSimple = () => {
    procesarIngresoIngrediente(ingredienteSeleccionado.value, cantidadIngrediente.value);
    ingredienteSeleccionado.value = '';
    cantidadIngrediente.value = '';
};

const agregarDesdeCalculadora = () => {
    procesarIngresoIngrediente(ingredienteSeleccionado.value, porcentajeProyectado.value);
    ingredienteSeleccionado.value = '';
    calcPorcentajeInterno.value = '';
};

const quitarIngrediente = (index) => {
    producto.value.receta.splice(index, 1);
};

<<<<<<< HEAD
=======
// --- FUNCIÓN ELIMINAR
const eliminarProducto = async () => {
    if (!confirm(`⚠️ ¿Estás SEGURO de eliminar "${producto.value.nombre}"?\n\nEsta acción borrará:\n- El producto\n- Sus movimientos de stock\n- Su fórmula (si es prod. terminado)`)) {
        return;
    }

    guardando.value = true;

    try {
        const response = await axios.delete(`${apiBase}/Productos/eliminar/${producto.value.id}`, getConfig());
        
        alert(response.data.mensaje);
        router.push('/productos'); 

    } catch (e) {
        console.error(e);
        const msgBackend = e.response?.data?.mensaje;
        const msgGeneral = e.message;

        if (msgBackend) {
            alert(msgBackend);
        } else if (e.response?.status === 404) {
            alert("❌ El producto no se encontró (quizás ya fue borrado).");
            router.push('/');
        } else {
            alert("❌ Error al eliminar: " + msgGeneral);
        }
    } finally {
        guardando.value = false;
    }
};
// --- FUNCIÓN GUARDAR ---
>>>>>>> master
const guardarConfiguracion = async () => {
    if (producto.value.esProductoTerminado && totalPorcentaje.value !== 100) {
        return Alertas.advertencia(`⚠️ La receta debe sumar exactamente 100%. Actual: ${totalPorcentaje.value}%`);
    }
    if (producto.value.esMateriaPrima && (!producto.value.rubro || producto.value.precioCosto <= 0)) {
        return Alertas.advertencia(`⚠️ Por favor complete el Rubro y el Precio de Costo (debe ser mayor a cero).`);
    }

    guardando.value = true;
    try {
        const payload = {
            stockMinimo: Number(producto.value.stockMinimo),
            pesoEspecifico: Number(producto.value.pesoEspecifico),
<<<<<<< HEAD
            precioCosto: Number(producto.value.precioCosto),
            rubro: producto.value.rubro,
=======
            stockActual: Number(producto.value.stockActual), // Stock editable
>>>>>>> master
            esMateriaPrima: producto.value.esMateriaPrima,
            esProductoTerminado: producto.value.esProductoTerminado,
            esFazon: producto.value.esFazon,
            receta: producto.value.receta.map(item => ({
                materiaPrimaId: item.materiaPrimaId,
                cantidad: Number(item.cantidad)
            }))
        };

<<<<<<< HEAD
        await api.put(`/Productos/configurar/${producto.value.id}`, payload);
        
        Alertas.exito("Configuración guardada correctamente.");
=======
        await axios.put(`${apiBase}/Productos/configurar/${producto.value.id}`, payload, getConfig());
        
        alert("✅ Datos actualizados correctamente.");
>>>>>>> master
        router.back();
        
    } catch (e) {
        console.error(e);
        const msg = e.response?.data || e.message; 
        Alertas.error("Error al guardar: " + (typeof msg === 'object' ? JSON.stringify(msg) : msg));
    } finally {
        guardando.value = false;
    }
};

const volver = () => {
    if (window.history.length > 1) { router.back(); } else { router.push('/'); }
};
</script>

<template>
    <div class="container-edit">
        <div v-if="loading" class="loading">Cargando datos...</div>
        
        <div v-else class="card-edit">
            <div class="header">
                <h2>⚙️ Configuración de Producto</h2>
                <p class="subtitle">{{ producto.nombre }}</p>
                <div class="badges-header">
                    <span v-if="producto.esProductoTerminado" class="badge-tipo pt">📦 Producto Terminado</span>
                    <span v-else-if="producto.esMateriaPrima" class="badge-tipo mp">🏭 Materia Prima Pura</span>
                    <span v-if="producto.esFazon" class="badge-tipo faz">🤝 Fazon</span>
                </div>
            </div>

            <div class="seccion-box clasificacion-box">
                <h4>🏷️ Clasificación del Material</h4>
                <div class="botones-clasificacion">
                    <button 
                        :class="['btn-class', { active: producto.esProductoTerminado }]" 
                        @click="setTipoProducto('PT')"
                    >
                        📦 Producto Terminado (Lleva Fórmula)
                    </button>
                    <button 
                        :class="['btn-class', { active: producto.esMateriaPrima }]" 
                        @click="setTipoProducto('MP')"
                    >
                        🏭 Materia Prima / Insumo
                    </button>
                </div>
            </div>

            <div class="seccion-box">
<<<<<<< HEAD
                <h4>📊 Parámetros Técnicos y Comerciales</h4>
                <div class="grid-2">
=======
                <h4>📊 Inventario y Datos Técnicos</h4>
                
                <div class="grid-3">
>>>>>>> master
                    <div class="campo">
                        <label>SKU (Flexxus)</label>
                        <input type="text" v-model="producto.codigoSku" disabled class="input-readonly">
                    </div>
                    
                    <div class="campo">
<<<<<<< HEAD
                        <label>🏷️ Rubro <span v-if="producto.esMateriaPrima" style="color:red">*</span></label>
                        <select v-model="producto.rubro" :class="{'input-error': faltaRubro}">
                            <option value="" disabled>-- Seleccionar Rubro --</option>
                            <option value="MATERIA PRIMA">Materia Prima Virgen</option>
                            <option value="ADITIVO">Aditivo</option>
                            <option value="MASTERBATCH">Masterbatch / Color</option>
                            <option value="PRODUCTO TERMINADO">Producto Terminado</option>
                            <option value="OTROS">Otros / Insumos Generales</option>
                        </select>
                        <small v-if="faltaRubro" class="text-error">El rubro es obligatorio</small>
                    </div>

                    <div class="campo mt-2">
                        <label>💵 Precio de Costo (U$D / $) <span v-if="producto.esMateriaPrima" style="color:red">*</span></label>
                        <div class="input-group">
                            <span class="unit-left">$</span>
                            <input 
                                type="number" 
                                v-model.number="producto.precioCosto" 
                                step="0.01" 
                                class="input-money"
                                :class="{'input-error': faltaPrecioCosto}"
                            >
                        </div>
                        <small v-if="faltaPrecioCosto" class="text-error">Ingrese un costo válido > 0</small>
                    </div>

                    <div class="campo mt-2" v-if="producto.esProductoTerminado">
                        <label>🧪 Peso Específico (g/cm³)</label>
                        <div class="input-group">
                            <input type="number" v-model.number="producto.pesoEspecifico" step="0.0001">
                            <span class="unit">g/cm³</span>
                        </div>
                    </div>

                    <div class="campo mt-2">
                        <label>📉 Stock Mínimo (Alerta Kg)</label>
                        <input type="number" v-model.number="producto.stockMinimo" placeholder="Ej: 100">
                    </div>
                </div>
=======
                        <label>📦 Stock Actual (Kg)</label>
                        <div class="input-stock-wrapper">
                            <input type="number" v-model.number="producto.stockActual" class="input-stock" step="0.01">
                        </div>
                    </div>

                    <div class="campo">
                        <label>⚠️ Stock Mínimo (Alerta)</label>
                        <input type="number" v-model.number="producto.stockMinimo" placeholder="Ej: 100">
                    </div>
                </div>

                <div class="campo mt-2">
                    <label>🧪 Peso Específico (g/cm³)</label>
                    <div class="input-group">
                        <input type="number" v-model.number="producto.pesoEspecifico" step="0.0001">
                        <span class="unit">g/cm³</span>
                    </div>
                    <small>Usado para calcular rendimiento.</small>
                </div>
>>>>>>> master
            </div>

            <div v-if="producto.esProductoTerminado" class="seccion-box">
                <div class="header-receta">
                    <h4>📝 Fórmula (Porcentajes)</h4>
                    <div class="total-badge" :class="totalPorcentaje === 100 ? 'ok' : 'error'">
                        Total: {{ totalPorcentaje }}%
                    </div>
                </div>

                <div class="buscador-receta">
                    <select v-model="ingredienteSeleccionado" class="select-mp">
                        <option value="" disabled selected>🔍 Seleccionar Insumo Virgen / Master...</option>
                        <option v-for="mp in listaMateriasPrimas" :key="mp.id" :value="mp.id">
                            {{ mp.codigoSku || mp.CodigoSku }} - {{ mp.nombre || mp.Nombre }}
                        </option>
                    </select>
                    
                    <input v-if="!mostrarCalculadora" type="number" v-model="cantidadIngrediente" placeholder="%" class="input-cant" min="0" max="100" step="0.0001">
                    <button v-if="!mostrarCalculadora" @click="agregarIngredienteSimple" class="btn-add">➕</button>
                    
                    <button @click="mostrarCalculadora = !mostrarCalculadora" class="btn-toggle-calc" :title="mostrarCalculadora ? 'Carga Simple' : 'Usar Calculadora de Capas'">
                        {{ mostrarCalculadora ? '❌ Cerrar Calc.' : '🧮 Calc. de Capas' }}
                    </button>
                </div>

                <div v-if="mostrarCalculadora" class="caja-calculadora">
                    <h5>🧮 Calculadora de Capas</h5>
                    <div class="calc-grid">
                        <div class="campo-calc">
                            <label>1. % de la Capa en la Bobina</label>
                            <input type="number" v-model="calcPorcentajeCapa" placeholder="Ej: 20">
                        </div>
                        <div class="campo-calc">
                            <label>2. % del Insumo en esa Capa</label>
                            <input type="number" v-model="calcPorcentajeInterno" placeholder="Ej: 99.92" step="0.0001">
                        </div>
                        <div class="campo-resultado">
                            <label>Porcentaje Real Resultante:</label>
                            <div class="resultado-numero">{{ porcentajeProyectado.toFixed(4) }} %</div>
                        </div>
                    </div>
                    <button @click="agregarDesdeCalculadora" class="btn-add-calc">➕ Agregar Insumo Calculado</button>
                </div>

                <div class="tabla-receta-wrapper">
                    <table class="tabla-receta">
                        <thead>
                            <tr>
                                <th>Insumo</th>
                                <th width="120" class="text-center">Porcentaje</th>
                                <th width="40"></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="(item, index) in producto.receta" :key="index">
                                <td>{{ item.nombreInsumo }}</td>
                                <td class="text-center font-bold">{{ item.cantidad }} %</td>
                                <td>
                                    <button @click="quitarIngrediente(index)" class="btn-x">×</button>
                                </td>
                            </tr>
                            <tr v-if="producto.receta.length === 0">
<<<<<<< HEAD
                                <td colspan="3" class="text-center text-muted">Aún no hay ingredientes.</td>
=======
                                <td colspan="3" class="text-center text-muted">Agregue materias primas para completar el 100%.</td>
>>>>>>> master
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>

            <div class="footer-actions">
<<<<<<< HEAD
                <button @click="volver" class="btn-cancelar" :disabled="guardando">Cancelar</button>
                <button @click="guardarConfiguracion" class="btn-guardar" :disabled="!puedeGuardar">
                    <span v-if="guardando">Guardando...</span>
                    <span v-else-if="faltaPrecioCosto || faltaRubro">⚠️ Faltan Datos Obligatorios</span>
                    <span v-else-if="!puedeGuardar">⚠️ Complete el 100%</span>
                    <span v-else>💾 Guardar Todo</span>
=======
                <button @click="eliminarProducto" class="btn-eliminar" :disabled="guardando">
                    🗑️ Eliminar Producto
>>>>>>> master
                </button>

                <div class="acciones-derecha">
                    <button @click="volver" class="btn-cancelar" :disabled="guardando">Cancelar</button>
                    
                    <button @click="guardarConfiguracion" class="btn-guardar" :disabled="!puedeGuardar">
                        <span v-if="guardando">Guardando...</span>
                        <span v-else-if="!puedeGuardar">⚠️ Complete 100%</span>
                        <span v-else>💾 Guardar Cambios</span>
                    </button>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
.container-edit { display: flex; justify-content: center; padding: 20px; background: #f4f6f8; min-height: 100vh; font-family: 'Segoe UI', sans-serif; }
<<<<<<< HEAD
.card-edit { background: white; width: 900px; max-width: 95vw; padding: 30px; border-radius: 8px; box-shadow: 0 4px 15px rgba(0,0,0,0.1); }
=======
.card-edit { 
    background: white; 
    width: 900px; 
    max-width: 95vw;
    padding: 30px; 
    border-radius: 8px; 
    box-shadow: 0 4px 15px rgba(0,0,0,0.1); 
}
>>>>>>> master

.header { text-align: center; margin-bottom: 20px; border-bottom: 1px solid #eee; padding-bottom: 15px; }
.header h2 { margin: 0; color: #2c3e50; }
.subtitle { color: #7f8c8d; margin-top: 5px; font-weight: bold; font-size: 1.1em; }
.badges-header { margin-top: 10px; display: flex; justify-content: center; gap: 10px; }
.badge-tipo { padding: 4px 10px; border-radius: 12px; font-size: 0.85em; font-weight: bold; text-transform: uppercase; }
.badge-tipo.pt { background: #e8f5e9; color: #2e7d32; border: 1px solid #c8e6c9; }
.badge-tipo.mp { background: #fff3e0; color: #ef6c00; border: 1px solid #ffe0b2; }
.badge-tipo.faz { background: #f3e5f5; color: #7b1fa2; border: 1px solid #e1bee7; }

.clasificacion-box { background: #f0f7ff; border-color: #b3d4fc; }
.botones-clasificacion { display: flex; gap: 10px; }
.btn-class { flex: 1; padding: 12px; border: 2px solid #ccc; background: white; border-radius: 6px; cursor: pointer; font-weight: 600; color: #555; transition: all 0.2s; }
.btn-class:hover { border-color: #3498db; background: #f8fbff; }
.btn-class.active { border-color: #3498db; background: #3498db; color: white; box-shadow: 0 2px 8px rgba(52,152,219,0.3); }

.seccion-box { background: #f8f9fa; border: 1px solid #e9ecef; border-radius: 6px; padding: 20px; margin-bottom: 20px; }
.seccion-box h4 { margin-top: 0; color: #3498db; margin-bottom: 15px; border-bottom: 1px solid #eee; padding-bottom: 5px; }

/* GRID DE 3 COLUMNAS */
.grid-3 { display: grid; grid-template-columns: 1fr 1fr 1fr; gap: 15px; }

.campo { display: flex; flex-direction: column; }
.campo label { font-weight: bold; margin-bottom: 5px; color: #555; font-size: 0.9em; }
.campo input, .campo select { padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; font-family: inherit; }
.input-readonly { background: #e9ecef; color: #666; cursor: not-allowed; }

<<<<<<< HEAD
.input-error { border: 2px solid #e74c3c !important; background-color: #fdf2f1; }
.text-error { color: #e74c3c; font-size: 0.8em; font-weight: bold; margin-top: 4px; }
.input-group { position: relative; display: flex; align-items: center; }
=======
/* ESTILO INPUT STOCK */
.input-stock {
    border: 2px solid #3498db !important;
    background-color: #f0f8ff;
    font-weight: bold;
    color: #2c3e50;
    width: 100%;
    box-sizing: border-box;
}
.input-stock:focus { box-shadow: 0 0 5px rgba(52, 152, 219, 0.5); outline: none; }

.mt-2 { margin-top: 15px; }
.input-group { position: relative; }
>>>>>>> master
.input-group input { width: 100%; padding-right: 50px; box-sizing: border-box; }
.input-money { padding-left: 30px !important; font-weight: bold; color: #27ae60; }
.unit { position: absolute; right: 10px; color: #999; }
.unit-left { position: absolute; left: 12px; color: #27ae60; font-weight: bold; font-size: 1.1em; }

<<<<<<< HEAD
=======
/* RECETA */
>>>>>>> master
.header-receta { display: flex; justify-content: space-between; align-items: center; margin-bottom: 15px; }
.total-badge { font-weight: bold; padding: 5px 15px; border-radius: 20px; font-size: 0.9em; }
.total-badge.ok { background: #27ae60; color: white; }
.total-badge.error { background: #c0392b; color: white; animation: pulse 2s infinite; }
<<<<<<< HEAD

.buscador-receta { display: flex; gap: 10px; margin-bottom: 15px; background: white; padding: 10px; border: 1px solid #eee; border-radius: 6px; align-items: center; }
=======
.buscador-receta { display: flex; gap: 10px; margin-bottom: 15px; background: white; padding: 10px; border: 1px solid #eee; border-radius: 6px; }
>>>>>>> master
.select-mp { flex-grow: 1; padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; }
.input-cant { width: 100px; padding: 10px; border: 1px solid #bdc3c7; border-radius: 4px; text-align: center; }
.btn-add { background: #27ae60; color: white; border: none; border-radius: 4px; width: 50px; height: 40px; cursor: pointer; font-size: 1.4em; transition: background 0.2s; }
.btn-add:hover { background: #219150; }
.btn-toggle-calc { background: #34495e; color: white; border: none; padding: 10px 15px; border-radius: 4px; cursor: pointer; font-weight: bold; transition: background 0.2s; }
.btn-toggle-calc:hover { background: #2c3e50; }

.caja-calculadora { background: #e8f4f8; border: 2px dashed #3498db; border-radius: 6px; padding: 15px; margin-bottom: 15px; }
.caja-calculadora h5 { margin: 0 0 10px 0; color: #2980b9; font-size: 1em; }
.calc-grid { display: flex; gap: 15px; margin-bottom: 10px; align-items: flex-end; }
.campo-calc { flex: 1; display: flex; flex-direction: column; gap: 5px; }
.campo-calc label { font-size: 0.85em; font-weight: bold; color: #555; }
.campo-calc input { padding: 8px; border: 1px solid #bdc3c7; border-radius: 4px; }
.campo-resultado { display: flex; flex-direction: column; gap: 5px; align-items: center; justify-content: flex-end; }
.campo-resultado label { font-size: 0.8em; font-weight: bold; color: #7f8c8d; }
.resultado-numero { background: #27ae60; color: white; font-weight: bold; padding: 8px 15px; border-radius: 4px; font-size: 1.1em; }
.btn-add-calc { width: 100%; padding: 10px; background: #2980b9; color: white; border: none; font-weight: bold; border-radius: 4px; cursor: pointer; margin-top: 5px; }
.btn-add-calc:hover { background: #1f618d; }

.tabla-receta-wrapper { border: 1px solid #dee2e6; border-radius: 4px; overflow: hidden; background: white; }
.tabla-receta { width: 100%; border-collapse: collapse; }
.tabla-receta th { background: #34495e; color: white; padding: 12px; text-align: left; font-size: 0.9em; }
.tabla-receta td { padding: 10px 12px; border-bottom: 1px solid #f1f1f1; }
.btn-x { background: none; border: none; color: #c0392b; font-weight: bold; cursor: pointer; font-size: 1.4em; }
.btn-x:hover { color: #e74c3c; transform: scale(1.1); }

/* FOOTER ACTIONS */
.footer-actions { 
    display: flex; 
    justify-content: space-between; 
    align-items: center;
    border-top: 1px solid #eee; 
    padding-top: 20px; 
}
.acciones-derecha { display: flex; gap: 15px; }

.btn-eliminar { padding: 12px 20px; border: 1px solid #e74c3c; background: #fff; color: #e74c3c; border-radius: 4px; cursor: pointer; font-weight: bold; transition: all 0.3s; }
.btn-eliminar:hover:not(:disabled) { background: #e74c3c; color: white; }
.btn-eliminar:disabled { opacity: 0.5; cursor: not-allowed; }

.btn-cancelar { padding: 12px 25px; border: 1px solid #ccc; background: white; border-radius: 4px; cursor: pointer; font-weight: bold; color: #666; }
.btn-guardar { padding: 12px 30px; border: none; background: #3498db; color: white; border-radius: 4px; cursor: pointer; font-weight: bold; font-size: 1em; transition: background 0.3s; }
.btn-guardar:disabled { background: #bdc3c7; cursor: not-allowed; }
.btn-guardar:hover:not(:disabled) { background: #2980b9; }

.text-center { text-align: center; }
.text-muted { color: #999; font-style: italic; padding: 20px; }
.font-bold { font-weight: bold; color: #2c3e50; }

@media (max-width: 768px) {
    .grid-3 { grid-template-columns: 1fr; }
    .footer-actions { flex-direction: column-reverse; gap: 15px; }
    .acciones-derecha { width: 100%; justify-content: space-between; }
    .btn-eliminar { width: 100%; }
}

@keyframes pulse { 0% { opacity: 1; } 50% { opacity: 0.8; } 100% { opacity: 1; } }
</style>