import Vue from 'vue'
import Router from 'vue-router'
import layout from '@/components/layout'
import Login from '@/Page/User/Login'
Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'layout',
      component: layout
    },
    {
      path: '/login',
      name: 'login',
      component: Login
    }
  ]
})
