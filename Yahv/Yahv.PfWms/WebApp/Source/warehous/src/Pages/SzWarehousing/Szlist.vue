<style scoped>
.input_width {
  width: 15%;
  margin-right: 15px;
}
.pages {
  float: right;
  padding-top: 20px;
}
.mb20 {
  margin-bottom: 20px;
}
</style>
<template>
  <div>
    <!-- 搜索条件 -->
    <div class="mb20">
      <label>运输批次号：</label><Input v-model.trim="searchData.LotNumber" placeholder="运输批次号" class="input_width" />
      <label>车牌号：</label><Input v-model.trim="searchData.CarNumber" placeholder="车牌号" class="input_width" />
      <label>运输时间：</label>
       <DatePicker  :clearable='false' ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder="运输时间"
          class="input_width" 
          @on-change='changeData'></DatePicker>
      <!-- <label>状态：</label> -->
      <!-- <Select v-model.trim="searchData.status" class="input_width" placeholder="状态">
        <Option value="false">待处理</Option>
        <Option value="true">已处理</Option>
      </Select> -->
      <Button type="primary" @click="search" icon="ios-search">查询</Button>
      <Button type="error"  @click="reset" icon="ios-trash">清空</Button>
    </div>
    <!-- 运输列表 -->
    <div>
      <Table :columns="listTitle" :data="listData" :loading="loading" ref="table" :max-height="tableHeight">
        <template slot-scope="{ row, index }" slot="LotNumber">
          <p>{{row.Waybill.LotNumber}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="CarrierName">
          <p>{{row.Waybill.CarrierName}}</p>
        </template>
        <!-- <template slot-scope="{ row, index }" slot="Code">
          <p>{{row.Waybill.Code}}</p>
        </template> -->
        <template slot-scope="{ row, index }" slot="DepartDate">
          <p>{{row.Waybill.DepartDate|showDate}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="Driver">
          <p>{{row.Waybill.Driver}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="WaybillTypeDes">
          <p>{{row.Waybill.Type}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="BoxNumber">
          <p>{{row.Waybill.TotalParts}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="UpperNumber">
          <p>{{row.Waybill.ShelveQuantity}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="CarNumber">
          <span>{{row.Waybill.CarNumber1}}</span> &nbsp;&nbsp;&nbsp;
          <span>{{row.Waybill.CarNumber2}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" icon="md-create" size="small" @click="actionBtn(row.Waybill.ID)">详情</Button>
          <!-- <Button disabled icon="md-checkmark-circle" size="small">完成</Button> -->
        </template>
      </Table>
    </div>
    <div class="pages">
      <Page
        show-total
        show-elevator 
        show-sizer
        :total="total"
        :page-size="searchData.PageSize"
        :current="searchData.PageIndex"
        :page-size-opts="showPageArr"
        @on-change="changePage"
        @on-page-size-change="changePageSize"
      />
    </div>

    <Drawer
      :closable="true"
      v-model="showTransportDetail"
      @on-visible-change="DrawerState"
      :width="80"
    >
      <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import {cgszsortingslist} from "../../api/CgApi";
export default {
  inject: ["reload"],
  data() {
    return {
      carrierList: [],
      listTitle: [
        {
          type: "index",
          width: 60,
          align: "center"
        },
        {
          title: "运输批次号",
          slot: "LotNumber"
        },
        {
          title: "承运商",
          slot: "CarrierName"
        },
        // {
        //   title:"运单号",
        //   slot:'Code'
        // },
        {
          title: "车牌号",
          slot: "CarNumber"
        },
        {
          title: "运输时间",
          slot: "DepartDate"
        },
        {
          title: "司机姓名",
          slot: "Driver"
        },
        {
          title: "运输类型",
          slot: "WaybillTypeDes"
        },
        {
          title: "总件数",
          slot: "BoxNumber"
        },
        {
          title: "已上架数",
          slot: "UpperNumber"
        },
        {
          title: "操作",
          slot: "action",
          width: 200,
          align: "center"
        }
      ],
      listData: [ ],
      loading: false, //列表是否在加载中
      total: 0, //列表总行数
      carrierName: "", //
      searchData: {
        WhID:sessionStorage.getItem('UserWareHouse'), //库房ID
        LotNumber: "", //运输批次号
        CarNumber: "", //车牌号
        WaybillExcuteStatus: "10", // 运单执行状态
        Code: "", //运单code
        PageSize:20,
        PageIndex: 1,
        StartDate: null,
        EndDate: null,
        Status:false
      },
      getDataInterval: null,
      tableHeight:500
    };
  },
  beforeRouteUpdate(to,from,next){
    if(to.path=="/Szlist/0"&&this.showTransportDetail==false){
      // this.searchData.Status=false
      this.reload(); //刷新当前页面
    }else if(to.path=="/Szlist/1"&&this.showTransportDetail==false){
      //  this.searchData.Status=true
       this.reload(); //刷新当前页面
    }
    next()
  },
  computed: {
    showTransportDetail: {
      get: function() {
        return this.$store.state.common.TransportDetail;
      },
      set: function(newValue) {}
    },
    showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  created() {
    // this.Carriers();
    this.loading = true;
    this.getListData();
    // this.getDataInterval = setInterval(() => {
    //   this.getListData()
    // }, 6000)
  },
  mounted() {
    this.setNav();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  methods: {
    setNav() {
      var cc = [
        {
          title: "待入库",
          href: "/TransportList"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    actionBtn(LotNumber) {
      this.$store.dispatch("setTransportDetail", true);
      var type=null
       if(this.$route.params.Type==0){
         type=false
       }else{
         type=true
      }
      this.$router.push({ path: "/Szlist/SzDetail/" + LotNumber+"?status="+ type});
      // this.$router.push({
      //     name: 'SzDetail',
      //     query: {ID:LotNumber }
      //   });
    },
    DrawerState(value) {
      if (value == true) {
        this.$store.dispatch("setTransportDetail", true);
      } else {
        this.$store.dispatch("setTransportDetail", false);
        this.$router.go(-1); //控制路由跳回原来页面
        // this.getListData()
      }
    },
    Carriers() {
      //承运商列表
      Carriers().then(res => {
        this.carrierList = res.obj;
      });
    },
    getListData() {
      console.log(this.$route.params.Type)
      if(this.$route.params.Type==0){
        this.searchData.Status=false
      }else{
        this.searchData.Status=true
      }
      cgszsortingslist(this.searchData).then(res => {
        console.log(res)
        this.listData = res.obj.Data;
        this.total = res.obj.Total;
        this.loading = false;
      });
    },
    changePage(value) {
      this.searchData.PageIndex = value;
      this.loading = true;
      this.getListData();
    },
    changePageSize(value){
       this.loading = true;
       this.searchData.PageSize = value;
       this.getListData();
    },
    search() {
      //搜索数据请求接口
      console.log(this.searchData)
      this.searchData.PageIndex = 1;
      this.loading = true;
      this.getListData();
    },
    reset() {
      //重置数据
      this.searchData.LotNumber = "";
      this.searchData.CarNumber = "";
      this.searchData.Code = "";
      this.searchData.PageIndex = 1;
      this.searchData.StartDate= null
      this.searchData.EndDate= null
      this.loading = true;
      this.$refs.element.handleClear();
      this.getListData();
    },
    changeData(val){
       this.searchData.StartDate= val[0]
       this.searchData.EndDate= val[1]
    }
  },
  beforeDestroy() {
    clearInterval(this.getDataInterval);
  }
};
</script>
