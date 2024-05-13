<style scoped>
.details_title {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.info_box {
  width: 100%;
  min-height: 200px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  text-indent: 20px;
}
.infoitem {
  min-height: 45px;
  line-height: 45px;
  font-size: 14px;
}
 .infoitem label {
  padding-right: 15px;
}

.TransportDetail >>> .ivu-table-cell{
  padding-left: 0px;
  padding-right: 0px;
}
.PartNumberbox p{
  border-bottom: 1px solid #dddddd;
  min-height: 40px;
  line-height:39px;
}
.PartNumberbox p:nth-last-child(){
  border: none
}
.otderstyle .PartNumberbox{
  display:table;
  width: 100%;
  min-height:40px;
}
.PartNumberbox ul li{
  border-bottom: 1px solid #dddddd; 
  min-height:40px;
}
.otderstyle .tablespan{
  display:table-cell; vertical-align:middle;
  border-bottom: 1px solid #dddddd; 
} 

.TransportDetail >>> td.ivu-table-expanded-cell {
    padding: 20px 20px;
    background: #f8f8f9;
}
/* .TransportDetail >>> .subCol ul li{
      margin:0 -18px;
      list-style:none;
      text-Align: center;
      padding: 9px;
      border-bottom:1px solid #ccc;
      overflow-x: hidden;
}
.subCol ul li:last-child{
  border-bottom: none
} */
</style>
<template>
  <div class="TransportDetail">
    <p class="details_title">基本信息</p>
    <div class="info_box">
      <Row>
        <Col span="9">
          <p class="infoitem">
            <label class>运输批次：</label>
            <span class>{{detailinfo.LotNumber}}</span>
          </p>
          <!-- <p class="infoitem">
            <label class>状&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;态：</label>
            <span class>可出库</span>
          </p> -->
          <p class="infoitem">
            <label class>承&nbsp;&nbsp;运&nbsp;&nbsp;商：</label>
            <span class>{{detailinfo.CarrierName}}</span>
          </p>
          <p class="infoitem">
            <label class>司&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;机：</label>
            <span class>{{detailinfo.Driver}}</span>
          </p>
          <p class="infoitem">
            <label class>联系电话：</label>
            <span class>{{detailinfo.Phone}}</span>
          </p>
          <p class="infoitem">
            <label class>运输时间：</label>
            <span class>{{detailinfo.DepartDate|showDate}}</span>
          </p>
          <p class="infoitem">
            <label class>运输类型：</label>
            <span class>{{detailinfo.WaybillTypeDes}}</span>
          </p>
        </Col>
        <Col span="6">
          <p class="infoitem">
            <!-- <label class="">运输时间：</label>
            <span class="">2019-12-7</span>-->
          </p>
          <p class="infoitem">
            <label class>截单状态：</label>
            <span class>{{detailinfo.CuttingOrderStatusDes}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;数&nbsp;&nbsp;量：</label>
            <span class>{{detailinfo.TotalQuantity}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;金&nbsp;&nbsp;额：</label>
            <span class>{{detailinfo.TotalMoney}}</span>
          </p>
          <p class="infoitem">
            <label class>总毛重(kg)：</label>
            <span class>{{detailinfo.TotalWeight}}</span>
          </p>
          <p class="infoitem">
            <label class>总&nbsp;&nbsp;箱&nbsp;&nbsp;数：</label>
            <span class>{{detailinfo.BoxNumber}}</span>
          </p>
        </Col>
        <Col span="6">
          <!-- 根据最新需求，不做打印处理 -->
          <!-- <Button type="success">清单打印</Button> -->
          <Button type="success" style="margin-top:20px;" @click="isconfirm=true">完成出库</Button>
        </Col>
      </Row>
    </div>
    <p class="details_title">运输清单</p>
    <div class="tablebox">
    </div>
    <div>
      <div>
        <!-- <Table :columns="columns" :data="reportList"  border></Table> -->
         <Table :columns="columns10" :data="detailinfo.Notices">
            <template slot-scope="{ row, index }" slot="total">
               {{row.BoxesNotices.length}}
            </template>
         </Table>
      </div>
    </div>
    <Modal
        v-model="isconfirm"
        title="提示"
        @on-ok="ok_confirm"
        @on-cancel="cancel">
        <p>是否确认出库</p>
    </Modal>
  </div>
</template>
<script>
import expandRow from './table-expand.vue';
import {TransportDetail,OutputEnter} from "../../api";
export default {
  components: { expandRow },
  data() {
    return {
      isconfirm:false,//是否确认出库
      lotnumber:this.$route.params.ID,//运输批次号
      houseid:sessionStorage.getItem("UserWareHouse"),
      tableData: [
        {
          boxnumber: "Nl1919979", //箱号
          createdata: "2019-11-4", //封箱时间
          Packingperson: "张三", //封箱人
          orderid: "11111", //订单编号
          clientID: "11111", //客户编号
          clientName: "11111", //客户名称
          spanlen: 3
        },
        {
          boxnumber: "Nl1919979",
          createdata: "2019-11-4",
          Packingperson: "张三",
          orderid: "orderid333",
          clientID: "Nl898989",
          clientName: "远大电子科技有限公司",
          spanlen: 3
        },
        {
          boxnumber: "Nl1919979",
          createdata: "2019-11-4",
          Packingperson: "张三",
          orderid: "orderid333",
          clientID: "Nl898989",
          clientName: "远大电子科技有限公司",
          spanlen: 3
        },
        {
          boxnumber: "Nl1919979",
          createdata: "2019-11-4",
          Packingperson: "张三",
          orderid: "adasdd",
          clientID: "Nl8989891",
          clientName: "杭州比一比电子科技有限公司",
          spanlen: 3
        },
        {
          boxnumber: "Nl1919979",
          createdata: "2019-11-4",
          Packingperson: "张三",
          orderid: "adasdd",
          clientID: "Nl8989891",
          clientName: "杭州比一比电子科技有限公司",
          spanlen: 3
        }
      ],
      columns1: [
        {
          title: "#",
          slot: "index",
          align: "center"
        },
        {
          title: "箱号",
          slot: "boxnumber",
          align: "center"
        },
        {
          title: "装箱时间",
          slot: "boxtimer",
          align: "center"
        },
        {
          title: "装箱人",
          slot: "boxperson",
          align: "center"
        },
        {
          title: "订单编号",
          slot: "orderID",
          align: "center",
          width:140,
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center"
        },
        {
          title: "制造商",
          slot: "Manufacturer",
          align: "center"
        },
        {
          title: "封装",
          slot: "PackageCase",
          align: "center"
        },
        {
          title: "批次",
          slot: "DateCode",
          align: "center"
        },
        {
          title: "应到小计",
          slot: "Total",
          align: "center"
        },
        {
          title: "客户编号",
          slot: "ClientID",
          align: "center"
        },
        {
          title: "客户名称",
          slot: "ClientName",
          align: "center"
        }
      ],
      data1: [
        {
          boxnumber: "第一箱",
          boxtimer: "2019-12-15",
          boxperson: "张三",
          order: [
            {
              orderID: "12121212",
              product: [
                {
                  CreateDate: "/Date(1578474657693)/",
                  ID: "C4713C21258938F4D060B62C11511FB1",
                  Manufacturer: "deng100",
                  PackageCase: "null",
                  Packaging: "null",
                  PartNumber: "deng200",
                  DateCode: "批次",
                  Total: 200
                },
                {
                  CreateDate: "/Date(1578474657693)/",
                  ID: "C4713C21258938F4D060B62C11511FB1",
                  Manufacturer: "deng100",
                  PackageCase: "null",
                  Packaging: "null",
                  PartNumber: "deng200",
                  DateCode: "批次",
                  Total: 200
                }
              ]
            },
            {
              orderID: "12121212",
              product: [
                {
                  CreateDate: "/Date(1578474657693)/",
                  ID: "C4713C21258938F4D060B62C11511FB1",
                  Manufacturer: "deng100",
                  PackageCase: "null",
                  Packaging: "null",
                  PartNumber: "deng200",
                  DateCode: "批次",
                  Total: 200
                }
              ]
            }
          ],
          ClientID: "BL767567",
          ClientName: "杭州比一比"
        },
        {
          boxnumber: "第一箱",
          boxtimer: "2019-12-15",
          boxperson: "张三",
          order: [
            {
              orderID: "12121212",
              product: [
                {
                  CreateDate: "/Date(1578474657693)/",
                  ID: "C4713C21258938F4D060B62C11511FB1",
                  Manufacturer: "deng100",
                  PackageCase: "null",
                  Packaging: "null",
                  PartNumber: "deng200",
                  DateCode: "批次",
                  Total: 200
                }
              ]
            }
          ],
          ClientID: "BL767567",
          ClientName: "杭州比一比"
        }
      ],
      detailinfo:"",//详情信息
      Notices: [
           {
                "ClientID": "056676CA99542C2C72544C83128D5CAD",
                "ClientName": "深圳市拓普仪器有限公司",
                "Boxes": {
                    "ID": "WLHK01_WLT20200205033",
                    "Code": "WL200205033",
                    "AdminID": "Admin00057",
                    "WarehouseID": "HK01_WLT",
                    "CreateDate": "\/Date(1580869128013)\/",
                    "Summary": "",
                    "Status": 500,
                    "_disabled": true,
                    "CodePrefix": null,
                    "DateStr": null,
                    "StatusDescription": "已报关",
                    "AdminName": "tsa"
                },
                "Order": [
                    {
                        "OrderID": "第一箱",
                        "PickingNotice": [
                            {
                                "Product": {
                                    "ID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                    "PartNumber": "PIC32MX534F064H-I/PT",
                                    "Manufacturer": "MICROCHIP",
                                    "PackageCase": null,
                                    "Packaging": null,
                                    "CreateDate": "\/Date(1576734614890)\/"
                                },
                                "Output": {
                                    "ID": "Opt2020020500000001",
                                    "InputID": "Ipt2020020400000041",
                                    "OrderID": "WL46120200204001",
                                    "TinyOrderID": "WL46120200204001-01",
                                    "ItemID": "OrderItem20200204000030",
                                    "OwnerID": "Admin0000000333",
                                    "SalerID": null,
                                    "CustomerServiceID": null,
                                    "PurchaserID": null,
                                    "Currency": 2,
                                    "Price": 10.0000000,
                                    "StorageID": "20200205000001",
                                    "CreateDate": "\/Date(1580870386357)\/",
                                    "Checker": null
                                },
                                "ID": "NT20200205000002",
                                "Type": 315,
                                "WareHouseID": "HK01_WLT",
                                "WaybillID": "Waybill202002050001",
                                "InputID": "Ipt2020020400000041",
                                "OutputID": "Opt2020020500000001",
                                "ProductID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                "Supplier": null,
                                "DateCode": "P24",
                                "Quantity": 100.0000000,
                                "StockQuantity": 0,
                                "SurplusQuantity": -100.0000000,
                                "Conditions": null,
                                "CreateDate": "\/Date(1580870386340)\/",
                                "Status": 100,
                                "Source": 30,
                                "Target": 200,
                                "BoxCode": "WL200205033",
                                "BoxDate": "\/Date(-62135596800000)\/",
                                "Weight": null,
                                "NetWeight": null,
                                "Volume": null,
                                "ShelveID": null,
                                "Files": null,
                                "Visable": true,
                                "Checked": false,
                                "Input": null,
                                "BoxingSpecs": null,
                                "Sorting": null
                            },
                            {
                                "Product": {
                                    "ID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                    "PartNumber": "PIC32MX534F064H-I/PT",
                                    "Manufacturer": "MICROCHIP",
                                    "PackageCase": null,
                                    "Packaging": null,
                                    "CreateDate": "\/Date(1576734614890)\/"
                                },
                                "Output": {
                                    "ID": "Opt2020020500000001",
                                    "InputID": "Ipt2020020400000041",
                                    "OrderID": "WL46120200204001",
                                    "TinyOrderID": "WL46120200204001-01",
                                    "ItemID": "OrderItem20200204000030",
                                    "OwnerID": "Admin0000000333",
                                    "SalerID": null,
                                    "CustomerServiceID": null,
                                    "PurchaserID": null,
                                    "Currency": 2,
                                    "Price": 10.0000000,
                                    "StorageID": "20200205000001",
                                    "CreateDate": "\/Date(1580870386357)\/",
                                    "Checker": null
                                },
                                "ID": "NT20200205000002",
                                "Type": 315,
                                "WareHouseID": "HK01_WLT",
                                "WaybillID": "Waybill202002050001",
                                "InputID": "Ipt2020020400000041",
                                "OutputID": "Opt2020020500000001",
                                "ProductID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                "Supplier": null,
                                "DateCode": "P24",
                                "Quantity": 100.0000000,
                                "StockQuantity": 0,
                                "SurplusQuantity": -100.0000000,
                                "Conditions": null,
                                "CreateDate": "\/Date(1580870386340)\/",
                                "Status": 100,
                                "Source": 30,
                                "Target": 200,
                                "BoxCode": "WL200205033",
                                "BoxDate": "\/Date(-62135596800000)\/",
                                "Weight": null,
                                "NetWeight": null,
                                "Volume": null,
                                "ShelveID": null,
                                "Files": null,
                                "Visable": true,
                                "Checked": false,
                                "Input": null,
                                "BoxingSpecs": null,
                                "Sorting": null
                            }
                        ]
                    },
                    {
                        "OrderID": "第二箱",
                        "PickingNotice": [
                            {
                                "Product": {
                                    "ID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                    "PartNumber": "PIC32MX534F064H-I/PT",
                                    "Manufacturer": "MICROCHIP",
                                    "PackageCase": null,
                                    "Packaging": null,
                                    "CreateDate": "\/Date(1576734614890)\/"
                                },
                                "Output": {
                                    "ID": "Opt2020020500000001",
                                    "InputID": "Ipt2020020400000041",
                                    "OrderID": "WL46120200204001",
                                    "TinyOrderID": "WL46120200204001-01",
                                    "ItemID": "OrderItem20200204000030",
                                    "OwnerID": "Admin0000000333",
                                    "SalerID": null,
                                    "CustomerServiceID": null,
                                    "PurchaserID": null,
                                    "Currency": 2,
                                    "Price": 10.0000000,
                                    "StorageID": "20200205000001",
                                    "CreateDate": "\/Date(1580870386357)\/",
                                    "Checker": null
                                },
                                "ID": "NT20200205000002",
                                "Type": 315,
                                "WareHouseID": "HK01_WLT",
                                "WaybillID": "Waybill202002050001",
                                "InputID": "Ipt2020020400000041",
                                "OutputID": "Opt2020020500000001",
                                "ProductID": "AB804C2DA2B8AD037F74B21289AD7C87",
                                "Supplier": null,
                                "DateCode": "P24",
                                "Quantity": 100.0000000,
                                "StockQuantity": 0,
                                "SurplusQuantity": -100.0000000,
                                "Conditions": null,
                                "CreateDate": "\/Date(1580870386340)\/",
                                "Status": 100,
                                "Source": 30,
                                "Target": 200,
                                "BoxCode": "WL200205033",
                                "BoxDate": "\/Date(-62135596800000)\/",
                                "Weight": null,
                                "NetWeight": null,
                                "Volume": null,
                                "ShelveID": null,
                                "Files": null,
                                "Visable": true,
                                "Checked": false,
                                "Input": null,
                                "BoxingSpecs": null,
                                "Sorting": null
                            }
                        ]
                },
            ]
        }
    ],
    columns10: [
                {
                    type: 'expand',
                    width: 50,
                    render: (h, params) => {
                        return h(expandRow, {
                            props: {
                                row: params.row
                            }
                        })
                    }
                },
                {
                    title: '订单号',
                    key: 'TinyOrderID'
                },
                {
                    title: '总箱数',
                    slot: 'total'
                },
                {
                    title: '总重量',
                    key: 'address'
                }
      ],
    
  };
  },
  created() {
    this.getData(this.tableData);
    // console.log(this.$route.params.ID)
    this.getdetail()
  },
  mounted() {},
  methods: {
    getdetail(){
      var data={
        warehouseID:this.houseid,//库房编号（必填）
        lotnumber:this.lotnumber,//运输批次号（必填）
      }
      TransportDetail(data).then(res=>{
          console.log(res)
          this.detailinfo=res.obj;
      })
    },
    ok_confirm(){
      OutputEnter(this.lotnumber).then(res=>{
          if(res.val==0){
            this.$Message.success('出库完成');
           this.$store.dispatch("setTransportDetail",false);
          }else if(res.val==2){
             this.$Message.error('该运输批次未截单，请先截单');
          }else{
             this.$Message.error('出库失败');
          }
      })
         
    },
    cancel(){
      this.isconfirm=false;
    },
    getData(list){
        //console.log(list[0]);
        for ( let field in list[0]) {
            var k = 0;
            while (k < list.length) {
                list[k][field + 'span'] = 1;
                list[k][field + 'dis'] = false;
                for (var i = k + 1; i <= list.length - 1; i++) {
                    if (list[k][field] == list[i][field] && list[k][field] != '') {
                        list[k][field + 'span']++;
                        list[k][field + 'dis'] = false;
                        list[i][field + 'span'] = 1;
                        list[i][field + 'dis'] = true;
                    } else {
                        break;
                    }
                }
                k = i;
            }
        }
        return list;
    }
  }
};
</script>