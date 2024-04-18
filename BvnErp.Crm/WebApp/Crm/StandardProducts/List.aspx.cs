using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Erp.Generic;
using Needs.Utils.Serializers;

namespace WebApp.Crm.StandardProducts
{
    /// <summary>
    /// 标准产品展示页面
    /// </summary>
    public partial class List  : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载数据
        /// </summary>
        protected void data()
        {
            string Name = Request.QueryString["Name"];
            var data = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts.OrderByDescending(c=>c.CreateDate).Where(item => true);
            if (!string.IsNullOrWhiteSpace(Name))
            {
                data = data.Where(item => item.Name == Name);
            }

            //数据转化为前台展示所需数据
            Func<NtErp.Crm.Services.Models.StandardProduct, object> convert = item => new
            {
                item.ID,
                item.Origin,
                item.Name,
                item.Packaging,
                item.PackageCase,
                item.Batch,
                item.DateCode,
                VendorName = item.Manufacturer.Name,
            };

            this.Paging(data, convert);
        }
    }
}