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
          v-model.trim="PartNumber"
          placeholder="请输入型号"
          class="secrchinput"
        />
      </p>
      <p class="searchinputbox">
        <span>品牌：</span>
        <Input
          v-model.trim="Brand"
          placeholder="请输入品牌"
          class="secrchinput"
        />
      </p>
      <!-- <p class="searchinputbox">
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
      </p> -->
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
          Mpq:{{row.NoticeMpq}}&nbsp;/&nbsp;{{ row.NoticePackageNumber}}&nbsp;/&nbsp;{{row.NoticeTotal}}
        </template> -->
      </Table>
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
import { Show_Product_Outbound } from "../../api/Out";
export default {
  data() {
    return {
      tableHeight:500,
      loading:true,
      total: 0,
      PartNumber:null,
      Brand:null,
      PageIndex: 1,
      PageSize: 20,
      StartDate: null,
      EndDate: null,

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
        //   title: "Mpq / 已分拣(件) / 已分拣总量",
        //   slot: "Quantity",
        //   align: "center",
        // },
        {
          title: "Mpq",
          key: "NoticeMpq",
          align: "center",
        },
        {
          title: "已分拣(件)",
          key: "NoticePackageNumber",
          align: "center",
        },
        {
          title: "已分拣总量",
          key: "NoticeTotal",
          align: "center",
        }
        // {
        //   slot: "action",
        //   title: "操作",
        // },
      ],
      data1: [],
    };
  },
  computed: {
    showPageArr() {
      return this.$store.state.common.PageArr;
    },
  },
  created() {
    this.setnva();
    this.searchbtn();
  },
  mounted() {
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "出库单",
          href: "",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changedata(value) {
      this.StartDate = value[0];
      this.EndDate = value[1];
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
				this.PageIndex=1
				this.searchbtn()
			}, 
   searchbtn() {
      this.loading=true
      this.PageIndex=1
      var data = {
        PartNumber:this.PartNumber,
        Brand:this.Brand,
        PageIndex: this.PageIndex,
        PageSize: this.PageSize,
      };
      console.log(data);
      Show_Product_Outbound(data).then((res) => {
        this.loading=false
        console.log(res);
        this.data1 = res.data;
        this.total = res.Total;
      });
    },
    cleanbtn() {
      this.PartNumber = null;
      this.Brand = null;
      this.PageIndex = 1;
      this.searchbtn()
    },
  },
};
</script>