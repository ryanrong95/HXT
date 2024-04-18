using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.ExchangeRates.Extends
{
    /// <summary>
    /// 主键生成扩展方法
    /// </summary>
    public static class PKeyTypeExtend
    {
        public static string[] Pick(this PKeyType keyType, int length)
        {
            string[] serirs;
            if (length == 1)
                serirs = new string[1] { Layers.Data.PKeySigner.Pick(keyType) };
            else
                serirs = Layers.Data.PKeySigner.Series(keyType, length).OrderBy(item => item).ToArray();

            return serirs;
        }
    }
}
