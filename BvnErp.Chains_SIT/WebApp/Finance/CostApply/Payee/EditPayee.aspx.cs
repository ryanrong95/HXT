using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply.Payee
{
    public partial class EditPayee : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string CostApplyPayeeID = Request.QueryString["CostApplyPayeeID"];
            string From = Request.QueryString["From"];
            this.Model.From = From;

            string PayeeName = string.Empty;
            string PayeeAccount = string.Empty;
            string PayeeBank = string.Empty;

            if (From == "Edit")
            {
                var payee = new Needs.Ccs.Services.Views.PayeeDetailView().GetResult(CostApplyPayeeID);
                PayeeName = payee.PayeeName;
                PayeeAccount = payee.PayeeAccount;
                PayeeBank = payee.PayeeBank;
            }

            this.Model.Payee = new
            {
                CostApplyPayeeID = CostApplyPayeeID,
                PayeeName = PayeeName,
                PayeeAccount = PayeeAccount,
                PayeeBank = PayeeBank,
            }.Json();
        }

        protected void Save()
        {
            try
            {
                string From = Request.Form["From"];
                string CostApplyPayeeID = Request.Form["CostApplyPayeeID"];
                string PayeeName = Request.Form["PayeeName"];
                string PayeeAccount = Request.Form["PayeeAccount"];
                string PayeeBank = Request.Form["PayeeBank"];

                var payee = new Needs.Ccs.Services.Models.CostApplyPayee();
                if (From == "Edit")
                {
                    payee = new Needs.Ccs.Services.Views.Origins.CostApplyPayeesOrigin()
                                        .Where(t => t.ID == CostApplyPayeeID).FirstOrDefault();
                }
                else
                {
                    payee.ID = Guid.NewGuid().ToString("N");
                    payee.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                    payee.Status = Needs.Ccs.Services.Enums.Status.Normal;
                    payee.CreateDate = DateTime.Now;
                }

                payee.PayeeName = PayeeName;
                payee.PayeeAccount = PayeeAccount;
                payee.PayeeBank = PayeeBank;
                payee.UpdateDate = DateTime.Now;

                payee.Enter();

                Response.Write((new { success = true, message = "提交成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }


    }
}