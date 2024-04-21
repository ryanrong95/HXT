using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Web.Mvc;
using Yahv.Web.Mvc.Filters;
using ConsoleApp.vTaskers.Services;
using Layers.Data.Sqls;

namespace MvcApi.Controllers.chonggou
{
    /// <summary>
    /// 拣货出库接口
    /// </summary>
    public class cgPickingsController : Controller
    {
        /// <summary>
        /// 根据前端的过滤条件，获取对应的出库运单
        /// </summary>
        /// <param name="jPost">Picking.搜索参数.json</param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult Show(JPost jPost)
        {
            var Params = new
            {
                warehouseid = jPost["WareHouseID"].Value<string>(),
                key = jPost["Key"]?.Value<string>(),
                client = jPost["Client"]?.Value<string>(),
                partnumber = jPost["ProductPartNumber"]?.Value<string>(),
                excutestatus = jPost["WaybillExcuteStatus"]?.Value<string>(),
                source = jPost["Source"]?.Value<int>(),
                startdate = jPost["StartDate"]?.Value<string>(),
                enddate = jPost["EndDate"]?.Value<string>(),
                pageindex = jPost["PageIndex"]?.Value<int>() ?? 1,
                pagesize = jPost["PageSize"]?.Value<int>() ?? 20,
            };

            //根据查询条件过滤数据
            using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
            {
                long ticksThisTime = 0;
                System.Diagnostics.Stopwatch timePerParse;
                timePerParse = System.Diagnostics.Stopwatch.StartNew();
                var linq = view.SearchByWareHouseID(Params.warehouseid);
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //主单号
                if (!string.IsNullOrWhiteSpace(Params.key))
                {
                    linq = linq.SearchByID(Params.key);
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //客户
                if (!string.IsNullOrWhiteSpace(Params.client))
                {
                    linq = linq.SearchByClient(Params.client);
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //产品型号
                if (!string.IsNullOrWhiteSpace(Params.partnumber))
                {
                    linq = linq.SearchByPartNumber(Params.partnumber);
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                ////开始时间
                if (!string.IsNullOrWhiteSpace(Params.startdate))
                {
                    linq = linq.SearchByStartDate(DateTime.Parse(Params.startdate));
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //结束时间
                if (!string.IsNullOrWhiteSpace(Params.enddate))
                {
                    linq = linq.SearchByEndDate(DateTime.Parse(Params.enddate));
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //业务类型
                if (Params.source != null && Params.source != (int)CgNoticeSource.All)
                {
                    linq = linq.SearchBySource(Params.source.Value);
                }
                //timePerParse.Stop();
                //ticksThisTime = timePerParse.ElapsedMilliseconds;

                //timePerParse = System.Diagnostics.Stopwatch.StartNew();
                //出库状态过滤
                if (!string.IsNullOrWhiteSpace(Params.excutestatus))
                {
                    string[] sources = Params.excutestatus.Split(',');
                    var statuslist = ExtendsEnum.ToArray<CgPickingExcuteStatus>().Where(item => sources.Contains(((int)item).ToString())).ToArray();
                    var searchlist = Enum.GetValues(typeof(CgPickingExcuteStatus)).Cast<CgPickingExcuteStatus>().Except<CgPickingExcuteStatus>(new CgPickingExcuteStatus[] { CgPickingExcuteStatus.All, CgPickingExcuteStatus.Completed }).ToArray();

                    if (statuslist.Any())
                    {
                        //不包含全部就执行查询
                        if (!statuslist.Contains(CgPickingExcuteStatus.All))
                        {
                            linq = linq.SearchByStatus(statuslist);
                        }
                        else
                        {
                            linq = linq.SearchByStatus(searchlist);
                        }
                    }
                }
                //timePerParse.Stop();
                ticksThisTime = timePerParse.ElapsedMilliseconds;

                var result = linq.GetPagelistData(Params.pageindex, Params.pagesize);

                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 获取详细数据
        /// </summary>
        /// <param name="waybillID">运单ID</param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            using (var repository = new PvWmsRepository())
            {
                var waybill = repository.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().SingleOrDefault(item => item.wbID == id);
                if (waybill == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Data = $"您所请求的运单ID: {id}不存在",
                    }, JsonRequestBehavior.AllowGet);
                }

                var source = (CgNoticeSource)waybill.Source;
                if (source != CgNoticeSource.AgentBreakCustomsForIns)
                {
                    using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
                    {
                        var data = view.GetDetail(id);

                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    // 获取深圳代报关内单的详情信息
                    using (var view = new Wms.Services.chonggous.Views.CgSzInsidePickingsView())
                    {
                        var data = view.GetDetail(id);

                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
            }
                
        }

        /// <summary>
        /// 获取深圳代报关内单出库单打印数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetSzPrintData(string id)
        {
            using (var view = new Wms.Services.chonggous.Views.CgSzInsidePickingsView())
            {
                var data = view.GetSzPrintData(id);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据产品型号和品牌过滤清单数据
        /// </summary>
        /// <param name="WaybillID"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public ActionResult Notices(string WaybillID, string key)
        {
            using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
            {
                var data = view.GetDetailNoticeByID(WaybillID, key);

                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 出库提交
        /// </summary>
        /// <param name="jPost">Pickings.视图（提交）.json</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Out(JPost jpost)
        {
            try
            {
                using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
                {
                    view.Completed(jpost);
                    return Json(new
                    {
                        Success = true,
                        Data = string.Empty,
                    });
                }
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

        /// <summary>
        /// 深圳代报关内单出库
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult SzInsideOut(string id)
        {
            try
            {
                using (var view = new Wms.Services.chonggous.Views.CgSzInsidePickingsView())
                {
                    view.Completed(id);
                    return Json(new
                    {
                        Success = true,
                        Data = string.Empty,
                    });
                }
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


        /// <summary>
        /// 转报关分拣
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult TurnDeclare(JPost jpost)
        {
            using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
            {
                view.SortingCompleted(jpost);
                return Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });
            }
        }

        /// <summary>
        /// 深圳分拣异常
        /// </summary>
        /// <param name="waybillid">运单ID</param>
        /// <param name="adminid">操作人ID</param>
        /// <param name="orderid">订单ID</param>
        /// <param name="summary">异常原因</param>
        /// <returns></returns>
        public ActionResult PackingExcept(string waybillid, string adminid, string orderid, string summary)
        {
            using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
            {
                view.Excepted(waybillid, adminid, orderid, summary);
                return Json(new
                {
                    Success = true,
                    Data = string.Empty,
                });

            }
        }


        /// <summary>
        /// 收货确认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[HttpPayload]
        public ActionResult ReceiptConfirm(string adminID, string waybillid/*JPost jpost*/)
        {
            //var adminID = jpost["adminID"].Value<string>();
            //var waybillID = jpost["waybillid"].Value<string>();
            Taskers.ReceiptConfirm(adminID, waybillid);
            return Json(new
            {
                Success = true,
                Data = string.Empty,
            });
        }


        /// <summary>
        /// 深圳出库单
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        public ActionResult SZOutList(JPost jPost)
        {



            var Params = new
            {
                warehouseid = jPost["warehouseid"].Value<string>(),
                code = jPost["code"]?.Value<string>(),
                client = jPost["client"]?.Value<string>(),
                carrier = jPost["Carrier"]?.Value<string>(),
                status = jPost["status"]?.Value<int?>() ?? (int)IsUpload.All,
                pageindex = jPost["pageindex"]?.Value<int?>() ?? 1,
                pagesize = jPost["pagesize"]?.Value<int?>() ?? 50,
            };

            //根据查询条件过滤数据
            using (var view = new Wms.Services.chonggous.Views.CgSzPickingsView())
            {

                var linq = view.SearchByWareHouseID(Params.warehouseid);

                //承运商
                if (!string.IsNullOrWhiteSpace(Params.carrier))
                {
                    linq = linq.SearchByCarrier(Params.carrier);
                }

                //客户名称
                if (!string.IsNullOrWhiteSpace(Params.client))
                {
                    linq = linq.SearchByClient(Params.client);
                }
                //入仓号
                if (!string.IsNullOrWhiteSpace(Params.code))
                {
                    linq = linq.SearchByCode(Params.code);
                }
                //是否上传
                if (Params.status != null && Params.status != (int)IsUpload.All)
                {
                    linq = linq.SearchByStatus(Params.status);
                }

                var result = linq.GetPagelistData(Params.pageindex, Params.pagesize);

                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// 一键打印运单后, 保存运单号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateWaybillCode(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["WaybillID"]?.Value<string>(),
                code = jpost["Code"]?.Value<string>(),
            };

            if (string.IsNullOrEmpty(arguments.waybillID) || string.IsNullOrEmpty(arguments.code))
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "请检查参数, WaybllID, 及 Code不能为空",
                }, JsonRequestBehavior.DenyGet);
            }

            using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
            {
                view.UpdateWaybillCode(arguments.waybillID, arguments.code);
            }

            return Json(new JMessage
            {
                code = 200,
                success = true,
                data = "调用成功",
            });
        }

        /// <summary>
        /// 更新深圳出库运单的快递方式，（只能是顺丰，或者跨越速运
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateWaybillExpress(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["WaybillID"]?.Value<string>(),
                shipperCode = jpost["ShipperCode"]?.Value<string>(),
                exType = jpost["ExType"]?.Value<int?>(),
                exPayType = jpost["ExPayType"]?.Value<int?>(),
                thirdPartyCardNo = jpost["ThirdPartyCardNo"]?.Value<string>(),
            };

            if (string.IsNullOrEmpty(arguments.waybillID) || string.IsNullOrEmpty(arguments.shipperCode))
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "请检查参数，WaybillID, 及 ShipperCode 不能为空",
                }, JsonRequestBehavior.DenyGet);
            }

            if (arguments.exPayType.Value == 4 && string.IsNullOrWhiteSpace(arguments.thirdPartyCardNo))
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "请检查参数，计费方式为第三方月结时，第三方月结账号不能为空",
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
                {
                    view.UpdateWaybillExpress(arguments.waybillID, arguments.shipperCode, arguments.exType, arguments.exPayType, arguments.thirdPartyCardNo);
                    return Json(new JMessage
                    {
                        code = 200,
                        success = true,
                        data = "调用更新成功",
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }

        /// <summary>
        /// 用于修改当货运类型是快递时的WaybillCode
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyWaybillCode(JPost jpost)
        {
            var waybillID = jpost["WaybillID"].Value<string>();
            var code = jpost["Code"].Value<string>();

            if (string.IsNullOrEmpty(waybillID) || string.IsNullOrEmpty(code))
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "参数WaybillID 和Code 不能为空或null",
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
                {
                    view.ModifyWaybillCode(waybillID, code.Trim());

                    return Json(new JMessage
                    {
                        code = 200,
                        success = true,
                        data = string.Empty,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {                
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }

        /// <summary>
        /// 用于修改货运方式是快递的运单的收货人信息 地址、联系人、电话
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult ModifyWaybillConsigneeInfo(JPost jpost)
        {            
            var waybillID = jpost["WaybillID"].Value<string>();
            var address = jpost["Address"].Value<string>();
            var name = jpost["Name"].Value<string>();
            var mobile = jpost["Mobile"].Value<string>();

            if (string.IsNullOrEmpty(waybillID))
            {
                
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "参数WaybillID不能为空或null",
                }, JsonRequestBehavior.DenyGet);
            }

            if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(name) || string.IsNullOrEmpty(mobile))
            {                
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = "参数Address, Name, Mobile不能为空或null",
                }, JsonRequestBehavior.DenyGet);
            }

            try
            {
                using (var view = new Wms.Services.chonggous.Views.CgPickingsView())
                {
                    view.ModifyWaybillConsigneeInfo(waybillID, address.Trim(), name.Trim(), mobile.Trim());

                    return Json(new JMessage
                    {
                        code = 200,
                        success = true,
                        data = string.Empty,
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new JMessage
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.DenyGet);
            }            
        }
    }
}