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
    public class BaseDocuCodesView : UniqueView<Models.BaseDocuCode, ScCustomsReponsitory>
    {
        public BaseDocuCodesView()
        {
        }

        internal BaseDocuCodesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseDocuCode> GetIQueryable()
        {
            return from baseDocuCode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDocuCode>()
                   select new Models.BaseDocuCode
                   {
                       ID = baseDocuCode.ID,
                       Code = baseDocuCode.Code,
                       Name = baseDocuCode.Name,
                   };
        }
    }
}