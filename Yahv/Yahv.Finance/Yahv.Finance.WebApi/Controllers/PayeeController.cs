using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Finance.WebApi.Filter;
using Yahv.Underly;
using Yahv.Underly.Enums;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using PKeyType = Yahv.Finance.Services.PKeyType;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 收款
    /// </summary>
    public class PayeeController : ClientController
    {
        #region 收款
        /// <summary>
        /// 收款
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult PayeeLeftEnter(InputParam<PayeeLeftInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            PayeeLeftInputDto input = new PayeeLeftInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var payeeLefsView = new PayeeLeftsRoll())
            using (var accountsView = new AccountsRoll())
            using (var epView = new EnterprisesRoll())
            using (var flowsView = new FlowAccountsRoll())
            using (var payerRights = new PayerRightsRoll())
            using (var moneyOrders = new MoneyOrdersRoll())
            {
                try
                {
                    input = param.Model;
                    var entity = payeeLefsView.FirstOrDefault(item => item.FormCode == input.SeqNo);

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.SeqNo}]流水号已存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            //账户是否存在
                            var account = accountsView.FirstOrDefault(item => item.Code == input.Account);
                            if (account == null || string.IsNullOrWhiteSpace(account.ID))
                            {
                                result = new JMessage()
                                {
                                    data = $"[{input.Account}]账户不存在!",
                                    success = false,
                                    code = 500,
                                };
                            }
                            else
                            {
                                var type = FlowAccountType.BankStatement;
                                MoneyOrder moneyOrder = null;

                                //如果是承兑汇票收款，默认走一个背书流程
                                if (input.ReceiptType == (int)PaymentMethord.BankAcceptanceBill
                                    || input.ReceiptType == (int)PaymentMethord.CommercialAcceptanceBill)
                                {
                                    type = FlowAccountType.MoneyOrder;
                                    moneyOrder = moneyOrders.FirstOrDefault(item => item.Code == input.SeqNo);

                                    if (moneyOrder != null && !moneyOrder.ID.IsNullOrEmpty())
                                    {
                                        var endorsement = new Endorsement()
                                        {
                                            CreateDate = DateTime.Now,
                                            CreatorID = input.CreatorID,
                                            EndorseDate = input.ReceiptDate,
                                            IsTransfer = true,
                                            MoneyOrderID = moneyOrder.ID,
                                            PayerAccountID = moneyOrder.PayeeAccountID,
                                            PayeeAccountID = account.ID,
                                        };
                                        //背书转让
                                        endorsement.Enter();

                                        //修改承兑汇票 持票人账户ID
                                        repository.Update<Layers.Data.Sqls.PvFinance.MoneyOrders>(new
                                        {
                                            PayeeAccountID = account.ID
                                        }, item => item.ID == moneyOrder.ID);
                                    }
                                }

                                string flowId = PKeySigner.Pick(PKeyType.FlowAcc);
                                //收款流水
                                new FlowAccount()
                                {
                                    ID = flowId,
                                    Type = type,
                                    AccountMethord = AccountMethord.Receipt,
                                    AccountID = account.ID,
                                    Currency = account.Currency,
                                    Price = input.Amount,
                                    PaymentDate = input.ReceiptDate,
                                    FormCode = input.SeqNo,
                                    Currency1 = Currency.CNY,
                                    ERate1 = input.Rate,
                                    Price1 = input.Amount * input.Rate,
                                    CreatorID = input.CreatorID,
                                    TargetAccountName = input.Payer,
                                    MoneyOrderID = moneyOrder?.ID,
                                    PaymentMethord = (PaymentMethord)input.ReceiptType     //收款方式
                                }.Add();

                                //收款
                                entity = new PayeeLeft()
                                {
                                    AccountCatalogID = input.FeeType,
                                    AccountCode = input.Account,
                                    AccountID = account.ID,
                                    PayerNature = (NatureType)input.AccountSource,
                                    PayerName = input.Payer,
                                    Currency = account.Currency,
                                    Price = input.Amount,
                                    Currency1 = Currency.CNY,
                                    ERate1 = input.Rate,
                                    Price1 = input.Amount * input.Rate,
                                    CreatorID = input.CreatorID,
                                    Summary = input.Summary,
                                    FlowID = flowId,
                                };
                                entity.Add();

                                //不包含付款人的话，新增付款人企业
                                if (!epView.Any(item => item.Name == input.Payer.Trim()) && (NatureType)input.AccountSource == NatureType.Public)
                                {
                                    new Enterprise()
                                    {
                                        Name = input.Payer.Trim(),
                                        Type = EnterpriseAccountType.Client,
                                        CreatorID = Npc.Robot.Obtain(),
                                        ModifierID = Npc.Robot.Obtain(),
                                    }.Enter();
                                }
                            }
                        }
                    }
                    else if (param.Option.ToLower() == OptionConsts.update)
                    {
                        //如果旧流水号有值，说明修改了流水号
                        if (!string.IsNullOrEmpty(input.OldSeqNo))
                        {
                            entity = payeeLefsView.FirstOrDefault(item => item.FormCode == input.OldSeqNo);
                        }

                        if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"未找到要修改的记录!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            if (payerRights.Any(item => item.PayerLeftID == entity.ID))
                            {
                                result = new JMessage()
                                {
                                    data = $"该收款已核销，不能再修改!",
                                    success = false,
                                    code = 500,
                                };
                            }
                            else
                            {
                                var account = accountsView.FirstOrDefault(item => item.Code == input.Account);
                                var flowAccount = flowsView.FirstOrDefault(item => item.ID == entity.FlowID);
                                if (flowAccount == null || string.IsNullOrEmpty(flowAccount.ID))
                                {
                                    result = new JMessage()
                                    {
                                        data = $"未找到流水记录!",
                                        success = false,
                                        code = 500,
                                    };
                                }
                                else
                                {
                                    flowAccount.AccountMethord = AccountMethord.Receipt;
                                    flowAccount.AccountID = account.ID;
                                    flowAccount.Currency = account.Currency;
                                    flowAccount.Price = input.Amount;
                                    flowAccount.PaymentDate = input.ReceiptDate;
                                    flowAccount.FormCode = input.SeqNo;
                                    flowAccount.Currency1 = Currency.CNY;
                                    flowAccount.ERate1 = input.Rate;
                                    flowAccount.Price1 = input.Amount * input.Rate;
                                    flowAccount.CreatorID = input.CreatorID;
                                    flowAccount.TargetAccountName = input.Payer;
                                    flowAccount.PaymentMethord = (PaymentMethord)input.ReceiptType;
                                    flowAccount.Enter();

                                    entity.AccountCatalogID = input.FeeType;
                                    entity.AccountCode = input.Account;
                                    entity.AccountID = account.ID;
                                    entity.PayerNature = (NatureType)input.AccountSource;
                                    entity.PayerName = input.Payer;
                                    entity.Currency = account.Currency;
                                    entity.Price = input.Amount;
                                    entity.ERate1 = input.Rate;
                                    entity.Price1 = input.Amount * input.Rate;
                                    entity.CreatorID = input.CreatorID;
                                    entity.Summary = input.Summary;
                                    entity.Enter();
                                }
                            }

                            //不包含付款人的话，新增付款人企业
                            if (!epView.Any(item => item.Name == input.Payer.Trim()) && (NatureType)input.AccountSource == NatureType.Public)
                            {
                                new Enterprise()
                                {
                                    Name = input.Payer.Trim(),
                                    Type = EnterpriseAccountType.Client,
                                    CreatorID = Npc.Robot.Obtain(),
                                    ModifierID = Npc.Robot.Obtain(),
                                }.Enter();
                            }
                        }
                    }
                    else
                    {
                        result = new JMessage()
                        {
                            data = $"操作失败，不支持该操作!",
                            success = false,
                            code = 500,
                        };
                    }

                    if (result.success)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result = new JMessage()
                    {
                        data = $"操作异常!{ex.Message}",
                        success = false,
                        code = 500,
                    };
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }

        /// <summary>
        /// 批量收款
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult PayeeLeftBatchEnter(InputParam<IEnumerable<PayeeLeftInputDto>> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            IEnumerable<PayeeLeftInputDto> inputs = new List<PayeeLeftInputDto>();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var payeeLefsView = new PayeeLeftsRoll())
            using (var accountsView = new AccountsRoll())
            using (var epView = new EnterprisesRoll())
            using (var flowsView = new FlowAccountsRoll())
            using (var payerRights = new PayerRightsRoll())
            {
                try
                {
                    var flows = new List<FlowAccount>();
                    var lefts = new List<PayeeLeft>();
                    var eps = new List<Enterprise>();

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        inputs = param.Model;

                        foreach (var input in inputs)
                        {
                            var entity = payeeLefsView.FirstOrDefault(item => item.FormCode == input.SeqNo);

                            if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
                            {
                                result = new JMessage()
                                {
                                    data = $"[{input.SeqNo}]流水号已存在!",
                                    success = false,
                                    code = 500,
                                };
                                break;
                            }
                            else
                            {
                                //账户是否存在
                                var account = accountsView.FirstOrDefault(item => item.Code == input.Account);
                                if (account == null || string.IsNullOrWhiteSpace(account.ID))
                                {
                                    result = new JMessage()
                                    {
                                        data = $"[{input.Account}]账户不存在!",
                                        success = false,
                                        code = 500,
                                    };
                                    break;
                                }
                                else
                                {
                                    string flowId = PKeySigner.Pick(PKeyType.FlowAcc);
                                    //收款流水
                                    flows.Add(new FlowAccount()
                                    {
                                        ID = flowId,
                                        AccountMethord = AccountMethord.Receipt,
                                        AccountID = account.ID,
                                        Currency = account.Currency,
                                        Price = input.Amount,
                                        PaymentDate = input.ReceiptDate,
                                        FormCode = input.SeqNo,
                                        Currency1 = Currency.CNY,
                                        ERate1 = input.Rate,
                                        Price1 = input.Amount * input.Rate,
                                        CreatorID = input.CreatorID,
                                        TargetAccountName = input.Payer,
                                        PaymentMethord = (PaymentMethord)input.ReceiptType,     //收款方式
                                        CreateDate = DateTime.Now,
                                    });

                                    //收款
                                    lefts.Add(new PayeeLeft()
                                    {
                                        AccountCatalogID = input.FeeType,
                                        AccountCode = input.Account,
                                        AccountID = account.ID,
                                        PayerNature = (NatureType)input.AccountSource,
                                        PayerName = input.Payer,
                                        Currency = account.Currency,
                                        Price = input.Amount,
                                        Currency1 = Currency.CNY,
                                        ERate1 = input.Rate,
                                        Price1 = input.Amount * input.Rate,
                                        CreatorID = input.CreatorID,
                                        Summary = input.Summary,
                                        FlowID = flowId,
                                        CreateDate = DateTime.Now,
                                        Status = GeneralStatus.Normal,
                                    });

                                    //不包含付款人的话，新增付款人企业
                                    if (!epView.Any(item => item.Name == input.Payer.Trim()) && (NatureType)input.AccountSource == NatureType.Public)
                                    {
                                        eps.Add(new Enterprise()
                                        {
                                            Name = input.Payer.Trim(),
                                            Type = EnterpriseAccountType.Client,
                                            CreatorID = Npc.Robot.Obtain(),
                                            ModifierID = Npc.Robot.Obtain(),
                                            CreateDate = DateTime.Now,
                                            ModifyDate = DateTime.Now,
                                            Status = GeneralStatus.Normal,
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        result = new JMessage()
                        {
                            data = $"操作失败，不支持该操作!",
                            success = false,
                            code = 500,
                        };
                    }

                    if (result.success)
                    {
                        if (flows.Any())
                        {
                            flowsView.AddRange(flows);
                        }
                        if (lefts.Any())
                        {
                            payeeLefsView.AddRange(lefts);
                        }

                        if (eps.Any())
                        {
                            epView.AddRange(eps);
                        }

                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    result = new JMessage()
                    {
                        data = $"操作异常!{ex.Message}",
                        success = false,
                        code = 500,
                    };
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }


        #endregion

        #region 同步大赢家收款
        /// <summary>
        /// 同步大赢家收款
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SyncDyjPayeeLeft(int days, string numberId = "863", string key = "8c2b75ad115b467a8e976123033319f2")
        {
            var message = new JMessage() { code = 200, success = true, data = "操作成功!" };

            //获取大赢家接口数据
            var resultJson = Utils.Http.ApiHelper.Current.JPost(ConfigurationManager.AppSettings["DyjApiHost"] + "/api/ShouKuan/GetShouKuanList", new
            {
                uid = "",
                sdate = DateTime.Now.AddDays(-days).ToString("yyyy-MM-dd"),
                edate = DateTime.Now.ToString("yyyy-MM-dd"),
                pid = "",
                numberId = numberId,
                skTypeId = "",
                zdUid = "",
                key = key
            }).Replace("\\\"", "'").TrimStart('"').TrimEnd('"');

            var result = resultJson.JsonTo<DyjResultModel<PayeeLeftDyjDto>>();
            int count = 0;
            int existed_count = 0;
            int error_count = 0;

            if (result.isSuccess)
            {
                using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
                using (var payeeLefsView = new PayeeLeftsRoll(reponsitory))
                using (var accountsView = new AccountsRoll(reponsitory))
                using (var epView = new EnterprisesRoll(reponsitory))
                using (var adminView = new Yahv.Finance.Services.Views.AdminsTopView(reponsitory))
                using (var catalogsView = new AccountCatalogsRoll(reponsitory))
                {
                    PayeeLeft entity = null;

                    var accounts = accountsView.Where(item => item.Status == GeneralStatus.Normal).ToArray();
                    var admins = adminView.Where(item => item.Status != AdminStatus.Closed).ToArray();
                    var catalogs = catalogsView.Get(AccountCatalogType.Input.GetDescription(), AccountCatalogConduct.SupplyChain.GetDescription()).ToArray();
                    var payeeLefts = payeeLefsView.Where(item => item.CreateDate >= DateTime.Now.AddDays(-days * 2)).ToArray();

                    foreach (var input in result.data)
                    {
                        //默认值
                        input.收款类型 = "代收货款";
                        input.制单人 = input.制单人 == "孙月华" ? "杨艳梦" : input.制单人;        //孙月华的单子，对应Erp里边杨艳梦的账号

                        if (payeeLefts != null && payeeLefts.Any(item => item.FormCode == input.ID))
                        {
                            existed_count++;
                            continue;
                        }

                        try
                        {
                            //账户是否存在
                            var account = accounts.FirstOrDefault(item => item.DyjShortName == input.帐户);
                            if (account == null || string.IsNullOrWhiteSpace(account.ID))
                            {
                                error_count++;
                                Services.Oplogs.Oplog(input.制单人, LogModular.同步收款接口_大赢家_Api, Services.Oplogs.GetMethodInfo(), $"账户不存在!", input.Json(), url: Request.Url.ToString());
                                continue;
                            }

                            string flowId = PKeySigner.Pick(PKeyType.FlowAcc);
                            //收款流水
                            new FlowAccount()
                            {
                                ID = flowId,
                                AccountMethord = AccountMethord.Receipt,
                                AccountID = account.ID,
                                Currency = account.Currency,
                                Price = input.外币总金额,
                                PaymentDate = Convert.ToDateTime(input.制单日期),
                                FormCode = input.ID,
                                Currency1 = Currency.CNY,
                                ERate1 = (input.收款总金额 / input.外币总金额).Round(5),
                                Price1 = input.收款总金额,
                                CreatorID = admins.FirstOrDefault(item => item.RealName == input.制单人)?.ID,
                                TargetAccountName = input.付款单位,
                                PaymentMethord = Dyj2PaymentMethord(input.结算方式),     //收款方式
                            }.Enter();

                            //收款
                            entity = new PayeeLeft()
                            {
                                AccountCatalogID = catalogs.FirstOrDefault(item => item.Name == input.收款类型)?.ID,
                                AccountCode = account.Code,
                                AccountID = account.ID,
                                PayerNature = NatureType.Public,
                                PayerName = input.付款单位,
                                Currency = account.Currency,
                                Price = input.外币总金额,
                                Currency1 = Currency.CNY,
                                ERate1 = (input.收款总金额 / input.外币总金额).Round(5),
                                Price1 = input.收款总金额,
                                CreatorID = admins.FirstOrDefault(item => item.RealName == input.制单人)?.ID,
                                Summary = input.摘要,
                                FlowID = flowId,
                                AccountName = account.Name,
                            };
                            entity.AddAccountWork += PayeeLeft_AddAccountWork;
                            entity.Enter();

                            //不包含付款人的话，新增付款人企业
                            if (!epView.Any(item => item.Name == input.付款单位.Trim()))
                            {
                                new Enterprise()
                                {
                                    Name = input.付款单位.Trim(),
                                    Type = EnterpriseAccountType.Client,
                                    CreatorID = Npc.Robot.Obtain(),
                                    ModifierID = Npc.Robot.Obtain(),
                                }.Enter();
                            }

                            count++;
                            Services.Oplogs.Oplog(input.制单人, LogModular.同步收款接口_大赢家_Api, Services.Oplogs.GetMethodInfo(), $"[{input.ID}]同步成功!", input.Json(), url: Request.Url.ToString());
                        }
                        catch (Exception ex)
                        {
                            error_count++;
                            Services.Oplogs.Oplog(input.制单人, LogModular.同步收款接口_大赢家_Api, Services.Oplogs.GetMethodInfo(), $"[{input.ID}]同步异常!{ex.Message}", input.Json(), url: Request.Url.ToString());
                        }
                    }
                }
            }
            else
            {
                message.code = 500;
                message.success = false;
                message.data = result.message;
            }

            message.data = $"同步结束!一共{result.data.Count}条，成功{count}条，已存在{existed_count}条，失败{error_count}条。";
            return Json(message);
        }

        /// <summary>
        /// 大赢家结算方式转换
        /// </summary>
        private PaymentMethord Dyj2PaymentMethord(string method)
        {
            PaymentMethord result = PaymentMethord.BankTransfer;

            switch (method)
            {
                case "现金":
                    result = PaymentMethord.Cash;
                    break;
                case "支票":
                    result = PaymentMethord.Cheque;
                    break;
                case "电汇":
                    result = PaymentMethord.BankTransfer;
                    break;
                case "承兑":
                    result = PaymentMethord.BankAcceptanceBill;
                    break;
                case "转账":
                    result = PaymentMethord.BankTransfer;
                    break;
                case "同城划账":
                case "内部转账":
                case "预收(付)转应收(付)":
                    break;
                default:
                    break;
            }

            return result;
        }

        /// <summary>
        /// 添加认领数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PayeeLeft_AddAccountWork(object sender, Usually.SuccessEventArgs e)
        {
            var entity = sender as PayeeLeft;
            AccountWork model = new AccountWork();
            try
            {
                model.PayeeLeftID = entity.ID;
                model.Conduct = Conduct.Chain.GetDescription();
                model.Enter();
                Services.Oplogs.Oplog(entity.CreatorID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), "新增", model.Json());
            }
            catch (Exception ex)
            {
                Services.Oplogs.Oplog(entity.CreatorID, LogModular.收款认领, Services.Oplogs.GetMethodInfo(), "新增异常!" + ex.Message, model.Json());
            }
        }
        #endregion

        #region 收款核销
        /// <summary>
        /// 收款核销
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult PayeeRightEnter(InputParam<PayeeRightInputDto> param)
        {
            var result = new JMessage() { code = 200, success = false, data = "操作成功!" };
            var input = param.Model;

            try
            {
                using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
                using (var leftView = new PayeeLeftsRoll(repository))
                using (var rightView = new PayeeRightsRoll(repository))
                {
                    //获取银行流水
                    var entity = leftView.FirstOrDefault(item => item.FormCode == input.SeqNo);
                    if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
                    {
                        result = new JMessage()
                        {
                            data = $"[{input.SeqNo}]流水不存在!",
                            success = false,
                            code = 500,
                        };
                    }
                    else
                    {
                        //银行流水余额是否够核销
                        var balance = rightView.GetWriteOffBalance(input.SeqNo) - input.Amount;
                        if (balance < 0)
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.SeqNo}]核销金额不足!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];

                            var right = new PayeeRight()
                            {
                                Currency = entity.Currency,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                PayeeLeftID = entity.ID,
                                ERate1 = rate,
                                Price = input.Amount,
                                SenderID = param.Sender,
                                Price1 = input.Amount * rate,
                                AccountCatalogID = input.FeeType,
                            };
                            right.Enter();
                        }
                    }
                }

                //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款核销_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                return Json(result);
            }
            catch (Exception ex)
            {
                result = new JMessage()
                {
                    data = $"操作异常!{ex.Message}",
                    success = false,
                    code = 500,
                };
                //Services.Oplogs.Oplog(input.CreatorID, LogModular.收款核销_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                return Json(result);
            }
        }
        #endregion
    }
}