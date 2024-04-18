import {
    PFWMS_API
  } from "@/main" //调用整体的url
  import axios from 'axios'

  // 出库
  // 1 出库待提货页面  ../pswms/DappApi/OutNotices/Show_PickUp_NotArranged
  export function PickUp_NotArranged(data) { 
    return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_PickUp_NotArranged",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 2 出库已提货页面  ../pswms/DappApi/OutNotices/Show_PickUp_Arranged
  export function PickUp_Arranged(data) { 
    return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_PickUp_Arranged",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }


// 3 出库送货上门 待安排 ../pswms/DappApi/OutNotices/Show_Delivery_NotArranged
export function Delivery_NotArranged(data) { 
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_Delivery_NotArranged",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 4 出库送货上门 已安排 ../pswms/DappApi/OutNotices/Show_Delivery_Arranged
export function Delivery_Arranged(data) { 
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_Delivery_Arranged",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 出库送货上门 已完成 ../pswms/DappApi/OutNotices/Show_Delivery_Completed
export function Delivery_Completed(data) { 
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_Delivery_Completed",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5// 出库通知待处理 ../pswms/DappApi/OutNotices/Show
export function Show_UnExited(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_UnExited",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 6  出库通知 已处理 ../pswms/DappApi/OutNotices/Show
export function Show_Exited(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Show_Exited",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库详情 ../pswms/DappApi/OutNotices/Detail?NoticeID=
export function NoticeDetail(ID) {
  return axios.get(PFWMS_API + "/DappApi/OutNotices/Detail?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库获取 费用列表 ../pswms/DappApi/NoticeCharges/List?id=
export function NoticeCharges_list(ID) {
  return axios.get(PFWMS_API + "/DappApi/NoticeCharges/List?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库单 列表 
export function Show_Product_Outbound(data) {
  return axios.post(PFWMS_API + "/DappApi/Reports/Show_Product_Outbound",data).then((res) => {
    return Promise.resolve(res.data)
  })
}




// 1出库送货上门 详情提交 ../pswms/DappApi/OutNotices/Delivery_Arranged
export function submit_Delivery(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Delivery_Arranged",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2 出库自提安排 详情提交 ./pswms/DappApi/OutNotices/PickUp_Arranged
export function submit_PickUp(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/PickUp_Arranged",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 3 出库通知 详情提交 ./pswms/DappApi/OutNotices/Express_Arranged
export function Express_Arranged(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/Express_Arranged",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 出库产品通知项 异常提交 
export function ItemException(data) {
  return axios.post(PFWMS_API + "/DappApi/OutNotices/ItemException",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 通用接口 待出库 
//  状态 http://sz.warehouse.b1b.com/dappapi/Enums/NoticeStatus
export function NoticeStatus_UnComplete(data) { 
  return axios.get(PFWMS_API + "/dappapi/Enums/NoticeStatus_UnComplete",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// //  状态 已出库
export function NoticeStatus_Complete(data) { 
  return axios.get(PFWMS_API + "/dappapi/Enums/NoticeStatus_Complete",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


//获取司机列表 http://sz.warehouse.b1b.com/dappapi/Takers/List
export function TakersList(data) { 
  return axios.get(PFWMS_API + "/dappapi/Takers/List",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 打印预出库单后调用此接口  ../pswms/DappApi/OutNotices/Print_PreDeliveryFile?id=
export function Print_PreDeliveryFile(id) { 
  return axios.post(PFWMS_API + "/dappapi/OutNotices/Print_PreDeliveryFile?id="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取发货人信息http://sz.warehouse.b1b.com/dappapi/Fixed/SzSender
export function GetSzSender() { 
  return axios.get(PFWMS_API + "/dappapi/Fixed/SzSender").then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取承运类型 ExpressMethod
export function ExpressMethod(expressName) { 
  return axios.get(PFWMS_API + "/dappapi/Enums/ExpressMethod?expressName="+expressName).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 获取支付类型  FreightPayer
export function FreightPayer() { 
  return axios.get(PFWMS_API + "/dappapi/Enums/FreightPayer").then((res) => {
    return Promise.resolve(res.data)
  })
}


// 更改出库状态 Upload_CustomSignFile
export function Upload_CustomSignFile(id) { 
  return axios.post(PFWMS_API + "/dappapi/OutNotices/Upload_CustomSignFile?id="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 送货待安排新增司机
// http://sz.warehouse.b1b.com/DappApi/Takers/Enter
export function TakersEnter(data) { 
  return axios.post(PFWMS_API + "/dappapi/Takers/Enter",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//修改司机信息 http://sz.warehouse.b1b.com/DappApi/Takers/Modify
export function TakersModify(data) { 
  return axios.post(PFWMS_API + "/dappapi/Takers/Modify",data).then((res) => {
    return Promise.resolve(res.data)
  })
}