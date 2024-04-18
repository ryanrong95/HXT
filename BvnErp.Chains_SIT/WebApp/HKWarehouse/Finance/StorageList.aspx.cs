using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.HKWarehouse.Finance
{
    /// <summary>
    /// 入库通知--香港库房
    /// 入库通知列表查询界面
    /// </summary>
    public partial class StorageList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 在库数据加载
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string ClientCode = Request.QueryString["ClientCode"];
            
            var storage = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKStoreStorage.AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                storage = storage.Where(item => item.Sorting.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                storage = storage.Where(item => item.Order.Client.ClientCode.Contains(ClientCode));
            }
            Func<Needs.Ccs.Services.Models.StoreStorage, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Sorting.OrderID,
                ClientCode = item.Order.Client.ClientCode,
                Model = item.OrderItem.Model,
                Name = item.Sorting.OrderItem.Category.Name,
                Manufacturer = item.OrderItem.Manufacturer,
                Quantity = item.Quantity,
                UnitPrice = item.Sorting.OrderItem.UnitPrice,
                Currency = item.Order.Currency,
                Origin = item.Sorting.OrderItem.Origin,
                BoxIndex = item.BoxIndex,
                StockCode = item.StockCode,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
            };
            storage = storage.OrderByDescending(t => t.CreateDate);
            this.Paging(storage, convert);
        }

        /// <summary>
        /// 入库数据加载
        /// </summary>
        protected void dataInput()
        {
            string OrderID = Request.QueryString["OrderID"];
            string ClientCode = Request.QueryString["ClientCode"];
            var storage = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKSorting.AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                storage = storage.Where(item => item.OrderID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                storage = storage.Where(item => item.Order.Client.ClientCode.Contains(ClientCode));
            }
            Func<Needs.Ccs.Services.Models.HKSorting, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ClientCode = item.Order.Client.ClientCode,
                Model = item.OrderItem.Model,
                Name = item.OrderItem.Category.Name,
                Manufacturer = item.OrderItem.Manufacturer,
                Quantity = item.Quantity,
                UnitPrice = item.OrderItem.UnitPrice,
                Currency = item.Order.Currency,
                Origin = item.OrderItem.Origin,
                BoxIndex = item.BoxIndex,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            };
            storage = storage.OrderByDescending(t => t.CreateDate);
            this.Paging(storage, convert);
        }

        /// <summary>
        /// 出库数据加载
        /// </summary>
        protected void dataOutput()
        {
            string OrderID = Request.QueryString["OrderID"];
            string ClientCode = Request.QueryString["ClientCode"];
            var exitNoticeItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.HKExitNoticeItem.AsQueryable();
            exitNoticeItem = exitNoticeItem.Where(item => item.ExitNoticeStatus == ExitNoticeStatus.Exited);
            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNoticeItem = exitNoticeItem.Where(item => item.DecList.DeclarationNoticeItem.Sorting.OrderID.Contains(OrderID));
            }

            if (!string.IsNullOrEmpty(ClientCode))
            {
                exitNoticeItem = exitNoticeItem.Where(item => item.DecList.DeclarationNoticeItem.Sorting.Order.Client.ClientCode.Contains(ClientCode));
            }
            Func<Needs.Ccs.Services.Models.HKExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.DecList.DeclarationNoticeItem.Sorting.OrderID,
                ClientCode = item.DecList.DeclarationNoticeItem.Sorting.Order.Client.ClientCode,
                Model = item.DecList.GoodsModel,
                //Name = item.DecList.DeclarationNoticeItem.Sorting.Product.Name,
                CustomsName = item.DecList.GName,
                Manufacturer = item.DecList.GoodsBrand,
                Quantity = item.Quantity,
                UnitPrice = item.DecList.DeclPrice,
                Currency = item.DecList.TradeCurr,
                Origin = item.DecList.OriginCountry,
                BoxIndex = item.DecList.CaseNo,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
            };
            exitNoticeItem = exitNoticeItem.OrderByDescending(t => t.UpdateDate);
            this.Paging(exitNoticeItem, convert);
        }
    }
}