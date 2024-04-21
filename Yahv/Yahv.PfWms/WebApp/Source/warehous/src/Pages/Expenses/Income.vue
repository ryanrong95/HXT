<style scoped>
  .cashbox > > > .ivu-tabs-content {
    min-height: 433px;
  }
</style>
<template>
  <div>
    <div class="cashbox">
      <Tabs value="Bookkeeping" @on-click="changetab" :animated="false">
        <TabPane :label="Bookkeepinglabel" name="Bookkeeping">
          <Spin size="large" fix v-show="spinShow==true"></Spin>
          <Bookkeeping ref="Bookkeeping"
                       v-if='this.$route.query.type == "out"'
                       v-show="spinShow==false"
                       v-bind:lableName='lableName'
                       v-bind:Currencyarr="Currencyarr"
                       v-bind:Subjectarr="Subjectarr"
                       v-bind:Payeearr="Bookkeeping.Payeearr"
                       v-bind:Payerarr="Bookkeeping.Payerarr"
                       v-bind:OrderID='OrderID'
                       v-bind:WaybillID='WaybillID'
                       v-bind:Records.sync='Records'
                       v-bind:conduct='conduct'
                       v-bind:EnterType='EnterType'
                       @fatherMethod="changtabledata"
                       v-bind:Files='BookeepFile'
                       @delbookeep='delbookeep'></Bookkeeping>
          <div v-else>
            <IncomeBookkeeping ref="IncomeBookkeeping"
                               v-show="spinShow==false"
                               v-bind:lableName='lableName'
                               v-bind:Currencyarr="Currencyarr"
                               v-bind:Subjectarr="Subjectarr"
                               v-bind:Payeearr="Bookkeeping.Payeearr"
                               v-bind:Payerarr="Bookkeeping.Payerarr"
                               v-bind:OrderID='OrderID'
                               v-bind:WaybillID='WaybillID'
                               v-bind:Records.sync='Records'
                               v-bind:conduct='conduct'
                               v-bind:EnterType='EnterType'
                               @fatherMethod="changtabledata"
                               v-bind:Files='BookeepFile'
                               @delbookeep='delbookeep'></IncomeBookkeeping>
          </div>
        </TabPane>
        <TabPane :label="Cashlabel" name="Cash">
          <Cash v-if='this.$route.query.type == "out"'
                ref="Cash"
                v-bind:Currencyarr="Currencyarr"
                v-bind:Subjectarr="Subjectarr"
                v-bind:Payeearr="Cash.Payeearr"
                v-bind:Payerarr="Cash.Payerarr"
                v-bind:OrderID='OrderID'
                v-bind:WaybillID='WaybillID'
                v-bind:Currents.sync='Currents'
                v-bind:conduct='conduct'
                v-bind:EnterType='EnterType'
                @fatherMethod="changtabledata"
                v-bind:Files='CashFile'
                @delCash='delCash'></Cash>
          <div v-else>
            <IncomeCash ref="IncomeCash"
                        v-bind:Currencyarr="Currencyarr"
                        v-bind:Subjectarr="Subjectarr"
                        v-bind:Payeearr="Cash.Payeearr"
                        v-bind:Payerarr="Cash.Payerarr"
                        v-bind:OrderID='OrderID'
                        v-bind:WaybillID='WaybillID'
                        v-bind:Currents.sync='Currents'
                        v-bind:conduct='conduct'
                        v-bind:EnterType='EnterType'
                        @fatherMethod="changtabledata"
                        v-bind:Files='CashFile'
                        @delCash='delCash'></IncomeCash>
          </div>
        </TabPane>
      </Tabs>
    </div>
  </div>
</template>
<script>
  import Cash from "./Cash";
  import Bookkeeping from "./Bookkeeping";
  import IncomeCash from "./IncomeCash";
  import IncomeBookkeeping from "./IncomeBookkeeping";
  import { currency, subject, IncomeRecords, OutcomeRecords, IncomeParters, OutcomeParters } from "../../api/CgApi";
  import { Paymentslist } from "../../api/index";
  export default {
    components: {
      Cash,
      Bookkeeping,
      IncomeCash,
      IncomeBookkeeping
    },
    data() {
      return {
        EnterType: null,//录入费用的类型 收入/支出
        conduct: null,
        otype: "",
        spinShow: false,
        Bookkeepinglabel: "记账（应收）",
        Cashlabel: "现金（实收）",
        getformdata: "",
        Currencyarr: [], //币种
        Subjectarr: [],//科目
        Bookkeeping: {
          Payee: "",//	收款人
          Payer: "",//付款人
          Payeearr: [],//收款人
          Payerarr: [],//付款人
        },
        Cash: {
          Payee: "",//	收款人
          Payer: "",//付款人
          Payeearr: [],//收款人
          Payerarr: [],//付款人
        },
        Payee: "",//	收款人
        Payer: "",//付款人
        OrderID: '',//订单号
        WaybillID: "",//运单ID
        Records: [],//记账
        Currents: [],//现金
        Source: "",
        lableName: "",//
        CashFile: [],//现金文件
        BookeepFile: [],//记账文件
      };
    },
    created() {
      this.EnterType = this.$route.query.type
      this.otype = this.$route.query.otype
      this.OrderID = this.$route.query.OrderID
      this.WaybillID = this.$route.query.webillID
      this.Source = this.$route.query.Source
      this.conduct = this.$route.query.conduct
      this.changtabledata()
      console.log(this.conduct)
      window["PhotoUploaded"] = this.changedimg;
      //  window["delimg"]=this.delimg
    },
    mounted() {

      if (this.$route.query.type == "in") {
        this.Bookkeepinglabel = "记账";
        this.Cashlabel = "现金";
        this.lableName = '应收'
        this.getlistdata(this.$route.query.type)
        this.IncomeParters(this.OrderID)
        // this.IncomeRecords(this.WaybillID)
        // this.Payee=this.Bookkeeping.Payeearr[0].ID

      } else {
        this.Bookkeepinglabel = "记账";
        this.Cashlabel = "现金";
        this.lableName = '应付'
        this.getlistdata(this.$route.query.type)
        this.OutcomeParters(this.OrderID)
        // this.OutcomeParters(this.WaybillID)
      }
      this.currency_subject();
      // this.getsubject(this.$route.query.type);
      this.getsubject(this.$route.query.type, this.conduct);
    },
    methods: {
      changetab(name) {
        this.changtabledata()
      },
      changtabledata() {
        if (this.$route.query.type == "in") {
          this.IncomeRecords(this.OrderID)
        } else {
          this.OutcomeRecords(this.OrderID)
        }
      },
      currency_subject() {
        //币种
        currency().then(res => {
          this.Currencyarr = res.obj;
          // this.$refs.Bookkeeping.Currencyarr = this.Currencyarr;
          // this.$refs.Cash.Currencyarr = this.Currencyarr;
        });
      },
      getsubject(val, type) {
        console.log(type)
        var conductdes = ''
        var isCustomsdata = ''
        if (type == 30 || type == 35 || type == 60) {
          conductdes = '代报关'
          isCustomsdata = true
        } else {
          conductdes = '代仓储'
          isCustomsdata = false
        }
        var data = {
          type: val,
          conduct: conductdes,
          isCustoms: isCustomsdata
        };
        console.log(data);
        subject(data).then(res => {
          this.Subjectarr = res;
          // this.$refs.Bookkeeping.Subjectarr = this.Subjectarr;
          // this.$refs.Cash.Subjectarr = this.Subjectarr;
          // console.log(this.subject)
        });
      },
      getlistdata(type) {
        // console.log(this.$route.query.webillID)
        if (type == 'in') {
          this.IncomeRecords(this.$route.query.OrderID)
        } else {
          this.OutcomeRecords(this.$route.query.OrderID)
        }
      },
      IncomeRecords(id) { //收入记录
        IncomeRecords(id).then(res => {
          console.log(res)
          if (res.success == true) {
            this.Records = res.data.Records;//记账
            this.Currents = res.data.Currents;//现金
          }
        })
      },
      OutcomeRecords(id) { //支出记录
        OutcomeRecords(id).then(res => {
          if (res.success == true) {
            this.Records = res.data.Records;//记账
            this.Currents = res.data.Currents;//现金
          }
        })
      },

      IncomeParters(OrderID) { //收入----收款人 付款人
        IncomeParters(OrderID).then(res => {
          this.Bookkeeping.Payeearr = res.data.Record.Payees;
          this.Bookkeeping.Payerarr = res.data.Record.Payers;
          this.Cash.Payeearr = res.data.Current.Payees;
          this.Cash.Payerarr = res.data.Current.Payers
          this.$refs.IncomeCash.PayeeID = res.data.Current.Payees[0].ID
          this.$refs.IncomeCash.PayerID = res.data.Current.Payers[0].ID
          this.$refs.IncomeBookkeeping.PayeeID = res.data.Record.Payees[0].ID
          this.$refs.IncomeBookkeeping.PayerID = res.data.Record.Payers[0].ID
        })
      },
      OutcomeParters(OrderID) { //支出---收款人 付款人
        OutcomeParters(OrderID).then(res => {
          this.Bookkeeping.Payeearr = res.data.Record.Payees;
          this.Bookkeeping.Payerarr = res.data.Record.Payers;
          this.Cash.Payeearr = res.data.Current.Payees;
          this.Cash.Payerarr = res.data.Current.Payers
          this.$refs.Cash.PayeeID = res.data.Current.Payees[0].ID
          this.$refs.Cash.PayerID = res.data.Current.Payers[0].ID
          this.$refs.Bookkeeping.PayeeID = res.data.Record.Payees[0].ID
          this.$refs.Bookkeeping.PayerID = res.data.Record.Payers[0].ID
        })
      },
      changedimg(message) {
        var imgdata = message[0];
        var newfile = {
          CustomName: imgdata.FileName,
          ID: imgdata.FileID,
          Url: imgdata.Url,
        };
        if (imgdata.SessionID == 'Cash') {
          this.CashFile.push(newfile)
        } else {
          this.BookeepFile.push(newfile)
        }
        // this.Files.push(newfile)
        // alert(JSON.stringify(this.Files))
      },
      delbookeep() {
        this.BookeepFile = []
      },
      delCash() {
        this.CashFile = []
      }
    }
  };
</script>
