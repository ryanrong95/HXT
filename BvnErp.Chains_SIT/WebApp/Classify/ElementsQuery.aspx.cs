using Needs.Ccs.Services.Enums;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Classify
{
    /// <summary>
    /// 申报要素查询界面
    /// 用于显示海关编码对应的申报要素
    /// </summary>
    public partial class ElementsQuery : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 根据海关编码获取申报要素
        /// </summary>
        /// <returns></returns>
        protected object GetElements()
        {
            string hsCode = Request.Form["HScode"];
            string origin = Request.Form["Origin"];
            var tariff = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.CustomsTariffs.Where(item => item.HSCode == hsCode.Trim()).FirstOrDefault();

            if (tariff != null)
            {
                //查询申报要素默认值
                var elementsDefaults = tariff.ElementsDefaults.Select(item => new { item.ElementName, item.DefaultValue });
                //查询原产地税率
                var curDate = DateTime.Parse(DateTime.Now.ToShortDateString());
                var importTax = tariff.OriginTariffs.FirstOrDefault(item => item.Type == CustomsRateType.ImportTax && item.Origin == origin &&
                                                                            item.StartDate <= curDate && (item.EndDate == null || item.EndDate >= curDate));
                // var addedValueTax = tariff.OriginTariffs.Where(item => item.Type == CustomsRateType.AddedValueTax && item.Origin == origin).FirstOrDefault();

                //监管条件 海关监管条件中含有A的 或者 商检监管条件中，带有L或者带有M的商品编码
                var flag = false;
                if (tariff.InspectionCode != null)
                {
                    string[] inspectionCodes = { "L", "M" };
                    //是否商检标志

                    foreach (var ins in inspectionCodes)
                    {
                        if (tariff.InspectionCode.IndexOf(ins) != -1)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                if (tariff.RegulatoryCode != null && tariff.RegulatoryCode.IndexOf("A") != -1)
                {
                    flag = true;
                }
                return new
                {
                    DeclareElements = tariff.Elements,
                    TariffName = tariff.Name,
                    TariffID = tariff.ID,
                    TariffRate = importTax == null ? tariff.MFN / 100 : (importTax.Rate + tariff.MFN) / 100,
                    ValueAddTaxRate = tariff.AddedValue / 100,
                    Unit1 = tariff.Unit1,
                    Unit2 = tariff.Unit2,
                    CIQCode = tariff.CIQCode,
                    //Y原产地证明
                    RegulatoryCode = tariff.RegulatoryCode,
                    //监管代码：商检，
                    InspectionCodeFlag = flag,
                    //申报要素默认值 
                    ElementsDefaults = elementsDefaults
                };
            }
            return null;
        }
    }
}