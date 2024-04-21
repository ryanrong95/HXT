using System;
using System.Linq;
using Layers.Data;
using Layers.Data.Sqls;
using Layers.Linq;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Underly;
using Yahv.Underly.Enums.PvFinance;

namespace Yahv.Finance.Services.Models.Rolls
{
    public class PayerImportInput
    {
        #region 属性
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 摘要（类型）
        /// </summary>
        public string AccountCatalogID { get; set; }

        /// <summary>
        /// 我方账户ID
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 对方账号
        /// </summary>
        public string TargetCode { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public PayerImportType Type { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string FormCode { get; set; }
        #endregion

        #region 持久化
        /// <summary>
        /// 持久化
        /// </summary>
        public string Enter()
        {
            string result = string.Empty;

            try
            {
                using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
                using (var tran = reponsitory.OpenTransaction())
                using (var accountsView = new AccountsRoll(reponsitory))
                {
                    string flowId = string.Empty;
                    Account sourceAccount = null;
                    Account targetAccount = null;

                    sourceAccount = accountsView.FirstOrDefault(item => item.ID == this.AccountID);
                    targetAccount = accountsView.FirstOrDefault(item => item.Code == this.TargetCode);
                    if (targetAccount == null)
                    {
                        return "对方账号不存在!";
                    }

                    var rate = ExchangeRates.Universal[sourceAccount.Currency, Currency.CNY];
                    var flowType = 1;

                    //付款
                    if (this.Type == PayerImportType.Borrow)
                    {
                        //货款（除了货款，全部走费用）    AccCatType0077(货款)
                        if (this.AccountCatalogID == "AccCatType0077")
                        {
                            string applyId = PKeySigner.Pick(PKeyType.PayerApply);
                            //申请
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerApplies()
                            {
                                ID = applyId,
                                ApplierID = this.CreatorID,
                                Status = (int)ApplyStauts.Completed,
                                CreateDate = DateTime.Now,
                                CreatorID = this.CreatorID,
                                Price = this.Price,
                                ApproverID = this.CreatorID,
                                Currency = (int)sourceAccount.Currency,
                                ExcuterID = this.CreatorID,
                                IsPaid = false,
                                SenderID = SystemSender.Center.GetFixedID(),
                                PayeeAccountID = sourceAccount.ID,
                                PayerAccountID = targetAccount.ID,
                            });

                            //应付
                            string payerLeftId = PKeySigner.Pick(PKeyType.PayerLeft);
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerLefts()
                            {
                                ID = payerLeftId,
                                Status = (int)GeneralStatus.Normal,
                                CreateDate = DateTime.Now,
                                CreatorID = this.CreatorID,
                                AccountCatalogID = this.AccountCatalogID,
                                ApplyID = applyId,
                                Currency = (int)sourceAccount.Currency,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = rate,
                                PayeeAccountID = targetAccount.ID,
                                PayerAccountID = sourceAccount.ID,
                                Price = this.Price,
                                Price1 = this.Price * rate,
                            });

                            //付款流水
                            flowId = PKeySigner.Pick(PKeyType.FlowAcc);
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                            {
                                ID = flowId,
                                AccountMethord = (int)AccountMethord.Payment,
                                AccountID = sourceAccount.ID,
                                Currency = (int)sourceAccount.Currency,
                                Price = -this.Price,
                                PaymentDate = this.CreateDate,
                                FormCode = this.FormCode,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = rate,
                                Price1 = -this.Price * rate,
                                CreatorID = this.CreatorID,
                                CreateDate = DateTime.Now,
                                TargetAccountName = targetAccount.Name,
                                TargetAccountCode = targetAccount.Code,
                                PaymentMethord = (int)PaymentMethord.BankTransfer,
                                Type = flowType,
                            });

                            //付款核销
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayerRights()
                            {
                                ID = PKeySigner.Pick(PKeyType.PayerRight),
                                CreateDate = DateTime.Now,
                                CreatorID = this.CreatorID,
                                Currency = (int)sourceAccount.Currency,
                                Price = this.Price,
                                Price1 = this.Price * rate,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = rate,
                                FlowID = flowId,
                                PayerLeftID = payerLeftId,
                            });
                        }
                        else
                        {
                            string applyId = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.ChargeApply);

                            //申请
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.ChargeApplies()
                            {
                                ID = applyId,
                                PayeeAccountID = targetAccount.ID,
                                PayerAccountID = sourceAccount.ID,
                                Type = (int)CostApplyType.Normal,
                                IsImmediately = false,
                                Currency = (int)sourceAccount.Currency,
                                Price = this.Price,
                                SenderID = SystemSender.Center.GetFixedID(),
                                ApplierID = this.CreatorID,
                                ExcuterID = this.CreatorID,
                                CreatorID = this.CreatorID,
                                CreateDate = DateTime.Now,
                                Status = (int)ApplyStauts.Completed,
                            });

                            //流水
                            flowId = PKeySigner.Pick(PKeyType.FlowAcc);
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                            {
                                ID = flowId,
                                AccountMethord = (int)AccountMethord.Finance,
                                AccountID = this.AccountID,
                                Currency = (int)sourceAccount.Currency,
                                Price = -this.Price,
                                PaymentDate = CreateDate,
                                FormCode = this.FormCode,
                                Currency1 = (int)Currency.CNY,
                                ERate1 = rate,
                                Price1 = -this.Price * rate,
                                CreatorID = this.CreatorID,
                                CreateDate = DateTime.Now,
                                TargetAccountName = targetAccount.Name,
                                TargetAccountCode = targetAccount.Code,
                                PaymentMethord = (int)PaymentMethord.BankTransfer,
                                Type = flowType,
                            });

                            //申请项
                            reponsitory.Insert(new Layers.Data.Sqls.PvFinance.ChargeApplyItems()
                            {
                                ID = PKeySigner.Pick(PKeyType.ChargeApplyItem),
                                ApplyID = applyId,
                                IsPaid = false,
                                ExpectedTime = CreateDate,
                                AccountCatalogID = this.AccountCatalogID,
                                Price = this.Price,
                                FlowID = flowId,
                                CreateDate = DateTime.Now,
                                ModifyDate = DateTime.Now,
                                Status = (int)GeneralStatus.Normal,
                            });
                        }
                    }
                    //收款
                    else if (this.Type == PayerImportType.Loan)
                    {
                        flowId = PKeySigner.Pick(PKeyType.FlowAcc);

                        reponsitory.Insert(new Layers.Data.Sqls.PvFinance.FlowAccounts()
                        {
                            ID = flowId,
                            AccountMethord = (int)AccountMethord.Receipt,
                            AccountID = sourceAccount.ID,
                            Currency = (int)sourceAccount.Currency,
                            Price = this.Price,
                            PaymentDate = this.CreateDate,
                            FormCode = this.FormCode,
                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = this.Price * rate,
                            CreatorID = this.CreatorID,
                            CreateDate = DateTime.Now,
                            TargetAccountName = targetAccount.Name,
                            TargetAccountCode = targetAccount.Code,
                            PaymentMethord = (int)PaymentMethord.BankTransfer,
                            Type = flowType,
                        });

                        reponsitory.Insert(new Layers.Data.Sqls.PvFinance.PayeeLefts()
                        {
                            ID = PKeySigner.Pick(PKeyType.PayeeLeft),
                            AccountCatalogID = this.AccountCatalogID,
                            AccountID = this.AccountID,
                            PayerName = targetAccount.Name,
                            Currency = (int)sourceAccount.Currency,
                            Price = this.Price,
                            Currency1 = (int)Currency.CNY,
                            ERate1 = rate,
                            Price1 = this.Price * rate,
                            CreatorID = this.CreatorID,
                            CreateDate = DateTime.Now,
                            FlowID = flowId,
                            Status = (int)GeneralStatus.Normal,
                            PayerNature = (int)NatureType.Public,
                        });
                    }

                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
        #endregion
    }
}