using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    //[EnumNaming("地区规范")]
    public enum FixedArea
    {
        #region Area
        /// <summary>
        /// 中国
        /// </summary>
        [Fixed("FArea01")]
        [Description("国内")]
        MainLand = 1,
        /// <summary>
        /// 国外
        /// </summary>
        [Fixed("FArea02")]
        [Description("国外")]
        Oversea = 2,
        /// <summary>
        /// 香港
        /// </summary>
        [Fixed("FArea03")]
        [Description("香港")]
        HongKong = 3

        #endregion
    }
}
