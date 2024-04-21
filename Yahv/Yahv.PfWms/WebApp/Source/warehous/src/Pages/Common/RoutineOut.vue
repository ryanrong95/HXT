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
</style>
<template>
  <div>
    <div style="width:100%;">
      <div class="itembox">
        <p class="detailtitle">基础信息</p>
        <p>常规出库</p>
        <div style="width:100%;height:200px;background:#f5f7f9;margin:15px 0">
          <Row>
            <Col span="6">
              <ul class="detail_li">
                <li class="itemli">
                  <span>ID:</span>
                  <span>2154885457</span>
                </li>
                <li class="itemli">
                  <span>状态:</span>
                  <span>部分到货</span>
                </li>
                <li class="itemli">
                  <span>业务类型:</span>
                  <span>代仓储-转运</span>
                </li>
              </ul>
            </Col>
            <Col span="6">
              <ul class="detail_li">
                <li class="itemli">
                  <span>通知时间:</span>
                  <span>2019-07-30</span>
                </li>
                <li class="itemli">
                  <span>供应商:</span>
                  <span>Digkey</span>
                </li>
                <li class="itemli">
                  <span>到货方式:</span>
                  <span>国际快递</span>
                  <!-- <a href="">历史到货
                         <Badge :count="10"></Badge>
                  </a>-->
                  <Badge :count="5" :offset="[9,-12]" class-name="demo-badge-alone">
                    <a href="#">历史到货</a>
                  </Badge>
                </li>
              </ul>
            </Col>
            <Col span="6">
              <ul class="detail_li">
                <li class="itemli">
                  <span>入仓号:</span>
                  <span>NL020</span>
                </li>
                <li class="itemli">
                  <span>运单号(本次):</span>
                  <span>
                    <Input style="width:60%" v-model="details.WaybillNo" />
                  </span>
                </li>
                <li class="itemli">
                  <span>承运商(本次):</span>
                  <span>
                    <Select v-model="details.Carrier" style="width:200px">
                      <Option
                        v-for="item in details.CarrierList"
                        :value="item.value"
                        :key="item.value"
                      >{{ item.label }}</Option>
                    </Select>
                  </span>
                </li>
                <li class="itemli">
                  <span>输送地:</span>
                  <span>
                    <Input style="width:60%" v-model="details.Conveyorsite" />
                  </span>
                </li>
              </ul>
            </Col>
            <Col span="6">
              <ul class="detail_li">
                <li class="itemli">
                  <Button type="primary" icon="ios-search">传照</Button>
                  <Button type="primary" icon="ios-search">拍照</Button>
                </li>
                <li></li>
              </ul>
            </Col>
          </Row>
        </div>
      </div>
      <div class="itembox">
        <p class="detailtitle">产品清单</p>
        <div style="margin:15px 0">
          <Input
            search
            enter-button="筛选"
            placeholder="输入筛选内容"
            style="width:20%;float:left;margin-right:5px"
          />
          <Button type="primary">清单打印</Button>
          <Button type="primary">标签打印</Button>
          <Button type="primary">一键打印</Button>
          <Button type="primary">收支明细</Button>
          <div style="float:right">
            <Button type="error">到货异常</Button>
            <Button type="success" icon="md-checkmark">入库完成</Button>
          </div>
        </div>
        <div>
          <div class="detail_tablebox">
            <Table :columns="details.columns1" :data="details.data1">
              <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
              <template
                slot-scope="{ row, index }"
                slot="Arrival"
              >{{row.Shouldarrive}}&nbsp;/&nbsp;{{row.Alreadyarrived}}</template>
              <template slot-scope="{ row, index }" slot="Quantity">
                <Input v-model="row.Quantity" />
              </template>
              <template slot-scope="{ row, index }" slot="Country_origin">
                <Input v-model="row.Country_origin" />
              </template>
              <template slot-scope="{ row, index }" slot="StockCode">
                <Input v-model="row.StockCode" />
              </template>
              <template slot-scope="{ row, index }" slot="volume">
                <Input v-model="row.volume" />
              </template>
              <template slot-scope="{ row, index }" slot="GrossWeight">
                <Input v-model="row.GrossWeight" />
              </template>
              <template slot-scope="{ row, index }" slot="operation">
                <Button type="success" icon="md-reverse-camera">拍照</Button>
                <Button type="success" icon="md-checkmark">拆项</Button>
              </template>
            </Table>
            <div class="successbtn">
              <Button type="success" icon="md-checkmark">入库完成</Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      details: {
        //详情页
        WaybillNo: "", //运单号(本次)
        Carrier: "yunda", //承运商(本次)
        CarrierList: [
          //承运商列表
          {
            value: "shunfeng",
            label: "顺丰快递"
          },
          {
            value: "yunda",
            label: "韵达快递"
          },
          {
            value: "lianbang",
            label: "联邦快递"
          },
          {
            value: "ems",
            label: "EMS"
          }
        ],
        Conveyorsite: "美国", //输送地,
        columns1: [
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
            key: "Model"
          },
          {
            title: "品牌",
            key: "brand"
          },
          {
            title: "批次",
            key: "batch"
          },
          {
            title: "已到/应到",
            slot: "Arrival",
            align: "center"
          },
          {
            title: "本次到货",
            slot: "Quantity",
            align: "center"
          },
          {
            title: "原产地",
            slot: "Country_origin",
            align: "center"
          },
          {
            title: "入库库位",
            slot: "StockCode",
            align: "center"
          },
          {
            title: "体积(cm³)",
            slot: "volume",
            align: "center"
          },
          {
            title: "毛重(g)",
            slot: "GrossWeight",
            align: "center"
          },
          {
            title: "操作",
            slot: "operation",
            align: "center",
            width: 200
          }
        ],
        data1: [
          {
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "5000", //应到
            Alreadyarrived: "500", //已到
            Quantity: "50", //本次到货数量
            StockCode: "558", //库位号
            Country_origin: "美国", //原产地
            volume: "50", //体积
            GrossWeight: "50000"
          },
          {
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "1000",
            Alreadyarrived: "200",
            Quantity: "20",
            StockCode: "258",
            Country_origin: "香港",
            volume: "50", //体积
            GrossWeight: "50000"
          },
          {
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "33333",
            Shouldarrive: "33333",
            Alreadyarrived: "3333",
            Quantity: "3333",
            StockCode: "33333",
            Country_origin: "北京",
            volume: "50", //体积
            GrossWeight: "50000"
          },
          {
            Model: "MPX68HFHDGF",
            brand: "FREESCEE",
            batch: "1032",
            Shouldarrive: "1000",
            Alreadyarrived: "200",
            Quantity: "20",
            StockCode: "258",
            Country_origin: "香港",
            volume: "50", //体积
            GrossWeight: "50000"
          }
        ]
      }
    };
  }
};
</script>