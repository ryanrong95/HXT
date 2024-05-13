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
  float: left;
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
    <!--暂时隐藏-->
    <!-- <div class="shaixuanbox">
      <em class="title">快速筛选：</em>
      <span
        :class="(testarrstate.indexOf(item.value)!==-1&&!istagall)||item.value==0&&istagall?'item_status':'item_status2'"
        v-for="item in NoticeStatus"
        :key="item.value"
        @click="testarrs(item.value)"
      >{{item.name }}</span>
    </div> -->
    <div>
      <Input
        v-model.trim="search_datas.key"
        placeholder="运单号/入仓号/订单号"
        autofocus
        @on-enter="search_btn"
        class="inputwidth"
      />
      <!-- <Input v-model="department" placeholder="部门" class="inputwidth" /> -->
      <Input
        v-model.trim="search_datas.supplier"
        placeholder="供应商"
        @on-enter="search_btn"
        class="inputwidth"
      />
      <Input
        v-model.trim="search_datas.partnumber"
        placeholder="产品型号"
        @on-enter="search_btn"
        class="inputwidth"
      />

      <Select v-model.trim="search_datas.status" placeholder="请选择状态" class="inputwidth">
        <Option v-for="item in NoticeStatus" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select>
      <Select v-model.trim="search_datas.source" placeholder="业务类型" class="inputwidth">
        <Option v-for="item in NoticeType" :value="item.value" :key="item.value">{{ item.name }}</Option>
      </Select>
      <div class="inputwidth">
        <DatePicker
          ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder="选择开始与结束时间"
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
      <!-- 暂时隐藏 申报窗口 -->
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

      <Table :columns="Notice_title" :data="Notice_list" :loading="loading">
        <template slot-scope="{ row, index }" slot="ID">
          <span>{{row.WaybillID}}</span>
          <Tag v-if="row.Source==10" color="#009688">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==20" color="#2F4056">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==30" color="#FF5722">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==40" color="#1E9FFF">{{row.SourceDescription}}</Tag>
          <Tag v-if="row.Source==50" color="#FFA2D3">{{row.SourceDescription}}</Tag>
        </template>
        <template slot-scope="{ row, index }" slot="CreateDate">
          <span>{{row.CreateDate|showDate}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button disabled v-if="row.OStatusDes==200" @click="showDrawer(row.WaybillID,row.Source)">查看</Button>
          <Button type="primary" size="small" v-else @click="showDrawer(row.WaybillID,row.Source)">处理</Button>
        </template>
      </Table>
      <div class="pages">
        <Page :total="total" @on-change="changepage" :page-size="10" :current="search_datas.PageIndex" />
      </div>
    </div>
    <Drawer
      title
      :closable="false"
      v-model="showdetail"
      :width="80"
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
import moment from "moment";
import RoutineEnter from "@/Pages/Common/RoutineEnter"; //代入库
import Declare from "@/Pages/Common/Declare"; //代报关
import imgtest from "@/Pages/Common/imgtes"; //图片上传
import Customswindow from "@/Pages/Common/Customswindow";
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
      search_datas: {
        warehouseid: sessionStorage.getItem("UserWareHouse"),
        key: "", //运单号(可以为空)  入仓号
        supplier: "", //供应商(可以为空)
        status: "", //通知状态(可以为空)
        // alltime:"",
        startdate: "", //开始时间(可以为空)
        enddate: "", //结束时间(可以为空)
        partnumber: "", //型号(可以为空)
        source: "", //入库类型(可以为空)source
        PageIndex: 1, //当前页码
        PageSize: 10 //每页条数
      },
      // string warehouseid,string key=null, string partnumber = null,
      // string supplier=null,string startdate=null,string enddate=null,int status=0, int source = 0, int pageindex = 1, int pagesize = 20
      Notice_title: [
        // {
        //   type: "selection",
        //   width: 60,
        //   align: "center"
        // },
        {
          title: "ID",
          slot: "ID",
          align: "left",
          width: 250
        },
        {
          title: "通知时间",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "到货方式",
          key: "WaybillTypeDescription",
          align: "center"
        },
        {
          title: "入仓号",
          key: "EnterCode",
          align: "center"
        },
        {
          title: "运单号",
          key: "Code",
          align: "center",
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
        //   title: "重量(g)",
        //   key: "weight"
        // },
        // {
        //   title: "来源",
        //   key: "source"
        // },
        {
          title: "状态",
          key: "ExcuteStatusDescription"
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
    };
  },
  created () {
    if(this.setIntervaltimer){
        clearTimeout(this.setIntervaltimer);
        this.setIntervaltimer=null;
    }
   window['setIntervaltimer'] =this.setIntervaltimer;
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
         clearInterval(this.setIntervaltimer);
         this.setIntervaltimer=null;
      }else{
         var _this = this;
         _this.wareing(_this.search_datas); 
        this.setIntervaltimer = setInterval(()=> {
          _this.wareing(_this.search_datas); //实时获取通知列表
        }, 6000);
          this.wareing(this.search_datas)
      }
      return this.$store.state.common.showdetail;
    },
    showtype() {
      return this.$store.state.common.showtypein;
    }
  },
  mounted() {
    this.setnva();
    var datas = {
      warehouseID: this.warehouseID, //"BJ04",//this.warehouseID,
      status: this.search_datas.status,
      PageIndex: this.search_datas.PageIndex, //当前页码
      PageSize: 10 //每页条数
      // source:
    };
    this.wareing(datas); //获取通知列表
    this.getstatus();
    // var _this = this;
    // this.setIntervaltimer = setInterval(()=> {
    //    _this.wareing(_this.search_datas); //实时获取通知列表
    //  }, 6000);
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
//  beforeRouteLeave (to, from , next) {
//   clearTimeout(this.setIntervaltimer);
//   next()
// },
//  beforeRouteUpdate (to, from, next) {
//     // console.log(this)
//     console.log(to.path)
//     if(to.path=='/Warehousing'){
//         this.pickingsout(this.search_data)
//     }else{
//        clearTimeout(this.setIntervaltimer);
//     }
//     next()
//     // 在当前路由改变，但是该组件被复用时调用
//     // 举例来说，对于一个带有动态参数的路径 /foo/:id，在 /foo/1 和 /foo/2 之间跳转的时候，
//     // 由于会渲染同样的 Foo 组件，因此组件实例会被复用。而这个钩子就会在这个情况下被调用。
//     // 可以访问组件实例 `this`
// },
  methods: {
    fatherMethod() {
      // this.showdetail = false;
      this.$store.dispatch("setshowdetail", false);
      // this.showtype=0;
      this.$store.dispatch("setshowtypein", 0);
      var datas = {
        warehouseID: this.warehouseID,
        status: this.search_datas.status,
        PageIndex: this.search_datas.PageIndex, //当前页码
        PageSize: 10 //每页条数
      };
      this.wareing(datas); //获取通知列表
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
      clearInterval(this.setIntervaltimer);//实时获取通知列表  在处理页面停止实时请求
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
        this.$router.push({ path: "/Warehousing/Customswindow" });
      } else {
        // this.showtype=1; //待收货 常规入库
        this.$store.dispatch("setshowtypein", 1);
        this.timer1 = new Date().getTime();
        var that = this;
        setTimeout(function() {
          that.$refs.routineenter.WayParterdata(); //输送地列表
          // that.$refs.Declare.Carriers();            //承运商
          that.$refs.routineenter.getdetail_data(id);
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
       _this.wareing(_this.search_datas); 
    //   this.setIntervaltimer = setInterval(()=> {
    //    _this.wareing(_this.search_datas); //实时获取通知列表
    //  }, 6000);
      this.$store.dispatch("setshowdetail", false);
      // console.log(this.showtype)
    },
    changepage(value) {
      this.loading=true;
      this.search_datas.PageIndex = value;
      this.wareing(this.search_datas);
    },
    changedata(value) {
      //时间格式 获取开始时间与结束时间
      // console.log(value)
      this.search_datas.startdate = value[0];
      this.search_datas.enddate = value[1];
    },
    search_btn() {
      //根据搜索条件查询
      // console.log(this.search_datas)
      if (
        this.search_datas.key != "" ||
        this.search_datas.status != "" ||
        this.search_datas.supplier != "" ||
        this.search_datas.startdate != "" ||
        this.search_datas.enddate != "" ||
        this.search_datas.partnumber != "" ||
        this.search_datas.source != ""
      ){
         this.loading=true;
         this.search_datas.PageIndex=1;
         this.wareing(this.search_datas);
      }else{
        this.$Message.error('请至少输入一个查询条件');
      }
      
      // clearInterval(this.setIntervaltimer);
    },
    clear_btn() {
      //清空搜索条件
      if (
        this.search_datas.key != "" ||
        this.search_datas.status != "" ||
        this.search_datas.supplier != "" ||
        this.search_datas.startdate != "" ||
        this.search_datas.enddate != "" ||
        this.search_datas.partnumber != "" ||
        this.search_datas.source != ""
      ) {
        this.search_datas = {
          warehouseid: sessionStorage.getItem("UserWareHouse"),
          key: "", //运单号(可以为空)  入仓号
          supplier: "", //供应商(可以为空)
          status: "", //通知状态(可以为空)
          // alltime:"",
          startdate: "", //开始时间(可以为空)
          enddate: "", //结束时间(可以为空)
          partNumberr: "", //型号(可以为空)
          source: "", //入库类型(可以为空)source
          PageIndex: 1, //当前页码
          PageSize: 10 //每页条数
        };
        this.loading=true;
        this.search_datas.PageIndex=1;
        this.search_datas.status = "";
        this.$refs.element.handleClear();
        this.wareing(this.search_datas);
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
      this.$router.push({ path: "/Warehousing/separate" });
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
      console.log(cc)
    }
  },
  beforeDestroy() {
        console.log("入库组件被销毁")
        clearInterval(this.setIntervaltimer);
        this.setIntervaltimer=null;
		},
  destroyed() {
      clearInterval(this.setIntervaltimer);
      this.setIntervaltimer=null;
  }
};
</script>
