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
  /* display: inline-block; */
  float: left;
  margin-right: 50px;
}
</style>
<template >
  <div>
    <p class="detailtitle">提货信息</p>
    <div class="setboxtop infobox" v-if="infodata!=null">
      <div class="rowcols">
        <ul class="infoul">
            <li><span>订单号：</span><span>{{infodata.FormID}}</span></li>
            <!-- <li><span>联系人：</span><span>{{infodata.Contact}}</span></li> -->
            <li>
              <span>司&nbsp;&nbsp;&nbsp;机：</span>
              <div style="display: inline-block">
                <Select
                  v-model="infodata.TakerName"
                  filterable
                  @on-change="changeCarrier"
                  style="width:180px"
                >
                  <Option
                    v-for="item in Carrier"
                    :value="item.Name"
                    :label="item.Name"
                    :key="item.value"
                    :label-in-value="true"
                  >
                    <p>
                       <span>{{ item.Name }}</span>
                       <span>{{ item.Phone }}</span>
                       <span>{{ item.Licence }}</span>
                    </p>
                  </Option>
                </Select></span>
              </div>
            </li>
            <!-- <li>
              <span>订单备注：</span
              ><span>{{infodata.Summary}}
              </span>
            </li> -->
          </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
            <li><span>通知类型：</span><span>{{infodata.NoticeTypeDes}}</span></li>
            <!-- <li><span>联系电话：</span><span>{{infodata.Phone}}</span></li> -->
            <li>
              <span>车&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;牌：</span
              ><span>{{infodata.TakerLicense}}</span>
            </li>
          </ul>
      </div>
      <div class="rowcols">
         <ul class="infoul">
            <li>
              <span>客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：</span
              ><span>{{infodata.ClientName}}</span>
            </li>
            <li><span>提货时间：</span><span>{{infodata.TakingTime|showDate}}</span></li>
          </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
            <li><span>运输方式：</span><span>{{infodata.TransportModeDes}}</span></li>
            <li>
              <span>提货地址：</span
              ><span>{{infodata.Address}}</span>
            </li>
          </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
             <li><span>联系人：</span><span>{{infodata.Contact}}</span></li>
             <li>
              <span>异常备注：</span
              ><span
                ><Input
                  v-model="infodata.Exception"
                  placeholder="请输入异常备注"
                  style="width: 180px"
              />
              </span>
            </li>
        </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
             <li><span>联系电话：</span><span>{{infodata.Phone}}</span></li>
             <li>
              <Button
                type="success"
                size="small"
                icon="md-checkbox-outline"
                :disabled='isdisable==true?true:false'
                @click="submit_Pickupgoods"  >提交</Button>
           </li>
        </ul>
      </div>
    </div>
    <div>
        <!-- 图片列表  -->
        <PhotoList :childendata='childendata'></PhotoList>
         <!-- 图片列表  -->
    </div>
    <p class="detailtitle">提货文件</p>
    <div style="margin: 10px 5px">
      <Flielist :ID='infodata.ID'></Flielist>
    </div>
    <p class="detailtitle">
      <span style="margin-right:10px">费用明细</span> 
      <Button type="success" size="small" icon="md-add" @click="showNoticeCharges">客户费用录入</Button></p>
    <div style="margin: 10px 5px">
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
import{InPlanNoticesDetail,InPlanNoticesArrange} from '../../api/Enter'
import {NoticeCharges_list,TakersList} from "../../api/Out";

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
      value1: "",
      value: "",
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
      cityList: [
        {
          value: "New York",
          label: "New York",
        },
        {
          value: "London",
          label: "London",
        },
        {
          value: "Sydney",
          label: "Sydney",
        },
        {
          value: "Ottawa",
          label: "Ottawa",
        },
        {
          value: "Paris",
          label: "Paris",
        },
        {
          value: "Canberra",
          label: "Canberra",
        },
      ],
      model11: "",
      model12: [],

      Costcolumns: [
        {
          title: "科目",
          key: "name",
        },
        {
          title: "类型",
          key: "age",
        },
        {
          title: "数量",
          key: "age",
        },
        {
          title: "单价",
          key: "age",
        },
        {
          title: "单位",
          key: "age",
        },
        {
          title: "总额",
          key: "age",
        },
        {
          title: "记录时间",
          key: "age",
        },
        {
          title: "操作",
          slot: "action",
          width: 100,
          align: "center",
        },
      ],

      ID:null,
      infodata:null,
      Carrier:[],
      NoticeChargeslist: [], //费用列表
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
        showtype:2
      },
      isdisable:false
    };
  },
  created(){
      this.ID=this.$route.params.detailID
      console.log(this.ID)
  },
  mounted() {
      this.InPlanNoticesDetail(this.$route.params.detailID)
      this.NoticeCharges_list(this.$route.params.detailID)
      this.TakersList()
  },
  methods:{
    //提交提货数据
    submit_Pickupgoods(){
      var data={
        "NoticeID":this.infodata.ID,     //NoticeID
        "ConsignorID":this.infodata.ConsignorID, //交货人ID 
        "TakerName":this.infodata.TakerName,    //提货人姓名
        "TakerPhone":this.infodata.TakerPhone,   //提货人联系电话
        "TakerLicence":this.infodata.TakerLicense, //提货人车牌
        "Exception":this.infodata.Exception// 可以不填  //异常备注
      }
      InPlanNoticesArrange(data).then(res=>{
        console.log(res)
        if(res.success==true){
          this.$Message.success('提交成功');
           this.isdisable=true
        }else{
           this.$Message.error(res.data);
        }
      })
    },
    InPlanNoticesDetail(ID){
      InPlanNoticesDetail(ID).then(res=>{
        console.log(res)
        this.infodata=res.data
        this.sumbitChargesdata.FormID=this.infodata.FormID
        this.sumbitChargesdata.NoticeID=this.infodata.ID
        this.sumbitChargesdata.ClientID=this.infodata.ClientID
        this.sumbitChargesdata.ClientName=this.infodata.ClientName

        this.childendata.ID=this.infodata.ID
        if(!this.infodata.TakerName!=true&&!this.infodata.TakerLicense!=true){
            this.isdisable=true
        }else{
          this.isdisable=false
        }
      })
    },
    //司机列表
    TakersList(){
      TakersList().then(res=>{
        this.Carrier=res.data
      })
    },
    changeCarrier(value) {
      var obj = this.Carrier.filter((item) => item.Name == value)[0];
      this.infodata.TakerLicense = obj.Licence;
      this.infodata.TakerName=obj.Name;
      this.infodata.TakerPhone=obj.Phone
    },
    //费用列表
    NoticeCharges_list(id) {
      NoticeCharges_list(id).then((res) => {
        this.NoticeChargeslist = res;
        this.Chargeslistloading=false
      });
    },
    //加载子组件数据
    showNoticeCharges(){
      this.showCharges=true
      this.timer = new Date().getTime()
    },
    ok_NoticeCharges(){
      this.$refs.NoticeCharges.submitbtn()
    },
    cancel_NoticeCharges(){
      this.$refs.NoticeCharges.cancelbtn()
      this.showCharges=false
    },
    fatherMethod(value){
        this.Chargeslistloading=true
        this.showCharges=value
        this.NoticeCharges_list(this.$route.params.detailID)
    }

  }
};
</script>
