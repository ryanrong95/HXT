using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Agency2Declare
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载
        /// </summary>
        protected void data()
        {
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string ID = Request.QueryString["ID"];
            string PartNumber = Request.QueryString["PartNumber"];
            string ClientName = Request.QueryString["ClientName"];

            //var financeAccounts = new Needs.Ccs.Services.Views.StoragesTopOriginalView().OrderByDescending(t=>t.EnterDate).AsQueryable();
           
            var financeAccounts =  Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyStorageTopView.AsQueryable();
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                DateTime dtStart = Convert.ToDateTime(StartDate);
                financeAccounts = financeAccounts.Where(t => t.EnterDate >= dtStart);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                DateTime dtEnd = Convert.ToDateTime(EndDate);
                financeAccounts = financeAccounts.Where(t => t.EnterDate<= dtEnd);
            }
            if (!string.IsNullOrEmpty(ID))
            {
                ID = ID.Trim();
                financeAccounts = financeAccounts.Where(t => t.ID==ID);
            }
            if (!string.IsNullOrEmpty(PartNumber))
            {
                PartNumber = PartNumber.Trim();
                financeAccounts = financeAccounts.Where(t => t.PartNumber == PartNumber);
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim();
                financeAccounts = financeAccounts.Where(t => t.ClientName == ClientName);
            }

            Func<Needs.Ccs.Services.Models.StorageTop, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.EnterDate.ToString("yyyy-MM-dd HH:mm:ss"),
                PartNumber = item.PartNumber,
                CustomsName = item.CustomsName,
                Manufacturer = item.Manufacturer,
                Supplier = item.Supplier,
                Currency = item.Currency?.GetDescription(),               
                UnitPrice = item.UnitPrice?.ToString("0.00"),
                Quantity = item.Quantity?.ToString("0.00"),
                WareHouseName = item.WareHouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                TotalPrice = (item.UnitPrice * item.Quantity)?.ToString("0.00"),
                WareHouseID = item.WareHouseID,
                IsCheck = false, //我的库存页面复选框
                InputID = item.InputID,
                Origin = item.Origin,
                Num = item.Quantity,
                ClientID = item.ClientID,
                ClientName = item.ClientName,
                SaleQuantity = item.Quantity, //购物车一开始的数量
            };
            this.Paging(financeAccounts, convert);
        }
    }
}