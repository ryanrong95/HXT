using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HKWarehouseApi.Controllers
{
    public class MyApiController : System.Web.Mvc.Controller
    {
        /// <summary>
        /// 受保护的构造器
        /// </summary>
        protected MyApiController()
        {

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
        public Models.JsonpResult Jsonp(object data, string callback, string contentType = "application/json")
        {
            return new Models.JsonpResult(data, callback, contentType);
        }
    }
}