import {Carriers} from "@/api/index"
const homeindex ={
    state:{
        Acrivename:"",
        Budgetdetail:false,  //收支明细弹出框
        showdetail:false,    //右侧抽屉弹出
        closeLeaseDetail:false,
        showdetailout:false,//出库右侧列表
        showtypeout:0,  //出库显示不同组件的状态
        showtypein:0,   //入库显示不同组件的状态
        TransportDetail:false,//报关运输详情
    },
    getters:{
        // getRoles: state => state.roles,
    },
    mutations:{
        // setnvadatamut(state,payload){
        //     state.listData=payload
        // }
        setBudgetdata(state,payload){
            state.Budgetdetail=payload;
        },
        changeshow(state,payload){
            state.showdetail=payload;
        },
        changecloseLD(state,payload){
            state.closeLeaseDetail=payload;
        },
        changeshowout(state,payload){
            state.showdetailout=payload;
        },
        changeshowtype(state,payload){
            state.showtypeout=payload;
        },
        changeshowtypein(state,payload){
            state.showtypein=payload;
        },
        changeTransportDetail(state,payload){
           state.TransportDetail=payload;
        },
        changeAcrivename(state,payload){
            state.Acrivename=payload;
        }
    },
    actions:{
        // setnvadata(context,payload){
        //   //处理异步请求的
        //   //context 是 store的上下文(store的别名)
        //     context.commit("setnvadatamut",payload)
        //  // console.log(context,payload)
        // },
        setBudget(context,payload){
            context.commit("setBudgetdata",payload)
        },
        setshowdetail(context,payload){
            context.commit("changeshow",payload)
        },
        setcloseLd(context,payload){
            context.commit("changecloseLD",payload)
        },
        setshowdetailout(context,payload){
            context.commit("changeshowout",payload)
        },
        setshowtype(context,payload){
            context.commit("changeshowtype",payload)
        },
        setshowtypein(context,payload){
            context.commit("changeshowtypein",payload)
        },
        setTransportDetail(context,payload){
            context.commit("changeTransportDetail",payload)
        },
        setAcrivename(context,payload){
            context.commit("changeAcrivename",payload)
        }
      }
  }
  export default homeindex
