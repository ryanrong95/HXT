
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using System.Reflection;


namespace NtErp.Wss.Sales.Services.Utils.Structures
{
    static public class FieldNamingExtends
    {
        /// <summary>
        /// 获取枚举的标题
        /// </summary>
        /// <param name="e">枚举自对象</param>
        /// <returns>标题</returns>
        static string GetTitle(this Enum e)
        {
            string name = Enum.GetName(e.GetType(), e);

            if (name == null)
            {
                return "";
            }

            System.Reflection.MemberInfo[] mis = e.GetType().GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            Attribute[] attributes = Attribute.GetCustomAttributes(mis[0]);

            if (attributes == null || attributes.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的特性");
            }

            NamingAttribute naming = attributes.SingleOrDefault(item => item is NamingAttribute) as NamingAttribute;

            if (naming == null)
            {
                return "";
            }
            else
            {
                return naming.Name;
            }
        }

        /// <summary>
        /// 获取枚举的标题
        /// </summary>
        /// <param name="e">枚举自对象</param>
        /// <returns>标题</returns>
        public static string GetTitle(this Enum e, string separator = null)
        {
            Type type = e.GetType();
            if (type.GetCustomAttribute<FlagsAttribute>() == null)
            {
                return GetTitle(e);
            }
            else
            {
                return string.Join(separator
                    , e.ToString().Split(',').Select(item => GetTitle((Enum)Enum.Parse(type, item.Trim()))));
            }
        }

        /// <summary>
        /// 转换枚举值数组
        /// </summary>
        /// <param name="e">枚举自对象</param>
        /// <returns>值数组</returns>
        public static T[] ToValues<T>(this T e) where T : struct
        {
            if (!(e is Enum))
            {
                throw new Exception();
            }
            else
            {
                int val = Convert.ToInt32(e);
                if (val == int.MaxValue)
                {
                    return Enum.GetNames(typeof(T)).Where(item => item != "All")
                        .Select(item => (T)Enum.Parse(typeof(T), item)).ToArray();
                }
            }
            var arry = e.ToString().Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var rtn = arry.Select(item => (T)Enum.Parse(typeof(T), item)).ToArray();
            return rtn;
        }

        public static object[] GetValues(this Enum e)
        {
            string name = Enum.GetName(e.GetType(), e);

            System.Reflection.MemberInfo[] mis = e.GetType().GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            Attribute[] attributes = Attribute.GetCustomAttributes(mis[0]);

            if (attributes == null || attributes.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的特性");
            }
            if (attributes.Length > 1)
            {
                throw new NotSupportedException("不支持多个特性");
            }

            NamingAttribute titleAttr = attributes[0] as NamingAttribute;
            if (titleAttr == null)
            {
                throw new NotSupportedException("不支持[" + titleAttr.GetType().FullName + "]特性");
            }
            return titleAttr.Values;
        }

        public static T GetValue<T>(this Enum e, int index)
        {
            string name = Enum.GetName(e.GetType(), e);

            System.Reflection.MemberInfo[] mis = e.GetType().GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            Attribute[] attributes = Attribute.GetCustomAttributes(mis[0]);

            if (attributes == null || attributes.Length == 0)
            {
                throw new NotSupportedException("没有找到响应的特性");
            }
            if (attributes.Length > 1)
            {
                throw new NotSupportedException("不支持多个特性");
            }

            NamingAttribute titleAttr = attributes[0] as NamingAttribute;
            if (titleAttr == null)
            {
                throw new NotSupportedException("不支持[" + attributes[0].GetType().FullName + "]特性");
            }
            return (T)titleAttr.Values[index];
        }
    }


    /// <summary>
    /// 枚举标题
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Field)]
    public class NamingAttribute : Attribute
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 固定版本
        /// </summary>
        public double Version { get; private set; }
        /// <summary>
        /// 其他值
        /// </summary>
        public object[] Values { get; private set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="title">标题</param>
        public NamingAttribute(string title, params object[] arry)
        {
            this.Name = title;
            if (arry != null && arry.Length > 0)
            {
                this.Values = arry;
            }
            this.Version = 2.11;
        }

        public NamingAttribute() : this("Nullable")
        {
        }
    }
    /// <summary>
    /// 用于提供搜索功能的字段
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchAttribute : Attribute
    {
        /// <summary>
        /// 固定版本
        /// </summary>
        public double Version { get; private set; }

        public SearchAttribute()
        {
            this.Version = 2.01;
        }
    }
}
