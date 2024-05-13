using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebApp.Services;
using WinApp.Services;
using Wms.Services.Models;
using Wms.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace WinApp
{
    /// <summary>
    /// 打印模板化开发
    /// </summary>
    public partial class PrintForm : Form
    {


        Gecko.GeckoWebBrowser browser = null;

        //浏览器地址
        string webPath = $"{WebApp.Services.FromType.Scheme.GetDescription()}://{WebApp.Services.FromType.Web.GetDescription()}";
        //打印对象
        Printings print;
        //当前打印机
        string printer;

        //接受到的前台的信息
        public string message;
        public PrintForm(string data)
        {
            InitializeComponent();


            this.WindowState = FormWindowState.Minimized;

            this.message = data;
            string[] str = data.Split('&');

            string id = str[0].Substring(4);

            //根据打印机id获得对应打印机
            printer = PrintingManager.Printer(id);

            print = new PrintingsView()[id];
            this.Width = print.Width ?? 360;
            this.Height = print.Height ?? 360; ;

            Gecko.Xpcom.Initialize("Firefox64");
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            Browser($"{webPath}/#{print.Url}{message}");

            browser.AddMessageEventListener("PageEvents", (param) => PageEvents(param));
            //this.Hide();
        }

        private void PageEvents(string param)
        {
            var data = param.ToString().JsonTo<NameValuePair>();
            switch (data.Name)
            {
                //打印回传方法
                case "printingpostback":
                    PrintingPostBackEvent(data.Value);
                    break;
            }
        }

        private void PrintingPostBackEvent(string value)
        {
            new Gecko.JQuery.JQueryExecutor(browser.Window).ExecuteJQuery($"var message={message}");
        }

        public void Browser(string url = null)
        {

            if (browser != null)
            {
                browser.Dispose();
            }

            browser = new GeckoWebBrowser();

            browser.PreviewKeyDown += Browser_PreviewKeyDown;
            browser.Dock = DockStyle.Fill;

            if (!string.IsNullOrEmpty(url))
            {
                browser.Navigate(url);
            }
            else
            {
                browser.LoadHtml("");
            }

            this.Controls.Add(browser);

        }

        private void Browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

            //f8打印
            if (e.KeyValue == 119)
            {
                try
                {
                    //this.Print(printer);
                    //browser.Print(printer, 1);
                }
                catch
                {
                    MessageBox.Show("您的系统还没有打印机,请添加打印机 !");
                }
            }
        }

        private void PrintForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            ////browser.Reload();
            //System.Environment.Exit();
        }

        //public void Print(string printer)
        //{
        //    var domWindow = this.browser.Window.DomWindow;
        //    var proxy = (mozIDOMWindowProxy)domWindow;

        //    nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);

        //    try
        //    {
        //        print.Print(new  PrinterManager(this.browser).GetSilent(printer,1), null);
        //    }
        //    catch (COMException e)
        //    {
        //        if (e.ErrorCode != GeckoError.NS_ERROR_ABORT)
        //            throw;
        //    }

        //    Marshal.ReleaseComObject(print);
        //}
    }
}
