import {
  XDT_API
} from "@/main" //调用整体的url
import axios from 'axios'
import { type } from "jquery"

//入库通知状态
// http://api0.for-ic.net/HKWarehouseNew/GetEntryNoticeStatus
export function EntryNoticeStatus() { //入库通知状态
  return axios.get(XDT_API + "/HKWarehouseNew/GetEntryNoticeStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}

//待装箱列表数据
// http://api0.for-ic.net/HKWarehouseNew/UnSortingList
export function UnSortingData(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/UnSortingList",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//已封箱列表数据
// http://api0.for-ic.net/HKWarehouseNew/SealedList
export function SealedList(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/SealedList",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//香港提货方式
  //http://api0.for-ic.net/HKWarehouseNew/GetHKDeliveryType
export function HKDeliveryType() { 
  return axios.get(XDT_API + "/HKWarehouseNew/GetHKDeliveryType").then((res) => {
    return Promise.resolve(res.data)
  })
}

//代报关装箱基础信息
//http://api0.for-ic.net/HKWarehouseNew/SortingBasicInfo
export function SortingBasicInfo(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/SortingBasicInfo?entryNoticeID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//计算仓储天数
//http://api0.for-ic.net/HKWarehouseNew/StockingDays
export function StockedDays(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/StockingDays?orderID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//待装箱产品数据
//http://api0.for-ic.net/HKWarehouseNew/LoadNoticeItems
export function LoadNoticeItems(id,keys) { 
  return axios.get(XDT_API + "/HKWarehouseNew/LoadNoticeItems?entryNoticeID="+id+"&keys="+keys).then((res) => {
    return Promise.resolve(res.data)
  })
}

//待装箱产品数据
//http://api0.for-ic.net/HKWarehouseNew/LoadNoticeItems
export function GetOrigin() { 
  return axios.get(XDT_API + "/HKWarehouseNew/GetOrigin").then((res) => {
    return Promise.resolve(res.data)
  })
}

//修改产地
//http://api0.for-ic.net/HKWarehouseNew/ChangeOrigin
export function ChangeOrigin(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeOrigin",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//修改型号
//http://api0.for-ic.net/HKWarehouseNew/ChangeProductModel
export function ChangeProductModel(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeProductModel",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//修改品牌
//http://api0.for-ic.net/HKWarehouseNew/ChangeManufacturer
export function ChangeManufacturer(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeManufacturer",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//修改批次号
//http://api0.for-ic.net/HKWarehouseNew/ChangeBatch
export function ChangeBatch(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeBatch",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//拆分型号
//http://api0.for-ic.net/HKWarehouseNew/SplitModelData
export function SplitModelData(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/SplitModelData",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//无通知产品录入
//http://api0.for-ic.net/HKWarehouseNew/UnExpectedGoods
export function UnExpectedGoods(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/UnExpectedGoods",data).then((res) => {
    return Promise.resolve(res.data)
  })
}


//重量预测
//http://api0.for-ic.net/HKWarehouseNew/WeightEstimate
export function WeightEstimate(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/WeightEstimate",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//国际快递信息
//http://api0.for-ic.net/HKWarehouseNew/GetExpressInfo
export function GetExpressInfo() { 
  return axios.get(XDT_API + "/HKWarehouseNew/GetExpressInfo").then((res) => {
    return Promise.resolve(res.data)
  })
}

//订单所涉及的国际快递
//http://api0.for-ic.net/HKWarehouseNew/OrderWaybillInfo
export function OrderWaybillInfo(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/OrderWaybillInfo?orderID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//验证箱号是否可用
//http://api0.for-ic.net/HKWarehouseNew/BoxIndexValidate
export function BoxIndexValidate(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/BoxIndexValidate",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//装箱
//http://api0.for-ic.net/HKWarehouseNew/PackingBoxIndex
export function PackingBoxIndex(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/PackingBoxIndex",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//已装箱产品数据
//http://api0.for-ic.net/HKWarehouseNew/LoadPackedProduct
export function LoadPackedProduct(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/LoadPackedProduct?orderID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//删除装箱
//http://api0.for-ic.net/HKWarehouseNew/DeletePacking
export function DeletePacking(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/DeletePacking",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//封箱
//http://api0.for-ic.net/HKWarehouseNew/Sealed
export function Sealed(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/Sealed",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//取消封箱
//http://api0.for-ic.net/HKWarehouseNew/CancelSealed
export function CancelSealed(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/CancelSealed",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//运输批次状态
//http://api0.for-ic.net/HKWarehouseNew/GetCutStatus
export function GetCutStatus() { 
  return axios.get(XDT_API + "/HKWarehouseNew/GetCutStatus").then((res) => {
    return Promise.resolve(res.data)
  })
}

//运输批次
//http://api0.for-ic.net/HKWarehouseNew/ManifestVoyageList
export function ManifestVoyageList(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ManifestVoyageList",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//承运商信息
//http://api0.for-ic.net/HKWarehouseNew/CarrierInteLogisticsList
export function CarrierInteLogisticsList() { 
  return axios.get(XDT_API + "/HKWarehouseNew/CarrierInteLogisticsList").then((res) => {
    return Promise.resolve(res.data)
  })
}

//报关运输基本信息数据
//http://api0.for-ic.net/HKWarehouseNew/VoyageInfo
export function VoyageInfo(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/VoyageInfo?voyageNo="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//运输批次单号详情
//http://api0.for-ic.net/HKWarehouseNew/VoyageDetail
export function VoyageDetail(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/VoyageDetail ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//更改封条号码
//http://api0.for-ic.net/HKWarehouseNew/UpdateHKSealNo
export function UpdateHKSealNo(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/UpdateHKSealNo ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取专门仓储费科目
export function Storagechargejson() {
  return axios.get(XDT_API + '/Content/jsons/HKWarehouse/Storagecharge.json').then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取代报关收费科目
export function DeclarationJson() {
  return axios.get(XDT_API + '/Content/jsons/HKWarehouse/Declaration.json').then((res) => {
    return Promise.resolve(res.data)
  })
}

//获取代报关收费科目
export function GetCarrierType() {
  return axios.get(XDT_API + '/Content/jsons/HKWarehouse/Carrier.json').then((res) => {
    return Promise.resolve(res.data)
  })
}

//库房费用 插入华芯通
export function InsertHKWarehouseFee(data) {
  return axios.post(XDT_API + '/Finance/InsertHKWarehouseFeeNew',data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//保存香港收获信息
//http://api0.for-ic.net/HKWarehouseNew/HKWaybillUpdate
export function HKWaybillUpdate(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/HKWaybillUpdate ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//计算件数
//http://api0.for-ic.net/HKWarehouseNew/totalPackNo
export function totalPackNo(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/totalPackNo ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//无通知产品数据
//http://api0.for-ic.net/HKWarehouseNew/UnExceptedList
export function LoadUnExcepted(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/UnExceptedList?orderID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//删除无通知产品录入
//http://api0.for-ic.net/HKWarehouseNew/UnExceptedList
export function DeleteUnExceptedList(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/DeleteUnExceptedList?ID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//生成提货委托书、货物流转确认单
//http://api0.for-ic.net/HKWarehouseNew/PrintExportFiles
export function PrintExportFiles(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/PrintExportFiles ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//更改提货状态
//http://api0.for-ic.net/HKWarehouseNew/DeliveryNoticesUpdate
export function DeliveryNoticesUpdate(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/DeliveryNoticesUpdate ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//更改箱号
//http://api0.for-ic.net/HKWarehouseNew/ChangeBoxIndex
export function ChangeBoxIndex(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeBoxIndex ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//更改无通知录入产品箱号
//http://api0.for-ic.net/HKWarehouseNew/ChangeUnExpectedBoxIndex
export function ChangeUnExpectedBoxIndex(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/ChangeUnExpectedBoxIndex ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//判断敏感区域是否和一般区域装在一个箱子里
//http://api0.for-ic.net/HKWarehouseNew/SensitiveArea
export function SensitiveArea(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/SensitiveArea ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//有未处理的无通知录入产品，不能封箱
//http://api0.for-ic.net/HKWarehouseNew/CanSeal
export function CanSeal(id) { 
  return axios.get(XDT_API + "/HKWarehouseNew/CanSeal?orderID="+id).then((res) => {
    return Promise.resolve(res.data)
  })
}

//香港操作日志
//http://api0.for-ic.net/HKWarehouseNew/HKOperationLog
export function HKOperationLog(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/HKOperationLog ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

//香港确认 运输批次完成
//http://api0.for-ic.net/HKWarehouseNew/UpdateVoyaeStatus
export function UpdateVoyaeStatus(data) { 
  return axios.post(XDT_API + "/HKWarehouseNew/UpdateVoyaeStatus ",data).then((res) => {
    return Promise.resolve(res.data)
  })
}