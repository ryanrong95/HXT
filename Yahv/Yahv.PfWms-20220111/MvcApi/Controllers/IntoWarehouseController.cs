//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Transactions;
//using System.Web;
//using System.Web.Mvc;
//using Wms.Services;
//using Wms.Services.Enums;
//using Wms.Services.Models;
//using Wms.Services.Views;
//using Yahv.Underly;
//using Yahv.Underly.Attributes;

//namespace MvcApp.Controllers
//{
//    public class IntoWarehouseController : Controller
//    {
//        #region 自定义枚举
//        enum Message
//        {
//            [Description("成功")]
//            Success = 0,
//            [Description("系统出现异常")]
//            CatchException = 1,
//            [Description("请确保通知编号、到货数量、入库库位数据不能为空")]
//            DataWrong = 2,
//            [Description("请确保到货数量不能为0")]
//            CannotBeZero = 3,
//            [Description("库位编号输入有误")]
//            ShelfNumberWrong = 4,
//            [Description("请确保通知编号不为空")]
//            NoticeIsnull = 5,
//            [Description("输入的通知编号为空")]
//            NoticeInvalid = 6
//        }
//        #endregion

//        #region 私有变量
//        Message message;
//        #endregion

//        // 根据noticeID返回入库页面相关信息
//        //public ActionResult Index(string noticeID)
//        //{
//        //    if (string.IsNullOrEmpty(noticeID))
//        //    {
//        //        message = Message.NoticeIsnull;
//        //        return Json(new { val = (int)message, obj = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//        //    }

//        //    Notices notice = new NoticesView().Where(p => p.ID == noticeID).ToList().FirstOrDefault();
//        //    if (notice == null)
//        //    {
//        //        message = Message.NoticeInvalid;
//        //        return Json(new { val = (int)message, obj = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//        //    }



//        //    int arrivalsCount = 0;
//        //    var list = new List<object>();

//        //    var rst = new StoragesView().Where(p => p.ItemID == notice.Inputs.ItemID);
//        //    if (rst != null)
//        //    {
//        //        var query = rst.GroupBy(p => p.ItemID);
//        //        foreach (var item in query)
//        //        {
//        //            arrivalsCount = item.Sum(p => p.Quantity);
//        //        }
//        //    }

//        //    var entity = new { notice.ID, notice.CreateDate, notice.Waybills.ClientCode, notice.StatusDes, notice.Inputs.Supplier, notice.Waybills.Code, notice.TargetDes, notice.Waybills.Type, notice.Waybills.CarrierID, notice.StandardProducts.PartNumber, notice.StandardProducts.Manufacturer, notice.Inputs.DataCode, notice.Quantity, arrivalsCount, notice.Inputs.Origin };
//        //    return Json(new { obj = entity }, JsonRequestBehavior.AllowGet);
//        //}

//         /// <summary>
//         /// 用于测试时为HttpGet，正式使用时是HttpPost并将上边的注释去掉
//         /// </summary>
//         /// <param name="noticeID"></param>
//         /// <param name="shelfName"></param>
//         /// <param name="arrivalsCount"></param>
//         /// <param name="carrierID"></param>
//         /// <param name="waybillCode"></param>
//         /// <param name="volume"></param>
//         /// <param name="weight"></param>
//         /// <returns></returns>
//        [HttpGet]
//        public ActionResult Index(string noticeID, string shelfName, string arrivalsCount, string carrierID = null, string waybillCode = null, string volume = null, string weight = null)
//        {
//            //检测通知编号、库位号、到货数量是否为空
//            if (string.IsNullOrEmpty(noticeID) || string.IsNullOrEmpty(shelfName) || string.IsNullOrEmpty(arrivalsCount))
//            {
//                message = Message.DataWrong;
//                return Json(new { val = (int)message, obj = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//            }

//            //检测通知编号是否有效
//            Notices notice = new NoticesView().Where(p => p.ID == noticeID).FirstOrDefault();
//            if (notice == null)
//            {
//                return Json(new { obj = Message.DataWrong }, JsonRequestBehavior.AllowGet);
//            }

//            //检测到货数量是否为0
//            int arrivalInt = 0;
//            if (!int.TryParse(arrivalsCount, out arrivalInt) || arrivalInt == 0)
//            {
//                message = Message.CannotBeZero;
//                return Json(new { val = (int)message, obj = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//            }

//            //检测库位是否有效
//            //Shelves shelve = new ShelvesView().Where(p => p.ID.ToUpper() == shelfName.ToUpper() || p.ID.ToUpper() == shelfName.ToUpper()).ToList().FirstOrDefault();
//            //if (shelve == null)
//            //{
//            //    message = Message.ShelfNumberWrong;
//            //    return Json(new { val = (int)message, obj = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//            //}

//            //为体积赋值
//            decimal volumeDecimal = 0;
//            decimal? volumeNull;
//            if (!decimal.TryParse(volume, out volumeDecimal))
//            {
//                volumeNull = null;
//            }
//            else
//            {
//                volumeNull = volumeDecimal;
//            }

//            //为重量赋值
//            decimal weightDecimal = 0;
//            decimal? weightNull;
//            if (!decimal.TryParse(weight, out weightDecimal))
//            {
//                weightNull = null;
//            }
//            else
//            {
//                weightNull = weightDecimal;
//            }

//            bool changeWb = false;
//            Waybills wb = notice.Waybills;

//            //修改waybill代码
//            if (!string.IsNullOrEmpty(waybillCode))
//            {
//                if (wb != null && wb.Code != waybillCode)
//                {
//                    changeWb = true;
//                    wb.Code = waybillCode;
//                }
//            }

//            if (!string.IsNullOrEmpty(carrierID))
//            {
//                if (wb != null && wb.CarrierID != carrierID)
//                {
//                    changeWb = true;
//                    wb.CarrierID = carrierID;
//                }
//            }

//            TransactionOptions transactionOption = new TransactionOptions();

//            //设置事务隔离级别
//            transactionOption.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;

//            // 设置事务超时时间为60秒
//            transactionOption.Timeout = new TimeSpan(0, 0, 60);

//            //using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOption))
//            //{
//                try
//                {
//                    if (changeWb)
//                    {
//                        wb.Enter();
//                    }

//                    //添加Sorting
//                    Sorting sorting = new Sorting() { BoxingCode = notice.BoxCode, AdminID = "11111", NoticeID = notice.ID, Quantity = arrivalInt, Volume = volumeNull, Weight = weightNull };
//                    sorting.Enter();
//                    //添加Storage
//                    Storages storage = new Storages() { InputID = notice.InputID, ItemID = notice.Inputs.ItemID, NoticeID = notice.ID, OrderID = notice.Inputs.OrderID, ProductID = notice.Inputs.ProductID, Quantity = arrivalInt, /*ShelveID = shelve.ID,*/ SortingID = sorting.ID, Status = StoragesStatus.Normal, Type = StoragesTypes.Inventory, WareHouseID = "1111" };
//                    storage.Enter();
//                    //scope.Complete();
//                }
//                catch (Exception ex)
//                {
//                    //if (scope != null)
//                    //{
//                    //    scope.Dispose();
//                    //}

//                    return Json(new { obj = ex.Message }, JsonRequestBehavior.AllowGet); 
//                }
//            //}

//            return Json(new { obj = Message.Success }, JsonRequestBehavior.AllowGet);
//        }
//    }
//}