using ConsoleApp.vTaskers.Services;
using Kdn.Library.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using WinApp.Services;
using Wms.Services.chonggous;
using Wms.Services.chonggous.Views;
using Yahv.Services;
using Yahv.Underly;
using Yahv.Utils.Http;
using Yahv.Utils.Kdn;
using Yahv.Utils.Serializers;

namespace CaTester
{
    class Program
    {
        static void Main(string[] args)
        {

            var json = new WayRequirementsView().CheckTests.Json();

            var arry = json.JsonTo<CheckRequirement>();

            //Console.WriteLine(arry.Length);

            //"".OrderByDescending


            string[] boxcodes = @"
26
28
24
27
4
23
5
18
WL10
WL11
1265554
1265163
1264712
1265407
1265399
1265172
1265090
1264912
1264794
1264648
1264237
1265308
1265199
1265188
1265109
1264263
1264864
1264531
1264647
19
20
25
1265330
1265272
1265225
1264629
1265409
1265275
1265236
1265388
1265267
1265392
1264344
1265078
1265277
1265257
1265391
1265274
1265393
1265250
1264340
1265074
1265289
1265084
1265110
29
31
33
35
36
30
WL05-WL06
WL04
WL07
WL01-WL03
11
7
12
21
14
9
13
10
16
17
8
2
22
15
1264716
1265400
1265238
3
1
6
1265395
32
34
1265379
SJ35
1264348
1265152
WL12
WL14
WL13
1265593
WL26-WL29
WL24
WL22
WL17
WL16
WL21
WL19
WL25
WL15
WL20
WL18
WL23
WL13
WL15
WL14
1255160
T001
WL09
WL08
WL8006
WL8007
1256278
".Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);


            /*
             WL01-WL100
             WL132-WL133
             */

            var jjj = CgSzSortingsView.GetTotalPart(boxcodes.Select(item => item.ToUpper().Trim()));

            // Console.WriteLine(jjj.Count());

            //var ss = new Wms.Services.chonggous.Views.CgDelcareShipView();
            //ss.AutoVouchers("1202006111443");

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(Wms.Services.FromType.XdtSZShiped.GetDescription(), new Wms.Services.Models.SZToXDT
            {
                AdminID = "Admin00548",
                WaybillID = "Waybill202006170014"
            });



            var jjj1 = WhSettings.SZ["深圳市芯达通供应链管理有限公司"].ID;


            Console.WriteLine(jjj1);
            //

            Console.WriteLine();

            #region ApiHelper

            //LitTools.Current["生成香港出库通知"].Log("开始调用：AutoHkExit");
            //string url = "http://fixed2.b1b.com/Yahv/customs-easyui/controls/Redirect.html";

            //url = "http://127.0.0.1:8234/My1/Show/?k=1";

            //ApiHelper.Asynchron.Get<string>(url, new
            //{
            //    a = 1,
            //    b = 2
            //}, (json, ex) =>
            //{
            //    Console.WriteLine(json);
            //});

            //Console.WriteLine();

            //ApiHelper.Asynchron.Get(url);
            //ApiHelper.Asynchron.Get(url, (json, ex) =>
            //{

            //});

            //ApiHelper.Asynchron.Get(url, null, (json, ex) =>
            //{

            //});

            //ApiHelper.Asynchron.Post(url, null, (json, ex) =>
            //{

            //});

            //ApiHelper.Asynchron.JPost(url, new
            //{
            //    a = 1,
            //    b = 2
            //}, (json, ex) =>
            //{
            //    Console.WriteLine(json);
            //});

            //Console.Read();

            #endregion


            //string json = System.IO.File.ReadAllText(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\CaTester\111.json");
            //json.JsonTo<KdnPrint>();

            Console.WriteLine(pccAreas.Current.Count());

            foreach (var item in pccAreas.Current)
            {
                Console.WriteLine(item.n);
            }


            Console.Read();


            //foreach (var item in pccAreas.Current["中国"].s.SelectMany(item => item.s.Select(c => c.n.Last())).Distinct())
            //{
            //    Console.WriteLine(item);
            //}

            //KdnAddress senderAddress;
            //KdnAddress receiverAddress;

            //if ("上海市青浦区明珠路2号".TryAddress(out senderAddress))
            //{
            //    var sender = new Sender(senderAddress)
            //    {

            //    }; 
            //}




            //WinApp.Services.Kdn.Class1._1();

            //Console.WriteLine(Guid.NewGuid().ToString("N").Length); ;

            //Console.WriteLine(HttpUtils.RequestQuery.Count);
            //Console.WriteLine(HttpUtils.RequestForm.Count);

            //string txt = System.IO.File.ReadAllText(@"d:\12.txt", Encoding.UTF8);

            //var j = txt.JsonTo<PrintParameter>();

            //foreach (var item in GeckoHelper.GetAllPrinterNames())
            //{
            //    Console.WriteLine(item);
            //}

            //Console.WriteLine(GeckoHelper.NeedPrinterConfig()); ;

            //var configs = new PrinterConfigs();

            //configs["产品标签"].PrinterName = "123";
            //configs.Save();

            //configs = new PrinterConfigs();
            //foreach (var item in configs)
            //{
            //    Console.WriteLine(item.Json());
            //}

            //ApiHelper.Current.Get("http://www.baidu.com", new { b = 1 });






            //print.PrinterSettings.

        }
    }
}
