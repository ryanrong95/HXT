using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.PdaApi.Services.Extends;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.PdaApi.Controllers
{
    /// <summary>
    /// Barcode接口
    /// </summary>
    public class BarcodesController : Controller
    {
        [HttpPost]
        public ActionResult Base64(JPost jpost)
        {
            var args = new
            {
                Data = jpost["Data"]?.Value<string>(),
                Type = jpost["Type"]?.Value<int>() ?? 9, //默认Code128
                Width = jpost["Width"]?.Value<int>() ?? 200,
                Height = jpost["Height"]?.Value<int>() ?? 50,
                ShowText = jpost["ShowText"]?.Value<bool>() ?? true,
            };

            if (string.IsNullOrEmpty(args.Data))
                throw new ArgumentNullException("用于渲染条形码的数据不能为空");

            var result = args.Data.Barcode(args.Type, args.Width, args.Height, args.ShowText);
            return Json(new { success = true, code = 200, data = result }, JsonRequestBehavior.DenyGet);
        }
    }
}