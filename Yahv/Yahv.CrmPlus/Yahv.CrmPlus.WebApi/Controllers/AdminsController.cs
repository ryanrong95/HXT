using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.CrmPlus.WebApi.Controllers
{
    public class AdminsController : Controller
    {
        // GET: Admins
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        ///获取指定阅读人
        /// </summary>
        /// <returns></returns>
       [HttpGet]
        public ActionResult AdminLists()
        {
            var admins = new YaHv.CrmPlus.Services.Views.Rolls.AdminsAllRoll().Where(item => item.RoleID == FixedRole.SaleManager.GetFixedID() || item.RoleID == FixedRole.PM.GetFixedID());
            return Json(admins.Select(item=> new { ID=item.ID,Name=item.RealName}).Take(20), JsonRequestBehavior.AllowGet);

        }
    }
}