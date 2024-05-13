using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Wms.Services;
using Wms.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace MvcApi.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //string str = "{\"Notices\":[{\"Product\":null,\"Picking\":null,\"Output\":{\"ID\":\"Opt2019112700000011\",\"InputID\":null,\"OrderID\":\"Order201911270009\",\"ItemID\":\"OrderItem20191127000020\",\"OwnerID\":\"SA01\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":114.28571428571428571428571429,\"StorageID\":\"STOR20191115000001\",\"CreateDate\":\"2019-11-27T15:13:58.20355+08:00\"},\"ID\":null,\"Type\":305,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":\"Waybill201911270009\",\"InputID\":null,\"OutputID\":\"Opt2019112700000011\",\"ProductID\":\"108D8DEACCD73B70DEC3570A1164E977\",\"Supplier\":null,\"DateCode\":null,\"Quantity\":7,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-11-27T15:13:58.2025518+08:00\",\"Status\":100,\"Source\":20,\"Target\":400,\"BoxCode\":null,\"Weight\":0.02,\"Volume\":0.01,\"ShelveID\":null,\"Files\":[],\"Visable\":true,\"Checked\":false},{\"Product\":null,\"Picking\":null,\"Output\":{\"ID\":\"Opt2019112700000012\",\"InputID\":null,\"OrderID\":\"Order201911270009\",\"ItemID\":\"OrderItem20191127000021\",\"OwnerID\":\"SA01\",\"SalerID\":null,\"CustomerServiceID\":null,\"PurchaserID\":null,\"Currency\":2,\"Price\":116.66666666666666666666666667,\"StorageID\":\"STOR20191115000002\",\"CreateDate\":\"2019-11-27T15:13:58.20355+08:00\"},\"ID\":null,\"Type\":305,\"WareHouseID\":\"HK01_WLT\",\"WaybillID\":\"Waybill201911270009\",\"InputID\":null,\"OutputID\":\"Opt2019112700000012\",\"ProductID\":\"F403C1EDCA449C362985D7AF108E2291\",\"Supplier\":null,\"DateCode\":null,\"Quantity\":6,\"Conditions\":{\"DevanningCheck\":false,\"Weigh\":false,\"CheckNumber\":false,\"OnlineDetection\":false,\"AttachLabel\":false,\"PaintLabel\":false,\"Repacking\":false,\"PickByValue\":false},\"CreateDate\":\"2019-11-27T15:13:58.20355+08:00\",\"Status\":100,\"Source\":20,\"Target\":400,\"BoxCode\":null,\"Weight\":0.02,\"Volume\":0.01,\"ShelveID\":null,\"Files\":[],\"Visable\":true,\"Checked\":false}]}";
            //var strObj = str.JsonTo<Yahv.Services.Models.PickingWaybill>();

            return null;
        }
    }
}