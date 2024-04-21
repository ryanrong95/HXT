using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
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
using Yahv.Underly.Enums.PvFinance;
using Yahv.Utils.Extends;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 承兑汇票
    /// </summary>
    public class MoneyOrderController : ClientController
    {
        private const string AccountCatalog = "AccCatType0034";     //预收账款

        #region 新增承兑汇票
        /// <summary>
        /// 承兑汇票新增
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult MoneyOrderEnter(InputParam<CenterAcceptanceBillInput> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            CenterAcceptanceBillInput input = new CenterAcceptanceBillInput();

            try
            {
                using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
                using (var tran = repository.OpenTransaction())
                using (var moView = new MoneyOrdersRoll(repository))
                using (var accountsView = new AccountsRoll(repository))
                {
                    input = param.Model;
                    var entity = moView.FirstOrDefault(item => item.Code == input.Code);

                    //FlowAccount flowAccount = null;
                    //PayeeLeft payeeLeft = null;

                    //收款账户 
                    var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.PayeeAccountNo);
                    //付款账户
                    var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.PayerAccountNo);

                    if (payeeAccount == null || string.IsNullOrEmpty(payeeAccount.ID))
                    {
                        result.IsFailed($"[{input.PayeeAccountNo}]账户不存在!");
                    }
                    else if (payerAccount == null || string.IsNullOrEmpty(payerAccount.ID))
                    {
                        result.IsFailed($"[{input.PayerAccountNo}]账户不存在!");
                    }
                    else
                    {
                        if (param.Option.ToLower() == OptionConsts.insert)
                        {
                            if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
                            {
                                result.IsFailed($"[{input.Code}]已存在!");
                            }
                            else
                            {
                                entity = new MoneyOrder()
                                {
                                    BankCode = input.BankCode,
                                    BankName = input.BankName,
                                    BankNo = input.BankNo,
                                    Code = input.Code,
                                    CreatorID = input.CreatorID,
                                    CreateDate = input.CreateDate,
                                    EndDate = input.EndDate,
                                    IsTransfer = input?.IsTransfer ?? false,
                                    ModifierID = input.CreatorID,
                                    ModifyDate = input.CreateDate,
                                    Name = input.Name,
                                    Nature = input.Nature,
                                    PayeeAccountID = payeeAccount.ID,
                                    PayerAccountID = payerAccount.ID,
                                    Price = input.Price,
                                    StartDate = input.StartDate,
                                    Type = input.Type,
                                    Status = input.Status,
                                    IsMoney = input?.IsMoney ?? false,
                                };
                                entity?.Add();
                                FilesSync(input.Files, input.CreatorID, entity.ID);

                                //string newPayeeLeftID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.PayeeLeft);
                                //string newFlowID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc);

                                //flowAccount = new FlowAccount()
                                //{
                                //    ID = newFlowID,
                                //    Type = FlowAccountType.MoneyOrder,
                                //    AccountMethord = AccountMethord.Receipt,
                                //    AccountID = payeeAccount?.ID,
                                //    Currency = Currency.CNY,
                                //    Price = input.Price,
                                //    PaymentDate = input.EndDate,
                                //    FormCode = input.Code,
                                //    Currency1 = Currency.CNY,
                                //    ERate1 = 1,
                                //    Price1 = input.Price,
                                //    CreatorID = input.CreatorID,
                                //    TargetAccountName = payerAccount?.Name,
                                //    TargetAccountCode = payerAccount?.Code,

                                //    PaymentMethord = (input.Type == MoneyOrderType.Commercial) ? PaymentMethord.CommercialAcceptanceBill : PaymentMethord.BankAcceptanceBill,
                                //    MoneyOrderID = entity.ID,
                                //};
                                //flowAccount.Add();

                                //payeeLeft = new PayeeLeft()
                                //{
                                //    ID = newPayeeLeftID,
                                //    AccountCatalogID = AccountCatalog,
                                //    AccountID = payeeAccount?.ID,
                                //    PayerName = payerAccount?.Name,
                                //    Currency = Currency.CNY,
                                //    Price = input.Price,

                                //    Currency1 = Currency.CNY,
                                //    ERate1 = 1,
                                //    Price1 = input.Price,

                                //    CreatorID = input.CreatorID,
                                //    FlowID = newFlowID,
                                //    PayerNature = NatureType.Public,
                                //};
                                //payeeLeft.Add();
                            }
                        }
                        else
                        {
                            result.IsFailed($"承兑汇票不允许修改!");
                        }
                    }

                    if (result.success)
                    {
                        tran.Commit();
                    }
                    else
                    {
                        tran.Rollback();
                    }
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                result.IsFailed(ex);
                return Json(result);
            }
        }
        #endregion

        #region 承兑调拨
        /// <summary>
        /// 承兑调拨申请
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult AcceptanceEnter(InputParam<AcceptanceInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            AcceptanceInputDto input = new AcceptanceInputDto();

            try
            {
                using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
                using (var tran = repository.OpenTransaction())
                using (var moView = new MoneyOrdersRoll(repository))
                using (var accountsView = new AccountsRoll(repository))
                {
                    input = param.Model;

                    //调出账户
                    var payerAccount = accountsView.FirstOrDefault(item => item.Code == input.OutAccountNo);
                    var payerAccountID = payerAccount?.ID;
                    //调入账户
                    var payeeAccount = accountsView.FirstOrDefault(item => item.Code == input.InAccountNo);
                    var payeeAccountID = payeeAccount?.ID;
                    //承兑汇票
                    var moneyOrder = moView.FirstOrDefault(item => item.Code == input.DiscountSeqNo);


                    AcceptanceApply apply = null;
                    AcceptanceLeft payerLeft = null;
                    AcceptanceLeft payeeLeft = null;

                    FlowAccount payerFlow = null;   //调出流水
                    AcceptanceRight payerRight = null;      //调出核销

                    FlowAccount payeeFlow = null;       //调入流水
                    AcceptanceRight payeeRight = null;      //调入核销

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        //检查付款账户是否存在
                        if (string.IsNullOrEmpty(payerAccountID))
                        {
                            result.IsFailed($"[{input.OutAccountNo}]调出账户不存在!");
                        }
                        else if (string.IsNullOrEmpty(payeeAccountID))
                        {
                            result.IsFailed($"[{input.InAccountNo}]调入账户不存在!");
                        }
                        else if (moneyOrder == null || moneyOrder.ID.IsNullOrEmpty())
                        {
                            result.IsFailed($"[{input.DiscountSeqNo}]承兑汇票不存在!");
                        }
                        else
                        {
                            var payerCurrency = CurrencyHelper.GetCurrency(input.OutCurrency);
                            var payeeCurrency = CurrencyHelper.GetCurrency(input.InCurrency);

                            var payeeRate = ExchangeRates.Universal[payeeCurrency, Currency.CNY];
                            var payerRate = ExchangeRates.Universal[payerCurrency, Currency.CNY];

                            //承兑金额
                            decimal excChangePrice = (input?.DiscountInterest > 0 ? input.OutAmount - input.DiscountInterest : input.OutAmount) ?? 0;
                            var payeePrice = input.InAmount;

                            //添加申请
                            apply = new AcceptanceApply()
                            {
                                ApplierID = input.CreatorID,
                                ApproverID = GetDefaultApproverID(),
                                CreatorID = input.CreatorID,
                                Currency = payerCurrency,
                                MoneyOrderID = moneyOrder.ID,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Price = input.OutAmount,
                                Status = ApplyStauts.Completed,
                                Type = AcceptanceType.Discount,
                                SenderID = SystemSender.Center.GetFixedID(),
                            };
                            apply.Enter();

                            //修改承兑汇票
                            repository.Update<Layers.Data.Sqls.PvFinance.MoneyOrders>(new
                            {
                                Status = (int)MoneyOrderStatus.Exchanged,
                                ExchangeDate = input.PaymentDate,
                                ExchangePrice = excChangePrice,
                                ModifierID = input.CreatorID,
                                ModifyDate = DateTime.Now,
                            }, item => item.ID == moneyOrder.ID);

                            #region 调出
                            //调出
                            payerLeft = new AcceptanceLeft()
                            {
                                AccountMethord = AccountMethord.Output,
                                ApplyID = apply.ID,
                                CreatorID = input.CreatorID,
                                Currency = payerCurrency,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Price = input.OutAmount,
                                Status = GeneralStatus.Normal,
                            };
                            payerLeft.Enter();

                            //调出流水
                            payerFlow = new FlowAccount()
                            {
                                ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                                Currency = payerCurrency,
                                CreateDate = DateTime.Now,
                                AccountID = payerAccountID,
                                AccountMethord = AccountMethord.Output,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = payerRate,
                                FormCode = input.OutSeqNo,
                                TargetAccountCode = payeeAccount?.Code,
                                TargetAccountName = payeeAccount?.Name,
                                PaymentDate = input.PaymentDate,
                                Price = -input.OutAmount,
                                Price1 = -(input.OutAmount * payerRate).Round(),
                                PaymentMethord = (PaymentMethord)input.PaymentType,
                                Type = FlowAccountType.MoneyOrder,
                                MoneyOrderID = moneyOrder.ID,
                            };
                            payerFlow.Add();

                            //调出核销
                            payerRight = new AcceptanceRight()
                            {
                                CreatorID = input.CreatorID,
                                AcceptanceLeftID = payerLeft.ID,
                                FlowID = payerFlow.ID,
                                Price = input.OutAmount,
                            };
                            payerRight.Enter();
                            #endregion

                            #region 调入
                            //调入
                            payeeLeft = new AcceptanceLeft()
                            {
                                AccountMethord = AccountMethord.Input,
                                ApplyID = apply.ID,
                                CreatorID = input.CreatorID,
                                Currency = payerCurrency,
                                PayeeAccountID = payeeAccountID,
                                PayerAccountID = payerAccountID,
                                Price = payeePrice,
                                Status = GeneralStatus.Normal,
                            };
                            payeeLeft.Enter();

                            //调入流水
                            payeeFlow = new FlowAccount()
                            {
                                ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                                Currency = payeeCurrency,
                                CreateDate = DateTime.Now,
                                AccountID = payeeAccountID,
                                AccountMethord = AccountMethord.Input,
                                CreatorID = input.CreatorID,
                                Currency1 = Currency.CNY,
                                ERate1 = payeeRate,
                                FormCode = input.InSeqNo,
                                TargetAccountCode = payerAccount.Code,
                                TargetAccountName = payerAccount.Name,
                                PaymentDate = input.PaymentDate,
                                Price = payeePrice,
                                Price1 = (payeePrice * payeeRate).Round(),
                                PaymentMethord = (PaymentMethord)input.PaymentType,
                                Type = FlowAccountType.MoneyOrder,
                                MoneyOrderID = moneyOrder.ID,
                            };
                            payeeFlow.Add();

                            //调入核销
                            payeeRight = new AcceptanceRight()
                            {
                                AcceptanceLeftID = payeeLeft.ID,
                                CreatorID = input.CreatorID,
                                FlowID = payeeFlow.ID,
                                Price = payeePrice,
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
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                result.IsFailed(ex);
                return Json(result);
            }
        }
        #endregion

        #region 私有函数
        #region 根据地址同步承兑汇票附件
        private void FilesSync(List<CenterAcceptanceBillInput.AcceptanceBillFile> files, string creatorID, string applyID)
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
                        Type = FileDescType.MoneyOrder,
                        CustomName = fileName,
                        CreatorID = creatorID,
                        FilesMapsArray = new FilesMap[]
                        {
                            new FilesMap()
                            {
                                Name = FilesMapName.MoneyOrderID.ToString(),
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
        #endregion
    }
}