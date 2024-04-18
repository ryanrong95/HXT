<template>
  <div>
    <Table :columns="Filecolumns" :data="data1" max-height='260px'>
      <template slot-scope="{ row, index }" slot="action">
        <Button type="primary" size="small" @click="yulan(row.Url)">
          <Icon custom="iconfont icon-yulan" />
          预览</Button
        >
        <Button type="primary" size="small" @click="dayin(row.Url)" icon="ios-print-outline"
          >打印</Button
        >
      </template>
    </Table>
  </div>
</template>
<script>
import {GetNoticeFiles} from "../../api/index"
import { GetPrinterDictionary,FilesProcess, FilePrint} from "@/js/browser.js";
export default {
  props: ['ID'],  //注意这里要父组件的名字一致
  data() {
    return {
      Filecolumns: [ 
        {
          title: "文件名称",
          key: "CutomName",
        },
        {
          title: "文件类型",
          key: "TypeDes",
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center",
        },
      ],
      data1: [],
    };
  },
  created(){
      console.log(this.ID)
      this.getlist(this.ID)
  },
  mounted(){

  },
  methods:{
      getlist(ID){
          GetNoticeFiles(ID).then(res=>{
              console.log(res)
              this.data1=res.data
          })
      },
      yulan(url){
         var data={
            Url:url
          }
         FilesProcess(data) 
      },
      dayin(url){
        var configs = GetPrinterDictionary()
        var getsetting = configs['文档打印']
        getsetting.Url = url
        var data = getsetting
        FilePrint(data)
      }
  },
};
</script>