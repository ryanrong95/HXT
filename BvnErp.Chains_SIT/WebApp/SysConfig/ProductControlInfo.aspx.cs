using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig
{
    public partial class ProductControlInfo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ProductControlType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ProductControlType>().Select(item => new { ID = item.Key, Name = item.Value }).Json();
        }

        protected void Save()
        {
            Needs.Ccs.Services.Models.ProductControl productControl = new Needs.Ccs.Services.Models.ProductControl();
            productControl.Type = (Needs.Ccs.Services.Enums.ProductControlType)Enum.Parse(typeof(Needs.Ccs.Services.Enums.ProductControlType), Request.Form["Type"]);
            productControl.Name = Request.Form["Name"];
            productControl.Model = Request.Form["Model"];
            productControl.Manufacturer = Request.Form["Manufacturer"];
            productControl.EnterError += ProductControl_EnterError;
            productControl.EnterSuccess += ProductControl_EnterSuccess;
            productControl.Enter();
        }

        private void ProductControl_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProductControl_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}