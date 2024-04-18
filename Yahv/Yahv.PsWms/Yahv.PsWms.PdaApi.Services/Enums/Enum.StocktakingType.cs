using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.PdaApi.Services.Enums
{
    /// <summary>
    /// 盘点类型
    /// </summary>
    public enum StocktakingType
    {
        [Description("按个")]
        Single = 1,

        [Description("最小包装")]
        MinPackage = 2
    }
}
