<style scoped>
.setpadding {
  padding-top: 10px;
}
</style>
<template>
  <div>
    <p>
      <label>承运商：</label>
      <span>{{EnterpriseName}}</span>
    </p>
    <p class="setpadding">
      <label><em style="color:red;">*&nbsp;</em>司&nbsp;&nbsp;机：</label>
      <Input v-model="addDriverName" placeholder="请输入司机" style="width:80%" />
    </p>
    <p class="setpadding">
      <label><em>&nbsp;</em>手机号：</label>
      <Input v-model="Mobile" type="tel" placeholder="请输入手机号" style="width:80%" />
    </p>
  </div>
</template>
<script>
import { DriverAdd} from "../../api/CgApi";
export default {
  name: "AddDriver",
  props: ["EnterpriseName"],
  data() {
    return {
      addDriverName: null,
      Mobile:null,
    };
  },
  mounted() {

  },
  methods: {
    sumbit_btn(){
      if(this.addDriverName==''||this.addDriverName==null){
         this.$Message.error("请输入司机");
      }else{
         this.$emit('setdisabled',true)
        var data={
          EnterpriseName:this.EnterpriseName,
          Name: this.addDriverName,
          Mobile:this.Mobile,
          Status: 200,
          IsChcd:false,
          Creator:sessionStorage.getItem("userID")
        }
        DriverAdd(data).then(res=>{
          if(res.success==true&&res.code==200){
            this.$Message.success("添加成功");
            this.$emit('ok_addDriver',this.addDriverName) 
            this.delitem()
          }else if(res.code==100){
            this.$Message.error(res.data);
            this.$emit('ok_addDriver','false') 
          }else if(res.code==300){
            this.$Message.error(res.data);
           this.$emit('ok_addDriver','false') 
          }else{
            this.$Message.error(res.data);
           this.$emit('ok_addDriver','false') 
          }
          this.$emit('setdisabled',false)
        })
      }
    },
     delitem(){
       this.addDriverName='';
       this.Mobile=''
       this.$emit('setdisabled',false)
    },
  }
};
</script>