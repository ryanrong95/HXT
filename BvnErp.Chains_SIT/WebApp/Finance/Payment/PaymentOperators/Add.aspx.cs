using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.PaymentOperators
{
    public partial class Add : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var query = new Needs.Ccs.Services.Views.PaymentOperatorListView())
            {
                var view = query;
                view = view.SearchByAvaiableButNotReal();
                this.Model.AvaiblePaymentOperators = view.ToMyPage().Json();
            }
        }

        protected void Save()
        {
            try
            {
                var AdminID = Request.Form["AdminID"];

                Needs.Ccs.Services.Models.FinancePaymentOperator financePaymentOperator = new Needs.Ccs.Services.Models.FinancePaymentOperator();
                financePaymentOperator.ID = Guid.NewGuid().ToString("N");
                financePaymentOperator.AdminID = AdminID;
                financePaymentOperator.Type = Needs.Ccs.Services.Enums.PaymentOperatorType.PaymentOperator;
                financePaymentOperator.Status = Needs.Ccs.Services.Enums.Status.Normal;
                financePaymentOperator.CreateDate = DateTime.Now;
                financePaymentOperator.UpdateDate = DateTime.Now;

                financePaymentOperator.Enter();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}