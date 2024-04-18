using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.CustomExchangeRate
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
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.CustomRates.AsQueryable();

            if (!string.IsNullOrEmpty(code))
            {
                exchangeRate = exchangeRate.Where(item => item.Code.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                exchangeRate = exchangeRate.Where(item => item.Name.Contains(name));
            }
            Func<Needs.Ccs.Services.Models.CustomExchangeRate, object> convert = item => new
            {
                item.ID,
                item.Code,
                item.Name,
                item.Rate,
                item.Summary,
                CreateDate= item.CreateDate,
                UpdateDate = item.UpdateDate?.ToString("yyyy-MM-dd HH:mm:ss")
            };
            base.Paging(exchangeRate, convert);
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void DeleteExchangeRate()
        {
            string id = Request.Form["ID"];
            var account = Needs.Wl.Admin.Plat.AdminPlat.CustomRates[id];
            account.AbandonSuccessed += ExchangeRateAbandonSuccessed;
            account.Abandon();
        }

        /// <summary>
        /// 汇率删除后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRateAbandonSuccessed(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Needs.Ccs.Services.Models.CustomExchangeRate rate = e.Object as Needs.Ccs.Services.Models.CustomExchangeRate;
            rate.Log(Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID), "管理员[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]删除了海关汇率");
        }
    }
}