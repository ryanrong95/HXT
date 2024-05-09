<style scoped>
.setpadding {
  padding-top: 10px;
}
.setmust{
  color: red;
}
</style>
<template>
  <div>
    <p>
      <label><em class="setmust">*</em> 名&nbsp;&nbsp;称：</label>
      <Input v-model="name" placeholder="请输入名称" style="width:80%" />
    </p>
    <p class="setpadding">
      <label><em class="setmust">*</em> 简&nbsp;&nbsp;称：</label>
      <Input v-model="simpleName" placeholder="请输入简称" style="width:80%" />
    </p>
    <p class="setpadding">
       <label><em class="setmust"></em> &nbsp;联系人：</label>
       <Input v-model="ContactName" placeholder="请输入联系人" style="width:80%" />
    </p>
    <!-- <p class="setpadding">
       <label><em class="setmust"></em> 手机号：</label>
       <Input v-model="Driverinfo.Mobile" placeholder="请输入手机号" style="width:80%" />
    </p>
    <p class="setpadding">
       <label><em class="setmust"></em> 车牌号：</label>
       <Input v-model="Carinfo.CarNumber1" placeholder="请输入车牌号" style="width:80%" />
    </p> -->
  </div>
</template>
<script>
import { AddContact,GetCarrierType} from "../../api/CgApi";
export default {
  name: "AddsCarrier",
  
   props: ["Conveyingplace"],
  data() {
    return {
      name: '',
      simpleName: '',
      region: 'HKG',
      Type: 2,
      Typelist:[],
      ContactName:'',//联系人
    };
  },
  mounted() {
    // this.GetCarrierType()
  },
  methods: {
    sumbit_btn() {
      if(this.name!=''&&this.simpleName!=''&&this.Type!=''&&this.region!=''){
        this.$emit('setdisabled',true)
        var data={
              Name: this.name,
              Code: this.simpleName,
              Type: this.Type, //承运商类型
              Place: this.region,
              Summary:'',
              IsInternational:false,
              Creator:sessionStorage.getItem("userID"),
              ContactName:this.ContactName
        }
        AddContact(data).then(res => {
          var resdata={
            Carrier:{
              ID:res.Carrier.ID,
              Name:this.name
              }
            }
          if(res.Carrier.success==true){
               this.$Message.success("添加成功");
               this.$emit('ok_contacts',resdata)
               this.delitem()
          }else{
             this.$Message.error("添加失败，请联系管理人员");
             this.$emit('ok_contacts','false')
          }
           this.$emit('setdisabled',false)
        });
      }else{
        this.$Message.error("请输入必填项目");
      }
      
    },
    delitem(){
        this.name=''
        this.simpleName=''
        this.ContactName=''
        this.$emit('setdisabled',false)
    },
    // 获取类型
    GetCarrierType(){
      GetCarrierType().then(res=>{
        console.log(res)
        this.Typelist=res
      })
    }
    
  }
};
</script>