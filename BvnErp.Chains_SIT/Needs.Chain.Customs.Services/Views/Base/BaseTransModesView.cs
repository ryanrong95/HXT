using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseTransModesView : UniqueView<Models.BaseTransMode, ScCustomsReponsitory>
    {
        public BaseTransModesView()
        {
        }

        internal BaseTransModesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseTransMode> GetIQueryable()
        {
            return from baseTransMode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseTransMode>()
                   select new Models.BaseTransMode
                   {
                       ID = baseTransMode.ID,
                       Code = baseTransMode.Code,
                       Name = baseTransMode.Name
                   };
        }
    }
}