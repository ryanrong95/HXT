using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Linq;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Needs.Wl.Logs.Services;
using Needs.Wl.Models;
using Needs.Wl.User.Plat;
using Needs.Wl.Web.Mvc;
using Needs.Wl.Web.Mvc.Utils;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebMvc.Models;

namespace WebMvc.Controllers
{
    /// <summary>
    /// 会员订单
    /// 新增订单 我的订单 待确认订单 待付汇订单 
    /// </summary>
    [UserHandleError(ExceptionType = typeof(Exception))]
    public class OrdersController : UserController
    {
        #region 新增订单  修改订单 提交订单

        // GET: 新增订单
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Add(string id)
        {
            AddViewModel model = new AddViewModel();
            var current = Needs.Wl.User.Plat.UserPlat.Current;
            model.OrderProducts = new OrderProductModel[0];
            model.HKFiles = new FileModel[0];
            model.PIFiles = new FileModel[0];
            model.QueryIDs = id;
            //预归类下单
            if (!string.IsNullOrWhiteSpace(id))
            {
                var ids = id.Split(',');
                if (ids.Count() == 0)
                {
                    return View("Error");  //id错误
                }

                var view = current.MyClassifiedPreProducts;
                view.Predicate = s => s.ClassifyStatus == Needs.Wl.Models.Enums.ClassifyStatus.Done && ids.Contains(s.ID);
                view.AllowPaging = false;

                var firstproduct = view.FirstOrDefault();

                view.Predicate = s => s.ClassifyStatus == Needs.Wl.Models.Enums.ClassifyStatus.Done && ids.Contains(s.ID) && s.Currency == firstproduct.Currency;
                view.AllowPaging = false;

                //验证产品的币种都相同\是否已经全部归类
                if (view.RecordCount != ids.Count())
                {
                    return View("Error");  //产品未归类或币种不一致
                }

                model.IsClssified = true;
                model.Currency = firstproduct.Currency;
                model.OrderProducts = view.ToList().Select(item => new OrderProductModel
                {
                    Name = item.ProductName,
                    Manufacturer = item.Manufacturer,
                    Model = item.Model,
                    Unit = "007",
                    UnitLabel = "007 个",
                    Type = (Needs.Ccs.Services.Enums.ItemCategoryType)((int)item.Type),
                    TaxCode = item.TaxCode,
                    TaxName = item.TaxName,
                    HSCode = item.HSCode,
                    Elements = item.Elements,
                    Unit1 = item.Unit1,
                    Unit2 = item.Unit2,
                    CIQCode = item.CIQCode,
                    TariffRate = item.TariffRate,
                    AddedValueRate = item.AddedValueRate,
                    ExciseTaxRate = item.ExciseTaxRate,
                    ClassifyFirstOperatorID = item.ClassifyFirstOperator,
                    ClassifySecondOperatorID = item.ClassifySecondOperator,
                    //Declarant = item.ClassifySecondOperator,
                    InspectionFee = item.InspectionFee,
                    UnitPrice = 0,
                    ProductUnionCode = item.ProductUnionCode ?? string.Empty,
                }).ToArray();
            }
            return View(model);
        }

        public JsonResult GetAddData(string id)
        {
            AddViewModel model = new AddViewModel();
            var current = Needs.Wl.User.Plat.UserPlat.Current;
            //产品
            model.OrderProducts = new OrderProductModel[0];
            model.HKFiles = new FileModel[0];
            model.PIFiles = new FileModel[0];
            model.Currency = "USD";
            //预归类下单
            if (!string.IsNullOrWhiteSpace(id))
            {
                var ids = id.Split(',');
                if (ids.Count() == 0)
                {
                    return base.JsonResult(VueMsgType.error, "页面异常");
                }

                var view = current.MyClassifiedPreProducts;
                view.Predicate = s => s.ClassifyStatus == Needs.Wl.Models.Enums.ClassifyStatus.Done && ids.Contains(s.ID);
                view.AllowPaging = false;

                var firstproduct = view.FirstOrDefault();

                view.Predicate = s => s.ClassifyStatus == Needs.Wl.Models.Enums.ClassifyStatus.Done && ids.Contains(s.ID) && s.Currency == firstproduct.Currency;
                view.AllowPaging = false;

                //验证产品的币种都相同\是否已经全部归类
                if (view.RecordCount != ids.Count())
                {
                    return base.JsonResult(VueMsgType.error, "页面异常");
                }

                model.IsClssified = true;
                model.Currency = firstproduct.Currency;
                model.OrderProducts = view.ToList().Select(item => new OrderProductModel
                {
                    Name = item.ProductName,
                    Manufacturer = item.Manufacturer,
                    Model = item.Model,
                    Unit = "007",
                    UnitLabel = "007 个",
                    Type = (Needs.Ccs.Services.Enums.ItemCategoryType)((int)item.Type),
                    TaxCode = item.TaxCode,
                    TaxName = item.TaxName,
                    HSCode = item.HSCode,
                    Elements = item.Elements,
                    Unit1 = item.Unit1,
                    Unit2 = item.Unit2,
                    CIQCode = item.CIQCode,
                    TariffRate = item.TariffRate,
                    AddedValueRate = item.AddedValueRate,
                    ExciseTaxRate = item.ExciseTaxRate,
                    ClassifyFirstOperatorID = item.ClassifyFirstOperator,
                    ClassifySecondOperatorID = item.ClassifySecondOperator,
                    InspectionFee = item.InspectionFee,
                    UnitPrice = 0,
                    ProductUnionCode = item.ProductUnionCode,
                }).ToArray();
            }

            //数据绑定
            //币种
            var viewCurrencies = Needs.Wl.User.Plat.UserPlat.Currencies;
            viewCurrencies.AllowPaging = false;

            model.CurrencyOptions = viewCurrencies.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //国家/地区
            var viewCountries = Needs.Wl.User.Plat.UserPlat.Countries;
            viewCountries.AllowPaging = false;
            model.OriginOptions = viewCountries.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //单位
            var viewUnits = Needs.Wl.User.Plat.UserPlat.Units;
            viewUnits.AllowPaging = false;
            model.UnitOptions = viewUnits.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //包装
            var viewPackTypes = Needs.Wl.User.Plat.UserPlat.PackTypes;
            viewPackTypes.AllowPaging = false;
            model.WrapOptions = viewPackTypes.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //香港交货方式
            model.HKDeliveryTypeOptions = EnumUtils.ToDictionary<HKDeliveryType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //深圳交货方式
            model.SZDeliveryTypeOptions = EnumUtils.ToDictionary<SZDeliveryType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //证件类型
            model.IdTypeOptions = EnumUtils.ToDictionary<IDType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            var supplerView = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            supplerView.AllowPaging = false;
            model.SupplierOptions = supplerView.ToList().Select(item => new
            {
                value = item.ID,
                text = item.ChineseName
            }).Json();

            model.PayExchangeSupplierOptions = model.SupplierOptions;

            //是否预付款
            model.IsPrePaid = current.Client.Agreement().ProductFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;
            //收货地址
            var consignee = current.MyConsignees;
            consignee.AllowPaging = false;
            model.ClientConsigneeOptions = consignee.ToList().Select(item => new
            {
                value = item.ID,
                text = item.Name,
                name = item.Contact.Name,
                mobile = item.Contact.Mobile,
                address = item.Address
            }).Json();

            var defaultconsignee = consignee.GetDefault();
            if (defaultconsignee != null)
            {
                model.ClientConsignee = defaultconsignee.ID;  //默认收货地址
                model.ClientConsigneeName = defaultconsignee.Name;
                model.clientContact = defaultconsignee.Contact.Name;
                model.clientConsigneeAddress = defaultconsignee.Address;
                model.clientContactMobile = defaultconsignee.Contact.Mobile;
            }

            model.HKDeliveryType = ((int)HKDeliveryType.SentToHKWarehouse).ToString();  //香港交货方式默认为‘送货’
            model.SZDeliveryType = ((int)SZDeliveryType.PickUpInStore).ToString();  //国内交货方式默认为‘自提’
            model.PayExchangeSupplier = new string[0];  //付汇供应商实例化

            //供应商
            var supplierView = current.MySuppliers;
            if (supplierView.RecordCount > 0)
            {
                var address = supplierView.FirstOrDefault().Addresses();
                model.SupplierAddressOptions = address.ToArray().Select(item => new
                {
                    value = item.ID,
                    text = "联系人:" + item.Contact.Name + ";电话:" + item.Contact.Mobile + ";地址:" + item.Address,
                    address = item.Address,
                    contact = item.Contact.Name,
                    mobile = item.Contact.Mobile,
                    isDefault = item.IsDefault
                }).Json();

                var supplierDefaultAddress = address.GetDefault();
                if (supplierDefaultAddress != null)
                {
                    model.SupplierAddress = supplierDefaultAddress.ID;
                    model.SupplierAddressName = supplierDefaultAddress.Address;
                    model.supplierContact = supplierDefaultAddress.Contact.Name;
                    model.supplierContactMobile = supplierDefaultAddress.Contact.Mobile;
                }
            }

            model.WrapType = "22";
            return base.JsonResult(VueMsgType.success, "", model.Json());
        }

        /// <summary>
        /// 我的产品的分部视图
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult _PartialMyPreProducts()
        {
            var product = Needs.Wl.User.Plat.UserPlat.Current.MyProducts.ToList().Select(item => new
            {
                item.ID,
                item.Name,
                item.Model,
                item.Manufacturer,
                item.Batch,
            });
            return PartialView(product);
        }

        /// <summary>
        /// POST:新增订单确认
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult Adds(AddViewModel model)
        {
            try
            {
                #region 前台数据
                //1)录入的产品信息
                var productList = model.OrderProducts;
                //2)香港交货方式
                var hkDeliveryType = model.HKDeliveryType;
                var supplierID = model.Supplier;
                var waybillNO = model.WayBillNo;
                //var pickupTime = model.PickupTime;
                var supplierAddressName = model.SupplierAddressName;
                var supplierAddress = model.SupplierAddress;
                var supplierContact = model.supplierContact;
                var supplierContactMobile = model.supplierContactMobile;
                //3)国内交货方式
                var szDeliveryType = model.SZDeliveryType;
                var clientPicker = model.ClientPicker;
                var clientPickerMobile = model.ClientPickerMobile;
                var idType = model.IDType;
                var idNumber = model.IDNumber;
                var ClientConsigneeName = model.ClientConsigneeName;
                var clientConsigneeAddress = model.clientConsigneeAddress;
                var clientContact = model.clientContact;
                var clientContactMobile = model.clientContactMobile;
                //4)付汇供应商
                var payExchangeSupplierIDs = model.PayExchangeSupplier;
                //5)其他信息
                var currency = model.Currency;
                var isFullVehicle = model.IsFullVehicle;
                var isLoan = model.IsLoan;
                var wrapType = model.WrapType;
                var packNO = string.IsNullOrWhiteSpace(model.PackNo) ? null : (int?)int.Parse(model.PackNo);
                var summary = model.Summary;
                #endregion

                //用户ID
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                var userID = current.ID;
                var client = current.Client;//客户
                var user = current.ToUser();
                var supplier = client.Suppliers()[supplierID];//客户供应商

                //新增订单
                var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[model.ID];
                if (order == null)
                {
                    order = new Needs.Ccs.Services.Models.Order();
                    order.Client = client.ToCssClinet();
                    order.OrderConsignee = new Needs.Ccs.Services.Models.OrderConsignee();
                    order.OrderConsignee.OrderID = order.ID;
                    order.OrderConsignor = new Needs.Ccs.Services.Models.OrderConsignor();
                    order.OrderConsignor.OrderID = order.ID;
                }
                else
                {
                    order.Items.RemoveAll();
                    order.PayExchangeSuppliers.RemoveAll();
                    order.MainOrderFiles.RemoveAll();
                }

                order.SetUser(user.ToCssUser());
                order.AdminID = client.Merchandiser.ID;
                order.UserID = Needs.Wl.User.Plat.UserPlat.Current.ID;
                order.ClientAgreement = client.Agreement().ToCssClientAgreement();
                order.Currency = currency;
                order.IsFullVehicle = isFullVehicle;
                order.IsLoan = isLoan;
                order.PackNo = packNO;
                order.WarpType = wrapType;
                order.Summary = summary;
                order.Type = OrderType.Outside;
                order.CustomsExchangeRate = null;
                order.RealExchangeRate = null;

                //香港交货信息
                order.OrderConsignee.ClientSupplier = supplier.ToCssClientSupplier();
                order.OrderConsignee.Type = (HKDeliveryType)Enum.Parse(typeof(HKDeliveryType), hkDeliveryType);
                if (order.OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)  //送货
                {
                    order.OrderConsignee.WayBillNo = waybillNO;
                }
                else  //自提
                {
                    order.OrderConsignee.Contact = supplierContact;
                    order.OrderConsignee.Mobile = supplierContactMobile;
                    order.OrderConsignee.Address = supplierAddressName;
                    order.OrderConsignee.PickUpTime = DateTime.Parse(model.PickupTimeStr);

                    if (model.HKFiles.Length != 0)
                    {
                        order.MainOrderFiles.Add(new Needs.Ccs.Services.Models.MainOrderFile
                        {
                            MainOrderID = order.MainOrderID,
                            User = user.ToCssUser(),
                            Name = model.HKFiles[0].name,
                            FileType = FileType.DeliveryFiles,
                            FileFormat = model.HKFiles[0].fileFormat,
                            Url = model.HKFiles[0].URL
                        });
                    }
                }

                //国内交货信息
                order.OrderConsignor.Type = (SZDeliveryType)Enum.Parse(typeof(SZDeliveryType), szDeliveryType);
                if (order.OrderConsignor.Type == SZDeliveryType.PickUpInStore) //自提
                {
                    order.OrderConsignor.Contact = clientPicker;
                    order.OrderConsignor.Mobile = clientPickerMobile;
                    order.OrderConsignor.IDType = idType;
                    order.OrderConsignor.IDNumber = idNumber;
                }
                else
                {
                    order.OrderConsignor.Name = ClientConsigneeName;
                    order.OrderConsignor.Contact = clientContact;
                    order.OrderConsignor.Mobile = clientContactMobile;
                    order.OrderConsignor.Address = clientConsigneeAddress;
                }

                //付汇供应商
                foreach (var payExchangeSupplierID in payExchangeSupplierIDs)
                {
                    if (string.IsNullOrWhiteSpace(payExchangeSupplierID))
                        continue;

                    var payExchangeSupplier = client.Suppliers()[payExchangeSupplierID];
                    order.PayExchangeSuppliers.Add(new Needs.Ccs.Services.Models.OrderPayExchangeSupplier
                    {
                        OrderID = order.ID,
                        ClientSupplier = payExchangeSupplier.ToCssClientSupplier()
                    });
                }
                if (model.PIFiles != null)
                {
                    //原始发票
                    foreach (var invoice in model.PIFiles)
                    {
                        order.MainOrderFiles.Add(new Needs.Ccs.Services.Models.MainOrderFile
                        {
                            MainOrderID = order.MainOrderID,
                            User = user.ToCssUser(),
                            Name = invoice.name,
                            FileType = FileType.OriginalInvoice,
                            FileFormat = invoice.fileFormat,
                            Url = invoice.URL
                        });
                    }
                }
                order.ClassifyProducts = new List<ClassifyProduct>();
                //订单项
                foreach (var pageProduct in productList)
                {
                    decimal qty = pageProduct.Quantity;
                    decimal unitPrice = (pageProduct.TotalPrice / qty).ToRound(4);
                    decimal? grossWeight = null;
                    if (pageProduct.GrossWeight != null && pageProduct.GrossWeight.ToString() != "")
                    {
                        grossWeight = (decimal)pageProduct.GrossWeight;
                    }

                    if (model.IsClssified)  //已归类产品
                    {
                        //产品分类
                        Needs.Ccs.Services.Models.OrderItemCategory orderItemCategory = new Needs.Ccs.Services.Models.OrderItemCategory();
                        orderItemCategory.CIQCode = pageProduct.CIQCode;
                        orderItemCategory.Elements = pageProduct.Elements;
                        orderItemCategory.HSCode = pageProduct.HSCode;
                        orderItemCategory.Name = pageProduct.Name;
                        orderItemCategory.TaxCode = pageProduct.TaxCode;
                        orderItemCategory.TaxName = pageProduct.TaxName;
                        orderItemCategory.Type = pageProduct.Type.Value;
                        orderItemCategory.Unit1 = pageProduct.Unit1;
                        orderItemCategory.Unit2 = pageProduct.Unit2;
                        //orderItemCategory.Declarant = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(pageProduct.Declarant);
                        orderItemCategory.ClassifyFirstOperator = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(pageProduct.ClassifyFirstOperatorID);
                        orderItemCategory.ClassifySecondOperator = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(pageProduct.ClassifySecondOperatorID);
                        //关税
                        OrderItemTax ImportTax = new OrderItemTax();
                        ImportTax.Rate = pageProduct.TariffRate.Value;
                        ImportTax.ReceiptRate = pageProduct.TariffRate.Value;
                        ImportTax.Type = CustomsRateType.ImportTax;

                        //增值税率
                        OrderItemTax AddedValueTax = new OrderItemTax();
                        AddedValueTax.Rate = pageProduct.AddedValueRate.Value;
                        AddedValueTax.ReceiptRate = pageProduct.AddedValueRate.Value;
                        AddedValueTax.Type = CustomsRateType.AddedValueTax;

                        //消费税率
                        OrderItemTax ExciseTax = new OrderItemTax();
                        ExciseTax.Rate = pageProduct.ExciseTaxRate ?? 0m;
                        ExciseTax.ReceiptRate = pageProduct.ExciseTaxRate?? 0m;
                        ExciseTax.Type = CustomsRateType.ConsumeTax;

                        var productControls = new ProductControlsView(pageProduct.Model).ToList();
                        foreach (var control in productControls)
                        {
                            if (control.Type == ProductControlType.CCC)
                            {
                                pageProduct.IsSysCCC = true;
                            }
                            if (control.Type == ProductControlType.Forbid)
                            {
                                pageProduct.IsSysForbid = true;
                                orderItemCategory.Type |= ItemCategoryType.Forbid;
                            }
                        }

                        //如果来自疫区，则为归类类型添加“检疫”
                        string origin = pageProduct.Origin;
                        if (Needs.Wl.User.Plat.UserPlat.CustomsQuarantines.IsQuarantine(origin))
                        {
                            orderItemCategory.Type |= ItemCategoryType.Quarantine;
                        }

                        order.ClassifyProducts.Add(new ClassifyProduct
                        {
                            OrderID = order.ID,
                            //Product = product,
                            Client = client.ToCssClinet(),
                            Name = pageProduct.Name.Trim(),
                            Manufacturer = pageProduct.Manufacturer.Trim(),
                            Model = pageProduct.Model,
                            Batch = pageProduct.Batch,
                            Origin = pageProduct.Origin,
                            Quantity = qty,
                            Unit = pageProduct.Unit,
                            UnitPrice = unitPrice,
                            Currency = currency,
                            TotalPrice = pageProduct.TotalPrice,
                            GrossWeight = grossWeight,
                            ClassifyStatus = ClassifyStatus.Done,
                            Category = orderItemCategory,
                            IsSysForbid = pageProduct.IsSysForbid,
                            IsSysCCC = pageProduct.IsSysCCC,
                            IsCCC = (orderItemCategory.Type & ItemCategoryType.CCC) > 0,
                            IsOriginProof = (orderItemCategory.Type & ItemCategoryType.OriginProof) > 0,
                            IsInsp = (orderItemCategory.Type & ItemCategoryType.Inspection) > 0,
                            InspectionFee = pageProduct.InspectionFee,
                            ImportTax = ImportTax,
                            AddedValueTax = AddedValueTax,
                            ExciseTax = ExciseTax,
                            ProductUniqueCode = pageProduct.ProductUnionCode,
                        });
                    }
                    else
                    {
                        order.ClassifyProducts.Add(new ClassifyProduct
                        {
                            OrderID = order.ID,
                            OrderType = OrderType.Outside,
                            Client = client.ToCssClinet(),
                            Name = pageProduct.Name.Trim(),
                            Manufacturer = pageProduct.Manufacturer.Trim(),
                            Model = pageProduct.Model,
                            Batch = pageProduct.Batch,
                            Origin = pageProduct.Origin,
                            Quantity = qty,
                            Unit = pageProduct.Unit,
                            UnitPrice = unitPrice,
                            Currency = currency,
                            TotalPrice = pageProduct.TotalPrice,
                            GrossWeight = grossWeight,
                            ProductUniqueCode = pageProduct.ProductUnionCode,
                        });
                    }
                }

                //验证ProductUniqueCode 是否重复
                List<string> ProductUnionCodes = order.ClassifyProducts.Where(t => !string.IsNullOrEmpty(t.ProductUniqueCode) && t.ProductUniqueCode != null).Select(t => t.ProductUniqueCode).ToList();
                var OrderItemView = new Needs.Ccs.Services.Views.Origins.OrderItemsOrigin();

                var count = OrderItemView.Where(t => ProductUnionCodes.Contains(t.ProductUniqueCode)).Count();

                if (count > 0)
                {                  
                    return base.JsonResult(VueMsgType.error, "保存失败：物料号不能重复!");
                }

                order.DeclarePrice = order.ClassifyProducts.Select(item => item.TotalPrice).Sum();
                //正常下单
                if (order.OrderStatus == OrderStatus.Draft)
                {
                    if (model.isComfirmed)
                    {
                        order.OrderStatus = OrderStatus.Confirmed;
                    }
                    if (model.IsClssified)  //已归类
                    {
                        order.OrderStatus = OrderStatus.Classified;
                    }
                    order.Enter();
                }

                //已退回订单重新下单
                else if (order.OrderStatus == OrderStatus.Returned)
                {
                    if (model.isComfirmed)
                    {
                        order.OrderStatus = OrderStatus.Confirmed;
                    }
                    else
                    {
                        order.OrderStatus = OrderStatus.Draft;
                    }
                    order.ReEnter();
                }
                JsonResult json = new JsonResult
                {
                    Data = new
                    {
                        orderid = order.ID,
                        clientcode = client.ClientCode
                    }
                };
                return base.JsonResult(VueMsgType.success, "新增成功", json.Json());
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "保存失败：" + ex.Message);
            }
        }

        /// <summary>
        /// 订单编辑
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Edit(string id, string v)
        {
            id = id.InputText();
            v = v.InputText();
            ViewBag.navid = v;
            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[id];
            AddViewModel model = new AddViewModel();
            if (v == "DraftOrders" || v == "MyOrders")
            {
                model.IsDraft = true;
                if (order == null)
                {
                    return View("Error");
                }
                else if (order.OrderStatus != OrderStatus.Draft)
                {
                    return View("Error");
                }
            }
            else if (v == "RejectedOrders")
            {
                if (order == null)
                {
                    return View("Error");
                }
                else if (order.OrderStatus != OrderStatus.Returned)
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }

            model.ID = id;
            model.Currency = order.Currency;

            var viewCountries = Needs.Wl.User.Plat.UserPlat.Countries;
            viewCountries.AllowPaging = false;
            var listCountries = viewCountries.ToList();

            var viewUnits = Needs.Wl.User.Plat.UserPlat.Units;
            viewUnits.AllowPaging = false;
            var listUnits = viewUnits.ToList();

            //TODO:Items优化查询 Unit UnitCode
            var orderlist = order.Items.ToList().Select(item => new OrderProductModel
            {
                Batch = item.Batch,
                Name = item.Category?.Name ?? item.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Origin = item.Origin,
                OriginLabel = listCountries.Where(c => c.Code == item.Origin).FirstOrDefault() == null ? "" : listCountries.Where(c => c.Code == item.Origin).FirstOrDefault().Name,
                Quantity = item.Quantity,
                Unit = item.Unit,
                UnitLabel = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Code + " " + listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                GrossWeight = item.GrossWeight,
            }).ToArray();
            //产品
            model.OrderProducts = orderlist;
            //数据绑定

            //币种
            var viewCurrencies = Needs.Wl.User.Plat.UserPlat.Currencies;
            viewCurrencies.AllowPaging = false;
            model.CurrencyOptions = viewCurrencies.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //国家地区
            model.OriginOptions = listCountries.Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //单位
            model.UnitOptions = listUnits.Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            //包装
            var viewPackType = Needs.Wl.User.Plat.UserPlat.PackTypes;
            viewPackType.AllowPaging = false;
            model.WrapOptions = viewPackType.ToArray().Select(item => new
            {
                value = item.Code,
                text = item.Code + " " + item.Name
            }).Json();

            model.HKDeliveryTypeOptions = EnumUtils.ToDictionary<HKDeliveryType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            model.SZDeliveryTypeOptions = EnumUtils.ToDictionary<SZDeliveryType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //证件类型
            model.IdTypeOptions = EnumUtils.ToDictionary<IDType>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //供应商
            var viewSuppliers = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            viewSuppliers.AllowPaging = false;
            model.SupplierOptions = viewSuppliers.ToArray().Select(item => new
            {
                value = item.ID,
                text = item.ChineseName
            }).Json();

            model.PayExchangeSupplierOptions = model.SupplierOptions;

            //香港交货方式

            var OrderConsignee = order.OrderConsignee; //香港交货
            model.HKDeliveryType = ((int)OrderConsignee.Type).ToString();
            model.Supplier = OrderConsignee.ClientSupplier.ID; //供应商ID
            var supplier = Needs.Wl.User.Plat.UserPlat.Current.Client.Suppliers()[model.Supplier];  //供应商
            if (supplier == null)
            {
                var su = Needs.Wl.User.Plat.UserPlat.Current.Client.Suppliers().FirstOrDefault();
                if (su != null)
                {
                    model.Supplier = su.ID; //默认ID
                    var address = su.Addresses();
                    model.SupplierAddressOptions = address.ToArray().Select(item => new
                    {
                        value = item.ID,
                        text = "联系人：" + item.Contact.Name + "  电话：" + item.Contact.Mobile + "  地址：" + item.Address,
                        address = item.Address,
                        contact = item.Contact.Name,
                        mobile = item.Contact.Mobile,
                        isDefault = item.IsDefault
                    }).Json();

                    var SupplierAddress = address.GetDefault();
                    if (SupplierAddress != null)
                    {
                        model.SupplierAddress = SupplierAddress.ID;
                        model.SupplierAddressName = order.OrderConsignee.Address;
                        model.supplierContact = order.OrderConsignee.Contact;
                        model.supplierContactMobile = order.OrderConsignee.Mobile;
                    }
                }
            }
            else
            {
                var addressView = supplier.Addresses();
                addressView.Predicate = item => item.Address == OrderConsignee.Address;

                //TODO:代码需要优化
                var oldaddress = addressView.FirstOrDefault();
                if (oldaddress == null)
                {
                    var address = supplier.Addresses();
                    model.SupplierAddressOptions = address.ToArray().Select(item => new
                    {
                        value = item.ID,
                        text = "联系人：" + item.Contact.Name + "  电话：" + item.Contact.Mobile + "  地址：" + item.Address,
                        address = item.Address,
                        contact = item.Contact.Name,
                        mobile = item.Contact.Mobile,
                        isDefault = item.IsDefault
                    }).Json();

                    //供应商的默认提货地址
                    var SupplierAddress = address.GetDefault();
                    if (SupplierAddress != null)
                    {
                        model.SupplierAddress = SupplierAddress.ID;
                        model.SupplierAddressName = SupplierAddress.Address;
                        model.supplierContact = SupplierAddress.Contact.Name;
                        model.supplierContactMobile = SupplierAddress.Contact.Mobile;
                    }
                }
                else
                {
                    var address = supplier.Addresses();
                    model.SupplierAddressOptions = address.ToArray().Select(item => new
                    {
                        value = item.ID,
                        text = "联系人：" + item.Contact.Name + "  电话：" + item.Contact.Mobile + "  地址：" + item.Address,
                        address = item.Address,
                        contact = item.Contact.Name,
                        mobile = item.Contact.Mobile,
                        isDefault = item.IsDefault
                    }).Json();

                    model.SupplierAddress = oldaddress.ID;
                    model.SupplierAddressName = oldaddress.Address;
                    model.supplierContact = oldaddress.Contact.Name;
                    model.supplierContactMobile = oldaddress.Contact.Mobile;
                }
            }

            //送货
            model.HKFiles = new FileModel[0];
            if (OrderConsignee.Type == HKDeliveryType.SentToHKWarehouse)
            {
                model.WayBillNo = OrderConsignee.WayBillNo; //物流单号
            }
            {
                model.PickupTime = OrderConsignee.PickUpTime;
                //提货文件
                model.HKFiles = order.MainOrderFiles.Where(item => item.FileType == FileType.DeliveryFiles).Select(item => new FileModel
                {
                    name = item.Name,
                    fileFormat = item.FileFormat,
                    URL = item.Url
                }).ToArray();
            }

            //国内交货方式

            var consignor = order.OrderConsignor; //国内交货
            model.SZDeliveryType = ((int)consignor.Type).ToString();
            if (consignor.Type == SZDeliveryType.PickUpInStore)
            {
                model.ClientPicker = consignor.Contact;
                model.ClientPickerMobile = consignor.Mobile;
                model.IDType = consignor.IDType;
                model.IDNumber = consignor.IDNumber;
            }

            var viewConsignees = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees;
            viewConsignees.AllowPaging = false;
            var consigneeList = viewConsignees.ToList();

            model.ClientConsigneeOptions = consigneeList.Select(item => new
            {
                value = item.ID,
                text = item.Name,
                name = item.Contact.Name,
                mobile = item.Contact.Mobile,
                address = item.Address
            }).Json();

            var clientConsignee = consigneeList.Where(item => item.Name == consignor.Name).FirstOrDefault();
            if (clientConsignee == null)
            {

                var defaultconsignee = consigneeList.Where(item => item.IsDefault == true).FirstOrDefault();
                if (defaultconsignee != null)
                {
                    model.ClientConsignee = defaultconsignee.ID;  //默认收货地址
                    model.clientContact = defaultconsignee.Contact.Name;
                    model.ClientConsigneeName = defaultconsignee.Name;
                    model.clientConsigneeAddress = defaultconsignee.Address;
                    model.clientContactMobile = defaultconsignee.Contact.Mobile;
                }
            }
            else
            {
                model.ClientConsignee = clientConsignee.ID;
                model.clientContact = clientConsignee.Contact.Name;
                model.ClientConsigneeName = clientConsignee.Name;
                model.clientConsigneeAddress = clientConsignee.Address;
                model.clientContactMobile = clientConsignee.Contact.Mobile;
            }

            //协议里是否预付款
            model.IsPrePaid = Needs.Wl.User.Plat.UserPlat.Current.Client.Agreement().ProductFeeClause.PeriodType == Needs.Wl.Models.Enums.PeriodType.PrePaid;

            //付汇供应商
            var paySuppliers = order.PayExchangeSuppliers.Select(item => item.ClientSupplier.ID).ToList();
            var joinpaySuppliers = viewSuppliers.ToList().Select(item => item.ID).ToList().Intersect(paySuppliers).ToArray();

            model.PayExchangeSupplier = new string[0];  //付汇供应商实例化
            if (paySuppliers.Count() > 0)
            {
                model.PayExchangeSupplier = joinpaySuppliers;
            }
            model.PIFiles = order.MainOrderFiles.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new FileModel
            {
                name = item.Name,
                fileFormat = item.FileFormat,
                URL = item.Url
            }).ToArray();

            //其他信息
            model.IsFullVehicle = order.IsFullVehicle;
            if (!model.IsPrePaid)
            {
                model.IsLoan = order.IsLoan;
            }
            model.WrapType = order.WarpType;
            model.PackNo = order.PackNo.ToString();
            model.Summary = order.Summary;
            return View(model);
        }

        /// <summary>
        /// 获取供应商收货地址
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetSupplierAddress(string supplier)
        {
            var address = "";
            string supplierID = supplier.InputText();
            var entity = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers[supplierID];
            if (entity == null)
            {
                //TODO:返回到供应商列表页面；尝试攻击或在Url中输入错误的ID
                return base.JsonResult(VueMsgType.error, "供应商不存在！");
            }
            else
            {
                address = entity.Addresses().ToArray().Select(item => new
                {
                    value = item.ID,
                    text = "联系人:" + item.Contact.Name + " 电话:" + item.Contact.Mobile + " 地址:" + item.Address,
                    address = item.Address,
                    contact = item.Contact.Name,
                    mobile = item.Contact.Mobile,
                    isDefault = item.IsDefault
                }).Json();
            }

            return base.JsonResult(VueMsgType.success, "", address);
        }

        /// <summary>
        /// 获取收货地址
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetConsigneeList()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyConsignees;
            view.AllowPaging = false;
            var consignee = view.ToList().Select(item => new
            {
                value = item.ID,
                text = item.Name,
                name = item.Contact.Name,
                mobile = item.Contact.Mobile,
                address = item.Address
            }).Json();

            return base.JsonResult(VueMsgType.success, "", consignee);
        }

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetSuppliersList()
        {
            var supplier = Needs.Wl.User.Plat.UserPlat.Current.MySuppliers;
            supplier.AllowPaging = false;
            var SupplierOptions = supplier.ToArray().Select(item => new
            {
                value = item.ID,
                text = item.ChineseName
            }).Json();

            return base.JsonResult(VueMsgType.success, "", new { SupplierOptions }.Json());
        }

        #endregion

        #region 草稿订单
        /// <summary>
        /// 草稿订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DraftOrders()
        {
            return View();
        }

        /// <summary>
        /// 获取草稿订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetDraftOrders()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyDraftOrders;

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            view.PageSize = rows;
            view.PageIndex = page;

            int total = view.RecordCount;
            var list = await view.ToListAsync();
            var result = list.Select(item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.Supplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                OrderMaker = item.User == null ? "跟单员" : item.User.RealName,
                isLoading = true,
            });

            return JsonResult(VueMsgType.success, "", new { list = result.ToArray(), total }.Json());
        }

        /// <summary>
        /// POST:草稿订单删除
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DeleteOrder(string orderID)
        {
            orderID = orderID.InputText();
            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyDraftOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (order.Status == Status.Delete)
            {
                return base.JsonResult(VueMsgType.error, "该订单已删除，请勿重复操作");
            }
            order.SetUser(Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID].ToCssUser());
            order.Delete();
            return base.JsonResult(VueMsgType.success, "订单删除成功");
        }

        /// <summary>
        /// POST:草稿订单删除
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult DeleteOrder2(string orderID)
        {
            try
            {
                orderID = orderID.InputText();
                var order = Needs.Wl.User.Plat.UserPlat.Current.MyOrders[orderID];
                order.Deleted += Order_Deleted;
                order.Delete();
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败。");
            }

            return base.JsonResult(VueMsgType.success, "订单删除成功");
        }

        private void Order_Deleted(object sender, Needs.Wl.Models.Hanlders.OrderDeletedEventArgs e)
        {
            var current = Needs.Wl.User.Plat.UserPlat.Current;
            e.Order.Log(current.ID, "用户[" + current.RealName + "]删除了订单。");
        }

        #endregion

        #region 待客户确认订单

        /// <summary>
        /// GET: 等待确认的订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult PreConfirms()
        {
            return View();
        }

        /// <summary>
        /// 获取待确认的订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetPreConfirmsOrders()
        {

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var view = Needs.Wl.User.Plat.UserPlat.Current.WebSite.UnConfirmedOrdersView;
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;

            int recordCount = view.RecordCount;
            var orderlist = view.ToList();

            Func<Needs.Wl.User.Plat.Views.UnConfirmedOrdersViewModel, object> convert = item => new
            {
                ID = item.OrderID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.ClientSupplierChineseName,
                Contact = item.OrderConsignorContact,
                OrderMaker = item.OrderMaker,
                OrderStatus = item.OrderStatus.GetDescription(),
                isLoading = true,
                IsBecauseModified = item.IsBecauseModified,
            };

            return this.Paging1(orderlist, recordCount, convert);
        }

        /// <summary>
        /// 待确认订单取消
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="reason"></param>
        /// <param name="isBecauseModified">是否是因为修改（删除型号/修改数量）引发的确认</param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CancelConfirm(string orderID, string reason, bool isBecauseModified)
        {

            orderID = orderID.InputText();

            var predicate = PredicateBuilder.Create<Needs.Wl.User.Plat.Views.UnConfirmedOrdersViewModel>();
            predicate = predicate.And(item => item.OrderID == orderID);

            var view = Needs.Wl.User.Plat.UserPlat.Current.WebSite.UnConfirmedOrdersView;
            view.AllowPaging = false;
            view.Predicate = predicate;

            var theOrderInfo = view.FirstOrDefault();
            if (theOrderInfo == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (theOrderInfo.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Canceled)
            {
                return base.JsonResult(VueMsgType.error, "该订单已取消，请勿重复操作");
            }

            var user = Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID].ToCssUser();

            //如果是因为修改（删除型号/修改数量）引发的确认，需要将管控置为已处理（通过），并将订单取消挂起
            if (isBecauseModified)
            {
                ClientUnConfirmedControl clientUnConfirmedControl = new ClientUnConfirmedControl(orderID, theOrderInfo.OrderStatus.GetHashCode(), user);
                clientUnConfirmedControl.CancelHangUp();
            }

            var newQuotedOrder = new Needs.Ccs.Services.Models.QuotedOrder();
            newQuotedOrder.ID = orderID;
            newQuotedOrder.SetUser(user);
            newQuotedOrder.CanceledSummary = reason;
            newQuotedOrder.Cancel();
            return base.JsonResult(VueMsgType.success, "订单取消成功");



            /*
            orderID = orderID.InputText();
            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyQuotedOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (order.OrderStatus == OrderStatus.Canceled)
            {
                return base.JsonResult(VueMsgType.error, "该订单已取消，请勿重复操作");
            }
            order.SetUser(Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID].ToCssUser());
            order.CanceledSummary = reason;
            order.Cancel();
            return base.JsonResult(VueMsgType.success, "订单取消成功");
            */
        }

        /// <summary>
        /// POST:订单确认
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [HttpPost]
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult CheckPreConfirm(string orderID, string URL, string name, string fileFormat, bool isBecauseModified)
        {
            try
            {
                orderID = orderID.InputText();
                var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyQuotedOrdersView2[orderID];//订单明细
                var user = Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID];

                if (order.OrderStatus == OrderStatus.Quoted)
                {
                    order.SetUser(user.ToCssUser());
                    order.QuoteConfirm();
                }
                else if (isBecauseModified)
                {
                    order.SetUser(user.ToCssUser());
                    order.ModelModifiedConfirm();
                }
                else
                {
                    return View("Error");
                }

                //代理报关委托书保存
                if (!string.IsNullOrWhiteSpace(URL))
                {
                    var agentfile = order.MainOrderFiles.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
                    if (agentfile == null)
                    {
                        agentfile = new Needs.Ccs.Services.Models.MainOrderFile
                        {
                            MainOrderID = order.MainOrderID,
                            User = user.ToCssUser(),
                            Name = name,
                            FileType = FileType.AgentTrustInstrument,
                            FileFormat = fileFormat,
                            Url = URL,
                            FileStatus = OrderFileStatus.Auditing
                        };
                        agentfile.Enter();
                    }
                    else if (agentfile.FileStatus != OrderFileStatus.Audited)
                    {
                        agentfile.User = user.ToCssUser();
                        agentfile.Name = name;
                        agentfile.FileFormat = fileFormat;
                        agentfile.Url = URL;
                        agentfile.FileStatus = OrderFileStatus.Auditing;
                        agentfile.Enter();
                    }
                }

                return base.JsonResult(VueMsgType.success, "订单确认成功");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        ///// <summary>
        ///// 订单确认
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //[UserActionFilter(UserAuthorize = true)]
        //public ActionResult Confirm2(string id, bool v)
        //{
        //    id = id.InputText();
        //    var model = new ConfirmViewModel();

        //    model.IsBecauseModified = v;
        //    if (v) //是型号变更的订单确认，查询管控表中查询型号变更原因
        //    {
        //        var modelChangeOrderControls = Needs.Wl.User.Plat.UserPlat.Current.WebSite.OrderControlsViewForConfirm.Where(t => t.OrderID == id).ToList();
        //        if (modelChangeOrderControls != null && modelChangeOrderControls.Any())
        //        {
        //            model.ModelModifiedInfo = modelChangeOrderControls.Select(t => t.Summary).ToArray();
        //        }
        //    }

        //    model.AgentProxyFiles = new FileModel[0];
        //    var confirm = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyQuotedOrdersView2[id];
        //    if (confirm == null)
        //    {
        //        return View("Error");
        //    }

        //    var viewCountries = Needs.Wl.User.Plat.UserPlat.Countries;
        //    viewCountries.AllowPaging = false;
        //    var listCountries = viewCountries.ToList();

        //    var viewUnits = Needs.Wl.User.Plat.UserPlat.Units;
        //    viewUnits.AllowPaging = false;
        //    var listUnits = viewUnits.ToList();

        //    //税点
        //    var taxpoint = 1 + confirm.Client.Agreement.InvoiceTaxRate;
        //    //代理费率、最低代理费
        //    decimal agencyRate = confirm.AgencyFeeExchangeRate * confirm.Client.Agreement.AgencyRate;
        //    decimal minAgencyFee = confirm.Client.Agreement.MinAgencyFee;
        //    bool isAverage = confirm.DeclarePrice * agencyRate < minAgencyFee ? true : false;
        //    //平摊代理费、其他杂费
        //    decimal aveAgencyFee = confirm.AgencyFee * taxpoint / confirm.Items.Count();

        //    //产品明细
        //    //TODO:Items的查询优化：Origin、Unit 关税、增值税
        //    model.Products = confirm.Items.Select(item => new ComfirmProducts
        //    {
        //        Origin = listCountries.Where(c => c.Code == item.Origin).FirstOrDefault() == null ? "" : listCountries.Where(c => c.Code == item.Origin).FirstOrDefault().Name,
        //        Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
        //        Batch = item.Batch,
        //        GrossWeight = item.GrossWeight,
        //        Name = item.Category?.Name ?? item.Name,
        //        Manufacturer = item.Manufacturer,
        //        Model = item.Model,
        //        Quantity = item.Quantity,
        //        UnitPrice = item.UnitPrice,
        //        TotalPrice = item.TotalPrice,
        //        DeclareValue = item.TotalPrice * confirm.RealExchangeRate.Value,
        //        TraiffRate = item.ImportTax.Rate,
        //        Traiff = item.ImportTax.Value.Value,
        //        AddTaxRate = item.AddedValueTax.Rate,
        //        AddTax = item.AddedValueTax.Value.Value,
        //        AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
        //        InspectionFee = item.InspectionFee.GetValueOrDefault() * taxpoint,
        //        TotalTaxFee = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint,
        //        TotalDeclareValue = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
        //        TotalTaxFee_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint,
        //        TotalDeclareValue_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
        //    }).ToArray();

        //    //产品合计
        //    var SumTraiff = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
        //    var SumAddValue = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
        //    model.Products_Num = model.Products.Sum(item => item.Quantity.ToRound(2));
        //    model.Products_DeclareValue = model.Products.Sum(item => item.DeclareValue).ToString("0.00");
        //    model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
        //    model.Products_Traiff = SumTraiff.ToString("0.00");
        //    model.Products_AddTax = SumAddValue.ToString("0.00");
        //    model.Products_AgencyFee = model.Products.Sum(item => item.AgencyFee).ToString("0.00");
        //    model.Products_InspectionFee = model.Products.Sum(item => item.InspectionFee).ToString("0.00");
        //    model.Products_TotalTaxFee = (model.Products.Sum(item => item.TotalTaxFee_Except_TraAndAdd) + SumAddValue + SumTraiff).ToString("0.00");
        //    model.Products_TotalDeclareValue = (model.Products.Sum(item => item.TotalDeclareValue_Except_TraAndAdd) + SumAddValue + SumTraiff).ToString("0.00");

        //    //订单信息
        //    model.ID = confirm.ID;
        //    var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(confirm.Currency);

        //    model.Currency = currency == null ? "" : currency.Name;
        //    model.CurrencyCode = confirm.Currency;
        //    model.IsFullVehicle = confirm.IsFullVehicle ? "是" : "否";
        //    model.IsAdvanceMoneny = confirm.IsLoan ? "是" : "否";

        //    var pack = Needs.Wl.User.Plat.UserPlat.PackTypes.FindByCode(confirm.WarpType);
        //    if (pack != null)
        //    {
        //        model.WrapType = pack.Name;
        //    }

        //    model.PackNo = confirm.PackNo.ToString();
        //    model.Summary = confirm.Summary;
        //    model.CreateDate = confirm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
        //    model.OrderMaker = confirm.OrderMaker;

        //    var deliveryFile = confirm.Files.Where(item => item.FileType == FileType.DeliveryFiles).FirstOrDefault();
        //    if (deliveryFile != null)
        //    {
        //        model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
        //    }

        //    //香港交货方式信息
        //    var OrderConsignee = confirm.OrderConsignee;
        //    model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
        //    model.HKDeliveryType = OrderConsignee.Type.GetDescription();
        //    model.supplierContact = OrderConsignee.Contact;
        //    model.supplierContactMobile = OrderConsignee.Mobile;
        //    model.SupplierAddress = OrderConsignee.Address;
        //    model.WayBillNo = OrderConsignee.WayBillNo;
        //    if (OrderConsignee.Type == HKDeliveryType.PickUp)
        //    {
        //        model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
        //        model.isPickUp = true;
        //    }

        //    //国内交货信息
        //    var OrderConsignor = confirm.OrderConsignor;
        //    model.SZDeliveryType = OrderConsignor.Type.GetDescription();
        //    model.clientContact = OrderConsignor.Contact;
        //    model.clientContactMobile = OrderConsignor.Mobile;
        //    model.clientConsigneeAddress = OrderConsignor.Address;
        //    model.isSZPickUp = OrderConsignor.Type == SZDeliveryType.PickUpInStore;
        //    model.IDNumber = OrderConsignor.IDNumber;

        //    //发票信息
        //    model.invoice = new WebMvc.Models.InvoiceModel();
        //    var invoice = confirm.Client.Invoice;  //发票对象
        //    model.invoice.invoiceType = confirm.Client.Agreement.InvoiceType.GetDescription();
        //    model.invoice.invoiceDeliveryType = invoice.DeliveryType.GetDescription();
        //    model.invoice.invoiceTitle = invoice.Title;
        //    model.invoice.invoiceTel = invoice.Tel;
        //    model.invoice.invoiceTaxCode = invoice.TaxCode;
        //    model.invoice.invoiceAddress = invoice.Address;
        //    model.invoice.invoiceAccount = invoice.BankAccount;
        //    model.invoice.invoiceBank = confirm.Client.Invoice.BankName;
        //    model.invoice.contactName = confirm.Client.InvoiceConsignee.Name;
        //    model.invoice.contactMobile = confirm.Client.InvoiceConsignee.Mobile;
        //    model.invoice.contactTel = confirm.Client.InvoiceConsignee.Tel;
        //    model.invoice.contactAddress = confirm.Client.InvoiceConsignee.Address;

        //    //付汇供应商信息
        //    model.PayExchangeSupplier = confirm.PayExchangeSuppliers.Select(item => new
        //    {
        //        Name = item.ClientSupplier.ChineseName,
        //    }).ToArray();
        //    model.PIFiles = confirm.Files.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new
        //    {
        //        Name = item.Name,
        //        Status = item.Status.GetDescription(),
        //        Url = AppConfig.Current.FileServerUrl + @"/" + item.Url.ToUrl()
        //    }).ToArray();
        //    var AgentProxyURL = confirm.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
        //    if (AgentProxyURL != null)
        //    {
        //        //报关委托书
        //        model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + AgentProxyURL.Url.ToUrl();
        //        model.AgentProxyName = AgentProxyURL.Name;
        //        model.AgentProxyStatus = AgentProxyURL.FileStatus == OrderFileStatus.Audited;
        //    }
        //    return View(model);
        //}

        /// <summary>
        /// 订单确认
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Confirm(string id, bool v)
        {
            id = id.InputText();
            var model = new ConfirmViewModel();

            var current = Needs.Wl.User.Plat.UserPlat.Current;
            var order = current.MyOrders[id];

            //验证 Url是否非法输入
            if (order == null || order.ClientID != current.ClientID)
            {
                return View("Error");
            }

            model.ID = order.ID;
            model.IsBecauseModified = v;

            if (v) //是型号变更的订单确认，查询管控表中查询型号变更原因
            {
                var modelChangeOrderControls = order.OrderControls().GetUserConfirm();
                if (modelChangeOrderControls != null && modelChangeOrderControls.Any())
                {
                    model.ModelModifiedInfo = modelChangeOrderControls.Select(t => t.Summary).ToArray();
                }
            }

            model.AgentProxyFiles = new FileModel[0];

            var listCountries = Needs.Wl.User.Plat.UserPlat.Countries.ToList();
            var listUnits = Needs.Wl.User.Plat.UserPlat.Units.ToList();
            var agreement = order.Agreement();//订单的补充协议
            var files = order.Files().ToList();//订单的附件
            var orderItems = order.CategoriedItems().ToArray();//产品明细
            int orderItemCount = orderItems.Count();//订单产品项数量

            //增值税税率
            var taxPoint = 1 + agreement.InvoiceTaxRate;
            //代理费率、最低代理费
            decimal agencyRate = order.AgencyFeeExchangeRate(agreement) * agreement.AgencyRate;
            decimal minAgencyFee = agreement.MinAgencyFee;
            bool isAverage = order.DeclarePrice * agencyRate < minAgencyFee ? true : false;

            var premiums = order.Premiums().ToArray();//订单的费用
            decimal agencyFee = premiums.Where(s => s.Type == Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee).Select(f => f.UnitPrice * f.Count * f.Rate).FirstOrDefault();
            decimal otherFee = premiums.Where(item => item.Type != Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee && item.Type != Needs.Wl.Models.Enums.OrderPremiumType.InspectionFee)
               .Sum(item => item.Count * item.UnitPrice * item.Rate);

            //平摊代理费、其他杂费
            decimal aveAgencyFee = agencyFee * taxPoint / orderItemCount;
            decimal aveOtherFee = otherFee * taxPoint / orderItemCount;

            //产品明细
            model.Products = orderItems.Select(item => new ComfirmProducts
            {
                Origin = listCountries.Where(c => c.Code == item.Origin).FirstOrDefault()?.Name,
                Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault()?.Name,
                Batch = item.Batch,
                GrossWeight = item.GrossWeight,
                Name = string.IsNullOrEmpty(item.CategoriedName) ? item.Name : item.CategoriedName,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                ItemCategoryTypes = item.Type.GetFlagsDescriptions(",").Split(','),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                DeclareValue = item.TotalPrice * order.RealExchangeRate.Value,
                TraiffRate = item.ImportTaxRate,
                Traiff = item.ImportTaxValue,
                AddTaxRate = item.AddedValueRate,
                AddTax = item.AddedValue,
                AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint,
                InspectionFee = premiums.Where(s => s.Type == Needs.Wl.Models.Enums.OrderPremiumType.InspectionFee && s.OrderItemID == item.ID).Select(s => s.UnitPrice * s.Count * s.Rate).FirstOrDefault() * taxPoint,
                TotalTaxFee = item.TotalTaxAndAddedValue + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint,
                TotalTaxFee_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint,
                TotalDeclareValue = item.TotalTaxAndAddedValue + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint + item.TotalPrice * order.RealExchangeRate.Value,
                TotalDeclareValue_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint + item.TotalPrice * order.RealExchangeRate.Value,
            }).ToArray();

            //产品合计
            //ryan 20210113 外单税费小于50不收 钟苑平
            //var SumTraiff = (order.Type != Needs.Wl.Models.Enums.OrderType.Outside && model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
            //var SumAddValue = (order.Type != Needs.Wl.Models.Enums.OrderType.Outside && model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
            var SumTraiff = (model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
            var SumAddValue = (model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
            model.Products_Num = model.Products.Sum(item => item.Quantity.ToRound(2));
            model.Products_DeclareValue = model.Products.Sum(item => item.DeclareValue).ToString("0.00");
            model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
            model.Products_Traiff = SumTraiff.ToString("0.00");
            model.Products_AddTax = SumAddValue.ToString("0.00");
            model.Products_AgencyFee = model.Products.Sum(item => item.AgencyFee).ToString("0.00");
            model.Products_InspectionFee = model.Products.Sum(item => item.InspectionFee).ToString("0.00");
            model.Products_TotalTaxFee = (model.Products.Sum(item => item.TotalTaxFee_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
            model.Products_TotalDeclareValue = (model.Products.Sum(item => item.TotalDeclareValue_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");

            //订单信息
            var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(order.Currency);
            model.Currency = currency == null ? "" : currency.Name;
            model.CurrencyCode = order.Currency;
            model.IsFullVehicle = order.IsFullVehicle ? "是" : "否";
            model.IsAdvanceMoneny = order.IsLoan ? "是" : "否";

            var pack = Needs.Wl.User.Plat.UserPlat.PackTypes.FindByCode(order.WarpType);
            if (pack != null)
            {
                model.WrapType = pack.Name;
            }

            model.PackNo = order.PackNo.ToString();
            model.Summary = order.Summary;
            model.CreateDate = order.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.OrderMaker = order.User == null ? "跟单员" : order.User.RealName;



            //香港交货方式信息
            var OrderConsignee = order.Consignee();
            model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
            model.HKDeliveryType = OrderConsignee.Type.GetDescription();
            model.supplierContact = OrderConsignee.Contact;
            model.supplierContactMobile = OrderConsignee.Mobile;
            model.SupplierAddress = OrderConsignee.Address;
            model.WayBillNo = OrderConsignee.WayBillNo;
            if (OrderConsignee.Type == Needs.Wl.Models.Enums.HKDeliveryType.PickUp)
            {
                model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
                model.isPickUp = true;
            }

            //国内交货信息
            var orderConsignor = order.Consignor();
            model.SZDeliveryType = orderConsignor.Type.GetDescription();
            model.clientContact = orderConsignor.Contact;
            model.clientContactMobile = orderConsignor.Mobile;
            model.clientConsigneeAddress = orderConsignor.Address;
            model.isSZPickUp = orderConsignor.Type == Needs.Wl.Models.Enums.SZDeliveryType.PickUpInStore;
            model.IDNumber = orderConsignor.IDNumber;

            //发票信息
            model.invoice = new WebMvc.Models.InvoiceModel();
            var invoice = current.Client.Invoice();  //发票对象
            var invoiceConsignee = current.Client.InvoiceConsignee();  //发票收件人
            model.invoice = new InvoiceModel()
            {
                invoiceType = agreement.InvoiceType.GetDescription(),
                invoiceDeliveryType = invoice.DeliveryType.GetDescription(),
                invoiceTitle = invoice.Title,
                invoiceTaxCode = invoice.TaxCode,
                invoiceTel = invoice.Tel,
                invoiceAddress = invoice.Address,
                invoiceAccount = invoice.BankAccount,
                invoiceBank = invoice.BankName,
                contactName = invoiceConsignee.Name,
                contactMobile = invoiceConsignee.Mobile,
                contactTel = invoiceConsignee.Tel,
                contactAddress = invoiceConsignee.Address
            };

            //付汇供应商信息
            model.PayExchangeSupplier = order.PayExchangeSupplier().ToArray().Select(item => new
            {
                Name = item.ChineseName,
            }).ToArray();

            //TODO:临时用
            var orderforFile = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[id];
            var orderfiles = orderforFile.MainOrderFiles;

            var deliveryFile = orderfiles.Where(s => s.FileType == FileType.DeliveryFiles).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
            }

            //合同发票
            model.PIFiles = orderfiles.Where(s => s.FileType == FileType.OriginalInvoice).Select(item => new
            {
                Name = item.Name,
                Status = item.FileStatus.GetDescription(),
                Url = AppConfig.Current.FileServerUrl + @"/" + item.Url,
            }).ToArray();

            //报关委托书
            var file = orderfiles.Where(s => s.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
            if (file != null)
            {
                model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + file.Url;
                model.AgentProxyName = file.Name;
                model.AgentProxyStatus = file.FileStatus == OrderFileStatus.Audited;
            }

            return View(model);
        }

        #endregion

        #region 挂起订单
        /// <summary>
        /// 挂起订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult HangUpOrders()
        {
            return View();
        }

        #endregion

        #region 已退回订单
        /// <summary>
        /// 已退回订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult RejectedOrders()
        {
            return View();
        }

        /// <summary>
        /// 获取已退回订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetRejectedOrders()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyRejectedOrders;

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            view.PageIndex = page;
            view.PageSize = rows;
            var total = view.RecordCount;

            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.Supplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                Summary = item.ReturnedSummary,
                isLoading = true,
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 已退回订单取消
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult CancelRejected(string orderID, string reason)
        {
            orderID = orderID.InputText();
            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyReturnedOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            if (order.OrderStatus == OrderStatus.Canceled)
            {
                return base.JsonResult(VueMsgType.error, "该订单已取消，请勿重复操作");
            }
            order.SetUser(Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID].ToCssUser());
            order.CanceledSummary = reason;
            order.Cancel();
            return base.JsonResult(VueMsgType.success, "订单取消成功");
        }
        #endregion

        #region 已取消订单
        /// <summary>
        /// 已取消订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult CanceledOrders()
        {
            return View();
        }


        #endregion

        #region  待收货订单
        /// <summary>
        /// 已完成订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult UnReceivedOrders()
        {
            return View();
        }

        /// <summary>
        /// 获取待出库订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetUnReceievedOrders2()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyUnReceivedOrders;

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            view.PageIndex = page;
            view.PageSize = rows;

            var total = view.RecordCount;
            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                //ConsignorType = item.Consignor.Type.GetDescription(),
                //Name = item.Consignor.Name,
                //Contact = item.Consignor.Contact,
                OrderStatus = item.OrderStatus.GetDescription(),
                isLoading = true,
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 获取待出库订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetUnReceievedOrders()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyUnReceivedExitNoticeView;
            view.PageIndex = page;
            view.PageSize = rows;

            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.PageModels.AllUnReceivedExitNoticeViewModel>();

            view.PageIndex = page;
            view.PageSize = rows;

            view.Predicate = predicate;
            var total = view.RecordCount;

            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                OrderID = item.OrderID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Name = item.ReceiveCompanyName,
                Contact = item.ReceiverName,
                isLoading = true,
                MainOrderCreateDate = item.MainCreateDate.ToString("yyyy-MM-dd HH:mm"),
                ExitType = item.ExitType.GetDescription(),
                ExitTypeValue = (int)item.ExitType,
                ExitNoticeStatus = item.ExitNoticeStatus,
                IsComplete = item.ExitNoticeStatus == Needs.Wl.Models.Enums.ExitNoticeStatus.Completed ? true : false,
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());
        }

        /// <summary>
        /// 确认收货
        /// </summary>
        /// <param name="orderID"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ConfirmReceived2(string orderID)
        {
            orderID = orderID.InputText();
            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyWarehouseExitedOrdersView1[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }
            order.SetUser(Needs.Wl.User.Plat.UserPlat.Current.Client.Users()[Needs.Wl.User.Plat.UserPlat.Current.ID].ToCssUser());
            order.RecieveConfirm();
            return base.JsonResult(VueMsgType.success, "订单确认收货成功");
        }

        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ConfirmReceived(string orderID, string exitNoticeID)
        {
            orderID = orderID.InputText();
            exitNoticeID = exitNoticeID.InputText();
            var current = Needs.Wl.User.Plat.UserPlat.Current;
            var order = current.MyMianOrders[orderID];
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "该订单不存在");
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //改这一单出库通知的状态
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNotices>(new { ExitNoticeStatus = (int)Needs.Ccs.Services.Enums.ExitNoticeStatus.Completed }, item => item.ID == exitNoticeID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>(new { ExitNoticeStatus = (int)Needs.Ccs.Services.Enums.ExitNoticeStatus.Completed }, item => item.ExitNoticeID == exitNoticeID);

                //查询主订单的所有型号的数量
                var orderQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                on c.ID equals d.OrderID
                                where c.MainOrderId == orderID && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                select d.Quantity).Sum();

                //查询主订单下所有出库通知的数量
                var exitNoticeQty = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                                     join d in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                     on c.ID equals d.ExitNoticeID
                                     where c.OrderID == orderID && c.Status == (int)Status.Normal
                                     select d.Quantity).Sum();

                //如果两个数量相等，则继续，如果数量不等则只改出库通知的状态
                if (orderQty == exitNoticeQty)
                {
                    //判断所有的出库通知是否都已经变成已完成
                    var status = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNotices>()
                                  where c.OrderID == orderID && c.Status == (int)Status.Normal
                                  select new { ExitNoticeStatus = c.ExitNoticeStatus }).ToList();
                    bool isAny = status.Any(item => (item.ExitNoticeStatus != (int)ExitNoticeStatus.Completed));

                    if (!isAny)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                            new { OrderStatus = (int)Needs.Ccs.Services.Enums.OrderStatus.Completed },
                            item => item.MainOrderId == orderID && item.OrderStatus != (int)Needs.Wl.Models.Enums.OrderStatus.Canceled
                                    && item.OrderStatus != (int)Needs.Wl.Models.Enums.OrderStatus.Returned);

                        //写日志
                        var OrderIDs = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                                        where c.MainOrderId == orderID && c.OrderStatus != (int)OrderStatus.Canceled && c.OrderStatus != (int)OrderStatus.Returned
                                        select c.ID).ToList();

                        foreach (var orderid in OrderIDs)
                        {
                            string thisOrderID = orderid;

                            // OrderLogs 表插入日志
                            reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderLogs>(new Layer.Data.Sqls.ScCustoms.OrderLogs()
                            {
                                ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderLog),
                                OrderID = thisOrderID,
                                UserID = current.ID,
                                OrderStatus = (int)OrderStatus.Completed,
                                CreateDate = DateTime.Now,
                                Summary = "该订单已出库完成，已将订单状态置为【完成】",
                            });

                            int targetCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderTraces>()
                                .Where(t => t.OrderID == thisOrderID
                                         && t.Step == (int)OrderTraceStep.Completed).Count();
                            if (targetCount <= 0)
                            {
                                // OrderTraces 表中插入订单轨迹
                                reponsitory.Insert<Layer.Data.Sqls.ScCustoms.OrderTraces>(new Layer.Data.Sqls.ScCustoms.OrderTraces()
                                {
                                    ID = Needs.Overall.PKeySigner.Pick(Needs.Ccs.Services.PKeyType.OrderTrace),
                                    OrderID = thisOrderID,
                                    UserID = current.ID,
                                    Step = (int)OrderTraceStep.Completed,
                                    CreateDate = DateTime.Now,
                                    Summary = "您的订单已完成，感谢使用一站式报关服务，期待与您的下次合作",
                                });
                            }

                        }
                    }
                }
            }

            return base.JsonResult(VueMsgType.success, "确认收货成功");
        }
        #endregion

        #region  已完成订单
        /// <summary>
        /// 已完成订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult CompeletedOrders()
        {
            return View();
        }


        #endregion

        #region 待开票订单
        /// <summary>
        /// 待开票订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult UnInvoicedOrders()
        {
            return View();
        }

        /// <summary>
        /// 获取待开票订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetUnInvoicedOrders()
        {
            var view = Needs.Wl.User.Plat.UserPlat.Current.MyUnInvocieOrders;

            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;

            var total = view.RecordCount;
            var list = view.ToList().Select(item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                InvoiceType = item.InvoiceType.GetDescription(),
                InvoiceTaxRate = item.InvoiceTaxRate,
                OrderStatus = item.OrderStatus.GetDescription(),
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2).ToString(),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                isLoading = true,
                MainOrderID = item.MainOrderID,
                MainOrderCreateDate = item.MainOrderCreateDate.ToString("yyyy-MM-dd HH:mm")
            });

            return JsonResult(VueMsgType.success, "", new { list = list.ToArray(), total }.Json());

        }

        #endregion

        #region 对账单

        /// <summary>
        /// 对账单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        //public ActionResult OrderBills(string id, string v)
        //{
        //    id = id.InputText();
        //    v = v.InputText();
        //    var bill = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrderBills[id];

        //    if (bill == null || (v != "MyOrders"))
        //    {
        //        return View("Error");
        //    }
        //    else
        //    {
        //        var purchaser = PurchaserContext.Current;

        //        OrderBillsViewModel model = new OrderBillsViewModel();
        //        ViewBag.navid = v;
        //        //税点
        //        var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
        //        //代理费率、最低代理费
        //        decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
        //        decimal minAgencyFee = bill.Agreement.MinAgencyFee;
        //        bool isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
        //        //平摊代理费、其他杂费
        //        decimal aveAgencyFee = bill.AgencyFee * taxpoint / bill.Items.Count();
        //        decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();

        //        //基本信息
        //        var client = bill.Client;
        //        model.User_name = client.Company.Name;
        //        model.User_tel = client.Company.Contact.Tel;
        //        model.AgentName = purchaser.CompanyName;
        //        model.AgentAddress = purchaser.Address;
        //        model.AgentTel = purchaser.Tel;
        //        model.AgentFax = purchaser.UseOrgPersonTel;
        //        model.Account = purchaser.BankName;
        //        model.AccountID = purchaser.AccountId;
        //        model.RealExchangeRate = bill.RealExchangeRate;
        //        model.CustomsExchangeRate = bill.CustomsExchangeRate;
        //        model.DueDate = bill.GetDueDate().ToString("yyyy年MM月dd日");
        //        model.OrderID = bill.ID;
        //        model.CreateDate = bill.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
        //        model.IsLoan = bill.IsLoan;
        //        model.Currency = bill.Currency;
        //        model.ContractNO = bill.ContractNO;
        //        model.SealUrl = "../../" + PurchaserContext.Current.SealUrl.ToUrl();

        //        //产品明细
        //        var list = bill.Items.Select(item => new
        //        {
        //            Name = item.Category.Name,
        //            Model = item.Model,
        //            Quantity = item.Quantity,
        //            UnitPrice = item.UnitPrice,
        //            DeclarePrice = item.TotalPrice,
        //            TariffRate = item.ImportTax.Rate,
        //            TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
        //            Traiff = item.ImportTax.Value,
        //            AddedValueTax = item.AddedValueTax.Value,
        //            AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
        //            IncidentalFee = bill.InspFees.Where(fee => fee.OrderItemID == item.ID)
        //                                      .Select(fee => fee.Count * fee.UnitPrice * fee.Rate)
        //                                      .FirstOrDefault() * taxpoint + aveOtherFee
        //        }).ToArray();

        //        var SumTraiff = (bill.OrderType != OrderType.Outside && list.Sum(item => item.Traiff.Value) < 50) ? 0 : list.Sum(item => item.Traiff.Value);
        //        var SumAddValue = (bill.OrderType != OrderType.Outside && list.Sum(item => item.AddedValueTax.Value) < 50) ? 0 : list.Sum(item => item.AddedValueTax.Value);
        //        model.SumQuantity = list.Sum(item => item.Quantity).ToString("0");
        //        model.SumDeclarePrice = list.Sum(item => item.DeclarePrice).ToString("0.00");

        //        //历史数据显示方式变更
        //        if (bill.Order.CreateDate > Convert.ToDateTime("2019-05-30 23:59"))
        //        {
        //            //总和
        //            model.SumTotalCNYPrice = list.Sum(item => item.TotalCNYPrice).ToString("0.00");
        //            model.SumTraiff = SumTraiff.ToString("0.00");
        //            model.SumAddedValueTax = SumAddValue.ToString("0.00");
        //            model.SumAgencyFee = list.Sum(item => item.AgencyFee).ToString("0.00");
        //            model.SumIncidentalFee = list.Sum(item => item.IncidentalFee).ToString("0.00");
        //            model.SumTotalTax = (list.Sum(item => item.AgencyFee + item.IncidentalFee) + SumTraiff + SumAddValue).ToString("0.00");
        //            model.SumTotalDeclarePrice = (list.Sum(item => item.TotalCNYPrice + item.AgencyFee + item.IncidentalFee) + SumTraiff + SumAddValue).ToString("0.00");
        //            model.Productlist = list.Select(item => new
        //            {
        //                Name = item.Name,
        //                Model = item.Model,
        //                Quantity = item.Quantity,
        //                UnitPrice = item.UnitPrice,
        //                TariffRate = item.TariffRate,
        //                DeclarePrice = item.DeclarePrice.ToString("0.00"),
        //                TotalCNYPrice = item.TotalCNYPrice.ToString("0.00"),
        //                Traiff = item.Traiff.Value.ToString("0.00"),
        //                AddedValueTax = item.AddedValueTax.Value.ToString("0.00"),
        //                AgencyFee = item.AgencyFee.ToString("0.00"),
        //                IncidentalFee = item.IncidentalFee.ToString("0.00"),
        //                TotalTax = (item.Traiff.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToString("0.00"),
        //                TotalDeclarePrice = (item.TotalCNYPrice + item.Traiff.Value + item.AddedValueTax.Value + item.AgencyFee + item.IncidentalFee).ToString("0.00")
        //            }).ToArray();
        //        }
        //        else
        //        {
        //            //历史数据
        //            model.Productlist = list.Select(item => new
        //            {
        //                Name = item.Name,
        //                Model = item.Model,
        //                Quantity = item.Quantity,
        //                UnitPrice = item.UnitPrice,
        //                TariffRate = item.TariffRate,
        //                DeclarePrice = item.DeclarePrice.ToString("0.00"),
        //                TotalCNYPrice = item.TotalCNYPrice.ToString("0.00"),
        //                Traiff = item.Traiff.Value.ToString("0.00"),
        //                AddedValueTax = item.AddedValueTax.Value.ToString("0.00"),
        //                AgencyFee = item.AgencyFee.ToString("0.00"),
        //                IncidentalFee = item.IncidentalFee.ToString("0.00"),
        //                TotalTax = (Math.Round(item.Traiff.Value, 2) + Math.Round(item.AddedValueTax.Value, 2) + Math.Round(item.AgencyFee, 2) + Math.Round(item.IncidentalFee, 2)).ToString("0.00"),
        //                TotalDeclarePrice = (item.TotalCNYPrice + Math.Round(item.Traiff.Value, 2) + Math.Round(item.AddedValueTax.Value, 2) + Math.Round(item.AgencyFee, 2) + Math.Round(item.IncidentalFee, 2)).ToString("0.00")
        //            }).ToArray();

        //            var lists = list.Select(item => new
        //            {
        //                TotalCNYPrice = Math.Round(item.TotalCNYPrice, 2),
        //                Traiff = Math.Round(item.Traiff.Value, 2),
        //                AddedValueTax = Math.Round(item.AddedValueTax.Value, 2),
        //                AgencyFee = Math.Round(item.AgencyFee, 2),
        //                IncidentalFee = item.IncidentalFee,
        //            });

        //            var SumTotalCNYPriceValue = lists.Sum(item => item.TotalCNYPrice);
        //            var SumTraiffValue = lists.Sum(item => item.Traiff);
        //            var SumAddedValueTaxValue = lists.Sum(item => item.AddedValueTax);
        //            var SumAgencyFeeValue = lists.Sum(item => item.AgencyFee);
        //            var SumIncidentalFeeValue = lists.Sum(item => item.IncidentalFee);

        //            model.SumTotalCNYPrice = SumTotalCNYPriceValue.ToString("0.00");
        //            model.SumTraiff = SumTraiffValue.ToString("0.00");
        //            model.SumAddedValueTax = SumAddedValueTaxValue.ToString("0.00");
        //            model.SumAgencyFee = SumAgencyFeeValue.ToString("0.00");
        //            model.SumIncidentalFee = SumIncidentalFeeValue.ToString("0.00");
        //            model.SumTotalTax = (Math.Round(SumTraiffValue, 2) + Math.Round(SumAddedValueTaxValue, 2) + Math.Round(SumAgencyFeeValue, 2) + Math.Round(SumIncidentalFeeValue, 2)).ToString();
        //            model.SumTotalDeclarePrice = (Math.Round(SumTotalCNYPriceValue, 2) + Convert.ToDecimal(model.SumTotalTax)).ToString("0.00");

        //        }


        //        var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[id];
        //        if (order != null)
        //        {
        //            var file = order.Files.Where(item => item.FileType == FileType.OrderBill && item.Status == Status.Normal).FirstOrDefault();
        //            if (file != null)
        //            {
        //                model.BillFileUrl = AppConfig.Current.FileServerUrl + @"/" + file.Url;
        //                model.BillFileStatus = file.FileStatus == OrderFileStatus.Audited;
        //                model.BillFileName = file.Name;
        //            }
        //        }
        //        if (order.IsLoan)
        //        {

        //        }
        //        return View(model);
        //    }
        //}

        public ActionResult OrderBills(string id, string v)
        {
            id = id.InputText();
            v = v.InputText();
            var viewModel = new MainOrderBillViewModel();

            if (v != "MyOrders")
            {
                return View("Error");
            }
            else
            {
                viewModel = getModel(id);
            }

            return View(viewModel);
        }

        private MainOrderBillViewModel getModel(string id)
        {
            var OrderIDs = new Orders2View().Where(item => item.MainOrderID == id && item.OrderStatus >= Needs.Ccs.Services.Enums.OrderStatus.Quoted
                                                   && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Canceled && item.OrderStatus != Needs.Ccs.Services.Enums.OrderStatus.Returned)
                           .Select(item => item.ID).ToList();


            if (OrderIDs.Count == 0)
            {
                return null;
            }
            else
            {
                var purchaser = PurchaserContext.Current;
                var vendor = VendorContext.Current;


                MainOrderBillViewModel viewModel = new MainOrderBillViewModel();
                viewModel.MainOrderID = id;
                viewModel.OrderIDs = new List<string>();
                viewModel.OrderIDs = OrderIDs;
                //var MainBillItems = viewModel.Items;


                viewModel.AgentName = purchaser.CompanyName;
                viewModel.AgentAddress = purchaser.Address;
                viewModel.AgentTel = purchaser.Tel;
                viewModel.AgentFax = purchaser.UseOrgPersonTel;
                viewModel.Purchaser = purchaser.CompanyName;
                viewModel.Bank = purchaser.BankName;
                viewModel.Account = purchaser.AccountName;
                viewModel.AccountId = purchaser.AccountId;
                viewModel.SealUrl = PurchaserContext.Current.SealUrl.ToUrl();
                viewModel.Bills = new List<MainOrderBillItem>();


                foreach (var orderid in OrderIDs)
                {

                    var bill = new Needs.Ccs.Services.Views.OrderBillsView2(orderid).FirstOrDefault();
                    //var billItems = MainBillItems.Where(t => t.OrderID == orderid);


                    //TODO:优化ClientName，ClientTel,Currency,IsLoan,DueDate,CreateDate只要赋值一次
                    viewModel.ClientName = bill.Client.Company.Name;
                    viewModel.ClientTel = bill.Client.Company.Contact.Tel;
                    viewModel.Currency = bill.Currency;
                    viewModel.IsLoan = bill.IsLoan;
                    viewModel.DueDate = bill.GetDueDate().ToString("yyyy年MM月dd日");
                    viewModel.CreateDate = bill.CreateDate.ToString();
                    viewModel.ClientType = bill.Client.ClientType;

                    //如果订单尚未报价，订单缺少海关汇率、实时汇率，不能生成对账单
                    if (bill.CustomsExchangeRate == 0 || bill.RealExchangeRate == 0)
                    {
                        return null;
                    }
                    else
                    {
                        //税点
                        var taxpoint = 1 + bill.Agreement.InvoiceTaxRate;
                        //代理费率、最低代理费
                        decimal agencyRate = bill.AgencyFeeExchangeRate * bill.Agreement.AgencyRate;
                        bool isAverage = false;
                        decimal minAgencyFee = bill.Agreement.MinAgencyFee;
                        switch (bill.Order.OrderBillType)
                        {
                            case OrderBillType.Normal:
                                isAverage = bill.DeclarePrice * agencyRate < minAgencyFee ? true : false;
                                break;

                            case OrderBillType.MinAgencyFee:
                                isAverage = false;
                                break;

                            case OrderBillType.Pointed:
                                isAverage = true;
                                break;
                        }

                        //平摊代理费、其他杂费
                        decimal AgencyFee = bill.AgencyFee * taxpoint;
                        decimal aveAgencyFee = AgencyFee / bill.Items.Count();
                        decimal aveOtherFee = bill.OtherFee * taxpoint / bill.Items.Count();

                        MainOrderBillItem Item = new MainOrderBillItem();

                        Item.Products = new List<MainOrderBillItemProduct>();
                        Item.OrderID = bill.ID;
                        Item.ContrNo = bill.ContractNO ?? "";
                        Item.RealExchangeRate = bill.RealExchangeRate;
                        Item.CustomsExchangeRate = bill.CustomsExchangeRate;
                        Item.OrderType = bill.OrderType;
                        Item.AgencyFee = AgencyFee;

                        Item.Products = bill.Items.Select(item => new
                        MainOrderBillItemProduct
                        {
                            ProductName = item.Category.Name.Trim(),
                            Model = item.Model.Trim(),
                            Quantity = item.Quantity,
                            UnitPrice = item.UnitPrice,
                            TotalPrice = item.TotalPrice,
                            TariffRate = item.ImportTax.Rate,
                            TotalCNYPrice = (item.TotalPrice * bill.ProductFeeExchangeRate),
                            Traiff = item.ImportTax.Value,
                            AddedValueTax = item.AddedValueTax.Value,
                            AgencyFee = isAverage ? aveAgencyFee : (item.TotalPrice * agencyRate * taxpoint),
                            IncidentalFee = (item.InspectionFee.GetValueOrDefault() * taxpoint + aveOtherFee)
                        }).ToList();

                        Item.PartProducts = new List<MainOrderBillItemProduct>();
                        Item.PartProducts.Add(Item.Products.FirstOrDefault());

                        Item.totalQty = Item.Products.Sum(t => t.Quantity);
                        Item.totalPrice = Item.Products.Sum(t => t.TotalPrice);
                        Item.totalCNYPrice = Item.Products.Sum(t => t.TotalCNYPrice);
                        Item.totalTraiff = Item.Products.Sum(t => t.Traiff);
                        Item.totalAddedValueTax = Item.Products.Sum(t => t.AddedValueTax);
                        Item.totalAgencyFee = Item.Products.Sum(t => t.AgencyFee);
                        Item.totalIncidentalFee = Item.Products.Sum(t => t.IncidentalFee);

                        //ryan 20210113 外单税费小于50不收 钟苑平
                        //if (bill.OrderType != OrderType.Outside && Item.totalTraiff < 50)
                        if (Item.totalTraiff < 50)
                        {
                            Item.totalTraiff = 0;
                        }
                        if (Item.totalAddedValueTax < 50)
                        {
                            Item.totalAddedValueTax = 0;
                        }

                        viewModel.Bills.Add(Item);
                    }
                }

                var current = Needs.Wl.User.Plat.UserPlat.Current;
                var mainorder = current.MyMianOrders[id];

                var OrderBillFile = mainorder.Files().GetOrderBill();

                viewModel.FileID = OrderBillFile?.ID;
                viewModel.FileStatus = OrderBillFile == null ? OrderFileStatus.NotUpload.GetDescription() :
                                        OrderBillFile.FileStatus.GetDescription();
                viewModel.FileName = OrderBillFile == null ? "" : OrderBillFile.Name;
                viewModel.Url = OrderBillFile == null ? "" : OrderBillFile.Url;
                //viewModel.FileStatusValue = OrderBillFile == null ? Needs.Wl.Models.Enums.OrderFileStatus.NotUpload : OrderBillFile.FileStatus;
                viewModel.Url = AppConfig.Current.FileServerUrl + @"/" + OrderBillFile?.Url.ToUrl();
                viewModel.summaryTotalPrice = viewModel.Bills.Sum(t => t.totalPrice);
                viewModel.summaryTotalCNYPrice = viewModel.Bills.Sum(t => t.totalCNYPrice);
                viewModel.summaryPay = viewModel.Bills.Sum(t => t.totalTraiff) + viewModel.Bills.Sum(t => t.totalAddedValueTax) + viewModel.Bills.Sum(t => t.totalAgencyFee) + viewModel.Bills.Sum(t => t.totalIncidentalFee);
                viewModel.summaryPayAmount = viewModel.summaryTotalCNYPrice + viewModel.summaryPay;

                viewModel.summaryTotalTariff = viewModel.Bills.Sum(t => t.totalTraiff);
                viewModel.summaryTotalAddedValueTax = viewModel.Bills.Sum(t => t.totalAddedValueTax);
                viewModel.summaryTotalAgencyFee = viewModel.Bills.Sum(t => t.totalAgencyFee);
                viewModel.summaryTotalIncidentalFee = viewModel.Bills.Sum(t => t.totalIncidentalFee);
                viewModel.CreateDate = mainorder.CreateDate.ToString("yyyy-MM-dd HH:mm");

                return viewModel;
            }
        }

        /// <summary>
        /// 删除对账单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DelBillFile(string id)
        {
            try
            {
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                var order = current.MyMianOrders[id];

                var OrderBillFile = order.Files().GetOrderBill();


                if (order == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！删除失败！");
                }

                if (OrderBillFile != null)
                {
                    if (OrderBillFile.FileStatus == Needs.Wl.Models.Enums.OrderFileStatus.Audited)
                    {
                        return base.JsonResult(VueMsgType.error, "原对账单已审核通过！删除失败！");
                    }
                    OrderBillFile.Delete();
                }
                else
                {
                    return base.JsonResult(VueMsgType.error, "对账单不存在！删除失败！");
                }

                return base.JsonResult(VueMsgType.success, "删除成功！");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "删除失败！");
            }
        }

        /// <summary>
        /// 保存对账单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SaveBill(string id, string URL, string name, string fileFormat)
        {
            try
            {
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                var order = current.MyMianOrders[id];

                var OrderBillFile = order.Files().GetOrderBill();

                if (order == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！上传失败！");
                }

                if (OrderBillFile != null)
                {
                    if (OrderBillFile.FileStatus == Needs.Wl.Models.Enums.OrderFileStatus.Audited)
                    {
                        return base.JsonResult(VueMsgType.error, "原对账单已审核通过！上传失败！");
                    }

                }
                var auditingFile = new Needs.Wl.Models.MainOrderFile();
                if (OrderBillFile != null)
                {
                    if (OrderBillFile.FileStatus == Needs.Wl.Models.Enums.OrderFileStatus.Auditing)
                    {
                        auditingFile = OrderBillFile;
                    }
                }

                //用户ID
                var userID = current.ID;
                //客户
                var client = current.Client;
                var user = client.Users()[userID];
                auditingFile.OrderID = id;
                auditingFile.FileType = Needs.Wl.Models.Enums.FileType.OrderBill;
                auditingFile.FileFormat = fileFormat;
                auditingFile.CreateDate = DateTime.Now;
                auditingFile.FileStatus = Needs.Wl.Models.Enums.OrderFileStatus.Auditing;
                auditingFile.Name = name;
                auditingFile.Url = URL;
                auditingFile.User = user;
                auditingFile.Enter();
                return base.JsonResult(VueMsgType.success, "上传成功！");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "上传失败");
            }
        }

        /// <summary>
        /// 下载对账单
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetBillsPDF(string id)
        {
            try
            {
                id = id.InputText();
                var bill = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrderBills[id];
                if (bill == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！");
                }
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();
                bill.SaveAs(file.FilePath);
                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetBillsPDF2(string id)
        {
            try
            {
                id = id.InputText();
                var bill = getModel(id);
                if (bill == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！");
                }
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();
                bill.SaveAs(file.FilePath);
                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// Icgoo请求的，不分小订单的对账单
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public ActionResult OrderBillsForIcgoo(string id, string v)
        {
            id = id.InputText();
            v = v.InputText();
            var viewModel = new MainOrderBillViewModel();

            if (v != "MyOrders")
            {
                return View("Error");
            }
            else
            {

                viewModel = getModel(id);

                viewModel.ProductsForIcgoo = new List<MainOrderBillItemProduct>();
                viewModel.PartProductsForIcgoo = new List<MainOrderBillItemProduct>();
                foreach (var t in viewModel.Bills)
                {
                    viewModel.ProductsForIcgoo.AddRange(t.Products);
                    viewModel.PartProductsForIcgoo.AddRange(t.PartProducts);
                }


                return View(viewModel);
            }
        }

        public JsonResult GetIcgooBills(string id)
        {
            try
            {
                id = id.InputText();
                var viewModel = new MainOrderBillViewModel();
                viewModel = getModel(id);
                if (viewModel == null)
                {
                    return base.JsonResult(VueMsgType.error, "该订单不存在！");
                }

                viewModel.ProductsForIcgoo = new List<MainOrderBillItemProduct>();
                viewModel.PartProductsForIcgoo = new List<MainOrderBillItemProduct>();
                foreach (var t in viewModel.Bills)
                {
                    viewModel.ProductsForIcgoo.AddRange(t.Products);
                    viewModel.PartProductsForIcgoo.AddRange(t.PartProducts);
                }
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                //更换 itextsharp插件
                var orderbill = new OrderBillToPdf(viewModel);
                orderbill.SaveAs(file.FilePath);
                //viewModel.SaveASIcgoo(file.FilePath);

                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        #endregion

        #region 我的订单

        /// <summary>
        /// GET: 我的订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult MyOrders()
        {
            var model = new MyOrdersViewModel();

            //订单状态
            model.OrderStatusOptions = EnumUtils.ToDictionary<Needs.Wl.Models.Enums.OrderStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //开票状态
            model.InvoiceStatusOptions = EnumUtils.ToDictionary<Needs.Wl.Models.Enums.InvoiceStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            //付汇状态
            model.PayExchangeStatusOptions = EnumUtils.ToDictionary<Needs.Wl.Models.Enums.PayExchangeStatus>().Select(item => new
            {
                value = item.Key,
                text = item.Value
            }).Json();

            return View(model);
        }

        /// <summary>
        /// 获取我的订单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetMyOrderList2()
        {
            var orderStatus = Request.Form["orderStatus"];  //订单状态
            var invoiceStatus = Request.Form["invoiceStatus"];  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"];  //付汇状态
            var startDate = Request.Form["startDate"]; //订单日期
            var endDate = Request.Form["endDate"]; //订单日期
            var orderID = Request.Form["orderID"]; //订单编号

            var orders = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrdersExtends1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<UserOrderExtends, bool>> expression = item => true;

            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                var dStart = DateTime.Parse(startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);
                Expression<Func<UserOrderExtends, bool>> lambda = item => item.CreateDate >= dStart && item.CreateDate <= dEnd;
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(orderStatus))) //订单状态
            {
                if ((OrderStatus)int.Parse(orderStatus) == OrderStatus.Classified || (OrderStatus)int.Parse(orderStatus) == OrderStatus.Quoted || (OrderStatus)int.Parse(orderStatus) == OrderStatus.QuoteConfirmed)
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.OrderStatus == (OrderStatus)int.Parse(orderStatus) && (!item.IsHangUp);
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.OrderStatus == (OrderStatus)int.Parse(orderStatus);
                    lambdas.Add(lambda);
                }
            }
            if ((!string.IsNullOrWhiteSpace(invoiceStatus))) //开票状态
            {
                Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.InvoiceStatus == (InvoiceStatus)int.Parse(invoiceStatus);
                lambdas.Add(lambda);
            }
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus))) //付汇状态
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount == 0;
                    lambdas.Add(lambda);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0;
                    lambdas.Add(lambda);
                }

            }
            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.Trim();

                Expression<Func<Needs.Ccs.Services.Models.UserOrderExtends, bool>> lambda = item => item.ID.Contains(orderID);
                lambdas.Add(lambda);
            }
            #region 页面需要数据
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Func<Needs.Ccs.Services.Models.UserOrderExtends, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.OrderConsignee.ClientSupplier.ChineseName,
                OrderStatus = item.OrderStatus.GetDescription(),
                isDelete = item.OrderStatus == OrderStatus.Draft ? true : false,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2).ToString(),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                isShowBill = item.OrderStatus > OrderStatus.Quoted && item.OrderStatus <= OrderStatus.Completed,
                isLoading = true,
                isTihuo = item.IsShowTihou,
                specialTypeLabel = item.OrderVoyages == null ? "" : GetSpecialTypeLabel(item.OrderVoyages),
                isUnPay = item.PaidExchangeAmount < item.DeclarePrice && item.OrderStatus >= OrderStatus.QuoteConfirmed && item.OrderStatus <= OrderStatus.Completed,  //是否是待付汇
                IsPrePayExchange = item.ClientAgreement.IsPrePayExchange,
                isUnReceived = item.OrderStatus == OrderStatus.WarehouseExited, //是否待收货
                isDraft = item.OrderStatus == OrderStatus.Draft,
                isUnConfirm = item.OrderStatus == OrderStatus.Quoted && !item.IsHangUp, // 是否待确认
                IsBecauseModified = item.IsBecauseModified,
                MainOrderID = item.MainOrderID,
            };
            return this.Paging1(orderlist, orderlist.Total, convert);
            #endregion
        }

        /// <summary>
        /// 获取我的订单
        /// TODO:供应商使用付汇供应商
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<JsonResult> GetMyOrderList()
        {
            int page = int.Parse(Request.Form["page"].InputText());
            int rows = int.Parse(Request.Form["rows"].InputText());

            var orderID = Request.Form["orderID"].InputText(); //订单编号
            var orderStatus = Request.Form["orderStatus"].InputText();  //订单状态
            var invoiceStatus = Request.Form["invoiceStatus"].InputText();  //开票状态
            var payExchangeStatus = Request.Form["payExchangeStatus"].InputText();  //付汇状态
            var startDate = Request.Form["startDate"].InputText(); //订单日期
            var endDate = Request.Form["endDate"].InputText(); //订单日期

            var view = Needs.Wl.User.Plat.UserPlat.Current.MyAllOrders;
            view.PageIndex = page;
            view.PageSize = rows;

            var predicate = PredicateBuilder.Create<Needs.Wl.Client.Services.PageModels.AllOrderViewModel>();

            //订单编号
            if (!string.IsNullOrEmpty(orderID))
            {
                orderID = orderID.InputText().Trim();
                predicate = predicate.And(item => item.ID.Contains(orderID));
            }

            //下单时间
            if ((!string.IsNullOrWhiteSpace(startDate)) && (!string.IsNullOrWhiteSpace(endDate)))
            {
                var dStart = DateTime.Parse(startDate);
                dStart = new DateTime(dStart.Year, dStart.Month, dStart.Day);
                var dEnd = DateTime.Parse(endDate);
                dEnd = new DateTime(dEnd.Year, dEnd.Month, dEnd.Day, 23, 59, 59);

                predicate = predicate.And(item => item.CreateDate >= dStart && item.CreateDate <= dEnd);
            }

            //订单状态
            if ((!string.IsNullOrWhiteSpace(orderStatus)))
            {
                if ((OrderStatus)int.Parse(orderStatus) == OrderStatus.Classified || (OrderStatus)int.Parse(orderStatus) == OrderStatus.Quoted || (OrderStatus)int.Parse(orderStatus) == OrderStatus.QuoteConfirmed)
                {
                    predicate = predicate.And(item => item.OrderStatus == (Needs.Wl.Models.Enums.OrderStatus)int.Parse(orderStatus) && (!item.IsHangUp));
                }
                else
                {
                    predicate = predicate.And(item => item.OrderStatus == (Needs.Wl.Models.Enums.OrderStatus)int.Parse(orderStatus));
                }
            }

            //开票状态
            if ((!string.IsNullOrWhiteSpace(invoiceStatus)))
            {
                predicate = predicate.And(item => item.InvoiceStatus == (Needs.Wl.Models.Enums.InvoiceStatus)int.Parse(invoiceStatus));
            }

            //付汇状态
            if ((!string.IsNullOrWhiteSpace(payExchangeStatus)))
            {
                if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.UnPay)
                {
                    predicate = predicate.And(item => item.PaidExchangeAmount == 0);
                }
                else if ((PayExchangeStatus)int.Parse(payExchangeStatus) == PayExchangeStatus.Partial)
                {
                    predicate = predicate.And(item => item.PaidExchangeAmount < item.DeclarePrice && item.PaidExchangeAmount != 0);
                }
                else
                {
                    predicate = predicate.And(item => item.PaidExchangeAmount == item.DeclarePrice && item.PaidExchangeAmount != 0);
                }
            }

            view.Predicate = predicate;
            var total = view.RecordCount;
            var list = await view.ToListAsync();

            var orderlist = list.Select(item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Currency = item.Currency,
                DeclarePrice = item.DeclarePrice,
                Suppliers = item.PayExchangeSuppliers.Select(c => c.ChineseName).ToArray(),
                OrderStatus = item.OrderStatus.GetDescription(),
                isDelete = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Draft ? true : false,
                InvoiceStatus = item.InvoiceStatus.GetDescription(),
                PayExchangeStatus = item.PayExchangeStatus.GetDescription(),
                Remittance = (item.DeclarePrice - item.PaidExchangeAmount).ToRound(2).ToString(),
                Remittanced = item.PaidExchangeAmount.ToRound(2).ToString(),
                isShowBill = item.OrderStatus > Needs.Wl.Models.Enums.OrderStatus.Quoted && item.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Completed,
                isLoading = true,
                specialTypeLabel = item.OrderVoyages == null ? "" : GetSpecialTypeLabel(item.OrderVoyages.ToArray()),
                isUnPay = item.PaidExchangeAmount < item.DeclarePrice && item.OrderStatus >= Needs.Wl.Models.Enums.OrderStatus.QuoteConfirmed && item.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Completed,  //是否是待付汇
                IsPrePayExchange = item.IsPrePayExchange,
                isUnReceived = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.WarehouseExited, //是否待收货
                isDraft = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Draft,
                isUnConfirm = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Quoted && !item.IsHangUp, // 是否待确认
                IsBecauseModified = item.IsBecauseModified,
                MainOrderID = item.MainOrderID,
                IsReturned = item.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Returned ? true : false,
                MainOrderCreateDate = item.MainOrderCreateDate.ToString("yyyy-MM-dd HH:mm"),
            });

            return JsonResult(VueMsgType.success, "", new { list = orderlist.ToArray(), total }.Json());
        }

        /// <summary>
        /// 循环输出订单的特殊类型标签
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetSpecialTypeLabel(IEnumerable<Needs.Wl.Models.OrderVoyage> list)
        {
            string str = "";
            foreach (var item in list)
            {
                str += "<span class='spacialTip'>" + item.Type.GetDescription() + "</span>";
            }
            return str;
        }

        /// <summary>
        /// 循环输出订单的特殊类型标签
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private string GetSpecialTypeLabel(List<Needs.Ccs.Services.Models.OrderVoyage> list)
        {
            string str = "";
            foreach (var item in list)
            {
                str += "<span class='spacialTip'>" + item.Type.GetDescription() + "</span>";
            }
            return str;
        }

        /// <summary>
        /// 产品归类特殊类型的复合枚举
        /// </summary>
        /// <param name="itemCategoryType"></param>
        /// <returns></returns>
        private string GetCategoryDescriptions(ItemCategoryType? itemCategoryType)
        {
            string str = "";
            foreach (ItemCategoryType type in Enum.GetValues(typeof(ItemCategoryType)))
            {
                if ((itemCategoryType & type) > 0)
                {
                    str += "<span class='spacialTip'>" + type.GetDescription() + "</span>";
                }
            }
            return str;
        }

        /// <summary>
        /// GET: 订单详情
        /// </summary>
        /// <returns></returns>
        //[UserActionFilter(UserAuthorize = true)]
        //public ActionResult Info2(string id, string v)
        //{
        //    id = id.InputText();          
        //    var model = new ConfirmViewModel();
        //    var confirm = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[id];
        //    if (confirm == null)
        //    {
        //        return null;
        //    }

        //    var listCountries = Needs.Wl.User.Plat.UserPlat.Countries.ToList();
        //    var listUnits = Needs.Wl.User.Plat.UserPlat.Units.ToList();

        //    if (confirm.OrderStatus == OrderStatus.Quoted)
        //    {
        //        model.isPreConfirm = true;

        //        //税点
        //        var taxpoint = 1 + confirm.Client.Agreement.InvoiceTaxRate;
        //        //代理费率、最低代理费
        //        decimal agencyRate = confirm.AgencyFeeExchangeRate * confirm.Client.Agreement.AgencyRate;
        //        decimal minAgencyFee = confirm.Client.Agreement.MinAgencyFee;
        //        bool isAverage = confirm.DeclarePrice * agencyRate < minAgencyFee ? true : false;
        //        //平摊代理费、其他杂费
        //        decimal aveAgencyFee = confirm.AgencyFee * taxpoint / confirm.Items.Count();
        //        decimal aveOtherFee = confirm.OtherFee * taxpoint / confirm.Items.Count();

        //        //产品明细
        //        //TODO:优化Items查询
        //        model.Products = confirm.Items.Select(item => new ComfirmProducts
        //        {
        //            Origin = listCountries.Where(c => c.Code == item.Origin).FirstOrDefault() == null ? "" : listCountries.Where(c => c.Code == item.Origin).FirstOrDefault().Name,
        //            Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
        //            Batch = item.Batch,
        //            GrossWeight = item.GrossWeight,
        //            Name = item.Category?.Name ?? item.Name,
        //            Manufacturer = item.Manufacturer,
        //            Model = item.Model,
        //            ItemCategoryTypes = item.Category?.Type.GetFlagsDescriptions(",").Split(','),
        //            Quantity = item.Quantity,
        //            UnitPrice = item.UnitPrice,
        //            TotalPrice = item.TotalPrice,
        //            DeclareValue = item.TotalPrice * confirm.RealExchangeRate.Value,
        //            TraiffRate = item.ImportTax.Rate,
        //            Traiff = item.ImportTax.Value.Value,
        //            AddTaxRate = item.AddedValueTax.Rate,
        //            AddTax = item.AddedValueTax.Value.Value,
        //            AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint,
        //            InspectionFee = item.InspectionFee.GetValueOrDefault() * taxpoint,
        //            TotalTaxFee = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint,
        //            TotalTaxFee_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint,
        //            TotalDeclareValue = (decimal)(item.ImportTax.Value) + (decimal)(item.AddedValueTax.Value) + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
        //            TotalDeclareValue_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxpoint) + item.InspectionFee.GetValueOrDefault() * taxpoint + item.TotalPrice * confirm.RealExchangeRate.Value,
        //        }).ToArray();

        //        //产品合计
        //        var SumTraiff = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
        //        var SumAddValue = (confirm.Type != OrderType.Outside && model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
        //        model.Products_Num = model.Products.Sum(item => item.Quantity.ToRound(2));
        //        model.Products_DeclareValue = model.Products.Sum(item => item.DeclareValue).ToString("0.00");
        //        model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
        //        model.Products_Traiff = SumTraiff.ToString("0.00");
        //        model.Products_AddTax = SumAddValue.ToString("0.00");
        //        model.Products_AgencyFee = model.Products.Sum(item => item.AgencyFee).ToString("0.00");
        //        model.Products_InspectionFee = model.Products.Sum(item => item.InspectionFee).ToString("0.00");
        //        model.Products_TotalTaxFee = (model.Products.Sum(item => item.TotalTaxFee_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
        //        model.Products_TotalDeclareValue = (model.Products.Sum(item => item.TotalDeclareValue_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
        //    }
        //    else
        //    {
        //        //产品明细
        //        model.Products = confirm.Items.Select(item => new ComfirmProducts
        //        {
        //            Origin = listCountries.Where(s => s.Code == item.Origin).FirstOrDefault() == null ? "" : listCountries.Where(s => s.Code == item.Origin).FirstOrDefault().Name,
        //            Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
        //            Batch = item.Batch,
        //            GrossWeight = item.GrossWeight,
        //            Name = item.Category?.Name ?? item.Name,
        //            TotalPrice = item.TotalPrice,
        //            Manufacturer = item.Manufacturer,
        //            Model = item.Model,
        //            ItemCategoryTypes = item.Category?.Type.GetFlagsDescriptions(",").Split(','),
        //            Quantity = item.Quantity,
        //            UnitPrice = item.UnitPrice,
        //        }).ToArray();
        //        model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
        //    }

        //    //订单信息
        //    model.ID = confirm.ID;
        //    var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(confirm.Currency);
        //    model.Currency = currency == null ? "" : currency.Name;
        //    model.CurrencyCode = confirm.Currency;
        //    model.IsFullVehicle = confirm.IsFullVehicle ? "是" : "否";
        //    model.IsAdvanceMoneny = confirm.IsLoan ? "是" : "否";

        //    var pack = Needs.Wl.User.Plat.UserPlat.PackTypes.FindByCode(confirm.WarpType);
        //    if (pack != null)
        //    {
        //        model.WrapType = pack.Name;
        //    }

        //    model.PackNo = confirm.PackNo.ToString();
        //    model.Summary = confirm.Summary;
        //    model.CreateDate = confirm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
        //    model.OrderMaker = confirm.OrderMaker;

        //    var deliveryFile = confirm.Files.Where(item => item.FileType == FileType.DeliveryFiles).FirstOrDefault();
        //    if (deliveryFile != null)
        //    {
        //        model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
        //    }

        //    //香港交货方式信息
        //    var OrderConsignee = confirm.OrderConsignee;
        //    model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
        //    model.HKDeliveryType = OrderConsignee.Type.GetDescription();
        //    model.supplierContact = OrderConsignee.Contact;
        //    model.supplierContactMobile = OrderConsignee.Mobile;
        //    model.SupplierAddress = OrderConsignee.Address;
        //    model.WayBillNo = OrderConsignee.WayBillNo;
        //    if (OrderConsignee.Type == HKDeliveryType.PickUp)
        //    {
        //        model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
        //        model.isPickUp = true;
        //    }

        //    //国内交货信息
        //    var OrderConsignor = confirm.OrderConsignor;
        //    model.SZDeliveryType = OrderConsignor.Type.GetDescription();
        //    model.clientContact = OrderConsignor.Contact;
        //    model.clientContactMobile = OrderConsignor.Mobile;
        //    model.clientConsigneeAddress = OrderConsignor.Address;
        //    model.isSZPickUp = OrderConsignor.Type == SZDeliveryType.PickUpInStore;
        //    model.IDNumber = OrderConsignor.IDNumber;

        //    //发票信息
        //    model.invoice = new WebMvc.Models.InvoiceModel();
        //    var invoice = confirm.Client.Invoice;  //发票对象
        //    model.invoice.invoiceType = confirm.Client.Agreement.InvoiceType.GetDescription();
        //    model.invoice.invoiceDeliveryType = invoice.DeliveryType.GetDescription();
        //    model.invoice.invoiceTitle = invoice.Title;
        //    model.invoice.invoiceTel = invoice.Tel;
        //    model.invoice.invoiceTaxCode = invoice.TaxCode;
        //    model.invoice.invoiceAddress = invoice.Address;
        //    model.invoice.invoiceAccount = invoice.BankAccount;
        //    model.invoice.invoiceBank = confirm.Client.Invoice.BankName;
        //    model.invoice.contactName = confirm.Client.InvoiceConsignee.Name;
        //    model.invoice.contactMobile = confirm.Client.InvoiceConsignee.Mobile;
        //    model.invoice.contactTel = confirm.Client.InvoiceConsignee.Tel;
        //    model.invoice.contactAddress = confirm.Client.InvoiceConsignee.Address;
        //    //付汇供应商信息
        //    model.PayExchangeSupplier = confirm.PayExchangeSuppliers.Select(item => new
        //    {
        //        Name = item.ClientSupplier.ChineseName,
        //    }).ToArray();

        //    #region PI,报关委托书放在主订单上了
        //    //model.PIFiles = confirm.Files.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new
        //    //{
        //    //    Name = item.Name,
        //    //    Status = item.Status.GetDescription(),
        //    //    Url = AppConfig.Current.FileServerUrl + @"/" + item.Url,
        //    //}).ToArray();
        //    ////报关委托书
        //    //var file = confirm.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
        //    //if (file != null)
        //    //{
        //    //    model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + file.Url;
        //    //    model.AgentProxyName = file.Name;
        //    //    model.AgentProxyStatus = file.FileStatus == OrderFileStatus.Audited;
        //    //}
        //    //model.IsShowAgentProxy = (int)confirm.OrderStatus >= (int)OrderStatus.Quoted;
        //    //return View();
        //    #endregion
        //    return View(model);
        //}

        /// <summary>
        /// GET: 订单详情
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ConfirmViewModel InfoDetail(string id)
        {
            id = id.InputText();
            var model = new ConfirmViewModel();
            //var confirm = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[id];
            var current = Needs.Wl.User.Plat.UserPlat.Current;
            var confirm = current.MyOrders[id];
            if (confirm == null)
            {
                return null;
            }

            var listCountries = Needs.Wl.User.Plat.UserPlat.Countries.ToList();
            var listUnits = Needs.Wl.User.Plat.UserPlat.Units.ToList();
            var agreement = confirm.Agreement();//订单的补充协议
            var files = confirm.Files().ToList();//订单的附件
            var orderItems = confirm.CategoriedItems().ToArray();//产品明细
            int orderItemCount = orderItems.Count();//订单产品项数量

            //if (confirm.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Quoted)
            //{
            //    model.isPreConfirm = true;

            //    //税点
            //    var taxPoint = 1 + agreement.InvoiceTaxRate;
            //    //代理费率、最低代理费
            //    decimal agencyRate = confirm.AgencyFeeExchangeRate(agreement) * agreement.AgencyRate;
            //    decimal minAgencyFee = agreement.MinAgencyFee;
            //    bool isAverage = confirm.DeclarePrice * agencyRate < minAgencyFee ? true : false;

            //    var premiums = confirm.Premiums().ToArray();//订单的费用
            //    decimal agencyFee = premiums.Where(s => s.Type == Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee).Select(f => f.UnitPrice * f.Count * f.Rate).FirstOrDefault();
            //    decimal otherFee = premiums.Where(item => item.Type != Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee && item.Type != Needs.Wl.Models.Enums.OrderPremiumType.InspectionFee)
            //       .Sum(item => item.Count * item.UnitPrice * item.Rate);

            //    //平摊代理费、其他杂费
            //    decimal aveAgencyFee = agencyFee * taxPoint / orderItemCount;
            //    decimal aveOtherFee = otherFee * taxPoint / orderItemCount;

            //    //产品明细
            //    //TODO:优化Items查询
            //    model.Products = orderItems.Select(item => new ComfirmProducts
            //    {
            //        Origin = listCountries.Where(c => c.Code == item.Origin).FirstOrDefault()?.Name,
            //        Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault()?.Name,
            //        Batch = item.Batch,
            //        GrossWeight = item.GrossWeight,
            //        Name = string.IsNullOrEmpty(item.CategoriedName) ? item.Name : item.CategoriedName,
            //        Manufacturer = item.Manufacturer,
            //        Model = item.Model,
            //        ItemCategoryTypes = item.Type.GetFlagsDescriptions(",").Split(','),
            //        Quantity = item.Quantity,
            //        UnitPrice = item.UnitPrice,
            //        TotalPrice = item.TotalPrice,
            //        DeclareValue = item.TotalPrice * confirm.RealExchangeRate.Value,
            //        TraiffRate = item.ImportTaxRate,
            //        Traiff = item.ImportTaxValue,
            //        AddTaxRate = item.AddedValueRate,
            //        AddTax = item.AddedValue,
            //        AgencyFee = isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint,
            //        InspectionFee = premiums.Where(s => s.Type == Needs.Wl.Models.Enums.OrderPremiumType.InspectionFee && s.ID == item.ID).Select(s => s.UnitPrice * s.Count * s.Rate).FirstOrDefault(),
            //        TotalTaxFee = item.TotalTaxAndAddedValue + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint,
            //        TotalTaxFee_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint,
            //        TotalDeclareValue = item.TotalTaxAndAddedValue + (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint + item.TotalPrice * confirm.RealExchangeRate.Value,
            //        TotalDeclareValue_Except_TraAndAdd = (isAverage ? aveAgencyFee : item.TotalPrice * agencyRate * taxPoint) + item.InspectionFee.GetValueOrDefault(0) * taxPoint + item.TotalPrice * confirm.RealExchangeRate.Value,
            //    }).ToArray();

            //    //产品合计
            //    var SumTraiff = (confirm.Type != Needs.Wl.Models.Enums.OrderType.Outside && model.Products.Sum(item => item.Traiff) < 50) ? 0 : model.Products.Sum(item => item.Traiff);
            //    var SumAddValue = (confirm.Type != Needs.Wl.Models.Enums.OrderType.Outside && model.Products.Sum(item => item.AddTax) < 50) ? 0 : model.Products.Sum(item => item.AddTax);
            //    model.Products_Num = model.Products.Sum(item => item.Quantity.ToRound(2));
            //    model.Products_DeclareValue = model.Products.Sum(item => item.DeclareValue).ToString("0.00");
            //    model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
            //    model.Products_Traiff = SumTraiff.ToString("0.00");
            //    model.Products_AddTax = SumAddValue.ToString("0.00");
            //    model.Products_AgencyFee = model.Products.Sum(item => item.AgencyFee).ToString("0.00");
            //    model.Products_InspectionFee = model.Products.Sum(item => item.InspectionFee).ToString("0.00");
            //    model.Products_TotalTaxFee = (model.Products.Sum(item => item.TotalTaxFee_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
            //    model.Products_TotalDeclareValue = (model.Products.Sum(item => item.TotalDeclareValue_Except_TraAndAdd) + SumTraiff + SumAddValue).ToString("0.00");
            //}
            //else
            //{
            //产品明细
            model.Products = orderItems.Select(item => new ComfirmProducts
            {
                Origin = listCountries.Where(s => s.Code == item.Origin).FirstOrDefault() == null ? "" : listCountries.Where(s => s.Code == item.Origin).FirstOrDefault().Name,
                Unit = listUnits.Where(c => c.Code == item.Unit).FirstOrDefault() == null ? "" : listUnits.Where(c => c.Code == item.Unit).FirstOrDefault().Name,
                Batch = item.Batch,
                GrossWeight = item.GrossWeight,
                Name = string.IsNullOrEmpty(item.CategoriedName) ? item.Name : item.CategoriedName,
                TotalPrice = item.TotalPrice,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                ItemCategoryTypes = item.Type.GetFlagsDescriptions(",").Split(','),
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
            }).ToArray();
            model.Products_TotalPrice = model.Products.Sum(item => item.TotalPrice).ToString("0.00");
            //}

            model.PartProducts = new List<ComfirmProducts>();
            model.PartProducts.Add(model.Products.FirstOrDefault());


            //订单信息
            model.ID = confirm.ID;
            var currency = Needs.Wl.User.Plat.UserPlat.Currencies.FindByCode(confirm.Currency);
            model.Currency = currency == null ? "" : currency.Name;
            model.CurrencyCode = confirm.Currency;
            model.IsFullVehicle = confirm.IsFullVehicle ? "是" : "否";
            model.IsAdvanceMoneny = confirm.IsLoan ? "是" : "否";
            model.DeclarePrice = confirm.DeclarePrice;
            model.OrderStatusName = confirm.OrderStatus.GetDescription();
            model.OrderStatus = confirm.OrderStatus;

            var pack = Needs.Wl.User.Plat.UserPlat.PackTypes.FindByCode(confirm.WarpType);
            if (pack != null)
            {
                model.WrapType = pack.Name;
            }

            model.PackNo = confirm.PackNo.ToString();
            model.Summary = confirm.Summary;
            model.CreateDate = confirm.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.OrderMaker = confirm.User == null ? "跟单员" : confirm.User.RealName;

            var deliveryFile = files.Where(s => s.FileType == Needs.Wl.Models.Enums.FileType.DeliveryFiles).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
            }

            //香港交货方式信息
            var OrderConsignee = confirm.Consignee();
            model.Supplier = OrderConsignee.ClientSupplier.ChineseName;
            model.HKDeliveryType = OrderConsignee.Type.GetDescription();
            model.supplierContact = OrderConsignee.Contact;
            model.supplierContactMobile = OrderConsignee.Mobile;
            model.SupplierAddress = OrderConsignee.Address;
            model.WayBillNo = OrderConsignee.WayBillNo;
            if (OrderConsignee.Type == Needs.Wl.Models.Enums.HKDeliveryType.PickUp)
            {
                model.PickupTime = DateTime.Parse(OrderConsignee.PickUpTime.ToString()).ToString("yyyy-MM-dd");
                model.isPickUp = true;
            }

            //国内交货信息
            var OrderConsignor = confirm.Consignor();
            model.SZDeliveryType = OrderConsignor.Type.GetDescription();
            model.clientContact = OrderConsignor.Contact;
            model.clientContactName = OrderConsignor.Name;
            model.clientContactMobile = OrderConsignor.Mobile;
            model.clientConsigneeAddress = OrderConsignor.Address;
            model.isSZPickUp = OrderConsignor.Type == Needs.Wl.Models.Enums.SZDeliveryType.PickUpInStore;
            model.IDNumber = OrderConsignor.IDNumber;

            //发票信息 发票信息不需要显示了
            //model.invoice = new WebMvc.Models.InvoiceModel();
            //var invoice = current.Client.Invoice();  //发票对象
            //var invoiceConsignee = current.Client.InvoiceConsignee();  //发票收件人
            //model.invoice = new InvoiceModel()
            //{
            //    invoiceType = agreement.InvoiceType.GetDescription(),
            //    invoiceDeliveryType = invoice.DeliveryType.GetDescription(),
            //    invoiceTitle = invoice.Title,
            //    invoiceTaxCode = invoice.TaxCode,
            //    invoiceTel = invoice.Tel,
            //    invoiceAddress = invoice.Address,
            //    invoiceAccount = invoice.BankAccount,
            //    invoiceBank = invoice.BankName,
            //    contactName = invoiceConsignee.Name,
            //    contactMobile = invoiceConsignee.Mobile,
            //    contactTel = invoiceConsignee.Tel,
            //    contactAddress = invoiceConsignee.Address
            //};
            //付汇供应商信息

            model.PayExchangeSupplier = confirm.PayExchangeSupplier().ToArray().Select(item => new
            {
                Name = item.ChineseName,
            }).ToArray();

            #region PI,报关委托书放在主订单上了
            //model.PIFiles = confirm.Files.Where(item => item.FileType == FileType.OriginalInvoice).Select(item => new
            //{
            //    Name = item.Name,
            //    Status = item.Status.GetDescription(),
            //    Url = AppConfig.Current.FileServerUrl + @"/" + item.Url,
            //}).ToArray();
            ////报关委托书
            //var file = confirm.Files.Where(item => item.FileType == FileType.AgentTrustInstrument).FirstOrDefault();
            //if (file != null)
            //{
            //    model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + file.Url;
            //    model.AgentProxyName = file.Name;
            //    model.AgentProxyStatus = file.FileStatus == OrderFileStatus.Audited;
            //}
            //model.IsShowAgentProxy = (int)confirm.OrderStatus >= (int)OrderStatus.Quoted;
            //return View();
            #endregion

            return model;
        }

        /// <summary>
        /// 订单拆分之后，详情页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult Info(string id, string v)
        {
            id = id.InputText();
            v = v.InputText();
            ViewBag.navid = v;
            var model = new ConfirmViewModels();
            model.MainOrderID = id;
            model.Confirms = new List<ConfirmViewModel>();

            //如果传的ID是子订单ID
            string[] orderNoIfSub = id.Split('-');
            if (orderNoIfSub.Length > 1)
            {
                id = orderNoIfSub[0];
            }

            var orders = new OrdersView().Where(item => item.MainOrderID == id).Select(item => item.ID).ToList();
            foreach (var item in orders)
            {
                var test = InfoDetail(item);
                model.Confirms.Add(test);
                model.DeclarePrice += test.DeclarePrice;
            }


            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[orders[0]];
            var files = order.MainOrderFiles;
            //原始PI
            model.PIFiles = files.Where(file => file.FileType == FileType.OriginalInvoice).Select(item => new
            {
                Name = item.Name,
                Status = item.Status.GetDescription(),
                Url = AppConfig.Current.FileServerUrl + @"/" + item.Url,
            }).ToArray();

            //报关委托书
            var agencyFile = files.Where(item => item.FileType == FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (agencyFile != null)
            {
                model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + agencyFile.Url;
                model.AgentProxyName = agencyFile.Name;
                model.AgentProxyStatus = agencyFile.FileStatus == OrderFileStatus.Audited;
            }


            var orderBill = files.Where(item => item.FileType == FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (orderBill != null)
            {
                model.OrderBillURL = AppConfig.Current.FileServerUrl + @"/" + orderBill.Url;
                model.OrderBillName = orderBill.Name;
                model.OrderBillStatus = orderBill.FileStatus == Needs.Ccs.Services.Enums.OrderFileStatus.Audited;
            }

            var deliveryFile = files.Where(item => item.FileType == FileType.DeliveryFiles).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
            }


            model.PayExchangeSupplier = model.Confirms[0].PayExchangeSupplier;
            var mainorder = Needs.Wl.User.Plat.UserPlat.Current.MainOrders[id];
            model.CreateDate = mainorder.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.IsShowAgentProxy = !model.Confirms.Any(t => t.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Quoted);
            model.IsShowOrderBill = !model.Confirms.Any(t => t.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Quoted);
            return View(model);
        }

        /// <summary>
        /// GET: 订单详情
        /// </summary>
        /// <returns></returns>
        //[UserActionFilter(UserAuthorize = true)]
        //public ActionResult Info(string id, string v)
        //{
        //    id = id.InputText();
        //    v = v.InputText();
        //    ViewBag.navid = v;

        //    var current = Needs.Wl.User.Plat.UserPlat.Current;
        //    var order = current.MyOrders[id];

        //    //验证 Url是否非法输入
        //    if (order == null || order.ClientID != current.ClientID)
        //    {
        //        return View("Error");
        //    }

        //    var model = new ConfirmViewModel();

        //    var listCountries = Needs.Wl.User.Plat.UserPlat.Countries.ToList();
        //    var listUnits = Needs.Wl.User.Plat.UserPlat.Units.ToList();
        //    var agreement = order.Agreement();//订单的补充协议
        //    var files = order.Files().ToList();//订单的附件
        //    var orderItems = order.CategoriedItemsItems().ToArray();//产品明细
        //    int orderItemCount = orderItems.Count();//订单产品项数量

        //    if (order.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Quoted)
        //    {
        //        model.isPreConfirm = true;
        //        //增值税税率
        //        var taxPoint = 1 + agreement.InvoiceTaxRate;
        //        //代理费率、最低代理费
        //        decimal agencyRate = order.AgencyFeeExchangeRate() * agreement.AgencyRate;
        //        decimal minAgencyFee = agreement.MinAgencyFee;
        //        bool isAverage = order.DeclarePrice * agencyRate < minAgencyFee ? true : false;

        //        var premiums = order.Premiums().ToArray();//订单的费用
        //        decimal agencyFee = premiums.Where(s => s.Type == Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee).Select(f => f.UnitPrice * f.Count * f.Rate).FirstOrDefault();
        //        decimal otherFee = premiums.Where(item => item.Type != Needs.Wl.Models.Enums.OrderPremiumType.AgencyFee && item.Type != Needs.Wl.Models.Enums.OrderPremiumType.InspectionFee)
        //           .Sum(item => item.Count * item.UnitPrice * item.Rate);

        //        //平摊代理费、其他杂费
        //        decimal aveAgencyFee = agencyFee * taxPoint / orderItemCount;
        //        decimal aveOtherFee = otherFee * taxPoint / orderItemCount;

        public ActionResult InfoForIcgoo(string id, string v)
        {
            id = id.InputText();
            v = v.InputText();
            ViewBag.navid = v;
            var model = new ConfirmViewModels();
            model.MainOrderID = id;
            model.Confirms = new List<ConfirmViewModel>();

            //如果传的ID是子订单ID
            string[] orderNoIfSub = id.Split('-');
            if (orderNoIfSub.Length > 1)
            {
                id = orderNoIfSub[0];
            }

            model.ProductsForIcgoo = new List<ComfirmProducts>();
            model.PartProductsForIcgoo = new List<ComfirmProducts>();

            var orders = new OrdersView().Where(item => item.MainOrderID == id).Select(item => item.ID).ToList();
            foreach (var item in orders)
            {
                var test = InfoDetail(item);
                model.Confirms.Add(test);
                model.DeclarePrice += test.DeclarePrice;

                model.ProductsForIcgoo.AddRange(test.Products);
                model.PartProductsForIcgoo.AddRange(test.PartProducts);
            }


            var order = Needs.Wl.User.Plat.UserPlat.Current.WebSite.MyOrders1[orders[0]];
            var files = order.MainOrderFiles;
            //原始PI
            model.PIFiles = files.Where(file => file.FileType == FileType.OriginalInvoice).Select(item => new
            {
                Name = item.Name,
                Status = item.Status.GetDescription(),
                Url = AppConfig.Current.FileServerUrl + @"/" + item.Url,
            }).ToArray();

            //报关委托书
            var agencyFile = files.Where(item => item.FileType == FileType.AgentTrustInstrument).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (agencyFile != null)
            {
                model.AgentProxyURL = AppConfig.Current.FileServerUrl + @"/" + agencyFile.Url;
                model.AgentProxyName = agencyFile.Name;
                model.AgentProxyStatus = agencyFile.FileStatus == OrderFileStatus.Audited;
            }


            var orderBill = files.Where(item => item.FileType == FileType.OrderBill).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (orderBill != null)
            {
                model.OrderBillURL = AppConfig.Current.FileServerUrl + @"/" + orderBill.Url;
                model.OrderBillName = orderBill.Name;
                model.OrderBillStatus = orderBill.FileStatus == Needs.Ccs.Services.Enums.OrderFileStatus.Audited;
            }

            var deliveryFile = files.Where(item => item.FileType == FileType.DeliveryFiles).OrderByDescending(item => item.CreateDate).FirstOrDefault();
            if (deliveryFile != null)
            {
                model.DeliveryFiles = AppConfig.Current.FileServerUrl + @"/" + deliveryFile.Url.ToUrl();
            }


            model.PayExchangeSupplier = model.Confirms[0].PayExchangeSupplier;
            var mainorder = Needs.Wl.User.Plat.UserPlat.Current.MainOrders[id];
            model.CreateDate = mainorder.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
            model.IsShowAgentProxy = !model.Confirms.Any(t => t.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Quoted);
            model.IsShowOrderBill = !model.Confirms.Any(t => t.OrderStatus <= Needs.Wl.Models.Enums.OrderStatus.Quoted);
            return View(model);
        }


        /// <summary>
        /// 发票详情
        /// </summary>
        /// <param name="id">订单ID</param>
        /// <param name="v">上级页面ID</param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public async Task<ActionResult> Invoice(string id, string v)
        {
            ViewBag.navid = v;//返回上一级页面

            id = id.InputText();

            var model = new OrdersInvoiceViewModel();
            model.ID = id;

            var order = Needs.Wl.User.Plat.UserPlat.Current.MyOrders[id];
            if (order == null)
            {
                return View("Error");
            }

            model.invoice = new WebMvc.Models.InvoiceModel();

            //TODO:订单中的发票信息应该使用开票通知中的发票信息
            var current = Needs.Wl.User.Plat.UserPlat.Current.Client;
            var invoice = await current.InvoiceAsync();//客户的开票信息
            var invoiceConsignee = await current.InvoiceConsigneeAsyn();  //发票收件地址
            var agreement = await order.AgreementAsync();//订单的补充协议
            var orderInvoices = order.Invoices(); //订单的发票信息

            //发票信息
            model.invoice.invoiceType = agreement.InvoiceType.GetDescription();
            model.invoice.invoiceDeliveryType = invoice.DeliveryType.GetDescription();
            model.invoice.invoiceTitle = invoice.Title;
            model.invoice.invoiceTel = invoice.Tel;
            model.invoice.invoiceTaxCode = invoice.TaxCode;
            model.invoice.invoiceAddress = invoice.Address;
            model.invoice.invoiceAccount = invoice.BankAccount;
            model.invoice.invoiceBank = invoice.BankName;
            model.invoice.contact = invoiceConsignee.Name;
            model.invoice.mobile = invoiceConsignee.Mobile;
            model.invoice.contactAddress = invoiceConsignee.Address;

            //如果订单已经开票
            if (orderInvoices.RecordCount > 0)
            {
                var waybill = await order.InvoicesWaybill().FirstOrDefaultAsync(); //发票运单
                if (waybill != null)
                {
                    model.invoice.ExpressCompany = waybill.Carrier.Name;
                    model.invoice.ExpressCode = waybill.WaybillCode;
                    model.invoice.CreateDate = waybill.CreateDate.ToString("yyyy-MM-dd HH:mm:ss");
                }

                model.invoice.InvoiceNo = orderInvoices.GetOrderInvoicesNo();
            }

            //海关发票
            if (agreement.InvoiceType == Needs.Wl.Models.Enums.InvoiceType.Service)
            {
                model.IsCustomsInvoice = (int)order.OrderStatus >= (int)OrderStatus.Declared;  //开服务发票有海关发票
                if (model.IsCustomsInvoice)
                {
                    var decHeadFiles = await order.DecHeadFiles().ToListAsync();//报关单附件
                    var decHeadTaxs = await order.DecHeadTaxs().ToListAsync();//关税、增值税、消费税

                    var addedValue = decHeadTaxs.Where(s => s.TaxType == Needs.Wl.Models.Enums.DecTaxType.AddedValueTax).FirstOrDefault();
                    var addedValueFile = decHeadFiles.Where(s => s.FileType == Needs.Wl.Models.Enums.FileType.DecHeadVatFile).FirstOrDefault();

                    if (addedValue != null && addedValueFile != null)
                    {
                        model.IsAddInvoice = true;
                        model.VATInvoice = AppConfig.Current.FileServerUrl + @"/" + addedValueFile.Url.ToUrl();
                    }

                    var tariff = decHeadTaxs.Where(s => s.TaxType == Needs.Wl.Models.Enums.DecTaxType.Tariff).FirstOrDefault();
                    var tariffFile = decHeadFiles.Where(s => s.FileType == Needs.Wl.Models.Enums.FileType.DecHeadTariffFile).FirstOrDefault();

                    if (tariff != null && tariffFile != null)
                    {
                        model.IsTariffInvoice = true;
                        model.TariffInvoice = AppConfig.Current.FileServerUrl + @"/" + tariffFile.Url.ToUrl();
                    }

                    var consumptionTax = decHeadTaxs.Where(s => s.TaxType == Needs.Wl.Models.Enums.DecTaxType.ConsumptionTax).FirstOrDefault();
                    var consumptionTaxFile = decHeadFiles.Where(s => s.FileType == Needs.Wl.Models.Enums.FileType.ConsumptionTaxFile).FirstOrDefault();

                    if (consumptionTax != null && consumptionTaxFile != null)
                    {
                        model.IsConsumptionTax = true;
                        model.ConsumptionTaxInvoice = AppConfig.Current.FileServerUrl + @"/" + consumptionTaxFile.Url.ToUrl();
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// 订单跟踪
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetTrace(string id)
        {
            id = id.InputText();

            var order = Needs.Wl.User.Plat.UserPlat.Current.MyOrders[id];
            var view = order.Traces();
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "订单状态异常");
            }

            var log = view.ToList().Select(item => new
            {
                Step = item.Step.GetDescription(),
                Date = item.CreateDate.ToString("yyyy-MM-dd") + "/" + item.CreateDate.GetWeekName(),
                Time = item.CreateDate.ToString("HH:mm:ss"),
                Summary = item.Summary,
                isCompleted = order.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Completed,
                isDot = false
            });
            return base.JsonResult(VueMsgType.success, "", log.Json());
        }

        /// <summary>
        /// 主订单跟踪
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetMainTrace(string id)
        {
            id = id.InputText();

            var order = Needs.Wl.User.Plat.UserPlat.Current.MainOrders[id];
            var view = order.Traces();
            if (order == null)
            {
                return base.JsonResult(VueMsgType.error, "订单状态异常");
            }

            var log = view.ToList().Select(item => new
            {
                Step = item.Step.GetDescription(),
                Date = item.CreateDate.ToString("yyyy-MM-dd") + "/" + item.CreateDate.GetWeekName(),
                Time = item.CreateDate.ToString("HH:mm:ss"),
                Summary = item.Summary,
                isCompleted = order.OrderStatus == Needs.Wl.Models.Enums.OrderStatus.Completed,
                isDot = false
            });
            return base.JsonResult(VueMsgType.success, "", log.Json());
        }



        #endregion

        #region 文件操作 文件上传下载

        /// <summary>
        ///保存报关委托书
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SaveAgentProxy(string id, string URL, string name, string fileFormat)
        {
            try
            {
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                id = id.InputText();
                var order = current.MyMianOrders[id];
                if (order == null)
                {
                    return base.JsonResult(VueMsgType.error, "错误的订单信息，上传失败！");
                }

                var file = order.Files().GetAgentTrustInstrument();
                if (file != null && file.FileStatus == Needs.Wl.Models.Enums.OrderFileStatus.Audited)
                {
                    return base.JsonResult(VueMsgType.error, "对账单文件已审核通过，不可重新上传。");
                }

                if (file == null)
                {
                    file = new Needs.Wl.Models.MainOrderFile();
                }

                file.MainOrderID = id;
                file.FileType = Needs.Wl.Models.Enums.FileType.AgentTrustInstrument;
                file.FileFormat = fileFormat.InputText();
                file.FileStatus = Needs.Wl.Models.Enums.OrderFileStatus.Auditing;
                file.Name = name.InputText();
                file.Url = URL.InputText();
                file.User = current.ToUser();
                file.Enter();

                return base.JsonResult(VueMsgType.success, "上传成功！");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "上传失败");
            }
        }

        /// <summary>
        ///保存对账单
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult SaveOrderBill(string id, string URL, string name, string fileFormat)
        {
            try
            {
                var current = Needs.Wl.User.Plat.UserPlat.Current;
                id = id.InputText();

                var order = current.MyMianOrders[id];
                if (order == null)
                {
                    return base.JsonResult(VueMsgType.error, "错误的订单信息，上传失败！");
                }

                var file = order.Files().GetOrderBill();
                if (file != null && file.FileStatus == Needs.Wl.Models.Enums.OrderFileStatus.Audited)
                {
                    return base.JsonResult(VueMsgType.error, "对账单文件已审核通过，不可重新上传。");
                }

                if (file == null)
                {
                    file = new Needs.Wl.Models.MainOrderFile();
                }


                file.MainOrderID = id;
                file.FileType = Needs.Wl.Models.Enums.FileType.OrderBill;
                file.FileFormat = fileFormat.InputText();
                file.FileStatus = Needs.Wl.Models.Enums.OrderFileStatus.Auditing;
                file.Name = name.InputText();
                file.Url = URL.InputText();
                file.User = current.ToUser();
                file.Enter();

                return base.JsonResult(VueMsgType.success, "上传成功！");
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, "上传失败");
            }
        }

        /// <summary>
        /// 导出报关委托书
        /// </summary>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DownloadAgentProxy2(string id)
        {
            try
            {
                id = id.InputText();

                //TODO:改写成这样
                //var files = Needs.Wl.User.Plat.UserPlat.Current.OrderContext[id].Files;
                //var agentProxy = files.GetAgentTrustInstrument();

                var agentProxy = new MainOrderAgentProxiesView()[id];
                if (agentProxy == null)
                {
                    return base.JsonResult(VueMsgType.error, "导出失败！");
                }


                //创建文件目录
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                var vendor = new VendorContext(VendorContextInitParam.OrderID, agentProxy.Order.ID).Current1;

                agentProxy.SaveAs(file.FilePath, vendor);
                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 导出报关委托书
        /// </summary>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult DownloadAgentProxy(string id)
        {
            try
            {
                id = id.InputText();
                string[] orderids = id.Split('-');

                var Orders = new Orders2View().Where(item => item.MainOrderID == orderids[0] && item.OrderStatus != OrderStatus.Canceled && item.OrderStatus != OrderStatus.Returned).ToList();
                var agentProxy = new Needs.Ccs.Services.Views.MainOrderAgentProxiesView().Where(t => t.Order.ID == Orders.FirstOrDefault().ID).FirstOrDefault();

                //创建文件目录
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload);
                file.CreateDateDirectory();

                var vendor = new VendorContext(VendorContextInitParam.OrderID, agentProxy.Order.ID).Current1;

                if (agentProxy.Client.ClientType == ClientType.External)
                {
                    MainOrderAgentProxyViewModel model = new MainOrderAgentProxyViewModel();
                    model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                    model.Orders = Orders;
                    var order = Orders.FirstOrDefault();
                    model.ID = order.MainOrderID;
                    model.Client = agentProxy.Client;
                    model.PackNo = Orders.Sum(t => t.PackNo);
                    model.WarpType = order.WarpType;
                    model.Currency = order.Currency;
                    model.SaveAs(file.FilePath);
                }
                else
                {
                    //itextsharp生成，超过10页
                    AgentProxyToPdf model = new AgentProxyToPdf();
                    model.Orders = new List<Needs.Ccs.Services.Models.Order>();
                    model.Orders = Orders;
                    var order = Orders.FirstOrDefault();
                    model.ID = order.MainOrderID;
                    model.Client = agentProxy.Client;
                    model.PackNo = Orders.Sum(t => t.PackNo);
                    model.WarpType = order.WarpType;
                    model.Currency = order.Currency;
                    model.SaveAs(file.FilePath);
                }
                
                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }


        /// <summary>
        /// 导出提货单
        /// </summary>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult ImportTihuoFile(string id)
        {
            try
            {
                id = id.InputText();
                var notice = new SZExitNoticeView()[id];

                //创建文件目录
                Needs.Wl.Web.Mvc.Utils.FileDirectory file = new Needs.Wl.Web.Mvc.Utils.FileDirectory(DateTime.Now.Ticks + ".pdf");
                file.SetChildFolder(SysConfig.Dowload).CreateDateDirectory();
                notice.SaveAs(file.FilePath);

                return base.JsonResult(VueMsgType.success, "", file.LocalFileUrl);
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 我的订单文件上传
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult FileUpload(HttpPostedFileBase file)
        {
            var result = new List<OrderProductModel>();
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];
                string fileExtension = Path.GetExtension(file.FileName).ToLower();//文件拓展名
                string[] allowFiles = { ".xls", ".xlsx" };
                if (allowFiles.Contains(fileExtension) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传.xls或.xlsx格式的文件。");
                }

                System.Data.DataTable dt = Needs.Wl.Web.Mvc.Utils.NPOIHelper.ExcelToDataTableProducts(fileExtension, file.InputStream, true);

                foreach (System.Data.DataRow row in dt.Rows)
                {
                    var array = row.ItemArray;
                    int no = 0;
                    if (!int.TryParse(array[0].ToString(), out no))
                    {
                        continue;
                    }

                    //无效行
                    if (array[7].ToString() == string.Empty && array[10].ToString() == string.Empty)
                    {
                        continue;
                    }

                    var orignvalue = "";  //原产地值
                    var orignlable = "";  //原产地名称

                    //国家地区查询
                    var viewCountries = Needs.Wl.User.Plat.UserPlat.Countries;
                    viewCountries.Predicate = item => item.Name == array[8].ToString() || item.Code == array[8].ToString().ToUpper();
                    var orign = viewCountries.FirstOrDefault();
                    if (orign != null)
                    {
                        orignvalue = orign.Code;
                        orignlable = orign.Name;
                    }

                    var model = new OrderProductModel
                    {
                        ProductUnionCode = array[1].ToString(),
                        Batch = array[3].ToString(),
                        Name = array[5].ToString(),
                        Manufacturer = array[6].ToString(),
                        Model = array[7].ToString(),
                        Origin = orignvalue,
                        OriginLabel = orignlable,
                        Quantity = array[10].GetType().Name == "DBNull" ? 0 : Convert.ToDecimal(array[10]),
                        Unit = "007", //单位默认个
                        UnitLabel = "007 个",
                        TotalPrice = array[12].GetType().Name == "DBNull" ? 0 : Math.Round(Convert.ToDecimal(array[12]), 2, MidpointRounding.AwayFromZero),
                    };

                    model.UnitPrice = Math.Round(model.TotalPrice / model.Quantity, 4);

                    result.Add(model);
                }
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
            return base.JsonResult(VueMsgType.success, "", result.Json());
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult UploadOrderFile(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                //后台也要校验
                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && fileFormat == ".pdf")
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }
                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片或者pdf文件！");
                }
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(SysConfig.Order);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);
                return base.JsonResult(VueMsgType.success, "", new FileModel { name = fileName, URL = result[1], fileFormat = file.ContentType, fullURL = result[2] }.Json());
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }

        /// <summary>
        /// 提货文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult UploadPickUpFile(HttpPostedFileBase file)
        {
            try
            {
                //拿到上传来的文件
                file = Request.Files["file"];

                string fileFormat = Path.GetExtension(file.FileName).ToLower();
                string[] Files = { ".pdf", ".doc", ".docx" };
                //后台也要校验
                var fileSize = file.ContentLength / 1024;
                if (fileSize > 1024 * 3 && Files.Contains(fileFormat))
                {
                    return base.JsonResult(VueMsgType.error, "上传的文件大小不超过3M！");
                }

                string[] allowFiles = { ".jpg", ".bmp", ".pdf", ".gif", ".png", ".pdf", ".doc", ".docx", ".jpeg" };
                if (allowFiles.Contains(fileFormat) == false)
                {
                    return base.JsonResult(VueMsgType.error, "文件格式错误，请上传图片、Word文件或者pdf文件！");
                }
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(SysConfig.Order);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);
                var contentType = file.ContentType;
                if (fileFormat == ".docx")
                {
                    contentType = "application/msword";
                }
                return base.JsonResult(VueMsgType.success, "", new { name = fileName, URL = result[1], fileFormat = contentType, fullURL = result[2] }.Json());
            }
            catch (Exception ex)
            {
                ex.Log();
                return base.JsonResult(VueMsgType.error, ex.Message);
            }
        }
        #endregion

        #region 付款记录



        #endregion

        #region 缴税记录
        [UserActionFilter(UserAuthorize = true)]
        public ActionResult TaxRecord()
        {
            return View();
        }

        #endregion

        #region 待收货，改成按出库通知显示
        /// <summary>
        /// 获取待出库通知
        /// </summary>
        /// <returns></returns>
        [UserActionFilter(UserAuthorize = true)]
        public JsonResult GetUnExitedNotice()
        {
            int page = int.Parse(Request.Form["page"]);
            int rows = int.Parse(Request.Form["rows"]);

            Needs.Wl.Warehouse.Services.Views.SZUnExitedListViewNew view = new Needs.Wl.Warehouse.Services.Views.SZUnExitedListViewNew();
            view.AllowPaging = true;
            view.PageIndex = page;
            view.PageSize = rows;

            int recordCount = view.RecordCount;
            var exitNotices = view.ToList();

            Func<Needs.Wl.Warehouse.Services.PageModels.SZUnExitedListViewNewModels, object> convert = item => new
            {
                ID = item.ExitNoticeID,
                OrderID = item.OrderID,//订单编号
                ClientCode = item.ClientCode,
                ClientName = item.ClientName,//客户

                PackNo = item.PackNo,
                AdminName = item.AdminName,//制单人
                ExitType = item.ExitType.GetDescription(),
                NoticeStatus = item.ExitNoticeStatus.GetDescription(),
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
            };


            return JsonResult(VueMsgType.success, "", new { list = exitNotices.Select(convert).ToArray(), recordCount }.Json());
        }

        [UserActionFilter(UserAuthorize = true)]
        public ActionResult ExitItems()
        {
            string id = Request["ID"];
            var exitNotice = new Needs.Ccs.Services.Views.SZExitNoticeItemView().Where(x => x.ExitNoticeID == id).OrderBy(x => x.StoreStorage.BoxIndex);
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                WrapType = item.Sorting.WrapType,
                Manufacturer = item.Sorting.OrderItem.Manufacturer,
                UpdateDate = item.UpdateDate//装箱日期
            };
            return View(exitNotice.Select(convert));
        }
        #endregion
    }
}