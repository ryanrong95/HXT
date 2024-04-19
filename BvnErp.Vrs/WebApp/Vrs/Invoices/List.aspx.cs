using Needs.Erp;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using Needs.Utils.Serializers;
using Needs.Web;
using NtErp.Vrs.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Invoices
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            IQueryable<NtErp.Vrs.Services.Models.Invoice> query = ErpPlot.Current.Publishs.InvoicesAll;
            Expression<Func<NtErp.Vrs.Services.Models.Invoice, bool>> expression = item => true;
            string companyname = Request.QueryString["txtName"];
            string contactname = Request.QueryString["txtContact"];
            string type = Request.QueryString["_type"];
            if (!string.IsNullOrWhiteSpace(companyname))
            {
                expression = expression.And(item => item.Company.Name.Contains(companyname));
            }
            if (!string.IsNullOrWhiteSpace(contactname))
            {
                expression = expression.And(item => item.Contact.Name.Contains(contactname));
            }
            if (type != "-1" && !string.IsNullOrWhiteSpace(type))
            {
                expression = expression.And(item => item.Type == (InvoiceType)int.Parse(type));
            }
            Response.Paging(query.Where(expression), item => new
            {
                item.ID,
                Type = item.Type.GetDescription(),
                item.Required,
                Company = item.Company.Name,
                Contact = item.Contact.Name,
                Code = item.Company.Code,
                item.Address,
                item.Postzip,
                item.Bank,
                item.BankAddress,
                item.Account,
                item.SwiftCode
            });
        }
        protected void InvoiceAbandon()
        {
            string queryId = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.InvoicesAll[queryId] ?? new NtErp.Vrs.Services.Models.Invoice();
            entity.AbandonSuccess += AbandonSuccess;
            entity.Abandon();
        }
        private void AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write(new { success = true }.Json());
        }
    }
}