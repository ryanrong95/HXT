<style scoped>
.lableinfo {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.infobox {
  width: 100%;
  min-height: 100px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  padding: 10px 10px;
}
.col {
  width: 30%;
  display: inline-block;
  line-height: 40px;
  font-size: 14px;
}
.tabledetail {
  margin-top: 20px;
  padding-bottom: 100px;
}
</style>
<template>
  <div>
    <p class="lableinfo">基础信息</p>
    <div class="infobox">
      <div class="">
        <p class="col">客户公司：{{ClientName}}</p>
        <p class="col">付款公司：{{PayerName}}</p>
        <p class="col">币种：{{CurrencyName}}</p>
      </div>
      <div class="">
        <p class="col">开始日期：{{s_begin}}</p>
        <p class="col">结束日期：{{s_end}}</p>
      </div>
    </div>
    <p class="lableinfo">付款信息</p>
    <div class="tabledetail" >
      <Table :columns="columnstitle" :data="dataDetail" :loading="loading"></Table>
    </div>
  </div>
</template>
<script>

import { showExpendDetail} from "../../api/CgApi";
export default {
  data() {
    return {
      loading: true,
      columnstitle: [
        {
          type: "index",
          width: 60,
          align: "center",
        },
        {
          title: "订单号",
          key: "OrderID",
          align: "center",
        },
        {
          title: "收款公司",
          key: "PayeeName",
          align: "center",
        },
        {
          title: "分类",
          key: "Catalog",
          align: "center",
        },
        {
          title: "科目",
          key: "Subject",
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
          title: "创建时间",
          key: "CreateDate",
          align: "center",
        },
      ],
      dataDetail: [  ],

          s_begin:null, //搜索的起始时间,就是点击详情页时页面中的搜索条件，如果没有就null
          s_end:null, //搜索的结束时间,没有就null
          clientId:null, //客户ID,就用点击记录中的对应clientID
          payee:null, //收款公司ID, 点击记录中的PayeeID
          currency:null, //币种
          ClientName:null,
          PayeeName:null,
          CurrencyName:null,
    };
  },
  created(){
           this.s_begin=this.$route.query.s_begin, //搜索的起始时间,就是点击详情页时页面中的搜索条件，如果没有就null
          this.s_end=this.$route.query.s_end, //搜索的结束时间,没有就null
          this.clientId=this.$route.query.clientId, //客户ID,就用点击记录中的对应clientID
          this.payer=this.$route.query.payer, //收款公司ID, 点击记录中的PayeeID
          this.currency=this.$route.query.currency, //币种
          this.ClientName=this.$route.query.ClientName,
          this.PayerName=this.$route.query.PayerName,
          this.CurrencyName=this.$route.query.CurrencyName
          
  },
  mounted(){
         this.getdata()
  },
  methods:{
    getdata(){
      var data={
          s_begin:this.s_begin, //搜索的起始时间,就是点击详情页时页面中的搜索条件，如果没有就null
          s_end:this.s_end, //搜索的结束时间,没有就null
          clientid:this.clientId, //客户ID,就用点击记录中的对应clientID
          payer:this.payer, //收款公司ID, 点击记录中的PayeeID
          currency:this.currency, //币种
      }
      showExpendDetail(data).then(res=>{
        this.loading=false
        this.dataDetail=res
      })
    }
  }
};
</script>