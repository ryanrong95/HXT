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
    width: 75px;
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

  .transferclass {
    width: 150px;
    height: 200px;
    overflow-x: hidden;
    overflow-y: scroll;
  }

  .Mustfill {
    color: red;
  }

  .linkurlcolor {
    color: #2d8cf0;
  }

  .Filesbox:hover {
    cursor: pointer;
  }

  .sync_btn {
    font-size: 30px;
    vertical-align: middle;
    padding-right: 10px;
  }

  .hoverbtn:hover {
    cursor: pointer;
  }
</style>
<template>
  <div class="Declare">
    <div style="width:100%;">
      <!-- 代报关基础信息 -->
      <div class="itembox">
        <p class="detailtitle">代报关基础信息</p>
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row>
            <Col style="width: 20%;float: left;">
            <ul class="detail_li detail1">
              <li class="itemli">
                <span class="detail_title1">ID：</span>
                <span>{{waybillinfo.EntryNoticeID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状态：</span>
                <span>
                  {{waybillinfo.EntryNoticeStatusName}}
                  <Button v-if="waybillinfo.EntryNoticeID!=null" icon='md-calendar' type="success" size='small' @click="logchange">日志管理</Button>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2">
                <span class="detail_title1">提货状态：</span>
                <Tag color="magenta" v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==1">等待提货</Tag>
                <Tag color="blue" v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==3">提货中</Tag>
                <Tag color="green" v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==2">提货完成</Tag>
              </li>
              <li class="itemli">
                <span class="detail_title1">业务类型：</span>
                <span>代报关</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">客服人员：</span>
                <span>{{waybillinfo.Merchandiser?waybillinfo.Merchandiser.RealName:''}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1" style="width:112px">是否收取入仓费：</span>
                <span>
                  <Tag color="red" v-if="waybillinfo.ChargeWH==2"><span v-if="waybillinfo.ChargeWH==2">不收取</span></Tag>
                  <em v-else><span v-if="waybillinfo.ChargeWH==1">收取</span></em>
                </span>
                
              </li>
               <li class="itemli" v-if="waybillinfo.ChargeWH==1">
                <span class="detail_title1">收取方式：</span>
                <span>{{waybillinfo.ChargeType}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.ChargeWH==1">
                <span class="detail_title1">收取金额：</span>
                <span>{{waybillinfo.AmountWH}}</span>
              </li>
              
              <!-- <li class="itemli" v-if="waybillinfo.Condition!=null">
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
              </li> -->
            </ul>
            </Col>
            <Col style="width: 26%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title2">制单时间：</span>
                <span>{{waybillinfo.OrderCreateDate|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">供应商：</span>
                <span style="display: inline-block;width: 75%;line-height: 20px;">
                  {{waybillinfo.ClientSupplierName}}
                  <Tag color="volcano">LV {{waybillinfo.ClientSupplierGrade}}</Tag>
                </span>
                <!-- <Tooltip content="一级" placement="right">
                  <img style="vertical-align: middle" src="../../assets/img/vip_1.png" alt />
                </Tooltip>-->
              </li>
              <li class="itemli">
                <span class="detail_title2">到货方式：</span>
                <span v-if="waybillinfo.HKDeliveryType==2">{{waybillinfo.HKDeliveryTypeName}}</span>
                <Select v-else
                        v-model="waybillinfo.HKDeliveryType"
                        style="width:40%"
                        @on-change='changewaybillType'>
                  <Option v-for="(item,index) in TypeArr"
                          :value="item.value"
                          :key="index">
                    {{ item.label}}
                  </Option>
                </Select>
                <!-- <a href="javascript:void(0)" @click="showhistory">历史到货</a> -->
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2">
                <span class="detail_title2">自提时间：</span>
                <span>{{waybillinfo.PickUpTime}}</span>
              </li>
              <!-- <li class="itemli" v-if="waybillinfo.HKDeliveryType==1">
                <span class="detail_title2">到货时间：</span>
                <span>{{waybillinfo.AppointTime|showDateexact}}</span>
              </li>-->
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2">
                <span class="detail_title2">提货文件：</span>
                <span style="display: inline-block;">
                  <p v-for="item in waybillinfo.PickupFiles" :key="item.FileName">
                    <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.FileUrl)">{{item.FileName}}</span>
                    <a @click="fileprint(item.FileUrl)">打印</a>
                  </p>
                </span>
              </li>
            </ul>
            </Col>
            <Col style="width: 31%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title3"><em>&nbsp;&nbsp;</em>入&nbsp;&nbsp;仓&nbsp;&nbsp;号：</span>
                <span>
                  {{waybillinfo.ClientCode}}({{waybillinfo.ClientName}})
                  <Tag color="geekblue">LV {{waybillinfo.ClientRank}}</Tag>
                  <!-- <i style=" background: #f90; color: #ffffff; padding: 6px; border-radius: 50%; font-size: 17px;" v-if="waybillinfo.IsClientLs==true">租</i> -->
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType!=2">
                <span class="detail_title3">运单号(本次)：</span>
                <span>
                  <Input style="width:60%" v-model="waybillinfo.WayBillCode" />
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title3">
                  <em v-if="waybillinfo.HKDeliveryType==2" class="Mustfill">*</em>
                  <i v-if="waybillinfo.HKDeliveryType==3">快递公司(本次):</i>
                  <i v-else>承运商(本次):</i>
                </span>
                <span>
                  <Select v-model="waybillinfo.CarrierID"
                          :label-in-value="true"
                          filterable
                          @on-change="changeacrrier"
                          style="width:60%"
                          :disabled='waybillinfo.EntryNoticeStatus==3?true:false'>
                    <Option v-for="item in CarrierList" :value="item.ID" :key="item.ID">{{item.Name}}</Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCarrier=true" v-if="waybillinfo.EntryNoticeStatus!=3&&waybillinfo.HKDeliveryType==2&&waybillinfo.CarrierID!='Personal'" />
                  <Icon type="md-add" @click="showaddcontacts=true" v-if="waybillinfo.EntryNoticeStatus!=3&&waybillinfo.HKDeliveryType==3&&waybillinfo.CarrierID!='Personal'" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2&&IsPersonal==false">
                <span class="detail_title3"><em v-if="waybillinfo.HKDeliveryType==2" class="Mustfill">*</em>&nbsp;司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Select transfer
                          v-model="waybillinfo.DriverID"
                          style="width:60%"
                          :label-in-value="true"
                          filterable
                          :disabled='waybillinfo.EntryNoticeStatus==3?true:false'>
                    <Option v-for="(item,index) in DriversArr" :value="item.Name" :key="index">{{item.Name}}</Option>
                  </Select>
                </span>
                <Icon type="md-add" @click="showaddDriver=true" v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.CarrierID!=null" />
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2&&IsPersonal==false">
                <span class="detail_title3"><em v-if="waybillinfo.HKDeliveryType==2" class="Mustfill">*</em>&nbsp;车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Select transfer
                          v-model="waybillinfo.CarNumber"
                          style="width:60%"
                          :label-in-value="true"
                          @on-create="handleCreate3"
                          :disabled='waybillinfo.EntryNoticeStatus==3?true:false'>
                    <Option v-for="item in CarArr"
                            :value="item.value"
                            :key="item.value">
                      {{ item.value }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCar=true" v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.CarrierID!=null" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Input v-model="waybillinfo.DriverID" style="width:60%" placeholder="请输入司机" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Input v-model="waybillinfo.CarNumber" style="width:60%" placeholder="请输入车牌号" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2">
                <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>联&nbsp;&nbsp;系&nbsp;&nbsp;人:</span>
                <span> {{waybillinfo.PickUpContactName}} &nbsp;&nbsp;(电话:{{waybillinfo.PickUpContactPhone}})</span>
              </li>
              <li class="itemli" v-if="waybillinfo.HKDeliveryType==2">
                <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>提货地址:</span>
                <span>{{waybillinfo.PickUpAddress}}</span>
              </li>
               
              <!-- <li class="itemli">
                <span class="detail_title3"><em>&nbsp;&nbsp;</em>输&nbsp;&nbsp;送&nbsp;&nbsp;地：</span>
                <span>
                  <span v-if="waybillinfo.ConsignorPlaceText!=''">{{waybillinfo.ConsignorPlaceText}}</span>
                  <span v-else style="color:red;">暂无输送地</span>
                  <Icon class="sethover"
                        @click="clickClient(waybillinfo.ConsignorPlace,'ClientCode','ClientCode')"
                        type="md-create" />
                </span>
              </li> -->
            </ul>
            </Col>
            <Col style="width:23%;float: left;">
            <ul class="detail_li" style="margin-left:20px;">
              <li class="itemli">
                <span class>订单号：</span>
                <span>{{waybillinfo.OrderID}}</span>
              </li>
              <li class="itemli">
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SaveHKDeliveryInfo()">
                    保存香港交货信息
                  </Button>
                  <Button type="primary"
                          size="small"
                          v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==1"
                          @click="TakeGoods(3)">
                    {{TakeGoodsName}}
                  </Button>
                  <Button type="primary"
                          size="small"
                          v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==3"
                          @click="TakeGoods(2)">
                    提货完成
                  </Button>
                </div>
                <div class="setupload">
                  <Button :disabled='Operateddisable==false?true:false'
                          type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload('Waybill')">
                    传照
                  </Button>
                  <Button :disabled='Operateddisable==false?true:false'
                          size="small"
                          type="primary"
                          icon="md-reverse-camera"
                          @click="fromphotos('Waybill')">
                    拍照
                  </Button>
                </div>
              </li>
              <li style="clear: both;">
                <div v-for="item in waybillinfo.StorageFiles" class="Filesbox" :key="item.FileName">
                  <span class="linkurlcolor" @click="clackFilesProcess(item.FileUrl)">{{item.FileName}}</span>
                  <Tooltip content="删除" placement="top">
                    <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                  </Tooltip>
                  <Tooltip content="打印" placement="top">
                    <Icon type="ios-print-outline" @click="fileprint(item.FileUrl)" />
                  </Tooltip>
                </div>
              </li>
              <!-- <li style="position: absolute; top: 147px; right: 0;"
                  v-if="waybillinfo.HKDeliveryType==2&&waybillinfo.DeliveryNoticeStatus==1">
                <div>
                  <Button type="primary" @click="TakeGoods">{{TakeGoodsName}}</Button>
                </div>
              </li> -->
            </ul>
            </Col>
            <Col span="3">
            </Col>
          </Row>
         
        </div>
      </div>

      <!-- 产品清单 -->
      <div class="itembox">
        <p class="detailtitle">产品清单</p>
        <div style="margin:15px 0">
          <ButtonGroup style="width:28%">
            <Input v-model.trim="searchkey"
                   placeholder="请输入品牌或型号"
                   clearable
                   @on-clear="search_pro"
                   style="width:80%;float:left;position: relative;left: 3px" />
            <Button style="float:left" @click="search_pro" type="primary" :disabled='Operateddisable==false?true:false'>筛选</Button>
          </ButtonGroup>
          <Button type="primary" @click="Labelprint" :disabled="isdisabled==true?true:false">标签打印</Button>
          <Button type="primary"
                  @click="clickshowchangebox"
                  :disabled="isdisabled==true?true:false">
            一键入箱
          </Button>
          <!-- <Button type="primary" @click="PrintBoxcode" :disabled='Operateddisable==false?true:false'>箱签打印</Button> -->
          <Button type="primary" @click="showBudgetIn" :disabled='Operateddisable==false?true:false'>收入</Button>
          <Button type="primary" @click="showBudgetOut" :disabled='Operateddisable==false?true:false'>支出</Button>
          <Button type="primary" @click="abnormal_enter=true" :disabled="isdisabled==true?true:false">无通知产品录入</Button>
          <Button type="primary" v-if="inWarehousedays>FreeStockDays" @click="showBudget2">仓储费录入</Button>
          <span v-if="inWarehousedays>FreeStockDays" style="font-size:11px;color:red">（请收取仓储费，总共需收取仓储费的时长：{{inWarehousedays-FreeStockDays}}天）</span>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table ref="selection"
                   :columns="tabletitle"
                   :data="ProductList"
                   :loading="loading"
                   @on-selection-change="handleSelectRow">
              <template slot-scope="{ row, index }" slot="indexs">
                {{index+1}}
              </template>
              <template slot-scope="{ row, index }" slot="SpecialType">
                <div>
                  <ul>
                    <li>
                      <Tag v-if="(row.OrderItemType&1)>0" color="primary">商检</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.OrderItemType&2)>0" color="warning">CCC</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.OrderItemType&8)>0" color="error">禁运</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.OrderItemType&32)>0" color="magenta">高价值</Tag>
                    </li>
                  </ul>
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="OrderItemQty">
                <span v-if="row.iscx!=false">{{row.OrderItemQty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="PackedQty">
                <span v-if="row.iscx!=false">{{row.PackedQty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="RelQty">
                <div v-if="row.iscx!=false">
                  <P v-if="row.isenter!=true">
                    <span v-if="Number(row.RelQty)<=0">0</span>
                    <span v-else>{{row.RelQty}}</span>
                  </P>
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="Country_origin">
                <span>{{row.Origin+" "+row.OriginText}}</span>
                <Icon class="sethover" @click="clickOrigin(row.Origin,row,index)" type="md-create" />
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button type="primary"
                          size="small"
                          icon="md-checkmark"
                          @click="chaixiang(index,row)">
                    拆项
                  </Button>
                </div>
              </template>
            </Table>
          </div>
        </div>
      </div>

      <!-- 已装箱信息 -->
      <div class="itembox">
        <p class="detailtitle">已装箱信息</p>
        <div>
          <div class="detail_tablebox">
            <Table ref="Packedselection"
                   :columns="Packedtabletitle"
                   :data="PackedProductList"
                   :loading="PackedLoading">
              <template slot-scope="{ row, index }" slot="indexs">
                {{index+1}}
              </template>

              <template slot-scope="{ row, index }" slot="OrderID">
                <span>{{row.OrderID}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="BoxIndex">
                <span>{{row.BoxIndex}}</span>
                <Icon v-if="waybillinfo.EntryNoticeStatus!=3" type="md-create" @click="changeboxcode(row)" />
              </template>
              <template slot-scope="{ row, index }" slot="Model">
                <span>{{row.Model}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="SpecialType">
                <div>
                  <ul>
                    <li>
                      <Tag v-if="(row.SpecialType&1)>0" color="primary">商检</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.SpecialType&2)>0" color="warning">CCC</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.SpecialType&8)>0" color="error">禁运</Tag>
                    </li>
                    <li>
                      <Tag v-if="(row.SpecialType&32)>0" color="magenta">高价值</Tag>
                    </li>
                  </ul>
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="Manufacturer">
                <span>{{row.Manufacturer}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="DateCode">
                <span>{{row.Batch}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Origin">
                <span>{{row.Origin}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="GrossWeight">
                <span>{{row.GrossWeight}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="OrderItemQty">
                <span>{{row.OrderItemQty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Quantity">
                <span>{{row.Quantity}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="CarrierName">
                <span>{{row.CarrierName}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="OrderWaybillCode">
                <span>{{row.OrderWaybillCode}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button type="error"
                          size="small"
                          icon="md-checkmark"
                          @click="cancelPacking(index,row)">
                    删除
                  </Button>
                </div>
              </template>
            </Table>
          </div>
        </div>
        <p class="detailtitle">异常到货信息</p>
        <div>
          <div class="detail_tablebox">
            <Table ref="UnExceptedselection"
                   :columns="UnExceptedtabletitle"
                   :data="UnExceptedList"
                   :loading="UnExceptedLoading">
              <template slot-scope="{ row, index }" slot="indexs">
                {{index+1}}
              </template>
              <template slot-scope="{ row, index }" slot="OrderID">
                <span>{{row.OrderID}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="BoxIndex">
                <span>{{row.BoxIndex}}</span>
                <Icon v-if="waybillinfo.EntryNoticeStatus!=3" type="md-create" @click="changeUnExpecteddBoxCode(row)" />
              </template>
              <template slot-scope="{ row, index }" slot="Model">
                <span>{{row.Model}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Manufacturer">
                <span>{{row.Brand}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="DateCode">
                <span>{{row.Batch}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Origin">
                <span>{{row.Origin}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="GrossWeight">
                <span>{{row.GrossWeight}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Quantity">
                <span>{{row.Qty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button type="error"
                          size="small"
                          icon="md-checkmark"
                          @click="delUnExcepted(index,row)">
                    删除
                  </Button>
                </div>
              </template>
            </Table>
          </div>
        </div>
        <div style="width:100%;min-height:50px;background:#f5f7f9;margin:5px 0">
          <Row>
            <Col style="width: 100px;float: left;">
            <span class>合计：</span>
            </Col>
            <Col style="width: 200px;float: left;">
            <span class>总重量(Kg)：</span>
            <span>{{SumInfo.totalWeight}}</span>
            </Col>
            <Col style="width: 200px;float: left;">
            <span class>总件数：</span>
            <span>{{SumInfo.totalBoxes}}</span>
            </Col>
            <Col style="width: 200px;float: left;">
            <span class>总数量：</span>
            <span>{{SumInfo.totalItems}}</span>
            </Col>
            <Col style="width: 200px;float: left;">
            <Button type="primary" @click="SealOrderCheck" :disabled="isSealDisabled==true?true:false">封箱</Button>
            <Button type="primary" @click="CancelSealOrder" :disabled="isCancelSealDisabled==true?true:false">取消封箱</Button>
            </Col>
          </Row>
        </div>
      </div>

    </div>
    <!-- 输送地列表与更改 开始-->
    <Modal v-model="setOrigin" title="选择原产地" @on-cancel="cancel">
      <Select v-model="Origin" filterable :label-in-value="true" @on-change="changOrigin">
        <Option v-for="item in Conveyingplace"
                :value="item.Code"
                :key="item.ID"
                :label="item.Code+' '+item.Name">
        </Option>
      </Select>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="primaryOrigin">确定</Button>
      </div>
    </Modal>
    <!-- 拆封型号 开始-->
    <Modal v-model="splitModel" title="拆分型号" @on-cancel="cancel">
      <ul>
        <li style="margin-left:30px">
          <span class="detail_title1">型号：</span>
          <Input class="inputitems" style="width:300px;" v-model="SplitModel_datas.Model" :disabled="true" />
        </li>
        <li style="margin-left:30px;margin-top:10px">
          <span class="detail_title1">品牌：</span>
          <Input class="inputitems" style="width:300px" v-model.trim="SplitModel_datas.Manufacturer" @on-enter="splitModelConfirm" />
        </li>
        <li style="margin-left:30px;margin-top:10px">
          <span class="detail_title1">产地：</span>
          <Select v-model.trim="SplitModel_datas.Origin" filterable :label-in-value="true" @on-change="changOrigin" style="width:300px">
            <Option v-for="item in Conveyingplace"
                    :value="item.Code"
                    :key="item.ID"
                    :label="item.Code+' '+item.Name">
            </Option>
          </Select>
        </li>
        <li style="margin-left:30px;margin-top:10px">
          <span class="detail_title1">数量:</span>
          <Input class="inputitems" style="width:300px;" v-model="SplitModel_datas.Quantity" @on-enter="splitModelConfirm" />
        </li>
      </ul>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="splitModelConfirm" :disabled="splitDisable">确定</Button>
      </div>
    </Modal>
    <!-- 拆封型号 结束-->
    <!-- 未通知产品信息录入  开始-->
    <Modal v-model="abnormal_enter"
           title="无通知产品录入"
           width="90"
           ref="modal"
           @on-ok="ok_abnormal"
           @on-cancel="cancel"
           :mask-closable="false"
           @on-visible-change="changeabnormal_enter">
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>
      <div>
        <div>
          <p class="detailtitle">
            无通知产品录入
            <span style="margin-left: 15px;color:red;">注意：仅能录入该运单下与通知型号不匹配的产品信息</span>
          </p>
          <div style="padding:15px 0px;">
            <Noting-Enter :key="timer" ref="NotingEntername" :Storagesarr="Storages" :Storehouselist="Storehouselist" :status='2' :EnterCode='waybillinfo.ClientCode' :orderID='waybillinfo.OrderID'></Noting-Enter>
          </div>
        </div>
      </div>
    </Modal>
    <!-- 未通知产品信息录入  结束-->
    <!-- 装箱 开始-->
    <Modal v-model="PackingModal" title="装箱" @on-cancel="cancel">
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
               placeholder="请输入箱号"
               style="width:80%"
               @on-blur='checkBoxCode(newboxcode)' />
      </p>
      <p style="padding-top:10px;">
        <label> <em class="Mustfill">*</em>重量(Kg)： </label>
        <Input v-model="sharevalue" placeholder="请输入重量" @on-blur='TestAVGWeightsum' @on-enter='TestAVGWeightsum' style="width:80%" />
      </p>
      <p style="padding-top:10px;">
        <label> <em class="Mustfill">&nbsp;</em>推算重量(Kg)： </label>
        <span>{{AVGWeightsum}}</span>
      </p>
      <p style="padding-top:10px;">
        <Checkbox v-model="PackingInfo_datas.IsExpress">是否国际快递</Checkbox>
      </p>
      <p style="padding-top:10px;">
        <label>
          快递公司：
        </label>
        <Select v-model.trim="PackingInfo_datas.CarrierID" filterable :label-in-value="true" @on-change="changeExpress" :disabled="!PackingInfo_datas.IsExpress" style="width:80%">
          <Option v-for="item in ExpressCompanyInfo"
                  :value="item.ID"
                  :key="item.Code"
                  :label="item.Code+' '+item.Name">
          </Option>
        </Select>
      </p>
      <p style="padding-top:10px;">
        <label>
          快递单号：
        </label>
        <Input v-model.trim="PackingInfo_datas.WaybillNo" :disabled="!PackingInfo_datas.IsExpress" style="width:80%" />
      </p>

      <div class="detail_tablebox" style="padding-top:10px;">
        <Table ref="waybillSelection"
               :columns="Waybilltabletitle"
               :data="WaybillList"
               :loading="waybillListLoading">
          <template slot-scope="{ row, index }" slot="indexs">
            {{index+1}}
          </template>
          <template slot-scope="{ row, index }" slot="CarrierCode">
            <span>{{row.CarrierCode}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="WaybillNo">
            <span>{{row.WaybillNo}}</span>
          </template>
        </Table>
      </div>
      <div>
        <span v-if="isSensitive" style="font-size:11px;color:red">({{SensitiveAreaShow}}不能与一般产地装在同一个箱子中）</span>
      </div>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="btnPacking" :disabled='btnPackingdisable'>确定</Button>
      </div>
    </Modal>

    <!--仓储费录入 开始-->
    <Modal title="仓储费录入"
           v-model="storagecharge"
           :mask-closable="false"
           :footer-hide="true"
           @on-visible-change="changestoragecharge"
           :width='60'>
      <p slot="header">
        仓储费录入
        <span style="font-size:14px;padding-left:10px">(总共需收取仓储费的时长：{{inWarehousedays-FreeStockDays}}天)</span>
      </p>
      <Storagecharge ref='Storagecharge' :OrderID="mainOrderID" :timedifference="inWarehousedays-FreeStockDays" :TinyOrderID="tinyOrderID" @Parentfun="Parentfun"></Storagecharge>
    </Modal>
    <!--仓储费录入 结束-->
    <!--收入费用录入 开始-->
    <Modal title=""
           v-model="feeRecord"
           :mask-closable="false"
           :footer-hide="true"
           @on-visible-change="changefeeRecord"
           :width='60'>
      <HKIncome ref='HKIncome' :EnterType="EnterTypeIn" :OrderID="mainOrderID" :WaybillID="entryNoticeID" :TinyOrderID="tinyOrderID"></HKIncome>
    </Modal>
    <!--收入费用录入 结束-->
    <!--支出费用录入 开始-->
    <Modal title=""
           v-model="feeRecordOut"
           :mask-closable="false"
           :footer-hide="true"
           @on-visible-change="changefeeRecordOut"
           :width='60'>
      <HKIncome ref='HKOut' :EnterType="EnterTypeOut" :OrderID="mainOrderID" :WaybillID="entryNoticeID" :TinyOrderID="tinyOrderID"></HKIncome>
    </Modal>
    <!--支出费用录入 结束-->
    <!-- 新增快递公司 开始 -->
    <Modal v-model="showaddcontacts"
           title="新增快递公司"
           :mask-closable="false">
      <div slot="close">
        <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(8)" />
      </div>
      <Add-contacts ref="Addcontacts" @ok_contacts="ok_contacts" @setdisabled='setdisabled' :Conveyingplace='Conveyingplace'></Add-contacts>
      <div slot="footer">
        <Button @click="ok_addCarrier(8)" :disabled='adddisabled'>取消</Button>
        <Button @click="ok_addCarrier(7)" :disabled='adddisabled' type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 新增快递公司 结束-->
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
    <!-- 修改箱号开始 -->
    <Modal v-model="showEdit"
           title="修改箱号"
           :mask-closable="false"
           @on-visible-change="visiblechange">

      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>

      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日期：
        </label>
        <DatePicker type="date"
                    style="width:80%"
                    :options="options3"
                    placeholder="请选择生成箱号的时间"
                    :clearable="false"
                    :value="saleDate"
                    @on-change="changeDataPacked"></DatePicker>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱号：
        </label>

        <Input v-model.trim="newboxcodePacked"
               maxlength="30"
               placeholder="请输入临时箱号"
               style="width:80%"
               @on-blur="handleCreate1(newboxcodePacked)" />
      </p>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok_changeWait">确定</Button>
      </div>
    </Modal>
    <!-- 修改箱号结束 -->
    <!-- 敏感地区提示开始 -->
    <Modal v-model="SensitiveModal" title="敏感地区提示" @on-cancel="cancel">
      <div>
        <span style="font-size:11px;color:red">({{SensitiveModalContext}}不能与一般产地装在同一个箱子中）</span>
      </div>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="SealOrder">确定</Button>
      </div>
    </Modal>
    <!-- 敏感地区提示结束 -->
    <!-- 操作日志 -->
    <Modal v-model="showlogged"
           :footer-hide='true'
           :mask-closable='false'
           width='65'>
      <div slot="close">
        <Icon style="font-size:21px;color:#cccccc;padding-top: 5px" type="ios-close-circle-outline" />
      </div>
      <div slot="header">
        <span style="font-size:18px;color:#1aaff7;">日志管理</span>
      </div>
      <HKLog ref="logged" v-if="showlogged" :key='loggdetime' :WaybillID='waybillinfo.OrderID'></HKLog>
    </Modal>
    <!-- <div class="dync mount"></div> -->
    <!-- 操作日志 -->
  </div>
</template>
<script>
  import NotingEnter from '@/Pages/HKWarehouse/HKNothingEnter';
  import Storagecharge from '@/Pages/HKExpenses/HKStorageCharge';
  import HKIncome from '@/Pages/HKExpenses/HKIncome';
  import AddCarrier from '../Subassembly/Add_Carrier';
  import AddDriver from '../Subassembly/Add_Driver';
  import AddCar from '../Subassembly/Add_Car';
  import Addcontacts from '../Subassembly/Add_contacts';
  import HKLog from './HKlog.vue';
  import {
    SortingBasicInfo, StockedDays, LoadNoticeItems, GetOrigin, ChangeOrigin, ChangeProductModel, ChangeManufacturer, ChangeBatch, SplitModelData,
    UnExpectedGoods, WeightEstimate, GetExpressInfo, OrderWaybillInfo, BoxIndexValidate, PackingBoxIndex, LoadPackedProduct,
    DeletePacking, Sealed, CancelSealed, HKWaybillUpdate, totalPackNo, LoadUnExcepted, DeleteUnExceptedList, DeliveryNoticesUpdate,
    ChangeBoxIndex, SensitiveArea, CanSeal, ChangeUnExpectedBoxIndex
  } from "../../api/XdtApi"; //引入XDT api 的接口
  import { CgCarriers, GetDriversCars, CgDeleteFiles } from "../../api/CgApi";
  import {
    TemplatePrint,
    GetPrinterDictionary,
    FilePrint,
    FormPhoto,
    SeletUploadFile,
    FilesProcess
  } from "@/js/browser.js";
import Accessrecords from '../Stock/Accessrecords.vue';
  let product_url = require("../../../static/pubilc.dev");
  export default {
    name: "HKDeclare",
    components: {
      "Noting-Enter": NotingEnter,
      'Storagecharge': Storagecharge,
      'HKIncome': HKIncome,
      'Add-Carrier': AddCarrier,
      'Add-Driver': AddDriver,
      'Add-Car': AddCar,
      'Add-contacts': Addcontacts,
      'HKLog': HKLog,
        Accessrecords
    },
    data() {
      return {
        btnPackingdisable: false,//装箱按钮是否可用
        showlogged: false,//操作日志
        loggdetime: '',//操作日志时间
        printurl: product_url.pfwms,
        FreeStockDays: 5,
        SensitiveModal: false,//封箱提示窗口
        SensitiveModalContext: "",
        isSensitive: false,// 敏感产地提示是否显示
        SensitiveAreaShow: "",
        splitDisable: false, //拆分按钮是否可用
        EnterTypeIn: "in",
        EnterTypeOut: "out",
        loading: true,
        waybillListLoading: true,
        PackedLoading: true,
        UnExceptedLoading: true,
        entryNoticeID: this.$route.params.entryNoticeID, //运单号
        mainOrderID: this.$route.params.mainOrderID, //主订单号
        tinyOrderID: this.$route.params.orderID,
        storagecharge: false,//仓储费录入
        feeRecord: false,//收入费用录入
        feeRecordOut: false,//支出费用录入
        TakeGoodsName: "我去提货",
        waybillinfo: {},
        ProductList: [], //分拣列表
        WaybillList: [], //国际快递列表
        PackedProductList: [],//已装箱产品列表
        UnExceptedList: [],//异常到货产品列表
        inWarehousedays: "",
        searchkey: "", //筛选
        Operateddisable: true,
        isdisabled: false,
        isSealDisabled: true,//封箱按钮是否可用
        isCancelSealDisabled: true,//取消封箱按钮是否可用
        SelectRow: [], //多选
        setOrigin: false,//是否显示更改产地窗口
        Conveyingplace: [],//产地信息
        Origin: '',//选中的产地
        ChangeOrigin_datas: {
          OriginValue: '',
          OrderItemID: '',
          AdminID: '',
          OriginText: ''
        },//修改产地参数
        SelectdeItemID: '',//选中行的OrderItemID
        SelectedIndex: '',// 选中的行
        splitModel: false, //是否显示拆项窗口
        abnormal_enter: false,//是否显示无通知产品录入
        Storages: [],
        timer: '',
        Storehouselist: [],
        PackingModal: false,//是否显示装箱窗口
        saleDate: '',//箱号时间
        AVGWeightsum: null,//推算重量
        options3: {
          disabledDate(date) {
            return date && date.valueOf() < Date.now() - 86400000;
          }
        },
        newboxcode: '',//箱号
        sharevalue: null,//分摊的总重量
        ExpressCompanyInfo: [],//国际快递下拉框
        showEdit: false, //已装箱更改箱号
        oldboxcode: "",//已装箱更改箱号
        packingIDBoxChange: "",//选中更改箱号那行的PackingID
        unExpectedBoxChange: "",//选中的异常到货更改箱号那行的ID
        newboxcodePacked: "",
        PackingInfo_datas: {
          OrderID: '',
          EntryNoticeID: '',
          PackingDate: '',
          BoxIndex: '',
          Weight: '',
          IsExpress: false,
          CarrierID: '',
          ExpressCompany: '',
          WaybillNo: '',
          AdminID: '',
          PackingItems: []
        },
        ChangeModel_datas: {
          OrderItemID: '',
          adminID: '',
          ProductModel: ''
        },
        ChangeBrand_datas: {
          OrderItemID: '',
          AdminID: '',
          Manufacturer: ''
        },
        SplitModel_datas: {
          OrderItemID: '',
          Origin: '',
          Manufacturer: '',
          Quantity: '',
          adminID: ''
        },
        UnExpectedGoods_datas: [],//
        EstimateWeight_datas: [],
        CarrierList: [], //承运商列表
        IsPersonal: false,//是否为个人承运商
        CarArr: [], //车牌号
        CarArrName: '',//车牌号名称
        DriversArr: [], // 司机,
        DriversName: '',//司机名称
        adddisabled: false,
        showaddCarrier: false,//是否可以新增承运商
        showaddcontacts: false,//
        oldcrriers: null,//保存初始化的的承运商
        showaddDriver: false,// 是否可以新增司机
        addDriverName: null,//新增司机姓名
        showaddCar: false,//是否可以新增车牌号
        addCarName: null,//新增车牌号名称
        SumInfo: {
          totalWeight: 0,
          totalBoxes: 0,
          totalItems: 0,
        },
        TypeArr: [
          {
            value: 3,
            label: "快递"
          },
          {
            value: 4,
            label: "国际快递"
          },
          {
            value: 1,
            label: "送货上门"
          },
        ],
        tabletitle: [
          {
            type: "selection",
            width: 50,
            align: "center",
            fixed: "left"
          },
          {
            title: "#",
            slot: "indexs",
            align: "left",
            width: 50
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
                h("span", {}, "型号")
              ]);
            },
            key: "Model",
            align: "center",
            width: 270,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: vm.trim(params.row.Model)
                },
                on: {
                  "on-blur"(event) {
                    if (event.target.value != '' && event.target.value != vm.ProductList[params.index].Model) {
                      params.row.Model = event.target.value;
                      vm.ProductList[params.index] = params.row;

                      vm.SelectedItemID = params.row.OrderItemID;
                      vm.ChangeModel_datas.ProductModel = params.row.Model;
                      vm.ChangeModel_datas.OrderItemID = vm.SelectedItemID;
                      vm.ChangeModel_datas.adminID = sessionStorage.getItem("userID");
                      ChangeProductModel(vm.ChangeModel_datas).then(res => {
                        if (res.success) {
                          vm.$Message.success(res.message);
                        } else {
                          vm.$Message.error(res.message);
                        }
                      });
                    }
                    if (vm.SelectRow.length > 0) {
                      for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                        if (params.row.ID == vm.SelectRow[i].ID) {
                          vm.SelectRow[i] = params.row;
                        }
                      }
                    }
                  },
                }
              });
            }
          },
          {
            title: "特殊类型",
            slot: "SpecialType",
            align: "center",
            width: 200
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
                h("span", {}, "品牌")
              ]);
            },
            key: "Brand",
            align: "center",
            width: 220,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: vm.trim(params.row.Brand)
                },
                on: {
                  "on-blur"(event) {
                    if (event.target.value != '' && event.target.value != vm.ProductList[params.index].Brand) {
                      params.row.Brand = event.target.value;
                      vm.ProductList[params.index] = params.row;

                      vm.SelectedItemID = params.row.OrderItemID;
                      vm.ChangeBrand_datas.Manufacturer = params.row.Brand;
                      vm.ChangeBrand_datas.OrderItemID = vm.SelectedItemID;
                      vm.ChangeBrand_datas.AdminID = sessionStorage.getItem("userID");
                      ChangeManufacturer(vm.ChangeBrand_datas).then(res => {
                        if (res.success) {
                          vm.$Message.success(res.message);
                        } else {
                          vm.$Message.error(res.message);
                        }
                      });
                    }
                    if (vm.SelectRow.length > 0) {
                      for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                        if (params.row.ID == vm.SelectRow[i].ID) {
                          vm.SelectRow[i] = params.row;
                        }
                      }
                    }
                  },
                }
              });
            }
          },
          {
            title: "应到",
            slot: "OrderItemQty",
            align: "center",
            width: 100
          },
          {
            title: "已到",
            slot: "PackedQty",
            align: "center",
            width: 100
          },
          {
            title: "剩余",
            slot: "RelQty",
            align: "center",
            width: 100
          },
          {
            title: "批号",
            key: "batch",
            align: "center",
            width: 200,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: vm.trim(params.row.DateCode)
                },
                on: {
                  "on-blur"(event) {
                    if (event.target.value != '' && event.target.value != vm.ProductList[params.index].DateCode) {
                      params.row.DateCode = event.target.value;
                      vm.ProductList[params.index] = params.row;

                      vm.SelectedItemID = params.row.OrderItemID;
                      var changeBatch = {
                        Batch: params.row.DateCode,
                        OrderItemID: vm.SelectedItemID,
                        AdminID: sessionStorage.getItem("userID")
                      };
                      ChangeBatch(changeBatch).then(res => {
                        if (res.success) {
                          vm.$Message.success(res.message);
                        } else {
                          vm.$Message.error(res.message);
                        }
                      });
                    }
                    if (vm.SelectRow.length > 0) {
                      for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                        if (params.row.ID == vm.SelectRow[i].ID) {
                          vm.SelectRow[i] = params.row;
                        }
                      }
                    }
                  },
                }
              });
            }
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
                h("span", {}, "本次到货")
              ]);
            },
            title: "本次到货",
            key: "Quantity",
            align: "center",
            width: 100,
            render: (h, params) => {
              var vm = this;
              var newCurrentQuantity = 0;
              var disabledstatus = false;
              if (Number(params.row.RelQty) < 0) {
                newCurrentQuantity = 0
              } else {
                newCurrentQuantity = params.row.RelQty
              }
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: newCurrentQuantity,
                },
                on: {
                  "on-blur"(event) {
                    if (event.target.value != "") {
                      var reg = /^[0-9]*$/;
                      // var reg = /^\d+(\.\d{0,2})?$/;
                      if (reg.test(event.target.value) == true && event.target.value != 0 && event.target.value <= params.row.RelQty) {
                        params.row.CurrentQuantity = Number(event.target.value);
                        vm.ProductList[params.index] = params.row;
                      } else {
                        vm.$Message.error("只能输入正整数,且不能大于剩余数量!");
                        (event.target.value = null),
                          (params.row.CurrentQuantity = null);
                        vm.ProductList[params.index] = params.row;
                      }
                      // vm.clicktest(params.row);
                    } else {
                      event.target.value = null,
                        params.row.CurrentQuantity = null;
                      vm.ProductList[params.index] = params.row;
                      // vm.clicktest(params.row);
                    }
                    if (vm.SelectRow.length > 0) {
                      for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                        if (params.row.ID == vm.SelectRow[i].ID) {
                          vm.SelectRow[i] = params.row;
                        }
                      }
                    }
                  },
                  'on-change'() {

                  }
                }
              });
            }
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
                h("span", {}, "原产地")
              ]);
            },
            slot: "Country_origin",
            align: "center",
            width: 200
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 100, //220
            //fixed: "right"
          }
        ],
        Waybilltabletitle: [
          {
            width: 50,
            align: "center",
            fixed: "left",
            render: (h, params) => {
              return h('Radio', {
                props: {
                  value: params.row.isselected
                },
                on: {
                  'on-change': () => {
                    this.WaybillList.forEach(items => { // 每次选中下一条数据时将每一条数据设为false
                      this.$set(items, 'isselected', false)// 正确
                    })
                    this.WaybillList[params.index].isselected = true // 当前数据设为已选中状态
                    this.PackingInfo_datas.CarrierID = this.WaybillList[params.index].CarrierID;
                    this.PackingInfo_datas.WaybillNo = this.WaybillList[params.index].WaybillNo;
                  }
                }
              })
            }
          },
          {
            title: "序号",
            slot: "indexs",
            align: "left",
            width: 30
          },
          {
            title: "快递公司",
            slot: "CarrierCode",
            align: "center",
            width: 195
          },
          {
            title: "运单号",
            slot: "WaybillNo",
            align: "center",
            width: 195
          },
        ],
        Packedtabletitle: [
          {
            title: "#",
            slot: "indexs",
            align: "left",
            width: 30
          },
          {
            title: "订单号",
            slot: "OrderID",
            align: "center",
            width: 150
          },
          {
            title: "箱号",
            slot: "BoxIndex",
            align: "center",
            width: 80
          },
          {
            title: "型号",
            slot: "Model",
            align: "center",
            width: 200
          },
          {
            title: "特殊类型",
            slot: "SpecialType",
            align: "center",
            width: 150
          },
          {
            title: "品牌",
            slot: "Manufacturer",
            align: "center",
            width: 200
          },
          {
            title: "批号",
            slot: "DateCode",
            align: "center",
            width: 100
          },
          {
            title: "原产地",
            slot: "Origin",
            align: "center",
            width: 100
          },
          {
            title: "重量",
            slot: "GrossWeight",
            align: "center",
            width: 80
          },
          {
            title: "应到",
            slot: "OrderItemQty",
            align: "center",
            width: 100
          },
          {
            title: "实到",
            slot: "Quantity",
            align: "center",
            width: 100
          },
          {
            title: "承运商",
            slot: "CarrierName",
            align: "center",
            width: 150
          },
          {
            title: "运单号",
            slot: "OrderWaybillCode",
            align: "center",
            width: 150
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 100, //220
            //fixed: "right"
          }
        ],
        UnExceptedtabletitle: [
          {
            title: "#",
            slot: "indexs",
            align: "left",
            width: 50
          },
          {
            title: "订单号",
            slot: "OrderID",
            align: "center",
            width: 250
          },
          {
            title: "箱号",
            slot: "BoxIndex",
            align: "center",
            width: 100
          },
          {
            title: "型号",
            slot: "Model",
            align: "center",
            width: 250
          },
          {
            title: "品牌",
            slot: "Manufacturer",
            align: "center",
            width: 250
          },
          {
            title: "批号",
            slot: "DateCode",
            align: "center",
            width: 200
          },
          {
            title: "原产地",
            slot: "Origin",
            align: "center",
            width: 200
          },
          {
            title: "重量",
            slot: "GrossWeight",
            align: "center",
            width: 145
          },
          {
            title: "实到",
            slot: "Quantity",
            align: "center",
            width: 145
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 100, //220
            //fixed: "right"
          }
        ],
      }
    },
    created() {
      this.SortingBasicInfo(this.$route.params.entryNoticeID);
      this.LoadFirst(this.$route.params.entryNoticeID, this.$route.params.orderID)
      // this.LoadNoticeItems(this.$route.params.entryNoticeID);
      // this.LoadPackedProduct(this.$route.params.orderID);
      this.LoadUnExcepted(this.$route.params.orderID);
      this.LoadOrigin();
      this.LoadExpress();
      this.StockedDays(this.$route.params.orderID);
    },
    methods: {
      SortingBasicInfo(id) {
        if (id != undefined) {
          SortingBasicInfo(id).then(res => {
            this.waybillinfo = res.obj;
            this.loading = false;
            this.Carriers(this.waybillinfo.HKDeliveryType)
            if (this.waybillinfo.HKDeliveryType == 2 && this.waybillinfo.CarrierID == 'Personal') {
              this.IsPersonal = true
            } else {
              this.GetDriversCars(this.waybillinfo.CarrierID);
              this.IsPersonal = false
            }
            this.CarArrName = this.waybillinfo.CarNumber
            this.DriversName = this.waybillinfo.DriverID
            this.$forceUpdate();
          });
        }
      },
      StockedDays(orderID) {
        if (orderID != undefined) {
          StockedDays(orderID).then(res => {
            this.inWarehousedays = res.data;
            this.$forceUpdate();
          });
        }
      },
      LoadFirst(entryNoticeID, orderID) {
        if (entryNoticeID != undefined && orderID != undefined) {
          Promise.all([LoadNoticeItems(entryNoticeID, ""), LoadPackedProduct(orderID)]).then(res => {
            this.ProductList = res[0].obj;
            this.PackedProductList = res[1].obj.rows;
            this.PackedLoading = false;
            this.ButtonDisable();
            this.sumcal();
          })
        }
      },
      LoadNoticeItems(entryNoticeID) {
        if (entryNoticeID != undefined) {
          LoadNoticeItems(entryNoticeID, "").then(res => {
            this.ProductList = res.obj;
            this.ButtonDisable();
          });
        }
      },
      LoadPackedProduct(orderID) {
        if (orderID != undefined) {
          LoadPackedProduct(orderID).then(res => {
            this.PackedProductList = res.obj.rows;
            this.PackedLoading = false;
            this.ButtonDisable();
            this.sumcal();
          });
        }
      },
      LoadUnExcepted(orderID) {
        if (orderID != undefined) {
          LoadUnExcepted(orderID).then(res => {
            this.UnExceptedList = res.obj.rows;
            this.UnExceptedLoading = false;
            this.sumcal();
          });
        }
      },
      LoadOrigin() {
        GetOrigin().then(res => {
          this.Conveyingplace = res.obj;
        });
      },
      LoadExpress() {
        GetExpressInfo().then(res => {
          this.ExpressCompanyInfo = res.obj;
        });
      },
      //更改到货方式
      changewaybillType(value) {
        this.Carriers(value)
        if (this.CarrierList.length <= 0) {
          this.waybillinfo.CarrierID = ''
        } else {
          this.waybillinfo.CarrierID = this.oldcrriers
        }
      },
      handleSelectRow(value) {
        //多选事件 获取选中的数据
        this.SelectRow = value;
      },
      search_pro() {
        //筛选按钮
        this.loading = true;
        LoadNoticeItems(this.entryNoticeID, this.searchkey).then(res => {
          this.ProductList = res.obj;
          var selectedID = [];
          for (var i = 0; i < this.SelectRow.length; i++) {
            selectedID.push(this.SelectRow[i].ID);
          }
          this.ProductList.forEach(item => {
            if (selectedID.indexOf(item.ID) >= 0) {
              item._checked = true;
            }
          });
          this.loading = false;
          this.ButtonDisable();
          this.sumcal();
        });
      },
      search_Packed() {
        this.PackedLoading = true;
        LoadPackedProduct(this.waybillinfo.OrderID).then(res => {
          this.PackedProductList = res.obj.rows;
          this.PackedLoading = false;
          this.ButtonDisable();
          this.sumcal();
        });
      },
      search_UnExpected() {
        this.UnExceptedLoading = true;
        LoadUnExcepted(this.waybillinfo.OrderID).then(res => {
          this.UnExceptedList = res.obj.rows;
          this.UnExceptedLoading = false;
          this.sumcal();
        });
      },
      Labelprint() {
        //标签打印 选中多个
        var arr = this.SelectRow;
        var printsrr = [];
        if (arr.length <= 0) {
          this.$Message.error("请选择要操作的产品项");
        } else {
          for (var i = 0; i < arr.length; i++) {
            var obj = {
              Quantity: arr[i].CurrentQuantity == undefined ? arr[i].RelQty : arr[i].CurrentQuantity, //数量
              inputsID: arr[i].ID, //id
              Catalog: '', //品名
              PartNumber: arr[i].Model, //型号
              Manufacturer: arr[i].Brand, //品牌
              Packing: ' ', //包装
              PackageCase: ' ', //封装
              origin: arr[i].Origin + ' ' + arr[i].OriginText, //产地
              SourceDes: "代报关",//业务
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
      trim(str) {
        //去除前后空格
        if (str) {
          return str.replace(/(^\s*)|(\s*$)/g, "");
        }
      },
      clickOrigin(value, row, index) {
        if (row.PackedQty == 0) {
          this.setOrigin = true;
          this.Origin = value;
          this.SelectedItemID = row.OrderItemID;
          this.SelectedIndex = index;
        } else {
          this.$Message.error("已到货不能更改产地，如产地不一致，请拆分型号");
        }
      },
      cancel() {
        this.setOrigin = false;
        this.splitModel = false;
        this.PackingModal = false;
        this.isSensitive = false;
        this.SensitiveAreaShow = "";
        this.showEdit = false;
        this.oldboxcode = null;
        this.packingIDBoxChange = null;
        this.unExpectedBoxChange = null;
        this.SensitiveModal = false;
        this.SensitiveModalContext = null;
        this.btnPackingdisable = false;
      },
      changOrigin(value) {
        this.ChangeOrigin_datas.OriginValue = value.value;
        this.ChangeOrigin_datas.OriginText = value.label;
      },
      changeExpress(value) {

      },
      primaryOrigin() {
        var vm = this;
        vm.ChangeOrigin_datas.OrderItemID = this.SelectedItemID;
        vm.ChangeOrigin_datas.AdminID = sessionStorage.getItem("userID");
        ChangeOrigin(this.ChangeOrigin_datas).then(res => {
          if (res.success) {
            vm.ProductList[vm.SelectedIndex].Origin = this.ChangeOrigin_datas.OriginValue;
            vm.ProductList[vm.SelectedIndex].OriginText = this.ChangeOrigin_datas.OriginText.slice(3);
            vm.setOrigin = false;
            var selectedID = [];
            for (var i = 0; i < this.SelectRow.length; i++) {
              selectedID.push(this.SelectRow[i].ID);
            }
            this.ProductList.forEach(item => {
              if (selectedID.indexOf(item.ID) >= 0) {
                item._checked = true;
              }
            });
          } else {
            vm.setOrigin = false;
            vm.$Message.error(res.message);
          }
        });
      },
      chaixiang(index, row) {
        var vm = this;
        vm.splitModel = true;
        vm.SplitModel_datas.OrderItemID = row.OrderItemID;
        vm.SplitModel_datas.Model = row.Model;
        vm.SplitModel_datas.Manufacturer = row.Brand;
        vm.SplitModel_datas.Quantity = row.CurrentQuantity == undefined ? row.RelQty : row.CurrentQuantity;
        vm.SplitModel_datas.Origin = row.Origin;
        vm.SplitModel_datas.adminID = sessionStorage.getItem("userID");
        vm.SelectedIndex = index;
      },
      splitModelConfirm() {
        var vm = this;
        if (vm.SplitModel_datas.Quantity == "") {
          vm.$Message.error("请输入数量!");
          return;
        }
        if (vm.SplitModel_datas.Origin == vm.ProductList[vm.SelectedIndex].Origin) {
          vm.$Message.error("拆分型号的产地要与原产地不一致!");
          return;
        }
        if (vm.SplitModel_datas.Quantity == vm.ProductList[vm.SelectedIndex].OrderItemQty) {
          vm.$Message.error("拆分型号的数量不能和原数量相等!");
          return;
        }
        vm.splitDisable = true;
        SplitModelData(this.SplitModel_datas).then(res => {
          if (res.success) {
            vm.$Message.success(res.message);
            vm.splitModel = false;
            vm.search_pro();
          } else {
            vm.$Message.error(res.message);
            vm.splitModel = false;
          }
          vm.splitDisable = false;
        });
      },
      //无通知产品录入保存
      ok_abnormal() {
        for (var i = 0, lens = this.Storages.length; i < lens; i++) {
          if (this.Storages[i].BoxCode.slice(0, 3) != 'HXT') {
            this.$Message.error("请输入正确的箱号");
            return;
          } else if ((this.Storages[i].CurrentQuantity != null) && (this.Storages[i].Product.PartNumber != null) && (this.Storages[i].origin != null) && (this.Storages[i].BoxCode != null) && (this.Storages[i].Summary != null) && (this.Storages[i].Weight != null) && (this.Storages[i].Product.Manufacturer != null)) {
            //if(i==this.Storages.length-1){
            var data = {
              OrderID: this.waybillinfo.OrderID,
              Model: this.Storages[i].Product.PartNumber,
              Brand: this.Storages[i].Product.Manufacturer,
              Qty: this.Storages[i].CurrentQuantity,
              Batch: this.Storages[i].DateCode,
              Origin: this.Storages[i].origin,
              BoxIndex: this.Storages[i].BoxCode,
              GrossWeight: this.Storages[i].Weight,
              Volume: this.Storages[i].Volume,
              UnExpectedReason: this.Storages[i].Summary,
            };
            this.UnExpectedGoods_datas.push(data);
            //}
          } else {
            this.$refs.modal.visible = true;
            this.abnormal_enter = true;
            this.$Message.error("请输入必填项目");
            return;
          }
        }
        this.Storages = [];
        UnExpectedGoods(this.UnExpectedGoods_datas).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.abnormal_enter = false;
            this.UnExpectedGoods_datas = [];
            this.search_UnExpected();
          } else {
            this.$Message.error(res.message);
            this.abnormal_enter = false;
            this.UnExpectedGoods_datas = [];
          }
          this.$forceUpdate();
        });

      },
      changeabnormal_enter(value) {
        if (value == false) {

        } else {
          this.Storages = [];
          var data = {
            iscx: false,
            isenter: true,
            _disabled: false,
            ID: "LR" + 1 + new Date().getTime(),
            NoticeID: null,
            Product: {
              PartNumber: null,
              Manufacturer: null,
              PackageCase: null,
              Packaging: null,
            },
            Conditions: {
              DevanningCheck: false,
              Weigh: false,
              CheckNumber: false,
              OnlineDetection: false,
              AttachLabel: false,
              PaintLabel: false,
              Repacking: false,
              IsCCC: false,
              IsCIQ: false,
              IsEmbargo: false,
              IsHighPrice: false,
              IsDeclared: false,
            },
            WaybillID: null,
            InputID: null,
            DateCode: null,
            Quantity: null,
            ArrivedQuantity: null,
            LeftQuantity: null,
            CurrentQuantity: null,
            Source: null,
            BoxCode: null,
            Weight: null,
            NetWeight: null,
            Volume: null,
            ShelveID: null,
            Type: null,
            origintext: null,
            origin: null,
            originDes: null,
            Summary: null,
            SortingID: "",
            Files: [],
            boxdate: null
          };
          this.Storages.push(data);
          this.timer = new Date().getTime();
          // this.$refs.NotingEntername.Storehouselist=this.Storehouselist
          this.$nextTick(function () {
          });
        }
      },
      clickshowchangebox() {
        if (this.SelectRow.length != 0) {
          const myDate = new Date();
          const year = myDate.getFullYear(); // 获取当前年份
          const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
          const day = myDate.getDate(); // 获取当前日（1-31）
          this.saleDate = `${year}/${month}/${day}`;
          this.PackingInfo_datas.PackingDate = this.saleDate;
          for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
            var data = {
              Model: this.SelectRow[i].Model,
              Qty: 1000
            };
            this.EstimateWeight_datas.push(data);
          }
          WeightEstimate(this.EstimateWeight_datas).then(res => {
            if (res.success) {
              this.AVGWeightsum = res.data;
            } else {
              this.AVGWeightsum = res.message;
            }
          });
          this.LoadWaybillExpress(this.waybillinfo.OrderID);
          this.PackingCheck();
          this.PackingModal = true;
        } else {
          this.$Message.error("请选择一条记录!");
        }
      },
      changeData(val) {
        this.saleDate = val;
        this.PackingInfo_datas.PackingDate = val;
      },
      checkBoxCode(newboxcode) {
        if (newboxcode != "" && newboxcode.slice(0, 3) == 'HXT') {
          var BoxIndexValidate_data = {
            BoxIndex: newboxcode,
            PackingDate: this.saleDate
          };
          BoxIndexValidate(BoxIndexValidate_data).then(res => {
            if (res.success) {
              this.PackingInfo_datas.BoxIndex = newboxcode;
            } else {
              this.newboxcode = "";
              this.PackingInfo_datas.BoxIndex = "";
              this.$Message.error(res.message);
            }
          });
        } else {
          this.$Message.error('请输入正确的箱号')
          this.newboxcode = null
        }
      },
      TestAVGWeightsum() {
        if (this.sharevalue != null) {
          var reg = /^\d+(\.\d{0,5})?$/;
          if (reg.test(this.sharevalue) == false || this.sharevalue == 0) {
            this.$Message.error("请输入数字,小数点保留五位，且不等于零");
            this.sharevalue = null
          } else {
            if (this.sharevalue < (this.AVGWeightsum - this.AVGWeightsum * (1 / 2)) || this.sharevalue > (this.AVGWeightsum + this.AVGWeightsum * (3 / 2))) {
              this.$Modal.warning({ title: '超过浮动50%！', });
            }
          }
        }
      },
      showBudgetIn() {
        this.feeRecord = true;
      },
      showBudgetOut() {
        this.feeRecordOut = true;
      },
      showBudget2() {
        this.storagecharge = true;
        this.$refs.Storagecharge.IncomeParters(this.waybillinfo.MainOrderID)
      },
      changestoragecharge(value) {
        this.storagecharge = value
      },
      changefeeRecord(value) {
        this.feeRecord = value
      },
      changefeeRecordOut(value) {
        this.feeRecordOut = value
      },
      Parentfun() {

      },
      LoadWaybillExpress(orderID) {
        if (orderID != undefined) {
          OrderWaybillInfo(orderID).then(res => {
            this.WaybillList = res.obj;
            this.waybillListLoading = false;
            if (this.WaybillList.length != 0) {
              this.PackingInfo_datas.IsExpress = true;
              this.PackingInfo_datas.CarrierID = this.WaybillList[0].CarrierID;
              this.PackingInfo_datas.WaybillNo = this.WaybillList[0].WaybillNo;
            } else if (this.waybillinfo.HKDeliveryType == 4) {
              this.PackingInfo_datas.IsExpress = true;
              this.PackingInfo_datas.CarrierID = this.waybillinfo.CarrierID;
              this.PackingInfo_datas.WaybillNo = this.waybillinfo.WayBillCode;
            }
          });
        }
      },
      select(selection, row) {
        this.$refs.waybillSelection.clearSelection();
        if (selection.length == 0)
          return;
        this.$refs.waybillSelection.toggleRowSelection(row, true);
      },
      PackingCheck() {
        var sensiArea = {
          BoxIndex: "",
          Areas: []
        };
        sensiArea.BoxIndex = this.PackingInfo_datas.BoxIndex;
        for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
          sensiArea.Areas.push(this.SelectRow[i].Origin);
        }
        var sensiAreas = [];
        sensiAreas.push(sensiArea)
        SensitiveArea(sensiAreas).then(res => {
          if (res.success) {
            this.isSensitive = false;
            this.SensitiveAreaShow = "";
          } else {
            this.isSensitive = true;
            this.SensitiveAreaShow = res.message;
          }
        });
      },
      btnPacking() {
        if (this.sharevalue == "" || this.PackingInfo_datas.BoxIndex == "" || this.PackingInfo_datas.BoxIndex == null) {
          this.$Message.error("请输入箱号和重量");
          return;
        }
        if (this.PackingInfo_datas.IsExpress == true && (this.PackingInfo_datas.CarrierID == "" ||
          this.PackingInfo_datas.WaybillNo == "" ||
          this.PackingInfo_datas.CarrierID == null ||
          this.PackingInfo_datas.WaybillNo == null)) {
          this.$Message.error("请输入快递公司和快递单号");
          return;
        }
        this.btnPackingdisable = true;
        this.PackingInfo_datas.OrderID = this.waybillinfo.OrderID;
        this.PackingInfo_datas.EntryNoticeID = this.entryNoticeID;
        this.PackingInfo_datas.Weight = this.sharevalue;
        this.PackingInfo_datas.AdminID = sessionStorage.getItem("userID");
        var data = {};
        for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
          data = {
            EntryNoticeItemID: this.SelectRow[i].ID,
            OrderItemID: this.SelectRow[i].OrderItemID,
            Quantity: this.SelectRow[i].CurrentQuantity == undefined ? this.SelectRow[i].RelQty : this.SelectRow[i].CurrentQuantity
          };
          this.PackingInfo_datas.PackingItems.push(data);
        }

        PackingBoxIndex(this.PackingInfo_datas).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.PackingModal = false;
            this.btnPackingdisable = false;
            this.SelectRow = [];
            this.search_pro();
            this.search_Packed();
          } else {
            this.$Message.error(res.message);
          }
          this.sharevalue = "";
          this.newboxcode = "";
          this.PackingInfo_datas.BoxIndex = "";
          this.PackingInfo_datas.PackingItems = [];
          this.PackingInfo_datas.CarrierID = "";
          this.PackingInfo_datas.WaybillNo = "";
        });

      },
      SealOrderCheck() {
        var sensiAreas = [];
        var boxIndexs = [];
        for (var i = 0, lens = this.PackedProductList.length; i < lens; i++) {
          var test = boxIndexs.indexOf(this.PackedProductList[i].BoxIndex);
          if (boxIndexs.indexOf(this.PackedProductList[i].BoxIndex) < 0) {
            boxIndexs.push(this.PackedProductList[i].BoxIndex);
          }
        }
        for (var i = 0; i < boxIndexs.length; i++) {
          var sensiArea = {
            BoxIndex: boxIndexs[i],
            Areas: []
          };
          for (var j = 0; j < this.PackedProductList.length; j++) {
            if (this.PackedProductList[j].BoxIndex == boxIndexs[i]) {
              sensiArea.Areas.push(this.PackedProductList[j].Origin.slice(0, 3));
            }
          }
          sensiAreas.push(sensiArea);
        }

        CanSeal(this.tinyOrderID).then(res1 => {
          if (res1.success) {
            SensitiveArea(sensiAreas).then(res => {
              if (res.success) {
                this.SealOrder();
              } else {
                this.SensitiveModal = true;
                this.SensitiveModalContext = res.message;
              }
            });
          } else {
            this.$Message.error(res1.message);
          }
        });
      },
      SealOrder() {
        var data = {
          EntryNoticeID: this.entryNoticeID,
          OrderID: this.waybillinfo.OrderID,
          AdminID: sessionStorage.getItem("userID"),
        };
        Sealed(data).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.isSealDisabled = true;
            this.isCancelSealDisabled = false;
          } else {
            this.$Message.error(res.message);
          }
          this.SensitiveModal = false;
        });
      },
      CancelSealOrder() {
        var data = {
          PackingID: this.PackedProductList[0].PackingID,
        };
        CancelSealed(data).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.isSealDisabled = false;
            this.isCancelSealDisabled = true;
          } else {
            this.$Message.error(res.message);
          }
        });
      },
      cancelPacking(index, row) {
        var data = {
          PackingID: row.PackingID,
          EntryNoticeID: this.entryNoticeID,
          AdminID: sessionStorage.getItem("userID"),
        };
        DeletePacking(data).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.search_pro();
            this.search_Packed();
            this.isSealDisabled = true;
          } else {
            this.$Message.error(res.message);
          }
        });
      },
      delUnExcepted(index, row) {
        DeleteUnExceptedList(row.ID).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
            this.search_UnExpected();
          } else {
            this.$Message.error(res.message);
          }
        });
      },
      sumcal() {
        var totalweight = 0;
        var totalItems = 0;
        var boxInfo = [];
        for (var i = 0, lens = this.PackedProductList.length; i < lens; i++) {
          boxInfo.push(this.PackedProductList[i].BoxIndex);
          totalweight += this.PackedProductList[i].GrossWeight;
          totalItems += this.PackedProductList[i].Quantity;
        }
        for (var i = 0, lens = this.UnExceptedList.length; i < lens; i++) {
          boxInfo.push(this.UnExceptedList[i].BoxIndex);
          totalweight += this.UnExceptedList[i].GrossWeight;
          totalItems += this.UnExceptedList[i].Qty;
        }

        this.SumInfo.totalWeight = totalweight.toFixed(2);
        this.SumInfo.totalItems = totalItems;
        var postInfo = {
          boxes: boxInfo
        }
        totalPackNo(postInfo).then(res => {
          this.SumInfo.totalBoxes = res.data;
        });

      },
      changeacrrier(value) { //改变承运商的时候触发
        if (value != undefined) {
          if (this.waybillinfo.HKDeliveryType == 2 && value.value == 'Personal') {
            this.IsPersonal = true
            this.waybillinfo.CarNumber = this.CarArrName;
            this.waybillinfo.DriverID = this.DriversName;
          } else {
            if (this.waybillinfo.HKDeliveryType == 2) {
              this.IsPersonal = false
              this.waybillinfo.CarNumber = '';
              this.waybillinfo.DriverID = '';
              this.GetDriversCars(value.value);
            } else {
              this.IsPersonal = false
              this.waybillinfo.CarNumber = '';
              this.waybillinfo.DriverID = '';
            }
            this.waybillinfo.CarrierName = value.label
          }
          this.oldcrriers = value.value
        }
      },
      Carriers(type) { //承运商列表
        if (type == 1) {
          type = 2;
        } else if (type == 2) {
          type = 1;
        }
        CgCarriers(type, 'HK', 100).then(res => {
          this.CarrierList = res;
        });
      },
      GetDriversCars(key) {
        //根据送货上门承运商获取司机与车牌号
        GetDriversCars(key).then(res => {
          this.CarArr = []
          var data = res.Transports
          if (data.length > 0) {
            var item = {}
            for (var i = 0; i < data.length; i++) {
              if (data[i].CarNumber1 != null && data[i].CarNumber2 != null) {
                item = {
                  value: data[i].CarNumber1 + '  ' + data[i].CarNumber2
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
          this.DriversArr = res.Drivers; // 司机
        });
      },
      handleCreate3(val) {
        this.CarArr.push({
          ID: val,
          Name: val
        });
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
        } else if (type == 7) {
          this.$refs.Addcontacts.sumbit_btn()
        } else if (type == 8) {
          this.$refs.Addcontacts.delitem()
          this.showaddcontacts = false
        }

      },
      //保存快递公司
      ok_contacts(res) {
        this.adddisabled = false
        if (res != 'false') {
          var housid = sessionStorage.getItem("UserWareHouse")
          var carrie = { ID: res.Carrier.ID, Name: res.Carrier.Name }
          this.CarrierList.push(carrie)
          this.Carriers(this.waybillinfo.HKDeliveryType, housid)
          this.waybillinfo.CarrierID = res.Carrier.ID
          this.waybillinfo.CarrierName = res.Carrier.Name
          this.oldcrriers = res.Carrier.ID
          this.showaddcontacts = false;
          this.$refs.Addcontacts.delitem()
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
          this.waybillinfo.CarNumber = res
          this.showaddCar = false
          this.$refs.AddDriver.delitem()
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
          this.Carriers(this.waybillinfo.Type, housid)
          this.waybillinfo.CarrierID = res.Carrier.ID
          this.waybillinfo.CarrierName = res.Carrier.Name

          var data = { Name: res.Driverinfo.Name, ID: "", }
          this.DriversArr.push(data)
          this.waybillinfo.Driver = res.Driverinfo.Name

          var data = { value: res.Carinfo.CarNumber1, }
          this.CarArr.push(data)
          this.waybillinfo.CarNumber = res.Carinfo.CarNumber1

          this.GetDriversCars(this.waybillinfo.CarrierID);
          this.showaddCarrier = false;
          this.$refs.addCarrier.delitem()

        }
      },
      SeletUpload(type) {// 传照
        window["PhotoUploaded"] = this.changed;
        var data = {
          SessionID: this.waybillinfo.EntryNoticeID,
          AdminID: sessionStorage.getItem("userID"),
          Data: {
            WayBillID: this.waybillinfo.EntryNoticeID,
            WsOrderID: this.waybillinfo.MainOrderID,
            Type: 8000
          }
        };
        SeletUploadFile(data);
      },
      fromphotos(type) {
        window["PhotoUploaded"] = this.changed;
        var data = {
          SessionID: this.waybillinfo.EntryNoticeID,
          AdminID: sessionStorage.getItem("userID"),
          Data: {
            WayBillID: this.waybillinfo.EntryNoticeID,
            WsOrderID: this.waybillinfo.MainOrderID,
            Type: 8000
          }
        };
        FormPhoto(data);
      },
      clackFilesProcess(url) {
        var data = {
          Url: url
        }
        FilesProcess(data)
      },
      fileprint(printurl) {
        var configs = GetPrinterDictionary()
        var getsetting = configs['文档打印']
        getsetting.Url = printurl
        var data = getsetting
        FilePrint(data)
      },
      SaveHKDeliveryInfo() {
        var data = {
          orderID: this.tinyOrderID,
          CarrierID: this.waybillinfo.CarrierID,
          DriverID: this.waybillinfo.DriverID,
          CarNumber: this.waybillinfo.CarNumber,
          WaybillNo: this.waybillinfo.WayBillCode,
          HKDeliveryType: this.waybillinfo.HKDeliveryType
        };
        HKWaybillUpdate(data).then(res => {
          if (res.success) {
            this.$Message.success(res.message);
          } else {
            this.$Message.error(res.message);
          }
        });
      },
      ButtonDisable() {
        if (this.ProductList.length > 0) {
          this.isdisabled = false;
        } else if (this.PackedProductList.length > 0 && this.ProductList.length == 0) {
          this.isdisabled = true;
          this.isSealDisabled = false;
        }
      },
      TakeGoods(status) {
        //我去提货
        //var admin= sessionStorage.getItem("userID")
        var data = {
          orderID: this.tinyOrderID,
          deliveryNoticeStatus: status
        };
        DeliveryNoticesUpdate(data).then(res => {
          if (res.success == true) {
            this.waybillinfo.DeliveryNoticeStatus = status
            this.$forceUpdate();
            if (status == 3) {
              this.$Message.success("提货锁定成功，请去提货");
            } else {
              this.$Message.success("提货完成");
            }
          } else {
            this.$Message.error("提货锁定失败");
          }
        });
      },
      changeboxcode(value) {
        this.showEdit = true;
        this.oldboxcode = value.BoxIndex;
        this.packingIDBoxChange = value.PackingID;
      },
      changeUnExpecteddBoxCode(value) {
        this.showEdit = true;
        this.oldboxcode = value.BoxIndex;
        this.unExpectedBoxChange = value.ID;
      },
      visiblechange(value) {
        if (value == true) {
          const myDate = new Date();
          const year = myDate.getFullYear(); // 获取当前年份
          const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
          const day = myDate.getDate(); // 获取当前日（1-31）
          // 日期格式：2019/07/29 - 2019/07/29
          this.saleDate = `${year}/${month}/${day}`;
        } else {
          this.newboxcodePacked = null;
          this.oldboxcode = null;
          this.packingIDBoxChange = null;
          this.unExpectedBoxChange = null;
        }
      },
      changeDataPacked(val) {
        this.saleDate = val;
        if (this.newboxcodePacked != "" && this.newboxcodePacked != null) {
          this.handleCreate1(this.newboxcodePacked);
        }
      },
      handleCreate1(val) {
        //箱号添加
        if (val != "" && val.slice(0, 3) == 'HXT') {
          var BoxIndexValidate_data = {
            BoxIndex: val,
            PackingDate: this.saleDate
          };
          BoxIndexValidate(BoxIndexValidate_data).then(res => {
            if (res.success) {
              this.newboxcodePacked = val;
            } else {
              this.newboxcodePacked = null;
              this.$Message.error(res.message);
            }
          });
        } else {
          this.$Message.error('请输入正确的箱号')
          this.newboxcodePacked = null
        }
      },
      ok_changeWait() {
        setTimeout(() => {
          this.$forceUpdate();
          this.ok_change();
        }, 2000);
      },
      // 提交修改的箱号
      ok_change() {
        if (this.newboxcodePacked != null && this.newboxcodePacked != "" && this.packingIDBoxChange != null && this.packingIDBoxChange != "") {
          var BoxChange = {
            PackingID: this.packingIDBoxChange,
            PackingDate: this.saleDate,
            NewBoxIndex: this.newboxcodePacked,
            //OldBoxIndex : this.oldboxcode,
            AdminID: sessionStorage.getItem("userID"),
            OrderID: this.tinyOrderID,
          }
          ChangeBoxIndex(BoxChange).then(res => {
            if (res.success) {
              this.search_Packed();
              this.$Message.success(res.message);
              this.newboxcodePacked = null;
              this.packingIDBoxChange = null;
            } else {
              this.$Message.error(res.message)
            }
          });
        } else if (this.newboxcodePacked != null && this.newboxcodePacked != "" && this.unExpectedBoxChange != null && this.unExpectedBoxChange != "") {
          var BoxChange = {
            UnExpectedID: this.unExpectedBoxChange,
            PackingDate: this.saleDate,
            NewBoxIndex: this.newboxcodePacked,
            AdminID: sessionStorage.getItem("userID"),
            OrderID: this.tinyOrderID,
          }
          ChangeUnExpectedBoxIndex(BoxChange).then(res => {
            if (res.success) {
              this.search_UnExpected();
              this.$Message.success(res.message);
              this.newboxcodePacked = null;
              this.unExpectedBoxChange = null;
            } else {
              this.$Message.error(res.message)
            }
          });
        } else {
          this.$Message.error("请选择或输入箱号");
        }
      },
      //删除上传文件按
      handleRemove(file, type) {
        var data = {
          id: file.ID
        }
        CgDeleteFiles(data).then(res => {
          if (res.Success == true) {
            this.Removebackfun(file, type)
            this.$Message.success('删除成功')
          } else {
            this.$Message.error('删除失败')
          }
        })
      },
      //删除成功后，删除本地数据
      Removebackfun(file, type) {
        if (type == "Waybill") {
          this.waybillinfo.StorageFiles.splice(this.waybillinfo.StorageFiles.indexOf(file), 1);
        }
      },
      changed(message) {
        var imgdata = message[0];
        var newfile = {
          FileName: imgdata.FileName,
          ID: imgdata.FileID,
          Url: imgdata.Url,
          Type: 8000
        };
        this.waybillinfo.StorageFiles.push(newfile);
        this.$forceUpdate();
      },
      //操作日志的展示
      logchange() {
        this.showlogged = true
        this.loggdetime = new Date().getTime()
      },
    },
    mounted() {

    },
  }
</script>
