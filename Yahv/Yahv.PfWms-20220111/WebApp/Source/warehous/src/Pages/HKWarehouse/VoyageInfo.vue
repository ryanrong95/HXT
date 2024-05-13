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

  .linkurlcolor {
    color: #2d8cf0;
  }

  .frilebox li {
    line-height: 25px;
  }

    .frilebox li :hover {
      cursor: pointer;
    }

  .editbtn :hover {
    cursor: pointer;
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
            {{detailinfo.VoyageNo}}          
          </span>
        </p>

        <p class="infoitem">
          <label class>截单状态：</label>
          <span v-if="detailinfo.CutStatus==0">等待</span>
          <span v-if="detailinfo.CutStatus==1">已截单</span>
          <span v-if="detailinfo.CutStatus==2">已完成</span>
        </p>


        <p class="infoitem">
          <label>封&nbsp;&nbsp;条&nbsp;&nbsp;号：</label>
          <span>
            <ButtonGroup v-if="IsSealNo==false">
              <Input v-model.trim="SealNo"
                     placeholder="请输入封条号"
                     clearable
                     style="width:160px;float:left;position: relative;left: 3px" />
              <Button style="float:left" type="primary" @click="setSealNo">确定</Button>
            </ButtonGroup>
            <i v-else>
              <!-- {{SealNo}} -->
              <em v-if="SealNo!=''&&SealNo!=null">{{SealNo}}</em>
              <em v-else style="color:red;">暂无封条号</em>
              <Tooltip class="editbtn" content="录入/修改封条号" placement="right-start" theme="light">
                <Icon type="md-create" @click="IsSealNo=false" />
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
          <span>{{detailinfo.VoyageTypeName}}</span>        
        </p>
        <p class="infoitem">
          <label class>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机：</label>
          <span class>{{detailinfo.DriverName}}</span>
        </p>
        <p class="infoitem">
          <label class>联系电话：</label>
          <span class>{{detailinfo.DriverPhone}}</span>
        </p>
        <p class="infoitem">
          <label class>运输时间：</label>
          <span class>{{detailinfo.TransportTime|showDate}}</span>
        </p>
        </Col>
        <Col span="6">
        <p class="infoitem">
          <label class>总&nbsp;&nbsp;数&nbsp;&nbsp;量：</label>
          <span class>{{detailinfo.TotalQuantity}}</span>
        </p>
        <p class="infoitem">
          <label class>总&nbsp;&nbsp;条&nbsp;&nbsp;数：</label>
          <span class>{{detailinfo.TotalItems}}</span>
        </p>
        <p class="infoitem">
          <label class>总&nbsp;&nbsp;箱&nbsp;&nbsp;数：</label>
          <span class>{{detailinfo.TotalPackNo}}</span>
        </p>
        <p class="infoitem">
          <label class>总&nbsp;&nbsp;金&nbsp;&nbsp;额：</label>
          <span class>{{detailinfo.Currency}}{{detailinfo.TotalAmount}}</span>
        </p>
        <p class="infoitem">
          <label class>总毛重(kg)：</label>
          <span class>{{detailinfo.TotalGrossWt}}</span>
        </p>
        </Col>
        <Col span="4">
        <div style="margin-top:8px;">
          <Button type="primary" icon="ios-cloud-upload" @click="SeletUpload('Waybill')">传照</Button>
          <Button type="primary" icon="md-reverse-camera" @click="fromphotos('Waybill')">拍照</Button>         
          <!-- <p >文件列表：</p> -->
          <ul class="frilebox">
            <li v-for="item in detailinfo.VoyageFiles">
              <span class="linkurlcolor" @click="clackFilesProcess(item.FileUrl)">{{item.FileName}}</span>
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
            <Button type="primary" icon="ios-print-outline" :disabled="disableclick==1?true:false" @click="printfile('thwt')">提货委托书</Button>
            <Button type="primary" icon="ios-print-outline" :disabled="disableclick==2?true:false" @click="printfile('hwlz')">货物流转确认书</Button> 
            <Button type="primary" v-if="detailinfo.CutStatus==1" @click="confirm()">完成</Button>           
          </div>
        </div>
        <div style="clear: both;padding-top:15px">
          <Table :columns="titlelable" :data="Notices" :border="Notices.length > 0">           
            <template slot-scope="{ row, index }" slot="boxcode">
              <div class="subCol">
                <ul>
                  <li v-for="item in row.BoxInfo">{{item.BoxIndex}}</li>
                </ul>
              </div>
            </template>           
          </Table>
        </div>
      </div>
    </div>
    
   
  </div>
</template>
<script>
import { FormPhoto, SeletUploadFile, FilesProcess ,FilePrint,GetPrinterDictionary} from "@/js/browser.js";
import { CgDelcareShipDetail,UpdateHKSealNumber,CgDeleteFiles, } from "../../api/CgApi";
import {VoyageInfo,VoyageDetail,UpdateHKSealNo,PrintExportFiles,UpdateVoyaeStatus} from "../../api/XdtApi";
export default {
  components: {
    "name":"VoyageInfo",
  },
  data() {
    return {
      disableclick:0,
      IsSealNo: true,
      SealNo: "", //封条号               
      lotnumber: this.$route.params.voyageNo, //运输批次号
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
          key: "OrderID"
        },
        {
          title: "客户编号",
          key: "ClientCode"
        },
        {
          title: "客户名称",
          key: "ClientName"
        },
        {
          title: "总条数",
          key: "ItemCount",
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
                    this.Notices[params.index].BoxInfo.map(item => {
                      return h("li", {}, item.PackingDate);
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
     
    };
  },
  created() {
    window["PhotoUploaded"] = this.changed;
  },
  mounted() {
    this.getdetail();
    this.getItemInfo();
  },
  methods: {
    getdetail() {
      //获取详情页面数据
      VoyageInfo(this.lotnumber).then(res => {      
        this.detailinfo = res.data;
        this.SealNo =res.data.SealNo;
      });
    },

    getItemInfo(){
       var data={
          voyageNo:this.lotnumber,
          PageIndex : 1,
          PageSize : 200
       };
       VoyageDetail(data).then(res => {           
        this.Notices = res.obj.rows;
        this.AllData = res.obj.rows;

        for (var i = 0, len = res.obj.rows.length; i < len; i++) {
          if (this.Notices[i].ClientType == 2) {            
            this.externalData.push(this.Notices[i]);
          } else {
            this.innerData.push(this.Notices[i]);
          }
        }
      });
    },
    
    cancel() {
      
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
           
    },
    SeletUpload(type) {
      // 传照
      var data = {
        SessionID: type,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.detailinfo.ID,
          ShipID:this.detailinfo.VoyageNo
        }
      };
      SeletUploadFile(data);     
    },
    fromphotos(type) {
      var data = {
        SessionID: type,
        AdminID: sessionStorage.getItem("userID"),
        Data: {
          WayBillID: this.detailinfo.ID,
          ShipID:this.detailinfo.VoyageNo
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
        FileName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      this.detailinfo.VoyageFiles.push(newfile)
    },
    handleRemove(file) {
       var data={
          id:file.ID
        }
       CgDeleteFiles(data).then(res=>{
          if(res.Success==true){
            // this.detailinfo.Files.splice(file, 1);
            this.detailinfo.VoyageFiles.splice(this.detailinfo.VoyageFiles.indexOf(file), 1);
            this.$Message.success('删除成功')
          }else{
            this.$Message.error('删除失败')
          }
      })
    },
    setSealNo() {
      if (this.SealNo != ""&&this.SealNo != null) {
          var data={
            voyageNo:this.lotnumber,
            sealNo:this.SealNo
          }
          UpdateHKSealNo(data).then(res=>{
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
            voyageNo:this.detailinfo.VoyageNo,
            fileType:file,
            totalPackNo:this.detailinfo.TotalPackNo,
            totalWeight:this.detailinfo.TotalGrossWt,
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
    }, 
    confirm(){
          var data={
            voyageNo:this.lotnumber,
            packNo:this.detailinfo.TotalPackNo
          }
          UpdateVoyaeStatus(data).then(res=>{
            if(res.success==true){
              this.$Message.success("确认成功");            
            }else{
              this.$Message.error(res.data);
            }
          })
    }
  }
};
</script>
