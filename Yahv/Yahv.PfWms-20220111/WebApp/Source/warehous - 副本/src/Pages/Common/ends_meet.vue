<style scoped>
.formtitle{
    padding-left:10px;
}
.inputs{
    width:188px;
}
/* .titlelable{
    font-size: 14px;
}
.titlelable em{
    font-size: 12px;
} */
</style>
<template>
     <div>
        <div>
            <h1 style="font-size: 18px;position: absolute;top:13px;">收支明细</h1>
            <div style="padding:10px 0px;font-size: 14px;">
               <span style="margin-right: 10px;" class="titlelable">发货方式：<em>{{getformdata.WaybillTypeDescription}}</em> </span>
               <span style="margin-right: 10px;" class="titlelable" v-if="getformdata.WaybillType!=1">运单号：<em>{{getformdata.Code}}</em> </span>
               <span style="margin-right: 10px;"class="titlelable" >入仓号：<em>{{getformdata.EnterCode}}</em></span>
               <span style="margin-right: 10px;"class="titlelable" >公司名称：<em>{{getformdata.ClientName}}</em></span>
            </div>
            <p style="padding-bottom:10px">
                    <span>业务类型：</span>
                    <RadioGroup v-model="detailtypes" @on-change="detailchange">
                        <Radio label="1">收入</Radio>
                        <Radio label="2">支出</Radio>
                    </RadioGroup>
            </p>
            <div style="padding-top:10px;" >
                 <Table height="300" :columns="listtitle" :data="listin" v-if="detailtypes==1" :loading="loading">
                     <template slot-scope="{ row }" slot="OriginalDate">
                         <span>{{row.LeftDate|showDate}}</span>
                     </template>
                     <template slot-scope="{ row }" slot="action">
                       <p v-for="(item,index) in row.Files" :key="index">{{item.CustomName}}</p>
                    </template>
                 </Table>
                 <Table height="300" :columns="listtitleout" :data="listout" v-if="detailtypes==2" :loading="loading">
                     <template slot-scope="{ row }" slot="OriginalDate">
                         <span>{{row.LeftDate|showDate}}</span>
                     </template>
                     <template slot-scope="{ row }" slot="action">
                      <p v-for="(item,index) in row.Files" :key="index">
                          <span>{{item.CustomName}}</span>
                      </p>
                    </template>
                 </Table>
            </div>
             <div style="padding: 10px 5px; border: 1px solid #dddddd;margin-top: 10px;">
                 <h1 style="font-size:18px;padding-bottom:5px;">
                     录入收支信息
                     <em style="font-size:12px;color:red;">*为必填项</em>
                </h1>
                <p style="padding-bottom:10px">
                    <span>业务类型：</span>
                    <RadioGroup v-model="types" @on-change="change">
                        <Radio label="1">收入</Radio>
                        <Radio label="2">支出</Radio>
                    </RadioGroup>
                </p>
                <div>
                    <div style="padding-bottom:10px">
                        <label>
                             <span class=""><em style="font-size:12px;color:red;">*</em> 付款方：</span> 
                             <Input v-model="formdata.payer" disabled  placeholder="付款方" class="inputs" />
                        </label>
                        <label >
                            <span class="formtitle"><em style="font-size:12px;color:red;">*</em>收款方：</span>
                            <Input v-model="formdata.PayeeName" disabled  placeholder="收款方" class="inputs" />
                        </label>
                        <!-- <label>
                            <span class="formtitle">批次：</span>
                            <Select v-model="formdata.DateCode" class="inputs" placeholder="选择批次">
                                <Option v-for="(item,index) in DateCodearr" :value="item" :key="index">
                                    <span style="float:right;">{{item}}</span>
                                </Option>
                            </Select>
                        </label> -->
                        <label > 
                            <span class="formtitle"><em style="font-size:12px;color:red;">*</em>科目：</span>
                            <Select v-model="formdata.Subject" class="inputs" placeholder="选择科目" @on-change="change_subject">
                                <Option v-for="item in subject" :value="item.Name" :key="item.Name">{{ item.Name }}</Option>
                            </Select>
                        </label>
                         <label for="">
                            <span class="">
                                <em style="font-size:12px;color:red;">*</em>
                               币&nbsp;&nbsp;&nbsp;种： 
                            </span>
                            <Select v-model="formdata.currency" class="inputs" placeholder="选择币种" :disabled='disabledis'>
                                <Option v-for="item in this.currency" :value="item.value" :key="item.value">{{ item.name }}</Option>
                            </Select>
                        </label>
                    </div>
                    <div>
                        <!-- <label for="">
                            <span class="">
                                <em style="font-size:12px;color:red;">*</em>
                               币&nbsp;&nbsp;&nbsp;种： 
                            </span>
                            <Select v-model="formdata.currency" class="inputs" placeholder="选择币种" :disabled='disabledis'>
                                <Option v-for="item in this.currency" :value="item.value" :key="item.value">{{ item.name }}</Option>
                            </Select>
                        </label> -->
                        <label>
                             <span v-if="placeholderleft=='应收'" class=""><em style="font-size:12px;color:red;">*</em>应&nbsp;&nbsp;&nbsp; 收：</span>
                             <span v-if="placeholderleft=='应付'" class=""><em style="font-size:12px;color:red;">*</em>应&nbsp;&nbsp;&nbsp; 付：</span>
                            <Input v-model="leftprice" :disabled='disabledis' :placeholder="placeholderleft" @on-blur="testprice(leftprice,'leftprice')" class="inputs" />
                        </label>
                        <label >
                             <span v-if="placeholderright=='实收'" class="formtitle">实&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;收：</span>
                             <span v-if="placeholderright=='实付'" class="formtitle">实&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;付：</span>
                            <Input v-model="rightprice"  :placeholder="placeholderright" @on-blur="testprice(rightprice,'rightprice')" class="inputs" />   
                        </label>
                    <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUpload('Waybill')">传照</Button>
                    <Button type="primary" size="small" icon="md-reverse-camera" @click="fromphotos('Waybill')">拍照</Button>
                    </div>
                    <div style="padding: 10px 0px;">
                        <span >单据：</span>
                        <ul>
                            <li v-for="item in Files">  
                                <span>{{item.CustomName}}</span>
                                <Icon type="ios-trash" @click="handleRemovelist(item)"/>
                            </li>
                        </ul>
                      <Button type="primary" size="large" style="float: right;margin-right: 20px;position:relative;bottom: 30px;" @click="sumbit">提交</Button>                        
                    </div>
              </div> 
            </div>
           
        </div>
    </div> 
</template>
<script>
import imgtest from "@/Pages/Common/imgtes";
import {Paymentslist,currency,subject,submitfee,History} from "../../api";
import {FormPhoto,SeletUploadFile,PictureShow} from "@/js/browser.js"
import moment from "moment";
export default {
    components: {
        "img-test": imgtest,
    },
    data() {
        return {
            otype:"",
            loading:true,
            placeholderleft:"应收",
            placeholderright:"实收",
            model9:"",
            types:"1",  //收支类型 1 为收入 2为支出
            value1:"",
            switch1:true,
            cityList: [
                    {
                        value: 'New York',
                        label: 'New York'
                    },
                    {
                        value: 'London',
                        label: 'London'
                    },
                    {
                        value: 'Sydney',
                        label: 'Sydney'
                    },
                    {
                        value: 'Ottawa',
                        label: 'Ottawa'
                    },
                    {
                        value: 'Paris',
                        label: 'Paris'
                    },
                    {
                        value: 'Canberra',
                        label: 'Canberra'
                    }
                ],
            model1: '',
            listtitle: [
                    {
                        title:"录入时间",
                        slot: 'OriginalDate'
                    },
                    {
                        title: '科目',
                        key: 'Subject'
                    },
                    {
                        title: '币种',
                        key: 'CurrencyName'
                    },
                    {
                        title: '应收',
                        key: 'LeftPrice'
                    },
                    {
                        title: '实收',
                        key: 'RightPrice'
                    },
                    {
                        title: '付款人',
                        key: 'Payer'
                    },
                    {
                        title: '收款人',
                        key: 'Payee'
                    },
                    // {
                    //     title: '批次',
                    //     key: 'Summay'
                    // },
                    {
                         title: '单据',
                        slot: 'action',
                        width: 150,
                        align: 'center'
                    }
                ],
            listtitleout: [
                    {
                        title:"录入时间",
                        slot: 'OriginalDate'
                    },
                    {
                        title: '科目',
                        key: 'Subject'
                    },
                    {
                        title: '币种',
                        key: 'CurrencyName'
                    },
                    {
                        title: '应付',
                        key: 'LeftPrice'
                    },
                    {
                        title: '实付',
                        key: 'RightPrice'
                    },
                    {
                        title: '付款人',
                        key: 'Payer'
                    },
                    {
                        title: '收款人',
                        key: 'Payee'
                    },
                    {
                        title: '批次',
                        key: 'Summay'
                    },
                    {
                         title: '单据',
                        slot: 'action',
                        width: 150,
                        align: 'center'
                    }
                ],
            data2: [
                    {
                        DateCode: '1',    //批次
                        Subject: '停车费', //科目
                        currency: "USA",  //币种
                        Should:"20",      //应收
                        Actual:"10",       //实收
                        payee: '李四',    //收款人 
                        operator:"张山",  //录入人
                        CreateDate: '2016-10-03' //录入时间
                    },
                    {
                        DateCode: '1',
                        Subject: '停车费', //科目
                        currency: "USA",  //币种
                        Should:"20",      //应收
                        Actual:"10",       //实收
                        payee: '李四',    //收款人 
                        operator:"张山",  //录入人
                        CreateDate: '2016-10-03' //录入时间
                    },
                    {
                        DateCode: '1',
                        Subject: '停车费', //科目
                        currency: "USA",  //币种
                        Should:"20",      //应收
                        Actual:"10",       //实收
                        payee: '李四',    //收款人 
                        operator:"张山",  //录入人
                        CreateDate: '2016-10-03' //录入时间
                    },
                    {
                        DateCode: '1',
                        Subject: '停车费', //科目
                        currency: "USA",  //币种
                        Should:"20",      //应收
                        Actual:"10",       //实收
                        payee: '李四',    //收款人 
                        operator:"张山",  //录入人
                        CreateDate: '2016-10-03' //录入时间
                    },
                    {
                        DateCode: '1',
                        Subject: '停车费', //科目
                        currency: "USA",  //币种
                        Should:"20",      //应收
                        Actual:"10" ,      //实收
                        payee: '李四',    //收款人 
                        operator:"张山",  //录入人
                        CreateDate: '2016-10-03' //录入时间
                    },
                    
            ],
            DateCodearr:[ ],
            Files:[],
            detailtypes:"1",
            waybillid:"",//运单号
            yinshou:"",//应收
            shishou:"",//实收
            yingfu:"",//应付
            shifu:"",//实付

            leftprice:"", //应
            rightprice:"", //实
            currency:"",//币种
            subject:"",//费用科目
            formdata:{  //提交信息
                payer:"",
                payerID:"",
                PayeeName:"",//收款人
                PayeeNameID:"",
                DateCode:"",//批次
                Subject:"",//科目
                currency:"",//币种
                Should:"",//应该
                Actual:"",//实际
            },
            fee:"",
            getformdata:"",//获取提交的所有数据
            disabledis:false,//是否禁止选中
            listout:[],//支出列表
            listin:[],//收入列表
        }
       
    },
    created() {
        // console.log("收支明细被加载")
        this.waybillid=this.$route.params.webillID;
        this.otype=this.$route.params.otype;
        console.log(this.otype)
        window ['PhotoUploaded']=this.changed;
        this.getdata_in_from("in")
        this.getdata_in(this.detailtypes)
        this.currency_subject()
        this.getsubject("in")
        this.History()
    },
    mounted() {
        // console.log("收支明细被加载")
        
        // this.getdata_in(this.detailtypes)
        // this.getdata_in_from("in")
        // this.currency_subject()
        // this.getsubject("in")
        // this.History()
    },
    filters:{
        showDate: function(val) {  //时间格式转换
        // console.log(val)
        if (val != "") {
            if (val || "") {
            var b = val.split("(")[1];
            var c = b.split(")")[0];
            var result = moment(+c).format("YYYY-MM-DD");
            return result;
            }
        }
        },
  },
    methods:{
        History(){  //批次
            var data={
                waybillid:this.waybillid
            }
            History(data).then(res=>{
                
                if(res.obj==[]){
                    this.DateCodearr=[]
                }else{
                    this.DateCodearr=res.obj
                }
            })
        },
        currency_subject(){  //币种
            currency().then(res => {
                this.currency=res.obj;
                // console.log(this.currency)
             });
             var codes=""
             if(this.getformdata.EnterCode!=null){
                 codes=this.getformdata.EnterCode
             }else{
                 codes=""
             }
            //  var data={
            //      entercode:codes,
            //  }
            //  subject(data).then(res => {
            //     this.subject=res;
            //     // console.log(this.subject)
            //  });
        },
        getsubject(val){
           var data={
                type:val
            }
            subject(data).then(res => {
                this.subject=res;
                // console.log(this.subject)
             });
        },
        change(value){
            if(value=="1"){
                this.getsubject('in')
                this.placeholderleft="应收";
                this.placeholderright="实收";
                this.formdata.payer=this.getformdata.ClientName; //付款方
                this.formdata.PayeeName=this.getformdata.PayeeName; //收款方
                this.formdata.payerID=this.getformdata.ClientID; //付款方ID
                this.formdata.PayeeNameID=this.getformdata.PayeeID; //收款方ID
            }else{
                this.getsubject('out')
                this.placeholderleft="应付";
                this.placeholderright="实付";
                this.formdata.payer=this.getformdata.ThirdName; //付款方
                this.formdata.PayeeName=this.getformdata.CarrierName; //收款方
                this.formdata.payerID=this.getformdata.ThirdID; //付款方ID
                this.formdata.PayeeNameID=this.getformdata.CarrierID; //收款方ID
            }
        },
        sumbit(){
            if(this.formdata.PayeeNameID==""||this.formdata.PayeeNameID==null){
                    this.$Message.error('收款人为空，不需要添加支出费用');
            } else if(this.formdata.payer!=""&&this.formdata.PayeeName!=""&&this.formdata.Subject!=''&&this.formdata.currency&&this.leftprice!=""){
                var feetype="1"
                if(this.types=='1'){
                    feetype="in"
                }else{
                    feetype="out"
                }
                var fee={
                    // ArrivalBatch:this.formdata.DateCode,//到货批次
                    Currency:this.formdata.currency,     //币种
                    FeeType:feetype,               //类型
                    Files: this.Files,                //文件
                    LeftPrice:this.leftprice,    //应收 应付
                    OrderID: this.getformdata.OrderID,      //订单id
                    Payee:this.formdata.PayeeNameID,        //收款人
                    Payer: this.formdata.payerID,        //付款人
                    RightPrice:this.rightprice,   //实收实付
                    Subject: this.formdata.Subject,    //科目
                    WaybillID:this.getformdata.WaybillID
                }
                // console.log(fee)
                if(this.formdata.PayeeNameID==""||this.formdata.PayeeNameID==null){
                    this.$Message.error('收款人为空，不需要添加支出费用');
                 }else{
                     submitfee(fee).then(res => {
                        if(res.success==true){
                            this.$Message.success('保存成功');
                            this.getdata_in(this.detailtypes);
                            this.formdata.DateCode='';//到货批次
                            this.formdata.currency='';     //币种
                            this.types="1";           //类型
                            this.Files=[];                //文件
                            this.leftprice="";   //应收 应付
                            // this.formdata.PayeeName="";        //收款人
                            // this.formdata.payer="";      //付款人
                            this.rightprice=""; //实收实付
                            this.formdata.Subject="";   //科目
                            this.change("1")
                        }else{
                            this.$Message.error('保存失败，请核实信息是否正确');
                        }
                });
                 }
            }else{
                 this.$Message.error('请输入必填项目');
            }
        },
        detailchange(value){
            // console.log(value)
            this.loading=true;
            this.detailtypes=value
            this.getdata_in(this.detailtypes)
        },
        changeimgs(newdata, row) {  //上传照片
            if(row=="webaill"){
                this.Files.push(newdata)
            }
            // console.log(this.Files)
        },
         getdata_in_from(type){  //获取数据 明细清单
            var newrype=""
            if(type=="1"){
              newrype="in"
            }else{
                newrype="out"
            }
            var data={
                waybillid:this.waybillid,
                type:newrype,
                otype:this.otype
            }
            Paymentslist(data).then(res => {
                // console.log(res)
                this.getformdata=res;
                // console.log(this.getformdata)
                this.formdata.payer=this.getformdata.ClientName; //付款方
                this.formdata.PayeeName=this.getformdata.PayeeName; //收款方
                this.formdata.payerID=this.getformdata.ClientID; //付款方ID
                this.formdata.PayeeNameID=this.getformdata.PayeeID; //收款方ID
                this.fee=res.fee;
                // this.listin=this.getformdata.Payments;
                // this.listout=this.getformdata.Payments;
                // this.loading=false;
             });
        },
        getdata_in(type){  //获取数据 明细清单
            var newrype=""
            if(type=="1"){
              newrype="in"
            }else{
                newrype="out"
            }
            var data={
                waybillid:this.waybillid,
                type:newrype,
                otype:this.otype
            }
            Paymentslist(data).then(res => {
                // console.log(res)
                this.getformdata=res;
                // console.log(this.getformdata)
                // this.formdata.payer=this.getformdata.ClientName; //付款方
                // this.formdata.PayeeName=this.getformdata.PayeeName; //收款方
                // this.formdata.payerID=this.getformdata.ClientID; //付款方ID
                // this.formdata.PayeeNameID=this.getformdata.PayeeID; //收款方ID
                this.fee=res.fee;
                this.listin=this.getformdata.Payments;
                this.listout=this.getformdata.Payments;
                this.loading=false;
             });
        },
     fromphotos(type){  //拍照
        if(type=="Waybill"){
            var data={
                SessionID:this.waybillid,
                AdminID:sessionStorage.getItem("userID"),
            }
            FormPhoto(data)
        }
    },
    
    SeletUpload(type){// 传照
        if(type=="Waybill"){
            var data={
                SessionID:this.waybillid,
                AdminID:sessionStorage.getItem("userID"),
            }
            SeletUploadFile(data)
        }
    },
    handleRemovelist(file) {  //删除照片
      this.Files.splice(this.Files.indexOf(file),1);
    },
    changed(message){   //后台调用winfrom 拍照的方法
      this.testfunction(message)  //前台拿到返回值处理数据
    },
    testfunction(message){  //前台处理数据的方法
      var imgdata=message[0]
      var newfile={
            CustomName:imgdata.FileName,
            ID: imgdata.FileID,
            Url: imgdata.Url,
        };
      if(imgdata.SessionID==this.waybillid){
          this.Files.push(newfile);
      }
     },
     testprice(value,type){  //验证金额
         console.log(type)
        if(value!=''){
            // var testis=/(^([-]?)[1-9]([0-9]+)?(\.[0-9]{1,2})?$)|(^([-]?)(0){1}$)|(^([-]?)[0-9]\.[0-9]([0-9])?$)/; 
           var testis= /^(([1-9][0-9]*)|(([0]\.\d{1,2}|[1-9][0-9]*\.\d{1,2})))$/
            var Result = testis.test(value)
            if(Result==false){
                if(type=='leftprice'){
                    this.leftprice='';
                }else{
                    this.rightprice="";
                }
                this.$Message.error('请输入正确的金额，金额保留到小数点后两位');
            }
        }
     },
     change_subject(value){
        var newarr = []
        newarr=this.subject.filter(item => item.Name==value&&item.PayQuote.Price!=null)
        var obj=newarr[0]
        if(newarr.length>0) {
                this.disabledis=true;
                // this.rightprice=obj.PayQuote.Price;   //实收实付
                this.rightprice="";   //实收实付
                this.leftprice=obj.PayQuote.Price;
                this.formdata.currency=obj.PayQuote.Currency;
        }else{
                this.disabledis=false;
                this.rightprice="";   //实收实付
                this.leftprice="";
                this.formdata.currency="";
            }
    },
     PictureShow(url){  //图片展示
      var data={
        Url:url
      }
      PictureShow(data)
    },
}
}

// var obj={
//     orderinfo:{

//     },

//     Costlis:[
//         {
//             Subject: '停车费', //科目
//             currency: "USA",  //币种
//             Should:"20",      //应收
//             Actual:"10",       //实收
//             payee: '李四',    //收款人 
//             operator:"张山",  //录入人
//             CreateDate: '2016-10-03' //录入时间
//         }
//     ]
// }
</script>