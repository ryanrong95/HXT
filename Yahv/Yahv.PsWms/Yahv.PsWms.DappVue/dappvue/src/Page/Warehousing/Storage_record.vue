<style scoped>
.Warehousing .searchinputbox {
  display: inline-block;
  width: 20%;
  padding-right: 20px;
}
.Warehousing .secrchinput {
  width: 75%;
}
.Warehousing .tablebox {
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
        <span>型号：</span>
        <Input
          v-model="PartNumber"
          placeholder="请输入型号"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>订单号：</span>
        <Input
          v-model="FormID"
          placeholder="请输入品牌"
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
          :clearable='false'
          separator="  至  "
          @on-change="changedata"
          class="secrchinput"
          v-model="saleDate"
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
         <Tabs v-model="Status" @on-click="changetabel">
            <TabPane label="正常分拣" name="true" > 
                 <Table :columns="columns1" :data="data1" :loading="loading">
                    <template slot-scope="{ row, index }" slot="formid">
                    {{ row.FormID }}
                    </template>
                    <template slot-scope="{ row, index }" slot="Partnumber">
                    {{ row.Product.Partnumber }}
                    </template>
                    <template slot-scope="{ row, index }" slot="Brand">
                    <p >{{ row.Product.Brand }}</p>
                    </template>
                    <template slot-scope="{ row, index }" slot="Package">
                    <p >{{ row.Product.Package }}</p>
                    </template>
                    <template slot-scope="{ row, index }" slot="DateCode">
                    {{ row.Product.DateCode}}
                    </template>
                    <!-- <template slot-scope="{ row, index }" slot="Quantity">
                     <p>Mpq:{{row.StorageMpq}}&nbsp;/&nbsp;{{ row.StoragePackageNumber}}&nbsp;/&nbsp;{{row.StorageTotal}}</p>  
                    </template> -->
                    <template slot-scope="{ row, index }" slot="CreateDate">
                     {{ row.CreateDate| showDateexact}}
                    </template>
                </Table>
            </TabPane>
            <TabPane label="异常到货" name="false">
                <Table :columns="columns2" :data="data2" :loading="loading">
                    <template slot-scope="{ row, index }" slot="formid">
                    {{ row.FormID }}
                    </template>
                    <template slot-scope="{ row, index }" slot="Partnumber">
                    {{ row.Product.Partnumber }}
                    </template>
                    <template slot-scope="{ row, index }" slot="Brand">
                    <p >{{ row.Product.Brand }}</p>
                    </template>
                    <template slot-scope="{ row, index }" slot="Package">
                    <p >{{ row.Product.Package }}</p>
                    </template>
                    <template slot-scope="{ row, index }" slot="DateCode">
                    {{ row.Product.DateCode}}
                    </template>
                    <!-- <template slot-scope="{ row, index }" slot="StorageMpq">
                      <p>Mpq:{{row.StorageMpq}}&nbsp;/&nbsp;{{ row.StoragePackageNumber}}&nbsp;/&nbsp;{{row.StorageTotal}}</p>                    
                    </template>
                     <template slot-scope="{ row, index }" slot="StorageMpq">
                      <p>Mpq:{{row.StorageMpq}}&nbsp;/&nbsp;{{ row.StoragePackageNumber}}&nbsp;/&nbsp;{{row.StorageTotal}}</p>                    
                    </template>
                     <template slot-scope="{ row, index }" slot="StorageMpq">
                      <p>Mpq:{{row.StorageMpq}}&nbsp;/&nbsp;{{ row.StoragePackageNumber}}&nbsp;/&nbsp;{{row.StorageTotal}}</p>                    
                    </template> -->
                    <template slot-scope="{ row, index }" slot="CreateDate">
                     {{ row.CreateDate| showDateexact}}
                    </template>
                </Table>
            </TabPane>
        </Tabs>
        <div class="pages">
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
import { Show_Product_Inbound } from "../../api/Enter";
export default {
  data() {
    return {
      loading:true,
      total: 100,
      PartNumber:null,
      FormID:null,
      PageIndex: 1,
      PageSize: 20,
      Start: null,
      End: null,
      Status:"true",
      saleDate:null,
      columns1: [
        {
          type: "index",
          width: 60,
          align: "center",
        },
         {
          title: "订单ID",
          slot: "formid",
          align: "center",
        },
        {
          title: "型号",
          slot: "Partnumber",
          align: "center",
        },
        {
          title: "品牌",
          slot: "Brand",
          align: "center",
        },
        {
          title: "封装",
          slot: "Package",
          align: "center",
        },
        {
          title: "批次",
          slot: "DateCode",
          align: "center",
        },
        // {
        //   renderHeader: (h, params) => {
        //     return h("div", [
        //       h("span", {}, "Mpq / 已分拣(件) / 已分拣总量")
        //     ]);
        //  },
        //   slot: "Quantity",
        //   align: "center",
        // },
        {
          title: "Mpq",
          key: "StorageMpq",
          align: "center",
        },
        {
          title: "已分拣(件)",
          key: "StoragePackageNumber",
          align: "center",
        },
        {
          title: "已分拣总量",
          key: "StorageTotal",
          align: "center",
        },
        {
          title: "入库日期",
          slot: "CreateDate",
          align: "center",
        },
      ],
       columns2: [
        {
          type: "index",
          width: 60,
          align: "center",
        },
         {
          title: "订单ID",
          slot: "formid",
          align: "center",
        },
        {
          title: "型号",
          slot: "Partnumber",
          align: "center",
        },
        {
          title: "品牌",
          slot: "Brand",
          align: "center",
        },
        {
          title: "封装",
          slot: "Package",
          align: "center",
        },
        {
          title: "批次",
          slot: "DateCode",
          align: "center",
        },
        // {
        //   title: "Mpq / 已分拣(件) / 已分拣总量",
        //   slot: "Quantity",
        //   align: "center",
        //   width:203
        // },
        {
          title: "Mpq",
          key: "StorageMpq",
          align: "center",
        },
        {
          title: "已分拣(件)",
          key: "StoragePackageNumber",
          align: "center",
        },
        {
          title: "已分拣总量",
          key: "StorageTotal",
          align: "center",
        },
        {
          title: "异常",
          key: "Exception",
          align: "center",
        },
        {
          title: "入库日期",
          slot: "CreateDate",
          align: "center",
        },
      ],
      data1: [],
      data2: [],
    };
  },
  computed: {
    showPageArr() {
      return this.$store.state.common.PageArr;
    },
  },
  created() {
    this.setnva();
    this.getDates()
    this.searchbtn();
  },
  mounted() {
   
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "入库单",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    getDates() {
        const myDate = new Date();
        const year = myDate.getFullYear(); // 获取当前年份
        const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
        const day = myDate.getDate(); // 获取当前日（1-31）
        this.saleDate=[`${year}-${month}-${day}`,`${year}-${month}-${day}`]
        this.Start=`${year}-${month}-${day}`
        this.End=`${year}-${month}-${day}`
    },
    changedata(value) {
      this.Start = value[0];
      this.End = value[1];
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
    searchbtnclick(){
      this.PageIndex=1;
      this.searchbtn()
    },
   searchbtn() {
      this.loading=true
      var data = {
        PartNumber:this.PartNumber,
        FormID:this.FormID,
        PageIndex: this.PageIndex,
        PageSize: this.PageSize,
        Start:this.Start,
        End:this.End,
        Status:JSON.parse(this.Status)
      };
      console.log(data);
      Show_Product_Inbound(data).then((res) => {
        this.loading=false
        this.total = res.Total;
        if(this.Status=='true'){
          this.data1 = res.data;
        }else{
          this.data2 = res.data;
        }
      });
    },
    cleanbtn() {
      this.PartNumber = null;
      this.FormID = null;
      this.PageIndex = 1;
      this.getDates()
      this.searchbtn()
    },
    changetabel(name){
      this.PageIndex = 1;
      this.searchbtn()
    }
  },
};
</script>