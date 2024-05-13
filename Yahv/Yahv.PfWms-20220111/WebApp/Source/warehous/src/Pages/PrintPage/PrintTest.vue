<template>
  <div class="boxprint">
    <button @click="test">测试后台返回值2</button>
    <!-- <button onclick="alert(testRtnValue2());">测试后台返回值2</button> -->
  </div>
</template>
<script>


  import { testRtnValue2 } from "@/js/browser.js"
  // import VueBarcode from '@xkeshi/vue-barcode'; //导入条形码插件
  

 
  export default {
    name: "PrintTest",
   components: {
  //  barcode: VueBarcode

    },   
  data() {
  return {
 
  }
  },
   //computed:{
   //    print_data(){
         
   //        return pobj;
         
   //    }
   //},
  methods: {
    test(){
      // testRtnValue2()
      alert("")
    }
  },
  created(){
  },
  mounted() {
    //this.print_data = Obj;
    // alert(this.print_data.name)

    }
    
  }

   
</script>
<style scoped>
  /* #app {
  width: 8cm;
  height: 8cm;
  overflow: hidden;
}*/
.boxprint{
   margin: 0;
   padding: 10px
} 
</style>
