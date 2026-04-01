import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vitest/config';
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import packageJson from './package.json'

// https://vite.dev/config/
export default defineConfig({
  base: './',
  plugins: [
    vue(),
    vueDevTools(),
  ],
  test: {
    environment: 'jsdom', // Simula el navegador
    globals: true
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  define: {
    '__APP_VERSION__': JSON.stringify(packageJson.version)
  }
})