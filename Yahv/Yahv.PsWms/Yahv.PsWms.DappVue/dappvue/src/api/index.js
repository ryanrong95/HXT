import {
    PFWMS_API,
    ERM_API
  } from "@/main" //调用整体的url
  import axios from 'axios'


  // // 登录
  // export function UserLogin(name, password) { //登录接口ERM
  //   return axios.post(PFWMS_API + "/ermapi/admins/Login?userName=" + name + "&password=" + password).then((res) => {
  //     return Promise.resolve(res.data)
  //   })
  // }

  // export function UserLogin(name, password) { //登录接口ERM 测试版
  //   return axios.post("http://warehouse0.ic360.cn/ErmAPI/admins/Login?userName=" + name + "&password=" + password).then((res) => {
  //     return Promise.resolve(res.data)
  //   })
  // }

  export function UserLogin(name, password) { //登录接口ERM 生产版
    return axios.post("http://sz.warehouse.for-ic.net:60077/ErmAPI/admins/Login?userName=" + name + "&password=" + password).then((res) => {
      return Promise.resolve(res.data)
    })
  }


  // export function UserLogin(name, password) { //登录接口ERM 测试版
  //   return axios.post(ERM_API+"/ErmAPI/admins/Login?userName=" + name + "&password=" + password).then((res) => {
  //     return Promise.resolve(res.data)
  //   })
  // }

//费用录入提交 /pswms/DappApi/NoticeCharges/Enter
export function NoticeChargesEnter(data) { 
  return axios.post(PFWMS_API + "/DappApi/NoticeCharges/Enter",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 费用删除  ../pswms/DappApi/NoticeCharges/Delete?id=
export function NoticeChargesDelete(ID) { 
  return axios.post(PFWMS_API + "/DappApi/NoticeCharges/Delete?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}


//获取承运商  http://sz.warehouse.b1b.com/dappapi/Enums/Express
export function getExpress(data) { 
  return axios.get(PFWMS_API + "/dappapi/Enums/Express").then((res) => {
    return Promise.resolve(res.data)
  })
}


//获取司机列表 http://sz.warehouse.b1b.com/dappapi/Takers/List
export function TakersList(data) { 
  return axios.get(PFWMS_API + "/dappapi/Takers/List",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 文件部分
// 1 获取文件 http://sz.warehouse.b1b.com/dappapi/NoticeFiles/show?id=noticeItem136
export function GetNoticeFiles(ID) { 
  return axios.get(PFWMS_API + "/dappapi/NoticeFiles/show?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 2 获取单据，外观拍照等图片文件 http://sz.warehouse.b1b.com/dappapi/PhotoFiles/show?id=noticeItem136
export function GetPhotoFiles(ID) { 
  return axios.get(PFWMS_API + "/dappapi/PhotoFiles/show?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}
// 3 3.单据，外观拍照等图片文件(根据给的Type返回对应类型的图片文件) http://sz.warehouse.b1b.com/dappapi/PhotoFiles/show
export function GetPhototype(data) { 
  return axios.post(PFWMS_API + "/dappapi/PhotoFiles/show",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 删除图片 http://sz.warehouse.b1b.com/dappapi/Files/Delete?ID=File202101150001
export function PhotoFileDelete(ID) {
  return axios.post(PFWMS_API + "/dappapi/Files/Delete?ID="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5 .通知项中型号对应的文件或图片 http://sz.warehouse.b1b.com/dappapi/PartnumberFiles/show?id=noticeItem134
export function PartnumberFiles(ID) { 
  return axios.get(PFWMS_API + "/dappapi/PartnumberFiles/show?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}