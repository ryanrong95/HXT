<template>
  <div>
    <!-- <div style="margin-bottom: 20px;">
            <label for="">选择运单：</label>
             <Select v-model="model11" filterable style="width:80%">
                <Option v-for="item in cityList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
    </div>-->
    <div>
      <p class="detailtitle">基本信息</p>
      <div>
        <Row
          style="width: 100%;min-height: 110px; background: rgb(245, 247, 249);margin: 15px 0px; padding:10px;"
        >
          <Col span="8">
            <ul class="detail_li detail1">
              <li class="itemli">
                <span class="detail_title1">分拣时间</span>
                <span>{{waybillinfo.CreateDate|showDate}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">分拣数量：</span>
                <span>{{waybillinfo.SortingQuantity}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">分&nbsp;拣&nbsp;人：</span>
                <span>{{waybillinfo.AdminName}}</span>
              </li>
            </ul>
          </Col>
          <Col span="8">
            <ul class="detail_li detail1">
              
              <li class="itemli">
                <span class="detail_title1">入&nbsp;仓&nbsp;号：</span>
                <span>
                  {{waybillinfo.EnterCode}}
                  <em>({{waybillinfo.ClientName}})</em>
                </span>
              </li>
              <li class="itemli">
                <span class="detail_title1">状&nbsp;&nbsp;&nbsp;&nbsp;态：</span>
                <span>{{waybillinfo.ExcuteStatusDes}}</span>
              </li>
              <li class="itemli">
                <span class="detail_title1">业务类型：</span>
                <span>{{waybillinfo.TypeDes}}</span>
              </li>
            </ul>
          </Col>
          <Col span="8">
            <ul class="detail_li detail1">
                <li>
                    <h1>图片列表</h1>
                </li>
                <li v-for="item in waybillinfo.Files">{{item.CustomName}}</li>
            </ul>
          </Col>
        </Row>
      </div>
    </div>
    <div>
      <p class="detailtitle">产品信息</p>
      <div style="margin-top:20px">
        <Table
          :loading="loading"
          :columns="columns1"
          :data="detailitem"
          @on-selection-change=""
        >
          <template slot-scope="{ row, index }" slot="indexs">{{index+1}}</template>
          <template slot-scope="{ row, index }" slot="imglist">
            <p v-for="(item,index) in row.Files">
              <span>{{item.CustomName}}</span>
            </p>
            <!-- <Input v-model="row.typeimg" /> -->
          </template>
        </Table>
      </div>
    </div>
  </div>
</template>
<script>
import { historyDetail} from "../../api/CgApi"; //引入api 的接口
export default {
  name: "Historygoods",
  data() {
    return {
      loading:true,
      columns1: [
        {
          title: "#",
          slot: "indexs",
          align: "left",
          width: 50
          // fixed: 'right'
        },
        {
          title: "型号",
          key: "PartNumber",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Product.PartNumber);
          }
        },
        {
          title: "品牌",
          key: "Catalog",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Product.Manufacturer);
          }
        },
        {
          title: "批号",
          key: "DateCode",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {},params.row.DateCode);
          }
        },
        {
          title: "本次到货",
          key: "TruetoQuantity",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Quantity);
          }
        },
        {
          title: "原产地",
          key: "Origin",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Origin);
          }
        },
        {
          title: "入库库位",
          key: "ShelveID",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {},params.row.ShelveID);
          }
        },
        {
          title: "体积(cm³)",
          key: "Volume",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span", {}, params.row.Volume);
          }
        },
        {
          title: "毛重(g)",
          key: "GrossWeight",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("span",{},params.row.Weight);
          }
        },
        {
          title: "图片",
          slot: "imglist",
          align: "center",
          width: 200
        }
      ],
      detailitem: [],
      waybillinfo:{},
      test:"fjdsfkssssssssssssssssssss"
    };
  },
  created() {
    console.log("历史到货被从新加载");
  //  this.gethistory()
  },
  mounted() {},
  methods: {
    // gethistory(data) {
    //   //获取历史到货信息
    //   console.log(data)
    //   historyDetail(data).then(res => {
    //     console.log(res);
    //     this.detailitem=res.Sortings;
    //     this.waybillinfo=res.Waybill;
    //     this.loading=false;
    //   });
    // }
  }
};
</script>   
<style scoped>
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.detail_li .itemli {
  line-height: 45px;
  font-size: 14px;
}
</style>