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
    public class BaseOrgCodesView : UniqueView<Models.BaseOrgCode, ScCustomsReponsitory>
    {
        public BaseOrgCodesView()
        {
        }

        internal BaseOrgCodesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseOrgCode> GetIQueryable()
        {
            return from baseOrgCode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseOrgCode>()
                   select new Models.BaseOrgCode
                   {
                       ID = baseOrgCode.ID,
                       Code = baseOrgCode.Code,
                       Name = baseOrgCode.Name,
                       Detail = baseOrgCode.Detail
                   };
        }
    }
}