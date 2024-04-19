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

namespace WebApp.Vrs.Contacts
{
    public partial class List : Needs.Web.Forms.ClientPage
    {
        protected void data()
        {
            Expression<Func<NtErp.Vrs.Services.Models.Contact, bool>> expression = null;
            string names = Request.QueryString["txtName"];
            string id = Request.QueryString["id"];
            if (names != null)
            {
                expression = expression.And(item => item.Name.Contains(names));
            }
            if (!string.IsNullOrEmpty(id))
            {
                expression = expression.And(item => item.CompanyID == id);
            }
            using (var context = new Needs.Linq.LinqContext())
            {
                IQueryable<NtErp.Vrs.Services.Models.Contact> data = ErpPlot.Current.Publishs.MyContacts;
                if (expression != null)
                {
                    data = data.Where(expression);
                }
                //Response.Paging(data);
                Response.Paging(data, item => new
                {
                    item.ID,
                    item.Name,
                    Sex = item.Sex ? "男" : "女",
                    Birthday = DateTime.Parse(item.Birthday).ToString("D"),
                    item.Tel,
                    item.Email,
                    item.Mobile,
                    Job= item.Job.GetDescription(),
                    Status = item.Status.GetDescription(),
                    CompanyName = item.Company.Name,
                });


                Needs.Linq.LinqContext.Current.Dispose();
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object CompanyMessage()
        {
            var com = ErpPlot.Current.Publishs.CompaniesAll;
            string companyid = Request.QueryString[nameof(companyid)];
            this.Model = com.Where(i => i.ID == companyid).FirstOrDefault();
            return this.Model;
        }


        protected void del()
        {
            string id = Request.Form["id"];
            var entity = ErpPlot.Current.Publishs.MyContacts[id] ?? new NtErp.Vrs.Services.Models.Contact();
            entity.AbandonSuccess += EnterSuccess;
            entity.Abandon();
        }
        private void EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            base.Alert("删除成功", this.Request.Url);
        }
    }
}