using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Orders.Services.Views
{
    /// <summary>
    /// 订单的付汇记录
    /// </summary>
    public class OrderPayExchangeRecordsView : QueryView<Models.OrderPayExchangeRecord, ScCustomsReponsitory>
    {
        private string OrderID;

        public OrderPayExchangeRecordsView(string orderID)
        {
            this.OrderID = orderID;
        }

        protected override IQueryable<Models.OrderPayExchangeRecord> GetIQueryable()
        {
            return from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                   join apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on item.PayExchangeApplyID equals apply.ID
                   join userTable in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Users>() on apply.UserID equals userTable.ID into users
                   from user in users.DefaultIfEmpty()
                   where apply.Status == (int)Needs.Wl.Models.Enums.Status.Normal && item.OrderID == this.OrderID
                   orderby item.CreateDate descending
                   select new Models.OrderPayExchangeRecord
                   {
                       ID = item.ID,
                       OrderID = item.OrderID,
                       SupplierName = apply.SupplierName,
                       ApplyTime = apply.CreateDate,
                       User = user == null ? null : new Needs.Wl.Models.User()
                       {
                           ID = user.ID,
                           Name = user.Name,
                           RealName = user.RealName
                       },
                       Amount = item.Amount,
                       Status = (Needs.Wl.Models.Enums.PayExchangeApplyStatus)apply.PayExchangeApplyStatus
                   };
        }
    }
}