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
  width: 100px;
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
.waybillprintcss {
  padding-top: 15px;
  font-size: 14px;
}
.printbtn {
  width: 100%;
  text-align: center;
  margin-top: 20px;
}
.userfrom ul li {
  padding-top: 15px;
  padding-left: 10px;
}
.userfrom {
  width: 100%;
  height: auto;
  padding: 15px 0;
  clear: both;
  overflow: hidden;
}
.Mustfill {
  color: red;
}
.userinfodata {
  width: 48%;
  min-height: 215px;
  float: left;
  border: 1px dashed #ddd;
  padding-top: 10px;
}
.userinfodata h1 {
  padding-left: 10px;
}
.inputform {
  width: 80%;
}
.icon1:hover {
  cursor: pointer;
}
.listcode {
  padding-left: 5px;
}
.listcode li {
  padding-bottom: 5px;
}
.ivu-table .demo-table-info-row td {
  display: none;
}
.ivu-table .demo-table-error-row td {
  background-color: #edfff3;
}
.childeninput {
  width: 80%;
  margin-left: 10px;
}
.setimgcolor {
  color: #2d8cf0;
}
.setimgcolor:hover {
  cursor: pointer;
}
.allprintstyle{
  background: #2d8cf0;
  border-radius: 5px;
  margin-right: 20px
}
.allprintactive{
  background: #d2d2dc;
  border-radius: 5px;
  margin-right: 20px
}
</style>
<template>
  <div>
    <!-- <Spin size="large" fix v-if="spinShow"></Spin> -->
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
            <Col style="width: 23%;float: left;">
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
                  <span class="detail_title2">发货方式：</span>
                  <span>{{waybillinfo.WaybillTypeDescription}}</span>
                  <!-- <a href="">历史到货
                         <Badge :count="10"></Badge>
                  </a>-->
                </li>
                <li class="itemli">
                  <span class="detail_title2">包装要求：</span>
                  <span
                    style="line-height: 20px;width: 70%;display: inline-block;"
                  >{{waybillinfo.Packaging}}</span>
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
                <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title3">随货文件：</span>
                  <span>
                    <div v-for="(item,index) in waybillinfo.Files" v-if="item.Type==25">
                      <span>{{item.CustomName}}</span>
                      <a @click="fileprint(item.Url)">打印</a>
                    </div>
                  </span>
                </li>
              </ul>
            </Col>
            <Col style="width: 30%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title3">客户编号：</span>
                  <span>{{waybillinfo.EnterCode}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==1">
                  <span class="detail_title3">联系人：</span>
                  <span >{{waybillinfo.Consignee.Contact}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==1">
                  <span class="detail_title3">提货人证件：</span>
                  <span >{{waybillinfo.Consignee.IDNumber}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==1">
                  <span class="detail_title3">提货人电话：</span>
                  <span>{{waybillinfo.Consignee.Phone}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType!=1">
                  <span class="detail_title3">运单号(本次)：</span>
                  <span>
                    <Input style="width:60%" v-model="waybillinfo.Code" />
                    <!-- <Icon type="md-add-circle" :size="18" color="#57a3f3" class="icon1" @click="showChildrencode=true" /> -->
                    <Icon
                      type="md-add-circle"
                      :size="18"
                      color="#57a3f3"
                      class="icon1"
                      @click="showChildrencode=true"
                    />
                  </span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType!=1">
                  <span class="detail_title3">承运商(本次):</span>
                  <span>
                    <Select
                      v-model="waybillinfo.CarrierID"
                      style="width:60%"
                      @on-change="changeacrrier"
                      :label-in-value="true"
                    >
                      <Option
                        v-for="item in CarrierList"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>

                <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title3">司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                  <span>
                    <Select
                      v-model="waybillinfo.WayLoading.Driver"
                      style="width:60%"
                      :label-in-value="true"
                      @on-change="changedrivers"
                    >
                      <Option
                        v-for="item in DriversArr"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title3">车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                  <span>
                    <Select
                      v-model="waybillinfo.WayLoading.CarNumber1"
                      style="width:60%"
                      :label-in-value="true"
                      @on-change="changedrivers"
                    >
                      <Option
                        v-for="item in CarArr"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.CarNumber1 }}&nbsp;&nbsp;&nbsp;&nbsp;{{ item.CarNumber2 }}</Option>
                    </Select>
                  </span>
                </li>

                <li class="itemli" v-if="waybillinfo.WaybillType!=1">
                  <span class="detail_title3" style="float: left;">收货地址：</span>
                  <span
                    v-if="waybillinfo.Consignee!=undefined"
                    style="display: inline-block;max-width: 225px;line-height: 20px;"
                  >{{waybillinfo.Consignee.Address}}</span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType!=2">
                  <span class="detail_title3">随货文件：</span>
                  <span>
                    <div v-for="(item,index) in waybillinfo.Files" v-if="item.type==25">
                      <span>{{item.CustomName}}</span>
                      <a @click="fileprint(item.Url)">打印</a>
                    </div>
                  </span>
                </li>
              </ul>
            </Col>
            <!-- <Col style="width:20%;float: left;" v-if="waybillinfo.WaybillType==2"> 
              <ul class="detail_li">
                <li class="itemli" v-if="waybillinfo.WaybillType!=1">
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
               </li>
               <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title3">司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                  <span>
                    <Select v-model="waybillinfo.CarrierID" style="width:60%" @on-change="changeacrrier" :label-in-value="true">
                      <Option
                        v-for="item in CarrierList"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>
                <li class="itemli" v-if="waybillinfo.WaybillType==2">
                  <span class="detail_title3">车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                  <span>
                    <Select v-model="waybillinfo.CarrierID" style="width:60%" @on-change="changeacrrier" :label-in-value="true">
                      <Option
                        v-for="item in CarrierList"
                        :value="item.ID"
                        :key="item.ID"
                      >{{ item.Name }}</Option>
                    </Select>
                  </span>
                </li>
              </ul>
            </Col>-->
            <Col style="width: 20%;float: left;">
              <ul class="detail_li" style="margin-left:20px;">
                <li class="itemli" v-if="waybillinfo.ExcuteStatus!=215">
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
                <li class="itemli"  v-if="waybillinfo.WaybillType==2&&waybillinfo.ExcuteStatus==215||waybillinfo.WaybillType==1&&waybillinfo.ExcuteStatus==215" style="padding-top:10px;clear: both;">
                  <span style="float:left;line-height: 27px;">送&nbsp;货&nbsp;单&nbsp;照&nbsp;片：</span>
                  <div class="setupload">
                    <Button
                      type="primary"
                      size="small"
                      icon="ios-cloud-upload"
                      @click="SeletUpload('Clientimg')"
                    >传照</Button>
                  </div>
                  <div class="setupload">
                    <Button
                      type="primary"
                      icon="md-reverse-camera"
                      @click="fromphotos('Clientimg')"
                    >拍照</Button>
                  </div>
                </li>
                <!-- <li class="itemli"  v-if="waybillinfo.WaybillType==2||waybillinfo.WaybillType==1" style="padding-top:10px;clear: both;">
                  <span style="float:left;line-height: 27px;">送&nbsp;货&nbsp;单&nbsp;照&nbsp;片：</span>
                  <div class="setupload">
                    <Button
                      type="primary"
                      size="small"
                      icon="ios-cloud-upload"
                      @click="SeletUpload('Clientimg')"
                    >传照</Button>
                  </div>
                  <div class="setupload">
                    <Button
                      type="primary"
                      icon="md-reverse-camera"
                      @click="fromphotos('Clientimg')"
                    >拍照</Button>
                  </div>
                </li> -->
                <li style="clear: both;">
                  <div v-for="(item,index) in waybillinfo.Files">
                    <div v-if="item.Type==8010" class="setimgcolor">
                      <span >{{item.CustomName}}</span>
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
                    </div>
                  </div>
                  <p v-if="waybillinfo.WaybillType==2&&waybillinfo.ExcuteStatus==215||waybillinfo.WaybillType==1&&waybillinfo.ExcuteStatus==215">单据照片：</p>
                  <div v-for="(item,index) in waybillinfo.Files">
                    <div v-if="item.Type==8020" class="setimgcolor">
                      <span >{{item.CustomName}}</span>
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Clientimg')"></Icon>
                    </div>
                  </div>
                </li>
              </ul>
            </Col>
          </Row>
          <Modal v-model="showChildrencode" title="录入子运单" @on-visible-change="visiblechange">
            <div>
              <Input
                v-model="Childrencode"
                placeholder="请输入子运单号"
                style="width: 300px"
                @on-blur="testchildrencode"
              />
              <Button type="primary" @click="Submitcode">保存</Button>
            </div>
            <h2 style="padding:8px 0px;">子运单号列表</h2>
            <ul class="listcode" v-if="Childrencodearr.length>0">
              <li v-for="(item,index) in Childrencodearr" :key="index">
                <span>{{item}}</span>
                <Icon type="ios-trash" @click="sliptitem(index)" />
              </li>
            </ul>
            <div v-else style="text-align: center;border: 1px solid #dddddd;padding: 10px;">
              <h3>暂无数据</h3>
            </div>
            <div slot="footer">
              <Button type="primary" @click="Submitall">确定</Button>
            </div>
          </Modal>
        </div>
      </div>
      <div class="itembox">
        <p class="detailtitle">产品清单</p>
        <div style="margin:15px 0">
          <ButtonGroup style="width:28%">
            <Input
              v-model.trim="searchkey"
              placeholder="输入筛选内容"
              style="width:80%;float:left;position: relative;left: 3px"
            />
            <Button style="float:left" type="primary">筛选</Button>
            <!-- <Button style="float:left" @click="search_pro" type="primary">筛选</Button> -->
          </ButtonGroup>
          <ButtonGroup  :class="waybillinfo.ExcuteStatus==215?'':'allprintstyle'">
            <Button
              type="primary"
              :disabled="waybillinfo.ExcuteStatus==215?true:false"
              style="padding-left: 5px;padding-right: 5px;"
              @click="allprint"
            >一键打印</Button>
            <Dropdown @on-click="printdata" trigger="click">
              <Button
                type="primary"
                :disabled="waybillinfo.ExcuteStatus==215?true:false"
                style="padding-left: 5px;padding-right: 5px;"
              >
                <Icon type="ios-arrow-down"></Icon>
              </Button>
              <DropdownMenu slot="list">
                <DropdownItem name="outprint">出库单打印</DropdownItem>
                <!-- <DropdownItem name="waybuillprint" :disabled="isdisabledFn">运单打印</DropdownItem> -->
                <DropdownItem name="waybuillprint" v-if="waybillinfo.WaybillType!=1">运单打印</DropdownItem>
                <DropdownItem name="Boxing_print">送货单打印</DropdownItem>
              </DropdownMenu>
            </Dropdown>
          </ButtonGroup>

          <Button type="primary" @click="showBudget">收支明细</Button>
          <!-- <Button type="primary" @click="routelinktest">代转运跳转测试</Button> -->
          <div style="float:right">
            <Button
              type="primary"
              shape="circle"
              icon="md-checkmark"
              @click="finish_btn"
              :disabled="waybillinfo.ExcuteStatus==215?true:false"
            >出库完成</Button>
            <Button
              :disabled="waybillinfo.ExcuteStatus==215?true:false"
              type="warning"
              shape="circle"
              icon="ios-alert-outline"
              @click="isAbnormal=true"
            >出库异常</Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table
              :loading="loading"
              :columns="listtitles"
              :data="detailitem"
              :row-class-name="rowClassName"
              @on-selection-change="handleSelectRow"
            >
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <!-- <template slot-scope="{ row, index }" slot="check_statu">
                <Tag type="border" color="primary" v-if="row.Checked==false">未复核</Tag>
                <Tag type="border" color="primary" v-else>未复核</Tag>
              </template>-->
              <template
                slot-scope="{ row, index }"
                slot="PartNumber"
                v-if="row.Product!=undefined"
              >{{row.Product.PartNumber}}</template>
              <template
                slot-scope="{ row, index }"
                slot="Manufacturer"
                v-if="row.Product!=undefined"
              >{{row.Product.Manufacturer}}</template>
              <template slot-scope="{ row, index }" slot="DateCode">{{row.DateCode}}</template>
              <template slot-scope="{ row, index }" slot="ShelveID">{{row.ShelveID}}</template>
              <template slot-scope="{ row, index }" slot="StockQuantity">{{row.StockQuantity}}</template>
              <template slot-scope="{ row, index }" slot="Quantity">{{row.Quantity}}</template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="(item,index) in row.Files" v-if="item.Type==8010" class="setimgcolor">
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
                    type="primary"
                    size="small"
                    icon="ios-cloud-upload"
                    @click="SeletUpload(row.ID)"
                  >传照</Button>
                </div>
                <div class="setupload">
                  <!-- <Button type="primary" icon="md-reverse-camera" @click="photoing(row)">拍照</Button> -->
                  <Button type="primary" icon="md-reverse-camera" @click="fromphotos(row.InputID)">拍照</Button>
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
                :disabled="waybillinfo.ExcuteStatus==215?true:false"
              >出库完成</Button>
              <!-- <Button type="error">到货异常</Button> -->
              <Button
                type="warning"
                icon="ios-alert-outline"
                @click="isAbnormal=true"
                :disabled="waybillinfo.ExcuteStatus==215?true:false"
              >出库异常</Button>
            </div>
          </div>
        </div>
      </div>
      <div v-if="WarehousingMsg==true">
        <Modal
          v-model="WarehousingMsg"
          title="确定出库"
          @on-ok="ok_Warehousing"
          @on-cancel="cancel_Warehousing"
        >
          <div>
            <span>是否进行出库操作</span>
          </div>
        </Modal>
      </div>

      <!-- 异常到货 开始-->
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
          <em class="Mustfill">*</em>出库异常原因
        </span>
        <Input v-model="Summary.Summary" type="textarea" :rows="2" placeholder="备注" />
        <div slot="footer">
          <Button @click="closeerror">取消</Button>
          <Button type="primary" @click="Abnormal_btn">确定</Button>
        </div>
      </Modal>
      <!-- 异常到货 结束-->

      <!-- 收支明细 开始 -->
      <Modal
        v-model="Budgetdetail"
        width="55%"
        :closable="false"
        :mask-closable="false"
        :footer-hide="true"
      >
        <div style="clear: both; overflow: hidden;font-size:16px;padding-bottom:10px">
          <!-- <Icon type="ios-information-circle" @click="closeBudget"></Icon> -->
          <!-- <h1 style="50%;float:left;">录入收支信息</h1> -->
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
    </div>
    <!-- 历史到货 开始 -->
    <Drawer :closable="false" :width="70" v-model="historydata">
      <Historys-dom :key="historydetail.times"></Historys-dom>
    </Drawer>
    <!-- 历史到货 结束 -->

    <!-- 运单打印  开始 -->
    <Drawer :width="70" v-model="waybillprintbox">
      <div v-if="waybuillform!=null">
        <div>
          <p class="detailtitle">托寄物详细资料</p>
          <div class="waybillprintcss">
            <ul>
              <li style="padding-bottom:15px;">
                <label>
                  运&nbsp;&nbsp;单&nbsp;号：
                  <Input
                    v-model="waybuillform.Code"
                    style="width:80%"
                    size="large"
                    placeholder="发货件数"
                  />
                </label>
                <br />
              </li>
              <li style="padding-bottom:15px;">
                <label>
                  <em class="Mustfill">*</em>件&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;数：
                  <Input
                    v-model="waybuillform.TotalParts"
                    style="width:80%"
                    size="large"
                    placeholder="发货件数"
                  />
                </label>
                <br />
              </li>
              <li style="padding-bottom:15px;">
                <label>
                  重&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;量：
                  <Input
                    v-model="waybuillform.TotalVolume"
                    style="width:80%"
                    size="large"
                    placeholder="请输入重量"
                  />
                </label>
                <br />
              </li>
              <li>
                <label>
                  体&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;积：
                  <Input
                    v-model="waybuillform.TotalWeight"
                    style="width:80%"
                    size="large"
                    placeholder="请输入体积"
                  />
                </label>
              </li>
            </ul>
          </div>
          <div v-if="UserWareHouse!='HK01_WLT'">
            <div class="waybillprintcss">
              <label style="padding-top:15px;">
                <em class="Mustfill">*</em>承&nbsp;运&nbsp;商：
                <RadioGroup v-model="modelstype" @on-change="changwaytype">
                  <Radio
                    v-for="item in ShipperCodesarr"
                    :label="item.Value"
                    :key="item.Value"
                  >{{item.Name}}</Radio>
                </RadioGroup>
              </label>
            </div>
            <div style="padding-top:15px">
              <label>
                <span style="font-size:14px;">货运类型：</span>
                <div style="display: inline-block;">
                  <Select v-model="IGetExpType" style="width:200px">
                    <Option
                      v-for="(item,index) in GetExpTypesarr"
                      :value="item.Value"
                      :key="index"
                    >{{ item.Name }}</Option>
                  </Select>
                </div>
              </label>
            </div>
          </div>
          <!-- 香港库房特殊配置 -->
          <div v-else class="waybillprintcss">
            <label style="padding-top:15px;">
              <em class="Mustfill">*</em>承&nbsp;运&nbsp;商：
              <RadioGroup v-model="modelstypehk">
                <!-- <Radio label="香港顺丰">香港顺丰</Radio>
                        <Radio label="DHL">DHL</Radio>
                        <Radio label="UPS">UPS</Radio>
                        <Radio label="FedEx">FedEx</Radio>
                <Radio label="TNT">TNT</Radio>-->
                <Radio
                  v-for="(item,index) in localCarrier"
                  :value="item.ID"
                  :label="item.ID"
                  :key="index"
                >{{item.Name}}</Radio>
              </RadioGroup>
            </label>
          </div>
          <div class="userfrom">
            <div style="float:left" class="userinfodata">
              <h1>寄件人信息</h1>
              <div>
                <ul>
                  <li>
                    <label>
                      <em class="Mustfill">*</em>
                      <span>寄&nbsp;&nbsp;件&nbsp;&nbsp;人：</span>
                      <Input
                        v-model="waybuillform.Consignor.Contact"
                        placeholder="请输入寄件人姓名"
                        disabled
                        class="inputform"
                      />
                    </label>
                  </li>
                  <li>
                    <label>
                      <em class="Mustfill">*</em>
                      <span>电&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;话：</span>
                      <Input
                        v-model="waybuillform.Consignor.Phone"
                        placeholder="请输入寄件人电话"
                        class="inputform"
                        disabled
                      />
                    </label>
                  </li>
                  <li style="padding-bottom:10px;">
                    <label>
                      <em class="Mustfill">*</em>
                      <span>详细地址：</span>
                      <Input
                        v-model="waybuillform.Consignor.Address"
                        type="textarea"
                        class="inputform"
                        :rows="3"
                        :autosize="{maxRows: 3,minRows: 3}"
                        placeholder="请输入寄件人详细地址"
                        disabled
                      />
                    </label>
                  </li>
                </ul>
              </div>
            </div>
            <div style="float:right" class="userinfodata">
              <h1>收件人信息</h1>
              <div>
                <div>
                  <ul>
                    <li>
                      <label>
                        <em class="Mustfill">*</em>
                        <span>收&nbsp;&nbsp;件&nbsp;&nbsp;人：</span>
                        <Input
                          v-model="waybuillform.Consignee.Company"
                          placeholder="请输入收件人姓名"
                          class="inputform"
                          disabled
                        />
                      </label>
                    </li>
                    <li>
                      <label>
                        <em class="Mustfill">*</em>
                        <span>电&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 话：</span>
                        <Input
                          v-model="waybuillform.Consignee.Phone"
                          placeholder="请输入收件人电话"
                          class="inputform"
                          disabled
                        />
                      </label>
                    </li>
                    <li style="padding-bottom:10px;">
                      <label>
                        <em class="Mustfill">*</em>
                        <span>详细地址 ：</span>
                        <Input
                          v-model="waybuillform.Consignee.Address"
                          type="textarea"
                          class="inputform"
                          :rows="3"
                          :autosize="{maxRows: 3,minRows: 3}"
                          placeholder="请输入收件人详细地址"
                          disabled
                        />
                      </label>
                    </li>
                  </ul>
                </div>
              </div>
            </div>
          </div>
          <div class="printbtn">
            <Button style="padding-right:15px;" @click="closewaybull_btn">取消</Button>
            <Button type="primary" v-if="UserWareHouse!='HK01_WLT'" @click="sumbit_print">保存并打印</Button>
            <Button type="primary" v-else @click="sumbit_print">保存</Button>
          </div>
        </div>
      </div>
    </Drawer>
    <!-- 运单打印  结束 -->
    <!-- 打印出库单 开始 -->
    <Modal v-model="outprint" title="提示" @on-ok="outorder_print" @on-cancel="outorder_clear">
      <p>确认打印出库单</p>
    </Modal>
    <!-- 打印出库单 结束 -->
  </div>
</template>
<script>
import { mapActions, mapGetters } from 'vuex'
import imgtest from '@/Pages/Common/imgtes'
import Photograph from '@/Pages/Common/Photograph'
import Historys from '@/Pages/Common/Historygoods'
import {
  pickingsdetail,
  sortingupload,
  search_detail,
  getWayParter,
  Carriers,
  pickingsenter,
  initwaybill,
  GetShipperCodes,
  GetExpTypes,
  UpdateWayBillCode,
  SubCodeEnter,
  getchildencode,
  TestCheck,
  GetLocalCarriers,
  GetDriversCars,
  initwaybillenter,
  UpdateFile,
  DeleteFile
} from '../../api'
import {
  FilePrint,
  PageEvent,
  FormPhoto,
  SeletUploadFile,
  TemplatePrint,
  GetPrinterDictionary,
  PrintKdn,
  PrintOuptNotice,
  PrintDeliveryList,
  ReprintKdnFaceSheet,
  PictureShow
} from '@/js/browser.js'
// import {TemplatePrint,GetPrinterDictionary,FilePrint,FormPhoto} from "@/js/browser.js"
let Base64 = require('js-base64').Base64
let product_url = require('../../../static/pubilc.dev')
import Vue from 'vue'
import moment from 'moment'
import qs from 'qs'
import $ from 'jquery'
export default {
  name: 'RoutineEnter',
  components: {
    'img-test': imgtest,
    'photo-graph': Photograph,
    'Historys-dom': Historys
  },
  props: {
    fatherMethod: {
      type: Function,
      default: null
    }
  },
  data() {
    return {
      stringcode: '',
      Childrencodearr: [],
      Childrencode: '', //子运单号
      showChildrencode: false, //添加子运单的状态
      language: 'SC', //语言 本港繁体,国际
      disabledwaybuill: false,
      UserWareHouse: sessionStorage.getItem('UserWareHouse'),
      printurl: product_url.pfwms,
      productinfo: {
        count: '' //件数
      },
      ShipperCodesarr: [], //运单承运商
      GetExpTypesarr: [], //指定承运商类型
      outprint: false,
      waybuillform: null,
      modelstype: 'SF', //面单打印承运商
      modelstypehk: '098F3D3E2D243EA534E57A882F377BC7', //如果是香港库房，默认使用香港顺丰
      IGetExpType: 3, //默认货运类型
      waybillprintbox: false, //运单打印弹出框
      viewtime: '',
      // Budgetdetail:false,  //收支详情
      loading: false, //loading效果
      printlist: [], //打印列表
      Conveyingplace: [], //输送地列表
      ClientCode: '', //默认输送地
      model11: '',
      sethousbox: false, //一键入库弹出框
      housenumber: '', //选择的库位号
      Storehouselist: [],
      detail_ID: '',
      searchkey: '', //筛选条件
      showphoto2: false, //显示拍照弹出框,
      time: '',
      WarehousingMsg: false, //完成入库的提示 数量对与不对
      Nomatching: [], //数量不对提示型号
      details: {
        //详情页
        waybillinfo: {}, //详情运单信息
        wayBillID: '',
        total: 0,
        pageIndex: 1,
        pageSize: 10000,
        WaybillNo: '90416165067', //运单号(本次)
        Carrier: 'yunda', //承运商(本次)
        Conveyorsite: '', //输送地,
        fathertype: '' //调用拍照设备的父组件类型
      },
      uploadList: [],
      files: '',
      SelectRow: [], //多选 选择的列表
      isAbnormal: false, //是否异常到货
      remarks: '', //备注
      Reason: '外观损坏', //异常原因
      errordata: [
        //异常原因列表
        {
          value: '外观损坏',
          label: '外观损坏'
        },
        {
          value: '产品数量不相符',
          label: '产品数量不相符'
        },
        {
          value: '产品型号不相符',
          label: '产品型号不相符'
        },
        {
          value: '参数不相符（批次/产地）',
          label: '参数不相符（批次/产地）'
        },
        {
          value: '包装受潮严重',
          label: '包装受潮严重'
        },
        {
          value: '其他',
          label: '其他'
        }
      ],
      Summary: {}, //后台提供的备注信息对象
      historydata: false, //历史到货的抽屉
      historydetail: {
        //历史到货数据
        times: '', //时间，每次获取新的版本
        waybillLIst: [
          {
            id: '11111',
            name: '111111',
            time: '1111'
          },
          {
            id: '2222222',
            name: '22222',
            time: '2222'
          },
          {
            id: '333',
            name: '3333',
            time: '33333'
          }
        ]
      },
      company: '', //入仓号对应公司
      listtitles: [
        //详情列表标题
        {
          title: '# ',
          slot: 'indexs',
          align: 'left',
          width: 40
          // fixed: 'right'
        },

        {
          title: '型号',
          slot: 'PartNumber',
          align: 'left'
        },
        {
          title: '品牌',
          slot: 'Manufacturer',
          align: 'left'
        },
        {
          title: '批次',
          slot: 'DateCode',
          align: 'left'
        },
        {
          title: '库位',
          slot: 'ShelveID',
          align: 'left'
        },
        {
          title: '库存数量',
          slot: 'StockQuantity',
          align: 'left'
        },
        {
          title: '发货数量',
          slot: 'Quantity',
          align: 'left'
        },
        {
          title: '毛重(kg)',
          key: 'Weight',
          align: 'center',
          render: (h, params) => {
            var vm = this
            return h('Input', {
              props: {
                //将单元格的值给Input
                value: params.row.Weight,
                autofocus: true
              },
              on: {
                'on-change'(event) {},
                'on-blur'(event) {
                  var reg = /^\d+(\.\d{0,2})?$/
                  var newtarget = vm.trim(event.target.value)
                  if (newtarget != '') {
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget
                      vm.detailitem[params.index] = params.row
                    } else {
                      vm.$Message.error('只能输入数字,包括两位数的小数点')
                      params.row.Weight = ''
                      event.target.value = ''
                      vm.detailitem[params.index] = params.row
                    }
                  }
                },
                'on-enter': event => {
                  var reg = /^\d+(\.\d{0,2})?$/
                  // console.log(event.target.value)
                  var newtarget = vm.trim(event.target.value)
                  // console.log(newtarget)
                  if (reg.test(newtarget) == true) {
                    params.row.Weight = newtarget
                    vm.detailitem[params.index] = params.row
                    var Inputs = params.row.Inputs
                    var StandardProducts = params.row.StandardProducts
                    var data2 = [
                      {
                        ID: '0001', //打印机ID
                        Type: 200, //打印类型 200产品  100库位
                        url: '',
                        size: {
                          width: '363',
                          height: '360'
                        },
                        data: {
                          // ID:"0001",
                          // Type:200,
                          Quantity: params.row.Quantity, //数量
                          inputsID: Inputs.ID, //id
                          Catalog: StandardProducts.Catalog, //品名
                          PartNumber: StandardProducts.PartNumber, //型号
                          Manufacturer: StandardProducts.Manufacturer, //品牌
                          Packing: StandardProducts.Packing, //包装
                          PackageCase: StandardProducts.PackageCase, //封装
                          origin: Inputs.Origin
                        }
                      }
                    ]
                    var newdata = JSON.stringify(data2)
                    //  console.log(data2)
                    //  PageEvent("{\"Name\":\"printing\",\"Value\":\""+Base64.encode(newdata)+"\"}");
                  } else {
                    vm.$Message.error('只能输入数字,包括两位数的小数点')
                    params.row.Weight = ''
                    event.target.value = ''
                    vm.detailitem[params.index] = params.row
                  }
                }
              }
            })
          }
        },
        {
          title: '复核状态',
          key: 'check_statu',
          align: 'center',
          // width: 60,
          render: (h, params) => {
            var vm = this
            if (params.row.Checked == false) {
              return h(
                'span',
                {
                  props: {
                    //将单元格的值给Input
                  },
                  style: {
                    display: 'inline-block',
                    padding: '5px 10px',
                    color: '#000000',
                    border: '1px solid #e8eaec',
                    background: '#f7f7f7'
                  }
                },
                '未复核'
              )
            } else if (params.row.Checked == true) {
              return h(
                'span',
                {
                  props: {
                    //将单元格的值给Input
                  },
                  style: {
                    display: 'inline-block',
                    padding: '5px 10px',
                    color: '#52c41a',
                    border: '1px solid #b7eb8f',
                    background: '#f6ffed'
                  }
                },
                '复核通过'
              )
            }
          }
        },
        {
          title: '图片',
          slot: 'imglist',
          align: 'center',
          width: 200
        },
        {
          title: '操作',
          slot: 'operation',
          align: 'center',
          width: 160
        }
      ],
      detailitem: [],
      CarrierList: [],
      wayBillID: this.$route.params.detailID, //详情页ID
      waybillinfo: {
        Files: []
      },
      CarArr: [], //车票号
      DriversArr: [], // 司机
      localCarrier: [
        {
          ID: '098F3D3E2D243EA534E57A882F377BC7',
          Name: '香港顺丰'
        },
        {
          ID: '53C6F3CCA240D98B5E648D93115A2426',
          Name: 'DHL'
        },
        {
          ID: '0E60AC85BC39F1E221D66A048FF164DA',
          Name: 'UPS'
        },
        {
          ID: 'F2D4A36F60A5F4DF5769E10303B7CC93',
          Name: 'FedEx'
        },
        {
          ID: '3ED8D415AB1BBAC628429A85851E53B6',
          Name: 'TNT'
        }
        // 00F9F0FC34384D91159C8D2CF40E4013	123
        // 00F9F0FC34384D91159C8D2CF40E4014	345
        // 098F3D3E2D243EA534E57A882F377BC7	香港顺丰
        // 0E60AC85BC39F1E221D66A048FF164DA	UPS
        // 3ED8D415AB1BBAC628429A85851E53B6	TNT
        // 53C6F3CCA240D98B5E648D93115A2426	DHL
        // 7B636A69BA0EAA6949375554CBB892E3	韵达
        // 83BCB3F6D0D8F422B120FF98BEB85AF5	跨越速运
        // EDB28BD0C988A537D55507E9D33144B2	本地货运
        // F2D4A36F60A5F4DF5769E10303B7CC93	FedEx
      ]
    }
  },
  filters: {
    showDate: function(val) {
      //时间格式转换
      // console.log(val)
      if (val != '') {
        if (val || '') {
          var b = val.split('(')[1]
          var c = b.split(')')[0]
          var result = moment(+c).format('YYYY-MM-DD')
          return result
        }
      }
    },
    changenumber: function(val) {
      //
      // console.log(val)
      if (val != '') {
        var newnumber = Number(val)
        console.log(newnumber)
        return newnumber
      }
    }
  },
  computed: {
    getarrs() {
      return this.Nomatching
    },
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail
    },
    isdisabledFn() {
      if (this.UserWareHouse == 'HK01_WLT') {
        this.disabledwaybuill = true
      } else {
        this.disabledwaybuill = false
      }
      return this.disabledwaybuill
    },
    setstring() {
      console.log(this.stringcode)
      return this.stringcode
    }
  },
  mounted() {
    this.$Spin.show()
    this.getdetail_data()
    let _this = this
    var strings = ''
    document.onkeydown = function(e) {
      //  console.log(e)
      if (e.keyCode != 16 && e.keyCode != 13) {
        strings += e.key
        //  console.log(this.stringcode)
      } else if (e.keyCode === 13) {
        _this.stringcode = strings
        _this.changestatus(_this.stringcode)
        strings = ''
        //   _this.stringcode="";
      }
    }
    // if(this.UserWareHouse=="HK01_WLT"){
    //     // this.modelstype="098F3D3E2D243EA534E57A882F377BC7"
    //      this.modelstype="香港顺丰"
    //      console.log(this.modelstype)
    // }
  },
  created() {
    // console.log("重新加载-------常规分拣");
    window['KdnPrinted'] = this.changed
    window['PhotoUploaded'] = this.changimgs
  },
 beforeRouteUpdate (to, from, next) {
    // console.log(this)
    console.log(to)
    next()
    // 在当前路由改变，但是该组件被复用时调用
    // 举例来说，对于一个带有动态参数的路径 /foo/:id，在 /foo/1 和 /foo/2 之间跳转的时候，
    // 由于会渲染同样的 Foo 组件，因此组件实例会被复用。而这个钩子就会在这个情况下被调用。
    // 可以访问组件实例 `this`
},
  methods: {
    changestatus(inputid) {
      for (var i = 0; i < this.detailitem.length; i++) {
        if (this.detailitem[i].InputID == inputid) {
          var _this = this
          var indexs = i
          TestCheck(this.detailitem[i].OutputID).then(res => {
            if (res.success == true) {
              _this.detailitem[indexs].Checked = true
              _this.$Message.success('复核成功')
            } else {
              _this.$Message.error('复核失败，请检查该条码是否在该出库单中')
            }
          })
        } else {
          //  this.$Message.error("复核失败，请检查该条码是否在该出库单中");
        }
      }
    },
    testwinfrom() {
      // alert(TestCallJsFuntion())
      TestCallJsFuntion()
      // alert(Zyz)
      // Zyz("jfkgdfhgjdfhgjdhgjkdfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhfhj")
    },

    trim(str) {
      //去除前后空格
      return str.replace(/(^\s*)|(\s*$)/g, '')
    },
    ok() {
      this.$Message.info('Clicked ok')
      this.showphoto2 = false
    },
    cancel() {
      this.$Message.info('Clicked cancel')
      this.showphoto2 = false
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      this.SelectRow = value
    },
    search_pro() {
      if (this.searchkey == '') {
        this.$Message.error('请输入查询条件')
      } else {
        search_detail(this.details.wayBillID, encodeURI(this.searchkey)).then(
          res => {
            if (res.obj != '') {
              this.details.total = res.obj.Total
              this.details.waybillinfo = res.waybill
              this.details.detailitem = res.obj.Data
            }
          }
        )
      }
    },
    finish_btn() {
      var isnull = null //判断实际到货数量 true 为空 false 不为空
      var that = this
      var dataarr = this.detailitem
      var null_num = 1
      //  that.Nomatching=Nomatching;
      if (this.waybillinfo.WaybillType == 2) {
        //如果是送货上门，需要填写承运商，司机，车牌号
        console.log(this.waybillinfo.CarrierID)
        if ( this.waybillinfo.CarrierID == null || this.waybillinfo.CarrierID == '' ) {
          this.$Message.error('请先选择承运商')
        } else if (this.waybillinfo.WayLoading.Driver == null) {
          this.$Message.error('请选择司机')
        } else if (this.waybillinfo.WayLoading.CarNumber1 == null) {
          this.$Message.error('请选择车牌号')
        } else {
          that.WarehousingMsg = true
        }
      } else {
        that.WarehousingMsg = true
      }
      // for (var i = 0; i < dataarr.length; i++) {
      //   if (dataarr[i].TruetoQuantity == "") {
      //     isnull = true;
      //     null_num++;
      //   } else {
      //     isnull = false;
      //     // null_num=0
      //   }
      // }
    },
    setWarehousing(data) {
      //确定出库，调取后台出库接口
      pickingsenter(data).then(res => {
        if (res.val == 0) {
          //成功之后调用父组件的关闭
          this.$Message.success('出库完成，一秒后自动关闭')
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
             this.$Message.error('库存不足，无法出库')
        }else {
          this.$Message.error('出库操作失败,请从新操作')
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
      // console.log(this.waybillinfo)
      // alert(JSON.stringify(this.waybillinfo))
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
    changeimgs(newdata, row) {
      //上传照片
      //通过子组件传递的数据改变父组件的值
      var arr = this.detailitem
      for (var j = 0, arrlien = arr.length; j < arrlien; j++) {
        if (arr[j].ID == row.ID) {
          arr[j].Files.push(newdata)
        }
      }
    },

    getdetail_data() {
      var data = {
        warehouseID: sessionStorage.getItem('UserWareHouse'),
        wayBillID: this.wayBillID
      }
      pickingsdetail(data).then(res => {
        this.waybillinfo = res.obj
        this.detailitem = res.obj.Notices
        this.$Spin.hide()
        if (this.waybillinfo.WaybillType == 2) {
          //如果是送货上门，需要调用送货上门的承运商
          this.GetLocalCarriers()
        } else {
          this.Carriers()
        }
      })
    },
    detailspage(value) {
      //分页
      // var data={
      //   wayBillID:this.details.wayBillID,
      //   pageIndex:this.details.pageIndex,
      //   pageSize:this.details.pageSize,
      // }
      this.details.pageIndex = value
      this.getdetail_data(this.details.wayBillID)
    },
    handleBeforeUpload(file) {
      // 创建一个 FileReader 对象
      let reader = new FileReader()
      // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
      // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
      // 读取文件作为 URL 可访问地址
      reader.readAsDataURL(file)
      const _this = this
      reader.onloadend = function(e) {
        file.url = reader.result
        var newimg = {
          AdminID: '',
          ClientID: '',
          CreateDate: '',
          CustomName: file.name,
          FileBase64Code: file.url,
          ID: '',
          InputID: '',
          LocalFile: '',
          Status: 0,
          StatusDes: '',
          StorageID: '',
          Type: 0,
          TypeDes: '',
          Url: '',
          WaybillID: ''
        }
        _this.waybillinfo.Files.push(newimg)
      }
      return false
    },
    handleRemove(file,type) {
      if(type=="Clientimg"){
        DeleteFile(file.ID).then(res=>{
          console.log(res)
          if(res.val==0){
            this.waybillinfo.Files.splice(this.waybillinfo.Files.indexOf(file), 1)
          }else{
            this.$Message.error('删除失败')
          }
        })
      }else{
        this.waybillinfo.Files.splice(this.waybillinfo.Files.indexOf(file), 1)
      }
    },
    handleFormatError(file) {
      this.$Notice.warning({
        title: '文件格式不正确',
        desc:
          '文件 ' + file.name + ' 格式不正确，请上传 jpg 或 png 格式的图片。'
      })
    },
    handleMaxSize(file) {
      this.$Notice.warning({
        title: '超出文件大小限制',
        desc: '文件 ' + file.name + ' 太大，不能超过 2M。'
      })
    },
    handleRemovelist(row, file) {
      var arr = this.detailitem
      for (var j = 0; j < arr.length; j++) {
        //删除指定下标 的元素
        if (arr[j].ID == row.ID) {
          arr[j].Files.splice(file, 1)
        }
      }
    },

    //拍照方法的调用
    closephoto() {
      //关闭按钮 父组件关闭子组件摄像头
      this.$refs.photograph.closeCamera()
      var that = this
      setTimeout(function() {
        that.showphoto2 = false
      }, 20)
    },
    upload_btn() {
      //上传图片
      this.$refs.photograph.uploadphoto()
      // this.$refs.photograph.closeCamera();
      var that = this
      setTimeout(function() {
        that.showphoto2 = false
      }, 20)
    },
    photoing(type) {
      //打开拍照组件
      console.log(type)
      this.fathertype = type
      this.showphoto2 = true
      // 一. 通过时间戳加载，会调用初始化数据
      // this.time = new Date().getTime()

      //  二。将子组件初始值设为空
      this.$refs.photograph.closeCamera()
      ;(this.$refs.photograph.list = []), (this.$refs.photograph.model1 = '')
      this.$refs.photograph.setCamera()
      this.$refs.photograph.callCamera()

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
      console.log(this.fathertype)
      if (this.fathertype == 'waybill') {
        //运单照片列表
        this.details.waybillinfo.Files.push(naedata)
      } else {
        //
        var arr = this.detailitem
        for (var j = 0; j < arr.length; j++) {
          if (arr[j].ID == this.fathertype.ID) {
            arr[j].Files.push(naedata)
          }
        }
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
    errorstatue(value) {
      if (value == true) {
        this.isAbnormal = true
      } else {
        this.isAbnormal = false
        this.Summary.Summary = ''
      }
    },
    showhistory() {
      //展示历史订单组件
      this.historydata = true
      this.historydetail.times = new Date().getTime()
    },
    historywaybill(name) {
      console.log(name)
      this.showhistory()
    },
    WayParterdata() {
      //输送地列表
      getWayParter().then(res => {
        console.log(res)
        this.Conveyingplace = res.obj
      })
    },

    changConveyingplace(value, row) {
      //改变输送地 与通知项的原产地地址
      console.log(value)
      console.log(row)
      if (row == 'ClientCode') {
        //运单的输送地
        this.details.waybillinfo.CorPlace = value.value
        this.details.waybillinfo.CorPlaceDes = value.label
      } else {
        //商品的原产地
        // row.
      }
    },
    showBudget() {
      //展开收支明细
      // this.Budgetdetail=true;
      // this.viewtime=new Date().getTime()
      // console.log(this.$route.params)
      this.$store.dispatch('setBudget', true)
      // this.$router.push({ path:"/Outgoing/outdetail/Budget"})
      // this.$router.push({
      //   path: "/Outgoing/outdetail/Budget?webillID=" + this.wayBillID
      // });
      this.$router.push({
        name: 'ends_meetout',
        params: {
          webillID: this.wayBillID,
          otype:"out"
        }
      })
    },
    closeBudget() {
      //关闭收支明细
      console.log('关闭收支明细')
      this.$router.go(-1)
      // this.Budgetdetail=false;
      this.$store.dispatch('setBudget', false)
    },
    // 承运商列表
    Carriers() {
      Carriers().then(res => {
        this.CarrierList = res.obj
      })
    },
    GetLocalCarriers() {
      //送货上门承运商
      GetLocalCarriers().then(res => {
        let that = this
        this.CarrierList = res.obj
        if (this.CarrierList.length > 0 ) {
          var arr=this.CarrierList.filter(function(item){
            return item.ID == that.waybillinfo.CarrierID
          })
          if (arr.length) {
            this.GetDriversCars(arr[0].ID)
          }
        } else {
        }
      })
    },
    GetDriversCars(key) {
      //根据送货上门承运商获取司机与车牌号
      GetDriversCars(key).then(res => {
        console.log(res)
        this.CarArr = res.obj.cars //车票号
        this.DriversArr = res.obj.drivers // 司机
        // if (this.DriversArr.length > 0) {
        //   this.waybillinfo.WayLoading.Driver = this.DriversArr[0].ID
        // }
        // if (this.CarArr.length > 0) {
        //   this.waybillinfo.WayLoading.Driver = this.CarArr[0].ID
        // }
      })
    },
    fromphotos(type) {
      if (type == 'Waybill') {
        var data = {
          SessionID: this.waybillinfo.WaybillID,
          AdminID: sessionStorage.getItem('userID')
        }
        FormPhoto(data)
      } else {
        var data = {
          SessionID: type,
          AdminID: sessionStorage.getItem('userID')
        }
        FormPhoto(data)
      }
    },
    changed(message) {
      //调用打印运单号，返回运单号的值
      console.log(this.waybillinfo.Code)
      this.waybillinfo.Code = message
      this.waybuillform.Code = message
      // var data={
      //   code:message
      // }
      // UpdateWayBillCode(data).then(res=>{  //将打印的运单号传递给后台
      //   alert(JSON.stringify(res))
      // })
      this.initwaybillenter(this.modelstype)
    },
    changimgs(message) {
      this.testfunction(message)
    },
    SetUpdateFile(id,waybillID,customName,type,newfile){
      UpdateFile(id,waybillID,customName,type).then(res=>{
        if(res.val==0){
          this.waybillinfo.Files.push(newfile)
        }else{
          this.$Message.error('上传失败，请重新上传')
        }
      })
    },
    testfunction(message) {
      //前台处理数据的方法图片文件
      var imgdata = message[0]
      var Typeimg=""
      if(imgdata.SessionID=="Clientimg"){
        Typeimg=8020
      }else{
        Typeimg=8010;
      }
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: Typeimg
      }
      if (imgdata.SessionID == this.waybillinfo.WaybillID) {
        this.waybillinfo.Files.push(newfile)
      } else if(imgdata.SessionID=='Clientimg'){  //如果是收获文件照片。则上传到后台
        this.SetUpdateFile(imgdata.FileID,this.waybillinfo.WaybillID,imgdata.FileName,8020,newfile)
        
      }
      else {
        for (var i = 0; i < this.detailitem.length; i++) {
          if (this.detailitem[i].ID == imgdata.SessionID) {
            this.detailitem[i].Files.push(newfile)
          }
        }
      }
    },
    SeletUpload(ID) {
      // 传照
      var data = {
        SessionID: ID,
        AdminID: sessionStorage.getItem('userID')
      }
      console.log(data)
      SeletUploadFile(data)
    },
    printdata(value) {
      //选择打印类型
      console.log(value)
      if (value == 'waybuillprint') {
        this.waybillinfon()
        this.GetShipperCodes()
        this.GetExpTypes('SF')
        this.waybillprintbox = true
      } else if (value == 'outprint') {
        this.outprint = true
      } else {
        //  this.waybillprintbox = true;
        this.Boxing_print()
      }
    },
    changwaytype(value) {
      //运输类型切换
      // this.waybuillform.CarrierID=value;
      // this.GetExpTypes(this.waybuillform.CarrierID)
      this.modelstype = value
      this.GetExpTypes(this.modelstype)
    },
    sumbit_print() {
      //运单打印功能
      // TemplatePrint(data);
      if (this.UserWareHouse != 'HK01_WLT') {
        //如果不是万路通库房，则可以打印运单
        this.waybillprint()
      } else {
        this.initwaybillenter(this.modelstypehk)
      }
    },

    initwaybillenter(itemmodels) {
      // 保存运单信息
      var data = {
        ID: this.waybillinfo.WaybillID, //运单ID
        Code: this.waybuillform.Code, //运单号
        TotalParts: this.waybuillform.TotalParts, //件数
        TotalWeight: this.waybuillform.TotalWeight, //重量
        TotalVolume: this.waybuillform.TotalVolume, //体积
        CarrierID: itemmodels //承运商ID
      }
      console.log(data)
      initwaybillenter(data).then(res => {
        if (res.success == true) {
          this.$Message.success('保存成功，两秒后自动关闭')
          var _this = this
          setImmediate(() => {
            this.waybillprintbox = false
          }, 2000)
        } else {
          this.$Message.error('保存失败，请检查相关信息')
        }
      })
    },
    outorder_print() {
      //出库单打印功能
      var data = {
        data: this.waybillinfo
      }
      PrintOuptNotice(this.waybillinfo)
    },
    Boxing_print() {
      //装箱单打印 拣货单 送货单
      if (this.waybillinfo.WaybillType == 4) {
        //国际
        this.language = 'EN'
      } else if (this.waybillinfo.WaybillType == 3) {
        //本港
        this.language = 'TC'
      } else {
        this.language = 'SC'
      }
      var data = {
        Language: this.language,
        data: this.waybillinfo
      }
      PrintDeliveryList(data)
    },
    waybillprint() {
      //运单打印
      var printdata = {
        ShipperCode: this.modelstype, //快递公司
        ExpType: this.IGetExpType, //货运类型
        Sender: {
          //发件人
          Company: this.waybuillform.Consignor.Company,
          Name: this.waybuillform.Consignor.Contact,
          Mobile: this.waybuillform.Consignor.Phone,
          Address: this.waybuillform.Consignee.Address //暂时调用收货人地址
        },
        Receiver: {
          //收件人
          Company: this.waybuillform.Consignee.Company,
          Name: this.waybuillform.Consignee.Contact,
          Mobile: this.waybuillform.Consignee.Phone,
          Address: this.waybuillform.Consignee.Address
        },
        Quantity: this.waybuillform.TotalParts, //数量
        Remark: this.waybuillform.Summary, //备注
        volume: this.waybuillform.TotalVolume, //重量
        Weight: this.waybuillform.TotalWeight, //体积
        Commodity: [{ GoodsName: '客户器件' }] //
      }
      // console.log(this.waybillinfo.Code)
      if (this.waybillinfo.Code != '') {
        ReprintKdnFaceSheet(this.waybillinfo.Code)
      } else {
        PrintKdn(printdata)
      }
      // PrintKdn(printdata)
      // console.log(printdata)
    },
    outorder_clear() {
      this.outprint = false
    },
    allprint() {
      //一键打印
      var housname = sessionStorage.getItem('UserWareHouse')
      if (housname == 'HK01_WLT') {
        //香港库房没有面单打印
      } else {
        if (this.waybillinfo.CarrierName == '顺丰快递') {
          console.log(this.waybillinfo.CarrierName)
          this.modelstype = 'SF'
          this.IGetExpType = 3
          this.waybillprint()
        } else if (this.waybillinfo.CarrierName == '跨越速运集团有限公司') {
          this.modelstype = 'KYSY'
          this.IGetExpType = 2
          this.waybillprint()
        }
      }
      // if(this.waybillinfo.CarrierName=="顺丰快递"){
      //     console.log(this.waybillinfo.CarrierName)
      //    this.modelstype="SF";
      //    this.IGetExpType=3;
      //    this.waybillprint()
      //   }else if(this.waybillinfo.CarrierName=="跨越速运集团有限公司"){
      //     this.modelstype="KYSY";
      //     this.IGetExpType=2;
      //     this.waybillprint()
      //   }

      if (
        this.waybillinfo.WaybillType == 2 ||
        this.waybillinfo.WaybillType == 1
      ) {
        //自提或送货上门，打印两份装箱单
        this.Boxing_print()
        this.Boxing_print()
      } else {
        this.Boxing_print()
      }
      this.outorder_print() //出库通知单(拣货单)必打
    },
    fileprint(printurl) {
      var configs = GetPrinterDictionary()
      var getsetting = configs['文档打印']
      getsetting.Url = printurl
      var data = getsetting
      FilePrint(data)
    },
    waybillinfon() {
      //获取运单信息（面单上的信息）
      initwaybill(this.waybillinfo.WaybillID).then(res => {
        // console.log(res);
        this.waybuillform = res
      })
    },
    GetShipperCodes() {
      //获取承运商信息
      GetShipperCodes().then(res => {
        console.log(res)
        // this.waybuillform=res;
        this.ShipperCodesarr = res
      })
    },
    GetExpTypes(id) {
      //获取承运商对应的承运类型
      if (id == 'SF') {
        this.IGetExpType = 3
      } else {
        this.IGetExpType = 2
      }
      GetExpTypes(id).then(res => {
        this.GetExpTypesarr = res
        // this.waybuillform=res;
      })
      console.log(this.IGetExpType)
    },
    changeacrrier(value) {
      //改变承运商的时候触发
      console.log(value)
      this.waybillinfo.CarrierName = value.label
      if (this.waybillinfo.WaybillType == 2) {
        this.GetDriversCars(value.value)
      } else {
      }
    },
    changedrivers(value) {
      console.log(this.waybillinfo)
    },
    rowClassName(row, index) {
      // console.log(this.stringcode)
      if (this.searchkey != '') {
        // return 'demo-table-info-row';
        if (
          row.Product.PartNumber == this.searchkey ||
          row.Product.Manufacturer == this.searchkey
        ) {
          return ''
        } else {
          return 'demo-table-info-row'
        }
      } else {
        return ''
      }

      // return "";
    },
    setcode() {
      this.showChildrencode = true
      this.getchildencode(this.waybillinfo.WaybillID)
    },
    Submitcode() {
      if (this.Childrencode == '') {
        this.$Message.error('请输入运单号')
      } else if (this.Childrencodearr.length >= 8) {
        this.$Message.error('最多可输入8个子运单')
      } else {
        this.Childrencodearr.push(this.Childrencode)
        this.Childrencode = ''
      }
    },
    testchildrencode() {
      var aa = /^[A-Za-z0-9]+$/
      var bb = aa.test(this.Childrencode)
      if (bb == false) {
        this.$Message.error('请输入正确的运单号')
        this.Childrencode = ''
      } else {
      }
    },
    Submitall() {
      //提交子运单
      if (this.Childrencodearr.length > 0) {
        var newcode = this.Childrencodearr.join(',')
        var data = {
          waybillID: this.waybillinfo.WaybillID,
          subCodes: newcode
        }
        console.log(data)
        SubCodeEnter(data).then(res => {
          console.log(res)
          if (res.success == true) {
            this.$Message.success('提交成功')
            this.getchildencode(this.waybillinfo.WaybillID)
            var that = this
            that.showChildrencode = false

            // setTimeout(function() {
            //   that.showChildrencode = false
            // }, 3000)
          } else {
            this.$Message.error('提交失败,请重试')
          }
        })
      } else {
        this.$Message.error('请输入子运单')
      }
    },
    sliptitem(index) {
      //删除子运单
      this.Childrencodearr.splice(index, 1)
    },
    getchildencode(id) {
      //获取子运单
      getchildencode(id).then(res => {
        if (res != '') {
          this.Childrencodearr = res.split(',')
        } else {
          this.Childrencodearr = []
        }
      })
    },
    visiblechange(value) {
      if (value == true) {
        this.showChildrencode = true
        this.getchildencode(this.waybillinfo.WaybillID)
      } else {
        this.showChildrencode = false
      }
    },
    PictureShow(url) {
      var data = {
        Url: url
      }
      PictureShow(data)
    },
    closewaybull_btn() {
      this.waybillprintbox = false
    },
    routelinktest(){//代转运测试出库关闭按钮
          this.$store.dispatch('setshowtype', 0)
          this.$store.dispatch('setshowdetailout', false)
          // this.$router.go(-1); 
          this.$router.push({ path:"/Outgoing"})
    }
  }
}
</script>
