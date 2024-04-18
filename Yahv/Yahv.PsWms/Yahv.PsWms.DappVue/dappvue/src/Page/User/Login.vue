<template>
  <div class="login">
    <div class="loginbox">
      <div class="ivu-card-head">
        <p><i class="ivu-icon ivu-icon-log-in"></i> <span>欢迎登录</span></p>
      </div>
      <div class="inputbox">
        <Form ref="formInline" :model="formInline" :rules="ruleInline">
          <FormItem prop="user">
            <Input
              type="text"
              v-model="formInline.user"
              placeholder="请输入用户名"
              size="large"
            >
              <Icon type="ios-person-outline" slot="prepend"></Icon>
            </Input>
          </FormItem>
          <FormItem prop="password">
            <Input
              type="password"
              v-model="formInline.password"
              placeholder="请输入密码"
              size="large"
              @keyup.enter.native="handleSubmit('formInline')"
            >
              <Icon type="ios-lock-outline" slot="prepend"></Icon>
            </Input>
          </FormItem>
          <FormItem>
            <!-- <Button type="primary" @click="handleSubmit('formInline')" size="large">登录</Button> -->
            <!-- <Button type="primary" @click="token" size="large">登录2</Button> -->
            <Button type="primary" @click="handleSubmit('formInline')" long
              >登录</Button
            >
          </FormItem>
        </Form>
      </div>
    </div>
  </div>
</template>
<script>
import {UserLogin} from "../../api"  //引入api 的接口
import { mapActions } from "vuex";
let Base64 = require("js-base64").Base64;
export default {
  data() {
    return {
      formInline: {
        user: "",
        password: "",
      },
      ruleInline: {
        user: [
          {
            required: true,
            message: "请输入用户名",
            trigger: "blur",
          },
        ],
        password: [
          {
            required: true,
            message: "请输入密码",
            trigger: "blur",
          },
          {
            type: "string",
            min: 6,
            message: "密码的长度不能小于6位",
            trigger: "blur",
          },
        ],
      },
      newrouters: [
        {
          component: "layout", //公共部分的载体
          path: "/",
          children: [
            {
              path: "/Pickupgoods", //入库自提已安排
              name: "Pickupgoods",
              prentname: "Warehousing",
              component: "Pickupgoods",
              children:[
                {
                   path: "PickupDetail/:detailID", //入库已安排
                   name: "PickupgoodsDetail",
                   prentname: "Warehousing",
                   component: "PickupgoodsDetail",
                }
              ]
            },
            {
              path: "/Pickupgoodspending", //入库自提待安排
              name: "Pickupgoodspending",
              prentname: "Warehousing",
              component: "Pickupgoodspending",
              children:[
                {
                   path: "PickupDetail/:detailID", //入库待安排
                   name: "PickupDetail",
                   prentname: "Warehousing",
                   component: "PickupgoodsDetail",
                }
              ]
            },
            {
              path: "/WhingNoticePending", //入库通知待处理
              name: "WhingNoticePending",
              prentname: "Warehousing",
              component: "NoticePending",
              children:[
                {
                   path: "Detail/:detailID", //入库通知待处理 详情
                   name: "NoticeDetail",
                   prentname: "Warehousing",
                   component: "NoticeDetail",
                }
              ]
            },
            {
              path: "/WhingNoticeProcessed", //入库通知已处理
              name: "WhingNoticeProcessed",
              prentname: "Warehousing",
              component: "NoticeProcessed",
              children:[
                {
                   path: "Detail/:detailID", //入库通知待处理 详情
                   name: "ProcessedNoticeDetail",
                   prentname: "Warehousing",
                   component: "NoticeDetail",
                }
              ]
            },
            {
              path: "/WhingStorage_record", //入库单
              name: "WhingStorage_record",
              prentname: "Warehousing",
              component: "Storage_record",
            },
            {
              path: "/OutPending", //出库通知待处理
              name: "OutPending",
              prentname: "Outbound",
              component: "OutPending",
              children:[
                {
                   path: "OutDetail/:detailID", //出库通知待处理
                   name: "OutboundDetail",
                   prentname: "Outbound",
                   component: "OutboundDetail",
                }
              ]
            },
            {
              path: "/OutProcessed", //出库通知已处理
              name: "OutProcessed",
              prentname: "Outbound",
              component: "OutProcessed",
              children:[
                {
                   path: "OutProcessed/:detailID", //出库通知待处理
                   name: "OutProcessedDetail",
                   prentname: "Outbound",
                   component: "OutboundDetail",
                }
              ]
            },
             {
              path: "/Delivering", //出库送货待安排
              name: "Delivering",
              prentname: "Outbound",
              component: "Delivering",
              children:[
                {
                   path: "DeliveringDeatil/:detailID", //出库送货待安排详情
                   name: "DeliveringDeatil",
                   prentname: "Outbound",
                   component: "DeliveringDeatil",
                }
              ]
            },
            {
              path: "/Delivered", //出库送货已安排
              name: "Delivered",
              prentname: "Outbound",
              component: "Delivered",
              children:[
                {
                   path: "DeliveredDeatil/:detailID", //出库送货已安排详情
                   name: "DeliveredDeatil",
                   prentname: "Outbound",
                   component: "DeliveringDeatil",
                }
              ]
            },
            {
              path: "/DeliverFinish", //出库送货已完成
              name: "DeliverFinish",
              prentname: "Outbound",
              component: "DeliverFinish",
              children:[
                {
                   path: "DeliverFinishDeatil/:detailID", //出库送货已安排详情
                   name: "DeliverFinishDeatil",
                   prentname: "Outbound",
                   component: "DeliveringDeatil",
                }
              ]
            },
            {
              path: "/Extracting", //出库自提待安排
              name: "Extracting",
              prentname: "Outbound",
              component: "Extracting",
              children:[
                {
                   path: "ExtractDetail/:detailID", //出库自提待安排详情
                   name: "ExtractDetail",
                   prentname: "Outbound",
                   component: "ExtractDetail",
                }
              ]
            },
            {
              path: "/Extracted", //出库自提已安排
              name: "Extracted",
              prentname: "Outbound",
              component: "Extracted",
              children:[
                {
                   path: "ExtractedDetail/:detailID", //出库自提待安排详情
                   name: "ExtractedDetail",
                   prentname: "Outbound",
                   component: "ExtractDetail",
                }
              ]
            },
            {
              path: "/Outputrecords", //出库单
              name: "Outputrecords",
              prentname: "Outbound",
              component: "Outputrecords",
            },
            {
              path: "/AllocationPrint", //系统配置打印设置
              name: "AllocationPrint",
              prentname: "Allocation",
              component: "print",
            },
            {
              path: "/StoragesList", //在库 库存管理
              name: "StoragesList",
              prentname: "Storages",
              component: "StoragesList",
            },
            {
              path: "/ShelveList", //在库 库位管理列表
              name: "ShelveList",
              prentname: "Storages",
              component: "ShelveList",
              children:[
                {
                   path: "ShelveDetail/:detailID", //在库 库位管理详情
                   name: "ShelveDetail",
                   prentname: "Storages",
                   component: "ShelveDetail",
                }
              ]
            },
          ],
        },
      ],
    };
  },
  mounted() {},
  methods: {
    ...mapActions({ add_Routes: "add_Routes" }),
    handleSubmit(name) {
      this.$refs[name].validate((valid) => {
        if (valid) {
            UserLogin(this.formInline.user,this.formInline.password).then((res) => {
               if(res.code==200){
                 console.log(res)
                   sessionStorage.setItem('user', res.data.Role.ID)  //存储用户信息
                  sessionStorage.setItem('username', res.data.RealName)  //存储用户信息
                  sessionStorage.setItem('userID', res.data.ID)  //存储用户信息
                  sessionStorage.setItem("routes", JSON.stringify(this.newrouters)); //存储该用户拥有的权限路由
                  this.add_Routes(this.newrouters); //触发vuex里的增加路由
                  this.$router.push({
                  name: "WhingNoticePending",
                  });
                  sessionStorage.setItem("Activename", '待入库'); //存储菜单
                  sessionStorage.setItem("openname", JSON.stringify(['入库管理','入库通知'])); //存储菜单
               }else{
                 this.$Message.error("用户名或密码不正确");
               }
          })
          
        } else {
        }
      });
    },
  },
};
</script>
<style scoped>
.login {
  width: 100%;
  height: 100%;
  background: url("../../../static/img/login.png") no-repeat center center;

} 
.loginbox {
  min-width: 420px;
  min-height: 200px;
  background: #ffffff;
  position: absolute;
  left: 50%;
  top: 50%;
  margin-top: -90px;
  margin-left: 145px;
  /* padding: 0 25px 25px; */
  border-radius: 5px;
}
.ivu-card-head {
  border-bottom: 1px solid #e8eaec;
  padding: 14px 16px;
  line-height: 1;
}
.error {
  width: 100%;
  height: 30px;
}
.inputbox {
  padding: 16px;
}
</style>