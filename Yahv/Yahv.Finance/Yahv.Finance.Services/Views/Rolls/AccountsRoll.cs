using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;
using Yahv.Underly.Enums;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class AccountsRoll : QueryView<Account, PvFinanceReponsitory>
    {
        public AccountsRoll()
        {
        }

        protected AccountsRoll(PvFinanceReponsitory reponsitory, IQueryable<Account> iQueryable) : base(reponsitory, iQueryable)
        {
        }

        public AccountsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Account> GetIQueryable()
        {
            var accountsOrigin = new AccountsOrigin(this.Reponsitory);
            var goldStoresOrigin = new GoldStoresOrigin(this.Reponsitory);
            var enterprisesOrigin = new EnterprisesOrigin(this.Reponsitory);
            var moAcocuntIds = new MapsAccountTypeOrigin(this.Reponsitory)
                .Where(item => GetMoAccountTypeID().Contains(item.AccountTypeID))
                .Select(item => item.AccountID).ToArray();



            var iQuery = from account in accountsOrigin
                         join goldStore in goldStoresOrigin on account.GoldStoreID equals goldStore.ID into goldStoresOrigin2
                         from goldStore in goldStoresOrigin2.DefaultIfEmpty()
                         join ep in enterprisesOrigin on account.EnterpriseID equals ep.ID into epJoin
                         from ep in epJoin.DefaultIfEmpty()
                         select new Account
                         {
                             ID = account.ID,
                             Name = account.Name,
                             ShortName = account.ShortName,
                             Code = account.Code,
                             NatureType = account.NatureType,
                             ManageType = account.ManageType,
                             Currency = account.Currency,
                             BankName = account.BankName,
                             OpeningBank = account.OpeningBank,
                             BankAddress = account.BankAddress,
                             District = account.District,
                             SwiftCode = account.SwiftCode,
                             OpeningTime = account.OpeningTime,
                             IsHaveU = account.IsHaveU,
                             BankNo = account.BankNo,
                             OwnerID = account.OwnerID,
                             GoldStoreID = account.GoldStoreID,
                             EnterpriseID = account.EnterpriseID,
                             PersonID = account.PersonID,
                             CreatorID = account.CreatorID,
                             ModifierID = account.ModifierID,
                             CreateDate = account.CreateDate,
                             ModifyDate = account.ModifyDate,
                             Status = account.Status,
                             Summary = account.Summary,
                             Source = account.Source,
                             IsVirtual = account.IsVirtual,
                             DyjShortName = account.DyjShortName,

                             GoldStoreName = goldStore != null ? goldStore.Name : "",
                             Enterprise = ep,

                             IsAcceptance = moAcocuntIds.Contains(account.ID),
                         };
            return iQuery;
        }

        /// <summary>
        /// 分页方法
        /// </summary>
        /// <returns></returns>
        public object ToMyPage(int? pageIndex = null, int? pageSize = null)
        {
            IQueryable<Account> iquery = this.IQueryable.Cast<Account>().OrderByDescending(item => item.CreateDate);
            int total = iquery.Count();

            if (pageIndex.HasValue && pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                iquery = iquery.Skip(pageSize.Value * (pageIndex.Value - 1)).Take(pageSize.Value);
            }

            //获取数据
            var ienum_myAccount = iquery.ToArray();

            var adminsView = new AdminsTopView(this.Reponsitory);

            var linq_creator = from applier in adminsView
                               where ienum_myAccount.Select(item => item.CreatorID).Contains(applier.ID)
                               select new
                               {
                                   ID = applier.ID,
                                   Name = applier.RealName,
                               };

            var ienums_creator = linq_creator.ToArray();

            //流水
            var flowsView = new FlowAccountsRoll(this.Reponsitory);
            var linq_flows = from flow in flowsView
                             group flow by flow.AccountID into _group
                             where ienum_myAccount.Select(item => item.ID).Contains(_group.Key)
                             select new
                             {
                                 ID = _group.Key,
                                 Value = _group.Sum(g => g.Price)
                             };
            var ienums_flows = linq_flows.ToArray();

            var ienums_linq = from account in ienum_myAccount
                              join creator in ienums_creator on account.CreatorID equals creator.ID into _creator
                              from creator in _creator.DefaultIfEmpty()
                              join flow in ienums_flows on account.ID equals flow.ID into _flow
                              from flow in _flow.DefaultIfEmpty()
                              select new Account
                              {
                                  ID = account.ID,
                                  Name = account.Name,
                                  ShortName = account.ShortName,
                                  Code = account.Code,
                                  NatureType = account.NatureType,
                                  ManageType = account.ManageType,
                                  Currency = account.Currency,
                                  BankName = account.BankName,
                                  OpeningBank = account.OpeningBank,
                                  BankAddress = account.BankAddress,
                                  District = account.District,
                                  SwiftCode = account.SwiftCode,
                                  OpeningTime = account.OpeningTime,
                                  IsHaveU = account.IsHaveU,
                                  BankNo = account.BankNo,
                                  OwnerID = account.OwnerID,
                                  GoldStoreID = account.GoldStoreID,
                                  EnterpriseID = account.EnterpriseID,
                                  PersonID = account.PersonID,
                                  CreatorID = account.CreatorID,
                                  ModifierID = account.ModifierID,
                                  CreateDate = account.CreateDate,
                                  ModifyDate = account.ModifyDate,
                                  Status = account.Status,
                                  Source = account.Source,

                                  GoldStoreName = account.GoldStoreName,
                                  CreatorName = creator?.Name,
                                  Balance = flow?.Value,
                              };

            var results = ienums_linq.ToArray();

            Func<Account, object> convert = item => new
            {
                AccountID = item.ID,
                item.ShortName,
                NatureTypeInt = item.NatureType.GetHashCode(),
                NatureTypeDes = item.NatureType.GetDescription(),
                GoldStoreName = item.GoldStoreName,
                BankName = item.BankName,
                Code = item.Code,
                CurrencyDes = item.Currency.GetDescription(),
                StatusDes = item.Status.GetDescription(),
                item.CreatorName,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Balance = item.Balance,
                //SourceDes = item.Source.GetDescription(),
                item.Name,
            };

            if (!pageIndex.HasValue && !pageSize.HasValue)//如果是无值就表示：忽略本逻辑
            {
                Func<dynamic, object> convertAgain = item => new
                {

                };

                return results.Select(convert).Select(convertAgain).ToArray();
            }

            return new
            {
                total = total,
                Size = pageSize ?? 20,
                Index = pageIndex ?? 1,
                rows = results.Select(convert).ToArray(),
            };
        }

        /// <summary>
        /// 根据金库名称查询
        /// </summary>
        /// <param name="goldStoreName"></param>
        /// <returns></returns>
        public AccountsRoll SearchByGoldStoreName(string goldStoreName)
        {
            var linq = from query in this.IQueryable
                       where query.GoldStoreName.Contains(goldStoreName)
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据银行账号查询
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AccountsRoll SearchByCode(string code)
        {
            var linq = from query in this.IQueryable
                       where query.Code.Contains(code)
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据企业名称查询
        /// </summary>
        /// <param name="enterpriseName"></param>
        /// <returns></returns>
        public AccountsRoll SearchByEnterpriseName(string enterpriseName)
        {
            var myQuery = this.IQueryable;

            var enterprisesOrigin = new EnterprisesOrigin(this.Reponsitory);

            var linqs_enterprise = from enterprise in enterprisesOrigin
                                   where enterprise.Status == GeneralStatus.Normal
                                      && enterprise.Name.Contains(enterpriseName)
                                   select enterprise.ID;

            var linq = from query in myQuery
                       join item in linqs_enterprise on query.EnterpriseID equals item
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据状态查询
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public AccountsRoll SearchByStatus(GeneralStatus status)
        {
            var linq = from query in this.IQueryable
                       where query.Status == status
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据名称查询
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AccountsRoll SearchByName(string name)
        {
            var linq = from query in this.IQueryable
                       where query.Name.Contains(name) || query.ShortName.Contains(name)
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据账户性质查询
        /// </summary>
        /// <param name="natureType"></param>
        /// <returns></returns>
        public AccountsRoll SearchByNatureType(NatureType natureType)
        {
            var linq = from query in this.IQueryable
                       where query.NatureType == natureType
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据币种查询
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public AccountsRoll SearchByCurrency(Currency currency)
        {
            var linq = from query in this.IQueryable
                       where query.Currency == currency
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据币种查询
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public AccountsRoll SearchByEpAccountType(EnterpriseAccountType accountType)
        {
            var linq = from query in this.IQueryable
                       where query.Enterprise.Type == accountType
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 根据账户管理员查询(内部公司)
        /// </summary>
        /// <returns></returns>
        public AccountsRoll SearchByOwnerID(string ownerId)
        {
            var linq = from query in this.IQueryable
                       where (query.OwnerID == ownerId && query.EnterpriseID != null && ((query.Enterprise.Type & EnterpriseAccountType.Company) != 0))
                             || (query.Enterprise.Type & EnterpriseAccountType.Client) != 0
                             || (query.Enterprise.Type & EnterpriseAccountType.Supplier) != 0
                       select query;

            var view = new AccountsRoll(this.Reponsitory, linq);
            return view;
        }

        /// <summary>
        /// 批量启用
        /// </summary>
        /// <param name="ids"></param>
        public void Enable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Accounts>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Normal,
                }, item => ids.Contains(item.ID));
            }
        }

        /// <summary>
        /// 批量禁用
        /// </summary>
        /// <param name="ids"></param>
        public void Disable(string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                reponsitory.Update<Layers.Data.Sqls.PvFinance.Accounts>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)GeneralStatus.Closed,
                }, item => ids.Contains(item.ID));
            }
        }

        #region 获取承兑账户
        /// <summary>
        /// 获取承兑账户
        /// </summary>
        /// <returns></returns>
        public IQueryable<Account> GetMoneyOrderAccounts()
        {
            var ids = GetMoAccountTypeID();

            var moAcocuntIds = new MapsAccountTypeOrigin(this.Reponsitory)
                .Where(item => ids.Contains(item.AccountTypeID))
                .Select(item => item.AccountID).ToArray();

            return from query in this.IQueryable
                   where moAcocuntIds.Contains(query.ID)
                   select query;
        }
        #endregion

        #region 获取承兑类型ID
        /// <summary>
        /// 获取承兑类型ID
        /// </summary>
        /// <returns></returns>
        public string[] GetMoAccountTypeID()
        {
            //银行承兑账户、商业承兑账户Is
            return new[] { "AccType004", "AccType005" };
        }
        #endregion

        #region 是否为承兑账户
        /// <summary>
        /// 是否为承兑账户
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool IsAcceptanceAccount(string accountId)
        {
            return GetMoneyOrderAccounts().Any(item => item.ID == accountId);
        }
        #endregion
    }
}
