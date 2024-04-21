using Gecko;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinApp.Services;
using Yahv.Underly;

namespace WinApp
{
    public partial class HomePage : Form
    {
        public HomePage()
        {
            this.Load += HomePage_InitializeComponent;
            InitializeComponent();
        }


        GeckoWebBrowser firefox;

        private void HomePage_Load(object sender, EventArgs e)
        {
            GeckoJsToCsHelper.Initialize();

            this.Text = "库房登陆";
            var firefox = this.firefox = new GeckoWebBrowser
            {
                Dock = DockStyle.Fill,
                NoDefaultContextMenu = false,
            };
            this.tlpMain.Controls.Add(firefox, 0, 0);

            var statusBar = new Controls.UcStatusBar();
            this.tlpMain.Controls.Add(statusBar, 0, 1);
            SimHelper.Initialize(statusBar);

            string homeUrl = $"{Config.SchemeName}://{Config.DomainName}/index.html";

            //启用摄像头功能
            GeckoPreferences.User["media.navigator.permission.disabled"] = true;
            GeckoPreferences.User["permissions.default.camera"] = 100;

            //设置浏览器的userAgent（用户代理）
            GeckoPreferences.User["general.useragent.override"] = "yuanda-V23.29.28.47.98.70.63.54K93";

            //http://hv.warehouse.b1b.com/#/login

            ////线程异常
            //throw new Exception("2");

            this.firefox.Navigate(homeUrl, GeckoLoadFlags.FirstLoad);

            //this.firefox.Navigate(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\test1.html", GeckoLoadFlags.FirstLoad);

           

            SimHelper.Initialize(this.firefox);
            GeckoJsToCsHelper.Initialize(this.firefox);

            //鼠标恢复默认
            this.Cursor = Cursors.Default;
        }

        private void HomePage_InitializeComponent(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
        }
    }
}
