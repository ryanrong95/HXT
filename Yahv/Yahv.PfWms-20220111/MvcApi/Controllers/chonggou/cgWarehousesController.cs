using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.chonggous;
using Wms.Services.Enums;
using Wms.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    /// <summary>
    /// 显示库房
    /// </summary>
    public class cgWarehousesController : Controller
    {
        /// <summary>
        /// 获得对应的库房
        /// </summary>
        /// <returns></returns>
        public ActionResult Show()
        {
            var data = new[]
            {
                Yahv.Services.WhSettings.HK,
                Yahv.Services.WhSettings.SZ,
            };

            return Json(data.Select(item => new
            {
                value = item.ID,
                label = item.Name,
                children = item.Doors.Select(tem => new
                {
                    value = tem.ID,
                    label = tem.Name,
                })
            }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据库房id获得对应的页面菜单
        /// </summary>
        /// <param name="whid">库房ID</param>
        /// <returns></returns>
        public ActionResult Menus(string whid)
        {
            Yahv.Services.Menu[] menu;

            if (whid.ToUpper().StartsWith(nameof(Yahv.Services.WhSettings.HK)))
            {
                whid = nameof(Yahv.Services.WhSettings.HK);
            }
            if (whid.ToUpper().StartsWith(nameof(Yahv.Services.WhSettings.SZ)))
            {
                whid = nameof(Yahv.Services.WhSettings.SZ);
            }
            switch (whid)
            {
                case nameof(Yahv.Services.WhSettings.HK):
                    {
                        menu = Yahv.Services.WhSettings.HK.Menus;
                    }
                    break;
                case nameof(Yahv.Services.WhSettings.SZ):
                    {
                        menu = Yahv.Services.WhSettings.SZ.Menus;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return Json(menu, JsonRequestBehavior.AllowGet);
        }
    }
}