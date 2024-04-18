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
    /// /// <summary>
    /// 交通工具View
    /// </summary>
    /// </summary>
    public class OrderWaybillItemView : UniqueView<Models.OrderWaybillItem, ScCustomsReponsitory>
    {
        public OrderWaybillItemView()
        {
        }

        internal OrderWaybillItemView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.OrderWaybillItem> GetIQueryable()
        {
            var sortingView = new SortingsView(this.Reponsitory);
            return from waybillItem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWaybillItems>()
                   join sorting in sortingView on waybillItem.SortingID equals sorting.ID
                   select new Models.OrderWaybillItem
                   {
                       ID = waybillItem.ID,
                       OrderWaybillID = waybillItem.OrderWaybillID,
                       Sorting = sorting,
                   };
        }
    }

}
