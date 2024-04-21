using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvRoute.Services.Enums;
using Yahv.PvRoute.Services.Models;
using Yahv.Underly;

namespace Yahv.PvRoute.Services.Views.Origins
{
    public class BillOrigin : QueryView<Bill, PvRouteReponsitory>
    {
        internal BillOrigin()
        {

        }
        internal BillOrigin(PvRouteReponsitory reponsitory) : base(reponsitory)
        {

        }
        protected override IQueryable<Bill> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvRoute.Bills>()
                   select new Bill()
                   {
                       ID = entity.ID,
                       FaceOrderID = entity.FaceOrderID,
                       Quantity = entity.Quantity,
                       Weight = entity.Weight,
                       Price = entity.Price,
                       Currency = (Currency)entity.Currency,
                       Carrier = (PrintSource)entity.Carrier,
                       FeeDetail = entity.FeeDetail,
                       Checker = entity.Checker,
                       CheckTime = entity.CheckTime,
                       Reviewer = entity.Reviewer,
                       ReviewTime = entity.ReviewTime,
                       Cashier = entity.Cashier,
                       CashierTime = entity.CashierTime,
                       DateIndex = entity.DateIndex,
                       Source = (RecordSource)entity.Source,
                       CreateDate = entity.CreateDate
                   };
        }
    }
}
