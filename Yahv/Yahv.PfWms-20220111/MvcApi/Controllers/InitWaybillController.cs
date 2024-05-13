using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace MvcApi.Controllers
{
    public class InitWaybillController : Controller
    {
        // GET: InitWaybill
        public ActionResult Index()
        {
            var pc = Request.Params;
            var waybillid = pc["waybillid"];
            if (!string.IsNullOrEmpty(waybillid))
            {
                var obj =  new Wms.Services.Views.ServicesWaybillsTopView().Where(item => item.ID == waybillid).FirstOrDefault();
                return Json(obj,JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        /// <summary>
        /// 保存运单
        /// </summary>
        /// <param name="ID">运单ID</param>
        /// <param name="Code">运单号</param>
        /// <param name="TotalParts">件数</param>
        /// <param name="TotalWeight">重量</param>
        /// <param name="TotalVolume">体积</param>
        /// <param name="CarrierID">承运商编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost obj)
        {
            try
            {
                var waybill = obj.ToObject<EnterObj>();
                Yahv.Erp.Current.WareHourse.UpdateWayBillInfo(waybill.ID,waybill.Code,waybill.TotalParts,waybill.TotalWeight,waybill.TotalVolume,waybill.CarrierID);
                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new JMessage { success = false, code = 404, data = "保存失败!" }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    class EnterObj
    {
        public string ID { get; set; }
        public string Code { get; set; }
        public int? TotalParts { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? TotalVolume { get; set; }
        public string CarrierID { get; set; }
    }
}