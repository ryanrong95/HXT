using Needs.Erp;
using Needs.Erp.Models;
using Needs.Utils.Linq;
using Needs.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Vrs.LeadContacts
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            var pid = Request["pid"];
            string key = Request["key"] ?? "";

            Expression<Func<NtErp.Vrs.Services.Models.Contact, bool>> exp = item => true;
       
            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And(item => item.Name.StartsWith(key));
            }
            var relations = Needs.Erp.ErpPlot.Current.Publishs.MyContactViews.Where(item => item.ID == pid).ToArray().Select(item => item.CompanyID).ToArray();

            Response.Paging(Needs.Erp.ErpPlot.Current.Publishs.MyContacts.Where(exp).ToArray().Select(item => new { item.ID, item.Name, Sex = item.Sex ? "男" : "女", Birthday = DateTime.Parse(item.Birthday).ToString("D"), item.Tel, item.Email, item.Mobile, CompanyName = item.Company.Name, item.Status,  Checked = relations.Contains(item.ID) ? true : false }));
        }
        protected object CompanyMessage()
        {
            var com = ErpPlot.Current.Publishs.CompaniesAll;
            string companyid = Request.QueryString[nameof(companyid)];
            this.Model = com.Where(i => i.ID == companyid).FirstOrDefault();
            return this.Model;
        }
        protected void Enter()
        {
            var id = Request["id"];
            var pid = Request["pid"];
            //Needs.Erp.ErpPlot.Current.Limits.AdminsAll[pid].Publishs.MyContactViews.Bind(id);

        }
        protected void Remove()
        {
            var id = Request["id"];
            var pid = Request["pid"];
           // Needs.Erp.ErpPlot.Current.Limits.AdminsAll[pid].Publishs.MyContactViews.UnBind(id);
        }
    }
}