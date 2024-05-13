<template>
    <div>
        <Form ref="formValidatename" :model="formValidate" :rules="ruleValidate" :label-width="115">
        <FormItem label="库区名称" prop="name">
            <Input v-model="formValidate.name" placeholder="Enter your name"></Input>
        </FormItem>
        <FormItem label="用途" prop="interest">
            <RadioGroup v-model="formValidate.interest">
                <Radio v-for="item in listdata" :label="item.ID" :key="item.ID" >{{item.purpose}}</Radio>
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
  import {GetPurposes,} from "../../api/index"
    export default {
        name:"formarea",
        props:["formValidate","ruleValidate"],
        data () {
            return {
                formValidatename:this.formValidate,
                istrue:false,
                listdata:[],
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
          },
            GetPurposes() {
             GetPurposes("").then(res=>{
               console.log(res)
               this.listdata=res.obj
              })
          },
    },
    mounted(){
          this.GetPurposes()
        }
    }
</script>
