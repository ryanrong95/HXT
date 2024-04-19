using NtErp.Wss.Oss.Services.Models;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 运单项视图
    /// </summary>
    public class WayItemsOrder : UniqueView<WayItemOrder, CvOssReponsitory>
    {
        string orderID;
        public WayItemsOrder(string orderID)
        {
            this.orderID = orderID;
        }

        protected override IQueryable<WayItemOrder> GetIQueryable()
        {
            //var waybillsView = new WaybillAlls(this.Reponsitory);
            var wayItemAlls = new WayItemAlls(this.Reponsitory);

            var linq = from entity in wayItemAlls
                           //join bill in waybillsView on entity.WaybillID equals bill.ID
                       where entity.OrderID == this.orderID
                       select new WayItemOrder
                       {
                           ID = entity.ID,
                           WaybillID = entity.WaybillID,
                           OrderID = entity.OrderID,
                           OrderItemID = entity.OrderItemID,
                           Weight = entity.Weight,
                           Count = entity.Count,
                           Source = entity.Source,
                           //Bill = new WaybillKit
                           //{
                           //    Carrier = bill.Carrier,
                           //    Weight = bill.Weight
                           //}
                       };

            return linq;
        }
    }
}
