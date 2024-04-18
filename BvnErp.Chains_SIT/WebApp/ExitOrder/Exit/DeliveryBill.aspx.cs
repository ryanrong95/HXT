using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 出库通知-出库界面
    /// 深圳库房
    /// </summary>
    public partial class DeliveryBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];          
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = new CenterSZExitNoticeItemView().Where(item => item.ID == ID).FirstOrDefault();
            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ID,
                    OrderID = exitNotice.OrderID,
                    ClientName = exitNotice.ConsigneeCompany,
                    Contactor = exitNotice.ConsigneeContact,
                    ContactTel = exitNotice.ConsigneePhone,
                    Address = exitNotice.ConsigneeAddress,//送货地址
                    DriverName = exitNotice.ConsignorContact,
                    DriverTel = exitNotice.ConsignorPhone,
                    License = exitNotice.CarNumber,
                    DeliveryTime = exitNotice.CreateDate.ToString("yyyy-MM-dd"),
                    SZPackingDate = exitNotice.BoxingDate.ToString("yyyy-MM-dd"),//出库的装箱日期
                    PackNo = exitNotice.Quantity,
                    ExitType = (int)WaybillType.DeliveryToWarehouse,
                    Purchaser = PurchaserContext.Current.CompanyName,
                    SealUrl = "../../" + PurchaserContext.Current.BillStamp.ToUrl()
                }.Json();
            }
        }

        protected void ProductData()
        {
            string id = Request["ExitNoticeID"];          
            var exitNotice = new CenterSZExitNoticeItemView().Where(item => item.ID == id).OrderBy(x => x.BoxCode);
            Func<CenterWayBill, object> convert = item => new
            {
                ID = item.ID,              
                CaseNumber = item.BoxCode,
                StockCode = item.ShelveID,
                PackingDate = item.BoxingDate.ToString("yyyy-MM-dd"),  //出库的装箱日期                
                ProductName = item.DeclareName,
                Model = item.Manufacturer,
                Qty = item.Quantity,               
                Manufacturer = item.Manufacturer,

            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());
        }

        /// <summary>
        /// 送货单信息 原来的方法
        /// </summary>
        protected void LoadData1()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.MySZExitNotice[ID];
            var SZPackingDate = exitNotice.SZItems.Select(x => x.Sorting.SZPackingDate).FirstOrDefault();
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
                    SZPackingDate = SZPackingDate?.ToString("yyyy-MM-dd") == null ? "" : SZPackingDate?.ToString("yyyy-MM-dd"),//出库的装箱日期
                    PackNo = exitNotice.DeliveryBill.PackNo,
                    ExitType = (int)ExitType.Delivery,
                    Purchaser = PurchaserContext.Current.CompanyName,
                    SealUrl= "../../" + PurchaserContext.Current.BillStamp.ToUrl()
                }.Json();
            }
        }

        /// <summary>
        /// 待出库商品信息 原来的方法
        /// </summary>
        protected void ProductData1()
        {

            string id = Request["ExitNoticeID"];
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.Where(x => x.ExitNoticeID == id).OrderBy(x => x.StoreStorage.BoxIndex);
            Func<SZExitNoticeItem, object> convert = item => new
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

            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());
        }
    }
}