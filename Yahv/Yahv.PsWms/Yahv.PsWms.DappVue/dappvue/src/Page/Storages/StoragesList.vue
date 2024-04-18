<style scoped>
	.Warehousing .searchinputbox {
  display: inline-block;
  width: 16%;
  padding-right: 20px;
}
.Warehousing .secrchinput {
  width: 75%;
}
.Warehousing .tablebox{
    padding-top: 20px;
}
.Warehousing .pages {
  float: right;
  padding-top: 20px;
}
.addinput{
  width: 82%;
}
.lable{
  display: inline-block;
 width: 16%;
  text-align: right;
}
.errorinfo{
	font-size:12px;
	color:#999;
	padding-left:17%;
	margin-top:-6px;
}
.inputerror{
	border-color:1px solid red;
}
.bitian{
  color: red;
}
</style>
<template>
	<div class="Warehousing">
		<div class="serchbox">
			<p class="searchinputbox">
				<span>订单ID：</span>
				<Input v-model="searchdata.OrderID" placeholder="请输入订单ID" class="secrchinput" />
			</p>
			<p class="searchinputbox">
				<span>客户：</span>
				<Input v-model="searchdata.ClientName" placeholder="请输入客户名称" class="secrchinput" />
			</p>
			<p class="searchinputbox">
				<span>库位：</span>
				<Input v-model="searchdata.ShelveCode" placeholder="请输入库位" class="secrchinput" />
			</p>
			<p class="searchinputbox">
				<span>型号：</span>
				<Input v-model="searchdata.Partnumber" placeholder="请输入型号" class="secrchinput" />
			</p>
			<Button type="primary" icon="ios-search" style="margin-right: 10px" @click="searchbtnclick">查询</Button>
			<Button type="error" icon="ios-trash" style="margin-right: 10px" @click="cancelbtn">清空</Button>
		</div>
		<div class="tablebox">
			<Table :columns="columns1" :data="data1" :loading="loading" :max-height="tableHeight" ref="table">
				<!-- <template slot-scope="{ row, index }" slot="Total">
					<span >
					{{row.Mpq}}/{{row.PackageNumber}}/{{row.Total}}</span>
				</template> -->
				<template slot-scope="{ row, index }" slot="ModifyDate">
					<p>{{row.ModifyDate|showDate}}</p>
				</template>
				<template slot-scope="{ row, index }" slot="action">
					<Button type="primary" size="small" @click="printlabel(row)">打印</Button>
					<Button type="primary" size="small" @click="splitStorage(row)" :disabled="row.Total==1">拆分</Button>
				</template>
			</Table>
			<div class="pages">
				<Page :total="total" :page-size="searchdata.PageSize" show-total :current="searchdata.PageIndex" :page-size-opts="showPageArr"
				 @on-page-size-change="changepagesize" @on-change="changepage" show-elevator show-sizer />
			</div>

		</div>
		<!-- 拆分库位 -->
		<Modal v-model="splitShelve" title="拆分库位" @on-visible-change="visiblechangeadd">
			<div>
				<div v-show="columnItem.Mpq>1&&columnItem.PackageNumber>1">
					<p style="padding-bottom:8px">
						<span class="lable"><em class="bitian">*</em>按件拆分：</span>
						<Input v-model="splitStorageData.Quantity" placeholder="请输入数量" class="addinput inputerror" />
					</p>
					<p class="errorinfo">提示：数量最小为1，最大不超过{{columnItem.PackageNumber}}</p>
				</div>
				<div v-show="columnItem.Mpq>1&&columnItem.PackageNumber==1||columnItem.Mpq==1&&columnItem.PackageNumber>=2">
					<p style="padding-bottom:8px">
						<span class="lable"><em class="bitian">*</em>按量拆分：</span>
						<Input v-model="splitStorageData.Quantity" placeholder="请输入数量" class="addinput" />
					</p>
					<p class="errorinfo">提示：数量最小为1，最大不超过{{columnItem.Total}}</p>
				</div>
				<p>
					<span class="lable">备注：</span>
					<Input v-model="splitStorageData.Summary" placeholder="请输入备注" class="addinput" />
				</p>
			</div>
			<div slot="footer">
				<Button type="text" @click="cancel">取消</Button>
				<Button type="primary" @click="ok">确定</Button>
			</div>
			
		</Modal>


	</div>
</template>
<script>
	import {
		Storagelist,
		SplitStorage
	} from "../../api/Storages"
	import {
		TemplatePrint,
		GetPrinterDictionary
	} from "../../js/browser";
	let product_url = require("../../../static/pubilc.dev");
	export default {
		data() {
			
			return {
				loading: true,
				showDrawer: false,
				total: 0,
				searchdata: {
					"OrderID": "", //查询参数，（入库）订单ID
					"ClientName": "", //查询参数，客户名称
					"ShelveCode": "", //查询参数，库位
					"Partnumber": "", //查询参数，型号
					"PageIndex": 1, //分页参数，第几页 
					"PageSize": 20 //分页参数，每页几条数据
				},
				columns1: [
					//  {
					//     type: 'index',
					//     width: 60,
					//     align: 'center'
					// },
					{
						title: "库位",
						key: "ShelveCode",
						align: 'center'
					},
					{
						title: "订单ID",
						key: "OrderID",
						align: 'center',
						width:150
					},
					{
						title: "客户",
						key: "ClientName",
						align: 'center'
					},
					{
						title: "型号",
						key: "Partnumber",
						align: 'center'
					},
					{
						title: "品牌",
						key: "Brand",
						align: 'center'
					},
					{
						title: "封装",
						key: "Package",
						align: 'center'
					},
					{
						title: "批次",
						key: "DateCode",
						align: 'center'
					},
					// {{row.Mpq}}/{{row.PackageNumber}}/{{row.Total}}</span>
					{
						title: "Mpq",
						key: "Mpq",
						align: 'center'
					},
					{
						title: "数量",
						key: "PackageNumber",
						align: 'center'
					},
					{
						title: "总数量",
						key: "Total",
						align: 'center'
					},
					{
						title: "异常",
						key: "Exception",
						align: 'center'
					},
					{
						title: "备注",
						key: "Summary",
						align: 'center'
					},
					{
						slot: "ModifyDate",
						title: "最后更新时间",
						align: 'center'
					},
					{
						slot: "action",
						title: "操作",
						align: 'center'
					},
				],
				data1: [],
				tableHeight: 500,
				printurl: product_url.pfwms,
				splitShelve: false, //要拆分库存的模态框状态
				columnItem: {
					Mpq: 1,
					PackageNumber: 1,
					Total: 0,
				},
				splitStorageData: {
					ID: "",
					//AdminID: "张三",
					// ShelveID: "",
					Quantity: 1,
					Summary: "",
				}, //要拆分的库存数据
				
			};
		},
		computed: {
			showPageArr() {
				return this.$store.state.common.PageArr;
			}
		},
		created() {
			this.searchbtn()

		},
		mounted() {
			this.setnva();
			this.tableHeight = window.innerHeight - this.$refs.table.$el.offsetTop - 140
		},
		methods: {

			setnva() {
				var cc = [{
					title: "库存记录",
					href: "",
				}, ];
				this.$store.dispatch("setnvadata", cc);
			},
			changepagesize(value) {
				this.searchdata.PageSize = value
				this.searchbtn()
			},
			changepage(value) {
				this.searchdata.PageIndex = value
				this.searchbtn()
			},
			searchbtnclick() {
				this.searchdata.PageIndex = 1
				this.searchbtn()
			},
			searchbtn() {
				this.loading = true
				Storagelist(this.searchdata).then(res => {
					this.data1 = res.data.Data
					this.total = res.data.Total
					this.loading = false
				})
			},
			cancelbtn() {
				this.searchdata.OrderID = null;
				this.searchdata.ClientName = null;
				this.searchdata.ShelveCode = null;
				this.searchdata.Partnumber = null;
				this.searchdata.PageIndex = 1
				this.searchbtn()
			},
			printlabel(row) {
				var configs = GetPrinterDictionary();
				var getsetting = configs["库存标签"];
				var str = getsetting.Url;
				var testurl = str.indexOf("http") != -1;
				if (testurl == true) {
					getsetting.Url = getsetting.Url;
				} else {
					getsetting.Url = this.printurl + getsetting.Url;
				}
				var data = {
					setting: getsetting,
					data: [{
						listdata: row
					}]
				};
				TemplatePrint(data);
			},
			splitStorage(row) {
				console.log(row)
				this.splitShelve = true;
				this.splitStorageData.ID = row.ID;
				this.columnItem.Mpq = row.Mpq;
				this.columnItem.PackageNumber = row.PackageNumber;
				this.columnItem.Total = row.Total;
				console.log(this.columnItem.ID)
				console.log(this.columnItem.Mpq)
				console.log(this.columnItem.PackageNumber)
			},
			ok() {

				// var AdminID = sessionStorage.getItem('userID');
				// this.splitStorageData.AdminID = AdminID;
				// console.log(this.splitStorageData.AdminID)

				if (this.splitStorageData.Quantity === '') {
					this.splitShelve = true;
					this.$Message.warning('数量不能为空');
					return false;
				} else if (this.splitStorageData.Quantity < 1) {
					this.splitShelve = true;
					this.$Message.warning('数量不能小于1');
					return false;
				} else if (this.splitStorageData.Quantity > this.columnItem.PackageNumber - 1 && this.columnItem.PackageNumber > 1 ||
					this.splitStorageData.Quantity > this.columnItem.Total - 1 && this.columnItem.PackageNumber == 1) {
					this.splitShelve = true;
					this.$Message.warning('数量不能超过最大数量');
					return false;
				} else {
					SplitStorage(this.splitStorageData).then(res => {
						console.log(res)
						if (res.success == true) {
							this.splitShelve = false;
							this.$Message.success('拆分库存成功');
							this.searchbtn();
						} else {
							this.splitShelve = true;
							this.$Message.error(res.data);
						}

					})
				}
				// if (!this.splitStorageData.Quantity != true) {
				// 	SplitStorage(this.splitStorageData).then(res => {
				// 		console.log(res)
				// 		if (res.success == true) {
				// 			this.$Message.success('拆分库存成功');
				// 		} else {
				// 			this.$Message.error(res.data);
				// 		}

				// 	})
				// } else {
				// 	this.$Message.warning('请输入必填项');
				// }
			},
			cancel() {
				this.splitShelve = false;
			},
			visiblechangeadd(value) {
				// this.splitStorageData.ShelveID = "";
				this.splitStorageData.Quantity = 1;
				this.splitStorageData.Summary = "";
			},
		},
	};
</script>
