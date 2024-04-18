using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.PaymentOperators
{
    public partial class PaymentOperatorLists : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            using (var query = new Needs.Ccs.Services.Views.PaymentOperatorListView())
            {
                var view = query;

                view = view.SearchByPaymentOperator();

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void Delete()
        {
            try
            {
                var FinancePaymentOperatorID = Request.Form["FinancePaymentOperatorID"];

                Needs.Ccs.Services.Models.FinancePaymentOperator financePaymentOperator = new Needs.Ccs.Services.Models.FinancePaymentOperator();
                financePaymentOperator.ID = FinancePaymentOperatorID;

                financePaymentOperator.Abandon();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "操作失败：" + ex.Message }).Json());
            }
        }

    }
}