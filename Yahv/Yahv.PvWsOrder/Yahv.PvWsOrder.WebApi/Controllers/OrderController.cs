using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.ClientViews;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Web.Mvc;
using Layers.Data.Sqls;
using Yahv.Utils.Serializers;
using System.Threading.Tasks;
using Yahv.Utils.Converters.Contents;
using System.Configuration;
using Yahv.PvWsOrder.Services.Extends;
using Newtonsoft.Json.Linq;

namespace Yahv.PvWsOrder.WebApi.Controllers
{
    public class OrderController : Controller
    {
        /// <summary>
        /// 内单订单对接接口
        /// </summary>
        /// <param name="confirm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrderSubmit(JPost obj)
        {
            try
            {
                var order = obj.ToObject<Models.PvWsOrderInsApiModel>();
                DeclareOrder orderExtends = new DeclareOrder();
                using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
                {
                    //基础信息获取
                    var client = new WsClientsTopView<PvWsOrderReponsitory>(reponsitory).FirstOrDefault(item => item.Name == order.ClientName);
                    if (client == null)
                    {
                        throw new Exception("该客户不存在！");
                    }
                    //平台公司数据
                    var company = new Services.Views.CompanysAll(reponsitory).SingleOrDefault(item => item.Name == order.DeclarationCompany);
                    var companyContact = company.Contacts.FirstOrDefault();  //公司联系人
                    var invoice = new MyInvoicesView(client.ID).SingleOrDefault(); //客户发票
                    var supplierview = new wsnSuppliersTopView<PvWsOrderReponsitory>(reponsitory); //客户供应商

                    orderExtends.ID = order.VastOrderID;
                    orderExtends.ClientID = client.ID;
                    orderExtends.Type = OrderType.Declare;
                    orderExtends.PayeeID = company.ID;
                    orderExtends.CreatorID = order.AdminID;
                    orderExtends.MainStatus = CgOrderStatus.待审核;
                    orderExtends.InvoiceID = invoice.ID;
                    orderExtends.SupplierID = supplierview.SingleOrDefault(item => item.OwnID == client.ID && item.RealEnterpriseName == order.OrderConsignee.ClientSupplierName)?.ID;
                    orderExtends.EnterCode = client.EnterCode;
                    orderExtends.SettlementCurrency = Currency.CNY;
                    //获取
                    orderExtends.PayExchangeSuppliers = supplierview.Where(item => order.PayExchangeSuppliers.Contains(item.RealEnterpriseName))
                        .Select(item => item.ID).ToArray();

                    #region 运单数据保存
                    //入库数据
                    orderExtends.InWaybill = new Waybill()
                    {
                        Code = order.OrderConsignee.WayBillNo,
                        Type = order.OrderConsignee.Type == 1 ? WaybillType.DeliveryToWarehouse : WaybillType.PickUp,
                        FreightPayer = order.OrderConsignee.Type == 1 ? WaybillPayer.Consignor : WaybillPayer.Consignee,
                        CreatorID = order.AdminID,
                        ModifierID = order.AdminID,
                        Supplier = order.OrderConsignee.ClientSupplierName,
                        EnterCode = client.EnterCode,
                        ExcuteStatus = (int)CgSortingExcuteStatus.Sorting,
                        Consignee = new Yahv.Services.Models.WayParter
                        {
                            Company = company.Name,
                            Address = company.RegAddress,
                            Contact = companyContact?.Name,
                            Phone = companyContact?.Tel ?? companyContact?.Mobile,
                            Email = companyContact?.Email,
                            Place = Origin.HKG.ToString(),
                        },
                        Consignor = new Yahv.Services.Models.WayParter
                        {
                            Company = order.OrderConsignee.ClientSupplierName,
                        },
                        OrderID = order.VastOrderID,
                        NoticeType = Yahv.Services.Enums.CgNoticeType.Enter,
                        Source = Yahv.Services.Enums.CgNoticeSource.AgentBreakCustomsForIns,
                    };
                    if (orderExtends.InWaybill.Type == WaybillType.PickUp)
                    {
                        orderExtends.InWaybill.WayLoading = new Yahv.Services.Models.WayLoading()
                        {
                            TakingAddress = order.OrderConsignee.Address,
                            TakingContact = order.OrderConsignee.Contact,
                            TakingDate = order.OrderConsignee.PickUpTime,
                            TakingPhone = order.OrderConsignee.Mobile ?? order.OrderConsignee.Tel,
                            CreatorID = order.AdminID,
                            ModifierID = order.AdminID,
                        };
                    }

                    //深圳出库数据
                    orderExtends.OutWaybill = new Waybill()
                    {
                        Type = (WaybillType)order.OrderConsignor.Type,
                        FreightPayer = order.OrderConsignor.Type == 1 ? WaybillPayer.Consignee : WaybillPayer.Consignor,
                        CreatorID = order.AdminID,
                        ModifierID = order.AdminID,
                        EnterCode = client.EnterCode,
                        ExcuteStatus = (int)CgPickingExcuteStatus.Picking,
                        Status = GeneralStatus.Closed,
                        Consignee = new Yahv.Services.Models.WayParter()
                        {
                            Company = order.OrderConsignor.Name,
                            Contact = order.OrderConsignor.Contact,
                            Phone = order.OrderConsignor.Mobile ?? order.OrderConsignor.Tel,
                            Address = order.OrderConsignor.Address,
                            Place = Origin.CHN.ToString(),
                        },
                        Consignor = new Yahv.Services.Models.WayParter()
                        {
                            Company = company.Name,
                            Address = company.RegAddress,
                            Contact = companyContact?.Name,
                            Phone = companyContact?.Tel ?? companyContact?.Mobile,
                            Email = companyContact?.Email,
                            Place = Origin.CHN.ToString(),
                        },
                        OrderID = order.VastOrderID,
                        NoticeType = Yahv.Services.Enums.CgNoticeType.Out,
                        Source = Yahv.Services.Enums.CgNoticeSource.AgentBreakCustomsForIns,
                    };
                    #endregion

                    orderExtends.OrderItems = order.Items.Select(item => new OrderItem
                    {
                        ID = item.ID,
                        Type = Services.Enums.OrderItemType.Normal,
                        TinyOrderID = item.TinyOrderID,
                        OrderID = order.VastOrderID,
                        Name = item.ProductName,
                        Product = new Yahv.Services.Models.CenterProduct
                        {
                            PartNumber = item.Model,
                            Manufacturer = item.Brand
                        },
                        Origin = item.Origin,
                        Unit = (LegalUnit)int.Parse(item.Unit),
                        UnitPrice = item.UnitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice,
                        Currency = (Currency)EnumHelper.GetEnumValue(typeof(Currency), item.Currency),
                        GrossWeight = item.GrossWeight,
                        IsAuto = true,
                    }).ToArray();

                    //持久化
                    var result = this.InsideOrderEnter(orderExtends, reponsitory);

                    //归类信息同步
                    this.InsideOrderClassify(order);

                    //返回归类信息
                    var json = new JMessage() { code = 200, success = true, data = orderExtends.InWaybill.ID };
                    return Json(json, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                //错误日志记录
                Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }


        #region  订单退回
        /// <summary>
        /// 芯达通订单退回对接接口
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult OrderReturn(JPost obj)
        {
            try
            {
                var VastOrderID = obj["VastOrderID"].Value<string>();
                var tinyorderid = obj["TinyOrderID"].Value<string>();
                var adminid = obj["AdminID"].Value<string>();

                //传输日志记录
                Yahv.PvWsOrder.Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = VastOrderID,
                    Operation = "芯达通跟单退回订单对接报文",
                    Summary = obj.Json(),
                });

                using (var reponsitory = new PvCenterReponsitory(false))
                using (var res = new PvWsOrderReponsitory(false))
                {
                    //大订单退回还是单独小订单退回
                    var IsTinyReturned = res.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Any(item => item.OrderID == VastOrderID
                    && item.TinyOrderID != tinyorderid);

                    var status = IsTinyReturned ? CgOrderStatus.退回 : CgOrderStatus.暂存;
                    //变更订单状态为退回
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false
                    }, item => item.MainID == VastOrderID && item.Type == (int)OrderStatusType.MainStatus);
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = VastOrderID,
                        CreatorID = adminid,
                        Type = (int)OrderStatusType.MainStatus,
                        CreateDate = DateTime.Now,
                        Status = (int)status,
                        IsCurrent = true
                    });

                    //到货信息删除
                    res.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        Status = (int)Services.Enums.OrderItemStatus.Deleted,
                    }, item => item.TinyOrderID == tinyorderid && item.Type == 2);

                    if (IsTinyReturned)
                    {
                        //变更订单项状态
                        res.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            Status = (int)Services.Enums.OrderItemStatus.Returned, //报关退回的数据
                        }, item => item.TinyOrderID == tinyorderid && item.Status != (int)Services.Enums.OrderItemStatus.Deleted);
                    }
                    else
                    {
                        //更新运单状态为退回
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.Waybills>(new
                        {
                            Status = 600,
                        }, item => item.OrderID == VastOrderID);

                        //文件删除
                        reponsitory.Delete<Layers.Data.Sqls.PvCenter.FilesDescription>(item => item.WsOrderID == VastOrderID);
                    }
                    res.Submit();
                    reponsitory.Submit();
                }
                //返回归类信息
                var json = new JMessage() { code = 200, success = true, data = "操作成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //错误日志记录
                Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region  开票完成
        /// <summary>
        /// 开票完成
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult InvoiceComplete(JPost obj)
        {
            try
            {
                var VastOrderID = obj["VastOrderID"].Value<string>();
                var adminid = obj["AdminID"].Value<string>();

                //传输日志记录
                Services.Logger.log("NPC", new Yahv.Services.Models.OperatingLog
                {
                    MainID = VastOrderID,
                    Operation = "开票完成",
                    Summary = obj.Json(),
                });

                var orderIds = VastOrderID.Split(',');
                using (var reponsitory = new PvCenterReponsitory(false))
                {
                    foreach (var id in orderIds)
                    {
                        //变更订单状态为退回
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                        {
                            IsCurrent = false
                        }, item => item.MainID == id && item.Type == (int)OrderStatusType.InvoiceStatus);

                        reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                        {
                            ID = Guid.NewGuid().ToString(),
                            MainID = id,
                            CreatorID = adminid,
                            Type = (int)OrderStatusType.InvoiceStatus,
                            CreateDate = DateTime.Now,
                            Status = (int)OrderInvoiceStatus.Invoiced,
                            IsCurrent = true
                        });
                        reponsitory.Submit();
                    }
                }
                //返回归类信息
                var json = new JMessage() { code = 200, success = true, data = "操作成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //错误日志记录
                Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion


        /// <summary>
        /// 报关订单发入库通知（临时用）
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderToEnterNotice(string orderID)
        {
            try
            {
                var json = new JMessage() { code = 200, success = true, data = "" };
                //变更订单项状态
                using (var res = new Layers.Data.Sqls.PvWsOrderReponsitory())
                {
                    var order = new Yahv.PvWsOrder.Services.Views.OrderAlls().Single(item => item.ID == orderID);
                    json.data = order.CgEntryNotice();
                }
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 报关订单发入库通知（临时用）
        /// </summary>
        /// <returns></returns>
        public ActionResult OrderToBoxNotice(string orderID)
        {
            try
            {
                var json = new JMessage() { code = 200, success = true, data = "" };
                //变更订单项状态
                using (var res = new Layers.Data.Sqls.PvWsOrderReponsitory())
                {
                    var order = new Yahv.PvWsOrder.Services.Views.OrderAlls().Single(item => item.ID == orderID);
                    json.data = order.CgDecBoxingNotice();
                }
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }


        #region 帮助方法
        /// <summary>
        /// 数据持久化
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reponsitory"></param>
        /// <returns></returns>
        private DeclareOrder InsideOrderEnter(DeclareOrder order, PvWsOrderReponsitory reponsitory)
        {
            //运单数据插入
            Task task = new Task(() =>
            {
                order.InWaybill.ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                order.OutWaybill.ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill);
                order.InWaybill.Enter();
                order.OutWaybill.Enter();
            });
            task.Start();

            #region 订单主体数据持久化
            //订单主表数据插入
            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Orders()
            {
                ID = order.ID,
                Type = (int)order.Type,
                ClientID = order.ClientID,
                InvoiceID = order.InvoiceID,
                PayeeID = order.PayeeID,
                BeneficiaryID = order.BeneficiaryID,
                CreateDate = DateTime.Now,
                ModifyDate = DateTime.Now,
                Summary = order.Summary,
                SupplierID = order.SupplierID,
                CreatorID = order.CreatorID,
                SettlementCurrency = (int?)order.SettlementCurrency,
                TotalPrice = order.OrderItems.Sum(item => item.TotalPrice),
            });

            //付汇供应商插入
            if (order.PayExchangeSuppliers.Count() > 0)
            {
                reponsitory.Insert(order.PayExchangeSuppliers.Select(item => new Layers.Data.Sqls.PvWsOrder.MapsSupplier
                {
                    OrderID = order.ID,
                    SupplierID = item,
                }).ToArray());
            }

            //产品数据录入
            var products = order.OrderItems.Select(item => item.Product);
            foreach (var product in products)
            {
                Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(product);
            }

            //订单项数据录入
            var linq = order.OrderItems.Select(item => new Layers.Data.Sqls.PvWsOrder.OrderItems
            {
                ID = item.ID,
                OrderID = item.OrderID,
                InputID = item.ID,
                Type = (int)item.Type,
                TinyOrderID = item.TinyOrderID,
                ProductID = string.Concat(item.Product.PartNumber, item.Product.Manufacturer).MD5(),
                CustomName = item.Name,
                DateCode = item.DateCode,
                Origin = item.Origin,
                Unit = (int)item.Unit,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                TotalPrice = item.TotalPrice,
                Currency = (int)item.Currency,
                GrossWeight = item.GrossWeight,
                CreateDate = item.CreateDate,
                ModifyDate = item.ModifyDate,
                IsAuto = true,
                Status = (int)item.Status,
            }).ToArray();
            reponsitory.Insert(linq);
            #endregion

            task.Wait();

            //订单拓展表数据持久化
            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderInputs()
            {
                ID = order.ID,
                WayBillID = order.InWaybill.ID,
                Currency = (int)order.OrderItems.FirstOrDefault().Currency,
            });
            reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.OrderOutputs()
            {
                ID = order.ID,
                WayBillID = order.OutWaybill.ID,
                Currency = (int)order.OrderItems.FirstOrDefault().Currency,
            });

            #region 状态插入
            //中心库
            using (PvCenterReponsitory centerReponsitory = new PvCenterReponsitory())
            {
                #region 主状态
                centerReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.MainStatus,
                    Status = (int)order.MainStatus,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
                #endregion

                #region 支付状态
                centerReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.PaymentStatus,
                    Status = (int)OrderPaymentStatus.Waiting,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
                #endregion

                #region 开票状态
                centerReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.InvoiceStatus,
                    Status = (int)OrderInvoiceStatus.UnInvoiced,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
                #endregion

                #region 付汇状态
                centerReponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                {
                    ID = Guid.NewGuid().ToString(),
                    MainID = order.ID,
                    Type = (int)OrderStatusType.RemittanceStatus,
                    Status = (int)OrderRemittanceStatus.UnRemittance,
                    CreateDate = DateTime.Now,
                    CreatorID = order.CreatorID,
                    IsCurrent = true,
                });
                #endregion
            }
            #endregion

            return order;
        }

        private void InsideOrderClassify(Models.PvWsOrderInsApiModel order)
        {
            var data = order.Items.Where(item => item.ClassifyStatus == Services.XDTModels.ClassifyStatus.Done).Select(item => new
            {
                ItemID = item.ID,
                PartNumber = item.Model,
                Manufacturer = item.Brand,
                HSCode = item.CustomsCode,
                TariffName = item.ProductName,
                item.TaxCode,
                item.TaxName,
                LegalUnit1 = item.FirstLegalUnit,
                LegalUnit2 = item.SecondLegalUnit,
                VATRate = item.ValueAddedRate,
                ImportPreferentialTaxRate = item.TariffRate,
                OriginRate = 0m,
                ExciseTaxRate = item.ExciseTaxRate.GetValueOrDefault(),
                item.CIQCode,
                item.Elements,
                Ccc = item.CCC,
                item.Embargo,
                item.HkControl,
                item.Coo,
                item.CIQ,
                item.CIQprice,
                item.IsHighPrice,
                item.IsDisinfected,
                CreatorID = item.ClassifySecondAdminID,
                CreateDate = item.ClassifyTime,
                UpdateDate = item.ClassifyTime,
            });
            var pvdataapi = new Yahv.PvWsOrder.Services.PvDataApiSetting();
            var url = ConfigurationManager.AppSettings[pvdataapi.ApiName] + pvdataapi.SyncClassified;
            var result = Yahv.Utils.Http.ApiHelper.Current.JPost<JMessage>(url, new
            {
                results = data.ToList()
            });
        }
        #endregion

        #region 报关订单客户确认

        public ActionResult DeclareConfirm(JPost obj)
        {
            try
            {
                var VastOrderID = obj["VastOrderID"].Value<string>();

                using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
                {
                    var orderalls = new Yahv.PvWsOrder.Services.ClientViews.OrderAlls(reponsitory);
                    var order = orderalls.GetDecNoticeDataByOrderID(VastOrderID);
                    orderalls.Confirm(order, Npc.Robot.Obtain());
                }

                var json = new JMessage() { code = 200, success = true, data = "操作成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                //错误日志记录
                Yahv.PvWsOrder.Services.Logger.Error("NPC", ex.Message);
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        /// <summary>
        /// 更新EnterCode
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public ActionResult UpdateEnterCode(JPost obj)
        {
            try
            {
                var VastOrderID = obj["VastOrderID"].Value<string>();
                var EnterCode = obj["EnterCode"].Value<string>();

                using (var res = new PvWsOrderReponsitory(false))
                {
                    res.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                    {
                        EnterCode = EnterCode,
                        ModifyDate = DateTime.Now
                    }, item => item.ID == VastOrderID );
                    res.Submit();
                }

                var json = new JMessage() { code = 200, success = true, data = "操作成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 更新订单状态到“已装箱”
        /// </summary>
        [HttpPost]
        public ActionResult UpdateOrderStatusToBoxed(JPost obj)
        {
            try
            {
                var MainID = obj["MainID"].Value<string>();
                var AdminID = obj["AdminID"].Value<string>();


                using (var reponsitory = new PvCenterReponsitory())
                {
                    reponsitory.Update<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>(new
                    {
                        IsCurrent = false,
                    }, item => item.Type == 1 && item.MainID == MainID);

                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = MainID,
                        Type = 1, //订单主状态，  
                        Status = (int)CgOrderStatus.已装箱,
                        CreateDate = DateTime.Now,
                        CreatorID = AdminID,
                        IsCurrent = true,
                    });
                }

                var json = new JMessage() { code = 200, success = true, data = "操作成功" };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var json = new JMessage() { code = 300, success = false, data = ex.Message };
                return Json(json, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
