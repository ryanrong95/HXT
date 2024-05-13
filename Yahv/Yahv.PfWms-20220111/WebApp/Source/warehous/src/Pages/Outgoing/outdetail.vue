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
/* .ivu-table .demo-table-info-row td {
  display: none;
}
.ivu-table .demo-table-error-row td {
  background-color: #edfff3;
} */
  .ivu-table .demo-table-info-row td{
        background-color: #2db7f5;
        color: #fff;
    }
    .ivu-table .demo-table-error-row td{
        background-color: #ff6600;
        color: #fff;
    }
.childeninput {
  width: 80%;
  margin-left: 10px;
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
.linkurlcolor{
  color: #2d8cf0;
}
.Filesbox:hover{
  cursor: pointer;
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
                <span>{{waybillinfo.ID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状态：</span>
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
              <li class="itemli">
                <span class="detail_title1">总货值：</span>
                <span style="color:'#ccc';font-width:600">{{waybillinfo.chgTotalCurrency}} {{waybillinfo.chgTotalPrice}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">总&nbsp;&nbsp;额&nbsp;&nbsp;度：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.total}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">是否逾期：</span>
                <span style="color:'#ccc';font-width:600" v-if="waybillinfo.overDue==true">是</span>
                <span style="color:'#ccc';font-width:600" v-else>否</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Condition!=undefined">
                <Icon type="md-alert" v-if="waybillinfo.Condition!=undefined" style="font-size: 22px;color: #da2828;" />
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
                <Tag color="#FFA2D3" v-if="waybillinfo.Condition.IsTurnDeclare==true">转报关</Tag>
              </li>
            </ul>
            </Col>
            <Col style="width: 24%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title2">制单时间：</span>
                <span v-if="detailitem.length>0">{{detailitem[0].CreateDate|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2" style="float: left;">供&nbsp;&nbsp;应&nbsp;&nbsp;商：</span>
                <span style="display: inline-block; width: 78%; line-height: 1;">
                  <p v-for="(item,index) in waybillinfo.Supplier">{{item.Supplier}}&nbsp;&nbsp;<Tag color="volcano">LV {{item.SupplierGrade}}</Tag></p>
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title2">发货方式：</span>
                <!-- <span>{{waybillinfo.TypeName}}</span> -->
                <span v-if="warehouseID.indexOf('SZ')!=-1||(waybillinfo.Type==1||waybillinfo.Type==2) ">{{waybillinfo.TypeName}}</span>
                <Select v-else
                        v-model="waybillinfo.Type"
                        style="width:40%"
                        @on-change='changewaybillType'>
                  <Option v-for="(item,index) in TypeArr"
                          :value="item.value"
                          :key="index">
                    {{ item.label}}
                  </Option>
                </Select>
              </li>
              <li class="itemli">
                <span class="detail_title2">总&nbsp;&nbsp;欠&nbsp;&nbsp;款：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.totalDebt}}</span>
              </li>
              <li class="itemli" v-if="DelivaryOpportunity!=null">
                <span class="detail_title2">送货时机：</span>
                <span style="color:'#ccc';font-width:600">{{DelivaryOpportunity}}</span>
              </li>
              <li class="itemli" v-if="ispay!=null">
                <span class="detail_title2">收款状态：</span>
                <span style="color:'#ccc';font-width:600">{{ispay}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title2">自提时间：</span>
                <span>{{waybillinfo. AppointTime|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">特殊标签：</span>
                <span style="line-height: 20px;width: 70%;display: inline-block;">
                  <div v-for="(item,index) in waybillinfo.LableFile">
                    <p>
                      <a>{{item.CustomName}}</a>
                      <a @click="fileprint(item.Url)">打印</a>
                    </p>
                  </div>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title2" style="float:left">随货文件：</span>
                <span style="float:left">
                  <div v-for="(item,index) in waybillinfo.Files" v-if="item.Type==25">
                    <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
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
                <span>{{waybillinfo.EnterCode}}（{{waybillinfo.ClientName}}）<Tag color="geekblue">LV {{clientGrade}}</Tag>
                <i style=" background: #f90; color: #ffffff; padding: 6px; border-radius: 50%; font-size: 17px;" v-if="waybillinfo.IsClientLs==true">租</i>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3">提货人：</span>
                <span>{{waybillinfo.coeContact}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3">提货人证件：</span>
                <span>{{waybillinfo.IDTypeName}}（{{waybillinfo.IDNumber}}）</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3">提货人电话：</span>
                <span>{{waybillinfo.coePhone}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3">运单号(本次)：</span>
                <span>
                  <Input style="width:60%" v-model="waybillinfo.Code" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3"><em v-if="waybillinfo.Type==2" class="Mustfill">*</em>承运商(本次):</span>
                <span>
                  <Select v-model="waybillinfo.CarrierID"
                          style="width:60%"
                          @on-change="changeacrrier"
                          :label-in-value="true">
                    <Option v-for="item in CarrierList"
                            :value="item.ID"
                            :key="item.ID">
                      {{ item.Name }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCarrier=true" v-if="waybillinfo.Type==2" />
                </span>
              </li>

              <li class="itemli" v-if="waybillinfo.Type==2&&IsPersonal==false">
                <span class="detail_title3"><em class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Select v-model="waybillinfo.Driver"
                          style="width:60%"
                          :label-in-value="true">
                    <Option v-for="item in DriversArr"
                            :value="item.Name"
                            :key="item.ID">
                      {{ item.Name }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddDriver=true" v-if="waybillinfo.Type==2&&waybillinfo.CarrierID!=null" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2&&IsPersonal==false">
                <span class="detail_title3"><em class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Select v-model="waybillinfo.CarNumberID"
                          style="width:60%"
                          :label-in-value="true">
                    <Option v-for="item in CarArr"
                            :value="item.value"
                            :key="item.ID">
                      {{ item.value }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCar=true" v-if="waybillinfo.Type==2&&waybillinfo.CarrierID!=null" />
                </span>
              </li>

              <li class="itemli" v-if="waybillinfo.Type==2&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Input v-model="waybillinfo.Driver" style="width:60%" placeholder="请输入司机" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Input v-model="waybillinfo.CarNumberID" style="width:60%" placeholder="请输入车牌号" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3">收&nbsp;&nbsp;货&nbsp;&nbsp;人：</span>
                <span>{{waybillinfo.coeContact}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3">收货人电话：</span>
                <span>{{waybillinfo.coePhone}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3" style="float: left;">收货地址：</span>
                <span style="display: inline-block;max-width:60%;line-height: 20px;">
                  <i> {{waybillinfo.CoeAddress}}</i>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3" style="float:left">随货文件：</span>
                <span style="display:">
                  <div v-for="(item,index) in waybillinfo.Files" v-if="item.Type==25">
                    <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                    <a @click="fileprint(item.Url)">打印</a>
                  </div>
                </span>
              </li>
            </ul>
            </Col>
            <Col style="width: 20%;float: left;">
            <ul class="detail_li" style="margin-left:20px;">
              <li class="itemli">备注：{{waybillinfo.Summary}}</li>
              <li class="itemli">
                <span style="float:left;line-height: 27px;" v-if="waybillinfo.ExcuteStatus!=215">发货情况拍照：</span>
                <div v-if="waybillinfo.ExcuteStatus!=215">
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
                  </div>
                </div>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2&&waybillinfo.ExcuteStatus=='215'||waybillinfo.Type==1&&waybillinfo.ExcuteStatus=='215'" style="padding-top:10px;clear: both;">
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
                <div>
                  <p>发货情况照片：</p>
                  <div v-for="(item,index) in waybillinfo.FeliverGoodFile">
                    <div v-if="item.Type==8010" class="Filesbox">
                      <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                      <Tooltip content="删除" placement="top">
                        <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                      </Tooltip>
                      <Tooltip content="打印" placement="top">
                        <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                      </Tooltip>
                    </div>
                  </div>
                </div>
                <div v-if="waybillinfo.Type==2&&waybillinfo.ExcuteStatus==215||waybillinfo.Type==1&&waybillinfo.ExcuteStatus==215">
                  <p>送货单照片：</p>
                  <div v-for="(item,index) in waybillinfo.SendGoodsFile">
                    <div v-if="item.Type==8020" class="Filesbox">
                      <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                      <Tooltip content="删除" placement="top">
                        <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Clientimg')"></Icon>
                      </Tooltip>
                      <Tooltip content="打印" placement="top">
                        <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                      </Tooltip>
                    </div>
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
                   placeholder="输入型号或品牌"
                   clearable
                   @on-clear='search_pro'
                   style="width:80%;float:left;position: relative;left: 3px" />
            <!-- <Button style="float:left" @click="search_pro" type="primary" :disabled='true'>筛选</Button> -->
            <Button style="float:left" @click="search_pro" type="primary">筛选</Button>
          </ButtonGroup>
          <Button type="primary" @click="showprint=true">一键打印</Button>
          <Button type="primary" @click="showBudget('meet','in',waybillinfo.Source)">收入</Button>
          <Button type="primary" @click="showBudget('meet','out',waybillinfo.Source)">支出</Button>
          <Button type="primary" v-if='warehouseID.indexOf("HK")!=-1' @click="showneed = true">特殊要求</Button>
          <Button type="primary" @click="showBudget2" v-if="showStoragecharge==true">仓储费录入</Button>
          <span v-if="showStoragecharge==true" style="font-size:11px;color:red">（请收取仓储费，总共需收取仓储费的时长：{{timedifference}}天）</span>
          <div style="float:right">
            <Button type="primary"
                    shape="circle"
                    icon="md-checkmark"
                    @click="finish_btn"
                    :disabled="isdisabled==true?true:false">
              出库完成
            </Button>
            <Button v-if='warehouseID.indexOf("HK")==-1'
                    :disabled="isdisabled==true?true:false"
                    type="warning"
                    shape="circle"
                    icon="ios-alert-outline"
                    @click="Abnormalsettrue">
              出库异常
            </Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table :loading="loading"
                   :columns="listtitles"
                   :data="detailitem"
                   @on-selection-change="handleSelectRow">
              <template slot-scope="{ row, index }"
                        slot="PartNumber"
                        v-if="row.Product!=undefined">
                {{row.Product.PartNumber}}
              </template>
              <template slot-scope="{ row, index }"
                        slot="Manufacturer"
                        v-if="row.Product!=undefined">
                {{row.Product.Manufacturer}}
              </template>
              <!-- <template slot-scope="{ row, index }" slot="DateCode">{{row.DateCode}}</template> -->
              <template slot-scope="{ row, index }" slot="OriginName">
                {{row.OriginName}}
              </template>
              <template slot-scope="{ row, index }" slot="ShelveID">
                {{row.ShelveID}}
              </template>
              <template slot-scope="{ row, index }" slot="NoticeQuantity">
                {{row.Quantity}}
              </template>
              <template slot-scope="{ row, index }" slot="PickedQuantity">
                {{row.PickedQuantity}}
              </template>
              <template slot-scope="{ row, index }" slot="LeftQuantity">
                {{row.LeftQuantity}}
              </template>
              <!-- <template slot-scope="{ row, index }" slot="PickingQuantity">{{row.Pickings.Quantity}}</template> -->
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="(item,index) in row.Imagefiles" class="Filesbox">
                  <span class='linkurlcolor' @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <Icon v-if="item._disabled!=true"
                        type="ios-trash-outline"
                        :ref="row.ID"
                        @click.native="handleRemove(item,row,index)"></Icon>
                </p>
                <!-- <Input v-model="row.typeimg" /> -->
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <!-- <img-test v-bind:type="row" v-on:changitem="changeimgs($event,row)"></img-test> -->
                  <Button :disabled='row._disabled==true?true:false'
                          type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload(row)">
                    传照
                  </Button>
                </div>
                <div class="setupload">
                  <Button type="primary"
                          :disabled='row._disabled==true?true:false'
                          icon="md-reverse-camera" size="small"
                          @click="fromphotos(row)">
                    拍照
                  </Button>
                </div>
              </template>
            </Table>
            <div style="padding:10px;">
              <!-- <Page style="float:right" :total="details.total" :page-size='details.pageSize' @on-change='detailspage' /> -->
            </div>
            <div class="successbtn">
              <Button type="primary"
                      icon="md-checkmark"
                      @click="finish_btn"
                      :disabled="isdisabled==true?true:false">
                出库完成
              </Button>
              <!-- <Button type="error">到货异常</Button> -->
              <Button v-if='warehouseID.indexOf("HK")==-1'
                      type="warning"
                      icon="ios-alert-outline"
                      @click="Abnormalsettrue"
                      :disabled="isdisabled==true?true:false">
                出库异常
              </Button>
            </div>
          </div>
        </div>
      </div>
      <div v-if="WarehousingMsg==true">
        <Modal v-model="WarehousingMsg"
               title="确定出库"
               @on-ok="ok_Warehousing"
               @on-cancel="cancel_Warehousing">
          <div>
            <span>是否进行出库操作</span>
          </div>
        </Modal>
      </div>

      <!-- 异常到货 开始-->
      <Modal v-model="isAbnormal" title="出库异常" @on-visible-change="errorstatue">
        <div slot="close">
          <Icon type="md-close"
                color="rgb(33, 28, 28)"
                @click="closeerror"
                style="font-size:18px;" />
        </div>
        <span style="line-height: 1; display: block; padding-bottom: 10px;">
          <em class="Mustfill">*</em>出库异常原因
        </span>
        <Input v-model="Summary" type="textarea" :rows="2" placeholder="备注" />
        <div slot="footer">
          <Button @click="closeerror">取消</Button>
          <Button type="primary" @click="Abnormal_btn" :disabled="isdisabled==true?true:false">确定</Button>
        </div>
      </Modal>
      <!-- 异常到货 结束-->
      <!-- 收支明细 开始 -->
      <Modal v-model="Budgetdetail"
             width="55%"
             :closable="false"
             :mask-closable="false"
             :footer-hide="true">
        <div style="position: absolute;right:20px;z-index:99999;width:30px">
          <Icon type="ios-close"
                style="float:right;font-size:30px;font-weight:bold;"
                @click="closeBudget" />
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
    <!-- 一键打印弹出 开始 -->
    <Modal v-model="showprint"
           title="打印"
           @on-ok="CgAllprint"
           @on-cancel="cancel">
      <CheckboxGroup v-model="fruit">
        <Checkbox label="出库单打印"></Checkbox>
        <Checkbox label="送货单打印"></Checkbox>
        <Checkbox label="国际快递针式打印" v-if="waybillinfo.Type==4&&(waybillinfo.CarrierName=='UPS'||waybillinfo.CarrierName=='TNT')"></Checkbox>
        <Checkbox label="运单打印" v-if="waybillinfo.Type==1&&waybillinfo.Extype!=null&&waybillinfo.ExPayType!=null"></Checkbox>
      </CheckboxGroup>
    </Modal>
    <!-- 一键打印弹出 结束 -->
    <!-- 新增承运商 开始 -->
    <Modal v-model="showaddCarrier"
           title="新增承运商"
           :mask-closable="false">
      <div slot="close">
        <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(2)" />
      </div>
      <Add-Carrier ref="addCarrier" @fatherMethodCarrier="fatherMethodCarrier" @setdisabled='setdisabled' :Conveyingplace='Conveyingplace'></Add-Carrier>
      <div slot="footer">
        <Button @click="ok_addCarrier(2)" :disabled='adddisabled'>取消</Button>
        <Button @click="ok_addCarrier(1)" :disabled='adddisabled' type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 新增承运商 结束-->
    <!-- 新增司机 开始 -->
    <Modal v-model="showaddDriver"
           title="新增司机"
           :mask-closable="false">
      <div slot="close">
        <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(4)" />
      </div>
      <Add-Driver ref="AddDriver" :EnterpriseName='waybillinfo.CarrierName' @setdisabled='setdisabled' @ok_addDriver='ok_addDriver'></Add-Driver>
      <div slot="footer">
        <Button @click="ok_addCarrier(4)" :disabled='adddisabled'>取消</Button>
        <Button @click="ok_addCarrier(3)" :disabled='adddisabled' type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 新增司机 结束 -->
    <!-- 新增车牌号 开始 -->
    <Modal v-model="showaddCar"
           title="新增车牌号"
           :mask-closable="false">
      <div slot="close">
        <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(6)" />
      </div>
      <Add-Car ref="addcar" :EnterpriseName='waybillinfo.CarrierName' @setdisabled='setdisabled' @ok_addCar='ok_addCar'></Add-Car>
      <div slot="footer">
        <Button @click="ok_addCarrier(6)" :disabled='adddisabled'>取消</Button>
        <Button @click="ok_addCarrier(5)" :disabled='adddisabled' type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 新增车牌号 i结束-->
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
    <!-- 特殊标签显示 -->
    <Modal width='60'
           v-model="showneed"
           title="特殊要求">
      <Table :columns="needtitle" :data="needdata">
        <template slot-scope="{ row, index }" slot="files">
          <span v-if="row.File!=null" class='linkurlcolor Filesbox' @click="clackFilesProcess(row.File.Url)">{{row.File.CustomName}}</span>
        </template>
      </Table>
      <div slot="footer"></div>
    </Modal>
    <!-- 特殊标签显示 -->
    <!-- 支票信息展示 -->
    <!-- <Modal
     width='60'
      v-model="showneed"
      title="发票信息">
       <Table :columns="CheckRequirementstitle" :data="CheckRequirementsdata"></Table>
       <div slot="footer"></div>
  </Modal> -->
    <!-- 支票信息展示 -->
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
  import { mapActions, mapGetters } from 'vuex'
  import imgtest from '@/Pages/Common/imgtes'
  import Photograph from '@/Pages/Common/Photograph'
  import Historys from '@/Pages/Common/Historygoods'
  import AddCarrier from '../Subassembly/Add_Carrier'
  import AddDriver from '../Subassembly/Add_Driver'
  import AddCar from '../Subassembly/Add_Car'
  import logged from '../Common/logged'
  import Storagecharge from '@/Pages/Expenses/Storagecharge'
  import {
    getWayParter,
    initwaybill,
    SubCodeEnter,
    getchildencode,
    TestCheck,
    initwaybillenter,
  } from '../../api'
  import {
    CgPickingsDetail, CgpickingsoutBtn, CgPickingsSearch, CgpickingsErrorBtn, CgCarriers, GetDriversCars, CgReciptConfirm,
    CgDeleteFiles, Getclientdata, IsPayPayments, IsRecordWarehouseFee
  } from '../../api/CgApi'

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
    FilesProcess,
    PrintNationalWaybill
  } from '@/js/browser.js'
  // import {TemplatePrint,GetPrinterDictionary,FilePrint,FormPhoto} from "@/js/browser.js"
  let Base64 = require('js-base64').Base64
  let product_url = require('../../../static/pubilc.dev')
  let lodash = require("lodash");
  import Vue from 'vue'
  import moment from 'moment'
  import qs from 'qs'
  import $ from 'jquery'
  export default {
    name: 'RoutineEnter',
    components: {
      'img-test': imgtest,
      'photo-graph': Photograph,
      'Historys-dom': Historys,
      'Add-Carrier': AddCarrier,
      'Add-Driver': AddDriver,
      'Add-Car': AddCar,
      'logg-ed': logged,
      'Storagecharge': Storagecharge
    },
    props: {
      fatherMethod: {
        type: Function,
        default: null
      }
    },
    data() {
      return {
        showStoragecharge: false,//是否收取入仓费
        timedifference: null,//时差
        storagecharge: false,//仓储费录入
        CheckRequirements: false,//支票信息
        CheckRequirementstitle: [
          {
            type: 'index',
            width: 60,
            align: 'center'
          },
          {
            title: '送货时机',
            slot: 'timing',
            align: 'left'
          },
          {
            title: '支票处理',
            slot: 'action',
            align: 'left'
          },
          {
            title: '支票抬头',
            slot: 'Invoicetitle',
            align: 'left'
          },
          {
            title: '送票地址',
            slot: 'TypeName',
            align: 'left'
          },
          {
            title: '币种',
            slot: 'Currency',
            align: 'left'
          },
          {
            title: '价格',
            slot: 'Price',
            align: 'left'
          },
          {
            title: '创建日期',
            slot: 'CreateDate',
            align: 'left'
          },
        ],
        CheckRequirementsdata: [],
        needtitle: [
          {
            type: 'index',
            width: 60,
            align: 'center'
          },
          {
            title: '服务项目',
            key: 'TypeName',
            align: 'left'
          },
          {
            title: '服务要求',
            key: 'Name',
            align: 'left'
          },
          {
            title: '数量',
            key: 'Quantity',
            align: 'left'
          },
          {
            title: '具体要求',
            key: 'Requirement',
            align: 'left'
          },
          {
            title: '服务费(RMB)',
            key: 'TotalPrice',
            align: 'left'
          },
          {
            title: '文件',
            slot: 'files',
            align: 'left'
          },
        ],
        needdata: [],
        showneed: false,//显示特殊标签
        ispay: null,//是否收款
        DelivaryOpportunity: null,//送货时机
        loggdetime: '',//操作日志时间
        showlogged: false,//操作日志
        Conveyingplace: [],//原产地
        showaddDriver: false,// 是否可以新增司机
        addDriverName: null,//新增司机姓名
        showaddCar: false,//是否可以新增车牌号
        addCarName: null,//新增车牌号名称
        showaddCarrier: false,//是否可以新增承运商
        adddisabled: false,//添加按钮禁止点击

        IsPersonal: false,//是否有个人承运商
        clientGrade: null,//客户等级
        isdisabled: false,//禁止点击
        originalarr: [],//列表原始数据
        TypeArr: [
          {
            value: 3,
            label: "快递"
          },
          {
            value: 4,
            label: "国际快递"
          },
        ],
        IsfileDel: false,
        fruit: [],//打印选中
        showprint: false,
        fromroute: "",
        warehouseID: sessionStorage.getItem("UserWareHouse"), //当前库房ID
        pusharr: [], //出库提交的JSON
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
        loading: true, //loading效果
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
        Summary: '', //后台提供的备注信息对象
        historydata: false, //历史到货的抽屉
        historydetail: {
          //历史到货数据
          times: '', //时间，每次获取新的版本
          waybillLIst: []
        },
        company: '', //入仓号对应公司
        listtitles: [//详情列表标题
          {
            type: 'index',
            width: 60,
            align: 'center'
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
          // {
          //   title: '批次',
          //   slot: 'DateCode',
          //   align: 'left'
          // },
          {
            title: '产地',
            slot: 'OriginName',
            align: 'left'
          },
          {
            title: '库位',
            slot: 'ShelveID',
            align: 'left'
          },
          {
            title: '应出库数量',
            slot: 'NoticeQuantity',
            align: 'left'
          },
          {
            title: '已出库数量',
            slot: 'PickedQuantity',
            align: 'left'
          },
          {
            title: '剩余数量',
            slot: 'LeftQuantity',
            align: 'left'
          },
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
                h("span", {}, "本次发货数量")
              ]);
            },
            key: 'PickingQuantity',
            align: 'left',
            render: (h, params) => {
              var vm = this
              var dis = ''
              if (params.row.CurrentQuantity <= 0) {
                dis = true
              } else {
                dis = false
              }
              return h('Input', {
                props: {
                  //将单元格的值给Input
                  value: params.row.CurrentQuantity,
                  disabled: true,
                },
                on: {
                  'on-change'(event) { },
                  'on-blur'(event) {
                    var reg = /^[1-9]\d*$/
                    var newtarget = vm.trim(event.target.value)
                    if (newtarget != '') {
                      if (reg.test(newtarget) == true) {
                        if (newtarget > params.row.LeftQuantity) {
                          vm.$Message.error('发货数量不能大于剩余数量')
                          params.row.CurrentQuantity = ''
                          event.target.value = ''
                          vm.detailitem[params.index] = params.row
                        } else {
                          params.row.CurrentQuantity = newtarget
                          vm.detailitem[params.index] = params.row
                        }

                      } else {
                        vm.$Message.error('只能输入正整数')
                        params.row.CurrentQuantity = ''
                        event.target.value = ''
                        vm.detailitem[params.index] = params.row
                      }
                    } else {
                      params.row.CurrentQuantity = ''
                      event.target.value = ''
                      vm.detailitem[params.index] = params.row
                    }
                    vm.clicktest(params.row)
                  },
                }
              })
            }
          },
          {
            title: '毛重(kg)',
            key: 'Weight',
            align: 'center',
            render: (h, params) => {
              var vm = this
              var dis = ''
              if (params.row.CurrentQuantity <= 0) {
                dis = true
              } else {
                dis = false
              }
              return h('Input', {
                props: {
                  //将单元格的值给Input
                  value: params.row.Weight,
                  autofocus: true,
                  disabled: dis,
                },
                on: {
                  'on-change'(event) { },
                  'on-blur'(event) {
                    var reg = /^\d+(\.\d{0,2})?$/
                    var newtarget = vm.trim(event.target.value)
                    if (newtarget != '') {
                      if (reg.test(newtarget) == true) {
                        params.row.Weight = newtarget
                        vm.detailitem[params.index] = params.row
                      } else {
                        vm.$Message.error('只能输入数字,包括两位数的小数点')
                        params.row.Weight = null
                        event.target.value = null
                        vm.detailitem[params.index] = params.row
                      }
                    } else {
                      params.row.Weight = null
                      event.target.value = null
                      vm.detailitem[params.index] = params.row
                    }
                    vm.changeoriginalarr(params.row)
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
                            Quantity: params.row.CurrentQuantity, //数量
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
        wayBillID: this.$route.params.wayBillID, //详情页ID
        waybillinfo: '',
        CarArr: [], //车牌号
        CarArrName: '',//车牌号名称
        DriversArr: [], // 司机
        DriversName: '',//司机名称
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
        ]
      }
    },
    filters: {
      showDate: function (val) {
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
      changenumber: function (val) {
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
      },
      // searchkey(){
      //   this.rowClassName()
      // }
    },
    beforeRouteEnter(to, from, next) {
      console.log(from)
      next(vm => {
        vm.fromroute = from
      })
    },
    mounted() {
      // this.$Spin.show()
      this.WayParterdata()
      this.getdetail_data()
      let _this = this
      var strings = ''
      document.onkeydown = function (e) {
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
    },
    created() {
      // console.log("重新加载-------常规分拣");
      window['KdnPrinted'] = this.changed
      window['PhotoUploaded'] = this.changimgs
      console.log(this.$route.params)
    },
    methods: {
      IsPayPayments(id) {
        IsPayPayments(id, 1).then(res => {
          console.log(res)
          if (res == 1) {
            this.ispay = "已收款"
          } else if (res == 2) {
            this.ispay = "未收款"
          } else {
            this.ispay = null
          }
        })
      },
      rowClassName(row, index) {
        if (index == 1) {
          console.log(index)
          return 'demo-table-info-row';
        } else if (index == 2) {
          return 'demo-table-error-row';
        }
        return '';
      },
      clicktest: lodash.throttle(function (paramsrow) {  //同步修改选中的数据
        for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
          if (paramsrow.ID == this.SelectRow[i].ID) {
            this.SelectRow[i] = paramsrow
          }
        }
      }, 1000),
      //修改原始数据
      changeoriginalarr(row) {
        for (var i = 0, lens = this.originalarr.length; i < lens; i++) {
          if (this.originalarr[i].ID == row.ID) {
            this.originalarr[i] = row
          }
        }
      },
      changestatus(ID) {  //出库复核
        for (var i = 0; i < this.detailitem.length; i++) {
          if (this.detailitem[i].ID == ID) {
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
        // this.$Message.info('Clicked ok')
        this.showphoto2 = false
      },
      cancel() {
        // this.$Message.info('Clicked cancel')
        this.showphoto2 = false
        this.showprint = false;
        this.showaddCarrier = false
        this.showaddDriver = false
        this.showaddCar = false
      },
      handleSelectRow(value) {
        //多选事件 获取选中的数据
        this.SelectRow = value
      },
      search_pro() {
        this.loading = true
        if (this.searchkey != '') {
          CgPickingsSearch(this.waybillinfo.ID, this.searchkey).then(res => {
            // this.detailitem=res;
            if (res.length > 0) {
              var historydata = this.detailitem
              var newdata = []
              var result = new Array();
              var sameCount = 0;
              for (var i = 0; i < historydata.length; i++) {
                var tempA = historydata[i].ID;
                for (var j = 0; j < res.length; j++) {
                  var tempB = res[j].ID;
                  if (tempA == tempB) {
                    newdata.push(historydata[i])
                  }
                }
              }
              this.detailitem = newdata
            } else {
              this.detailitem = []
            }
          })
        } else {
          this.detailitem = this.originalarr
        }
        this.loading = false;
      },
      finish_btn() {
        if (this.DelivaryOpportunity == "先款后货" && this.ispay == "未收款") {   //判断是否是先款后货，并且是已收款则可以出库
          this.$Message.error('该订单为先款后货,且收款状态为未收款，需财务收款后再进行出库')
        } else {
          console.log(this.originalarr)
          this.pusharr = []
          var isshow = false;
          for (var i = 0, lens = this.originalarr.length; i < lens; i++) {
            if (this.originalarr[i].CurrentQuantity == '' || this.originalarr[i].CurrentQuantity == '0') {
              this.$Message.error('请填写本次发货数量')
              this.pusharr = [];
              isshow = false
              break;
            } else {
              if (this.originalarr[i].CurrentQuantity > this.originalarr[i].LeftQuantity) {
                this.$Message.error('发货数量不能大于剩余数量')
                this.pusharr = [];
                isshow = false
                break;
              } else {
                var obj = {
                  NoticeID: this.originalarr[i].ID,
                  OutputID: this.originalarr[i].OutputID,
                  BoxCode: this.originalarr[i].ShelveID,
                  Quantity: this.originalarr[i].CurrentQuantity,
                  Weight: this.originalarr[i].Weight,
                  NetWeight: this.originalarr[i].NetWeight,
                  Volume: this.originalarr[i].Pickings.Volume,
                  Files: this.originalarr[i].Imagefiles
                }
                this.pusharr.push(obj)
                isshow = true
              }
            }
          }
          if (isshow == true) {
            if (this.waybillinfo.Type == 2) {//如果是送货上门，需要填写承运商，司机，车牌号
              if (this.waybillinfo.CarrierID == null || this.waybillinfo.CarrierID == '') {
                this.$Message.error('请先选择承运商')
              } else if (this.waybillinfo.Driver == null || this.waybillinfo.Driver == '') {
                this.$Message.error('请选择司机')
              } else if (this.waybillinfo.CarNumberID == null || this.waybillinfo.CarNumberID == '') {
                this.$Message.error('请选择车牌号')
              } else {
                this.WarehousingMsg = true
              }
            } else {
              this.WarehousingMsg = true
            }
          }
        }
      },

      setWarehousing(data) {
        //确定出库，调取后台出库接口
        this.isdisabled = true;
        CgpickingsoutBtn(data).then(res => {
          console.log(res)
          if (res.Success == true) {
            this.$Message.success('出库完成，一秒后自动关闭')
            var _this = this
            setTimeout(function () {
              if (_this.fromroute.name == 'CgUntreated') {
                _this.$store.dispatch('setshowtype', 0)
                _this.$store.dispatch('setshowdetailout', false)
                _this.$router.push({ path: "/Outgoing/10" })
              } else {
                _this.$store.dispatch('setshowtype', 0)
                _this.$store.dispatch('setshowdetailout', false)
                _this.$router.go(-1)
              }
            }, 1000)

          } else {
            this.$Message.error(res.Data)
            this.isdisabled = false;
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
      ok_Warehousing() {  //点击确定按钮，进行出库操作
        this.WarehousingMsg = false
        this.isdisabled = true;
        var uploaddata = {
          Waybill: {
            ID: this.waybillinfo.ID,
            ExcuteStatus: this.waybillinfo.ExcuteStatus,
            Type: this.waybillinfo.Type, //库房发货方式
            OrderID: this.waybillinfo.OrderID, //所属订单
            CarrierID: this.waybillinfo.CarrierID, //送货是库房定义,快递是客户定义
            Summary: this.waybillinfo.Summary,
            Driver: this.waybillinfo.Driver, //报关是申报人员定的不可修改,送货是库房定义的
            CarNumber1: this.waybillinfo.CarNumberID,
            Files: this.waybillinfo.FeliverGoodFile,
            LoadingExcuteStatus: this.waybillinfo.LoadingExcuteStatus, //请参考提送货枚举 d:\projects_vs2015\yahv\solutions\yahv.underly\enums\enum.cgexcutestatus.cs
            WarehouseID: this.warehouseID,//当前库房ID
            Code: this.waybillinfo.Code
          },
          AdminID: sessionStorage.getItem('userID'), //当前的操作人
          Pickings: this.pusharr //用inputID 关联 Noitice
        }
        console.log(uploaddata)
        this.setWarehousing(uploaddata)
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

      getdetail_data() {  //获取详情页数据
        CgPickingsDetail(this.wayBillID).then(res => {
          console.log(res)
          this.Getclientdata(res.Waybill.EnterCode)
          this.originalarr = res.Notices
          this.waybillinfo = res.Waybill
          this.detailitem = res.Notices
          this.detailitem.forEach(item => {
            if (item.LeftQuantity <= 0) {
              item._disabled = true;
            } else {
              item._disabled = false;
            }
          })
          this.$Spin.hide()
          this.Carriers(res.Waybill.Type)
          this.GetDriversCars(res.Waybill.CarrierID)
          this.DelivaryOpportunity = res.Waybill.DelivaryOpportunity
          this.IsPayPayments(res.Waybill.OrderID)
          if (res.Waybill.OrderRequirements == null) {
            this.needdata = []
          } else {
            this.needdata = res.Waybill.OrderRequirements
          }
          if (res.Waybill.Type == 2 && res.Waybill.CarrierID == 'Personal') {
            this.IsPersonal = true
          } else {
            this.IsPersonal = true
            this.GetDriversCars(res.Waybill.CarrierID)
          }
          this.CarArrName = res.Waybill.CarNumber1
          this.DriversName = res.Waybill.Driver
          if (this.warehouseID.indexOf('SZ') == -1) {  //如果不是深圳库房，则没有运单打印
            if (res.Waybill.Extype != null && res.Waybill.ExPayType != null) {  //处理打印的类型
              this.fruit = ['出库单打印', '送货单打印', '运单打印']//打印选中
            } else {
              this.fruit = ['出库单打印', '送货单打印']//打印选中
            }
          } else {
            this.fruit = ['出库单打印', '送货单打印', '运单打印']//打印选中
          }

          var newdata = null //最近处理订单的时间，用与计算仓储天数
          if (res.Waybill.ExcuteStatus == 215) {  //如果是已处理订单，则时间为最后一次操作时间
            this.isdisabled = true;
            newdata = this.moment(res.Waybill.ModifyDate).format('YYYY-MM-DD')
          } else {  // 如果是未处理，则为系统当前时间
            this.isdisabled = false; 
            newdata = this.moment(new Date().getTime()).format('YYYY-MM-DD')
          }
          this.loading = false;
      
          //如果是未租赁，并且天数大于五天，则判断是否收取过入仓费，如未收取，则需要录入入仓费
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

          //var olddata = this.moment(res.Waybill.FirstTempDate).format('YYYY-MM-DD')
          //var startDate = Date.parse(olddata);
          //var endDate = Date.parse(newdata);
          //var days = ((endDate - startDate) / (1 * 24 * 60 * 60 * 1000)) + 1;
          //this.timedifference = days
          //console.log(this.timedifference)
          //if (res.Waybill.IsClientLs == false && days > 5) {
          //  this.showStoragecharge = true  //只要暂存天数超过五天，无论是已收还是未收都可以录入
          //  this.IsRecordWarehouseFee(res.Waybill.OrderID)
          //}

        })
      },
      //更改到货方式
      changewaybillType(value) {
        this.Carriers(value)
      },
      detailspage(value) {
        this.details.pageIndex = value
        this.getdetail_data(this.details.wayBillID)
      },
      Removebackfun(file, type) {
        if (type == "Clientimg") {
          this.waybillinfo.SendGoodsFile.splice(this.waybillinfo.SendGoodsFile.indexOf(file), 1)
        } else if (type == "Waybill") {
          this.waybillinfo.FeliverGoodFile.splice(this.waybillinfo.FeliverGoodFile.indexOf(file), 1)
        } else {
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
      //运单信息文件删除
      handleRemove(file, type, index) {
        var data = {
          id: file.ID
        }
        this.ok_CgDeleteFiles(data, file, type)
      },

      //确认删除上传的文件
      ok_CgDeleteFiles(data, file, type) {
        CgDeleteFiles(data).then(res => {
          console.log(res)
          if (res.Success == true) {
            this.IsfileDel = true
            this.$Message.success('删除成功')
            this.Removebackfun(file, type)
          } else {
            this.IsfileDel = false
            this.$Message.error('删除失败')
          }
        })
      },
      // 选中某条产品，点击异常到货
      Abnormalsettrue() {
        this.isAbnormal = true
      },
      //到货异常 确认按钮
      Abnormal_btn() {
        if (this.Summary == undefined || this.Summary == '') {
          this.isAbnormal = true
          this.$Message.error('请输入异常原因')
        } else {
          this.isdisabled = true;
          var data = {
            waybillid: this.waybillinfo.ID,
            adminid: sessionStorage.getItem('userID'),
            orderid: this.waybillinfo.OrderID,
            summary: this.Summary
          }
          CgpickingsErrorBtn(data).then(res => {
            if (res.Success == true) {
              this.$Message.success('出库完成，一秒后自动关闭')
              var _this = this
              setTimeout(function () {
                if (_this.fromroute.name == 'CgUntreated') {
                  _this.$store.dispatch('setshowtype', 0)
                  _this.$store.dispatch('setshowdetailout', false)
                  _this.$router.push({ path: "/Outgoing/10" })
                } else {
                  _this.$store.dispatch('setshowtype', 0)
                  _this.$store.dispatch('setshowdetailout', false)
                  _this.$router.go(-1)
                }
              }, 1000)

            } else {
              this.$Message.error('出库操作失败,请从新操作')
              this.isdisabled = false;
            }
          })
          this.isAbnormal = false
        }
      },
      closeerror() {
        //异常到货关闭
        this.isAbnormal = false
        this.Summary = '' //备注
        // this.Reason = '外观损坏' //异常原因
      },
      errorstatue(value) {
        if (value == true) {
          this.isAbnormal = true
        } else {
          this.isAbnormal = false
          this.Summary = ''
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

      showBudget(type, Budget, Source) {
        //收支明细展开
        this.$store.dispatch("setBudget", true);
        if (type == "meet") {
          var namemeet = "";
          this.$router.push({
            name: 'outdetailBudget',
            query: {
              webillID: this.waybillinfo.ID,
              OrderID: this.waybillinfo.OrderID,
              type: Budget,
              otype: "out",
              conduct: Source
            }
          });
        } else {
          var namemeet = "";
          if (this.routername == "CgUntreated") {
            namemeet = "/CgUntreated/Oplog";
          } else {
            namemeet = "/CgProcessed/Oplog";
          }
          this.$router.push({
            path: namemeet,
            params: {
              webillID: this.details.waybillinfo.WaybillID
            }
          });
        }
      },
      closeBudget() {
        //关闭收支明细
        console.log('关闭收支明细')
        this.$router.go(-1)
        // this.Budgetdetail=false;
        this.$store.dispatch('setBudget', false)
      },
      Carriers(type) {  //重构承运商
        CgCarriers(type, this.warehouseID, 200).then(res => {
          this.CarrierList = res
        })
      },
      GetDriversCars(key) { //根据送货上门承运商获取司机与车牌号
        GetDriversCars(key).then(res => {
          this.CarArr = []
          var data = res.Transports
          if (data.length > 0) {
            var item = {}
            for (var i = 0; i < data.length; i++) {
              if (data[i].CarNumber1 != null && data[i].CarNumber2 != null) {
                item = {
                  value: data[i].CarNumber1 + '\xa0\xa0' + data[i].CarNumber2
                }
              } else {
                item = {
                  value: data[i].CarNumber1
                }
              }
              this.CarArr.push(item)
            }
          } else {
            this.CarArr = []
          }
          // this.CarArr = res.Transports; //车牌号
          this.DriversArr = res.Drivers; // 司机
          this.waybillinfon.Driver = null;
          this.waybillinfon.CarNumber1 = null;
        })
      },
      fromphotos(type) {
        if (type == "Waybill" || type == "Clientimg") {
          var Type = null
          if (type == "Waybill") {
            Type = 8010
          } else {
            Type = 8020
          }
          var data = {
            SessionID: type,
            AdminID: sessionStorage.getItem("userID"),
            Data: {
              WayBillID: this.waybillinfo.ID,
              WsOrderID: this.waybillinfo.OrderID,
              NoticeID: '',
              InputID: '',
              Type: Type
            }
          };
          FormPhoto(data);
        } else {
          var data = {
            SessionID: type.ID,
            AdminID: sessionStorage.getItem("userID"),
            Data: {
              WayBillID: this.waybillinfo.ID,
              WsOrderID: this.waybillinfo.OrderID,
              NoticeID: type.ID,
              InputID: '',
              Type: 8010,
            }
          };
          FormPhoto(data);
        }
      },
      changed(message) {
        //调用打印运单号，返回运单号的值
        this.waybillinfo.Code = message
        this.waybuillform.Code = message
        this.initwaybillenter(this.modelstype)
      },
      changimgs(message) {
        this.testfunction(message)
      },

      //前台处理数据的方法图片文件
      testfunction(message) {
        var imgdata = message[0];
        var newfile = {
          CustomName: imgdata.FileName,
          ID: imgdata.FileID,
          Url: imgdata.Url,
          Type: null
        }
        if (imgdata.SessionID == 'Waybill') {
          newfile.Type = 8010
          this.waybillinfo.FeliverGoodFile.push(newfile)
        } else if (imgdata.SessionID == 'Clientimg') {
          newfile.Type = 8020
          var adminid = sessionStorage.getItem("userID")
          var webillID = this.waybillinfo.ID
          CgReciptConfirm(adminid, webillID).then(res => {
            if (res.Success == true) {
              this.waybillinfo.SendGoodsFile.push(newfile)
            }
          })
        }
        else {
          newfile.Type = 8010
          for (var i = 0; i < this.detailitem.length; i++) {
            if (this.detailitem[i].ID == imgdata.SessionID) {
              this.detailitem[i].Imagefiles.push(newfile)
              this.changeoriginalarr(this.detailitem[i])
            }
          }
        }
        // alert(JSON.stringify(newfile))
      },
      SeletUpload(type) { // 传照
        if (type == "Waybill" || type == "Clientimg") {
          var Type = null
          if (type == "Waybill") {
            Type = 8010
          } else {
            Type = 8020
          }
          var data = {
            SessionID: type,
            AdminID: sessionStorage.getItem("userID"),
            Data: {
              WayBillID: this.waybillinfo.ID,
              WsOrderID: this.waybillinfo.OrderID,
              NoticeID: '',
              InputID: '',
              Type: Type
            }
          };
          SeletUploadFile(data);
        } else {
          var data = {
            SessionID: type.ID,
            AdminID: sessionStorage.getItem("userID"),
            Data: {
              WayBillID: this.waybillinfo.ID,
              WsOrderID: this.waybillinfo.OrderID,
              NoticeID: type.ID,
              InputID: '',
              Type: 8010
            }
          };
          SeletUploadFile(data);
        }
      },
      //选择打印类型
      printdata(value) {
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
      //运输类型切换
      changwaytype(value) {
        this.modelstype = value
        this.GetExpTypes(this.modelstype)
      },
      //运单打印功能
      sumbit_print() {
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
      //出库单打印功能
      outorder_print() {
        var data = {
          waybillinfo: this.waybillinfo,
          listdata: this.detailitem
        }
        PrintOuptNotice(data)
      },
      //装箱单打印 拣货单 送货单
      Boxing_print() {
        var Numcopies = null
        if (this.warehouseID.indexOf('SZ') != -1) { //深圳库房
          if (this.waybillinfo.Type == 4 || this.waybillinfo.Type == 3) {
            this.language = 'SC'
            Numcopies = 2
          } else {
            this.language = 'SC'
            Numcopies = 2
          }
        } else { //香港库房
          if (this.waybillinfo.Type == 4) {//国际
            this.language = 'EN'
            Numcopies = 2
          } else if (this.waybillinfo.Type == 3) {//本港
            this.language = 'TC'
            Numcopies = 2
          } else {
            this.language = 'TC'
            Numcopies = 4
          }
        }
        var data = {
          warehouseID: this.warehouseID,
          Language: this.language,
          waybillinfo: this.waybillinfo,
          listdata: this.detailitem,
          Numcopies: Numcopies
        }
        PrintDeliveryList(data)
      },

      waybillprint() {
        var printdata = {
          ShipperCode: this.waybillinfo.CarrierName,//承运商名称, //this.modelstype, //快递公司
          ExPayType: this.waybillinfo.ExPayType, //this.IGetExpType, //货运类型
          ExpType: this.waybillinfo.Extype,
          Sender: {
            //发件人
            Company: this.waybillinfo.Sender.Company,
            Name: this.waybillinfo.Sender.Name,
            Mobile: this.waybillinfo.Sender.Mobile,
            Address: "广东省" + this.waybillinfo.Sender.Address//this.waybillinfo.Sender.Address //暂时调用收货人地址
          },
          Receiver: {
            //收件人
            Company: this.waybillinfo.Receiver.Company,
            Name: this.waybillinfo.Receiver.Name,
            Mobile: this.waybillinfo.Receiver.Mobile,
            Address: this.waybillinfo.Receiver.Address
          },
          Quantity: this.waybillinfo.TotalParts, //数量
          Remark: '', //备注
          volume: this.waybillinfo.TotalWeight, //重量
          Weight: this.waybillinfo.TotalWeight, //体积
          Commodity: [{ GoodsName: '客户器件' }] //
        }
        if (this.waybillinfo.Code != null && this.waybillinfo.Code != '') {
          ReprintKdnFaceSheet(this.waybillinfo.Code)
        } else {
          PrintKdn(printdata)
          PrintNationalWaybill()  //国际快递打印测试
        }
      },
      outorder_clear() {
        this.outprint = false
      },
      //一键打印重构
      CgAllprint() {
        console.log(this.fruit)
        if (this.fruit.indexOf('运单打印') > -1) {  //如果运单打印存在 调用打印运单的方法
          this.waybillprint()
        }
        if (this.fruit.indexOf('国际快递针式打印') > -1) {
          this.Printinternational()//国际快递打印
        }
        if (this.fruit.indexOf('出库单打印') != -1) {
          this.outorder_print()
        }
        if (this.fruit.indexOf('送货单打印') != -1) {
          this.Boxing_print()

        }
      },

      //文件打印
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
      changeacrrier(value) {
        if (this.waybillinfo.Type == 2 && value.value == 'Personal') {
          this.IsPersonal = true
          this.waybillinfo.CarNumberID = this.CarArrName;
          this.waybillinfo.Driver = this.DriversName;
        } else {
          if (this.waybillinfo.Type == 2) {
            this.IsPersonal = false
            this.waybillinfo.CarNumberID = '';
            this.waybillinfo.Driver = '';
            this.GetDriversCars(value.value);
          } else {
            this.IsPersonal = false
            this.waybillinfo.CarNumberID = '';
            this.waybillinfo.Driver = '';
          }
          this.waybillinfo.CarrierName = value.label
        }
        // this.GetDriversCars(value.value)
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
      closewaybull_btn() {
        this.waybillprintbox = false
      },
      //查看照片货或文件
      clackFilesProcess(url) {
        var data = {
          Url: url
        }
        console.log(data)
        FilesProcess(data)
      },
      // 获取客户等级
      Getclientdata(entdata) {
        Getclientdata(entdata).then(res => {
          this.clientGrade = res.obj.Grade
        })
      },
      //保存承运商,司机，车牌号
      ok_addCarrier(type) {
        if (type == 1) {
          this.$refs.addCarrier.sumbit_btn()
        } else if (type == 2) {
          this.$refs.addCarrier.delitem()
          this.showaddCarrier = false
        } else if (type == 3) {
          this.$refs.AddDriver.sumbit_btn()
        } else if (type == 4) {
          this.$refs.AddDriver.delitem()
          this.showaddDriver = false
        } else if (type == 5) {
          this.$refs.addcar.sumbit_btn()
        } else if (type == 6) {
          this.$refs.addcar.delitem()
          this.showaddCar = false
        }

      },
      setdisabled(type) {
        this.adddisabled = type
      },
      fatherMethodCarrier(res) {
        this.adddisabled = false
        if (res != 'false') {
          var housid = sessionStorage.getItem("UserWareHouse")
          var carrie = { ID: res.Carrier.ID, Name: res.Carrier.Name }
          this.CarrierList.push(carrie)
          this.Carriers(this.waybillinfo.Type)
          this.waybillinfo.CarrierID = res.Carrier.ID
          this.waybillinfo.CarrierName = res.Carrier.Name

          var data = { Name: res.Driverinfo.Name, ID: "", }
          this.DriversArr.push(data)
          this.waybillinfo.Driver = res.Driverinfo.Name

          var data = { value: res.Carinfo.CarNumber1, }
          this.CarArr.push(data)
          this.waybillinfo.CarNumberID = res.Carinfo.CarNumber1

          this.GetDriversCars(this.waybillinfo.CarrierID);
          this.showaddCarrier = false;
          this.$refs.addCarrier.delitem()
        }
      },
      // 保存司机 DriverAdd,
      ok_addDriver(res) {
        this.adddisabled = false
        if (res != 'false') {
          var data = {
            Name: res,
            ID: "",
          }
          this.DriversArr.push(data)
          this.GetDriversCars(this.waybillinfo.CarrierID);
          this.waybillinfo.Driver = res
          this.showaddDriver = false;
          this.$refs.AddDriver.delitem()
        }
      },
      // 保存车牌号 TransportAdd
      ok_addCar(res) {
        this.adddisabled = false
        if (res != 'false') {
          var data = {
            value: res,
          }
          this.CarArr.push(data)
          this.GetDriversCars(this.waybillinfo.CarrierID);
          this.waybillinfo.CarNumberID = res
          this.showaddCar = false
          this.$refs.AddDriver.delitem()
        }
      },
      //操作日志的展示
      logchange() {
        this.showlogged = true
        this.loggdetime = new Date().getTime()
      },
      //国际快递打印
      Printinternational() {
        var printdata = {
          "expType": this.waybillinfo.CarrierName,//快递类型
          "ReceiverAccountNo": null,//收件人的账号
          "ReceiverCustomNo": null,//海关收件人身份证号
          "TelOfContactPerson": this.waybillinfo.Receiver.Mobile,//联系人电话
          "NameOfContactPerson": this.waybillinfo.Receiver.Name,//联系人姓名
          "ReceiverCompany": this.waybillinfo.coeCompany,//收件人公司
          "ReceiverAddress": this.waybillinfo.Receiver.Address,//收件人地址
          "ReceiverPostalCode": null,//收件人邮编
          "ReceiverCountry": null,//收件人国家
        }
        PrintNationalWaybill(printdata)  //国际快递打印测试

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
            //this.showStoragecharge = false //已收取
          } else {
            //this.showStoragecharge = true //未收取
            this.isdisabled = true;
          }
        })
      },
      Parentfun() {
        //this.showStoragecharge = false
        if (this.waybillinfo.ExcuteStatus == 215) {
          this.isdisabled = true;
        } else {
          this.isdisabled = false;
        }
      }
    }
  }
</script>
