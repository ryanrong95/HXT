using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Yahv.PvWsOrder.Services.Common;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Erp;

namespace Yahv.PvOms.WebApp.Clients
{
    public partial class BillDetails : ErpParticlePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }

        public void LoadData()
        {
            var ID = Request.QueryString["ID"];
            var query = new PvWsOrder.Services.Views.Alls.BillsBase().Single(t => t.ID == ID);
            this.Model.BillData = new
            {
                ClientName = query.Client.Name,
                Address = query.Client.RegAddress??"",
                Contact = query.Client.Contact.Name,
                Mobile = query.Client.Contact.Mobile??"",
                Currency = query.Currency.ToString(),
            };
        }

        protected object data()
        {
            var ID = Request.QueryString["ID"];
            var bill = new PvWsOrder.Services.Views.Alls.BillsBase().Single(t => t.ID == ID);
            var billItems = new PvWsOrder.Services.Views.Alls.BillItemsAll().SearchByID(ID).ToMyObject();
            if (bill.Currency == Currency.CNY)
            {
                return new
                {
                    rows = billItems.Select(item => new
                    {
                        item.ID,
                        item.OrderID,
                        item.Consignee,
                        item.TypeDec,
                        item.Region,
                        Currency = bill.Currency.GetDescription(),
                        LeftDate = item.LeftDate.ToString("yyyy-MM-dd"),
                        LeftTotalPrice = item.LeftTotalPrice.ToString("f2"),
                        RemainTotalPrice = (item.LeftTotalPrice - item.RightTotalPrice).ToString("f2"),
                        StockFee = item.StockFee.ToString("f2"),
                        RegistrationFee = item.RegistrationFee.ToString("f2"),
                        EnterFee = item.EnterFee.ToString("f2"),
                        OtherFee = item.OtherFee.ToString("f2"),
                        DeliveryFee = item.DeliveryFee.ToString("f2"),
                        LabelFee = item.LabelFee.ToString("f2"),
                        CustomClearFee = item.CustomClearFee.ToString("f2"),
                    }).ToArray(),
                    total = billItems.Count(),
                }.Json();
            }
            else
            {
                return new
                {
                    rows = billItems.Select(item => new
                    {
                        item.ID,
                        item.OrderID,
                        item.Consignee,
                        item.TypeDec,
                        item.Region,
                        Currency = bill.Currency.GetDescription(),
                        LeftDate = item.LeftDate.ToString("yyyy-MM-dd"),
                        LeftTotalPrice = item.HKDLeftTotalPrice.ToString("f2"),
                        RemainTotalPrice = (item.HKDLeftTotalPrice - item.HKDRightTotalPrice).ToString("f2"),
                        StockFee = item.HKDStockFee.ToString("f2"),
                        RegistrationFee = item.HKDRegistrationFee.ToString("f2"),
                        EnterFee = item.HKDEnterFee.ToString("f2"),
                        OtherFee = item.HKDOtherFee.ToString("f2"),
                        DeliveryFee = item.HKDDeliveryFee.ToString("f2"),
                        LabelFee = item.HKDLabelFee.ToString("f2"),
                        CustomClearFee = item.HKDCustomClearFee.ToString("f2"),
                    }).ToArray(),
                    total = billItems.Count(),
                }.Json();
            }
        }

        /// <summary>
        /// 账单导出
        /// </summary>
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var ID = Request.QueryString["ID"];
            var bill = new PvWsOrder.Services.Views.Alls.BillsBase().Single(t => t.ID == ID);
            string filePath = bill.ToPdf();
            //下载文件
            DownLoadFile(filePath);
        }
    }
}