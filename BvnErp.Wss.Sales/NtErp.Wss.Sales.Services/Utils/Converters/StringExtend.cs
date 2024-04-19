using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Utils.Converters
{
    /// <summary>
    /// 字符串扩展
    /// </summary>
    public static class StringExtend
    {
        const string vector2 = "^yj201￥—5PK_&(:<\"—>[&`1256~*)%:<\"`1256~dc—）M:?}{`:3\r6~**?}a—a\":<\"`1201￥556~*!@::>[*&$%*()&!#@^M&801￥59\t:a9\t:a:$(\"\n}&89\t:a9\t:a*?}{`03\r6~**?}{`0!2:>[&89\t:a9\t:a￥57!2:>[&4}:a)$";
        const string vector1 = "^*()&!#@^M&801￥59\t:a9\t:a:$(\"\n}&89\t:a9\t:a*?}{`03\r6~**?}{`0!2:>[&89\t:a9\t:a￥57!2:>[&4}:a)yj201￥—5PK_&(:<\"—>[&`1256~*)%:<\"`1256~dc—）M:?}{`:3\r6~**?}a—a\":<\"`1201￥556~*!@::>[*&$%";

        static public string MD5(this string txt, string sign = "X")
        {
            if (string.IsNullOrEmpty(txt))
                return "";
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] b = Encoding.Default.GetBytes(txt);
                b = md5.ComputeHash(b);
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                    ret.Append(b[i].ToString(sign).PadLeft(2, '0'));
                return ret.ToString();
            }
        }

        static public string MD5(this string[] arry, string sign = "X")
        {
            var txt = string.Join("", arry);
            if (string.IsNullOrEmpty(txt))
                return "";
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] b = Encoding.Default.GetBytes(txt);
                b = md5.ComputeHash(b);
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                    ret.Append(b[i].ToString(sign).PadLeft(2, '0'));
                return ret.ToString();
            }
        }

        static public string MD516(this string txt)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                string rtn = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(txt)), 4, 8);
                rtn = rtn.Replace("-", "");
                return rtn;
            }
        }

        /// <summary>
        /// 密码转换
        /// </summary>
        /// <param name="txt">源字符串</param>
        /// <returns>转换后的密码</returns>
        static public string Password(this string txt)
        {
            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < txt.Length; index++)
            {
                //素数
                builder.Append(vector1[vector1.Length % (txt[index] / 7)]);
                builder.Append(txt[index]);
                builder.Append(vector1[vector1.Length % (txt[index] / 3)]);
            }

            return builder.ToString().MD5();
        }

        public const string PKV = "^yj201￥5PK_&(*)%dc——）M::aa\"as\"s\t\a\\\r\n\"NQSC&$%";

        static public string PasswodOld(this string txt)
        {
            return string.Concat(txt, PKV).MD5("x");
        }

        /// <summary>
        /// Token转换
        /// </summary>
        /// <param name="txt">源字符串</param>
        /// <returns>转换后的Token</returns>
        static public string Token(this string txt)
        {
            StringBuilder builder = new StringBuilder();

            for (int index = 0; index < txt.Length; index++)
            {
                builder.Append(vector2[vector2.Length % (txt[index] / 7)]);
                builder.Append(txt[index]);
                builder.Append(vector2[vector2.Length % (txt[index] / 3)]);
            }

            return builder.ToString().MD5();
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <returns>首字母小写字符内容</returns>
        static public string FirstLower(this string txt)
        {
            return ForLower(txt, 0);
        }

        /// <summary>
        /// 第二字母小写
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <returns>第二字母小写字符内容</returns>
        static public string SecondLower(this string txt)
        {
            return ForLower(txt, 1);
        }

        /// <summary>
        /// 第二字母小写
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <returns>第二字母小写字符内容</returns>
        static public string JinterfaceLower(this string txt)
        {
            return SecondLower(txt);
        }

        /// <summary>
        /// 首字母小写
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <returns>首字母小写字符内容</returns>
        static public string ForLower(this string txt, int index)
        {
            if (string.IsNullOrWhiteSpace(txt))
            {
                return txt;
            }

            if (txt.Length == 1)
            {
                return txt.ToLower();
            }

            var select = txt[index];

            if (char.IsLetter(select) && !char.IsLower(select))
            {
                var arry = txt.ToCharArray();
                arry[index] = char.ToLower(select);
                return new string(arry);
            }
            else
            {
                return txt;
            }
        }


        static Regex regexStandardSpace = new Regex(@"\s+", RegexOptions.Singleline);

        /// <summary>
        /// 空格标准化
        /// </summary>
        /// <param name="txt">字符内容</param>
        /// <returns>空格标准化内容</returns>
        static public string StandardSpace(this string txt)
        {
            return regexStandardSpace.Replace(txt, " ");
        }

        public static string FirstUpper(this string str)
        {
            var arry = str.ToArray();
            arry[0] = char.ToUpper(arry[0]);
            return new string(arry);
        }
    }
}
