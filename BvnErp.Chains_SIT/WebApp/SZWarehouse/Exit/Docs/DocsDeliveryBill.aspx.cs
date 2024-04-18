using Needs.Ccs.Services;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Exit.Docs
{
    public partial class DocsDeliveryBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 送货单信息
        /// </summary>
        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ID];
            //var PackingDate = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.EntryNotice
            //    .Where(x => x.Order.ID == exitNotice.Order.ID && x.EntryNoticeStatus == Needs.Ccs.Services.Enums.EntryNoticeStatus.Sealed)
            //    .FirstOrDefault()
            //    .UpdateDate;
            // var SZPackingDate = exitNotice.SZItems.Select(x => x.Sorting.SZPackingDate).FirstOrDefault();

            var exitNoticeFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticeFilesView
                .Where(t => t.ExitNoticeID == ID && t.FileType == Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                .FirstOrDefault();

            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.DeliveryBill.ExitNoticeID,
                    OrderID = exitNotice.DeliveryBill.OrderID,
                    ClientName = exitNotice.DeliveryBill.ClientName,
                    Contactor = exitNotice.DeliveryBill.Contactor,
                    ContactTel = exitNotice.DeliveryBill.ContactTel,
                    Address = exitNotice.DeliveryBill.Address,//送货地址
                    DriverName = exitNotice.DeliveryBill.DriverName,
                    DriverTel = exitNotice.DeliveryBill.DriverTel,
                    License = exitNotice.DeliveryBill.License,
                    DeliveryTime = exitNotice.DeliveryBill.DeliveryTime.ToString("yyyy-MM-dd"),
                    //SZPackingDate = PackingDate.ToString("yyyy-MM-dd") == null ? "" : PackingDate.ToString("yyyy-MM-dd"),//出库的装箱日期取的是香港的封箱日期
                    PackNo = exitNotice.DeliveryBill.PackNo,
                    ExitType = (int)Needs.Ccs.Services.Enums.ExitType.Delivery,
                    Purchaser = PurchaserContext.Current.CompanyName,
                    SealUrl = "../../../" + PurchaserContext.Current.BillStamp.ToUrl(),
                    CreateDate = exitNotice.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    IsFileUploaded = exitNoticeFile != null ? true : false,
                    FileName = exitNoticeFile != null ? exitNoticeFile.Name : string.Empty,
                    FileUrl = exitNoticeFile != null ? ConfigurationManager.AppSettings["FileServerUrl"] + @"/" + exitNoticeFile.URL.Replace(@"\", @"/") : string.Empty,
                    ExitNoticeStatus = exitNotice.ExitNoticeStatus,
                }.Json();
            }
        }

        /// <summary>
        /// 待出库商品信息
        /// </summary>
        protected void ProductData()
        {

            string id = Request["ExitNoticeID"];
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.Where(x => x.ExitNoticeID == id).OrderBy(x => x.StoreStorage.BoxIndex);
            Func<Needs.Ccs.Services.Models.SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                SortingID = item.Sorting.ID,
                CaseNumber = item.StoreStorage.BoxIndex,
                StockCode = item.StoreStorage.StockCode,
                PackingDate = item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd") == null ? "" : item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd"),  //出库的装箱日期
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                WrapType = item.Sorting.WrapType,
                Manufacturer = item.Sorting.OrderItem.Manufacturer,
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd")//装箱日期
            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());
        }

    }
}