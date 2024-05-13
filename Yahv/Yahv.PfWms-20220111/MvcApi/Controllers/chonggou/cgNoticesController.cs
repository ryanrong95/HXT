using Yahv.Utils.EventExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using Wms.Services.Models;
using Yahv.Linq.Extends;
using Yahv.Underly;
using Yahv.Underly.Attributes;
using Yahv.Usually;
using Yahv.Services.Views;
using MvcApi.Models;
using Yahv.Utils.Kdn;
using Kdn.Library;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;
using Wms.Services.chonggous.Views;
using Newtonsoft.Json.Linq;
using Yahv.Services.Enums;

namespace MvcApp.Controllers
{
    public class cgNoticesController : Controller
    {

        

        /// <summary>
        /// 入库通知
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult In(JPost jpost)
        {
            try
            {
                using (CgInNoticesView view = new CgInNoticesView())
                {
                    LitTools.Current["调用入库通知Json"].Log(jpost.ToObject<object>().ToString());
                    var waybill = jpost["Waybill"];
                    var enter = jpost["Enter"];
                    var delete = jpost["Delete"];

                    JToken notices = null;
                    if (enter != null)
                    {
                        notices = enter["Notices"];
                    }
                    else
                    {
                        notices = jpost["Notices"];
                    }

                    view.EnterWaybill(waybill);
                    view.Enter(notices, waybill["WaybillID"].Value<string>());

                    //不再考虑此种情况
                    //if (delete != null)
                    //{
                    //    view.Delete(delete);
                    //}
                }

                var result = Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });
                LitTools.Current["调用入库通知结果"].Log(result.Data.ToString());
                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });
                LitTools.Current["调用入库通知结果"].Log(result.Data.ToString());
                return result;
            }
        }

        /// <summary>
        /// 装箱通知, // 只有来源是转报关，并且通知类型是装箱的才会调用此接口
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Boxing(JPost jpost)
        {
            try
            {
                using (CgBoxingNoticesView boxingView = new CgBoxingNoticesView())
                {
                    LitTools.Current["调用装箱通知Json"].Log(jpost.ToObject<object>().ToString());
                    var waybill = jpost["Waybill"];
                    var enter = jpost["Enter"];
                    var notices = enter["Notices"];
                    //var delete = jpost["Delete"];

                    boxingView.EnterBoxing(waybill, notices);

                    //if (delete != null)
                    //{
                    //    boxingView.Delete(delete);
                    //}

                    var result = Json(new
                    {
                        Success = true,
                        Data = string.Empty
                    });

                    LitTools.Current["调用装箱通知结果"].Log(result.Data.ToString());
                    return result;
                }
            }
            catch (Exception ex)
            {
                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });
                LitTools.Current["调用装箱通知结果"].Log(result.Data.ToString());
                return result;
            }
        }

        /// <summary>
        /// 生成香港出库
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Out(JPost jpost)
        {
            try
            {
                using (CgOutNoticesView view = new CgOutNoticesView())
                {
                    LitTools.Current["调用出库通知Json"].Log(jpost.ToObject<object>().ToString());
                    var waybill = jpost["Waybill"];
                    var Notices = jpost["Notices"];

                    var waybillid = view.WaybillEnter(waybill);
                   
                    view.OutNoticeEnter(Notices, waybillid);
                }
                var result = Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });
                LitTools.Current["调用出库通知结果"].Log(result.Data.ToString());

                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });

                LitTools.Current["调用出库通知结果"].Log(result.Data.ToString());

                return result;
            }
        }

        /// <summary>
        /// 深圳出库通知的时候调用
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SZOut(JPost jpost)
        {
            try
            {
                using (CgOutNoticesView view = new CgOutNoticesView())
                {
                    LitTools.Current["调用深圳出库通知Json"].Log(jpost.ToObject<object>().ToString());
                    var waybill = jpost["Waybill"];
                    var Notices = jpost["Notices"];
                    var waybillid = view.SZWaybillEnter(waybill);
                    view.OutNoticeEnter(Notices, waybillid);
                }
                var result = Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });
                LitTools.Current["调用深圳出库调用出库通知结果"].Log(result.Data.ToString());

                return result;
            }
            catch (Exception ex)
            {
                var result = Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });

                LitTools.Current["调用深圳出库通知结果"].Log(result.Data.ToString());

                return result;
            }
        }

        /// <summary>
        /// 取消订单的出库通知
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancleOutNoitce(string id)
        {
            try
            {
                using (CgOutNoticesView view = new CgOutNoticesView())
                {
                    view.OutNoticeCancel(id);
                }
                return Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Success = false,
                    Data = ex.Message,
                });
            }
        }
    }
}
