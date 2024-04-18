using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.PsWms.SzMvc.Services.Enums
{
    /// <summary>
    /// 包装类型（盘点类型）
    /// </summary>
    public enum StocktakingType
    {
        /// <summary>
        /// 按个
        /// </summary>
        [Description("个")]
        Single = 1,

        /// <summary>
        /// 最小包装
        /// </summary>
        [Description("Mpq")]
        MinPackage = 2,
    }
}
