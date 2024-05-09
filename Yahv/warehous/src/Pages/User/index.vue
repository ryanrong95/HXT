<template>
  <div class="login">
    <div class="loginbox">
      <ul>
        <li class="logintype">账号登录</li>
      </ul>
      <div>
        <Form ref="formInline" :model="formInline" :rules="ruleInline">
          <FormItem prop="user">
            <Input type="text" v-model="formInline.user" placeholder="请输入用户名" size="large">
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
            <Button type="primary" @click="handleSubmit('formInline')" size="large">登录</Button>
            <!-- <Button type="primary" @click="token" size="large">登录2</Button> -->
          </FormItem>
        </Form>
      </div>
    </div>
  </div>
</template>
<script>
import {mapActions} from 'vuex'
import {UserLogin,UserMenu} from "../../api"  //引入api 的接口
import {Logon} from "@/js/browser.js"
let Base64 = require('js-base64').Base64
export default {
  data() {
    return {
      formInline: {
        user: "",
        password: ""
      },
      ruleInline: {
        user: [
          {
            required: true,
            message: "请输入用户名",
            trigger: "blur"
          }
        ],
        password: [
          {
            required: true,
            message: "请输入密码",
            trigger: "blur"
          },
          {
            type: "string",
            min: 6,
            message: "密码的长度不能小于6位",
            trigger: "blur"
          }
        ]
      },
      newrouters:[
        {
          component:"layout",//公共部分的载体
          path: "/",
          children:[
            {
              path: "/",  //首页
              name: "home",
              component: "Home",
              prentname:"Home"
            },
            {
              path: "/Outputrecords",  //出库单
              name: "Outputrecords",
              component:"Outputrecords",
              prentname:"Outgoing"
            },
            {
              path: "/Documentupload", // 深圳出库批量上传发货文件
              name: "Documentupload",
              component: "Documentupload",
              prentname:"Outgoing"
            },
            {
              path: "/Outgoing/:id",  //出库
              name: "Outgoing",
              component: "Outgoing",
              prentname:"Outgoing",
              children:[
                  {
                    path:"/Outgoing/:id/outdetail/:wayBillID",  //出库详情
                    name: "outdetail",
                    component: "outdetail",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/:id/outdetail/:wayBillID/BudgetIncome",  //出库收入与支出
                        name: "outdetailBudget",
                        component: "Income",
                        prentname:"Expenses",
                      },
                    ]
                  },
                 {
                    path: "/Outgoing/OutDeclareDetail/:detailID",  //出库报关详情
                    name: "",
                    component: "OutDeclareDetail",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/:id/outdetail/:wayBillID/BudgetIncome",  //出库收入与支出
                        name: "outdetailBudgetDeclare",
                        component: "Income",
                        prentname:"Expenses",
                      },
                    ]
                 },
                 {
                    path: "/Outgoing/TurnDeclareDetail/:detailID",  //出库转报关详情
                    name: "",
                    component: "TurnDeclareDetail",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/:id/outdetail/:wayBillID/BudgetIncome",  //出库转报关收入与支出
                        name: "TurnDeclareBudgetDeclare",
                        component: "Income",
                        prentname:"Expenses",
                      },
                    ]
                 },
                 {
                    path: "/Outgoing/Szoutgoing/:detailID",  //深圳详情
                    name: "",
                    component: "Szoutgoing",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/:id/outdetail/:wayBillID/BudgetIncome",  //深圳出库收入与支出
                        name: "SzoutgoingBudgetDeclare",
                        component: "Income",
                        prentname:"Expenses",
                      },
                    ]
                 },
                
              ]
            },
            {
              path: "/TurnDeclare/:id",  //转报关出库
              name: "TurnDeclare",
              component: "TurnDeclareList",
              prentname:"TurnDeclare",
              children:[
                 {
                    path: "/TurnDeclare/TurnDeclareDetail/:detailID",  //出库转报关详情
                    name: "",
                    component: "TurnDeclareDetail",
                    prentname:"TurnDeclare",
                    children:[
                      {
                        path:"/TurnDeclare/:id/TurnDeclareDetail/:wayBillID/BudgetIncome",  //出库转报关收入与支出
                        name: "TurnDeclareBudgetIncome",
                        component: "Income",
                        prentname:"Expenses",
                      },
                    ]
                   }
               ]
            },
            {
              path: "/storage",//库房总体设置
              name: "Storage",
              component: "Storage",
              prentname:"storage"
            },
            {
              path: "/waybill", //运单管理页
              name: "waybill",
              component: "waybill",
              prentname:"waybill"
            },
            {
              path: "/Allocation",  //配置页显示的区域
              component:"index",
              prentname:"allocation",
              children: [
                {
                  path: "/Allocation",  //总体配置页
                  name: "Allocation",
                  component: "Allocation",
                  prentname:"allocation"
                },
                {
                  path: "/Allocation/print", //打印配置Currenthouse
                  name: "print",
                  component: "print",
                  prentname:"allocation"
                },
                {
                  path: "/Allocation/Currenthouse", //当前库房配置
                  name: "Currenthouse",
                  component: "Currenthouse",
                  prentname:"allocation"
                },
              ]
            },
            {
              path: "/Stock",//在库管理
              component: "index",
              prentname:"Stock",
              children: [
                {
                  path: "/Stock/searchstock", //库存查询
                  name: "searchstock",
                  component: "searchstock",
                  prentname:"Stock"
                },
                {
                  path: "/Stock/StorageCharge", //仓储费管理
                  name: "StorageCharge",
                  component: "StorageCharge",
                  prentname:"Stock"
                },
                {
                  path: "/Stock/Accessrecords", //深圳出入库单
                  name: "Accessrecords",
                  component: "Accessrecords",
                  prentname:"Stock"
                },
              ]
            },
            {
                path: "/Lease", //库位分配
                name: "Lease",
                component: "Lease",
                prentname:"Stock",
                children:[
                  {
                    path: "/Lease/leasedetail", //库位分配列表
                    name: "leasedetail",
                    component: "LeaseDetail",
                    prentname:"Stock"
                  }
                ]
            },
             {
                path: "/ShelveManage", //库位分配列表
                name: "ShelveManage",
                component: "ShelveManage",
                prentname:"CgStorehouse"
              },
              {
                path: "/Pallet", //添加卡板
                name: "Pallet",
                component: "PalletAdd",
                prentname:"CgStorehouse"
              },
              {
                path: "/Region", //添加库区
                name: "Region",
                component: "RegionAdd",
                prentname:"CgStorehouse"
              },
            {
                path: "/Transport", //报关运输列表
                component: "index",
                prentname:"Transport",
                children:[
                  {
                    path: "/Transport/TransportList", //报关运输列表
                    name: "TransportList",
                    component: "TransportList",
                    prentname:"Transport",
                    children:[
                      {
                        path: "/Transport/TransportList/detail/:id", //报关运输详情
                        name: "TransportDetail",
                        component: "TransportDetail",
                        prentname:"Transport"
                      },
                    ],
                  },
                  
                  {
                      path: "/Transport/Customswindow/:id", //申报窗口
                      name: "TransportListCustomswindow",
                      component: "Customswindow",
                      prentname:"Transport",
                  },
                ]
            },

            {
                path: "/Szlist/:Type", //深圳入库
                name: "Szlist",
                component: "Szlist",
                prentname:"SzWarehousing",
                children:[
                  {
                    path: "/Szlist/SzDetail/:ID", //深圳入库详情
                    name: "SzDetail",
                    component: "SzDetail",
                    prentname:"SzWarehousing"
                  }
                ]
            },
            {
                path: "/Storagerecord", //香港入库单
                name: "Storagerecord",
                component: "Storage_record",
                prentname:"Cgenter",
            },
            {
              path: "/CgProcessed",  //入库重构(已处理)
              name: "CgProcessed",
              component: "Warehousing",
              prentname:"Cgenter",
              children:[
                {
                  path:"/CgProcessed/Budget",  //入库收支明细
                  name: "CgProcessed_meet",
                  component: "ends_meet",
                  prentname:"Common",
                },
                {
                  path:"/CgProcessed/Oplog",  //入库操作日志
                  name: "CgProcessed_Oplog",
                  component: "Oplog",
                  prentname:"Common",
                },
                {
                  path:"/CgProcessed/Income",  //入库收入,支出重构
                  name: "CgProcessed_Income",
                  component: "Income",
                  prentname:"Expenses",
                },
               {
                path: "/CgProcessed/Declare",  //报关页面
                name: "CgProcessedDeclare",
                component: "Declare",
                prentname:"Cgenter",
                children:[
                    {
                      path:"/CgProcessed/Declare/Budget",  //报关收支明细
                      name: "ends_meetDeclare",
                      component: "Income",
                      prentname:"Expenses",
                    },
                  ]
               },
              ]
            },
            {
              path: "/CgUntreated",  //入库重构（未处理）
              name: "CgUntreated",
              component: "Warehousing",
              prentname:"Cgenter",
              children:[
                {
                  path:"/CgUntreated/Budget",  //入库收支明细
                  name: "CgUntreated_meet",
                  component: "ends_meet",
                  prentname:"Common",
                },
                {
                  path:"/CgUntreated/Oplog",  //入库操作日志
                  name: "CgUntreated_Oplog",
                  component: "Oplog",
                  prentname:"Common",
                },
                {
                  path:"/CgUntreated/Income",  //入库收入,支出重构
                  name: "CgUntreated_Income",
                  component: "Income",
                  prentname:"Expenses",
                },
               {
                path: "/CgUntreated/Declare",  //报关页面
                name: "Declare",
                component: "Declare",
                prentname:"Common",
                children:[
                    {
                      path:"/CgUntreated/Declare/Budget/:webillID/:otype",  //报关收支明细
                      name: "ends_meetDeclare",
                      component: "ends_meet",
                      prentname:"Common",
                    },
                  ]
               },
              ]
            },
            {
                path: "/Separate",  //暂存管理
                component: "index",
                prentname:"Separate",
                children:[
                    {
                      path: "/Separate/:type", //待处理,已处理暂存列表
                      name: "Separate",
                      component: "NewSeparatelist",
                      prentname:"Separate",
                      children:[
                        {
                          path: "/Separate/:type/revise", //暂存录入
                          name:"Newseparate",
                          component: "Newseparate",
                          prentname:"Separate"
                        },
                      ]
                    },
                     {
                      path: "/Separate/list", //暂存列表
                      name: "Separatelist",
                      component: "Separatelist",
                      prentname:"Separate",
                      children:[
                        {
                          path: "/Separate/list/revise", //暂存录入
                          name:"revise",
                          component: "separate",
                          prentname:"Separate"
                        },
                      ]
                    },
                    {
                      path: "/Separate/separateenter", //暂存录入
                      name:"separateenter",
                      component: "separate",
                      prentname:"Separate"
                    },
                  ]
              },
              {
                path: "/IncomeList",  //香港收入费用管理
                component: "IncomeList",
                prentname:"Statisticalcost",
                children:[
                  {
                    path: "/IncomeList/IncomeListDetail",  //香港收入费用管理详情
                    name:"IncomeListDetail",
                    component: "IncomeListDetail",
                    prentname:"Statisticalcost",
                  }
                ]
              },
              {
                path: "/Expendlist",  //香港支出费用管理
                component: "Expendlist",
                prentname:"Statisticalcost",
                children:[
                  {
                    path: "/Expendlist/ExpendlistDetail",  //香港支入费用管理详情
                    component: "ExpendlistDetail",
                    prentname:"Statisticalcost",
                  }
                ]
              }
          ],
         
        }
      ],


      MenuItem:[],
    };
  },
  mounted() {
    console.log(navigator.userAgent)//获取浏览器的userAgent
  },
  methods: {
   
   ...mapActions({add_Routes: 'add_Routes'}),
  handleSubmit(name) {
      this.$refs[name].validate(valid => {
        if (valid) {
          var datas={
            username:this.formInline.user,
            password:Base64.encode(this.formInline.password)
          }
            UserLogin(this.formInline.user,this.formInline.password).then((res) => {
               if(res.code==200){
                 var val=document.cookie; //获取cookie
                 Logon(val);
                //  this.UserMenu()
                  sessionStorage.setItem('user', res.data.Role.ID)  //存储用户信息
                  sessionStorage.setItem('username', res.data.RealName)  //存储用户信息
                  sessionStorage.setItem('userID', res.data.ID)  //存储用户信息
                  sessionStorage.setItem('routes', JSON.stringify(this.newrouters))//存储该用户拥有的权限路由
                  this.add_Routes(this.newrouters) //触发vuex里的增加路由
                  sessionStorage.setItem('userReadata', JSON.stringify(res.data))  //存储用户信息(判断所拥有的库房权限)
               }else{
                 this.$Message.error("用户名或密码不正确");
               }
          }) 
        } else {
          // this.$Message.error("请输入用户名或密码");
        }
      });
    },
    UserMenu(){  //获取菜单列表
      UserMenu().then((res) => {
               if(res.code==200){
                 this.MenuItem=res.data;
                 console.log(this.MenuItem)
                //  sessionStorage.setItem('menuData', JSON.stringify(res.data))  //存储该用户拥有的权限菜单
                 sessionStorage.setItem('routes', JSON.stringify(this.newrouters))//存储该用户拥有的权限路由
                 this.add_Routes(this.newrouters) //触发vuex里的增加路由
               }
          })  
    },
  }
};
</script>
<style scoped>
.login {
  width: 100%;
  height: 100%;
  /* background: url("../../static/img/login.png") no-repeat center center; */
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
  margin-left: 70px;
  padding: 35px 25px;
  border-radius: 5px;
}
.logintype {
  color: #333333;
  font-weight: bold;
  font-size: 20px;
  text-indent: 18px;
  border-left: 4px solid #e7141a;
  line-height: 1;
  margin-bottom: 25px;
  position: relative;
}
.error {
  width: 100%;
  height: 30px;
}
</style>