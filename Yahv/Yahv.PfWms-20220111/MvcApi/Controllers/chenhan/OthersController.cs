using System;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;

using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Services.Enums;
using Wms.Services.Enums;

namespace MvcApp.Controllers
{
    public class OthersController : Controller
    {
        /// <summary>
        /// 原产地 
        /// </summary>
        /// <returns></returns>
        public ActionResult Origins(string key)
        {
            Expression<Func<OriginEntity, bool>> exp = item => true;

            var list = Enum.GetValues(typeof(Origin)).Cast<Origin>().Select(item => { var origin = item.GetOrigin(); return new OriginEntity { ID = ((int)item).ToString(), CorPlace = origin.Code, CorPlaceDes = origin.ChineseName, Text = origin.Code + " " + origin.ChineseName }; }).ToList();
            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And(item => item.Text.Contains(key));
            }
            return Json(new { obj = list.Where(exp.Compile()) }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        ///获取承运商
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Carriers(string key = null)
        {
            var data = new Wms.Services.CarriersTopViewServices().Carriers(key);
            return Json(new { obj = data.Select(item => new { item.ID, item.Name }) }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 发货方式为送货上门时获得的承运商
        /// </summary>
        /// <returns></returns>
        public ActionResult GetLocalCarriers(string key=null)
        {
            var data = new Wms.Services.CarriersTopViewServices().GetLocalCarriers(key);
            return Json(new { obj = data.Select(item => new { item.ID, item.Name }) }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DriversCars(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                var drivers = new Wms.Services.Views.DriversTopView().Where(item => item.EnterpriseID == key);
                var cars = new Wms.Services.Views.TransportTopView().Where(item => item.EnterpriseID == key);

                return Json(new { obj = new { drivers = drivers, cars = cars } },JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 获取库区/货架/库位的用途
        /// </summary>
        /// <returns></returns>
        public ActionResult Purposes()
        {
            var data = Enum.GetValues(typeof(ShelvesPurpose)).Cast<ShelvesPurpose>();
            return Json(new { obj = data.Select(item => new { ID = ((int)item).ToString(), purpose = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult CuttingOrderStatus()
        {
            var data = Enum.GetValues(typeof(CgCuttingOrderStatus)).Cast<CgCuttingOrderStatus>();
            return Json(new { obj = data.Select(item => new { ID = ((int)item).ToString(), Status = item.GetDescription() }) }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据Client 的EnterCode 获取对应的等级等信息
        /// </summary>
        /// <param name="enterCode"></param>
        /// <returns></returns>
        public ActionResult GetClientData(string enterCode)
        {
            var data = new Wms.Services.Views.ClientsView().Where(item => item.EnterCode == enterCode).SingleOrDefault();

            return Json(new { obj = data == null ? null : new { Grade = data .Grade, Vip = data.Vip, Name = data.Name}  }, JsonRequestBehavior.AllowGet);
        }
    }

    public class OriginEntity
    {
        public string ID { get; set; }
        public string CorPlace { get; set; }
        public string CorPlaceDes { get; set; }
        public string Text { get; set; }
    }
}