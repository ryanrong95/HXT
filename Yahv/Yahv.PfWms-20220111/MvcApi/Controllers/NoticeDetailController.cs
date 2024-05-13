//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Web;
//using System.Web.Mvc;
//using Wms.Services.Models;
//using Wms.Services.Views;
//using Yahv.Linq.Extends;

//namespace MvcApp.Controllers
//{
//    public class NoticeDetailController : Controller
//    {
//        // GET: NoticeDetail
//        /// <summary>
//        /// 根据运单得到分拣详情
//        /// </summary>
//        /// <param name="wayBillID">运单编号</param>
//        /// <param name="key">搜索关键词</param>
//        /// <param name="pageIndex">当前页</param>
//        /// <param name="pageSize">页大小</param>
//        /// <url>/api/noticedetail</url>
//        /// <returns></returns>
//        public ActionResult Index(string wayBillID,string key=null,int pageIndex = 1, int pageSize = 20)
//        {
//            Expression<Func<Notices, bool>> exp = item => item.WaybillID == wayBillID;


//            if (!string.IsNullOrEmpty(key))
//            {
//                exp = exp.And(item => item.StandardProducts.PartNumber.StartsWith(key) || item.StandardProducts.Manufacturer.StartsWith(key) || item.DateCode == key);
//            }

//            var list = new NoticesView().AsEnumerable().Where(exp.Compile());
//            if (list.Count() <= 0)
//            {
//                return Json(new
//                {
//                    obj =new { },waybill =new { },Summay=new { }
//                }, JsonRequestBehavior.AllowGet);
//            }
           
                
//            var first = list.First();

//            var files = new FileInfosView().Where(item => item.WaybillID == wayBillID).ToArray();
         
//            var Supplier = string.Join(",", list.Select(item => item.Supplier).Distinct().ToArray());

//            return Json(new
//            {
//                obj = list.Select(item => new { item.ID, item.WaybillID, item.WareHouseID, item.Type, item.TypeDes, item.ProductID, item.StandardProducts, item.Inputs, item.BoxCode, item.InputID, item.Quantity, item.SortedQuantity, TruetoQuantity = item.Quantity, item.ShelveID, item.Weight, item.Volume, FileInfos=new FileInfosView().Where(tem=>tem.NoticeID==item.ID), Conditions = item.Conditions ?? new NoticeCondition() { }, item.Status, item.StatusDes, PID = item.PID ?? item.ID ?? "", Supplier=item.Supplier??"" }).Paging(pageIndex, pageSize),
//                waybill = new WaybillsView().AsEnumerable().Where(item => item.ID == wayBillID).Select(item => new
//                {
//                    item.ID,
//                    item.Code,
//                    item.CarrierID,
//                    item.Condition,
//                    item.ConsigneeID,
//                    item.ConsignorID,
//                    item.CreatorID,
//                    item.CarrierAccount,
//                    item.EnterCode,
//                    FatherID = item.FatherID ?? "",
//                    FileInfos= files,
//                    item.FreightPayer,
//                    item.TransferID,
//                    item.ModifierID,
//                    item.TotalWeight,
//                    item.CreateDate,
//                    item.ModifyDate,
//                    item.TotalParts,
//                    item.Status,
//                    item.Subcodes,
//                    item.VoyageNumber,
//                    item.Type,
//                    item.TypeDes,
//                    item.OStatus,
//                    item.OStatusDes,
//                    BType = first.Type,
//                    BTypedes = first.TypeDes,
//                    first.Inputs.OrderID,
//                    Client = new { Name = item.Client.Name ?? "" },
//                    item.CorPlace,
//                    item.CorPlaceDes,
//                    Supplier

//                }).FirstOrDefault(),
//                Summary = new Summaries { ID = wayBillID, Otype = 100, Title = "", Summary = "" }
//            }, JsonRequestBehavior.AllowGet);; ;

//        }



//        public ActionResult Index1(string wayBillID=null, string key = null, int pageIndex = 1, int pageSize = 20)
//        {
//            var list = new SortingNoticesRoll();

//            var files = new FileInfosView().Where(item => item.WaybillID == wayBillID).ToArray();

//            return Json(new
//            {
//                obj = list.Select(item => new {item.ID, item.WaybillID, item.WareHouseID, item.Type, item.TypeDes, item.ProductID, item.StandardProducts, item.Inputs, item.BoxCode, item.InputID, item.Quantity, item.SortedQuantity, TruetoQuantity = item.Quantity, item.ShelveID, item.Weight, item.Volume, FileInfos = new FileInfosView().Where(tem => tem.NoticeID == item.ID), Conditions = item.Conditions ?? new NoticeCondition() { }, item.Status, item.StatusDes, PID = item.PID ?? item.ID ?? "", Supplier = item.Supplier  }),
//                waybill = new WaybillsView().Where(item => item.ID == wayBillID).Select(item => new
//                {
//                    item.ID,
//                    item.Code,
//                    item.CarrierID,
//                    item.Condition,
//                    item.ConsigneeID,
//                    item.ConsignorID,
//                    item.CreatorID,
//                    item.CarrierAccount,
//                    item.EnterCode,
//                    FatherID = item.FatherID ?? "",
//                    FileInfos = files,
//                    item.FreightPayer,
//                    item.TransferID,
//                    item.ModifierID,
//                    item.TotalWeight,
//                    item.CreateDate,
//                    item.ModifyDate,
//                    item.TotalParts,
//                    item.Status,
//                    item.Subcodes,
//                    item.VoyageNumber,
//                    item.Type,
//                    item.TypeDes,
//                    item.OStatus,
//                    item.OStatusDes,
//                    //BType = first.Type,
//                    //BTypedes = first.TypeDes,
//                    //first.Inputs.OrderID,
//                    Client = new { Name = item.Client.Name ?? "" },
//                    item.CorPlace,
//                    item.CorPlaceDes,
                 

//                }
//                ).FirstOrDefault(),
//                Summary = new Summaries { ID = wayBillID, Otype = 100, Title = "", Summary = "" }
//            }, JsonRequestBehavior.AllowGet);

//        }

        
//    }
//}