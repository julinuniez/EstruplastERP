<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import api from '@/services/axiosInstance';

const clientes = ref<any[]>([]);
const materialesBase = ref<any[]>([]);
const todosLosProductos = ref<any[]>([]); 
const loading = ref(false);
const mensaje = ref('');
const advertencia = ref(''); 

// Colores/Variantes estándar para el Datalist
const variantesEstandar = [
    'BLANCO', 'NEGRO', 'NATURAL', 'AZUL', 'ROJO', 
    'VERDE', 'AMARILLO', 'GRIS', 'NARANJA', 'MULTICOLOR',
    'SILLAS', 'BALDES', 'PARAGOLPES', 'CAJONES'
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

        // ✅ LISTA EXACTA Y DURA (Los 7 Vengadores)
        const nombresExactos = [
            "PAI", "PEAD", "POLIPROPILENO", "BIOPLASTICO", "ABS", "RESISTENTE AL FREON", "POLIETILENO"
        ];

        // Filtramos para que traiga SOLAMENTE esos 7 nombres literales
        materialesBase.value = resProd.data.filter((p: any) => 
            p.nombre && nombresExactos.includes(p.nombre.toUpperCase().trim())
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
            const esDeCliente = form.value.clienteId 
                ? p.clienteId === Number(form.value.clienteId)
                : p.clienteId === null;
            const nombre = p.nombre.toUpperCase();
            const esScrapSucio = p.esScrap === true && 
                                 !nombre.includes("MOLIDO") && 
                                 !nombre.includes("PELLET") &&
                                 !nombre.includes("RECUPERADO");
            return esDeCliente && esScrapSucio && nombre.includes(nombreBase);
        })
        .map(p => {
            let variedad = p.nombre.toUpperCase()
                .replace('[SCRAP]', '')
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
        return alert("⚠️ Faltan datos: Material o Kilos.");
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

        const res = await api.post('/Movimientos/ingresar-scrap-sucio', payload);
        
        mensaje.value = `✅ ÉXITO: Ingresados ${form.value.kilos}kg a "${res.data.producto}"`;
        
        form.value.kilos = 0; 
        form.value.productoExistenteId = null; 
        form.value.variedad = ''; 

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
    <div class="contenedor-scrap">
        <div class="card">
            <div class="header">
                <h2>🗑️ Ingreso de Scrap (Sucio)</h2>
                <p>Ingrese residuos para lavar/moler.</p>
            </div>

            <label>Origen (Dueño):</label>
            <select v-model="form.clienteId">
                <option v-for="c in clientes" :key="c.id" :value="c.id">{{ c.razonSocial }}</option>
            </select>

            <label>Familia Base:</label>
            <select v-model="form.materialBaseId">
                <option value="" disabled>-- Seleccione Familia --</option>
                <option v-for="m in materialesBase" :key="m.id" :value="m.id">{{ m.nombre }}</option>
            </select>

            <div class="seccion-variedad">
                <label>Variedad / Detalle:</label>
                
                <div v-if="variantesExistentes.length > 0" class="sugerencias">
                    <small>Variantes existentes (Clic para sumar stock):</small>
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

                <small v-if="advertencia" class="texto-advertencia">{{ advertencia }}</small>
            </div>

            <div class="preview" v-if="form.materialBaseId">
                <div v-if="form.productoExistenteId">
                    ✅ <strong>SUMANDO STOCK A:</strong><br> 
                    {{ variantesExistentes.find(v => v.id === form.productoExistenteId)?.variedad || 'Selección Existente' }}
                </div>
                <div v-else>
                    🏷️ <strong>NUEVA ETIQUETA:</strong><br>
                    [SCRAP] {{ materialesBase.find(m => m.id == Number(form.materialBaseId))?.nombre }} {{ form.variedad.toUpperCase() }}
                </div>
            </div>

            <label>Peso (Kg):</label>
            <input type="number" v-model="form.kilos" class="input-kilos" min="0">

            <button @click="guardar" :disabled="loading" class="btn-guardar">
                {{ loading ? '⏳ Guardando...' : '📥 INGRESAR SCRAP' }}
            </button>

            <div v-if="mensaje" class="alerta">{{ mensaje }}</div>
        </div>
    </div>
</template>

<style scoped>
.contenedor-scrap { display: flex; justify-content: center; padding: 40px; background: #f4f6f9; min-height: 90vh; }
.card { background: white; padding: 30px; border-radius: 12px; width: 500px; box-shadow: 0 4px 20px rgba(0,0,0,0.08); height: fit-content; }
.header { border-bottom: 2px solid #e67e22; margin-bottom: 20px; padding-bottom: 10px; text-align: center; }
.header h2 { margin: 0; color: #d35400; font-size: 1.5rem; }
.header p { margin: 5px 0 0; color: #7f8c8d; font-size: 0.9rem; }
label { display: block; font-weight: 700; margin-top: 15px; margin-bottom: 5px; color: #34495e; }
select, input { width: 100%; padding: 12px; border: 1px solid #dcdcdc; border-radius: 6px; font-size: 1rem; }
.input-variedad { width: 100%; box-sizing: border-box; } 
.seccion-variedad { background: #fff8f3; padding: 15px; border-radius: 8px; border: 1px dashed #f0ceb6; margin-top: 15px; }
.chips-container { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 10px; }
.chip { background: white; border: 1px solid #e67e22; color: #e67e22; padding: 5px 10px; border-radius: 15px; cursor: pointer; font-size: 0.8rem; }
.chip:hover { background: #fff3e0; }
.chip.activo { background: #e67e22; color: white; border-width: 2px; font-weight: bold; }
.texto-advertencia { color: #d9534f; font-weight: bold; display: block; margin-top: 5px; font-size: 0.85rem; }
.preview { margin-top: 15px; color: #7f8c8d; font-size: 0.9rem; background: #fdf2e9; padding: 10px; border-radius: 6px; text-align: center; border-left: 4px solid #e67e22; }
.input-kilos { font-size: 1.4rem; font-weight: bold; color: #2c3e50; text-align: center; }
.btn-guardar { margin-top: 25px; width: 100%; padding: 15px; background: #2c3e50; color: white; border: none; font-weight: bold; border-radius: 6px; cursor: pointer; font-size: 1.1rem; transition: 0.3s; }
.btn-guardar:hover { background: #34495e; }
.alerta { margin-top: 20px; padding: 15px; background: #d4edda; color: #155724; border-radius: 6px; text-align: center; font-weight: bold; border: 1px solid #c3e6cb; }
</style>