<style scoped>
  .setmast{
    color: red;;
  }
</style>
<template>
    <div>
        <div>
          <p style="padding-bottom:10px">
            <label><em class="setmast">*</em>快递公司：</label>
              <Select v-model="expressdata.CarrierID" style="width:80%" @on-change="changeCarrier">
                  <Option v-for="item in CarrierArr" :value="item.Value" :key="item.Value">{{ item.Name }}</Option>
              </Select>
          </p>
          <p style="padding-bottom:10px">
            <label><em class="setmast">*</em>快递方式：</label>
             <Select v-model="expressdata.Expresstype" style="width:80%">
                  <Option v-for="item in ExpresstypeArr"  :value="item.Value" :key="item.Value">{{ item.Name }}</Option>
              </Select>
          </p>
          <p style="padding-bottom:10px" v-if="expressdata.PayTypes==4">
            <label><em class="setmast">*</em>第三方月结卡号:</label>
            <Input style="width:73%" v-model="expressdata.ThirdPartyCardNo" />              
          </p>
          <p>
            <label><em class="setmast">*</em>计费方式：</label> 
            <RadioGroup v-model="expressdata.PayTypes" v-if="expressdata.CarrierID!='EMS'">
              <Radio v-for="item in PayTypesArr" :label="item.Value" :key="item.Value">{{item.Name}}</Radio>
            </RadioGroup>
            <RadioGroup v-model="expressdata.PayTypes" v-if="expressdata.CarrierID=='EMS'">
              <Radio v-for="item in PayTypesArrForEMS" :label="item.Value" :key="item.Value">{{item.Name}}</Radio>
            </RadioGroup>
          </p>
        </div>
    </div>
</template>
<script>
  import {GetExpTypes,GetShipperCodes,GetPayTypes,UpdateWaybillExpress,GetPayTypesForEMS} from '../../api/CgApi'
  export default {
  // props: ['WaybillID','uploadwaybill'],
  props: {
  uploadwaybill: {
  type: Function,
  default: null
  },
  WaybillID:{
  type:String
  },
  ShipperCode:{
  type:String
  },
  Expresstype:{
  type:Number
  },
  PayType:{
  type:Number
  },
  ThirdPartyCardNo:{
  type:String
  }
  },
  name:'Express',
  data(){
  return {
  expressdata:{
  CarrierID:null,//承运商
  Expresstype:null,//快递方式
  PayTypes:null,
  ThirdPartyCardNo:null,
  },
  CarrierArr:[],
  ExpresstypeArr:[],
  PayTypesArr:[],
  PayTypesArrForEMS:[]
  }
  },
  mounted(){
  this.GetShipperCodes()
  this.GetPayTypes()
  this.GetPayTypesForEMS()
  GetExpTypes(this.ShipperCode)
  this.expressdata.CarrierID = this.ShipperCode
  this.expressdata.Expresstype = this.Expresstype
  this.expressdata.PayTypes = this.PayType
  this.expressdata.ThirdPartyCardNo = this.ThirdPartyCardNo
  console.log('this.PayType='+this.PayType)
  console.log('this.ShipperCode='+this.ShipperCode)
  console.log('this.Expresstype='+this.Expresstype)
  console.log('this.ThirdPartyCardNo='+this.ThirdPartyCardNo)
  },
  methods:{
  // 获取快递公司编码
  GetExpTypes(type){
  GetExpTypes(type).then(res => {
  console.log(res)
  this.ExpresstypeArr=res
  })
  },
  // 获取快递类型
  GetShipperCodes(){
  GetShipperCodes().then(res => {
  console.log(res)
  console.log("current this.ShipperCode="+this.ShipperCode)
  this.CarrierArr=res
  if(this.ShipperCode=='SF')
  {
  this.GetExpTypes(res[0].Value)
  }else if(this.ShipperCode=='KYSY'){
  this.GetExpTypes(res[1].Value)
  }else {
  this.GetExpTypes(res[2].Value)
  }

  })
  },
  // 获取计费方式
  GetPayTypes(){
  GetPayTypes().then(res => {
  console.log(res)
  this.PayTypesArr=res
  })
  },
  GetPayTypesForEMS(){
  GetPayTypesForEMS().then(res => {
  console.log(res)
  this.PayTypesArrForEMS=res  
  })
  },
  changeCarrier(val){
  this.GetExpTypes(val)
  this.expressdata.Expresstype = null
  this.expressdata.PayTypes = null
  this.expressdata.ThirdPartyCardNo=null
  },  
  UpdateWaybillExpress(){
  var data={
  "WaybillID":this.WaybillID,
  "ShipperCode":this.expressdata.CarrierID, // 快递公司编码SF 或者 KYSY
  "ExType": this.expressdata.Expresstype, // 快递方式
  "ExPayType":this.expressdata.PayTypes, // 快递计费方式
  "ThirdPartyCardNo":this.expressdata.ThirdPartyCardNo // 第三方月结卡号
  }


  UpdateWaybillExpress(data).then(res=>{
  if(res.success==true){
  this.uploadwaybill()
  this.cleanexpressdata()
  this.$Message.success('快递修改成功')
  }else{
  this.$Message.error(res.data)
  }
  })
  },
  cleanexpressdata(){
  this.expressdata.CarrierID=null,//承运商
  this.expressdata.Expresstype=null,//快递方式
  this.expressdata.PayTypes=null, //计费方式
  this.expressdata.ThirdPartyCardNo=null //第三方月结卡号
  }

  }
  }
</script>
