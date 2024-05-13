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

export default new Router({
  mode: 'history',
  routes: [
    {
      path: '',
      name: 'layout',
      component: Layout,
      children:[
        {
          path:"",
          name:"home",
          component:Home,
        },
        {
          path:"/Storage",
          name:"Storage",
          component:Storage,
        },
        {
          path:"/kufang",
          name:"kufang",
          component:kufang,
        },
        {
          path:"/area",
          name:"area",
          component:Area,
        },
        {
          path:"/shelves",
          name:"shelves",
          component:Shelves,
        },
        {
          path:"/Location",
          name:"Location",
          component:Location,
        },
        {
          path:"/Allocation",
          component:{template:`<router-view></router-view>`},
          children:[
           {
            path:"/Allocation",
            name:"Allocation",
            component:Allocation,
           },
           {
            path:"/print",
            name:"print",
            component:()=>import("@/Pages/allocation/print")
           }
          ]
        }
      ]
    },
    {
      path: '/login',
      name: 'login',
      component:()=>import("@/Pages/User")
    }
  ]
})
