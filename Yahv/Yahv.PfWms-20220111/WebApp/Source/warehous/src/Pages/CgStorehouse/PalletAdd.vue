<style scoped>
.title_box {
     text-indent: 10px;
    border-left: 4px solid #2d8cf0;
    font-size: 15px;
    line-height: 32px;
    margin-right: 20px;
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
      <Button type="primary" style="margin-left:15px"  icon='md-add' @click="openmodel(1)">添加卡版</Button>
    </h1>
    <div style="padding-top:15px">
      <Table :columns="columns1" :data="palletarr" :loading='loading'>
          <template slot-scope="{ row, index }" slot="Indexs">
             <span>{{index+1}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="Code">
             <span>{{row.PalletIDs}}</span>
          </template>
          <template slot-scope="{ row, index }" slot="actions">
              <Button type="error" size='small' icon='ios-trash-outline' @click='confirmDel(row.PalletIDs)'>删除</Button>  
          </template>
      </Table>
      <div class="pagebox">
         <Page :total="total" :page-size='pageSize' :current='pageIndex' @on-change='changepage'/>
      </div>
    </div>
    <Modal
        v-model="showmodel"
        :title="titlemodel"
        :mask-closable="false"
        @on-visible-change='changeshowmodel'>
        <div class="inputbox">
            <label for=""><span class="bitian">*</span>卡板编号：</label>
            <Select v-model="Code" filterable class="inputwidth">
                <Option v-for="(i,index) in 99" :value="i" :key="index">{{i}}</Option>
            </Select>
        </div>
        <div slot="footer">
              <Button @click="cancel_btn">取消</Button>
              <Button type="primary" @click="ok_btn">确定</Button>
        </div>
    </Modal>
  </div>
</template>
<script>
import {ShowPallets,CgSetPallet,CgDelete} from '../../api/CgApi'
export default {
  data() {
    return {
      loading:true,
      total:0,
      pageIndex:1,
      pageSize:10,
      columns1: [
        {
          title: "#",
          slot: "Indexs",
        //   width:100,

        },
        {
          title: "卡板编号",
          slot: "Code"
        },
        {
          title: "操作",
          slot: "actions",
          align: "center"
        }
      ],
      palletarr:[],
      titlemodel:"",//弹出框title
      showmodel:false,
      Code:"",//编号
      Waybill:"",
    };
  },
  created() {
    this.Waybill=sessionStorage.getItem("UserWareHouse")
    this.ShowPallets()
  },
  mounted() {
    this.setnva();
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "卡板管理",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    ok_btn(){
       if(this.Code==''){
         this.$Message.error('请选择卡板编号');
       }else{
         var newcode=(Array(2).join('0') + this.Code).slice(-2)
         console.log(newcode)
          var data={
          whCode:this.Waybill,
          pallet:newcode
        }
        CgSetPallet(data).then(res=>{
          if(res.success==true){
            this.$Message.success('添加成功');
            this.showmodel=false;
            this.ShowPallets()
          }else if(res.code){
             this.$Message.error('已存在');
          }
        })
       }
       
    },
    cancel_btn(){
        this.showmodel=false;
    },
    changeshowmodel(val){
        if(val==false){
          this.Code=''
          this.showmodel=false;
        }else{
          this.showmodel=true;
        }
    },
    openmodel(val){
        console.log(val)
        if(val==1){
            this.titlemodel='添加卡板'
        }else{
            this.titlemodel='修改卡板'
        }
        this.showmodel=true;
    },
    ShowPallets(){
      var data={
        whid:this.Waybill,
        pageIndex:this.pageIndex,
        pageSize:this.pageSize,
      }
      ShowPallets(data).then(res=>{
        this.palletarr=res.obj.Data;
        this.total=res.obj.Total;
        this.loading=false
      })
    },
    changepage(val){
      this.pageIndex=val;
      this.loading=true;
      this.ShowPallets()
    },
    confirmDel(id){
      var id=id;
        this.$Modal.confirm({
            title: '删除',
            content: '<p>是否确认删除此卡板?</p>',
            onOk: () => {
              var data={
                id:id
              }
              console.log(data)
                CgDelete(data).then(res=>{
                  console.log(res)
                  if(res.success==true){
                     this.$Message.success('删除成功');
                     this.ShowPallets()
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