// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import iView from 'iview';
import 'iview/dist/styles/iview.css';
import axios from 'axios'
import './assets/iconfont/iconfont.css'  //字体图标
//import moment from 'moment'//导入时间转换文件
import store from "./store"
import ECharts from 'vue-echarts'
import 'echarts/lib/chart/line'

import * as filters from './filters/index'
Object.keys(filters).forEach(key => {
    Vue.filter(key, filters[key])
})

Vue.component('chart', ECharts)

Vue.config.productionTip = false
Vue.use(iView);
const PFWMS_API=process.env.PFWMS_API //引入不同的地址版本，根据不同的环境，引入不同的接口地址
export {  //将接口地址惊醒暴露，方便在页面里进行引用
  PFWMS_API,//库房接口
}



// 用户手动刷新页面，这是路由会被重设，要重新新增
if (sessionStorage.getItem('user')) {
  let routes = JSON.parse(sessionStorage.getItem('routes'))
  store.dispatch("add_Routes", routes)
}
// 登录状态判断
router.beforeEach((to, from , next) => {
  
  if(to.path.indexOf("printtest")!=-1||to.path.indexOf("productlabel")!=-1){
    next()
  }else{
    if (!sessionStorage.getItem('user') && to.path !== '/login') {
      next({
        path: '/login',
        query: {redirect: to.fullPath}
      })
    } else {
      next()
    }
  }
  
})

Vue.prototype.axios = axios
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: { App },
  template: '<App/>'
})


