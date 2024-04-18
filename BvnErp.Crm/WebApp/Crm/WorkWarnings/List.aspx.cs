using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorkWarnings
{
    /// <summary>
    /// 工作提醒列表页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                this.Model.Status = EnumUtils.ToDictionary<WarningStatus>().Select(item=> new { text = item.Value, value = item.Key }).Json();
            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        protected void data()
        {
            string Status = Request.QueryString["Status"];
            string Resource = Request.QueryString["Resource"];
            string SDate = Request.QueryString["SDate"];
            string EDate = Request.QueryString["EDate"];
            var works = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWarning.AsQueryable();

            if(!string.IsNullOrWhiteSpace(Status))
            {
                works = works.Where(item => item.Status == (WarningStatus)int.Parse(Status));
            }
            if(!string.IsNullOrWhiteSpace(Resource))
            {
                works = works.Where(item => item.Resource.Contains(Resource));
            }
            if (!string.IsNullOrWhiteSpace(SDate))  //开始时间
            {
                works = works.Where(a => a.UpdateDate.Date >= DateTime.Parse(SDate));
            }
            if (!string.IsNullOrWhiteSpace(EDate))  //结束时间
            {
                works = works.Where(a => a.UpdateDate.Date <= DateTime.Parse(EDate));
            }
            //对象转化
            Func<NtErp.Crm.Services.Models.WorkWarning, object> linq = c => new
            {
                c.ID,
                c.UpdateDate,
                c.Summary,
                TypeName = c.Type.GetDescription().ToString(),
                StatusName = c.Status.GetDescription().ToString(),
                c.Type,
                c.MainID,
                c.Resource,
            };
            this.Paging(works, linq);
        }

        /// <summary>
        /// 更新消息的状态
        /// </summary>
        protected void UpdateStatus()
        {
            var id = Request.Form["ID"];  //关联ID
            var warning = Needs.Erp.ErpPlot.Current.ClientSolutions.WorksWarning[id];
            if (warning != null)
            {
                warning.Status = WarningStatus.read;  //更新为已读
                warning.Enter();
            }
        }
    }
}