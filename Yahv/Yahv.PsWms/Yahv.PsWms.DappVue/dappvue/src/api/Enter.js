import {
    PFWMS_API
  } from "@/main" //调用整体的url
  import axios from 'axios'

//   1 入库通知 待入库 http://sz.warehouse.b1b.com/dappapi/InNotices/Show
export function InNoticesShow(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

//   2 入库通知详情 http://sz.warehouse.b1b.com/dappapi/InNotices/Detail?id=N202101180004
export function InNoticesDetail(ID) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Detail?id="+ID).then((res) => {
      return Promise.resolve(res.data)
    })
  }


  // 3 入库自提列表 http://sz.warehouse.b1b.com/dappapi/InPlanNotices/Show
  export function InPlanNotices(data) { //
    return axios.post(PFWMS_API + "/dappapi/InPlanNotices/Show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 4 入库自提 详情 http://sz.warehouse.b1b.com/dappapi/InPlanNotices/Detail?id=N202101180004
  export function InPlanNoticesDetail(ID) { //
    return axios.post(PFWMS_API + "/dappapi/InPlanNotices/Detail?id="+ID).then((res) => {
      return Promise.resolve(res.data)
    })
  }


  // 5.通知详情页中通知项列表(分页) http://sz.warehouse.b1b.com/dappapi/InNotices/NoticeItems
  export function NoticeItems(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/NoticeItems",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }


  // 6.入库通知详情页面中通知项的心跳 http://sz.warehouse.b1b.com/dappapi/InNotices/Hearting
  export function Hearting(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Hearting",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }


  // 7 入库分拣 http://sz.warehouse.b1b.com/dappapi/InNotices/Sorting
  export function InNoticesSorting(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Sorting",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  //8  自提信息提交 // http://sz.warehouse.b1b.com/dappapi/InPlanNotices/Arrange 
  export function InPlanNoticesArrange(data) { //
    return axios.post(PFWMS_API + "/dappapi/InPlanNotices/Arrange",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 9 入库详情页基础信息提交  http://sz.warehouse.b1b.com/dappapi/InNotices/Update
  export function InNoticesUpdate(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Update",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 10 入库单列表 http://sz.warehouse.b1b.com/dappapi/Reports/Show_Product_Inbound
  export function Show_Product_Inbound(data) { //
    return axios.post(PFWMS_API + "/dappapi/Reports/Show_Product_Inbound",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 11 入库复核 http://sz.warehouse.b1b.com/dappapi/InNotices/Review
  export function Review(data) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/Review",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

  // 12 新增分拣 获取NoticeitemID http://sz.warehouse.b1b.com/dappapi/InNotices/GetNoticeItemID 
  export function GetNoticeItemID() { //
    return axios.get(PFWMS_API + "/dappapi/InNotices/GetNoticeItemID").then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 13 删除分拣 http://sz.warehouse.b1b.com/dappapi/InNotices/DeleteNoticeItem?id=noticeitem123456
  export function DeleteNoticeItem(ID) { //
    return axios.post(PFWMS_API + "/dappapi/InNotices/DeleteNoticeItem?id="+ID).then((res) => {
      return Promise.resolve(res.data)
    })
  }