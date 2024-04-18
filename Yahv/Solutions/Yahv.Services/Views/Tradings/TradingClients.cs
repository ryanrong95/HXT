using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingClientsTopView<TReponsitory> : ClientsAll<TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingClientsTopView()
        {

        }
        public TradingClientsTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Client
                   select map;
        }
    }

    /// <summary>
    /// 当前登录人管理的客户
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class MyTradingClientsTopView<TReponsitory> : ClientsAll<TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingClientsTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingClientsTopView(TReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
        {
            this.admin = admin;
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Client && map.SubID == this.admin.ID
                   select map;
        }

    }
}
