<style>
#Customsbox .title {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
  margin-bottom: 10px;
}
#Customsbox .subCol ul li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  padding: 5px;
  border-bottom: 1px solid #ccc;
  overflow: hidden;
  line-height: 45px;
}
#Customsbox .subCol ul li:last-child {
  border-bottom: none;
}
#Customsbox .tablebox {
  clear: both;
  padding-top: 20px;
}
#Customsbox .pagebox{
  text-align: right;
  margin-top: 20px;
}
.Apply_btn{
  float: right;
}
</style>
<template>
  <div class="Customswindow" id="Customsbox">
    <p class="windowtitle title">装箱清单</p>
    <div>
      <ButtonGroup style="width:328px">
        <Input
          v-model.trim="key"
          placeholder="输入箱号/型号/品牌/小订单号/订单号"
          clearable
          @on-clear="search_btn"
          style="width:80%;float:left;position: relative;left: 3px"
        />
        <!-- <Button style="float:left" @click="search_pro" type="primary">筛选</Button> -->
        <Button style="float:left" type="primary" @click="search_btn">筛选</Button>
      </ButtonGroup>
      <div class="Apply_btn">
        <!-- <Button type="text" icon="ios-create-outline" ghost @click="backpagetest">跳转测试</Button> -->
        <Button type="warning" icon="ios-create-outline" :disabled='isDisabled==true?true:false'  @click="AskCustoms">报关申请</Button>
      </div>
    </div>
    <div class="tablebox">
      <Table :columns="columns" 
             :data="reportList" :loading="loading" 
             :border="reportList.length > 0" 
             @on-selection-change="selection_chenge"
             ref="table" :max-height="tableHeight"
             >
        <template slot-scope="{ row, index }" slot="orderID">
          {{row.TinyOrderID}}
        </template>
        <!-- <template slot-scope="{ row, index }" slot="Boxcode">
           {{row.BoxCode}}
        </template> -->
        <template slot-scope="{ row, index }" slot="PackageType">
           {{row.PackageType}}
        </template>
        <template slot-scope="{ row, index }" slot="Entercode">
           {{row.EnterCode}}
        </template>
        <template slot-scope="{ row, index }" slot="BoxingDate">
           {{row.BoxingDate|showDateexact}}
        </template>
        <template slot-scope="{ row, index }" slot="Admin">
           {{row.Packer}}
        </template>
        <template slot-scope="{ row, index }" slot="AVGWeight">
           {{row.TotalWeight}}
        </template>
        <template slot-scope="{ row, index }" slot="total">
           {{row.TotalBoxCode}}
        </template>
        <template slot-scope="{ row, index }" slot="boxcode">
          <div class="subCol">
            <ul>
              <li v-for="item in row.Items" style="text-align: left;">
                <span>{{item.Declare.BoxCode|showboxcode}}</span>
              </li>
            </ul>
          </div>
        </template>
        <template slot-scope="{ row, index }" slot="PartNumber">
          <div class="subCol">
            <ul>
              <li v-for="item in row.Items" style="text-align: left;">
                <span>{{item.Procut.PartNumber}}</span>
                <span v-if="item.Condition!=null">
                      <Tag v-if="item.Condition.IsCIQ==true" color="primary">商检</Tag>
                      <Tag v-if="item.Condition.IsCCC==true" color="warning">CCC</Tag>
                      <Tag v-if="item.Condition.IsEmbargo==true" color="error">禁运</Tag>
                      <Tag v-if="item.Condition.IsHighPrice==true" color="magenta">高价值</Tag>
                      <!-- <Tag  color="primary">商检</Tag>
                      <Tag  color="warning">CCC</Tag>
                      <Tag  color="error">禁运</Tag>
                      <Tag  color="magenta">高价值</Tag> -->
                </span>
              </li>
            </ul>
          </div>
        </template>
      </Table>
    </div>
    <div class="pagebox">
      <Page :total="Total" 
       show-total
       show-elevator 
       show-sizer
      :current='PageIndex' 
      :page-size='PageSize'
      @on-change='changepage' 
      :page-size-opts="showPageArr"
      @on-page-size-change="changepagesize"
      />
    </div>
    <!-- 申报窗口确认 开始 -->
    <Modal v-model="isdeclare" title="提示" @on-ok="ok_declare" @on-cancel="cancel_declare">
      <p>您是否确认申请报关？</p>
    </Modal>
    <!-- 申报窗口确认 结束-->
  </div>
</template>
<script >
//  库位，规格 是可选择的？（建议下拉选择可输入）
import $ from "jquery";
import {CgDelcarelist,CustomsApply} from "../../api/CgApi"
// import {GetUsableShelves,boxproducts,BoxingSpecs,GetBoxes,,ChangeBoxCode} from "../../api";
export default {
  name: "Customswindow",
  data() {
    return {
      isDisabled:false,
      key: "",
      Status:20,
      loading: true,
      isdeclare: false, //申报弹窗
      selectionarr: [], //全选与多选
      PageIndex:1,
      PageSize:20,
      Total:0,
      columns: [
        {
          type: 'selection',
          width: 60,
          align: 'center'
        },
        { title: "小订单号", slot: "orderID", align: "center",maxWidth:100 },
         { title: "入仓号", slot: "Entercode", align: "center",maxWidth:80},
        { title: "箱号", slot: "boxcode",align: "center", width:180,},
       
        
        
        {
          title: "型号",
          slot: "PartNumber",
          align: "center",
        },
        {
          title: "品牌",
          key: "list",
          align: "center",
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].Items.map(item => {
                    return h("li", {}, item.Procut.Manufacturer);
                  })
                )
              ]
            );
          }
        },
        {
          title: "申报数量",
          key: "list",
          align: "center",
          width:100,
          render: (h, params) => {
            return h(
              "div",
              {
                attrs: {
                  class: "subCol"
                }
              },
              [
                h(
                  "ul",
                  this.reportList[params.index].Items.map(item => {
                    return h("li", {}, item.Declare.Quantity);
                  })
                )
              ]
            );
          }
        },
        { title: "总重量(Kg)", slot: "AVGWeight", align: "center",maxWidth:80},
        { title: "总箱数", slot: "total", align: "center",maxWidth:80},
         { title: "包装码", slot: "PackageType", align: "center",maxWidth:80},
        { title: "装箱时间", slot: "BoxingDate", align: "center",maxWidth:100},
        { title: "装箱人", slot: "Admin", align: "center",maxWidth:80},
      ],
      reportList2:[],
      reportList: [],
      tableHeight:600,
    };
  },
  created() {
    this.setnva();
    this.Status=this.$route.params.id;
    this.getlist() 
    this.setdisable()
  },
   beforeRouteUpdate(to,from,next){
       this.Status=to.params.id;
       this.PageIndex=1,
       this.PageSize=20,
       this.key='',
       this.setdisable()
      this.getlist()
        next()
  },
  mounted() {
    this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 100
  },
  computed: {
    showPageArr(){
        return this.$store.state.common.PageArr;
      }
  },
  methods: {
    setdisable(){
      if(this.Status==20){
        this.isDisabled=false;
      }else{
        this.isDisabled=true;
      }
      // this.PageIndex=1;
    },
    setnva() {
      var cc = [
        {
          title: "报关申报",
          href: "/Transport/Customswindow"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    getlist(){  //获取报关数据
    console.log(this.PageIndex)
      var data={
        PageIndex:this.PageIndex,//页码
        PageSize:this.PageSize,//单页条数
        Status:this.Status,//枚举请参考，目前我们只涉及20（未申报），30（已申报）
        // Packer:sessionStorage.getItem("userID"),//装箱人
        First:this.key//型号、厂商、箱号，陈翰觉得这样做不是很合理？
      }
        CgDelcarelist(data).then(res=>{
          console.log(res)
          this.Total=res.obj.Total;
          if(res.obj.Total>0){
              this.reportList=res.obj.Data;
            if(this.Status==30){
              this.reportList.forEach(item => {
                item._disabled =true;
              })
            }
          }else{
            this.reportList=[];            
          }
          this.loading=false;
        })
    },
    search_btn(){ //根据信号品牌筛选
      this.loading=true;
      this.PageIndex=1;
      this.getlist()
    },
    ok_declare() {
      //确认申请报关
       this.isDisabled=true;
       var BoxIds=[]
      for (var i = 0; i < this.selectionarr.length; i++) {
        BoxIds.push(this.selectionarr[i].TinyOrderID);
      }
      console.log(BoxIds);
      var data={
        AdminID:sessionStorage.getItem("userID"),
        TinyOrderID:BoxIds
      }
      CustomsApply(data).then(res => {
        this.isDisabled=false;
        if (res.val == 400) {
          this.$Message.error({
            content:res.msg,
            duration: 5
          });
        } else {
          this.$Message.success("申报成功");
          this.getlist();
        }
      });
    },
    cancel_declare() {
      //取消申请报关
    },
    selection_chenge(selection) {
      this.selectionarr = selection;
    },
    AskCustoms() {
      //报关申请
      if (this.selectionarr.length <=0) {
        this.$Message.error({
          content: "至少选择一个要申请报关的产品",
          duration: 1,
          closable: true,
          top: 50
        });
      } else {
        this.isdeclare = true;
      }
    },
    search_btn() {
      //筛选
      this.loading = true;
      this.getlist();
    },
    clear_search() {
      //清空筛选
      this.loading = true;
      this.getlist();
    },
    //分页
    changepage(value){
       this.loading = true;
       this.PageIndex=value;
       this.getlist()
    },
    changepagesize(value){
       this.loading = true;
       this.PageSize=value;
       this.getlist()
    }
  }
};
</script>

