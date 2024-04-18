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

namespace WebApp.Finance.Payment.PayExchange
{
    public partial class UnApprovedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据加载
        /// </summary>
        //protected void data()
        //{
        //    string ClientCode = Request.QueryString["ClientCode"];
        //    string StartDate = Request.QueryString["StartDate"];
        //    string EndDate = Request.QueryString["EndDate"];

        //    var applyView = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.UnApprovalPayExchangeApply.AsQueryable();
        //    if (!string.IsNullOrEmpty(ClientCode))
        //    {
        //        applyView = applyView.Where(item => item.Client.ClientCode == ClientCode);
        //    }
        //    if (!string.IsNullOrEmpty(StartDate))
        //    {
        //        var start = Convert.ToDateTime(StartDate);
        //        applyView = applyView.Where(item => item.CreateDate >= start);
        //    }
        //    if (!string.IsNullOrEmpty(EndDate))
        //    {
        //        var end = Convert.ToDateTime(EndDate).AddDays(1);
        //        applyView = applyView.Where(item => item.CreateDate < end);
        //    }
        //    Func<UnApprovalPayExchangeApply, object> linq = item => new
        //    {
        //        ID = item.ID,
        //        CreateDate = item.CreateDate.ToShortDateString(),
        //        ClientCode = item.Client.ClientCode,
        //        SupplierName = item.SupplierName,
        //        SupplierEnglishName = item.SupplierEnglishName,
        //        BankName = item.BankName,
        //        BankAccount = item.BankAccount,
        //        Status = item.PayExchangeApplyStatus == PayExchangeApplyStatus.Audited ? "待审批" : item.PayExchangeApplyStatus.GetDescription(),
        //        Currency = item.Currency,
        //        TotalAmount = item.TotalAmount
        //    };
        //    applyView = applyView.OrderByDescending(item => item.CreateDate);
        //    this.Paging(applyView, linq);
        //}

        /// <summary>
        /// 数据加载 2020-09-30 by yeshuangshuang
        /// </summary>
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ID = Request.QueryString["ApplyID"];
            string OrderID = Request.QueryString["OrderID"];
            int AuditedInt = (int)PayExchangeApplyStatus.Audited;
            using (var query = new Needs.Ccs.Services.Views.ApprovalPayExchangeApplyView())
            {
                var view = query;
                if (!string.IsNullOrWhiteSpace(ClientCode))
                {
                    ClientCode = ClientCode.Trim();
                    view = view.SearchByClientCode(ClientCode);
                }

                if (!string.IsNullOrWhiteSpace(ID))
                {
                    view = view.SearchByID(ID.Trim());
                }
                if (!string.IsNullOrEmpty(StartDate))
                {
                    DateTime start = Convert.ToDateTime(StartDate);
                    view = view.SearchByStartDate(start);
                }
                if (!string.IsNullOrEmpty(EndDate))
                {
                    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByEndDate(end);
                }
                if (!string.IsNullOrEmpty(AuditedInt.ToString()))
                {
                    view = view.SearchByStatus(AuditedInt);
                }
                if (!string.IsNullOrWhiteSpace(OrderID))
                {
                    view = view.SearchByOrderID(OrderID);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }
    }
}
