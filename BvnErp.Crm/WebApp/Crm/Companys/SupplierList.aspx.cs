using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.Companys
{
    /// <summary>
    /// 供应商管理页面
    /// </summary>
    public partial class SupplierList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(c => c.Type == CompanyType.Supplier)
                .OrderByDescending(c => c.CreateDate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                company = company.Where(c => c.Name.Contains(Name));
            }
            this.Paging(company);
        }
    }
}