// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import store from "./store"
import router from './router';
import ViewUI from 'view-design';
import 'view-design/dist/styles/iview.css';
Vue.use(ViewUI);
Vue.config.productionTip = false


const PFWMS_API=process.env.PFWMS_API //引入不同的地址版本，根据不同的环境，引入不同的接口地址
const SERM=process.env.SERM
export {  //将接口地址惊醒暴露，方便在页面里进行引用
  PFWMS_API,//库房接口
  SERM
}
console.log(SERM)
if (sessionStorage.getItem('user')) {
  let routes = JSON.parse(sessionStorage.getItem('routes'))
  store.dispatch("add_Routes", routes)
}

import * as filters from './filters/index'
Object.keys(filters).forEach(key => {
    Vue.filter(key, filters[key])
})


// 登录状态判断
router.beforeEach((to, from , next) => {
    if (!sessionStorage.getItem('user') && to.path !== '/login') {
      next({
        path: '/login',
        query: {redirect: to.fullPath}
      })
    } else {
      next()
    }
  
  
})

new Vue({
  el: '#app',
  store,
  router,
  components: { App },
  template: '<App/>'
})

