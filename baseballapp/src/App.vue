<script lang="ts">
import { defineComponent, computed } from 'vue'
import { useRoute } from 'vue-router'
import MainLayout from './components/MainLayout.vue'
import NotificationManager from './components/NotificationManager.vue'

export default defineComponent({
  name: 'App',
  components: {
    MainLayout,
    NotificationManager
  },
  setup() {
    const route = useRoute()

// メタ情報に hideLayout が設定されている場合は false を返す
    const isLayoutVisible = computed(() => {
      return !route.meta.hideLayout
    })

    return { isLayoutVisible }
  }
})
</script>

<template>
  <v-app id="inspire">
    <MainLayout v-if="isLayoutVisible" />
    
    <v-main>
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>
    <NotificationManager />
  </v-app>
</template>

<style>
html, body {
  margin: 0;
  padding: 0;
  /* overflow: hidden; はサイドバーやコンテンツが長い場合に
     スクロールできなくなる可能性があるため、必要に応じて調整してください */
}

.v-application {
  font-family: 'Roboto', sans-serif;
}
</style>