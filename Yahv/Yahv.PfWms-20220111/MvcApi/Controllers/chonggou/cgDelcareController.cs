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
using Wms.Services.chonggous.Models;

namespace MvcApp.Controllers
{
    /// <summary>
    /// 申报
    /// </summary>
    public class cgDelcareController : Controller
    {

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="jpost">
        /// json 参数
        /// 只需要传递数组TinyOrderID数组
        /// </param>
        /// <returns>成功或是报错！</returns>
        [HttpPayload]
        public ActionResult GetList(JPost jpost)
        {
            var arguments = new
            {
                LotNumber = jpost["First"]?.Value<string>(),
                Packer = jpost["Packer"]?.Value<string>(),
                Status = jpost["Status"]?.Value<int>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
            };

            using (CgDelcaresView view = new CgDelcaresView())
            {
                CgDelcaresView data = view;
                if (!string.IsNullOrWhiteSpace(arguments.LotNumber))
                {
                    data = data.SearchByFirst(arguments.LotNumber);
                }

                if (!string.IsNullOrWhiteSpace(arguments.Packer))
                {
                    data = data.SearchByPacker(arguments.Packer);
                }

                if (arguments.Status.HasValue)
                {
                    data = data.SearchByStatus((Wms.Services.Enums.TinyOrderDeclareStatus)arguments.Status);
                }

                var results = data.ToMyPage(arguments.pageIndex, arguments.pageSize);

                return Json(new
                {
                    obj = results
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 获取具体小订单信息
        /// </summary>
        /// <param name="id">小订单ID</param>
        /// <returns>小订单信息</returns>
        public ActionResult Detail(string id)
        {
            using (CgDelcaresView data = new CgDelcaresView().SearchByID(id))
            {
                return Json(data.FirstOrDefault(), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 申报
        /// </summary>
        /// <param name="jarry">
        /// json 参数
        /// 只需要传递数组TinyOrderID数组
        /// </param>
        /// <returns>成功或是报错！</returns>
        [HttpPayload]
        public ActionResult Declare(JPost jpost)
        {
            //乔霞会通知勾选的 TinyOrderID
            //由于沈忱要求与乔霞沟通 完成添加adminID
            var adminID = jpost["AdminID"].ToObject<string>();
            var arry = jpost["TinyOrderID"].ToObject<string[]>();
            string url = Wms.Services.FromType.CustomApply.GetDescription();

            try
            {
                using (CgDelcaresView view = new CgDelcaresView())
                {
                    view.Delcaring(adminID, arry);
                    //这里会把以上的 TinyOrderID 发送给 http://api0.for-ic.net/Declaration/DeclarationNotice
                    Yahv.Utils.Http.ApiHelper.Current.JPost(url, view.SearhDeclareByTinyOrderID(arry));
                }

                return Json(new
                {
                    val = 200,
                    msg = "操作成功！"
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    val = 400,
                    msg = ex.Message,
                });
            }
        }

        /// <summary>
        /// 更新Input信息
        /// </summary>
        /// <param name="jarry">formsJArrayPost CgDelcaresView.MyUpdatePut[]</param>
        /// <returns>成功或是报错！</returns>
        [HttpPayload]
        public ActionResult UpdateInput(JArrayPost jarry)
        {
            LitTools.Current["更新订单"].Log("开始调用：UpdateInput");
            LitTools.Current["更新订单"].Log("调用：UpdateInput对应的Json为: " + jarry.ToObject<object>().ToString());

            var arry = jarry.ToObject<CgDelcaresView.MyUpdatePut[]>();
            try
            {
                using (CgDelcaresView view = new CgDelcaresView())
                {
                    view.UpdateInput(arry);
                }
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "修改成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LitTools.Current["更新订单"].Log("失败调用：UpdateInput！");
                LitTools.Current["更新订单"].Log("失败调用：UpdateInput的原因为:" + ex.Message + ex.StackTrace);

                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "修改失败: " + ex.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新Output信息
        /// </summary>
        /// <param name="jarry">formsJArrayPost CgDelcaresView.MyUpdatePut[]</param>
        /// <returns>成功或是报错！</returns>
        [HttpPost]
        public ActionResult UpdateOutput(JArrayPost jarry)
        {
            LitTools.Current["更新订单"].Log("开始调用：UpdateOutput");
            LitTools.Current["更新订单"].Log("调用：UpdateOutput对应的Json为: " + jarry.ToObject<object>().ToString());

            var arry = jarry.ToObject<CgDelcaresView.MyUpdatePut[]>();
            try
            {
                using (CgDelcaresView view = new CgDelcaresView())
                {
                    view.UpdateOutput(arry);
                }
                return Json(new JMessage
                {
                    success = true,
                    code = 200,
                    data = "修改成功！"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LitTools.Current["更新订单"].Log("失败调用：UpdateOutput！");
                LitTools.Current["更新订单"].Log("失败调用：UpdateOutput的原因为:" + ex.Message + ex.StackTrace);

                return Json(new JMessage
                {
                    success = false,
                    code = 400,
                    data = "修改失败: " + ex.Message,
                }, JsonRequestBehavior.AllowGet);
            }
            
        }

        ///// <summary>
        ///// 申报参数
        ///// </summary>
        ///// <param name="forms">item:包涵小订单ID的数组</param>
        ///// <returns>成功或是报错！</returns>
        //[HttpPost]
        //[Obsolete("经与荣检商议，不能重复申报。因此废弃，相关逻辑转移到申报中")]
        //public ActionResult Delcaring(FormCollection forms)
        //{
        //    var arry = forms["items"].Split(',');

        //    using (CgDelcaresView view = new CgDelcaresView())
        //    {
        //        view.Delcaring(arry);
        //    }
        //    return Json(new JMessage
        //    {
        //        success = true,
        //        code = 200,
        //        data = "修改成功！"
        //    }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 申报成功（生成香港出库通知）（荣检调用）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AutoHkExitNotice(JPost obj)
        {
            LitTools.Current["生成香港出库通知"].Log("开始调用：AutoHkExitNotice");
            LitTools.Current["生成香港出库通知"].Log("调用：AutoHkExitNotice对应的Json为: " + obj.ToObject<object>().ToString());

            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    var Delcare = obj.ToObject<CgDelcare>();
                    service.AutoHkExitNotice(Delcare);
                }
                sw.Stop();
                var json = new JMessage() { code = 200, success = true, data = "调用成功,时长" + sw.Elapsed.TotalMilliseconds };

                LitTools.Current["生成香港出库通知"].Log("成功调用：AutoHkExitNotice");

                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };

                LitTools.Current["生成香港出库通知"].Log("失败调用：AutoHkExitNotice！");
                LitTools.Current["生成香港出库通知"].Log("失败调用：AutoHkExitNotice的原因为:" + ex.Message + ex.StackTrace);
                return Json(json, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 报关截单（更新运单数据）（荣检调用）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DelcareCutting(JPost obj)
        {
            LitTools.Current["生成香港出库通知"].Log("开始调用：DelcareCutting");
            LitTools.Current["生成香港出库通知"].Log("调用DelcareCutting对应的Json为:" + obj.ToObject<object>().ToString());
            try
            {
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    var Delcare = obj.ToObject<CgDelcareCutting>();
                    service.DelcareCutting(Delcare);
                }
                var json = new JMessage() { code = 200, success = true, data = "调用成功" };
                LitTools.Current["生成香港出库通知"].Log("成功调用：DelcareCutting");
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };
                LitTools.Current["生成香港出库通知"].Log("失败调用：DelcareCutting！");
                LitTools.Current["生成香港出库通知"].Log("失败DelcareCutting的原因为:" + ex.Message + ex.StackTrace);
                
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 香港报关出库（库房前端调用）
        /// </summary>
        /// <param name="lotNumber">运输批次号</param>
        /// <returns></returns>
        public ActionResult AutoHkExit(string lotNumber, string adminID)
        {
            LitTools.Current["生成香港出库通知"].Log("开始调用：AutoHkExit");
            LitTools.Current["生成香港出库通知"].Log("调用：AutoHkExit对应的lotNumber为: " + lotNumber);
            try
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    //已经放到任务中开发了
                    //service.AutoHkExit(lotNumber);
                    service.Completed(lotNumber, adminID);
                    //已经放到任务中开发了
                    //service.AutoVouchers(lotNumber);
                }
                sw.Stop();
                var json = new JMessage() { code = 200, success = true, data = "调用成功,时长" + sw.Elapsed.TotalMilliseconds };
                LitTools.Current["生成香港出库通知"].Log("成功调用：AutoHkExit");
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };
                LitTools.Current["生成香港出库通知"].Log("失败调用：AutoHkExit！");
                LitTools.Current["生成香港出库通知"].Log("失败调用：AutoHkExit的原因为: " + ex.Message + ex.StackTrace);
                LitTools.Current["生成香港出库通知"].Log("失败调用：AutoHkExit对应的lotNumber为: " + lotNumber);
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 深圳上架【单箱】（库房前端调用）
        /// </summary>
        /// <param name="shelveID">库位号</param>
        /// <param name="Boxcode">箱号</param>
        /// <returns></returns>
        public ActionResult SZPlace(string shelveID, string Boxcode)
        {
            try
            {
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    service.SzPlace(shelveID, Boxcode);
                }
                var json = new JMessage() { code = 200, success = true, data = "调用成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 深圳上架【多箱】（库房前端调用）
        /// </summary>
        /// <param name="shelveID">库位号</param>
        /// <param name="boxcodes">箱号</param>
        /// <returns></returns>
        public ActionResult SZPlace(string shelveID, params string[] boxcodes)
        {
            try
            {
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    service.SzPlace(shelveID, boxcodes);
                }
                var json = new JMessage() { code = 200, success = true, data = "调用成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 深圳价格更新
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult SzPriceUpdate(JPost obj)
        {
            try
            {
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    var SZPrice = obj.ToObject<CgDelcareSZPrice>();
                    service.SzPriceUpdate(SZPrice);
                }
                var json = new JMessage() { code = 200, success = true, data = "调用成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = true, data = "调用失败：" + ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新保存香港库房封条号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult UpdateHKSealNumber(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["WaybillID"]?.Value<string>(),
                hkSealNumber = jpost["HKSealNumber"]?.Value<string>(),
            };

            if (string.IsNullOrEmpty(arguments.waybillID) || string.IsNullOrEmpty(arguments.hkSealNumber))
            {   
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "请检查参数是否正确, WaybillID, 及 HKSealNumber不能为Null"
                }, JsonRequestBehavior.DenyGet);
            }

            using (CgDelcareShipView service = new CgDelcareShipView())
            {
                service.UpdateHKSealNumber(arguments.waybillID, arguments.hkSealNumber);
            }

            return Json(new JMessage()
            {
                code = 200,
                success = true,
                data = "调用成功",
            }, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 返回打印文件所在位置
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult PrintExportFiles(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["WaybillID"]?.Value<string>(),
                fileName = jpost["FileName"]?.Value<string>(),
                wbTotalParts = jpost["TotalParts"]?.Value<int?>(),
            };

            if (string.IsNullOrEmpty(arguments.waybillID) || (arguments.fileName != "thwt" && arguments.fileName != "hwlz") || arguments.wbTotalParts.HasValue == false)
            {
                return Json(new JMessage()
                {
                    code = 400,
                    success = false,
                    data = "请确认参数是否正确, WaybillID，TotalParts, FileName不能为Null, 并且FileName 只能为 thwt(提货委托书), 或者 hwlz(货物流转书)",
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                using (CgDelcareShipView service = new CgDelcareShipView())
                {
                    string realName = $"{arguments.waybillID}{arguments.fileName}.pdf";

                    string basePath = Server.MapPath("../");
                    string exportPath = Server.MapPath("../Export/");
                    string templatePath = Server.MapPath("../Template/");
                    string realFilePath = exportPath + realName;
                    string url = Request.Url.ToString();
                    string newUrl = url.Substring(0, url.LastIndexOf("cgDelcare"));
                    var newFileName = service.ExportFile(arguments.waybillID, arguments.fileName, basePath, arguments.wbTotalParts.Value);
                    string newUrlPath = newUrl + "Export/" + newFileName;
                    return Json(new JMessage()
                    {
                        code = 200,
                        success = true,
                        data = newUrlPath,
                    }, JsonRequestBehavior.DenyGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new JMessage()
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }
    }
}
