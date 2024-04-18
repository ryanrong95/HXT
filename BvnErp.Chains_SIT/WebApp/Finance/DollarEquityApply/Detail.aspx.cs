using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.DollarEquityApply
{
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string DollarEquityApplyID = Request.QueryString["DollarEquityApplyID"];

            var dollarEquityApply = new Needs.Ccs.Services.Views.Origins.DollarEquityAppliesOrigin().Where(t => t.ID == DollarEquityApplyID)
                .Select(item => new
                {
                    ApplyID = item.ApplyID,
                    Amount = item.Amount,
                    Currency = item.Currency,
                    SupplierChnName = item.SupplierChnName,
                    SupplierEngName = item.SupplierEngName,
                    BankName = item.BankName,
                    BankAccount = item.BankAccount,
                    BankAddress = item.BankAddress,
                    SwiftCode = item.SwiftCode,
                }).FirstOrDefault();

            this.Model.DollarEquityApply = dollarEquityApply.Json();
        }





    }
}