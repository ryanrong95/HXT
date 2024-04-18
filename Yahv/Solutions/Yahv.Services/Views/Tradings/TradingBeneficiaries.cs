using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls.PvbCrm;
using Yahv.Underly;
using Yahv.Underly.Erps;

namespace Yahv.Services.Views
{
    public class TradingBeneficiariesTopView<IReponsitory> : BeneficiariesTopView<IReponsitory>
        where IReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        public TradingBeneficiariesTopView()
        {

        }
        public TradingBeneficiariesTopView(IReponsitory reponsitory) : base(reponsitory)
        {

        }
        override protected IQueryable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView> GetMapIQueryable()
        {
            return from map in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MapsBEnterTopView>()
                   where map.Bussiness == (int)Business.Trading
                        && map.Type == (int)MapsType.Beneficiary
                   select map;
        }
    }

    /// <summary>
    /// 当前登录人的受益人
    /// </summary>
    /// <typeparam name="IReponsitory"></typeparam>
    public class MyTradingBeneficiariesTopView<IReponsitory> : TradingBeneficiariesTopView<IReponsitory>
        where IReponsitory : class, Layers.Linq.IReponsitory, IDisposable, new()
    {
        IErpAdmin admin;
        public MyTradingBeneficiariesTopView(IErpAdmin admin)
        {
            this.admin = admin;
        }
        public MyTradingBeneficiariesTopView(IReponsitory reponsitory, IErpAdmin admin) : base(reponsitory)
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
