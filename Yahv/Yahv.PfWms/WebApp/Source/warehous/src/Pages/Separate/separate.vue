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
.linkurlcolor{
  color: #2d8cf0;
}
.Filesbox:hover{
  cursor: pointer;
}
.inputwidths{
  width:60%;
}
.mastercontent{
    color: red;
    font-size: 12px;
    padding-left: 5px;
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
                <Input v-model="Waybill.Code" placeholder="请输入运单号" class="inputwidths" />
              </label>
            </p>
            <p class="setpadding">
              <label>
                入&nbsp;&nbsp;仓&nbsp;&nbsp;号：
                <Input v-model="Waybill.EnterCode" placeholder="请输入入仓号" class="inputwidths" />
              </label>
            </p>
            <p class="setpadding">
              <label>
                订&nbsp;&nbsp;单&nbsp;&nbsp;号：
                <Input v-model="Waybill.OrderID" placeholder="请输入订单号" class="inputwidths" />
              </label>
            </p>
            <p style="width: 172%;" class="setpadding">
              <label>
                备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：
                <Input
                  style="width:87%;"
                  type="textarea"
                  v-model="Waybill.Summary"
                  :autosize="{maxRows: 3,minRows: 3}"
                  placeholder="请输入备注信息"
                />
              </label>
            </p>
          </Col>
          <Col :xs="2" :sm="16" :md="10" :lg="7">
            <div>
              <label>
                承&nbsp;&nbsp;&nbsp;运&nbsp;&nbsp;&nbsp;&nbsp;商：
              </label>
              <Select filterable v-model="Waybill.CarrierID" class="inputwidths">
                  <Option
                    v-for="(item,index) in CarrierList"
                    :value="item.ID"
                    :label="item.Name"
                    :key="index"
                  >
                    <span>{{item.Name}}</span>
                  </Option>
                </Select>
            </div>
            <p class="setpadding">
              <label>
                发&nbsp;&nbsp;&nbsp;货&nbsp;&nbsp;&nbsp;&nbsp;人：
                <Input v-model="Waybill.Contact" placeholder="请输入发货人姓名" class="inputwidths" />
              </label>
            </p>
            <p class="setpadding">
              <label>
                供&nbsp;&nbsp;&nbsp;货&nbsp;&nbsp;&nbsp;&nbsp;商：
                <Input v-model="Waybill.Supplier" placeholder="请输入供货商" class="inputwidths" />
              </label>
            </p>
          </Col>
          <Col :xs="2" :sm="4" :md="4" :lg="7">
            <div>
              <label>输&nbsp;&nbsp;&nbsp;送&nbsp;&nbsp;&nbsp;&nbsp;地：</label>
              <Select v-model="Waybill.ConsignorPlace" style="width:200px" filterable clearable>
                <Option
                  v-for="item in Conveyingplace"
                  :value="item.CorPlace"
                  :key="item.ID"
                >{{item.Text}}</Option>
              </Select>
            </div>
            <p class="setpadding">
              <label>
                发货人电话：
                <Input
                  v-model="Waybill.Phone"
                  @on-blur="setphone"
                  type="tel"
                  placeholder="请输入发货人电话"
                  style="width: 200px"
                />
              </label>
            </p>
             <p class="setpadding">
              <label>
                <em class="bitian">*</em>暂&nbsp;存&nbsp;库&nbsp;位：
              </label>
               <Select v-model="Waybill.ShelveID" style="width:200px" filterable>
                  <Option
                    v-for="(item,index) in ShelveArr"
                    :value="item.ShelveID"
                    :label="item.ShelveID"
                    :key="index"
                  >
                    <span>{{item.ShelveID}}</span>
                  </Option>
                </Select>
            </p>
            <p class="setpadding" v-if="TempDays!=null">
              <label>
                &nbsp;&nbsp;暂&nbsp;存&nbsp;天&nbsp;数：
              </label>
              {{TempDays}}
            </p>
          </Col>
          <Col :xs="2" :sm="4" :md="4" :lg="3">
            <div class="setupload">
              <!-- <img-test type="webaill" v-on:changitem="changeimgs($event,'webaill')"></img-test> -->
              <Button
                type="primary"
                size="small"
                icon="ios-cloud-upload"
                @click="SeletUpload('Waybill')"
              >传照</Button>
            </div>
            <div class="setupload" style="float:rigth">
              <Button size="small" type="primary" icon="md-reverse-camera" @click="fromphotos('Waybill')">拍照</Button>
            </div>
            <ul style="clear:both;">
              <li v-for="item in Waybill.Files">
                <p class="Filesbox">
                  <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <Icon class="iconstyle" type="md-trash" @click="delimg(item)" />
                </p>
              </li>
            </ul>
          </Col>
        </Row>
      </div>
    </div>
    <div>
      <div style="padding-top:10px;">
        <p class="detailtitle">描述信息<em class="mastercontent">（ 如果录入描述性信息，则描述必填 ）</em></p>
        <div style="padding-top:10px;">
          <Table :columns="Summaryarr" :data="Summarydata">
            <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
            <template slot-scope="{ row, index }" slot="action">
              <Icon
                @click="addSummary(index)"
                type="ios-add-circle-outline"
                style="margin-right:5px;font-size:20px;font-width:600"
              />
              <Icon
                @click="delrow(index,'Summary',row)"
                type="ios-trash-outline"
                style="font-size:20px;font-width:600"
              />
            </template>
          </Table>
        </div>
      </div>
      <div style="padding-top:30px;">
        <p class="detailtitle">产品信息<em class="mastercontent">（ 如果有录入型号，则数量与原产地必填 ）</em></p>
        <div style="padding-top:10px;">
          <Table :columns="columns7" :data="Storages">
            <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
            <template slot-scope="{ row, index }" slot="action">
              <Icon
                @click="addrow(index)"
                type="ios-add-circle-outline"
                style="margin-right:5px;font-size:20px;font-width:600"
              />
              <Icon
                @click="delrow(index,'product',row)"
                type="ios-trash-outline"
                style="font-size:20px;font-width:600"
              />
            </template>
            <template slot-scope="{ row, index }" slot="Place">
              <Select
                v-model="row.Origin"
                :transfer="true"
                filterable
                clearable
              >
                <Option
                  v-for="(item,index) in Conveyingplace"
                  :value="item.CorPlace"
                  :key="index"
                >{{item.Text}}</Option>
              </Select>
            </template>
          </Table>
          <div class="buttons">
            <Button
              type="primary"
              size="large"
              style="margin-rigth:5px"
              @click="submitbtn"
              v-if="isEdit==false"
            >保存</Button>
            <Button
              type="primary"
              size="large"
              style="margin-rigth:5px"
              @click="submitbtn"
              v-else
            >修改</Button>
            <Button size="large" @click="clear_btn">取消</Button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
import { getWayParter, Carriers, TempStorage } from "../../api";
  import {
    GetUsableShelves, enterforpda, cgNewtempstocksDetail,CgDeleteFiles,CgAllsCarriers} from "../../api/CgApi";

import { FormPhoto, SeletUploadFile,FilesProcess} from "@/js/browser.js";
import imgtest from "@/Pages/Common/imgtes";
export default {
  components: {
    "img-test": imgtest
  },
  data() {
    return {
      TempDays:null,//暂存天数
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
        ID: "", //WaybillID,
        Code: "", //运单号
        EnterCode: "", // 入仓号
        Type: 3,
        OrderID: "",
        CarrierID: "",
        ConsignorID: "",
        ConsigneeID: "", // 收货人ID
        ConsignorPlace: "", //发货人
        Summary: "",
        WarehouseID: sessionStorage.getItem("UserWareHouse"),
        ShelveID: "",
        Phone: "", //发货人Phone,
        FreightPayer: null, //运费付费方,
        Contact: "", //发货人
        Supplier: "",
        TempEnterCode: "",
        Files: []
      },

      value: "",
      cityList: [ ],
      model1: "",
      columns7: [
        {
          title: "",
          slot: "indexs",
          width: 30,
          align: "center"
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center"
        },
        {
          renderHeader: (h, params) => {
              return h("div", [
                h(
                  "span",
                  {
                    style: {
                      color: "red",
                      "vertical-align": "middle",
                      "padding-right": "5px",
                      "font-size": "20px;"
                    }
                  },
                  "*"
                ),
                h("span", {}, "型号")
              ]);
          },
          key: "PartNumber",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Product.PartNumber
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Product.PartNumber = event.target.value;
                  vm.Storages[params.index] = params.row;
                }
              }
            });
          }
        },
        {
          title: "品牌",
          key: "Manufacturer",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Product.Manufacturer
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Product.Manufacturer = event.target.value;
                  vm.Storages[params.index] = params.row;
                }
              }
            });
          }
        },
        {
          title: "批号",
          key: "DateCode",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.DateCode
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.DateCode = event.target.value;
                  vm.Storages[params.index] = params.row;
                }
              }
            });
          }
        },
        {
          renderHeader: (h, params) => {
              return h("div", [
                h(
                  "span",
                  {
                    style: {
                      color: "red",
                      "vertical-align": "middle",
                      "padding-right": "5px",
                      "font-size": "20px;"
                    }
                  },
                  "*"
                ),
                h("span", {}, "数量")
              ]);
          },
          key: "Quantity",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Quantity
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Quantity = event.target.value;
                  vm.Storages[params.index] = params.row;
                },
                "on-blur"(event) {
                  // var reg = /^[0-9]*$/;
                  var reg = /^\d+(\.\d{0,2})?$/;
                  if (reg.test(event.target.value) == true) {
                    params.row.Quantity = event.target.value;
                    vm.Storages[params.index] = params.row;
                  } else {
                    vm.$Message.error("只能输入数量,且保留小数点后两位");
                    (event.target.value = ""), (params.row.Quantity = "");
                    vm.Storages[params.index] = params.row;
                  }
                }
              }
            });
          }
        },
        {
          renderHeader: (h, params) => {
              return h("div", [
                h(
                  "span",
                  {
                    style: {
                      color: "red",
                      "vertical-align": "middle",
                      "padding-right": "5px",
                      "font-size": "20px;"
                    }
                  },
                  "*"
                ),
                h("span", {}, "原产地")
              ]);
          },
          slot: "Place",
          maxWidth:180
        }
      ],
      Storages: [
        {
          SortingID: "",
          Type: "",
          StorageID: "",
          InputID: "",
          Origin: "",
          ProductID: "",
          Product: {
            PartNumber: "",
            Manufacturer: "",
            PackageCase: "",
            Packing: ""
          },
          DateCode: "",
          CreateDate: "",
          ShelveID: "",
          WareHouseID: sessionStorage.getItem("UserWareHouse"),
          Total: "",
          Quantity: "",
          Supplier: "",
          Summary: ""
        }
      ],
      Storages2: [
        {
          SortingID: "",
          Type: "",
          StorageID: "",
          InputID: "",
          Origin: "",
          ProductID: "",
          Product: {
            PartNumber: "",
            Manufacturer: "",
            PackageCase: "",
            Packing: ""
          },
          DateCode: "",
          CreateDate: "",
          ShelveID: "",
          WareHouseID: sessionStorage.getItem("UserWareHouse"),
          Total: "",
          Quantity: "",
          Supplier: "",
          Summary: ""
        }
      ],
      Summaryarr: [
        {
          title: "",
          slot: "indexs",
          width: 30,
          align: "center"
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center"
        },
        {
          renderHeader: (h, params) => {
              return h("div", [
                h(
                  "span",
                  {
                    style: {
                      color: "red",
                      "vertical-align": "middle",
                      "padding-right": "5px",
                      "font-size": "20px;"
                    }
                  },
                  "*"
                ),
                h("span", {}, "描述")
              ]);
          },
          key: "Summary",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Summary
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Summary = event.target.value;
                  vm.Summarydata[params.index] = params.row;
                }
              }
            });
          }
        }
      ],
      Summarydata: [
        {
          SortingID: "",
          Type: "",
          StorageID: "",
          InputID: "",
          Origin: "",
          ProductID: "",
          Product: null, // 默认Quantity:0, ProductID: ""
          DateCode: "",
          CreateDate: "",
          ShelveID: "",
          WareHouseID: sessionStorage.getItem("UserWareHouse"),
          Total: "",
          Quantity: "",
          Supplier: "",
          Summary: ""
        }
      ],
      Summarydata2: [
        {
          SortingID: "",
          Type: "",
          StorageID: "",
          InputID: "",
          Origin: "",
          ProductID: "",
          Product: null, // 默认Quantity:0, ProductID: ""
          DateCode: "",
          CreateDate: "",
          ShelveID: "",
          WareHouseID: sessionStorage.getItem("UserWareHouse"),
          Total: "",
          Quantity: "",
          Supplier: "",
          Summary: ""
        }
      ],
      Delete:[],//删除数据的ID
    };
  },
  created() {
    if (this.$route.name == "revise") {
      this.isEdit = true;
      this.ID = this.$route.query.detailID;
      console.log(this.$route);
      this.getdetail(this.$route.query.detailID);
    }
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
      cgNewtempstocksDetail(Waybill).then(res => {
        var ShelveIDcode = "";
        if(res.ProductStorages.length == 0){
          this.Storages=this.Storages2
        } else{
         this.Storages = res.ProductStorages;
          ShelveIDcode = this.Storages[0].ShelveID;
        }
        if(res.SummaryStorages.length==0){
          this.Summarydata=this.Summarydata2
        }else{
         this.Summarydata = res.SummaryStorages;
         ShelveIDcode = this.Summarydata[0].ShelveID;
        }
        var waybill = {
          ID: res.Waybill.ID, //WaybillID,
          Code: res.Waybill.Code, //运单号
          EnterCode: res.Waybill.EnterCode, // 入仓号
          Type: res.Waybill.Type,
          OrderID: res.Waybill.OrderID,
          CarrierID: res.Waybill.CarrierID,
          ConsignorID: res.Waybill.Consignor.ID,
          ConsigneeID: res.Waybill.ConsigneeID, // 收货人ID
          ConsignorPlace: res.Waybill.Consignor.Place, //发货人
          Summary: res.Waybill.Summary,
          WarehouseID: sessionStorage.getItem("UserWareHouse"),
          ShelveID: ShelveIDcode,
          Phone: res.Waybill.Consignor.Phone, //发货人Phone,
          FreightPayer: null, //运费付费方,
          Contact: res.Waybill.Consignor.Contact, //发货人
          Supplier: res.Waybill.Supplier,
          TempEnterCode: res.Waybill.TempEnterCode,
          Files: res.Waybill.Files
        };
        this.Waybill = waybill;
        this.TempDays=res.Waybill.TempDays
      });
    },
    setnva() {
      var cc = [
        {
          title: "暂存录入",
          href: "/Separate/separateenter"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    handleCreate1(val) {
      this.CarrierList.push({
        ID: val,
        Name: val
      });
    },
    addrow(index) {
      //添加一行空数据
      console.log(index);
      var newrow = {
        SortingID: "",
        Type: "",
        StorageID: "",
        InputID: "",
        Origin: "",
        ProductID: "",
        Product: {
          PartNumber: "",
          Manufacturer: "",
          PackageCase: "",
          Packing: ""
        },
        DateCode: "",
        CreateDate: "",
        ShelveID: "",
        WareHouseID: sessionStorage.getItem("UserWareHouse"),
        Total: "",
        Quantity: "",
        Supplier: "",
        Summary: ""
      };
      this.Storages.splice(index + 1, 0, newrow);
    },
    addSummary(index) {
      var newSummary = {
        SortingID: "",
        Type: "",
        StorageID: "",
        InputID: "",
        Origin: "",
        ProductID: "",
        Product: null, // 默认Quantity:0, ProductID: ""
        DateCode: "",
        CreateDate: "",
        ShelveID: "",
        WareHouseID: sessionStorage.getItem("UserWareHouse"),
        Total: "",
        Quantity: "",
        Supplier: "",
        Summary: ""
      };
      this.Summarydata.splice(index + 1, 0, newSummary);
    },
    delrow(index, type,row) {
      //删除所选数据
      if (type == "Summary") {
        console.log(this.Summarydata.length);
        if (this.Summarydata.length > 1) {
          if(row.SortingID!=''&&row.StorageID!=''){
            var data={
              SortingID:row.SortingID,
              StorageID:row.StorageID
            }
            this.Delete.push(data)
          }
          this.Summarydata.splice(index, 1);
        } else {
        }
      } else {
        if (this.Storages.length > 1) {
          if(row.SortingID!=''&&row.StorageID!=''){
            var data={
              SortingID:row.SortingID,
              StorageID:row.StorageID
            }
            this.Delete.push(data)
          }
          this.Storages.splice(index, 1);
        } else {
        }
      }
    },
    getWayParter() {
      //获取输送地列表
      getWayParter().then(res => {
        console.log(res);
        this.Conveyingplace = res.obj;
      });
    },
    Carriers() {
      //获取承运商列表
      CgAllsCarriers(this.WareHouseID).then(res => {
        this.CarrierList = res;
      });
    },
    submitbtn() {
      if (this.Waybill.ShelveID == "") {
        this.$Message.error("请输暂存库位");
      } else {
        var Summarytrue = ""; //备注
        var Storagestrue = ""; //产品
        var NewSummary = []; //备注
        var NewStorages = []; //产品
        var ShelveID= this.Waybill.ShelveID
        // if (this.Summarydata.length == 1 && this.Summarydata[0].Summary == "") {
        //   Summarytrue = false;
        // } else {
        //   Summarytrue = true;
        //   if (this.Summarydata.length >= 1) {
        //     NewSummary = this.Summarydata.filter(function (element, index, self) {
        //        element.ShelveID =ShelveID;
        //   　　 return element.Summary!='';
        // 　　});
        //   }
        // }

        if (this.Summarydata.length >= 1) {
          for (var i = 0; i < this.Summarydata.length; i++) {
            if (this.Summarydata[i].Summary != "") {
              this.Summarydata[i].ShelveID=ShelveID
               NewSummary.push(this.Summarydata[i]);
            } else {
            }
         }
          }
        if(this.Storages.length>=1){
          for (var i = 0; i < this.Storages.length; i++) {
            if (this.Storages[i].Product.PartNumber != "") {
              this.Storages[i].ShelveID=ShelveID
              NewStorages.push(this.Storages[i]);
            } else {
              
            }
         }
        }
        
        if (NewStorages.length ==0&&NewSummary.length==0) {
          this.$Message.error("请录入描述信息或者产品信息,产品信息请录入必填项");
        }else{
          if(NewStorages.length>0){
            for(var i=0;i<NewStorages.length;i++){
              if(NewStorages[i].Product.PartNumber!=""&&NewStorages[i].Quantity!=""&&NewStorages[i].Origin!=""){
                if(i==NewStorages.length-1){
                   var data = {
                    AdminID: sessionStorage.getItem("userID"),
                    Waybill: this.Waybill,
                    SummaryStorages: NewSummary,
                    ProductStorages: NewStorages,
                     Delete:this.Delete
                  };
                  this.cgtempstocks(data)
                  console.log(data)
                }
              }else{
                this.$Message.error("型号数量原产地为必填");
                break;
              }
            }
          }else{
             var data = {
              AdminID: sessionStorage.getItem("userID"),
              Waybill: this.Waybill,
              SummaryStorages: NewSummary,
              ProductStorages: NewStorages,
              Delete:this.Delete
            };
              console.log(data)
            this.cgtempstocks(data)
          }
            
          }
       
      }
    },
    cgtempstocks(data){
      enterforpda(data).then(res => {
              console.log(res);
              if (res.Success == true) {
                this.$Message.success("暂存库录入成功");
                if (this.isEdit == true) {
                  //  this.$router.go(-1);
                  var _this=this
                  setTimeout(function(){
                   _this.$store.dispatch("setSpearatedrawer", false);
                  },1000)
                } else {
                  (this.Waybill = {
                    ID: "", //WaybillID,
                    Code: "", //运单号
                    EnterCode: "", // 入仓号
                    Type: 3,
                    OrderID: "",
                    CarrierID: "",
                    ConsignorID: "",
                    ConsigneeID: "", // 收货人ID
                    ConsignorPlace: "", //发货人
                    Summary: "",
                    WarehouseID: sessionStorage.getItem("UserWareHouse"),
                    ShelveID: "",
                    Phone: "", //发货人Phone,
                    FreightPayer: null, //运费付费方,
                    Contact: "", //发货人
                    Supplier: "",
                    TempEnterCode: "",
                    Files: []
                  }),
                    (this.Summarydata = [
                      {
                        StorageID: "",
                        Type: "",
                        StorageID: "",
                        InputID: "",
                        Origin: "",
                        ProductID: "",
                        Product: null, // 默认Quantity:0, ProductID: ""
                        DateCode: "",
                        CreateDate: "",
                        ShelveID: "",
                        WareHouseID: sessionStorage.getItem("UserWareHouse"),
                        Total: "",
                        Quantity: "",
                        Supplier: "",
                        Summary: ""
                      }
                    ]);
                  this.Storages = [
                    {
                      SortingID: "",
                      Type: "",
                      StorageID: "",
                      InputID: "",
                      Origin: "",
                      ProductID: "",
                      Product: {
                        PartNumber: "",
                        Manufacturer: "",
                        PackageCase: "",
                        Packing: ""
                      },
                      DateCode: "",
                      CreateDate: "",
                      ShelveID: "",
                      WareHouseID: sessionStorage.getItem("UserWareHouse"),
                      Total: "",
                      Quantity: "",
                      Supplier: "",
                      Summary: ""
                    }
                  ];
                  this.Delete=[];
                }
              } else {
                this.$Message.error(
                  "保存失败，请检查录入信息是否正确或联系管理员"
                );
                 this.Delete=[];
              }
            });
    },
    changeimgs(newdata, row) {
      //上传照片
      if (row == "webaill") {
        this.Files.push(newdata);
      }
    },
    delimg(file) {
      var data={
          id:file.ID
       }
      CgDeleteFiles(data).then(res=>{
        if(res.Success==true){
          this.$Message.success( "删除成功" );
          this.Waybill.Files.splice(this.Files.indexOf(file), 1);
        }else{
          this.$Message.error( "删除失败" );
        }
      })
      console.log(this.Files);
    },
    fromphotos() {
      //拍照
      var data = {
        SessionID: "zancun",
        AdminID: sessionStorage.getItem("userID")

      };
      FormPhoto(data);
    },
    SeletUpload(type) {
      // 传照
      if (type == "Waybill") {
        var data = {
          SessionID: this.waybillid,
          AdminID: sessionStorage.getItem("userID")
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
        type: 8000
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
      if(this.$route.name == "revise"){
          this.$store.dispatch("setSpearatedrawer", false);
      }else{
        this.cleardata()
      }
    },
    GetUsableShelves() {
      var id = sessionStorage.getItem("UserWareHouse");
      GetUsableShelves("HK").then(res => {
        console.log(res);
        this.ShelveArr = res.obj;
      });
    },
    clackFilesProcess(url){
      var data={
        Url:url
      }
      FilesProcess(data)
    },
    cleardata(){
      this.Waybill = {
                    ID: "", //WaybillID,
                    Code: "", //运单号
                    EnterCode: "", // 入仓号
                    Type: 3,
                    OrderID: "",
                    CarrierID: "",
                    ConsignorID: "",
                    ConsigneeID: "", // 收货人ID
                    ConsignorPlace: "", //发货人
                    Summary: "",
                    WarehouseID: sessionStorage.getItem("UserWareHouse"),
                    ShelveID: "",
                    Phone: "", //发货人Phone,
                    FreightPayer: null, //运费付费方,
                    Contact: "", //发货人
                    Supplier: "",
                    TempEnterCode: "",
                    Files: []
                  }
      this.Summarydata = [
                      {
                        StorageID: "",
                        Type: "",
                        StorageID: "",
                        InputID: "",
                        Origin: "",
                        ProductID: "",
                        Product: null, // 默认Quantity:0, ProductID: ""
                        DateCode: "",
                        CreateDate: "",
                        ShelveID: "",
                        WareHouseID: sessionStorage.getItem("UserWareHouse"),
                        Total: "",
                        Quantity: "",
                        Supplier: "",
                        Summary: ""
                      }
                    ]
        this.Storages = [
                    {
                      SortingID: "",
                      Type: "",
                      StorageID: "",
                      InputID: "",
                      Origin: "",
                      ProductID: "",
                      Product: {
                        PartNumber: "",
                        Manufacturer: "",
                        PackageCase: "",
                        Packing: ""
                      },
                      DateCode: "",
                      CreateDate: "",
                      ShelveID: "",
                      WareHouseID: sessionStorage.getItem("UserWareHouse"),
                      Total: "",
                      Quantity: "",
                      Supplier: "",
                      Summary: ""
                    }
                  ];

    }
  }
};
</script>
