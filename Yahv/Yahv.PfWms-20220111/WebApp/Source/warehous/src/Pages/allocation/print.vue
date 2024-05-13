<style scoped>
ul {
  border: 1px solid #dddddd;
  /* padding-bottom: 40px; */
  border-bottom: 0;
  /* width: 50%; */
}

.item_li {
  line-height: 50px;
  border-bottom: 1px solid #dddddd;
}

.item_li span {
  margin-right: 10px;
}

.left {
  display: inline-block;
  width: 150px;
  text-align: center;
  border-right: 1px solid #dddd;
}

.right {
  width: 19%;
  display: inline-block;
  text-align: center;
}
.printitem{
  display: inline-block;
  width:340px;
  border-right:1px solid #ddd;
  padding-right: 10px;
}
.strong {
  font-weight: bold;
  font-size: 14px;
}
</style>
<template>
  <div>
    <div>
      <ul>
        <li class="item_li">
          <span class="left strong">打印对象</span>
          <span class="right strong printitem">打印机</span>
          <span class="right strong">打印说明</span>
        </li>
        <li v-for="(item,index) in config" class="item_li">
          <span class="left">{{item.Name}}</span>
          <div style="display:height:50px" class="printitem">
            <div v-if="item.Name=='跨越速运打印'">
              <Input v-model="item.PrinterName" placeholder="请输入打印机编号" style="width: 280px" />
              <Tooltip content="保存" placement="right">
                <Icon class="iconstyle" type="md-checkmark-circle-outline" @click="changeinput(index,item.PrinterName)" />
              </Tooltip>

            </div>
            <div v-else>
              <Select v-model="item.PrinterName" style="width:280px" @on-change="changes($event,item)">
                <Option v-for="itemPrinter in printerNames"
                        :value="itemPrinter"
                        :key="itemPrinter">
                  {{ itemPrinter }}
                </Option>
              </Select>
            </div>
          </div>
          
          <span>
            {{item.Summary}}
          </span>
        </li>
      </ul>
      <div style="width:30%;text-align:center;margin-top:20px">
        <!-- <Button type="primary" @click="Printer_btn">打印</Button> -->
      </div>
    </div>
  </div>
  <!-- <div>
    <div>
       <Table border :columns="titles" :data="data">
          <template slot-scope="{ row }" slot="name">
              <strong>{{ row.name }}</strong>
          </template>
          <template slot-scope="{ row }" slot="Printer">
              <Select v-model="row.Text" style="width:200px">
                <Option v-for="item in cityList" :value="item.value" :key="item.value">{{ item.label }}</Option>
              </Select>
          </template>
       </Table>
    </div>
  </div>-->
</template>
<script>
import { mapActions, mapGetters } from "vuex";
// import browser from "@/js/browser"
import { GetPrinterConfig,SetPrinterConfig,GetAllPrinterNames} from "@/js/browser.js";
let Base64 = require("js-base64").Base64;
var printerNames = GetAllPrinterNames();
var configs = GetPrinterConfig();
export default {
  name: "print",
  data() {
    return {
      printerNames:printerNames,
      config: configs
    };
  },
  mounted() {
    this.setnva();
    // alert(JSON.stringify(this.config))
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "打印配置",
          href: "/print"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changeinput(index, item) {
      if (item == '' || item == null) {
        this.$Message.error('请输入正确的打印机编号');
      } else {
        this.config[index].PrinterName = item
        // alert(JSON.stringify(this.config))
        SetPrinterConfig(this.config)
        this.$Message.success("保存成功");
      }
    },
    //changes(key,value){  //修改库房打印机配置的时候，将配置的值传递给后台记录
    //  PageEvent("{\"Name\":\"setprintingset\",\"Value\":\"{\\\"Name\\\":\\\"" +  Base64.encode(JSON.stringify(key)) + "\\\",\\\"Value\\\":\\\"" + value + "\\\"}\"}");
    //}

    changes(key, value) {
      SetPrinterConfig(this.config)
      //修改库房打印机配置的时候，将配置的值传递给后台记录
    }
  }
};
// GetPrinterConfig 获取打印对象
</script>

