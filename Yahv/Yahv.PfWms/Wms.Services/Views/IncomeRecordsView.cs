using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;

namespace Wms.Services.Views
{
    /// <summary>
    /// 收入记录
    /// </summary>
    public class IncomeRecordsView : QueryView<Object, PvWmsRepository>
    {
        protected override IQueryable<object> GetIQueryable()
        {
            throw new NotImplementedException();
        }
    }
}
