using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 包装种类
    /// </summary>
    public class BaseWrapTypesView : UniqueView<Models.WrapType, ScCustomsReponsitory>
    {
        public BaseWrapTypesView()
        {
        }

        internal BaseWrapTypesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.WrapType> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseWrapTypes>()
                   select new Models.WrapType
                   {
                       ID = entity.ID,
                       Code = entity.Code,
                       Name = entity.Name
                   };
        }
    }
}