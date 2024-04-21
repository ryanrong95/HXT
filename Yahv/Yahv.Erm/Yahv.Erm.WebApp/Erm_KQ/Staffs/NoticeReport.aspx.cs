using System;
using System.Linq;
using Yahv.Erm.Services;
using Yahv.Erm.Services.Models.Origins;
using Yahv.Erm.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using Yahv.Web.Forms;

namespace Yahv.Erm.WebApp.Erm_KQ.Staffs
{
    public partial class NoticeReport : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Pass()
        {
            try
            {
                string id = Request.Form["ID"];
                string Summary = Request.Form["Summary"];
                string ReportDate = Request.Form["ReportDate"];
                var staff = Alls.Current.Staffs[id];
                if (staff != null)
                {
                    //已通知
                    var log = new Logs_StaffApproval();
                    log.StaffID = staff.ID;
                    log.ApprovalStep = StaffApprovalStep.Notice;
                    log.NoticeReportStatus = StaffNoticeReportStatus.Notified;
                    log.AdminID = Erp.Current.ID;
                    log.Context = new Logs_StaffApprovalContext() { ReportDate = ReportDate, Summary = Summary }.Json();
                    log.Enter();

                    //入职报到日志
                    var log2 = new Logs_StaffApproval();
                    log2.StaffID = staff.ID;
                    log2.ApprovalStep = StaffApprovalStep.Entry;
                    log2.EntryReportStatus = StaffEntryReportStatus.WaitingReport;
                    log2.Context = new Logs_StaffApprovalContext() { ReportDate = ReportDate, Summary = Summary }.Json();
                    log2.Enter();

                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工入职",
                        $"行政通知入职", staff.Json());
                }
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }
    }
}