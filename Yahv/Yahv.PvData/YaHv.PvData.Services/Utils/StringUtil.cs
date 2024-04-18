using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaHv.PvData.Services.Utils
{
    public static class StringUtil
    {
        /// <summary>
        /// 特殊字符处理
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string FixSpecialChars(this string str)
        {
            return str.Replace("&quot;", "\"")
                    .Replace("&#39;", "\'")
                    .Replace("&amp;", "&")
                    .Replace("&#181;", "µ")
                    .Replace("&#252;", "ü")
                    .Trim();
        }
    }
}
