using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Industries
{
    /// <summary>
    /// 行业展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            var industry = Needs.Erp.ErpPlot.Current.ClientSolutions.MyIndustries.OrderByDescending(c => c.CreateDate).
                Where(c => c.FatherID == null).OrderBy(c => c.Name);
            this.Paging(industry);
        }
    }
}