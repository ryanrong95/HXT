<style scoped>
.inputwidth {
  width: 200px;
  margin-right: 15px;
}
.pagebox{
  text-align: right;
  margin-top: 30px;
}
</style>
<template>
  <div>
    <!-- 搜索条件 -->
    <div>
      <Input v-model.trim="search_data.LotNumber" placeholder="运输批次号" class="inputwidth" />
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
      <Select v-model="search_data.Carrier" class="inputwidth" placeholder="选择承运商" ref="resetSelect">
        <Option v-for="(item,index) in Carrierarr" :value="item.ID" :key="index">{{ item.Name }}</Option>
      </Select>
      <!-- <Input v-model.trim="search_data.carNumber" placeholder="车牌号" class="inputwidth" /> -->
      <Select v-model="search_data.Status" class="inputwidth" placeholder="截单状态" ref="resetSelect">
        <Option v-for="(item,index) in OrderStatusarr" :value="item.Value" :key="index">{{ item.Description }}</Option>
      </Select>
      <Button type="primary" icon="ios-search" @click="search_btn">查询</Button>
      <Button type="error" icon="ios-trash"  @click="cancel_btn">清空</Button>
    </div>
    <div style="margin:15px 0;height:35px">
      <!-- 根据最新需求，不做打印 -->
      <!-- <Button type="primary">清单打印</Button> -->
      <!-- <div style="width:100px;float:right">
        <Badge :count="5" type="error">
                <Button type="primary" icon="md-checkbox-outline" @click="dyncMount">申报窗口</Button>
        </Badge>
    </div> -->
    </div>
    <!-- 运输列表 -->
    <div>
      <Table  ref="selection" :max-height="tableHeight" :columns="list_title" :data="list_data" :loading="loading">
          <template slot-scope="{ row, index }" slot="LotNumber">
             <p>{{row.LotNumber}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="DepartDate">
             <p>{{row.DepartDate|showDate}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="CarNumber">
              <span>{{row.CarNumber1}}</span> &nbsp;&nbsp;&nbsp;<span>{{row.CarNumber2}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="Driver">
             <p>{{row.Driver}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="WaybillType">
             <p v-if="row.IsOnevehicle==true">包车</p>
             <p v-if="row.IsOnevehicle==false">普通</p>
          </template>
          <template slot-scope="{ row, index }" slot="CuttingOrderStatus">
              <Tag color="orange" v-if="row.CuttingOrderStatus==0">等待</Tag>
               <Tag color="green" v-if="row.CuttingOrderStatus==1">已截单</Tag>
               <Tag color="default" v-if="row.CuttingOrderStatus==2">已完成</Tag>
          </template>
          <template slot-scope="{ row, index }" slot="action">
              <Button type="primary" icon="md-create" size="small"  @click="action_btn(row.ID)">处理</Button>
          </template> 
      </Table>
      <div class="pagebox">
         <Page 
          show-total
          :total="total" 
          show-elevator 
          show-sizer
          :current="search_data.PageIndex"  
          :page-size="search_data.PageSize" 
          @on-change="changepage"
          :page-size-opts="showPageArr"
         @on-page-size-change="changepagesize"/>
      </div>
    </div>
    <Drawer :closable="true"  :mask-closable="false"  v-model="showTransportDetail" @on-visible-change="Drawerstate" :width='85'>
        <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import {Carriers} from "../../api"
import {CustomTransport,EnumCgCuttingOrderStatu} from "../../api/CgApi"
export default {
  data() {
    return {
      total:0,
      loading:true,
      search_data:{
        PageIndex:1,//页码
        PageSize:20,//单页条数
        LotNumber:null,//运输批次
        Carrier:null,//承运商,如果列表开发这麻烦可以用input[type=text]代替
        StartDate:null,//开始时间，与荣检约定用：DepartDate
        EndDate:null,//借宿时间，与荣检约定用：DepartDate
        Status:1,
      },
      list_title: [
        {
            type: 'index',
            width: 60,
            align: 'center'
        },
        {
          title: "运输批次号",
          slot: "LotNumber"
        },
        {
          title: "运输时间",
          slot: "DepartDate"
        },
        {
          title: "承运商",
          key: "CarrierName"
        },
        {
          title: "车牌号",
          slot: "CarNumber"
        },
        {
          title: "司机姓名",
          slot: "Driver"
        },
        {
          title: "运输类型",
          slot: "WaybillType"
        },
        {
          title: "截单状态",
          slot: "CuttingOrderStatus"
        },
        {
          title: '操作',
          slot: 'action',
          width: 100,
          align: 'center'
        }
      ],
      list_data: [],
      Carrierarr:[],
      OrderStatusarr:[],
      tableHeight:500
    };
  },
  computed: {
      showTransportDetail:{
       get: function () {
         return this.$store.state.common.TransportDetail;
       },
        set: function (newValue) {
       }    
      },
       showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  created() {
    this.CuttingOrderStatus()
    this.getCustomTransport(this.search_data)
    this.getCarriers()

  },
  mounted() {
    this.setnva();
    this.tableHeight = window.innerHeight - this.$refs.selection.$el.offsetTop - 100
  },
  methods: {
    CuttingOrderStatus(){
      EnumCgCuttingOrderStatu().then(res=>{
        console.log(res)
        this.OrderStatusarr=res;
      })
    },
    getCustomTransport(data){
      CustomTransport(data).then(res=>{
        console.log(res)
        this.list_data=res.obj.Data;
        this.total=res.obj.Total;
        this.loading=false;
      })
    },
    setnva() {
      var cc = [
        {
          title: "报关运输",
          href: "/TransportList"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    action_btn(ID){ //点击进入处理页面
        this.$store.dispatch("setTransportDetail",true);
        this.$router.push({path:"/Transport/TransportList/detail/"+ID})
        // this.$router.push({ path: "/Outgoing/OutDeclareDetail/" + id });
    },
    Drawerstate(value){
        if(value==true){
            this.$store.dispatch("setTransportDetail",true);
        }else{
            this.$store.dispatch("setTransportDetail",false);
            this.$router.go(-1);   //控制路由跳回原来页面
            this.getCustomTransport(this.search_data)
        }
    },
    dyncMount(){  //申报窗口打开页面
        this.$store.dispatch("setTransportDetail",true);
        this.$router.push({ path:"/TransportList/Customswindow"})
    },
    changepage(value){
      this.loading=true;
      this.search_data.PageIndex=value;
      this.getCustomTransport(this.search_data)
    },
    changepagesize(value){
      this.loading=true;
      this.search_data.PageSize=value;
      this.getCustomTransport(this.search_data)
    },
    changedata(value) {//时间格式 获取开始时间与结束时间
      console.log(value)
      if(value[0]==''||value[1]==''){
          this.search_data.StartDate=null;
          this.search_data.EndDate=null;
      }else{
        this.search_data.StartDate = value[0];
        this.search_data.EndDate = value[1];
      }
      
    },
    getCarriers(){
      Carriers().then(res=>{
        this.Carrierarr=res.obj;
      })
    },
    search_btn(){  
         this.loading=true;
         this.search_data.PageIndex=1;
         this.getCustomTransport(this.search_data)
    },
    cancel_btn(){
      this.loading=true;
      this.search_data.LotNumber=null;
      this.search_data.Carrier=null;
      this.search_data.StartDate=null;
      this.search_data.EndDate=null;
      this.search_data.Status=1;
      this.search_data.PageIndex=1;
      this.getCustomTransport(this.search_data)
      this.$refs.element.handleClear();

    }
  }
};
</script>
