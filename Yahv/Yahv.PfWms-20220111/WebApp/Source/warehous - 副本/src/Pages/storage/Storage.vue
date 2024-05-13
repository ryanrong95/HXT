<template>
    <div>
        <ul>
            <li style="width:220px;float:left;margin-right:20px">
               <Card  >
                   <router-link to="/kufang">
                      <div style="text-align:center">
                        <h3>库房配置</h3>
                    </div> 
                   </router-link>
                </Card> 
            </li>
            <!-- <li style="width:220px;float:left;margin-right:20px">
                <Card >
                   <router-link :to="{name:'Areapage',params:{fatherID:'0'}}">
                      <div style="text-align:center">
                        <h3>库区配置</h3>
                    </div> 
                   </router-link>
                </Card>
            </li>
            <li style="width:220px;float:left;margin-right:20px">
                <Card >
                   <router-link :to="{name:'shelves',params:{AreafatherID:'0'}}">
                      <div style="text-align:center">
                        <h3>货架配置</h3>
                    </div> 
                   </router-link>
                </Card>
            </li>
            <li style="width:220px;float:left;margin-right:20px">
                <Card >
                   <router-link :to="{name:'location',params:{locationID:'0'}}">
                      <div style="text-align:center">
                        <h3>库位配置</h3>
                    </div> 
                   </router-link>
                </Card>
            </li> -->
        </ul>
         
        
    </div>
</template>
<script>
export default {
    name:"storage",
    data() {
        return {
            
        }
    },
    mounted(){
        this.setnva()
    },
   methods: {
       setnva(){
                var cc=[
                    {
                        title:"库房配置",
                        href:"/Storage"
                    },
                ]
                this.$store.dispatch("setnvadata",cc)
            }
   },
}
</script>