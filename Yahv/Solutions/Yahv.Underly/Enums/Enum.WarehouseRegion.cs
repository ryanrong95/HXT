using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public enum Region
    {
        /// <summary>
        /// 北京地区库房
        /// </summary>
        [Description("北京")]
        BJ = 1,
        /// <summary>
        /// 上海地区库房
        /// </summary>
        [Description("上海")]
        SH = 2,
        /// <summary>
        /// 深圳地区库房
        /// </summary>
        [Description("深圳")]
        SZ = 3,
        /// <summary>
        /// 香港地区库房
        /// </summary>
        [Description("香港")]
        HK = 4,
    }
}
