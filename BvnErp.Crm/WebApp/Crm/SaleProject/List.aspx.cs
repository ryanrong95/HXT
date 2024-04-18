using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.SaleProject
{
    /// <summary>
    /// 销售机会统计
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.DeclareStatusData = EnumUtils.ToDictionary<ProductStatus>().Select(item => new { text = item.Value, value = item.Key }).Json();
                DateTime dt = DateTime.Now;
                DateTime startMonth = dt.AddDays(1 - dt.Day).Date; //本月月初
                DateTime endMonth = startMonth.AddMonths(1).AddDays(-1).Date; //本月月末
                this.Model.StartDateData = startMonth;
                this.Model.EndDateData = endMonth;
                this.Model.Brands = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name }).OrderBy(item => item.Name).Json();
                var admins = new NtErp.Crm.Services.Views.AdminTopView().Select(item => new { item.ID, item.RealName }).ToArray();
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

            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        protected void data()
        {
            string SDate = Request.QueryString["SDate"];
            string EDate = Request.QueryString["EDate"];
            string ID = Request.QueryString["ID"];
            string DeclareStatusID = Request.QueryString["DeclareStatusID"];
            string adminID = Request["s_admin"];
            string brand = Request["s_brand"];
            string creatorID = Request["s_creator"];

            var projectstatistics = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjectStatistics.AsQueryable();
            if (!string.IsNullOrEmpty(creatorID) && creatorID != "0")
            {
                projectstatistics = projectstatistics.Where(t => t.ProjectAdminID == creatorID);
            }
            if (!string.IsNullOrEmpty(brand) && brand != "0")
            {
                projectstatistics = projectstatistics.Where(t => t.standardProduct.ManufacturerID == brand);
            }
            if (!string.IsNullOrEmpty(adminID) && adminID != "0")
            {
                projectstatistics = new NtErp.Crm.Services.Views.MyProjectStatisticsView(adminID).GetOwn();
            }

            if (!string.IsNullOrWhiteSpace(ID))  //projectID
            {
                projectstatistics = projectstatistics.Where(c => c.ProjectID.Contains(ID));
            }
            if (!string.IsNullOrWhiteSpace(DeclareStatusID))  //产品状态
            {
                var status = (ProductStatus)int.Parse(DeclareStatusID);
                projectstatistics = projectstatistics.Where(a => a.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(SDate))  //开始时间
            {
                projectstatistics = projectstatistics.Where(a => a.UpdateDate >= DateTime.Parse(SDate));
            }
            if (!string.IsNullOrWhiteSpace(EDate))  //结束时间
            {
                projectstatistics = projectstatistics.Where(a => a.UpdateDate <= DateTime.Parse(EDate));
            }
            //按照产品状态分组
            var data = from pro in projectstatistics
                       group pro by new { pro.ProjectID, pro.CreateDate, pro.ProjectUpdateDate, pro.ProjectType, pro.ProjectAdminName, pro.ProjectCurrency, pro.ProjectClientName, pro.ProjectCompanyName, pro.Status } into items
                       select new
                       {
                           items.Key.ProjectID,
                           items.Key.ProjectCompanyName,
                           items.Key.ProjectType,
                           items.Key.ProjectClientName,
                           items.Key.ProjectAdminName,
                           items.Key.ProjectCurrency,
                           items.Key.ProjectUpdateDate,
                           items.Key.Status,
                           RefValuation = items.Sum(item => (item.RefUnitPrice * item.RefQuantity) / 10000),
                           ExpectValuation = items.Sum(item => (item.RefUnitPrice * item.RefQuantity * item.ExpectRate) / 10000 / 100),
                       };

            //按币种统计机会总额和预计成交
            var totaldata = from project in projectstatistics.ToArray()
                            group project by project.ProjectCurrency into items
                            select new
                            {
                                ProjectCurrency = items.Key,
                                RefValuation = items.Sum(item => (item.RefUnitPrice * item.RefQuantity) / 10000),
                                ExpectValuation = items.Sum(item => (item.RefUnitPrice * item.RefQuantity * item.ExpectRate) / 10000 / 100),
                            };
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            int total = data.Count();
            var query = data.OrderByDescending(t => t.ProjectUpdateDate).Skip(rows * (page - 1)).Take(rows).ToArray();
            Response.Write(new
            {
                rows = query.Select(
                        c => new
                        {
                            c.ProjectID,
                            TypeName = c.ProjectType.GetDescription(),
                            ClientName = c.ProjectClientName,
                            CompanyName = c.ProjectCompanyName,
                            CurrencyName = c.ProjectCurrency.GetDescription(),
                            AdminName = c.ProjectAdminName,
                            UpdateDate = c.ProjectUpdateDate,
                            ExpectValuation = c.ExpectValuation.ToString("f5"),
                            RefValuation = c.RefValuation.ToString("f5"),
                            DeclareStatus = c.Status.GetDescription(),
                        }
                     ).ToArray(),
                total = total,
                totaldata = totaldata.Select(item => new
                {
                    CurrencyName = item.ProjectCurrency.GetDescription(),
                    RefValuation = item.RefValuation.ToString("f5"),
                    ExpectValuation = item.ExpectValuation.ToString("f5"),
                }).ToArray(),
            }.Json());
        }
    }
}