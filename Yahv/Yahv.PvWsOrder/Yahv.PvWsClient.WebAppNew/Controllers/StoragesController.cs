using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly;
using Yahv.Utils.Extends;
using Yahv.Utils.Npoi;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class StoragesController : UserController
    {
        #region 入库单查询
        /// <summary>
        /// 入库单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult InStorage()
        {
            //数据绑定
            ViewBag.WareHouseOptions = this.GetWarehouses().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();

            ViewBag.SupplierOptions = Yahv.Client.Current.MySupplier.Select(item => new
            {
                value = item.EnglishName,
                text = item.EnglishName,
            }).ToArray();

            ViewBag.OrderTypeOptions = // ExtendsEnum.ToDictionary<OrderType>()
            new[] { OrderType.Recieved, OrderType.Transport, OrderType.TransferDeclare, OrderType.Declare, }
            .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
            .Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 获取入库单列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetInStorage()
        {
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                StartDate = Request.Form["startDate"],
                EndDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                Supplier = Request.Form["Supplier"],
                OrderType = Request.Form["OrderType"],
                WarehouseID = Request.Form["WarehouseID"],
            };

            var Instorageview = Yahv.Client.Current.MyInStorage;

            #region 查询条件过滤
            if (!string.IsNullOrWhiteSpace(paramlist.StartDate))
            {
                Instorageview = Instorageview.SearchByStartDate(DateTime.Parse(paramlist.StartDate));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.EndDate))
            {
                Instorageview = Instorageview.SearchByEndDate(DateTime.Parse(paramlist.EndDate).AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                Instorageview = Instorageview.SearchByPartNumber(paramlist.PartNumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                Instorageview = Instorageview.SearchByOrderID(paramlist.OrderID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.Supplier))
            {
                Instorageview = Instorageview.SearchBySupplier(paramlist.Supplier.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderType))
            {
                Instorageview = Instorageview.SearchByOrderType(int.Parse(paramlist.OrderType));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.WarehouseID))
            {
                Instorageview = Instorageview.SearchByWareHouseID(paramlist.WarehouseID);
            }
            #endregion

            var total = Instorageview.Count();

            var data = Instorageview.Skip((paramlist.page - 1) * paramlist.rows).Take(paramlist.rows).ToArray().Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                item.PartNumber,
                item.Manufacturer,
                Currency = ((Currency?)item.Currency)?.GetDescription(),
                UnitPrice = item.UnitPrice?.ToString("0.00"),
                Quantity = item.Quantity.ToString("0.00"),
                TotalPrice = item.TotalPrice?.ToString("0.00"),
                WareHouseName = item.WarehouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                item.OrderID,
                OrderType = ((OrderType)item.OrderType).GetDescription(),
                IsChecked=false,
            });

            return this.Paging(data, total);
        }



        #endregion

        #region 出库单查询
        /// <summary>
        /// 出库单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult OutStorage()
        {
            //数据绑定
            ViewBag.WareHouseOptions = this.GetWarehouses().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            ViewBag.OrderTypeOptions = // ExtendsEnum.ToDictionary<OrderType>()
            new[] { OrderType.Transport, OrderType.Delivery, OrderType.TransferDeclare, OrderType.Declare, }
            .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
            .Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }


        /// <summary>
        /// 获取入库单列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetOutStorage()
        {
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                StartDate = Request.Form["startDate"],
                EndDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                ConsigneeName = Request.Form["ConsigneeName"],
                OrderType = Request.Form["OrderType"],
                WarehouseID = Request.Form["WarehouseID"],
            };

            var outstorageview = Yahv.Client.Current.MyOutStorage;

            #region 查询条件过滤
            if (!string.IsNullOrWhiteSpace(paramlist.StartDate))
            {
                outstorageview = outstorageview.SearchByStartDate(DateTime.Parse(paramlist.StartDate));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.EndDate))
            {
                outstorageview = outstorageview.SearchByEndDate(DateTime.Parse(paramlist.EndDate).AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                outstorageview = outstorageview.SearchByPartNumber(paramlist.PartNumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                outstorageview = outstorageview.SearchByOrderID(paramlist.OrderID.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.ConsigneeName))
            {
                outstorageview = outstorageview.SearchByConsigneeName(paramlist.ConsigneeName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderType))
            {
                outstorageview = outstorageview.SearchByOrderType(int.Parse(paramlist.OrderType));
            }
            if (!string.IsNullOrWhiteSpace(paramlist.WarehouseID))
            {
                outstorageview = outstorageview.SearchByWareHouseID(paramlist.WarehouseID);
            }
            #endregion

            var total = outstorageview.Count();

            var data = outstorageview.Skip((paramlist.page - 1) * paramlist.rows).Take(paramlist.rows).ToArray().Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                item.PartNumber,
                item.Manufacturer,
                Currency = ((Currency?)item.Currency)?.GetDescription(),
                UnitPrice = item.UnitPrice?.ToString("0.00"),
                Quantity = item.Quantity.ToString("0.00"),
                TotalPrice = item.TotalPrice?.ToString("0.00"),
                WareHouseName = item.WarehouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                item.OrderID,
                OrderType = ((OrderType)item.OrderType).GetDescription(),
                IsChecked = false,
            });

            return this.Paging(data, total);
        }
        #endregion

        #region 我的库存
        /// <summary>
        /// 我的付汇
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult MyStorage()
        {
            //数据绑定
            ViewBag.WareHouseOptions = this.GetWarehouses().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();

            return View();
        }

        /// <summary>
        /// 获取我的库存列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetMyStorage()
        {
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                StartDate = Request.Form["startDate"],
                EndDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                WarehouseID = Request.Form["WarehouseID"],
                Supplier = Request.Form["Supplier"],
                MultiField = Request.Form["MultiField"],
                MultiField2 = Request.Form["MultiField2"],
            };

            var storageView = Yahv.Client.Current.MyStorages.Where(item => true);
            //开始时间
            if (!string.IsNullOrWhiteSpace(paramlist.StartDate))
            {
                storageView = storageView.Where(item => item.CreateDate >= DateTime.Parse(paramlist.StartDate));
            }
            //结束时间
            if (!string.IsNullOrWhiteSpace(paramlist.EndDate))
            {
                storageView = storageView.Where(item => item.CreateDate <= DateTime.Parse(paramlist.EndDate).AddDays(1));
            }
            //型号
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                storageView = storageView.Where(item => item.Product.PartNumber.Contains(paramlist.PartNumber.Trim()));
            }
            //订单号
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                storageView = storageView.Where(item => item.Input.OrderID.Contains(paramlist.OrderID.Trim()));
            }
            //库房
            if (!string.IsNullOrWhiteSpace(paramlist.WarehouseID))
            {
                storageView = storageView.Where(item => item.WareHouseID.StartsWith(paramlist.WarehouseID));
            }
            //供应商
            if (!string.IsNullOrWhiteSpace(paramlist.Supplier))
            {
                storageView = storageView.Where(item => item.Supplier.Contains(paramlist.Supplier.Trim()));
            }
            //型号、订单号、供应商
            if (!string.IsNullOrEmpty(paramlist.MultiField))
            {
                storageView = storageView.Where(item => item.Product.PartNumber.Contains(paramlist.MultiField.Trim())
                                                     || item.Input.OrderID.Contains(paramlist.MultiField.Trim())
                                                     || item.Supplier.Contains(paramlist.MultiField.Trim()));
            }
            //型号、供应商
            if (!string.IsNullOrEmpty(paramlist.MultiField2))
            {
                storageView = storageView.Where(item => item.Product.PartNumber.Contains(paramlist.MultiField2.Trim())
                                                     || item.Supplier.Contains(paramlist.MultiField2.Trim()));
            }

            int total = storageView.Count();
            var data = storageView.Skip((paramlist.page - 1) * paramlist.rows).Take(paramlist.rows).ToArray().Select(item => new StorageListViewModel
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                PartNumber = item.Product.PartNumber,
                CustomsName = item.Product.CustomsName,
                Manufacturer = item.Product.Manufacturer,
                Supplier = item.Supplier,
                Currency = item.Input.Currency?.GetDescription(),
                CurrencyShortName = item.Input.Currency?.GetCurrency()?.ShortName,
                CurrencyInt = item.Input.Currency?.GetHashCode().ToString(),
                UnitPrice = item.Input.UnitPrice?.ToString("0.00"),
                Quantity = item.Quantity.ToString("0"),
                WareHouseName = item.WareHouseID.StartsWith("HK") ? "香港库房" : "深圳库房",
                TotalPrice = (item.Input.UnitPrice * item.Quantity)?.ToString("0.00"),
                WareHouseID = item.WareHouseID,
                IsCheck = false, //我的库存页面复选框
                InputID = item.InputID,
                Origin = item.Origin,
                PackageCase = item.Product.PackageCase,
                DateCode = item.DateCode,
                Num = item.Quantity,

                SaleQuantity = item.Quantity, //购物车一开始的数量
            });

            return this.Paging(data, total);
        }

        /// <summary>
        /// 我的库存分部页
        /// </summary>
        /// <returns></returns>
        public ActionResult _PartialMyStorage(string frompage)
        {
            ViewData["frompage"] = frompage; // frompage 这个变量是从 RenderAction 中传过来的
            return PartialView();
        }
        #endregion

        /// <summary>
        /// 获取所有库房
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetWarehouses()
        {
            Dictionary<string, string> whs = new Dictionary<string, string>();
            var hkwarehouse = Yahv.Services.WhSettings.HK;
            var szwarehouse = Yahv.Services.WhSettings.SZ;

            whs.Add(hkwarehouse.ID, hkwarehouse.Name);
            whs.Add(szwarehouse.ID, szwarehouse.Name);

            return whs;
        }
    }
}