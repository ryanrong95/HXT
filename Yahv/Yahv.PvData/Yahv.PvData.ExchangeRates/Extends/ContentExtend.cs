using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.PvData.ExchangeRates.Extends
{
    public static class ContentExtend
    {
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="context">内容</param>
        /// <param name="sign">标志</param>
        /// <returns>MD5摘要值</returns>
        static public string MD5(this string context, string sign = "X")
        {
            if (string.IsNullOrEmpty(context))
                return "";
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] b = Encoding.Default.GetBytes(context);
                b = md5.ComputeHash(b);
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                    ret.Append(b[i].ToString(sign).PadLeft(2, '0'));
                return ret.ToString();
            }
        }
    }
}
