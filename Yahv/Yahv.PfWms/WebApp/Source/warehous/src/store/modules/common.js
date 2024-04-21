import {Carriers} from "@/api/index"
const homeindex ={
    state:{
        Acrivename:sessionStorage.getItem("Activename"),//展开后选中的子菜单
        openname:sessionStorage.getItem("openname"),//展开的菜单
        Budgetdetail:false,  //收支明细弹出框
        showdetail:false,    //右侧抽屉弹出
        closeLeaseDetail:false,
        showdetailout:false,//出库右侧列表
        showtypeout:0,  //出库显示不同组件的状态
        showtypein:0,   //入库显示不同组件的状态
        TransportDetail:false,//报关运输详情
        Spearatedrawer:false,//暂存右侧弹出
        WayParter:[],
        showModalShelve:false,//添加库区与货架的弹出框
        PageArr:[10,20,30,40,50]
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
            // state.Acrivename=payload;
            state.Acrivename=sessionStorage.getItem("Activename")
        },
        changeopenname(state,payload){
            // state.openname=payload;
            state.openname=sessionStorage.getItem("openname")
        },
        changeWayParter(state,payload){
            state.openname=payload;
        },
        changeSpearatedrawer(state,payload){
            state.Spearatedrawer=payload;
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
        },
        setopenname(context,payload){
            context.commit("changeopenname",payload)
        },
        setWayParter(context,payload){
            context.commit("changeWayParter",payload)
        },
        setSpearatedrawer(context,payload){
            context.commit("changeSpearatedrawer",payload)
        }
      }
  }
  export default homeindex
