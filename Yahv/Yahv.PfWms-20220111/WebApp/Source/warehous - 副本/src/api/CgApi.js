import {
    PFWMS_API
  } from "@/main" //调用整体的url
  import axios from 'axios'

//   入库列表
//   http://hv.erp.b1b.com/wmsapi/cgsortings/show?id=id&pageindex=1&pagesize=20
  export function cgsortings(id,pageindex,pagesize) { //用户菜单接口ERM
    return axios.get(PFWMS_API + "/wmsapi/cgsortings/show?id="+id+"&pageindex="+pageindex+"&pagesize="+pagesize).then((res) => {
      return Promise.resolve(res.data)
    })
  }
// 入库列表搜索
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/show/
  export function cgSearch_sortings(data) { //用户菜单接口ERM
    return axios.post(PFWMS_API + "/wmsapi/cgsortings/show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 入库分拣详情
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/detail/Waybill202001140002
  export function CgDetail(id) { //用户菜单接口ERM
    return axios.get(PFWMS_API + "/wmsapi/cgsortings/detail/"+id).then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 入库分拣提交
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/enter/ 
  export function cgenter(data) { //用户菜单接口ERM
    return axios.post(PFWMS_API + "/wmsapi/cgsortings/enter/",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }