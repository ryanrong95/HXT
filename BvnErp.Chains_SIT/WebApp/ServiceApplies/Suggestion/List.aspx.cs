using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ServiceApplies.Suggestion
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string Summary = Request.QueryString["Summary"];
            var suggestions = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.Suggestions.OrderBy(item => item.CreateDate).AsQueryable()
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal);
            if (!string.IsNullOrEmpty(Summary))
            {
                Summary = Summary.Trim();
                suggestions = suggestions.Where(t => t.Summary.Contains(Summary));
            }
            Func<Needs.Ccs.Services.Models.Suggestions, object> convert = item => new
            {
                ID = item.ID,
                Name = item.Name,
                Phone = item.Phone,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                Summary = item?.Summary
            };
            this.Paging(suggestions, convert);
        }
    }
}