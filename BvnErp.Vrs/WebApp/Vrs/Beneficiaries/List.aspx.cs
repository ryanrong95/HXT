using Needs.Erp;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.Beneficiaries
{
    public partial class List : Needs.Web.Forms.ClientPage
    {
        protected void data()
        {
            Expression<Func<NtErp.Vrs.Services.Models.Beneficiary, bool>> expression = null;
            string names = Request.QueryString["txtName"];
            if (names != null)
            {
                expression = expression.And(item => item.Bank.Contains(names));
            }
            using (var context = new Needs.Linq.LinqContext())
            {
                IQueryable<NtErp.Vrs.Services.Models.Beneficiary> data = ErpPlot.Current.Publishs.MyBeneficiaries;

                if (expression != null)
                {
                    data = data.Where(expression);
                }
                Response.Paging(data,item=>new {
                    item.ID,
                    item.Bank,
                    Method=item.Method.GetDescription(),
                    Currency=item.Currency,
                    item.Address,
                    item.SwiftCode,
                    Status=item.Status.GetDescription(),
                    ContactName=item.Contact.Name,
                    CompanyName = item.Company.Name
                });

                Needs.Linq.LinqContext.Current.Dispose();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        protected object ContactMessage()
        {
            var com = ErpPlot.Current.Publishs.MyContacts;
            string contactid = Request.QueryString[nameof(contactid)];
            this.Model = com.Where(i => i.ID == contactid).FirstOrDefault();
            return this.Model;
        }
        protected void del()
        {
            string id = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.MyBeneficiaries[id] ?? new NtErp.Vrs.Services.Models.Beneficiary();
            entity.EnterSuccess += EnterSuccess;
            entity.Abandon();
        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("删除成功", this.Request.Url);
        }
    }
}