<template>
  <div class="Storage">
    <div class="search_btn">
      <Input v-model.trim="area_search.ID" placeholder="库房编号"  class="searchtable" />
        <Select v-model="area_search.Purpose" filterable class="searchtable" placeholder="业务类型">
          <Option v-for="(item,index) in interestlist" :value="item.ID" :key="item.index">{{ item.purpose }}</Option>
        </Select>
        <Select 
          class="searchtable"
          placeholder="请输入并选择负责人"
          v-model="area_search.ManagerID"
          filterable
          remote
          :remote-method="remoteMethod2"
          :loading="loading1">
          <Option v-for="(option, index) in search_damin" :value="option.ID" :key="index">{{option.RealName}}</Option>
        </Select>
      <Button type="primary" icon="ios-search" @click="query_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
    </div>
    <div>
      <Table :columns="wearhoustitle" :data="wearhousdata"  :loading="loading">
        <template slot-scope="{ row }" slot="name">
          <strong>{{ row.Name }}</strong>
          <Tag v-for="item in row.AbilitiesDes1" :key="item.Key" color="#ac0">{{item.Value}}</Tag>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" size="small" style="margin-right: 5px" :to="{name:'Logichouse',params:{fatherID:row.ID}}">查看门牌库房</Button>
          <Button type="primary" size="small" style="margin-right: 5px" :to="{name:'Areapage',params:{fatherID:row.ID}}">查看库区</Button>
          <!-- <Button type="primary" size="small" style="margin-right: 5px" @click="showmodel(2,row)">编辑</Button> -->
          <!-- <Button type="error" size="small" @click="remove(row)">删除</Button> -->
        </template>
      </Table>
    </div>
    <div class="buttonbox">
    
      <!-- <div style="float:right">
    <Page :total="100" show-elevator />
  </div> -->
    </div>
  </div>
</template>
<script>
// import Forms from "./form";
import {GetWarehouse,GetPurposes,GetAdmins, } from "../../api"; //引入api 的接口
import {PageEvent} from "@/js/browser.js"
export default {
  name: "Storage",
  components: {
    // Forms
  },
  data() {
    return {
      interestlist:[],
      search_damin:[],
      loading1:false,
      area_search: {
        ID:"",//编号
        Purpose:"",  //业务类型
        ManagerID:"",//负责人
      },
      loading:true,
      isbtns:0,//区分是修改还是新增的按钮 0暂无 1 新增  2 修改
      Physicshouse:[],//物理库房
      modal8: false,
      value: "",
      formValidate: {
        name: "", //库房名称
        RegionCode:"",//所属地区 地区简码
        CrmWarehouse:"",
        Ownertype:1,
        Address:"",
        //Ownerinpit:"", //所有人
        ResponsePerson:"",//所属业务员
        lease: "", //是否可添加库区
        //Personcharge: "", //负责人
        // warehousetype: '', //库房类型
        interest: [], //功能
        desc: "", //备注
        ID:"",//修改时需要传递ID
      },
      ruleValidate: {
        name: [{ required: true, message: "此项为必填", trigger: "blur" }],
        // Ownerinpit: [{ required: true, message: "此项为必填", trigger: "change" }],
        lease: [{ required: true, message: "此项为必填", trigger: "change" }],
        // Personcharge: [
        //   { required: true, message: "此项为必填", trigger: "change" }
        // ],
        RegionCode: [
          { required: true, message: "此项为必填", trigger: "change" }
        ],
        // CrmWarehouse:[
        //   {required:true,message:"此项为必填",trigger:"change"}
        // ],
        // Address:[{ required: true, message: "此项为必填", trigger: "blur" }],
        interest: [
          {
            required: true,
            type: "array",
            min: 1,
            message: "Choose at least one hobby",
            trigger: "change"
          }
        ],
         desc: [{ required: true, message: "此项为必填", trigger: "blur" }],
      },
      wearhoustitle: [
        {
          title: "库房名称",
          slot: "name",
          // fixed: 'left'
        },
        {
          title:"库房编号",
          key:"ID"
        },
        {
          title: "库区数量",
          key: "Count"
        },
        //{
        //  title: "所有人",
        //  key: "EnterpriseName"
        //},
        //{
        //  title: "负责人",
        //  key: "ManagerName"
        //},
        {
            title: "业务类型",
            key: "PurposeDes"
        },
        {
          title: "所在地址",
          key: "Address"
        },
        {
          title: "备注",
          key: "Summary"
        },
        {
          title: "操作",
          slot: "action",
          align: "center",
          // fixed: 'right'
          width:400
        }
      ],
      wearhousdata: [],
      edit_datas:{},
    };
  },
  methods: {
    query_btn(){
        // if(this.area_search.ID!=""||this.area_search.ManagerID!=""||this.area_search.Purpose!=""){
        //    this.area_search.Type=this.wearhousdata[0].Type
        //    this.GetWarehouse(this.area_search)
        // }else{
        //   this.$Message.warning('请输入查询条件');
        //   this.area_search.Type="";
        // }
          //  this.area_search.Type=this.wearhousdata[0].Type
           this.loading=true;
           this.GetWarehouse(this.area_search)
      },
    empty_btn(){
       
        this.area_search.Type="";
        this.area_search.ID="";
        this.area_search.ManagerID="";
        this.area_search.Purpose="";
        this.loading=true;
        this.GetWarehouse()
    },
      GetPurposes() { //用途
        GetPurposes("").then(res => {
          console.log(res)
          this.interestlist = res.obj;

        })
      },
    remoteMethod2(query){
       GetAdmins(query).then(res => {
         console.log(res)
          this.search_damin=res;
        })
    },
    showmodel(type,row) {
      if (type == 1) {
        this.modal8 = true;
        this.isbtns=1
      } else {
        this.modal8 = true;
        this.isbtns=2
        // console.log(row)
        this.edit_datas=row;
        this.formValidate={
          name:this.edit_datas.Name, //库房名称
          RegionCode:this.edit_datas.RegionCode,//所属地区
          CrmWarehouse:this.edit_datas.CrmWarehouse,//所属Crm库房
          Address:this.edit_datas.Address,//地址
          //Ownerinpit:this.edit_datas.OwnerID, //所有人
          ResponsePerson:this.edit_datas.ResponsePerson,//所属业务员
          lease:this.edit_datas.Addible.toString(), //是否可添加库区
          //Personcharge:this.edit_datas.ManagerID, //负责人
          warehousetype:this.edit_datas.Type.toString(), //库房类型this.edit_datas.Type,
          interest:this.edit_datas.AbilitiesDes2, //功能this.edit_datas.Abilities
          desc: this.edit_datas.Summary, //备注
          ID:this.edit_datas.ID
        };
      }
      this.$refs.formValidate2.GetEnterprise();
      this.$refs.formValidate2.GetAdmin("");
      this.$refs.formValidate2.GetCrmWarehouse();
    },
    show(index) {
      this.$router.push({
        //在本窗口打开
        path: "/area"
      });
    },
    goLogichouse(){
      this.$router.push({
        //在本窗口打开
        path: "/Logichouse"
      });
    },
    remove(row) { //删除库房按钮
      console.log(row);
    },
    ok(){  //新增库房确认按钮///修改按钮确定 
      this.$refs.formValidate2.handleSubmit();
      if (this.$refs.formValidate2.istrue == false) {
        this.modal8 = true;
      } else {
        this.modal8 = false;
        var datas={
          Type:1,
          FatherID:"",
          RegionCode:this.formValidate.RegionCode,//地区简码，
          CrmWarehouse:this.formValidate.CrmWarehouse,//所属Crm库房
          Name:this.formValidate.name,//库房名称,名称不能重复，
          Address:this.formValidate.Address,//库房地址，
          Abilities:this.formValidate.interest.toString(),//是否具备检测/报关/恒温/抽真空功能（例如 “1111” “1010” 这种形式），
          Addible:this.formValidate.lease, //是否可添加库区this.formValidate.lease，
          Summary:this.formValidate.desc, //备注，
          //OwnerID:this.formValidate.Ownerinpit, //所有人，
          ResponsePerson:this.formValidate.ResponsePerson,//业务员
          //ManagerID:this.formValidate.Personcharge, //负责人，管理人员，
          ID:this.formValidate.ID,//修改为必填项）
        };
         addhouse(datas).then(res=>{
           if(res.val==0){
             this.$Message.success(res.msg)
             this.getpfwms()
           }else{
             this.$Message.error(res.msg);
           };
           this.cancel();
           console.log(this.formValidate)
        })
      }
    },
    cancel() {  //关闭窗口
      this.$refs.formValidate2.handleReset();
      this.formValidate.ID=''
      this.modal8 = false;
    },
    setnva() {
      var cc = [
        {
          title: "库房配置",
          href: "/Storage"
        },
        {
          title: "库房",
          href: "/kufang"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    confirm() {
      this.$Modal.confirm({
        title: "删除",
        content: "<p>您确定要删除此库房吗？</p>",
        onOk: () => {
          this.$Message.info("删除成功");
        },
        onCancel: () => {
          this.$Message.info("取消删除");
        }
      });
    },
    // getpfwms() { //默认库房列表
    //   //调取api 的接口
    //   getpfwms().then(res => {
    //     // this.datas=res
    //     this.data6 = res.obj.Data;
    //     // console.log(res.obj);
    //   });
    // },
    GetWarehouse(Data){  //最新获取库房列表
       GetWarehouse(Data).then(res => {
        // this.datas=res
        this.wearhousdata = res.obj.Data;
        this.loading=false;
        // console.log(res.obj);
      });
    }
  },
  mounted() {
    //发起 获取服务器数据的 action
    // this.$store.dispatch("getCityData")
    this.setnva();//设置导航
    // this.getpfwms();//默认库房列表
    this.GetWarehouse()
    this.GetPurposes()
  }
};
</script>
<style scoped>
.buttonbox {
  margin-bottom: 20px;
  margin-top: 20px;
}
.Storage .ivu-btn > .ivu-icon {
  line-height: 1;
  vertical-align: middle;
  font-size: 16px;
}
.search_btn {
  margin-bottom: 20px;
}
.search_btn .ivu-input-wrapper {
  max-width: 12%;
  min-width: 170px;
  margin-right: 6px;
}
.function_li li {
  line-height: 30px;
}
.Storage .ivu-table:after {
  width: auto;
}
.searchtable{
    max-width: 12%;
    min-width: 170px;
    margin-right: 6px;
  }
</style>
