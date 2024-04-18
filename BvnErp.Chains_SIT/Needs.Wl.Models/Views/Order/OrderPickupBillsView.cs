using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.Models.Enums;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class OrderPickupBillsView : View<Models.OrderPickupBill, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderPickupBillsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<Models.OrderPickupBill> GetIQueryable()
        {
            return from notice in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                   join deliver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>() on notice.ID equals deliver.ExitNoticeID
                   join consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Consignees>() on deliver.ConsigneeID equals consignee.ID
                   where notice.ExitType == (int)ExitType.PickUp && notice.WarehouseType == (int)WarehouseType.ShenZhen && notice.Status == (int)Status.Normal
                   && notice.OrderID == this.OrderID
                   select new Models.OrderPickupBill
                   {
                       ID = notice.ID,
                       Code = deliver.Code,
                       PackNo = deliver.PackNo,
                       Name = consignee.Name,
                       IDType = (Enums.IDType)consignee.IDType,
                       IDNumber = consignee.IDNumber,
                       Mobile = consignee.Mobile,
                   };
        }
    }
}