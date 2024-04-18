using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Yahv.PvWsPortal2.MvcApp.Controllers
{ 
    public class BaseController : Controller
    {
        public int SearchCount
        {
            get
            {
                var count = Yahv.Utils.Http.iSession.Current["SearchCount"];
                int result = 0;
                int.TryParse(count?.ToString(),out result);
                return result;
            }
            set
            {
                Yahv.Utils.Http.iSession.Current["SearchCount"] = value.ToString();
            }
        }

        public JsonResult JsonResult(VueMsgType type, string message, string data)
        {
            return Json(new { type = type.ToString(), msg = message, data = data }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResult(VueMsgType type, string message)
        {
            return Json(new { type = type.ToString(), msg = message }, JsonRequestBehavior.AllowGet);
        }
    }

    /// <summary>
    /// 客户端消息类型枚举
    /// </summary>
    public enum VueMsgType
    {
        success,
        warning,
        info,
        error
    }
}