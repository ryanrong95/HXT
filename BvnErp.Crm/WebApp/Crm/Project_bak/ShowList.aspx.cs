using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Project_bak
{
    public partial class ShowList : Uc.PageBase
    {

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        #region 加载显示数据

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void LoadData()
        {
            //string id = Request.QueryString["ID"];
            //var projectdossier = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.GetTop(1, item => item.Project.ID == id).SingleOrDefault();
            //if (projectdossier != null)
            //{
            //    this.Model.Project = new
            //    {
            //        projectdossier.Project.ID,
            //        projectdossier.Project.Name,
            //        TypeName = projectdossier.Project.Type.GetDescription(),
            //        ClientName = projectdossier.Project.Client.Name,
            //        CompanyName = projectdossier.Project.Company.Name,
            //        IndustryName = projectdossier.Industries.SingleOrDefault()?.Name,
            //        CurrencyName = projectdossier.Project.Currency.GetDescription(),
            //        projectdossier.Project.StartDate,
            //        projectdossier.Project.EndDate,
            //        projectdossier.Project.Summary,
            //    }.Json();
            //}
            //else
            //{
            //    this.Model.Project = projectdossier.Json();
            //}

        }

        /// <summary>
        /// 查询列表数据
        /// </summary>
        protected void data()
        {
            //string id = Request.QueryString["ID"];
            //var products = Needs.Erp.ErpPlot.Current.ClientSolutions.MyProjects.MyProducts(id);

            //Func<NtErp.Crm.Services.Models.ProductItem, object> convert = item => new
            //{
            //    item.ID,
            //    item.standardProduct.Name,
            //    item.RefUnitQuantity,
            //    item.RefQuantity,
            //    item.RefUnitPrice,
            //    item.RefTotalPrice,
            //    item.ExpectRate,
            //    item.ExpectTotal,
            //    item.Quantity,
            //    item.UnitPrice,
            //    item.TotalPrice,
            //    item.Status,
            //    item.Count,
            //    CompeteManu = item.CompeteProduct?.ManufacturerID,
            //    CompeteModel = item.CompeteProduct?.Name,
            //    CompetePrice = item.CompeteProduct?.UnitPrice,
            //    item.OriginNumber,
            //    StatusName = item.Status.GetDescription(),
            //    VendorID = item.standardProduct.Manufacturer.ID,
            //    VendorName = item.standardProduct.Manufacturer.Name,
            //    item.ExpectDate,
            //    item.IsApr,
            //    item.FAEAdminName,
            //    item.PMAdminName,
            //};

            //this.Paging(products, convert);
        }
        #endregion
    }
}