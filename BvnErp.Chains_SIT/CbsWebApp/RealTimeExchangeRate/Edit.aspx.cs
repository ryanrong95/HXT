using Needs.Cbs.Services.Models.Origins;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Needs.Cbs.WebApp.RealTimeExchangeRate
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Model.CurrencyData = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.CustomsSettings[Services.Enums.BaseType.Currency].Select(item => new { value = item.Code, text = item.Name }).Json();
                string id = Request.QueryString["id"];
                this.Model.loaddata = new
                {
                    ID = id,
                }.Json();

                load();
            }
        }

        private void load()
        {
            string id = Request.QueryString["id"];
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.ExchangeRates[id];

            if (exchangeRate != null)
            {
                this.Model.ExchangeRateData = new
                {
                    exchangeRate.Code,
                    exchangeRate.Rate,
                    exchangeRate.Summary
                }.Json();
            }
            else
            {
                this.Model.ExchangeRateData = exchangeRate.Json();
            }
        }

        protected void Save()
        {
            string id = Request.Form["ID"];
            string rateType = Request.Form["RateType"];
            var exchangeRate = Needs.Wl.Admin.Plat.AdminPlat.Current.Cbs.ExchangeRates[id] as ExchangeRate ?? new ExchangeRate();

            exchangeRate.Changing += ExchangeRateChanging;
            exchangeRate.OnChanging();

            exchangeRate.Type = Needs.Cbs.Services.Enums.ExchangeRateType.RealTime;
            exchangeRate.Rate = decimal.Parse(Request.Form["Rate"]);
            exchangeRate.Code = Request.Form["Code"];
            exchangeRate.Summary = Request.Form["Summary"];

            exchangeRate.EnterError += ExchangeRate_EnterError;
            exchangeRate.EnterSuccess += ExchangeRate_EnterSuccess;
            exchangeRate.Enter();
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRate_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRate_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }

        /// <summary>
        /// 汇率操作前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExchangeRateChanging(object sender, Needs.Cbs.Services.Hanlders.ExchangeRateChangingEventArgs e)
        {
            if (string.IsNullOrEmpty(e.ExchangeRate.Code))
            {
                e.ExchangeRate.Log("管理员[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]新增了实时汇率");
            }
            else
            {
                e.ExchangeRate.Log("管理员[" + Needs.Wl.Admin.Plat.AdminPlat.Current.RealName + "]修改了实时汇率");
            }
        }
    }
}