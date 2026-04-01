import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { createPinia } from 'pinia'

// 👇 1. IMPORTA DAYJS Y LOS PLUGINS NECESARIOS
import dayjs from "dayjs";
import isSameOrBefore from "dayjs/plugin/isSameOrBefore";
import isSameOrAfter from "dayjs/plugin/isSameOrAfter";
import isBetween from "dayjs/plugin/isBetween"; // Útil para gantt
import customParseFormat from "dayjs/plugin/customParseFormat"; // Para evitar errores de formato

// 👇 2. ACTIVA LOS PLUGINS (EXTEND)
dayjs.extend(isSameOrBefore);
dayjs.extend(isSameOrAfter);
dayjs.extend(isBetween);
dayjs.extend(customParseFormat);

const app = createApp(App)

app.use(createPinia())
app.use(router)

// Opcional: Hacer dayjs global (para usar $dayjs en templates)
app.config.globalProperties.$dayjs = dayjs;
app.mount('#app')