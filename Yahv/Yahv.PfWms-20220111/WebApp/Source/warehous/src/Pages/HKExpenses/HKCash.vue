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

  .spanMust {
    color: red;
  }

  .setpadding_bottom {
    padding-bottom: 15px;
    font-size: 16px;
  }
</style>
<template>
  <div class="cashbox">

    <div>
      <div>
        <h1 class="setpadding_bottom">费用录入</h1>
        <p class="iteminput">
          <label for>收款人：</label>
          <!-- <Input v-model="Payee" placeholder="请输入收款人" class="inputwidth" /> -->
          <Select v-model="PayeeID"
                  filterable
                  class="inputwidth"
                  transfer
                  :disabled="this.Payeearr.length==1?true:false"
                  @on-change='changePayeeID'>
            <Option v-for="(item,index) in this.Payeearr" :value="item.ID" :key="index">{{item.Name}}</Option>
          </Select>
        </p>
        <p class="iteminput" v-if="this.Payeearr.length>1&&PayeeID=='A235E8E7773EDB26697E7E771915197D'">
          <label for><span class="spanMust">*</span>名&nbsp;&nbsp;&nbsp;称：</label>
          <Input v-model="Anonymity" placeholder="请输入名称" class="inputwidth" />
        </p>
        <p class="iteminput">
          <label for>付&nbsp;款&nbsp;&nbsp;人：</label>
          <!-- <Input v-model="Payer" placeholder="请输入付款人" class="inputwidth" /> -->
          <Select v-model="PayerID"
                  filterable
                  class="inputwidth"
                  transfer
                  :disabled="this.Payerarr.length==1?true:false">
            <Option v-for="(item,index) in this.Payerarr" :value="item.ID" :key="index">{{item.Name}}</Option>
          </Select>
        </p>
        <p class="iteminput" v-if="this.Payerarr.length>1&&PayerID=='A235E8E7773EDB26697E7E771915197D'">
          <label for><span class="spanMust">*</span>名&nbsp;&nbsp;&nbsp;称：</label>
          <Input v-model="Anonymity" placeholder="请输入名称" class="inputwidth" />
        </p>
        <p class="iteminput">
          <label for><span class="spanMust">*</span>科&nbsp;&nbsp;&nbsp;目：</label>
          <Select v-model="Subject"
                  filterable
                  class="inputwidth"
                  transfer
                  @on-change="change_subject">
            <Option v-for="(item,index) in this.Subjectarr" :value="item.Name" :key="item.index">{{item.Name}}</Option>
          </Select>
        </p>

        <!-- 快递运费时 -->
        <p class="iteminput" v-if="Subject=='快递运费'&&EnterType=='out'">
          <label for><span class="spanMust">*</span>运单号：</label>
          <Select v-model="TrackingNumber" filterable class="inputwidth" transfer :disabled="disabledis">
            <Option v-for="(item,index) in TrackingNumberarr" :value="item.wbCode" :key="item.index"> {{ item.wbCode }}</Option>
          </Select>
        </p>


        <p class="iteminput" v-if="Labelfee==true">
          <label for><span class="spanMust">*</span>数&nbsp;&nbsp;&nbsp;量：</label>
          <Input v-model="Labeltotal" placeholder="请输入金额" @on-blur="Labelchange(Labeltotal)" class="inputwidth" />
        </p>
        <p class="iteminput">
          <label for><span class="spanMust">*</span>币&nbsp;&nbsp;&nbsp;种：</label>
          <Select v-model="Currency" filterable class="inputwidth" transfer>
            <Option v-for="(item,index) in Currencyarr" :value="item.value" :key="item.index">{{item.name}}</Option>                    
          </Select>
        </p>
        <p class="iteminput">
          <label for><span class="spanMust">*</span>金&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;额：</label>
          <Input v-model="RightPrice" @on-blur="testprice(RightPrice)" placeholder="请输入金额" class="inputwidth" />
        </p>
        <!-- <Button type="primary" @click="submit_btn">提交</Button> -->
        <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUploadFile">传照</Button>
        <Button size="small" type="primary" icon="md-reverse-camera" @click="FormPhoto">拍照</Button>
        <h3 style="padding-bottom:5px">文件列表：</h3>
        <ul v-if="Files.length>0">
          <li v-for="item in Files">
            {{item.CustomName}}
            <Icon type="ios-trash" @click="handleRemove(item)" />
          </li>
        </ul>
        <div v-else>暂无数据</div>
        <div style="padding:15px 0">
          <Button type="primary" size="small" icon='md-checkmark' @click="submit_btn" :disabled="submitDisable">提交</Button>
        </div>
      </div>
      <div>
        <h1 class="setpadding_bottom">流水列表</h1>
        <Table stripe :columns="columns1" :data="Currents" max-height="380">
          <template slot-scope="{ row }" slot="type">
            <span>{{row.Conduct}}</span>
          </template>
          <template slot-scope="{ row }" slot="Subject">
            <span>{{row.Subject}}</span>
          </template>
          <template slot-scope="{ row }" slot="TargetName">
            <span>{{row.TargetName}}</span>
          </template>
          <template slot-scope="{ row }" slot="TrackingNumber" v-if="EnterType=='out'">
            <span>{{row.TrackingNumber}}</span>
          </template>
          <template slot-scope="{ row }" slot="RightPrice">
            <span>{{row.Price}}</span>
          </template>
          <template slot-scope="{ row }" slot="createdata">
            <span>{{row.CreateDate}}</span>
          </template>
          <template slot-scope="{ row }" slot="user">
            <span>{{row.Creator}}</span>
          </template>
        </Table>
      </div>
    </div>
  </div>
</template>
<script>
import { currency, subject, submitfeeHK ,CgDeleteFiles,GetWaybillCodeByCarrierID} from "../../api/CgApi";
import{InsertHKWarehouseFee,OrderWaybillInfo} from "../../api/XdtApi";
import{ SeletUploadFile,FormPhoto} from '../../js/browser'
export default {
   props: ["Currencyarr", "Subjectarr",'Payeearr','Payerarr','OrderID','WaybillID','Currents','conduct','Files','delCash','EnterType','TinyOrderID'],
  data() {
    return {
      Labeltotal:'',
      Labelfee:false, //是否是处理标签费
      spinShow:true,
      Subject: "", //科目
      Currency: 3, //币种 默认港币
      RightPrice: "", //实收 实付金额
      disabledis: false, //根据科目判断是否可修改
      submitDisable:false,//提交按钮是否可用
      columns1: [
        {
            type: 'index',
            width: 60,
            align: 'center'
        },
        {
          title: "业务",
          slot: "type",
          align: "center"
        },
        {
          title: "科目",
          slot: "Subject",
          align: "center"
        },
        {
          title: "收款人",
          slot: "TargetName",
          align: "center"
        },
        {
          title: "运单号",
          slot: "TrackingNumber",
          align: "center"
        },
        {
          title: "金额",
          slot: "RightPrice",
          align: "center"
        },
        {
          title: "发生时间",
          slot: "createdata",
          align: "center",
          width:160
        },
        {
          title: "录入人",
          slot: "user",
          align: "center"
        }
      ],
      PayeeID:"",//收款人
      PayerID:"",//付款人,
      Anonymity:null,//名称
      TrackingNumber:'', //运单号
      TrackingNumberarr:[], //所对应的运单号数组
      // Files:[],
      Premiums:[], //提交给芯达通的费用
      WaybillInfo:[],//快递信息
    };
  },
  watch:{
      theUrl:{
        Currents(newValue, oldValue) {
           //刷新audio的配置，不让它受上一条影响
          this.refreshAudio()
            //把新拿到的值更新到url上
          this.url = (newValue!="")? this.Commonconst.recordForwardPrefix + newValue : ''
        },
        deep: true
      }
    },
  mounted() {
      // window["PhotoUploaded"] = this.changedimg;
      var WareHouseName=sessionStorage.getItem("WareHouseName")
      if(WareHouseName.indexOf('香港')!=-1&&this.EnterType=='in'){
          this.Subject="入仓费"
      }
      if(this.EnterType=='in'){
        this.columns1[3].title="付款人"
        this.columns1.splice(this.columns1.findIndex(item => item.title === "运单号"), 1)
        //console.log(this.columns1)
      }else{
        this.columns1[3].title="收款人"
      }
      OrderWaybillInfo(this.TinyOrderID).then(res => {                  
                    this.WaybillInfo = res.obj;       
                });
  },

  methods: {
    testprice(value){  //验证金额
        if(value!=''){
            // var testis=/(^([-]?)[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^([-]?)(0){1}$)|(^([-]?)[0-9]\.[0-9]([0-9])?$)/;
           var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/
            var Result = testis.test(value)
            if(Result==false){
                this.RightPrice='';
                this.$Message.error('请输入正确的金额，金额保留到小数点后两位');
            }
        }
     },
     Labelchange(val){
      if(val!=''){
            // var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/
            var testis=/^[0-9]*$/
            var Result = testis.test(val)
            if(Result==false){
                this.RightPrice='';
                this.$Message.error('请输入处理标签的数量');
            }else{
               this.disabledis = true;
               this.Currency = 1;
               if(val<=10){
                 this.RightPrice=100
               }else{
                 var num1=val-10
                 var number=num1*5
                 this.RightPrice=100+number
               }


            }
      }
     },
     changePayeeID(value){
       if(this.Subject=="快递运费"){
         this.TrackingNumber=''
        //  GetWaybillCodeByCarrierID(this.OrderID,value).then(res=>{
        //   this.TrackingNumberarr=res.data
        // })
        this.GetWaybillCode(this.PayeeID);
       }
     },
    change_subject(value) {
      if(value=='处理标签费'){
          this.Labelfee=true;
      }else{
         this.Labelfee=false;
      }
      var newarr = [];
      newarr = this.Subjectarr.filter(
        item => item.Name == value && item.PayQuote.Price != null
      );
      //console.log(newarr);
      var obj = newarr[0];
      if (newarr.length > 0) {
        this.disabledis = true;
        this.Currency = obj.PayQuote.Currency;
        this.RightPrice = obj.PayQuote.Price;
        // this.rightprice=obj.PayQuote.Price;   //实收实付
        // this.rightprice="";   //实收实付
        // this.leftprice=obj.PayQuote.Price;
        // this.formdata.currency=obj.PayQuote.Currency;
      } else {
        if(value=='处理标签费'){
          this.disabledis = true;
          this.Currency = 1
        }else{
          this.disabledis = false;
          this.Currency = 3
          this.LeftPrice = ""
        }
      }
      if(value=='快递运费'){
        // GetWaybillCodeByCarrierID(this.OrderID,this.PayeeID).then(res=>{
        //   this.TrackingNumberarr=res.data
        // })
        this.GetWaybillCode(this.PayeeID);
      }
    },
    currency_subject() {
      //币种
      currency().then(res => {
        this.Currencyarr = res.obj;
      });
    },
    submit_btn() {
      var id='A235E8E7773EDB26697E7E771915197D'
      if(this.Subject==""||this.Currency==""||this.RightPrice==""||((this.PayeeID==id||this.PayerID==id)&&(this.Anonymity==null||this.Anonymity==''))){
         this.$Message.error('请输入必填项');
      }else{
        if(this.Subject=='处理标签费'&&this.Labeltotal==''){
           this.$Message.error('请输入处理标签的个数');
        }else{
          if(this.Subject=='快递运费'&&this.TrackingNumber==''&&this.EnterType=='out'){

          }else{
            var Source=null
            if(sessionStorage.getItem("WareHouseName").indexOf('深圳')!=-1){
              Source='深圳库房'
            }else{
              Source='香港库房'
            }
          var conductdes=null
            var Catalog=null
            if(this.conduct==30||this.conduct==35||this.conduct==60){
              conductdes='代报关'
              Catalog="杂费"
            }else{
              conductdes='代仓储'
              Catalog="仓储服务费"
            }
          var fee = {
            Currency: this.Currency, //币种
            FeeType: "out", //类型 in out
            Files: this.Files, //文件
            LeftPrice: this.RightPrice, //应收 应付
            OrderID: this.OrderID, //订单id
            Payee: this.PayeeID, //收款人
            Payer: this.PayerID, //付款人
            RightPrice: this.RightPrice, //实收实付
            Subject: this.Subject, //科目
            WaybillID: this.WaybillID,
            Quantity:this.Labeltotal,
            Anonymity:this.Anonymity,//匿名时候的名称
            Conduct:conductdes,//业务类型
            Source:Source,
            TrackingNumber:this.TrackingNumber,
            Catalog:Catalog
          };
          this.submitDisable = true;
          this.InsertHKWarehouseFee(fee);
            submitfeeHK(fee).then(res => {
              if (res.success == true) {
                this.$Message.success("保存成功");
                this.Subject = ""; //科目
                this.Currency = 3; //币种 ,默认是港币
                this.LeftPrice = ""; //实收 实付金额
                this.Labeltotal=''
                this.Files=[],
                this.RightPrice='',
                this.Anonymity=''
                this.$emit('fatherMethod');
                this.$emit('delCash')
              } else {
                this.$Message.error(res.data);
              }
              this.submitDisable = false;
            });
          }

        }

      }

    },
     SeletUploadFile(){
      var data = {
        SessionID: 'Cash',
        AdminID: sessionStorage.getItem("userID"),
        Data:{
          WayBillID:this.WaybillID,
          WsOrderID:this.OrderID,
          Type:8000
        }
      };
        SeletUploadFile(data);
    },
    FormPhoto(){
      var data = {
        SessionID:'Cash',
        AdminID: sessionStorage.getItem("userID"),
        Data:{
          WayBillID:this.WaybillID,
          WsOrderID:this.OrderID,
          Type:8000
        }
      };
      FormPhoto(data)
    },
    //后台调用winfrom 拍照的方法
    changedimg(message) {
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
      };
      this.Files.push(newfile)
      // alert(JSON.stringify(this.Files))
    },
       //删除上传文件按
    handleRemove(file) {
       var data={
          id:file.ID
        }
       CgDeleteFiles(data).then(res=>{
          //console.log(res)
          if(res.Success==true){
            this.$Message.success('删除成功')
             this.Files.splice(this.Files.indexOf(file), 1);
          }else{
            this.$Message.error('删除失败')
          }
      })
    },
    InsertHKWarehouseFee(fee){  
      //1是CNY,3是HKD  
      var submitCurrency = "HKD";
      if(this.Currency==1){
         submitCurrency = "CNY";
      } 
      var data = {      
         TinyOrderID:this.TinyOrderID,
         AdminID:sessionStorage.getItem("userID"),        
         Subject:fee.Subject, //科目,
         //WhesFeeType:2,
         Count:fee.Quantity==""?1:this.Quantity,
         UnitPrice:this.RightPrice,
         Currency:submitCurrency,
         PaymentType:1,         
      };
      this.Premiums.push(data);
      var submit_data = {
        "Premiums":this.Premiums
      }
      InsertHKWarehouseFee(submit_data).then((res) => {
              if (res.success == true) {
                //this.$Message.success("芯达通保存成功"); 
                this.Premiums = [];          
              } else {
                this.$Message.error(res.data);
                this.Premiums = [];
              }
            });
    },
     GetWaybillCode(carrierID){
       this.TrackingNumberarr = [];
        for (var i = 0, lens = this.WaybillInfo.length; i < lens; i++) {              
             if(this.WaybillInfo[i].CarrierID==carrierID){
               this.TrackingNumberarr.push({"wbCode":this.WaybillInfo[i].WaybillNo});
             }       
        }
    }
  }
};
</script>
