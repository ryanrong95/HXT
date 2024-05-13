import Vue from 'vue'
import Router from 'vue-router'
import Layout from '@/components/layout'
import Storage from '@/Pages/storage/home'
import Area from '@/Pages/storage/area'
import Shelves from '@/Pages/storage/shelves'
import Location from '@/Pages/storage/location'
import kufang from '@/Pages/storage'
import Allocation from '@/Pages/allocation'
import Home from '@/Pages/Home'

Vue.use(Router)

let constantRouterMap =[
    {
      path: '/',
      name: 'layout',
      component: Layout,
      meta: ['admin', 'user',"qxqx"],
      children:[
        {
          path:"/",
          name:"home",
          component:Home,
          meta: ['admin', 'user',"qxqx"]
        },
        {
          path:"/Storage",
          name:"Storage",
          component:Storage,
          meta: ['admin', 'user']
        },
        {
          path:"/kufang",
          name:"kufang",
          component:kufang,
          meta: ['admin', 'user']
        },
        {
          path:"/area",
          name:"area",
          component:Area,
          meta: ['admin', 'user']
        },
        {
          path:"/shelves",
          name:"shelves",
          component:Shelves,
          meta: ['admin', 'user']
        },
        {
          path:"/Location",
          name:"Location",
          component:Location,
          meta: ['admin', 'user']
        },
        {
          path:"/Allocation",
          component:{template:`<router-view></router-view>`},
          meta: ['admin', 'user',"qxqx"],
          children:[
           {
            path:"/Allocation",
            name:"Allocation",
            component:Allocation,
            meta: ['admin', 'user']
           },
           {
            path:"/print",
            name:"print",
            component:()=>import("@/Pages/allocation/print"),
            meta: ['admin', 'user']
           }
          ]
        }
      ]
    },
    {
      path: '/login',
      name: 'login',
      component:()=>import("@/Pages/User"),
      meta: ['admin', 'user']
    }
  ]


// export default new Router({
//   routes: constantRouterMap
// })
// export default new Router({
//   // mode: 'history',
//   routes:constantRouterMap
// })


let route =  new Router({  
  mode: 'history',
  base: './',
  routes: constantRouterMap,
})
route.beforeEach((to, from, next) => {   //利用签好字守卫在登录之前进行权限控制
  //获取用户权限信息，为空即没登录，跳转至登录页
  if (to.path === '/login') {
    next();
  } else {
    let role = sessionStorage.getItem('roles') || route.app.$options.store.state.roles;
    console.log(role)
    if (role === '') {  
      next('/login');
    } else {
      if(to.matched.every(item => item.meta.indexOf(role) > -1)) {  //循环路由，看是否有权限进行访问
        next();
      } else {
        next('/login');
      }
    }
  }
});

export default route;