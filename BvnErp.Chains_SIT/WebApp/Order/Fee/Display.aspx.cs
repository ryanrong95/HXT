using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Fee
{
    /// <summary>
    /// 订单杂费维护界面
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化基本信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[id];

            this.Model.AllData = new
            {
                OrderID = order.ID,
                OrderStatus = order.OrderStatus
            }.Json();
        }

        /// <summary>
        /// 初始化订单杂费信息
        /// </summary>
        protected void dataOrderFees()
        {
            string id = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders[id];
            var taxPoint = 1 + order.ClientAgreement.InvoiceTaxRate;

            var orderFees = from fee in order.MiscFees
                            select new
                            {
                                fee.ID,
                                OrderID = fee.OrderID,
                                Type = GetShowType(fee),
                                Count = fee.Count,
                                UnitPrice = fee.UnitPrice.ToString("0.0000"),
                                TotalPrice = (fee.Count * fee.UnitPrice).ToString("0.0000"),
                                Currency = fee.Currency,
                                Rate = fee.Rate.ToString("0.0000"),
                                PremiumStatus = fee.GetPremiumStatus(taxPoint),
                                IsPaid = fee.GetPremiumStatus(taxPoint).GetDescription(),
                                PaymentDate = fee.PaymentDate?.ToShortDateString(),
                            };

            Response.Write(new
            {
                rows = orderFees.ToArray(),
                total = orderFees.Count()
            }.Json());
        }

        private string GetShowType(Needs.Ccs.Services.Models.OrderPremium fee)
        {
            string showType = "";

            string otherFeeName = !string.IsNullOrEmpty(fee.Name) ? "(" + fee.Name + ")" : "";
            showType += fee.Type == OrderPremiumType.OtherFee ? fee.Type.GetDescription() + otherFeeName : fee.Type.GetDescription();

            var standardRemark = ConvertStandardRemarkToObj(fee.StandardRemark);
            if (standardRemark != null)
            {
                showType += "(" + standardRemark?.SelectedStd?.Name + ")";
            }
            return showType;
        }

        private Needs.Ccs.Services.Models.StandardRemark ConvertStandardRemarkToObj(string standardRemark)
        {
            if (string.IsNullOrEmpty(standardRemark))
            {
                return null;
            }

            try
            {
                var obj = JsonConvert.DeserializeObject<Needs.Ccs.Services.Models.StandardRemark>(standardRemark);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 删除费用
        /// </summary>
        protected void Delete()
        {
            try
            {
                string id = Request.Form["ID"];
                var fee = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.OrderPremiums[id];
                if (fee != null)
                {
                    fee.AbandonSuccess += Fee_AbandonSuccess;
                    fee.Abandon();

                    //重新传这个订单的杂费给 Yahv
                    var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                    Needs.Ccs.Services.Models.PaymentToYahvOtherFee paymentToYahvOtherFee = new Needs.Ccs.Services.Models.PaymentToYahvOtherFee(fee.OrderID, admin.ID);
                    paymentToYahvOtherFee.Execute();
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fee_AbandonSuccess(object sender, SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "删除成功！", ID = e.Object }).Json());
        }
    }
}