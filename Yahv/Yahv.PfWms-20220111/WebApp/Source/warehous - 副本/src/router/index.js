import Vue from 'vue'
import Router from 'vue-router'
Vue.use(Router)
let route =  new Router({  
  // mode: 'history',
  base: './',
  routes: [
    {
      path: '/login',
      name: 'login',
      component:()=>import("@/Pages/User"),
    },
    {
      path: "/printtest", //库位标签打印
      name: "PrintTest",
      component:()=>import("@/Pages/PrintPage/PrintTest"),
    },
    {
      path: "/productlabel", //产品标签打印
      name: "productlabel",
      component:()=>import("@/Pages/PrintPage/productlabel"),
    },
    {
      path: '*',
      name: '404',
      component:()=>import("@/Pages/404"),
    }
  ],
})
export default route;