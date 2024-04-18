<style scoped>
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.setboxtop {
  margin: 5px 0px;
}
.infobox {
  width: 100%;
  max-height: 150px;
  min-height: 100px;
  padding: 0px 10px;
}
.infoul li {
  font-size: 15px;
  line-height: 30px;
}
.rowcols{
  float: left;
  margin-right: 40px;
}
</style>
<template >
  <div v-if="infodata!=null">
    <p class="detailtitle">提货信息</p>
    <div class="setboxtop infobox">
      <div class="rowcols">
         <ul class="infoul">
            <li><span>订单号：</span><span>{{infodata.FormID}}</span></li>
            <li><span>提货人：</span><span>{{infodata.Consignee.TakerName}}</span></li>
            <li>
              <span>异常备注：</span
              ><span
                ><Input
                 :disabled='submitbtndisable==true?true:false'
                  v-model="infodata.Consignee.Summary"
                  placeholder="请输入异常备注"
                  style="width: 200px"
              /></span>
            </li>
          </ul>
      </div>
      <div class="rowcols">
         <ul class="infoul">
            <li><span>通知类型：</span><span>{{ infoall.NoticeTypeDec }}</span></li>
            <li><span>联系电话：</span><span>{{infodata.Consignee.TakerPhone}}</span></li>
             <li>
              <Button type="success" size="small" icon="md-checkbox-outline" :disabled='submitbtndisable==true?true:false' @click="submit_PickUp">提交</Button>
            </li>
          </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
            <li>
              <span>客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：</span
              ><span>{{infodata.ClientName}}</span>
            </li>
            <li><span>提货时间：</span><span>{{infodata.Consignee.TakingTime|showDateexact}}</span></li>
          </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
            <li><span>运输方式：</span><span>{{ infoall.TransportModeDec }}</span></li>
            <li>
              <span>提货证件：</span
              ><span>{{infodata.Consignee.TakerIDCode}}({{infoall.IDTypeDec}})</span>
            </li>
        </ul>
      </div>
    </div>
    <div>
        <!-- 图片列表  -->
        <PhotoList :childendata='childendata'></PhotoList>
         <!-- 图片列表  -->
    </div>
    <p class="detailtitle">
      <span>文件列表</span>
       <Button type="success" size="small" icon="ios-print-outline" @click="printSHD">打印送货单</Button>
    </p>
    <div style="margin: 10px 5px">
       <Flielist :ID='infodata.ID'></Flielist>
    </div>
    <p class="detailtitle">
      <span style="margin-right:10px">费用明细</span> 
      <Button type="success" size="small" icon="md-add" @click="showNoticeCharges">客户费用录入</Button></p>
    <div style="margin: 10px 5px">
      <!-- <Table :columns="Chargescolumns" :data="data1">
        <template slot-scope="{ row, index }" slot="action">
          <Button type="error" size="small" icon="ios-trash-outline" >删除</Button>
        </template>
      </Table> -->
       <NoticeChargeslist :NoticeChargeslist='NoticeChargeslist' :Chargeslistloading='Chargeslistloading'  @fatherMethod="fatherMethod"></NoticeChargeslist>
    </div>
    <!-- 费用录入  开始-->
     <Modal
        :width='53'
        v-model="showCharges"
        title="客户费用录入">
        <NoticeCharges :key="timer" ref="NoticeCharges" :sumbitChargesdata='sumbitChargesdata' @fatherMethod="fatherMethod"></NoticeCharges>
        <div slot="footer">
            <Button @click="cancel_NoticeCharges">取消</Button>
            <Button @click="ok_NoticeCharges" type="primary">确定</Button>
        </div>
     </Modal>
     <!-- 费用录入  结束-->
  </div>
</template>
<script>
import{NoticeDetail,NoticeCharges_list,submit_PickUp} from "../../api/Out"
import{PrintDeliveryList} from '../../js/browser'

import NoticeCharges from "../Publicview/NoticeCharges";
import NoticeChargeslist from "../Publicview/NoticeChargeslist";
import Flielist from "../Publicview/Filelist"
import PhotoList from "../Publicview/PhotoList"
export default {
   components: {
     NoticeCharges,
     NoticeChargeslist,
     Flielist,
     PhotoList
  },
  data() {
    return {
      submitbtndisable:false,
      Filecolumns: [
        {
          title: "文件名称",
          key: "name",
        },
        {
          title: "文件类型",
          key: "age",
        },
        {
          title: "操作",
          slot: "action",
          width: 150,
          align: "center",
        },
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03",
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01",
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02",
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04",
        },
      ],
      model11: "",
      model12: [],


      Chargescolumns: [
        {
          title: "科目",
          key: "Subject",
        },
        {
          title: "类型",
          key: "Type",
        },
        {
          title: "数量",
          key: "Quantity",
        },
        {
          title: "单价",
          key: "UnitPrice",
        },
        {
          title: "单位",
          key: "Unit",
        },
        {
          title: "总额",
          key: "Total",
        },
        {
          title: "记录时间",
          key: "CreateDate",
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center",
        },
      ],

      infodata:null,
      infoall:null,
      NoticeChargeslist:[],

      Chargeslistloading:true, //费用loading
      showCharges:false,//是否显示费用弹出框,
      sumbitChargesdata: {
        FormID:null,
        NoticeID: null, //是	string	通知ID
        ClientID:null,
        ClientName:null,
      },  
      timer:'',
      childendata:{ //图片组件参数
        ID:null,
        showtype:9,
        Status:null
      },

    };
  },
    created(){
     this.NoticeDetail(this.$route.params.detailID)
  },
  beforeMount(){
    
  },
  mounted(){
    console.log(this.$route.matched[1].path)
    if(this.$route.matched[1].path=='/Extracted'){
      this.submitbtndisable=true
    }else{
      this.submitbtndisable=false
    }
    //  this.NoticeDetail(this.$route.params.detailID)
     this.NoticeCharges_list(this.$route.params.detailID)
  },
  methods: {
    //详情基础信息
    NoticeDetail(id){
      NoticeDetail(id).then(res=>{
        this.infodata=res.Notice
        this.infoall=res
         this.sumbitChargesdata.FormID=this.infodata.FormID
        this.sumbitChargesdata.NoticeID=this.infodata.ID
        this.sumbitChargesdata.ClientID=this.infodata.ClientID
        this.sumbitChargesdata.ClientName=this.infodata.ClientName

        this.childendata.ID=this.infodata.ID
        this.childendata.Status=this.infodata.Status
      })
    },
    //费用列表
    NoticeCharges_list(id){
      NoticeCharges_list(id).then(res=>{
        console.log(res)
        this.NoticeChargeslist=res
        this.Chargeslistloading=false
      })
    },
    submit_PickUp(){
      var data={
        ID:this.infodata.ConsigneeID,	                  //是	string	ConsigneeID
        TakerName: this.infodata.Consignee.TakerName,	  //是	string	提货人
        TakingTime: this.infodata.Consignee.TakingTime,	//是	string	提货时间
        TakerPhone: this.infodata.Consignee.TakerPhone,	//是	string	联系电话
        Address: this.infodata.Consignee.Address,	    //是	string	提货地址
        TakerIDType: this.infodata.Consignee.TakerIDType,	//是	string	证件类型
        TakerIDCode: this.infodata.Consignee.TakerIDCode,	//是	string	证件号码
        Summary: this.infodata.Consignee.Summary,    	//否	string	异常备注
      }
      console.log(data)
      submit_PickUp(data).then((res) => {
          if (res.Success == true) {
            this.$Message["success"]({
              background: "success",
              content: "提交成功",
            });
            this.submitbtndisable=true
          } else {
            this.$Message["error"]({
              background: "error",
              content: res.Data,
            });
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },

     //加载子组件数据
    showNoticeCharges(){
      this.showCharges=true
      this.timer = new Date().getTime()
    },
    ok_NoticeCharges(){
      this.$refs.NoticeCharges.submitbtn()
      console.log(this.$refs.NoticeCharges.istrue)
    },
    cancel_NoticeCharges(){
      this.$refs.NoticeCharges.cancelbtn()
      this.showCharges=false
    },
    
    fatherMethod(value){
        this.Chargeslistloading=true
        this.showCharges=value
        this.NoticeCharges_list(this.$route.params.detailID)
    },

    //打印部分
    printSHD(){
      var data = {
        waybillinfo: this.infoall,
        listdata:this.infoall.Items,
        Numcopies:2
      }
      console.log(data)
      PrintDeliveryList(data)
    },
  },
};
</script>
