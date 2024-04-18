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
    public class CustomsElementsDefaultsAll : UniqueView<Model.Origins.CustomsElementsDefault, ScCustomsReponsitory>
    {
        public CustomsElementsDefaultsAll()
        {
        }
        protected override IQueryable<Model.Origins.CustomsElementsDefault> GetIQueryable()
        {
            return new Origins.CustomsElementsDefaultOrigin();
        }
    }
}
