<style scoped>
.detailtitle {
    line-height: 24px;
    border-left: 5px solid #2d8cf0;
    font-weight: bold;
    font-size: 16px;
    text-indent: 10px;
}
.box{
    width: 100%;
    min-height: 120px;
    background: rgb(245, 247, 249);
    margin: 15px 0px;
}
.buttons{
    width:100%;
    margin-top: 20px;
    text-align: center;
}
.setupload {
  /* width: 50px; */
  height: 30px;
  border: none;
  float: left;
  line-height: 1;
  margin-right: 5px;
}
.iconstyle:hover{
    cursor: pointer;
}
</style>
<template>
    <div>
        <div>
            <p class="detailtitle">基础信息</p>
            <div class="box">
                <Row style="padding:10px;">
                    <Col :xs="2" :sm="4" :md="6" :lg="6">
                        <p>
                            <label >运&nbsp;&nbsp;单&nbsp;&nbsp;号：
                                <Input v-model="Code" placeholder="请输入运单号" style="width: 200px" />
                            </label>
                        </p>
                        <p style="padding-top:10px">
                          <label >入&nbsp;&nbsp;仓&nbsp;&nbsp;号：
                            <Input v-model="EnterCode" placeholder="请输入入仓号" style="width: 200px" />
                          </label>  
                        </p>
                        <p style="width: 172%;padding-top:10px">
                            <label >备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：
                              <Input style="width:90%;float:right"  type="textarea" v-model="Summary":autosize="{maxRows: 3,minRows: 3}"  placeholder="请输入备注信息" />
                           </label>
                        </p>
                    </Col>
                    <Col :xs="20" :sm="16" :md="12" :lg="6">
                    <p>
                        <label >承&nbsp;&nbsp;&nbsp;运&nbsp;&nbsp;&nbsp;&nbsp;商：
                            <Select v-model="CarrierID"  style="width:200px">
                                <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
                            </Select>
                        </label>
                    </p>
                    <p style="padding-top:10px">
                        <label >发货人姓名：
                            <Input v-model="ConsignorName" placeholder="请输入发货人姓名" style="width: 200px" />
                        </label>
                    </p>
                   
                    </Col>
                    <Col :xs="2" :sm="4" :md="6" :lg="6">
                       <p >
                        <label >输&nbsp;&nbsp;&nbsp;送&nbsp;&nbsp;&nbsp;&nbsp;地：
                                <Select v-model="Place"  style="width:200px" >
                                    <Option v-for="item in Conveyingplace" :value="item.CorPlace" :key="item.ID">{{item.Text}}</Option>
                                </Select>
                            </label> 
                       </p>
                       <p style="padding-top:10px">
                        <label >发货人电话：
                             <Input v-model="phonenumber" @on-blur="setphone" type="tel" placeholder="请输入发货人电话" style="width: 200px" />                               
                        </label>
                       </p> 
                       <!-- <p style="padding-top:10px">
                          <label >暂&nbsp;存&nbsp;库&nbsp;位：
                            <Input v-model="ShelveID" placeholder="请输入库位" style="width: 200px" />
                          </label>
                       </p>  -->
                       <p style="padding-top:10px">
                          <label ><em class="bitian">*</em>暂&nbsp;存&nbsp;库&nbsp;位：
                            <!-- <Input v-model="ShelveID" placeholder="请输入库位" style="width: 200px" /> -->
                            <Select v-model="ShelveID"  style="width:200px">
                                    <Option v-for="item in ShelveArr" :value="item.ID" :key="item.ID">{{item.ID}}</Option>
                            </Select>
                             <!-- <Select v-model="CarrierID"  style="width:200px">
                                <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
                             </Select> -->
                          </label>
                       </p> 
                    </Col>
                    <Col :xs="2" :sm="4" :md="6" :lg="6">
                        <div class="setupload">
                            <!-- <img-test type="webaill" v-on:changitem="changeimgs($event,'webaill')"></img-test> -->
                            <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUpload('Waybill')">传照</Button>
                        </div>
                        <div class="setupload" style="float:rigth">
                            <Button size="small" type="primary" icon="md-reverse-camera" @click="fromphotos()">拍照</Button>
                        </div>
                        <ul style="clear:both;">
                            <li v-for="item in Files" v-if="item.Type==8000">
                                <p>
                                   <span @click="PictureShow(item.Url)">{{item.CustomName}}</span> 
                                   <Icon class="iconstyle" type="md-trash" @click="delimg(item)" />
                                </p>
                            </li>
                        </ul>
                    </Col>
                </Row>
            </div>
        </div>
        <div>
            <div style="padding-top:10px;">
                <p class="detailtitle">描述信息</p>
                <div style="padding-top:10px;">
                    <Table :columns="Summaryarr" :data="Summarydata">
                        <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
                        <template slot-scope="{ row, index }" slot="action">
                                <Icon @click="addSummary(index)" type="ios-add-circle-outline" style="margin-right:5px;font-size:20px;font-width:600" />
                                <Icon @click="delrow(index,'Summary')" type="ios-trash-outline" style="font-size:20px;font-width:600" />
                        </template>
                    </Table>
                </div>
            </div>
            <div style="padding-top:30px;">
                <p class="detailtitle">产品信息</p>
                <div style="padding-top:10px;">
                    <Table :columns="columns7" :data="Storages">
                        <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
                        <template slot-scope="{ row, index }" slot="action">
                                <Icon @click="addrow(index)" type="ios-add-circle-outline" style="margin-right:5px;font-size:20px;font-width:600" />
                                <Icon @click="delrow(index,'product')" type="ios-trash-outline" style="font-size:20px;font-width:600" />
                        </template>
                        <template slot-scope="{ row, index }" slot="Place">
                                <Select v-model="row.Place"  style="width:200px" :transfer="true" filterable>
                                        <Option v-for="item in Conveyingplace" :value="item.CorPlace" :key="item.ID">{{item.Text}}</Option>
                                </Select>
                        </template>
                    </Table>
                    <div class="buttons">
                        <Button size="large" @click="clear_btn">取消修改</Button>
                        <Button type="primary" size="large" style="margin-rigth:5px" @click="submitbtn">保存</Button>
                    </div>
                </div>
             </div>
        </div>
    </div>
</template>
<script>
import {getWayParter,Carriers,SparateDetails,TempStorage,GetUsableShelves} from "../../api";
import {CgDeleteFiles} from "../../api/CgApi";
import {FormPhoto,SeletUploadFile} from "@/js/browser.js"
import imgtest from "@/Pages/Common/imgtes";
export default {
    components: {
    "img-test": imgtest,
  },
    data() {
        return {
            waybillid:"",
            Consignorid:"",
            myplace:"",//本地输送地
            Conveyingplace:[],//输送地列表
            Carrier:"",//承运商
            CarrierList:[],//承运商列表
            WareHouseID:sessionStorage.getItem('UserWareHouse'),//库房id

            ShelveID:"",//暂存库位
            ShelveArr:[],//暂存数组
            Code:"",//运单号
            WaybillType:"",//运单类型,
            CarrierID:"",//承运商ID,
            ConsignorID:"",//发货人,
            ExcuteStatus:101,//执行状态,
            Place:"",//原产地
            phonenumber:"",//发件人电话
            ConsignorName:"",//发货人姓名：
            EnterCode:"",// 入仓号
            Summary:"",  //备注
            Files:[],//文件

            value:"",
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
            columns7: [
                    {
                        title:"",
                        slot:"indexs",
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: '操作',
                        slot: 'action',
                        width: 100,
                        align: 'center'
                    },
                    {
                        title: '型号',
                        key: 'PartNumber',
                        render: (h, params) => {
                        var vm = this;
                        return h("Input", {
                            props: {
                            //将单元格的值给Input
                            value: params.row.Product.PartNumber,
                            },
                            on: {
                            "on-change"(event) {
                                    //值改变时
                                    //将渲染后的值重新赋值给单元格值
                                    params.row.Product.PartNumber = event.target.value;
                                    vm.Storages[params.index] = params.row;
                                }
                            }
                        });
                        }
                    },
                    {
                        title: '品牌',
                        key: 'Manufacturer',
                        render: (h, params) => {
                        var vm = this;
                        return h("Input", {
                            props: {
                            //将单元格的值给Input
                            value: params.row.Product.Manufacturer,
                            },
                            on: {
                            "on-change"(event) {
                                    //值改变时
                                    //将渲染后的值重新赋值给单元格值
                                    params.row.Product.Manufacturer = event.target.value;
                                    vm.Storages[params.index] = params.row;
                                }
                            }
                        });
                        }
                    },
                    {
                        title: '批号',
                        key: 'DateCode',
                        render: (h, params) => {
                        var vm = this;
                        return h("Input", {
                            props: {
                            //将单元格的值给Input
                            value: params.row.DateCode,
                            },
                            on: {
                                "on-change"(event) {
                                    //值改变时
                                    //将渲染后的值重新赋值给单元格值
                                    params.row.DateCode = event.target.value;
                                    vm.Storages[params.index] = params.row;
                                }
                            }
                        });
                        }
                    },
                    {
                        title: '数量',
                        key: 'Quantity',
                        render: (h, params) => {
                        var vm = this;
                        return h("Input", {
                            props: {
                            //将单元格的值给Input
                            value: params.row.Quantity,
                            },
                            on: {
                                "on-change"(event) {
                                    //值改变时
                                    //将渲染后的值重新赋值给单元格值
                                    params.row.Quantity = event.target.value;
                                    vm.Storages[params.index] = params.row;
                                },
                                "on-blur"() {
                                    // var reg = /^[0-9]*$/;
                                    if(event.target.value!=''){
                                        var reg= /^\d+(\.\d{0,2})?$/;
                                        if (reg.test(event.target.value) == true) {
                                            params.row.Quantity = event.target.value;
                                        vm.Storages[params.index] = params.row;
                                        } else {
                                            vm.$Message.error("只能输入数量");
                                            (event.target.value = ""),
                                            (params.row.Quantity = "");
                                            vm.Storages[params.index] = params.row;
                                        }
                                    }
                                    
                                 }
                            }
                        });
                        }
                    },
                    {
                        title: '原产地',
                        slot: 'Place',
                    },
                ],
            Storages: [
                    {
                        WareHouseID:sessionStorage.getItem('UserWareHouse'),
                        Quantity:"",//数量,
                        ShelveID:"",//库位编号,
                        DateCode:"",//批次号,
                        Place:"", //产品原产地
                        Product:{
                          PartNumber:"",//型号
                          Manufacturer:"",//制造商，品牌 
                        }
                    },
              ],
            Summaryarr:[
                {
                        title:"",
                        slot:"indexs",
                        width: 30,
                        align: 'center'
                },
                {
                    title: '操作',
                    slot: 'action',
                    width: 100,
                    align: 'center'
                },
                {
                        title: '描述',
                        key: 'Summary',
                        render: (h, params) => {
                        var vm = this;
                        return h("Input", {
                            props: {
                            //将单元格的值给Input
                            value: params.row.Summary
                            },
                            on: {
                            "on-change"(event) {
                                    //值改变时
                                    //将渲染后的值重新赋值给单元格值
                                    params.row.Summary = event.target.value;
                                    vm.Summarydata[params.index] = params.row;
                                }
                            }
                        });
                        }
                    },
            ],
            Summarydata:[
                {
                   WareHouseID:sessionStorage.getItem('UserWareHouse'),//库房编号,
                   Quantity:0,//
                   ShelveID:"",//库位编号,
                   Summary:"",//简介
                }
            ],
            objs:{}
        }
    },
    created() {
        console.log("暂存页加载"),
        window ['PhotoUploaded']=this.changed;
        this.getWayParter();
        this.Carriers()
        this.GetUsableShelves()
        console.log(this.WareHouseID)
    },
    mounted() {
        
    },
    methods: {
        Initialization(ID){  //页面加载。初始化数据
          var data={
              warehouseID:this.WareHouseID,//库房编号
              waybillID:ID,//运单编号
          };
         SparateDetails(data).then((res) => {
            console.log(res)
            this.objs=res.obj;
            //this.ShelveID=res.obj.//  ShelveID:"",//暂存库位
            this.Code=res.obj.Code// Code:"",//运单号
            this.CarrierID=res.obj.CarrierID// CarrierID:"",//承运商ID,
            this.ConsignorID=res.obj.Consignor.Contact// ConsignorID:"",//发货人,联系人
            this.Place=res.obj.Consignor.Place// Place:"",//原产地
            this.phonenumber=res.obj.Consignor.Phone// phonenumber:"",//发件人电话
            this.ConsignorName=res.obj.Consignor.Contact// ConsignorName:"",//发货人姓名：
            this.EnterCode=res.obj.EnterCode// EnterCode:"",// 入仓号
            this.Summary=res.obj.Summary// Summary:"",  //备注
            this.Files=res.obj.Files// Files:[],//文件
            this.waybillid=res.obj.WaybillID;
            this.Consignorid=res.obj.ConsignorID;
            // alert(JSON.stringify(this.Files))
            if(res.obj.ProductStorages.length<=0){
                this.Storages=[
                    {
                        WareHouseID:sessionStorage.getItem('UserWareHouse'),
                        Quantity:"",//数量,
                        ShelveID:"",//库位编号,
                        DateCode:"",//批次号,
                        Place:"", //产品原产地
                        Product:{
                          PartNumber:"",//型号
                          Manufacturer:"",//制造商，品牌 
                        }
                    },
                ]
            }else{
                 this.Storages=res.obj.ProductStorages;  //产品信息
            }

            if(res.obj.SummaryStorages.length<=0){
                this.Summarydata=[
                    {
                        WareHouseID:sessionStorage.getItem('UserWareHouse'),//库房编号,
                        Quantity:0,//
                        ShelveID:"",//库位编号,
                        Summary:"",//简介 
                    }
                ]
            }else{
                 this.Summarydata=res.obj.SummaryStorages;  //描述信息
            }
           
           
            if(res.obj.SummaryStorages.length>0){
                this.ShelveID=this.Summarydata[0].ShelveID
            }else{
                this.ShelveID=this.Storages[0].ShelveID
            }
          
           })
        },
        addrow(index){    //添加一行空数据
            console.log(index)
            var newrow={
                        WareHouseID:sessionStorage.getItem('UserWareHouse'),
                        Quantity:"",//数量,
                        ShelveID:"",//库位编号,
                        DateCode:"",//批次号,
                        Place:"", //产品原产地
                        Product:{
                          PartNumber:"",//型号
                          Manufacturer:"",//制造商，品牌 
                     }
            };
        this.Storages.splice(index + 1, 0, newrow);
        },
        addSummary(index){
            var newSummary={
                   WareHouseID:sessionStorage.getItem('UserWareHouse'),//库房编号,
                   Quantity:0,//
                   ShelveID:"",//库位编号,
                   Summary:"",//简介
            };
         this.Summarydata.splice(index + 1, 0, newSummary);
        },
        delrow(index,type){  //删除所选数据
            if(type=="Summary"){
                this.Summarydata.splice(index, 1);
            }else{
                this.Storages.splice(index, 1);
            }
            
        },
        getWayParter(){ //获取输送地列表
            getWayParter().then(res => {
                console.log(res)
                this.Conveyingplace=res.obj;
            });
        },
        Carriers(){ //获取承运商列表
            Carriers().then(res => {
                this.CarrierList=res.obj;
                });
        },
        GetUsableShelves(){  //获取暂存库位数据
            var id=sessionStorage.getItem('UserWareHouse');
            console.log(id)
            GetUsableShelves(id).then(res=>{
                console.log(res)
                this.ShelveArr=res.obj;
            })
        },
        submitbtn(){
            var Summarytrue="";  //备注
            var Storagestrue=""; //产品
            var NewSummary=[];//备注
            var NewStorages=[];//产品
        if(this.Summarydata.length==1&&this.Summarydata[0].Summary==""){
             Summarytrue=false
        }else{
             Summarytrue=true;
             if(this.Summarydata.length>=1){
                 for(var i=0;i<this.Summarydata.length;i++){
                    if(this.Summarydata[i].Summary!=""){
                        NewSummary.push(this.Summarydata[i])
                    }
                 }
             }
        }
        if(this.Storages.length==1&&this.Storages[0].Product.PartNumber==""){
             Storagestrue=false;
        }else{
            Storagestrue=true;
            for(var i=0;i<this.Storages.length;i++){
                    if(this.Storages[i].Product.PartNumber!=""){
                        NewStorages.push(this.Storages[i])
                    }
                }
        }
        if(Summarytrue==true||Storagestrue==true){
            // alert("正确")
        }else{
             this.$Message.error('请输入产品信息或描述信息');
        }
 
            if(this.ShelveID==""){
                 this.$Message.error('请输暂存库位');
            }else{
            var magtrue="";//库位必填
            if(Summarytrue==true||Storagestrue==true){
             NewStorages.map((item) => {
               item.ShelveID=this.ShelveID;
               if(item.Quantity==""||item.Place==""){
                   magtrue=true;
               }else{
                   magtrue=false;
               }
            });
            NewSummary.map((item) => {
               item.ShelveID=this.ShelveID;
            })
            
            if(magtrue==true){
                    this.$Message.error('数量与产地为必填');
            }else{
                 var objs={
                    WaybillID:this.waybillid, 
                    Code:this.Code,//运单号
                    WaybillType:3,//运单类型, 指快递
                    CarrierID:this.CarrierID,//承运商ID, 
                    ExcuteStatus:300,//执行状态, 无101 暂存状态
                    Files:this.Files,
                    EnterCode:this.EnterCode,//入仓号
                    Summary:this.Summary,//简介（运单）,
                    SummaryStorages:NewSummary,
                    ConsignorID:this.Consignorid,
                    Consignor:{
                        Contact:this.ConsignorName,//联系人姓名,
                        Phone:this.phonenumber,//联系人电话,
                        Place:this.Place,//原产地
                    },
                    ProductStorages:NewStorages,//产品列表
                }
                
                console.log(objs)
                var data={
                    obj:JSON.stringify(objs)
                }

               TempStorage(data).then(res => {
                console.log(res)
                if(res.val==0){
                     this.$Message.success("修改成功,两秒后关闭该页面");
                    var that=this;
                    setTimeout(function(){
                      that.$emit('uploadlist');                        
                    },2000)
                }else{
                    this.$Message.error('修改失败，请检查录入信息是否正确');
                }
             });

            }
        }
        
        }
        },  
        changeimgs(newdata, row) {  //上传照片
            if(row=="webaill"){
                this.Files.push(newdata)
            }
        },
        delimg(file){
              var data={
                 id:file.ID
               }
            CgDeleteFiles(data).then(res=>{
                if(res.Success==true){
                    this.Files.splice(this.Files.indexOf(file),1);
                    this.$Message.success('删除成功')
                }else{
                    this.$Message.error('删除失败')
                }
            })
        },
      fromphotos(){ //拍照
            var data={
                SessionID:"zancun",
                AdminID:sessionStorage.getItem("userID")
            }
            FormPhoto(data)
      
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
    changed(message){   //后台调用winfrom 拍照的方法
      this.testfunction(message)  //前台拿到返回值处理数据
    },
    testfunction(message){  //前台处理数据的方法
      var imgdata=message[0]
      var newfile={
                      CustomName:imgdata.FileName,
                      ID: imgdata.FileID,
                      Url: imgdata.Url,
                      type:8000,
                };
          this.Files.push(newfile);
    },
    setphone(){
        if(this.phonenumber!=''){
            var mPattern = /(^(13[0-9]|14[5|7]|15[0|1|2|3|5|6|7|8|9]|18[0|1|2|3|5|6|7|8|9])\d{8}$)/;
            var phones = mPattern.test(this.phonenumber)
            console.log(phones)
            this.disabledphone=phones
            if(phones==false){
                this.$Message.error('请输入正确的手机号');
                this.phonenumber=""
            }
        }
    },
    clear_btn(){
        this.Initialization(this.waybillid)
    },
    PictureShow(url) {
      //图片展示
      var data = {
        Url: url
      };
      PictureShow(data);
    }
    },
}
</script>
