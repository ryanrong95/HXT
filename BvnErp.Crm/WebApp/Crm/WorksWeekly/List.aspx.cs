using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksWeekly
{
    /// <summary>
    /// 工作周报展示列表页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var work = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => new { item.ID, item.RealName });
                this.Model.AdminData = work.Json();
            }
        }
        /// <summary>
        /// 加载数据
        /// </summary>
        protected void data()
        {
            string Admin = Request.QueryString["Admin"];
            string CurWeek = Request.QueryString["CurWeek"];
            var data = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWeekly.Where(item => true);
            if (!string.IsNullOrWhiteSpace(Admin))
            {
                data = data.Where(c => c.Admin.ID == Admin);
            }
            if (!string.IsNullOrWhiteSpace(CurWeek))
            {
                data = data.Where(c => c.WeekOfYear == int.Parse(CurWeek));
            }

            Func<NtErp.Crm.Services.Models.WorksWeekly, object> linq = c => new
            {
                c.ID,
                c.UpdateDate,
                c.CreateDate,
                AdminName = c.Admin.RealName,
                c.Context,
                c.WeekOfYear,
                isShow = Needs.Erp.ErpPlot.Current.ID == c.Admin.ID,
                StatusName = c.Status.GetDescription(),
            };
            this.Paging(data, linq);
        }
    }
}