using Projects = NtErp.Crm.Services.Models.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Crm.Services.Views.Projects;
using Needs.Utils.Descriptions;
using Needs.Linq;
using Needs.Utils.Serializers;

namespace WebApp.Crm.SaleChanceManagement
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName });
                this.Model.Admins = admins.Json();
                var currentAdmin = new NtErp.Crm.Services.Views.AdminTopView()[Needs.Erp.ErpPlot.Current.ID];
                if (currentAdmin.JobType == NtErp.Crm.Services.Enums.JobType.Sales)
                {
                    this.Model.Creators = admins.Where(t => t.ID == currentAdmin.ID).Json();
                }
                else
                {
                    this.Model.Creators = admins.Json();
                }
                this.Model.CurrentAdmin = currentAdmin;
                this.Model.Brands = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            Expression<Func<Projects.Project, bool>> predicate = query();
            var projectView = new ProjectsView(Needs.Erp.ErpPlot.Current).AsQueryable();
            string adminID = Request["s_admin"];
            string brand = Request["s_brand"];
            string creatorID = Request["s_creator"];

            if (!string.IsNullOrEmpty(creatorID) && creatorID != "0")
            {
                predicate = predicate.And(item => item.Admin.ID == creatorID);
            }
            if (!string.IsNullOrEmpty(brand) && brand != "0")
            {
                projectView = new ProjectsView(Needs.Erp.ErpPlot.Current).GetByManufacture(brand);
            }
            if (!string.IsNullOrEmpty(adminID) && adminID != "0")
            {
                projectView = new ProjectsView(adminID).GetOwn();
            }
            this.Paging(projectView.Where(predicate).OrderByDescending(t => t.UpdateDate), item => new
            {
                ClientName = item.Client.Name,
                item.ClientID,
                item.ID,
                ProjectName = item.Name,
                item.ProductName,
                Currency = item.Currency.GetDescription(),
                IndustryName = item.Industry.Name,
                CompanyName = item.Company.Name,
                ProjectType = item.Type.GetDescription(),
                item.StartDate,
                item.Contactor,
                item.Phone,
                ProductDate = item.ProductDate?.ToShortDateString(),
                item.MonthYield,
                item.ExpectTotal,
                AdminRealName = item.Admin.RealName,
                CreateDate = item.CreateDate.ToShortDateString(),
                UpdateDate = item.UpdateDate.ToShortDateString(),
            });

        }

        /// <summary>
        /// 查询销售机会下的具体产品
        /// </summary>
        protected void ListData()
        {
            var ProjectID = Request["ProjectID"];
            var ClientID = Request["ClientID"];

            var productItemsView = new ProductItemsView(ProjectID);

            this.Paging(productItemsView, item => new
            {
                item.ID,
                item.ProjectID,
                ClientID = ClientID,
                item.StandardProduct?.Name,
                item.StandardProduct?.Origin,
                VendorName = item.StandardProduct?.Manufacturer?.Name,
                StatusName = item.Status.GetDescription(),
                item.RefUnitQuantity,
                item.RefQuantity,
                item.RefUnitPrice,
                item.ExpectRate,
                ExpectDate = item.ExpectDate?.ToShortDateString(),
                item.ExpectQuantity,
                item.ExpectTotal,
                CompeteModel = item.CompeteProduct?.Name,
                CompeteManu = item.CompeteProduct?.ManufacturerID,
                CompetePrice = item.CompeteProduct?.UnitPrice,
                File = item.Voucher,

                item.Summary,
                SaleAdminName = item.SaleAdmin?.RealName,
                AssistantAdiminName = item.AssistantAdmin?.RealName,
                PMAdminName = item.PMAdmin?.RealName,
                PurchaseAdminName = item.PurChaseAdmin?.RealName,
                FAEAdminName = item.FAEAdmin?.RealName,

                IsSample = item.IsSample ? "是" : "否",
                SampleType = item.Sample?.Type.GetDescription(),
                SampleDate = item.Sample?.Date.ToShortDateString(),
                SampleQuantity = item.Sample?.Quantity,
                SamplePrice = item.Sample?.UnitPrice,
                SampleTotalPrice = item.Sample?.TotalPrice,
                SampleContactor = item.Sample?.Contactor,
                SamplePhone = item.Sample?.Phone,
                SampleAddress = item.Sample?.Address,

                ReportDate = item.Enquiry?.ReportDate.ToShortDateString(),
                MOQ = item.Enquiry?.MOQ,
                MPQ = item.Enquiry?.MPQ,
                EnquiryValidity = item.Enquiry?.Validity.ToShortDateString(),
                EnquiryValidityCount = item.Enquiry?.ValidityCount,
                EnquirySalePrice = item.Enquiry?.SalePrice,
                EnquirySummary = item.Enquiry?.Summary,
            });
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<Projects.Project, bool>> query()
        {
            string projectName = Request["s_projectName"];
            string clientName = Request["s_clientName"];

            Expression<Func<Projects.Project, bool>> predicate = item => true;

            if (!string.IsNullOrEmpty(projectName))
            {
                predicate = predicate.And(item => item.Name.Contains(projectName));
            }

            if (!string.IsNullOrEmpty(clientName))
            {
                predicate = predicate.And(item => item.Client.Name.Contains(clientName));
            }


            return predicate;
        }
    }
}