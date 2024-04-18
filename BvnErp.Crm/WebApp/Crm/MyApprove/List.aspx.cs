using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Crm.MyApprove
{
    /// <summary>
    /// 我的审核展示页面
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
                SetDropDownList();
            }
        }

        private void SetDropDownList()
        {
            this.Model.Admin = Needs.Erp.ErpPlot.Current.ClientSolutions.MyStaffs.Select(item => new { item.ID, item.RealName }).Json();
            this.Model.Status = EnumUtils.ToDictionary<ApplyStatus>().Select(item => new { value = item.Key, text = item.Value }).Json();
        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string adminid = Request.QueryString["AdminID"];
            string status = Request.QueryString["Status"];
            var MyApply = new NtErp.Crm.Services.Views.ApplyAlls().Where(item => true);

            if (!string.IsNullOrWhiteSpace(adminid))
            {
                MyApply = MyApply.Where(item => item.Admin.ID == adminid);
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                MyApply = MyApply.Where(item => item.Status == (ApplyStatus)int.Parse(status));
            }

            Func<NtErp.Crm.Services.Models.Apply, object> convert = item => new
            {
                item.ID,
                item.Type,
                item.Status,
                item.CreateDate,
                TypeName = item.Type.GetDescription(),
                item.MainID,
                AdminName = item.Admin.RealName,
                StatusName = item.Status.GetDescription(),
            };

            this.Paging(MyApply, convert);
        }
    }
}