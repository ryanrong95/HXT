using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.DomesticExpress.Type
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            PageInit();
        }
        private void PageInit()
        {
            string ID = Request.QueryString["ID"];
            string CompanyID = Request.QueryString["CompanyID"];
            this.Model.AllData = "".Json();
            if (ID != null)
            {
                var type = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes[ID];
                this.Model.AllData = new
                {
                    ID = type.ID,
                    TypeName = type.TypeName,
                    TypeValue = type.TypeValue,
                    CompanyID = CompanyID
                }.Json();
            }
            else {
                this.Model.AllData = new
                {
                    CompanyID = CompanyID
                }.Json();
            }         
        }
        protected void Save()
        {
            var ExpressCompanyID = Request.Form["CompanyID"];
            var ID= Request.Form["ID"];
            var TypeName = Request.Form["TypeName"];
            var TypeValue = int.Parse(Request.Form["TypeValue"]);
            var ValueData = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == ExpressCompanyID)
                .Where(item => item.Status == Needs.Ccs.Services.Enums.Status.Normal).Where(item=>item.TypeValue==TypeValue).FirstOrDefault();
            if (ValueData != null&& ID=="undefined") {
                Response.Write((new { success = false, message = "保存失败,已有此快递方式的值" }).Json());
                return;
            }
            var type = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes[ID] as
             Needs.Ccs.Services.Models.ExpressType ?? new Needs.Ccs.Services.Models.ExpressType();
            type.ExpressCompanyID = ExpressCompanyID;
            type.TypeName = TypeName;
            type.TypeValue = TypeValue;
            type.EnterSuccess += Type_EnterSuccess;
            type.EnterError += Type_EnterError;
            type.Enter();
        }

        private void Type_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Type_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}