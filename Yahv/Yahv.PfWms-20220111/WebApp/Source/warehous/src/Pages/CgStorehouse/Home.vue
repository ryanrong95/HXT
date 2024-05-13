
<style scoped>
.settitle {
  text-indent: 10px;
  border-left: 4px solid #2d8cf0;
  font-size: 15px;
  line-height: 32px;
  margin-right: 20px;
  float: left;
}
.tablebox{
    padding-top: 30px;
}
</style>
<template>
  <div>
    <!-- <h1 class="settitle">ku</h1> -->
    <div>
      <h1 class="settitle">当前库房编码：HK</h1>
      <div>
        <!-- <Button type="primary" @click="showmodel_btn('/CgStorehouse/Pallet','添加卡板库位')">添加卡板库位</Button>
        <Button type="primary" @click="showmodel_btn('/CgStorehouse/Region','添加库区')">添加库区</Button> -->
        <Button type="primary" @click="showmodel_btn('/CgStorehouse/Pallet','添加卡板库位')">添加卡板库位</Button>
        <Button type="primary" @click="showmodel_btn('/CgStorehouse/Region','添加库区')">添加库区</Button>
      </div>
    </div>
    <div class="tablebox">
      <Table stripe :columns="columns1" :data="data1">
           <template slot-scope="{ row,index }" slot="index">
                <strong>{{ index+1 }}</strong>
           </template>
           <template slot-scope="{ row,index }" slot="Type">
                <strong>{{ row.name }}</strong>
           </template>
           <template slot-scope="{ row,index }" slot="Code">
                <strong>{{ row.name }}</strong>
           </template>
           <template slot-scope="{ row,index }" slot="itemShelve">
                <strong>{{ row.name }}</strong>
           </template>
           <template slot-scope="{ row,index }" slot="action">
                <Button type="primary" size="small" style="margin-right: 5px" @click="setShelve=true">管理货架库位</Button>
                <Button type="error" size="small">删除货架</Button>
           </template>
      </Table>
    </div>
     <Drawer title="货架库位管理" :closable="false" v-model="setShelve" width='80' @on-visible-change='handleLoad' closable>
       <!-- <ShelveManage :key="timer" ></ShelveManage> -->
       <router-view></router-view>
     </Drawer>
     <Modal
        v-model="showModal"
        :title="showModaltitle"
        @on-ok="ok"
        @on-cancel="cancel"
        @on-visible-change='showModalchange'>
        <router-view></router-view>
    </Modal>
  </div>
</template>
<script>
import ShelveManage from './ShelveManage'
export default {
 components: {
   ShelveManage,
  },
  data() {
    return {
      columns1: [
        {
          title: "#",
          slot: "index",
          width:50,
        },
        {
          title: "库位类型",
          slot: "Type",
          align: 'center'
        },
        {
          title: "编号",
          slot: "Code",
          align: 'center'
        },
        {
          title: "库位个数",
          slot: "itemShelve",
          align: 'center'
        },
        {
          title: "操作",
          slot: "action",
          align: 'center'
        },
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03"
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01"
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02"
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04"
        }
      ],
      setShelve:false, // 左侧抽屉
      showModalShelve:false, //弹出框
      showModaltitle:"",
      timer: '',
      
    };
  },
  created() {
    this.setnva();
  },
  computed: {
      timer2(){
          return new Date().getTime()
      },
      showModal(){
        return this.$store.state.common.showModalShelve;
      }
  },
  mounted() {},
  methods: {
    setnva() {
      var cc = [
        {
          title: "库位管理",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    handleLoad (value) { //跳转至库位管理页面
        if(value==true){
            this.timer = new Date().getTime()
            // /CgStorehouse/ShelveManage
            this.$router.push({
                path: '/CgStorehouse/ShelveManage',
                query: {
                    ID: '11111'
                }
            });
        }else{
            this.$router.go(-1);
        }
     },
     showmodelitem(text){

     },
     showmodel_btn(url,text){
        this.$router.push({
                path: url,
            });
        
        this.$store.dispatch("setshowdetail", false);
        this.showModaltitle=text;
     },
     showModalchange(value){
         if(value==false){
             this.$router.go(-1);
         }
     }
  }
};
</script>