using System;
using System.Collections.Generic;
using System.Linq;

namespace Yahv.Underly
{
    /// <summary>
    /// 名值对
    /// </summary>
    public class NameValuePair
    {
        /// <summary>
        /// 名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 枚举工具类
    /// </summary>
    public class ExtendsEnum
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
                var key = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type)).ToString();
                var value = item.GetDescription();
                dic[key] = value;
                //dic.Add(key, value);
            }
            return dic;
        }

        /// <summary>
        /// 获取键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public NameValuePair[] ToNameDictionary<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<Enum>().Select(item => new NameValuePair
            {
                Name = item.GetDescription(),
                Value = item.ToString()
            }).ToArray();
        }

        /// <summary>
        /// 获取键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <param name="excepts">要除去的项</param>
        /// <returns>键值对字典</returns>
        static public T[] ToArray<T>(params T[] excepts)
        {
            var type = typeof(T);
            var query = Enum.GetValues(type).Cast<Enum>().Select(item => (T)Enum.ToObject(type, item));

            if (excepts.Length != 0)
            {
                query = query.Where(item => !(excepts.Contains(item)));
            }

            return query.ToArray();
        }

        /// <summary>
        /// 获取键值对字典
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>键值对字典</returns>
        static public Dictionary<T, string> ToEnumDictionary<T>()
        {
            Type type = typeof(T);
            Dictionary<T, string> dic = new Dictionary<T, string>();
            foreach (var item in Enum.GetValues(type).Cast<Enum>())
            {
                var value = item.GetDescription();
                dic.Add((T)Enum.ToObject(type, item), value);
            }
            return dic;
        }

        /// <summary>
        /// 枚举返回前端接口结构
        /// </summary>
        public class Formation
        {
            /// <summary>
            /// 描述信息
            /// </summary>
            public string Description { get; internal set; }

            /// <summary>
            /// 枚举名称
            /// </summary>
            public string Name { get; internal set; }

            /// <summary>
            /// 枚举值
            /// </summary>
            public object Value { get; internal set; }
        }

        /// <summary>
        /// 枚举返回前端接口结构
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns>前端接口结构</returns>
        static public Formation[] ForFrontEnd<T>()
        {
            Type type = typeof(T);
            return Enum.GetValues(type).Cast<Enum>().Select(item => new Formation
            {
                Description = item.GetDescription(),
                Name = item.ToString(),
                Value = Convert.ChangeType(Enum.ToObject(type, item), Enum.GetUnderlyingType(type))
            }).ToArray();

        }
    }
}
