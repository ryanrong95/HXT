using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Trace
{
    public partial class ShowList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        void PageInit()
        {
            string id = Request.QueryString["id"];
            var report = new NtErp.Crm.Services.Views.ReportsAlls()[id];
            var replay = new NtErp.Crm.Services.Views.ReplyAlls().Where(item => item.ReportID == id);
            if (report != null)
            {
                this.Model.AllData = new
                {
                    JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(report.Context).Content,
                    Date = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(report.Context).Date.ToString(),
                    NextDate = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(report.Context).NextDate.ToString(),
                    TypeName = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(report.Context).Type.GetDescription(),
                    OriginalStaffs = JsonSerializerExtend.JsonTo<NtErp.Crm.Services.Models.Report>(report.Context).OriginalStaffs ?? string.Empty,
                    Reply = replay.OrderBy(c => c.UpdateDate).Select(item => new
                    {
                        item.Admin.RealName,
                        item.UpdateDate,
                        item.Context
                    }).ToArray(),
                    ClientName = report.Client.Name
                }.Json();
            }
            else
            {
                this.Model.AllData = string.Empty.Json();
            }
            
        }
    }
}