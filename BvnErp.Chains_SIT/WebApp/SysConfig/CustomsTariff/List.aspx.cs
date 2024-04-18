using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.CustomsTariff
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void data()
        {
            var tariffs = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.AsQueryable();

            string code = Request.QueryString["HSCode"];
            string name = Request.QueryString["Name"];

            if (!string.IsNullOrEmpty(code))
            {
                tariffs = tariffs.Where(item => item.HSCode.Contains(code));
            }

            if (!string.IsNullOrEmpty(name))
            {
                tariffs = tariffs.Where(item => item.Name.Contains(name));
            }

            Func<Needs.Ccs.Services.Models.CustomsTariff, object> convert = tariff => new
            {
                tariff.ID,
                tariff.HSCode,
                tariff.Name,
                tariff.MFN,
                tariff.General,
                tariff.AddedValue,
                tariff.Consume,
                tariff.Elements,
                tariff.RegulatoryCode,
                tariff.Unit1,
                tariff.Unit2,
                tariff.CIQCode
            };

            this.Paging(tariffs, convert);
        }

        /// <summary>
        /// 删除税则
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs[id];

            if (tariff != null)
            {
                tariff.AbandonSuccess += Tariff_AbandonSuccess;
                tariff.Abandon();
            }
        }

        /// <summary>
        /// 删除税则成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tariff_AbandonSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Alert("删除成功!");
        }
    }
}