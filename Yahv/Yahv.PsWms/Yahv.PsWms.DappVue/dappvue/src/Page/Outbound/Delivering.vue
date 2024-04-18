<style scoped>
.Warehousing .searchinputbox {
  display: inline-block;
  width: 20%;
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
          v-model.trim="FormID"
          placeholder="请输入订单号"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>客户名称：</span>
        <Input
          v-model.trim="ClientName"
          placeholder="请输入客户名称"
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
         @click="cleanbtn"
        >清空</Button
      >
    </div>
    <div class="tablebox">
      <Table :columns="columns1" :data="data1" :loading="loading" :max-height="tableHeight" ref="table">
          <template slot-scope="{ row, index }" slot="formid">
             {{row.FormID}}
          </template>
           <template slot-scope="{ row, index }" slot="ClientName">
             {{row.ClientName}}
          </template>
          <template slot-scope="{ row, index }" slot="CreateDate">
             {{row.CreateDate|showDateexact}}
          </template>
          <template slot-scope="{ row, index }" slot="TakingTime">
            <p v-if="row.Consignee!=undefined"> {{row.Consignee.TakingTime|showDateexact}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="action">
             <Button type="primary" size="small" @click="toDetail(row.ID)">安排</Button>
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
       <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import{Delivery_NotArranged,NoticeDetail} from "../../api/Out"
export default {
  data() {
    return {
      tableHeight:500,
      loading:true,
      total:0,
      showDrawer:false,
      searchdata: {
        name: null,
        orderID: null,
        StartDate: null,
        EndDate: null,
        PageSize:10,
        PageIndex:1,
      },
      FormID:null,
      ClientName:null,
      Status:null,
      PageIndex:1,
      PageSize:20,
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
          title: "客户",
          slot: "ClientName",
        },
        {
          title: "通知时间",
          slot: "CreateDate",
        },
        // {
        //   title: "送货时间",
        //   slot: "TakingTime",
        // },
        
        {
          slot: "action",
          title: "操作",
        },
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03",
          service:"111"
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01",
          service:"111"
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02",
          service:"111"
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04",
          service:"111"
        },
      ],
    };
  },
  computed:{
      showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  created(){
    this.searchbtn()
    this.setnva();
  },
  mounted() {
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "出库送货待安排",
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
      this.PageSize = value;
      this.searchbtn();
    },
    changepage(value) {
      this.PageIndex = value;
      this.searchbtn();
    },
    visiblechange(value){
         if(value==false){
            this.$router.go(-1); //控制路由跳回原来页面
            this.searchbtn();
        }
    },
    toDetail(ID){
        this.showDrawer=true
        this.$router.push({
          name: "DeliveringDeatil",
          params: {
            detailID:ID
          }
        });
    },
    searchbtnclick(){
				this.PageIndex=1
				this.searchbtn()
			},
    searchbtn(){
      this.loading=true
      var data={
          FormID:this.FormID,
          ClientName:this.ClientName,
          Status:this.Status,
          PageIndex:this.PageIndex,
          PageSize:this.PageSize,
          StartDate:this.StartDate,
          EndDate:this.EndDate,
        }
      Delivery_NotArranged(data).then(res=>{
        console.log(res)
        this.data1=res.Data.data;
        this.total=res.Data.Total;
        this.loading=false
      })
    },
    cleanbtn(){
       this.loading=true
       this.FormID=null;
       this.ClientName=null;
       this.Status=null;
       this.PageIndex=1;
       this.StartDate=null
       this.EndDate=null
       this.$refs.element.handleClear();
       this.searchbtn()
    },

  },
};
</script>