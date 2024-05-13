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
              <!-- <Icon @click.native="collapsedSider" :class="rotateIcon" type="md-menu" size="24"></Icon> -->
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
              <div style="color:#ffffff" v-if="current!=null">
                <span style="float:left">{{ current }}</span>
                <div style="display: inline-block;">
                  <!-- <Cascader :data="WareHouseRoles" change-on-select  @on-change="handleChange">
                    <Icon type="md-arrow-dropdown" size="20" color="#ffffff" />
                  </Cascader>-->

                  <Cascader
                    :data="WareHouseRoles"
                    @on-change="handleChange"
                    @on-visible-change="changestatus"
                    change-on-select
                  >
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
                <DropdownItem name="msg">消息</DropdownItem>-->
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
          accordion
          :collapsed-width="78"
          v-model="isCollapsed"
          :style="{background: '#fff'}  "
        >
          <Menu
            ref="side_menu"
            v-if="current!=null"
            :active-name="getactive"
            theme="light"
            width="auto"
            accordion
            :open-names="getopenname"
            :class="menuitemClasses"
            @on-select="setmenutime"
            @on-open-change="setopenname"
          >
            <Submenu
              v-for="(item,index) in MenuItem"
              :name="item.openname"
              :key="index"
              width="auto"
            >
              <template slot="title">
                <i :class="item.icon "></i>
                {{item.name}}
              </template>
              <MenuItem
                v-for="(i_menu,index) in item.children"
                :name="i_menu.activatname"
                :to="i_menu.router"
                :key="index"
              >{{i_menu.name}}</MenuItem>
            </Submenu>
          </Menu>
        </Sider>
        <Layout
          :style="{padding: '0 24px 24px','overflow-y':'scroll',}"
          class="content_box"
          id="content_box"
        >
          <Breadcrumb :style="{margin: '24px 0'}" v-if="BreadcrumbItem!='undefined'">
            <BreadcrumbItem
              v-for="(item,index) in BreadcrumbItem"
              :key="index"
              :to="item.href"
            >{{item.title}}</BreadcrumbItem>
          </Breadcrumb>
          <Content :style="{padding: '24px', background: '#fff'}" class="Contentview">
            <router-view v-if="isRouterAlive"></router-view>
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
import { Cgwarehouses, CgwarehousesMenus } from "../api/CgApi";

export default {
  // inject: ["reload"],
  name: "layout",
  provide() {
    return {
      reload: this.reload,
    };
  },
  data() {
    return {
      isRouterAlive: true,
      Housid: sessionStorage.getItem("UserWareHouse"),
      activename: "在库管理",
      infobox: true,
      isCollapsed: false,
      WareHouseRoles: [],
      MenuItem: [],
      current: null,
      username: "", //用户名
      menuitem: [
        {
          name: "入库管理",
          icon: "",
          children: [
            {
              name: "首页",
              acrivename: "home",
              router: "/",
            },
            {
              name: "入库管理",
              acrivename: "ruku",
              router: "/Warehousing",
            },
          ],
        },
        {
          name: "库房",
          icon: "",
          router: "/",
        },
      ],
      newmenuitem: [],
      text: "jiangsu",
      data2: [
        {
          value: "beijing",
          label: "北京",
          code: "HK01",
          children: [],
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
                  children: [],
                },
              ],
            },
            {
              value: "suzhou",
              label: "苏州",
              code: "SZ50",
              children: [],
            },
          ],
        },
      ],
      data: [
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
      ],
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
      // console.log(this.MenuItem)
      if(this.MenuItem.length>0){
          this.$nextTick(() => {
          this.$refs.side_menu.updateActiveName();
         });
         return this.$store.state.common.Acrivename;
      }
      
    },
    getopenname() {
      console.log(this.MenuItem)
      if(this.MenuItem.length>0){
         var arrs = [];
         arrs.push(this.$store.state.common.openname);
         this.$nextTick(() => {
            this.$refs.side_menu.updateOpened();
         });
          return arrs;
      }
     
    },
  },
  mounted() {
    this.setheigths();
  },
  watch: {
    getopenname() {
      // console.log(this.getopenname)
    },
    $route(to, from) {
      clearInterval();
    },
  },
  created() {
    this.username = sessionStorage.getItem("username");
    console.log(sessionStorage.getItem("UserWareHouse"));
    this.Cgwarehouses();
    // this.MenuItem=JSON.stringify(sessionStorage.getItem("menuData"))
    
  },
  methods: {
    reload() {
      this.isRouterAlive = false;
      this.$nextTick(function () {
        this.isRouterAlive = true;
      });
    },
    setheigths() {
      if (document.getElementById("content_box") != null) {
        var odiv = document.getElementById("content_box");
        var newheight = document.documentElement.clientHeight - 50;
        odiv.style.height = newheight + "px";
        //  console.log(odiv)
        window.onresize = function () {
          var newheight = document.documentElement.clientHeight - 50;
          odiv.style.height = newheight + "px";
          //  console.log(document.getElementById("content_box").style.height)
        };
      }
    },
    changestatus(value) {
      //选择库房以后，跳转到首页
      // console.log(value)
      if (value == false) {
        this.$router.push("/"); //控制页面跳到首页
        // this.reload() //刷新当前页面
        this.getMenu();
        sessionStorage.setItem("openname", this.MenuItem[0].openname);
        this.$store.dispatch("setopenname", this.MenuItem[0].openname);
        sessionStorage.setItem(
          "Activename",
          this.MenuItem[0].children[0].activatname
        );
        this.$store.dispatch(
          "setAcrivename",
          this.MenuItem[0].children[0].activatname
        );
      }
    },
    handleChange(value, selectedData) {
      var codes = "";
      var Names = "";
      selectedData.forEach((item) => {
        codes = item.value;
        Names = item.label;
        sessionStorage.setItem("WareHouseName", Names); //存储库房编号
        sessionStorage.setItem("UserWareHouse", codes); //存储库房名称
        this.current = Names;
      });
    },
    getMenu() {
      //根据库房获取对应菜单
      var Housid = sessionStorage.getItem("UserWareHouse");
      CgwarehousesMenus(Housid).then((res) => {
        this.MenuItem = res;
        if (this.MenuItem.length > 0) {
          sessionStorage.setItem("menuData", JSON.stringify(res));
          if (sessionStorage.getItem("openname") == null) {
            sessionStorage.setItem("openname", this.MenuItem[0].openname);
            this.$store.dispatch("setopenname", this.MenuItem[0].openname);
          } else {
            var newmenu = sessionStorage.getItem("openname");
            this.$store.dispatch("setopenname", newmenu);
          }
          if (sessionStorage.getItem("Activename") == null) {
            sessionStorage.setItem(
              "Activename",
              this.MenuItem[0].children[0].activatname
            );
            this.$store.dispatch(
              "setAcrivename",
              this.MenuItem[0].children[0].activatname
            );
          } else {
            var newmenu = sessionStorage.getItem("Activename");
            this.$store.dispatch("setAcrivename", newmenu);
          }
        }
      });
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
        window.sessionStorage.removeItem("openname");
        window.sessionStorage.removeItem("userReadata");
        this.$router.push("/login");
      }
    },
    Cgwarehouses() {
      //获取用户拥有权限的库房
      Cgwarehouses().then((res) => {
        var newres = [];
        var housearr = res;
        var userinfo = JSON.parse(sessionStorage.getItem("userReadata")).Role;
        var HKs = "FRole014";
        var SZs = "FRole015";
        console.log(userinfo);
        if (userinfo.IsSuper == true) {
          newres = res;
        } else {
          if (userinfo.ChildRoles.length > 0) {
            for (var i = 0; i < userinfo.ChildRoles.length; i++) {
              if (userinfo.ChildRoles[i].ID == HKs) {
                newres.push(housearr[0]);
              } else if (userinfo.ChildRoles[i].ID == SZs) {
                newres.push(housearr[1]);
              } else {
                newres = res;
              }
            }
          } else {
            if (userinfo.ID == HKs) {
              newres.push(housearr[0]);
            } else if (userinfo.ID == SZs) {
              newres.push(housearr[1]);
            }
          }
        }

        this.WareHouseRoles = newres;
        if (newres.length > 0) {
          this.current = newres[0].label;
          var Storage = sessionStorage.getItem("WareHouseName");
          var code = sessionStorage.getItem("UserWareHouse");
          // console.log(Storage)
          if (Storage == null || code == null) {
            sessionStorage.setItem("UserWareHouse", newres[0].value); //存储用户信息库房编号
            sessionStorage.setItem("WareHouseName", newres[0].label); //存储用户信息 库房名称
            this.current = newres[0].label;
          } else {
            this.current = Storage;
          }
        } else {
          this.current = null;
        }

        this.getMenu();
      });
    },
    setmenutime(name) {
      sessionStorage.setItem("Activename", name); //存储菜单
      this.$store.dispatch("setAcrivename", name);
      // console.log(name)
    },
    setopenname(name) {
      var name = name[0];
      // console.log(name)
      sessionStorage.setItem("openname", name); //存储菜单
      this.$store.dispatch("setopenname", name);
    },
  },
};
</script>
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
  /* width: 100px; */
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
