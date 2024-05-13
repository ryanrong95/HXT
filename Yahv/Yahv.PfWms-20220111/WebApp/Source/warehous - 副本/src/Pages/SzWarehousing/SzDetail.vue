<style scoped>
.details_title {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.info_box {
  width: 100%;
  min-height: 200px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  padding: 0 15px;
}
.info_item {
  min-height: 45px;
  line-height: 45px;
  font-size: 14px;
}
.info_item label {
  width: 90px;
  display: inline-block;
}
.transport_detail .tablebox {
  padding-top: 20px;
}
.ul li {
  height: 48px;
  line-height: 48px;
}
.notice {
  margin-top: 20px;
  display: inline-block;
  vertical-align: middle;
  margin-left: 10px;
  color: #2d8cf0;
}
</style>
<template>
  <div class="transport_detail">
    <p class="details_title">基本信息</p>
    <div class="info_box">
      <Row>
        <Col span="12">
          <p class="info_item">
            <label class>运输批次号：</label>
            <span class>{{titleMsg.LotNumber}}</span>
          </p>
          <p class="info_item">
            <label class>车&nbsp;&nbsp;牌&nbsp;&nbsp;号：</label>
            <span class>{{titleMsg.CarNumber1}}&nbsp;&nbsp;{{titleMsg.CarNumber2}}</span>
          </p>
          <p class="info_item">
            <label class>运输时间：</label>
            <span class>{{titleMsg.DepartDate|showDate}}</span>
          </p>
          <p class="info_item">
            <label class>总&nbsp;&nbsp;件&nbsp;&nbsp;数：</label>
            <span class>{{titleMsg.totalBoxes}}</span>
          </p>
        </Col>
        <Col span="12">
          <p class="info_item">
            <label class>承&nbsp;&nbsp;运&nbsp;&nbsp;商：</label>
            <span class>{{titleMsg.CarrierName}}</span>
          </p>
          <p class="info_item">
            <label class>司机姓名：</label>
            <span class>{{titleMsg.Driver}}</span>
          </p>
          <p class="info_item">
            <label class>运输类型：</label>
            <span class>{{titleMsg.WaybillTypeDes}}</span>
          </p>
          <p class="info_item">
            <label class>总毛重(kg)：</label>
            <span class>{{titleMsg.TotalWeight}}</span>
          </p>
        </Col>
      </Row>
    </div>
    <p class="details_title">明细</p>
    <Button type="success" style="margin-top:20px;" @click="shelves">上架</Button>
    <span class="notice">注意：如果已上架，仍然可以点击上架修改库位号</span>
    <div class="tablebox">
      <Table
        ref="selection"
        :columns="listTitle"
        :data="listData"
        @on-select="onSelect"
        @on-select-all="onSelectAll"
        @on-selection-change="onSelectionChange"
      >
        <template slot-scope="{ row, index }" slot="BoxDate">
          <p>{{row.BoxDate|showDate}}</p>
        </template>
      </Table>
    </div>
    <Modal title="标题" v-model="modal" width="300">
      库位号：
      <!-- <Input v-model="shelveID" placeholder="请输入库位号" style="width: 200px" /> -->
      <Select v-model="shelveID" style="width: 200px">
        <Option v-for="item in shelveArr" :value="item.ID" :key="item.ID">{{item.ID}}</Option>
      </Select>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok">确定</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
import {
  GetUsableShelves,
  GetCustomTransportDetail,
  UpperShelf
} from '../../api'
export default {
  data() {
    return {
      titleMsg: {
        LotNumber: '', //运输批次号
        CarrierName: '', //承运商
        CarNumber1: '', //车牌号1
        CarNumber2: '', //车牌号2
        DepartDate: '', //运输时间
        Driver: '', //司机姓名
        WaybillTypeDes: '', //运输类型
        totalBoxes: '', //总件数
        TotalWeight: '' //总毛重(kg)
      },
      listData: [],
      listTitle: [
        {
          type: 'selection',
          width: 60,
          align: 'center'
        },
        {
          title: '订单编号',
          key: 'TinyOrderID'
        },
        {
          title: '客户编号',
          key: 'ClientID'
        },
        {
          title: '客户名称',
          key: 'ClientName'
        },
        {
          title: '装箱日期',
          slot: 'BoxDate'
        },
        {
          title: '箱号',
          key: 'BoxCode'
        },
        {
          title: '库位号',
          key: 'ShelveID'
        },
        {
          title: '状态',
          key: 'ShelvesStatus'
        }
      ],
      modal: false,
      selection: [],
      shelveID: '', //库位ID
      shelveArr: [] //库位号
    }
  },
  created() {
    this.titleMsg.LotNumber = this.$route.params.ID
    this.GetCustomTransportDetail()
  },
  methods:{
    // 获取运单批次号详情，以及赋值
    GetCustomTransportDetail() {
      let that = this
      var warehouseID = sessionStorage.getItem('UserWareHouse')
      var lotNumber = this.$route.params.ID
      GetCustomTransportDetail(warehouseID, lotNumber).then(res => {
        if (res && res.obj) {
          that.handleTitleMsg(res.obj)
          if (res.obj.SortingNotices && res.obj.SortingNotices.length) {
            that.handleListData(res.obj.SortingNotices)
          }
        }
      })
    },
    // 获取运单批次号详情
    GetCustomTransportDetailFun(cb) {
      let that = this
      var warehouseID = sessionStorage.getItem('UserWareHouse')
      var lotNumber = this.$route.params.ID
      GetCustomTransportDetail(warehouseID, lotNumber).then(res => {
        if (res && res.obj) {
          if (cb) {
            cb(res.obj)
          }
        }
      })
    },
    //处理title信息
    handleTitleMsg(data) {
      this.titleMsg.CarrierName = data.CarrierName
      this.titleMsg.CarNumber1 = data.CarNumber1
      this.titleMsg.CarNumber2 = data.CarNumber2
      this.titleMsg.DepartDate = data.DepartDate
      this.titleMsg.Driver = data.Driver
      this.titleMsg.WaybillTypeDes = data.WaybillTypeDes
      this.titleMsg.totalBoxes = data.TotalQuantity
      this.titleMsg.TotalWeight = data.TotalWeight
    },
    //处理列表信息
    handleListData(data) {
      this.listData = []
      for (let i = 0; i < data.length; i++) {
        const element = data[i]
        let listJson = {}
        listJson.BoxDate = element.BoxDate //装箱日期
        listJson.BoxCode = element.BoxCode //箱号
        if (element.Input) {
          listJson.TinyOrderID = element.Input.TinyOrderID //订单编号
          listJson.ClientID = element.Input.ClientID //客户编号
          listJson.ClientName = element.Input.ClientName //客户名称
        }
        if (element.Storage) {
          listJson.ShelveID = element.Storage.ShelveID //库位号
          listJson.ShelvesStatus = element.Storage.ShelvesStatus //上架状态
        }
        this.listData.push(listJson)
      }
    },
    //选择列表数据事件
    onSelect(selection, row) {
      this.selection = selection
    },
    //全选择列表数据事件
    onSelectAll(selection) {
      this.selection = selection
    },
    //更改列表数据事件
    onSelectionChange(selection) {
      this.selection = selection
    },
    //上架
    shelves() {
      let that = this
      if (this.selection.length) {
        if (this.shelveArr.length) {
          that.modal = true
        } else {
          this.getUsableShelves(function() {
            that.modal = true
          })
        }
      } else {
        this.$Message.warning('请选择您要上架的货物')
      }
    },
    // 获取库位号
    getUsableShelves(cb) {
      var id = sessionStorage.getItem('UserWareHouse')
      GetUsableShelves(id).then(res => {
        this.shelveArr = res.obj
        if (cb) {
          cb()
        }
      })
    },
    //确认上架
    ok() {
      let that = this
      if (!this.shelveID) {
        this.$Message.warning('请选择库位号')
      } else {
        this.UpperShelf(function(res){
          if(!res.val){
            that.$Message.success(res.msg)
            that.getListData()
          }else{
            that.$Message.error(res.msg)
          }
          that.shelveID = ''
          that.modal=false
          that.selection=[]
        })
      }
    },
    //取消上架
    cancel() {
      this.shelveID = ''
      this.modal=false
    },
    //请求上架接口
    UpperShelf(cb){
      let boxCodes=[]
      for (let i = 0; i < this.selection.length; i++) {
        boxCodes.push(this.selection[i].BoxCode)
      }
      UpperShelf(boxCodes,this.shelveID).then(res=>{
        if(cb){
          cb(res)
        }
      })
    },
    //获取列表数据(上架后重新刷新数据)
    getListData() {
      let that = this
      this.GetCustomTransportDetailFun(function(data){
        if (data.SortingNotices && data.SortingNotices.length) {
            that.handleListData(data.SortingNotices)
          }
      })
    }
  }
}
</script>