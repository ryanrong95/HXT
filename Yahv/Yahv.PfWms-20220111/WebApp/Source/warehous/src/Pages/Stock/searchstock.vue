<template>
  <div>
    <div>
      <!-- <Input v-model="search_data.Customercode" placeholder="客户编号" class="setwidth" /> -->
      <!-- <Input v-model="search_data.ID" placeholder="库存编号" class="setwidth" />
      <Input v-model="search_data.SortingID" placeholder="分拣编号" class="setwidth" />-->
      <label for="">型号：</label><Input v-model.trim="search_data.PartNumber" placeholder="型号" class="setwidth" />
      <label for="">品牌：</label><Input v-model.trim="search_data.Manufacturer" placeholder="品牌" class="setwidth" />
      <label for="">库位：</label><Input v-model.trim="search_data.ShelveID" placeholder="库位" class="setwidth" />
      <!-- <Input v-model="search_data.batch" placeholder="批次" class="setwidth" /> -->
      <label for="">开始时间与结束时间：</label>
      <DatePicker ref="element" type="daterange" placement="bottom-end" placeholder="选择入库时间" :editable="false"
        separator="  至  " @on-change="changedata" style="width:240px;"></DatePicker>
      <Button type="primary" icon="ios-search" @click="search_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
    </div>
    <div style="margin-top:20px;">
      <!-- <Table :columns="columns1" :data="data1"></Table> -->
      <Table :columns="storages_title" :data="storages_data" :loading="loading" ref="table" :max-height="tableHeight">
        <template slot-scope="{ row }" slot="PartNumber">
          <p>{{ row.PartNumber }}</p>
        </template>
        <template slot-scope="{ row }" slot="Manufacturer">
          <p>{{ row.Manufacturer }}</p>
        </template>
        <template slot-scope="{ row }" slot="Datacode">
          <p>{{ row.DateCode }}</p>
        </template>
        <template slot-scope="{ row }" slot="Origin">
          <p>{{ row.Origin }}</p>
        </template>
        <template slot-scope="{ row }" slot="Supplier">
          <p>{{ row.Supplier }}</p>
        </template>
        <template slot-scope="{ row }" slot="CreateDate">
          <p>{{ row.CreateDate | showDateexact }}</p>
        </template>
        <template slot-scope="{ row }" slot="Actions">
          <!-- <Button type="primary" size='small' icon='md-create' @click="openshowEdit(row)">修改数量</Button> -->
          <Button type="primary" size='small' icon='md-create' @click="openshowLocateEdit(row)">修改库位</Button>
        </template>
      </Table>
    </div>
    <div style="margin-top:20px;float:right">
      <!-- <Page ref="pages" :total="total" @on-change="changepage" :page-size="search_data.PageSize" show-elevator /> -->
      <Page :total="total" :page-size="search_data.PageSize" show-total :current="search_data.PageIndex"
        :page-size-opts="showPageArr" @on-page-size-change="changepagesize" @on-change="changepage" show-elevator
        show-sizer />
    </div>
    <Modal v-model="showEdit" title="修改库存数量">
      <div>
        <Input v-model.trim="EditQuantity" @on-blur='EditQuantityblur' placeholder="请输入数量" />
      </div>
      <div slot="footer">
        <Button @click="Clear">取消</Button>
        <Button @click='Ok_btn' type="primary">确认</Button>
      </div>
    </Modal>
    <Modal v-model="showLocateEdit" title="修改库位">
      <div>
        <Input v-model.trim="EditLocate" @on-blur='EditLocateblur' placeholder="请输入库位" />
      </div>
      <div slot="footer">
        <Button @click="Clear">取消</Button>
        <Button @click='Ok_Locate_btn' type="primary">确认</Button>
      </div>
    </Modal>
  </div>
</template>
<script>
import { Cgstoragesshow, CgUpdateDeliveredQty, CgUpdateDeliveredLocate } from "../../api/CgApi";

// import moment from "moment"
export default {
  data() {
    return {
      showEdit: false,
      showLocateEdit: false,
      EditQuantity: "",
      EditLocate: "",
      obj: null,
      current: 1,
      loading: true,
      search_data: {
        WhID: sessionStorage.getItem("UserWareHouse"),
        PartNumber: null,
        Manufacturer: null,
        ShelveID: null,
        StartTime: null,
        EndTime: null,
        PageIndex: 1,
        PageSize: 50
      },
      total: 0, //分页总数
      storages_title: [
        {
          type: 'index',
          width: 60,
          align: 'center'
        },
        {
          title: "型号",
          slot: "PartNumber"
        },
        {
          title: "品牌",
          slot: "Manufacturer"
        },
        {
          title: "批号",
          slot: "Datacode",
        },
        {
          title: "原产地",
          slot: "Origin",
        },
        {
          title: "供应商",
          slot: "Supplier",
        },
        {
          title: "数量",
          key: "Quantity"
        },
        {
          title: "入库时间",
          slot: "CreateDate"
        },
        {
          title: "所属库位编号",
          key: "ShelveID"
        },
        {
          title: "存储状态",
          key: "StoragesStatusDes"
        },
        {
          title: "操作",
          slot: "Actions"
        }
      ],
      storages_data: [],
      columns1: [
        {
          title: "品名",
          key: "name"
        },
        {
          title: "品牌",
          key: "age"
        },
        {
          title: "型号",
          key: "address"
        },
        {
          title: "批次号",
          key: "address"
        },
        {
          title: "数量",
          key: "address"
        },
        {
          title: "入库时间",
          key: "address"
        },
        {
          title: "进价总值",
          key: "address"
        },
        {
          title: "报关总值",
          key: "address"
        },
        {
          title: "销售单价",
          key: "address"
        },
        {
          title: "销售总值",
          key: "address"
        }
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03"
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01"
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02"
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04"
        }
      ],
      tableHeight: 500,
    };
  },
  computed: {
    showPageArr() {
      return this.$store.state.common.PageArr;
    }
  },
  mounted() {
    this.setnva();
    this.storages(this.search_data);
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "库存查询",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    storages(data) {
      //初始化数据与查询数据
      Cgstoragesshow(data).then(res => {
        if (res.val != 3) {
          if (res.obj.Total != 0) {
            this.storages_data = res.obj.Data;
            this.total = res.obj.Total;
          } else {
            this.storages_data = [];
            this.total = 0;
          }
          this.loading = false;
        }
      });
    },
    changedata(value) {
      //时间格式 获取开始时间与结束时间
      if (value[0] == '' || value[1] == '') {
        this.search_data.StartTime = null;
        this.search_data.EndTime = null;
      } else {
        this.search_data.StartTime = value[0];
        this.search_data.EndTime = value[1];
      }

    },
    changepage(value) {
      //改变页码
      this.loading = true;
      this.search_data.PageIndex = value;
      this.storages(this.search_data);
    },
    changepagesize(value) {
      this.loading = true;
      this.search_data.PageSize = value;
      this.search_data.PageIndex = 1
      this.storages(this.search_data);
    },
    search_btn() {
      if (
        this.search_data.PartNumber != null ||
        this.search_data.Manufacturer != null ||
        this.search_data.StartTime != null ||
        this.search_data.EndTime != null ||
        this.search_data.ShelveID != null
      ) {
        this.search_data.PageIndex = 1;
        this.loading = true;
        this.storages(this.search_data);
      } else {
        this.$Message.warning("请输入查询条件");
      }
    },
    empty_btn() {
      if (
        this.search_data.PartNumber != null ||
        this.search_data.Manufacturer != null ||
        this.search_data.StartTime != null ||
        this.search_data.EndTime != null ||
        this.search_data.ShelveID != null
      ) {
        this.search_data = {
          // Customercode:"",//客户编号
          // ID:"",//库存编号
          // SortingID:"",//分拣编号
          PartNumber: null, //型号
          Manufacturer: null, //品牌
          ShelveID: null,
          StartTime: null, //开始时间
          EndTime: null, //结束时间
          PageIndex: 1,
          PageSize: 10
        };
        this.loading = true;
        this.$refs.element.handleClear();
        this.storages(this.search_data);
      }
    },
    //改变数量的时候触发
    EditQuantityblur() {
      var reg = /\d+(\.\d{0,2})?/
      if (reg.test(this.EditQuantity) == false || this.EditQuantity == 0) {
        this.$Message.error('只能输入数量且不小于0');
        this.EditQuantity = ''
      }
    },
    //展开修改弹出框
    openshowEdit(row) {
      this.showEdit = true;
      this.obj = row
    },
    openshowLocateEdit(row) {
      this.showLocateEdit = true;
      this.obj = row
    },
    visiblechange(value) {
      if (value == false) {
        this.showEdit = false;
        this.obj = null
      } else {
        this.showEdit = true;
      }
      this.EditQuantity = ''
    },
    // 取消保存
    Clear() {
      this.showEdit = false;
      this.showLocateEdit = false;
      this.$Message.info('取消修改');
    },
    //提交修改
    Ok_btn() {
      console.log(this.obj)
      if (this.EditQuantity != '') {
        var reg = /\d+(\.\d{0,2})?/
        if (reg.test(this.EditQuantity) == true) {
          var data = {
            storageId: this.obj.ID,  //库存ID
            quantity: this.EditQuantity,// 在库到货数量
          }
          CgUpdateDeliveredQty(data).then(res => {
            if (res.Success == true) {
              this.$Message.success('数量修改成功');
              this.showEdit = false;
              this.loading = true;
              this.storages(this.search_data);
              this.EditQuantity = ''
            } else {
              this.$Message.error('数量修改失败');
            }
          })
        } else {
          this.$Message.error('只能输入数量且不小于0');
        }

      } else {
        this.$Message.error('数量不能为空');
      }
    },
    //修改库位
    Ok_Locate_btn() {
      console.log(this.obj)
      if (this.EditLocate != '') {
        var data = {
          storageId: this.obj.ID,  //库存ID
          locate: this.EditLocate,// 修改后库位
        }
        CgUpdateDeliveredLocate(data).then(res => {
          if (res.Success == true) {
            this.$Message.success('数量库位成功');
            this.showLocateEdit = false;
            this.loading = true;
            this.storages(this.search_data);
            this.EditLocate = ''
          } else {
            this.$Message.error('库位修改失败');
          }
        })
      } else {
        this.$Message.error('库位不能为空');
      }
    },
  },
};
</script>
<style scoped>
.setwidth {
  width: 200px;
  margin-right: 10px;
  margin-bottom: 10px;
}
</style>>
