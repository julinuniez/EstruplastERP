<script setup lang="ts">
import { ref, computed } from 'vue';
import dayjs from 'dayjs';
import 'dayjs/locale/es';
import weekOfYear from 'dayjs/plugin/weekOfYear';

dayjs.extend(weekOfYear);
dayjs.locale('es');

// --- CONSTANTES ---
const ALTURA_HORA = 60; // 60px = 1 hora
const HORA_INICIO = 0;
const HORA_FIN = 24;

const fechaBase = ref(dayjs().startOf('week').add(1, 'day')); // Lunes

const diasSemana = computed(() => Array.from({ length: 6 }, (_, i) => fechaBase.value.add(i, 'day')));
const horasDia = Array.from({ length: HORA_FIN - HORA_INICIO }, (_, i) => i + HORA_INICIO);

// --- DATOS ---
const tareasPlanificadas = ref([
    { 
        id: 1, opId: 504, cliente: 'Coca Cola', producto: 'Botellas 500ml',
        color: '#3498db', maquinaId: 1, 
        fecha: dayjs().startOf('week').add(1, 'day').format('YYYY-MM-DD'), 
        horaInicio: 9.0, duracion: 4.0 
    }
]);

const tareasPendientes = ref([
    { id: 2, opId: 504, cliente: 'Coca Cola', producto: 'Tapas Rojas', color: '#e74c3c', duracionEstimada: 3.0 },
    { id: 3, opId: 505, cliente: 'Villavicencio', producto: 'Etiquetas', color: '#27ae60', duracionEstimada: 5.0 }
]);

// --- UTILS ---
const formatoHora = (decimal: number) => {
    let horas = Math.floor(decimal);
    let minutos = Math.round((decimal - horas) * 60);
    if (minutos === 60) { minutos = 0; horas += 1; }
    return `${horas.toString().padStart(2, '0')}:${minutos.toString().padStart(2, '0')}`;
};
const calcularFin = (inicio: number, duracion: number) => formatoHora(inicio + duracion);

const getEstiloOrden = (tarea: any) => {
    const top = (tarea.horaInicio - HORA_INICIO) * ALTURA_HORA;
    const height = tarea.duracion * ALTURA_HORA;
    const left = tarea.maquinaId === 1 ? '2%' : '52%'; 
    return { top: `${top}px`, height: `${height}px`, left, width: '46%', backgroundColor: tarea.color };
};

// ==========================================
// 1. LOGICA MOVER (DRAG & DROP)
// ==========================================

const startDragPendiente = (evt: DragEvent, tarea: any) => {
    evt.dataTransfer!.effectAllowed = 'copyMove';
    evt.dataTransfer!.setData('origen', 'pendiente');
    evt.dataTransfer!.setData('tareaId', tarea.id.toString());
    (evt.target as HTMLElement).style.opacity = '0.5';
};

const startDragPlanificado = (evt: DragEvent, tarea: any) => {
    evt.dataTransfer!.effectAllowed = 'move';
    evt.dataTransfer!.setData('origen', 'planificado');
    evt.dataTransfer!.setData('tareaId', tarea.id.toString());
    evt.dataTransfer!.setData('offsetY', evt.offsetY.toString());
    (evt.target as HTMLElement).style.opacity = '0.5';
};

const endDrag = (evt: DragEvent) => { (evt.target as HTMLElement).style.opacity = '1'; };

const onDrop = (evt: DragEvent, fechaDestino: string) => {
    const origen = evt.dataTransfer!.getData('origen');
    const tareaId = parseInt(evt.dataTransfer!.getData('tareaId'));
    const rect = (evt.currentTarget as HTMLElement).getBoundingClientRect();

    // CALCULO HORA (Y)
    let clickOffsetY = 0;
    if (origen === 'planificado') {
        clickOffsetY = parseFloat(evt.dataTransfer!.getData('offsetY'));
    } else {
        clickOffsetY = 10; // Default si viene del buzón
    }
    const relativeY = evt.clientY - rect.top - clickOffsetY;
    let nuevaHora = (relativeY / ALTURA_HORA) + HORA_INICIO;
    nuevaHora = Math.round(nuevaHora * 4) / 4; // 15 min snap

    // CALCULO MAQUINA (X)
    const relativeX = evt.clientX - rect.left;
    const nuevaMaquinaId = (relativeX > rect.width / 2) ? 2 : 1;

   if (origen === 'pendiente') {
        // PENDIENTE -> CALENDARIO
        const index = tareasPendientes.value.findIndex(t => t.id === tareaId);
        if (index !== -1) {
            const t = tareasPendientes.value[index] as any; 
            
            if (t) {
                tareasPendientes.value.splice(index, 1);
                tareasPlanificadas.value.push({
                    ...t, 
                    fecha: fechaDestino, 
                    horaInicio: Math.max(0, nuevaHora), 
                    maquinaId: nuevaMaquinaId, 
                    duracion: t.duracionEstimada || 1 
                } as any); 
            }
        }
    } else {
        const t = tareasPlanificadas.value.find(x => x.id === tareaId);
        if (t) {
            t.fecha = fechaDestino;
            t.horaInicio = Math.max(0, nuevaHora);
            t.maquinaId = nuevaMaquinaId;
        }
    }
};

// ==========================================
// 2. LOGICA REDIMENSIONAR (ESTIRAR DURACIÓN)
// ==========================================
const resizingTarea = ref<any>(null);
const startResizeY = ref(0);
const startHeight = ref(0);

const startResize = (evt: MouseEvent, tarea: any) => {
    evt.stopPropagation(); // Evitar que empiece a arrastrarse
    evt.preventDefault();  // Evitar selección de texto
    
    resizingTarea.value = tarea;
    startResizeY.value = evt.clientY;
    startHeight.value = tarea.duracion * ALTURA_HORA;
    
    window.addEventListener('mousemove', onResizing);
    window.addEventListener('mouseup', stopResize);
};

const onResizing = (evt: MouseEvent) => {
    if (!resizingTarea.value) return;
    
    const deltaY = evt.clientY - startResizeY.value;
    let nuevaDuracion = (startHeight.value + deltaY) / ALTURA_HORA;
    
    // Mínimo 15 minutos (0.25)
    nuevaDuracion = Math.max(0.25, Math.round(nuevaDuracion * 4) / 4);
    
    resizingTarea.value.duracion = nuevaDuracion;
};

const stopResize = () => {
    // Aquí llamarías a la API para guardar la nueva duración
    console.log("Nueva duración guardada:", resizingTarea.value?.duracion);
    resizingTarea.value = null;
    window.removeEventListener('mousemove', onResizing);
    window.removeEventListener('mouseup', stopResize);
};

// --- NAVEGACIÓN ---
const cambiarSemana = (dir: number) => { fechaBase.value = fechaBase.value.add(dir, 'week'); };
const irAHoy = () => { fechaBase.value = dayjs().startOf('week').add(1, 'day'); };
</script>

<template>
  <div class="layout-wrapper">
    
    <div class="sidebar-pendientes">
        <h3>📋 Pendientes</h3>
        <p class="sub">Arrastra para asignar</p>
        
        <div class="lista-pendientes">
            <div 
                v-for="tarea in tareasPendientes" :key="tarea.id" 
                class="card-pendiente"
                :style="{ borderLeftColor: tarea.color }"
                draggable="true"
                @dragstart="startDragPendiente($event, tarea)"
                @dragend="endDrag"
            >
                <div class="card-header">
                    <span class="op-tag">#{{ tarea.opId }}</span>
                    <span class="duracion-tag">⏳{{ tarea.duracionEstimada }}h</span>
                </div>
                <div class="prod-nom">{{ tarea.producto }}</div>
            </div>
            <div v-if="tareasPendientes.length === 0" class="empty">✅ Todo listo</div>
        </div>
    </div>

    <div class="main-calendar">
        <div class="header-controls">
            <h2>📅 Planificación de Planta</h2>
            <div class="nav-controls">
                <button @click="cambiarSemana(-1)">◀</button>
                <span class="fecha-titulo">{{ fechaBase.format('MMMM YYYY') }}</span>
                <button @click="cambiarSemana(1)">▶</button>
                <button @click="irAHoy" class="btn-hoy">Hoy</button>
            </div>
        </div>

        <div class="calendar-frame">
            <div class="time-col">
                <div class="time-head">Hora</div>
                <div v-for="h in horasDia" :key="h" class="time-cell" :style="{height: ALTURA_HORA + 'px'}">
                    <span>{{ h.toString().padStart(2,'0') }}:00</span>
                </div>
            </div>

            <div class="days-grid">
                <div class="days-head">
                    <div v-for="dia in diasSemana" :key="dia.toString()" class="day-th" :class="{'hoy': dia.isSame(dayjs(), 'day')}">
                        <div class="day-lbl">{{ dia.format('ddd D') }}</div>
                        <div class="mq-lbl"><span>Chica</span><span>Grande</span></div>
                    </div>
                </div>

                <div class="days-body">
                    <div 
                        v-for="dia in diasSemana" :key="dia.toString()" 
                        class="day-col"
                        @dragover.prevent
                        @drop="onDrop($event, dia.format('YYYY-MM-DD'))"
                    >
                        <div class="mq-divider"></div>
                        <div v-for="h in horasDia" :key="h" class="grid-line" :style="{height: ALTURA_HORA + 'px'}"></div>

                        <div 
                            v-for="tarea in tareasPlanificadas.filter(t => t.fecha === dia.format('YYYY-MM-DD'))"
                            :key="tarea.id"
                            class="event-block"
                            :style="getEstiloOrden(tarea)"
                            draggable="true"
                            @dragstart="startDragPlanificado($event, tarea)"
                            @dragend="endDrag"
                        >
                            <div class="evt-title">#{{ tarea.opId }} - {{ tarea.cliente }}</div>
                            <div class="evt-prod">{{ tarea.producto }}</div>
                            <div class="evt-time">
                                ⏱️ {{ formatoHora(tarea.horaInicio) }} - {{ calcularFin(tarea.horaInicio, tarea.duracion) }}
                            </div>

                            <div class="resize-handle" @mousedown="startResize($event, tarea)">═</div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
  </div>
</template>

<style scoped>
.layout-wrapper { display: flex; height: 100%; font-family: 'Segoe UI', sans-serif; background: #f4f6f9; }

/* SIDEBAR */
.sidebar-pendientes { width: 220px; background: white; border-right: 1px solid #ddd; padding: 15px; display: flex; flex-direction: column; }
.sub { color: #999; font-size: 0.8rem; margin-bottom: 10px; }
.card-pendiente { background: #fff; border: 1px solid #eee; border-left: 4px solid #ccc; padding: 8px; margin-bottom: 8px; border-radius: 4px; cursor: grab; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
.card-pendiente:active { cursor: grabbing; }
.card-header { display: flex; justify-content: space-between; font-size: 0.75rem; font-weight: bold; margin-bottom: 2px; }
.prod-nom { font-size: 0.85rem; color: #555; }
.empty { text-align: center; color: green; margin-top: 20px; font-weight: bold; }

/* CALENDAR */
.main-calendar { flex: 1; display: flex; flex-direction: column; padding: 15px; overflow: hidden; }
.header-controls { display: flex; justify-content: space-between; align-items: center; margin-bottom: 10px; }
.calendar-frame { flex: 1; display: flex; border: 1px solid #ccc; background: white; border-radius: 6px; overflow-y: auto; }

/* COLUMNA HORAS */
.time-col { width: 50px; background: #fafafa; border-right: 1px solid #eee; }
.time-head { height: 50px; border-bottom: 1px solid #eee; display: flex; align-items: center; justify-content: center; font-size: 0.7rem; color: #aaa; }
.time-cell { border-bottom: 1px solid transparent; position: relative; }
.time-cell span { position: absolute; top: -7px; right: 5px; font-size: 0.7rem; color: #999; }

/* GRILLA DÍAS */
.days-grid { flex: 1; display: flex; flex-direction: column; overflow-x: auto; }
.days-head { display: flex; height: 50px; border-bottom: 2px solid #ddd; }
.day-th { flex: 1; min-width: 140px; border-right: 1px solid #eee; display: flex; flex-direction: column; }
.day-lbl { flex: 1; display: flex; align-items: center; justify-content: center; font-weight: bold; color: #555; }
.hoy .day-lbl { color: #3498db; }
.mq-lbl { display: flex; height: 16px; font-size: 0.6rem; text-transform: uppercase; color: #aaa; font-weight: bold; border-top: 1px solid #eee; }
.mq-lbl span { flex: 1; text-align: center; background: #f9f9f9; }
.mq-lbl span:last-child { border-left: 1px solid #eee; }

.days-body { display: flex; flex: 1; }
.day-col { flex: 1; min-width: 140px; border-right: 1px solid #eee; position: relative; }
.mq-divider { position: absolute; left: 50%; top: 0; bottom: 0; border-left: 1px dashed #eee; }
.grid-line { border-bottom: 1px solid #f2f2f2; }

/* EVENTO */
.event-block { position: absolute; border-radius: 3px; color: white; padding: 4px; font-size: 0.75rem; box-shadow: 0 2px 4px rgba(0,0,0,0.15); cursor: grab; overflow: hidden; display: flex; flex-direction: column; z-index: 10; border: 1px solid rgba(0,0,0,0.1); }
.event-block:hover { z-index: 20; box-shadow: 0 4px 8px rgba(0,0,0,0.25); }
.evt-title { font-weight: bold; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
.evt-prod { font-size: 0.7rem; opacity: 0.9; margin-bottom: auto; }
.evt-time { font-size: 0.7rem; font-family: monospace; }

/* MANIJA RESIZE */
.resize-handle { height: 10px; background: rgba(0,0,0,0.1); cursor: s-resize; display: flex; align-items: center; justify-content: center; font-size: 8px; color: rgba(255,255,255,0.6); margin-top: 2px; }
.resize-handle:hover { background: rgba(0,0,0,0.3); color: white; }

.nav-controls button { cursor: pointer; padding: 4px 8px; }
.fecha-titulo { margin: 0 10px; font-weight: bold; text-transform: capitalize; }
</style>