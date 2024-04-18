
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using WebApi.Models;
using WebApi.Models.ApiUtils;

namespace WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [CustomBasicAuthenticationFilter]
    public class IcgooController : ApiController
    {
        /// <summary>
        /// Icgoo 下单接口
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage CreateOrderMQ(PartNoJson obj)
        {
            try
            {
                //解析Json
                string jsonresult = obj.jsonStr;
                PartNoReceive model = JsonConvert.DeserializeObject<PartNoReceive>(jsonresult);

                var CurrencyView = new Needs.Ccs.Services.Views.BaseCurrenciesView().Where(item => item.Code == model.currency).FirstOrDefault();
                if (CurrencyView == null)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("币制代码不正确", Encoding.UTF8, "application/json"),
                    };
                }

                var user = new Needs.Ccs.Services.Views.ApiClientsView().Where(item => item.CompanyCode == model.ccode).FirstOrDefault();
                if (user == null)
                {
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent("公司代码不正确", Encoding.UTF8, "application/json"),
                    };
                }
                else
                {
                    //创建IcgooPost记录，并持久化
                    IcgooMQ mq = new IcgooMQ();
                    mq.ID = ChainsGuid.NewGuidUp();
                    mq.PostData = jsonresult;
                    mq.IsAnalyzed = true;
                    mq.Status = Needs.Ccs.Services.Enums.Status.Normal;
                    mq.UpdateDate = mq.CreateDate = DateTime.Now;
                    mq.CompanyType = Needs.Ccs.Services.Enums.CompanyTypeEnums.Icgoo;
                    mq.Summary = user.ClientId;
                    mq.Enter();

                    //加入队列
                    string UserName = System.Configuration.ConfigurationManager.AppSettings["UserName"];
                    string Password = System.Configuration.ConfigurationManager.AppSettings["Password"];
                    string HostName = System.Configuration.ConfigurationManager.AppSettings["HostName"];
                    string Port = System.Configuration.ConfigurationManager.AppSettings["Port"];
                    string VirtualHost = System.Configuration.ConfigurationManager.AppSettings["VirtualHost"];

                    Needs.Ccs.Services.Models.MQMethod mqMethod = new Needs.Ccs.Services.Models.MQMethod(UserName, Password, HostName, Convert.ToInt16(Port), VirtualHost);
                    string returnmsg = "";

                    bool isSuccess = false;
                    if (VirtualHost.ToLower().Equals("wl"))
                    {
                        isSuccess = mqMethod.ProduceIcgoo(mq.ID, ref returnmsg);
                    }
                    else
                    {
                        isSuccess = mqMethod.ProduceIcgooInXDT(mq.ID, ref returnmsg);
                    }


                    //返回值
                    string json = JsonConvert.SerializeObject(new
                    {
                        eoOrderNo = user.ClientCode + "-" + model.refNo,
                        status = isSuccess ? "success" : "fail",
                        msg = returnmsg,
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }
            }
            catch
            {
                IcgooMQ mq = new IcgooMQ();
                mq.ID = ChainsGuid.NewGuidUp();
                mq.PostData = obj.jsonStr;
                mq.IsAnalyzed = false;
                mq.Status = Needs.Ccs.Services.Enums.Status.Normal;
                mq.UpdateDate = mq.CreateDate = DateTime.Now;
                mq.Enter();

                return new HttpResponseMessage()
                {
                    Content = new StringContent("提交数据不正确", Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// Icgoo提交卡板数
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PalletNumber(PalletNumberJson obj)
        {
            bool isSuccess = false;
            try
            {
                DateTime DeclareDate = DateTime.Now;
                if (!string.IsNullOrEmpty(obj.DeclareDate))
                {
                    DateTime.TryParse(obj.DeclareDate, out DeclareDate);
                }

                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //var CurrentDate = DeclareDate.Date;
                    //var StartDate = CurrentDate;
                    //var EndDate = CurrentDate.AddDays(1);

                    //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Pallets>(new
                    //{ Status = (int)Needs.Ccs.Services.Enums.Status.Delete },
                    //t => t.NoticeTime >= StartDate && t.NoticeTime < EndDate && t.Stock==obj.Stock);

                    reponsitory.Insert<Layer.Data.Sqls.ScCustoms.Pallets>(new Layer.Data.Sqls.ScCustoms.Pallets
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        Stock = obj.Stock,
                        Pallet = obj.Pallet,
                        NoticeTime = DeclareDate,
                        Status = (int)Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }

                isSuccess = true;

                string json = JsonConvert.SerializeObject(new
                {
                    Success = isSuccess ? "success" : "fail",
                    Code = "200",
                    Data = "提交成功"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = "fail",
                    Code = "400",
                    Data = "提交失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// Icgoo提交付款记录
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage PayRMB(IcgooFinanceReceipt receipt)
        {
            bool isSuccess = false;
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceReceipts>().Count(item => item.SeqNo == receipt.SeqNo);
                    if (count > 0)
                    {
                        string repeatJson = JsonConvert.SerializeObject(new
                        {
                            Success = false,
                            Code = "400",
                            Data = "该流水号已提交"
                        });
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(repeatJson, Encoding.UTF8, "application/json"),
                        };
                    }
                }

                string clientID = System.Configuration.ConfigurationManager.AppSettings["IcgooClientID"];
                string FinanceVaultID = System.Configuration.ConfigurationManager.AppSettings["FinanceVaultID"];
                string AccountID = System.Configuration.ConfigurationManager.AppSettings["AccountID"];
                string IcgooPayAdminID = System.Configuration.ConfigurationManager.AppSettings["IcgooPayAdminID"];
                var financeReceipt = new Needs.Ccs.Services.Models.FinanceReceipt();
                financeReceipt.Payer = receipt.Payer;
                financeReceipt.SeqNo = receipt.SeqNo;
                financeReceipt.FeeType = FinanceFeeType.DepositReceived;
                financeReceipt.ReceiptType = PaymentType.TransferAccount;
                financeReceipt.ReceiptDate = Convert.ToDateTime(receipt.ReceiptDate);
                financeReceipt.Currency = "RMB";
                financeReceipt.Rate = 1;
                financeReceipt.Vault = new FinanceVault
                {
                    ID = FinanceVaultID
                };
                financeReceipt.Amount = receipt.Amount;
                financeReceipt.Account = new FinanceAccount
                {
                    ID = AccountID
                };
                financeReceipt.Admin = Needs.Underly.FkoFactory<Admin>.Create(IcgooPayAdminID);
                financeReceipt.Enter();

                //更新余额
                IcgooBalance icgooBalance = new IcgooBalance();
                icgooBalance.ClientID = clientID;
                icgooBalance.Balance = receipt.Amount;
                icgooBalance.Currency = "RMB";
                icgooBalance.TriggerSource = "Icgoo调用付款接口";
                icgooBalance.UpdateBalance();

                isSuccess = true;

                string json = JsonConvert.SerializeObject(new
                {
                    Success = isSuccess ? true : false,
                    Code = "200",
                    Data = "付款成功"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "付款失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }

        }

        /// <summary>
        /// 转付汇申请
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [System.Web.Http.HttpPost]
        public HttpResponseMessage EntrustPayExchange(EntrustPayExchange obj)
        {
            bool isSuccess = false;
            try
            {
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>().Count(item => item.ApplyID == obj.ApplyID);
                    if (count > 0)
                    {
                        string repeatJson = JsonConvert.SerializeObject(new
                        {
                            Success = false,
                            Code = "400",
                            Data = "该申请号已提交"
                        });
                        return new HttpResponseMessage()
                        {
                            Content = new StringContent(repeatJson, Encoding.UTF8, "application/json"),
                        };
                    }
                }

                isSuccess = true;
                string IcgooClientID = System.Configuration.ConfigurationManager.AppSettings["IcgooClientID"];
                var entrust = new Needs.Ccs.Services.Models.DollarEquityApply();
                entrust.ID = ChainsGuid.NewGuidUp();
                entrust.ApplyID = obj.ApplyID;
                entrust.ClientID = IcgooClientID;
                entrust.SupplierChnName = obj.SupplierChnName;
                entrust.SupplierEngName = obj.SupplierEngName;
                entrust.BankName = obj.BankName;
                entrust.BankAddress = obj.BankAddress;
                entrust.BankAccount = obj.BankAccount;
                entrust.SwiftCode = obj.SwiftCode;
                entrust.Amount = obj.Amount;
                entrust.Currency = "USD";
                entrust.IsPaid = false;
                entrust.ExpectDate = Convert.ToDateTime(obj.ExpectPayDate);
                entrust.UpdateDate = DateTime.Now;
                entrust.Enter();


                string json = JsonConvert.SerializeObject(new
                {
                    Success = isSuccess ? true : false,
                    Code = "200",
                    Data = "转付汇申请成功"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "转付汇申请失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 获取委托付汇进度
        /// </summary>
        /// <param name="ApplyID"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetPayExchangeStatus(string ApplyID)
        {
            try
            {
                string json = "";
                var apply = new DollarEquityAppliesViewOrigin().Where(t => t.ApplyID == ApplyID).FirstOrDefault();
                if (apply == null)
                {
                    json = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = "该ID未提交转付汇申请"
                    });
                }
                else
                {
                    List<string> urls = new List<string>();
                    string filePrefix = System.Configuration.ConfigurationManager.AppSettings["FileServerUrl"];
                    using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                    {
                        var fileURLs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplyFiles>().
                            Where(t => t.DollarEquityApplyID == apply.ID).Select(t => t.Url).ToList();
                        foreach (var t in fileURLs)
                        {
                            urls.Add(filePrefix + @"/" + t.Replace("\\", @"/"));
                        }
                    }


                    json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = new
                        {
                            IsPaid = apply.IsPaid,
                            //FileURL = apply.FileURL
                            FileURL = urls
                        }
                    });
                }


                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询委托付汇进度失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 余额查询接口
        /// </summary>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage IcgooBalance()
        {
            try
            {
                string clientID = System.Configuration.ConfigurationManager.AppSettings["IcgooClientID"];
                var balanceView = new ClientBalanceViewOrigin().Where(t => t.ClientID == clientID);
                var RMBBalance = balanceView.Where(t => t.Currency == "RMB").FirstOrDefault();
                var USDBalance = balanceView.Where(t => t.Currency == "USD").FirstOrDefault();
                decimal rmbBalance = 0, usdBalance = 0;
                if (RMBBalance != null)
                {
                    rmbBalance = RMBBalance.Balance.Value;
                }
                if (USDBalance != null)
                {
                    usdBalance = USDBalance.Balance.Value;
                }

                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    Data = new
                    {
                        RMBBalance = rmbBalance,
                        DollarEquity = usdBalance
                    }
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };

            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询余额失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }

        }

        /// <summary>
        /// 查询资金核销明细
        /// </summary>
        /// <param name="icgooOrder"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage VerificationInfo(string icgooOrder)
        {
            try
            {
                VerificationInfo info = new VerificationInfo();
                List<OrderFeeInfo> OrderFeeInfos = new List<OrderFeeInfo>();
                bool IsAllVerification = true;
                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var Orders = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>().Where(t => t.IcgooOrder == icgooOrder)
                                                                                                 .OrderBy(t => t.CreateDate).Select(t => t.OrderID).ToList();
                    if (Orders.Count() == 0)
                    {
                        IsAllVerification = false;
                    }
                    var OrderReceiptsView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>();
                    var PayExchangeApplyItemsView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();
                    var PayExchangeApplyView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
                    var OrdersView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
                    var OrderItemsView = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
                    foreach (var orderID in Orders)
                    {
                        OrderFeeInfo orderFeeInfo = new OrderFeeInfo();
                        orderFeeInfo.OrderItemInfos = new List<OrderItemInfo>();
                        var receipt = OrderReceiptsView.Where(t => t.OrderID == orderID && t.Type == (int)OrderReceiptType.Received).ToList();
                        var goodsReceipt = -receipt.Where(t => t.FeeType == (int)OrderFeeType.Product).Sum(t => t.Amount);
                        var tariffReceipt = -receipt.Where(t => t.FeeType == (int)OrderFeeType.Tariff).Sum(t => t.Amount);
                        var addReceipt = -receipt.Where(t => t.FeeType == (int)OrderFeeType.AddedValueTax).Sum(t => t.Amount);
                        var agencyFeeReceipt = -receipt.Where(t => t.FeeType == (int)OrderFeeType.AgencyFee).Sum(t => t.Amount);
                        var incidentalReceipt = -receipt.Where(t => t.FeeType == (int)OrderFeeType.Incidental).Sum(t => t.Amount);

                        orderFeeInfo.OrderID = orderID;
                        orderFeeInfo.GoodsValue = goodsReceipt;
                        orderFeeInfo.Tariff = tariffReceipt;
                        orderFeeInfo.AddedVauleTax = addReceipt;
                        orderFeeInfo.AgencyFee = agencyFeeReceipt;
                        orderFeeInfo.Incidental = incidentalReceipt;

                        var should = OrderReceiptsView.Where(t => t.OrderID == orderID && t.Type == (int)OrderReceiptType.Receivable).ToList();
                        var tariffShould = should.Where(t => t.FeeType == (int)OrderFeeType.Tariff).Sum(t => t.Amount);
                        var addShould = should.Where(t => t.FeeType == (int)OrderFeeType.AddedValueTax).Sum(t => t.Amount);
                        var agencyFeeShould = should.Where(t => t.FeeType == (int)OrderFeeType.AgencyFee).Sum(t => t.Amount);
                        var incidentalShould = should.Where(t => t.FeeType == (int)OrderFeeType.Incidental).Sum(t => t.Amount);
                        tariffShould = tariffShould < 50 ? 0 : tariffShould;
                        addShould = addShould < 50 ? 0 : addShould;
                        if ((tariffReceipt - tariffShould != 0) || (addReceipt - addShould != 0) || (agencyFeeReceipt - agencyFeeShould != 0) || (incidentalReceipt - incidentalShould != 0))
                        {
                            IsAllVerification = false;
                        }

                        decimal applyUSD = 0, applyRMB = 0;
                        var applyIDs = PayExchangeApplyItemsView.Where(t => t.OrderID == orderID).OrderBy(t => t.CreateDate).ToList();

                        foreach (var payExchange in applyIDs)
                        {
                            var pay = PayExchangeApplyView.Where(t => t.ID == payExchange.PayExchangeApplyID).FirstOrDefault();
                            if (pay.PayExchangeApplyStatus == (int)PayExchangeApplyStatus.Audited || pay.PayExchangeApplyStatus == (int)PayExchangeApplyStatus.Approvaled || pay.PayExchangeApplyStatus == (int)PayExchangeApplyStatus.Completed)
                            {
                                applyUSD += payExchange.Amount;
                                applyRMB += Math.Round(payExchange.Amount * pay.ExchangeRate, 2, MidpointRounding.AwayFromZero);
                                orderFeeInfo.ExchangeRate = pay.ExchangeRate;
                            }
                        }


                        var currentOrder = OrdersView.Where(t => t.ID == orderID).FirstOrDefault();
                        if (Math.Round(currentOrder.DeclarePrice, 2, MidpointRounding.AwayFromZero) != applyUSD || goodsReceipt != applyRMB)
                        {
                            IsAllVerification = false;
                        }

                        var currentOrderItems = OrderItemsView.Where(t => t.OrderID == orderID).ToList();
                        foreach (var orderItem in currentOrderItems)
                        {
                            OrderItemInfo orderItemInfo = new OrderItemInfo();
                            orderItemInfo.Model = orderItem.Model;
                            orderItemInfo.Brand = orderItem.Manufacturer;
                            orderItemInfo.ProductUnionCode = orderItem.ProductUniqueCode;
                            orderItemInfo.Qty = orderItem.Quantity;
                            orderFeeInfo.OrderItemInfos.Add(orderItemInfo);
                        }
                        OrderFeeInfos.Add(orderFeeInfo);
                    }
                }

                info.IsAllVerification = IsAllVerification;
                info.OrderFeeInfos = OrderFeeInfos;

                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    Data = info
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };

            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询核销明细失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 创建客户账户
        /// </summary>
        /// <param name="clientID"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage CreateClientBalance(string clientID)
        {
            try
            {
                ClientBalance clientRMBBalance = new ClientBalance();
                clientRMBBalance.ClientID = clientID;
                clientRMBBalance.ClientAccount = "RMBAccount";
                clientRMBBalance.Currency = "RMB";

                ClientBalance clientUSDBalance = new ClientBalance();
                clientUSDBalance.ClientID = clientID;
                clientUSDBalance.ClientAccount = "USDAccount";
                clientUSDBalance.Currency = "USD";

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientBalance
                    {
                        ID = clientRMBBalance.ID,
                        ClientID = clientID,
                        ClientAccount = clientRMBBalance.ClientAccount,
                        Balance = 0,
                        Currency = "RMB",
                        Version = 0,
                        Status = 200,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now                      
                    });

                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientBalance
                    {
                        ID = clientUSDBalance.ID,
                        ClientID = clientID,
                        ClientAccount = clientUSDBalance.ClientAccount,
                        Balance = 0,
                        Currency = "USD",
                        Version = 0,
                        Status = 200,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now
                    });
                }

                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "200",
                    Data = "新建账户成功"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "新建账户失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 根据时间段查询核销情况
        /// </summary>
        /// <param name="StartDate"></param>
        /// <param name="EndDate"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage IsVerified(DateTime StartDate,DateTime EndDate)
        {
            try
            {
                List<SummaryVerificationInfo> Infos = new List<SummaryVerificationInfo>();
                DateTime dtStart = StartDate;
                DateTime dtEnd = EndDate.AddDays(1);

                using (var reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var OrderInfos = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>()
                                    where c.VerifyDate >= dtStart && c.VerifyDate < dtEnd
                                    && c.IcgooOrder.Contains("CM")
                                    orderby c.CreateDate
                                    select c).ToList();

                    var IcgooOrders = OrderInfos.Select(t => t.IcgooOrder).Distinct();
                    foreach(var t in IcgooOrders)
                    {
                        SummaryVerificationInfo info = new SummaryVerificationInfo();
                        info.IcgooOrder = t;

                        bool IsVerified = true;
                        var tinyOrders = OrderInfos.Where(item => item.IcgooOrder == t).ToList();
                        foreach(var tinyOrder in tinyOrders)
                        {
                            if(!(tinyOrder.IsVerified == null ? false : tinyOrder.IsVerified.Value))
                            {
                                IsVerified = false;
                            }
                        }
                        info.IsAllVerified = IsVerified;

                        Infos.Add(info);
                    }                          

                }

                string json = JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Code = "200",
                    Data = Infos
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };

            }
            catch (Exception ex)
            {
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询核销明细失败"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manufacturer"></param>
        /// <returns></returns>
        [System.Web.Http.HttpGet]
        public HttpResponseMessage IcgooClassifyResult(string model,string manufacturer)
        {
            try
            {
                if (string.IsNullOrEmpty(model))
                {
                    string modelnull = JsonConvert.SerializeObject(new
                    {
                        Success = false,
                        Code = "400",
                        Data = "型号不能为空"
                    });
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(modelnull, Encoding.UTF8, "application/json"),
                    };
                }


                //var redis = RedisHelper.Current.connectionMultiplexer;
                //var db = redis.GetDatabase();
                //if (db.KeyExists(model)) 
                //{
                //    string modelnull = JsonConvert.SerializeObject(new
                //    {
                //        Success = false,
                //        Code = "400",
                //        Data = "同一型号，1min之内只能查询一次"
                //    });
                //    return new HttpResponseMessage()
                //    {
                //        Content = new StringContent(modelnull, Encoding.UTF8, "application/json"),
                //    };
                //}
                //else 
                //{
                //    db.StringSet(model, model);
                //    db.KeyExpire(model, DateTime.Now.AddMinutes(1));
                //}
                

                using (var query = new Needs.Ccs.Services.Views.IcgooClassifyResultView(model)) 
                {
                    var view = query;
                    if (!string.IsNullOrEmpty(manufacturer)) 
                    {
                        view = view.SearchByBrand(manufacturer);                       
                    }
                    List<Needs.Ccs.Services.Models.IcgooClassifyResult> results = view.ToMyPage(1, 50);
                    string json = JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Code = "200",
                        Data = results
                    }); 
                    return new HttpResponseMessage()
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json"),
                    };
                }                
            }
            catch (Exception ex)
            {
                ex.CcsLog("Icgoo查询归类信息");
                string json = JsonConvert.SerializeObject(new
                {
                    Success = false,
                    Code = "400",
                    Data = "查询归类信息出错"
                });
                return new HttpResponseMessage()
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json"),
                };
            }
        }
    }
}