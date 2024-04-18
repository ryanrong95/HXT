const common ={
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
        PageArr:[10,20,30,40,50]
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
export default common