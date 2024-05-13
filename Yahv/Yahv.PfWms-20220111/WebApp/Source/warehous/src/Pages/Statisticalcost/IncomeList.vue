<style scoped>
.itemlable {
  display: inline-block;
  width: 23%;
  margin-bottom: 10px;
}
.listbox {
  margin-top: 20px;
}
.pagebox{
  text-align: right;
  padding: 10px;
}
.summary{
  color: red;
  text-align: right;
  padding-right: 20px;
}
</style>
<template>
  <div>
    <div class="top">
      <p class="itemlable">
        <label>客户名称：</label>
        <Select v-model="search.s_client"  filterable style="width: 73%">
          <Option
            v-for="item in clientArr"
            :value="item.value"
            :key="item.value"
            >{{ item.text }}</Option
          >
        </Select>
      </p>
      <p class="itemlable">
        <label>收款公司：</label>
        <Select v-model="search.s_payee" filterable style="width: 73%">
          <Option
            v-for="item in CompanieArr"
            :value="item.value"
            :key="item.value"
            >{{ item.text }}</Option
          >
        </Select>
      </p>
      <p class="itemlable">
        <label>币种：</label>
        <Select v-model="search.s_currency" filterable style="width: 73%">
          <Option
            v-for="item in CurrencieArr"
            :value="item.value"
            :key="item.value"
            >{{ item.text }}</Option
          >
        </Select>
      </p>
      <p class="itemlable">
        <label>入仓号：</label>
        <Input v-model="search.s_code"  placeholder="请输入入仓号" style="width: 73%" />
      </p>
      <p class="itemlable">
          <label>日期选择：</label>
           <DatePicker type="daterange" :clearable='false' :value="value2" ref="element" show-week-numbers @on-change='changeDatePicker' style="width: 73%"></DatePicker>
      </p>
      <Button type="primary" icon="ios-search" @click="search_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="clear_btns">清空</Button>
    </div>
    <p class="summary" v-if="datalist.length>0">{{datalist[0].summary}}</p>
    <div class="listbox">
      <Table :columns="titleName" :data="datalist" :loading="loading"  ref="table" :max-height="tableHeight">
        <template slot-scope="{ row, index }" slot="action">
          <Button
            type="primary"
            size="small"
            style="margin-right: 5px"
            @click="show(row)"
            >查看</Button
          >
        </template>
      </Table>
       <div class="pagebox">
        <Page
          show-total
          show-elevator 
          show-sizer
          :total="total"
          :page-size="search.s_pagesize"
          :current="search.s_pageindex"
          @on-change="changepage"
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize" 
        />
      </div>
    </div>
    <Drawer title="收款详情" :width="80"  v-model="showDetail" @on-visible-change="changevisible">
      <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import moment from "moment";
import { getclients,GetCompanies,GetCurrencies,showIncomeList} from "../../api/CgApi";
export default {
  name: "IncomeList",
  data() {
    return {
      value2:[],
       loading: true,
      total:100,//总条数
      showDetail:false,//显示详情弹出框
      search:{
        s_begin:null, //搜索起始时间
        s_end:null,   //搜索结束时间
        s_client:null,  //客户ID
        s_code:null, //为EnterCode 入仓号
        s_currency:null, //币种
        s_payee:null, //收款公司ID
        s_pageindex:1,   //当前页码
        s_pagesize:20    //页面大小
        },
        clientArr:[],//客户公司
        CurrencieArr:[],//币种
        CompanieArr:[],//内部公司
        titleName: [
          {
            type: "index",
            width: 60,
            align: "center",
          },
          {
            title: "客户名称",
            key: "ClientName",
            align: "center",
          },
          {
            title: "收款公司",
            key: "PayeeName",
            align: "center",
          },
          {
            title: "入仓号",
            key: "EnterCode",
            align: "center",
          },
          {
            title: "币种",
            key: "CurrencyName",
            align: "center",
          },
          {
            title: "记账",
            key: "LeftPrice",
            align: "center",
          },
          {
            title: "现金",
            key: "RightPrice",
            align: "center",
          },
          {
            title: "条数",
            key: "Count",
            align: "center",
          },
          {
            title: "操作",
            slot: "action",
            align: "center",
          },
        ],
        datalist:[],//数据列表
        tableHeight:500,

    };
  },
  created(){
      this.getCurDay()
      this.getinfo()
      this.search_btn()
     
  },
  computed: {
      showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  mounted() {
    this.setnva();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  methods: {
    showDatefun(val) {
    //时间格式转换
    // console.log(val)
    if (val != "") {
      if (val || "") {
        var result =moment().format("YYYY-MM-DD")
        return result;
      }
    }
  },
  getCurDay(){
  this.value2=[];
  let nowdays = new Date();
  let year = nowdays.getFullYear();
  let month = nowdays.getMonth() + 1;
  month = month > 9 ? month : "0" + month;

  let firstDayOfCurMonth = `${year}-${month}-01`;
  let lastDay = new Date(year, month, 0);
  let lastDayOfCurMonth = `${year}-${month}-${lastDay.getDate()}`;

  this.search.s_begin=firstDayOfCurMonth;
  this.search.s_end=lastDayOfCurMonth;
  this.value2.push(this.search.s_begin)
  this.value2.push(this.search.s_end)
},
    setnva() {
      var cc = [
        {
          title: "收入统计",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    //查看详情
    show(row) {
      console.log(row)
      this.showDetail=true
      this.$router.push({
        path: "/IncomeList/IncomeListDetail",
        query: {
          s_begin:row.Begin, //搜索的起始时间,就是点击详情页时页面中的搜索条件，如果没有就null
          s_end:row.End, //搜索的结束时间,没有就null
          clientId:row.ClientID, //客户ID,就用点击记录中的对应clientID
          payee:row.Payee, //收款公司ID, 点击记录中的PayeeID
          currency:row.Currency, //币种
          ClientName:row.ClientName,
          PayeeName:row.PayeeName,
          CurrencyName:row.CurrencyName 
        }
      });
    },
    //查询
    search_btn() {
      this.search.s_pageindex=1
      this.searchdata()
    },
    searchdata(){
       this.loading=true
       showIncomeList(this.search).then(res=>{
        this.loading=false
        this.datalist=res.Data;
        this.total=res.Total
      })
    },
    //清空
    clear_btns() {
      this.$refs.element.handleClear();
      this.search.s_begin=null
      this.search.s_end=null
      this.search.s_client=null
      this.search.s_code=null
      this.search.s_currency=null
      this.search.s_payee=null
      this.search.s_pageindex=1
      // this.search.s_pagesize=20
      this.getCurDay()
      this.searchdata()
    },
    //抽屉显示状态
    changevisible(value){
        if(value==false){
            this.$router.go(-1);
        }
    },

     //改变页码的时候调用
    changepage(value) { 
      this.search.s_pageindex = value;
      console.log(this.search)
     this.searchdata(this.search);
    },
    //改变条数
    changepagesize(value){
     console.log(value)
      this.search.s_pagesize = value;
      this.searchdata(this.search);
    },
    //选择日期
     changeDatePicker(value){ //改变时间
        if(value[0]==''){
          this.search.s_begin=null
        }else{
          this.search.s_begin=value[0]
        }
        if(value[1]==''){
         this.search.s_end=null
        }else{
         this.search.s_end=value[1]
        }
        
        
    },
    // 获取基础信息
    getinfo(){
        // getclients,GetCompanies,GetCurrencies
        getclients().then(res => {
            this.clientArr=res
        });
        GetCompanies().then(res => {
            this.CompanieArr=res
        });
        GetCurrencies().then(res => {
            this.CurrencieArr=res
        });
    }

  },
};
</script>