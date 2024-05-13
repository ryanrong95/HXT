using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;

namespace MvcApi.Controllers.chonggou
{
    public class StaffController : Controller
    {
        // GET: Staff
        public ActionResult Index()
        {
            var data = StaffManage.GetData().Select(item => new
            {
                Name = item.Name,
                Age = item.Age
            });

            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}