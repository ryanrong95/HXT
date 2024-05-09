<style scpoed>
.pagebox {
  text-align: right;
  padding: 10px 0;
}
.setinputs{
  width:12%;
  margin-right:20px;
  /* margin-bottom: 10px; */
}
</style>
<template>
  <div>
    <div>
      <label for="运单号">
        <span v-if="routedata=='/Separate/1'">订单号/运单号：</span>
        <span v-else>运单号：</span>
      </label>
      <Input
        v-model.trim="searchdata.Code"
        :placeholder="routedata=='/Separate/1'?'请输入订单号/运单号':'运单号'"
       class="setinputs"
      />
      
      <label for="运单号">承运商：</label>
      <Select v-model="searchdata.CarrierID" placeholder class="setinputs">
        <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
      </Select>
      <label for="库位">库位：</label>
      <Input v-model.trim="searchdata.ShelveID" placeholder='库位'   class="setinputs"/>
      <!-- <label for="库位">型号：</label>
      <Input v-model.trim="searchdata.PartNumber" placeholder='型号'   class="setinputs"/> -->
      <label for="暂存录入时间：">暂存录入时间：</label>
      <DatePicker type="daterange"
        ref="element"
        split-panels 
        placeholder="请选择一个时间段" 
        class="setinputs"
        :clearable='false'
        @on-change='changeDatePicker'>
       </DatePicker>
      <Button type="primary" icon="ios-search" @click="search_btn">查询</Button>
      <Button type="error"  icon="ios-trash"  @click="clear_btns">清空</Button>
    </div>
    <div style="padding-top:40px;">
      <Table :columns="titles" :data="listdata" :loading="loading" ref="table" :max-height="tableHeight">
        <!-- <template slot-scope="{ row,index }" slot="indexs">
          <strong>{{ index+1}}</strong>
        </template> -->
         <template slot-scope="{ row,index }" slot="Code">
          <p>{{row.Waybill.Code}}</p>
        </template>
        <template slot-scope="{ row,index }" slot="OrderID">
          <p>{{row.Waybill.ForOrderID}}</p>
        </template>
         <!-- <template slot-scope="{ row,index }" slot="OrderType">
          <p>{{row.Waybill.TypeDes}}</p>
        </template> -->
        <template slot-scope="{ row,index }" slot="CarrierName">
          <p>{{row.Waybill.CarrierName}}</p>
        </template>
        <template slot-scope="{ row,index }" slot="EnterCode">
          <p>{{row.Waybill.EnterCode}}</p>
        </template>
        <template slot-scope="{ row,index }" slot="CreateDate">
          <span>{{row.Waybill.CreateDate|showDate}}</span>
        </template>
        <template slot-scope="{ row,index }" slot="TempDays">
          <span>{{row.Waybill.TempDays}}</span>
        </template>
        <template slot-scope="{ row,index }" slot="Summary">
          <span>{{row.Waybill.Summary}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <!-- <Button type="error" size="small" @click="remove(index)">删除</Button> -->
          <Button
            v-if="row.Waybill.Status==1"
            type="primary"
            size="small"
            style="margin-right: 5px"
            @click="show(row.Waybill.ID)"
          >修改</Button>
          <Button
            v-else
            type="primary"
            size="small"
            style="margin-right: 5px"
            @click="show(row.Waybill.ID)"
          >查看</Button>
        </template>
      </Table>
      <div class="pagebox">
        <Page
          show-total
          show-elevator 
          show-sizer
          :total="total"
          :page-size="searchdata.PageSize"
          :current="searchdata.pageindex"
          @on-change="changepage"
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize" 
        />
      </div>
    </div>
    <Drawer title v-model="Spearatedrawer" width="90" @on-visible-change='uploadlist'>
      <router-view v-if="Spearatedrawer==true"></router-view>
    </Drawer>
  </div>
</template>
<script>
import { Carriers } from "../../api";
import {SparateWaybill } from "../../api/CgApi";
import moment from "moment";
export default {
  inject: ["reload"],
  data() {
    return {
      CarrierList: [],
      loading: true,
      timer1: "",
      warehouseID: sessionStorage.getItem("UserWareHouse"),
      total: 0,
      PageIndex: 1,
      value: "",
      // Spearatedrawer: false,
      titles: [
        {
          type: 'index',
          width: 60,
          align: 'center'
        },
        {
          title: "订单号",
          slot: "OrderID"
        },
        {
          title: "运单号",
          slot: "Code"
        },
        // {
        //   title: "到货方式",
        //   slot: "OrderType"
        // },
        {
          title: "承运商",
          slot: "CarrierName"
        },
        {
          title: "暂存录入时间",
          slot: "CreateDate"
        },
        {
          title: "入仓号",
          slot: "EnterCode"
        },
        {
          title:"暂存天数",
          slot:'TempDays'
        },
        {
          title: "备注",
          slot: "Summary"
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center"
        }
      ],
      listdata: [ ],
      searchdata: {
        WhID: sessionStorage.getItem("UserWareHouse"),
        Code: "",
        CarrierID: "",
        ShelveID: "",
        StartTime: "",
        EndTime: "",
        Status:null,//状态,待处理 1，已处理 2，
        PageIndex: 1,
        PageSize: 20,



//           "WhID":"HK",    //库房ID
//     "Code":"",      //具体的运单号
//     "CarrierID":"",//承运商ID
//     "ShelveID":"", //库位号
//     "StartTime":"", //开始时间
//     "EndTime":"", //结束时间
//     "Status":null, //状态,待处理 1，已处理 2，如果不查这个状态的话就用null就可以（孙滢歌使用null就可以， 乔霞在获取列表时或者为1待处理， 或者为2已处理）
//     "PageIndex":1, //分页的序号
//     "PageSize":10  //分页的每页大小
      },
      tableHeight:500,
      routedata:null,
    };
  },
  beforeRouteUpdate(to,from,next){
    console.log(from.path)
    console.log(this.Spearatedrawer)
    // this.reload();
    if(from.path==="/Separate/1"&&this.Spearatedrawer==false){
      // this.searchData.Status=false
      this.reload(); //刷新当前页面
    }else if(from.path==="/Separate/0"&&this.Spearatedrawer==false){
      //  this.searchData.Status=true
       this.reload(); //刷新当前页面
    }
    next()
  },
  created(){
    this.routedata=this.$route.path
  },
  computed: {
    Spearatedrawer:{
       get(){
           return this.$store.state.common.Spearatedrawer;
        },
        set(val){}
      },
      showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  mounted() {
    this.Carriers();
    this.SparateWaybill(this.searchdata);
    this.setnva();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
    // console.log(this.$route.path)
   
  },
  methods: {
    setnva() {
      if(this.$route.path=='/Separate/0'){
        var cc = [
        {
          title: "待处理",
          href: ""
        }
       ];
       this.$store.dispatch("setnvadata", cc);
       this.searchdata.Status=1
       this.titles.splice(this.titles.findIndex(item => item.title === "订单号"), 1)
      }else{
         var cc = [
        {
          title: "已处理",
          href: ""
        }
       ];
       this.$store.dispatch("setnvadata", cc);
       this.searchdata.Status=2
       
      //  var data={
      //     title: "订单号",
      //     slot: "OrderID"
      //   }
      //   this.splice(index, 0, val);
      //   this.titles.push(data)
      }
     
    },
    // cgtempstockshow() {
    //   //初始化列表数据
    //   cgtempstockshow(this.warehouseID, 1, 10).then(res => {
    //     console.log(res);
    //     this.total = res.obj.Total;
    //     this.listdata = res.obj.Data;
    //     this.loading = false;
    //   });
    // },
    changeDatePicker(value){ //改变时间
        this.searchdata.StartTime=value[0]
        this.searchdata.EndTime=value[1]
    },
    clearsearch(){
      this.loading = true;
      this.SparateWaybill(this.searchdata);
    },
    uploadlist(value) {
      // console.log(value)
      if(value==false){
        this.loading = true;
        this.$store.dispatch("setSpearatedrawer", false);
        this.$router.go(-1); //控制路由跳回原来页面
        this.SparateWaybill(this.searchdata);
      }
      
    },
    
    show(index) {
      // this.Spearatedrawer = true;
      this.$store.dispatch("setSpearatedrawer", true);
      // /Separate/revise
      this.$router.push({
        path:this.$route.path+'/revise',
        query: {
          detailID:index,
        }
      });
      // this.timer1 = new Date().getTime();
      // var that = this;
      // setTimeout(function() {
      //   that.$refs.routineenter.Initialization(index); //初始化详情数据
      // }, 20);
    },
    changepage(value) {  //改变页码的时候调用
      this.loading = true;
      this.searchdata.PageIndex = value;
      this.SparateWaybill(this.searchdata);
    },
    changepagesize(value){
      this.loading = true;
      this.searchdata.PageSize = value;
      this.SparateWaybill(this.searchdata);
    },
    SparateWaybill(data) {
      SparateWaybill(data).then(res => {
        this.total = res.obj.Total;
        this.listdata = res.obj.Data;
        this.loading = false;
      });
    },
    Carriers() {
      //获取承运商列表
      Carriers().then(res => {
        this.CarrierList = res.obj;
      });
    },
    search_btn() {
      //搜索
      this.loading = true;
      this.searchdata.PageIndex=1;
      this.SparateWaybill(this.searchdata);
    },
    clear_btns() {
      //清空
      console.log(this.searchdata)
      this.searchdata.Code='';
      this.searchdata.CarrierID='';
      this.searchdata.ShelveID='';
      this.searchdata.StartTime='';
      this.searchdata.EndTime='';
      this.searchdata.PageIndex=1;
      this.loading = true;
      this.$refs.element.handleClear();
      this.SparateWaybill(this.searchdata);
    },
  }
};
</script>