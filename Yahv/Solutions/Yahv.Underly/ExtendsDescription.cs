using Yahv.Underly.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Yahv.Underly
{
    /// <summary>
    /// Yahv.Underly 扩展 Description
    /// </summary>
    static public partial class ExtendsDescription
    {
        /// <summary>
        /// 获取枚举描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举描述</returns>
        static public string GetDescription(this Enum e)
        {
            Type type = e.GetType();
            string name = Enum.GetName(type, e);

            FlagsAttribute flags = type.GetCustomAttribute<FlagsAttribute>();

            if (flags != null || string.IsNullOrWhiteSpace(name))
            {
                List<string> list = new List<string>();
                var ienums = Enum.GetValues(type).Cast<Enum>();
                foreach (var item in ienums)
                {
                    if (e.HasFlag(item))
                    {
                        list.Add(GetDescriptions(item)[0]);
                    }
                }

                if (list.Count > 1 && (int)Enum.GetValues(type).GetValue(0) == 0)
                {
                    list.RemoveAt(0);
                }

                return string.Join(",", list);
            }
            else
            {
                return GetDescriptions(e)[0];
            }
        }

        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public string[] GetDescriptions(this Enum e)
        {
            Type type = e.GetType();
            string name = Enum.GetName(type, e);
            MemberInfo[] mis = type.GetMember(name);
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
        /// 获取属性描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举描述</returns>
        static public string GetDescription(this PropertyInfo e)
        {
            return GetDescriptions(e)[0];
        }

        /// <summary>
        /// 获取属性的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public string[] GetDescriptions(this PropertyInfo e)
        {
            var attribute = e.GetCustomAttribute<DescriptionAttribute>();

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
        static public string GetFlagsDescriptions(this Enum e, string separator = ",")
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
        /// 获取符合枚举值数组
        /// </summary>
        /// <typeparam name="T">枚举的返回类型</typeparam>
        /// <param name="e">指定的枚举</param>
        /// <returns>符合枚举值数组</returns>
        static public T[] GetFlagsValues<T>(this Enum e)
        {
            return Enum.GetValues(e.GetType()).Cast<Enum>().Where(item =>
            {
                return e.HasFlag(item);
            }).Cast<T>().ToArray();
        }

        /// <summary>
        /// 获取符合枚举值数组
        /// </summary>
        /// <typeparam name="T">枚举的返回类型</typeparam>
        /// <param name="e">指定的枚举</param>
        /// <param name="isIncludeZero">当结果数量多于1个值的时候，是否包涵0值数据</param>
        /// <returns>符合枚举值数组</returns>
        static public T[] GetFlagsValues<T>(this Enum e, bool isIncludeZero)
        {
            var items = Enum.GetValues(e.GetType()).Cast<object>().Where(item =>
            {
                return e.HasFlag((Enum)item);
            });
            if (!isIncludeZero && items.Count() > 1)
            {
                items = items.Where(item => (int)item != 0);
            }
            return items.Cast<T>().ToArray();
        }


        /// <summary>
        /// 返回包涵的Flag组合
        /// </summary>
        /// <typeparam name="T">枚举值</typeparam>
        /// <param name="evalue"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        static public T[] GetHasFlag<T>(this T evalue, params T[] excepts) where T : struct, IComparable, IFormattable, IConvertible
        {
            Type type = typeof(T);

            if (!type.IsEnum || type.GetCustomAttribute<FlagsAttribute>() == null)
            {
                return new[] { evalue };
            }


            var name_excepts = excepts.Select(item => item.ToString());

            var values = Enum.GetNames(type).Where(item => !name_excepts.Contains(item)).ToArray();

            var subsets = from m in Enumerable.Range(0, 1 << values.Length)
                          select (from i in Enumerable.Range(0, values.Length)
                                  where (m & (1 << i)) != 0
                                  select values[i]).ToArray();

            var value = evalue.ToString();

            var results = subsets.Select(item => string.Join(",", item))
                .Where(item => !string.IsNullOrWhiteSpace(item) && item.Contains(value)).ToArray();


            return results.Select(item => Enum.Parse(type, item)).Cast<T>().Distinct().ToArray();
        }
    }
}
