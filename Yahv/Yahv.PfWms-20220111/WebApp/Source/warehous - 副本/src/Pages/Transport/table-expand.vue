<style scoped>
    .expand-row{
        margin-bottom: 16px;
    }
.subCol  li{
      margin:0 -18px;
      list-style:none;
      text-Align: center;
      padding-top: 9px;
      padding-bottom: 9px;
      border-bottom:1px solid #ccc;
      overflow-x: hidden;
      overflow: hidden;
      min-height: 37px;
}
.subCol  li:last-child{
  border-bottom: none
}
</style>
<template>
    <div>
       <Table border :columns="columns1" :data="row.BoxesNotices">
           <template slot-scope="{ row, index }" slot="Code">
               {{row.Boxes.Code}}
            </template>
            <template slot-scope="{ row, index }" slot="ClientName">
                {{row.ClientName}}
            </template>
             <template slot-scope="{ row, index }" slot="ClientID">
                 {{row.ClientID}}
            </template>
            <template slot-scope="{ row, index }" slot="PartNumber">
                 <ul class="subCol">
                    <li  v-for="item in row.PickingNotice">
                         <Tooltip :content="item.Product.PartNumber" placement="top" class="PartNumbername">
                            <span class="itemPartNumber" >{{item.Product.PartNumber}}</span>
                         </Tooltip>
                    </li>
                </ul>
            </template>
            <template slot-scope="{ row, index }" slot="Manufacturer">
                 <ul class="subCol">
                    <li  v-for="item in row.PickingNotice">{{item.Product.Manufacturer}}</li>
                </ul>
            </template>
            <template slot-scope="{ row, index }" slot="PackageCase">
                <ul class="subCol">
                    <li  v-for="item in row.PickingNotice">{{item.Product.PackageCase}}</li>
                </ul>
            </template>
            <template slot-scope="{ row, index }" slot="DateCode">
                 <ul class="subCol">
                    <li  v-for="item in row.PickingNotice">{{item.DateCode}}</li>
                </ul>
            </template>
            <template slot-scope="{ row, index }" slot="Quantity">
                <ul class="subCol">
                    <li  v-for="item in row.PickingNotice">{{item.Quantity}}</li>
                </ul>
                <!-- <div v-for="item in row.Productarr">{{item.Quantity}}</div> -->
            </template>
       </Table>
    </div>
</template>
<script>
    export default {
        props: {
            row: Object
        },
        data(){
            return{
                columns1: [
                    {
                        slot: 'Code',
                        title: '箱号',
                        align:'center'
                    },
                    {
                        slot: 'ClientName',
                        title: '客户名称',
                        align:'center'
                    },
                    {
                        slot: 'ClientID',
                        title: '客户编号',
                        align:'center'
                    },
                    {
                        slot: 'PartNumber',
                        title: '型号',
                        align:'center',
                        tooltip:true,
                    },
                    {
                        slot: 'Manufacturer',
                        title: '制造商',
                        align:'center'
                    },
                    {
                        slot: 'PackageCase',
                        title: '封装',
                        align:'center',
                    },
                    {
                        slot: 'DateCode',
                        title: '批次',
                        align:'center'
                    },
                    {
                        slot: 'Quantity',
                        title: '应到小计',
                        align:'center'
                    },
                ]
            }
        }
    };
</script>