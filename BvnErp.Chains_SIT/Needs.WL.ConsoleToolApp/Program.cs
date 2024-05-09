using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Models.HttpUtility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.WL.ConsoleToolApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //PayExchange.UpdatePayExchange updatePayExchange = new PayExchange.UpdatePayExchange();
            //updatePayExchange.update();

            //WaybillInfo waybillInfo = new WaybillInfo();
            //waybillInfo.doWaybill();

            //List<String> packs = new List<string>();
            //packs.Add("WL2115-WL2119");
            //packs.Add("GOUT/2648035(CX1)");
            //OuterCalculate outerCalculate = new OuterCalculate(packs);
            //outerCalculate.Calculate();



            //List<string> InvoiceNotices = new List<string>();    
            //InvoiceNotices.Add("IVNT20221214000005");

            //foreach (var item in InvoiceNotices)
            //{


            //    XmlGeneRequestModel requestModel = new XmlGeneRequestModel(item, 0, 1);
            //    InvoiceXmlRequestModel invoiceXmlRequestModel = new InvoiceXmlRequestModel();
            //    invoiceXmlRequestModel.request_service = InvoiceApiSetting.ServiceName;
            //    invoiceXmlRequestModel.request_item = "申请开票_错误处理";
            //    invoiceXmlRequestModel.data = requestModel;
            //    invoiceXmlRequestModel.token = "92A0E2EC-F9F4-41DB-B623-E135A1336254";

            //    string URL = "http://cw.51db.com:9098/";
            //    string requestUrl = URL + InvoiceApiSetting.GenerateXmlUrl;
            //    string apiclient = JsonConvert.SerializeObject(invoiceXmlRequestModel);

            //    HttpResponseMessage response = new HttpResponseMessage();
            //    response = new HttpClientHelp().HttpClient("POST", requestUrl, apiclient);

            //    if (response == null || response.StatusCode != HttpStatusCode.OK)
            //    {
            //        Console.WriteLine(item + "提交失败!");
            //    }
            //    else
            //    {
            //        Console.WriteLine(item + "提交成功!");
            //    }
            //}


            //ClearingData clearing = new ClearingData();
            //clearing.GenerateHistory();


            //GetUnSortingList getUnSortingList = new GetUnSortingList();
            //getUnSortingList.GetResult();
            //getUnSortingList.GenerateHistory();
            //Console.ReadLine();


            //PackingHistory packingHistory = new PackingHistory();
            //DateTime dtStart = Convert.ToDateTime("2021-01-01");
            //DateTime dtEnd = Convert.ToDateTime("2021-06-01");
            //packingHistory.GetOrderIDs(dtStart, dtEnd);
            ////packingHistory.GetOrderIDs();
            //packingHistory.GenerateHistory();


            //try
            //{
            //    //DateTime dtStart = Convert.ToDateTime("2021-07-09");
            //    //DateTime dtEnd = Convert.ToDateTime("2021-07-12");            
            //    DateTime dtStart = Convert.ToDateTime(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
            //    DateTime dtEnd = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));

            //    //DecListRepair decListRepair = new DecListRepair(dtStart, dtEnd);
            //    //decListRepair.RepairNew();


            //    //DateTime dtStart = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
            //    //DateTime dtEnd = Convert.ToDateTime(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            //    DecListRepair decListRepair = new DecListRepair(dtStart, dtEnd);
            //    decListRepair.UpdateInputID();
            //}
            //catch(Exception ex)
            //{

            //}
            //finally
            //{
            //    System.Environment.Exit(0);
            //}
        }
    }
}
