<style >
#Printboxcode .subCol ul li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  padding: 9px;
  border-bottom: 1px solid #ccc;
  overflow: hidden;
  line-height:30px;
}
#Printboxcode .subCol ul li:last-child {
  border-bottom: none;
}
</style>
<template>
  <div id="Printboxcode">
    <Table :border="reportList.length > 0" :columns="columns1" :data="reportList">
      <template slot-scope="{ row, index }" slot="Boxarr">
        <p>{{row.Series}}</p>
        <p>
          <Button type="primary" size="small" @click="printitem(1,row)">打印</Button>
        </p>
      </template>
      <template slot-scope="{ row, index }" slot="Package">
        <div class="subCol">
          <ul>
            <li v-for="item in row.BoxMessage">
              <Select v-model="item.PackageType" style="width:90%" transfer @on-change="changeselect($event,item)">
                <Option
                  v-for="(i,index) in PackageTypeArr"
                  :value="i.GBCode"
                  :key="index"
                >{{ i.ChineseName }}</Option>
              </Select>
            </li>
          </ul>
        </div>
      </template>
      <template slot-scope="{ row, index }" slot="CreateDate">
        <div class="subCol">
          <ul>
            <li v-for="item in row.BoxMessage">{{item.CreateDate|showDate}}</li>
          </ul>
        </div>
      </template>
      <template slot-scope="{ row, index }" slot="Action">
        <div class="subCol">
          <ul>
            <li v-for="item in row.BoxMessage">
              <Button type="primary" size="small" @click="printitem(2,item)">打印</Button>
            </li>
          </ul>
        </div>
      </template>
    </Table>
  </div>
</template>
<script>
import {
  printboxcode,
  GetPackageType,
  cgEnterPackageType
} from "../../api/CgApi";
import { TemplatePrint, GetPrinterDictionary } from "@/js/browser.js";
let product_url = require("../../../static/pubilc.dev");
export default {
  data() {
    return {
      PackageTypeArr: [],
      parentsdata: "",
      printurl: product_url.pfwms,
      reportList: [],
      columns1: [
        {
          title: "连续箱号",
          slot: "Boxarr",
          width: 200
        },
        {
          title: "箱号",
          key: "list",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].BoxMessage.map(item => {
                    return h("li", {}, item.ID);
                  })
                )
              ]
            );
          }
        },
        {
          title: "包装类型",
          slot: "Package",
          align: "center"
        },
        {
          title: "入仓号",
          key: "EnterCode",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].BoxMessage.map(item => {
                    return h("li", {}, item.EnterCode);
                  })
                )
              ]
            );
          }
        },
        {
          title: "所属库房",
          key: "WhName",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].BoxMessage.map(item => {
                    return h("li", {}, item.WhName);
                  })
                )
              ]
            );
          }
        },
        {
          title: "生产日期",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "装箱人",
          key: "AdminID",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].BoxMessage.map(item => {
                    return h("li", {}, item.AdminName);
                  })
                )
              ]
            );
          }
        },
        {
          title: "操作",
          slot: "Action"
        }
      ]
    };
  },
  created() {
    // this.getparents()
    // this.GetPackageType()
  },
  methods: {
    //获取打印列表数据
    getparents(val) {
      this.parentsdata = val;
      printboxcode(this.parentsdata).then(res => {
        console.log(res);
        if (res.length <= 0) {
          this.reportList = [];
        } else {
          this.reportList = res;
        }
      });
    },
    //获取包装类型
    GetPackageType() {
      GetPackageType().then(res => {
        this.PackageTypeArr = res;
      });
    },
    //修改或保存包装类型
    changeselect(event,item) {
      var data = {
        boxCode: item.ID, //箱号（必要的）
        packageType: event, //包装种类
        adminID: item.AdminID //操作人（必要的）
      };
      cgEnterPackageType(data).then(res => {
        if(res.success==true){
           this.$Message.success('包装类型修改成功');
        }
      });
    },
    //打印
    printitem(type, item) {
      var newdata = [];
      if (type == 1) {
        var BoxMessage = item.BoxMessage;
        for (var i = 0; i < BoxMessage.length; i++) {
          var obj = {
            ID: BoxMessage[i].ID,
            EnterCode: BoxMessage[i].EnterCode,
            AdminID: BoxMessage[i].AdminID,
            CreateDate: BoxMessage[i].CreateDate,
            Quantity: BoxMessage[i].Quantity,
            Weight: BoxMessage[i].Weight
          };
          newdata.push(obj);
        }
      } else {
        var obj = {
          ID: item.ID,
          EnterCode: item.EnterCode,
          AdminID: item.AdminID,
          CreateDate: new Date( parseInt(item.CreateDate.substr(6, 13))  ).toLocaleDateString(),
          Quantity: item.Quantity,
          Weight: item.Weight
        };
        newdata.push(obj);
      }
      console.log(newdata);
      var configs = GetPrinterDictionary();
      // alert(JSON.stringify(configs))
      var getsetting = configs["箱签打印"];
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
    }
  }
};
</script>