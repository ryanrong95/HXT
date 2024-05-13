using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Enums;
using Needs.Utils.Descriptions;

namespace MvcApp.Controllers
{
    public class GetEnumController : Controller
    {
        // GET: GetEnum
        public ActionResult Index()
        {
            var noticeType = new List<object>();
            foreach (NoticesType item in Enum.GetValues(typeof(NoticesType)))
            {
                noticeType.Add(new { name=item.GetDescription(),value=(int)item});
            }

            var noticeStatus = new List<object>();
            foreach (NoticesStatus item in Enum.GetValues(typeof(NoticesStatus)))
            {
                noticeStatus.Add(new { name = item.GetDescription(), value = (int)item });
            }
     
            return Json(new { NoticeType=noticeType,NoticeStatus=noticeStatus},JsonRequestBehavior.AllowGet);
        }
    }
}