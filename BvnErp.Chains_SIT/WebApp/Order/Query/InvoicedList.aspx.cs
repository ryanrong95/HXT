using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Query
{
    public partial class InvoicedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
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
            string InvoiceType = Request.QueryString["InvoiceType"];
            string CompanyName = Request.QueryString["CompanyName"];
            string ClientCode = Request.QueryString["ClientCode"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var noticeList = new Needs.Ccs.Services.Views.InvoiceNotices1View();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<InvoiceNotice, bool>> expression = item => item.Client.Merchandiser.ID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            #region 页面查询条件

            if (string.IsNullOrEmpty(InvoiceType) == false)
            {
                int type = int.Parse(InvoiceType);
                Expression<Func<InvoiceNotice, bool>> lambda1 = item => (int)item.InvoiceType == type;
                lamdas.Add(lambda1);
            }
            if (string.IsNullOrEmpty(CompanyName) == false)
            {
                Expression<Func<InvoiceNotice, bool>> lambda1 = item => item.Client.Company.Name.Contains(CompanyName);
                lamdas.Add(lambda1);
            }
            if (string.IsNullOrEmpty(ClientCode) == false)
            {
                Expression<Func<InvoiceNotice, bool>> lambda1 = item => item.Client.ClientCode == ClientCode;
                lamdas.Add(lambda1);
            }
            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start = Convert.ToDateTime(StartDate);
                Expression<Func<InvoiceNotice, bool>> lambda1 = item => item.CreateDate >= start;
                lamdas.Add(lambda1);
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                Expression<Func<InvoiceNotice, bool>> lambda1 = item => item.CreateDate < end;
                lamdas.Add(lambda1);
            }

            #endregion

            #region 页面需要数据

            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var notices = noticeList.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = notices.Select(
                        item => new
                        {
                            ID = item.ID,
                            ClientCode = item.Client.ClientCode,
                            CompanyName = item.Client.Company.Name,
                            InvoiceType = item.InvoiceType.GetDescription(),
                            Amount = item.Amount.ToRound(2),
                            DeliveryType = item.ClientInvoice.DeliveryType.GetDescription(),
                            WaybillCode = item.WaybillCode,
                            Status = item.InvoiceNoticeStatus.GetDescription(),
                            CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                            ApplyName = item.Apply.RealName,
                            InvoiceNoSummary = item.InvoiceNoSummary
                        }
                     ).ToArray(),
                total = notices.Total,
            }.Json());

            #endregion
        }

        /// <summary>
        /// 删除开票通知
        /// </summary>
        protected void CancelApply()
        {
            string InvoiceNoticeID = Request.Form["InvoiceNoticeID"];
            try
            {
                var invoiceNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.InvoiceNotice.Where(item => item.ID == InvoiceNoticeID).FirstOrDefault();
                invoiceNotice.Abandon();
                Response.Write((new { success = true, message = "删除成功" }).Json());
            }
            catch(Exception ex)
            {
                ex.CcsLog("开票通知删除错误："+ InvoiceNoticeID);
                Response.Write((new { success = false, message = "删除失败" }).Json());
            }
        }
    }
}