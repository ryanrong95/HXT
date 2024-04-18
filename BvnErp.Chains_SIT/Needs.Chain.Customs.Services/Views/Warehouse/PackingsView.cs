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
    /// 装箱结果View
    /// </summary>
    public class PackingsView : UniqueView<Models.Packing, ScCustomsReponsitory>
    {
        public PackingsView()
        {
        }

        public PackingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Packing> GetIQueryable()
        {
            var result = from packing in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Packings>()
                         where packing.Status == (int)Enums.Status.Normal
                         orderby packing.BoxIndex
                         select new Models.Packing
                         {
                             ID = packing.ID,
                             AdminID = packing.AdminID,
                             OrderID = packing.OrderID,
                             BoxIndex = packing.BoxIndex,
                             PackingDate = packing.PackingDate,
                             Weight = packing.Weight,
                             WrapType = packing.WrapType,
                             PackingStatus = (Enums.PackingStatus)packing.PackingStatus,
                             Status = (Enums.Status)packing.Status,
                             CreateDate = packing.CreateDate,
                             UpdateDate = packing.UpdateDate,
                             Summary = packing.Summary,
                         };
            return result;
        }
    }
}
