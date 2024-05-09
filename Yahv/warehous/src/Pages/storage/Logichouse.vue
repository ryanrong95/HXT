<template>
  <div class="areabox">
    <div class="srarch_input">
      <div class="srarch_input">
        <Input v-model.trim="ID" placeholder="门牌编号" class="searchtable" />
        <Select v-model="Purpose" filterable class="searchtable" placeholder="业务类型">
          <Option v-for="(item,index) in interestlist" :value="item.ID" :key="item.index">{{ item.purpose }}</Option>
        </Select>
        <Select 
          class="searchtable"
          placeholder="请输入并选择负责人"
          v-model="ManagerID"
          filterable
          remote
          :remote-method="remoteMethod2"
          :loading="loading1">
          <Option v-for="(option, index) in search_damin" :value="option.ID" :key="index">{{option.RealName}}</Option>
        </Select>
        <Button type="primary" icon="ios-search" @click="query_btn">查询</Button>
        <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
    </div>
    </div>
    <div>
      <Table :columns="columns12" :data="data6" :loading="loading">
        <template slot-scope="{ row }" slot="Name">
          <strong>{{ row.Name }}</strong>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" size="small" style="margin-right: 5px" :to="{name:'Areapage',params:{fatherID:row.ID}}">查看库区</Button>
          <!-- <Button type="error" size="small" @click="remove(index)">删除</Button> -->
        </template>
      </Table>
    </div>
    <div class="buttenbox">
      <div style="float:right">
        <Page :total="totals"  :page-size='6' :current="pageindex" @on-change="changepage" />
      </div>

    </div>
  </div>
</template>
<script>
  import formarea from "./Logichouse_form"
  import { GetWarehouse, GetPurposes,GetAdmins, } from "../../api"; //引入api 的接口
  export default {
    name: "Logichouse",
    components: {
      formarea: formarea
    },
    data() {
      
      return {
        loading: true,
        fatherID: this.$route.params.fatherID,
        pageindex: 1,
        ID:"",//编号
        Purpose:"",  //业务类型
        ManagerID:"",//负责人
        Type:"", //类型
        isshowadd: false,
        columns12: [
          {
            title: '门牌名称',
            slot: 'Name'
          },
          {
            title: '门牌编号',
            key: 'ID'
          },
          {
            title: "库区数量",
            key: "Count"
          },
          {
            title: '业务类型',
            key: 'PurposeDes'
          },
          {
            title: '地址',
            key: 'Address'
          },
          {
            title: "备注",
            key: "Summary"
          },
          {
            title: '操作',
            slot: 'action',
            width: 200,
            align: 'center'
          }
        ],
        data6: [],
        interestlist:[],
        search_damin:[],
        loading1:false,
        formValidate: {
          name: '',
          Ownertype: "",//所有人类型
          Owner: "",//所有人
          parents: "",//所属库房
          mail: '',
          city: '',
          gender: '',
          goods: "",
          interest: [],
          date: '',
          time: '',
          desc: ''
        },
        ruleValidate: {
          name: [
            { required: true, message: '此项为必填', trigger: 'blur' }
          ],
          Ownertype: [
            { required: true, message: '此项为必填', trigger: 'change' }
          ],
          Owner: [
            { required: true, message: '此项为必填', trigger: 'change' },
          ],
          parents: [
            { required: true, message: '此项为必填', trigger: 'change' },
          ],
          mail: [
            { required: true, message: '此项为必填', trigger: 'blur' },
          ],
          city: [
            { required: true, message: '此项为必填', trigger: 'change' }
          ],
          gender: [
            { required: true, message: '此项为必填', trigger: 'change' }
          ],
          goods: [
            { required: true, message: '此项为必填', trigger: 'change' }
          ],
          interest: [
            { required: true, type: 'array', min: 1, message: 'Choose at least one hobby', trigger: 'change' },
          ],
          desc: [
            { required: true, message: '此项为必填', trigger: 'blur' },
            { type: 'string', min: 20, message: 'Introduce no less than 20 words', trigger: 'blur' }
          ]
        },
        totals: 0,

      }
    },
    methods: {
      query_btn(){
            // if(this.ID!=""||this.ManagerID!=""||this.Purpose!=""){
            //   this.Type=this.data6[0].Type
            //    var data = {
            //             Key: this.fatherID,
            //             PageIndex: 1,
            //             PageSize: 6,
            //             ID:this.ID,//编号
            //             Purpose:this.Purpose,  //业务类型
            //             ManagerID:this.ManagerID,//负责人
            //             Type: this.Type, //类型
            //       }
            //   this.GetWarehouse(data)
            // }else{
            //   this.$Message.warning('请输入查询条件');
            //   this.Type="";
            // }
              //  this.Type=this.data6[0].Type
               var data = {
                        Key: this.fatherID,
                        PageIndex: 1,
                        PageSize: 6,
                        ID:this.ID,//编号
                        Purpose:this.Purpose,  //业务类型
                        ManagerID:this.ManagerID,//负责人
                        // Type: this.Type, //类型
                  }
          this.loading=true;
          this.GetWarehouse(data)
          },
      empty_btn(){
          
            var data = {
                        Key: this.fatherID,
                        PageIndex: 1,
                        PageSize: 6,
                        ID:this.ID,//编号
                        Purpose:this.Purpose,  //业务类型
                        ManagerID:this.ManagerID,//负责人
                        // Type: this.Type, //类型
              }
           this.pageindex=1;
           this.Purpose="";
           this.ManagerID="";
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
      showmodel() {
        this.isshowadd = true
      },
      ok() {
        this.$refs.formValidate2.handleSubmit();
        if (this.$refs.formValidate2.istrue == false) {
          this.isshowadd = true;
        } else {
          this.isshowadd = false;
          console.log(this.formValidate)
        }
      },
      cancel() {
        this.$refs.formValidate2.handleReset();
        this.isshowadd = false;
      },
      show(index) {
        this.$router.push({  //在本窗口打开
          name: 'shelves',
        })
      },
      remove(index) {
        // this.data6.splice(index, 1);
      },
      setnva() {
        var cc = [
          //    {
          //         title:"配置",
          //         href:"/"
          //     },
          //     {
          //         title:"全部配置",
          //         href:"/"
          //     },
          {
            title: "库房配置",
            href: "/kufang"
          },
          {
            title: "门牌库房",
            href: ""
          }
        ]
        this.$store.dispatch("setnvadata", cc)
      },
      GetWarehouse(data) {  //最新获取库房列表
        GetWarehouse(data).then(res => {
          this.data6 = res.obj.Data;
          this.totals = res.obj.Total;
          this.loading = false;
          // this.datas=res
          // this.wearhousdata = res.obj.Data;
          // console.log(res.obj);
        });
      },
      changepage(value) {
        this.loading=true;
        this.pageindex = value;
        var data = {
          Key: this.fatherID,
          PageIndex: this.pageindex,
          PageSize: 6,
          ID:this.ID,//编号
          Purpose:this.Purpose,  //业务类型
          ManagerID:this.ManagerID,//负责人
          Type: this.Type, //类型
        }
        this.GetWarehouse(data)
      }
    },
    mounted() {
      //发起 获取服务器数据的 action
      // this.$store.dispatch("getCityData")
      this.setnva()
      console.log(this.$route.params.fatherID)
      var data = {
        Key: this.fatherID,
        PageIndex: this.pageindex,
        PageSize: 6,
        ID:this.ID,//编号
        Purpose:this.Purpose,  //业务类型
        ManagerID:this.ManagerID,//负责人
        Type: this.Type, //类型
      }
      this.GetWarehouse(data)
      this.GetPurposes()
    }
  }
</script>
<style scoped>
  .buttenbox {
    margin-top: 20px;
    margin-bottom: 20px;
  }

  .srarch_input {
    margin-bottom: 30px;
  }

  .ivu-input-wrapper {
    max-width: 12%;
    min-width: 170px;
    margin-right: 6px;
  }

  .ivu-btn > .ivu-icon {
    line-height: 1;
    vertical-align: middle;
    font-size: 20px;
  }

  .ivu-btn {
    margin-right: 6px;
  }
.searchtable{
    max-width: 12%;
    min-width: 170px;
    margin-right: 6px;
  }
</style>
