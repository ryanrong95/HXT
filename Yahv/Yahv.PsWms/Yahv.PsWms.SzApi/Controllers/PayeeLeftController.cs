using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Web.Mvc;
using Yahv.Linq;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.SzApi.Controllers
{
    public class PayeeLeftController : Controller
    {

        public ActionResult Index()
        {
            return Json(new { msg = "dfs" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SynPayeeLeft(JPost jpost)
        {
            try
            {
                var payeeLeft = jpost.ToObject<PayeeLeft>();
                //添加日志
                Log log = new Log
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.RealChangeOrderStatusRequest.GetDescription(),
                    MainID = payeeLeft?.ID,
                    Url = "/PayerLeft/SynPayeeLeft",
                    Content = jpost.ToString(),
                    CreateDate = DateTime.Now,
                };
                log.Insert();
                //保存数据
                payeeLeft.Enter();

                Response.StatusCode = 200;
                return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}