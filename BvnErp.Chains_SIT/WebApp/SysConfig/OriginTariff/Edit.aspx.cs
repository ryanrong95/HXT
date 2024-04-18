using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SysConfig.OriginTariff
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load();

            }
        }
        private void load()
        {
            this.Model.CustomsRateType = EnumUtils.ToDictionary<CustomsRateType>().Select(item => new { value = item.Key, text = item.Value }).Json();
            this.Model.OriginData=Needs.Wl.Admin.Plat.AdminPlat.Countries.Select(item => new { value = item.Code, text = item.Code+"-"+item.Name }).Json();
            this.Model.HscodeData= Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Take(10).Select(item => new { value = item.ID, text = item.HSCode }).Json();
            string id = Request.QueryString["id"];
            var originTariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.OriginTariffs[id];
            if (originTariff != null)
            {
                this.Model.OriginTariffData = new
                {
                    id=originTariff.ID,
                    HSCode = originTariff.HSCode,
                    Name = originTariff.Name,
                    Origin = originTariff.Origin,
                    CustomsRateType = (int)originTariff.Type,
                    Rate = originTariff.Rate,
                    StartDate=originTariff.StartDate.ToString("yyyy-MM-dd"),
                    EndDate=originTariff.EndDate?.ToString("yyyy-MM-dd"),
                }.Json();
            }
            else
            {
                this.Model.OriginTariffData = originTariff.Json();
            }
        }

        protected void Save()
        {
            string id = Request.Form["id"];
            var originTariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.OriginTariffs[id] as Needs.Ccs.Services.Models.CustomsOriginTariff ??
                new Needs.Ccs.Services.Models.CustomsOriginTariff();
            string HSCode = Request.Form["HSCode"];
            originTariff.CustomsTariffID = HSCode;
            originTariff.HSCode = Request.Form["HSCodeValue"];
            originTariff.Name = Request.Form["Name"];
            originTariff.Origin = Request.Form["Origin"];
            originTariff.StartDate = Convert.ToDateTime(Request.Form["StartDate"]);
            if (Request.Form["EndDate"] != "") {
                originTariff.EndDate = Convert.ToDateTime(Request.Form["EndDate"]);
            }            
            originTariff.Type = (CustomsRateType)int.Parse((Request.Form["CustomsRateType"]));
            originTariff.Rate = decimal.Parse(Request.Form["Rate"]);
            originTariff.EnterError += OriginTariff_EnterError;
            originTariff.EnterSuccess += OriginTariff_EnterSuccess;
            originTariff.Enter();
        }

        protected void getTariffName()
        {
            string name = "noResult";

            string id = Request.Form["Code"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs[id];
            if (tariff != null)
            {
                name = tariff.Name;
            }
            Response.Write(name);
        }

        protected object getDropdownlist()
        {
            string value = Request.Form["value"];
            return Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(items => items.HSCode.Contains(value)).Take(10).Select(item => new { value = item.ID, text = item.HSCode });
        }


        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginTariff_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write((new { success = false, message = "保存失败" }).Json());
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OriginTariff_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }
    }
}