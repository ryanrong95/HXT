<style scoped>
.Noticedetail .detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  /* text-indent: 10px; */
}
.Noticedetail .setboxtop {
  margin: 5px 0px;
}
.Noticedetail .infobox {
  display: inline-block;
  width: 100%;
  max-height: 150px;
  min-height: 60px;
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
table tr,
thead {
  border-bottom: 1px solid #e8eaec;
}
table thead {
  border-top: 1px solid #e8eaec;
  background-color: #f8f8f9;
}
table td {
  /* border-left: 1px solid #804040;
  border-top: 1px solid #804040; */
  text-align: center;
}

.inputclass {
  height: 25px;
  width: 80%;
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
}
table thead {
  width: calc(100% - 17px);
}
.rowcols {
  /* display: inline-block; */
  float: left;
  margin-right: 30px;
}
.delicon{
    font-size: 17px;
    color: red;
    position: relative;
    top: -65px;
    left: -12px;
}
.delicon:hover{
  cursor: pointer;
}
</style>
<template>
  <div class="Noticedetail" v-if="infodata!=null">
    <p class="detailtitle" ><span style="margin: 0 10px;">基础信息</span</p>
    <div class="setboxtop infobox">
      <div>
        <div class="rowcols">
          <ul class="infoul">
            <li>
              <span>订单号：</span><span>{{ infodata.FormID }}</span>
            </li>
            <!-- 客户自提显示 -->
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>提货人：</span
              ><span>{{ infodata.Consignee.TakerName }}</span>
            </li>
            <!-- 客户自提显示 -->

            <!-- 送货上门 -->
               <li v-if="infoall.TransportModeDec == '送货上门'">
                <span>联系人：</span><span>{{ infodata.Consignee.Contact }}</span>
               </li>
                <li v-if="infoall.TransportModeDec == '送货上门'">
                  <span>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机：</span
                   ><span>{{ infodata.Consignee.TakerName }}</span>
                </li>
            
            <!-- 送货上门 -->

            <!-- 承运商 -->
            <li v-if="infoall.TransportModeDec == '快递'">
              <span>承运商：</span>
              <span>
                <Select
                  v-model="infodata.Consignee.Carrier"
                  style="width: 200px"
                  :disabled="infodata.Consignee.Carrier == 'DB' ? true : false||submitbtndisable == true ? true : false"
                   @on-change="changeacrrier"
                >
                  <Option
                    v-for="item in Expresarr"
                    :value="item.value"
                    :key="item.value"
                    >{{ item.name }}</Option
                  >
                </Select>
              </span>
            </li>
            <li v-if="infoall.TransportModeDec == '快递'&&infodata.Consignee.Carrier == 'SF'">
              <span>回签单号：</span>
              <span>{{infodata.Consignee.TrackingCode}}  </span>
            </li>
            <li v-if="infoall.TransportModeDec == '快递'&&infodata.Consignee.ExpressPayer==3" style="padding-top: 8px">
              <span>异常备注：</span >
              <span
                ><Input
                  v-model="infodata.Consignee.Summary"
                  placeholder="如有异常，请输入异常备注"
                  style="width: 200px"
              /></span>
              <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled="submitbtndisable == true ? true : false"
                @click="submitall"
                >提交</Button
              >
            </li>
            <!-- 承运商 -->
          </ul>
        </div>
        <div class="rowcols">
          <ul class="infoul">
            <li>
              <span>客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：</span
              ><span>{{ infodata.ClientName }}</span>
            </li>
            <!-- 客户自提显示 -->
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>联系电话：</span
              ><span>{{ infodata.Consignee.TakerPhone }}</span>
            </li>
            <!-- 客户自提显示 -->

            <!-- 送货上门 -->
             <li v-if="infoall.TransportModeDec == '送货上门'">
              <span>联系电话：</span><span>{{ infodata.Consignee.Phone }}</span>
            </li>
            <li v-if="infoall.TransportModeDec == '送货上门'">
                <span>司机电话：</span
              ><span>{{ infodata.Consignee.TakerPhone }}</span>
            </li>
            <!-- 送货上门 -->

            <!-- 承运商 -->
            <li v-if="infoall.TransportModeDec == '快递'">
              <span>运&nbsp;&nbsp;单&nbsp;号：</span
              ><Input
                v-model="infodata.Consignee.WaybillCode"
                :disabled="infodata.Consignee.Carrier != 'DB' ? true : false||submitbtndisable == true ? true : false"
                placeholder="请输入运单号"
                style="width: 180px"
              />
            </li>
            <!-- 承运商 -->
          </ul>
        </div>
        <div class="rowcols">
          <ul class="infoul">
            <li>
              <span>通知类型：</span><span>{{ infoall.NoticeTypeDec }}</span>
            </li>
            <!-- 客户自提显示 -->
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>证件类型：</span
              ><span>{{ infoall.IDTypeDec }}</span>
            </li>
            <!-- 客户自提显示 -->

            <!-- 送货上门 -->
            <li v-if="infoall.TransportModeDec == '送货上门'">
              <span>送货地址：</span
              ><span>{{ infodata.Consignee.Address }}</span>
            </li>
            <li v-if="infoall.TransportModeDec == '送货上门'">
              <!-- <span>司机电话：</span
              ><span>{{ infodata.Consignee.TakerPhone }}</span> -->
               <span>异常备注：</span>
               <span
                  ><Input
                    v-model="infodata.Consignee.Summary"
                    placeholder="如有异常，请输入异常备注"
                    style="width: 180px"
                /></span>
                 <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled="submitbtndisable == true ? true : false"
                @click="submitall"
                >提交</Button
              >
            </li>
            <!-- 送货上门 -->

            <!-- 承运商 -->
            <li v-if="infoall.TransportModeDec == '快递'">
              <span>承运类型：</span
              ><span>
                 <Select
                  v-model="infodata.Consignee.ExpressTransportInt"
                  :disabled="infodata.Consignee.Carrier == 'DB' ? true : false||submitbtndisable == true ? true : false"
                  style="width: 200px">
                  <Option
                    v-for="item in ExpressTransportarr"
                    :value="item.value"
                    :key="item.value"
                    >{{ item.name }}</Option
                  >
                </Select>
              </span>
            </li>
            <!-- 承运商 -->
          </ul>
        </div>
        <div class="rowcols">
          <ul class="infoul">
            <li>
              <span>运输方式：</span><span>{{ infoall.TransportModeDec }}</span>
            </li>
            <!-- 客户自提显示 -->
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>证件号码：</span
              ><span>{{ infodata.Consignee.TakerIDCode }}</span>
            </li>
            <!-- 客户自提显示 -->

            <!-- 送货上门 -->
            <li v-if="infoall.TransportModeDec == '送货上门'">
              <!-- <span>异常备注：</span>
              <span
                ><Input
                  v-model="infodata.Summary"
                  placeholder="如有异常，请输入异常备注"
                  style="width: 180px"
              /></span> -->
            </li>
            <!-- 送货上门 -->

            <!-- 承运商 -->
            <li v-if="infoall.TransportModeDec == '快递'">
              <!-- <span>发货时间：</span
              ><span>{{ infodata.Consignee.TakingTime|showDateexact }}</span> -->
               <span>支付方式：</span
              ><span>
                 <Select
                  v-model="infodata.Consignee.ExpressPayer"
                  :disabled="infodata.Consignee.Carrier == 'DB' ? true : false||submitbtndisable == true ? true : false"
                  style="width: 200px">
                  <Option
                    v-for="item in ExpressPayerarr"
                    :value="item.value"
                    :key="item.value"
                    >{{ item.name }}</Option
                  >
                </Select>
              </span>
            </li>
            <!-- 承运商 -->
          </ul>
        </div>
        <div class="rowcols">
          <ul class="infoul">
            <!-- 客户自提 -->
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>提货时间：</span
              ><span>{{ infodata.Consignee.TakingTime|showDateexact}}</span>
            </li>
            <li v-if="infoall.TransportModeDec == '自提'">
              <span>异常备注：</span>
              <span
                ><Input
                  v-model="infodata.Consignee.Summary"
                  placeholder="如有异常，请输入异常备注"
                  style="width: 180px"
              /></span>
              <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled="submitbtndisable == true ? true : false"
                @click="submitall"
                >提交</Button
              >
            </li>
            <!-- 客户自提 -->

            <!-- 送货上门 -->
            <li v-if="infoall.TransportModeDec == '送货上门'&&infodata.Consignee.TakingTime!=null">
               <span>送货时间：</span
              ><span>{{ infodata.Consignee.TakingTime|showDateexact }}</span>
            </li>
            <!-- 送货上门 -->

            <!-- 承运商 -->
            <li v-if="infoall.TransportModeDec == '快递'">
              <span>收货地址：</span><span>{{infodata.Consignee.Address}}</span>
            </li>
            <li v-if="infoall.TransportModeDec == '快递'&&infodata.Consignee.ExpressPayer!=3">
              <span>异常备注：</span
              ><span
                ><Input
                  v-model="infodata.Consignee.Summary"
                  placeholder="如有异常，请输入异常备注"
                  style="width: 180px"
              /></span>
              <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled="submitbtndisable == true ? true : false"
                @click="submitall"
                >提交</Button
              >
            </li>
            <li v-if="infoall.TransportModeDec == '快递'&&infodata.Consignee.ExpressPayer==3">
              <span>月结账号：</span
              ><span
                ><Input
                 :disabled='submitbtndisable == true ? true : false'
                  v-model="infodata.Consignee.ExpressEscrow"
                  placeholder="请输入月结账号"
                  style="width: 180px"
              /></span>
              <!-- <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled="submitbtndisable == true ? true : false"
                @click="submitall"
                >提交</Button > -->
            </li>
            <!-- 承运商 -->
          </ul>
        </div>
      </div>
    </div>
    <div style="margin-bottom:10px;text-indent:10px" v-if="infoall.Requres.length>0">
      特殊要求：
       <Tag color="volcano" v-for="item in infoall.Requres" :key="item.Name">
         <span v-if="item.Name=='其他要求'">{{item.Contents}}</span>
         <span v-else>{{item.Name}}</span>
       </Tag>
    </div>
    <div>
      <!-- 图片列表  -->
      <PhotoList :childendata="childendata"></PhotoList>
      <!-- 图片列表  -->
    </div>
    <p class="detailtitle">
      <span style="margin: 0 10px;">文件列表</span>
    </p>
    <div style="margin: 10px 5px">
      <Flielist :ID="infodata.ID"></Flielist>
    </div>
    <p class="detailtitle">
      <span style="margin: 0 10px;">费用明细</span>
      <Button
        type="success"
        size="small"
        icon="md-add"
        @click="showNoticeCharges"
        >客户费用录入</Button
      >
    </p>
    <div style="margin: 10px 5px">
      <NoticeChargeslist
        :NoticeChargeslist="NoticeChargeslist"
        :Chargeslistloading="Chargeslistloading"
        @fatherMethod="fatherMethod"
      ></NoticeChargeslist>
    </div>
    <p class="detailtitle">
      <span style="margin: 0 10px;">产品清单</span>
      <Button
        style="margin-left: 10px"
        type="success"
        size="small"
        icon="ios-print-outline"
        @click="peoductlableall"
        >打印全部标签</Button
      >
      <Button
        style="margin-left: 10px"
        type="success"
        size="small"
        icon="ios-print-outline"
        @click="printYCKD"
        >打印预出库单</Button
      >
       <Button
        style="margin-left: 10px"
        type="success"
        size="small"
        icon="ios-print-outline"
        @click="printSHD"
        >打印送货单</Button
      >
      <Checkbox v-if="infodata.Consignee.Carrier=='SF'" v-model="Receiptprint">回单</Checkbox>
      <Button
       v-if="infodata.Consignee.Carrier=='SF'||infodata.Consignee.Carrier=='KY'"
        style="margin-left: 10px"
        type="success"
        size="small"
        icon="ios-print-outline"
        :disabled='DisprintWaybillclick==true?true:false'
        @click="printWaybill"
        >打印运单</Button
      >
      <Button
        style="margin-left: 10px"
        type="success"
        size="small"
        icon="ios-print-outline"
        @click="printall"
        >打印全部</Button
      >
    </p>
    <div style="padding-top: 10px">
      <table cellspacing="" cellpadding="" class="tablebox" width="100%">
        <thead>
          <th>订单ID</th>
          <th>型号</th>
          <th>品牌</th>
          <th>封装</th>
          <th>批次</th>
          <th>Mpq</th>
          <th>数量</th>
          <th>异常</th>
          <th>操作</th>
        </thead>
        <tbody
          style="width: 100%; height: 500px; display: block; overflow-y: scroll"
        >
          <tr
            v-for="(item, index) in NoticeItems"
            :key="index" >
          <!-- :class="{ active: currentIndex === index }"
            v-on:click="changeActive(index, item)" -->
            <!--<td>
                                <input  @change="curChange()" style="margin:0px 8px" type="checkbox" v-model="item._checked" />
                            </td>-->
            <!-- <td class="tdbox">{{index+1}}</td> -->
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.FormID" /> -->
              <p>{{ item.FormID }}</p>
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.Partnumber" /> -->
              <span>{{ item.Partnumber }}</span>
              <Icon type="md-images" @click="PartnumberFiles(item.ID)"/>
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.Brand" /> -->
              <p>{{ item.Brand }}</p>
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.Package" /> -->
              <p>{{ item.Package }}</p>
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.DateCode" /> -->
              <p>{{ item.DateCode }}</p>
            </td>
            <td class="tdbox">
              <p>Mpq:{{ item.Mpq }}</p>
              <!-- <input class="inputclass" type="text" v-model="item.Total" /> -->
              <!-- <p>Mpq:{{ item.Mpq }}&nbsp;/&nbsp;{{ item.PackageNumber }}<span v-if="item.Mpq>1">件</span><span v-else>个</span></p> -->
            </td>
            <td class="tdbox">
              {{ item.PackageNumber }}<span v-if="item.Mpq>1">件</span><span v-else>个</span>
            </td>
            <td class="tdbox">
              {{item.Exception}}
              <!-- <input
                class="inputclass"
                type="text"
                v-model.trim="item.Exception"
                @blur="ExitException(item)"
              /> -->
            </td>
            <td class="tdbox">
              <!-- <input class="inputclass" type="text" v-model="item.Name" @keyup.enter="inputEnter($event)" /> -->
              <div style="margin: 4px 0px">
                <Button type="primary" size="small" icon="ios-print-outline" @click="peoductlable(item)">打印</Button >
                <Button type="primary" size="small" icon="ios-cash-outline"  @click="FormPhotobtn(item)">拍照</Button >
              </div>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!-- 费用录入  开始-->
    <Modal :width="53" v-model="showCharges" title="客户费用录入">
      <NoticeCharges
        :key="timer"
        ref="NoticeCharges"
        :sumbitChargesdata="sumbitChargesdata"
        @fatherMethod="fatherMethod"
      ></NoticeCharges>
      <div slot="footer">
        <Button @click="cancel_NoticeCharges">取消</Button>
        <Button @click="ok_NoticeCharges" type="primary">确定</Button>
      </div>
    </Modal>
    <!-- 费用录入  -->
    <!-- 通知项图片展示 -->
    <Modal :width="53" v-model="showproductfile" title="图片列表">
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
  </div>
</template>
<script>
import { NoticeDetail,NoticeCharges_list, ItemException,Express_Arranged, Print_PreDeliveryFile,GetSzSender,ExpressMethod,FreightPayer} from "../../api/Out";
import { getExpress,PartnumberFiles,PhotoFileDelete} from "../../api/index";

import NoticeCharges from "../Publicview/NoticeCharges";
import NoticeChargeslist from "../Publicview/NoticeChargeslist";
import Flielist from "../Publicview/Filelist";
import PhotoList from "../Publicview/PhotoList";

import { PrintFaceSheet,TemplatePrint,GetPrinterDictionary,FormPhoto,FilesProcess ,PrintDeliveryList,ReprintFaceSheet,PrintPreDeliveryLabel,TrackingCode} from "../../js/browser";
let product_url = require("../../../static/pubilc.dev");
export default {
  components: {
    NoticeCharges,
    NoticeChargeslist,
    Flielist,
    PhotoList,
  },
  name: "tables",
  data() {
    return {
      printWaybillclick:false,//是否可以点击打印
      Receiptprint:false,//是否打印回单
      timer: "",
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
      type: 1,
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

      NoticeChargeslist: [], //费用列表
      Chargeslistloading: true, //费用loading
      infodata: null,
      infoall: null,
      NoticeItems: [],
      showCharges: false, //是否显示费用弹出框,
      sumbitChargesdata: {
        FormID: null,
        NoticeID: null, //是	string	通知ID
        ClientID: null,
        ClientName: null,
      },
      Expresarr: [], //承运商列表
      submitbtndisable: true,
      childendata: {
        //图片组件参数
        ID: null,
        showtype: 0,
        Carrier:null,
      },
      printurl: product_url.pfwms,
      showproductfile:false,
      productlist:[],//产品项图片列表
      clickimg:null,
      ExpressTransportarr:[],//承运类型
      ExpressPayerarr:[],//支付方式
    };
  },
  created() {
    this.$nextTick(function () {
      this.NoticeDetail(this.$route.params.detailID);
      this.NoticeCharges_list(this.$route.params.detailID);
      this.NoticeDetailItemdata(this.$route.params.detailID);
    });
    // this.NoticeDetail(this.$route.params.detailID);
    // this.NoticeCharges_list(this.$route.params.detailID);
    // this.NoticeDetailItemdata(this.$route.params.detailID);
    console.log(this.$route.matched[1].path);
    if (this.$route.matched[1].path == "/OutProcessed") {
      this.submitbtndisable = true;
    } else {
      this.submitbtndisable = false;
    }
    window.keyboardclick = this.keyboardclick;
     window["PhotoUploaded"] = this.changed;
     window['KdPrinted'] =this.changedcode
     window['TrackingCode'] =this.chengeTrackingCode
  },
  computed: {
    DisprintWaybillclick(){
      return this.printWaybillclick
    }
  },
  mounted() {
  },
  methods: {
   setNumber(numbers){
     return Number(numbers)
   },
   //详情基础信息
    NoticeDetail(id) {
      NoticeDetail(id).then((res) => {
        console.log(res);
        this.infodata = res.Notice;
        this.infoall = res;
        this.NoticeItems = res.Items;
        // this.infoall.TransportModeDec = "快递";
        console.log(this.infodata);
        this.sumbitChargesdata.FormID = this.infodata.FormID;
        this.sumbitChargesdata.NoticeID = this.infodata.ID;
        this.sumbitChargesdata.ClientID = this.infodata.ClientID;
        this.sumbitChargesdata.ClientName = this.infodata.ClientName;

        this.childendata.ID = this.infodata.ID;
        this.childendata.Carrier=this.infodata.Consignee.Carrier

        if (this.infoall.TransportModeDec == "快递") {
           this.getExpress();
           this.ExpressMethod(this.infodata.Consignee.Carrier)
           this.FreightPayer()
        }
      }); 
    },
    //详情产品基础信息
    NoticeDetailItemdata(id) {
      NoticeDetail(id).then((res) => {
        this.NoticeItems = res.Items;
      });
    },
    //费用列表
    NoticeCharges_list(id) {
      NoticeCharges_list(id).then((res) => {
        console.log(res);
        this.Chargeslistloading = false;
        this.NoticeChargeslist = res;
      });
    },
    //获取承运商
    getExpress() {
      getExpress().then((res) => {
        this.Expresarr = res.obj;
      });
    },
    // 获取承运类型
    ExpressMethod(name){
      ExpressMethod(name).then(res=>{
        this.ExpressTransportarr=res.obj
      })
    },
    //  修改承运商
    changeacrrier(value){
      this.ExpressMethod(value)
      this.infodata.Consignee.WaybillCode=null
      this.infodata.Consignee.ExpressTransportInt=null
      this.infodata.Consignee.ExpressPayer=null
      this.childendata.Carrier=value
      this.Receiptprint=false
      this.infodata.Consignee.TrackingCode=null
      this.printWaybillclick=false
      console.log(this.childendata)
    },
    FreightPayer(){
      FreightPayer().then(res=>{
        this.ExpressPayerarr=res.obj
      })
    },
    //产品项异常录入
    ExitException(item) {
      if (!item.Exception != true) {
        console.log(item.Exception);
        var data = {
          ID: item.ID,
          Exception: item.Exception,
        };
        ItemException(data).then((res) => {
          if (res.Success == true) {
            this.NoticeDetailItemdata(this.$route.params.detailID);
          }
        });
      }
    }, 
    // 提交承运商等信息
    submitall() {
      var data = {
        ID: this.infodata.Consignee.ID,
        Carrier: this.infodata.Consignee.Carrier,
        ExpressEscrow:this.infodata.Consignee.ExpressEscrow,
        WaybillCode: this.infodata.Consignee.WaybillCode,
        FreightPayer:this.infodata.Consignee.ExpressPayer,
        ExpressTransport:this.infodata.Consignee.ExpressTransportInt,
        Summary: this.infodata.Consignee.Summary,
        TrackingCode:this.infodata.Consignee.TrackingCode
      };
      Express_Arranged(data).then((res) => {
        if (res.Success == true) {
          this.$Message["success"]({
            background:true,
            content: "提交成功",
          });
          this.NoticeDetail(this.$route.params.detailID);
        } else {
          this.$Message["error"]({
            background:true,
            content: res.Data,
          });
        }
        this.printWaybillclick=false
      });
    },
    // 获取通知项中的图片
    PartnumberFiles(id){
      this.clickimg=id
      PartnumberFiles(id).then(res=>{
        this.productlist=res.data
        if(this.productlist.length>0){
          this.showproductfile=true
        }else{
          //  this.$Message.warning('该产品暂时没有图片');
           this.$Message["warning"]({
            background:true,
            content: "该产品暂时没有图片",
          });
        }
        
      })
    },
    // 拍照
    FormPhotobtn(row){
      var data={
          "SiteuserID":"",//网站上传人 先不传
          "AdminID":sessionStorage.getItem("userID"),//上传人
          "Data":{
          "MainID":row.ID,//主要ID
          "Type":1
          } 
        }
      FormPhoto(data)
    },
   //删除通知项中的图片
   PhotoFileDelete(ID){
        PhotoFileDelete(ID).then(res=>{
          if(res.success==true){
              // this.$Message.success('删除成功');
              this.$Message["success"]({
                background:true,
                content: "删除成功",
              });
              this.PartnumberFiles(this.clickimg)
          }
        })
    },
    // 图片预览
    FilesProcess(url){
      var data={
        Url:url
      }
      FilesProcess(data)
    },
    // 打印部分
    // 1 打印运单
    printWaybill() {
      if (!this.infodata.Consignee.WaybillCode==true) {
        if(!this.infodata.Consignee.ExpressPayer!=true&&!this.infodata.Consignee.ExpressTransportInt!=true){
           this.printWaybillclick=true
            this.$Message.info({
                content: '打印中，请稍等！请勿连续点击打印！',
                duration: 2
            });
           GetSzSender().then(res=>{
              var data = {
              OrderID:this.infodata.FormID, //必填
              ExPayType: this.infodata.Consignee.ExpressPayer, //必填支付类型
              ShipperCode: this.infodata.Consignee.Carrier, //必填 承运商编号
              ExpType: this.infodata.Consignee.ExpressTransportInt, //必填快递类型
              MonthlyCard:this.infodata.Consignee.ExpressEscrow,
              IsSignBack:this.Receiptprint,//是否打印回单
              Sender: {
                //发件人，必填
                Company: res.Company,
                Contact:res.Contact, //必填
                Mobile: res.Mobile,
                Tel:res.Tel,
                Province: res.Province,
                City: res.City,
                Region:res.Region,
                Address: res.Address, //跨越必填
              },
              Receiver: {
                //收件人
                Contact: this.infodata.Consignee.Contact, //必填姓名
                Mobile: this.infodata.Consignee.Phone,//手机
                Address:this.infodata.Consignee.Address, //跨越必填详细地址
              }, 
              Commodity: [
                //顺丰必填
                {
                  GoodsName: "客户器件", //必填
                },
              ],
            };
          this.printWaybillclick=true
           PrintFaceSheet(data);
        })  
        }else{
         this.$Message["warning"]({
            background:true,
            content: "请选择承运类型和支付方式",
          });
        }
      }else{
         this.$Message.info({
                content: '打印中，请稍等！请勿连续点击打印！',
                duration: 2
            });
        var data={
          Code:this.infodata.Consignee.WaybillCode
        }
        ReprintFaceSheet(data)
        if(this.Receiptprint==true&&!this.infodata.Consignee.TrackingCode==false){
           var data={
            Code:this.infodata.Consignee.TrackingCode
          }
          ReprintFaceSheet(data)
        }
      } 
    },
    //调用打印运单号，返回运单号的值
    changedcode(message) {
      var msg=JSON.parse(message) 
      this.infodata.Consignee.WaybillCode =msg.WaybillCode
      if(!msg.TrackingCode==false){
        this.infodata.Consignee.TrackingCode=msg.TrackingCode
      }
      this.submitall()
    },
    // 2 出库标签打印
    peoductlable(row){
      var configs = GetPrinterDictionary();
      var getsetting = configs["出库标签打印"];
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
            listdata:row
          }
        ]
      };
      if(row.Mpq>1){
         for(var i=0;i<row.PackageNumber;i++){
          TemplatePrint(data);
        }
      }else{
           TemplatePrint(data);
       }
    },
    // 3 打印全部出库标签
    peoductlableall(){
      for(var i=0,len=this.NoticeItems.length;i<len;i++){
        this.peoductlable(this.NoticeItems[i])
      }
    },
    // 4 打印预出库单
    printYCKD(){
      var data = {
            waybill: this.infoall,
            listdata:this.NoticeItems
          }
      PrintPreDeliveryLabel(data)
      Print_PreDeliveryFile(this.infodata.ID).then(res=>{
        // alert(JSON.stringify(res))
      })
    },
    // 5 打印送货单
    printSHD(){
      var data = {
        waybillinfo: this.infoall,
        listdata:this.NoticeItems,
        Numcopies:2
      }
      PrintDeliveryList(data)
    },
    // 6 打印全部标签
    printall(){
      this.printYCKD()
      this.peoductlableall()
      this.printSHD()
      if(this.infodata.Consignee.Carrier=='SF'||this.infodata.Consignee.Carrier=='KY'){
        this.printWaybill()
      }
    },

    //加载子组件数据
    showNoticeCharges() {
      this.showCharges = true;
      this.timer = new Date().getTime();
      // this.$refs.NoticeCharges.sumbitdata=this.sumbitChargesdata
    },
    ok_NoticeCharges() {
      this.$refs.NoticeCharges.submitbtn();
      console.log(this.$refs.NoticeCharges.istrue);
    },
    cancel_NoticeCharges() {
      this.$refs.NoticeCharges.cancelbtn();
      this.showCharges = false;
    },
    fatherMethod(value) {
      this.showCharges = value;
      this.Chargeslistloading = true;
      this.NoticeCharges_list(this.$route.params.detailID);
    },
  },
  destroyed() {
    clearInterval(this.timenmae);
    this.documentclick = null;
  },
};
</script>