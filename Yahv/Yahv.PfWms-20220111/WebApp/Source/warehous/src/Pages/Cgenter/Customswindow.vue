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
  /* width: 100px; */
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
    padding-left: 0px;
    padding-right:0px;
    text-align: center;
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
.arritem{
      margin:0 -18px;
      list-style:none;
      text-Align: center;
      padding-top: 9px;
      padding-bottom: 9px;
      border-bottom:1px solid #ccc;
      overflow-x: hidden;
      overflow: hidden;
      min-height: 35px;
}
ivu-table-cell div .arritem:last-child{
  border-bottom: none
}
.tabletitle{
  line-height: 35px;
  background: #ececec;
  border: 1px solid #dddddd;
  border-bottom: none;
  /* display: flex;
  flex-direction:row;
  justify-content: space-around; */

    font-size: 0;
    -webkit-text-size-adjust:none;
}
.tabletitle span{
  display: inline-block;
  font-size: 14px;
  /* height: 35px; */
}
/* .boxtable >>>  .ivu-table td{
  border-bottom: 1px solid #e8eaec;
} */
.ordertable >>> .ivu-table-wrapper{
    border: none;
    border-left: 1px solid #e8eaec;
}
.boxtable >>> .ivu-table-wrapper{
    border: none;
    border-left: 1px solid #e8eaec;
}
/* #Customsbox >>> .ivu-table-tbody tr:last-child  td{
  border-bottom: none;
} */
.ordertable >>> ivu-table-column-center{
  border-left: 1px solid #e8eaec;
}
.titleboxs{
  display: flex;
  flex-direction:row;
  justify-content: space-around;
  text-align: center
}
.ordertable >>> .ivu-select-single .ivu-select-selection{
  height: 35px;
}
.tagbox{
   display: flex;
  flex-direction:row;
  justify-content: space-between;
}
</style>
<template>
  <div class="Customswindow" id="Customsbox">
    <p  class="windowtitle title">装箱清单重构</p>
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
        <!-- <Button type="text" icon="ios-create-outline" ghost @click="backpagetest">跳转测试</Button> -->
        <Button type="text" icon="ios-create-outline" ghost @click="AskCustoms">报关申请</Button>
      </div>
    </div>
    <div style="margin:10px 0;clear: both;">
      <!-- <Checkbox v-model="Isstatus" @on-change="changestatus">显示已报关</Checkbox> -->
      <Checkbox v-model="Isall" @on-change="changeall">显示所有人</Checkbox>
    </div>
    <div>
      <div class="tabletitle">
        <span style="text-align:center" id="span1">#</span>
        <span style="text-align:center" id="span2">订单号</span>
        <span style="text-align:center" id="span3">申报状态</span>
        <span style="text-align:center" id="span4">装箱时间</span>
        <span style="text-align:center" id="span5">装箱人</span>
        <span style="text-align:center" id="span6">客户编号</span>
        <span style="text-align:center" id="span7">箱号</span>
        <!-- <span style="text-align:center" id="span8">库位</span>
        <span style="text-align:center" id="span9">规格</span>
        <span style="text-align:center" id="span10">总毛重</span> -->
        <span style="text-align:center" id="span11">毛重</span>
        <span style="text-align:center" id="span12">型号</span>
        <span style="text-align:center" id="span13">品牌</span>
        <span style="text-align:center" id="span14">批次</span>        
        <span style="text-align:center" id="span15">数量</span>        
        <span style="text-align:center" id="span16">原产地</span>
        <span style="text-align:center;padding-left:11px;" id="span16">操作</span>        
     </Row>   
      </div>
      
       <Table :columns="columns1" :data="new_datas" :show-header='false' class="ordertable" border>
          <template slot-scope="{ row, index }" slot="Address">
            <Table :columns="columns2" :data="row.boxdata" :show-header='false' class="boxtable">
              <template slot-scope="{ row, index }" slot="ShelveID">
                  <Select v-model="row.ShelveID"  filterable  transfer style="padding:0px 3px" @on-change='testselect($event,index,row)'>
                    <Option v-for="item in ShelveArr" :value="item.ID" :key="item.ID">{{item.ID}}</Option>
                  </Select>
              </template>
              <template slot-scope="{ row, index }" slot="BoxSpecs">
                  <Select v-model="row.BoxingSpecs" transfer style="padding:0px 3px">
                    <Option v-for="item in boxSpecsarr" :value="item.value" :key="item.value">{{ item.name }}</Option>
                  </Select>
              </template>
               <template slot-scope="{ row, index }" slot="test1">
                  <Table :columns="columns3" :data="row.productdata" :show-header='false' class="producttable">
                    <template slot-scope="{ row, index }" slot="PartNumber">
                      <div class="tagbox">
                        <div>AD685/RER_FD/508-TREGDG</div>
                        <ul>
                         <li><Tag color="primary">primary</Tag></li>
                         <!-- <li><Tag color="primary">primary</Tag></li>
                         <li><Tag color="primary">primary</Tag></li> -->
                        </ul>
                      </div>
                     
                    </template>
                    <template slot-scope="{ row, index }" slot="Proaction">
                      <Button type="primary" size="small">变更箱号</Button>
                    </template>
                  </Table>
              </template>
            </Table>
        </template>
       </Table>
    </div>
    <div>
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
import $ from 'jquery'
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
      columns1: [
        {
          type: "selection",
          width: 50,
          align: "center"
        },
        {
            title: '订单号',
            key: 'name',
            align: "center",
            width:90
        },
        {
            title: '申报状态',
            key: 'age',
            align: "center",
            width:80
        },
        {
            title: '装箱时间',
            key: 'age',
            align: "center",
            width:80
        },
        {
            title: '装箱人',
            key: 'age',
            align: "center",
            width:80
        },
        {
            title: '客户编号',
            key: 'age',
            align: "center",
            width:80
        },
        {
            slot: 'Address',
            title: 'address',
            align: "left"
        }
       ],
    columns2: [
              {
                  title: '箱号',
                  key: 'name',
                  width:80
              },
              // {
              //     title: '库位',
              //     slot: 'ShelveID',
              //     width:110,
              //     align:"left",
              // },
              // {
              //     title: '规格',
              //     slot: 'BoxSpecs',
              //     width:80
              // },
              // {
              //     title:'总毛重',
              //     key: 'age',
              //     width:90,
              //     render: (h, params) => {
              //     var vm = this;
              //     return h("Input", {
              //       props: {
              //         //将单元格的值给Input
              //         value: '',
              //         // disabled:istrue
              //       },
              //       style:{
              //          padding: '0 3px'
              //        },
              //       on: {
              //         "on-change"(event) {
              //           //值改变时
              //           //将渲染后的值重新赋值给单元格值
              //           // params.row.TotalWeight = event.target.value;
              //           // vm.detail_list[params.index] = params.row;
              //         }
              //       }
              //     });
              //   }
              // },
              {
                  title: '',
                  slot: 'test1',
              }
       ],
    columns3: [
              {
                  title: '毛重',
                  key: 'name',
                  width:110,
              },
              {
                  title: '型号',
                  slot: 'PartNumber',
                  width:170,
              },
              {
                  title: '品牌',
                  key: 'age',
                  width:140,
              },
              {
                  title: '批次',
                  key: 'age',
                  width:80,
              },
              {
                  title: '数量',
                  key: 'age',
                  width:70,
              },
              {
                  title: '原产地',
                  key: 'age',
                  minWidth:110,
              },
              {
                  title: '操作',
                  slot: 'Proaction',
                  minWidth:90,
              },
       ],
     data1: [
        {
            name: 'John Brown',
            age: 18,
            address: 'New York No. 1 Lake Park',
            date: '2016-10-03'
        },
        {
            name: 'Jim Green',
            age: 24,
            address: 'London No. 1 Lake Park',
            date: '2016-10-01'
        },
        {
            name: 'Joe Black',
            age: 30,
            address: 'Sydney No. 1 Lake Park',
            date: '2016-10-02'
        },
        {
            name: 'Jon Snow',
            age: 26,
            address: 'Ottawa No. 2 Lake Park',
            date: '2016-10-04'
        }
    ],
    new_datas:[
      {
          name: '订单1',
          age: 18,
          address: 'New York No. 1 Lake Park',
          date: '2016-10-03',
          boxdata:[
            {
              name: 'John Brown',
              ShelveID: 'HK01-F01-0101',
              address: 'New York No. 1 Lake Park',
              date: '2016-10-03',
              productdata:[
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
              ]
            },
            {
                  name: 'John Brown',
                  ShelveID: 'HK01-F01-0102',
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                  productdata:[
                      {
                        name: 'John Brown',
                        age: 18,
                        address: 'New York No. 1 Lake Park',
                        date: '2016-10-03',
                      },
                  ]
            },
            {
                  name: 'John Brown',
                  ShelveID: 'HK01-F01-0102',
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                  productdata:[
                      {
                        name: 'John Brown',
                        age: 18,
                        address: 'New York No. 1 Lake Park',
                        date: '2016-10-03',
                      },
                      {
                        name: 'John Brown',
                        age: 18,
                        address: 'New York No. 1 Lake Park',
                        date: '2016-10-03',
                      },
                  ]
            },
          ]
      },
      {
          name: '订单二',
          age: 18,
          address: 'New York No. 1 Lake Park',
          date: '2016-10-03',
          boxdata:[
            {
              name: 'John Brown',
              ShelveID: 'HK01-F01-0101',
              address: 'New York No. 1 Lake Park',
              date: '2016-10-03',
              productdata:[
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
                {
                  name: 'John Brown',
                  age: 18,
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                },
              ]
            },
            {
                  name: 'John Brown',
                  ShelveID: 'HK01-F01-0102',
                  address: 'New York No. 1 Lake Park',
                  date: '2016-10-03',
                  productdata:[
                      {
                        name: 'John Brown',
                        age: 18,
                        address: 'New York No. 1 Lake Park',
                        date: '2016-10-03',
                      },
                  ]
            },
          ]
      }
    ]
    };
  },
  created() {
    // this. GetUsableShelves()
    // this.getBoxingSpecs()
    // this.getlist()
    // this.getboxarr()
    this.setnva()
  },
  mounted() {
    // console.log($(".ordertable table tr:eq(0)").find('td:eq(0)').width())
    // console.log($(".ordertable table tr:eq(0)").find('td:eq(1)').width())
    // console.log($(".ordertable table tr:eq(0)").find('td:eq(2)').width())
    // console.log($(".producttable table tr:eq(0)").find('td:eq(0)').width())
    // console.log($(".producttable table tr:eq(0)").find('td:eq(1)').width())
    // console.log($(".producttable table tr:eq(0)").find('td:eq(2)').width())
    // console.log($(".producttable table tr:eq(0)").find('td:eq(3)').width())
    // console.log($(".producttable table tr:eq(0)").find('td:eq(4)').width())
    console.log($(".producttable  table tr:eq(0) td:eq(0)").width())


     $("#span1").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(0)').width()+1)
     $("#span2").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(1)').width()+1)
     $("#span3").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(2)').width()+1)
     $("#span4").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(3)').width()+1)
     $("#span5").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(4)').width()+1)
     $("#span6").width($(".ordertable table:eq(0) tr:eq(0)").find('td:eq(5)').width()+1)
     $("#span7").width($(".boxtable table tr:eq(0)").find('td:eq(0)').width()+1)
    //  $("#span8").width($(".boxtable table tr:eq(0)").find('td:eq(1)').width()+1)
    //  $("#span9").width($(".boxtable table tr:eq(0)").find('td:eq(2)').width()+1)
    //  $("#span10").width($(".boxtable table tr:eq(0)").find('td:eq(3)').width()+1)
     $("#span11").width($(".producttable table tr:eq(0)").find('td:eq(0)').width()+1)
     $("#span12").width($(".producttable table tr:eq(0)").find('td:eq(1)').width()+1)
     $("#span13").width($(".producttable table tr:eq(0)").find('td:eq(2)').width()+1)
     $("#span14").width($(".producttable table tr:eq(0)").find('td:eq(3)').width()+1)
     $("#span15").width($(".producttable table tr:eq(0)").find('td:eq(4)').width()+1)
    //  $("#span16").width($(".producttable table tr:eq(0)").find('td:eq(5)').width()+1)
     $("#span16").width(130)

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
    setnva() {
      var cc = [
        {
          title: "申报",
          href: "/Transport/Customswindow"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    backpagetest(){
      this.$store.dispatch("setshowdetail", false);
      this.$router.push({ path: "/Cgenter" });
    },
    testselect(event,index,row){
      console.log(event)
      console.log(index)
      console.log(row)
      // console.log(this.new_datas)
    },
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

