using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.DomesticExpress.Company
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageInit();
        }
        private void PageInit()
        {
            this.Model.AllData = "".Json();
            string id = Request.QueryString["ID"];
            var Companies = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies[id];
            if (Companies != null)
            {
                this.Model.AllData = new
                {
                    ID = id,
                    Name = Companies.Name,
                    Code = Companies.Code,
                    CustomerName = Companies.CustomerName,
                    CustomerPwd = Companies.CustomerPwd,
                    MonthCode = Companies.MonthCode,
                }.Json();
            }
        }
        protected void Save()
        {
            var ID = Request.Form["ID"];
            var Name = Request.Form["Name"];
            var Code = Request.Form["Code"];
            var CustomerName = Request.Form["CustomerName"];
            var CustomerPwd = Request.Form["CustomerPwd"];
            var MonthCode = Request.Form["MonthCode"];
            var company = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies[ID] as
             Needs.Ccs.Services.Models.ExpressCompany ;
            company.Name = Name;
            company.Code = Code;
            company.CustomerName = CustomerName;
            company.CustomerPwd = CustomerPwd;
            company.MonthCode = MonthCode;
            company.EnterSuccess += Company_EnterSuccess;
            company.EnterError += Company_EnterError;
            company.Enter();
        }

        private void Company_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Company_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}