using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace WebApp.SysConfig.CustomsTariff
{
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化税则基本信息
        /// </summary>
        protected void LoadData()
        {
            string tariffID = Request.QueryString["ID"];

            if (!string.IsNullOrWhiteSpace(tariffID))
            {
                var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs[tariffID];

                this.Model.TariffData = new
                {
                    ID=tariff.ID,
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
                    tariff.CIQCode,
                    tariff.Summary,
                    tariff.InspectionCode,
                }.Json();
            }
            else
            {
                this.Model.TariffData = new
                {

                }.Json();
            }
        }

        /// <summary>
        /// 保存税则信息
        /// </summary>
        protected void Save()
        {
            string tariffID = Request.Form["ID"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs[tariffID] ?? new Needs.Ccs.Services.Models.CustomsTariff();

            tariff.HSCode = Request.Form["HSCode"];
            tariff.Name = Request.Form["Name"];
            tariff.MFN = decimal.Parse(Request.Form["MFN"]);
            tariff.General = decimal.Parse(Request.Form["General"]);
            tariff.AddedValue = decimal.Parse(Request.Form["AddedValue"]);
            if (!string.IsNullOrWhiteSpace(Request.Form["Consume"]))
                tariff.Consume = decimal.Parse(Request.Form["Consume"]);
            tariff.Elements = Request.Form["Elements"];
            tariff.RegulatoryCode = Request.Form["RegulatoryCode"];
            tariff.Unit1 = Request.Form["Unit1"];
            tariff.Unit2 = Request.Form["Unit2"];
            tariff.CIQCode = Request.Form["CIQCode"];
            tariff.Summary = Request.Form["Summary"];
            tariff.InspectionCode = Request.Form["InspectionCode"];

            tariff.EnterSuccess += Tariff_EnterSuccess;
            tariff.Enter();
        }

        /// <summary>
        /// 保存税则成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tariff_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功" }).Json());
        }
    }
}