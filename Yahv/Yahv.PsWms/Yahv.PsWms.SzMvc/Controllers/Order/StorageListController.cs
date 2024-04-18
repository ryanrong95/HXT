using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PsWms.SzMvc.Models;
using Yahv.PsWms.SzMvc.Services.Enums;
using Yahv.PsWms.SzMvc.Services.Views;

namespace Yahv.PsWms.SzMvc.Controllers
{
    public partial class OrderController : BaseController
    {
        #region 页面

        /// <summary>
        /// 订单列表
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageList(string tabNo)
        {
            string[] prepareTabNos = new string[] { "1", "2" };
            if (!prepareTabNos.Contains(tabNo))
            {
                tabNo = "1";
            }

            ViewBag.tabNo = tabNo;
            return View();
        }

        /// <summary>
        /// 订单列表-入库订单
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageListIn() { return PartialView(); }

        /// <summary>
        /// 订单列表-出库订单
        /// </summary>
        /// <returns></returns>
        public ActionResult StorageListOut() { return PartialView(); }

        #endregion


        /// <summary>
        /// 获取入库订单列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageListIn(GetStorageListInModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theSiteuserID = siteuser.SiteuserID;

            using (var query = new InStorageListView())
            {
                var view = query;
                view = view.SearchBySiteuserID(theSiteuserID);

                if (!string.IsNullOrEmpty(searchModel.OrderStatusInt))
                {
                    view = view.SearchByOrderStatus((OrderStatus)(Convert.ToInt32(searchModel.OrderStatusInt)));
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateBegin))
                {
                    DateTime begin = DateTime.Parse(searchModel.CreateDateBegin);
                    view = view.SearchByCreateDateBegin(begin);
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateEnd))
                {
                    DateTime end = DateTime.Parse(searchModel.CreateDateEnd);
                    end = end.AddDays(1);
                    view = view.SearchByCreateDateEnd(end);
                }

                if (!string.IsNullOrWhiteSpace(searchModel.OrderID))
                {
                    searchModel.OrderID = searchModel.OrderID.Trim();
                    view = view.SearchByOrderID(searchModel.OrderID);
                }

                if (!string.IsNullOrWhiteSpace(searchModel.PartNumber))
                {
                    searchModel.PartNumber = searchModel.PartNumber.Trim();
                    view = view.SearchByPartNumber(searchModel.PartNumber);
                }

                return Json(new { type = "", msg = "", data = view.ToMyPage(searchModel.page, searchModel.rows) }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取出库订单列表数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetStorageListOut(GetStorageListOutModel searchModel)
        {
            var siteuser = Yahv.PsWms.SzMvc.SiteCoreInfo.Current;
            string theSiteuserID = siteuser.SiteuserID;

            using (var query = new OutStorageListView())
            {
                var view = query;
                view = view.SearchBySiteuserID(theSiteuserID);

                if (!string.IsNullOrEmpty(searchModel.OrderStatusInt))
                {
                    view = view.SearchByOrderStatus((OrderStatus)(Convert.ToInt32(searchModel.OrderStatusInt)));
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateBegin))
                {
                    DateTime begin = DateTime.Parse(searchModel.CreateDateBegin);
                    view = view.SearchByCreateDateBegin(begin);
                }

                if (!string.IsNullOrEmpty(searchModel.CreateDateEnd))
                {
                    DateTime end = DateTime.Parse(searchModel.CreateDateEnd);
                    end = end.AddDays(1);
                    view = view.SearchByCreateDateEnd(end);
                }

                if (!string.IsNullOrWhiteSpace(searchModel.OrderID))
                {
                    searchModel.OrderID = searchModel.OrderID.Trim();
                    view = view.SearchByOrderID(searchModel.OrderID);
                }

                if (!string.IsNullOrWhiteSpace(searchModel.PartNumber))
                {
                    searchModel.PartNumber = searchModel.PartNumber.Trim();
                    view = view.SearchByPartNumber(searchModel.PartNumber);
                }

                return Json(new { type = "", msg = "", data = view.ToMyPage(searchModel.page, searchModel.rows) }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}