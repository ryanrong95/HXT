using System.Web.Mvc;
using Yahv.Utils.Serializers;

namespace Yahv.Web.Mvc
{
    /// <summary>
    /// jsonp 处理结果
    /// </summary>
    public class JsonpResult : ActionResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <param name="callback">回调函数</param>
        /// <param name="contentType">内容编码</param>
        public JsonpResult(object data, string callback, string contentType = "application/json")
        {
            this.Data = data;
            this.CallbackName = callback;
            this.ContentType = contentType;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 回调函数
        /// </summary>
        public string CallbackName { get; set; }

        /// <summary>
        /// 内容编码
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 执行处理结果
        /// </summary>
        /// <param name="context">上下文</param>
        public override void ExecuteResult(ControllerContext context)
        {
            //var js = new System.Web.Script.Serialization.JavaScriptSerializer();
            //var jsonp = this.CallbackName + "(" + js.Serialize(this.Data) + ")";
            var jsonp = this.CallbackName + "(" + this.Data.Json() + ")";

            context.HttpContext.Response.ContentType = this.ContentType;
            context.HttpContext.Response.Write(jsonp);
        }
    }
}
