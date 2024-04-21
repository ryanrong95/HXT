<style>
#TransportDetail .details_title {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
#TransportDetail .info_box {
  width: 100%;
  min-height: 200px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  /* text-indent: 20px; */
}
#TransportDetail .infoitem {
  min-height: 45px;
  line-height: 45px;
  font-size: 14px;
}
#TransportDetail .infoitem label {
  padding-right: 10px;
  padding-left: 20px;
}
#TransportDetail .RadioGroupbox {
  padding: 15px 0;
}
#TransportDetail .subCol ul li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  padding: 9px;
  border-bottom: 1px solid #ccc;
  overflow-x: hidden;
}
#TransportDetail .subCol ul li:last-child {
  border-bottom: none;
}
.frilebox {
  padding-top: 10px;
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
.editbtn :hover{
  cursor:pointer;
}
</style>
<template>
  <div class="TransportDetail" id="TransportDetail">
    <p class="details_title">基本信息</p>
    <div class="info_box">
      <Row v-if="detailinfo!=null">
        <Col span="7">
          <p class="infoitem">
            <label class>运输批次：</label>
            <span class>
              {{detailinfo.LotNumber}}
              <Button
                v-if="detailinfo.ID!=null"
                icon="md-calendar"
                type="success"
                size="small"
                @click="logchange"
              >日志管理</Button>
            </span>
          </p>

          <p class="infoitem">
            <label class>截单状态：</label>
            <span v-if="detailinfo.CuttingOrderStatus==0">等待</span>
            <span v-if="detailinfo.CuttingOrderStatus==1">已截单</span>
            <span v-if="detailinfo.CuttingOrderStatus==2">已完成</span>
          </p>


          <p class="infoitem">
            <label>封&nbsp;&nbsp;条&nbsp;&nbsp;号：</label>
            <span>
              <ButtonGroup v-if="IsSealNo==false">
                <Input
                  v-model.trim="SealNo"
                  placeholder="请输入封条号"
                  clearable
                  style="width:160px;float:left;position: relative;left: 3px"
                />
                <Button style="float:left" type="primary" @click="setSealNo">确定</Button>
              </ButtonGroup>
              <i v-else>
                <!-- {{SealNo}} -->
                <em v-if="SealNo!=''&&SealNo!=null">{{SealNo}}</em>
                <em v-else style="color:red;">暂无封条号</em>
                 <Tooltip class="editbtn" content="录入/修改封条号" placement="right-start" theme="light">
                    <Icon type="md-create"  @click="IsSealNo=false" />
                 </Tooltip>
                
              </i>
            </span>
          </p>
        </Col>
        <Col span="6">
          <p class="infoitem">
            <label class>承&nbsp;&nbsp;运&nbsp;&nbsp;商：</label>
            <span class>{{detailinfo.CarrierName}}</span>
          </p>
          <p class="infoitem">
            <label class>运输类型：</label>
            <span v-if="detailinfo.IsOnevehicle==true">包车</span>
            <span v-if="detailinfo.IsOnevehicle==false">普通</span>
          </p>
          <p class="infoitem">
            <label class>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机：</label>
            <span class>{{detailinfo.Driver}}</span>
          </p>
           <p class="infoitem">
            <label class>联系电话：</label>
            <span class>{{detailinfo.Phone}}</span>
          </p>
        <p class="infoitem">
            <label class>运输时间：</label>
            <span class>{{detailinfo.DepartDate|showDate}}</span>
          </p>     
        </Col>
        <Col span="6">
         <p class="infoitem">
            <label class>总&nbsp;&nbsp;数&nbsp;&nbsp;量：</label>
            <span class>{{detailinfo.TotalQuantity}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;条&nbsp;&nbsp;数：</label>
            <span class>{{detailinfo.TotalRecord}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;箱&nbsp;&nbsp;数：</label>
            <span class>{{detailinfo.TotalParts}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;金&nbsp;&nbsp;额：</label>
            <span class>{{detailinfo.Currency}}{{detailinfo.TotalPrice}}</span>
          </p>
          <p class="infoitem">
            <label class>总毛重(kg)：</label>
            <span class>{{detailinfo.TotalWeight}}</span>
          </p>
        </Col>
        <Col span="4">
          <div style="margin-top:8px;">
            <Button type="primary" icon="ios-cloud-upload" @click="SeletUpload('Waybill')">传照</Button>
            <Button type="primary" icon="md-reverse-camera" @click="fromphotos('Waybill')">拍照</Button>
            <!-- <p >文件列表：</p> -->
            <ul class="frilebox">
              <li v-for="item in detailinfo.Files">
                 <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                 <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
              </li>
            </ul>
          </div>
        </Col>
      </Row>
    </div>
    <p class="details_title">运输清单</p>
    <div class="tablebox"></div>
    <div>
      <div>
        <div class="RadioGroupbox">
          <div style="width:60%;float:left;padding-top:5px">
            <label>订单类型：</label>
            <RadioGroup v-model="disabledGroup" @on-change="changeradio" style="padding-right:20px">
              <Radio label="0">全部</Radio>
              <Radio label="1">内单</Radio>
              <Radio label="2">外单</Radio>
            </RadioGroup>
            <Button type="primary" icon="ios-print-outline"  :disabled="disableclick==1?true:false"  @click="printfile('thwt')">提货委托书</Button>
            <Button type="primary" icon="ios-print-outline"  :disabled="disableclick==2?true:false"  @click="printfile('hwlz')">货物流转确认书</Button>
          </div>
          <div v-if="detailinfo!=null" style="width:35%;float: right;text-align: right;padding-right:50px">
            <Button
              icon="md-checkmark"
              type="success"
              :disabled="disabledIs==true?true:false"
              @click="isconfirm=true"
            >完成出库</Button>
          </div>
        </div>
        <div style="clear: both;padding-top:15px">
          <Table :columns="titlelable" :data="Notices" :border="Notices.length > 0">
            <template slot-scope="{ row, index }" slot="total">{{row.BoxesNotices.length}}</template>
            <template slot-scope="{ row, index }" slot="boxcode">
              <div class="subCol">
                <ul>
                  <li v-for="item in row.PackDate">{{item.BoxCode|showboxcode}}</li>
                </ul>
              </div>
            </template>
          </Table>
          <div v-if="detailinfo!=null" style="text-align:center;padding:30px 0">
            <Button
              icon="md-checkmark"
              type="success"
              :disabled="disabledIs==true?true:false"
              @click="isconfirm=true"
            >完成出库</Button>
          </div>
        </div>
      </div>
    </div>
    <Modal v-model="isconfirm" title="提示" @on-ok="ok_confirm" @on-cancel="cancel">
      <p>是否确认出库</p>
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
      <logg-ed ref="logged" :key="loggdetime" v-if="showlogged" :WaybillID="detailinfo.ID"></logg-ed>
    </Modal>
    <!-- 操作日志 -->
  </div>
</template>
<script>
// import expandRow from "./table-expand.vue";
// import {TransportDetail,OutputEnter,CgDelcareShipDetail} from "../../api";
import { FormPhoto, SeletUploadFile, FilesProcess ,FilePrint,GetPrinterDictionary} from "@/js/browser.js";
import logged from "../Common/logged";
import { CgDelcareShipDetail, CgAutoHkExit,UpdateHKSealNumber,PrintExportFiles,CgDeleteFiles, } from "../../api/CgApi";
export default {
  components: {
    "logg-ed": logged
  },
  data() {
    return {
      disableclick:0,
      IsSealNo: true,
      SealNo: "", //封条号
      loggdetime: "", //操作日志时间
      showlogged: false, //操作日志
      isconfirm: false, //是否确认出库
      lotnumber: this.$route.params.id, //运输批次号
      houseid: sessionStorage.getItem("UserWareHouse"),
      detailinfo: null, //详情信息
      titlelable: [
        {
          type: "index",
          width: 60,
          align: "center"
        },
        {
          title: "订单号",
          key: "TinyOrderID"
        },
        {
          title: "客户编号",
          key: "EnterCode"
        },
        {
          title: "客户名称",
          key: "ClientName"
        },
        {
          title: "总条数",
          key: "Count",
          width: 100
        },
        {
          title: "箱号",
          slot: "boxcode",
          align: "center"
        },
        {
          title: "装箱时间",
          key: "list",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.Notices[params.index].PackDate.map(item => {
                    return h("li", {}, item.PackDate);
                  })
                )
              ]
            );
          }
        }
      ],
      Notices: [], //绑定数据
      AllData: [], //全部数据
      innerData: [], //内单数据
      externalData: [], //外单数据
      disabledGroup: "0", //默认选中的值
      disabledIs: true
    };
  },
  created() {
    window["PhotoUploaded"] = this.changed;
  },
  mounted() {
    this.getdetail();
  },
  methods: {
    getdetail() {
      //获取详情页面数据
      CgDelcareShipDetail(this.lotnumber).then(res => {
        if (res.CuttingOrderStatus == 1) {
          this.disabledIs = false;
        } else {
          this.disabledIs = true;
        }
        this.detailinfo = res;
        this.SealNo =res.HKSealNumber
        this.Notices = res.Notices;
        this.AllData = res.Notices;
        for (var i = 0, len = res.Notices.length; i < len; i++) {
          if (res.Notices[i].Source == 35) {
            this.innerData.push(res.Notices[i]);
          } else {
            this.externalData.push(res.Notices[i]);
          }
        }
      });
    },
    ok_confirm() {
      //确认出库
      this.disabledIs = true;
      var adminID = sessionStorage.getItem("userID");
      CgAutoHkExit(this.detailinfo.LotNumber, adminID).then(res => {
        if (res.success == true && res.code == 200) {
          this.$Message.success("出库完成");
          var _this = this;
          setTimeout(function() {
            _this.$store.dispatch("setTransportDetail", false);
          }, 1000);
        } else {
          this.$Message.error(res.data);
        }
        this.disabledIs = false;
      });
    },
    cancel() {
      //取消出库
      this.isconfirm = false;
    },
    changeradio(value) {
      if (value == "0") {
        this.Notices = this.AllData;
      } else if (value == "1") {
        this.Notices = this.innerData;
      } else {
        this.Notices = this.externalData;
      }
    },
    //操作日志的展示
    logchange() {
      this.showlogged = true;
      this.loggdetime = new Date().getTime();
    },
    SeletUpload(type) {
      // 传照
      var data = {
        SessionID: type.ID,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.detailinfo.ID,
          ShipID:this.detailinfo.LotNumber
        }
      };
      SeletUploadFile(data);
    },
    fromphotos(type) {
      var data = {
        SessionID: type.ID,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.detailinfo.ID,
          ShipID:this.detailinfo.LotNumber
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
    changed(message) {
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      this.detailinfo.Files.push(newfile)
    },
    handleRemove(file,type) {
       var data={
          id:file.ID
        }
       CgDeleteFiles(data).then(res=>{
          if(res.Success==true){
            // this.detailinfo.Files.splice(file, 1);
            this.detailinfo.Files.splice(this.detailinfo.Files.indexOf(file), 1);
            this.$Message.success('删除成功')
          }else{
            this.$Message.error('删除失败')
          }
      })
    },
    setSealNo() {
      if (this.SealNo != ""&&this.SealNo != null) {
          var data={
          WaybillID:this.detailinfo.ID,
          HKSealNumber:this.SealNo
          }
          UpdateHKSealNumber(data).then(res=>{
          if(res.success==true){
          this.$Message.success("封条号保存成功");
          this.IsSealNo = true;
          }else{
          this.$Message.error(res.data);
          this.IsSealNo = false;
          }
          })

          } else {
          this.$Message.warning("请输入封条号");
          }
          },
          printfile(file){
          var data={
          WaybillID:this.detailinfo.ID,
          FileName:file,
          TotalParts:this.detailinfo.TotalParts,
          }
          if(file=='thwt'){
          this.disableclick=1
          }else{
          this.disableclick=2
          }
          PrintExportFiles(data).then(res=>{
          if(res.success==true){
          this.fileprint(res.data)
          this.$Message.success("打印成功");
          }else{
          this.$Message.error(res.data);
          }
          })
          var that=this
          setTimeout(function(){
          that.disableclick=0;
          },500)
          },
          fileprint(printurl) {
          var configs = GetPrinterDictionary()
          var getsetting = configs['文档打印']
          getsetting.Url = printurl
          var data = getsetting
          FilePrint(data)
          // for(var i=0;i<3;i++){
      //    FilePrint(data)
      // }
    },
  }
};
</script>