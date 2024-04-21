<style scoped>
.ivu-table-cell {
    padding-left: 5px;
    padding-right: 5px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
    word-break: break-all;
    box-sizing: border-box;
}
.inputwidth {
  width: 200px;
  margin-right: 20px;
  margin-bottom: 10px;
  /* float: left; */
}
.pages {
  float: right;
  padding-top: 20px;
}
.successbtn {
  width: 50%;
  margin: 20px auto;
}
.closDrawer_btn {
  font-size: 18px;
  font-weight: bold;
  position: absolute;
  right: 20px;
  top: 10px;
}
.closDrawer_btn .icon1:hover {
  cursor: pointer;
}
.shaixuanbox {
  padding-bottom: 30px;
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
label{
  font-size: 14px;
  line-height: 32px;
    /* vertical-align: text-bottom; */
}
</style>
<template>
  <div>
    <!--暂时隐藏-->
    <div class="shaixuanbox">
      <em class="title">快速筛选：</em>
      <span class="item_status2">全部</span>
      <span
        :class="(testarrstate.indexOf(item.value)!==-1&&!istagall)||item.value==0&&istagall?'item_status':'item_status2'"
        v-for="item in NoticeStatus"
        :key="item.value"
        @click="testarrs(item.value)"
      >{{item.name }}</span>
    </div>
    <!-- <h1>重构</h1> -->
    <div>
        <label for="运单号/入仓号/订单号">运单号/入仓号/订单号：</label>
          <Input
            v-model.trim="search_datas.key"
            placeholder=""
            autofocus
            @on-enter="search_btn"
            class="inputwidth"
          />
      <label for="供应商">供应商：</label>
      <Input
        v-model.trim="search_datas.WaybillSupplier"
        placeholder=""
        @on-enter="search_btn"
        class="inputwidth"
      />
      <label for="供应商">产品型号：</label>
      <Input
        v-model.trim="search_datas.ProductPartNumber"
        placeholder=""
        @on-enter="search_btn"
        class="inputwidth"
      />

      <!-- <Select v-model.trim="search_datas.WaybillExcuteStatus" placeholder="请选择状态" class="inputwidth">
        <Option v-for="item in NoticeStatus" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select> -->
      <label for="业务类型">业务类型：</label>
      <Select v-model.trim="search_datas.Source" placeholder="" class="inputwidth">
        <Option v-for="item in NoticeType" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select><br/>
      <div style="float: left;padding-right: 20px;">
        <label for="业务类型"> 开始时间与结束时间：&nbsp;&nbsp;</label>
        <DatePicker
          ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder=""
          :editable="false"
          separator="  至  "
          @on-change="changedata"
          style="width:200px;"
        ></DatePicker>
      </div>
      <Button type="primary" icon="ios-search" @click="search_btn" style="margin-right:10px">查询</Button>
      <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
    </div>
    <div style="width:100%;display: inline-block; text-align: right;margin-top: 30px">
      <Button type="primary" style="margin-right:10px" @click="staging">到货暂存</Button>
      <Badge :count="50">
        <Button type="primary" icon="md-checkbox-outline" @click="showDrawer('',1000)">申报窗口</Button>
      </Badge>
    </div>
    <div style="width:100%;display: inline-block; margin-top:20px;">
      <!-- 临时按钮  -->
      <!-- <div>
        <Button type="primary" size="small" @click="showDrawer('Waybill201912260006','30')">报关处理</Button>
      </div> -->
      <!-- 临时按钮  -->

      <Table :columns="Notice_title" :data="Notice_list" :loading="loading" @on-row-dblclick="tabledblclick">
        <template slot-scope="{ row, index }" slot="orderID">
          <!-- <span>{{row.Waybill.OrderID}}</span> -->
          WL46120200217004
        </template>
        <template slot-scope="{ row, index }" slot="Sourcedec">
          <Tag v-if="row.Waybill.Source==10" color="#009688">{{row.Waybill.SourceDes}}</Tag>
          <Tag v-if="row.Waybill.Source==20" color="#2F4056">{{row.Waybill.SourceDes}}</Tag>
          <Tag v-if="row.Waybill.Source==30" color="#FF5722">{{row.Waybill.SourceDes}}</Tag>
          <Tag v-if="row.Waybill.Source==40" color="#1E9FFF">{{row.Waybill.SourceDes}}</Tag>
          <Tag v-if="row.Waybill.Source==50" color="#FFA2D3">{{row.Waybill.SourceDes}}</Tag>
        </template>
        <template slot-scope="{ row, index }" slot="CreateDate">
          <span>{{row.Waybill.CreateDate|showDate}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="WaybillTypeDescription">
          <!-- <span>{{row.CreateDate|showDate}}</span> -->
          {{row.Waybill.TypeDes}}
        </template>
        <template slot-scope="{ row, index }" slot="EnterCode">
          <!-- <span>{{row.CreateDate|showDate}}</span> -->
          {{row.Waybill.EnterCode}}
        </template>
        <template slot-scope="{ row, index }" slot="Code">
          {{row.Waybill.Code}}
        </template>
        <template slot-scope="{ row, index }" slot="CarrierName">
         {{row.Waybill.Carrier}}
        </template>
        <template slot-scope="{ row, index }" slot="Supplier">
          {{row.Waybill.Supplier}}
        </template>
        <template slot-scope="{ row, index }" slot="ExcuteStatusDescription">
          {{row.Waybill.ExcuteStatusDes}}
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button disabled v-if="row.ExcuteStatus==200" @click="showDrawer(row.Waybill.ID,row.Waybill.Source)">查看</Button>
          <Button type="primary" size="small" v-else @click="showDrawer(row.Waybill.ID,row.Waybill.Source)">处理</Button>
        </template>
      </Table>
      <div class="pages">
        <Page :total="total" @on-change="changepage" :page-size="10" :current="search_datas.PageIndex"  />
      </div>
    </div>
    <Drawer
      title
      :closable="false"
      v-model="showdetail"
      :width="90"
      :scrollable="true"
      :mask-closable="false"
    >
      <div>
        <div slot="close" class="closDrawer_btn">
          <!-- <Icon @click="closDrawer" type="ios-close-circle-outline" /> -->
          <!-- <Icon type="md-close" /> -->
          <Icon @click="closDrawer" type="md-close" class="icon1" />
        </div>
        <routine-enter
          v-if="showtype==1"
          :fatherMethod="fatherMethod"
          ref="routineenter"
          :key="timer1"
        ></routine-enter>
        <!-- <Declare v-if="showtype==2" ref="Declare" :key="timer2"></Declare> -->
        <!-- <Customs-window v-if="showtype==3" :key="timer3"></Customs-window> -->
        <router-view></router-view>
        <!-- <router-view v-if="showtype==2"></router-view>
        <router-view v-if="showtype==4"></router-view>
        <router-view v-if="showtype==3" :key="$route.fullPath"></router-view>
        <router-view v-if="showtype==5"></router-view> -->
      </div>
    </Drawer>
  </div>
</template>
<script>
import Vue from "Vue";
import { wareing, NoticeSource, ExcuteStatus } from "../../api"; //引入api 的接口
import { cgsortings,cgSearch_sortings, } from "../../api/CgApi"; //引入api 的接口

import moment from "moment";
import RoutineEnter from "@/Pages/Cgenter/RoutineEnter"; //代入库
import Declare from "@/Pages/Common/Declare"; //代报关
import imgtest from "@/Pages/Common/imgtes"; //图片上传
import Customswindow from "@/Pages/Cgenter/Customswindow";
export default {
  name: "Warehousing",
  components: {
    "routine-enter": RoutineEnter,
    Declare: Declare,
    "img-test": imgtest,
    "Customs-window": Customswindow
  },
  data() {
    return {
      testarrstate: [100,105,108, 110,115,125,130],
      istagall: 0,
      timer1: "",
      timer2: "",
      timer3: "",
      loading: true, //loading效果
      warehouseID: sessionStorage.getItem("UserWareHouse"),
      search_datas:{ //新的搜搜
        "key": "",
        "WaybillSupplier": "",
        "ProductPartNumber": "",
        "WaybillExcuteStatus":'100,105,108, 110,115,125,130',
        "Source": "",
        "StartDate":"",
        "EndDate":"",
        "PageSize": 10,
        "PageIndex": 1,
        "WhID":sessionStorage.getItem("UserWareHouse"),
      },
      Notice_title: [
        // {
        //   type: "selection",
        //   width: 60,
        //   align: "center"
        // },
        {
          title: "订单号",
          slot: "orderID",
          align: "center",
          width: 180
        },
        {
          title: "业务类型",
          slot: "Sourcedec",
          align: "center",
          width: 110
        },
        // {
        //   title: "自提时间",
        //   slot: "CreateDate",
        //   align: "center"
        // },
        {
          title: "到货方式",
          slot: "WaybillTypeDescription",
          align: "center"
        },
        {
          title: "运单号",
          slot: "Code",
          align: "center",
        },
         {
          title: "承运商",
          slot: "CarrierName",
          align: "center"
        },
        {
          title: "入仓号",
          slot: "EnterCode",
          align: "center"
        },
        {
          title: "供应商",
          slot: "Supplier",
          align: "center"
        },
        {
          title: "操作时间",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "状态",
          slot: "ExcuteStatusDescription",
          align: "center",
        },
        {
          title: "操作",
          slot: "action",
          align: "center"
          // fixed: 'right'
        }
      ],
      Notice_list: [], //通知列表

      // showdetail: false,//左侧抽屉是否显示
      // showtype:0,//显示处理的视图 1 代收货  2代报关  4暂存  5出库详情
      Showdetailid: "",
      NoticeStatus: [],
      NoticeType: [],
      total: 0,
      setIntervaltimer:null,
      timenmae:null,
    };
  },
  created () {
   this.clear_time()
   window['setIntervaltimer'] =this.setIntervaltimer;
   this.cgsortings()
  },
  computed: {
    getid() {
      console.log(this.Showdetailid);
      return this.Showdetailid;
    },
    gethouse() {
      return sessionStorage.getItem("UserWareHouse");
    },
    showdetail() {
      if(this.$store.state.common.showdetail==true){
         clearInterval(this.timenmae);
         this.timenmae=null;
      }else{
         var _this = this;
         this.new_time()
        //  _this.wareing(_this.search_datas); 
        // this.setIntervaltimer = setInterval(()=> {
        //   _this.wareing(_this.search_datas); //实时获取通知列表
        // }, 6000);
        //   this.wareing(this.search_datas)
      }
      return this.$store.state.common.showdetail;
    },
    showtype() {
      return this.$store.state.common.showtypein;
    }
  },
  mounted() {
    this.setnva();
    // var datas = {
    //   warehouseID: this.warehouseID, //"BJ04",//this.warehouseID,
    //   status: this.search_datas.status,
    //   PageIndex: this.search_datas.PageIndex, //当前页码
    //   PageSize: 10 //每页条数
    //   // source:
    // };
    // this.wareing(datas); //获取通知列表
    this.getstatus();
    // var _this = this;
    // this.setIntervaltimer = setInterval(()=> {
    //    _this.wareing(_this.search_datas); //实时获取通知列表
    //  }, 6000);
  //  this.new_time()
  },
  filters: {
    showDate: function(val) {
      // console.log(val)
      if (val != "") {
        if (val || "") {
          var b = val.split("(")[1];
          var c = b.split(")")[0];
          var result = moment(+c).format("YYYY-MM-DD");
          return result;
        }
      }
    }
  },
  methods: {
    tabledblclick(row,index){
      console.log(row)
      this.showDrawer(row.Waybill.ID,row.Waybill.Source)
    },
    new_time(){  //创建实时请求
      var that=this
      this.timenmae=setInterval(() => {
        setTimeout(()=>{
          that.cgSearch_sortings(that.search_datas);    
          }, 0)
      }, 2000)
    },
    clear_time(){ //销毁定时器
      clearInterval(this.timenmae)
      this.timenmae=null;
    },
    fatherMethod() {
      this.$store.dispatch("setshowdetail", false);
      this.$store.dispatch("setshowtypein", 0);
      
      // var datas = {
      //   warehouseID: this.warehouseID,
      //   status: this.search_datas.status,
      //   PageIndex: this.search_datas.PageIndex, //当前页码
      //   PageSize: 10 //每页条数
      // };
      // this.wareing(datas); //获取通知列表
    },
    dyncMount() {
      //点击按钮加载子组件
      // this.showdetail = true;
      this.$store.dispatch("setshowdetail", true);
      // this.showtype=3;
      this.$store.dispatch("setshowtypein", 3);
      this.timer = new Date().getTime();
    },
    showDrawer(id, type) {
      //根据获取的类型，展示不同的组件
      // this.showdetail = true;
      this.clear_time()
      this.setIntervaltimer=null;
      this.$store.dispatch("setshowdetail", true);
      this.$store.dispatch("setshowtypein", 2);
      this.Showdetailid = id;
      if (type == 30) {
        this.$router.push({
          name: 'Declare',
          params: {
            wayBillID: id
          }
        })
      } else if (type == 1000) {
        //报关申请窗口
        // this.showtype=3;
        this.$store.dispatch("setshowtypein", 3);
        // this.timer3 = new Date().getTime()
        // this.$router.push({ path: "/Warehousing/Customswindow" });Cgenter
        this.$router.push({ path: "/Cgenter/Customswindow" });
      } else {
        // this.showtype=1; //待收货 常规入库
        this.$store.dispatch("setshowtypein", 1);
        this.timer1 = new Date().getTime();
        var that = this;
        setTimeout(function() {
          that.$refs.routineenter.WayParterdata(); //输送地列表
          // that.$refs.Declare.Carriers();            //承运商
          // that.$refs.routineenter.getdetail_data(id);
          that.$refs.routineenter.CgDetail_new(id);
          // that.$refs.routineenter.GetUsableShelves()  //调用可用库位号
        }, 20);
      }
    },
    closDrawer() {
      //关闭抽屉的方法
      if (this.showtype == 4 || this.showtype == 3|| this.showtype == 2) {
        this.$router.go(-1); //控制路由跳回原来页面
      }
      // this.showdetail=false;
      var _this = this;
       this.cgSearch_sortings(this.search_datas);
      this.$store.dispatch("setshowdetail", false);
    },
    changepage(value) {
      this.loading=true;
      this.search_datas.PageIndex = value;
      this.cgSearch_sortings(this.search_datas);
      // this.wareing(this.search_datas);
    },
    changedata(value) {
      //时间格式 获取开始时间与结束时间
      // console.log(value)
      //  "StartDate":"",
      //   "EndDate":"",
      this.search_datas.StartDate = value[0];
      this.search_datas.EndDate = value[1];
    },
    search_btn() {
      //根据搜索条件查询
      // console.log(this.search_datas)
        // "key": "",
        // "WaybillSupplier": "",
        // "ProductPartNumber": "",
        // "WaybillExcuteStatus": "115,108",
        // "Source": "",
        // "StartDate":"",
        // "EndDate":"",
        // "PageSize": 10,
        // "PageIndex": 1,
        // "WhID":sessionStorage.getItem("UserWareHouse"),
      if (
        this.search_datas.key != "" ||
        this.search_datas.WaybillExcuteStatus != "" ||
        this.search_datas.WaybillSupplier != "" ||
        this.search_datas.StartDate != "" ||
        this.search_datas.EndDate != "" ||
        this.search_datas.ProductPartNumber != "" ||
        this.search_datas.Source != ""
      ){
         this.loading=true;
         this.search_datas.PageIndex=1;
        this.cgSearch_sortings(this.search_datas);
        // console.log(this.search_datas)
      }else{
        this.$Message.error('请至少输入一个查询条件');
      }
      
      // clearInterval(this.setIntervaltimer);
    },
    clear_btn() {
      //清空搜索条件
      if (
        this.search_datas.key != "" ||
        this.search_datas.WaybillExcuteStatus != "" ||
        this.search_datas.WaybillSupplier != "" ||
        this.search_datas.StartDate != "" ||
        this.search_datas.EndDate != "" ||
        this.search_datas.ProductPartNumber != "" ||
        this.search_datas.Source != ""
      ) {
        this.search_datas = {
          WhID: sessionStorage.getItem("UserWareHouse"),
          key: "", //运单号(可以为空)  入仓号
          WaybillSupplier: "", //供应商(可以为空)
          WaybillExcuteStatus:this.testarrstate.join(","), //通知状态(可以为空)
          StartDate: "", //开始时间(可以为空)
          EndDate: "", //结束时间(可以为空)
          ProductPartNumber: "", //型号(可以为空)
          Source: "", //入库类型(可以为空)source
          PageIndex: 1, //当前页码
          PageSize: 10 //每页条数
        };
        this.loading=true;
        this.search_datas.PageIndex=1;
        this.$refs.element.handleClear();
         this.cgSearch_sortings(this.search_datas);
         console.log(this.search_datas)
        var _this = this;
      }
      
      // this.setIntervaltimer = setInterval(function() {
      //   _this.wareing(_this.search_datas); //实时获取通知列表
      // }, 5000);
    },
    wareing(data) {
      //获取基本数据与查询的数据
      wareing(data)
        .then(res => {
          this.total = res.obj.Total;
          this.loading = false;
          if (this.total != 0) {
            this.Notice_list = res.obj.Data;
          } else {
            this.Notice_list = [];
          }
        })
        .catch(err => {
          this.loading = false;
          this.Notice_list = [];
        });
    },
    getstatus() {
      //获取通知状态与业务类型
      NoticeSource().then(res => {
        //业务类型
        //  console.log(res)
        this.NoticeType = res.obj;
      });
      ExcuteStatus().then(res => {
        //通知状态
        //  console.log(res)
        this.NoticeStatus = res.obj;
        // var data = {
        //   name: "全部",
        //   value: 0
        // };
        // this.NoticeStatus.splice(0, 0,data);
      });
    },
    setnva() {
      var cc = [
        {
          title: "入库",
          href: "/Warehousing"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    staging() {
      //  this.showdetail = true;
      this.$store.dispatch("setshowdetail", true);
      //  this.showtype=4;
      this.$store.dispatch("setshowtypein", 4);
      // this.$router.push({ path: "/Warehousing/separate" });
      this.$router.push({ path: "/Cgenter/separate" });      
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
      this.search_datas.WaybillExcuteStatus=this.testarrstate.join(",")
      this.search_btn()
      // console.log(this.search_datas.WaybillExcuteStatus)

    },
    // 重构 初始化获取数据
    cgsortings(){
      cgsortings(this.warehouseID,1,10).then(res=>{
        // console.log(res)
        this.total = res.obj.Total;
        this.Notice_list=res.obj.Data;
        this.loading = false;
      })
    },
    // 搜索获取数据的方法
    cgSearch_sortings(data){
      cgSearch_sortings(data).then(res=>{
        // console.log(res)
        this.total = res.obj.Total;
        this.Notice_list=res.obj.Data;
        this.loading = false;
      })
    }
  },
  beforeDestroy() {
        console.log("入库组件被销毁")
        this.clear_time()
		},
  destroyed() {
      this.clear_time()
  }
};
</script>
