using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BaseDestCodeView : UniqueView<Models.BaseDestCode, ScCustomsReponsitory>
    {
        public BaseDestCodeView()
        {
        }

        internal BaseDestCodeView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BaseDestCode> GetIQueryable()
        {
            return from port in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseDestCode>()
                   select new Models.BaseDestCode
                   {
                       ID = port.ID,
                       Code = port.Code,
                       Name = port.Name,                       
                   };
        }
    }
}
