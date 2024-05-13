using Gecko;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Web.Services;
using Web.Services.Models;


namespace WinApp
{
    public partial class Main : Form
    {
        Gecko.GeckoWebBrowser browser = null;
        public String userToken { get; set; }
        public Main()
        {
            InitializeComponent();
            Xpcom.Initialize("Firefox64");

           
        }

        Logon logon { get; set; }


        private void Main_Load(object sender, EventArgs e)
        {
            this.Text = "库房登陆";
            browser = new GeckoWebBrowser { Dock = DockStyle.Fill };
            browser.Navigate(string.Concat(FromType.Scheme.GetDescription(), "://", FromType.Web.GetDescription()));

            browser.AddMessageEventListener("PageEvents", (param) => PageEvents(param));
           
            this.Controls.Add(browser);
            this.Cursor = Cursors.Default;

        }


        private void PageEvents(string param)
        {
           
            var data = param.ToString().JsonTo<NameValue>();
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
                case "warehousesinit":
                    StoreHouseSetInitEvent();
                    break;
                case "setwarehouseset":
                    SetWareHouseEvent(data.Value);             
                    break;
                default:
                    break;
            }
        }


        private void SetWareHouseEvent(object data)
        {
            var obj = data.ToString().JsonTo<NameValue>();
            WareHouseManager.Enter(obj.Name, obj.Value);
        }

        private void SetPrintEvent(object data)
        {
            var obj = data.ToString().JsonTo<NameValue>();
            PrintingManager.Enter(obj.Name, obj.Value);
        }
        private void LogonEvent(object data)
        {
            var logon = data.ToString().JsonTo<Logon>();

            this.userToken = logon.Token;
            this.logon = logon;
        }

        private void PageInitEvent()
        {
            new Gecko.JQuery.JQueryExecutor(browser.Window).ExecuteJQuery($"var IsWinform=1; var user={logon.Json()};var wareHouseID=\"{WareHouseManager.WareHouseID}\"");
        }

        private void PrintSetInitEvent()
        {
            new Gecko.JQuery.JQueryExecutor(browser.Window).ExecuteJQuery($"var IsWinform=1; var user={logon.Json()};var obj={PrintingManager.List()};");
        }

        private void StoreHouseSetInitEvent()
        {
         
            new Gecko.JQuery.JQueryExecutor(browser.Window).ExecuteJQuery($"var IsWinform=1; var user={logon.Json()};var obj={WareHouseManager.List()};");
        }

       
    }
}
