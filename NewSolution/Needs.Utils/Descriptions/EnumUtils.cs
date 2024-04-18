using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Utils.Descriptions
{
    /// <summary>
    /// 枚举工具类
    /// </summary>
    public class EnumUtils
    {
        /// <summary>
        /// 获取键值对列表
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对列表</returns>
        static public List<KeyValuePair<string, string>> ToKeyValuePair<T>()
        {
            Type type = typeof(T);
            return new List<KeyValuePair<string, string>>(Enum.GetValues(type).Cast<Enum>().Select(item =>
            {
                var key = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)).ToString();
                var value = item.GetDescription();
                return new KeyValuePair<string, string>(key, value);
            }));
        }

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
                var key = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)).ToString();
                var value = item.GetDescription();
                dic.Add(key, value);
            }
            return dic;
        }
  
        /// <summary>
        /// 获取键值名称对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值名称对字典</returns>
        static public Dictionary<T, string> ToEnumNameDictionary<T>(Func<string, bool> predicate = null) where T : struct
        {
            Type type = typeof(T);
            Dictionary<T, string> dic = new Dictionary<T, string>();

            foreach (var item in Enum.GetValues(type).Cast<T>())
            {
                var key = item;
                var value = Enum.GetName(type, item);
                if (predicate != null && predicate(value))
                {
                    dic.Add(key, value);
                }
            }

            return dic;
        }
    }
}
