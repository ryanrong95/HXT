//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using Wms.Services.Models;
//using System.Linq.Expressions;
//using Wms.Services.Enums;

//using Yahv.Usually;
//using Yahv.Underly.Attributes;
//using Yahv.Linq.Extends;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using Wms.Services;
//using Yahv.Utils.Converters;
//using Wms.Services.Views;

//namespace MvcApp.Controllers
//{
//    public class StorageController : Controller
//    {

//        enum Message
//        {
//            [Description("库位变更成功")]
//            Success = 0,
//            [Description("库位变更失败")]
//            Failed = 1,
//            [Description("所要变更的库位不存在")]
//            ShelveNotExist = 2,
//            [Description("第一个库位不存在产品信息")]
//            StoragesIsNull = 3
//        }
//        Message message;

//        /// <summary>
//        /// 库存查询
//        /// </summary>
//        /// <param name="id">库存编号</param>
//        /// <param name="warehouseID">库房编号</param>
//        /// <param name="type">库存类型</param>
//        /// <param name="sortingID">分拣编号</param>
//        /// <param name="productID">产品编号</param>
//        /// <param name="partNumber">型号</param>
//        /// <param name="manufacture">制造商</param>
//        /// <param name="beginDate">开始时间</param>
//        /// <param name="endDate">结束时间</param>
//        /// <param name="pageIndex">当前页</param>
//        /// <param name="pageSize">页大小</param>
//        /// <returns></returns>
//        /// <url>http://dev.pfwms.com/api/storage</url>
//        // GET: Inputs
//        public ActionResult Index(string id = null, string warehouseID = null, int? type = null, string sortingID = null, string productID = null, string partNumber = null, string manufacturer = null, string beginDate = null, string endDate = null, int pageIndex = 1, int pageSize = 10)
//        {
//            try
//            {
//                Expression<Func<Storage, bool>> exp = item => true;

//                if (!string.IsNullOrWhiteSpace(id))
//                {
//                    exp = exp.And(item => item.ID == id);
//                }

//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    exp = exp.And(item => item.WareHouseID.ToUpper() == warehouseID.ToUpper());
//                }

//                if (type != null)
//                {
//                    exp = exp.And(item => item.Type == (StoragesType)type);
//                }

//                if (!string.IsNullOrWhiteSpace(sortingID))
//                {
//                    exp = exp.And(item => item.SortingID == sortingID);
//                }

//                if (!string.IsNullOrWhiteSpace(productID))
//                {
//                    exp = exp.And(item => item.ProductID == productID);
//                }

//                if (!string.IsNullOrWhiteSpace(partNumber))
//                {
//                    exp = exp.And(item => item.Product.PartNumber.Contains(partNumber));

//                }

//                if (!string.IsNullOrWhiteSpace(manufacturer))
//                {

//                    exp = exp.And(item => item.Product.Manufacturer.Contains(manufacturer));

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

//                return Json(new { obj = new StoragesView().Where(exp).OrderByDescending(item=>item.CreateDate).Paging(pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception ex)
//            {

//                return Json(null, JsonRequestBehavior.AllowGet);
//            }

//        }


//        /// <summary>
//        /// 获得库存信息
//        /// </summary>
//        /// <param name="inputID">进项ID</param>
//        /// <param name="key">型号/制造商/客户编号/库位号</param>
//        /// <returns></returns>
//        public ActionResult GetStorage(string warehouseID = null, string inputID = null, string key = null, int pageIndex = 1, int pageSize = 10)
//        {
//            try
//            {
//                Expression<Func<Storage, bool>> exp = item => item.Quantity > 0;
//                if (!string.IsNullOrWhiteSpace(warehouseID))
//                {
//                    exp = PredicateExtends.And(exp, item => item.WareHouseID == warehouseID);
//                }
//                if (!string.IsNullOrWhiteSpace(inputID))
//                {
//                    exp = PredicateExtends.And(exp, item => item.InputID == inputID);
//                }
//                if (!string.IsNullOrWhiteSpace(key))
//                {
//                    exp = PredicateExtends.And(exp, item => item.Product.PartNumber.Contains(key) || item.Product.Manufacturer.Contains(key) || item.Input.ClientID == key || item.ShelveID.Contains(key));
//                }

//                return Json(new
//                {
//                    obj = new StoragesView().Where(exp).Select(item => new
//                    {
//                        ID = item.ID,
//                        ClientID = item.Input.ClientID,
//                        Place = item.Place,
//                        Quantity = item.Quantity,
//                        Product = item.Product,
//                        ShelveID = item.ShelveID,
//                        Summary = item.Summary
//                    }).Paging(pageIndex, pageSize)
//                }, JsonRequestBehavior.AllowGet);
//            }
//            catch
//            {

//                return Json(null, JsonRequestBehavior.AllowGet);
//            }

//        }

//        [HttpPost]
//        public ActionResult ChangePosition(Storage[] storages, string newShelveID)
//        {
//            try
//            {
//                var storageServiece = new Wms.Services.StorageServices();
//                storageServiece.Success += StorageServiece_Success;
//                storageServiece.Failed += StorageServiece_Failed;
//                storageServiece.ShelveNotExist += StorageServiece_ShelveNotExist;
//                storageServiece.ChangePosition(storages, newShelveID.ToUpper());
//                return Json(new { val = (int)message, msg = message.GetDescription() });
//            }
//            catch
//            {
//                return Json(new { val = (int)Message.Failed, msg = Message.Failed.GetDescription() });
//            }

//        }

//        [HttpPost]

//        public ActionResult ShelvesMigration(string oldShelveID,string newShelveID)
//        {
//            try
//            {
//                var storageServiece = new Wms.Services.StorageServices();
//                storageServiece.Success += StorageServiece_Success;
//                storageServiece.Failed += StorageServiece_Failed;
//                storageServiece.ShelveNotExist += StorageServiece_ShelveNotExist;
//                storageServiece.StoragesIsNull += StorageServiece_StoragesIsNull;
//                storageServiece.ChangePositionByShelveID(oldShelveID, newShelveID.ToUpper());
//                return Json(new { val = (int)message, msg = message.GetDescription() });
//            }
//            catch
//            {
//                return Json(new { val = (int)Message.Failed, msg = Message.Failed.GetDescription() });
//            }
//        }

//        private void StorageServiece_StoragesIsNull(object sender, ErrorEventArgs e)
//        {
//            message = Message.StoragesIsNull;
//        }

//        private void StorageServiece_ShelveNotExist(object sender, ErrorEventArgs e)
//        {
//            message = Message.ShelveNotExist;
//        }

//        private void StorageServiece_Failed(object sender, ErrorEventArgs e)
//        {
//            message = Message.Failed;
//        }

//        private void StorageServiece_Success(object sender, SuccessEventArgs e)
//        {
//            message = Message.Success;
//        }
//    }

//    public class TempStorageController : Controller
//    {
//        enum Message
//        {
//            [Description("暂存成功")]
//            Success = 0,
//            [Description("暂存失败")]
//            Failed = 1,
//            [Description("描述信息和产品信息不能同时为空")]
//            IsNull = 2
//        }

//        Message message;

//        ///// <summary>
//        ///// 获得暂存运单
//        ///// </summary>
//        ///// <param name="warehouseID">库房ID，必填</param>
//        ///// <param name="excuteStatus">执行状态，必填</param>
//        ///// <param name="waybillID">运单号</param>
//        ///// <param name="carrierID">承运商编号</param>
//        ///// <param name="place">发货地</param>
//        ///// <param name="shelveID">库位</param>
//        ///// <param name="createDate">创建时间</param>
//        ///// <param name="pageindex">当前页码</param>
//        ///// <param name="pagesize">每页显示记录数</param>
//        ///// <returns></returns>
//        ///// <returns></returns>
//        //public ActionResult Index(string warehouseID, string excuteStatus, string waybillID = null, string carrierID = null, string place = null, string shelveID = null, string createDate = null, int pageindex = 1,
//        //    int pagesize = 20)
//        //{
//        //    return Json(new Wms.Services.StorageServices().GetWaybill(warehouseID, excuteStatus, waybillID, carrierID, place, shelveID, createDate, pageindex, pagesize), JsonRequestBehavior.AllowGet);
//        //}

//        ///// <summary>
//        ///// 暂存运单详情
//        ///// </summary>
//        ///// <returns></returns>
//        //public ActionResult Detail(string warehouseID,string waybillID)
//        //{
//        //    var data = new StorageServices().WayBillDetail(warehouseID, waybillID);
//        //    return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
//        //}

//        [HttpPost]
//        public ActionResult Enter(object obj)
//        {

//            try
//            {
//                var waybill = obj.ToString().JsonTo<TempStorageWaybill>();
//                if (waybill.ProductStorages.Length == 0 && waybill.SummaryStorages.Length == 0)
//                {
//                    return Json(new { val = message, msg = message.GetDescription() });
//                }

//                var warehouse = Yahv.Erp.Current.WareHourse;
//                warehouse.TempStorageSuccess += Storage_Success;
//                warehouse.TempStorageFailed += Storage_Failed;

//                warehouse.TempStorageEnter(waybill);
//                return Json(new { val = message, msg = message.GetDescription() }); ;
//            }
//            catch (Exception ex)
//            {
//                return Json(new { val = Message.Failed, msg = Message.Failed.GetDescription() });
//            }

//        }

//        private void Storage_Failed(object sender, ErrorEventArgs e)
//        {
//            message = Message.Failed;
//        }

//        private void Storage_Success(object sender, SuccessEventArgs e)
//        {
//            message = Message.Success;
//        }
//    }
//}