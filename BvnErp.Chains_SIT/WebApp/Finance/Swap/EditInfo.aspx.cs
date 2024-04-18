using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Swap
{
    public partial class EditInfo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        protected void LoadData()
        {
            string ID = Request.QueryString["ID"];
            //金库
            this.Model.VaultData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinanceVault.Select(item => new { Value = item.ID, Text = item.Name }).Json();

            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice[ID];
            this.Model.AllData = new
            {
                SwapNoticeID = ID,
                TotalAmount = notice.TotalAmount,
                BankName = notice.BankName,
                Currency = notice.Currency,
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
                DecHeadID = item.SwapDecHead.ID,
            };
            Response.Write(new
            {
                rows = items.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 从换汇通知中移除一个报关单
        /// </summary>
        protected void DeleteDecHeadFromSwapNotice()
        {
            try
            {
                string SwapNoticeID = Request.Form["SwapNoticeID"];
                string DeleteDecHeadID = Request.Form["DeleteDecHeadID"];

                Needs.Wl.Finance.Services.Models.DeleteDecFromSwapNoticeHandler handler = 
                    new Needs.Wl.Finance.Services.Models.DeleteDecFromSwapNoticeHandler(SwapNoticeID, DeleteDecHeadID);
                handler.Execute();

                Response.Write((new { success = true, message = "", }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "错误：" + ex.Message, }).Json());
            }

        }

    }
}