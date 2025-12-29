import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '@/views/LoginView.vue'
import MatchRecordsView from '../views/MatchRecordsView.vue'
import StatsEntryView from "../views/StatsEntryView.vue"
import Master from "../views/MasterManagementView.vue"

const router = createRouter({
history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      redirect: '/login',
      meta: { hideLayout: true }
    },
    {
      path: '/login',
      name: 'login',
      component: LoginView,
      meta: { hideLayout: true }
    },
    {
      path: '/main', // ログイン後のメイン画面として想定
      name: 'main',
      component: MatchRecordsView // 仮で試合記録画面を割り当て
    },
    {
      path:"/statsenrty",
      name:"statsenrty",
      component:StatsEntryView
    },
    {
      path:"/master",
      name:"master",
      component:Master
    }
  ]
})

export default router