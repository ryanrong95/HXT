import {
    PFWMS_API
  } from "@/main" //调用整体的url
  import axios from 'axios'
import { type } from "jquery"

// 获取到货方式枚举
// /wmsapi/enums/WaybillType
export function CgWaybillType() { //
  return axios.get(PFWMS_API + "/wmsapi/enums/WaybillType").then((res) => {
    return Promise.resolve(res.data)
  })
}
//   入库列表
//   http://hv.erp.b1b.com/wmsapi/cgsortings/show?id=id&pageindex=1&pagesize=20
  export function cgsortings(id,pageindex,pagesize) { //
    return axios.get(PFWMS_API + "/wmsapi/cgsortings/show?id="+id+"&pageindex="+pageindex+"&pagesize="+pagesize).then((res) => {
      return Promise.resolve(res.data)
    })
  }
// 入库列表搜索
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/show/
  export function cgSearch_sortings(data) { //
    return axios.post(PFWMS_API + "/wmsapi/cgsortings/show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 入库分拣详情
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/detail/Waybill202001140002
  export function CgDetail(id) { //
    return axios.get(PFWMS_API + "/wmsapi/cgsortings/detail/"+id).then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 入库分拣提交
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/enter/ 
  export function cgenter(data) { //
    return axios.post(PFWMS_API + "/wmsapi/cgsortings/enter/",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 获取SortingID的接口
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/GetSortingID
  export function GetSortingID() { //
    return axios.get(PFWMS_API + "/wmsapi/cgsortings/GetSortingID").then((res) => {
      return Promise.resolve(res.data)
    })
  }
  // 1.4根据waybillID获取到货历史的WaybillIDs
  // http://hv.warehouse.b1b.com/wmsapi/cgsortings/history/id
  // export function historyList(id) { //
  //   return axios.get(PFWMS_API + "/wmsapi/cgsortings/history/"+id).then((res) => {
  //     return Promise.resolve(res.data)
  //   })
  // }
//   1.5根据waybillID获取历史到货信息
// http://hv.warehouse.b1b.com/wmsapi/cgsortings/historydetail/
export function historyDetail(id) { //
  return axios.get(PFWMS_API + "/wmsapi/cgsortings/historydetail/"+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 历史到货删除
// http://localhost:8081/pfwms/wmsapi/cgsortings/DeleteSorting 
export function HistorDeleteSorting(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgsortings/DeleteSorting",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 获取全部承运商
// http://hv.warehouse.b1b.com/wmsapi/cgCarriers/Alls/?whCode=SZ
export function CgAllsCarriers(whCode) { 
  return axios.get(PFWMS_API + "/wmsapi/cgCarriers/Alls?whCode="+whCode).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 获取承运商 /cgCarriers/Show/ 
// 参数 waybillType：参见WaybillType枚举  whCode：库房编码
export function CgCarriers(waybillType,whCode,noticeType) { 
  return axios.get(PFWMS_API + "/wmsapi/cgCarriers/Show?waybillType="+waybillType+"&whCode="+whCode+'&noticeType='+noticeType).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 送货上门根据承运商选择司机与车牌
//  /cgCarriers/Drivers/
export function GetDriversCars(key) { 
  return axios.get(PFWMS_API + "/wmsapi/cgCarriers/Drivers?ID=" + key).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 入库分拣筛选
// http://hv.warehouse.b1b.com/wmsapi/cgsortings/detail/?id=Waybill202001070005&key=150545-2
export function search_detail(id,key) { 
  return axios.get(PFWMS_API + "/wmsapi/cgsortings/detail?id="+id+"&key="+key).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 入库分拣可用库位号
// http://hv.warehouse.b1b.com//wmsapi/Shelve/GetUsableShelves?whid=HK01_WLT /wmsapi/cgShelves/GetUsableShelves
export function GetUsableShelves(ID) {
  return axios.get(PFWMS_API + "/wmsapi/cgShelves/GetUsableShelves?whid=" + ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
//我去提货
// hv.warehouse.b1b.com/wmsapi/Sortings/TakeGoods
export function TakeGoods(id,adminid) {
  return axios.get(PFWMS_API + "/wmsapi/cgsortings/takegoods?id="+id+"&adminid="+adminid,).then((res) => {
    return Promise.resolve(res.data)
  })
}

//入库通知业务类型  出库通知业务类型
// http://hv.warehouse.b1b.com/WmsApi/enums/NoticeSource
export function NoticeSource() { //入库通知业务类型
  return axios.get(PFWMS_API + "/wmsapi/enums/NoticeSource").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 收支明细获取币种和科目
// 币种
// http://hv.warehouse.b1b.com/wmsapi/enums/currency
export function currency() {
  return axios.get(PFWMS_API + "/wmsapi/enums/currency").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 费用科目
// http://hv.warehouse.b1b.com/wmsapi/payments/subject
// http://hv.warehouse.b1b.com/wmsapi/payments/subject?entercode=1212&count=0
export function subject(data) {
  return axios.get(PFWMS_API + "/wmsapi/payments/subject", { params: data }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 费用录入保存提交
// http://hv.warehouse.b1b.com/wmsapi/Payments/enter
export function submitfee(data) {
  return axios.post(PFWMS_API + "/wmsapi/Payments/enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 香港费用录入保存提交
// http://hv.warehouse.b1b.com/wmsapi/Payments/enter
export function submitfeeHK(data) {
  return axios.post(PFWMS_API + "/wmsapi/Payments/HKEnter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 费用明细
// 收入记录
// {{host}}/wmsapi/Payments/IncomeRecords?waybillid=Waybill202003020024
export function IncomeRecords(id) {
  return axios.get(PFWMS_API + "/wmsapi/Payments/IncomeRecords?orderId="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 支出记录
// {{host}}/wmsapi/Payments/OutcomeRecords?waybillid=Waybill202001160044
export function OutcomeRecords(id) {
  return axios.get(PFWMS_API + "/wmsapi/Payments/OutcomeRecords?orderId="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// --收入 收款公司、付款公司
// {{host}}/wmsapi/Payments/IncomeParters?waybillid=Waybill202003100007 &otype=in
// otype in 入库 out 出库
export function IncomeParters(orderId) {
  return axios.get(PFWMS_API + "/wmsapi/Payments/IncomeParters?orderId="+orderId).then((res) => {
    return Promise.resolve(res.data)
  })
}
export function OutcomeParters(orderId) {
  return axios.get(PFWMS_API + "/wmsapi/Payments/OutcomeParters?orderId="+orderId).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 1.获取运单接口(根据承运商ID, 订单ID)
// http://localhost:8081/pfwms/wmsapi/Payments/GetWaybillCodeByCarrierID
export function GetWaybillCodeByCarrierID(OrderID,CarrierID) {
  return axios.get(PFWMS_API+"/wmsapi/Payments/GetWaybillCodeByCarrierID?OrderID="+OrderID+"&CarrierID="+CarrierID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 暂存部分
// 1. 暂存录入
// http://hv.warehouse.b1b.com/wmsapi/cgtempstocks/enter
// http://warehouse0.ic360.cn/wmsapi/cgNewtempstocks/enterforpda 新接口 修改暂存信息
export function enterforpda(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgNewtempstocks/enterforpda", data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 3. 已有暂存搜索
// http://hv.warehouse.b1b.com/wmsapi/cgtempstocks/show 废弃接口
// http://warehouse0.ic360.cn/wmsapi/cgNewTempStocks/show  新接口
export function SparateWaybill(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgNewTempStocks/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 4. 修改暂存 详情
// http://hv.warehouse.b1b.com/wmsapi/cgtempstocks/detail/Waybill202002150001 废弃
// http://warehouse0.ic360.cn/wmsapi/cgNewtempstocks/detail/Waybill202011270003 新接口
export function cgNewtempstocksDetail(Waybill) {
  return axios.get(PFWMS_API + "/wmsapi/cgNewtempstocks/detail/"+Waybill).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 5 暂存获取可用库位号 http://warehouse0.ic360.cn/wmsapi/cgShelves/GetUsableShelves?whid=HK


// 出库部分
//1 出库列表搜索 pickingsout
export function pickingsout(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgPickings/Show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2.详情页面的数据展示  wmsapi/cgPickings/Detail
export function CgPickingsDetail(Waybill) {
  return axios.get(PFWMS_API + "/wmsapi/cgPickings/detail/"+Waybill).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 3. 出库详情页面 筛选
// wmsapi/cgPickings/Notices
export function CgPickingsSearch(WaybillID,key) {
  return axios.get(PFWMS_API + "/wmsapi/cgPickings/Notices?WaybillID="+WaybillID+"&key="+key).then((res) => {
    return Promise.resolve(res.data)
  })
}
//4 出库提交按钮 wmsapi/cgPickings/Out
export function CgpickingsoutBtn(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgPickings/Out/",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
//5 出库异常 cgPickings/OutExcept string waybillid,string adminid,string orderid, string summary
//出库异常 深圳出库接口（PackingExcept） 目前没有香港出库异常的情况
export function CgpickingsErrorBtn(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgPickings/PackingExcept",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 6 转报关出库 完成分拣 TurnDeclare
export function CgpickingsTurnDeclare(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgPickings/TurnDeclare",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 7 出库客户确认单文件上传
// http://hv.warehouse.b1b.com/wmsapi/cgPickings/ReciptConfirm
export function CgReciptConfirm(adminID,waybillid) {
  return axios.post(PFWMS_API +'/wmsapi/cgPickings/ReceiptConfirm?adminID='+adminID+'&waybillid='+waybillid).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 8 出库保存打印的运单号http://localhost:8081/pfwms/wmsapi/cgPickings/UpdateWaybillCode
export function CgUpdateWaybillCode(data) {
  return axios.post(PFWMS_API +'/wmsapi/cgPickings/UpdateWaybillCode',data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//8  图片删除 单张http://hv.warehouse.b1b.com/wmsapi/cgFiles/Delete
export function CgDeleteFiles(data) {
  return axios.post(PFWMS_API +'/wmsapi/cgFiles/Delete',data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// //9 图片删除 多张 http://hv.warehouse.b1b.com/wmsapi/cgFiles/DeleteFiles 
// export function CgDeleteFileall(data) {
//   return axios.post(PFWMS_API +'/wmsapi/cgFiles/DeleteFiles',data).then((res) => {
//     return Promise.resolve(res.data)
//   })
// }

//9 图片删除 多张  （苏亮） http://hv.warehouse.b1b.com/wmsapi/cgNewtempstocks/deletefile
export function CgDeleteFileall(data) {
  return axios.post(PFWMS_API +'/wmsapi/cgNewtempstocks/deletefile',data).then((res) => {
    return Promise.resolve(res.data)
  })
}



// 库房设置部分
// 1.1 获取库区  /wmsapi/cgShelves/ShowRegions
export function CggetShowRegions(data) {
  return axios.get(PFWMS_API + "/wmsapi/cgShelves/ShowRegions",{ params: data }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 1.2 添加或修改库区
// /wmsapi/cgShelves/SetRegion
export function CgSetRegion(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgShelves/SetRegion",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2 卡板部分
// 2.1 获取卡板信息 /wmsapi/cgShelves/ShowPallets
export function ShowPallets(data) {
  return axios.get(PFWMS_API + "/wmsapi/cgShelves/ShowPallets",{ params: data }).then((res) => {
    return Promise.resolve(res.data)
  })
}
//2.2 添加或修改卡板信息 /wmsapi/cgShelves/SetPallet
export function CgSetPallet(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgShelves/SetPallet",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 3  库位部分
// 3.1 库位的获取 /wmsapi/cgShelves/ShowPlaces
export function CgShowPallets(data) {
  return axios.get(PFWMS_API + "/wmsapi/cgShelves/ShowPlaces",{ params: data }).then((res) => {
    return Promise.resolve(res.data)
  })
}
//3.2 添加或修改库位信息 /wmsapi/cgShelves/SetPlace
export function CgSetPlace(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgShelves/SetPlace",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 获取当前库房与库房权限
// /wmsapi/cgwarehouses/Show  //获得对应库房
export function Cgwarehouses() {
  return axios.get(PFWMS_API + "/wmsapi/cgwarehouses/Show").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 5 根据当前库房获取对应菜单权限
// /wmsapi/cgwarehouses/Menus
export function CgwarehousesMenus(id) {
  return axios.get(PFWMS_API + "/wmsapi/cgwarehouses/Menus?whid="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 6 删除库区，库位，卡板
// /wmsapi/cgShelves/Delete
export function CgDelete(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgShelves/Delete",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//7 箱号部分
// 1 获取可用箱号部分  /wmsapi/cgBoxes/Show
export function CgBoxesShow(data) {
  return axios.get(PFWMS_API + "/wmsapi/cgBoxes/Show",{ params: data }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 7.2 申请连续箱号
// http://hv.warehouse.b1b.com/wmsapi/cgBoxes/EnterSeries
export function CgEnterSeries(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgBoxes/EnterSeries",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 8 报关管理
//8.1.获取以小定单为单位的报关数据 /cgDelcare/GetList/
export function CgDelcarelist(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgDelcare/GetList",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 8.2 选中后点击申报按钮
export function CustomsApply(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgDelcare/Declare",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 8.3 报关运输列表
//  /CgDelcareShip/GetList/
export function CustomTransport(data) {
  return axios.post(PFWMS_API + "/wmsapi/CgDelcareShip/GetList",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 8.4 报关运输详情
// /CgDelcareShip/Detail/{ID}
export function CgDelcareShipDetail(ID) {
  return axios.get(PFWMS_API + "/wmsapi/CgDelcareShip/Detail?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 8.5 报关运输出库
// /CgDelcareShip/AutoHkExit	
export function CgAutoHkExit(ID,adminID) {
  return axios.get(PFWMS_API + "/wmsapi/cgDelcare/AutoHkExit?LotNumber="+ID+"&adminID="+adminID).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 8.6 截单状态枚举
// http://hv.warehouse.b1b.com/wmsapi/cgDelcareShip/EnumCgCuttingOrderStatu
export function EnumCgCuttingOrderStatu() {
  return axios.get(PFWMS_API + "/wmsapi/cgDelcareShip/EnumCgCuttingOrderStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 箱签打印接口
//  1  4.4箱号打印接口 http://hv.warehouse.b1b.com/wmsapi/cgBoxes/GetPrintInfo
export function printboxcode(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgBoxes/GetPrintInfo1",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2保存已经选择的箱号 http://hv.warehouse.b1b.com/wmsapi/cgBoxes/Enter
// export function BoxcodeEnter(data) {
//   return axios.post(PFWMS_API + "/wmsapi/cgBoxes/Enter",data).then((res) => {
//     return Promise.resolve(res.data)
//   })
// }
// 2保存已经选择的箱号 7-14 大改 新接口  http://hv.warehouse.b1b.com/wmsapi/cgBoxes/NewEnter
export function BoxcodeEnter(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgBoxes/NewEnter",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// // 3 页面关闭 删除箱号
// // http://hv.warehouse.b1b.com/wmsapi/cgBoxes/Delete
// export function BoxcodeDelete(data) {
//   return axios.post(PFWMS_API + "/wmsapi/cgBoxes/Delete",data).then((res) => {
//     return Promise.resolve(res.data)
//   })
// }
// 3 页面关闭 删除箱号 7-14 大改 新接口  http://hv.warehouse.b1b.com/wmsapi/cgBoxes/NewDelete
export function BoxcodeDelete(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgBoxes/NewDelete",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 入库单 出库单 出入库单
// 入库单 
// 1 左侧入库单  wmsapi/cgInputReport/InputReportGroup  cgInputReport/InputReport
export function CgInputReportGroup(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgInputReport/InputReportGroup",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 订单入库，到货历史
// 1 cgInputReport/OrderInputReportGroup  参数：warehouseId：库房ID orderId:订单ID
export function CgOrderInputReportGroup(warehouseId,orderId,status) {
  return axios.get(PFWMS_API + "/wmsapi/cgInputReport/OrderInputReportGroup?warehouseId="+warehouseId+"&orderId="+orderId+"&status="+status).then((res) => {
    return Promise.resolve(res.data)
  })
}
//出库单 cgOutputReport/OutputReportGroup
export function CgOutputReportGroup(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgOutputReport/OutputReport",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 在库管理 -- 出入库单
// Get：cgClientReport/ClientReport
export function CgClientReport(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgClientReport/ClientReport",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 在库管理
// http://hv.warehouse.b1b.com/wmsapi/cgstorages/show 
export function Cgstoragesshow(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgstorages/show",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 库存数量的修改
// http://hv.warehouse.b1b.com/wmsapi/cgStorages/UpdateDeliveredQty 
export function CgUpdateDeliveredQty(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgstorages/UpdateDeliveredQty",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 库位的修改
// http://hv.warehouse.b1b.com/wmsapi/cgStorages/UpdateDeliveredQty 
export function CgUpdateDeliveredLocate(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgstorages/UpdateDeliveredLocate",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 深圳入库
// 1 搜获与获取 http://hv.warehouse.b1b.com/wmsapi/cgszsortings/show/
export function cgszsortingslist(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgszsortings/show",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2.详情页数据获取
// http://hv.warehouse.b1b.com/wmsapi/cgszsortings/detail/Waybill202001140002
export function Cgszsortingsdetail(ID) {
  return axios.get(PFWMS_API + "/wmsapi/cgszsortings/detail/"+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 3 上架 http://hv.warehouse.b1b.com/wmsapi/cgszsortings/update/ 
export function cgszsortingsupdate(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgszsortings/update",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 4 获取深圳打印入库单数据的接口
// http://hv.warehouse.b1b.com/wmsapi/cgszsortings/detailprint/Waybill202006040050
export function Cgszsdetailprint(ID,AdminID) {
  return axios.get(PFWMS_API + "/wmsapi/cgszsortings/detailprint/"+ID+"?AdminID="+AdminID ).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5. 获取深圳可用库位号
// http://hv.warehouse.b1b.com/wmsapi/cgShelves/SZShow
export function cgSZShelves(whCode) {
  return axios.get(PFWMS_API + "/wmsapi/cgShelves/SZShow?whCode="+whCode).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 6. 保存深圳入库上架 库位号的保存
// http://hv.warehouse.b1b.com/wmsapi/cgShelves/SZEnter
export function cgSZEnter(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgShelves/SZEnter",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 包装类型
// 1.1获取箱号包装种类
// http://hv.warehouse.b1b.com/wmsapi/cgBoxes/GetPackageType
export function GetPackageType() {
  return axios.get(PFWMS_API + "/wmsapi/cgBoxes/GetPackageType").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 1.2修改箱号的包装种类
// http://hv.warehouse.b1b.com/wmsapi/cgBoxes/EnterPackageType
// 参数
// "boxCode": null, //箱号（必要的）
// "packageType": null, //包装种类(必要的)
// "adminID": null,//操作人（必要的）
export function cgEnterPackageType(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgBoxes/EnterPackageType",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// wmsapi/others/getclientdata?enterCode=NL020
export function Getclientdata(enterCode) {
  return axios.get(PFWMS_API + "/wmsapi/others/getclientdata?enterCode="+enterCode).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 深圳出库批量上传送货单
// cgPickings/SZOutList
export function cgSZOutList(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgPickings/SZOutList",data ).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 新增承运商
export function CarrierAdd(data) {
  return axios.post(PFWMS_API+"/CsrmAPI/Carrier",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 获取承运商的类型
// /CsrmAPI/Carrier/Type
export function GetCarrierType() {
  return axios.get(PFWMS_API+"/CsrmAPI/Carrier/Type").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 新增司机
export function DriverAdd(data) {
  return axios.post(PFWMS_API+"/CsrmAPI/Driver",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 新增车牌号
export function TransportAdd(data) {
  return axios.post(PFWMS_API+"/CsrmAPI/Transport",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 同时新增承运商，司机，车牌号 hv.erp.b1b.com/csrmapi/Carrier/AllEnter
export function AddAllEnter(data) {
  return axios.post(PFWMS_API+"/CsrmAPI/Carrier/AllEnter",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 快递与联系人的新增 hv.erp.b1b.com/csrmapi/Carrier/Contact
export function AddContact(data) {
  return axios.post(PFWMS_API+"/CsrmAPI/Carrier/Contact",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 深圳出库 获取打印数据（报关内单）
// http://localhost:8081/pfwms/wmsapi/cgPickings/GetSzPrintData/Waybill202006120004
export function GetSzPrintData(id) {
  return axios.get(PFWMS_API+"/wmsapi/cgPickings/GetSzPrintData/"+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 深圳出库详情
// http://localhost:8081/pfwms/wmsapi/cgPickings/Detail/Waybill202006120004
export function GetSzDetail(id) {
  return axios.get(PFWMS_API+"/wmsapi/cgPickings/Detail/"+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 深圳出库分拣按钮
// http://localhost:8081/pfwms/wmsapi/cgPickings/SzInsideOut/Waybill202006120024
export function SzInsideOut(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgPickings/SzInsideOut",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
//入库退回状态控制
// http://localhost:8081/pfwms/wmsapi/cgsortings/GetWaybillCurrentStatus 
export function GetWaybillCurrentStatus(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgsortings/GetWaybillCurrentStatus",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 操作日志
// http://localhost:8081/pfwms/wmsapi/cgLogOperators/show
export function cgLogOperators(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgLogOperators/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 报关运输详情页面的香港库房封条号更新接口
// http://localhost:8081/pfwms/wmsapi/cgDelcare/UpdateHKSealNumber
export function UpdateHKSealNumber(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgDelcare/UpdateHKSealNumber",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 导出打印单接口  thwt(提货委托书), 或者 hwlz(货物流转书)
// http://localhost:8081/pfwms/wmsapi/cgDelcare/PrintExportFiles
export function PrintExportFiles(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgDelcare/PrintExportFiles",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 封箱部分接口
// 1 .展示风向列表数据
// http://localhost:8081/pfwms/wmsapi/cgInputReport/TinyOrderIDReportGroup?warehouseId=HK&orderId=XL00920200813009
export function GetSealinglist(warehouseId,orderId) {
  return axios.get(PFWMS_API+"/wmsapi/cgInputReport/TinyOrderIDReportGroup?warehouseId="+warehouseId+"&orderId="+orderId).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2 .封箱修改数量的接口
// http://localhost:8081/pfwms/wmsapi/cgSortings/ModifyQuantity
export function ModifyQuantity(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgSortings/ModifyQuantity",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 3 .确认封箱接口  请求方式:Post
// http://localhost:8081/pfwms/wmsapi/cgSortings/CloseBoxes
export function CloseBoxes(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgSortings/CloseBoxes",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


//1.获取快递公司编码
// http://localhost:8081/pfwms/wmsapi/Kdn/GetShipperCodes
// 请求方式Get
export function GetShipperCodes() {
  return axios.get(PFWMS_API+"/wmsapi/Kdn/GetShipperCodes").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2.获取快递类型
// http://localhost:8081/pfwms/wmsapi/Kdn/GetExpTypes?shipperCode=KYSY
// 请求方式Get
export function GetExpTypes(type) {
  return axios.get(PFWMS_API+"/wmsapi/Kdn/GetExpTypes?shipperCode="+type).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 3.获取计费方式
// http://localhost:8081/pfwms/wmsapi/Kdn/GetPayTypes
// 请求方式 GET
export function GetPayTypes() {
  return axios.get(PFWMS_API+"/wmsapi/Kdn/GetPayTypes").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 4.运单修改快递信息接口, 更新深圳出库详情里的数据
// 提交时调用这个接口
// http://localhost:8081/pfwms/wmsapi/cgPickings/UpdateWaybillExpress
export function UpdateWaybillExpress(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgPickings/UpdateWaybillExpress",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 5.获取EMS计费方式
// http://localhost:8081/pfwms/wmsapi/Kdn/GetPayTypesForEMS
// 请求方式 GET
export function GetPayTypesForEMS() {
  return axios.get(PFWMS_API+"/wmsapi/Kdn/GetPayTypesForEMS").then((res) => {
    return Promise.resolve(res.data)
  })
}



// 转报关管理
// 1 转报关列表
// http://localhost:8081/pfwms/wmsapi/cgCustomsStorage/Show
export function cgCustomsStorageShow(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgCustomsStorage/Show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 2  转报关运单详情信息
// http://localhost:8081/pfwms/wmsapi/cgCustomsStorage/Detail/Waybill202008170019
export function cgCustomsStorageDetail(ID) {
  return axios.get(PFWMS_API+"/wmsapi/cgCustomsStorage/Detail/"+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
//  3 转报关获取封箱页面数据
// http://localhost:8081/pfwms/wmsapi/cgCustomsStorageReport/TinyOrderIDReportGroup?orderId=XL00920200817003
export function TinyOrderIDReportGroup(ID) {
  return axios.get(PFWMS_API+"/wmsapi/cgCustomsStorageReport/TinyOrderIDReportGroup?orderId="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 4 删除转报关已经拣货的历史记录
// http://localhost:8081/pfwms/wmsapi/cgCustomsStorage/DeletePicking
export function DeletePicking(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgCustomsStorage/DeletePicking",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5.转报关封箱接口
// http://localhost:8081/pfwms/wmsapi/cgCustomsStorage/CloseBoxes
export function cgCustomsStorageCloseBoxes(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgCustomsStorage/CloseBoxes",data).then((res) => {
    return Promise.resolve(res.data)
  })
}
//6  重构转报关出库分拣
// cgCustomsStorage/TurnDeclare
export function cgCustomsStorageTurnDeclare(data) { 
  return axios.post(PFWMS_API + "/wmsapi/cgCustomsStorage/TurnDeclare",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 判断是否收款
// wmsapi/Payments/IsPay?orderID=XL00120201021501
export function IsPayPayments(ID,type) {
  return axios.get(PFWMS_API+"/wmsapi/Payments/IsPay?orderID="+ID+"&type="+type).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 收入支出统计接口
// 1 获取客户公司 http://hv.warehouse.b1b.com/wmsapi/incomestatistics/getclients
export function getclients() {
  return axios.get(PFWMS_API+"/wmsapi/incomestatistics/getclients").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2 获取内部公司 http://hv.warehouse.b1b.com/wmsapi/incomestatistics/GetCompanies
export function GetCompanies() {
  return axios.get(PFWMS_API+"/wmsapi/incomestatistics/GetCompanies").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 3 获取币种 http://hv.warehouse.b1b.com/wmsapi/incomestatistics/GetCurrencies
export function GetCurrencies() {
  return axios.get(PFWMS_API+"/wmsapi/incomestatistics/GetCurrencies").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 获取收入列表 http://hv.warehouse.b1b.com/wmsapi/incomestatistics/show
export function showIncomeList(data) {
  return axios.post(PFWMS_API+"/wmsapi/incomestatistics/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5 获取收入详情页接口 http://hv.warehouse.b1b.com/wmsapi/incomestatistics/getdetail
export function showIncomeDetail(data) {
  return axios.post(PFWMS_API+"/wmsapi/incomestatistics/getdetail",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 6 获取支出列表 http://hv.warehouse.b1b.com/wmsapi/PayStatistics/show
export function showExpendlist(data) {
  return axios.post(PFWMS_API+"/wmsapi/PayStatistics/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 7 获取支出详情 http://hv.warehouse.b1b.com/wmsapi/PayStatistics/getdetail
export function showExpendDetail(data) {
  return axios.post(PFWMS_API+"/wmsapi/PayStatistics/getdetail",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


//获取代报关收费科目
export function DeclarationJson() {
  return axios.get(PFWMS_API + '/Print/Declaration.json').then((res) => {
    return Promise.resolve(res.data)
  })
}
//获取代仓储收费科目
export function SorageJson() {
  return axios.get(PFWMS_API + '/Print/Sorage.json').then((res) => {
    return Promise.resolve(res.data)
  })
}
//获取专门仓储费科目
export function Storagechargejson() {
  return axios.get(PFWMS_API + '/Print/Storagecharge.json').then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取单加仓储费记录列表
//http://hv.warehouse.b1b.com/wmsapi/Payments/IncomeRecordsForWarehouseFee?orderId=WL62420210611001
export function IncomeRecordsForWarehouseFee(OrderID) {
  return axios.get(PFWMS_API + '/wmsapi/Payments/IncomeRecordsForWarehouseFee?orderId=' + OrderID).then((res) => {
    return Promise.resolve(res.data)
  })
}
//判断是否收取过入仓费
//http://hv.warehouse.b1b.com/wmsapi/Payments/IsRecordWarehouseFee?OrderId=WL02520210708004
export function IsRecordWarehouseFee(OrderID) {
  return axios.get(PFWMS_API + '/wmsapi/Payments/IsRecordWarehouseFee?OrderId=' + OrderID).then((res) => {
    return Promise.resolve(res.data)
  })
}


//仓储费管理 
// 1 获取列表数据 http://hv.warehouse.b1b.com/wmsapi/statisticstorage/show
export function statisticstorageshow(data) {
  return axios.post(PFWMS_API+"/wmsapi/statisticstorage/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2 修改仓储费数据接口 http://hv.warehouse.b1b.com/wmsapi/statisticstorage/Modify 
export function statisticstorageModify(data) {
  return axios.post(PFWMS_API+"/wmsapi/statisticstorage/Modify",data).then((res) => {
    return Promise.resolve(res.data)
  })
}




// 深圳出库 快递类型 修改运单号
// http://hv.warehouse.b1b.com/wmsapi/cgPickings/ModifyWaybillCode
export function ModifyWaybillCode(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgPickings/ModifyWaybillCode",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2.用于修改货运方式是快递的运单的收货人信息 地址、联系人、电话
// 接口: http://hv.warehouse.b1b.com/wmsapi/cgPickings/ModifyWaybillConsigneeInfo
// 请求方式:Post
export function ModifyWaybillConsigneeInfo(data) {
  return axios.post(PFWMS_API+"/wmsapi/cgPickings/ModifyWaybillConsigneeInfo",data).then((res) => {
    return Promise.resolve(res.data)
  })
}



//二次封箱 需改运单号与承运商
// 二次封箱 需改运单号与承运商
// 接口: http://warehouse0.ic360.cn/wmsapi/cgSortings/ModifyWbCodeAndCarrierID
// 请求方式:Post
export function CGModifyWbCodeAndCarrierID(data) {
  return axios.post(PFWMS_API + "/wmsapi/cgSortings/ModifyWbCodeAndCarrierID", data).then((res) => {
    return Promise.resolve(res.data)
  })
}
