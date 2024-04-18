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
        <span>订单：</span>
        <Input
          v-model.trim="searchdata.FormID"
          placeholder="请输入订单号"
          class="secrchinput"
        />
      </p>
       <p class="searchinputbox">
        <span>型号：</span>
        <Input
          v-model.trim="searchdata.Partnumber"
          placeholder="请输入型号"
          class="secrchinput"
        />
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
        @click="cancelbtn"
        >清空</Button
      >
    </div>
    <div class="tablebox">
      <Table :columns="columns1" :data="data1" :loading="loading" :max-height="tableHeight" ref="table">
        <template slot-scope="{ row, index }" slot="CreateDate">
          {{row.CreateDate|showDate}}
        </template>
          <template slot-scope="{ row, index }" slot="action">
            <Button type="primary" size="small" @click="toDetail(row.ID)">分拣</Button>
        </template>
      </Table>
       <div class="pages">
        <!-- <Page :total="total"  @on-change="changepage" :page-size="10" :current="search_datas.PageIndex" /> -->
        <Page :total="total" :page-size="searchdata.PageSize" 
          show-total
          :current="searchdata.PageIndex" 
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize" 
          @on-change="changepage"  
          show-elevator 
          show-sizer />
      </div>
    </div>
     <Drawer 
     :width='92'
     v-model="showDrawer"
     @on-visible-change='visiblechange'>
       <div slot="close" style="font-size: 28px; color: #000000;padding-right: 10px;">
           <Icon type="md-close" />
       </div>
       <router-view ></router-view>
    </Drawer>
  </div>
</template>
<script>
import {InNoticesShow} from "../../api/Enter"
export default {
  data() {
    return {
       loading: true,
       showDrawer:false,
       total:0,
      searchdata: {
        WarehouseID:'SZ',
        Status:1,
        Partnumber: null,
        FormID: null,
        Start: null,
        End: null,
        PageSize:20,
        PageIndex:1,
      },
      columns1: [
         {
            type: 'index',
            width: 60,
            align: 'center'
        },
        {
          title: "订单ID",
          key: "FormID",
        },
        {
          title: "货运类型",
          key: "TransportModeDes",
        },
        {
          title: "客户",
          key: "ClientName",
        },
        {
          title: "客服",
          key: "TrackerName",
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
      tableHeight:500,
    };
  },
  computed:{
      showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  created(){
    this.searchbtn()
  },
  mounted() {
    this.setnva();
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140

  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "入库通知待处理",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changedata(value) {
      this.searchdata.Start = value[0];
      this.searchdata.End = value[1];
    },

    changepagesize(value){
      this.searchdata.PageSize=value
      this.searchbtn()
    },
    changepage(value) {
      this.searchdata.PageIndex=value
      this.searchbtn()
    },
    toDetail(ID){
        this.showDrawer=true
        this.$router.push({
          name: "NoticeDetail",
          params: {
            detailID:ID
          }
        });
    },
    visiblechange(value){
         if(value==false){
            this.$router.go(-1); //控制路由跳回原来页面
        }
    },
    searchbtnclick(){
      this.searchdata.PageIndex=1;
      this.searchbtn()
    },
    searchbtn(){
      this.loading=true
      InNoticesShow(this.searchdata).then(res=>{
        this.data1=res.data.Data
        this.total=res.data.Total
        this.loading=false
      })
    },
    cancelbtn(){
      this.searchdata.Partnumber=null;
      this.searchdata.FormID=null;
      this.searchdata.Start=null;
      this.searchdata.End=null;
      this.searchdata.PageIndex=1;
      this.$refs.element.handleClear();
      this.searchbtn()
    }
  },
};
</script>