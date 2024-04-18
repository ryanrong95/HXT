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

namespace Yahv.PsWms.DappForm.Services.Controls
{
    public partial class SFPrint : Form
    {
        GeckoWebBrowser firefox;

        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrinterName { get; set; }
        public SFPrint()
        {
            InitializeComponent();
            this.Disposed += SFPrint_Disposed;
        }

        private void SFPrint_Disposed(object sender, EventArgs e)
        {
            if (firefox != null)
            {
                firefox.Dispose();
            }
        }

        private void SFPrint_Load(object sender, EventArgs e)
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

        public void Print(object data, string html)
        {
            this.Show();

            var printerOnload = Properties.Resource.printerOnload;
            int index = html.IndexOf("<head>") + "<head>".Length;
            html = html.Insert(index, $"<script>{printerOnload}</script>");
            this.firefox.LoadHtml(html);
        }

        private void Print(string param)
        {
            //MessageBox.Show("开始打印！");
            this.Print();
            //this.Close();
            //this.Dispose();
        }

        private void Print()
        {
            var domWindow = this.firefox.Window.DomWindow;
            var proxy = (mozIDOMWindowProxy)domWindow;

            nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);

            try
            {
                var settings = new GeckoPrinterManager(this.firefox).GetSilent(PrinterName);

                //var with = settings.GetPaperWidthAttribute() * 0.9;
                //settings.SetPaperWidthAttribute(with);

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
