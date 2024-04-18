<style scoped>
.setright {
   padding-right: 15px;
   display: inline-block;
    padding-top: 10px;
}
.mastcolor{
  color:red;
}
</style>
<template>
  <div>
    <div class="setbottom">
      <span>订单号：{{sumbitChargesdata.FormID}}</span
      ><span style="padding-left: 20px">客户：{{sumbitChargesdata.ClientName}}</span>
    </div>
    <div>
      <span class="setright">
        <label><span class="mastcolor">*</span>类型:</label>
        <Select v-model="Chargesdata.Type" style="width: 180px" @on-change='changetype'>
          <Option
            v-for="item in TypeList"
            :value="item.value"
            :key="item.value"
            >{{ item.label }}</Option
          >
        </Select>
      </span>
       <span class="setright" v-if="Chargesdata.Type=='支出'">
        <label><span class="mastcolor">*</span>收款人:</label>
        <Select v-model="Chargesdata.TakerID" style="width: 180px">
          <Option
            v-for="item in TakerList"
            :value="item.ID"
            :key="item.ID"
            >{{ item.Name }}</Option
          >
        </Select>
      </span>
      <span class="setright">
        <label><span class="mastcolor">*</span>科目:</label>
        <Input
          v-model="Chargesdata.Subject"
          placeholder="请输入科目"
          style="width: 180px"
        />
      </span>
      <span class="setright">
        <label><span class="mastcolor">*</span>单价:</label>
        <Input
          v-model="Chargesdata.UnitPrice"
          placeholder="请输入单价"
          style="width: 180px"
          @on-blur="computtotal_UnitPrice"
        />
      </span>
      <span class="setright">
        <label><span class="mastcolor">*</span>数量:</label>
        <Input
          v-model="Chargesdata.Quantity"
          placeholder="请输入数量"
          style="width: 180px"
          @on-blur="computtotal_Quantity"
        />
      </span>
      <span class="setright">
        <label v-if="Chargesdata.Type=='支出'">总&nbsp;&nbsp;&nbsp;价:</label>
        <label v-else>总价:</label>
        <Input
          v-model="Chargesdata.Total"
          placeholder="请输入总价"
          disabled
          style="width: 180px"
        />
      </span>
    </div>
  </div>
</template>
<script>
import { NoticeChargesEnter,} from "../../api/index";
import { TakersList} from "../../api/Out"
export default {
  props: {
    sumbitChargesdata: {
      type: Object,
      default() {
        return {};
      },
    },
  },
  data() {
    return {
      TypeList: [
        {
          value: "收入",
          label: "收入",
        },
        {
          value: "支出",
          label: "支出",
        },
      ],
      Chargesdata: {
        AdminID:sessionStorage.getItem("userID"),
        FormID:null,
        NoticeID:null, //是	string	通知ID
        Type: '收入', //是	string	费用类型：收入或支出
        Source: 1, //是	string	费用来源：库房或跟单
        PayerID: null, //是	string	付款人ID
        PayeeID: null, //是	string	收款人ID
        TakerID: null, //是	string	收款人ID（个人）
        Conduct: 1, //是	string	业务类型:代仓储
        Subject: null, //是	string	费用科目
        Currency: 1, //是	string	币种
        Quantity: null, //是	string	数量
        UnitPrice: null, //是	string	单价
        Total: null, //是	string	总价
      },
      TakerList:[],
      showCharges:false,//是否显示费用弹出框,
      // sumbitChargesdata: {
      //   NoticeID: null, //是	string	通知ID
      //   ClientID:null
      // },
    };
  },
  created() {},
  mounted() {},
  methods: {
    TakersList(){
      TakersList().then(res=>{
        this.TakerList=res.data
      })
    },
    changetype(value){
      if(value=='支出'){
        this.TakersList()
      }
    },
    computtotal_Quantity() {
      if(!this.Chargesdata.Quantity!=true){
        var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d)))$/
        var Result = testis.test(this.Chargesdata.Quantity)
          if(Result==false){
              this.Chargesdata.Quantity=null;
              // this.$Message.warning('请输入整数');
              this.$Message["warning"]({
                background:true,
                content: "请输入整数",
              });
          }else{
             this.Chargesdata.Total =this.Chargesdata.Quantity * this.Chargesdata.UnitPrice;
             console.log(this.Chargesdata.Total);
          }
      }
    },
     computtotal_UnitPrice() {
      if(!this.Chargesdata.UnitPrice!=true){
        var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,3})))$/
        var Result = testis.test(this.Chargesdata.UnitPrice)
          if(Result==false){
              this.Chargesdata.UnitPrice=null;
              // this.$Message.warning('请输入单价,保留到小数点后三位');
              this.$Message["warning"]({
                background:true,
                content: "请输入单价,保留到小数点后三位",
              });
          }else{
             this.Chargesdata.Total =this.Chargesdata.Quantity * this.Chargesdata.UnitPrice;
             console.log(this.Chargesdata.Total);
          }
      }
    },
    submitbtn() {
     this.Chargesdata.NoticeID=this.sumbitChargesdata.NoticeID;
     this.Chargesdata.FormID=this.sumbitChargesdata.FormID
     if(this.Chargesdata.Type=='收入'){
      //  DBAEAB43B47EB4299DD1D62F764E6B6A 芯达通ID 收入的收款人
      if(!this.Chargesdata.Subject||!this.Chargesdata.Quantity||!this.Chargesdata.UnitPrice){
        this.$Message["warning"]({
          background:true,
          content: "请输入必填项",
        });
      }else{
        this.Chargesdata.PayeeID='DBAEAB43B47EB4299DD1D62F764E6B6A'  //收款人
        this.Chargesdata.PayerID=this.sumbitChargesdata.ClientID;    //付款人
        this.NoticeChargesEnter(this.Chargesdata)
      }
     }else{
          if(!this.Chargesdata.Subject||!this.Chargesdata.Quantity||!this.Chargesdata.UnitPrice||!this.Chargesdata.TakerID){
             this.$Message["warning"]({
              background:true,
              content: "请输入必填项",
            });
          }else{
            this.Chargesdata.PayerID='DBAEAB43B47EB4299DD1D62F764E6B6A' //付款人
            this.Chargesdata.PayeeID=null
            this.NoticeChargesEnter(this.Chargesdata)
          }
     }
     
     
    },
   NoticeChargesEnter(data){
      NoticeChargesEnter(data).then((res) => {
        if (res.Success == true) {
          // this.$Message.success('费用录入成功'); 
           this.$Message["success"]({
            background:true,
            content: "费用录入成功",
          });
          setTimeout(()=>{
              this.$emit("fatherMethod", false);
              this.cancelbtn()
          },300)  
        } else {
          // this.$Message.error(res.Data);
          this.$Message["error"]({
            background:true,
            content: res.Data,
          });
          this.$emit("fatherMethod", true);
          this.cancelbtn()
        }
      });
    },

    cancelbtn(){
      this.Chargesdata={
        FormID:null,
        NoticeID:null, //是	string	通知ID
        Type: '收入', //是	string	费用类型：收入或支出
        Source: 1, //是	string	费用来源：库房或跟单
        PayerID: null, //是	string	付款人ID
        PayeeID: null, //是	string	收款人ID
        TakerID: null, //是	string	收款人ID（个人）
        Conduct: 1, //是	string	业务类型:代仓储
        Subject: null, //是	string	费用科目
        Currency: 1, //是	string	币种
        Quantity: null, //是	string	数量
        UnitPrice: null, //是	string	单价
        Total: null, //是	string	总价
      }

    }
  },
};
</script>