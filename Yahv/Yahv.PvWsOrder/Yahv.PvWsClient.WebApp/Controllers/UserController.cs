using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebApp.Controllers
{
    public class UserController: Controller
    {
        public UserController()
        {
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ViewBag.UserName = Client.Current==null?"": Client.Current.UserName;
        }

        public JsonResult JsonResult(VueMsgType type, string message)
        {
            return Json(new { type = type.ToString(), msg = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResult(VueMsgType type, string message, string data)
        {
            return Json(new { type = type.ToString(), msg = message, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Paging<T>(IEnumerable<T> queryable, int total, Func<T, object> converter = null)
        {
            if (converter == null)
            {
                return JsonResult(VueMsgType.success, "", new { list = queryable.ToArray(), total }.Json());
            }
            else
            {
                return JsonResult(VueMsgType.success, "", new { list = queryable.Select(converter).ToArray(), total }.Json());
            }
        }

        public JsonResult Paging<T>(IEnumerable<T> queryable, Func<T, object> converter = null)
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            int total = queryable.Count();
            var query = queryable.Skip(rows * (page - 1)).Take(rows);

            if (converter == null)
            {
                return JsonResult(VueMsgType.success, "", new { list = query.ToArray(), total }.Json());
            }
            else
            {
                return JsonResult(VueMsgType.success, "", new { list = query.Select(converter).ToArray(), total }.Json());
            }
        }

        /// <summary>
        /// 操作日志
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="operation">操作内容</param>
        /// <param name="summary">备注</param>
        public void OperationLog(string id,string operation,string summary="")
        {
            Client.Current.OrderOperateLog.Log(new Services.Models.OperatingLog
            {
                Operation = operation,
                MainID =id,
                Summary=summary
            });
        }

        /// <summary>
        /// 错误操作日志
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="operation">操作内容</param>
        /// <param name="summary">备注</param>
        public void ErrorOperationLog(string id, string operation, string summary = "")
        {
            Client.Current.Errorlog.Log(new Services.Models.OperatingLog
            {
                Operation = operation,
                MainID = id,
                Summary = summary
            });
        }
    }
}