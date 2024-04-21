<style scpoed>
.pagebox{
  text-align: right;
  padding: 10px 0;
}
</style>
<template>
  <div>
    <div>
      <label for="运单号">运单号：</label>
      <Input v-model.trim="searchdata.waybillCode" placeholder="" style="width:200px;margin-right:20px" />
      <label for="运单号">承运商：</label>
      <Select v-model="searchdata.carrierID" placeholder="" style="width:200px;margin-right:20px">
          <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
      </Select>
      <label for="库位">库位：</label>
      <Input v-model.trim="searchdata.shelveID" placeholder="" style="width: 200px;margin-right:20px"  />
      <label for="暂存录入时间：">暂存录入时间：</label>
      <DatePicker type="date" placeholder="" style="width: 200px;margin-right:20px" @on-change="changedata" ref="element"></DatePicker>
      <Button type="primary" @click="search_btn">查询</Button>
      <Button type="error" @click="clear_btn">清空</Button>
    </div>
    <div style="padding-top:40px;">
      <Table  :columns="titles" :data="listdata" :loading="loading">
        <template slot-scope="{ row,index }" slot="indexs">
          <strong>{{ index+1}}</strong>
        </template>
        <template slot-scope="{ row,index }" slot="CreateDate">
          <span>
              {{row.CreateDate|showDate}}
          </span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <!-- <Button type="error" size="small" @click="remove(index)">删除</Button> -->
          <Button type="primary" size="small" style="margin-right: 5px" @click="show(row.WaybillID)">修改</Button>
        </template>
      </Table>
      <div class="pagebox">
         <Page :total="total" :page-size="10" :current="searchdata.pageindex" @on-change="changepage"/>
      </div>
    </div>
    <Drawer title v-model="Spearatedrawer" width="70">
      <div>
        <Details :key="timer1" ref="routineenter" @uploadlist='uploadlist'></Details>
      </div>
    </Drawer>
  </div>
</template>
<script>
import Details from "./SeparateDetail";
import {wareing,SparateWaybill,Carriers} from "../../api"
import moment from "moment"

export default {
  components: {
    Details: Details
  },
  data() {
    return {
      CarrierList:[],
      loading:true,
      timer1:"",
      warehouseID:sessionStorage.getItem('UserWareHouse'),
      total:0,
      PageIndex:1,
      value:"",
      Spearatedrawer: false,
      titles:[
          {
          title: "#",
          slot: "indexs",
          align: "center",
          width:50,
        },
        {
          title: "运单号",
          key: "Code"
        },
        {
          title: "到货方式",
          key: "WaybillTypeDes"
        },
        {
          title:"承运商",
          key:"CarrierName"
        },
        {
          title: "暂存录入时间",
          slot: "CreateDate",
        },
        {
          title: "入仓号",
          key: "EnterCode"
        },
        {
          title: "描述",
          key: "Summary"
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center"
        }
      ],
      listdata:[
        // {
        //   WaybillID:"111111",
        //   CreateDate:"21212",
        //   WaybillTypeDescription:"送货",
        //   EnterCode:"dasdas",
        //   Code:"12121212"
        // }
      ],
      searchdata:{
         warehouseID:sessionStorage.getItem('UserWareHouse'),//库房ID，必填 
         excuteStatus:300,//执行状态，必填 
         waybillCode:"",//运单号 
         carrierID:"",//承运商编号 
         place:"",//发货地 
         shelveID:"",//库位 
         createDate:"",//创建时间 
         pageindex:1,// 当前页码 
         pagesize:10,//每页显示记录数 
      }
      // listdata:[
      //     {
      //         PartNumber:"AD6525654*89585",
      //         Manufacturer:"MMKSFERJEIRJ",
      //         DateCode:"1OIO",
      //         Quantity:"10000",
      //         Place:"中国"
      //     },
      //     {
      //         PartNumber:"AD6525654*89585",
      //         Manufacturer:"MMKSFERJEIRJ",
      //         DateCode:"1OIO",
      //         Quantity:"10000",
      //         Place:"中国"
      //     },
      //     {
      //         PartNumber:"AD6525654*89585",
      //         Manufacturer:"MMKSFERJEIRJ",
      //         DateCode:"1OIO",
      //         Quantity:"10000",
      //         Place:"中国"
      //     }
      // ]
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
  mounted() {
    this.setnva();
    this.SparateWaybill(this.searchdata)
    this.Carriers()
  },
  methods: {
    uploadlist(){
       this.Spearatedrawer=false;
       this.SparateWaybill(this.searchdata)
    },
    setnva() {
        var cc = [
            {
            title: "在库管理",
            href: "/Stock"
            },
            {
            title: "暂存管理",
            href: "/Stock/Separate"
            }
        ];
        this.$store.dispatch("setnvadata", cc);
        },
    show(index) {
        this.Spearatedrawer = true;
        this.timer1 = new Date().getTime()
          var that=this
          setTimeout(function(){
           that.$refs.routineenter.Initialization(index); //初始化详情数据
          },20)
      },
    remove(index) {

        },
    changepage(value){
      this.loading=true;
      this.searchdata.pageindex=value;
        this.SparateWaybill(this.searchdata)
     },
     SparateWaybill(data){
       SparateWaybill(data).then((res) => {
            console.log(res)
            this.total=res.Total
            if(res.Total<=0){
              this.listdata=[]
            }else{
             this.listdata=res.Data;
            }
             this.loading=false;
           })
     },
     Carriers(){ //获取承运商列表
            Carriers().then(res => {
                this.CarrierList=res.obj;
                });
     },
    search_btn(){  //搜索
       this.loading=true;
       this.SparateWaybill(this.searchdata)
    },
    clear_btn(){  //清空
       this.searchdata={
         warehouseID:sessionStorage.getItem('UserWareHouse'),//库房ID，必填 
         excuteStatus:300,//执行状态，必填 
         waybillCode:"",//运单号 
         carrierID:"",//承运商编号 
         place:"",//发货地 
         shelveID:"",//库位 
         createDate:"",//创建时间 
         pageindex:1,// 当前页码 
         pagesize:10,//每页显示记录数 
      }
      this.loading=true;
      this.$refs.element.handleClear()
      this.SparateWaybill(this.searchdata)
    },
    changedata(value){
        this.searchdata.createDate=value;
    }

 },
    
};
</script>