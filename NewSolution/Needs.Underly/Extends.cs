using Needs.Underly.Attributes;
using Needs.Underly.Legals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    static public class Extends
    {
        static public ICurrency GetLegal(this Currency currency)
        {
            return Legally.Current[currency];
        }

        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public ICurrency GetICurrency(this Currency e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = typeof(Currency).GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<CurrenyAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute;
        }

        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public IOrigin GetOrigin(this Origin e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = typeof(Origin).GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<OriginAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute;
        }
    }
}
