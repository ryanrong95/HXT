<template>
    <div>
        <div>
            <!-- <Input v-model="search_data.Customercode" placeholder="客户编号" class="setwidth" /> -->
            <!-- <Input v-model="search_data.ID" placeholder="库存编号" class="setwidth" />
            <Input v-model="search_data.SortingID" placeholder="分拣编号" class="setwidth" /> -->
            <Input v-model.trim="search_data.PartNumber" placeholder="型号" class="setwidth" />
            <!-- <Input v-model.trim="search_data.Catalog" placeholder="品名" class="setwidth" /> -->
            <Input v-model.trim="search_data.Manufacturer" placeholder="品牌" class="setwidth" />
            <!-- <Input v-model="search_data.batch" placeholder="批次" class="setwidth" /> -->
             <DatePicker
                ref="element"
                type="daterange"
                placement="bottom-end"
                placeholder="选择入库时间"
                :editable="false"
                separator="  至  "
                @on-change="changedata"
                style="width:240px;"
                ></DatePicker>
            <Button type="primary" @click="search_btn">查询</Button>
            <Button type="error" @click="empty_btn">清空</Button>

        </div>
        <div style="margin-top:20px;">
            <!-- <Table :columns="columns1" :data="data1"></Table> -->
            <Table :columns="storages_title" :data="storages_data" :loading="loading">
                 <template slot-scope="{ row }" slot="Catalog">
                    <p v-if="row.Product!=null"> {{row.Product.Catalog}}</p>
                 </template>
                 <template slot-scope="{ row }" slot="Manufacturer">
                     <p v-if="row.Product!=null">{{row.Product.Manufacturer}}</p>
                 </template>
                 <template slot-scope="{ row }" slot="PartNumber">
                     <p v-if="row.Product!=null">{{row.Product.PartNumber}}</p>
                 </template>
                 <template slot-scope="{ row }" slot="CreateDate">
                     <p >{{row.CreateDate|showDate}}</p>
                 </template>
            </Table>
        </div>
        <div style="margin-top:20px;float:right">
             <Page ref="pages" :total="total"  @on-change='changepage'  :page-size='10' show-elevator/>
        </div>
    </div>
</template>
<script>
import {storages} from "../../api"
// import moment from "moment"
export default {
    data() {
        return {
            current:1,
            loading:true,
            search_data:{
                warehouseID:sessionStorage.getItem("UserWareHouse"),
                // Customercode:"",//客户编号
                // ID:"",//库存编号
                // SortingID:"",//分拣编号
                PartNumber:"",//型号
                Manufacturer:"",//品牌
                beginDate:"",//开始时间
                endDate:"",//结束时间
                pageIndex:1,
                pageSize:10
            },
            total:0,//分页总数
            // pagedata:{
            //  pageIndex:1,
            //  pageSize:1
            // },
            storages_title:[
                // {
                //     title:"库存编号",
                //     key:"ID",
                // },
                // {
                //     title:"分拣编号",
                //     key:"SortingID",
                // },
                {
                    title:"型号",
                    slot:"PartNumber",
                },
                // {
                //     title:"品名",
                //     slot:"Catalog",
                // },
                {
                    title:"品牌",
                    slot:"Manufacturer",
                },
                
                {
                    title:"数量",
                    key:"Quantity",
                },
                {
                    title: '入库时间',
                    slot: 'CreateDate'
                },
                {
                    title:"所属库位编号",
                    key:"ShelveID",
                },
                {
                    title:"存储状态",
                    key:"StoragesStatusDes",
                }
            ],
            storages_data:[],
            columns1: [
                    {
                        title: '品名',
                        key: 'name'
                    },
                    {
                        title: '品牌',
                        key: 'age'
                    },
                    {
                        title: '型号',
                        key: 'address'
                    },
                    {
                        title: '批次号',
                        key: 'address'
                    },
                    {
                        title: '数量',
                        key: 'address'
                    },
                    {
                        title: '入库时间',
                        key: 'address'
                    },
                    {
                        title: '进价总值',
                        key: 'address'
                    },
                    {
                        title: '报关总值',
                        key: 'address'
                    },
                    {
                        title: '销售单价',
                        key: 'address'
                    },
                    {
                        title: '销售总值',
                        key: 'address'
                    },
                    
                ],
           data1: [
                    {
                        name: 'John Brown',
                        age: 18,
                        address: 'New York No. 1 Lake Park',
                        date: '2016-10-03'
                    },
                    {
                        name: 'Jim Green',
                        age: 24,
                        address: 'London No. 1 Lake Park',
                        date: '2016-10-01'
                    },
                    {
                        name: 'Joe Black',
                        age: 30,
                        address: 'Sydney No. 1 Lake Park',
                        date: '2016-10-02'
                    },
                    {
                        name: 'Jon Snow',
                        age: 26,
                        address: 'Ottawa No. 2 Lake Park',
                        date: '2016-10-04'
                    }
                ]
        }
    },
    mounted(){
         //发起 获取服务器数据的 action
         // this.$store.dispatch("getCityData")
         //<param name="id">库存编号</param>
        /// <param name="warehouseID">库房编号</param>
        /// <param name="sortingID">分拣编号</param>
        /// <param name="productID">产品编号</param>
        /// <param name="partNumber">型号</param>
        /// <param name="catalog">品名</param>
        /// <param name="manufacture">制造商、厂商、</param>
        /// <param name="orderID">订单编号</param>
        /// <param name="beginDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">页大小</param>
          this.setnva()
          this.storages(this.search_data)
    },
   methods:{
       setnva(){
                var cc=[
                   {
                        title:"在库管理",
                        href:"/Stock"
                    },
                    {
                        title:"在库管理",
                        href:"/Stock"
                    },
                ]
                this.$store.dispatch("setnvadata",cc)
        },
        storages(data){  //初始化数据与查询数据
            storages(data).then((res) => {
             if(res.val!=3){
                 if(res.obj.Total!=0){
                 this.storages_data=res.obj.Data;
                 this.total=res.obj.Total;
                }else{
                    this.storages_data=[];
                    this.total=0
                };
                this.loading=false;
             }
             
           })
        },
        changedata(value){  //时间格式 获取开始时间与结束时间
        this.search_data.beginDate=value[0];
        this.search_data.endDate=value[1];
        },
        changepage(value){ //改变页码
         this.loading=true;
         this.search_data.pageIndex=value;
         this.current=1
         this.storages(this.search_data)
        },
        search_btn(){
            if(this.search_data.PartNumber!=''||this.search_data.Manufacturer!=''||this.search_data.beginDate!=''||this.search_data.endDate!=''){
                this.search_data.pageIndex=1;
                this.loading=true;
                this.storages(this.search_data);
                this.$refs['pages'].currentPage = 1;
            }else{
                this.$Message.warning('请输入查询条件');
            }
            
        },
        empty_btn(){
            if(this.search_data.PartNumber!=''||this.search_data.Manufacturer!=''||this.search_data.beginDate!=''||this.search_data.endDate!=''){
                this.search_data={
                    // Customercode:"",//客户编号
                    // ID:"",//库存编号
                    // SortingID:"",//分拣编号
                    PartNumber:"",//型号
                    Manufacturer:"",//品牌
                    beginDate:"",//开始时间
                    endDate:"",//结束时间
                    pageIndex:1,
                    pageSize:10
                };
                this.loading=true;
                this.$refs.element.handleClear()
                this.$refs['pages'].currentPage = 1;
                this.storages(this.search_data);
            }
        },
   },
//    filters:{
//         showDate:function (val) {
//           // console.log(val)
//             if (val != "") {
//               if(val||""){
//                 var b = val.split("(")[1];
//                 var c = b.split(")")[0];
//                 var result = moment(+c).format('YYYY-MM-DD-HH');
//                 return result;
//               }
                
//             }
//         },
//     },
}
</script>
<style scoped>
    .setwidth{
        width:200px;
        margin-right: 10px;
    }
</style>>
