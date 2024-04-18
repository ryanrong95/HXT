using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Descriptions
{
    static public class Extends
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举描述</returns>
        static public string GetDescription(this Enum e)
        {
            return GetDescriptions(e)[0];
        }

        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public string[] GetDescriptions(this Enum e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = e.GetType().GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<DescriptionAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute.Contexts;
        }

        /// <summary>
        /// 获取复合枚举的Flag类型枚举的值
        /// </summary>
        /// <param name="e"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        static public string GetFlagsDescriptions(this Enum e, string separator)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Enum.GetValues(e.GetType()).Cast<Enum>())
            {
                var value = item.GetDescription();
                if ((e.GetHashCode() & item.GetHashCode()) > 0)
                {
                    stringBuilder.Append(value);
                    stringBuilder.Append(separator);
                }
            }

            return stringBuilder.ToString().TrimEnd(separator.ToCharArray());
        }

        /// <summary>
        /// 获取Enums的Flag类型枚举的值
        /// </summary>
        /// <param name="es"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        static public string GetEnumsDescriptions(this Enum[] enums, string separator)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in enums)
            {
                var value = item.GetDescription();
                stringBuilder.Append(value);
                stringBuilder.Append(separator);
            }

            return stringBuilder.ToString().TrimEnd(separator.ToCharArray());
        }
    }
}