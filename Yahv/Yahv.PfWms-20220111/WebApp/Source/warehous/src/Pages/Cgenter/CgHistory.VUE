<style scoped>
.subCol li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  padding: 9px;
  border-bottom: 1px solid #ccc;
  overflow-x: hidden;
  height: 47px;
  /* line-height: 45px; */
}
.subCol li:last-child {
  border-bottom: none;
}
.Mustfill {
  color: red;
}
.Statisticsbox {
  height: 48px;
  background: #f8f8f9;
  line-height: 48px;
  text-indent: 20px;
  border: 1px solid #e8eaec;
  border-top: none;
}
.Statisticsbox span {
  padding-right: 30px;
}
</style>
<template>
  <div>
    <!-- <div>
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
    </div>-->
    <div>
      <p class="detailtitle">到货信息</p>
      <div style="margin-top:20px">
        <Tabs @on-click="changeTabs" :value="Nameitem">
          <TabPane label="正常到货 / 封箱" name="3" v-if="Type==2">
            <div >
              <Sealing-html @hietorgetDetail='hietorgetDetail' v-bind:OrderID="OrderID" ref='Sealing' v-bind:Type="Type" v-bind:EnterCode='EnterCode' :WaybillID='WaybillID' :ExcuteStatus='ExcuteStatus' v-bind:chengeCarrier="chengeCarrier">
              </Sealing-html>
            </div>
          </TabPane>
          <TabPane label="正常到货" name="0" v-else>
            <div>
              <Table
                :columns="Normal"
                :data="NormalData"
                :border="NormalData.length > 0"
                :loading="loading"
              >
                <template slot-scope="{ row, index }" slot="PartNumber">
                  <span>{{row.PartNumber}}</span>
                </template>
                <template slot-scope="{ row, index }" slot="Manufacturer">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.Product.Manufacturer}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="DateCode">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.DateCode}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="Origin">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.Origin}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="Weight">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.Weight}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="Volume">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.Volume}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="BoxCode">
                  <ul class="subCol" v-if="Type==1">
                    <li v-for="item in row.Datas">{{item.ShelveID}}</li>
                  </ul>
                  <ul class="subCol" v-else>
                    <li v-for="item in row.Datas">
                      <span>{{item.BoxCode|showboxcode}}</span>
                      <Icon v-if="item.Display==true" type="md-create" @click="changeboxcode(item)" />
                    </li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="Quantity">
                  <span>{{row.YdQty}}</span>
                </template>
                <template slot-scope="{ row, index }" slot="PickedQuantity">
                  <span>{{row.SdQty}}</span>
                </template>
                <template slot-scope="{ row, index }" slot="CreateDate">
                  <span>{{row.LastEnterDate|showDateexact}}</span>
                </template>
                <template slot-scope="{ row, index }" slot="CarrierName">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.CarrierName}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="code">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">{{item.wbCode}}</li>
                  </ul>
                </template>
                <template slot-scope="{ row, index }" slot="Action">
                  <ul class="subCol">
                    <li v-for="item in row.Datas">
                      <Button
                        type="error"
                        size="small"
                        @click="delItem(item)"
                        v-if="item.Display==true"
                        :disabled="Operateddisable==false?true:false"
                      >删除</Button>
                    </li>
                  </ul>
                </template>
              </Table>
              <p class="Statisticsbox" v-if="NormalData.length > 0">
                <span>合计：</span>
                <span>总重量(Kg)：{{Normalinfo.TotalWeight}}</span>
                <span>总件数：{{Normalinfo.TotalPart}}</span>
                <span>总数量：{{Normalinfo.TotalQuantity}}</span>
              </p>
            </div>
          </TabPane>
          <TabPane label="异常到货" name="1">
            <Table
              :columns="Abnormal"
              :data="AbnormalData"
              :border="AbnormalData.length > 0"
              :loading="loading"
            >
              <template slot-scope="{ row, index }" slot="PartNumber">
                <span>{{row.PartNumber}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Manufacturer">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.Product.Manufacturer}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="DateCode">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.DateCode}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="Origin">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.Origin}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="Weight">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.Weight}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="Volume">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.Volume}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="BoxCode">
                <ul class="subCol" v-if="Type==1">
                  <li v-for="item in row.Datas">{{item.ShelveID}}</li>
                </ul>
                <ul class="subCol" v-else>
                  <li v-for="item in row.Datas">
                    <span>{{item.BoxCode|showboxcode}}</span>
                    <Icon type="md-create" @click="changeboxcode(item)" />
                  </li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="Quantity">
                <span>{{row.YdQty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="PickedQuantity">
                <span>{{row.SdQty}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="CreateDate">
                <span>{{row.LastEnterDate|showDateexact}}</span>
              </template>
              <template slot-scope="{ row, index }" slot="Summary">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.Summary}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="CarrierName">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.CarrierName}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="code">
                <ul class="subCol">
                  <li v-for="item in row.Datas">{{item.wbCode}}</li>
                </ul>
              </template>
              <template slot-scope="{ row, index }" slot="Action">
                <ul class="subCol">
                  <li v-for="item in row.Datas">
                    <Button
                      type="error"
                      size="small"
                      @click="delItem(item)"
                      v-if="item.Display==true"
                      :disabled="Operateddisable==false?true:false"
                    >删除</Button>
                  </li>
                </ul>
              </template>
            </Table>
            <p class="Statisticsbox" v-if="AbnormalData.length > 0">
              <span>合计：</span>
              <span>总重量(Kg)：{{Abnormainfo.TotalWeight}}</span>
              <span>总件数：{{Abnormainfo.TotalPart}}</span>
              <span>总数量：{{Abnormainfo.TotalQuantity}}</span>
            </p>
          </TabPane>
        </Tabs>
      </div>
    </div>

    <Modal
      v-model="showEdit"
      title="修改箱号"
      :mask-closable="false"
      @on-visible-change="visiblechange"
    >
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>

      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日期：
        </label>
        <DatePicker
          type="date"
          style="width:80%"
          :options="options3"
          placeholder="请选择生成箱号的时间"
          :clearable="false"
          :value="saleDate"
          @on-change="changeData"
        ></DatePicker>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱号：
        </label>
        <!-- <Input
        v-model.trim="newboxcode" 
        maxlength="30" 
        placeholder="请输入临时箱号" 
        style="width:80%"/>-->
        <Input
          v-model.trim="newboxcode"
          maxlength="30"
          placeholder="请输入临时箱号"
          style="width:80%"
          @on-blur="handleCreate1(newboxcode)"
        />
      </p>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok_change">确定</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
import {
  CgOrderInputReportGroup,
  HistorDeleteSorting,
  BoxcodeEnter,
  CgBoxesShow,
  BoxcodeDelete
} from "../../api/CgApi"; //引入api 的接口
import Sealing from '../Public/Sealing'
export default {
  name: "Historygoods",
    props: ["OrderID", "Type", "EnterCode", "uploadeCgDetail_new", 'WaybillID', 'ExcuteStatus','chengeCarrier'],
   components: {
    "Sealing-html":Sealing,
  },
  data() {
    return {
      options3: {
        disabledDate(date) {
          return date && date.valueOf() < Date.now() - 86400000;
        }
      },
      saleDate: "", //箱号时间
      Operateddisable: true, //删除按钮状态
      isclickbtn: false,
      enterCode: null,
      boxingarr: [],
      houseid: sessionStorage.getItem("UserWareHouse"),
      oldboxcode: "",
      newboxcode: "",
      newboxcodeback: null,
      oldBoxCode2: null, //输入框中的旧箱号
      boxcodetype: "1",
      showEdit: false,
      Nameitem: "0",
      loading: true,
      warehouseId: null,
      waybillinfo: {},
      NormalData: [], //正常数据
      AbnormalData: [], //异常数据
      Normalinfo: null,
      Abnormainfo: null,
      Normal: [
        {
          type: "index",
          width: 60,
          align: "center"
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center"
        },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: "center"
        },
        {
          title: "批号",
          slot: "DateCode",
          align: "center"
        },
        {
          title: "原产地",
          slot: "Origin",
          align: "center"
        },
        {
          title: "重量(Kg)",
          slot: "Weight",
          align: "center"
        },
        {
          title: "体积",
          slot: "Volume",
          align: "center"
        },
        {
          title: "箱号",
          slot: "BoxCode",
          align: "center",
          width: 150
        },
        {
          title: "应到",
          slot: "Quantity",
          align: "center"
        },
        {
          title: "实到",
          slot: "PickedQuantity",
          align: "center"
        },
        {
          title: "入库时间",
          slot: "CreateDate",
          align: "center"
        }
      ],
      Abnormal: [
        {
          type: "index",
          width: 60,
          align: "center"
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center"
        },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: "center"
        },
        {
          title: "批号",
          slot: "DateCode",
          align: "center"
        },
        {
          title: "原产地",
          slot: "Origin",
          align: "center"
        },
        {
          title: "重量(Kg)",
          slot: "Weight",
          align: "center"
        },
        {
          title: "体积",
          slot: "Volume",
          align: "center"
        },
        {
          title: "箱号",
          slot: "BoxCode",
          align: "center",
          width: 150
        },
        {
          title: "应到",
          slot: "Quantity",
          align: "center"
        },
        {
          title: "实到",
          slot: "PickedQuantity",
          align: "center"
        },
        {
          title: "入库时间",
          slot: "CreateDate",
          align: "center"
        },
        {
          title: "异常原因",
          slot: "Summary",
          align: "center",
          width: 200
        }
      ]
    };
  },
  created() {},
  watch: {
    //  newboxcode(){
    //     var code=this.newboxcodeback
    //     if(code!=null){
    //       code=this.newboxcodeback.split("]")[1]
    //       console.log(this.newboxcodeback.split("]")[1])
    //     }else{
    //       code=this.newboxcodeback
    //     }
    //     return code
    //   }
  },
    mounted() {

      console.log(this.chengeCarrier)

    if (this.Type == 1) {
      this.Normal[7].title = "库位";
      this.Abnormal[7].title = "库位";
       this.changeTabs('0')
    } else {
       this.changeTabs('3')
      var CarrierNamedatas = {
        title: "承运商",
        slot: "CarrierName",
        align: "center",
        width: 150
      };
      this.Normal.push(CarrierNamedatas);
      this.Abnormal.push(CarrierNamedatas);
      var codedatas = {
        title: "运单号",
        slot: "code",
        align: "center",
        width: 150
      };
      this.Normal.push(codedatas);
      this.Abnormal.push(codedatas);
      var datas = {
        title: "操作",
        slot: "Action",
        align: "center"
      };
      this.Normal.push(datas);
      this.Abnormal.push(datas);
    }
  },
  methods: {
    hietorgetDetail(){
      console.log("qqqqqqqqqqqqqqqq")
       this.$emit("uploadeCgDetail_new");
    },
    setboxsplit(str) {
      //去除前后空格
      if (str) {
        return str.split("]")[1];
      }
    },
    CgOrderInputReportGroup(status) {
      (this.warehouseId = sessionStorage.getItem("UserWareHouse")), //库房id
        CgOrderInputReportGroup(this.warehouseId, this.OrderID, status).then(
          res => {
            this.loading = false;
            if (status == true) {
              this.Normalinfo = res;
              this.NormalData = res.Normal.sort(function(a, b) {
                return a.LastEnterDate < b.LastEnterDate ? 1 : -1;
              });

              // this.NormalData = res.Normal;
            } else {
              this.Abnormainfo = res;
              // this.AbnormalData = res.Abnormal;
              this.AbnormalData = res.Abnormal.sort(function(a, b) {
                return a.LastEnterDate < b.LastEnterDate ? 1 : -1;
              });
            }
          }
        );
    },
    changeTabs(name) {
      console.log(name);
      this.loading = true;
      this.Nameitem = name;
      if (name == "3") {
          this.$refs.Sealing.loading=true
          this.$refs.Sealing.getlistdata();  // 调用子组件的方法childClick
      }else if(name == "0"){
         this.CgOrderInputReportGroup(true);
      }
      else {
        this.CgOrderInputReportGroup(false);
      }
    },
    delItem(item) {
      var data = {
        StorageID: item.StorageID,
        AdminID: sessionStorage.getItem("userID")
      };
      HistorDeleteSorting(data).then(res => {
        if (res.Success == true) {
          this.$Message.success("删除成功");
          this.loading = true;
          this.changeTabs(this.Nameitem);
          this.$emit("uploadeCgDetail_new");
        } else if (res.Success == false) {
          this.$Message.error(res.Data);
        }
      });
    },
    changeboxcode(value) {
      this.showEdit = true;
      this.oldboxcode = value;
    },
    handleCreate1(val) {
      //箱号添加
      if (!val==false&&val!='WL') {
        if (this.oldBoxCode2 != null) {
          var data = {
            boxCode: this.oldBoxCode2
            // date:this.saleDate
          };
          BoxcodeDelete(data).then(res => {
            var newdata = {
              enterCode: this.EnterCode, // 统一使用当前运单的entercode
              code: val, // 箱号
              date: this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
              adminID: sessionStorage.getItem("userID") //装箱人使用当前操作的adminID
            };
            BoxcodeEnter(newdata).then(res => {
              console.log(res);
              if (res.success == false && res.code == 400) {
                this.$Message.error("箱号已经被选择，请选择其他箱号");
                this.newboxcode = null;
                this.newboxcodeback = null;
                this.oldBoxCode2 = null;
                this.isclickbtn = false;
              } else {
                this.newboxcode = this.setboxsplit(res.boxCode);
                this.newboxcodeback = res.boxCode;
                this.oldBoxCode2 = res.boxCode;
                this.isclickbtn = true;
              }
            });
          });
        } else {
          var data = {
            enterCode: this.EnterCode, // 统一使用当前运单的entercode
            code: val, // 箱号
            date: this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
            adminID: sessionStorage.getItem("userID") //装箱人使用当前操作的adminID
          };
          BoxcodeEnter(data).then(res => {
            if (res.success == false) {
              this.$Message.error(res.data);
              this.newboxcode = null;
              this.newboxcodeback = null;
              this.isclickbtn = false;
            } else {
              this.newboxcode = this.setboxsplit(res.boxCode);
              this.newboxcodeback = res.boxCode;
              this.oldBoxCode2 = res.boxCode;
              this.isclickbtn = true;
            }
          });
        }
      }else{
         this.$Message.error('请输入正确的箱号')
         this.newboxcode=null
      }
    },
    CgBoxesShow() {
      var data = {
        enterCode: this.EnterCode, //入仓号
        date: "" //箱号日期（可为空，为空时展示当前日期的箱号）
      };
      console.log(this.EnterCode);
      CgBoxesShow(data).then(res => {
        if (res.length > 0) {
          this.boxingarr = res;
        } else {
          this.boxingarr = [];
        }
      });
    },
    visiblechange(value) {
      if (value == true) {
        this.CgBoxesShow();
        this.showchangebox = true;
        const myDate = new Date();
        const year = myDate.getFullYear(); // 获取当前年份
        const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
        const day = myDate.getDate(); // 获取当前日（1-31）
        // 日期格式：2019/07/29 - 2019/07/29
        this.saleDate = `${year}/${month}/${day}`;
      } else {
        // this.BoxcodeDeletefun();
        this.newboxcode = null;
        this.oldBoxCode2 = null;
        this.newboxcodeback = null;
        this.boxcodetype = "1";
      }
    },
    // 删除选定的箱号
    BoxcodeDeletefun() {
      var data = {
        boxCode: this.newboxcodeback
        // date:this.saleDate
      };
      BoxcodeDelete(data).then(res => {
        console.log(res);
      });
    },
    oldBoxDelete(val) {
      var data = {
        boxCode: val
        // date:this.saleDate
      };
      BoxcodeDelete(data).then(res => {});
    },
    cancel() {
      // console.log(this.newboxcodeback)
      this.showEdit = false;
      this.oldBoxCode2 = null;
      this.BoxcodeDeletefun()
    },
    // 提交修改的箱号
    ok_change() {
      if (this.newboxcode != null && this.newboxcode != "") {
        if (this.isclickbtn == true) {
          var data = {
            date: this.saleDate,
            adminID: sessionStorage.getItem("userID"), //装箱人使用当前操作的adminID，必填
            storageID: this.oldboxcode.StorageID, //只在修改箱号中起作用, 如果有库存ID表示：修改某一在库的箱号，必填
            oldBoxCode: this.oldboxcode.BoxCode, //只在修改箱号中起作用, 如果有库存ID表示：必填
            newBoxCode: this.newboxcodeback //只在修改箱号中起作用, 如果有库存ID表示：必填
          };
          // console.log(this.oldboxcode)
          BoxcodeEnter(data).then(res => {
            if (res.success == false) {
              this.$Message.error(res.data);
              this.newboxcodeback = null;
              this.showEdit = true;
            } else {
              this.$Message.success("修改成功");
              this.changeTabs(this.Nameitem);
              this.showEdit = false;
            }
          });
        }
      } else {
        this.$Message.error("请选择或输入箱号");
      }
    },
    changeData(val) {
      this.saleDate = val;
      if (this.newboxcode != "" && this.newboxcode != null) {
        this.handleCreate1(this.newboxcode);
      }
    },
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
