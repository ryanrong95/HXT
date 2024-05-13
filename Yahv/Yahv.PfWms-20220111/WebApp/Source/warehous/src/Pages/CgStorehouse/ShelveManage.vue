<style scoped>
.titlestyle{
  text-indent: 10px;
  border-left: 4px solid #2d8cf0;
  font-size: 15px;
  line-height: 32px;
  margin-right: 20px;
  float: left;  
}
.tablebox{
    padding-top: 20px;
}
.inputwidth{
    width: 80%;
}
.title_lable{
  display: inline-block;
  min-width: 14%;
  text-align: right
}
.inputbox{
  margin-bottom: 15px;
}
.bitian{
    color: red;
    font-size: 20px;
    vertical-align: -webkit-baseline-middle;
    padding-right: 3px;
}
.pagebox{
  text-align: right;
  padding-top: 15px;
}
</style>
<template>
  <div>
    <div>
        <h1 class="titlestyle">当前库房编号：HK01</h1>
        <Button type="primary" @click="openmodel(1)" icon='md-add'>添加库位</Button>
        <Button type="primary" @click="PrintShelve(0)" icon="ios-print-outline">批量打印</Button>
    </div>
    <div class="tablebox">
        <Table :columns="columns1" :data="PalletArr" @on-selection-change='changeselect' ref="selection">
          <template slot-scope="{ row, index }" slot="Code">
             <span>{{row.PlaceIDs}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="actions">
            <Button type="primary" size='small' @click="PrintShelve(1,row)" icon="ios-print-outline">打印</Button>
            <Button type="error" size='small' @click="confirmDel(row.PlaceIDs)"icon='ios-trash-outline'>删除</Button>
          </template>
        </Table>
         <div class="pagebox">
         <Page :total="total" :page-size='pageSize' :current='pageIndex' @on-change='changepage'/>
      </div>
    </div>
    <Modal
        v-model="showmodel"
        :title="titlemodel"
        @on-visible-change='changeshowmodel'>
        <div class="inputbox">
            <label for="" class="title_lable"><span class="bitian">*</span>库区：</label>
            <Select v-model="fromdata.area" filterable class="inputwidth">
                <Option v-for="(i,index) in 26" :value="String.fromCharCode(65+index)" :key="index">{{ String.fromCharCode(65+index) }}</Option>
            </Select>
        </div>
        <div class="inputbox">
            <label for="" class="title_lable"><span class="bitian">*</span>货架号：</label>
            <Select v-model="fromdata.frame" filterable class="inputwidth">
                 <Option v-for="(item,index) in 100" :value="index" :key="index">{{ index }}</Option>
            </Select>
        </div>
        <div class="inputbox">
            <label for="" class="title_lable"><span class="bitian">*</span>层号：</label>
            <Select v-model="fromdata.row" filterable class="inputwidth">
                <Option v-for="(item,index) in Numberarr" :value="item" :key="index">{{ item }}</Option>
            </Select>
        </div>
        <div class="inputbox">
            <label for="" class="title_lable"><span class="bitian">*</span>库位号：</label>
            <Select v-model="fromdata.place" filterable class="inputwidth">
                 <Option v-for="(item,index) in 100" :value="index" :key="index">{{ index }}</Option>
            </Select>
        </div>
         <div slot="footer">
              <Button @click="cancel_btn">取消</Button>
              <Button type="primary" @click="ok_btn">确定</Button>
        </div>
    </Modal>
  </div>
</template>
<script>
import {CgShowPallets,CgSetPlace,CgDelete } from '../../api/CgApi'
import { GetPrinterDictionary, TemplatePrint } from "@/js/browser.js"
let product_url=require("../../../static/pubilc.dev")
export default {
  data() {
    return {
      SelectRow:[],
      columns1: [
        {
          type: 'selection',
          width: 60,
          align: 'center'
        },
         {
          type: 'index',
          width: 100,
          align: 'center'
        },
        {
          title: "编号",
          slot: "Code"
        },
        {
          title: "操作",
          slot: "actions",
          align: "center"
        }
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03"
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01"
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02"
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04"
        }
      ],
      titlemodel:"",//弹出框title
      showmodel:false,
      Numberarr:100,
      fromdata:{
        area:'',
        frame:'',
        row:'',
        place:'',
      },
      Waybill:"",
      PalletArr:[],
      loading:true,
      total:0,
      pageIndex:1,
      pageSize:10,
      printurl:product_url.pfwms,
    };
  },
  created() {
    this.Waybill=sessionStorage.getItem("UserWareHouse")
  },
  mounted() {
    this.setnva()
    this.CgShowPallets()
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "库位管理",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    CgShowPallets(){
      var data={
        whid:this.Waybill,
        pageIndex:this.pageIndex,
        pageSize:this.pageSize,
      }
      CgShowPallets(data).then(res=>{
        this.PalletArr=res.obj.Data
        this.total=res.obj.Total;
        this.loading=false
      })
    },
    changepage(val){
      this.pageIndex=val;
      this.loading=true;
      this.CgShowPallets()
    },
    ok_btn(){
      console.log(this.fromdata)
      if(this.fromdata.frame==''&&this.fromdata.area==''&&this.fromdata.row==''&&this.fromdata.place==""){
          this.$Message.error('请输入必填项');
      }else{
        var frame=(Array(2).join('0') + this.fromdata.frame).slice(-2)
        var place=(Array(2).join('0') + this.fromdata.place).slice(-2)
        var data='0'+this.fromdata.area+frame+this.fromdata.row+place;
        var obj={
          whCode:this.Waybill,//库房编号，
          place:data
        }
        CgSetPlace(obj).then(res=>{
          if(res.success==true){
            this.$Message.success('添加成功');
            this.showmodel=false;
            this.CgShowPallets()
            this.fromdata.area=''
            this.fromdata.frame=''
            this.fromdata.row=''
            this.fromdata.place=''
          }else if(res.code){
             this.$Message.error('已存在');
          }
        })
      }
       
    },
    cancel_btn(){
        this.showmodel=false;
    },
    changeshowmodel(val){
      if(val==false){
        this.fromdata.area=''
        this.fromdata.frame=''
        this.fromdata.row=''
        this.fromdata.place=''
      }
    },
    openmodel(val){
        console.log(val)
        if(val==1){
            this.titlemodel='添加库位'
        }else{
            this.titlemodel='修改库位'
        }
        this.showmodel=true;
    },
    confirmDel(id){
      var id=id;
        this.$Modal.confirm({
            title: '删除',
            content: '<p>是否确认删除此库位?</p>',
            onOk: () => {
              var data={
                id:id
              }
                CgDelete(data).then(res=>{
                  if(res.success==true){
                     this.$Message.success('删除成功');
                     this.CgShowPallets()
                  }else{
                     this.$Message.error('删除失败,请联系管理员');
                  }
                })
            },
            onCancel: () => {
                this.$Message.info('取消删除');
            }
        });
    },
    //多选发生变化的时候，返回选中后的数据
    changeselect(value){
      console.log(value)
      this.SelectRow = value;
    },
    //打印标签
    PrintShelve(type,item){
       if(type==0){
         if(this.SelectRow.length<=0){
          this.$Message.warning('请选择需要打印标签的库位');
         }else{
           console.log(this.SelectRow)
          this.custom()
         }
       }else{
         this.SelectRow=[]
         this.SelectRow.push(item)
         console.log( this.SelectRow)
         this.custom()
       }
    },
    //库位标签打印 确认弹出框
    custom () {
        this.$Modal.confirm({
          title: '是否打印库位标签',
          content: '<p></p>',
          okText: '确定',
          cancelText: '取消',
          onOk: () => {
              this.printOk()
          },
          onCancel: () => {
              this.$Message.info('取消打印');
          }
       }); 
    },
   //确认打印事件
    printOk() { 
        var printarr=[]
        for(var i=0,len=this.SelectRow.length;i<len;i++){
            var item={
              ID: this.SelectRow[i].PlaceIDs, //编号
              Name: this.SelectRow[i].PositionID,//位号
              EnterpriseName:"",
              FatherMsg: {
                  RegionName: this.SelectRow[i].RegionID, //库区
                  ShleveName: this.SelectRow[i].ShelveID,// 货架
                  WarehouseID: sessionStorage.getItem("UserWareHouse"),//库房编号
                  WarehouseName: sessionStorage.getItem("WareHouseName"),//库房名称
              },
              LeaseName:this.SelectRow[i].LeaseID
            }
            printarr.push(item)
        }
        if(printarr.length>0){
            var config = GetPrinterDictionary()
            var getsetting = config['库位标签'];
            var str=getsetting.Url
            var testurl=str.indexOf("http") != -1
              if(testurl==true){
                getsetting.Url=getsetting.Url
              }else{
                getsetting.Url=this.printurl+getsetting.Url
            }
            var data = {
              setting: getsetting,
              data: printarr
            };
            TemplatePrint(data);
        }
      },
  }
};
</script>