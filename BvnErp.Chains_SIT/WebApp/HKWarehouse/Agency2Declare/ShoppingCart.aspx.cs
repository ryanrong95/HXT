using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Agency2Declare
{
    public partial class ShoppingCart : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string WindowName = Request.QueryString["WindowName"];
            WindowName = WindowName ?? string.Empty;
            this.Model.WindowName = WindowName;
            this.Model.PvWsOrderUrl = System.Configuration.ConfigurationManager.AppSettings["PvWsOrder"];
            string ClientID = Request.QueryString["ClientID"];

            var Storage = new Needs.Ccs.Services.Views.StoragesTopOriginalView().Where(t => t.ClientID == ClientID).FirstOrDefault();
            var client = new Needs.Ccs.Services.Views.ClientsView().Where(t => t.Company.Name== Storage.ClientName).FirstOrDefault();
            this.Model.OutClientCode = client.ClientCode;
            this.Model.OutClientID = ClientID;
        }

        protected void data()
        {
            string Model = Request.QueryString["CartInfo"].Replace("&quot;", "\'").Replace("&amp;", "&");
            string[] IDS = Model.Split(',');
            List<string> stroageIDs = new List<string>();
            for(int i = 0; i < IDS.Length-1; i++)
            {
                stroageIDs.Add(IDS[i]);
            }
            //dynamic model = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Cart>>(Model);

            var financeAccounts = new Needs.Ccs.Services.Views.StoragesTopOriginalView().Where(t=> stroageIDs.Contains(t.ID)).AsQueryable();

            Func<Needs.Ccs.Services.Models.StorageTop, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.EnterDate.ToString("yyyy-MM-dd HH:mm:ss"),
                PartNumber = item.PartNumber,
                CustomsName = item.CustomsName,
                Manufacturer = item.Manufacturer,
                Supplier = item.Supplier,              
                UnitPrice = item.UnitPrice?.ToString("0.00"),
                Quantity = item.Quantity?.ToString("0.00"),
                WareHouseName = item.WareHouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                TotalPrice = (item.UnitPrice * item.Quantity)?.ToString("0.00"),
                WareHouseID = item.WareHouseID,
                IsCheck = false, //我的库存页面复选框
                InputID = item.InputID,
                Origin = item.Origin,
                Num = item.Quantity,

                SaleQuantity = item.Quantity, //购物车一开始的数量
            };

            this.Paging(financeAccounts, convert);

        }

        protected void CompanyName()
        {
            string ClientNo = Request.Form["ClientNo"];
            var client = new Needs.Ccs.Services.Views.ClientsView().Where(t => t.ClientCode == ClientNo).FirstOrDefault();
            if (client != null)
            {
                Response.Write((new { success = true, message = client.Company.Name }).Json());
            }
            else
            {
                Response.Write((new { success = false, message = "没有查询到该客户信息" }).Json());
            }
        }

        private class Cart
        {
            public string ID { get; set; }
            public string PartNumber { get; set; }
            public decimal Quantity { get; set; }
            public decimal OrderQuantity { get; set; }
        }
    }
}