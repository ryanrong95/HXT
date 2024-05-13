<style scoped>
  #warehousing >>> .ivu-table-cell {
    padding-left: 5px;
    padding-right: 5px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
    word-break: break-all;
    box-sizing: border-box;
  }

  .inputwidth {
    margin-right: 20px;
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

  label {
    font-size: 14px;
    line-height: 32px;
    /* vertical-align: text-bottom; */
  }
</style>
<template>
  <div id="warehousing">
    <!--暂时隐藏-->
    <!--<div class="shaixuanbox" v-if="isshowstate==true">
      <em class="title">快速筛选：</em>
      <span :class="(testarrstate.indexOf(item.value)!==-1&&!istagall)||item.value==10&&istagall?'item_status':'item_status2'"
            v-for="item in NoticeStatus"
            :key="item.value"
            @click="testarrs(item.value)">{{item.name }}</span>
    </div>-->
    <div>
      <p class="inputwidth">
        <label for="订单号">订单号：</label>
        <Input v-model.trim="search_datas.OrderID"
               placeholder
               autofocus
               @on-enter="search_btn"
               class="inputitems" />
      </p>
      <p class="inputwidth">
        <label for="入仓号">入仓号：</label>
        <Input v-model.trim="search_datas.ClientCode"
               placeholder
               @on-enter="search_btn"
               class="inputitems" />
      </p>
      <p class="inputwidth">
        <label for="供应商">供应商</label>
        <Input v-model.trim="search_datas.SupplierName"
               placeholder
               @on-enter="search_btn"
               class="inputitems" />
      </p>
      <p class="inputwidth">
        <label for="产品型号">产品型号：</label>
        <Input v-model.trim="search_datas.Model"
               placeholder
               @on-enter="search_btn"
               class="inputitems" />
      </p>    
      <p class="inputwidth">
        <label for="到货方式">到货方式：</label>
        <Select v-model.trim="search_datas.HKDeliveryType" placeholder class="inputitems">
          <Option v-for="item in WaybillTypeArr" :value="item.value" :key="item.value">{{ item.name }}</Option>
        </Select>
      </p>
      <p class="inputwidth">
        <label for="快递单号">快递单号：</label>
        <Input v-model.trim="search_datas.WaybillNo"
               placeholder
               @on-enter="search_btn"
               class="inputitems" />
      </p>    
      <div class="inputwidth">
        <label for="业务类型">开始时间与结束时间：&nbsp;&nbsp;</label>
        <DatePicker ref="element"
                    type="daterange"
                    placement="bottom-end"
                    placeholder
                    :editable="false"
                    separator="  至  "
                    @on-change="changedata"
                    style="width:200px;"></DatePicker>
      </div>
      <div style="float: left;">
        <Button type="primary" icon="ios-search" @click="search_btn" style="margin-right:10px">查询</Button>
        <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
      </div>
    </div>
   
    <div style="width:100%;display: inline-block; margin-top:20px;">
    

      <Table ref="table" :max-height="tableHeight"
             :columns="Notice_title"
             :data="Notice_list"
             :loading="loading"
             @on-row-dblclick="tabledblclick">
        <template slot-scope="{ row, index }" slot="OrderID">         
          {{row.OrderID}}
        </template>
       
        <template slot-scope="{ row, index }" slot="Type">
          <span>{{row.Type}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="EntryNumber" >
          <span>{{row.EnterCode==null?row.EntryNumber:row.EntryNumber+"/"+row.EnterCode}}</span>
        </template>
   
        <template slot-scope="{ row, index }" slot="SupplierName">
          <span>{{row.SupplierName}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="WayBillNo">        
          {{row.WayBillNo}}
        </template>
        <template slot-scope="{ row, index }" slot="Status">        
          {{row.Status}}
        </template>
        <template slot-scope="{ row, index }" slot="CreateDate">
          {{row.UpdateDate}}
        </template>      
        <template slot-scope="{ row, index }" slot="action">
          <Button :disabled="false"
                  size="small"
                  @click="showDrawer(row.ID,row.OrderID)">
            查看
          </Button>
        </template>
      </Table>
      <div class="pages">
        <!-- <Page :total="total"  @on-change="changepage" :page-size="10" :current="search_datas.PageIndex" /> -->
        <Page :total="total" :page-size="search_datas.PageSize"
              show-total
              :current="search_datas.PageIndex"
              :page-size-opts="showPageArr"
              @on-page-size-change="changepagesize"
              @on-change="changepage"
              show-elevator
              show-sizer />
      </div>
    </div>
    <Drawer title
            :closable="false"
            v-model="showdetail"
            :width="90"
            :scrollable="true"
            :mask-closable="false"
            @on-visible-change="changeDrawer">
      <div>
        <div slot="close" class="closDrawer_btn">
          <!-- <Icon @click="closDrawer" type="ios-close-circle-outline" /> -->
          <!-- <Icon type="md-close" /> -->
          <Icon @click="closDrawer" type="md-close" class="icon1" />
        </div>
        <routine-enter v-if="showtype==1" ref="routineenter" :key="timer1"></routine-enter>
        <router-view v-else></router-view>
      </div>
    </Drawer>
  </div>
</template>
<script>
import Vue from "Vue";
import { NoticeSource, } from "../../api"; //引入api 的接口
import { EntryNoticeStatus,SealedList,HKDeliveryType } from "../../api/XdtApi"; //引入XDT api 的接口
import moment from "moment";
import RoutineEnter from "@/Pages/Cgenter/RoutineEnter"; //代入库
import Declare from "@/Pages/Common/Declare"; //代报关
import imgtest from "@/Pages/Common/imgtes"; //图片上传
// import Customswindow from "@/Pages/Cgenter/Customswindow";
export default {
  inject: ["reload"],
  name: "SealedList",
  components: {
    "routine-enter": RoutineEnter,
    Declare: Declare,
    "img-test": imgtest
    // "Customs-window": Customswindow
  },
  data() {
    return {
      routername: "",
      isshowstate: true,
      testarrstate: [10],
      istagall: 0,
      timer1: "",
      timer2: "",
      timer3: "",
      loading: true, //loading效果
      warehouseID: sessionStorage.getItem("UserWareHouse"),
      search_datas: {
        //新的搜搜
        OrderID: "",
        ClientCode: "",
        SupplierName: "",
        HKDeliveryType:"",
        Model:"",
        StartDate: "",
        EndDate: "",
        PageSize: 10,
        PageIndex: 1,
        WhID: sessionStorage.getItem("UserWareHouse"),
        WaybillNo:"",
      },
      WaybillTypeArr:[],
      Notice_title: [
        // {
        //   type: "selection",
        //   width: 60,
        //   align: "center"
        // },
        {
          title: "订单号",
          slot: "OrderID",
          align: "center",
          width: 180
        },
        {
          title: "到货方式",
          slot: "Type",
          align: "center"
        },
        {
          title: "入仓号",
          slot: "EntryNumber",
          align: "center"
        },
         {
          title: "供应商",
          slot: "SupplierName",
          align: "center"
        },
        {
          title: "快递单号",
          slot: "WayBillNo",
          align: "center"
        },
        {
          title: "状态",
          slot: "Status",
          align: "center"
        },
        {
          title: "封箱时间",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "操作",
          slot: "action",
          align: "center"
          // fixed: 'right'
        }       
        // {
        //   title: "运单号",
        //   slot: "Code",
        //   align: "center"
        // },
        // {
        //   title: "承运商",
        //   slot: "CarrierName",
        //   align: "center"
        // },
        // {
        //   title: "客服人员",
        //   slot: "Merchandiser",
        //   align: "center"
        // },
        
      ],
      Notice_list: [], //通知列表

      // showdetail: false,//左侧抽屉是否显示
      // showtype:0,//显示处理的视图 1 代收货  2代报关  4暂存  5出库详情
      Showdetailid: "",
      NoticeStatus: [],
      NoticeType: [],
      total: 0,
      setIntervaltimer: null,
      timenmae: null,
      routetype: "",
      tableHeight:500,
    };
  },
  created() {
    this.CgWaybillType()
    this.routername = this.$route.name;   
    this.clear_time();
    window["setIntervaltimer"] = this.setIntervaltimer;
    this.cgSearch_sortings(this.search_datas);
  },
  computed: {
    getid() {
      return this.Showdetailid;
    },
    gethouse() {
      return sessionStorage.getItem("UserWareHouse");
    },
    showdetail() {
      if (this.$store.state.common.showdetailHK == true) {
        this.clear_time();
        this.timenmae = null;           
      } else {
        var _this = this;
      }
      return this.$store.state.common.showdetailHK;
    },
    showtype() {
      return this.$store.state.common.showtypein;
    },
    showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  mounted() {
    this.setnva();
    this.getstatus();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
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
    CgWaybillType(){
      HKDeliveryType().then(res=>{       
        this.WaybillTypeArr=res.obj
      })
    },
    tabledblclick(row, index) {    
      if(row.Waybill.Operated==true){
        this.showDrawer(row.ID, row.OrderID);
      }

    },
    new_time() {
      //创建实时请求
      var that = this;
      this.timenmae = setInterval(() => {
        setTimeout(() => {
          that.cgSearch_sortings(that.search_datas);
        }, 0);
      }, 2000);
    },
    clear_time() {
      //销毁定时器
      clearInterval(this.timenmae);
      this.timenmae = null;
    },
    dyncMount() {
      //点击按钮加载子组件
      // this.showdetail = true;
      this.$store.dispatch("setshowdetail", true);
      // this.showtype=3;
      this.$store.dispatch("setshowtypein", 3);
      this.timer = new Date().getTime();
    },
    showDrawer(id,orderID) {
      //  this.$router.push({
      //     name: "SealedInfo",
      //     params: {
      //       entryNoticeID: id,
      //       orderID:orderID,
      //       mainOrderID:orderID.slice(0,16)
      //     }
      //   });
        this.$store.dispatch("setshowdetailHK",true);       
        this.$router.push({ name: 'SealedInfo', params: { entryNoticeID: id, orderID:orderID,mainOrderID:orderID.slice(0,16)} });
    },

    closDrawer() {
      this.$router.go(-1); //控制路由跳回原来页面
      var _this = this;
      this.$store.dispatch("setshowdetailHK", false);     
    },
    changepage(value) {
      this.loading = true;
      this.search_datas.PageIndex = value;
      this.cgSearch_sortings(this.search_datas);     
    },
    changepagesize(value){
       this.loading = true;
       this.search_datas.PageSize = value;
       this.search_datas.PageIndex=1;
       this.cgSearch_sortings(this.search_datas);
    },
    changedata(value) {
      this.search_datas.StartDate = value[0];
      this.search_datas.EndDate = value[1];
    },
    //根据搜索条件查询
    search_btn() {
      if (
        this.search_datas.key != "" ||
        this.search_datas.WaybillExcuteStatus != "" ||
        this.search_datas.WaybillSupplier != "" ||
        this.search_datas.StartDate != "" ||
        this.search_datas.EndDate != "" ||
        this.search_datas.ProductPartNumber != ""||
        this.search_datas.WaybillNo != "") {
        this.loading = true;
        this.search_datas.PageIndex = 1;
        this.cgSearch_sortings(this.search_datas);
        // console.log(this.search_datas)
      } else {
        this.$Message.error("请至少输入一个查询条件");
      }
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
        this.search_datas.WaybillNo != "" ) {
        this.search_datas = {
          WhID: sessionStorage.getItem("UserWareHouse"),
          key: "", //运单号(可以为空)  入仓号
          WaybillSupplier: "", //供应商(可以为空)
          WaybillExcuteStatus: this.testarrstate.join(","), //通知状态(可以为空)
          StartDate: "", //开始时间(可以为空)
          EndDate: "", //结束时间(可以为空)
          ProductPartNumber: "", //型号(可以为空)
          WaybillNo : "",
          Source: 1, //入库类型(可以为空)source
          WaybillType:0,//到货方式
          PageIndex: 1, //当前页码
          PageSize: 10 //每页条数
        };
        this.loading = true;
        this.search_datas.PageIndex = 1;
        this.$refs.element.handleClear();
        this.cgSearch_sortings(this.search_datas);
        var _this = this;
      }
    },   
    getstatus() {
      //获取通知状态与业务类型
      NoticeSource().then(res => {
        //业务类型
        this.NoticeType = res.obj;
      });
      EntryNoticeStatus().then(res => {
        this.NoticeStatus = res.obj;
        var Statusarr = res.obj;
      });
    },
    setnva() {
      var cc = [
        {
          title: "已装箱列表",
          href: "/SealedList"
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
      this.search_datas.WaybillExcuteStatus = this.testarrstate.join(",");
      this.search_btn();
      // console.log(this.search_datas.WaybillExcuteStatus)
    },
    // 重构 初始化获取数据
    cgsortings() {
      SealedList(data).then(res => {
        // console.log(res)
        this.total = res.obj.total;
        this.Notice_list = res.obj.rows;
        this.loading = false;
      });
    },
    // 搜索获取数据的方法
    cgSearch_sortings(data) {      
      SealedList(data).then(res => {
        // console.log(res)
        this.total = res.obj.total;
        this.Notice_list = res.obj.rows;
        this.loading = false;      
      });
    },
    changeDrawer(value) {
      if (value == true) {
        this.clear_time();
        this.timenmae = null;
      } else {
        this.cgSearch_sortings(this.search_datas);
        //  this.new_time()
      }
    },
  },

  destroyed() {
    
  }
};
</script>
