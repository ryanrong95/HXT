using System.Web.Mvc;
using Yahv.Utils.Serializers;

namespace Yahv.Web.Mvc
{
    /// <summary>
    /// 访问者 控制器
    /// </summary>
    public class ClientController : Controller
    {
        /// <summary>
        /// 获取当前调用者
        /// </summary>
        static public ClientController Current
        {
            get
            {
                return System.Web.HttpContext.Current.Session["ClientController"] as ClientController;
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            System.Web.HttpContext.Current.Session["ClientController"] = this;
        }

        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected ClientController()
        {

        }

        /// <summary>
        /// eJson 
        /// </summary>
        /// <param name="data">对象</param>
        /// <returns>返回 json 字符串</returns>
        public void JsonResult(object data = null)
        {
            this.Response.ClearContent();
            this.Response.ContentType = "application/json";
            this.Response.Write((data ?? new object()).Json());
            this.Response.End();
        }

        JsonResult jsonResult;

        /// <summary>
        /// eJson 
        /// </summary>
        /// <param name="data">对象</param>
        /// <returns>返回 json 字符串</returns>
        public JsonResult eJson(object data = null)
        {
            if (data == null)
            {
                return this.jsonResult;
            }

            return this.jsonResult = this.Json(data) ?? this.jsonResult;
        }


        /// <summary>
        /// Jsonp
        /// </summary>
        /// <param name="data">返回数据</param>
        /// <param name="callback">回调函数</param>
        /// <param name="contentType">内容编码</param>
        /// <returns></returns>
        public JsonpResult Jsonp(object data, string callback, string contentType = "application/json")
        {
            return new JsonpResult(data, callback, contentType);
        }

        ///// <summary>
        ///// 接口访问是否授权
        ///// </summary>
        ///// <returns>appkey 匹配返回 ture，否则返回 false</returns>
        //public bool ApiAuthorized()
        //{
        //    var appkeys = System.Configuration.ConfigurationManager.AppSettings["appkey"].Split(',');
        //    foreach (var key in appkeys)
        //    {
        //        if (this.Request["appkey"] == key)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
    }
}
