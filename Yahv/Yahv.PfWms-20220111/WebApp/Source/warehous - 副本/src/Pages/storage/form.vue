<template>
    <div>
        <Form ref="formValidatename" :model="formValidate" :rules="ruleValidate" :label-width="110">
        <FormItem label="库房名称" prop="name">
            <Input v-model="formValidate.name" placeholder=""></Input>
        </FormItem>
        <FormItem label="所属地区" prop="RegionCode">
            <Select v-model="formValidate.RegionCode" >
                <Option v-for="item in regionMsg" :value="item.Key" :key="item.Key">{{ item.Value }}</Option>
            </Select>
        </FormItem>
         <FormItem label="所属Crm库房" prop="CrmWarehouse">
            <Select v-model="formValidate.CrmWarehouse" >
                <Option v-for="item in crmWarehouseMsg" :value="item.ID" :key="item.ID">{{ item.Name }}</Option>
            </Select>
        </FormItem>
        <FormItem label="库房地址" prop="Address">
            <Input v-model="formValidate.Address" placeholder=""></Input>
        </FormItem>
        <FormItem label="负责人" prop="Personcharge">
            <Select v-model="formValidate.Personcharge" 
            filterable
            :remote-method="GetAdmin"
            >
                <Option v-for="item in managerMsg" :value="item.ID" :key="item.ID">{{ item.RealName }}</Option>
            </Select>
            <!-- <Input v-model="formValidate.Personcharge" placeholder=""></Input> -->
        </FormItem>
        <FormItem label="所有人类型" prop="Ownertype">
           <RadioGroup v-model="formValidate.Ownertype" @on-change="changeOwner">
                <!-- <Radio label="Internalcompany">内部公司</Radio>
                <Radio label="Internalstaff">内部员工</Radio> -->
                <Radio :label="1" >客户</Radio>
                <Radio :label="2">供应商</Radio>
          </RadioGroup>
        </FormItem>
        <!-- <FormItem label="所有人" prop="Ownerinpit">
              <Select v-model="formValidate.Ownerinpit" 
              filterable
              remote
             :remote-method="GetEnterprise"
             
            >
                <Option v-for="item in enterpriseMsg" :value="item.Name" :key="item.Name">{{ item.Name }}</Option>
            </Select>
        </FormItem> -->
        <FormItem label="所有人" >
              <Select v-model="modelall"
              filterable
              :remote-method="GetEnterprise"
            >
                <Option v-for="item in cityList" :value="item.value" :key="item.value" :label="item.label">
                    <span>{{item.label}}</span>
                    <span style="float:right;color:#ccc">{{item.type}}</span>
                </Option>
            </Select>
        </FormItem>
         <FormItem label="所属业务员" prop="ResponsePerson">
              <Select v-model="formValidate.ResponsePerson" 
              filterable
             :remote-method="GetAdmin">
                <Option v-for="item in managerMsg" :value="item.ID" :key="item.ID">{{ item.RealName }}</Option>
            </Select>
             <!-- <Input v-model="formValidate.Ownerinpit" placeholder=""></Input> -->
        </FormItem>
        
        <FormItem label="具备功能" prop="interest">
            <CheckboxGroup v-model="formValidate.interest">
                <Checkbox v-for="item in abilitiesMsg" :key='item.Key' :label="item.Key">{{item.Value}}</Checkbox>
            </CheckboxGroup>
        </FormItem>
        <FormItem label="是否可添加库区" prop="lease" >
            <RadioGroup v-model="formValidate.lease">
                <Radio label='true'>是</Radio>
                <Radio label='false'>否</Radio>
            </RadioGroup>
        </FormItem>
        <FormItem label="备注" prop="desc">
            <Input v-model="formValidate.desc" type="textarea" :autosize="{minRows: 2,maxRows: 5}" placeholder="Enter something..."></Input>
        </FormItem>
        <FormItem>

        </FormItem>
    </Form>
    </div>
</template>
<script>
import {} from "../../api/index"; //引入api 的接口
    export default {
        name:"formall",
        props:["formValidate","ruleValidate"],//接收父组件传递过来的参数
        data () {
            return {
                regionMsg:[],//地区简码
                abilitiesMsg:[],
                managerMsg:[],
                enterpriseMsg:[],
                crmWarehouseMsg:[],
                searchAdmin:{
                    topnum:8
                },
                formValidatename:this.formValidate,
                istrue:false,
                Ownerlist:[
                    {
                        value1: 'beijing',
                        label1: '客户'
                    },
                    {
                        value1: 'shenzhen',
                        label1: '员工'
                    },
                    {
                        value1: 'xiangang',
                        label1: '供应商'
                    },
                    {
                        value1: 'shanghai',
                        label1: '公司'
                    },
                ],
                cityList: [
                    {
                        value: '1',
                        label: 'New York',
                        type:"客户"
                    },
                    {
                        value: '2',
                        label: 'London',
                        type:"供应商"
                    },
                    {
                        value: '3',
                        label: 'Sydney',
                        type:"客户"
                    },
                    {
                        value: '7',
                        label: 'Sydney',
                        type:"供应商"
                    },
                    {
                        value: '4',
                        label: 'Ottawa',
                        type:"供应商"
                    },
                    {
                        value: '5',
                        label: 'Paris',
                        type:"客户"
                    },
                    {
                        value: '6',
                        label: 'Canberra',
                        type:"供应商"
                    }
                ],
                modelall: '',
                model12: []
            }
        },
        methods: {
            handleSubmit () {
                // console.log(this.formValidatename)
                this.$refs.formValidatename.validate((valid) => {
                    // console.log(valid)
                    if (valid) {
                        // this.$Message.success('Success!');
                        this.istrue=true;
                    } else {
                        this.$Message.error('请填写相关信息');
                         this.istrue=false;
                    }
                    // console.log(valid)
                })
            },
            changeOwner(value){
                // console.log(this.formValidatename.Ownertype);
                // this.formValidatename.Ownerinpit=""
                this.GetEnterprise();
            },
            handleReset () {
                this.$refs.formValidatename.resetFields();
            },
            getSubWarehouse(){
                getSubWarehouse().then(res=>{
                   this.regionMsg=res.obj.regionMsg;
                   this.abilitiesMsg=res.obj.abilitiesMsg;
                //    console.log(res.obj)
                })
             },
             GetAdmin(query){
                //  console.log(this.searchAdmin.name)
                //  console.log(this.searchAdmin.topnum)
                 GetAdmin(query,this.searchAdmin.topnum).then(res=>{
                    this.managerMsg=res;   
                    // console.log(res);
                 })
             },
             GetEnterprise(query){
                 console.log(query)
                 if(query==undefined){
                     query="";
                 }else{
                     this.formValidatename.Ownerinpit=query;
                 }
                //  console.log(this.formValidatename.Ownerinpit)
                  GetEnterprise(this.formValidatename.Ownertype,query,5).then(res=>{
                    this.enterpriseMsg=res; 
                    // if(res.length==0){
                    //     // this.$Message.error('暂无搜索条件');
                    //     // this.formValidatename.Ownerinpit=""
                    // }else{

                    // }
                    // console.log(res);
                 })
             },
             GetCrmWarehouse(){
                 GetCrmWarehouse().then(res=>{
                     this.crmWarehouseMsg=res;
                    //  console.log(res);
                 })
             }
        },
        mounted() {
            this.getSubWarehouse();
            // this.GetAdmin("");
            // //this.GetEnterprise("");
            // this.GetCrmWarehouse();
        },
    }
</script>