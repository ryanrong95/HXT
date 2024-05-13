using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Enums;

namespace MvcApp.Controllers
{
    public class SubWarehouseController : Controller
    {
        // GET: SubWarehouse
        public ActionResult Index()
        {
            Dictionary<string, object> dict1 = new Dictionary<string, object>();
            Dictionary<string, object> dict2 = new Dictionary<string, object>();
            #region 获得地区简码

            foreach (var item in Enum.GetValues(typeof(RegionCode)))
            {
                dict1.Add(item.ToString(), ((RegionCode)item).GetDescription());
            }
            #endregion

            #region 获得库房能力

            foreach (var item in Enum.GetValues(typeof(WarehouseAbilities)))
            {
                dict2.Add(((int)item).ToString(), ((WarehouseAbilities)item).GetDescription());
            }
            #endregion

            return Json(new {obj=new { regionMsg = dict1.ToArray(), abilitiesMsg = dict2.ToArray() } }, JsonRequestBehavior.AllowGet);
        }
    }
}