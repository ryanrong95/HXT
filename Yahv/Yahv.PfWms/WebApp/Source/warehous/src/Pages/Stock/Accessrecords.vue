<style scoped>
.clearboth{
  clear: both;
}
.searchox{
  padding-bottom: 10px;
}
.search_width {
  width: 20%;
  padding-right: 10px;
  margin-bottom: 15px;
  float: left;
}
.search_width .inputs {
  width: 70%;
}
.pagebox{
    margin-top: 10px;
    text-align: right;
}
</style>
<template>
  <div>
      <div class="searchox">
        <p class="search_width">
          <label for>客户名称：</label>
          <Input v-model.trim="search_data.ClientName" placeholder="客户名称" class="inputs" />
        </p>
        <p class="search_width">
          <label for>型号：</label>
          <Input v-model.trim="search_data.PartNumber" placeholder="型号" class="inputs" />
        </p>
        <p class="search_width">
          <label for>日&nbsp;&nbsp;&nbsp;期：</label>
          <DatePicker
            type="daterange"
            ref="element"
            :clearable="false"
            class="inputs"
            v-model="saleDate"
            @on-change="changeDatePicker"
          ></DatePicker>
        </p>
          <Button type="primary" icon="ios-search" @click="search_btn" style="margin-right:10px">查询</Button>
        <Button type="error" icon="ios-trash" @click="clear_btn" style="margin-right:10px">清空</Button>
    </div>
    <div class="clearboth">
      <Table :columns="columns11" :data="DataArr" :border="DataArr.length > 0" :loading='loading' max-height="560">
        <template slot-scope="{ row, index }" slot="CreateData">
          <p>{{row.Date|showDate}}</p>
        </template>
        <!-- <template slot-scope="{ row, index }" slot="Product">
          <p>空</p>
        </template> -->
        <template slot-scope="{ row, index }" slot="PartNumber">
          <p>{{row.PartNumber}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="Manufacturer">
         <p>{{row.Manufacturer}}</p>
        </template>
        <!-- <template slot-scope="{ row, index }" slot="DateCode">
         <p>空</p>
        </template> -->
        <template slot-scope="{ row, index }" slot="Origin">
          <p>空</p>
        </template>


        <template slot-scope="{ row, index }" slot="lend_price">
          <p>{{row.InUnitPrice}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="lend_quantity">
          <p v-if="row.ShowType==1">{{row.Quantity}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="lend_money">
         <p>{{row.InTotal}}</p>
        </template>


        <template slot-scope="{ row, index }" slot="loan_price">
          <p>{{row.OutUnitPrice}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="loan_quantity">
         <p v-if="row.ShowType==2">{{row.Quantity}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="loan_money">
         <p>{{row.OutTotal}}</p>
        </template>


        <template slot-scope="{ row, index }" slot="profit">
          <p>{{row.profit}}</p>
        </template>


        <template slot-scope="{ row, index }" slot="Settlement_price">
          <p>{{row.BalanceUnitPrice}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="Settlement_quantity">
          <p>{{row.BalanceQuantity}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="Settlement_money">
          <p>{{row.BalanceTotal}}</p>
        </template>
      </Table>
      <div class="pagebox">
        <!-- <Page :total="total" show-elevator :current='search_data.PageIndex' :page-size='search_data.PageSize' @on-change='changepage'/> -->
      <Page :total="total" :page-size="search_data.PageSize" 
          show-total
          :current="search_data.PageIndex" 
          :page-size-opts="showPageArr"
          @on-page-size-change="changepagesize" 
          @on-change="changepage"  
          show-elevator 
          show-sizer />
      </div>
    </div>
  </div>
</template>
<script>
import { CgClientReport } from "../../api/CgApi";
export default {
  name: "Accessrecords",
  data() {
    return {
      total:0,
      loading:true,
      search_data: {
        WarehouseID: sessionStorage.getItem("UserWareHouse"),
        StartDate:null,
        EndDate: null,
        PartNumber: null,
        Manufacturer: null,
        ClientName: null,
        EnterCode: null,
        PageIndex:1,
        PageSize:50

      },
      columns11: [
        {
          title: "日期",
          slot: "CreateData",
          align: "center",
          width: 120,
          fixed: "left"
        },
        // {
        //   title: "品名",
        //   slot: "Product",
        //   align: "center"
        // },
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
        // {
        //   title: "批号",
        //   slot: "DateCode",
        //   align: "center"
        // },
        {
          title: "原产地",
          slot: "Origin",
          align: "center"
        },
        {
          title: "借方",
          align: "center",
          children: [
            {
              title: "单价",
              slot: "lend_price",
              align: "center"
            },
            {
              title: "数量",
              slot: "lend_quantity",
              align: "center",
              width: ""
            },
            {
              title: "金额",
              slot: "lend_money",
              align: "center",
              width: ""
            }
          ]
        },
        {
          title: "贷方",
          align: "center",
          children: [
            {
              title: "单价",
              slot: "loan_price",
              align: "center"
            },
            {
              title: "数量",
              slot: "loan_quantity",
              align: "center",
              width: ""
            },
            {
              title: "金额",
              slot: "loan_money",
              align: "center",
              width: ""
            }
          ]
        },
        {
          title: "出库利润",
          slot: "profit",
          align: "center"
        },
        {
          title: "结存",
          align: "center",
          children: [
            {
              title: "单价",
              slot: "Settlement_price",
              align: "center"
            },
            {
              title: "数量",
              slot: "Settlement_quantity",
              align: "center",
              width: ""
            },
            {
              title: "金额",
              slot: "Settlement_money",
              align: "center",
              width: ""
            }
          ]
        }
      ],
      DataArr: [],
      saleDate:""
    };
  },
  created() {
    this.setnva();
    this.getDates()
  },
  mounted() {
    this.CgClientReport();
  },
  computed:{
     showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  methods: {
      //获取当前时间
      getDates() {
          const myDate = new Date();
          const year = myDate.getFullYear(); // 获取当前年份
          const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
          const day = myDate.getDate(); // 获取当前日（1-31）
          this.saleDate=[`${year}-${month}-${day}`,`${year}-${month}-${day}`]
          this.search_data.StartDate=`${year}-${month}-${day}`
          this.search_data.EndDate=`${year}-${month}-${day}`
    },
    setnva() {
      var cc = [
        {
          title: "出入库单",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    // 获取列表数据
    CgClientReport() {
      CgClientReport(this.search_data) .then(res => {
          console.log(res);
          this.DataArr=res.obj.Data;
          this.total=res.obj.Total
          this.loading=false;
        })
        .catch(error => {
          console.log(error);
        });
    },
    // 时间改变的时候赋值给开始时间与结束时间
    changeDatePicker(val) {
      console.log(val)
      if(val[0]==''&&val[1]==''){
        this.search_data.StartDate=null;
        this.search_data.EndDate==null
      }else{
         this.search_data.StartDate=val[0];
         this.search_data.EndDate==val[1]
      }
    },
    //搜索
    search_btn(){
      this.loading=true
      this.CgClientReport();
    },
    //清空
    clear_btn(){
       this.getDates()
       this.search_data.ClientName=null
       this.search_data.PartNumber=null
       this.CgClientReport();
      //  this.$refs.element.handleClear();
       this.loading=true
       
    },
    changepage(value){
      this.loading=true
      this.search_data.PageIndex=value
      this.CgClientReport();
    },
    changepagesize(value){
      this.loading=true
      this.search_data.PageSize=value
      this.search_data.PageIndex=1
      this.CgClientReport();
    }
  }
};
</script>