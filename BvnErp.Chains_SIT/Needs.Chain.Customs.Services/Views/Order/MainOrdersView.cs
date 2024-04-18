using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class MainOrdersViewBase<T>: UniqueView<T, ScCustomsReponsitory> where T : Models.MainOrder, new()
    {
        public MainOrdersViewBase()
        {
        }

        internal MainOrdersViewBase(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<T> GetIQueryable()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>()
                   where order.Status == (int)Enums.Status.Normal
                   select new T
                   {
                       ID = order.ID,
                       Type = (Enums.OrderType)order.Type,
                       CreateDate = order.CreateDate,
                   };

        }
    }

    /// <summary>
    /// 只有Order表
    /// </summary>
    public class MainOrdersView : MainOrdersViewBase<Models.MainOrder>
    {
        public MainOrdersView() : base()
        {
        }

        public MainOrdersView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.MainOrder> GetIQueryable()
        {
            return base.GetIQueryable();
        }
    }
}
