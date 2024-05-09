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
.success{
  display: inline-block;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background: #19be6b;
  vertical-align: middle;
}
.warning{
  display: inline-block;
  width:14px;
  height: 14px;
  border-radius: 50%;
  background: #f90;
  vertical-align: middle;
}
.printbtn{
    margin-top: 20px;
    float: right;
    margin-right: 50px;
}
.linkurlcolor{
  color: #2d8cf0;
}
.frilebox li {
  line-height: 25px;
}
.frilebox li :hover{
  cursor:pointer;
}
</style>
<template>
  <div class="transport_detail">
    <p class="details_title">基本信息</p>
    <div class="info_box">
      <Row>
        <Col span="9">
          <p class="info_item">
            <label class>运输批次号：</label>
            <span class>{{titleMsg.LotNumber}}
              <Button
                v-if="titleMsg.LotNumber!=null"
                icon="md-calendar"
                type="success"
                size="small"
                @click="logchange"
              >日志管理</Button>
            </span>
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
          <p class="info_item">
            <label class>已上架数：</label>
            <span class>{{titleMsg.ShelveQuantity}}</span>
          </p>
        </Col>
        <Col span="9">
          <p class="info_item">
            <label class>承&nbsp;&nbsp;运&nbsp;&nbsp;商：</label>
            <span class>{{titleMsg.CarrierName}}</span>
          </p>
          <!-- <p class="info_item">
            <label class>运&nbsp;&nbsp;单&nbsp;&nbsp;号：</label>
            <span class>{{titleMsg.Code}}</span>
          </p> -->
          <p class="info_item">
            <label class>司机姓名：</label>
            <span class>{{titleMsg.Driver}}</span>
          </p>
          <p class="info_item">
            <label class>运输类型：</label>
            <span class>{{titleMsg.Type}}</span>
          </p>
          <p class="info_item">
            <label class>总毛重(kg)：</label>
            <span class>{{titleMsg.TotalWeight}}</span>
          </p>
          <p class="info_item">
            <label class>总条数：</label>
            <span class>{{titleMsg.TotalParts}}</span>
          </p>
        </Col>
        <Col span="6">
          <div style="margin-top:8px;">
            <Button type="primary" icon="ios-cloud-upload" @click="SeletUpload('Waybill')">传照</Button>
            <Button type="primary" icon="md-reverse-camera" @click="fromphotos('Waybill')">拍照</Button>
            <!-- <p >文件列表：</p> -->
            <ul class="frilebox">
              <li v-for="item in titleMsg.Files">
                 <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                 <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
              </li>
            </ul>
          </div>
        </Col>
      </Row>
    </div>
    <p class="details_title">明细</p>
    <Button type="success" style="margin-top:20px;" @click="shelves">上架</Button>
    <span class="notice">注意：如果已上架，仍然可以点击上架修改库位号</span>
    <span><Button type="primary" class="printbtn" @click="ok_print">入库单打印</Button></span>
    <div class="tablebox">
      <Table
        :loading="loading"
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
         <template slot-scope="{ row, index }" slot="BoxCode">
          <p>{{row.BoxCode|showboxcode}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="ShelvesStatus">
          <p>
           <!-- <span>{{row.ShelvesStatus}}</span>
           <span class="success" v-if="row.ShelvesStatus=='已上架'"></span>
           <span class="warning" v-else></span> -->
           <Tag color="green" v-if="row.ShelvesStatus=='已上架'">{{row.ShelvesStatus}}</Tag>
           <Tag color="orange" v-else>{{row.ShelvesStatus}}</Tag>
          </p>
        </template>
      </Table>
    </div>
    <Modal title="标题" v-model="modal" width="300" @on-visible-change='visiblechange'>
      库位号：
      <!-- <Input v-model="shelveID" placeholder="请输入库位号" style="width: 200px" /> -->
      <Select v-model="shelveID" style="width: 200px"  filterable allow-create @on-create="createShelve">
        <Option
          v-for="(item,index) in shelveArr"
          :value="item.ShelveID"
          :key="index"
        >{{item.ShelveID}}</Option>
      </Select>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok">确定</Button>
      </div>
    </Modal>
    <!-- 操作日志 -->
    <Modal v-model="showlogged" :footer-hide="true" :mask-closable="false" width="60">
      <div slot="close">
        <Icon
          style="font-size:21px;color:#cccccc;padding-top: 5px"
          type="ios-close-circle-outline"
        />
      </div>
      <div slot="header">
        <span style="font-size:18px;color:#1aaff7;">日志管理</span>
      </div>
      <logg-ed ref="logged" :key="loggdetime" v-if="showlogged" :WaybillID="titleMsg.WaybillID"></logg-ed>
    </Modal>
    <!-- 操作日志 -->
  </div>
</template>
<script>
import logged from "../Common/logged";
import { GetCustomTransportDetail, UpperShelf } from "../../api";
import { Cgszsortingsdetail, GetUsableShelves,cgszsortingsupdate,Cgszsdetailprint,cgSZShelves,cgSZEnter,CgDeleteFiles} from "../../api/CgApi";
import {PrintInputList,FormPhoto, SeletUploadFile, FilesProcess} from '@/js/browser.js'
export default {
  components: {
    "logg-ed": logged
  },
  data() {
    return {
      AdminID:null,
      loggdetime: "", //操作日志时间
      showlogged: false, //操作日志
      loading:true,
      ID: null,
      titleMsg: {
        WaybillID:"",//运单ID
        LotNumber: "", //运输批次号
        CarrierName: "", //承运商
        CarNumber1: "", //车牌号1
        CarNumber2: "", //车牌号2
        DepartDate: "", //运输时间
        Driver: "", //司机姓名
        Code: "", //运单号
        Type: "", //运输类型
        totalBoxes: "", //总件数
        ShelveQuantity: "", //已上架数
        TotalWeight: "", //总毛重(kg)
        TotalParts:'',//总件数
        Files:[],
      },
      listData: [],
      listTitle: [
        {
          type: "selection",
          width: 60,
          align: "center"
        },
        {
          title: "订单编号",
          key: "TinyOrderID",
          width:180
        },
        {
          title: "客户名称",
          key: "ClientName"
        },
        {
          title: "装箱日期",
          slot: "BoxDate"
        },
        {
          title: "箱号",
          slot: "BoxCode"
        },
        // {
        //   title: "数量",
        //   key: "Total",
        //   width:80
        // },
        // {
        //   title: "型号",
        //   key: "PartNumber"
        // },
        // {
        //   title: "品牌",
        //   key: "Manufacturer"
        // },
        // {
        //   title: "原产地",
        //   key: "Origin"
        // },
        {
          title: "库位号",
          key: "ShelveID"
        },
        {
          title: "状态",
          slot: "ShelvesStatus"
        }
      ],
      modal: false,
      selection: [],
      shelveID: "", //库位ID
      shelveArr: [] //库位号
    };
  },
  created() {
    console.log(this.$route.query.status)
     window["PhotoUploaded"] = this.changed;
    this.ID = this.$route.params.ID;
    this.Cgszsortingsdetail();
    this.AdminID=sessionStorage.getItem("userID")
  },
  methods: {
    //重构 获取详情数据
    Cgszsortingsdetail() {
      Cgszsortingsdetail(this.ID).then(res => {
        this.loading=false;
        this.handleTitleMsg(res.Waybill);
        this.handleListData(res.notices);
      });
    },
    // 获取运单批次号详情
    // GetCustomTransportDetailFun(cb) {
    //   let that = this;
    //   var warehouseID = sessionStorage.getItem("UserWareHouse");
    //   var lotNumber = this.$route.params.ID;
    //   GetCustomTransportDetail(warehouseID, lotNumber).then(res => {
    //     if (res && res.obj) {
    //       if (cb) {
    //         cb(res.obj);
    //       }
    //     }
    //   });
    // },
    //处理title信息
    handleTitleMsg(data) {
      this.titleMsg.WaybillID=data.ID
      this.titleMsg.LotNumber = data.LotNumber;
      this.titleMsg.CarrierName = data.CarrierName;
      this.titleMsg.CarNumber1 = data.CarNumber1;
      this.titleMsg.CarNumber2 = data.CarNumber2;
      this.titleMsg.DepartDate = data.DepartDate;
      this.titleMsg.Driver = data.Driver;
      this.titleMsg.Code = data.Code;
      this.titleMsg.Type = data.Type;
      this.titleMsg.totalBoxes = data.TotalParts //总箱数
      this.titleMsg.ShelveQuantity = data.ShelveQuantity;//已上架数
      this.titleMsg.TotalWeight = data.TotalWeight;
      this.titleMsg.TotalParts=data.TotalQuantity ;  //总条数
      this.titleMsg.Files=data.Files
    },
    //处理列表信息
    handleListData(data) {
      this.listData = [];
      for (let i = 0; i < data.length; i++) {
        const element = data[i];
        let listJson = {};
        listJson.WaybillID=element.WaybillID;
        listJson.BoxDate = this.titleMsg.DepartDate; //装箱日期
        listJson.BoxCode = element.BoxCode; //箱号
        listJson.TinyOrderID = element.TinyOrderID; //订单编号
        listJson.ClientName = element.Name; //客户名称
        listJson.ShelveID = element.ShelveID; //库位号
        listJson.ShelvesStatus = element.status; //上架状态
        listJson.StorageID=element.StorageID
        this.listData.push(listJson);
      }
    },
    //选择列表数据事件
    onSelect(selection, row) {
      this.selection = selection;
    },
    //全选择列表数据事件
    onSelectAll(selection) {
      this.selection = selection;
    },
    //更改列表数据事件
    onSelectionChange(selection) {
      this.selection = selection;
    },
    //上架
    shelves() {
      let that = this;
      if (this.selection.length) {
        if (this.shelveArr.length) {
          that.modal = true;
        } else {
          this.getUsableShelves();
          that.modal = true;
        }
      } else {
        this.$Message.error("请选择您要上架的货物");
      }
    },
    // 获取库位号
    getUsableShelves() {
      var id = sessionStorage.getItem("UserWareHouse");
      cgSZShelves(id).then(res=>{
         this.shelveArr = res;
      })
    },
    //确认上架
    ok() {
      let that = this;
      if (!this.shelveID) {
        this.$Message.warning("请选择库位号");
      } else {
       var  UpdateArr=[];
        for(var i=0,len=this.selection.length;i<len;i++){
          var data={
              AdminID:sessionStorage.getItem("userID"),
              ShelveID:this.shelveID,
              WaybillID:this.selection[i].WaybillID,
              BoxCode:this.selection[i].BoxCode
          }
           UpdateArr.push(data)
        }
        var data = {
          Waybill: {
            WaybillID: this.titleMsg.WaybillID, // 运单ID
            LotNumber:this.titleMsg.LotNumber // 运输批次号
          },
          Update: UpdateArr
        };
        cgszsortingsupdate(data).then(res=>{
          console.log(res)
          if(res.Success==true){
              this.modal = false;
              this.$Message.success('上架成功')
              this.Cgszsortingsdetail()
              this.shelveID=''
              this.selection=[]
          }else{
             this.$Message.error('上架失败')
          }
        })
      }
    },
    //取消上架
    cancel() {
      this.shelveID = "";
      this.modal = false;
    },
    ok_print(){
      Cgszsdetailprint(this.ID,this.AdminID).then(res=>{
        PrintInputList(res)
      })
    },
    // 创建库位
    createShelve(val){
      // this.shelveID=val
       this.shelveArr.push({ ShelveID:val});
       var data={
         whCode:sessionStorage.getItem("UserWareHouse"), // 库位号
         place:val, //库位
       }
       cgSZEnter(data).then(res=>{
          this.getUsableShelves()
       })
    },
    visiblechange(val){
      if(val==false){
        this.shelveID="";
      }
    },
    SeletUpload(type) {
      // 传照
      var data = {
        SessionID: type.ID,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.titleMsg.WaybillID,
          ShipID:this.titleMsg.LotNumber
        }
      };

      SeletUploadFile(data);
    },
    fromphotos(type) {
      var data = {
        SessionID: type.ID,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.titleMsg.WaybillID,
          ShipID:this.titleMsg.LotNumber
        }
      };
      FormPhoto(data);
    },
    clackFilesProcess(url) {
      var data = {
        Url: url
      };
      FilesProcess(data);
    },
     handleRemove(file) {
       var data={
          id:file.ID
        }
       CgDeleteFiles(data).then(res=>{
          if(res.Success==true){
            // this.titleMsg.Files.splice(file, 1);
            this.titleMsg.Files.splice(this.titleMsg.Files.indexOf(file), 1);
            this.$Message.success('删除成功')
          }else{
            this.$Message.error('删除失败')
          }
      })
    },
    changed(message) {
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      // this.titleMsg.Files.push(newfile)
      this.titleMsg.Files.push(newfile)
    },
    //操作日志的展示
    logchange() {
      this.showlogged = true;
      this.loggdetime = new Date().getTime();
    },
  }
};
</script>