using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    public static class ExtendsApiAddress
    {
        /// <summary>
        /// 获取api地址
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举描述</returns>
        static public string GetApiAddress(this Enum e)
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

            var attribute = mis[0].GetCustomAttribute<ApiAddressAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute.GetAddress();
        }

        /// <summary>
        /// 获取api host
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>枚举描述</returns>
        static public string GetHostName(this Enum e)
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

            var attribute = mis[0].GetCustomAttribute<ApiAddressAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute.GetHostName();
        }
    }
}