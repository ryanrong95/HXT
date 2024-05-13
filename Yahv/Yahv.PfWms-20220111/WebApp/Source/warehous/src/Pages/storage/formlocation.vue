<template>
    <div>
        <Form ref="formValidatename" :model="formValidate" :rules="ruleValidate" :label-width="115">
        <FormItem label="所属位置" prop="value1">
            <Cascader :data="formValidate.data" @on-change="setvalues" v-model="formValidate.value1"></Cascader>
        </FormItem>
        <FormItem label="添加库位个数" prop="number">
            <Input v-model="formValidate.number" placeholder=""></Input>
        </FormItem>
        <FormItem label="所有人类型" prop="Ownertype">
            <RadioGroup v-model="formValidate.Ownertype">
                <Radio label="Internalcompany">内部公司</Radio>
                <Radio label="Internalstaff">内部员工</Radio>
                <Radio label="client">客户</Radio>
                <Radio label="supplier">供应商</Radio>
            </RadioGroup>
        </FormItem>
        <FormItem label="所有人" prop="Owner">
            <!-- <Input v-model="formValidate.mail" placeholder=""></Input> -->
            <Select v-model="formValidate.Owner" >
                <Option v-for="item in cityList" :value="item.value1" :key="item.value1">{{ item.label1 }}</Option>
            </Select>
        </FormItem>
         <FormItem label="负责人" prop="Personcharge">
            <Input v-model="formValidate.city" placeholder=""></Input>
        </FormItem>
        <FormItem label="是否可租赁" prop="lease" v-if="formValidate.isleas==true">
            <RadioGroup v-model="formValidate.lease">
                <Radio label="yes">是</Radio>
                <Radio label="no">否</Radio>
            </RadioGroup>
        </FormItem>
        <!-- <FormItem label="是否可添加货架" prop="goods">
            <RadioGroup v-model="formValidate.goods">
                <Radio label="male">是</Radio>
                <Radio label="female">否</Radio>
            </RadioGroup>
        </FormItem> -->
        <FormItem label="用途" prop="interest">
            <CheckboxGroup v-model="formValidate.interest">
                <Checkbox label="通用"></Checkbox>
                <Checkbox label="备货"></Checkbox>
                <Checkbox label="代购"></Checkbox>
                <Checkbox label="暂存"></Checkbox>
                <Checkbox label="待报关"></Checkbox>
            </CheckboxGroup>
        </FormItem>
        <FormItem label="备注" prop="desc">
            <Input v-model="formValidate.desc" type="textarea" :autosize="{minRows: 2,maxRows: 5}" placeholder="Enter something..."></Input>
        </FormItem>
        <FormItem>
            <!-- <Button type="primary" @click="handleSubmit('formValidate')">Submit</Button>
            <Button @click="handleReset('formValidate')" style="margin-left: 8px">Reset</Button> -->
        </FormItem>
    </Form>
    </div>
</template>
<script>
    export default {
        name:"formlocation",
        props:["formValidate","ruleValidate"],
        data () {
            return {
                
                formValidatename:this.formValidate,
                istrue:false,
                model1:"",
                cityList:[
                    {
                        value1: 'beijing',
                        label1: '北京'
                    },
                    {
                        value1: 'shenzhen',
                        label1: '深圳'
                    },
                    {
                        value1: 'xiangang',
                        label1: '香港'
                    },
                    {
                        value1: 'shanghai',
                        label1: '上海'
                    },
                ],
                Ownertype:"",
            }
        },
        methods: {
            handleSubmit () {
                this.$refs.formValidatename.validate((valid) => {
                    if (valid) {
                        this.$Message.success('Success!');
                        this.istrue=true;
                    } else {
                        this.$Message.error('请填写相关信息');
                         this.istrue=false;
                    }
                    console.log(valid)
                })
            },
            handleReset () {
                this.$refs.formValidatename.resetFields();
                // this.$refs.formValidatename.value1="";
                // console.log(this.formValidatename)
            },
            setvalues(value){
                this.formValidatename.value1=value;
                console.log(this.formValidatename.value1)
            }
        }
    }
</script>