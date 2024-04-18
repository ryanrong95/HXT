using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney.Auditing
{
    public partial class CancelReason : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string From = Request.QueryString["From"];
            this.Model.From = From;
            if (!string.IsNullOrEmpty(id))
            {
                this.Model.ApplyID = id;
            }
        }

        /// <summary>
        /// 垫资申请拒绝
        /// </summary>
        protected void Save()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                string reason = Request.Form["Reason"];
                string From = Request.Form["From"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var summary = "";
                if (From == "Audit")
                {
                    summary = "风控【" + admin.RealName + "】拒绝了垫资申请；备注：" + reason;
                }
                else
                {
                    summary = "经理【" + admin.RealName + "】拒绝了垫资申请；备注：" + reason;
                }
                if (!string.IsNullOrEmpty(applyID))
                {
                    //修改垫资申请状态
                    Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApply = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                    {
                        ApplyID = applyID,
                        Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete,
                        UpdateDate = DateTime.Now,
                        Summary = reason,
                    };
                    Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        ApplyID = applyID,
                        Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Delete,
                        AdminID = admin.ID,
                        CreateDate = DateTime.Now,
                        Summary = summary
                    };

                    //保存 Begin

                    advanceMoneyApply.Delete();

                    advanceMoneyApplyLogs.Enter();

                    //保存 End
                    Response.Write((new { success = true, message = "保存成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "保存失败" }).Json());
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
    }
}