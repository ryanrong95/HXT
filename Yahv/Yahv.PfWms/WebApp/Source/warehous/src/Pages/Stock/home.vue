<template>
    <div class="allocation">
      <ul>
          <li class="cart_box">
              <router-link to="/Stock/searchstock" style="width:220px;min-height:200px;display:inline-block;">
                  <div>
                    <div class="imgbox">
                        <img src="../../assets/img/kucun.png" alt="">
                    </div>
                    <div class="infobox">
                        <p>库存查询</p>
                        <div class="detail">
                        
                        </div>
                    </div>
                  </div>
              </router-link>
          </li>
          <li class="cart_box">
              <router-link to="/Stock/Separate" style="width:220px;min-height:200px;display:inline-block;">
                  <div>
                    <div class="imgbox">
                        <img src="../../assets/img/kucun.png" alt="">
                    </div>
                    <div class="infobox">
                        <p>暂存管理</p>
                        <div class="detail">
                        
                        </div>
                    </div>
                  </div>
              </router-link>
          </li>
          <!-- <li class="cart_box">
              <router-link to="/Stock/Unusual" style="width:220px;min-height:200px;display:inline-block;">
                  <div>
                    <div class="imgbox">
                        <img src="../../assets/img/kucun.png" alt="">
                    </div>
                    <div class="infobox">
                        <p>异常到货管理</p>
                        <div class="detail">
                        
                        </div>
                    </div>
                  </div>
              </router-link>
          </li> -->
          <li class="cart_box">
              <router-link to="/Lease" style="width:220px;min-height:200px;display:inline-block;">
                  <div>
                    <div class="imgbox">
                        <img src="../../assets/img/kucun.png" alt="">
                    </div>
                    <div class="infobox">
                        <p>库位分配</p>
                        <div class="detail">
                        
                        </div>
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
                        title:"在库管理",
                        href:"/Stock"
                    },
                    {
                        title:"库存查询",
                        href:"/Stock/searchstock"
                    },
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