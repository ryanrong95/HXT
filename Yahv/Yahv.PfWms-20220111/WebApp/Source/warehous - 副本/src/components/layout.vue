<template>
  <div class="layout">
    <Layout>
      <Header>
        <Menu mode="horizontal" theme="dark" active-name="1">
          <div class="logobox">
            <div class="layout-logo">
              <img src="../assets/img/logo.png" alt />
            </div>
            <div class="closeicon">
              <Icon @click.native="collapsedSider" :class="rotateIcon" type="md-menu" size="24"></Icon>
            </div>
            <div
              class="WareHouseRoles"
              style="min-width:130px;height: 40px;color: #ffffff;float: left;"
            >
              <!-- <Select v-model="current">
                             <Option v-for="item in WareHouseRoles" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
              </Select>-->
              <!-- <Avatar src="https://i.loli.net/2017/08/21/599a521472424.jpg" />0 -->
              <!-- <span>{{current}}</span> -->
              <!-- <Dropdown trigger="click" @on-click="sethous">
                            <a href="javascript:void(0)">
                               <Icon type="md-arrow-dropdown" size="20" color="#ffffff" />
                            </a>
              </Dropdown>-->
              <div style="color:#ffffff">
                <span style="float:left">{{ current }}</span>
                <div style="display: inline-block;">
                  <Cascader :data="WareHouseRoles" @on-change="handleChange">
                    <Icon type="md-arrow-dropdown" size="20" color="#ffffff" />
                  </Cascader>
                </div>
              </div>
            </div>
          </div>
          <div class="Avatar">
            <!-- <Avatar src="https://i.loli.net/2017/08/21/599a521472424.jpg" /> -->
            <span style="color:#ffffff">{{username}}</span>
            <Dropdown trigger="click" @on-click="edit">
              <a href="javascript:void(0)">
                <Icon type="md-arrow-dropdown" size="20" color="#ffffff" />
              </a>
              <DropdownMenu slot="list">
                <!-- <DropdownItem name="center">个人中心</DropdownItem>
                <DropdownItem name="account">账号设置</DropdownItem>
                <DropdownItem name="msg">消息</DropdownItem> -->
                <DropdownItem name="tuichu">退出</DropdownItem>
              </DropdownMenu>
            </Dropdown>
          </div>
        </Menu>
      </Header>
      <Layout>
        <Sider
          ref="side1"
          hide-trigger
          collapsible
          :collapsed-width="78"
          v-model="isCollapsed"
          :style="{background: '#fff'}  "
        >
          <Menu
            :active-name="getactive"
            theme="light"
            width="auto"
            :open-names="['1']"
            :class="menuitemClasses"
            @on-select="setmenutime"
          >
            <MenuItem
              v-for="(item,index) in newmenuitem"
              :name="item.acrivename"
              :key="index"
              :to="item.router"
            >
              <i :class="item.icon"></i>
              <span>{{item.name}}</span>
            </MenuItem>
            <!-- <MenuItem  to="/TransportList">
                         <i class="iconfont icon-kufangguanli"></i>
                         <span>测试</span>
            </MenuItem> -->
          </Menu>
        </Sider>
        <Layout :style="{padding: '0 24px 24px','overflow-y':'scroll',}" class="content_box" id="content_box">
          <Breadcrumb :style="{margin: '24px 0'}" v-if="BreadcrumbItem!='undefined'">
            <BreadcrumbItem
              v-for="(item,index) in BreadcrumbItem"
              :key="index"
              :to="item.href"
            >{{item.title}}</BreadcrumbItem>
          </Breadcrumb>
          <Content :style="{padding: '24px', background: '#fff'}" class="Contentview">
            <router-view></router-view>
          </Content>
        </Layout>
      </Layout>
    </Layout>
  </div>
</template>

<script>
import { mapActions, mapGetters } from "vuex";
// import {UserWareHouseRoles} from "@/api"
import { UserWareHouseRoles } from "../api/index";
export default {
  inject: ["reload"],
  name: "layout",
  data() {
    return {
      activename: "在库管理",
      infobox: true,
      isCollapsed: false,
      WareHouseRoles: [],
      MenuItem: [],
      MenuItem1: [
        {
          name: "首页",
          acrivename: "首页",
          router: "/",
          icon: "iconfont icon-shouye"
        },
        {
          name: "入库管理",
          acrivename: "入库管理",
          router: "/Warehousing",
          icon: "iconfont icon-rukuguanli"
        },
        {
          name: "深圳入库",
          acrivename: "深圳入库",
          router: "/Szlist",
          icon: "iconfont icon-rukuguanli"
        },
        {
          name: "出库管理",
          acrivename: "出库管理",
          router: "/Outgoing",
          icon: "iconfont icon-chukuguanli"
        },
        {
          name: "报关运输",
          acrivename: "报关运输",
          router: "/TransportList",
          icon: "iconfont icon-kuaidi-"
        },
        {
          name: "在库管理",
          acrivename: "在库管理",
          router: "/Stock",
          icon: "iconfont icon-zaiku1"
        },
        {
          name: "报表统计",
          acrivename: "报表统计",
          router: "",
          icon: "iconfont icon-tongji"
        },
        {
          name: "库房设置",
          acrivename: "库房设置",
          router: "/storage",
          icon: "iconfont icon-kufangguanli"
        },
        {
          name: "系统配置",
          acrivename: "系统配置",
          router: "/Allocation",
          icon: "iconfont icon-shezhi"
        }
      ],
      current: "",
      username: "", //用户名
      menuitem: [
        {
          name: "入库管理",
          icon: "",
          children: [
            {
              name: "首页",
              acrivename: "home",
              router: "/"
            },
            {
              name: "入库管理",
              acrivename: "ruku",
              router: "/Warehousing"
            }
          ]
        },
        {
          name: "库房",
          icon: "",
          router: "/"
        }
      ],
      newmenuitem: [],
      text: "jiangsu",
      data2: [
        {
          value: "beijing",
          label: "北京",
          code: "HK01",
          children: []
        },
        {
          value: "jiangsu",
          label: "江苏",
          code: "JS00",
          children: [
            {
              value: "nanjing",
              label: "南京",
              code: "NJ02",
              children: [
                {
                  value: "dapaidang",
                  label: "大排档",
                  code: "DPA_01",
                  children: []
                }
              ]
            },
            {
              value: "suzhou",
              label: "苏州",
              code: "SZ50",
              children: []
            }
          ]
        }
      ],
      data:[
          {
             value: "beijing1",
             label: "北京库房",
             code: "HK01",
          },
          {
             value: "beijing2",
             label: "北京市场1",
             code: "HK01-01",
          },
          {
             value: "beijing3",
             label: "北京市场2",
             code: "HK01-02",
          },
          {
             value: "beijing4",
             label: "深圳库房",
             code: "SZ01",
          },
          {
             value: "xianggang",
             label: "香港库房",
             code: "HK02",
          },
        ]
    };
  },
  computed: {
    rotateIcon() {
      return ["menu-icon", this.isCollapsed ? "rotate-icon" : ""];
    },
    menuitemClasses() {
      return ["menu-item", this.isCollapsed ? "collapsed-menu" : ""];
    },
    BreadcrumbItem() {
      return this.$store.state.homeindex.listData;
    },
    getactive() {
      return  this.$store.state.common.Acrivename;
    }
  },
  mounted() {
       this.setheigths()
       this.UserWareHouseRoles()
       this.changemenu();
    //    alert("fhdsfhkj")
  },
  created() {
    // 从sessionStorage得到menuData
    this.MenuItem = JSON.parse(sessionStorage.getItem("menuData"));
    this.username = sessionStorage.getItem("username");
    // this.$store.dispatch("setAcrivename", "首页");
    console.log(sessionStorage.getItem("Activename"))
    if(sessionStorage.getItem("Activename")==null){
       sessionStorage.setItem("Activename",this.MenuItem[0].acrivename);
       this.$store.dispatch("setAcrivename",this.MenuItem[0].acrivename);
    }else{
        var newmenu=sessionStorage.getItem("Activename")
        this.$store.dispatch("setAcrivename",newmenu);
    }
  },
  //  beforeRouteUpdate (to, from, next) {
  //    console.log(to)
  //   if(to.path!=='/Outgoing'){
  //       clearTimeout(this.settimeouts);
  //   }else if(to.path!=='/Warehousing'){
  //      clearTimeout(this.setIntervaltimer);
  //   }
  //   next()
  //   // 在当前路由改变，但是该组件被复用时调用
  //   // 举例来说，对于一个带有动态参数的路径 /foo/:id，在 /foo/1 和 /foo/2 之间跳转的时候，
  //   // 由于会渲染同样的 Foo 组件，因此组件实例会被复用。而这个钩子就会在这个情况下被调用。
  //   // 可以访问组件实例 `this`
  // },
  methods: {
    setheigths(){
      if (document.getElementById("content_box") != null) {
        var odiv = document.getElementById("content_box");
        var newheight = document.documentElement.clientHeight - 50;
        odiv.style.height = newheight + "px";
        //  console.log(odiv)
        window.onresize = function() {
          var newheight = document.documentElement.clientHeight - 50;
          odiv.style.height = newheight + "px";
          //  console.log(document.getElementById("content_box").style.height)
        };
      }
    },
    handleChange(value, selectedData) {
      console.log(value)
      console.log(selectedData)
      var codes = "";
      var Names = "";
      selectedData.forEach(item => {
        codes = item.code;
        Names = item.label;
        //   this.text=(item.label).json('/')
      });
      // this.current = selectedData.map(o => o.label).join("/ ");
      this.current = selectedData.map(o => o.label).join(', ');
      sessionStorage.setItem("WareHouseName", Names); //存储库房编号
      sessionStorage.setItem("UserWareHouse", codes); //存储库房名称
      // this.$router.push("/");//控制页面跳到首页
      this.$router.push("/Warehousing");//控制页面跳到入库页面
      this.reload() //刷新当前页面
      sessionStorage.setItem("Activename",this.MenuItem[0].acrivename);
      this.$store.dispatch("setAcrivename",this.MenuItem[0].acrivename);
    },
    changemenu() {
      if (this.MenuItem != []) {
        for (var i = 0; i < this.MenuItem.length; i++) {
          for (var j = 0; j < this.MenuItem1.length; j++) {
            if (this.MenuItem[i].name == this.MenuItem1[j].name) {
              this.newmenuitem.push(this.MenuItem1[j]);
            }
          }
        }
        var test= {
          name: "报关运输",
          acrivename: "报关运输",
          router: "/TransportList",
          icon: "iconfont icon-kuaidi-"
        }
        var test3= {
          name: "入库重构",
          acrivename: "入库重构",
          router: "/Cgenter",
          icon: "iconfont icon-kuaidi-"
        }
        // var test2= {
        //   name: "深圳入库",
        //   acrivename: "深圳入库",
        //   router: "/Szlist",
        //   icon: "iconfont icon-rukuguanli"
        // }
        // this.newmenuitem.push(test);
        // this.newmenuitem.push(test2);
        this.newmenuitem.push(test3);
        // this.activename=this.newmenuitem[0].acrivename
      }
    },
    collapsedSider() {
      this.$refs.side1.toggleCollapse();
    },
    edit(name) {
      //退出
      if (name == "tuichu") {
        window.sessionStorage.removeItem("menuData");
        window.sessionStorage.removeItem("user");
        window.sessionStorage.removeItem("routes");
        window.sessionStorage.removeItem("username");
        window.sessionStorage.removeItem("WareHouseName");
        window.sessionStorage.removeItem("UserWareHouse");
        window.sessionStorage.removeItem("userID");
        window.sessionStorage.removeItem("Activename");
        this.$router.push("/login");
      }
    },
    sethous(name) {
      //设置库房，请求不库房数据
      this.current = name;
      var arr = this.WareHouseRoles;
      //   console.log(this.$route.path)
      var router = this.$route.path;
      for (var i = 0; i < arr.length; i++) {
        if (arr[i].Name == name) {
          //    console.log(arr[i])
          sessionStorage.setItem("WareHouseName", arr[i].Name);
          sessionStorage.setItem("UserWareHouse", arr[i].Code); //存储用户信息
          this.$router.push("/"); //控制页面跳到首页
          // this.$router.push(router)  //刷新当前页，不跳转
          this.reload(); //刷新当前页面
          //    var actives=""
          //    for(var i=0;i<this.MenuItem.length;i++){
          //        if(this.MenuItem[i].router==router){
          //            actives=this.MenuItem[i].acrivename;
          //        }
          //    }
          //    this.acrivename=actives;
          //   console.log(actives)
        }
      }
    },
    UserWareHouseRoles() {
      // sessionStorage.setItem('UserWareHouse', "HK01")  //存储用户信息库房编号临时库房
      UserWareHouseRoles().then(res => {
        if (res.code == 200) {
          this.WareHouseRoles = res.data;
          this.current = res.data[0].label;
          var Storage = sessionStorage.getItem("WareHouseName");
          var code = sessionStorage.getItem("UserWareHouse");
          // console.log(Storage)
          if (Storage == null || code == null) {
            sessionStorage.setItem("UserWareHouse", res.data[0].code); //存储用户信息库房编号
            sessionStorage.setItem("WareHouseName", res.data[0].label); //存储用户信息 库房名称
            this.current = res.data[0].label;
          } else {
            this.current = Storage;
          }
        }
      });
    },
    setmenutime(name){
      sessionStorage.setItem("Activename",name); //存储菜单
      this.$store.dispatch("setAcrivename",name);
    }
  }
};

// window.onload = function() {
//   // console.log(document.getElementById("content_box"))
//   if (document.getElementById("content_box") != null) {
//     var odiv = document.getElementById("content_box");
//     var newheight = document.documentElement.clientHeight - 50;
//     odiv.style.height = newheight + "px";
//      console.log(odiv.style.height)
//     //  console.log(odiv)
//     window.onresize = function() {
//       var newheight = document.documentElement.clientHeight - 50;
//       odiv.style.height = newheight + "px";
//       //  console.log(document.getElementById("content_box").style.height)
//     };
//   }
// };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->

<style scoped>
.ivu-menu-dark {
  background: #2d8cf0;
}
.ivu-layout-header {
  background: #2d8cf0;
  padding: 0 50px;
  height: 64px;
  line-height: 64px;
}
.layout {
  border: 1px solid #d7dde4;
  background: #f5f7f9;
  position: relative;
  border-radius: 4px;
  overflow: hidden;
  height: 100%;
}
.ivu-layout {
  min-height: 100%;
}
.layout-logo {
  width: 288px;
  height: 42px;
  /* background: #5b6270; */
  border-radius: 3px;
  float: left;
  position: relative;
  line-height: 40px;
  top: 10px;
  /* left: 20px; */
}
.layout-nav {
  width: 420px;
  margin: 0 auto;
  margin-right: 20px;
}
.logobox {
  min-width: 200px;
  height: auto;
}
.closeicon {
  float: left;
  color: #ffffff;
  /* margin-left: 20px; */
}
.content_box {
  height: auto;
  overflow-y: scroll;
  max-height: 100%;
  margin-bottom: 50px;
}
.menu-icon {
  transition: all 0.3s;
}
.rotate-icon {
  transform: rotate(-90deg);
}
.menu-item span {
  display: inline-block;
  overflow: hidden;
  width: 69px;
  text-overflow: ellipsis;
  white-space: nowrap;
  vertical-align: bottom;
  transition: width 0.2s ease 0.2s;
}
.menu-item i {
  transform: translateX(0px);
  transition: font-size 0.2s ease, transform 0.2s ease;
  vertical-align: middle;
  font-size: 16px;
}
.collapsed-menu span {
  width: 0px;
  transition: width 0.2s ease;
}
.collapsed-menu i {
  transform: translateX(5px);
  transition: font-size 0.2s ease 0.2s, transform 0.2s ease 0.2s;
  vertical-align: middle;
  font-size: 22px;
}

.Avatar {
  float: right;
  margin-right: 50px;
}
.WareHouseRoles {
  width: 100px;
  height: 40px;
  color: #ffffff;
}
.WareHouseRoles .ivu-select-arrow {
  position: absolute;
  top: 50%;
  right: 8px;
  line-height: 1;
  transform: translateY(-50%);
  font-size: 14px;
  color: #ffffff;
  transition: all 0.2s ease-in-out;
}
.WareHouseRoles .ivu-select-single .ivu-select-selection {
  height: 32px;
  position: relative;
  border: 0;
  background: #2d8cf0;
  color: #ffffff;
}
.ivu-menu-vertical.ivu-menu-light:after {
  width: 0;
}
</style>
