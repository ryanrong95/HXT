using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Notice
{
    public partial class InterestMatch : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void OrderCheck()
        {
            string OrderID = Request.Form["OrderID"];
            string ID = Request.Form["ID"];
            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyFundTransferApply[ID];

            var client = Needs.Wl.Admin.Plat.AdminPlat.Clients.Where(t => t.Company.Name == notices.Payer).FirstOrDefault();
            if (client == null)
            {
                Response.Write((new { success = false, message = "客户信息不存在" }).Json());
            }
            else
            {
                string OrderIDPrefix = OrderID.Substring(0, 5).ToUpper();
                if (OrderIDPrefix.Equals(client.ClientCode))
                {
                    string[] orders = OrderID.Split('-');
                    if (orders.Length == 1)
                    {
                        Response.Write((new { success = false, message = "请输入小订单号" }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = true, message = "" }).Json());
                    }                    
                }
                else
                {
                    Response.Write((new { success = false, message = "订单号不属于该付款人" }).Json());
                }
            }
        }

        protected void match()
        {

            string id = Request.Form["ID"];
            string orderID = Request.Form["OrderID"];
            try
            {
                Needs.Ccs.Services.Models.FundTransferApplies fundapply = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FundTransferApplies[id];
                fundapply.OrderID = orderID;
                fundapply.UpdateOrderID();

                var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                Needs.Ccs.Services.Models.PaymentApply apply = new Needs.Ccs.Services.Models.PaymentApply();
                apply.OrderID = orderID;
                apply.Applier = admin;
                apply.PayFeeType = Needs.Ccs.Services.Enums.FinanceFeeType.Incidental;//贴现成本，应该对应杂费类型，记录到成本中
                apply.FeeDesc = "手续费";
                apply.PayeeName = "承兑利息";
                if (fundapply.DiscountInterest == null)
                {
                    Response.Write((new { success = false, message = "财务未维护承兑利息" }).Json());
                }
                apply.Amount = fundapply.DiscountInterest.Value;
                apply.Currency = "CNY";
                apply.PayDate = fundapply.PaymentDate == null ? DateTime.Now : fundapply.PaymentDate.Value;
                apply.PayType = Needs.Ccs.Services.Enums.PaymentType.TransferAccount;
                apply.SetOperator(admin);
                apply.Enter();

                Response.Write((new { success = true, message = "提交成功" }).Json());

            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}