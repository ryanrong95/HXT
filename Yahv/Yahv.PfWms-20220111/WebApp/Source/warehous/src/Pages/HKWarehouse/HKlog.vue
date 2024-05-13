<style scoped>
.loglistbox {
  padding-top: 20px;
  padding-bottom: 46px;
}
.indexbox {
  display: inline-block;
  width: 22px;
  height: 22px;
  background: #c2c2c2;
  border-radius: 50%;
  line-height: 22px;
  text-align: center;
  margin-right: 10px;
  color: #ffffff;
}
.loglistbox li {
  line-height: 42px;
  font-size: 14px;
  border-bottom: 1px dashed #dcdee2;
}
.content {
  color: #333333;
}
.creatcdata {
  color: #999999;
  float: right;
  padding-right: 20px;
}
.titleactive {
  background: #2d8cf0;
}
.contentactive{
    color: #2d8cf0
}
.creatcdataactive{
    color: #333333;
}
.pagebox{
    text-align: center
}
.nulldataimg{
    text-align: center;
    padding: 20px 0px;
}
</style>
<template>
  <div>
    <div>
      <span>查询内容：</span>
      <Input v-model.trim="search.Key" placeholder="输入搜索内容关键字" search clearable style="width:300px" @on-clear='clearicon'  @on-search='search_btn' />
      <Button type="primary" @click="search_btn">搜索</Button>
    </div>
    <div v-if="loglist.length>0">
      <ul class="loglistbox">
        <li v-for="(item,index) in loglist"  @mouseenter="clickcategory(index)">
          <span :class="{titleactive:categoryIndex==index}" class="indexbox">{{index+1}}</span>
          <span :class="{contentactive:categoryIndex==index}" class="content">{{item.AdminName+" 在"+item.OperationTime+"装箱,"+"型号："+item.Model+",品牌:"+item.Brand+",产地:"+item.Origin+",数量:"+item.Qty+",箱号:"+item.BoxIndex}}</span>
          <span :class="{creatcdataactive:categoryIndex==index}" class="creatcdata">{{item.CreateDate|showDateexact}}</span>
        </li>
      </ul>
    </div>
    <div v-else class="nulldataimg">
        <img src="../../assets/img/null.png" alt="">
    </div>
    <div class="pagebox">
         <Page :total="total" show-elevator show-total :page-size='search.PageSize' :current='search.PageIndex' @on-change='changepage'/>
    </div>
  </div>
</template>
<script>
import {HKOperationLog} from '../../api/XdtApi'
export default {
  name: "HKLog",
  props: ["WaybillID"],
  data() {
    return {
      search:{
        OrderID:this.WaybillID,
        Key:"",
        PageSize:8,
        PageIndex:1
      },
      categoryIndex: 0,
      loglist: [],
      total:0,
    }
  },
  mounted() {
      this.getlog()
      console.log(this.search);
  },
 methods: {
     getlog() {
      HKOperationLog(this.search).then(res=>{
          this.loglist=res.obj.rows;
          this.total=res.obj.total

      })
    },
    clickcategory(index) {
     this.categoryIndex = index;
    },
    search_btn(){
        if(this.search.Key!=''){
            this.search.PageIndex=1
            this.getlog()
        }else{
           this.$Message.warning('请输入查询内容');
        }
    },
    clearicon(){
      this.search.Key=''
       this.search.PageIndex=1
       this.getlog()
    },
    changepage(index){
        this.search.PageIndex=index
        this.getlog()
    }
  }
};
</script>
