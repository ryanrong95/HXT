// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'
import App from './App'
import router from './router'
import iView from 'iview';
import 'iview/dist/styles/iview.css';
import axios from 'axios'
// import './assets/iconfont/iconfont.css'
import store from "./store"
Vue.config.productionTip = false
Vue.use(iView);
const API_ROOT = process.env.API_ROOT;   //引入不同的地址版本，根据不同的环境，引入不同的接口地址
export {  //将接口地址惊醒暴露，方便在页面里进行引用
  API_ROOT,
}
Vue.prototype.axios = axios
/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  store,
  components: { App },
  template: '<App/>'
})
