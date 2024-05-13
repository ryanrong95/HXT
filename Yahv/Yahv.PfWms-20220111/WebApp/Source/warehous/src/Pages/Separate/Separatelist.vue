<style scpoed>
.pagebox {
  text-align: right;
  padding: 10px 0;
}
.setinputs{
  width:12%;
  margin-right:20px;
  margin-bottom: 10px;
}
</style>
<template>
  <div>
    <div>
      <label for="运单号">运单号：</label>
      <Input
        v-model.trim="searchdata.Code"
        placeholder="运单号"
       class="setinputs"
      />
      <label for="运单号">承运商：</label>
      <Select v-model="searchdata.CarrierID" placeholder class="setinputs">
        <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
      </Select>
      <label for="库位">库位：</label>
      <Input v-model.trim="searchdata.ShelveID" placeholder='库位'   class="setinputs"/>
      <label for="库位">型号：</label>
      <Input v-model.trim="searchdata.PartNumber" placeholder='型号'   class="setinputs"/>
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
          <p>{{row.Waybill.TypeDes}}</p>
        </template>
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
            type="primary"
            size="small"
            style="margin-right: 5px"
            @click="show(row.Waybill.ID)"
          >修改</Button>
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
import { cgtempstockshow,SparateWaybill } from "../../api/CgApi";
import moment from "moment";

export default {
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
          title: "运单号",
          slot: "Code"
        },
        {
          title: "到货方式",
          slot: "OrderID"
        },
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
        PartNumber:"",
        PageIndex: 1,
        PageSize: 20,
      },
      tableHeight:500,
    };
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
    this.setnva();
    this.Carriers();
    this.SparateWaybill(this.searchdata);
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "已有暂存",
          href: "/Stock/Separate"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
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
      console.log(value)
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
        path:'/Separate/list/revise',
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
      this.searchdata.PartNumber='';
      this.searchdata.PageIndex=1;
      this.loading = true;
      this.$refs.element.handleClear();
      this.SparateWaybill(this.searchdata);
    },
  }
};
</script>