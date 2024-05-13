using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Web.Mvc.Filters;
using Yahv.Web.Mvc;
using Newtonsoft.Json.Linq;
using Wms.Services.chonggous.Views;
using Yahv.Underly;
using Layers.Data.Sqls;
using Yahv.Services.Enums;

namespace MvcApi.Controllers.chonggou
{
    /// <summary>
    /// 转报关接口
    /// </summary>
    public class cgCustomsStorageController : Controller
    {
        /// <summary>
        /// 根据前端过滤条件,获取出库运单
        /// </summary>
        /// <param name="jPost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult Show(JPost jPost)
        {
            var arguments = new
            {
                warehouseid = jPost["WareHouseID"].Value<string>(),
                key = jPost["Key"]?.Value<string>(),
                client = jPost["Client"]?.Value<string>(),
                partnumber = jPost["ProductPartNumber"]?.Value<string>(),
                excutestatus = jPost["ExcuteStatus"]?.Value<string>(),
                startdate = jPost["StartDate"]?.Value<string>(),
                enddate = jPost["EndDate"]?.Value<string>(),
                pageindex = jPost["PageIndex"]?.Value<int>() ?? 1,
                pagesize = jPost["PageSize"]?.Value<int>() ?? 20,
            };

            using (var view = new CgCustomsStorageView())
            {
                var linq = view.SearchByWareHouseID(arguments.warehouseid);

                if (!string.IsNullOrEmpty(arguments.key))
                {
                    linq = linq.SearchByID(arguments.key);
                }

                if (!string.IsNullOrEmpty(arguments.client))
                {
                    linq = linq.SearchByClient(arguments.client);
                }

                if (!string.IsNullOrEmpty(arguments.partnumber))
                {
                    linq = linq.SearchByPartNumber(arguments.partnumber);
                }

                if (!string.IsNullOrEmpty(arguments.startdate) && !string.IsNullOrEmpty(arguments.enddate))
                {
                    linq = linq.SearchByDate(DateTime.Parse(arguments.startdate), DateTime.Parse(arguments.enddate));
                }

                if (!string.IsNullOrEmpty(arguments.excutestatus))
                {
                    string[] sources = arguments.excutestatus.Split(',');
                    var excutestatuslist = ExtendsEnum.ToArray<CgPickingExcuteStatus>().Where(item => sources.Contains(((int)item).ToString())).ToArray();
                    var searchlist = Enum.GetValues(typeof(CgPickingExcuteStatus)).Cast<CgPickingExcuteStatus>().Except<CgPickingExcuteStatus>(new CgPickingExcuteStatus[] { CgPickingExcuteStatus.Completed, CgPickingExcuteStatus.All }).ToArray();

                    if (!excutestatuslist.Contains(CgPickingExcuteStatus.All))
                    {
                        linq = linq.SearchByStatus(excutestatuslist);
                    }
                    else
                    {
                        linq = linq.SearchByStatus(searchlist);
                    }
                }

                var result = linq.GetPagelistData(arguments.pageindex, arguments.pagesize);
                return Json(result, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 获取转报关详细数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
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
                        Data = $"您请求的运单ID:{id}不存在",
                    }, JsonRequestBehavior.AllowGet);
                }
                if ((CgNoticeSource)waybill.Source != CgNoticeSource.AgentCustomsFromStorage)
                {
                    return Json(new
                    {
                        Success = false,
                        Data = $"您所请求的运单{id}不是转报关订单, 不能使用此接口",
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    using (var view = new CgCustomsStorageView())
                    {
                        var data = view.GetDetail(id);
                        return Json(data, JsonRequestBehavior.AllowGet);
                    }
                }
            }
        }

        /// <summary>
        /// 转报关分拣
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TurnDeclare(JPost jpost)
        {
            using (var view = new CgCustomsStorageView())
            {
                view.PickingCompleted(jpost);
                return Json(new
                {
                    Success = true,
                    Data = string.Empty,
                }, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 封箱操作 -- 提供封箱检查
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseBoxes(JPost jpost)
        {
            var waybillID = jpost["WaybillID"]?.Value<string>();
            var adminID = jpost["AdminID"]?.Value<string>();
            var arry = jpost["TinyOrderID"]?.Values<string>();

            try
            {
                using (var view = new CgCustomsStorageView())
                {
                    view.CloseBoxes(waybillID, adminID, arry.ToArray());
                    return Json(new JMessage
                    {
                        code = 200,
                        success = true,
                        data = "封箱成功!"
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
                });
            }
        }

        /// <summary>
        /// 删除已经拣货的拣货历史记录
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult DeletePicking(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["StorageID"]?.Value<string>(),
                pickingID = jpost["PickingID"]?.Value<string>(),
                adminID = jpost["AdminID"]?.Value<string>(),
            };

            using (var view = new CgCustomsStorageView())
            {
                if (string.IsNullOrEmpty(arguments.storageID) || string.IsNullOrEmpty(arguments.pickingID) || string.IsNullOrEmpty(arguments.adminID))
                {
                    return Json(new JMessage
                    {
                        code = 400,
                        success = false,
                        data = "请检查参数是否正确, AdminID,StorageID,及PickingID 不能为null"
                    });
                }
                else
                {
                    try
                    {
                        view.DeletePicking(arguments.pickingID, arguments.storageID, arguments.adminID);
                        return Json(new JMessage
                        {
                            code = 200,
                            success = true,
                            data = "删除成功",
                        }, JsonRequestBehavior.DenyGet);
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

        /// <summary>
        /// 修改拣货历史中的箱号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult ModifyBoxCode(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["StorageID"]?.Value<string>(),
                boxcode = jpost["BoxCode"]?.Value<string>(),
                adminID = jpost["AdminID"]?.Value<string>(),
            };

            using (var view = new CgCustomsStorageView())
            {
                if (string.IsNullOrEmpty(arguments.storageID) || string.IsNullOrEmpty(arguments.boxcode) || string.IsNullOrEmpty(arguments.adminID))
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确,PickingID, StorageID, BoxCode, AdminID 不能为null"
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    try
                    {
                        view.ModifyBoxCode(arguments.storageID, arguments.boxcode, arguments.adminID);

                        return Json(new
                        {
                            Success = true,
                            Data = string.Empty,
                        }, JsonRequestBehavior.DenyGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new
                        {
                            Success = false,
                            Data = ex.Message,
                        }, JsonRequestBehavior.DenyGet);
                    }

                }
            }
        }
    }
}