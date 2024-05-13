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
  <div class="Declare">
    <div style="width:100%;">
      <div class="itembox">
        <p class="detailtitle">代报关基础信息</p>
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row v-if="">
            <Col style="width: 20%;float: left;">
            <ul class="detail_li detail1">
              <li class="itemli">
                <span class="detail_title1">ID：</span>
                <span>{{waybillinfo.ID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状态：</span>
                <span>
                  {{waybillinfo.ExcuteStatusDes}}
                  <Button v-if="waybillinfo.ID!=null" icon='md-calendar' type="success" size='small' @click="logchange">日志管理</Button>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title1">提货状态：</span>
                <Tag color="magenta" v-if="waybillinfo.Type&&waybillinfo.LoadingExcuteStatus==100">等待提货</Tag>
                <Tag color="blue" v-if="waybillinfo.Type==1&&waybillinfo.LoadingExcuteStatus==105">提货中</Tag>
                <Tag color="green" v-if="waybillinfo.Type==1&&waybillinfo.LoadingExcuteStatus==200">提货完成</Tag>
              </li>
              <li class="itemli">
                <span class="detail_title1">业务类型：</span>
                <span>{{waybillinfo.SourceDes}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">客服人员：</span>
                <span>{{waybillinfo.Merchandiser}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1" style="width:112px">是否收取入仓费：</span>
                <span>
                  <Tag color="red" v-if="waybillinfo.ChargeWH=='不收取'">{{waybillinfo.ChargeWH}}</Tag>
                  <em v-else>{{waybillinfo.ChargeWH}}</em>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Condition!=null">
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
            <Col style="width: 26%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title2">制单时间：</span>
                <span>{{waybillinfo.CreateDate|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">供应商：</span>
                <span style="display: inline-block;width: 75%;line-height: 20px;">
                  {{waybillinfo.Supplier}}
                  <Tag color="volcano">LV {{waybillinfo.SupplierGrade}}</Tag>
                </span>
                <!-- <Tooltip content="一级" placement="right">
                <img style="vertical-align: middle" src="../../assets/img/vip_1.png" alt />
              </Tooltip> -->
              </li>
              <li class="itemli">
                <span class="detail_title2">到货方式：</span>
                <span v-if="waybillinfo.Type==1">{{waybillinfo.TypeDes}}</span>
                <Select v-else
                        v-model="waybillinfo.Type"
                        style="width:40%"
                        @on-change='changewaybillType'
                        :disabled='waybillinfo.ExcuteStatus==120?true:false'>
                  <Option v-for="(item,index) in TypeArr"
                          :value="item.value"
                          :key="index">
                    {{ item.label}}
                  </Option>
                </Select>
                <!-- <a href="javascript:void(0)" @click="showhistory">历史到货</a> -->
                </Badge>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title2">自提时间：</span>
                <span>{{waybillinfo.TakingDate|showDateexact}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2">
                <span class="detail_title2">到货时间：</span>
                <span>{{waybillinfo.AppointTime|showDateexact}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title2">提货文件：</span>
                <span style="display: inline-block;">
                  <p v-for="(item,index) in  waybillinfo.Files" v-if="item.Type==10">
                    <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                    <a v-if="item.Type==10" @click="fileprint(item.Url)">打印</a>
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
                  {{waybillinfo.EnterCode}}({{waybillinfo.ClientName}})
                  <Tag color="geekblue">LV {{clientGrade}}</Tag>
                  <i style=" background: #f90; color: #ffffff; padding: 6px; border-radius: 50%; font-size: 17px;" v-if="waybillinfo.IsClientLs==true">租</i>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3">运单号(本次)：</span>
                <span>
                  <Input style="width:60%" v-model="waybillinfo.Code" />
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title3">
                  <em v-if="waybillinfo.Type==1" class="Mustfill">*</em>
                  <i v-if="waybillinfo.Type==3">快递公司(本次):</i>
                  <i v-else>承运商(本次):</i>
                </span>
                <span>
                  <Select v-model="waybillinfo.CarrierID"
                          :label-in-value="true"
                          filterable
                          @on-change="changeacrrier"
                          style="width:60%"
                          :disabled='waybillinfo.ExcuteStatus==120?true:false'>
                    <Option v-for="item in CarrierList"
                            :value="item.ID"
                            :key="item.ID">
                      {{ item.Name }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCarrier=true" v-if="waybillinfo.ExcuteStatus!=120&&waybillinfo.Type==1&&waybillinfo.CarrierID!='Personal'" />
                  <Icon type="md-add" @click="showaddcontacts=true" v-if="waybillinfo.ExcuteStatus!=120&&waybillinfo.Type==3&&waybillinfo.CarrierID!='Personal'" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1&&IsPersonal==false">
                <span class="detail_title3"><em v-if="waybillinfo.Type==1" class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Select transfer
                          v-model="waybillinfo.Driver"
                          style="width:60%"
                          :label-in-value="true"
                          filterable
                          :disabled='waybillinfo.ExcuteStatus==120?true:false'>
                    <Option v-for="(item,index) in DriversArr"
                            :value="item.Name"
                            :key="index">
                      {{ item.Name }}
                    </Option>
                  </Select>
                </span>
                <Icon type="md-add" @click="showaddDriver=true" v-if="waybillinfo.Type==1&&waybillinfo.CarrierID!=null" />
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1&&IsPersonal==false">
                <span class="detail_title3"><em v-if="waybillinfo.Type==1" class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Select transfer
                          v-model="waybillinfo.CarNumber1"
                          style="width:60%"
                          :label-in-value="true"
                          @on-create="handleCreate3"
                          :disabled='waybillinfo.ExcuteStatus==120?true:false'>
                    <Option v-for="item in CarArr"
                            :value="item.value"
                            :key="item.value">
                      {{ item.value }}
                    </Option>
                  </Select>
                  <Icon type="md-add" @click="showaddCar=true" v-if="waybillinfo.Type==1&&waybillinfo.CarrierID!=null" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                <span>
                  <Input v-model="waybillinfo.Driver" style="width:60%" placeholder="请输入司机" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1&&IsPersonal==true">
                <span class="detail_title3"><em class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                <span>
                  <Input v-model="waybillinfo.CarNumber1" style="width:60%" placeholder="请输入车牌号" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>联&nbsp;&nbsp;系&nbsp;&nbsp;人:</span>
                <span> {{waybillinfo.TakingContact}} &nbsp;&nbsp;(电话:{{waybillinfo.TakingPhone}})</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>提货地址:</span>
                <span>{{waybillinfo.TakingAddress}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title3"><em>&nbsp;&nbsp;</em>输&nbsp;&nbsp;送&nbsp;&nbsp;地：</span>
                <span>
                  <span v-if="waybillinfo.ConsignorPlaceText!=''">{{waybillinfo.ConsignorPlaceText}}</span>
                  <span v-else style="color:red;">暂无输送地</span>
                  <Icon class="sethover"
                        @click="clickClient(waybillinfo.ConsignorPlace,'ClientCode','ClientCode')"
                        type="md-create" />
                </span>
              </li>
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
                  <Button :disabled='Operateddisable==false?true:false'
                          type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload('Waybill')">
                    传照
                  </Button>
                </div>
                <div class="setupload">
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
                <div v-for="item in waybillinfo.Files" class="Filesbox" v-if="item.Type==8000">
                  <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <!-- <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                <Icon type="ios-print-outline" @click="fileprint(item.Url)"/> -->
                  <Tooltip content="删除" placement="top">
                    <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                  </Tooltip>
                  <Tooltip content="打印" placement="top">
                    <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                  </Tooltip>
                </div>
              </li>
              <li style="position: absolute; top: 147px; right: 0;"
                  v-if="waybillinfo.Type==1&&waybillinfo.LoadingExcuteStatus==100">
                <div>
                  <Button type="primary" @click="TakeGoods">{{TakeGoodsName}}</Button>
                </div>
              </li>
            </ul>
            </Col>
            <Col span="3">

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
          <Button type="primary" @click="showBudget('meet','in',waybillinfo.Source)" :disabled='Operateddisable==false?true:false'>收入</Button>
          <Button type="primary" @click="showBudget('meet','out',waybillinfo.Source)" :disabled='Operateddisable==false?true:false'>支出</Button>
          <Button type="primary" @click="abnormal_enter=true" :disabled="isdisabled==true?true:false">无通知产品录入</Button>
          <Button type="primary" @click="showBudget2" v-if="showStoragecharge==true">仓储费录入</Button>
          <span v-if="showStoragecharge==true" style="font-size:11px;color:red">（请收取仓储费，总共需收取仓储费的时长：{{timedifference}}天）</span>
          <div style="float:right">
            <Icon type="md-sync" @click="sync_btn" class="sync_btn hoverbtn" />
            <Button :disabled="isdisabled==true?true:false"
                    type="primary"
                    shape="circle"
                    icon="md-checkmark"
                    @click="clickshowchangebox">
              入库完成
            </Button>
            <Button :disabled="isdisabled==true?true:false"
                    type="warning"
                    shape="circle"
                    icon="ios-alert-outline"
                    @click="isAbnormalclick">
              到货异常
            </Button>
          </div>
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
              <template slot-scope="{ row, index }" slot="PartNumber">
                <div style="display: flex;justify-content: space-between;align-items: center;">
                  <div style="float:left;text-align:left">
                    <span style="width:120px;overflow: hidden;">{{row.Product.PartNumber}}</span>
                  </div>
                  <ul style="float:right;">
                    <li>
                      <Tag v-if="row.Conditions.IsCIQ==true" color="primary">商检</Tag>
                    </li>
                    <li>
                      <Tag v-if="row.Conditions.IsCCC==true" color="warning">CCC</Tag>
                    </li>
                    <li>
                      <Tag v-if="row.Conditions.IsEmbargo==true" color="error">禁运</Tag>
                    </li>
                    <li>
                      <Tag v-if="row.Conditions.IsHighPrice==true" color="magenta">高价值</Tag>
                    </li>
                  </ul>
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="Arrival">
                <span v-if="row.iscx!=false">{{row.Quantity}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Already">
                <span v-if="row.iscx!=false">{{row.ArrivedQuantity}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Surplus">
                <div v-if="row.iscx!=false">
                  <P v-if="row.isenter!=true">
                    <span v-if="Number(row.LeftQuantity)<=0">0</span>
                    <span v-else>{{row.LeftQuantity}}</span>
                  </P>
                </div>
              </template>
              <template slot-scope="{ row, index }" slot="Country_origin">
                <span>{{row.origintext}}</span>
                <Icon class="sethover" v-if="row._disabled==false" @click="clickClient(row.origin,row,index)" type="md-create" />
              </template>
              <template slot-scope="{ row, index }" slot="TinyOrderID">
                {{row.TinyOrderID}}
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="item in row.Files" class="Filesbox">
                  <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <Icon type="ios-trash-outline"
                        :ref="row.ID"
                        @click.native="handleRemove(item,row)"></Icon>
                </p>
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button :disabled='row._disabled==false?false:true'
                          type="primary"
                          size="small"
                          icon="ios-cloud-upload"
                          @click="SeletUpload(row)">
                    传照
                  </Button>
                </div>
                <div class="setupload">
                  <Button :disabled='row._disabled==false?false:true'
                          type="primary"
                          icon="md-reverse-camera"
                          size="small"
                          @click="fromphotos(row)">
                    拍照
                  </Button>
                </div>
                <div class="setupload">
                  <Button :disabled='row._disabled==false?false:true'
                          v-if="row.iscx!=false"
                          type="primary"
                          size="small"
                          icon="md-checkmark"
                          @click="chaixiang(index,row)">
                    拆项
                  </Button>
                  <Button v-if="row.iscx==false"
                          :disabled='row._disabled==false?false:true'
                          type="primary"
                          size="small"
                          icon="ios-trash-outline"
                          @click="removechaixiang(index,row)">
                    删除
                  </Button>
                </div>
              </template>
            </Table>
            <div class="successbtn" style="margin-top:20px;">
              <Button type="primary"
                      icon="md-checkmark"
                      @click="clickshowchangebox"
                      :disabled="isdisabled==true?true:false">
                入库完成
              </Button>
              <Button type="warning"
                      icon="ios-alert-outline"
                      @click="isAbnormalclick"
                      :disabled="isdisabled==true?true:false">
                到货异常
              </Button>
            </div>
          </div>
          <!-- <div style="text-align: center;"  v-else>
            <img src="../../assets/img/null.png" alt="">
            <p>暂无搜索结果</p>
        </div> -->
        </div>
        <div>
          <History-Mount ref="History" v-if="reFresh" v-bind:OrderID='waybillinfo.OrderID' v-bind:EnterCode='waybillinfo.EnterCode' v-bind:Type='2' @uploadeCgDetail_new="uploadeCgDetail_new" :WaybillID='waybillinfo.ID' :ExcuteStatus='waybillinfo.ExcuteStatus' v-bind:chengeCarrier="chengeCarrier"></History-Mount>

          <!-- <History-Mount ref="History" v-if="reFresh" v-bind:OrderID='waybillinfo.OrderID' v-bind:EnterCode='waybillinfo.EnterCode' v-bind:Type='2' @uploadeCgDetail_new="uploadeCgDetail_new" :WaybillID='waybillinfo.ID' :ExcuteStatus='waybillinfo.ExcuteStatus'   v-bind:waybilltype="waybillinfo.Type" v-bind:waybillCarrierID="waybillinfo.CarrierID"  v-bind:waybillCode="waybillinfo.Code"></History-Mount>-->
        </div>
      </div>

    </div>
    <!-- 收支明细结束 -->
    <!-- 输送地列表与更改 开始-->
    <Modal v-model="setClientCode" title="选地地区" @on-cancel="cancel">
      <Select v-model="ClientCode" filterable :label-in-value="true" @on-change="changClientCode">
        <Option v-for="(item,index) in Conveyingplace2"
                :value="item.CorPlace"
                :key="item.ID">
          {{ item.Text }}
        </Option>
      </Select>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="primaryClientCode">确定</Button>
      </div>
    </Modal>
    <!-- 一键入箱 分摊重量合并 开始 -->
    <Modal v-model="showchangebox"
           title="一键入箱"
           :mask-closable='false'
           @on-visible-change="changeshowbox">
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>
      <p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;期：
        </label>
        <DatePicker type="date" style="width:80%" :options="options3" placeholder="请选择生成箱号的时间" :clearable='false' :value="saleDate" @on-change='changeData'></DatePicker>
      </p>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;号：
        </label>
        <Input v-model.trim="newboxcode"
               maxlength="30"
               placeholder="请输入箱号"
               style="width:80%"
               @on-blur='handleCreate1(newboxcode)' />
      </p>
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
            <Noting-Enter :key="timer" ref="NotingEntername" :Storagesarr="Storages" :Storehouselist="Storehouselist" :status='2' :EnterCode='waybillinfo.EnterCode' :orderID='waybillinfo.OrderID'></Noting-Enter>
          </div>
        </div>
      </div>
    </Modal>

    <!-- 未通知产品信息录入 结束 -->
    <!-- 确认入库 开始 -->
    <div v-if="WarehousingMsg==true">
      <Modal v-model="WarehousingMsg"
             title="分拣完成"
             @on-ok="ok_Warehousing(115)"
             @on-cancel="cancel_Warehousing">
        <div v-if="Nomatching.length!=0&&Warehousingstatus!=130">
          <h2>以下型号数量与通知应到数量不符，是否继续进行入库操作</h2>
          <ul>
            <li v-for="(item,index) in Nomatching" :key="index">
              <h2>{{item}}</h2>
            </li>
          </ul>
        </div>
        <div v-else>
          <span>是否确认分拣完成</span>
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
      <Input v-model="Summary" type="textarea" :rows="2" placeholder="备注" maxlength="100" show-word-limit />
      <div slot="footer">
        <Button @click="closeerror">取消</Button>
        <Button type="primary" @click="Abnormal_btn">确定</Button>
      </div>
    </Modal>
    <!-- 异常到货 结束-->
    <!-- 箱签打印 开始 -->
    <Modal title="箱签打印"
           width='80'
           v-model="showprintboxcode"
           :mask-closable="false">
      <Print-boxcode ref="printbox"></Print-boxcode>
      <div slot='footer'></div>
    </Modal>
    <!-- 箱签打印 结束 -->
    <!-- 历史到货 开始 -->
    <!-- <Drawer :width="70" v-model="historydata" @on-visible-change='changehistore'>
    <History-Mount v-if="reFresh" v-bind:OrderID='waybillinfo.OrderID' v-bind:enterCode='waybillinfo.EnterCode' v-bind:Type='2'></History-Mount>
  </Drawer> -->
    <!-- 历史到货 结束 -->
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
    <!-- 分摊重量 开始 -->
    <Modal v-model="showshare"
           title="分摊重量"
           @on-visible-change="changeshare">
      <div>
        <Input v-model="sharevalue" placeholder="请输入重量" />
      </div>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button @click="ok_share" type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 分摊重量 结束 -->
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
      <logg-ed ref="logged" v-if="showlogged" :key='loggdetime' :WaybillID='waybillinfo.ID'></logg-ed>
    </Modal>
    <!-- <div class="dync mount"></div> -->
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
import Vue from 'Vue';
import HistoryMount from './CgHistory';
import NotingEnter from "@/Pages/Cgenter/NotingEnter";
import Printboxcode from "@/Pages/Common/PrintBoxcode";
import AddCarrier from '../Subassembly/Add_Carrier'
import AddDriver from '../Subassembly/Add_Driver'
import AddCar from '../Subassembly/Add_Car'
import logged from '../Common/logged'
import Addcontacts from '../Subassembly/Add_contacts'
import Storagecharge from '@/Pages/Expenses/Storagecharge'
import { getWayParter,} from "../../api";
import {CgDetail, cgenter,GetSortingID, historyList, GetDriversCars,search_detail,TakeGoods,CgBoxesShow,CgEnterSeries,BoxcodeEnter,CgCarriers,CgDeleteFiles,Getclientdata,DriverAdd,TransportAdd,BoxcodeDelete,GetWaybillCurrentStatus,
IsRecordWarehouseFee} from "../../api/CgApi";
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint,
  FormPhoto,
  SeletUploadFile,
  FilesProcess
} from "@/js/browser.js";
let product_url = require("../../../static/pubilc.dev");
let lodash = require("lodash");
export default {
  name: "",
  components: {
    "History-Mount":HistoryMount,
    "Noting-Enter": NotingEnter,
    'Print-boxcode':Printboxcode,
    'Add-Carrier':AddCarrier,
    'Add-Driver':AddDriver,
    'Add-Car':AddCar,
    'logg-ed':logged,
    'Add-contacts': Addcontacts,
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
      oldcrriers:null,//保存初始化的的承运商
      saleDate:'',//箱号时间
      loggdetime:'',//操作日志时间
      showlogged:false,//操作日志
      Operateddisable:true,
      timerwaybill:null,//获取回退状态定时器
      Warehousingstatus:null,//入库状态
      isclick:true,//是都可以点击确认入库按钮
      AVGWeightsum:null,//推算重量
      issharevalue:true,//是否可以输入分摊重量
      boxcodetype:'2',
      adddisabled:false,
      showaddDriver:false,// 是否可以新增司机
      addDriverName:null,//新增司机姓名
      showaddCar:false,//是否可以新增车牌号
      addCarName:null,//新增车牌号名称
      showaddCarrier:false,//是否可以新增承运商
      showaddcontacts:false,//
      IsPersonal:false,//是否为个人承运商
      NullNotice:[],//无通知产品数组
      clientGrade:null,//客户等级
      sharevalue:null,//分摊的总重量
      showshare:false,//是否显示分摊重量
      Storehouselist:[],
      TypeArr:[
        {
          value:3,
          label:"快递"
        },
        {
          value:4,
          label:"国际快递"
        },
        {
          value:2,
          label:"送货上门"
        },
      ],
      loading:true,
      item:"",
      WarehouseID: sessionStorage.getItem("UserWareHouse"), //库房id
      showprintboxcode:false,//打印箱号的展示状态
      isdisabled:false,
      abnormal_enter: false,
      Storages:[],
      timer:'',
      TakeGoodsName: "我去提货",
      historydata: false, //历史到货的抽屉
      reFresh:false,
      historydetail: {
        //历史到货数据
        times: "", //时间，每次获取新的版本
        waybillLIst: [] //运单列表
      },
      WarehousingMsg: false,
      Nomatching: "", //数量不符合的型号
      Summary: '',//异常备注信息
      isAbnormal: false, //异常对话框
      searchkey: "", //筛选
      SelectRow: [], //多选
      applydata: [],
      detail_ID: "",
      CarrierList:[], //承运商列表
      CarArr: [], //车牌号
      CarArrName:'',//车牌号名称
      DriversArr: [], // 司机,
      DriversName:'',//司机名称
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
      waybillinfo: {}, //运单信息
      ProductList:[], //分拣列表
      wayBillID: this.$route.params.wayBillID, //运单号
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
          width: 30
          // fixed: 'right'
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center",
          width: 170
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
          key: "brand",
          align: "center",
          width: 120,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: vm.trim(params.row.Product.Manufacturer)
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Product.Manufacturer = event.target.value;
                  vm.ProductList[params.index] = params.row;
                  // for (var i = 0; i < vm.ProductList.length; i++) {
                  //   if (vm.ProductList[i].PID == params.row.ID) {
                  //      vm.ProductList[i].Product.Manufacturer =
                  //       event.target.value;
                  //   }
                  // }
                  vm.clicktest(params.row);
                },
                "on-blur"(event) {
                  if( event.target.value!=''){
                     params.row.Product.Manufacturer = event.target.value;
                     vm.ProductList[params.index] = params.row;
                  }else{
                    event.target.value = null,
                    params.row.Product.Manufacturer =null;
                    vm.ProductList[params.index] = params.row;
                  }
                  if(vm.SelectRow.length>0){
                    for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                    if ( params.row.SortingID == vm.SelectRow[i].SortingID) {
                        vm.SelectRow[i] =  params.row;
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
          slot: "Arrival",
          align: "center",
          width: 58
        },
        {
          title: "已到",
          slot: "Already",
          align: "center",
          width: 58
        },
        {
          title: "剩余",
          slot: "Surplus",
          align: "center",
          width: 58
        },
        {
          title: "批号",
          key: "batch",
          align: "center",
          width: 100,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: vm.trim(params.row.DateCode)
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.DateCode = event.target.value;
                  vm.ProductList[params.index] = params.row;
                  vm.clicktest(params.row);
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
              h("span", {}, "本次到货")
            ]);
          },
          title: "本次到货",
          key: "Quantity",
          align: "center",
          width: 90,
          render: (h, params) => {
            var vm = this;
            var newCurrentQuantity = 0;
            var disabledstatus = false;
            if(Number(params.row.CurrentQuantity)<0){
                    newCurrentQuantity=0
              }else{
                newCurrentQuantity=params.row.CurrentQuantity
              }
            return h("Input", {
              props: {
                //将单元格的值给Input
                value:newCurrentQuantity,
              },
              on: {
                "on-blur"(event) {
                  if (event.target.value != "") {
                    var reg = /^[0-9]*$/;
                    // var reg = /^\d+(\.\d{0,2})?$/;
                    if (reg.test(event.target.value) == true&&event.target.value!=0) {
                      params.row.CurrentQuantity = Number(event.target.value);
                      // if(params.row.iscx==false){
                      //     params.row.LeftQuantity = event.target.value;
                      //     params.row.ArrivedQuantity = 0;
                      //     params.row.Quantity = event.target.value;
                      // }
                      vm.ProductList[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入正整数");
                      (event.target.value = null),
                        (params.row.CurrentQuantity =null);
                      vm.ProductList[params.index] = params.row;
                    }
                    // vm.clicktest(params.row);
                  }else{
                    event.target.value = null,
                    params.row.CurrentQuantity =null;
                    vm.ProductList[params.index] = params.row;
                    // vm.clicktest(params.row);
                  }
                  if(vm.SelectRow.length>0){
                    for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                    if ( params.row.SortingID == vm.SelectRow[i].SortingID) {
                        vm.SelectRow[i] =  params.row;
                      }
                    }
                  }
                },
                'on-change'(){

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
          width: 130
        },
        {
          title:"小订单号",
          slot:"TinyOrderID",
          align: "center",
          width: 150
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
                  h("span", {}, "箱号")
                ]);
              },
              width: 180,
              key: 'Boxing_code',
               render: (h, params) => {
            var vm = this;
                return h("Input", {
                  props: {
                    //将单元格的值给Input
                    value: params.row.BoxCode,
                    disabled:true,
                  },
            });
          }
          },
        {
          title: "体积(m³)",
          key: "Volume",
          align: "center",
          width: 90,
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
                    vm.ProductList[params.index] = params.row;
                  } else {
                    vm.$Message.error("只能输入数字,包括两位数的小数点");
                    event.target.value = null;
                    params.row.Volume = null;
                    vm.ProductList[params.index] = params.row;
                  }
                  vm.clicktest(params.row);
                },
                "on-blur"() {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  var reg = /^\d+(\.\d{0,2})?$/;
                  // reg.test(event.target.value);
                  if (reg.test(event.target.value) == true) {
                    params.row.Volume = event.target.value;
                    vm.ProductList[params.index] = params.row;
                  } else {
                    vm.$Message.error("只能输入数字,包括两位数的小数点");
                    event.target.value = null;
                    params.row.Volume = null;
                    vm.ProductList[params.index] = params.row;
                  }
                  vm.clicktest(params.row);
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
              h("span", {}, "毛重(Kg)")
            ]);
          },
          key: "GrossWeight",
          align: "center",
          width: 90,
          render: (h, params) => {
            var vm = this;
           var  newWeight=null
            if(params.row.Weight==0){
              newWeight=null
              vm.ProductList[params.index].Weight=newWeight
            }else{
               newWeight=params.row.Weight
               vm.ProductList[params.index].Weight=newWeight
            }
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: newWeight
              },
              on: {
                "on-change"(event) {},
                "on-blur"(event) {
                  // var reg = /^\d+(\.\d{0,4})?$/;
                  // var reg = /^[1-9]\d*(\.\d+)?$/
                  // var reg = /^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/
                  var reg = /^\d+(\.\d{0,5})?$/;
                  var newtarget = vm.trim(event.target.value);
                  if (newtarget != null||newtarget != '') {
                    if (reg.test(newtarget) == true&&newtarget!=0) {
                      params.row.Weight = newtarget;
                      vm.ProductList[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入大于零的数字,且保留小数点后五位");
                      params.row.Weight = null;
                      event.target.value =null;
                      vm.ProductList[params.index] = params.row;
                    }
                  }else{
                     vm.$Message.error("请输入毛重");
                      params.row.Weight = null;
                      event.target.value =null;
                     vm.ProductList[params.index] = params.row;
                  }
                  if(vm.SelectRow.length>0){
                     for(var i=0,len=vm.SelectRow.length;i<len;i++){
                      if(vm.SelectRow[i].SortingID==params.row.SortingID){
                        vm.SelectRow[i]=params.row
                      }
                    }
                   }
                 },
                "on-enter": event => {
                  var newtarget = vm.trim(event.target.value);
                  // var reg = /^[+]{0,1}(\d+)$|^[+]{0,1}(\d+\.\d+)$/
                  var reg = /^\d+(\.\d{0,5})?$/;
                  if (reg.test(newtarget) == true&&newtarget!=0) {
                    params.row.Weight = newtarget;
                    vm.ProductList[params.index] = params.row;
                    var Input = params.row.Input;
                    var StandardProducts = params.row.Product;
                    var data2 = {
                      Quantity: params.row.CurrentQuantity, //数量
                      inputsID: params.row.SortingID, //id
                      Catalog: '', //品名
                      PartNumber: StandardProducts.PartNumber, //型号
                      Manufacturer: StandardProducts.Manufacturer, //品牌
                      Packing: StandardProducts.Packing, //包装
                      PackageCase: StandardProducts.PackageCase, //封装
                      origin: params.row.origintext,//产地
                      SourceDes:this.waybillinfo.SourceDes,//业务
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
                    // TemplatePrint(data);
                  } else {
                    vm.$Message.error("只能输入大于零的数字,且保留小数点后五位");
                    params.row.Weight =null;
                    event.target.value =null;
                    vm.ProductList[params.index] = params.row;
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
          width: 170
        },
        {
          title: "异常原因",
          key: "imglist",
          align: "center",
          width: 100,
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Summary
              },
              on: {
                "on-change"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  params.row.Summary = event.target.value;
                  vm.ProductList[params.index] = params.row;
                },
                "on-blur"(event) {
                  if (vm.SelectRow.length > 0) {
                    for (var i = 0, lens = vm.SelectRow.length; i < lens; i++) {
                      if (vm.SelectRow[i].SortingID == params.row.SortingID) {
                        vm.SelectRow[i] = params.row;
                      }
                    }
                  }
                }
              }
            });
          }
        },
        {
          title: "操作",
          slot: "operation",
          align: "center",
          width: 200, //220
          fixed: "right"
        }
      ],
      Conveyingplace: [], //输送地列表
      Conveyingplace2: [],
      chengevalue: {
        inputval: "",
        value: "",
        type: ""
      },
      setClientCode: false, //显示输送地选择模态框
      ClientCode: "", //默认输送地
      boxingarr: [],
      showchangebox: false, //显示一键更改箱号的的弹窗
      newboxcode: null, // 一键更改箱号后的新箱号
      newboxcodeback:null,
      oldboxcode:null,//保存旧箱号
      printurl: product_url.pfwms,
      printlist: [], //打印列表
      orderID:null,
      Entercode:null,
      GroupsData: [], //拆箱分组,
      chengeCarrier: { //历史到货修改承运商与运单号所需要的值
        waybilltype: null,//快递类型，
        CarrierID:null,//承运商
        code:null,//运单号
      }
    };
  },
  created() {
    window["PhotoUploaded"] = this.changed;
    // console.log(this.wayBillID);
    this.CgDetail_new(this.$route.params.wayBillID);
    // this.Carriers();
   
  },
  computed: {
    Budgetdetail() {
      // console.log(this.$store.state.Budget.Budgetdetail)
      return this.$store.state.common.Budgetdetail;
    }, 
  },
  watch: {
    orderID:function(){
      if(this.waybillinfo.OrderID!=null){
        this.reFresh=true
      }
      return this.waybillinfo.OrderID
    }
  },
  mounted() {
    this.WayParterdata();
  },
  methods: {
    setboxsplit(str) {
      //去除前后空格
      if(str){
         return str.split("]")[1]
      }
    },
    clicktest: lodash.throttle(function(paramsrow) {
      for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
        if (paramsrow.SortingID == this.SelectRow[i].SortingID) {
          this.SelectRow[i] = paramsrow;
        }
      }
    }, 300),
    // 即使更新选中
    setSelectrow(paramsrow){
      for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
        if (paramsrow.SortingID == this.SelectRow[i].SortingID) {
          this.SelectRow[i] = paramsrow;
        }
      }
    },
    search_pro() {
      //筛选按钮
      this.loading = true;
      search_detail(this.waybillinfo.ID, this.searchkey).then(res => {
        this.ProductList = res.Notice;
        this.loading = false;
      });
    },
    GetDriversCars(key) {
      //根据送货上门承运商获取司机与车牌号
      GetDriversCars(key).then(res => {
        this.CarArr=[]
        var data=res.Transports
        if(data.length>0){
          var item={}
            for(var i=0;i<data.length;i++){
              if(data[i].CarNumber1!=null&&data[i].CarNumber2!=null){
                 item ={
                    value:data[i].CarNumber1+'\xa0\xa0'+data[i].CarNumber2
                  }
              }else{
                 item ={
                    value:data[i].CarNumber1
                  }
              }
             
              this.CarArr.push(item)
            }
        }else{
          this.CarArr=[]
        }
        this.DriversArr = res.Drivers; // 司机
      });
    },
    changeacrrier(value) { //改变承运商的时候触发
        if(value!=undefined){
          if(this.waybillinfo.Type==1&&value.value=='Personal'){
            this.IsPersonal=true
            this.waybillinfo.CarNumber1=this.CarArrName;
            this.waybillinfo.Driver=this.DriversName;
          }else{
            if(this.waybillinfo.Type==1){
              this.IsPersonal=false
              this.waybillinfo.CarNumber1='';
              this.waybillinfo.Driver='';
              this.GetDriversCars(value.value);
            }else{
              this.IsPersonal=false
              this.waybillinfo.CarNumber1='';
              this.waybillinfo.Driver='';
            }
            this.waybillinfo.CarrierName=value.label
            }
            this.oldcrriers=value.value
      }
    },
     handleCreate1(val) {  //箱号添加
    //  console.log(val)
     if(!val==false&&val!='WL'){
        if(this.oldboxcode!=null&&this.oldboxcode!=''){
          var data={
            boxCode:this.oldboxcode,
          }             
           var data2={
              enterCode:this.waybillinfo.EnterCode, // 统一使用当前运单的entercode
              code:val, // 箱号
              date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
              adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
            }
          BoxcodeDelete(data).then(res=>{
            var data={
              enterCode:this.waybillinfo.EnterCode, // 统一使用当前运单的entercode
              code:val, // 箱号
              date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
              adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
            }
            BoxcodeEnter(data).then(res=>{
              if(res.success==false){
              this.$Message.error(res.data)
              this.newboxcode=null
              this.newboxcodeback=null
              this.oldboxcode=null
              this.isclick=true
              }else{
                this.oldboxcode=res.boxCode
                this.newboxcode=this.setboxsplit(res.boxCode)
                this.newboxcodeback=res.boxCode
                this.isclick=false
              }
             
            })
          })
       }else{
          var data={
                enterCode:this.waybillinfo.EnterCode, // 统一使用当前运单的entercode
                code:val, // 箱号
                date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
                adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
            }
            BoxcodeEnter(data).then(res=>{
              if(res.success==false){
              this.$Message.error(res.data)
              this.newboxcode=null
              this.newboxcodeback=null
              this.oldboxcode=null
              this.isclick=true
              }else{
                this.oldboxcode=res.boxCode
                this.newboxcode=this.setboxsplit(res.boxCode)
                this.newboxcodeback=res.boxCode
                this.isclick=false
              }
            })
        }
      }else{
         this.$Message.error('请输入正确的箱号')
         this.newboxcode=null
      }
      
    },
    handleCreate2(val) {
      //添加司机
      this.DriversArr.push({
        ID: val,
        Name: val
      });
    },
    handleCreate3(val) {
      this.CarArr.push({
        ID: val,
        Name: val
      });
    },
    changePartNumber(event, rowid, index, row) {
      //改变型号
      if (this.ProductList[index].InputID == rowid) {
        this.ProductList[index].Product.PartNumber = event.target.value;
        this.clicktest(this.ProductList[index]);
        for (var i = 0; i < this.ProductList.length; i++) {
          if (this.ProductList[i].PID == row.ID) {
            this.ProductList[i].Product.PartNumber = event.target.value;
          }
        }
      }
    },
    trim(str) {
      //去除前后空格
      if(str){
         return str.replace(/(^\s*)|(\s*$)/g, "");
      }
    },
    handleRemovelist(row, file) {
      var arr =this.ProductList
      for (var j = 0; j < arr.length; j++) {
        //删除指定下标 的元素
        if (arr[j].ID == row.ID) {
          arr[j].Files.splice(file, 1);
        }
      }
    },
     //删除上传文件按
    handleRemove(file,type) {
       var data={
          id:file.ID
        }
       CgDeleteFiles(data).then(res=>{
          if(res.Success==true){
            this.Removebackfun(file,type)
            this.$Message.success('删除成功')
          }else{
            this.$Message.error('删除失败')
          }
      })
    },
    //删除成功后，删除本地数据
    Removebackfun(file,type){
     if(type=="Waybill"){
         this.waybillinfo.Files.splice(this.waybillinfo.Files.indexOf(file), 1);
      }else{
        var arr =this.ProductList
        for (var j = 0; j < arr.length; j++) {
          //删除指定下标 的元素
          if (arr[j].ID == type.ID) {
            arr[j].Files.splice(file, 1);
          }
        }
      }
    },
    CgBoxesShow() {
      //获取可用箱号
      var houseid = sessionStorage.getItem("UserWareHouse");
      var data = {
        // whCode: sessionStorage.getItem("UserWareHouse"), //库房标识（库房编号）
        enterCode: this.waybillinfo.EnterCode, //入仓号
        date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
        // orderID:this.waybillinfo.OrderID
      };
      CgBoxesShow(data).then(res => {
        if (res.length > 0) {
          this.boxingarr = res;
        }else{
          this.boxingarr=[]
        }
      });
    },
    showBudget(type, Budget, Source) {
      //收支明细展开
      this.$store.dispatch("setBudget", true);
      if (type == "meet") {
        var namemeet = "";
        this.$router.push({
          path: "/CgProcessed/Declare/Budget",
          query: {
            webillID: this.waybillinfo.ID,
            OrderID: this.waybillinfo.OrderID,
            type: Budget,
            otype: "in",
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
      this.$router.go(-1);
      this.$store.dispatch("setBudget", false);
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      this.SelectRow = value;
    },
    cancel() {
      // this.$Message.info("");
      this.showphoto = false;
      this.setClientCode = false;
       this.abnormal_enter = false;
       this.showshare=false;
       this.showchangebox=false,
       this.showaddCarrier=false
       this.showaddDriver=false
      this.showaddCar=false
      this.oldboxcode=null
      if(this.showchangebox==false){
        this.BoxcodeDeletefun()
      }
    if(this.abnormal_enter==false){
        this.Storages.forEach(item => {
            if (item.BoxCode!=null||item.BoxCode!='') {
                var data={
                  boxCode:item.BoxCode,
                }
                BoxcodeDelete(data).then(res=>{
                })
            }
          });
     }
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
      GetSortingID().then(res => {
        var chaixiangdata = {
          AVGWeight:row.AVGWeight,
          PID:row.ID,
          iscx: false,
          ID: "CX" + index + new Date().getTime(),
          NoticeID: row.NoticeID,
          Product: {
            PartNumber: row.Product.PartNumber,
            Manufacturer: row.Product.Manufacturer,
            PackageCase: row.Product.PackageCase,
            Packaging: row.Product.Packaging
          },
          WaybillID: row.WaybillID,
          InputID: row.InputID,
          DateCode: row.DateCode,
          Quantity: row.Quantity,
          ArrivedQuantity: row.ArrivedQuantity,
          LeftQuantity:row.LeftQuantity,
          CurrentQuantity:null,
          Conditions: row.Conditions,
          Source: row.Source,
          Weight: null,
          NetWeight: row.NetWeight,
          Volume: row.Volume,
          BoxCode: null,
          ShelveID: row.ShelveID,
          Type: row.Type,
          origintext: row.origintext,
          originID: row.originID,
          originDes: row.originDes,
          origin:row.origin,
          Summary: row.Summary,
          SortingID: res,
          Files: [],
          Sortings: [],
          _disabled: false,
          TinyOrderID:row.TinyOrderID
        };
        this.ProductList.splice(index + 1, 0, chaixiangdata);
        this.$refs.selection.selectAll(false);
        this.SelectRow=[]
        this.ProductList[index].CurrentQuantity=null
        this.ProductList[index].Weight=null
      });
    },
    removechaixiang(index) {
      //移除拆项
      this.ProductList.splice(index, 1);
    },
    // 初始化重构数据
    CgDetail_new(id) {
       clearInterval(this.timerwaybill)
       this.timerwaybill = null
      if (id != undefined) {
        var EnterCodes=null
        CgDetail(id).then(res => {
          this.Getclientdata(res.Waybill.EnterCode)
          this.orderID=res.Waybill.OrderID
          this.waybillinfo = res.Waybill;
          this.ProductList = res.Notice;
          this.oldcrriers=this.waybillinfo.CarrierID
          this.loading = false;
          var newdata = null;

          //this.chengeCarrier={ //历史到货修改承运商与运单号所需要的值
          //  waybilltype: res.Waybill.Type,//快递类型，
          //  CarrierID: res.Waybill.CarrierID,//承运商
          //  code: res.Waybill.CarrierID//运单号
          // }

          this.chengeCarrier.waybilltype = res.Waybill.Type;//快递类型，
          this.chengeCarrier.CarrierID = res.Waybill.CarrierID;//快递类型，
          this.chengeCarrier.code = res.Waybill.CarrierID;//快递类型，

          if(res.Waybill.ExcuteStatus==120){
            this.isdisabled=true;
            //如果是已处理订单，则时间为最后一次操作时间
            newdata = this.moment(res.Waybill.ModifyDate).format('YYYY-MM-DD')
          }else{
            // this.isdisabled=false;
            if(this.waybillinfo.Type==1&&this.waybillinfo.LoadingExcuteStatus==100){
                this.isdisabled=true;
                this.Operateddisable=false
            }else{
                this.isdisabled=false;
              this.Operateddisable = true
            }
            
            // 如果是未处理，则为系统当前时间
            newdata = this.moment(new Date().getTime()).format('YYYY-MM-DD')
          }
         
          this.Carriers(res.Waybill.Type)
          if(this.waybillinfo.Type==1&&res.Waybill.CarrierID=='Personal'){
            this.IsPersonal=true
          }else{
             this.GetDriversCars(res.Waybill.CarrierID);
             this.IsPersonal=false
          }
            this.CarArrName=res.Waybill.CarNumber1
            this.DriversName = res.Waybill.Driver


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
                  this.IsRecordWarehouseFee(this.waybillinfo.OrderID, this.waybillinfo.ID)
                }

              } else if (FirstTempDate <= LsEndDate) {
                var duration = ((ModifyDate - LsEndDate) / (1 * 24 * 60 * 60 * 1000));
                console.log(duration + "在租期内，且出库日期在租期外")
                if (duration >= 1) { //暂存超过五天
                  this.timedifference = duration
                  this.showStoragecharge = true
                  this.IsRecordWarehouseFee(this.waybillinfo.OrderID, this.waybillinfo.ID)
                }
              } else if (ModifyDate <= LsEndDate) {
                this.showStoragecharge = false
                this.waybillinfo.IsClientLs=true
              }
            } else {
              var duration = ((ModifyDate - FirstTempDate) / (1 * 24 * 60 * 60 * 1000)) + 1;
              console.log(duration + "未租赁，应免租五天")
              if (duration>5) { //暂存超过五天
                this.timedifference = duration - 5
                this.showStoragecharge = true 
                this.IsRecordWarehouseFee(this.waybillinfo.OrderID, this.waybillinfo.ID)
              }
            }
          } else {
            if (res.Waybill.ExcuteStatus != 120) {
              this.getwaybillstatus(this.waybillinfo.ID)// 订单回退状态
            }
          }
        });
       
      }

      
    },
    //更改到货方式
    changewaybillType(value){
       this.Carriers(value)
       if(this.CarrierList.length<=0){
          this.waybillinfo.CarrierID=''
      }else{
          this.waybillinfo.CarrierID=this.oldcrriers
      }
    },
    fromphotos(type) {
      if (type == "Waybill") {
        var data = {
          SessionID: this.waybillinfo.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            Type:8000
          }
        };
        FormPhoto(data);
      } else {
        var data = {
          SessionID: type.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            NoticeID:type.NoticeID,
            InputID:type.InputID,
            Type:8000
          }
        };
        FormPhoto(data);
      }
    },
    changed(message) {
      //后台调用winfrom 拍照的方法
      this.testfunction(message); //前台拿到返回值处理数据
    },
    testfunction(message) {  //前台处理数据的方法
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      if (imgdata.SessionID == this.waybillinfo.ID) {
        this.waybillinfo.Files.push(newfile);
      } else {
        for (var i = 0; i < this.ProductList.length; i++) {
          if (this.ProductList[i].ID == imgdata.SessionID) {
            this.ProductList[i].Files.push(newfile);
          }
        }
      }
    },
    SeletUpload(type) {// 传照
      if (type == "Waybill") {
        var data = {
          SessionID: this.waybillinfo.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.waybillinfo.ID,
            WsOrderID:this.waybillinfo.OrderID,
            Type:8000
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
            NoticeID:type.NoticeID,
            InputID:type.InputID,
            Type:8000
          }
        };
      
        SeletUploadFile(data);
      }
    },
    Carriers(type) { //承运商列表
      CgCarriers(type,this.WarehouseID,100).then(res => {
        this.CarrierList = res;
      });
    },
    WayParterdata() {
      //输送地列表
      getWayParter().then(res => {
       
        this.Conveyingplace = res.obj;
      });
    },
    clickClient(value, type, index) {
      //显示更改输送地与原产地的方法
      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = value;
    },
    changClientCode(value) {
      //地址改变的时候,保留改变后的地址
      this.chengevalue.inputval = value;
    },
     primaryClientCode() {
      //确认更改地址
      if (this.chengevalue.type == "ClientCode") {
        this.waybillinfo.ConsignorPlace = this.chengevalue.inputval.value;
        this.waybillinfo.ConsignorPlaceText = this.chengevalue.inputval.label;
      } else {
        this.chengevalue.type.originDes = this.chengevalue.inputval.label;
        // this.ProductList[2].origintext = this.chengevalue.inputval.label
        for (var j = 0, l = this.ProductList.length; j < l; j++) {
            if (this.chengevalue.type.SortingID == this.ProductList[j].SortingID) {
              this.ProductList[j].origin = this.chengevalue.inputval.value;
              this.ProductList[j].origintext = this.chengevalue.inputval.label;
            }
            this.ProductList[j]._checked=false
         }
       
         if(this.SelectRow.length>0){
              for(var i=0,len=this.SelectRow.length;i<len;i++){
                for(var j = 0, l = this.ProductList.length; j < l; j++){
                  if(this.SelectRow[i].SortingID==this.ProductList[j].SortingID){
                      this.ProductList[j]._checked=true
                   }
                   if(this.chengevalue.type.SortingID==this.SelectRow[i].SortingID){
                      this.SelectRow[i].origin = this.chengevalue.inputval.value;
                      this.SelectRow[i].origintext = this.chengevalue.inputval.label;
                   }
                }
                }
            }
      }
      this.setClientCode = false;
    },
    clickshowchangebox() {
      //点击一键入箱，显示入箱弹窗
      if (this.SelectRow.length == 0) {
        this.$Message.error("至少选择一条产品");
      } else {
        // this.showchangebox = true;
        for(var i=0;i<this.SelectRow.length;i++){
          if((this.SelectRow[i].CurrentQuantity!=null)&&(this.SelectRow[i].Product.Manufacturer!=null)){
            if(i==this.SelectRow.length-1){
              if(this.waybillinfo.Type==1){
                  if (this.waybillinfo.CarrierID == '' ) {
                      this.$Message.error('请先选择承运商')
                    } else if (this.waybillinfo.Driver == '') {
                      this.$Message.error('请选择司机')
                    } else if (this.waybillinfo.CarNumber1 == '') {
                      this.$Message.error('请选择车牌号')
                    } else{
                      this.Warehousingstatus=115
                      if(this.Warehousingstatus==115){
                        this.finish_btn2()
                        //  this.showboxmax()
                      }
                    }
              }else{
                this.finish_btn2()
                this.Warehousingstatus=115
              }
            }
          }else{
             this.$Message.error("品牌，本次到货不能为空");
             break
          }
        
        }

      }
    },
    showboxmax(){
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
                  if(nullarr.length!=0&&widtharr.length!=0){
                    this.$Message.error("勾选型号重量有误");
                  }else{
                    if(nullarr.length>0){
                        this.issharevalue=true
                    }else{
                      this.issharevalue=false;
                    }
                    this.sharevalue=null
                    this.newboxcodeback=null
                    this.newboxcode=null
                    this.boxcodetype='1'
                    // this.CgBoxesShow();
                    this.showchangebox = true;
                    const myDate = new Date();
                    const year = myDate.getFullYear(); // 获取当前年份
                    const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
                    const day = myDate.getDate(); // 获取当前日（1-31）
                    // 日期格式：2019/07/29 - 2019/07/29
                    this.saleDate = `${year}/${month}/${day}`;
                  }
            }
          }
      }
     
    },
    // 判断是否都有重量

    //箱号发生变化的时候
    changeshowbox(value){
      if(value==true){
        this.AVGWeightsum=null
        this.SelectRow.forEach(element => {
          this.AVGWeightsum+=element.AVGWeight*element.CurrentQuantity
        });
      }
    },
    // 删除选定的箱号
    BoxcodeDeletefun(){
      // alert(this.newboxcode)
      if(this.newboxcodeback!=null){
        var data={
        boxCode:this.newboxcodeback,
        // date:this.saleDate
        }
        BoxcodeDelete(data).then(res=>{
         
        })
      }
      
    },
    oldBoxDelete(boxcode){ //删除旧箱号
     var data={
        boxCode:boxcode,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
        
      })
    },
     //确认更改箱号
    ok_changebox() {
      // console.log(this.newboxcode)
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
              if(this.isclick==false){
                 this.finish_btn()
                  this.showchangebox=false
              }
              
          }
        }
        
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
            Quantity: arr[i].CurrentQuantity, //数量
            inputsID: arr[i].SortingID, //id
            Catalog: '', //品名
            PartNumber: arr[i].Product.PartNumber, //型号
            Manufacturer: arr[i].Product.Manufacturer, //品牌
            Packing: arr[i].Product.Packing, //包装
            PackageCase: arr[i].Product.PackageCase, //封装
            origin: arr[i].origintext, //产地
            SourceDes:this.waybillinfo.SourceDes,//业务
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
    finish_btn(){
       if(this.waybillinfo.Type==1){
          if (this.waybillinfo.CarrierID == '' ) {
              this.$Message.error('请先选择承运商')
            } else if (this.waybillinfo.Driver == '') {
              this.$Message.error('请选择司机')
            } else if (this.waybillinfo.CarNumber1 == '') {
              this.$Message.error('请选择车牌号')
            } else{
              // this.finish_btn2()
               this.WarehousingMsg = true;
            }
      }else{
        // this.finish_btn2()
         this.WarehousingMsg = true;
      }
    },
    finish_btn2() {
      console.log(this.SelectRow)
      if (this.SelectRow.length <= 0) {
        this.$Message.error("请选择要操作的产品项");
      } else {
        bbq: for (var k = 0, len = this.SelectRow.length; k < len; k++) {
          // if (this.SelectRow[k].isenter == true) {
          //   this.$Message.error("无通知产品请按照异常到货处理");
          //   this.WarehousingMsg = false;
          //   break;
          // } else
          
          if (this.SelectRow[k].originID == "") {
            this.$Message.error("请选择原产地");
            this.WarehousingMsg = false;
            break;
          } else {
            if ( this.SelectRow[k].CurrentQuantity == null) {
              if(this.SelectRow[k].Weight ==null){
                this.$Message.error("请输入必填项目");
                this.WarehousingMsg = false;
                break;
              }
              this.$Message.error("请输入必填项目");
              this.WarehousingMsg = false;
              break;
            } else {
              var SelectRowK=k
              function sum(arr) {
                return eval(arr.join("+"));
              }
              var arr = this.SelectRow;
              this.Nomatching=[];
              var Nomatching = [];
              var map = {},
                dest = [];
                this.GroupsData=[];
              for (var i = 0; i < arr.length; i++) {
                var ai = arr[i];
                if (!map[ai.NoticeID]) {
                  var arrs={
                    PartNumber:ai.Product.PartNumber,
                    Manufacturer:ai.Product.Manufacturer,
                    origin:ai.origin,
                    PID:ai.PID
                  }
                  this.GroupsData.push(arrs)
                  dest.push({
                    ID:ai.ID,
                    Manufacturer:ai.Product.Manufacturer,
                    origin:ai.origin,
                    name: ai.Product.PartNumber,
                    Quantity: ai.LeftQuantity,
                    NoticeID: ai.NoticeID,
                    data: [Number(ai.CurrentQuantity)],
                    arrdata:[arrs]
                  });
                  // dest.arrdata.push(arrs)
                  map[ai.NoticeID] = ai.NoticeID;
                } else {
                  for (var j = 0; j < dest.length; j++) {
                    var dj = dest[j];
                    if (dj.NoticeID == ai.NoticeID) {
                      var arrs={
                          PartNumber:ai.Product.PartNumber,
                          Manufacturer:ai.Product.Manufacturer,
                          origin:ai.origin,
                          PID:ai.PID,
                          ID:ai.ID
                        }
                      dj.arrdata.push(arrs)
                      dj.data.push(Number(ai.CurrentQuantity));
                      this.GroupsData.push(arrs)
                      break;
                    }
                  }
                }
              }


              var that = this;
              // that.GroupsData=dest
              for (var i = 0; i < dest.length; i++) {
                var item = dest[i];
                var total = sum(item.data);
                if (item.Quantity == "") {
                  // this.$Message.error("无通知产品请按照异常到货处理");
                   this.$Message.error({
                                  content: "无通知产品请按照异常到货处理",
                                  duration: 3
                              })
                  that.WarehousingMsg = false;
                  break bbq;
                } 
                else if (item.Quantity < total) {
                  this.$Message.error({
                                  content: "本次到货数量不能大于剩余数量,请按照异常情况处理",
                                  duration: 3
                              })
                  that.WarehousingMsg = false;
                  break bbq;
                } 
                else {
                  if (item.Quantity == 0) {
                    this.$Message.error("剩余数量为0，不需要提交");
                    that.WarehousingMsg = false;
                    break bbq;
                  } else {
                    if (item.Quantity > total||item.Quantity <total) {
                      Nomatching.push(item.name);
                      that.Nomatching = Nomatching;
                      var info=item.arrdata
                    } 
                    if(SelectRowK==this.SelectRow.length-1&&i==dest.length-1){
                      // console.log(this.SelectRow)
                      this.testrepeat(SelectRowK,dest)//检查拆项验证
                      //  this.showboxmax()
                    }
                   }
                }
               }
              
            }
          }
        }
      }
    },

    //检查型号品牌原产地是否一致 
    testrepeat(SelectRowK,dest){
      console.log(dest)
  aaa:for(var k=0;k<dest.length;k++){
          var arr=dest[k].arrdata
          arr.sort();
          if(arr.length>=2){
            for (var i = 0; i < arr.length - 1; i++) {
              for (var j = i + 1; j < arr.length; j++) {
                if (arr[i].Manufacturer === arr[j].Manufacturer && arr[i].origin === arr[j].origin) {
                  this.$Message.error({
                    content: "品牌，产地一致，不允许进行拆项处理",
                    duration: 3
                  })
                  break aaa;
                }else{
                  if(k==dest.length-1){
                    this.showboxmax()
                  }
                }
              }
            }
          }else{
            console.log(k)
            if(k==dest.length-1){
               this.showboxmax()
            }
          }
      }
    },
    ok_Warehousing(type,datatype) {
      if(this.issharevalue==true){
        this.ok_share()
      }
      var Summary_txt = "";
      this.WarehousingMsg = false;
      this.isdisabled=true
      //点击确定按钮，进行入库操作
      var deatilarr = null
      if(datatype=='enter'){
        deatilarr=this.NullNotice
      }else{
        deatilarr=this.SelectRow
      }
      var newarr = [];
      var boxcode=null
      for (var i = 0, len = deatilarr.length; i < len; i++) {
        if (deatilarr[i].Quantity != "" || deatilarr[i].Quantity != null) {
           if (datatype=='enter') {
                boxcode=deatilarr[i].BoxCode
                // console.log(deatilarr[i].BoxCode)
            }else{
              // Summary_txt=deatilarr[i].Summary
              boxcode=this.newboxcodeback
            }
          if(boxcode==null||boxcode==''){
              this.$Message.error({
                content: "请选择箱号",
                duration: 3
            })
            break;
          }else{
            var IsSplit=false
            if(datatype!='enter'&&deatilarr[i].iscx==false){
                IsSplit=true
            }
            var data = {
            NoticeID: deatilarr[i].NoticeID,
            SortingID: deatilarr[i].SortingID,
            StorageID: "",
            InputID: deatilarr[i].InputID,
            BoxCode: boxcode,
            Quantity: deatilarr[i].CurrentQuantity,
            Weight: deatilarr[i].Weight,
            NetWeight: deatilarr[i].NetWeight,
            Volume: deatilarr[i].Volume,
            ShelveID: deatilarr[i].ShelveID,
            DateCode: deatilarr[i].DateCode,
            Origin: deatilarr[i].origin,
            Summary:deatilarr[i].Summary,
            Product: {
              PartNumber: deatilarr[i].Product.PartNumber,
              Manufacturer: deatilarr[i].Product.Manufacturer,
              PackageCase: deatilarr[i].Product.PackageCase,
              Packaging: deatilarr[i].Product.Packaging
            },
            Files:deatilarr[i].Files,
            IsSplit:IsSplit
          };
          // console.log(data)
          newarr.push(data);
          }
          
        }
      }
      console.log(newarr)
      var newcode=null;
      if(this.waybillinfo.Code==''||this.waybillinfo.Code==null){
          newcode=null
      }else{
          newcode=this.waybillinfo.Code
      }
      var obj = {
        Waybill: {
          ID: this.waybillinfo.ID,
          EnterCode: this.waybillinfo.EnterCode,
          ExcuteStatus: this.Warehousingstatus, //异常130  部分入库115
          Type: this.waybillinfo.Type,
          Source: this.waybillinfo.Source,
          OrderID: this.waybillinfo.OrderID,
          NoticeType: this.waybillinfo.NoticeType,
          CarrierID: this.waybillinfo.CarrierID,
          ConsignorID: this.waybillinfo.ConsignorID,
          ConsignorPlace: this.waybillinfo.ConsignorPlace,
          TransferID: this.waybillinfo.TransferID, //代转运需要该值,用于发出库通知
          Summary: null,
          Driver: this.waybillinfo.Driver,
          CarNumber1: this.waybillinfo.CarNumber1,
          Files:this.waybillinfo.Files,
          LoadingExcuteStatus: this.waybillinfo.LoadingExcuteStatus,
          Code:newcode,
          Supplier:this.waybillinfo.Supplier
        },
        Sortings: newarr,
        AdminID: sessionStorage.getItem("userID")
       };
      this.setWarehousing(obj);
      // }
    },
    cancel_Warehousing() {
      //点击取消按钮，取消入库
      this.WarehousingMsg = false;
      this.oldboxcode=null
      this.BoxcodeDeletefun()
    },
    setWarehousing(data) {
      cgenter(data).then(res => {
        this.Warehousingstatus=null;
        if (res.Success == true) {
          this.$Message.success("入库完成");
          if(this.isAbnormal==true){
            this.isAbnormal=false;
            this.Summary=''
            this.Nomatching=[]
          }
          var that = this;
          that.loading=true
          setTimeout(function() {
            that.CgDetail_new(that.$route.params.wayBillID);
            that.$refs.selection.selectAll(false)
                      if(that.$refs.History.Nameitem=='3'){
              that.$refs.History.changeTabs('3')
            }else if(that.$refs.History.Nameitem=='1'){
              that.$refs.History.changeTabs('1')
            }else{
              that.$refs.History.changeTabs('0')
            }
          }, 100);
        } else {
          this.Nomatching=[]
           this.$Message.error({
            content:res.Data,
            duration: 5
          });
           this.isdisabled=false;
            if(this.isAbnormal==true){
            this.isAbnormal=false;
            this.Summary=''
            this.Nomatching=[]
          }
          var that = this;
          that.loading=true
          setTimeout(function() {
            that.CgDetail_new(that.$route.params.wayBillID);
            that.$refs.selection.selectAll(false)
                      if(that.$refs.History.Nameitem=='3'){
              that.$refs.History.changeTabs('3')
            }else if(that.$refs.History.Nameitem=='1'){
              that.$refs.History.changeTabs('1')
            }else{
              that.$refs.History.changeTabs('0')
            }
          }, 100);
        }
      });
    },
    testrouter(){
       this.$store.dispatch("setshowdetail", false);
       this.$router.go(-1);
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
    isAbnormalclick() {
      if (this.SelectRow.length <= 0) {
        this.$Message.error("请选择到货异常的产品");
      } else{
        for(var i=0,len=this.SelectRow.length;i<len;i++){
          if(this.SelectRow[i].Summary==null){
            this.$Message.error("请输入异常原因");
              break
          }else{
            if((this.SelectRow[i].CurrentQuantity!=null)&&(this.SelectRow[i].Product.Manufacturer!=null)){
              if(i==this.SelectRow.length-1){
                  this.showboxmax()
                  this.Warehousingstatus=130
              }
            }else{
              this.$Message.error("品牌，本次到货不能为空");
              this.Warehousingstatus=null
              break
            }
          }
        }
      }
    },
    //到货异常 确认按钮
    Abnormal_btn() {
      if (this.Summary == undefined || this.Summary == "") {
        this.isAbnormal = true;
        this.$Message.error("请输入异常原因");
        // console.log(this.isAbnormal)
      } else {
        this.ok_Warehousing(130);
      }
    },
    closeerror() {
      //异常到货关闭
      this.Summary = ""; //备注
      this.isAbnormal = false;
    },
    changeerror(value) {
      // console.log(this.isAbnormal)
      if (value == true) {
        this.isAbnormal = true;
      } else {
        this.isAbnormal = false;
        this.Summary = ""; //备注
      }
    },
    showhistory(name) {
      this.historydata = true;
      this.reFresh=true
    },
    changehistore(value){
      if(value==false){
        this.reFresh=false
      }
    },
    TakeGoods() {
      //我去提货
      var admin= sessionStorage.getItem("userID")
      TakeGoods(this.waybillinfo.ID,admin).then(res => {
        if (res.Success == true) {
          this.$Message.success("提货锁定成功，请去提货");
          // this.waybillinfo.ExcuteStatusDes = "正在提货中";
          this.waybillinfo.LoadingExcuteStatus=105
           this.isdisabled=false;
        } else {
          this.$Message.error("提货锁定失败");
           this.isdisabled=true;
        }
      });
    },
     changeabnormal_enter(value) {
      if (value == false) {

      } else {
        this.Storages = [];
        GetSortingID().then(res => {
          var data = {
            iscx: false,
            isenter: true,
            _disabled: false,
            ID: "LR" + 1 + new Date().getTime(),
            NoticeID: null,
            Product: {
              PartNumber: null,
              Manufacturer:null,
              PackageCase: null,
              Packaging: null,
            },
            Conditions:{
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
            SortingID: res,
            Files: [],
            boxdate:null
          };
          this.Storages.push(data);
          this.timer = new Date().getTime();
          // this.$refs.NotingEntername.Storehouselist=this.Storehouselist
          this.$nextTick(function () {
          });
        });
      }
    },
    //无通知产品录入保存
    ok_abnormal() {
    this.NullNotice=[]
      for(var i=0,lens=this.Storages.length;i<lens;i++){
          if((this.Storages[i].CurrentQuantity!=null)&&(this.Storages[i].Product.PartNumber!=null)&&(this.Storages[i].origin!=null)&&(this.Storages[i].BoxCode!=null)&&(this.Storages[i].Summary!=null)&&(this.Storages[i].Weight!=null)&&(this.Storages[i].Product.Manufacturer!=null)){
               this.NullNotice.push(this.Storages[i])
              if(i==this.Storages.length-1){
                //  this.$Message.success("测试入库");
                this.Warehousingstatus=130
                this.ok_Warehousing(130,'enter');
              }
          }else{
              this.$refs.modal.visible = true;
              this.abnormal_enter=true;
              this.NullNotice=[]
              this.$Message.error("请输入必填项目");
              break;
          }
           this.modal = false
      }

    },
    PrintBoxcode(){  //箱签打印
      var Obj={
        Source:this.waybillinfo.Source,
        waybillID:this.waybillinfo.ID
      }
      this.showprintboxcode=true;
      this.$refs.printbox.getparents(Obj) 
      this.$refs.printbox.GetPackageType() 
    },
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
    //确认分摊重量
    ok_share(){
          // this.showshare=false
          var Totalquantity=null
          var singlet =null;
          // console.log(this.SelectRow)
          for(var i=0,lens=this.SelectRow.length;i<lens;i++){
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
    //分摊重量弹出框改变的时候
    changeshare(value){
      if(value==false){
        this.sharevalue=null
      }
    },
   // 获取客户等级
    Getclientdata(entdata){
      Getclientdata(entdata).then(res=>{
        this.clientGrade=res.obj.Grade
      })
    },
     //保存承运商,司机，车牌号
    ok_addCarrier(type){
      if(type==1){
         this.$refs.addCarrier.sumbit_btn()
      }else if(type==2){
        this.$refs.addCarrier.delitem()
        this.showaddCarrier=false
      }else if(type==3){
         this.$refs.AddDriver.sumbit_btn()
      }else if(type==4){
         this.$refs.AddDriver.delitem()
         this.showaddDriver=false
      }else if(type==5){
         this.$refs.addcar.sumbit_btn()
      }else if(type==6){
         this.$refs.addcar.delitem()
         this.showaddCar=false
      }else if(type==7){
         this.$refs.Addcontacts.sumbit_btn()
      }else if(type==8){
         this.$refs.Addcontacts.delitem()
         this.showaddcontacts=false
      }
     
    },
    setdisabled(type){
      this.adddisabled=type
    },
    fatherMethodCarrier(res){
      this.adddisabled=false
      if(res!='false'){
         var housid=sessionStorage.getItem("UserWareHouse")
        var carrie={ID:res.Carrier.ID,Name:res.Carrier.Name}
        this.CarrierList.push(carrie)
        this.Carriers(this.waybillinfo.Type,housid)
        this.waybillinfo.CarrierID=res.Carrier.ID
        this.waybillinfo.CarrierName=res.Carrier.Name

         var data={ Name:res.Driverinfo.Name, ID:"", }
        this.DriversArr.push(data)
        this.waybillinfo.Driver=res.Driverinfo.Name

         var data={ value:res.Carinfo.CarNumber1, }
        this.CarArr.push(data)
        this.waybillinfo.CarNumber1=res.Carinfo.CarNumber1
        
        this.GetDriversCars(this.waybillinfo.CarrierID);
        this.showaddCarrier=false;
        this.$refs.addCarrier.delitem()

      }
    },
    //保存快递公司
    ok_contacts(res){
      this.adddisabled=false
       if(res!='false'){
        var housid=sessionStorage.getItem("UserWareHouse")
        var carrie={ID:res.Carrier.ID,Name:res.Carrier.Name}
        this.CarrierList.push(carrie)
        this.Carriers(this.waybillinfo.Type,housid)
        this.waybillinfo.CarrierID=res.Carrier.ID
        this.waybillinfo.CarrierName=res.Carrier.Name
        this.oldcrriers=res.Carrier.ID
        this.showaddcontacts=false;
        this.$refs.Addcontacts.delitem()
      }
    },
    // 保存司机 DriverAdd,
    ok_addDriver(res){
      this.adddisabled=false
      if(res!='false'){
        var data={
          Name:res,
          ID:"",
        }
          this.DriversArr.push(data)
          this.GetDriversCars(this.waybillinfo.CarrierID);
          this.waybillinfo.Driver=res
          this.showaddDriver=false;
          this.$refs.AddDriver.delitem()
      }
    },
    // 保存车牌号 TransportAdd
    ok_addCar(res){
      this.adddisabled=false
      if(res!='false'){
        var data={
          value:res,
        }
        this.CarArr.push(data)
        this.GetDriversCars(this.waybillinfo.CarrierID);
        this.waybillinfo.CarNumber1=res
        this.showaddCar=false
        this.$refs.AddDriver.delitem()
      }
    },
    uploadeCgDetail_new(){
      this.loading=true
      this.CgDetail_new(this.$route.params.wayBillID);
    },
    TestAVGWeightsum(){
      if(this.sharevalue!=null){
        var reg = /^\d+(\.\d{0,5})?$/;
        if(reg.test(this.sharevalue) == false||this.sharevalue==0){
            this.$Message.error("请输入数字,小数点保留五位，且不等于零");
            this.sharevalue=null
        }else{
          if(this.sharevalue<(this.AVGWeightsum-this.AVGWeightsum*(1/2))||this.sharevalue>(this.AVGWeightsum+this.AVGWeightsum*(3/2))){
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
    getwaybillstatus(data){
       var postdata={
            WaybillIDs:[data]
        }
        var _this=this
       this.timerwaybill = setInterval(() => {
          setTimeout(() => {
           GetWaybillCurrentStatus(postdata).then(res=>{
             if(res.Data[0].Operated==false){
                _this.isdisabled=true;
                _this.Operateddisable=false

             }else{
               if(_this.waybillinfo.Type==1&&_this.waybillinfo.LoadingExcuteStatus==100){
                   _this.isdisabled=true;
                   _this.Operateddisable=false
               }else{
                  _this.isdisabled=false;
                  _this.Operateddisable=true
               }
               

             }
           })
          }, 0)
      }, 2000)
      // this.timerwaybill=timerwaybill
      this.$once('hook:beforeDestroy', function() {
            clearInterval(this.timerwaybill)
            this.timerwaybill = null
      })
    },
    changeData(val){
      this.saleDate=val
      if(this.newboxcode!=''&&this.newboxcode!=null){
        this.handleCreate1(this.newboxcode)
      }
    },
    //刷新当前数据
    sync_btn(){
       this.loading = true;
       this.CgDetail_new(this.$route.params.wayBillID);
       this.$refs.History.changeTabs('3')
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
    IsRecordWarehouseFee(OrderID, WaybillID) {
      IsRecordWarehouseFee(OrderID).then(res => {
        if (res.data == "False") {
          this.isdisabled = true; //未收取
        } else {
          if (this.waybillinfo.ExcuteStatus != 120) {
            this.getwaybillstatus(this.waybillinfo.ID)// 订单回退状态
          }
         
        }
      })
    },
    Parentfun() {
      if (this.waybillinfo.ExcuteStatus == 120) {
        this.isdisabled = true;
      } else {
        this.isdisabled = false;
        this.getwaybillstatus(this.waybillinfo.ID)// 订单回退状态
      }
    }
  },
   destroyed() {
    clearInterval(this.timerwaybill)
    this.timerwaybill = null
    this.timerwaybill = null;
  }
};
</script>
