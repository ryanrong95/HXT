<template>
    <div>
        <Form ref="formValidatename" :model="formValidate" :rules="ruleValidate" :label-width="115">
        <FormItem label="库房名称" prop="name">
            <Input v-model="formValidate.name" placeholder="请输入逻辑库房名称"></Input>
        </FormItem>
        <FormItem label="所属库房" prop="parents">
            <!-- <Input v-model="formValidate.mail" placeholder=""></Input> -->
            <Select v-model="formValidate.parents" >
                <Option v-for="item in warehouselist" :value="item.value1" :key="item.value1">{{ item.label1 }}</Option>
            </Select>
        </FormItem>
        <!-- <FormItem label="所有人类型" prop="Ownertype">
           <RadioGroup v-model="formValidate.Ownertype">
                <Radio label="Internalcompany">内部公司</Radio>
                <Radio label="Internalstaff">内部员工</Radio>
                <Radio label="client">客户</Radio>
                <Radio label="supplier">供应商</Radio>
          </RadioGroup>
        </FormItem> -->
        <FormItem label="所有人" prop="Owner">
            <!-- <Input v-model="formValidate.mail" placeholder=""></Input> -->
            <Select v-model="formValidate.Owner" >
                <Option v-for="item in Ownerlist" :value="item.value1" :key="item.value1">{{ item.label1 }}</Option>
            </Select>
        </FormItem>
        <!-- <FormItem label="所有人" prop="mail">
            <Input v-model="formValidate.mail" placeholder="Enter your e-mail"></Input>
        </FormItem> -->
         <FormItem label="负责人" prop="mail">
            <Input v-model="formValidate.mail" placeholder="Enter your e-mail"></Input>
        </FormItem>
        <FormItem label="是否可租赁" prop="gender">
            <RadioGroup v-model="formValidate.gender">
                <Radio label="male">是</Radio>
                <Radio label="female">否</Radio>
            </RadioGroup>
        </FormItem>
        <FormItem label="是否可添加货架" prop="goods">
            <RadioGroup v-model="formValidate.goods">
                <Radio label="male">是</Radio>
                <Radio label="female">否</Radio>
            </RadioGroup>
        </FormItem>
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
        name:"formarea",
        props:["formValidate","ruleValidate"],
        data () {
            return {
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
                warehouselist:[
                    {
                        value1:"beijing",
                        label1:"北京库房"
                    },
                    {
                        value1:"shenzhen",
                        label1:"深圳库房"
                    },
                    {
                        value1:"shanghai",
                        label1:"上海库房"
                    },
                ]
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
            }
        }
    }
</script>