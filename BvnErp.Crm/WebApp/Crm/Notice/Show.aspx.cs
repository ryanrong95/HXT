using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Notice
{
    /// <summary>
    /// 公告详情展示页面
    /// </summary>
    public partial class Show : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        protected void  LoadData()
        {
            string id = Request.QueryString["ID"];
            var notice = Needs.Erp.ErpPlot.Current.ClientSolutions.MyNotice[id];
            if(notice.Status == NtErp.Crm.Services.Enums.WarningStatus.unread)
            {
                Needs.Erp.ErpPlot.Current.ClientSolutions.MyNotice.Read(id);
            }
            this.AdminName.Text = notice.Admin.RealName;
            //公告板的附件
            var files = new NtErp.Crm.Services.Views.FileAlls().Where(item => item.NoticeID == notice.ID && item.Status == Status.Normal);
            this.Model.Files = files.Select(item => new { item.ID, item.Name, item.Url }).Json();
            this.Model.AllData = notice.Json();
        }
    }
}