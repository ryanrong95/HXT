using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Layers.Data;
using Yahv.Finance.Services;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Rolls;
using Yahv.Finance.WebApi.Filter;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.Web.Mvc;

namespace Yahv.Finance.WebApi.Controllers
{
    /// <summary>
    /// 账户
    /// </summary>
    public class AccountController : ClientController
    {
        /// <summary>
        /// 账户创建/修改
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [SenderAuthorize]
        public ActionResult Enter(InputParam<AccountInputDto> param)
        {
            var result = new JMessage() { code = 200, success = true, data = "操作成功!" };
            AccountInputDto input = new AccountInputDto();

            try
            {
                using (var view = new AccountsRoll())
                {
                    input = param.Model;
                    var entity = view.FirstOrDefault(item => item.Code == input.BankAccount);

                    if (param.Option.ToLower() == OptionConsts.insert)
                    {
                        if (entity != null && !string.IsNullOrWhiteSpace(entity.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.BankAccount}]已存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            entity = new Account()
                            {
                                NatureType = NatureType.Public,
                                Name = input.CompanyName ?? input.AccountName,
                                ShortName = input.AccountName,
                                BankName = GetSimilarBank(input.BankName),
                                Code = input.BankAccount,
                                ManageType = ManageType.Normal,
                                Currency = GetCurrency(input.Currency),
                                OpeningBank = input.BankName,
                                BankAddress = input.BankAddress,
                                District = GetDistrict(input.Region),
                                SwiftCode = input.SwiftCode,
                                IsHaveU = false,
                                OwnerID = input.Owner,
                                GoldStoreID = GetGoldStoreID(input.VaultName),
                                EnterpriseID = GetEnterpriseID(input.CompanyName),
                                CreatorID = input.CreatorID,
                                ModifierID = input.CreatorID,
                                Source = (AccountSource)input.AccountSource,
                                IsVirtual = input.IsVirtual,
                            };
                            entity.Enter();
                            InsertAccountType(entity.ID, input.AccountType);

                            //初始化余额
                            if (input.Balance != null && input.Balance > 0)
                            {
                                var rate = ExchangeRates.Universal[entity.Currency, Currency.CNY];

                                //收款流水
                                new FlowAccount()
                                {
                                    ID = PKeySigner.Pick(Yahv.Finance.Services.PKeyType.FlowAcc),
                                    AccountMethord = AccountMethord.Receipt,
                                    AccountID = entity.ID,
                                    Currency = entity.Currency,
                                    Price = input.Balance.Value,
                                    PaymentDate = DateTime.Now,
                                    FormCode = "余额初始化",
                                    Currency1 = Currency.CNY,
                                    ERate1 = rate,
                                    Price1 = input.Balance.Value * rate,
                                    CreatorID = input.CreatorID,
                                    PaymentMethord = PaymentMethord.BankTransfer,     //收款方式
                                }.Enter();
                            }
                        }
                    }
                    else if (param.Option.ToLower() == OptionConsts.update)
                    {
                        entity = view.FirstOrDefault(item => item.Code == input.BankAccount);

                        if (entity == null || string.IsNullOrWhiteSpace(entity.ID))
                        {
                            result = new JMessage()
                            {
                                data = $"[{input.BankAccount}]不存在!",
                                success = false,
                                code = 500,
                            };
                        }
                        else
                        {
                            entity.NatureType = NatureType.Public;
                            entity.Name = input.CompanyName ?? input.AccountName;
                            entity.ShortName = input.AccountName;
                            entity.BankName = GetSimilarBank(input.BankName);
                            entity.ManageType = ManageType.Normal;
                            entity.Currency = GetCurrency(input.Currency);
                            entity.OpeningBank = input.BankName;
                            entity.BankAddress = input.BankAddress;
                            entity.District = GetDistrict(input.Region);
                            entity.SwiftCode = input.SwiftCode;
                            entity.IsHaveU = false;
                            entity.OwnerID = input.Owner;
                            entity.GoldStoreID = GetGoldStoreID(input.VaultName);
                            entity.EnterpriseID = GetEnterpriseID(input.CompanyName);
                            entity.ModifierID = input.CreatorID;
                            entity.Source = (AccountSource)input.AccountSource;
                            entity.IsVirtual = input.IsVirtual;
                            entity.Enter();
                            InsertAccountType(entity.ID, input.AccountType);
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
                }

                //Services.Oplogs.Oplog(input.CreatorID, LogModular.账户管理_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
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
                //Services.Oplogs.Oplog(input.CreatorID, LogModular.账户管理_Api, Services.Oplogs.GetMethodInfo(), result.data, param.Json(), url: Request.Url.ToString());
                return Json(result);
            }
        }

        #region 私有函数

        #region 根据简称获取币种
        /// <summary>
        /// 根据简称获取币种
        /// </summary>
        /// <returns></returns>
        private Currency GetCurrency(string shortName)
        {
            return Enum.GetValues(typeof(Currency)).Cast<Currency>()
                .FirstOrDefault(item => item.GetCurrency().ShortName == shortName);
        }
        #endregion

        #region 根据编码获取区域名称

        private string GetDistrict(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return string.Empty;
            }

            return Enum.GetValues(typeof(Origin))
                    .Cast<Origin>()
                    .FirstOrDefault(item => item.GetOrigin().Code.ToLower() == code.ToLower())
                    .GetDescription();
        }
        #endregion

        #region 根据库房名称获取ID
        /// <summary>
        /// 根据库房名称获取ID
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetGoldStoreID(string name)
        {
            string result = string.Empty;

            using (var view = new GoldStoresRoll())
            {
                result = view.FirstOrDefault(item => item.Name == name && item.Status == GeneralStatus.Normal)?.ID;
            }

            return result;
        }
        #endregion

        #region 根据公司名称获取ID

        private string GetEnterpriseID(string name)
        {
            string result = string.Empty;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            using (var view = new EnterprisesRoll())
            {
                var model = view.FirstOrDefault(item => item.Name == name);
                if (model != null && !string.IsNullOrWhiteSpace(model.ID))
                {
                    result = model.ID;
                }
                else
                {
                    model = new Enterprise()
                    {
                        Name = name.Trim(),
                        Type = name.Trim() == ConfigurationManager.AppSettings["CompanyName"] ? EnterpriseAccountType.Company : (EnterpriseAccountType.Client | EnterpriseAccountType.Supplier),
                        CreatorID = Npc.Robot.Obtain(),
                        ModifierID = Npc.Robot.Obtain(),
                    };
                    model.Enter();
                    result = model.ID;
                }
            }

            return result;
        }
        #endregion

        #region 根据开户行获取银行名称
        /// <summary>
        /// 根据开户行获取银行名称
        /// </summary>
        /// <param name="name">银行名称</param>
        /// <returns></returns>
        private string GetSimilarBank(string name)
        {
            var result = name;

            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            using (var view = new BanksRoll())
            {
                Dictionary<string, float> dic = new Dictionary<string, float>();
                var banks = view.Select(item => new { item.ID, item.Name, item.EnglishName }).ToArray();
                foreach (var bank in banks)
                {
                    var float1 = EditorDistance.Levenshtein(bank.Name, name);
                    var float2 = EditorDistance.Levenshtein(bank.EnglishName, name);

                    if (float1 >= float2)
                    {
                        dic.Add(bank.Name, float1);
                    }
                    else
                    {
                        dic.Add(bank.Name, float2);
                    }
                }

                result = dic.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

            return result;
        }
        #endregion

        #region 增加账户类型
        /// <summary>
        /// 增加账户类型
        /// </summary>
        /// <param name="accountId">账户ID</param>
        /// <param name="accountsType">类型</param>
        private void InsertAccountType(string accountId, string accountsType)
        {
            using (var accountTypeView = new AccountTypesRoll())
            using (var maps = new MapsAccountTypeRoll())
            {
                if (!string.IsNullOrWhiteSpace(accountsType))
                {
                    var list = new List<MapsAccountType>();
                    var accountTypes = accountTypeView.Where(item => item.Status == GeneralStatus.Normal).ToArray();
                    var array = MapXdt.MapAccountTypeFromXdt(accountsType);
                    if (array.Length <= 0) return;
                    foreach (var type in array)
                    {
                        list.Add(new MapsAccountType()
                        {
                            AccountID = accountId,
                            AccountTypeID = accountTypes.FirstOrDefault(item => item.Name == type)?.ID
                        });
                    }

                    maps.BatchEnter(accountId, list.ToArray());
                }
            }
        }
        #endregion

        #endregion
    }
}