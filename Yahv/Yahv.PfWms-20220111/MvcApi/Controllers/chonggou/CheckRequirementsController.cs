using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers.chonggou
{
    public class CheckRequirementsController : Controller
    {
        public ActionResult Enter(JPost jpost)
        {
            try
            {
                var requirement = jpost["CheckRequirement"];
                var arguments = new
                {
                    waybillID = jpost["WaybillID"]?.Value<string>(),
                    checkRequirements = jpost["CheckRequirement"].ToObject<CheckRequirement>(),
                };

                using (var view = new WayRequirementsView())
                {
                    view.Enter(arguments.waybillID, arguments.checkRequirements);
                    return Json(new JMessage
                    {
                        code = 200,
                        success = true,
                        data = "",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                });
            }
        }
    }
}