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
       <label><em class="setmust">*</em> &nbsp;司&nbsp;&nbsp;&nbsp;机：</label>
       <Input v-model="Driverinfo.Name" placeholder="请输入司机" style="width:80%" />
    </p>
    <p class="setpadding">
       <label><em class="setmust">*</em> 手机号：</label>
       <Input v-model="Driverinfo.Mobile" placeholder="请输入手机号" style="width:80%" />
    </p>
    <p class="setpadding">
       <label><em class="setmust">*</em> 车牌号：</label>
       <Input v-model="Carinfo.CarNumber1" placeholder="请输入车牌号" style="width:80%" />
    </p>
  </div>
</template>
<script>
import { AddAllEnter,GetCarrierType} from "../../api/CgApi";
export default {
  name: "AddsCarrier",
  
   props: ["Conveyingplace"],
  data() {
    return {
      name: '',
      simpleName: '',
      region: 'HKG',
      Type: 1,
      Typelist:[],
      Driverinfo:{ //司机信息
        EnterpriseName:null,
        Name:'', //姓名必填
        Status: 200,
        IsChcd:false,
        Creator:sessionStorage.getItem("userID"),
        Mobile:'',//手机号 必填
      },
      Carinfo:{
            EnterpriseName:null,
            Type: 1,
            CarNumber1:'',//车牌号必填
            Creator:sessionStorage.getItem("userID")
        }
    };
  },
  mounted() {
    this.GetCarrierType()
  },
  methods: {
    sumbit_btn() {
      if(this.name!=''&&this.simpleName!=''&&this.Type!=''&&this.region!=''&&this.Driverinfo.Name!=''&&this.Driverinfo.Mobile!=''&&this.Carinfo.CarNumber1!=''){
        this.$emit('setdisabled',true)
        this.Driverinfo.EnterpriseName=this.name
        this.Carinfo.EnterpriseName=this.name
        var data={
          Carrier:{
              Name: this.name,
              Code: this.simpleName,
              Type: this.Type, //承运商类型
              Place: this.region,
              Summary: "",
              Status: 200,
              Creator:sessionStorage.getItem("userID")
          },
          Driver:this.Driverinfo,
          Transport:this.Carinfo
        }
        AddAllEnter(data).then(res => {
          var resdata={
            Carrier:{
              ID:res.Carrier.ID,
              Name:this.name
            },
            Driverinfo:this.Driverinfo,
            Carinfo:this.Carinfo
          }
          if(res.Carrier.success==true&&res.Driver.success==true&&res.Transport.success==true){
               this.$Message.success("添加成功");
               this.$emit('fatherMethodCarrier',resdata)
               this.delitem()
          }else if(res.Carrier.ID==null){
             this.$Message.error("承运商添加失败");
             this.$emit('fatherMethodCarrier','false')
          }else if(res.Driver.ID==null){
             this.$Message.error("司机添加失败");
             this.$emit('fatherMethodCarrier','false')
          }else if(res.Transport.ID==null){
             this.$Message.error("车牌号添加失败");
             this.$emit('fatherMethodCarrier','false')
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
        this.region='HKG'
        this.Type=1
        this.Driverinfo.Name=''
        this.Driverinfo.Mobile=''
        this.Carinfo.CarNumber1=''
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