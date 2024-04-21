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
    public partial class UnReport : ErpParticlePage
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
                    var log = Erp.Current.Erm.Logs_StaffApprovalAll.Single(item => item.ApprovalStep == StaffApprovalStep.Entry && item.StaffID == staff.ID);
                    log.EntryReportStatus = StaffEntryReportStatus.UnReport;
                    log.AdminID = Erp.Current.ID;
                    log.Summary = Summary;
                    log.Enter();
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