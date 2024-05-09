<style scoped>
.subCol li {
  margin: 0 -18px;
  list-style: none;
  text-align: center;
  /* padding: 9px; */
  border-bottom: 1px solid #ccc;
  overflow-x: hidden;
  height: 45px;
  line-height: 44px;
}
.subCol li:last-child {
  border-bottom: none;
}
.Mustfill{
  color: red;
}
.Statisticsbox{
  height: 48px;
  background: #f8f8f9;
  line-height: 48px;
  text-indent: 20px;
  border:1px solid #e8eaec;
  border-top: none;
}
.Statisticsbox .Statisticspan{
  padding-right: 30px;
}
#tablebox td{
  font-size: 12px;
}
.changeboxcodeicon:hover{
  cursor: pointer;
}

  .setlinen {
    margin-bottom:10px;
  }
</style>
<template>
  <div>
    <div id="tablebox">
      <Table @on-selection-change='selectionchange'
             :columns="Normal"
             :data="NormalData"
             :border="NormalData.length > 0"
             :loading="loading">
        <template slot-scope="{ row, index }" slot="orderid">
          <span>{{row.TinyOrderID}}</span>
        </template>
        <template slot-scope="{ row, index }" slot="BoxCode">
          <ul class="subCol">
            <li v-for="item in row.Groups">
              {{item.BoxCode|showboxcode}}
              <Icon class="changeboxcodeicon" type="md-create" v-if="row.IsAbleDeclare==true" @click="changeboxcode(item)" />
            </li>

          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="PartNumber">
          <ul class="subCol">
            <li v-for="item in row.Groups">
              {{item.Product.PartNumber}}
              <span v-if="item.Condition!=null">
                <Tag v-if="item.Conditions.IsCIQ==true" color="primary">商检</Tag>
                <Tag v-if="item.Conditions.IsCCC==true" color="warning">CCC</Tag>
                <Tag v-if="item.Conditions.IsEmbargo==true" color="error">禁运</Tag>
                <Tag v-if="item.Conditions.IsHighPrice==true" color="magenta">高价值</Tag>
              </span>
            </li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Manufacturer">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.Product.Manufacturer}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="DateCode">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.DateCode}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Origin">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.Origin}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Weight">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.Weight}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Volume">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.Volume}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Quantity">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.YdQty}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="PickedQuantity">
          <ul class="subCol">
            <li v-for="(item,index) in row.Groups">
              <Input style="width:70%" v-if="itemindex==index&&showinput==true" type="number" @on-blur="ok_changeQty(item)" @on-enter="ok_changeQty(item)" v-model="item.SdQty" />
              <p v-else>
                <span>{{item.SdQty}}</span>
                <!-- <Icon v-if="row.IsAbleDeclare==true" type="md-create" @click="changeQuantityBtn(index)"/> -->
              </p>
            </li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="CreateDate">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.EnterDate|showDateexact}}</li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="CarrierName">
          <ul class="subCol">
            <li v-for="item in row.Groups">{{item.CarrierName}}
            <Icon v-if="chengeCarrier.waybilltype==3||chengeCarrier.waybilltype==4" class="changeboxcodeicon" type="md-create" @click="showchangecarriercode(item)" /></li>
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="code">
          <ul class="subCol">
            <li v-for="item in row.Groups">
              {{item.wbCode}}
              <Icon v-if="chengeCarrier.waybilltype==3||chengeCarrier.waybilltype==4" class="changeboxcodeicon" type="md-create" @click="showchangecarriercode(item)" />
            </li>
           
          </ul>
        </template>
        <template slot-scope="{ row, index }" slot="Action">
          <ul class="subCol">
            <li v-for="item in row.Groups">
              <Button type="error"
                      size="small"
                      @click="delItem(item)"
                      :disabled="item.Display==false?true:false">
                删除
              </Button>
            </li>
          </ul>
        </template>
      </Table>
      <p class="Statisticsbox" v-if="NormalData.length > 0">
        <span class="Statisticspan">合计：</span>
        <span class="Statisticspan">总重量(Kg)：{{Normalinfo.TotalWeight}}</span>
        <span class="Statisticspan">总件数：{{Normalinfo.TotalPart}}</span>
        <span class="Statisticspan">总数量：{{Normalinfo.TotalQuantity}}</span>
        <Button type="primary" icon='md-checkmark' :disabled='Normalinfo.IsDisabledDeclareBtn==false?false:true' @click="ok_Sealing">确认封箱</Button>
      </p>
    </div>
    <Modal v-model="showEdit"
           title="修改箱号"
           :mask-closable="false"
           @on-visible-change='visiblechange'>
      <span slot="close">
        <Icon type="ios-close" @click="cancel" />
      </span>

      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>日期：
        </label>
        <DatePicker type="date" style="width:80%" :options="options3" placeholder="请选择生成箱号的时间" :clearable='false' :value="saleDate" @on-change='changeData'></DatePicker>
      </p>
      <p style="padding-top:10px;">
        <label>
          <em class="Mustfill">*</em>箱号：
        </label>
        <Input v-model.trim="newboxcode"
               maxlength="30"
               placeholder="请输入临时箱号"
               style="width:80%"
               @on-blur='handleCreate1(newboxcode)' />
      </p>
      <div slot="footer">
        <Button @click="cancel">取消</Button>
        <Button type="primary" @click="ok_change">确定</Button>
      </div>
    </Modal>

    <!-- 修改数量 开始 -->
    <!-- 修改数量 结束 -->
    <!-- 确认封箱 -->
    <Modal v-model="showsealing"
           @on-ok="ok_sealing"
           @on-cancel="cancel_sealing">
      <div slot="header">
        <Icon type="md-help-circle" style="font-size:28px;color:#f90" />
        <span style="font-size: 15px;line-height: 26px;vertical-align: bottom; color: #0c0c0c; font-weight: bold;">以下订单不符合重量浮动规则，是否确认封箱</span>
      </div>
      <p v-for="item in overweight">
        {{item.TinyOrderID}}
        <span style="padding-right:5px">总重量：{{item.TotalWeight}}</span>
        <span style="padding-right:5px">平均重量：{{item.TotalAvgWeight}}</span>
      </p>
    </Modal>
    <!-- 确认封箱结束 -->
    <!--修改承运商与运单号 开始-->
    <Modal v-model="changeCarriercode"
           title="修改承运商/运单号"
           :mask-closable='false'
           @on-visible-change="ISchangeCarriercode">
      <div>
        <div class="setlinen">
          <label for=""><em class="Mustfill">*</em>快递类型:</label>
          <Select v-model="chengeCarrier.waybilltype" style="width:80%"
                  @on-change="changetype">
            <Option v-for="(item,index) in TypeArr"
                    :value="item.value"
                    :key="index">
              {{ item.label}}
            </Option>
          </Select>
        </div>
        <div class="setlinen">
          <label for=""><em class="Mustfill">*</em>承&nbsp;&nbsp;运&nbsp;&nbsp;商:</label>
          <Select v-model="chengeCarrierCarrierID" style="width:80%" @on-change="chengeCarriercode=null">
            <Option v-for="item in CarrierList"
                    :value="item.ID"
                    :key="item.ID">
              {{ item.Name }}
            </Option>
          </Select>
        </div>
        <div class="setlinen">
          <label for=""><em class="Mustfill">*</em>运&nbsp;&nbsp;单&nbsp;&nbsp;号:</label>
          <Input v-model="chengeCarriercode" placeholder="请输入运单号" style="width:80%" />
        </div>
      </div>
      <div slot="footer">
        <Button @click="changeCarriercode=false">取消</Button>
        <Button type="primary" @click="ok_changecarrier">确定</Button>
      </div>
    </Modal>
    <!--修改承运商与运单号 开始-->
  </div>
</template>
<script>
  import { GetSealinglist, HistorDeleteSorting, BoxcodeEnter, CgBoxesShow, BoxcodeDelete, ModifyQuantity, CloseBoxes, CgCarriers, CGModifyWbCodeAndCarrierID } from "../../api/CgApi"; //引入api 的接口
export default {
  name: "Sealing",
    props: ["OrderID", "Type", 'EnterCode', 'hietorgetDetail', 'WaybillID', 'ExcuteStatus', 'chengeCarrier'],
  data() {
    return {
      changeCarriercode: false,
      TypeArr: [
        {
          value: 3,
          label: "快递"
        },
        {
          value: 4,
          label: "国际快递"
        },
      ],
      CarrierList: [],//承运商列表
      chengeCarrierCarrierID: null,//承运商
      chengeCarriercode: null,//运单号
      options3: {
          disabledDate (date) {
              return date && date.valueOf() < Date.now() - 86400000;
          }
      },

      itemindex:-1,
      showinput:false,
      changeQuantity:false,//修改数量的弹出框
      NewQuantity:null,//修改后的数量
      selectdata:[],//选中的项
      saleDate:'',//箱号时间
      Operateddisable:true,//删除按钮状态
      isclickbtn:true,
      isSealingclick:true,//封箱按钮是否可用
      enterCode:null,
      boxingarr:[],
      houseid:sessionStorage.getItem("UserWareHouse"),
      AdminID:sessionStorage.getItem("userID"),
      oldboxcode:'',
      newboxcode:'',
      newboxcodeback:null,
      oldBoxCode2:null,//输入框中的旧箱号
      boxcodetype:'1',
      showEdit:false,
      Nameitem:'0',
      loading: true,
      warehouseId: null,
      waybillinfo: {},
      NormalData: [], //正常数据
      AbnormalData: [], //异常数据
      Normalinfo:null,
      Abnormainfo:null,
      Normal: [
        {
          type: 'selection',
          width: 60,
          align: 'center'
        },
        {
          title: "订单号",
          slot: "orderid",
          align: "center"
        },
        {
          title: "箱号",
          slot: "BoxCode",
          align: "center"
        },
        {
          title: "型号",
          slot: "PartNumber",
          align: "center",
          width:180
        },
        {
          title: "品牌",
          slot: "Manufacturer",
          align: "center"
        },
        {
          title: "批号",
          slot: "DateCode",
          align: "center"
        },
        {
          title: "原产地",
          slot: "Origin",
          align: "center"
        },
        {
          title: "重量(Kg)",
          slot: "Weight",
          align: "center"
        },
        {
          title: "体积",
          slot: "Volume",
          align: "center"
        },
        {
          title: "应到",
          slot: "Quantity",
          align: "center"
        },
        {
          title: "实到",
          slot: "PickedQuantity",
          align: "center",
           width:130
        },
        {
          title: "承运商",
          slot: "CarrierName",
          align: "center",
          width:150
        },
        {
          title: "运单号",
          slot: "code",
          align: "center",
          width:150
        },
        {
          title: "入库时间",
          slot: "CreateDate",
          align: "center",
           width:145
        }
      ],
      overweight:[],//超过浮动规则的小订单
      showsealing: false,
      GroupsWaybillID:null,//修改运单号需要的id

    };
  },
    created() {
     
  },
  computed:{
    getExcuteStatus(){
      console.log(this.ExcuteStatus)
      return this.ExcuteStatus
    },
  },
    mounted() {
      console.log(this.chengeCarrier)

    if (this.Type == 1) {
      this.Normal[7].title = "库位";
      this.Abnormal[7].title = "库位";
    } else {
      var CarrierNamedatas = {
        title: "承运商",
        slot: "CarrierName",
        align: "center",
        width:150
      };
      // this.Normal.push(CarrierNamedatas);
    //   this.Abnormal.push(CarrierNamedatas);
       var codedatas = {
        title: "运单号",
        slot: "code",
        align: "center",
         width:150
      };
      // this.Normal.push(codedatas);
    //   this.Abnormal.push(codedatas);
      var datas = {
        title: "操作",
        slot: "Action",
        align: "center"
      };
      this.Normal.push(datas);
    //   this.Abnormal.push(datas);
    }
  },
  methods: {
     setboxsplit(str) {
      //去除前后空格
      if(str){
         return str.split("]")[1]
      }
    },
    getlistdata(){
      GetSealinglist(this.houseid,this.OrderID).then(res=>{
        this.loading=false
        this.Normalinfo = res;
        this.NormalData=res.TinyOrderID_Group
      })
    },
    delItem(item) {
      this.$Modal.confirm({
            title: '是否确认删除该分拣',
            // content: '<p>Content of dialog</p><p>Content of dialog</p>',
            onOk: () => {
                var data = {
                  StorageID:item.StorageID,
                  AdminID:sessionStorage.getItem("userID")
                };
                HistorDeleteSorting(data).then(res => {
                    if(res.Success==true){
                      this.$Message.success('删除成功');
                      this.loading=true
                      // this.changeTabs(this.Nameitem)
                      this.getlistdata()
                      this.$emit('hietorgetDetail')
                    }else if(res.Success==false){
                      this.$Message.error(res.Data);
                    }
                });
            },
            onCancel: () => {
                // this.$Message.info('Clicked cancel');
            }
        });
    },
    changeboxcode(value){
      console.log(value)
      this.showEdit=true
      this.oldboxcode=value
    },
    handleCreate1(val) {  //箱号添加
      if(val!=''&&val!=null){
         if(this.oldBoxCode2!=null){
               var data={
                    boxCode:this.oldBoxCode2,
                    // date:this.saleDate
                  }
                  BoxcodeDelete(data).then(res=>{
                    var newdata={
                       enterCode:this.EnterCode, // 统一使用当前运单的entercode
                       code:val, // 箱号
                       date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
                       adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
                    }
                    BoxcodeEnter(newdata).then(res=>{
                      console.log(res)
                      if(res.success==false&&res.code==400){
                        this.$Message.error("箱号已经被选择，请选择其他箱号");
                        this.newboxcode=null;
                        this.newboxcodeback=null
                        this.oldBoxCode2=null;
                        this.isclickbtn=false
                      }else{
                        this.newboxcode=this.setboxsplit(res.boxCode)
                        this.newboxcodeback=res.boxCode
                        this.oldBoxCode2=res.boxCode
                        this.isclickbtn=true
                      }
                    })
                })
              }else{
                 var data={
                     enterCode:this.EnterCode, // 统一使用当前运单的entercode
                    code:val, // 箱号
                    date:this.saleDate, //为原有箱号管理保留的时间要求，就是我可以生产指定一天的箱号
                    adminID:sessionStorage.getItem("userID"),//装箱人使用当前操作的adminID
                  }
                  BoxcodeEnter(data).then(res=>{
                    if(res.success==false){
                    this.$Message.error(res.data)
                    this.newboxcode=null
                    this.newboxcodeback=null
                    this.isclickbtn=false
                    }else{
                      this.newboxcode=this.setboxsplit(res.boxCode)
                      this.newboxcodeback=res.boxCode
                      this.oldBoxCode2=res.boxCode
                      this.isclickbtn=true
                    }
                  })
              }
            
           
         
      }
      
    },
     CgBoxesShow() {
      var data = {
        enterCode: this.EnterCode, //入仓号
        date: "", //箱号日期（可为空，为空时展示当前日期的箱号）
      };
        console.log(this.EnterCode)
        CgBoxesShow(data).then(res => {
          if (res.length > 0) {
            this.boxingarr = res;
          }else{
            this.boxingarr=[]
          }
        });
    },
    visiblechange(value){
      if(value==true){
        this.CgBoxesShow()
        this.showchangebox = true;
        const myDate = new Date();
        const year = myDate.getFullYear(); // 获取当前年份
        const month = myDate.getMonth() + 1; // 获取当前月份(0-11,0代表1月所以要加1);
        const day = myDate.getDate(); // 获取当前日（1-31）
        // 日期格式：2019/07/29 - 2019/07/29
        this.saleDate = `${year}/${month}/${day}`;
      }else{
        this.newboxcode=null
        this.oldBoxCode2=null
        this.newboxcodeback=null
        this.boxcodetype='1'
      }
    },
     // 删除选定的箱号
    BoxcodeDeletefun(){
      var data={
        boxCode:this.newboxcodeback,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
        console.log(res)
      })
    },
    oldBoxDelete(val){
      var data={
        boxCode:val,
        // date:this.saleDate
      }
      BoxcodeDelete(data).then(res=>{
      })
    },
    cancel(){
      this.BoxcodeDeletefun()
      this.showEdit=false
      this.oldBoxCode2=null
      this.newboxcodeback=null
    },
    // 提交修改的箱号
    ok_change(){
      if(this.newboxcode!=null&&this.newboxcode!=''){
        if(this.isclickbtn==true&&this.newboxcodeback!=null){
          var data={
          date:this.saleDate,
          adminID:sessionStorage.getItem("userID"), //装箱人使用当前操作的adminID，必填
          storageID:this.oldboxcode.StorageID, //只在修改箱号中起作用, 如果有库存ID表示：修改某一在库的箱号，必填
          oldBoxCode:this.oldboxcode.BoxCode, //只在修改箱号中起作用, 如果有库存ID表示：必填
          newBoxCode:this.newboxcodeback,//只在修改箱号中起作用, 如果有库存ID表示：必填
          source:30, /// ---->新增的, 就是当前通知类型,比如代报关 30, 转报关 60
          }
          BoxcodeEnter(data).then(res=>{
              if(res.success==false){
                this.$Message.error(res.data)
                this.newboxcodeback=null
                this.showEdit=true
              }else{
                this.$Message.success('修改成功')
                 this.getlistdata()
                this.showEdit=false
              }
          })
        }
        
      }else{
        this.$Message.error('请输入箱号')
      }
      
    },
      changeData(val){
      this.saleDate=val
      if(this.newboxcode!=''&&this.newboxcode!=null){
        this.handleCreate1(this.newboxcode)
      }
    },
    ok_Sealing(){
      if(this.selectdata.length<=0){
        // this.$Message.error('请选择要封箱的订单')
        this.$Message.warning({
                content: '请选择要封箱的订单',
                duration: 3
        });
      }else{
      var CloseBoxesArr=[];
      this.overweight=[]
      var newsrr=this.selectdata
      for(var i=0,lens=newsrr.length;i<lens;i++){
        CloseBoxesArr.push(newsrr[i].TinyOrderID)
        if(newsrr[i].TotalWeight<(newsrr[i].TotalAvgWeight-newsrr[i].TotalAvgWeight*(1/2))||newsrr[i].TotalWeight>(newsrr[i].TotalAvgWeight+newsrr[i].TotalAvgWeight*(1/2))){
          this.overweight.push(newsrr[i])
        }
      }
      var data={
        AdminID:sessionStorage.getItem("userID"),
        TinyOrderID:CloseBoxesArr,
        WaybillID:this.WaybillID
      }
      console.log(this.overweight)

      if(this.overweight.length>0){
         this.showsealing=true
      }else{
        this.showsealing=false
        this.$Modal.confirm({
          title: '是否确认封箱',
          // content: ``,
          onOk: () => {
              this.isSealingclick=false;
              this.CloseBoxes(data)
          },
          onCancel: () => {
              // this.$Message.info('Clicked cancel');
          }
        });
      }
      }
    },
    ok_sealing(){
       var CloseBoxesArr=[];
       var newsrr=this.selectdata
      for(var i=0,lens=newsrr.length;i<lens;i++){
        CloseBoxesArr.push(newsrr[i].TinyOrderID)
      }
       var data={
        AdminID:sessionStorage.getItem("userID"),
        TinyOrderID:CloseBoxesArr,
        WaybillID:this.WaybillID
      }
       this.isSealingclick=false;
       this.CloseBoxes(data)
    },
    cancel_sealing(){
      this.showsealing=false
    },
     // 确认封箱
    CloseBoxes(data){
      CloseBoxes(data).then(res=>{
        if(res.success==true){
          this.loading=true
          this.getlistdata()
          this.$Message.success('封箱成功');
           this.selectdata=[]
        }else{
          this.$Message.error({
                content: res.data,
                duration: 3
          });
        }
        this.isSealingclick=true;
      })
    },
    selectionchange(val){
      this.selectdata=val
      console.log(this.selectdata.length)
    },

    changeQuantityBtn(index){
      this.itemindex=index;
      this.showinput=true;
    },
    ok_changeQty(val){
      console.log(val)
      if(val.SdQty!=''&&val.SdQty!=null){
        this.loading=true
        var AdminID=sessionStorage.getItem("userID");
        var data={
          StorageID:val.StorageID,
          AdminID:AdminID,
          Quantity:val.SdQty
        }
        this.chengeModify(data)
      }else{
        this.$Message.warning({
                content: '请输入要修改的数量',
                duration: 3
        });
      }
    },
    //修改箱号与数量接口
    chengeModify(data){
      ModifyQuantity(data).then(res=>{
        console.log(res)
        if(res.Success==true){
          this.itemindex=-1;
          this.showinput=false;
          this.getlistdata()
        }else{
          this.$Message.error({
                content: res.Data,
                duration: 3
          });
        }
      })
    },
    ISchangeCarriercode(value) {
      if (value == false) {
        this.changeCarriercode = false
      }
    },

    //跟据类获取承运商
    getcarrier(type) {
      CgCarriers(type, this.houseid, 100).then(res => {
        this.CarrierList = res;
      });
    },

    //显示修改弹出
    showchangecarriercode(Groups) {
      console.log(Groups)
      this.changeCarriercode = true
      this.GroupsWaybillID = Groups.WaybillID
      this.chengeCarrierCarrierID = Groups.CarrierID
      this.chengeCarriercode = Groups.wbCode
    this.getcarrier(this.chengeCarrier.waybilltype)
    },
    changetype(value) {
      this.getcarrier(value)
      this.chengeCarrierCarrierID = null
      this.chengeCarriercode = null

    },
    ok_changecarrier() {
      if (!this.chengeCarrier.waybilltype == false && !this.chengeCarrierCarrierID == false && !this.chengeCarriercode == false) {
        let data = {
          "WaybillID": this.GroupsWaybillID,
          "wbCode": this.chengeCarriercode,
          "CarrierID": this.chengeCarrierCarrierID,
          "Type": this.chengeCarrier.waybilltype
        }
        CGModifyWbCodeAndCarrierID(data).then(res => {
          if (res.Success == true) {
            this.changeCarriercode = false
            this.loading = true
            this.getlistdata()
            this.$Message.success({
              content: "修改成功",
              duration: 3
            });
          }
        })
      } else {
        this.changeCarriercode = true
        this.$Message.error({
          content: res.Data,
          duration: 3
        });
      }
     
    }
  }
};
</script>   
<style scoped>
.detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
.detail_li .itemli {
  line-height: 45px;
  font-size: 14px;
}
</style>
