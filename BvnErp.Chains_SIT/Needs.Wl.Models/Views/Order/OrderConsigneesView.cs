using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 代理订单收货人的视图
    /// </summary>
    public class OrderConsigneesView : View<Models.OrderConsignee, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderConsigneesView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<OrderConsignee> GetIQueryable()
        {
            return from consignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>()
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on consignee.ClientSupplierID equals supplier.ID
                   where consignee.OrderID == this.OrderID && consignee.Status == (int)Enums.Status.Normal
                   select new Models.OrderConsignee
                   {
                       ID = consignee.ID,
                       OrderID = consignee.OrderID,
                       ClientSupplier = new ClientSupplier()
                       {
                           ID = supplier.ID,
                           ChineseName = supplier.ChineseName,
                           Name = supplier.Name,
                       },
                       Type = (Enums.HKDeliveryType)consignee.Type,
                       Contact = consignee.Contact,
                       Mobile = consignee.Mobile,
                       Tel = consignee.Tel,
                       Address = consignee.Address,
                       PickUpTime = consignee.PickUpTime,
                       WayBillNo = consignee.WayBillNo,
                       Status = consignee.Status,
                       CreateDate = consignee.CreateDate,
                       UpdateDate = consignee.UpdateDate,
                       Summary = consignee.Summary
                   };
        }
    }
}
