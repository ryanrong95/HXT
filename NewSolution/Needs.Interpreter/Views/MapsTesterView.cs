using Layer.Data.Sqls;
using Needs.Interpreter.Model;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Interpreter.Domain
{
    [Obsolete("理论上永不上，仅仅作为示例使用。类似这样的应该直接进入视图")]
    public class MapsTesterView : QueryView<MapTester, BvTesterReponsitory>
    {
        public MapsTesterView()
        {
        }

        protected MapsTesterView(BvTesterReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<MapTester> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvTester.MapsTester>()
                   select new MapTester
                   {
                       MainID = entity.MainID,
                       SubID = entity.SubID
                   };
        }

    }
}
