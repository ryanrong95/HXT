using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahv.Utils.Converters.Contents
{
    /// <summary>
    /// 内容扩展
    /// </summary>
    public static class ContentExtend
    {
        const string vector2 = "^yj201￥—5PK_&(:<\"—>[&`1256~*)%:<\"`1256~dc—）M:?}{`:3\r6~**?}a—a\":<\"`1201￥556~*!@::>[*&$%*()&!#@^M&801￥59\t:a9\t:a:$(\"\n}&89\t:a9\t:a*?}{`03\r6~**?}{`0!2:>[&89\t:a9\t:a￥57!2:>[&4}:a)$";
        const string vector1 = "^*()&!#@^M&801￥59\t:a9\t:a:$(\"\n}&89\t:a9\t:a*?}{`03\r6~**?}{`0!2:>[&89\t:a9\t:a￥57!2:>[&4}:a)yj201￥—5PK_&(:<\"—>[&`1256~*)%:<\"`1256~dc—）M:?}{`:3\r6~**?}a—a\":<\"`1201￥556~*!@::>[*&$%";

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

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="context">内容</param>
        /// <param name="encoding">编码</param>
        /// <param name="sign">标志</param>
        /// <returns>MD5摘要值</returns>
        static public string MD5(this string context, Encoding encoding, string sign = "X")
        {
            if (string.IsNullOrEmpty(context))
                return "";
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] b = encoding.GetBytes(context);
                b = md5.ComputeHash(b);
                StringBuilder ret = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                    ret.Append(b[i].ToString(sign).PadLeft(2, '0'));
                return ret.ToString();
            }
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="arry">内容数组</param>
        /// <param name="sign">标志</param>
        /// <returns>MD5摘要值</returns>
        static public string MD5(this string[] arry, string sign = "X")
        {
            return MD5((IEnumerable<string>)arry, sign);
        }

        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="arry">内容数组</param>
        /// <param name="sign">标志</param>
        /// <returns>MD5摘要值</returns>
        static public string MD5(this IEnumerable<string> arry, string sign = "X")
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


        const string PKV = "^yj201￥5PK_&(*)%dc——）M::aa\"as\"s\t\a\\\r\n\"NQSC&$%";

        /// <summary>
        /// 老 密码转换
        /// </summary>
        /// <param name="txt">源字符串</param>
        /// <returns>转换后的密码</returns>
        static public string PasswordOld(this string txt)
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


        /// <summary>
        /// 一期MD5加密，为了一期的账号密码迁移到二期可以使用
        /// 华芯通MD5加密方法，为了账号密码通用使用
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StrToMD5(this string str)
        {
            //MD5加密
            var md5 = System.Security.Cryptography.MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //二次加密
            var yxjmd5 = "YuanDa:" + sb.ToString();
            //重新赋值 
            sb = new StringBuilder();
            bs = md5.ComputeHash(Encoding.UTF8.GetBytes(yxjmd5));
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            //所有字符转为大写
            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 转换为Url格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrl(this string str)
        {
            return str.Replace(@"\", @"/");
        }
    }
}
