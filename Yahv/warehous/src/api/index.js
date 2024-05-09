import {
  PFWMS_API
} from "@/main" //调用整体的url
import axios from 'axios'

//库房部分
// export function getpfwms(id,name,ownerID,managerID,address){  //库房列表与查询
//     return axios.get(PFWMS_API+"/wmsapi/warehouse?id="+id+"&name="+name+"&ownerID="+ownerID+"&managerID="+managerID+"&address="+address).then((res) => {
//         return Promise.resolve(res.data)
//       })
// }
// export function getSubWarehouse(){  //库房枚举信息
//   return axios.get(PFWMS_API+"/wmsapi/SubWarehouse").then((res) => {
//       return Promise.resolve(res.data)
//     })
// }
// export function GetWarehouse(type){  //获取库房的部分信息
//   return axios.get(PFWMS_API+"/wmsapi/Warehouse/GetWarehouse?type="+type).then((res) => {
//       return Promise.resolve(res.data)
//     })
// }
// export function GetAdmin(name,topnum){  //获取库房的负责人
//   return axios.get(PFWMS_API+"/wmsapi/admins?name="+name+"&topnum="+topnum).then((res) => {
//       return Promise.resolve(res.data)
//     })
// }
// export function GetEnterprise(type,name,topnum){  //获取库房的所有人
//   return axios.get(PFWMS_API+"/wmsapi/enterprise?type="+type+"&name="+name+"&topnum="+topnum).then((res) => {
//       return Promise.resolve(res.data)
//     })
// }
// export function GetCrmWarehouse(){  //获取库房所属的crm库房
//   return axios.get(PFWMS_API+"/wmsapi/CrmWarehouse").then((res) => {
//       return Promise.resolve(res.data)
//     })
// }
// export function addhouse(data){  //新增库房与修改库房
//   return axios.post(PFWMS_API+"/wmsapi/Warehouse",data).then((res) => {
//       return Promise.resolve(res.data)
//     })
// }





export function UserLogin(name, password) { //登录接口ERM
  return axios.post(PFWMS_API + "/ErmAPI/admins/Login?userName=" + name + "&password=" + password).then((res) => {
    return Promise.resolve(res.data)
  })
}
//export function UserLogin(name, password) { //登录接口ERM
//  return axios.post( "http://warehouse0.ic360.cn/ErmAPI/admins/Login?userName=" + name + "&password=" + password).then((res) => {
//    return Promise.resolve(res.data)
//  })
//}


export function UserMenu() { //用户菜单接口ERM
  return axios.get(PFWMS_API + "/ErmAPI/admins/Roles?business=库房管理").then((res) => {
    return Promise.resolve(res.data)
  })
}
// export function UserWareHouseRoles(){  //用户拥有的客户权限ERM
//   return axios.get(PFWMS_API+"/ErmAPI/admins/WareHouseTreeRoles").then((res) => {
//       return Promise.resolve(res.data)
//     })
// }

export function UserWareHouseRoles() { //用户拥有的客户权限ERM 库法权限
  return axios.get(PFWMS_API + "/ErmAPI/admins/WareHouseRoles").then((res) => {
    return Promise.resolve(res.data)
  })
}


// 库区部分的接口
export function getArea(data) { //库区搜索与列表  货架搜索与列表
  return axios.get(PFWMS_API + "/api/Shelve", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

//入库部分
// 通知列表
// http://hv.warehouse.b1b.com/ApiWms/sortings?warehouseID=HK01&pageindex=1&pagesize=20
export function wareing(data) { //入库通知
  return axios.get(PFWMS_API + "/wmsapi/sortings", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

//入库通知详情
// http://hv.warehouse.b1b.com/ApiWms/sortings/detail?warehouseID=HK01&waybillid=Waybill201910120021
export function Noticedetail(data) { //入库通知详情
  return axios.get(PFWMS_API + "/wmsapi/sortings/detail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 入库通知inputid
export function GetInputID() { //入库通知详情
  return axios.get(PFWMS_API + "/wmsapi/sortings/newinput").then((res) => {
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


// //入库通知状态
// // http://hv.warehouse.b1b.com/WmsApi/enums/ExcuteStatus
// export function ExcuteStatus(){  //入库通知状态
//   return axios.get(PFWMS_API+"/wmsapi/enums/ExcuteStatus").then((res) => {
//       return Promise.resolve(res.data)
//     })
// }


//入库通知状态
// http://hv.warehouse.b1b.com/wmsapi/enums/SortingExcuteStatus
export function ExcuteStatus() { //入库通知状态
  return axios.get(PFWMS_API + "/wmsapi/enums/SortingExcuteStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}
//出库通知状态
// http://hv.warehouse.b1b.com/wmsapi/enums/PickingExcuteStatus
export function outStatus() { //入库通知状态
  return axios.get(PFWMS_API + "/wmsapi/enums/PickingExcuteStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}

//入库分拣完成
//http://dev.pfwms.com/api/storages
// http://hv.warehouse.b1b.com/WmsApi/sortings/Enter
export function sortingupload(data) { //入库完成接口
  return axios.post(PFWMS_API + "/wmsapi/sortings/Enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}



//通知详情的产品项查询 筛选
export function search_detail(warehouseID, waybillid, key) {
  return axios.get(PFWMS_API + "/wmsapi/sortings/detail?warehouseID=" + warehouseID + "&waybillid=" + waybillid + "&key=" + key).then((res) => {
    return Promise.resolve(res.data)
  })
}

//输送地
// others/origins
export function getWayParter() {
  return axios.get(PFWMS_API + "/wmsapi/others/origins").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 入库分拣可用库位号
// http://hv.warehouse.b1b.com//wmsapi/Shelve/GetUsableShelves?whid=HK01_WLT
export function GetUsableShelves(ID, ClientID) {
  return axios.get(PFWMS_API + "/wmsapi/Shelve/GetUsableShelves?whid=" + ID + "&ClientID=" + ClientID).then((res) => {
    return Promise.resolve(res.data)
  })
}



//承运商
// http://hv.warehouse.b1b.com/wmsapi/others/Carriers
export function Carriers() {
  return axios.get(PFWMS_API + "/wmsapi/others/Carriers").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 历史到货
// http://hv.warehouse.b1b.com/wmsapi/sortings/History?waybillid=Waybill201910120021
export function History(data) {
  return axios.get(PFWMS_API + "/wmsapi/sortings/History", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 历史到货详细列表
// http://hv.warehouse.b1b.com/wmsapi/sortings/Historydetail?waybillid=Waybill201910120021&orderid=Order201909290013
export function Historydetails(data) {
  return axios.get(PFWMS_API + "/wmsapi/sortings/Historydetail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 在库管理
// （1）库存查询
// http://hv.warehouse.b1b.com/wmsapi/storage?WareHouseID=hk01&type=800&PartNumber=&Catalog=&Manufacturer=&beginDate=&endDate=&pageIndex=1&pageSize=10
export function storages(data) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/storage", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
//  （2）暂存管理 
// /wmsapi/Waybill
// 暂存运单列表
export function SparateWaybill(data) { //暂存运单列表
  return axios.get(PFWMS_API + "/wmsapi/Waybill", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
//暂存运单详情
export function SparateDetails(data) { //暂存运单详情
  return axios.get(PFWMS_API + "/wmsapi/Waybill/Detail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 库位分配

// /wmsapi/lsnotice库位分配通知
export function lsnotice(data) { //库位分配通知 与通知搜索
  return axios.get(PFWMS_API + "/wmsapi/lsnotice", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 租赁状态
// /wmsapi/lsnotice/GetStatus
export function GetStatulsnotice() { //库位分配通知 与通知搜索
  return axios.get(PFWMS_API + "/wmsapi/lsnotice/GetStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 租赁状态详情
// /wmsapi/lsnotice/Detail
export function lsnoticeDetail(data) { //租赁状态详情
  return axios.get(PFWMS_API + "/wmsapi/lsnotice/Detail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 库位租赁修改
// /wmsapi/lsnotice/Enter
export function lsnoticeEnter(data) { //
  return axios.post(PFWMS_API + "/wmsapi/lsnotice/Enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 到货暂存部分
// TempStorage/Enter
export function TempStorage(data) { //
  return axios.post(PFWMS_API + "/wmsapi/TempStorage/Enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}



// 出库通知
// http://hv.warehouse.b1b.com/wmsapi/pickings?warehouseid=HK01
export function pickingsout(data) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/pickings", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库通知---详情分拣
// http://hv.warehouse.b1b.com/wmsapi/pickings/detail?warehouseid=HK01&waybillid=Waybill201910250032
export function pickingsdetail(data) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/pickings/detail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库完成
// http://hv.warehouse.b1b.com/wmsapi/pickings/enter
export function pickingsenter(data) {
  return axios.post(PFWMS_API + "/wmsapi/pickings/enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库客户确认单文件上传
// Pickings/UpdateFile 传值id,waybillID,customName,type
export function UpdateFile(id,waybillID,customName,type) {
  return axios.post(PFWMS_API + "/wmsapi/Pickings/UpdateFile?id="+id+"&waybillID="+waybillID+"&customName="+customName+"&type="+type, ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 出库客户确认单文件删除
// Pickings/DeleteFile post方式，传值id
export function DeleteFile(id) {
  return axios.post(PFWMS_API + "/wmsapi/Pickings/DeleteFile?id="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}
// http://hv.warehouse.b1b.com/wmsapi/pickings/UpdateWayBillCode
//将打印的运单号传递给后台
export function UpdateWayBillCode(data) {
  return axios.post(PFWMS_API + "/wmsapi/pickings/UpdateWayBillCode", data).then((res) => {
    return Promise.resolve(res.data)
  })
}
export function initwaybillenter(data) { //出库时保存运单信息
  return axios.post(PFWMS_API + "/wmsapi/initwaybill/enter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 保存子运单
// hv.warehouse.b1b.com/WmsApi/sortings/SubCodeEnter   
export function SubCodeEnter(data) {
  return axios.post(PFWMS_API + "/wmsapi/sortings/SubCodeEnter", data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 获取子运单
// http://hv.warehouse.b1b.com/WmsApi/sortings/subcode?waybillid=Waybill201910290011
export function getchildencode(id) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/sortings/subcode?waybillid=" + id).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 出库复核
// http://hv.warehouse.b1b.com/wmsapi/pickings/check  
export function TestCheck(ID) {
  return axios.post(PFWMS_API + "/wmsapi/pickings/check?outputID=" + ID, ).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 送货上门获取承运商
export function GetLocalCarriers() { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/others/GetLocalCarriers").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 送货上门根据承运商选择司机与车牌
// others/DriversCars
export function GetDriversCars(key) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/others/DriversCars?key=" + key).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 库房部分最新修改

// 一.库房
// /wmsapi/Shelve/GetWarehouse  对象：（Key：父亲ID(若传值则显示库房下的门牌库房，不传值显示大库房)
export function GetWarehouse(data) { //库存查询
  return axios.get(PFWMS_API + "/wmsapi/Shelve/GetWarehouse", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 一.库区 / 货架  /库位 列表获取
// /wmsapi/Shelve  对象：（Key：父亲ID(若传值则显示库房下的门牌库房，不传值显示大库房) PageIndex：当前页码 PageSize：每页记录数）
export function Shelvelist(data) {
  return axios.get(PFWMS_API + "/wmsapi/Shelve", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
export function GetPurposes(data) {
  return axios.get(PFWMS_API + "/wmsapi/Others/Purposes", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

export function AddShelve(data) {
  return axios.post(PFWMS_API + "/wmsapi/Shelve", data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取库位规格
export function GetSpecs() {
  return axios.get(PFWMS_API + "/wmsapi/Spec").then((res) => {
    return Promise.resolve(res.data)
  })
}

// /wmsapi/admins  //获取负责人接口
// 10	/wmsapi/admins	获取库区/货架/库位负责人	get	{ID:下拉框的value值,RealName:下拉框的显示值}	Json	name:下拉框的name值
export function GetAdmins(data) {
  return axios.get(PFWMS_API + "/wmsapi/admins?name=" + data).then((res) => {
    return Promise.resolve(res.data)
  })
}
// /wmsapi/Shelve/Delete  删除库区，库位，货架
export function ShelveDelete(data) {
  return axios.get(PFWMS_API + "/wmsapi/Shelve/Delete?shelveID=" + data).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 收支明细部分（收入，支出，提交）
// 收支列表
// http://hv.warehouse.b1b.com/wmsapi/Payments?waybillid=Waybill201910120020&type=in
// http://hv.warehouse.b1b.com/wmsapi/Payments?waybillid=Waybill201910120020&type=out
export function Paymentslist(data) {
  return axios.get(PFWMS_API + "/wmsapi/Payments", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

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
  return axios.get(PFWMS_API + "/wmsapi/payments/subject", {
    params: data
  }).then((res) => {
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


//出库运单信息
// http://localhost:8080/pfwms/wmsapi/initwaybill?waybillid=Waybill201911130001
export function initwaybill(id) {
  return axios.get(PFWMS_API + "/wmsapi/initwaybill?waybillid=" + id, ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 承运商（陈）
// http://hv.warehouse.b1b.com/wmsapi/Kdn/GetShipperCodes
export function GetShipperCodes() {
  return axios.get(PFWMS_API + "/wmsapi/Kdn/GetShipperCodes", ).then((res) => {
    return Promise.resolve(res.data)
  })
}
// http://hv.warehouse.b1b.com/wmsapi/Kdn/GetExpTypes?shipperCode=sf
export function GetExpTypes(id) {
  return axios.get(PFWMS_API + "/wmsapi/Kdn/GetExpTypes?shipperCode=" + id, ).then((res) => {
    return Promise.resolve(res.data)
  })
}


//我去提货
// hv.warehouse.b1b.com/wmsapi/Sortings/TakeGoods
export function TakeGoods(waybaillid) {
  return axios.post(PFWMS_API + "/wmsapi/Sortings/TakeGoods", waybaillid).then((res) => {
    return Promise.resolve(res.data)
  })
}

//列表展示或下拉选择箱号
// http://localhost:8080/pfwms/wmsapi/boxes?warehouseid=HK002&status=200
export function GetBoxes(id, status) {
  return axios.get(PFWMS_API + "/wmsapi/boxes?warehouseid=" + id + "&status=" + status, ).then((res) => {
    return Promise.resolve(res.data)
  })
}
//添加箱号时日期
// http://localhost:8080/pfwms/wmsapi/boxes/dates
export function GetBoxesdates() {
  return axios.get(PFWMS_API + "/wmsapi/boxes/dates").then((res) => {
    return Promise.resolve(res.data)
  })
}

// 生成箱号接口
// http://localhost:8080/pfwms/wmsapi/boxes/enter
export function Boxenter(Applydata) {
  return axios.post(PFWMS_API + "/wmsapi/boxes/enter", Applydata).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 报关运输
// 运输列表 （搜索）CustomTransport
export function CustomTransport(data) {
  return axios.get(PFWMS_API + "/wmsapi/CustomTransport", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 报关运输详情
// /wmsapi/CustomTransport/Detail
export function TransportDetail(data) {
  return axios.get(PFWMS_API + "/wmsapi/CustomTransport/Detail", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 出库按钮
// /wmsapi/CustomTransport/OutputEnter
export function OutputEnter(lotnumber) {
  return axios.post(PFWMS_API + "/wmsapi/CustomTransport/OutputEnter?lotnumber=" + lotnumber).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 报关运输的截单状态
// /wmsapi/Others/CuttingOrderStatus
export function CuttingOrderStatus() {
  return axios.get(PFWMS_API + "/wmsapi/Others/CuttingOrderStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}
// 申报窗口
// 获取数据
export function boxproducts(data) {
  return axios.get(PFWMS_API + "/wmsapi/boxes/boxproducts", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 获取箱号规格
// http://hv.warehouse.b1b.com/wmsapi/Enums/BoxingSpecs
export function BoxingSpecs(data) {
  return axios.get(PFWMS_API + "/wmsapi/Enums/BoxingSpecs", {
    params: data
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 申请报关按钮
// http://hv.warehouse.b1b.com/wmsapi/pickings/CustomsApply
export function CustomsApply(Applydata) {
  return axios.post(PFWMS_API + "/wmsapi/pickings/CustomsApply", Applydata).then((res) => {
    return Promise.resolve(res.data)
  })
}

// http://hv.warehouse.b1b.com/wmsapi/Boxes/ChangeBoxCode
export function ChangeBoxCode(Applydata) {
  return axios.post(PFWMS_API + "/wmsapi/Boxes/ChangeBoxCode", Applydata).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 深圳入库运输批次号详情
// http://hv.warehouse.b1b.com/wmsapi/CustomTransport/EnterDetail
export function GetCustomTransportDetail(warehouseID, lotNumber) {
  return axios.get(PFWMS_API + "/wmsapi/CustomTransport/EnterDetail?warehouseID=" + warehouseID + "&lotNumber=" + lotNumber).then((res) => {
    return Promise.resolve(res.data)
  })
}


// 深圳入库箱号上架
// http://hv.warehouse.b1b.com/wmsapi/CustomTransport/UpperShelf
export function UpperShelf(boxCodes, newShelveID) {
  return axios.post(PFWMS_API + "/wmsapi/CustomTransport/UpperShelf", {
    boxCodes: boxCodes,
    newShelveID: newShelveID
  }).then((res) => {
    return Promise.resolve(res.data)
  })
}
