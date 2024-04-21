<style>
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
  width: 70px;
  /* font-weight: bold; */
}
.detail_title2 {
  display: inline-block;
  width: 75px;
}
.detail_title3 {
  display: inline-block;
  width: 95px;
}
.detail_tablebox td .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.detail_tablebox th .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.sethover {
  cursor: pointer;
}

.ivu-table .demo-table-info-row td {
  display: none;
}
.ivu-table .demo-table-error-row td {
  background-color: #ff6600;
  color: #fff;
}
.ivu-table td.demo-table-info-column {
  background-color: #2db7f5;
  color: #fff;
}
.ivu-table .demo-table-info-cell-name {
  background-color: #2db7f5;
  color: #fff;
}
.ivu-table .demo-table-info-cell-age {
  background-color: #ff6600;
  color: #fff;
}
.ivu-table .demo-table-info-cell-address {
  background-color: #187;
  color: #fff;
}
.setimgcolor {
  color: #2d8cf0;
}
.setimgcolor:hover {
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
          <Row v-if="details.waybillinfo!=null">
            <Col style="width: 20%;float: left;">
              <ul class="detail_li detail1">
                <li class="itemli">
                  <span class="detail_title1">ID：</span>
                  <span>{{details.waybillinfo.WaybillID}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title1">状态：</span>
                  <span>{{details.waybillinfo.ExcuteStatusDescription}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title1">业务类型：</span>
                  <span>{{details.waybillinfo.SourceDescription}}</span>
                </li>
                <li class="itemli" v-if="details.waybillinfo.Conditions!=undefined">
                  <Icon
                    type="md-alert"
                    v-if="Conditionstype"
                    style="font-size: 22px;color: #da2828;"
                  />
                  <Tag color="geekblue" v-if="details.waybillinfo.Conditions.AgencyCheck==true">代检查</Tag>
                  <Tag color="purple" v-if="details.waybillinfo.Conditions.AgencyPayment==true">代垫货款</Tag>
                  <Tag color="blue" v-if="details.waybillinfo.Conditions.AgencyReceive==true">代收货款</Tag>
                  <Tag color="cyan" v-if="details.waybillinfo.Conditions.ChangePackaging==true">代收货款</Tag>
                  <Tag color="green" v-if="details.waybillinfo.Conditions.LableServices==true">标签服务</Tag>
                  <Tag color="gold" v-if="details.waybillinfo.Conditions.PayForFreight==true">垫付运费</Tag>
                  <Tag color="orange" v-if="details.waybillinfo.Conditions.Repackaging==true">重新包装</Tag>
                  <Tag color="volcano" v-if="details.waybillinfo.Conditions.UnBoxed==true">拆箱验货</Tag>
                  <Tag color="red" v-if="details.waybillinfo.Conditions.VacuumPackaging==true">真空包装</Tag>
                  <Tag
                    color="magenta"
                    v-if="details.waybillinfo.Conditions.WaterproofPackaging==true"
                  >防水包装</Tag>
                </li>
              </ul>
            </Col>
            <Col style="width: 26%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title2">通知时间：</span>
                  <span>{{details.waybillinfo.CreateDate|showDate}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">供应商：</span>
                  <span>{{details.waybillinfo.Supplier}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">到货方式：</span>
                  <span>{{details.waybillinfo.WaybillTypeDescription}}</span>
                  <!-- <a href="">历史到货
                         <Badge :count="10"></Badge>
                  </a>-->
                  <Badge
                    :count="historydetail.waybillLIst.length"
                    :offset="[9,-12]"
                    class-name="demo-badge-alone"
                    v-if="historydetail.waybillLIst.length>0"
                  >
                    <!-- <a @click="showhistory">历史到货</a> -->
                    <Dropdown @on-click="showhistory">
                      <a href="javascript:void(0)">历史到货</a>
                      <DropdownMenu slot="list">
                        <DropdownItem
                          v-for="(item,index) in historydetail.waybillLIst"
                          :name="item"
                          :key="index"
                        >
                          <span>{{item}}</span>
                        </DropdownItem>
                      </DropdownMenu>
                    </Dropdown>
                  </Badge>
                </li>
                <li class="itemli">
                  <span class="detail_title2">订单号：</span>
                  <span
                    style="line-height: 20px;width: 70%;display: inline-block;"
                  >{{details.waybillinfo.OrderID}}</span>
                </li>
              </ul>
            </Col>
            <Col style="width: 31%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title3">入仓号：</span>
                  <span>{{details.waybillinfo.EnterCode}}</span>
                  <em>({{details.waybillinfo.ClientName}})</em>
                </li>
                <li class="itemli">
                  <span class="detail_title3">运单号(本次)：</span>
                  <span>
                    <Input style="width:60%" v-model="details.waybillinfo.Code" />
                  </span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">承运商(本次):</span>
                  <span>
                    <Select v-model="details.waybillinfo.CarrierID" style="width:60%">
                      <Option
                        v-for="item in CarrierList"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">输送地：</span>
                  <span>
                    <!-- <Input style="width:60%" v-model="details.Conveyorsite" /> -->
                    <span
                      v-if="details.waybillinfo.PlaceDescription!=0"
                    >{{details.waybillinfo.PlaceDescription}}</span>
                    <span v-else style="color:red;">暂无输送地</span>
                    <Icon
                      class="sethover"
                      @click="clickClient(details.waybillinfo.Place,'ClientCode','ClientCode')"
                      type="md-create"
                    />
                  </span>
                </li>
                <li class="itemli" v-if="details.waybillinfo.WaybillType==1">
                  <div class="detail_title3" style="float:left">提货文件：</div>
                  <div style="float:left">
                    <p
                      class="setimgcolor"
                      v-for="(item,index) in details.waybillinfo.Files"
                      v-if="item.Type==10"
                    >
                      <span>{{item.CustomName}}</span>
                      <span @click="fileprint(item.Url)">打印</span>
                    </p>
                  </div>
                </li>
              </ul>
            </Col>
            <Col style="width: 23%;float: left;">
              <ul class="detail_li" style="margin-left:20px;">
                <li class="itemli">
                  <!-- <img-test ref="allimg" :type="1"></img-test> -->

                  <div class="setupload">
                    <Button
                      type="primary"
                      size="small"
                      icon="ios-cloud-upload"
                      @click="SeletUpload(details.waybillinfo.WaybillID)"
                    >传照</Button>
                  </div>
                  <div class="setupload">
                    <!-- <Button type="primary" icon="ios-search" @click="photoing('waybill')">拍照</Button> -->
                    <Button
                      type="primary"
                      icon="md-reverse-camera"
                      @click="fromphotos('Waybill')"
                    >拍照</Button>
                  </div>
                </li>
                <li style="clear: both;">
                  <div v-for="(item,index) in details.waybillinfo.Files" v-if="item.Type==8000">
                    <p class="setimgcolor">
                      <span @click="PictureShow(item.Url)">{{item.CustomName}}</span>
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
                    </p>
                  </div>
                </li>
                <li
                  style="position: absolute; top: 147px; right: 0;" v-if="details.waybillinfo.WaybillType==1&&details.waybillinfo.ExcuteStatus==100&&TakeGoodsName!='正在提货'">
                  <Button type="primary" @click="TakeGoods">{{TakeGoodsName}}</Button>
                </li>
              </ul>
            </Col>
          </Row>
          <!-- <Modal v-model="showphoto2" title="拍照并上传" width="1006px">
            <photo-graph ref="photograph" v-on:changupload="changupload($event,fathertype)"></photo-graph>
            <div slot="footer">
              <Button type="text" ref="clostbtn" @click="closephoto">关闭</Button>
              <Button type="primary" ref="okbtn" @click="upload_btn">确定</Button>
            </div>
          </Modal> -->
        </div>
      </div>
      <div class="itembox">
        <p class="detailtitle">产品清单</p>
        <div style="margin:15px 0">
          <ButtonGroup style="width:28%">
            <Input
              v-model="searchkey"
              placeholder="请输入品牌或型号"
              style="width:80%;float:left;position: relative;left: 3px"
            />
            <!-- <Button style="float:left" @click="search_pro" type="primary">筛选</Button> -->
            <Button style="float:left" type="primary" @click="search_filter">筛选</Button>
          </ButtonGroup>
          <Button type="primary" @click="detailelist">清单打印</Button>
          <Button type="primary" @click="Labelprint">标签打印</Button>
          <Button
            type="primary"
            @click="SetStorehouse"
            :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
          >一键入库</Button>
          <Button type="primary" @click="showBudget">收支明细</Button>
          <!-- <Button type="primary" @click="fileprint">文件打印</Button> -->
          <!--<Button type="primary" @click="imgprint">图片打印</Button>-->
          <!-- <Button type="primary" @click="toout_btn('Waybill201912210018')">代转运测试</Button> -->
          <div style="float:right">
            <Button
              :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
              type="primary"
              shape="circle"
              icon="md-checkmark"
              @click="finish_btn"
            >入库完成</Button>
            <Button
              :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
              type="warning"
              shape="circle"
              icon="ios-alert-outline"
              @click="isAbnormalclick"
            >到货异常</Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table
              ref="selection"
              :loading="loading"
              :columns="details.columns1"
              :data="details.detailitem"
              :row-class-name="rowClassName"
              @on-selection-change="handleSelectRow"
            >
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <template slot-scope="{ row, index }" slot="Arrival">
                <span
                  v-if="row.Enabled==true&&row.iscx!=false"
                >{{row.SortedQuantity}}&nbsp;/&nbsp;{{row.Quantity}}</span>
                <!-- <span>0&nbsp;/&nbsp;{{row.Quantity}}</span> -->
              </template>
              <template slot-scope="{ row, index }" slot="OriginDes">
                <span>{{row.Input.OriginDescription}}</span>
                <Icon class="sethover" @click="clickClient(row.Input.Origin,row,index)" type="md-create" />
                <!-- <Select 
                      v-model="row.Input.Origin"
                      filterable
                      style="width:60%"
                      :label-in-value="true"
                      @on-change="changConveyingplace($event,row)">
                  <Option v-for="(item,index) in Conveyingplace"
                          :value="item.ID"
                          :key="item.ID"
                      >{{ item.CorPlaceDes }}</Option>
                </Select>-->
              </template>
              <template slot-scope="{ row, index }" slot="ShelveID">
                <Select
                  v-if="row.Enabled==true"
                  v-model="row.ShelveID"
                  filterable
                  :transfer="true"
                  :label-in-value="true"
                  @on-change="changConveyingplace($event,row,index)"
                >
                  <Option
                    v-for="(item,index) in Storehouselist"
                    :value="item.ID"
                    :key="item.ID"
                  >{{ item.ID }}</Option>
                </Select>
                <Select
                  v-else
                  :disabled="true"
                  v-model="row.ShelveID"
                  filterable
                  :transfer="true"
                  :label-in-value="true"
                  @on-change="changConveyingplace($event,row,index)"
                >
                  <Option
                    v-for="(item,index) in Storehouselist"
                    :value="item.ID"
                    :key="item.ID"
                  >{{ item.ID }}</Option>
                </Select>
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="(item,index) in row.Files" v-if="item.Type==8000" class="setimgcolor">
                  <span @click="PictureShow(item.Url)">{{item.CustomName}}</span>
                  <Icon
                    type="ios-trash-outline"
                    :ref="row.ID"
                    @click.native="handleRemovelist(row,index)"
                  ></Icon>
                </p>
                <!-- <Input v-model="row.typeimg" /> -->
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <!-- <img-test v-bind:type="row" v-on:changitem="changeimgs($event,row)"></img-test> -->
                  <Button
                    :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    size="small"
                    icon="ios-cloud-upload"
                    @click="SeletUpload(row.InputID)"
                  >传照</Button>
                </div>
                <div class="setupload">
                  <!-- <Button type="primary" icon="md-reverse-camera" @click="photoing(row)">拍照</Button> -->
                  <Button
                    :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    icon="md-reverse-camera"
                    @click="fromphotos(row.InputID)"
                  >拍照</Button>
                </div>
                <div class="setupload" v-if="row.iscx!=false">
                  <Button
                    :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    icon="md-checkmark"
                    @click="chaixiang(index,row)"
                  >拆项</Button>
                </div>
                <div class="setupload" v-if="row.iscx==false">
                  <Button
                    :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    icon="ios-trash-outline"
                    @click="removechaixiang(index,row)"
                  >删除</Button>
                </div>
              </template>
            </Table>
            <div style="padding:10px;">
              <!-- <Page style="float:right" :total="details.total" :page-size='details.pageSize' @on-change='detailspage' /> -->
            </div>
            <div class="successbtn">
              <Button
                type="primary"
                icon="md-checkmark"
                @click="finish_btn"
                :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
              >入库完成</Button>
              <!-- <Button type="error">到货异常</Button> -->
              <Button
                type="warning"
                icon="ios-alert-outline"
                @click="isAbnormalclick"
                :disabled="details.waybillinfo.ExcuteStatus== 120 ? true:false"
              >到货异常</Button>
              <!-- <Button type="warning" icon="ios-alert-outline" @click="toout_btn">出库</Button> -->
            </div>
          </div>
        </div>
      </div>
      <div v-if="WarehousingMsg==true">
        <Modal
          v-model="WarehousingMsg"
          title="确定入库"
          @on-ok="ok_Warehousing"
          @on-cancel="cancel_Warehousing"
        >
          <div v-if="Nomatching.length!=0">
            <!-- <span v-for="(item,index) in Nomatching" :key="index">{{item}}与通知型号不都，是否入库</span> -->
            <h2>以下型号数量与通知应到数量不符，是否继续进行入库操作</h2>
            <ul>
              <li v-for="(item,index) in Nomatching" :key="index">
                <h2>{{item}}</h2>
              </li>
            </ul>
          </div>
          <div v-else>
            <span>是否全部入库</span>
          </div>
        </Modal>
      </div>
      <!-- 一键入库 开始-->
      <Modal v-model="sethousbox" title="一键入库" @on-cancel="cancel">
        <div slot="close">
          <Icon
            type="md-close-circle"
            color="rgb(33, 28, 28)"
            @click="closeerror"
            style="font-size:18px;"
          />
        </div>
        <span style="line-height:26px;">选择库位</span>
        <Select v-model="housenumber" filterable v-if="Storehouselist.length>0">
          <Option v-for="item in Storehouselist" :value="item.ID" :key="item.ID">{{ item.ID }}</Option>
        </Select>
        <div slot="footer">
          <Button @click="cancel">取消</Button>
          <Button type="primary" @click="changehouse">确定</Button>
        </div>
      </Modal>
      <!-- 一键入库 结束-->

      <!-- 异常到货 开始-->
      <Modal v-model="isAbnormal" title="到货异常" @on-visible-change="changeerror">
        <div slot="close">
          <Icon
            type="md-close"
            color="rgb(33, 28, 28)"
            @click="closeerror"
            style="font-size:18px;"
          />
        </div>
        <!-- <span style="line-height:26px;">选择异常原因</span>
        <Select v-model="Reason" filterable>
          <Option v-for="item in errordata" :value="item.value" :key="item.value">{{ item.label }}</Option>
        </Select>-->
        <span style="line-height:26px;">异常原因</span>
        <Input v-model="Summary.Summary" type="textarea" :rows="2" placeholder="备注" />
        <div slot="footer">
          <Button @click="closeerror">取消</Button>
          <Button type="primary" @click="Abnormal_btn">确定</Button>
        </div>
      </Modal>
      <!-- 异常到货 结束-->
    </div>
    <!-- 历史到货 开始 -->
    <Drawer :closable="true" :width="70" :mask-closable='true' v-model="historydata">
      <Historys-dom :key="historydetail.times" ref="Historygoods"></Historys-dom>
    </Drawer>
    <!-- 历史到货 结束 -->

    <!-- 收支明细 开始 -->
    <Modal
      v-model="Budgetdetail"
      width="55%"
      :closable="false"
      :mask-closable="false"
      :footer-hide="true"
    >
      <div style="clear: both; overflow: hidden;font-size:16px;padding-bottom:10px">
        <Icon
          type="ios-close"
          style="float:right;font-size:30px;font-weight:bold;"
          @click="closeBudget"
        />
      </div>
      <div>
        <router-view></router-view>
      </div>
    </Modal>
    <!-- 收支明细结束 -->

    <!-- 输送地列表与更改 开始-->
    <Modal v-model="setClientCode" title="选地地区" @on-cancel="cancel">
      <Select v-model="ClientCode" filterable :label-in-value="true" @on-change="changClientCode">
        <Option
          v-for="(item,index) in Conveyingplace2"
          :value="item.CorPlace"
          :key="item.ID"
        >{{ item.Text }}</Option>
      </Select>
      <div slot="footer">
        <!-- <Button >取消</Button> -->
        <Button type="primary" @click="primaryClientCode">确定</Button>
      </div>
    </Modal>
    <!-- 输送地列表与更改 结束-->
  </div>
</template>
<script>
import imgtest from "@/Pages/Common/imgtes";
import Photograph from "@/Pages/Common/Photograph";
import Historys from "@/Pages/Common/Historygoods";
// import Vue from 'vue'
import {
  Noticedetail,
  sortingupload,
  search_detail,
  getWayParter,
  Carriers,
  History,
  GetUsableShelves,
  TakeGoods,
  GetInputID
} from "../../api";
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint,
  FormPhoto,
  SeletUploadFile,
  PictureShow
} from "@/js/browser.js";
let Base64 = require("js-base64").Base64;
let product_url = require("../../../static/pubilc.dev");
import $ from "jquery";
import Vue from "vue";
import moment from "moment";
// import _ from 'lodash'
// Vue.prototype._ = _
let lodash = require("lodash");

// var getuplode=""

export default {
  name: "RoutineEnter",
  components: {
    "img-test": imgtest,
    "photo-graph": Photograph,
    "Historys-dom": Historys
  },
  props: {
    fatherMethod: {
      type: Function,
      default: null
    }
  },
  data() {
    return {

      // getuplode:getuplode,
      // config:configs,
      // Budgetdetail:false, //收支明细
      TakeGoodsName: "我去提货",
      Conditionstype: true,
      printurl: product_url.pfwms,
      loading: true, //loading效果
      printlist: [], //打印列表
      Conveyingplace: [], //输送地列表
      Conveyingplace2: [],
      ConveyingplaceID:"",//输送地ID
      chengevalue: {
        inputval: "",
        value: "",
        type: ""
      },
      setClientCode: false, //显示输送地选择模态框
      ClientCode: "", //默认输送地
      model11: "",
      sethousbox: false, //一键入库弹出框
      housenumber: "", //选择的库位号
      Storehouselist: [], //库位号列表
      detail_ID: "",
      searchkey: "", //筛选条件
      showphoto2: false, //显示拍照弹出框,
      time: "",
      WarehousingMsg: false, //完成入库的提示 数量对与不对
      Nomatching: [], //数量不对提示型号
      details: {
        //详情页
        waybillinfo: {}, //详情运单信息
        wayBillID: "",
        total: 0,
        pageIndex: 1,
        pageSize: 10000,
        WaybillNo: "90416165067", //运单号(本次)
        Conveyorsite: "", //输送地,
        columns1: [
          {
            type: "selection",
            width: 50,
            align: "center"
          },
          {
            title: "# ",
            slot: "indexs",
            align: "left",
            width: 30
            // fixed: 'right'
          },
          {
            title: "型号",
            key: "PartNumber",
            // width: 80,
            render: (h, params) => {
              var vm = this;
              if (params.row.iscx == false) {
                return h("span", {}, params.row.Product.PartNumber);
              } else {
                return h("Input", {
                  props: {
                    //将单元格的值给Input
                    value: params.row.Product.PartNumber
                  },
                  on: {
                    "on-change"(event) {
                      //值改变时
                      //将渲染后的值重新赋值给单元格值
                      params.row.Product.PartNumber = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                      var arr = vm.details.detailitem;
                      for (var i = 0; i < arr.length; i++) {
                        if (arr[i].PID == params.row.ID) {
                          arr[i].Product.PartNumber = event.target.value;
                        }
                      }
                      vm.clicktest(params.row);
                      // var cc=lodash.throttle(function(){
                      //       console.log('hello')
                      //       console.log(this)
                      // },1000)
                    }
                  }
                });
              }
            }
          },
          {
            title: "品牌",
            key: "Manufacturer",
            // width: 80,
            render: (h, params) => {
              var vm = this;
              if (params.row.iscx == false) {
                return h("span", {}, params.row.Product.Manufacturer);
              } else {
                return h("Input", {
                  props: {
                    //将单元格的值给Input
                    value: params.row.Product.Manufacturer
                  },
                  on: {
                    "on-change"(event) {
                      //值改变时
                      //将渲染后的值重新赋值给单元格值
                      params.row.Product.Manufacturer = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                      var arr = vm.details.detailitem;
                      for (var i = 0; i < arr.length; i++) {
                        if (arr[i].PID == params.row.ID) {
                          arr[i].Product.Manufacturer = event.target.value;
                        }
                      }
                      vm.clicktest(params.row);
                    }
                  }
                });
              }
            }
          },
          {
            title: "已到/应到",
            slot: "Arrival",
            align: "center"
            // width: 70
          },
          {
            title: "批次",
            key: "DateCode",
            align: "center",
            // width: 60,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: params.row.Input.DateCode
                },
                on: {
                  "on-change"(event) {
                    //值改变时
                    //将渲染后的值重新赋值给单元格值
                    params.row.Input.DateCode = event.target.value;
                    vm.details.detailitem[params.index] = params.row;
                    vm.clicktest(params.row);
                  }
                }
              });
            }
          },
          {
            title: "本次到货",
            key: "TruetoQuantity",
            align: "center",
            // width: 50,
            render: (h, params) => {
              var vm = this;
              var prients = null;
              console;
              if (params.row.Enabled == false) {
                return h("Input", {
                  props: {
                    //将单元格的值给Input
                    value: params.row.TruetoQuantity,
                    disabled: true
                  },
                  on: {
                    "on-change"(event) {},
                    "on-blur"(event) {}
                  }
                });
              } else {
                if (params.row.iscx == false) {
                  return h("Input", {
                    props: {
                      //将单元格的值给Input
                      value: params.row.TruetoQuantity
                    },
                    on: {
                      "on-change"(event) {
                        // var reg = /^[0-9]*$/;
                        var reg= /^\d+(\.\d{0,2})?$/;
                        if (reg.test(event.target.value) == true) {
                          params.row.TruetoQuantity = event.target.value;
                          vm.details.detailitem[params.index] = params.row;
                          vm.clicktest(params.row);
                        } else {
                          vm.$Message.error("只能输入正整数");
                          (event.target.value = ""),
                            (params.row.TruetoQuantity = "");
                          vm.details.detailitem[params.index] = params.row;
                        }
                      },
                      "on-blur"(event) {
                        console.log(event.target.value);
                        if (event.target.value != "") {
                          // var reg = /^[0-9]*$/;
                          var reg= /^\d+(\.\d{0,2})?$/;
                          if (reg.test(event.target.value) == true) {
                            params.row.TruetoQuantity = event.target.value;
                            vm.details.detailitem[params.index] = params.row;
                          } else {
                            vm.$Message.error("只能输入正整数");
                            (event.target.value = ""),
                              (params.row.TruetoQuantity = "");
                            vm.details.detailitem[params.index] = params.row;
                          }
                        }
                      }
                    }
                  });
                } else {
                  return h("Input", {
                    props: {
                      //将单元格的值给Input
                      value: params.row.TruetoQuantity
                    },
                    on: {
                      "on-change"(event) {
                        // var reg = /^[0-9]\d*$/;
                        var reg= /^\d+(\.\d{0,2})?$/;
                        if (reg.test(event.target.value) == true) {
                          params.row.TruetoQuantity = event.target.value;
                          vm.details.detailitem[params.index] = params.row;
                          vm.clicktest(params.row);
                        } else if (event.target.value < 0) {
                          (event.target.value = ""),
                            (params.row.TruetoQuantity = "");
                          vm.details.detailitem[params.index] = params.row;
                        } else {
                          vm.$Message.error("只能输入正整数");
                          (event.target.value = ""),
                            (params.row.TruetoQuantity = "");
                          vm.details.detailitem[params.index] = params.row;
                        }
                      },
                      "on-blur"(event) {
                        if (event.target.value != "") {
                          // var reg = /^[0-9]\d*$/;
                          var reg= /^\d+(\.\d{0,2})?$/;
                          if (reg.test(event.target.value) == true) {
                            params.row.TruetoQuantity = event.target.value;
                            vm.details.detailitem[params.index] = params.row;
                          } else {
                            vm.$Message.error("只能输入正整数");
                            (event.target.value = ""),
                              (params.row.TruetoQuantity = "");
                            vm.details.detailitem[params.index] = params.row;
                          }
                        }
                      }
                    }
                  });
                }
              }
            }
          },
          {
            title: "原产地",
            slot: "OriginDes",
            align: "left"
            // width:100
          },
          {
            title: "入库库位",
            slot: "ShelveID",
            align: "left",
            width: 135
          },
          // {
          //   title: "入库库位",
          //   key: "ShelveID",
          //   align: "center",
          //   // width: 60,
          //   render: (h, params) => {
          //     var vm = this;
          //     return h("Input", {
          //       props: {
          //         //将单元格的值给Input
          //         value: params.row.ShelveID
          //       },
          //       on: {
          //         "on-change"(event) {
          //           //值改变时
          //           //将渲染后的值重新赋值给单元格值
          //           params.row.StockCode = event.target.value;
          //           vm.details.detailitem[params.index] = params.row;
          //         }
          //       }
          //     });
          //   }
          // },
          {
            title: "体积(m³)",
            key: "Volume",
            align: "center",
            // width: 70,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: params.row.Volume
                },
                on: {
                  "on-change"(event) {
                    //值改变时
                    //将渲染后的值重新赋值给单元格值
                    var reg = /^\d+(\.\d{0,2})?$/;
                    // reg.test(event.target.value);
                    if (reg.test(event.target.value) == true) {
                      params.row.Volume = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      event.target.value = "";
                      params.row.Volume = "";
                      vm.details.detailitem[params.index] = params.row;
                    }
                  },
                  "on-blur"() {
                    //值改变时
                    //将渲染后的值重新赋值给单元格值
                    var reg = /^\d+(\.\d{0,2})?$/;
                    // reg.test(event.target.value);
                    if (reg.test(event.target.value) == true) {
                      params.row.Volume = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      event.target.value = "";
                      params.row.Volume = "";
                      vm.details.detailitem[params.index] = params.row;
                    }
                  }
                }
              });
            }
          },
          {
            title: "毛重(kg)",
            key: "Weight",
            align: "center",
            // width: 60,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: params.row.Weight,
                  autofocus: true
                },
                on: {
                  "on-change"(event) {},
                  "on-blur"(event) {
                    var reg = /^\d+(\.\d{0,2})?$/;
                    var newtarget = vm.trim(event.target.value);
                    if (newtarget != "") {
                      if (reg.test(newtarget) == true) {
                        params.row.Weight = newtarget;
                        vm.details.detailitem[params.index] = params.row;
                      } else {
                        vm.$Message.error("只能输入数字,包括两位数的小数点");
                        params.row.Weight = "";
                        event.target.value = "";
                        vm.details.detailitem[params.index] = params.row;
                      }
                    }
                  },
                  "on-enter": event => {
                    var reg = /^\d+(\.\d{0,2})?$/;
                    // console.log(event.target.value)
                    var newtarget = vm.trim(event.target.value);
                    // console.log(newtarget)
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget;
                      vm.details.detailitem[params.index] = params.row;
                      var Input = params.row.Input;
                      var StandardProducts = params.row.Product;
                      var data2 = {
                        Quantity: params.row.Quantity, //数量
                        inputsID: InputID, //id
                        Catalog: StandardProducts.Catalog, //品名
                        PartNumber: StandardProducts.PartNumber, //型号
                        Manufacturer: StandardProducts.Manufacturer, //品牌
                        Packing: StandardProducts.Packing, //包装
                        PackageCase: StandardProducts.PackageCase, //封装
                        origin: Input.OriginDescription //产地
                      };
                      var newdata = [];
                      newdata.push(data2);
                      var configs = GetPrinterDictionary();
                      var getsetting = configs["产品标签"];
                      // var href=window.location.protocol+"//"+window.location.host;
                      // var newurl="http://hv.warehouse.b1b.com"+getsetting.Url;
                      // getsetting.Url=newurl;
                      var str = getsetting.Url;
                      var testurl = str.indexOf("http") != -1;
                      if (testurl == true) {
                        getsetting.Url = getsetting.Url;
                      } else {
                        getsetting.Url = this.printurl + getsetting.Url;
                      }
                      var data = {
                        setting: getsetting,
                        data: newdata
                      };
                      TemplatePrint(data);
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      params.row.Weight = "";
                      event.target.value = "";
                      vm.details.detailitem[params.index] = params.row;
                    }
                  }
                }
              });
            }
          },
          {
            title: "图片",
            slot: "imglist",
            align: "center",
            width: 190
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 210
          }
        ],
        data1: [
          {
            iscx: true,
            ID: "1",
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "5000", //应到
            Alreadyarrived: "500", //已到
            Quantity: "50", //本次到货数量
            StockCode: "558", //库位号
            Country_origin: "美国", //原产地
            volume: "50", //体积
            GrossWeight: "50000",
            uploadimg: []
          },
          {
            iscx: true,
            ID: "2",
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "1000",
            Alreadyarrived: "200",
            Quantity: "20",
            StockCode: "258",
            Country_origin: "香港",
            volume: "50", //体积
            GrossWeight: "50000",
            uploadimg: []
          },
          {
            iscx: true,
            ID: "3",
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "33333",
            Shouldarrive: "33333",
            Alreadyarrived: "3333",
            Quantity: "3333",
            StockCode: "33333",
            Country_origin: "北京",
            volume: "50", //体积
            GrossWeight: "50000",
            uploadimg: []
          },
          {
            iscx: true,
            ID: "4",
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "1000",
            Alreadyarrived: "200",
            Quantity: "20",
            StockCode: "258",
            Country_origin: "香港",
            volume: "50", //体积
            GrossWeight: "50000",
            uploadimg: []
          }
        ],
        detailitem: [],
        fathertype: "" //调用拍照设备的父组件类型
      },
      uploadList: [],
      files: "",
      SelectRow: [], //多选 选择的列表
      isAbnormal: false, //是否异常到货
      remarks: "", //备注
      Reason: "外观损坏", //异常原因
      errordata: [
        //异常原因列表
        {
          value: "外观损坏",
          label: "外观损坏"
        },
        {
          value: "产品数量不相符",
          label: "产品数量不相符"
        },
        {
          value: "产品型号不相符",
          label: "产品型号不相符"
        },
        {
          value: "参数不相符（批次/产地）",
          label: "参数不相符（批次/产地）"
        },
        {
          value: "包装受潮严重",
          label: "包装受潮严重"
        },
        {
          value: "其他",
          label: "其他"
        }
      ],
      Summary: {}, //后台提供的备注信息对象
      historydata: false, //历史到货的抽屉
      historydetail: {
        //历史到货数据
        times: "", //时间，每次获取新的版本
        waybillLIst: [] //运单列表
      },
      company: "", //入仓号对应公司
      CarrierList: [], //承运商列表
      filterarr: [],
      Driversarr: [], //司机列表
      CarArr: [] //车牌号列表
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
    },
    changenumber: function(val) {
      //
      // console.log(val)
      if (val != "") {
        var newnumber = Number(val);
        console.log(newnumber);
        return newnumber;
      }
    }
  },
  computed: {
    getarrs() {
      return this.Nomatching;
    },
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail;
    }
  },
  mounted() {
    // this.search_pro()
    // this.WayParterdata()
    //  window['lili'].cc()
  },
  created() {
    // console.log("重新加载-------常规分拣");
    // this.WayParterdata()
    this.Carriers();
     if ( this.settimeouts ) {
     clearTimeout(this.settimeouts);
    }
    if ( this.setIntervaltimer ) {
     clearTimeout(this.setIntervaltimer);
    }
    window["PhotoUploaded"] = this.changed;
  },
  beforeRouteUpdate (to, from, next) {
    console.log(to)
    console.log(from)
    console.log(next)
    next()
  },
  // beforeDestroy(){
  //   delete window["PhotoUploaded"]
  // },
  methods: {
    clicktest: lodash.throttle(function(paramsrow) {
      //修改数据，触发改变inputid 的方法，修改对应数据的inputid
      var rows = paramsrow;
      var arr = this.details.detailitem;
      GetInputID().then(res => {
        if (res) {
          var newinputid = res.id;
          for (var i = 0; i < arr.length; i++) {
            if (arr[i].InputID == rows.InputID) {
              arr[i].InputID = newinputid;
              arr[i].Input.ID = newinputid;
              arr[i].Input.Code = newinputid;
            }
          }
        }
      });
    }, 1000),
    trim(str) {
      //去除前后空格
      return str.replace(/(^\s*)|(\s*$)/g, "");
    },
    ok() {
      this.$Message.info("Clicked ok");
      this.showphoto2 = false;
    },
    cancel() {
      this.$Message.info("取消");
      this.showphoto2 = false;
      this.sethousbox = false;
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      this.SelectRow = value;
      //  console.log(this.SelectRow)
    },
    search_pro() {
      console.log(this.searchkey);

      var warehouseID = sessionStorage.getItem("UserWareHouse");
      search_detail(
        warehouseID,
        this.details.wayBillID,
        encodeURI(this.searchkey)
      ).then(res => {
        console.log(res);
        if (res.obj != "") {
          // this.details.total = res.obj.Total;
          // this.details.waybillinfo = res.waybill;
          this.details.detailitem = res.obj.Notices;
        }
      });
    },
    finish_btn() {
      var isnull = null; //判断实际到货数量 true 为空 false 不为空
      var that = this;
      var dataarr = this.details.detailitem;
      var null_num = 1;
      for (var i = 0; i < dataarr.length; i++) {
        if (dataarr[i].TruetoQuantity == "") {
          isnull = true;
          null_num++;
        } else {
          isnull = false;
          null_num = 1;
          // null_num=0
        }
      }

      // console.log(null_num)
      // if(isnull!=1){
      //   this.$Message.error("请填写实际到货数量");
      // }
      function sum(arr) {
        return eval(arr.join("+"));
      }
      var arr = dataarr;
      var Nomatching = [];
      var map = {},
        dest = [];
      for (var i = 0; i < arr.length; i++) {
        var ai = arr[i];
        if (!map[ai.PID]) {
          dest.push({
            name: ai.Product.PartNumber,
            Quantity: ai.Quantity,
            PID: ai.PID,
            data: [Number(ai.TruetoQuantity)]
          });
          map[ai.PID] = ai.PID;
        } else {
          for (var j = 0; j < dest.length; j++) {
            var dj = dest[j];
            if (dj.PID == ai.PID) {
              dj.data.push(Number(ai.TruetoQuantity));
              break;
            }
          }
        }
      }

      for (var i = 0; i < dest.length; i++) {
        var item = dest[i];
        var total = sum(item.data);
        console.log(total);
        if (item.Quantity < total || item.Quantity > total) {
          Nomatching.push(item.name);
          // console.log("实际分拣数量大于通知数量"+item.name+" 总数 "+total)
        } else {
          // console.log(item.name+" 的数量正确  "+total)
        }
      }
      that.Nomatching = Nomatching;
      that.WarehousingMsg = true;
    },
    setWarehousing(data) {
      //确定入库，调取后台入库接口
      sortingupload(data).then(res => {
        if (res.val == 0) {
          //成功之后调用父组件的关闭方法
          if (this.details.waybillinfo.Source == 40) {
            this.$Message.success("该订单为转运订单，两秒后跳转至出库处理页面");
            var that = this;
            setTimeout(function() {
              that.toout_btn(res.id);
              // that.toout_btn('Waybill201912210018');
            }, 2000);
          } else {
            if (this.fatherMethod) {
              this.$Message.success("操作完成，一秒后自动关闭");
              var that = this;
              setTimeout(function() {
                that.fatherMethod();
              }, 1000);
            }
          }
        } else if (res.val == 400) {
          if (this.fatherMethod) {
            this.$Message.error("此订单已被关闭，请处理其他订单");
            var that = this;
            setTimeout(function() {
              that.fatherMethod();
            }, 2000);
          }
        } else if (res.val == 500) {
          this.$Message.error("请输入数量，数量不能为空");
        } else {
          this.$Message.error("入库操作失败,请从新操作");
        }
      });
    },
    getCookie(cookieName) {
      var strCookie = document.cookie;
      var arrCookie = strCookie.split("; ");
      for (var i = 0; i < arrCookie.length; i++) {
        var arr = arrCookie[i].split("=");
        if (cookieName == arr[0]) {
          return arr[1];
        }
      }
      return "";
    },
    ok_Warehousing() {
      //点击确定按钮，进行入库操作
      //  this.Summary.Title=this.Reason
      console.log(this.details.detailitem);
      this.details.waybillinfo.Notices = this.details.detailitem;
      var uploaddata = {
        obj: JSON.stringify(this.details.waybillinfo),
        Status: 120, //200  120
        token: this.getCookie("ydcx_Yahv.Erp"),
        Summary: ""
        // Summary:JSON.stringify(this.Summary)
      };
      this.setWarehousing(uploaddata);

      this.WarehousingMsg = false;
    },
    cancel_Warehousing() {
      //点击取消按钮，取消入库
      this.WarehousingMsg = false;
    },
    changeimgs(newdata, row) {
      //上传照片
      //通过子组件传递的数据改变父组件的值
      //  console.log(newdata)
      var arr = this.details.detailitem;
      for (var j = 0, arrlien = arr.length; j < arrlien; j++) {
        if (arr[j].ID == row.ID) {
          arr[j].Files.push(newdata);
        }
      }
      // console.log(this.details.detailitem)
    },
    chaixiang(index, row) {
      //拆箱分拣
      var newinputid = "";
      GetInputID().then(res => {
        console.log(res);
        newinputid = res.id;
        // var newdata = {
        //   iscx: false,
        //   Enabled: true, //判断是够可修改，是否是从Sorted 中获取而来
        //   BoxCode: row.BoxCode,
        //   CheckValue: row.CheckValue,
        //   IsOriginalNotice:false, 
        //   Condition: {
        //     AttachLabel: row.Condition.AttachLabel,
        //     CheckNumber: row.Condition.CheckNumber,
        //     DevanningCheck: row.Condition.DevanningCheck,
        //     OnlineDetection: row.Condition.OnlineDetection,
        //     PaintLabel: row.Condition.PaintLabel,
        //     PickByValue: row.Condition.PickByValue,
        //     Repacking: row.Condition.Repacking,
        //     Weigh: row.Condition.Weigh
        //   },
        //   DateCode: row.DateCode,
        //   Files: [],
        //   ID: "CX" + index + new Date().getTime(),
        //   PID: row.ID, //父ID
        //   WareHouseID: row.WareHouseID,
        //   WayBillID: row.WayBillID,
        //   ProductID: row.ProductID,
        //   InputID: newinputid,
        //   ClientID: row.ClientID,
        //   Input: {
        //     ClientID: row.Input.ClientID,
        //     ClientName:row.Input.ClientName,
        //     Code: row.Input.Code,
        //     CreateDate: row.Input.CreateDate,
        //     Currency: row.Input.Currency,
        //     DateCode: row.Input.DateCode,
        //     EnterpriseID: row.Input.EnterpriseID,
        //     ID: row.Input.ID,
        //     ItemID: row.Input.ItemID,
        //     OrderID: row.Input.OrderID,
        //     Origin: row.Input.Origin,
        //     OriginID: row.Input.OriginID,
        //     OriginDescription: row.Input.OriginDescription,
        //     ProductID: row.Input.ProductID,
        //     PurchaserID: row.Input.PurchaserID,
        //     PayeeID:row.Input.PayeeID,
        //     SalerID: row.Input.SalerID,
        //     ThirdID:row.Input.ThirdID,
        //     TrackerID: row.Input.TrackerID,
        //     UnitPrice: row.Input.UnitPrice,
        //     TinyOrderID:row.Input.TinyOrderID
        //   },
        //   Product: {
        //     CreateDate: row.Product.CreateDate,
        //     ID: row.Product.ID,
        //     Manufacturer: row.Product.Manufacturer,
        //     PackageCase: row.Product.PackageCase,
        //     Packaging: row.Product.Packaging,
        //     PartNumber: row.Product.PartNumber
        //   },
        //   Quantity: row.Quantity,
        //   ShelveID: row.ShelveID,
        //   SortedQuantity: row.SortedQuantity,
        //   Source: row.Source,
        //   TruetoQuantity: row.TruetoQuantity,
        //   Volume: row.Volume,
        //   WayBillID: row.WayBillID,
        //   Weight: row.Weight
        // };
         var newdata = {
          iscx: false,
          Enabled: true, //判断是够可修改，是否是从Sorted 中获取而来
          BoxCode: row.BoxCode,
          CheckValue: row.CheckValue,
          IsOriginalNotice:false, 
          Condition: row.Condition,
          DateCode: row.DateCode,
          Files: [],
          ID: "CX" + index + new Date().getTime(),
          PID: row.ID, //父ID
          WareHouseID: row.WareHouseID,
          WayBillID: row.WayBillID,
          ProductID: row.ProductID,
          InputID: newinputid,
          ClientID: row.ClientID,
          Input: row.Input,
          Product: row.Product,
          Quantity: row.Quantity,
          ShelveID: row.ShelveID,
          SortedQuantity: row.SortedQuantity,
          Source: row.Source,
          TruetoQuantity: row.TruetoQuantity,
          Volume: row.Volume,
          WayBillID: row.WayBillID,
          Weight: row.Weight
        };
        newdata.Input.ID=newinputid;
        newdata.Input.Code=newinputid
        console.log(newdata)
        this.details.detailitem.splice(index + 1, 0, newdata);
      });
    },
    removechaixiang(index) {
      //移除拆项
      this.details.detailitem.splice(index, 1);
      //  console.log(this.details.detailitem)
    },
    getdetail_data(id) {
      //初始化数据
      if (id != "") {
        this.details.wayBillID = id;
        var datas = {
          waybillid: this.details.wayBillID
        };
        this.History(datas);
        // http://hv.warehouse.b1b.com/ApiWms/Notice?warehouseID=HK01&waybillid=Waybill201910120006
        var data = {
          warehouseID: sessionStorage.getItem("UserWareHouse"),
          wayBillID: this.details.wayBillID,
          pageIndex: this.details.pageIndex,
          pageSize: this.details.pageSize
        };
        Noticedetail(data).then(res => {
          // console.log(res);
          // this.details.total = res.obj.Total;
          var obj = res.obj.Notices;
          var objlength = obj.length;
          console.log(res.obj.Notices);
          var newnotice = res.obj.Notices;
          for (var i = 0; i < newnotice.length; i++) {
            if (newnotice[i].Sorted.length > 0) {
              var Sorteds = newnotice[i].Sorted;
              for (var j = 0; j < Sorteds.length; j++) {
                const key = "iscx";
                const value = false;
                Reflect.set(Sorteds[j], key, value);
                var newSorted = Sorteds[j];
                newnotice.splice(i + 1, 0, newSorted);
              }
            } else {
            }
            // this.details.detailitem = res.obj.Notices;
          }
          this.details.detailitem = newnotice;
          this.details.waybillinfo = res.obj;
          this.GetUsableShelves(); //调用可用库位
          this.loading = false;

          for (i in res.obj.Conditions) {
            if (res.obj.Conditions[i] == true) {
              this.Conditionstype = true;
              break;
            } else {
              this.Conditionstype = false;
            }
          }

          for (var i = 0; i < objlength; i++) {
            //为清单打印保留原始数据
            var item = {
              PartNumber: obj[i].Product.PartNumber, //型号
              Manufacturer: obj[i].Product.Manufacturer, //品牌
              Quantity: obj[i].Quantity, //应到
              SortedQuantity: obj[i].SortedQuantity, //已到
              DateCode: obj[i].Input.DateCode, //批次
              TruetoQuantity: "", //本次到货
              Origin: obj[i].Input.OriginDescription, //原产地
              ShelveID: obj[i].ShelveID, //入库库位
              Volume: obj[i].Volume, //体积
              Weight: obj[i].Weight //毛重
            };
            this.printlist.push(item);
          }
          // console.log(this.printlist)
        });
      }
    },
    detailspage(value) {
      //分页
      // var data={
      //   wayBillID:this.details.wayBillID,
      //   pageIndex:this.details.pageIndex,
      //   pageSize:this.details.pageSize,
      // }
      console.log(this.details.wayBillID);
      this.details.pageIndex = value;
      this.getdetail_data(this.details.wayBillID);
    },
    handleBeforeUpload(file) {
      // 创建一个 FileReader 对象
      let reader = new FileReader();
      // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
      // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
      // 读取文件作为 URL 可访问地址
      reader.readAsDataURL(file);
      const _this = this;
      reader.onloadend = function(e) {
        file.url = reader.result;
        var newimg = {
          AdminID: "",
          ClientID: "",
          CreateDate: "",
          CustomName: file.name,
          FileBase64Code: file.url,
          ID: "",
          InputID: "",
          LocalFile: "",
          Status: 0,
          StatusDes: "",
          StorageID: "",
          Type: 0,
          TypeDes: "",
          Url: "",
          WaybillID: ""
        };
        _this.details.waybillinfo.Files.push(newimg);
      };
      return false;
    },
    handleRemove(file) {
      this.details.waybillinfo.Files.splice(
        this.details.waybillinfo.Files.indexOf(file),
        1
      );
    },
    handleFormatError(file) {
      this.$Notice.warning({
        title: "文件格式不正确",
        desc:
          "文件 " + file.name + " 格式不正确，请上传 jpg 或 png 格式的图片。"
      });
    },
    handleMaxSize(file) {
      this.$Notice.warning({
        title: "超出文件大小限制",
        desc: "文件 " + file.name + " 太大，不能超过 2M。"
      });
    },
    handleRemovelist(row, file) {
      var arr = this.details.detailitem;
      for (var j = 0; j < arr.length; j++) {
        //删除指定下标 的元素
        if (arr[j].ID == row.ID) {
          arr[j].Files.splice(file, 1);
        }
      }
    },

    //拍照方法的调用
    closephoto() {
      //关闭按钮 父组件关闭子组件摄像头
      this.$refs.photograph.closeCamera();
      var that = this;
      setTimeout(function() {
        that.showphoto2 = false;
      }, 20);
    },
    upload_btn() {
      //上传图片
      this.$refs.photograph.uploadphoto();
      // this.$refs.photograph.closeCamera();
      var that = this;
      setTimeout(function() {
        that.showphoto2 = false;
      }, 20);
    },
    photoing(type) {
      //打开拍照组件
      console.log(type);
      this.fathertype = type;
      this.showphoto2 = true;
      // 一. 通过时间戳加载，会调用初始化数据
      // this.time = new Date().getTime()

      //  二。将子组件初始值设为空
      this.$refs.photograph.closeCamera();
      this.$refs.photograph.list = [];
      this.$refs.photograph.model1 = "";
      this.$refs.photograph.setCamera();
      this.$refs.photograph.callCamera();

      //  三. 初始值加载后不在加载初始值
      // if(this.$refs.photograph.model1==""){
      //   this.$refs.photograph.setCamera()
      //   this.$refs.photograph.callCamera();
      // }else{
      //   this.$refs.photograph.callCamera();
      // }
    },
    changupload(naedata, type) {
      //拍照
      //将拍的照片传到更新到父组件
      // console.log(this.fathertype);
      if (this.fathertype == "waybill") {
        //运单照片列表
        this.details.waybillinfo.Files.push(naedata);
      } else {
        //
        var arr = this.details.detailitem;
        for (var j = 0; j < arr.length; j++) {
          if (arr[j].ID == this.fathertype.ID) {
            arr[j].Files.push(naedata);
          }
        }
      }
    },
    //查询可用库位的库位编号
    GetUsableShelves() {
      var id = sessionStorage.getItem("UserWareHouse");
      GetUsableShelves(id, this.details.waybillinfo.ClientID).then(res => {
        // console.log(res)
        this.Storehouselist = res.obj;
      });
    },

    // 一键入库
    SetStorehouse() {
      if (this.SelectRow.length <= 0) {
        this.$Message.error("请选择要操作的产品项");
      } else {
        // console.log(this.SelectRow)
        this.sethousbox = true;
      }
    },
    //一键入库后确认改变库位
    changehouse() {
      // this.sethousbox=false;
      // console.log(this.SelectRow);
      var SelectRows = this.SelectRow;
      var detaiitem = this.details.detailitem;
      for (var i = 0; i < this.details.detailitem.length; i++) {
        for (var j = 0; j < SelectRows.length; j++) {
          if (this.details.detailitem[i].ID == SelectRows[j].ID&&this.details.detailitem[i].Enabled==true) {
            this.details.detailitem[i].ShelveID = this.housenumber;
          }
        }
      }
      this.SelectRow = [];
      this.sethousbox = false;
      this.$refs.selection.selectAll(false);
    },
    isAbnormalclick() {
      this.isAbnormal = true;
    },
    //到货异常 确认按钮
    Abnormal_btn() {
      // console.log(this.Summary.Summary)
      if (this.Summary.Summary == undefined || this.Summary.Summary == "") {
        this.isAbnormal = true;
        this.$Message.error("请输入异常原因");
        // console.log(this.isAbnormal)
      } else {
        this.details.waybillinfo.Notices = this.details.detailitem;
        this.Summary.Title = this.Reason;
        var uploaddata = {
          obj: JSON.stringify(this.details.waybillinfo),
          // Summary:JSON.stringify(this.Summary),
          token: this.getCookie("ydcx_Yahv.Erp"),
          Summary: this.Summary.Summary,
          Status: 130 //300
        };
        if (this.details.detailitem.length > 0) {
          this.setWarehousing(uploaddata); //接口
        } else {
          this.$Message.error("暂无数据");
        }
        this.isAbnormal = false;
      }
    },
    closeerror() {
      //异常到货关闭
      this.Summary.Summary = ""; //备注
      this.Reason = "外观损坏"; //异常原因
      this.isAbnormal = false;
    },
    changeerror(value) {
      // console.log(this.isAbnormal)
      if (value == true) {
        this.isAbnormal = true;
      } else {
        this.isAbnormal = false;
        this.Summary.Summary = ""; //备注
      }
    },
    Labelprint() {
      //标签打印 选中多个
      var arr = this.SelectRow;
      var printsrr = [];
      if (arr.length <= 0) {
        this.$Message.error("请选择要操作的产品项");
      } else {
        for (var i = 0; i < arr.length; i++) {
          var Inputs = arr[i].Inputs;
          var StandardProducts = arr[i].StandardProducts;
          var obj = {
            Quantity: arr[i].Quantity, //数量
            inputsID: arr[i].InputID, //id
            Catalog: arr[i].Product.Catalog, //品名
            PartNumber: arr[i].Product.PartNumber, //型号
            Manufacturer: arr[i].Product.Manufacturer, //品牌
            Packing: arr[i].Product.Packing, //包装
            PackageCase: arr[i].Product.PackageCase, //封装
            origin: arr[i].Input.OriginDescription //产地
          };
          printsrr.push(obj);
        }
        var configs = GetPrinterDictionary();
        var getsetting = configs["产品标签"];
        // var href=window.location.protocol+"//"+window.location.host;
        // var newurl="http://hv.warehouse.b1b.com"+getsetting.Url
        // getsetting.Url=newurl

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
    showhistory(name) {
      //展示历史订单组件
      this.historydata = true;
      this.historydetail.times = new Date().getTime();
      var namenew=name.trim().split(/\s+/)
      // console.log(name.trim().split(/\s+/))
      var that = this;
      var data = {
        waybillid: namenew[0],
      };
      setTimeout(function() {
        that.$refs.Historygoods.gethistory(data);
      }, 20);
    },
    historywaybill(name) {
      //显示历史到货详情数据
      // http://hv.warehouse.b1b.com/wmsapi/sortings/Historydetail?waybillid=Waybill201910120021&orderid=Order201909290013
      var data = {
        waybillid: this.details.wayBillID,
        orderid: name
      };
      // this.showhistory(data)
    },
    WayParterdata() {
      //输送地列表
      getWayParter().then(res => {
        this.Conveyingplace = res.obj;
      });
    },
    // 承运商列表
    Carriers() {
      Carriers().then(res => {
        this.CarrierList = res.obj;
      });
    },
    detailelist() {
      //清单打印
      if (this.printlist.length > 0) {
        var obj = [
          {
            ID: "0003",
            url: "",
            size: {
              width: "595",
              height: "842"
            },
            data: {
              waybill: this.details.waybillinfo,
              listdata: this.printlist
            }
          }
        ];
        // var newdata=JSON.stringify(obj)
        var configs = GetPrinterDictionary();
        var printsrr = {
          waybill: this.details.waybillinfo,
          listdata: this.printlist
        };
        var getsetting = configs["清单打印"];
        // var newurl="http://hv.warehouse.b1b.com"+getsetting.Url
        // getsetting.Url=newurl
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
              waybill: this.details.waybillinfo,
              listdata: this.printlist
            }
          ]
        };
        TemplatePrint(data);
      }
    },
    changConveyingplace(value, row,index) {
      //改变库位号
      console.log(value)
      console.log(row)
      console.log(index)
      if(value!=undefined){
        this.details.detailitem[index].ShelveID=value.value;
      }
      // this.details.detailitem.forEach(item => {
      //   if (item.ID == row.ID) {
      //     console.log(item.ID)
      //     if (value != undefined) {
      //       item.ShelveID = value.value;
      //     }
      //   }
      // });
    },
    showBudget() {
      //收支明细展开
      // this.Budgetdetail=true;

      this.$store.dispatch("setBudget", true);
      this.$router.push({
        name: "ends_meet",
        params: {
          webillID: this.details.waybillinfo.WaybillID,
          otype:"in",
        }
      });
    },
    closeBudget() {
      //收支明细关闭
      this.$router.go(-1);
      // this.Budgetdetail=false;
      this.$store.dispatch("setBudget", false);
    },
    History(data) {
      //历史运单
      History(data).then(res => {
        //  console.log(res)
        this.historydetail.waybillLIst = res.obj;
      });
    },
    clickClient(value, type,index) {
      //显示更改输送地与原产地的方法
      console.log(value)
      console.log(type)
      console.log(index)

      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = String(value);
      this.ConveyingplaceID=index;
    },
    changClientCode(value) {
      //地址改变的时候,保留改变后的地址
      // console.log(value)
      // console.log(this.chengevalue)
      this.chengevalue.inputval = value;
    },
    primaryClientCode() {
      //确认更改地址
      console.log(this.chengevalue)
      if (this.chengevalue.type == "ClientCode") {
        this.details.waybillinfo.Place = this.chengevalue.inputval.value;
        this.details.waybillinfo.PlaceDescription = this.chengevalue.inputval.label;
      } else if(this.ConveyingplaceID!='ClientCode'){
        this.chengevalue.type.Input.OriginDescription = this.chengevalue.inputval.label;
        this.details.detailitem[this.ConveyingplaceID].Input.Origin = this.chengevalue.inputval.value;
        this.details.detailitem[this.ConveyingplaceID].Input.OriginDescription = this.chengevalue.inputval.label;
        // var newNotices = this.details.detailitem;
        // for (var i = 0, l = this.details.detailitem.length; i < l; i++) {
        //   if (this.chengevalue.type.ID == newNotices[i].ID) {
        //     this.details.detailitem[i].Input.Origin = this.chengevalue.inputval.value;
        //     this.details.detailitem[ i].Input.OriginDescription = this.chengevalue.inputval.label;
        //     break;
        //   }
        // }
      }
      this.setClientCode = false;
    },
    // fileprint(){
    //     var configs=GetPrinterDictionary()
    //     var getsetting=configs['文档打印']
    //     getsetting.Url="https://www.mouser.cn/datasheet/2/18/Amphenol_02192019_AAS-920-729A-Thermometrics-NTC_T-1534465.pdf";
    //     var data={
    //           setting:getsetting,
    //           data:null,
    //         }
    //     FilePrint(data);
    // },
    fileprint(printurl) {
      var configs = GetPrinterDictionary();
      var getsetting = configs["文档打印"];
      getsetting.Url = printurl;
      var data = getsetting;
      FilePrint(data);
    },
    imgprint() {
      var configs = GetPrinterDictionary();
      var getsetting = configs["图片打印"];
      var str = getsetting.Url;
      var testurl = str.indexOf("http") != -1;
      if (testurl == true) {
        getsetting.Url = getsetting.Url;
      } else {
        getsetting.Url = this.printurl + getsetting.Url;
      }
      var datas = {
        setting: getsetting,
        data: [
          {
            src: "C:\\Users\\Administrator\\Pictures\\test1.png"
          }
        ]
      };
      FilePrint(datas);
    },
    fromphotos(type) {
      //拍照
      if (type == "Waybill") {
        var data = {
          SessionID: this.details.waybillinfo.WaybillID,
          AdminID: sessionStorage.getItem("userID")
        };
        FormPhoto(data);
      } else {
        var data = {
          SessionID: type,
          AdminID: sessionStorage.getItem("userID")
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
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      if (imgdata.SessionID == this.details.waybillinfo.WaybillID) {
        this.details.waybillinfo.Files.push(newfile);
      } else {
        // alert(JSON.stringify(imgdata))
        // this.details.detailitem[imgdata.SessionID].Files.push(newfile);
        for (var i = 0; i < this.details.detailitem.length; i++) {
          if (this.details.detailitem[i].InputID == imgdata.SessionID) {
            this.details.detailitem[i].Files.push(newfile);
          }
        }
      }
    },
    SeletUpload(ID) {
      // 传照
      var data = {
        SessionID: ID,
        AdminID: sessionStorage.getItem("userID")
      };
      SeletUploadFile(data);
    },
    TakeGoods() {
      //我去提货
      var data = {
        waybillid: this.details.waybillinfo.WaybillID
      };
      TakeGoods(data).then(res => {
        if (res.val == 1) {
          this.details.waybillinfo.ExcuteStatusDescription = "正在提货中";
          this.TakeGoodsName = "正在提货";
        } else {
          this.$Message.error("提货锁定失败");
        }
      });
    },
    search_filter() {
      //前台实现搜索查询功能
      // if(this.searchkey==""){
      //      this.$Message.error("查询数据不能为空");
      // }else{
      //   $.grep(this.details.detailitem, function(n,i){
      //     if(row.Product.PartNumber==this.searchkey||row.Product.Manufacturer==this.searchkey){
      //        this.rowClassName(n,i)
      //     }
      //   });
      // }
    },
    rowClassName(row, index) {
      if (this.searchkey != "") {
        // return 'demo-table-info-row';
        if (
          row.Product.PartNumber.indexOf(this.searchkey.toUpperCase()) !== -1 ||
          row.Product.PartNumber.indexOf(this.searchkey.toLowerCase()) !== -1 ||
          row.Product.Manufacturer.indexOf(this.searchkey.toUpperCase()) !== -1 ||
          row.Product.Manufacturer.indexOf(this.searchkey.toLowerCase()) !== -1
        ) {
          return "";
        } else {
          return "demo-table-info-row";
        }
      } else {
        return "";
      }
    },
    toout_btn(id) {
      this.$store.dispatch("setshowdetail", false);
      this.$store.dispatch("setshowdetailout", true);
      this.$router.push({ path: "/Outgoing/outdetail/" + id });
      this.$store.dispatch("setshowtype", 1);
      sessionStorage.setItem("Activename","出库管理");
      this.$store.dispatch("setAcrivename",'出库管理');
    },
    PictureShow(url) {
      //图片展示
      var data = {
        Url: url
      };
      PictureShow(data);
    }
    // rowclick(row,index){
    //    $(window).keypress(function (e) {
    //             if (e.keyCode == 13) {
    //                 if (tempstring.length == 7 && /\s.*\d\.\d/ig.test(tempstring)) {

    //                     //在点击一行数据后(本例是div)，可以把当前的称重字段的作为称重的填充目标
    //                     this.details.detailitem[index].Weight=tempstring;
    //                     //$('.cz').filter(function () {
    //                     //    return $(this).val() == "";
    //                     //}).first().val($.trim(tempstring));

    //                 }
    //                 tempstring = '';
    //             }
    //             tempstring += e.key;
    //             if (tempstring.length > 7) {
    //                 tempstring = tempstring.substring(tempstring.length - 7);
    //             }
    //         });
    // }
  }
};

// window ['PhotoUploaded'] = function(message){
//   alert(JSON.stringify(message));
//   getuplode=message;
//   alert(JSON.stringify(this))
// };
// PhotoUploaded("fjsdhfsdjkhfksjdh")
// window['Zyz'] =function(data){
//   // alert(JSON.stringify(data))
//   getuplode=data;
//   // alert(JSON.stringify(getuplode))
// };
</script>
