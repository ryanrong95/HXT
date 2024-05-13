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

namespace WinApp.Services.Controls
{

    /// <summary>
    /// 打印快递鸟顺丰/跨越速运
    /// </summary>
    public partial class PrintKdnForm : Form
    {
        public PrintKdnForm()
        {
            InitializeComponent();
            this.Disposed += PrintKdnForm_Disposed;
        }

        private void PrintKdnForm_Disposed(object sender, EventArgs e)
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


        /// <summary>
        /// 设置html
        /// </summary>
        /// <param name="content">打印内容</param>
        /// <param name="correct">修正内容</param>
        public void Html(string content, string correct)
        {
            string html = content;
            
            //html = html.Replace("月结卡号:075517225569", "月结卡号:075*******69");
            html = html.Replace("月结卡号:075568610585", "月结卡号:075*******85");//原创新恒远跨越月结卡号：075517225569停用，更新为深圳市芯达通供应链管理有限公司跨越月结账号：075568610585，2021.4.1日启用

            //int SW1 = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;

            int index = html.IndexOf("<head>") + "<head>".Length;
            html = html.Insert(index, $"<script>{Properties.Resource.printerOnload}</script>");

            correct = correct.Replace("/Print/js", $"{Config.SchemeName}://{Config.DomainName}/Print/js");

            //if (SW1 == 1920)
            //{
            //    html = html.Insert(index, $"<style>{Properties.Resource.kysy_1920}</style>");
            //}
            //else
            //{
            //    html = html.Insert(index, $"<style>{Properties.Resource.kysy_1440}</style>");
            //}

            index = html.IndexOf("</head>");
            html = html.Insert(index, correct);

            this.firefox.LoadHtml(html);
        }


        private void Print(string param)
        {
            this.Print();
            this.Close();
            this.Dispose();
        }

        private void Print()
        {

            //未实现
            //throw new Exception("8");

            var domWindow = this.firefox.Window.DomWindow;
            var proxy = (mozIDOMWindowProxy)domWindow;

            nsIWebBrowserPrint print = Xpcom.QueryInterface<nsIWebBrowserPrint>(proxy);

            try
            {
                var settings = new GeckoPrinterManager(this.firefox).GetSilent(PrinterName);

                settings.SetPaperHeightAttribute(120);
                settings.SetPaperWidthAttribute(100);

                //settings.SetMarginRightAttribute(0); //设置左/右边距属性（不能设置打不出来东西），默认0就行
                //settings.SetEdgeLeftAttribute(20); //设置边缘左属性
                //settings.SetResolutionAttribute(1920*1080);//设置分辨率属性
                //settings.SetScalingAttribute(1000); //设置缩放属性
                //settings.SetShrinkToFitAttribute(true);//将“收缩”设置为“适合”属性
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
