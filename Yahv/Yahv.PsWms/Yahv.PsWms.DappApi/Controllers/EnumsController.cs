using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.DappApi.Controllers
{
    public class EnumsController : Controller
    {
        private class NewSource<T>
        {
            public T value { get; set; }
            public string name { get; set; }

            public NewSource(T value, string name)
            {
                this.value = value;
                this.name = name;
            }
        }

        public ActionResult NoticeStatus()
        {
            var val = Enum.GetValues(typeof(Services.Enums.NoticeStatus)).Cast<Services.Enums.NoticeStatus>().Where(item => item != 0)
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NoticeStatus_UnComplete()
        {
            var val = Enum.GetValues(typeof(Services.Enums.NoticeStatus)).Cast<Services.Enums.NoticeStatus>()
                .Where(item => 
                item != Services.Enums.NoticeStatus.Rejected &&
                item != Services.Enums.NoticeStatus.Completed &&
                item != Services.Enums.NoticeStatus.Arrivaling &&
                item != Services.Enums.NoticeStatus.Stocking &&
                item != Services.Enums.NoticeStatus.Closed)
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NoticeStatus_Complete()
        {
            var val = Enum.GetValues(typeof(Services.Enums.NoticeStatus)).Cast<Services.Enums.NoticeStatus>()
                .Where(item => item == Services.Enums.NoticeStatus.Arrivaling || item == Services.Enums.NoticeStatus.Rejected || item == Services.Enums.NoticeStatus.Completed)
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Currency()
        {
            var val = Enum.GetValues(typeof(Currency)).Cast<Currency>().Where(item => item != 0)
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IDType()
        {
            var val = Enum.GetValues(typeof(IDType)).Cast<IDType>()
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Express()
        {
            var val = Enum.GetValues(typeof(Services.Enums.Express)).Cast<Services.Enums.Express>()
                .Select(item => new
                {
                    value = item.ToString(),
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FreightPayer()
        {
            var val = Enum.GetValues(typeof(Services.Enums.FreightPayer)).Cast<Services.Enums.FreightPayer>()
                .Select(item => new
                {
                    value = item,
                    name = item.GetDescription()
                });
            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExpressMethod(string expressName)
        {
            var val = new object();
            if (expressName == Services.Enums.Express.SF.ToString())
            {
                val = Enum.GetValues(typeof(Services.Enums.ExpressMethodSF)).Cast<Services.Enums.ExpressMethodSF>()
                   .Select(item => new
                   {
                       value = item,
                       name = item.GetDescription()
                   });
            }
            if (expressName == Services.Enums.Express.KY.ToString())
            {
                val = Enum.GetValues(typeof(Services.Enums.ExpressMethodKY)).Cast<Services.Enums.ExpressMethodKY>()
                    .Select(item => new
                    {
                        value = item,
                        name = item.GetDescription()
                    });
            }
            if (expressName == Services.Enums.Express.DB.ToString())
            {
                val = Enum.GetValues(typeof(Services.Enums.ExpressMethodDB)).Cast<Services.Enums.ExpressMethodDB>()
                    .Select(item => new
                    {
                        value = item,
                        name = item.GetDescription()
                    });
            }

            return Json(new { obj = val }, JsonRequestBehavior.AllowGet);
        }
    }
}