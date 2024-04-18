using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Needs.Ccs.Services
{
    static public class Extends
    {
        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public AttributeItems GetDescriptions(this Enum e)
        {
            string name = Enum.GetName(e.GetType(), e);
            System.Reflection.MemberInfo[] mis = e.GetType().GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<MultiDescriptionAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute.Contexts;
        }
            
        public static  string GetXmlEnumAttributeValueFromEnum<TEnum>(this TEnum value) where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);
            if (!enumType.IsEnum) return null;//or string.Empty, or throw exception

            var member = enumType.GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return null;//or string.Empty, or throw exception

            var attribute = member.GetCustomAttributes(false).OfType<System.Xml.Serialization.XmlEnumAttribute>().FirstOrDefault();
            if (attribute == null) return null;//or string.Empty, or throw exception
            return attribute.Name;
        }
    }

    static public class MultiEnumUtils
    {
        /// <summary>
        /// 获取键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public Dictionary<string, string> ToDictionary<T>()
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var value = item.GetDescriptions();
                dic.Add(value.Code, value.Text);
            }
            return dic;
        }

        /// <summary>
        /// 获取枚举的代码值
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public string ToCode<T>(Enum enums)
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var value = item.GetDescriptions();
                if (enums.GetHashCode() == item.GetHashCode())
                {
                    return value.Code;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取枚举的文本
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public string ToText<T>(Enum enums)
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var value = item.GetDescriptions();
                if (enums.GetHashCode() == item.GetHashCode())
                {
                    return value.Text;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取枚举的文本
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public string ToText<T>(string enums)
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var value = item.GetDescriptions();
                if (enums == value.Code)
                {
                    return value.Text;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public Dictionary<string, string> ToDictionaryStr<T>()
        {
            Type type = typeof(T);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var key = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)).ToString();
                var value = item.ToString();
                dic.Add(key, value);
            }
            return dic;
        }
    }
}