import {
    PFWMS_API
  } from "@/main" //调用整体的url
  import axios from 'axios'

//   1 库存记录 http://sz.warehouse.b1b.com/DappApi/Storages/Show
 export function Storagelist(data) { //
    return axios.post(PFWMS_API + "/DappApi/Storages/Show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

//   2 库位管理 http://sz.warehouse.b1b.com/DappApi/Shelves/Show
export function ShelveList(data) { //
    return axios.post(PFWMS_API + "/DappApi/Shelves/Show",data).then((res) => {
      return Promise.resolve(res.data)
    })
  }

// 3 新增库位  URL：http://sz.warehouse.b1b.com/DappApi/Shelves/Enter
export function ShelveEnter(data) { //
  return axios.post(PFWMS_API + "/DappApi/Shelves/Enter",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 4 删除库位 URL：http://sz.warehouse.b1b.com/DappApi/Shelves/Delete
export function ShelveDelete(data) { //
  return axios.post(PFWMS_API + "/DappApi/Shelves/Delete",data).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 5库位详情 URL: http://sz.warehouse.b1b.com/DappApi/Shelves/Detail?id=Shelve202101110002
export function ShelveDetail(ID) { //
  return axios.get(PFWMS_API + "/DappApi/Shelves/Detail?id="+ID).then((res) => {
    return Promise.resolve(res.data)
  })
}

// 6拆分库位 URL: http://sz.warehouse.b1b.com/DappApi/Storages/Inventory
export function SplitStorage(data) { //
    return axios.post(PFWMS_API + "/DappApi/Storages/Inventory",data).then((res) => {
        return Promise.resolve(res.data)
    })
}