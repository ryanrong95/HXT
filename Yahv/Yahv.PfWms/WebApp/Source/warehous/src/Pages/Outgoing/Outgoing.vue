<style scoped>
.inputwidth {
  margin-right: 60px;
  margin-bottom: 10px;
  float: left;
}
.inputitems {
  width: 200px;
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

.detail_li >>> .demo-badge-alone {
  height: 18px !important;
  line-height: 16px !important;
  padding: 0 4px !important;
  font-size: 12px !important;
}
.detail_tablebox {
  width: 100%;
  height: auto;
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
.icon1 {
  font-size: 20px;
  font-weight: bold;
}
.icon1:hover {
  cursor: pointer;
}
.shaixuanbox {
  padding-bottom: 15px;
}
.shaixuanbox .title {
  font-size: 16px;
}
.shaixuanbox span:hover {
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
.tabledata >>> .ivu-table-cell{
  padding-left: 2px;
  padding-right: 2px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  word-break: break-all;
  box-sizing: border-box;
}
</style>
<template>
  <div>
    <!-- 暂时隐藏 -->
    <div id="outgoing">
      <div class="shaixuanbox" v-if="isshowstate==true">
        <em class="title">快速筛选：</em>
        <span
          :class="(testarrstate.indexOf(item.value)!==-1&&!istagall)||item.value==10&&istagall?'item_status':'item_status2'"
          v-for="item in NoticeStatus"
          :key="item.value"
          @click="testarrs(item.value)"
        >{{item.name }}</span>
      </div>
    </div>
    <!-- <Button type="primary" size="small" @click="showDrawer('Waybill202003250011',20)">处理</Button>
    <Button type="primary" size="small" @click="showDrawer('WL41620200302003',30)">代报关处理</Button> -->
    <div style>
      <p class="inputwidth" v-if="warehouseID.indexOf('SZ')==-1">
        <label for="订单号/运单号">运单号/入仓号/订单号/运输批次：</label>
        <Input v-model.trim="search_data.Key" @on-enter='serch_btn' placeholder="运单号/入仓号/订单号/运输批次" style="width:220px" />
      </p>
      <p class="inputwidth" v-else>
        <label for="订单号/运单号">运单号/入仓号/订单号/ID：</label>
        <Input v-model.trim="search_data.Key" @on-enter='serch_btn' placeholder="运单号/入仓号/订单号/ID" style="width:220px" />
      </p>
      <p class="inputwidth">
        <label for="客户">客户：</label>
        <Input v-model.trim="search_data.Client" placeholder="客户" class="inputitems" />
      </p>
      <p class="inputwidth">
        <label for="产品型号：">产品型号：</label>
        <Input v-model.trim="search_data.ProductPartNumber" placeholder="产品型号" class="inputitems" />
      </p>
      <div class="inputwidth">
        <label for="业务类型" style="float: left;line-height:32px;">业务类型：</label>
        <Select v-model.trim="search_data.Source" placeholder="业务类型" style="float:left;width:200px">
          <Option v-for="item in NoticeType" :value="item.value" :key="item.value">{{ item.name }}</Option>
        </Select>
      </div>
      <div style="float: left;padding-right: 20px;">
        <label for="开始时间与结束时间">开始时间与结束时间：&nbsp;&nbsp;</label>
        <DatePicker
          ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder="选择开始与结束时间"
          separator="  至  "
          style="200px"
          @on-change="changedata"
        ></DatePicker>
      </div>
      <!-- <Input v-model="source" placeholder="来源(供应商/库房/客户)" class="inputwidth" /> -->
      <div style="float:left">
        <Button type="primary" icon="ios-search" @click="serch_btn" style="margin-right:10px">查询</Button>
        <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
      </div>
    </div>
    <div style="width:100%;display: inline-block; margin-top:20px;"></div>
    <div style="width:100%;display: inline-block; margin-top:20px;">
      <Table :columns="columns1" :data="data1" :loading="loading" ref="table" :max-height="tableHeight" class="tabledata">
        <!-- <template slot-scope="{ row, index }" slot="OrderID" v-if="warehouseID.indexOf('SZ')==-1">
          <span v-if="row.LotNumber!=null">{{row.LotNumber}}</span>
          <span v-else>{{row.OrderID}}</span>
        </template> -->
        <template slot-scope="{ row, index }" slot="WaybillID">{{row.ID}}</template>

        <template slot-scope="{ row, index }" slot="OrderID" >
          <p v-if="warehouseID.indexOf('SZ')==-1">
             <span v-if="row.LotNumber!=''">{{row.LotNumber}}</span>
             <span v-else>{{row.OrderID}}</span>
          </p>
          <p v-else>
            <span>{{row.OrderID}}</span>
            <!-- <span v-else>{{row.ID}}</span> -->
          </p>
          
        </template>
        <template slot-scope="{ row, index }" slot="Sourcetype">
          <Tag v-if="row.Source==10" color="#009688">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==20" color="#2F4056">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==30" color="#FF5722">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==40" color="#1E9FFF">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==50" color="#FFA2D3">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==60" color="#FFA2D3">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==35" color="volcano">{{row.SourceDescription}}</Tag>
        </template>
        <template slot-scope="{ row, index }" slot="Supplier">
          <span v-if="warehouseID.indexOf('SZ')!=-1">
            <em v-if="row.LotNumber!=''">{{row.LotNumber}}</em>
            <em v-else>{{row.OrderID}}</em>
          </span>
          <span v-else> {{row.Supplier}}</span>
         
          </template>
        <template slot-scope="{ row, index }" slot="WaybillTypeDescription">{{row.WaybillTypeDescription}}</template>
        <template slot-scope="{ row, index }" slot="CreateDate">{{row.CreateDate|showDateexact}}</template>
        <template slot-scope="{ row, index }" slot="ModifyDate"> {{row.ModifyDate|showDateexact}} </template>
        <template slot-scope="{ row, index }" slot="Merchandiser">{{row.Merchandiser}}</template>
        <template slot-scope="{ row, index }" slot="action">
          <Button v-if="row.ExcuteStatus==215"  size="small" @click="showDrawer(row.ID,row.Source)">查看</Button>
          <Button v-else type="primary" size="small" @click="showDrawer(row.ID,row.Source)">处理</Button>
        </template>
      </Table>
      <div class="pages">
        <!-- <Page
          :total="Total"
          :page-size="search_data.PageSize"
          :current="search_data.PageIndex"
          @on-change="changepage"
        /> -->
        <Page :total="Total" :page-size="search_data.PageSize" 
          show-total
          :current="search_data.PageIndex" 
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize" 
          @on-change="changepage"  
          show-elevator 
          show-sizer />
      </div>
    </div>
    <Drawer
      :closable="false"
      v-model="showdetail"
      :width="90"
      :scrollable="true"
      :mask-closable="false"
    >
      <div class="closDrawer_btn">
        <Icon @click="closDrawer" type="md-close" class="icon1" />
      </div>
      <div>
        <router-view></router-view>
      </div>
    </Drawer>
  </div>
</template>
<script>
import Customswindow from "@/Pages/Common/Customswindow";
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint
} from "@/js/browser.js";
import { NoticeSource, outStatus } from "../../api";
import { pickingsout } from "../../api/CgApi";
let product_url = require("../../../static/pubilc.dev");
import moment from "moment";
export default {
  inject: ["reload"],
  name: "Outgoing",
  components: {
    "Customs-window": Customswindow
    //  "out-details":outdetails
  },
  data() {
    return {
      warehouseID:sessionStorage.getItem("UserWareHouse"),
      isshowstate: true, //是否显示状态
      search_data2: {
        warehouseid: sessionStorage.getItem("UserWareHouse"), //库房编号
        PageIndex: 1, //当前页面
        PageSize: 10, //当前页数
        CustomName: "", //客户
        status: "", //状态
        key: "", //订单号
        partnumber: "", //型号
        source: 1, //业务类型
        startdate: "", //开始时间
        enddate: "" //结束时间
      },
      search_data: {
        WareHouseID: sessionStorage.getItem("UserWareHouse"),//库房ID
        Key: "",//订单号/运单号
        Client: "",//客户名称
        ProductPartNumber: "",//产品型号
        WaybillExcuteStatus:'',//使用原有枚举
        Source: 1,//..业务类型(枚举)
        StartDate: "",//yyyy-MM-dd
        EndDate: "",//yyyy-MM-dd
        PageSize: 20,
        PageIndex: 1
      },
      testarrstate: [10],
      istagall: 0,
      printurl: product_url.pfwms,
      Total: 0, //总数
      PageIndex: 1,
      pagesize: 10,
      loading: true,
      // showdetail:false,
      // showtype:0,
      timer3: 0,
      Printing_data: "all_Printing", //打印的
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
        Conveyorsite: "美国" //输送地,
      },
      // WaybillNo: "", //运单号（快递到货）、提货单号（自提）、送货单号（送货上门）
      CustomName: "", //部门
      Supplier: "", //供应商
      model1: "", //状态
      orderID: "", //订单号
      Model: "", //型号
      startdat: "", //开始时间
      enddate: "", //结束时间
      source: "", //业务类型
      NoticeStatus: [], // 状态数组
      NoticeType: [], //业务类型状态
      columns1: [
        {
          title:"运单ID",
          slot: "WaybillID",
          align: "center",
          width: 155
        },
        {
          title: "订单号",
          slot: "OrderID",
          align: "center",
          width: 150
          // fixed: 'right'
        },
        {
          title: "业务类型",
          slot: "Sourcetype",
          align: "center"
        },
        {
          title: "送货方式",
          slot: "WaybillTypeDescription",
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
          align: "CarrierName"
        },
        {
          title: "供应商",
          slot: "Supplier",
          align: "center"
        },
        {
          title: "客服人员",
          slot: "Merchandiser",
          align: "center"
        },
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
      data1: [],
      value1: false,
      settimeouts: null,
      timenmae:null,
      tableHeight:500,
    };
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
  computed: {
    showdetail() {
      if (this.$store.state.common.showdetailout == true) {
        //  this.clear_time()
        // this.settimeouts = null;
      } else {
        // this.new_time()       
      }
      return this.$store.state.common.showdetailout;
    },
    showtype() {
      return this.$store.state.common.showtypeout;
    },
    showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  created() {
    if (this.$route.params.id == 10) {
      this.testarrstate = [10];
      this.search_data.WaybillExcuteStatus = this.testarrstate.join(",");
      this.isshowstate = true;
      var data = {
        title: "制单时间",
        slot: "CreateDate",
        align: "center"
      }
      this.columns1.splice(this.columns1.length - 1, 0, data); //

    } else {
      this.testarrstate = [215];
      this.search_data.WaybillExcuteStatus = this.testarrstate.join(",");
      this.isshowstate = false;

      var data = {
        title: "处理时间",
        slot: "ModifyDate",
        align: "center"
      }
      this.columns1.splice(this.columns1.length - 1, 0, data); //

    }
  },
  beforeRouteUpdate(to,from,next){
        if(from.fullPath=='/Outgoing/10'&&to.fullPath=='/Outgoing/215'){
             this.reload(); //刷新当前页面  
        }else if(from.fullPath=='/Outgoing/215'&&to.fullPath=='/Outgoing/10'){
            this.reload(); //刷新当前页面 
        }else{
          clearInterval(this.timenmae)
        }
         next()
  },
  watch:{
    $route(to,from){ //监听路由变化，根据不同路由刷新当前页面
    console.log(to.path)
    var that=this
    if(to.path=='/Outgoing/10'||to.path=='/Outgoing/215'){
        this.pickingsout(this.search_data)
        // this.new_time()
    }else{
      this.clear_time()
    }
    // if(to.path=='/CgProcessed'&&this.showdetail==false){
    //        this.reload()
    //   }else if(to.path=='/CgUntreated'&&this.showdetail==false){
    //        this.reload()
    //   }
    }
},
  mounted() {
    this.setnva();
    this.getstatus();
    this.pickingsout(this.search_data)
    if(this.warehouseID.indexOf('SZ')!=-1){
       this.columns1[1].title='订单号'
       this.columns1[7].title='运输批次号'
    }else{
      this.columns1[1].title='订单号 / 运输批次'
      this.columns1[7].title='供应商'
    }
   this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
    // this.new_time()
  },

  methods: {
    new_time(){  //创建实时请求
      var that=this
      this.timenmae=setInterval(() => {
        setTimeout(()=>{
         that.pickingsout(that.search_data) 
          }, 0)
      }, 3000)
    },
    clear_time(){ //销毁定时器
      clearInterval(this.timenmae)
      this.timenmae=null;
    },
    dyncMount() {
      //点击按钮加载子组件  //加载申报窗页面
      this.$store.dispatch("setshowdetailout", true);
      this.$store.dispatch("setshowtype", 3);
      this.timer3 = new Date().getTime();
      this.$router.push({ path: "/Outgoing/Customswindow" });
    },
    showDrawer(id, type) {
      this.$store.dispatch("setshowdetailout", true);
        if (type == 30) {
          this.$store.dispatch("setshowtype", 2);
          this.$router.push({ path: "/Outgoing/OutDeclareDetail/" + id });
        } else if(type == 35&&this.warehouseID.indexOf('SZ')!=-1){
            this.$store.dispatch("setshowtype", 2);
            this.$router.push({ path: "/Outgoing/Szoutgoing/" + id });
        }else if(type == 35&&this.warehouseID.indexOf('HK')!=-1){
            this.$store.dispatch("setshowtype", 2);
            this.$router.push({ path: "/Outgoing/OutDeclareDetail/" + id });
        }
        else if(type == 60){
          if(this.warehouseID.indexOf('HK')==-1){
            this.$store.dispatch("setshowtype", 2);
            this.$router.push({ path: "/Outgoing/OutDeclareDetail/" + id });
          }else{
            this.$store.dispatch("setshowtype", 2);
            this.$router.push({ path: "/Outgoing/TurnDeclareDetail/" + id });
          }
        }
        else {
          this.$store.dispatch("setshowtype", 1);
          this.$router.push({
            name: 'outdetail',
            params: {
              wayBillID: id
            }
          })
        }
      
      
    },
    closDrawer() {
      //关闭抽屉的方法
      // if (this.showtype == 1 || this.showtype == 3 || this.showtype == 2) {
      //   // this.$router.go(-1);   //控制路由跳回原来页面
      //   this.$router.push({ path: "/Outgoing" });
      // }
      this.$router.go(-1);
      // this.pickingsout(this.search_data);
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
    outorderprint() {
      //打印出库通知
      var printsrr = [{ name: "1111" }];
      var configs = GetPrinterDictionary();
      var getsetting = configs["出库通知打印"];
      var href = window.location.protocol + "//" + window.location.host;
      var newurl = "http://hv.warehouse.b1b.com" + getsetting.Url;
      getsetting.Url = newurl;
      var data = {
        setting: getsetting,
        data: printsrr
      };
      TemplatePrint(data);
    },
    serch_btn() { //搜索按钮
      if (
        this.search_data.Client != "" ||
        this.search_data.Key != "" ||
        this.search_data.ProductPartNumber != "" ||
        this.search_data.Source != "" ||
        this.search_data.EndDate != "" ||
        this.search_data.EndDate != ""
      ) {
        this.search_data.PageIndex = 1;
        this.pickingsout(this.search_data);
        this.loading = true;
      } else {
        this.$Message.error("请至少输入一个查询条件");
      }
    },
    clear_btn() {
      if (
        this.search_data.Client != "" ||
        this.search_data.Key != "" ||
        this.search_data.ProductPartNumber != "" ||
        this.search_data.Source != "" ||
        this.search_data.EndDate != "" ||
        this.search_data.EndDate != ""
      ) {
        this.loading = true;
        var data=this.testarrstate.join(",")
        this.search_data = {
          WareHouseID: sessionStorage.getItem("UserWareHouse"),//库房ID
          Key: "",//订单号/运单号
          Client: "",//客户名称
          ProductPartNumber: "",//产品型号
          WaybillExcuteStatus:data,//使用原有枚举
          Source: 1,//..业务类型(枚举)
          StartDate: "",//yyyy-MM-dd
          EndDate: "",//yyyy-MM-dd
          PageSize: 10,
          PageIndex: 1
        };
        this.$refs.element.handleClear();
        this.pickingsout(this.search_data);
        // warehouseid, key, partnumber, CustomName, startdate, enddate, status, source, ntype, pageindex, pagesize
      }
    },
    changedata(value) {
      //时间格式 获取开始时间与结束时间
      this.search_data.StartDate = value[0];
      this.search_data.EndDate = value[1];
    },
    changepage(value) {
      this.loading = true;
      this.PageIndex = value;
      this.search_data.PageIndex = value;
      this.pickingsout(this.search_data);
    },
    changepagesize(value){
      this.loading = true;
      this.PageIndex = value;
      this.search_data.PageSize = value;
      this.pickingsout(this.search_data);
    },
    pickingsout(data) {
      var _this = this;
      pickingsout(data).then(res => {
        if (res.obj.Data != []) {
          this.loading = false;
          this.data1 = res.obj.Data;
          this.Total = res.obj.Total;
        } else {
        }
      });
    },
    getstatus() {
      //获取通知状态与业务类型
      NoticeSource().then(res => {
        //业务类型
        //  console.log(res)
        this.NoticeType = res.obj;
      });
      outStatus().then(res => {
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
    sumbit_print() {
      //运单打印功能
      var configs = GetPrinterDictionary();
      var getsetting = configs["装箱单打印"];
      // var href=window.location.protocol+"//"+window.location.host;
      var str = getsetting.Url;
      var testurl = str.indexOf("http") != -1;
      if (testurl == true) {
        getsetting.Url = getsetting.Url;
      } else {
        getsetting.Url = this.printurl + getsetting.Url;
      }
      var data = {
        setting: getsetting,
        data: [
          {
            name: "11111"
          }
        ]
      };
      TemplatePrint(data);
    },
    testarrs(item) {
      if (item == 10) {
        if (this.testarrstate.indexOf(item) == -1) {
          this.testarrstate = [];
          this.testarrstate.push(item);
          this.istagall = true;
        } else {
          // this.testarrstate = [];
          this.istagall = false;
        }
      } else {
        this.testarrstate = this.testarrstate.filter(function(item2) {
          return item2 != 10;
        });
        this.istagall = false;
        if (this.testarrstate.indexOf(item) == -1) {
          this.testarrstate.push(item);
        } else {
          this.testarrstate = this.testarrstate.filter(function(item2) {
            return item2 != item;
          });
          if (this.testarrstate.length == 0) {
            this.testarrstate.push(10);
          }
        }
      }
      var cc = this.testarrstate.join(",");
      this.search_data.WaybillExcuteStatus=this.testarrstate.join(",")
      this.serch_btn()
    },
    closeTimer() {
      //关闭定时器
      window.clearInterval(this.settimeouts);
      this.settimeouts = null;
    },
  },
  beforeDestroy() {
   this.clear_time()
  },
  destroyed() {
   this.clear_time()
  }
};
</script>
