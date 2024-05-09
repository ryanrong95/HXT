<style scoped>
    .oplogtitle{
        position: relative;top:-27px;width:200px
    }
    .page{
         padding-top: 15px;
         text-align: right;
    }
</style>
<template>
    <div>
        <h1 class="oplogtitle">操作日志</h1>
        <div>
            <Table stripe :columns="columns1" :data="data1"></Table>
        </div>
        <div class="page">
            <Page :total="100" show-elevator />
        </div>
    </div>
</template>
<script>
    export default {
        data () {
            return {
                columns1: [
                    {
                        title: '操作时间',
                        key: 'name'
                    },
                    {
                        title: '操作人',
                        key: 'age'
                    },
                    {
                        title: '操作类型',
                        key: 'address'
                    },
                    {
                        title: '描述',
                        key: 'address'
                    }
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
                    
                ]
            }
        },
        mounted() {
            console.log("dasdkasdpasdop")
        },
    }
</script>