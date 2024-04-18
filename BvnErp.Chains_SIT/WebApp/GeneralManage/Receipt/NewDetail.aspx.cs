using Needs.Wl;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Serializers;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;

namespace WebApp.GeneralManage.Receipt
{
    public partial class NewDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            // 初始化信息
            var orderID = Request.QueryString["ID"];
            var received = Request.QueryString["Status"];
            var receivedStatus = (OrderReceivedStatus)Enum.Parse(typeof(OrderReceivedStatus), received);
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[orderID];

            //客户信息
            this.Model.ClientInfo = new
            {
                ClientCode = order.Client.ClientCode,
                ClientName = order.Client.Company.Name,
                ClientRank = order.Client.ClientRank.GetDescription(),
                Salesman = order.Client.ServiceManager.RealName,
                Merchandiser = order.Client.Merchandiser.RealName
            }.Json();

            //订单信息
            var declarePrice = (order.DeclarePrice * order.ProductFeeExchangeRate).ToRound(2);
            var declareDate = Needs.Wl.Admin.Plat.AdminPlat.Current.Declaration.DeclareHead.Where(d => d.OrderID == orderID)
                .Select(s => s.DDate).FirstOrDefault();
            this.Model.OrderInfo = new
            {
                OrderId = orderID,
                CreateDate = order.CreateDate.ToShortDateString(),
                //需要查报关日期 DDate 可为null
                DeclareDate = declareDate?.ToShortDateString(),
                DeclarePrice = declarePrice,
                IsLoan = order.IsLoan == true ? "是" : "否",
                OrderStatus = order.OrderStatus.GetDescription(),
                //ClientAgreementId=order.ClientAgreement.ID
                ClientID=order.Client.ID
            }.Json();


            //利润计算 已收款计算利润
            //税代费 = 实收关税+实收增值税+实收代理费（不含税）+实收杂费（不含税）+香港现金费用
            //费用 = 订单关税+订单增值税+实付杂费
            //利润 = 税代费-费用

            var orderReceived = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceived.FirstOrDefault(o => o.ID == orderID);
            if (receivedStatus == OrderReceivedStatus.Received)
            {
                this.Model.Profit = new
                {
                    ReceiptStatus = "已收款",
                    TaxAgentAmount = orderReceived.TaxGeneratTotal.ToString("0.00"),
                    FeeAmount = orderReceived.FeeTotal.ToString("0.00"),
                    Profit = orderReceived.Profit.ToString("0.00"),
                    ProfitRate= (orderReceived.Profit/ orderReceived.FeeTotal*100).ToString("0.00")+"%"
                }.Json();
            }
            else
            {
                this.Model.Profit = new
                {
                    ReceiptStatus = "未收款/部分收款",
                    TaxAgentAmount = "--",
                    FeeAmount = "--",
                    Profit = "--",
                    ProfitRate="--"
                }.Json();
            }

            //收款

            var receipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderAllStats.FirstOrDefault(o => o.ID == orderID);

            var receiptList = new List<dynamic>()
            {
                new
                {
                    FeeType = OrderFeeType.Product,
                    FeeTypeDesc = OrderFeeType.Product.GetDescription(),
                    Receivable = receipts.Product.ToString("0.00"),
                    ReceivableDate = receipts.ProductFeeDueDate.ToShortDateString(),
                    Received = receipts.ProductReceived.ToString("0.00")
                },
                new
                {
                    FeeType = OrderFeeType.Tariff,
                    FeeTypeDesc = OrderFeeType.Tariff.GetDescription(),
                    Receivable = receipts.TariffReceivable.ToString("0.00"),
                    ReceivableDate = receipts.TaxFeeDueDate.ToShortDateString(),
                    Received =receipts.TariffReceived.ToString("0.00")
                },
                new
                {
                    FeeType = OrderFeeType.AddedValueTax,
                    FeeTypeDesc = OrderFeeType.AddedValueTax.GetDescription(),
                    Receivable = receipts.AVTReceivable.ToString("0.00"),
                    ReceivableDate = receipts.TaxFeeDueDate.ToShortDateString(),
                    Received =receipts.AVTReceived.ToString("0.00")
                },
                new
                {
                    FeeType = OrderFeeType.AgencyFee,
                    FeeTypeDesc = OrderFeeType.AgencyFee.GetDescription(),
                    Receivable = receipts.AgencyReceivable.ToString("0.00"),
                    ReceivableDate = receipts.AgencyFeeDueDate.ToShortDateString(),
                    Received =receipts.AgencyReceived.ToString("0.00")
                },
                new
                {
                    FeeType = OrderFeeType.Incidental,
                    FeeTypeDesc = OrderFeeType.Incidental.GetDescription(),
                    Receivable = receipts.IncidentalReceivable.ToString("0.00"),
                    ReceivableDate = receipts.IncidentalFeeDueDate.ToShortDateString(),
                    Received =receipts.IncidentalReceived.ToString("0.00")
                },
                new
                {
                    FeeType =WhsePaymentType.Cash,
                    FeeTypeDesc = "现金费用",
                    Receivable = "-",
                    ReceivableDate = "-",
                    Received =receipts.HKFeeReceived.ToString("0.00")
                }
            };

            this.Model.Receipts = receiptList.Json();

            //付款
            var paymentList = new List<dynamic>()
            {
                new 
                {
                    PayFeeType = OrderFeeType.Product,
                    PayFeeTypeDesc= OrderFeeType.Product.GetDescription(),
                    PayReceived = receipts.ProductPaid.ToString("0.00"),
                    PayDate = receipts.ProductPayDate?.ToShortDateString()
                },
                new 
                {
                    PayFeeType =OrderFeeType.Tariff,
                    PayFeeTypeDesc= OrderFeeType.Tariff.GetDescription(),
                    PayReceived = receipts.TariffReceivable.ToString("0.00"),
                    PayDate = receipts.TariffPayDate?.ToShortDateString()
                },
                new 
                {
                    PayFeeType = OrderFeeType.AddedValueTax,
                    PayFeeTypeDesc = OrderFeeType.AddedValueTax.GetDescription(),
                    PayReceived = receipts.AVTReceivable.ToString("0.00"),
                    PayDate = receipts.AVTPayDate?.ToShortDateString()
                },
                new 
                {
                    PayFeeType = OrderFeeType.Incidental,
                    PayFeeTypeDesc = OrderFeeType.Incidental.GetDescription(),
                    PayReceived = receipts.IncidentalPaid.ToString("0.00"),
                    PayDate =receipts.IncidentalPayDate?.ToShortDateString()
                }
            };
            this.Model.Payments = paymentList.Json();
        }
    }
}