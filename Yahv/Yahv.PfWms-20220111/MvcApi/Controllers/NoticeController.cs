using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using System.Globalization;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Utils.EventExtend;
using Yahv.Underly;
using Yahv.Utils.Converters;
using Yahv.Linq.Extends;
using Wms.Services.Views;

namespace MvcApp.Controllers
{
    public class NoticeController : Controller
    {
        enum Message
        {
            [Description("入库成功")]
            Success = 0,
            [Description("入库失败")]
            Fail = 1,
            //[Description("请先配置库房编号")]
            //WarehouseIsNull = 2,
            //[Description("状态值有误")]
            //StatusError = 3,
            //[Description("类型有误")]
            //TypeError = 4,
            //[Description("时间格式有误")]
            //TimeFormatError = 5,
            //[Description("数量为0,通知无效")]
            //NoticeInvalid = 6,
            //[Description("产品编号不能为空")]
            //ProductIDEmpty = 7,
            //[Description("运单编号不能为空")]
            //WaybillIDEmpty = 8,
        }

        Message message;

        /// <summary>
        /// 获取通知
        /// </summary>
        /// <param name="key">箱号（ID）/运输批次号</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public ActionResult Index(string warehouseID, string key, int pageIndex = 1, int pageSize = 10)
        {

            return Json(new { obj = new Wms.Services.NoticeServices().GetNotices(warehouseID.ToUpper(), key, pageIndex, pageSize) }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult Index(string warehouseID, string shelveID, PDANotices[] notices)
        {
            try
            {
                var warehouse = Yahv.Erp.Current.WareHourse;
                warehouse.NoticeSuccess += Warehouse_NoticeSuccess;
                warehouse.NoticeFailed += Warehouse_NoticeFailed;
                warehouse.NoticeEnter(warehouseID, shelveID.ToUpper(), notices);
                return Json(new { val = message, msg = message.GetDescription() });
            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }

        }

        [HttpPost]
        public ActionResult Enter(string warehouseID,string shelveID,string key)
        {
            try
            {
                var warehouse = Yahv.Erp.Current.WareHourse;
                warehouse.NoticeSuccess += Warehouse_NoticeSuccess;
                warehouse.NoticeFailed += Warehouse_NoticeFailed;
                warehouse.NoticeEnter(warehouseID, shelveID.ToUpper(), key);
                return Json(new { val = message, msg = message.GetDescription() });
            }
            catch(Exception ex)
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }
        }

        private void Warehouse_NoticeFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Warehouse_NoticeSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }

        //private void Datas_NoticeFailed(object sender, ErrorEventArgs e)
        //{
        //    message = Message.Fail;
        //}

        //private void Datas_NoticeSuccess(object sender, SuccessEventArgs e)
        //{
        //    message = Message.Success;
        //}
    }
}