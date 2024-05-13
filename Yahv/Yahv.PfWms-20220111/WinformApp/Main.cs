using Gecko;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Printers;
using WinApp.Services;
using Wms.Services.Models_chenhan;
using Wms.Services.Views;
using Yahv;
using Yahv.Models;
using Yahv.Underly;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;

namespace WinApp
{
    public partial class Main : Form
    {
        Gecko.GeckoWebBrowser MainBrowser = null;
        Gecko.GeckoWebBrowser SubBrowser = null;

        public String userToken { get; set; }
        public Main()
        {
            InitializeComponent();
            Xpcom.Initialize("Firefox");
        }


        private void Main_Load(object sender, EventArgs e)
        {
            GeckoJsToCsHelper.Initialize();

            this.Text = "库房登陆";
            MainBrowser = new GeckoWebBrowser { Dock = DockStyle.Fill };
            SubBrowser = new GeckoWebBrowser();


            this.Controls.Add(MainBrowser);

            MainBrowser.Navigate(string.Concat(WebApp.Services.FromType.Scheme.GetDescription(), "://", WebApp.Services.FromType.Web.GetDescription(), "/index.html"));

            MainBrowser.AddMessageEventListener("PageEvents", (param) => PageEvents(param));



            GeckoJsToCsHelper.Initialize(this.MainBrowser);



            //#if (DEBUG)
            //            StringBuilder funs = new StringBuilder();
            //            funs.AppendLine("export function FireEvent(name, data){var event = new MessageEvent(name,{'view': window, 'bubbles': false, 'cancelable': false, 'data': data});document.dispatchEvent(event);}");
            //            funs.AppendLine();
            //            funs.AppendLine("export function PageEvent(data) { FireEvent(\"PageEvents\", data); }");
            //            funs.AppendLine();
            //#endif
            //            foreach (var method in typeof(GeckoHelper).GetMethods().Where(item => item.GetCustomAttribute<GeckoFuntionAttribute>() != null))
            //            {

            //#if (DEBUG)

            //                funs.AppendLine("export function " + method.Name + "(data) { FireEvent(\"" + method.Name + "\", data); }");
            //                funs.AppendLine();
            //#endif
            //                MainBrowser.AddMessageEventListener(method.Name, (param) =>
            //                {
            //                    var value = new object[] { param };
            //                    method.Invoke(null, value);
            //                });
            //            };


            //#if (DEBUG)
            //            funs.AppendLine("PageEvent(\"{\\\"name\\\":\\\"pageinit\\\"}\"); ");
            //            funs.AppendLine();
            //            var file = @"D:\Projects_vs2015\Yahv\Yahv.PfWms\WebApp\Source\warehous\src\js\browser.js";
            //            File.SetAttributes(file, FileAttributes.Normal);
            //            System.IO.File.WriteAllText(file, funs.ToString(), Encoding.UTF8);

            //#endif



            //鼠标恢复默认
            this.Cursor = Cursors.Default;

        }



        private void PageEvents(string param)
        {
            var data = param.ToString().JsonTo<NameValuePair>();
            switch (data.Name)
            {
                case "logon":
                    LogonEvent(data.Value);
                    break;
                case "pageinit":
                    PageInitEvent();
                    break;
                case "printinginit":
                    PrintSetInitEvent();
                    break;
                case "setprintingset":
                    SetPrintEvent(data.Value);
                    break;
                //调用打印页面(data.Value=数组)
                case "printing":
                    PrintingEvent(data.Value);
                    break;

                //调用文件上传页面
                case "fileupload":
                    FileUploadEvent(data.Value);
                    break;
                //调用拍照上传页面
                case "photoupload":
                    PhotoUploadEvent(data.Value);
                    break;
                //文件上传回调方法
                case "fileuploadpostback":
                    FileUploadPostbackEvent(data.Value);
                    break;
                //拍照上传回调方法
                case "photouploadpostback":
                    PhotoUploadPostbackEvent(data.Value);
                    break;
                default:
                    break;
            }
        }

       
        private void PhotoUploadPostbackEvent(string value)
        {
            throw new NotImplementedException();
        }

        private void FileUploadPostbackEvent(string value)
        {
            throw new NotImplementedException();
        }

        private void PhotoUploadEvent(string value)
        {
            new PhotoForm(value).Show(this);
            //new Gecko.JQuery.JQueryExecutor(browser.Window).ExecuteJQuery($"var base64str={Base64Str} ");
        }

        private void FileUploadEvent(string value)
        {
            new FileForm(value).Show(this);
        }

        private void SetPrintEvent(string data)
        {
            var obj = data.Base64Decode().JsonTo<Setting>();
            PrintingManager.Enter(obj);
        }
        private void LogonEvent(string data)
        {
            Yahv.Ring.Cookie(data);
        }

        private void PageInitEvent()
        {

            new Gecko.JQuery.JQueryExecutor(MainBrowser.Window).ExecuteJQuery($"var IsWinform=true; ");
        }

        private void PrintSetInitEvent()
        {
            try
            {
                //new Gecko.JQuery.JQueryExecutor(MainBrowser.Window).ExecuteJQuery($"var IsWinform=true; var obj={PrintingManager.List()};");

                new Gecko.JQuery.JQueryExecutor(MainBrowser.Window).ExecuteJQuery($"var IsWinform=true; var obj={PrintingManager.List()};");
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public static object printingObj = new object();


        class MyClass
        {
            public Setting Setting { get; set; }

            public object[] data { get; set; }
        }

        private void PrintingEvent(string data)
        {
            //var setting = data.JsonTo<Setting>();

            #region 没有用了
            //try
            //{
            //    lock (printingObj)
            //    {
            //        Obj[] paras = data.Base64Decode().JsonTo<Obj[]>();
            //        //当前打印机
            //        string printer;

            //        for (int i = 0; i < paras.Count(); i++)
            //        {
            //            var printerOnload = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config", "printset.config"));
            //            var arry = printerOnload.JsonTo<Setting[]>();

            //            var setting = arry.Where(item => item.ID == paras[i].ID).FirstOrDefault();

            //            if (Path.GetExtension(setting.Url) == ".html")
            //            {
            //                //setting.Url= string.Concat(WebApp.Services.FromType.Scheme.GetDescription(), "://", WebApp.Services.FromType.Web.GetDescription(),setting.Url);
            //                setting.Url = "http://hv.warehouse.b1b.com" + setting.Url;
            //                //根据打印对象ID获得对应打印机
            //                printer = PrintingManager.Printer(paras[i].ID);

            //                //模板打印
            //                PrintHelper.Current.Template.Print(new { obj = paras[i].data, size = paras[i].size }, setting);
            //            }
            //            else
            //            {
            //                PrintHelper.Current.File.Print(setting);
            //            }

            //            //Setting setting = new Setting
            //            //{
            //            //    Height = printting.Height,
            //            //    Url = "http://hv.warehouse.b1b.com" + printting.Url,
            //            //    Width = printting.Width,
            //            //    Name = printting.Name,
            //            //    Printer = printer
            //            //};

            //            //Ring.Current.WhService.LabelHelper.Product.Print(new { obj = paras[i].data, size = paras[i].size }, setting);
            //        }
            //    }

            //    //只打印产品标签的写法
            //    //foreach (var item in data.Base64Decode().JsonTo<ProductParas[]>())
            //    //{
            //    //    Ring.Current.WhService.LabelHelper.Product.Print(item);
            //    //}

            //}
            //catch (Exception ex)
            //{

            //}

            #endregion

        }


        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Environment.Exit(0);
        }

        #region 旧的打印代码，无用
        //private void Print(Obj obj)
        //{

        //    try
        //    {
        //        //当前打印机
        //        string printer;

        //        //根据打印机id获得对应打印机
        //        printer = PrintingManager.Printer(obj.ID);

        //        var pv = new PrintingsView()[obj.ID];

        //        if (pv != null)
        //        {

        //            var json = obj.Json();
        //            SubBrowser.Width = pv.Width;
        //            SubBrowser.Height = pv.Height;

        //            SubBrowser.Navigate(string.Concat(WebApp.Services.FromType.Scheme.GetDescription(), "://", WebApp.Services.FromType.Web.GetDescription(), "/#", pv.Url, "?", json.Base64Encode()), GeckoLoadFlags.BypassCache);

        //            //SubBrowser.Dock = DockStyle.Fill;
        //            //this.Controls.Clear();
        //            //this.Controls.Add(SubBrowser);

        //            var now = DateTime.Now;
        //            while (true)
        //            {
        //                Application.DoEvents();
        //                if (now.AddSeconds(2) < DateTime.Now)
        //                {
        //                    break;
        //                }
        //            }


        //            SubBrowser.Print(printer, 1, true);
        //        }

        //        var newtime = DateTime.Now;
        //        while (true)
        //        {

        //            Application.DoEvents();
        //            if (newtime.AddSeconds(1) < DateTime.Now)
        //            {
        //                break;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {


        //    }

        //}
        #endregion

    }

}
