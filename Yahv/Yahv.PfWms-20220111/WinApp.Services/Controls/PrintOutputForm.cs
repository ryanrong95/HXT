using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yahv.Utils.Serializers;

namespace WinApp.Services.Controls
{
    /// <summary>
    /// 出库通知打印
    /// </summary>
    public partial class PrintOutputForm : Form
    {
        public PrintOutputForm()
        {
            InitializeComponent();
            this.Disposed += PrintOutputForm_Disposed;
        }

        private void PrintOutputForm_Disposed(object sender, EventArgs e)
        {
            if (firefox != null)
            {
                firefox.Dispose();
            }
        }

        GeckoWebBrowser firefox;

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }

        private void Print_Load(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;


            this.KeyPreview = true;
            firefox = new GeckoWebBrowser();
            //Xpcom.Initialize("Firefox");

            firefox.Dock = DockStyle.Fill;
            this.Controls.Add(firefox);
            firefox.NoDefaultContextMenu = true; //禁用右键菜单

            //this.Height = 863;
            ////this.Height = 700;
            //this.Width = 379;


            firefox.AddMessageEventListener("Print", (param) => Print(param));
        }

        public void Print(object data)
        {
            //未实现
           //throw new Exception("8");
            this.Show();

            string html;
            using (var client = new WebClient())
            {
                string url = $"{Config.SchemeName}://{Config.DomainName}/Print/html/outgoing.html";

                client.Headers.Add("user-agent", "yuanda-V23.29.28.47.98.70.63.54K93");
                client.Encoding = System.Text.Encoding.UTF8;//解决获取用户信息乱码问题

                html = client.DownloadString(url);
                html = html.Replace("/Print/js", $"{Config.SchemeName}://{Config.DomainName}/Print/js");
            }

            var printerOnload = Properties.Resource.printerOnload;

            int index = html.IndexOf("<head>") + "<head>".Length;
            html = html.Insert(index, $"<script>var model = {data?.Json()};{printerOnload}</script>");

            this.firefox.LoadHtml(html);
        }


        private void Print(string param)
        {
            //throw new Exception("8");
            this.Print();
            this.Close();
            this.Dispose();
        }

        private void Print()
        {
            //throw new Exception("8");
            var domWindow = this.firefox.Window.DomWindow;
            var proxy = (mozIDOMWindowProxy)domWindow;

            nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);


            try
            {
                var settings = new GeckoPrinterManager(this.firefox).GetSilent(PrinterName);
                settings.SetPaperHeightAttribute(0.8);//厂商给的默认值:0.8
                print.Print(settings, null);
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode != GeckoError.NS_ERROR_ABORT)
                    throw ex;
            }
            finally
            {
                Marshal.ReleaseComObject(print);
            }
        }
    }
}
