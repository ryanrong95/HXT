<style scoped>
   .detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
} 
.delicon{
    font-size: 17px;
    color: red;
    position: relative;
    top: -65px;
    left: 39px;
}
.imgbox,.delicon:hover{
  cursor: pointer;
}
.imgbox{
  float: left;
  width: 60px;
  height: 50px;
}
.nulltitle{
    color:#ccb742
}
.nullbox{
    display: flex;
    flex-direction: column;
}
.imglistbox{
   display: flex;
   flex-direction: row;
   flex-wrap: nowrap;
  justify-content:  flex-start;
}
</style>
<template>
   <div>
        <p class="detailtitle"> <span>图片列表</span> 
        <span style="padding-left: 20px;" v-if="childendata.showtype==0">
          拍照/传照类型：
          <RadioGroup v-model="phototype" style="text-indent:0px;vertical-align: bottom;">
            <Radio :label="1">外观</Radio>
            <Radio :label="2">单据</Radio>
            <Radio v-if="childendata.Carrier=='SF'" :label="10">回单</Radio>
        </RadioGroup>
        </span>
         <span v-if="childendata.showtype==0">
            <Button type="primary" size="small" icon="md-reverse-camera" @click="FormPhotobtn(phototype)">拍照</Button>
            <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUploadbtn(phototype)">传照</Button>
         </span>
         <span v-if="childendata.showtype==2">
              <!-- 入库自提单据拍照 -->
             <Button type="primary" size="small" icon="md-reverse-camera" @click="FormPhotobtn(phototype)">拍照</Button>
             <Button type="primary" size="small" icon="ios-cloud-upload" @click="SeletUploadbtn(phototype)">传照</Button>
         </span>
         <span v-if="childendata.showtype==8">
              <!-- 出库送货单据拍照 -->
              <Button type="primary" size="small" icon="md-reverse-camera"  @click="FormPhotobtn(phototype)">司机签字拍照</Button>
              <!-- <Button type="primary" size="small" icon="md-reverse-camera" @click="FormPhotobtn(phototype)">客户签字拍照</Button> -->
         </span>
         <span v-if="childendata.showtype==9">
              <!-- 出库自提单据拍照 -->
              <Button type="primary" size="small" icon="md-reverse-camera" @click="FormPhotobtn(phototype)" :disabled='childendata.Status==300?false:true||childendata.Status==600?false:true'>客户签字拍照</Button>
         </span>
     </p>
     <div style="margin: 5px 5px; width: 100%;" class="imglistbox">
       <div style="margin-right:20px">
         <p style="padding-bottom:5px;font-weight: bold;">单据图片列表</p>
         <div v-if="danjulist.length<=0" class="nullbox">
            <img  src="../../assets/img/null.jpg" alt="" style="width:48px;height:48px">
            <span class="nulltitle">暂无单据图片</span>
          </div>
         <ul v-else>
            <li v-for="item in danjulist" style="float:left" class="imgbox">
              <img style="width:48px;height:48px" :src="item.Url" alt="" @click="yulan(item.Url)">
              <Icon type="md-close-circle" v-if="danjulist.length>1" class="delicon" @click="PhotoFileDelete(item.ID,2)"/>
            </li>
          </ul>
       </div>
       <div style="margin-right:20px" v-if="childendata.showtype==0">
          <p style="padding-bottom:5px;font-weight: bold;">外观图片列表</p>
          <div v-if="waiguanlist.length<=0" class="nullbox">
            <img  src="../../assets/img/null.jpg" alt="" style="width:48px;height:48px">
            <span class="nulltitle">暂无外观图片</span>
          </div>
           <ul v-else>
            <li v-for="item in waiguanlist" style="float:left" class="imgbox">
              <img style="width:48px;height:48px" :src="item.Url" alt="" @click="yulan(item.Url)">
              <Icon type="md-close-circle" class="delicon" @click="PhotoFileDelete(item.ID,1)"/>
            </li>
          </ul>
       </div>
        <div style="margin-right:20px" v-if="childendata.Carrier=='SF'">
         <p style="padding-bottom:5px;font-weight: bold;">回单图片列表</p>
         <div v-if="huidanlist.length<=0" class="nullbox">
            <img  src="../../assets/img/null.jpg" alt="" style="width:48px;height:48px">
            <span class="nulltitle">暂无回单图片</span>
          </div>
         <ul v-else>
            <li v-for="item in huidanlist" style="float:left" class="imgbox">
              <img style="width:48px;height:48px" :src="item.Url" alt="" @click="yulan(item.Url)">
              <Icon type="md-close-circle" v-if="huidanlist.length>1" class="delicon" @click="PhotoFileDelete(item.ID,10)"/>
            </li>
          </ul>
       </div>
     </div>
   </div>
</template>
<script>
import {GetPhotoFiles,PhotoFileDelete,GetPhototype} from "../../api/index";
import{FormPhoto,SeletUploadFiles,FilesProcess} from '../../js/browser'
import {Upload_CustomSignFile} from "../../api/Out";
export default {
    props: {
    childendata: {
      type: Object,
      default() {
        return {};
      },
    },
  },
    data(){
      return{
        danjulist:[],
        waiguanlist:[], 
        huidanlist:[],
        phototype:1,  //1外观 2单据 10回单
      }
    },
    created(){
         window["PhotoUploaded"] = this.changed;
    },
    mounted(){
        // if(this.childendata.showtype==2||this.childendata.showtype==3||this.childendata.showtype==4){
        //     this.phototype=2
        //     this.GetPhototype(2)
        // }else if(this.childendata.showtype==1){
        //     this.phototype=1
        //     this.GetPhototype(1)
        // }else{
        //     this.phototype=1
        //     this.GetPhototype(1)
        //     this.GetPhototype(2)    
        // }
        
        if(this.childendata.showtype==2){
            this.phototype=2
            this.GetPhototype(2)
        }else if(this.childendata.showtype==8){
            this.phototype=8
            this.GetPhototype(8)
        }else if(this.childendata.showtype==9){
            this.phototype=9
            this.GetPhototype(9)
        }else if(this.childendata.showtype==1){
            this.phototype=1
            this.GetPhototype(1)
        }
        else{
            this.phototype=1
            this.GetPhototype(1)
            this.GetPhototype(2)    
          if(this.childendata.Carrier=='SF'){
            this.GetPhototype(10)
          }
        }
       
      console.log(this.childendata)
        // this.setstatusfun()
    },
    methods:{
         // 拍照部分
    FormPhotobtn(type){
      if(type==1&&this.waiguanlist.length>=6){
        this.$Message.warning('该型号照片个数超过6个，每个型号允许拍摄的图片最多为6张');
      }else{
        var data={
          "SiteuserID":"",//网站上传人 先不传
          "AdminID":sessionStorage.getItem("userID"),//上传人
          "Data":{
          "MainID":this.childendata.ID,//主要ID
          "Type":type
          } 
         }
        FormPhoto(data)
      }
      
    },
    // 传照
    SeletUploadbtn(type){
      if(type==1&&this.waiguanlist.length>=6){
        this.$Message.warning('该型号照片个数超过6个，每个型号允许拍摄的图片最多为6张');
      }else{
        var data={
          "SiteuserID":"",//网站上传人 先不传
            "AdminID":sessionStorage.getItem("userID"),//上传人
            "Data":{
            "MainID":this.childendata.ID,//主要ID
            "Type":type
            } 
          }
        SeletUploadFiles(data)
      }
     
    },
    // 添加到数组中
    changed(msg){
      this.GetPhototype(this.phototype)
         this.setstatusfun()
    },
    setstatusfun(){
      if(this.danjulist.length==0&&this.childendata.showtype==9){
          Upload_CustomSignFile(this.childendata.ID).then(res=>{
        }).catch(error => {
            this.$Message.error(error.response.data.Data);
        })
      }else{
       
       }
    },
    // 删除
    PhotoFileDelete(ID,Type){
      if((this.danjulist.length<=1&&this.childendata.showtype==8)||(this.danjulist.length<=1&&this.childendata.showtype==9)){
         this.$Message.warning('如果已经上传过签字文件，则必须保留一个图片');
      }else{
        PhotoFileDelete(ID).then(res=>{
          if(res.success==true){
              this.$Message.success('删除成功');
              this.GetPhototype(Type)
          }
        })
      }
        
    },
    yulan(url){
       var data={
            Url:url
        } 
         FilesProcess(data)
      },
    GetPhototype(type){
        var data={
             "ID":this.childendata.ID, //通知ID ,NoticeID
             "Type":type // Type 文件类型： null返回单据，外观图片，1 外观文件, 2单据文件
        }
        GetPhototype(data).then(res=>{
          console.log(type)
            if(type==1){
                this.waiguanlist=res.data
            } else if(type==10){
                this.huidanlist=res.data
            }else{
                this.danjulist=res.data
            }
        })
    }
}

}
</script>