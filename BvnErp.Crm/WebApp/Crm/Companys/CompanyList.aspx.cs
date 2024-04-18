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
    /// 公司管理页面
    /// </summary>
    public partial class CompanyList : Uc.PageBase
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
            var company = Needs.Erp.ErpPlot.Current.ClientSolutions.Companys.Where(item=>item.Type == CompanyType.plot)
                .OrderByDescending(c => c.CreateDate).AsQueryable();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                company = company.Where(item => item.Name.Contains(Name));
            }
            this.Paging(company);
        }
    }
}