<template>
  <div style="text-align: center">
    <!-- <Row>
      <Col span="24" class="itemcol">
        <div class="Noteisnumber">
          <ul class="backbox">
            <li class="back1 backpub" @click="toitempage(2)">
              <p class="setnumbers">3000</p>
              <p class="setnumbers">出库通知</p>
            </li>
            <li class="back2 backpub" @click="toitempage(1)">
              <p class="setnumbers">100</p>
              <p class="setnumbers">入库通知</p>
            </li>
            <li class="back3 backpub">
              <p class="setnumbers">8</p>
              <p class="setnumbers">在检测</p>
            </li>
            <li class="back4 backpub">
              <p class="setnumbers">50</p>
              <p class="setnumbers">待取货</p>
            </li>
          </ul>
        </div>
        <div class="ItemChart">
          <p
            style="margin: 0px 0px 0px 20px;font-size: 18px;border-left: 6px solid #2487ff;text-indent: 10px;"
          >
            入库工作量统计
            <span>(近七天)</span>
          </p>
          <chart
            ref="chart1"
            :options="orgOptions"
            style="width:80%;height:320px"
            :auto-resize="true"
          ></chart>
        </div>
        <div class="ItemChart">
          <p
            style="margin: 0px 0px 0px 20px;font-size: 18px;border-left: 6px solid #fe5052;text-indent: 10px;"
          >
            入库工作量统计
            <span>(近七天)</span>
          </p>
          <chart
            ref="chart1"
            :options="orgOptions2"
            style="width:80%;height:320px"
            :auto-resize="true"
          ></chart>
        </div>
      </Col>
    </Row>-->
	<!--<div style="text-align:left;width:100%;">
		<span>代仓储</span> <Cascader
		:transfer="true"
		:data="data"
		v-model="value1"
		@on-change="changecascader"
		style="width:200px"
		></Cascader>
		<p v-if="shownumber1==true"><span>数量</span> <Input v-model="number1" @on-blur='changenumber()'  style="width: 300px" /></p>
		<p><span>金额</span> <Input v-model="prices1origin"  style="width: 300px" /></p>
		
	</div>
   
	<br/>
	<div style="text-align:left;width:100%;">
	 <label for="">代报关</label><Cascader
		:transfer="true"
		:data="data2"
		v-model="value2"
		@on-change="changecascader2"
		style="width:200px"
		></Cascader>
		 <Input v-model="prices2"  style="width: 300px" />
	</div>-->
    
    <img src="../../assets/img/home.jpg" alt />
  </div>
  <!-- <div></div> -->
</template>
<script>
import { mapActions, mapGetters } from "vuex";
import { getpfwms } from "../../api"; //引入api 的接口FilePrint
import { TemplatePrint, GetPrinterDictionary } from "@/js/browser.js";
import {
  PFWMS_API
} from "@/main" //调用整体的url
import axios from 'axios'
export default {
  name: "home",
  data() {
    return {
      orgOptions: {},
      orgOptions2: {},
      uploadList: [],
      cc: [
        {
          title: "首页",
          href: "/",
        },
      ],
      datas: [],
      columns1: [
        {
          title: "Name",
          key: "name",
        },
        {
          title: "Age",
          key: "age",
        },
        {
          title: "Address",
          key: "address",
        },
      ],
      data1: [
        {
          name: "John Brown",
          age: 18,
          address: "New York No. 1 Lake Park",
          date: "2016-10-03",
        },
        {
          name: "Jim Green",
          age: 24,
          address: "London No. 1 Lake Park",
          date: "2016-10-01",
        },
        {
          name: "Joe Black",
          age: 30,
          address: "Sydney No. 1 Lake Park",
          date: "2016-10-02",
        },
        {
          name: "Jon Snow",
          age: 26,
          address: "Ottawa No. 2 Lake Park",
          date: "2016-10-04",
        },
      ],
      value1: [],
	  data: [],
	  prices1:'',
	  prices1origin:'',
	  number1:'',
	  shownumber1:false,
      value2: [],
	  data2: [],
	  prices2:''
    };
  },
  computed() {},
  mounted() {
    this.setnva();
    (this.orgOptions = {
      xAxis: {
        type: "category",
        data: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
      },
      yAxis: {
        type: "value",
      },
      series: [
        {
          data: [820, 932, 901, 934, 1290, 1330, 1320],
          type: "line",
          smooth: true,
          itemStyle: {
            normal: {
              color: "#3287fe",
              lineStyle: {
                color: "#3287fe",
              },
            },
          },
        },
      ],
    }),
      (this.orgOptions2 = {
        xAxis: {
          type: "category",
          data: ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"],
        },
        yAxis: {
          type: "value",
        },
        series: [
          {
            data: [1, 2, 3, 4, 5, 6, 7],
            type: "line",
            smooth: true,
          },
        ],
      });
    //  this.drawLine();
    // this.gettestdata()
    // this.getmvcdata()
    // this.getbaseapi()
    // this.getbasemj()
	// this.getpfwms()
	
	// console.log(appData)
		// Declarationjson().then(res=>{
		// 	console.log(res)
		// })
		axios.get(PFWMS_API + '/Print/Declaration.json').then(res=>{
			// console.log(res.data)
			this.data2=res.data
		})
		axios.get(PFWMS_API + '/Print/Sorage.json').then(res=>{
			console.log(res.data)
			this.data=res.data
		})
  },
  methods: {
    setnva() {
      var cc = [
        {
          title: "首页",
          href: "/",
        },
      ];
      this.$store.dispatch("setnvadata", cc);
    },
    toitempage(value) {
      if (value == 1) {
        //入库
        this.$router.push("/Warehousing");
        this.$store.dispatch("setAcrivename", "入库管理");
      } else if (value == 2) {
        //出库
        this.$router.push("/Outgoing");
        this.$store.dispatch("setAcrivename", "出库管理");
      }
    },
    changecascader(value, selectedData) {
		this.prices1='';
		this.number1='';
	  var data=selectedData[selectedData.length-1]
	  if(data.Isquantity==true){
		  if(this.number1!=''){
			  this.prices1=data.prices
			  this.prices1origin=this.number1*data.prices
		  }else{
			  this.prices1=data.prices
			  this.prices1origin=data.prices
		  }
		  this.shownumber1=true
	  }else{
		  this.shownumber1=false
		  this.prices1=data.prices
		  this.prices1origin=data.prices
	  }
	  
	},
	changenumber(){
		console.log(this.number1)
		console.log(this.prices1)
		console.log(this.prices1origin)
		if(this.number1!=''){
		  this.prices1origin=this.number1*this.prices1
		}
		// this.prices1=this.number1*this.prices1
	},
    changecascader2(value, selectedData) {
      console.log(value);
      console.log(selectedData);
    },
  },
};
</script>
<style scoped>
.Navigation {
  height: 52px;
  line-height: 52px;
  width: 200px;
  background: #3287fe;
  font-size: 18px;
  color: #ffffff;
  /* text-align: center; */
  /* border-left:4px solid #3287fe; */
}
.itemcol {
  min-height: 200px;
}
.Noteisnumber {
  min-height: 150px;
  /* border: 1px solid #dddddd; */
}
.Noteisnumber .backbox {
  width: 100%;
  height: 150px;
  display: flex;
  justify-content: space-between;
}
.ItemChart {
  background: #ffffff;
  /* width: 100%; */
  min-height: 340px;
  box-shadow: #f3f3f3 0px 0px 20px 3px;
  margin-top: 20px;
  padding-top: 15px;
  /* box-shadow: 10px 10px 5px #888888; */
}
.Navbox {
  width: 200px;
  height: 100%;
  border-radius: 5px;
  /* background: #ccc; */
  min-height: 300px;
  overflow: hidden;
  box-shadow: #f3f3f3 0px 0px 20px 3px;
}
.navitem {
  width: 80%;
  height: 120px;
  text-align: center;
  margin: 0 auto;
  border-bottom: 1px solid #dddddd;
}
.navitem :hover {
  cursor: pointer;
  color: #3287fe;
}
.icons span {
  display: inline-block;
  font-size: 60px;
}
.backpub {
  width: 240px;
  height: 150px;
  float: left;
  border-radius: 5px;
  overflow: hidden;
  /* margin-left: 20px; */
}
.backpub:hover {
  cursor: pointer;
}
.back1 {
  background: url("../../assets/img/out.png");
  box-shadow: 0px 5px 16px -5px #76c0ff;
}
.back2 {
  background: url("../../assets/img/enter.png");
  box-shadow: 0px 5px 16px -5px #ffb868;
}
.back3 {
  background: url("../../assets/img/jiance.png");
  box-shadow: 0px 5px 16px -5px #fd6d75;
}
.back4 {
  background: url("../../assets/img/quhuo.png");
  box-shadow: 0px 5px 16px -5px #18e5d5;
}
.setnumbers {
  color: #ffffff;
  margin-left: 25px;
}
.backpub p:nth-child(1) {
  font-size: 36px;
  padding-top: 32px;
  padding-bottom: 28px;
  font-weight: bold;
}
.backpub p:last-child {
  font-size: 18px;
}
</style>

