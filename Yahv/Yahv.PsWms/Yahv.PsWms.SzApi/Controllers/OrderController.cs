using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzApi.Attribute;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Models;
using Yahv.PsWms.SzMvc.Services.Models.Origin;
using Yahv.Underly;
using Yahv.Web.Mvc;

namespace Yahv.PsWms.SzApi.Controllers
{
    public class OrderController : Controller
    {
        /// <summary>
        /// 修改订单状态（已变成到货完成接口）
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ChangeOrderStatus(JPost jpost)
        {
            try
            {
                string OrderID = jpost["OrderID"]?.Value<string>();
                string OrderStatus = jpost["OrderStatus"]?.Value<string>();

                StringBuilder sbLogContent = new StringBuilder();
                sbLogContent.AppendFormat(@"OrderID = {0}, OrderStatus = {1}", OrderID, OrderStatus);

                Log log = new Log
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.ChangeOrderStatusRequest.GetDescription(),
                    MainID = OrderID,
                    Url = "/Order/ChangeOrderStatus",
                    Content = sbLogContent.ToString(),
                    CreateDate = DateTime.Now,
                };
                log.Insert();

                int orderStatusInt = 0;
                if (int.TryParse(OrderStatus, out orderStatusInt) == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, msg = "OrderStatus 应该为整数" }, JsonRequestBehavior.AllowGet);
                }

                bool isOrderStatusOk = false;
                foreach (SzMvc.Services.Enums.OrderStatus item in Enum.GetValues(typeof(SzMvc.Services.Enums.OrderStatus)))
                {
                    if ((int)item == orderStatusInt)
                    {
                        isOrderStatusOk = true;
                        break;
                    }
                }
                if (isOrderStatusOk == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, msg = "OrderStatus 不是正确的订单状态值" }, JsonRequestBehavior.AllowGet);
                }

                new Order().ChangeOrderStatus(OrderID, (SzMvc.Services.Enums.OrderStatus)orderStatusInt);

                //根据 DeliveryTopView 同步订单项信息 Begin

                DeliveryCompleteHandler handler = new DeliveryCompleteHandler(OrderID);
                if (handler.CheckIsHasOrder() == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, msg = "PsOrder.dbo.Orders 表中不存在该订单号：" + OrderID }, JsonRequestBehavior.AllowGet);
                }

                handler.SyncOrderItem();

                //根据 DeliveryTopView 同步订单项信息 End

                Response.StatusCode = 200;
                return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Real修改订单状态
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RealChangeOrderStatus(JPost jpost)
        {
            try
            {
                string OrderID = jpost["OrderID"]?.Value<string>();
                string OrderStatus = jpost["OrderStatus"]?.Value<string>();

                StringBuilder sbLogContent = new StringBuilder();
                sbLogContent.AppendFormat(@"OrderID = {0}, OrderStatus = {1}", OrderID, OrderStatus);

                Log log = new Log
                {
                    ID = Guid.NewGuid().ToString("N"),
                    ActionName = LogAction.RealChangeOrderStatusRequest.GetDescription(),
                    MainID = OrderID,
                    Url = "/Order/RealChangeOrderStatus",
                    Content = sbLogContent.ToString(),
                    CreateDate = DateTime.Now,
                };
                log.Insert();

                int orderStatusInt = 0;
                if (int.TryParse(OrderStatus, out orderStatusInt) == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, msg = "OrderStatus 应该为整数" }, JsonRequestBehavior.AllowGet);
                }


                bool isOrderStatusOk = false;
                foreach (SzMvc.Services.Enums.OrderStatus item in Enum.GetValues(typeof(SzMvc.Services.Enums.OrderStatus)))
                {
                    if ((int)item == orderStatusInt)
                    {
                        isOrderStatusOk = true;
                        break;
                    }
                }
                if (isOrderStatusOk == false)
                {
                    Response.StatusCode = 500;
                    return Json(new { success = false, msg = "OrderStatus 不是正确的订单状态值" }, JsonRequestBehavior.AllowGet);
                }

                new Order().ChangeOrderStatus(OrderID, (SzMvc.Services.Enums.OrderStatus)orderStatusInt);


                Response.StatusCode = 200;
                return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        ///// <summary>
        ///// 到货完成接口
        ///// </summary>
        ///// <param name="jpost"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public JsonResult DeliveryComplete(JPost jpost)
        //{
        //    try
        //    {
        //        string OrderID = jpost["OrderID"]?.Value<string>();

        //        StringBuilder sbLogContent = new StringBuilder();
        //        sbLogContent.AppendFormat(@"OrderID = {0}", OrderID);

        //        Log log = new Log
        //        {
        //            ID = Guid.NewGuid().ToString("N"),
        //            ActionName = LogAction.DeliveryCompleteRequest.GetDescription(),
        //            MainID = OrderID,
        //            Url = "/Order/DeliveryComplete",
        //            Content = sbLogContent.ToString(),
        //            CreateDate = DateTime.Now,
        //        };
        //        log.Insert();

        //        DeliveryCompleteHandler handler = new DeliveryCompleteHandler(OrderID);
        //        if (handler.CheckIsHasOrder() == false)
        //        {
        //            Response.StatusCode = 500;
        //            return Json(new { success = false, msg = "PsOrder.dbo.Orders 表中不存在该订单号：" + OrderID }, JsonRequestBehavior.AllowGet);
        //        }

        //        handler.SyncOrderItem();




        //        Response.StatusCode = 200;
        //        return Json(new { success = true, msg = "" }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.StatusCode = 500;
        //        return Json(new { success = false, msg = ex.Message }, JsonRequestBehavior.AllowGet);
        //    }
        //}
    }
}