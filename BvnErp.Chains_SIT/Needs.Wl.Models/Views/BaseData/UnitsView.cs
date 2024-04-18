using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 单位
    /// 默认不分页
    /// </summary>
    public class UnitsView : View<Models.Unit, ScCustomsReponsitory>
    {
        public UnitsView()
        {
            this.AllowPaging = false;
        }

        internal UnitsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Unit> GetIQueryable()
        {
            return from unit in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.BaseUnits>()
                   select new Models.Unit
                   {
                       ID = unit.ID,
                       Code = unit.Code,
                       Name = unit.Name,
                   };
        }

        public Unit FindByCode(string code)
        {
            return this.GetIQueryable().Where(s => s.Code == code).FirstOrDefault();
        }
    }
}
