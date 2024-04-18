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
    /// 品牌管理展示页面
    /// </summary>
    public partial class ManufactureList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(c => c.Type == CompanyType.Manufacture)
                .OrderByDescending(c => c.CreateDate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                company = company.Where(c => c.Name.Contains(Name));
            }
            this.Paging(company);
        }
    }
}