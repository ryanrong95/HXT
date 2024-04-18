using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views.Alls;
using Needs.Wl.PlanningService.Services.Order;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;


namespace Needs.Wl.PlanningService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            //List<string> ids = new List<string>();
            //DateTime dtFrom = Convert.ToDateTime("2020-07-13");
            //DateTime dtTo = dtFrom.AddDays(4);
            //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            //{
            //    ids = responsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooPostLog>().
            //        Where(t => t.Summary == "9E467CBBE0C4D78C856C58041E055580" && t.CreateDate > dtFrom && t.CreateDate < dtTo).OrderBy(t => t.CreateDate).
            //        Select(t => t.ID).ToList();

            //}            
            //GetIcgooPI get = new GetIcgooPI(ids);
            //get.getPI();

            //IEnumerable<ApiNotice> list = new ApiNoticesAll().Where(item => item.PushStatus == PushStatus.Pushing && item.ClientID == "9E467CBBE0C4D78C856C58041E055580" && item.PushType == PushType.DutiablePrice).ToArray();

            ////using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            ////{
            ////    responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
            ////                new
            ////                {
            ////                    UpdateDate = DateTime.Now,
            ////                    PushStatus = (int)PushStatus.Pushing
            ////                }, item => item.PushStatus == (int)PushStatus.Unpush && item.ClientID == "a7f0a9f7e352a66e2e95ec02459e094c");
            ////}

            //////推送
            //foreach (var notice in list)
            //{
            //    try
            //    {
            //        IApiCallBack callBack = ApiCallBackFactory.Create(notice.Client);
            //        callBack?.SetNotice(notice);
            //        callBack?.CallBack();
            //    }
            //    catch (Exception ex)
            //    {
            //        using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            //        {
            //            //推送异常，更新ApiNotice状态，
            //            responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
            //            new
            //            {
            //                PushStatus = PushStatus.PushFailure,
            //                UpdateDate = DateTime.Now
            //            }, item => item.ID == notice.ID);
            //        }
            //        continue;
            //    }
            //}

            //List<string> IDS = new List<string>();


            //var clients = ApiService.Current.Clients;
            //var client = ApiService.Current.Clients["dyj"];

            //var apiSettings = ApiService.Current.ApiSettings;
            //var apiSetting = ApiService.Current.ApiSettings["dyj"];

            //var apis = apiSetting.Apis;
            //var api = apis[ApiType.PreProduct];

            //获取未推送的信息
            //IEnumerable<ApiNotice> lists = new ApiNoticesAll().Where(item => item.ID == "9AE65A5AAF354E02BB76C07A24DE20BD").ToArray();
            ////IEnumerable<ApiNotice> lists = new ApiNoticesAll().Where(item => IDS.Contains(item.ID)).ToArray();
            //foreach (var notice in lists)
            //{
            //    IApiCallBack callBack = ApiCallBackFactory.Create(notice.Client);
            //    callBack.SetNotice(notice);
            //    callBack.CallBack();
            //}

            ////获取未归类产品
            //IPreProductRequest preProductRequest = PreProductRequestFactory.Create("icgooxdt");
            //preProductRequest.Process();

            //获取咨询产品
            //IcgooConsultRequest preProductRequest = new IcgooConsultRequest();
            //preProductRequest.Process();

            //获取大赢家订单
            //IOrderRequest OrderRequest = OrderRequestFactory.Create("dyj");
            //OrderRequest.Process();

            //创建大赢家订单            
            //CreateDyjOrder order = new CreateDyjOrder(client.PayExchangeSupplier, client.DeclareCompany, client.Name);
            //order.Create("E8A06F690E1D4666A29454E424B9A2A6");

            //CenterCreateDyjOrder order = new CenterCreateDyjOrder(client.DeclareCompany, client.Name);
            //order.CreateNew("318BAA98CD4842B49F014BB35DE7CCD1");

            //创建Icgoo订单
            //Dictionary<string, string> PayExchangeSupplierMap = new Dictionary<string, string>();
            //PayExchangeSupplierMap.Add("a7f0a9f7e352a66e2e95ec02459e094c", "5ce22f6cd6c946f8007308308619e13f");
            //PayExchangeSupplierMap.Add("5DEAE1100CC624003F93EF4CB757308C", "107C97581A73151D0DBB97A2C671EA98");
            //CenterCreateIcgooOrder order = new CenterCreateIcgooOrder(PayExchangeSupplierMap);
            //order.Create("AC0EC542DE644DD783437874A5632084");

            //CreateIcgooOrder order = new CreateIcgooOrder();
            //order.Create("107CBB36B14A4945A711335401EA0787");

            //CenterCreateIcgooOrder order = new CenterCreateIcgooOrder();
            //order.Create("AC0ADB8EE37549E2A78EE03D19399F3D");


            //创建Icgoo订单
            //IcgooOrderCreate order = new IcgooOrderCreate();
            //order.Process();

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new PlanningService()
            };
            ServiceBase.Run(ServicesToRun);

            //生成 PI
            //ok - Anda International Trade Group Limited
            //ok - HK HUANYU ELECTRONICS TECHNOLOGY CO., LIMITED
            //ok - HK Lianchuang Electronics Co.,Limited
            //HONGKONG HONGTU INTERNATIONAL LOGISTICS CO., LIMITED
            //ok - IC360 Electronics Limited
            //ok - IC360 GROUP LIMITED

            //PIGener pIGener = new PIGener("NL02020190619031");      //83Anda  96LC
            //PIGener pIGener = new PIGener("NL02020190620001");      //83Anda  96LC   105IC360GROUP  112HY
            //PIGener pIGener = new PIGener("XL00120200409001-01");   //83Anda  112HY  103IC360
            //PIGener pIGener = new PIGener("NL02020191114006");      //83Anda  112HY  还有个 HongTu 没有匹配生成
            //pIGener.Execute();

            // XL00120200729501-01
            // XL00120200729501-02

            //PIGener pIGener = new PIGener("");
            //pIGener.Execute();

            //string[] orderIDs = GetOrderIDs();
            //foreach (var orderID in orderIDs)
            //{
            //    PIGener pIGener = new PIGener(orderID);
            //    pIGener.Execute();
            //}

            //for (int i = 1; i <= 1; i++)
            //{
            //    PIGener pIGener1111 = new PIGener("-0" + i);
            //    pIGener1111.Execute();
            //}

            //测试 SwapNoticeReceiptUse
            //for (int i = 0; i < 1000; i++)
            //{
            //    SwapNoticeReceiptUseHandler swapNoticeReceiptUseHandler = new SwapNoticeReceiptUseHandler();
            //    swapNoticeReceiptUseHandler.Execute();

            //    System.Threading.Thread.Sleep(200);
            //}

            //List<string> clientIDS = new List<string>();
            //string CXZXBJID = System.Configuration.ConfigurationManager.AppSettings["CXZXBJID"];
            //string CXZXSZID = System.Configuration.ConfigurationManager.AppSettings["CXZXSZID"];
            //string SZCXZXID = System.Configuration.ConfigurationManager.AppSettings["SZCXZXID"];
            //string CXZXSDID = System.Configuration.ConfigurationManager.AppSettings["CXZXSDID"];
            //string BJXDNID = System.Configuration.ConfigurationManager.AppSettings["BJXDNID"];
            //string FCGYLID = System.Configuration.ConfigurationManager.AppSettings["FCGYLID"];
            //clientIDS.Add(CXZXBJID);
            //clientIDS.Add(CXZXSZID);
            //clientIDS.Add(SZCXZXID);
            //clientIDS.Add(CXZXSDID);
            //clientIDS.Add(BJXDNID);
            //clientIDS.Add(FCGYLID);

            //using (Layer.Data.Sqls.ScCustomsReponsitory responsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            //{
            //    responsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
            //                new
            //                {
            //                    ClientID = "9E467CBBE0C4D78C856C58041E055580"
            //                }, item => clientIDS.Contains(item.ClientID));
            //}

        }


        private static string[] GetOrderIDs()
        {
            string originString = @"";

            List<string> orderIDs = new List<string>();

            using (StringReader sr = new StringReader(originString))
            {
                string line;
                int lineIndex = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    //Console.WriteLine("行{0}:{1}", ++lineIndex, line);
                    line = line.Replace("PIGener报错|TinyOrderID =", "").Replace(" ", "");
                    orderIDs.Add(line);
                }
            }

            return orderIDs.ToArray();
        }


    }
}