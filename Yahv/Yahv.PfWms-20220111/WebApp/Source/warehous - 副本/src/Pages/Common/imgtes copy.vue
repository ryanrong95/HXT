<template>
    <div>
        <div class="uoload_box" style="display:block;width:80px;height:31px;line-height:1;">
            <Upload ref="upload" :show-upload-list="false" 
            :format="['jpg','jpeg','png']" :max-size="2048" 
            :before-upload="handleBeforeUpload" 
            :on-format-error="handleFormatError" 
            :on-exceeded-size="handleMaxSize"
            type="drag" 
            action=""
            style="border:none">       
                <Button type="primary" icon="ios-search">传照</Button>
            </Upload>
        </div>
        <!-- <ul>
             <li  class="upload_list" v-for="item in uploadList">
                 <span>{{item.name}}</span>
                 <span>
                <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
            </span>
             </li>
         </ul> -->
         <!-- <Button type="text" @click="upload" >点击上传</Button> -->
         
         <!-- <div  v-for="item in uploadList">
            <span>{{item.name}}</span>
            <span>
                <Icon type="ios-trash-outline" @click.native="handleRemove(item)"></Icon>
            </span>
        </div> -->
    </div>
</template>
<script>
import {imgupload} from "../../api"
import{PFWMS_API} from "@/main"
export default {
    props:["type"],
    data(){
        return {
            uploadList: [],
            rowuploadlist:[],
            url:PFWMS_API+"/api/FileUpLoad",
            filelist:{},
            typeimg:this.type
        }
    },
    mounted() {
    },
  methods: {
    handleBeforeUpload(file) {
        // 创建一个 FileReader 对象
        let reader = new FileReader()
        // readAsDataURL 方法用于读取指定 Blob 或 File 的内容
        // 当读操作完成，readyState 变为 DONE，loadend 被触发，此时 result 属性包含数据：URL（以 base64 编码的字符串表示文件的数据）
        // 读取文件作为 URL 可访问地址
        reader.readAsDataURL(file)
        const _this = this
        reader.onloadend = function (e) {
            //  console.log(_this.typeimg)
             if(_this.typeimg==1){
                 file.url = reader.result
                _this.uploadList.push(file)
             }else{
                file.url = reader.result
                _this.rowuploadlist.push(file)
             }
        }
        // console.log(this.uploadList)     
        var key="uploadimg" 
        var value=this.rowuploadlist
        this.typeimg[key]=value
        console.log(this.typeimg)  
        return false;
    },
    upload () {
        // console.log(this.filelist)
        console.log(this.uploadList)
        //   imgupload(this.uploadList).then((res) => {
        //      console.log(res)
        //     })
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
.uoload_box .ivu-upload-drag {
    border: 0;
}
.upload_list{
    height: 30px;
    line-height: 30px;
}
</style>