declare module '*.vue' {
  import type { DefineComponent } from 'vue';
  // Define un tipo para el componente Vue (asegura compatibilidad con TS)
  const component: DefineComponent<{}, {}, any>; 
  export default component;
}