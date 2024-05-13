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
using Yahv.Web.Mvc.Filters;
using Yahv.Web.Mvc;
using Wms.Services;
using Wms.Services.chonggous.Views;
using Newtonsoft.Json.Linq;
using Yahv.Services.Enums;
using Wms.Services.chonggous.Models;

namespace MvcApp.Controllers
{
    public class cgSortingsController : Controller
    {
        /// <summary>
        /// 分拣展示
        /// </summary>
        /// <param name="id">必须有唯一的参数</param>
        /// <returns>返回默认的视图</returns>
        public ActionResult Show(string id, int pageindex = 1, int pagesize = 20)
        {
            //默认的状态: SortingExcuteStatus枚举中去除 完成入库状态
            //默认的类型: 没有要求
            //固定参数


            var includes = ExtendsEnum.ToArray<CgSortingExcuteStatus>(CgSortingExcuteStatus.Completed, CgSortingExcuteStatus.All);

            var data = new CgSortingsView().SearchByWareHouseID(id)
                .SearchByStatus(includes).ToMyPage(pageindex, pagesize);
            //对接乔霞

            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据前端的过滤条件，获取对应的运单项
        /// </summary>
        /// <returns></returns>
        /// <remarks>Sortings.搜索交互参数.json</remarks>
        [HttpPayload]
        public ActionResult Show(JPost jpost)
        {
            //在搜索的时候固定接收  post forms 方式访问字段，以避免如下情况：
            //http://url?skdfksd=asdf&odsfs=33yyyy-MM-dd HH:mm:ss

            var arguments = new
            {
                whid = jpost["WhID"]?.Value<string>(),
                key = jpost["key"]?.Value<string>(),
                waybillid = jpost["WaybillID"]?.Value<string>(),
                supplier = jpost["WaybillSupplier"]?.Value<string>(),
                startdate = jpost["StartDate"]?.Value<string>(),
                enddate = jpost["EndDate"]?.Value<string>(),
                partnumber = jpost["ProductPartNumber"]?.Value<string>(),
                status = jpost["WaybillExcuteStatus"]?.Value<string>(),
                sources = jpost["Source"]?.Value<int>(),
                entercode = jpost["WaybillCode"]?.Value<string>(),
                pageIndex = jpost["PageIndex"]?.Value<int?>() ?? 1,
                pageSize = jpost["PageSize"]?.Value<int?>() ?? 20,
                waybilltype = jpost["WaybillType"]?.Value<int?>(),
            };

            using (var view = new CgSortingsView())
            {
                var data = view;

                if (!string.IsNullOrEmpty(arguments.whid))
                {
                    data = data.SearchByWareHouseID(arguments.whid);
                }

                if (!string.IsNullOrEmpty(arguments.supplier))
                {
                    data = data.SearchBySupplier(arguments.supplier);
                }

                if (!string.IsNullOrEmpty(arguments.startdate))
                {
                    data = data.SearchByStartDate(DateTime.Parse(arguments.startdate));
                }

                if (!string.IsNullOrEmpty(arguments.enddate))
                {
                    data = data.SearchByEndDate(DateTime.Parse(arguments.enddate));
                }

                if (!string.IsNullOrEmpty(arguments.partnumber))
                {
                    data = data.SearchByPartNumber(arguments.partnumber);
                }

                if (arguments.waybilltype != null && arguments.waybilltype != (int)WaybillType.All)
                {
                    data = data.SearchByWaybillType(arguments.waybilltype.Value);
                }

                if (!string.IsNullOrEmpty(arguments.status))
                {
                    string[] status = arguments.status.Split(',');
                    List<CgSortingExcuteStatus> excuteStatus = new List<CgSortingExcuteStatus>();

                    if (status.Contains(((int)CgSortingExcuteStatus.All).ToString()))
                    {
                        excuteStatus.AddRange(ExtendsEnum.ToArray(CgSortingExcuteStatus.All, CgSortingExcuteStatus.Completed));
                    }
                    else
                    {
                        foreach (var statu in status)
                        {
                            excuteStatus.Add((CgSortingExcuteStatus)(int.Parse(statu)));
                        }
                    }

                    data = data.SearchByStatus(excuteStatus.ToArray());
                }
                else
                {
                    var includes = ExtendsEnum.ToArray(CgSortingExcuteStatus.Completed, CgSortingExcuteStatus.All);
                    data = data.SearchByStatus(includes);
                }
                //业务类型
                if (arguments.sources != null && arguments.sources != (int)CgNoticeSource.All)
                {
                    data = data.SearchBySource(arguments.sources.Value);
                }

                // 订单号，运单号，入仓号
                if (!string.IsNullOrEmpty(arguments.key))
                {
                    data = data.SearchByID(arguments.key);
                }

                var result = data.ToMyPage(arguments.pageIndex, arguments.pageSize);
                return Json(new { obj = result }, JsonRequestBehavior.AllowGet);
            }
            // \cgSortings\Show
            //通知的过程
            /*
             Waybill
             +Notcie
            */
        }

        /// <summary>
        /// 获取具体运单项信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Detail(string id)
        {
            var key = Request["key"];

            var data = new CgSortingsView().SearchByWaybillID(id)
                .ToMyArray(key ?? "");

            return Json(data[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取历史到货WaybillID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult History(string id)
        {
            var data = new CgHistoriesView().HistoryWaybillIDs(id);
            return Json(new { obj = data }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取历史到货信息
        /// </summary>
        /// <param name="id">运单</param>
        /// <returns></returns>
        public ActionResult HistoryDetail(string id)
        {
            var data = new CgHistoriesView().HistoryDetail(id).ToHistory();

            return Json(data[0], JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetLogs(string id)
        {
            return null;
        }

        /// <summary>
        /// 添加并录入
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Enter(JPost jpost)
        {
            //通知的过程
            /*
             Waybill
             +Notcie
            */

            try
            {
                string transferID;
                new CgSortingsView().EnterNew(jpost, out transferID);
                return Json(new
                {
                    Success = true,
                    Data = string.IsNullOrEmpty(transferID) ? string.Empty : transferID,
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

        /// <summary>
        /// 实时获取SortingID
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSortingID()
        {
            string sortingID = new CgSortingsView().GetSortingID();
            return Json(sortingID, JsonRequestBehavior.AllowGet);
        }

        // 提货
        public ActionResult TakeGoods(string id)
        {
            var adminid = Request["adminid"];
            CgSortingsView sortingView = new CgSortingsView();
            sortingView.TakeGoods(id, adminid);
            return Json(new
            {
                Success = true,
                Data = string.Empty,
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取申报项日志信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetDeclareItemInfo(string id)
        {
            using (var view = new CgSortingsView())
            {
                var result = view.GetDeclareItemInfo(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 删除已经代报关的分拣入库
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult DeleteSorting(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["StorageID"]?.Value<string>(), 
                adminID = jpost["AdminID"]?.Value<string>(),            
            };

            using (var view = new CgSortingsView())
            {
                if (string.IsNullOrEmpty(arguments.storageID))
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确, AdminID 及StorageID 不能为null"
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    try
                    {
                        view.DeleteSorting(arguments.storageID, arguments.adminID);
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

        /// <summary>
        /// 修改到货历史中的箱号
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult ModifyBoxCode(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["StorageID"]?.Value<string>(),                
                boxcode = jpost["BoxCode"]?.Value<string>(),
                adminID = jpost["AdminID"]?.Value<string>(),
            };

            using (var view = new CgSortingsView())
            {
                if (string.IsNullOrEmpty(arguments.storageID) || string.IsNullOrEmpty(arguments.boxcode) || string.IsNullOrEmpty(arguments.adminID))
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确, StorageID, BoxCode, AdminID 不能为null"
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

        /// <summary>
        /// 修改封箱页面的箱号或者到货数量
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyQuantity(JPost jpost)
        {
            var arguments = new
            {
                storageID = jpost["StorageID"]?.Value<string>(),
                adminID = jpost["AdminID"]?.Value<string>(),                
                quantity = jpost["Quantity"]?.Value<decimal>(),
            };

            using (var view = new CgSortingsView())
            {
                if (string.IsNullOrEmpty(arguments.storageID) || string.IsNullOrEmpty(arguments.adminID) || arguments.quantity.HasValue == false)
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确, StorageID,AdminID不能为空, 并且要修改的数量 Quantity不能为空!"
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    try
                    {
                        view.ModifyQuantity(arguments.storageID, arguments.adminID, arguments.quantity.Value);

                        return Json(new
                        {
                            Success = true,
                            Data = "修改成功",
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

        /// <summary>
        /// 针对代报关装箱或者封箱后修改运单号及承运商信息,
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ModifyWbCodeAndCarrierID(JPost jpost)
        {
            var arguments = new
            {
                waybillID = jpost["WaybillID"]?.Value<string>(),
                wbCode = jpost["wbCode"]?.Value<string>(),
                carrierID = jpost["CarrierID"]?.Value<string>(),
                type = jpost["Type"]?.Value<int?>(),
            };

            using (var view = new CgSortingsView())
            {
                if (string.IsNullOrEmpty(arguments.waybillID) || string.IsNullOrEmpty(arguments.wbCode) || string.IsNullOrEmpty(arguments.carrierID) || arguments.type.HasValue == false)
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确, wbCode,CarrierID, type"
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    try
                    {
                        view.ModifyWbCodeAndCarrierID(arguments.waybillID, arguments.wbCode, arguments.carrierID, arguments.type.Value);

                        return Json(new
                        {
                            Success = true,
                            Data = "修改成功",
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

        /// <summary>
        /// 获取Waybill是否可以操作状态
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPayload]
        public ActionResult GetWaybillCurrentStatus(JPost jpost)
        {
            var arguments = new
            {
                waybillIDs = jpost["WaybillIDs"]?.Values<string>(),
            };

            using (var view = new CgSortingsView())
            {
                if (arguments.waybillIDs.Count() == 0)
                {
                    return Json(new
                    {
                        Success = false,
                        Data = "请检查参数是否正确, WaybillIDs 不能为null"
                    }, JsonRequestBehavior.DenyGet);
                }
                else
                {
                    try
                    {
                        var result = view.GetWaybillCurrentStatus(arguments.waybillIDs.ToArray());
                        return Json(new
                        {
                            Success = true,
                            Data = result,
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

        /// <summary>
        /// 封箱操作旧的
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseBoxesOld(JPost jpost)
        {
            try
            {
                using (var view = new CgSortingsView())
                {
                    var cgCloseBoxes = jpost.ToObject<CgCloseBoxes>();

                    foreach (var item in cgCloseBoxes.Items)
                    {
                        if (string.IsNullOrEmpty(item.TinyOrderID) || string.IsNullOrEmpty(item.OrderID) || string.IsNullOrEmpty(item.StorageID) || string.IsNullOrEmpty(item.ItemID) || item.Quantity.HasValue == false
                            || string.IsNullOrEmpty(item.AdminID) || string.IsNullOrEmpty(item.BoxCode) || string.IsNullOrEmpty(item.EnterCode) || item.Weight.HasValue == false)
                        {
                            throw new Exception("请检查参数是否正确, TinyOrderID, OrderID, StorageID, ItemID, AdminID, BoxCode, EnterCode, Quantity, Weight 均不能为空!");
                        }
                    }

                    view.CloseBoxesOld(cgCloseBoxes);

                    return Json(new JMessage()
                    {
                        code = 200,
                        success = true,
                        data = "封箱成功",
                    }, JsonRequestBehavior.DenyGet);
                }
            }
            catch (Exception ex)
            {
                var json = new JMessage()
                {
                    code = 300,
                    success = false,
                    data = ex.Message,
                };
                return Json(json, JsonRequestBehavior.DenyGet);
            }
        }

        /// <summary>
        /// 封箱操作新的---该功能就是提供检查
        /// </summary>
        /// <param name="jpost"></param>
        /// <returns></returns>
        public ActionResult CloseBoxes(JPost jpost)
        {
            var waybillID = jpost["WaybillID"]?.Value<string>();
            var adminID = jpost["AdminID"]?.Value<string>();
            var arry = jpost["TinyOrderID"].ToObject<string[]>();
              
            try
            {
                using (CgSortingsView view = new CgSortingsView())
                {
                    view.CloseBoxes(waybillID, adminID, arry);
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
        ///约定暂时不用
        /// </summary>
        /// <returns></returns>
        [Obsolete("与乔霞商议")]
        public ActionResult Delete()
        {

            //不返回 ，表示成功

            //返回代表错误 

            return Json(new
            {

            });
        }
    }
}
