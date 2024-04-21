using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models;
using Yahv.Underly;

namespace Yahv.Payments.Views
{
    /// <summary>
    /// 银行流水
    /// </summary>
    public class BankFlowAccountsView : QueryView<BankFlowAccount, PvbCrmReponsitory>
    {
        public BankFlowAccountsView()
        {

        }

        public BankFlowAccountsView(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        public BankFlowAccount this[string formCode]
        {
            get { return this.Single(item => item.FormCode == formCode); }
        }

        protected override IQueryable<BankFlowAccount> GetIQueryable()
        {
            int type = (int)AccountType.BankStatement;

            var flowAccounts = Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.FlowAccounts>()
                .Where(item => item.Type == type);

            var linq = from f in flowAccounts
                       group f by new { f.Payer, f.Payee, f.Currency, f.FormCode, f.Account } into g
                       select new BankFlowAccount()
                       {
                           Payee = g.Key.Payee,
                           Payer = g.Key.Payer,
                           Currency = (Currency)g.Key.Currency,
                           FormCode = g.Key.FormCode,
                           Account = g.Key.Account,
                           Price = g.Sum(item => item.Price),
                           //FlowAccountID = g.FirstOrDefault(item => item.FormCode == g.Key.FormCode && item.Price > 0).ID,
                           FlowAccountID = "FlowAccountID"
                       };

            return linq;
        }
    }
}
