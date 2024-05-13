<style scoped>
.search_width {
  width: 22%;
  padding-right: 10px;
  margin-bottom: 15px;
  float: left;
}
.search_width .inputs {
 width: 65%;
}
label {
  font-size: 14px;
  line-height: 32px;
}
.tablebox {
  clear: both;
  padding-top: 15px;
}
.subCol li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  padding: 9px;
  border-bottom: 1px solid #ccc;
  overflow-x: hidden;
  min-height: 37px;
}
.subCol li:last-child {
  border-bottom: none;
}
</style>
<template>
  <div>
    <!-- 搜索条件 -->
    <div>
      <p class="search_width">
        <label for>客户名称：</label>
        <Input v-model="search_data.ClientName" placeholder="客户名称" class="inputs" />
      </p>
      <p class="search_width">
        <label for>入&nbsp;&nbsp;仓&nbsp;&nbsp;号：</label>
        <Input v-model="search_data.EnterCode" placeholder="入仓号" class="inputs" />
      </p>
      <p class="search_width">
        <label for>供应商：</label>
        <Input v-model="search_data.Supplier" placeholder="供应商" class="inputs" />
      </p>
      <p class="search_width">
        <label for>订&nbsp;&nbsp;单&nbsp;&nbsp;号：</label>
        <Input v-model="search_data.OrderID" placeholder="订单号" class="inputs" />
      </p>
      <p class="search_width">
        <label for>型&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;号：</label>
        <Input v-model="search_data.PartNumber" placeholder="型号" class="inputs" />
      </p>
      <p class="search_width">
        <label for>品&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;牌：</label>
        <Input v-model="search_data.Manufacturer" placeholder="品牌" class="inputs" />
      </p>
       <p class="search_width">
        <label for>日&nbsp;&nbsp;&nbsp;期：</label>
        <DatePicker type="daterange" ref='element' :clearable="false" class="inputs" @on-change='changeDatePicker'></DatePicker>
      </p>
      <p class="search_width">
        <label for>状&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;态：</label>
        <Select v-model="Status" class="inputs">
          <Option v-for="(item,index) in statusList" :value="item.value" :key="index">{{ item.label }}</Option>
        </Select>
      </p>
      <p class="search_width">
        <label for>业务类型：</label>
        <Select v-model="search_data.Source" class="inputs">
          <Option v-for="item in SourceData" :value="item.value" :key="item.value">{{ item.name }}</Option>
        </Select>
      </p>
      <p class="search_width">
        <label for>操&nbsp;&nbsp;作&nbsp;&nbsp;人：</label>
        <Input v-model="search_data.AdminName" placeholder="操作人" class="inputs" />
      </p>
      <p class="search_width">
        <Button type="primary" icon="ios-search" @click="search_btn" style="margin-right:10px">查询</Button>
        <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
      </p>
    </div>
    <!-- 列表展示 -->
    <div class="tablebox">
      <Table :columns="columns" :data="data1" :border="data1.length > 0" :loading="loading" ref="table" :max-height="tableHeight">
          <template slot-scope="{ row, index }" slot="CustomName" >
             <p>{{row.ClientName}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="EnterCode" >
             <p>{{row.EnterCode}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="Supplier" >
              <p>{{row.Supplier}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="PartNumber" >
              <p>{{row.PartNumber}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="Manufacturer" >
            <ul class="subCol">
              <li v-for="(itemD,index) in row.Datas">{{itemD.Product.Manufacturer}}</li>
            </ul>
          </template>
          <template slot-scope="{ row, index }" slot="Datacode" >
             <ul class="subCol">
              <li v-for="(itemD,index) in row.Datas">{{itemD.DateCode}}</li>
            </ul>
          </template>
          <template slot-scope="{ row, index }" slot="Origin" >
             <ul class="subCol">
              <li v-for="(itemD,index) in row.Datas">{{itemD.Origin}}</li>
            </ul>
          </template>
          <template slot-scope="{ row, index }" slot="YdQty" >
             <p>{{row.YdQty}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="SdQty" >
             <p>{{row.SdQty}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="Price" >
            <ul class="subCol">
             <li v-for="(itemD,index) in row.Datas">{{itemD.Currency}}{{itemD.UnitPrice}}</li>
            </ul>
          </template>
          <template slot-scope="{ row, index }" slot="Summary" >
             <ul class="subCol">
             <li v-for="(itemD,index) in row.Datas">{{itemD.Summary}}</li>
            </ul>
          </template>
           <template slot-scope="{ row, index }" slot="LastEnterDate" >
             <p>{{row.LastEnterDate|showDateexact}}</p>
          </template>
          <template slot-scope="{ row, index }" slot="AdminName" >
              <ul class="subCol">
               <li v-for="(itemD,index) in row.Datas">{{itemD.SortingAdminRealName}}</li>
              </ul>
          </template>
      </Table>
      <div style="text-align: right;padding-top:15px;padding-bottom: 30px;">
        <Page :total="total" 
        show-total
        show-elevator 
        show-sizer
        :current='search_data.PageIndex' 
        @on-change='changePage' 
        :page-size='search_data.PageSize'
        :page-size-opts="showPageArr"
        @on-page-size-change="changepagesize" 
        />
      </div>
    </div>
  </div>
</template>
<script>
import { CgInputReportGroup,NoticeSource} from "../../api/CgApi";
export default {
  data() {
    return {
      total:0,//总条数
      loading:true,
      titlelable1: [
        {
          type: 'index',
          width: 60,
          align: 'center'
        },
        {
          title: "客户",
          slot: "CustomName",
          align: 'center'
        },
        {
          title: "入仓号",
          slot: "EnterCode",
          align: 'center'
        },
        {
          title: "供应商",
          slot: "Supplier",
          align: 'center'
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: 'center'
        },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: 'center'
        },
        {
          title: "批号",
          slot: "Datacode",
          align: 'center'
        },
        {
          title: "原产地",
          slot: "Origin",
          align: 'center'
        },
        {
          title: "应到",
          slot: "YdQty",
          align: 'center'
        }
        ,
        {
          title: "实到",
          slot: "SdQty",
          align: 'center'
        },
        {
          title: "核算单价",
          slot: "Price",
          align: 'center'
        },
        {
          title:"操作人",
          slot:'AdminName',
          align:'center'
        },
        {
          title:"入库时间",
          slot:'LastEnterDate',
          align:'center'
        },
        
      ],
      titlelable2: [
        {
          type: 'index',
          width: 60,
          align: 'center'
        },
        {
          title: "客户",
          slot: "CustomName",
          align: 'center'
        },
        {
          title: "入仓号",
          slot: "EnterCode",
          align: 'center'
        },
        {
          title: "供应商",
          slot: "Supplier",
          align: 'center'
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: 'center'
        },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: 'center'
        },
        {
          title: "批号",
          slot: "Datacode",
          align: 'center'
        },
        {
          title: "原产地",
          slot: "Origin",
          align: 'center'
        },
        {
          title: "应到",
          slot: "YdQty",
          align: 'center'
        }
        ,
        {
          title: "实到",
          slot: "SdQty",
          align: 'center'
        },
        {
          title: "核算单价",
          slot: "Price",
          align: 'center'
        },
        {
          title: "异常原因",
          slot: "Summary",
          align: 'center'
        },
        {
          title:"操作人",
          slot:'AdminName',
          align:'center'
        },
        {
          title:"入库时间",
          slot:'LastEnterDate',
          align:'center'
        },
        
      ],
      columns:null,
      data1: [],
      statusList: [
        {
          value: "1",
          label: "正常入库"
        },
        {
          value: "2",
          label: "异常入库"
        }
      ],
      Status:'1',
      SourceData:[],
      search_data: {
        WarehouseID: sessionStorage.getItem("UserWareHouse"),
        OrderID: null,
        StartDate: null,
        EndDate: null,
        Source: 1,
        PartNumber: null,
        Manufacturer: null,
        Supplier: null,
        ClientID: null,
        ClientName: null,
        EnterCode: null,
        Status: true,
        PageIndex:1,
        PageSize:20,
        AdminName:null,
      },
      tableHeight:500,
    };
  },
  created() {
    this.columns=this.titlelable1
    this.setnva();
    this.NoticeSource()
    this.CgInputReportGroup(this.search_data);
  },
  computed:{
    showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  mounted() {
   this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "入库单",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    //搜索
    search_btn() {
      var Isstatus = null;
      if (this.Status == "1") {
       this.search_data.Status = true;
       this.columns=this.titlelable1
      } else {
         this.search_data.Status = false;
          this.columns=this.titlelable2
      }
      this.search_data.PageIndex=1
      this.loading=true
      this.CgInputReportGroup(this.search_data);
    },
    //清空
    clear_btn() {
      this.loading=true
      this.search_data.OrderID=null;
      this.search_data.StartDate=null;
      this.search_data.StartDate=null;
      this.search_data.Source=1;
      this.search_data.PartNumber=null;
      this.search_data.Manufacturer=null;
      this.search_data.Supplier=null;
      this.search_data.ClientName=null;
      this.search_data.EnterCode=null;
      this.search_data.Status=true;
      this.search_data.PageIndex=1;
      this.search_data.AdminName=null;
      this.Status = "1"
      this.$refs.element.handleClear();
      this.CgInputReportGroup(this.search_data);
    },
    //获取列表数据
    CgInputReportGroup(data) {
      CgInputReportGroup(data).then(res => {
        console.log(res);
        this.total=res.Total
        this.data1=res.Data
        this.loading=false;
      });
    },
    // 获取业务类型的枚举
    NoticeSource(){
      NoticeSource().then(res=>{
        this.SourceData=res.obj;
      })
    },
    //时间发生变化的时候，修改搜索的开始始建于结束时间
    changeDatePicker(value){
      console.log(value)
      if(value[0]==''||value[1]==''){
        this.search_data.StartDate=null
        this.search_data.EndDate=null
      }else{
        this.search_data.StartDate=value[0]
        this.search_data.EndDate=value[1]
      }
    },
    //页码改变的时候，请求当前页码数据
    changePage(value){
     this.loading=true;
     this.search_data.PageIndex=value
     this.CgInputReportGroup(this.search_data);
    },
    changepagesize(value){
       this.loading=true;
       this.search_data.PageSize=value
       this.search_data.PageIndex=1
       this.CgInputReportGroup(this.search_data);
    }
  }
};
</script>