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
    public partial class OrderList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }


        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string OrderID = Request.QueryString["OrderID"];
            string EntryNumber = Request.QueryString["EntryNumber"];

            var entryNoticeView = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.EntryNotice;
            var data = entryNoticeView.Where(entryEntry => entryEntry.WarehouseType == WarehouseType.HongKong);
            if (!string.IsNullOrEmpty(OrderID))
            {
                data = data.Where(entryEntry => entryEntry.Order.ID.Contains(OrderID));
            }
            if (!string.IsNullOrEmpty(EntryNumber))
            {
                data = data.Where(entryEntry => entryEntry.Order.Client.ClientCode == EntryNumber);
            }
            Func<Needs.Ccs.Services.Models.EntryNotice, object> convert = item => new
            {
                ID = item.ID,
                OrderID = item.Order.ID,
                EntryNumber = item.Order.Client.ClientCode,
                ClientName = item.Order.Client.Company.Name,
                SupplierName = item.Order.OrderConsignee.ClientSupplier.Name,
                CreateDate = item.Order.CreateDate.ToString("yyyy-MM-dd"),
                Status = item.Order.OrderStatus.GetDescription(),
            };
            data = data.OrderByDescending(t => t.CreateDate);
            this.Paging(data, convert);
        }
    }
}