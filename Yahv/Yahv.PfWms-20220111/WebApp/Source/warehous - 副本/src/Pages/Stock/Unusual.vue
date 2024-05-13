<style scpoed>
</style>
<template>
  <div>
    <div>
      <Input v-model="value" placeholder="运单号" style="width: 200px" />
      <Input v-model="value" placeholder="承运商" style="width: 200px" />
      <Input v-model="value" placeholder="输送地" style="width: 200px" />
      <!-- <Input v-model="value" placeholder="暂存库位" style="width: 200px" /> -->
      <DatePicker type="date" placeholder="选择时间" style="width: 200px"></DatePicker>
    </div>
    <div style="padding-top:40px;">
      <Table  :columns="titles" :data="listdata">
        <template slot-scope="{ row,index }" slot="indexs">
          <strong>{{ index+1}}</strong>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="error" size="small" @click="remove(index)">删除</Button>
          <Button type="primary" size="small" style="margin-right: 5px" @click="show(index)">查看</Button>
        </template>
      </Table>
    </div>
    <Drawer title v-model="Spearatedrawer" width="70">
      <div>
        <UnusualDetail></UnusualDetail>
      </div>
    </Drawer>
  </div>
</template>
<script>
import UnusualDetail from "./UnusualDetail";
import {wareing} from "../../api"
export default {
  components: {
    UnusualDetail: UnusualDetail
  },
  data() {
    return {
        value:"",
      Spearatedrawer: false,
      columns12: [
        {
          title: "Name",
          slot: "name"
        },
        {
          title: "Age",
          key: "age"
        },
        {
          title: "Address",
          key: "address"
        },
        {
          title: "Action",
          slot: "action",
          width: 150,
          align: "center"
        }
      ],
      data6: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park"
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park"
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park"
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park"
        }
      ],
      titles:[
          {
          title: "#",
          slot: "indexs",
          align: "center",
          width:50,

        },
        {
          title: "型号",
          key: "PartNumber"
        },
        {
          title: "品牌",
          key: "Manufacturer"
        },
        {
          title: "批次",
          key: "DateCode"
        },
        {
          title: "数量",
          key: "Quantity"
        },
        {
          title: "原产地",
          key: "Place"
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center"
        }
      ],
      listdata:[
          {
              PartNumber:"AD6525654*89585",
              Manufacturer:"MMKSFERJEIRJ",
              DateCode:"1OIO",
              Quantity:"10000",
              Place:"中国"
          },
          {
              PartNumber:"AD6525654*89585",
              Manufacturer:"MMKSFERJEIRJ",
              DateCode:"1OIO",
              Quantity:"10000",
              Place:"中国"
          },
          {
              PartNumber:"AD6525654*89585",
              Manufacturer:"MMKSFERJEIRJ",
              DateCode:"1OIO",
              Quantity:"10000",
              Place:"中国"
          }
      ]
    };
  },
  mounted() {
    this.setnva();
  },
  methods: {
    setnva() {
        var cc = [
            {
            title: "在库管理",
            href: "/Stock"
            },
            {
            title: "暂存管理",
            href: "/Stock/Separate"
            }
        ];
        this.$store.dispatch("setnvadata", cc);
        },
    show(index) {
        this.Spearatedrawer = true;
        },
    remove(index) {

        },
    wareing(){
      wareing.then()
    }



    }
};
</script>