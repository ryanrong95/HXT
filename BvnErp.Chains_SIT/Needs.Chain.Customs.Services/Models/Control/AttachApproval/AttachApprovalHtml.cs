using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public partial class AttachApproval
    {
        #region 为“重新生成对账单”生成参考信息Html

        /// <summary>
        /// 为“重新生成对账单”生成参考信息Html
        /// </summary>
        /// <param name="mainOrderID"></param>
        /// <returns></returns>
        public string GetReferenceInfoHtmlForGenerateBill(string mainOrderID)
        {
            decimal subtotalPrice = 0, subtotalCNYPrice = 0;

            StringBuilder sbReferenceInfo = new StringBuilder();

            var bill = getModel(mainOrderID);

            string IsHistory = "0";

            sbReferenceInfo.Append("<div id='splitOrderBills' style='margin-left: 1%; width: 98%; margin-top: 15px; '>");

            if (bill != null && bill.Bills != null && bill.Bills.Any())
            {
                for (var i = 0; i < bill.Bills.Count; i++)
                {
                    var item = bill.Bills[i];
                    var str = "";
                    str += "<table id='products'" + i + " class='border-table' style='margin-top: 5px'>";
                    //if (bill['ContrNo'] == "")
                    {
                        str += "<tr><td class='content' style='text-align:left' colspan='7'>订单编号：" + item.OrderID + "</td>";
                    }
                    //else
                    //{
                    //    str += "<td class='content' style='text-align:left' colspan='7'>订单编号：" + item.OrderID + " 合同号：" + item['ContrNo'] + "</td>";
                    //}
                    str += "<td class='content' style='text-align:left' colspan='7'>实时汇率：" + item.RealExchangeRate + " 海关汇率：" + item.CustomsExchangeRate + "</td></tr>";
                    str += "<tr style='background-color: whitesmoke'>";
                    str += "<th style='width: 5%;'>序号</th>";
                    str += "<th style='width: 10%; text-align: left'>报关品名</th>";
                    str += "<th style='width: 10%; text-align: left'>规格型号</th>";
                    str += "<th style='width: 6%;'>数量</th>";
                    str += "<th style='width: 7%;'>报关单价(" + bill.Currency + ")</th>";
                    str += "<th style='width: 7%;'>报关总价(" + bill.Currency + ")</th>";
                    str += "<th style='width: 7%;'>关税率</th>";
                    str += "<th style='width: 7%;'>报关货值(CNY)</th>";
                    str += "<th style='width: 6%;'>关税(CNY)</th>";
                    str += "<th style='width: 7%;'>增值税(CNY)</th>";
                    str += "<th style='width: 7%;'>代理费(CNY)</th>";
                    str += "<th style='width: 6%;'>杂费(CNY)</th>";
                    str += "<th style='width: 7%;'>税费合计(CNY)</th>";
                    str += "<th style='width: 8%;'>报关总金额(CNY)</th>";
                    str += "</tr>";

                    var totalPrice = (item.totalCNYPrice + item.totalTraiff + item.totalAddedValueTax + item.totalAgencyFee + item.totalIncidentalFee)?.ToRound(2);
                    var tariffIsZero = false;
                    var addedValueTaxZero = false;
                    if (item.totalTraiff == 0)
                    {
                        tariffIsZero = true;
                    }
                    if (item.totalAddedValueTax == 0)
                    {
                        addedValueTaxZero = true;
                    }
                    str += InitProducts(item.Products, IsHistory, item.OrderType, item.AgencyFee, totalPrice, tariffIsZero, addedValueTaxZero, out subtotalPrice, out subtotalCNYPrice);
                    str += "</table>";
                    sbReferenceInfo.Append(str);
                }
            }

            sbReferenceInfo.Append("</div>");

            //费用合计明细 Begin

            sbReferenceInfo.Append("<table id='subTotal' title='费用合计明细' class='border-table' style='margin-left: 1%; width: 98%; margin-top: 5px; '><tbody>");

            string subTotalStr = "<tr><td class='content' style='text-align:right;width:80%'>货值小计</td><td style='text-align:left;width:20%'>" + bill.Currency + " " + subtotalPrice.ToRound(2) + "<br/>CNY " + subtotalCNYPrice.ToRound(2) + "</td></tr>" +
                "<tr><td class='content' style='text-align:right;width:80%'>税代费小计</td><td style='text-align:left;width:20%'>CNY " + bill.summaryPay?.ToRound(2) + "</td></tr>" +
                "<tr><td class='content' style='text-align:right;width:80%'>应收总金额合计</td><td style='text-align:left;width:20%'>CNY " + bill.summaryPayAmount?.ToRound(2) + "</td></tr>";

            sbReferenceInfo.Append(subTotalStr);

            sbReferenceInfo.Append("</tbody></table>");

            //费用合计明细 End

            string html = sbReferenceInfo.ToString();
            html = html.Replace("'", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");

            if (!string.IsNullOrEmpty(html))
            {
                html = html.Trim();
            }

            this.ReferenceInfo = html;

            return html;
        }

        //报关商品明细
        private string InitProducts(
            List<MainOrderBillItemProduct> data, 
            string IsHistory, 
            OrderType orderType, 
            decimal AgencyFee, 
            decimal? baoguanzonghuozhi, 
            bool tariffIsZero, 
            bool addedValueTaxZero,
            out decimal subtotalPriceOut,
            out decimal subtotalCNYPriceOut)
        {
            decimal subtotalQty = 0;
            decimal subtotalPrice = 0, subtotalCNYPrice = 0;

            decimal subtotalAgencyFee = 0, subtotalIncidentalFee = 0;
            decimal subtotalTraiff = 0, subtotalAddedValueTax = 0;
            decimal subtotalTaxFee = 0, subtotalAmount = 0;


            string replaceQuotes = "这里是一个双引号";
            string replaceSingleQuotes = "这里是一个单引号";

            var str = "";
            decimal totalQty = 0;
            decimal totalPrice = 0;
            decimal totalCNYPrice = 0;
            decimal totalAgencyFee = 0;
            decimal totalIncidentalFee = 0;
            decimal totalTraiff = 0;
            decimal totalAddedValueTax = 0;
            decimal totalTaxFee = 0;
            decimal totalAmount = 0;

            for (var index = 0; index < data.Count; index++)
            {
                var row = data[index];
                var count = index + 1;

                if (IsHistory == "1")
                {

                    //拼接表格的行和列
                    str += "<tr><td>" + count + "</td><td style='text-align:left'>" + row.ProductName + "</td><td style='text-align:left'>" +
                        row.Model.Replace(replaceQuotes, "\"").Replace(replaceSingleQuotes, "\'") + "</td>" +
                        "<td>" + row.Quantity.ToString("0.####") + "</td><td>" + row.UnitPrice.ToRound(4) + "</td>" + "<td>" + row.TotalPrice.ToRound(2) + "</td>" +
                        "<td>" + row.TariffRate.ToRound(4) + "</td><td>" + row.TotalCNYPrice.ToRound(2) + " </td>" +
                        "<td>" + row.Traiff?.ToRound(2) + "</td><td>" + row.AddedValueTax?.ToRound(2) + " </td>" +
                        "<td>" + row.AgencyFee.ToRound(2) + "</td><td>" + row.IncidentalFee.ToRound(2) + "</td>" +
                        "<td>" + (row.Traiff?.ToRound(2) + row.AddedValueTax?.ToRound(2) + row.AgencyFee.ToRound(2) + row.IncidentalFee.ToRound(2))?.ToRound(2) + "</td>" +
                        "<td>" + (row.TotalCNYPrice + row.Traiff?.ToRound(2) + row.AddedValueTax?.ToRound(2) + row.AgencyFee.ToRound(2) + row.IncidentalFee.ToRound(2))?.ToRound(2) + "</td></tr>";


                    totalQty += row.Quantity;
                    totalPrice += row.TotalPrice;
                    totalCNYPrice += row.TotalCNYPrice.ToRound(2);
                    totalTraiff += row.Traiff?.ToRound(2) ?? 0;
                    totalAddedValueTax += row.AddedValueTax?.ToRound(2) ?? 0;
                    
                    totalIncidentalFee += row.IncidentalFee;

                }
                else
                {
                    //拼接表格的行和列
                    str += "<tr><td>" + count + "</td><td style='text-align:left'>" + row.ProductName + "</td><td style='text-align:left'>" +
                        row.Model.Replace(replaceQuotes, "\"").Replace(replaceSingleQuotes, "\'") + "</td>" +
                        "<td>" + row.Quantity.ToString("0.####") + "</td><td>" + row.UnitPrice.ToRound(4) + "</td>" + "<td>" + row.TotalPrice.ToRound(2) + "</td>" +
                        "<td>" + row.TariffRate.ToRound(4) + "</td><td>" + row.TotalCNYPrice.ToRound(2) + "</td>" +
                        "<td>" + row.Traiff?.ToRound(2) + " </td><td>" + row.AddedValueTax?.ToRound(2) + " </td>" +
                        "<td>" + row.AgencyFee.ToRound(2) + " </td><td>" + row.IncidentalFee.ToRound(2) + " </td>" +
                        "<td>" + (row.Traiff + row.AddedValueTax + row.AgencyFee + row.IncidentalFee)?.ToRound(2) + " </td>" +
                        "<td>" + (row.TotalCNYPrice + row.Traiff + row.AddedValueTax + row.AgencyFee + row.IncidentalFee)?.ToRound(2) + " </td></tr>";

                    //统计合计信息
                    totalQty += row.Quantity;
                    totalPrice += row.TotalPrice;
                    totalCNYPrice += row.TotalCNYPrice;
                    totalTraiff += row.Traiff ?? 0;
                    totalAddedValueTax += row.AddedValueTax ?? 0;
                    
                    totalIncidentalFee += row.IncidentalFee;
                }
            }

            if (tariffIsZero)
            {
                totalTraiff = 0;
            }
            if (addedValueTaxZero)
            {
                totalAddedValueTax = 0;
            }

            subtotalQty += totalQty;
            subtotalPrice += totalPrice;
            subtotalCNYPrice += totalCNYPrice;
            subtotalTraiff += totalTraiff;
            subtotalAddedValueTax += totalAddedValueTax;
            
            subtotalIncidentalFee += totalIncidentalFee;


            if (IsHistory == "1")
            {
                totalAgencyFee = AgencyFee.ToRound(2);
                totalTaxFee = totalTraiff.ToRound(2) + totalAddedValueTax.ToRound(2) + totalAgencyFee + totalIncidentalFee.ToRound(2);
                totalAmount = totalCNYPrice + totalTraiff.ToRound(2) + totalAddedValueTax.ToRound(2) + totalAgencyFee
                    + totalIncidentalFee.ToRound(2);
                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += "<tr><td colspan='3'>合计：</td>" +
                    "<td>" + totalQty.ToString("0.####") + "</td><td></td><td>" + totalPrice.ToRound(2) + "</td><td></td><td>" + totalCNYPrice.ToRound(2) + " </td>" +
                    "<td>" + totalTraiff.ToRound(2) + " </td><td>" + totalAddedValueTax.ToRound(2) + "</td>" +
                    "<td>" + totalAgencyFee.ToRound(2) + " </td><td>" + totalIncidentalFee.ToRound(2) + "</td>" +
                    "<td>" + totalTaxFee.ToRound(2) + " </td>" +
                    "<td>" + totalAmount.ToRound(2) + " </td></tr>";
            }
            else
            {
                totalAgencyFee = AgencyFee.ToRound(2);
                subtotalAgencyFee += totalAgencyFee;
                totalTaxFee = totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;
                totalAmount = totalCNYPrice + totalTraiff + totalAddedValueTax + totalAgencyFee + totalIncidentalFee;

                subtotalTaxFee += totalTaxFee;
                subtotalAmount += totalAmount;
                str += "<tr><td colspan='3'>合计：</td>" +
                    "<td>" + totalQty.ToString("0.####") + "</td><td></td><td>" + totalPrice.ToRound(2) + "</td><td></td><td>" + totalCNYPrice.ToRound(2) + "</td>" +
                    "<td>" + totalTraiff.ToRound(2) + " </td><td>" + totalAddedValueTax.ToRound(2) + "</td>" +
                    "<td>" + totalAgencyFee.ToRound(2) + " </td><td>" + totalIncidentalFee.ToRound(2) + "</td>" +
                    "<td>" + totalTaxFee.ToRound(2) + " </td>" +
                    "<td>" + baoguanzonghuozhi + " </td></tr>";
            }


            subtotalPriceOut = subtotalPrice;
            subtotalCNYPriceOut = subtotalCNYPrice;

            return str;
        }

        protected MainOrderBillViewModel getModel(string id)
        {
            var viewModel = new MainOrderBillViewModel();
            var model = getModelStander(id);
            if (model == null)
            {
                return null;
            }
            else
            {
                #region 两个Model 转换              
                viewModel.MainOrderID = id;

                viewModel.Bills = model.Bills;

                var purchaser = PurchaserContext.Current;
                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();

                viewModel.ClientName = model.OrderBill.Client.Company.Name;
                viewModel.ClientTel = model.OrderBill.Client.Company.Contact.Tel;
                viewModel.Currency = model.OrderBill.Currency;
                viewModel.IsLoan = model.OrderBill.IsLoan;
                viewModel.DueDate = model.OrderBill.GetDueDate().ToString("yyyy年MM月dd日");
                viewModel.CreateDate = model.OrderBill.CreateDate.ToString();
                viewModel.ClientType = model.OrderBill.Client.ClientType;

                var OrderBillFile = model.OrderBillFile;

                viewModel.FileID = OrderBillFile?.ID;
                viewModel.FileStatus = OrderBillFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                                        OrderBillFile.FileStatus.GetDescription();
                viewModel.FileName = OrderBillFile == null ? "" : OrderBillFile.Name;
                viewModel.Url = OrderBillFile == null ? "" : OrderBillFile.Url;
                //viewModel.FileStatusValue = OrderBillFile == null ? Needs.Wl.Models.Enums.OrderFileStatus.NotUpload : OrderBillFile.FileStatus;

                viewModel.Url = FileDirectory.Current.FileServerUrl + "/" + OrderBillFile?.Url.ToUrl();
                viewModel.summaryTotalPrice = model.BillTotalPrice;
                viewModel.summaryTotalCNYPrice = model.BillTotalCNYPrice;
                viewModel.summaryTotalTariff = model.BillTotalTariff;
                viewModel.summaryTotalAddedValueTax = model.BillTotalAddedValueTax;
                viewModel.summaryTotalAgencyFee = model.BillTotalAgencyFee;
                viewModel.summaryTotalIncidentalFee = model.BillTotalIncidentalFee;

                viewModel.summaryPay = model.BillTotalTaxAndFee;
                viewModel.summaryPayAmount = model.BillTotalDeclarePrice;


                viewModel.CreateDate = model.MainOrder.CreateDate.ToString("yyyy-MM-dd HH:mm");
                #endregion

                return viewModel;
            }
        }

        private MainOrderBillStander getModelStander(string id)
        {
            var Orders = new Views.Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                  && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                         .ToList();

            var purchaser = PurchaserContext.Current;
            if (Orders.Count == 0)
            {
                return null;
            }
            else
            {
                MainOrderBillStander mainOrderBillStander = new MainOrderBillStander(purchaser, Orders);

                return mainOrderBillStander;
            }
        }

        #endregion

        #region 为“删除型号、修改数量”生成参考信息Html

        public string GetReferenceInfoHtmlForDeleteModelAndChangeModel(string tinyOrderID)
        {
            var order = new Needs.Ccs.Services.Views.OrdersView()[tinyOrderID];

            Needs.Ccs.Services.Views.OrderDetailOrderItemsView view = new Needs.Ccs.Services.Views.OrderDetailOrderItemsView(order.ID, order.Type);
            view.AllowPaging = false;
            IList<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list = view.ToList();

            CheckOrderItemIsShowModifyBtn(order, list, view);

            Func<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel, object> convert = item => new
            {
                ID = item.OrderItemID,
                Name = !string.IsNullOrEmpty(item.OrderItemCategoryName) ? item.OrderItemCategoryName : item.OrderItemName,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice.ToString("0.0000"),
                TotalPrice = item.TotalPrice.ToRound(2).ToString("0.00"),
                Unit = item.Unit,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight?.ToString("0.0000"),
                IsShowModifyBtn = item.IsShowModifyBtn,
                NotShowReason = item.NotShowReason,
            };

            var rows = list.Select(convert).ToArray();

            string html = GenerateHtmlStrForDeleteChange(rows);
            html = html.Replace("'", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");

            if (!string.IsNullOrEmpty(html))
            {
                html = html.Trim();
            }

            this.ReferenceInfo = html;

            return html;
        }

        private void CheckOrderItemIsShowModifyBtn(
            Needs.Ccs.Services.Models.Order order,
            IList<Needs.Ccs.Services.Views.OrderDetailOrderItemsModel> list,
            Needs.Ccs.Services.Views.OrderDetailOrderItemsView view)
        {
            if (order.OrderStatus == OrderStatus.Draft)
            {
                //订单状态草稿不能操作
                foreach (var item in list)
                {
                    item.IsShowModifyBtn = false;
                    item.NotShowReason = "订单状态：草稿";
                }

                return;
            }

            if (order.Type == OrderType.Icgoo || order.Type == OrderType.Inside)
            {
                bool decHeadIsSuccess = view.GetDecHeadIsSuccess();
                if (decHeadIsSuccess)
                {
                    //1. 报关完成不能操作
                    foreach (var item in list)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "已报关完成";
                    }

                    return;
                }
            }
            else if (order.Type == OrderType.Outside)
            {
                Needs.Ccs.Services.Enums.EntryNoticeStatus hkEntryNoticeStatus = view.GetHkEntryNoticeStatus();
                if (hkEntryNoticeStatus == Needs.Ccs.Services.Enums.EntryNoticeStatus.Sealed)
                {
                    //1.订单已封箱，不能操作
                    foreach (var item in list)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "订单已封箱";
                    }

                    return;
                }
            }


            if (order.Type == OrderType.Outside)
            {
                //2. 型号已装箱，不能操作
                foreach (var item in list)
                {
                    if (item.IsHkPacked)
                    {
                        item.IsShowModifyBtn = false;
                        item.NotShowReason = "型号已装箱";
                    }
                }
            }
        }

        private string GenerateHtmlStrForDeleteChange(object[] rows)
        {
            int rowsNumber = 0;
            if (rows != null && rows.Any())
            {
                rowsNumber = rows.Length;
            }

            StringBuilder sbReferenceInfo = new StringBuilder();

            sbReferenceInfo.AppendFormat(@"
<div id='reference-info' style='height: 363px; overflow: auto; margin-top: 10px; '>
          <div class='panel datagrid easyui-fluid' style='width: 1598px;'>
		<div class='panel-header panel-header-noborder' style='width: 1588px;'>
			<div class='panel-title'>产品信息</div>
			<div class='panel-tool'/>
		</div>
		<div class='datagrid-wrap panel-body panel-body-noborder' title='' style='width: 1598px;'>
			<div class='datagrid-view' style='width: 1598px; height: {0}px;'>
				<div class='datagrid-view1' style='width: 31px;'>
					<div class='datagrid-header' style='width: 31px; height: 24px;'>
						<div class='datagrid-header-inner' style='display: block;'>
							<table class='datagrid-htable' border='0' cellspacing='0' cellpadding='0' style='height: 25px;'>
								<tbody>
									<tr class='datagrid-header-row'>
										<td rowspan = '0' >

                                            <div class='datagrid-header-rownumber'/>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</div>
					<div class='datagrid-body' style='width: 31px; margin-top: 0px; height: {1}px;'>
						<div class='datagrid-body-inner'>
							<table class='datagrid-btable' cellspacing='0' cellpadding='0' border='0'>
								<tbody>", rowsNumber * 25 + 25, rowsNumber * 25);

            if (rows != null && rows.Any())
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    sbReferenceInfo.AppendFormat(@"
                        <tr id='datagrid-row-r1-1-{0}' datagrid-row-index='{0}' class='datagrid-row' style='height: 25px; '>
                                        <td class='datagrid-td-rownumber'>
		                        <div class='datagrid-cell-rownumber'>{1}</div>
	                        </td>
                        </tr>", i, i + 1);
                }
            }

            sbReferenceInfo.AppendFormat(@"
                                </tbody>
							</table>
						</div>
					</div>
					<div class='datagrid-footer' style='width: 31px; '>
                           <div class='datagrid-footer-inner' style='display: none;'/>
					</div>
				</div>
				<div class='datagrid-view2' style='width: 1567px;'>
					<div class='datagrid-header' style='width: 1567px; height: 24px;'>
						<div class='datagrid-header-inner' style='display: block;'>
							<table class='datagrid-htable' border='0' cellspacing='0' cellpadding='0' style='height: 25px;'>
								<tbody>
									<tr class='datagrid-header-row'>
                                        <td field='Name'>
                                            <div class='datagrid-cell datagrid-cell-c1-Name' style='text-align: left;'>
												<span>品名</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Manufacturer' class=''>
											<div class='datagrid-cell datagrid-cell-c1-Manufacturer' style='text-align: center;'>
												<span>品牌</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Model' >

                                            <div class='datagrid-cell datagrid-cell-c1-Model' style='text-align: left;'>
												<span>型号</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Quantity' >

                                            <div class='datagrid-cell datagrid-cell-c1-Quantity' style='text-align: center;'>
												<span>数量</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'UnitPrice' >

                                            <div class='datagrid-cell datagrid-cell-c1-UnitPrice' style='text-align: center;'>
												<span>单价</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'TotalPrice' class=''>
											<div class='datagrid-cell datagrid-cell-c1-TotalPrice' style='text-align: center;'>
												<span>总价</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Unit' class=''>
											<div class='datagrid-cell datagrid-cell-c1-Unit' style='text-align: center;'>
												<span>单位</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Origin' class=''>
											<div class='datagrid-cell datagrid-cell-c1-Origin' style='text-align: center;'>
												<span>产地</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'GrossWeight' >

                                            <div class='datagrid-cell datagrid-cell-c1-GrossWeight' style='text-align: center;'>
												<span>毛重</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
										<td field = 'Btn' >

                                            <div class='datagrid-cell datagrid-cell-c1-Btn' style='text-align: left;'>
												<span>说明</span>
												<span class='datagrid-sort-icon'/>
											</div>
										</td>
									</tr>
								</tbody>
							</table>
						</div>
					</div>
					<div class='datagrid-body' style='width: 1567px; margin-top: 0px; overflow-x: hidden; height: {0}px;'>
						<table class='datagrid-btable' cellspacing='0' cellpadding='0' border='0'>
							<tbody>", rowsNumber * 25);

            if (rows != null && rows.Any())
            {
                for (int i = 0; i < rows.Length; i++)
                {
                    dynamic dyRow = (dynamic)rows[i];

                    sbReferenceInfo.AppendFormat(@"
                                <tr id='datagrid-row-r1-2-{0}' datagrid-row-index='{0}' class='datagrid-row' style='height: 25px; '>
                                    <td field = 'Name' >            
                                        <div style = 'text-align:left;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Name'>{1}</div>
									</td>
									<td field = 'Manufacturer' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Manufacturer'>{2}</div>
									</td>
									<td field = 'Model' >
                                        <div style='text-align:left;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Model'>{3}</div>
									</td>
									<td field = 'Quantity' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Quantity'>{4}</div>
									</td>
									<td field = 'UnitPrice' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-UnitPrice'>{5}</div>
									</td>
									<td field = 'TotalPrice' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-TotalPrice'>{6}</div>
									</td>
									<td field = 'Unit' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Unit'>{7}</div>
									</td>
									<td field = 'Origin' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Origin'>{8}</div>
									</td>
									<td field = 'GrossWeight' >
                                        <div style='text-align:center;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-GrossWeight'>{9}</div>
									</td>
									<td field = 'Btn' >
                                        <div style='text-align:left;white-space:normal;height:auto;' class='datagrid-cell datagrid-cell-c1-Btn'>
											<span>无说明</span>
										</div>
									</td>
								</tr>", i,
                                dyRow.Name,//rows[i].GetType().GetField("Name").GetValue(rows[i]),                                
                                dyRow.Manufacturer,//rows[i].GetType().GetField("Manufacturer").GetValue(rows[i]), 
                                dyRow.Model,      //rows[i].GetType().GetField("Model").GetValue(rows[i]), 
                                dyRow.Quantity,           //rows[i].GetType().GetField("Quantity").GetValue(rows[i]), 
                                dyRow.UnitPrice,      //rows[i].GetType().GetField("UnitPrice").GetValue(rows[i]), 
                                dyRow.TotalPrice,     //rows[i].GetType().GetField("TotalPrice").GetValue(rows[i]), 
                                dyRow.Unit,            //rows[i].GetType().GetField("TotalPrice").GetValue(rows[i]), 
                                dyRow.Origin,       //rows[i].GetType().GetField("Origin").GetValue(rows[i]), 
                                dyRow.GrossWeight      //rows[i].GetType().GetField("GrossWeight").GetValue(rows[i])
                            );
                }
            }

            sbReferenceInfo.Append(@"
                            </tbody>
						</table>
					</div>
					<div class='datagrid - footer' style='width: 1567px; '>
                           <div class='datagrid-footer-inner' style='display: none;'/>
					</div>
				</div>
				<table id = 'products' style='display: none;' title='产品信息' class='datagrid-f'>
					<thead>
						<tr>
							<th data-options='field:'Name',align:'left'' style='width: 18%;'>品名</th>
							<th data-options='field:'Manufacturer',align:'center'' style='width: 12%;'>品牌</th>
							<th data-options='field:'Model',align:'left'' style='width: 18%;'>型号</th>
							<th data-options='field:'Quantity',align:'center'' style='width: 8%;'>数量</th>
							<th data-options='field:'UnitPrice',align:'center'' style='width: 8%;'>单价</th>
							<th data-options='field:'TotalPrice',align:'center'' style='width: 8%;'>总价</th>
							<th data-options='field:'Unit',align:'center'' style='width: 6%;'>单位</th>
							<th data-options='field:'Origin',align:'center'' style='width: 6%;'>产地</th>
							<th data-options='field:'GrossWeight',align:'center'' style='width: 6%;'>毛重</th>
							<th data-options='field:'Btn',align:'left',formatter:OperationModel' style='width: 8%;'>说明</th>
						</tr>
					</thead>
				</table>
				<style type = 'text/css' easyui='true'>
.datagrid-cell-c1-Name{width:275px
    }
.datagrid-cell-c1-Manufacturer{width:180px
}
.datagrid-cell-c1-Model{width:275px}
.datagrid-cell-c1-Quantity{width:117px}
.datagrid-cell-c1-UnitPrice{width:117px}
.datagrid-cell-c1-TotalPrice{width:117px}
.datagrid-cell-c1-Unit{width:85px}
.datagrid-cell-c1-Origin{width:85px}
.datagrid-cell-c1-GrossWeight{width:85px}
.datagrid-cell-c1-Btn{width:117px}
				</style>
			</div>
		</div>
	</div>");

            return sbReferenceInfo.ToString();
        }

        #endregion

        #region 为“拆分订单”生成参考信息Html

        public string GetReferenceInfoHtmlForSplitOrder(string tinyOrderID)
        {
            //订单产品信息
            var products = new Views.OrdersView()[tinyOrderID];
            Func<Needs.Ccs.Services.Models.OrderItem, object> convert1 = item => new
            {
                item.ID,
                Batch = item.Batch,
                Name = item.Category == null ? item.Name : item.Category.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                DeclaredQuantity = item.DeclaredQuantity ?? 0,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
                TotalPrice = item.TotalPrice.ToRound(2),
                ProductDeclareStatus = item.ProductDeclareStatus.GetDescription()
            };
            var productsRows = products.Items.Select(convert1).ToArray();

            //装箱信息
            var packingBill = new Needs.Ccs.Services.Views.SortingsView().GetSortingPacking();
            var packingDatas = packingBill.Where(Item => Item.OrderID == tinyOrderID);
            Func<SortingPacking, object> convert2 = item => new
            {
                ID = item.ID,
                PackingID = item.Packing.ID,
                SortingID = item.ID,
                BoxIndex = item.BoxIndex,
                NetWeight = item.NetWeight,
                GrossWeight = item.GrossWeight,
                Model = item.OrderItem.Model,//产品型号
                ProductName = item.OrderItem.Name,  //产品名称
                CustomsName = item.OrderItem.Category.Name,  //报关品名
                Quantity = item.Quantity,
                Origin = item.OrderItem.Origin,
                Manufacturer = item.OrderItem.Manufacturer,
                DecStatus = item.DecStatus.GetDescription(),
                Status = item.Packing.PackingStatus.GetDescription(),
                PickDate = item.Packing.PackingDate.ToString("yyyy-MM-dd")
            };
            var packingBillRows = packingDatas.Select(convert2).ToArray();

            //产生 html 字符串
            string html1 = GenerateHtmlStrForProductsRows(productsRows);
            html1 = html1.Replace("'", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");

            if (!string.IsNullOrEmpty(html1))
            {
                html1 = html1.Trim();
            }

            string html2 = GenerateHtmlStrForPackingBillRows(packingBillRows);
            html2 = html2.Replace("'", "&quot;").Replace("<", "&lt;").Replace(">", "&gt;");

            if (!string.IsNullOrEmpty(html2))
            {
                html2 = html2.Trim();
            }

            this.ReferenceInfo = html1 + "这是一个超级分隔符" + html2;

            return html1 + "这是一个超级分隔符" + html2;
        }

        private string GenerateHtmlStrForProductsRows(object[] productsRows)
        {

            

            return "";
        }

        private string GenerateHtmlStrForPackingBillRows(object[] packingBillRows)
        {




            return "";
        }

        #endregion

    }
}
