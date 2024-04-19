using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Utils
{
    /// <summary>
    /// 小数点计算
    /// </summary>
    static public class DecimalHelper
    {

        /// <summary>
        /// 获取陈列价
        /// 取4位小数,小数点4位后非0进1
        /// </summary>
        /// <param name="val">实际价格</param>
        /// <returns>陈列价</returns>
        public static decimal Fourh(this decimal val)
        {
            return Fifh(val, 4);
        }
        /// <summary>
        /// 五入四不舍
        /// </summary>
        /// <param name="val">计算值</param>
        /// <returns>五入四不舍值</returns>
        static decimal Fifh(this decimal val, int prec)
        {
            if (val == 0M)
            {
                return 0M;
            }

            bool isMinus = val < 0M;
            val = Math.Abs(val);

            decimal augend = (decimal)Math.Pow(10, -(prec + 1)) * 4M;
            var rtn = val + augend;
            rtn = Math.Round(rtn, prec, MidpointRounding.AwayFromZero);
            return isMinus ? -rtn : rtn;
        }

    }
}
