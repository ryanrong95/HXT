
<template>
  <div class="areabox">
    <div class="srarch_input">
      <Input v-model.trim="query_field.ID" placeholder="编号"  class="searchtable" />
      <Select v-model="query_field.Purpose" filterable class="searchtable" placeholder="业务类型">
        <Option v-for="(item,index) in interestlist" :value="item.ID" :key="item.index">{{ item.purpose }}</Option>
      </Select>
      <Select 
        class="searchtable"
        placeholder="请输入并选择负责人"
        v-model="query_field.ManagerID"
        filterable
        remote
        :remote-method="remoteMethod2"
        :loading="loading1">
        <Option v-for="(option, index) in search_damin" :value="option.ID" :key="index">{{option.RealName}}</Option>
      </Select>
      <Button type="primary" icon="ios-search" @click="query_btn">查询</Button>
      <Button type="error" icon="ios-trash" @click="empty_btn">清空</Button>
      <Button icon="md-add" @click="showmodel">新增货架</Button>
    </div>
    <div>
      <Table :columns="Shelves_title" :data="Shelves_data" :loading="loading">
        <template slot-scope="{ row }" slot="Name">
          <strong>{{ row.Name }}</strong>
        </template>
        <template slot-scope="{ row }" slot="FatherID">
          <strong>{{ row.FatherID}}</strong>
        </template>
        <template slot-scope="{ row, index }" slot="action">
          <Button type="primary" size="small" style="margin-right: 5px" :to="{name:'location',params:{locationID:row.ID}}">查看库位</Button>
          <Button type="error" size="small" @click="remove(row.ID)">删除</Button>
        </template>
      </Table>
    </div>
    <div class="buttenbox">
      <div style="float:left">
        <Modal title="新增货架"
               v-model="isshowadd"
               @on-visible-change="cancel_btn"
               :mask-closable="false">
          <div>
            <label><em class="bitian">*</em>货架名称：</label>
            <Input v-model="formValidate.name" placeholder="请输入两位数字,如'01',货架名称不能重复'" class="frominpit" @on-blur="testname"/>
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
            <Input v-model="formValidate.desc" type="textarea" :autosize="{minRows: 2,maxRows: 5}" placeholder="" class="frominpit" />
          </div>
          <div slot="footer">
            <Button @click="cancel_btn(false)">取消</Button>
            <Button type="primary" @click="ok">确定</Button>
          </div>
        </Modal>
        <!-- <Button type="error" icon="md-trash">批量删除</Button> -->
      </div>
      <div style="float:right">
        <Page :total="Total" :page-size="6" @on-change="changepage" />
      </div>

    </div>
  </div>
</template>
<script>
  import formarea from "./formshelves";
  import { Shelvelist, GetPurposes, AddShelve,GetAdmins,ShelveDelete} from "../../api/index";
  export default {
    name: "shelves",
    components: {
      formarea: formarea
    },
    beforeRouteEnter (to, from, next){
     next(vm => {
       // 通过 `vm` 访问组件实例,将值传入oldUrl
       vm.oldUrl = from.path
     })
   },
    data() {
      return {
        search_damin:[],//搜索所有人数组
        area_search: {
          Name: "",
          Type:"",
          ManagerID:""
        },
        loading1:false,  //远程搜索loading
        adminsarr:[],//负责人数组
        oldUrl:"",
        loading:true,
        interestlist: "",
        formdata: {
          interest: "",
        },
        oldurl: this.$route.fullPath,
        isshowadd: false,
        formValidate: {
          name: '',
          //Ownertype:"",//所有人类型
          //Owner:"",//所有人
          //parents:[],//所属库房
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
          //Ownertype:[
          //    { required: true, message: '此项为必填', trigger: 'change' }
          //],
          //Owner: [
          //    { required: true, message: '此项为必填', trigger: 'change' },
          //],
          //parents: [
          //    // { required: true, message: '此项为必填', trigger: 'blur' }
          //    { required: true, type: 'array', min: 1, message: '请选择库位位置', trigger: 'change' },
          //],
          //mail: [
          //    { required: true, message: '此项为必填', trigger: 'blur' },
          //],
          //city: [
          //    { required: true, message: '此项为必填', trigger: 'change' }
          //],
          //gender: [
          //    { required: true, message: '此项为必填', trigger: 'change' }
          //],
          //goods: [
          //    { required: true, message: '此项为必填', trigger: 'change' }
          //],
          interest: [
            { required: true, type: 'array', min: 1, message: 'Choose at least one hobby', trigger: 'change' },
          ],
          desc: [
            { required: true, message: '此项为必填', trigger: 'blur' },
            { type: 'string', min: 20, message: 'Introduce no less than 20 words', trigger: 'blur' }
          ]
        },
        query_field: {
          key: this.$route.params.AreafatherID,//父亲ID（必填）
          PageIndex: "1",//当前页码
          PageSize: "6",//每页记录数）
          ID:"",//编号
          Purpose:"",  //业务类型
          ManagerID:"",//负责人
          Type:"", //类型
        },
        searchtype:"",
        Shelves_title: [
          {
            type: 'selection',
            width: 60,
            align: 'center'
          },
          {
            title: '货架名称',
            key: 'Name'
          },
          {
            title:"编号",
            key:"ID"
          },
          {
            title: '库位数量',
            key: 'Count'
          },
          {
            title: '所属位置',
            key: 'FatherName'
          },
          {
            title: '业务类型',
            key: 'PurposeDes'
          },
          {
            title: "备注",
            key: "Summary"
          },
          // {
          //     title: '所有人',
          //     key: ''
          // },
          // {
          //     title: '负责人',
          //     key: 'ManagerID'
          // },
          {
            title: '操作',
            slot: 'action',
            width: 200,
            align: 'center'
          }
        ],
        Shelves_data: [],
        Total: 0,
      }
    },
    methods: {
      getCookie(cookieName) {
            var strCookie = document.cookie;
            var arrCookie = strCookie.split("; ");
            for(var i = 0; i < arrCookie.length; i++){
                var arr = arrCookie[i].split("=");
                if(cookieName == arr[0]){
                    return arr[1];
                }
            }
            return "";
        },
      GetPurposes() {
        GetPurposes("").then(res => {
          console.log(res)
          this.interestlist = res.obj
        })
      },
      showmodel() {
        this.isshowadd = true
      },
      ok() {
        if(this.formValidate.name!=""&&this.formdata.interest!=""&&this.formValidate.desc!=""&&this.formValidate.admins){
          var datas = {
            Name: this.formValidate.name,//货架名称,名称不能重复，
            FatherID: this.$route.params.AreafatherID,//如何取值
            Type: 30,
            Purpose: this.formdata.interest.toString(),//单选值如何选值
            Summary: this.formValidate.desc,
            ManagerID:this.formValidate.admins,
          };
          console.log(datas);
          AddShelve(datas).then(res => {
            if (res.val == 0) {
              this.$Message.success(res.msg)
              this.isshowadd = false;
              this.Shelvelist(this.query_field); //库区初始化
              this.GetPurposes();//获取用途
              this.formValidate.name="";  //初始化input
              this.formdata.interest="";
              this.formValidate.desc="";
              this.formValidate.admins="";
            } else {
              this.$Message.error(res.msg);
            };
            //this.cancel();
            console.log(this.formValidate);
          })

        }else{
            this.$Message.error("请输入信息");
        }
         
      },
      cancel() {
        this.$refs.formValidate2.handleReset();
        this.isshowadd = false;
      },
      show(index) {
        this.$router.push({  //在本窗口打开
          path: '/Location',
        })
      },
      remove(index) {
        var ID=index;
         this.$Modal.confirm({
                title: '提示',
                content: '是否要删除该货架',
                onOk: () => {
                    ShelveDelete(ID).then(res=>{
                      console.log(res)
                      if(res.val==0){
                        this.$Message.success("删除成功");
                        this.Shelvelist(this.query_field);
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
      setnva(oldurls) {
        
        var cc = [
          {
            title: "库房配置",
            href: "/kufang"
          },
          {
            title: "库区配置",
            href:this.getCookie("kuquurl"),
          },
          {
            title: "货架配置",
            href: this.$route.path
          }
        ]
        this.$store.dispatch("setnvadata", cc)
      },
      Shelvelist(data) {  //货架列表初始化
        Shelvelist(data).then(res => {
          this.Total = res.obj.Total;
          if(this.Total==0){
           this.Shelves_data=[];
          }else{
            this.Shelves_data = res.obj.Data;
          }
           this.loading=false;
        })
      },
      changepage(value) { //分页点击
        this.loading=true;
        this.query_field.PageIndex = value;
        this.Shelvelist(this.query_field)
      },
      testname(){
        // this.formValidate.name
        var testdata=/^[0-9]{2}$/;
        var Result = testdata.test(this.formValidate.name);
        console.log(Result)
        if(Result==false){
             this.$Message.error("只能输入两位数字,如'01'");
             this.formValidate.name='';
        }
      },
     cancel_btn(value){ //取消按钮
      console.log(value)
      if(value==false){
          this.isshowadd = false;
          this.formValidate.name="";  //初始化input
          this.formdata.interest="";
          this.formValidate.desc="";
          this.formValidate.admins="";
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
       empty_btn() {  //清空按钮
        //  this.query_field.PageIndex=1;
        this.loading=true;
         this.query_field={
              key: this.$route.params.AreafatherID,//父亲ID（必填）
              PageIndex: "1",//当前页码
              PageSize: "6",//每页记录数）
              ID:"",//编号
              Purpose:"",  //业务类型
              ManagerID:"",//负责人
              Type:""
            },
        this.Shelvelist(this.query_field)
      },
      query_btn() {  //库区查询
        this.loading=true;
        this.query_field.PageIndex=1;
        this.query_field.Type= this.searchtype;
        this.Shelvelist(this.query_field)
      },
    },
    mounted() {
        this.$nextTick(()=>{
       // 验证是否获取到了上页的url
       /* eslint-disable no-console */
       console.log(this.oldUrl)
        this.setnva(this.oldUrl)
     })
      this.Shelvelist(this.query_field);
      if(this.Shelves_data.length>0){
        this.searchtype=this.Shelves_data[0].Type
        }else{
          this.searchtype="";
        }
      this.GetPurposes()//获取用途
      // console.log(this.$route.path)
      document.cookie="huojiaurl="+this.$route.path;
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
.frominpit{
  width:80%;
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
