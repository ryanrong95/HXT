using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单项的视图
    /// </summary>
    public class CostApplyItemsView : UniqueView<Models.CostApplyItem, ScCustomsReponsitory>
    {
        public CostApplyItemsView()
        {
        }

        public CostApplyItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.CostApplyItem> GetIQueryable()
        {
            var linq = from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyItems>()
                       where orderItem.Status == (int)Enums.Status.Normal
                       select new Models.CostApplyItem
                       {
                           ID = orderItem.ID,
                           CostApplyID = orderItem.CostApplyID,
                           FeeType = (Enums.FinanceFeeType)orderItem.FeeType,
                           FeeDesc = orderItem.FeeDesc,
                           Amount = orderItem.Amount,
                           Status = (Enums.Status)orderItem.Status,
                           CreateDate = orderItem.CreateDate,
                           UpdateDate = orderItem.UpdateDate,
                           Summary = orderItem.Summary,
                       };

            return linq;
        }

        public IEnumerable<Models.CostApplyItem> GetOrderItemsOrigin(string CostApplyID)
        {

            return from orderItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CostApplyItems>()
                   where orderItem.CostApplyID == CostApplyID && orderItem.Status == (int)Enums.Status.Normal
                   select new Models.CostApplyItem
                   {
                       ID = orderItem.ID,
                       CostApplyID = orderItem.CostApplyID,
                       FeeType = (Enums.FinanceFeeType)orderItem.FeeType,
                       FeeDesc = orderItem.FeeDesc,
                       Amount = orderItem.Amount,
                       Status = (Enums.Status)orderItem.Status,
                       CreateDate = orderItem.CreateDate,
                       UpdateDate = orderItem.UpdateDate,
                       Summary = orderItem.Summary,                       
                   };
        }

        public bool CheckOrderItemsVaild(string ProductUniqueCode)
        {
            return this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Any(t => t.ProductUniqueCode == ProductUniqueCode);
        }
    }
}
