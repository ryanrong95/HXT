using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseUnitsView : UniqueView<Models.Unit, ScCustomsReponsitory>
    {
        public BaseUnitsView()
        {
        }

        internal BaseUnitsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Unit> GetIQueryable()
        {
            return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>()
                   select new Models.Unit
                   {
                       ID = unit.ID,
                       Code = unit.Code,
                       Name = unit.Name,
                   };
        }
    }
}
