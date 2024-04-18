using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.SzApi.Controllers
{
    public class ClientApiController : Controller
    {
        /// <summary>
        /// 根据 ClientID 查询业务员的信息（姓名和手机号）
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetServiceManagerInfo(JPost jpost)
        {
            try
            {
                string ClientID = jpost["ClientID"]?.Value<string>();

                var serviceManagerInfo = new ServiceManagerInfoView(ClientID).GetTheServiceManagerInfo();
                if (serviceManagerInfo == null)
                {
                    Response.StatusCode = 200;
                    return Json(new { success = false, msg = "根据 ClientID = " + ClientID + " 查询不到业务员信息" }, JsonRequestBehavior.AllowGet);
                }

                var theServiceManager = new
                {
                    AdminID = serviceManagerInfo.AdminID,
                    RealName = serviceManagerInfo.RealName,
                    Mobile = serviceManagerInfo.Mobile,
                };

                Response.StatusCode = 200;
                return Json(new { success = true, msg = "", serviceManager = theServiceManager }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 200;
                return Json(new { success = false, msg = "发生错误：" + ex.Message, }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}