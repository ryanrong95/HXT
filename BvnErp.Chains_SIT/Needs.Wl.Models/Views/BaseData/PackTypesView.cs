using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class PackTypesView : View<Models.BasePackType, ScCustomsReponsitory>
    {
        public PackTypesView()
        {
        }

        internal PackTypesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.BasePackType> GetIQueryable()
        {
            return from basePackType in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BasePackType>()
                   orderby basePackType.Code
                   select new Models.BasePackType
                   {
                       ID = basePackType.ID,
                       Code = basePackType.Code,
                       Name = basePackType.Name
                   };
        }

        public BasePackType FindByCode(string code)
        {
            return this.GetIQueryable().Where(s => s.Code == code).FirstOrDefault();
        }
    }
}
