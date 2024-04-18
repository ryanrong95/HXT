using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DailyDeclareSumaryView : UniqueView<Models.DailyDeclareSummary, ScCustomsReponsitory>
    {
        public DailyDeclareSumaryView()
        { }

        internal DailyDeclareSumaryView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
       
        protected override IQueryable<Models.DailyDeclareSummary> GetIQueryable()
        {
            var result = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                         join d in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>()
                         on c.ID equals d.DeclarationID
                         group new { c,d} by new
                         {
                             ContrNo = c.ContrNo,
                             OwnerName = c.OwnerName,
                             PackNo = c.PackNo,
                             NetWt = c.NetWt,
                             GrossWt = c.GrossWt,
                             IsInspection = c.IsInspection,
                             IsQuarantine = c.IsQuarantine
                         } into m
                         select new DailyDeclareSummary
                         {
                             ContrNo = m.Key.ContrNo,
                             GQty = m.Sum(t=>t.d.GQty),
                             DeclTotal = m.Sum(t=>t.d.DeclTotal),
                             OwnerName = m.Key.OwnerName,
                             PackNo = m.Key.PackNo,
                             NetWt = m.Key.NetWt,
                             GrossWt = m.Key.GrossWt,
                             IsInspection = m.Key.IsInspection,
                             IsQuarantine = m.Key.IsQuarantine
                         };

            return result;
        }
    }  
}
