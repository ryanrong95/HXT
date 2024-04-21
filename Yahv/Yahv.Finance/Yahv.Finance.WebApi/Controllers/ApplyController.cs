using System;
using System.Collections.Generic;
using System.Linq;
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
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 申请
    /// </summary>
    public class ApplyController : ClientController
    {
        /// <summary>
        /// 代付货款申请
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult PayForGoodsEnter(InputParam<PayForGoodsInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            PayForGoodsInputDto input = new PayForGoodsInputDto();

            using (var repository = LinqFactory<PvFinanceReponsitory>.Create())
            using (var tran = repository.OpenTransaction())
            using (var accountsView = new AccountsRoll(repository))
            using (var accountCatalogsView = new AccountCatalogsRoll(repository))
            using (var enterprisesView = new Yahv.Finance.Services.Views.Rolls.EnterprisesRoll(repository))
            {
                try
                {
                    input = param.Model;

                    var accounts = accountsView.Where(item => item.Code == input.PayeeCode || item.Code == input.PayerCode).ToArray();
                    var epIds = accounts.Select(item => item.EnterpriseID).ToArray();
                    var enterprises = enterprisesView.Where(item => item.Name == input.PayeeName || item.Name == input.PayerName || epIds.Contains(item.ID)).ToArray();
                    //付款账户
                    var payerAccount = GetOrAddPayerAccount(repository, accounts, input, enterprises);
                    //收款账户
                    var payeeAccount = GetOrAddPayeeAccount(repository, accounts, input, enterprises);

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        var currency = CurrencyHelper.GetCurrency(input.PayeeCurrency);
                        decimal rate = ExchangeRates.Universal[currency, Currency.CNY];

                        //申请
                        var payerApplie = new PayerApply()
                        {
                            Currency = currency,
                            ApplierID = input.CreatorID,
                            CreatorID = input.CreatorID,
                            IsPaid = false,
                            Price = input.Price,
                            Status = ApplyStauts.Paying,
                            PayeeAccountID = payeeAccount.ID,
                            PayerAccountID = payerAccount.ID,
                            SenderID = param.Sender,
                            ExcuterID = input.ExecutorID,
                            PayerID = payerAccount?.EnterpriseID,
                        };
                        payerApplie.Enter();
                        //应付
                        var payerLeft = new PayerLeft()
                        {
                            PayeeAccountID = payeeAccount.ID,
                            PayerAccountID = payerAccount.ID,
                            Currency = currency,
                            ApplyID = payerApplie.ID,
                            CreatorID = input.CreatorID,
                            Currency1 = Currency.CNY,
                            ERate1 = rate,
                            //Price1 = (input.Price * rate).Round(2),
                            Price1 = input.Price * rate,
                            Price = input.Price,
                            Status = GeneralStatus.Normal,
                            PayerID = payerAccount?.EnterpriseID,
                            AccountCatalogID = accountCatalogsView.Get("付款类型", "供应链业务").FirstOrDefault(item => item.Name == "代付货款")?.ID,
                        };
                        payerLeft.Enter();
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

                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.代付货款申请_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                    //Services.Oplogs.Oplog(input.CreatorID, LogModular.代付货款申请_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                    return Json(result);
                }
            }
        }

        #region 私有函数
        private Account GetOrAddPayeeAccount(PvFinanceReponsitory reponsitory, Account[] accounts, PayForGoodsInputDto input, Enterprise[] enterprises)
        {
            var account = accounts.FirstOrDefault(item => item.Code == input.PayeeCode);

            if (account != null && !string.IsNullOrEmpty(account.ID))
            {
                //如果供应商名称不一致，更新名称
                if (account.Enterprise?.Name != input.PayeeName)
                {
                    var enterprise = enterprises.FirstOrDefault(item => item.ID == account.EnterpriseID);
                    if (enterprise != null)
                    {
                        enterprise.Name = input.PayeeName;
                        enterprise.Enter();
                    }
                }

                return account;
            }
            else
            {
                string id = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Account);
                account = new Account()
                {
                    ID = id,
                    NatureType = NatureType.Public,
                    Name = input.PayeeName,
                    BankName = input.PayeeBank,
                    Code = input.PayeeCode,
                    ManageType = ManageType.Normal,
                    Currency = CurrencyHelper.GetCurrency(input.PayeeCurrency),
                    IsHaveU = false,
                    CreatorID = input.CreatorID,
                    ModifierID = input.CreatorID,
                    Source = AccountSource.Simple,
                    IsVirtual = false,
                };

                //企业不存在 添加企业
                var enterprise = enterprises
                                .Where(t => t.Name == input.PayeeName
                                && t.Status == GeneralStatus.Normal).FirstOrDefault();
                if (enterprise == null)
                {
                    //要新增一个 Enterprise
                    enterprise = new Enterprise()
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

        private Account GetOrAddPayerAccount(PvFinanceReponsitory reponsitory, Account[] accounts, PayForGoodsInputDto input, Enterprise[] enterprises)
        {
            var account = accounts.FirstOrDefault(item => item.Code == input.PayerCode);

            if (account != null && !string.IsNullOrEmpty(account.ID))
            {
                return account;
            }
            else
            {
                string id = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Account);
                account = new Account()
                {
                    ID = id,
                    NatureType = NatureType.Public,
                    Name = input.PayerName,
                    BankName = input.PayerBank,
                    Code = input.PayerCode,
                    ManageType = ManageType.Normal,
                    Currency = CurrencyHelper.GetCurrency(input.PayerCurrency),
                    IsHaveU = false,
                    CreatorID = input.CreatorID,
                    ModifierID = input.CreatorID,
                    Source = AccountSource.Simple,
                    IsVirtual = false,
                };

                //企业不存在 添加企业
                var enterprise = enterprises
                                .Where(t => t.Name == input.PayerName
                                && t.Status == GeneralStatus.Normal).FirstOrDefault();
                if (enterprise == null)
                {
                    //要新增一个 Enterprise
                    enterprise = new Enterprise()
                    {
                        ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.Enterprise),
                        Name = input.PayerName,
                        Type = EnterpriseAccountType.Company,
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
    }
}