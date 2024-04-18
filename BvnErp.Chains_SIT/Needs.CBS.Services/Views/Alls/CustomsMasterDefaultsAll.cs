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
    /// 海关申报地默认关联视图
    /// </summary>
    public class CustomsMasterDefaultsAll : UniqueView<Models.Origins.CustomsMasterDefault, ScCustomsReponsitory>
    {
        public CustomsMasterDefaultsAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsMasterDefault> GetIQueryable()
        {
            return new Origins.CustomsMasterDefaultsOrigin();
        }
    }
}
