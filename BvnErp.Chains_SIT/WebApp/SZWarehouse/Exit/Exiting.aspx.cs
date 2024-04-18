using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 出库通知-出库界面
    /// 深圳库房
    /// </summary>
    public partial class Exiting : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 数据加载
        /// </summary>
        protected void data()
        {
            string ExitNoticeID = Request.QueryString["ExitNoticeID"];
            if (string.IsNullOrEmpty(ExitNoticeID) == true)
            {
                Response.Write(new
                {
                    rows = 0,
                    total = 0
                }.Json());
                return;
            }
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ExitNoticeID];
            if (exitNotice == null)
            {
                Response.Write(new
                {
                    rows = 0,
                    total = 0
                }.Json());
                return;
            }
            var data = exitNotice.SZItems.OrderBy(x=>x.Sorting.BoxIndex);
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                OrderID = exitNotice.Order.ID,//订单编号
                SortingID = item.Sorting.ID,
                BoxIndex = item.Sorting.BoxIndex,
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                //WrapType = item.Sorting.WrapType,
                Manufactors = item.Sorting.OrderItem.Manufacturer,
                StockCode = item.StoreStorage?.StockCode,
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray(),
                total = data.Count()
            }.Json());
        }

        /// <summary>
        /// 扫描出库
        /// </summary>
        protected void OutStock()
        {
            try
            {
                string ExitNoticeID = Request["ExitNoticeID"];
                if (string.IsNullOrEmpty(ExitNoticeID))
                {
                    throw new Exception("通知编号为空");
                }
                var ExitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ExitNoticeID];
                if (ExitNotice == null)
                {
                    throw new Exception("查询通知结果为NULL");
                }
                if (ExitNotice.ExitNoticeStatus >= ExitNoticeStatus.Exited)
                {
                    throw new Exception("订单已出库");
                }
                foreach (var item in ExitNotice.SZItems)
                {
                    if (string.IsNullOrEmpty(item.StoreStorage?.StockCode))
                    {
                        throw new Exception("产品还未入库");
                    }
                }
                //出库
                ExitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                ExitNotice.OutStock();
               
                Response.Write((new { success = true, message = "出库成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "出库失败，" + ex.Message }).Json());
            }
        }
    }
}