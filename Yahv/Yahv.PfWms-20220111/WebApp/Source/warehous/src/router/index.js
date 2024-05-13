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
    },
    // {
    //   path: '/UnSortingList',
    //   name: 'UnSortingList',
    //   component:()=>import("@/Pages/HKWarehouse/UnSortingList"),
    // },
    // {
    //   path: '/SealedList',
    //   name: 'SealedList',
    //   component:()=>import("@/Pages/HKWarehouse/SealedList"),
    // },
    // {
    //   path: '/HKDeclare',
    //   name: 'HKDeclare',
    //   component:()=>import("@/Pages/HKWarehouse/HKDeclare"),
    // },
    // {
    //   path: '/SealedInfo',
    //   name: 'SealedInfo',
    //   component:()=>import("@/Pages/HKWarehouse/SealedInfo"),
    // },
    // {
    //   path: '/VoyageList',
    //   name: 'VoyageList',
    //   component:()=>import("@/Pages/HKWarehouse/VoyageList"),
    // },
    // {
    //   path: '/VoyageInfo',
    //   name: 'VoyageInfo',
    //   component:()=>import("@/Pages/HKWarehouse/VoyageInfo"),
    // },
    {
      path: '/HKStorageCharge',
      name: 'HKStorageCharge',
      component:()=>import("@/Pages/HKExpenses/HKStorageCharge"),
    },
    {
      path: '/HKIncome',
      name: 'HKIncome',
      component:()=>import("@/Pages/HKExpenses/HKIncome"),
    },
    {
      path: '/HKNothingEnter',
      name: 'HKNothingEnter',
      component:()=>import("@/Pages/HKWarehouse/HKNothingEnter"),
    }
  ],
})
export default route;