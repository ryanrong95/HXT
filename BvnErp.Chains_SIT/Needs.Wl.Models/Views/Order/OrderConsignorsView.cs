using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 代理订单交货人的视图
    /// </summary>
    public class OrderConsignorsView : View<Models.OrderConsignor, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderConsignorsView(string orderID)
        {
            this.OrderID = orderID;
            this.AllowPaging = false;
        }

        protected override IQueryable<OrderConsignor> GetIQueryable()
        {
            return from consignor in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>()
                   where consignor.OrderID == this.OrderID && consignor.Status == (int)Enums.Status.Normal
                   select new Models.OrderConsignor
                   {
                       ID = consignor.ID,
                       OrderID = consignor.OrderID,
                       Type = (Enums.SZDeliveryType)consignor.Type,
                       Name = consignor.Name,
                       Contact = consignor.Contact,
                       Mobile = consignor.Mobile,
                       Tel = consignor.Tel,
                       Address = consignor.Address,
                       IDType = consignor.IDType,
                       IDNumber = consignor.IDNumber,
                       Status = consignor.Status,
                       CreateDate = consignor.CreateDate,
                       UpdateDate = consignor.UpdateDate,
                       Summary = consignor.Summary
                   };
        }
    }
}