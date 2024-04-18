using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Needs.Wl.Web.WeChat
{
    public class UserController : Controller
    {
        public UserController()
        {

        }

        public JsonResult JsonResult(VueMsgType type, string message)
        {
            return Json(new { type = type.ToString(), msg = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResult(VueMsgType type, string message, string data)
        {
            return Json(new { type = type.ToString(), msg = message, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Paging1<T>(IEnumerable<T> queryable, int total, Func<T, object> converter = null)
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
    }

    public enum VueMsgType
    {
        success,
        warning,
        info,
        error
    }
}
