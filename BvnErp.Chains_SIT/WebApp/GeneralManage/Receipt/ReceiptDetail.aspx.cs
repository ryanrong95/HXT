using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Needs.Wl;

namespace WebApp.GeneralManage.Receipt
{
    public partial class ReceiptDetail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            data();
        }

        protected void data()
        {
            var orderId = Request.QueryString["ID"];
            var feeType = Request.QueryString["FeeType"];
            var hkFlag = Request.QueryString["Flag"];

            //香港现金费用
            if (hkFlag == "true")
            {
                var orderWhesPremiums = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.OrderWhesPremium.Where(w =>
                    w.OrderID == orderId && w.WhsePaymentType == WhsePaymentType.Cash &&
                    w.WarehousePremiumsStatus == WarehousePremiumsStatus.Payed).ToList();

                var totalAmount = orderWhesPremiums.Sum(t => (decimal?)t.UnitPrice * t.Count * t.ExchangeRate).GetValueOrDefault();

                var result = new List<dynamic>();
                result.AddRange(orderWhesPremiums.Select(hk => new
                {
                    Date = hk.UpdateDate.ToString().Replace("T", ""),
                    Payee = hk.Creater.RealName,
                    ReceivedAmount = (hk.UnitPrice * hk.Count * hk.ExchangeRate).ToString("0.00"),
                    SeqNo = "",
                    ReceiptDate = "",
                }));

                result.Add(new
                {
                    Date = "总计",
                    Payee = "",
                    ReceivedAmount = totalAmount.ToString("0.00")
                });
                this.Model.Receipts = result.Json();
            }
            //其他费用
            else
            {
                var orderReceipts = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceipts.Where(o =>
                    o.OrderID == orderId &&
                    o.Type == OrderReceiptType.Received &&
                    o.FeeType == (OrderFeeType)Enum.Parse(typeof(OrderFeeType), feeType, true)

                ).ToList();

                var totalAmount = orderReceipts.Sum(o => (decimal?)-o.Amount).GetValueOrDefault();

                var result = new List<dynamic>();

                result.AddRange(orderReceipts.Select(orderReceipt => new
                {
                    Date = orderReceipt.UpdateDate.ToString().Replace("T", ""),
                    Payee = orderReceipt.Admin.RealName,
                    ReceivedAmount = (-orderReceipt.Amount).ToString("0.00"),
                    SeqNo = orderReceipt.SeqNo,
                    ReceiptDate = orderReceipt.ReceiptDate?.ToString("yyyy-MM-dd"),
                }));
                result.Add(new
                {
                    Date = "总计",
                    Payee = "",
                    ReceivedAmount = totalAmount.ToString("0.00")
                });
                this.Model.Receipts = result.Json();
            }
        }
    }
}