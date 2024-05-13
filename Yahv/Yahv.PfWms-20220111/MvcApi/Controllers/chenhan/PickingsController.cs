using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using System.Linq.Expressions;
using Wms.Services.Enums;

using Yahv.Usually;
using Yahv.Underly.Attributes;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Web.Mvc.Filters;
using Yahv.Web.Mvc;
using Wms.Services.Views;
using Wms.Services;
using Yahv.Services.Models.PvCenter;
using Yahv.Utils.Serializers;

namespace MvcApp.Controllers
{
    public class PickingsController : Controller
    {

        [HttpPost]
        public ActionResult UPChcd(JPost obj)
        {

            try
            {

                var entity = obj.ToObject<Voyage>();

                //登录验证先去掉
                //var wh = Yahv.Erp.Current.WareHourse;
                //wh.UpdateVoyage(entity);

                entity.Enter();
                return Json(new { val = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { val = 400, msg = "操作失败！" + ex.Message });

            }

        }

        /// <summary>
        /// 无 action 表示 获取数据
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult Index(string warehouseid, string key = null, string partnumber = null, string CustomName = null, string startdate = null, string enddate = null, string status = null, int source = 0, int? ntype = null, int pageindex = 1, int pagesize = 20)
        {

            var data = new WayBillServices().PickWayBills(warehouseid, key, partnumber, CustomName, startdate, enddate, status, source, ntype, pageindex, pagesize);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);


        }


        virtual public ActionResult Detail(string warehouseid, string waybillid, string key = null)
        {

            var data = new WayBillServices().PickWayBill(warehouseid, waybillid, key);
            return Json(new { obj = data, Status = 100 }, JsonRequestBehavior.AllowGet);

        }



        /// <summary>
        /// 有action 表示提交方法
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        virtual public ActionResult Enter(object obj, string summary, int status, string token = null)
        {

            try
            {
                var wh = Yahv.Erp.Current.WareHourse;
                wh.WaybillClosed += Wh_WaybillClosed;
                wh.LackStockEvent += Wh_LackStockEvent;
                var waybill = obj.ToString().JsonTo<Wms.Services.Models.PickingWaybill>();
                //if (waybill.Notices.First().Source==Yahv.Services.Enums.NoticeSource.AgentBreakCustoms)
                //{
                //    if (waybill.CuttingOrderStatus == CuttingOrderStatus.Waiting)
                //    {
                //        return Json(new { val = 410, msg = "报关未截单,不能出库!" });
                //    }
               
                
                wh.PickingEnter(waybill, summary, status);

               

            }
            catch (Exception ex)
            {
                return Json(new { val = 4, msg = "出库失败"+ex.Message });

            }

            if (LackStoc)
            {
                return Json(new { val = 300, msg = "库存不足" });
            }

            if (waybillCodeStatus)
            {
                return Json(new { val = 400, msg = "订单关闭" });
            }
            return Json(new { val = 0, msg = "出库成功" });

        }

        bool LackStoc = false;
        private void Wh_LackStockEvent(object sender, EventArgs e)
        {
            LackStoc = true;
        }

        bool waybillCodeStatus = false;
        private void Wh_WaybillClosed(object sender, EventArgs e)
        {
            waybillCodeStatus = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waybillids">如果有多个，请用 , 分隔</param>
        /// <param name="code">运单号</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateWayBillCode(string waybillids, string code)
        {
            try
            {
                Yahv.Erp.Current.WareHourse.UpdateWaybillCode(waybillids, code);
                //new Wms.Services.WayBillServices().UpdateWayBillCode(waybillids, code);
                return Json(new JMessage { success = true, code = 1, data = "保存成功！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JMessage { success = false, code = 0, data = "保存失败！" }, JsonRequestBehavior.AllowGet);
            }
        }


        int code = 200;
        string mess = "";
        [HttpPost]
        virtual public ActionResult Submit(JPost obj = null)
        {


            try
            {
                var waybill = obj.ToObject<PickingWaybill>();
                waybill.LackStockEvent += Pwb_LackStockEvent;
                waybill.Submit();
                // new PickingWaybill().Submit();
            }
            catch (Exception ex)
            {
                code = 300;
                mess = ex.Message + "--" + ex.Source;
            }

            switch (code)
            {
                case 200:
                    return Json(new JMessage
                    {
                        success = true,
                        code = 200,
                        data = "提交成功！"
                    }, JsonRequestBehavior.AllowGet);
                case 300:
                    return Json(new JMessage
                    {
                        success = false,
                        code = 300,
                        data = "提交失败！" + mess
                    }, JsonRequestBehavior.AllowGet);
                case 400:
                    return Json(new JMessage
                    {
                        success = false,
                        code = 400,
                        data = "缺少库存！"
                    }, JsonRequestBehavior.AllowGet);
                default:
                    return Json(new JMessage
                    {
                        success = false,
                        code = 300,
                        data = "提交失败！"
                    }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public ActionResult UpdateCuttingOrderStatus(JPost obj)
        {
            try
            {

                var entity = obj.ToObject<CuttingOrderParams>();
                var wh = Yahv.Erp.Current.WareHourse;
                wh.UpdateCuttingOrderStatus(entity.waybillIDs, entity.CuttingOrderStatus)
;
                return Json(new { val = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { val = 400, msg = "操作失败！" + ex.Message });

            }

        }

        [HttpPost]
        public ActionResult CustomsApply(JPost obj)
        {
            try
            {
                var entity = obj.ToObject<DeclearCustom>();

                Yahv.Erp.Current.WareHourse.CustomsApply(entity.WHID,entity.BoxIds);

                return Json(new { val = 200, msg = "操作成功！" });
            }
            catch (Exception ex)
            {
                return Json(new { val = 400, msg = "操作失败！" + ex.Message });

            }

        }



        [HttpPost]
        /// <summary>
        /// 确认出库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        virtual public ActionResult Confirm(string orderid, string itemid = null)
        {
            try
            {
                new PickingWaybill().ConfirmOrder(orderid, itemid);
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "提交成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        /// <summary>
        /// 取消出库
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        virtual public ActionResult Cancel(string orderid, string itemid = null)
        {
            try
            {
                new PickingWaybill().CancelOrder(orderid, itemid);
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "提交成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        bool outOfStock = false;

        [HttpPost]
        public virtual ActionResult LockStore(JPost obj)
        {
            try
            {
                var pwb = new PickingWaybill();
                pwb.OutOfStock += Pwb_OutOfStock;
                pwb.LockStore(obj.ToObject<Yahv.Services.Models.LockStoreParam>());

                if (outOfStock)
                {
                    return Json(new JMessage
                    {
                        success = false,
                        code = 400,
                        data = "库存不足！"
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "提交成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！" + ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private void Pwb_OutOfStock(object sender, EventArgs e)
        {
            outOfStock = true;
        }

        [HttpPost]
        public virtual ActionResult CancelLockStore(JPost obj)
        {
            try
            {

                new PickingWaybill().CancelLockStore(obj.ToObject<Yahv.Services.Models.LockStoreParam>());
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "提交成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        virtual public ActionResult Check(string outputID)
        {
            try
            {
                Yahv.Erp.Current.WareHourse.Check(outputID);
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "提交成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        private void Pwb_LackStockEvent(object sender, EventArgs e)
        {
            code = 400;

        }

        int val = 0;
        [HttpPost]
        /// <summary>
        /// 删除未出库的出库通知
        /// </summary>
        /// <param name="noticeID"></param>
        /// <returns></returns>
        virtual public ActionResult Delete(string noticeID)
        {
            try
            {
                var warehouse = Yahv.Erp.Current.WareHourse;
                warehouse.NoticeNotExist += Warehouse_NoticeNotExist;
                warehouse.DeleteOutStock(noticeID);
            }
            catch (Exception ex)
            {
                return Json(new { val = "1", msg = "删除失败" }, JsonRequestBehavior.AllowGet);
            }
            string msg = "删除成功";
            if (val == 2)
            {
                msg = "通知数据不存在";
            }
            return Json(new { val = val, msg = msg }, JsonRequestBehavior.AllowGet);
        }

        private void Warehouse_NoticeNotExist(object sender, ErrorEventArgs e)
        {
            val = 2;
        }


        /// <summary>
        /// 出库拣货通知
        /// </summary>
        /// <param name="warehouseID">库房编号</param>
        /// <param name="key">型号/制造商</param>
        /// <param name="waybillID">运单编号</param>
        /// <param name="tinyOrderID">小订单编号</param>
        /// <param name="vastOrderID">订单编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public ActionResult GetPickingNotice(string warehouseID, string key = null, string waybillID = null, string tinyOrderID = null, string vastOrderID = null, int pageIndex = 1, int pageSize = 20)
        {
            //Expression<Func<Yahv.Services.Models.PickingNotice, bool>> exp = item => item.WareHouseID == warehouseID.ToUpper();

            //if (!string.IsNullOrWhiteSpace(key))
            //{
            //    exp = PredicateExtends.And(exp, item => item.Product.Manufacturer.Contains(key) || item.Product.PartNumber.Contains(key));
            //}

            //if (!string.IsNullOrWhiteSpace(waybillID))
            //{
            //    exp = PredicateExtends.And(exp, item => item.WaybillID == waybillID);
            //}

            //if (!string.IsNullOrWhiteSpace(tinyOrderID))
            //{
            //    exp = PredicateExtends.And(exp, item => item.Output.TinyOrderID == tinyOrderID);
            //}
            //if (!string.IsNullOrWhiteSpace(vastOrderID))
            //{
            //    exp = PredicateExtends.And(exp, item => item.Output.OrderID == vastOrderID);
            //}

            //var data = new Wms.Services.Views.PickingNoticesView().Where(exp).Paging(pageIndex, pageSize);

            //return Json(new { obj = data }, JsonRequestBehavior.AllowGet);


            var data = new WayBillServices().GetPickingNotice(warehouseID.ToUpper(), key, waybillID, tinyOrderID, vastOrderID, pageIndex, pageSize);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public ActionResult UpdateFile(string id,string waybillID,string customName,int type)
        {
            try
            {
                //1.累计运单状态，不可逆，判断运单状态不是200就改成200，否则不予修改，运单修改一下，累计logs_waybill 状态
                //2.控制台确认收货
                var wh = Yahv.Erp.Current.WareHourse;
                wh.UpdateFile(id, waybillID, customName, type);
                return Json(new { val = 0, msg = "上传成功" });
            }
            catch 
            {
                return Json(new { val = 1, msg = "上传失败" });
            }

        }

       [HttpPost]
       public ActionResult DeleteFile(string id)
        {
            try
            {
                var wh = Yahv.Erp.Current.WareHourse;
                wh.DeleteFile(id);
                return Json(new { val = 0, msg = "删除成功" });
            }
            catch 
            {
                return Json(new { val = 1, msg = "删除失败" });
            }
        }

        public class CuttingOrderParams
        {
            public string[] waybillIDs { get; set; }
            public int CuttingOrderStatus { get; set; }
        }


        public class DeclearCustom
        {
            public string WHID { get; set; }
            public string[] BoxIds { get; set; }
        }

        public class InfoByInput {
            public int InputID { get; set; }
            public decimal Weight { get; set; }

        }
    }
}


