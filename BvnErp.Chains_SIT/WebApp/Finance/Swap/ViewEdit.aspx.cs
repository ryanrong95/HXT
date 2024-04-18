using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap
{
    public partial class ViewEdit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            //金库
            this.Model.VaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Select(item => new { Value = item.ID, Text = item.Name }).Json();

            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];

            this.Model.AllData = new
            {
                ID = ID,
                Currency = notice.Currency,
                TotalAmount = notice.TotalAmount,
                BankName = notice.BankName
            }.Json();
        }

        /// <summary>
        /// 加载换汇通知
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var items = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNoticeItem.Where(item => item.SwapNoticeID == ID);
            Func<SwapNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                ContrNo = item.SwapDecHead.ContrNo,
                OrderID = item.SwapDecHead.OrderID,
                Currency = item.SwapDecHead.Currency,
                SwapAmount = item.Amount,  //item.SwapDecHead.SwapAmount,
                DDate = item.SwapDecHead.DDate?.ToString("yyyy-MM-dd HH:mm:ss"),
            };
            Response.Write(new
            {
                rows = items.Select(convert).ToArray()
            }.Json());
        }
        /// <summary>
        /// 审批通过
        /// </summary>
        protected void Save()
        {
            try
            {
                string ID = Request.Form["ID"];
                //查询
                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];

                notice.Approve();
                Needs.Ccs.Services.Models.SwapNoticeLog swapNoticeLog = new Needs.Ccs.Services.Models.SwapNoticeLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    SwapNoticeID = ID,
                    Admin = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                    Status = Needs.Ccs.Services.Enums.SwapStatus.Auditing,
                    CreateDate = DateTime.Now,
                    Summary ="操作人[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]提交了换汇申请审批通过，等待换汇",
                };
                swapNoticeLog.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 审批拒绝
        /// </summary>
        protected void UnApprove()
        {
            try
            {
                string ID = Request.Form["ID"];
                string Summary = Request.Form["Summary"];
                var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];
                notice.Summary = Summary;
                notice.unApprove();
                Needs.Ccs.Services.Models.SwapNoticeLog swapNoticeLog = new Needs.Ccs.Services.Models.SwapNoticeLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    SwapNoticeID = ID,
                    Admin = Needs.Wl.Admin.Plat.AdminPlat.Current.ID,
                    Status = Needs.Ccs.Services.Enums.SwapStatus.RefuseAudit,
                    CreateDate = DateTime.Now,
                    Summary ="操作人[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]提交了拒绝换汇申请，备注：" + Summary,
                };
                swapNoticeLog.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }

    }
}