using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseEntryPortsView : UniqueView<Models.BaseEntryPort, ScCustomsReponsitory>
    {
        public BaseEntryPortsView()
        {
        }

        internal BaseEntryPortsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseEntryPort> GetIQueryable()
        {
            return from EntryPort in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseEntryPort>()
                   select new Models.BaseEntryPort
                   {
                       ID = EntryPort.ID,
                       Code = EntryPort.Code,
                       Name = EntryPort.Name,
                       RomanName = EntryPort.RomanName
                   };
        }
    }
}