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
using Wms.Services;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc.Filters;
using Yahv.Web.Mvc;

namespace MvcApp.Controllers
{
    public class SortingsController : ClientController
    {

        /// <summary>
        /// 无 action 表示 获取数据
        /// </summary>
        /// <returns></returns>
        virtual public ActionResult Index(string warehouseid, string key = null, string partnumber = null, string supplier = null, string startdate = null, string enddate = null, int? status = null, int source = 0, int? ntype = null, int pageindex = 1, int pagesize = 20)
        {

            var data = new WayBillServices().WayBills(warehouseid, key, partnumber, supplier, startdate, enddate, status, source, ntype, pageindex, pagesize);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);

        }


        virtual public ActionResult Detail(string warehouseid, string waybillid, string key = null)
        {
            var data = new WayBillServices().WayBill(warehouseid, waybillid, key);
            return Json(new { obj = data, Status = 100 }, JsonRequestBehavior.AllowGet);

        }


        virtual public ActionResult History(string waybillid)
        {
            var data = new WayBillServices().HistoryToWaybillCode_Date(waybillid);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        virtual public ActionResult HistoryDetail(string waybillid)
        {
            // var data = new WayBillServices().HistoryDetailByWaybillCode_Date(waybillid, orderid);

            var data = new WayBillServices().HistoryDetail(waybillid);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        virtual public ActionResult NewInput()
        {
            return Json(new { id = new WayBillServices().GetInputID() }, JsonRequestBehavior.AllowGet);
        }


        //virtual public ActionResult Index(string id)
        //{
        //    //var s = new Summaries();
        //    //var data = new WayBillServices().WayBill(warehouseid, waybillid);
        //    //return Json(new { obj = data, Summary = new Summaries { ID = data.WaybillID, Title = "", Otype = 10, Summary = "" } }, JsonRequestBehavior.AllowGet);
        //    return null;
        //}

        /// <summary>
        /// 去拿货
        /// </summary>
        /// <param name="waybillid"></param>
        /// <returns></returns>
        [HttpPost]
        virtual public ActionResult TakeGoods(string waybillid)
        {
            try
            {
                Yahv.Erp.Current.WareHourse.TakeGoods(waybillid);
                return Json(new { val = 1, msg = "成功" });
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                return Json(new { val = 0, msg = "失败" });

            }

        }

        int enterError = 0;
        string returnInfo = "";
        bool NoBoxCode = false;

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
                wh.TransferEvent += Wh_TransferEvent;
                wh.NOArrivalEvent += Wh_NOArrivalEvent;
                wh.BreakCustomEvent += Wh_BreakCustomEvent;
                wh.NoBoxCodeEvent += Wh_NoBoxCodeEvent;
                wh.SortingEnter(obj, summary, status);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;
                return Json(new { val = 4, msg = "操作失败" });

            }

            if (transferType)
            {
                return Json(new { val = 0, msg = "操作成功", id = waybillid });
            }

            if (waybillCodeStatus)
            {
                return Json(new { val = 400, msg = "订单关闭" });
            }
            if (noArrival)
            {
                return Json(new { val = 500, msg = "没有填写到货数量" });
            }

            if (enterError == 600)
            {
                return Json(new { val = 600, msg = "实际到货数量不能大于通知的数量" });
            }

            if (NoBoxCode)
            {
                return Json(new { val = 610, msg = "没有指定箱号!" });
            }

            return Json(new { val = 0, msg = "操作成功" });

        }

        private void Wh_NoBoxCodeEvent(object sender, GeneralEventArgs e)
        {
            NoBoxCode = true;
        }

        private void Wh_BreakCustomEvent(object sender, GeneralEventArgs e)
        {
            enterError = 600;
            returnInfo = e.Values[0].ToString();
        }

        bool noArrival = false;
        private void Wh_NOArrivalEvent(object sender, GeneralEventArgs e)
        {
            noArrival = true;
        }

        bool transferType = false;
        string waybillid = null;
        private void Wh_TransferEvent(object sender, GeneralEventArgs e)
        {
            waybillid = e.Values[0].ToString();
            transferType = true;
        }

        bool waybillCodeStatus = false;
        private void Wh_WaybillClosed(object sender, EventArgs e)
        {
            waybillCodeStatus = true;
        }

        [HttpPost]
        virtual public ActionResult Submit(JPost jpost)
        {
            var service = new WayBillServices();
            try
            {
                var waybill = jpost.ToObject<SortingWaybill>();
                service.Submit(waybill);
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "提交失败！" + ex.Message + "--" + ex.Source
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new JMessage
            {
                success = true,
                code = 200,
                data = "提交成功！"
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        virtual public ActionResult UpdateItem(string inputID, string itemID = null, string tinyOrderID = null)
        {

            if (string.IsNullOrWhiteSpace(itemID) && string.IsNullOrWhiteSpace(tinyOrderID))
            {
                return Json(new { val = 0, msg = "操作成功" });
            }

            var service = new WayBillServices();
            try
            {
                service.UpdateItem(inputID, itemID, tinyOrderID);
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    success = false,
                    code = 300,
                    data = "修改失败！" + ex.Message + "--" + ex.Source
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new JMessage
            {
                success = true,
                code = 200,
                data = "修改成功！"
            }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 保存子运单号
        /// </summary>
        /// <param name="waybillID">运单编号</param>
        /// <param name="subCodes">子运单号（以“,”分隔）</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubCodeEnter(JPost post)
        {

            try
            {
                var service = new WayBillServices();
                var obj = post.ToObject<SubCodePara>();
                service.UpdateWayBillSubCode(obj.waybillID, obj.subCodes);
            }
            catch
            {
                return Json(new JMessage
                {
                    success = true,
                    code = 400,
                    data = "提交失败！"
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new JMessage
            {
                success = true,
                code = 200,
                data = "提交成功！"
            }, JsonRequestBehavior.AllowGet);
        }



        public ActionResult SubCode(string waybillID)
        {
            var service = new WayBillServices();

            var subCodes = service.GetWayBillSubCode(waybillID);

            return Json(subCodes, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 入库分拣通知
        /// </summary>
        /// <param name="warehouseID">库房编号</param>
        /// <param name="key">型号/制造商</param>
        /// <param name="waybillID">运单编号</param>
        /// <param name="tinyOrderID">小订单编号</param>
        /// <param name="vastOrderID">订单编号</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public ActionResult GetSortingNotice(string warehouseID, string key = null, string waybillID = null, string tinyOrderID = null, string vastOrderID = null, int pageIndex = 1, int pageSize = 20)
        {
            Expression<Func<Yahv.Services.Models.SortingNotice, bool>> exp = item => item.WareHouseID == warehouseID.ToUpper();


            if (!string.IsNullOrWhiteSpace(key))
            {
                exp = PredicateExtends.And(exp, item => item.Product.Manufacturer.Contains(key) || item.Product.PartNumber.Contains(key));
            }
            if (!string.IsNullOrWhiteSpace(waybillID))
            {
                exp = PredicateExtends.And(exp, item => item.WaybillID == waybillID);
            }

            if (!string.IsNullOrWhiteSpace(tinyOrderID))
            {
                exp = PredicateExtends.And(exp, item => item.Input.TinyOrderID == tinyOrderID);
            }
            if (!string.IsNullOrWhiteSpace(vastOrderID))
            {
                exp = PredicateExtends.And(exp, item => item.Input.OrderID == vastOrderID);
            }
            var data= new Wms.Services.Views.SortingNoticesView().Where(exp).Paging(pageIndex, pageSize);

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateInfoByInputID(JPost obj)
        {
            try
            {
                Yahv.Erp.Current.WareHourse.UpdateInfoByInputID(obj.ToObject<InfoByInput>());
                return Json(new { val = 200, msg = "操作成功" },JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { val = 400, msg = "操作失败" + ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

    }

    public class SubCodePara
    {
        public string waybillID { get; set; }
        public string subCodes { get; set; }

    }

}