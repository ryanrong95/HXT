using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NtErp.Crm.Services;
using Needs.Utils.Serializers;
using NtErp.Crm.Services.Enums;

namespace WebApp.Crm.StandardProducts
{
    /// <summary>
    /// 标准编辑页面
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PageInit();
            }
        }

        /// <summary>
        /// 页面数据初始化
        /// </summary>
        void PageInit()
        {
            this.Model.VendorData = Needs.Erp.ErpPlot.Current.ClientSolutions.MyManufactures.Select(item => new { item.ID, item.Name }).
                OrderBy(item => item.Name).Json();
            string id = Request.QueryString["id"];
            this.Model.AllData = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[id].Json();

        }

        /// <summary>
        /// 保存
        /// </summary>
        protected void btnSave_Click(object sender, EventArgs e)
        {
            string id = Request.QueryString["id"];
            var Stand = Needs.Erp.ErpPlot.Current.ClientSolutions.StandardProducts[id] as
             NtErp.Crm.Services.Models.StandardProduct ?? new NtErp.Crm.Services.Models.StandardProduct();
            Stand.Origin = Request.Form["Origin"];
            Stand.Name = Request.Form["Name"];
            string vendorid = Request.Form["VendorID"];
            Stand.Manufacturer = Needs.Underly.FkoFactory<NtErp.Crm.Services.Models.Company>.Create(vendorid);
            Stand.Packaging = Request.Form["Packaging"];
            Stand.PackageCase = Request.Form["PackageCase"];
            Stand.Batch = Request.Form["Batch"];
            Stand.DateCode = Request.Form["DateCode"];
            Stand.EnterSuccess += Contact_EnterSuccess;
            Stand.Enter();
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contact_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            var url = Request.UrlReferrer ?? Request.Url;
            this.Alert("保存成功", url, true);
        }

    }
}