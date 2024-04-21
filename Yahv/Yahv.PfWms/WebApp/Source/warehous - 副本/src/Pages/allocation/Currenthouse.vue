<style scoped>
ul{
  /* border: 1px solid #dddddd; */
  /* padding-bottom: 40px; */
  border-bottom: 0;
  width: 50%;
  height: auto;
}
.item_li {
  line-height: 50px;
  /* border-bottom: 1px solid #dddddd; */
}
.item_li span{
  margin-right:10px;
}
.left{
  display: inline-block;
  width:80px;
  text-align: center;
  /* border-right: 1px solid #dddd; */
}
.right{
  width: 19%;
  display: inline-block;
  text-align: center
}
.strong{
  font-weight: bold;
  font-size: 14px;
}
</style>
<template>
  <div style="width:100%;height:100%">
    <div>
      <ul>
        <li  class="item_li" v-for="item in config">
          <span class="left">当前库房:</span>
          <Select  v-model="item.Value" style="width:300px" @on-change="changes(item.Value)">
            <Option v-for="item_storehourses in storehourses"  :value="item_storehourses.Value" :key="item_storehourses.Value">{{item_storehourses.Name }}</Option>
            <!-- <Option v-for="item_storehourses in storehourses"  :value="item_storehourses.Name+'/'+item_storehourses.Value" :key="item_storehourses.Name">{{item_storehourses.Value }}</Option> -->
          </Select>
        </li>
      </ul>
       <!-- <ul>
        <li  class="item_li" v-for="item in config2">
          <span class="left">当前库房:</span>
          <Select  v-model="item.aa" style="width:300px" @on-change="changes(item.Value)">
            <Option v-for="item_storehourses in storehourses2"  :value="item_storehourses.Value" :key="item_storehourses.Value">{{item_storehourses.Name }}</Option>
        
          </Select>
        </li>
      </ul> -->
    </div>
  </div>
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import {PageEvent} from "@/js/browser.js"
export default {
  name: "Currenthouse",
  data() {
    return {
      config:[
          {
            "Value":"BJ07/ddddddddddd11"
          }
      ],
      storehourses:[
          {
             "Name1":"北京库房11",
             "Value":"BJ07/ddddddddddd11"
         },
      ],
      config2:[
          {
            "aa":"北京库房11",
            "bb":"BJ07/ddddddddddd11"
          }
      ],
      storehourses2:[
          {
             "aa":"北京库房11",
            "bb":"BJ07/ddddddddddd11"
        },
        {
             "aa":"北京库房1",
            "bb":"BJ07/ddddddddddd1"
        },
        {
             "aa":"北京库房2",
            "bb":"BJ07/ddddddddddd2"
        },
        {
             "aa":"北京库房3",
            "bb":"BJ07/ddddddddddd3"
        },
      ]
    };
  },
  mounted() {
    this.setnva();
    // PageEvent("{\"name\":\"warehousesinit\"}");
    console.log(obj)
    if(obj!=defined){
      this.config=obj.config;
      this.storehourses=obj.storehourses;
    }
    
    // console.log(browser)
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "系统配置",
          href: "/Allocation"
        },
        {
          title: "全部配置",
          href: "/Allocation"
        },
        {
          title: "当前库房配置",
          href: "/Currenthouse"
        }
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    changes(val){
        console.log(val)
        var key="";
        // PageEvent("{\"Name\":\"setwarehouseset\",\"Value\":\"{\\\"Name\\\":\\\"" + key + "\\\",\\\"Value\\\":\\\"" + val + "\\\"}\"}");
    }
  }
};
</script>
<style lang="">
</style>