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
  width: 70px;
  /* font-weight: bold; */
}
.detail_title2 {
  display: inline-block;
  width: 100px;
}
.detail_title3 {
  display: inline-block;
  width: 120px;
}
.Declare /deep/ .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.ivu-table .demo-table-info-row td {
  display: none;
}
.ivu-table .demo-table-error-row td {
  background-color: #ff6600;
  color: #fff;
}
.transferclass{
  width:150px;
  height: 200px;
  overflow-x: hidden;
  overflow-y: scroll;
}
</style>
<template>
  <div class="Declare">
    <div style="width:100%;">
      <div class="itembox">
        <p class="detailtitle">代报关基础信息</p>
        <!-- <p>{{getids}}</p> -->
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row>
            <Col span="5">
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
                <li class="itemli" v-if="waybillinfo!=''">
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
                  <Tag color="lime" v-if="details.waybillinfo.Conditions.QualityInspection==true">质检</Tag>
                  <Tag color="yellow" v-if="details.waybillinfo.Conditions.Unboxing==true">拆包装</Tag>
                </li>
              </ul>
            </Col>
            <Col span="5">
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
                  <span class="detail_title2">到货方式：</span>
                  <span>{{waybillinfo.WaybillTypeDescription}}</span>
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
                <li class="itemli" v-if="waybillinfo.WaybillType==1">
                  <span class="detail_title2">提货文件：</span>
                  <span
                    v-for="(item,index) in  waybillinfo.Files"
                    v-if="item.Type==10"
                  >{{item.CustomName}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title2">提货文件：</span>
                  <span
                    v-for="(item,index) in  waybillinfo.Files"
                    v-if="item.Type==25"
                  >{{item.CustomName}}</span>
                </li>
              </ul>
            </Col>
            <Col span="6">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title3">入仓号：</span>
                  <span>{{waybillinfo.EnterCode}}({{waybillinfo.ClientName}})</span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">运单号(本次)：</span>
                  <span>
                    <Input style="width:60%" v-model="waybillinfo.Code" />
                  </span>
                </li>
                <li class="itemli">
                  <span class="detail_title3">承运商(本次):</span>
                  <span>
                    <Select v-model="details.Carrier" style="width:60%">
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
                    <span v-if="waybillinfo.PlaceDescription!=''">{{waybillinfo.PlaceDescription}}</span>
                    <span v-else style="color:red;">暂无输送地</span>
                    <Icon
                      class="sethover"
                      @click="clickClient(waybillinfo.Place,'ClientCode','ClientCode')"
                      type="md-create"
                    />
                  </span>
                </li>
              </ul>
            </Col>
            <Col span="5">
              <ul class="detail_li" style="margin-left:20px;">
                <li class="itemli">
                  <span class>订单号：</span>
                  <span>{{waybillinfo.OrderID}}</span>
                </li>
                <li class="itemli">
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
                  </div>
                </li>
                <li style="clear: both;">
                  <div v-for="item in waybillinfo.Files" v-if="item.Type==8000">
                    <span>{{item.CustomName}}</span>
                    <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
                  </div>
                </li>
              </ul>
            </Col>
            <Col span="3">
              <div
                style="position: absolute; top: 147px; right: 0;"
                v-if="waybillinfo.WaybillType==1&&waybillinfo.ExcuteStatus!=105"
              >
                <Button type="primary" @click="TakeGoods">{{TakeGoodsName}}</Button>
              </div>
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
          <Button type="primary" 
          @click="clickshowchangebox"
          :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
          >一键入箱</Button>
          <Button type="primary" @click="showBudget">收支明细</Button>
          <div style="float:right">
            <Button
              :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
              type="primary"
              shape="circle"
              icon="md-checkmark"
              @click="finish_btn"
            >入库完成</Button>
            <Button
              :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
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
              :columns="tabletitle"
              :data="waybillinfo.Notices"
              @on-selection-change="handleSelectRow"
              :row-class-name="rowClassName"
            >
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <template slot-scope="{ row, index }" slot="PartNumber">
                <div style="display: flex;justify-content: space-between;align-items: center;">
                    <div style="float:left;text-align:left">
                      <Input
                        style="width:120px;overflow: hidden;"
                        v-if="row.iscx!=false"
                        v-model="row.Product.PartNumber"
                        @on-change="changePartNumber($event,row.InputID,index,row)"
                      />
                      <span v-else style="width:120px;overflow: hidden;">{{row.Product.PartNumber}}</span>
                    </div>
                    <ul style="float:right;">
                      <li><Tag v-if="row.Condition.IsCIQ==true" color="primary">商检</Tag></li>
                      <li><Tag v-if="row.Condition.IsCCC==true" color="warning">CCC</Tag></li>
                      <li><Tag v-if="row.Condition.IsEmbargo==true" color="error">禁运</Tag></li>
                      <li><Tag v-if="row.Condition.IsHighPrice==true" color="magenta">高价值</Tag></li>
                    </ul>
                </div>
              </template>
              <template
                slot-scope="{ row, index }"
                slot="Arrival"
              >
              <p v-if="row.iscx!=false">
                  {{row.SortedQuantity}}&nbsp;/&nbsp;{{row.Quantity}}
              </p>
              </template>
              <template slot-scope="{ row, index }" slot="Country_origin">
                <span v-if="row.Input.Origin!=''">{{row.Input.OriginDescription}}</span>
                <span v-else style="color:red;">暂无输送地</span>
                <Icon
                  class="sethover"
                  @click="clickClient(row.Input.Origin,row,index)"
                  type="md-create"
                />
              </template>
              <template slot-scope="{row,index}" slot="Boxing_code">
                <!-- <Dropdown :transfer="true"  trigger="click"  @on-click="clickDropdown($event,index)" transfer-class-name="transferclass">
                  <span>
                    <em>{{row.BoxCode}}</em>
                    <Icon type="ios-arrow-down"></Icon>
                  </span>
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
                </Dropdown> -->
                <div style="display:inline-block;width:80%">
                  <Select v-model="row.BoxCode" :transfer='true' @on-change="clickDropdown($event,index)">
                    <Option v-for="(item,index) in boxingarr" :value="item.Code" :key="index">{{ item.Code }}</Option>
                 </Select>
                </div>
                <div style="display:inline-block;width:15%">
                  <Icon type="md-add" @click="Applybox=true"/>
                </div>
                 
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="item in row.Files">
                  <span v-if="item.Type==8000">{{item.CustomName}}</span>
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
                    :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
                  >传照</Button>
                </div>
                <div class="setupload">
                  <Button
                    type="primary"
                    icon="md-reverse-camera"
                    @click="fromphotos(row.InputID)"
                    :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
                  >拍照</Button>
                </div>
                <div class="setupload" v-if="row.iscx!=false">
                  <!-- <Button type="primary" icon="md-checkmark" @click="chaixiang(index,row)">拆项</Button> -->
                </div>
                <div class="setupload" v-if="row.iscx==false">
                  <!-- <Button
                    :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
                    type="primary"
                    icon="ios-trash-outline"
                    @click="removechaixiang(index,row)"
                  >删除</Button> -->
                </div>
              </template>
            </Table>
            <div class="successbtn" style="margin-top:20px;">
             <!-- <Button
                type="primary"
                icon="md-checkmark"
                @click="testbtn"
              >入库完成测试</Button> -->
              <Button
                type="primary"
                icon="md-checkmark"
                @click="finish_btn"
                :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
              >入库完成</Button>
              <Button
                type="warning"
                icon="ios-alert-outline"
                @click="isAbnormalclick"
                :disabled="waybillinfo.ExcuteStatus== 120 ? true:false"
              >到货异常</Button>
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
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="primaryClientCode">确定</Button>
      </div>
    </Modal>
    <!-- 输送地列表与更改 结束-->
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
    <!-- 确认入库 开始 -->
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
    <!-- 确认入库 结束 -->
    <!-- 异常到货 开始-->
    <Modal v-model="isAbnormal" title="到货异常" @on-visible-change="changeerror">
      <div slot="close">
        <Icon type="md-close" color="rgb(33, 28, 28)" @click="closeerror" style="font-size:18px;" />
      </div>
      <span style="line-height:26px;">异常原因</span>
      <Input v-model="Summary.Summary" type="textarea" :rows="2" placeholder="备注" />
      <div slot="footer">
        <Button @click="closeerror">取消</Button>
        <Button type="primary" @click="Abnormal_btn">确定</Button>
      </div>
    </Modal>
    <!-- 异常到货 结束-->
    <!-- 历史到货 开始 -->
    <Drawer :closable="true" :width="70" :mask-closable="true" v-model="historydata">
      <Historys-dom :key="historydetail.times" ref="Historygoods"></Historys-dom>
    </Drawer>
    <!-- 历史到货 结束 -->
  </div>
</template>
<script>
import Historys from "@/Pages/Common/Historygoods";
import {
  GetBoxesdates,
  GetBoxes,
  Noticedetail,
  Boxenter,
  getWayParter,
  GetInputID,
  sortingupload,
  History,
  TakeGoods
} from "../../api";
import { TemplatePrint, GetPrinterDictionary, FilePrint,FormPhoto, SeletUploadFile} from "@/js/browser.js";
let product_url = require("../../../static/pubilc.dev");
let lodash = require("lodash");
export default {
  name: "",
  components: {
    "Historys-dom": Historys
  },
  data() {
    return {
      TakeGoodsName: "我去提货",
      historydata: false, //历史到货的抽屉
      historydetail: {
        //历史到货数据
        times: "", //时间，每次获取新的版本
        waybillLIst: [] //运单列表
      },
      WarehousingMsg: false,
      Nomatching: "", //数量不符合的型号
      Summary: {
        Summary: ""
      },
      isAbnormal: false, //异常对话框
      searchkey: "", //筛选
      SelectRow: [], //多选
      Applybox: false,
      applydata: [],
      applyfrom: {
        WarehouseID: sessionStorage.getItem("UserWareHouse"), //库房id
        Summary: "", //备注
        CodePrefix: "HXT", //前缀
        DateStr: "" //时间
      },
      detail_ID: "",
      showphoto: false, //显示拍照弹出框
      CarrierList: [], //承运商列表
      Conveyingplace: [], //输送地列表
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
      waybillinfo: "", //运单信息
      wayBillID: this.$route.params.wayBillID, //运单号
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
                  vm.waybillinfo.Notices[params.index] = params.row;
                      for (var i = 0; i < vm.waybillinfo.Notices.length; i++) {
                        if (vm.waybillinfo.Notices[i].PID == params.row.ID) {
                          vm.waybillinfo.Notices[i].Product.Manufacturer = event.target.value;
                        }
                      }
                  vm.clicktest(params.row);
                }
              }
            });
          }
        },
        {
          title: "已到/应到",
          slot: "Arrival",
          align: "center"
        },
        {
          title: "批次",
          key: "batch",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.batch
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Input.DateCode = event.target.value;
                  vm.waybillinfo.Notices[params.index] = params.row;
                  vm.clicktest(params.row);
                }
              }
            });
          }
        },
        {
          title: "本次到货",
          key: "Quantity",
          align: "center",
          render: (h, params) => {
            var vm = this;
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
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: params.row.TruetoQuantity
                },
                on: {
                  "on-change"(event) {
                    // var reg = /^[0-9]*$/;
                    var reg = /^\d+(\.\d{0,2})?$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.TruetoQuantity = event.target.value;
                      vm.waybillinfo.Notices[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("只能输入正整数");
                      (event.target.value = ""),
                        (params.row.TruetoQuantity = "");
                      vm.waybillinfo.Notices[params.index] = params.row;
                    }
                  },
                  "on-blur"(event) {
                    console.log(event.target.value);
                    if (event.target.value != "") {
                      // var reg = /^[0-9]*$/;
                      var reg = /^\d+(\.\d{0,2})?$/;
                      if (reg.test(event.target.value) == true) {
                        params.row.TruetoQuantity = event.target.value;
                        vm.waybillinfo.Notices[params.index] = params.row;
                        // vm.clicktest(params.row);
                      } else {
                        vm.$Message.error("只能输入正整数");
                        (event.target.value = ""),
                          (params.row.TruetoQuantity = "");
                        vm.waybillinfo.Notices[params.index] = params.row;
                      }
                    }
                  }
                }
              });
            }
          }
        },
        {
          title: "原产地",
          slot: "Country_origin",
          align: "center"
        },
        {
          title: "箱号",
          slot: "Boxing_code",
          align: "center",
          width:160
        },
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
                    vm.waybillinfo.Notices[params.index] = params.row;
                  } else {
                    vm.$Message.error("只能输入数字,包括两位数的小数点");
                    event.target.value = "";
                    params.row.Volume = "";
                    vm.waybillinfo.Notices[params.index] = params.row;
                  }
                },
                "on-blur"() {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  var reg = /^\d+(\.\d{0,2})?$/;
                  // reg.test(event.target.value);
                  if (reg.test(event.target.value) == true) {
                    params.row.Volume = event.target.value;
                    vm.waybillinfo.Notices[params.index] = params.row;
                  } else {
                    vm.$Message.error("只能输入数字,包括两位数的小数点");
                    event.target.value = "";
                    params.row.Volume = "";
                    vm.waybillinfo.Notices[params.index] = params.row;
                  }
                }
              }
            });
          }
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
                "on-change"(event) {},
                "on-blur"(event) {
                  var reg = /^\d+(\.\d{0,4})?$/;
                  var newtarget = vm.trim(event.target.value);
                  if (newtarget != "") {
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget;
                      vm.waybillinfo.Notices[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数字,包括四位数的小数点");
                      params.row.Weight = "";
                      event.target.value = "";
                      // vm.details.detailitem[params.index] = params.row;
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
                    vm.waybillinfo.Notices[params.index] = params.row;
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
                    vm.waybillinfo.Notices[params.index] = params.row;
                  }
                }
              }
              // on: {
              //   "on-change"(event) {
              //     //值改变时
              //     //将渲染后的值重新赋值给单元格值
              //     params.row.GrossWeight = event.target.value;
              //     vm.details.data1[params.index] = params.row;
              //   }
              // }
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
          width: 140  //220
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
      boxingarr: [
        // { ID: "11111111" },
        // { ID: "22222222" },
        // { ID: "33333333" },
        // { ID: "44444444" }
      ],
      showchangebox: false, //显示一键更改箱号的的弹窗
      newboxcode: "", // 一键更改箱号后的新箱号
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
      model1: "",
      printurl: product_url.pfwms,
      printlist: [] //打印列表
    };
  },
  created() {
    window["PhotoUploaded"] = this.changed;
    // console.log(this.wayBillID);
    this.getdetail_data();
    this.getboxarr()
  },
  computed: {
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail;
    }
  },
  mounted() {
    this.WayParterdata();
    var datas = {
      waybillid: this.waybillinfo.wayBillID
    };
    this.History(datas);
  },
  methods: {
    clicktest: lodash.throttle(function(paramsrow) {
      //修改数据，触发改变inputid 的方法，修改对应数据的inputid
      var rows = paramsrow;
      var arr = this.waybillinfo.Notices;
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
    changePartNumber(event, rowid, index,row) {//改变型号
      if (this.waybillinfo.Notices[index].InputID == rowid) {
        this.waybillinfo.Notices[index].Product.PartNumber = event.target.value;
         this.clicktest(this.waybillinfo.Notices[index]);
         for (var i = 0; i < this.waybillinfo.Notices.length; i++) {
            if (this.waybillinfo.Notices[i].PID ==row.ID) {
               this.waybillinfo.Notices[i].Product.PartNumber = event.target.value;
            }
          }
      }
      // console.log(this.waybillinfo.Notices[index].Product);
      // this.waybillinfo.Notices[index]
    },
    trim(str) {
      //去除前后空格
      return str.replace(/(^\s*)|(\s*$)/g, "");
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
    handleRemove(file) {
      this.waybillinfo.Files.splice(this.waybillinfo.Files.indexOf(file), 1);
    },
    getboxarr(){
      var houseid=sessionStorage.getItem("UserWareHouse")
      GetBoxes(houseid,"200").then(res=>{
        this.boxingarr=res;
      })
    },
    GetBoxesdates() {
      //选择箱号日期
      GetBoxesdates().then(res => {
        console.log(res);
        this.applydata = res;
      });
    },
    showBudget() {
      //收支明细展开
      // this.$store.dispatch("setBudget", true);
      // this.$router.push({
      //   name: "ends_meet",
      //   params: {
      //     webillID: this.WaybillID
      //   }
      // });

      this.$store.dispatch("setBudget", true);
      this.$router.push({
        name: "ends_meetDeclare",
        params: {
          webillID: this.wayBillID,
          otype: "in"
        }
      });
    },
    closeBudget() {
      //收支明细关闭
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
      // this.$Message.info("");
      this.showphoto = false;
      this.Applybox = false;
      this.setClientCode = false;
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
          iscx: false,
          Enabled: true, //判断是够可修改，是否是从Sorted 中获取而来
          IsOriginalNotice:false, 
          BoxCode: row.BoxCode,
          CheckValue: row.CheckValue,
          ClientID: row.ClientID,
          Condition: {
            AttachLabel: row.Condition.AttachLabel,
            CheckNumber: row.Condition.CheckNumber,
            DevanningCheck: row.Condition.DevanningCheck,
            OnlineDetection: row.Condition.OnlineDetection,
            PaintLabel: row.Condition.PaintLabel,
            PickByValue: row.Condition.PickByValue,
            Repacking: row.Condition.Repacking,
            Weigh: row.Condition.Weigh
          },
          DateCode: row.DateCode,
          Files: [],
          ID: "CX" + index + new Date().getTime(),
          PID: row.ID, //父ID
          WareHouseID: row.WareHouseID,
          WayBillID: row.WayBillID,
          ProductID: row.ProductID,
          InputID: newinputid,
          Input: {
            ClientID: row.Input.ClientID,
            ClientName: row.Input.ClientName,
            Code: newinputid,
            CreateDate: row.Input.CreateDate,
            Currency: row.Input.Currency,
            DateCode: row.Input.DateCode,
            EnterpriseID: row.Input.EnterpriseID,
            ID: newinputid,
            ItemID: row.Input.ItemID,
            OrderID: row.Input.OrderID,
            Origin: row.Input.Origin,
            OriginID: row.Input.OriginID,
            OriginDescription: row.Input.OriginDescription,
            ProductID: row.Input.ProductID,
            PurchaserID: row.Input.PurchaserID,
            SalerID: row.Input.SalerID,
            ThirdID:row.Input.ThirdID,
            TrackerID: row.Input.TrackerID,
            UnitPrice: row.Input.UnitPrice,
            TinyOrderID:row.Input.TinyOrderID
          },
          Product: {
            CreateDate: row.Product.CreateDate,
            ID: row.Product.ID,
            Manufacturer: row.Product.Manufacturer,
            PackageCase: row.Product.PackageCase,
            Packaging: row.Product.Packaging,
            PartNumber: row.Product.PartNumber
          },
          Quantity: row.Quantity,
          ShelveID: row.ShelveID,
          SortedQuantity: row.SortedQuantity,
          Source: row.Source,
          TruetoQuantity: row.TruetoQuantity,
          Volume: row.Volume,
          WayBillID: row.WayBillID,
          Weight: row.Weight
        };
        this.waybillinfo.Notices.splice(index + 1, 0, newdata);
      });
    },
    removechaixiang(index) {
      //移除拆项
      this.waybillinfo.Notices.splice(index, 1);
    },
    getdetail_data(id) {
      var data = {
        warehouseID: sessionStorage.getItem("UserWareHouse"),
        wayBillID: this.wayBillID
      };
      // console.log(data)
      Noticedetail(data).then(res => {
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
        res.obj.Notices = newnotice;
        this.waybillinfo = res.obj;
      });
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
        Type: 8000
      };
      if (imgdata.SessionID == this.waybillinfo.WaybillID) {
        this.waybillinfo.Files.push(newfile);
      } else {
        // alert(JSON.stringify(imgdata))
        // this.details.detailitem[imgdata.SessionID].Files.push(newfile);
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
    clickClient(value, type, index) {
      //显示更改输送地与原产地的方法
      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = String(value);
      this.ConveyingplaceID = index;
    },
    changClientCode(value) {
      //地址改变的时候,保留改变后的地址
      this.chengevalue.inputval = value;
    },
    primaryClientCode() {
      //确认更改地址
      console.log(this.chengevalue);
      if (this.chengevalue.type == "ClientCode") {
        this.waybillinfo.Place = this.chengevalue.inputval.value;
        this.waybillinfo.PlaceDescription = this.chengevalue.inputval.label;
      } else if (this.ConveyingplaceID != "ClientCode") {
        this.chengevalue.type.Input.OriginDescription = this.chengevalue.inputval.label;
        this.waybillinfo.Notices[
          this.ConveyingplaceID
        ].Input.Origin = this.chengevalue.inputval.value;
        this.waybillinfo.Notices[
          this.ConveyingplaceID
        ].Input.OriginDescription = this.chengevalue.inputval.label;
      }
      this.setClientCode = false;
    },
    clickDropdown(event, index) {
      //改变箱号
      console.log(event);
      // console.log(index);
      this.waybillinfo.Notices[index].BoxCode = event;
      console.log(this.waybillinfo.Notices);
    },
    okapply() {
      //确认申请新箱号
      console.log(this.applyfrom);
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
          CodePrefix: "HXT", //前缀
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
      //入库完成
      var isnull = null; //判断实际到货数量 true 为空 false 不为空
      var that = this;
      var dataarr = this.waybillinfo.Notices;
      var null_num = 1;
      for (var i = 0; i < dataarr.length; i++) {
        if (dataarr[i].TruetoQuantity == "" || dataarr[i].TruetoQuantity == null) {
          isnull = true;
          null_num++;
          break;
        } else {
          isnull = false;
          null_num = 1;
          // null_num=0
        }
      }

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
        var islarge=false;
        for (var i = 0; i < dest.length; i++) {
          var item = dest[i];
          var total = sum(item.data);
          // console.log(total);
          if (total>item.Quantity ) {
            islarge=true;
            Nomatching.push(item.name);
            // this.$Message.error("分拣数量与到货通知不符");
            break;
          } 
          else if(total < item.Quantity ){
              Nomatching.push(item.name);
          }
          // if (item.Quantity < total || item.Quantity > total) {
          //   Nomatching.push(item.name);
          //    break;
          // } else {

          // }
        }
        this.Nomatching = Nomatching;
        // console.log(this.Nomatching)
        if(islarge==true){
          this.$Message.error("分拣数量大于通知数量,不能进行入库");
        }
        else{
          that.WarehousingMsg = true;
        }

    },
    ok_Warehousing() {
      //点击确定按钮，进行入库操作
      //  this.Summary.Title=this.Reason
      console.log(this.waybillinfo)
      var uploaddata = {
        obj: JSON.stringify(this.waybillinfo),
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
    setWarehousing(data) {
      //确定入库，调取后台入库接口
      sortingupload(data).then(res => {
        if (res.val == 0) { //成功之后调用关闭方法
            this.$Message.success("操作完成，一秒后自动关闭");
            var that = this;
            setTimeout(function() {
              that.$store.dispatch("setshowdetail", false);
              that.$router.push({ path: "/Warehousing" });
            }, 1000);
        } else if (res.val == 400) {
            this.$Message.error("此订单已被关闭，请处理其他订单");
            var that = this;
            setTimeout(function() {
              that.$store.dispatch("setshowdetail", false);
              that.$router.push({ path: "/Warehousing" });
            }, 2000);
        } else if (res.val == 500) {
          this.$Message.error("请输入数量，数量不能为空");
        } else if(res.val == 610){
          this.$Message.error("请选择一个箱号");
        }
        else {
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
    torouters() {
      //测试入库跳转
      this.$store.dispatch("setshowdetail", false);
      this.$router.push({ path: "/Warehousing" });
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
        var uploaddata = {
          obj: JSON.stringify(this.waybillinfo),
          // Summary:JSON.stringify(this.Summary),
          token: this.getCookie("ydcx_Yahv.Erp"),
          Summary: this.Summary.Summary,
          Status: 130 //300
        };

        this.setWarehousing(uploaddata); //接口
        this.isAbnormal = false;
      }
    },
    closeerror() {
      //异常到货关闭
      this.Summary.Summary = ""; //备注
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
    showhistory(name) {
      //展示历史订单组件
      this.historydata = true;
      this.historydetail.times = new Date().getTime();
      var namenew = name.trim().split(/\s+/);
      // console.log(name.trim().split(/\s+/))
      var that = this;
      var data = {
        waybillid: namenew[0]
      };
      setTimeout(function() {
        that.$refs.Historygoods.gethistory(data);
      }, 20);
    },
    History(data) {
      //历史运单
      History(data).then(res => {
        //  console.log(res)
        this.historydetail.waybillLIst = res.obj;
      });
    },
    TakeGoods() {
      //我去提货
      var data = {
        waybillid: this.waybillinfo.WaybillID
      };
      TakeGoods(data).then(res => {
        if (res.val == 1) {
          this.waybillinfo.ExcuteStatusDescription = "正在提货中";
          this.TakeGoodsName = "正在提货";
        } else {
          this.$Message.error("提货锁定失败");
        }
      });
    },
    testbtn(){
      console.log(this.waybillinfo)
    }
  }
};
</script>