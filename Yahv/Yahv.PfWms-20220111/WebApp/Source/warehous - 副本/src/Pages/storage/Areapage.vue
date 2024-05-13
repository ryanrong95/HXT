
<template>
  <div class="areabox">
    <div class="srarch_input">
      <Input v-model.trim="listdata.ID" placeholder="库区编号"  class="searchtable" />
      <Select v-model="listdata.Purpose" filterable class="searchtable" placeholder="业务类型">
        <Option v-for="(item,index) in interestlist" :value="item.ID" :key="item.index">{{ item.purpose }}</Option>
      </Select>
      <Select 
        class="searchtable"
        placeholder="请输入并选择负责人"
        v-model="listdata.ManagerID"
        filterable
        remote
        :remote-method="remoteMethod2"
        :loading="loading1">
        <Option v-for="(option, index) in search_damin" :value="option.ID" :key="index">{{option.RealName}}</Option>
      </Select>
      <Button type="primary" icon="ios-search" @click="query_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
      <Button icon="md-add" @click="showmodel">新增库区</Button>
    </div>
    <div>
      <Table :columns="columns12" :data="getlistdata" :loading='loading' >
        <template slot-scope="{ row }" slot="Name">
          <strong>{{ row.Name }}</strong>
        </template>
        <template slot-scope="{ row }" slot="FatherMsg">
          <strong>{{ row.FatherMsg.Name }}</strong>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" size="small" style="margin-right: 5px" :to="{name:'shelves',params:{AreafatherID:row.ID}}">查看货架</Button>
          <Button type="error" size="small" @click="remove(row.ID)">删除</Button>
        </template>
      </Table>
    </div>
    <div class="buttenbox">
      <div style="float:left">
        <Modal title="新增库区"
               v-model="isshowadd"
               @on-visible-change="cancel_btn"
               :mask-closable="false">
          <div>
            <label><em class="bitian">*</em> 库区名称：</label>
            <Input v-model="formValidate.name" placeholder="只能输入一位大写英文字母作为库区名称" style="width:80%" @on-blur="testname"/>
          </div>
          <br />
          <div>
            <label for="">
               <em class="bitian">*</em>负&nbsp;&nbsp;责&nbsp;&nbsp;人：
               <Select
                style="width:80%"
                placeholder="请输入并选择负责人"
                v-model="formValidate.admins"
                filterable
                remote
                :remote-method="remoteMethod1"
                :loading="loading1">
                <Option v-for="(option, index) in adminsarr" :value="option.ID" :key="index">{{option.RealName}}</Option>
             </Select>
            </label>
          </div>
          <br/>
          <div>
            <label><em class="bitian">*</em>用　　途：</label>
            <RadioGroup v-model="formdata.interest">
              <Radio v-for="item in interestlist" :label="item.ID" :key="item.ID">{{item.purpose}}</Radio>
            </RadioGroup>
          </div>
          <br />
          <div>
            <label><em class="bitian">*</em>备　　注：</label>
            <Input v-model="formValidate.desc" type="textarea" :autosize="{minRows: 2,maxRows: 5}" placeholder="请输入备注" style="width:80%"/>
          </div>
          <div slot="footer">
            <Button @click="cancel_btn(false)">取消</Button>
            <Button type="primary" @click="ok">确定</Button>
          </div>
        </Modal>
        <!-- <Button type="error" icon="md-trash">批量删除</Button> -->
      </div>
      <div style="float:right">
        <Page :total=total  :current="listdata.PageIndex" @on-change="changepage" :page-size="6" />
      </div>

    </div>
    <!-- <div class="buttenbox">
        <div style="float:left">
            <Modal
                title="新增库区"
                v-model="isshowadd"
                :mask-closable="false">
                <formarea ref="formValidate2" :formValidate="formValidate" :ruleValidate="ruleValidate"></formarea>
                <div slot="footer">
                        <Button @click="cancel">取消</Button>
                        <Button type="primary" @click="ok">确定</Button>
                </div>
            </Modal>
        </div>
        <div style="float:right">
           <Page :total=total show-elevator :current="listdata.PageIndex" @on-change="changepage"/>
        </div>

    </div> -->
  </div>
</template>
<script>
  import formarea from "./formarea"
  import { Shelvelist, GetPurposes, AddShelve ,GetAdmins,ShelveDelete} from "../../api/index"
  export default {
    name: "Areapage",
    components: {
      formarea: formarea
    },
    data() {
      return {
        loading1:false,  //远程搜索loading
        adminsarr:[],//负责人数组,
        
        loading:true,
        interestlist: "",
        formdata: {
          interest: "",
        },
        total: 0,//数据总条数
        listdata: {
          Key: this.$route.params.fatherID,//：父亲ID（必填）
          PageIndex: 1,//：当前页码
          PageSize: "6",//：每页记录数
          ID:"",//编号
          Purpose:"",  //业务类型
          ManagerID:"",//负责人
          Type:"", //类型
        },
        houselist: [
          {
            Name: "1111",
            Value: "1111111"
          },
        ],//库房列表
        isshowadd: false,
        columns12: [
          {
            type: 'selection',
            width: 60,
            align: 'center'
          },
          {
            title: '库区名称',
            slot: 'Name'
          },
          {
            title:"编号",
            key:"ID",
          },
            {
            title: '货架数量',
            key: 'Count'
          },
          {
            title: '业务类型',
            key: 'PurposeDes'
          },
          {
            title: '所属位置',
            key: 'FatherName'
          },
          {
            title: "备注",
            key: "Summary"
          },
          // {
          //     title: '所有人',
          //     key: 'Subordinates'
          // },
          //{
          //  title: '负责人',
          //  key: 'ManagerID'
          //},
          // {
          //     title: '货架数量',
          //     key: 'age'
          // },
          // {
          //     title: '用途',
          //     key: 'function',
          // },
          {
            title: '操作',
            slot: 'action',
            width: 200,
            align: 'center'
          }
        ],
        getlistdata: [],
        search_damin:[],//搜索所有人数组
        area_search: {
          Name: "",
          Type:"",
          ManagerID:""
        },
        formValidate: {
          name: '',
          //Ownertype:"",//所有人类型
          //Owner:"",//所有人
          //parents:"",//所属库房
          //mail: '',
          //city: '',
          //gender: '',
          //goods:"",
          interest: "",
          //date: '',
          //time: '',
          desc: '',
          admins:"",//负责人
        },
        ruleValidate: {
          name: [
            { required: true, message: '此项为必填', trigger: 'blur' }
          ],
          interest: [
            { required: true, type: 'array', min: 1, message: 'Choose at least one hobby', trigger: 'change' },
          ],
          desc: [
            { required: true, message: '此项为必填', trigger: 'blur' },
            { type: 'string', min: 20, message: 'Introduce no less than 20 words', trigger: 'blur' }
          ]
        },

      }
    },
     beforeRouteEnter (to, from, next){
     next(vm => {
       // 通过 `vm` 访问组件实例,将值传入oldUrl
       vm.oldUrl = from.path
     })
   },
   mounted() {
      //发起 获取服务器数据的 action
      // this.$store.dispatch("getCityData")
      this.$nextTick(()=>{  //调用钩子，获取到上一级的url
        // this.setnva(this.oldUrl)
        console.log(this.oldUrl)
     })
      this.setnva()
      this.Shelvelist() //库区初始化
      this.GetPurposes()//获取用途
      //   this.GetWarehouse()
      // console.log(this.$route.path)
      document.cookie="kuquurl="+this.$route.path;

    },
    methods: {
      GetPurposes() { //用途
        GetPurposes("").then(res => {
          console.log(res)
          this.interestlist = res.obj;

        })
      },
      showmodel() {
        this.isshowadd = true
      },
      ok() {
        //this.$refs.formValidate2.handleSubmit();
        //if (this.$refs.formValidate2.istrue == false) {
        //  this.isshowadd = true;
        //} else {
        //  this.isshowadd = false;
      if(this.formValidate.name!=""&&this.formValidate.desc!=""&&this.formdata.interest!=""&&this.formValidate.admins!=""){
         var datas = {
            Name: this.formValidate.name,//库区名称,名称不能重复，
            FatherID: this.$route.params.fatherID,//如何取值
            Type: 20,
            ManagerID:this.formValidate.admins,
            Purpose: this.formdata.interest.toString(),//单选值如何选值
            Summary: this.formValidate.desc
          };

          console.log(datas);
          AddShelve(datas).then(res => {
            if (res.val == 0) {
              this.$Message.success(res.msg)
              this.isshowadd = false;
              this.Shelvelist(); //库区初始化
              this.GetPurposes();//获取用途
              this.formValidate.name=""  //初始化input
              this.formdata.interest=""
              this.formValidate.desc="",
              this.formValidate.admins=""
            } else {
              this.$Message.error(res.msg);
            };
            //this.cancel();
            console.log(this.formValidate);
          })
      }else{
        this.$Message.error('请输入信息');
      }
         
        //}
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
        var ID=index;
         this.$Modal.confirm({
                title: '提示',
                content: '是否要删除该库区',
                onOk: () => {
                    ShelveDelete(ID).then(res=>{
                      console.log(res)
                      if(res.val==0){
                        this.$Message.success("删除成功");
                        this.Shelvelist()
                      }else{
                        this.$Message.error("删除失败");
                      }
                    })
                },
                onCancel: () => {
                    this.$Message.info('取消删除');
                }
             });
      },
      setnva() {
        var cc = [
          {
            title: "库房配置",
            href: "/kufang"
          },
          {
            title: "库区配置",
            href: this.$route.path
          }
        ]
        this.$store.dispatch("setnvadata", cc)
      },
      Shelvelist() {  //库区初始化
        Shelvelist(this.listdata).then(res => {
          this.getlistdata = res.obj.Data;
          this.total = res.obj.Total;
          this.loading=false;
        })
      },
      query_btn() {  //库区查询
        this.loading=true;
        this.listdata.FatherID = this.area_search.parentname;
        this.listdata.PageIndex = 1;
        this.listdata.PageSize=6;
        console.log(this.listdata)
        this.Shelvelist(this.listdata)
      },
      empty_btn() {  //清空按钮
          // this.listdata.PageIndex = "",
          // this.listdata.ManagerID = "",
          // this.listdata.PageIndex = 1;
          this.listdata={
              Key: this.$route.params.fatherID,//：父亲ID（必填）
              PageIndex: 1,//：当前页码
              PageSize: "6",//：每页记录数
              ID:"",//编号
              Purpose:"",  //业务类型
              ManagerID:"",//负责人
              Type:"", //类型
        },
          this.loading=true;
          this.Shelvelist(this.listdata)
      },
      GetWarehouse() { //获取库房
        GetWarehouse("").then(res => {
          // console.log(res)
          this.houselist = res.obj
        })
      },
      changepage(index) {  //点击分页请求数据
        this.loading=true;
        this.listdata.PageIndex = index;
        this.Shelvelist()
      },
      testname(){
        // var tests= /(^[A-Z]{1,1})/
      var tests=/^[A-Z]1*$/
          var Result = tests.test(this.formValidate.name)
         if(Result==false){
            this.formValidate.name='';
            this.$Message.error("只能输入一位大写英文字母");
         }else{

         }
      },
      cancel_btn(value){ //取消按钮
      console.log(value)
      if(value==false){
          this.isshowadd = false;
          this.formValidate.name=""  //初始化input
          this.formdata.interest=""
          this.formValidate.admins=""
          this.formValidate.desc=""
         }
         
      },
      remoteMethod1(query){
       GetAdmins(query).then(res => {
         console.log(res)
          this.adminsarr=res;
        })
       },
       remoteMethod2(query){
       GetAdmins(query).then(res => {
         console.log(res)
          this.search_damin=res;
        })
       },
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

 .searchtable{
    max-width: 12%;
    min-width: 170px;
    margin-right: 6px;
  }

.ivu-modal-body .frominput{
  width: 80%;
}
  .ivu-btn > .ivu-icon {
    line-height: 1;
    vertical-align: middle;
    font-size: 20px;
  }

  .ivu-btn {
    margin-right: 6px;
  }
  .bitian{
    color: red;
  }
</style>
