using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class EnterpriseController : Controller
    {
        // GET: Enterprise
        public ActionResult Index(int type,string name = null, int topnum = 10)
        {
            //Expression<Func<Enterprises, bool>> exp = item => true;
            //if (!string.IsNullOrEmpty(name))
            //{
            //    exp = PredicateBuilder.And(exp, item => item.Name.Contains(name));
            //}

            if (type == 1)
            {
                Expression<Func<Clients, bool>> exp = item => true;
                if (!string.IsNullOrEmpty(name))
                {
                    exp = PredicateBuilder.And(exp, item => item.Name.Contains(name));
                }
                return Json(new ClientsView().Where(exp).Take(topnum), JsonRequestBehavior.AllowGet);
            }
            else if (type == 2)
            {
                Expression<Func<Suppliers, bool>> exp = item => true;
                if (!string.IsNullOrEmpty(name))
                {
                    exp = PredicateBuilder.And(exp, item => item.Name.Contains(name));
                }
                return Json(new SuppliersView().Where(exp).Take(topnum), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
            //return Json(new EnterprisesTopView().Where(exp).Take(topnum), JsonRequestBehavior.AllowGet);
        }
    }
}