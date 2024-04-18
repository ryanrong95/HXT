using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.CostApply
{
    public partial class PayeeList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string From = Request.QueryString["From"];
            From = From ?? string.Empty;
            this.Model.From = From;
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            //lambdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayeeListViewModel, bool>>)(t => t.AdminID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID));

            int totalCount = 0;
            var view = new Needs.Ccs.Services.Views.PayeeListView();
            var payeeList = view.GetResults(out totalCount, page, rows, lambdas.ToArray());

            Response.Write(new
            {
                rows = payeeList.Select(
                        item => new
                        {
                            CostApplyPayeeID = item.CostApplyPayeeID,
                            PayeeName = item.PayeeName,
                            PayeeAccount = item.PayeeAccount,
                            PayeeBank = item.PayeeBank,
                        }
                     ).ToArray(),
                total = totalCount,
            }.Json());
        }

        protected void Delete()
        {
            try
            {
                string CostApplyPayeeID = Request.Form["CostApplyPayeeID"];

                var payee = new Needs.Ccs.Services.Views.Origins.CostApplyPayeesOrigin()
                                            .Where(t => t.ID == CostApplyPayeeID).FirstOrDefault();
                payee.Abandon();

                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "删除失败：" + ex.Message }).Json());
            }
        }

    }
}