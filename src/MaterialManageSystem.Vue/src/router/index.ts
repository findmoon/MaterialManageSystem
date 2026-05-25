import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/pages/LoginPage.vue')
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/pages/RegisterPage.vue')
    },
    {
      path: '/',
      name: 'home',
      component: () => import('@/pages/HomePage.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/warehouse',
      name: 'warehouse',
      component: () => import('@/pages/WarehousePage.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/partno',
      name: 'partno',
      component: () => import('@/pages/PartNoPage.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/reel',
      name: 'reel',
      component: () => import('@/pages/ReelPage.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/warning',
      name: 'warning',
      component: () => import('@/pages/WarningPage.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

router.beforeEach((to, _from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next('/login')
  } else if (to.path === '/login' && token) {
    next('/')
  } else {
    next()
  }
})

export default router
