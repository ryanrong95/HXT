using Layer.Data.Sqls;
using Needs.Ccs.Services;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.View.Alls
{
    public class CustomsTariffsAll : UniqueView<Model.Origins.CustomsTariff, ScCustomsReponsitory>
    {
        public CustomsTariffsAll()
        {
        }
        protected override IQueryable<Model.Origins.CustomsTariff> GetIQueryable()
        {
            return new Origins.CustomsTariffOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }
}
