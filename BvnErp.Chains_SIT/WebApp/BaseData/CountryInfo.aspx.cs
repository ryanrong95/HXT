using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.BaseData
{
    public partial class CountryInfo : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void CountrySave()
        {
            Needs.Ccs.Services.Models.Country country = new Needs.Ccs.Services.Models.Country();
            country.Code= Request.Form["Code"];
            country.Name= Request.Form["Name"];
            country.EnglishName= Request.Form["EnglishName"];
            country.EnterError += Country_EnterError;
            country.EnterSuccess += Country_EnterSuccess;
            country.Enter();
        }

        private void Country_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Country_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }
    }

}