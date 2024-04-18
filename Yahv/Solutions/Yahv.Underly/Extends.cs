using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// Underly 扩展类
    /// </summary>
    static public partial class Extends
    {
        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public ICurrency GetCurrency(this Currency e)
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
        static public IDistrict GetDistrict(this District e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = typeof(District).GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<DistrictAttribute>();

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
        static public IUnit GetUnit(this LegalUnit e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = typeof(LegalUnit).GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<UnitAttribute>();

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

        /// <summary>
        /// 获取枚举的多个描述
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>多个描述的数组</returns>
        static public IPackage GetPackage(this Package e)
        {
            string name = Enum.GetName(e.GetType(), e);
            MemberInfo[] mis = typeof(Package).GetMember(name);
            if (mis == null || mis.Length == 0)
            {
                throw new NotSupportedException("没有找到指定的调用成员");
            }
            if (mis.Length > 1)
            {
                throw new NotSupportedException("不支持多个成员调用");
            }

            var attribute = mis[0].GetCustomAttribute<PackageAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute;
        }

        /// <summary>
        /// 获取npc名称
        /// </summary>
        /// <param name="npc"></param>
        /// <returns></returns>
        static public string Obtain(this Npc npc)
        {
            return $"{nameof(Npc)}-{npc.ToString()}"; ;
        }

        /// <summary>
        /// 获取固定ID
        /// </summary>
        /// <param name="e">枚举值</param>
        /// <returns>固定ID</returns>
        static public string GetFixedID(this Enum e)
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

            var attribute = mis[0].GetCustomAttribute<FixedAttribute>();

            if (attribute == null)
            {
                throw new NotSupportedException("没有找到指定的特性");
            }

            return attribute.ID;

        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns></returns>
        static public decimal Round(this decimal value, int decimals = 2)
        {
            decimal result = value;

            if (value < 0)
            {
                result = Math.Abs(result);
            }

            result = Math.Round(result, decimals, MidpointRounding.AwayFromZero);

            if (value < 0)
            {
                result = 0 - result;
            }

            return result;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns></returns>
        static public decimal ToRound1(this decimal value, int decimals = 2)
        {
            decimal result = value;

            if (value < 0)
            {
                result = Math.Abs(result);
            }

            result = Math.Round(result, decimals, MidpointRounding.AwayFromZero);

            if (value < 0)
            {
                result = 0 - result;
            }

            return result;
        }

        /// <summary>
        /// 字符转换标准法定单位
        /// </summary>
        /// <param name="context">字符串</param>
        /// <returns>标准法定单位枚举</returns>
        static public LegalUnit ToLegalUnit(this string context)
        {
            context = context.Trim();
            return Enum.GetValues(typeof(LegalUnit)).Cast<LegalUnit>()
                .SingleOrDefault(item => item.GetUnit().Code == context);
        }
    }
}
