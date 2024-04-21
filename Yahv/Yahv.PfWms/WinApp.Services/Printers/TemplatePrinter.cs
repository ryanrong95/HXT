using Gecko;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services;
using WinApp.Services.Controls;
using WinApp.Services.FilePrints;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace WinApp.Printers
{
    public class TemplatePrinter : PrinterBase
    {
        PrintForm printForm = null;

        internal TemplatePrinter()
        {

            this.printForm = new PrintForm();
        }

        public void Print(object data, PrinterConfig config)
        {
            //未实现
            //throw new Exception("7");
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "yuanda-V23.29.28.47.98.70.63.54K93");//设置UserAgent
                client.Encoding = System.Text.Encoding.UTF8;//解决获取用户信息乱码问题

                string html = client.DownloadString(config.Url);

                html = html.Replace("/Print/js", $"{Config.SchemeName}://{Config.DomainName}/Print/js");

                var printerOnload = Services.Properties.Resource.printerOnload; /*System.IO.File.ReadAllText(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "printerOnload.js"))*/

                int index = html.IndexOf("<head>") + "<head>".Length;
                html = html.Insert(index, $"<script>var model={data.Json()};{printerOnload}</script>");

                this.printForm.WindowState = FormWindowState.Minimized;
                this.printForm.ShowInTaskbar = false;

                if (config.Width.HasValue && config.Height.HasValue)
                {
                    this.printForm.Width = config.Width.Value;
                    if (config.Height != 0)
                    {
                        this.printForm.Height = config.Height.Value;
                    }
                }
                //else
                //{
                //    this.printForm.WindowState = FormWindowState.Maximized;
                //}

                this.printForm.Show();

                this.printForm.PrinterName = config.PrinterName;
                //data 发到浏览器
                this.printForm.Html(html);
            }

        }

        override public void Dispose()
        {
            this.printForm.Dispose();
            this.printForm = null;
        }
    }
}
