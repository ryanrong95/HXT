<template>
  <div>
    <Table :columns="Chargescolumns" :data="NoticeChargeslist" max-height='200' :loading="Chargeslistloading">
      <template slot-scope="{ row, index }" slot="Type">
        <span v-if="row.Type == 1">收入</span>
        <span v-else>支出</span>
      </template>
      <template slot-scope="{ row, index }" slot="CreateDate">
        {{ row.CreateDate | showDate }}
      </template>
      <template slot-scope="{ row, index }" slot="action">
        <Button type="error" size="small" icon="ios-trash-outline" @click="NoticeChargesDelete(row.ID)">删除</Button>
      </template>
    </Table>
  </div>
</template>
<script>
import{NoticeChargesDelete} from '../../api/index'
export default {
    props: ['NoticeChargeslist','Chargeslistloading'],
  data() {
    return {
      Chargescolumns: [
        {
          title: "科目",
          key: "Subject",
          align: "center",
        },
        {
          title: "类型",
          slot: "Type",
          align: "center",
        },
        {
          title: "数量",
          key: "Quantity",
          align: "center",
        },
        {
          title: "单价",
          key: "UnitPrice",
          align: "center",
        },
        {
          title: "单位",
          key: "Unit",
          align: "center",
        },
        {
          title: "总额",
          key: "Total",
          align: "center",
        },
        {
          title: "记录时间",
          slot: "CreateDate",
          align: "center",
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center",
        },
      ],
    };
  },
  methods:{
      NoticeChargesDelete(id){
          NoticeChargesDelete(id).then(res=>{
              if(res.Success==true){
                this.$Message["success"]({
                  background: "success",
                  content:'删除成功',
                });
                setTimeout(()=>{
                    this.$emit("fatherMethod", false);
                },300)
              }
          })
      }
  },
};
</script>