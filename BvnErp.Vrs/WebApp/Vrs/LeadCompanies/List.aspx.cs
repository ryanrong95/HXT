using Needs.Web;
using System;
using System.Linq;
using Needs.Utils.Serializers;
using System.Linq.Expressions;
using Needs.Utils.Linq;
using Needs.Utils.Descriptions;

namespace WebApp.Vrs.LeadVenders
{
    public partial class List : Needs.Web.Sso.Forms.ErpPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string key = Request["key"] ?? "";
            var pid = Request["pid"];
            Expression<Func<NtErp.Vrs.Services.Models.Company, bool>> expression = item=>true;
            if (!string.IsNullOrEmpty(key))
            {
                expression = expression.And(item=>item.Name.StartsWith(key));
            }
            var relations = Needs.Erp.ErpPlot.Current.Publishs.MyAdminsComapnies.Where(item=>item.AdminID==pid).ToArray().Select(item => item.CompanyID).ToArray();

            Response.Paging(Needs.Erp.ErpPlot.Current.Publishs.CompaniesAll.Where(expression).ToArray().Select(item => new { item.ID, item.Name, item.Code, Type=item.Type.GetDescription(), item.RegisteredCapital,item.CorporateRepresentative, item.Address, Checked = relations.Contains(item.ID) ? true : false }));

        }
        protected void Enter()
        {
            var id = Request["id"];
            var pid = Request["pid"];
            // Needs.Erp.ErpPlot.Current.Limits.Admins[pid].Plots.MyClients.Bind(id);
            Needs.Erp.ErpPlot.Current.Publishs.MyAdminsComapnies.Bind(id,pid);
        }

        protected void Remove()
        {
            var id = Request["id"];
            var pid = Request["pid"];
            //Needs.Erp.ErpPlot.Current.Limits.Admins[pid].Plots.MyClients.UnBind(id);
            Needs.Erp.ErpPlot.Current.Publishs.MyAdminsComapnies.UnBind(id,pid);
        }
    }
}