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
    /// 申报要素默认值视图
    /// </summary>
    public class CustomsElementsDefaultsAll : UniqueView<Models.Origins.CustomsElementsDefault, ScCustomsReponsitory>
    {
        public CustomsElementsDefaultsAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsElementsDefault> GetIQueryable()
        {
            return new Origins.CustomsElementsDefaultsOrigin();
        }
    }
}
