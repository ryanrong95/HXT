<template>
  <div>
    <!--开启摄像头-->
    <!-- <img @click="callCamera" :src="headImgSrc" alt="摄像头"> -->
    <div>
      <span style="font-weight: bold;font-size: 16px;">选择拍照设备：</span>
      <Select v-model="model1" style="width:200px" v-if="list!=[]" @on-change="chengeCamera">
        <Option v-for="item in list" :value="item.deviceId" :key="item.deviceId">{{ item.label }}</Option>
      </Select>
    </div>
    <div style="width:100%;height:450px">
      <video ref="video" width="450" height="350" autoplay style="float:left;"></video>
      <!--canvas截取流-->
      <span v-show="showcanvas==false"class="noimg" >暂无图片</span>
      <canvas v-show="showcanvas==true" ref="canvas" width="440" height="340" class="canvas"></canvas>
      <!--图片展示-->
    </div>
    <!--确认-->
    <div>
      <Button type="primary" @click="photograph">拍照</Button>
      <Button type="primary" @click="closeCamera">关闭摄像头</Button>
    </div>
  </div>
</template>   
<script>
import Vue from 'vue'
export default {
  name: "Photograph",
  data() {
    return {
      showcanvas: false,
      headImgSrc: "",
      list: [],
      model1: ""
    };
  },
  created() {
    // this.setCamera();
    // this.callCamera();
  },
  mounted() {},
  methods: {
    //初始化设置摄像头设备
    setCamera() {
      var that = this;
      if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
          console.log("不支持 enumerateDevices() .");
          return;
        }

        // 列出相机和麦克风.

        // navigator.mediaDevices.enumerateDevices() .then(function(devices) {
        // console.log(device)
        //   devices.forEach(function(device) {
        //     if(device.kind=='videoinput'){
        //       that.list.push(device);
        //     }
        //     // console.log(device.kind + ": " + device.label +
        //     //             " id = " + device.deviceId);
            
        //   });
        //    that.model1 = that.list[0].deviceId;
        //    console.log(that.model1)
        // })
        // .catch(function(err) {
        //   console.log(err.name + ": " + err.message);
        // });
      // console.log(this.list)
      // console.log(this.model1)

     
      navigator.mediaDevices.enumerateDevices().then(function(devices) {
          var listdevices=devices
          var data=[];
          alert(JSON.stringify(listdevices))
          if(listdevices.length>0){
            for (var i = 0; i <= listdevices.length; i++) {
             if (listdevices[i].kind == "videoinput") {
               that.list.push(listdevices[i]);
               that.model1 = that.list[0].deviceId;
             }
           }
          }
          //  that.model1 = that.list[0].deviceId;
        })
        .catch();

      // console.log(this.list)
      // console.log(that.model1)
    },
    // 调用摄像头
    callCamera() {
      var that = this;
      navigator.mediaDevices
        .getUserMedia({
          audio: false,
          video: {
            optional: [
              {
                sourceId: that.model1
              }
            ]
          }
        })
        .then(success => {
           var video = document.querySelector('video');
            // 旧的浏览器可能没有srcObject
            // if ("srcObject" in video) {
            //   video.srcObject = stream;
            // } else {
            //   // 防止在新的浏览器里使用它，应为它已经不再支持了
            //   video.src = window.URL.createObjectURL(stream);
            // }

          // 摄像头开启成功
          this.$refs["video"].srcObject = success;
          // 实时拍照效果
          this.$refs["video"].play();
        })
        .catch(error => {
          console.error("摄像头开启失败，请检查摄像头是否可用！");
        });
    },
    // 拍照
    photograph() {
      this.showcanvas = true;
      let ctx = this.$refs["canvas"].getContext("2d");
      // 把当前视频帧内容渲染到canvas上
      ctx.drawImage(this.$refs["video"], 0, 0, 450, 340);
      // 转base64格式、图片格式转换、图片质量压缩
      let imgBase64 = this.$refs["canvas"].toDataURL("image/webp");
      this.headImgSrc=imgBase64
     // console.log(imgBase64); // 由字节转换为KB 判断大小
      let str = imgBase64.replace("data:image/jpeg;base64,", "");
      let strLength = str.length;
      let fileLength = parseInt(strLength - (strLength / 8) * 2); // 图片尺寸  用于判断
      let size = (fileLength / 1024).toFixed(2);
      

      // 保存到本地
        // let ADOM = document.createElement('a')
        // ADOM.href = this.headImgSrc;
        // ADOM.download = new Date().getTime() + '.jpg'
        // ADOM.click()
    },
    uploadphoto(){
      //   console.log(size) // 上传拍照信息  调用接口上传图片 .........
      var newimg={ 
              AdminID: "",
              ClientID: "",
              CreateDate:"",
              CustomName: new Date().getTime() + '.jpg',
              FileBase64Code:this.headImgSrc,
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
      this.closeCamera()
      this.$emit("changupload",newimg)
      // this.closeCamera()
    },
    // 关闭摄像头
    closeCamera() {
      if (!this.$refs["video"].srcObject) return;
      let stream = this.$refs["video"].srcObject;
      let tracks = stream.getTracks();
      tracks.forEach(track => {
        console.log(track)
        track.stop();
      });
      this.$refs["video"].srcObject = null;
      // this.$refs["canvas"]='';
      this.showcanvas = false;
      // alert("是否合法开始看")
    },
    chengeCamera(value) {
      //不同摄像头的切换
      console.log(value);
      this.callCamera();
    }
  }
};
</script>
<style scoped>
.canvas {
  border: 1px solid #dddddd;
  float: right;
  /* font-weight: bold */
}
.noimg{
  /* display: inline-block; */
  width: 450px;
  height: 340px;
  border: 1px solid #dddddd;
  float: right;
  text-align: center;
  line-height: 340px;
  font-weight: bold;
}
</style>
