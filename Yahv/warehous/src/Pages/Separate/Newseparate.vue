<style scoped>
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.box {
  width: 100%;
  min-height: 120px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  padding-left: 10px;
  padding-bottom: 10px;
}
.buttons {
  width: 100%;
  margin-top: 20px;
  text-align: center;
}
.setupload {
  /* width: 50px; */
  height: 30px;
  border: none;
  float: left;
  line-height: 1;
  margin-right: 5px;
}
.iconstyle:hover {
  cursor: pointer;
}
.bitian {
  color: red;
}
.setpadding {
  padding-top: 10px;
}
.linkurlcolor {
  color: #2d8cf0;
}
.Filesbox:hover {
  cursor: pointer;
}
.inputwidths {
  width: 60%;
}
.mastercontent {
  color: red;
  font-size: 12px;
  padding-left: 5px;
}
.libox li {
  display: inline-block;
  margin-right: 10px;
}
.imgbox {
  width: 160px;
  height: 160px;
  border: 1px solid #ccc;
  display: table-cell;
  vertical-align: middle;
  text-align: center;
}
.imgbox:hover {
  cursor: pointer;
}
.imgbox img {
  width: 130px;
  height: 130px;
}
.cancelicon {
  position: relative;
  font-size: 20px;
  color: red;
  top: 10px;
  /* right: -9px; */
  left: 150px;
}
.cancelicon:hover{
  cursor: pointer;
}
</style>
<template>
  <div>
    <div>
      <p class="detailtitle">基础信息</p>
      <div class="box">
        <Row class="setpadding">
          <Col :xs="2" :sm="4" :md="4" :lg="7">
          <p>
            <label>
              运&nbsp;&nbsp;单&nbsp;&nbsp;号：
              <Input v-model="Waybill.Code"
                     placeholder="请输入运单号"
                     class="inputwidths" />
            </label>
          </p>
          <p class="setpadding">
            <label>
              入&nbsp;&nbsp;仓&nbsp;&nbsp;号：
              <Input v-model="Waybill.EnterCode"
                     placeholder="请输入入仓号"
                     class="inputwidths" />
            </label>
          </p>
          <p style="width: 172%" class="setpadding">
            <label>
              备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：
              <Input style="width: 87%"
                     type="textarea"
                     v-model="Waybill.Summary"
                     :autosize="{ maxRows: 3, minRows: 3 }"
                     placeholder="请输入备注信息" />
            </label>
          </p>
          </Col>
          <Col :xs="2" :sm="16" :md="10" :lg="7">
          <div>
            <label>
              承&nbsp;&nbsp;&nbsp;运&nbsp;&nbsp;&nbsp;&nbsp;商：
            </label>
            <Select filterable
                    v-model="Waybill.CarrierID"
                    class="inputwidths">
              <Option v-for="(item, index) in CarrierList"
                      :value="item.ID"
                      :label="item.Name"
                      :key="index">
                <span>{{ item.Name }}</span>
              </Option>
            </Select>
          </div>
          <p class="setpadding">
            <label>
              供&nbsp;&nbsp;&nbsp;货&nbsp;&nbsp;&nbsp;&nbsp;商：
              <Input v-model="Waybill.Supplier"
                     placeholder="请输入供货商"
                     class="inputwidths" />
            </label>
          </p>
          </Col>
          <Col :xs="2" :sm="4" :md="4" :lg="7">
          <p class="setpadding">
            <label>
              <em class="bitian">*</em>暂&nbsp;存&nbsp;库&nbsp;位：
            </label>
            <Select v-model="Waybill.ShelveID"
                    style="width: 200px"
                    filterable>
              <Option v-for="(item, index) in ShelveArr"
                      :value="item.ShelveID"
                      :label="item.ShelveID"
                      :key="index">
                <span>{{ item.ShelveID }}</span>
              </Option>
            </Select>
          </p>
          <p class="setpadding" v-if="Waybill.TempDays!= null">
            <label> &nbsp;&nbsp;暂&nbsp;存&nbsp;天&nbsp;数： </label>
            {{ Waybill.TempDays }}
          </p>
          <p class="setpadding" v-if="Waybill.Status==2">
            <label> &nbsp;&nbsp;订&nbsp;&nbsp;&nbsp;单&nbsp;&nbsp;&nbsp;号： </label>
            {{ Waybill.ForOrderID }}
          </p>
          </Col>
          <!-- <Col :xs="2" :sm="4" :md="4" :lg="3">
          <div class="setupload">
            <Button
              type="primary"
              size="small"
              icon="ios-cloud-upload"
              @click="SeletUpload('Waybill')"
              >传照</Button
            >
          </div>
          <div class="setupload" style="float: rigth">
            <Button
              size="small"
              type="primary"
              icon="md-reverse-camera"
              @click="fromphotos('Waybill')"
              >拍照</Button
            >
          </div>
          <ul style="clear: both">
            <li v-for="item in Waybill.Files">
              <p class="Filesbox">
                <span
                  class="linkurlcolor"
                  @click="clackFilesProcess(item.Url)"
                  >{{ item.CustomName }}</span
                >
                <Icon
                  class="iconstyle"
                  type="md-trash"
                  @click="delimg(item)"
                />
              </p>
            </li>
          </ul>
        </Col> -->
        </Row>
      </div>
    </div>
    <div>
      <div style="padding-top: 30px">
        <p class="detailtitle">
          <em style="padding-right: 10px">图片列表</em>
          <Button type="primary"
                  size="small"
                  icon="ios-cloud-upload"
                  @click="SeletUpload('Waybill')"
                  :disabled="Waybill.Status==1?false:true">
            传照
          </Button>
          <Button size="small"
                  type="primary"
                  icon="md-reverse-camera"
                  @click="fromphotos('Waybill')"
                  :disabled="Waybill.Status==1?false:true">
            拍照
          </Button>
          <Button icon='md-calendar' type="success" size='small' @click="logchange">日志管理</Button>
        </p>
        <div style="padding-top: 20px">
          <ul class="libox" v-if="Waybill.Files.length > 0">
            <li v-for="(item, index) in Waybill.Files" :key="item.ID">
              <Icon v-if="Waybill.Status==1"
                    class="cancelicon"
                    type="md-close-circle"
                    @click="delimg(item)" />
              <div class="imgbox" @click="clackFilesProcess(item.Url)">
                <img :src="item.Url" alt="" />
              </div>
            </li>
          </ul>
          <div v-else style="text-align: center;">
            <img src="../../assets/img/null.png" alt="">
            <p>暂无数据</p>
          </div>
        </div>
        <div style="padding-top: 10px">
          <div class="buttons">
            <Button type="primary"
                    size="large"
                    style="margin-rigth: 5px"
                    @click="submitbtn"
                    :disabled="Waybill.Status==1?false:true">
              保存
            </Button>
            <Button size="large" @click="clear_btn">取消</Button>
          </div>
          <!-- <div class="buttons" v-else>
           <Button size="large" @click="clear_btn">关闭</Button>
        </div> -->
        </div>
      </div>
    </div>
    <!-- 操作日志 -->
    <Modal v-model="showlogged"
           :footer-hide='true'
           :mask-closable='false'
           width='60'>
      <div slot="close">
        <Icon style="font-size:21px;color:#cccccc;padding-top: 5px" type="ios-close-circle-outline" />
      </div>
      <div slot="header">
        <span style="font-size:18px;color:#1aaff7;">日志管理</span>
      </div>
      <logg-ed ref="logged" :key='loggdetime' v-if="showlogged" :WaybillID='Waybill.ID'></logg-ed>
    </Modal>
    <!-- <div class="dync mount"></div> -->
    <!-- 操作日志 -->
  </div>
</template>
<script>
import { getWayParter, Carriers, TempStorage } from "../../api";
import {
  GetUsableShelves,
  enterforpda,
  cgNewtempstocksDetail,
  CgDeleteFileall,
  CgAllsCarriers,
} from "../../api/CgApi";

  import { FormPhoto, SeletUploadFile, FilesProcess } from "@/js/browser.js";
  import logged from '../Common/logged'
  export default {
    components: {
      'logg-ed': logged,
    },
  data() {
    return {
      loggdetime: '',//操作日志时间
      showlogged: false,//操作日志
      TempDays: null, //暂存天数
      ID: "",
      isEdit: false, //判断是新录入还是修改
      myplace: "", //本地输送地
      Conveyingplace: [], //输送地列表
      Carrier: "", //承运商
      CarrierList: [], //承运商列表
      WareHouseID: sessionStorage.getItem("UserWareHouse"), //库房id

      ShelveID: "", //暂存库位
      ShelveArr: [], //暂存库位列表
      Code: "", //运单号
      WaybillType: "", //运单类型,
      CarrierID: "", //承运商ID,
      ConsignorID: "", //发货人,
      ExcuteStatus: 101, //执行状态,
      Place: "", //原产地
      phonenumber: "", //发件人电话
      ConsignorName: "", //发货人姓名：
      EnterCode: "", // 入仓号
      Summary: "", //备注
      Files: [], //文件
      Waybill: {
        ID: null,
        Code: null, //运单Code，既是快递的快递单号
        EnterCode: null, // 入仓号
        ClientName: null, //客户名称
        Supplier: null, //供应商名称
        Status: null, //Waybill运单的状态，
        StatusDes: null, //Waybill运单的状态，
        CarrierID: null, //承运商ID
        CarrierName: null, //承运商名字
        ForOrderID: null,
        CreateDate: null,
        Summary: null, // 运单的描述,备注
        Files: [], // 这个文件的具体格式我整理完后发你一份
        TempDays: null, // 暂存天数
        ShelveID: null,
        Status:null,//状态
      },

      value: "",
      cityList: [],
      model1: "",
      Delete: [], //删除数据的ID
      Delfiles:[]
    };
  },
  created() {
    // if (this.$route.name == "revise") {
    //   this.isEdit = true;
    //   this.ID = this.$route.query.detailID;
    //   console.log(this.$route);
    //   this.getdetail(this.$route.query.detailID);
    // }
    this.getdetail(this.$route.query.detailID);
    this.setnva();
    window["PhotoUploaded"] = this.changed;
    this.getWayParter();
    this.Carriers();
    this.GetUsableShelves();
  },
  mounted() {},
  methods: {
    getdetail(Waybill) {
      // alert("获取详情页数据");
      cgNewtempstocksDetail(Waybill).then((res) => {
        var waybill = {
          ID: res.Waybill.ID,
          Code: res.Waybill.Code, //运单Code，既是快递的快递单号
          EnterCode: res.Waybill.EnterCode, // 入仓号
          ClientName: res.Waybill.ClientName, //客户名称
          Supplier: res.Waybill.Supplier, //供应商名称
          Status: res.Waybill.Status, //Waybill运单的状态，
          StatusDes: res.Waybill.StatusDes, //Waybill运单的状态，
          CarrierID: res.Waybill.CarrierID, //承运商ID
          CarrierName: res.Waybill.CarrierName, //承运商名字
          ForOrderID: res.Waybill.ForOrderID,
          CreateDate: res.Waybill.CreateDate,
          Summary: res.Waybill.Summary, // 运单的描述,备注
          Files: res.Waybill.Files, // 这个文件的具体格式我整理完后发你一份
          TempDays: res.Waybill.TempDays, // 暂存天数
          ShelveID: res.Waybill.ShelveID,
        };
        this.Waybill = waybill;
      });
    },
    setnva() {
      var cc = [
        {
          title: "暂存录入",
          href: "/Separate/separateenter",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    handleCreate1(val) {
      this.CarrierList.push({
        ID: val,
        Name: val,
      });
    },
    getWayParter() {
      //获取输送地列表
      getWayParter().then((res) => {
        console.log(res);
        this.Conveyingplace = res.obj;
      });
    },
    Carriers() {
      //获取承运商列表
      CgAllsCarriers(this.WareHouseID).then((res) => {
        this.CarrierList = res;
      });
    },
    submitbtn() {
      if (this.Waybill.ShelveID == null || this.Waybill.Files.length <= 0) {
        this.$Message.error("库位与图片不能为空");
      } else {
        this.delimgall()
        var newFiles=[]
        this.Waybill.Files.forEach((item,index)=>{
          var aa={
            ID:item.ID
          }
          newFiles.push(aa)
        })
        var submitobj = {
          AdminID:sessionStorage.getItem("userID"),
          Waybill: {
            ID: this.Waybill.ID,
            Code: this.Waybill.Code,
            EnterCode:this.Waybill.EnterCode,
            CarrierID:this.Waybill.CarrierID,
            Summary:this.Waybill.Summary,
            ForOrderID:this.Waybill.ForOrderID,
            WareHouseID:sessionStorage.getItem("UserWareHouse"),
            ShelveID:this.Waybill.ShelveID,
            Supplier:this.Waybill.Supplier,
            Files:newFiles,
          },
        };
        enterforpda(submitobj).then(res=>{
          console.log(res)
          if(res.Success == true){
            this.$Message.success("暂存库录入成功");
            var _this = this;
            setTimeout(function () {
              _this.$store.dispatch("setSpearatedrawer", false);
            }, 1000);
          }else{
            this.$Message.error(res.Data);
          }
        })
      }
    },

    // 删除数组中的图片
    delimg(file) {
       this.$Message.success("删除成功");
       this.Waybill.Files.splice(this.Waybill.Files.indexOf(file), 1);
       this.Delfiles.push(file.ID)
    },

    // 执行删除数据接口
     delimgall() {
      var data={
        ID:this.Delfiles
      }
       CgDeleteFileall(data).then((res) => {
         
         });
    },
    fromphotos() {
      //拍照
      var data = {
        SessionID: "zancun",
        AdminID: sessionStorage.getItem("userID"),
      };
      FormPhoto(data);
    },
    SeletUpload(type) {
      // 传照
      if (type == "Waybill") {
        var data = {
          SessionID: this.waybillid,
          AdminID: sessionStorage.getItem("userID"),
        };
        SeletUploadFile(data);
      }
    },
    changed(message) {
      //后台调用winfrom 拍照的方法
      this.testfunction(message); //前台拿到返回值处理数据
    },
    testfunction(message) {
      //前台处理数据的方法
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        type: 8000,
      };
      this.Waybill.Files.push(newfile);
    },
    setphone() {
      if (this.Waybill.Phone != "") {
        var mPattern = /(^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$)/;
        var phones = mPattern.test(this.Waybill.Phone);
        console.log(phones);
        this.disabledphone = phones;
        if (phones == false) {
          this.$Message.error("请输入正确的手机号");
          this.Waybill.Phone = "";
        }
      }
    },
    clear_btn() {
      //取消到暂存录入
      // this.$route.name == "revise"
      // if (this.$route.name == "revise") {
      //   this.$store.dispatch("setSpearatedrawer", false);
      // } else {
      //   this.cleardata();
      // }
       this.$store.dispatch("setSpearatedrawer", false);
    },
    GetUsableShelves() {
      var id = sessionStorage.getItem("UserWareHouse");
      GetUsableShelves("HK").then((res) => {
        console.log(res);
        this.ShelveArr = res.obj;
      });
    },
    clackFilesProcess(url) {
      var data = {
        Url: url,
      };
      FilesProcess(data);
    },
    //操作日志的展示
    logchange() {
      this.showlogged = true
      this.loggdetime = new Date().getTime()
    },
  },
};
</script>
