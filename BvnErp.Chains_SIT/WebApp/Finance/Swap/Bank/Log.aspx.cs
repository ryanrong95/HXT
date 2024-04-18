using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap.Bank
{
    public partial class Log : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            ID = ID.Trim();
            var head = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapLimitCountryLogs.Where(item=>item.BankID==ID).OrderByDescending(item=>item.CreateDate).AsQueryable();

            Func<Needs.Ccs.Services.Models.SwapLimitCountryLog, object> convert = trace => new
            {
                ID = trace.ID,
                CreateDate = trace.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Admin = trace.Admin != null ? trace.Admin.RealName : string.Empty,
                Summary = trace.Summary,
                
            };

            this.Paging(head, convert);

        }
    }
}