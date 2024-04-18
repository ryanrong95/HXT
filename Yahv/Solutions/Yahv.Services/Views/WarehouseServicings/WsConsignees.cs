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
    /// 代仓储业务客户收件地址
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class WsConsigneesTopView<TReponsitory> : ConsigneesTopView<TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsConsigneesTopView()
        {

        }
        public WsConsigneesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)MapsType.Consignee
                   select map;
        }
    }
}
