using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models.Generic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project
{
    /// <summary>
    /// 销售机会展示页面
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var clientid = Request.QueryString["ClientID"];
                this.Model.Client = new NtErp.Crm.Services.Views.ClientAlls()[clientid].Json();
            }
        }

        #region 数据加载
        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            var clientid = Request.QueryString["ClientID"];
            var View = new NtErp.Crm.Services.Views.ProjectAlls().SearchByClientID(clientid);

            Func<NtErp.Crm.Services.Models.Project, object> linq = item => new
            {
                item.ID,
                item.Name,
                item.ProductName,
                item.CompanyName,
                Currency = item.Currency.GetDescription(),
                IndustryName = item.Industry?.Name,
                Type = item.Type.GetDescription(),
                item.Contactor,
                StartDate = item.StartDate?.ToString("yyyy-MM-dd"),
                item.MonthYield,
                item.ExpectTotal,
                ProductDate = item.ProductDate?.ToString("yyyy-MM-dd"),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate= item.UpdateDate.ToString("yyyy-MM-dd"),
                AdminRealName = item.AdminName,
            };

            this.Paging(View.OrderByDescending(item=>item.UpdateDate), linq);
        }

        /// <summary>
        /// 查询产品项数据
        /// </summary>
        protected void ListData()
        {
            var ProjectId = Request.QueryString["ProjectID"];
            var View = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByProjectID(ProjectId).OrderByDescending(item => item.UpdateDate);

            #region 拼凑页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var data = View.Skip(rows * (page - 1)).Take(rows).ToArray();

            Response.Write(new
            {
                rows = data.Select(
                        item => new
                        {
                            item.ID,
                            ProjectID = ProjectId,
                            item.standardProduct.Name,
                            item.standardProduct.Origin,
                            VendorName = item.standardProduct.Manufacturer.Name,
                            item.RefUnitQuantity,
                            item.RefQuantity,
                            item.RefUnitPrice,
                            item.ExpectRate,
                            item.Status,
                            StatusName = item.Status.GetDescription(),
                            ExpectDate = item.ExpectDate?.ToString("yyyy-MM-dd"),
                            item.ExpectQuantity,
                            item.ExpectTotal,
                            CompeteManu = item.CompeteProduct?.ManufacturerID,
                            CompeteModel = item.CompeteProduct?.Name,
                            CompetePrice = item.CompeteProduct?.UnitPrice,
                            Files = item.Files,
                            FileName = item.Files?.SingleOrDefault(t => t.Type == NtErp.Crm.Services.Models.FileType.Item)?.Name,
                            FileUrl = item.Files?.SingleOrDefault(t => t.Type == NtErp.Crm.Services.Models.FileType.Item)?.Url,
                            item.FAEAdminName,
                            item.PMAdminName,
                            item.SaleAdminName,
                            item.AssistantAdiminName,
                            item.PurchaseAdminName,
                            item.IsApr,

                            IsSample = item.Sample?.ID != null ? "是" : "否",
                            SampleType = item.Sample?.Type.GetDescription(),
                            SampleDate = item.Sample?.Date.ToString("yyyy-MM-dd"),
                            SampleQuantity = item.Sample?.Quantity,
                            SamplePrice = item.Sample?.UnitPrice,
                            SampleTotalPrice = item.Sample?.TotalPrice,
                            SampleContactor = item.Sample?.Contactor,
                            SamplePhone = item.Sample?.Phone,
                            SampleAddress = item.Sample?.Address,

                            ReplyDate = item.Enquiry?.ReplyDate.ToString(),
                            ReplyPrice = item.Enquiry?.ReplyPrice,
                            RFQ = item.Enquiry?.RFQ,
                            OriginModel = item.Enquiry?.OriginModel,
                            ReportDate = item.Enquiry?.ReportDate.ToString("yyyy-MM-dd"),
                            MOQ = item.Enquiry?.MOQ,
                            MPQ = item.Enquiry?.MPQ,
                            EnquiryCurrency = item.Enquiry?.Currency.GetDescription(),
                            EnquiryERate = item.Enquiry?.ExchangeRate,
                            EnquiryTRate = item.Enquiry?.TaxRate,
                            EnquiryTariff = item.Enquiry?.Tariff,
                            EnquiryOtherRate = item.Enquiry?.OtherRate,
                            EnquiryCost = item.Enquiry?.Cost,
                            EnquiryValidity = item.Enquiry?.Validity.ToString("yyyy-MM-dd"),
                            EnquiryValidityCount = item.Enquiry?.ValidityCount,
                            EnquirySalePrice = item.Enquiry?.SalePrice,
                            EnquirySummary = item.Enquiry?.Summary,

                            Summary = item.Summary,
                        }
                     ).ToArray(),
                total = View.Count(),
            }.Json());
            #endregion
        }

        #endregion
    }
}