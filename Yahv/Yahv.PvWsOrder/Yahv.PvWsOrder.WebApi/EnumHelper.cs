using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Yahv.PvWsOrder.WebApi
{
    public static class EnumHelper
    {
        /// <summary>
        /// 根据枚举的名称获取值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="enumName"></param>
        /// <returns></returns>
        public static int GetEnumValue(Type enumType, string enumName)
        {
            try
            {
                if (!enumType.IsEnum)
                    throw new ArgumentException("enumType必须是枚举类型");
                var values = Enum.GetValues(enumType);
                var ht = new Hashtable();
                foreach (var val in values)
                {
                    ht.Add(Enum.GetName(enumType, val), val);
                }
                return (int)ht[enumName];
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}