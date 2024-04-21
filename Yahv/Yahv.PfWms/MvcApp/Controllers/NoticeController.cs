using Needs.Linq;
using Needs.Utils.Descriptions;
using Needs.Utils.EventExtend;
using Needs.Utils.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Needs.Utils.Serializers;
using Needs.Linq.Extends;
using System.Globalization;
using Needs.Utils.Converters;

namespace MvcApp.Controllers
{
    public class NoticeController : Controller
    {
        enum Message
        {
            [Description("成功")]
            Success = 0,
            [Description("失败")]
            Fail = 1,
            [Description("请先配置库房编号")]
            WarehouseIsNull = 2,
            [Description("状态值有误")]
            StatusError = 3,
            [Description("类型有误")]
            TypeError = 4,
            [Description("时间格式有误")]
            TimeFormatError = 5,
            [Description("数量为0,通知无效")]
            NoticeInvalid = 6,
            [Description("产品编号不能为空")]
            ProductIDEmpty = 7,
            [Description("运单编号不能为空")]
            WaybillIDEmpty = 8,
        }

        Message message;

        /// <summary>
        /// 获取通知
        /// </summary>
        /// <param name="type">通知类型</param>
        /// <param name="warehouseID">所在库房</param>
        /// <param name="key">运单号/入仓号/ID</param>
        /// <param name="supplier">供应商</param>
        /// <param name="status">通知状态</param>
        /// <param name="starttime">开始日期</param>
        /// <param name="endtime">截止日期</param>
        /// <param name="orderID">订单编号</param>
        /// <param name="partNumber">产品型号</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(string warehouseID = null, Wms.Services.Enums.NoticesType? type = null, string key = null, string supplier = null, Wms.Services.Enums.NoticesStatus? status = null, string starttime = null, string endtime = null, string orderID = null, string partNumber = null, string customerServiceID = null, int PageIndex = 1, int PageSize = 20)
        {
            //库房编号不能为空
       
            Expression<Func<Notices, bool>> exp = item=>true;

            if (!string.IsNullOrEmpty(warehouseID))
            {
                exp = exp.And(item => item.WareHouseID == warehouseID);
            }

            if (type != null && Enum.IsDefined(typeof(Wms.Services.Enums.NoticesType), type))
            {
                exp = exp.And(item => item.Type == type);
            }

            //验证状态
            if (status != null)
            {
                if (!Enum.IsDefined(typeof(Wms.Services.Enums.NoticesStatus), status))
                {
                    return Json(new { val = (int)Message.StatusError, msg = Message.StatusError.GetDescription() });
                }
                exp = exp.And(item => item.Status == status);
            }

            #region 验证时间
            //验证时间
            DateTime? startDate, EndDate;
            DateExtend.DateHandle(starttime, endtime, out startDate, out EndDate);

            if (startDate != null)
            {
                exp = exp.And(item => item.CreateDate >= startDate);
            }

            if (EndDate != null)
            {
                exp = exp.And(item => item.CreateDate < EndDate);
            }


            //验证运单号/入仓号/ID
            if (!string.IsNullOrEmpty(key))
            {
                exp = exp.And(item => item.Waybills.EnterCode == key || item.Waybills.Code == key || item.ID == key);
            }

            //验证供应商
            if (!string.IsNullOrWhiteSpace(supplier))
            {
                exp = exp.And(item => item.Inputs.Supplier.Contains(supplier));
            }


            //验证orderID 
            if (!string.IsNullOrWhiteSpace(orderID))
            {
                exp = exp.And(item => item.Inputs.OrderID == orderID);
            }

            //验证产品型号
            if (!string.IsNullOrWhiteSpace(partNumber))
            {
                exp = exp.And(item => item.StandardProducts.PartNumber == partNumber);
            }

            //验证采购员
            if (!string.IsNullOrEmpty(customerServiceID))
            {
                exp = exp.And(item => item.Inputs.CustomerServiceID == customerServiceID);
            }

            #endregion

            return Json(new { obj = new Wms.Services.Views.NoticesView().GroupBy(item=>item.WaybillID).AsEnumerable().Select(item=>item.First()).Where(exp.Compile()).Page(PageIndex, PageSize) }, JsonRequestBehavior.AllowGet);
        }

       

        [HttpPost]
        public ActionResult Index(Notices datas)
        {
            try
            {
                if (datas.ID == null)
                {
                    //库房编号不能为空
                    if (string.IsNullOrWhiteSpace(datas.WareHouseID))
                    {
                        //请先配置库房ID
                        return Json(new { val = (int)Message.WarehouseIsNull, msg = Message.WarehouseIsNull.GetDescription() });
                    }
                    if (datas.Quantity == 0)
                    {
                        //通知数量为0，通知无效
                        return Json(new { val = (int)Message.NoticeInvalid, msg = Message.NoticeInvalid.GetDescription() });
                    }
                    if (string.IsNullOrWhiteSpace(datas.ProductID))
                    {
                        //产品ID为空
                        return Json(new { val = (int)Message.ProductIDEmpty, msg = Message.ProductIDEmpty.GetDescription() });
                    }
                    if (string.IsNullOrWhiteSpace(datas.Waybills.Code))
                    {
                        //运单编号为空
                        return Json(new { val = (int)Message.WaybillIDEmpty, msg = Message.WaybillIDEmpty.GetDescription() });
                    }
                }
                //.
                datas.AddEvent("NoticeSuccess", new SuccessHandler(Datas_NoticeSuccess))
                    .AddEvent("NoticeFailed", new ErrorHandler(Datas_NoticeFailed))
                    .Enter();
                return Json(new { val = (int)message, msg = message.GetDescription() });
            }
            catch
            {
                return Json(new { val = (int)Message.Fail, msg = Message.Fail.GetDescription() });
            }

        }

        private void Datas_NoticeFailed(object sender, ErrorEventArgs e)
        {
            message = Message.Fail;
        }

        private void Datas_NoticeSuccess(object sender, SuccessEventArgs e)
        {
            message = Message.Success;
        }
    }
}