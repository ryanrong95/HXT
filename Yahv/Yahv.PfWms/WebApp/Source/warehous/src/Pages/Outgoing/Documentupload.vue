<style scoped>
.inputwidth {
  width: 20%;
  margin-bottom: 10px;
  float: left;
}
.iteminputs {
  width: 70%;
}
.tablelable {
  font-size: 18px;
}
.buttonbox {
  clear: both;
  padding-top: 10px;
  padding-bottom: 20px;
}
.spantitle {
  display: inline-block;
  line-height: 20px;
}
.pagebox {
  padding: 20px;
  text-align: right;
}
</style>
<template>
  <div>
    <div>
      <p class="inputwidth">
        <label>入仓号：</label>
        <Input v-model.trim="search_data.code" placeholder="入仓号" class="iteminputs" />
      </p>
      <p class="inputwidth">
        <label>公司名称：</label>
        <Input v-model.trim="search_data.client" placeholder="客户名称" class="iteminputs" />
      </p>
      <p class="inputwidth">
        <label>承运商：</label>
        <Select v-model="search_data.CarrierID" class="iteminputs">
          <Option v-for="item in CarrierArr" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
        </Select>
      </p>
      <p class="inputwidth">
        <label>状态：</label>
        <Select v-model="search_data.status" class="iteminputs">
          <Option v-for="item in statusArr" :value="item.value" :key="item.value">{{ item.label }}</Option>
        </Select>
      </p>
      <Button type="primary" icon="ios-search" @click="searchbtn">查询</Button>
      <Button type="error" icon="ios-trash" @click="cancelbtn">清空</Button>
    </div>
    <div class="buttonbox">
      <Button type="primary" icon="ios-cloud-upload" size="small"  @click="clickphoto(1,'all')">传照</Button>
      <Button type="primary" icon="md-reverse-camera" size="small" @click="clickphoto(2,'all')">传照</Button>
      <span class="spantitle">注意：可批量上传</span>
    </div>
    <div>
      <Table ref="selection" :columns="titlelist" :data="listarr" :loading='loading' @on-selection-change='selecttable'> </Table>
      <div class="pagebox">
        <Page
          :total="total"
          show-elevator
          :current="search_data.PageIndex"
          :page-size="search_data.PageSize"
          @on-change='changepage'
        />
      </div>
    </div>
  </div>
</template>
<script>
import {
  GetPrinterDictionary,
  FilesProcess
} from "@/js/browser.js";
import {CgAllsCarriers,cgSZOutList} from '../../api/CgApi'
export default {
  name: "Documentupload",
  data() {
    return {
      loading:true,
      Selectrow:[],
      warehouseID: sessionStorage.getItem("UserWareHouse"), //当前库房ID  
      total: 0, //总条数
      search_data: {
        warehouseid:sessionStorage.getItem("UserWareHouse"),
        code :null,
        client:null,
        carrier:null,
        status:0,
        pageindex:1,
        pagesize:5,
      },
      statusArr: [
        {
          value: 0,
          label: "全部"
        },
        {
          value: 1,
          label: "未上传"
        },
        {
          value: 2,
          label: "已上传"
        }
      ],
      titlelist: [
        {
          type: "selection",
          width: 60,
          align: "center"
        },
        {
          type: "index",
          width: 60,
          align: "center"
        },
        {
          title: "入仓号",
          key: "EnterCode",
          align: "center"
        },
        {
          title: "公司名称",
          key: "ClientName",
          align: "center"
        },
        {
          title: "承运商",
          key: "CarrierName",
          align: "center"
        },
        {
          title: "送货单号",
          key: "ID",
          align: "center"
        },
        {
          title: "上传状态",
          key: "status",
          align: "center"
        },
        {
          title: "操作",
          key: "action",
          align: "center",
          render: (h, params) => {
            return h("div", [
              h(
                "Button",
                {
                  props: {
                    type: "primary",
                    size: "small",
                    icon: "ios-cloud-upload"
                  },
                  style: {
                    marginRight: "5px"
                  },
                  on: {
                    click: () => {}
                  }
                },
                "传照"
              ),
              h(
                "Button",
                {
                  props: {
                    type: "primary",
                    size: "small",
                    icon: "md-reverse-camera"
                  },
                  on: {
                    click: () => {}
                  }
                },
                "拍照"
              )
            ]);
          }
        }
      ],
      listarr: [],
      CarrierArr:[],//承运商列表
    };
  },
  created() {
    this.setnva();
    this.CgAllsCarriers()
    window["PhotoUploaded"] = this.changed;
  },
  mounted() {
    this.cgSZOutList()
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "批量上传送货文件",
          href: ""
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    //传照拍照
    clickphoto(type, itemType) {
      var data=[];
      if(itemType=='all'){
        if(this.Selectrow.length<=0){
          this.$Message.error('请选择要操作的送货单');
        }else{
          for(var i=0;i<this.Selectrow.length;i++){
            var item={
            SessionID: this.Selectrow[i].ID,
            AdminID: sessionStorage.getItem("userID"),
            Data: {
                WayBillID: null,
                WsOrderID: null,
                NoticeID: null,
                InputID: null,
                Type: null
              }
            }
            data.push(item)
          }
        }
        
      }else{
        var item={
          SessionID: 'all',
          AdminID: sessionStorage.getItem("userID"),
          Data: {
              WayBillID: null,
              WsOrderID: null,
              NoticeID: null,
              InputID: null,
              Type: null
            }
          }
          data.push(item)
      }
     console.log(data)
     if(type==1){
      //  SeletUploadFiles(data)
     }else{
      //  FormPhotos(data)
     }
    },
    // 搜索
    searchbtn() {
      this.loading=true;
      this.cgSZOutList()
    },
    // 清空搜索
    cancelbtn() {
      this.loading=true;
      this.search_data.pageindex=1
      this.cgSZOutList()
    },
    //获取承运商
    CgAllsCarriers(){
        CgAllsCarriers(this.warehouseID).then(res=>{
            this.CarrierArr=res
        })
    },
    //获取列表数据
    cgSZOutList(){
      cgSZOutList(this.search_data).then(res=>{
         this.loading=false;
         this.listarr=res.obj.Data
         this.total=res.obj.Total
        console.log(res)
      })
    },
    //修改页码
    changepage(value){
      this.search_data.pageindex=value
      this.loading=true;
      this.cgSZOutList()
    },
    changed(msg){
      alert(JSON.stringify(msg))
      this.cgSZOutList()
    },
    selecttable(value){
      this.Selectrow=value
    }
  }
};
</script>