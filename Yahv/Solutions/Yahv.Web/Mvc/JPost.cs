using Newtonsoft.Json.Linq;
using System.Text;
using System.Web;

namespace Yahv.Web.Mvc
{
    /// <summary>
    /// Payload参数对象
    /// </summary>
    public class JPost
    {
        /// <summary>
        /// JObject
        /// </summary>
        protected JObject JObject { get; private set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        public JPost()
        {
            var request = HttpContext.Current.Request;
            var bytes = new byte[request.InputStream.Length];
            request.InputStream.Position = 0;
            request.InputStream.Read(bytes, 0, bytes.Length);
            string context = Encoding.UTF8.GetString(bytes);
            this.JObject = JObject.Parse(context);
        }


        /// <summary>
        /// 获取 JToken
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>JToken</returns>
        public JToken this[string index]
        {
            get { return this.JObject[index]; }
            set { this.JObject[index] = value; }
        }

        /// <summary>
        /// 对象转化
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>转化类型实例</returns>
        public T ToObject<T>()
        {
            return this.JObject.ToObject<T>();
        }

        /// <summary>
        /// 隐式转换
        /// </summary>
        /// <param name="v">来源数据</param>
        public static implicit operator JObject(JPost v)
        {
            //以一个地址+结构的方式访问一段内存
            return v.JObject;
        }
    }
}
