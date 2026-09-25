<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import api from '@/services/axiosInstance';
import { Alertas } from '@/utils/alertas';

const clientes = ref<any[]>([]);
const materialesBase = ref<any[]>([]);
const todosLosProductos = ref<any[]>([]); 
const loading = ref(false);
const mensaje = ref('');
const advertencia = ref(''); 

const form = ref({
    clienteId: '',
    materialBaseId: '',
    variedad: '',
    nuevaVariedadPersonalizada: '',
    kilos: 0,
    productoExistenteId: null as number | null 
});

watch(() => form.value.variedad, () => {
    form.value.productoExistenteId = null;
    if (form.value.variedad !== 'NUEVO') {
        form.value.nuevaVariedadPersonalizada = '';
    }
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

        materialesBase.value = resProd.data.filter((p: any) => 
            p.nombre && nombresExactos.includes(p.nombre.toUpperCase().trim()) && (p.esMateriaPrima || p.EsMateriaPrima)
        );
        
    } catch (e) { console.error(e); }
});

const limpiarNombreVariedad = (nombreOriginal: string, nombreBaseAQuitar: string = '') => {
    let limpio = (nombreOriginal || '').toUpperCase().trim();

    limpio = limpio.replace('[MOLIDO]', '').replace('MOLIDO', '').trim();

    const prefijosMB = ['MASTERBATCH ', 'MASTER ', 'MB ', 'MB. ', 'COLOR '];
    for (const prefijo of prefijosMB) {
        if (limpio.startsWith(prefijo)) {
            limpio = limpio.substring(prefijo.length).trim();
            break; 
        }
    }

    if (nombreBaseAQuitar) {
        limpio = limpio.replace(nombreBaseAQuitar, '').trim();
    }

    limpio = limpio.replace(/\(\s*(PAI\vert{}PEAD\vert{}POLIPROPILENO\vert{}ABS\vert{}FREON\vert{}POLIETILENO\vert{}PP\vert{}KG\vert{}KGS\vert{}KG\.\vert{}KGS\.)\s*\)/g, '');

    limpio = limpio
        .replace(/\bKGS?\.?\b/g, '')
        .replace(/\(\s*\)/g, '')
        .replace(/[-]/g, ' ')
        .replace(/^\s*-\s*/, '')
        .replace(/\s+/g, ' ')
        .trim();

    return limpio;
};

const variantesExistentes = computed(() => {
    if (!form.value.materialBaseId) return [];
    const materialPadre = materialesBase.value.find(m => m.id === Number(form.value.materialBaseId));
    if (!materialPadre) return [];
    
    const nombreBase = materialPadre.nombre.toUpperCase().trim();

    return todosLosProductos.value
        .filter(p => {
            const esDeCliente = form.value.clienteId 
                ? p.clienteId === Number(form.value.clienteId)
                : (!p.clienteId || p.clienteId === 0 || p.clienteId === 1);
            
            const nombre = p.nombre.toUpperCase();
            const catId = Number(p.categoriaInsumoId || p.CategoriaInsumoId || 0);
            
            const esMolido = (p.esMateriaPrima === true || p.EsMateriaPrima === true) && 
                             (catId === 4 || nombre.includes("MOLIDO") || nombre.includes("SCRAP"));
                             
            return esDeCliente && esMolido && nombre.includes(nombreBase);
        })
        .map(p => {
            let variedad = limpiarNombreVariedad(p.nombre, nombreBase);
            
            return {
                id: p.id,
                variedad: variedad || 'GENÉRICO',
                stock: p.stockActual ?? 0
            };
        })
        .filter(v => v.variedad.length > 0)
        .sort((a, b) => a.variedad.localeCompare(b.variedad)); // 🚀 ORDEN ALFABÉTICO (Antes era por stock)
});

const nombresVariedadesSugeridas = computed(() => {
    const nombresUnicos = new Set<string>();
    
    variantesExistentes.value.forEach(v => nombresUnicos.add(v.variedad));

    todosLosProductos.value.forEach(p => {
        const catId = Number(p.categoriaInsumoId || p.CategoriaInsumoId || 0);
        
        if (catId === 2) {
            const esGlobal = !p.clienteId || p.clienteId === 0 || p.clienteId === 1;
            const esDelCliente = form.value.clienteId ? (p.clienteId === Number(form.value.clienteId)) : false;

            if (esGlobal || esDelCliente) {
                let nombreLimpio = limpiarNombreVariedad(p.nombre);
                    
                if (nombreLimpio && nombreLimpio.length >= 2) {
                    nombresUnicos.add(nombreLimpio);
                }
            }
        }
    });

    if (nombresUnicos.size === 0) {
        nombresUnicos.add('BLANCO');
        nombresUnicos.add('NEGRO');
        nombresUnicos.add('NATURAL');
        nombresUnicos.add('GENÉRICO');
    }

    // 🚀 ORDEN ALFABÉTICO PERFECTO PARA EL DESPLEGABLE
    return Array.from(nombresUnicos).sort((a, b) => a.localeCompare(b));
});

const usarVariante = (variedad: string, id: number) => {
    form.value.variedad = variedad;
    setTimeout(() => {
        form.value.productoExistenteId = id; 
    }, 50); 
};

const matchIdExistente = computed(() => {
    const match = variantesExistentes.value.find(v => v.variedad === form.value.variedad);
    return match ? match.id : null;
});

const guardar = async () => {
    if (!form.value.materialBaseId || form.value.kilos <= 0) {
        return Alertas.advertencia("⚠️ Faltan datos: Seleccione la Familia y cargue los Kilos.");
    }
    
    const variedadFinal = form.value.variedad === 'NUEVO' ? form.value.nuevaVariedadPersonalizada.trim() : form.value.variedad;

    if (!variedadFinal) {
        return Alertas.advertencia("⚠️ Debe seleccionar o escribir un color/variedad para el molido.");
    }

    loading.value = true;
    mensaje.value = '';

    try {
        const payload = {
            ClienteId: form.value.clienteId ? Number(form.value.clienteId) : null,
            MaterialBaseId: Number(form.value.materialBaseId),
            Variedad: variedadFinal === 'GENÉRICO' ? '' : variedadFinal,
            Kilos: Number(form.value.kilos),
            ProductoExistenteId: matchIdExistente.value || form.value.productoExistenteId 
        };

        const res = await api.post('/Movimientos/ingresar-molido', payload);
        
        mensaje.value = `✅ ÉXITO: Ingresados ${form.value.kilos}kg a "${res.data.producto}"`;
        
        form.value.kilos = 0; 
        form.value.productoExistenteId = null; 
        form.value.variedad = ''; 
        form.value.nuevaVariedadPersonalizada = '';

        const resProd = await api.get('/Productos');
        todosLosProductos.value = resProd.data;

    } catch (e: any) {
        Alertas.error("❌ Error: " + (e.response?.data?.mensaje || e.message));
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

        <div class="seccion-variedad" v-if="form.materialBaseId">
            <label>3️⃣ Variedad / Color del Molido:</label>
            
            <select v-model="form.variedad" class="select-variedad">
                <option value="" disabled>Seleccione Color...</option>
                <option v-for="v in nombresVariedadesSugeridas" :key="v" :value="v">
                    {{ v }}
                </option>
                <option disabled>────────────────────</option>
                <option value="NUEVO">✨ + Crear Nuevo Color / Variedad...</option>
            </select>

            <div v-if="form.variedad === 'NUEVO'" style="margin-top: 10px;">
                <input 
                    type="text" 
                    v-model="form.nuevaVariedadPersonalizada" 
                    placeholder="Escriba el nombre del nuevo color (Ej: ROJO FUEGO)"
                    class="input-variedad"
                    style="border-color: #3498db; background-color: #ebf5fb;"
                >
            </div>

            <div v-if="variantesExistentes.length > 0 && form.variedad !== 'NUEVO'" class="sugerencias">
                <small style="display:block; margin-top: 10px; margin-bottom: 5px; color: #7f8c8d;">
                    Lotes de molido de este cliente que ya existen en el sistema:
                </small>
                <div class="chips-container">
                    <button 
                        v-for="v in variantesExistentes" 
                        :key="v.id"
                        @click="usarVariante(v.variedad, v.id)" 
                        class="chip"
                        :class="{ 'activo': form.variedad === v.variedad }"
                        type="button"
                    >
                        {{ v.variedad }}
                    </button>
                </div>
            </div>
        </div>

        <div class="preview" v-if="form.materialBaseId && form.variedad">
            <div v-if="matchIdExistente">
                ✅ <strong>SUMANDO STOCK A:</strong><br> 
                {{ variantesExistentes.find(v => v.id === matchIdExistente)?.variedad || 'Selección Existente' }}
            </div>
            <div v-else>
                🏷️ <strong>NUEVO MATERIAL MOLIDO A CREAR:</strong><br>
                [MOLIDO] {{ form.variedad === 'NUEVO' ? form.nuevaVariedadPersonalizada.toUpperCase() : form.variedad.toUpperCase() }} ({{ materialesBase.find(m => m.id == Number(form.materialBaseId))?.nombre }})
            </div>
        </div>

        <label>4️⃣ Peso (Kg):</label>
        <input type="number" v-model="form.kilos" class="input-kilos" min="0">

        <button @click="guardar" :disabled="loading || (!form.variedad) || (form.variedad === 'NUEVO' && !form.nuevaVariedadPersonalizada)" class="btn-guardar">
            {{ loading ? '⏳ Guardando...' : '📥 INGRESAR MOLIENDA' }}
        </button>

        <div v-if="mensaje" class="alerta">{{ mensaje }}</div>
    </div>
</template>

<style scoped>
.contenedor-scrap-plano { display: flex; flex-direction: column; width: 100%; max-width: 600px; margin: 0 auto; }
label { display: block; font-weight: 700; margin-top: 15px; margin-bottom: 5px; color: #34495e; }
select, input { width: 100%; padding: 12px; border: 1px solid #dcdcdc; border-radius: 6px; font-size: 1rem; box-sizing: border-box; }
.input-variedad { width: 100%; box-sizing: border-box; font-weight: bold; } 
.select-variedad { font-weight: bold; color: #2c3e50; cursor: pointer; }
.seccion-variedad { background: #f4f6f7; padding: 15px; border-radius: 8px; border: 1px dashed #bdc3c7; margin-top: 15px; }
.chips-container { display: flex; flex-wrap: wrap; gap: 8px; margin-bottom: 5px; }
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