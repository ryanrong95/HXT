using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace Needs.Utils.Serializers
{
    public enum Formatting
    {
        None = 0,
        Indented = 1
    }

    /// <summary>
    /// 序列化小打
    /// </summary>
    public static class JsonSerializerExtend
    {
        static public string Json(this object obj)
        {
            return Json(obj, Formatting.None);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="converters">转换器集合</param>
        /// <returns>json</returns>
        static public string Json(this object obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting)formatting);
        }

        static public string Json(this object obj, bool isFormatting = true)
        {
            return JsonConvert.SerializeObject(obj, isFormatting ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">json内容</param>
        /// <returns>目标对象类型实例</returns>
        static public T JsonTo<T>(this string json) { return JsonConvert.DeserializeObject<T>(json); }

        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <typeparam name="type">目标对象类型</typeparam>
        /// <param name="json">json内容</param>
        /// <returns>目标对象类型实例</returns>
        static public object JsonTo(this string json, Type type) { return JsonConvert.DeserializeObject(json, type); }
        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <param name="json">json内容</param>
        /// <returns>Newtonsoft.Json.Linq.JObject</returns>
        static public JObject JsonTo(this string json) { return JsonConvert.DeserializeObject(json) as JObject; }
    }
}
