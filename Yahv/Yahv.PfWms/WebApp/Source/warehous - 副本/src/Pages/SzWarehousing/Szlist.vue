<style scoped>
.input_width {
  width: 200px;
  margin-right: 15px;
}
.pages {
  float: right;
  padding-top: 20px;
}
.mb20 {
  margin-bottom: 20px;
}
</style>
<template>
  <div>
    <!-- 搜索条件 -->
    <div class="mb20">
      <Input v-model.trim="searchData.lotNumber" placeholder="运输批次号" class="input_width" />
      <Select v-model.trim="searchData.carrierID" class="input_width" placeholder="承运商">
        <Option v-for="item in carrierList" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
      </Select>
      <Button type="primary" @click="search">查询</Button>
      <Button type="error" @click="reset">重置</Button>
    </div>
    <!-- 运输列表 -->
    <div>
      <Table :columns="listTitle" :data="listData" :loading="loading">
        <template slot-scope="{ row, index }" slot="DepartDate">
          <p>{{row.DepartDate|showDate}}</p>
        </template>
        <template slot-scope="{ row, index }" slot="CarNumber">
          <span>{{row.CarNumber1}}</span> &nbsp;&nbsp;&nbsp;
          <span>{{row.CarNumber2}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" icon="md-create" size="small" @click="actionBtn(row)">详情</Button>
          <Button disabled icon="md-checkmark-circle" size="small">完成</Button>
        </template>
      </Table>
    </div>
    <div class="pages">
      <Page
        :total="total"
        :page-size="searchData.PageSize"
        :current="searchData.PageIndex"
        @on-change="changePage"
      />
    </div>

    <Drawer
      :closable="true"
      v-model="showTransportDetail"
      @on-visible-change="DrawerState"
      :width="70"
    >
      <router-view></router-view>
    </Drawer>
  </div>
</template>
<script>
import { CustomTransport, Carriers } from '../../api'
export default {
  data() {
    return {
      carrierList: [],
      listTitle: [
        {
          type: 'index',
          width: 60,
          align: 'center'
        },
        {
          title: '运输批次号',
          key: 'LotNumber'
        },
        {
          title: '承运商',
          key: 'CarrierName'
        },
        {
          title: '车牌号',
          slot: 'CarNumber'
        },
        {
          title: '运输时间',
          slot: 'DepartDate'
        },
        {
          title: '司机姓名',
          key: 'Driver'
        },
        {
          title: '运输类型',
          key: 'WaybillTypeDes'
        },
        {
          title: '总箱数',
          key: 'BoxNumber'
        },
        {
          title: '已上架箱数',
          key: 'UpperNumber'
        },
        {
          title: '操作',
          slot: 'action',
          width: 200,
          align: 'center'
        }
      ],
      listData: [
        // {
        //   LotNumber: '1100258973020',
        //   CarrierName: '运达国际物流有限公司',
        //   CarNumber1: 'WD 4364',
        //   DepartDate: '2019-01-02',
        //   Driver: '张三',
        //   WaybillTypeDes: '普通',
        //   BoxNumber: '10',
        //   shelvesNumber: '9'
        // }
      ],
      loading: false, //列表是否在加载中
      total: 0, //列表总行数
      carrierName: '', //
      searchData: {
        type: '310', // 类型
        warehouseID: sessionStorage.getItem('UserWareHouse'), //库房编号
        lotNumber: '', //运输批次号
        carrierID: '', //承运商ID
        PageIndex: 1, //当前页面
        PageSize: 10 //当前页数
      },
      getDataInterval:null
    }
  },
  computed: {
    showTransportDetail: {
      get: function() {
        return this.$store.state.common.TransportDetail
      },
      set: function(newValue) {}
    }
  },
  created() {
    this.Carriers()
    this.loading = true
    this.getListData()
    this.getDataInterval = setInterval(() => {
      this.getListData()
    }, 6000)
  },
  mounted() {
    this.setNav()
  },
  methods: {
    setNav() {
      var cc = [
        {
          title: '待入库',
          href: '/TransportList'
        }
      ]
      this.$store.dispatch('setnvadata', cc)
    },
    actionBtn(row) {
      //点击进入处理页面
      this.$store.dispatch('setTransportDetail', true)
      this.$router.push({ path: '/Szlist/SzDetail/' + row.LotNumber })
    },
    DrawerState(value) {
      if (value == true) {
        this.$store.dispatch('setTransportDetail', true)
      } else {
        this.$store.dispatch('setTransportDetail', false)
        this.$router.go(-1) //控制路由跳回原来页面
      }
    },
    Carriers() {
      //承运商列比奥
      Carriers().then(res => {
        this.carrierList = res.obj
      })
    },
    getListData() {
      CustomTransport(this.searchData).then(res => {
        this.listData = res.obj.Data
        this.total = res.obj.Total
        this.loading = false
      })
    },
    changePage(value) {
      this.searchData.PageIndex = value
      this.loading = true
      this.getListData()
    },
    search() {
      //搜索数据请求接口
      this.searchData.PageIndex = 1
      this.loading = true
      this.getListData()
    },
    reset() {
      //重置数据
      this.searchData.lotNumber = ''
      this.searchData.carrierID = ''
      this.searchData.PageIndex = 1
      this.loading = true
      this.getListData()
    }
  },
  beforeDestroy () {
    clearInterval(this.getDataInterval)
  }
}
</script>
