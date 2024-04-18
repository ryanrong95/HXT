using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Underly;

namespace Yahv.Services.Views
{

    /// <summary>
    /// 已被wsnSupplierConsignorsTopView代替
    /// 代仓储供应商提货地址
    /// </summary>
    [Obsolete]
    public class WsConsignors<TReponsitory> : ConsignorsTopView<TReponsitory>
         where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsConsignors()
        {

        }
        public WsConsignors(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)MapsType.Consignor
                   select map;
        }

    }
}
