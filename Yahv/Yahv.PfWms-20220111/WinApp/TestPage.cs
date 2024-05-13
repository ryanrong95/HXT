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
    public partial class TestPage : Form
    {
        public TestPage()
        {
            this.Load += TestPage_InitializeComponent;
            InitializeComponent();
        }


        GeckoWebBrowser firefox;

        private void TestPage_Load(object sender, EventArgs e)
        {
            GeckoJsToCsHelper.Initialize();
            GeckoHelper.InitCamera(PhotoPage.Current);

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

            string homeUrl = $"{WebApp.Services.FromType.Scheme.GetDescription()}://{WebApp.Services.FromType.Web.GetDescription()}/index.html";

            //启用摄像头功能
            GeckoPreferences.User["media.navigator.permission.disabled"] = true;
            GeckoPreferences.User["permissions.default.camera"] = 100;


            //http://hv.warehouse.b1b.com/#/login

            this.firefox.Navigate(@"D:\Vs2015_Projects\Yahv\Yahv.PfWms\WinformApp\Html\test1.html", GeckoLoadFlags.FirstLoad);

            //this.firefox.Navigate(@"D:\Projects_vs2015\Yahv\Yahv.PfWms\WinformApp\Html\test1.html", GeckoLoadFlags.FirstLoad);

            GeckoJsToCsHelper.Initialize(this.firefox);

            this.firefox.DocumentCompleted += Firefox_DocumentCompleted;

            //鼠标恢复默认
            this.Cursor = Cursors.Default;
        }

        private void Firefox_DocumentCompleted(object sender, Gecko.Events.GeckoDocumentCompletedEventArgs e)
        {
            using (AutoJSContext context = new AutoJSContext(firefox.Window))
            {
                string result;
                //context.EvaluateScript("this['DFGHJKL']", firefox.Window.DomWindow, out result);
                //context.EvaluateScript("this['model']=1; ", firefox.Window.DomWindow, out result);
                //context.EvaluateScript("this['Zyz']({x:1})", (nsISupports)firefox.DomDocument.DomObject, out result);
                context.EvaluateScript("this['Zyz']({x:1,y:2})", (nsISupports)firefox.Window.DomWindow, out result);
            }
        }

        private void TestPage_InitializeComponent(object sender, EventArgs e)
        {
            Xpcom.Initialize("Firefox");
            CheckForIllegalCrossThreadCalls = false;
        }
    }
}
