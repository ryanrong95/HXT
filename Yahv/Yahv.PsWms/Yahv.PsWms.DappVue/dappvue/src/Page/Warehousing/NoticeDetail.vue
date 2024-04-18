<style scoped>
.Noticedetail .detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.Noticedetail .setboxtop {
  margin: 5px 0px;
}
.Noticedetail .infobox {
  display: inline-block;
  width: 100%;
  max-height: 150px;
  /* min-height: 75px; */
  padding: 0px 10px;
}
.infoul li {
  font-size: 15px;
  line-height: 30px;
}
table {
  /* border-right: 1px solid #804040;
  border-bottom: 1px solid #804040; */
  border-collapse: collapse;
  font-size: 14px;
  /* border: 1px solid #000; */
}
table tr,thead{
  border-bottom: 1px solid #e8eaec
}
table thead{
  border-top: 1px solid #e8eaec;
  background-color: #f8f8f9
}
table td {
  /* border-left: 1px solid #804040;
  border-top: 1px solid #804040; */
  text-align: center;
}

.inputclass {
  height: 25px;
  width: 77%;
  margin: 3px 7px;
  border-radius: 4px;
  border: 1px solid #ccc;
  text-indent: 5px;
}

.inputtotal{
  height: 25px;
  width: 50px;
  margin: 3px 7px;
  border-radius: 4px;
  border: 1px solid #ccc;
}
input[type="text"]:focus {
  border: 1px solid #afecab;
}

.htmlbox {
  width: 100%;
  margin: 0 auto;
}

.active {
  background: #deedf7;
}
.Noticedetail th {
  line-height: 30px;
}
table thead,
tbody tr {
  display: table;
  width: 100%;
  table-layout: fixed;
}
th {
  /* border-right: 1px solid; */
  white-space: nowrap
}
table thead {
  width: calc(100% - 17px);
}
th,td{
  white-space: nowrap
}
.rowcols{
  /* display: inline-block; */
  float: left;
  margin-right: 30px;
}
.delicon{
       font-size: 20px;
    color: red;
    position: relative;
    top: -60px;
    left: 35px;
}
.imgbox,.delicon:hover{
  cursor: pointer;
}
.imgbox{
  float: left;
  width: 70px;
  height: 70px;
  margin-right: 10px;
}
.itemimgbox{
    display: inline-block;
    width: 12%;
    font-size: 19px;
}
.addbtnstyle{
    display: inline-block;
}
.rightspan{
  width:95px;
  display: inline-block;
  text-align: right;
}
.leftspan{
  width:60px;
  display: inline-block;
  text-align: left;
}
.delicon{
    font-size: 17px;
    color: red;
    position: relative;
    top: -85px;
    left: 59px;
}
.delicon:hover{
  cursor: pointer;
}
.masticon{
    color: red;
    font-size: 18px;
    vertical-align: sub;
    padding-right: 3px;
}
</style>
<template>
  <div class="Noticedetail" v-if="infodata!=null" id="detail">
    <p class="detailtitle">基础信息</p>
    <div class="setboxtop infobox" >
       <div class="rowcols">
          <ul class="infoul">
            <li><span>订单号：</span><span>{{infodata.FormID}}</span></li>
            <li v-if='infodata.TransportMode==2'><span>承运商：</span>
               <span>
                    <Select v-model="infodata.Carrier" style="width:200px">
                        <Option v-for="item in Expresarr" :value="item.value" :key="item.value">{{ item.name }}</Option>
                    </Select>
                  </span>
            </li>
            <li v-if='infodata.TransportMode==1'><span>联系人：</span><span>{{infodata.Contact}}</span></li>
            <li v-if='infodata.TransportMode==1'><span>司&nbsp;&nbsp;&nbsp;机：</span><span> 
              <Select
                  v-model="infodata.TakerName"
                  filterable
                  @on-change="changeCarrier"
                  style="width:200px"
                  :disabled='true'
                >
                  <Option
                    v-for="item in Takers"
                    :value="item.Name"
                    :label="item.Name"
                    :key="item.value"
                    :label-in-value="true"
                  >
                    <p>
                       <span>{{ item.Name }}</span>
                       <span>{{ item.Licence }}</span>
                    </p>
                   <p><span>{{ item.Phone }}</span></p>
                  </Option>
                </Select></span></li>

          </ul>
       </div>
        <div class="rowcols">
         <ul class="infoul">
            <li><span>客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：</span><span>{{infodata.ClientName}}</span></li>
            <li  v-if='infodata.TransportMode==2'><span>运单号：</span><span><Input v-model="infodata.WaybillCode" placeholder="请输入运单号" style="width:180px" /></span></li>
            <li  v-if='infodata.TransportMode==1'><span>联系电话：</span><span>{{infodata.Phone}}</span></li>
            <li  v-if='infodata.TransportMode==1'><span>车&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;牌：</span><span>{{infodata.TakerLicense}}</span></li>
          </ul>
       </div>
        <div class="rowcols">
         <ul class="infoul">
            <li><span>通知类型：</span><span>{{infodata.NoticeTypeDes}}</span></li>
            <li  v-if='infodata.TransportMode==1'><span>提货时间：</span><span>{{infodata.TakingTime|showDate}}</span></li>
            <li v-if='infodata.TransportMode==2'><span>异常备注：</span><span><Input v-model="infodata.Exception" placeholder="异常备注" style="width: 170px" /> 
              </span>
              </li>
            <li v-if='infodata.TransportMode==1'>
              <span>异常备注：</span>
              <span><Input v-model="infodata.Exception" placeholder="异常备注" style="width: 170px" /></span>
            </li>
          </ul>
       </div>
        <div class="rowcols">
         <ul class="infoul">
            <li><span>运输方式：</span><span>{{infodata.TransportModeDes}}</span></li>
            <li v-if='infodata.TransportMode==1'><span>提货地址：</span><span>{{infodata.Address}}</span></li>
            <li v-if='infodata.TransportMode==1||infodata.TransportMode==2'> <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'
                @click="submit_Pickupgoods"  >提交</Button></li>
                
          </ul>
       </div>
       <!-- <div class="rowcols">
         <ul class="infoul">
            <li  v-if='infodata.TransportMode==2'><span>送货时间：</span><span>{{infodata.TakingTime|showDate}}</span></li>
         </ul>
       </div> -->
       <div class="rowcols" v-if="infodata.TransportMode==3">
         <ul class="infoul">
           <li>
             <span>异常备注：</span>
             <span><Input v-model="infodata.Exception" placeholder="异常备注" style="width: 170px" /></span>
             <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'
                @click="submit_Pickupgoods"  >提交</Button>
           </li>
         </ul>
       </div>
      </div>
      <p style="padding-left: 10px;" v-if="infodata.Requires.length>0">  特殊需求：
        <Tag color="magenta" v-for="item in infodata.Requires" :key="item.Contents">{{item.Contents}}</Tag></p>
      <div>
        <!-- 图片列表  -->
        <PhotoList :childendata='childendata'></PhotoList>
         <!-- 图片列表  -->
      </div>
     <p class="detailtitle" style="clear: both;"> <span>文件列表</span> </p>
    <div style="margin: 10px 5px">
       <Flielist :ID='infodata.ID'></Flielist>
    </div>
   <p class="detailtitle">
      <span style="margin-right:10px">费用明细</span> 
      <Button type="success" size="small" icon="md-add" @click="showNoticeCharges" >客户费用录入</Button></p>
    <div style="margin: 10px 5px">
       <NoticeChargeslist :NoticeChargeslist='NoticeChargeslist' :Chargeslistloading='Chargeslistloading'  @fatherMethod="fatherMethod"></NoticeChargeslist>
    </div>
    <p class="detailtitle">产品清单    
        <span style="" class="addrowbtn" @click="addrow" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'>
          <Button type="success" size="small" icon="md-add" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'>新增分拣</Button>  
        </span> 
         <Button type="info" size="small" icon='md-checkbox-outline' :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' @click="Review">完成复核</Button>
    </p>
    <div style="padding-top: 10px">
      <table
        cellspacing=""
        cellpadding=""
        class="tablebox"
        width="100%"
        id='producttable'
      >
        <thead>
          <th><span class="masticon">*</span>型号</th>
          <th><span class="masticon">*</span>品牌</th>
          <th>封装</th>
          <th>批次</th>
          <!-- <th><span class="masticon">*</span>数量(Mpq/应到/本次/已到)</th> -->
          <th style="width:85px"><span class="masticon">*</span>Mpq</th>
          <th style="width:85px">应到</th>
          <th style="width:95px"><span class="masticon">*</span>本次到货</th>
          <th style="width:85px">已到</th>
          <th>异常</th>
          <th>备注</th>
          <th>操作</th>
        </thead>
        <tbody
          style="width: 100%; display: block; overflow-y: scroll"
        >
          <tr
            v-for="(item, index) in Noticedata"
            :key="index"
            :class="{ active: currentIndex ==index }"
            v-on:click="changeActive(index, item)"
          >
            <td class="tdbox">
              <input class="inputclass" type="text" v-model="item.Partnumber"  :disabled='item.StorageTotal>0?true:false||infodata.Status==600?true:false||infodata.Status==500?true:false'/>
              <span class="itemimgbox">
                <Icon type="md-images" class="remclass" v-if="item.FileExist==true" @click="PartnumberFiles(item.ID)"/>
              </span>
            </td>
            <td class="tdbox">
              <input class="inputclass" type="text" v-model="item.Brand" :disabled='item.StorageTotal>0?true:false||infodata.Status==600?true:false||infodata.Status==500?true:false' />
            </td>
            <td class="tdbox">
              <input class="inputclass" type="text" v-model="item.Package" :disabled='item.StorageTotal>0?true:false||infodata.Status==600?true:false||infodata.Status==500?true:false' />
            </td>
            <td class="tdbox">
              <input class="inputclass" type="text" v-model="item.DateCode" :disabled='item.StorageTotal>0?true:false||infodata.Status==600?true:false||infodata.Status==500?true:false' />
            </td>
            <td style="width:85px">
              <div v-if="item.IsNew==true" style="display: inline-block">
                <span v-if="item.StoragePackageNumber>0">{{item.Mpq}}</span>
                <input v-else class="inputtotal" type="text" placeholder="Mpq"  :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.Mpq" @blur="changeActive(index, item)"/>
              </div>
                <div v-else style="display: inline-block">
                  <span>{{item.Mpq}}</span>
               </div>
            </td>
             <td style="width:85px">
              <p v-if="item.IsNew==false"> {{item.PackageNumber}}</p>
            </td>
             <td  style="width:95px">
               <span>
                <input v-if="item.IsNew==true" class="inputtotal" type="text"  :disabled='item.StoragePackageNumber>0?true:false' v-model="item.DeliveryCount" />
                <input v-else class="inputtotal" type="text"  :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.DeliveryCount" @change="setDeliveryCount($event,index)"/>
               </span>
              <em v-if="item.Mpq>1">件</em>
              <em v-else>个</em>
            </td>
             <td style="width:85px">
              {{item.StoragePackageNumber}}
            </td>
            <!-- <td class="tdbox">
              <div v-if="item.IsNew==true" style="display: inline-block">
                <span v-if="item.StoragePackageNumber>0">{{item.Mpq}}</span>
                <input v-else class="inputtotal" type="text" placeholder="Mpq"  :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.Mpq" @blur="changeActive(index, item)"/>/&nbsp;
              </div>
              <div v-else style="display: inline-block">
                  <span>{{item.Mpq}} /&nbsp;{{item.PackageNumber}}&nbsp;/ </span>
              </div>
                <input class="inputtotal" type="text"  :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.DeliveryCount" @change="setDeliveryCount($event,index)"/>
                <em v-if="item.Mpq>1">件</em>
                <em v-else>个</em>
                <span> /&nbsp;{{item.StoragePackageNumber}} </span>
            </td> -->
            <td class="tdbox">
              <input class="inputclass" type="text" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.Exception" />
            </td>
             <td class="tdbox">
              <input class="inputclass" type="text" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' v-model="item.Summary" />
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.Name" @keyup.enter="inputEnter($event)" /> -->
              <div style="margin: 4px 0px">
                <Button type="primary" size="small" @click="printitem(item.Printer)" :disabled='item.Printer.length<=0?true:false'>打印标签</Button>
                <Button type="primary" size="small" @click="FormPhotobtn(item)" class="remclass">拍照</Button>
                <Button v-if="item.IsNew==true&&infodata.Status!=600&&infodata.Status!=600" type="primary" size="small" @click="delItemnotice(item.ID)" class="remclass">删除</Button>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
      <div style="padding:10px 0px 50px 0px;text-align: center;">
         <span style="" class="addrowbtn" @click="addrow" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'>
          <Button type="success" size="small" icon="md-add" :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false'>新增分拣</Button>  
         </span> 
         <Button type="info" size="small" icon='md-checkbox-outline' :disabled='infodata.Status==600?true:false||infodata.Status==500?true:false' @click="Review">完成复核</Button>
      </div>
    </div>
    <!-- 费用录入  开始-->
     <Modal
        :width='53'
        v-model="showCharges"
        title="客户费用录入">
        <NoticeCharges :key="timer" ref="NoticeCharges" :sumbitChargesdata='sumbitChargesdata' @fatherMethod="fatherMethod"></NoticeCharges>
        <div slot="footer">
            <Button @click="cancel_NoticeCharges">取消</Button>
            <Button @click="ok_NoticeCharges" type="primary">确定</Button>
        </div>
     </Modal>
     <!-- 费用录入  结束-->
     <!-- 通知项图片展示 -->
    <Modal :width="53" v-model="showproductfile" title="图片列表" @on-visible-change='showproductfilechange'>
      <div v-if="productlist.length<=0" style="display: flex;flex-direction: column; width: 100px; margin: 0 auto;">
        <img  src="../../assets/img/null.jpg" alt="" style="width:48px;height:48px">
        <span class="nulltitle">暂无外观图片</span>
      </div>
      <ul v-else style="display: flex;">
         <li v-for="item in productlist" style="" class="imgbox">
           <img style="width:70px;height:70px" :src="item.Url" alt="" @click="FilesProcess(item.Url)">
           <Icon type="md-close-circle" class="delicon" @click="PhotoFileDelete(item.ID,)"/>
         </li>
      </ul>
      <div slot="footer">
        <Button type="primary" @click="showproductfile=false">关闭</Button>
      </div>
    </Modal>
    <!-- 打印列表展示 -->
    <Modal
        :width='53'
        v-model="printbox"
        :footer-hide='true'
        title="打印列表">
         <Table :columns="printcolumns" :data="printitemdata">
           <template slot-scope="{ row, index }" slot="action">
                <Button type="primary" size="small" @click="printInlabel(row)">打印入库标签</Button>
          </template>
         </Table>
     </Modal>
  </div>
</template>
<script>
document.onkeydown = function () {
  if (
    event.keyCode == 37 ||
    event.keyCode == 38 ||
    event.keyCode == 39 ||
    event.keyCode == 40
  ) {
    var td1 = document.activeElement.parentNode;
    var tr1 = td1.parentNode;
    var t1 = tr1.parentNode;
    var rows = t1.rows;
    var cells = tr1.cells;
    var j = td1.cellIndex;
    var i = tr1.rowIndex;
    var inputs = "";
    switch (event.keyCode) {
      case 37:
        if (j - 1 < 0) return false;
        if (rows[i].cells[j - 1].childNodes.length != 0) {
          inputs = rows[i].cells[j - 1].childNodes;
        }
        break;
      case 38:
        if (i - 1 < 0) return false;
        if (rows[i - 1].cells[j].childNodes.length != 0) {
          inputs = rows[i - 1].cells[j].childNodes;
          console.log("上键" + (i - 1));
          var index = i - 1;
          window.keyboardclick(index);
        }
        break;
      case 39:
        if (j + 1 >= cells.length) return false;
        if (rows[i].cells[j + 1].childNodes.length != 0) {
          inputs = rows[i].cells[j + 1].childNodes;
        }
        break;
      case 40:
        if (i + 1 >= rows.length) return false;
        if (rows[i + 1].cells[j].childNodes.length != 0) {
          inputs = rows[i + 1].cells[j].childNodes;
          console.log("下键" + (i + 1));
          var index = i + 1;
          window.keyboardclick(index);
        }
        break;
    }
    if (inputs != "") {
      for (var k = 0; k < inputs.length; k++) {
        if (inputs[k].type == "text") {
          inputs[k].focus();
        }
      }
    }
  }
};

import{InNoticesDetail ,NoticeItems,Hearting,InNoticesSorting,InNoticesUpdate,Review,GetNoticeItemID,DeleteNoticeItem} from '../../api/Enter'
import {NoticeCharges_list,TakersList} from "../../api/Out";
import {GetPhotoFiles,PhotoFileDelete,getExpress,PartnumberFiles} from "../../api/index";

import{FormPhoto,SeletUploadFiles,FilesProcess,TemplatePrint,GetPrinterDictionary} from '../../js/browser'
let product_url = require("../../../static/pubilc.dev");
import NoticeCharges from "../Publicview/NoticeCharges";
import NoticeChargeslist from "../Publicview/NoticeChargeslist";
import Flielist from "../Publicview/Filelist"
import PhotoList from "../Publicview/PhotoList"
export default {
  components: {
     NoticeCharges,
     NoticeChargeslist,
     Flielist,
     PhotoList
  },
  name: "tables",
  data() {
    return {
      printbox:false,
      AdminID:sessionStorage.getItem("userID"),
      ID:null,
      allcheckbox: false,
      total: 3000,
      pageSize: 20,
      data: [],
      loading: false,
      currentIndex: null,
      oldobj: null,
      newobj: null,
      timenmae: null, //定时器
      loading: true,
      type:1,
      Costcolumns: [
        {
          title: "科目",
          key: "name",
        },
        {
          title: "类型",
          key: "age",
        },
        {
          title: "数量",
          key: "age",
        },
        {
          title: "单价",
          key: "age",
        },
        {
          title: "单位",
          key: "age",
        },
        {
          title: "总额",
          key: "age",
        },
        {
          title: "记录时间",
          key: "age",
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center",
        },
      ],
       data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03",
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01",
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02",
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04",
        },
      ],
      infodata:null,
      Takers:[],
      NoticeChargeslist: [], //费用列表
      Chargeslistloading:true, //费用loading
      showCharges:false,//是否显示费用弹出框,
      sumbitChargesdata: {
        FormID:null,
        NoticeID: null, //是	string	通知ID
        ClientID:null,
        ClientName:null,
      },  
      timer:'',
      PhotoFilearr:[],//图片列表
      phototype:'1',
      photobtntype:null,
      childendata:{ //图片组件参数
        ID:null,
        showtype:0
      },
      Expresarr:[], //承运商列表
      showproductfile:false,
      productlist:[],//产品项图片列表
      clickimg:null,
      printurl: product_url.pfwms,
      printcolumns:[
        {
            title: '#',
            type: 'index',
            width:60,
            align:'center'
        },
        {
            title: '型号',
            key: 'Partnumber',
            align:'center'
        },
        {
            title: '品牌',
            key: 'Brand',
            align:'center'
        },
        {
            title: '封装',
            key: 'Package',
            align:'center'
        },
        {
            title: '批次',
            key: 'DateCode',
            align:'center'
        },
        {
            title: '数量',
            key: 'PackageNumber',
            align:'center'
        },
        {
            title: '操作',
            slot: 'action',
            align:'center'
        }
      ],
      printitemdata:[],
      Noticedata:[],// 通知列表
    };
  },
  created() {
     this.ID=this.$route.params.detailID
     this.$nextTick(function () {
      this.getData(this.$route.params.detailID);
      this.TakersList()
      this.NoticeCharges_list(this.$route.params.detailID)
      // this.GetPhotoFiles()
      this.NoticeItems(this.$route.params.detailID)
      window["PhotoUploadedFun"] = this.changedimg;
    });
    window.keyboardclick = this.keyboardclick;
  },
  computed: {},
  mounted() {
   window.addEventListener('click', this.wsFunc,true)
  },
  methods: {
    wsFunc(e) {
      // console.log(e.target.className)
         if (
          e.target.className === "tablebox" ||
          e.target.className === "inputclass" ||
          e.target.className === "tdbox" ||
          e.target.className === "addrowbtn"||
          e.target.className === "inputtotal"
        ) {
        } else {
          this.removeActive();
        }
    },
    //提交提货数据
    submit_Pickupgoods(){
      var data={
        "NoticeID":this.infodata.ID, //通知ID
        "ConsignorID":this.infodata.ConsignorID, // 交货人ID
        "Carrier":this.infodata.Carrier, // 承运商
        "WaybillCode":this.infodata.WaybillCode, //运单号
        "Exception":this.infodata.Exception // 异常备注
      }
      InNoticesUpdate(data).then(res=>{
        console.log(res)
        if(res.success==true){
          this.$Message.success('提交成功');
        }else{
           this.$Message.error(res.data);
        }
      })
    },
    //获取详情基础信息的接口
    getData(id) {
     InNoticesDetail(id).then(res=>{
       this.infodata=res.data
       this.sumbitChargesdata.FormID=this.infodata.FormID
       this.sumbitChargesdata.NoticeID=this.infodata.ID
       this.sumbitChargesdata.ClientID=this.infodata.ClientID
       this.sumbitChargesdata.ClientName=this.infodata.ClientName

       this.childendata.ID=this.infodata.ID
       if(this.infodata.TransportMode!=1){
         this.getExpress()
       }
     })
    },
    // 承运商列表
    getExpress(){
      getExpress().then(res=>{
        this.Expresarr=res.obj
      })
    },
    //司机列表
    TakersList(){
      TakersList().then(res=>{
        this.Takers=res.data
      })
    },

    //费用列表
    NoticeCharges_list(id) {
      NoticeCharges_list(id).then((res) => {
        this.NoticeChargeslist = res;
        this.Chargeslistloading=false
      });
    },
    changeCarrier(value) {
      var obj = this.Takers.filter((item) => item.Name == value)[0];
      this.infodata.TakerLicense = obj.Licence;
      this.infodata.TakerName=obj.Name;
    },


    aa(row) {
      console.log(row);
    },
    inputEnter(e) {
      var value = e.target.value;
      console.log(value);
    },
    //入库通知详情页面中通知项的心跳
    Hearting(ID,index){
      var data={
           "AdminID":sessionStorage.getItem("userID"), // 当前对通知项进行操作的请求人
           "NoticeItemID":ID //通知项ID
        }
      Hearting(data).then(res=>{
        if(res.data==true){
          // this.data[index].isdisable=true
        }else{
          // this.data[index].isdisable=false
        }
      })
    },
    // 提交分拣接口
    InNoticesSorting(oldobj){
      // console.log(oldobj)
      var newStocktakingType=null
      if(oldobj.Mpq>1){
        newStocktakingType=2
      }else{
        newStocktakingType=1
      }
      var data={
        Sortings:[
          {
            'IsNew': oldobj.IsNew,
            "NoticeID": this.infodata.ID, //通知ID
            "NoticeItemID": oldobj.ID,//通知项ID
            "StocktakingType":newStocktakingType, //盘点类型, 1 按量(个)分拣, 2 按MPQ分拣
            "Mpq":  oldobj.Mpq, //最小包装量
            "PackageNumber": oldobj.PackageNumber, //应到数量
            "Total": Number(oldobj.Mpq)*Number(oldobj.DeliveryCount), // 总数
            "SorterID":this.AdminID, //分拣人ID
            "DeliveryCount":Number(oldobj.DeliveryCount), //本次到货数量
            "ShelveID": null, //货架ID 不用给值,就默认null, 这个值是上架时才有的
            "Exception":oldobj.Exception, //异常到货说明
            "Summary":oldobj.Summary,
            "Product": {
              "Partnumber": oldobj.Partnumber, //型号
              "Brand": oldobj.Brand,//品牌
              "Package":oldobj.Package, //封装
              "DateCode":oldobj.DateCode,//批次
              "Mpq":oldobj.Mpq,//最小包装量
              "Moq":oldobj.Moq //最小起订量
            }
         }
       ]
      }
      InNoticesSorting(data).then(res=>{
        // console.log(res)
        if(res.success==true){
          this.$Message.success('分拣成功');
          this.NoticeItems(this.$route.params.detailID)
        }else{
            this.$Message.error(res.data);
        }
      })
    },
    
    //键盘移动控制选中与提交
    keyboardclick(index) {
      this.currentIndex = index;
      this.oldobj = this.newobj;
      this.newobj = this.Noticedata[index];
      this.Hearting(this.newobj.ID)
      // console.log(
      //   "通讯session,禁止其他人操作当前行数据-当前数据:" +
      //     JSON.stringify(this.newobj)
      // );
     if(this.oldobj!=null){
        var ret2 = this.Noticedata.find((v) => {
                return v.ID == this.oldobj.ID;
          });
          if(!ret2.Partnumber==false&&!ret2.Brand==false&&!ret2.DeliveryCount==false){
            this.InNoticesSorting(ret2)
          }else{
            // this.$Message.warning('请输入分拣数据');
          }
        // console.log("保存上一行数据" + JSON.stringify(this.oldobj));
      }
    },

    //鼠标点击某一行，进行通讯
    changeActive(index, item) {
      // console.log(item)
      if (this.currentIndex != index) {
        this.currentIndex = index;
        this.oldobj = this.newobj;
        this.newobj = item;
        this.Hearting(this.newobj.ID)
      
        // console.log(
        //   "通讯session,禁止其他人操作当前行数据-当前数据:" +
        //     JSON.stringify(this.newobj.Partnumber+'分拣数量：'+this.newobj.DeliveryCount)
        // );
        if(this.oldobj!=null){
          var ret2 = this.Noticedata.find((v) => {
                return v.ID == this.oldobj.ID;
          });
          if(!ret2.Partnumber==false&&!ret2.Brand==false&&!ret2.DeliveryCount==false){
            this.InNoticesSorting(ret2)
          }else{
            // this.$Message.warning('请输入分拣数据');
          }
        //  console.log("保存上一行数据" + this.oldobj.Partnumber+"分拣数量："+this.oldobj.DeliveryCount);
        }
       
      } else {
        // console.log("选中同一行");
      }
    },
    //鼠标移除表格后调用的事件
    removeActive() {
      // console.log(this.newobj)
      if(this.newobj!=null){
        if(!this.newobj.Partnumber==false&&!this.newobj.Brand==false&&!this.newobj.DeliveryCount==false){
          if(this.newobj.DeliveryCount>0){
            this.InNoticesSorting(this.newobj)
          }
          }else{
            // this.$Message.warning('请输入分拣数据');
          }
       }
      // console.log(
      //   "鼠标移除表格，提交移除表格前的最后一个修改" +
      //     JSON.stringify(this.newobj)
      // );
      this.oldobj = null;
      this.newobj = null;
      this.currentIndex = null;
    },


  setDeliveryCount(e,index){
    this.Noticedata[index].DeliveryCount=e.target.value
  },

    //			实时获取数据
    realtime() {
      var that = this;
      this.timenmae = setInterval(() => {
        setTimeout(() => {
          if (this.loading == false) {
            
          }
        }, 0);
      }, 2000);
    },
    //	点击新增分拣按钮
    addrow() {
      GetNoticeItemID().then(res=>{
        var obj = {
        IsNew:true,
        Brand: null,
        DateCode:null,
        DeliveryCount: 0,
        Exception: null,
        FileCount: 0,
        FileExist: false,
        ID: res,
        Mpq:1,
        Package:null,
        PackageNumber: 0,
        Partnumber: null,
        Printer: [],
        StoragePackageNumber: 0,
        StorageTotal: 0,
        Summary: null,
        Total:null,
      };
      this.Noticedata.push(obj);
      // this.$forceUpdate();
      this.currentIndex = this.Noticedata.length - 1;
      console.log(this.Noticedata.length)
      this.newobj= this.Noticedata[this.currentIndex]
      })
      
    },
    // 删除新增分拣
    delItemnotice(ID){
        this.$Modal.confirm({
          title: '是否确定删除该分拣？',
          onOk: () => {
             DeleteNoticeItem(ID).then(res=>{
              if(res.success==true){
                this.$Message.success('删除成功');
                this.NoticeItems(this.$route.params.detailID)
              }
            })
          },
          onCancel: () => {
            
          }
      });
    },

    // // 拍照部分
   // 获取通知项中的图片
    PartnumberFiles(id){
      this.clickimg=id
      PartnumberFiles(id).then(res=>{
        this.productlist=res.data
        if(this.productlist.length>0){
          this.showproductfile=true
        }else{
           this.$Message.warning('该产品暂时没有图片');
        }
        
      })
    },
    // 拍照
    FormPhotobtn(row){
      if(row.FileCount>=6){
        this.$Message.warning('该型号照片个数超过6个，每个型号允许拍摄的图片最多为6张');
      }else{
          var data={
            "SiteuserID":"",//网站上传人 先不传
            "AdminID":sessionStorage.getItem("userID"),//上传人
            "Data":{
            "MainID":row.ID,//主要ID
            "Type":1
            } 
          }
        FormPhoto(data)
      }
      
    },
    // 添加到数组中
    changedimg(msg){
      this.NoticeItems(this.$route.params.detailID)
    },
   //删除通知项中的图片
   PhotoFileDelete(ID){
        PhotoFileDelete(ID).then(res=>{
          if(res.success==true){
              this.$Message.success('删除成功');
              this.PartnumberFiles(this.clickimg)
          }
        })
    },
    showproductfilechange(value){
      if(value==false){
        this.NoticeItems(this.$route.params.detailID)
      }
    },
    FilesProcess(url){
      var data={
          Url:url
        }
         FilesProcess(data)
    },



   //获取Noticeitem NoticeItems
   NoticeItems(ID){
     var data={
          ID:ID, //通知ID
       }
     NoticeItems(data).then(res=>{
      //  console.log(data)
       this.Noticedata=res.data
     })
   },
  //打印入库标签
   printInlabel(item){
     console.log(item)
      var configs = GetPrinterDictionary();
      var getsetting = configs["入库标签打印"];
      var str = getsetting.Url;
      var testurl = str.indexOf("http") != -1;
      if (testurl == true) {
        getsetting.Url = getsetting.Url;
      } else {
        getsetting.Url = this.printurl + getsetting.Url;
      }
      var data = {
        setting: getsetting,
        data: [
          {
            waybill: this.infodata,
            listdata:item
          }
        ]
      };
      TemplatePrint(data);
   },

  printitem(Printer){
    if(Printer.length>1){
      this.printitemdata=Printer
      this.printbox=true
    }else{
      this.printInlabel(Printer[0])
    }
  },
   //加载子组件数据
    showNoticeCharges(){
      this.showCharges=true
      this.timer = new Date().getTime()
    },
    ok_NoticeCharges(){
      this.$refs.NoticeCharges.submitbtn()
      console.log(this.$refs.NoticeCharges.istrue)
    },
    cancel_NoticeCharges(){
      this.$refs.NoticeCharges.cancelbtn()
      this.showCharges=false
    },
    fatherMethod(value){
        this.Chargeslistloading=true
        this.showCharges=value
        this.NoticeCharges_list(this.$route.params.detailID)
    },


    // 入库复核
    Review(){
       this.$Modal.confirm({
          title: '是否确认完成复核？',
          onOk: () => {
              var data={
              NoticeID:this.infodata.ID, // 要复核的入库通知ID
              ReviewerID:sessionStorage.getItem("userID")  // 复核人ID
            }
            Review(data).then(res=>{
              if(res.success==true){
                  this.$Message.success('复核成功');
                  this.getData(this.$route.params.detailID);
                  this.NoticeItems(this.$route.params.detailID)
              }else{
                this.$Message.error(res.data);
              }
            })
          },
          onCancel: () => {
              
          }
      });

      
    },

  },
  destroyed() {
    clearInterval(this.timenmae);
    window.removeEventListener('click', this.wsFunc,true)
  },
   beforeDestroy() {
    window.removeEventListener('click', this.wsFunc,true)
  },
};
</script>