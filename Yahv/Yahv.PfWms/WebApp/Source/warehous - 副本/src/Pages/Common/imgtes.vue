<template>
    <div>
        <div class="uoload_box setupload">
            <Upload ref="upload" :show-upload-list="false" 
            :format="['jpg','jpeg','png']" :max-size="2048" 
            :before-upload="handleBeforeUpload" 
            :on-format-error="handleFormatError" 
            :on-exceeded-size="handleMaxSize"
            type="select" 
            action=""
            style="border:none">       
                <Button type="primary" icon="ios-cloud-upload">传照</Button>
            </Upload>
        </div>
    </div>
</template>
<script>
import {imgupload} from "../../api"
import Vue from 'vue'
import{PFWMS_API} from "@/main"
export default {
    props:["type"],
    data(){
        return {
            uploadList: [],
            rowuploadlist:{},
            url:PFWMS_API+"/api/FileUpLoad",
            filelist:{},
            typeimg:this.type
        }
    },
    mounted() {
    },
  methods: {
    // handleBeforeUpload(file) {
    //     // 创建一个 FileReader 对象
    //     let reader = new FileReader()
    //     // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
    //     // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
    //     // 读取文件作为 URL 可访问地址
    //     reader.readAsDataURL(file)
    //     const _this = this
    //     reader.onloadend = function (e) {
    //         //  console.log(_this.typeimg)
           
    //         file.url = reader.result
    //          if(_this.typeimg==1){
    //             file.url = reader.result
    //             _this.uploadList.push(file)
    //          }else{
    //             // console.log(file.name)
    //              file.url = reader.result
    //            var newdata={
    //               AdminID: "",
    //               ClientID: "",
    //               CreateDate:"",
    //               CustomName: file.name,
    //               FileBase64Code:file.url,
    //               ID: "",
    //               InputID: "",
    //               LocalFile: "",
    //               Status: 0,
    //               StatusDes: "",
    //               StorageID: "",
    //               Type: 0,
    //               TypeDes: "",
    //               Url:  "",
    //               WaybillID:  "",
    //             }
    //             this.rowuploadlist=newdata
    //          }
    //     } 
    //     console.log(this.rowuploadlist)
    //     // console.log(this.rowuploadlist)
    //     // var value=this.rowuploadlist
    //     // Vue.set(this.typeimg,"uploadimg",value)
    //     this.$emit("changitem",this.rowuploadlist)
    //     // console.log(this.typeimg)
    //     return false;
    // },
    handleBeforeUpload(file) {
        // 创建一个 FileReader 对象
        let reader = new FileReader()
        // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
        // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
        // 读取文件作为 URL 可访问地址
        reader.readAsDataURL(file)
        const _this = this
        reader.onloadend = function (e) {
            file.url = reader.result
            // console.log(file.url)
            var newimg={
                   AdminID: "",
                  ClientID: "",
                  CreateDate:"",
                  CustomName:file.name,
                  FileBase64Code:file.url,
                  ID: "",
                  InputID: "",
                  LocalFile: "",
                  Status: 0,
                  StatusDes: "",
                  StorageID: "",
                  Type: 0,
                  TypeDes: "",
                  Url:  "",
                  WaybillID:  "",
                }
        //  _this.rowuploadlist=newimg;
         _this.$emit("changitem",newimg)
        }
        // console.log(this.rowuploadlist)
        //  this.$emit("changitem",this.rowuploadlist)

        return false;
    },
    handleRemove(file) {
        this.uploadList.splice(this.uploadList.indexOf(file), 1)
    },
    handleFormatError(file) {
      this.$Notice.warning({
        title: '文件格式不正确',
        desc: '文件 ' + file.name + ' 格式不正确，请上传 jpg 或 png 格式的图片。'
      })
    },
    handleMaxSize(file) {
      this.$Notice.warning({
        title: '超出文件大小限制',
        desc: '文件 ' + file.name + ' 太大，不能超过 2M。'
      })
    },
  }
}
</script>
<style scoped>
.demo-upload-list {
    display: inline-block;
    width: 60px;
    height: 60px;
    text-align: center;
    line-height: 60px;
    border: 1px solid transparent;
    border-radius: 4px;
    overflow: hidden;
    background: #fff;
    position: relative;
    box-shadow: 0 1px 1px rgba(0, 0, 0, .2);
    margin-right: 4px;
}

.demo-upload-list img {
    width: 100%;
    height: 100%;
}

.demo-upload-list-cover {
    display: none;
    position: absolute;
    top: 0;
    bottom: 0;
    left: 0;
    right: 0;
    background: rgba(0, 0, 0, .6);
}

.demo-upload-list:hover .demo-upload-list-cover {
    display: block;
}

.demo-upload-list-cover i {
    color: #fff;
    font-size: 20px;
    cursor: pointer;
    margin: 0 2px;
}

.ivu-icon {
    line-height: 58px;
}
.setupload{
  /* width: 50px; */
  height: 30px;
  border: none;
  float: left;
  line-height: 1;
  margin-right: 3px;
}
.setupload .ivu-btn{
  padding:2px 2px 2px;
  font-size: 12px;
}
.setupload .ivu-upload .ivu-upload-drag{
  border:0px !important;
}
.upload_list{
    height: 30px;
    line-height: 30px;
}
</style>
