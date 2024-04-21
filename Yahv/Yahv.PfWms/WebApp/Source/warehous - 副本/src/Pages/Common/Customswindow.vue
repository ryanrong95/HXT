<style scoped>
#Customsbox .Customswindow .title {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
  margin-bottom: 10px;
}
#Customsbox .Apply_btn {
  background: rgb(255, 153, 0);
  width: 100px;
  height: 40px;
  line-height: 40px;
  float: right;
  text-align: center;
  margin-right: 50px;
  border-radius: 5px;
}
#Customsbox .Apply_btn .ivu-btn-text:hover {
  color: #ffffff;
}
#Customsbox >>> .ivu-table-cell {
    padding-left: 5px;
    padding-right: 5px;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: normal;
    word-break: break-all;
    box-sizing: border-box;
}
#Customsbox .windowtitle{
    line-height: 24px;
    border-left: 5px solid #2d8cf0;
    font-weight: bold;
    font-size: 16px;
    text-indent: 10px;
    margin-bottom: 20px;
}
#Customsbox .itemPartNumber{
  display: inline-block;
  /* width: 80px; */
  /* overflow: hidden; */
}
.arritem{
  line-height: 35px;
}
.settag1{
  display: inline-block;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #ac0;
}
.settag2{
  display: inline-block;
  width: 10px;
  height: 10px;
  border-radius: 50%;
  background: #f90
}
.PartNumbername{
  /* display: inline-block; */
  width: 70%;
  height: 35px;
  float: left;
  overflow: hidden;
}
.PartNumTag{
  float: right;
  width:26%;
}
</style>
<template>
  <div class="Customswindow" id="Customsbox">
    <p  class="windowtitle title">装箱清单</p>
    <div>
      <!-- <Input
        v-model.trim="key"
        search
        enter-button="筛选"
        placeholder="输入箱号/型号/品牌"
        @on-search="search_btn"
        style="width:20%;float:left;margin-right:5px"
      /> -->
      <ButtonGroup style="width:28%">
            <Input
              v-model.trim="key"
              placeholder="输入箱号/型号/品牌"
              clearable
              @on-clear="clear_search"
              style="width:80%;float:left;position: relative;left: 3px"
            />
            <!-- <Button style="float:left" @click="search_pro" type="primary">筛选</Button> -->
            <Button style="float:left" type="primary" @click="search_btn">筛选</Button>
          </ButtonGroup>
      <!-- <Button type="primary">报关单打印</Button> -->
      <Button type="primary" @click="printboxcode">箱签打印</Button>
      <Button type="primary" @click="Warehousing_btn">一键入库</Button>
      <div class="Apply_btn">
        <Button type="text" icon="ios-create-outline" ghost @click="AskCustoms">报关申请</Button>
      </div>
    </div>
    <div style="margin:10px 0;clear: both;">
      <Checkbox v-model="Isstatus" @on-change="changestatus">显示已报关</Checkbox>
      <Checkbox v-model="Isall" @on-change="changeall">显示所有人</Checkbox>
    </div>
    <div>
      <Table 
        :loading="loading"
        :columns="detail_title" 
        :data="detail_list" 
        @on-selection-change="selection_chenge"
        max-height="700"
        ref="selection" >
        <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
        <template slot-scope="{ row, index }" slot="boxnumber">{{row.Code}}</template>
        <template slot-scope="{ row, index }" slot="state">
          <p>
            <span>{{row.StatusDescription}}</span>
            <span  v-if="row.Status==500" class="settag1"></span>
            <span v-else class="settag2"></span>
            </p>
          <!-- <p>{{row.StatusDescription}}</p> -->
        </template>
        <template slot-scope="{ row, index }" slot="specs">
          <Select v-model="row.BoxingSpecs" transfer :disabled="row.Status==500?true:false">
            <Option v-for="item in boxSpecsarr" :value="item.value" :key="item.value">{{ item.name }}</Option>
          </Select>
        </template>
        <template slot-scope="{ row, index }" slot="gross">
          <div class="arritem" v-for="item in row.Notices">
             <!-- <span class="itemPartNumber">{{item.Weight}}</span> -->
             <Input v-model="item.Weight" placeholder="请输入毛重" :disabled="row.Status==500?true:false"/>
          </div>
        </template>
        <template slot-scope="{ row, index }" slot="ClientID">
          <div class="arritem" v-for="item in row.Notices">
             <span class="itemPartNumber">{{item.Input.ClientName}}</span>
          </div>
        </template>
        <template slot-scope="{ row, index }" slot="PartNumber">
          <div class="arritem" v-for="item in row.Notices" v-if="item.Product!=null">
             <Tooltip :content="item.Product.PartNumber" placement="top" class="PartNumbername">
                  <span class="itemPartNumber" >{{item.Product.PartNumber}}</span>
             </Tooltip>
             <Tag checkable color="primary" class="PartNumTag" v-if="item.Conditions.IsCIQ==true">商检</Tag>
             <Tag checkable color="warning" class="PartNumTag" v-if="item.Conditions.IsCCC==true">CCC</Tag>
             <Tag checkable color="warning" class="PartNumTag" v-if="item.Conditions.error==true">禁运</Tag>
             <Tag checkable color="warning" class="PartNumTag" v-if="item.Conditions.magenta==true">高价值</Tag>
          </div>
          <div v-else style="height:35px">

          </div>
        </template>
        <template slot-scope="{ row, index }" slot="Manufacturer" >
         <div class="arritem" v-for="item in row.Notices" v-if="item.Product!=null">
             <span class="itemPartNumber">{{item.Product.PartNumber}}</span>
          </div>
        </template>
        <template slot-scope="{ row, index }" slot="Datacode">
           <div class="arritem" v-for="item in row.Notices" v-if="item.Product!=null">
             <span class="itemPartNumber">{{item.Input.DateCode}}</span>
           </div>
        </template>
        <template slot-scope="{ row, index }" slot="Quantity">
          <div class="arritem" v-for="item in row.Notices" v-if="item.Product!=null">
             <span class="itemPartNumber">{{item.Quantity}}</span>
           </div>
        </template>
        <template slot-scope="{ row, index }" slot="place">
          <div class="arritem" v-for="item in row.Notices" v-if="item.Product!=null">
             <span class="itemPartNumber">{{item.OriginDescription}}</span>
           </div>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <div v-for="item in row.Notices" class="arritem">
             <Button type="primary" size="small" @click="chengbox(item.BoxCode)" :disabled="row.Status==500?true:false">变更箱号</Button>
          </div>
        </template>
      </Table>
    </div>
     <!-- 申报窗口确认 开始 -->
        <Modal
          v-model="isdeclare"
          title="提示"
          @on-ok="ok_declare"
          @on-cancel="cancel_declare">
          <p>您是否确认申请报关？</p>
      </Modal>
      <!-- 申报窗口确认 结束-->
      <!-- 一键入库 开始 -->
        <Modal
          v-model="isWarehousing"
          title="请选择库位"
          @on-ok="ok_Shelve"
          @on-cancel="cancel_Shelve"
          @on-visible-change=Shelvestate>
          <div>
            <Select v-model="ShelveID"  filterable>
                    <Option v-for="item in ShelveArr" :value="item.ID" :key="item.ID">{{item.ID}}</Option>
            </Select>
          </div>
      </Modal>
      <!-- 一键入库 结束-->
      <!-- 变更箱号 开始 -->
      <Modal
        v-model="Isshowbox"
        title="选择箱号"
        @on-ok="ok_changebox"
        @on-cancel="cancel">
        <!-- <Button type="text"  icon="md-add" style="margin-bottom: 10px;">申请箱号</Button><br/> -->
        <Select v-model="newboxcode">
          <Option v-for="item in boxingarr" :value="item.Code" :key="item.ID">{{ item.Code }}</Option>
        </Select>
        
    </Modal>
   <!-- 变更箱号 结束-->
  </div>
</template>
<script >
//  库位，规格 是可选择的？（建议下拉选择可输入）
import Vue from "Vue";
import {GetUsableShelves,boxproducts,BoxingSpecs,GetBoxes,CustomsApply,ChangeBoxCode} from "../../api";
export default {
  name: "Customswindow",
  data() {
    return {
      loading:false,
      isdeclare:false, //申报弹窗
      isWarehousing:false,  //是否一键入库
      ShelveID:"",//一键入库库位
      ShelveArr:[],//暂存库位列表
      selectionarr:[], //全选与多选
      fruit: [],
      detail_title: [
        {
          type: "selection",
          width: 50,
          align: "center"
        },
        {
          title: "#",
          slot: "indexs",
          align: "left",
          width: 30
        },
        {
          title: "箱号",
          slot: "boxnumber",
          align: "center"
        },
        {
          title: "库位",
          key: "Storehouse",
          align: "center",
          width:135,
          render: (h, params) => {
            var vm = this;
            var istrue=false;
            if(params.row.Status==500){
                istrue=true;
            }else{
                istrue=false
            }
            return (
              h("div"),
              [
                h(
                  "Select",
                  {
                    props: {
                      value: params.row.ShelveID,
                      transfer: true,
                      disabled:istrue
                    },
                    on: {
                      "on-change": value => {
                        // this.detail_list[params.row].Storehouse = event.target.value;
                        params.row.ShelveID = value;
                        vm.detail_list[params.index] = params.row
                      }
                    }
                  },
                  vm.ShelveArr.map(function(type) {
                    return h(
                      "Option",
                      {
                        props: { value: type.ID }
                      },
                      type.ID
                    );
                  })
                )
              ]
            );
          }
        },
        // {
        //   title: "库位",
        //   slot: "state",
        //   align: "center"
        // },
        {
          title: "状态",
          slot: "state",
          align: "center"
        },
        {
          title: "规格",
          slot: "specs",
          align: "center"
        },
        {
          title: "总毛重(kg)",
          key: "TotalWeight",
          align: "center",
          // width: 60,
          render: (h, params) => {
            var vm = this;
            var istrue=false;
            if(params.row.Status==500){
                istrue=true;
            }else{
                istrue=false
            }
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.TotalWeight,
                disabled:istrue
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.TotalWeight = event.target.value;
                  vm.detail_list[params.index] = params.row;
                }
              }
            });
          }
        },
        {
          title: "毛重(kg)",
          slot: "gross",
          align: "center"
        },
        {
          title: "客户名称",
          slot: "ClientID",
          align: "center",
          width:180,
        },
        
        {
          title: "型号",
          slot: "PartNumber",
          align: "left",
          width:180,
        },
        {
              type: 'expand',
              width: 50,
              render: (h, params) => {
                  return h('div', {
                      // props: {
                      //     row: params.row
                      // }
                  },'111111111111')
              }
          },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: "left",
          width:150,
        },
        {
          title: "批次",
          slot: "Datacode",
          align: "center"
        },
        {
          title: "数量",
          slot: "Quantity",
          align: "center"
        },
        {
          title: "原产地",
          slot: "place",
          align: "center"
        },
        {
          title: "操作",
          slot: "action",
          align: "center"
        }
      ],
      detail_list: [  ],
      cityList: [
        {
          value: "New York",
          label: "New York"
        },
        {
          value: "London",
          label: "London"
        },
        {
          value: "Sydney",
          label: "Sydney"
        },
        {
          value: "Ottawa",
          label: "Ottawa"
        },
        {
          value: "Paris",
          label: "Paris"
        },
        {
          value: "Canberra",
          label: "Canberra"
        }
      ],
      Isall:false, //显示所有人
      Isstatus:false,//显示已报关
      whid:sessionStorage.getItem("UserWareHouse"), //库房编号
      // all:0,//0 显示自己的箱子 1显示所有人
      // status:"",//状态
      key:"",//根据搜索条件搜索
      Isshowbox:false,//箱号变更
      boxSpecsarr:[],//箱子规格
      newboxcode:"",//选择的新箱号
      oldboxcode:"",//旧箱号
      boxingarr:[],//可用箱号列表
    };
  },
  created() {
    this. GetUsableShelves()
    this.getBoxingSpecs()
    this.getlist()
    this.getboxarr()
  },
  mounted() {
  },
  computed: {
    all(){
      if(this.Isall==true){
        return 1
      }else{
        return 0
      }
    },
    status(){
      if(this.Isstatus==true){
        return 500
      }else{
        return 0
      }
    }
  },
  methods: {
    getboxarr(){  //获取可用箱号
      var houseid=sessionStorage.getItem("UserWareHouse")
      GetBoxes(houseid,"200").then(res=>{
        this.boxingarr=res;
      })
    },
    getBoxingSpecs(){//获取箱号规格
      BoxingSpecs().then(res=>{
        console.log(res)
        this.boxSpecsarr=res.obj;
      })
    },
    getlist(){  //获取列表数据
      var data={
        whid:this.whid, //库房编号
        all:this.all,//0 显示自己的箱子 1显示所有人
        status:this.status,//状态
        key:this.key,//箱号
      }
      boxproducts(data).then(res=>{
        // console.log(res)
        this.detail_list=res.Data;
        this.loading=false;
      })
    },
    ok_declare() {  //确认申请报关
      // console.log(this.selectionarr)
      var data={
         WHID:sessionStorage.getItem("UserWareHouse"),
         BoxIds:[]
       }
      for(var i=0;i<this.selectionarr.length;i++){
          data.BoxIds.push(this.selectionarr[i].ID)
      }
      console.log(data)
      CustomsApply(data).then(res=>{
          if(res.val==400){
             this.$Message.error('申报失败，请重新操作');
          }else{
            this.$Message.success('申报成功');
            this.getlist()
          }
      })
      this.$refs.selection.selectAll(false);
    },
    cancel_declare(){  //取消申请报关

    },
    Warehousing_btn(){  // 一键入库弹出
      if(this.selectionarr.length<=0){
        this.$Message.error({
                    content: '至少选择一个要入库的产品',
                    duration: 1,
                    closable: true,
                    top:50,
                });
      }else{
        this.isWarehousing=true;
      }
    },
    ok_Shelve(){  //确认更改库位
        var selectarr=this.selectionarr;
        var detailarr=this.detail_list;
        for(var i=0;i<this.detail_list.length;i++){
          for(var j=0;j<selectarr.length;j++){
            if(this.detail_list[i].ID==selectarr[j].ID){
              this.detail_list[i].ShelveID=this.ShelveID;
            }
          }
        }
        this.selectionarr=[];
        this.$refs.selection.selectAll(false);
    },
    cancel_Shelve(){  //取消入库
      this.ShelveID="";
    },
    Shelvestate(value){
      if(value==true){
        this.isWarehousing=true;
      }else{
        this.isWarehousing=false;
        this.ShelveID="";
      }
    },
    selection_chenge(selection){
      this.selectionarr=selection;
    },
    GetUsableShelves(){
      var id=sessionStorage.getItem('UserWareHouse');
      GetUsableShelves(id).then(res=>{
        this.ShelveArr=res.obj;
      })
    },
    AskCustoms(){  //报关申请
       if(this.selectionarr.length<=0){
        this.$Message.error({
                    content: '至少选择一个要申请报关的产品',
                    duration: 1,
                    closable: true,
                    top:50,
                });
      }else{
        this.isdeclare=true;
      }
    },
   changestatus(){  //显示已报报关
      this.loading=true;
      this.getlist()
    },
   changeall(value){  //显示所有人
      this.loading=true;
      this.getlist()
    },
   search_btn(){  //筛选
        this.loading=true;
        this.getlist()
  },
   clear_search(){  //清空筛选
      this.loading=true;
      this.getlist()
    },
   changegross(){ //修改毛重
    
   },
   chengbox(oldcode){  //显示箱号
    this.Isshowbox=true;
    this.oldboxcode=oldcode;
    this.newboxcode=oldcode
   },
   cancel(){ //取消
    this.Isshowbox=false;
    this.oldboxcode="",
    this.newboxcode=""
   },
   ok_changebox(){ //确认变更箱号
      var data={
        WHID: sessionStorage.getItem("UserWareHouse"),
        oldCode:this.oldboxcode,
        newCode:this.newboxcode
      }
      console.log(data)
      ChangeBoxCode(data).then(res=>{
        console.log(res)
        if(res.success==true){
          this.$Message.success('箱号变更成功');
          this.getlist()
        }else{
          this.$Message.error('箱号变更失败');
        }
      })
   },
   printboxcode(){  //箱签打印
      var obj={
        ID:"",//箱号id
        boxcode:'',//箱号
        ClientName:"",//客户名称
        entCode:"",  //入仓号
        Boxing:"",//装箱人
        CreateDate:"",//装箱时间
        TotalParts:"",//条数
        TotalWeight:"",//总毛重
        IsCCC: false,
        IsCIQ: true,
        IsEmbargo: false,
        IsHighPrice: false,
      }
      this.$Message.warning('该服务暂未开通');
   }
  }
};
</script>

