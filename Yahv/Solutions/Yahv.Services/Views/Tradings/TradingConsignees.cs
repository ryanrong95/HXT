using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingConsigneesTopView<TReponsitory> : ConsigneesTopView<TReponsitory>
        where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingConsigneesTopView()
        {

        }
        public TradingConsigneesTopView(TReponsitory reponsitory) : base(reponsitory)
        {

        }

        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Consignee
                   select map;
        }
    }
    /// <summary>
    /// 当前登录人管理的到货地址
    /// </summary>
    /// <typeparam name="IReponsitory"></typeparam>
    public class MyTradingConsigneesTopView<IReponsitory> : ConsigneesTopView<IReponsitory>
        where IReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingConsigneesTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingConsigneesTopView(IReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
        {
            this.admin = admin;
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Consignee && map.CtreatorID == this.admin.ID
                   select map;
        }
    }
}
