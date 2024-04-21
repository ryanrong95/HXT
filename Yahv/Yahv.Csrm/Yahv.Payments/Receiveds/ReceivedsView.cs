using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Linq;
using Yahv.Payments.Models;
using Yahv.Underly;

namespace Yahv.Payments
{
    /// <summary>
    /// 实收视图
    /// </summary>
    public class ReceivedsView : UniqueView<Received, PvbCrmReponsitory>
    {
        public ReceivedsView()
        {

        }

        public ReceivedsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Received> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.Receiveds>()
                   select new Received()
                   {
                       Price = entity.Price,
                       AccountType = (AccountType)entity.AccountType,
                       CreateDate = entity.CreateDate,
                       ID = entity.ID,
                       WaybillID = entity.WaybillID,
                       AdminID = entity.AdminID,
                       ReceivableID = entity.ReceivableID,
                       OrderID = entity.OrderID,
                       Summay = entity.Summay,
                       FlowID = entity.FlowID,
                       AccountCode = entity.AccountCode,
                       Currency1 = (Currency)entity.Currency1,
                       Price1 = entity.Price1,
                       Rate1 = entity.Rate1,
                   };
        }
    }
}
