using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;


namespace Yahv.Erm.Fingerprints.Services
{
    /// <summary>
    /// json格式化方式
    /// </summary>
    public enum Formatting
    {
        /// <summary>
        /// 不格式化
        /// </summary>
        None = 0,
        /// <summary>
        /// 缩进
        /// </summary>
        Indented = 1
    }

    /// <summary>
    /// 序列化小打
    /// </summary>
    public static class JsonSerializerExtend
    {
        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <returns>不格式化的对象</returns>
        static public string Json(this object obj)
        {
            return Json(obj, Formatting.None);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="formatting">转换器集合</param>
        /// <returns>json</returns>
        static public string Json(this object obj, Formatting formatting = Formatting.None)
        {
            var format = (Newtonsoft.Json.Formatting)formatting;
            if (obj == null)
            {
                return JValue.CreateNull().ToString(format);
            }

            if (obj is JObject)
            {
                return ((JObject)obj).ToString(format);
            }
            return JsonConvert.SerializeObject(obj, format);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="isFormatting">是否格式化</param>
        /// <returns>json</returns>
        static public string Json(this object obj, bool isFormatting = true)
        {
            var format = isFormatting ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None;
            if (obj == null)
            {
                return JValue.CreateNull().ToString(format);
            }
            if (obj is JObject)
            {
                return ((JObject)obj).ToString(format);
            }
            return JsonConvert.SerializeObject(obj, format);
        }

        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">json内容</param>
        /// <returns>目标对象类型实例</returns>
        static public T JsonTo<T>(this string json) 
        {
            if (typeof(T) == typeof(string))
            {
                return (T)(object)json;
            }

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <param name="type">目标对象类型</param>
        /// <param name="json">json内容</param>
        /// <returns>目标对象类型实例</returns>
        static public object JsonTo(this string json, Type type)
        {
            if (type == typeof(string))
            {
                return json;
            }

            if (type == typeof(JObject))
            {
                return JObject.Parse(json);
            }

            return JsonConvert.DeserializeObject(json, type);
        }
        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <param name="json">json内容</param>
        /// <returns>Newtonsoft.Json.Linq.JObject</returns>
        static public JObject JsonTo(this string json) { return JsonConvert.DeserializeObject(json) as JObject; }

        ///// <summary>
        ///// json 序列化
        ///// </summary>
        ///// <param name="obj">对象</param>
        ///// <returns>Newtonsoft.Json.Linq.JObject</returns>
        //static public JObject JsonTo(this object obj) { return obj.Json().JsonTo(); }


        ///// <summary>
        ///// json 序列化
        ///// </summary>
        ///// <param name="obj">对象</param>
        ///// <returns>Newtonsoft.Json.Linq.JArray</returns>
        //static public JArray JArrayTo(this object obj) { return JsonConvert.DeserializeObject(obj.Json()) as JArray; }
    }
}
