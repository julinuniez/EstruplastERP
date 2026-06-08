import { fileURLToPath, URL } from 'node:url'
<<<<<<< HEAD
import { defineConfig } from 'vitest/config';
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import packageJson from './package.json'

// https://vite.dev/config/
export default defineConfig({
  base: './',
=======

import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
>>>>>>> master
  plugins: [
    vue(),
    vueDevTools(),
  ],
<<<<<<< HEAD
  test: {
    environment: 'jsdom', // Simula el navegador
    globals: true
=======
  server: {
    proxy: {
      '/api': {
        target: 'https://localhost:7244', 
        changeOrigin: true,
        secure: false,
      }
    }
>>>>>>> master
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
<<<<<<< HEAD
  define: {
    '__APP_VERSION__': JSON.stringify(packageJson.version)
  },
  // 🚀 ACÁ ESTÁ LA SOLUCIÓN: El tubo directo hacia C#
  server: {
    proxy: {
      '/api': {
        // Fijate bien que coincida si tu Swagger dice http o https
        target: 'https://localhost:7244', 
        changeOrigin: true,
        secure: false, // 👈 Esto es CLAVE para que .NET no te rechace la conexión local
      }
    }
  }
})
=======
})
>>>>>>> master
