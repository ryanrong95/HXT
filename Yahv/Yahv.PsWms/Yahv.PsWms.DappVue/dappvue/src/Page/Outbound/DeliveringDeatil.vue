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
.rowcols {
  float: left;
  margin-right: 30px;
}
.ulbox {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
}
.liitem {
  /* width: 19%; */
  margin-right: 20px;
}
.addTaker {
  font-size: 18px;
}
.addTaker:hover {
  cursor: pointer;
}
.maststyle {
  color: red;
}
</style>
<template >
  <div v-if="infodata != null">
    <p class="detailtitle">送货信息</p>
    <div class="setboxtop infobox">
      <div class="rowcols">
        <ul class="infoul">
          <li>
            <span>订单号：</span><span>{{ infodata.FormID }}</span>
          </li>
          <li>
            <span>联系人：</span><span>{{ infodata.Consignee.Contact }}</span>
          </li>
          <li>
            <span>司&nbsp;&nbsp;&nbsp;机：</span>
            <div style="display: inline-block">
              <Select
                v-model="infodata.Consignee.TakerName"
                filterable
                :disabled="submitbtndisable == true ? true : false"
                @on-change="changeCarrier"
              >
                <Option
                  v-for="item in Carrier"
                  :value="item.Name"
                  :label="item.Name"
                  :key="item.value"
                  :label-in-value="true"
                >
                  <span>{{ item.Name }}</span>
                  <span>{{ item.Phone }}</span>
                  <span style=" color: #ccc">{{
                    item.Licence
                  }}</span>
                  <span style="float:right">
                    <Icon v-if="submitbtndisable==false" type="md-create"  @click="setshowaddTaker(2,item)"/>
                  </span>
                </Option>
              </Select>
            </div>
            <Icon v-if="submitbtndisable==false" type="md-add" class="addTaker" @click="setshowaddTaker(1)" />
          </li>
        </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
          <li>
            <span>通知类型：</span><span> {{ infoall.NoticeTypeDec }} </span>
          </li>
          <li>
            <span>联系电话：</span><span>{{ infodata.Consignee.Phone }}</span>
          </li>
          <li>
            <span>车&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;牌：</span
            ><span>{{ infodata.Consignee.TakerLicense }}</span>
          </li>
        </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
          <li>
            <span>客&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;户：</span
            ><span>{{ infodata.ClientName }}</span>
          </li>
          <li>
            <span>送货地址：</span>
            <span>{{ infodata.Consignee.Address }}</span>
          </li>
          <li>
            <span>异常备注：</span
            ><span
              ><Input
                :disabled="submitbtndisable == true ? true : false"
                v-model.trim="infodata.Consignee.Summary"
                placeholder="请输入备注"
                style="width: 180px"
            /></span>
            <Button
              type="success"
              size="small"
              icon="md-checkbox-outline"
              @click="submit_Delivery"
              :disabled="submitbtndisable == true ? true : false"
              >提交</Button
            >
          </li>
        </ul>
      </div>
      <div class="rowcols">
        <ul class="infoul">
          <li>
            <span>运输方式：</span><span>{{ infoall.TransportModeDec }}</span>
          </li>
          <li v-if="infodata.Consignee.TakingTime != null">
            <span>送货时间：</span
            ><span>{{ infodata.Consignee.TakingTime | showDateexact }}</span>
          </li>
        </ul>
      </div>
    </div>
    <div>
      <!-- 图片列表  -->
      <PhotoList :childendata="childendata"></PhotoList>
      <!-- 图片列表  -->
    </div>
    <p class="detailtitle">
      <span>文件列表</span>
      <Button
        type="success"
        size="small"
        icon="ios-print-outline"
        @click="printSHD"
        >打印送货单</Button
      >
    </p>
    <div style="margin: 10px 5px">
      <Flielist :ID="infodata.ID"></Flielist>
    </div>
    <p class="detailtitle">
      <span style="margin-right: 10px">费用明细</span>
      <Button
        type="success"
        size="small"
        icon="md-add"
        @click="showNoticeCharges"
        >客户费用录入</Button
      >
    </p>
    <div style="margin: 10px 5px">
      <NoticeChargeslist
        :NoticeChargeslist="NoticeChargeslist"
        :Chargeslistloading="Chargeslistloading"
        @fatherMethod="fatherMethod"
      ></NoticeChargeslist>
    </div>
    <!-- 费用录入  开始-->
    <Modal :width="53" v-model="showCharges" title="客户费用录入">
      <NoticeCharges
        :key="timer"
        ref="NoticeCharges"
        :sumbitChargesdata="sumbitChargesdata"
        @fatherMethod="fatherMethod"
      ></NoticeCharges>
      <div slot="footer">
        <Button @click="cancel_NoticeCharges">取消</Button>
        <Button @click="ok_NoticeCharges" type="primary">确定</Button>
      </div>
    </Modal>
    <!-- 费用录入  结束-->
    <!-- 新增司机 开始-->
    <Modal
      v-model="showaddTaker"
      :title="Typetitle==1?'新增司机信息':'修改司机信息'">
      <ul>
        <li>
          <span><em class="maststyle">*</em> 姓&nbsp;&nbsp;&nbsp;名：</span>
          <Input
            v-model.trim="Name"
            placeholder="请输入司机姓名"
            :disabled='Typetitle==2?true:false'
            style="width: 80%"
          />
        </li>
        <li style="padding: 10px 0">
          <span><em class="maststyle">*</em>手机号：</span>
          <Input
            v-model.trim="Phone"
            placeholder="请输入司机手机号"
            style="width: 80%"
          />
        </li>
        <li>
          <span><em class="maststyle">*</em>车牌号：</span>
          <Input
            v-model.trim="Licence"
            placeholder="请输入司机车牌号"
            style="width: 80%"
          />
        </li>
      </ul>
       <div slot="footer">
        <Button @click="visiblevisible(false)">取消</Button>
        <Button @click="ok_addTaker" v-if="Typetitle==1" type="primary">确定</Button>
        <Button @click="ok_ExitTaker" v-if="Typetitle==2" type="primary">确定</Button>
      </div>
    </Modal>
    <!-- 新增司机  结束-->
  </div>
</template>
<script>
import {
  NoticeDetail,
  NoticeCharges_list,
  submit_Delivery,
  TakersList,
  TakersEnter,
  TakersModify
} from "../../api/Out";
import { PrintDeliveryList } from "../../js/browser";
import NoticeCharges from "../Publicview/NoticeCharges";
import NoticeChargeslist from "../Publicview/NoticeChargeslist";
import Flielist from "../Publicview/Filelist";
import PhotoList from "../Publicview/PhotoList";
export default {
  components: {
    NoticeCharges,
    NoticeChargeslist,
    Flielist,
    PhotoList,
  },
  data() {
    return {
      submitbtndisable: false,
      Carrier: [], //司机列表
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
      infodata: null,
      infoall: null,
      NoticeChargeslist: [], //费用列表
      Chargeslistloading: true, //费用loading
      showCharges: false, //是否显示费用弹出框,
      sumbitChargesdata: {
        FormID: null,
        NoticeID: null, //是	string	通知ID
        ClientID: null,
        ClientName: null,
      },
      timer: "",
      childendata: {
        //图片组件参数
        ID: null,
        showtype: 8,
      },
      showaddTaker: false, //添加司机弹窗的状态
      Name: null, // 姓名
      Licence: null, //车牌
      Phone: null, // 电话号
      Typetitle:1
    };
  },
  created() {
    console.log(this.$route.matched[1].path);
    if (this.$route.matched[1].path == "/Delivering") {
      // this.submitbtndisable=false
      this.childendata.showtype = 8;
    } else {
      // this.submitbtndisable=true
      if (this.$route.matched[1].path == "/Delivered") {
        this.childendata.showtype = 9;
      } else {
        this.childendata.showtype = 9;
      }
    }
  },
  mounted() {
    this.TakersList();
    this.NoticeDetail(this.$route.params.detailID);
    this.NoticeCharges_list(this.$route.params.detailID);
  },
  methods: {
    //详情基础信息
    NoticeDetail(id) {
      NoticeDetail(id).then((res) => {
        console.log(res);
        this.infodata = res.Notice;
        this.infoall = res;
        this.sumbitChargesdata.FormID = this.infodata.FormID;
        this.sumbitChargesdata.NoticeID = this.infodata.ID;
        this.sumbitChargesdata.ClientID = this.infodata.ClientID;
        this.sumbitChargesdata.ClientName = this.infodata.ClientName;

        this.childendata.ID = this.infodata.ID;
        console.log(!this.infodata.Consignee.TakerLicense);
        if ( !this.infodata.Consignee.TakerLicense == false && !this.infodata.Consignee.TakerName == false ) {
          this.submitbtndisable = true;
        } else {
          this.submitbtndisable = false;
        }
      });
    },
    //费用列表
    NoticeCharges_list(id) {
      NoticeCharges_list(id).then((res) => {
        this.NoticeChargeslist = res;
        this.Chargeslistloading = false;
      });
    },
    //提交基础信息
    submit_Delivery() {
      var data = {
        ID: this.infodata.ConsigneeID,
        TakerName: this.infodata.Consignee.TakerName,
        TakingTime: this.infodata.Consignee.TakingTime,
        TakerPhone: this.infodata.Consignee.TakerPhone,
        Address: this.infodata.Consignee.Address,
        TakerLicense: this.infodata.Consignee.TakerLicense,
        Summary: this.infodata.Consignee.Summary,
      };
      console.log(data);
      submit_Delivery(data)
        .then((res) => {
          if (res.Success == true) {
            this.$Message["success"]({  background:true, content: "提交成功",  });
            this.submitbtndisable = true;
          } else {
            this.$Message["error"]({ background:true, content: res.Data, });
          }
        })
        .catch((error) => {
          console.log(error);
        });
    },
    //获取司机列表
    TakersList() {
      TakersList().then((res) => {
        console.log(res);
        this.Carrier = res.data;
      });
    },
    changeCarrier(value) {
      var obj = this.Carrier.filter((item) => item.Name == value)[0];
      this.infodata.Consignee.TakerLicense = obj.Licence;
      this.infodata.Consignee.TakerName = obj.Name;
      this.infodata.Consignee.TakerPhone = obj.Phone;
    },
    //加载子组件数据
    showNoticeCharges() {
      this.showCharges = true;
      this.timer = new Date().getTime();
    },
    ok_NoticeCharges() {
      this.$refs.NoticeCharges.submitbtn();
      console.log(this.$refs.NoticeCharges.istrue);
    },
    cancel_NoticeCharges() {
      this.$refs.NoticeCharges.cancelbtn();
      this.showCharges = false;
    },
    fatherMethod(value) {
      this.Chargeslistloading = true;
      this.showCharges = value;
      this.NoticeCharges_list(this.$route.params.detailID);
    },

    //打印部分
    printSHD() {
      var data = {
        waybillinfo: this.infoall,
        listdata: this.infoall.Items,
        Numcopies: 2,
      };
      console.log(data);
      PrintDeliveryList(data);
    },

    // 新增司机部分
    ok_addTaker() {
      if (!this.Name != true && !this.Licence != true && !this.Phone != true) {
        var data = {
          Name:this.Name, // 姓名
          Licence:this.Licence, //车牌
          Type: 1, // 1 司机, 2一般拿货人
          isTrustee: false, // 是否受托人
          Phone: this.Phone, // 电话号
        }
        TakersEnter(data).then(res=>{
          if (res.success == true) {
            this.$Message["success"]({  background:true,   content: "提交成功", });
            this.visiblevisible(false)
            this.TakersList()
          } else {
            this.$Message["error"]({background:true, content: res.data,duration: 5, closable: true  });
          }
        }).catch(error=>{
             this.$Message["error"]({ background:true, content:error.response.data.data,duration: 5, closable: true });
        })
      } else {
        this.$Message["warning"]({ background:true,content: '请输入必填项',duration: 5, closable: true });
      }
    },
    visiblevisible(value) {
      if (value == false) {
        this.showaddTaker=false
        this.Name=null// 姓名
        this.Licence=null //车牌
        this.Phone=null // 电话号
      }
    },
    setshowaddTaker(value,obj){
      this.Typetitle=value
      this.showaddTaker=true
      if(value==2){
         this.Name=obj.Name// 姓名
         this.Licence=obj.Licence //车牌
         this.Phone=obj.Phone // 电话号
      }
    },
    ok_ExitTaker(){
      if (!this.Name != true && !this.Licence != true && !this.Phone != true) {
        var data = {
          Name:this.Name, // 姓名
          Licence:this.Licence, //车牌
          Phone: this.Phone, // 电话号
        }
        TakersModify(data).then(res=>{
          if (res.success == true) {
            this.$Message["success"]({ background:true,  content: "修改成功", });
            this.infodata.Consignee.TakerLicense=this.Licence
            this.visiblevisible(false)
            this.TakersList()
          } else {
            this.$Message["error"]({ background:true,content: res.data, duration: 5, closable: true});
          }
        }).catch(error=>{
             this.$Message["error"]({  background:true, content:error.response.data.data,duration: 5, closable: true});
        })
      } else {
        this.$Message["warning"]({ background:true,  content: '请输入必填项',duration: 5, closable: true});
      }
    }
  },
};
</script>
