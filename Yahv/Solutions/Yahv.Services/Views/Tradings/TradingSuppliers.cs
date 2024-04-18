using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingSuppliersTopView<TReponsitory> : SuppliersTopView<TReponsitory>
            where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingSuppliersTopView()
        {

        }
        public TradingSuppliersTopView(TReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Supplier> GetIQueryable()
        {
            var mapsView = from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                           where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Supplier
                           select map;
            var linq = from entity in base.GetIQueryable()
                       join map in mapsView on entity.ID equals map.EnterpriseID
                       select entity;
            return linq;
        }
    }

    /// <summary>
    /// 当前登录人管理的供应商
    /// </summary>
    /// <typeparam name="TReponsitory"></typeparam>
    public class MyTradingSuppliersTopView<TReponsitory> : SuppliersTopView<TReponsitory>
           where TReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingSuppliersTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingSuppliersTopView(TReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
        {
            this.admin = admin;
        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading && map.Type == (int)MapsType.Supplier && map.SubID == this.admin.ID
                   select map;
        }
       
    }
}
