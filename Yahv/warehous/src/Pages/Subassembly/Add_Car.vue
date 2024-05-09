<style scoped>
.setpadding {
  padding-top: 10px;
}
</style>
<template>
  <div>
    <p>
      <label>&nbsp;&nbsp;&nbsp;承运商：</label>
      <span>{{EnterpriseName}}</span>
    </p>
    <p class="setpadding">
      <label><em style="color:red;">*</em>&nbsp;车牌号：</label>
      <Input v-model="addCarName" placeholder="请输入车牌号" style="width:80%" />
    </p>
  </div>
</template>
<script>
import { TransportAdd } from "../../api/CgApi";
export default {
  name: "AddDriver",
  props: ["EnterpriseName"],
  data() {
    return {
      addCarName: null,
    };
  },
  mounted() {},
  methods: {
    sumbit_btn() {
       if(this.addCarName==''||this.addCarName==null){
         
         this.$Message.error("请输入车牌号");
          }else{
             this.$emit('setdisabled',true)
            var data={
                EnterpriseName: this.EnterpriseName,
                Type: 1,
                CarNumber1:this.addCarName,
                Creator:sessionStorage.getItem("userID")
            }
            TransportAdd(data).then(res=>{
               
              if(res.success==true&&res.code==200){
                this.$Message.success("添加成功");
                this.$emit('ok_addCar',this.addCarName)
                this.delitem()
              }else if(res.code==100){
                this.$Message.error(res.data);
                this.$emit('ok_addCar','false')
              }else if(res.code==300){
                this.$Message.error(res.data);
                this.$emit('ok_addCar','false')
              }else{
                this.$Message.error(res.data);
                this.$emit('ok_addCar','false')
              }
              this.$emit('setdisabled',false)
            })

          }
    },
    delitem() {
      this.addCarName = null;
      this.$emit('setdisabled',false)
    }
  }
};
</script>