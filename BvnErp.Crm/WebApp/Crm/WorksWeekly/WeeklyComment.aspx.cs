using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.WorksWeekly
{
    /// <summary>
    /// 工作周报评论
    /// </summary>
    public partial class WeeklyComment : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }

        /// <summary>
        /// 页面初始化
        /// </summary>
        protected void PageInit()
        {
            AdminName.Text = Needs.Erp.ErpPlot.Current.RealName;
            CreateDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];     //MianID
            string type = Request.QueryString["Type"];  //类型
            var rely = new NtErp.Crm.Services.Models.Reply();  //点评
            switch ((WarningType)int.Parse(type))
            {
                case WarningType.CommentWorksWeekly:  //工作周报点评
                    rely.WorksWeekly = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.WorksWeekly>.Create(id);
                    break;
                case WarningType.CommentWorksOther:   //工作计划
                    rely.WorksOther = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.WorksOther>.Create(id);
                    break;
                case WarningType.CommentTrace:         //客户跟踪记录
                case WarningType.ProjectComment:       //销售机会跟踪记录
                    rely.Report = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Report>.Create(id);
                    break;
            }
            string commentvalue = Request.Form["CommentValue"];  //点评内容
            rely.Context = commentvalue;
            rely.Admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(Needs.Erp.ErpPlot.Current.ID);
            rely.EnterSuccess += Rely_EnterSuccess;
            rely.Enter();
        }

        /// <summary>
        /// 成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rely_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            //指定阅读人处理
            Readers(e.Object);
            ReplyWarning(sender as NtErp.Crm.Services.Models.Reply);
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

        /// <summary>
        /// 指定阅读人处理
        /// </summary>
        /// <param name="replyid"></param>
        private void Readers(string replyid)
        {
            string[] readers = Request.Form["Reader"]?.Split(',').ToArray();
            var replyview = new NtErp.Crm.Services.Views.ReplyAlls();
            var reply = replyview[replyid];
            if (readers == null)
            {
                return;
            }
            foreach (string reader in readers)
            {
                //阅读人绑定点评
                var admin = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.AdminTop>.Create(reader);
                replyview.BindingReader(reply, admin);
                //提醒阅读人
                var warning = new NtErp.Crm.Services.Views.WorkWarningsAlls().Where(item => item.MainID == reply.ReportID && item.Type ==
                   WarningType.CommentTraceReadWarning && item.Admin.ID == admin.ID).SingleOrDefault() ?? new NtErp.Crm.Services.Models.WorkWarning();
                warning.MainID = reply.ReportID;
                warning.Type = WarningType.CommentTraceReadWarning;
                warning.Resource = Needs.Erp.ErpPlot.Current.RealName;
                warning.Admin = admin;
                warning.Summary = "客户的跟踪记录点评指定阅读人阅读";
                warning.Enter();
            }
        }

        /// <summary>
        /// 工作提醒保存
        /// </summary>
        /// <param name="reply">reply对象</param>
        private void ReplyWarning(NtErp.Crm.Services.Models.Reply reply)
        {
            string id = Request.QueryString["id"];
            string type = Request.QueryString["Type"];
            //提醒保存
            var warning = new NtErp.Crm.Services.Views.WorkWarningsAlls().Where(item => item.MainID == id && item.Type == (WarningType)int.Parse(type)).SingleOrDefault();
            var warn = warning ?? new NtErp.Crm.Services.Models.WorkWarning();
            switch ((WarningType)int.Parse(type))
            {
                case WarningType.CommentWorksWeekly:  //工作周报点评
                    warn.Summary = "工作周报点评提醒";
                    warn.Admin = reply.WorksWeekly.Admin;
                    break;
                case WarningType.CommentWorksOther:   //工作计划
                    warn.Summary = "工作计划点评提醒";
                    warn.Admin = reply.WorksOther.Admin;
                    break;
                case WarningType.CommentTrace:         //客户跟踪记录
                    warn.Summary = "客户跟踪记录点评提醒";
                    warn.Admin = reply.Report.Admin;
                    break;
                case WarningType.ProjectComment:       //销售机会跟踪记录
                    warn.Summary = "销售机会跟踪记录点评提醒";
                    warn.Admin = reply.Report.Admin;
                    break;
            }
            warn.Type = (WarningType)int.Parse(type);
            warn.Resource = Needs.Erp.ErpPlot.Current.RealName;
            warn.MainID = id;
            warn.Status = WarningStatus.unread;
            warn.Enter();
        }
    }
}