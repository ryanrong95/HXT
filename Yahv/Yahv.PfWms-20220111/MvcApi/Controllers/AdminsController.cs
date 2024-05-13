using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;
using Yahv.Linq.Extends;

namespace MvcApp.Controllers
{
    public class AdminsController : Controller
    {
        // GET: Admins
        public ActionResult Index(string name = null, int topnum = 10)
        {
            Expression<Func<Yahv.Services.Models.Admin, bool>> exp = item => true;
            //Func<Admin, bool> ex = item => true;
            if (!string.IsNullOrEmpty(name))
            {
                exp = PredicateExtends.And(exp, item => item.RealName.Contains(name));
            }
            return Json(new AdminsView().Where(exp).Select(item => new
            {
                item.ID,
                item.RealName
            }).Take(topnum), JsonRequestBehavior.AllowGet);
        }
    }
}