using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BalanceValueView : UniqueView<BalanceValueViewModel, ScCustomsReponsitory>
    {
        protected override IQueryable<BalanceValueViewModel> GetIQueryable()
        {
            var linq = from clientBalance in new Needs.Ccs.Services.Views.ClientBalanceViewOrigin(this.Reponsitory)
                       join client in new Needs.Ccs.Services.Views.Origins.ClientsOrigin(this.Reponsitory) on clientBalance.ClientID equals client.ID
                       where clientBalance.Status == Needs.Ccs.Services.Enums.Status.Normal
                          && client.Status == Needs.Ccs.Services.Enums.Status.Normal
                          && clientBalance.Currency == "USD"
                          && client.ClientCode == "XL002"
                       select new BalanceValueViewModel
                       {
                           Balance = clientBalance.Balance,
                       };

            return linq;
        }
    }

    public class BalanceValueViewModel : IUnique
    {
        public string ID { get; set; }

        public decimal? Balance { get; set; }
    }

}
