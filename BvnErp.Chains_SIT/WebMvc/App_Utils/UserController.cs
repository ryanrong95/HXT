using Needs.Ccs.Services;
using Needs.Utils.Serializers;
using Needs.Wl.User.Plat;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace Needs.Wl.Web.Mvc
{
    abstract public class UserController : Controller
    {
        public UserController()
        {
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var user = UserPlat.Current;

            ViewBag.UserName = user == null ? "" : user.UserName;

            var purchaser = PurchaserContext.Current;
            ViewBag.CompanyShortName = purchaser.ShortName;  //公司简称
            ViewBag.WebUrl = purchaser.OfficalWebsite;   //前端主页
            ViewBag.WebHtml = purchaser.OfficalHtml;
            var shortCode = ConfigurationManager.AppSettings["Purchaser"];
            ViewBag.Logo = "logo_" + shortCode + ".png";
            ViewBag.LogoWhite = "logo_white_" + shortCode + ".png";
            ViewBag.IsHideOldUrl = shortCode == "XDT" ? false : true;
            ViewBag.IsInside = user == null ? "n" : (user.Client.ClientType == Needs.Wl.Models.Enums.ClientType.Internal ? "y" : "n");
        }

        public JsonResult JsonResult(VueMsgType type, string message)
        {
            return Json(new { type = type.ToString(), msg = message }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JsonResult(VueMsgType type, string message, string data)
        {
            return Json(new { type = type.ToString(), msg = message, data = data }, JsonRequestBehavior.AllowGet);
        }

        [Obsolete("作废")]
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

        public JsonResult Paging<T>(IQueryable<T> queryable, Func<T, object> converter = null)
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
}