<style scoped>
.buttonstyle {
  margin:10px 0;
}
label{
  font-size: 16px;
  font-weight: bold;
}
.infodatastyle span{
  padding-right: 15px;
}
</style>
<template>
  <div>
    <Spin size="large" fix v-if="spinShow"></Spin>
    <div v-if="infodata!=null" class="infodatastyle">
      <span><label>库位 ：</label>{{ infodata.ShelveCode }}</span>
      <span><label>尺寸：</label>{{ infodata.Size }}</span>
      <span><label>所属公司：</label>{{ infodata.Company }}</span>
    </div>
    <Button type="success" class="buttonstyle" size="small" icon="ios-print-outline" @click="printlabel('all')">打印库存标签</Button >
    <div v-if="infodata!=null">
      <Table
        border
        ref="selection"
        :columns="columns4"
        :data="infodata.Storages"
        @on-selection-change='changeselection'
      >
        <!-- <template slot-scope="{ row, index }" slot="total">
          <div v-if="row.Mpq > 1">
            {{ row.Mpq }}&nbsp;/&nbsp;{{ row.PackageNumber}}&nbsp;/&nbsp;{{
              row.Total
            }}
          </div>
          <div v-else>
            {{ row.Total }}
          </div>
        </template> -->
      </Table>
    </div>
  </div>
</template>
<script>
import { ShelveDetail } from "../../api/Storages";
import {	TemplatePrint,	GetPrinterDictionary	} from "../../js/browser";
let product_url = require("../../../static/pubilc.dev");
export default {
  data() {
    return {
      spinShow:true,
      detailID: null,
      infodata: null,
      currentIndex: null,
      columns4: [
        {
          type: "selection",
          width: 60,
          align: "center",
        },
        {
          title: "订单号",
          key: "OrderID",
          align: "center",
        },
        {
          title: "客户",
          key: "ClientName",
          align: "center",
        },
        {
          title: "型号",
          key: "Partnumber",
          align: "center",
        },
        {
          title: "品牌",
          key: "Brand",
          align: "center",
        },
        {
          title: "封装",
          key: "Package",
          align: "center",
        },
        {
          title: "批次",
          key: "DateCode",
          align: "center",
        },
        // {
        //   title: "数量",
        //   slot: "total",
        //   align: "center",
        // },
         {
          title: "Mpq",
          key: "Mpq",
          align: "center",
        },
        {
          title: "件数",
          key: "PackageNumber",
          align: "center",
        },
        {
          title: "总数",
          key: "Total",
          align: "center",
        },
        {
          title: "备注",
          key: "Summary",
          align: "center",
        },
      ],
      selection: [],
      printurl: product_url.pfwms,
    };
  },
  created() {
    this.detailID = this.$route.params.detailID;
    this.ShelveDetail(this.detailID);
  },
  methods: {
    ShelveDetail(detailID) {
      ShelveDetail(detailID).then((res) => {
        this.infodata = res.data;
        this.spinShow=false;
      });
    },
    changeselection(value){
      this.selection=value
    },
    printlabel(){
       if(this.selection.length>0){
          for(var i=0,len=this.selection.length;i<len;i++){
            this.printfun(this.selection[i])
          }
        }else{
          this.$Message.warning('请选择要打印的标签');
        }
    },
    printfun(row){
         var configs = GetPrinterDictionary();
          var getsetting = configs["库存标签"];
          var str = getsetting.Url;
          var testurl = str.indexOf("http") != -1;
          if (testurl == true) {
            getsetting.Url = getsetting.Url;
          } else {
            getsetting.Url = this.printurl + getsetting.Url;
          }
          var data = {
            setting: getsetting,
            data: [{
              listdata: row
            }]
          };
          TemplatePrint(data);
    },
  },
};
</script>