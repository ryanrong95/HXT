using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Linq.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using YaHv.PvData.Services;
using YaHv.PvData.Services.Utils;


namespace Yahv.PvData.WebApp.SysConfig.ClassifyHistoryCopy
{
    public partial class List : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                init();
            }
        }

        void init()
        {
            this.Model.Admin = new
            {
                ID = Yahv.Erp.Current.ID,
                UserName = Yahv.Erp.Current.UserName,
                RealName = Yahv.Erp.Current.RealName
            }.Json();

            var pvdataApi = new PvDataApiSetting();

            this.Model.DomainUrls = new
            {
                PvDataApiUrl = ConfigurationManager.AppSettings[pvdataApi.ApiName],
            }.Json();
        }

        protected object data()
        {
            if (query() == null)
            {
                return new
                {
                    rows = "",
                    total = 0
                };
            }

            var view = Yahv.Erp.Current.PvData.StandardPartnumbersForPlugins.Where(query());

            //return this.Paging(view, item => new
            //{
            //    item.PartNumber,
            //    item.Manufacturer,
            //    item.HSCode,
            //    TariffName = item.Name,
            //    item.TaxCode,
            //    item.TaxName,
            //    item.LegalUnit1,
            //    item.LegalUnit2,
            //    ImportPreferentialTaxRate = item.TariffRate,
            //    item.VATRate,
            //    item.ExciseTaxRate,
            //    Elements = item.Elements.FixSpecialChars(),
            //    item.CIQCode,
            //    item.SpecialTypes,

            //    item.ID,
            //    item.Ccc,
            //    item.Embargo,
            //    item.HkControl,
            //    item.Coo,
            //    item.CIQ,
            //    item.CIQprice,
            //    item.Summary,
            //    OrderDate =  item.OrderDate?.ToString("yyyy-MM-dd"),

            //    //StandardTariffRate = new 
            //});


            Func<YaHv.PvData.Services.Models.StandardPartnumbersForPlugin, YaHv.PvData.Services.Models.StandardPartnumbersForPluginViewModel> convert = item => new YaHv.PvData.Services.Models.StandardPartnumbersForPluginViewModel
            {
                PartNumber = item.PartNumber,
                Manufacturer = item.Manufacturer,
                HSCode = item.HSCode,
                TariffName = item.Name,
                TaxCode = item.TaxCode,
                TaxName = item.TaxName,
                LegalUnit1 = item.LegalUnit1,
                LegalUnit2 = item.LegalUnit2,
                ImportPreferentialTaxRate = item.TariffRate,
                VATRate = item.VATRate,
                ExciseTaxRate = item.ExciseTaxRate,
                Elements = item.Elements.FixSpecialChars(),
                CIQCode = item.CIQCode,
                SpecialTypes = item.SpecialTypes,

                ID = item.ID,
                Ccc = item.Ccc,
                Embargo = item.Embargo,
                HkControl = item.HkControl,
                Coo = item.Coo,
                CIQ = item.CIQ,
                CIQprice = item.CIQprice,
                Summary = item.Summary,
                OrderDate = item.OrderDate?.ToString("yyyy-MM-dd"),
                StandardTariffRate = ""
            };

            var resultArr = view.Select(convert).ToArray();

            foreach (var item in resultArr)
            {
                var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll()[item.HSCode];
                var rate = tariff.ImportControlTaxRate.HasValue && (tariff.ImportControlTaxRate.Value < tariff.ImportPreferentialTaxRate) ? tariff.ImportControlTaxRate.Value : tariff.ImportPreferentialTaxRate;
                item.StandardTariffRate = (rate / 100).ToString();
            }

            return new
            {
                rows = resultArr,
                total = view.Count()
            };
        }

        /// <summary>
        /// 获取搜索查询条件
        /// </summary>
        /// <returns></returns>
        private Expression<Func<YaHv.PvData.Services.Models.StandardPartnumbersForPlugin, bool>> query()
        {

            #region 搜索条件
            var predicate = PredicateExtends.True<YaHv.PvData.Services.Models.StandardPartnumbersForPlugin>(); // 条件拼接

            string partNumber = Request["partNumber"];
            if (!string.IsNullOrEmpty(partNumber))
            {
                predicate = predicate.And(item => item.PartNumber.StartsWith(partNumber.Trim()));
            }
            else
            {
                return null;
            }

            string manufacturer = Request["manufacturer"];
            if (!string.IsNullOrEmpty(manufacturer))
            {
                predicate = predicate.And(item => item.Manufacturer.StartsWith(manufacturer.Trim()));
            }

            string hsCode = Request["hsCode"];
            if (!string.IsNullOrEmpty(hsCode))
            {
                predicate = predicate.And(item => item.HSCode.StartsWith(hsCode.Trim()));
            }

            string name = Request["name"];
            if (!string.IsNullOrEmpty(name))
            {
                predicate = predicate.And(item => item.Name.Contains(name.Trim()));
            }

            string startDate = Request["startDate"];
            if (!string.IsNullOrEmpty(startDate))
            {
                DateTime dtStart = Convert.ToDateTime(startDate);
                predicate = predicate.And(item => item.OrderDate >= dtStart);
            }

            string endDate = Request["endDate"];
            if (!string.IsNullOrEmpty(endDate))
            {
                DateTime dtEnd = Convert.ToDateTime(endDate).AddDays(1);
                predicate = predicate.And(item => item.OrderDate < dtEnd);
            }

            return predicate;
            #endregion
        }
    }
}