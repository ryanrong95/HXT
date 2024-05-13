using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace MvcApi.Controllers
{
    public class WaybillController : Controller
    {
        enum Message
        {
            [Description("不存在")]
            NotExist = 0,
            [Description("存在")]
            Exist = 1,
            [Description("运单已关闭")]
            WaybillClosed = 2,
            [Description("出现异常，请稍后再试")]
            Abnormal = 3,
            [Description("运单已出库")]
            WaybillOutStock = 4
        }
        Message message;

        /// <summary>
        /// 运单是否存在
        /// </summary>
        /// <param name="waybillID">运单编号</param>
        /// <returns></returns>
        public ActionResult WaybillIsExist(string waybillID)
        {
            try
            {
                var waybillServices = Yahv.Erp.Current.WareHourse;
                waybillServices.WaybillClosed += WaybillServices_WaybillClosed;
                waybillServices.WaybillExist += WaybillServices_WaybillExist;
                waybillServices.WaybillNotExist += WaybillServices_WaybillNotExist;
                waybillServices.WaybillOutStock += WaybillServices_WaybillOutStock;

                waybillServices.WaybillIsExist(waybillID);

                return Json(new { val = message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { val = Message.Abnormal, msg = Message.Abnormal.GetDescription() }, JsonRequestBehavior.AllowGet);
            }

        }

        private void WaybillServices_WaybillOutStock(object sender, EventArgs e)
        {
            message = Message.WaybillOutStock;
        }

        private void WaybillServices_WaybillNotExist(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            message = Message.NotExist;
        }

        private void WaybillServices_WaybillExist(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            message = Message.Exist;
        }

        private void WaybillServices_WaybillClosed(object sender, EventArgs e)
        {
            message = Message.WaybillClosed;
        }

        /// <summary>
        /// 获得暂存运单
        /// </summary>
        /// <param name="warehouseID">库房ID，必填</param>
        /// <param name="excuteStatus">执行状态，必填</param>
        /// <param name="waybillCode">运单号</param>
        /// <param name="carrierID">承运商编号</param>
        /// <param name="place">发货地</param>
        /// <param name="shelveID">库位</param>
        /// <param name="createDate">创建时间</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="enterCode">入仓号</param>
        /// <param name="tempEnterCode">入库单号</param>
        /// <param name="pageindex">当前页码</param>
        /// <param name="pagesize">每页显示记录数</param>
        /// <returns></returns>
        /// <returns></returns>
        // GET: Waybill
        public ActionResult Index(string warehouseID, string excuteStatus, string waybillCode = null, string carrierID = null, string place = null, string shelveID = null, string createDate = null, string startDate = null, string endDate = null, string enterCode = null, string tempEnterCode = null, int pageindex = 1,
            int pagesize = 20)
        {
            return Json(new Wms.Services.WayBillServices().TempWaybill(warehouseID, excuteStatus, waybillCode, carrierID, place, shelveID, createDate, startDate, endDate, enterCode, tempEnterCode, pageindex, pagesize), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 暂存运单详情
        /// </summary>
        // GET: Waybill
        public ActionResult Detail(string warehouseID, string waybillID)
        {
            var data = new Wms.Services.WayBillServices().TempStorageWayBill(warehouseID, waybillID);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 关联订单
        /// </summary>
        /// <param name="enterCode">入仓号</param>
        /// <param name="tempEnterCode">入库单号</param>
        /// <param name="partNumber">型号</param>
        /// <param name="manufacture">品牌</param>
        /// <param name="number">数量</param>
        /// <param name="place">产地</param>
        /// <param name="carrierID">承运商</param>
        /// <returns></returns>
        public ActionResult LinkedOrder(string enterCode = null, string tempEnterCode = null, string partNumber = null, string manufacture = null, int? quantity = null, string place = null, string carrierID = null)
        {
            return View();
        }
    }
}