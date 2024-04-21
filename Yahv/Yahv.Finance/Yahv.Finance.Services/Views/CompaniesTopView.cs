using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;

namespace Yahv.Finance.Services.Views
{
    public class CompaniesTopView : UniqueView<Company, PvFinanceReponsitory>
    {
        public CompaniesTopView()
        {

        }

        public CompaniesTopView(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Company> GetIQueryable()
        {
            return new Yahv.Services.Views.CompaniesTopView<PvFinanceReponsitory>();
        }
    }
}
