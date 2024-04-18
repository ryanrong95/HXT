using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class WrapTypeInfo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Save()
        {
            Needs.Ccs.Services.Models.WrapType wrapType = new Needs.Ccs.Services.Models.WrapType();
            wrapType.Code = Request.Form["Code"];
            wrapType.Name = Request.Form["Name"];
            wrapType.EnterError += WrapType_EnterError;
            wrapType.EnterSuccess += WrapType_EnterSuccess;
            wrapType.Enter();
        }
        private void WrapType_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WrapType_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }
}