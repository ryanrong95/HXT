using Yahv.Utils.Serializers;
using System.Web;

namespace Yahv.Utils.Http
{
    /// <summary>
    /// Http 通用扩展类
    /// </summary>
    static public class HttpExtends
    {
        /// <summary>
        /// Json返回
        /// </summary>
        /// <param name="response">当前 ASP.NET 操作的 HTTP 响应信息</param>
        /// <param name="data">数据对象</param>
        /// <param name="isClearContent">是否清理当前响应内容</param>
        static public void Json(this HttpResponse response, object data, bool isClearContent = true)
        {
            if (isClearContent)
            {
                response.ClearContent();
            }

            response.ContentType = "application/json";
            response.Write(data.Json());
        }

        /// <summary>
        /// Json返回
        /// </summary>
        /// <param name="response">当前 ASP.NET 操作的 HTTP 响应信息</param>
        /// <param name="data">数据对象</param>
        /// <param name="isClearContent">是否清理当前响应内容</param>
        static public void Content(this HttpResponse response, string data, bool isClearContent = true)
        {
            if (isClearContent)
            {
                response.ClearContent();
            }
            response.ContentType = "text/plain";
            response.Write(data.Json());
        }
    }
}
