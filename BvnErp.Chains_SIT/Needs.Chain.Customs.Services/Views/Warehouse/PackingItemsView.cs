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
    /// 分拣装箱结果View
    /// </summary>
    public class PackingItemsView : UniqueView<Models.PackingItem, ScCustomsReponsitory>
    {
        public PackingItemsView()
        {
        }

        internal PackingItemsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.PackingItem> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);

            return from packingItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PackingItems>()
                   join sorting in sortingView on packingItem.SortingID equals sorting.ID
                   where packingItem.Status == (int)Enums.Status.Normal
                   select new Models.PackingItem
                   {
                       ID = packingItem.ID,
                       PackingID = packingItem.PackingID,
                       Sorting = sorting,
                       Status = (Enums.Status)packingItem.Status,
                       CreateDate = packingItem.CreateDate,
                   };
        }
    }
}
