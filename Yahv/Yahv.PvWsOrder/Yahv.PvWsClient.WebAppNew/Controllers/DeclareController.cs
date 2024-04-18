using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Yahv.Linq.Extends;
using Yahv.PvWsClient.WebAppNew.App_Utils;
using Yahv.PvWsClient.WebAppNew.Controllers.Attribute;
using Yahv.PvWsClient.WebAppNew.Models;
using Yahv.PvWsOrder.Services;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsClient.WebAppNew.Controllers
{
    public class DeclareController : UserController
    {
        //protected override void OnResultExecuted(ResultExecutedContext filterContext)
        //{
        //    base.OnResultExecutedFunction(filterContext);
        //}

        #region 新增,提交报关单

        /// <summary>
        /// 新增报关单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareAdd(string id)
        {
            DeclareAddViewModel model = new DeclareAddViewModel();
            var current = Client.Current;
            //收货地址
            var receiveOptions = current.MyConsignees.Where(item => item.Place == Origin.CHN.ToString()).Select(item => new { value = item.ID, text = item.Title, address = item.Address, name = item.Name, mobile = item.Mobile, palce = item.Place }).ToArray();

            // 用户协议
            var agreemrnt = current.MyAgreement.FirstOrDefault();
            // 报关类型
            var declareTypeName = string.Empty;
            if (agreemrnt != null)
            {
                if (agreemrnt.InvoiceType == PvWsOrder.Services.XDTClientView.Invoice.Full)
                {
                    declareTypeName = "单抬头";
                }
                else if (agreemrnt.InvoiceType == PvWsOrder.Services.XDTClientView.Invoice.Service)
                {
                    declareTypeName = "双抬头";
                }
            }

            model.OrderItems = new Models.OrderItem[0];
            model.HKFiles = new FileModel[0];
            model.PIFiles = new FileModel[0];
            //model.PackingFiles = new FileModel[0];
            model.PayExchangeSupplier = new string[0];  //付汇供应商实例化
            //香港交货方式 默认快递
            model.HKDeliveryType = ((int)WaybillType.LocalExpress).ToString();
            model.SZDeliveryType = ((int)WaybillType.LocalExpress).ToString();
            //香港仓库 地址 联系人 电话
            var HKwarehouse = Services.WhSettings.HK[PvClientConfig.WareHouseID]; //深圳默认库房
            var contact1 = Alls.Current.Company[HKwarehouse.Enterprise.ID]?.Contacts.FirstOrDefault();
            model.SZPickAddress = PvClientConfig.SZWareHouseID; //默认一个自提库房
            model.WareHouseAddress = HKwarehouse.Address;
            model.WareHouseName = contact1?.Name;
            model.WareHouseTel = contact1?.Tel;
            model.EnterCode = current.MyClients.EnterCode;

            #region 是否快捷下单
            var para = Request.Form["para"];
            if (!string.IsNullOrWhiteSpace(para))
            {
                var ids = para.Split(',');
                var product = current.MyClassifiedPreProducts.Where(item => ids.Contains(item.ID)).ToArray();
                if (!product.Any())
                {
                    return View("Error"); //待提交订单方可修改
                }
                model.OrderItems = product.Select(item => new Models.OrderItem
                {
                    PreproductID = item.ID,
                    ProductUnicode = item.ProductUnionCode,
                    Manufacturer = item.Manufacturer,
                    Name = item.ProductName,
                    PartNumber = item.Model,
                    Unit = (int)LegalUnit.个,
                }).ToArray();
                model.IsClssified = true;
                var currency = ExtendsEnum.ToArray<Currency>().FirstOrDefault(item => item.ToString() == product.First().Currency);
                model.Currency = ((int)currency).ToString();
            }
            #endregion

            #region 是否由我的库存跳转

            var products = Request.Form["products"];
            if (!string.IsNullOrWhiteSpace(products))
            {
                var productList = JsonConvert.DeserializeObject<StorageListViewModel[]>(products).Select(item => new
                {
                    item.ID,
                    item.SaleQuantity,
                    item.CurrencyShortName,
                    item.CurrencyInt,
                }).ToArray();
                var productListIDs = productList.Select(item => item.ID).ToArray();

                var storageList = Yahv.Client.Current.MyStorages.Where(t => productListIDs.Contains(t.ID)).ToArray();
                model.OrderItems = (from storage in storageList
                                    join product in productList on storage.ID equals product.ID
                                    select new Models.OrderItem
                                    {
                                        StorageID = storage.ID, //StorageID
                                        Name = "", //Name - 品名
                                        Manufacturer = storage.Product.Manufacturer, //Manufacturer - 品牌
                                        PartNumber = storage.Product.PartNumber, //PartNumber - 型号
                                        Origin = storage.Origin, //Origin - 产地
                                        Quantity = product.SaleQuantity, //Quantity - 数量
                                        StockNum = storage.Quantity, //StockNum - 库存数量
                                        Unit = (int)LegalUnit.个, //Unit - 单位
                                        TotalPrice = 0, //TotalPrice - 总价
                                        UnitPrice = 0, //UnitPrice - 单价
                                        InputID = storage.InputID, //InputID
                                        CurrencyShortName = product.CurrencyShortName, //币种 ShortName
                                        CurrencyInt = product.CurrencyInt, //币种 Int
                                    }).ToArray();

                var currency = ExtendsEnum.ToArray<Currency>().FirstOrDefault(item => item.ToString() == productList.First().CurrencyShortName);
                model.Currency = ((int)currency).ToString();

                model.IsTransfer = true;
            }

            #endregion

            if (!string.IsNullOrWhiteSpace(id))  //订单编辑
            {
                var order = current.MyDeclareOrders.GetOrderDetail(id);
                if (order == null || order.MainStatus != CgOrderStatus.暂存)
                {
                    return View("Error"); //待提交订单方可修改
                }
                model.ID = order.ID;
                model.IsEdit = true;
                //付汇供应商
                model.PayExchangeSupplier = order.PayExchangeSupplier.Select(item => item.ID).ToArray();
                //订单项
                var orderItems = order.OrderItems.Select(item => new Models.OrderItem
                {
                    ID = item.ID,
                    InputID = item.InputID,
                    Manufacturer = item.Product.Manufacturer,
                    Name = item.Name,
                    Origin = item.Origin,
                    OutputID = item.OutputID,
                    PartNumber = item.Product.PartNumber,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice,
                    Unit = (int)item.Unit,
                    UnitPrice = item.UnitPrice,
                    StorageID = item.StorageID,
                    StockNum = (current.MyStorages[item.StorageID]?.Quantity).GetValueOrDefault(),
                }).ToArray();

                var productListIDs = orderItems.Select(t => t.StorageID).ToArray();
                var storageList = Yahv.Client.Current.MyStorages.Where(t => productListIDs.Contains(t.ID)).ToArray();
                if (storageList != null && storageList.Any() && orderItems != null && orderItems.Any())
                {
                    for (int i = 0; i < orderItems.Length; i++)
                    {
                        var thestorage = storageList.Where(t => t.ID == orderItems[i].StorageID).FirstOrDefault();
                        if (thestorage != null)
                        {
                            orderItems[i].CurrencyShortName = thestorage.Input.Currency?.GetCurrency()?.ShortName;
                            orderItems[i].CurrencyInt = thestorage.Input.Currency?.GetHashCode().ToString();
                        }
                    }
                }
                model.OrderItems = orderItems;

                model.IsTransfer = order.Type != OrderType.Declare;
                if (!model.IsTransfer)
                {
                    //香港交货方式
                    model.HKDeliveryType = ((int)order.InWaybill.Type).ToString();
                    model.HKExpressNumber = order.InWaybill.Code;
                    model.HKExpressName = order.InWaybill.CarrierID;
                    if (order.InWaybill.Type == WaybillType.PickUp)
                    {
                        var wayLoading = order.InWaybill.WayLoading;
                        model.PickupTime = wayLoading.TakingDate;
                        model.SupplierAddress = new PvWsOrder.Services.ClientViews.MySupplierConsignors(Client.Current.EnterpriseID, model.PayExchangeSupplier).
                            FirstOrDefault(item => item.Address == wayLoading.TakingAddress && item.Contact == wayLoading.TakingContact && item.Mobile == wayLoading.TakingPhone)?.ID;
                    }
                    model.HKFreight = order.InWaybill.FreightPayer == WaybillPayer.Consignee;
                }
                //国内交货方式
                var outWayLoading = order.OutWaybill.WayLoading;
                model.SZPickupTime = outWayLoading?.TakingDate;
                model.SZDeliveryType = ((int)order.OutWaybill.Type).ToString();
                model.ClientPicker = outWayLoading?.TakingContact;
                model.ClientPickerMobile = outWayLoading?.TakingPhone;
                model.IDType = ((int?)order.OutWaybill.Consignee.IDType).ToString();
                model.IDNumber = order.OutWaybill.Consignee.IDNumber;
                var consignee = order.OutWaybill.Consignee;
                var address = receiveOptions.FirstOrDefault(item => item.name == consignee.Contact && item.address == consignee.Address && item.mobile == consignee.Phone);
                model.ClientConsignee = address?.value;
                model.ClientConsigneeName = address?.name;
                model.ClientContactMobile = address?.mobile;
                model.ClientConsigneeAddress = address?.address;
                model.SZPickAddress = Services.WhSettings.TH.Doors.FirstOrDefault(item => item.Name == order.OutWaybill.Consignor?.Company)?.ID;
                model.CharteredBus = order.OutWaybill.WayCondition.IsCharterBus;

                //其他信息
                model.PackNo = order.OutWaybill.TotalParts;
                model.GrossWeight = order.OutWaybill.TotalWeight;

                //提货文件
                model.HKFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).Select(item => new FileModel
                {
                    ID = item.ID,
                    name = item.CustomName,
                    URL = item.Url,
                    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                }).ToArray();
                //PI文件
                model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new FileModel
                {
                    ID = item.ID,
                    name = item.CustomName,
                    URL = item.Url,
                    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                }).ToArray();
                //装箱文件
                //model.PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new FileModel
                //{
                //    name = item.CustomName,
                //    URL = item.Url,
                //    fileFormat = Path.GetExtension(item.CustomName).ToLower()
                //}).ToArray();
                model.Currency = ((int?)order.Output.Currency).ToString();
                model.IsClssified = order.OrderItems.Any(item => !string.IsNullOrWhiteSpace(item.PreProductID));
            }
            //基础数据
            var data = new
            {
                CarrierOptions = Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
                UnitOptions = UnitEnumHelper.ToUnitDictionary()
                .Where(t => WidelyUsedUnit.Values.Contains(t.Value))
                .Select(item => new { value = item.Value, text = item.Name }).ToArray(), // + " (" + item.Code + ")"
                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "ZZZ" && item.Value != "NG" && item.Value != "Unknown")
                .Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray(),
                ReceiveOptions = receiveOptions,
                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>()
                                    .Where(t => t.Key != IDType.PickSeal.GetHashCode().ToString())
                                    .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PackTypeOptions = ExtendsEnum.ToDictionary<Package>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0" && item.Key != Currency.CNY.GetHashCode().ToString())
                    .Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayCurrencyDefault = new { value = Currency.USD.GetHashCode().ToString(), text = Currency.USD.GetDescription(), },
                HKDeliveryTypeOptions = // ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0")
                new [] { WaybillType.LocalExpress, WaybillType.DeliveryToWarehouse, WaybillType.PickUp, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "上门提货" : item.Value,
                }).ToArray(),
                SZDeliveryTypeOptions = // ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0")
                new[] { WaybillType.LocalExpress, WaybillType.DeliveryToWarehouse, WaybillType.PickUp, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "客户自提" : item.Value,
                }).ToArray(),
                SZPickAddressOptions = Services.WhSettings.TH.Doors.Select(item => new { value = item.ID, text = item.Name + "(" + item.Address + ")" })
            };

            ViewBag.DeclareTypeName = declareTypeName;
            ViewBag.Options = data;
            return View(model);
        }

        /// <summary>
        /// 报关单退回编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareEdit(string id)
        {
            var current = Client.Current;
            var order = current.MyDeclareOrders.GetOrderDetail(id);
            if (order == null || order.MainStatus != CgOrderStatus.退回)
            {
                return View("Error"); //待提交订单方可修改
            }
            DeclareAddViewModel model = new DeclareAddViewModel();
            //收货地址
            var receiveOptions = current.MyConsignees.Where(item => item.Place == Origin.CHN.ToString()).Select(item => new { value = item.ID, text = item.Title, address = item.Address, name = item.Name, mobile = item.Mobile, palce = item.Place }).ToArray();
            //香港仓库 地址 联系人 电话
            var HKwarehouse = Services.WhSettings.HK[PvClientConfig.WareHouseID]; //深圳默认库房
            var contact1 = Alls.Current.Company[HKwarehouse.Enterprise.ID]?.Contacts.FirstOrDefault();
            model.SZPickAddress = PvClientConfig.SZWareHouseID; //默认一个自提库房
            model.WareHouseAddress = HKwarehouse.Address;
            model.WareHouseName = contact1?.Name;
            model.WareHouseTel = contact1?.Tel;
            model.EnterCode = current.MyClients.EnterCode;
            model.ID = order.ID;
            //付汇供应商
            model.PayExchangeSupplier = order.PayExchangeSupplier.Select(item => item.ID).ToArray();
            //订单项
            model.OrderItems = order.OrderItems.Select(item => new Models.OrderItem
            {
                ID = item.ID,
                InputID = item.InputID,
                TinyOrderID = item.TinyOrderID,
                Manufacturer = item.Product.Manufacturer,
                Name = item.Name,
                Origin = item.Origin,
                OutputID = item.OutputID,
                PartNumber = item.Product.PartNumber,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                Unit = (int)item.Unit,
                UnitPrice = item.UnitPrice,
                StorageID = item.StorageID,
                StockNum = (current.MyStorages[item.StorageID]?.Quantity).GetValueOrDefault(),
                Status = (int)item.Status
            }).ToArray();
            model.IsTransfer = order.Type != OrderType.Declare;
            if (!model.IsTransfer)
            {
                //香港交货方式
                model.HKDeliveryType = ((int)order.InWaybill.Type).ToString();
                model.HKExpressNumber = order.InWaybill.Code;
                model.HKExpressName = order.InWaybill.CarrierID;
                if (order.InWaybill.Type == WaybillType.PickUp)
                {
                    var wayLoading = order.InWaybill.WayLoading;
                    model.PickupTime = wayLoading.TakingDate;
                    model.SupplierAddress = new PvWsOrder.Services.ClientViews.MySupplierConsignors(Client.Current.EnterpriseID, model.PayExchangeSupplier).
                        FirstOrDefault(item => item.Address == wayLoading.TakingAddress && item.Contact == wayLoading.TakingContact && item.Mobile == wayLoading.TakingPhone)?.ID;
                }
                model.HKFreight = order.InWaybill.FreightPayer == WaybillPayer.Consignee;
            }
            //国内交货方式
            var outWayLoading = order.OutWaybill.WayLoading;
            model.SZPickupTime = outWayLoading?.TakingDate;
            model.SZDeliveryType = ((int)order.OutWaybill.Type).ToString();
            model.ClientPicker = outWayLoading?.TakingContact;
            model.ClientPickerMobile = outWayLoading?.TakingPhone;
            model.IDType = ((int?)order.OutWaybill.Consignee.IDType).ToString();
            model.IDNumber = order.OutWaybill.Consignee.IDNumber;
            var consignee = order.OutWaybill.Consignee;
            var address = receiveOptions.FirstOrDefault(item => item.name == consignee.Contact && item.address == consignee.Address && item.mobile == consignee.Phone);
            model.ClientConsignee = address?.value;
            model.ClientConsigneeName = address?.name;
            model.ClientContactMobile = address?.mobile;
            model.ClientConsigneeAddress = address?.address;
            model.SZPickAddress = Services.WhSettings.TH.Doors.FirstOrDefault(item => item.Name == order.OutWaybill.Consignor?.Company)?.ID;
            model.CharteredBus = order.OutWaybill.WayCondition.IsCharterBus;

            //其他信息
            model.PackNo = order.OutWaybill.TotalParts;
            model.GrossWeight = order.OutWaybill.TotalWeight;

            //提货文件
            model.HKFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).Select(item => new FileModel
            {
                ID = item.ID,
                name = item.CustomName,
                URL = item.Url,
                fileFormat = Path.GetExtension(item.CustomName).ToLower()
            }).ToArray();
            //PI文件
            model.PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new FileModel
            {
                ID = item.ID,
                name = item.CustomName,
                URL = item.Url,
                fileFormat = Path.GetExtension(item.CustomName).ToLower()
            }).ToArray();
            //装箱文件
            //model.PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new FileModel
            //{
            //    name = item.CustomName,
            //    URL = item.Url,
            //    fileFormat = Path.GetExtension(item.CustomName).ToLower()
            //}).ToArray();
            model.Currency = ((int?)order.Output.Currency).ToString();
            //基础数据
            var data = new
            {
                CarrierOptions = Alls.Current.Carriers.ToList().Select(item => new { value = item.ID, text = item.Name }).ToArray(),
                UnitOptions = UnitEnumHelper.ToUnitDictionary().Select(item => new { value = item.Value, text = item.Name + " (" + item.Code + ")" }).ToArray(),
                OriginOptions = ExtendsEnum.ToNameDictionary<Origin>().Where(item => item.Value != "ZZZ" && item.Value != "NG" && item.Value != "Unknown")
                .Select(item => new { value = item.Value, text = item.Value + " " + item.Name }).OrderBy(item => item.value).ToArray(),
                SupplierOptions = current.MySupplier.Select(item => new { value = item.ID, text = item.EnglishName }).ToArray(),
                ReceiveOptions = receiveOptions,
                IDTypeOptions = ExtendsEnum.ToDictionary<IDType>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PackTypeOptions = ExtendsEnum.ToDictionary<Package>().Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                PayCurrencyOptions = ExtendsEnum.ToDictionary<Currency>().Where(item => item.Key != "0").Select(item => new { value = item.Key, text = item.Value }).ToArray(),
                HKDeliveryTypeOptions = // ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0")
                new[] { WaybillType.LocalExpress, WaybillType.DeliveryToWarehouse, WaybillType.PickUp, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Key == "1" ? "上门提货" : item.Value,
                }).ToArray(),
                SZDeliveryTypeOptions = // ExtendsEnum.ToDictionary<WaybillType>().Where(item => item.Key != "4" && item.Key != "0")
                new[] { WaybillType.LocalExpress, WaybillType.DeliveryToWarehouse, WaybillType.PickUp, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {

                    value = item.Key,
                    text = item.Key == "1" ? "客户自提" : item.Value,
                }).ToArray(),
                SZPickAddressOptions = Services.WhSettings.TH.Doors.Select(item => new { value = item.ID, text = item.Name + "(" + item.Address + ")" })
            };
            ViewBag.Options = data;
            return View(model);
        }

        /// <summary>
        /// 报关单提交
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult DeclareSubmit(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<DeclareAddViewModel>(data);

                if (model.Currency == Currency.CNY.GetHashCode().ToString())
                {
                    return JsonResult(VueMsgType.error, "货值币种不能为人民币");
                }

                var current = Client.Current;
                var clientinfo = current.MyClients; //客户基本信息

                DeclareOrder order = string.IsNullOrWhiteSpace(model.ID) ? new DeclareOrder() : current.MyDeclareOrders.GetOrderDetail(model.ID);

                //订单
                order.ID = model.ID;
                order.ClientID = current.EnterpriseID;
                order.CreatorID = current.ID;
                order.Type = model.IsTransfer ? OrderType.TransferDeclare : OrderType.Declare;
                order.PayeeID = PvClientConfig.CompanyID;
                //order.BeneficiaryID = Alls.Current.CompanyPayee.ID;  //公司的受益人
                order.MainStatus = model.IsSubmit ? CgOrderStatus.待审核 : CgOrderStatus.暂存;
                order.InvoiceID = current.MyInvoice.SingleOrDefault()?.ID;
                order.SupplierID = model.PayExchangeSupplier[0]; //默认第一个付汇供应商
                order.EnterCode = model.EnterCode;
                order.PayExchangeSuppliers = model.PayExchangeSupplier;
                order.SettlementCurrency = Currency.CNY;

                PvWsOrder.Services.Models.OrderCondition orderCondition = new PvWsOrder.Services.Models.OrderCondition();
                var orderInputs = order.Input ?? new OrderInput();
                var wayCondition = new WayCondition()
                {
                    PayForFreight = model.HKFreight,
                    IsCharterBus = model.CharteredBus,
                };
                if (!model.IsTransfer)
                {
                    #region 代收
                    orderCondition.IsCharterBus = model.CharteredBus;
                    //入库
                    orderInputs.IsPayCharge = false;
                    orderInputs.Conditions = orderCondition.Json();
                    orderInputs.Currency = (Currency)int.Parse(model.Currency);
                    order.Input = orderInputs;

                    //获取香港库房信息
                    var HKWarehouse = Services.WhSettings.HK[PvClientConfig.WareHouseID];
                    var HKWarehouseEnterprise = Alls.Current.Company[HKWarehouse.Enterprise.ID];
                    var contact1 = HKWarehouseEnterprise?.Contacts.FirstOrDefault();

                    var supplier = current.MySupplier[model.PayExchangeSupplier[0]];
                    var pickAddress = new PvWsOrder.Services.ClientViews.MySupplierConsignors(Client.Current.EnterpriseID, model.PayExchangeSupplier).FirstOrDefault(c => c.ID == model.SupplierAddress);
                    order.InWaybill = order.InWaybill ?? new PvWsOrder.Services.ClientModels.Waybill();
                    order.InWaybill.Consignee = new WayParter
                    {
                        Company = HKWarehouse.Name,
                        Address = HKWarehouse.Address,
                        Contact = contact1?.Name,
                        Phone = contact1?.Mobile,
                        Email = contact1?.Email,
                        Place = Origin.HKG.GetOrigin().Code,
                    };
                    order.InWaybill.Consignor = new WayParter
                    {
                        Company = clientinfo.Name,
                        Place = clientinfo.Place,
                    };
                    order.InWaybill.WayLoading = (WaybillType)int.Parse(model.HKDeliveryType) != WaybillType.PickUp ? null : new WayLoading
                    {
                        TakingDate = DateTime.Parse(model.PickupTimeStr),
                        TakingAddress = pickAddress?.Address,
                        TakingContact = pickAddress?.Contact,
                        TakingPhone = pickAddress?.Mobile,
                        CreatorID = current.ID,
                        ModifierID = current.ID,
                    };
                    order.InWaybill.Code = model.HKExpressNumber;
                    order.InWaybill.CarrierID = model.HKExpressName;
                    order.InWaybill.Type = (WaybillType)int.Parse(model.HKDeliveryType);
                    order.InWaybill.FreightPayer = model.HKFreight ? WaybillPayer.Consignee : WaybillPayer.Consignor;
                    order.InWaybill.TotalParts = model.PackNo;
                    order.InWaybill.TotalWeight = model.GrossWeight;
                    order.InWaybill.CreatorID = current.ID;
                    order.InWaybill.ModifierID = current.ID;
                    order.InWaybill.Condition = wayCondition.Json();
                    order.InWaybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                    order.InWaybill.Supplier = supplier?.RealEnterpriseName;
                    order.InWaybill.EnterCode = model.EnterCode;
                    order.InWaybill.NoticeType = Services.Enums.CgNoticeType.Enter;
                    order.InWaybill.Source = model.IsTransfer ? Services.Enums.CgNoticeSource.AgentCustomsFromStorage : Services.Enums.CgNoticeSource.AgentBreakCustoms;
                    #endregion 
                }

                #region 代发
                //条件
                orderCondition = new PvWsOrder.Services.Models.OrderCondition { IsCharterBus = model.CharteredBus };

                wayCondition = new WayCondition { IsCharterBus = model.CharteredBus };

                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition
                {
                    IsCharterBus = model.CharteredBus
                };

                //出库
                var orderOutputs = new OrderOutput
                {
                    Currency = (Currency)int.Parse(model.Currency),
                    IsReciveCharge = false,
                    Conditions = orderCondition.Json()
                };
                order.Output = orderOutputs;

                //运单
                WaybillPayer freightPayer;
                //深圳库房信息
                var SZwarehouse = Services.WhSettings.TH[model.SZPickAddress ?? PvClientConfig.SZWareHouseID];

                //收货人信息
                var consignee = new WayParter
                {
                    Company = clientinfo.Name,
                    Place = Origin.CHN.ToString(),
                };
                if ((WaybillType)int.Parse(model.SZDeliveryType) == WaybillType.PickUp)
                {
                    consignee.Contact = model.ClientPicker;
                    consignee.Phone = model.ClientPickerMobile;
                    consignee.IDType = string.IsNullOrWhiteSpace(model.IDType) ? null : (IDType?)int.Parse(model.IDType);
                    consignee.IDNumber = model.IDNumber;
                    freightPayer = WaybillPayer.Consignee;
                    consignee.Address = SZwarehouse.Address; //深圳库房 
                }
                else
                {
                    consignee.Contact = model.ClientConsigneeName;
                    consignee.Phone = model.ClientContactMobile;
                    consignee.Address = model.ClientConsigneeAddress;
                    freightPayer = WaybillPayer.Consignor;
                }

                order.OutWaybill = order.OutWaybill ?? new PvWsOrder.Services.ClientModels.Waybill();
                order.OutWaybill.Consignee = consignee;
                order.OutWaybill.Consignor = new WayParter
                {
                    Company = SZwarehouse.Name,
                };
                order.OutWaybill.WayLoading = (WaybillType)int.Parse(model.SZDeliveryType) != WaybillType.PickUp ? null : new WayLoading
                {
                    TakingDate = DateTime.Parse(model.SZPickupTimeStr),
                    TakingAddress = SZwarehouse.Address,
                    TakingContact = model.ClientPicker,
                    TakingPhone = model.ClientPickerMobile,
                    CreatorID = current.ID,
                    ModifierID = current.ID,
                };
                order.OutWaybill.Type = (WaybillType)int.Parse(model.SZDeliveryType);
                order.OutWaybill.FreightPayer = freightPayer;
                order.OutWaybill.TotalParts = model.PackNo;
                order.OutWaybill.TotalWeight = model.GrossWeight;
                order.OutWaybill.CreatorID = current.ID;
                order.OutWaybill.ModifierID = current.ID;
                order.OutWaybill.EnterCode = model.EnterCode;
                order.OutWaybill.Condition = wayCondition.Json();
                order.OutWaybill.ExcuteStatus = (int)CgPickingExcuteStatus.Picking;
                order.OutWaybill.NoticeType = model.IsTransfer ? Services.Enums.CgNoticeType.Boxing : Services.Enums.CgNoticeType.Out;
                order.OutWaybill.Source = model.IsTransfer ? Services.Enums.CgNoticeSource.AgentCustomsFromStorage : Services.Enums.CgNoticeSource.AgentBreakCustoms;
                #endregion

                //订单项    
                order.OrderItems = model.OrderItems.Select(item => new PvWsOrder.Services.ClientModels.OrderItem
                {
                    Product = new CenterProduct
                    {
                        PartNumber = item.PartNumber.Trim(),
                        Manufacturer = item.Manufacturer.Trim(),
                    },
                    ProductUnicode = item.ProductUnicode,
                    PreProductID = item.PreproductID,
                    ID = item.ID,
                    Name = item.Name.Trim(),
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
                    Quantity = item.Quantity,
                    Currency = (Currency)int.Parse(model.Currency),
                    UnitPrice = item.UnitPrice,
                    Unit = (LegalUnit)item.Unit,
                    TotalPrice = item.TotalPrice,
                    Conditions = orderItemCondition.Json(),
                    StorageID = item.StorageID,
                }).ToArray();

                //订单附件

                //PI文件
                var files = model.PIFiles.Where(item => item.ID == null).Select(item => new CenterFileDescription { Type = (int)FileType.Invoice, CustomName = item.name, Url = item.URL, AdminID = current.ID }).ToList();

                //提货文件
                files.AddRange(model.HKFiles.Where(item => item.ID == null).Select(item => new CenterFileDescription { Type = (int)FileType.Delivery, CustomName = item.name, Url = item.URL, AdminID = current.ID }));

                //装箱文件
                //files.AddRange(model.PackingFiles.Select(item => new CenterFileDescription { Type = (int)FileType.FollowGoods, CustomName = item.name, Url = item.URL, AdminID = current.ID }));

                order.OrderFiles = files.ToArray();
                order.XDTClientName = current.XDTClientName;
                order.Enter();
                return JsonResult(VueMsgType.success, "新增成功", order.ID);
            }
            catch (Exception ex)
            {
                Client.Current.Errorlog.Log("报关订单保存失败：" + ex.Message + "/nSummary:" + ex.StackTrace);
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 报关单提交退回编辑
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult DeclareEditSubmit(string data)
        {
            try
            {
                var model = JsonConvert.DeserializeObject<DeclareAddViewModel>(data);
                var current = Client.Current;
                DeclareOrder order = current.MyDeclareOrders.GetOrderDetail(model.ID);
                if (order == null)
                {
                    return JsonResult(VueMsgType.error, "保存失败：未找到该订单");
                }

                //订单
                order.ID = model.ID;
                var orderItemCondition = new PvWsOrder.Services.Models.OrderItemCondition
                {
                    IsCharterBus = model.CharteredBus
                };
                //订单项    
                order.OrderItems = model.OrderItems.Where(c => c.Status != 200).Select(item => new PvWsOrder.Services.ClientModels.OrderItem
                {
                    Product = new CenterProduct
                    {
                        PartNumber = item.PartNumber.Trim(),
                        Manufacturer = item.Manufacturer.Trim(),
                    },
                    TinyOrderID = item.TinyOrderID,
                    ProductUnicode = item.ProductUnicode,
                    ID = item.ID,
                    Name = item.Name.Trim(),
                    InputID = item.InputID,
                    OutputID = item.OutputID,
                    Origin = string.IsNullOrWhiteSpace(item.Origin) ? (Origin.Unknown).ToString() : item.Origin,
                    Quantity = item.Quantity,
                    Currency = (Currency)int.Parse(model.Currency),
                    UnitPrice = item.UnitPrice,
                    Unit = (LegalUnit)item.Unit,
                    TotalPrice = item.TotalPrice,
                    Conditions = orderItemCondition.Json(),
                    StorageID = item.StorageID,
                    Status = OrderItemStatus.Returned
                }).ToArray();
                order.EnterCode = current.EnterCode;
                order.XDTClientName = current.XDTClientName;
                order.ReturnEnter();
                return JsonResult(VueMsgType.success, "新增成功", order.ID);
            }
            catch (Exception ex)
            {
                Client.Current.Errorlog.Log("报关订单保存失败：" + ex.Message);
                return JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }
        #endregion


        #region 报关订单列表

        /// <summary>
        /// 列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult DeclareList()
        {
            var defaultOrderStatus = string.Empty;
            var param = Request.Form["param"];
            if (!string.IsNullOrEmpty(param))
            {
                var paramModel = JsonConvert.DeserializeObject<LinkToDeclareListModel>(param);
                if (int.TryParse(paramModel.dec_status, out var decStatusInt))
                {
                    defaultOrderStatus = decStatusInt.ToString();
                }
            }

            var orderstatuslist = new [] { CgOrderStatus.待审核, CgOrderStatus.待确认, CgOrderStatus.待交货, CgOrderStatus.已装箱,
                                           CgOrderStatus.待报关,CgOrderStatus.待收货, CgOrderStatus.客户已收货, };
            ViewBag.OrderStatusOptions = // ExtendsEnum.ToDictionary<CgOrderStatus>().Where(item => orderstatuslist.Contains(item.Value))
                orderstatuslist.ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Value,
                }).ToArray();
            ViewBag.DefaultOrderStatus = defaultOrderStatus;

            ViewBag.InvoiceStatusOptions = ExtendsEnum.ToDictionary<OrderInvoiceStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            ViewBag.RemittanceStatusOptions = ExtendsEnum.ToDictionary<OrderRemittanceStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 列表-精简版
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult DeclareListSimple()
        {
            var orderStatusList = new string[] { CgOrderStatus.退回.GetDescription(),CgOrderStatus.暂存.GetDescription(),
                CgOrderStatus.取消.GetDescription() };
            ViewBag.OrderStatusOptions = ExtendsEnum.ToDictionary<CgOrderStatus>().Where(item => orderStatusList.Contains(item.Value))
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Value,
                }).ToArray();

            ViewBag.InvoiceStatusOptions = ExtendsEnum.ToDictionary<OrderInvoiceStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            ViewBag.RemittanceStatusOptions = ExtendsEnum.ToDictionary<OrderRemittanceStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();
            return View();
        }

        /// <summary>
        /// 根据条件查询列表数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetDeclareList()
        {
            var declareorder = Client.Current.MyDeclareOrders;

            #region 查询条件
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                OrderStatus = Request.Form["OrderStatus"],
                InvoiceStatus = Request.Form["InvoiceStatus"],
                Supplier = Request.Form["Supplier"],
                RemittanceStatus = Request.Form["RemittanceStatus"],
            };
            if (!string.IsNullOrWhiteSpace(paramlist.Supplier))
            {
                declareorder = declareorder.SearchBySupplier(paramlist.Supplier.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                declareorder = declareorder.SearchByPart(paramlist.PartNumber.Trim());
            }

            //固定查询条件,过滤状态数据
            var orderstatuslist = new CgOrderStatus[] { CgOrderStatus.待审核, CgOrderStatus.待确认, CgOrderStatus.待交货, CgOrderStatus.已装箱, 
                                                        CgOrderStatus.待报关, CgOrderStatus.待收货, CgOrderStatus.客户已收货 };

            if (IsMobileLogin())
            {
                var orderstatuslist_1 = new List<CgOrderStatus>(orderstatuslist);
                orderstatuslist_1.AddRange(new CgOrderStatus[] { CgOrderStatus.退回, CgOrderStatus.暂存, CgOrderStatus.取消 });
                orderstatuslist = orderstatuslist_1.ToArray();
            }

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => orderstatuslist.Contains(item.MainStatus);
            lambdas.Add(lambda);

            if (!string.IsNullOrWhiteSpace(paramlist.startDate))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(paramlist.startDate);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.endDate))
            {
                lambda = item => item.CreateDate < DateTime.Parse(paramlist.endDate).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                lambda = item => item.ID.Contains(paramlist.OrderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.InvoiceStatus))
            {
                lambda = item => item.InvoiceStatus == (OrderInvoiceStatus)int.Parse(paramlist.InvoiceStatus);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderStatus))
            {
                lambda = item => item.MainStatus == (CgOrderStatus)int.Parse(paramlist.OrderStatus);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.RemittanceStatus))
            {
                lambda = item => item.RemittanceStatus == (OrderRemittanceStatus)int.Parse(paramlist.RemittanceStatus);
                lambdas.Add(lambda);
            }
            #endregion

            var data = declareorder.GetPageListOrders(lambdas.ToArray(), paramlist.rows, paramlist.page);
            var list = data.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                item.SupplierName,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                RemittanceStatus = item.RemittanceStatus.GetDescription(),
                OrderStatus = item.MainStatus.GetDescription(),
                OrderStatusInt = item.MainStatus.GetHashCode(),
                item.Type,
                OrderType = item.Type.GetDescription(),
                Currency = item.Output.Currency?.GetDescription(),
                item.TotalPrice,
                IsCancel = item.MainStatus == CgOrderStatus.待确认,
                IsEdit = item.MainStatus == CgOrderStatus.暂存,
                IsReturn = item.MainStatus == CgOrderStatus.退回,
                IsUnConfirmed = item.MainStatus == CgOrderStatus.待确认,

                TotalTraiff = item.TotalTraiff < 50 ? "0.00" : item.TotalTraiff.ToString("0.00"),
                TotalAddTax = item.TotalAddTax < 50 ? "0.00" : item.TotalAddTax.ToString("0.00"),
                TotalAgencyFee = item.TotalAgencyFee.ToString("0.00"),
                TotalInspectionFee = item.TotalInspectionFee.ToString("0.00"),
                IsTransfer = item.Type == OrderType.TransferDeclare,
            });
            return this.Paging(list, data.Total);
        }

        /// <summary>
        /// 根据条件查询列表数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetDeclareListSimple()
        {
            var declareOrder = Client.Current.MyDeclareOrders;

            #region 查询条件
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                OrderStatus = Request.Form["OrderStatus"],
                InvoiceStatus = Request.Form["InvoiceStatus"],
                Supplier = Request.Form["Supplier"],
                RemittanceStatus = Request.Form["RemittanceStatus"],
            };
            if (!string.IsNullOrWhiteSpace(paramlist.Supplier))
            {
                declareOrder = declareOrder.SearchBySupplier(paramlist.Supplier.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                declareOrder = declareOrder.SearchByPart(paramlist.PartNumber.Trim());
            }

            //固定查询条件,过滤状态数据
            var orderStatusList = new CgOrderStatus[] { CgOrderStatus.退回, CgOrderStatus.暂存, CgOrderStatus.取消 };
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => orderStatusList.Contains(item.MainStatus);
            lambdas.Add(lambda);

            if (!string.IsNullOrWhiteSpace(paramlist.startDate))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(paramlist.startDate);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.endDate))
            {
                lambda = item => item.CreateDate < DateTime.Parse(paramlist.endDate).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                lambda = item => item.ID.Contains(paramlist.OrderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.InvoiceStatus))
            {
                lambda = item => item.InvoiceStatus == (OrderInvoiceStatus)int.Parse(paramlist.InvoiceStatus);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderStatus))
            {
                lambda = item => item.MainStatus == (CgOrderStatus)int.Parse(paramlist.OrderStatus);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.RemittanceStatus))
            {
                lambda = item => item.RemittanceStatus == (OrderRemittanceStatus)int.Parse(paramlist.RemittanceStatus);
                lambdas.Add(lambda);
            }
            #endregion

            var data = declareOrder.GetPageListOrders(lambdas.ToArray(), paramlist.rows, paramlist.page);
            var list = data.Select(item => new
            {
                item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                item.SupplierName,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                RemittanceStatus = item.RemittanceStatus.GetDescription(),
                OrderStatus = item.MainStatus.GetDescription(),
                item.Type,
                Currency = item.Output.Currency?.GetDescription(),
                item.TotalPrice,
                IsEdit = item.MainStatus == CgOrderStatus.暂存,
                IsReturn = item.MainStatus == CgOrderStatus.退回
            });
            return this.Paging(list, data.Total);
        }

        #endregion


        #region 报关单详情
        /// <summary>
        /// 报关单订单详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            var order = Client.Current.MyDeclareOrders.GetOrderDetail(id);
            if (order != null)
            {
                //代收自提文件
                var fileurl = PvClientConfig.FileServerUrl + @"/";
                var deliveryFile = order.OrderFiles.Where(item => item.Type == (int)FileType.Delivery).FirstOrDefault();
                var _deliveryFile = "";
                if (deliveryFile != null)
                {
                    _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
                }
                //代收合同发票
                var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
                {
                    Name = item.CustomName,
                    Url = fileurl + item.Url,
                }).ToArray();
                //装箱单
                //var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
                //{
                //    Name = item.CustomName,
                //    Url = fileurl + item.Url,
                //}).ToArray();
                //对账单
                var orderBillURL = "";
                var orderBillName = "";
                var orderBillStatus = false;
                var orderBill = order.OrderFiles.Where(item => item.Type == (int)FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
                var IsUploadOrderBill = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传对账单
                if (orderBill != null)
                {
                    orderBillURL = fileurl + orderBill.Url;
                    orderBillName = orderBill.CustomName;
                    orderBillStatus = orderBill.Status == FileDescriptionStatus.Approved;
                    IsUploadOrderBill = IsUploadOrderBill && orderBill.Status == FileDescriptionStatus.Audting;
                }
                //报关委托书
                var orderAgentURL = "";
                var orderAgentName = "";
                var orderAgentStatus = false;
                var orderAgent = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
                var IsUploadOrderAgent = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传报关委托书
                if (orderAgent != null)
                {
                    orderAgentURL = fileurl + orderAgent.Url;
                    orderAgentName = orderAgent.CustomName;
                    orderAgentStatus = orderAgent.Status == Services.Models.FileDescriptionStatus.Approved;
                    IsUploadOrderAgent = IsUploadOrderAgent && orderAgent.Status == Services.Models.FileDescriptionStatus.Audting;
                }
                //销售合同
                var orderSalesURL = "";
                var orderSalesName = "";
                var orderSalesStatus = false;
                var orderSales = order.OrderFiles.Where(item => item.Type == (int)FileType.SalesContract).OrderByDescending(item => item.CreateDate).FirstOrDefault();
                var IsUploadOrderSales = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传销售合同
                if (orderSales != null)
                {
                    orderSalesURL = fileurl + orderSales.Url;
                    orderSalesName = orderSales.CustomName;
                    orderSalesStatus = orderSales.Status == Services.Models.FileDescriptionStatus.Approved;
                    IsUploadOrderSales = IsUploadOrderSales && orderSales.Status == Services.Models.FileDescriptionStatus.Audting;
                }
                var orderItems = order.OrderItems.Select(item => new
                {
                    item.Product.PartNumber,
                    Name = item.ClassfiedName ?? item.Name,
                    item.Product.Manufacturer,
                    OriginLabel = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                    item.Quantity,
                    item.UnitPrice,
                    UnitLabel = item.Unit.GetDescription(),
                    item.TotalPrice,
                    item.ProductUnicode,
                }).ToArray();
                //开票类型
                var invoiceType = new PvWsOrder.Services.XDTClientView.OrderAgreementTopView().GetTypeByOrderID(order.ID);

                //查询快递单号
                var wbCodeModels = new Yahv.PvWsOrder.Services.ClientViews.CgWaybillsTopView()
                                                        .Where(t => t.OrderID == order.ID
                                                                 && t.NoticeType == Services.Enums.CgNoticeType.Out
                                                                 && t.WareHouseID.StartsWith("SZ")
                                                                 && t.wbCode != null).ToArray();

                string wbCodes = "";
                if (wbCodeModels != null && wbCodeModels.Length > 0)
                {
                    var codes = wbCodeModels.Select(t => t.wbCode).ToArray();
                    wbCodes = string.Join(",", codes);
                }

                var model = new
                {
                    order.ID,
                    CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreateDateDateString = order.CreateDate.ToString("yyyy-MM-dd"),
                    TotalMoney = $"{order.OrderItems.Sum(c => c.TotalPrice):N}", //报关总价
                    //账单
                    OrderBillURL = orderBillURL,
                    OrderBillName = orderBillName,
                    OrderBillStatus = orderBillStatus,
                    IsUploadOrderBill,
                    //代理报关委托书
                    OrderAgentURL = orderAgentURL,
                    OrderAgentName = orderAgentName,
                    OrderAgentStatus = orderAgentStatus,
                    IsUploadOrderAgent,
                    //销售合同
                    OrderSalesURL = orderSalesURL,
                    OrderSalesName = orderSalesName,
                    OrderSalesStatus = orderSalesStatus,
                    IsUploadOrderSales = (int)invoiceType == 0 && IsUploadOrderSales,
                    XDTInvoiceType = (int)invoiceType,
                    PIFiles, //PI
                    //PackingFiles, //装箱单
                    OrderItems = orderItems,
                    MainStatus = order.MainStatus.GetDescription(),
                    order.InWaybill.TotalParts, //总件数
                    order.InWaybill.TotalWeight,//总毛重
                    Currency = order.Input.Currency.GetDescription(), //币种
                    HKDeliveryTypeName = order.InWaybill.Type.GetDescription(), //香港交货方式
                    HKDeliveryType = order.InWaybill.Type,
                    //香港自提
                    TakingDetailAddress = order.InWaybill.WayLoading?.TakingAddress,
                    TakingDate = order.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                    order.InWaybill.WayLoading?.TakingContact,
                    order.InWaybill.WayLoading?.TakingPhone,
                    DeliveryFile = _deliveryFile, //提货文件
                    HKIsFreightPayer = order.InWaybill.FreightPayer == WaybillPayer.Consignee ? "是" : "否", //是否垫付运费
                    IsFullVehicle = order.OutWaybill.WayCondition.IsCharterBus ? "是" : "否", //是否包车
                    //香港快递
                    order.InWaybill.Code,
                    order.InWaybill.CarrierName,
                    //深圳交货
                    SZDeliveryTypeName = order.OutWaybill.Type.GetDescription(),
                    OutWaybillCode = (order.OutWaybill.Type == WaybillType.LocalExpress ? order.OutWaybill.Code : ""),
                    SZDeliveryType = order.OutWaybill.Type,
                    //深圳自提
                    OutTakingContact = order.OutWaybill.Consignee?.Contact,
                    OutTakingPhone = order.OutWaybill.Consignee?.Phone,
                    SZTakingDetailAddress = order.OutWaybill.WayLoading?.TakingAddress,
                    SZTakingDate = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                    CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
                    Certificate = order.OutWaybill.Consignee?.IDNumber,
                    //深圳送货上门、快递
                    ReceivedContact = order.OutWaybill.Consignee.Contact,
                    ReceivedPhone = order.OutWaybill.Consignee.Phone,
                    ReceivedAddress = order.OutWaybill.Consignee.Address,
                    WbCodes = wbCodes,
                    PayExchangeSupplier = order.PayExchangeSupplier.Select(c => c.RealEnterpriseName).ToArray(), //付汇供应商

                };
                return View(model);
            }
            return View("Error");
        }

        /// <summary>
        /// 转报关详情
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult TransDecDetail()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];

            var order = Client.Current.MyDeclareOrders.GetOrderDetail(id);
            if (order == null)
            {
                return View("Error");
            }
            var fileurl = PvClientConfig.FileServerUrl + @"/";
            //代收合同发票
            var PIFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();

            //对账单
            var orderBillURL = "";
            var orderBillName = "";
            var orderBillStatus = false;
            var orderBill = order.OrderFiles.Where(item => item.Type == (int)FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            var IsUploadOrderBill = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传对账单
            if (orderBill != null)
            {
                orderBillURL = fileurl + orderBill.Url;
                orderBillName = orderBill.CustomName;
                orderBillStatus = orderBill.Status == FileDescriptionStatus.Approved;
                IsUploadOrderBill = IsUploadOrderBill && orderBill.Status == FileDescriptionStatus.Audting;
            }
            //报关委托书
            var orderAgentURL = "";
            var orderAgentName = "";
            var orderAgentStatus = false;
            var orderAgent = order.OrderFiles.Where(item => item.Type == (int)FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            var IsUploadOrderAgent = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传报关委托书
            if (orderAgent != null)
            {
                orderAgentURL = fileurl + orderAgent.Url;
                orderAgentName = orderAgent.CustomName;
                orderAgentStatus = orderAgent.Status == FileDescriptionStatus.Approved;
                IsUploadOrderAgent = IsUploadOrderAgent && orderAgent.Status == FileDescriptionStatus.Audting;
            }
            //销售合同
            var orderSalesURL = "";
            var orderSalesName = "";
            var orderSalesStatus = false;
            var orderSales = order.OrderFiles.Where(item => item.Type == (int)FileType.SalesContract).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            var IsUploadOrderSales = order.MainStatus > CgOrderStatus.待确认 && order.MainStatus != CgOrderStatus.取消; //客户已确认才可上传销售合同
            if (orderSales != null)
            {
                orderSalesURL = fileurl + orderSales.Url;
                orderSalesName = orderSales.CustomName;
                orderSalesStatus = orderSales.Status == Services.Models.FileDescriptionStatus.Approved;
                IsUploadOrderSales = IsUploadOrderSales && orderSales.Status == Services.Models.FileDescriptionStatus.Audting;
            }
            //装箱单
            //var PackingFiles = order.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            //{
            //    Name = item.CustomName,
            //    Url = fileurl + item.Url,
            //}).ToArray();
            var orderItems = order.OrderItems.Select(item => new
            {
                item.Product.PartNumber,
                Name = item.ClassfiedName ?? item.Name,
                item.Product.Manufacturer,
                OriginLabel = ExtendsEnum.ToNameDictionary<Origin>().FirstOrDefault(a => a.Value == item.Origin)?.Name,
                item.Quantity,
                item.UnitPrice,
                UnitLabel = item.Unit.GetDescription(),
                item.TotalPrice,
            }).ToArray();
            //var outOrderCondition = JsonConvert.DeserializeObject<PvWsOrder.Services.Models.OrderCondition>(order.Output.Conditions);
            //开票类型
            var invoiceType = new PvWsOrder.Services.XDTClientView.OrderAgreementTopView().GetTypeByOrderID(order.ID);
            var model = new
            {
                order.ID,
                MainStatus = order.MainStatus.GetDescription(),
                CreateDate = order.CreateDate.ToString("yyyy-MM-dd"),
                Currency = order.Output.Currency.GetDescription(),
                PIFiles,
                //PackingFiles,
                SZDeliveryTypeName = order.OutWaybill.Type.GetDescription(),
                OutWaybillCode = (order.OutWaybill.Type == WaybillType.LocalExpress ? order.OutWaybill.Code : ""),
                SZDeliveryType = order.OutWaybill.Type,
                //自提
                SZTakingDetailAddress = order.OutWaybill.WayLoading?.TakingAddress,
                SZTakingDate = order.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                OutTakingContact = order.OutWaybill.Consignee?.Contact,
                OutTakingPhone = order.OutWaybill.Consignee?.Phone,
                CertificateType = order.OutWaybill.Consignee?.IDType?.GetDescription(),
                Certificate = order.OutWaybill.Consignee?.IDNumber,
                //送货上门、快递
                ReceivedContact = order.OutWaybill.Consignee?.Contact,
                ReceivedPhone = order.OutWaybill.Consignee?.Phone,
                ReceivedAddress = order.OutWaybill.Consignee?.Address,
                IsFullVehicle = order.OutWaybill.WayCondition.IsCharterBus ? "是" : "否", //是否包车
                order.OutWaybill.TotalParts,
                TotalMoney = $"{order.OrderItems.Sum(c => c.TotalPrice):N}",
                OrderItems = orderItems,
                PayExchangeSupplier = order.PayExchangeSupplier.Select(c => c.RealEnterpriseName).ToArray(),
                OrderBillURL = orderBillURL,
                OrderBillName = orderBillName,
                OrderBillStatus = orderBillStatus,
                IsUploadOrderBill,
                OrderAgentURL = orderAgentURL,
                OrderAgentName = orderAgentName,
                OrderAgentStatus = orderAgentStatus,
                IsUploadOrderAgent,
                //销售合同
                OrderSalesURL = orderSalesURL,
                OrderSalesName = orderSalesName,
                OrderSalesStatus = orderSalesStatus,
                IsUploadOrderSales = (int)invoiceType == 0 && IsUploadOrderSales,
                XDTInvoiceType = (int)invoiceType,
            };
            return View(model);
        }
        #endregion


        #region 取消订单
        /// <summary>
        /// 待客户确认取消订单
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <param name="reason">取消原因</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult CancelConfirm(string orderID, string reason)
        {
            try
            {
                Client.Current.MyDeclareOrders.ClientCancel(orderID, reason);
                Payments.PaymentManager.Erp(Client.Current.ID).Received.Abolish(orderID);
                return JsonResult(VueMsgType.success, "取消成功");
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, "取消失败");
            }
        }
        #endregion


        #region 订单待确认收货列表
        /// <summary>
        /// 订单待确认收货列表
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult UnReceiptedList()
        {
            ViewBag.ConfirmReceiptStatusOptions = ExtendsEnum.ToDictionary<Yahv.PvWsOrder.Services.ClientViews.ConfirmReceiptStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value,
            }).ToArray();

            ViewBag.ConfirmReceiptStatusDefaultValue = Yahv.PvWsOrder.Services.ClientViews.ConfirmReceiptStatus.UnConfirm.GetHashCode().ToString();

            ViewBag.WaybillTypeOptions = new[] { WaybillType.LocalExpress, WaybillType.DeliveryToWarehouse, WaybillType.PickUp, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Select(item => new
                {
                    value = item.Key,
                    text = item.Value,  // item.Key == "1" ? "客户自提" : item.Value,
                }).ToArray();

            return View();
        }

        /// <summary>
        /// 根据条件筛选数据
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetUnReceiptedList()
        {
            var paralist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                OrderID = Request.Form["OrderID"],
            };

            var confirmReceiptStatus = Request.Form["confirmReceiptStatus"];
            int? confirmReceiptStatusInt = null;
            if (!string.IsNullOrEmpty(confirmReceiptStatus) && int.TryParse(confirmReceiptStatus, out int confirmReceiptStatus_1))
            {
                confirmReceiptStatusInt = confirmReceiptStatus_1;
            }

            var waybillType = Request.Form["waybillType"];
            int? waybillTypeInt = null;
            if (!string.IsNullOrEmpty(waybillType) && int.TryParse(waybillType, out int waybillType_1))
            {
                waybillTypeInt = waybillType_1;
            }

            //var orders = Client.Current.MyUnReceiptedOrders.GetAllConfirmReceiptOrders(confirmReceiptStatusInt).Where(item => true);


            var expOrder = ((Expression<Func<Yahv.Services.Models.WsOrder, bool>>)null).And(item => true);

            //页面查询条件过滤
            if (!string.IsNullOrWhiteSpace(paralist.startDate))
            {
                // orders = orders.Where(item => item.CreateDate >= DateTime.Parse(paralist.startDate));
                expOrder = expOrder.And(item => item.CreateDate >= DateTime.Parse(paralist.startDate));
            }
            if (!string.IsNullOrWhiteSpace(paralist.endDate))
            {
                // orders = orders.Where(item => item.CreateDate <= DateTime.Parse(paralist.endDate).AddDays(1));
                expOrder = expOrder.And(item => item.CreateDate <= DateTime.Parse(paralist.endDate).AddDays(1));
            }
            if (!string.IsNullOrWhiteSpace(paralist.OrderID))
            {
                // orders = orders.Where(item => item.ID.Contains(paralist.OrderID.Trim()));
                expOrder = expOrder.And(item => item.ID.Contains(paralist.OrderID.Trim()));
            }

            // orders = orders.OrderByDescending(t => t.ID);

            var tuple = Client.Current.MyUnReceiptedOrders.GetAllConfirmReceiptOrders(expOrder, confirmReceiptStatusInt, waybillTypeInt, paralist.page, paralist.rows);

            var data = tuple.Item1.ToArray().Select(item => new
            {
                OrderID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                CreateDateTimeString = item.CreateDate.ToString("HH:mm:ss"),
                MainStatus = item.MainStatus.GetDescription(),
                Type = (int)item.Type,
                Waybills = item.OutWaybills.Select(way => new
                {
                    way.ID,
                    Type = way.Type.GetDescription(),
                    CreateDate = way.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreateDateDateString = way.CreateDate.ToString("yyyy-MM-dd"),
                    CreateDateTimeString = way.CreateDate.ToString("HH:mm:ss"),
                    way.Code,
                    consigneeName = way.Consignee.Contact,
                    consigneeCompany = way.Consignee.Company,
                    confirmReceiptStatus = way.ConfirmReceiptStatus,
                }).ToArray(),
            });

            // return this.Paging(data);
            return JsonResult(VueMsgType.success, "", new { list = data, total = tuple.Item2, }.Json());
        }


        /// <summary>
        /// 代报关订单客户确认收货
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="WaybillID"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public JsonResult ReceiptedOrder(string OrderID, string WaybillID)
        {
            try
            {
                Client.Current.MyUnReceiptedOrders.ReceiptOrder(OrderID, WaybillID);
                return JsonResult(VueMsgType.success, "订单确认成功");
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }

        }
        #endregion


        #region 待确认报关单列表
        /// <summary>
        /// 待确认报关订单
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public ActionResult UnConfirmedDecList()
        {
            var needShowOrderTypes = new []
            {
                OrderType.Transport,
                OrderType.Delivery,
                OrderType.TransferDeclare,
                OrderType.Declare,
            };
            var needShowOrderTypesValue = needShowOrderTypes.Select(t => t.GetHashCode().ToString()).ToArray();
            ViewBag.OrderTypeOptions = // ExtendsEnum.ToDictionary<OrderType>()
                new[] { OrderType.TransferDeclare, OrderType.Declare, }
                .ToDictionary(item => item.GetHashCode().ToString(), item => item.GetDescription())
                .Where(t => needShowOrderTypesValue.Contains(t.Key))
                .Select(item => new
                {
                    value = item.Key,
                    text = ChangeOrderTypeNameInUnConfirmedDecList(item.Key, item.Value),
                }).ToArray();
            return View();
        }

        /// <summary>
        /// UnConfirmedDecList 页面中修改 OrderType 显示的名字
        /// </summary>
        private string ChangeOrderTypeNameInUnConfirmedDecList(string value, string text)
        {
            if (value == OrderType.TransferDeclare.GetHashCode().ToString())
            {
                return "库存";
            }
            else if (value == OrderType.Declare.GetHashCode().ToString())
            {
                return "报关";
            }
            else
            {
                return text;
            }
        }

        /// <summary>
        /// 根据筛选条件查询数据结果集
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetUnConfirmedList()
        {
            var unconfirmedOrders = Client.Current.MyUnConfirmedOrders;

            #region 查询条件
            var paramlist = new
            {
                page = int.Parse(Request.Form["page"]),
                rows = int.Parse(Request.Form["rows"]),
                startDate = Request.Form["startDate"],
                endDate = Request.Form["endDate"],
                PartNumber = Request.Form["PartNumber"],
                OrderID = Request.Form["OrderID"],
                Supplier = Request.Form["Supplier"],
                OrderType = Request.Form["OrderType"],
            };
            if (!string.IsNullOrWhiteSpace(paramlist.PartNumber))
            {
                unconfirmedOrders = unconfirmedOrders.SearchByPart(paramlist.PartNumber.Trim());
            }
            if (!string.IsNullOrWhiteSpace(paramlist.Supplier))
            {
                unconfirmedOrders = unconfirmedOrders.SearchBySupplier(paramlist.Supplier.Trim());
            }
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => true;
            if (!string.IsNullOrWhiteSpace(paramlist.startDate))
            {
                lambda = item => item.CreateDate >= DateTime.Parse(paramlist.startDate);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.endDate))
            {
                lambda = item => item.CreateDate < DateTime.Parse(paramlist.endDate).AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderID))
            {
                lambda = item => item.ID.Contains(paramlist.OrderID.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrWhiteSpace(paramlist.OrderType))
            {
                lambda = item => item.Type == (OrderType)int.Parse(paramlist.OrderType);
                lambdas.Add(lambda);
            }

            // 该列表只显示“代报关”和“转报关”的订单
            Expression<Func<WsOrder, bool>> lambdaOrderType = item => true;
            lambdaOrderType = item => new[] { OrderType.TransferDeclare, OrderType.Declare, }.Contains(item.Type);
            lambdas.Add(lambdaOrderType);

            #endregion

            var data = unconfirmedOrders.GetPageListOrders(lambdas.ToArray(), paramlist.rows, paramlist.page);
            var list = data.Select(item => new
            {
                item.ID,
                item.SupplierName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                CreateDateDateString = item.CreateDate.ToString("yyyy-MM-dd"),
                OrderType = item.Type.GetDescription(),
                Currency = item.Output.Currency.GetDescription(),
                TotalPrice = item.TotalPrice.ToString("0.00"),
                TotalTraiff = item.TotalTraiff < 50 ? "0.00" : item.TotalTraiff.ToString("0.00"),
                TotalAddTax = item.TotalAddTax < 50 ? "0.00" : item.TotalAddTax.ToString("0.00"),
                TotalAgencyFee = item.TotalAgencyFee.ToString("0.00"),
                TotalInspectionFee = item.TotalInspectionFee.ToString("0.00"),
                IsTransfer = item.Type == OrderType.TransferDeclare,
            });
            return this.Paging(list, data.Total);
        }
        #endregion


        #region 待确认报关单详情
        /// <summary>
        /// 报关订单已报价详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true)]
        public ActionResult DeclareConfirm()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            //获取订单数据
            var orderdetail = Client.Current.MyUnConfirmedOrders.GetOrderDetail(id);
            if (orderdetail == null)
            {
                return View("Error");
            }

            #region 文件
            var fileurl = PvClientConfig.FileServerUrl + @"/";
            //代收自提文件
            var deliveryFile = orderdetail.OrderFiles.FirstOrDefault(item => item.Type == (int)FileType.Delivery);
            var _deliveryFile = "";
            if (deliveryFile != null)
            {
                _deliveryFile = fileurl + deliveryFile.Url.ToUrl();
            }
            //代收合同发票
            var PIFiles = orderdetail.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            //var PackingFiles = orderdetail.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            //{
            //    Name = item.CustomName,
            //    Url = fileurl + item.Url,
            //}).ToArray();
            #endregion

            var orderItems = orderdetail.OrderItems.Select(item => new
            {
                item.ID,
                item.Product.PartNumber,
                Name = item.ClassfiedName ?? item.Name,
                item.Product.Manufacturer,
                item.Quantity,
                item.UnitPrice,
                item.TotalPrice,
                DeclareValue = item.DeclareTotalPrice,  //报关货值
                item.TraiffRate,
                item.Traiff,
                item.ExciseTaxRate,
                item.ExcisePrice,
                item.AddTaxRate,
                item.AddTax,
                item.AgencyFee,
                item.InspectionFee,//杂费
                TotalTaxFee = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee,  //税费合计
                TotalDeclareValue = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice, //报关总金额
                item.ItemsTerm,
            }).ToArray();

            //费用总价计算
            var totalentity = new
            {
                Quantity = orderItems.Sum(c => c.Quantity),
                TotalPrice = orderItems.Sum(c => c.TotalPrice),
                DeclareValue = orderItems.Sum(c => c.DeclareValue),
                Traiff = orderItems.Sum(c => c.Traiff) < 50 ? 0 : orderItems.Sum(c => c.Traiff),
                Excise = orderItems.Sum(c => c.ExcisePrice) < 50 ? 0 : orderItems.Sum(c => c.ExcisePrice),
                AddTax = orderItems.Sum(c => c.AddTax) < 50 ? 0 : orderItems.Sum(c => c.AddTax),
                AgencyFee = orderItems.Sum(c => c.AgencyFee).Round(),
                InspectionFee = orderItems.Sum(c => c.InspectionFee).Round()
            };
            var Products_TotalTaxFee = totalentity.Traiff + totalentity.Excise + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee;
            var Products_TotalDeclareValue = totalentity.Traiff + totalentity.Excise + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee + totalentity.DeclareValue;

            var model = new
            {
                orderdetail.ID,
                Currency = orderdetail.Input.Currency?.GetDescription(),
                CurrencyCode = orderdetail.Input.Currency?.GetCurrency().ShortName,
                orderdetail.InWaybill.TotalParts,
                orderdetail.InWaybill.TotalWeight,
                orderdetail.SupplierName,
                HKDeliveryType = orderdetail.InWaybill.Type,
                HKDeliveryTypeName = orderdetail.InWaybill.Type.GetDescription(),
                TakingDate = orderdetail.InWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                orderdetail.InWaybill.WayLoading?.TakingContact,
                orderdetail.InWaybill.WayLoading?.TakingAddress,
                orderdetail.InWaybill.WayLoading?.TakingPhone,
                IsFullVehicle = orderdetail.OutWaybill.WayCondition.IsCharterBus ? "是" : "否",
                HKIsFreightPayer = orderdetail.InWaybill.WayCondition.PayForFreight ? "是" : "否",
                orderdetail.InWaybill.Code,
                orderdetail.InWaybill.CarrierName,
                SZDeliveryType = orderdetail.OutWaybill.Type,
                SZDeliveryTypeName = orderdetail.OutWaybill.Type.GetDescription(),
                SZTakingDate = orderdetail.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                SZTakingDetailAddress = orderdetail.OutWaybill.WayLoading?.TakingAddress,
                OutTakingContact = orderdetail.OutWaybill.Consignee?.Contact,
                OutTakingPhone = orderdetail.OutWaybill.Consignee?.Phone,
                CertificateType = orderdetail.OutWaybill.Consignee.IDType?.GetDescription(),
                Certificate = orderdetail.OutWaybill.Consignee.IDNumber,
                ReceivedContact = orderdetail.OutWaybill.Consignee.Contact,
                ReceivedPhone = orderdetail.OutWaybill.Consignee.Phone,
                ReceivedAddress = orderdetail.OutWaybill.Consignee.Address,
                OrderItems = orderItems.Select(item => new
                {
                    item.ID,
                    item.PartNumber,
                    item.Name,
                    item.Manufacturer,
                    item.Quantity,
                    item.UnitPrice,
                    TotalPrice = item.TotalPrice.ToString("0.00"),
                    DeclareValue = item.DeclareValue.ToString("0.00"),
                    TraiffRate = item.TraiffRate.ToString("0.0000"),
                    Traiff = item.Traiff.ToString("0.00"),
                    ExciseTaxRate = item.ExciseTaxRate.ToString("0.0000"),
                    ExcisePrice = item.ExcisePrice.ToString("0.00"),
                    AddTaxRate = item.AddTaxRate.ToString("0.0000"),
                    AddTax = item.AddTax.ToString("0.00"),
                    AgencyFee = item.AgencyFee.ToString("0.00"),
                    InspectionFee = item.InspectionFee.ToString("0.00"),
                    TotalTaxFee = item.TotalTaxFee.ToString("0.00"),
                    TotalDeclareValue = item.TotalDeclareValue.ToString("0.00"),
                    item.ItemsTerm.Ccc,
                    item.ItemsTerm.Embargo,
                    item.ItemsTerm.Coo,
                    item.ItemsTerm.CIQ,
                    item.ItemsTerm.IsHighPrice,
                    item.ItemsTerm.HkControl,
                }).ToArray(),
                Products_Num = totalentity.Quantity,
                Products_TotalPrice = totalentity.TotalPrice.ToString("0.00"),
                Products_DeclareValue = totalentity.DeclareValue.ToString("0.00"),
                Products_Traiff = totalentity.Traiff.ToString("0.00"),
                Products_Excise = totalentity.Excise.ToString("0.00"),
                Products_AddTax = totalentity.AddTax.ToString("0.00"),
                Products_AgencyFee = totalentity.AgencyFee.ToString("0.00"),
                Products_InspectionFee = totalentity.InspectionFee.ToString("0.00"),
                Products_TotalTaxFee = Products_TotalTaxFee.ToString("0.00"),
                Products_TotalDeclareValue = Products_TotalDeclareValue.ToString("0.00"),
                PIFiles,
                //PackingFiles,
                DeliveryFile = _deliveryFile,
            };

            return View(model);
        }

        /// <summary>
        /// 转报关订单已报价详情
        /// </summary>
        /// <returns></returns>
        public ActionResult TransDecConfirm()
        {
            var para = Request.Form["para"];
            if (string.IsNullOrWhiteSpace(para))
            {
                return View("Error");
            }
            var paraArr = JsonConvert.DeserializeObject<string[]>(para);
            ViewBag.lasturl = paraArr[1];
            var id = paraArr[0];
            //获取订单数据
            var orderdetail = Client.Current.MyUnConfirmedOrders.GetOrderDetail(id);
            if (orderdetail == null)
            {
                return View("Error");
            }

            #region 文件
            //代收自提文件
            var fileurl = PvClientConfig.FileServerUrl + @"/";
            //代收合同发票
            var PIFiles = orderdetail.OrderFiles.Where(item => item.Type == (int)FileType.Invoice).Select(item => new
            {
                Name = item.CustomName,
                Url = fileurl + item.Url,
            }).ToArray();
            //装箱单
            //var PackingFiles = orderdetail.OrderFiles.Where(item => item.Type == (int)FileType.FollowGoods).Select(item => new
            //{
            //    Name = item.CustomName,
            //    Url = fileurl + item.Url,
            //}).ToArray();
            #endregion

            var orderItems = orderdetail.OrderItems.Select(item => new
            {
                item.ID,
                item.Product.PartNumber,
                Name = item.ClassfiedName ?? item.Name,
                item.Product.Manufacturer,
                item.Quantity,
                item.UnitPrice,
                item.TotalPrice,
                DeclareValue = item.DeclareTotalPrice,  //报关货值
                item.TraiffRate,
                item.Traiff,
                item.AddTaxRate,
                item.AddTax,
                item.AgencyFee,
                item.ExciseTaxRate,
                item.ExcisePrice,
                item.InspectionFee,//杂费
                TotalTaxFee = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee,  //税费合计
                TotalDeclareValue = item.Traiff + item.ExcisePrice + item.AddTax + item.AgencyFee + item.InspectionFee + item.DeclareTotalPrice, //报关总金额
                item.ItemsTerm
            }).ToArray();

            //费用总价计算
            var totalentity = new
            {
                Quantity = orderItems.Sum(c => c.Quantity),
                TotalPrice = orderItems.Sum(c => c.TotalPrice),
                DeclareValue = orderItems.Sum(c => c.DeclareValue),
                Traiff = orderItems.Sum(c => c.Traiff) < 50 ? 0 : orderItems.Sum(c => c.Traiff),
                Excise = orderItems.Sum(c => c.ExcisePrice) < 50 ? 0 : orderItems.Sum(c => c.ExcisePrice),
                AddTax = orderItems.Sum(c => c.AddTax) < 50 ? 0 : orderItems.Sum(c => c.AddTax),
                AgencyFee = orderItems.Sum(c => c.AgencyFee).Round(),
                InspectionFee = orderItems.Sum(c => c.InspectionFee).Round(),
            };
            var Products_TotalTaxFee = totalentity.Traiff + totalentity.Excise + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee;
            var Products_TotalDeclareValue = totalentity.Traiff + totalentity.Excise + totalentity.AddTax + totalentity.AgencyFee + totalentity.InspectionFee + totalentity.DeclareValue;

            var model = new
            {
                orderdetail.ID,
                Currency = orderdetail.Output.Currency?.GetDescription(),
                CurrencyCode = orderdetail.Output.Currency?.GetCurrency().ShortName,
                orderdetail.OutWaybill.TotalParts,
                orderdetail.OutWaybill.TotalWeight,
                orderdetail.SupplierName,
                SZDeliveryType = orderdetail.OutWaybill.Type,
                SZDeliveryTypeName = orderdetail.OutWaybill.Type.GetDescription(),
                SZTakingDate = orderdetail.OutWaybill.WayLoading?.TakingDate?.ToString("yyyy-MM-dd"),
                OutTakingContact = orderdetail.OutWaybill.Consignee?.Contact,
                OutTakingPhone = orderdetail.OutWaybill.Consignee?.Phone,
                SZTakingDetailAddress = orderdetail.OutWaybill.WayLoading?.TakingAddress,
                CertificateType = orderdetail.OutWaybill.Consignee.IDType?.GetDescription(),
                Certificate = orderdetail.OutWaybill.Consignee.IDNumber,
                ReceivedContact = orderdetail.OutWaybill.Consignee.Contact,
                ReceivedPhone = orderdetail.OutWaybill.Consignee.Phone,
                ReceivedAddress = orderdetail.OutWaybill.Consignee.Address,
                OrderItems = orderItems.Select(item => new
                {
                    item.ID,
                    item.PartNumber,
                    item.Name,
                    item.Manufacturer,
                    item.Quantity,
                    item.UnitPrice,
                    TotalPrice = item.TotalPrice.ToString("0.00"),
                    DeclareValue = item.DeclareValue.ToString("0.00"),
                    TraiffRate = item.TraiffRate.ToString("0.0000"),
                    Traiff = item.Traiff.ToString("0.00"),
                    AddTaxRate = item.AddTaxRate.ToString("0.0000"),
                    AddTax = item.AddTax.ToString("0.00"),
                    ExciseTaxRate = item.ExciseTaxRate.ToString("0.0000"),
                    ExcisePrice = item.ExcisePrice.ToString("0.00"),
                    AgencyFee = item.AgencyFee.ToString("0.00"),
                    InspectionFee = item.InspectionFee.ToString("0.00"),
                    TotalTaxFee = item.TotalTaxFee.ToString("0.00"),
                    TotalDeclareValue = item.TotalDeclareValue.ToString("0.00"),
                    item.ItemsTerm.Ccc,
                    item.ItemsTerm.Embargo,
                    item.ItemsTerm.Coo,
                    item.ItemsTerm.CIQ,
                    item.ItemsTerm.IsHighPrice,
                    item.ItemsTerm.HkControl,
                }).ToArray(),
                Products_Num = totalentity.Quantity,
                Products_TotalPrice = totalentity.TotalPrice.ToString("0.00"),
                Products_DeclareValue = totalentity.DeclareValue.ToString("0.00"),
                Products_Traiff = totalentity.Traiff.ToString("0.00"),
                Products_AddTax = totalentity.AddTax.ToString("0.00"),
                Products_Excise = totalentity.Excise.ToString("0.00"),
                Products_AgencyFee = totalentity.AgencyFee.ToString("0.00"),
                Products_InspectionFee = totalentity.InspectionFee.ToString("0.00"),
                Products_TotalTaxFee = Products_TotalTaxFee.ToString("0.00"),
                Products_TotalDeclareValue = Products_TotalDeclareValue.ToString("0.00"),
                PIFiles,
                //PackingFiles,
            };

            return View(model);
        }
        #endregion


        #region 客户确认/取消报价

        /// <summary>
        /// 代报关订单客户确认
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, MobileLog = true)]
        public JsonResult Confirm(string OrderID)
        {
            try
            {
                Client.Current.MyUnConfirmedOrders.ClientConfirm(OrderID);
                return JsonResult(VueMsgType.success, "订单确认成功");
            }
            catch (Exception ex)
            {
                return JsonResult(VueMsgType.error, ex.Message);
            }

        }
        #endregion

        /// <summary>
        /// 获取未处理订单数量（手机端使用）
        /// </summary>
        /// <returns></returns>
        [UserAuthorize(UserAuthorize = true, InformalMembership = true)]
        public JsonResult GetUnHandleOrderCount()
        {
            var declareorder = Client.Current.MyDeclareOrders;

            var UnConfirmedDecCount = GetUnHandleOrderCount(new CgOrderStatus[] { CgOrderStatus.待确认 });
            var UnReceiptedCount = GetUnHandleOrderCount(new CgOrderStatus[] { CgOrderStatus.待收货 });

            var data = new
            {
                UnConfirmedDecCount,
                UnReceiptedCount,
            };

            return JsonResult(VueMsgType.success, "", data.Json());
        }

        private int GetUnHandleOrderCount(CgOrderStatus[] orderstatuslist)
        {
            var declareorder = Client.Current.MyDeclareOrders;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<WsOrder, bool>> lambda = item => orderstatuslist.Contains(item.MainStatus);
            lambdas.Add(lambda);
            int total = declareorder.GetUnHandleOrderCount(lambdas.ToArray());
            return total;
        }

    }
}