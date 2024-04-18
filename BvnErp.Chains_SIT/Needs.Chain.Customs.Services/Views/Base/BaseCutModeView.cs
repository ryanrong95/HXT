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
    public class BaseCutModeView : UniqueView<Models.BaseCutMode, ScCustomsReponsitory>
    {
        public BaseCutModeView()
        {
        }

        internal BaseCutModeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseCutMode> GetIQueryable()
        {
            return from cutMode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCutMode>()
                   select new Models.BaseCutMode
                   {
                       ID = cutMode.ID,
                       Code = cutMode.Code,
                       Name = cutMode.Name,
                       Detail=cutMode.Detail
                   };
        }
    }
}