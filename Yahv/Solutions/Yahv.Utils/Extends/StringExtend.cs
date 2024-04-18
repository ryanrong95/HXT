using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Utils.Extends
{
    public static class StringExtend
    {
        public static string Base64Encode(this string str)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(str));
        }

        public static string Base64Decode(this string str)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(str));
        }

        static public string GetQueryParams(this object param)
        {
            var values = param.GetType().GetProperties().Select(item =>
            {
                var value = item.GetValue(param);
                if (value == null)
                {
                    return null;
                }
                else
                {
                    if (value.GetType().IsEnum)
                    {
                        return $"{item.Name}={Convert.ChangeType(value, value.GetType().GetEnumUnderlyingType())}";
                    }
                    if (item.Name == "Data")
                    {
                        return string.Join("&", value.GetType().GetProperties().Select(tem =>
                        {
                            return $"{tem.Name}={tem.GetValue(value)}";
                        }));
                    }

                    return $"{item.Name}={value}";
                }
            }).Where(item => item != null);

            return string.Join("&", values);
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
                    if (item.Key == "Data")
                    {
                        return string.Join("&", value.GetType().GetProperties().Select(tem =>
                        {
                            return $"{tem.Name}={tem.GetValue(value)}";
                        }));
                    }

                    return $"{item.Key}={value}";
                }
            }));
        }

        ///任意字符串
        ///全角字符串
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        ///<summary>
        /// 快递鸟转全角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ToKdnFullAngle(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            int[] number = { 37, 38, 40 , 41, 43, 60, 62 };// % & () + < >这七个字符的特殊处理

            for (int i = 0; i < c.Length; i++)
            {
                if (number.Contains(c[i]))
                {
                    c[i] = (char)(c[i] + 65248);
                }
            }
            return new string(c);
        }

        ///任意字符串
        ///全角字符串
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248 
        ///<summary>
        /// 快递鸟转半角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ToKdnHalfAngle(this string input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            int[] number = { 65285, 65286, 65288, 65289, 65291, 65308, 65310 };

            for (int i = 0; i < c.Length; i++)
            {
                if (number.Contains(c[i]))
                {
                    c[i] = (char)(c[i] - 65248);
                }
            }
            return new string(c);
        }


        /// <summary>
        /// 半角转全角符号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ToFullAngle(this string input)
        {
            // 半角转全角：  
            char[] array = input.ToCharArray();
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == 32)
                {
                    array[i] = (char)12288;
                    continue;
                }
                if (array[i] < 127)
                {
                    array[i] = (char)(array[i] + 65248);
                }
            }
            return new string(array);
        }

        /// <summary>
        /// 转半角的函数
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        static public string ToHalfAngle(this string input)
        {
            char[] c = input.ToCharArray();
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

        public static bool IsNullOrEmpty(this string input)
        {
            return string.IsNullOrEmpty(input);
        }

        public static bool IsNullOrWhiteSpace(this string input)
        {
            return string.IsNullOrWhiteSpace(input);
        }

        public static string[] Split(this string input, string separator, StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries)
        {
            return input.Split(new[] { separator }, options);
        }
    }
}
