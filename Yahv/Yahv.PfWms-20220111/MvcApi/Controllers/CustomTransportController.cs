using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services;
using Wms.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Attributes;

namespace MvcApi.Controllers
{
    /// <summary>
    /// 报关运输
    /// </summary>
    public class CustomTransportController : Controller
    {
        enum Message
        {
            [Description("上架成功")]
            Success = 0,
            [Description("上架失败")]
            Failed = 1,
        }

        // GET: CustomTransport
        /// <summary>
        /// 报关运输
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="warehouseID">库房编号</param>
        /// <param name="lotnumber">运输批次号</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="carrierID">承运商编号</param>
        /// <param name="carNumber">车牌号</param>
        /// <param name="pageindex">当前页码</param>
        /// <param name="pagesize">每页记录数</param>
        /// <param name="status">截单状态</param>
        /// <returns></returns>
        public ActionResult Index(int type, string warehouseID, string lotnumber = null, string startdate = null, string enddate = null, string carrierID = null, string carNumber = null, string status = null,
             int pageindex = 1, int pagesize = 20)
        {
            CuttingOrderStatus? currentStatus = null;
            int number_currentStatus;

            if (int.TryParse(status, out number_currentStatus))
            {
                currentStatus = (CuttingOrderStatus)number_currentStatus;
            }

            var data = new CustomServieces().GetCustomTransport(type, warehouseID, lotnumber, startdate, enddate, carrierID, carNumber, currentStatus, pageindex, pagesize);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 报关运输出库详情
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <param name="lotnumber"></param>
        /// <returns></returns>
        public ActionResult Detail(string warehouseID, string lotnumber)
        {
            var data = new CustomServieces().Detail(warehouseID, lotnumber);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        int val = 0;
        string msg = "出库成功";
        [HttpPost]
        public ActionResult OutputEnter(/*string warehouseID,*/ string lotnumber)
        {
            try
            {
                var wh = Yahv.Erp.Current.WareHourse;
                wh.OutputSuccess += Wh_OutputSuccess;
                wh.OutputFailed += Wh_OutputFailed;
                wh.NotCutting += Wh_NotCutting;
                wh.StoQuantityLess += Wh_StoQuantityLess;
                wh.OutputEnter(/*warehouseID,*/lotnumber);
                if (val == 1)
                {
                    msg = "出库失败";
                }
                else if (val == 2)
                {
                    msg = "该运输批次未截单，请先截单";
                }
                else if (val == 3)
                {
                    msg = "库存数量不足，无法出库";
                }
                return Json(new { val = val, msg = msg });
            }
            catch
            {
                return Json(new { val = 1, msg = "出库失败" });
            }
        }

        private void Wh_StoQuantityLess(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            val = 3;
        }

        private void Wh_NotCutting(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            val = 2;
        }

        private void Wh_OutputFailed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            val = 1;
        }

        private void Wh_OutputSuccess(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            val = 0;
        }

        /// <summary>
        /// 深圳入库详情
        /// </summary>
        /// <param name="warehouseID"></param>
        /// <param name="lotnumber"></param>
        /// <returns></returns>
        public ActionResult EnterDetail(string warehouseID, string lotnumber)
        {
            var data = new CustomServieces().EnterDetail(warehouseID, lotnumber);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        Message message;

        /// <summary>
        /// 上架
        /// </summary>
        /// <param name="boxCodes">箱号</param>
        /// <param name="newShelveID">库位编号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpperShelf(string[] boxCodes, string newShelveID)
        {
            try
            {
                var serviece = new CustomServieces();
                serviece.Success += Serviece_Success;
                serviece.Failed += Serviece_Failed;
                serviece.UpperShelf(boxCodes, newShelveID);

                return Json(new { val = message, msg = message.GetDescription() });
            }
            catch
            {
                return Json(new { val = (int)Message.Failed, msg = Message.Failed.GetDescription() });
            }

        }

        private void Serviece_Failed(object sender, Yahv.Usually.ErrorEventArgs e)
        {
            message = Message.Failed;
        }

        private void Serviece_Success(object sender, Yahv.Usually.SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}