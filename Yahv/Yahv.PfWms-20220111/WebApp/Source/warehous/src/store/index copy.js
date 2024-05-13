
import Vue from 'vue'
import Vuex from 'vuex'
Vue.use(Vuex)
const store = new Vuex.Store({
    //存储用户权限信息，初始时为空
    roles: '',
    namespaced:true,
    state:{
      listData:[
              {
                  title:"库房配置",
                  href:"/"
              },
              {
                  title:"库区配置",
                  href:"/area"
              }
          ],
    },
    getters:{
        getRoles: state => state.roles,
    },
    mutations:{
        setnvadatamut(state,payload){
            state.listData=payload
        },
        //添加用户权限，如果存在，不添加
        addRoles(state,{roles}) {
            state.roles = roles;
        }
    },
    actions:{
        setnvadata(context,payload){
          //处理异步请求的
          //context 是 store的上下文(store的别名)

            context.commit("setnvadatamut",payload)
         // console.log(context,payload)
        },
        addRoles:({commit},{roles}) =>{
            commit('addRoles',{roles});
        }
      }
})

export default store


// import Vue from 'vue'
// import Vuex from 'vuex'

// Vue.use(Vuex)

// const state = {
//   //存储用户权限信息，初始时为空
//   roles: ''
// }

// const getters = {
//   getRoles: state => state.roles,
// }

// const mutations = {
//   //添加用户权限，如果存在，不添加
//   addRoles(state,{roles}) {
//     state.roles = roles;
//   }
// }

// const actions = {
//   addRoles:({commit},{roles}) =>{
//     commit('addRoles',{roles});
//   }
// }

// export default new Vuex.Store({
//   state,
//   getters,
//   actions,
//   mutations
// })
