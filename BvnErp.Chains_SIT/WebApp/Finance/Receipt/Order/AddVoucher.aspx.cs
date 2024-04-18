using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Order
{
    public partial class AddVoucher : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string FinanceReceiptId = Request.QueryString["FinanceReceiptId"];

            this.Model.FinanceReceiptId = FinanceReceiptId;
        }
        /*
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
            else if (!string.IsNullOrEmpty(voucher.FinanceReceiptID))
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
                string FinanceReceiptId = Request.Form["FinanceReceiptId"];
                string currentAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

                var voucher = new Needs.Ccs.Services.Views.FinanceVoucherView()
                    .Where(t => t.ID == InputVoucherID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

                if (voucher == null)
                {
                    Response.Write((new { success = "false", message = "不存在该抵用券", }).Json());
                    return;
                }
                else if (!string.IsNullOrEmpty(voucher.FinanceReceiptID))
                {
                    Response.Write((new { success = "false", message = "该抵用券已被使用", }).Json());
                    return;
                }

                //使用抵用券
                voucher.FinanceReceiptID = FinanceReceiptId;
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
        */
    }
}