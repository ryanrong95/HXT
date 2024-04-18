using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 香港交货方式
    /// </summary>
    public enum CompanyTypeEnums
    {
        /// <summary>
        /// 内单
        /// </summary>
        [Description("A类")]
        Inside = 1,

        /// <summary>
        /// Icgoo等公司
        /// </summary>
        [Description("Icgoo")]
        Icgoo = 2,

        /// <summary>
        /// 外单
        /// </summary>
        [Description("B类")]
        OutSide = 3,

        /// <summary>
        /// Icgoo等公司
        /// </summary>
        [Description("快包")]
        FastBuy = 4,
    }
}