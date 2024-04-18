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
    public class CustomsQuarantinesAll : UniqueView<Model.Origins.CustomsQuarantine, ScCustomsReponsitory>
    {
        public CustomsQuarantinesAll()
        {
        }
        protected override IQueryable<Model.Origins.CustomsQuarantine> GetIQueryable()
        {
            return new Origins.CustomsQuarantineOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }
}
