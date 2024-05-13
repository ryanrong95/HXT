//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Transactions;
//using System.Web;
//using System.Web.Mvc;
//using Wms.Services.Enums;
//using Wms.Services.Models;
//using Wms.Services.Views;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Underly.Attributes;
//using Yahv.Usually;
//using Yahv.Utils.Converters;
//using Yahv.Utils.Converters.Contents;
//using Yahv.Utils.Serializers;

//namespace MvcApp.Controllers
//{
//    public class StoragesController : Controller
//    {
//        #region 自定义枚举
//        enum Message
//        {
//            [Description("成功")]
//            Success = 0,
//            [Description("传入数据有误")]
//            Error = 1,
//            [Description("编辑数据出错")]
//            EditWrong = 2,
//            [Description("系统出现异常")]
//            CatchException = 3,
//            [Description("失败")]
//            Fail = 4
//        }
//        #endregion

//        #region 私有变量
//        Message message;
//        #endregion

//        #region 公共接口
//        /// <summary>
//        /// 库存查询
//        /// </summary>
//        /// <param name="id">库存编号</param>
//        /// <param name="warehouseID">库房编号</param>
//        /// <param name="sortingID">分拣编号</param>
//        /// <param name="productID">产品编号</param>
//        /// <param name="partNumber">型号</param>
//        /// <param name="catalog">型号</param>
//        /// <param name="manufacture">型号</param>
//        /// <param name="orderID">订单编号</param>
//        /// <param name="beginDate">开始时间</param>
//        /// <param name="endDate">结束时间</param>
//        /// <param name="pageIndex">当前页</param>
//        /// <param name="pageSize">页大小</param>
//        /// <returns></returns>
//        /// <url>http://dev.pfwms.com/api/storages</url>
//        // GET: Inputs
//        public ActionResult Index(string id = null, string warehouseID = null, string sortingID = null, string productID = null, string partNumber = null, string catalog = null, string manufacture = null, string orderID = null, string beginDate = null, string endDate = null, int pageIndex = 1, int pageSize = 10)
//        {
//            try
//            {


//                Expression<Func<Storages, bool>> exp = item => true;

//                if (!string.IsNullOrWhiteSpace(id))
//                {
//                    exp = exp.And(item => item.ID == id);
//                }

//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    exp = exp.And(item => item.WareHouseID.ToUpper().Contains(warehouseID.ToUpper()));
//                }

//                if (!string.IsNullOrWhiteSpace(sortingID))
//                {
//                    exp = exp.And(item => item.SortingID == sortingID);
//                }

//                if (!string.IsNullOrWhiteSpace(productID))
//                {
//                    exp = exp.And(item => item.ProductID == productID);
//                }

//                if (!string.IsNullOrWhiteSpace(orderID))
//                {
//                    exp = exp.And(item => item.OrderID == orderID);
//                }
//                if (!string.IsNullOrWhiteSpace(partNumber))
//                {
//                    exp = exp.And(item => item.Product.PartNumber.Contains(partNumber));

//                }

//                if (!string.IsNullOrWhiteSpace(catalog))
//                {
//                    exp = exp.And(item => item.Product.Catalog.Contains(catalog));

//                }

//                if (!string.IsNullOrWhiteSpace(manufacture))
//                {
//                    exp = exp.And(item => item.Product.Manufacturer.Contains(manufacture));

//                }

//                DateTime? startDate, EndDate;
//                DateExtend.DateHandle(beginDate, endDate, out startDate, out EndDate);

//                if (startDate != null)
//                {
//                    exp = exp.And(item => item.CreateDate >= startDate);
//                }

//                if (EndDate != null)
//                {
//                    exp = exp.And(item => item.CreateDate < EndDate);
//                }

//                return Json(new { obj = new StoragesView().AsEnumerable().Where(exp.Compile()).Paging(pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
//            }
//            catch
//            {
//                message = Message.CatchException;
//                return Json(new { val = (int)message, msg = message.GetDescription() }, JsonRequestBehavior.AllowGet);
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="obj"></param>
//        /// <returns></returns>
//        [HttpPost]
//        public ActionResult Index(uploaddata obj)
//        {

//            var waybill = obj.waybill.JsonTo<Waybills>();

//            var notices = obj.Data.JsonTo<Notices[]>();
//            var status = obj.Status;
//            var summary = obj.Summary.JsonTo<Summaries>();

//            //var stats = obj.Data.JsonTo<Notices[]>();
//            try
//            {

//                //数据处理
//                foreach (var tem in notices)
//                {
//                    //CX :和前台的约定，如果发现了以CX（拆项）开头的id，后台要替换成“空”
//                    if (tem.ID.StartsWith("CX"))
//                    {
//                        tem.ID = "";
//                    }

//                    //var temid = tem.ID;
//                    //if (temid == "")
//                    //{
//                    //    temid = tem.PID;
//                    //}

//                    ////得到本次入库通知的总数量
//                    //var count = stats.Where(item => item.ID == temid || item.PID == temid).Select(item => item.TruetoQuantity).Sum();
//                    ////查询原通知
//                    //var notice = new NoticesView()[temid];
//                    ////获取原产品
//                    //var p = notice.StandardProducts;
//                    ////判断有没有改动，有改动为异常到货暂存，
//                    //if (tem.StandardProducts.PartNumber.StartsWith(p.PartNumber) && string.Concat(p.Manufacturer, notice.Inputs.Origin, notice.DateCode, notice.Quantity).MD5() == string.Concat(tem.StandardProducts.Manufacturer, tem.Inputs.Origin, tem.DateCode, count).MD5())
//                    //{
//                    //    tem.Status = NoticesStatus.Completed;
//                    //}
//                    //else
//                    //{
//                    //    tem.Status = NoticesStatus.TempStorage;
//                    //}

//                }

//                //保证数据正确及完整性



//                //保存运单
//                waybill.Enter();


//                if (waybill.FileInfos != null && waybill.FileInfos.Length > 0)
//                {
//                    //保存运单文件
//                    foreach (var item in waybill.FileInfos)
//                    {
//                        if (!string.IsNullOrEmpty(item.FileBase64Code))
//                        {
//                            item.Url = Yahv.Utils.FileServices.Save(item.FileBase64Code, $"/waybill/{DateTime.Now.ToString("yyyyMMdd")}/{waybill.ID}/");
//                            item.WaybillID = waybill.ID;
//                            item.Type = FileTypes.EnterPicture;
//                            item.Enter();
//                        }
//                    }
//                }

//                //入库

//                foreach (var tem in notices)
//                {
//                    using (var scope = new TransactionScope())
//                    {

//                        //保存进项信息
//                        var inputs = tem.Inputs;
//                        inputs.Enter();

//                        //保存分拣信息
//                        var sorting = new Sorting() { AdminID = "", BoxingCode = tem.BoxCode, NoticeID = tem.ID, Quantity = tem.Quantity, Weight = tem.Weight, Volume = tem.Volume };
//                        sorting.Enter();

//                        //保存产品信息
//                        var product = tem.StandardProducts;
//                        product.Enter();

//                        StoragesTypes st = StoragesTypes.Inventory;
//                        if (tem.Status == NoticesStatus.PartialArriva)
//                        {
//                            st = StoragesTypes.StagingStock;
//                        }

//                        //保存库存信息
//                        var storage = new Storages { Type = st, InputID = tem.InputID, IsLock = false, ItemID = tem.Inputs.ItemID, NoticeID = tem.ID, OrderID = tem.Inputs.OrderID, ProductID = tem.ProductID, Quantity = tem.Quantity, ShelveID = tem.ShelveID, SortingID = sorting.ID, Status = StoragesStatus.Normal };
//                        storage.Enter();

//                        //保存文件信息
//                        if (tem.FileInfos != null && tem.FileInfos.Length > 0)
//                        {
//                            foreach (var item in tem.FileInfos)
//                            {
//                                if (!string.IsNullOrEmpty(item.FileBase64Code))
//                                {

//                                    item.Url = Yahv.Utils.FileServices.Save(item.FileBase64Code, $"/storage/{DateTime.Now.ToString("yyyyMMdd")}/{storage.ID}/");
//                                    item.WaybillID = waybill.ID;
//                                    item.StorageID = storage.ID;
//                                    item.Type = FileTypes.EnterPicture;
//                                    item.Enter();
//                                }
//                            }
//                        }

//                        //保存通知
//                        tem.Status = (NoticesStatus)status;
//                        tem.InputID = inputs.ID;
//                        tem.Enter();

//                        scope.Complete();
//                    }                   

//                }

//                //保存备注
//                if (string.IsNullOrEmpty(summary.ID))
//                {
//                    summary.Enter();
//                }


//                message = Message.Success;
//                return Json(new { val = (int)message, msg = message.GetDescription() });
//            }
//            catch (Exception ex)
//            {
//                var mes = ex.Message;
//                message = Message.Fail;
//                return Json(new { val = (int)message, msg = message.GetDescription() });
//            }


//        }



//        private void Datas_EnterError(object sender, ErrorEventArgs e)
//        {
//            message = Message.EditWrong;
//        }

//        private void Datas_EnterSuccess(object sender, SuccessEventArgs e)
//        {
//            message = Message.Success;
//        }
//        #endregion

//    }

//    public class uploaddata
//    {
//        public string Data { get; set; }
//        public string waybill { get; set; }
//        public int Status { get; set; }
//        public string Summary { get; set; }
//    }


//}