using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
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
    public partial class DocsExpressBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 快递单信息
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
            //    .Where(x => x.Order.ID == exitNotice.Order.ID && x.EntryNoticeStatus == EntryNoticeStatus.Sealed)
            //    .FirstOrDefault()
            //    .UpdateDate;
            var exitNoticeFile = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExitNoticeFilesView
                .Where(t => t.ExitNoticeID == ID && t.FileType == Needs.Wl.Models.Enums.FileType.ReceiptConfirmationFile && t.Status == Needs.Wl.Models.Enums.Status.Normal)
                .FirstOrDefault();

            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ExpressBill.ExitNoticeID,
                    OrderID = exitNotice.ExpressBill.OrderID,
                    ClientName = exitNotice.ExpressBill.ClientName,
                    Contactor = exitNotice.ExpressBill.Contactor,//提货人
                    ContactTel = exitNotice.ExpressBill.ContactTel,
                    Address = exitNotice.ExpressBill.Address,
                    ExpressComp = exitNotice.ExpressBill.ExpressComp,
                    ExpressTy = exitNotice.ExpressBill.ExpressTy,
                    ExpressPayType = exitNotice.ExpressBill.ExpressPayType.GetDescription(),
                    PackNo = exitNotice.ExpressBill.PackNo,
                    ExitType = (int)ExitType.Express,
                    DeliveryTime = exitNotice.ExpressBill.DeliveryTime.ToString("yyyy-MM-dd"),
                    //SZPackingDate = PackingDate.ToString("yyyy-MM-dd"),//出库的装箱日期取的是香港的封箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
                    BillStamp = "../../../" + PurchaserContext.Current.BillStamp.ToUrl(),
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
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                SortingID = item.Sorting.ID,
                CaseNumber = item.StoreStorage.BoxIndex,
                StockCode = item.StoreStorage.StockCode,
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