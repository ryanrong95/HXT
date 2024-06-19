using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
//using System.Data.Linq;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace CnsleApp
{

    class Otders
    {
        public string InputID { get; set; }
    }

    class MyClass
    {
        public string ID { get; set; }

        public Otders Otders { get; set; }

        public void Enter()
        {
            using (var r1 = LinqFactory<HvRFQReponsitory>.Create())
            {
                var items = r1.ReadTable<Layers.Data.Sqls.HvRFQ.Logs_Errors>();

                Console.WriteLine(items.Count());
            }
        }
    }

    //enum MyEnum : long
    //{

    //}

    //public struct OrderType
    //{
    //    public int Normal { get; private set; }

    //    int[] arry = new int[10];

    //    public OrderType()
    //    {
    //        this.Normal = arry[0];
    //    }
    //}

    class Program
    {




        static void Main(string[] args)
        {


            //Task.


            Uri uri = new Uri("http://filesszwh0.ic360.cn/InDelivery/2021/01/27/c4573d5b1611735106583.jpg");
            var filename = VirtualPathUtility.GetFileName(uri.AbsolutePath);
            var path = uri.AbsolutePath;

            using (var r1 = LinqFactory<HvRFQReponsitory>.Create())
            using (var tsql = r1.TSql)
            {
                using (var r2 = LinqFactory<HvRFQReponsitory>.Create())
                {
                    new MyClass().Enter();
                }

                var items = r1.ReadTable<Layers.Data.Sqls.HvRFQ.Logs_Errors>().Where(item => item.ID == 123);

                tsql.Update<Layers.Data.Sqls.HvRFQ.Logs_Errors>(new
                {
                    Codes = 2
                }, item => item.ID == 123);

                Console.WriteLine(items.Count());
            }


            return;

            //using (SqlConnection conn = new SqlConnection("Data Source=172.30.10.199,5311;Initial Catalog=HvRFQ;Persist Security Info=True;User ID=udata;Password=Turing2019;MultipleActiveResultSets=True"))
            using (var hvRfq = new HvRFQReponsitory())
            using (var pvCrm = new PvbCrmReponsitory())
            using (var tsql = hvRfq.TSql)
            {
                var linq = from inquiry in hvRfq.ReadTable<Layers.Data.Sqls.HvRFQ.Inquiries>()
                           join client in pvCrm.ReadTable<Layers.Data.Sqls.PvbCrm.Enterprises>()
                           on inquiry.ClientID equals client.ID
                           select new
                           {
                               inquiry.ID,
                               client.Name
                           };

                Console.WriteLine(linq.ToArray());

                tsql.Update<Layers.Data.Sqls.HvRFQ.Logs_Errors>(new
                {
                    Codes = 2
                }, item => item.ID == 123);
            }

            //for (int reffer = 0; reffer < 100; reffer++)
            //{
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (var tsql = new HvRFQReponsitory().TSql)
            {
                var list = new List<Layers.Data.Sqls.HvRFQ.Logs_Errors>();
                for (int index = 0; index < 20; index++)
                {
                    list.Add(new Layers.Data.Sqls.HvRFQ.Logs_Errors
                    {
                        AdminID = "Admin00058",
                        Page = "http://erp8.ic360.cn/RFQ/Sale/Boms/Edit.aspx",
                        Message = "引发类型为“System.Web.HttpUnhandledException”的异常",
                        Source = "System.Web.HttpUnhandledException",
                        CreateDate = DateTime.Now,
                        Stack = @"在 System.Web.UI.Page.HandleError(Exception e)     在 System.Web.UI.Page.ProcessRequestMain(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint)     在 System.Web.UI.Page.ProcessRequest(Boolean includeStagesBeforeAsyncPoint, Boolean includeStagesAfterAsyncPoint)     在 System.Web.UI.Page.ProcessRequest()     在 System.Web.UI.Page.ProcessRequest(HttpContext context)     在 ASP.sale_boms_edit_aspx.ProcessRequest(HttpContext context) 位置 C:\Windows\Microsoft.NET\Framework64\v4.0.30319\Temporary ASP.NET Files\rfq\89454e02\12544a7b\App_Web_edit.aspx.b343a2c5.imchgbte.0.cs:行号 0     在 System.Web.HttpApplication.CallHandlerExecutionStep.System.Web.HttpApplication.IExecutionStep.Execute()     在 System.Web.HttpApplication.ExecuteStepImpl(IExecutionStep step)     在 System.Web.HttpApplication.ExecuteStep(IExecutionStep step, Boolean& completedSynchronously)",
                        Codes = "测试"
                    });
                }

                tsql.Insert(list.ToArray());
            }
            watch.Stop();
            Console.WriteLine($"{1}:{watch.ElapsedMilliseconds / 1000m}秒");
            //}




            //Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //new Thread(() =>
            //{
            //    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //}).Start();

            //new Thread(() =>
            //{
            //    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //}).Start();

            //new Thread(() =>
            //{
            //    Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            //}).Start();






            Console.Read();

            //var rb = new RoleBase
            //{ 
            //};

            //rb == 


            //var k = new TopRole
            //{
            //    ID = "Role00041",
            //    Type = RoleType.Compose
            //} == FixedRole.SaleManager;

            //Console.WriteLine(k);

            var url = $"{"http://fix.szhxd.net"}/Yahv/leftbusiness/";
            //string[] lefts = { "erm.json",     "rfq.json",      "crm.json",   "srm.json", "pfwms.json","cbs.json",
            //                   "wltwms.json",  "cywms.json",    "hywms.json", "xdtwms.json",
            //                   "hycms.json",   "xdtcms.json",   "hyfms.json", "xdtfms.json",
            //                   "hycdms.json",  "xdtcdms.json",  "hydms.json", "xdtdms.json",
            //                   "hyerp.json",   "xdterp.json" ,"pvwsorder.json","pvdata.json", 
            //                   "cxhy.json"};

            //string[] lefts = { "erm.json", "rfq.json", "crm.json", "srm.json", "pvwsorder.json", "pfwms.json", "pvdata.json", "cxhy.json", "xdt.json", };

            string[] lefts = new string[] { "erm.json", "rfq.json", "crm.json", "srm.json", "pvwsorder.json", "pfwms.json", "pvdata.json" };


            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                foreach (string name in lefts)
                {
                    string json = webClient.DownloadString(url + name);
                    Console.WriteLine($"下载:{url + name}");
                }
            }

            Console.WriteLine("完成");


            Console.Read();
        }
    }
}
