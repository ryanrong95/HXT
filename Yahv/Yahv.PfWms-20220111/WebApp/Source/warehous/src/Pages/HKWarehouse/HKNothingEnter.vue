<style scoped>
  .Mustfill {
    color: red;
  }
</style>
<template>
  <div>
    <Table :columns="columns7" :data="Storages">
      <template slot-scope="{ row, index }" slot="indexs">
        {{index+1}}
      </template>
      <template slot-scope="{ row, index }" slot="action">
        <Icon @click="addrow(index)"
              type="ios-add-circle-outline"
              style="margin-right:5px;font-size:20px;font-width:600" />
        <Icon @click="delrow(index)"
              type="ios-trash-outline"
              style="font-size:20px;font-width:600" />
      </template>
      <template slot-scope="{ row, index }" slot="Place">
        <Select v-model="row.origin"
                :transfer="true"
                filterable
                :label-in-value="true"
                @on-change="changoriginID($event,row)">
          <Option v-for="item in Conveyingplace" :value="item.CorPlace" :key="item.ID">{{item.Text}}</Option>
        </Select>
      </template>
      <template slot-scope="{ row, index }" slot="ShelveID">
        <div v-if="status==1">
          <Select size="default"
                  v-model="row.ShelveID"
                  filterable
                  :transfer="true"
                  :label-in-value="true"
                  @on-change="changConveyingplace($event,row)">
            <Option v-for="(item,index) in Storehouselist"
                    :value="item.ShelveID"
                    :label="item.ShelveID"
                    :key="index">
              <span>{{item.ShelveID}}</span>
              <span style="float:right;color:#ccc">{{item.TotalPackage}}</span>
            </Option>
          </Select>
        </div>
        <div v-else>
          <span>{{row.BoxCode|showboxcode}}</span>
          <!-- <Input v-model="row.BoxCode" placeholder="" disabled style='width:90%' /> -->
          <Icon type="md-create" @click="showboxarr(row.SortingID)" />
          <!-- <Select
            size="default"
            v-model="row.BoxCode"
            filterable
            :transfer="true"
            :label-in-value="true"
            @on-change="changConveyingplace2($event,row)"
          >
            <Option
              v-for="(item,index) in Boxingarr"
              :value="item.Code"
              :label="item.Code"
              :key="index"
            >
              <span>{{item.Code}}</span>
            </Option>
          </Select> -->
        </div>
      </template>
      <template slot-scope="{ row, index }" slot="imglist">
        <p v-for="(item,index) in row.Files">
          <span>{{item.CustomName}}</span>
          <Icon type="ios-trash-outline" :ref="row.ID" @click.native="handleRemovelist(row,index)"></Icon>
        </p>
        <!-- <Input v-model="row.typeimg" /> -->
      </template>
      <!-- <template slot-scope="{ row, index }" slot="operation"> -->
      <!-- <div class="setupload">
                  <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUpload(row.ID)">传照</Button>
              </div>
              <div class="setupload">
                  <Button type="primary" size="small" icon="md-reverse-camera" @click="fromphotos(row.ID)">拍照</Button>
      </div>-->
      <!-- </template> -->
    </Table>
    <Modal v-model="showEdit"
           title="选择箱号"
           :mask-closable="false"
           @on-visible-change='visiblechange'>
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日期：
        </label>
        <DatePicker type="date" style="width:80%" :options="options3" placeholder="请选择生成箱号的时间" :clearable='false' :value="saleDate" @on-change='changeData'></DatePicker>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱号：
        </label>
        <Input v-model.trim="newboxcode"
               maxlength="30"
               placeholder="请输入临时箱号"
               style="width:80%"
               @on-blur='handleCreate1(newboxcode)' />
      </p>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok_change">确定</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
import { GetSortingID,BoxcodeEnter,CgBoxesShow,BoxcodeDelete} from "../../api/CgApi";
import { getWayParter } from "../../api/index";
import {BoxIndexValidate} from "../../api/XdtApi";
export default {
  name:"HKNothingEnter",
  props: ["Storagesarr", "Storehouselist",'status','EnterCode','orderID'],
 
  data() {
    return {
       options3: {
          disabledDate (date) {
              return date && date.valueOf() < Date.now() - 86400000;
          }
      },
      saleDate:'',//箱号时间
      isclickbtn:false,
      itemSortingID:null,
      newboxcode:null,
      newboxcodeback:null,
      oldboxcode:null,//旧箱号
      boxingarr:[],
      boxcodetype:'1',
      showEdit:false,
      Conveyingplace: [],
      Storages: this.Storagesarr,
      titlename:"",
      columns7: [
        {
          title: "",
          slot: "indexs",
          width: 30,
          align: "center"
        },
        {
          title: " ",
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
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value:params.row.Product.PartNumber
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Product.PartNumber =event.target.value
                  vm.Storages[params.index] = params.row;
                  //   vm.consoleStorages()
                },
                'on-blur'(event){
                  var values=event.target.value
                  if(values!=null&&values!=''){
                     params.row.Product.PartNumber =vm.trim(values)
                     vm.Storages[params.index] = params.row;
                  }else{
                     params.row.Product.PartNumber =null
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
                h("span", {}, '品牌')
              ]);
            },
          key: "Manufacturer",
          align: "center",
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
                  //   vm.consoleStorages()
                },
                'on-blur'(event){
                  var values=event.target.value
                  if(values!=null&&values!=''){
                     params.row.Product.Manufacturer =vm.trim(values)
                     vm.Storages[params.index] = params.row;
                  }else{
                     params.row.Product.Manufacturer =null
                     vm.Storages[params.index] = params.row;
                  }
                }
              }
            });
          }
        },
        {
          title: "批号",
          key: "DateCode",
          align: "center",
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
                },
                'on-blur'(event){
                  var values=event.target.value
                  if(values!=null||values!=''){
                     params.row.DateCode =vm.trim(values)
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
                h("span", {}, "到货数量")
              ]);
            },
          key: "Quantity",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.CurrentQuantity
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  if (event.target.value != "") {
                    var reg = /^\d+(\.\d{0,2})?$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.CurrentQuantity = event.target.value;
                      vm.Storages[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数量");
                      (event.target.value = null),
                        (params.row.CurrentQuantity = null);
                      vm.Storages[params.index] = params.row;
                    }
                  }
                },
                'on-blur'(event){
                  var values=event.target.value
                  if(values!=null&&values!=''){
                     params.row.CurrentQuantity =vm.trim(values)
                     vm.Storages[params.index] = params.row;
                  }else{
                     params.row.CurrentQuantity =null
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
          slot: "Place"
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
                h("span", {}, this.status==1?'库位':'箱号')
              ]);
            },
          slot: "ShelveID",
          align: "center",
          width:250
        },
        {
          title: "体积(cm³)",
          key: "Volume",
          align: "center",
          // width: 70,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Volume
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  var reg = /^\d+(\.\d{0,2})?$/;
                  // reg.test(event.target.value);
                  if (reg.test(event.target.value) == true) {
                    params.row.Volume = event.target.value;
                    vm.Storages[params.index] = params.row;
                  } else {
                    //   vm.$Message.error("只能输入数字,包括两位数的小数点");
                    params.row.Volume = "";
                    event.target.value = "";
                    vm.Storages[params.index] = params.row;
                  }
                },
                "on-blur"() {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  var reg = /^\d+(\.\d{0,2})?$/;
                  // reg.test(event.target.value);
                  var values=event.target.value
                  if(values!=''||values!=null){
                    if (reg.test(values) == true) {
                      params.row.Volume =vm.trim(values);
                      vm.Storages[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      params.row.Volume = null;
                      event.target.value = null;
                      vm.Storages[params.index] = params.row;
                    }
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
                  this.status==1?'':'*'
                ),
                h("span", {}, '毛重(kg)')
              ]);
            },
          key: "Weight",
          align: "center",
          // width: 60,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Weight,
                autofocus: true
              },
              on: {
                "on-change"(event) {
                  var reg = /^\d+(\.\d{0,2})?$/;
                  var newtarget = vm.trim(event.target.value);
                  if (newtarget != ""&&newtarget != null) {
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget;
                      vm.Storages[params.index] = params.row;
                    } else {
                      params.row.Weight = "";
                      event.target.value = "";
                      vm.Storages[params.index] = params.row;
                    }
                  }
                },
                "on-blur"(event) {
                   var newtarget = vm.trim(event.target.value);
                  console.log(newtarget)
                  if (newtarget != null&&newtarget !='') {
                    var reg = /^\d+(\.\d{0,2})?$/;
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget;
                      vm.Storages[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      params.row.Weight = null;
                      event.target.value =null;
                      vm.Storages[params.index] = params.row;
                    }
                  }else{
                      params.row.Weight = null;
                      event.target.value =null;
                      vm.Storages[params.index] = params.row;
                  }
                },
                "on-enter": event => {
                  var reg = /^\d+(\.\d{0,2})?$/;
                  var newtarget = vm.trim(event.target.value)
                  if (reg.test(newtarget) == true) {
                    params.row.Weight = newtarget;
                    vm.Storages[params.index] = params.row;
                    var Input = params.row.Input;
                    var StandardProducts = params.row.Product;
                    var data2 = {
                      Quantity: params.row.Quantity, //数量
                      inputsID: Input.ID, //id
                      Catalog: StandardProducts.Catalog, //品名
                      PartNumber: StandardProducts.PartNumber, //型号
                      Manufacturer: StandardProducts.Manufacturer, //品牌
                      Packing: StandardProducts.Packing, //包装
                      PackageCase: StandardProducts.PackageCase, //封装
                      origin: Input.Origin
                    };
                    var newdata = [];
                    newdata.push(data2);
                    var configs = GetPrinterDictionary();
                    var getsetting = configs["产品标签"];
                    var str = getsetting.Url;
                    var testurl = str.indexOf("http") != -1;
                    if (testurl == true) {
                      getsetting.Url = getsetting.Url;
                    } else {
                      getsetting.Url = this.printurl + getsetting.Url;
                    }
                    var data = {
                      setting: getsetting,
                      data: newdata
                    };
                    TemplatePrint(data);
                  } else {
                    vm.$Message.error("只能输入数字,包括两位数的小数点");
                    params.row.Weight = null;
                    event.target.value = null;
                    vm.Storages[params.index] = params.row;
                  }
                }
              }
            });
          }
        },
        // {
        //   title: "图片",
        //   slot: "imglist",
        //   align: "center",
        //   width: 180
        // },
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
                h("span", {}, '异常原因')
              ]);
            },
          key: "imglist",
          align: "center",
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
                  //console.log(event.target.value)
                  params.row.Summary = event.target.value;
                  vm.Storages[params.index] = params.row;
                   var values=event.target.value
                  if(values!=null&&values!=''){
                     params.row.Summary =vm.trim(values)
                     vm.Storages[params.index] = params.row;
                  }else{
                     params.row.Summary =null
                     vm.Storages[params.index] = params.row;
                  }
                },
                'on-blur'(event){
                  console.log(event.target.value)
                  var values=event.target.value
                  if(values!=null&&values!=''){
                     params.row.Summary =vm.trim(values)
                     vm.Storages[params.index] = params.row;
                  }else{
                     params.row.Summary =null
                     vm.Storages[params.index] = params.row;
                  }
                }
              }
            });
          }
        },
        // {
        //   title: "操作",
        //   slot: "operation",
        //   align: "center"
        // }
      ]
    };
  },
  created() {

  },
  computed: {
  },
  // watch: {
  //  newboxcode(){
  //     var code=this.newboxcodeback
  //     if(code!=null){
  //       code=this.newboxcodeback.split("]")[1]
  //       // console.log(this.newboxcodeback.split("]")[1])
  //     }else{
  //       code=this.newboxcodeback
  //     }
  //     return code
  //   }
  // },
  mounted() {
    this.WayParterdata();
    console.log(this.status)
    if(this.status==1){
      // this.columns7[7].title="库位"
      this.titlename="库位"
      this
    }else{
      this.titlename="箱号"
      // this.columns7[7].title="箱号"
    }
  },
  methods: {
    setboxsplit(str) {
      //去除前后空格
      if(str){
         return str.split("]")[1]
      }
    },
    trim(str) {
      //去除前后空格
      return str.replace(/(^\s*)|(\s*$)/g, "");
    },
    WayParterdata() {
      //输送地列表
      getWayParter().then(res => {
        this.Conveyingplace = res.obj;
      });
    },
    CgBoxesShow() {
      var data = {
        enterCode: this.EnterCode, //入仓号
        date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
      };
        CgBoxesShow(data).then(res => {
          if (res.length > 0) {
            this.boxingarr = res;
          }else{
            this.boxingarr=[]
          }
        });
    },
    delrow(index) {
      //删除选中的录入信息
      this.Storages.splice(index, 1);
    },
    addrow(index) {
      GetSortingID().then(res => {
        var newrow = {
          iscx: false,
          isenter: true,
          ID: "LR" + index + new Date().getTime(),
          NoticeID: null,
          Product: {
            PartNumber: null,
            Manufacturer:null,
            PackageCase:null,
            Packaging: null
          },
          Conditions:{
            DevanningCheck: false,
            Weigh: false,
            CheckNumber: false,
            OnlineDetection: false,
            AttachLabel: false,
            PaintLabel: false,
            Repacking: false,
            IsCCC: false,
            IsCIQ: false,
            IsEmbargo: false,
            IsHighPrice: false,
            IsDeclared: false,
          },
          WaybillID:null,
          InputID: null,
          DateCode:null,
          Quantity: null,
          ArrivedQuantity:null,
          LeftQuantity:null,
          CurrentQuantity:null,
          Source:null,
          BoxCode: null,
          Weight:null,
          NetWeight: null,
          Volume:null,
          ShelveID: null,
          Type:null,
          origintext:null,
          origin:null,
          originDes: null,
          Summary: null,
          SortingID: res,
          Files: [],
          _disabled: false,
          boxdate:null
        };
        this.Storages.push(newrow);
      });
    },
    consoleStorages() {
      this.$parent.consoleStorages();
      // console.log(this.Storages)
    },
    //改变库位号
    changConveyingplace(value, row) {
      this.Storages.forEach(item => {
        if (item.ID == row.ID) {
          item.ShelveID = value.value;
        }
      });
    },
    //改变库位号
    changConveyingplace2(value, row) {
      // var data={
      //       enterCode:this.EnterCode,//必选参数，入仓号
      //       code:value.value,//
      //       date:Date.parse(new Date()),//必选参数，当前时间（可传空值过来会默认当前时间）
      //       adminID:sessionStorage.getItem("userID"),//装箱人
      //       tinyOrderID:null,
      //       orderID:this.orderID
      //   }
       var data={
          boxCode:value.value, // 只在录入箱号中起作用
          adminID: sessionStorage.getItem("userID"), //装箱人使用当前操作的adminID
          enterCode:this.EnterCode, // 统一使用当前运单的entercode
        }
        BoxcodeEnter(data).then(res=>{
          console.log(res)
          if(res.success==false&&res.code==400){
             this.$Message.error("箱号已经被选择，请选择其他箱号");
             this.Storages.forEach(item => {
              if (item.ID == row.ID) {
                item.BoxCode = null;
              }
            });
          }else{
            this.Storages.forEach(item => {
              if (item.ID == row.ID) {
                item.BoxCode = value.value;
              }
            });
          }
        })
    },
    changoriginID(value, row) {
      console.log(value);
      this.Storages.forEach(item => {
        if (item.ID == row.ID) {
          item.origin = value.value;
          item.origintext = value.label;
          item.originDes = value.label;
          // console.log(this.Storages)
        }
      });
    },
    visiblechange(value){
      if(value==true){
        // this.CgBoxesShow()
        const myDate = new Date();
        const year = myDate.getFullYear(); // 获取当前年份
        const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
        const day = myDate.getDate(); // 获取当前日（1-31）
        this.saleDate = `${year}/${month}/${day}`;
        this.newboxcode=null
        this.newboxcodeback=null
      }else{
        // this.BoxcodeDelete()
        this.newboxcode=null
        this.newboxcodeback=null
        this.oldboxcode=null
        this.boxcodetype='1'
      }
    },
    cancel(){
      this.showEdit=false
      this.oldboxcode=null
      if(this.newboxcode!=null){
        this.BoxcodeDelete()
      }
      // this.delarrbox()
    },
     handleCreate1(val) {  //箱号添加
      if(!val==false){
          if(val.slice(0,3)!='HXT'){
              this.$Message.error("请输入正确的箱号");
              this.newboxcode = null; 
              return;
          }

          var BoxIndexValidate_data = {
              BoxIndex : val,
              PackingDate : this.saleDate
          };  
          BoxIndexValidate(BoxIndexValidate_data).then(res => {          
              if(res.success){
                this.newboxcode = val;  
                this.newboxcodeback=val;
                this.isclickbtn=true           
              }else{                  
                this.newboxcode = "";             
                this.$Message.error(res.message);
                this.isclickbtn=false
              }
          });
      }else{
         this.$Message.error('请输入正确的箱号')
         this.newboxcode=null
      }

    },
    CgBoxesShow() {
      var data = {
        enterCode: this.EnterCode, //入仓号
        date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
      };
        console.log(this.EnterCode)
        CgBoxesShow(data).then(res => {
          if (res.length > 0) {
            this.boxingarr = res;
          }else{
            this.boxingarr=[]
          }
        });
    },
    ok_change(){
      if(this.newboxcodeback!=''&&this.newboxcodeback!=null){
          if(this.isclickbtn==true){
            this.Storages.forEach(item => {
              if (item.SortingID == this.itemSortingID) {
                 item.BoxCode = this.newboxcodeback;
                 item.boxdate=this.saleDate
              }
            });
            this.showEdit=false
            this.newboxcodeback=null
            this.newboxcode=null
          }
      }else{
         this.$Message.error('请选择或输入箱号')
      }
    },
    showboxarr(item){
      // console.log(item)
      this.itemSortingID=item
      this.showEdit=true
    },
     // 删除选定的箱号
    BoxcodeDelete(){
      // alert(this.newboxcode)
      var data={
        boxCode:this.newboxcodeback,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
        // console.log(res)
      })
    },
    oldBoxDelete(val){//删除旧箱号
      var data={
        boxCode:val,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
        // console.log(res)
      })
    },
    delarrbox(){
      console.log(this.Storages)
      this.Storages.forEach(item => {
        if (item.BoxCode!=null||item.BoxCode!='') {
            var data={
              boxCode:item.BoxCode,
              // date:item.boxdate
            }
            BoxcodeDelete(data).then(res=>{
              // console.log(res)
            })
        }
      });
    },
    changeData(val){
      this.saleDate=val
       if(this.newboxcode!=''&&this.newboxcode!=null){
        this.handleCreate1(this.newboxcode)
      }
    }
  }
};
</script>
