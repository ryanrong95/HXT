using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ConsoleApp.vTaskers.Services;
using Layers.Data;
using Wms.Services.Models;
using Wms.Services.Views;
using Yahv.Payments;
using Yahv.Services;
using Yahv.Services.Enums;
using Yahv.Underly;
using Yahv.Web.Mvc;
using Layers.Data.Sqls;
using Layers.Linq;
using Newtonsoft.Json;
using Wms.Services;
using Wms.Services.chonggous.Views;
using Yahv.Linq.Extends;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.Underly.Attributes;
using Carrier = Yahv.Services.Models.Carrier;

namespace MvcApi.Controllers
{
    public class PaymentsController : Controller
    {
        // GET: Payments
        /// <summary>
        /// 
        /// </summary>
        /// <param name="waybillid"></param>
        /// <param name="type">type:in 收入 ，out 支出</param>
        /// <param name="otype">type:in 入库 ，out 出库</param>
        /// <param name="isCash">是否为现金记账</param>
        /// <returns></returns>
        public ActionResult Index(string waybillid, string type, string otype)
        {
            var roll = new Wms.Services.Views.PaymentsWayBillRoll().SearchByWaybillD(waybillid).ToFillData(type, otype);
            var result = (PaymentWaybill)roll;
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 科目接口
        /// </summary>
        /// <param name="type">类型（in 收入 out 支出）</param>
        /// <param name="conduct">业务</param>
        /// <param name="isCustoms">是否报关业务</param>
        /// <returns></returns>
        public object Subject(string type, string conduct, bool isCustoms)
        {
            var ls = new List<string>();
            string catalog = isCustoms ? CatalogConsts.杂费 : CatalogConsts.仓储服务费;
            var view = new SubjectsView().Where(item => item.Catalog == catalog);

            if (type == "in")
            {
                ls.AddRange(view.Where(item => (item.Conduct == conduct || item.Conduct == "供应链") && item.Type == SubjectType.Input).Select(item => item.Name));
            }
            if (type == "out")
            {
                ls.AddRange(view.Where(item => (item.Conduct == conduct || item.Conduct == "供应链") && item.Type == SubjectType.Output).Select(item => item.Name));
            }

            var list = new List<Subject>();
            var subjects = GetFixedSubjects();

            foreach (var item in ls)
            {
                if (!subjects.Contains(item))
                {
                    continue;
                }

                var r = PaymentTools.Receivables[conduct, catalog, item];

                var value = r?.Quotes;

                if (value != null)
                {
                    list.Add(new Subject { Name = item, PayQuote = new SubjectPayQuote { Currency = value.Currency, Price = value.Price }, SubjectName = r.Subject, Catalog = catalog });
                }
                else
                {
                    list.Add(new Subject { Name = item, PayQuote = new SubjectPayQuote { Currency = null, Price = null }, SubjectName = r.Subject, Catalog = catalog });
                }
            }


            return Json(list.OrderBy(item => item.Name), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取固定科目
        /// </summary>
        /// <returns></returns>
        private string[] GetFixedSubjects()
        {
            return new string[]
            {
                "入仓费",
                "仓储费",
                "收货异常费用",
                "处理标签费",
                "包装费",
                "快递运费",
                "提货费",
                "登记费",
                "隧道费",
                "车场费",
                "超重费",
                "包车费",
            };
        }

        [HttpPost]
        public ActionResult HKEnter(JPost fee)
        {
            try
            {
                var model = fee.ToObject<Fee>();
                Yahv.Erp.Current.WareHourse.FeeEnter(model);

                // 香港库房的应收，应付费用收取
                // 香港库房重构的库房收入，支出费用，不通过后台任务处理

                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;

                return Json(new JMessage { success = false, code = 404, data = $"保存失败!{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Enter(JPost fee)
        {
            try
            {
                var model = fee.ToObject<Fee>();
                Yahv.Erp.Current.WareHourse.FeeEnter(model);

                InsertTaskPool(model.OrderID);      //报关出库后，新增的费用也需要同步给芯达通

                return Json(new JMessage { success = true, code = 200, data = "保存成功!" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                var mes = ex.Message;

                return Json(new JMessage { success = false, code = 404, data = $"保存失败!{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// 收入记录
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IncomeRecords(string orderId)
        {
            var json = new JSingle<PaymentRecord>()
            {
                code = 200,
                success = true
            };

            try
            {
                using (var vouchers = new VouchersStatisticsView())
                using (var enterprisesView = new EnterprisesTopView())
                {
                    var query = vouchers.Where(item => item.OrderID == orderId && (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费)).ToArray();
                    var enterprises = enterprisesView.Where(item => query.Select(q => q.Payer).Contains(item.ID)).ToArray();

                    json.data = new PaymentRecord()
                    {
                        Records = query?.Where(item => item.RightDate != item.LeftDate)     //非现金
                        .Where(item => item.Subject != SubjectConsts.仓储费)
                        .Join(enterprises, q => q.Payer, e => e.ID, (item, ep) => new PaymentRecord.RecordModel()
                        {
                            Catalog = item.Catalog,
                            Conduct = item.Business,
                            Subject = item.Subject,
                            RecordPrice = $"{item.OriginCurrency.GetCurrency().Symbol} {item.OriginPrice.ToString("F2")}",
                            SettlePrice = $"{item.Currency.GetCurrency().Symbol} {item.LeftPrice.ToString("F2")}",

                            CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Creator = item.AdminName,
                            TrackingNumber = item.TrackingNumber,
                            TargetName = ep.Name,
                            Descirption = GetFormatDescription(item.Data),
                            Quantity = item.Quantity,
                        }).ToArray(),
                        Currents = query?.Where(item => item.RightDate == item.LeftDate)        //现金
                        .Join(enterprises, q => q.Payer, ep => ep.ID, (item, ep) => new PaymentRecord.CurrentModel()
                        {
                            Conduct = item.Business,
                            Catalog = item.Catalog,
                            Subject = item.Subject,
                            Price = $"{item.Currency.GetCurrency().Symbol} {(item?.RightPrice ?? 0).ToString("F2")}",

                            CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Creator = item.AdminName,
                            TrackingNumber = item.TrackingNumber,
                            TargetName = ep.Name,
                            Descirption = GetFormatDescription(item.Data),
                            Quantity = item.Quantity,
                        }).ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 收入，记账 ---代仓储 仓储费， 代报关 仓储费
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IncomeRecordsForWarehouseFee(string orderId)
        {
            var json = new JSingle<PaymentRecord>()
            {
                code = 200,
                success = true
            };

            try
            {
                using (var vouchers = new VouchersStatisticsView())
                using (var enterprisesView = new EnterprisesTopView())
                {
                    var query = vouchers.Where(item => item.OrderID == orderId && (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费)).ToArray();
                    var enterprises = enterprisesView.Where(item => query.Select(q => q.Payer).Contains(item.ID)).ToArray();

                    json.data = new PaymentRecord()
                    {
                        Records = query?.Where(item => item.RightDate != item.LeftDate).Where(item => item.Subject == SubjectConsts.仓储费)     //非现金
                        .Join(enterprises, q => q.Payer, e => e.ID, (item, ep) => new PaymentRecord.RecordModel()
                        {
                            Catalog = item.Catalog,
                            Conduct = item.Business,
                            Subject = item.Subject,
                            RecordPrice = $"{item.OriginCurrency.GetCurrency().Symbol} {item.OriginPrice.ToString("F2")}",
                            SettlePrice = $"{item.Currency.GetCurrency().Symbol} {item.LeftPrice.ToString("F2")}",

                            CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Creator = item.AdminName,
                            TrackingNumber = item.TrackingNumber,
                            TargetName = ep.Name,
                            Descirption = GetFormatDescriptionForWarehouseFee(item.Data),
                            Quantity = item.Quantity,
                        }).ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 是否记录过仓储费
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IsRecordWarehouseFee(string orderId)
        {    
            var json = new JMessage()
            {
                code = 200,
                success = true
            };

            if (string.IsNullOrEmpty(orderId))
            {
                json.code = 500;
                json.success = false;
                json.data = "参数orderId不能为空,请检查参数是否正确";
                return Json(json, JsonRequestBehavior.AllowGet);
            }

            try
            {
                using (var vouchers = new VouchersStatisticsView())
                using (var enterprisesView = new EnterprisesTopView())
                {
                    var query = vouchers.Where(item => item.OrderID == orderId && (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费)).ToArray();
                    var record = query?.Where(item => item.RightDate != item.LeftDate).FirstOrDefault(item => item.Subject == SubjectConsts.仓储费);
                    json.data = (record != null ? true : false).ToString();
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 支出记录
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OutcomeRecords(string orderId)
        {
            var json = new JSingle<PaymentRecord>()
            {
                success = true,
                code = 200,
            };

            try
            {
                using (var view = new PaymentsStatisticsView())
                using (var enterprisesView = new EnterprisesTopView())
                {
                    var query = view.Where(item => item.OrderID == orderId && (item.Catalog == CatalogConsts.杂费 || item.Catalog == CatalogConsts.仓储服务费)).ToArray();
                    var enterprises = enterprisesView.Where(item => query.Select(q => q.Payee).Contains(item.ID)).ToArray();

                    json.data = new PaymentRecord()
                    {
                        Records = query?.Where(item => item.RightDate != item.LeftDate)     //非现金
                        .Join(enterprises, q => q.Payee, e => e.ID, (item, ep) => new PaymentRecord.RecordModel()
                        {
                            Catalog = item.Catalog,
                            Conduct = item.Business,
                            Subject = item.Subject,
                            RecordPrice = $"{item.Currency.GetCurrency().Symbol} {item.LeftPrice.ToString("F2")}",
                            SettlePrice = $"{item.Currency.GetCurrency().Symbol} {item.LeftPrice.ToString("F2")}",

                            CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Creator = item.AdminName,
                            TrackingNumber = item.TrackingNumber,
                            TargetName = ep.Name,
                        }).ToArray(),
                        Currents = query?.Where(item => item.RightDate == item.LeftDate)        //现金
                        .Join(enterprises, q => q.Payee, e => e.ID, (item, ep) => new PaymentRecord.CurrentModel()
                        {
                            Conduct = item.Business,
                            Catalog = item.Catalog,
                            Subject = item.Subject,
                            Price = $"{item.Currency.GetCurrency().Symbol} {(item?.RightPrice ?? 0).ToString("F2")}",

                            CreateDate = item.LeftDate.ToString("yyyy-MM-dd HH:mm:ss"),
                            Creator = item.AdminName,
                            TrackingNumber = item.TrackingNumber,
                            TargetName = ep.Name,
                        }).ToArray()
                    };
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #region 收入合作人接口
        /// <summary>
        /// 收入合作人接口
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult IncomeParters(string orderId)
        {
            var json = new JSingle<dynamic>()
            {
                code = 200,
                success = true,
            };

            try
            {
                using (var wbView = new Wms.Services.Views.PaymentsWayBillRoll())
                using (var carriersView = new Wms.Services.Views.CarriersTopView())
                using (var orderView = new Wms.Services.Views.OrdersTopView())
                using (var epView = new EnterprisesTopView())
                {
                    var order = orderView.FirstOrDefault(item => item.ID == orderId);
                    var carriers = carriersView.OrderBy(item => item.Name).ToList();
                    carriers.Insert(0, new Carrier() { ID = AnonymousEnterprise.Current.ID, Name = AnonymousEnterprise.Current.Name });

                    json.data = new
                    {
                        //记账
                        Record = new
                        {
                            //收款人
                            Payees = new List<dynamic>()
                            {
                                new
                                {
                                    ID = order?.PayeeID,
                                    Name = epView.Single(item=>item.ID==order.PayeeID).Name,
                                }
                            },
                            //客户名称
                            Payers = new List<dynamic>()
                            {
                                new
                                {
                                    ID = order?.ClientID,
                                    Name = epView.Single(item=>item.ID==order.ClientID).Name,
                                }
                                ,new
                                {
                                     ID = AnonymousEnterprise.Current.ID,
                                    Name = AnonymousEnterprise.Current.Name,
                                }
                            },
                        },
                        //现金记账
                        Current = new
                        {
                            //收款人
                            Payees = new List<dynamic>()
                            {
                                new
                                {
                                    ID = WhPartners.OutOrder[order?.PayeeID]?.ID,
                                    Name = WhPartners.OutOrder[order?.PayeeID]?.Name,
                                }
                            },
                            //客户名称
                            Payers = carriers.Select(item => new
                            {
                                ID = item.ID,
                                Name = item.Name,
                            })
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
                json.data = ex.Message;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        #region bak

        /// <summary>
        /// 收入合作人接口
        /// </summary>
        /// <param name="waybillid">运单号</param>
        /// <param name="otype">otype:in 入库 ，out 出库</param>
        /// <returns></returns>
        //[HttpGet]
        //public ActionResult IncomeParters(string waybillid, string otype)
        //{
        //    var json = new JSingle<dynamic>()
        //    {
        //        code = 200,
        //        success = true,
        //    };

        //    try
        //    {
        //        using (var wbView = new Wms.Services.Views.PaymentsWayBillRoll())
        //        using (var carriersView = new Wms.Services.Views.CarriersTopView())
        //        {
        //            var waybill = wbView.SearchByWaybillD(waybillid).ToFillData("in", otype); ;
        //            var carriers = carriersView.OrderBy(item => item.Name).ToList();
        //            carriers.Insert(0, new Carrier() { ID = AnonymousEnterprise.Current.ID, Name = AnonymousEnterprise.Current.Name });

        //            json.data = new
        //            {
        //                //记账
        //                Record = new
        //                {
        //                    //收款人
        //                    Payees = new List<dynamic>()
        //                    {
        //                        new
        //                        {
        //                            ID = waybill?.PayeeID,
        //                            Name = waybill?.PayeeName,
        //                        }
        //                    },
        //                    //客户名称
        //                    Payers = new List<dynamic>()
        //                    {
        //                        new
        //                        {
        //                            ID = waybill?.ClientID,
        //                            Name = waybill?.ClientName,
        //                        }
        //                        ,new
        //                        {
        //                             ID = AnonymousEnterprise.Current.ID,
        //                            Name = AnonymousEnterprise.Current.Name,
        //                        }
        //                    },
        //                },
        //                //现金记账
        //                Current = new
        //                {
        //                    //收款人
        //                    Payees = new List<dynamic>()
        //                    {
        //                        new
        //                        {
        //                            ID = WhPartners.OutOrder[waybill?.PayeeID]?.ID,
        //                            Name = WhPartners.OutOrder[waybill?.PayeeID]?.Name,
        //                        }
        //                    },
        //                    //客户名称
        //                    Payers = carriers.Select(item => new
        //                    {
        //                        ID = item.ID,
        //                        Name = item.Name,
        //                    })
        //                }
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        json.code = 500;
        //        json.success = false;
        //        json.data = ex.Message;
        //    }

        //    return Json(json, JsonRequestBehavior.AllowGet);
        //}  
        #endregion
        #endregion

        #region 支出合作人接口
        /// <summary>
        /// 支出合作人接口
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult OutcomeParters(string orderId)
        {
            var json = new JSingle<dynamic>()
            {
                code = 200,
                success = true,
            };

            try
            {
                using (var wbView = new Wms.Services.Views.PaymentsWayBillRoll())
                using (var carriersView = new Wms.Services.Views.CarriersTopView())
                using (var orderView = new Wms.Services.Views.OrdersTopView())
                using (var epView = new EnterprisesTopView())
                {
                    var order = orderView.Single(item => item.ID == orderId);
                    var carries = carriersView.OrderBy(item => item.Name).Select(item => new PayInfo
                    {
                        ID = item.ID,
                        Name = item.Name,
                    });

                    var carriesIncludeAnony = carries.ToList();
                    carriesIncludeAnony.Insert(0, new PayInfo() { ID = AnonymousEnterprise.Current.ID, Name = AnonymousEnterprise.Current.Name });

                    json.data = new
                    {
                        //记账
                        Record = new
                        {
                            //承运商
                            Payees = carries,
                            //付款人
                            Payers = new List<dynamic>()
                            {
                                new
                                {
                                    ID = WhPartners.OutOrder[order.PayeeID].ID,
                                    Name = WhPartners.OutOrder[order.PayeeID].Name,
                                }
                            },
                        },
                        //现金记账
                        Current = new
                        {
                            //承运商和匿名
                            Payees = carriesIncludeAnony,
                            //付款人
                            Payers = new List<dynamic>
                            {
                                new
                                {
                                    ID = WhPartners.OutOrder[order.PayeeID].ID,
                                    Name = WhPartners.OutOrder[order.PayeeID].Name,
                                }
                            }
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                json.code = 500;
                json.success = false;
                json.data = ex.Message;
            }

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 根据订单号, 承运商ID, 获取对应的code, 应支付
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="carrierID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWaybillCodeByCarrierID(string orderID, string carrierID)
        {
            try
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    var waybillView = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => item.OrderID == orderID);

                    var linq = from waybill in waybillView
                               where waybill.wbCarrierID == carrierID
                               select new
                               {
                                   waybill.wbCode,
                                   waybill.wbCarrierID,
                               };

                    var codes = from waybill in linq
                                select new
                                {
                                    waybill.wbCode
                                };

                    return Json(new JSingle
                    {
                        code = 200,
                        success = true,
                        data = codes.Distinct().ToArray(),
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new JSingle
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 根据订单ID, 客户ID获取运单号
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetWaybillCodeByClientID(string orderID, string clientID)
        {
            try
            {
                using (var reponsitory = new PvWmsRepository())
                {
                    var waybillView = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.WaybillsTopView>().Where(item => item.OrderID == orderID);
                    var enterCode = reponsitory.ReadTable<Layers.Data.Sqls.PvWms.ClientsTopView>().Single(item => item.ID == clientID).EnterCode;

                    var codes = from waybill in waybillView
                                where waybill.wbEnterCode == enterCode
                                select new
                                {
                                    waybill.wbCode,
                                };

                    return Json(new JSingle
                    {
                        code = 200,
                        success = true,
                        data = codes.Distinct().ToArray(),
                    }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new JSingle
                {
                    code = 400,
                    success = false,
                    data = ex.Message,
                }, JsonRequestBehavior.AllowGet);
            }
        }

        #region bak
        ///// <summary>
        ///// 支出合作人接口
        ///// </summary>
        ///// <param name="waybillid">运单号</param>
        ///// <param name="otype">otype:in 入库 ，out 出库</param>
        ///// <returns></returns>
        //[HttpGet]
        //public ActionResult OutcomeParters(string waybillid, string otype)
        //{
        //    var json = new JSingle<dynamic>()
        //    {
        //        code = 200,
        //        success = true,
        //    };

        //    try
        //    {
        //        using (var wbView = new Wms.Services.Views.PaymentsWayBillRoll())
        //        using (var carriersView = new Wms.Services.Views.CarriersTopView())
        //        {
        //            var waybill = wbView.SearchByWaybillD(waybillid).ToFillData("in", otype); ;
        //            var carries = carriersView.OrderBy(item => item.Name).Select(item => new PayInfo
        //            {
        //                ID = item.ID,
        //                Name = item.Name,
        //            });

        //            var carriesIncludeAnony = carries.ToList();
        //            carriesIncludeAnony.Insert(0, new PayInfo() { ID = AnonymousEnterprise.Current.ID, Name = AnonymousEnterprise.Current.Name });

        //            json.data = new
        //            {
        //                //记账
        //                Record = new
        //                {
        //                    //承运商
        //                    Payees = carries,
        //                    //付款人
        //                    Payers = new List<dynamic>()
        //                    {
        //                        new
        //                        {
        //                            ID = WhPartners.OutOrder[waybill.PayeeID].ID,
        //                            Name = WhPartners.OutOrder[waybill.PayeeID].Name,
        //                        }
        //                    },
        //                },
        //                //现金记账
        //                Current = new
        //                {
        //                    //承运商和匿名
        //                    Payees = carriesIncludeAnony,
        //                    //付款人
        //                    Payers = new List<dynamic>
        //                    {
        //                        new
        //                        {
        //                            ID = WhPartners.OutOrder[waybill.PayeeID].ID,
        //                            Name = WhPartners.OutOrder[waybill.PayeeID].Name,
        //                        }
        //                    }
        //                }
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        json.code = 500;
        //        json.success = false;
        //        json.data = ex.Message;
        //    }

        //    return Json(json, JsonRequestBehavior.AllowGet);
        //} 
        #endregion
        #endregion

        #region 是否支付
        /// <summary>
        /// 是否支付
        /// </summary>
        /// <param name="orderID">订单ID</param>
        /// <param name="type">1代收（出库） 2代付（入库）</param>
        /// <returns>1已支付 2未支付</returns>
        [HttpGet]
        public int IsPay(string orderID, int type)
        {
            using (var applicationsView = new ApplicationsTopView())
            {
                var applications = applicationsView.Where(item => item.OrderID == orderID && item.Status == GeneralStatus.Normal);

                if (applications == null || !applications.Any())
                {
                    return 2;
                }

                //代收货款（出库）
                if (type == 1)
                {
                    if (applications.Where(item => item.Type == ApplicationType.Receival).Any(item => item.ReceiveStatus != ApplicationReceiveStatus.Received))
                    {
                        return 2;
                    }
                }
                //代付货款
                else
                {
                    if (applications.Where(item => item.Type == ApplicationType.Payment).Any(item => item.PaymentStatus != ApplicationPaymentStatus.Paid))
                    {
                        return 2;
                    }
                }

                return 1;
            }
        }

        #region bak
        ///// <summary>
        ///// 是否支付
        ///// </summary>
        ///// <param name="orderID">订单ID</param>
        ///// <param name="type">1代收（出库） 2代付（入库）</param>
        ///// <returns>1已支付 2未支付</returns>
        //[HttpGet]
        //public int IsPay(string orderID, int type)
        //{
        //    using (var ordersView = new OrdersTopView())
        //    using (var vouchersView = new VouchersStatisticsView())
        //    using (var paymentsView = new PaymentsStatisticsView())
        //    using (var applicationsView = new ApplicationsTopView())
        //    {
        //        var order = ordersView.Single(item => item.ID == orderID);

        //        //代收货款（出库）
        //        if (type == 1)
        //        {
        //            var vouchers = vouchersView.Where(item => item.Catalog == CatalogConsts.仓储服务费 && item.Subject == SubjectConsts.代收货款)
        //           .Where(item => item.OrderID == orderID).ToArray();

        //            //是否存在  不入账情况
        //            if (applicationsView.Any(item => item.OrderID == orderID && item.IsEntry == false))
        //            {
        //                //不入账   已审核：已支付；没有审核：未支付
        //                bool isNotPay = applicationsView.Where(item => item.OrderID == orderID && item.IsEntry == false)
        //                    .Any(item => item.ApplicationStatus != Yahv.PvWsOrder.Services.Enums.ApplicationStatus.Examined);

        //                //审批未通过 返回未支付
        //                if (isNotPay)
        //                {
        //                    return 2;
        //                }
        //                else
        //                {
        //                    //已审批通过 并且没有其他申请应收数据，直接返回已付款
        //                    if (vouchers.Length <= 0)
        //                    {
        //                        return 1;
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                //不包含 代收货款
        //                if (vouchers.Length <= 0)
        //                {
        //                    return 2;
        //                }
        //            }

        //            //应收费用判断
        //            if (vouchers.Sum(item => item.Remains) == 0)
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 2;
        //            }
        //        }
        //        //代付货款
        //        else
        //        {
        //            //根据订单ID查找申请ID
        //            var applicationIds = applicationsView.Where(item => item.OrderID == orderID).Select(item => item.ID).ToArray();

        //            //根据申请ID 查询代付货款是否支付
        //            var payments = paymentsView.Where(item => item.Catalog == CatalogConsts.仓储服务费 && item.Subject == SubjectConsts.代付货款)
        //           .Where(item => applicationIds.Contains(item.ApplicationID)).ToArray();


        //            //不包含 代付货款
        //            if (payments.Length <= 0)
        //            {
        //                return 2;
        //            }
        //            if (payments.Sum(item => item.Remains) == 0)
        //            {
        //                return 1;
        //            }
        //            else
        //            {
        //                return 2;
        //            }
        //        }
        //    }
        //} 
        #endregion
        #endregion

        #region 私有函数
        /// <summary>
        /// 格式化json数据
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        static string GetFormatDescription(string json)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(json))
            {
                return result;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<JsonData>(json);
                JsonData child = data.Children;
                while (child?.Value != null)
                {
                    result += child.Value + " - ";
                    child = child.Children;
                }
            }
            catch (Exception)
            {
            }

            return result.Trim().Trim('-').Trim();
        }

        /// <summary>
        /// 格式化json数据 For 仓储费
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        static string GetFormatDescriptionForWarehouseFee(string json)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(json))
            {
                return result;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<JsonData>(json);
                result += data.Value + " - ";
                JsonData child = data.Children;
                while (child?.Value != null)
                {
                    result += child.Value + " - ";
                    child = child.Children;
                }
            }
            catch (Exception)
            {
            }

            return result.Trim().Trim('-').Trim();
        }

        /// <summary>
        /// 添加任务
        /// </summary>
        void InsertTaskPool(string orderId)
        {
            if (string.IsNullOrEmpty(orderId)) return;

            using (var reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                reponsitory.Insert(new Layers.Data.Sqls.PvCenter.TasksPool()
                {
                    ID = PKeySigner.Pick(PkeyType.TasksPool),
                    Name = TaskSettgins.PvCenter.芯达通费用同步,
                    MainID = orderId,
                    CreateDate = DateTime.Now
                });
            }
            //using (var waybills = new ServicesWaybillsTopView())
            //using (var outputs = new OutputsRoll())
            //using (var notices = new CgNoticesView())
            //{
            //    if (string.IsNullOrEmpty(orderId)) return;

            //    var query = from waybill in waybills.Select(item => new { item.OrderID, item.Source, item.ID }).ToArray()
            //                join notice in notices.Select(item => new { item.WaybillID, item.OutputID }).ToArray() on waybill.ID equals notice.WaybillID
            //                join output in outputs.Select(item => new { item.ID }).ToArray() on notice.OutputID equals output.ID
            //                where waybill.OrderID == orderId && (waybill.Source == CgNoticeSource.AgentBreakCustoms
            //                          || waybill.Source == CgNoticeSource.AgentBreakCustomsForIns
            //                          || waybill.Source == CgNoticeSource.AgentCustomsFromStorage)
            //                select new
            //                {
            //                    waybill.OrderID
            //                };

            //    if (query != null && query.Any(item => item.OrderID == orderId))
            //    {
            //        ConsoleApp.vTaskers.Services.LitTools.Current.Record(orderId, name: TaskSettgins.PvCenter.芯达通费用同步);
            //    }
            //}
        }
        #endregion
    }

    public class Subject
    {
        public string Name { get; set; }
        public SubjectPayQuote PayQuote { get; set; }
        public string SubjectName { get; set; }
        public string Catalog { get; set; }
    }
    public class SubjectPayQuote
    {
        public Currency? Currency { get; set; }
        public Decimal? Price { get; set; }
    }

    class PayInfo
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }

    enum DelivaryOpportunity
    {
        [Description("现货现款")]
        CashOn = 1,
        [Description("先款后货")]
        PaymentBeforeDelivery = 2,
        [Description("先货后款")]
        PaymentAfterDelivery = 3,
    }


}