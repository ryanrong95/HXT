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
              path: "/Warehousing",  //入库
              name: "Warehousing",
              component: "Warehousing",
              prentname:"Warehousing",
              children:[
                {
                  path:"/Warehousing/Budget/:webillID/:otype",  //入库收支明细
                  name: "ends_meet",
                  component: "ends_meet",
                  prentname:"Common",
                },
                {
                  path:"/Warehousing/separate",  //到货暂存
                  name: "",
                  component: "separate",
                  prentname:"Common",
                },
                {
                path: "/Warehousing/Customswindow",  //入库申报窗口
                name: "",
                component: "Customswindow",
                prentname:"Common"
               },
               {
                path: "/Warehousing/Declare",  //报关页面
                name: "Declare",
                component: "Declare",
                prentname:"Common",
                children:[
                    {
                      path:"/Warehousing/Declare/Budget/:webillID/:otype",  //报关收支明细
                      name: "ends_meetDeclare",
                      component: "ends_meet",
                      prentname:"Common",
                    },
                  ]
               },
              ]
            },
            {
              path: "/Outgoing",  //出库
              name: "Outgoing",
              component: "Outgoing",
              prentname:"Outgoing",
              children:[
                  {
                    path:"/Outgoing/outdetail/:detailID",  //出库详情
                    name: "",
                    component: "outdetail",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/outdetail/Budget/:webillID/:otype",  //出库收支明细
                        name: "ends_meetout",
                        component: "ends_meet",
                        prentname:"Common",
                      },
                    ]
                  },
                  {
                    path: "/Outgoing/Customswindow",  //出库申报窗口
                    name: "",
                    component: "Customswindow",
                    prentname:"Common"
                 },
                 {
                    path: "/Outgoing/OutDeclareDetail/:detailID",  //出库报关详情
                    name: "",
                    component: "OutDeclareDetail",
                    prentname:"Outgoing",
                    children:[
                      {
                        path:"/Outgoing/OutDeclareDetail/Budget/:webillID/:otype",  //出库收支明细
                        name: "ends_meetDeclareout",
                        component: "ends_meet",
                        prentname:"Common",
                      },
                    ]
                 },
                
              ]
            },
            {
              path: "/storage",//库房总体设置
              name: "Storage",
              component: "Storage",
              prentname:"storage"
            },
            {
              path: "/kufang",//库房
              name: "Home",
              component: "Home",
              prentname:"storage"
            },
            {
              path: "/Logichouse/:fatherID",//逻辑库房
              name: "Logichouse",
              component: "Logichouse",
              prentname:"storage"
            },
            {
              path: "/Areapage/:fatherID",//库区
              name: "Areapage",
              component: "Areapage",
              prentname:"storage"
            },
            {
              path: "/shelves/:AreafatherID",  //货架
              name: "shelves",
              component: "Shelves",
              prentname:"storage"
            },
            {
              path: "/Location/:locationID", //库位
              name: "location",
              component: "location",
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
                  path: "/Stock", //在哭管理首页
                  name: "Stock",
                  component: "home",
                  prentname:"Stock"
                },
                {
                  path: "/Stock/searchstock", //库存查询
                  name: "searchstock",
                  component: "searchstock",
                  prentname:"Stock"
                },
                {
                  path: "/Stock/Separate", //到货列表
                  name: "Separate",
                  component: "Separate",
                  prentname:"Stock"
                },
                {
                  path: "/Stock/Unusual", //到货列表
                  name: "Unusual",
                  component: "Unusual",
                  prentname:"Stock"
                },
              ]
            },
            {
                path: "/Lease", //到货列表
                name: "Lease",
                component: "Lease",
                prentname:"Stock",
                children:[
                  {
                    path: "/Lease/leasedetail", //到货列表
                    name: "leasedetail",
                    component: "LeaseDetail",
                    prentname:"Stock"
                  }
                ]
            },
            {
                path: "/TransportList", //报关运输列表
                name: "TransportList",
                component: "TransportList",
                prentname:"Transport",
                children:[
                  {
                    path: "/TransportList/TransportDetail/:ID", //报关运输详情
                    name: "TransportDetail",
                    component: "TransportDetail",
                    prentname:"Transport"
                  },
                  {
                      path: "/TransportList/Customswindow", //申报窗口
                      name: "TransportListCustomswindow",
                      component: "Customswindow",
                      prentname:"Common",
                  },
                ]
            },
            {
                path: "/Szlist", //深圳入库
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
              path: "/Cgenter",  //入库
              name: "Cgenter",
              component: "Warehousing",
              prentname:"Cgenter",
              children:[
                {
                  path:"/Cgenter/Budget/:webillID/:otype",  //入库收支明细
                  name: "Cgends_meet",
                  component: "ends_meet",
                  prentname:"Common",
                },
                {
                  path:"/Cgenter/separate",  //到货暂存
                  name: "",
                  component: "separate",
                  prentname:"Common",
                },
                {
                path: "/Cgenter/Customswindow",  //入库申报窗口
                name: "",
                component: "Customswindow",
                prentname:"Cgenter"
               },
               {
                path: "/Cgenter/Declare",  //报关页面
                name: "Declare",
                component: "Declare",
                prentname:"Common",
                children:[
                    {
                      path:"/Cgenter/Declare/Budget/:webillID/:otype",  //报关收支明细
                      name: "ends_meetDeclare",
                      component: "ends_meet",
                      prentname:"Common",
                    },
                  ]
               },
              ]
            },
          ],
         
        }
      ],
      MenuItem:[],
    };
  },
  mounted() {
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
                 this.UserMenu()
                  sessionStorage.setItem('user', res.data.Role.ID)  //存储用户信息
                  sessionStorage.setItem('username', res.data.UserName)  //存储用户信息
                  sessionStorage.setItem('userID', res.data.ID)  //存储用户信息
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
                 sessionStorage.setItem('menuData', JSON.stringify(res.data))  //存储该用户拥有的权限菜单
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