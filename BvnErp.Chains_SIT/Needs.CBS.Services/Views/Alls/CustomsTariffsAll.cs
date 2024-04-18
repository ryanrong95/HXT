using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Views.Alls
{
    /// <summary>
    /// 海关税则视图
    /// </summary>
    public class CustomsTariffsAll : UniqueView<Models.Origins.CustomsTariff, ScCustomsReponsitory>
    {
        public CustomsTariffsAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsTariff> GetIQueryable()
        {
            return new Origins.CustomsTariffsOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }
}
