<style scoped>
.title_box {
  border-left: 5px solid #2d8cf0;
  text-indent: 15px;
  font-size: 15px;
  line-height: 32px;
}
.inputwidth{
    width: 80%;
}
.inputbox{
    margin-bottom: 15px;
}
.bitian{
    color: red;
    font-size: 20px;
    vertical-align: -webkit-baseline-middle;
    padding-right: 3px;
}
.pagebox{
  text-align: right;
  padding-top: 15px;
}
</style>
<template>
  <div>
    <h1 class="title_box">
      当前库房编号：HK001
      <Button type="primary" style="margin-left:15px" icon='md-add' @click="openmodel(1)">添加库区</Button>
    </h1>
    <div style="padding-top:15px">
      <Table :columns="columns1" :data="Regionarr" :loading='loading'>
          <template slot-scope="{ row, index }" slot="Indexs">
             <span>{{index+1}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="Code">
             <span>{{row.RegionIDs}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="Name">
             <span>{{row.Name}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="actions">
              <Button type="primary" style="margin-left:15px" size='small' icon='md-create' @click="openmodel(row)">修改</Button>
              <Button type="error" size='small' @click="confirmDel(row.RegionIDs)" icon='ios-trash-outline'>删除</Button>  
          </template>
      </Table>
      <div class="pagebox">
         <Page :total="total" :page-size='pageSize' :current='pageIndex' @on-change='changepage'/>
      </div>
    </div>
    <Modal
        v-model="showmodel"
        :title="titlemodel"
        @on-visible-change='changeshowmodel'>
        <div class="inputbox">
            <label for=""><span class="bitian">*</span>库区编号：</label>
            <!-- <Input v-model="Code" placeholder="请输入库房编号" class="inputwidth" /> -->
            <Select v-model="Code" filterable class="inputwidth" :disabled="IsEdit==true?true:false">
                <Option v-for="(i,index) in 26" :value="'0'+String.fromCharCode(65+index)" :key="index">{{'0'+String.fromCharCode(65+index) }}</Option>
            </Select>
        </div>
        <div class="inputbox">
            <label for=""><span class="bitian">&nbsp;</span>库区名称：</label>
            <Input v-model="Name" placeholder="请输入库房名称" class="inputwidth"/>
        </div>
        <div slot="footer">
              <Button @click="cancel_btn">取消</Button>
              <Button type="primary" @click="ok_btn">确定</Button>
        </div>
    </Modal>
  </div>
</template>
<script>
import {CggetShowRegions,CgSetRegion,CgDelete} from '../../api/CgApi'
export default {
  data() {
    return {
      columns1: [
        {
          title: "#",
          slot: "Indexs",
          width:100,

        },
        {
          title: "库区编号",
          slot: "Code"
        },
        {
          title: "名称",
          slot: "Name"
        },
        {
          title: "操作",
          slot: "actions",
          align: "center"
        }
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
      titlemodel:"",//弹出框title
      showmodel:false,
      Code:"",//编号
      Name:"",//名称
      Waybill:"",
      codearr:[],
      Regionarr:[],
      loading:true,
      total:0,
      pageIndex:1,
      pageSize:10,
      IsEdit:false,//是否是修改库位
    };
  },
  created() {
    this.Waybill=sessionStorage.getItem("UserWareHouse")
  },
  mounted() {
    this.setnva();
    this.CggetShowRegions()
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "库区管理",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    ok_btn(){
      if(this.Code==''){
         this.$Message.error('请选择库区编号');
      }else{
        console.log(this.IsEdit)
        this.showmodel=false;
        var data={
          whCode:this.Waybill,//库房编号，
          region:this.Code,//库区编号，
          name:this.Name,//库区名称
        }
        var that=this
        CgSetRegion(data).then(res=>{
          if(res.success==true){
            this.$Message.success('操作成功');
            this.showmodel=false;
            this.CggetShowRegions()
          }else if(res.code){
             this.$Message.error('已存在');
          }
        })
      }
        
    },
    cancel_btn(){
        this.showmodel=false;
    },
    openmodel(val){
        console.log(val)
        if(val==1){
            this.titlemodel='添加库区'
            this.Code=''//编号
            this.Name=''//名称
            this.IsEdit=false;
        }else{
              this.IsEdit=true;
              this.titlemodel='修改库区'
              this.Code=val.RegionCode//编号
              this.Name=val.Name//名称
        }
        this.showmodel=true;
    },
    CggetShowRegions(){
      var data={
        whid:this.Waybill,
        pageIndex:this.pageIndex,
        pageSize:this.pageSize,
      }
      CggetShowRegions(data).then(res=>{
          this.Regionarr=res.obj.Data;
          this.total=res.obj.Total;
          this.loading=false
      })
    },
    changepage(val){
      this.pageIndex=val;
      this.loading=true;
      this.CggetShowRegions()
    },
    changeshowmodel(val){
      if(val==false){
         this.whCode='';
         this.region='';
         this.IsEdit=false;
      }else{
         
      }
    },
    confirmDel(id){
      var id=id;
        this.$Modal.confirm({
            title: '删除',
            content: '<p>是否确认删除此库区?</p>',
            onOk: () => {
              var data={
                id:id
              }
                CgDelete(data).then(res=>{
                  if(res.success==true){
                     this.$Message.success('删除成功');
                     this.CggetShowRegions()
                  }else{
                     this.$Message.error('删除失败,请联系管理员');
                  }
                })
            },
            onCancel: () => {
                this.$Message.info('取消删除');
            }
        });
    },
  }
};
</script>