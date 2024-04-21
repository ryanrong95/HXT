<template>
  <div class="product">
    <div v-if="print_data!=null">
      <div class="barcodebox">
        <barcode :value="print_data.inputsID" :options="barcode_option" tag="svg"></barcode>
        <!-- <p>{{print_data.ProductID}}</p> -->
      </div>
      <!-- <p>产品编号:{{print_data.ProductID}}</p> -->
      <p>
        <span>型号:{{print_data.PartNumber}}</span>
        <span>品牌:{{print_data.Manufacturer}}</span>
      </p>
      <p>
        <span>包装:{{print_data.Packing}}</span>
        <span>封装:{{print_data.PackageCase}}</span>
      </p>
      <p>
        <span>产地:{{print_data.origin}}</span>
        <span>数量:{{print_data.Quantity}}</span>
      </p>
    </div>
  </div>
</template>
<script>
//import { PageEvent } from "@/js/browser.js"
import VueBarcode from "@xkeshi/vue-barcode"; //导入条形码插件
let Base64 = require("js-base64").Base64;
var ps = window.location.href.split("?");
if (ps.length > 1) {
  var obj = JSON.parse(Base64.decode(ps[1]));
} else {
  var obj = null;
  obj={
    ID:111,
    inputsID:869769579657597,//产品ID
    Catalog:111111,//品名
    PartNumber:1111111,//型号
    Quantity:200,//数量
    Manufacturer:11111,//品牌
    Packing:11111,//包装
    PackageCase:1111111,//封装
    origin:111111,//产地
  }
}

export default {
  name: "productlabel",
  components: {
    barcode: VueBarcode
  },
  data() {
    return {
      print_data: obj,
      barcode_option: {
        displayValue: false, //是否默认显示条形码数据
        //textPosition  :'top', //条形码数据显示的位置
        background: "#fff", //条形码背景颜色
        valid: function(valid) {},
        width: "1.7px", //单个条形码的宽度
        height: "40px",
        fontSize: "16px", //字体大小
        format: "CODE128", //选择要使用的条形码类型
        margin: 2
      }
    };
  },
  //computed:{
  //    print_data(){

  //        return pobj;

  //    }
  //},
  methods: {
    getdata() {
      console.log(this.$route.params);
      // PageEvent("{\"Name\":\"printingpostback\"}");
    }
  },
  created() {},
  mounted() {
    console.log(this.print_data);
    //this.print_data = Obj;
    // alert(this.print_data.name)
  }
};
</script>
<style scoped>
.product {
  width: 8cm;
  height: 6cm;
  overflow: hidden;
  /* border: 1px solid #ddd; */
}
.boxprint {
  margin: 0;
  padding: 10px;
}
p {
  font-size: 16px;
  /* line-height: 32px; */
  padding: 10px;
  display: flex;
  /* display: flex;
     justify-content: space-between; */
}
p span {
  text-align: left;
  width: 50%;
}
.barcodebox {
  padding: 10px;
  padding-bottom: 0px;
}
</style>
