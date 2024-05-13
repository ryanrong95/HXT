<template>
  <div>
     <div class="srarch_input">
      <Input v-model.trim="query_field.ID" @on-enter="query_btn" placeholder="编号"   class="searchtable" />
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
        <Button type="error" icon="ios-trash" @click="handleReset">清空</Button>
        <Button icon="md-add" @click="showmodel">新增库位</Button>
        <Button icon="ios-print-outline" @click="showprintbox">打印库位标签</Button>
      </div>
    <div>
      <Modal title="新增库位"
             v-model="isshowadd"
             @on-visible-change="clarn_btn"
             :mask-closable="false">
        <div>
          <label><em class="bitian">*</em> 库位名称：</label>
          <Input v-model="formdata.name" placeholder="只能输入两位数字,如'01'" class="frominput" @on-blur="testname"/>
        </div>
        <br />
        <div>
          <label><em class="bitian">*</em>所属层数：</label>
          <!-- <Input v-model="formdata.shelves" placeholder="" class="frominput"/> -->
          <Select v-model="formdata.shelves" class="frominput">
            <Option v-for="(item,index) in shelvesarr" :value="item" :key="index">{{ item }}</Option>
          </Select>
        </div>
        <br />
        <div>
            <label for="">
               <em class="bitian">*</em>负&nbsp;&nbsp;责&nbsp;&nbsp;人：
               <Select
                style="width:80%"
                placeholder="请输入并选择负责人"
                v-model="formdata.admins"
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
          <RadioGroup v-model="formdata.interest" class="frominput">
            <Radio v-for="item in interestlist" :label="item.ID" :key="item.ID">{{item.purpose}}</Radio>
          </RadioGroup>
        </div>
        <br />
        <div>
          <label><em class="bitian">*</em>所属规格：</label>
          <Select v-model="formdata.specid" class="frominput">
            <Option v-for="item in speclist" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
          </Select>
        </div>
        <br/>
        <div>
          <label><em class="bitian">*</em>备　　注：</label>
          <Input v-model="formdata.desc" type="textarea" :autosize="{minRows: 2,maxRows: 5}" placeholder="请输入备注信息"  class="frominput"/>
        </div>
        <div slot="footer">
          <Button @click="clarn_btn(false)">取消</Button>
          <Button type="primary" @click="ok">确定</Button>
        </div>
      </Modal>
      <Modal v-model="printlabel"
             @on-ok="ok"
             @on-cancel="cancel"
             :mask-closable="false"
             :closable='false'>
        <p slot="header" style="font-size:16px;font-weight: bold;">
          <span style="float:left">库位标签打印</span>
          <Icon type="md-close" style="float:right" @click="printNo" />
        </p>
        <div>
          <h1 class="print_title">标签示例图</h1>
          <div class="print_box">
            <div style="margin:5px 20px">
              <h2>香港库房</h2>
              <P style="padding-top:5px;">
                <span>库区：A </span>
                <span>货架：01 </span>
                <span>位号：0101 </span>
                </P>
                <div style="text-align:center">
                  <barcode :value="print_data.value" :options="barcode_option" tag="svg"></barcode>
                </div>
              <div style="font-size:12px;">
                <p>租用人：代仓储会员测试有限公司</p>
                <!-- <p>负责人：张三</p>
                <p>所有人：李四四四</p> -->
              </div>
              
            </div>

          </div>
        </div>
        <div slot="footer">
          <Button @click="printNo">取消</Button>
          <Button @click="printOk" type="primary">确定</Button>
        </div>
      </Modal>
    </div>
    <div style="clear:both;padding-top:20px;">
      <!-- <Table  :columns="columns1" :data="data1"></Table>
      <Table class="table2" :show-header="false" :columns="columns1" :data="data1"></Table> -->
      <div class="">
        <div class="table">
          <Table 
          :columns="listtitle" 
          :data="listdata" 
          :loading="loading" 
          @on-selection-change="handleSelectRow">
            <!-- <template slot-scope="{ row }" slot="tags">
              <Tag :color="row.tags"></Tag> 
            </template>  -->
            <template slot-scope="{ row, index }" slot="action">
              <Button type="error" size="small" @click="remove(row.ID)">删除</Button>
            </template>
          </Table>
          <div style="text-align: right;padding-top: 10px;">
            <Page :total="total" :page-size="6" :current="query_field.PageIndex" @on-change="changepages"/>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
  import formlocation from "./formlocation"
  import { Shelvelist, GetPurposes, GetSpecs, AddShelve ,GetAdmins,ShelveDelete} from "../../api/index"
  import { GetPrinterDictionary, TemplatePrint } from "@/js/browser.js"
  import VueBarcode from '@xkeshi/vue-barcode'; //导入条形码插件
  let product_url=require("../../../static/pubilc.dev")
  export default {
    components: {
      formlocation: formlocation,
      barcode: VueBarcode
    },
    name: "Location",
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
        SelectRow:[],
        oldUrl:"",
        loading:true,
        total:0,
        printurl:product_url.pfwms,
        interestlist: "",
        formdata: {
          name:"",
          shelves:"",
          interest: "",
          specid: "",
          desc:"",
          admins:"",//负责人
        },
        shelvesarr:["01","02","03","04","05","06","07","08","09","10","11","12","13","14","15","16","17","18","19","20"],
        speclist: "",
        arrys: [         
          {
              name: 'PO',
              value: 'D1P74F244SCX-QR-668-01',
            },
            {
              name: 'Cust.#',
              value: 'AD620ARZ-01',
            },
            {
              name: 'Mfr.#',
              value: 'Ver3333333',
            },
            {
              name: 'QTY',
              value: '450000000',
            },
            {
              name: 'DC',
              value: '201716',
            },
            {
              name: 'Origin',
              value: 'MY',
            },
            {
              name: 'Origin',
              value: '047972437',
            },
        ],
        barcode_option: {

          displayValue: true, //是否默认显示条形码数据
          //textPosition  :'top', //条形码数据显示的位置
          background: '#fff', //条形码背景颜色
          valid: function (valid) { },
          width: '1.7px', //单个条形码的宽度
          height: '40px',
          fontSize: '16px', //字体大小
          format: "CODE128",//选择要使用的条形码类型
          margin: 2,
        },
        printlabel: false,
        isshowadd: false,
        Code: "",
        value1: [],
        data: [
          {
            value: 'beijing',
            label: '北京库房',
            children: [
              {
                value: 'gugong',
                label: 'A库区'
              },
              {
                value: 'tiantan',
                label: 'B库区'
              },
              {
                value: 'wangfujing',
                label: 'C库区'
              }
            ]
          },
          {
            value: 'jiangsu',
            label: '深圳库房',
            children: [
              {
                value: 'nanjing',
                label: '02A',
                children: [
                  {
                    value: 'fuzimiao',
                    label: 'HK02-T01',
                  }
                ]
              },
              {
                value: 'suzhou',
                label: '05B',
                children: [
                  {
                    value: 'zhuozhengyuan',
                    label: 'HK02-T02',
                  },
                  {
                    value: 'shizilin',
                    label: 'HK02-T03',
                  }
                ]
              }
            ],
          }
        ],
        columns1: [
          {
            title: ' ',
            slot: 'tags',
            width: 80,
            align: 'center'
          },
          {
            title: '库位编号',
            key: 'name'
          },
          {
            title: '所属库房',
            key: 'age'
          },
          {
            title: '级别',
            key: 'address'
          },
          {
            title: '状态',
            slot: 'status',
          }
        ],
        data1: [
          {
            name: 'BJ01-A05-0308',
            age: "北京库房",
            address: '一级',
            status: '001',
            tags: "#ac0",
          },
          {
            name: 'BJ01-A15-0308',
            age: "北京库房",
            address: '二级',
            status: '002',
            tags: "#ac0",
          },
          {
            name: 'BJ01-B06-0308',
            age: "北京库房",
            address: '三级',
            status: '003',
            tags: "#FFA2D3",
          },
          {
            name: 'BJ01-C05-0308',
            age: "北京库房",
            address: '五级',
            status: '001',
            tags: "#FFA2D3",
          }
        ],
        formValidate: {
          number: "1",//添加库位的个数
          Owner: '',//所有人
          Ownertype: "",//所有人类型
          Personcharge: '',//负责人
          isleas: false,//是否可租赁
          lease: 'yes',//是否租赁
          goods: "",
          interest: [],
          date: '',
          time: '',
          desc: '',
          value1: [], //库位所属区域
          data: [
            {
              value: 'beijing',
              label: '北京库房',
              children: [
                {
                  value: 'gugong',
                  label: 'A库区'
                },
                {
                  value: 'tiantan',
                  label: 'B库区'
                },
                {
                  value: 'wangfujing',
                  label: 'C库区'
                }
              ]
            },
            {
              value: 'jiangsu',
              label: '深圳库房',
              children: [
                {
                  value: 'nanjing',
                  label: '02A',
                  children: [
                    {
                      value: 'fuzimiao',
                      label: 'HK02-T01',
                    }
                  ]
                },
                {
                  value: 'suzhou',
                  label: '05B',
                  children: [
                    {
                      value: 'zhuozhengyuan',
                      label: 'HK02-T02',
                      children: [
                        {
                          value: 'zhuozhengyuan',
                          label: 'HK02-T022222222',
                        },
                        {
                          value: 'shizilin',
                          label: 'HK02-T03',
                        }
                      ]
                    },
                    {
                      value: 'shizilin',
                      label: 'HK02-T03',
                    }
                  ]
                }
              ],
            }
          ],
        },
        ruleValidate: {
          value1: [
            // { required: true, message: '此项为必填', trigger: 'blur' }
            { required: true, type: 'array', min: 1, message: '请选择库位位置', trigger: 'change' },
          ],
          Owner: [
            { required: true, message: '此项为必填', trigger: 'change' },
          ],
          Personcharge: [
            { required: true, message: '此项为必填', trigger: 'blur' }
          ],
          // lease: [
          //     { required: true, message: '此项为必填', trigger: 'change' }
          // ],
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
        query_field: {
          key: this.$route.params.locationID,//父亲ID（必填）
          PageIndex: 1,//当前页码
          PageSize: 6,//每页记录数）
          ID:"",//编号
          Purpose:"",  //业务类型
          ManagerID:"",//负责人
          Type:50
        },
        print_data: {
          name: 'PO',
          value: 'A2012601',
        },
        listtitle: [
          {
            type: 'selection',
            width: 60,
            align: 'center'
          },
          // {
          //   title: ' ',
          //   slot: 'tags',
          //   width: 80,
          //   align: 'center'
          // },
          {
            title: '库位名称',
            key: 'Name'
          },
          {
            title: '库位编号',
            key: 'ID'
          },
          {
            title: '业务类型',
            key: 'PurposeDes'
          },
          {
            title: '规格',
            key: 'SpecID'
          },
          //{
          //  title: '状态',
          //  slot: 'status',
          //},
          {
            title: '所属位置',
            key: 'FatherName'
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
        listdata: [],
      }
    },
    beforeRouteEnter (to, from, next){
     next(vm => {
       // 通过 `vm` 访问组件实例,将值传入oldUrl
       vm.oldUrl = from.path
     })
   },
    mounted() {
      this.$nextTick(()=>{  //调用钩子，获取到上一级的url
        this.setnva(this.oldUrl)
     })
      this.Shelvelist(this.query_field)
      this.GetPurposes()//获取用途
      this.GetSpecs();//获取库位规格
      //console.log(JSON.stringify(this.$router.go(-1)))
      // console.log(this.$route.path)
      document.cookie="kuweiurl="+this.$route.path;
      console.log(this.getCookie("huojiaurl"))
      console.log(this.getCookie("kuquurl"))
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
      testname(){  //正则验证库位名称
        // this.formValidate.name
        var testdata=/^[0-9]{2}$/;
        var Result = testdata.test(this.formdata.name);
        console.log(Result)
        if(Result==false){
             this.$Message.error("只能输入两位数字,如'01'");
             this.formdata.name='';
        }
      },
      GetPurposes() {
        GetPurposes("").then(res => {
          console.log(res)
          this.interestlist = res.obj
        })
      },
      GetSpecs() {
        GetSpecs().then(res => {
          console.log(res);
          this.speclist = res.obj;
        })
      },
      handleReset() {
        this.loading=true;
        this.query_field={
          key: this.$route.params.locationID,//父亲ID（必填）
          PageIndex: 1,//当前页码
          PageSize: 6,//每页记录数）
          ID:"",//编号
          Purpose:"",  //业务类型
          ManagerID:"",//负责人
          Type:50
        },
        this.Shelvelist(this.query_field)
      },
      query_btn(){
        // this.query_field={
        //   key: this.$route.params.locationID,//父亲ID（必填）
        //   PageIndex: 1,//当前页码
        //   PageSize: 6,//每页记录数）
        //   ID:"",//编号
        //   Purpose:"",  //业务类型
        //   ManagerID:"",//负责人
        //   Type:50
        // },
        this.loading=true;
        this.Shelvelist(this.query_field)
      },
      selectchange(value) {
        console.log(value)
        this.value1 = value
      },
      setnva(oldurl) {
        var cc = [
          {
            title: "库房配置",
            href: "/kufang"
          },
          {
            title:"库区配置",
            href:this.getCookie("kuquurl")
          },
          {
            title: "货架配置",
            href:this.getCookie("huojiaurl")
          },
          {
            title: "库位",
            href:""
          },
        ]
        this.$store.dispatch("setnvadata", cc)
      },
      showmodel() {
        this.isshowadd = true
      },
      ok() {
        if(this.formdata.name!=""&&this.formdata.shelves!=""&&this.formdata.interest!=""&&this.formdata.specid!=""&&this.formdata.desc!=""&&this.formdata.admins){
              var newmane=this.formdata.shelves+this.formdata.name
             var datas = {
              Name: newmane,//库位名称,名称不能重复，
              FatherID: this.$route.params.locationID,//如何取值
              Type: 50,
              Purpose: this.formdata.interest.toString(),//单选值如何选值
              SpecID: this.formdata.specid.toString(),
              Summary: this.formdata.desc,
              ManagerID:this.formdata.admins
            };
          console.log(datas);
              AddShelve(datas).then(res => {
                this.query_field.PageIndex=1;
                if (res.val == 0) {
                  this.$Message.success(res.msg)
                  this.isshowadd = false;
                  this.Shelvelist(this.query_field); //库区初始化
                  this.GetPurposes();//获取用途
                  this.GetSpecs();
                } else {
                  this.$Message.error(res.msg);
                };
              })
        }else{
          this.$Message.error("请将信息补充完整");
        }
      },
      cancel() {
        this.$refs.formValidate2.handleReset();
        this.formValidate.value1 = []
        this.isshowadd = false;
      },
      Shelvelist(data) {  //货架列表初始化
        Shelvelist(data).then(res => {
          console.log(res);
          this.listdata = res.obj.Data;
          this.total=res.obj.Total;
          this.loading=false;
        })
      },
      printOk() { //确认打印按钮
        var printarr=[]
        for(var i=0,len=this.SelectRow.length;i<len;i++){
            var item={
              ID:this.SelectRow[i].ID,
              Name:this.SelectRow[i].Name,
              EnterpriseName:this.SelectRow[i].EnterpriseName,
              FatherMsg:this.SelectRow[i].FatherMsg,
              LeaseName:this.SelectRow[i].LeaseName
            }
            printarr.push(item)
        }
        if(printarr.length>0){
            var config = GetPrinterDictionary()
            var getsetting = config['库位标签'];
            var str=getsetting.Url
            var testurl=str.indexOf("http") != -1
              if(testurl==true){
                getsetting.Url=getsetting.Url
              }else{
                getsetting.Url=this.printurl+getsetting.Url
            }
            var data = {
              setting: getsetting,
              data: printarr
            };
            // alert(JSON.stringify(data))
            TemplatePrint(data);
            this.printlabel = false;
        }
       

      },
      printNo() { //取消打印按钮
        // alert("取消打印")
        this.printlabel = false;
      },
      showprintbox(){
        if(this.SelectRow.length<=0){
          this.$Message.error('请至少选择一个库位进行打印');
        }else{
          this.printlabel = true;
          console.log(this.SelectRow)
        }
      },
      changepages(value){  //分页事件
         this.loading=true;
         this.query_field.PageIndex=value;
         this.Shelvelist(this.query_field);

      },
      clarn_btn(value){
        console.log(value)
        if(value!=true){
             this.isshowadd = false;
             this.formdata.name=''; 
             this.formdata.shelves='';
             this.formdata.interest='';
             this.formdata.specid='';
             this.formdata.desc='';
             this.formdata.admins="",
             this.adminsarr=[];
             }
      },
       handleSelectRow(value) {
      //多选事件 获取选中的数据
       this.SelectRow = value;
      //  console.log(this.SelectRow)
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
    remove(index) {
    var ID=index;
      this.$Modal.confirm({
            title: '提示',
            content: '是否要删除该库位',
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
    },
    
  }
</script>

<style scoped>
  .search_box li {
    /* width: 20%; */
    float: left;
    margin-right: 20px;
  }

  .table2 {
    border-top: none;
  }

  .tablelist {
    border: 1px solid #dddddd;
    padding: 10px;
    margin-bottom: 15px;
  }

    .tablelist .title {
      line-height: 40px;
    }

  .print_box {
    width: 47%;
    min-height: 135px;
    border: 1px solid #dddddd;
    /* margin-bottom: 10px; */
  }

  .print_title {
    font-size: 16px;
    margin-bottom: 5px;
    margin-top: 5px;
  }

  .label_class {
    margin-right: 8px;
  }

  .searchtable{
    max-width: 12%;
    min-width: 170px;
    margin-right: 6px;
  }

.ivu-modal-body .frominput{
  width: 80%;
}
.bitian{
    color: red;
  }
</style>
