
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsPortal2.MvcApp.App_Utils
{
    public static class DecimalExtends
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