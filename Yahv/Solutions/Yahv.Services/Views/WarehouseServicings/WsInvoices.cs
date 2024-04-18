using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Converters.Contents;

namespace Yahv.Services.Views
{
    public class WsInvoicesTopView<TReponsitory> : InvoicesTopView<TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public WsInvoicesTopView()
        {

        }
        public WsInvoicesTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.WarehouseServicing && map.Type == (int)MapsType.Invoice
                   select map;
        }

    }

   

    
}
