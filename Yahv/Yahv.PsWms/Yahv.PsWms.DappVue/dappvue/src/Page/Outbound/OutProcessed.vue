<style scoped>
.Warehousing .searchinputbox {
  display: inline-block;
  width: 16%;
  padding-right: 20px;
}
.Warehousing .secrchinput {
  width: 75%;
}
.Warehousing .tablebox{
    padding-top: 20px;
}
.Warehousing .pages {
  float: right;
  padding-top: 20px;
}
</style>
<template>
  <div class="Warehousing">
    <div class="serchbox">
      <p class="searchinputbox">
        <span>订单ID：</span>
        <Input
          v-model.trim="FormID"
          placeholder="请输入订单号"
          class="secrchinput"
        />
      </p>
       <p class="searchinputbox">
        <span>型号：</span>
        <Input
          v-model.trim="Partnumber"
          placeholder="请输入型号"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>客户：</span>
        <Input
          v-model.trim="ClientName"
          placeholder="请输入客户"
          class="secrchinput"
        />
      </p>
       <p class="searchinputbox">
        <span>状态：</span>
         <Select v-model="Status"  class="secrchinput">
            <Option v-for="item in Statusarr" :value="item.value" :key="item.value">{{ item.name }}</Option>
        </Select>
      </p>
      <p class="searchinputbox">
        <span>日期：</span>
        <DatePicker
          ref="element"
          type="daterange"
          placement="bottom-end"
          placeholder
          :editable="false"
          separator="  至  "
          @on-change="changedata"
          class="secrchinput"
        ></DatePicker>
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
        @click="cleanbtn"
        >清空</Button
      >
    </div>
    <div class="tablebox">
      <Table :columns="columns1" :data="data1" :loading='loading' :max-height="tableHeight" ref="table" >
        <template slot-scope="{ row, index }" slot="formid">
         <p> {{ row.FormID}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="TransportModeDec">
         <p> {{ row.TransportModeDec}}
           <span v-if="row.TransportModeDec=='快递'&&row.Consignee.WaybillCode!=null">({{row.Consignee.WaybillCode}})</span>
         </p>
        </template>
        <template slot-scope="{ row, index }" slot="ClientName">
         <p>{{ row.ClientName }}</p> 
        </template>
        <template slot-scope="{ row, index }" slot="TrackerName">
         <p>{{ row.TrackerName }}</p> 
        </template>
        <template slot-scope="{ row, index }" slot="StatusDec">
         <p>{{ row.StatusDec }}</p> 
        </template>
         <template slot-scope="{ row, index }" slot="CreateDate">
          {{ row.CreateDate|showDateexact }}
        </template>
          <template slot-scope="{ row, index }" slot="action">
            <Button type="primary" size="small" @click="toDetail(row.ID)">查看</Button>
        </template>
      </Table>
       <div class="pages">
        <!-- <Page :total="total"  @on-change="changepage" :page-size="10" :current="search_datas.PageIndex" /> -->
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
     <Drawer 
     :width='92'
     v-model="showDrawer"
     @on-visible-change='visiblechange'>
       <div slot="close" style="font-size: 28px; color: #000000;padding-right: 10px;">
           <Icon type="md-close" />
       </div>
       <router-view :key="key"></router-view>
    </Drawer>
  </div>
</template>
<script>

import { Show_Exited,NoticeStatus_Complete } from "../../api/Out";
export default {
  data() {
    return {
      tableHeight:500,
       loading:true,
       showDrawer:false,
       total:0,
      Partnumber:null,
      FormID: null,
      ClientName: null,
      Status: null,
      PageIndex: 1,
      PageSize: 20,
      StartDate: null,
      EndDate: null,
      columns1: [
         {
            type: 'index',
            width: 60,
            align: 'center'
        },
        {
          title: "订单ID",
          slot: "formid",
        },
        {
          title: "货运类型",
          slot: "TransportModeDec",
        },
        {
          title: "状态",
          slot: "StatusDec",
        },
        {
          title: "客户",
          key: "ClientName",
        },
        {
          title: "客服",
          slot: "TrackerName",
        },
        {
          title: "通知时间",
          slot: "CreateDate",
        },
        {
          slot: "action",
          title: "操作",
        },
      ],
      data1: [],
      Statusarr: [ ],
    };
  },
  computed:{
      showPageArr(){
        return this.$store.state.common.PageArr;
      },
      key() {
        return this.$route.name !== undefined? this.$route.name +new Date(): this.$route +new Date()
      }
  },
  created(){
    this.NoticeStatus()
    this.searchbtn()
  },
  mounted() {
    this.setnva();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140
  },
  methods: {
    NoticeStatus(){
      NoticeStatus_Complete().then(res=>{
        console.log(res)
        this.Statusarr=res.obj
      })
    },
    setnva() {
      var cc = [
        {
          title: "出库通知已处理",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changedata(value) {
      if(value[0]==''||value[1]==''){
        this.StartDate=null
        this.EndDate = null;
      }else{
         this.StartDate = value[0];
         this.EndDate = value[1];
      }
    },

     changepagesize(value) {
      console.log(value);
      this.PageSize = value;
      this.searchbtn();
    },
    changepage(value) {
      console.log(value);
      this.PageIndex = value;
      this.searchbtn();
    },
    toDetail(ID){
        this.showDrawer=true
        this.$router.push({
          name: "OutProcessedDetail",
          params: {
            detailID:ID
          }
        });
    },
    visiblechange(value){
         if(value==false){
            this.$router.go(-1); //控制路由跳回原来页面
            this.searchbtn()
        }
    },
    searchbtnclick(){
				this.PageIndex=1
				this.searchbtn()
			}, 
     searchbtn() {
      this.loading=true
      var data = {
        Partnumber:this.Partnumber,
        FormID: this.FormID,
        ClientName: this.ClientName,
        Status: this.Status,
        PageIndex: this.PageIndex,
        PageSize: this.PageSize,
        StartDate: this.StartDate,
        EndDate:this.EndDate,
      };
      console.log(data);
      Show_Exited(data).then((res) => {
        this.loading=false
        console.log(res);
        this.data1 = res.Data.data;
        this.total = res.Data.Total;
      });
    },
    cleanbtn() {
      this.loading=true
      this.Partnumber=null,
      this.FormID = null;
      this.ClientName = null;
      this.Status = null;
      this.PageIndex = 1;
      this.StartDate=null
      this.EndDate=null
      this.$refs.element.handleClear();
      this.searchbtn()
    },
  },
};
</script>