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
    public class BaseDistrictCodeView : UniqueView<Models.BaseDistrictCode, ScCustomsReponsitory>
    {
        public BaseDistrictCodeView()
        {
        }

        internal BaseDistrictCodeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<BaseDistrictCode> GetIQueryable()
        {
            return from baseDistrictCode in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDistrictCode>()
                   select new Models.BaseDistrictCode
                   {
                       ID = baseDistrictCode.ID,
                       Code = baseDistrictCode.Code,
                       Name = baseDistrictCode.Name,
                   };
        }
    }
}