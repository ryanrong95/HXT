using Needs.Ccs.Services.Enums;
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
    public partial class ApprovedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            this.Model.BankData = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapBanks.Select(item => new { value = item.ID, text = item.Name }).Json();
        }

        /// <summary>
        /// 加载换汇通知
        /// </summary>
        protected void data()
        {
            string BankName = Request.QueryString["BankName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.SwapNotice.AsQueryable()
                .Where(item => item.SwapStatus == SwapStatus.ApprovedAudit );//SwapStatus.Auditing

            if (!string.IsNullOrEmpty(BankName))
            {
                BankName = BankName.Trim();
                notices = notices.Where(t => t.BankName == BankName);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                notices = notices.Where(t => t.CreateDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                notices = notices.Where(t => t.CreateDate < end);
            }

            Func<SwapNotice, object> convert = item => new
            {
                ID = item.ID,
                Creator = item.Admin.RealName,
                item.Currency,
                item.TotalAmount,
                item.BankName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                SwapStatus = item.SwapStatus.GetDescription(),
                SwapStatusInt = item.SwapStatus,
                item.ConsignorCode,
            };
            this.Paging(notices, convert);
        }
    }
}