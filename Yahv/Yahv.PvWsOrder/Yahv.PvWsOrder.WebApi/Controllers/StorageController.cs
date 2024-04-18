using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.PvWsOrder.WebApi.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class StorageController : Controller
    {
        [HttpPost]
        public JsonResult List(StorageListRequestModel request)
        {
            var storages = new MyStorageView().GetStorageList(request.ClientName);
            return Json(new { code = 200, success = true, data = storages, }, JsonRequestBehavior.AllowGet);
        }
    }
}