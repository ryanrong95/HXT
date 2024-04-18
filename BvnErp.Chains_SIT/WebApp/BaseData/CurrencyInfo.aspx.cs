using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class CurrencyInfo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void Save()
        {
            Needs.Ccs.Services.Models.Currency currency = new Needs.Ccs.Services.Models.Currency();
            currency.Code = Request.Form["Code"];
            currency.Name = Request.Form["Name"];
            currency.EnglishName = Request.Form["EnglishName"];
            currency.EnterError += Currency_EnterError;
            currency.EnterSuccess += Currency_EnterSuccess;
            currency.Enter();
        }

        private void Currency_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Currency_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }


    }
}