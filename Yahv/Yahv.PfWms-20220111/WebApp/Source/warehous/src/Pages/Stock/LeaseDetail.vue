
<template>
  <div id="LeaseDetail">
    <p class="detailtitle">基本信息</p>
    <div class="box">
      <Row style="padding:10px;">
        <Col :xs="2" :sm="4" :md="6" :lg="8">
          <p>
            <label>
              订单编号：
              <!-- <Input v-model="detailinfo.OrderID" placeholder="请输入订单编号" style="width: 200px" /> -->
              <span>{{detailinfo.OrderID}}</span>
            </label>
          </p>
          <p style="padding-top:10px">
            <label>
              客户编号：
              <!-- <Input v-model="detailinfo.ClientID" placeholder="请输入客户编号" style="width: 200px" /> -->
              <span>{{detailinfo.ClientID}}</span>
            </label>
          </p>
          <p style="width:163%;padding-top:10px">
            <label>
              备&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;注：
              <span
                style="width:90%;float:right"
              >{{detailinfo.Summary}}</span>
              <!-- <Input style="width:90%;float:right"  type="textarea" v-model="detailinfo.Summary":autosize="{maxRows: 3,minRows: 3}"  placeholder="请输入备注信息" /> -->
            </label>
          </p>
        </Col>
        <Col :xs="20" :sm="16" :md="12" :lg="8">
          <p>
            <label>
              下单时间：
              <span>{{detailinfo.CreateDate|showDate}}</span>
            </label>
          </p>
          <p style="padding-top:10px">
            <label>
              租赁状态：
              <span>{{detailinfo.StatuDes}}</span>
            </label>
          </p>
        </Col>
        <Col :xs="2" :sm="4" :md="6" :lg="8">
          <p>
            <label>
              租赁开始时间：
              <span>{{detailinfo.StartDate|showDate}}</span>
            </label>
          </p>
          <p style="padding-top:10px">
            <label>
              租赁结束时间：
              <span>{{detailinfo.EndDate|showDate}}</span>
            </label>
          </p>
        </Col>
      </Row>
    </div>
    <p class="detailtitle">产品列表</p>
    <div style="padding-top:15px;" class="tablebox">
       <Table :columns="columnsnew" :data="reportListnew" border>
        <template slot-scope="{ row, index }" slot="grade_name">
          {{index+1}}
        </template>
      </Table>
      <div class="button_div">
        <Button type="primary" @click="sumbithous" v-if="detailinfo.Status==2||detailinfo.Status==3">保存</Button>
      </div>

    </div>
  </div>
</template>
<script>
// HK01-F01-0101
import { lsnoticeDetail,lsnoticeEnter } from "../../api";
import moment from "moment";
export default {
  data() {
    return {
      detailinfo: "",
      ID: this.$route.query.ID,

      columnsnew: [
        { title: "#", slot: "grade_name", align: "center" ,width:50},
        { title: "等级", key: "SpecID", align: "center", width:100},
        {
          title: "分配库位",
          key: "list",
          align: "center",
          render: (h, params) => {
            if(this.reportListnew[params.index].ShelveIDs.length<=0){
              // this.reportListnew[params.index].ShelveIDs.length=this.reportListnew[params.index].Quantity;
              for(var i=0;i<this.reportListnew[params.index].Quantity;i++){
                    var newstring="";
                    this.reportListnew[params.index].ShelveIDs.push(newstring)
              }
            }
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
                    {
                       attrs: {
                          class: "subColdiv"
                        }
                    },
                    this.reportListnew[params.index].ShelveIDs.map((item,index) => {
                    return h('li',{
                              style:{
                                "line-height":"30px",
                                "padding": "8px 0",
                                "width": "100%",
                                "border-bottom": "1px solid #dddddd",
                              }
                            },[
                            h('Select', {
                                props:{
                                    value:item,
                                    transfer:true,
                                },
                                style:{
                                   width: "90%",
                                   margin: "0 auto",
                                   padding:"0",
                                },
                                attrs: {
                                  class: "subColli"
                                },
                                on: {
                                    'on-change':(value) => {
                                        this.reportListnew[params.index].ShelveIDs[index] = value;
                                    }
                                },
                            },
                          this.reportListnew[params.index].UsableShelves.map(function(type){
                                return h('Option', {
                                    props: {value: type.ID}
                                }, type.ID);
                            }))
                        ])
                    
                    })
                  )
                ]
              );

          }
        },
      ],
      columns2: [
        { title: "#", slot: "grade_name", align: "center" ,width:50},
        { title: "等级", key: "SpecID", align: "center", width:100},
        {
          title: "分配库位",
          key: "list",
          align: "center",
          render: (h, params) => {
              if(this.reportList2[params.index].ShelveIDs=[]){
                for(i=0;i<this.reportList2[params.index].Quantity;i++){
                  var news={
                    ii:"gdjkdjgk"
                  };
                  this.reportList2[params.index].ShelveIDs.push(news)
                }
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
                    {
                       attrs: {
                          class: "subColdiv"
                        },
                        
                    },
                    this.reportList2[params.index].ShelveIDs.map((item,index) => {
                         console.log(item)
                    return h('li',{
                              style:{
                                "line-height":"30px",
                                "padding": "8px 0",
                                "width": "100%",
                                "border-bottom": "1px solid #dddddd",
                              }
                            },[
                            h('Select', {
                                props:{
                                    value:item,
                                    transfer:true,
                                },
                                style:{
                                   width: "90%",
                                   margin: "0 auto",
                                   padding:"0",
                                },
                                attrs: {
                                  class: "subColli"
                                },
                                on: {
                                    'on-change':(value) => {
                                        this.reportList2[params.index].ShelveIDs[index] = value;
                                    }
                                },
                            },
                          this.reportList2[params.index].UsableShelves.map(function(type){
                                return h('Option', {
                                    props: {value: type.ID}
                                }, type.ID);
                            }))
                        ])
                    
                    })
                  )
                ]
              );
              }else{
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
                    {
                       attrs: {
                          class: "subColdiv"
                        },
                        
                    },
                    this.reportList2[params.index].ShelveIDs.map((item,index) => {
                         console.log(item)
                    return h('li',{
                              style:{
                                "line-height":"30px",
                                "padding": "8px 0",
                                "width": "100%",
                                "border-bottom": "1px solid #dddddd",
                              }
                            },[
                            h('Select', {
                                props:{
                                    value:item,
                                    transfer:true,
                                },
                                style:{
                                   width: "90%",
                                   margin: "0 auto",
                                   padding:"0",
                                },
                                attrs: {
                                  class: "subColli"
                                },
                                on: {
                                    'on-change':(value) => {
                                        this.reportList2[params.index].ShelveIDs[index] = value;
                                    }
                                },
                            },
                          this.reportList2[params.index].UsableShelves.map(function(type){
                                return h('Option', {
                                    props: {value: type.ID}
                                }, type.ID);
                            }))
                        ])
                    
                    })
                  )
                ]
              );
              }
              

          }
        },
      ],
     reportList2: [
        {
            ID: "LsNotice201911280001",
            Quantity: 5,
            ShelveIDs: ["","","",""],
            SpecID: "AB01",
            UsableShelves:[
              {
                ID:"hk02",
              },
              {
                ID:"sz04",
              },
            ]
      },
       {
            ID: "LsNotice201911280001",
            Quantity: 5,
            ShelveIDs: ["","",],
            SpecID: "AB01",
            UsableShelves:[
              {
                ID:"sz01",
              },
              {
                ID:"sz014",
              },
              {
                ID:"hk02",
              },
            ]
      }
    ],
    volumeTypes:[
      {
        ID: "HK01-F01-0114"
      },
      {
        ID: "HK01-F01-0205"
      }
    ],
    reportListnew:[],
    };
  },
  filters: {
    showDate: function(val) {
      //时间格式转换
      // console.log(val)
      if (val != "") {
        if (val || "") {
          var b = val.split("(")[1];
          var c = b.split(")")[0];
          var result = moment(+c).format("YYYY-MM-DD");
          return result;
        }
      }
    }
  },
  mounted() {
    this.getdetail();
    console.log("重新加载");
    
  },
  methods: {
     sumbithous(){
          var isnull=0;
          var newarr=this.reportListnew;
          newarr.map((item,index)=>{
            item.ShelveIDs.map((shelveitem,index)=>{
                if(shelveitem==""){
                  // this.$Message.error('分配的库位不可为空');
                  isnull++;
                }else{
                   
                }
            })
          })
          var data={
             orderID:this.detailinfo.OrderID,
             lsnotices:this.reportListnew,
          }
          console.log(isnull)
          if(isnull==0){
             lsnoticeEnter(data).then(res=>{
                console.log(res)
                if(res.val==0){
                  this.$Message.success('保存成功,两秒后自动跳转至通知页面');
                    var _this=this;
                    setTimeout(function(){
                      this.$store.dispatch("setcloseLd", false);
                    },2000)
                }else if(res.val==1){
                  this.$Message.error('保存失败，请核实库位');
                }else if(res.val==2){
                  this.$Message.error('所选择库位已经被使用，请重新添加');
                }
              })
          }else{
             this.$Message.error('分配的库位不可为空');
          }
        
      //  var reportListnew=this.reportListnew;
      //  var isrepeat=true;
      //  var isnull=true;
      //  reportListnew.map((item)=>{
      //     // console.log(item)
      //     var oldshelve=item.ShelveIDs
      //     var newShelveIDs=item.ShelveIDs;
      //    for(var i=0;i<newShelveIDs.length;i++){
      //     //  console.log(newShelveIDs[i])
      //      if(newShelveIDs[i]!=""){
      //         isnull=false;
      //         for(var j=0;j<newShelveIDs.length;j++){
      //           if(newShelveIDs[i]==newShelveIDs[j]){
      //             this.$Message.error('库位不能重复')
      //             isrepeat=true; 
      //           }else{
      //             isrepeat=false; 
      //           }
      //         }
      //      }else{
      //        this.$Message.error('库位不能为空')
            
      //      }
      //    }
      //  })
      //   if(isnull==false&&isrepeat==false){
      //     console.log(reportListnew)
      //   //   lsnoticeEnter(reportListnew).then(res=>{
      //   //   console.log(res)
      //   //   if(res.val==0){
      //   //      this.$Message.success('保存成功,两秒后自动跳转至通知页面');
      //   //       var _this=this;
      //   //       setTimeout(function(){
      //   //          this.$store.dispatch("setcloseLd", false);
      //   //       },2000)
      //   //   }else if(res.val==1){
      //   //     this.$Message.error('保存失败，请核实库位');
      //   //   }else if(res.val==2){
      //   //     this.$Message.error('所选择库位已经被使用，请重新添加');
      //   //   }
      //   // })
      //  }
        
      },
    getdetail() {
      var data1 = {
        warehouseID: sessionStorage.getItem("UserWareHouse"), //库房编号
        orderID: this.ID //订单编号
      };
      lsnoticeDetail(data1).then(res => {
        this.detailinfo = res.obj;
        this.reportListnew= res.obj.LsNotices;
      });
    }
  }
};
</script>
<style>
#LeaseDetail .detailtitle {
  line-height: 24px;
  border-left: 5px solid #2d8cf0;
  font-weight: bold;
  font-size: 16px;
  text-indent: 10px;
}
#LeaseDetail .box {
  width: 100%;
  min-height: 120px;
  background: rgb(245, 247, 249);
  margin: 15px 0px;
  font-size: 14px;
}
#LeaseDetail .subCol>ul>li{
    line-height: 30px;
    padding: 8px 0;
    width: 100%;
    border-bottom: 1px solid #dddddd;
    /* margin:0 -18px; */
}
#LeaseDetail .subCol>ul>li:last-child{
  border-bottom: none;
} 

#LeaseDetail .ivu-table-cell{
  padding: 0;
}
.button_div{
  text-align: center;
  padding:15px 0;
}
 /* #LeaseDetail .subCol>ul>li{
      margin:0 -18px;
      list-style:none;
      text-Align: center;
      padding-top: 9px;
      border-bottom:1px solid #ccc;
      overflow-x: hidden ;
      min-height: 42px;
}
#LeaseDetail .subCol>ul>li:last-child{
  border-bottom: none;
}  */

/* #LeaseDetail .subCol .subColdiv .subColli{
      margin:0 -18px;
      list-style:none;
      text-Align: center;
      padding-top: 9px;
      border-bottom:1px solid #ccc;
      overflow-x: hidden ;
      min-height: 42px;
      padding-bottom: 9px;
}
.ivu-select-selection{
  line-height: 30px;
  width: 80%;
  margin: 0 auto;
}
#LeaseDetail .subCol .subColdiv .subColli:last-child{
  border-bottom: none;
} */
</style>