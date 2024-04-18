using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyApprove
{
    public partial class ProductView : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var Itemid = Request.QueryString["itemid"];
                var product = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByID(Itemid);
                var project = new NtErp.Crm.Services.Views.ProjectAlls().SearchByItemID(Itemid);
                if (product != null)
                {
                    this.Model.ProductData = new
                    {
                        project.ClientName,
                        project.CompanyName,
                        project.Name,
                        project.ProductName,
                        IndustryName = project.Industry?.Name,
                        CurrencyName = project.Currency.GetDescription(),
                        ItemID = product.ID,
                        ItemName = product.standardProduct.Name,
                        ItemOrigin = product.standardProduct.Origin,
                        ManufactureName = product.standardProduct.Manufacturer.Name,
                        product.RefUnitQuantity,
                        product.RefQuantity,
                        product.RefUnitPrice,
                        StatusName = product.Status.GetDescription(),
                        ExpectDate = product.ExpectDate?.ToString("yyyy-MM-dd"),
                        product.ExpectRate,
                        product.ExpectQuantity,
                        CompeteManu = product.CompeteProduct?.ManufacturerID,
                        CompeteModel = product.CompeteProduct?.Name,
                        CompetePrice = product.CompeteProduct?.UnitPrice,
                        product.SaleAdminName,
                        product.AssistantAdiminName,
                        product.PurchaseAdminName,
                        product.PMAdminName,
                        product.FAEAdminName,
                        ReportDate = product.ReportDate?.ToString("yyyy-MM-dd"),

                        SampleType = product.Sample?.Type.GetDescription(),
                        SampleDate = product.Sample?.Date.ToString("yyyy-MM-dd"),
                        SampleQuantity = product.Sample?.Quantity,
                        SamplePrice = product.Sample?.UnitPrice,
                        SampleContactor = product.Sample?.Contactor,
                        SamplePhone = product.Sample?.Phone,
                        SampleAddress = product.Sample?.Address,

                        EnquiryReplyDate = product.Enquiry?.ReplyDate.ToString("yyyy-MM-dd"),
                        EnquiryReplyPrice = product.Enquiry?.ReplyPrice,
                        EnquiryRFQ = product.Enquiry?.RFQ,
                        EnquiryOriginModel = product.Enquiry?.OriginModel,
                        EnquiryMOQ = product.Enquiry?.MOQ,
                        EnquiryMPQ = product.Enquiry?.MPQ,
                        EnquiryCurrency = product.Enquiry?.Currency.GetDescription(),
                        EnquiryERate = product.Enquiry?.ExchangeRate,
                        EnquiryTRate = product.Enquiry?.TaxRate,
                        EnquiryTariff = product.Enquiry?.Tariff,
                        EnquiryOtherRate = product.Enquiry?.OtherRate,
                        EnquiryCost = product.Enquiry?.Cost,
                        EnquiryValidity = product.Enquiry?.Validity.ToString("yyyy-MM-dd"),
                        EnquiryValidityCount = product.Enquiry?.ValidityCount,
                        EnquirySalePrice = product.Enquiry?.SalePrice,
                        EnquirySummary = product.Enquiry?.Summary,
                    }.Json();
                    this.Model.Files = product.Files.Json();
                }
                else
                {
                    this.Model.Files = product.Json();
                    this.Model.ProductData = product.Json();
                }
            }
        }

        /// <summary>
        /// 审批记录展示
        /// </summary>
        protected void data()
        {
            var applyid = Request.QueryString["ID"];
            var applysteps = new NtErp.Crm.Services.Views.ApplyStepAlls().Where(item => item.ApplyID == applyid);
            Func<NtErp.Crm.Services.Models.ApplyStep, object> convert = item => new
            {
                item.AdminName,
                StatusName = item.Status.GetDescription(),
                item.AprDate,
                item.Comment,
            };
            this.Paging(applysteps, convert);
        }
    }
}