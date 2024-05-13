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
    width: 80px;
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

  .detail_tablebox > > > .ivu-table-cell {
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

  .Mustfill {
    color: red;
  }

  .linkurlcolor {
    color: #2d8cf0;
  }

  .Filesbox:hover {
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
            <Col style="width: 23%;float: left;">
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

              <li class="itemli">
                <span class="detail_title1">总&nbsp;&nbsp;货&nbsp;&nbsp;值：</span>
                <span style="color:'#ccc';font-width:600">{{waybillinfo.chgTotalCurrency}}  {{waybillinfo.chgTotalPrice}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">是否逾期：</span>
                <span style="color:'#ccc';font-width:600" v-if="waybillinfo.overDue==true">是</span>
                <span style="color:'#ccc';font-width:600" v-else>否</span>
              </li>
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
            <Col style="width: 27%;float: left;">
            <ul class="detail_li">
              <li class="itemli">
                <span class="detail_title2">制单时间：</span>
                <span>{{waybillinfo.CreateDate|showDateexact}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2" style="float: left;">供&nbsp;&nbsp;应&nbsp;&nbsp;商：</span>
                <span style="display: inline-block; width:70%; line-height: 1;">
                  <p v-for="(item,index) in waybillinfo.Supplier">{{item.Supplier}}&nbsp;&nbsp;<Tag color="volcano">LV {{item.SupplierGrade}}</Tag></p>
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title2">发货方式：</span>
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
                <span class="detail_title2">总&nbsp;&nbsp;额&nbsp;&nbsp;度：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.total}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">总&nbsp;&nbsp;欠&nbsp;&nbsp;款：</span>
                <span style="color:'#ccc';font-width:600">CNY {{waybillinfo.totalDebt}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title2">特殊标签：</span>
                <span style="line-height: 20px;width: 70%;display: inline-block;">
                  <div v-for="(item,index) in waybillinfo.LableFile">
                    <a class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</a>
                    <a @click="fileprint(item.Url)">打印</a>
                  </div>
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2">
                <span class="detail_title2" style="float: left;">随货文件：</span>
                <span style="display: inline-block;">
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
                <span class="detail_title3">客&nbsp;&nbsp;户&nbsp;&nbsp;编&nbsp;&nbsp;号：</span>
                <span>{{waybillinfo.EnterCode}}（{{waybillinfo.ClientName}}）<Tag color="geekblue">LV {{clientGrade}}</Tag></span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3">自提时间：</span>
                <span>{{waybillinfo. AppointTime|showDateexact}}</span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==1">
                <span class="detail_title3">联&nbsp;&nbsp;&nbsp;&nbsp;系&nbsp;&nbsp;&nbsp;&nbsp;人：</span>
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
                <span class="detail_title3">承运商(本次)：</span>
                <span>
                  <Select v-model="waybillinfo.CarrierID"
                          style="width:60%"
                          @on-change="changeacrrier"
                          :label-in-value="true"
                          :disabled='true'>
                    <Option v-for="item in CarrierList"
                            :value="item.ID"
                            :key="item.ID">
                      {{ item.Name }}
                    </Option>
                  </Select>
                  <Icon type="md-create" v-if="warehouseID.indexOf('SZ')!=-1&&waybillinfo.Type==3" @click="showchangeexpress=true" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3">运单号(本次)：</span>
                <span>
                  <Input style="width:60%" v-model="waybillinfo.Code" :disabled='true' />
                </span>
                <Icon type="md-create" v-if="warehouseID.indexOf('SZ')!=-1&&waybillinfo.Type==3" @click="clickshowcode" />
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2">
                <span class="detail_title3">司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机：</span>
                <span>
                  <Input v-model="waybillinfo.Driver" :disabled='true' style="width:60%" />
                </span>
              </li>
              <li class="itemli" v-if="waybillinfo.Type==2">
                <span class="detail_title3">车&nbsp;&nbsp;&nbsp;&nbsp;牌&nbsp;&nbsp;&nbsp;&nbsp;号：</span>
                <span>
                  <Input v-model="waybillinfo.CarNumberID" :disabled='true' style="width:60%" placeholder="请输入车牌号" />
                </span>
              </li>

              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3" style="float: left;">联&nbsp;&nbsp;&nbsp;系&nbsp;&nbsp;&nbsp;人：</span>
                <span style="display: inline-block;max-width:60%;line-height: 20px;">
                  <i v-if="waybillinfo!=null"> {{waybillinfo.Receiver.Name}}</i>
                </span>
                <Icon type="md-create" v-if="warehouseID.indexOf('SZ')!=-1&&waybillinfo.Type==3" @click="clickshoReceiver" />
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3" style="float: left;">联&nbsp;&nbsp;系&nbsp;&nbsp;电&nbsp;&nbsp;话：</span>
                <span style="display: inline-block;max-width:60%;line-height: 20px;">
                  <i v-if="waybillinfo!=null"> {{waybillinfo.Receiver.Mobile}}</i>
                </span>
                <Icon type="md-create" v-if="warehouseID.indexOf('SZ')!=-1&&waybillinfo.Type==3" @click="clickshoReceiver" />
              </li>

              <li class="itemli" v-if="waybillinfo.Type!=1">
                <span class="detail_title3" style="float: left;">收&nbsp;&nbsp;货&nbsp;&nbsp;地&nbsp;&nbsp;址：</span>
                <span style="display: inline-block;max-width:60%;line-height: 20px;">
                  <i v-if="waybillinfo!=null">{{waybillinfo.CoeAddress}}</i>
                </span>
                <Icon type="md-create" v-if="warehouseID.indexOf('SZ')!=-1&&waybillinfo.Type==3" @click="clickshoReceiver" />
              </li>
              <li class="itemli" v-if="waybillinfo.Type!=2&&warehouseID.indexOf('SZ')==-1">
                <span class="detail_title3">随&nbsp;&nbsp;货&nbsp;&nbsp;文&nbsp;&nbsp;件：</span>
                <div v-for="(item,index) in waybillinfo.Files" style="display: inline-block;" v-if="item.Type==25">
                  <!-- <span>{{item.CustomName}}</span> -->
                  <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <a @click="fileprint(item.Url)">打印</a>
                </div>
              </li>
            </ul>
            </Col>
            <Col style="width: 20%;float: left;">
            <ul class="detail_li" style="margin-left:20px;">
              <li v-if="warehouseID.indexOf('SZ')!=-1" class="itemli">运输批次号：{{waybillinfo.LotNumber}}</li>
              <li class="itemli" v-if="waybillinfo.Type==3&&warehouseID.indexOf('SZ')!=-1">
                <span class="detail_title3">随&nbsp;&nbsp;货&nbsp;&nbsp;文&nbsp;&nbsp;件：</span>
                <div v-for="(item,index) in waybillinfo.Files" style="display: inline-block;" v-if="item.Type==25">
                  <!-- <span>{{item.CustomName}}</span> -->
                  <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <a @click="fileprint(item.Url)">打印</a>
                </div>
              </li>
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
              <li class="itemli" v-if="IsSendGoodsFile==true" style="padding-top:10px;clear: both;">
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
                    <Tooltip content="删除" placement="top">
                      <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                    </Tooltip>
                    <Tooltip content="打印" placement="top">
                      <Icon type="ios-print-outline" @click="fileprint(item.Url)" />
                    </Tooltip>
                  </div>
                </div>
                <p v-if="IsSendGoodsFile==true">送货单照片：</p>
                <div v-if="IsSendGoodsFile==true">
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
                   placeholder="请输入品牌或型号"
                   clearable
                   @on-clear='search_pro'
                   style="width:80%;float:left;position: relative;left: 3px" />
            <Button style="float:left" @click="search_pro" type="primary">筛选</Button>
            <!-- <Button style="float:left" type="primary">筛选</Button> -->
          </ButtonGroup>
          <!-- <Button type="primary" @click="Labelprint">标签打印</Button> -->
          <Button type="primary" @click="clickshowchangebox" v-if='warehouseID.indexOf("SZ")==-1' :disabled='waybillinfo.ExcuteStatus==215?true:false'>一键入箱</Button>
          <Button type="primary" @click="showprint=true">一键打印</Button>
          <Button type="primary" @click="showBudget('meet','in',waybillinfo.Source)">收入</Button>
          <Button type="primary" @click="showBudget('meet','out',waybillinfo.Source)">支出</Button>
          <div style="float:right">
            <Button type="primary"
                    shape="circle"
                    icon="md-checkmark"
                    @click="finish_btn"
                    :disabled="isdisabled==true?true:false">
              完成分拣
            </Button>
            <Button v-if='warehouseID.indexOf("HK")==-1'
                    :disabled="isdisabled==true?true:false"
                    type="warning"
                    shape="circle"
                    icon="ios-alert-outline"
                    @click="isAbnormal=true">
              分拣异常
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
              <template slot-scope="{row,index}" slot="Boxing_code">
                <span v-if='warehouseID.indexOf("SZ")!=-1'>{{row.BoxCode|showboxcode}}</span>
                <div style="display:inline-block;width:80%" v-else>
                  <Select v-model="row.BoxCode"
                          :transfer="true"
                          filterable
                          allow-create
                          :disabled='row.CurrentQuantity<=0?true:false'
                          @on-create="handleCreate1($event,index)"
                          @on-change="clickDropdown($event,index)">
                    <Option v-for="(item,index) in boxingarr"
                            :value="item.Code"
                            :key="index"
                            :label="item.Code">
                      <span>{{item.Code}}</span>
                    </Option>
                  </Select>
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
                      @click="finish_btn"
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
    <!-- 一键入箱 开始 -->
    <Modal v-model="showchangebox"
           title="选择箱号"
           @on-ok="ok_changebox"
           @on-cancel="cancel">
      <Select v-model="newboxcode">
        <Option v-for="item in boxingarr" :value="item.Code" :key="item.ID">{{ item.Code }}</Option>
      </Select>
    </Modal>
    <!-- 一键入箱 结束 -->
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
    <!-- 异常出库 开始 -->
    <Modal v-model="isAbnormal" title="出库异常" @on-visible-change="errorstatue">
      <div slot="close">
        <Icon type="md-close"
              color="rgb(33, 28, 28)"
              @click="closeerror"
              style="font-size:18px;" />
      </div>
      <span style="line-height: 1; display: block; padding-bottom: 10px;">
        <em class="Mustfill">*</em>分拣异常原因
      </span>
      <Input v-model="Summary" type="textarea" :rows="2" placeholder="备注" />
      <div slot="footer">
        <Button @click="closeerror">取消</Button>
        <Button type="primary" @click="Abnormal_btn">确定</Button>
      </div>
    </Modal>
    <!-- 异常到货 结束-->
    <!-- 一键打印弹出 开始 -->
    <Modal v-model="showprint"
           title="打印">
      <p slot="header" style="font-size:16px">
        <span>打印</span>
        <!-- <Icon style="font-size:20px" type="ios-desktop-outline" /> -->
      </p>
      <CheckboxGroup v-model="fruit">
        <Checkbox label="出库单打印"></Checkbox>
        <Checkbox label="送货单打印"></Checkbox>
        <Checkbox v-if="Iswaybillprint==true" label="运单打印"></Checkbox>
      </CheckboxGroup>
      <Modal v-model="preview" fullscreen title="Fullscreen Modal">
        <div>This is a fullscreen modal</div>
      </Modal>
      <div slot='footer'>
        <Button @click="cancel">取消</Button>
        <!-- <Button @click="preview_btn" type="primary">预览</Button> -->
        <!-- <Button @click="preview_btn" type="info">预览</Button> -->
        <Button @click="CgAllprint" type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 一键打印弹出 结束 -->
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
    <!-- 修改发货快递公司 开始 -->
    <Modal :mask-closable='false'
           v-model="showchangeexpress"
           title="修改承运商">
      <!--<Express ref='Express' v-if="showchangeexpress" :WaybillID="waybillinfo.ID"  :uploadwaybill='uploadwaybill'></Express>-->
      <Express ref='Express' v-if="showchangeexpress" :WaybillID="waybillinfo.ID" :ShipperCode="waybillinfo.CarrierName=='EMS'?'EMS':(waybillinfo.CarrierName=='顺丰'? 'SF' : 'KYSY')" :Expresstype=waybillinfo.Extype :PayType=waybillinfo.ExPayType :ThirdPartyCardNo="waybillinfo.ThirdPartyCardNo" :uploadwaybill='uploadwaybill'></Express>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button @click="ok_changeexpress" type="primary">确认</Button>
      </div>
    </Modal>
    <!-- 修改发货快递公司 结束 -->
    <Modal :mask-closable='false'
           v-model="showchangewaybillcode"
           title="修改运单号">
      <Input v-model.trim="changewaybillcodeinput" placeholder="请输入运单号" />
      <div slot="footer">
        <Button @click="cancel_changecode">取消</Button>
        <Button @click="ok_changecode" type="primary">确认</Button>
      </div>
    </Modal>
    <Modal :mask-closable='false'
           v-model="showchangeReceiver"
           title="修改收货信息">
      <div>
        联&nbsp;&nbsp;系&nbsp;&nbsp;人：<Input v-model.trim="Receiverdata.Name" style="width:80%" placeholder="请输入联系人" />
      </div>
      <div style="padding:10px 0">
        联系电话：<Input v-model.trim="Receiverdata.Mobile" style="width:80%" placeholder="请输入电话" />
      </div>
      <div>
        收货地址：<Input v-model.trim="Receiverdata.Address" style="width:80%" placeholder="请输入收货地址" />
      </div>
      <div slot="footer">
        <Button @click="cancel_changeReceiver">取消</Button>
        <Button @click="ok_changeReceiver" type="primary">确认</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
  import logged from '../Common/logged'
  import Express from './Express'
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
  import { CgPickingsDetail, CgpickingsoutBtn, CgPickingsSearch, CgBoxesShow, CgpickingsErrorBtn, CgReciptConfirm, CgDeleteFiles, GetDriversCars, CgCarriers, Getclientdata, CgUpdateWaybillCode, ModifyWaybillCode, ModifyWaybillConsigneeInfo } from '../../api/CgApi'
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
    /* PrintKdn,*/
    /*ReprintKdnFaceSheet,*/
    PrintFaceSheet,
    ReprintFaceSheet,
    FilesProcess
  } from "@/js/browser.js";
  export default {
    name: "",
    components: {
      'logg-ed': logged,
      'Express': Express
    },
    data() {
      return {

        showchangeexpress: false,
        loggdetime: '',//操作日志时间
        showlogged: false,//操作日志
        clientGrade: null,//客户等级
        isdisabled: false,//是否禁止选中
        preview: false,
        originalarr: [],//原始数据
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
        IsfileDel: false,//删除文件 返回值
        fruit: [],//打印选中
        Iswaybillprint: false,
        showprint: false,//显示一件打印
        loading: true,
        pusharr: [],
        SelectRow: [], //多选数组
        searchkey: "",
        boxingarr: [], //箱号
        printlist: [], //清单打印数据
        wayBillID: this.$route.params.detailID,
        detail_ID: "",
        showphoto: false, //显示拍照弹出框
        CarrierList: [], //承运商列表
        Conveyingplace: [], //输送地列表
        waybillinfo: null,//运单信息
        detailitem: [],//通知信息
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
            fixed: 'left',
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
            width: 180
          },
          {
            title: "品牌",
            key: "brand",
            align: "center",
            width: 100,
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.Product.Manufacturer);
            }
          },
          {
            title: "产地",
            key: "OriginName",
            width: 100,
            align: "center",
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.OriginName);
            }
          },
          {
            title: "库位",
            key: "ShelveID",
            align: "center",
            width: 120,
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.ShelveID);
            }
          },
          {
            title: "应出数量",
            key: "Quantity",
            width: 80,
            align: "center",
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.Quantity);
            }
          },
          {
            title: "已出数量",
            key: "PickedQuantity",
            width: 80,
            align: "center",
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.PickedQuantity);
            }
          },
          {
            title: "剩余数量",
            key: "LeftQuantity",
            align: "center",
            width: 80,
            render: (h, params) => {
              var vm = this;
              return h("span", {}, params.row.LeftQuantity);
            }
          },
          {
            title: "本次发货数量",
            key: "practical",
            align: "center",
            width: 80,
            render: (h, params) => {
              var vm = this;
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value: params.row.CurrentQuantity,
                  disabled: true,
                },
                on: {
                  "on-change"(event) {
                    //值改变时
                    //将渲染后的值重新赋值给单元格值
                    params.row.CurrentQuantity = event.target.value;
                    vm.detailitem[params.index] = params.row;
                  }
                }
              });
            }
          },
          {
            title: "箱号",
            slot: "Boxing_code",
            align: "center",
            width: 220
          },
          {
            title: "毛重(kg)",
            key: "GrossWeight",
            width: 130,
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
                    vm.detailitem[params.index] = params.row;
                  },
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
            width: 150,//220
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
        applydata: [],
        cityList: [],
        WarehousingMsg: false, //完成入库的提示
        isAbnormal: false, //是否异常到货
        Summary: null, //后台提供的备注信息对象
        CarArr: [], //车票号
        DriversArr: [], // 司机
        warehouseID: sessionStorage.getItem("UserWareHouse"), //当前库房ID
        IsSendGoodsFile: false,
        showchangewaybillcode: false,//修改运单号状态
        changewaybillcodeinput: null,//将原有运单号给新的变量
        showchangeReceiver: false,//
        Receiverdata: {
          WaybillID: "", // 运单ID
          Address: "", // 收货地址
          Name: "", // 收货联系人
          Mobile: "" // 收货电话
        }
      };
    },
    filters: {
      showDate: function (val) {
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
      //window['KdnPrinted'] = this.changedcode
      window['KdPrinted'] = this.changedcode

    },
    computed: {
      Budgetdetail() {
        // console.log(this.$store.state.Budget.Budgetdetail)
        return this.$store.state.common.Budgetdetail;
      }
    },
    mounted() {
      this.getdetail_data();
    },
    methods: {
      //去除前后空格
      trim(str) {
        return str.replace(/(^\s*)|(\s*$)/g, '')
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
      showBudget(type, Budget, Source) {
        //收支明细展开
        this.$store.dispatch("setBudget", true);
        if (type == "meet") {
          var namemeet = "";
          this.$router.push({
            name: 'outdetailBudgetDeclare',
            query: {
              webillID: this.waybillinfo.ID,
              OrderID: this.waybillinfo.OrderID,
              type: Budget,
              otype: "out",
              conduct: Source
            }
          });
        } else {
        }
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
        // this.$Message.info("Clicked ok");
        this.showphoto = false;
      },
      cancel() {
        // this.$Message.info("Clicked cancel");
        this.showphoto = false;
        this.showprint = false;
        this.showchangeexpress = false
        this.$refs.Express.cleanexpressdata()
      },
      getdetail_data() {  //获取详情页数据
        CgPickingsDetail(this.wayBillID).then(res => {
          this.Getclientdata(res.Waybill.EnterCode)
          if (res.Waybill.ExcuteStatus == 215) {
            this.isdisabled = true
          } else {
            this.isdisabled = false
          }
          this.waybillinfo = res.Waybill;
          console.log(this.waybillinfo)
          this.originalarr = res.Notices
          for (var i = 0, len = res.Notices.length; i < len; i++) {
            if (res.Notices[i].CurrentQuantity <= 0) {
              res.Notices[i]._disabled = true
            } else {
              res.Notices[i]._disabled = false
            }
          }
          this.detailitem = res.Notices
          this.getboxarr()
          this.Carriers(res.Waybill.Type);
          this.GetDriversCars(res.Waybill.CarrierID)
          this.loading = false;
          // this.fruit=['出库单打印','送货单打印','运单打印']
          if (this.warehouseID.indexOf('SZ') != -1) {  //如果不是深圳库房，则没有运单打印
            if (res.Waybill.ExcuteStatus == 215) {
              if (res.Waybill.Type == 1 || res.Waybill.Type == 2) {
                this.IsSendGoodsFile = true
              } else {
                this.IsSendGoodsFile = false
              }
            }
            if (res.Waybill.Type != 1 || res.Waybill.Type != 2) { //如果送货方式不是自提或送货上门，并且快放类型与快递方式为顺丰或跨域则显示运单打印
              if (res.Waybill.Extype != null && res.Waybill.ExPayType != null) {  //处理打印的类型
                this.fruit = ['送货单打印', '运单打印']//打印选中
                this.Iswaybillprint = true
              } else {
                this.fruit = ['送货单打印']//打印选中
                this.Iswaybillprint = false
              }
            } else {
              this.fruit = ['送货单打印']//打印选中
              this.Iswaybillprint = false
            }
          } else {
            this.fruit = ['送货单打印']//打印选中
            this.Iswaybillprint = false
          }
        })
      },
      //修改送货方式
      changewaybillType(value) {
        this.Carriers(value);
      },
      detailspage(value) {
        this.getdetail_data();
      },
      fromphotos(type) {
        if (type == "Waybill" || type == 'Clientimg') {
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
              Type: 8010
            }
          };
          FormPhoto(data);
        }
      },
      //调用打印运单号，返回运单号的值
      changedcode(message) {
        this.waybillinfo.Code = message
        var data = {
          WaybillID: this.waybillinfo.ID,
          Code: message
        }
        CgUpdateWaybillCode(data).then(res => {
          if (res.success == true) {
            this.$Message.success("运单打印成功");
          } else {
            this.$Message.error(res.data);
          }
        })
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
          Type: null
        }
        if (imgdata.SessionID == 'Waybill') {
          newfile.Type = 8010
          this.waybillinfo.FeliverGoodFile.push(newfile);
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
              this.detailitem[i].Imagefiles.push(newfile);
              this.changeoriginalarr(this.detailitem[i])
            }
          }
        }
      },
      SeletUpload(type) { // 传照
        if (type == "Waybill" || type == 'Clientimg') {
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
          console.log(data)
          SeletUploadFile(data);
        }
      },
      //承运商列表
      Carriers(type) {
        var whid = this.warehouseID
        CgCarriers(type, whid, 200).then(res => {
          this.CarrierList = res;
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
      clickDropdown(event, index) {//改变箱号
        this.detailitem[index].BoxCode = event;
        this.changeoriginalarr(this.detailitem[index])
      },
      getboxarr() {//获取可用箱号
        // var houseid=sessionStorage.getItem("UserWareHouse")
        // GetBoxes(houseid,"200").then(res=>{
        //   this.boxingarr=res;
        // })
        var data = {
          whCode: sessionStorage.getItem("UserWareHouse"), //库房标识（库房编号）
          enterCode: this.waybillinfo.EnterCode, //入仓号
          date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
          // orderID:this.waybillinfo.OrderID
        };
        console.log(data)
        CgBoxesShow(data).then(res => {
          console.log(res);
          if (res.length > 0) {
            this.boxingarr = res;
          } else {
            this.boxingarr = []
          }
        });
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
        for (var i = 0, ilen = this.detailitem.length; i < ilen; i++) {
          for (var j = 0; j < this.SelectRow.length; j++) {
            if (this.detailitem[i].ID == this.SelectRow[j].ID) {
              this.detailitem[i].BoxCode = this.newboxcode;
            }
          }
        }
        this.SelectRow = [];
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
        console.log(this.originalarr)
        if (this.warehouseID.indexOf('SZ') != -1 && this.waybillinfo.Type == 3 && this.waybillinfo.Code == null) {
          this.$Message.error('请先打印运单')
        } else {
          var istrue = 1;
          this.pusharr = []
          for (var i = 0, len = this.originalarr.length; i < len; i++) {
            if (this.originalarr[i].BoxCode != null) {
              var obj = {
                NoticeID: this.originalarr[i].ID,
                OutputID: this.originalarr[i].OutputID,
                BoxCode: this.originalarr[i].BoxCode,
                Quantity: this.originalarr[i].CurrentQuantity,
                Weight: this.originalarr[i].Weight,
                NetWeight: this.originalarr[i].NetWeight,
                Volume: this.originalarr[i].Pickings.Volume,
                Files: this.originalarr[i].Imagefiles
              }
              this.pusharr.push(obj)
            } else {
              this.$Message.error('请选择箱号')
              this.pusharr = []
              istrue++;
              break;
            }
            if (istrue == 1) {
              this.WarehousingMsg = true
            }
          }
        }

      },
      setWarehousing(data) {
        //确定出库，调取后台出库接口
        CgpickingsoutBtn(data).then(res => {
          console.log(res)
          if (res.Success == true) {
            this.$Message.success('出库完成，一秒后自动关闭')
            var _this = this
            setTimeout(function () {
              _this.$store.dispatch('setshowtype', 0)
              _this.$store.dispatch('setshowdetailout', false)
              _this.$router.go(-1)
            }, 1000)

          } else {
            this.$Message.error(res.Data)
            this.isdisabled = false
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
        this.isdisabled = true
        this.WarehousingMsg = false
        var uploaddata = {
          Waybill: {
            ID: this.waybillinfo.ID,
            ExcuteStatus: this.waybillinfo.ExcuteStatus,
            Type: this.waybillinfo.Type, //库房自定影
            OrderID: this.waybillinfo.OrderID, //所属订单
            CarrierID: this.waybillinfo.CarrierID, //送货是库房定义,快递是客户定义
            Summary: "",
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
        this.setWarehousing(uploaddata)
        console.log(uploaddata)
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
          this.Summary = ''
        }
      },
      Abnormal_btn() {
        this.isAbnormal = false
        if (this.Summary == undefined || this.Summary == '') {
          this.isAbnormal = true
          this.$Message.error('请输入异常原因')
        } else {
          var data = {
            waybillid: this.waybillinfo.ID,
            adminid: sessionStorage.getItem('userID'),
            orderid: this.waybillinfo.OrderID,
            summary: this.Summary
          }
          console.log(data)
          CgpickingsErrorBtn(data).then(res => {
            if (res.Success == true) {
              this.$Message.success('出库完成，一秒后自动关闭')
              var _this = this
              setTimeout(function () {
                _this.$store.dispatch('setshowtype', 0)
                _this.$store.dispatch('setshowdetailout', false)
                _this.$router.go(-1)
              }, 1000)

            } else {
              this.$Message.error(res.Data)
            }
          })
        }
      },
      closeerror() {
        //异常到货关闭
        this.isAbnormal = false
        this.Summary = '' //备注
        this.Reason = '外观损坏' //异常原因
      },
      //文件信息删除后的回调函数 删除本地图片信息
      Removebackfun(file, type) {
        console.log(file)
        console.log(type)
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
      // 点击删除文件信息
      handleRemove(file, type) {
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
            this.Removebackfun(file, type)
            this.$Message.success('删除成功')
          } else {
            this.IsfileDel = false
            this.$Message.error('删除失败')
          }
        })
      },
      changeacrrier(value) {
        //改变承运商的时候触发
        console.log(value)
        this.waybillinfo.CarrierName = value.label
        if (this.waybillinfo.Type == 2) {
          this.GetDriversCars(value.value)
        } else {
        }
      },
      GetDriversCars(key) { //根据送货上门承运商获取司机与车牌号
        GetDriversCars(key).then(res => {
          this.CarArr = res.Transports; //车票号
          this.DriversArr = res.Drivers; // 司机
          // this.waybillinfo.Driver='';
          // this.waybillinfo.CarNumber1='';
        })
      },
      // search_pro() {  //筛选
      //     this.loading=true
      //      CgPickingsSearch(this.waybillinfo.ID,this.searchkey).then(res=>{
      //         this.detailitem=res;
      //         this.loading=false;
      //       })
      //   },
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
      //一键打印重构
      CgAllprint() {
        this.showprint = false;
        if (this.fruit.indexOf('运单打印') != -1) {  //如果运单打印存在 调用打印运单的方法
          this.waybillprint()
        }
        if (this.fruit.indexOf('出库单打印') != -1) {
          this.outorder_print()
        }
        if (this.fruit.indexOf('送货单打印') != -1) {
          this.Boxing_print()
          //  if(this.waybillinfo.Type==1||this.waybillinfo.Type==2){
          //     this.Boxing_print()
          //     this.Boxing_print()
          //     this.Boxing_print()
          //     this.Boxing_print()
          //  }else{
          //     this.Boxing_print()
          //     this.Boxing_print()
          //  }
        }
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
          console.log("深圳库房")
          if (this.waybillinfo.Type == 4 || this.waybillinfo.Type == 3) {
            this.language = 'SC'
            Numcopies = 2
          } else {
            this.language = 'SC'
            Numcopies = 2
          }

        } else { //香港库房
          console.log("香港库房")
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
      //运单打印
      waybillprint() {
        var printdata = {
          OrderID: this.waybillinfo.ID,
          IsSignBack: false,
          MonthlyCard: this.waybillinfo.ThirdPartyCardNo,// 第三方月结卡号
          ShipperCode: this.waybillinfo.CarrierName,//承运商名称, //this.modelstype, //快递公司
          ExPayType: this.waybillinfo.ExPayType, //this.IGetExpType, //货运类型
          ExpType: this.waybillinfo.Extype,
          Sender: {
            //发件人
            Company: this.waybillinfo.Sender.Company,
            Contact: this.waybillinfo.Sender.Name,
            Mobile: this.waybillinfo.Sender.Mobile,
            Tel: "",
            Province: "广东省",
            City: "深圳市",
            Region: "龙岗区",
            Address: this.waybillinfo.Sender.Address//this.waybillinfo.Sender.Address //暂时调用收货人地址
          },
          Receiver: {
            //收件人
            Company: this.waybillinfo.Receiver.Company,
            Contact: this.waybillinfo.Receiver.Name,
            Mobile: this.waybillinfo.Receiver.Mobile,
            Address: this.waybillinfo.Receiver.Address
          },
          //Quantity: this.waybillinfo.TotalParts, //数量
          //Remark: '', //备注
          //volume: this.waybillinfo.TotalWeight, //重量
          //Weight: this.waybillinfo.TotalWeight, //体积
          Commodity: [{ GoodsName: '客户器件' }] //
        }
        if (this.waybillinfo.Code != null && this.waybillinfo.Code != '') {
          var datas = {
            code: this.waybillinfo.Code
          }
          //ReprintKdnFaceSheet(datas)  //快递鸟方法
          ReprintFaceSheet(datas)
        } else {
          // alert(JSON.stringify(printdata))
          //PrintKdn(printdata)          //快递鸟方法
          PrintFaceSheet(printdata)
        }
      },
      fileprint(printurl) {
        var configs = GetPrinterDictionary()
        var getsetting = configs['文档打印']
        getsetting.Url = printurl
        var data = getsetting
        FilePrint(data)
      },
      clackFilesProcess(url) {
        var data = {
          Url: url
        }
        FilesProcess(data)
      },
      //预览出库单送货单
      preview_btn() {
        this.preview = true;
      },
      // 获取客户等级
      Getclientdata(entdata) {
        Getclientdata(entdata).then(res => {
          console.log(res)
          this.clientGrade = res.obj.Grade
        })
      },
      //操作日志的展示
      logchange() {
        this.showlogged = true
        this.loggdetime = new Date().getTime()
      },
      //提交修改后的承运商
      ok_changeexpress() {
        if (this.$refs.Express.expressdata.CarrierID != null && this.$refs.Express.expressdata.Expresstype != null && this.$refs.Express.expressdata.PayTypes != null) {
          if (this.$refs.Express.expressdata.PayTypes == 4 && this.$refs.Express.expressdata.ThirdPartyCardNo == null) {
            this.$Message.error('请输入必填项')
            this.showchangeexpress = true
          }
          else {
            this.showchangeexpress = false
            this.$refs.Express.UpdateWaybillExpress()
          }
        } else {
          this.$Message.error('请输入必填项')
          this.showchangeexpress = true
        }
      },
      uploadwaybill() {
        CgPickingsDetail(this.wayBillID).then(res => {
          this.waybillinfo = res.Waybill;
        })
      },
      clickshowcode() {
        this.showchangewaybillcode = true
        this.changewaybillcodeinput = this.waybillinfo.Code
      },
      //修改运单号接口
      ok_changecode() {
        if (!this.changewaybillcodeinput == false) {
          var data = {
            "WaybillID": this.waybillinfo.ID, // 运单ID
            "Code": this.changewaybillcodeinput // 运单号
          }
          ModifyWaybillCode(data).then(res => {
            if (res.success == true) {
              this.waybillinfo.Code = this.changewaybillcodeinput
              this.changewaybillcodeinput = null
              this.showchangewaybillcode = false
            } else {
              this.$Message.error(res.data);
            }

          }).catch((error) => {
            console.log(error.response.data);
            this.$Message.error(error.response.data.data);
          });
        } else {
          this.$Message.error({ content: '请输入运单号', duration: 10 });
        }

      },
      // 取消运单号的修改
      cancel_changecode() {
        this.changewaybillcodeinput = null
        this.showchangewaybillcode = false
      },
      //修改收件人部分
      clickshoReceiver() {
        this.showchangeReceiver = true
        this.Receiverdata.WaybillID = this.waybillinfo.ID
        this.Receiverdata.Address = this.waybillinfo.CoeAddress
        this.Receiverdata.Name = this.waybillinfo.Receiver.Name
        this.Receiverdata.Mobile = this.waybillinfo.Receiver.Mobile
      },
      ok_changeReceiver() {
        console.log(this.Receiverdata)
        if (!this.Receiverdata.Address == false && !this.Receiverdata.Name == false && !this.Receiverdata.Mobile == false) {
          ModifyWaybillConsigneeInfo(this.Receiverdata).then(res => {
            if (res.success == true) {
              this.$Message.success('修改成功')
              this.waybillinfo.CoeAddress = this.Receiverdata.Address
              this.waybillinfo.Receiver.Name = this.Receiverdata.Name
              this.waybillinfo.Receiver.Mobile = this.Receiverdata.Mobile
              console.log(this.Receiverdata.Address)

              this.Receiverdata.Name = null
              this.Receiverdata.Mobile = null
              this.showchangeReceiver = false
            } else {
              this.$Message.error(res.data);
            }
          }).catch((error) => {
            this.$Message.error(error.response.data.data)
          });
        } else {
          this.$Message.error('信息不能为空')
        }

      },
      cancel_changeReceiver() {
        this.Receiverdata.Address = null
        this.Receiverdata.Name = null
        this.Receiverdata.Mobile = null
        this.showchangeReceiver = false
      }
    },
  };
</script>
