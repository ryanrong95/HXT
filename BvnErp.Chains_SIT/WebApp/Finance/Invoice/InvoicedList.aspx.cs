using Needs.Ccs.Services;
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

namespace WebApp.Finance
{
    public partial class InvoicedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboBoxData();
            }
        }

        protected void LoadComboBoxData()
        {
            //申请人
            this.Model.ApplyData = Needs.Wl.Admin.Plat.AdminPlat.Admins.Select(item => new { Value = item.ID, Text = item.RealName }).Json();
            //开票类型
            this.Model.InvoiceTypeData = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceType>().Select(item => new { Value = item.Key, Text = item.Value }).Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ApplyID = Request.QueryString["Apply"];
            string InvoiceType = Request.QueryString["InvoiceType"];
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string InvStartDate = Request.QueryString["InvStartDate"];
            string InvEndDate = Request.QueryString["InvEndDate"];
            string OrderID = Request.QueryString["OrderID"];
            string InvoiceNo = Request.QueryString["InvoiceNo"];
            string OutStockStatus = Request.QueryString["OutStockStatus"];

            if (!string.IsNullOrEmpty(OrderID) || !string.IsNullOrEmpty(InvoiceNo))
            {
                using (var query = new Needs.Ccs.Services.Views.InvoicedItemListView())
                {
                    var view = query;

                    if (string.IsNullOrEmpty(OrderID) == false)
                    {
                        view = view.SearchByOrderID(OrderID);
                    }
                    if (string.IsNullOrEmpty(InvoiceNo) == false)
                    {
                        view = view.SearchByInvoiceNo(InvoiceNo);
                    }
                   
                    Response.Write(view.ToMyPage(page, rows).Json());
                }
            }
            else
            {
                using (var query = new Needs.Ccs.Services.Views.InvoicedListView())
                {
                    var view = query;

                    if (string.IsNullOrEmpty(ApplyID) == false)
                    {
                        view = view.SearchByApplyID(ApplyID);
                    }
                    if (string.IsNullOrEmpty(InvoiceType) == false)
                    {
                        int type = int.Parse(InvoiceType);
                        view = view.SearchByInvoiceType(type);
                    }
                    if (string.IsNullOrEmpty(CompanyName) == false)
                    {
                        view = view.SearchByCompanyName(CompanyName);
                    }
                    if (string.IsNullOrEmpty(ClientCode) == false)
                    {
                        view = view.SearchByClientCode(ClientCode);
                    }
                    if (string.IsNullOrEmpty(StartDate) == false)
                    {
                        DateTime start = Convert.ToDateTime(StartDate);
                        view = view.SearchByStartDate(start);
                    }
                    if (string.IsNullOrEmpty(EndDate) == false)
                    {
                        DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                        view = view.SearchByEndDate(end);
                    }
                    if (string.IsNullOrEmpty(InvStartDate) == false)
                    {
                        DateTime start = Convert.ToDateTime(InvStartDate);
                        view = view.SearchByInvStartDate(start);
                    }
                    if (string.IsNullOrEmpty(InvEndDate) == false)
                    {
                        DateTime end = Convert.ToDateTime(InvEndDate).AddDays(1);
                        view = view.SearchByInvEndDate(end);
                    }
                    //
                    if (!string.IsNullOrEmpty(OutStockStatus) && OutStockStatus != "全部")
                    {
                        bool isstock = OutStockStatus == "0" ? false : true;
                        view = view.SearchByOutStockStatus(isstock);
                    }

                    Response.Write(view.ToMyPage(page, rows).Json());
                }
            }

            



            //var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.AsQueryable()
            //    .Where(item=>item.InvoiceNoticeStatus== InvoiceNoticeStatus.Confirmed);
            //if (string.IsNullOrEmpty(ApplyID) == false)
            //{
            //    notices = notices.Where(item => item.Apply.ID == ApplyID);
            //}
            //if (string.IsNullOrEmpty(InvoiceType) == false)
            //{
            //    int type = int.Parse(InvoiceType);
            //    notices = notices.Where(item => (int)item.InvoiceType == type);
            //}
            //if (string.IsNullOrEmpty(CompanyName) == false)
            //{
            //    notices = notices.Where(item => item.Client.Company.Name.Contains(CompanyName));
            //}
            //if (string.IsNullOrEmpty(ClientCode) == false)
            //{
            //    notices = notices.Where(item => item.Client.ClientCode == ClientCode);
            //}
            //if (string.IsNullOrEmpty(StartDate) == false)
            //{
            //    DateTime start = Convert.ToDateTime(StartDate);
            //    notices = notices.Where(item => item.CreateDate >= start);
            //}
            //if (string.IsNullOrEmpty(EndDate) == false)
            //{
            //    DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
            //    notices = notices.Where(item => item.CreateDate < end);
            //}
            ////前台显示
            //Func<InvoiceNotice, object> convert = item => new
            //{
            //    ID = item.ID,
            //    ClientCode = item.Client.ClientCode,
            //    CompanyName = item.Client.Company.Name,
            //    InvoiceType = item.InvoiceType.GetDescription(),
            //    Amount = item.Amount.ToRound(2),
            //    DeliveryType = item.ClientInvoice.DeliveryType.GetDescription(),
            //    WaybillCode = item.WaybillCode,
            //    Status = item.InvoiceNoticeStatus.GetDescription(),
            //    CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            //    ApplyName = item.Apply.RealName,
            //    InvoiceNoticeFileCount = item.InvoiceNoticeFileCount,
            //};

            //this.Paging(notices, convert);
        }

        protected void OutStock() {

            var IDs = Request["IDs"].Replace("&quot;", "").Trim().Split(',');
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => IDs.Contains(item.ID)).ToList();

            var msg = "";
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            foreach (var notice in data) {

                var push = new OutStock(notice, admin);

                msg += push.PushOutStock() + "<br />";
            }

            Response.Write((new { success = true, message = msg }).Json());
        }
    }
}