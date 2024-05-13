using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class CgTestController : Controller
    {
        [HttpPost]
        public ActionResult Enter(JPost obj)
        {
            try
            {
                var boxes = obj.ToObject<Boxes>();
                Yahv.Erp.Current.WareHourse.BoxEnter(boxes);
                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;

                return Json(new JMessage { success = false, code = 404, data = "保存失败!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}