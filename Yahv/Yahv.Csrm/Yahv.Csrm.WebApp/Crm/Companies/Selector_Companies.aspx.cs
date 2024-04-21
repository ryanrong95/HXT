using System;
using System.Linq;
using System.Linq.Expressions;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Forms;
using YaHv.Csrm.Services.Extends;
using YaHv.Csrm.Services.Models.Origins;
using YaHv.Csrm.Services.Views.Rolls;

namespace Yahv.Csrm.WebApp.Crm.Companies
{
    public partial class Selector_Companies : ClientPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected object data()
        {
            Expression<Func<Company, bool>> predicate = item => item.CompanyStatus == ApprovalStatus.Normal;
            var name = Request["s_name"];
            if (!string.IsNullOrWhiteSpace(name))
            {
                predicate = predicate.And(item => item.Enterprise.Name.Contains(name));
            }
            var query = new CompaniesRoll().Where(predicate);
            return new
            {
                rows = query.OrderBy(item => item.Enterprise.Name).ToArray().Select(item => new
                {
                    item.ID,
                    item.Enterprise.Name,
                    item.Enterprise.AdminCode,
                    Range = item.Range.GetDescription(),
                    Type = item.Type.GetDescription(),
                    item.Enterprise.District,
                    Status = item.CompanyStatus.GetDescription()
                })
            };
        }

        protected void del()
        {
            var arry = Request.Form["items"].Split(',');
            new CompaniesRoll().Where(t => arry.Contains(t.ID)).Delete();
        }
        protected void Enable()
        {
            var arry = Request.Form["items"].Split(',');
            new CompaniesRoll().Where(t => arry.Contains(t.ID)).Enable();
        }
        protected void Unable()
        {
            var arry = Request.Form["items"].Split(',');
            new CompaniesRoll().Where(t => arry.Contains(t.ID)).Unable();
        }

    }
}