using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyApprove
{
    /// <summary>
    /// 产品销售状态审批页面
    /// </summary>
    public partial class ProductApr : Uc.PageBase
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
        /// 审批同意
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAllow_Click(object sender, EventArgs e)
        {
            this.Appr(ApplyStep.Allow);
            this.UpdateStatus(ApplyStep.Allow);
            Alert("操作成功", Request.Url, true);
        }


        /// <summary>
        /// 审批否决
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVote_Click(object sender, EventArgs e)
        {
            this.Appr(ApplyStep.Vote);
            this.UpdateStatus(ApplyStep.Vote);
            Alert("操作成功", Request.Url, true);
        }

        /// <summary>
        /// 审批记录
        /// </summary>
        /// <param name="applystep"></param>
        protected void Appr(ApplyStep applystep)
        {
            var appr = new NtErp.Crm.Services.Models.ApplyStep();
            appr.ApplyID = Request.Form["ApplyID"];
            appr.AdminID = Needs.Erp.ErpPlot.Current.ID;
            appr.Step = (int)applystep;
            appr.Status = applystep;
            appr.Comment = Request.Form["AprSummary"];
            appr.AprDate = DateTime.Now;
            appr.Enter();
        }

        /// <summary>
        /// 更新原数据状态
        /// </summary>
        /// <param name="applystep"></param>
        protected void UpdateStatus(ApplyStep applystep)
        {
            string applyid = Request.Form["ApplyID"];
            var apply = new NtErp.Crm.Services.Views.ApplyAlls()[applyid] as
                NtErp.Crm.Services.Models.Apply ?? new NtErp.Crm.Services.Models.Apply();
            apply.Status = (applystep == ApplyStep.Allow) ? ApplyStatus.Approval : ApplyStatus.NotApproval;

            var product = new NtErp.Crm.Services.Views.ProductItemAlls()[apply.MainID];
            if(applystep == ApplyStep.Allow)
            {
                product.Status = (ProductStatus)apply.Type;
                product.ExpectRate = (int)product.Status;
                product.StatusEnter();
            }

            apply.Enter();
        }
    }
}