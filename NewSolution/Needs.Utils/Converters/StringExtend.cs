using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Needs.Utils.Converters
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

        /// <summary>
        /// 转换为Url格式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrl(this string str)
        {
            return str.Replace(@"\", @"/");
        }

        /// <summary>
        /// 一期MD5加密，为了一期的账号密码迁移到二期可以使用
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
        /// 移除特殊字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="characters"></param>
        /// <returns></returns>
        public static string RemoveChars(this string str, string[] characters)
        {
            foreach (var c in characters)
            {
                str = str.Replace(c, "");
            }
            return str;
        }

        static public Dictionary<string, object> GetQueryDictionary(this object param)
        {
            var values = param.GetType().GetProperties().Select(item =>
            {
                var value = item.GetValue(param);
                if (value != null)
                {
                    if (value.GetType().IsEnum)
                    {
                        return new
                        {
                            item.Name,
                            Value = Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType())
                        };
                    }
                }

                return new
                {
                    item.Name,
                    Value = value
                };

            }).Where(item => item.Value != null);

            return values.ToDictionary(item => item.Name, item => item.Value);
        }

        static public string GetQueryParams(this Dictionary<string, object> param)
        {
            return string.Join("&", param.Select(item =>
            {
                var value = item.Value;
                if (value == null)
                {
                    return null;
                }
                else
                {
                    if (value.GetType().IsEnum)
                    {
                        return $"{item.Key}={Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType())}";
                    }
                    return $"{item.Key}={value}";
                }
            }));
        }

        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="fullAngle"></param>
        /// <returns></returns>
        static public string ToHalfAngle(this string fullAngle)
        {
            char[] c = fullAngle.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }
    }
}
