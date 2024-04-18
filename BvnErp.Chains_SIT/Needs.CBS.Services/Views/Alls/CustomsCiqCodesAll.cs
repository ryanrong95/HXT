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
    /// 海关检验检疫编码视图
    /// </summary>
    public class CustomsCiqCodesAll : UniqueView<Models.Origins.CustomsCiqCode, ScCustomsReponsitory>
    {
        public CustomsCiqCodesAll()
        {
        }

        protected override IQueryable<Models.Origins.CustomsCiqCode> GetIQueryable()
        {
            return new Origins.CustomsCiqCodesOrigin().Where(t => t.Status == Enums.Status.Normal);
        }
    }
}
