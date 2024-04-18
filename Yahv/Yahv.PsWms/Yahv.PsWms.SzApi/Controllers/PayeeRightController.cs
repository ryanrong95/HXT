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
    public class PayeeRightController : Controller
    {

        public ActionResult Index()
        {
            return Json(new { msg = "dfs" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SynPayeeRight(JPost jpost)
        {
            try
            {
                var payeeRight = jpost.ToObject<PayeeRight>();
                //添加日志
                Log log = new Log
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.RealChangeOrderStatusRequest.GetDescription(),
                    MainID = payeeRight?.ID,
                    Url = "/PayeeRight/SynPayeeRight",
                    Content = jpost.ToString(),
                    CreateDate = DateTime.Now,
                };
                log.Insert();
                //保存数据
                payeeRight.Currency = Currency.CNY;
                payeeRight.CreateDate = DateTime.Now;
                payeeRight.Enter();

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