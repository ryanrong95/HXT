using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Ccs.Services.Enums;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 付汇记录视图
    /// </summary>
    public class OrderPayExchangeItemsView : UniqueView<Models.OrderPayExchangeRecord, ScCustomsReponsitory>
    {
        public OrderPayExchangeItemsView()
        {

        }

        internal OrderPayExchangeItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<OrderPayExchangeRecord> GetIQueryable()
        {
            var payexchangeitem = new PayExchangeApplieItemsView(this.Reponsitory);
            var userView = new UsersView(this.Reponsitory);
            return from item in payexchangeitem 
                   join apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>() on item.PayExchangeApplyID equals apply.ID
                   join user in userView on apply.UserID equals user.ID into users
                   from _user in users.DefaultIfEmpty()
                   where apply.Status == (int)Enums.Status.Normal
                   select new OrderPayExchangeRecord
                   {
                       ID = item.ID,
                       OrderID=item.OrderID,
                       SupplierName = apply.SupplierName,
                       ApplyTime = apply.CreateDate,
                       User = _user,
                       Amount = item.Amount,
                       Status = (PayExchangeApplyStatus)apply.PayExchangeApplyStatus,
                       FatherID = apply.FatherID
                   };
        }
    }
}
