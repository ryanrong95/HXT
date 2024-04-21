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
      <Input v-model.trim="search_data.lotnumber" placeholder="运输批次号" class="inputwidth" />
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
      <Select v-model="search_data.carrierID" class="inputwidth" placeholder="选择承运商" ref="resetSelect">
        <Option v-for="(item,index) in Carrierarr" :value="item.ID" :key="index">{{ item.Name }}</Option>
      </Select>
      <Input v-model.trim="search_data.carNumber" placeholder="车牌号" class="inputwidth" />
      <Select v-model="search_data.status" class="inputwidth" placeholder="截单状态" ref="resetSelect">
        <Option v-for="(item,index) in OrderStatusarr" :value="item.ID" :key="index">{{ item.Status }}</Option>
      </Select>
      <Button type="primary" @click="search_btn">查询</Button>
      <Button type="error" @click="cancel_btn">清空</Button>
    </div>
    <div style="margin:15px 0;height:35px">
      <!-- 根据最新需求，不做打印 -->
      <!-- <Button type="primary">清单打印</Button> -->
      <div style="width:100px;float:right">
        <Badge :count="5" type="error">
                <Button type="primary" icon="md-checkbox-outline" @click="dyncMount">申报窗口</Button>
        </Badge>
    </div>
    </div>
    <!-- 运输列表 -->
    <div>
      <Table  ref="selection" :columns="list_title" :data="list_data" :loading="loading">
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
             <p>{{row.WaybillTypeDes}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="CuttingOrderStatus">
             <p>{{row.CuttingOrderStatusDes}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="action">
              <Button type="primary" icon="md-create" size="small" :disabled="row.CuttingOrderStatus==200?true:false" @click="action_btn(row.LotNumber)">处理</Button>
          </template>
      </Table>
      <div class="pagebox">
         <Page :total="total" show-elevator :current="search_data.pageindex"  :page-size="search_data.pagesize" @on-change="changepage"/>
      </div>
    </div>
    <Drawer :closable="true"  :mask-closable="false"  v-model="showTransportDetail" @on-visible-change="Drawerstate" :width='85'>
       <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import {CustomTransport,Carriers,CuttingOrderStatus} from "../../api"
export default {
  data() {
    return {
      total:0,
      loading:true,
      search_data:{
        type:"315",  //类型
        warehouseID:sessionStorage.getItem("UserWareHouse"),  //库房编号
        lotnumber:"",  //运输批次号
        startdate:"",  //开始时间
        enddate:"",  //结束时间
        carrierID:"",  //承运商编号 
        carNumber:"",  //车牌号
        status:"100",     //接单状态
        pageindex:1,  //当前页码
        pagesize:2,  //每页记录数
      },
      list_title: [
        {
          type: "selection",
          width: 60,
          align: "center"
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
      OrderStatusarr:[]
    };
  },
  computed: {
      showTransportDetail:{
       get: function () {
         return this.$store.state.common.TransportDetail;
       },
        set: function (newValue) {
       }    
      }
  },
  created() {
    this.CuttingOrderStatus()
    this.getCustomTransport()
    this.getCarriers()

  },
  mounted() {
    this.setnva();
  },
  methods: {
    CuttingOrderStatus(){
      CuttingOrderStatus().then(res=>{
        console.log(res)
        this.OrderStatusarr=res.obj;
      })
    },
    getCustomTransport(){
      CustomTransport(this.search_data).then(res=>{
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
        this.$router.push({ path:"/TransportList/TransportDetail/"+ID})
        // this.$router.push({ path:"/Outgoing/OutDeclareDetail/"+id})
    },
    Drawerstate(value){
        if(value==true){
            this.$store.dispatch("setTransportDetail",true);
        }else{
            this.$store.dispatch("setTransportDetail",false);
            this.$router.go(-1);   //控制路由跳回原来页面
        }
    },
    dyncMount(){  //申报窗口打开页面
        this.$store.dispatch("setTransportDetail",true);
        this.$router.push({ path:"/TransportList/Customswindow"})
    },
    changepage(value){
      this.loading=true;
      this.search_data.pageindex=value;
      this.getCustomTransport()
    },
    changedata(value) {//时间格式 获取开始时间与结束时间
      this.search_data.startdate = value[0];
      this.search_data.enddate = value[1];
    },
    getCarriers(){
      Carriers().then(res=>{
        this.Carrierarr=res.obj;
      })
    },
    search_btn(){
        
        if(this.search_data.lotnumber!=""||
          this.search_data.carrierID!=""||
          this.search_data.startdate!=""||
          this.search_data.enddate!="" ||
          this.search_data.carNumber!=""||
          this.search_data.status!=''){
            this.loading=true;
            this.search_data.pageindex=1;
            this.getCustomTransport()
        }else{
          this.$Message.warning('请输入查询条件');
        }
      // console.log(this.search_data)
      // if(this.search_data.lotnumber==""||this.search_data.startdate==""||this.search_data.enddate==""||this.search_data.carrierID==""||this.search_data.carrierID==undefined||this.search_data.carNumber==""){
      //   this.$Message.warning('请输入查询条件');        
      // }else{
      //   this.loading=true;
      //   this.search_data.pageindex=1;
      //   this.getCustomTransport()
      // }
    },
    cancel_btn(){
        if(this.search_data.lotnumber!=""||
          this.search_data.carrierID!=""||
          this.search_data.carrierID!=undefined||
          this.search_data.startdate!=""||
          this.search_data.enddate!="" ||
          this.search_data.carNumber!=""||
          this.search_data.status!=""){
              this.search_data.lotnumber="";
              this.search_data.startdate="";
              this.search_data.enddate="";
              this.search_data.carrierID="";
              this.search_data.carNumber="";
              this.loading=true;
              this.search_data.pageindex=1;
              this.search_data.status='100'
              this.getCustomTransport()
              this.$refs.element.handleClear();
        }else{
          // this.$Message.warning('请输入查询条件');
        }
        this.search_data.carrierID=""
        console.log(this.search_data)
    }
  }
};
</script>
