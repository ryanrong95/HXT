<style scpoed>
.pagestyle{
      text-align: right;
      padding-top: 15px;
}
</style>
<template>
  <div>
    <div>
      <Input v-model="formdata.key" placeholder="客户编号/订单编号" style="width: 200px" />
      <Select v-model="formdata.status" style="width:200px" placeholder="状态">
        <Option v-for="item in statusarr" :value="item.Status" :key="item.Status">{{ item.StatusDes }}</Option>
      </Select>
       <Button type="primary" @click="query_btn">查询</Button>
       <Button type="error" @click="empty_btn">清空</Button>
    </div>
    <div style="padding-top:40px;">
      <Table  :columns="titles" :data="listdata" :loading="loading">
        <template slot-scope="{ row,index }" slot="indexs">
          <strong>{{ index+1}}</strong>
        </template>
        <template slot-scope="{ row,index }" slot="StartDate">
         {{row.StartDate|showDate}}
        </template>
        <template slot-scope="{ row,index }" slot="EndDate">
         {{row.EndDate|showDate}}
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <!-- <Button type="error" size="small" @click="remove(index)">删除</Button> -->
          <Button v-if="row.Status==2||row.Status==3" type="primary" size="small" style="margin-right: 5px"  @click="show(row.ID)">分配库位</Button>
          <Button v-else  type="primary" size="small" style="margin-right: 5px" @click="show(row.ID)">查看库位</Button>
        </template>
      </Table>
      <div class="pagestyle">
         <Page :total="total" :current="formdata.pageIndex" :page-size="formdata.pageSize" @on-change="chanpage"/>
      </div>
    </div>
    <Drawer title v-model="Spearatedrawer" width="70" @on-visible-change="changevisible">
      <div>
        <router-view></router-view>
      </div>
    </Drawer>
  </div>
</template>
<script>

import {lsnotice,GetStatulsnotice} from "../../api"
import moment from "moment";
export default {
  components: {

  },
  data() {
    return {
      loading:true,
      // Spearatedrawer: false,
      titles:[
          {
          title: "#",
          slot: "indexs",
          align: "center",
          width:50,

        },
        {
          title: "订单编号",
          key: "ID"
        },
        {
          title: "客户编号",
          key: "ClientID"
        },
        {
          title: "租赁开始时间",
          slot: "StartDate"
        },
        {
          title: "租赁结束时间",
          slot: "EndDate"
        },
        {
          title: "状态",
          key: "StatusDes"
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center"
        }
      ],
      listdata:[],
      total:0,
      statusarr:[],
      formdata:{
        key:"",//订单编号/客户编号
        status:"2",//状态
        pageIndex:1,//当前页码
        pageSize:6//每页记录数
      }
    };
  },
  computed: {
    Spearatedrawer:{
      // return this.$store.state.common.closeLeaseDetail;
      get: function () {
      return this.$store.state.common.closeLeaseDetail;
        },
        set: function (newValue) {
          
       }
    },
  },
  mounted() {
    this.setnva();
    this.GetStatulsnotice()
    this.lsnotice(this.formdata)
  },
  filters: {
    showDate: function(val) {  //时间格式转换
      // console.log(val)
      if (val != "") {
        if (val || "") {
          var b = val.split("(")[1];
          var c = b.split(")")[0];
          var result = moment(+c).format("YYYY-MM-DD");
          return result;
        }
      }
    },
  },
  methods: {
    setnva() {
        var cc = [
            {
            title: "在库管理",
            href: "/Stock"
            },
            {
            title: "库位分配",
            href: ""
            }
        ];
        this.$store.dispatch("setnvadata", cc);
    },
    show(index) {
        // this.Spearatedrawer = true;
        this.$store.dispatch("setcloseLd", true);
        // :to="{path:'/Lease/leasedetail',query:{ID:'jspang'}}"
        this.$router.push({ path: '/Lease/leasedetail', query: { ID: index }});
    },
    remove(index) {

    },
    lsnotice(data){
      lsnotice(data).then(res=>{
        console.log(res)
        this.listdata=res.obj.Data;
        this.total=res.obj.Total;
        this.loading=false;
      })
    },
    chanpage(value){
        console.log(value)
        this.formdata.pageIndex=value;
        this.lsnotice(this.formdata)
     },
      query_btn() {  //库区查询
        console.log(this.formdata)
        if(this.formdata.key!=''||this.formdata.status!=""){
          this.formdata.pageIndex=1;
          this.lsnotice(this.formdata)
        }else{
           this.$Message.error('请输入要查询的内容');
        }
      },
      empty_btn() {  //清空按钮
          this.formdata={
              key:"",//订单编号/客户编号
              status:"2",//状态
              pageIndex:1,//当前页码
              pageSize:6//每页记录数
            } 
         this.lsnotice(this.formdata)
      },
      GetStatulsnotice(){
        GetStatulsnotice().then(res=>{
          console.log(res)
          this.statusarr=res.obj;
          this.formdata.status="2";
        })
      },
      changevisible(value){
        console.log(value)
        if(value==true){
          // this.Spearatedrawer=true;
          this.$store.dispatch("setcloseLd", true);
        }else{
          // this.Spearatedrawer=false;
          this.$store.dispatch("setcloseLd", false);
          this.$router.go(-1)
        }
      }
    }
};
</script>