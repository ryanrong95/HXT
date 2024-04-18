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

namespace WebApp.PayExchange.AuditedStyleUse
{
    public partial class List : Uc.PageBase
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
            this.Model.PayExchangeApplyStatus = EnumUtils.ToDictionary<PayExchangeApplyStatus>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var applyView = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyPayExchangeApply
                .Where(item => item.PayExchangeApplyStatus >= PayExchangeApplyStatus.Audited).AsQueryable();

            if (!string.IsNullOrEmpty(ClientCode))
            {
                applyView = applyView.Where(item => item.Client.ClientCode == ClientCode);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var start = Convert.ToDateTime(StartDate);
                applyView = applyView.Where(item => item.CreateDate >= start);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var end = Convert.ToDateTime(EndDate).AddDays(1);
                applyView = applyView.Where(item => item.CreateDate < end);
            }
            Func<AdminPayExchangeApply, object> linq = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate,
                ClientCode = item.Client.ClientCode,
                SupplierName = item.SupplierName,
                SupplierEnglishName = item.SupplierEnglishName,
                BankName = item.BankName,
                BankAccount = item.BankAccount,
                Status = item.PayExchangeApplyStatus.GetDescription(),
            };
            applyView = applyView.OrderByDescending(item => item.CreateDate);
            this.Paging(applyView, linq);
        }
    }
}