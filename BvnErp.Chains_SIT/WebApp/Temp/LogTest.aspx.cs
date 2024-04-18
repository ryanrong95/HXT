using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Serializers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Needs.Ccs.Services.Models;
using Needs.Underly;
using Needs.Ccs.Services.ApiSettings;
using System.Configuration;

namespace WebApp.Temp
{
    /// <summary>
    /// 用于临时测试，后期删除
    /// </summary>
    public partial class LogTest : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //var quotedOrder = new Needs.Ccs.Services.Views.QuoteConfirmedOrdersView().ToArray();

            var apisetting = new PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.AutoClassify;
            var data = new
            {
                PartNumber = "123",
                Manufacturer = "45678910",
                UnitPrice = 1.1,
                isVerifyPriceFluctuation = true,
                highPriceLimit = 0.8,
                lowPriceLimit = 0.3
            };
            var result = Needs.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(url, data);
            //code: 100 - 接口调用成功，但是没有自动归类信息、200 - 接口调用成功，有自动归类数据、300 - 接口调用异常
            if (result.code == 200)
            {
                
            }
            else
            {
                var log = new Needs.Ccs.Services.Models.ClassifyApiLogs()
                {
                    ClassifyProductID = "",
                    Url = url,
                    RequestContent = data.Json(),
                    ResponseContent = result.Json(),
                    Summary = "产品归类 - 自动归类"
                };
                log.Enter();
            }


            //this.Model.Logs = new Needs.Ccs.Services.Views.OrderLogsView().Where(log => log.OrderID == "WL00120190129001").Json();
            //DyjClassifyTest();

            //string[] itemIDs = { "XDTOrderItem202003040000000009", "XDTOrderItem202002240000000004" };
            //var cps = new Needs.Ccs.Services.Views.Alls.ClassifyProductsAll().GetTop(itemIDs.Length, item => itemIDs.Contains(item.ID), null, null, null);
            //foreach (var cp in cps)
            //{
            //    cp.ImportTax.ImportPreferentialTaxRate = cp.ImportTax.Rate;
            //    cp.ImportTax.OriginRate = 0m;
            //}
        }

        private void DyjClassifyTest()
        {
            string jsonText = Needs.Utils.Http.ApiHelper.Current.JPost("http://172.16.6.61:8100/users/type_details/", new
            {
                part_name = "FXTH87EH116T1",
                code = "ec371813ff99a4a1028193df5a0d1263"

            });
            if (jsonText == "")
            {
                return;
            }

            JObject jObject = (JObject)JToken.Parse(jsonText);
            if (jObject["status"].ToString() == "failure")
            {
                return;
            }

            //海关编码
            IEnumerable<HSCode> hsCode = JsonConvert.DeserializeObject<IEnumerable<HSCode>>(jObject["HS编码"].ToString());
            if (hsCode == null || hsCode.Count() == 0)
            {
                return;
            }

            //申报要素
            IEnumerable<HSElements> hsElements = JsonConvert.DeserializeObject<IEnumerable<HSElements>>(jObject["申报要素"].ToString());
            if (hsElements == null || hsElements.Count() == 0)
            {
                return;
            }

            DyjJsonObject jsonObject = new DyjJsonObject();
            jsonObject.HSCodes = hsCode;
            jsonObject.HSElements = hsElements;

            //商检
            jsonObject.Inspections = JsonConvert.DeserializeObject<IEnumerable<Inspection>>(jObject["商检"].ToString());
            //禁运
            jsonObject.Embargos = JsonConvert.DeserializeObject<IEnumerable<Embargo>>(jObject["禁运"].ToString());
            //3C
            jsonObject.CCCs = JsonConvert.DeserializeObject<IEnumerable<CCC>>(jObject["3C"].ToString());
            //关税率
            jsonObject.TariffRates = JsonConvert.DeserializeObject<IEnumerable<TariffRate>>(jObject["关税率"].ToString());
            //税务编码
            jsonObject.TaxCodes = JsonConvert.DeserializeObject<IEnumerable<TaxCode>>(jObject["税收编码"].ToString());
        }
    }
}