<script setup lang="ts">
import FormularioMolido from '@/components/FormularioMolido.vue';
</script>

<template>
  <div class="contenedor-scrap">
    <div class="header-seccion">
        <h2>♻️ Gestión de Molienda</h2>
        <small style="color: #7f8c8d;">Ingreso directo de material molido listo</small>
    </div>

    <div class="contenido-tab">
        <div class="panel-animado">
            <FormularioMolido />
        </div>
    </div>
  </div>
</template>

<style scoped>
.contenedor-scrap { 
    max-width: 1200px; 
    margin: 0 auto; 
    background: white; 
    padding: 25px; 
    border-radius: 8px; 
    box-shadow: 0 4px 10px rgba(0,0,0,0.05); 
}
.header-seccion { 
    margin-bottom: 20px; 
    border-bottom: 2px solid #eee; 
    padding-bottom: 15px; 
}
.header-seccion h2 { 
    margin: 0; 
    color: #27ae60; 
}
.panel-animado { 
    animation: fadeIn 0.3s ease-in-out; 
}
@keyframes fadeIn { 
    from { opacity: 0; transform: translateY(5px); } 
    to { opacity: 1; transform: translateY(0); } 
}
</style>