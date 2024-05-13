using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Yahv.Linq.Extends;
using Yahv.Underly;

namespace MvcApi.Controllers
{
    public class OriginsController : Controller
    {

        /// <summary>
        /// 原产地（输送地）
        /// </summary>
        /// <param name="key">关键字</param>
        /// <returns></returns>
        /// <Url>/ApiWms/Origins?key=</Url>
        public ActionResult Index(string key=null)
        {
            Expression<Func<Origin, bool>> exp = item=>true;
            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And(item => item.GetOrigin().ChineseName.Contains(key)||item.GetOrigin().Code.ToLower().Contains(key.ToLower()));
            }
            return Json(new { obj = Enum.GetValues(typeof(Origin)).Cast<Origin>().Where(exp.Compile()).Select(item => { var origin = item.GetOrigin(); return new { ID = ((int)item).ToString(), CorPlace = origin.Code, CorPlaceDes = origin.ChineseName }; }) }, JsonRequestBehavior.AllowGet) ;
        }
    }
}