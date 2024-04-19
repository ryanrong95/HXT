using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;


namespace NtErp.Wss.Sales.Services.Underly.Serializers
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
        public static string Json(this object obj)
        {
            return Json(obj, Formatting.None);
        }

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <param name="converters">转换器集合</param>
        /// <returns>json</returns>
        public static string Json(this object obj, Formatting formatting = Formatting.None)
        {
            return JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting)formatting, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }

        public static string Json(this object obj, bool isFormatting = true)
        {
            return JsonConvert.SerializeObject(obj, isFormatting ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss"
            });
        }

        /// <summary>
        /// json 反序列化
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="json">json内容</param>
        /// <returns>目标对象类型实例</returns>
        public static T JsonTo<T>(this string json) { return JsonConvert.DeserializeObject<T>(json); }
    }
}
