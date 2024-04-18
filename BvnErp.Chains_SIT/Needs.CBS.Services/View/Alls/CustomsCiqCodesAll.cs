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
    public class CustomsCiqCodesAll : UniqueView<Model.Origins.CustomsCiqCode, ScCustomsReponsitory>
    {
        public CustomsCiqCodesAll()
        {
        }
        protected override IQueryable<Model.Origins.CustomsCiqCode> GetIQueryable()
        {
            return new Origins.CustomsCiqCodeOrigin().Where(t => t.Status==Enums.Status.Normal);
        }
    }
}
