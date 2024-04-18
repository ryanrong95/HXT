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
    public class BaseCustomMasterView : UniqueView<Models.BaseCustomMaster, ScCustomsReponsitory>
    {
        public BaseCustomMasterView()
        {
        }

        internal BaseCustomMasterView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseCustomMaster> GetIQueryable()
        {
            return from customMaster in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseCustomMaster>()
                   select new Models.BaseCustomMaster
                   {
                       ID = customMaster.ID,
                       Code = customMaster.Code,
                       Name = customMaster.Name,
                   };
        }
    }
}