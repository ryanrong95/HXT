using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class cgInternalOrdersController : Controller
    {
        [HttpPost]
        public ActionResult Enter(JPost obj)
        {
            try
            {
                var xdtOrder = obj.ToObject<PvWsOrderInsApiModel>();
                InternalOrderServices service = new InternalOrderServices();
                service.Enter(xdtOrder);
                var json = new JMessage() { code = 200, success = true, data = "内单数据接口成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "内单数据接口失败：" + (ex.InnerException ?? ex).Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AbandonByOrderID(string orderId)
        {
            try
            {
                InternalOrderServices service = new InternalOrderServices();
                service.AbandonByOrderID(orderId);
                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AbandonByOrderItemID(string orderItemId)
        {
            try
            {
                InternalOrderServices service = new InternalOrderServices();
                service.AbandonByOrderItemID(orderItemId);
                var json = new JMessage() { code = 200, success = true, data = "成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult XDTOrderEnter()
        {
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                InternalOrderServices service = new InternalOrderServices();
                service.XDTOrderEnter();

                sw.Stop();
                var json = new JMessage() { code = 200, success = true, data = "调用成功,时长" + sw.Elapsed.TotalMilliseconds };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = "失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
    }
}