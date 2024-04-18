using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApp.Crm.Project
{
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadComboData();
                LoadData();
            }
        }

        #region 数据初始化加载
        /// <summary>
        /// 下拉框数据加载
        /// </summary>
        protected void LoadComboData()
        {
            this.Model.Admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).Json();
            this.Model.IsReport = EnumUtils.ToDictionary<IsProtected>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Status = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.SampleType = EnumUtils.ToDictionary<SampleType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Currency = EnumUtils.ToDictionary<CurrencyType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.Vender = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name })
                .OrderBy(item => item.Name).Json();
        }

        /// <summary>
        /// 加载销售机会数据
        /// </summary>
        protected void LoadData()
        {
            var Itemid = Request.QueryString["ItemID"];
            var projectId = Request.QueryString["ProjectID"];
            var project = new NtErp.Crm.Services.Views.ProjectAlls().SearchByID(projectId);
            var productitem = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByID(Itemid);
            
            this.Model.Project = new
            {
                project.ClientName,
                project.CompanyName,
                project.Name,
                project.ProductName,
                IndustryName = project.Industry?.Name,
                CurrencyName = project.Currency.GetDescription(),
            }.Json();
            if (productitem != null)
            {
                this.Model.ProductItem = new
                {
                    ItemID = productitem.ID,
                    ItemName = productitem.standardProduct.Name,
                    ItemOrigin = productitem.standardProduct.Origin,
                    ManufactureID = productitem.standardProduct.ManufacturerID,
                    productitem.Summary,
                    productitem.RefUnitQuantity,
                    productitem.RefQuantity,
                    productitem.RefUnitPrice,
                    productitem.Status,
                    productitem.ExpectRate,
                    productitem.ExpectDate,
                    productitem.ExpectQuantity,
                    productitem.ExpectTotal,
                    CompeteManu = productitem.CompeteProduct?.ManufacturerID,
                    CompeteModel = productitem.CompeteProduct?.Name,
                    CompetePrice = productitem.CompeteProduct?.UnitPrice,
                    productitem.SaleAdminID,
                    productitem.AssistantAdiminID,
                    productitem.PurchaseAdminID,
                    productitem.PMAdminID,
                    productitem.FAEAdminID,                    
                    SampleType = productitem.Sample?.Type,
                    SampleDate = productitem.Sample?.Date,
                    SampleQuantity = productitem.Sample?.Quantity,
                    SamplePrice = productitem.Sample?.UnitPrice,
                    SampleContactor = productitem.Sample?.Contactor,
                    SamplePhone = productitem.Sample?.Phone,
                    SampleAddress = productitem.Sample?.Address,

                }.Json();
                this.Model.Files = productitem.Files.Json();                
            }
            else
            {
                this.Model.ProductItem = productitem.Json();
                this.Model.Files = productitem.Json();                
            }
        }

        #endregion

        /// <summary>
        /// 获取询价信息数据
        /// </summary>
        protected void data()
        {
            var Itemid = Request.QueryString["ItemID"];
            var projectId = Request.QueryString["ProjectID"];
            var project = new NtErp.Crm.Services.Views.ProjectAlls().SearchByID(projectId);
            var productitem = new NtErp.Crm.Services.Views.ProductItemAlls().SearchByID(Itemid);

            IEnumerable<Enquiry> enquiries = productitem.Enquiries;

            Response.Write(new
            {
                rows = enquiries.Select(item => new
                {
                    ReportDate = item?.ReportDate.ToString("yyyy-MM-dd"),
                    item?.MOQ,
                    item?.MPQ,
                    item?.Currency,
                    Validity = item?.Validity.ToString("yyyy-MM-dd"),
                    item?.ValidityCount,
                    item?.SalePrice,
                    item?.Summary,
                }),
                total = enquiries.Count(),
            }.Json());
        }
    }
}