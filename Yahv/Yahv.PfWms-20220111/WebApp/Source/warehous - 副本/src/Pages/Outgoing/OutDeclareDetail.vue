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
  width: 100px;
}
.detail_title3 {
  display: inline-block;
  width: 80px;
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
</style>
<template>
  <div>
    <div style="width:100%;">
      <div class="itembox">
        <p class="detailtitle">基础信息</p>
        <!-- <p>{{getids}}</p> -->
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row>
            <Col style="width: 20%;float: left;">
              <ul class="detail_li detail1">
                <li class="itemli">
                  <span class="detail_title1">ID：</span>
                  <span>{{waybillinfo.WaybillID}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title1">状态：</span>
                  <span>{{waybillinfo.ExcuteStatusDescription}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title1">业务类型：</span>
                  <span>{{waybillinfo.SourceDescription}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title1">总货值：</span>
                  <span style="color:'#ccc';font-width:600">{{waybillinfo.TotalGoodsValue}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.Conditions!=undefined">
                  <Icon type="md-alert" style="font-size: 22px;color: #da2828;" />
                  <Tag color="geekblue" v-if="waybillinfo.Conditions.AgencyCheck==true">代检查</Tag>
                  <Tag color="purple" v-if="waybillinfo.Conditions.AgencyPayment==true">代垫货款</Tag>
                  <Tag color="blue" v-if="waybillinfo.Conditions.AgencyReceive==true">代收货款</Tag>
                  <Tag color="cyan" v-if="waybillinfo.Conditions.ChangePackaging==true">代收货款</Tag>
                  <Tag color="green" v-if="waybillinfo.Conditions.LableServices==true">标签服务</Tag>
                  <Tag color="gold" v-if="waybillinfo.Conditions.PayForFreight==true">垫付运费</Tag>
                  <Tag color="orange" v-if="waybillinfo.Conditions.Repackaging==true">重新包装</Tag>
                  <Tag color="volcano" v-if="waybillinfo.Conditions.UnBoxed==true">拆箱验货</Tag>
                  <Tag color="red" v-if="waybillinfo.Conditions.VacuumPackaging==true">真空包装</Tag>
                  <Tag color="magenta" v-if="waybillinfo.Conditions.WaterproofPackaging==true">防水包装</Tag>
                </li>
              </ul>
            </Col>
            <Col style="width: 26%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title2">通知时间：</span>
                  <span>{{waybillinfo.CreateDate|showDate}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">供应商：</span>
                  <span>{{waybillinfo.Supplier}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">客户编号：</span>
                  <span>{{waybillinfo.EnterCode}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">包装要求</span>
                  <span>{{waybillinfo.Packaging}}</span>
                </li>
                <!-- <li class="itemli">
                  <span class="detail_title2">提货文件</span>
                  <span style="line-height: 20px;width: 70%;display: inline-block;">
                   <div v-for="(item,index) in waybillinfo.Files" v-if="item.type==24">
                      <a>{{item.CustomName}}</a>
                      <a @click="fileprint(item.Url)">打印</a>
                    </div>
                  </span>
                </li>-->
              </ul>
            </Col>
            <Col style="width: 31%;float: left;">
              <ul class="detail_li">
                <!-- <li class="itemli">
                  <span class="detail_title3">客户编号：</span>
                  <span>{{waybillinfo.EnterCode}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">运单号(本次)：</span>
                  <span >
                    <Input style="width:60%" v-model="waybillinfo.Code" />
                    <Icon type="md-add-circle" :size="18" color="#57a3f3" class="icon1" @click="showChildrencode=true" />
                  </span>
                </li>-->
                <!-- <li class="itemli">
                  <span class="detail_title3">承运商(本次):</span>
                  <span>
                    <Select v-model="waybillinfo.CarrierID" style="width:60%" @on-change="changeacrrier" :label-in-value="true">
                      <Option
                        v-for="item in CarrierList"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>-->
                <li class="itemli">
                  <span class="detail_title3">订单号：</span>
                  <span>{{waybillinfo.OrderID}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">随货文件：</span>
                  <div v-for="(item,index) in waybillinfo.Files" v-if="item.Type==25">
                    <span>{{item.CustomName}}</span>
                    <a @click="fileprint(item.Url)">打印</a>
                  </div>
                </li>
                <li class="itemli">
                  <span class="detail_title2">特殊标签：</span>
                  <span style="line-height: 20px;width: 70%;display: inline-block;">
                    <div v-for="(item,index) in waybillinfo.Files" v-if="item.Type==24">
                      <a>{{item.CustomName}}</a>
                      <a @click="fileprint(item.Url)">打印</a>
                    </div>
                  </span>
                </li>
              </ul>
            </Col>
            <Col style="width: 23%;float: left;">
              <ul class="detail_li" style="margin-left:20px;">
                <!-- <li class="itemli">
                  <span class="detail_title3">提货人证件：</span>
                  <span
                    v-if="waybillinfo.WayLoading!=undefined"
                  >{{waybillinfo.WayLoading.CarNumber1}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">提货人电话：</span>
                  <span
                    v-if="waybillinfo.WayLoading!=undefined"
                  >{{waybillinfo.WayLoading.TakingPhone}}</span>
                </li>-->
                <li class="itemli">
                  <span style="float:left;line-height: 27px;">发货情况照片：</span>
                  <div class="setupload">
                    <Button
                      type="primary"
                      size="small"
                      icon="ios-cloud-upload"
                      @click="SeletUpload(waybillinfo.WaybillID)"
                    >传照</Button>
                  </div>
                  <div class="setupload">
                    <Button
                      type="primary"
                      icon="md-reverse-camera"
                      @click="fromphotos('Waybill')"
                    >拍照</Button>
                    <!-- <Button type="primary" icon="ios-search" @click="photoing('waybill')">拍照</Button> -->
                  </div>
                </li>
                <li style="clear: both;">
                  <div v-for="(item,index) in waybillinfo.Files">
                    <div v-if="item.Type==8010">
                      <span>{{item.CustomName}}</span>
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
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
            <Input
              v-model="searchkey"
              placeholder="请输入品牌或型号"
              style="width:80%;float:left;position: relative;left: 3px"
            />
            <!-- <Button style="float:left" @click="search_pro" type="primary">筛选</Button> -->
            <Button style="float:left" type="primary">筛选</Button>
          </ButtonGroup>
          <Button type="primary" @click="detailelist">清单打印</Button>
          <Button type="primary" @click="Labelprint">标签打印</Button>
          <Button type="primary" @click="clickshowchangebox">一键入箱</Button>
          <Button type="primary" @click="showBudget">收支明细</Button>
          <div style="float:right">
           <Button
              type="primary"
              shape="circle"
              icon="md-checkmark"
              @click="finish_btn"
              :disabled="waybillinfo.ExcuteStatus==215?true:false"
            >完成分拣</Button>
            <Button
              :disabled="waybillinfo.ExcuteStatus==215?true:false"
              type="warning"
              shape="circle"
              icon="ios-alert-outline"
              @click="isAbnormal=true"
            >分拣异常</Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table
              ref="selection"
              :columns="tabletitle"
              :data="waybillinfo.Notices"
              @on-selection-change="handleSelectRow"
              :row-class-name="rowClassName"
            >
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <template slot-scope="{ row, index }" slot="PartNumber">
                <div style="display: flex;justify-content: space-between;align-items: center;">
                  <div style="width:120px;overflow: hidden;">
                    <span>{{row.Product.PartNumber}}</span>
                  </div>
               <ul style="float:right;">
                   <li><Tag v-if="row.Condition.IsCIQ==true" color="primary">商检</Tag></li>
                   <li><Tag v-if="row.Condition.IsCCC==true" color="warning">CCC</Tag></li>
                   <li><Tag v-if="row.Condition.IsEmbargo==true" color="error">禁运</Tag></li>
                   <li><Tag v-if="row.Condition.IsHighPrice==true" color="magenta">高价值</Tag></li> 
                </ul>
                </div>
                
              </template>
              <template slot-scope="{row,index}" slot="Boxing_code">
                <Dropdown trigger="click" :transfer="true" @on-click="clickDropdown($event,index)">
                  <a href="javascript:void(0)">
                    <span v-if="row.BoxCode!=null">{{row.BoxCode}}</span>
                    <span v-else>暂无箱号</span>
                    <Icon type="ios-arrow-down"></Icon>
                  </a>
                  <DropdownMenu slot="list">
                    <Button type="text" @click="Applybox=true">
                      <Icon type="md-add" />申请箱号
                    </Button>
                    <DropdownItem
                      v-for="(item,index) in boxingarr"
                      :name="item.Code"
                      :key="index"
                    >{{item.Code}}</DropdownItem>
                  </DropdownMenu>
                </Dropdown>
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="item in row.Files">
                  <span v-if="item.Type==8010">{{item.CustomName}}</span>
                  <Icon
                    type="ios-trash-outline"
                    :ref="row.ID"
                    @click.native="handleRemovelist(row,item)"
                  ></Icon>
                </p>
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button
                    type="primary"
                    size="small"
                    icon="ios-cloud-upload"
                    @click="SeletUpload(row.InputID)"
                  >传照</Button>
                </div>
                <div class="setupload">
                  <Button
                    type="primary"
                    icon="md-reverse-camera"
                    @click="fromphotos(row.InputID)"
                  >拍照</Button>
                </div>
                <div class="setupload" v-if="row.iscx!=false">
                  <!-- <Button type="primary" icon="md-checkmark" @click="chaixiang(index,row)">拆项</Button> -->
                </div>
                <div class="setupload" v-if="row.iscx==false">
                  <Button
                    :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    icon="ios-trash-outline"
                    @click="removechaixiang(index,row)"
                  >删除</Button>
                </div>
              </template>
            </Table>
           <div class="successbtn" style="margin-top:20px;">
             <!-- <Button
                type="primary"
                icon="md-checkmark"
                @click="testbtn"
              >出库完成测试</Button> -->
              <Button
                type="primary"
                icon="md-checkmark"
                @click="finish_btn"
                :disabled="waybillinfo.ExcuteStatus==215?true:false"
              >完成分拣</Button>
              <Button
                type="warning"
                icon="ios-alert-outline"
                @click="isAbnormal=true"
                :disabled="waybillinfo.ExcuteStatus==215?true:false"
              >分拣异常</Button>
            </div>
          </div>
        </div>
      </div>
    </div>
    <!-- 收支明细 开始 -->
    <Modal
      v-model="Budgetdetail"
      width="55%"
      :closable="false"
      :mask-closable="false"
      :footer-hide="true"
    >
      <div style="clear: both; overflow: hidden;font-size:16px;padding-bottom:10px">
        <Icon type="ios-close-circle" style="float:right" @click="closeBudget" />
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
          :value="item.ID"
          :key="item.ID"
        >{{ item.CorPlaceDes }}</Option>
      </Select>
      <div slot="footer">
        <Button>取消</Button>
        <Button type="primary" @click="primaryClientCode">确定</Button>
      </div>
    </Modal>
    <!-- 输送地列表与更改 结束-->
    <!-- 申请箱号 开始 -->
    <Modal
      v-model="Applybox"
      title="箱号申请"
      @on-visible-change="visiblechange"
      @on-ok="okapply"
      @on-cancel="cancel"
    >
      <div>
        <label>
          日期：
          <Select v-model="applyfrom.DateStr" style="width:80%">
            <Option v-for="(item,index) in applydata" :value="item" :key="index">{{ item }}</Option>
          </Select>
        </label>
      </div>
      <div style="margin-top:20px">
        <label>
          备注：
          <Input v-model="applyfrom.Summary" placeholder="请输入备注" style="width:80%" />
        </label>
      </div>
    </Modal>
    <!-- 申请箱号结束 -->

    <!-- 一键入箱 开始 -->
    <Modal
      v-model="showchangebox"
      title="选择箱号"
      @on-visible-change="changboxvisible"
      @on-ok="ok_changebox"
      @on-cancel="cancel"
    >
      <Select v-model="newboxcode">
        <Option v-for="item in boxingarr" :value="item.Code" :key="item.ID">{{ item.Code }}</Option>
      </Select>
    </Modal>
    <!-- 一键入箱 结束 -->
    <!-- 确认出库 开始 -->
     <div v-if="WarehousingMsg==true">
        <Modal
          v-model="WarehousingMsg"
          title="确定分拣"
          @on-ok="ok_Warehousing"
          @on-cancel="cancel_Warehousing"
        >
          <div>
            <span>是否确认分拣</span>
          </div>
        </Modal>
      </div>
      <!-- 确认出库 结束 -->
       <Modal v-model="isAbnormal" title="出库异常" @on-visible-change="errorstatue">
        <div slot="close">
          <Icon
            type="md-close"
            color="rgb(33, 28, 28)"
            @click="closeerror"
            style="font-size:18px;"
          />
        </div>
        <span style="line-height: 1; display: block; padding-bottom: 10px;">
          <em class="Mustfill">*</em>分拣异常原因
        </span>
        <Input v-model="Summary.Summary" type="textarea" :rows="2" placeholder="备注" />
        <div slot="footer">
          <Button @click="closeerror">取消</Button>
          <Button type="primary" @click="Abnormal_btn">确定</Button>
        </div>
      </Modal>
      <!-- 异常到货 结束-->
  </div>
</template>
<script>
// import imgtest from "@/Pages/Common/imgtes"
import {
  pickingsdetail,
  Carriers,
  WayParterdata,
  GetInputID,
  GetBoxesdates,
  Boxenter,
  pickingsenter,
  GetBoxes
} from "../../api";
import moment from "moment";
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint,
  FormPhoto,
  SeletUploadFile
} from "@/js/browser.js";
export default {
  name: "",
  components: {
    //  'img-test':imgtest
  },
  data() {
    return {
      SelectRow: [], //多选数组
      searchkey: "",
      boxingarr: [ ], //箱号
      printlist: [], //清单打印数据
      wayBillID: this.$route.params.detailID,
      detail_ID: "",
      showphoto: false, //显示拍照弹出框
      CarrierList: [], //承运商列表
      Conveyingplace: [], //输送地列表
      waybillinfo: {},
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
          type: "selection",
          width: 50,
          align: "center"
        },
        {
          title: " ",
          slot: "indexs",
          align: "left",
          width: 30
          // fixed: 'right'
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center",
          width: 180
        },
        {
          title: "品牌",
          key: "brand",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Product.Manufacturer);
          }
        },
        {
          title: "批次",
          key: "batch",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.DateCode);
          }
        },
        {
          title: "库位",
          key: "ShelveID",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.ShelveID);
          }
        },
        {
          title: "库存数量",
          key: "StockNumber",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.StockQuantity);
          }
        },
        {
          title: "应发数量",
          key: "AnswerNumber",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Quantity);
          }
        },
        {
          title: "实际数量",
          key: "practical",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: "无数据"
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  // params.row.GrossWeight = event.target.value;
                  // vm.details.data1[params.index] = params.row;
                }
              }
            });
          }
        },
        {
          title: "箱号",
          slot: "Boxing_code",
          align: "center"
        },
        {
          title: "毛重(kg)",
          key: "GrossWeight",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Weight
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Weight = event.target.value;
                  vm.waybillinfo.Notices[params.index] = params.row;
                }
              }
            });
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
          width: 140,//220
        }
      ],
      tablelist: [
        {
          iscx: true,
          type: 1, //1商检 2 ccc
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
          type: 0, //1商检 2 ccc
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
          type: 1, //1商检 2 ccc
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
          type: 2, //1商检 2 ccc
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
      Applybox: false,
      applydata: [],
      applyfrom: {
        WarehouseID: sessionStorage.getItem("UserWareHouse"), //库房id
        Summary: "", //备注
        CodePrefix: "WL", //前缀
        DateStr: "" //时间
      },
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
      WarehousingMsg: false, //完成入库的提示 
      isAbnormal: false, //是否异常到货
      Summary: {
        Summary:""
      }, //后台提供的备注信息对象
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
    this.Carriers();
    this.getdetail_data();
    this.getboxarr()
  },
  computed: {
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail;
    }
  },
  mounted() {},
  methods: {
    rowClassName(row, index) {
      if (this.searchkey != "") {
        // return 'demo-table-info-row';
        if (
          row.Product.PartNumber.indexOf(this.searchkey.toUpperCase()) !== -1 ||
          row.Product.PartNumber.indexOf(this.searchkey.toLowerCase()) !== -1 ||
          row.Product.Manufacturer.indexOf(this.searchkey.toUpperCase()) !==
            -1 ||
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
    showBudget() {
      //收支明细展开
      this.$store.dispatch("setBudget", true);
      this.$router.push({
        name: "ends_meetDeclareout",
        params: {
          webillID: this.wayBillID,
          otype: "out"
        }
      });
    },
    closeBudget() {
      //收支明细关闭
      console.log("关闭收支明细");
      this.$router.go(-1);
      // this.Budgetdetail=false;
      this.$store.dispatch("setBudget", false);
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      console.log(value);
      this.SelectRow = value;
    },
    ok() {
      this.$Message.info("Clicked ok");
      this.showphoto = false;
    },
    cancel() {
      this.$Message.info("Clicked cancel");
      this.showphoto = false;
    },
    finish_btn() {
      console.log(this.details.data1);
    },
    changeimgs(newdata, row) {
      //通过子组件传递的数据改变父组件的值
      var arr = this.details.data1;
      for (var j = 0; j < arr.length; j++) {
        if (arr[j].ID == row.ID) {
          arr[j].uploadimg = newdata;
        }
      }
    },
    chaixiang(index, row) {
      //拆箱分拣
      var newinputid = "";
      GetInputID().then(res => {
        console.log(res);
        newinputid = res.id;
        var newdata = {
          BoxCode: row.BoxCode,
          iscx: false,
          Enabled: true, //判断是够可修改，是否是从Sorted 中获取而来
          IsOriginalNotice:false, 
          Product: {
            ID: row.Product.ID,
            PartNumber: row.Product.PartNumber,
            Manufacturer: row.Product.Manufacturer,
            PackageCase:row.Product.PackageCase,
            Packaging: row.Product.Packaging,
            CreateDate: row.Product.CreateDate
          },
          Picking: row.Picking,
          Output: {
            ID: row.Output.ID,
            InputID: row.Output.InputID,
            OrderID: row.Output.OrderID,
            TinyOrderID: row.Output.TinyOrderID,
            ItemID: row.Output.ItemID,
            OwnerID: row.Output.OwnerID,
            SalerID: row.Output.SalerID,
            CustomerServiceID: row.Output.CustomerServiceID,
            PurchaserID: row.Output.PurchaserID,
            Currency: row.Output.Currency,
            Price: row.Output.Price,
            StorageID: row.Output.StorageID,
            CreateDate: row.Output.CreateDate,
            Checker: row.Output.Checker,
            TinyOrderID:row.Output.TinyOrderID
          },
          ID: "CX" + index + new Date().getTime(),
          Type: row.Type,
          WareHouseID:row.WareHouseID,
          WaybillID:row.WaybillID,
          InputID: newinputid,
          OutputID: row.OutputID,
          ProductID: row.ProductID,
          Supplier: row.Supplier,
          DateCode:row.DateCode,
          Quantity: row.Quantity,
          StockQuantity: row.StockQuantity,
          SurplusQuantity:row.SurplusQuantity,
          CreateDate: row.CreateDate,
          Status: row.Status,
          Source: row.Source,
          Target: row.Target,
          BoxCode: row.BoxCode,
          Weight: row.Weight,
          NetWeight: row.NetWeight,
          Volume: row.Volume,
          ShelveID: row.ShelveID,
          Files: [],
          Visable: row.ShelveID,
          Checked: row.ShelveID,
          Input: {
            ID: row.Input.ID,
            Code: row.Input.Code,
            OriginID:row.Input.OriginID,
            OrderID: row.Input.OrderID,
            TinyOrderID: row.Input.TinyOrderID,
            ItemID: row.Input.ItemID,
            ProductID: row.Input.ProductID,
            ClientID: row.Input.ClientID,
            PayeeID: row.Input.PayeeID,
            ThirdID: row.Input.ThirdID,
            TrackerID: row.Input.TrackerID,
            SalerID: row.Input.SalerID,
            PurchaserID: row.Input.PurchaserID,
            Currency: row.Input.Currency,
            UnitPrice: row.Input.UnitPrice,
            DateCode: row.Input.DateCode,
            Origin:row.Input.Origin,
            OriginDescription: row.Input.OriginDescription,
            CreateDate: row.Input.CreateDate,
            ClientID:row.Input.ClientID,
            ClientName: row.Input.ClientName,
            SalerID: row.Input.SalerID,
            ThirdID: row.Input.ThirdID,
            TinyOrderID: row.Input.TinyOrderID,
            TrackerID: row.Input.TrackerID,
            UnitPrice: row.Input.UnitPrice,
          }
        };
        this.waybillinfo.Notices.splice(index + 1, 0, newdata);
      });
    },
    removechaixiang(index) {
      //移除拆项
      this.waybillinfo.Notices.splice(index, 1);
    },
    getdetail_data() {
      var data = {
        warehouseID: sessionStorage.getItem("UserWareHouse"),
        wayBillID: this.wayBillID
      };
      console.log(data);
      pickingsdetail(data).then(res => {
        console.log(res);
        // this.waybillinfo=res.obj;
        // this.details.total = res.obj.Total;
        var objlength = res.obj.Notices.length;
        var obj = res.obj.Notices;
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
        // var newnotice = res.obj.Notices;
        // for (var i = 0; i < newnotice.length; i++) {
        //   if (newnotice[i].Sorted.length > 0) {
        //     var Sorteds = newnotice[i].Sorted;
        //     for (var j = 0; j < Sorteds.length; j++) {
        //       const key = "iscx";
        //       const value = false;
        //       Reflect.set(Sorteds[j], key, value);
        //       var newSorted = Sorteds[j];
        //       newnotice.splice(i + 1, 0, newSorted);
        //     }
        //   } else {
        //   }
        //   // this.details.detailitem = res.obj.Notices;
        // }
        // res.obj.Notices = newnotice;
        this.waybillinfo = res.obj;
      });
    },
    detailspage(value) {
      this.getdetail_data();
    },
    fromphotos(type) {
      if (type == "Waybill") {
        var data = {
          SessionID: this.waybillinfo.WaybillID,
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
        Type: 8010
      };
      if (imgdata.SessionID == this.waybillinfo.WaybillID) {
        this.waybillinfo.Files.push(newfile);
      } else {
        for (var i = 0; i < this.waybillinfo.Notices.length; i++) {
          if (this.waybillinfo.Notices[i].InputID == imgdata.SessionID) {
            this.waybillinfo.Notices[i].Files.push(newfile);
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
    Carriers() {
      //承运商列比奥
      Carriers().then(res => {
        this.CarrierList = res.obj;
      });
    },
    WayParterdata() {
      //输送地列表
      getWayParter().then(res => {
        console.log(res);
        this.Conveyingplace = res.obj;
      });
    },
    clickClient(value, type) {
      //显示更改输送地与原产地的方法
      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = String(value);
    },
    changClientCode(value) {
      //地址改变的时候,保留改变后的地址
      // console.log(value)
      // console.log(this.chengevalue)
      this.chengevalue.inputval = value;
    },
    primaryClientCode() {
      //确认更改地址
      console.log(this.chengevalue);
      if (this.chengevalue.type == "ClientCode") {
        this.details.Place = this.chengevalue.inputval.value;
        this.details.PlaceDescription = this.chengevalue.inputval.label;
      } else {
        this.chengevalue.type.Input.OriginDescription = this.chengevalue.inputval.label;
        var newNotices = this.details.waybillinfo.Notices;
        for (var i = 0, l = newNotices.length; i < l; i++) {
          if (this.chengevalue.type.ID == newNotices[i].ID) {
            newNotices[i].Input.Origin = this.chengevalue.inputval.value;
            newNotices[
              i
            ].Input.OriginDescription = this.chengevalue.inputval.label;
            break;
          }
        }
      }
      this.setClientCode = false;
      console.log(this.details.waybillinfo);
    },
    clickDropdown(event, index) {
      //改变箱号
      console.log(event);
      console.log(index);
      this.waybillinfo.Notices[index].BoxCode = event;
      console.log(this.waybillinfo.Notices);
    },
    getboxarr(){
      var houseid=sessionStorage.getItem("UserWareHouse")
      GetBoxes(houseid,"200").then(res=>{
        this.boxingarr=res;
      })
    },
    okapply() {
      //确认申请新箱号
      // console.log(this.applyfrom);
      Boxenter(this.applyfrom).then(res => {
        console.log(res);
        if (res.success == true) {
          this.$Message.success("申请成功");
          this.getboxarr()
        } else {
          this.$Message.error("箱号申请失败");
        }
      });
    },
    // cancelapply(){ //取消申请

    // },
    visiblechange(value) {
      //申请箱号状态发生变化的时候
      if (value == true) {
        this.Applybox = true;
        this.GetBoxesdates();
      } else {
        this.Applybox = false;
        this.applyfrom = {
          WarehouseID: sessionStorage.getItem("UserWareHouse"), //库房id
          Summary: "", //备注
          CodePrefix: "WL", //前缀
          DateStr: "" //时间
        };
      }
    },
    changboxvisible(value) {
      //更改箱号弹窗更改状态发生变化的时候
      if (value == true) {
        this.showchangebox = true;
      } else {
        this.showchangebox = false;
        this.newboxcode = "";
      }
    },
    clickshowchangebox() {
      //点击一键入箱，显示入箱弹窗
      if (this.SelectRow.length == 0) {
        this.$Message.error("至少选择一条产品");
      } else {
        this.showchangebox = true;
      }
    },
    ok_changebox() {
      //确认更改箱号
      console.log(this.newboxcode);
      for(var i=0,ilen=this.waybillinfo.Notices.length;i<ilen;i++){
        for(var j=0;j<this.SelectRow.length;j++){
            if(this.waybillinfo.Notices[i].ID==this.SelectRow[j].ID){
              this.waybillinfo.Notices[i].BoxCode=this.newboxcode;
            }
        }
      }
      this.SelectRow=[];
      this.$refs.selection.selectAll(false);
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
      var isnull = null //判断实际到货数量 true 为空 false 不为空
      var that = this
      var dataarr = this.detailitem
      var null_num = 1;
      that.WarehousingMsg = true
    },
    setWarehousing(data) {
      //确定出库，调取后台出库接口
      pickingsenter(data).then(res => {
        if (res.val == 0) {
          //成功之后调用父组件的关闭
          this.$Message.success('分拣完成，一秒后自动关闭')
          var that = this
          setTimeout(function() {
            // that.fatherMethod();
            that.$store.dispatch('setshowtype', 0)
            that.$store.dispatch('setshowdetailout', false)
            that.$router.push({ path:"/Outgoing"})
          }, 1000)
        } else if (res.val == 400) {
          this.$Message.error('此订单已关闭，请返回处理其他订单')
          var that = this
          setTimeout(function() {
            // that.fatherMethod();
            that.$store.dispatch('setshowtype', 0)
            that.$store.dispatch('setshowdetailout', false)
          }, 1000)
        }else if(res.val == 300){
             this.$Message.error('库存不足，无法分拣')
        }else {
          this.$Message.error('分拣操作失败,请从新操作')
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
      // this.Summary.Title=this.Reason
      console.log(this.waybillinfo)
      var uploaddata = {
        obj: JSON.stringify(this.waybillinfo),
        Status: 215, //200
        token: this.getCookie('ydcx_Yahv.Erp'),
        // Summary: JSON.stringify(this.Summary)
        Summary: ''
      }
      this.setWarehousing(uploaddata)
      this.WarehousingMsg = false
    },
    cancel_Warehousing() {
      //点击取消按钮，取消出库
      this.WarehousingMsg = false
    },
    errorstatue(value) {
      if (value == true) {
        this.isAbnormal = true
      } else {
        this.isAbnormal = false
        this.Summary.Summary = ''
      }
    },
    //到货异常 确认按钮
    Abnormal_btn() {
      console.log(this.Summary.Summary)
      if (this.Summary.Summary == undefined || this.Summary.Summary == '') {
        this.isAbnormal = true
        this.$Message.error('请输入异常原因')
      } else {
        this.Summary.Title = this.Reason
        var uploaddata = {
          obj: JSON.stringify(this.waybillinfo),
          Status: 220, //200 出库异常
          token: this.getCookie('ydcx_Yahv.Erp'),
          Summary: this.Summary.Summary
          // Summary: JSON.stringify(this.Summary)
        }
        this.setWarehousing(uploaddata) //接口
        this.isAbnormal = false
      }
    },
     closeerror() {
      //异常到货关闭
      this.isAbnormal = false
      this.Summary.Summary = '' //备注
      this.Reason = '外观损坏' //异常原因
    },
    testbtn(){
      console.log(this.waybillinfo)
    },
    handleRemove(file) {
      this.waybillinfo.Files.splice(this.waybillinfo.Files.indexOf(file), 1);
    },
    handleRemovelist(row, file) {
      var arr = this.waybillinfo.Notices;
      for (var j = 0; j < arr.length; j++) {
        //删除指定下标 的元素
        if (arr[j].ID == row.ID) {
          arr[j].Files.splice(file, 1);
        }
      }
    },
  },
};
</script>