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
    /// 海关检疫视图
    /// </summary>
    public class CustomsQuarantinesAll : UniqueView<Models.Origins.CustomsQuarantine, ScCustomsReponsitory>
    {
        public CustomsQuarantinesAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsQuarantine> GetIQueryable()
        {
            return new Origins.CustomsQuarantinesOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }
}
