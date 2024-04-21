<style scoped>
.inputwidth {
  width: 14%;
  margin-right: 20px;
  margin-bottom: 10px;
  float: left;
}
.pages {
  float: right;
  padding-top: 20px;
}
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.detail_li .itemli {
  line-height: 40px;
}
.ivu-row {
  padding: 10px;
}

.detail_li .demo-badge-alone {
  height: 18px !important;
  line-height: 16px !important;
  padding: 0 4px !important;
  font-size: 12px !important;
}
.detail_tablebox {
  width: 100%;
  height: auto;
}
.successbtn{
  width: 50%;
  margin:20px auto;
}
.closDrawer_btn{
    font-size: 18px;
    font-weight: bold;
    position: absolute;
    right: 20px;
    top: 10px;
}
.icon1{
  font-size: 20px;
  font-weight: bold;
}
.icon1:hover{
  cursor: pointer;
}
.shaixuanbox {
  padding-bottom: 15px;
}
.shaixuanbox .title {
  font-size: 16px;
}
.shaixuanbox span:hover{
  cursor: pointer;
}
.item_status {
  display: inline-block;
  padding: 3px 7px;
  background: #2d8cf0;
  border: 1px solid #2d8cf0;
  margin-right: 5px;
  color: #ffffff;
  border-radius: 5px;
}
.item_status2 {
  display: inline-block;
  padding: 3px 7px;
  border: 1px solid #dddddd;
  margin-right: 5px;
  border-radius: 5px;
}
</style>
<template>
  <div>
     <!-- 暂时隐藏 -->
    <!-- <div>
      <div class="shaixuanbox">
       <em class="title">快速筛选：</em>
        <span
          :class="(testarrstate.indexOf(item.value)!==-1&&!istagall)||item.value==0&&istagall?'item_status':'item_status2'"
          v-for="item in NoticeStatus"
          :key="item.value"
          @click="testarrs(item.value)"
        >{{item.name }}</span>
      </div>
    </div> -->
    <div style="">
      <!-- <Input v-model="WaybillNo" placeholder="运单号" class="inputwidth" /> -->
      <Input v-model.trim="search_data.key" placeholder="订单号/运单号" class="inputwidth" />
      <Input v-model.trim="search_data.CustomName" placeholder="客户" class="inputwidth" />
      <Input v-model.trim="search_data.partnumber" placeholder="产品型号" class="inputwidth" />
      <!-- <Input v-model="Supplier" placeholder="供应商" class="inputwidth" /> -->
      <Select v-model.trim="search_data.status" placeholder="请选择状态"   class="inputwidth">
        <Option v-for="item in NoticeStatus" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select>
      <Select v-model.trim="search_data.source" placeholder="业务类型"   class="inputwidth">
        <Option v-for="item in NoticeType" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select>
      <div class="inputwidth">
        <DatePicker
         ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder="选择开始与结束时间"
          separator="  至  "
          style="width:225px;"
           @on-change="changedata"
        ></DatePicker>
      </div>
      <!-- <Input v-model="source" placeholder="来源(供应商/库房/客户)" class="inputwidth" /> -->
      <div style="float:left">
        <Button type="primary" icon="ios-search" @click="serch_btn" style="margin-right:10px">查询</Button>
        <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
      </div>
      
    </div>
    <div style="width:100%;display: inline-block; margin-top:20px;">
        <!-- <Dropdown>
          <Button type="primary">
              一键打印
              <Icon type="ios-arrow-down"></Icon>
          </Button>
          <DropdownMenu slot="list">
              <DropdownItem>出库单打印</DropdownItem>
              <DropdownItem>运单打印</DropdownItem>
              <DropdownItem >装箱单打印</DropdownItem>
          </DropdownMenu>
        </Dropdown> -->
      <!-- <div style="width:100px;float:left">
        <Select v-model="Printing_data" style="width:100px">
          <Option value="all_Printing" >一键打印</Option>
          <Option value="outgoing" >出库单打印</Option>
          <Option value="weball" >运单打印</Option>
          <Option value="Packing" >装箱单打印</Option>
        </Select>
      </div> -->
      <!-- 申报窗口暂时隐藏 -->
      <div style="width:100px;float:right">
        <Badge :count="5" type="error">
               <Button type="primary" icon="md-checkbox-outline" @click="dyncMount">申报窗口</Button>
        </Badge>
      </div>
      <!-- 申报窗口暂时隐藏 -->
    </div>
    <div style="width:100%;display: inline-block; margin-top:20px;">
      <!-- 临时按钮 开始 -->
      <!-- <Button type="primary" icon="md-checkbox-outline" @click="showDrawer('Waybill202001070017',30)">申报出库页面</Button> -->
      <!-- 临时按钮 结束 -->
      <Table :columns="columns1" :data="data1"  :loading="loading">
        <template slot-scope="{ row, index }" slot="WaybillID">
          {{row.WaybillID}}
          <Tag v-if="row.Source==10" color="#009688">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==20" color="#2F4056">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==30" color="#FF5722">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==40" color="#1E9FFF">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==50" color="#FFA2D3">{{row.SourceDescription}}</Tag>
        </template>
        <template slot-scope="{ row, index }" slot="CreateDate">
          {{row.CreateDate|showDate}}
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" size="small" @click="showDrawer(row.WaybillID,row.Source)">处理</Button>
        </template>
      </Table>
      <div class="pages">
        <Page :total="Total"  :page-size="search_data.PageSize" :current="search_data.PageIndex" @on-change="changepage" />
      </div>
    </div>
    <Drawer  :closable="false" v-model="showdetail" :width="80" :scrollable="true" :mask-closable="false">
      <div class="closDrawer_btn">
          <Icon @click="closDrawer" type="md-close" class="icon1"/>
      </div>
      <div>
        <!-- <out-details v-if="showtype==1"   ref="routineenter" :key="timer1"></out-details> -->
        <!-- <Declare  v-if="showtype==2" ref="Declare" :key="timer2"></Declare>  -->
        <!-- <Customs-window v-if="showtype==3" :key="timer3"></Customs-window> -->
        <router-view></router-view>
        <!-- <router-view v-if="showtype==1" ></router-view>
        <router-view v-if="showtype==2" ></router-view>
        <router-view v-if="showtype==3" ></router-view> -->
      </div>
    </Drawer>
  </div>
</template>
<script>
import Customswindow from "@/Pages/Common/Customswindow";
import {TemplatePrint,GetPrinterDictionary,FilePrint} from "@/js/browser.js"
import {pickingsout,NoticeSource,outStatus} from "../../api";
let product_url=require("../../../static/pubilc.dev")
import moment from "moment"
export default {
  name: "Outgoing",
  components: {
     "Customs-window":Customswindow,
    //  "out-details":outdetails
  },
  data() {
    return {
      search_data:{
          warehouseid:sessionStorage.getItem('UserWareHouse'),  //运单编号
          PageIndex:1,  //当前页面
          PageSize:10,  //当前页数
          CustomName: "", //客户
          status:"", //状态
          key:"", //订单号
          partnumber: "", //型号
          source:"", //业务类型
          startdate:"",//开始时间
          enddate:"",  //结束时间
      },
      testarrstate: [200,205,210,220],
      istagall: 0,
      printurl:product_url.pfwms,
      Total:0,//总数
      PageIndex:1,
      pagesize:10,
      loading:true,
      // showdetail:false,
      // showtype:0,
      timer3:0,
      Printing_data:"all_Printing",//打印的
      details: {
        //详情页
        WaybillNo: "90416165067", //运单号(本次)
        Carrier: "yunda", //承运商(本次)
        CarrierList: [
          //承运商列表
          {
            value: "shunfeng",
            label: "顺丰快递"
          },
          {
            value: "yunda",
            label: "韵达快递"
          },
          {
            value: "lianbang",
            label: "联邦快递"
          },
          {
            value: "ems",
            label: "EMS"
          }
        ],
        Conveyorsite: "美国", //输送地,
        columns1: [
          {
            type: "selection",
            width: 50,
            align: "center"
          },
          {
          title: " ",
          slot: "indexs",
          align: "left",
          width: 30,
          // fixed: 'right'
          },
          {
            title: "型号",
            key: "Model"
          },
          {
            title: "品牌",
            key: "brand"
          },
          {
            title: "批次",
            key: "batch"
          },
          {
            title: "已到/应到",
            slot: "Arrival",
            align: "center",
          },
          {
            title: "本次到货",
            slot: "Quantity",
            align: "center",
          },
          {
            title: "原产地",
            slot: "Country_origin",
            align: "center",
          },
          {
            title: "入库库位",
            slot: "StockCode",
            align: "center",
          },
          {
            title: "体积(cm³)",
            slot: "volume",
            align: "center",
          },
          {
            title: "毛重(g)",
            slot: "GrossWeight",
            align: "center",
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width:200
          },
        ],
      },
      // WaybillNo: "", //运单号（快递到货）、提货单号（自提）、送货单号（送货上门）
      CustomName: "", //部门
      Supplier: "", //供应商
      cityList: [
        //状态列表
        {
          value: "New York",
          label: "New York"
        },
        {
          value: "London",
          label: "London"
        },
        {
          value: "Sydney",
          label: "Sydney"
        },
        {
          value: "Ottawa",
          label: "Ottawa"
        },
        {
          value: "Paris",
          label: "Paris"
        },
        {
          value: "Canberra",
          label: "Canberra"
        }
      ],
      model1: "", //状态
      orderID: "", //订单号
      Model: "", //型号
      startdat:"",//开始时间
      enddate:"",//结束时间
      source: "", //业务类型
      NoticeStatus:[],// 状态数组
      NoticeType:[],  //业务类型状态
      columns1: [
        {
          title: "ID",
          slot: "WaybillID",
          // align: "center"
          width: 220
          // fixed: 'right'
        },
        {
          title: "通知时间",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "送货方式",
          key: "WaybillTypeDescription",
          align: "center"
        },
        {
          title: "客户名称",
          key: "ClientName",
          align: "center"
        },
        {
          title: "运单号",
          key: "Code",
          align: "center"
        },
        {
          title: "承运商",
          key: "CarrierName",
          align: "center"
        },
        {
          title: "供应商",
          key: "Supplier",
          align: "center"
        },
        // {
        //   title: "来源",
        //   key: "source"
        // },
        {
          title: "状态",
          key: "ExcuteStatusDescription",
          align: "center"
        },
        {
          title: "操作",
          slot: "action",
          align: "center"
          // fixed: 'right'
        }
      ],
      data1:[],
      value1: false,
      settimeouts:null,
    };
  },
   filters:{
        showDate:function (val) {
          // console.log(val)
            if (val != "") {
              if(val||""){
                var b = val.split("(")[1];
                var c = b.split(")")[0];
                var result = moment(+c).format('YYYY-MM-DD');
                return result;
              }
                
            }
        },
    },
  computed:{
    showdetail(){
        if(this.$store.state.common.showdetailout==true){
           clearInterval(this.settimeouts);
           this.settimeouts=null;
        }else{
          this.pickingsout(this.search_data); 
           this.settimeouts = setInterval(()=>{
              this.pickingsout(this.search_data); //实时获取通知列表
          }, 6000);
        }
        return  this.$store.state.common.showdetailout;
    },
    showtype(){
        return  this.$store.state.common.showtypeout;
    }
  },
  created() {
  },
  mounted() {
    this.setnva();
    this.getstatus()
    // var data={
    //   warehouseid:sessionStorage.getItem('UserWareHouse'),
    //   status:this.model1,
    //   PageIndex:this.PageIndex,
    //   PageSize:this.pagesize,
    // }
    var searchdata={
          warehouseid:sessionStorage.getItem('UserWareHouse'),  //运单编号
          PageIndex:this.PageIndex,  //当前页面
          PageSize:this.pagesize,  //当前页数
          CustomName: this.CustomName, //客户
          status: this.model1, //状态
          orderID:this.orderID, //订单号
          partnumber: this.Model, //型号
          source: this.storagetype, //业务类型
          startdate:this.startdat,//开始时间
          enddate:this.enddate,  //结束时间
      }
    this.pickingsout(this.search_data)
    // this.settimeouts = setInterval(()=>{
    //      this.pickingsout(this.search_data); //实时获取通知列表
    // }, 6000);
  },
// beforeRouteLeave (to, from , next) {
//   clearTimeout(this.settimeouts)
//    next()
//   },
//  beforeRouteUpdate (to, from, next) {
//     if(to.path=='/Outgoing'){
//         this.pickingsout(this.search_data)
//     }else{
//        clearTimeout(this.settimeouts);
//     }
//     next()
//   },
  methods: {
    dyncMount(){  //点击按钮加载子组件  //加载申报窗页面
    // this.showdetail = true;
    this.$store.dispatch("setshowdetailout", true);
    // this.showtype=3;
    // // console.log(this.showtype=3)
    this.$store.dispatch("setshowtype", 3);
    this.timer3 = new Date().getTime()
     this.$router.push({ path:"/Outgoing/Customswindow"})
    },
    // showDrawer(id) {
    //   // this.value1 = true;
    //   this.showdetail = true;
    //   console.log(id);
    //   this.showtype=1;
    //   this.timer1 = new Date().getTime()
    // },
     showDrawer(id,type) {
       clearInterval(this.settimeouts);
       this.settimeouts=null;
      this.$store.dispatch("setshowdetailout", true);
      if(type==30){  //如果是
        this.$store.dispatch("setshowtype", 2);
        this.$router.push({ path:"/Outgoing/OutDeclareDetail/"+id})
      }else{
        this.$store.dispatch("setshowtype", 1);
        this.$router.push({ path:"/Outgoing/outdetail/"+id})
      }
    },
    closDrawer(){  //关闭抽屉的方法
      if(this.showtype==1||this.showtype==3||this.showtype==2){
        // this.$router.go(-1);   //控制路由跳回原来页面
         this.$router.push({ path:"/Outgoing"})
      }
      this.pickingsout(this.search_data)
      // this.settimeouts = setInterval(()=>{
      //    this.pickingsout(this.search_data); //实时获取通知列表
      // }, 6000);
      this.$store.dispatch("setshowdetailout", false);

      // console.log(this.showtype)
    },
    setnva() {
      var cc = [
        {
          title: "出库",
          href: "/Warehousing"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    outorderprint(){  //打印出库通知
      var printsrr=[{name:"1111"}]
      var configs=GetPrinterDictionary()
        var getsetting=configs['出库通知打印']
        var href=window.location.protocol+"//"+window.location.host;
        var newurl="http://hv.warehouse.b1b.com"+getsetting.Url
        getsetting.Url=newurl
        var data = {
             setting:getsetting,
             data: printsrr
            };
        TemplatePrint(data);
    },
    serch_btn(){  //搜索按钮
       if(this.search_data.CustomName!=''||this.search_data.status!=''||this.search_data.key!=''||this.search_data.partnumber!=''||this.search_data.source!=''||this.search_data.startdate!=''||this.search_data.enddate!=''){
             this.search_data.PageIndex=1;
            this.pickingsout(this.search_data)
             this.loading=true;
       }else{
          this.$Message.error('请至少输入一个查询条件');
       }
    },
    clear_btn(){
     if(this.search_data.CustomName!=''||this.search_data.status!=''||this.search_data.key!=''||this.search_data.partnumber!=''||this.search_data.source!=''||this.search_data.startdate!=''||this.search_data.enddate!=''){
           this.search_data={
              warehouseid:sessionStorage.getItem('UserWareHouse'),  //运单编号
              PageIndex:1,  //当前页面
              PageSize:10,  //当前页数
              CustomName: "", //客户
              status:"", //状态
              key:"", //订单号
              partnumber: "", //型号
              source:"", //业务类型
              startdate:"",//开始时间
              enddate:"",  //结束时间
          };
          this.$refs.element.handleClear();
          this.pickingsout(this.search_data)
          // warehouseid, key, partnumber, CustomName, startdate, enddate, status, source, ntype, pageindex, pagesize
       }
    },
    changedata(value){  //时间格式 获取开始时间与结束时间
      // console.log(value)
      this.startdate=value[0];
      this.enddate=value[1];

      this.search_data.startdate=value[0];
      this.search_data.enddate=value[1];
    },
    changepage(value){
        this.loading=true;
        this.PageIndex=value;
        this.search_data.PageIndex=value;
        //  var searchdata={
        //   warehouseid:sessionStorage.getItem('UserWareHouse'),  //运单编号
        //   PageIndex:this.PageIndex,  //当前页面
        //   PageSize:this.pagesize,  //当前页数
        //   CustomName: this.CustomName, //客户
        //   status: this.model1, //状态
        //   orderID:this.orderID, //订单号
        //   Model: this.Model, //型号
        //   storagetype: this.storagetype, //业务类型
        //   startdate:this.startdat,//开始时间
        //   enddate:this.enddate,  //结束时间
        //  }
          this.pickingsout(this.search_data)
    },
    pickingsout(data){
      // console.log(data)
      var _this=this;
      pickingsout(data).then(res => {
        if(res.obj.Data!=[]){
           this.loading=false;
            this.data1=res.obj.Data;
            this.Total=res.obj.Total;
            
        }else{
        }
        });
    },
    getstatus(){ //获取通知状态与业务类型
       NoticeSource().then((res) => {  //业务类型
            //  console.log(res)
            this.NoticeType=res.obj;
        })
        outStatus().then((res) => {  //通知状态
            //  console.log(res)
            this.NoticeStatus=res.obj;
            // var data = {
            //   name: "全部",
            //   value: 0
            // };
        // this.NoticeStatus.splice(0, 0,data);

        })
    },
     sumbit_print(){  //运单打印功能
      var configs=GetPrinterDictionary()
      var getsetting=configs['装箱单打印']
        // var href=window.location.protocol+"//"+window.location.host;
         var str=getsetting.Url
         var testurl=str.indexOf("http") != -1;
        if(testurl==true){
          getsetting.Url=getsetting.Url
        }else{
           getsetting.Url=this.printurl+getsetting.Url;
        }
        var data = {
             setting:getsetting,
             data:[
               {
                 name:"11111"
               }
             ]
            };
        TemplatePrint(data);
    },
    testarrs(item) {
      if (item == 0) {
        if (this.testarrstate.indexOf(item) == -1) {
          this.testarrstate = [];
          this.testarrstate.push(item);
          this.istagall = true;
        } else {
          // this.testarrstate = [];
          this.istagall = false;
        }
      } 
      else {
        this.testarrstate = this.testarrstate.filter(function(item2) {
            return item2 != 0;
          });
        this.istagall = false;
        if (this.testarrstate.indexOf(item) == -1) {
          this.testarrstate.push(item);
        } else {
          this.testarrstate = this.testarrstate.filter(function(item2) {
            return item2 != item;
          });
          if(this.testarrstate.length==0){
            this.testarrstate.push(0);
          }
        }
      }
      var cc=this.testarrstate.join(",")
    },
    closeTimer() {  //关闭定时器
          window.clearInterval(this.settimeouts)
          this.settimeouts = null
      },

  },
beforeDestroy() {
    //  console.log("出库组件被销毁")
      clearInterval(this.settimeouts);
      this.settimeouts=null;
    // this.closeTimer()
},
  destroyed() {
    //组件销毁
    //如果定时器在运行则关闭
     clearInterval(this.settimeouts);
     this.settimeouts=null;
    // this.closeTimer()
  },

};
</script>