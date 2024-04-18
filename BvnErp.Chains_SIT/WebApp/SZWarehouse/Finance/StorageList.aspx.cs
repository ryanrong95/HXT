using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWarehouse.Finance
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

            var storage= Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZStoreStorage.AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                storage = storage.Where(item => item.Sorting.OrderID.Contains(OrderID));
            }

            Func<Needs.Ccs.Services.Models.SZStoreStorage, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Sorting.OrderID,
                ClientCode = item.Order.Client.ClientCode,
                Model = item.OrderItem.Model,
                Name = item.Sorting.OrderItem.Category.Name,
                Manufacturer = item.OrderItem.Manufacturer,
                Quantity = item.Quantity, 
                Currency = item.Order.Currency,
                UnitPrice = item.SZSorting.UnitPrice,
                Origin = item.SZSorting.Origin,
                BoxIndex = item.BoxIndex,
                StockCode = item.StockCode,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
                UpdateDate = item.UpdateDate.ToString("yyyy-MM-dd"),
            };
            this.Paging(storage, convert);
        }

        /// <summary>
        /// 入库数据加载
        /// </summary>
        protected void dataInput()
        {
            string OrderID = Request.QueryString["OrderID"];

            var storage = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZSorting.AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                storage = storage.Where(item => item.OrderID.Contains(OrderID));
            }

            Func<Needs.Ccs.Services.Models.SZSorting, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                ClientCode = item.Order.Client.ClientCode,
                Model = item.OrderItem.Model,
                Name = item.OrderItem.Category.Name,
                Manufacturer = item.OrderItem.Manufacturer,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Currency = item.Order.Currency,
                Origin = item.OrderItem.Origin,
                BoxIndex = item.BoxIndex,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            };
            this.Paging(storage, convert);
        }

        /// <summary>
        /// 出库数据加载
        /// </summary>
        protected void dataOutput()
        {
            string OrderID = Request.QueryString["OrderID"];

            var exitNoticeItem = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.AsQueryable();
            if (!string.IsNullOrEmpty(OrderID))
            {
                exitNoticeItem = exitNoticeItem.Where(item => item.Sorting.OrderID.Contains(OrderID));
            }

            Func<Needs.Ccs.Services.Models.SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Sorting.OrderID,
                ClientCode = item.Sorting.Order.Client.ClientCode,
                Model = item.Sorting.OrderItem.Model,
                Name = item.Sorting.OrderItem.Category.Name,
                Manufacturer = item.Sorting.OrderItem.Manufacturer,
                Quantity = item.Quantity,
                UnitPrice = item.Sorting.UnitPrice,
                Currency = item.Sorting.Order.Currency,
                Origin = item.Sorting.Origin,
                BoxIndex = item.Sorting.BoxIndex,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd"),
            };
            this.Paging(exitNoticeItem, convert);
        }
    }
}