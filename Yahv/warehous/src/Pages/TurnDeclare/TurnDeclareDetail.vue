<style scoped>
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.detail_li .itemli {
  line-height: 40px;
  font-size: 14px;
}
.ivu-row {
  padding: 10px;
}

.detail_li .demo-badge-alone {
  height: 18px !important;
  line-height: 16px !important;
  padding: 0 4px !important;
  font-size: 12px !important;
}
.detail_tablebox {
  width: 100%;
  height: auto;
}
.setupload {
  /* width: 50px; */
  height: 30px;
  border: none;
  float: left;
  line-height: 1;
  margin-right: 5px;
}
.setupload .ivu-btn {
  padding: 2px 2px 2px;
  font-size: 12px;
}
.setupload .ivu-upload .ivu-upload-drag {
  border: 0px !important;
}
.successbtn {
  text-align: center;
}
.detail_title1 {
  display: inline-block;
  width: 95px;
  /* font-weight: bold; */
}
.detail_title2 {
  display: inline-block;
  width: 78px;
}
.detail_title3 {
  display: inline-block;
  width: 100px;
}
.detail_tablebox >>> .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: normal;
  word-break: break-all;
  box-sizing: border-box;
}
.ivu-table .demo-table-info-row td {
  display: none;
}
.Mustfill{
  color: red;
}
.linkurlcolor{
  color: #2d8cf0;
}
.Filesbox:hover{
  cursor: pointer;
}
.sync_btn{
    font-size: 30px;
    vertical-align: middle;
    padding-right: 10px;
}
.hoverbtn:hover{
 cursor: pointer;
}
</style>
<template>
  <div>
    <div style="width:100%;">
      <div class="itembox">
        <p class="detailtitle">基础信息</p>
        <!-- <p>{{getids}}</p> -->
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row v-if="waybillinfo!=null">
            <Col style="width: 20%;float: left;">
            <ul class="detail_li detail1">
              <li class="itemli">
                <span class="detail_title1">ID：</span>
                <span>{{waybillinfo.ID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;态：</span>
                <span>
                  {{waybillinfo.ExcuteStatusName}}
                  <Button v-if="waybillinfo.ID!=null" icon='md-calendar' type="success" size='small' @click="logchange">日志管理</Button>
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title1">业务类型：</span>
                <span>{{waybillinfo.SourceName}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">客服人员：</span>
                <span>{{waybillinfo.Merchandiser}}</span>
              </li>
              <!-- <li class="itemli">
              <span class="detail_title1">总&nbsp;&nbsp;货&nbsp;&nbsp;值：</span>
              <span style="color:'#ccc';font-width:600">{{waybillinfo.chgTotalCurrency}} {{waybillinfo.chgTotalPrice}}</span>
            </li> -->
              <!-- <li class="itemli">
              <span class="detail_title1">是否逾期：</span>
              <span style="color:'#ccc';font-width:600" v-if="waybillinfo.overDue==true">是</span>
              <span style="color:'#ccc';font-width:600" v-else>否</span>
            </li> -->
              <li class="itemli" v-if="waybillinfo.Condition!=undefined">
                <Icon type="md-alert" style="font-size: 22px;color: #da2828;" />
                <Tag color="geekblue" v-if="waybillinfo.Condition.AgencyCheck==true">代检查</Tag>
                <Tag color="purple" v-if="waybillinfo.Condition.AgencyPayment==true">代垫货款</Tag>
                <Tag color="blue" v-if="waybillinfo.Condition.AgencyReceive==true">代收货款</Tag>
                <Tag color="cyan" v-if="waybillinfo.Condition.ChangePackaging==true">代收货款</Tag>
                <Tag color="green" v-if="waybillinfo.Condition.LableServices==true">标签服务</Tag>
                <Tag color="gold" v-if="waybillinfo.Condition.PayForFreight==true">垫付运费</Tag>
                <Tag color="orange" v-if="waybillinfo.Condition.Repackaging==true">重新包装</Tag>
                <Tag color="volcano" v-if="waybillinfo.Condition.UnBoxed==true">拆箱验货</Tag>
                <Tag color="red" v-if="waybillinfo.Condition.VacuumPackaging==true">真空包装</Tag>
                <Tag color="magenta" v-if="waybillinfo.Condition.WaterproofPackaging==true">防水包装</Tag>
                <Tag color="lime" v-if="waybillinfo.Condition.QualityInspection==true">质检</Tag>
                <Tag color="yellow" v-if="waybillinfo.Condition.Unboxing==true">拆包装</Tag>
              </li>
            </ul>
            </Col>
            <Col style="width: 23%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title2">订&nbsp;&nbsp;单&nbsp;&nbsp;号：</span>
                <span>{{waybillinfo.OrderID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">制单时间：</span>
                <span>{{waybillinfo.CreateDate|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">总&nbsp;&nbsp;额&nbsp;&nbsp;度：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.total}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">总&nbsp;&nbsp;欠&nbsp;&nbsp;款：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.totalDebt}}</span>
              </li>
            </ul>
            </Col>
            <Col style="width: 30%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title3">客&nbsp;&nbsp;户&nbsp;&nbsp;编&nbsp;&nbsp;号：</span>
                <span>
                  {{waybillinfo.EnterCode}}（{{waybillinfo.ClientName}}）<Tag color="geekblue">LV {{clientGrade}}</Tag>
                  <i style=" background: #f90; color: #ffffff; padding: 6px; border-radius: 50%; font-size: 17px;" v-if="waybillinfo.IsClientLs==true">租</i>
               </span>
              </li>
              <li class="itemli">
                <span class="detail_title3">总&nbsp;&nbsp;货&nbsp;&nbsp;值：</span>
                <span style="color:'#ccc';font-width:600">{{waybillinfo.chgTotalCurrency}} {{waybillinfo.chgTotalPrice}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title3">是否逾期：</span>
                <span style="color:'#ccc';font-width:600" v-if="waybillinfo.overDue==true">是</span>
                <span style="color:'#ccc';font-width:600" v-else>否</span>
              </li>
            </ul>
            </Col>
            <Col style="width: 20%;float: left;">
            <ul class="detail_li" style="margin-left:20px;">
              <li class="itemli" v-if="waybillinfo.ExcuteStatus!=215">
                <span style="float:left;line-height: 27px;">发货情况拍照：</span>
                <div class="setupload">
                  <Button type="primary"
                          icon="ios-cloud-upload"
                          @click="SeletUpload('Waybill')">
                    传照
                  </Button>
                </div>
                <div class="setupload">
                  <Button type="primary"
                          icon="md-reverse-camera"
                          @click="fromphotos('Waybill')">
                    拍照
                  </Button>
                  <!-- <Button type="primary" icon="ios-search" @click="photoing('waybill')">拍照</Button> -->
                </div>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2&&waybillinfo.ExcuteStatus==215||waybillinfo.Type==1&&waybillinfo.ExcuteStatus==215" style="padding-top:10px;clear: both;">
                <span style="float:left;line-height: 27px;">送&nbsp;货&nbsp;单&nbsp;拍&nbsp;照：</span>
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload('Clientimg')">
                    传照
                  </Button>
                </div>
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="md-reverse-camera"
                          @click="fromphotos('Clientimg')">
                    拍照
                  </Button>
                </div>
              </li>
              <li style="clear: both;">
                <p>发货情况照片：</p>
                <div v-for="(item,index) in waybillinfo.FeliverGoodFile">
                  <div v-if="item.Type==8010" class="Filesbox">
                    <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                    <!-- <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon> -->
                    <Tooltip content="删除" placement="top">
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')" />
                    </Tooltip>
                    <Tooltip content="打印" placement="top">
                      <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                    </Tooltip>
                  </div>
                </div>
                <p v-if="waybillinfo.WaybillType==2&&waybillinfo.ExcuteStatus==215||waybillinfo.WaybillType==1&&waybillinfo.ExcuteStatus==215">送货单照片：</p>
                <div v-for="(item,index) in waybillinfo.SendGoodsFile">
                  <div v-if="item.Type==8020" class="Filesbox">
                    <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                    <Tooltip content="删除" placement="top">
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Clientimg')" />
                    </Tooltip>
                    <Tooltip content="打印" placement="top">
                      <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                    </Tooltip>
                  </div>
                </div>
              </li>
            </ul>
            </Col>
          </Row>
        </div>
      </div>
      <div class="itembox">
        <p class="detailtitle">产品清单</p>
        <div style="margin:15px 0">
          <ButtonGroup style="width:28%">
            <Input v-model.trim="searchkey"
                   placeholder="请输入品牌或型号"
                   clearable
                   @on-clear='search_pro'
                   style="width:80%;float:left;position: relative;left: 3px" />
            <Button style="float:left" @click="search_pro" type="primary">筛选</Button>
          </ButtonGroup>
          <Button type="primary" @click="clickshowchangebox" :disabled='waybillinfo.ExcuteStatus==215?true:false'>一键入箱</Button>
          <Button type="primary" @click="showBudget('meet','in',waybillinfo.Source)">收入</Button>
          <Button type="primary" @click="showBudget('meet','out',waybillinfo.Source)">支出</Button>
          <Button type="primary" @click="showBudget2" v-if="showStoragecharge==true">仓储费录入</Button>
          <span v-if="showStoragecharge==true" style="font-size:11px;color:red">（请收取仓储费，总共需收取仓储费的时长：{{timedifference}}天）</span>
          <div style="float:right">
            <Icon type="md-sync" @click="sync_btn" class="sync_btn hoverbtn" />
            <Button type="primary"
                    shape="circle"
                    icon="md-checkmark"
                    @click="clickshowchangebox"
                    :disabled="isdisabled==true?true:false">
              完成分拣
            </Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table ref="selection"
                   :columns="tabletitle"
                   :data="detailitem"
                   :loading=loading
                   @on-selection-change="handleSelectRow">
              <template slot-scope="{ row, index }" slot="PartNumber">
                <div style="display: flex;justify-content: space-between;align-items: center;">
                  <div style="width:120px;overflow: hidden;">
                    <span>{{row.Product.PartNumber}}</span>
                  </div>
                  <ul style="float:right;" v-if="row.Conditions!=null">
                    <li><Tag v-if="row.Conditions.IsCIQ==true" color="primary">商检</Tag></li>
                    <li><Tag v-if="row.Conditions.IsCCC==true" color="warning">CCC</Tag></li>
                    <li><Tag v-if="row.Conditions.IsEmbargo==true" color="error">禁运</Tag></li>
                    <li><Tag v-if="row.Conditions.IsHighPrice==true" color="magenta">高价值</Tag></li>
                  </ul>
                </div>
              </template>
              <template slot-scope="{row,index}" slot="TinyOrderID">
                <p>{{row.TinyOrderID}}</p>
              </template>
              <template slot-scope="{row,index}" slot="Boxing_code">
                <span v-if='warehouseID.indexOf("SZ")!=-1'>{{row.BoxCode}}</span>
                <div style="display:inline-block;" v-else>
                  <Input v-model="row.BoxCode" :disabled='true' />
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="item in row.Imagefiles" class="Filesbox">
                  <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <Icon type="ios-trash-outline"
                        :ref="row.ID"
                        @click.native="handleRemove(item,row)"></Icon>
                </p>
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload(row)"
                          :disabled='row._disabled==true?true:false'>
                    传照
                  </Button>
                </div>
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="md-reverse-camera"
                          @click="fromphotos(row)"
                          :disabled='row._disabled==true?true:false'>
                    拍照
                  </Button>
                </div>
              </template>
            </Table>
            <div class="successbtn" style="margin-top:20px;">
              <Button type="primary"
                      icon="md-checkmark"
                      @click="clickshowchangebox"
                      :disabled="isdisabled==true?true:false">
                完成分拣
              </Button>
              <Button v-if='warehouseID.indexOf("HK")==-1'
                      type="warning"
                      icon="ios-alert-outline"
                      @click="isAbnormal=true"
                      :disabled="isdisabled==true?true:false">
                分拣异常
              </Button>
            </div>
          </div>
          <div>
            <p style="margin-bottom:20px" class="detailtitle">封箱</p>
            <Sealing-html v-if="reFresh==true" @hietorgetDetail='hietorgetDetail' v-bind:OrderID="waybillinfo.OrderID" Type="2" ref='reFresh' v-bind:EnterCode='waybillinfo.EnterCode' v-bind:WaybillID='waybillinfo.ID'></Sealing-html>
          </div>
        </div>
      </div>
    </div>
    <!-- 收支明细 开始 -->
    <Modal v-model="Budgetdetail"
           width="55%"
           :closable="false"
           :mask-closable="false"
           :footer-hide="true">
      <div style="position: absolute;right:20px;z-index:99999;width:30px">
        <Icon type="ios-close" style="float:right;font-size:30px;font-weight:bold;" @click="closeBudget" />
      </div>
      <div>
        <router-view></router-view>
      </div>
    </Modal>
    <!-- 收支明细结束 -->
    <Modal v-model="showchangebox"
           title="一键入箱"
           :mask-closable='false'
           @on-visible-change="changeshowbox">
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;期：
        </label>
        <DatePicker type="date" style="width:80%" :options="options3" placeholder="请选择生成箱号的时间" :clearable='false' :value="saleDate" @on-change='changeData'></DatePicker>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;号：
        </label>
        <Input v-model.trim="newboxcode"
               maxlength="30"
               placeholder="请输入临时箱号"
               style="width:80%"
               @on-blur='handleCreate1(newboxcode,$event)' />
      </p>
      <p style="padding-top:10px;" v-if="issharevalue==true">
        <label> <em class="Mustfill">*</em>重量(Kg)： </label>
        <Input v-model="sharevalue" placeholder="请输入重量" @on-blur='TestAVGWeightsum' @on-enter='TestAVGWeightsum' style="width:80%" />
      </p>
      <p style="padding-top:10px;">
        <label> <em class="Mustfill">&nbsp;</em>推算重量(Kg)： </label>
        <span>{{AVGWeightsum}}</span>
      </p>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok_changebox">确定</Button>
      </div>
    </Modal>
    <!-- 一键入箱 分摊重量合并 结束 -->
    <!-- 确认出库 开始 -->
    <div v-if="WarehousingMsg==true">
      <Modal v-model="WarehousingMsg"
             title="确定分拣"
             @on-ok="ok_Warehousing"
             @on-cancel="cancel_Warehousing">
        <div>
          <span>是否确认分拣</span>
        </div>
      </Modal>
    </div>
    <!-- 确认出库 结束 -->
    <!-- 一键打印弹出 开始 -->
    <Modal v-model="showprint"
           title="打印"
           @on-ok="CgAllprint"
           @on-cancel="cancel">
      <CheckboxGroup v-model="fruit">
        <Checkbox label="出库单打印"></Checkbox>
        <Checkbox label="送货单打印"></Checkbox>
      </CheckboxGroup>
    </Modal>
    <!-- 一键打印弹出 结束 -->
    <!-- 箱签打印 开始 -->
    <Modal title="箱签打印"
           width='80'
           v-model="showprintboxcode"
           :mask-closable="false">
      <Print-boxcode ref="printbox"></Print-boxcode>
    </Modal>
    <!-- 箱签打印 结束 -->
    <!-- 操作日志 -->
    <Modal v-model="showlogged"
           :footer-hide='true'
           :mask-closable='false'
           width='60'>
      <div slot="close">
        <Icon style="font-size:21px;color:#cccccc;padding-top: 5px" type="ios-close-circle-outline" />
      </div>
      <div slot="header">
        <span style="font-size:18px;color:#1aaff7;">日志管理</span>
      </div>
      <logg-ed ref="logged" :key='loggdetime' v-if="showlogged" :WaybillID='waybillinfo.ID'></logg-ed>
    </Modal>
    <!-- 操作日志 -->
    <!--仓储费录入 开始-->
    <Modal title="仓储费录入"
           v-model="storagecharge"
           :mask-closable="false"
           :footer-hide="true"
           @on-visible-change="changestoragecharge"
           :width='60'>
      <p slot="header">
        仓储费录入
        <span style="font-size:14px;padding-left:10px">(总共需收取仓储费的时长：{{timedifference}}天)</span>
      </p>
      <Storagecharge ref='Storagecharge' :OrderID="waybillinfo.OrderID" :timedifference="timedifference" @Parentfun="Parentfun"></Storagecharge>
    </Modal>
    <!--仓储费录入 结束-->
  </div>
</template>
<script>
// import imgtest from "@/Pages/Common/imgtes"
import {
  pickingsdetail,
  GetInputID,
  GetBoxesdates,
  Boxenter,
  pickingsenter,
  GetBoxes
} from "../../api";
  import { cgCustomsStorageDetail, CgPickingsSearch, CgBoxesShow, CgpickingsErrorBtn, cgCustomsStorageTurnDeclare, CgDeleteFiles, BoxcodeEnter, CgEnterSeries, Getclientdata, BoxcodeDelete, IsRecordWarehouseFee} from '../../api/CgApi'
import moment from "moment";
let lodash = require("lodash");
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint,
  FormPhoto,
  SeletUploadFile,
  PrintDeliveryList,
  PrintOuptNotice,
  FilesProcess
} from "@/js/browser.js";
import Printboxcode from "@/Pages/Common/PrintBoxcode";
import logged from '../Common/logged';
  import Sealing from "./Sealing"
  import Storagecharge from '@/Pages/Expenses/Storagecharge'
export default {
  name: "",
  components: {
    'Print-boxcode':Printboxcode,
    'logg-ed':logged,
    "Sealing-html": Sealing,
    'Storagecharge': Storagecharge
  },
  data() {
    return {
      showStoragecharge: false,//是否收取入仓费
      timedifference: null,//时差
      storagecharge: false,//仓储费录入
      options3: {
          disabledDate (date) {
              return date && date.valueOf() < Date.now() - 86400000;
          }
      },
      reFresh:false,
      saleDate:'',//箱号时间
      loggdetime:'',//操作日志时间
      showlogged:false,//操作日志
      isclick:false,//判断时候可以分配箱号入库
      AVGWeightsum:null,//推算重量
      issharevalue:true,//是否可以输入分摊重量
      sharevalue:null,//分摊的总重量
      boxcodetype:'1',//
      clientGrade:null,//客户等级
      isdisabled:false,//是否禁止选中
      originalarr:[],//原始数据
      TypeArr:[
        {
          value:3,
          label:"快递"
        },
        {
          value:4,
          label:"国际快递"
        },
      ],
      showprintboxcode:false,//打印箱号的展示状态
      fruit: [],//打印选中
      showprint:false,//显示一件打印
      loading:true,
      pusharr:[],
      SelectRow: [], //多选数组
      searchkey: "",
      boxingarr: [ ], //箱号
      printlist: [], //清单打印数据
      wayBillID: this.$route.params.detailID,
      detail_ID: "",
      showphoto: false, //显示拍照弹出框
      CarrierList: [], //承运商列表
      Conveyingplace: [], //输送地列表
      waybillinfo: {},//运单信息
      detailitem:[],//通知信息
      details: {
        //详情页
        wayBillID: "",
        total: 0,
        pageIndex: 1,
        pageSize: 1,
        WaybillNo: "90416165067", //运单号(本次)
        Carrier: "", //承运商(本次)
        PlaceDescription: "美国" //输送地,
      },
      tabletitle: [
        {
          type: 'selection',
          width: 60,
          align: 'center',
          fixed: 'left'
        },
         {
            type: 'index',
            width: 60,
            align: 'center'
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center",
          minWidth: 180
        },
        {
          title: "品牌",
          key: "brand",
          align: "center",
          width:100,
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Product.Manufacturer);
          }
        },
        {
          title: "产地",
          key: "OriginName",
          width:100,
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.OriginName);
          }
        },
        {
          title: "应出数量",
          key: "Quantity",
          align: "center",
          width:80,
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Quantity);
          }
        },
        {
          title: "已出数量",
          key: "PickedQuantity",
          align: "center",
          width:80,
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.PickedQuantity);
          }
        },
        {
          title: "剩余数量",
          key: "LeftQuantity",
          align: "center",
          width:80,
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.LeftQuantity);
          }
        },
        {
          title: "本次发货数量",
          key: "practical",
          align: "center",
          width:90,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.CurrentQuantity,
                disabled:true
              },
              on: {
                 "on-change"(event) {
                  console.log(event.target.value);
                  if (event.target.value != "") {
                    var reg = /^[0-9]*$/;
                    // var reg = /^\d+(\.\d{0,2})?$/;
                    if (reg.test(event.target.value) == true&&event.target.value!=0) {
                      params.row.CurrentQuantity = Number(event.target.value);
                      vm.detailitem[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入正整数");
                      (event.target.value = null),
                        (params.row.CurrentQuantity =null);
                      vm.detailitem[params.index] = params.row;
                    }
                    vm.clicktest(params.row);
                  }else{
                    event.target.value = null,
                    params.row.CurrentQuantity =null;
                    vm.detailitem[params.index] = params.row;
                    vm.clicktest(params.row);
                  }
                },
                // "on-blur"(){
                //   vm.clicktest(params.row);
                //   vm.changeoriginalarr(params.row)
                // }
              }
            });
          }
        },
        {
          title: "小订单号",
          slot: "TinyOrderID",
          align: "center",
          minWidth: 150
        },
        // {
        //   renderHeader: (h, params) => {
        //       return h("div", [
        //         h(
        //           "span",
        //           {
        //             style: {
        //               color: "red",
        //               "vertical-align": "middle",
        //               "padding-right": "5px",
        //               "font-size": "20px;"
        //             }
        //           },
        //           "*"
        //         ),
        //         h("span", {}, "箱号")
        //       ]);
        //     },
        //   slot: "Boxing_code",
        //   align: "center",
        //   width:220
        // },
        {
          renderHeader: (h, params) => {
              return h("div", [
                h(
                  "span",
                  {
                    style: {
                      color: "red",
                      "vertical-align": "middle",
                      "padding-right": "5px",
                      "font-size": "20px;"
                    }
                  },
                  "*"
                ),
                h("span", {}, "毛重(Kg)")
              ]);
            },
          key: "GrossWeight",
          align: "center",
          width:130,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Weight
              },
              on: {
                "on-blur"(event) {
                  // var reg = /^\d+(\.\d{0,4})?$/;
                  // var reg = /^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/
                  var reg = /^\d+(\.\d{0,5})?$/;
                  // var newtarget = vm.trim(event.target.value);
                  if (event.target.value != "") {
                    if (reg.test(event.target.value) == true&&event.target.value!=0) {
                      params.row.Weight = event.target.value;
                      vm.detailitem[params.index] = params.row;
                    } else {
                      vm.$Message.error("请输入大于零的数字,保留小数点后五位");
                      params.row.Weight =null;
                      event.target.value = null;
                      vm.detailitem[params.index] = params.row;
                    }
                  }else{
                       vm.$Message.error("重量不能为空");
                      params.row.Weight =null;
                      event.target.value = null;
                  }
                  if(vm.SelectRow.length>0){
                     for(var i=0,len=vm.SelectRow.length;i<len;i++){
                      if(vm.SelectRow[i].SortingID==params.row.SortingID){
                        vm.SelectRow[i]=params.row
                      }
                    }
                   }
                  // vm.clicktest(params.row);
                  vm.changeoriginalarr(params.row)
                },
              }
            });
          }
        },
        {
          title: "库位",
          key: "ShelveID",
          align: "center",
          width:120,
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.ShelveID);
          }
        },
        {
          title: "图片",
          slot: "imglist",
          align: "center",
          width: 200
        },
        {
          title: "操作",
          slot: "operation",
          align: "center",
          width: 150,//220
          fixed: 'right',
        }
      ],
      tablelist: [],
      uploadList: [],
      files: "",
      Conveyingplace: [], //输送地列表
      Conveyingplace2: [],
      chengevalue: {
        inputval: "",
        value: "",
        type: ""
      },
      setClientCode: false, //显示输送地选择模态框
      ClientCode: "", //默认输送地
      showchangebox: false, //显示一键更改箱号的的弹窗
      newboxcode: "", // 一键更改箱号后的新箱号
      newboxcodeback:'',
      oldboxcode:null,//保存旧箱号，提供释放的功能
      applydata: [],
      WarehousingMsg: false, //完成入库的提示 
      isAbnormal: false, //是否异常到货
      Summary:null, //后台提供的备注信息对象
      CarArr: [], //车票号
      DriversArr: [], // 司机
     warehouseID: sessionStorage.getItem("UserWareHouse"), //当前库房ID
    };
  },
  filters: {
    showDate: function(val) {
      //时间格式转换
      // console.log(val)
      if (val != "") {
        if (val || "") {
          var b = val.split("(")[1];
          var c = b.split(")")[0];
          var result = moment(+c).format("YYYY-MM-DD");
          return result;
        }
      }
    }
  },
  created() {
    window["PhotoUploaded"] = this.changed;
    this.getdetail_data();
  },
  computed: {
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail;
    }
  },
  mounted() {
   
  },
  methods: {
    setboxsplit(str) {
      //去除前后空格
      if(str){
         return str.split("]")[1]
      }
    },
    clicktest: lodash.throttle(function(paramsrow) {  //同步修改选中的数据
      for(var i=0,lens=this.SelectRow.length;i<lens;i++){
          if(paramsrow.ID==this.SelectRow[i].ID){
            this.SelectRow[i]=paramsrow
          }
      }
    }, 1000),
    //修改原始数据
    changeoriginalarr(row){
      for(var i=0,lens=this.originalarr.length;i<lens;i++){
          if(this.originalarr[i].ID==row.ID){
            this.originalarr[i]=row
          }
      }
    },
    showBudget(type, Budget,Source) {
      //收支明细展开
      this.$store.dispatch("setBudget", true);
      if (type == "meet") {
        var namemeet = "";
        this.$router.push({
          name: 'TurnDeclareBudgetIncome',
          query: {
            webillID: this.waybillinfo.ID,
            OrderID:this.waybillinfo.OrderID,
            type: Budget,
            otype:"out",
            conduct:Source
          }
        });
      } else {

      }
    },
    closeBudget() {
      //收支明细关闭
      console.log("关闭收支明细");
      this.$router.go(-1);
      this.$store.dispatch("setBudget", false);
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      console.log(value);
      this.SelectRow = value;
    },
    ok() {
      this.showphoto = false;
    },
    cancel() {
      this.showphoto = false;
      this.showchangebox=false
      this.oldboxcode=null
      if( this.showchangebox==false){
        this.BoxcodeDelete()
      } 
    },
    getdetail_data() {  //获取详情页数据
      cgCustomsStorageDetail(this.wayBillID).then(res => {
        this.Getclientdata(res.Waybill.EnterCode)

        var newdata = null //最近处理订单的时间，用与计算仓储天数
        if(res.Waybill.ExcuteStatus==215){
          this.isdisabled = true
          newdata = this.moment(res.Waybill.ModifyDate).format('YYYY-MM-DD')
        }else{
          this.isdisabled = false
          newdata = this.moment(new Date().getTime()).format('YYYY-MM-DD')
        }
        this.waybillinfo = res.Waybill
        for(var i=0,len=res.Notices.length;i<len;i++){
          if(res.Notices[i].CurrentQuantity<=0){
            res.Notices[i]._disabled=true
          }else{
            res.Notices[i]._disabled=false
          }
        }
        this.detailitem = res.Notices
        this.originalarr=res.Notices
      //  this.getboxarr()
        this.loading=false;
        this.fruit = ['出库单打印', '送货单打印']

        if (res.Waybill.FirstTempDate != null) {
          var olddata = this.moment(res.Waybill.FirstTempDate).format('YYYY-MM-DD')
          var LsEndDate = Date.parse(this.moment(res.Waybill.LsEndDate).format('YYYY-MM-DD'))//租赁截至时间
          var FirstTempDate = Date.parse(olddata); //入库时间
          var ModifyDate = Date.parse(newdata);//出库时间
          if (res.Waybill.LsEndDate) {
            this.waybillinfo.IsClientLs = false
            if (FirstTempDate > LsEndDate) {
              var duration = ((ModifyDate - FirstTempDate) / (1 * 24 * 60 * 60 * 1000)) + 1;
              console.log(duration + "租赁过期减五天")
              if (duration > 5) { //暂存超过五天
                this.timedifference = duration - 5
                this.showStoragecharge = true
                this.IsRecordWarehouseFee(this.waybillinfo.OrderID)
              }

            } else if (FirstTempDate <= LsEndDate) {
              var duration = ((ModifyDate - LsEndDate) / (1 * 24 * 60 * 60 * 1000));
              console.log(duration + "在租期内，且出库日期在租期外")
              if (duration >= 1) { //暂存超过五天
                this.timedifference = duration
                this.showStoragecharge = true
                this.IsRecordWarehouseFee(this.waybillinfo.OrderID)
              }
            } else if (ModifyDate <= LsEndDate) {
              this.showStoragecharge = false
              this.waybillinfo.IsClientLs = true
            }
          } else {
            var duration = ((ModifyDate - FirstTempDate) / (1 * 24 * 60 * 60 * 1000)) + 1;
            console.log(duration + "未租赁，应免租五天")
            if (duration > 5) { //暂存超过五天
              this.timedifference = duration - 5
              this.showStoragecharge = true
              this.IsRecordWarehouseFee(this.waybillinfo.OrderID)
            }
          }
        }

        ////如果是未完成并未租赁，并且天数大于五天，则判断是否收取过入仓费，如未收取，则需要录入入仓费
        //var olddata = this.moment(res.Waybill.FirstTempDate).format('YYYY-MM-DD')
        //var startDate = Date.parse(olddata);
        //var endDate = Date.parse(newdata);
        //var days = ((endDate - startDate) / (1 * 24 * 60 * 60 * 1000))+1;
        //this.timedifference = days
        //if (res.Waybill.IsClientLs == false && days > 5) {
        //  this.showStoragecharge = true  //只要暂存天数超过五天，无论是已收还是未收都可以录入
        //  this.IsRecordWarehouseFee(res.Waybill.OrderID)
        //}

        this.reFresh = true
      })
    },
    fromphotos(type) {
      if (type == "Waybill"||type =='Clientimg') {
        var Type=null
        if(type == "Waybill"){
          Type=8010
        }else{
           Type=8020
        }
        var data = {
          SessionID:type,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            Type:Type
          }
        };
        FormPhoto(data);
      }  else {
        var data = {
          SessionID: type.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            NoticeID:type.ID,
            InputID:'',
            Type:8010
          }
        };
        FormPhoto(data);
      }
    },
    changed(message) {
      //后台调用winfrom 拍照的方法
      this.testfunction(message); //前台拿到返回值处理数据
    },
    testfunction(message) {
      //前台处理数据的方法
      var imgdata = message[0];
      if (imgdata.SessionID == 'Waybill') {
        var newfile = {
            CustomName: imgdata.FileName,
            ID: imgdata.FileID,
            Url: imgdata.Url,
            Type: 8010
          };
        this.waybillinfo.FeliverGoodFile.push(newfile);
      } else if(imgdata.SessionID == 'Clientimg'){
         var newfile = {
            CustomName: imgdata.FileName,
            ID: imgdata.FileID,
            Url: imgdata.Url,
            Type: 8020
          }
        this.waybillinfo.SendGoodsFile.push(newfile)
      }
      else {
        var newfile = {
            CustomName: imgdata.FileName,
            ID: imgdata.FileID,
            Url: imgdata.Url,
            Type: 8010
          };
        for (var i = 0; i < this.detailitem.length; i++) {
          if (this.detailitem[i].ID == imgdata.SessionID) {
            this.detailitem[i].Imagefiles.push(newfile);
            this.changeoriginalarr(this.detailitem[i])
          }
        }
      }
    },
    SeletUpload(type) { // 传照
     if (type == "Waybill"||type =='Clientimg') {
        var Type=null
        if(type == "Waybill"){
          Type=8010
        }else{
           Type=8020
        }
        var data = {
          SessionID:type,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            Type:Type
          }
        };
        SeletUploadFile(data);
      } else {
        var data = {
          SessionID: type.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            NoticeID:type.ID,
            InputID:'',
            Type:8010
          }
        };
        SeletUploadFile(data);
      }
    },
    clickClient(value, type) {
      //显示更改输送地与原产地的方法
      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = String(value);
    },
    //箱号发生变化的时候
    changeshowbox(value){
      if(value==true){
        this.AVGWeightsum=null
         this.SelectRow.forEach(element => {
          this.AVGWeightsum+=element.AVGWeight*element.CurrentQuantity
        });
        const myDate = new Date();
        const year = myDate.getFullYear(); // 获取当前年份
        const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
        const day = myDate.getDate(); // 获取当前日（1-31）
        this.saleDate = `${year}/${month}/${day}`;
      }
    },
   handleCreate1(val) {  //箱号添加
      if(!val==false&&val!='WL'){
             var newdata={
                    enterCode:this.waybillinfo.EnterCode, // 统一使用当前运单的entercode
                    code:val, // 箱号
                    date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
                    adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
               }
               console.log(newdata)
             if(this.oldboxcode!=null){
                  var data={
                     boxCode:this.oldboxcode,
                    //  date:this.saleDate
                  }
                  BoxcodeDelete(data).then(res=>{
                      BoxcodeEnter(newdata).then(res=>{
                        if(res.success==false){
                        this.$Message.error(res.data)
                        this.newboxcode=null
                        this.oldboxcode=null
                        this.newboxcodeback=null
                        this.isclick=false
                        }else{
                          this.newboxcode=this.setboxsplit(res.boxCode)
                          this.oldboxcode=res.boxCode
                          this.newboxcodeback=res.boxCode
                          this.isclick=true
                        }
                      })
                  })
              }else{
                    BoxcodeEnter(newdata).then(res=>{
                        if(res.success==false){
                        this.$Message.error(res.data)
                        this.newboxcode=null
                        this.oldboxcode=null
                        this.newboxcodeback=null
                        this.isclickbtn=false
                        }else{
                          this.newboxcode=this.setboxsplit(res.boxCode)
                          this.oldboxcode=res.boxCode
                          this.newboxcodeback=res.boxCode
                          this.isclick=true
                        }
                      })
              }
      }else{
         this.$Message.error('请输入正确的箱号')
         this.newboxcode=null
      }
    },
     // 删除选定的箱号
    BoxcodeDelete(){
      var data={
        boxCode:this.newboxcodeback,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
        
      })
    },
    oldBoxDelete(val){
      var data={
        boxCode:val,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
      })
    },
    //获取可用箱号
    getboxarr(){
      var data = {
        whCode: sessionStorage.getItem("UserWareHouse"), //库房标识（库房编号）
        enterCode:this.waybillinfo.EnterCode, //入仓号
        date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
        orderID:this.waybillinfo.OrderID
      };
      CgBoxesShow(data).then(res => {
       
        if (res.length > 0) {
          this.boxingarr = res;
        }else{
          this.boxingarr=[]
        }
      });
    },
    okapply() {
      //确认申请新箱号
      // console.log(this.applyfrom);
      Boxenter(this.applyfrom).then(res => {
        
        if (res.success == true) {
          this.$Message.success("申请成功");
          this.getboxarr()
        } else {
          this.$Message.error("箱号申请失败");
        }
      });
    },
    clickshowchangebox() {
      //点击一键入箱，显示入箱弹窗
      if (this.SelectRow.length == 0) {
        this.$Message.error("至少选择一条产品");
      } else {
        // this.CgBoxesShow()
        // this.showchangebox = true;
        var TurnID=this.SelectRow[0].TinyOrderID
        for(var i=0;i<this.SelectRow.length;i++){
            if(this.SelectRow[i].TinyOrderID!=TurnID){
              this.$Message.error("小订单号不一致，请选择小订单号一致的进行操作");
              break;
            }else{
              if(i==this.SelectRow.length-1){
                var nullarr=[]
                  var widtharr=[]
                  for(var i=0,lens=this.SelectRow.length;i<lens;i++){
                    if(this.SelectRow[i].Weight==null){
                      nullarr.push(this.SelectRow[i])
                    }else{
                      widtharr.push(this.SelectRow[i])
                    }
                  }
                  console.log(nullarr)
                  console.log(widtharr)
                  if(nullarr.length!=0&&widtharr.length!=0){
                    this.$Message.error("勾选型号重量有误");
                  }else{
                    if(nullarr.length>0){
                        this.issharevalue=true
                    }else{
                       this.issharevalue=false;
                    }
                    this.sharevalue=null
                    this.newboxcode=null
                    this.newboxcodeback=null
                    this.boxcodetype='1'
                    // this.getboxarr();
                    this.showchangebox = true;
                  }
                 
              }
            }
        }
      }
    },
    ok_changebox() {
      if(this.newboxcode==null||this.newboxcode==''){
         this.$Message.error("请输入必填项");
         this.showchangebox=true;
      }else{
        if((this.issharevalue==true)&&(this.sharevalue==null)){
            this.$Message.error("请输入必填项");
            this.showchangebox=true;
        }else{
          var reg = /^\d+(\.\d{0,2})?$/;
          if(this.issharevalue==true&&(reg.test(this.sharevalue) == false||this.sharevalue==0)){
            this.$Message.error("请输入数字,小数点保留两位，且不等于零");
          }else{
            if(this.isclick==true){
              if(this.issharevalue==true){
                // this.ok_share()
                this.finish_btn()
              }else{
                 this.finish_btn()
              }
              // console.log(this.isclick)
              // this.finish_btn()
              this.showchangebox=false
            }
              
          }
        }
        
      }
    },
     //确认分摊重量
    ok_share(){
          var Totalquantity=null
          var singlet =null;
          // console.log(this.SelectRow)
          for(var i=0,lens=this.SelectRow.length;i<lens;i++){
            console.log(this.SelectRow[i])
            if(this.SelectRow[i].CurrentQuantity!=null&&this.SelectRow[i].CurrentQuantity!=''){
              Totalquantity+=Number(this.SelectRow[i].CurrentQuantity)
            }
          }
          if(Totalquantity!=null){
            for(var i=0,lens=this.SelectRow.length;i<lens;i++){
              if(this.SelectRow[i].CurrentQuantity!=null&&this.SelectRow[i].CurrentQuantity!=''){
                this.SelectRow[i].Weight=(this.SelectRow[i].CurrentQuantity*(this.sharevalue/Totalquantity)).toFixed(2)
              }
            }
          }
        // this.finish_btn()
    },
    GetBoxesdates() {
      //选择箱号日期
      GetBoxesdates().then(res => {
        console.log(res);
        this.applydata = res;
      });
    },
    detailelist() {
    //清单打印
    if (this.printlist.length > 0) {
      var configs = GetPrinterDictionary();
      var getsetting = configs["清单打印"];
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
            waybill: this.waybillinfo,
            listdata: this.printlist
          }
        ]
      };
      TemplatePrint(data);
    }
  },
  Labelprint() {
    //标签打印 选中多个
    console.log(this.SelectRow);
    var arr = this.SelectRow;
    var printsrr = [];
    if (arr.length <= 0) {
      this.$Message.error("请选择要操作的产品项");
    } else {
      for (var i = 0; i < arr.length; i++) {
          var Inputs = arr[i].Inputs;
          var StandardProducts = arr[i].StandardProducts;
          var obj = {
            Quantity: arr[i].CurrentQuantity, //数量
            inputsID: arr[i].Input.SortingID, //id
            Catalog: arr[i].Product.Catalog, //品名
            PartNumber: arr[i].Product.PartNumber, //型号
            Manufacturer: arr[i].Product.Manufacturer, //品牌
            Packing: arr[i].Product.Packing, //包装
            PackageCase: arr[i].Product.PackageCase, //封装
            origin: arr[i].originDes,//产地
            SourceDes:this.details.waybillinfo.SourceDes,//业务
          };
          printsrr.push(obj);
        }
      var configs = GetPrinterDictionary();
      var getsetting = configs["产品标签"];
      var str = getsetting.Url;
      var testurl = str.indexOf("http") != -1;
      if (testurl == true) {
        getsetting.Url = getsetting.Url;
      } else {
        getsetting.Url = this.printurl + getsetting.Url;
      }

      var data = {
        setting: getsetting,
        data: printsrr
      };
      TemplatePrint(data);
    }
  },
  finish_btn() {
    if(this.SelectRow.length<=0){
      this.$Message.error('请选择要分拣的产品')
    }else{
       this.WarehousingMsg = true 
      }
    
    },
     setWarehousing(data) {
      //确定出库，调取后台出库接口
       cgCustomsStorageTurnDeclare(data).then(res=>{
          console.log(res)
          if(res.Success==true){
             this.$Message.success('分拣完成')
               var _this=this
             setTimeout(function(){
                _this.loading=true
                _this.getdetail_data(this.wayBillID);
                _this.SelectRow=[]
                _this.$refs.reFresh.loading=true
                _this.$refs.reFresh.getlistdata()
             },1000)
            
          }else{
            this.$Message.error(res.Data)
            this.isdisabled=false
          }
      })
    },
    getCookie(cookieName) {
      var strCookie = document.cookie
      var arrCookie = strCookie.split('; ')
      for (var i = 0; i < arrCookie.length; i++) {
        var arr = arrCookie[i].split('=')
        if (cookieName == arr[0]) {
          return arr[1]
        }
      }
      return ''
    },
    ok_Warehousing() {
      //点击确定按钮，进行出库操作
      if(this.issharevalue==true){
        this.ok_share()
      }
      console.log(this.issharevalue)
      // console.log(this.newboxcode)
       this.isdisabled=true
       this.WarehousingMsg = false
        this.pusharr=[]
       for(var i=0,len=this.SelectRow.length;i<len;i++){
          var index=i
            var obj={
                NoticeID: this.SelectRow[i].ID,
                OutputID:this.SelectRow[i].OutputID,
                BoxCode: this.newboxcodeback,
                Quantity: this.SelectRow[i].CurrentQuantity,
                Weight: this.SelectRow[i].Weight,
                NetWeight: this.SelectRow[i].NetWeight,
                Volume: this.SelectRow[i].Pickings.Volume,
                Files: this.SelectRow[i].Imagefiles,
              }
           this.pusharr.push(obj)
         }
    var uploaddata={
      Waybill:{
            ID: this.waybillinfo.ID,
            ExcuteStatus:this.waybillinfo.ExcuteStatus,
            Type:this.waybillinfo.Type, //库房自定影
            OrderID:this.waybillinfo.OrderID, //所属订单
            Summary: "",
            Files:this.waybillinfo.FeliverGoodFile,
            LoadingExcuteStatus:this.waybillinfo.LoadingExcuteStatus, //请参考提送货枚举 d:\projects_vs2015\yahv\solutions\yahv.underly\enums\enum.cgexcutestatus.cs
            WarehouseID:sessionStorage.getItem("UserWareHouse")
        },
        AdminID:sessionStorage.getItem('userID') , //当前的操作人
        Pickings:this.pusharr //用inputID 关联 Noitice
    }
      this.setWarehousing(uploaddata)
      // console.log(this.pusharr)
    },
    cancel_Warehousing() {
      //点击取消按钮，取消出库
      this.WarehousingMsg = false
      this.oldboxcode=null
      this.BoxcodeDelete()
    },
    errorstatue(value) {
      if (value == true) {
        this.isAbnormal = true
      } else {
        this.isAbnormal = false
        this.Summary = ''
      }
    },
    Abnormal_btn() {
      if (this.Summary == undefined || this.Summary == '') {
        this.isAbnormal = true
        this.$Message.error('请输入异常原因')
      } else {
          this.isdisabled=true
          this.isAbnormal = false
          var data={
            waybillid:this.waybillinfo.ID,
            adminid:sessionStorage.getItem('userID'),
            orderid:this.waybillinfo.OrderID,
            summary:this.Summary
          }
          console.log(data)
          CgpickingsErrorBtn(data).then(res=>{
            if(res.Success==true){
             this.$Message.success('出库完成，一秒后自动关闭')
               var _this=this
             setTimeout(function(){
                  _this.$store.dispatch('setshowtype', 0)
                  _this.$store.dispatch('setshowdetailout', false)
                  _this.$router.go(-1)
             },1000)
            
          }else{
            this.$Message.error('出库操作失败,请从新操作')
            this.isdisabled=false;
          }
          })
        this.isAbnormal = false
      }
    },
     closeerror() {
      //异常到货关闭
      this.isAbnormal = false
      this.Summary = '' //备注
      this.Reason = '外观损坏' //异常原因
    },
    // 点击删除文件信息
    handleRemove(file,type) {
      var data={
          id:file.ID
        }
         CgDeleteFiles(data).then(res=>{
          if(res.Success==true){
            this.IsfileDel=true
            this.Removebackfun(file,type)
            this.$Message.success('删除成功')
          }else{
            this.IsfileDel=false
            this.$Message.error('删除失败')
          }
      })
    },
    Removebackfun(file,type){
      if(type=="Clientimg"){
          this.waybillinfo.SendGoodsFile.splice(this.waybillinfo.SendGoodsFile.indexOf(file), 1)
      }else if(type=="Waybill"){
         this.waybillinfo.FeliverGoodFile.splice(this.waybillinfo.FeliverGoodFile.indexOf(file), 1)
      }else{
        var arr = this.detailitem;
        for (var j = 0; j < arr.length; j++) {
          //删除指定下标 的元素
          if (arr[j].ID == type.ID) {
             arr[j].Imagefiles.splice(file, 1)
             this.changeoriginalarr(arr[j])
          }
        }
      }
    },
    changeacrrier(value) {
      //改变承运商的时候触发
      this.waybillinfo.CarrierName = value.label
      if (this.waybillinfo.Type == 2) {
        this.GetDriversCars(value.value)
      } else {
      }
    },
    search_pro() {
      this.loading=true
      if(this.searchkey!=''){
         CgPickingsSearch(this.waybillinfo.ID,this.searchkey).then(res=>{
            // this.detailitem=res;
            if(res.length>0){
                var historydata=this.detailitem
                var newdata=[]
                var result = new Array();
                var sameCount = 0;
                for(var i=0;i<historydata.length;i++){
                    var tempA = historydata[i].ID;
                    for(var j=0;j<res.length;j++){
                        var tempB = res[j].ID;
                        if(tempA == tempB){
                           newdata.push(historydata[i])
                        }
                    }
                }
                  this.detailitem=newdata
             }else{
               this.detailitem=[]
             }
          })
      }else{
        this.detailitem=this.originalarr
      }
      this.loading=false;
    },
     //一键打印重构
    CgAllprint(){
      if(this.fruit.indexOf('出库单打印')!=-1){
           this.outorder_print()
      }
      if(this.fruit.indexOf('送货单打印')!=-1){
          this.Boxing_print()
      }
    },
    //出库单打印功能
    outorder_print() {
      var data = {
          waybillinfo: this.waybillinfo,
          listdata:this.detailitem
      }
      PrintOuptNotice(data)
    },
     //装箱单打印 拣货单 送货单
    Boxing_print() {
      var Numcopies=null
      if(this.warehouseID.indexOf('SZ')!=-1){ //深圳库房
        if(this.waybillinfo.Type == 4||this.waybillinfo.Type == 3){
           this.language = 'SC'
           Numcopies=2
        }else{
           this.language = 'SC'
          Numcopies=4
        }
      }else{ //香港库房
          if (this.waybillinfo.Type == 4) {//国际
             this.language = 'EN'
             Numcopies=2
          } else if(this.waybillinfo.Type == 3){//本港
            this.language = 'TC'
            Numcopies=2
          }else{
             this.language = 'TC'
             Numcopies=4
          }
      }
      var data = {
        Language: this.language,
        waybillinfo: this.waybillinfo,
        listdata:this.detailitem,
        Numcopies:Numcopies
      }
      PrintDeliveryList(data)
    },
    //箱签打印
    PrintBoxcode(){ 
       var Obj={
        Source:this.waybillinfo.Source,
        waybillID:this.waybillinfo.ID
      }
      this.showprintboxcode=true;
      this.$refs.printbox.getparents(Obj) 
      this.$refs.printbox.GetPackageType() 
    },
    //文件打印
  fileprint(printurl) {
    var configs = GetPrinterDictionary()
    var getsetting = configs['文档打印']
    getsetting.Url = printurl
    var data = getsetting
    FilePrint(data)
  },
  clackFilesProcess(url){
      var data={
        Url:url
      }
      FilesProcess(data)
    },
    // 获取客户等级
  Getclientdata(entdata){
      Getclientdata(entdata).then(res=>{
        console.log(res)
        this.clientGrade=res.obj.Grade
      })
    },
    TestAVGWeightsum(){
      if(this.sharevalue!=null){
        var reg = /^\d+(\.\d{0,5})?$/;
        if(reg.test(this.sharevalue) == false||this.sharevalue==0){
            this.$Message.error("请输入数字,小数点保留五位，且不等于零");
            this.sharevalue=null
        }else{
          if(this.sharevalue<(this.AVGWeightsum-(this.AVGWeightsum*(1/2)))||this.sharevalue>(this.AVGWeightsum+(this.AVGWeightsum*(1/2)))){
             this.$Modal.warning({ title: '超过浮动50%！',});
          }
        }
      }
    },
    //操作日志的展示
    logchange(){
      this.showlogged=true
      this.loggdetime=new Date().getTime()
    },
    changeData(val){
      this.saleDate=val
       if(this.newboxcode!=''&&this.newboxcode!=null){
        this.handleCreate1(this.newboxcode)
      }
    },
     hietorgetDetail(){
      //  this.$emit("uploadeCgDetail_new");
      this.loading=true
      this.getdetail_data(this.wayBillID);
    },
     sync_btn(){
       this.loading = true;
       this.getdetail_data(this.wayBillID);
       this.$refs.reFresh.loading=true
       this.$refs.reFresh.getlistdata()
    },
    //仓储费录入
    showBudget2() {
      this.storagecharge = true
      this.$refs.Storagecharge.IncomeParters(this.waybillinfo.OrderID)
    },
    changestoragecharge(value) {
      this.storagecharge = value
    },
    //是否收取入仓费
    IsRecordWarehouseFee(OrderID) {
      IsRecordWarehouseFee(OrderID).then(res => {
        console.log(res)
        if (res.data == "True") {
          
        } else {
          this.isdisabled = true;

        }
      })
    },
    Parentfun() {
      if (this.waybillinfo.ExcuteStatus == 215) {
        this.isdisabled = true;
      } else {
        this.isdisabled = false;
      }
    }
  },
};
</script>
