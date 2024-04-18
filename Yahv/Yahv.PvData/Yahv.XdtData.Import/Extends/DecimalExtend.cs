using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.XdtData.Import.Extends
{
    public static class DecimalExtend
    {
        /// <summary>
        /// 四舍五入  IEEE规范
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="length">保留位数</param>
        /// <returns></returns>
        public static decimal ToRound(this decimal value, int length)
        {
            return Math.Round((decimal)value, length, MidpointRounding.AwayFromZero);
        }
    }
}
