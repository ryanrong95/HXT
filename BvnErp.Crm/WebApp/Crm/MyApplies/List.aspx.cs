using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyApplies
{
    /// <summary>
    /// 我的申请展示页面
    /// </summary>
    public partial class List : Uc.PageBase
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

            }
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            var MyApply = Needs.Erp.ErpPlot.Current.ClientSolutions.MyApplies;

            Func<NtErp.Crm.Services.Models.Apply, object> convert = item => new
            {
                item.ID,
                TypeName = item.Type.GetDescription(),
                item.MainID,
                AdminName = item.Admin.UserName,
                StatusName = item.Status.GetDescription(),
                item.CreateDate,
                item.UpdateDate,
                item.Summary
            };

            this.Paging(MyApply, convert);
        }
    }
}