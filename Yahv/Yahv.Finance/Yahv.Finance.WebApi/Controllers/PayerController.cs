using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Data.Sqls.PvFinance;
using Layers.Linq;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Finance.WebApi.Filter;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;
using FilesDescription = Yahv.Finance.Services.Models.Origins.FilesDescription;
using FilesMap = Yahv.Finance.Services.Models.Origins.FilesMap;
using FlowAccount = Yahv.Finance.Services.Models.Origins.FlowAccount;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 付款
    /// </summary>
    public class PayerController : ClientController
    {
        #region 货款
        /// <summary>
        /// 货款(调拨)
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [SenderAuthorize]
        public ActionResult ProductFeeEnter(InputParam<PayerInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            PayerInputDto input = new PayerInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var accountsView = new AccountsRoll(repository))
            {
                try
                {
                    input = param.Model;
                    //付款账号
                    var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.OutAccountNo);
                    string payerAccountCode = payerAccount?.Code;

                    //中间账户 外币账户
                    var midAccountNo = input.MidAccountNo;

                    //供应商账户
                    var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.InAccountNo);


                    //收款账户对付款账户汇率（人民币账户对供应商 or 中间账户对供应商）
                    decimal currentRate = input.Rate;
                    //本位币汇率 收款账户的本位币汇率
                    decimal currentRate1 = 1;
                    //人民币账户对中间账户（人民币） or 中间账户对外币中户（外币）
                    var currency = Currency.CNY;
                    //付款金额  人民币对中间账户 or 中间账户对供应商账户
                    decimal price = input.RMBAmount;
                    //人民币对供应商账户（付款流水号） or   中间账户对供应商账户（付款流水号）
                    string seqNo = input.OutSeqNo;

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        //检查付款账户是否存在
                        if (payerAccount == null || string.IsNullOrEmpty(payerAccount.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.OutAccountNo}]付款账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else if (payeeAccount == null || string.IsNullOrEmpty(payeeAccount?.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.OutAccountNo}]收款账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(midAccountNo))
                            {
                                var midAccount = accountsView.FirstOrDefault(item => item.Code == midAccountNo);

                                if (midAccount == null)
                                {
                                    result = new JMessage()
                                    {
                                        data = $"[{input.OutAccountNo}]中间账户不存在!",
                                        success = false,
                                        code = 500,
                                    };
                                }
                                else
                                {
                                    //人民币账户到外币账户（中间账户）    
                                    FundTransferEnter(new InputParam<FundTransferInputDto>()
                                    {
                                        Sender = param.Sender,
                                        Option = OptionConsts.insert,
                                        Model = new FundTransferInputDto()
                                        {
                                            CreatorID = input.CreatorID,
                                            Rate = input.Rate,
                                            FeeType = ((int)FundTransferType.ProductFee).ToString(),
                                            PaymentType = input.PaymentType,
                                            PaymentDate = input.PaymentDate,

                                            OutAmount = Math.Abs(input.RMBAmount),
                                            OutCurrency = Currency.CNY.GetCurrency().ShortName,
                                            OutSeqNo = input.OutSeqNo,
                                            OutAccountNo = input.OutAccountNo,

                                            InAmount = input.Amount,
                                            InCurrency = input.Currency,
                                            InSeqNo = input.MidInSeqNo,
                                            InAccountNo = input.MidAccountNo,
                                        }
                                    });

                                    //如果存在中间账户，付款账户则转为中间账户
                                    payerAccountCode = input.MidAccountNo;
                                    //中间账户 对 外币
                                    currentRate = 1;
                                    //中间账户 的本位币汇率
                                    currentRate1 = ExchangeRates.Universal[currency, Currency.CNY];
                                    //中间账户对供应商账户的外币金额
                                    price = input.Amount;
                                    //中间账户 币种
                                    currency = CurrencyHelper.GetCurrency(input.Currency);
                                    //付款流水号
                                    seqNo = input.MidOutSeqNo;
                                }
                            }

                            //付款账户（人民币账户or中间账户）到 供应商账户    
                            FundTransferEnter(new InputParam<FundTransferInputDto>()
                            {
                                Sender = param.Sender,
                                Option = OptionConsts.insert,
                                Model = new FundTransferInputDto()
                                {
                                    CreatorID = input.CreatorID,
                                    Rate = currentRate,
                                    FeeType = ((int)FundTransferType.ProductFee).ToString(),
                                    PaymentType = input.PaymentType,
                                    PaymentDate = input.PaymentDate,

                                    OutAmount = Math.Abs(price),
                                    OutCurrency = currency.GetCurrency().ShortName,
                                    OutSeqNo = seqNo,
                                    OutAccountNo = payerAccountCode,

                                    InAmount = input.Amount,
                                    InCurrency = input.Currency,
                                    InSeqNo = input.InSeqNo,
                                    InAccountNo = input.InAccountNo,
                                }
                            });


                            #region 手续费
                            //手续费
                            if (input.Poundage != null && input.Poundage > 0)
                            {
                                var accuntCatalogsView = new Yahv.Finance.Services.Views.Rolls.AccountCatalogsRoll();
                                ChargeEnter(new InputParam<ChargeInputDto>()
                                {
                                    Sender = param.Sender,
                                    Option = param.Option,
                                    Model = new ChargeInputDto()
                                    {
                                        CreatorID = input.CreatorID,
                                        Currency = Currency.CNY.GetCurrency().ShortName,
                                        Rate = 1,

                                        PaymentDate = input.PaymentDate,
                                        PaymentType = input.PaymentType,
                                        AccountNo = input.OutAccountNo,
                                        MoneyType = 2,
                                        ReceiveAccountNo = null,

                                        Amount = input.Poundage.Value,
                                        SeqNo = input.PoundageSeqNo,
                                        FeeItems = new List<ChargeItemDto>()
                                        {
                                            new ChargeItemDto()
                                            {
                                                Amount = input.Poundage.Value,
                                                FeeType = accuntCatalogsView.Get("付款类型","综合业务","费用")
                                                    .FirstOrDefault(item=>item.Name=="银行手续费")?.ID
                                            }
                                        }
                                    }
                                });
                            }
                            #endregion
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

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.货款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.货款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }

        /// <summary>
        /// 货款
        /// </summary>
        /// <param name="param"></param>
        /// <remarks>付款给真实的供应商</remarks>
        /// <returns></returns>
        [SenderAuthorize]
        public ActionResult PaymentEnter(InputParam<PaymentInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            PaymentInputDto input = new PaymentInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var accountsView = new AccountsRoll(repository))
            {
                try
                {
                    input = param.Model;
                    var payerAccountID = accountsView.FirstOrDefault(item => item.Code == input.PayerAccount)?.ID;
                    var payeeAccountID = GetOrAddPayeeAccount(repository, accountsView, input).ID;

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        //检查付款账户是否存在
                        if (string.IsNullOrEmpty(payerAccountID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.PayerAccount}]付款账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            var rate1 = ExchangeRates.Universal[CurrencyHelper.GetCurrency(input.PayerCurrency), Currency.CNY];
                            var catalogId = new AccountCatalogsRoll().GetID("付款类型", "供应链业务", "货款");

                            //添加付款申请
                            var apply = new PayerApply()
                            {
                                Currency = CurrencyHelper.GetCurrency(input.PayerCurrency),
                                ApplierID = input.CreatorID,
                                CreatorID = input.CreatorID,
                                IsPaid = false,
                                Price = input.PayerAmount,
                                Status = ApplyStauts.Completed,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                SenderID = SystemSender.Xindatong.GetFixedID(),
                            };
                            apply.Enter();

                            //添加应付
                            var payerLeft = new PayerLeft()
                            {
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Currency = CurrencyHelper.GetCurrency(input.PayerCurrency),
                                ApplyID = apply.ID,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = rate1,
                                Price1 = (input.PayerAmount * rate1).Round(),
                                Price = input.PayerAmount,
                                Status = GeneralStatus.Normal,
                                AccountCatalogID = catalogId,
                            };
                            payerLeft.Enter();

                            //添加付款流水
                            string flowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                            var flowAccount = new FlowAccount()
                            {
                                ID = flowID,
                                Currency = CurrencyHelper.GetCurrency(input.PayerCurrency),
                                CreateDate = DateTime.Now,
                                AccountID = payerAccountID,
                                AccountMethord = AccountMethord.Payment,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = rate1,
                                FormCode = input.SeqNo,
                                TargetAccountCode = input.PayeeAccount,
                                TargetAccountName = input.PayeeName,
                                TargetRate = 1 / input.Rate,        //芯达通是收款对付款汇率，中心记录的是付款对收款
                                PaymentDate = input.PaymentDate,
                                Price = -input.PayerAmount,
                                Price1 = -(input.PayerAmount * rate1).Round(),
                                PaymentMethord = (PaymentMethord)input.PaymentType,
                            };
                            flowAccount.Enter();

                            //添加应付核销
                            var payerRight = new PayerRight()
                            {
                                Currency = CurrencyHelper.GetCurrency(input.PayerCurrency),
                                CreateDate = DateTime.Now,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                FlowID = flowID,
                                ERate1 = input.Rate,
                                PayerLeftID = payerLeft.ID,
                                Price = input.PayerAmount,
                                Price1 = (input.PayerAmount * rate1).Round()
                            };
                            payerRight.Enter();
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

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.货款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.货款接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }
        #endregion

        #region 费用
        /// <summary>
        /// 费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [SenderAuthorize]
        public ActionResult ChargeEnter(InputParam<ChargeInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            ChargeInputDto input = new ChargeInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var accountsView = new AccountsRoll(repository))
            using (var flowsView = new FlowAccountsRoll(repository))
            using (var chargeAppliesView = new ChargeAppliesRoll(repository))
            using (var chargeItemsView = new ChargeApplyItemsRoll(repository))
            {
                try
                {
                    input = param.Model;
                    var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.AccountNo);
                    var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.ReceiveAccountNo);
                    string flowId = string.Empty;

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        var payerAccountID = payerAccount?.ID;
                        var payeeAccountID = payeeAccount?.ID;
                        var currency = CurrencyHelper.GetCurrency(input.Currency);

                        //检查付款账户是否存在
                        if (string.IsNullOrEmpty(payerAccountID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.AccountNo}]付款账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        //else if (string.IsNullOrEmpty(payeeAccountID))
                        //{
                        //    result = new JMessage()
                        //    {
                        //        data = $"[{input.ReceiveAccountNo}]收款账户不存在!",
                        //        success = false,
                        //        code = 500,
                        //    };
                        //}
                        else if (payerAccount.Currency != currency)
                        {
                            result = new JMessage()
                            {
                                data = $"币种异常!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            //添加费用申请
                            var apply = new ChargeApply()
                            {
                                Currency = CurrencyHelper.GetCurrency(input.Currency),
                                ApplierID = input.CreatorID,
                                CreatorID = input.CreatorID,
                                Price = input.Amount,
                                Status = ApplyStauts.Completed,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                SenderID = SystemSender.Xindatong.GetFixedID(),
                                Type = CostApplyType.Normal,
                                ApproverID = GetDefaultApproverID(),
                            };
                            apply.Enter();

                            //费用项不能为空
                            if (input.FeeItems == null || input.FeeItems.Count <= 0)
                            {
                                result = new JMessage()
                                {
                                    data = $"[{input.ReceiveAccountNo}]费用项不能为空!",
                                    success = false,
                                    code = 500,
                                };
                            }
                            else
                            {
                                string flowID = string.Empty;
                                FlowAccount flowAccount;
                                List<ChargeApplyItem> applyItems = new List<ChargeApplyItem>();

                                foreach (var chargeItemDto in input.FeeItems)
                                {
                                    //添加费用流水
                                    flowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                                    flowAccount = new FlowAccount()
                                    {
                                        ID = flowID,
                                        Currency = currency,
                                        CreateDate = DateTime.Now,
                                        AccountID = payerAccountID,
                                        AccountMethord = AccountMethord.Payment,
                                        CreatorID = input.CreatorID,
                                        Currency1 = Currency.CNY,
                                        ERate1 = input.Rate,
                                        FormCode = input.SeqNo,
                                        TargetAccountCode = input.ReceiveAccountNo,
                                        TargetAccountName = payeeAccount?.Name,
                                        PaymentDate = input.PaymentDate,
                                        Price = -chargeItemDto.Amount,
                                        Price1 = -(chargeItemDto.Amount * input.Rate).Round(),
                                        PaymentMethord = (PaymentMethord)input.PaymentType,
                                    };
                                    flowAccount.Add();

                                    applyItems.Add(new ChargeApplyItem()
                                    {
                                        AccountCatalogID = chargeItemDto.FeeType,
                                        Status = ApplyItemStauts.Paid,
                                        ApplyID = apply.ID,
                                        IsPaid = input.MoneyType == 2,
                                        ExpectedTime = input.PaymentDate,
                                        Price = chargeItemDto.Amount,
                                        FlowID = flowID,
                                        CreateDate = DateTime.Now,
                                        ModifyDate = DateTime.Now,
                                        Summary = chargeItemDto.FeeDesc,
                                    });
                                }

                                //费用申请项
                                if (applyItems.Count > 0)
                                {
                                    new Yahv.Finance.Services.Views.Rolls.ChargeApplyItemsRoll(repository).InsertOrUpdate(apply.ID, applyItems);
                                }

                                //费用附件
                                FilesSync(input.Files, input.CreatorID, apply.ID);
                            }
                        }
                    }
                    else if (param.Option.ToLower() == OptionConsts.update)
                    {
                        var payerAccountID = payerAccount?.ID;

                        //检查付款账户是否存在
                        if (string.IsNullOrEmpty(payerAccountID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.AccountNo}]付款账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            //根据流水号和付款账户 定位流水ID
                            var flow = flowsView.FirstOrDefault(item =>
                                item.FormCode == input.OldSeqNo && item.AccountID == payerAccountID);
                            flowId = flow?.ID;

                            if (string.IsNullOrEmpty(flowId))
                            {
                                result = new JMessage()
                                {
                                    data = $"[{input.OldSeqNo}]流水号不存在!",
                                    success = false,
                                    code = 500,
                                };
                            }
                            else
                            {
                                var applyItem = chargeItemsView.FirstOrDefault(item => item.FlowID == flowId);
                                var apply = chargeAppliesView.FirstOrDefault(item => item.ID == applyItem.ApplyID);

                                if (applyItem == null || apply == null)
                                {
                                    result = new JMessage()
                                    {
                                        data = $"[{input.OldSeqNo}]申请或申请项不存在!",
                                        success = false,
                                        code = 500,
                                    };
                                }
                                else
                                {
                                    var diff = input.Amount - (applyItem?.Price ?? 0);
                                    apply.Price = apply.Price + diff;
                                    apply.Enter();

                                    applyItem.Price = input.Amount;
                                    applyItem.Enter();

                                    flow.Price = -input.Amount;
                                    flow.Price1 = -input.Amount * flow.ERate1;
                                    flow.FormCode = input.SeqNo;
                                    flow.Enter();


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
                        tran.Commit();

                        if (param.Option.ToLower() == OptionConsts.update)
                        {
                            //重新计算余额
                            flowsView.Recalculation(payerAccount.ID, flowId);
                        }
                    }
                    else
                    {
                        tran.Rollback();
                    }

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.费用接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }

        /// <summary>
        /// 费用
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [SenderAuthorize]
        public ActionResult ChargeBatchEnter(InputsParam<ChargeInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            List<ChargeInputDto> inputs = new List<ChargeInputDto>();
            string creatorId = default(string);

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var accountsView = new AccountsRoll(repository))
            {
                try
                {
                    inputs = param.Model.ToList();
                    var idsUpdate = new List<string>();

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        var applyIds = PKeySigner.Series(Yahv.Finance.Services.PKeyType.ChargeApply, inputs.Count, orderBy: PKeySigner.OrderBy.Ascending);
                        var applys = new List<Layers.Data.Sqls.PvFinance.ChargeApplies>();
                        var applyItems = new List<Layers.Data.Sqls.PvFinance.ChargeApplyItems>();
                        var flows = new List<Layers.Data.Sqls.PvFinance.FlowAccounts>();



                        for (int m = 0; m < inputs.Count; m++)
                        {
                            var input = inputs[m];
                            creatorId = input.CreatorID;

                            var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.AccountNo);
                            var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.ReceiveAccountNo);

                            var payerAccountID = payerAccount?.ID;
                            var payeeAccountID = payeeAccount?.ID;
                            var currency = CurrencyHelper.GetCurrency(input.Currency);

                            //检查付款账户是否存在
                            if (string.IsNullOrEmpty(payerAccountID))
                            {
                                result.data = result.data + $"[{input.AccountNo}]付款账户不存在!";
                                continue;
                            }
                            else if (payerAccount.Currency != currency)
                            {
                                result.data = result.data + $"[{input.AccountNo}]币种异常!";
                                continue;
                            }
                            else
                            {
                                //添加费用申请
                                var apply = new Layers.Data.Sqls.PvFinance.ChargeApplies()
                                {
                                    ID = applyIds[m],
                                    PayeeAccountID = payeeAccountID,
                                    PayerAccountID = payerAccountID,
                                    Type = (int)CostApplyType.Normal,
                                    IsImmediately = false,
                                    Currency = (int)CurrencyHelper.GetCurrency(input.Currency),
                                    Price = input.Amount,
                                    //Summary = this.Summary,
                                    //CallBackUrl = this.CallBackUrl,
                                    //CallBackID = this.CallBackID,
                                    SenderID = SystemSender.Xindatong.GetFixedID(),
                                    ApplierID = input.CreatorID,
                                    //ExcuterID = this.ExcuterID,
                                    CreatorID = input.CreatorID,
                                    CreateDate = DateTime.Now,
                                    ApproverID = GetDefaultApproverID(),
                                    Status = (int)ApplyStauts.Completed,
                                    PayerID = payerAccount.EnterpriseID,
                                };

                                //费用项不能为空
                                if (input.FeeItems == null || input.FeeItems.Count <= 0)
                                {
                                    result = new JMessage()
                                    {
                                        data = $"[{input.ReceiveAccountNo}]费用项不能为空!",
                                        success = false,
                                        code = 500,
                                    };
                                }
                                else
                                {
                                    //添加申请
                                    applys.Add(apply);

                                    var flowIds = PKeySigner.Series(Yahv.Finance.Services.PKeyType.FlowAcc, input.FeeItems.Count);
                                    var itemsIds = PKeySigner.Series(Yahv.Finance.Services.PKeyType.ChargeApplyItem, input.FeeItems.Count);

                                    for (int i = input.FeeItems.Count - 1; i >= 0; i--)
                                    {
                                        var chargeItemDto = input.FeeItems[i];

                                        //添加费用流水
                                        flows.Add(new FlowAccounts()
                                        {
                                            ID = flowIds[i],
                                            AccountMethord = (int)AccountMethord.Payment,
                                            AccountID = payerAccountID,
                                            Currency = (int)currency,
                                            Price = -chargeItemDto.Amount,
                                            //Balance = this.Balance,
                                            PaymentDate = input.PaymentDate,
                                            FormCode = input.SeqNo,
                                            Currency1 = (int)Currency.CNY,
                                            ERate1 = input.Rate,
                                            Price1 = -(chargeItemDto.Amount * input.Rate).Round(),
                                            //Balance1 = this.Balance1,
                                            CreatorID = input.CreatorID,
                                            CreateDate = DateTime.Now,
                                            TargetAccountName = payeeAccount?.Name,
                                            TargetAccountCode = input.ReceiveAccountNo,
                                            PaymentMethord = input.PaymentType,
                                            TargetRate = 1,
                                            Type = 1,
                                        });

                                        applyItems.Add(new ChargeApplyItems()
                                        {
                                            ID = itemsIds[i],
                                            ApplyID = apply.ID,
                                            IsPaid = input.MoneyType == 2,
                                            ExpectedTime = input.PaymentDate,
                                            AccountCatalogID = chargeItemDto.FeeType,
                                            Price = chargeItemDto.Amount,
                                            Summary = chargeItemDto.FeeDesc,
                                            FlowID = flowIds[i],
                                            CreateDate = DateTime.Now,
                                            ModifyDate = DateTime.Now,
                                            Status = (int)GeneralStatus.Normal,
                                        });

                                        idsUpdate.Add(flowIds[i]);
                                    }
                                }
                            }
                        }

                        //添加申请
                        if (applys.Count > 0)
                        {
                            repository.Insert((IEnumerable<Layers.Data.Sqls.PvFinance.ChargeApplies>)applys);
                        }
                        //添加流水
                        if (flows.Count > 0)
                        {
                            repository.Insert((IEnumerable<Layers.Data.Sqls.PvFinance.FlowAccounts>)flows);
                        }

                        //费用申请项
                        if (applyItems.Count > 0)
                        {
                            repository.Insert((IEnumerable<Layers.Data.Sqls.PvFinance.ChargeApplyItems>)applyItems);
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

                        //Task.Run(() =>
                        //{
                        //    //触发器 重新计算余额
                        //    if (idsUpdate.Count > 0)
                        //    {
                        //        foreach (var id in idsUpdate)
                        //        {
                        //            repository.Update<Layers.Data.Sqls.PvFinance.FlowAccounts>(new
                        //            {
                        //                Balance = 0,
                        //            }, item => item.ID == id);
                        //        }
                        //    }
                        //});
                    }
                    else
                    {
                        tran.Rollback();
                    }

                    //Services.Oplogs.Oplog(creatorId, LogModular.费用接口_批量_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                    //Services.Oplogs.Oplog(creatorId, LogModular.费用接口_批量_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }
        #endregion

        #region 调拨
        /// <summary>
        /// 调拨
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [SenderAuthorize]
        public ActionResult FundTransferEnter(InputParam<FundTransferInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            FundTransferInputDto input = new FundTransferInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var accountsView = new AccountsRoll(repository))
            {
                try
                {
                    input = param.Model;
                    //调出账户
                    var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.OutAccountNo);
                    var payerAccountID = payerAccount?.ID;
                    //调入账户
                    var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.InAccountNo);
                    var payeeAccountID = payeeAccount?.ID;

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        //检查付款账户是否存在
                        if (string.IsNullOrEmpty(payerAccountID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.OutAccountNo}]调出账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else if (string.IsNullOrEmpty(payeeAccountID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.InAccountNo}]调入账户不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            var payerCurrency = CurrencyHelper.GetCurrency(input.OutCurrency);
                            var payeeCurrency = CurrencyHelper.GetCurrency(input.InCurrency);

                            var payeeRate = ExchangeRates.Universal[payeeCurrency, Currency.CNY];
                            var payerRate = ExchangeRates.Universal[payerCurrency, Currency.CNY];

                            var payerFlowId = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);
                            var payeeFlowId = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                            //如果包含贴现利息的话，扣除调贴现利息的金额；芯达通默认调出金额包含贴现利息
                            decimal payerPrice = (input?.DiscountInterest > 0 ? input.OutAmount - input.DiscountInterest : input.OutAmount) ?? 0;
                            //如果包含快捷支付手续费，也需要扣除快捷支付手续费；芯达通默认调出金额包含快捷支付手续费
                            payerPrice = (input?.QRCodeFee > 0 ? payerPrice - input.QRCodeFee : payerPrice) ?? 0;
                            var payeePrice = input.InAmount;

                            //添加调拨申请
                            var apply = new SelfApply()
                            {
                                Currency = payerCurrency,
                                ApplierID = input.CreatorID,
                                CreatorID = input.CreatorID,
                                Status = ApplyStauts.Completed,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                SenderID = SystemSender.Xindatong.GetFixedID(),
                                Price = payerPrice,
                                TargetCurrency = payeeCurrency,
                                TargetERate = input.Rate,
                                TargetPrice = payeePrice,
                                ApproverID = GetDefaultApproverID(),
                            };
                            apply.Enter();

                            #region 调出
                            //调出
                            var payerLeft = new SelfLeft()
                            {
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Currency = payerCurrency,
                                ApplyID = apply.ID,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = payerRate,
                                Price1 = (payerPrice * payerRate).Round(),
                                Price = payerPrice,
                                Status = GeneralStatus.Normal,
                                AccountCatalogID = input.FeeType,
                                CreateDate = DateTime.Now,
                                AccountMethord = AccountMethord.Output,
                            };
                            payerLeft.Enter();

                            //调出流水
                            //var payerFlow = new FlowAccount()
                            //{
                            //    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                            //    Currency = payerCurrency,
                            //    CreateDate = DateTime.Now,
                            //    AccountID = payerAccountID,
                            //    AccountMethord = AccountMethord.Output,
                            //    CreatorID = input.CreatorID,
                            //    Currency1 = Currency.CNY,
                            //    ERate1 = payerRate,
                            //    FormCode = input.OutSeqNo,
                            //    TargetAccountCode = payeeAccount?.Code,
                            //    TargetAccountName = payeeAccount?.Name,
                            //    PaymentDate = input.PaymentDate,
                            //    Price = -payerPrice,
                            //    Price1 = -(payerPrice * payerRate).Round(),
                            //    PaymentMethord = (PaymentMethord)input.PaymentType,
                            //};
                            //payerFlow.Enter();
                            repository.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                            {
                                ID = payerFlowId,
                                AccountMethord = (int)AccountMethord.Output,
                                AccountID = payerAccountID,
                                Currency = (int)payerCurrency,
                                Price = -payerPrice,
                                PaymentDate = input.PaymentDate,
                                FormCode = input.OutSeqNo,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = payerRate,
                                Price1 = -(payerPrice * payerRate).Round(),
                                CreatorID = input.CreatorID,
                                CreateDate = DateTime.Now,
                                TargetAccountName = payeeAccount?.Name,
                                TargetAccountCode = payeeAccount?.Code,
                                PaymentMethord = (int)(PaymentMethord)input.PaymentType,
                                Type = 1,
                            });


                            //调出核销
                            var payerRight = new SelfRight()
                            {
                                SelfLeftID = payerLeft.ID,
                                FlowID = payerFlowId,
                                CreateDate = DateTime.Now,
                                CreatorID = input.CreatorID,
                                OriginCurrency = payerCurrency,
                                OriginPrice = payerPrice,
                                Rate = input.Rate,
                                TargetCurrency = payeeCurrency,
                                TargetPrice = payeePrice,
                                OriginCurrency1 = Currency.CNY,
                                OriginERate1 = payerRate,
                                OriginPrice1 = (payerPrice * payerRate).Round(),
                                TargetCurrency1 = Currency.CNY,
                                TargetERate1 = payeeRate,
                                TargetPrice1 = (payeePrice * payeeRate).Round(),
                            };
                            payerRight.Enter();
                            #endregion

                            #region 调入
                            //调入
                            var payeeLeft = new SelfLeft()
                            {
                                AccountMethord = AccountMethord.Input,
                                CreateDate = DateTime.Now,
                                Currency = payeeCurrency,
                                AccountCatalogID = input.FeeType,
                                ApplyID = apply.ID,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = payeeRate,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Price = payeePrice,
                                Price1 = (payeePrice * payeeRate).Round(),
                                Status = GeneralStatus.Normal,
                            };
                            payeeLeft.Enter();

                            //调入流水
                            //var payeeFlow = new FlowAccount()
                            //{
                            //    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                            //    Currency = payeeCurrency,
                            //    CreateDate = DateTime.Now,
                            //    AccountID = payeeAccountID,
                            //    AccountMethord = AccountMethord.Input,
                            //    CreatorID = input.CreatorID,
                            //    Currency1 = Currency.CNY,
                            //    ERate1 = payeeRate,
                            //    FormCode = input.InSeqNo,
                            //    TargetAccountCode = payerAccount.Code,
                            //    TargetAccountName = payerAccount.Name,
                            //    PaymentDate = input.PaymentDate,
                            //    Price = payeePrice,
                            //    Price1 = (payeePrice * payeeRate).Round(),
                            //    PaymentMethord = (PaymentMethord)input.PaymentType,
                            //};
                            //payeeFlow.Enter();
                            repository.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                            {
                                ID = payeeFlowId,
                                AccountMethord = (int)AccountMethord.Input,
                                AccountID = payeeAccountID,
                                Currency = (int)payeeCurrency,
                                Price = payeePrice,
                                PaymentDate = input.PaymentDate,
                                FormCode = input.InSeqNo,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = payeeRate,
                                Price1 = (payeePrice * payeeRate).Round(),
                                CreatorID = input.CreatorID,
                                CreateDate = DateTime.Now,
                                TargetAccountName = payerAccount.Name,
                                TargetAccountCode = payerAccount.Code,
                                PaymentMethord = (int)(PaymentMethord)input.PaymentType,
                                Type = 1,
                            });

                            //调入核销
                            var payeeRight = new SelfRight()
                            {
                                CreateDate = DateTime.Now,
                                CreatorID = input.CreatorID,
                                FlowID = payeeFlowId,
                                OriginCurrency = payeeCurrency,
                                OriginCurrency1 = Currency.CNY,
                                OriginERate1 = payeeRate,
                                OriginPrice = payeePrice,
                                OriginPrice1 = (payeePrice * payeeRate).Round(),
                                Rate = 1,       //调入核销 源币种和目标币种一致
                                SelfLeftID = payeeLeft.ID,
                                TargetCurrency = payeeCurrency,
                                TargetCurrency1 = Currency.CNY,
                                TargetERate1 = payeeRate,
                                TargetPrice = payeePrice,
                                TargetPrice1 = (payeePrice * payeeRate).Round(),
                            };
                            payeeRight.Enter();
                            #endregion
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

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    //return Json(result);
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
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.调拨接口_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }

            #region 贴现利息
            var accuntCatalogsView = new Yahv.Finance.Services.Views.Rolls.AccountCatalogsRoll();
            //贴现利息
            if (input.DiscountInterest != null && input.DiscountInterest > 0)
            {
                ChargeEnter(new InputParam<ChargeInputDto>()
                {
                    Sender = param.Sender,
                    Option = param.Option,
                    Model = new ChargeInputDto()
                    {
                        CreatorID = input.CreatorID,
                        Currency = input.OutCurrency,
                        Rate = input.Rate,
                        PaymentDate = DateTime.Parse(input.PaymentDate?.ToString()),
                        PaymentType = input.PaymentType,
                        AccountNo = input.OutAccountNo,
                        MoneyType = 2,
                        ReceiveAccountNo = null,

                        SeqNo = input.DiscountSeqNo,
                        Amount = input.DiscountInterest.Value,
                        FeeItems = new List<ChargeItemDto>()
                                    {
                                        new ChargeItemDto()
                                        {
                                            Amount = input.DiscountInterest.Value,
                                            FeeType = accuntCatalogsView.Get("付款类型","综合业务","费用")
                                                .FirstOrDefault(item=>item.Name=="贴现利息")?.ID
                                        }
                                    }
                    }
                });
            }
            #endregion

            #region 手续费
            //手续费
            if (input.Poundage != null && input.Poundage > 0)
            {
                ChargeEnter(new InputParam<ChargeInputDto>()
                {
                    Sender = param.Sender,
                    Option = param.Option,
                    Model = new ChargeInputDto()
                    {
                        CreatorID = input.CreatorID,
                        Currency = input.OutCurrency,
                        Rate = input.Rate,

                        PaymentDate = DateTime.Parse(input.PaymentDate?.ToString()),
                        PaymentType = input.PaymentType,
                        AccountNo = input.OutAccountNo,
                        MoneyType = 2,
                        ReceiveAccountNo = null,

                        Amount = input.Poundage.Value,
                        SeqNo = input.PoundageSeqNo,
                        FeeItems = new List<ChargeItemDto>()
                                    {
                                        new ChargeItemDto()
                                        {
                                            Amount = input.Poundage.Value,
                                            FeeType = accuntCatalogsView.Get("付款类型","综合业务","费用")
                                                .FirstOrDefault(item=>item.Name=="银行手续费")?.ID
                                        }
                                    }
                    }
                });
            }
            #endregion

            #region 手续费
            //手续费
            if (input.QRCodeFee != null && input.QRCodeFee > 0)
            {
                ChargeEnter(new InputParam<ChargeInputDto>()
                {
                    Sender = param.Sender,
                    Option = param.Option,
                    Model = new ChargeInputDto()
                    {
                        CreatorID = input.CreatorID,
                        Currency = input.OutCurrency,
                        Rate = input.Rate,

                        PaymentDate = DateTime.Parse(input.PaymentDate?.ToString()),
                        PaymentType = input.PaymentType,
                        AccountNo = input.OutAccountNo,
                        MoneyType = 2,
                        ReceiveAccountNo = null,

                        Amount = input.QRCodeFee.Value,
                        SeqNo = input.QRCodeFeeSeqNo,
                        FeeItems = new List<ChargeItemDto>()
                        {
                            new ChargeItemDto()
                            {
                                Amount = input.QRCodeFee.Value,
                                FeeType = accuntCatalogsView.Get("付款类型","综合业务","费用")
                                    .FirstOrDefault(item=>item.Name=="银行手续费")?.ID
                            }
                        }
                    }
                });
            }
            #endregion

            return Json(result);
        }
        #endregion

        #region 私有函数

        #region 获取或新增收款账户

        /// <summary>
        /// 获取或新增收款账户
        /// </summary>
        private Services.Models.Origins.Account GetOrAddPayeeAccount(PvFinanceReponsitory reponsitory, AccountsRoll accountsView, PaymentInputDto input)
        {
            var account = accountsView.FirstOrDefault(item => item.Code == input.PayeeAccount);

            if (account != null && !string.IsNullOrEmpty(account.ID))
            {
                return account;
            }
            else
            {
                string id = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Account);
                account = new Services.Models.Origins.Account()
                {
                    ID = id,
                    NatureType = NatureType.Public,
                    Name = input.PayeeName,
                    BankName = input.PayeeBankName,
                    Code = input.PayeeAccount,
                    ManageType = ManageType.Normal,
                    Currency = CurrencyHelper.GetCurrency(input.PayeeCurrency),
                    IsHaveU = false,
                    CreatorID = input.CreatorID,
                    ModifierID = input.CreatorID,
                    Source = AccountSource.Simple,
                };

                //企业不存在 添加企业
                var enterprise = new EnterprisesRoll().Where(t => t.Name == input.PayeeName
                                                                && t.Status == GeneralStatus.Normal).FirstOrDefault();
                if (enterprise == null)
                {
                    //要新增一个 Enterprise
                    enterprise = new Services.Models.Origins.Enterprise()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Enterprise),
                        Name = input.PayeeName,
                        Type = EnterpriseAccountType.Supplier,
                        CreatorID = input.CreatorID,
                        ModifierID = input.CreatorID,
                    };
                    //保存
                    enterprise.Enter();
                }

                account.EnterpriseID = enterprise.ID;
                account.Enter();
            }

            return account;
        }

        #endregion

        #region 根据地址同步费用附件
        private void FilesSync(List<CenterFeeFile> files, string creatorID, string applyID)
        {
            if (files == null || files.Count <= 0)
            {
                return;
            }

            using (var webClient = new WebClient())
            using (FilesDescriptionRoll filesView = new FilesDescriptionRoll())
            {
                Uri uri = null;
                string fileName = String.Empty;
                List<FilesDescription> list = new List<FilesDescription>();

                foreach (var feeFile in files)
                {
                    uri = new Uri(feeFile.Url);
                    fileName = VirtualPathUtility.GetFileName(uri.AbsolutePath);

                    DirectoryInfo di = new DirectoryInfo(ConfigurationManager.AppSettings["FileSavePath"]);
                    string relativeFilePath = Path.Combine(DateTime.Now.Year.ToString(),
                        DateTime.Now.Month.ToString(),
                        DateTime.Now.Day.ToString(),
                        fileName);
                    string fullFileName = Path.Combine(di.FullName,
                        relativeFilePath);

                    FileInfo fi = new FileInfo(fullFileName);
                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }

                    webClient.DownloadFile(uri, fi.FullName);

                    list.Add(new FilesDescription()
                    {
                        Url = relativeFilePath.Replace("\\", "/"),
                        Type = MapXdt.MapFileDescTypeFromXdt(feeFile.FileType),
                        CustomName = fileName,
                        CreatorID = creatorID,
                        FilesMapsArray = new FilesMap[]
                        {
                            new FilesMap()
                            {
                                Name = FilesMapName.ChargeApplyID.ToString(),
                                Value =applyID
                            }
                        }
                    });
                }

                if (list.Count > 0)
                {
                    filesView.Add(list.ToArray());
                }
            }
        }
        #endregion

        #region 芯达通默认审批人
        /// <summary>
        /// 芯达通默认审批人
        /// </summary>
        /// <returns></returns>
        private string GetDefaultApproverID()
        {
            //张庆永
            return "Admin00526";
        }
        #endregion

        #region 获取流水条数
        /// <summary>
        /// 获取流水条数
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        private int GetFlowsCountByInputs(List<ChargeInputDto> inputs)
        {
            int count = 0;

            if (inputs.Count <= 0)
                return count++;

            foreach (var input in inputs)
            {
                count = count + input.FeeItems.Count;
            }

            return count;
        }
        #endregion

        #endregion
    }
}