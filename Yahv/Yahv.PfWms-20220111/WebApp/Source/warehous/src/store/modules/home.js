const homeindex ={
  state:{
    routers:[],
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
      }
  },
  actions:{
      setnvadata(context,payload){
        //处理异步请求的
        //context 是 store的上下文(store的别名)
          context.commit("setnvadatamut",payload)
       // console.log(context,payload)
      },
    }
}
export default homeindex
// export default {
//     namespaced:true,
//     state:{
//       routers:[],
//       listData:[
//               {
//                   title:"库房配置",
//                   href:"/"
//               },
//               {
//                   title:"库区配置",
//                   href:"/area"
//               }
//           ],
//     },
//     getters:{
//         getRoles: state => state.roles,
//     },
//     mutations:{
//         setnvadatamut(state,payload){
//             state.listData=payload
//         }
//     },
//     actions:{
//         setnvadata(context,payload){
//           //处理异步请求的
//           //context 是 store的上下文(store的别名)
//             context.commit("setnvadatamut",payload)
//          // console.log(context,payload)
//         },
       
//       }
//   }