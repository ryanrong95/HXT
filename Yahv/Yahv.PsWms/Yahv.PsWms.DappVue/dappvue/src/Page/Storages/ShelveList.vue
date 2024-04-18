<style scoped>
.Warehousing .searchinputbox {
  display: inline-block;
  width: 19%;
  padding-right: 20px;
}
.Warehousing .secrchinput {
  width: 75%;
}
.Warehousing .tablebox {
  padding-top: 10px;
}
.Warehousing .pages {
  float: right;
  padding-top: 20px;
}
.addinput{
  width: 82%;
}
.lable{
  display: inline-block;
  width: 16%;
  text-align: right;
}
.bitian{
  color: red;
}
</style>
<template>
  <div class="Warehousing">
    <div class="serchbox">
      <p class="searchinputbox">
        <span>订单ID：</span>
        <Input
          v-model="searchdata.OrderID"
          placeholder="请输入订单ID"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>所属公司：</span>
        <Input
          v-model="searchdata.Company"
          placeholder="请输入所属公司"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>库位：</span>
        <Input
          v-model="searchdata.ShelveCode"
          placeholder="请输入库位"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>型号：</span>
        <Input
          v-model="searchdata.Partnumber"
          placeholder="请输入型号"
          class="secrchinput"
        />
      </p>
      <Button
        type="primary"
        icon="ios-search"
        style="margin-right: 10px"
        @click="searchbtnclick"
        >查询</Button
      >
      <Button
        type="error"
        icon="ios-trash"
        style="margin-right: 10px"
        @click="cancelbtn"
        >清空</Button
      >
    </div>
    <div class="tablebox">
      <div style="padding-bottom: 10px">
        <Button
          type="success"
          size="small"
          icon="md-add"
          @click="addShelve = true"
        >
          新增库位</Button
        >
      </div>
      <Table
        :columns="columns1"
        :data="data1"
        :loading="loading"
        :max-height="tableHeight"
        ref="table"
      >
        <template slot-scope="{ row, index }" slot="action">
          <div v-if="row.IsCanDelete == false">
            <Button type="primary" size="small" @click="printShelve(row)">打印</Button>
            <Button type="primary" size="small" @click="toDetail(row.ID)">详情</Button>
          </div>
          <div v-if="row.IsCanDelete == true">
            <Button type="primary" size="small" @click="printShelve(row)">打印</Button>
            <Button type="primary" size="small" @click="DeleteShelve(row)">删除</Button>
          </div>
        </template>
      </Table>
      <div class="pages">
        <!-- <Page :total="total"  @on-change="changepage" :page-size="10" :current="search_datas.PageIndex" /> -->
        <Page
          :total="total"
          :page-size="searchdata.PageSize"
          show-total
          :current="searchdata.PageIndex"
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize"
          @on-change="changepage"
          show-elevator
          show-sizer
        />
      </div>
    </div>
    <Drawer :width="92" v-model="showDrawer" @on-visible-change="visiblechange">
      <div
        slot="close"
        style="font-size: 28px; color: #000000; padding-right: 10px"
      >
        <Icon type="md-close" />
      </div>
      <router-view></router-view>
    </Drawer>
    <!-- 新增库位 -->
    <Modal v-model="addShelve" title="新增库位"  @on-visible-change="visiblechangeadd">
      <div>
        <p style="padding-bottom:8px">
          <span class="lable"><em class="bitian">*</em>库位：</span>
          <Input
            v-model="addShelvedata.Code"
            placeholder="请输入库位号"
            class="addinput"
          />
        </p>
        <p style="padding-bottom:8px">
          <span class="lable"><em class="bitian">*</em>尺寸：</span>
          <Input
            v-model="addShelvedata.Size"
            placeholder="请输入尺寸"
            class="addinput"
          />
        </p>
        <p style="padding-bottom:8px">
          <span class="lable"><em class="bitian">*</em>所属公司：</span>
          <Input
            v-model="addShelvedata.Company"
            placeholder="请输入所属公司"
            class="addinput"
          />
        </p>
        <p>
          <span class="lable">备注：</span>
          <Input
            v-model="addShelvedata.Summary"
            placeholder="请输入备注"
            class="addinput"
          />
        </p>
      </div>
      <div slot="footer">
          <Button type="text"  @click="cancel">取消</Button>
          <Button type="primary" @click="ok">确定</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
import { ShelveList,ShelveEnter,ShelveDelete} from "../../api/Storages";
import {TemplatePrint,GetPrinterDictionary } from "../../js/browser";
let product_url = require("../../../static/pubilc.dev");
export default {
  data() {
    return {
      addShelve: false,
      loading: true,
      showDrawer: false,
      total: 0,
      searchdata: {
        OrderID: "", //查询参数，（入库）订单ID
        Company: "", //查询参数，所属公司
        ShelveCode: "", //查询参数，库位
        Partnumber: "", //查询参数，型号
        PageIndex: 1, //分页参数，第几页
        PageSize: 20, //分页参数，每页几条数据
      },
      columns1: [
        //  {
        //     type: 'index',
        //     width: 60,
        //     align: 'center'
        // },
        {
          title: "库位",
          key: "ShelveCode",
        },
        {
          title: "所属公司",
          key: "Company",
        },
        {
          title: "尺寸",
          key: "Size",
        },
        {
          title: "当前情况",
          key: "CurrentStorage",
        },
        {
          title: "备注",
          key: "Summary",
        },
        {
          slot: "action",
          title: "操作",
        },
      ],
      data1: [],
      tableHeight: 500,
      addShelvedata: {
        Code: null, //库位
        Size:null, //尺寸
        Company:null, //所属公司
        Summary:null, //备注
      },
      printurl: product_url.pfwms,
    };
  },
  computed: {
    showPageArr() {
      return this.$store.state.common.PageArr;
    },
  },
  created() {
    this.searchbtn();
  },
  mounted() {
    this.setnva();
   this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "库位管理",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changepagesize(value) {
      this.searchdata.PageSize = value;
      this.searchbtn();
    },
    changepage(value) {
      this.searchdata.PageIndex = value;
      this.searchbtn();
    },
    visiblechange(value) {
      if (value == false) {
        this.$router.go(-1); //控制路由跳回原来页面
      }
    },
    searchbtnclick(){
				this.searchdata.PageIndex=1
				this.searchbtn()
			},
    searchbtn() {
      this.loading = true;
      ShelveList(this.searchdata).then((res) => {
        this.data1 = res.data.Data;
        this.total = res.data.Total;
        this.loading = false;
      });
    },
    cancelbtn() {
      this.searchdata.OrderID = null;
      this.searchdata.Company = null;
      this.searchdata.ShelveCode = null;
      this.searchdata.Partnumber = null;
      this.searchdata.PageIndex = 1;
      this.searchbtn();
    },
    ok() {
      console.log(!this.addShelvedata.Code!=true)
      if(!this.addShelvedata.Code!=true&&!this.addShelvedata.Size!=true&&!this.addShelvedata.Company!=true){
        ShelveEnter(this.addShelvedata).then(res=>{
          if(res.success==true){
            this.$Message.success('新增库位成功');
            this.addShelve=false
            this.searchbtnclick()
          }else{
            this.addShelve=true
            this.$Message.error(res.data);
          }
        })
      }else{
        this.addShelve=true
        this.$Message.warning('请输入必填项');
      }
    },
    cancel() {
      this.addShelve=false
    },
    visiblechangeadd(value){
      if(value==false){
        this.addShelvedata.Code=null;
        this.addShelvedata.Size=null;
        this.addShelvedata.Company=null;
        this.addShelvedata.Summary=null;
      }
    },
    DeleteShelve(row){
      var data={
        "id":row.ID
      }
      ShelveDelete(data).then(res=>{
          if(res.success==true){
            this.$Message.success('库位删除成功');
            this.searchbtn();
          }else{
            this.$Message.error(res.data);
          }
      })
    },
    toDetail(ID){
        this.showDrawer=true
        this.$router.push({
          name: "ShelveDetail",
          params: {
            detailID:ID
          }
        });
    },
    printShelve(row){
        var configs = GetPrinterDictionary();
      var getsetting = configs["库位标签"];
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
            listdata:row
          }
        ]
      };
      TemplatePrint(data);
       this.$Message.success('打印成功');
    }
  },
};
</script>