using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Wms.Services.Views;
using Needs.Utils.Serializers;
using Web.Services;
using System.Configuration;
using System.Transactions;

namespace MvcApp.Controllers
{
    public class NoticeInfoController : Controller
    {
        #region 自定义枚举
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("通知信息或者订单信息不能为空")]
            DataWrong = 1,
            [Description("标准产品数据不完整")]
            ProductNull = 2,
            [Description("添加标准产品信息时出现异常")]
            AddProductException = 3,
            [Description("添加进项信息时出现异常")]
            AddInputException = 4,
            [Description("添加通知信息时出现异常")]
            AddNoticeException = 5,
            [Description("添加销项信息时出现异常")]
            AddOutputException = 9,
            [Description("找不到销项对应的进项信息")]
            FindInputByOutputIDException = 6,
            [Description("通过项编号找不到对应的进项信息")]
            CannotFindInputByItemID = 7,
            [Description("通过项编号找不到对应的进项信息")]
            CannotFindOutputByItemID = 8
        }
        #endregion

        #region 私有变量
        Message message;
        #endregion
        /// <summary>
        /// 如果Input属性不为空则是进项通知，否则为销项通知
        /// </summary>
        /// <param name="noticeInfoJson"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        public ActionResult Index(string noticeInfoJson, bool isInput)
        {
            NoticeInfo noticeInfo = noticeInfoJson.JsonTo<NoticeInfo>();
            if (noticeInfo == null)
            {
                message = Message.DataWrong;
                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
            }

            //如果是进项通知
            if (noticeInfo.Input != null)
            {
                //添加产品信息
                message = AddStandardProduct(noticeInfo);
                if (message != Message.Success)
                {
                    return Json(new { val = (int)message, msg = message.GetDescription(), JsonRequestBehavior.AllowGet });
                }

                //添加进项信息
                message = AddInput(noticeInfo);
                if (message != Message.Success)
                {
                    return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
                }

                //添加本地通知信息
                message = AddNotice(noticeInfo);
                if (message != Message.Success)
                {
                    return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                //添加销项信息
                message = AddOutput(noticeInfo);
                if (message != Message.Success)
                {
                    return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
        }

        #region 私有方法
        /// <summary>
        /// 将标准产品添加到本地StandardProduct表中
        /// </summary>
        /// <param name="noticeInfo">通知信息</param>
        /// <returns></returns>
        private Message AddStandardProduct(NoticeInfo noticeInfo)
        {
            //TransactionScope scope = new TransactionScope();
            try
            {
                //using (scope)
                //{

                //    scope.Complete();
                //}
                if (noticeInfo.Items != null)
                {
                    foreach (var item in noticeInfo.Items)
                    {
                        StandardProducts product = new StandardProducts() { Catalog = item.ProductPro.Catalog, Manufacturer = item.ProductPro.Manufacturer, PartNumber = item.ProductPro.PartNumber, UnitGrossWeightBL = item.ProductPro.UnitGrossWeightBL, UnitGrossWeightTL = item.ProductPro.UnitGrossWeightTL, UnitGrossVolume = item.ProductPro.UnitVolume, PackageCase = item.PackageCase, Packing = item.Packing };
                        product.Enter();
                    }
                }

                return Message.Success;
            }
            catch
            {
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}

                return Message.AddProductException;
            }
        }

        /// <summary>
        /// 将进项信息添加到本地Input表中
        /// </summary>
        /// <param name="noticeInfo"></param>
        /// <returns></returns>
        private Message AddInput(NoticeInfo noticeInfo)
        {
            //TransactionScope scope = new TransactionScope();
            try
            {
                //using (scope)
                //{
                //    scope.Complete();
                //}
                if (noticeInfo.Waybills != null)
                {
                    foreach (var item in noticeInfo.Items)
                    {
                    }
                }

                return Message.Success;
            }
            catch
            {
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}

                return Message.AddInputException;
            }
        }

        private Message AddWayBill(NoticeInfo noticeInfo)
        {
            //TransactionScope scope = new TransactionScope();
            try
            {
                //using (scope)
                //{

                //    scope.Complete();
                //}

                return Message.Success;
            }
            catch
            {
                //if (scope != null)
                //{
                //    scope.Dispose();
                //}

                return Message.AddInputException;
            }
        }

        /// <summary>
        /// 将通知信息添加到本地Notice表中
        /// </summary>
        /// <param name="noticeInfo"></param>
        /// <returns></returns>
        private Message AddNotice(NoticeInfo noticeInfo, bool isinput = true)
        {
            TransactionScope scope = new TransactionScope();

            try
            {
                using (scope)
                {
                    //创建箱号
                    Boxes box = new Boxes() { };
                    //foreach (var order in noticeInfo.Orders)
                    //{
                    //    foreach (var waybill in order.Waybills)
                    //    {
                    //        foreach (var item in waybill.Items)
                    //        {
                    //            if (isinput)
                    //            {
                    //                string inputID = new InputsView().Where(it => it.ItemID == item.ID)?.ToString();
                    //                if (!string.IsNullOrEmpty(inputID))
                    //                {
                    //                    Notices notice = new Notices() { BoxCode = box.Code, Conditions = order.Conditions?.JsonTo<NoticeCondition>(), InputID = inputID, ProductID = item.ProductPro.ProductID, Quantity = item.Quantity, ShelveID = noticeInfo.ShelveID, Source = noticeInfo.Source, Target = noticeInfo.Target, Status = Wms.Services.Enums.NoticesStatus.Waiting, Type = noticeInfo.Type, WareHouseID = noticeInfo.WarehouseID, Volume = noticeInfo.Volume, Weight = noticeInfo.Weight, WaybillID = waybill.ID };
                    //                    notice.Enter();
                    //                }
                    //                else
                    //                {
                    //                    return Message.CannotFindInputByItemID;
                    //                }
                    //            }
                    //            else
                    //            {
                    //                string outputID = new OutputsView().Where(it => it.ItemID == item.ID)?.ToString();
                    //                if (!string.IsNullOrEmpty(outputID))
                    //                {
                    //                    Notices notice = new Notices() { BoxCode = box.Code, Conditions = order.Conditions.JsonTo<NoticeCondition>(), OutputID = outputID, ProductID = item.ProductPro.ProductID, Quantity = item.Quantity, ShelveID = noticeInfo.ShelveID, Source = noticeInfo.Source, Target = noticeInfo.Target, Status = Wms.Services.Enums.NoticesStatus.Waiting, Type = noticeInfo.Type, WareHouseID = noticeInfo.WarehouseID, Volume = noticeInfo.Volume, Weight = noticeInfo.Weight, WaybillID = waybill.WaybillCode };
                    //                    notice.Enter();
                    //                }
                    //                else
                    //                {
                    //                    return Message.CannotFindOutputByItemID;
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    scope.Complete();
                }

                return Message.Success;
            }
            catch
            {
                if (scope != null)
                {
                    scope.Dispose();
                }

                return Message.AddOutputException;
            }
        }

        /// <summary>
        /// 添加销项信息到Outputs表中
        /// </summary>
        /// <param name="noticeInfo"></param>
        /// <returns></returns>
        private Message AddOutput(NoticeInfo noticeInfo)
        {
            TransactionScope scope = new TransactionScope();
            try
            {
                using (scope)
                {
                    //foreach (var order in noticeInfo.Orders)
                    //{
                    //    foreach (var waybill in order.Waybills)
                    //    {
                    //        foreach (var item in waybill.Items)
                    //        {
                    //            Inputs input = new InputsView().Where(p => p.ItemID == item.ID).FirstOrDefault();
                    //            if (input != null)
                    //            {
                    //                Outputs output = new Outputs() { Currency = input.Currency, InputID = input.ID, ItemID = item.ID, OrderID = input.OrderID, OwnerID = input.OwnerID, Price = input.UnitPrice, PurchaserID = input.PurchaserID, CustomerServiceID = input.CustomerServiceID, SalerID = input.SalerID };
                    //                output.Enter();
                    //            }
                    //            else
                    //            {
                    //                return Message.FindInputByOutputIDException;
                    //            }
                    //        }
                    //    }
                    //}

                    scope.Complete();
                }

                return Message.Success;
            }
            catch
            {
                if (scope != null)
                {
                    scope.Dispose();
                }

                return Message.AddInputException;
            }
        }
        #endregion
    }
}