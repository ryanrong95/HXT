using Needs.Cbs.Services.Enums;
using Needs.Cbs.Services.Models.Origins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.CustomExchangeRate
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            string code = Request.QueryString["Code"];
            string name = Request.QueryString["Name"];
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.ExchangeRates[ExchangeRateType.Custom];

            if (!string.IsNullOrEmpty(code))
            {
                exchangeRate = exchangeRate.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                exchangeRate = exchangeRate.Where(item => item.Name.Contains(name));
            }

            base.Paging(exchangeRate);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteExchangeRate()
        {
            string id = Request.Form["ID"];
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.ExchangeRates[id];
            exchangeRate.AbandonSuccess += ExchangeRateAbandonSuccessed;
            exchangeRate.Abandon();
        }

        /// <summary>
        /// 汇率删除后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRateAbandonSuccessed(object sender, Needs.Linq.SuccessEventArgs e)
        {
            ExchangeRate rate = e.Object as ExchangeRate;
            rate.Log( "管理员[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]删除了海关汇率");
        }
    }
}