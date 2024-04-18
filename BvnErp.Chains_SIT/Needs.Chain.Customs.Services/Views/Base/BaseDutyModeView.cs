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
    public class BaseDutyModeView : UniqueView<Models.BaseDutyMode, ScCustomsReponsitory>
    {
        public BaseDutyModeView()
        {
        }

        internal BaseDutyModeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseDutyMode> GetIQueryable()
        {
            return from baseDutyMode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDutyMode>()
                   select new Models.BaseDutyMode
                   {
                       ID = baseDutyMode.ID,
                       Code = baseDutyMode.Code,
                       Name = baseDutyMode.Name,
                   };
        }
    }
}