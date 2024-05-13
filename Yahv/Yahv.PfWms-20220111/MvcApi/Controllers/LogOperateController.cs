using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Services.Models;

namespace MvcApi.Controllers
{
    public class LogOperateController : Controller
    {
        // GET: LogOperate
        [HttpPost]
        public ActionResult Index(Log_Operating log)
        {
            try
            {
                var wh = Yahv.Erp.Current.WareHourse;
                wh.Logs_Operate(log);
                return Json(new { val=0,msg="成功"});
            }
            catch 
            {
                return Json(new { val = 1, msg = "失败" });

            }

        }
    }
}