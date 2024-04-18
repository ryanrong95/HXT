using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;

namespace Yahv.PvWsClient.WebAppNew.App_Utils
{
    public static class EnumHelper
    {
        /// <summary>
        /// 根据枚举code获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        //public static int GetEnumValue<T>(string enumCode)
        //{
        //    try
        //    {
        //        var enumType = typeof(T);
        //        if (!enumType.IsEnum)
        //            throw new ArgumentException("enumType必须是枚举类型");
        //        var values = Enum.GetValues(enumType);
        //        var ht = new Hashtable();
        //        foreach (var val in values)
        //        {
        //            ht.Add(Enum.GetName(enumType, val), val);
        //        }
        //        return (int)ht[enumCode];
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}

        /// <summary>
        /// 根据枚举的Code获取枚举值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumCode"></param>
        /// <returns></returns>
        //public static T GetEnum<T>(string enumCode)
        //{
        //    try
        //    {
        //        Type t = typeof(T);
        //        var enumValue = GetEnumValue<T>(enumCode);

        //        T value = (T)Enum.ToObject(t, enumValue);

        //        return value;
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //}
    }

    /// <summary>
    /// 单位枚举
    /// </summary>
    public class UnitEnumHelper
    {
        /// <summary>
        /// 枚举转键值对
        /// </summary>
        /// <returns></returns>
        static public List<Unit> ToUnitDictionary()
        {
            Type type = typeof(LegalUnit);
            List<Unit> list = new List<Unit>();
            foreach (var item in Enum.GetValues(type))
            {
                var extends = ((LegalUnit)item).GetUnit();
                int num = (int)(LegalUnit)item;
                var unit = new Unit { Value = num, Code = extends.Code, Name = extends.Name };
                list.Add(unit);
            }
            return list;
        }
    }

    /// <summary>
    /// 单位枚举
    /// </summary>
    public class Unit
    {
        public int Value { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }
    }
}