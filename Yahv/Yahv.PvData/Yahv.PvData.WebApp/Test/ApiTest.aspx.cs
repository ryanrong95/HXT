using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;
using YaHv.PvData.Services.Utils;

namespace Yahv.PvData.WebApp.Test
{
    public class Controls
    {
        public string PartNumber;
        public bool IsSysCcc;
        public bool IsSysEmbargo;
    }

    public partial class ApiTest : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            TestWebApi();
            //TestFixSpecialChars();
            //TestDecimalPrecision();
            //TestMD5ID();
            //TestGetMultiSysControls();
        }

        void TestWebApi()
        {
            System.Diagnostics.Stopwatch watcher1 = new System.Diagnostics.Stopwatch();
            watcher1.Start();

            string url = "http://hv.erp.b1b.com/PvDataApi/Product/GetIdByInfo";
            var result = Yahv.Utils.Http.ApiHelper.Current.Get<JMessage>(url, new
            {
                partNumber = "LTC2644IMS-L8#PBF",
                manufacturer = "LTC"
            });
            string id = result.data;

            watcher1.Stop();
            TimeSpan span1 = watcher1.Elapsed;

            System.Diagnostics.Stopwatch watcher2 = new System.Diagnostics.Stopwatch();
            watcher2.Start();

            url = "http://erp80.ic360.cn/PvDataApi/Classify/GetAutoClassified";
            var result2 = Yahv.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(url, new
            {
                partNumber = "RC0603FR-07620RL",
                manufacturer = "Yageo",
                unitPrice = 0.1,
                origin = "USA"  //可选参数，下单时如果没有选产地可以不填
            });
            dynamic data = result2.data;

            watcher2.Stop();
            TimeSpan span2 = watcher2.Elapsed;

            url = "http://hv.erp.b1b.com/PvDataApi/ClassifyInfo";
            var result3 = Yahv.Utils.Http.ApiHelper.Current.Get<JSingle<dynamic>>(url, new
            {
                cpnId = "48DEC037A51CE69537FD4EE26943A0E9"
            });
            data = result3.data;
            string elementsJson = data.Elements;
            Dictionary<string, string> elements = elementsJson.JsonTo<Dictionary<string, string>>();
            this.Model.Elements = elements.Select(e => new { e.Key, e.Value });
        }

        void TestFixSpecialChars()
        {
            string c = "FP301-1-48&quot; - White".FixSpecialChars();
            string c1 = "FP301-1-48&quot; - White".Replace("&quot;", "\"");
            string c2 = "FP301-1-48&#39; - White".FixSpecialChars();
            string c3 = "FP301-1-48&#39; - White".Replace("&#39;", "\"");
            string c4 = "FP301-1-48&amp; - White".FixSpecialChars();
            string c5 = "FP301-1-48&amp; - White".Replace("&amp;", "\"");

            string id = string.Concat("CRCW0603120RFKEAC", "VISHAY").MD5();

            string guid = GuidUtil.NewGuidUp();

            DateTime day = DateTime.MinValue;
            int counter = 0;

            for (int i = 0; i < 10; i++)
            {
                if (day.Date == DateTime.Today.Date)
                {
                    Interlocked.Increment(ref counter);
                    string dyjid = "DYJ" + day.ToString("yyyyMMdd") + counter.ToString().PadLeft(6, '0');
                }
                else
                {
                    counter = 1;
                    day = DateTime.Today.Date;
                }
            }
        }

        void TestDecimalPrecision()
        {
            decimal rate = decimal.Parse("0.1300");

            var rateStr = decimal.Parse(rate.ToString("0.#######")).ToString();

            decimal rate2 = decimal.Parse("0.0800");
            var rateStr2 = decimal.Parse(rate2.ToString("0.#######")).ToString();

            decimal rate3 = decimal.Parse("0.0000000");
            var rateStr3 = (decimal.Parse(rate3.ToString("0.#######"))).ToString();

            decimal rate4 = decimal.Parse("13.0000000");
            var rateStr4 = ((decimal.Parse(rate4.ToString("0.#######"))) / 100).ToString();

            decimal rate5 = decimal.Parse("7.0000000");
            var rateStr5 = ((decimal.Parse(rate5.ToString("0.#######"))) / 100).ToString();
        }

        void TestMD5ID()
        {
            var sysEmbargo = new YaHv.PvData.Services.Views.Alls.ProductControlsAll()["SKY16601-555LF", YaHv.PvData.Services.ControlType.Embargo];

            var md5ID = string.Concat("0480995701", "MOLEX").MD5();
            var md5ID1 = string.Concat("0003091042", "MOLEX", "8547200000", "绝缘塑料外壳", "035", null, "0.1300000", "0.0700000", "0.0000000",
                "4|3|开关电源用|尼龙制|MOLEX牌|型号:0003091042|||插座外壳", null, null, "999", "1090411020000000000", "*绝缘制品*绝缘塑料外壳").MD5();

            var cpn = new YaHv.PvData.Services.Views.Alls.ClassifiedPartNumbersAll()["E7C701E6AFF89A3E70444D117845A40B"];
            var tariff = new YaHv.PvData.Services.Views.Alls.TariffsAll()["8547200000"];

            //var md5Str = string.Concat(cpn.PartNumber, cpn.Manufacturer, cpn.HSCode, cpn.TariffName,
            //                            tariff.LegalUnit1, tariff.LegalUnit2,
            //                            tariff.VATRate / 100,
            //                            tariff.ImportPreferentialTaxRate / 100,
            //                            tariff.ExciseTaxRate / 100,
            //                            cpn.Elements, cpn.SupervisionRequirements, cpn.CIQC, cpn.CIQCode,
            //                            cpn.TaxCode, cpn.TaxName);
            //var md5ID2 = md5Str.MD5();

            decimal vatRate = 0.13m;
            decimal importRate = 0.07m;
            decimal exciseRate = 0;

            var md5Str = string.Concat("0003091042", "MOLEX", "8547200000", "绝缘塑料外壳", "035", vatRate, importRate, exciseRate,
                "4|3|开关电源用|尼龙制|MOLEX牌|型号:0003091042|||插座外壳", "999", "1090411020000000000", "*绝缘制品*绝缘塑料外壳");

            var md5ID3 = string.Concat("0003091042", "molex", "8547200000", "绝缘塑料外壳", "035", vatRate, importRate, exciseRate,
                "4|3|开关电源用|尼龙制|MOLEX牌|型号:0003091042|||插座外壳", "999", "1090411020000000000", "*绝缘制品*绝缘塑料外壳").MD5();

            var md5ID4 = string.Concat("GCM1885C1H220JA16D", "murata", "8532241000", "片式多层瓷介电容器", "035", "054", 0.13, 0, 0,
                "4|3|多层,片式|瓷介质|Murata牌|型号:GCM1885C1H220JA16D|||50V,22pF/电视机线路板用", "999", "1090519010000000000", "*电子元件*片式多层瓷介电容器").MD5();

            decimal? nullDecimal = null;
            var test = (nullDecimal.GetValueOrDefault() / 100).ToString("0.0000000");
        }

        void TestGetMultiSysControls()
        {
            List<string> partNumbers = new List<string>()
            {
                "9-1474653-1", "HRHF-125FR-10/97-CS8413", "IPQ-4019-0-583MSP-MT-00-0", "A2470-CFG1"
            };

            var url = "http://hv.erp.b1b.com/PvDataApi/Classify/GetMultiSysControls";

            //调用中心数据接口获取自动归类信息
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JSingle<string>>(url, new
            {
                PartNumbers = partNumbers
            });

            if (result.code == 200)
            {
                var data = result.data.JsonTo<List<Controls>>();
            }
            if (result.code == 300)
            {
                var msg = result.data;
            }

        }
    }
}