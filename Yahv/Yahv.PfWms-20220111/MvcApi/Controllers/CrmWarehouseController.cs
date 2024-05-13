using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class CrmWarehouseController : Controller    
    {
        /// <summary>
        /// 获得crm的大区
        /// </summary>
        /// <returns></returns>
        // GET: CrmWarehouse
        public ActionResult Index()
        {
            return Json(new CrmWarehouseView(), JsonRequestBehavior.AllowGet);
        }
    }
}