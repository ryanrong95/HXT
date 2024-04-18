using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.PreProduct
{
    /// <summary>
    /// 预归类产品管控审批记录
    /// </summary>
    public partial class Record : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 初始化审批记录
        /// </summary>
        protected void data()
        {
            string model = Request.QueryString["Model"];
            string clientCode = Request.QueryString["ClientCode"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<PreProductControl, bool>> expression = item => true;

            #region 页面查询条件
            if (!string.IsNullOrWhiteSpace(model))
            {
                lamdas.Add((Expression<Func<PreProductControl, bool>>)(item => item.PreProduct.Model.Contains(model.Trim())));
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                lamdas.Add((Expression<Func<PreProductControl, bool>>)(item => item.PreProduct.Client.ClientCode.Contains(clientCode.Trim())));
            }
            #endregion

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var records = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyPreProductControlRecords.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = records.Select(
                       item => new
                       {
                           item.ID,
                           item.PreProduct.Client.ClientCode,
                           ClientName = item.PreProduct.Client.Company.Name,

                           item.PreProduct.Model,
                           item.PreProduct.Manufacturer,
                           item.Category.HSCode,
                           item.Category.ProductName,
                           item.Category.ClassifyFirstOperatorName,
                           item.Category.ClassifySecondOperatorName,
                           Type = item.Type.GetDescription(),
                           Status = item.Status.GetDescription(),
                           Approver = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                           CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                           ApproveDate = item.ApproveDate.ToString("yyyy-MM-dd HH:mm:ss"),
                       }
                    ).ToArray(),
                total = records.Total,
            }.Json());
            #endregion
        }
    }
}