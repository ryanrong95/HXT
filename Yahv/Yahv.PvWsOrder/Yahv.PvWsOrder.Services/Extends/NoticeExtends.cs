using Layers.Data.Sqls;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Extends
{
    /// <summary>
    /// 通知拓展类
    /// </summary>
    public static class NoticeExtends
    {
        #region 会员端接口
        /// <summary>
        /// 重构后的代报关入库通知
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string CgDecEntryNotice(this ClientModels.OrderExtends order)
        {
            #region 拼凑入库通知对象
            var Waybill = new
            {
                WaybillID = order.InWaybill.ID,
                Supplier = order.InWaybill.Supplier,
                Type = order.InWaybill.Type,
            };
            var Notices = order.OrderItems.Select(item => new
            {
                Type = CgNoticeType.Enter,
                WareHouseID = WhSettings.HK["HK02"].ID,//TODO:报关外单到畅运库房
                WaybillID = order.InWaybill.ID,
                item.InputID,
                item.OutputID,
                item.CustomName,
                item.DateCode,
                item.Origin,
                item.Quantity,
                Source = CgNoticeSource.AgentBreakCustoms,
                Target = NoticesTarget.Default,
                Weight = item.GrossWeight,
                item.Volume,
                Conditions = new
                {
                    DevanningCheck = order.InWaybill.WayCondition.UnBoxed,//是否拆箱验货
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                    IsDeclared = item.OrderItemCondition.IsDeclare, //是否报关
                    IsCCC = item.ItemsTerm.Ccc, //CCC
                    IsCIQ = item.ItemsTerm.CIQ, //商检
                    IsEmbargo = item.ItemsTerm.Embargo, //禁运
                    IsHighPrice = item.ItemsTerm.IsHighPrice, //高价值
                }.Json(),
                Product = new
                {
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Product.PackageCase,
                    item.Product.Packaging,
                },
                Input = new
                {
                    ID = item.InputID,
                    Code = item.InputID,
                    OrderID = item.OrderID,
                    item.TinyOrderID,
                    ItemID = item.ID,
                    ClientID = order.ClientID,
                    PayeeID = order.PayeeID,
                    //ThirdID = order.ClientID,
                    Currency = item.Currency,
                    UnitPrice = item.UnitPrice,
                },
            });
            var data = new
            {
                Waybill,
                Enter = new { Notices },
            };
            var jsondata = data.Json();
            #endregion

            //传输日志记录
            Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
            {
                MainID = order.ID,
                Operation = "库房入库通知备份",
                Summary = jsondata,
            });

            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
            return result;
        }

        /// <summary>
        /// 转报关单装箱通知
        /// </summary>
        /// <param name="order">订单数据</param>
        /// <returns></returns>
        public static string CgDecBoxingNotice(this ClientModels.OrderExtends order)
        {
            var Waybill = new
            {
                WaybillID = order.OutWaybill.ID,
                Supplier = order.OutWaybill.Supplier,
                Type = order.OutWaybill.Type,
                NoticeType = CgNoticeType.Boxing,
                Source = CgNoticeSource.AgentCustomsFromStorage,
                AdminID = order.CreatorID,
            };
            var Notices = order.OrderItems.Select(item => new
            {
                WareHouseID = WhSettings.HK["HK02"].ID,//TODO:报关外单到畅运库房
                item.StorageID,
                item.Quantity,
                Weight = item.GrossWeight,
                item.Volume,
                Conditions = new
                {
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                    IsDeclared = item.OrderItemCondition.IsDeclare, //是否报关
                    IsCCC = item.ItemsTerm.Ccc, //CCC
                    IsCIQ = item.ItemsTerm.CIQ, //商检
                    IsEmbargo = item.ItemsTerm.Embargo, //禁运
                    IsHighPrice = item.ItemsTerm.IsHighPrice, //高价值
                }.Json(),
                Output = new
                {
                    ID = item.OutputID,
                    InputID = item.InputID,
                    item.OrderID,
                    item.TinyOrderID,
                    ItemID = item.ID,
                    OwnerID = order.ClientID,
                    Currency = item.Currency,
                    Price = item.UnitPrice,
                },
            });
            //入库通知结构
            var data = new
            {
                Waybill = Waybill,
                Enter = new { Notices },
            };

            //传输日志记录
            Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
            {
                MainID = order.ID,
                Operation = "库房装箱通知备份",
                Summary = data.Json(),
            });

            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgBoxingNotice;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);

            return result;
        }

        /// <summary>
        /// 产生出库通知
        /// </summary>
        /// <param name="order"></param>
        public static string CgOutNotice(this ClientModels.OrderExtends order)
        {
            #region 拼凑出库对象
            var Waybill = new
            {
                WaybillID = order.OutWaybill.ID,
                Supplier = order.OutWaybill.Supplier,
                Type = order.OutWaybill.Type,
                Requirement = order.Requirements.Select(item => new
                {
                    item.ID,
                    OrderID = order.ID,
                    item.Type,
                    item.Name,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice,
                    item.Requirement,
                }).ToArray(),
                CheckRequirement = new
                {
                    ApplicationID = order.Receive?.ID,
                    DelivaryOpportunity = (int?)order.Receive?.DelivaryOpportunity.GetValueOrDefault(),
                }
            };

            var Notices = order.OrderItems.Select(item => new
            {
                Type = CgNoticeType.Out,
                WareHouseID = WhSettings.HK["HK01"].ID,//代仓储默认香港万路通库房
                WaybillID = order.OutWaybill.ID,
                InputID = item.InputID,
                OutputID = item.OutputID,
                ProductID = item.ProductID,
                DateCode = item.DateCode,
                Quantity = item.Quantity,
                Source = CgNoticeSource.AgentSend,
                Target = NoticesTarget.Default,
                Weight = item.GrossWeight,
                Volume = item.Volume,
                StorageID = item.StorageID,
                Origin = item.Origin,
                Conditions = new
                {
                    DevanningCheck = order.OutWaybill.WayCondition.UnBoxed,//是否拆箱验货 ????
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                }.Json(),
                Output = new
                {
                    ID = item.OutputID,
                    InputID = item.InputID,
                    OrderID = order.ID,
                    item.TinyOrderID,
                    ItemID = item.ID,
                    OwnerID = order.ClientID,
                    Currency = item.Currency,
                    Price = item.UnitPrice,
                },
            }).ToArray();

            var data = new
            {
                Waybill,
                Notices,
            };
            #endregion

            //传输日志记录
            Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
            {
                MainID = order.ID,
                Operation = "库房出库通知备份",
                Summary = data.Json(),
            });

            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgOutNotice;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);

            return result;
        }

        /// <summary>
        /// 芯达通订单新增对接
        /// </summary>
        /// <param name="orderExtends"></param>
        /// <returns></returns>
        public static string XDTOrderNotice(this ClientModels.OrderExtends orderExtends)
        {
            var xdtorder = new XDTModels.ReceptorOrder()
            {
                ID = orderExtends.ID,
                UserID = orderExtends.CreatorID,
                ClientName = orderExtends.XDTClientName,
                ClientCode = orderExtends.EnterCode,
                IsReturned = orderExtends.IsReturned,
                Summary = orderExtends.Summary,
            };

            //订单项
            xdtorder.Items = orderExtends.OrderItems.Select(item => new XDTModels.OrderItems()
            {
                ID = item.ID,
                PreProductID = item.PreProductID,
                TinyOrderID = item.TinyOrderID ?? item.OrderID + "-01",
                Name = item.Name,
                Origin = item.Origin,
                Quantity = item.Quantity,
                Unit = item.Unit.GetUnit().Code,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice,
                GrossWeight = item.GrossWeight,
                Model = item.Product.PartNumber,
                Manufacturer = item.Product.Manufacturer,
                ProductUniqueCode = item.ProductUnicode ?? "",
                Batch = item.DateCode,
            }).ToList();

            #region 香港提货信息,深圳交货信息
            if (orderExtends.Type == OrderType.Declare)
            {
                xdtorder.IsFullVehicle = orderExtends.InWaybill.WayCondition.IsCharterBus;
                xdtorder.Currency = orderExtends.Input.Currency.ToString();
                xdtorder.IsLoan = orderExtends.Input.IsPayCharge.GetValueOrDefault();
                xdtorder.PackNo = orderExtends.InWaybill.TotalParts;
                xdtorder.WarpType = "22";
                xdtorder.OrderConsignee = new XDTModels.OrderConsignee()
                {
                    Type = orderExtends.InWaybill.Type == WaybillType.PickUp ? XDTModels.HKDeliveryType.PickUp : XDTModels.HKDeliveryType.SentToHKWarehouse,
                    WayBillNo = orderExtends.InWaybill.Code,
                };
                //香港提货信息录入
                if (orderExtends.InWaybill.Type == WaybillType.PickUp)
                {
                    xdtorder.OrderConsignee.Contact = orderExtends.InWaybill.WayLoading?.TakingContact;
                    xdtorder.OrderConsignee.Mobile = orderExtends.InWaybill.WayLoading?.TakingPhone;
                    xdtorder.OrderConsignee.Address = orderExtends.InWaybill.WayLoading?.TakingAddress;
                    xdtorder.OrderConsignee.PickUpTime = orderExtends.InWaybill.WayLoading?.TakingDate;
                }
            }
            else
            {
                xdtorder.IsFullVehicle = orderExtends.OutWaybill.WayCondition.IsCharterBus;
                xdtorder.Currency = orderExtends.Output.Currency.ToString();
                xdtorder.IsLoan = false;
                xdtorder.PackNo = orderExtends.OutWaybill.TotalParts;
                xdtorder.WarpType = "22";
                xdtorder.OrderConsignee = new XDTModels.OrderConsignee()
                {
                    Type = XDTModels.HKDeliveryType.SentToHKWarehouse,
                };
            }

            //深圳客户自提
            if (orderExtends.OutWaybill.Type == WaybillType.PickUp)
            {
                xdtorder.OrderConsignor = new XDTModels.OrderConsignor()
                {
                    Type = XDTModels.SZDeliveryType.PickUpInStore,
                    Name = orderExtends.OutWaybill.Consignee.Company,
                    Contact = orderExtends.OutWaybill.WayLoading.TakingContact,
                    Mobile = orderExtends.OutWaybill.WayLoading.TakingPhone,
                    IDNumber = orderExtends.OutWaybill.Consignee.IDNumber,
                    IDType = ((int?)orderExtends.OutWaybill.Consignee.IDType).ToString(),
                };
            }
            else
            {
                xdtorder.OrderConsignor = new XDTModels.OrderConsignor()
                {
                    Type = orderExtends.OutWaybill.Type == WaybillType.DeliveryToWarehouse ? XDTModels.SZDeliveryType.SentToClient :
                        XDTModels.SZDeliveryType.Shipping,
                    Name = orderExtends.OutWaybill.Consignee.Company,
                    Contact = orderExtends.OutWaybill.Consignee.Contact,
                    Mobile = orderExtends.OutWaybill.Consignee.Phone,
                    Address = orderExtends.OutWaybill.Consignee.Address,
                };
            }
            #endregion

            //供应商查询
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                var suppliers = new ClientViews.MySuppliers(orderExtends.ClientID);
                if (orderExtends.Type == OrderType.Declare)
                {
                    xdtorder.OrderConsignee.ClientSupplierName = suppliers[orderExtends.SupplierID].EnglishName;
                }
                else
                {
                    xdtorder.OrderConsignee.ClientSupplierName = suppliers.FirstOrDefault(item => item.OwnID == orderExtends.ClientID).EnglishName;
                }
                var supplier = suppliers.Where(item => orderExtends.PayExchangeSuppliers.Contains(item.ID)).ToList();
                xdtorder.PayExchangeSuppliers = suppliers.Where(item => orderExtends.PayExchangeSuppliers.Contains(item.ID)).Select(item => new XDTModels.OrderPayExchangeSuppliers
                {
                    ClientSupplierName = item.EnglishName,
                }).ToList();
            }

            //传输日志记录
            Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
            {
                MainID = orderExtends.ID,
                Operation = "订单发送给芯达通json备份",
                Summary = xdtorder.Json(),
            });

            //调用芯达通接口，传递订单数据
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetReceptorOrder;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, xdtorder);
            return result;
        }

        /// <summary>
        /// 芯达通订单确认通知
        /// </summary>
        /// <param name="orderConfirmed"></param>
        /// <returns></returns>
        public static string XDTOrderConfirm(this XDTModels.OrderConfirmed orderConfirmed)
        {
            //调用芯达通接口，传递订单数据
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.OrderConfirm;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, orderConfirmed);
            return result;
        }

        /// <summary>
        /// 芯达通付汇申请
        /// </summary>
        /// <param name="unPayExchangeOrder"></param>
        /// <returns></returns>
        public static string XDTPayExchange(this XDTModels.PayExchangeApply unPayExchangeOrder)
        {
            //调用芯达通接口，传递订单数据
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.PayExchange;
            var response = HttpPostRaw(apiurl, unPayExchangeOrder.Json());
            return response;
        }

        /// <summary>
        /// 芯达通付汇申请删除接口
        /// </summary>
        /// <returns></returns>
        public static string XDTDeletePayExchange(this XDTClientView.UserPayExchangeApply apply, string userID)
        {
            //调用芯达通接口，传递订单数据
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.DeletePayExchange;
            var url = apiurl + "?ID=" + apply.ID + "&UserID=" + userID; //apply.UserID;

            var response = CommonHttpRequest(url, "DELETE");

            return response;
        }
        #endregion

        #region 管理端接口

        /// <summary>
        /// 入库通知
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string CgEntryNotice(this Order order)
        {
            #region 查询出需要的订单数据
            var items = order.Orderitems.Where(item => item.Type == Enums.OrderItemType.Normal);
            //查询出订单项数据
            var orderitems = from item in items
                             where item.OrderID == order.ID
                             select new OrderItem
                             {
                                 ID = item.ID,
                                 OrderID = item.OrderID,
                                 InputID = item.InputID,
                                 ProductID = item.ProductID,
                                 TinyOrderID = item.TinyOrderID,
                                 Origin = item.Origin,
                                 DateCode = item.DateCode,
                                 Quantity = item.Quantity,
                                 Currency = (Currency)item.Currency,
                                 UnitPrice = item.UnitPrice,
                                 Unit = item.Unit,
                                 TotalPrice = item.TotalPrice,
                                 CreateDate = item.CreateDate,
                                 ModifyDate = item.ModifyDate,
                                 GrossWeight = item.GrossWeight,
                                 Volume = item.Volume,
                                 Conditions = item.Conditions,
                                 Status = item.Status,
                                 IsAuto = item.IsAuto,
                                 WayBillID = item.WayBillID,
                                 Product = item.Product,
                                 CustomName = item.CustomName,
                             };
            #endregion

            #region 拼凑入库通知对象
            CgNoticeSource noticeSource = CgNoticeSource.AgentEnter;
            if (order.Type == OrderType.Transport)
            {
                noticeSource = CgNoticeSource.Transfer;
            }
            if (order.Type == OrderType.Declare)
            {
                noticeSource = CgNoticeSource.AgentBreakCustoms;
            }
            var Waybill = new
            {
                WaybillID = order.OrderInput.Waybill.ID,
                Supplier = order.OrderInput.Waybill.Supplier,
                Type = order.OrderInput.Waybill.Type,
                OrderID = order.ID,
                Source = noticeSource,
                NoticeType = CgNoticeType.Enter,
                CheckRequirement = new
                {
                    //order.OrderInput.IsPayCharge,
                    IsReciveCharge = order.OrderOutput == null ? false : order.OrderOutput.IsReciveCharge,
                }

            };
            var Notices = orderitems.ToArray().Select(item => new
            {
                Type = CgNoticeType.Enter,
                WareHouseID = ConfigurationManager.AppSettings["HKWLTID"],
                WaybillID = order.OrderInput.Waybill.ID,
                item.InputID,
                item.OutputID,
                item.DateCode,
                item.Quantity,
                Origin = item.OriginGetCode,
                CustomsName = item.CustomName,
                Source = noticeSource,
                Target = NoticesTarget.Default,
                Weight = item.GrossWeight,
                item.Volume,
                Conditions = new
                {
                    DevanningCheck = order.OrderInput.Waybill.WayCondition.UnBoxed,//是否拆箱验货
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                }.Json(),
                Product = new
                {
                    item.Product.PartNumber,
                    item.Product.Manufacturer,
                    item.Product.PackageCase,
                    item.Product.Packaging,
                },
                Input = new
                {
                    ID = item.InputID,
                    Code = item.InputID,
                    OrderID = item.OrderID,
                    item.TinyOrderID,
                    ItemID = item.ID,
                    ClientID = order.ClientID,
                    PayeeID = order.PayeeID,
                    ThirdID = order.ClientID,
                    Currency = (Currency)item.Currency,
                    UnitPrice = item.UnitPrice,
                },
            });

            var data = new { Waybill, Notices };
            var dataStr = data.Json();
            #endregion

            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
            return result;
        }

        /// <summary>
        /// 生成出库通知(代发货)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string CgOutNotice(this Models.Order order)
        {
            var Waybill = new
            {
                WaybillID = order.OrderOutput.Waybill.ID,
                Summary = order.OrderOutput.Waybill.Summary,
                Type = order.OrderOutput.Waybill.Type,
            };
            var Notices = order.Orderitems.Select(item => new
            {
                Type = CgNoticeType.Out,
                WareHouseID = ConfigurationManager.AppSettings["HKWLTID"],
                WaybillID = order.OrderOutput.Waybill.ID,
                InputID = item.InputID,
                OutputID = item.OutputID,
                ProductID = item.Product.ID,
                DateCode = item.DateCode,
                Quantity = item.Quantity,
                Source = (int)CgNoticeSource.AgentSend,
                Target = (int)NoticesTarget.Default,
                Weight = (decimal)item.GrossWeight,
                Volume = (decimal)item.Volume,
                StorageID = item.StorageID,
                Origin = item.Origin,
                Conditions = new NoticeCondition()
                {
                    DevanningCheck = order.OrderOutput.Waybill.WayCondition.UnBoxed,//是否拆箱验货 
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                }.Json(),
                Output = new Output()
                {
                    ID = item.OutputID,
                    InputID = item.InputID,
                    OrderID = item.OrderID,
                    TinyOrderID = item.TinyOrderID,
                    ItemID = item.ID,
                    Currency = item.Currency,
                    Price = item.UnitPrice,
                    TrackerID = order.CreatorID,
                    OwnerID = order.ClientID,
                },
            }).ToArray();

            //组成出库通知对象
            var picking = new { Waybill, Notices };
            var pickStr = picking.Json();
            order.OperateLog("香港发香港出库单", pickStr);

            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgOutNotice;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, picking);

            order.OperateLog("库房出库通知接口", result.ToString());
            return result;
        }

        /// <summary>
        /// 取消出库通知
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public static string CancelOutNotice(this Models.Order order)
        {
            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName]
                + apisetting.CgCancelOutNotice + "?WaybillID=" + order.OrderOutput.WayBillID;

            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl);

            return result;
        }

        public static string OrderToXDT(this Models.Order orderParam, bool isReturned = false)
        {
            //先查询一下
            var order = new Views.OrderAlls().Where(item => item.ID == orderParam.ID).FirstOrDefault();

            //构造参数
            var submitData = new XDTModels.ReceptorOrder();
            submitData.ID = order.ID;
            submitData.Type = XDTModels.OrderType.Outside;
            submitData.AdminID = orderParam.OperatorID;
            submitData.ClientName = order.OrderClient.Name;
            submitData.ClientCode = order.OrderClient.EnterCode;
            submitData.Currency = order.OrderInput.Currency.ToString();
            submitData.IsFullVehicle = order.OrderOutput.OrderCondition.IsCharterBus;
            submitData.IsLoan = order.OrderInput.IsPayCharge ?? false;
            submitData.PackNo = order.OrderInput.Waybill.TotalParts;
            submitData.WarpType = order.OrderInput.Waybill.Packaging ?? "22";
            submitData.InvoiceStatus = order.InvoiceStatus;
            submitData.PaidExchangeAmount = 0;
            submitData.IsHangUp = false;
            submitData.IsReturned = isReturned;
            submitData.Status = XDTModels.Status.Normal;
            submitData.OrderBillType = XDTModels.OrderBillType.Normal;
            submitData.CreateDate = DateTime.Now;
            submitData.UpdateDate = DateTime.Now;
            submitData.Summary = order.Summary;

            submitData.Items = orderParam.Orderitems.Select(t => new XDTModels.OrderItems
            {
                ID = t.ID,
                TinyOrderID = t.TinyOrderID,
                PreProductID = t.ProductID,
                Origin = t.OriginGetCode,
                Quantity = t.Quantity,
                Unit = t.Unit.GetUnit().Code,
                UnitPrice = t.UnitPrice,
                TotalPrice = t.UnitPrice * t.Quantity,
                GrossWeight = t.GrossWeight,
                IsSampllingCheck = false,
                ClassifyStatus = XDTModels.ClassifyStatus.Unclassified,
                ProductUniqueCode = t.ProductUniqueCode ?? "",
                Status = 0,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
                Summary = null,
                Name = t.CustomName,
                Model = t.Product.PartNumber,
                Manufacturer = t.Product.Manufacturer,
                Batch = t.DateCode,
            }).ToList();

            //香港交货人
            submitData.OrderConsignee = new XDTModels.OrderConsignee()
            {
                ClientSupplierName = order.OrderSupplier.EnglishName,
                Type = XDTModels.HKDeliveryType.SentToHKWarehouse,
                WayBillNo = order.OrderInput.Waybill.Code,
            };
            if (order.OrderInput.Waybill.Type == WaybillType.PickUp)
            {
                submitData.OrderConsignee.Type = XDTModels.HKDeliveryType.PickUp;
                submitData.OrderConsignee.Contact = order.OrderInput.Waybill.WayLoading.TakingContact;
                submitData.OrderConsignee.PickUpTime = order.OrderInput.Waybill.WayLoading.TakingDate;
                submitData.OrderConsignee.Address = order.OrderInput.Waybill.WayLoading.TakingAddress;
                submitData.OrderConsignee.Mobile = order.OrderInput.Waybill.WayLoading.TakingPhone;
            }
            //深圳收货人
            submitData.OrderConsignor = new XDTModels.OrderConsignor();
            if (order.OrderOutput.Waybill.Type == WaybillType.PickUp)
            {
                submitData.OrderConsignor.Type = XDTModels.SZDeliveryType.PickUpInStore;
            }
            if (order.OrderOutput.Waybill.Type == WaybillType.DeliveryToWarehouse)
            {
                submitData.OrderConsignor.Type = XDTModels.SZDeliveryType.SentToClient;
            }
            if (order.OrderOutput.Waybill.Type == WaybillType.LocalExpress)
            {
                submitData.OrderConsignor.Type = XDTModels.SZDeliveryType.Shipping;
            }
            submitData.OrderConsignor.Name = order.OrderOutput.Waybill.Consignee.Company;
            submitData.OrderConsignor.Contact = order.OrderOutput.Waybill.Consignee.Contact;
            submitData.OrderConsignor.Mobile = order.OrderOutput.Waybill.Consignee.Phone;
            submitData.OrderConsignor.Tel = order.OrderOutput.Waybill.Consignee.Phone;
            submitData.OrderConsignor.Address = order.OrderOutput.Waybill.Consignee.Address;
            submitData.OrderConsignor.IDType = order.OrderOutput.Waybill.Consignee.IDType == null ? "" : ((int)order.OrderOutput.Waybill.Consignee.IDType).ToString();
            submitData.OrderConsignor.IDNumber = order.OrderOutput.Waybill.Consignee.IDNumber;
            //付汇供应商查询
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                var suppliers = new Yahv.Services.Views.wsnSuppliersTopView<PvWsOrderReponsitory>()
                    .Where(item => order.PaymentSuppliers.Contains(item.ID)).ToList();
                submitData.PayExchangeSuppliers = suppliers.Select(item => new XDTModels.OrderPayExchangeSuppliers
                {
                    ClientSupplierName = item.EnglishName,
                }).ToList();
            }
            string requestData = submitData.Json();
            orderParam.OperateLog("芯达通报关订单对接接口调用", requestData);
            //调用芯达通接口，传递订单数据
            var apisetting = new WlAdminApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetReceptorOrder;
            var result = Yahv.Utils.Http.ApiHelper.Current.PostData(apiurl, submitData);
            orderParam.OperateLog("芯达通报关订单对接接口结果", result);
            return result;
        }

        public static string CgDecBoxingNotice(this Models.Order order)
        {
            var Waybill = new
            {
                WaybillID = order.OrderOutput.Waybill.ID,
                Supplier = order.OrderOutput.Waybill.Supplier,
                Type = order.OrderOutput.Waybill.Type,
                NoticeType = CgNoticeType.Boxing,
                Source = CgNoticeSource.AgentCustomsFromStorage,
                AdminID = order.CreatorID,
            };
            var Notices = order.Orderitems.Select(item => new
            {
                WareHouseID = "HK02",//TODO：香港库房
                item.StorageID,
                item.Quantity,
                Weight = item.GrossWeight,
                item.Volume,
                Conditions = new
                {
                    Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                    CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                    OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                    AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                    PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                    Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                    IsCCC = item.OrderItemsTerm.Ccc, //CCC
                    IsCIQ = item.OrderItemsTerm.CIQ, //商检
                    IsEmbargo = item.OrderItemsTerm.Embargo, //禁运
                    IsHighPrice = item.OrderItemsTerm.IsHighPrice, //高价值
                }.Json(),
                Output = new
                {
                    ID = item.OutputID,
                    InputID = item.InputID,
                    item.OrderID,
                    item.TinyOrderID,
                    OwnerID = order.ClientID,
                    ItemID = item.ID,
                    Currency = item.Currency,
                    Price = item.UnitPrice,
                    TrackerID = order.CreatorID,
                },
            });
            //入库通知结构
            var data = new
            {
                Waybill = Waybill,
                Enter = new { Notices = Notices },
                Delete = new
                {
                    ItemID = order.Orderitems
                    .Where(item => item.Status == Enums.OrderItemStatus.ConfirmDelete)
                    .Select(item => item.ID).ToArray(),
                },
            };
            var dataStr = data.Json();
            //调用库房接口
            var apisetting = new PvWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgBoxingNotice;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);

            return result;
        }

        #endregion

        #region 辅助方法
        public static string HttpPostRaw(string url, string data, string contentType = "application/json")
        {
            string value = "";
            try
            {
                HttpWebRequest reqest = (HttpWebRequest)WebRequest.Create(url);
                reqest.Method = "POST";
                reqest.ContentType = contentType;
                Stream stream = reqest.GetRequestStream();
                byte[] bs = System.Text.Encoding.UTF8.GetBytes(data);
                stream.Write(bs, 0, bs.Length);
                stream.Flush();
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)reqest.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                value = sr.ReadToEnd();
                response.Close();
                return value;
            }
            catch (Exception ex)
            {
                Logger.log("HttpPostRaw", new OperatingLog
                {
                    MainID = url,
                    Operation = data,
                    Summary = ex.Message,
                });
                return "";
            }
        }

        /// <summary>
        /// 通用请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string CommonHttpRequest(string url, string type, string data = "")
        {
            HttpWebRequest myRequest = null;
            Stream outstream = null;
            HttpWebResponse myResponse = null;
            StreamReader reader = null;
            try
            {
                //构造http请求的对象
                myRequest = (HttpWebRequest)WebRequest.Create(url);


                //设置
                myRequest.ProtocolVersion = HttpVersion.Version10;
                myRequest.Method = type;

                if (data.Trim() != "")
                {
                    myRequest.ContentType = "text/xml";
                    myRequest.ContentLength = data.Length;
                    myRequest.Headers.Add("data", data);

                    //转成网络流
                    byte[] buf = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(data);

                    outstream = myRequest.GetRequestStream();
                    outstream.Flush();
                    outstream.Write(buf, 0, buf.Length);
                    outstream.Flush();
                    outstream.Close();
                }
                // 获得接口返回值
                myResponse = (HttpWebResponse)myRequest.GetResponse();
                reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string ReturnXml = reader.ReadToEnd();
                reader.Close();
                myResponse.Close();
                myRequest.Abort();
                return ReturnXml;
            }
            catch (Exception)
            {
                if (outstream != null) outstream.Close();
                if (reader != null) reader.Close();
                if (myResponse != null) myResponse.Close();
                if (myRequest != null) myRequest.Abort();
                return "";
            }
        }
        #endregion
    }
}
