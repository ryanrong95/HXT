using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data;
using Layers.Linq;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Finance.Services.Models.Rolls;
using Yahv.Finance.Services.Views.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Rolls
{
    public class FlowAccountsRoll : QueryView<FlowAccount, PvFinanceReponsitory>
    {
        public FlowAccountsRoll()
        {
        }

        public FlowAccountsRoll(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<FlowAccount> GetIQueryable()
        {
            var flows = new FlowAccountsOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);

            return from entity in flows
                   join account in accounts on entity.AccountID equals account.ID into _account
                   from account in _account.DefaultIfEmpty()
                   join admin in admins on entity.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   select new FlowAccount()
                   {
                       Currency = entity.Currency,
                       ID = entity.ID,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       Price = entity.Price,
                       Price1 = entity.Price1,
                       Currency1 = entity.Currency1,
                       ERate1 = entity.ERate1,
                       TargetAccountCode = entity.TargetAccountCode,
                       TargetAccountName = entity.TargetAccountName,
                       PaymentDate = entity.PaymentDate,
                       PaymentMethord = entity.PaymentMethord,
                       FormCode = entity.FormCode,
                       AccountMethord = entity.AccountMethord,
                       AccountID = entity.AccountID,
                       Balance1 = entity.Balance1,
                       Balance = entity.Balance,
                       AccountCode = account.Code,
                       SourceName = entity.SourceName,
                       CreatorName = admin.RealName,
                       TargetRate = entity.TargetRate,
                       MoneyOrderID = entity.MoneyOrderID,
                       Type = entity.Type,
                   };
        }

        public IQueryable<FlowAccountVoucher> GetVouchersOrigin()
        {
            var flows = new FlowAccountsOrigin(this.Reponsitory);
            var accounts = new AccountsOrigin(this.Reponsitory);
            var admins = new AdminsTopView(this.Reponsitory);
            var enterprises = new EnterprisesOrigin(this.Reponsitory);

            return from entity in flows
                   join account in accounts on entity.AccountID equals account.ID into _account
                   from account in _account.DefaultIfEmpty()
                   join admin in admins on entity.CreatorID equals admin.ID into _admin
                   from admin in _admin.DefaultIfEmpty()
                   join enterprise in enterprises on account.EnterpriseID equals enterprise.ID into _enterprise
                   from enterprise in _enterprise.DefaultIfEmpty()
                   select new FlowAccountVoucher()
                   {
                       Currency = entity.Currency,
                       ID = entity.ID,
                       AccountMethord = entity.AccountMethord,
                       Balance = entity.Balance,
                       CompanyName = enterprise != null ? enterprise.Name : string.Empty,
                       CreateDate = entity.CreateDate,
                       Creator = admin.RealName,
                       LeftPrice = entity.Price >= 0 ? entity.Price : 0,
                       RightPrice = entity.Price < 0 ? entity.Price : 0,
                       AccountID = entity.AccountID,
                       Balance1 = entity.Balance1,
                       LeftPrice1 = entity.Price1 >= 0 ? entity.Price1 : 0,
                       Rate = entity.ERate1,
                       RightPrice1 = entity.Price1 < 0 ? entity.Price1 : 0,
                   };
        }

        /// <summary>
        /// 删除流水
        /// </summary>
        /// <param name="id">流水ID</param>
        public void Abandon(string id)
        {
            new FlowAccount() { ID = id }.Abandon();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        public void Abandon(params string[] ids)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                if (ids.Length > 0)
                {
                    reponsitory.Delete<Layers.Data.Sqls.PvFinance.FlowAccounts>(item => ids.Contains(item.ID));
                }
            }
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="flowId"></param>
        /// <returns></returns>
        public FlowAccount this[string flowId]
        {
            get { return this.Single(item => item.ID == flowId); }
        }

        /// <summary>
        /// 批量添加流水
        /// </summary>
        /// <param name="flows"></param>
        public void AddRange(IEnumerable<FlowAccount> flows)
        {
            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                var data = flows.Select(item => new Layers.Data.Sqls.PvFinance.FlowAccounts()
                {
                    ID = item.ID ?? PKeySigner.Pick(PKeyType.FlowAcc),
                    AccountMethord = (int)item.AccountMethord,
                    AccountID = item.AccountID,
                    Currency = (int)item.Currency,
                    Price = item.Price,
                    Balance = item.Balance,
                    PaymentDate = item.PaymentDate,
                    FormCode = item.FormCode,
                    Currency1 = (int)item.Currency1,
                    ERate1 = item.ERate1,
                    Price1 = item.Price1,
                    Balance1 = item.Balance1,
                    CreatorID = item.CreatorID,
                    CreateDate = DateTime.Now,
                    TargetAccountName = item.TargetAccountName,
                    TargetAccountCode = item.TargetAccountCode,
                    PaymentMethord = (int)item.PaymentMethord,
                    Type = (int)item.Type
                });

                reponsitory.Insert(data);
            }
        }

        /// <summary>
        /// 重新计算余额（id以后的余额重新计算）
        /// </summary>
        /// <param name="accountId">账户ID</param>
        /// <param name="flowId">流水ID</param>
        public void Recalculation(string accountId, string flowId)
        {
            if (string.IsNullOrEmpty(accountId) || string.IsNullOrEmpty(flowId))
            {
                return;
            }

            using (var reponsitory = LinqFactory<PvFinanceReponsitory>.Create())
            {
                string sql = @"DECLARE @accountId VARCHAR(50) ,
                                @id VARCHAR(50)

                            SET @accountId = {0}
	                            --银行卡号

                            DECLARE id_cursor CURSOR
                            FOR
                                SELECT  f.ID
                                FROM    dbo.FlowAccounts f
                                WHERE   f.AccountID=@accountId
                                        AND f.ID >= {1};
                            OPEN id_cursor
                            FETCH NEXT FROM id_cursor INTO @id
                            WHILE @@FETCH_STATUS = 0
                                BEGIN
                                    UPDATE  dbo.FlowAccounts
                                    SET     Balance = 0
                                    WHERE   ID = @id
                                    PRINT @id
                                    FETCH NEXT FROM id_cursor INTO @id
                                END
                            CLOSE id_cursor
                            DEALLOCATE id_cursor";

                reponsitory.Query<int>(sql, accountId, flowId);
            }
        }
    }
}
