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
  padding-top: 15px;
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
  width: 100px;
}
.detail_tablebox >>> .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.detail_tablebox >>> .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.detail_tablebox /deep/ .ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.detail_tablebox /deep/.ivu-table-cell {
  padding-left: 5px;
  padding-right: 5px;
}
.sethover {
  cursor: pointer;
}
.itembox >>> .ivu-select-input {
  font-size: 14px !important;
}
.itembox >>> .detail_tablebox >>> .ivu-input-wrapper {
  font-size: 14px !important;
}
.itembox span {
  font-size: 14px !important;
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
  <div>
    <div style="width:100%;font-size:12px;">
      <div class="itembox">
        <p class="detailtitle">基础信息</p>
        <!-- <p>{{getids}}</p> -->
        <div style="width:100%;min-height:200px;background:#f5f7f9;margin:15px 0">
          <Row>
            <Col style="width: 21%;float: left;">
            <ul class="detail_li detail1">
              <li class="itemli">
                <span class="detail_title1">ID：</span>
                <span>{{details.waybillinfo.ID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">订单号：</span>
                <span style="line-height: 20px;width: 70%;display: inline-block;">{{details.waybillinfo.OrderID}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状态：</span>
                <span>
                  {{details.waybillinfo.ExcuteStatusDes}}
                  <Button v-if="details.waybillinfo.ID!=null" icon='md-calendar' type="success" size='small' @click="logchange">日志管理</Button>
                </span>
              </li>
              <li class="itemli" v-if="details.waybillinfo.Type==1">
                <span class="detail_title1">提货状态：</span>
                <Tag color="magenta" v-if="details.waybillinfo.Type&&details.waybillinfo.LoadingExcuteStatus==100">等待提货</Tag>
                <Tag color="blue" v-if="details.waybillinfo.Type==1&&details.waybillinfo.LoadingExcuteStatus==105">提货中</Tag>
                <Tag color="green" v-if="details.waybillinfo.Type==1&&details.waybillinfo.LoadingExcuteStatus==200">提货完成</Tag>
              </li>
              <li class="itemli">
                <span class="detail_title1">业务类型：</span>
                <span>{{details.waybillinfo.SourceDes}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">客服人员：</span>
                <span>{{details.waybillinfo.Merchandiser}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1" style="width:112px">是否收取入仓费：</span>
                <span v-bind:class="{ activecolor: details.waybillinfo.ChargeWH=='不收取' }"> 
                  <Tag color="red" v-if="details.waybillinfo.ChargeWH=='不收取'">{{details.waybillinfo.ChargeWH}}</Tag>
                  <em v-else>{{details.waybillinfo.ChargeWH}}</em>
                </span>
              </li>
              <li class="itemli" v-if="details.waybillinfo.Condition!=undefined">
                <Icon type="md-alert"
                      v-if="Conditionstype"
                      style="font-size: 22px;color: #da2828;" />
                <Tag color="geekblue" v-if="details.waybillinfo.Condition.AgencyCheck==true">代检查</Tag>
                <Tag color="purple" v-if="details.waybillinfo.Condition.AgencyPayment==true">代垫货款</Tag>
                <Tag color="blue" v-if="details.waybillinfo.Condition.AgencyReceive==true">代收货款</Tag>
                <Tag color="cyan" v-if="details.waybillinfo.Condition.ChangePackaging==true">代收货款</Tag>
                <Tag color="green" v-if="details.waybillinfo.Condition.LableServices==true">标签服务</Tag>
                <Tag color="gold" v-if="details.waybillinfo.Condition.PayForFreight==true">垫付运费</Tag>
                <Tag color="orange" v-if="details.waybillinfo.Condition.Repackaging==true">重新包装</Tag>
                <Tag color="volcano" v-if="details.waybillinfo.Condition.UnBoxed==true">拆箱验货</Tag>
                <Tag color="red" v-if="details.waybillinfo.Condition.VacuumPackaging==true">真空包装</Tag>
                <Tag color="magenta" v-if="details.waybillinfo.Condition.WaterproofPackaging==true">防水包装</Tag>
                <Tag color="lime" v-if="details.waybillinfo.Condition.QualityInspection==true">质检</Tag>
                <Tag color="yellow" v-if="details.waybillinfo.Condition.Unboxing==true">拆包装</Tag>
              </li>
            </ul>
            </Col>
            <Col style="width: 26%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title2">制单时间：</span>
                  <span>{{details.waybillinfo.CreateDate|showDateexact}}</span>
                </li>
                 <li class="itemli" v-if="details.waybillinfo.Type==1">
                  <span class="detail_title2">提货时间：</span>
                  <span>{{details.waybillinfo.TakingDate|showDateexact}}</span>
                </li>
                <li class="itemli" v-else>
                  <span class="detail_title2">到货时间：</span>
                  <span>{{details.waybillinfo.CreateDate|showDateexact}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title2">供应商：</span>
                  <span style="display: inline-block;max-width: 300px; line-height: 1;">
                    <p>{{details.waybillinfo.Supplier}}
                      <Tag color="volcano">LV {{details.waybillinfo.SupplierGrade}}</Tag>
                    </p>
                  </span>
                  <br />
                </li>
                <li class="itemli">
                  <span class="detail_title2">到货方式：</span>
                  <span v-if="details.waybillinfo.Type==1||details.waybillinfo.Type==2">{{details.waybillinfo.TypeDes}}</span>
                  <Select
                      v-else
                      v-model="details.waybillinfo.Type"
                      style="width:40%"
                      @on-change='changewaybillType'
                    >
                      <Option
                        v-for="(item,index) in TypeArr"
                        :value="item.value"
                        :key="index"
                      >{{ item.label}}</Option>
                    </Select>
                  <!-- <a v-if="Sortings.length>0" href="javascript:void(0)" @click="showhistory">历史到货</a> -->
                  <a href="javascript:void(0)" @click="showhistory">历史到货</a>
                </li>

                <li class="itemli" v-if="details.waybillinfo.IsPayCharge==true">
                   <span class="detail_title2">代付货款：</span>
                   <span style="display: inline-block;">是</span>
                </li>
                <li class="itemli" v-if="ispay!=null">
                   <span class="detail_title2">付款状态：</span>
                   <span style="display: inline-block;">{{ispay}}</span>
                </li>


                <li class="itemli" v-if="details.waybillinfo.Type==1">
                  <span class="detail_title2">提货文件：</span>
                  <span style="display: inline-block;">
                    <div v-for="(item,index) in details.waybillinfo.Files" v-if="item.Type==10" style="line-height: 1;">
                      <p >
                        <!-- <span>{{item.CustomName}}</span> -->
                        <span class="linkurlcolor Filesbox" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                        <a @click="fileprint(item.Url)">打印</a>
                      </p>
                    </div>
                  </span>
                </li>
              </ul>
            </Col>
            <Col style="width: 34%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <span class="detail_title3">入&nbsp;&nbsp;&nbsp;&nbsp;仓&nbsp;&nbsp;&nbsp;&nbsp;号：</span>
                  <span>{{details.waybillinfo.EnterCode}}</span>
                  <em>({{details.waybillinfo.ClientName}})</em>
                  <Tag color="geekblue">LV {{clientGrade}}</Tag>
                </li>
                <li class="itemli" v-if="details.waybillinfo.Type!=1">
                  <span class="detail_title3">运单号(本次)：</span>
                  <span>
                    <Input style="width:60%" v-model="details.waybillinfo.Code" />
                  </span>
                </li>
                <li class="itemli">
                  <span class="detail_title3" style=" display: inline-block;width: 100px;"><em v-if="details.waybillinfo.Type==1" class="Mustfill">*</em>
                   <i v-if="details.waybillinfo.Type==3">快递公司(本次):</i>
                   <i v-else>承运商(本次):</i>
                  </span>
                  <span>
                    <Select
                      v-model="details.waybillinfo.CarrierID"
                      style="width:60%"
                      filterable
                      @on-change="changeacrrier"
                      :label-in-value="true"
                      :disabled='details.waybillinfo.ExcuteStatus==120?true:false'
                    >
                      <Option
                        v-for="(item,index) in CarrierList"
                        :value="item.ID"
                        :key="index"
                      >{{ item.Name }}</Option>
                    </Select>
                    <!-- <Icon type="md-add" @click="showaddCarrier=true" v-if="details.waybillinfo.Type==1"/> -->
                    <Icon type="md-add" @click="showaddCarrier=true" v-if="details.waybillinfo.ExcuteStatus!=120&&details.waybillinfo.Type==1&&details.waybillinfo.CarrierID!='Personal'"/>
                    <Icon type="md-add" @click="showaddcontacts=true" v-if="details.waybillinfo.ExcuteStatus!=120&&details.waybillinfo.Type==3&&details.waybillinfo.CarrierID!='Personal'"/>
                  </span>
                </li>

                <li class="itemli" v-if="details.waybillinfo.Type==1&&IsPersonal==false">
                  <span class="detail_title3"><em v-if="details.waybillinfo.Type==1" class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                  <span>
                    <Select
                      v-model="details.waybillinfo.Driver"
                      style="width:60%"
                      :label-in-value="true"
                      transfer
                      filterable
                      @on-change="changedrivers"
                      :disabled='details.waybillinfo.ExcuteStatus==120?true:false'
                    >
                      <Option
                        v-for="(item,index) in DriversArr"
                        :value="item.Name"
                        :key="index"
                      >{{ item.Name }}</Option>
                    </Select>
                     <Icon type="md-add" @click="showaddDriver=true" v-if="details.waybillinfo.Type==1&&details.waybillinfo.CarrierID!=null"/>
                  </span>
                </li>
                <li class="itemli" v-if="details.waybillinfo.Type==1&&IsPersonal==false">
                  <span class="detail_title3"><em v-if="details.waybillinfo.Type==1" class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                  <span>
                    <Select
                      transfer
                      v-model="details.waybillinfo.CarNumber1"
                      style="width:60%"
                      :label-in-value="true"
                      @on-create="handleCreate3"
                      @on-change="changedrivers"
                      :disabled='details.waybillinfo.ExcuteStatus==120?true:false'
                    >
                      <Option
                        v-for="item in CarArr"
                        :value="item.value"
                        :key="item.value"
                      >{{ item.value }}</Option>
                    </Select>
                    <Icon type="md-add" @click="showaddCar=true" v-if="details.waybillinfo.Type==1&&details.waybillinfo.CarrierID!=null"/>
                  </span>
                </li>
                <li class="itemli" v-if="details.waybillinfo.Type==1&&IsPersonal==true">
                   <span class="detail_title3"><em class="Mustfill">*</em>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机:</span>
                   <span>
                     <Input v-model="details.waybillinfo.Driver" style="width:60%" placeholder="请输入司机"/>
                   </span>
                </li>
                <li class="itemli"  v-if="details.waybillinfo.Type==1&&IsPersonal==true">
                  <span class="detail_title3"><em class="Mustfill">*</em>车&nbsp;&nbsp;牌&nbsp;&nbsp;号:</span>
                  <span>
                    <Input v-model="details.waybillinfo.CarNumber1" style="width:60%" placeholder="请输入承运商" />
                  </span>
                </li>
                <li class="itemli"  v-if="details.waybillinfo.Type==1">
                  <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>联&nbsp;&nbsp;系&nbsp;&nbsp;人:</span>
                   <span> {{details.waybillinfo.TakingContact}} &nbsp;&nbsp;(电话:{{details.waybillinfo.TakingPhone}})</span>
                </li>
                <li class="itemli"  v-if="details.waybillinfo.Type==1">
                  <span class="detail_title3"><em class="Mustfill">&nbsp;&nbsp;</em>提货地址:</span>
                   <span>{{details.waybillinfo.TakingAddress}}</span>
                </li>
                <li class="itemli">
                  <span class="detail_title3"><em class="Mustfill"></em> <i v-if="details.waybillinfo.Type==1">&nbsp;&nbsp;</i> 输&nbsp;&nbsp;&nbsp;&nbsp;送&nbsp;&nbsp;&nbsp;&nbsp;地：</span>
                  <span>
                    <!-- <Input style="width:60%" v-model="details.Conveyorsite" /> -->
                    <span
                      v-if="details.waybillinfo.ConsignorPlace!=0"
                    >{{details.waybillinfo.ConsignorPlaceText}}</span>
                    <span v-else style="color:red;">暂无输送地</span>
                    <Icon
                      class="sethover"
                      @click="clickClient(details.waybillinfo.ConsignorPlace,'ClientCode')"
                      type="md-create"
                    />
                  </span>
                </li>
              </ul>
            </Col>
            <Col style="width: 19%;float: left;">
              <ul class="detail_li">
                <li class="itemli">
                  <!-- <img-test ref="allimg" :type="1"></img-test> -->

                  <div class="setupload">
                    <Button
                      type="primary"
                      size="small"
                      icon="ios-cloud-upload"
                      @click="SeletUpload('Waybill')"
                    >传照</Button>
                  </div>
                  <div class="setupload">
                    <!-- <Button type="primary" icon="ios-search" @click="photoing('waybill')">拍照</Button> -->
                    <Button
                      type="primary"
                      size="small"
                      icon="md-reverse-camera"
                      @click="fromphotos('Waybill')"
                    >拍照</Button>
                  </div>
                </li>
                <li style="clear: both;">
                  <div v-for="(item,index) in details.waybillinfo.Files" >
                    <p class="Filesbox" v-if="item.Type==8000">
                      <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                      <Tooltip content="删除" placement="top">
                         <Icon type="ios-trash-outline" @click.native="handleRemove(item,'Waybill')"></Icon>
                      </Tooltip>
                      <Tooltip content="打印" placement="top">
                          <Icon type="ios-print-outline" @click="fileprint(item.Url)"/>
                      </Tooltip>
                    </p>
                  </div>
                </li>
                <li
                  style="position: absolute; top: 147px; right: 0;"
                  v-if="details.waybillinfo.Type==1&&details.waybillinfo.LoadingExcuteStatus==100"
                >
                  <Button type="primary" :disabled="TakeGoodsDisable==true?false:true"  @click="TakeGoods">我去提货</Button>
                </li>
              </ul>
            </Col>
          </Row>
        </div>
      </div>
      <div class="itembox">
        <!-- 未通知产品信息录入  开始-->
        <Modal
          v-model="abnormal_enter"
          title="无通知产品录入"
          width="90"
          ref="modal"
          @on-ok="ok_abnormal"
          @on-cancel="cancel"
          @on-visible-change="changeabnormal_enter"
        >
            <div>
              <div>
                <p class="detailtitle">
                  无通知产品录入
                  <span style="margin-left: 15px;color:red;">注意：仅能录入该运单下与通知型号不匹配的产品信息</span>
                </p>
                <div style="padding:15px 0px;">
                  <Noting-Enter :key="timer" ref="NotingEntername" :Storagesarr="Storages" :Storehouselist="Storehouselist" :Boxingarr='boxingarr' :status='1'></Noting-Enter>
                </div>
              </div>
            </div>
        </Modal>

        <!-- 未通知产品信息录入 结束 -->
        <p class="detailtitle">通知产品清单</p>
        <div style="margin:15px 0">
          <ButtonGroup style="width:28%">
            <Input
              v-model.trim="searchkey"
              placeholder="请输入品牌或型号"
              clearable
              @on-clear="search_pro"
              style="width:80%;float:left;position: relative;left: 3px"
            />
            <Button style="float:left" @click="search_pro" type="primary">筛选</Button>
          </ButtonGroup>
          <!-- <Button type="primary" @click="testbtn('Waybill202006050023')">转报关测试</Button> -->
          <Button type="primary" @click="Labelprint" :disabled="isdisabled==true?true:false">标签打印</Button> 
          <Button type="primary" @click="SetStorehouse" :disabled="isdisabled==true?true:false">一键入库</Button>
          <Button type="primary" @click="showBudget('meet','in',details.waybillinfo.Source)">收入</Button>
          <Button type="primary" @click="showBudget('meet','out',details.waybillinfo.Source)">支出</Button>
          <Button type="primary" @click="abnormal_enter=true" :disabled="isdisabled==true?true:false">无通知产品录入</Button>
          <Button type="primary" @click="showshare=true" :disabled="isdisabled==true?true:false">分摊重量</Button>
          <div style="text-align: right;" v-if="details.waybillinfo.OStatus!=200">
            <Icon type="md-sync" @click="sync_btn" class="sync_btn hoverbtn"/>
            <div style="display: inline-block;">
              <Button type="primary" shape="circle" icon="md-checkmark" @click="finish_btn1" :disabled="isdisabled==true?true:false">入库完成</Button>
              <Button type="warning" shape="circle" icon="ios-alert-outline" @click="Abnormal" :disabled="isdisabled==true?true:false">到货异常</Button>
            </div>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table
              ref="selection"
              :loading="loading"
              :columns="details.columns1"
              :data="details.detailitem"
              size="large"
              @on-selection-change="handleSelectRow"
              @on-row-click="tabledblclick"
            >
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <template slot-scope="{ row, index }" slot="Arrival">
                <span v-if="row.iscx!=false">{{row.Quantity}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Already">
                <span v-if="row.iscx!=false">{{row.ArrivedQuantity}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Surplus">
                  <P v-if="row.iscx!=false">
                    <span v-if="Number(row.LeftQuantity)<=0">0</span> 
                    <span v-else>{{row.LeftQuantity}}</span> 
                 </P>
              </template>
              <template slot-scope="{ row, index }" slot="OriginDes">
                <span>{{row.origintext}}</span>
                <Icon class="sethover" v-if="row._disabled!=true" @click="clickClient(row.origin,row)" type="md-create" />
              </template>
              <template slot-scope="{ row, index }" slot="imglist">
                <p v-for="(item,index) in row.Files" class="Filesbox">
                  <span class="linkurlcolor" @click="clackFilesProcess(item.Url)">{{item.CustomName}}</span>
                  <Icon
                    type="ios-trash-outline"
                    :ref="row.ID"
                    @click.native="handleRemove(item,row)"
                  ></Icon>
                </p>
                <!-- <Input v-model="row.typeimg" /> -->
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <div class="setupload">
                  <Button
                    :disabled="row._disabled==true?true:false"
                    type="primary"
                    size="small"
                    icon="ios-cloud-upload"
                    @click="SeletUpload(row)"
                  >传照</Button>
                </div>
                <div class="setupload">
                  <Button
                    :disabled="row._disabled==true?true:false"
                    type="primary"
                    size="small"
                    icon="md-reverse-camera"
                    @click="fromphotos(row)"
                  >拍照</Button>
                </div>
                <div class="setupload" v-if="row.iscx!=false">
                  <Button
                    :disabled="row._disabled==true?true:false"
                    type="primary"
                    size="small"
                    icon="md-checkmark"
                    @click="chaixiang(index,row)"
                  >拆项</Button>
                </div>
                <div class="setupload" v-if="row.iscx==false">
                  <Button
                    :disabled="row._disabled==true?true:false"
                    v-if="row.iscx==false||row.isenter==true"
                    type="primary"
                    size="small"
                    icon="ios-trash-outline"
                    @click="removechaixiang(index,row)"
                  >删除</Button>
                </div>
              </template>
            </Table>
            <div class="successbtn" v-if="details.waybillinfo.OStatus!=200">
              <Button type="primary" icon="md-checkmark" @click="finish_btn1" :disabled="isdisabled==true?true:false">入库完成</Button>
              <Button type="warning" icon="ios-alert-outline" @click="Abnormal" :disabled="isdisabled==true?true:false">到货异常</Button>
            </div>
          </div>
        </div>
      </div>
      <div v-if="WarehousingMsg==true">
        <Modal
          v-model="WarehousingMsg"
          title="确定入库"
          @on-ok="ok_Warehousing(115)"
          @on-cancel="cancel_Warehousing"
        >
          <div v-if="Nomatching.length!=0">
            <!-- <span v-for="(item,index) in Nomatching" :key="index">{{item}}与通知型号不都，是否入库</span> -->
            <h2>以下型号数量与通知数量不符，是否继续进行入库操作</h2>
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
            @click="cancel"
            style="font-size:18px;"
          />
        </div>
        <span style="line-height:26px;">选择库位</span>
        <Select v-model="housenumber" filterable v-if="Storehouselist.length>0">
          <Option
            v-for="(item,index) in Storehouselist"
            :value="item.ShelveID"
            :label="item.ShelveID"
            :key="index"
          >
            <span>{{item.ShelveID}}</span>
            <span style="float:right;color:#ccc">{{item.TotalPackage}}</span>
          </Option>
        </Select>
        <div slot="footer">
          <Button @click="cancel">取消</Button>
          <Button type="primary" @click="changehouse">确定</Button>
        </div>
      </Modal>
      <!-- 一键入库 结束-->

      <!-- 异常到货 开始-->
      <!-- <Modal v-model="isAbnormal" title="到货异常">
        <div slot="close">
          <Icon
            type="md-close-circle"
            color="rgb(33, 28, 28)"
            @click="closeerror"
            style="font-size:18px;"
          />
        </div>
        <span style="line-height:26px;">异常原因</span>
        <Input v-model="Summary" type="textarea" :rows="2" placeholder="备注" maxlength="100" show-word-limit/>
        <div slot="footer">
          <Button @click="closeerror">取消</Button>
          <Button type="primary" @click="Abnormal_btn">确定</Button>
        </div>
      </Modal> -->
      <!-- 异常到货 结束-->
    </div>
    <!-- 历史到货 开始 -->
    <Drawer :width="70" v-model="historydata" @on-visible-change='changehistore'>
      <!-- <Historys-dom :key="historydetail.times" ref="Historygoods"></Historys-dom> -->
      <History-Mount ref='reFresh' v-if="reFresh" v-bind:OrderID='details.waybillinfo.OrderID' v-bind:Type='1'></History-Mount>
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
      <div style="position: absolute;right:20px;z-index:99999;width:30px">
        <Icon type="ios-close" style="float:right;font-size:30px;font-weight:bold;" @click="closeBudget" />
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
    <!-- 分摊重量 开始 -->
    <Modal
        v-model="showshare"
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
    <Modal
        v-model="showaddcontacts"
        title="新增快递公司"
        :mask-closable="false">
        <div slot="close">
          <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(8)"/>
        </div>
        <Add-contacts ref="Addcontacts"  @ok_contacts="ok_contacts" @setdisabled='setdisabled' :Conveyingplace='Conveyingplace'></Add-contacts>
        <div slot="footer">
           <Button @click="ok_addCarrier(8)" :disabled='adddisabled'>取消</Button>
           <Button @click="ok_addCarrier(7)" :disabled='adddisabled' type="primary">确认</Button>
        </div>
    </Modal>
     <!-- 新增快递公司 结束-->
    <!-- 新增承运商 开始 -->
    <Modal
        v-model="showaddCarrier"
        title="新增承运商"
        :mask-closable="false">
        <div slot="close">
          <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(2)"/>
        </div>
        <Add-Carrier ref="addCarrier"  @fatherMethodCarrier="fatherMethodCarrier" @setdisabled='setdisabled' :Conveyingplace='Conveyingplace'></Add-Carrier>
        <div slot="footer">
           <Button @click="ok_addCarrier(2)" :disabled='adddisabled'>取消</Button>
           <Button @click="ok_addCarrier(1)"  :disabled='adddisabled' type="primary">确认</Button>
        </div>
    </Modal>
     <!-- 新增承运商 结束-->
    <!-- 新增司机 开始 -->
     <Modal
        v-model="showaddDriver"
        title="新增司机"
        :mask-closable="false">
        <div slot="close">
          <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(4)"/>
        </div>
        <Add-Driver ref="AddDriver" :EnterpriseName='details.waybillinfo.CarrierName' @setdisabled='setdisabled' @ok_addDriver='ok_addDriver'></Add-Driver>
        <div slot="footer">
           <Button @click="ok_addCarrier(4)" :disabled='adddisabled'>取消</Button>
           <Button @click="ok_addCarrier(3)" :disabled='adddisabled' type="primary">确认</Button>
        </div>
    </Modal>
 <!-- 新增司机 结束 -->
<!-- 新增车牌号 开始 -->
     <Modal
        v-model="showaddCar"
        title="新增车牌号"
        :mask-closable="false">
        <div slot="close">
          <Icon type="md-close" style="font-size:20px;color: #999" @click="ok_addCarrier(6)"/>
        </div>
        <Add-Car ref="addcar" :EnterpriseName='details.waybillinfo.CarrierName' @setdisabled='setdisabled'  @ok_addCar='ok_addCar'></Add-Car>
        <div slot="footer">
           <Button @click="ok_addCarrier(6)" :disabled='adddisabled'>取消</Button>
           <Button @click="ok_addCarrier(5)" :disabled='adddisabled' type="primary">确认</Button>
        </div>
    </Modal>
    <!-- 新增车牌号 i结束-->
    <!-- 操作日志 -->
     <Modal
        v-model="showlogged"
        :footer-hide='true'
        :mask-closable='false'
        width='60'>
        <div slot="close">
          <Icon style="font-size:21px;color:#cccccc;padding-top: 5px" type="ios-close-circle-outline" />
        </div>
        <div slot="header">
          <span style="font-size:18px;color:#1aaff7;">日志管理</span>
        </div>
        <logg-ed ref="logged" :key='loggdetime' v-if="showlogged" :WaybillID='details.waybillinfo.ID'></logg-ed>
    </Modal>
     <!-- <div class="dync mount"></div> -->
    <!-- 操作日志 -->
  </div>
</template>
<script>
import NotingEnter from "@/Pages/Cgenter/NotingEnter";
import HistoryMount from './CgHistory';
import AddCarrier from '../Subassembly/Add_Carrier'
import AddDriver from '../Subassembly/Add_Driver'
import AddCar from '../Subassembly/Add_Car'
import logged from '../Common/logged'
import Addcontacts from '../Subassembly/Add_contacts'
// import Vue from 'vue'
import {
  Noticedetail,
  sortingupload,
  getWayParter,
  History
} from "../../api";
import {
  CgDetail,
  cgenter,
  GetSortingID,
  historyList,
  GetDriversCars,
  search_detail,
  TakeGoods,
  GetUsableShelves,
  CgCarriers,
  CgDeleteFiles,
  Getclientdata,
  DriverAdd,
  TransportAdd,
  IsPayPayments
} from "../../api/CgApi"; //引入api 的接口
import {
  TemplatePrint,
  GetPrinterDictionary,
  FilePrint,
  FormPhoto,
  SeletUploadFile,
  FilesProcess
} from "@/js/browser.js";
let Base64 = require("js-base64").Base64;
let product_url = require("../../../static/pubilc.dev");
import Vue from "vue";
import moment from "moment";
import qs from "qs";
let lodash = require("lodash");
export default {
  name: "RoutineEnter",
  components: {
    "Noting-Enter": NotingEnter,
    "History-Mount":HistoryMount,
    'Add-Carrier':AddCarrier,
    'Add-Driver':AddDriver,
    'Add-Car':AddCar,
    'logg-ed':logged,
    'Add-contacts':Addcontacts
  },
  props: {
   
  },
  data() {
    return {
      ispay:null,//是否收款
      TakeGoodsDisable:false,//自提是否可点击
      oldcrriers:null,//
      loggdetime:'',//操作日志时间
      showlogged:false,//操作日志
      adddisabled:false,//新增禁止选中
      showaddDriver:false,// 是否可以新增司机
      addDriverName:null,//新增司机姓名
      showaddCar:false,//是否可以新增车牌号
      addCarName:null,//新增车牌号名称
      showaddCarrier:false,//是否可以新增承运商
      showaddcontacts:false,//是否可以添加快递
      IsPersonal:false,//是否为个人承运商
      clientGrade:null,//客户等级
      sharevalue:null,//分摊的总重量
      showshare:false,//是否显示分摊重量
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
      boxingarr:[],//箱号
      isdisabled:false,
      timer: "", //当前时间，用于加载子组件
      abnormal_enter: false,
      SortingID: "",
      routername: "",
      Conditionstype: true,
      printurl: product_url.pfwms,
      loading: true, //loading效果
      printlist: [], //打印列表
      Conveyingplace: [], //输送地列表
      Conveyingplace2: [],
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
              return h("span", {}, params.row.Product.PartNumber);
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
                h("span", {}, "品牌")
              ]);
            },
            key: "Manufacturer",
            // width: 80,
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
                      var valus=''
                      if(event.target.value!=''){
                       valus= vm.trim(event.target.value)
                      }else{
                        valus=event.target.value
                      }
                      params.row.Product.Manufacturer = valus;
                      vm.details.detailitem[params.index] = params.row;
                      vm.clicktest(params.row);
                       for (var i = 0; i < vm.details.detailitem.length; i++) {
                        if (vm.details.detailitem[i].PID == params.row.ID) {
                          vm.details.detailitem[i].Product.Manufacturer = event.target.value;
                        }
                      }
                    },
                    "on-blur"(event) {
                        if( event.target.value!=''){
                          params.row.Product.Manufacturer = event.target.value;
                          vm.details.detailitem[params.index] = params.row;
                        }else{
                          event.target.value = null,
                          params.row.Product.Manufacturer =null;
                          vm.details.detailitem[params.index] = params.row;
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
            width: 60
          },
          {
            title: "已到",
            slot: "Already",
            align: "center",
            width: 60
          },
          {
            title: "剩余",
            slot: "Surplus",
            align: "center",
            width: 60
          },
          {            
            title: "封装",
            key: "PackageCase",
            align: "center",
            // width: 80,
            render: (h, params) => {
              var vm = this;
                return h("Input", {
                  props: {
                    //将单元格的值给Input
                    value: vm.trim(params.row.Product.PackageCase) 
                  },
                  on: {
                    "on-change"(event) {
                      //值改变时
                      //将渲染后的值重新赋值给单元格值
                      var valus=''
                      if(event.target.value!=''){
                       valus= vm.trim(event.target.value)
                      }else{
                        valus=event.target.value
                      }
                      params.row.Product.PackageCase = valus;
                      vm.details.detailitem[params.index] = params.row;
                      vm.clicktest(params.row);
                       for (var i = 0; i < vm.details.detailitem.length; i++) {
                        if (vm.details.detailitem[i].PID == params.row.ID) {
                          vm.details.detailitem[i].Product.PackageCase = event.target.value;
                        }
                      }
                    },
                    "on-blur"(event) {
                        if( event.target.value!=''){
                          params.row.Product.PackageCase = event.target.value;
                          vm.details.detailitem[params.index] = params.row;
                        }else{
                          event.target.value = null,
                          params.row.Product.PackageCase =null;
                          vm.details.detailitem[params.index] = params.row;
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
                h("span", {}, "批号")
              ]);
            },
            key: "DateCode",
            align: "center",
            // width: 60,
            render: (h, params) => {
              var vm = this;
              // var newDataecode=params.row.DateCode
              // if(params.row.DateCode!=''||params.row.DateCode!=null){
              //  newDataecode= vm.trim(params.row.DateCode)
              // }else{
              //   newDataecode=params.row.DateCode
              // }
              return h("Input", {
                props: {
                  //将单元格的值给Input
                  value:vm.trim(params.row.DateCode)
                },
                on: {
                  "on-change"(event) {
                    //值改变时
                    //将渲染后的值重新赋值给单元格值
                    var valus=''
                      if(event.target.value!=''||event.target.value!=null){
                       valus= vm.trim(event.target.value)
                      }else{
                        valus=event.target.value
                      }
                    params.row.DateCode = valus;
                    vm.details.detailitem[params.index] = params.row;
                    vm.clicktest(params.row);
                    if(vm.SelectRow.length>0){
                        for(var i=0,lens=vm.SelectRow.length;i<lens;i++){
                          if(vm.SelectRow[i].SortingID==params.row.SortingID){
                              vm.SelectRow[i]=params.row
                          }
                        }
                      }
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
            key: "TruetoQuantity",
            align: "center",
            // width: 50,
            render: (h, params) => {
              var vm = this;
              var prients = null;
              var numbers=''
              if(Number(params.row.CurrentQuantity)<0){
                    numbers=0
              }else{
                numbers=params.row.CurrentQuantity
              }
              return h("Input", {
               
                props: {
                  //将单元格的值给Input
                  value: numbers
                },
                on: {
                  "on-change"(event) {
                    var reg = /^[0-9]\d*$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.CurrentQuantity = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else if (event.target.value <= 0) {
                      vm.$Message.error("数量不能为0");
                      (event.target.value = null),
                        (params.row.CurrentQuantity =null);
                      vm.details.detailitem[params.index] = params.row;
                    } else {
                      vm.$Message.error("只能输入数量");
                      (event.target.value =null),
                        (params.row.CurrentQuantity =null);
                      vm.details.detailitem[params.index] = params.row;
                    }
                    for (var i = 0; i < vm.SelectRow.length; i++) {
                        if (params.row.SortingID == vm.SelectRow[i].SortingID) {
                          vm.SelectRow[i].CurrentQuantity = event.target.value;
                        }
                      }
                  },
                  "on-blur"() {
                    var reg = /^[1-9]\d*$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.CurrentQuantity = event.target.value;
                      vm.details.detailitem[params.index] = params.row;
                      // vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("请输入数量");
                      (event.target.value = null),
                        (params.row.CurrentQuantity = null);
                      vm.details.detailitem[params.index] = params.row;
                    }
                    for (var i = 0; i < vm.SelectRow.length; i++) {
                        if (params.row.SortingID == vm.SelectRow[i].SortingID) {
                          vm.SelectRow[i].CurrentQuantity = event.target.value;
                        }
                      }
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
            slot: "OriginDes",
            align: "left"
            // width:100
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
                h("span", {}, "入库库位")
              ]);
            },
            slot: "ShelveID",
            align: "left",
            width: 170,
            render: (h, params) => {
                var vm = this;
                  return h('Select', {
                    props: {
                      value: params.row.ShelveID, // 获取选择的下拉框的值
                      disabled:params.row._disabled==false?false:true,
                      transfer:true,
                      filterable:true ,
                    },
                    on: {
                      'on-change'(event){
                        params.row.ShelveID=event
                        vm.details.detailitem[params.index] = params.row;
                        vm.setselect(params.row)
                      },
                    }
                  }, this.Storehouselist.map((item) => { // this.productTypeList下拉框里的data
                    return h('Option', { // 下拉框的值
                      props: {
                        value: item.ShelveID,
                        label: item.ShelveID
                      }
                    })
                  }))
              }
          },
          {
            title: "体积(cm³)",
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
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("只能输入数字,包括两位数的小数点");
                      params.row.Volume = null;
                      event.target.value = null;
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
                      params.row.Volume =null;
                      event.target.value = null;
                      vm.details.detailitem[params.index] = params.row;
                    }
                    if(vm.SelectRow.length>0){
                        for(var i=0,lens=vm.SelectRow.length;i<lens;i++){
                          if(vm.SelectRow[i].SortingID==params.row.SortingID){
                              vm.SelectRow[i]=params.row
                          }
                        }
                      }
                  }
                }
              });
            }
          },
          {
            title: "毛重(Kg)",
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
                    // var reg = /^\d+(\.\d{0,2})?$/;
                   var reg = /^[+-]?(0|([1-9]\d*))(\.\d+)?$/g;
                    var newtarget = vm.trim(event.target.value);
                    if (newtarget != "") {
                      if (reg.test(newtarget) == true) {
                        params.row.Weight = newtarget;
                        vm.details.detailitem[params.index] = params.row;
                        vm.clicktest(params.row);
                      } else {
                        vm.$Message.error("只能输入数字,包括两位数的小数点");
                        params.row.Weight = null;
                        event.target.value = null;
                        vm.details.detailitem[params.index] = params.row;
                      }
                      if(vm.SelectRow.length>0){
                        for(var i=0,lens=vm.SelectRow.length;i<lens;i++){
                          if(vm.SelectRow[i].SortingID==params.row.SortingID){
                              vm.SelectRow[i]=params.row
                          }
                        }
                      }
                    }
                  },
                  "on-enter": event => {
                    var reg = /^[+-]?(0|([1-9]\d*))(\.\d+)?$/g;
                    // console.log(event.target.value)
                    var newtarget = vm.trim(event.target.value);
                    // console.log(newtarget)
                    if (reg.test(newtarget) == true) {
                      params.row.Weight = newtarget;
                      vm.details.detailitem[params.index] = params.row;
                      // var Input = params.row.Input;
                      // var StandardProducts = params.row.Product;
                      var data2 = {
                        Quantity: params.row.CurrentQuantity, //数量
                        inputsID: params.row.SortingID, //id
                        Catalog: '', //品名
                        PartNumber: params.row.Product.PartNumber, //型号
                        Manufacturer: params.row.Product.Manufacturer, //品牌
                        Packing:  params.row.Product.Packing, //包装
                        PackageCase: params.row.Product.PackageCase, //封装
                        origin: params.row.origintext,//产地
                        SourceDes:this.details.waybillinfo.SourceDes,//业务
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
            // width: 180
          },
          {
            title: "异常原因",
            key: "imglist",
            align: "center",
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
                    var valus=''
                      if(event.target.value!=''){
                       valus= vm.trim(event.target.value)
                      }else{
                        valus=event.target.value
                      }
                    params.row.Summary = valus;
                    vm.details.detailitem[params.index] = params.row;
                  },
                  "on-blur"(event) {
                    if(vm.SelectRow.length>0){
                        for(var i=0,lens=vm.SelectRow.length;i<lens;i++){
                          if(vm.SelectRow[i].SortingID==params.row.SortingID){
                              vm.SelectRow[i]=params.row
                          }
                        }
                      }
                  },
                  
                }
              });
            }
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 220
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
      Summary: "", //后台提供的备注信息对象
      historydata: false, //历史到货的抽屉
      reFresh:false,//是否显示历史到货组件
      historydetail: {
        //历史到货数据
        times: "", //时间，每次获取新的版本
        waybillLIst: [] //运单列表
      },
      company: "", //入仓号对应公司
      CarrierList: [], //承运商列表
      CarArr: [], //车票号
      CarArrName:'',//车牌号名称
      DriversArr: [], // 司机,
      DriversName:'',//司机名称
      Storages: [],
      Sortings:[], //分拣历史信息
      NullNotice:[]
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
  watch: {
    getuplode() {
      if (getuplode != "") {
        alert(JSON.stringify(getuplode));
      }
      return;
    }
  },
  mounted() {
  
  },
  created() {
    // console.log("重新加载-------常规分拣");
    // this.WayParterdata()
    window["PhotoUploaded"] = this.changed;
    this.routername = this.$route.name;
  },
  // beforeDestroy(){
  //   delete window["PhotoUploaded"]
  // },
  methods: {
    clicktest: lodash.throttle(function(paramsrow) {
      for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
        if (paramsrow.SortingID == this.SelectRow[i].SortingID) {
          this.SelectRow[i] = paramsrow;
        }
      }
    }, 1000),
    setselect(paramsrow){
      for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
        if (paramsrow.SortingID == this.SelectRow[i].SortingID) {
          this.SelectRow[i] = paramsrow;
        }
      }
    },
    tabledblclick(row,index){
      console.log(index)
    },
    handleCreate1(val) {// 添加承运商
      this.CarrierList.push({
        ID: val,
        Name: val
      });
    },
    handleCreate2(val) { //添加司机
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
    trim(str) {
      //去除前后空格
      if(str){
        return str.replace(/(^\s*)|(\s*$)/g, "");
      }
      
    },
    ok() {
      this.$Message.info("Clicked ok");
      this.showphoto2 = false;
    },
    cancel() {
      // this.$Message.info("Clicked cancel");
      this.showphoto2 = false;
      this.setClientCode = false;
      this.abnormal_enter = false;
      this.showshare=false
      this.showaddCarrier=false
      this.showaddDriver=false
      this.showaddCar=false
      this.sethousbox=false
    },
    handleSelectRow(value) {
      //多选事件 获取选中的数据
      this.SelectRow = value;
      //  console.log(this.SelectRow)
    },
    search_pro() {
      this.loading = true;
      search_detail(this.details.waybillinfo.ID, this.searchkey).then(res => {
        this.details.detailitem = res.Notice;
        this.loading = false;
      });
    },
    // 到货异常
    Abnormal() {
      if (this.SelectRow.length <= 0||this.SelectRow.length > 1) {
        this.$Message.error("请选择到货异常的产品，有且只有一条");
      } else {
        if(this.SelectRow[0].CurrentQuantity==null||this.SelectRow[0].CurrentQuantity==''){
            this.$Message.error("请输入大于零的到货数量");
        }else{
          if(this.SelectRow[0].ShelveID==null){
            this.$Message.error("请选择入库库位");
          }else{
            if(this.SelectRow[0].Summary!=null&&this.SelectRow[0].Summary!=''){
            if(this.SelectRow[0].DateCode!=null&&this.SelectRow[0].DateCode!=''){
              this.$Modal.confirm({
                  title: '到货异常',
                  content: '<p>是否进行异常到货分拣</p>',
                  onOk: () => {
                    console.log(this.SelectRow)
                       this.ok_Warehousing(130);
                  },
                  onCancel: () => {
                      // this.$Message.info('Clicked cancel');
                  }
             });
             }else
             {
              this.$Message.error("请输入批号");
             }
            }else{
              this.$Message.error("请输入异常原因");
            }
            // this.isAbnormal = true;
            
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
        this.isAbnormal = false;
        this.ok_Warehousing(130);
      }
    },
    closeerror() {
      //异常到货关闭
      this.isAbnormal = false;
      this.Summary = ""; //备注
    },
    finish_btn1(){  //完成入库按钮1 根据类型判断承运商与车牌号
      if(this.details.waybillinfo.Type==1){
          if (this.details.waybillinfo.CarrierID == '' ) {
              this.$Message.error('请先选择承运商')
            } else if (this.details.waybillinfo.Driver == '') {
              this.$Message.error('请选择司机')
            } else if (this.details.waybillinfo.CarNumber1 == '') {
              this.$Message.error('请选择车牌号')
            } else{
              this.finish_btn()
            }
      }else{
        this.finish_btn()
      }
    },
    finish_btn() {//完成入库按钮1
        if (this.SelectRow.length <= 0) {
        this.$Message.error("请选择要操作的产品项");
      } else {
        bbq: for (var k = 0, len = this.SelectRow.length; k < len; k++) {
          if (
            this.SelectRow[k].LeftQuantity == 0 ||
            this.SelectRow[k].LeftQuantity == ""
          ) {
            this.$Message.error("无通知产品请按照异常到货处理");
            this.WarehousingMsg = false;
            break;
          } else if (this.SelectRow[k].originID == "") {
            this.$Message.error("请选择原产地");
            this.WarehousingMsg = false;
            break;
          } else if(this.SelectRow[k].ShelveID==null||this.SelectRow[k].ShelveID==""){
             this.$Message.error("请选择入库库位");
             this.WarehousingMsg = false;
             break;
          }else if(this.SelectRow[k].Product.Manufacturer==null){
             this.$Message.error("品牌不能为空");
             this.WarehousingMsg = false;
             break;
          }else if(this.SelectRow[k].DateCode==null){
             this.$Message.error("请输入批号");
             this.WarehousingMsg = false;
             break;
          }else {
            if (this.SelectRow[k].CurrentQuantity == ""||this.SelectRow[k].CurrentQuantity == null||this.SelectRow[k].ShelveID==null) {
              this.$Message.error("请输入必填");
              this.WarehousingMsg = false;
              break;
            } else {
               var SelectRowK=k
              function sum(arr) {
                return eval(arr.join("+"));
              }
              var arr = this.SelectRow;
              var Nomatching = [];
              this.Nomatching=[];
              var map = {},
                dest = [];
              for (var i = 0; i < arr.length; i++) {
                var ai = arr[i];
                if (!map[ai.NoticeID]) {
                  dest.push({
                    name: ai.Product.PartNumber,
                    Quantity: ai.LeftQuantity,
                    NoticeID: ai.NoticeID,
                    data: [Number(ai.CurrentQuantity)]
                  });
                  map[ai.NoticeID] = ai.NoticeID;
                } else {
                  for (var j = 0; j < dest.length; j++) {
                    var dj = dest[j];
                    if (dj.NoticeID == ai.NoticeID) {
                      dj.data.push(Number(ai.CurrentQuantity));
                      break;
                    }
                  }
                }
              }
              console.log(dest)
              var that = this;
              for (var i = 0; i < dest.length; i++) {
                var item = dest[i];
                var total = sum(item.data);
                if (item.Quantity == ""||item.Quantity == null) {
                  this.$Message.error("无通知产品请按照异常到货处理");
                  that.WarehousingMsg = false;
                  break bbq;
                } else if (item.Quantity < total) {
                  this.$Message.error("本次到货数量不能大于剩余数量,请按照异常到货进行处理");
                  that.WarehousingMsg = false;
                  break bbq;
                } else {
                  if (item.Quantity == 0) {
                    this.$Message.error("剩余数量为0，不需要提交");
                    that.WarehousingMsg = false;
                    break bbq;
                  } else {
                    if (item.Quantity > total) {
                      Nomatching.push(item.name);
                      that.Nomatching = Nomatching;
                      // that.WarehousingMsg = true;
                      if(SelectRowK==this.SelectRow.length-1){
                          that.WarehousingMsg = true;
                        }
                    } else if (item.Quantity == total) {
                      if(SelectRowK==this.SelectRow.length-1){
                          that.WarehousingMsg = true;
                        }
                      // that.WarehousingMsg = true;
                    }
                  }
                }
              }
            }
          }
        }
      }  
    },
    setWarehousing(data) {
      cgenter(data).then(res => {
        if (res.Success == true) {
          this.loading=true
          if(this.details.waybillinfo.Source==40&&res.Data!=''){
            //  this.$Message.success("入库完成，正在跳转至出库分拣页面");
            //   var that = this;
            //   setTimeout(function() {
            //     that.testbtn(res.Data)
            //   }, 1000);

           this.$Message.success("入库完成");  //暂时不进行跳转，n目的是解决出库未收款的问题
            if(this.isAbnormal==true){
              this.isAbnormal=false;
              this.Summary=''
            }
            var that = this;
            setTimeout(function() {
            that.CgDetail_new(that.details.waybillinfo.ID)
            that.$refs.selection.selectAll(false)
            that.SelectRow=[];
            }, 100);


          }else{
            this.$Message.success("入库完成");
            if(this.isAbnormal==true){
              this.isAbnormal=false;
              this.Summary=''
            }
            var that = this;
            setTimeout(function() {
            that.CgDetail_new(that.details.waybillinfo.ID)
            that.$refs.selection.selectAll(false)
            that.SelectRow=[];
            }, 100);
          }
          
        } else {
          this.$Message.error(res.Data);
          this.isdisabled=false;
          if(this.isAbnormal==true){
              this.isAbnormal=false;
              this.Summary=''
            }
            var that = this;
            setTimeout(function() {
            that.CgDetail_new(that.details.waybillinfo.ID)
            that.$refs.selection.selectAll(false)
            that.SelectRow=[];
            }, 100);
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
    ok_Warehousing(type,datatype) {
      this.isdisabled=true;
      this.WarehousingMsg = false;
      var Summary_txt = "";
      //点击确定按钮，进行入库操作
      var deatilarr = null
      if(datatype=='enter'){
        deatilarr=this.NullNotice
      }else{
        deatilarr=this.SelectRow
      }
      var newarr = [];
      for (var i = 0, len = deatilarr.length; i < len; i++) {
        if (deatilarr[i].Quantity != "" || deatilarr[i].Quantity != null) {
          // if (type == 130&&datatype!='enter') {
          //     Summary_txt = this.Summary;
          // }else{
          //   Summary_txt=deatilarr[i].Summary
          // }
          Summary_txt=deatilarr[i].Summary
          var data = {
            NoticeID: deatilarr[i].NoticeID,
            SortingID: deatilarr[i].SortingID,
            StorageID: "",
            InputID: deatilarr[i].InputID,
            BoxCode: deatilarr[i].BoxCode,
            Quantity: deatilarr[i].CurrentQuantity,
            Weight: deatilarr[i].Weight,
            NetWeight: deatilarr[i].NetWeight,
            Volume: deatilarr[i].Volume,
            ShelveID: deatilarr[i].ShelveID,
            DateCode: deatilarr[i].DateCode,
            Origin: deatilarr[i].origin,
            Summary: Summary_txt,
            Product: {
              PartNumber: deatilarr[i].Product.PartNumber,
              Manufacturer: deatilarr[i].Product.Manufacturer,
              PackageCase: deatilarr[i].Product.PackageCase,
              Packaging: deatilarr[i].Product.Packaging
            },
            Files:deatilarr[i].Files
          };
          newarr.push(data);
        }
      }
      var newcode=null;
      if(this.details.waybillinfo.Code==''||this.details.waybillinfo.Code==null){
          newcode=null
        }else{
          newcode=this.details.waybillinfo.Code
        }
      var obj = {
        Waybill: {
          ID: this.details.waybillinfo.ID,
          EnterCode: this.details.waybillinfo.EnterCode,
          ExcuteStatus: type, //异常130  部分入库115
          Type: this.details.waybillinfo.Type,
          Source: this.details.waybillinfo.Source,
          OrderID: this.details.waybillinfo.OrderID,
          NoticeType: this.details.waybillinfo.NoticeType,
          CarrierID: this.details.waybillinfo.CarrierID,
          ConsignorID: this.details.waybillinfo.ConsignorID,
          ConsignorPlace: this.details.waybillinfo.ConsignorPlace,
          TransferID: this.details.waybillinfo.TransferID, //代转运需要该值,用于发出库通知
          Summary: Summary_txt,
          Driver: this.details.waybillinfo.Driver,
          CarNumber1: this.details.waybillinfo.CarNumber1,
          Files: this.details.waybillinfo.Files,
          LoadingExcuteStatus: this.details.waybillinfo.LoadingExcuteStatus,
          Code:newcode,
          Supplier:this.details.waybillinfo.Supplier,
          Type:this.details.waybillinfo.Type,
        },
        Sortings: newarr,
        AdminID: sessionStorage.getItem("userID")
      };
      this.setWarehousing(obj);
    },
    cancel_Warehousing() {
      //点击取消按钮，取消入库
      this.WarehousingMsg = false;
      // this.$refs.selection.selectAll(false);
    },
    //拆箱分拣
    chaixiang(index, row) {
      GetSortingID().then(res => {
        var chaixiangdata = {
          AVGWeight:row.AVGWeight,
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
          LeftQuantity: row.LeftQuantity,
          CurrentQuantity: null,
          Conditions: row.Conditions,
          Source: row.Source,
          Weight: null,
          NetWeight: row.NetWeight,
          Volume: row.Volume,
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
          _disabled: false
        };
        this.details.detailitem.splice(index + 1, 0, chaixiangdata);
        this.$refs.selection.selectAll(false);
        this.SelectRow=[]
        this.details.detailitem[index].CurrentQuantity=null
        this.details.detailitem[index].Weight=null
      });
    },
     //移除拆项
    removechaixiang(index) {
      this.details.detailitem.splice(index, 1);
    },
    // 初始化重构数据
    CgDetail_new(id) {
      if (id != undefined) {
        CgDetail(id).then(res => {
          // console.log(res)
          this.Getclientdata(res.Waybill.EnterCode)
          this.details.waybillinfo = res.Waybill;
          this.details.detailitem = res.Notice;
          this.Sortings=res.Sortings
          this.loading = false;
          
          if(res.Waybill.ExcuteStatus==120){
            this.isdisabled=true;
          }else{
            // this.isdisabled=false;
            if(res.Waybill.Type==1&&res.Waybill.LoadingExcuteStatus==100){
                this.isdisabled=true;
            }else{
                this.isdisabled=false;
            }
          }

          if(this.details.waybillinfo.IsPayCharge==true){
            this.IsPayPayments(this.details.waybillinfo.OrderID)
          }else{
            this.TakeGoodsDisable=true;
          }
          var housid=sessionStorage.getItem("UserWareHouse")
          this.Carriers(this.details.waybillinfo.Type,housid)
          if(res.Waybill.CarrierID=='Personal'){
            this.IsPersonal=true
          }else{
             this.GetDriversCars(res.Waybill.CarrierID);
             this.IsPersonal=false
          }
          this.CarArrName=res.Waybill.CarNumber1
          this.DriversName=res.Waybill.Driver
        });
      }

      this.GetUsableShelves(); //调用可用库位号
    },
    //修改到货方式
    changewaybillType(value){
         var housid=sessionStorage.getItem("UserWareHouse")
        this.Carriers(value,housid)
        if(this.CarrierList.length<=0){
          this.details.waybillinfo.CarrierID=''
        }else{
            this.details.waybillinfo.CarrierID=this.oldcrriers
        }
    },
    //查询可用库位的库位编号
    GetUsableShelves() {
      var id = sessionStorage.getItem("UserWareHouse");
      GetUsableShelves("HK").then(res => {
        this.Storehouselist = res.obj;
        // this.$refs.NotingEntername.Storehouselist=this.Storehouselist;
        // console.log(this.$refs.NotingEntername)
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
      var SelectRows = this.SelectRow;
      var detaiitem = this.details.detailitem;
      for (var i = 0; i < detaiitem.length; i++) {
        for (var j = 0; j < SelectRows.length; j++) {
          if (detaiitem[i].InputID == SelectRows[j].InputID) {
            detaiitem[i].ShelveID = this.housenumber;
          }
        }
      }
      this.SelectRow = [];
      this.sethousbox = false;
      this.$refs.selection.selectAll(false);
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
            inputsID: arr[i].Input.SortingID, //id
            Catalog: arr[i].Product.Catalog, //品名
            PartNumber: arr[i].Product.PartNumber, //型号
            Manufacturer: arr[i].Product.Manufacturer, //品牌
            Packing: arr[i].Product.Packing, //包装
            PackageCase: arr[i].Product.PackageCase, //封装
            origin: arr[i].origintext,//产地
            SourceDes:this.details.waybillinfo.SourceDes,//业务
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
    // 展示历史到货
    showhistory(name) {
      this.historydata = true;
      this.reFresh=true;
      // if(this.reFresh==true){
      //  this.$refs.reFresh.changeTabs("0")
      // }
    },
    // 历史到货组件发生变化的时候
    changehistore(value){
      if(value==false){
        this.reFresh=false
      }else{
        //  this.$refs.reFresh.changeTabs("0")
      }
    },
    //输送地列表
    WayParterdata() {
      getWayParter().then(res => {
        this.Conveyingplace = res.obj;
      });
    },
    // 承运商列表
    Carriers(type,houseid) {
      CgCarriers(type,houseid,100).then(res => {
        this.CarrierList = res;
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
        alert(JSON.stringify(getsetting));
        TemplatePrint(data);
      }
    },
    changConveyingplace(value, row,index) {//改变库位号
    if(value!=undefined){
      // this.details.detailitem[index].ShelveID=value.value
      // this.clicktest(this.details.detailitem[index]);
      // this.SelectRow=[];
      // this.$refs.selection.selectAll(false)
      //  for (var i = 0, lens = this.SelectRow.length; i < lens; i++) {
      //   if (this.details.detailitem[index].SortingID == this.SelectRow[i].SortingID) {
      //       this.SelectRow[i] = this.details.detailitem[index];
      //     }
      //   }

     }
      
    },
    showBudget(type, Budget,Source) {
      //收支明细展开
      this.$store.dispatch("setBudget", true);
      if (type == "meet") {
        var namemeet = "";
        if (this.routername == "CgUntreated") {
          namemeet = "/CgUntreated/Income";
        } else {
          namemeet = "/CgProcessed/Income";
        }
        this.$router.push({
          path: namemeet,
          query: {
            webillID: this.details.waybillinfo.ID,
            OrderID:this.details.waybillinfo.OrderID,
            type: Budget,
            otype:"in",
            conduct:Source
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

      // this.$router.push({path:'/Cgenter/1/Budget',query: { province:1,city:1}})
    },
    closeBudget() {
      //收支明细关闭
     
      this.$router.go(-1);
      this.$store.dispatch("setBudget", false);
    },
    clickClient(value, type) {
      //显示更改输送地与原产地的方法

      this.setClientCode = true;
      this.chengevalue.value = value;
      this.chengevalue.type = type;
      this.Conveyingplace2 = this.Conveyingplace;
      this.ClientCode = value;
    },
    changClientCode(value) {
      //地址改变的时候,保留改变后的地址
      // console.log(value)
      // console.log(this.chengevalue)
      this.chengevalue.inputval = value;
    },
    primaryClientCode() {
      //确认更改地址
       if (this.chengevalue.type == "ClientCode") {
        this.details.waybillinfo.ConsignorPlace = this.chengevalue.inputval.value;
        this.details.waybillinfo.ConsignorPlaceText = this.chengevalue.inputval.label;
      } else {
        this.chengevalue.type.originDes = this.chengevalue.inputval.label;
          for (var j = 0, l = this.details.detailitem.length; j < l; j++) {
            if (this.chengevalue.type.SortingID == this.details.detailitem[j].SortingID) {
              this.details.detailitem[j].origin = this.chengevalue.inputval.value;
              this.details.detailitem[j].origintext = this.chengevalue.inputval.label;
            }
            this.details.detailitem[j]._checked=false
         }
       
         if(this.SelectRow.length>0){
              for(var i=0,len=this.SelectRow.length;i<len;i++){
                for(var j = 0, l = this.details.detailitem.length; j < l; j++){
                  if(this.SelectRow[i].SortingID==this.details.detailitem[j].SortingID){
                      this.details.detailitem[j]._checked=true
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
      // console.log(this.details.waybillinfo);
    },
    fileprint(printurl) {
      var configs = GetPrinterDictionary()
      var getsetting = configs['文档打印']
      getsetting.Url = printurl
      var data = getsetting
      FilePrint(data)
    },
    fromphotos(type) {
      if (type == "Waybill") {
        var data = {
          SessionID: this.details.waybillinfo.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.details.waybillinfo.ID,
            WsOrderID:this.details.waybillinfo.OrderID,
            Type:8000
          }
        };
        FormPhoto(data);
      } else {
        var data = {
          SessionID: type.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.details.waybillinfo.ID,
            WsOrderID:this.details.waybillinfo.OrderID,
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
    testfunction(message) {//winfrom返回拍照信息后，前台处理数据的方法
      var imgdata = message[0];
      var newfile = {
        CustomName: imgdata.FileName,
        ID: imgdata.FileID,
        Url: imgdata.Url,
        Type: 8000
      };
      if (imgdata.SessionID == this.details.waybillinfo.ID) {
        this.details.waybillinfo.Files.push(newfile);
      } else {
        for (var i = 0; i < this.details.detailitem.length; i++) {
          if (this.details.detailitem[i].ID == imgdata.SessionID) {
            this.details.detailitem[i].Files.push(newfile);
          }
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
         this.details.waybillinfo.Files.splice(this.details.waybillinfo.Files.indexOf(file), 1)
      }else{
        var arr = this.details.detailitem;
        for (var j = 0; j < arr.length; j++) {
          if (arr[j].ID == type.ID) {
           arr[j].Files.splice(file, 1);
         }
        }
      }
    },
    SeletUpload(type) { // 传照
      if (type == "Waybill") {
        var data = {
          SessionID: this.details.waybillinfo.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.details.waybillinfo.ID,
            WsOrderID:this.details.waybillinfo.OrderID,
            Type:8000
          }
        };
        SeletUploadFile(data);
      } else {
        var data = {
          SessionID: type.ID,
          AdminID: sessionStorage.getItem("userID"),
          Data:{
            WayBillID:this.details.waybillinfo.ID,
            WsOrderID:this.details.waybillinfo.OrderID,
            NoticeID:type.NoticeID,
            InputID:type.InputID,
            Type:8000
          }
        };
        SeletUploadFile(data);
      }
    },
    TakeGoods() {
     //我去提货
      TakeGoods(
        this.details.waybillinfo.ID,
        sessionStorage.getItem("userID")
      ).then(res => {
        if (res.Success == true) {
          this.$Message.success("提货锁定成功，请去提货");
          // this.details.waybillinfo.ExcuteStatusDes = "正在提货中";
          this.details.waybillinfo.LoadingExcuteStatus=105
          this.isdisabled=false;
        } else {
          this.$Message.error("提货锁定失败");
        }
      });
    },
    IsPayPayments(id){
      IsPayPayments(id,2).then(res=>{
        console.log(res)
        if(res==1){ //已付款
        this.TakeGoodsDisable=true;
        this.ispay="已付款"
        }else if(res==2){ //未付款
          this.TakeGoodsDisable=false;
          this.ispay="未付款"
        }
      })
    },
    changeacrrier(value) { //改变承运商的时候触发
    if(value!=undefined){
      if(this.details.waybillinfo.Type==1&&value.value=='Personal'){
        this.IsPersonal=true
        this.details.waybillinfo.CarNumber1=this.CarArrName;
        this.details.waybillinfo.Driver=this.DriversName;
      }else{
        if(this.details.waybillinfo.Type==1){
          this.IsPersonal=false
          this.details.waybillinfo.CarNumber1='';
          this.details.waybillinfo.Driver='';
          this.GetDriversCars(value.value);
        }else{
          this.IsPersonal=false
          this.details.waybillinfo.CarNumber1='';
          this.details.waybillinfo.Driver='';
        }
        this.details.waybillinfo.CarrierName=value.label
        this.oldcrriers=value.value
      }
    }
      
    },
    //根据送货上门承运商获取司机与车牌号
    GetDriversCars(key) {
      GetDriversCars(key).then(res => {
        this.CarArr=[]
        var data=res.Transports
        if(data.length>0){
            for(var i=0;i<data.length;i++){
               var item={}
              if(data[i].CarNumber2!=null&&data[i].CarNumber1!=null){
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
    changedrivers(value) {
     
    },

    changeabnormal_enter(value) {
      
      if (value == false) {
      } else {
        this.Storages = [];
        GetSortingID().then(res => {
          var data = {
            iscx: false,
            isenter: true,
            ID: "LR" + 1 + new Date().getTime(),
            NoticeID: null,
            Product: {
              PartNumber:null,
              Manufacturer: null,
              PackageCase: null,
              Packaging: null
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
            Files: []
          };
          this.Storages.push(data);
          this.timer = new Date().getTime();
          this.$nextTick(function () {
          });
        });
      }
    },
    ok_abnormal() {
     this.NullNotice=[]
      for(var i=0,lens=this.Storages.length;i<lens;i++){
          if((this.Storages[i].CurrentQuantity!=null)&&(this.Storages[i].Product.PartNumber!=null)&&(this.Storages[i].origin!=null)&&(this.Storages[i].Summary!=null)&&(this.Storages[i].Product.Manufacturer!=null)&&(this.Storages[i].DateCode!=null&&this.Storages[i].DateCode!="")){
        this.NullNotice.push(this.Storages[i])
        if(i==this.Storages.length-1){
        //  this.$Message.success("测试入库完成");
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
        clackFilesProcess(url){
        var data={
        Url:url
        }
        FilesProcess(data)
        },
        //确认分摊重量
        ok_share(){
        if(this.sharevalue==null||this.sharevalue==""){
        this.$Message.error("请输入重量");
        this.showshare=true
        }else{
        var reg=/^[0-9]+(.[0-9]{1,3})?$/
        var reg = /^\d+(\.\d{0,2})?$/;
        if(reg.test(this.sharevalue) == false||this.sharevalue==0){
        this.$Message.error("请输入数字,小数点保留两位，且不等于零");
        this.showshare=true
        this.sharevalue=null
        }else{
        this.showshare=false
        var Totalquantity=null
        var singlet =null;
        for(var i=0,lens=this.details.detailitem.length;i<lens;i++){
            if(this.details.detailitem[i].CurrentQuantity!=null&&this.details.detailitem[i].CurrentQuantity!=''){
              Totalquantity+=Number(this.details.detailitem[i].CurrentQuantity) 
            }
          }
          if(Totalquantity!=null){
            for(var i=0,lens=this.details.detailitem.length;i<lens;i++){
              if(this.details.detailitem[i].CurrentQuantity!=null&&this.details.detailitem[i].CurrentQuantity!=''){
                 this.details.detailitem[i].Weight=(this.details.detailitem[i].CurrentQuantity*(this.sharevalue/Totalquantity)).toFixed(2)
              }
            
            }
          }
         
        }
      }
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
        this.Carriers(this.details.waybillinfo.Type,housid)
        this.details.waybillinfo.CarrierID=res.Carrier.ID
        this.details.waybillinfo.CarrierName=res.Carrier.Name

         var data={ Name:res.Driverinfo.Name, ID:"", }
        this.DriversArr.push(data)
        this.details.waybillinfo.Driver=res.Driverinfo.Name

         var data={ value:res.Carinfo.CarNumber1, }
        this.CarArr.push(data)
        this.details.waybillinfo.CarNumber1=res.Carinfo.CarNumber1
        
        this.GetDriversCars(this.details.waybillinfo.CarrierID);
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
        this.Carriers(this.details.waybillinfo.Type,housid)
        this.details.waybillinfo.CarrierID=res.Carrier.ID
        this.details.waybillinfo.CarrierName=res.Carrier.Name
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
          this.GetDriversCars(this.details.waybillinfo.CarrierID);
          this.details.waybillinfo.Driver=res
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
        this.GetDriversCars(this.details.waybillinfo.CarrierID);
        this.details.waybillinfo.CarNumber1=res
        this.showaddCar=false
        this.$refs.AddDriver.delitem()
      }
    },
    testbtn(id){//代转运的跳转
       this.$store.dispatch("setshowdetail", false);
       this.$store.dispatch("setshowdetailout", true);
       this.$router.push({ path: "Outgoing/10/outdetail/"+id });
       this.$store.dispatch("setshowtype", 1);
    },
    //操作日志的展示
    logchange(){
      this.showlogged=true
      this.loggdetime=new Date().getTime()
    },
    //刷新当前数据
    sync_btn(){
       this.loading = true;
       this.CgDetail_new(this.details.waybillinfo.ID);
      //  this.$refs.History.changeTabs('3')
    }
  }
};
</script>
