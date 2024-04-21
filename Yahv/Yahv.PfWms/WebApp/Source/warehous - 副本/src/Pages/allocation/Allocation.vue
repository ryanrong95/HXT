<template>
    <div class="allocation">
      <ul>
          <li class="cart_box">
              <router-link to="/Allocation/print" style="width:100%;min-height:200px;display:block;">
                <div class="imgbox">
                    <img src="../../assets/img/dayin_icon.png" alt="">
                </div>
                <div class="infobox">
                     <p>打印配置</p>
                     <div>
                       <!-- hfdhfgjdhghd -->
                      </div>
                </div>
              </router-link>
          </li>
      </ul>
    </div>
</template>
<script>
export default {
    name:"allocation",
    data(){
        return{

        }
    },
    mounted(){
         //发起 获取服务器数据的 action
         // this.$store.dispatch("getCityData")
          this.setnva()
    },
   methods:{
       setnva(){
                var cc=[
                   {
                        title:"系统配置",
                        href:"/Allocation"
                    },
                    {
                        title:"全部配置",
                        href:"/Allocation"
                    }
                ]
                this.$store.dispatch("setnvadata",cc)
            },
   }

}
</script>
<style scoped>
    ul{
        margin-top:40px;
    }
    .cart_box img{
        max-width: 60px;
        max-height: 60px;
        position: relative;
        top: 50%;
        margin-top: -30px;
    }
    .cart_box .imgbox{
            width: 80px;
            height: 80px;
            position: absolute;
            top: -40px;
            left: 50%;
            margin-left: -40px;
            background: #cccc;
            border-radius: 50%;
            background: #ffffff;
    }
    .cart_box{
       width:240px;
       border: 1px solid #dddddd;
       text-align: center;
       padding: 10px;
       float: left;
       margin-right: 16px;
       min-height: 200px;
       position: relative;
       /* background:#fafafa; */
    }
    .cart_box p{
        font-size: 14px;
        margin-bottom: 10px;
        font-weight: bold;
    }
    .cart_box a{
        color: #707070;
    }
    .infobox{
        margin-top: 35px;
    }
    .infobox .detail{
        text-align: left;
    }
</style>