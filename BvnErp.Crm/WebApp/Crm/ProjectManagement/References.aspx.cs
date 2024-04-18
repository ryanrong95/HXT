using Needs.Linq;
using NtErp.Crm.Services.Models;
using NtErp.Crm.Services.Views.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.ProjectManagement
{
    /// <summary>
    /// 产品型号参考查询页面
    /// </summary>
    public partial class References : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
        }

        /// <summary>
        /// 产品型号参考价集合
        /// </summary>
        protected void data()
        {
            // 搜索条件
            Expression<Func<EnquiryReference, bool>> predicate = query();

            this.Paging(new ProductItemEnquiriesView().GetReferences().Where(predicate).OrderByDescending(t=>t.UpdateDate), item => new
            {
                OriginModel = item.OriginModel, // 型号
                Name = item.Name,
                Manufacturer = item.Manufacturer, // 品牌
                MOQ = item.MOQ, // 最小起订量
                MPQ = item.MPQ, // 最小包装量
                Validity = item.Validity?.ToString("yyyy-MM-dd"), // 有效时间
                ValidityCount = item.ValidityCount, // 有效数量
                CurrencyDes = item.CurrencyDes, // 币种
                SalePrice = item.SalePrice, // 参考售价
                Summary = item.Summary // 特殊备注
            });
        }

        /// <summary>
        /// 搜索条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<EnquiryReference, bool>> query()
        {
            string mf = Request["s_manufacturer"];
            string product = Request["s_name"];

            Expression<Func<EnquiryReference, bool>> predicate = item => true; // 条件拼接

            if (!string.IsNullOrEmpty(mf))
            {
                predicate = predicate.And(item => item.Manufacturer.ToUpper().Contains(mf.ToUpper()));
            }

            if (!string.IsNullOrEmpty(product))
            {
                predicate = predicate.And(item => item.Name.ToUpper().Contains(product.ToUpper()));
            }

            return predicate;
        }

    }
}