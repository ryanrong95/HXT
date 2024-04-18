using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Common;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Applications.Receivables
{
    public partial class ExamineReject : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Submit()
        {
            try
            {
                ////基本信息
                string ID = Request.Form["ID"];
                string Summary = Request.Form["Summary"].Trim();
                var application = Erp.Current.WsOrder.Applications.SingleOrDefault(item => item.ID == ID);

                //审批驳回
                var log = new Application_Logs()
                {
                    ApplicationID = ID,
                    AdminID = Erp.Current.ID,
                    StepName = "跟单审核",
                    Status = PvWsOrder.Services.Enums.ApprovalStatus.Reject,
                    Summary = Summary,
                };
                application.Examine(false, log);

                Response.Write((new { success = true, message = "成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "失败：" + ex.Message }).Json());
            }
        }
    }
}