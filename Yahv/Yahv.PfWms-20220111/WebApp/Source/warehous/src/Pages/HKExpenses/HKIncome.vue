<style scoped>
  .cashbox >>> .ivu-tabs-content {
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
                       v-if='EnterType == "out"'
                       v-show="spinShow==false"
                       v-bind:lableName='lableName'
                       v-bind:Currencyarr="Currencyarr"
                       v-bind:Subjectarr="Subjectarr"
                       v-bind:Payeearr="Bookkeeping.Payeearr"
                       v-bind:Payerarr="Bookkeeping.Payerarr"
                       v-bind:OrderID='OrderID'
                       v-bind:TinyOrderID='TinyOrderID'
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
                               v-bind:TinyOrderID='TinyOrderID'
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
          <Cash v-if='EnterType == "out"'
                ref="Cash"
                v-bind:Currencyarr="Currencyarr"
                v-bind:Subjectarr="Subjectarr"
                v-bind:Payeearr="Cash.Payeearr"
                v-bind:Payerarr="Cash.Payerarr"
                v-bind:OrderID='OrderID'
                v-bind:TinyOrderID='TinyOrderID'
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
                        v-bind:TinyOrderID='TinyOrderID'
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
  import Cash from "./HKCash";
  import Bookkeeping from "./HKBookkeeping";
  import IncomeCash from "./HKIncomeCash";
  import IncomeBookkeeping from "./HKIncomeBookkeeping";
  import { currency, subject, IncomeRecords, OutcomeRecords, IncomeParters, OutcomeParters } from "../../api/CgApi";
  import { Paymentslist } from "../../api/index";
  export default {
    name : "HKIncome",
    props: ["EnterType","OrderID","WaybillID","TinyOrderID"],
    components: {
      Cash,
      Bookkeeping,
      IncomeCash,
      IncomeBookkeeping
    },
    data() {
      return {     
        conduct: null,       
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
        Records: [],//记账
        Currents: [],//现金       
        lableName: "",//
        CashFile: [],//现金文件
        BookeepFile: [],//记账文件
      };
    },
    created() {    
      this.conduct = "代报关"
      this.changtabledata()
      window["PhotoUploaded"] = this.changedimg;
    },
    mounted() {
      if (this.EnterType == "in") {
        this.Bookkeepinglabel = "记账";
        this.Cashlabel = "现金";
        this.lableName = '应收'
        this.getlistdata(this.EnterType)
        this.IncomeParters(this.OrderID)  
      } else {
        this.Bookkeepinglabel = "记账";
        this.Cashlabel = "现金";
        this.lableName = '应付'
        this.getlistdata(this.EnterType)
        this.OutcomeParters(this.OrderID)      
      }
      this.currency_subject();    
      this.getsubject(this.EnterType, this.conduct);
    },
    methods: {
      changetab(name) {
        this.changtabledata()
      },
      changtabledata() {
        if (this.EnterType == "in") {
          this.IncomeRecords(this.OrderID)
        } else {
          this.OutcomeRecords(this.OrderID)
        }
      },
      currency_subject() {
        //币种
        currency().then(res => {
          this.Currencyarr = res.obj;          
        });
      },
      getsubject(val, type) {
        //console.log(type)
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
        //console.log(data);
        subject(data).then(res => {
          this.Subjectarr = res;         
        });
      },
      getlistdata(type) {       
        if (type == 'in') {
          this.IncomeRecords(this.OrderID)
        } else {
          this.OutcomeRecords(this.OrderID)
        }
      },
      IncomeRecords(id) { //收入记录
        IncomeRecords(id).then(res => {
          //console.log(res)
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
          this.$forceUpdate();
        } else {
          this.BookeepFile.push(newfile)
          this.$forceUpdate();
        }       
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
