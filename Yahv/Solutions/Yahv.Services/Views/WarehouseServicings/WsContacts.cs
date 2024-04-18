using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Layers.Data.Sqls.PvbCrm;

namespace Yahv.Services.Views
{
    /// <summary>
    /// 当仓储联系人
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class WsContactsTopView<TReponsitory> : ContactsTopView<TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsContactsTopView()
        {

        }
        public WsContactsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)MapsType.Contact
                   select map;
        }
    
    }
}
