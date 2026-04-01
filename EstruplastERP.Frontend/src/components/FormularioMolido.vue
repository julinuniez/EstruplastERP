<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import api from '@/services/axiosInstance';

const clientes = ref<any[]>([]);
const materialesBase = ref<any[]>([]);
const todosLosProductos = ref<any[]>([]); 
const loading = ref(false);
const mensaje = ref('');
const advertencia = ref(''); 

const variantesEstandar = [
    'BLANCO', 'NEGRO', 'NATURAL', 'GRAL'
];

const form = ref({
    clienteId: '',
    materialBaseId: '',
    variedad: '',
    kilos: 0,
    productoExistenteId: null as number | null 
});

watch(() => form.value.variedad, () => {
    form.value.productoExistenteId = null; 
});

onMounted(async () => {
    try {
        const [resCli, resProd] = await Promise.all([
            api.get('/Clientes'),
            api.get('/Productos')
        ]);
        clientes.value = resCli.data;
        todosLosProductos.value = resProd.data; 

        const nombresExactos = [
            "PAI", "PEAD", "POLIPROPILENO", "ABS", "RESISTENTE AL FREON", "POLIETILENO"
        ];

        // Filtramos para que solo traiga las familias base que creamos por SQL
        materialesBase.value = resProd.data.filter((p: any) => 
            p.nombre && nombresExactos.includes(p.nombre.toUpperCase().trim()) && p.rubro === 'FAMILIA BASE'
        );
        
    } catch (e) { console.error(e); }
});

const variantesExistentes = computed(() => {
    if (!form.value.materialBaseId) return [];
    const materialPadre = materialesBase.value.find(m => m.id === Number(form.value.materialBaseId));
    if (!materialPadre) return [];
    
    const nombreBase = materialPadre.nombre.toUpperCase().trim();

    return todosLosProductos.value
        .filter(p => {
            // Filtra por el cliente seleccionado (o stock propio si no elige cliente)
            const esDeCliente = form.value.clienteId 
                ? p.clienteId === Number(form.value.clienteId)
                : p.clienteId === null;
            
            const nombre = p.nombre.toUpperCase();
            
            // 🚨 NUEVA LÓGICA: Busca solo Materia Prima que sea molienda
            const esMolido = p.esMateriaPrima === true && 
                             (nombre.includes("MOLIDO") || p.rubro === 'MOLIDO' || p.rubro === 'MOLIDO CLIENTE');
                             
            return esDeCliente && esMolido && nombre.includes(nombreBase);
        })
        .map(p => {
            // Limpia el nombre para mostrar solo la "Variedad" en los botoncitos
            let variedad = p.nombre.toUpperCase()
                .replace('[MOLIDO]', '')
                .replace('MOLIDO', '')
                .replace(nombreBase, '')
                .replace(/^\s*-\s*/, '')
                .trim();
            
            return {
                id: p.id,
                variedad: variedad || '(GENÉRICO)',
                stock: p.stockActual
            };
        })
        .filter(v => v.variedad.length > 0)
        .sort((a, b) => b.stock - a.stock);
});

const usarVariante = (variedad: string, id: number) => {
    form.value.variedad = variedad === '(GENÉRICO)' ? '' : variedad;
    setTimeout(() => {
        form.value.productoExistenteId = id; 
    }, 50); 
};

const guardar = async () => {
    if (!form.value.materialBaseId || form.value.kilos <= 0) {
        return alert("⚠️ Faltan datos: Seleccione la Familia y cargue los Kilos.");
    }

    loading.value = true;
    mensaje.value = '';

    try {
        const payload = {
            ClienteId: form.value.clienteId ? Number(form.value.clienteId) : null,
            MaterialBaseId: Number(form.value.materialBaseId),
            Variedad: form.value.variedad,
            Kilos: Number(form.value.kilos),
            ProductoExistenteId: form.value.productoExistenteId 
        };

        // 🚨 Le pega a la ruta que configuramos en el backend
        const res = await api.post('/Movimientos/ingresar-molido', payload);
        
        mensaje.value = `✅ ÉXITO: Ingresados ${form.value.kilos}kg a "${res.data.producto}"`;
        
        // Resetea el formulario para la próxima carga
        form.value.kilos = 0; 
        form.value.productoExistenteId = null; 
        form.value.variedad = ''; 

        // Refresca el stock para que las sugerencias se actualicen
        const resProd = await api.get('/Productos');
        todosLosProductos.value = resProd.data;

    } catch (e: any) {
        alert("❌ Error: " + (e.response?.data?.mensaje || e.message));
    } finally {
        loading.value = false;
    }
};
</script>

<template>
    <div class="contenedor-scrap-plano">
        <label>1️⃣ Origen (Dueño del Material):</label>
        <select v-model="form.clienteId">
            <option value="">Estruplast</option>
            <option v-for="c in clientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
        </select>

        <label>2️⃣ Familia Base:</label>
        <select v-model="form.materialBaseId">
            <option value="" disabled>Seleccione Familia</option>
            <option v-for="m in materialesBase" :key="m.id" :value="m.id">{{ m.nombre }}</option>
        </select>

        <div class="seccion-variedad">
            <label>3️⃣ Variedad / Detalle:</label>
            
            <div v-if="variantesExistentes.length > 0" class="sugerencias">
                <small>Variantes existentes (Clic para sumar stock al mismo lote):</small>
                <div class="chips-container">
                    <button 
                        v-for="v in variantesExistentes" 
                        :key="v.id"
                        @click="usarVariante(v.variedad, v.id)" 
                        class="chip"
                        :class="{ 'activo': form.productoExistenteId === v.id }"
                        type="button"
                    >
                        {{ v.variedad }} 
                    </button>
                </div>
            </div>

            <input 
                type="text" 
                v-model="form.variedad" 
                placeholder="Ej: Rojo, Sillas, Baldes..."
                class="input-variedad"
                list="lista-sugerencias"
            >
            <datalist id="lista-sugerencias">
                <option v-for="v in variantesEstandar" :key="v" :value="v"></option>
            </datalist>
        </div>

        <div class="preview" v-if="form.materialBaseId">
            <div v-if="form.productoExistenteId">
                ✅ <strong>SUMANDO STOCK A:</strong><br> 
                {{ variantesExistentes.find(v => v.id === form.productoExistenteId)?.variedad || 'Selección Existente' }}
            </div>
            <div v-else>
                🏷️ <strong>NUEVO MATERIAL MOLIDO A CREAR:</strong><br>
                [MOLIDO] {{ form.variedad ? form.variedad.toUpperCase() : 'GRAL' }} ({{ materialesBase.find(m => m.id == Number(form.materialBaseId))?.nombre }})
            </div>
        </div>

        <label>4️⃣ Peso (Kg):</label>
        <input type="number" v-model="form.kilos" class="input-kilos" min="0">

        <button @click="guardar" :disabled="loading" class="btn-guardar">
            {{ loading ? '⏳ Guardando...' : '📥 INGRESAR MOLIENDA' }}
        </button>

        <div v-if="mensaje" class="alerta">{{ mensaje }}</div>
    </div>
</template>

<style scoped>
.contenedor-scrap-plano { display: flex; flex-direction: column; width: 100%; max-width: 600px; margin: 0 auto; }
label { display: block; font-weight: 700; margin-top: 15px; margin-bottom: 5px; color: #34495e; }
select, input { width: 100%; padding: 12px; border: 1px solid #dcdcdc; border-radius: 6px; font-size: 1rem; box-sizing: border-box; }
.input-variedad { width: 100%; box-sizing: border-box; } 
.seccion-variedad { background: #f4f6f7; padding: 15px; border-radius: 8px; border: 1px dashed #bdc3c7; margin-top: 15px; }
.chips-container { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 10px; }
.chip { background: white; border: 1px solid #2980b9; color: #2980b9; padding: 5px 10px; border-radius: 15px; cursor: pointer; font-size: 0.8rem; }
.chip:hover { background: #ebf5fb; }
.chip.activo { background: #2980b9; color: white; border-width: 2px; font-weight: bold; }
.preview { margin-top: 15px; color: #2c3e50; font-size: 0.9rem; background: #e8f8f5; padding: 10px; border-radius: 6px; text-align: center; border-left: 4px solid #1abc9c; }
.input-kilos { font-size: 1.4rem; font-weight: bold; color: #2c3e50; text-align: center; border: 2px solid #27ae60; }
.btn-guardar { margin-top: 25px; width: 100%; padding: 15px; background: #27ae60; color: white; border: none; font-weight: bold; border-radius: 6px; cursor: pointer; font-size: 1.1rem; transition: 0.3s; }
.btn-guardar:hover { background: #2ecc71; }
.btn-guardar:disabled { background: #95a5a6; cursor: not-allowed; }
.alerta { margin-top: 20px; padding: 15px; background: #d4edda; color: #155724; border-radius: 6px; text-align: center; font-weight: bold; border: 1px solid #c3e6cb; }
</style>