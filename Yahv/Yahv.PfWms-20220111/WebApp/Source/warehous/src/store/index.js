
import Vue from 'vue'
import Vuex from 'vuex'
import homeindex from "./modules/home"
import addRoutes from "./modules/addRoutes"
import common from "./modules/common"
Vue.use(Vuex)

const store = new Vuex.Store({
    modules: {
        homeindex,
        addRoutes,
        common
    }
  })
  
  export default store