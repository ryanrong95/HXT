<template>
  <div>
    <div class="searchbox">
      <div class="inputbox">
        <label>客户名称：</label>
        <Input
          class="inputitem"
          v-model="client"
          placeholder="请输入客户名称"
        />
      </div>
      <div class="inputbox">
        <label>入仓号：</label
        ><Input
          class="inputitem"
          v-model="EnterCode"
          placeholder="请输入入仓号"
        />
      </div>
      <div class="inputbox">
        <label>日期：</label>
        <DatePicker
          type="daterange"
          ref="element"
          :clearable="false"
          class="inputitem"
          v-model="saleDate"
          @on-change="changeDatePicker"
        ></DatePicker>
      </div>
      <Button type="primary" icon="ios-search" @click="search_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
    </div>
    <div>
      <Button type="primary" @click="clickall">修改</Button
      ><span style="margin-left: 5px">注意：可以批量进行修改</span>
    </div>
    <div class="tablebox">
      <Table
        :columns="columns1"
        :data="listdata"
        ref="selection"
        :loading="loading"
        @on-selection-change="changeselection" :max-height="tableHeight">
        <template slot-scope="{ row }" slot="CreateDate"
          >{{ row.CreateDate | showDateexact }}
        </template>
        <template slot-scope="{ row }" slot="Action">
          <Button :disabled='row._disabled==true?true:false' type="primary" size="small" @click="clickitem(row)"
            >修改</Button
          >
        </template>
      </Table>
      <div style="margin-top: 20px; float: right">
        <Page
          :total="total"
          :page-size="PageSize"
          show-total
          :current="PageIndex"
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize"
          @on-change="changepage"
          show-elevator
          show-sizer
        />
      </div>
    </div>
  </div>
</template>
<script>
import { statisticstorageshow, statisticstorageModify } from "../../api/CgApi";
let lodash = require("lodash");
export default {
  data() {
    return {
      columns1: [
        {
          type: "selection",
          width: 60,
          align: "center",
        },
        {
          type: "index",
          width: 60,
          align: "center",
        },
        {
          title: "客户名称",
          key: "Name",
          align: "center",
        },
        {
          title: "入仓号",
          key: "EnterCode",
          align: "center",
        },
        {
          title: "日期",
          slot: "CreateDate",
          align: "center",
        },
        {
          title: "面积",
          key: "Quantity",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Quantity,
                disabled: params.row._disabled
              },
              on: {
                "on-blur"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  if (event.target.value != "") {
                    var reg = /^(\d+)(.\d{0,2})?$/; 
                    if (reg.test(event.target.value) == true) {
                      params.row.Quantity = Number(event.target.value);
                      vm.listdata[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("请输入体积");
                      event.target.value = "";
                      params.row.Quantity = "";
                      vm.listdata[params.index] = params.row;
                    }
                  } else {
                    vm.$Message.error("请输入体积");
                    event.target.value = "";
                    params.row.Quantity = "";
                    vm.listdata[params.index] = params.row;
                  }
                },
              },
            });
          },
        },
        {
          title: "大箱",
          key: "largebox",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Box3Quantity,
                disabled:params.row._disabled
              },
              on: {
                "on-blur"(event) {
                  //值改变时
                  //将渲染后的值重新赋值给单元格值
                  if (event.target.value != "") {
                    var reg = /^[0-9]*$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.Box3Quantity = Number(event.target.value);
                      vm.listdata[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("请输入数量");
                      event.target.value = "";
                      params.row.Box3Quantity = "";
                      vm.listdata[params.index] = params.row;
                    }
                  } else {
                    vm.$Message.error("请输入数量");
                    event.target.value = "";
                    params.row.Box3Quantity = "";
                    vm.listdata[params.index] = params.row;
                  }
                },
              },
            });
          },
        },
        {
          title: "中箱",
          key: "mediumbox",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Box2Quantity, //vm.trim(params.row.Box2Quantity)
                disabled:params.row._disabled
              },
              on: {
                "on-blur"(event) {
                  if (event.target.value != "") {
                    var reg = /^[0-9]*$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.Box2Quantity = Number(event.target.value);
                      vm.listdata[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("请输入数量");
                      event.target.value = "";
                      params.row.Box2Quantity = "";
                      vm.listdata[params.index] = params.row;
                    }
                  } else {
                    vm.$Message.error("请输入数量");
                    event.target.value = "";
                    params.row.Box2Quantity = "";
                    vm.listdata[params.index] = params.row;
                  }
                },
              },
            });
          },
        },
        {
          title: "小箱",
          key: "smallbox",
          align: "center",
          render: (h, params) => {
            var vm = this;
            return h("Input", {
              props: {
                //将单元格的值给Input
                value: params.row.Box1Quantity, //vm.trim(params.row.Box3Quantity)
                disabled:params.row._disabled
              },
              on: {
                "on-blur"(event) {
                  if (event.target.value != "") {
                    var reg = /^[0-9]*$/;
                    if (reg.test(event.target.value) == true) {
                      params.row.Box1Quantity = Number(event.target.value);
                      vm.listdata[params.index] = params.row;
                      vm.clicktest(params.row);
                    } else {
                      vm.$Message.error("请输入数量");
                      event.target.value = "";
                      params.row.Box1Quantity = "";
                      vm.listdata[params.index] = params.row;
                    }
                  } else {
                    vm.$Message.error("请输入数量");
                    event.target.value = "";
                    params.row.Box1Quantity = "";
                    vm.listdata[params.index] = params.row;
                  }
                },
              },
            });
          },
        },
        // {
        //   title: "中箱",
        //   slot: "mediumbox",
        //   align: "center",
        // },
        // {
        //   title: "小箱",
        //   slot: "smallbox",
        //   align: "center",
        // },
        {
          title: "操作",
          slot: "Action",
          align: "center",
        },
      ],
      listdata: [],
      total: 200,
      loading: true,
      client: "", //客户,
      EnterCode: "", //入仓号
      StartDate: "", //开始时间,
      EndDate: "", //结束时间
      PageIndex: 1, //页码
      PageSize: 20, //页数
      saleDate: "",
      Selectdata: [],
      tableHeight:500,
    };
  },
  computed: {
    showPageArr() {
      return this.$store.state.common.PageArr;
    },
  },
  mounted() {
    this.setnva();
    this.getDates();
    this.statisticstorageshow();
    this.tableHeight = window.innerHeight - this.$refs.selection.$el.offsetTop - 100
  },
  methods: {
    trim(str) {
      //去除前后空格
      if (str) {
        return str.replace(/(^\s*)|(\s*$)/g, "");
      }
    },
    setnva() {
      var cc = [
        {
          title: "仓储费管理",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    // 及时更新选中
    clicktest: lodash.throttle(function (paramsrow) {
      for (var i = 0, lens = this.Selectdata.length; i < lens; i++) {
        if (paramsrow.ID == this.Selectdata[i].ID) {
          this.Selectdata[i] = paramsrow;
        }
      }
    }, 0),
    getDates() {
      const myDate = new Date();
      const year = myDate.getFullYear(); // 获取当前年份
      const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
      const day = myDate.getDate(); // 获取当前日（1-31）
      this.saleDate = [`${year}-${month}-${day}`, `${year}-${month}-${day}`];
      this.StartDate = `${year}-${month}-${day}`;
      this.EndDate = `${year}-${month}-${day}`;
    },
    changeDatePicker(val) {
      console.log(val);
      if (val[0] == "" && val[1] == "") {
        this.StartDate = null;
        this.EndDate = null;
      } else {
        this.StartDate = val[0];
        this.EndDate = val[1];
      }
    },
    // 获取列表数据
    statisticstorageshow() {
      var data = {
        WhID: sessionStorage.getItem("UserWareHouse"),
        EnterCode: this.EnterCode,
        StartTime: this.StartDate,
        EndTime: this.EndDate,
        ClientName: this.client,
        PageIndex: this.PageIndex,
        PageSize: this.PageSize,
      };
      console.log(data);
      statisticstorageshow(data).then((res) => {
        console.log(res);
        this.total = res.data.Total;
        this.listdata = res.data.Data;
        this.loading = false;
      });
    },
    changepage(value) {
      //改变页码
      this.loading = true;
      this.PageIndex = value;
      this.statisticstorageshow();
    },
    changepagesize(value) {
      this.loading = true;
      this.PageSize = value;
      this.PageIndex = 1;
      this.statisticstorageshow();
    },
    // 收缩
    search_btn() {
      this.loading = true;
      this.PageIndex = 1;
      this.statisticstorageshow();
    },
    // 清空
    empty_btn() {
      (this.EnterCode = ""),
        (this.StartDate = ""),
        (this.EndDate = ""),
        (this.client = ""),
        (this.PageIndex = 1),
        (this.PageSize = 20),
        this.getDates();
      this.statisticstorageshow();
    },
    //弹出确认提示框
    confirm(type, data) {
      // console.log(data)
      var sumbitdata = [];
      if (type == 0) {
        for (var i = 0; i < data.length; i++) {
          var newdata = {
            Quantity: data[i].Quantity,
            Box1Quantity: data[i].Box1Quantity,
            Box2Quantity: data[i].Box2Quantity,
            Box3Quantity: data[i].Box3Quantity,
            AdminID: data[i].AdminID,
            ID: data[i].ID,
          };
          sumbitdata.push(newdata);
        }
      } else {
        var data = {
          Quantity: data.Quantity,
          Box1Quantity: data.Box1Quantity,
          Box2Quantity: data.Box2Quantity,
          Box3Quantity: data.Box3Quantity,
          AdminID: data.AdminID,
          ID: data.ID,
        };
        sumbitdata.push(data);
      }
      console.log(sumbitdata);
      if (sumbitdata.length > 0) {
        this.$Modal.confirm({
          title: "是否确认修改",
          // content: '',
          onOk: () => {
            // this.$Message.info("Clicked ok");
            this.statisticstorageModify(sumbitdata);
          },
          onCancel: () => {
            // this.$Message.info("Clicked cancel");
          },
        });
      } else {
      }
    },
    changeselection(selection) {
      console.log(selection);
      this.Selectdata = selection;
    },
    clickall() {
      if (this.Selectdata.length <= 0) {
        this.$Message.warning("请至少选择一条");
      } else {
        try {
          this.Selectdata.forEach((item, index) => {
            //跳出条件
            if (item.Box1Quantity!==""&&item.Box2Quantity!==""&&item.Box3Quantity!=="") {
              if (index == this.Selectdata.length - 1) {
                this.confirm(0, this.Selectdata);
              }
            } else {
              this.$Message.error("请输入数量");
              throw new Error("");
            }
          });
        } catch (e) {
          // if (e.message !== "LoopTerminates") throw e;
        }

        // this.confirm(0, this.Selectdata);
      }
    },
    clickitem(row) {
      if (row.Box1Quantity!==""&&row.Box2Quantity!==""&&row.Box3Quantity!=="" ) {
        this.confirm(1, row);
      } else {
        this.$Message.error("请输入数量");
      }
    },
    statisticstorageModify(arr) {
      var data = {
        StatisticStorages: arr,
      };
      statisticstorageModify(data).then((res) => {
        console.log(res);
        if (res.success == true) {
          this.$Message.success("修改成功");
          this.loading = true;
          this.statisticstorageshow();
          this.Selectdata=[]
        } else {
          this.$Message.error(res.data);
        }
      });
    },
  },
};
</script>
<style scoped>
.searchbox {
  padding-bottom: 30px;
}
.inputbox {
  width: 23%;
  display: inline-block;
  margin-right: 15px;
}
.inputitem {
  width: 60%;
}
.tablebox {
  padding-top: 20px;
}
</style>
