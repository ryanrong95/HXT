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
    public partial class ManagerApproval : ErpParticlePage
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
                var staff = Alls.Current.Staffs[id];
                if (staff != null)
                {
                    //审批通过日志
                    var log = new Logs_StaffApproval();
                    log.StaffID = staff.ID;
                    log.ApprovalStep = StaffApprovalStep.Manager;
                    log.ApprovalStatus = StaffApprovalStatus.Pass;
                    log.AdminID = Erp.Current.ID;
                    log.Summary = Summary;
                    log.Enter();

                    //通知入职报到日志
                    var log2 = new Logs_StaffApproval();
                    log2.StaffID = staff.ID;
                    log2.ApprovalStep = StaffApprovalStep.Notice;
                    log2.NoticeReportStatus = StaffNoticeReportStatus.UnNotified;
                    log2.Enter();

                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"经理审批通过", staff.Json());
                }
                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }

        protected void Fail()
        {
            try
            {
                string id = Request.Form["ID"];
                string Summary = Request.Form["Summary"];
                var staff = Alls.Current.Staffs[id];
                if (staff != null)
                {
                    var log = new Logs_StaffApproval();
                    log.StaffID = staff.ID;
                    log.ApprovalStep = StaffApprovalStep.Manager;
                    log.ApprovalStatus = StaffApprovalStatus.Fail;
                    log.AdminID = Erp.Current.ID;
                    log.Summary = Summary;
                    log.Enter();
                    Yahv.Oplogs.Oplog(Yahv.Erp.Current, Request.Url.ToString(), nameof(Yahv.Systematic.Erm), "员工信息",
                        $"经理审批未通过", staff.Json());
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