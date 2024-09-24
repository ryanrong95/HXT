<style scoped>
  .inputwidth {
    width: 260px;
    padding-right: 30px;
  }

  .frombox {
    padding-top: 20px;
  }

  .iteminput {
    display: inline-block;
    padding-bottom: 10px;
  }

  .setpadding_bottom {
    padding-bottom: 15px;
    font-size: 16px;
  }

  .spanMust {
    color: red;
  }

  .tanlebox /deep/ .ivu-table-cell {
    padding-left: 5px;
    padding-right: 5px;
  }

  .tanlebox {
    padding-bottom: 20px;
  }
</style>
<template>
  <div class="cashbox">
    <div>
      <div class="iteminput">
        <label>收款人：</label>
        <Select v-model="PayeeID"
                class="inputwidth"
                :disabled="true">
          <Option v-for="(item,index) in this.Payeearr"
                  :value="item.ID"
                  :key="index">
            {{ item.Name }}
          </Option>
        </Select>
      </div>
      <div class="iteminput">
        <label>付款人：</label>
        <Select v-model="PayerID"
                class="inputwidth"
                :disabled="true">
          <Option v-for="(item,index) in this.Payerarr"
                  :value="item.ID"
                  :key="index">
            {{ item.Name }}
          </Option>
        </Select>
      </div>
      <div class="iteminput">
        <div style="display:flex;flex-direction:row;">
          <label style="line-height: 2;"><span class="spanMust">*</span>科&nbsp;&nbsp;&nbsp;目：</label>
          <Cascader :transfer="true"
                    :data="Subjectarr"
                    v-model="Subject"
                    @on-change="changecascader"
                    class="inputwidth"></Cascader>
        </div>
      </div>
      <p class="iteminput">
        <label><span class="spanMust">*&nbsp;</span>数&nbsp;量：</label>
        <Input v-model="Quantity" placeholder="请输入数量" @on-blur="changenumber(Quantity)" class="inputwidth" />
      </p>
      <p class="iteminput">
        <label><span class="spanMust">*&nbsp;</span>{{lableName}}金&nbsp;额：</label>
        <Input v-model="LeftPrice" placeholder="请输入金额" @on-blur="testprice(LeftPrice)" :disabled="unitprice==null?false:true" class="inputwidth" />
      </p>
      <Button type="primary" size="small" icon='md-checkmark' @click="submit_btn">提交</Button>
    </div>
    <div class="tanlebox">
      <h1 class="setpadding_bottom">流水列表</h1>
      <Table stripe :columns="columns1" :data="Records" max-height="380">
        <template slot-scope="{ row, index }" slot="indexs">
          <span>{{ index + 1 }}</span>
        </template>
        <template slot-scope="{ row }" slot="type">
          <span>{{ row.Conduct }}</span>
        </template>
        <template slot-scope="{ row }" slot="Subject">
          <span>{{row.Descirption}}({{row.Quantity}})</span>
        </template>
        <template slot-scope="{ row }" slot="TargetName">
          <span>{{ row.TargetName }}</span>
        </template>
        <template slot-scope="{ row }" slot="valuationCurrency">
          <span>{{ row.RecordPrice }}</span>
        </template>
        <template slot-scope="{ row }" slot="SettlementCurrency">
          <span>{{ row.SettlePrice }}</span>
        </template>
        <template slot-scope="{ row }" slot="createdata">
          <span>{{ row.CreateDate }}</span>
        </template>
        <template slot-scope="{ row }" slot="user">
          <span>{{ row.Creator }}</span>
        </template>
      </Table>
      <!-- <div style="padding-top:15px;text-align: right;">
        <Page :total="100" show-elevator />
      </div> -->
    </div>
  </div>
</template>
<script>
import {
  currency,
  submitfeeHK,
  IncomeParters,
  IncomeRecordsForWarehouseFee
} from "../../api/CgApi";
import{Storagechargejson,InsertHKWarehouseFee} from "../../api/XdtApi";
export default {
    name:"HKStorageCharge",
    props: ["OrderID", "timedifference","TinyOrderID","Parentfun"],
  data() {
    return {
      Quantity: "",
      Labelfee: false, //是否是处理标签费
      Subject: [], //科目
      Currency: 3, //币种 ,默认是港币
      LeftPrice: "", //实收 实付金额
      unitprice: "", //单价
      disabledis: false, //根据科目判断是否可修改
      columns1: [
        {
          type: "index",
          width: 60,
          align: "center",
        },
        {
          title: "业务",
          slot: "type",
          align: "center",
        },
        {
          title: "科目",
          slot: "Subject",
          align: "center",
          width: 190,
        },
        {
          title: "收款人",
          slot: "TargetName",
          align: "center",
        },
        {
          title: "计价金额",
          slot: "valuationCurrency",
          align: "center",
        },
        {
          title: "结算金额",
          slot: "SettlementCurrency",
          align: "center",
        },
        {
          title: "发生时间",
          slot: "createdata",
          align: "center",
          width: 160,
        },
        {
          title: "录入人",
          slot: "user",
          align: "center",
        },
      ],
      PayeeID: "",
      PayerID: "",
      Payeearr:[],
      Payerarr:[],
      Anonymity: null, //名称
      // Files:[],
      showAnonymity: false,
      Subjectarr: [], //收入科目数组
      getselectedData: null, //将选中的科目转换为对象
      Files: [],

      Records: [],
      lableName:'',
      Premiums:[], //提交给华芯通的费用
    };
  },

  mounted() {

  },
    methods: {
      IncomeParters(OrderID) { //收入----收款人 付款人
        IncomeParters(OrderID).then(res => {
          this.Payeearr = res.data.Record.Payees;
          this.Payerarr = res.data.Record.Payers;
          this.PayeeID = res.data.Record.Payees[0].ID
          this.PayerID = res.data.Record.Payers[0].ID
        })
        this.getRecords(OrderID)
        this.getSubjectarr()
      },
      getRecords(OrderID) {
        IncomeRecordsForWarehouseFee(OrderID).then(res => {
          this.Records = res.data.Records
        })
      },
      getSubjectarr() {
        Storagechargejson().then(res => {
          this.Subjectarr = res
        })
      },
      changenumber(val) {
      if (val != "") {
        // var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/
        var testis = /^[0-9]*$/;
        var Result = testis.test(val);
        if (Result == true && val > 0) {
          this.LeftPrice = this.Quantity * this.unitprice * (this.timedifference);
        } else {
          // this.LeftPrice='';
          this.Quantity = "";
          this.$Message.error("请输入数量");
        }
      }
      },
      testprice(value) {  //验证金额
        if (value != '') {
          // var testis=/(^([-]?)[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^([-]?)(0){1}$)|(^([-]?)[0-9]\.[0-9]([0-9])?$)/;
          var testis = /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/
          var Result = testis.test(value)
          if (Result == false) {
            this.LeftPrice = '';
            this.$Message.error('请输入正确的金额，金额保留到小数点后两位');
          } else {

          }
        }
      },
    //切换科目标签
      //切换科目标签
      changecascader(value, selectedData) {
        var data = selectedData[selectedData.length - 1]
        //console.log(data)
        this.Quantity = ''
        if (data.Isquantity == true) {
          this.Labelfee = true
          this.unitprice = data.prices
          this.LeftPrice = data.prices
        } else {
          this.Labelfee = false
          this.unitprice = data.prices
          this.LeftPrice = data.prices
        }
        this.Currency = data.currency
        //console.log(this.Currency)
        var egs = selectedData.reverse()
        var result = {}
        var key = 'children'
        s(egs)
        function s(arrs) {
          arrs.forEach((o, i) => {
            if (!result) {
              result = o
              result[key] = []
            } else {
              o[key] = result
              result = o
            }
          })
        }
        this.getselectedData = result
      },

    currency_subject() {
      //币种
      currency().then((res) => {
        this.Currencyarr = res.obj;
      });
    },
    // 新科目收入提交
    submit_btn() {
      // console.log("币种"+this.Currency)
      // console.log("金额"+this.LeftPrice)
      if ( this.LeftPrice != "" && this.LeftPrice != null &&this.Subject[0] != "" ) {
        if (this.Labelfee == true && this.Quantity == "") {
          this.$Message.error("请输入数量");
        } else {
            var Source = null;
            if (sessionStorage.getItem("WareHouseName").indexOf("深圳") != -1) {
              Source = "深圳库房";
            } else {
              Source = "香港库房";
            }
            var conductdes = null;
            var Catalog = null;
            if (
              this.conduct == 30 ||
              this.conduct == 35 ||
              this.conduct == 60
            ) {
              conductdes = "代报关";
              Catalog = "杂费";
            } else {
              conductdes = "代仓储";
              Catalog = "仓储服务费";
            }
            var fee = {
              Currency: this.Currency, //币种
              FeeType: 'in', //类型 in out
              Files: this.Files, //文件
              LeftPrice: this.LeftPrice, //应收 应付
              OrderID: this.OrderID, //订单id
              Payee: this.PayeeID, //收款人
              Payer: this.PayerID, //付款人
              RightPrice: "", //实收实付
              Subject: '仓储费', //科目
              WaybillID: this.WaybillID,
              Quantity: this.Quantity, //标签给叔
              Anonymity: this.Anonymity, //匿名时候的名称
              Conduct: conductdes, //业务类型
              Source: Source,
              TrackingNumber: this.TrackingNumber,
              Catalog: Catalog,
              Data: this.getselectedData,
            };
            //console.log(fee);
            this.InsertHKWarehouseFee();
            submitfeeHK(fee).then((res) => {
              if (res.success == true) {
                this.$Message.success("保存成功");
                this.Subject = []; //科目
                this.Currency = ""; //币种 ,默认是港币
                this.LeftPrice = ""; //实收 实付金额
                this.Quantity = "";
                this.Files = [];
                this.getRecords(this.OrderID)
                this.$emit("Parentfun")
              } else {
                this.$Message.error(res.data);
              }
            });

        }
      } else {
        this.$Message.error("请输入必填项");
      }
    },
    InsertHKWarehouseFee(){  
      //1是CNY,3是HKD  
      var submitCurrency = "HKD";
      if(this.Currency==1){
         submitCurrency = "CNY";
      } 
      var data = {      
         TinyOrderID:this.TinyOrderID,
         AdminID:sessionStorage.getItem("userID"),        
         Subject:"仓储费",
         //WhesFeeType:2,
         Count:this.Quantity,
         UnitPrice:this.LeftPrice,
         Currency:submitCurrency,
         PaymentType:1,         
      };
      this.Premiums.push(data);
      var submit_data = {
        "Premiums":this.Premiums
      }
      InsertHKWarehouseFee(submit_data).then((res) => {
              if (res.success == true) {
                //this.$Message.success("华芯通保存成功");  
                this.Premiums = [];         
              } else {
                this.$Message.error(res.data);
                this.Premiums = [];
              }              
            });
    }
  },
};
</script>
