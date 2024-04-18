using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class BasePackTypesView : UniqueView<Models.BasePackType, ScCustomsReponsitory>
    {
        public BasePackTypesView()
        {
        }

        internal BasePackTypesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BasePackType> GetIQueryable()
        {
            return from basePackType in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePackType>()
                   select new Models.BasePackType
                   {
                       ID = basePackType.ID,
                       Code = basePackType.Code,
                       Name = basePackType.Name
                   };
        }
    }
}