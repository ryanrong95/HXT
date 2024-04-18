using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Bill
{
    public partial class AddVoucher : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string MainOrderID = Request.QueryString["MainOrderID"];

            this.Model.MainOrderID = MainOrderID;

            var orders = new Needs.Ccs.Services.Views.Origins.OrdersOrigin().Where(t => t.MainOrderID == MainOrderID).ToList();
            this.Model.OrderIDs = orders.Select(item => new { value = item.ID, text = item.ID }).Json();
        }

        /// <summary>
        /// 校验抵用券号
        /// </summary>
        protected void CheckVoucher()
        {
            string InputVoucherID = Request.Form["InputVoucherID"];

            var voucher = new Needs.Ccs.Services.Views.FinanceVoucherView()
                .Where(t => t.ID == InputVoucherID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

            if (voucher == null)
            {
                Response.Write((new { success = "false", message = "不存在该抵用券", }).Json());
                return;
            }
            else if (!string.IsNullOrEmpty(voucher.OrderID))
            {
                Response.Write((new { success = "false", message = "该抵用券已被使用", }).Json());
                return;
            }
            else
            {
                Response.Write((new { success = "true", message = "该抵用券可以使用", }).Json());
                return;
            }
        }

        /// <summary>
        /// 添加抵用券
        /// </summary>
        protected void Add()
        {
            try
            {
                string InputVoucherID = Request.Form["InputVoucherID"];
                string SelectedOrderID = Request.Form["SelectedOrderID"];
                string currentAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                var voucher = new Needs.Ccs.Services.Views.FinanceVoucherView()
                    .Where(t => t.ID == InputVoucherID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

                if (voucher == null)
                {
                    Response.Write((new { success = "false", message = "不存在该抵用券", }).Json());
                    return;
                }
                else if (!string.IsNullOrEmpty(voucher.OrderID))
                {
                    Response.Write((new { success = "false", message = "该抵用券已被使用", }).Json());
                    return;
                }

                var orderVoucher = new Needs.Ccs.Services.Views.FinanceVoucherView()
                    .Where(t => t.OrderID == SelectedOrderID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

                if (orderVoucher != null)
                {
                    Response.Write((new { success = "false", message = "订单 " + SelectedOrderID + " 已使用过抵用券", }).Json());
                    return;
                }

                //使用抵用券
                voucher.OrderID = SelectedOrderID;
                voucher.UseAdminID = currentAdminID;
                voucher.UseTime = DateTime.Now;
                voucher.Enter();

                Response.Write((new { success = "true", message = "提交成功", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "错误：" + ex.Message, }).Json());
            }
        }

    }
}