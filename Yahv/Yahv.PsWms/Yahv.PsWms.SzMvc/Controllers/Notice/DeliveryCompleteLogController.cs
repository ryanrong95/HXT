using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Views;
using Yahv.Underly;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class NoticeController : BaseController
    {
        /// <summary>
        /// 到货完成记录
        /// </summary>
        /// <returns></returns>
        public ActionResult DeliveryCompleteLog() { return View(); }

        /// <summary>
        /// 获取到货完成记录
        /// </summary>
        /// <returns></returns>
        public JsonResult GetLogs(GetLogsSearchModel searchModel)
        {
            using (PsOrderRepository repository = new PsOrderRepository())
            {
                var logs = repository.ReadTable<Layers.Data.Sqls.PsOrder.Logs>()
                                .Where(t => t.ActionName == LogAction.DeliveryCompleteHandle.GetDescription())
                                .OrderByDescending(t => t.CreateDate)
                                .Skip(searchModel.rows * (searchModel.page - 1)).Take(searchModel.rows)
                                .Select(item => new
                                {
                                    BatchID = item.MainID,
                                    CreateDate = item.CreateDate,
                                })
                                .ToArray();

                var logs1 = logs.Select(item => new
                {
                    BatchID = item.BatchID,
                    CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                }).ToArray();

                int tatal = repository.ReadTable<Layers.Data.Sqls.PsOrder.Logs>()
                                .Where(t => t.ActionName == LogAction.DeliveryCompleteHandle.GetDescription())
                                .Count();

                return Json(new { type = "success", msg = "", data = new { list = logs1, total = tatal } }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取所有类型的 OrderItem
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public JsonResult GetAllTypeOrderItem(GetAllTypeOrderItemSearchModel searchModel)
        {
            GetAllTypeOrderItemReturnModel returnModel = new GetAllTypeOrderItemReturnModel();

            ModifyOrderItemLogsView logsView = new ModifyOrderItemLogsView(searchModel.BatchID);
            var topview = logsView.GetTopviewOrderItem();
            var origin = logsView.GetOriginOrderItem();
            var new1 = logsView.GetNewOrderItem();

            Func<ModifyOrderItemLogsViewModel, GetAllTypeOrderItemReturnModel.OrderItem> selector = item => new GetAllTypeOrderItemReturnModel.OrderItem
            {
                OrderItemID = item.OrderItemID,
                OrderID = item.OrderID,
                ProductID = item.ProductID,
                CustomCode = item.CustomCode,
                StocktakingTypeDes = Convert.ToString((int)item.StocktakingType),
                Mpq = item.Mpq,
                PackageNumber = item.PackageNumber,
                Total = item.Total,
                CreateDateDes = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                ModifyDateDes = item.ModifyDate.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                StatusDes = Convert.ToString((int)item.Status),
                BakPartnumber = item.BakPartnumber,
                BakBrand = item.BakBrand,
                BakPackage = item.BakPackage,
                BakDateCode = item.BakDateCode,
                NoticeID = item.NoticeID,
                NoticeItemID = item.NoticeItemID,
            };

            returnModel.topview = topview.Select(selector).ToArray();
            returnModel.origin = origin.Select(selector).ToArray();
            returnModel.new1 = new1.Select(selector).ToArray();

            return Json(new { type = "success", msg = "", data = returnModel }, JsonRequestBehavior.AllowGet);
        }
    }
}