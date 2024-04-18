<style scoped lang='scss'>
.layout {
  border: 1px solid #d7dde4;
  background: #f5f7f9;
  position: relative;
  border-radius: 4px;
  overflow: hidden;
}
.layout-header-bar {
  background: #fff;
  box-shadow: 0 1px 1px rgba(0, 0, 0, 0.1);
}
.layout >>> .ivu-menu {
  font-size: 16px;
}
.layout >>> .ivu-menu-item {
  font-size: 16px;
}
.layout >>> .ivu-menu-submenu-title {
  padding: 6px 4px;
  position: relative;
  cursor: pointer;
  z-index: 1;
  transition: all 0.2s ease-in-out;
}
.layout >>> .ivu-menu-item {
  padding: 6px 4px;
  position: relative;
  cursor: pointer;
  z-index: 1;
  transition: all 0.2s ease-in-out;
}
$color : #ffffff ;
.settitlecolor {
  color:$color;
}
.ivu-layout-sider {
  background: #ffffff;
}
.ivu-layout-header {
  background: #2d8cf0;
  padding: 0 50px;
  height: 30px;
  line-height: 30px;
}
.ivu-menu-dark {
    background: #1f88f6;
}
.ivu-menu-horizontal{
  height: 30px;
  line-height: 30px;
}
.ivu-layout-header{
  position: fixed;
  padding: 0;
  width: 100%;
  height: 30px;
  z-index: 999;
}
.ivu-layout.ivu-layout-has-sider {
    flex-direction: row;
    padding-top: 30px;
}
.ivu-breadcrumb {
    color: #999;
    font-size: 15px;
}

</style>
<template>
  <div class="layout">
    <Layout>
      <Header>
        <Menu mode="horizontal" theme="dark" active-name="1">
          <div class="layout-logo"></div>
          <div class="layout-nav">
            <div style="float: right; padding-right: 10px">
                <span class="settitlecolor">{{username}}</span>
                <Dropdown trigger="click" @on-click="edit">
                  <a href="javascript:void(0)">
                    <Icon type="md-arrow-dropdown" size="20" color="#ffffff" />
                  </a>
                  <DropdownMenu slot="list">
                    <DropdownItem name="tuichu">退出</DropdownItem>
                  </DropdownMenu>
                </Dropdown>
              </div>
          </div>
        </Menu>
      </Header>

      <Layout>
        <Sider
          id="Sider"
          :style="{
            position: 'fixed',
            height: '100vh',
            left: 0,
            overflow: 'auto',
            width: '180px',
          }"
          width="180"
        >
          <p style="text-align: center; margin-top: 16px">
            <!-- <img style="width:20px;height:20px" src="../assets/logo.png" alt=""> -->
          </p>
          <Menu
            theme="light"
            ref="side_menu"
            :active-name="getactive"
            :open-names="getopenname"
            width="180"
            @on-select="setmenutime"
            @on-open-change="setopenname"
          >
            <Submenu name="入库管理">
              <template slot="title">
                <Icon type="ios-analytics" />
                入库管理
              </template>
              <!-- <MenuItem  style="padding-left:30px" name="1-1">入库通知</MenuItem> -->
              <Submenu name="入库通知">
                <template slot="title" style="padding-left: 30px"
                  >入库通知</template
                >
                <MenuItem name="待入库" to="/WhingNoticePending"
                  >待入库</MenuItem
                >
                <MenuItem name="已入库" to="/WhingNoticeProcessed"
                  >已入库</MenuItem
                >
                <MenuItem name="入库单" to="/WhingStorage_record"
                  >入库单</MenuItem
                >
              </Submenu>
              <Submenu name="提货通知">
                <template slot="title" style="padding-left: 30px"
                  >提货通知</template
                >
                <MenuItem name="提货待安排" to="/Pickupgoodspending"
                  >待安排</MenuItem
                >
                <MenuItem name="提货已安排" to="/Pickupgoods"
                  >已安排</MenuItem
                >
              </Submenu>
            </Submenu>
            <Submenu name="出库管理">
              <template slot="title">
                <Icon type="ios-filing" />
                出库管理
              </template>
              <Submenu name="送货安排">
                <template slot="title" style="padding-left: 30px"
                  >送货安排</template
                >
                <MenuItem name="送货待安排" to='/Delivering'>待安排</MenuItem>
                <MenuItem name="送货已安排" to='/Delivered'>已安排</MenuItem>
                <MenuItem name="送货已完成" to='/DeliverFinish'>已完成</MenuItem>
              </Submenu>
              <Submenu name="客户自提">
                <template slot="title" style="padding-left: 30px"
                  >客户自提</template
                >
                <MenuItem name="待提货" to='/Extracting'>待提货</MenuItem>
                <MenuItem name="已提货" to='/Extracted'>已提货</MenuItem>
              </Submenu>
              <Submenu name="出库通知">
                <template slot="title" style="padding-left: 30px"
                  >出库通知</template
                >
                <MenuItem name="待出库" to='/OutPending'>待出库</MenuItem>
                <MenuItem name="已出库" to='/OutProcessed'>已出库</MenuItem>
                <MenuItem name="出库单" to='/Outputrecords'>出库单</MenuItem>
              </Submenu>
            </Submenu>
            <Submenu name="4">
              <template slot="title">
                <Icon type="ios-cog" />
                在库管理
              </template>
              <MenuItem name="库存记录" to='/StoragesList'>库存记录</MenuItem>
              <MenuItem name="库位管理" to='/ShelveList'>库位管理</MenuItem>
            </Submenu>
            <Submenu name="系统配置">
              <template slot="title">
                <Icon type="ios-cog" />
                系统配置
              </template>
              <MenuItem name="打印配置" to='/AllocationPrint'>打印配置</MenuItem>
            </Submenu>
          </Menu>
        </Sider>
        <Layout :style="{ marginLeft: '180px',overflow:'hidden'}" >
          <Content :style="{ padding: '0 10px' }">
            <Breadcrumb :style="{ margin: '6px 0' }">
              <span class="settitlecolor">{{BreadcrumbItem[0].title}}</span>
            </Breadcrumb>
            <Card class="content_box" id="content_box">
              <div >
                <router-view></router-view>
              </div>
            </Card>
          </Content>
        </Layout>
      </Layout>
    </Layout>
  </div>
</template>
<script>
export default {
  data() {
    return {
      username:sessionStorage.getItem("username")
    };
  },
  computed: {
    getactive() {
      var Acrivename = sessionStorage.getItem("Activename");
      console.log(Acrivename);
      this.$nextTick(() => {
        this.$refs.side_menu.updateActiveName();
      });
      return Acrivename;
    },
    getopenname() {
      var openname = JSON.parse(sessionStorage.getItem("openname"));
      console.log(openname);
      this.$nextTick(() => {
        this.$refs.side_menu.updateOpened();
      });
      return openname;
    },
    BreadcrumbItem() {
      return this.$store.state.common.listData;
    },
  },
  mounted(){
    this.setheigths()
  },
  methods: {
    setmenutime(name) {
      console.log(name);
      sessionStorage.setItem("Activename", name); //存储菜单
    },
    setopenname(name) {
      console.log(name);
      sessionStorage.setItem("openname", JSON.stringify(name)); //存储菜单
    },
    //退出
    edit(name) {
      console.log(name);
      if (name == "tuichu") {
        // window.sessionStorage.removeItem("menuData");
        window.sessionStorage.removeItem("user");
        window.sessionStorage.removeItem("routes");
        window.sessionStorage.removeItem("Activename");
        window.sessionStorage.removeItem("openname");

        // window.sessionStorage.removeItem("username");
        // window.sessionStorage.removeItem("WareHouseName");
        // window.sessionStorage.removeItem("UserWareHouse");
        // window.sessionStorage.removeItem("userID");
        // window.sessionStorage.removeItem("Activename");
        // window.sessionStorage.removeItem("openname");
        // window.sessionStorage.removeItem("userReadata");
        this.$router.push("/login");
      }
    },
    setheigths() {
      if (document.getElementById("content_box") != null) {
        var odiv = document.getElementById("content_box");
        var newheight = document.documentElement.clientHeight - 70;
        odiv.style.height = newheight + "px";
         console.log(newheight)
        window.onresize = function () {
          var newheight = document.documentElement.clientHeight - 70;
          odiv.style.height = newheight + "px";
           console.log(newheight)
        };
      }
    },
  },
};
</script>
